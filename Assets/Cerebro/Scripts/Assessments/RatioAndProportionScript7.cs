using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;


namespace Cerebro {
	public class RatioAndProportionScript7 : BaseAssessment {

		public TEXDraw subQuestionText;
		private int num, num1, num2, num3, num4, num5, num6;
		float frac,frac1,frac2;
		private string Answer;

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "RAP07", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}

		public override void SubmitClick(){
			if (ignoreTouches || userAnswerText.text == "") {
				return;
			}
			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = true;
			string[] answerSplits;
			string[] userAnswerSplits;

			questionsAttempted++;
			updateQuestionsAttempted ();
			if (level == 1 && selector == 3) {
				if (!userAnswerText.text.Contains (":") || !userAnswerText.text.Contains ("/"))
					correct = false;
				else if (Answer.Contains (":") && Answer.Contains ("/")) {
					answerSplits = Answer.Split (new string[] { ":" }, System.StringSplitOptions.None);
					userAnswerSplits = userAnswerText.text.Split (new string[] { ":" }, System.StringSplitOptions.None);

					var fraction2Splits = answerSplits [1].Split (new string[]{ "/" }, System.StringSplitOptions.None);
					var fraction1Splits = userAnswerSplits [1].Split (new string[]{ "/" }, System.StringSplitOptions.None);

					if (fraction1Splits [0].Equals (fraction2Splits [0]) && fraction1Splits [1].Equals (fraction2Splits [1]))
						correct = true;
					else
						correct = false;
				} else
					correct = false;
			}
			else if (level == 1 && selector == 4) {
				if (!userAnswerText.text.Contains (":") || !userAnswerText.text.Contains ("/"))
					correct = false;
				else if (Answer.Contains (":") && Answer.Contains ("/")) {
					answerSplits = Answer.Split (new string[] { ":" }, System.StringSplitOptions.None);
					userAnswerSplits = userAnswerText.text.Split (new string[] { ":" }, System.StringSplitOptions.None);
					if (!userAnswerText.text.Contains (":") || (!userAnswerText.text.Contains ("/")))
						correct = false;
					var fraction3Splits = answerSplits [0].Split (new string[]{ "/" }, System.StringSplitOptions.None);
					var fraction4Splits = userAnswerSplits [0].Split (new string[]{ "/" }, System.StringSplitOptions.None);

					if (fraction3Splits [0].Equals (fraction4Splits [0]) && fraction3Splits [1].Equals (fraction4Splits [1]))
						correct = true;
					else
						correct = false;
				} else
					correct = false;
			}
			else if (userAnswerText.text.Contains (":") || userAnswerText.text.Contains ("/")) {
				answerSplits = Answer.Split (new string[] { ":" }, System.StringSplitOptions.None);
				if (userAnswerText.text.Contains (":")) {
					
					userAnswerSplits = userAnswerText.text.Split (new string[] { ":" }, System.StringSplitOptions.None);
				}
				else {

					userAnswerSplits = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
				}
				if (answerSplits.Length == userAnswerSplits.Length) {
					for (var i = 0; i < answerSplits.Length; i++) {
						float answer = 0;
						float userAnswer = 0;

						if (float.TryParse (answerSplits [i], out answer)) {
							answer = float.Parse (answerSplits [i]);
						} else {
							correct = false;
							break;
						}
						if (float.TryParse (userAnswerSplits [i], out userAnswer)) {
							userAnswer = float.Parse (userAnswerSplits [i]);
						} else {
							correct = false;
							break;
						}
						if (answer != userAnswer) {
							correct = false;
							break;
						}
					}
				} else
					correct = false;
			}
			else {
				if (Answer.Contains (":") || Answer.Contains ("/")) {
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

				UpdateStreak(8,12);

				updateQuestionsAttempted ();
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
				userAnswerText.text = "";
				userAnswerText.color = MaterialColor.textDark;
				ignoreTouches = false;
			} else {
				userAnswerText.text = Answer.ToString ();
				userAnswerText.color = MaterialColor.green800;
			}
			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			userAnswerText.color = MaterialColor.green800;
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 0))
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
			// Generating the parameters

