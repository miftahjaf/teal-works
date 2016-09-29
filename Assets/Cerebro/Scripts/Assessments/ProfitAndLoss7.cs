using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class ProfitAndLoss7 : BaseAssessment {

		private int SP;
		private int CP, MP;
		private int profit;
		private int loss;
		private float discount;
		private float profitPer;
		private float lossPer;
		private float disPer;
		private int rate;
		private int ans;
		private int CPm;
		private int SPm;
		private int num,num1,num2;

		private string Answer;
		private string ProfitOrLoss;
		//	private float multiplier;
		public Text SPText;
		public Text CPText;
		public Text ProfitText;
		public Text LossText;

		//		public Text QuestionText;

		public Text SPQuestion;
		public Text CPQuestion;
		public Text ProfitQuestion;
		public Text LossQuestion;

		public GameObject MCQ;

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "PL07", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			profit = 0;
			loss = 0;
			SP = 0;
			CP = 0;
			discount = 0f;
			profitPer = 0f;
			disPer = 0f;
			lossPer = 0f;


			levelUp = false;

			Answer = "0";
			GenerateQuestion ();
		}

		public override void SubmitClick(){
			if (ignoreTouches || userAnswerText.text == "") {
				return;
			}

			questionsAttempted++;
			updateQuestionsAttempted ();

			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;
			float answer = 0;
			if(float.TryParse(userAnswerText.text,out answer)) {
				answer = float.Parse (userAnswerText.text);
			}

			if (MCQ.activeSelf) {
				if (Answer == userAnswerText.text) {
					correct = true;
				} else {
					correct = false;
					AnimateMCQOptionCorrect(Answer);
				}
			} else if (Answer.Contains ("%")) {

				if (userAnswerText.text == Answer)
					correct = true;
				else
					correct = false;
			} else {
				if (answer == float.Parse (Answer)) {
					correct = true;
				}
			}

			if(correct == true) {
				if (Queslevel == 1) {
					increment = 5;
					UpdateStreak(8, 12);
				} else if (Queslevel == 2) {
					increment = 10;
					UpdateStreak(10, 15);
				} else if (Queslevel == 3) {
					increment = 10;
					UpdateStreak(12, 17);
				} else if (Queslevel == 4) {
					increment = 15;
					UpdateStreak(8, 12);
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

		void AnimateMCQOptionCorrect(string ans)
		{
			for (int i = 1; i <= 3; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.green800;
				}
			}
		}


		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
		

			SPQuestion.text = "Rs. " + SP.ToString ();
			CPQuestion.text = "Rs. " + CP.ToString ();


			//formulae ();


			//Generating random questions on Profit and Loss Level 1 Questions

			ProfitQuestion.gameObject.SetActive (false);
			LossQuestion.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (false);


			SPQuestion.gameObject.SetActive (false);
			CPQuestion.gameObject.SetActive (false);

			MCQ.gameObject.SetActive (false);
			numPad.SetActive (true);

			SPText.gameObject.SetActive (false);
			CPText.gameObject.SetActive (false);
			ProfitText.gameObject.SetActive (false);
			LossText.gameObject.SetActive (false);


			answerButton = null;
			userAnswerText = null;

			level = Queslevel;
			//CerebroHelper.DebugLog (level);
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				selector = GetRandomSelector (1, 6);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;

				if (selector == 1) {
					QuestionText.text = "Find the missing data (round off to 2 decimal places).";

					SPQuestion.gameObject.SetActive (true);
					CPQuestion.gameObject.SetActive (true);
					SPText.gameObject.SetActive (true);
					CPText.gameObject.SetActive (true);

					SP = 10 * Random.Range (50, 100);
					SPQuestion.text = "Rs. " + SP.ToString ();

					do {
						CP = 10 * Random.Range (50, 100);
					} while (CP == SP);

					CPQuestion.text = "Rs. " + CP.ToString ();

					if (SP > CP) {
						
						profit = SP - CP;
						profitPer = (float)profit / (float)CP;
						profitPer = profitPer * 100f;
						profitPer = MathFunctions.GetRounded (profitPer, 2);
						CerebroHelper.DebugLog (profitPer);
						ProfitText.gameObject.SetActive (true);
						ProfitQuestion.gameObject.SetActive (true);
						Answer = profitPer.ToString () + "%";

					} else {
						
						loss = CP - SP;
						lossPer = (float)loss / (float)CP;
						lossPer = lossPer * 100f;
						lossPer = MathFunctions.GetRounded (lossPer, 2);
						LossText.gameObject.SetActive (true);
						LossQuestion.gameObject.SetActive (true);
						Answer = lossPer.ToString () + "%";

					}
				} else if (selector == 2) {
					
					SPQuestion.gameObject.SetActive (true);
					CPQuestion.gameObject.SetActive (true);
					SPText.gameObject.SetActive (true);
					CPText.gameObject.SetActive (true);
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					do {
						CP = 50 * Random.Range (10, 21);
						SP = 50 * Random.Range (10, 21);
					} while (CP == SP);
					CPQuestion.text = "Rs. " + CP.ToString ();
					SPQuestion.text = "Rs. " + SP.ToString ();
					profit = SP - CP;
					profitPer = (float)profit / (float)CP * 100f;
					ProfitOrLoss = profitPer > 0 ? "profit" : "loss";
					profitPer = Mathf.Abs (MathFunctions.GetRounded (profitPer, 2));
					QuestionText.text = "Find the " + ProfitOrLoss + " percent (round off to 2 decimal places).";
					Answer = profitPer.ToString () + "%";

				} else if (selector == 3) {
					
					CP = 100 * Random.Range (200, 1000);
					int profitPer = Random.Range (5, 50);
					SP = CP + (CP * profitPer) / 100;
					QuestionText.text = "A laptop costs Rs. " + CP.ToString () + ". What should be the selling price (in Rs.) in order to make a gain of " + profitPer.ToString () + "% ?";
					Answer = SP.ToString (); 

				} else if (selector == 4) {

					CP = 20 * Random.Range (3, 10);
					int lossPer = 5 * Random.Range (1, 10);
					QuestionText.text = "Raman bought a toy for Rs. " + CP.ToString () + " and sold it at " + lossPer.ToString () + "% loss. Find the loss incurred (in Rs.).";
					loss = (CP * lossPer) / 100;
					Answer = loss.ToString ();

				} else if (selector == 5) {

					CP = 20 * Random.Range (3, 10);
					int lossPer = 5 * Random.Range (1, 10);
					QuestionText.text = "Raman bought a toy for Rs. " + CP.ToString () + " and sold it at " + lossPer.ToString () + "% loss. Find the selling price (in Rs.).";
					SP = CP - (CP * lossPer) / 100;
					Answer = SP.ToString ();

				}
			} else if (level == 2) {
				selector = GetRandomSelector (1, 7);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;

				if (selector == 1) {
					
					answerButton = GeneralButton;
					num1 = Random.Range (2, 10);
					num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);

					profitPer = (((float)num1 / (float)num2) - 1);
					profitPer = (float)(profitPer * 100);
					ProfitOrLoss = profitPer > 0 ? "profit" : "loss";
					profitPer = Mathf.Abs (MathFunctions.GetRounded (profitPer, 2));

					QuestionText.text = "A trader sells at " + num1 + "/" + num2 + " of his cost price. Find the " + ProfitOrLoss + " percent (round off to 2 decimal places).";
					Answer = profitPer.ToString () + "%";

				} else if (selector == 2) {
				
					num = Random.Range (2, 6);
					num = num * 10;
					CP = 10 * Random.Range (2, 10);
					CP = CP * num;
					discount = Random.Range (2, 10);
					discount = discount * 5;
					QuestionText.text = "A fruit seller buys " + num.ToString () + " apples for Rs. " + CP.ToString () + " and sells them at " + discount.ToString () + "% gain. Find the selling price (in Rs.) of apples per piece.";

					disPer = (float)((CP * discount) / 100);
					disPer = (float)((CP + disPer) / num);
					disPer = MathFunctions.GetRounded (disPer, 2);
					Answer = disPer.ToString ();
				
				} else if (selector == 3) {
				
					CP = 20 * Random.Range (22, 41);
					int profitPer = 5 * Random.Range (1, 10);
					SP = CP + (CP * profitPer) / 100;
					QuestionText.text = "By selling a pizza for Rs. " + SP.ToString () + ", a profit of " + profitPer.ToString () + "% is made. Find its cost price (in Rs.).";
					Answer = CP.ToString ();

				} else if (selector == 4) {
				
					CP = 20 * Random.Range (22, 41);
					int lossPer = 5 * Random.Range (1, 10);
					SP = CP - (CP * lossPer) / 100;
					QuestionText.text = "A man sells his watch for Rs. " + SP.ToString () + " and makes a loss of " + lossPer.ToString () + "%. Find its cost price (in Rs.).";
					Answer = CP.ToString ();

				} else if (selector == 5) {
				
					disPer = 5 * Random.Range (2, 10);
					MP = 20 * Random.Range (10, 50);
					QuestionText.text = "The discount offered on a football with marked price as Rs. " + MP.ToString () + " is " + disPer.ToString () + "%. Find the discount amount (in Rs).";
					discount = (MP * disPer) / 100;
					Answer = discount.ToString ();

				} else if (selector == 6) {
					
					disPer = 5 * Random.Range (2, 10);
					MP = 20 * Random.Range (10, 50);
					QuestionText.text = "The discount offered on a football with marked price Rs. " + MP.ToString () + " is " + disPer.ToString () + "%. Find the selling price of the football (in Rs).";
					discount = (MP * disPer) / 100;
					SP = MP - (int)discount;
					Answer = SP.ToString ();
				
				}
				 	 
			} else if (level == 3) {
				selector = GetRandomSelector (1, 8);

				if (selector == 1) {
					
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (5, 20);
					num1 = Random.Range (2, num - 1);
					num2 = num - num1;
					CP = 100 * Random.Range (5, 15);
					SPm = 100 * Random.Range (3, CP / 100);
					do {
						SP = 100 * Random.Range (CP / 100 + 1, CP / 100 + 6);
					} while ((num1 * SP) + (num2 * SPm) - (num * CP) == 0);

					profitPer = ((float)((num1 * SP) + (num2 * SPm) - (num * CP)) / (float)(num * CP)) * 100f;
					ProfitOrLoss = profitPer > 0 ? "profit" : "loss";
					profitPer = Mathf.Abs (MathFunctions.GetRounded (profitPer, 2));

					QuestionText.text = "A dealer bought " + num.ToString () + " chairs at Rs. " + CP.ToString () + " each. " + num1.ToString () + " of them are sold at Rs. " + SP.ToString () + " each and the remaining at Rs. " + SPm.ToString () + " each. Find the " + ProfitOrLoss + " percent (round off to 2 decimal places).";
					Answer = profitPer.ToString () + "%";
				
				} else if (selector == 2) {
					
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
				
					num1 = Random.Range (2, 10);
					num2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 10);

					profitPer = ((float)num1 / (float)num2) - 1;
					profitPer = (float)(profitPer * 100);
					ProfitOrLoss = profitPer > 0 ? "profit" : "loss";
					profitPer = Mathf.Abs (MathFunctions.GetRounded (profitPer, 2));

					QuestionText.text = "A trader sells his article in the ratio " + num1.ToString () + " : " + num2.ToString () + " to its cost price. What is his " + ProfitOrLoss + " percent (round off to 2 decimal places)?";
					Answer = profitPer.ToString () + "%";

				} else if (selector == 3) {
					
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					CP = 20 * Random.Range (25, 100);
					num = 10 * Random.Range (11, 50);
					int profitPer = 5 * Random.Range (2, 10);
					QuestionText.text = "Jack buys an old cycle for Rs. " + (CP - num).ToString () + " and spends Rs. " + num.ToString () + " on its repair. What should be the selling price in order to make a profit of " + profitPer.ToString () + "% ?";
					profit = (CP * profitPer) / 100;
					SP = CP + profit;
					Answer = SP.ToString ();

				} else if (selector == 4) {
				
					MCQ.SetActive (true);
					numPad.SetActive (false);
					num = 5 * Random.Range (2, 10);
					num1 = 5 * Random.Range (num / 5 - 1, num / 5 + 2);
					MP = 20 * Random.Range (50, 100);

					QuestionText.text = "Offer A : A shopkeeper offers " + num.ToString () + "% discount on a shirt.\nOffer B : Another shopkeeper gives flat Rs. " + ((num1 * MP) / 100).ToString () + " off.\nMarked price of the shirt is Rs. " + MP.ToString () + ". Which is a better offer?";

					if (num1 < num)
						Answer = "A";
					else if (num1 > num)
						Answer = "B";
					else
						Answer = "Equal";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "A";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "B";
					MCQ.transform.Find ("Option3").Find ("Text").GetComponent<Text> ().text = "Equal";
				
				} else if (selector == 5) {
					
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;

					num = Random.Range (2, 7);
					num1 = Random.Range (1, num);
					num = num * 10;
					num1 = num1 * 10;

					QuestionText.text = "A dishonest trader increases the marked price of his products by " + num.ToString () + "% and then offers a discount of " + num1.ToString () + "%. Find his profit percent.";
					Answer = (((100 + num) * (100 - num1)) / 100 - 100).ToString () + "%";

				} else if (selector == 6) {
					
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					SP = 100 * Random.Range (10, 40);
					profit = 5 * Random.Range (1, 10);
					loss = 5 * Random.Range (profit / 5 - 2, profit / 5 + 3);

					profitPer = (float)(100 + profit) / (float)100;
					profitPer = (float)SP / (float)profitPer;
					lossPer = (float)(100 - loss) / (float)100;
					lossPer = (float)SP / (float)lossPer;
					lossPer = (profitPer + lossPer);
					profitPer = (float)(2 * SP - lossPer);
					profitPer = (float)profitPer / (float)lossPer;
					profitPer = profitPer * 100f;
					ProfitOrLoss = profitPer > 0 ? "profit" : "loss";
					profitPer = Mathf.Abs (MathFunctions.GetRounded (profitPer, 2));

					QuestionText.text = "John sold 2 cameras at Rs. " + SP.ToString () + " each. He makes a profit of " + profit.ToString () + "% on one and loss of " + loss.ToString () + "% on the other. Find the net " + ProfitOrLoss + " percent (round off to 2 decimal places).";
					Answer = profitPer.ToString () + "%";
				
				} else if (selector == 7) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;

					CP = Random.Range (5, 15);
					num = 10 * Random.Range (10, 21);
					profit = 10 * Random.Range (1, 5);
					num1 = Random.Range (5, num / 2);

					while ((num * CP * (100 + profit)) % ((num - num1) * 100) != 0) {
						num1 = Random.Range (5, num / 2);
						num = 10 * Random.Range (10, 21);
					}
					
					QuestionText.text = "Jenny buys " + num.ToString () + " toys at Rs. " + CP.ToString () + " each. " + num1.ToString () + " of them break. What should be the SP of the remaining toys per toy so that she makes a " + profit.ToString () + "% profit overall.";
			
					Answer = ((num * CP * (100 + profit)) / ((num - num1) * 100)).ToString ();
				}
			}
				
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
			else if (value == 13) {   // -
				if (checkLastTextFor (new string[1]{ "-" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
			else if (value == 14) {   // -
				if (checkLastTextFor (new string[1]{ "%" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "%";
			}
		}

	}
}
