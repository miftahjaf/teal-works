using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class SimplifyQuestionGenerator : BaseAssessment {

//		public Text QuestionText;
		public TEXDraw Simplify_Question;
	//	public Text Simplify_Question;
		public TEXDraw AnswerPolynomial;
		public GameObject AnswerTab;

		public TEXDraw UserAnswer;
		//private int rand;
		private int coeff1;
		private int coeff2;
		private int coeff3;
		private int coeff4;
		private int coeff5;
		private int coeff6;
		private int addsub;
		private string expression1;
		private string expression2;

		private int answerLength;

		private string correctAnswerText = "";

		public int scoeff1;
		public int scoeff2;
		public int scoeff3;

		public GameObject SubmitBtn;

		void Start () {
			StartCoroutine(StartAnimation ());

			base.Initialise ("M", "Alg06", "S01", "A01");

			scorestreaklvls = new int[1];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			scoeff1 = 0;
			scoeff2 = 0;
			scoeff3 = 0;
			coeff1 = 0;
			coeff2 = 0;
			coeff3 = 0;
			coeff4 = 0;
			coeff5 = 0;
			coeff6 = 0;
			expression1 = "";
			expression2 = "";

			AnswerPolynomial.text = "";
			UserAnswer.text = "";

			GenerateQuestion ();
		}

		public override void SubmitClick(){
			if (ignoreTouches || UserAnswer.text == "") {
				return;
			}

			ignoreTouches = true;
			var correct = false;

			correctAnswerText = "";
			if (scoeff1 > 1) {
				correctAnswerText += scoeff1.ToString () + "x^2";
			} else if(scoeff1 == 1) {
				correctAnswerText += "x^2";
			} else if(scoeff1 == -1) {
				correctAnswerText += "-x^2";
			} else if(scoeff1 < -1) {
				correctAnswerText += "-" + Mathf.Abs(scoeff1).ToString () + "x^2";
			}

			if (scoeff2 > 1) {
				correctAnswerText += "+" + scoeff2.ToString () + "x";
			} else if(scoeff2 == 1) {
				correctAnswerText += "+x";
			} else if(scoeff2 == -1) {
				correctAnswerText += "-x";
			} else if(scoeff2 < -1) {
				correctAnswerText += "-" + Mathf.Abs(scoeff2).ToString () + "x";
			}

			if (scoeff3 > 0) {
				correctAnswerText += "+" + scoeff3.ToString (); 
			} else if(scoeff3 < 0) {
				correctAnswerText += "-" + Mathf.Abs(scoeff3).ToString ();
			}

			float ans1 = 0f;
			float ans2 = 0f;
			float ans3 = 0f;
			var answerTxt = UserAnswer.text;
			var str = answerTxt;
			int ignoreMe;
			float ignoreMeF;
			// COEFF1
			var splitStr = str.Split (new string[] { "x^2" }, System.StringSplitOptions.None);
			if (splitStr.Length > 1) {
				string remainingStr = splitStr [0];
				string reverseCoeff = "";
				for (int i = remainingStr.Length - 1; i >= 0; i--) {
					reverseCoeff = reverseCoeff + remainingStr [i];
					if (remainingStr [i] == "+"[0] || remainingStr [i] == "-"[0]) {
						break;
					}
				}
				string coeff = Reverse (reverseCoeff);
				string toRemove = coeff + "x^2";
				if (coeff == "+" || coeff == "") {
					coeff = "1";
				}
				if (coeff == "-") {
					coeff = "-1";
				}

				str = str.Replace (toRemove, "");
				if (float.TryParse (coeff, out ignoreMeF)) {
					ans1 = float.Parse (coeff);
				}
			}


			// COEFF2
			splitStr = str.Split (new string[] { "x" }, System.StringSplitOptions.None);
			if (splitStr.Length > 1) {
				string remainingStr = splitStr [0];
				string reverseCoeff = "";
				for (int i = remainingStr.Length - 1; i >= 0; i--) {
					reverseCoeff = reverseCoeff + remainingStr [i];
					if (remainingStr [i] == "+"[0] || remainingStr [i] == "-"[0]) {
						break;
					}
				}
				string coeff = Reverse (reverseCoeff);
				string toRemove = coeff + "x";
				if (coeff == "+" || coeff == "") {
					coeff = "1";
				}
				if (coeff == "-") {
					coeff = "-1";
				}

				str = str.Replace (toRemove, "");

				if (float.TryParse (coeff, out ignoreMeF)) {
					ans2 = float.Parse (coeff);
				}

			}


			// COEFF3
			if (int.TryParse (str, out ignoreMe)) {
				ans3 = int.Parse (str);
			}

			var increment = 5;
			if (scoeff1 == ans1 && scoeff2 == ans2 && scoeff3 == ans3) {
				correct = true;
			}

			expression1 = "";
			expression2 = "";

			if (correct == false) {
				StartCoroutine (ShowWrongAnimation());
			} else {
				StartCoroutine (ShowCorrectAnimation());
			}

			base.QuestionEnded (correct, level, increment, selector);
		}

		public string Reverse( string s )
		{
			char[] charArray = s.ToCharArray();
			System.Array.Reverse( charArray );
			return new string( charArray );
		}

		public override void ContinueButtonPressed() {
			HideContinueButton ();
			UserAnswer.color = MaterialColor.textDark;
			AnswerPolynomial.text = "";
			AnswerTab.transform.Find("Image").GetComponent<Image>().color = MaterialColor.textLight;
			showNextQuestion ();
		}

		protected override IEnumerator ShowWrongAnimation() {
			AnswerTab.transform.Find("Image").GetComponent<Image>().color = MaterialColor.red100;
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				yield return new WaitForSeconds (0.5f);
				UserAnswer.text = "";
				UserAnswer.color = MaterialColor.textDark;
				AnswerPolynomial.text = "";
				AnswerTab.transform.Find("Image").GetComponent<Image>().color = MaterialColor.textLight;
				ignoreTouches = false;
			} else {
				AnswerPolynomial.text = correctAnswerText;
			}
			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			AnswerTab.transform.Find("Image").GetComponent<Image>().color = MaterialColor.green100;
			yield return new WaitForSeconds (1);
			AnswerTab.transform.Find("Image").GetComponent<Image>().color = MaterialColor.textLight;
			UserAnswer.color = MaterialColor.textDark;
			showNextQuestion ();
		}

		protected override void GenerateQuestion() {
			AnswerPolynomial.text = "";

			base.QuestionStarted ();
			//Generating random questions on Simplifying Algebraic Expressions
			ignoreTouches = false;

			level = Queslevel;

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				selector = GetRandomSelector (1, 2);

				if (selector == 1) {
					coeff1 = Random.Range (-5, 5);
					expression1 = coeff1.ToString () + "x^2";
					if (coeff1 == 0) {
						expression1 = "";
					} else { 
						if (coeff1 == 1) {
							expression1 = "x^2";
						}
						if (coeff1 == -1) {
							expression1 = "-x^2";
						}
					}

					coeff2 = Random.Range (-5, 5);
					if (coeff2 == 0) {
						expression1 = expression1 + "";
					} 
					if (coeff2 == 1) {
						expression1 = expression1 + "+x";
					}
					if (coeff2 == -1) {
						expression1 = expression1 + "-x";
					}
					if (coeff2 > 1) {
						expression1 = expression1 + "+" + coeff2.ToString () + "x";
					}
					if (coeff2 < -1) {
						expression1 = expression1 + "" + coeff2.ToString () + "x";
					}


					coeff3 = Random.Range (-5, 5);
					if (coeff3 == 0) {
						expression1 = expression1 + "";
					} 
					if (coeff3 > 0) {
						expression1 = expression1 + "+" + coeff3.ToString ();
					}
					if (coeff3 < 0) {
						expression1 = expression1 + "" + coeff3.ToString ();
					}


					coeff4 = Random.Range (-5, 5);
					expression2 = coeff4.ToString () + "x^2";
					if (coeff4 == 0) {
						expression2 = "";
					} else { 
						if (coeff4 == 1) {
							expression2 = "x^2";
						}
						if (coeff4 == -1) {
							expression2 = "-x^2";
						}
					}

					coeff5 = Random.Range (-5, 5);
					if (coeff5 == 0) {
						expression2 = expression2 + "";
					} 
					if (coeff5 == 1) {
						expression2 = expression2 + "+x";
					}
					if (coeff5 == -1) {
						expression2 = expression2 + "-x";
					}
					if (coeff5 > 1) {
						expression2 = expression2 + "+" + coeff5.ToString () + "x";
					}
					if (coeff5 < -1) {
						expression2 = expression2 + "" + coeff5.ToString () + "x";
					}

					coeff6 = Random.Range (-5, 5);
					if (coeff6 == 0) {
						expression2 = expression2 + "";
					} 
					if (coeff6 > 0) {
						expression2 = expression2 + "+" + coeff6.ToString ();
					}
					if (coeff6 < 0) {
						expression2 = expression2 + "" + coeff6.ToString ();
					}

					addsub = Random.Range (-2, 2);
					if (addsub < 0) {
						Simplify_Question.text = "(" + expression1 + ")\\-(" + expression2 + ")";
						Simplify_Question.text = "(" + expression1 + ")\\-(" + expression2 + ")";
						Simplify_Question.text = "(" + expression1 + ")\\-(" + expression2 + ")";
						scoeff1 = coeff1 - coeff4;
						scoeff2 = coeff2 - coeff5;
						scoeff3 = coeff3 - coeff6;
					}
					if (addsub >= 0) {
						Simplify_Question.text = "(" + expression1 + ")\\+(" + expression2 + ")";
						Simplify_Question.text = "(" + expression1 + ")\\+(" + expression2 + ")";
						Simplify_Question.text = "(" + expression1 + ")\\+(" + expression2 + ")";
						scoeff1 = coeff1 + coeff4;
						scoeff2 = coeff2 + coeff5;
						scoeff3 = coeff3 + coeff6;	
					}

					CerebroHelper.DebugLog (scoeff1);
					CerebroHelper.DebugLog (scoeff2);
					CerebroHelper.DebugLog (scoeff3);
				}
			}
			UserAnswer.text = "";
		}

		public override void numPadButtonPressed(int value) {
			if (ignoreTouches) {
				return;
			}
			if (value <= 9) {
				UserAnswer.text += value.ToString ();
			} else if (value == 10) {    //Back
				if (UserAnswer.text.Length > 0) {
					if (checkLastTextFor (new string[1]{ "x^2" })) {
						UserAnswer.text = UserAnswer.text.Substring (0, UserAnswer.text.Length - 3);
					} else {
						UserAnswer.text = UserAnswer.text.Substring (0, UserAnswer.text.Length - 1);
					}
				}
			} else if (value == 11) {   // All Clear
				UserAnswer.text = "";
			} else if (value == 12) {   //x^2
				if(checkLastTextFor(new string[1]{"x^2"})) {
					UserAnswer.text = UserAnswer.text.Substring (0, UserAnswer.text.Length - 3);
				}
				if(checkLastTextFor(new string[1]{"x"})) {
					UserAnswer.text = UserAnswer.text.Substring (0, UserAnswer.text.Length - 1);
				}
				UserAnswer.text += "x^2";
			} else if (value == 13) {  //x
				if(checkLastTextFor(new string[1]{"x^2"})) {
					UserAnswer.text = UserAnswer.text.Substring (0, UserAnswer.text.Length - 3);
				}
				if(checkLastTextFor(new string[1]{"x"})) {
					UserAnswer.text = UserAnswer.text.Substring (0, UserAnswer.text.Length - 1);
				}
				UserAnswer.text += "x";
			} else if (value == 14) {
				if(checkLastTextFor(new string[2]{"+","-"})) {
					UserAnswer.text = UserAnswer.text.Substring (0, UserAnswer.text.Length - 1);
				}
				UserAnswer.text += "+";
			} else if (value == 15) {
				if(checkLastTextFor(new string[2]{"+","-"})) {
					UserAnswer.text = UserAnswer.text.Substring (0, UserAnswer.text.Length - 1);
				}
				UserAnswer.text += "-";
			}
		}
	}
}