			level = Queslevel;
			GeneralButton.gameObject.SetActive (true);
			answerButton = GeneralButton;
			subQuestionText.gameObject.SetActive(false);
			QuestionText.gameObject.SetActive (true);

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}


			if (level == 1) {
			
				selector = GetRandomSelector (1, 8);

				if (selector == 1) {
				
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 10);
					num2 = Random.Range (2, 6) + num1;
					num3 = Random.Range (1, 10);
					num4 = Random.Range (1, 10);
					num5 = Random.Range (2, 6) + num4;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express the given ratio in their lowest terms.";
					subQuestionText.text = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "} : " + num3.ToString () + " \\frac{" + num4.ToString () + "}{" + num5.ToString () + "}";
					num1 = (num2 * num) + num1;
					num4 = (num3 * num5) + num4;
					num1 = (num1 * num5);
					num2 = num2 * num4;
					int hcf = MathFunctions.GetHCF (num1, num2);
					num1 = num1 / hcf;
					num2 = num2 / hcf;
					Answer = num1.ToString () + ":" + num2.ToString ();
				} else if (selector == 2) {
				
					num = Random.Range (3, 30);
					num = num * 10;
					num1 = Random.Range (1, 15);
					num1 = num1 * 10;
					QuestionText.text = "Find the ratio of " + num.ToString () + "g and " + num1.ToString () + "g.";
					int hcf = MathFunctions.GetHCF (num, num1);
					num = num / hcf;
					num1 = num1 / hcf;
					Answer = num.ToString () + ":" + num1.ToString ();
				} else if (selector == 3) {
				
					num = Random.Range (2, 35);
					num1 = Random.Range (3, 35);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express in the form of 1:n where n is in its lowest form.";
					subQuestionText.text = num.ToString () + ":" + num1.ToString ();
					int hcf = MathFunctions.GetHCF (num1, num);
					num = num / hcf;
					num1 = num1 / hcf;
					Answer = "1:" + num1.ToString () + "/" + num.ToString ();
				} else if (selector == 4) {
				//010/013:1
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 10);
					num2 = Random.Range (2, 6)+num1;
					num3 = Random.Range (10, 100);
					frac = (float)num3 / (float)10;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express in the form of m:1 where m is in its lowest form.";
					subQuestionText.text = num.ToString () + "\\frac{" + num1.ToString () + "}{" + num2.ToString () + "} : " + frac.ToString ();
					num4 = (num2 * num) + num1;
					num5 = (num4 * 10);
					num6 = (num2 * num3);
					int hcf = MathFunctions.GetHCF(num5, num6);
					num5 = num5 / hcf;
					num6 = num6 / hcf;
					Answer = num5.ToString () + "/" + num6.ToString () + ":1";
				} else if (selector == 5) {

					num = Random.Range (1, 10);
					num1 = Random.Range (1, 10);
					num2 = Random.Range (1, 10)+num1;
					num3 = Random.Range (1, 10)+num1;
					QuestionText.text = "Which ratio is greater " + num.ToString () + ":" + num1.ToString () + " or " + num2.ToString () + ":" + num3.ToString () + "?";
					frac1 = (float)num / (float)num1;
					frac2 = (float)num2 / (float)num3;
				
					if (frac1 > frac2) {
						Answer = num.ToString () + ":" + num1.ToString ();
					
					} else {
						Answer = num2.ToString () + ":" + num3.ToString ();
					}
				} else if (selector == 6) {

					num2 = Random.Range (2, 10);
					num = num2 * num2;
					num3 = num2 + 2;
					num1 = num3 * num3;
					QuestionText.text = num.ToString () + ", x and " + num1.ToString () + " are in continued proportion. Find x.";
					num4 = num2 * num3;
					Answer = num4.ToString ();
				} else if (selector == 7) {
					num = Random.Range (1, 10);
					num1 = Random.Range (2, 10)+num;
					num3 = Random.Range (1, 6)+num;
					num2 = num3 * num;
					QuestionText.text = num.ToString () + "," + num1.ToString () + " and " + num2.ToString () + ",x are in proportion.Find x.";
					Answer = (num1 * num3).ToString ();
				}
			} else if (level == 2) {
			
				selector = GetRandomSelector (1, 10);
		

				if (selector == 1) {
				
					num = Random.Range (5, 16);
					num1 = Random.Range (5, 16);

					QuestionText.text = "The price of potatoes has changed from Rs. " + num.ToString () + " to Rs. " + num1.ToString () + ". What is the ratio of the increased (or decreased) price to the original price?";
					int hcf = MathFunctions.GetHCF (num, num1);
					num = num / hcf;
					num1 = num1 / hcf;
					Answer = num1.ToString () + ":" + num.ToString ();
				} else if (selector == 2) {
				
					num = Random.Range (1, 5);
					num1 = num + 4;
					num2 = Random.Range (2, 10);
					num2 = num2 * num1;
					QuestionText.text = "The ages of Mary and Max are in the ratio of " + num.ToString () + ":" + num1.ToString () + ". If Max is " + num2.ToString () + " years old, what is the age of Mary?";
					num3 = num2 / num1;
					num4 = num3 * num;
					Answer = num4.ToString ();
				} else if (selector == 3) {
				
					num = Random.Range (1, 11);
					num1 = Random.Range (2, 6)+num;
					num2 = num1 * 10;
					num3 = Random.Range (2, 11);
					QuestionText.text = "The ages of Frodo and Gandalf are in the ratio of " + num.ToString () + ":" + num1.ToString () + ". Gandalf is " + num2.ToString () + " years old. What will be the ratio of their ages "+num3.ToString()+" years later?";
					num4 = num2 / num1;
					num5 = num4 * num1 + num3;
					num6 = num4 * num + num3;
					int hcf = MathFunctions.GetHCF (num5, num6);
					num5 = num5 / hcf;
					num6 = num6 / hcf;
					Answer = num6.ToString () + ":" + num5.ToString ();
				} else if (selector == 4) {
				
					num = Random.Range (1, 11);
					num1 = num + 2;
					num2 = Random.Range (5, 21);
					num2 = num2 * 2;
					QuestionText.text = "The ratio of lions to tigers in a zoo is " + num1.ToString () + ":" + num.ToString () + ". If there are " + num2.ToString () + " more lions than tigers. Find the number of tigers.";
					num3 = num2 / 2;
					num3 = num3 * num;
					Answer = num3.ToString ();
				} else if (selector == 5) {
				
					num = Random.Range (2, 10);
					num1 = Random.Range (2, 10);
					num2 = Random.Range (2, 10);
					num3 = Random.Range (1, 10);
					num4 = Random.Range (1, 11);

					int price = (num * num1 + num1 * num2 + num2 * num) * num3;
					num4 = num4 * price;
					int x = num * num1 * num2 * num3;
				
					QuestionText.text = "Divide "+num4.ToString()+" among Ram, Lakshman and Bharat in the ratio 1/" + num.ToString () + " : 1/" + num1.ToString () + " : 1/" + num2.ToString () + ". How much is Ram's share?";
					float ans = (float)(num4 * x) /(float) ((price) * num);
					Answer = ans.ToString ();
				} else if (selector == 6) {
					num = Random.Range (2, 7);
					num2 = num * num;
					num1 = Random.Range (7, 16);
					num3 = num1 * num1;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Find the mean proportionality of the proportion: "; 
					subQuestionText.text= num2.ToString () + ":x::x:" + num3.ToString ();

					Answer = (num * num1).ToString ();
				} else if (selector == 7) {
				
					num2 = Random.Range (2, 7);
					num = num2 * num2;
					num3 = Random.Range (7, 16);
					num1 = num3 * num3;
					QuestionText.text = "Find the mean proportional between " + num.ToString () + " and " + num1.ToString () + ".";
					Answer = (num2 * num3).ToString ();
				} else if (selector == 8) {
				
					num = Random.Range (7,16);
					num1 = Random.Range (2,7);
					QuestionText.text = "An equilateral triangle's side is enlarged in the ratio " + num.ToString () + ":" + num1.ToString () + ". Find the ratio of the areas of the new and old triangle.";
					num3 = num * num;
					num4 = num1 * num1;
					int hcf = MathFunctions.GetHCF (num3, num4);
					num3 = num3 / hcf;
					num4 = num4 / hcf;
					Answer = num3.ToString () + ":" + num4.ToString ();
				} else if (selector == 9) {
				//calculation
			
					num3 = Random.Range (2, 7); //d
					num4 = Random.Range (13, 18); //e
					num1 = Random.Range (1, 6); //b
					num2 = Random.Range (6, 11); //c
					num5=Random.Range(5,12)*((num2*num3)+(num1*num4));
					num6 = (num5 * num2 * num3) / ((num2 * num3) + (num1 * num4));
					QuestionText.text = "A and B together have "+num5.ToString()+ ".If " + num1.ToString () + "/" + num2.ToString () + " of A's amount is equal to " + num3.ToString () + "/" + num4.ToString () + " of B's amount. Find A's amount.";
					Answer = num6.ToString ();
				}
			} else if (level == 3) {
			
				selector = GetRandomSelector (1, 8);
				GeneralButton.gameObject.SetActive (true);
				answerButton = GeneralButton;

				if (selector == 1) {
				
					num = Random.Range (10, 21);
					num = num * 100;
					num1 = Random.Range (10, 21);
					num1 = num1 * 100;
					num2 = Random.Range (10, 21);
					num2 = num2 * 100;
					num4 = (num + num1 + num2);
					num3 = (num4 * 20) / 100;
					QuestionText.text = "Karan, Krishh and Kavya invested Rs." + num.ToString () + ", Rs." + num1.ToString () + ", Rs." + num2.ToString () + " respectively in a business. If they made a profit of Rs." + num3.ToString () + ". What is Krishh's share of profit?";
					num4 = (num1 * num3) / num4;
					Answer = num4.ToString ();
				} else if (selector == 2) {
				
					num4 = Random.Range (5, 21);
					num = num4 * 10;
					num3 = num / 20;
					num1 = Random.Range (1, num3);
					num2 = (num / 10) - num1;
					QuestionText.text = "In a school exam there are " + num.ToString () + " students appearing. The ratio of fail to pass is " + num1.ToString () + ":" + num2.ToString () + ". Find the new ratio of fail to pass in case 10 more students would have passed the exam.";
					num5 = (num * num1) / num4;
					num6 = (num * num2) / num4;
					num6 = num6 + 10;
					num5 = num5 - 10;
					int hcf = MathFunctions.GetHCF (num5, num6);
					num5 = num5 / hcf;
					num6 = num6 / hcf;
					Answer = num5.ToString () + ":" + num6.ToString ();

				} else if (selector == 3) {

					num = Random.Range (1, 11);
					num = num * 5 * Random.Range(2,6);
					num1 = Random.Range (1, 11) * 5 * Random.Range(2,6);
					num2 = Random.Range (1, num);
					num3 = num - num2;
					int hcf1 = MathFunctions.GetHCF (num2, num3);
					num2 = num2 / hcf1;
					num3 = num3 / hcf1;
					num4 = Random.Range (1, num1);
					num5 = num1 - num4;
					int hcf2 = MathFunctions.GetHCF (num4, num5);
					num4 = num4 / hcf2;
					num5 = num5 / hcf2;
					QuestionText.text = "Two containers A and B contain " + num.ToString () + " litres and " + num1.ToString () + " litres of diluted milk respectively. The ratio of pure milk to water in A is " + num2.ToString () + ":" + num3.ToString () + " and " + num4.ToString () + ":" + num5.ToString () + " in B. A and B are emptied into a bigger container C. What is the ratio of pure milk to water in container C?";
					num2 = num2 + num4;
					num3 = num3 + num5;
					int hcf = MathFunctions.GetHCF (num2, num3);
					num2 = num2/hcf;
					num3 = num3 / hcf;
					Answer = num2.ToString () + ":" +num3.ToString ();
				} else if (selector == 4) {
				

					num = Random.Range (2, 10);
					num1 = Random.Range (2, 10)+num;
					num2 = Random.Range (2, 10);
					num3 = Random.Range (2, 10)+num2;
					QuestionText.text = "The ratio of Gas A to Gas B by volume, in air is " + num.ToString () + ":" + num1.ToString () + ". For equal volumes of Gas A and Gas B their weights are in the ratio of " + num2.ToString () + ":" + num3.ToString () + ". Find the ratio of Gas A to Gas B in the air by weight.";
					num4 = num * num2;
					num5 = num1 * num3;
					int hcf = MathFunctions.GetHCF (num4, num5);
					num4 = num4 / hcf;
					num5 = num5 / hcf;
					Answer = num4.ToString () + ":" + num5.ToString ();
				} else if (selector == 5) {
				
					num1 = Random.Range (2, 10);
					QuestionText.text = "If x:y = 1:" + num1.ToString () + ". Find 2x+3y : x+4y.";
					num = 2 + (3 * num1);
					num2 = 1 + (4 * num1);
					int hcf = MathFunctions.GetHCF (num, num2);
					num = num / hcf;
					num2 = num2 / hcf;
					Answer = num.ToString () + ":" + num2.ToString ();
				} else if (selector == 6) {
				
					num1 = Random.Range (2,10);
					num2 = Random.Range (2,10);
					num3 = Random.Range (2, 10);
					num = (num1 + num2 + num3) * 10*Random.Range(2,9);

					QuestionText.text = "A bag contains " + num.ToString () + " coins in the form of Rs. 5, Rs. 2 and Re. 1 coins in the ratio " + num1.ToString () + ":" + num2.ToString () + ":" + num3.ToString () + ". What is the total value of the Rs. 2 coins?";

					num4 = (num2 * num) / (num1+num2+num3);
					int ans = 2 * num4;
					Answer = ans.ToString ();
						
				}else if(selector==7){
				
					num = Random.Range (1, 8);
					num1 = Random.Range (5, 12);
					num2 = Random.Range (12, 16);
					QuestionText.text="If "+num.ToString()+"A = "+num1.ToString()+"B = "+num2.ToString()+"C. Find A:B:C.";
					int lcm = MathFunctions.GetLCM(num, num1);

					int lcm1 = MathFunctions.GetLCM (lcm, num2);
				
					num = (int)lcm1 /(int)num;
			
					num1= lcm1 / num1;

					num2 = lcm1 / num2;


					Answer = num.ToString () + ":" + num1.ToString () + ":" + num2.ToString ();
				}

			}

						
			CerebroHelper.DebugLog (Answer);
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
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
			} else if (value == 12) {   // :
				if(checkLastTextFor(new string[1]{":"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ":";
			} else if (value == 13) {   // .
				if(checkLastTextFor(new string[1]{"."})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			}
			else if (value == 14) {   // /
				if(checkLastTextFor(new string[1]{"/"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			} 
		}
	}
}
