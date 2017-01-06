using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class PercentageScript6 : BaseAssessment {

		public TEXDraw subQuestionText;
		public GameObject[] FractionNumber;
		private string Answer;
		private int num1, num2, num3, num4, num5;
		private float ans;

		private bool IsFractionEnable;
		private bool IsComparison;
		private string[] AnswerArray;

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "PER06", "S01", "A01");

			scorestreaklvls = new int[4];
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
			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;
			float answer = 0;

			if (IsFractionEnable) {
				
				string[] UserAns = new string[3];
				UserAns [0] = FractionNumber [0].transform.FindChild ("Text").GetComponent<Text> ().text;
				UserAns [1] = FractionNumber [1].transform.FindChild ("Text").GetComponent<Text> ().text;
				UserAns [2] = FractionNumber [2].transform.FindChild ("Text").GetComponent<Text> ().text;
				if (checkArrayValues (UserAns, AnswerArray)) {
					correct = true;
				} else {
					correct = false;
				}
			} else if (IsComparison) {
				
				if (Answer == userAnswerText.text)
					correct = true;
				else 
					correct = false;

			} else if(Answer.Contains("/")){

				var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
				var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
				correct = MathFunctions.checkFractions (userAnswers, correctAnswers);


			} else if(Answer.Contains(":")){

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
					UpdateStreak (12, 17);
					increment = 5;
				} else if (Queslevel == 2) {
					UpdateStreak (10, 15);
					increment = 10;
				} else if (Queslevel == 3) {
					UpdateStreak (10, 15);
					increment = 15;
				} else if (Queslevel == 4) {
					UpdateStreak (8, 12);
					increment = 15;
				} else if (Queslevel == 5) {
					UpdateStreak (8, 12);
					increment = 15;
				}

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
			if (IsFractionEnable) {
				for (int i = 0; i < 3; i++) {
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
						for (int i = 0; i < 3; i++) {
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
			} else {
				if (numPad.activeSelf) {
					if (IsFractionEnable) {
						for (int i = 0; i < 3; i++) {
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
				for (int i = 0; i < 3; i++) {
					FractionNumber[i].transform.FindChild("Text").GetComponent<Text>().color = MaterialColor.green800;
					var config = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations (2, GoLoopType.PingPong);
					var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1));
					var tween = new GoTween (FractionNumber[i].transform.FindChild("Text").gameObject.transform, 0.2f, config);
					flow.insert (0f, tween);
					flow.play ();
				}
				yield return new WaitForSeconds (1f);
				for (int i = 0; i < 3; i++) {
					FractionNumber[i].transform.FindChild("Text").GetComponent<Text>().color = MaterialColor.textDark;
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
			for (int i = 0; i < 3; i++) {
				FractionNumber[i].transform.FindChild("border").gameObject.SetActive(false);
			}
			FractionNumber[no].transform.FindChild("border").gameObject.SetActive(true);
			userAnswerText = FractionNumber [no].transform.FindChild ("Text").GetComponent<Text> ();
			userAnswerText.text = "";
		}

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			QuestionText.gameObject.SetActive (true);

			numPad.SetActive (true);
			answerButton = null;
			userAnswerText = null;
			subQuestionText.gameObject.SetActive (true);
			FractionNumber [0].transform.parent.gameObject.SetActive (false);
			IsFractionEnable = false;
			IsComparison = false;
			for (int i = 0; i < 3; i++) {
				FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().text = "";
				FractionNumber [i].transform.FindChild ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			level = Queslevel;

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				selector = GetRandomSelector (1, 8);
				if (selector == 1) {

					num1 = Random.Range (1, 21);
					num2 = Random.Range (2, 21);
					while ((num1 >= num2) || ((num1 * 100) % num2 != 0)) {
						num1 = Random.Range (1, 21);
						num2 = Random.Range (2, 21);
					}
					QuestionText.text = "Convert to percent :";
					subQuestionText.text = "\\frac{" + num1 + "}{" + num2 + "}";
					ans = (num1 * 100f) / (float)num2;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString () + "%";

				} else if (selector == 2) {
					
					num1 = Random.Range (2, 100);
					ans = (float)num1 / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Convert to percent :";
					subQuestionText.text = "" + ans;
					Answer = num1.ToString () + "%";
				
				} else if (selector == 3) {

					num1 = 5 * Random.Range (1, 20);
					QuestionText.text = "Convert to fraction (answer in shortest form) :";
					subQuestionText.text = num1 + "%";
					int hcf = MathFunctions.GetHCF (num1, 100);
					Answer = (num1/hcf) + "/" + (100/hcf);

				} else if (selector == 4) {
				
					num1 = Random.Range (2, 100);
					ans = (float)num1 / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Convert to decimal :";
					subQuestionText.text = num1 + "%";
					Answer = ans.ToString ();

				} else if (selector == 5) {

					num1 = Random.Range (10, 51);
					num2 = Random.Range (num1 + 1, 96);
					while (num2 % 5 != 0)
						num2 = Random.Range (num1 + 1, 96);
					ans = (float)(num1 * num2) / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Find the value :";
					subQuestionText.text = num1 + "% of " + num2;
					Answer = ans.ToString ();

				} else if (selector == 6) {

					num1 = Random.Range (1, 21);
					num2 = Random.Range (2, 21);
					while ((num1 == num2) || ((num1 * 100) % num2 != 0)) {
						num1 = Random.Range (1, 21);
						num2 = Random.Range (2, 21);
					}
					QuestionText.text = "Convert to percent :";
					subQuestionText.text = num1 + " : " + num2;
					ans = (num1 * 100f) / (float)num2;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString () + "%";

				} else if (selector == 7) {

					num1 = 5 * Random.Range (1, 20);
					QuestionText.text = "Convert to ratio (answer in shortest form) :";
					subQuestionText.text = num1 + "%";
					int hcf = MathFunctions.GetHCF (num1, 100);
					Answer = (num1/hcf) + ":" + (100/hcf);

				}
			}
			else if (level == 2) {
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				selector = GetRandomSelector (1, 7);
				if (selector == 1) {
					
					num1 = Random.Range (4, 31);
					num2 = Random.Range (5, 31);
					while ((num1 <= num2) || ((num1 * 100) % num2 != 0)) {
						num1 = Random.Range (4, 31);
						num2 = Random.Range (5, 31);
					}
					QuestionText.text = "Convert to percent :";
					subQuestionText.text = "\\frac{" + num1 + "}{" + num2 + "}";
					ans = (num1 * 100f) / (float)num2;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString () + "%";

				} else if (selector == 2) {
					
					num1 = Random.Range (101, 300);
					ans = (float)num1 / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Convert to percent :";
					subQuestionText.text = "" + ans;
					Answer = num1.ToString () + "%";

				} else if (selector == 3) {
					num1 = 5 * Random.Range (1, 20);
					QuestionText.text = "Convert to fraction (answer in shortest form) :";
					subQuestionText.text = num1 + "%";
					int hcf = MathFunctions.GetHCF (num1, 100);
					Answer = (num1 / hcf) + "/" + (100 / hcf);

				} else if (selector == 4) {

					num1 = Random.Range (101, 300);
					ans = (float)num1 / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Convert to decimal :";
					subQuestionText.text = num1 + "%";
					Answer = ans.ToString ();

				} else if (selector == 5) {

					num1 = Random.Range (11, 1000);
					int selectQuantity = Random.Range (0, 4);
					if (selectQuantity == 1)
						num1 = Random.Range (11, 100);
					string[] selectQuantity1 = new string[4]{ "g", "p", "ml", "m" };
					string[] selectQuantity2 = new string[4]{ "kg", "Rs.", "l", "km" };
					int[] conversionFactor = new int[4]{ 1000, 100, 1000, 1000 };
					ans = (float)(num1 * 100) / (float)conversionFactor [selectQuantity];
					ans = MathFunctions.GetRounded (ans, 3);
					QuestionText.text = "Express as percent :";
					subQuestionText.text = num1 + " " + selectQuantity1 [selectQuantity] + " of 1 " + selectQuantity2 [selectQuantity];
					Answer = ans.ToString () + "%";

				} else if (selector == 6) {

					float fraction, percentDecimal;
					IsComparison = true;

					do {
						num1 = Random.Range (3, 20); 
						num2 = Random.Range (num1 + 1, 3 * num1);
						num3 = Random.Range (11, 100); 
						while (num3 % 10 == 0)
							num3 = Random.Range (11, 100); 
						num4 = 10 * Random.Range (1, 10); 
						fraction = (float)num1 * 100f / (float)num2;
					} while (fraction == num3 || fraction == num4);

					percentDecimal = (float)num4 / 100f; 
					percentDecimal = MathFunctions.GetRounded (percentDecimal, 2);
					QuestionText.text = "Arrange in ascending order :";
					subQuestionText.text = num1 + "/" + num2 + ", " + num3 + "%, " + percentDecimal;

					Answer = "";
					if (num3 < num4) {
						if (num3 < fraction) {
							Answer += num3 + "%,";
							if (fraction < num4)
								Answer += num1 + "/" + num2 + "," + percentDecimal;
							else
								Answer += percentDecimal + "," + num1 + "/" + num2;
						} else
							Answer += num1 + "/" + num2 + "," + num3 + "%," + percentDecimal;
					} else {
						if (num4 < fraction) {
							Answer += percentDecimal + ",";
							if (fraction < num3)
								Answer += num1 + "/" + num2 + "," + num3 + "%";
							else
								Answer += num3 + "%," + num1 + "/" + num2;
						} else
							Answer += num1 + "/" + num2 + "," + percentDecimal + "," + num3 + "%";
					}
				}
			}
		 	else if (level == 3) {

				selector = GetRandomSelector (1, 7);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				subQuestionText.gameObject.SetActive (false);


				if (selector == 1) {
					
					num1 = 1000 * Random.Range (5, 25);
					num2 = Random.Range (11, 50);
					num3 = Random.Range (11, 30);
					QuestionText.text = "Aazar gets a salary of Rs. " + num1 + " per month. He spends " + num2 + "% of his income in household expenses, " + num3 + "% of the income in other personal necessities and the rest goes to his savings. Find the amount (in Rs.) he manages to save every month.";
					ans = (float)(num1 * (100 - num2 - num3)) / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString ();

				} else if (selector == 2) {
					
					num1 = Random.Range (1, 21);
					num2 = Random.Range (2, 21);
					while ((num1 == num2) || ((num1 * 100) % (num1 + num2) != 0)) {
						num1 = Random.Range (1, 21);
						num2 = Random.Range (2, 21);
					}
					QuestionText.text = "The ratio of the number of males to the number of females in a community is " + num1 + " : " + num2 + ". What is the percentage of males in the community?";
					ans = (num1 * 100f) / (float)(num1 + num2);
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString () + "%";

				} else if (selector == 3) {
					
					num1 = 100 * Random.Range (5, 25);
					num2 = Random.Range (11, 40);
					num3 = Random.Range (11, 40);
					while (num2 == num3)
						num3 = Random.Range (11, 40);
					QuestionText.text = "In a village, " + num2 + "% are children, " + num3 + "% are women and the rest are men. If there are " + num1 + " people in the village, calculate the number of men.";
					ans = (float)(num1 * (100 - num2 - num3)) / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString ();

				} else if (selector == 4) {
					
					num1 = Random.Range (1000, 2000);
					num2 = Random.Range (30, 70);
					while ((num1 * num2) % 100 != 0)
						num1 = Random.Range (1000, 2000);
					QuestionText.text = "In a school, " + num2 + "% of the students are girls. The school has " + num1 + " students. Calculate the number of boys in the school.";
					ans = (float)(num1 * (100 - num2)) / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString ();

				} else if (selector == 5) {

					num1 = 10000 * Random.Range (21, 210);
					num2 = Random.Range (5, 20);
					ans = (float)(num1 * (100 - num2)) / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "A car costing Rs. " + num1 + " reduces its price by " + num2 + "%. Find the new price (in Rs.).";
					Answer = ans.ToString ();

				} else if (selector == 6) { 
					
					num1 = Random.Range (1, 12);
					num2 = Random.Range (2, 12);
					while ((num1 >= num2) || ((num1 * 100) % num2 == 0)) {
						num1 = Random.Range (1, 12);
						num2 = Random.Range (2, 12);
					}
					QuestionText.text = num1 + "/" + num2 + " of a planet's atmosphere is carbon dioxide. What percent of the planet's atmosphere is carbon dioxide? (answer as a mixed fraction)";
					num3 = (num1 * 100) / num2;
					num4 = (num1 * 100) % num2;
					int hcf = MathFunctions.GetHCF (num4, num2);
					num4 /= hcf;
					num2 /= hcf;
					AnswerArray = new string[3];
					AnswerArray [0] = "" + num3;
					AnswerArray [1] = "" + num4;
					AnswerArray [2] = "" + num2;
					IsFractionEnable = true;
					GeneralButton.gameObject.SetActive (false);
					FractionNumber [0].transform.parent.gameObject.SetActive (true);
					SetFocusFractionNumber (0);
				}

			}
			else if (level == 4) {

				selector = GetRandomSelector (1, 6);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				subQuestionText.gameObject.SetActive (false);


				if (selector == 1) {

					num1 = Random.Range (1000, 2000);
					num3 = Random.Range (5, 51);
					while ((num1 * num3) % 100 != 0)
						num1 = Random.Range (1000, 2000);
					num2 = (num1 * (100 + num3)) / 100;
					QuestionText.text = "The population of a village increases from " + num1 + " to " + num2 + " in 10 years. What is the percentage increase in population?";
					Answer = num3.ToString () + "%";

				} else if (selector == 2) {

					num1 = Random.Range (10000, 30000);
					num3 = Random.Range (5, 21);
					while ((num1 * num3) % 100 != 0)
						num1 = Random.Range (1000, 2000);
					num2 = (num1 * (100 + num3)) / 100;
					QuestionText.text = "A television set costs Rs. " + num1 + ". Its price increases by " + num3 + "%. What is the price (in Rs.) after the increase?";
					Answer = num2.ToString ();


				} else if (selector == 3) {

					num1 = 100000 * Random.Range (2, 20);
					num3 = Random.Range (5, 21);
					num2 = (num1/10000) * (100 + num3) * (100 + num3);
					QuestionText.text = "If the annual increase in the population of a town is " + num3 + "% and the present number of inhabitants is " + (num1/100000) + " lakh, calculate the population of the town after 2 years.";
					Answer = num2.ToString ();

				} else if (selector == 4) {

					num1 = Random.Range (1000, 2000);
					num2 = Random.Range (30, 50);
					while ((num1 * num2) % 100 != 0)
						num1 = Random.Range (1000, 2000);
					num3 = (num1 * (100 - num2)) / 100;
					QuestionText.text = "In a school, " + num2 + "% of the students are girls. The number of boys is " + ((num1 * (100 - 2 * num2))/100) + " more than the number of girls. Calculate the number of boys.";
					Answer = num3.ToString ();

				} else if (selector == 5) {

					num1 = 10 * Random.Range (10, 51);
					num2 = 10 * Random.Range (50, 151);
					num3 = Random.Range (2, 10);
					num4 = Random.Range (2, 10);
					while (num3 == num4)
						num4 = Random.Range (2, 10);
					
					while ((num1 * num3) % 100 != 0)
						num1 = 10 * Random.Range (10, 51);

					while ((num2 * num4) % 100 != 0)
						num2 = 10 * Random.Range (50, 151);

					num5 = ((num1 * (100 + num3)) / 100) * ((num2 * (100 + num4)) / 100); 
					 
					QuestionText.text = "In 2012, a club has " + num1 + " members who pay Rs. " + num2 + " each as annual subscription. In 2013, the membership increases by " + num3 + "% and the annual subscription by " + num4 + "%. What is the total income from the subscription in 2013?";
					Answer = num5.ToString ();
				}

			}

			CerebroHelper.DebugLog ("level" + level);
			CerebroHelper.DebugLog ("selector" + selector);
			CerebroHelper.DebugLog (Answer);

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
			} else if (value == 12) {   // .
				if (checkLastTextFor (new string[1]{ "." })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			}
			else if (value == 13) {   // :
				if (checkLastTextFor (new string[1]{ "%" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "%";
			}
			else if (value == 14) {   // :
				if (checkLastTextFor (new string[1]{ "/" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			}
			else if (value == 15) {   // :
				if (checkLastTextFor (new string[1]{ ":" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ":";
			}
			else if (value == 16) {   // ,
				if (checkLastTextFor (new string[1]{ "," })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ",";
			}
		}
	}
}
