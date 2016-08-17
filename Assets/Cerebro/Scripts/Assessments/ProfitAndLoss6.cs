using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class ProfitAndLoss6 : BaseAssessment {

		private int SP;
		private int CP, MP;
		private int profit;
		private int loss;
		private float discount;
		private float profitPer;
		private float lossPer;
		private float disPer,newSugar;
		private int rate;
		private int ans;
		private int CPm, CPmug, CPs;
		private int SPm, SPmug, SPs;
		private int num,num1,num2;

		private string Answer;
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
			base.Initialise ("M", "PL06", "S01", "A01");

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

			SetLevelToLocalDB (1);

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
				}
			} else {
				if (answer == float.Parse (Answer)) {
					correct = true;
				}
			}

			if(correct == true) {
				if (Queslevel == 1) {
					increment = 5;
				} else if (Queslevel == 2) {
					increment = 10;
				} else if (Queslevel == 3) {
					increment = 15;
				}  else if (Queslevel == 4) {
					increment = 15;
				} 

				UpdateStreak(6,12);

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




		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters


			SPQuestion.text = "Rs. " + SP.ToString ();
			CPQuestion.text = "Rs. " + CP.ToString ();


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

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}




			if (level == 1) {
				selector = GetRandomSelector (1, 8);


				if (selector == 1) {
					QuestionText.text = "Find the missing data";

					SPQuestion.gameObject.SetActive (true);
					CPQuestion.gameObject.SetActive (true);
					SPText.gameObject.SetActive (true);
					CPText.gameObject.SetActive (true);
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					SP = (int)Random.Range (25, 100);

					SP = SP * 10;

					SPQuestion.text ="Rs. "+ SP.ToString();
					CP = (int)Random.Range (25, 100);
					CP = CP * 10;
					CPQuestion.text = "Rs. "+CP.ToString();
					if (SP > CP) {
						profit = SP - CP;
						ProfitText.gameObject.SetActive (true);
						ProfitQuestion.gameObject.SetActive (true);
						Answer = profit.ToString ();
					} else {
						loss = CP - SP;
						LossText.gameObject.SetActive (true);
						LossQuestion.gameObject.SetActive (true);
						Answer = loss.ToString ();
					}

				} else if (selector == 2) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					SP = Random.Range (25, 100);
					SP = SP * 10;
					profit = Random.Range (25, 100);
					profit = 10 * profit;
					QuestionText.text = "A fridge is sold for Rs " + SP.ToString () + " thereby making a profit of Rs " + profit.ToString () + ". What was the cost price? ";
					Answer = (SP - profit).ToString ();
				} else if (selector == 3) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					CP = Random.Range (4, 10);
					int NewCP = 15 * CP;
					rate = Random.Range (2, 11);
					ans = Random.Range(1,rate+1);
					SP = Random.Range (10, 25);
					int NewSP=SP*5;
					QuestionText.text = "John buys mangoes at the rate of " + rate.ToString () + " for Rs " + (rate*NewCP).ToString () + " and he sold at the rate of " + ans.ToString () + " for Rs " + (ans * NewSP).ToString () + ". Find the Profit on one mango.";
					Answer = (SP - CP).ToString();	
				} else if (selector == 4) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					CP = Random.Range (4, 10);
					CP = 15 * CP;
					rate = Random.Range (2, 11);
					ans = Random.Range(1,rate+1);
					SP = Random.Range (10, 25);
					SP=SP*5;
					QuestionText.text = "Rahul buys oranges at the rate of " + rate.ToString () + " for Rs. " + (rate*CP).ToString () + " and he sold at the rate of " + ans.ToString () + " for Rs." + (ans * SP).ToString () + ". Find the profit or loss made on selling one orange.";
					Answer = (SP - CP).ToString ();

				} else if (selector == 5) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					CP = Random.Range (10, 25);
					CP=CP*5;
					rate = Random.Range (2, 11);
					ans = Random.Range(1,rate+1);
					SP = Random.Range (10, 25);
					SP=SP*5;
					QuestionText.text = "Saloni buys apples at the rate of " + rate.ToString () + " for Rs. " + (rate*CP).ToString () + " and she sold at the rate of " + ans.ToString () + " for Rs." + (ans * SP).ToString () + ". Find the profit or loss made by selling a dozen apples";

					Answer = ((SP - CP) * 12).ToString ();

				} else if (selector == 6) {

					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					CP = Random.Range (1000, 4500);
					CP = CP * 10;
					SP = Random.Range (1000, 4500);
					SP = SP * 10;
					CPmug = Random.Range (50, 100);
					CPs = Random.Range (1000, 4000);
					CPs = CPs * 10;
					CPm = Random.Range (1000, 4000);
					CPm = CPm * 10;
					SPs = Random.Range (1000, 4000);
					SPs = SPs * 10;
					SPm = Random.Range (1000, 4000);
					SPm = SPm * 10;
					SPmug = Random.Range (50, 100);


					QuestionText.text = "Saurav buys a fridge, a mixer, a sofa set and a coffee mug for Rs." + CP.ToString () + ", Rs." + CPm.ToString () + ", Rs." + CPs.ToString () + ", Rs." + CPmug.ToString () + " respectively. If he sells the items for Rs."
						+ SP.ToString () + ", Rs." + SPm.ToString () + ", Rs." + SPs.ToString () + " and Rs." + SPmug.ToString () + ". Then how much profit or loss did he incur overall?";

					Answer = ((SP + SPm + SPmug + SPs) - (CP + CPm + CPmug + CPs)).ToString ();
				} else if (selector == 7) {
					CP = Random.Range (2, 10);
					CP = CP * 100;
					SP = Random.Range (2, 10);
					SP = SP * 100;

					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					SPQuestion.gameObject.SetActive (true);
					CPQuestion.gameObject.SetActive (true);
					SPText.gameObject.SetActive (true);
					CPText.gameObject.SetActive (true);
					SPQuestion.text = "Rs. " + SP.ToString();
					CPQuestion.text = "Rs. " + CP.ToString();
					QuestionText.text = "Calculate profit or loss and express your answer in %. Round off your answer to the nearest 2 decimal digits.";
					profit = SP - CP;
					profitPer = ((float)profit /(float) CP) * 100;
					profitPer=Mathf.Round (profitPer*100)/(float)100;
					Answer = profitPer.ToString ();
				}
			}

			if (level == 2) {
				selector = GetRandomSelector (1, 8);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;


				if (selector == 1) {
					CP = Random.Range (5, 21);
					CP = CP * 5;
					SP = Random.Range (5, 15);

					QuestionText.text = "A trader buys bananas for Rs." + CP.ToString () + " a dozen and sells them at Rs." + SP.ToString () + " per piece. Calculate his profit or loss in selling a dozen of bananas.";
					if (SP > (CP / 12))
						Answer = ((SP * 12) - CP).ToString ();
					else
						Answer = (CP - (SP * 12)).ToString ();
				} else if (selector == 2) {

					CP = Random.Range (50, 100);
					SP = Random.Range (5, 15);

					QuestionText.text = "A trader buys bananas for Rs." + CP.ToString () + " a dozen and sells them at Rs." + SP.ToString () + " per piece. What is his gain or loss % ? Round off your answer to the nearest 2 decimal digits.";
					if (SP > (CP / 12) || (CP/12)>SP) {
						profit = SP*12 -CP ;
						profitPer = ((float)(profit ) /(float) CP) * 100;
						profitPer = Mathf.Round (profitPer*100)/(float)100;
						Answer = profitPer.ToString ();
					} 
				} else if (selector == 3) {
					CP = Random.Range (2000, 5000);
					CP = 10 * CP;
					SP = Random.Range (2000, 5000);
					SP = 10 * SP;
					profit = SP - CP;

					QuestionText.text = "The making cost of a TV is Rs." + CP.ToString () + " and it is sold at a company store for Rs." + SP.ToString () + ". Calculate the profit or loss in selling 1 TV.";

					Answer = profit.ToString ();

				} else if (selector == 4) {
					CP = Random.Range (2000, 5000);
					CP = 10 * CP;
					SP = Random.Range (2000, 5000);
					SP = 10 * SP;
					profit = SP - CP;

					num = Random.Range (2, 30);
					QuestionText.text = "The making cost of a TV is Rs." + CP.ToString () + " and it is sold at a company store for Rs." + SP.ToString () + ". Calculate the profit or loss in selling " + num.ToString () + " TVs.";

					Answer = (num * profit).ToString ();

				} else if (selector == 5) {

					disPer = Random.Range (5, 30);
					MP = Random.Range (100, 400);
					MP = MP * 100;
					discount = (MP * disPer) / 100;
					SP = (int)(MP - discount);
					QuestionText.text = " Find SP of a computer which was bought at a discount of " + disPer.ToString () + "%. The MP for the computer is Rs." + MP.ToString ();
					Answer = SP.ToString ();
				} else if (selector == 6) {
					num = Random.Range (10, 100);
					num = num * 10;
					CP = Random.Range (1000, 5000);
					CP = 10 * CP;
					SP = Random.Range (1000, 5000);
					SP = 10 * SP;
					QuestionText.text = "Om bought a secondhand sofa for Rs." + CP.ToString () + " from Store A. He then took it to store B and sold it at Rs." + SP.ToString () + ". If it cost him Rs." + num.ToString () + " to travel from store A to store B, did he make a profit or loss? Find its %. Round off your answer to the nearest 2 decimal digits.";
					if (SP > CP + num || (CP+num)>SP) {
						profit = SP - (CP + num);
						disPer = ((float)profit / (float)(CP + num)) * 100;;
						disPer = Mathf.Round (disPer*100)/(float) 100;
						Answer = disPer.ToString ();
					} 
				} else if (selector == 7) {

					MP = Random.Range (150, 200);
					MP = MP * 10;

					SP = Random.Range (100, 150);
					SP = SP * 10;
					QuestionText.text = "In a shop, a school bag is marked for Rs." + MP.ToString () + " and sold at Rs." + SP.ToString () + ". Find the discount %. Round off your answer to the nearest 2 decimal digits";
					disPer=((float)(MP - SP) / (float)MP)*100;
					disPer = Mathf.Round (disPer*100)/(float)100;
					Answer = disPer.ToString ();
				}
			}

			if (level == 3) {
				selector =GetRandomSelector(1, 12);


				if (selector == 1) {

					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (2, 10);
					CP = Random.Range (50, 200);
					CP = CP * 10;
					profit = Random.Range (5, 50);
					QuestionText.text = "A ninja bought " + num.ToString () + " swords at Rs." + CP.ToString () + " per sword.He then sold them to his friends and made " + profit.ToString () + "% profit on the whole. What was his SP? Round off your answer to the nearest 2 decimal digits";
					disPer = (float)((num * CP * profit) / 100);
					disPer = Mathf.Round(disPer*100)/(float)100;
					Answer = ((num * CP) + disPer).ToString ();			// make corrections
				} else if (selector == 2) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (5, 11);
					num1 = Random.Range(1,num);
					num2 = num - num1;
					CP = Random.Range (10, 60);
					CP = CP * Random.Range (2, 10) * 10;
					QuestionText.text = "A store buys " + num.ToString () + " watches for Rs." + CP.ToString () + " each. They sell " + num1.ToString () + " for Rs." + (CP + 100).ToString () + " and other " + num2.ToString () + " for Rs." + (CP - 100).ToString () + ". Find the amount of their profit or loss.";
					Answer = ((num1 * (CP + 100) + num2 * (CP - 100)) - num * CP).ToString ();

				} else if (selector == 3) {

					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (2, 11);
					num = num * 10;
					num1 = Random.Range (2, 30);
					num2 = Random.Range (5, 30);
					QuestionText.text = "Sugar costs Rs." + num.ToString () + " a kg. Anura buys " + num1.ToString () + " kgs sugar a month. The price of sugar has gone up by " + num2.ToString () + "% . How much Anura pays before the price rises?";
					Answer = (num1 * num).ToString ();
				} else if (selector == 4) {

					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (2, 11);
					num = num * 10;
					num1 = Random.Range (2, 30);
					num2 = Random.Range (5, 30);
					QuestionText.text = "Sugar costs Rs." + num.ToString () + " a kg. Anura buys " + num1.ToString () + " kgs sugar a month. The price of sugar has gone up by " + num2.ToString () + "% . What is the new price for 1 kg of sugar? Round off your answer to the nearest 2 decimal digits.";
					disPer = (float)(num + (float)(num * num2) / 100);
					disPer = Mathf.Round (disPer*100)/(float)100;
					Answer =disPer.ToString ();
				} else if (selector == 5) {

					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (2, 11);
					num = num * 10;
					num1 = Random.Range (2, 25);
					num2 = Random.Range (5, 30);
					QuestionText.text = "Sugar costs Rs." + num.ToString () + " a kg. Anura buys " + num1.ToString () + " kgs sugar a month. The price of sugar has gone up by " + num2.ToString () + "% . How many kgs of sugar can Anura buy at present rate using same amount? Round off your answer to the nearest 2 decimal digits.";
					disPer = (float)((num * num1) / (float)(num + (float)(num * num2) / 100));
					disPer = Mathf.Round (disPer*100)/(float)100;
					Answer = disPer.ToString ();
				} else if (selector == 6) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (2, 11);
					num = num * 10;
					num1 = Random.Range (2, 25);
					num2 = Random.Range (5, 30);
					QuestionText.text = "Sugar costs Rs." + num.ToString () + " a kg. Anura buys " + num1.ToString () + " kgs sugar a month. The price of sugar has gone up by " + num2.ToString () + "% . By what percent has the consumption of sugar reduced in a month? Round off your answer to the nearest 2 decimal digits.";
					newSugar = (float)((num * num1) / (float)(num + (float)(num *num2) / 100));
					newSugar = Mathf.Round (newSugar*100)/(float)100;
					disPer = (float)(((num1 - newSugar) / num1) * 100);
					disPer = Mathf.Round (disPer*100)/(float)100;
					Answer = disPer.ToString ();
				} else if (selector == 7) {
					MCQ.SetActive (true);
					numPad.SetActive (false);
					num = Random.Range (2, 6);
					num1 = Random.Range(1,num);
					num2 = Random.Range (1, 10);
					num2 = 10 * num2;
					disPer =(float) ((float)(num1 / (num + num1)) * 100);
					disPer = Mathf.Round (disPer);
					QuestionText.text = "End of year sale:\n" + "Offer A-Buy " + num.ToString () + " get " + num1.ToString () + " free.\n" + "Offer B- Buy any number of items on flat discount of " + num2.ToString () + "%. Which is preferable A,B or C?";
					if (disPer > num2)
						Answer = "A";
					else if (disPer < num2)
						Answer = "B";
					else
						Answer = "Same";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "A";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "B";
					MCQ.transform.Find ("Option3").Find ("Text").GetComponent<Text> ().text = "Same";

				} else if (selector == 8) {

					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (1, 11);
					num = 10 * num;
					num1 = Random.Range (1, 4);
					num1 = 5 * num1;
					num2 = Random.Range (1, 6);
					num2 = 5 * num2;
					QuestionText.text = "MRP of a face cream of 100g is Rs." + num.ToString () + "\nOffer A- A new stock of cream has " + num1.ToString () + "% extra.\n" + "Offer B- Shopkeeper wants to clear his old stock by giving " + num2.ToString () + "% discount. Calculate SP of 100g of cream in offer A. Round off your answer to the nearest 2 decimal digits.";
					disPer = (float)(100 * num) / (100 + num1);
					disPer = Mathf.Round (disPer * 100) / (float)100;

					Answer = disPer.ToString ();
				} else if (selector == 9) {

					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (1, 11);
					num = 10 * num;
					num1 = Random.Range (1, 4);
					num1 = 5 * num1;
					num2 = Random.Range (1, 6);
					num2 = 5 * num2;
					QuestionText.text = "MRP of a face cream of 100g is Rs." + num.ToString () + "\n Offer A- A new stock of cream has " + num1.ToString () + "% extra.\n" + "Offer B- Shopkeeper wants to clear his old stock by giving " + num2.ToString () + "% discount. How much would you pay for 100g of cream in offer B? Round off your answer to the nearest 2 decimal digits.";
					disPer = (float)(num - (float)((num * num2) / 100));
					disPer = Mathf.Round (disPer*100)/(float)100;
					Answer = disPer.ToString ();

				} else if (selector == 10) {
					MCQ.SetActive (true);
					numPad.SetActive (false);
					num = Random.Range (1, 11);
					num = 10 * num;
					num1 = Random.Range (1, 4);
					num1 = 5 * num1;
					num2 = Random.Range (1, 6);
					num2 = 5 * num2;
					QuestionText.text = "MRP of a face cream of 100g is Rs." + num.ToString () + "\n Offer A- A new stock of cream has " + num1.ToString () + "% extra.\n" + "Offer B- Shopkeeper wants to clear his old stock by giving " + num2.ToString () + "% discount. Which is better offer?";
					if (((float)(100 * num) / (100 + num)) > (float)(num - (num * num2) / 100))
						Answer = "B";
					else if (((float)(100 * num) / (100 + num)) < (float)(num - (num * num2) / 100))
						Answer = "A";
					else
						Answer = "Same";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "A";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "B";
					MCQ.transform.Find ("Option3").Find ("Text").GetComponent<Text> ().text = "Same";
				} else if (selector == 11) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					num = Random.Range (1, 11);
					num = 10 * num;
					num1 = Random.Range (1, 4);
					num1 = 5 * num1;
					num2 = Random.Range (1, 6);
					num2 = 5 * num2;
					QuestionText.text = "MRP of a face cream of 100g is Rs." + num.ToString () + "\n Offer A- A new stock of cream has " + num1.ToString () + "% extra.\n" + "Offer B- Shopkeeper wants to clear his old stock by giving " + num2.ToString () + "% discount. Calculate the gain or loss in choosing offer B. Round off your answer to the nearest 2 decimal digits.";
					disPer=(float)((num *num2) / 100);
					disPer = Mathf.Round (disPer*100)/(float)100;

					Answer = disPer.ToString ();

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
		}

	}
}
