using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class PercentageScript5 : BaseAssessment {

		public TEXDraw subQuestionText;
		public GameObject[] FractionNumber;
		private string Answer;
		private int num1, num2, num3, num4;
		private float ans;

		private bool IsFractionEnable;
		private string[] AnswerArray;


		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "PER05", "S01", "A01");

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
			} else if(Answer.Contains("/")){

				var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
				var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
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
				UpdateStreak(8, 12);

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
		//	CerebroHelper.DebugLog ("hie");
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
				selector = GetRandomSelector (1, 6);
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
				}
				else if (selector == 5) {

					num1 = Random.Range (10, 51);
					num2 = Random.Range (num1 + 1, 96);
					while (num2 % 5 != 0)
						num2 = Random.Range (num1 + 1, 96);
					ans = (float)(num1 * num2) / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Find the value :";
					subQuestionText.text = num1 + "% of " + num2;
					Answer = ans.ToString ();
				}
			}
			else if (level == 2) {
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				selector = GetRandomSelector (1, 6);
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
					Answer = (num1/hcf) + "/" + (100/hcf);

				} else if (selector == 4) {

					num1 = Random.Range (101, 300);
					ans = (float)num1 / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Convert to decimal :";
					subQuestionText.text = num1 + "%";
					Answer = ans.ToString ();
				}
				else if (selector == 5) {

					num1 = Random.Range (11, 1000);
					int selectQuantity = Random.Range (0, 4);
					if (selectQuantity == 1)
						num1 = Random.Range (11, 100);
					string[] selectQuantity1 = new string[4]{"g", "p", "ml", "m"};
					string[] selectQuantity2 = new string[4]{"kg", "Rs.", "l", "km"};
					int[] conversionFactor = new int[4]{1000, 100, 1000, 1000};
					ans = (float)(num1 * 100) /(float)conversionFactor[selectQuantity];
					ans = MathFunctions.GetRounded (ans, 3);
					QuestionText.text = "Express as percent :";
					subQuestionText.text = num1 + " " + selectQuantity1[selectQuantity] + " of 1 " + selectQuantity2[selectQuantity];
					Answer = ans.ToString () + "%";
				}
			}
		 	else if (level == 3) {

				selector = GetRandomSelector (1, 6);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				subQuestionText.gameObject.SetActive (false);


				if (selector == 1) {
					
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
					SetFocusFractionNumber(0);

				} else if (selector == 2) {
					
					num1 = Random.Range (2, 100);
					ans = (float)num1 / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Sheela saves " + ans + " of her pocket money every month. What percent of her pocket money does she save?";
					Answer = num1.ToString () + "%";

				} else if (selector == 3) {
					
					num1 = 100 * Random.Range (3, 21);
					num2 = 5 * Random.Range (2, 11);
					QuestionText.text = "Arjun's mother bought a pair of shoes for him at a sale. The shoes cost Rs. " + num1 + ", but she got it at " + num2 + "% less than that. How much money (in Rs.) did she save?";
					ans = (float)(num1 * num2) / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString ();

				} else if (selector == 4) {
					
					num1 = 5 * Random.Range (1,16);
					QuestionText.text = num1 + "% of a class says that maths is their favourite subject. What fraction of the class has maths as their favourite subject?";
					int hcf = MathFunctions.GetHCF (num1, 100);
					Answer = (num1/hcf) + "/" + (100/hcf);

				} else if (selector == 5) {

					num1 = 5 * Random.Range (15, 101);
					num2 = Random.Range (50, 251);
					while ((num2 * 1000) % num1 != 0)
						num2 = Random.Range (50, 251);
					ans = (float)(num2 * 100) / num1;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Ankita got Rs. " + num1 + " from making and selling Diwali cards during the school fete. She spent Rs. " + num2 + " on buying a present at the fete for her mother. What percent of her money did she spend?";
					Answer = ans.ToString () + "%";
				}

			}
			else if (level == 4) {

				selector = GetRandomSelector (1, 6);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;
				subQuestionText.gameObject.SetActive (false);


				if (selector == 1) {

					num1 = Random.Range (5, 31);
					num2 = Random.Range (10, 31);
					while ((num1 >= num2) || ((num1 * 1000) % num2 != 0)) {
						num1 = Random.Range (5, 31);
						num2 = Random.Range (10, 31);
					}
					QuestionText.text = "In a test, Trisha got " + num1 + " sums correct out of " + num2 + ". What percent of the sums did she get correct?";
					ans = (num1 * 100f) / (float)num2;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString () + "%";

				} else if (selector == 2) {

					num1 = Random.Range (100, 151);
					num2 = Random.Range (60, 100);
					while ((num1 * 100) % num2 == 0 || MathFunctions.GetHCF(num1, num2) == 1) {
						num1 = Random.Range (100, 151);
						num2 = Random.Range (60, 100);
					}
					QuestionText.text = "AB de Villiers faced " + num2 + " balls and made " + num1 + " runs. What was his strike rate? (Strike rate is the percentage of runs scored per ball.)";
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
					SetFocusFractionNumber(0);


				} else if (selector == 3) {

					num1 = Random.Range (45, 101);
					num2 = Random.Range (num1 + 1, 200);
					while ((num1 >= num2) || (((num2 - num1) * 1000) % num2 != 0)) {
						num1 = Random.Range (45, 101);
						num2 = Random.Range (num1 + 1, 200);
					}
					QuestionText.text = "In a test match, Dravid hit " + num1 + " balls out of " + num2 + " balls that he faced. What percent of balls did he not hit?";
					ans = ((num2 - num1) * 100f) / (float)num2;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString () + "%";

				} else if (selector == 4) {

					num1 = 10 * Random.Range (10, 51);
					num2 = 5 * Random.Range (2, 11);
					QuestionText.text = "A calculator that costs Rs. " + num1 + " is being sold for " + num2 + "% less at a sale. How much would you pay if you buy the calculator?";
					ans = (float)num1 - (float)(num1 * num2) / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString ();

				} else if (selector == 5) {

					num1 = 5 * Random.Range (1, 20);
					num2 = 5 * Random.Range (1, 20);
					num3 = 5 * Random.Range (1, 20);
					num4 = Random.Range (50, 251);
					while ((((100 - num1) * num4) % 100 != 0) || (((100 - num1) * (100 - num2) * num4) % 10000 != 0) || (((100 - num1) * (100 - num2) * (100 - num3) * num4) % 1000000 != 0)) {
						num1 = 5 * Random.Range (1, 20);
						num2 = 5 * Random.Range (1, 20);
						num3 = 5 * Random.Range (1, 20);
						num4 = Random.Range (50, 251);
					}
					ans = ((100 - num1) * (100 - num2) * (100 - num3) * num4) / 1000000;
					ans = MathFunctions.GetRounded (ans, 2);
					QuestionText.text = "Shalini has " + num4 + " stickers. She gave " + num1 + "% of them to Vinita. She then gave " + num2 + "% of the remaining stickers to Smita. Again, she gave " + num3 + "% of the rest to Eshita. How many stickers are still with Shalini?";
					Answer = ans.ToString ();
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
		}
	}
}
