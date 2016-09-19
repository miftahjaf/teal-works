using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro
{ //
	public class FractionsScript5 : BaseAssessment
	{

		public TEXDraw subQuestionText;

		public GameObject MCQ;
		private string Answer;

		private int num,num1,num2,num3,num4,num5,num6;
		private float frac;
		int n,m, lcm = 0,a,b;

		void Start ()
		{

			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "FRA05", "S01", "A01");

			scorestreaklvls = new int[6];
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
				} else if (Answer.Contains ("/")) {
					var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
					correct = MathFunctions.checkFractions (userAnswers, correctAnswers);
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
					increment = 10;
				} else if (Queslevel == 5) {
					increment = 15;
				} else if (Queslevel == 6) {
					increment = 15;
				} else if (Queslevel == 7) {
					increment = 15;
				}

				UpdateStreak (8, 10);

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
			userAnswerLaText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<TEXDraw> ();
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


			subQuestionText.gameObject.SetActive (false);
			MCQ.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (true);
			numPad.SetActive (true);

			answerButton = GeneralButton;


			if (Queslevel > scorestreaklvls.Length) {
				level = Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
			
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {
				
					MCQ.SetActive (true);
					numPad.SetActive (false);
					GeneralButton.gameObject.SetActive (false);
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 6);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num1 = Random.Range (1, 6);
					int ansNum = ((num * num2) + num1);
					string option1 = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					string option2 = "";
					if (Random.Range (1, 3) == 1) {
						int rndNum = Random.Range (1, 10);
						while (rndNum == num) {
							rndNum = Random.Range (1, 10);
						}
						option2 = rndNum.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					} else {
						int rndNum = Random.Range (1, 5);
						while (rndNum == num1) {
							rndNum = Random.Range (1, 5);
						}
						option2 = num.ToString () + " \\frac{" + rndNum.ToString () + "}{" + num2.ToString () + "}";
					}
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert the following improper fraction to a mixed fraction :";
					subQuestionText.text = "\\frac{" + ansNum.ToString () + "}{" + num2.ToString () + "}";
					Answer = option1;
					if (Random.Range (1, 3) == 1) {
						string tmp = option1;
						option1 = option2;
						option2 = tmp;
					}
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = option1;
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = option2;
				} else if (selector == 2) {
				
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 6);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num1 = Random.Range (1, 6);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert the following mixed fraction into an improper fraction :";
					subQuestionText.text = num.ToString () + "\\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					Answer = ((num * num2) + num1).ToString () + "/" + num2.ToString ();
				} else if (selector == 3) {
				
					num = Random.Range (2, 11);
					num1 = Random.Range (num + 2, 21);
					num3 = Random.Range (2, 7);
					num2 = num3 * num1;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Find the missing data.";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "} = " + "\\frac{" + "?" + "}{" + num2.ToString () + "}";
					Answer = (num3 * num).ToString ();
				} else if (selector == 4) {
				
					num = Random.Range (1, 21);
					num1 = Random.Range (2, 21);
					while (num == num1)
						num = Random.Range (1, 21);
					num3 = Random.Range (2, 6);
					subQuestionText.gameObject.SetActive (true);
				
					QuestionText.text = "Find the missing data.";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "} = " + "\\frac{" + (num * num3).ToString () + "}{" + "?" + "}";
					Answer = (num3 * num1).ToString ();
				} else if (selector == 5) {
				
					num = Random.Range (2, 16);
					num3 = Random.Range (2, 10);
					num1 = num * num3;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Write the following fraction in its simplest form :";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "}";
					int hcf = MathFunctions.GetHCF (num, num1);
					num = num / hcf;
					num1 = num1 / hcf;
					Answer = num.ToString () + "/" + num1.ToString ();
				}
			}
			else if (level == 2)
			{
				selector = GetRandomSelector (1, 5);
				if (selector == 1) {
				
					num = Random.Range (2, 11);
					num1 = Random.Range (2, 11);
					num /= MathFunctions.GetHCF (num, num1);
					num2 = Random.Range (2, 11);
					if(Random.Range(0,2) == 0)
					{
						num3 = 2 * num1;
						num5 = 3 * num1;
					}
					else
					{
						num3 = 3 * num1;
						num5 = 2 * num1;
					}
					num4 = Random.Range (2, 11);
					num2 /= MathFunctions.GetHCF(num2, num3);
					num4 /= MathFunctions.GetHCF(num4, num5);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Add :";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "}"+ " + " + "\\frac{" + num2.ToString () + "}{" + num3.ToString () + "} + " + "\\frac{" + num4.ToString () + "}{" + num5.ToString () + "} = " + "?";
					num6 = MathFunctions.GetLCM (num1, num3);
					num6 = MathFunctions.GetLCM (num6, num5);
					num1 = num6 / num1;
					num3 = num6 / num3;
					num5 = num6 / num5;

					n = (num * num1) + (num2 * num3) + (num4 * num5);
					Answer = (n/MathFunctions.GetHCF(n,num6)).ToString () + "/" + (num6/MathFunctions.GetHCF(n,num6)).ToString ();

				} else if (selector == 2) {
				
					num = Random.Range (5, 10);
					num1 = Random.Range (1, 8);
					num2 = Random.Range(num1+1, 11);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					num3 = Random.Range (2, 6);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Subtract :";
					subQuestionText.text = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}" + " - " + num3.ToString ();
					num4 = (num2 * num) + num1;
					num5 = num4 - (num2 * num3);
					int hcf = Mathf.Abs (MathFunctions.GetHCF (num5,num2));
					Answer = (num5/hcf).ToString () + "/" + (num2/hcf).ToString ();

				} else if (selector == 3) {
			
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 8);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					num3 = Random.Range (1, 10);
					num4 = Random.Range (1, 8);
					num5 = Random.Range (num4 + 1, 10);
					while (MathFunctions.GetHCF(num4,num5) > 1)
						num4 = Random.Range (1, 8);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Subtract :";
					subQuestionText.text = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}" + " - " + num3.ToString () + " \\frac{" + num4.ToString () + "}{" + num5.ToString () + "}";
					num6 = MathFunctions.GetLCM (num2, num5);
					num1 = (num * num2) + num1;
					num4 = (num3 * num5) + num4;
					num2 = (num6 / num2);
					num5 = (num6 / num5);
					n = (num1 * num2) - (num4 * num5);
					int hcf = Mathf.Abs (MathFunctions.GetHCF (n,num6));
					Answer = (n/hcf).ToString () + "/" + (num6/hcf).ToString ();
				} else if (selector == 4) {
				
					num = Random.Range (1, 8);
					num1 = Random.Range (num + 1, 10);
					while (MathFunctions.GetHCF(num1,num) > 1)
						num = Random.Range (1, 8);
					num2 = Random.Range (1, 8);
					num3 = Random.Range (num2 + 1, 10);
					while (MathFunctions.GetHCF(num2,num3) > 1)
						num2 = Random.Range (1, 8);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Subtract :";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "} - " + "\\frac{" + num2.ToString () + "}{" + num3.ToString () + "}";
					num4 = MathFunctions.GetLCM (num1, num3);
					num1 = num4 / num1;
					num3 = num4 / num3;
					num = num * num1;
					num2 = num2 * num3;
					num5 = num - num2;
					int hcf = Mathf.Abs (MathFunctions.GetHCF (num5, num4));
					Answer = Answer = (num5/hcf).ToString () + "/" + (num4/hcf).ToString ();
				}
			} else if (level == 3) {
			
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {
					num = Random.Range (1, 11);
					num1 = Random.Range (5, 15);
					while(num == num1)
						num = Random.Range (1, 11);
					num2 = Random.Range (2, 11);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply and answer in lowest terms.";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "} \\times " + num2.ToString ();
					num3 = num * num2;

					int hcf = MathFunctions.GetHCF (num3, num1);
					num1 = num1 / hcf;
					num3 = num3 / hcf;
					Answer = num3.ToString () + "/" + num1.ToString ();
				} else if (selector == 2) {
				
					num = Random.Range (1, 11);
					num1 = Random.Range (5, 15);
					while(num == num1)
						num = Random.Range (1, 11);
					num2 = Random.Range (1, 10);
					num3 = Random.Range (2, 11);
					while(num2 == num3)
						num2 = Random.Range (1, 10);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply and answer in lowest terms.";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "} \\times " + "\\frac{" + num2.ToString () + "}{" + num3.ToString () + "}";
					num4 = num * num2;
					num5 = num1 * num3;
					int hcf = MathFunctions.GetHCF (num4, num5);
					num4 = num4 / hcf;
					num5 = num5 / hcf;
					Answer = num4.ToString () + "/" + num5.ToString ();
				} else if (selector == 3) {
				
					num = Random.Range (1, 11);
					num1 = Random.Range (2, 10);
					while(num == num1)
						num = Random.Range (1, 11);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply and answer in lowest terms.";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "} \\times 0";
					num2 = 0;
					Answer = num2.ToString ();
				} else if (selector == 4) {
				
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 8);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					num3 = Random.Range (1, 10);
					num4 = Random.Range (1, 8);
					num5 = Random.Range (num4 + 1, 10);
					while (MathFunctions.GetHCF(num4,num5) > 1)
						num4 = Random.Range (1, 8);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply and answer in lowest terms.";
					subQuestionText.text = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}" + " \\times " + num3.ToString () + " \\frac{" + num4.ToString () + "}{" + num5.ToString () + "}";
					num1 = (num * num2) + num1;
					num4 = (num3 * num5) + num4;
					num6 = num1 * num4;
					n = num2 * num5;
					int hcf = MathFunctions.GetHCF (num6, n);
					num6 = num6 / hcf;
					n = n / hcf;
					Answer = num6.ToString () + "/" + n.ToString ();
				}
			}
			else if (level == 4)
			{
				selector = GetRandomSelector (1, 5);
				if (selector == 1) {
				
					num = Random.Range (1, 12);
					QuestionText.text = num.ToString () + "/12 of one year is how many months?";
					num1 = (num * 12) / 12;
					Answer = num1.ToString ();
				} else if (selector == 2) {
				
					num = Random.Range (1, 12);
					num = num * 5;
					QuestionText.text = num.ToString () + "/60 of 1 minute is how many seconds?";
					num1 = (num * 60) / 60;
					Answer = num1.ToString ();
				} else if (selector == 3) {
				
					num = Random.Range (1, 6);
					QuestionText.text = num.ToString () + "/7 of 1 week is how many days?";
					num1 = (num * 7) / 7;
					Answer = num1.ToString ();
				} else if (selector == 4) {
				
					num = Random.Range (1, 17);
					num = num * 5;
					QuestionText.text = num.ToString () + "/90 of a right angle is how many degrees?";
					num1 = (num * 90) / 90;
					Answer = num1.ToString ();
				}
			}
			else if (level == 5)
			{
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {
				
					num = Random.Range (2, 15);
					num1 = Random.Range (2, 15);
					while (num == num1 || MathFunctions.GetHCF(num, num1) > 1)
						num1 = Random.Range (2, 15);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Find the reciprocal of the given fraction.";
					subQuestionText.text = "\\frac{" + num.ToString () + "}{" + num1.ToString () + "}";
					Answer = num1.ToString () + "/" + num.ToString ();
				} else if (selector == 2) {
				
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 8);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Find the reciprocal of the given fraction.";
					subQuestionText.text = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					num3 = (num * num2) + num1;
					Answer = num2.ToString () + "/" + num3.ToString ();
				} else if (selector == 3) {
					num1 = Random.Range (1, 8);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					num3 = Random.Range (2, 11);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Give the quotient in the lowest term :";
					subQuestionText.text = " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "} \\div " + num3.ToString ();
					n = (num2 * num3);
					int hcf = MathFunctions.GetHCF (num1, n);
					num1 = num1 / hcf;
					n = n / hcf;
					Answer = num1.ToString () + "/" + n.ToString ();
				} else if (selector == 4) {
			
					num = Random.Range (2, 11);
					num1 = Random.Range (1, 8);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Give the quotient in the lowest term :";
					subQuestionText.text = num.ToString () + " \\div \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					n = num * num2;
					int hcf = MathFunctions.GetHCF (n, num1);
					n = n / hcf;
					num1 = num1 / hcf;
					if(num1 == 1)
						Answer = n.ToString ();
					else
						Answer = n.ToString () + "/" + num1.ToString ();
				} else if (selector == 5) {
				
					num1 = Random.Range (1, 8);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					num3 = Random.Range (1, 8);
					while (num1 == num3)
						num3 = Random.Range (1, 8);
					num4 = Random.Range (num3 + 1, 10);
					while (MathFunctions.GetHCF(num3,num4) > 1)
						num4 = Random.Range (num3 + 1, 10);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Give the quotient in the lowest term :";
					subQuestionText.text = " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "} \\div " + " \\frac{" + num3.ToString () + "}{" + num4.ToString () + "}";
					n = num1 * num4;
					m = num2 * num3;
					int hcf = MathFunctions.GetHCF (n, m);
					n = n / hcf;
					m = m / hcf;
					if(m == 1)
						Answer = n.ToString ();
					else
						Answer = n.ToString () + "/" + m.ToString ();
				}
			} else if (level == 6) {
			
				selector = GetRandomSelector (1, 6);

				if (selector == 1) {

					num1 = Random.Range (1,11)*3;
					num2 = Random.Range (1, 24);
					QuestionText.text = "It takes 1/" + num1.ToString () + " of an hour for Jamie to knit one sweater. If she works for " + num2.ToString () + " hours, how many sweaters can she knit?";
					Answer = (num2 * num1).ToString ();
				} else if (selector == 2) {
				
					num = Random.Range (1, 11);
					num1 = Random.Range (2, 11);
					while (num == num1)
						num = Random.Range (1, 11);
					num2 = Random.Range (1, 11);
					num3 = Random.Range (2, 11);
					while (num2 == num3)
						num2 = Random.Range (1, 11);
					QuestionText.text = "Mary used " + num.ToString () + "/" + num1.ToString () + " metres of thread to stitch 1 dress and " + num2.ToString () + "/" + num3.ToString () + " metres to make another. How many metres of thread did she use for both dresses?";
					lcm = MathFunctions.GetLCM (num1, num3);
					num1 = lcm / num1;
					num3 = lcm / num3;
					num = num * num1;
					num2 = num2 * num3;
					num4 = num + num2;
					int hcf = MathFunctions.GetHCF (num4, lcm);
					Answer = (num4/hcf).ToString () + "/" + (lcm/hcf).ToString ();
				} else if (selector == 3) {
				
					num = Random.Range (2, 11);
					num1 = Random.Range (2, 11);
					while(num == num1)
						num = Random.Range (2, 11);
					num2 = num * Random.Range (1, 11) * 10;
					QuestionText.text = num.ToString () + "/" + num1.ToString () + " of a number is " + num2.ToString () + ", what is the number?";
					num3 = (num2 * num1) / num;
					Answer = num3.ToString ();
				} else if (selector == 4) {
				
					num = Random.Range (2, 11);
					num1 = Random.Range (1, 8);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					num3 = Random.Range (2, 11);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Give the quotient in the lowest term :";
					subQuestionText.text = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "} \\div " + num3.ToString ();
					num4 = (num * num2) + num1;
					num5 = num3 * num2;
					int hcf = MathFunctions.GetHCF (num4, num5);
					num4 = num4 / hcf;
					num5 = num5 / hcf;
					if (num5 == 1)
						Answer = num4.ToString ();
					else
						Answer = num4.ToString () + "/" + num5.ToString();
				} else if (selector == 5) {
				
					num = Random.Range (2, 11);
					num1 = Random.Range (1, 8);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF(num1,num2) > 1)
						num1 = Random.Range (1, 8);
					num3 = Random.Range (2, 11);
					num4 = Random.Range (1, 8);
					num5 = Random.Range (num4 + 1, 10);
					while (MathFunctions.GetHCF(num4,num5) > 1)
						num4 = Random.Range (1, 8);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Give the quotient in the lowest term where required:";
					subQuestionText.text = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "} \\div " + num3.ToString () + " \\frac{" + num4.ToString () + "}{" + num5.ToString () + "}";
					n = (num2 * num) + num1;
					m = (num5 * num3) + num4;
					n = n * num5;
					m = m * num2;
					int hcf = MathFunctions.GetHCF (n, m);
					n = n / hcf;
					m = m / hcf;
					if (n == 1)
						Answer = n.ToString ();
					else
						Answer = n.ToString () + "/" + m.ToString();

				}
			}
			CerebroHelper.DebugLog ("a"+level);
			CerebroHelper.DebugLog ("b"+ selector);
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
			} else if (value == 13) {   // .
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
