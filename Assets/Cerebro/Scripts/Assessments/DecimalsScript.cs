using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro
{
	public class DecimalsScript : BaseAssessment
	{

		public TEXDraw subQuestionLaText;
		public Text subQuestionText;

		private string Answer;

		void Start ()
		{
			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "DEC06", "S01", "A01");

			scorestreaklvls = new int[4];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
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
					correct = MathFunctions.checkFractions (userAnswers, correctAnswers);
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
				} else if (Queslevel == 2) {
					increment = 5;
				} else if (Queslevel == 3) {
					increment = 10;
				} else if (Queslevel == 4) {
					increment = 15;
				} else if (Queslevel == 5) {
					increment = 15;
				}

				UpdateStreak (8, 12);

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

		protected override IEnumerator ShowCorrectAnimation ()
		{
			if (userAnswerText != null) {
				userAnswerText.color = MaterialColor.green800;
			} 
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations (2, GoLoopType.PingPong);
			var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1));
			var tween = new GoTween (userAnswerText.gameObject.transform, 0.2f, config);
			flow.insert (0f, tween);
			flow.play ();
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
			numPad.SetActive (true);

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {
					int num1 = UnityEngine.Random.Range (1, 200);
					int den1 = UnityEngine.Random.Range (2, 50);
					int hcf = MathFunctions.GetHCF (num1, den1);
					if (hcf > 1) {
						num1 /= hcf;
						den1 /= hcf;
					}
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert the below fraction to decimal (round to 2 decimal places) :";
					subQuestionText.text = num1.ToString () + " / " + den1.ToString ();
					//Answer = Math.Round ((float)num1 / (float)den1 , 2);
					Answer = (Mathf.Round ((float)(num1 * 100 / den1)) / (float)100).ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert the below decimal to fraction :";
					subQuestionText.text = num1.ToString ();
					Answer = (num1 * 100).ToString () + "/" + 100;
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					string[] main = new string[] { "kg", "l", "years", "months", "weeks", "hours", "rupees" };
					string[] sub = new string[] { "g", "ml", "months", "days", "days", "minutes", "paise" };
					int[] conversion = new int[] { 1000, 1000, 12, 30, 7, 60, 100 };

					int rndselector = UnityEngine.Random.Range (0, main.Length);

					int number = UnityEngine.Random.Range (0, conversion [rndselector] * 10);
					if (rndselector == 5) {
						number -= number % 6;
					}
					if (conversion [rndselector] >= 1000) {
						number = UnityEngine.Random.Range (0, conversion [rndselector]) * 10;
					}
					int preDec = number / conversion [rndselector];
					int postDec = number % conversion [rndselector];

					float dec = (float)System.Math.Round ((((float)postDec / (float)conversion [rndselector]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					float ans = (float)preDec + (float)dec;

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express " + preDec.ToString() + " " + main[rndselector] + " " + postDec + " " + sub[rndselector] + " as " + main[rndselector] + " (round to 2 decimal places)";
					subQuestionText.text = "";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 4) {
					float num1 = (float)UnityEngine.Random.Range (1, 50) / (float)10;
					float den1 = (float)UnityEngine.Random.Range (2, 50) / (float)10;
					float ans = num1 / den1;
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert the below fraction to decimal (Round to 2 decimals)";
					subQuestionText.text = num1.ToString () + " / " + den1.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} 
			} else if (level == 2) {
				selector = GetRandomSelector (1, 7);

				if (selector == 1) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float num2 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float ans = num1 + num2;
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Add the following decimals";
					subQuestionText.text = num1.ToString () + " + " + num2.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float num2 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float ans = (num1 - num2);
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Subtract the following decimals";
					subQuestionText.text = num1.ToString () + " - " + num2.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float num2 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float ans = (num2 - num1);
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					QuestionText.text = "What should be added to " + num1.ToString () + " to get " + num2.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 4) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float num2 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float ans = (num1 - num2);
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					QuestionText.text = "What should be subtracted from " + num1.ToString () + " to get " + num2.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 5) {
					float num1 = (float)UnityEngine.Random.Range (1, 10) / (float)10;
					float num2 = (float)UnityEngine.Random.Range (1, 10) / (float)10;
					float ans = num1 * num2;
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply the following decimals (Round to 2 decimal places)";
					subQuestionText.text = num1.ToString () + " x " + num2.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 6) {
					float num1 = (float)UnityEngine.Random.Range (1, 10) / (float)10;
					float num2 = (float)UnityEngine.Random.Range (1, 10) / (float)10;
					float ans = num1 / num2;
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Divide the following decimals (Round to 2 decimal places)";
					subQuestionText.text = num1.ToString () + " / " + num2.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				}
			} else if (level == 3) {
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)10;
					float num2 = (float)UnityEngine.Random.Range (1, 10);
					float ans = num1 / num2;
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Round the answer to 2 decimal places";
					subQuestionText.text = "Sushant runs " + num1.ToString () + " kms in " + num2.ToString () + " hours. How many kms will he run in 1 hour?";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					float ans = (float)UnityEngine.Random.Range (1, 50);
					float num1 = (float)UnityEngine.Random.Range (1, 50);
					float num2 = (ans * num1) / 1000;

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Round the answer to 2 decimal places";
					subQuestionText.text = "A jug has capacity of " + num2.ToString () + " litres. A glass can fill " + num1.ToString () + " ml. How many glasses can the jug fill?";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					float ans = (float)UnityEngine.Random.Range (1, 50);
					float num1 = (float)UnityEngine.Random.Range (1, 50);
					float num2 = (ans * num1) / 1000;

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Round the answer to 2 decimal places";
					subQuestionText.text = "A river has a width of " + num2.ToString () + " kms. If the distance between 2 columns of a bridge is " + num1.ToString () + " m. How many columns are required?";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 4) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float num2 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float num3 = (float)UnityEngine.Random.Range (1, 100) / (float)100;

					float ans = num1 + num2 + num3;

					num1 *= 1000;
					num3 *= 1000;


					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Round the answer to 2 decimal places";
					subQuestionText.text = "To prepare a dish, " + num1.ToString () + " gms of rice is added to " + num2.ToString () + " kg of water and " + num3.ToString () + " gm of pulses. Find the total weight of the dish in kgs";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} 
			} else if (level == 4) {
				selector = GetRandomSelector (1, 4);

				if (selector == 1) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)10;
					float glasses = (float)UnityEngine.Random.Range (2, 20);
					float total = glasses * num1;
					float cost = UnityEngine.Random.Range (2, 20);


					float ans = cost * num1;
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Round the answer to 2 decimal places";
					subQuestionText.text = total.ToString() + " litres of juice in a jug is poured equally in " + glasses.ToString() + " glasses. If 1 liter of juice costs Rs. " + cost.ToString() + ". How much does each glass cost?(Answer in Rs.)";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)100;
					float people = Random.Range (2, 10);
					float num2 = UnityEngine.Random.Range (num1 * people, 20);
					num2 = (float)System.Math.Round ((num2 * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					float ans = num2 - (people * num1);
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;

					num1 = num1 * 1000;

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Round the answer to 2 decimal places";
					subQuestionText.text = "A cake has weight " + num2.ToString () + " kg. Each person eats " + num1.ToString () + " gms of cake. If there are " + people.ToString () + " people who have eaten, how much cake is left in kg?";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					float num1 = (float)UnityEngine.Random.Range (1, 100) / (float)10;
					float num2 = (float)UnityEngine.Random.Range (1, 100) / (float)10;
					float num3 = (float)UnityEngine.Random.Range (1, 50);
					float num4 = (float)UnityEngine.Random.Range (1, 50);

					float ans = (num1 * num3) + (num2 * num4);
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Round the answer to 2 decimal places";
					subQuestionText.text = "Kumar bought " + num1.ToString () + " kg sugar at Rs. " + num3.ToString () + " per kg and " + num2.ToString () + " kg tea at Rs. " + num4.ToString () + " per kg. How much did he pay in total?(Answer in Rs.)";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} 
			}

			CerebroHelper.DebugLog (Answer);
			userAnswerText = answerButton.gameObject.GetChildByName<Text> ("Text");
			userAnswerText.text = "";
		}

		public override void numPadButtonPressed (int value)
		{
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
