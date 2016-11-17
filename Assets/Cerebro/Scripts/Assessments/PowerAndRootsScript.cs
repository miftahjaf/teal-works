using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class PowerAndRootsScript : BaseAssessment {

		//public Text subQuestionText;

		private string Answer;

		void Start () {
			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "PAR06", "S01", "A01");// TODO

			scorestreaklvls = new int[6];
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

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (Answer.Contains (",")) {
				var correctAnswers = Answer.Split (new string[] { "," }, System.StringSplitOptions.None);
				var userAnswers = userAnswerText.text.Split (new string[] { "," }, System.StringSplitOptions.None);
				if (correctAnswers.Length != userAnswers.Length) {
					correct = false;
				} else {
					for (var i = 0; i < correctAnswers.Length; i++) {
						if (correctAnswers [i] != userAnswers [i]) {
							correct = false;
						}
					}
				}
			} else if (Answer.Contains (".")) {
				float user = float.Parse(userAnswerText.text);
				float correctAns=float.Parse(Answer);
				if (Mathf.Abs (user - correctAns) <= 0.001)
					correct = true;
				else
					correct = false;
			
			}
			else if (Answer.Contains ("/")) {
				
				var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
				var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
				correct = MathFunctions.checkFractions (userAnswers, correctAnswers);
			}else {
				if (userAnswerText.text == Answer) {
					correct = true;
				} else {
					correct = false;
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
					increment = 10;
				} else if (Queslevel == 5) {
					increment = 15;
				}else if (Queslevel == 6) {
					increment = 15;
				}
                else if (Queslevel == 7)
                {
                    increment = 15;
                }

                UpdateStreak(8,10);

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

		string getFraction(int num1,int num2){
			string res;
			if (num1 == num2)
				res = "1";
			else if (num1 == 0)
				res = "0";
			else{
				if (num1 < 0 && num2 < 0)
					res = (-num1).ToString () + "/" + (-num2).ToString ();
				else if (num1 < 0) {
					res = "-" + (-num1).ToString () + "/" + num2.ToString ();
				}
				else if (num2 < 0) {
					res = "-" + (num1).ToString () + "/" + (-num2).ToString ();
				}
				else
					res = num1.ToString () + "/" + num2.ToString ();
			}
			return res;

		}

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			level = Queslevel;

			answerButton = GeneralButton;
			GeneralButton.gameObject.SetActive (false);


			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			int num1;
			int num2; 
			int power1;
			int power2;
			//level 1:powers
			if (level == 1) {
				selector = GetRandomSelector (1, 12);
				QuestionText.text = "Compute the power : ";

				if (selector == 1) {												//compute the powers
					num1 = Random.Range (2, 6);
					power1 = Random.Range (2, 4);
					QuestionLatext.text = num1.ToString () + "^" + power1.ToString (); 
					GeneralButton.gameObject.SetActive (true);
					Answer = Mathf.Ceil (Mathf.Pow (num1, power1)).ToString ();
				} else if (selector == 2) {	//power is 0
					num1 = Random.Range (1, 401);
					power1 = 0;
					QuestionLatext.text = num1.ToString () + "^" + power1.ToString ();
					GeneralButton.gameObject.SetActive (true);
					Answer = "1";
				} else if (selector == 3) {    										// power is 1
					num1 = Random.Range (1, 401);
					power1 = 1;
					QuestionLatext.text = num1.ToString () + "^" + power1.ToString (); 
					GeneralButton.gameObject.SetActive (true);
					Answer = num1.ToString ();
				} else if (selector == 4) {    										//product of two powers
					num1 = Random.Range (2, 101);
					power1 = Random.Range (2, 50);
					power2 = Random.Range (2, 50);
					QuestionLatext.text = num1.ToString () + "^{" + power1.ToString () + "}\\cdot"
					+ num1.ToString () + "^{" + power2.ToString () + "} = " + num1.ToString () + "^? "; 
					GeneralButton.gameObject.SetActive (true);
					Answer = (power1 + power2).ToString ();
				} else if (selector == 5) {    //which is greater?
					QuestionText.text = "Put the correct sign : ";
					num1 = Random.Range (2, 6);
					power1 = Random.Range (1, 4);
					power2 = Random.Range (1, 4);
					QuestionLatext.text = num1.ToString () + "^{" + power1.ToString () + "}\\cdot"
						+ num1.ToString () + "^{" + power2.ToString () + "} \\square "
					+ num1.ToString () + "^{" + power1.ToString () + "}+"
					+ num1.ToString () + "^{" + power2.ToString () + "} ";
					GeneralButton.gameObject.SetActive (true);
					if (Mathf.Pow (num1, power1 + power2) > (Mathf.Pow (num1, power1) + Mathf.Pow (num1, power2)))
						Answer = ">";
					else
						Answer = "<=";
				} else if (selector == 6) {    						//division of two powers
					num1 = Random.Range (2, 101);
					power1 = Random.Range (2, 51);
					power2 = Random.Range (2, 51);
					int temp;
					if (power2 > power1) {
						temp = power1;
						power1 = power2;
						power2 = temp;
					}
					QuestionLatext.text = num1.ToString () + "^{" + power1.ToString () + "}\\div"
					+ num1.ToString () + "^{" + power2.ToString () + "} = " + num1.ToString () + "^? ";
					GeneralButton.gameObject.SetActive (true);
					Answer = (power1 - power2).ToString ();
				} else if (selector == 7) {	//power in negative numbers
					num1 = -1;
					power1 = Random.Range (1, 100);
					QuestionLatext.text = "(" + num1.ToString () + ")^{" + power1.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);
					if (power1 % 2 == 0)
						Answer = "1";
					else
						Answer = "-1";
				} else if (selector == 8) {	      //distribution of two powers
					var choose = Random.Range (1, 3);
					num1 = Random.Range (2, 21);
					num2 = Random.Range (2, 21);
					power1 = Random.Range (1, 101);
					GeneralButton.gameObject.SetActive (true);
					if (choose == 1) {
						QuestionLatext.text = num1.ToString () + "^{" + power1.ToString () + "}\\cdot"
						+ num2.ToString () + "^{" + power1.ToString () + "} = (" + num1.ToString () + "\\cdot" +
						num2.ToString () + ")^? "; 
						Answer = power1.ToString ();
					} else {
						QuestionLatext.text = "(" + num1.ToString () + "\\cdot" + num2.ToString () + ")^{" +
						power1.ToString () + "} = " + num1.ToString () + "^? \\cdot" + num2.ToString () + "^? ";
						Answer = power1.ToString () + "," + power1.ToString ();
					}  
				} else if (selector == 9) {	     // powers of negative numbers
					num1 = Random.Range (1, 13);
					power1 = Random.Range (1, 4);
					QuestionLatext.text = "(-" + num1.ToString () + ")^" + power1.ToString ();
					GeneralButton.gameObject.SetActive (true);
					if (power1 % 2 == 0)
						Answer = Mathf.Ceil (Mathf.Pow (num1, power1)).ToString ();
					else
						Answer = "-" + Mathf.Ceil (Mathf.Pow (num1, power1)).ToString ();
				} else if (selector == 10) {	//negative powers
					QuestionText.text += "(in fraction) ";
					num1 = Random.Range (2, 6);
					power1 = Random.Range (-6, -1);
					QuestionLatext.text = num1.ToString () + "^{" + power1.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);
					Answer = "1/" + Mathf.Ceil (Mathf.Pow (num1, -power1)).ToString ();
				} else {                         //power of power
					num1 = Random.Range (2, 6);
					power1 = Random.Range (2, 4);
					power2 = Random.Range (2, 4);
					QuestionLatext.text = "(" + num1.ToString () + "^" + power1.ToString () + ")^" + power2.ToString ();
					GeneralButton.gameObject.SetActive (true);
					Answer = Mathf.Ceil (Mathf.Pow (num1, (power1 * power2))).ToString ();
				}
			}

			//level 2: powers
			else if (level == 2) {
				selector = GetRandomSelector (1, 7);
				QuestionText.text = "Compute the power :";

				if (selector == 1) {
					num1 = Random.Range (1, 51);
					num2 = Random.Range (1, 51);
					power1 = Random.Range (1, 51);

					QuestionLatext.text =num1.ToString()+"^0"+" \\cdot "+num2.ToString()+ "^0"+" \\cdot "+
						power1.ToString()+"^0 = ";
					GeneralButton.gameObject.SetActive (true);
					Answer = "1";
				} else if (selector == 2) {
					num1 = Random.Range (1, 100);
					num2 = Random.Range (1, 100);
					power1 = Random.Range (2, 50);
					power2 = Random.Range (2, 50);
					QuestionLatext.text = num1.ToString () +"\\cdot"+ power1.ToString () + "^0 + " +
						num2.ToString () +"\\cdot"+ power2.ToString () + "^0 = ";
					GeneralButton.gameObject.SetActive (true);
					Answer = (num1+ num2).ToString ();
				} else if (selector == 3) {
					num1 = Random.Range (1, 50);
					num2 = Random.Range (1, 50);
					var num3 = Random.Range (1, 50);
					var num4 = Random.Range (1, 50);
					power1 = Random.Range (0, 2);
					power2 = Random.Range (0, 2);
					QuestionLatext.text = "(" + num1.ToString () + "+" + num3.ToString () + ")^" + power1.ToString ()
					+ " + (" + num2.ToString () + "+" + num4.ToString () + ")^" + power2.ToString () + " = ";
					GeneralButton.gameObject.SetActive (true);
					int ans = 0;
					ans = (power1 == 1 ? (num1 + num3) : 1) + (power2 == 1 ? (num2 + num4) : 1);
					Answer = ans.ToString ();
				} else if (selector == 4) {
					num1 = Random.Range (2, 50);
					num2 = Random.Range (2, 50);
					var num3 = Random.Range (2, 50);
					power1 = Random.Range (2, 50);
					power2 = Random.Range (2, 50);
					var power3 = Random.Range (2, 50);
					QuestionLatext.text = "\\frac{\\aalgebra^{" + num1.ToString () + "}\\cdot \\balgebra^{" +
						num2.ToString () + "}\\cdot \\calgebra^{" + num3.ToString () + "}}{\\aalgebra^{" +
						power1.ToString () + "}\\cdot \\balgebra^{" +
						power2.ToString () + "}\\cdot \\calgebra^{" + power3.ToString () + "}} = " +
						"\\aalgebra^?\\cdot \\balgebra^?\\cdot \\calgebra^?"; 
					GeneralButton.gameObject.SetActive (true);
					Answer = (num1 - power1).ToString () + "," + (num2 - power2).ToString () + "," +
					(num3 - power3).ToString ();
				} else if (selector == 5) {
					num1 = Random.Range (2, 50);
					num2 = Random.Range (2, 50);
					power1 = Random.Range (2, 50);
					QuestionLatext.text = "\\aalgebra^{-" + num1.ToString () + "}\\cdot \\balgebra^{" +
						num2.ToString () + "}\\cdot \\calgebra^{" + power1.ToString () + "}\\cdot \\aalgebra^{" +
						(num1 + 1).ToString () + "}\\cdot \\balgebra^{" +
						(num2 - 1).ToString () + "}\\cdot \\calgebra^{-" + power1.ToString () + "} = " +
						"\\aalgebra^?\\cdot \\balgebra^?\\cdot \\calgebra^?"; 
					GeneralButton.gameObject.SetActive (true);
					Answer = "1," + (2 * num2 - 1).ToString () + ",0";
				} else {
					QuestionText.text = "Find the value of : ";
					var coeff = Random.Range (2, 11);
					power1 = Random.Range (2, 6);
					power2 = Random.Range (2, 6);
					num1 = Random.Range (1, 6);
					num2 = Random.Range (1, 6);
					QuestionLatext.text = coeff.ToString () + "\\xalgebra^" + power1.ToString () + "\\yalgebra^" + power2.ToString ()
						+ " for \\xalgebra= " + num1.ToString () + ", \\yalgebra= " + num2.ToString () + ".";
					GeneralButton.gameObject.SetActive (true);
					Answer = Mathf.Ceil (coeff * Mathf.Pow (num1, power1) * Mathf.Pow (num2, power2)).ToString ();
				} 
			}

			//level 3: square of a number
			else if (level == 3) {
				selector = GetRandomSelector (1, 5);
				QuestionText.text = "Find the square of :";

				if (selector == 1) {          // Natural number
					num1 = Random.Range (2, 10);
					QuestionLatext.text = num1.ToString ();
					GeneralButton.gameObject.SetActive (true);
					Answer = (num1 * num1).ToString ();
				} else if (selector == 2) {          // Negative number
					num1 = Random.Range (-2, -10);
					QuestionLatext.text = num1.ToString ();
					GeneralButton.gameObject.SetActive (true);
					Answer = (num1 * num1).ToString ();
				} else if (selector == 3) {          // decimal     
					num1 = Random.Range (1, 10);
					float number = (float)num1;
					float square = (number * number) / (float)100;
					QuestionLatext.text = (number/10).ToString ();
					GeneralButton.gameObject.SetActive (true);
					Answer = square.ToString ();
				} else if (selector == 4) {          // fraction
					QuestionText.text += " (in fraction)";
					do {
						num1 = Random.Range (2, 14);
						num2 = Random.Range (2, 14);
					} while (MathFunctions.GetHCF (num1, num2) > 1);
					QuestionLatext.text = "\\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);
					Answer = getFraction (num1 * num1, num2 * num2);
				} 
			}
				

			//level 4: square root of a number
			else if (level == 4) {
				selector = GetRandomSelector (1, 6);
				QuestionText.text = "Compute :";

				if (selector == 1) {          // Natural number
					num1 = Random.Range (6, 16);
					QuestionLatext.text = "\\root{" + (num1 * num1).ToString () + "}";
					GeneralButton.gameObject.SetActive (true);
					Answer = num1.ToString ();
				} else if (selector == 2) {          // fraction
					QuestionText.text+=" (in fraction)";
					do { 
						num1 = Random.Range (2, 14);
						num2 = Random.Range (2, 14);
					} while (MathFunctions.GetHCF (num1, num2) > 1);
					QuestionLatext.text = "\\root{\\frac{" + (num1 * num1).ToString () + "}{"
					+ (num2 * num2).ToString () + "}}";
					GeneralButton.gameObject.SetActive (true);
					Answer = getFraction (num1,num2);
				} else if (selector == 3) {          // operation on square root
					int choose = Random.Range (1, 5);
					GeneralButton.gameObject.SetActive (true);

					if (choose == 1) { //Addition 
						do {
							num1 = Random.Range (2, 14);
							num2 = Random.Range (2, 14);
						} while (MathFunctions.GetHCF (num1, num2) > 1);
						QuestionLatext.text = "\\root {" + (num1 * num1).ToString () + "} + \\root {" +
						(num2 * num2).ToString () + "}";
						Answer = (num1 + num2).ToString ();
					} else if (choose == 2) { //Subtraction 
						do {
							num1 = Random.Range (2, 14);
							num2 = Random.Range (2, 14);
						} while (MathFunctions.GetHCF (num1, num2) > 1);
						QuestionLatext.text = "\\root {" + (num1 * num1).ToString () + "} - \\root {" +
						(num2 * num2).ToString () + "}";
						Answer = (num1 - num2).ToString ();
					} else if (choose == 3) { //Multiplication 
						do {
							num1 = Random.Range (2, 14);
							num2 = Random.Range (2, 14);
						} while (MathFunctions.GetHCF (num1, num2) > 1);
						QuestionLatext.text = "\\root {" + (num1 * num1).ToString () + "} \\times \\root {" +
						(num2 * num2).ToString () + "}";
						Answer = (num1 * num2).ToString ();
					} else {                 //division 
						QuestionText.text+=" (in fraction)";
						do {
							num1 = Random.Range (2, 14);
							num2 = Random.Range (2, 14);
						} while (MathFunctions.GetHCF (num1, num2) > 1);
						QuestionLatext.text = "\\root {" + (num1 * num1).ToString () + "} \\div \\root {" +
						(num2 * num2).ToString () + "}";
						//Answer = num1.ToString () + "/" + num2.ToString ();
						Answer=getFraction(num1,num2);
					}

				} else if (selector == 4) {          // fraction
					int choose = Random.Range (1, 5);
					do {
						num1 = Random.Range (2, 14);
						num2 = Random.Range (2, 14);
					} while (MathFunctions.GetHCF (num1, num2) > 1);
					do {
						power1 = Random.Range (2, 11);
						power2 = Random.Range (2, 11);
					} while (MathFunctions.GetHCF (power1, power2) > 1);
					GeneralButton.gameObject.SetActive (true);
					QuestionText.text+=" (in fraction)";
					if (choose == 1) {
						QuestionLatext.text = "\\frac{ \\root{" + (num1 * num1).ToString () + "} + " +
						"\\root{" + (num2 * num2).ToString () + "}}{ \\root{" + (power1 * power1).ToString () + "} + " +
						"\\root{" + (power2 * power2).ToString () + "}}";
						//Answer = (num1 + num2).ToString () + "/" + (power1 + power2).ToString ();
						Answer = getFraction (num1 + num2, power1 + power2);
					} else if (choose == 2) {
						if (power1 == power2)
							power1++;
						QuestionLatext.text = "\\frac{ \\root{" + (num1 * num1).ToString () + "} - " +
						"\\root{" + (num2 * num2).ToString () + "}}{ \\root{" + (power1 * power1).ToString () + "} - " +
						"\\root{" + (power2 * power2).ToString () + "}}";
						//Answer = (num1 - num2).ToString () + "/" + (power1 - power2).ToString ();
						Answer = getFraction (num1 - num2, power1 - power2);
					} else if (choose == 3) {
						QuestionLatext.text = "\\frac{ \\root{" + (num1 * num1).ToString () + "}}{\\root{" +
						(num2 * num2).ToString () + "}} +" +
						"\\frac{ \\root{" + (power1 * power1).ToString () + "}}{\\root{" +
						(power2 * power2).ToString () + "}}";
						int lcm = MathFunctions.GetLCM (num2, power2);
						num1 = (lcm / num2) * num1;
						power1 = (lcm / power2) * power1;
						//Answer = (num1 + power1).ToString () + "/" + lcm.ToString ();
						Answer=getFraction(num1+power1,lcm);
					} else {
						QuestionLatext.text = "\\frac{ \\root{" + (num1 * num1).ToString () + "}}{\\root{" +
							(num2 * num2).ToString () + "}} -" +
							"\\frac{ \\root{" + (power1 * power1).ToString () + "}}{\\root{" +
							(power2 * power2).ToString () + "}}";
						int lcm = MathFunctions.GetLCM (num2, power2);
						num1 = (lcm / num2) * num1;
						power1 = (lcm / power2) * power1;
						//Answer = (num1 - power1).ToString () + "/" + lcm.ToString ();
						Answer=getFraction(num1-power1,lcm);
					}
				}
				else if (selector == 5) {                           //decimal
					num1 = Random.Range (11, 20);
					float number = (float)num1;
					float square = (number * number) / (float)100;
					QuestionLatext.text = "\\root{" + square.ToString() + "}";
					GeneralButton.gameObject.SetActive (true);
					Answer = (number/10).ToString ();
				}
			}

			//level 5:cube root
			else if (level == 5) {
				selector = GetRandomSelector (1, 5);
				QuestionText.text = "Compute :";

				if (selector == 1) {          // Natural number
					num1 = Random.Range (2, 11);
					QuestionLatext.text = "\\root[3]{" + (num1*num1*num1).ToString () + "}";
					GeneralButton.gameObject.SetActive (true);
					Answer = num1.ToString ();
				} else if (selector == 2) {
					QuestionText.text += " (in fraction)";// fraction
					do {
						num1 = Random.Range (2, 11);
						num2 = Random.Range (2, 11);
					} while (MathFunctions.GetHCF (num1, num2) > 1);
					QuestionLatext.text = "\\root[3]{\\frac{" + (num1 * num1 * num1).ToString () + "}{"
						+ (num2 * num2 * num2).ToString() + "}}";
					GeneralButton.gameObject.SetActive (true);
					//Answer = num1.ToString () + "/" + num2.ToString ();
					Answer=getFraction(num1,num2);
				} else if (selector == 3) {          // operation on cube root
					int choose = Random.Range (1, 5);

					if (choose == 1) { //Addition 
						do {
							num1 = Random.Range (2, 11);
							num2 = Random.Range (2, 11);
						} while (MathFunctions.GetHCF (num1, num2) > 1);
						QuestionLatext.text = "\\root[3]{" + (num1 * num1*num1).ToString () + "} + \\root[3]{" +
							(num2 * num2*num2).ToString () + "}";
						GeneralButton.gameObject.SetActive (true);
						Answer = (num1 + num2).ToString ();
					} else if (choose == 2) { //Subtraction 
						do {
							num1 = Random.Range (2, 11);
							num2 = Random.Range (2, 11);
						} while (MathFunctions.GetHCF (num1, num2) > 1);
						QuestionLatext.text = "\\root[3]{" + (num1 * num1*num1).ToString () + "} - \\root[3]{" +
							(num2 * num2*num2).ToString () + "}";
						GeneralButton.gameObject.SetActive (true);
						Answer = (num1 - num2).ToString ();
					} else if (choose == 3) { //Multiplication 
						do {
							num1 = Random.Range (2, 11);
							num2 = Random.Range (2, 11);
						} while (MathFunctions.GetHCF (num1, num2) > 1);
						QuestionLatext.text = "\\root[3]{" + (num1 * num1*num1).ToString () + "} \\times \\root[3]{" +
							(num2 * num2*num2).ToString () + "}";
						GeneralButton.gameObject.SetActive (true);
						Answer = (num1 * num2).ToString ();
					} else {                 //division 
						do {
							num1 = Random.Range (2, 11);
							num2 = Random.Range (2, 11);
						} while (MathFunctions.GetHCF (num1, num2) > 1);
						QuestionLatext.text = "\\root[3]{" + (num1 * num1*num1).ToString () + "} \\div \\root[3]{" +
							(num2 * num2*num2).ToString () + "}";
						GeneralButton.gameObject.SetActive (true);
						//Answer = num1.ToString () + "/" + num2.ToString ();
						Answer=getFraction(num1,num2);
					}

				} else if (selector == 4) {          // fraction
					int choose = Random.Range (1, 5);
					do {
						num1 = Random.Range (2, 11);
						num2 = Random.Range (2, 11);
					} while (MathFunctions.GetHCF (num1, num2) > 1);
					do {
						power1 = Random.Range (2, 11);
						power2 = Random.Range (2, 11);
					} while (MathFunctions.GetHCF (power1, power2) > 1);
					QuestionText.text += " (in fraction)";
					if (choose == 1) {
						QuestionLatext.text = "\\frac{ \\root[3]{" + (num1 * num1*num1).ToString () + "} + " +
							"\\root[3]{" + (num2 * num2*num2).ToString () + "}}{ \\root[3]{" + (power1 * power1*power1).ToString () + "} + " +
							"\\root[3]{" + (power2 * power2*power2).ToString () + "}}";
						//Answer = (num1 + num2).ToString () + "/" + (power1 + power2).ToString ();
						GeneralButton.gameObject.SetActive (true);
						Answer=getFraction(num1+num2,power1+power2);
					} else if (choose == 2) {
						if (power1 == power2)
							power1++;
						QuestionLatext.text = "\\frac{ \\root[3]{" + (num1 * num1*num1).ToString () + "} - " +
							"\\root[3]{" + (num2 * num2*num2).ToString () + "}}{ \\root[3]{" + (power1*power1*power1).ToString () + "} - " +
							"\\root[3]{" + (power2 * power2*power2).ToString () + "}}";
						//Answer = (num1 - num2).ToString () + "/" + (power1 - power2).ToString ();
						GeneralButton.gameObject.SetActive (true);
						Answer=getFraction(num1-num2,power1-power2);
					} else if (choose == 3) {
						QuestionLatext.text = "\\frac{ \\root[3]{" + (num1 * num1*num1).ToString () + "}}{\\root[3]{" +
							(num2 * num2*num2).ToString () + "}} +" +
							"\\frac{ \\root[3]{" + (power1 * power1*power1).ToString () + "}}{\\root[3]{" +
							(power2 * power2*power2).ToString () + "}}";
						int lcm = MathFunctions.GetLCM (num2, power2);
						num1 = (lcm / num2) * num1;
						power1 = (lcm / power2) * power1;
						//Answer = (num1 + power1).ToString () + "/" + lcm.ToString ();
						GeneralButton.gameObject.SetActive (true);
						Answer=getFraction(num1+power1,lcm);
					} else {
						QuestionLatext.text = "\\frac{ \\root[3]{" + (num1 * num1*num1).ToString () + "}}{\\root[3]{" +
							(num2 * num2*num2).ToString () + "}} -" +
							"\\frac{ \\root[3]{" + (power1 * power1*power1).ToString () + "}}{\\root[3]{" +
							(power2 * power2*power2).ToString () + "}}";
						int lcm = MathFunctions.GetLCM (num2, power2);
						num1 = (lcm / num2) * num1;
						power1 = (lcm / power2) * power1;
						//Answer = (num1 - power1).ToString () + "/" + lcm.ToString ();
						GeneralButton.gameObject.SetActive (true);
						Answer=getFraction(num1-power1,lcm);
					}
				}
			}
			//level 6: cube of a number
			else if (level == 6) {
				selector = GetRandomSelector (1, 5);
				QuestionText.text = "Find the cube of :";

				if (selector == 1) {          // Natural number
					num1 = Random.Range (1, 10);
					QuestionLatext.text = num1.ToString ();
					GeneralButton.gameObject.SetActive (true);
					Answer = (num1 * num1 * num1).ToString ();
				} else if (selector == 2) {          // Negative number
					num1 = Random.Range (-2, -10);
					QuestionLatext.text = num1.ToString ();
					GeneralButton.gameObject.SetActive (true);
					Answer = (num1 * num1 * num1).ToString ();
				} else if (selector == 3) {          // decimal
					num1 = Random.Range (2, 16);
					float number = (float)num1;
					float cube = (num1 * num1 * num1) / (float)1000;
					QuestionLatext.text = (number / 10).ToString ();
					GeneralButton.gameObject.SetActive (true);
					Answer = cube.ToString ();
				} else if (selector == 4) {          //  fraction
					QuestionText.text+= " (in fraction)";
					do {
						num1 = Random.Range (2, 11);
						num2 = Random.Range (2, 11);
					} while (MathFunctions.GetHCF (num1, num2) > 1);
					QuestionLatext.text = "\\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);
					Answer = getFraction (num1 * num1 * num1, num2 * num2 * num2);
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
			} else if (value == 12) {   // '/'
				if(checkLastTextFor(new string[1]{"/"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			} else if (value == 13) {   // .
				if(checkLastTextFor(new string[1]{"."})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			} else if (value == 14) {   //,
				if(checkLastTextFor(new string[1]{","})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ",";
			} else if (value == 15) {   //-
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}  else if (value == 16) {   //>
				if(checkLastTextFor(new string[1]{">"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ">";
			}  else if (value == 17) {   //<=
				if(checkLastTextFor(new string[1]{"<="})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "<=";
			} 
		}
	}
}
