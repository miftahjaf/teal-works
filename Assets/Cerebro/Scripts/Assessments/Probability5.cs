using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class Probability5 : BaseAssessment {

		private string[] CardSuit = new string[] {"spade", "club", "hearts", "diamond"};
		private string[] nonNumberedCards = new string[] {"ace", "jack", "queen", "king"};
		private string Answer;
		private string[] expressions;
		private int[] numbers;
		private string word;
		private char letter;
		private char[] charArray;
		private int randSelector, randSelector1;
		private int numProb, denProb, hcf;

		public Text subQuestionText;
		public DiagramHelper diagramHelper;
		public GameObject MCQ;
		public GameObject ThreeChoice;
		public GameObject[] FractionNumber;

		private bool IsFractionEnable;
		private string[] AnswerArray;

		void Start () {

			base.Initialise ("M", "PRB05", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[4];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			GenerateQuestion ();
		}

		public override void SubmitClick(){
			if (ignoreTouches || userAnswerText.text == "") {
				return;
			}
			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = true;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (IsFractionEnable) {
				string[] UserAns = new string[2];
				UserAns [0] = FractionNumber [0].transform.FindChild ("Text").GetComponent<Text> ().text;
				UserAns [1] = FractionNumber [1].transform.FindChild ("Text").GetComponent<Text> ().text;

				if (AnswerArray [0] == "0" && UserAns [1] != "0")
					UserAns [1] = AnswerArray [1];

				if (AnswerArray [1] == "1" && UserAns [1] == "")
					UserAns [1] = "1";

				if (MathFunctions.checkFractions (UserAns, AnswerArray)) {
					correct = true;
				} else {
					correct = false;
				}
			} else if (MCQ.activeSelf || ThreeChoice.activeSelf) 
			{
				bool checkingMCQ = false;
				bool checkingThreeChoice = false;

				if (MCQ.activeSelf)
				{
					checkingMCQ = true;
				}

				if (ThreeChoice.activeSelf)
				{
					checkingThreeChoice = true;
				}

				if (checkingMCQ)
				{
					if (Answer == userAnswerText.text) {
						correct = true;
					} else {
						correct = false;
						AnimateMCQOptionCorrect (Answer);
					}
				}
				else if (checkingThreeChoice)
				{
					if (userAnswerText.text == Answer)
					{
						correct = true;
					}
					else
					{
						correct = false;
						AnimateThreeChoiceOptionCorrect(Answer);
					}
				}
			}
			if (correct == true) {
				if (Queslevel == 1) {
					increment = 5;
				} else if (Queslevel == 2) {
					increment = 5;
				} else if (Queslevel == 3) {
					increment = 10;
				} else if (Queslevel == 4) {
					increment = 15;
				} else if (Queslevel == 5) {
					increment = 15;
				}

				UpdateStreak(8,12);

				updateQuestionsAttempted ();
				StartCoroutine (ShowCorrectAnimation());
			} 
			else {
				for (var i = 0; i < scorestreaklvls.Length; i++) {
					scorestreaklvls [i] = 0;
				}
				StartCoroutine (ShowWrongAnimation());
			}

			base.QuestionEnded (correct, level, increment, selector);

		}

		public void ThreeChoiceOptionClicked(int value)
		{
			if (ignoreTouches)
			{
				return;
			}
			AnimateThreeChoiceOption(value);
			userAnswerText = ThreeChoice.transform.Find("Option" + value.ToString()).Find("Text").GetComponent<Text>();
			answerButton = ThreeChoice.transform.Find("Option" + value.ToString()).GetComponent<Button>();
			SubmitClick();
		}
		IEnumerator AnimateThreeChoiceOption(int value)
		{
			var GO = ThreeChoice.transform.Find("Option" + value.ToString()).gameObject;
			Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds(0.2f);
			Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1, 1, 1), false));
		}
		void AnimateThreeChoiceOptionCorrect(string ans)
		{
			if (isRevisitedQuestion) {
				return;
			}
			int index = -1;
			for (int i = 1; i <= 3; i++) {
				if (ThreeChoice.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().text == ans) {
					//index = i;
					ThreeChoice.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.green800;
				}
			}
			if (index != -1) {
				var GO = ThreeChoice.transform.Find ("Option" + index.ToString ()).gameObject;
				Go.to (ThreeChoice.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			}
		}
		void AnimateMCQOptionCorrect(string ans)
		{
			if (isRevisitedQuestion) {
				return;
			}
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.green800;
				}
			}
		}
		public void MCQOptionClicked (int value)
		{
			if (ignoreTouches) {
				return;
			}
			AnimateMCQOption (value);
			userAnswerText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<Text> ();
			answerButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
			SubmitClick ();
		}

		IEnumerator AnimateMCQOption (int value)
		{
			var GO = MCQ.transform.Find ("Option" + value.ToString ()).gameObject;
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
		}

		protected override IEnumerator ShowWrongAnimation() {
			if (IsFractionEnable) {
				for (int i = 0; i < 2; i++) {
					FractionNumber[i].transform.FindChild("Text").GetComponent<Text>().color = MaterialColor.red800;
					FractionNumber[i].transform.FindChild("border").gameObject.SetActive(false);
					Go.to (FractionNumber[i].transform.FindChild("Text").gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
				}
			} else {
				userAnswerText.color = MaterialColor.red800;
				Go.to (userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			}
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				if (numPad.activeSelf) {
					if (IsFractionEnable) {
						for (int i = 0; i < 2; i++) {
							FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().text = "";
						}
					} else {
						userAnswerText.text = "";
					}
				}
				if (userAnswerText != null) {

					userAnswerText.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
				userAnswerText.text = "";
			} else {
				if (numPad.activeSelf) {
					if (IsFractionEnable) {
						for (int i = 0; i < 2; i++) {
							FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().text = AnswerArray[i];
							FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().color = MaterialColor.green800;
						}
					} else {
						userAnswerText.text = Answer.ToString ();
						userAnswerText.color = MaterialColor.green800;
					}
				} else {

					userAnswerText.color = MaterialColor.textDark;
				}
			}
			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			if (IsFractionEnable) {
				for (int i = 0; i < 2; i++) {
					FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().color = MaterialColor.green800;
					var config = new GoTweenConfig ()
						.scale (new Vector3 (1.1f, 1.1f, 1f))
						.setIterations (2, GoLoopType.PingPong);
					var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1));
					var tween = new GoTween (FractionNumber [i].transform.FindChild ("Text").gameObject.transform, 0.2f, config);
					flow.insert (0f, tween);
					flow.play ();
				}
				yield return new WaitForSeconds (1f);
				for (int i = 0; i < 2; i++) {
					FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
				}
			} else {
				userAnswerText.color = MaterialColor.green800;
				var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations (2, GoLoopType.PingPong);
				var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1)); 
				var tween = new GoTween (userAnswerText.gameObject.transform, 0.2f, config);
				flow.insert (0f, tween);
				flow.play ();
				yield return new WaitForSeconds (1f);
				userAnswerText.color = MaterialColor.textDark;
			}

			showNextQuestion ();

			if (levelUp) {
				StartCoroutine (HideAnimation ());
				base.LevelUp ();
				yield return new WaitForSeconds (1.5f);
				StartCoroutine (StartAnimation ());
			}

		}
		public void SetFocusFractionNumber(int no)
		{
			for (int i = 0; i < 2; i++) {
				FractionNumber[i].transform.FindChild("border").gameObject.SetActive(false);
			}
			FractionNumber[no].transform.FindChild("border").gameObject.SetActive(true);
			userAnswerText = FractionNumber [no].transform.FindChild ("Text").GetComponent<Text> ();
			userAnswerText.text = "";
		}
		void RandomizeMCQOptionsAndFill(List<string> options)
		{
			int index = 0;
			int cnt = options.Count;
			for (int i = 1; i <= cnt; i++) {
				index = Random.Range (0, options.Count);
				MCQ.transform.Find ("Option"+i).Find ("Text").GetComponent<Text> ().text = options [index];
				options.RemoveAt (index);
			}
		}

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			List<string> options = new List<string>();

			level = Queslevel;
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			GeneralButton.gameObject.SetActive (false);
			subQuestionText.gameObject.SetActive (false);
			numPad.SetActive (true);
			MCQ.SetActive (false);
			ThreeChoice.gameObject.SetActive(false);
			diagramHelper.Reset ();
			FractionNumber [0].transform.parent.gameObject.SetActive (true);
			SetFocusFractionNumber(0);
			IsFractionEnable = true;

			for (int i = 0; i < 2; i++) {
				FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().text = "";
				FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			for (int i = 1; i < 4; i++) {
				ThreeChoice.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			answerButton = null;

			AnswerArray = new string[2];

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector <= 2) 
				{
					expressions = new string[] {"TWEEZERS", "DISPLAY", "ELECTROCUTE", "PROBABILITY", "FLOCK", "MATHEMATICS", "EXPRESS", "INTERNATIONAL", "CRICKET", "FOOTBALL", "TENNIS", "AHMEDABAD"};

					randSelector = Random.Range (0, expressions.Length); 
					word = expressions [randSelector];

					randSelector = Random.Range (0, word.Length); 
					letter = word [randSelector]; 
	
					denProb = word.Length;
					numProb = 0;

					foreach (char ch in word)
					{
						if (ch == letter)
							numProb ++;
					}

					QuestionText.text = "If one letter is chosen at random from the word " + word + ", what is the probability that the letter chosen is the letter " + letter + "?";

				}
				else if (selector == 3)
				{
					float initialAngle = Random.Range (-90, 90);
					int numberOfDivisions = Random.Range (4, 9);
					float InBetweenAngle = 360f / numberOfDivisions;
					randSelector = Random.Range (0, numberOfDivisions);
					charArray = new char[numberOfDivisions];

					for (int i = 0; i < numberOfDivisions; i++)
					{
						charArray[i] = (char) (65 + Random.Range (0, numberOfDivisions));
					}

					letter = charArray[randSelector];
					denProb = numberOfDivisions;
					numProb = 0;

					for (int i = 1; i <= numberOfDivisions; i++)
					{
						if (letter == charArray [i - 1])
							numProb ++;

						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, initialAngle + i * InBetweenAngle, false, 60f, 1));
						diagramHelper.AddAngleArc (new AngleArc ("" + charArray [i - 1], Vector2.zero, initialAngle + (i - 1) * InBetweenAngle, initialAngle + i * InBetweenAngle, 120, true, false));
					}
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, initialAngle + (2f * Random.Range (1, numberOfDivisions) + 1) * InBetweenAngle / 2f, true, 20f, 1));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (250, -20));

					QuestionText.text = "Find the probability of spinning the letter " + letter + ".";
						
				}
				else if (selector == 4) 
				{
					numbers = new int[6];
					randSelector = Random.Range (0, 6);

					for (int i = 0; i < 6; i++)
						numbers[i] = Random.Range (1, 10);

					denProb = numbers.Length;
					numProb = 0;

					for (int i = 0; i < 6; i++)
					{
						if (numbers[i] == numbers [randSelector])
							numProb ++;
					}

					QuestionText.text = "A number cube has 6 sides. The sides have the numbers ";
					for (int i = 0; i < numbers.Length - 1; i++)
						QuestionText.text += numbers[i] + ", ";
					QuestionText.text += "and " + numbers[numbers.Length - 1] + ". If the cube is thrown once, what is the probability of rolling the number " + numbers[randSelector] + "?";

				}
				else if (selector == 5) 
				{
					expressions = new string[] {"red", "green", "blue"};
					numbers = new int[expressions.Length];
					denProb = 0;

					for (int i = 0; i < expressions.Length; i++)
					{
						numbers [i] = Random.Range (3, 10);
						denProb += numbers [i];
					}

					randSelector = Random.Range (0, expressions.Length);
					numProb = numbers [randSelector];

					QuestionText.text = "What is the probability of choosing a " + expressions [randSelector] + " marble from a jar containing ";
					for (int i = 0; i < numbers.Length - 1; i++)
						QuestionText.text += numbers [i] + " " + expressions [i] + ", ";
					QuestionText.text += "and " + numbers [numbers.Length - 1] + " " + expressions [numbers.Length - 1] + " marbles?";
				}

				hcf = MathFunctions.GetHCF (numProb, denProb);
				numProb /= hcf;
				denProb /= hcf;

				AnswerArray[0] = "" + numProb;
				AnswerArray[1] = "" + denProb;
			}
			#endregion
			#region level2
			if (level == 2) 
			{
				selector = GetRandomSelector (1, 6);
				subQuestionText.gameObject.SetActive (true);
				numPad.SetActive (false);
				FractionNumber [0].transform.parent.gameObject.SetActive (false);
				IsFractionEnable = false;

				if (selector <= 3) 
				{
					ThreeChoice.gameObject.SetActive(true);
				
					expressions = new string[]{"You will go home this afternoon.",
												"Catching a cold next winter.",
												"The person sitting closest to you was born on the same day as you.",
												"A coin tossed in the air will float away.",
												"Food prices will increase in the next ten years.",
												"A six faced die will show a number less than 8 when tossed.",	
												"The moon will come crashing into the Earth in the next five minutes.",
												"A tossed coin will land on a head",
												"Both polar ice caps will melt tommorrow.",
												"A black cat will cross your path.",
												"Your pants will not fit you in three years time.",
												"Your pen will eventually run out.",
												"A double headed coin will show heads.",
												"The next apple you eat will be a green apple.",
												"In the future your calculators will do your homework for you.",
												"You will be the first person on Mars.",
												"You will roll a 6, 5 or a 4 on a die.",
												"Next time you use your computer it will crash.",
												"You randomly open the dictionary and find a word beginning with 's'.",
												"When you turn on the t.v. next, it will immediately show a commercial."};
					
					AnswerArray = new string[] {"Certain",
											    "Possible",
												"Possible",
												"Impossible",
												"Possible",
												"Certain",
												"Impossible",
												"Possible",
												"Impossible",
												"Possible",
												"Possible",
												"Certain",
												"Certain",
												"Possible",
												"Possible",
												"Possible",
												"Possible",
												"Possible",
												"Possible",
												"Possible"};

					randSelector = Random.Range (0, expressions.Length);
					Answer = AnswerArray [randSelector];
					QuestionText.text = "Pick the correct option about the given event.";
					subQuestionText.text = expressions [randSelector];

					ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "Certain";
					ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<Text>().text = "Possible";
					ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<Text>().text = "Impossible";
				}
				else if (selector >= 4)
				{
					MCQ.SetActive (true);

					expressions = new string[] {"A glass jar contains 27 yellow marbles. Describe the probability of picking a yellow marble.",
												"A glass jar contains 27 purple marbles. Describe the probability of picking a red marble.",
												"A glass jar contains a total of 26 marbles. The jar has yellow and blue marbles. There are 5 blue marbles. Describe the probability of picking a yellow marble.",
												"In a bag there are 12 black and 12 white balls. Describe the probability of picking a black ball.",
												"A deck of cards contains all cards excluding " + nonNumberedCards [Random.Range (0, nonNumberedCards.Length)] + ". Describe the probablity of picking up a number 10 card.",
												"A die is thrown. Describe the probability of getting an even number."};
					
					AnswerArray = new string[] {"Certain", "Impossible", "Likely", "Equal chance", "Unlikely", "Equal chance"};

					randSelector = Random.Range (0, expressions.Length);

					QuestionText.text = "Pick the correct option about the given event.";
					subQuestionText.text = expressions [randSelector];

					options.Add (AnswerArray [randSelector]);
					randSelector1 = Random.Range (0, 5);

					while (AnswerArray [randSelector] == AnswerArray [randSelector1])
						randSelector1 = Random.Range (0, 5);
					
					for (int i = 0; i < 5; i++)
					{						
						if (AnswerArray [i] != AnswerArray [randSelector] && AnswerArray [i] != AnswerArray [randSelector1])
							options.Add (AnswerArray[i]);
					}
						
					Answer = AnswerArray [randSelector];
					RandomizeMCQOptionsAndFill (options);
				}
			}
			#endregion
			#region level3
			if (level == 3) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector <= 2) 
				{
					numbers = new int[6];
					for (int i = 0; i < 6; i++)
						numbers[i] = Random.Range (1, 10);
					
					randSelector = Random.Range (0, 6);
					randSelector1 = Random.Range (0, 6);

					while (numbers[randSelector] == numbers[randSelector1])
						randSelector1 = Random.Range (0, 6);
					
					denProb = numbers.Length;
					numProb = 0;

					for (int i = 0; i < 6; i++)
					{
						if (numbers[i] == numbers[randSelector] || numbers[i] == numbers[randSelector1])
							numProb ++;
					}

					QuestionText.text = "A number cube has 6 sides. The sides have the numbers ";
					for (int i = 0; i < numbers.Length - 1; i++)
						QuestionText.text += numbers[i] + ", ";
					QuestionText.text += "and " + numbers[numbers.Length - 1] + ". If the cube is thrown once, what is the probability of rolling the number " + numbers[randSelector] + " or the number " + numbers[randSelector1] + "?";

				}
				else if (selector == 3) 
				{
					numbers = new int[6];
					for (int i = 0; i < 6; i++)
						numbers[i] = Random.Range (1, 10);

					randSelector = Random.Range (1,3);
					string OddOrEven = randSelector == 1 ? "odd" : "even";

					denProb = numbers.Length;
					numProb = 0;

					for (int i = 0; i < 6; i++)
					{
						if (numbers[i] % 2 != 0 && randSelector == 1)
							numProb ++;

						if (numbers[i] % 2 == 0 && randSelector == 2)
							numProb ++;
					}

					QuestionText.text = "A number cube has 6 sides. The sides have the numbers ";
					for (int i = 0; i < numbers.Length - 1; i++)
						QuestionText.text += numbers[i] + ", ";
					QuestionText.text += "and " + numbers[numbers.Length - 1] + ". If the cube is thrown once, what is the probability of rolling an " + OddOrEven + " number.";

				}
				else if (selector == 4) 
				{
					float initialAngle = Random.Range (-90, 90);
					int numberOfDivisions = Random.Range (4, 9);
					float InBetweenAngle = 360f / numberOfDivisions;
					bool allElementsSame = true;

					charArray = new char[numberOfDivisions];
					randSelector = Random.Range (0, numberOfDivisions);
					randSelector1 = Random.Range (0, numberOfDivisions);

					for (int i = 0; i < numberOfDivisions; i++)
						charArray [i] = (char) (65 + Random.Range (0, numberOfDivisions));

					//checking whether charArray has all identical elements
					for (int i = 1; i < numberOfDivisions; i++)
					{
						if (charArray [i] != charArray [0])
						{
							allElementsSame = false;
							break;
						}
					}
					//if all elements are same then change the first element
					if (allElementsSame)
					{
						while (charArray [charArray.Length - 1] == charArray [0])
							charArray [0] = (char) (65 + Random.Range (0, numberOfDivisions));
					}

					char letter1 = charArray [randSelector1];
					letter = charArray [randSelector];

					//both letters should be distinct
					while (letter == letter1)
					{
						randSelector1 = Random.Range (0, numberOfDivisions);
						letter1 = charArray [randSelector1];
					}
					
					denProb = numberOfDivisions;
					numProb = 0;
											
					for (int i = 1; i <= numberOfDivisions; i++)
					{
						if (letter == charArray [i - 1] || letter1 == charArray [i - 1])
							numProb ++;
						
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, initialAngle + i * InBetweenAngle, false, 60f, 1));
						diagramHelper.AddAngleArc (new AngleArc ("" + charArray [i - 1], Vector2.zero, initialAngle + (i - 1) * InBetweenAngle, initialAngle + i * InBetweenAngle, 120, true, false));
					}
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, initialAngle + (2f * Random.Range (1, numberOfDivisions) + 1) * InBetweenAngle / 2f, true, 20f, 1));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (250, -20));

					QuestionText.text = "Find the probability of spinning the letter " + letter + " or the letter " + letter1 + "."; 

				}
				else if (selector == 5) 
				{
					expressions = new string[] {"red", "green", "blue", "yellow"};
					numbers = new int[expressions.Length];
					denProb = 0;

					for (int i = 0; i < expressions.Length; i++)
					{
						numbers [i] = Random.Range (3, 10);
						denProb += numbers [i];
					}

					randSelector = Random.Range (0, expressions.Length);
					randSelector1 = Random.Range (0, expressions.Length);

					while (randSelector == randSelector1)
						randSelector1 = Random.Range (1, 4);
					
					numProb = numbers [randSelector] + numbers [randSelector1];

					QuestionText.text = "A bag contains ";
					for (int i = 0; i < numbers.Length - 1; i++)
						QuestionText.text += numbers [i] + " " + expressions [i] + ", ";
					QuestionText.text += "and " + numbers [numbers.Length - 1] + " " + expressions [numbers.Length - 1] + " marbles. What is the probability of pulling out a " + expressions[randSelector] + " or a " + expressions [randSelector1] + " marble?";;
				}

				hcf = MathFunctions.GetHCF (numProb, denProb);
				numProb /= hcf;
				denProb /= hcf;

				AnswerArray[0] = "" + numProb;
				AnswerArray[1] = "" + denProb;

			}
			#endregion
			#region level4
			if (level == 4) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) 
				{
					expressions = new string[] {"red", "green", "blue"};
					numbers = new int[expressions.Length];
					denProb = 0;

					for (int i = 0; i < expressions.Length; i++)
					{
						numbers [i] = Random.Range (3, 10);
						denProb += numbers [i];
					}

					randSelector = Random.Range (0, expressions.Length);
					numProb = denProb - numbers [randSelector];

					QuestionText.text = "What is the probability of not choosing a " + expressions [randSelector] + " marble from a jar containing ";
					for (int i = 0; i < numbers.Length - 1; i++)
						QuestionText.text += numbers [i] + " " + expressions [i] + ", ";
					QuestionText.text += "and " + numbers [numbers.Length - 1] + " " + expressions [numbers.Length - 1] + " marbles?";

				}
				else if (selector == 2) 
				{
					numbers = new int[6];
					randSelector = Random.Range (0, 6);

					for (int i = 0; i < 6; i++)
						numbers[i] = Random.Range (1, 10);

					denProb = numbers.Length;
					numProb = denProb;

					for (int i = 0; i < 6; i++)
					{
						if (numbers[i] == numbers [randSelector])
							numProb --;
					}

					QuestionText.text = "A number cube has 6 sides. The sides have the numbers ";
					for (int i = 0; i < numbers.Length - 1; i++)
						QuestionText.text += numbers[i] + ", ";
					QuestionText.text += "and " + numbers[numbers.Length - 1] + ". If the cube is thrown once, what is the probability of not rolling the number " + numbers[randSelector] + "?";

				}
				else if (selector == 3) 
				{
					randSelector = Random.Range (0, 4);
					randSelector1 = Random.Range (0, 4);

					if (Random.Range (1, 4) == 1)
					{
						while (randSelector == randSelector1)
							randSelector1 = Random.Range (0, 4);
						
						if (Random.Range (1, 3) == 1)
						{
							expressions = new string[] {CardSuit [randSelector], CardSuit [randSelector1]};
							numProb = 26;
							denProb = 52;
						}
						else
						{
							expressions = new string[] {nonNumberedCards [randSelector], nonNumberedCards [randSelector1]};
							numProb = 44;
							denProb = 52;
						}
					}
					else
					{
						expressions = new string[] {CardSuit [randSelector], nonNumberedCards [randSelector1]};
						numProb = 36;
						denProb = 52;
					}

					QuestionText.text = "Find the probability of not picking " + (expressions [0] == "ace" ? "an " : "a ") + expressions [0] + " or " + (expressions [1] == "ace" ? "an " : "a ") + expressions [1] + " when a card is picked at random from a deck of cards.";

				}
				else if (selector == 4) 
				{
					expressions = new string[] {"red", "green", "blue", "yellow"};
					numbers = new int[expressions.Length];
					denProb = 0;

					for (int i = 0; i < expressions.Length; i++)
					{
						numbers [i] = Random.Range (3, 10);
						denProb += numbers [i];
					}

					randSelector = Random.Range (0, expressions.Length);
					randSelector1 = Random.Range (0, expressions.Length);

					while (randSelector == randSelector1)
						randSelector1 = Random.Range (1, 4);

					numProb = denProb - (numbers [randSelector] + numbers [randSelector1]);

					QuestionText.text = "A bag contains ";
					for (int i = 0; i < numbers.Length - 1; i++)
						QuestionText.text += numbers [i] + " " + expressions [i] + ", ";
					QuestionText.text += "and " + numbers [numbers.Length - 1] + " " + expressions [numbers.Length - 1] + " marbles. What is the probability of not pulling out a " + expressions[randSelector] + " or a " + expressions [randSelector1] + " marble?";;

				}
				else if (selector == 5) 
				{
					float initialAngle = Random.Range (-90, 90);
					int numberOfDivisions = Random.Range (4, 9);
					float InBetweenAngle = 360f / numberOfDivisions;
					bool allElementsSame = true;

					charArray = new char[numberOfDivisions];
					randSelector = Random.Range (0, numberOfDivisions);
					randSelector1 = Random.Range (0, numberOfDivisions);

					for (int i = 0; i < numberOfDivisions; i++)
						charArray [i] = (char) (65 + Random.Range (0, numberOfDivisions));

					//checking whether charArray has all identical elements
					for (int i = 1; i < numberOfDivisions; i++)
					{
						if (charArray [i] != charArray [0])
						{
							allElementsSame = false;
							break;
						}
					}
					//if all elements are same then change the first element
					if (allElementsSame)
					{
						while (charArray [charArray.Length - 1] == charArray [0])
							charArray [0] = (char) (65 + Random.Range (0, numberOfDivisions));
					}

					char letter1 = charArray [randSelector1];
					letter = charArray [randSelector];

					//both letters should be distinct
					while (letter == letter1)
					{
						randSelector1 = Random.Range (0, numberOfDivisions);
						letter1 = charArray [randSelector1];
					}

					denProb = numberOfDivisions;
					numProb = denProb;

					for (int i = 1; i <= numberOfDivisions; i++)
					{
						if (letter == charArray [i - 1] || letter1 == charArray [i - 1])
							numProb --;

						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, initialAngle + i * InBetweenAngle, false, 60f, 1));
						diagramHelper.AddAngleArc (new AngleArc ("" + charArray [i - 1], Vector2.zero, initialAngle + (i - 1) * InBetweenAngle, initialAngle + i * InBetweenAngle, 120, true, false));
					}
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, initialAngle + (2f * Random.Range (1, numberOfDivisions) + 1) * InBetweenAngle / 2f, true, 20f, 1));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (250, -20));

					QuestionText.text = "Find the probability of not spinning either the letter " + letter + " or the letter " + letter1 + "."; 
				}

				hcf = MathFunctions.GetHCF (numProb, denProb);
				numProb /= hcf;
				denProb /= hcf;

				AnswerArray[0] = "" + numProb;
				AnswerArray[1] = "" + denProb;

			}
			#endregion
			CerebroHelper.DebugLog (Answer);
//			userAnswerText = GeneralButton.gameObject.GetChildByName<Text>("Text");
//			userAnswerText.text = "";
			if (!IsFractionEnable) {
				if (answerButton != null) {
					userAnswerText = answerButton.gameObject.GetChildByName<Text> ("Text");
					userAnswerText.text = "";
				}
			}
		}

		public override void numPadButtonPressed(int value) {
			if (ignoreTouches) {
				return;
			}
			if (value <= 9) {
				userAnswerText.text += value.ToString ();
			} else if (value == 10) {    //Back
				if (userAnswerText.text.Length > 0) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
			} else if (value == 11) {   // All Clear
				userAnswerText.text = "";
			} else if (value == 12) {   // '/' : Change focus to denominator
				SetFocusFractionNumber(1);
			}  
		}
	}
}		