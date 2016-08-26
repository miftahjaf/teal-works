using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro
{
	public class FractionsScript7 : BaseAssessment
	{

		public TEXDraw subQuestionText;

		public GameObject MCQ;
		private string Answer;

		private int num,num1,num2,num3,num4,num5,num6;
		private float frac;
		int x, y, lcm = 0,a,b,ans;

		void Start ()
		{

			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "FRA07", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}

		void AnimateMCQOptionCorrect(string ans)
		{
			for (int i = 1; i <= 2; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
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

			if (MCQ.activeSelf) {
				if (Answer == userAnswerLaText.text) {
					correct = true;
				}
				else{
					correct = false;
					AnimateMCQOptionCorrect(Answer);

				}
			} 

 
			else {
				if (Answer.Contains ("=")) {
					string correctAnswer = Answer.Split (new string[] { "=" }, System.StringSplitOptions.None) [1];
					if (userAnswerText.text == correctAnswer) {
						correct = true;
					} else {
						Answer = correctAnswer;
						correct = false;
					}
				} else if (Answer.Contains ("/") && level == 1 && selector == 4) {
				
					if (!userAnswerText.text.Contains ("/")) {
						correct = false;
					} else {
						var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
						var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
						if ((float.Parse (userAnswers [0]) / float.Parse (userAnswers [1])) > ((float)num / (float)num1) && ((float.Parse (userAnswers [0]) / float.Parse (userAnswers [1])) < ((float)num2 / (float)num3)))
							correct = true;
						else
							correct = false;
					}

				}else if (Answer.Contains ("/")) {
					var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
					correct = MathFunctions.checkFractions (userAnswers, correctAnswers);
				}
				else {
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
					increment = 10;
				} else if (Queslevel == 3) {
					increment = 10;
				} else if (Queslevel == 4) {
					increment = 15;
				}

				UpdateStreak (6, 10);

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

			for (int i = 1; i < 3; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}
			// Generating the parameters

			level = Queslevel;

			QuestionText.gameObject.SetActive (true);
			subQuestionText.gameObject.SetActive (false);
			MCQ.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (true);
			numPad.SetActive (true);

			answerButton = GeneralButton;


			if (Queslevel > scorestreaklvls.Length) {
				level = Random.Range (1, scorestreaklvls.Length + 1);
			}
				
			if (level == 1) {
			
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {
				
					num = Random.Range (1, 10);
					num1 = Random.Range (1, 6);
					num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF (num2, num1) != 1)
						num1 = Random.Range (1, 6);
					subQuestionText.gameObject.SetActive (true);

					QuestionText.text = "Convert the given mixed fraction to a simple fraction :";
					subQuestionText.text = num.ToString () + "\\frac{" + num1.ToString () + "}{" + num2.ToString () + "} ";
					Answer = ((num * num2) + num1).ToString()+"/"+ num2.ToString();

				}else if (selector == 2) {
					
					MCQ.SetActive (true);
					numPad.SetActive (false);
					int num = Random.Range (1, 10);
					int num1 = Random.Range (1, 6);
					int num2 = Random.Range (num1 + 1, 10);
					while (MathFunctions.GetHCF (num2, num1) != 1)
						num1 = Random.Range (1, 6);
					int ansNum = ((num * num2) + num1);
					string option1 = num.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					string option2 = "";
					if (Random.Range (1, 3) == 1) {
						int rndNum = Random.Range (1, 10);
						while ((rndNum == num1) || (rndNum==num)) {
							rndNum = Random.Range (1, 10);
						}
						option2 = rndNum.ToString () + " \\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					} else {
						int rndNum = Random.Range (1, 5);
						while ((rndNum == num2) || (rndNum==num1)) {
							rndNum = Random.Range (1, 5);
						}
						option2 = num.ToString () + " \\frac{" + rndNum.ToString () + "}{" + num2.ToString () + "}";
					}
					subQuestionText.gameObject.SetActive (true);
					GeneralButton.gameObject.SetActive (false);
					QuestionText.text = "Convert the given improper fraction to mixed fraction : ";
					subQuestionText.text = "\\frac{" + ansNum.ToString () + "}{" + num2.ToString () + "}";
					Answer = option1;
					if (Random.Range (1, 3) == 1) {
						string tmp = option1;
						option1 = option2;
						option2 = tmp;
					}
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = option1;
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = option2;
				} else if (selector == 3) {
				
					num = 3 * Random.Range (2, 8);
					num1 = Random.Range (2, 6);
					QuestionText.text = "What fraction is " + num.ToString () + " apples of " + num1.ToString () + " dozen apples?";
					num2 = num1 * 12;
					int hcf = MathFunctions.GetHCF (num, num2);
					num = num / hcf;
					num2 = num2 / hcf;
					if (num2 == 1)
						Answer = num.ToString ();
					else
					Answer = num.ToString () + "/" + num2.ToString ();
				} else if (selector == 4) {
				//check logic
					num = Random.Range (1, 20);
					num1 = num + Random.Range (1, 6);
					num /= MathFunctions.GetHCF (num, num1);
					num1 /= MathFunctions.GetHCF (num, num1);
					num2 = num + Random.Range (1, 6);
					num3 = num + Random.Range (6, 9);
					while (num * 1f / num1 > num2 * 1f / num3) {
						num2 = num + Random.Range (1, 9);	
						print ("going in");
					}
					num2 /= MathFunctions.GetHCF (num2, num3);
					num3 /= MathFunctions.GetHCF (num2, num3);

					QuestionText.text = "Insert a fraction between " + num.ToString() + "/" + num1.ToString () + " and " + num2.ToString () + "/" + num3.ToString() + ".";


//					lcm = MathFunctions.GetLCM (num1, num3);
//					num1 = lcm / num1;
//					num3 = lcm / num3;
//
//					num4 = num * num1;
//					num5 = num2 * num3;
//					if (num5 - num4 == 1) {
//					
//						num4 = num4 * 2;
//						num5 = num5 * 2;
//						lcm = lcm * 2;
//					}
//					num6 = Random.Range(num4, num5);
//					int hcf = MathFunctions.GetHCF (num6+1, lcm);
//					num6 = num6 / hcf;
//					lcm = lcm / hcf;
//					if (lcm == 1)
//						Answer = num6.ToString ();
//					else
//						Answer = num6.ToString() + "/" + lcm.ToString ();

					num4 = num + num2;
					num5 = num1 + num3;
					num6 = MathFunctions.GetHCF (num4, num5);
					num4 /= num6;
					num5 /= num6;
					Answer = num4.ToString () + "/" + num5.ToString ();
				}

			} else if (level == 2) {
			
				selector = GetRandomSelector (1, 5);
				if (selector == 1) {

					num = Random.Range (2, 8);
					num1 = Random.Range (num + 1, 11);
					while(MathFunctions.GetHCF (num, num1) > 1)
						num1 = Random.Range (num + 1, 11);
					QuestionText.text = "Emma, Avan and Jamie buy " + num.ToString () + "/" + num1.ToString () + " kg of fudge which is to be divided equally among them. How much fudge (in kg) will each one of them get?";
					num2 = 3 * num1;
					int hcf = MathFunctions.GetHCF (num, num2);
					num = num / hcf;
					num2 = num2 / hcf;
					if (num2 == 1)
						Answer = num.ToString ();
					else
					Answer = num.ToString()+"/"+ num2.ToString();

				} else if (selector == 2) {
				//doubt
					num = 10 * Random.Range (2, 11);
					num1 = Random.Range (2, 7);
					num2 = Random.Range (num1 + 1, 20);
					while(MathFunctions.GetHCF (num2, num1) > 1)
						num1 = Random.Range (2, 7);
					QuestionText.text = "A burger was marked at Rs. " + num.ToString () + ", but sold at " + num1.ToString () + "/" + num2.ToString () + " of its marked price. Find the sale price (in Rs.).";
					num3 = num * num1;
					int hcf = MathFunctions.GetHCF (num3, num2);
					num3 = num3 / hcf;
					num2 = num2 / hcf;
					if (num2 == 1)
						Answer = num3.ToString ();
					else
				    Answer = num3.ToString()+"/"+ num2.ToString();

				} else if (selector == 3) {
					//recheck
					num = Random.Range (1, 8);
					num1 = Random.Range (num + 1, 11);
					while(MathFunctions.GetHCF (num, num1) > 1)
						num = Random.Range (1, 8);
					num2 = Random.Range (1, 15);
					num2 = num2 * 100;
					QuestionText.text = "Narendra spent " + num.ToString () + "/" + num1.ToString () + "th of his money and still had Rs. " + num2.ToString () + " left. What was the initial amount he had?";
					num3 = num2 * num1;
					num4 = num1 - num;
					int hcf = MathFunctions.GetHCF (num3, num4);
					num3 = num3 / hcf;
					num4 = num4 / hcf;
					if (num4 == 1)
						Answer = num3.ToString ();
					else
						Answer =num3.ToString()+"/"+num4.ToString() ;
				} 
				else if (selector == 4) {

					num = Random.Range (2, 6);
					num1 = Random.Range (6, 11);
					while(MathFunctions.GetHCF (num, num1) > 1)
						num = Random.Range (2, 6);
					num2 = Random.Range (2, 8) *2;
					num3 = Random.Range (2, 8) *2;
					while (num * 1f / num1 + 1f / num2 + 1f / num3 - 1 > -0.0001 || num2 == num3) {
						num3 = Random.Range (2, 8) * 2;
						num2 = Random.Range (2, 8) * 2;
					}
					num4 = 50 * Random.Range(20,51);
					QuestionText.text = "Mohit spent " + num.ToString () + "/" + num1.ToString () + "th of his salary on food, 1/" + num2.ToString () + "th on transportation and 1/" + num3.ToString () + "th on rent. If he is still left with Rs. " + num4.ToString () + ", find his monthly salary.";
					num5 = MathFunctions.GetLCM (num1, num2);
					lcm = MathFunctions.GetLCM (num5, num3);
					num1 = lcm / num1;
					num2 = lcm / num2;
					num3 = lcm / num3;
					num5 = (num * num1 + num2 + num3);
					num6 = lcm - num5;
					ans = (num4 * lcm);
					int hcf = MathFunctions.GetHCF (ans, num6);
					ans = ans / hcf;
					num6 = num6 / hcf;
					if (num6 == 1)
						Answer = ans.ToString ();
					else
						Answer = ans.ToString()+"/"+ num6.ToString();

				}

			} else if (level == 3) {
			
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {
				
					num = Random.Range (2, 7);
					num1 = Random.Range (num+1, 11);
					while(MathFunctions.GetHCF (num, num1) > 1)
						num = Random.Range (2, 7);
					num2 = Random.Range (7, 11);
					num3 = Random.Range (2, 7);
					while(MathFunctions.GetHCF (num2, num3) > 1)
						num3 = Random.Range (2, 7);
					num4 = Random.Range (6,12);
					num5 = Random.Range (2, 6);
					while(MathFunctions.GetHCF (num4, num5) > 1)
						num5 = Random.Range (2, 6);
					num6 = Random.Range (15,21);

					while(MathFunctions.GetHCF (num4, num5) > 1)
						num5 = Random.Range (2, 6);
					QuestionText.text="Eden practices football for "+num.ToString()+"/"+num1.ToString()+" hours on friday, "+num2.ToString()+"/"+num3.ToString()+" hours on saturday and " + num4.ToString()+"/"+ num5.ToString() +" hours on sunday. He practices "+num6.ToString()+" hours in total during the week." +
						" Find the ratio of the practice done on the given days to the practice done on the rest of the days (answer as a fraction).";  
				
					lcm = MathFunctions.GetLCM (num1, num3);
					lcm = MathFunctions.GetLCM (lcm, num5);
					num1 = lcm / num1;
					num3 = lcm / num3;
					num5 = lcm / num5;
					ans = (num6 * lcm) - (num * num1 + num2 * num3 + num4 * num5);
					int ans1 = (lcm * num6);
					int hcf = MathFunctions.GetHCF (ans1, ans);
					ans1 = ans1 / hcf;
					ans = ans / hcf;
					if (ans1 == 1)
						Answer = ans.ToString ();
					else
					Answer=ans.ToString()+"/"+ ans1.ToString();

				}
				else if(selector==2){
				
					num = Random.Range(1,6);
					num1 = Random.Range (3, 8) + num;
					while(MathFunctions.GetHCF (num, num1) > 1)
						num = Random.Range(1,6);
					num2 = Random.Range(2,11);
					num2 = num2 * 5000;
					QuestionText.text="Bill wants to distribute his savings amongst Tanya, Mike and Rishabh. "+num.ToString()+"/"+num1.ToString()+"th of his savings is in the name of Tanya. Rest is equally divided among Mike and Rishabh. If Rishabh receives Rs."+num2.ToString()+", calculate Bill's total savings.";

					num3 = num1 - num;
					num4 = 2 * num1;
					num5 = (num4 * num2);
					int hcf = MathFunctions.GetHCF (num5, num3);
					num5 = num5 / hcf;
					num3 = num3 / hcf;
					if (num3 == 1)
						Answer = num5.ToString ();
					else
					Answer=num5.ToString()+"/"+num3.ToString();

				}else if(selector==3){
				
					num3 = Random.Range(1,6);
					num4 = Random.Range (num3 + 1,11);
					num = Random.Range (1, 6);
					num1 = Random.Range(num + 1,11);
					while(MathFunctions.GetHCF (num, num1) > 1)
						num = Random.Range (1, 6);
					while(MathFunctions.GetHCF (num4, num3) > 1 || Mathf.Abs(num * 1f / num1 - num3 * 1f / num4) < 0.0001)
						num3 = Random.Range (1, 6);
					num2 = Random.Range(1,11);
					num2 = num2 * 10;
					QuestionText.text="If "+num.ToString()+"/"+num1.ToString()+" of a number is "+num2.ToString()+" more than "+num3.ToString()+"/"+num4.ToString()+" of the same number, find the number.";
					lcm = MathFunctions.GetLCM (num1, num4);
					num1 = lcm / num1;
					num4 = lcm / num4;
					num5 = (num * num1) - (num3 * num4);
					num6 = num2 * lcm;
					int hcf = MathFunctions.GetHCF (num6, num5);
					num6 = num6 / hcf;
					num5 = num5 / hcf;
					if (num5 == 1)
						Answer = num6.ToString ();
					else
						Answer=num6.ToString()+"/"+num5.ToString();
				}
				else if(selector==4){
				
					num3 = Random.Range(1,6);
					num4 = Random.Range (num3 + 1,11);
					num = Random.Range (3, 8);
					num1 = Random.Range(num + 1,11);
					while(MathFunctions.GetHCF (num, num1) > 1)
						num = Random.Range (3, 8);
					while(MathFunctions.GetHCF (num4, num3) > 1 || num * 1f / num1 - num3 * 1f / num4 < -0.0001)
						num3 = Random.Range (1, 6);
					num2 = Random.Range(1,6);
					num2 = num2*10;
					QuestionText.text="A container of milk is "+num.ToString()+"/"+num1.ToString()+"th full. When "+num2.ToString()+" litres of milk was sold, the can was still "+num3.ToString()+"/"+num4.ToString()+"th full. Find the capacity of the can (in litres).";
					lcm = MathFunctions.GetLCM (num1, num4);
					num1 = lcm / num1;
					num4 = lcm / num4;
					num5 = (num * num1) - (num3 * num4);
					num6 = num2 * lcm;
					int hcf = MathFunctions.GetHCF (num6, num5);
					num6 = num6 / hcf;
					num5 = num5 / hcf;
					if (num5 == 1)
						Answer = num6.ToString ();
					else
					Answer=num6.ToString()+"/"+num5.ToString();

				}
				else if(selector==5){
				
					num = Random.Range(1,10);
					num1 = num+1;
					num3 = Random.Range(2,10);
					num4 = Random.Range(10,50);
					num4 = num4*100;
					QuestionText.text="Nikki and Neha together have "+num.ToString()+"/"+num1.ToString()+" of what Kashish has. Nikki's money is 1/" +num3.ToString()+" of Neha's. If Kashish has Rs. "+num4.ToString()+", how much money (in Rs.) belongs to Neha?";
					num2 = 1 + num3;
					num5 = (num * num4 * num3);
					num6 = (num1 * num2);
					int hcf = MathFunctions.GetHCF (num5, num6);
					num5 = num5 / hcf;
					num6 = num6 / hcf;
					if (num6 == 1)
						Answer = num5.ToString ();
					else
					Answer = num5.ToString () + "/" + num6.ToString ();

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
