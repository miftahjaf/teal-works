using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;


namespace Cerebro {
	public class Sets : BaseAssessment {

		public TEXDraw subQuestionText;
		public TEXDraw subQuestionText2;
		public GameObject MCQ;

		private int[] answer;
		private string Answer;
		private bool HasAnswerSet;

		public GameObject EquationButtons;
		public GameObject RangeButtons;

		private int[] numbers = new int[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "SET06", "S01", "A01");

			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			answer = new int[1];
			GenerateQuestion ();
		}
			
		public override void SubmitClick(){
			if (ignoreTouches) {
				return;
			}
			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = true;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (MCQ.activeSelf) {
				if (Answer == userAnswerText.text) {
					correct = true;
				} else {
					correct = false;
					AnimateMCQOptionCorrect (Answer);
				}
			} else if (EquationButtons.activeSelf || RangeButtons.activeSelf) {
				int ans1 = 0;
				int ans2 = 0;
				GameObject Buttons;
				if (EquationButtons.activeSelf) {
					Buttons = EquationButtons;
				} else {
					Buttons = RangeButtons;
				}
				var button1 = Buttons.transform.Find ("Button1").GetComponent<Button> ();
				var button2 = Buttons.transform.Find ("Button2").GetComponent<Button> ();
				var ans1Text = button1.gameObject.GetChildByName<Text> ("Text").text;
				var ans2Text = button2.gameObject.GetChildByName<Text> ("Text").text;
				if (int.TryParse (ans1Text, out ans1)) {
					ans1 = int.Parse (ans1Text);
				} else {
					correct = false;
				}
				if (int.TryParse (ans2Text, out ans2)) {
					ans2 = int.Parse (ans2Text);
				} else {
					correct = false;
				}
				if (correct == true && ans1 == answer [0] && ans2 == answer [1]) {
					correct = true;
				} else {
					correct = false;
				}

			} else if (HasAnswerSet) {
				correct = MathFunctions.checkSets (userAnswerText.text, Answer);
			} else {
				int userAns = 0;
				if (int.TryParse (userAnswerText.text, out userAns)) {
					userAns = int.Parse (userAnswerText.text);
				}

				if (userAns == int.Parse (Answer)) {
					correct = true;
				} else {
					correct = false;
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
					increment = 10;
				} else if (Queslevel == 5) {
					increment = 15;
				} else if (Queslevel == 6) {
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
		IEnumerator AnimateMCQOption (int value)
		{
			var GO = MCQ.transform.Find ("Option" + value.ToString ()).gameObject;
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
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

		protected override IEnumerator ShowWrongAnimation() {
			if (!(EquationButtons.activeSelf || RangeButtons.activeSelf)) {
				userAnswerText.color = MaterialColor.red800;
				Go.to (userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			} else {
				GameObject Buttons;
				if (EquationButtons.activeSelf) {
					Buttons = EquationButtons;
				} else {
					Buttons = RangeButtons;
				}
				var button1 = Buttons.transform.Find ("Button1").GetComponent<Button> ();
				var button2 = Buttons.transform.Find ("Button2").GetComponent<Button> ();
				Go.to (button1.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
				Go.to (button2.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
				button1.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.red800;
				button2.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.red800;
			}
			yield return new WaitForSeconds (0.5f);
			if (EquationButtons.activeSelf || RangeButtons.activeSelf) {
				GameObject Buttons;
				if (EquationButtons.activeSelf) {
					Buttons = EquationButtons;
				} else {
					Buttons = RangeButtons;
				}
				var button1 = Buttons.transform.Find ("Button1").GetComponent<Button> ();
				var button2 = Buttons.transform.Find ("Button2").GetComponent<Button> ();
				if (isRevisitedQuestion) {
					button1.gameObject.GetChildByName<Text> ("Text").text = "";
					button2.gameObject.GetChildByName<Text> ("Text").text = "";
					button1.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
					button2.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
					ignoreTouches = false;
				} else {
					button1.gameObject.GetChildByName<Text> ("Text").text = answer [0].ToString ();
					button2.gameObject.GetChildByName<Text> ("Text").text = answer [1].ToString ();
					button1.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.green800;
					button2.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.green800;
				}
			} else {
				if (isRevisitedQuestion) {
					if (numPad.activeSelf) {
						userAnswerText.text = "";
					} 
					if (userAnswerText != null) {
						userAnswerText.color = MaterialColor.textDark;
					}
					ignoreTouches = false;
				} else {
					if (numPad.activeSelf) {
						userAnswerText.text = Answer;
						userAnswerText.color = MaterialColor.green800;
					} else {
						userAnswerText.color = MaterialColor.textDark;
					}
				}
			}

			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			if (EquationButtons.activeSelf || RangeButtons.activeSelf) {
				GameObject Buttons;
				if (EquationButtons.activeSelf) {
					Buttons = EquationButtons;
				} else {
					Buttons = RangeButtons;
				}
				var button1 = Buttons.transform.Find ("Button1").GetComponent<Button> ();
				var button2 = Buttons.transform.Find ("Button2").GetComponent<Button> ();
				button1.gameObject.GetChildByName<Text>("Text").text = answer[0].ToString();
				button2.gameObject.GetChildByName<Text>("Text").text = answer[1].ToString();
				button1.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.green800;
				button2.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.green800;

				var config = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations( 2, GoLoopType.PingPong );
				var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
				var tween = new GoTween( button1.gameObject.transform, 0.2f, config );
				flow.insert( 0f, tween );
				flow.play ();

				var config2 = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations( 2, GoLoopType.PingPong );
				var flow2 = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
				var tween2 = new GoTween( button2.gameObject.transform, 0.2f, config2 );
				flow2.insert( 0f, tween2 );
				flow2.play ();

			} else {
				userAnswerText.color = MaterialColor.green800;

				var config = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations( 2, GoLoopType.PingPong );
				var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
				var tween = new GoTween( userAnswerText.gameObject.transform, 0.2f, config );
				flow.insert( 0f, tween );
				flow.play ();
			}

			yield return new WaitForSeconds (1f);
			if (!(EquationButtons.activeSelf || RangeButtons.activeSelf)) {
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

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			level = Queslevel;

			answerButton = GeneralButton;

			HasAnswerSet = false;
			subQuestionText.gameObject.SetActive (true);
			subQuestionText2.gameObject.SetActive (false);
			RangeButtons.SetActive (false);
			EquationButtons.SetActive (false);
			SetNumpadMode ();

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			#region level1
			if (level == 1)
			{
				selector = GetRandomSelector (1, 7);

				if (selector == 1) 
				{
					SetMCQMode (2);
					string[] questionList = new string[] {
						"NGood football players of your school.",
						"NBad football players of your school.",
						"NVariety of long grain rice.",
						"NAll the tall trees in your area.",
						"NGroup of good students in your class.",
						"NGroup of bad students in your class.",
						"NA collection of tasty foods.",
						"NAll the old employees of your school.",
						"NA collection of tasty foods.",
						string.Format ("YAll the factors of {0}.", Random.Range (2, 10) * Random.Range (2, 10)),
						string.Format ("YPeople of your community who are more than {0} years old.", Random.Range (40, 90)),
						"YTeachers of your school.",
						"YYour friends who come to school by cycle.",
						"YSwimmers of your state.",
						"YStates of India.",
						"YNeighbouring countries of India.",
						"YAll the countries of the world.",
						"YAll the cities of India."
					};

					int randSelector = Random.Range (0, questionList.Length);
					QuestionLatext.text = "Pick the correct option about the following collection of objects.";
					subQuestionText.text = questionList[randSelector].Substring (1);
					Answer = questionList[randSelector][0] == 'Y'? "Is a set.": "Is not a set.";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Is a set.";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "Is not a set.";
				} 
				else if (selector == 2) 
				{
					HasAnswerSet = true;
					int randNum = Random.Range (5, 20);
					int randNum1 = Random.Range (randNum + 1, 2 * randNum);
					string[] questionList = new string[] {
						string.Format ("Set of prime numbers less than {0}.", randNum),
						string.Format ("Set of prime numbers between than {0} and {1} inclusive.", randNum, randNum1),
						string.Format ("Set of odd numbers between {0} and {1} inclusive.", randNum, randNum1),
						string.Format ("Set of even numbers between {0} and {1} inclusive.", randNum, randNum1)
					};
					int randSelector = Random.Range (0, questionList.Length);
					QuestionLatext.text = "Write the set by listing its elements.";
					subQuestionText.text = questionList[randSelector];

					if (randSelector == 0)
					{
						answer = MathFunctions.GetPrimes (randNum).ToArray ();
					}
					else if (randSelector == 1)
					{
						answer = MathFunctions.GetPrimes (randNum, randNum1).ToArray ();
					}
					else if (randSelector == 2)
					{
						List<int> answerList = new List<int>();
						for (int i = randNum; i <= randNum1; i++){
							if (i % 2 != 0) {
								answerList.Add (i);
							}
						}
						answer = answerList.ToArray ();
					}
					else if (randSelector == 3)
					{
						List<int> answerList = new List<int>();
						for (int i = randNum; i <= randNum1; i++){
							if (i % 2 == 0) {
								answerList.Add (i);
							}
						}
						answer = answerList.ToArray ();
					}
					Answer = MathFunctions.getArrayAsSet (answer, false, true);
				}
				else if (selector == 3 || selector == 4) 
				{
					HasAnswerSet = true;
					QuestionLatext.text = "Express in Roster form.";

					var num1 = Random.Range (1, 10);    //a
					var num2 = Random.Range (1, 10);	//b
					var num3 = Random.Range (1, 10);	//c
					var numberSelector = Random.Range (1, 4);  // W, N, Z
					string numberType = "";
					var lowerLimit = 0;
					var upperLimit = 0;
					var showLowerLimit = false;
					List<int> subNumbers = new List<int> ();
					if (numberSelector == 1) {
						numberType = "W";
						lowerLimit = 0;
						if (Random.Range (0, 2) == 0) {					// whether to give lower limit or not
							lowerLimit = Random.Range (0, 5);
							showLowerLimit = true;
						}
						upperLimit = Random.Range (lowerLimit + 1, lowerLimit + 4);
					} else if (numberSelector == 2) {
						numberType = "N";
						lowerLimit = 1;
						if (Random.Range (0, 2) == 0) {					// whether to give lower limit or not
							lowerLimit = Random.Range (1, 5);
							showLowerLimit = true;
						}
						upperLimit = Random.Range (lowerLimit + 1, lowerLimit + 4);
					} else if (numberSelector == 3) {
						numberType = "Z";
						showLowerLimit = true;

						lowerLimit = Random.Range (-4, 0);
						upperLimit = Random.Range (0, 4);
					}
					for (var i = lowerLimit; i <= upperLimit; i++) {
						subNumbers.Add (i);
					}

					if (selector == 3) {
						//ax + b
						answer = getRosterForm (new int[]{ num1, num2 }, subNumbers.ToArray ());
						if (showLowerLimit) {
							subQuestionText.text = "\\lbrace" + MathFunctions.AlgebraicDisplayForm (num1, "\\xalgebra", true) + " + " + num2 + "| \\xalgebra \\in " + numberType + ", " + lowerLimit + " \\leq \\xalgebra \\leq " + upperLimit + "\\rbrace";
						} else {
							subQuestionText.text = "\\lbrace" + MathFunctions.AlgebraicDisplayForm (num1, "\\xalgebra", true) + " + " + num2 + "| \\xalgebra \\in " + numberType + ", \\xalgebra \\leq " + upperLimit + "\\rbrace";
						}
					} else if (selector == 4) {
						//ax2 + bx + c
						answer = getRosterForm (new int[]{ num1, num2, num3 }, subNumbers.ToArray ());
						if (showLowerLimit) {
							subQuestionText.text = "\\lbrace" + MathFunctions.AlgebraicDisplayForm (num1, "\\xalgebra^2", true) + MathFunctions.AlgebraicDisplayForm (num2, "\\xalgebra") + " + " + num3 + " | \\xalgebra \\in " + numberType + ", " + lowerLimit + " \\leq \\xalgebra \\leq " + upperLimit + "\\rbrace";
						} else {
							subQuestionText.text = "\\lbrace" + MathFunctions.AlgebraicDisplayForm (num1, "\\xalgebra^2", true) + MathFunctions.AlgebraicDisplayForm (num2, "\\xalgebra") + " + " + num3 + " | \\xalgebra \\in " + numberType + ", \\xalgebra \\leq " + upperLimit + "\\rbrace";
						}
					}
					Answer = MathFunctions.getArrayAsSet (answer, false, true);
				}
				else if (selector == 5 || selector == 6) 
				{			
					QuestionLatext.text = "Express in set builder form.";
					subQuestionText2.gameObject.SetActive (true);

					var num1 = Random.Range (1, 10);    //a
					var num2 = Random.Range (1, 10);	//b
					var numberSelector = Random.Range (1, 4);  // W, N, Z
					string numberType = "";
					var lowerLimit = 0;
					var upperLimit = 0;
					List<int> subNumbers = new List<int> ();
					if (numberSelector == 1) {
						numberType = "W";
						lowerLimit = 0;
						if (Random.Range (0, 2) == 0) {					// whether to give lower limit or not
							lowerLimit = Random.Range (0, 5);
						}
						upperLimit = Random.Range (lowerLimit, lowerLimit + 4);
					} else if (numberSelector == 2) {
						numberType = "N";
						lowerLimit = 1;
						if (Random.Range (0, 2) == 0) {					// whether to give lower limit or not
							lowerLimit = Random.Range (1, 5);
						}
						upperLimit = Random.Range (lowerLimit, lowerLimit + 4);
					} else if (numberSelector == 3) {
						numberType = "Z";
						lowerLimit = Random.Range (0, 4);
						lowerLimit = -lowerLimit;
						upperLimit = Random.Range (0, 4);
					}
					for (var i = lowerLimit; i <= upperLimit; i++) {
						subNumbers.Add (i);
					}

					if (selector == 5) 
					{
						//ax + b
						RangeButtons.SetActive (false);
						EquationButtons.SetActive (true);
						GeneralButton.gameObject.SetActive (false);

						answerButton = EquationButtons.transform.Find ("Button1").GetComponent<Button> ();
						EquationButtons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").text = "";
						EquationButtons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
						EquationButtons.transform.Find ("Button1").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
						InputButtonClicked (1);

						answer = new int[] {num1, num2};
						var question = getRosterForm (new int[]{ num1, num2 }, subNumbers.ToArray ());
						subQuestionText.text = MathFunctions.getArrayAsSet (question, true);
						subQuestionText2.text = "\\lbrace{? | \\xalgebra \\in " + numberType + ", " + lowerLimit + " \\leq \\xalgebra \\leq " + upperLimit + "}\\rbrace";
					} 
					else if (selector == 6) 
					{
						//ranges
						RangeButtons.SetActive (true);
						EquationButtons.SetActive (false);
						GeneralButton.gameObject.SetActive (false);
						answerButton = RangeButtons.transform.Find ("Button1").GetComponent<Button> ();
						RangeButtons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").text = "";
						RangeButtons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
						RangeButtons.transform.Find ("Button1").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
						InputButtonClicked (1);

						answer = new int[] {lowerLimit, upperLimit};
						var question = getRosterForm (new int[]{ num1, num2 }, subNumbers.ToArray ());
						subQuestionText.text = MathFunctions.getArrayAsSet (question, true);
						subQuestionText2.text = "\\lbrace" + MathFunctions.AlgebraicDisplayForm (num1, "\\xalgebra", true) + " + " + num2 + "| \\xalgebra \\in " + numberType + ", ? \\leq \\xalgebra \\leq ?\\rbrace";
					}
				} 
			}
			#endregion
			#region level2
			else if (level == 2)
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) 
				{
					SetMCQMode (2);

					List<string> questionList = new List<string> () {
						string.Format ("ISet of all points on a line segment of length {0} cm.", Random.Range (1, 10)),
						"FSet of names of capitals of all the countries.",
						"FSet of all citizens of India.",
						"ISet of all natural numbers.",
						"FThe set of your classmates.",
						"IThe set of stars in the sky.",
						string.Format ("FThe set of integers between {0} and {1}.", Random.Range (-10000, -100), Random.Range (100, 10000)),
						"IThe set of points that can be marked on a page of your book.",
						"FThe set of students who play cricket in a school.",
						"IThe set of all the numbers."
					};
					int randSelector = Random.Range (0, questionList.Count);
					QuestionLatext.text = "Pick the correct option about the given set.";
					subQuestionText.text = questionList[randSelector].Substring (1);
					Answer = questionList[randSelector][0] == 'I'? "Infinite set": "Finite set";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Infinite set";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "Finite set";
				} 
				else if (selector == 2) 
				{
					char randAlpha = (char)(65 + Random.Range (0, 26));
					int cardinalNumber = Random.Range (5, 11);

					QuestionLatext.text = string.Format ("Find \\nalgebra({0}).", randAlpha);
					subQuestionText.text = string.Format ("{0} = {1}", randAlpha, MathFunctions.getArrayAsSet (MathFunctions.GetUniqueIntRandomDataSet (5, 20, cardinalNumber).ToArray (), true));

					Answer = string.Format ("{0}", cardinalNumber);
				}
				else if (selector == 3) 
				{
					int randSelector = Random.Range (0, 4);
					int rangeMin = Random.Range (0, 6);
					int cardinalNumber = Random.Range (5, 16);
					char randAlpha = (char)(65 + Random.Range (0, 26));
					string inequality = GetInequality (rangeMin, cardinalNumber, randSelector);

					QuestionLatext.text = string.Format ("Find \\nalgebra({0}).", randAlpha);
					subQuestionText.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra \\in {1}, {2}}}\\rbrace", randAlpha, rangeMin == 0? "W": "N", inequality);
					Answer = string.Format ("{0}", cardinalNumber);
				}
				else if (selector == 4) 
				{
					SetMCQMode (2);

					char randAlpha1 = (char)(65 + Random.Range (0, 26));
					char randAlpha2 = (char)(65 + Random.Range (0, 26));
					while (randAlpha1 == randAlpha2) {
						randAlpha2 = (char)(65 + Random.Range (0, 26));
					}

					if (Random.Range (0, 3) == 0) { 
						subQuestionText.gameObject.SetActive (false);
						QuestionLatext.text = string.Format ("If \\nalgebra({0}) = \\nalgebra({1}), are {0} and {1} equivalent sets?", randAlpha1, randAlpha2);
						Answer = "Yes";
					}
					else {
						subQuestionText2.gameObject.SetActive (true);

						int rangeMin = Random.Range (0, 6);
						int rangeAddend = Random.Range (1, 6);
						string inequality1, inequality2;

						if (Random.Range (0, 2) == 0) 
						{
							int randSelector1 = Random.Range (0, 4);
							int randSelector2 = Random.Range (0, 4);
							int cardinalNumber = Random.Range (5, 16);
							inequality1 = GetInequality (rangeMin, cardinalNumber, randSelector1);
							inequality2 = GetInequality (rangeMin + rangeAddend, cardinalNumber, randSelector2);
							Answer = "Yes";
						} 
						else {
							int randSelector1 = Random.Range (0, 4);
							int randSelector2 = Random.Range (0, 4);
							int cardinalNumber1 = Random.Range (5, 16);
							int cardinalNumber2 = cardinalNumber1 + MathFunctions.GenerateRandomIntegerExcluding0 (-2, 3);
						
							inequality1 = GetInequality (rangeMin, cardinalNumber1, randSelector1);
							inequality2 = GetInequality (rangeMin + rangeAddend, cardinalNumber2, randSelector2);
							Answer = "No";
						}
							
						QuestionLatext.text = string.Format ("Are set {0} and set {1} equivalent?", randAlpha1, randAlpha2);
						subQuestionText.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra \\in {1}, {2}}}\\rbrace", randAlpha1, rangeMin == 0? "W": "N", inequality1);
						subQuestionText2.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra \\in {1}, {2}}}\\rbrace", randAlpha2, Random.Range (0, 2) == 0? "W": "N", inequality2);
					}

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Yes";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "No";
				}
				else if (selector == 5) 
				{
					SetMCQMode (2);

					char randAlpha1 = (char)(65 + Random.Range (0, 26));
					char randAlpha2 = (char)(65 + Random.Range (0, 26));
					while (randAlpha1 == randAlpha2) {
						randAlpha2 = (char)(65 + Random.Range (0, 26));
					}

					if (Random.Range (0, 3) == 0) { 
						subQuestionText.gameObject.SetActive (false);
						QuestionLatext.text = string.Format ("If \\nalgebra({0}) = \\nalgebra({1}), are {0} and {1} equal sets?", randAlpha1, randAlpha2);
						Answer = "No";
					}
					else {
						subQuestionText2.gameObject.SetActive (true);
						List<int> set1 = new List<int> ();
						List<int> set2 = new List<int> ();
						int cardinalNumber = Random.Range (5, 9);

						if (Random.Range (0, 2) == 0) {
							set1 = MathFunctions.GetUniqueIntRandomDataSet (10, 100, cardinalNumber);
							set2.AddRange (set1);
							set1.Shuffle ();
							set2.Shuffle ();
							Answer = "Yes";
						} else {
							set1 = MathFunctions.GetUniqueIntRandomDataSet (10, 100, cardinalNumber);
							set2 = MathFunctions.GetUniqueIntRandomDataSet (10, 100, cardinalNumber);
							set1.Shuffle ();
							set2.Shuffle ();
							Answer = "No";
						}

						QuestionLatext.text = string.Format ("Are set {0} and set {1} equal?", randAlpha1, randAlpha2);
						subQuestionText.text = string.Format ("{0} = {1}", randAlpha1, MathFunctions.getArrayAsSet (set1.ToArray (), true));
						subQuestionText2.text = string.Format ("{0} = {1}", randAlpha2, MathFunctions.getArrayAsSet (set2.ToArray (), true));
					}

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Yes";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "No";
				}
			}
			#endregion
			#region level3
			else if (level == 3)
			{
				selector = GetRandomSelector (1, 5);
				HasAnswerSet = true;

				var num1 = Random.Range (3, 7);
				var num2 = Random.Range (3, 7);

				int[] setA = new int[num1];
				int[] setB = new int[num2];
				int[] randNums1 = getRandomArray ();
				for (var i = 0; i < num1; i++) {
					setA [i] = randNums1 [i];
				}
				int[] randNums2 = getRandomArray ();
				for (var i = 0; i < num2; i++) {
					setB [i] = randNums2 [i];
				}

				subQuestionText.text = string.Format ("A = {0}, B = {1}", MathFunctions.getArrayAsSet (setA, true), MathFunctions.getArrayAsSet (setB, true));

				if (selector == 1) 
				{
					QuestionLatext.text = "Find A \\cup B.";
					answer = getUnion (setB, setA);
				} 
				else if (selector == 2) 
				{
					QuestionLatext.text = "Find A \\cap B.";
					answer = getIntersection (setB, setA);
				}
				else if (selector == 3) 
				{
					QuestionLatext.text = "Find A - B.";
					answer = getDifference (setA, setB);
				}
				else if (selector == 4) 
				{
					QuestionLatext.text = "Find B - A.";
					answer = getDifference (setB, setA);
				}
				Answer = MathFunctions.getArrayAsSet (answer, false, true);
			}
			#endregion
			#region level4
			else if (level == 4)
			{
				selector = GetRandomSelector (1, 6);

				char randAlpha1 = (char)(65 + Random.Range (0, 26));
				char randAlpha2 = (char)(65 + Random.Range (0, 26));
				while (randAlpha1 == randAlpha2) {
					randAlpha2 = (char)(65 + Random.Range (0, 26));
				}
				int[] set1 = MathFunctions.GetUniqueIntRandomDataSet (1, 10, Random.Range (4, 9)).ToArray (); 
				int[] set2 = MathFunctions.GetUniqueIntRandomDataSet (1, 10, Random.Range (4, 9)).ToArray (); 

				if (selector == 1) 
				{
					QuestionLatext.text = string.Format ("Find \\nalgebra({0} \\cup {1}), if :", randAlpha1, randAlpha2);
					subQuestionText.text = string.Format ("\\nalgebra({0}) = {1}, \\nalgebra({2}) = {3}, \\nalgebra({0} \\cap {2}) = {4}", randAlpha1, set1.Length, randAlpha2, set2.Length, getIntersection (set1, set2).Length);
					Answer = getUnion (set1, set2).Length.ToString ();
				} 
				else if (selector == 2) 
				{
					QuestionLatext.text = string.Format ("Find \\nalgebra({0} \\cap {1}), if :", randAlpha1, randAlpha2);
					subQuestionText.text = string.Format ("\\nalgebra({0}) = {1}, \\nalgebra({2}) = {3}, \\nalgebra({0} \\cup {2}) = {4}", randAlpha1, set1.Length, randAlpha2, set2.Length, getUnion (set1, set2).Length);
					Answer = getIntersection (set1, set2).Length.ToString ();
				}
				else if (selector == 3) 
				{
					QuestionLatext.text = string.Format ("Find \\nalgebra({0} - {1}), if :", randAlpha1, randAlpha2);
					if (Random.Range (0, 2) == 0) {
						subQuestionText.text = string.Format ("\\nalgebra({0}) = {2}, \\nalgebra({0} \\cap {1}) = {3}", randAlpha1, randAlpha2, set1.Length, getIntersection (set1, set2).Length);
					} else {
						subQuestionText.text = string.Format ("\\nalgebra({1}) = {2}, \\nalgebra({0} \\cup {1}) = {3}", randAlpha1, randAlpha2, set2.Length, getUnion (set1, set2).Length);
					}
					Answer = getDifference (set1, set2).Length.ToString ();
				}
				else if (selector == 4) 
				{
					QuestionLatext.text = string.Format ("Find \\nalgebra({1} - {0}), if :", randAlpha1, randAlpha2);
					if (Random.Range (0, 2) == 0) {
						subQuestionText.text = string.Format ("\\nalgebra({1}) = {2}, \\nalgebra({0} \\cap {1}) = {3}", randAlpha1, randAlpha2, set2.Length, getIntersection (set1, set2).Length);
					} else {
						subQuestionText.text = string.Format ("\\nalgebra({0}) = {2}, \\nalgebra({0} \\cup {1}) = {3}", randAlpha1, randAlpha2, set1.Length, getUnion (set1, set2).Length);
					}
					Answer = getDifference (set2, set1).Length.ToString ();
				}
				else if (selector == 5) 
				{
					SetMCQMode (2);

					QuestionLatext.text = string.Format ("Pick the correct option about the sets {0} and {1}, if :", randAlpha1, randAlpha2);
					int randSelector = Random.Range (0, 4);

					if (randSelector <= 1) 
					{
						int randNum = Random.Range (0, 2) == 0? 0: Random.Range (1, 10);
						subQuestionText.text = string.Format ("\\nalgebra({0} \\cap {1}) = {2}", randAlpha1, randAlpha2, randNum);
						Answer = randNum == 0? "Disjoint": "Overlapping";
					} 
					else if (randSelector == 1) 
					{
						subQuestionText2.gameObject.SetActive (true);

						int maxEven = Random.Range (50, 100);
						int maxOdd = maxEven + MathFunctions.GenerateRandomIntegerExcluding0 (-20, 21);

						subQuestionText.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra is an odd number, \\xalgebra {1} {2}}}\\rbrace", randAlpha1, Random.Range (0, 2) == 0? "\\leq" : "<", maxOdd);
						subQuestionText2.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra is an even number, \\xalgebra {1} {2}}}\\rbrace", randAlpha2, Random.Range (0, 2) == 0? "\\leq" : "<", maxEven);
						Answer = "Disjoint";
					} 
					else if (randSelector == 2)
					{
						subQuestionText2.gameObject.SetActive (true);

						int maxPrime = Random.Range (50, 100);
						int maxOdd = maxPrime + MathFunctions.GenerateRandomIntegerExcluding0 (-20, 21);

						subQuestionText.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra is an odd number, \\xalgebra {1} {2}}}\\rbrace", randAlpha1, Random.Range (0, 2) == 0? "\\leq" : "<", maxOdd);
						subQuestionText2.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra is a prime number, \\xalgebra {1} {2}}}\\rbrace", randAlpha2, Random.Range (0, 2) == 0? "\\leq" : "<", maxPrime);
						Answer = "Overlapping";
					} 

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Overlapping";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "Disjoint";
				}
			}
			#endregion
			#region level5
			else if (level == 5)
			{
				selector = GetRandomSelector (1, 9);
				StartSequence (8);
				subQuestionText2.gameObject.SetActive (true);

				int maxPrime = Random.Range (10, 14);
				bool includeMax = Random.Range (0, 2) == 0? true: false;
				char randAlpha1 = (char)(65 + Random.Range (0, 26));
				char randAlpha2 = (char)(65 + Random.Range (0, 26));
				while (randAlpha1 == randAlpha2) {
					randAlpha2 = (char)(65 + Random.Range (0, 26));
				}
				List<int> set1 = MathFunctions.GetUniqueIntRandomDataSet (1, 10, Random.Range (4, 9)); 
				List<int> set2 = MathFunctions.GetPrimes (maxPrime, includeMax);

				subQuestionText.text = string.Format ("{0} = {1}", randAlpha1, MathFunctions.getArrayAsSet (set1.ToArray (), true));
				subQuestionText2.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra is a prime number, \\xalgebra {1} {2}}}\\rbrace", randAlpha2, includeMax? "\\leq" : "<", maxPrime);
					
				if (selector == 1) //AuB
				{
					QuestionLatext.text = string.Format ("Find {0} \\cup {1}.", randAlpha1, randAlpha2);
					answer = getUnion (set1.ToArray (), set2.ToArray ());
				} 
				else if (selector == 2) //AinterB
				{
					QuestionLatext.text = string.Format ("Find {0} \\cap {1}.", randAlpha1, randAlpha2);
					answer = getIntersection (set1.ToArray (), set2.ToArray ());
				}
				else if (selector == 3) //A-B
				{
					QuestionLatext.text = string.Format ("Find {0} - {1}.", randAlpha1, randAlpha2);
					answer = getDifference (set1.ToArray (), set2.ToArray ());
				}
				else if (selector == 4) //B-A
				{
					QuestionLatext.text = string.Format ("Find {1} - {0}.", randAlpha1, randAlpha2);
					answer = getDifference (set2.ToArray (), set1.ToArray ());
				}
				else if (selector == 5) //nA-B
				{
					QuestionLatext.text = string.Format ("Find \\nalgebra({0}).", randAlpha1);
					answer = getDifference (set1.ToArray (), set2.ToArray ());
				}
				else if (selector == 6) //nB-A
				{
					QuestionLatext.text = string.Format ("Find \\nalgebra({0}).", randAlpha2);
					answer = getDifference (set2.ToArray (), set1.ToArray ());
				}
				else if (selector == 7) //nAuB
				{
					QuestionLatext.text = string.Format ("Find \\nalgebra({0} \\cup {1}).", randAlpha1, randAlpha2);
					answer = getUnion (set1.ToArray (), set2.ToArray ());
				}
				else if (selector == 8) //nAinterB
				{
					QuestionLatext.text = string.Format ("Find \\nalgebra({0} \\cap {1}).", randAlpha1, randAlpha2);
					answer = getIntersection (set1.ToArray (), set2.ToArray ());
				}
				if (selector <= 4) {
					HasAnswerSet = true;
					Answer = MathFunctions.getArrayAsSet (answer, false, true);
				} else {
					Answer = answer.Length.ToString ();
				}
			}
			#endregion

			CerebroHelper.DebugLog (Answer);
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
		}

		public void InputButtonClicked(int index) {
			GameObject buttons;
			if (EquationButtons.activeSelf) {
				buttons = EquationButtons;
			} else {
				buttons = RangeButtons;
			}
			if (index == 1) {
				answerButton = buttons.transform.Find ("Button1").GetComponent<Button> ();
				buttons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Image> ("Image").color = new Color (1f, 1f, 1f, 1f);
			} else if (index == 2) {
				answerButton = buttons.transform.Find ("Button2").GetComponent<Button> ();
				buttons.transform.Find ("Button1").GetComponent<Button> ().gameObject.GetChildByName<Image> ("Image").color = new Color (1f, 1f, 1f, 1f);
			}
			answerButton.gameObject.GetChildByName<Image> ("Image").color = new Color (200f/255f, 220f/255f, 255f/255f, 1f);
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
		}

		int[] getRosterForm(int[] equation, int[] numbers) {
			List<int> answers = new List<int> ();
			if (equation.Length == 2) {
				for (var i = 0; i < numbers.Length ; i++) {
					answers.Add ((equation [0] * numbers [i]) + equation [1]);
				}
			} else if (equation.Length == 3) {
				for (var i = 0; i < numbers.Length ; i++) {
					answers.Add ((equation [0] * numbers [i] * numbers [i]) + (equation [1] * numbers [i]) + equation [2]);
				}
			}
			return answers.ToArray ();
		}
		int[] getRandomArray()
		{
			int[] tmpNums = numbers.Clone () as int[];
			for (int t = 0; t < tmpNums.Length; t++ )
			{
				int tmp = tmpNums[t];
				int r = Random.Range(t, tmpNums.Length);
				tmpNums[t] = tmpNums[r];
				tmpNums[r] = tmp;
			}
			return tmpNums;
		}

		int[] getDifference(int[] setA, int[] setB) {
			List<int> answer = new List<int> ();
			for (var i = 0; i < setA.Length; i++) {
				var found = false;
				for (var j = 0; j < setB.Length; j++) {
					if (setA [i] == setB [j]) {
						found = true;
						break;
					}
				}
				if (!found) {
					answer.Add (setA [i]);
				}
			}
			return answer.ToArray ();
		}

		int[] getIntersection(int[] setA, int[] setB) {
			return setA.Intersect (setB).ToArray ();
		}

		int[] getUnion(int[] setA, int[] setB) {
			return setA.Union (setB).ToArray ();
		}

		string GetInequality (int rangeMin, int cardinalNumber, int Case)
		{
			string inequality = rangeMin.ToString ();

			if (Case == 0) {
				inequality += " < \\xalgebra < " + (rangeMin + cardinalNumber + 1);
			} else if (Case == 1) {
				inequality += " \\leq \\xalgebra < " + (rangeMin + cardinalNumber);
			} else if (Case == 2) {
				inequality += " \\leq \\xalgebra \\leq " + (rangeMin + cardinalNumber - 1);
			} else if (Case == 3) {
				inequality += " < \\xalgebra \\leq " + (rangeMin + cardinalNumber);
			}
			return inequality;
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
			} else if (value == 12) {   //} 
				if(checkLastTextFor(new string[1]{"}"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "}";
			} else if (value == 13) {   //{
				if(checkLastTextFor(new string[1]{"{"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "{";
			} else if (value == 14) {   // ,
				if(checkLastTextFor(new string[1]{","})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ",";
			} else if (value == 15) {   // -
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
		}

		protected void SetMCQMode (int NumberOfMCQ = 4)
		{
			this.MCQ.SetActive (true);
			Vector2[] positions;

			if (NumberOfMCQ == 3) 
			{
				positions = new Vector2[] {
					new Vector2 (-255, 40f),
					new Vector2 (0, 40f),
					new Vector2 (255, 40f),
					new Vector2 (0, 0f)
				};	
			} 
			else if (NumberOfMCQ == 2) {
				positions = new Vector2[] {
					new Vector2 (-180, 40f),
					new Vector2 (180, 40f),
					new Vector2 (-180, 0f),
					new Vector2 (180, 0f)
				};
			}
			else 
			{
				positions = new Vector2[] {
					new Vector2 (-180, 80f),
					new Vector2 (180, 80f),
					new Vector2 (-180, 0f),
					new Vector2 (180, 0f)
				};
			}

			for (int i = 1; i <= 4; i++)
			{
				MCQ.transform.Find ("Option" + i).gameObject.SetActive (i<=NumberOfMCQ);
				MCQ.transform.Find ("Option" + i).GetComponent<RectTransform> ().anchoredPosition = positions[i-1];
			}
			this.MCQ.SetActive (true);
			this.numPad.SetActive (false);
			this.GeneralButton.gameObject.SetActive (false);

			for (int i = 1; i <= 4; i++) {
				if(MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ())
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
				else
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}
		}

		protected void SetNumpadMode ()
		{
			this.numPad.SetActive (true);
			this.MCQ.SetActive (false);
			this.GeneralButton.gameObject.SetActive (true);
			ContinueBtn.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,ContinueBtn.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
			FlagButton.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,FlagButton.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
			SolutionButton.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,SolutionButton.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
		}
	}
}
