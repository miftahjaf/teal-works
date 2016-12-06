using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro
{
	public class OrderOfOperations5 : BaseAssessment
	{

		public TEXDraw subQuestionTEX;
		private string Answer;


		void Start ()
		{

			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "OOO05", "S01", "A01");

			scorestreaklvls = new int[3];
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
				if (userAnswerLaText.text == Answer) {
					correct = true;
				} else {
					correct = false;
				}
			} else {
				if (Answer.Contains ("=")) {
					string correctAnswer = Answer.Split (new string[] { "=" }, System.StringSplitOptions.None) [1];
					if (userAnswerText.text == correctAnswer) {
						correct = true;
					} else {
						Answer = correctAnswer;
						correct = false;
					}
				} else if (Answer.Contains (",")) {
					var correctAnswers = Answer.Split (new string[] { "," }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "," }, System.StringSplitOptions.None);
					if (correctAnswers.Length != userAnswers.Length) {
						correct = false;
					} else {
						for (var i = 0; i < correctAnswers.Length; i++) {
							var correctFraction = correctAnswers[i].Split (new string[] { "/" }, System.StringSplitOptions.None);
							var userFraction = userAnswers[i].Split (new string[] { "/" }, System.StringSplitOptions.None);
							if (!MathFunctions.checkFractions (userFraction, correctFraction)) {
								correct = false;
								break;
							}
						}
					}
				} else if (Answer.Contains ("/")) {
					var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
					correct = MathFunctions.checkFractions (userAnswers, correctAnswers);
				} else {
					if (userAnswerText.text == Answer) {
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
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations (2, GoLoopType.PingPong);
			var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1));
			GoTween tween = null;
			if (userAnswerText != null) {
				userAnswerText.color = MaterialColor.green800;
				tween = new GoTween (userAnswerText.gameObject.transform, 0.2f, config);
			} 
			if (userAnswerLaText != null) {
				userAnswerLaText.color = MaterialColor.green800;
				tween = new GoTween (userAnswerLaText.gameObject.transform, 0.2f, config);
			}

			flow.insert (0f, tween);
			flow.play ();
			yield return new WaitForSeconds (1f);

			if (userAnswerText != null) {
				userAnswerText.color = MaterialColor.textDark;
			} 
			if (userAnswerLaText != null) {
				userAnswerLaText.color = MaterialColor.textDark;
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
				Go.to (userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			} 
			if (userAnswerLaText != null) {
				userAnswerLaText.color = MaterialColor.red800;
				Go.to (userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			}


			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				if (numPad.activeSelf) {				// is not MCQ type question
					userAnswerText.text = "";
				}
				if (userAnswerText != null) {
					userAnswerText.color = MaterialColor.textDark;
				} 
				if (userAnswerLaText != null) {
					userAnswerLaText.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
			} else {
				if (numPad.activeSelf) {				// is not MCQ type question
					userAnswerText.text = Answer.ToString ();
					userAnswerText.color = MaterialColor.green800;
				} else {
					userAnswerLaText.color = MaterialColor.textDark;
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

			subQuestionTEX.gameObject.SetActive(true);
			GeneralButton.gameObject.SetActive (true);
			numPad.SetActive (true);

			if (Queslevel > scorestreaklvls.Length) {
				level = Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				
				selector = GetRandomSelector (1, 6);
				QuestionText.text = "Solve :";

				if (selector == 1) {
					
					int num1 = Random.Range (11, 100);
					int num2 = Random.Range (11, 100);
					int num3 = Random.Range (11, 100);
					while (num1 % 10 == 0)
						num1 = Random.Range (11, 100);
					while (num2 % 10 == 0 || num2 == num1)
						num2 = Random.Range (11, 100);
					while (num3 % 10 == 0 || num3 == num1 || num3 == num2)
						num3 = Random.Range (11, 100);
					
					float coeff1 = (float)num1 / 10f;
					float coeff2 = (float)num2 / 10f;
					float coeff3 = (float)num3 / 10f;
					int sign1 = 1;
					if (Random.Range (1, 3) == 1)
						sign1 *= -1;
					subQuestionTEX.text = coeff1 + ((sign1 > 0) ? (" + ") : (" - ")) + coeff2 + ((sign1 < 0) ? (" + ") : (" - ")) + coeff3;
					float ans = coeff1 + sign1 * coeff2 - sign1 * coeff3;
					ans = MathFunctions.GetRounded (ans, 1);
					Answer = ans.ToString ();

				} else if (selector == 2) {

					int num1 = Random.Range (10, 100);
					int num2 = Random.Range (2, 6);
					int num3 = Random.Range (6, 11);
					int num4 = Random.Range (10, 100);
					while (num1 ==num4)
						num4 = Random.Range (10, 100);
					int sign1 = 1;
					if (Random.Range (1, 3) == 1)
						sign1 *= -1;
					subQuestionTEX.text = num1 + ((sign1 > 0) ? (" + ") : (" - ")) + num2 + " \\times " + num3 + ((sign1 < 0) ? (" + ") : (" - ")) + num4;
					int ans = num1 + sign1 * num2 * num3 - sign1 * num4;
					Answer = ans.ToString ();

				} else if (selector == 3) {
					
					int num1 = Random.Range (10, 100);
					int num2 = Random.Range (10, 100);
					int num3 = Random.Range (2, 11);
					while (num2 % num3 != 0)
						num2 = Random.Range (10, 100);
					int sign1 = 1;
					if (Random.Range (1, 3) == 1)
						sign1 *= -1;
					subQuestionTEX.text = num1 + ((sign1 > 0) ? (" + ") : (" - ")) + num2 + " \\div " + num3 ;
					int ans = num1 + sign1 * num2 / num3;
					Answer = ans.ToString ();

				} else if (selector == 4) {

					int num1 = Random.Range (10, 100);
					int num2 = Random.Range (2, 11);
					int num3 = Random.Range (2, 6);
					while (num1 % (num2 * num3) != 0 || num1 / (num2 * num3) == 1)
						num1 = Random.Range (10, 100);
					subQuestionTEX.text = num1 + " \\div " + num2 + " \\div " + num3;
					int ans = num1 / (num2 * num3);
					Answer = ans.ToString ();

				} else if (selector == 5) {

					int num1 = Random.Range (2, 8);
					int num2 = Random.Range (num1 + 1, 11);
					int num3 = Random.Range (2, num2);
					subQuestionTEX.text = "\\frac{" + num1 + "}{" + num2 + "} + \\frac{" + num3 + "}{" + (num2 * num3) + "} \\div \\frac{" + num1 + "}{" + num2 + "} + \\frac{" + num3 + "}{" + num2 + "}"; 
					int ans_num = num1 * (num1 + num3) + num2;
					int ans_den = num1 * num2;
					int hcf = MathFunctions.GetHCF (ans_num, ans_den);
					ans_num /= hcf;
					ans_den /= hcf;
					if (ans_den == 1)
						Answer = ans_num.ToString ();
					else
						Answer = ans_num.ToString () + "/" + ans_den.ToString ();

				}
			} 
			else if (level == 2) {
				
				selector = GetRandomSelector (1, 6);
				QuestionText.text = "Solve :";

				if (selector == 1) {

					int num1 = Random.Range (10, 100);
					int num2 = Random.Range (2, 8);
					int num3 = Random.Range (num2 + 1, 11);
					int num4 = Random.Range (2, 20) * num3;
					int num5 = Random.Range (2, 11);
					while ((num1 * num5 * num3) % (num4 * num2) != 0){
						num1 = Random.Range (10, 100);
						num5 = Random.Range (2, 11);
					}
					subQuestionTEX.text = num1 + " \\div \\frac{" + num2 + "}{" + num3 + "} of " + num4 + " \\times " + num5; 
					int ans = (num1 * num5 * num3) / (num4 * num2);
					Answer = ans.ToString ();

				} else if (selector == 2) {

					int num1 = Random.Range (10, 100);
					int num2 = Random.Range (10, 100);
					int num3 = Random.Range (1, 6);
					int num4 = Random.Range (num3 + 1, 10);
					int num5 = Random.Range (2,10) * num4;
					while ((num2 * num4) % (num3 * num5) != 0)
						num2 = Random.Range (10, 100);
					subQuestionTEX.text = num1 + " + " + num2 + " \\div \\frac{" + num3 + "}{" + num4 + "} of " + num5; 
					int ans = num1 + (num2 * num4) / (num3 * num5);
					Answer = ans.ToString ();

				} else if (selector == 3) {

					int num1 = Random.Range (1, 4);
					int num2 = Random.Range (1, 4);
					int num3 = Random.Range (num2 + 1, 6);
					while (MathFunctions.GetHCF (num3, num2) != 1)
						num3 = Random.Range (num2 + 1, 6);
					
					int num4 = Random.Range (2, 10);
					int num5 = Random.Range (1, 4);
					int num6 = Random.Range (num5 + 1, 6);
					while (MathFunctions.GetHCF (num5, num6) != 1)
						num6 = Random.Range (num5 + 1, 6);
					
					int num7 = Random.Range (1, 4);
					int num8 = Random.Range (1, 4);
					int num9 = Random.Range (num8 + 1, 6);
					while (MathFunctions.GetHCF (num8, num9) != 1)
						num9 = Random.Range (num8 + 1, 6);
					
					subQuestionTEX.text = num1 + "\\frac{" + num2 + "}{" + num3 + "} \\times " + num4 + " - \\frac{" + num5 + "}{" + num6 + "} of " + num7 + "\\frac{" + num8 + "}{" + num9 + "}"; 
					int ans_num = num4 * num6 * num9 * (num1 * num3 + num2) - num3 * num5 * (num9 * num7 + num8);
					int ans_den = num3 * num6 * num9;
					int hcf = MathFunctions.GetHCF (ans_num, ans_den);
					ans_num /= hcf;
					ans_den /= hcf;
					if (ans_den == 1)
						Answer = ans_num.ToString ();
					else
						Answer = ans_num.ToString () + "/" + ans_den.ToString ();

				} else if (selector == 4) {

					int num1 = Random.Range (10, 26);
					int num2 = Random.Range (1, 10);
					int num5 = Random.Range (1, 10);
					int num4 = Random.Range (num5 + 1, 15);
					int num3 = Random.Range (num4 - num5 + 1, 20);
					subQuestionTEX.text = num1 + " + " + num2 + " - \\lbrace{" + num3 + " - ({" + num4 + " - " + num5 + "})}\\rbrace"; 
					int ans = num1 + num2 - (num3 - (num4 - num5));
					Answer = ans.ToString ();

				} else if (selector == 5) {

					int num1 = Random.Range (10, 100);
					int num2 = Random.Range (2, 6);
					int num3 = Random.Range (6, 11);
					int num4 = Random.Range (2, 21);
					while(num1 % (num2 * num3 + num4) != 0)
						num1 = Random.Range (10, 100);
					subQuestionTEX.text = num1 + " \\div \\lbrace{({" + num2 + " \\times " + num3 + "}) + " + num4 + "}\\rbrace";
					int ans = num1 / (num2 * num3 + num4);
					Answer = ans.ToString ();

				}
			} else if (level == 3) {

				selector = GetRandomSelector (1, 6);
				QuestionText.text = "Solve :";

				if (selector == 1) {

					int num1 = Random.Range (10, 200);
					int num2 = Random.Range (1, 200);
					while (num2 % 10 == 0)
						num2 = Random.Range (1, 100);
					int num3 = Random.Range (1, 100);
					while (num3 % 10 == 0)
						num3 = Random.Range (1, 100);
					int num5 = Random.Range (1, 100);
					while (num5 % 10 == 0)
						num5 = Random.Range (1, 100);
					int num4 = Random.Range (2,6) * num5;
					int num6 = Random.Range (1, 50);
					while (num6 % 10 == 0)
						num6 = Random.Range (1, 50);
					int num7 = Random.Range (1, 50);
					while (num7 % 10 == 0 || num7 == num6)
						num7 = Random.Range (1, 50);

					int num8 = num2 - (num3 - (10 * num4 / num5 - num6 + num7));
					while (num8 <= 0) {
						num2 = Random.Range (1, 200);
						while (num2 % 10 == 0)
							num2 = Random.Range (1, 100);
						num8 = num2 - (num3 - (10 * num4 / num5 - num6 + num7));
					}
					while((100 * num1) % num8 != 0)
						num1 = Random.Range (10, 200);
					
					float coeff1 = (float)num1 / 10f;
					float coeff2 = (float)num2 / 10f;
					float coeff3 = (float)num3 / 10f;
					float coeff4 = (float)num4 / 10f;
					float coeff5 = (float)num5 / 10f;
					float coeff6 = (float)num6 / 10f;
					float coeff7 = (float)num7 / 10f;
					subQuestionTEX.text = coeff1 + " \\div " + "[{" + coeff2 + " - \\lbrace{" + coeff3 + " - ({" + coeff4 + " \\div " + coeff5 + " - " + coeff6 + " + " + coeff7 + "})}\\rbrace}]";
					float ans = (float)num1 / (float)num8;
					ans = MathFunctions.GetRounded (ans, 4);
					Answer = ans.ToString ();

				} else if (selector == 2) {

					int den1 = Random.Range (2, 5);
					int num1 = Random.Range (1, 2 * den1);
					int den2 = den1 * Random.Range (2, 5);
					int num2 = Random.Range (1, den1);
					int num3 = Random.Range (1, 2 * den1);
					int num4 = Random.Range (1, 6);
					int den3 = Random.Range (2, 5) * num4;

					subQuestionTEX.text = "\\frac{" + num1 + "}{" + den2 + "} + \\lbrace{\\frac{" + num2 + "}{" + den1 + "} of ({\\frac{" + num3 + "}{" + den2 + "} + \\frac{" + num4 + "}{" + den3 + "}})}\\rbrace"; 
					int ans_num = num1 * den1 * den3 + num2 * (num3 * den3 + den2 * num4);
					int ans_den = den1 * den2 * den3;
					int hcf = MathFunctions.GetHCF (ans_num, ans_den);
					ans_num /= hcf;
					ans_den /= hcf;
					if (ans_den == 1)
						Answer = ans_num.ToString ();
					else
						Answer = ans_num.ToString () + "/" + ans_den.ToString ();

				} else if (selector == 3) {

					int num1 = Random.Range (10, 31);
					int num2 = Random.Range (1, 21);
					int num3 = Random.Range (10 ,21);
					int num4 = Random.Range (2, 11);
					int num5 = num4 * Random.Range (2, 10);

					while(((num2 + num3) % (num5 / num4)) != 0)
						num2 = Random.Range (1, 21);
					subQuestionTEX.text = num1 + " - ({" + num2 + " + " + num3 + "}) \\div \\frac{1}{" + num4 + "} of " + num5;
					int ans = num1 - (num2 + num3) / (num5 / num4);
					Answer = ans.ToString ();

				} else if (selector == 4) {

					int num1 = Random.Range (2, 5);
					int num2 = Random.Range (1, 10);
					int num3 = Random.Range (1, 10);
					int num5 = Random.Range (1, 10);
					int num4 = Random.Range (2, 6) * num5;
					int den3 = Random.Range (2, 10);
					while (num3 == den3)
						den3 = Random.Range (2, 10);
					int den5 = Random.Range (2, 10);
					while (num5 == den5)
						den5 = Random.Range (2, 10);
					int den4 = Random.Range (2, 6) * den5;

					subQuestionTEX.text = num1 + " \\times [{" + num2 + " + \\lbrace{\\frac{" + num3 + "}{" + den3 + "} of ({\\frac{" + num4 + "}{" + den4 + "} \\div \\frac{" + num5 + "}{" + den5 + "}})}\\rbrace}]";
					int ans_num = num1 * (num2 * den3 * den4 * num5 + num3 * num4 * den5);
					int ans_den = num5 * den4 * den3;
					int hcf = MathFunctions.GetHCF (ans_num, ans_den);
					ans_num /= hcf;
					ans_den /= hcf;
					if (ans_den == 1)
						Answer = ans_num.ToString ();
					else
						Answer = ans_num.ToString () + "/" + ans_den.ToString ();

				} else if (selector == 5) {

					int num1 = Random.Range (1, 4);
					int num2 = Random.Range (1, 4);
					int num3 = Random.Range (num2 + 1, 6);
					while (MathFunctions.GetHCF (num3, num2) != 1)
						num3 = Random.Range (num2 + 1, 6);

					int num4 = Random.Range (1, 10);
					int num5 = Random.Range (1, 4);
					int num6 = Random.Range (num5 + 1, 6);
					while (MathFunctions.GetHCF (num5, num6) != 1)
						num6 = Random.Range (num5 + 1, 6);

					int num7 = Random.Range (1, 4);
					int num8 = Random.Range (num7 + 1, 6);

					int num9 = Random.Range (1, 10);
					int num10 = Random.Range (1, 4);
					int num11 = Random.Range (num10 + 1, 6);
					while (MathFunctions.GetHCF (num10, num11) != 1)
						num11 = Random.Range (num10 + 1, 6);
					
					int num13 = Random.Range (2, 6);
					int num12 = Random.Range (num13 +1, 16);
					int num14 = Random.Range (1, 6);

					subQuestionTEX.text = num1 + "\\frac{" + num2 + "}{" + num3 + "} + [{" + num4 + "\\frac{" + num5 + "}{" + num6 + "} + \\frac{" + num7 + "}{" + num8 + "} \\times \\lbrace{" + num9 + "\\frac{" + num10 + "}{" + num11 + "} - ({\\frac{" + num12 + "}{" + num13 + "} - " + num14 + "})}\\rbrace}]";
					int ans_num = num8 * num11 * num13 * (num6 * (num1 * num3 + num2) + num3 * (num4 * num6 + num5));
					ans_num += num3 * num6 * num7 * (num13 * (num9 * num11 + num10) - num11 * (num12 - num13 * num14)); 	
					int ans_den = num3 * num6 * num8 * num11 * num13;
					int hcf = MathFunctions.GetHCF (ans_num, ans_den);
					ans_num /= hcf;
					ans_den /= hcf;
					if (ans_den == 1)
						Answer = ans_num.ToString ();
					else
						Answer = ans_num.ToString () + "/" + ans_den.ToString ();
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
			} else if (value == 12) {   // /
				if (checkLastTextFor (new string[1]{ "/" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			} else if (value == 13) {   // ,
				if (checkLastTextFor (new string[1]{ "." })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			} else if (value == 14) {   // -
				if (checkLastTextFor (new string[1]{ "-" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
		}
	}
}
