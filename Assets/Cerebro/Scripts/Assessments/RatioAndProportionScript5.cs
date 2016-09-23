using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class RatioAndProportionScript5 : BaseAssessment {

		public Text subQuestionText;
		public GameObject MCQ;
		public GameObject pictureParent;
		public Sprite[] fruitSprites;
		private string Answer;
		private int isProportion = 0;

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "RAP05", "S01", "A01");

			scorestreaklvls = new int[4];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
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


			if (MCQ.activeSelf){

				if (Answer == userAnswerText.text) 
				{
					correct = true;
				}
				else
				{
					correct = false;
					AnimateMCQOptionCorrect(Answer);

				}
				
			}
			else if (isProportion == 1)
			{
				if (Answer == userAnswerText.text)
					correct = true;
				else
					correct = false;
			}
			else if (Answer.Contains ("/")){
				var answerSplits = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
				var userAnswerSplits = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);

				if (answerSplits.Length == userAnswerSplits.Length) {
					for (var i = 0; i < answerSplits.Length; i++) {
						float answer = 0;
						float userAnswer = 0;

						if (float.TryParse (answerSplits [i], out answer)) {
							answer = float.Parse (answerSplits [i]);
						} else {
							correct = false;
							break;
						}
						if (float.TryParse (userAnswerSplits [i], out userAnswer)) {
							userAnswer = float.Parse (userAnswerSplits [i]);
						} else {
							correct = false;
							break;
						}
						if (answer != userAnswer) {
							correct = false;
							break;
						}
					}
				} else {
					correct = false;
				}
			}
			else{
				var answerSplits = Answer.Split (new string[] { ":" }, System.StringSplitOptions.None);
				var userAnswerSplits = userAnswerText.text.Split (new string[] { ":" }, System.StringSplitOptions.None);

				if (answerSplits.Length == userAnswerSplits.Length) {
					for (var i = 0; i < answerSplits.Length; i++) {
						float answer = 0;
						float userAnswer = 0;

						if (float.TryParse (answerSplits [i], out answer)) {
							answer = float.Parse (answerSplits [i]);
						} else {
							correct = false;
							break;
						}
						if (float.TryParse (userAnswerSplits [i], out userAnswer)) {
							userAnswer = float.Parse (userAnswerSplits [i]);
						} else {
							correct = false;
							break;
						}
						if (answer != userAnswer) {
							correct = false;
							break;
						}
					}
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

		void AnimateMCQOptionCorrect(string ans)
		{
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
			userAnswerText.color = MaterialColor.red800;
			Go.to( userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake( new Vector3( 0, 0, 20 ), GoShakeType.Eulers ) );
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				userAnswerText.text = "";
				userAnswerText.color = MaterialColor.textDark;
				ignoreTouches = false;
			} else {
				if (numPad.activeSelf) {
					userAnswerText.text = Answer.ToString ();
					userAnswerText.color = MaterialColor.green800;
				}
			}
			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			userAnswerText.color = MaterialColor.green800;
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations( 2, GoLoopType.PingPong );
			var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
			var tween = new GoTween( userAnswerText.gameObject.transform, 0.2f, config );
			flow.insert( 0f, tween );
			flow.play ();
			yield return new WaitForSeconds (1f);
			userAnswerText.color = MaterialColor.textDark;

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
			subQuestionText.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (true);
			MCQ.SetActive (false);
			numPad.SetActive (true);
			ResetPicture ();

			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				
				subQuestionText.gameObject.SetActive (true);
				selector = GetRandomSelector (1, 7);

				if (selector == 1) {												//

					subQuestionText.gameObject.SetActive (false);
					string[] fruits = new string[]{"apples","mangoes","pineapples","oranges","bananas"};
					int randIndex1 = Random.Range (0, fruits.Length);
					int randIndex2 = Random.Range (0, fruits.Length);
					while (randIndex1 == randIndex2) 
					{
						randIndex2 = Random.Range (0, fruits.Length);
					}

					QuestionText.text = "Write the ratio of "+ fruits[randIndex1] +" to "+fruits[randIndex2] +".";

					int[] numbers = new int[fruits.Length];
					int length = numbers.Length;
					int total = 0;
					int value = 0;
					for (int i = 0; i < length; i++) 
					{
						value = Random.Range (2, 6);
					    numbers [i] = value;
						total += value;
						for (int j = 0; j < value; j++)
						{
							AddObjectInPicture (fruitSprites [i]);
						}
					}
						
					Answer = numbers [randIndex1] + ":" + numbers [randIndex2];

				} else if (selector == 2) {											//Find missing term

					int num1 = Random.Range (2, 6);
					int num2 = Random.Range (num1 + 1, 10);
					int commonRatio = Random.Range (2, 10);
					int num3 = commonRatio * num1;
					int num4 = commonRatio * num2;
					QuestionText.text = "Find the missing number : ";
					subQuestionText.text = num1 + " : " + num2 + " = " + num3 + " : ___";
					Answer = num4.ToString ();

				} else if (selector == 3) {											//simplest form of ratio
					
					int commonRatio = Random.Range (2, 10);
					int num1 = commonRatio * Random.Range (2, 10);
					int num2 = commonRatio * Random.Range (2, 10);
					while (num1 == num2)
						num2 = commonRatio * Random.Range (2, 10);
					QuestionText.text = "Give the ratio in its simplest form :";
					subQuestionText.text = num1 + " : " + num2;
					int hcf = MathFunctions.GetHCF (num1, num2);
					num1 /= hcf;
					num2 /= hcf;
					Answer = num1 + ":" + num2;

				} else if (selector == 4) {											//simplest form with units
					
					string[] selectQuantity1 = new string[] {"m","years","weeks","hours","l","kg","Rs."};
					string[] selectQuantity2 = new string[] {"cm","months","days","minutes","ml","g","p"};
					int[] conversionFactor = new int[]{100, 12, 7, 60, 1000, 1000, 100 };
					int randomSelector = Random.Range (0, selectQuantity1.Length);
					int num1 = Random.Range (2, 10);
					int num2 = conversionFactor [randomSelector] * Random.Range (2, 10);
					while (num1 * conversionFactor[randomSelector] == num2)
						num2 = conversionFactor [randomSelector] * Random.Range (2, 10);
					QuestionText.text = "Give the ratio in its simplest form :";
					subQuestionText.text = num1 + " " + selectQuantity1[randomSelector] + " to " + num2 + " " + selectQuantity2[randomSelector] + ".";
					num2 /= conversionFactor [randomSelector];
					int hcf = MathFunctions.GetHCF (num1, num2);
					num1 /= hcf;
					num2 /= hcf;
					Answer = num1 + ":" + num2;

				} else if (selector == 5) {											//express as ratio

					subQuestionText.gameObject.SetActive (false);
					int num1 = Random.Range (2, 10);
					int num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);
					QuestionText.text = "In a shop there are " + num1 + " packets of brown bread and " + num2 + " packets of white bread. Give the ratio of brown bread to white bread in the shop.";
					Answer = num1 + ":" + num2;

				} else if (selector == 6) {											//Ratio to fraction

					int num1 = Random.Range (2, 10);
					int num2 = Random.Range (num1 + 1, 15);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);
					QuestionText.text = "Convert the following ratio to fraction :";
					subQuestionText.text = num1 + " : " + num2;
					Answer = num1 + "/" + num2;

				}
			} 

			else if (level == 2) {	
				
				selector = GetRandomSelector (1, 6);
				subQuestionText.gameObject.SetActive (true);

				if (selector == 1) {    										// express as proportion

					isProportion = 1;
					int num1 = Random.Range (2, 6);
					int num2 = Random.Range (num1 + 1, 10);
					int commonRatio = Random.Range (2, 10);
					int num3 = commonRatio * num1;
					int num4 = commonRatio * num2;
					QuestionText.text = "Write as proportion :";
					subQuestionText.text = num1 + " is to " + num2 + " is as " + num3 + " is to " + num4;
					Answer = num1 + ":" + num2 + "::" + num3 + ":" + num4;


				} else if (selector == 2) {											//check for proportion
					
					GeneralButton.gameObject.SetActive (false);
					MCQ.SetActive (true);
					numPad.SetActive (false);
					List<string> options = new List<string>();
					int num1 = Random.Range (2, 6);
					int num2 = Random.Range (num1 + 1, 10);
					int commonRatio1 = Random.Range (2, 10);
					int num3 = commonRatio1 * num1;
					int num4 = commonRatio1 * num2;
					int num5 = Random.Range (num1 + 1, 10);
					int num6 = Random.Range (num5 + 1, 15);
					int commonRatio2 = Random.Range (2, 10);
					int num7 = commonRatio2 * num5;
					int num8 = num7 + num6 - num5;
					QuestionText.text = "Which of the following are in proportion :";
					int randAct = Random.Range (1, 5);
					options.Add("(i) only");
					options.Add("(ii) only");
					options.Add("both");
					options.Add("none");
					if (randAct == 1) {
						subQuestionText.text = "(i) " + num1 + " is to " + num2 + " is as " + num3 + " is to " + num4 + "\n";
						subQuestionText.text += "(ii) " + num5 + " is to " + num6 + " is as " + num7 + " is to " + num8;
						Answer = options[0];
					} else if (randAct == 2) {
						subQuestionText.text = "(i) "  + num5 + " is to " + num6 + " is as " + num7 + " is to " + num8 + "\n";
						subQuestionText.text += "(ii) " + num1 + " is to " + num2 + " is as " + num3 + " is to " + num4;
						Answer = options[1];
					} else if (randAct == 3) {
						num8 = commonRatio2 * num6;
						subQuestionText.text = "(i) "  + num5 + " is to " + num6 + " is as " + num7 + " is to " + num8 + "\n";
						subQuestionText.text += "(ii) " + num1 + " is to " + num2 + " is as " + num3 + " is to " + num4;
						Answer = options[2];
					} else if (randAct == 4) {
						num4 = num3 + num2 - num1;
						subQuestionText.text = "(i) "  + num5 + " is to " + num6 + " is as " + num7 + " is to " + num8 + "\n";
						subQuestionText.text += "(ii) " + num1 + " is to " + num2 + " is as " + num3 + " is to " + num4;
						Answer = options[3];
					} 
					for (int i = 0; i < 4; i++)
						MCQ.transform.Find ("Option" + (i + 1)).Find ("Text").GetComponent<Text> ().text = options [i];

				} else if (selector == 3) {    										// express as proportion
					
					subQuestionText.gameObject.SetActive (false);
					isProportion = 1;
					int num1 = Random.Range (2, 6);
					int num2 = Random.Range (num1 + 1, 10);
					int commonRatio = Random.Range (2, 10);
					int num3 = commonRatio * num1;
					int num4 = commonRatio * num2;
					QuestionText.text = "There are " + num1 + " red pencils and " + num2 + " blue pencils in a box. " + num3 + " red pencils and " + num4 + " blue pencils are there in another box. Write down the ratios as a proportion.";
					Answer = num1 + ":" + num2 + "::" + num3 + ":" + num4;

				} else if (selector == 4) {		// Find missing number in proportion

					int num1 = Random.Range (2, 6);
					int num2 = Random.Range (num1 + 1, 10);
					int commonRatio = Random.Range (2, 10);
					int num3 = commonRatio * num1;
					int num4 = commonRatio * num2;
					QuestionText.text = "Find the missing number : ";
					subQuestionText.text = num1 + " : " + num2 + " :: " + "___ : " + num4;
					Answer = num3.ToString ();

				} else if (selector == 5) {											//Check for proportion 

					GeneralButton.gameObject.SetActive (false);
					MCQ.SetActive (true);
					numPad.SetActive (false);
					List<string> options = new List<string>();
					int num1 = Random.Range (2, 6);
					int num2 = Random.Range (num1 + 1, 10);
					int commonRatio1 = Random.Range (2, 10);
					int num3 = commonRatio1 * num1;
					int num4 = commonRatio1 * num2;
					int num5 = Random.Range (num1 + 1, 10);
					int num6 = Random.Range (num5 + 1, 15);
					int commonRatio2 = Random.Range (2, 10);
					int num7 = commonRatio2 * num5;
					int num8 = num7 + num6 - num5;
					QuestionText.text = "Which of the following are in proportion :";
					int randAct = Random.Range (1, 5);
					options.Add("(i) only");
					options.Add("(ii) only");
					options.Add("both");
					options.Add("none");
					if (randAct == 1) {
						subQuestionText.text = "(i) " + num1 + " books in " + num2 + " days, " + num3 + " books in " + num4 + " days\n";
						subQuestionText.text += "(ii) " + num5 + " books in " + num6 + " days, " + num7 + " books in " + num8 + " days";
						Answer = options[0];
					} else if (randAct == 2) {
						subQuestionText.text = "(i) "  + num5 + " books in " + num6 + " days, " + num7 + " books in " + num8 + " days\n";
						subQuestionText.text += "(ii) " + num1 + " books in " + num2 + " days, " + num3 + " books in " + num4 + " days";
						Answer = options[1];
					} else if (randAct == 3) {
						num8 = commonRatio2 * num6;
						subQuestionText.text = "(i) "  + num5 + " books in " + num6 + " days, " + num7 + " books in " + num8 + " days\n";
						subQuestionText.text += "(ii) " + num1 + " books in " + num2 + " days, " + num3 + " books in " + num4 + " days";
						Answer = options[2];
					} else if (randAct == 4) {
						num4 = num3 + num2 - num1;
						subQuestionText.text = "(i) "  + num5 + " books in " + num6 + " days, " + num7 + " books in " + num8 + " days\n";
						subQuestionText.text += "(ii) " + num1 + " books in " + num2 + " days, " + num3 + " books in " + num4 + " days";
						Answer = options[3];
					} 
					for (int i = 0; i < 4; i++)
						MCQ.transform.Find ("Option" + (i + 1)).Find ("Text").GetComponent<Text> ().text = options [i];
					
				}
			} 

			else if (level == 3) {	

				subQuestionText.gameObject.SetActive (false);
				selector = GetRandomSelector (1, 6);

				if (selector == 1) {												//Finding Ratio of two quantities

					int commonRatio = Random.Range (5, 10);
					int num1 = Random.Range (2, 10);
					int num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);
					Answer = num1 + ":" + num2;
					num1 *= commonRatio;
					num2 *= commonRatio;
					QuestionText.text = "Out of " + (num1 + num2) + " sheep on a farm, " + num1 + " are white and the rest are black. What is the ratio of white sheep to black sheep?";

				} else if (selector == 2) {    											// Finding Ratio of a quantity to total quantities

					int commonRatio = Random.Range (5, 10);
					int num1 = Random.Range (2, 10);
					int num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);
					Answer = num1 + ":" + (num1 + num2);
					num1 *= commonRatio;
					num2 *= commonRatio;
					QuestionText.text = "A bakery has " + num1 + " chocolate pasteries and " + num2 + " pineapple pasteries in its display window. What is the ratio of chocolate pasteries to all the pasteries on display?";

				} else if (selector == 3) {    										// Finding a quantity, given another quantity and ratio 

					int commonRatio = Random.Range (5, 10);
					int num1 = Random.Range (2, 10);
					int num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);
					QuestionText.text = "Ratio of students who walk to school to students who come by bus is " + num1 + " : " + num2 + ". " + (num1 * commonRatio) + " students walk to school. How many students come by bus?";
					Answer = (num2 * commonRatio).ToString ();

				} else if (selector == 4) {											// Find a missing quantity using proportion

					int commonRatio = Random.Range (5, 10);
					int num1 = Random.Range (2, 8);
					int num2 = Random.Range (num1 + 1, 11);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (num1 + 1, 11);
					QuestionText.text = num1 + " rakhis cost Rs. " + (num1 * commonRatio) + ". How much money will be needed to buy " + num2 + " rakhis?";
					Answer = (num2 * commonRatio).ToString ();

				} else if (selector == 5) {											// Find a distance on map using proportion

					int commonRatio = 100 * Random.Range (2, 10);
					int num1 = Random.Range (2, 8);
					int num2 = Random.Range (num1 + 1, 11);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (num1 + 1, 11);
					QuestionText.text = num1 + " cm on a map is the same as " + (num1 * commonRatio) + " km on land. What is the distance on land (in km) if the map shows the distance as " + num2 + " cm?";
					Answer = (num2 * commonRatio).ToString ();

				}  
			} 

			else if (level == 4) {			
				selector = GetRandomSelector (1, 6);

				if (selector == 1) {											

					int commonRatio = Random.Range (11, 20);
					int num1 = Random.Range (2, 10);
					int num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);
					QuestionText.text = "Zaheeda and her friend Smita brought " + ((num1 + num2) * commonRatio) + " pasteries for the class party. If they had brought it in the ratio of " + num1 + " : " + num2 + ", how many pasteries did Smita bring?";
					Answer = (num2 * commonRatio).ToString ();

				} else if (selector == 2) {    										

					int commonRatio = Random.Range (11, 20);
					int num1 = Random.Range (2, 10);
					int num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);
					QuestionText.text = "There are " + ((num1 + num2) * commonRatio) + " flats in a building. The ratio of occupied to unoccupied flats is " + num1 + " : " + num2 + ". How many occupied flats are there in the building?";
					Answer = (num1 * commonRatio).ToString ();

				}  else if (selector == 3) {										

					int commonRatio = Random.Range (11, 20);
					int num1 = Random.Range (2, 10);
					int num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);
					QuestionText.text = "Mrs. Kapoor spent money on vegetables and fruits in the ratio of " + num1 + " : " + num2 + " in a month. If she spent Rs. " + ((num1 + num2) * commonRatio) + " in all, how much (in Rs.) did she spend on fruits?";
					Answer = (num2 * commonRatio).ToString ();

				} else if (selector == 4) {									
					
					int commonRatio = Random.Range (30, 61);
					int num1 = Random.Range (2, 8);
					int num2 = Random.Range (num1 + 1, 11);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (num1 + 1, 11);
					QuestionText.text = "A restaurant uses " + num1 + " cheese cubes for " + num2 + " pieces of pizza. If " + (num2 * commonRatio) + " pieces of pizza have to be made, how many cheese cubes will be needed?";
					Answer = (num1 * commonRatio).ToString ();

				} else if (selector == 5) {									

					int commonRatio = Random.Range (30, 61);
					int num1 = Random.Range (2, 8);
					int num2 = Random.Range (num1 + 1, 11);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (num1 + 1, 11);
					QuestionText.text = "A " + num1 + " m tall street light has a shadow of " + num2 + " m long. What will be the length (in m) of the shadow of a building that is " + (num1 * commonRatio) + " m tall at the same point of time?";
					Answer = (num2 * commonRatio).ToString ();

				} 
			}
			CerebroHelper.DebugLog (Answer);
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
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
			} else if (value == 12) {   // :
				if(checkLastTextFor(new string[1]{"::"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ":";
			} else if (value == 13) {   // .
				if(checkLastTextFor(new string[1]{"/"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			} 
		}

		private void AddObjectInPicture(Sprite sprite)
		{
			GameObject g = new GameObject ();
			Image image= g.AddComponent<Image> ();
			image.sprite = sprite;
			g.transform.SetParent (pictureParent.transform, false);
		}

		private void ResetPicture()
		{
			foreach (Transform child in pictureParent.transform) 
			{
				Destroy (child.gameObject);
			}
		}
	}
}
