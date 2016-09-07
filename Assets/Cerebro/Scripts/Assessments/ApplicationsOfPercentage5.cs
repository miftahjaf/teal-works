using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class ApplicationsOfPercentage5 : BaseAssessment {

		public TEXDraw subQuestionText;
		public GameObject[] OptionsInput;
		private string Answer;
		private int num, num1, num2,num3,income,num4;
		private float per,per1;

		private bool TwoOptionsInputEnabled, Option1Selected, Option1Correct;
		private string[] AnswerArray;

		//public GameObject MCQ;

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "AOP05", "S01", "A01");

			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}




			levelUp = false;

			Answer = "0";
			GenerateQuestion ();
		}

		bool checkArrayValues(string[] A, string[] B)
		{
			if (A.Length != B.Length)
			{
				CerebroHelper.DebugLog(A.Length);
				CerebroHelper.DebugLog(B.Length);
				CerebroHelper.DebugLog("Length not equal");
				return false;
			}
			for (var i = 0; i < A.Length; i++)
			{
				var found = false;
				for (var j = 0; j < B.Length; j++)
				{
					if (A[i] == B[j])
					{
						CerebroHelper.DebugLog(A[i] + "found");
						found = true;
						break;
					}
				}
				if (!found)
				{
					CerebroHelper.DebugLog(A[i] + " not found");
					return false;
				}
			}
			return true;
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

			for (int i = 0; i < 2; i++) {
				OptionsInput [i].transform.FindChild ("border").gameObject.SetActive (false);
			}
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
			
			
			}
			else {
				if (Option1Selected == Option1Correct) {
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
					if (TwoOptionsInputEnabled) {
						if (Option1Correct) {
							userAnswerText = OptionsInput [0].transform.FindChild ("Text").GetComponent<Text> ();
						} else {
							userAnswerText = OptionsInput [1].transform.FindChild ("Text").GetComponent<Text> ();	
						}
					}
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

		public void SetFocusOptionInput(int no)
		{
			for (int i = 0; i < 2; i++) {
				OptionsInput[i].transform.FindChild("border").gameObject.SetActive(false);
				OptionsInput [i].transform.FindChild ("Text").GetComponent<Text> ().text = "";
			}
			OptionsInput[no].transform.FindChild("border").gameObject.SetActive(true);
			userAnswerText = OptionsInput [no].transform.FindChild ("Text").GetComponent<Text> ();
			Option1Selected = no == 0 ? true : false;
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
			TwoOptionsInputEnabled = false;
			OptionsInput [0].transform.parent.gameObject.SetActive (false);
			for (int i = 0; i < 2; i++) {
				OptionsInput[i].transform.FindChild("border").gameObject.SetActive(false);
				OptionsInput [i].transform.FindChild ("Text").GetComponent<Text> ().text = "";
				OptionsInput [i].transform.FindChild ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

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
					num = Random.Range (2, 11);
					num = num * 5;
					num1 = Random.Range (11, 20);
					num1 = num1 * 5;
					QuestionText.text = "Express as percent: " + num.ToString () + " out of " + num1.ToString () + ". Round off your answer to the nearest 2 decimal places.";
					per = ((float)num / (float)num1) * 100;
					per = Mathf.Round (per * 100) / (float)100;
					TwoOptionsInputEnabled = true;
					GeneralButton.gameObject.SetActive (false);
					OptionsInput [0].transform.parent.gameObject.SetActive (true);
					SetFocusOptionInput (0);
					Option1Correct = true;
					Answer = "10";
				} else if (selector == 2) {
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 10);
					num2 = Random.Range (1, 10);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express as ratio: ";
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
					QuestionText.text = "A broker purchases goods worth Rs." + num.ToString () + ". What is his commission at " + num1.ToString () + "%.";
					per = (float)(num * num1) / 100;
					Answer = per.ToString ();
				} else if (selector == 4) {
				
					num = Random.Range (1, 10);
					num = num * 5;
					num1 = Random.Range (10, 50);
					QuestionText.text = num.ToString () + "% of a number is " + num1.ToString () + ". Find the number. Round off your answer to the nearest integer.";
					per = (float)(num1 * 100) / (float)num;
					per = Mathf.Round (per);
					Answer = per.ToString ();
				}
			}
			else if (level == 2) {
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				selector = GetRandomSelector (1, 10);

				if (selector == 1) {
					num = Random.Range (1, 15);
					num = num * 10;
					num1 = Random.Range (2, 10);
					num1 = num1 * 10;
					QuestionText.text = "Speed of car A is " + num.ToString () + "km/h. Speed of car B is " + num1.ToString () + "% of the speed of car A. What is the ratio of the speed of car B to the speed of car A?";
					per = (float)(num1 * num) / 100;
					Answer = per.ToString () + ":" + num.ToString ();
				} else if (selector == 2) {
				
					num = Random.Range (1, 10);
					num = num * 5;
					num1 = Random.Range (50, 100);
					num1 = num1 * 10;
					QuestionText.text = "The value of a cycle is reduced by " + num.ToString () + "%. If the present value is Rs." + num1.ToString () + ", what was its value before? Round off your answer to the nearest 2 decimal places.";
					num = 100 - num;
					per = (float)num / 100;
					per = Mathf.Round (per * 100) / (float)100;
					per = (float)num1 / per;
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				} else if (selector == 3) {
	
					num = Random.Range (10, 30);

					num1 = Random.Range (10, 30);
					QuestionText.text = "Amit obtained " + num.ToString () + " and " + num1.ToString () + " marks respectively in two monthly tests. What is the change in percent? Round off your answer to the nearest 2 decimal places.";
					per = ((float)(num - num1) /(float) num) * 100;
					per = Mathf.Abs (per);
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				

				} else if (selector == 4) {
				
					num = Random.Range (2, 17);
					num = num * 5;
					num1 = Random.Range (5, 21);
					num1 = num1 * 10;
					QuestionText.text = "A number when decreased by " + num.ToString () + "% gives " + num1.ToString () + ". What is the number? Round off your answer to the nearest 2 decimal places.";
					per = (float)(100 - num) / 100;
					per = (float)num1 /(float) per;
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				} else if (selector == 5) {
				//not accepting decimal
					num = Random.Range (2, 10);
					num = num * 5;
					num1 = Random.Range (5, 15);
					num1 = num1 * 10;
					QuestionText.text = "The price of mangoes is increased by " + num.ToString () + "%. If the price before the increase was Rs." + num1.ToString () + " per kg. What is the present price per kg?";
					per = (float)(100 + num) / 100;
					per = per * num1;
					per = Mathf.Round (per);
					Answer = per.ToString ();
				} else if (selector == 6) {
					num = Random.Range (1, 11);
					num1 = Random.Range (1, 11);
					QuestionText.text = "A trader mixes two blends of tea, Darjeeling and Assam, in the ratio " + num.ToString () + ":" + num1.ToString () + ". What percent of the total mixture is Darjeeling tea? Round off your answer to the nearest 2 decimal digit.";
					per = (((float)num /(float) (num + num1)) * 100);
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				} else if (selector == 7) {
				
					num = Random.Range (1, 10);
					num = num * 5;
					QuestionText.text = "The side length of a square is increased by " + num.ToString () + "%. What is the percentage increase of its area? Round off your answer to the nearest 2 decimal places.";
					per = (float)(100 + num) / 100;
					per = ((per * per) - (1)) * 100;
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				} else if (selector == 8) {
				
					num = Random.Range (10, 30);
					num = num * 1000;
					num1 = Random.Range (10, 30);
					num1 = num1 * 1000;
					QuestionText.text = "A computer costs Rs." + num.ToString () + " this year. It costed Rs." + num1.ToString () + " a year ago. What is the percentage change in the value? Round off your answer to the nearest 2 decimal places.";
					per = ((float)(num1 - num) /(float) num1) * 100;
					per = Mathf.Abs (per);
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				} else if (selector == 9) {
				
					num = Random.Range (2, 11);
					num = num * 5;
					num1 = Random.Range (5, 21);
					num1 = num1 * 10;
					num2 = Random.Range (2, 11);
					num2 = num2 * 5;
					QuestionText.text = num.ToString () + "% of an amount is Rs." + num1.ToString () + ". Calculate " + num2.ToString () + "% of the same amount. Round off your answer to the nearest 2 decimal places.";
					per = (float)(num1 * 100) /(float) num;
					per = (float)(per * num2) / 100;
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				}
			
			
			} else if (level == 3) {

				selector = GetRandomSelector (1, 5);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;

				if (selector == 1) {
					//changes
				
					num = Random.Range (5, 10);
					num = num * 10;
					num1 = (100 - num) / 2;
					num2 = Random.Range (2, 21);
					num2 = num2 * 100;
					QuestionText.text = "Harvey spends " + num.ToString () + "% of his income in household expenses. " + num1.ToString () + "% of the remainder in personal necessities and " + num1.ToString () + "% of the amount left in his savings. Find his income if the amount he saves per month is Rs." + num2.ToString ()+".";
					per = ((float)(100 - num) / (float)100) * ((float)(100 - num1) / (float)100) * ((float)num1 / (float)100);
					per = (float)num2 / (float)per;
					Answer = per.ToString ();

				} else if (selector == 2) {
				

					num = Random.Range (1, 11);
					num = num * 100;
					num1 = Random.Range (1, 10);
					num1 = num1 * 10;
					num2 = Random.Range (1, 10);
					num2 = num2 * 10;
					QuestionText.text = "A school has " + num.ToString () + " students. In an election of student council " + num1.ToString () + "% did not vote. Rohit received " + num2.ToString () + "% of the total votes polled. How many votes did Rohit got?";
					per = (float)(num * num1) / 100;
					per = num - per;
					per = (float)(per * num2) / 100;
					Answer = per.ToString ();
				} else if (selector == 3) {
				
					num = Random.Range (1, 5);
					num1 = num + 1;
					num2 = Random.Range (1, 11);
					num2 = num2 * (2 * num + 1);
					QuestionText.text = "In a group of "+num2.ToString()+" children the ratio of boys to girls is " + num.ToString () + ":" + num1.ToString () + ". 5 more girls join the group. What is the percentage decrease of the boys? Round off your answer to the nearest 2 decimal places.";
					per = ((float)num / (float)(num + num1)) * num2;
					per1 = ((float)per / (float)(num2 + 5)) * 100;
					per = ((float)per / (float)num2) * 100;
					per = per - per1;
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				} else if (selector == 4) {
				
					num = Random.Range (1, 6);
					num = num * 10;
					num1 = Random.Range (1, 20);
					num1 = num1 * 100;
					QuestionText.text = "Mr. Louis receives an annual increment of " + num.ToString () + "% on his base salary. Last year his base salary was Rs." + num1.ToString () + ". The new base salary is the old base salary added to the increment. So, how much increment will he receive at the end of this year?";
					per = num1 + ((float)(num * num1) / (float)100);
					per = (float)(per * num) / (float)100;
					per = Mathf.Round (per * 100) / (float)100;
					Answer = per.ToString ();
				}
			}

			CerebroHelper.DebugLog ("level" + level);
			CerebroHelper.DebugLog ("selector" + selector);
			CerebroHelper.DebugLog (Answer);

			if (answerButton != null && !TwoOptionsInputEnabled) {
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
		}

	}
}
