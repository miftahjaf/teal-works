using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;

namespace Cerebro
{
	
	public class FactorsAndMultiplesScript : BaseAssessment
	{

		private string Answer;
		//		private string userAnswer;
		public Text subQuestionText;
		public GameObject MCQ;
		private List<int> primes = new List<int> {
			2,
			3,
			5,
			7,
			11,
			13,
			17,
			19,
			23,
			29,
			31,
			37,
			41,
			43,
			47,
			53,
			59,
			61,
			67,
			71,
			73,
			79,
			83,
			89,
			97,
			101,
			103,
			107,
			109,
			113,
			127,
			131,
			137,
			139,
			149,
			151,
			157,
			163,
			167,
			173,
			179,
			181,
			191,
			193,
			197,
			199,
			211,
			223,
			227,
			229,
			233,
			239,
			241,
			251,
			257,
			263,
			269,
			271,
			277,
			281,
			283,
			293
		};

		void Start ()
		{

			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "FAM06", "S01", "A01");

			scorestreaklvls = new int[6];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}
		
		// Update is called once per frame
		void Update ()
		{
		
		}

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

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
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {
					MCQ.SetActive (true);
					numPad.SetActive (false);

					int num1 = Random.Range (5, 41);
					int num2 = Random.Range (2, 7);
					while(num1 == num2)
						num2 = Random.Range (2, 7);
					int num3 = num1 * num2;
					Answer = "Yes";
					if (Random.Range (1, 3) == 1) {
						Answer = "No";
						num3 = num3 + Random.Range (1, num1 - 1);
					}
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option:";
					subQuestionText.text = "Is " + num3.ToString () + " perfectly divisible by " + num1.ToString () + "?";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Yes";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "No";
				} else if (selector == 2) {
					int num1 = Random.Range (5, 41);
					int num2 = Random.Range (1, 7);
					int num3 = num1 * num2;
					int num4 = Random.Range (1, num1 - 1);
					num3 = num3 + num4;
					QuestionText.text = "What is the remainder when " + num3.ToString () + " is divided by " + num1.ToString () + "?";
					Answer = num4.ToString ();
					GeneralButton.gameObject.SetActive (true);

				} else if (selector == 3) {
					int subselector = Random.Range (0, 4);
					int[] subselectorarr = new int[] { 2, 3, 5, 10 };

					int num1 = subselectorarr [subselector];
					int num2 = Random.Range (2, 11);

					int num3 = num1 * num2;

					Answer = "Yes";
					if (Random.Range (1, 3) == 1) {
						Answer = "No";
						num3 = num3 + Random.Range (1, num1);
					}

					MCQ.SetActive (true);
					numPad.SetActive (false);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option:";
					subQuestionText.text = "Is " + num3.ToString () + " divisible by " + num1.ToString () + "?";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Yes";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "No";
				} else if (selector == 4) {
					int num1 = Random.Range (2, 300);
					if (Random.Range (1, 3) == 1) {
						num1 = primes [Random.Range (0, primes.Count)];
						Answer = "Yes";
					} else {
						while (primes.Contains (num1)) {
							num1 = Random.Range (2, 300);
						}
						Answer = "No";
					}
					MCQ.SetActive (true);
					numPad.SetActive (false);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option:";
					subQuestionText.text = "Is " + num1.ToString () + " a prime number ?";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Yes";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "No";
				}
			} else if (level == 2) {
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {
					int num1 = Random.Range (5, 201);
					QuestionText.text = "Find all the factors of " + num1.ToString () + ".";
					Answer = MathFunctions.GetFactors (num1);
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int num1 = Random.Range (2, 20);
					QuestionText.text = "Find the first 5 multiples of " + num1.ToString () + ".";
					Answer = MathFunctions.GetMultiples (num1, 5);
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					int num1 = Random.Range (2, 300);
					if (Random.Range (1, 3) == 1) {
						num1 = primes [Random.Range (0, primes.Count)];
						Answer = "Prime";
					} else {
						while (primes.Contains (num1)) {
							num1 = Random.Range (2, 300);
						}
						Answer = "Composite";
					}

					MCQ.SetActive (true);
					numPad.SetActive (false);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option:";
					subQuestionText.text = "Is " + num1.ToString () + " a Prime number or a Composite number?";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Prime";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "Composite";
				} else if (selector == 4) {
					int num1 = Random.Range (2, 300);
					if (num1 % 2 == 0) {
						Answer = "Even";
					} else {
						Answer = "Odd";
					}

					MCQ.SetActive (true);
					numPad.SetActive (false);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option:";
					subQuestionText.text = "Is " + num1.ToString () + " Even or Odd?";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "Even";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "Odd";
				}
			} else if (level == 3) {
				selector = GetRandomSelector (1, 3);

				if (selector == 1) {
					int nums = Random.Range (2, 5);
					List<string> factors = new List<string> ();
					int num1 = 1;
					for (var i = 0; i < nums; i++) {
						int rndNum = primes [Random.Range (0, 7)];
						num1 *= rndNum;
						factors.Add (rndNum.ToString ());
					}
					QuestionText.text = "Express " + num1.ToString () + " as a product of prime factors.";
					Answer = string.Join ("x", factors.ToArray ());
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int num1 = Random.Range (1, 100);
					int num2 = Random.Range (num1 + 10, num1 + 30);
					QuestionText.text = "Write all prime numbers between " + num1.ToString () + " (inclusive) and " + num2.ToString () + " (inclusive).";
					List<string> answers = new List<string> ();
					for (var i = num1; i <= num2; i++) {
						if (primes.Contains (i)) {
							answers.Add (i.ToString ());
						}
					}
					Answer = string.Join (",", answers.ToArray ());
					GeneralButton.gameObject.SetActive (true);
				}
			} else if (level == 4) {
				selector = GetRandomSelector (1, 3);

				if (selector == 1) {
					int multiple = Random.Range (2, 10);
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 10);
					while (num2 == num1) {
						num2 = Random.Range (1, 10);
					}
					int num3 = Random.Range (1, 10);
					while (num3 == num1 || num3 == num2) {
						num3 = Random.Range (1, 10);
					}
					int lcm = MathFunctions.GetLCM (num1, num2, num3);
					Answer = (lcm * multiple).ToString ();
					num1 = num1 * multiple;
					num2 = num2 * multiple;
					num3 = num3 * multiple;
					QuestionText.text = "Find the LCM of " + num1.ToString () + ", " + num2.ToString () + " and " + num3.ToString () + ".";
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int multiple = Random.Range (2, 10);
					int num1 = Random.Range (2, 30);
					int num2 = Random.Range (2, 30);
					while (num2 == num1) {
						num2 = Random.Range (2, 30);
					}
					int lcm = MathFunctions.GetLCM (num1, num2);
					Answer = (lcm * multiple).ToString ();
					num1 = num1 * multiple;
					num2 = num2 * multiple;
					QuestionText.text = "Find the LCM of " + num1.ToString () + " and " + num2.ToString () + ".";
					GeneralButton.gameObject.SetActive (true);
				}
			} else if (level == 5) {
				selector = GetRandomSelector (1, 3);

				if (selector == 1) {
					int multiple = Random.Range (2, 10);
					int num1 = Random.Range (1, 10);
					int num2 = Random.Range (1, 10);
					while (num2 == num1) {
						num2 = Random.Range (1, 10);
					}
					int num3 = Random.Range (1, 10);
					while (num3 == num1 || num3 == num2) {
						num3 = Random.Range (1, 10);
					}
					num1 = num1 * multiple;
					num2 = num2 * multiple;
					num3 = num3 * multiple;

					int hcf1 = MathFunctions.GetHCF (num1, num2);
					int hcf = MathFunctions.GetHCF (hcf1, num3);

					Answer = hcf.ToString ();
					QuestionText.text = "Find the HCF of " + num1.ToString () + ", " + num2.ToString () + " and " + num3.ToString () + ".";
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int multiple = Random.Range (2, 10);
					int num1 = Random.Range (10, 200);
					int num2 = Random.Range (10, 200);
					while (num2 == num1) {
						num2 = Random.Range (10, 200);
					}
					num1 = num1 * multiple;
					num2 = num2 * multiple;

					int hcf = MathFunctions.GetHCF (num1, num2);
					Answer = hcf.ToString ();
					QuestionText.text = "Find the HCF of " + num1.ToString () + " and " + num2.ToString () + ".";
					GeneralButton.gameObject.SetActive (true);
				}
			} else if (level == 6) {
				selector = GetRandomSelector (1, 6);
			
				if (selector == 1) {
					int num1 = Random.Range (3, 20);
					int num2 = Random.Range (num1 + 1, 30);
					int num3 = Random.Range (1, num1);

					int lcm = MathFunctions.GetLCM (num1, num2);
					Answer = (lcm + num3).ToString ();
					QuestionText.text = "What is the least number which when divided by " + num1.ToString () + " and " + num2.ToString () + " leaves a remainder of " + num3.ToString () + "?";
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int multiple = Random.Range (2, 10);
					int num1 = Random.Range (3, 10);
					int num2 = Random.Range (3, 10);
					while (num2 == num1) {
						num2 = Random.Range (1, 10);
					}
					num1 = num1 * multiple;
					num2 = num2 * multiple;

					int hcf = MathFunctions.GetHCF (num1, num2);
					Answer = hcf.ToString ();

					int num3 = Random.Range (1, hcf);
					int num4 = Random.Range (1, hcf);
					int num5 = num1 + num3;
					int num6 = num2 + num4;

					QuestionText.text = "Find the greatest number that will divide " + num5.ToString () + " and " + num6.ToString () + " leaving the remainders " + num3.ToString () + " and " + num4.ToString () + " respectively.";
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					int num1 = Random.Range (1, 30);
					int num2 = Random.Range (1, 30);
					while (num2 == num1) {
						num2 = Random.Range (1, 30);
					}
					QuestionText.text = "Write two numbers that are multiples of " + num1.ToString () + " and " + num2.ToString () + ".";
					int lcm = MathFunctions.GetLCM (num1, num2);
					Answer = lcm.ToString () + "," + (lcm * 2).ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 4) {
					int num1 = Random.Range (1, 30);
					int num2 = Random.Range (1, 30);
					while (num2 == num1) {
						num2 = Random.Range (1, 30);
					}
					QuestionText.text = "Rahul has rectangular cloth pieces with sides " + num1.ToString () + "cm and " + num2.ToString () + "cm. He has to make a square cloth sheet by using the cloth pieces. What is the minimum length of the final cloth sheet?";
					int lcm = MathFunctions.GetLCM (num1, num2);
					Answer = lcm.ToString ();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 5) {
					int[] subselectorarr = new int[] { 12, 6, 20 };
					string[] subselectordesc = new string[] { "a dozen", "half a dozen", "a score" };
					int subselector = Random.Range (0, subselectorarr.Length);
				
					int num1 = subselectorarr [subselector];
					
					int num2 = Random.Range (2, 10);

					int num3 = num1 * num2;
					int num4 = Random.Range (num1 + 1, 120);
					while (num4 % num1 == 0) {
						num4 = Random.Range (num1 + 1, 120);
					}

					Answer = num3.ToString ();

					if (Random.Range (1, 3) == 1) {
						int tmp = num3;
						num3 = num4;
						num4 = tmp;
					}

					MCQ.SetActive (true);
					numPad.SetActive (false);
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Choose the correct option:";
					subQuestionText.text = "Eggs are packed in boxes of " + subselectordesc [subselector] + " each. Which of the following amounts will fill a whole number of boxes?";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = num3.ToString ();
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = num4.ToString ();
				} 

			}

			if (answerButton != null) {
				userAnswerText = answerButton.gameObject.GetChildByName<Text> ("Text");
				userAnswerText.text = "";
			}

			CerebroHelper.DebugLog (Answer);
		}

		public override void SubmitClick ()
		{
			if (ignoreTouches || userAnswerText.text == "") {
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
				}
			} else {
				if (level == 6 && selector == 3) {
					var userAnswers = userAnswerText.text.Split (new string[] { "," }, System.StringSplitOptions.None);
					var correctAnswers = Answer.Split (new string[] { "," }, System.StringSplitOptions.None);
					for (var i = 0; i < userAnswers.Length; i++) {
						int test;
						if (int.TryParse (userAnswers [i], out test)) {
							if (int.Parse (userAnswers [i]) % int.Parse (correctAnswers [0]) != 0) {
								correct = false;
							}
						} else {
							correct = false;
						}
					}
					if (correct) {
						if (userAnswers.Length < 2) {
							correct = false;
						} else {
							if (int.Parse (userAnswers [0]) == int.Parse (userAnswers [1])) {
								correct = false;
							}
						}
					}
				} else if (Answer.Contains (",")) {
					var correctAnswers = Answer.Split (new string[] { "," }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "," }, System.StringSplitOptions.None);
					correct = checkArrays (correctAnswers, userAnswers);
				} else if (Answer.Contains ("x")) {
					var correctAnswers = Answer.Split (new string[] { "x" }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split (new string[] { "x" }, System.StringSplitOptions.None);
					correct = checkArrays (correctAnswers, userAnswers);
				} else {
					if (userAnswerText.text == Answer) {
						correct = true;
					} else {
						correct = false;
						float test;
						if (float.TryParse (userAnswerText.text, out test) && float.TryParse (Answer, out test)) {
							if (float.Parse (userAnswerText.text) == float.Parse (Answer)) {
								correct = true;
							}
						}

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
				} else if (Queslevel == 5) {
					increment = 15;
				} else if (Queslevel == 6) {
					increment = 15;
				} else if (Queslevel == 7) {
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

		bool checkArrays (string[] a, string[]b)
		{
			
			foreach (var numberA in a) {
				bool hasDuplicate = false;
				foreach (var numberB in b) {
					if (numberA == numberB) {
						hasDuplicate = true;
					} else {
						float test;
						if (float.TryParse (numberA, out test) && float.TryParse (numberB, out test)) {
							if (float.Parse (numberA) == float.Parse (numberB)) {
								hasDuplicate = true;
							}
						}
					}
				}
				if (!hasDuplicate) {
					return false;
				}
			}
			return true;
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

		protected override IEnumerator ShowCorrectAnimation ()
		{
			userAnswerText.color = MaterialColor.green800;
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations (2, GoLoopType.PingPong);
			var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1));
			var tween = new GoTween (userAnswerText.gameObject.transform, 0.2f, config);
			flow.insert (0f, tween);
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

		protected override IEnumerator ShowWrongAnimation ()
		{
			userAnswerText.color = MaterialColor.red800;
			Go.to (userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				if (numPad.activeSelf) {				// is not MCQ type question
					userAnswerText.text = "";
				}
				userAnswerText.color = MaterialColor.textDark;
				ignoreTouches = false;
			} else {
				if (numPad.activeSelf) {				// is not MCQ type question
					userAnswerText.text = Answer.ToString ();
					userAnswerText.color = MaterialColor.green800;
				} else {
					userAnswerText.color = MaterialColor.textDark;
				}
			}
			ShowContinueButton ();
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
			} else if (value == 12) {   // ,
				if (checkLastTextFor (new string[1]{ "," })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ",";
			} else if (value == 13) {   // -
				if (checkLastTextFor (new string[1]{ "x" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "x";
			}
		}
	}
}
