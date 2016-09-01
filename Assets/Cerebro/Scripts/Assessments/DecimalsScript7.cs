using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro
{
	public class DecimalsScript7 : BaseAssessment
	{

		public TEXDraw subQuestionLaText;
		public Text subQuestionText;
		//  private int true_mcq_int = 0;
		public GameObject MCQ;
		private string Answer;
		private TEXDraw MCQAns;

		private int[] numbers = new int[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		void Start ()
		{
			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "DEC07", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}

		bool checkFractions (string[] userAnswers, string[] correctAnswers)
		{
			float num1 = float.Parse (userAnswers [0]);
			float num2 = float.Parse (correctAnswers [0]);
			float den2 = float.Parse (correctAnswers [1]);
			float den1 = 1;
			if (userAnswers.Length == 2) {
				den1 = float.Parse (userAnswers [1]);
			}


			if ((num1 / num2) == (den1 / den2)) {
				return true;
			}
			return false;
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
				if (userAnswerText.text == Answer) {
					correct = true;
				} else {
					correct = false;
					AnimateMCQOptionCorrect (Answer);
				}
			} else {				
				float answer;
				if (float.TryParse (userAnswerText.text, out answer) && float.TryParse (Answer, out answer)) {
					if (float.Parse (userAnswerText.text) == float.Parse (Answer)) {						
						correct = true;
					} else {
						correct = false;
					}
				} else if (!userAnswerText.text.Contains ("/") && (userAnswerText.text.LastIndexOf (".") != userAnswerText.text.IndexOf ("."))) {
					correct = false;
				} else if (userAnswerText.text.EndsWith ("/") || userAnswerText.text.EndsWith ("-") || userAnswerText.text.StartsWith ("/") || userAnswerText.text == "." || userAnswerText.text.EndsWith (".") || (userAnswerText.text.Contains ("-") && !userAnswerText.text.Contains ("/"))) {
					correct = false;
				} else {
					if (Answer.Contains ("/")) {

						var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
						var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
						correct = checkFractions (userAnswers, correctAnswers);

					} else if (userAnswerText.text.Contains ("/")) {

						var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
						if ((userAnswers [0].Contains ("-") && !userAnswers [0].StartsWith ("-")) || (userAnswers [1].Contains ("-") && !userAnswers [1].StartsWith ("-")) || (userAnswers [1].LastIndexOf (".") != userAnswers [1].IndexOf (".")) || (userAnswers [0].LastIndexOf (".") != userAnswers [0].IndexOf ("."))) {
							correct = false;
						} else {
							float ua1 = float.Parse (userAnswers [0]);
							float ua2 = float.Parse (userAnswers [1]);
							if (ua2 == 0 || (((float)ua1 / (float)ua2) != float.Parse (Answer)))
								correct = false;
						}
					} else {

						if (float.Parse (userAnswerText.text) == float.Parse (Answer)) {
							correct = true;
						} else {
							correct = false;
						}

					}

				}
			}

			if (correct == true) {
				if (Queslevel == 1) {
					increment = 8;
				} else if (Queslevel == 2) {
					increment = 15;
				} else if (Queslevel == 3) {
					increment = 15;         
				} 

				UpdateStreak (10, 16);

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
			userAnswerText.text = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text;
			MCQAns = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<TEXDraw> ();
			//  true_mcq_int = value;
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
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
		}
		protected override IEnumerator ShowCorrectAnimation ()
		{
			if (userAnswerText != null) {
				userAnswerText.color = MaterialColor.green800;
				if (!numPad.activeSelf) {
					MCQAns.color = MaterialColor.green800;
				}

				var config = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations (2, GoLoopType.PingPong);
				var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1));
				var tween = new GoTween (userAnswerText.gameObject.transform, 0.2f, config);
				flow.insert (0f, tween);
				flow.play ();
			} 
           
			yield return new WaitForSeconds (1f);

			if (userAnswerText != null) {
               
				userAnswerText.color = MaterialColor.textDark;
				if (!numPad.activeSelf) {
					MCQAns.color = MaterialColor.textDark;
				}
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
				if (!numPad.activeSelf) {
					MCQAns.color = MaterialColor.red800;
				}
				Go.to (userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			}

			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				if (numPad.activeSelf) {                // is not MCQ type question
					userAnswerText.text = "";
				}
				if (userAnswerText != null) {
					userAnswerText.color = MaterialColor.textDark;
				}
				if (userAnswerLaText != null) {
					userAnswerLaText.color = MaterialColor.textDark;
				}
				if (!numPad.activeSelf) {
					MCQAns.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
			} else {
				if (numPad.activeSelf) {               // is not MCQ type question
					CerebroHelper.DebugLog ("going in if");
					userAnswerText.text = Answer.ToString ();
					userAnswerText.color = MaterialColor.green800;
				} else {
					if (!numPad.activeSelf) {
						MCQAns.color = MaterialColor.textDark;
					}
					userAnswerText.color = MaterialColor.textDark;

				}
			}

			ShowContinueButton ();
		}

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();

			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}
			// Generating the parameters
            
			level = Queslevel;

			answerButton = GeneralButton;

			subQuestionText.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (false);
			MCQ.gameObject.SetActive (false);
			numPad.SetActive (true);
			subQuestionLaText.text = "";
			subQuestionText.text = "";
           
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
			#region L1
			if (level == 1) {
				selector = GetRandomSelector (1, 6);
				QuestionText.text = "";
				subQuestionText.text = "";
				if (selector == 1) {
					int num = UnityEngine.Random.Range (1, 50);
					while (num % 10 == 0)
						num = UnityEngine.Random.Range (1, 50);
					float num1 = num/10f;
					float den1 = UnityEngine.Random.Range(2,7) * num1;
					float ans = num1 / den1;
					ans = (float)System.Math.Round ((ans * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert the below fraction to decimal (round to 2 decimal places) :";
					subQuestionText.text = num1.ToString () + " / " + den1.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					string[] main = new string[] { "kg", "l", "years", "months", "weeks", "hours", "rupees" };
					string[] sub = new string[] { "g", "ml", "months", "days", "days", "minutes", "paise" };
					int[] conversion = new int[] { 1000, 1000, 12, 30, 7, 60, 100 };

					int rndselector = UnityEngine.Random.Range (0, main.Length);

					int number = UnityEngine.Random.Range (conversion[rndselector], conversion [rndselector] * 10);
					while (number % conversion [rndselector] == 0) {
						number = UnityEngine.Random.Range (1, conversion [rndselector]) * 10;
					}
					int preDec = number / conversion [rndselector];
					int postDec = number % conversion [rndselector];

					float dec = (float)System.Math.Round ((((float)postDec / (float)conversion [rndselector]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					float ans = (float)preDec + (float)dec;

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express " + preDec.ToString () + " " + main [rndselector] + " " + postDec + " " + sub [rndselector] + " as " + main [rndselector] + " (round to 2 decimal places) :";
					subQuestionText.text = "";
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					float num1 = (float)UnityEngine.Random.Range (1, 10) / (float)10;
					float num2 = (float)UnityEngine.Random.Range (1, 10) / (float)10;
					float ans = num1 * num2;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Multiply the following decimals (round to 2 decimal places) :";
					subQuestionText.text = num1.ToString () + " X " + num2.ToString ();
					Answer = ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 4) {                       /// not showing right ans afte wrong attepmt
					numPad.SetActive (false);
					int a = Random.Range (7, 30);
					a *= a;
					int b = Random.Range (6, 25);
					b *= b;
					b -= Random.Range (1, 9);

					int c = Random.Range (1, 9);
					string tempc = a + "." + b + c + b + c + "...";
					int d = Random.Range (1, 10);
					int e = Random.Range (1, 10);
					int f = Random.Range (1, 10);
					int g = Random.Range (1, 10);
					string tempd = d + "." + e + f + g + e + f + g + "...";

					MCQ.gameObject.SetActive (true);
					QuestionText.text = "Which of the following is an example of non-recurring non-terminating decimal ?";
					int rand1 = Random.Range (1, 5);
					int rand2 = Random.Range (1, 5);
					int rand3 = Random.Range (1, 5);

					while (rand1 == rand2)
						rand2 = Random.Range (1, 5);
					while (rand1 == rand3 || rand2 == rand3)
						rand3 = Random.Range (1, 5);
					int rand4 = 10 - (rand1 + rand2 + rand3);

					MCQ.transform.Find ("Option" + rand1.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text = "\\root{" + a + "}";
					MCQ.transform.Find ("Option" + rand2.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text = tempc;
					MCQ.transform.Find ("Option" + rand3.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text = tempd;
					MCQ.transform.Find ("Option" + rand4.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text = "\\root{" + b + "}";

					Answer = "\\root{" + b + "}";
					//GeneralButton.gameObject.SetActive (true);
				} else if (selector == 5) {        
					long num1 = Random.Range (2, 10); 
					int pow = Random.Range (2, 7);
					num1 = (int)Mathf.Pow (num1, pow);
					Debug.Log("First "+num1);
					while (num1 % 10 == 0) {
						num1 = num1 / 10;
					}
					Debug.Log("second "+num1);
					int tot = 0;
					string temp = num1.ToString ();
					tot += temp.Length;
					Debug.Log("length "+tot);
                   
					int num4 = Random.Range (1, 5);
					Debug.Log("num4 "+num4);
					for (int i = 0; i < num4; i++) {
						num1 *= 10;
					}
					QuestionText.text = "How many significant figures does the number " + num1 + " have?";
                    
					Answer = tot.ToString ();
					GeneralButton.gameObject.SetActive (true);
                     
				}
          
			}
            #endregion
            #region L2
            else if (level == 2) {
				selector = GetRandomSelector (1, 6);
				QuestionText.text = "";
				subQuestionText.text = "";
				if (selector == 1) {
					int rand = Random.Range (1, 3);
					if (rand == 1) {
						float num3 = Random.Range (10, 100);
						QuestionText.text = (num3 / 10).ToString ("F2");
					} else {
						float num3 = Random.Range (1, 10);
						QuestionText.text = (num3 / 10).ToString ("F2");
					}
					int num4 = Random.Range (1, 6);
					for (int i = 0; i < num4; i++) {
						QuestionText.text += '0';
					}
					QuestionText.text = "How many significant figures does the number " + QuestionText.text + " have?";
					if (rand == 1)
						Answer = (num4 + 3).ToString ();
					else
						Answer = (num4 + 2).ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int n1 = Random.Range (11, 99);
					int n2 = Random.Range (11, 99);
					int n3 = Random.Range (11, 99);
					float n11 = (float)n1 / 10;
					float n22 = (float)n2 / 10;
					float n33 = (float)n3 / 10;

					float n44 = Random.Range (11, 99);
					n44 /= 10;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Simplify (correct to 2 decimal places) :";
					subQuestionLaText.text = n11 + " \\times " + n22 + " + \\frac{" + n33 + "}{" + n44 + "}";
					float ans = (n11 * n22) + (n33 / n44);
					Answer = ans.ToString ("F2");
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					int num1 = Random.Range (11, 100);
					int num2 = Random.Range (11, 100);
					int num3 = Random.Range (11, 100);
					int num4 = Random.Range (1, 20);
					int num5 = Random.Range (1, 20);
					int num6 = Random.Range (101, 1000);
                    
					while (num4 % num5 != 0)
						num4 = Random.Range (1, 20);
					float n1 = ((float)num1) / 10;
					float n2 = ((float)num2) / 100;
					float n3 = ((float)num3) / 10;
					float n4 = ((float)num4) / 10;
					float n5 = ((float)num5) / 100;
					float n6 = ((float)num6) / 1000;
                    

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Simplify (correct to 2 decimal places) :";
					subQuestionLaText.text = n1 + " + " + n2 + " - ( " + n3 + " + ( " + "\\frac{" + n4 + "}{" + n5 + "}" + " + " + n6 + " ) " + " ) ";
					float ans = n1 + n2 - (n3 + (n4 / n5 + n6));
                    
					Answer = ans.ToString ("F2");
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 4) {
                    
					int c = Random.Range (2, 11);
					int d = Random.Range (1, 8);
					int e = Random.Range (d + 1, 10);
					while (MathFunctions.GetHCF(d, e) > 1)
						d = Random.Range (1, 8);

					string n2 = c + "\\frac{" + d + "}{" + e + "}";
					int n3 = Random.Range (2, 10);
					int g = Random.Range (1, 10);
					int h = Random.Range (2, 10);
					while (MathFunctions.GetHCF(g, h) > 1)
						g = Random.Range (1, 10);
					int i = Random.Range (1, 10);
					int j = Random.Range (2, 10);
	
					string n4 = "\\frac{" + g + "}{" + h + "}";
					int num2 = Random.Range (1, 10);
					float n5 = (float)num2 / 10;
					int l = Random.Range (2, 11);
					int m = Random.Range (1, 8);
					int n = Random.Range (m + 1, 10);
					while (MathFunctions.GetHCF(m, n) > 1)
						m = Random.Range (1, 8);
					string n6 = l + "\\frac{" + m + "}{" + n + "}";
                   
					float val2 = (float)((c * e) + d) / (float)e;
					float val4 = ((float)g / (float)h);
					float val6 = (float)((l * n) + m) / (float)n;

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Simplify (correct to 2 decimal places) :";
					subQuestionLaText.text = n2 + " + " + n3 + " + ( " + n4 + " + " + n5 + "( " + n6 + " ) " + ") ";
					float ans = val2 + n3 + (val4 + n5 * (val6));
					Answer = ans.ToString ("F2");
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 5) {
					int a = Random.Range (3, 10); 
                    
					int b = Random.Range (2, 10);
                 
					float ans = 1.0f / ((float)a * (float)b);
					QuestionText.text = "";
					subQuestionText.gameObject.SetActive (true);
                   
					QuestionText.text = "Convert in decimal (correct to 4 significant figures) :";
					subQuestionLaText.text = "\\frac{1}{" + (a * b) + "}";
					Answer = ans.ToString ("F4");
					GeneralButton.gameObject.SetActive (true);
				} 
                
			}
            #endregion
            #region L3
            else if (level == 3) {
				selector = GetRandomSelector (1, 5);
				QuestionText.text = "";
				subQuestionText.text = "";
				if (selector == 1) {
					int num = Random.Range (11111, 1000000);
					float num1 = ((float)num) / 1000000;
					QuestionText.text = "Round off " + num1.ToString ("F6") + " correct to nearest ten-thousandths :";
					int remainder = num % 100;
					int Ans = 0;
					if (remainder >= 50) {
						Ans = num - remainder;
						Ans += 100;
					} else
						Ans = num - remainder;
					float Ans_f = ((float)Ans) / 1000000;
					Answer = (Ans_f).ToString ("F4");
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int a = Random.Range (10000000, 100000000);
					QuestionText.text = "Round off " + a + " to nearest ten thousand :";
					int remainder = a % 10000;

					int Ans = 0;
					if (remainder >= 5000) {
						a -= remainder;
						Ans = a + 10000;
					} else {
						Ans = a - remainder;
					}
					Answer = Ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					int a = Random.Range (1, 10);

					int n1 = Random.Range (1000000, 10000000);

					while (!n1.ToString ().Contains (a.ToString ()) || (n1.ToString ().IndexOf (a.ToString ())) == n1.ToString ().LastIndexOf (a.ToString ())) {
						n1 = Random.Range (1000000, 10000000);
					}

					float num1 = (float)n1 / 100000;

					string temppp = num1.ToString ();
					string Ans = "";
					CerebroHelper.DebugLog (temppp);
					foreach (char c in temppp) {

						if ((int)c == 46) {
							if (Ans == "")
								Ans += "0";
							Ans += ".";
						} else if ((int)c == 48 + a) {
							Ans += a.ToString();
						} else if(Ans != "")
							Ans += "0";

					}

					QuestionText.text = "Calculate the sum of place values " + a + " in " + num1 + ".";
					Answer = Ans.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 4) {
					int num1 = Random.Range (1000000, 10000000);
					float x = (float)num1 / 10000;

					int remainder2 = num1 % 100;
					float ans = 0.0f;
					if (remainder2 >= 50) {
						ans = 100 - remainder2;
					} else
						ans = remainder2;
					ans /= 10000;
					QuestionText.text = "Calculate the error in rounding off to the nearest tenths " + x + ".";
					Answer = (ans).ToString ();

					Answer = Mathf.Abs (ans).ToString ("F4");
					GeneralButton.gameObject.SetActive (true);
				}
				#endregion L3
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
			} else if (value == 12) {   // .
				if (checkLastTextFor (new string[1]{ "." })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			} else if (value == 13) {   // -
				if (checkLastTextFor (new string[1]{ "-" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			} else if (value == 14) {   // /
				if (checkLastTextFor (new string[1]{ "/" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			}
		}
	}
}