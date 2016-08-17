using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MaterialUI;
using B83.ExpressionParser;

namespace Cerebro {
	public class NumberSystemScript : BaseAssessment {

		public Text subQuestionText;

		private string Answer;

		// Use this for initialization
		void Start () {
			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "NS06", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}
		
		public override void SubmitClick() {
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

			var answerNumber = float.Parse (Answer);
			float userAnswerNumber = 0;
			if (float.TryParse (userAnswerText.text, out userAnswerNumber)) {
				userAnswerNumber = float.Parse (userAnswerText.text);
			}

			if (answerNumber == userAnswerNumber) {
				correct = true;
			} else {
				correct = false;
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

		protected override void GenerateQuestion () {
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			level = Queslevel;

			answerButton = GeneralButton;
			subQuestionText.gameObject.SetActive (false);

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {												//Absolute value of number
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Find the absolute value";
					float num = Random.Range (-500, 500);
					subQuestionText.text = "| " + num + " |";
					Answer = Mathf.Abs (num).ToString ();
				} else if (selector == 2) {											//operations on absolute value
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Solve";
					float num1 = Random.Range (-100, 100);
					float num2 = Random.Range (-100, 100);
					if (Random.Range (0, 2) == 0) {
						subQuestionText.text = "| " + num1 + " |" + " + | " + num2 + " |";
						Answer = (Mathf.Abs (num1) + Mathf.Abs (num2)).ToString ();
					} else {
						subQuestionText.text = "| " + num1 + " |" + " - | " + num2 + " |";
						Answer = (Mathf.Abs (num1) - Mathf.Abs (num2)).ToString ();
					}
				} else if (selector == 3) {											//negative of absolute value
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Solve";
					float num1 = Random.Range (-500, 500);
					subQuestionText.text = "- | " + num1 + " |";
					Answer = (-Mathf.Abs (num1)).ToString ();
				} else if (selector == 4) {											//addition/subtraction of integers
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Solve";
					float num1 = Random.Range (-100, 100);
					float num2 = Random.Range (-100, 100);
					if (Random.Range (0, 2) == 0) {
						subQuestionText.text = "( " + num1 + " ) + ( " + num2 + " )";
						Answer = (num1 + num2).ToString ();
					} else {
						subQuestionText.text = "( " + num1 + " ) - ( " + num2 + " )";
						Answer = (num1 - num2).ToString ();
					}
				}
			} else if (level == 2) {
				selector = GetRandomSelector (1, 4);

				if (selector == 1) {												//multiplication/division of integers
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Solve";
					float num1 = Random.Range (-25, 25);
					float num2 = Random.Range (-25, 25);
					if (Random.Range (0, 2) == 0) {
						subQuestionText.text = "( " + num1 + " ) X ( " + num2 + " )";
						Answer = (num1 * num2).ToString ();
					} else {
						float multiplier = Random.Range (-10, 10);
						num2 = num1 * multiplier;
						subQuestionText.text = "( " + num2 + " ) / ( " + num1 + " )";
						Answer = (num2 / num1).ToString ();
					}
				} else if (selector == 2) {											//if ab=x, what is a.-b, -a.b
					subQuestionText.gameObject.SetActive (true);
					float num1 = Random.Range (-100, 100);
					QuestionText.text = "If a x b = " + num1;

					float num2 = Random.Range (0, 3);
					if (num2 == 0) {
						subQuestionText.text = "Find (-a) x (b)";
						Answer = (-num1).ToString ();
					} else if (num2 == 1) {
						subQuestionText.text = "Find (a) x (-b)";
						Answer = (-num1).ToString ();
					} else {
						subQuestionText.text = "Find (-a) x (-b)";
						Answer = (num1).ToString ();
					}
					
				} else if (selector == 3) {												//Word problems
					QuestionText.text = "Word Problem";

					subQuestionText.gameObject.SetActive (true);
					float num1 = Random.Range (-100, 100);
					while(num1 == 0)
						num1 = Random.Range (-100, 100);
					float num2 = Random.Range (-100, 100);
					while(num2 == 0)
						num2 = Random.Range (-100, 100);

					float randNum = Random.Range (0, 5);
					if (randNum == 0) {
						subQuestionText.text = "What should we add to " + num1 + " to get " + (num1 + num2).ToString () + "?";
						Answer = num2.ToString ();
					} else if (randNum == 1) {
						subQuestionText.text = "What should we subtract from " + num1 + " to get " + (num1 - num2).ToString () + "?";
						Answer = num2.ToString ();
					} else if (randNum == 2) {
						subQuestionText.text = "Subtract " + num1 + " from " + num2;
						Answer = (num2 - num1).ToString ();
					} else if (randNum == 3) {
						float num3 = Random.Range (-100, 100);
						while(num3 == 0)
							num3 = Random.Range (-100, 100);
						subQuestionText.text = "Subtract " + num1 + " from the sum of " + num2 + " and " + num3;
						Answer = ((num3 + num2) - num1).ToString ();
					} else if (randNum == 4) {
						float num3 = Random.Range (-100, 100);
						while(num3 == 0)
							num3 = Random.Range (-100, 100);
						subQuestionText.text = "Add " + num1 + " to the difference of " + num2 + " and " + num3;
						Answer = ((num2 - num3) + num1).ToString ();
					}
				}
			} else if (level == 3) {
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {												//if abc=x, what is a.-b.c, a.b.-c etc.
					subQuestionText.gameObject.SetActive (true);
					float num1 = Random.Range (-100, 100);
					if (Random.Range (0, 2) == 0) {
						QuestionText.text = "If (a) x (b) x (c) = " + num1;

						float num2 = Random.Range (0, 5);
						if (num2 == 0) {
							subQuestionText.text = "Find (-a) x (b) x (c)";
							Answer = (-num1).ToString ();
						} else if (num2 == 1) {
							subQuestionText.text = "Find (-a) x (-b) x (-c)";
							Answer = (-num1).ToString ();
						} else if (num2 == 2) {
							subQuestionText.text = "Find (-a) x (b) x (-c)";
							Answer = (num1).ToString ();
						} else if (num2 == 3) {
							subQuestionText.text = "Find (a) x (-b) x (c)";
							Answer = (-num1).ToString ();
						} else {
							subQuestionText.text = "Find (a) x (-b) x (-c)";
							Answer = (num1).ToString ();
						}
					} else {
						QuestionText.text = "If (a) x (-b) x (c) = " + num1;

						float num2 = Random.Range (0, 5);
						if (num2 == 0) {
							subQuestionText.text = "Find (-a) x (b) x (c)";
							Answer = (num1).ToString ();
						} else if (num2 == 1) {
							subQuestionText.text = "Find (-a) x (-b) x (-c)";
							Answer = (num1).ToString ();
						} else if (num2 == 2) {
							subQuestionText.text = "Find (-a) x (b) x (-c)";
							Answer = (-num1).ToString ();
						} else if (num2 == 3) {
							subQuestionText.text = "Find (-a) x (b) x (c)";
							Answer = (num1).ToString ();
						} else {
							subQuestionText.text = "Find (a) x (-b) x (-c)";
							Answer = (-num1).ToString ();
						}
					}
				} else if (selector == 2) {												//Simplify multiple operators
					QuestionText.text = "Simplify";

					subQuestionText.gameObject.SetActive (true);

					string equation = "";
					string[] ops = new string[]{ "*", "+", "-" };
					for (var i = 0; i < 5; i++) {
						var num = Random.Range (-10, 10);
						while (num == 0) {
							num = Random.Range (-10, 10);
						}
						var op = ops [Random.Range (0, ops.Length)];
						if (i != 4) {
							equation += "( " + num.ToString () + " ) " + op + " ";	
						} else {
							equation += "( " + num.ToString () + " )";
						}
					}
					var parser = new ExpressionParser ();
					Expression exp = parser.EvaluateExpression (equation);

					equation = equation.Replace ("*", "x");
					Answer = exp.Value.ToString ();
					subQuestionText.text = equation;
				} else if (selector == 3) {												//Simplify multiple operators with brackets
					QuestionText.text = "Simplify";

					subQuestionText.gameObject.SetActive (true);

					var num1 = Random.Range (-10, 10);
					var num2 = Random.Range (-10, 10);
					var num3 = Random.Range (-10, 10);
					var num4 = Random.Range (-10, 10);
					var num5 = Random.Range (-10, 10);
					var num6 = Random.Range (-10, 10);

					var randNum = Random.Range (0, 4);
					string equation = "";	
					if (randNum == 0) {
						equation = "( (" + num1 + ") * (" + num2 + ") ) + ( (" + num3 + ") * (" + num4 + ") )";
					} else if (randNum == 1) {
						equation = "( (" + num1 + ") * (" + num2 + ") ) - ( (" + num3 + ") * (" + num4 + ") )";
					} else if (randNum == 2) {
						equation = "(" + num1 + ") * ( (" + num2 + ") + (" + num3 + ") ) + (" + num4 + ") * ( (" + num5 + ") + (" + num6 + ") )";
					} else if (randNum == 3) {
						equation = "(" + num1 + ") * ( (" + num2 + ") + (" + num3 + ") ) - (" + num4 + ") * ( (" + num5 + ") + (" + num6 + ") )";
					}

					CerebroHelper.DebugLog (equation);
					var parser = new ExpressionParser ();
					Expression exp = parser.EvaluateExpression (equation);

					equation = equation.Replace ("*", "x");
					Answer = exp.Value.ToString ();
					subQuestionText.text = equation;
				} else if (selector == 4) {
					QuestionText.text = "Find the missing number";

					var num1 = Random.Range (-10, 10);
					var num2 = Random.Range (-10, 10);

					subQuestionText.gameObject.SetActive (true);
					var randNum = Random.Range (0, 3);
					if (randNum == 0) {
						var num3 = num1 + num2;
						subQuestionText.text = num1.ToString() + " + ? = " + num3;
						Answer = num2.ToString();
					} else if (randNum == 1) {
						var num3 = num1 - num2;
						subQuestionText.text = num1.ToString() + " - ? = " + num3;
						Answer = num2.ToString();
					} else if (randNum == 2) {
						while (num1 == 0) {
							num1 = Random.Range (-10, 10);
						}
						var num3 = num1 * num2;
						subQuestionText.text = num1.ToString() + " x ? = " + num3;
						Answer = num2.ToString();
					}

				}
			}

			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
			CerebroHelper.DebugLog (Answer);
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
			} else if (value == 12) {   // -
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			} 
		}
	}
}
