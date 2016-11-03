using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro
{
	public class FractionsScript : BaseAssessment
	{

		public TEXDraw subQuestionText;

		public GameObject MCQ;
		private string Answer;

		private int[] numbers = new int[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		void Start ()
		{

			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "FRA06", "S01", "A01");

			scorestreaklvls = new int[7];
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

			if (!numPad.activeSelf) {
				if (userAnswerLaText.text == Answer) {
					correct = true;
				} else {
					correct = false;
					AnimateMCQOptionCorrect(Answer);
				}
			} else {
				if (Answer.Contains ("=")) {
					string correctAnswer = Answer.Split (new string[] { "=" }, System.StringSplitOptions.None) [1];
					if (userAnswerText.text == correctAnswer) {
						correct = true;
					} else {
						Answer = correctAnswer;
						correct = false;
					}
				} else if (Answer.Contains (",")) {
					var correctAnswers = Answer.Split (new string[] { "," }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "," }, System.StringSplitOptions.None);
					if (correctAnswers.Length != userAnswers.Length) {
						correct = false;
					} else {
						for (var i = 0; i < correctAnswers.Length; i++) {
							var correctFraction = correctAnswers[i].Split (new string[] { "/" }, System.StringSplitOptions.None);
							var userFraction = userAnswers[i].Split (new string[] { "/" }, System.StringSplitOptions.None);
							if (!MathFunctions.checkFractions (userFraction, correctFraction)) {
								correct = false;
								break;
							}
						}
					}
				} else if (Answer.Contains ("/")) {
					var correctAnswers = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
					correct = MathFunctions.checkFractions (userAnswers, correctAnswers);
				} else {
					if (userAnswerText.text == Answer) {
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
					increment = 10;
				} else if (Queslevel == 5) {
					increment = 15;
				} else if (Queslevel == 6) {
					increment = 15;
				} else if (Queslevel == 7) {
					increment = 15;
				} else if (Queslevel == 8) {
					increment = 15;
				}

				UpdateStreak (8, 12);

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
			// Generating the parameters
			for (int i = 1; i < 3; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}
			level = Queslevel;

			answerButton = GeneralButton;

			subQuestionText.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (false);
			MCQ.gameObject.SetActive (false);
			numPad.SetActive (true);

			if (Queslevel > scorestreaklvls.Length) {
				level = Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				selector = GetRandomSelector (1, 4);

				if (selector == 1) {
					int num1 = Random.Range (1, 20);
					int num2 = Random.Range (2, 30);
					while(num1 == num2 || MathFunctions.GetHCF (num1, num2) > 1)
						num2 = Random.Range (2, 30);
					QuestionText.text = "Make a fraction using " + num1.ToString () + " as numerator and " + num2.ToString () + " as denominator.";
					Answer = num1.ToString () + "/" + num2.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					MCQ.SetActive (true);
					numPad.SetActive (false);

					int num1 = Random.Range (1, 40);
					int num2 = Random.Range (2, 40);
					while (num1 == num2 || MathFunctions.GetHCF (num1, num2) > 1)
						num1 = Random.Range (1, 40);

					if (num1 > num2) {
						Answer = "No";
					} else {
						Answer = "Yes";
					}

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option :";
					subQuestionText.text = "Is \\frac{" + num1.ToString () + "}{" + num2.ToString () + "} a proper fraction? ";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "Yes";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "No";
				} else if (selector == 3) {
					MCQ.SetActive (true);
					numPad.SetActive (false);

					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 40);
					int num3 = Random.Range (10, 40);
					num1 /= MathFunctions.GetHCF (num1, num3);
					int num4 = num3;
					if (Random.Range (1, 3) == 1) {
						Answer = "No";
						while (num4 == num3) {
							num4 = Random.Range (10, 40);
						}
					} else {
						Answer = "Yes";
					}

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option :";
					subQuestionText.text = "Are \\frac{" + num1.ToString () + "}{" + num3.ToString () + "} and \\frac{" + num2.ToString () + "}{" + num4.ToString () + "} like fractions? ";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "Yes";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "No";
				}
			} else if (level == 2) {
				selector = GetRandomSelector (1, 4);

				if (selector == 1) {
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 5);
					int num3 = Random.Range (num2 + 1, 10);
					while (MathFunctions.GetHCF (num3, num2) != 1)
						num3 = Random.Range (num2 + 1, 10);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert the given mixed fraction to improper fraction :";
					subQuestionText.text = num1.ToString () + " \\frac{" + num2.ToString () + "}{" + num3.ToString () + "}";
					Answer = ((num1 * num3) + num2).ToString () + "/" + num3.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {

					MCQ.SetActive (true);
					numPad.SetActive (false);
					subQuestionText.gameObject.SetActive (true);

					int num = Random.Range (1, 6);
					int num1 = Random.Range (1, 6);
					int num2 = Random.Range (num1 + 2, 10);
					int num3;

					while (MathFunctions.GetHCF (num2, num1) > 1)
						num1 = Random.Range (1, 6);

					string option1 = num + " \\frac{" + num1 + "}{" + num2 + "}";
					string option2 = "";

					if (Random.Range (1, 3) == 1) 
					{
						if (num == 1)
							num3 = 2;
						else if (Random.Range (1, 3) == 1)
							num3 = num + 1;
						else
							num3 = num - 1;

						option2 = num3 + " \\frac{" + num1 + "}{" + num2 + "}";
					} 
					else 
					{
						num3 = Random.Range (1, num2);

						while (MathFunctions.GetHCF (num2, num3) > 1 || num3 == num1)
							num3 = Random.Range (1, num2);

						option2 = num + " \\frac{" + num3 + "}{" + num2 + "}";
					}

					QuestionText.text = "Convert the given improper fraction to mixed fraction : ";
					subQuestionText.text = "\\frac{" + (num * num2 + num1) + "}{" + num2.ToString () + "}";
					Answer = option1;

					if (Random.Range (1, 3) == 1) {
						string tmp = option1;
						option1 = option2;
						option2 = tmp;
					}

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = option1;
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = option2;

				} else if (selector == 3) {
					int[] subselectorarr = new int[] { 12, 20 };
					string[] subselectordesc = new string[] { "years", "scores", "months", "bananas" };
					int subselector = Random.Range (0, subselectorarr.Length);

					int num1 = Random.Range (2, 100);
					Answer = num1.ToString () + "/" + subselectorarr [subselector].ToString ();
					QuestionText.text = "Express " + num1.ToString () + " " + subselectordesc [subselector + subselectorarr.Length] + " in " + subselectordesc [subselector] + " as an improper fraction.";
					GeneralButton.gameObject.SetActive (true);
				}
			} else if (level == 3) {
				selector = GetRandomSelector (1, 3);

				if (selector == 1) {
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (num1 + 1, 20);
					int multiplier = Random.Range (2, 10);
					int hcf = MathFunctions.GetHCF (num1, num2);
					num1 = num1 / hcf;
					num2 = num2 / hcf;
					Answer = "=" + num1.ToString () + "/" + num2.ToString ();
					num1 *= multiplier;
					num2 *= multiplier;

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert to simplest form :";
					subQuestionText.text = "\\frac{" + num1.ToString () + "}{" + num2.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int num1 = Random.Range (2, 6);
					int num2 = Random.Range (5, 10);
					int num3 = 1;
					int den1 = Random.Range (2, 6);
					int den2 = Random.Range (5, 10);
					int den3 = 2 * den1;
					while (MathFunctions.GetHCF (num1, den1) != 1 || MathFunctions.GetHCF (num2, den2) != 1 || den1 == den2 || den2 == den3) {
						den1 = Random.Range (2, 10);
						den2 = Random.Range (2, 10);
						den3 = 2 * den1;
					}
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Convert to like fractions (separate by comma) :";
					subQuestionText.text = "\\frac{" + num1.ToString () + "}{" + den1.ToString () + "}" + ", \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}" + ", \\frac{" + num3.ToString () + "}{" + den3.ToString () + "}";
					GeneralButton.gameObject.SetActive (true); 

					int lcm = MathFunctions.GetLCM (den1, den2, den3);
					num1 = (lcm / den1) * num1;
					num2 = (lcm / den2) * num2;
					num3 = (lcm / den3) * num3;
					Answer = num1.ToString () + "/" + lcm.ToString () + "," + num2.ToString () + "/" + lcm.ToString () + "," + num3.ToString () + "/" + lcm.ToString ();
				}
			} else if (level == 4) {
				selector = GetRandomSelector (1, 3);

				if (selector == 1) {
					MCQ.SetActive (true);
					numPad.SetActive (false);

					int num1 = Random.Range (1, 50);
					int num2 = Random.Range (1, 50);
					int den1 = Random.Range (2, 50);
					int den2 = Random.Range (2, 50);

					while ((float)num1 / (float)den1 == (float)num2 / (float)den2) {
						den2 = Random.Range (2, 50);
					}

					string option1 = "\\frac{" + num1.ToString () + "}{" + den1.ToString () + "}";
					string option2 = "\\frac{" + num2.ToString () + "}{" + den2.ToString () + "}";

					string descriptor = "greater";
					if (Random.Range (1, 3) == 1) {		// lesser
						descriptor = "smaller";
						if ((float)num1 / (float)den1 > (float)num2 / (float)den2) {
							Answer = option2;
						} else {
							Answer = option1;
						}
					} else {							//greater
						if ((float)num1 / (float)den1 > (float)num2 / (float)den2) {
							Answer = option1;
						} else {
							Answer = option2;
						}
					}

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option :";
					subQuestionText.text = "Which is the " + descriptor + " fraction?";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = option1;
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = option2;
				} else if (selector == 2) {
					MCQ.SetActive (true);
					numPad.SetActive (false);

					int num1 = Random.Range (1, 50);
					int num2 = Random.Range (1, 50);
					int den1 = Random.Range (2, 50);
					int den2 = Random.Range (2, 50);

					while ((float)num1 / (float)den1 == (float)num2 / (float)den2) {
						den2 = Random.Range (2, 50);
					}

					string option1 = "day 1";
					string option2 = "day 2";

					if ((float)num1 / (float)den1 > (float)num2 / (float)den2) {
						Answer = option1;
					} else {
						Answer = option2;
					}

					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option :";
					subQuestionText.text = "Ajay read \\frac{" + num1.ToString () + "}{" + den1.ToString () + "} of a book on day 1 and \\frac{" + num2.ToString () + "}{" + den2.ToString () + "} of the same book on day 2. When did he read more?";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = option1;
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = option2;
				}
			} else if (level == 5) {
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 10);
					int num3 = Random.Range (1, 10);
					int den = Random.Range (2, 20);

					Answer = (num1 + num2 + num3).ToString () + "/" + den.ToString ();
					QuestionText.text = "Simplify :";
					subQuestionText.gameObject.SetActive (true);
					subQuestionText.text = "\\frac{" + num1.ToString () + "}{" + den.ToString () + "}" + " + \\frac{" + num2.ToString () + "}{" + den.ToString () + "}" + " + \\frac{" + num3.ToString () + "}{" + den.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);

				} else if (selector == 2) {
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 10);
					int num3 = Random.Range (1, 10);
					int den1 = Random.Range (2, 6);
					int den2 = Random.Range (den1+1, 10);
					int den3 = 2 * den1;
					num1 /= MathFunctions.GetHCF (num1, den1);
					num2 /= MathFunctions.GetHCF (num2, den2);
					num3 /= MathFunctions.GetHCF (num3, den3);

					QuestionText.text = "Simplify :";
					subQuestionText.gameObject.SetActive (true);
					subQuestionText.text = "\\frac{" + num1.ToString () + "}{" + den1.ToString () + "}" + " + \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}" + " + \\frac{" + num3.ToString () + "}{" + den3.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);

					int lcm = MathFunctions.GetLCM (den1, den2, den3);
					num1 = (lcm / den1) * num1;
					num2 = (lcm / den2) * num2;
					num3 = (lcm / den3) * num3;
					Answer = (num1 + num2 + num3).ToString () + "/" + lcm.ToString ();
				} else if (selector == 3) {
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 10);
					int num3 = Random.Range (1, 10);
					int den1 = Random.Range (2, 6);
					int den2 = Random.Range (2, 6);
					int den3 = Random.Range (2, 6);

					while (MathFunctions.GetHCF (num1, den1) != 1 || num1 == den1)
						num1 = Random.Range (1, 10);
					while (MathFunctions.GetHCF (num2, den2) != 1 || num2 == den2)
						num2 = Random.Range (1, 10);
					while (MathFunctions.GetHCF (num3, den3) != 1 || num3 == den3)
						num3 = Random.Range (1, 10);

					int sign1 = 1;
					int sign2 = 1;
					string sign1s = " + ";
					string sign2s = " + ";
					if (Random.Range (1, 3) == 1) {
						sign1 = -1;
						sign1s = " - ";
					}
					if (Random.Range (1, 3) == 1) {
						sign2 = -1;
						sign2s = " - ";
					}

					QuestionText.text = "Simplify :";
					subQuestionText.gameObject.SetActive (true);
					subQuestionText.text = "\\frac{" + num1.ToString () + "}{" + den1.ToString () + "}" + sign1s + "\\frac{" + num2.ToString () + "}{" + den2.ToString () + "}" + sign2s + "\\frac{" + num3.ToString () + "}{" + den3.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);

					int lcm = MathFunctions.GetLCM (den1, den2, den3);
					num1 = (lcm / den1) * num1;
					num2 = (lcm / den2) * num2;
					num3 = (lcm / den3) * num3;

					num1 = num1 + (sign1 * num2) + (sign2 * num3);
					int hcf = MathFunctions.GetHCF (num1, lcm);
					if (hcf > 1) {
						num1 /= hcf;
						lcm /= hcf;
					}
					Answer = (num1).ToString () + "/" + lcm.ToString ();
				} else if (selector == 4) {
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 10);
					int num3 = Random.Range (1, 10);
					int den1 = Random.Range (2, 5);
					int den2 = Random.Range (den1+1, 8);
					int den3 = Random.Range (den2+1, 11);

					while (MathFunctions.GetHCF (num1, den1) != 1 || num1 == den1)
						num1 = Random.Range (1, 10);
					while (MathFunctions.GetHCF (num2, den2) != 1 || num2 == den2)
						num2 = Random.Range (1, 10);
					while (MathFunctions.GetHCF (num3, den3) != 1 || num3 == den3)
						num3 = Random.Range (1, 10);

					int sign1 = 1;
					int sign2 = -1;

					QuestionText.text = "Solve :";
					subQuestionText.gameObject.SetActive (true);
					subQuestionText.text = "Subtract \\frac{" + num3.ToString () + "}{" + den3.ToString () + "} from the sum of \\frac{" + num2.ToString () + "}{" + den2.ToString () + "} and \\frac{" + num1.ToString () + "}{" + den1.ToString () + "}.";
					GeneralButton.gameObject.SetActive (true);

					int lcm = MathFunctions.GetLCM (den1, den2, den3);
					num1 = (lcm / den1) * num1;
					num2 = (lcm / den2) * num2;
					num3 = (lcm / den3) * num3;
					Answer = (num1 + (sign1 * num2) + (sign2 * num3)).ToString () + "/" + lcm.ToString ();
				}  else if (selector == 5) {
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 10);
					int den1 = Random.Range (2, 10);
					int den2 = Random.Range (2, 10);

					while (MathFunctions.GetHCF (num1, den1) != 1 || num1 == den1)
						num1 = Random.Range (1, 10);
					while (MathFunctions.GetHCF (num2, den2) != 1 || num2 == den2)
						num2 = Random.Range (1, 10);
					
					QuestionText.text = "Solve :";
					subQuestionText.gameObject.SetActive (true);
					subQuestionText.text = "What should we subtract from \\frac{" + num1.ToString () + "}{" + den1.ToString () + "} to get \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}?";
					GeneralButton.gameObject.SetActive (true);

					int lcm = MathFunctions.GetLCM (den1, den2);
					num1 = (lcm / den1) * num1;
					num2 = (lcm / den2) * num2;
					Answer = (num1 - num2).ToString () + "/" + lcm.ToString ();
				}
			} else if (level == 6) {
				selector = GetRandomSelector (1, 4);

				if (selector == 1) {
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 10);
					int den1 = Random.Range (2, 10);
					int den2 = num1 * Random.Range (2, 10);

					while (MathFunctions.GetHCF (num1, den1) != 1 || num1 == den1)
						num1 = Random.Range (1, 10);
					while (MathFunctions.GetHCF (num2, den2) != 1 || num2 == den2)
						num2 = Random.Range (1, 10);

					if (Random.Range (1, 3) == 1) {
						num1 *= -1;
					}
					if (Random.Range (1, 3) == 1) {
						num2 *= -1;
					}

					QuestionText.text = "Simplify :";
					subQuestionText.gameObject.SetActive (true);
					subQuestionText.text = "\\frac{" + num1.ToString () + "}{" + den1.ToString () + "} \\times \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}";
					GeneralButton.gameObject.SetActive (true);
					int hcf = Mathf.Abs(MathFunctions.GetHCF (num1 * num2, den1 * den2));
					Answer = "=" + (num1 * num2/hcf).ToString () + "/" + (den1 * den2/hcf).ToString ();
				} else if (selector == 2) {
					int whole1 = Random.Range (1, 10);
					int num1 = Random.Range (1, 6);
					int den1 = Random.Range (num1 + 1, 10);

					int whole2 = Random.Range (1, 10);
					int num2 = Random.Range (1, 6);
					int den2 = Random.Range (num2 + 1, 10);

					while (MathFunctions.GetHCF (num1, den1) != 1)
						num1 = Random.Range (1, 6);
					while (MathFunctions.GetHCF (num2, den2) != 1)
						num2 = Random.Range (1, 6);

					QuestionText.text = "Simplify :";
					subQuestionText.gameObject.SetActive (true);
					GeneralButton.gameObject.SetActive (true);

					int subselector = Random.Range (1, 10);
					if (subselector <= 4) {
						subQuestionText.text = whole1.ToString () + " \\frac{" + num1.ToString () + "}{" + den1.ToString () + "} \\times " + whole2.ToString () + " \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}";
						num1 = (whole1 * den1) + num1;
						num2 = (whole2 * den2) + num2;
						Answer = (num1 * num2).ToString () + "/" + (den1 * den2).ToString ();
					} else if (subselector <= 8) {
						subQuestionText.text = whole1.ToString () + " \\frac{" + num1.ToString () + "}{" + den1.ToString () + "} \\times \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}";
						num1 = (whole1 * den1) + num1;
						Answer = (num1 * num2).ToString () + "/" + (den1 * den2).ToString ();
					} else {
						subQuestionText.text = whole1.ToString () + " \\frac{" + num1.ToString () + "}{" + den1.ToString () + "} \\times 0 \\times " + whole2.ToString () + " \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}";
						num1 = (whole1 * den1) + num1;
						num2 = (whole2 * den2) + num2;
						Answer = "=0";
					}
				} else if (selector == 3) {
					int whole1 = Random.Range (1, 10);
					int num1 = Random.Range (1, 6);
					int den1 = Random.Range (num1 + 1, 10);

					int whole2 = Random.Range (1, 10);
					int num2 = Random.Range (1, 6);
					int den2 = Random.Range (num2 + 1, 10);

					while (MathFunctions.GetHCF (num1, den1) != 1)
						num1 = Random.Range (1, 6);
					while (MathFunctions.GetHCF (num2, den2) != 1)
						num2 = Random.Range (1, 6);

					QuestionText.text = "Divide";
					subQuestionText.gameObject.SetActive (true);
					GeneralButton.gameObject.SetActive (true);

					int subselector = Random.Range (1, 10);
					if (subselector <= 4) {
						subQuestionText.text = whole1.ToString () + " \\frac{" + num1.ToString () + "}{" + den1.ToString () + "} \\div " + whole2.ToString () + " \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}";
						num1 = (whole1 * den1) + num1;
						num2 = (whole2 * den2) + num2;
						Answer = (num1 * den2).ToString () + "/" + (num2 * den1).ToString ();
					} else if (subselector <= 8) {
						subQuestionText.text = whole1.ToString () + " \\frac{" + num1.ToString () + "}{" + den1.ToString () + "} \\div \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}";
						num1 = (whole1 * den1) + num1;
						Answer = (num1 * den2).ToString () + "/" + (num2 * den1).ToString ();
					} else {
						subQuestionText.text = " 0  \\div " + whole2.ToString () + " \\frac{" + num2.ToString () + "}{" + den2.ToString () + "}";
						Answer = "=0";
					}
				} 
			} else if (level == 7) {
				selector = GetRandomSelector (1, 2);

				if (selector == 1) {
					int num1 = Random.Range (1, 6);
					int den1 = Random.Range (num1 + 1, 10);

					int whole2 = Random.Range (1, 10);
					int num2 = Random.Range (1, 6);
					int den2 = Random.Range (num2 + 1, 10);

					int whole3 = Random.Range (1, 10);
					int num3 = Random.Range (1, 6);
					int den3 = Random.Range (num3 + 1, 10);

					while (MathFunctions.GetHCF (num1, den1) != 1)
						num1 = Random.Range (1, 6);
					while (MathFunctions.GetHCF (num2, den2) != 1)
						num2 = Random.Range (1, 6);
					while (MathFunctions.GetHCF (num3, den3) != 1)
						num3 = Random.Range (1, 6);

					string[] operations = new string[]{ "+", "-", "\\div", "\\times" };
					string operation1 = operations [Random.Range (0, operations.Length)];
					string operation2 = operations [Random.Range (0, operations.Length)];

					QuestionText.text = "Simplify";
					subQuestionText.gameObject.SetActive (true);
					GeneralButton.gameObject.SetActive (true);

					subQuestionText.text = "( \\frac{" + num1.ToString () + "}{" + den1.ToString () + "} " + operation1 + " " + whole2.ToString () + " \\frac{" + num2.ToString () + "}{" + den2.ToString () + "} ) " + operation2 + " " + whole3.ToString() + " \\frac{" + num3.ToString () + "}{" + den3.ToString () + "}";
					num2 = (whole2 * den2) + num2;
					num3 = (whole3 * den3) + num3;
					if (operation1 == "+") {
						int lcm = MathFunctions.GetLCM (den1, den2);
						num1 = (lcm / den1) * num1;
						num2 = (lcm / den2) * num2;
						Answer = (num1 + num2).ToString () + "/" + lcm.ToString ();
					} else if (operation1 == "-") {
						int lcm = MathFunctions.GetLCM (den1, den2);
						num1 = (lcm / den1) * num1;
						num2 = (lcm / den2) * num2;
						Answer = (num1 - num2).ToString () + "/" + lcm.ToString ();
					} else if (operation1 == "\\div") {
						Answer = (num1 * den2).ToString () + "/" + (den1 * num2).ToString ();
					} else if (operation1 == "\\times") {
						Answer = (num1 * num2).ToString () + "/" + (den1 * den2).ToString ();
					}

					int tmpansnum = int.Parse (Answer.Split (new string[] { "/" }, System.StringSplitOptions.None) [0]);
					int tmpansnden = int.Parse (Answer.Split (new string[] { "/" }, System.StringSplitOptions.None) [1]);

					if (operation2 == "+") {
						int lcm = MathFunctions.GetLCM (tmpansnden, den3);
						tmpansnum = (lcm / tmpansnden) * tmpansnum;
						num3 = (lcm / den3) * num3;
						Answer = (tmpansnum + num3).ToString () + "/" + lcm.ToString ();
					} else if (operation2 == "-") {
						int lcm = MathFunctions.GetLCM (tmpansnden, den3);
						tmpansnum = (lcm / tmpansnden) * tmpansnum;
						num3 = (lcm / den3) * num3;
						Answer = (tmpansnum - num3).ToString () + "/" + lcm.ToString ();
					} else if (operation2 == "\\div") {
						Answer = (tmpansnum * den3).ToString () + "/" + (tmpansnden * num3).ToString ();
					} else if (operation2 == "\\times") {
						Answer = (tmpansnum * num3).ToString () + "/" + (tmpansnden * den3).ToString ();
					}
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
			} else if (value == 13) {   // ,
				if (checkLastTextFor (new string[1]{ "," })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ",";
			} else if (value == 14) {   // -
				if (checkLastTextFor (new string[1]{ "-" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
		}
	}
}
