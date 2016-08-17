using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro
{
	public class DecimalsScript5 : BaseAssessment
	{

		public TEXDraw subQuestionText;

		public GameObject MCQ;
		private string Answer;

		void Start ()
		{
			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "DEC05", "S01", "A01");

			scorestreaklvls = new int[4];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}

		bool checkFractions (string[] userAnswers, string[] correctAnswers)
		{
			float num1 = float.Parse (userAnswers [0]);
			float num2 = float.Parse (correctAnswers [0]);
			float den2 = float.Parse (correctAnswers [1]);
			float den1 = 1;
			if (userAnswers.Length == 2) {
				den1 = float.Parse (userAnswers [1]);
			}


			if ((num1 / num2) == (den1 / den2)) {
				return true;
			}
			return false;
		}

		public override void SubmitClick ()
		{
			if (ignoreTouches) {
				return;
			}
			if (numPad.activeSelf && userAnswerText.text == "") {
				return;
			}

			int increment = 0;
			ignoreTouches = true;

			//Checking if the response was correct and computing question level
			var correct = true;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (!numPad.activeSelf) {
				if (userAnswerText.text == Answer) {
					correct = true;
				} else {
					correct = false;
				}
			} else {
				if (Answer.Contains ("/")) {
					var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
					correct = checkFractions (userAnswers, correctAnswers);
					if (correctAnswers.Contains (".") != userAnswers.Contains (".")) {
						correct = false;
					}

				} else {
					float correctAns = float.Parse (Answer);
					float userAns = float.MinValue;
					if (float.TryParse (userAnswerText.text, out userAns)) {
						userAns = float.Parse (userAnswerText.text);
					}
					if (userAns == correctAns) {
						correct = true;
					} else {
						correct = false;
					}
				}
			}

			if (correct == true) {
				if (Queslevel == 1) {
					increment = 5;
					UpdateStreak (8, 12);
				} else if (Queslevel == 2) {
					increment = 5;
					UpdateStreak (4, 8);
				} else if (Queslevel == 3) {
					increment = 10;
					UpdateStreak (4, 8);
				} else if (Queslevel == 4) {
					increment = 15;
					UpdateStreak (4, 8);
				}

				//UpdateStreak (8, 12);

				updateQuestionsAttempted ();
				StartCoroutine (ShowCorrectAnimation ());
			} else {
				for (var i = 0; i < scorestreaklvls.Length; i++) {
					scorestreaklvls [i] = 0;
				}
				StartCoroutine (ShowWrongAnimation ());
			}

			base.QuestionEnded (correct, level, increment, selector);

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

		protected override IEnumerator ShowCorrectAnimation ()
		{
			if (userAnswerText != null) {
				userAnswerText.color = MaterialColor.green800;
				var config = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations (2, GoLoopType.PingPong);
				var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1));
				var tween = new GoTween (userAnswerText.gameObject.transform, 0.2f, config);
				flow.insert (0f, tween);
				flow.play ();
			} 

			yield return new WaitForSeconds (1f);

			if (userAnswerText != null) {
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

		protected override IEnumerator ShowWrongAnimation ()
		{
			if (userAnswerText != null) {
				userAnswerText.color = MaterialColor.red800;
			} 

			Go.to (userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				if (numPad.activeSelf) {				// is not MCQ type question
					userAnswerText.text = "";
				}
				if (userAnswerText != null) {
					userAnswerText.color = MaterialColor.textDark;
				} 
				ignoreTouches = false;
			} else {
				if (userAnswerText != null) {			// is not MCQ type question
					userAnswerText.text = Answer.ToString ();
					userAnswerText.color = MaterialColor.green800;
				}
			}

			ShowContinueButton ();
		}

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			level = Queslevel;

			answerButton = GeneralButton;

			subQuestionText.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (false);
			MCQ.gameObject.SetActive (false);
			numPad.SetActive (true);

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {
					float num1 = UnityEngine.Random.Range (10, 50) / 10f;
					if (num1 % 10 == 0) {
						num1 = UnityEngine.Random.Range (10, 50) / 10f;
					}
					float num2 = UnityEngine.Random.Range (2, 20);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply(Upto 2 decimal places)";
					subQuestionText.text = num1.ToString () + " \\times " + num2.ToString ();
					Answer = (((float)System.Math.Round((num1 * num2) * 100f, System.MidpointRounding.AwayFromZero)) / 100f).ToString (); //((float)(num1 * 100 / num2) / (float)100).ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int[] conversion = new int[] { 10, 100, 1000};
					int[] multiplier = new int[] { 0, 10, 100, 1000};
					int rndSelector = UnityEngine.Random.Range (0, conversion.Length);
					int rndDivider = UnityEngine.Random.Range (1, conversion.Length);

					int number = UnityEngine.Random.Range (1000, 10000);
					int preDec = number / conversion [rndSelector];
					int postDec = number % conversion [rndSelector];

					float dec = (float)System.Math.Round ((((float)postDec / (float)conversion [rndSelector]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					float num1 = (float)preDec + (float)dec;

					float num2 = conversion[rndDivider];
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply(Upto 2 decimal places)";
					subQuestionText.text = num1.ToString () + " \\times " + num2.ToString ();
					if (num2 != 0)
						Answer = (((float)System.Math.Round((num1 * num2) * 100f, System.MidpointRounding.AwayFromZero)) / 100f).ToString (); //((float)(num1 * 100 / num2) / (float)100).ToString ();
					else
						Answer = conversion [0].ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					float num1 = UnityEngine.Random.Range (10, 50) / 10f;
					float num2 = UnityEngine.Random.Range (1, 50) / 10f;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply(Upto 2 decimal places)";
					subQuestionText.text = num1.ToString () + " \\times " + num2.ToString ();
					Answer = (((float)System.Math.Round((num1 * num2) * 100f, System.MidpointRounding.AwayFromZero)) / 100f).ToString (); //((float)(num1 * 100 / num2) / (float)100).ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 4) {
					float num1 = UnityEngine.Random.Range (10, 50) / 10f;
					float num2 = UnityEngine.Random.Range (1, 10) / 100f;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply(Upto 2 decimal places)";
					subQuestionText.text = num1.ToString () + " \\times " + num2.ToString ();
					Answer = (((float)System.Math.Round((num1 * num2) * 1000f, System.MidpointRounding.AwayFromZero)) / 1000f).ToString (); //((float)(num1 * 100 / num2) / (float)100).ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 5) {
					float num1 = UnityEngine.Random.Range (1000, 10000) / 1000f;
					int num2 = UnityEngine.Random.Range (2, 10);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply(Upto 2 decimal places)";
					subQuestionText.text = num1.ToString () + " \\times " + num2.ToString ();
					Answer = (((float)System.Math.Round((num1 * num2) * 100f, System.MidpointRounding.AwayFromZero)) / 100f).ToString (); //((float)(num1 * 100 / num2) / (float)100).ToString ();
					GeneralButton.gameObject.SetActive (true);
				} 
			} else if (level == 2) {
				selector = GetRandomSelector (1, 2);

				int[] conversion = new int[] { 10, 100, 1000};
				int[] multiplier = new int[] { 10, 100, 1000};
				int rndSelector = UnityEngine.Random.Range (0, conversion.Length);
				int rndDivider = UnityEngine.Random.Range (0, conversion.Length);

				int number = UnityEngine.Random.Range (100, 1000);
				int preDec = number / conversion [rndSelector];
				int postDec = number % conversion [rndSelector];

				float dec = (float)System.Math.Round ((((float)postDec / (float)conversion [rndSelector]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
				float num1 = (float)preDec + (float)dec;

				float num2 = conversion[rndDivider];
				float ans = (((float)System.Math.Round ((num1 * num2) * 100f, System.MidpointRounding.AwayFromZero)) / 100f);
				subQuestionText.gameObject.SetActive (true);
				QuestionText.text = "Enter the missing number";
				subQuestionText.text = num1.ToString () + " \\times ? = " + ans.ToString ();
				Answer = num2.ToString ();
				GeneralButton.gameObject.SetActive (true);
			} else if (level == 3) {
				selector = GetRandomSelector (1, 2);

				int[] conversion = new int[] { 10, 100, 1000, 10000};
				int rndSelector = UnityEngine.Random.Range (0, conversion.Length);

				int number = UnityEngine.Random.Range (100, 10000);
				int preDec = number / conversion [rndSelector];
				int postDec = number % conversion [rndSelector];

				float dec = (float)System.Math.Round ((((float)postDec / (float)conversion [rndSelector]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
				float num1 = (float)preDec + (float)dec;

				float num2 = UnityEngine.Random.Range (2, 10);
				float ans = (((float)System.Math.Round ((num1 * num2) * 100f, System.MidpointRounding.AwayFromZero)) / 100f);

				subQuestionText.gameObject.SetActive (true);
				QuestionText.text = "Divide(upto 1 decimal place)";
				subQuestionText.text = ans.ToString () + " \\div " + num2.ToString ();
				Answer = num1.ToString ();
				GeneralButton.gameObject.SetActive (true);
			} else if (level == 4) {
				selector = GetRandomSelector (1, 2);

				int[] conversion = new int[] { 10, 100};
				int rndSelector = UnityEngine.Random.Range (0, conversion.Length);

				int number = UnityEngine.Random.Range (10, 1000);
				int preDec = number / conversion [rndSelector];
				int postDec = number % conversion [rndSelector];

				float dec = (float)System.Math.Round ((((float)postDec / (float)conversion [rndSelector]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
				float num1 = (float)preDec + (float)dec;

				float num2 = UnityEngine.Random.Range (2, 10);
				float ans = (((float)System.Math.Round ((num1 * num2) * 100f, System.MidpointRounding.AwayFromZero)) / 100f);

				subQuestionText.gameObject.SetActive (true);
				QuestionText.text = "Find the unit price (upto 2 decimal places)";
				subQuestionText.text = "Rs. " + ans.ToString () + " for " + num2.ToString () + " packets of bread.";
				Answer = num1.ToString ();
				GeneralButton.gameObject.SetActive (true);
			}

			CerebroHelper.DebugLog (Answer);
			userAnswerText = answerButton.gameObject.GetChildByName<Text> ("Text");
			userAnswerText.text = "";
		}

		public override void numPadButtonPressed (int value)
		{
			CerebroHelper.DebugLog ("num pad");
			if (ignoreTouches) {
				CerebroHelper.DebugLog ("num pad ignore touch");
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
			} else if (value == 12) {   // .
				if (checkLastTextFor (new string[1]{ "." })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			} else if (value == 13) {   // -
				if (checkLastTextFor (new string[1]{ "-" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			} else if (value == 14) {   // /
				if (checkLastTextFor (new string[1]{ "/" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			}
		}
	}
}
