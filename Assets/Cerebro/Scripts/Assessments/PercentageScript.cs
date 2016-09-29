using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class PercentageScript : BaseAssessment {

		public TEXDraw subQuestionText;
		private string Answer;
		private int num, num1, num2,num3,income,num4;
		private float per,per1;

		//public GameObject MCQ;

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "PER07", "S01", "A01");

			scorestreaklvls = new int[4];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}




			levelUp = false;

			Answer = "0";
			GenerateQuestion ();
		}

		public override void SubmitClick(){
			if (ignoreTouches) {
				return;
			}
			if (numPad.activeSelf && userAnswerText.text == "") {
				return;
			}

			questionsAttempted++;
			updateQuestionsAttempted ();

			CerebroHelper.DebugLog (userAnswerText.text);
			CerebroHelper.DebugLog (Answer);
			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;
			float answer = 0;
		/*	if(float.TryParse(userAnswerText.text,out answer)) {
				answer = float.Parse (userAnswerText.text);
			}*/


			 if(Answer.Contains(":")){

				var correctAnswers = Answer.Split (new string[] { ":" }, System.StringSplitOptions.None);
				var userAnswers = userAnswerText.text.Split (new string[] { ":" }, System.StringSplitOptions.None);
				correct = MathFunctions.checkFractions (userAnswers, correctAnswers);
			
			
			} else if(Answer.Contains("%")) {

				if (userAnswerText.text == Answer)
					correct = true;
				else
					correct = false;


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

			if(correct == true) {
				if (Queslevel == 1) {
					increment = 5;
				} else if (Queslevel == 2) {
					increment = 10;
				} else if (Queslevel == 3) {
					increment = 15;
				} else if (Queslevel == 4) {
					increment = 15;
				} else if (Queslevel == 5) {
					increment = 15;
				}
				UpdateStreak(6,10);

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

		protected override IEnumerator ShowWrongAnimation() {
			userAnswerText.color = MaterialColor.red800;
			Go.to( userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake( new Vector3( 0, 0, 20 ), GoShakeType.Eulers ) );
			yield return new WaitForSeconds (0.5f);
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

					userAnswerText.text = Answer.ToString ();
					userAnswerText.color = MaterialColor.green800;
				} else {

					userAnswerText.color = MaterialColor.textDark;
				}
			}
		//	CerebroHelper.DebugLog ("hie");
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
			QuestionText.gameObject.SetActive (true);

			numPad.SetActive (true);
			//MCQ.gameObject.SetActive (false);
			answerButton = null;
			userAnswerText = null;
			subQuestionText.gameObject.SetActive (false);

			level = Queslevel;

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				selector = GetRandomSelector (1, 5);
				if (selector == 1) {
					//07
					subQuestionText.gameObject.SetActive (true);
					num = Random.Range (2, 16);
					num1 = Random.Range (num + 1, 20);
					QuestionText.text = "Express as percent (round off to two decimal places) :";
					subQuestionText.text = num + " out of " + num1;
					per = ((float)num / (float)num1) * 100;
					per = MathFunctions.GetRounded (per, 2);
					Answer = per.ToString () + "%";

				} else if (selector == 2) {
					num = Random.Range (1, 4);
					num1 = Random.Range (1, 6);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num1 = Random.Range (1, 6);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express as ratio (simplest form) :";
					subQuestionText.text = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "} %";
					num4 = (num * num2) + num1;
					num3 = num2 * 100;
					int hcf = MathFunctions.GetHCF (num4, num3);
					num4 = num4 / hcf;
					num3 = num3 / hcf;

					Answer = num4.ToString () + ":" + num3.ToString ();
				
				} else if (selector == 3) {
					//not accepting 090
					num = Random.Range (20, 100);
					num = num * 100;
					num1 = Random.Range (2, 10);
					num1 = num1 * 5;
					QuestionText.text = "A broker purchases goods worth Rs. " + num.ToString () + ". What is his commission (in Rs.) at " + num1.ToString () + "%?";
					per = (float)(num * num1) / 100;
					Answer = per.ToString ();

				} else if (selector == 4) {
				
					num = Random.Range (1, 10);
					num = num * 5;
					num1 = Random.Range (10, 50);
					while(num == num1)
						num1 = Random.Range (10, 50);
					QuestionText.text = num.ToString () + "% of a number is " + num1.ToString () + ". Find the number (round off to two decimal places).";
					per = (float)(num1 * 100) / (float)num;
					per = MathFunctions.GetRounded (per, 2);
					Answer = per.ToString ();
				}

			} else if (level == 2) {
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				selector = GetRandomSelector (1, 6);

				if (selector == 1) {

					num1 = Random.Range (2, 10);
					num1 = num1 * 10;
					QuestionText.text = "Speed of car B is " + num1.ToString () + "% of the speed of car A. What is the ratio of the speed of car B to the speed of car A?";
					int hcf = MathFunctions.GetHCF (num1, 100);
					num1 /= hcf;
					Answer = num1.ToString () + ":" + (100/hcf).ToString ();

				} else if (selector == 2) {
				
					num = 5 * Random.Range (1, 10);
					num1 = 10 * Random.Range (50, 100);
					while ((num * num1) % 100 != 0)
						num = 5 * Random.Range (1, 10);
					num2 = num1 - (num * num1) / 100;
					QuestionText.text = "The value of a cycle is reduced by " + num.ToString () + "%. If the present value is Rs. " + num2.ToString () + ", what was its value before reduction?";
					Answer = num1.ToString ();

				} else if (selector == 3) {
	
					num = Random.Range (30, 80);
					num1 = Random.Range (num - 20, num + 20);
					while (num == num1)
						num1 = Random.Range (num - 20, num + 20);
					QuestionText.text = "Amit obtained " + num.ToString () + " and " + num1.ToString () + " marks respectively in two monthly tests. What is the percentage change in his marks (round off to two decimal places)?";
					per = ((float)(num - num1) * 100f) /(float) num;
					per = MathFunctions.GetRounded (Mathf.Abs (per), 2);
					Answer = per.ToString () + "%";
				
				} else if (selector == 4) {
				
					num = 5 * Random.Range (1, 10);
					num1 = 10 * Random.Range (50, 100);
					while ((num * num1) % 100 != 0)
						num = 5 * Random.Range (1, 10);
					num2 = num1 - (num * num1) / 100;
					QuestionText.text = "A number when decreased by " + num.ToString () + "% gives " + num2.ToString () + ". What is the number?";
					Answer = num1.ToString ();

				} else if (selector == 5) {
					//not accepting decimal
					num = 5 * Random.Range (1, 10);
					num1 = 10 * Random.Range (50, 100);
					while ((num * num1) % 100 != 0)
						num = 5 * Random.Range (1, 10);
					num2 = num1 + (num * num1) / 100;
					QuestionText.text = "The price of mangoes is increased by " + num.ToString () + "%. If the price before the increase was Rs. " + num1.ToString () + " per kg. What is the present price per kg (in Rs.)?";
					Answer = num2.ToString ();

				}

			} else if (level == 3) {

				selector = GetRandomSelector (1, 5);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;

				if (selector == 1) {
					num = Random.Range (1, 11);
					num1 = Random.Range (2, 11);
					while (MathFunctions.GetHCF (num, num1) > 1)
						num = Random.Range (1, 11);
					QuestionText.text = "A trader mixes two blends of tea, Darjeeling and Assam, in the ratio " + num.ToString () + " : " + num1.ToString () + ". What percent of the total mixture is Darjeeling tea (round off to 2 decimal places)?";
					per = (((float)num /(float) (num + num1)) * 100);
					per = MathFunctions.GetRounded (per, 2);
					Answer = per.ToString () + "%";

				} else if (selector == 2) {
				
					num = Random.Range (1, 10);
					num = num * 5;
					QuestionText.text = "The side length of a square is increased by " + num.ToString () + "%. What is the percentage increase in its area (round off to two decimal places)?";
					per = (float)(100 + num) / 100;
					per = (per * per - 1) * 100;
					per = MathFunctions.GetRounded (per, 2);
					Answer = per.ToString () + "%";

				} else if (selector == 3) {
				
					num = Random.Range (5, 30);
					num1 = 1000 * Random.Range (50, 100);
					num2 = num1 - (num * num1) / 100;
					QuestionText.text = "A computer costs Rs. " + num2.ToString () + " this year. It costed Rs. " + num1.ToString () + " a year ago. What is the percentage change in its value?";
					Answer = num.ToString () + "%";

				} else if (selector == 4) {
				
					num = Random.Range (2, 11);
					num = num * 5;
					num1 = Random.Range (5, 21);
					num1 = num1 * 10;
					num2 = 5 * Random.Range (2, 11);
					while (num1 == num2)
						num2 = 5 * Random.Range (2, 11);
					while ((num1 * num2) % num != 0)
						num = 5 * Random.Range (2, 11);
					QuestionText.text = num.ToString () + "% of an amount is Rs. " + num1.ToString () + ". Calculate " + num2.ToString () + "% of the same amount (in Rs.).";
					num3 = (num1 * num2) / num;
					Answer = num3.ToString ();
				}
			} else if (level == 4) {

				selector = GetRandomSelector (1, 5);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;

				if (selector == 1) {

					num1 = 5 * Random.Range (1, 10);
					num2 = 5 * Random.Range (1, 10);
					num3 = 5 * Random.Range (1, 10);
					num4 = 500 * Random.Range (10, 65);
					while ((((100 - num1) * (100 - num2) * num3 * num4)) % 1000000 != 0)
						num4 = 500 * Random.Range (10, 65);
					num = ((100 - num1) * (100 - num2) * num3 * num4) / 1000000;
					QuestionText.text = "Harvey spends " + num1.ToString () + "% of his income in household expenses, " + num2.ToString () + "% of the remainder in personal necessities and " + num3.ToString () + "% of the amount left in his savings. Find his income (in Rs.) if the amount he saves per month is Rs. " + num.ToString ()+".";
					Answer = num4.ToString ();

				} else if (selector == 2) {
				

					num1 = 5 * Random.Range (1, 10);
					num2 = 5 * Random.Range (1, 20);
					num4 = 25 * Random.Range (40, 400);
					while ((((100 - num1) * num4)) % 100 != 0 || (((100 - num1) * num2 * num4)) % 10000 != 0) 
						num4 = 25 * Random.Range (40, 400);
					num = ((100 - num1) * num2 * num4) / 10000;
					QuestionText.text = "A school has " + num4.ToString () + " students. In an election of student council " + num1.ToString () + "% did not vote. Rohit received " + num2.ToString () + "% of the total votes polled. How many votes did Rohit get?";
					Answer = num.ToString ();

				} else if (selector == 3) {
				
					num = Random.Range (2, 10);
					num1 = Random.Range (2, 10);
					while (num == num1)
						num = Random.Range (2, 10);
					num2 = (num + num1) * Random.Range (5, 11);
					num3 = Random.Range (5, 10);
					QuestionText.text = "In a group of " + num2.ToString() + " children the ratio of the number of boys to the number of girls is " + num.ToString () + " : " + num1.ToString () + ". If " + num3 + " more girls join the group, what is the decrease in percentage of boys (round off to two decimal places)?";
					per = ((float)num / (float)(num + num1)) * num2;
					per1 = ((float)per / (float)(num2 + num3)) * 100;
					per = ((float)per / (float)num2) * 100;
					per = per - per1;
					per = MathFunctions.GetRounded (per, 2);
					Answer = per.ToString () + "%";

				} else if (selector == 4) {
				
					num = 5 * Random.Range (1, 10);
					num1 = 1000 *  Random.Range (15, 50);
					while (((100 + num) * (100 + num) * num1) % 10000 != 0)
						num1 = 1000 *  Random.Range (15, 50);
					QuestionText.text = "Mr. Louis receives an annual increment of " + num.ToString () + "% on his base salary. Last year his base salary was Rs. " + num1.ToString () + ". The new base salary equals the old base salary added to the increment. So, how much increment will he receive at the end of this year?";
					per = num1 + (num * num1) / 100;
					per = (per * num) / 100;
					Answer = per.ToString ();

				}
			}

			CerebroHelper.DebugLog ("level" + level);
			CerebroHelper.DebugLog ("selector" + selector);
			CerebroHelper.DebugLog (Answer);

			if (answerButton != null) {
				userAnswerText = answerButton.gameObject.GetChildByName<Text> ("Text");
				userAnswerText.text = "";
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
			} else if (value == 12) {   // .
				if (checkLastTextFor (new string[1]{ "." })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			}
			else if (value == 13) {   // :
				if (checkLastTextFor (new string[1]{ ":" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ":";
			}
			else if (value == 14) {   // :
				if (checkLastTextFor (new string[1]{ "%" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "%";
			}
		}

	}
}
