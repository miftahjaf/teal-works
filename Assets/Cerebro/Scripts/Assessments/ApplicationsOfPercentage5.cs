using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class ApplicationsOfPercentage5 : BaseAssessment {

		public TEXDraw subQuestionTEX;
		public GameObject[] OptionsInput;
		private string Answer;
		private int num1, num2, num3, num4;

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


			if (Answer.Contains ("%")) {

				if (userAnswerText.text == Answer)
					correct = true;
				else
					correct = false;
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
				subQuestionTEX.gameObject.SetActive (true);
				answerButton = GeneralButton;
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {
					
					num1 = 100 * Random.Range (20, 51);
					num2 = 100 * Random.Range (3, 15);
					QuestionText.text = "Find Cost Price (in Rs.), if : ";
					subQuestionTEX.text = "Selling Price = Rs. " + num1 + ", Profit = Rs. " + num2;
					int ans = num1 - num2;
					Answer = ans.ToString ();

				} else if (selector == 2) {

					num1 = 100 * Random.Range (20, 51);
					num2 = 100 * Random.Range (3, 15);
					QuestionText.text = "Find Cost Price (in Rs.), if : ";
					subQuestionTEX.text = "Selling Price = Rs. " + num1 + ", Loss = Rs. " + num2;
					int ans = num1 + num2;
					Answer = ans.ToString ();
				
				} else if (selector == 3) {

					num1 = 100 * Random.Range (20, 51);
					num2 = 100 * Random.Range (3, 15);
					QuestionText.text = "Find Selling Price (in Rs.), if : ";
					subQuestionTEX.text = "Cost Price = Rs. " + num1 + ", Profit = Rs. " + num2;
					int ans = num1 + num2;
					Answer = ans.ToString ();

				} else if (selector == 4) {
				
					num1 = 100 * Random.Range (20, 51);
					num2 = 100 * Random.Range (3, 15);
					QuestionText.text = "Find Selling Price (in Rs.), if : ";
					subQuestionTEX.text = "Cost Price = Rs. " + num1 + ", Loss = Rs. " + num2;
					int ans = num1 - num2;
					Answer = ans.ToString ();

				} else if (selector == 5) {

					num1 = 1000 * Random.Range (10, 50);
					num2 = 100 * Random.Range (20, 51);
					QuestionText.text = "Find Amount (in Rs.), if : ";
					subQuestionTEX.text = "Principal = Rs. " + num1 + ", Simple Interest = Rs. " + num2;
					int ans = num1 + num2;
					Answer = ans.ToString ();

				}
			}
			else if (level == 2) {
				GeneralButton.gameObject.SetActive (true);
				subQuestionTEX.gameObject.SetActive (true);
				answerButton = GeneralButton;
				selector = GetRandomSelector (1, 6);

				if (selector == 1) {

					num1 = Random.Range (50, 81);
					num2 = 100 * Random.Range (num1 + 5, 101);
					num1 *= 100;
					QuestionText.text = "Find Profit (in Rs.), if : ";
					subQuestionTEX.text = "Cost Price = Rs. " + num1 + ", Selling Price = Rs. " + num2;
					int ans = num2 - num1;
					Answer = ans.ToString ();

				} else if (selector == 2) {
				
					num1 = Random.Range (50, 81);
					num2 = 100 * Random.Range (num1 + 5, 101);
					num1 *= 100;
					QuestionText.text = "Find Loss (in Rs.), if : ";
					subQuestionTEX.text = "Cost Price = Rs. " + num2 + ", Selling Price = Rs. " + num1;
					int ans = num2 - num1;
					Answer = ans.ToString ();

				} else if (selector == 3) {
	
					num1 = 50 * Random.Range (100, 161);
					num3 = Random.Range (500, 5001);
					while ((num3 * num1) % 10000 != 0)
						num3 = Random.Range (500, 5001);
					num2 = num1 + (num3 * num1) / 10000;
					QuestionText.text = "Find Profit %, if : ";
					subQuestionTEX.text = "Cost Price = Rs. " + num1 + ", Selling Price = Rs. " + num2;
					float ans = (float)num3 / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString () + "%";

				} else if (selector == 4) {
				
					num1 = 50 * Random.Range (100, 161);
					num3 = Random.Range (500, 5001);
					while ((num3 * num1) % 10000 != 0)
						num3 = Random.Range (500, 5001);
					num2 = num1 - (num3 * num1) / 10000;
					QuestionText.text = "Find Loss %, if : ";
					subQuestionTEX.text = "Cost Price = Rs. " + num1 + ", Selling Price = Rs. " + num2;
					float ans = (float)num3 / 100f;
					ans = MathFunctions.GetRounded (ans, 2);
					Answer = ans.ToString () + "%";

				} else if (selector == 5) {

					num1 = 1000 * Random.Range (10, 50);
					num2 = Random.Range (4, 15);
					num3 = Random.Range (2, 10);
					QuestionText.text = "Find Simple Interest (in Rs.), if : ";
					subQuestionTEX.text = "Principal = Rs. " + num1 + ", Rate of Interest = " + num2 + "%, Time = " + num3 + " years";
					int ans = num1 * num2 * num3 / 100;
					Answer = ans.ToString ();

				} 
			
			
			} else if (level == 3) {

				selector = GetRandomSelector (1, 6);
				GeneralButton.gameObject.SetActive (true);
				subQuestionTEX.gameObject.SetActive (false);
				answerButton = GeneralButton;

				if (selector == 1) {

					num1 = Random.Range (100, 201);
					num2 = Random.Range (11, 51);
					QuestionText.text = "Ramu bought apples from a farmer and sold them in the market. He sells them at Rs. " + num1 + " for a kilo and makes a profit of Rs. " + num2 + " on each kilo. How much did Ramu pay for each kilo (in Rs.) of apples?";
					int ans = num1 - num2;
					Answer = ans.ToString ();

				} else if (selector == 2) {
				
					num1 = Random.Range (100, 201);
					num2 = Random.Range (11, 51);
					QuestionText.text = "Shyam bought bananas from a shop. He then sold them to his friend Kamal at Rs. " + num1 + " for a dozen at a loss of Rs. " + num2 + " on each dozen. How much did he pay for each dozen (in Rs.) of bananas?";
					int ans = num1 + num2;
					Answer = ans.ToString ();

				} else if (selector == 3) {
				
					num1 = 50 * Random.Range (20, 51);
					num2 = 10 * Random.Range (11, 31);
					QuestionText.text = "Arjun makes jewellery using semi-precious stones. He has a necklace which cost him Rs. " + num1 + " to make. He sells this at a profit of Rs. " + num2 + " at his shop. What is the selling price (in Rs.) of the necklace?";
					int ans = num1 + num2;
					Answer = ans.ToString ();

				} else if (selector == 4) {
				
					num1 = 50 * Random.Range (20, 51);
					num2 = 10 * Random.Range (11, 31);
					QuestionText.text = "Anita buys earrings that cost her Rs. " + num1 + ". She then sells them at a loss of Rs. " + num2 + " to her friend Maria. At what price (in Rs.) did Maria buy the earrings?";
					int ans = num1 - num2;
					Answer = ans.ToString ();

				} else if (selector == 5) {

					subQuestionTEX.gameObject.SetActive (true);
					do {
						num1 = 100 * Random.Range (10, 50);
						num2 = Random.Range (4, 15);
						num3 = Random.Range (2, 10);
					} while ((num1 * 100) % (num2 * num3) != 0);
		
					QuestionText.text = "Find Principal (in Rs.), if : ";
					subQuestionTEX.text = "Simple Interest = Rs. " + num1 + ", Rate of Interest = " + num2 + "%, Time = " + num3 + " years";
					int ans = (num1 * 100) / (num2 * num3);
					Answer = ans.ToString ();

				} 
			}
			else if (level == 4) {

				selector = GetRandomSelector (1, 6);
				GeneralButton.gameObject.SetActive (true);
				subQuestionTEX.gameObject.SetActive (false);
				answerButton = GeneralButton;

				if (selector == 1) {

					num1 = 1000 * Random.Range (40, 101);
					num2 = Random.Range (11, 41);
					QuestionText.text = "A used bike dealer bought a bike for Rs. " + num1 + ". He sold it at a loss of " + num2 + "%. What was the selling price (in Rs.) of the bike?";
					int ans = num1 - (num1 * num2) / 100;
					Answer = ans.ToString ();

				} else if (selector == 2) {

					num1 = 100 * Random.Range (20, 91);
					num2 = Random.Range (11, 41);
					QuestionText.text = "A toy plane was manufactured at a cost of Rs. " + num1 + ". What price (in Rs.) should it be sold at to make a profit of " + num2 + "%?";
					int ans = num1 + (num1 * num2) / 100;
					Answer = ans.ToString ();

				} else if (selector == 3) {

					num1 = 1000 * Random.Range (2, 21);
					num2 = Random.Range (4, 15);
					num3 = Random.Range (2, 10);
					QuestionText.text = "Sushil deposited Rs. " + num1 + " in his bank account at " + num2 + "% per annum. How much interest (in Rs.) will he get at the end of " + num3 + " years?";
					int ans = (num1 * num2 * num3) / 100;
					Answer = ans.ToString ();

				} else if (selector == 4) {

					num1 = 10 * Random.Range (100, 501);
					num2 = Random.Range (2, 10);
					num3 = Random.Range (4, 15);
					while ((num1 * 100) % (num2 * num3) != 0)
						num1 = 10 * Random.Range (100, 501);
					QuestionText.text = "What was the loan amount (in Rs.) taken by Shyam if he paid an interest of Rs. " + num1 + " for " + num2 + " years at a rate of " + num3 + "%?";
					int ans = (num1 * 100) / (num2 * num3);
					Answer = ans.ToString ();

				} else if (selector == 5) {

					num1 = Random.Range (100, 501);
					num2 = Random.Range (2, 10);
					num3 = Random.Range (10, 26);
					while ((num1 * num2 * num3) % 12 != 0)
						num1 = Random.Range (100, 501);
					num1 *= 100;
					QuestionText.text = "A loan costs Veena " + num2 + "% per annum. If she takes a loan of Rs. " + num1 + " for " + num3 + " months, how much interest (in Rs.) will she have to pay?";
					int ans = (num1 * num2 * num3) / 1200;
					Answer = ans.ToString ();

				} 
			}
			else if (level == 5) {

				selector = GetRandomSelector (1, 6);
				GeneralButton.gameObject.SetActive (true);
				subQuestionTEX.gameObject.SetActive (false);
				answerButton = GeneralButton;

				if (selector == 1) {

					num1 = Random.Range (1000, 2001);
					num2 = Random.Range (400, 1001);
					num3 = Random.Range (num1 + num2 - 200, num1 + num2 + 200);
					QuestionText.text = "A man bought a trouser length for Rs. " + num1 + ". He spent Rs. " + num2 + " on tailoring charges. He then sold it at Rs. " + num3 + ". What was the profit or loss (in Rs.) on the deal?";
					int ans = num1 + num2 - num3;
					Option1Correct = (ans > 0) ? false : true;
					ans = Mathf.Abs (ans);
					Answer = ans.ToString ();

					TwoOptionsInputEnabled = true;
					GeneralButton.gameObject.SetActive (false);
					OptionsInput [0].transform.parent.gameObject.SetActive (true);
					SetFocusOptionInput (0);

				} else if (selector == 2) {

					num1 = Random.Range (20, 91);
					num2 = Random.Range (11, 41); 
					while ((num1 * num2) % 5 != 0) {
						num1 = Random.Range (20, 91);
						num2 = Random.Range (11, 41);
					}
					num1 *= 100;
					int RandomDay = Random.Range (1, 28);
					int RandomMonth = Random.Range (1, 12);
					num3 = Random.Range (1, 5);
					System.DateTime firstDate = System.DateTime.ParseExact ("2015"+(RandomMonth<10?"0":"")+RandomMonth+(RandomDay<10?"0":"")+RandomDay, "yyyyMMdd", null);
					System.DateTime lastDate = firstDate.AddDays (73 * num3);
					string firstDay = firstDate.ToString ("dd MMM, yyyy");
					string lastDay = lastDate.ToString ("dd MMM, yyyy");
					if (firstDay [0] == '0')
						firstDay = firstDay.Substring (1);
					if (lastDay [0] == '0')
						lastDay = lastDay.Substring (1);
					QuestionText.text = "Mohini borrows Rs. " + num1 + " at " + num2 + "% interest. She takes the loan from " + firstDay + " to " + lastDay + ". How much interest (in Rs.) does she pay?";
					int ans = (num1 * num2 * num3) / 500;
					Answer = ans.ToString ();

				} else if (selector == 3) {

					num1 = 1000 * Random.Range (2, 21);
					num2 = 73 * Random.Range (2, 16);
					num3 = Random.Range (10, 26);
					QuestionText.text = "Shiv takes a loan for " + num2 + " days. He borrows Rs. " + num1 + " from the bank at " + num3 + "% per annum. How much does he pay back (in Rs.) to the bank after " + num2 + " days?";
					int ans = (num1 * num2 * num3) / 36500;
					Answer = ans.ToString ();

				} else if (selector == 4) {

					num1 = 100 * Random.Range (50, 101);
					num2 = 100 * Random.Range (4, 11);
					int ans = Random.Range (-50, 51);
					while (ans == 0)
						ans = Random.Range (-50, 51);

					num3 = ((num1 + num2) * (ans + 100)) / 100;
					QuestionText.text = "A TV that was bought for Rs. " + num1 + " was sold at Rs. " + num3 + ". What was the profit or loss percent, if accidental damage repairs cost an extra Rs. " + num2 + " before selling?";

					Option1Correct = (ans > 0) ? true : false;
					ans = Mathf.Abs (ans);
					Answer = ans.ToString () + "%";

					TwoOptionsInputEnabled = true;
					GeneralButton.gameObject.SetActive (false);
					OptionsInput [0].transform.parent.gameObject.SetActive (true);
					SetFocusOptionInput (0);

				} else if (selector == 5) {

					num1 = Random.Range (100, 501);
					num2 = Random.Range (10, 26);
					num3 = Random.Range (10, 51);
					while (num3 % 12 == 0)
						num3 = Random.Range (10, 51);
					while ((num1 * num2 * num3) % 12 != 0)
						num1 = Random.Range (100, 501);
					num1 *= 100;
					QuestionText.text = "Gina borrows Rs. " + num1 + " to buy a new computer. If she takes the loan at " + num2 + "% per annum and repays the loan after " + num3 + " months, how much (in Rs.) will she have to pay back?";
					int ans = num1 + (num1 * num2 * num3) / 1200;
					Answer = ans.ToString ();

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
				if (checkLastTextFor (new string[1]{ "%" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "%";
			}
		}

	}
}
