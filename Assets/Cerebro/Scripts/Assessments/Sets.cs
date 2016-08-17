using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;


namespace Cerebro {
	public class Sets : BaseAssessment {

		public TEXDraw subQuestionText;
		public TEXDraw subQuestionText2;

		private int[] Answer;

		public GameObject EquationButtons;
		public GameObject RangeButtons;

		private int[] numbers = new int[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "SET06", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = new int[1];
			GenerateQuestion ();
		}

		bool checkArrayValues(int[] A, int[] B) {
			if (A.Length != B.Length) {
				CerebroHelper.DebugLog ("Length not equal");
				return false;
			}
			for (var i = 0; i < A.Length; i++) {
				var found = false;
				for (var j = 0; j < B.Length; j++) {
					if (A [i] == B [j]) {
						found = true;
						break;
					}
				}
				if (!found) {
					CerebroHelper.DebugLog (A[i] + " not found");
					return false;
				}
			}
			return true;
		}
		public override void SubmitClick(){
			if (ignoreTouches) {
				return;
			}
			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = true;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (level != 3) {
				string[] userAnswerSplits = userAnswerText.text.Split (new string[] { "," }, System.StringSplitOptions.None);

				List<int> userAnswers = new List<int> ();

				for (var i = 0; i < userAnswerSplits.Length; i++) {
					int userAnswer = 0;
					if (int.TryParse (userAnswerSplits [i], out userAnswer)) {
						userAnswer = int.Parse (userAnswerSplits [i]);
						userAnswers.Add (userAnswer);
					} else {
						correct = false;
						break;
					}
				}

				if (checkArrayValues (Answer, userAnswers.ToArray ())) {
					correct = true;
				} else {
					correct = false;
				}
			} else if (level == 3) {
				int ans1 = 0;
				int ans2 = 0;
				GameObject Buttons;
				if (selector == 1) {
					Buttons = EquationButtons;
				} else {
					Buttons = RangeButtons;
				}
				var button1 = Buttons.transform.Find ("Button1").GetComponent<Button> ();
				var button2 = Buttons.transform.Find ("Button2").GetComponent<Button> ();
				var ans1Text = button1.gameObject.GetChildByName<Text>("Text").text;
				var ans2Text = button2.gameObject.GetChildByName<Text>("Text").text;
				if (int.TryParse (ans1Text, out ans1)) {
					ans1 = int.Parse (ans1Text);
				} else {
					correct = false;
				}
				if (int.TryParse (ans2Text, out ans2)) {
					ans2 = int.Parse (ans2Text);
				} else {
					correct = false;
				}
				if (correct == true && ans1 == Answer [0] && ans2 == Answer [1]) {
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
			if (level != 3) {
				userAnswerText.color = MaterialColor.red800;
				Go.to (userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			} else {
				GameObject Buttons;
				if (selector == 1) {
					Buttons = EquationButtons;
				} else {
					Buttons = RangeButtons;
				}
				var button1 = Buttons.transform.Find ("Button1").GetComponent<Button> ();
				var button2 = Buttons.transform.Find ("Button2").GetComponent<Button> ();
				Go.to (button1.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
				Go.to (button2.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
				button1.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.red800;
				button2.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.red800;
			}
			yield return new WaitForSeconds (0.5f);
			if (level == 3) {
				GameObject Buttons;
				if (selector == 1) {
					Buttons = EquationButtons;
				} else {
					Buttons = RangeButtons;
				}
				var button1 = Buttons.transform.Find ("Button1").GetComponent<Button> ();
				var button2 = Buttons.transform.Find ("Button2").GetComponent<Button> ();
				if (isRevisitedQuestion) {
					button1.gameObject.GetChildByName<Text> ("Text").text = "";
					button2.gameObject.GetChildByName<Text> ("Text").text = "";
					button1.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
					button2.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
					ignoreTouches = false;
				} else {
					button1.gameObject.GetChildByName<Text> ("Text").text = Answer [0].ToString ();
					button2.gameObject.GetChildByName<Text> ("Text").text = Answer [1].ToString ();
					button1.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.green800;
					button2.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.green800;
				}
			} else {
				if (isRevisitedQuestion) {
					userAnswerText.text = "";
					userAnswerText.color = MaterialColor.textDark;
					ignoreTouches = false;
				} else {
					userAnswerText.text = "{ " + getStringFromArray (Answer) + " }";
					userAnswerText.color = MaterialColor.green800;
				}
			}

			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			if (level == 3) {
				GameObject Buttons;
				if (selector == 1) {
					Buttons = EquationButtons;
				} else {
					Buttons = RangeButtons;
				}
				var button1 = Buttons.transform.Find ("Button1").GetComponent<Button> ();
				var button2 = Buttons.transform.Find ("Button2").GetComponent<Button> ();
				button1.gameObject.GetChildByName<Text>("Text").text = Answer[0].ToString();
				button2.gameObject.GetChildByName<Text>("Text").text = Answer[1].ToString();
				button1.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.green800;
				button2.gameObject.GetChildByName<Text> ("Text").color = MaterialColor.green800;

				var config = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations( 2, GoLoopType.PingPong );
				var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
				var tween = new GoTween( button1.gameObject.transform, 0.2f, config );
				flow.insert( 0f, tween );
				flow.play ();

				var config2 = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations( 2, GoLoopType.PingPong );
				var flow2 = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
				var tween2 = new GoTween( button2.gameObject.transform, 0.2f, config2 );
				flow2.insert( 0f, tween2 );
				flow2.play ();

			} else {
				userAnswerText.text = "{ " + userAnswerText.text + " }";
				userAnswerText.color = MaterialColor.green800;

				var config = new GoTweenConfig ()
					.scale (new Vector3 (1.1f, 1.1f, 1f))
					.setIterations( 2, GoLoopType.PingPong );
				var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
				var tween = new GoTween( userAnswerText.gameObject.transform, 0.2f, config );
				flow.insert( 0f, tween );
				flow.play ();
			}

			yield return new WaitForSeconds (1f);
			if (level != 3) {
				userAnswerText.color = MaterialColor.textDark;
			}

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

			answerButton = GeneralButton;

			subQuestionText.gameObject.SetActive (false);
			subQuestionText2.gameObject.SetActive (false);
			RangeButtons.SetActive (false);
			EquationButtons.SetActive (false);
			GeneralButton.gameObject.SetActive (true);

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1) {
				var num1 = Random.Range (3, 7);
				var num2 = Random.Range (3, 7);

				int[] setA = new int[num1];
				int[] setB = new int[num2];
				int[] randNums1 = getRandomArray ();
				for (var i = 0; i < num1; i++) {
					setA [i] = randNums1 [i];
				}
				int[] randNums2 = getRandomArray ();
				for (var i = 0; i < num2; i++) {
					setB [i] = randNums2 [i];
				}
				subQuestionText.gameObject.SetActive (true);

				subQuestionText.text = "A = \\lbrace " + getStringFromArray(setA) + " \\rbrace" + " , B = \\lbrace " + getStringFromArray(setB) + " \\rbrace";

				selector = GetRandomSelector (1, 5);

				if (selector == 1) {
					QuestionLatext.text = "Find A - B";
					Answer = getDifference (setA, setB);
				} else if (selector == 2) {
					QuestionLatext.text = "Find B - A";
					Answer = getDifference (setB, setA);
				} else if (selector == 3) {
					QuestionLatext.text = "Find A \\cup B";
					Answer = getUnion (setB, setA);
				} else if (selector == 4) {
					QuestionLatext.text = "Find A \\cap B";
					Answer = getIntersection (setB, setA);
				}
			} 

			else if (level == 2) {			
				QuestionLatext.text = "Express in Roster form";
				subQuestionText.gameObject.SetActive (true);

				var num1 = Random.Range (1, 10);    //a
				var num2 = Random.Range (1, 10);	//b
				var num3 = Random.Range (1, 10);	//c
				var numberSelector = Random.Range (1, 4);  // W, N, Z
				string numberType = "";
				var lowerLimit = 0;
				var upperLimit = 0;
				var showLowerLimit = false;
				List<int> subNumbers = new List<int> ();
				if (numberSelector == 1) {
					numberType = "W";
					lowerLimit = 0;
					if (Random.Range (0, 2) == 0) {					// whether to give lower limit or not
						lowerLimit = Random.Range (0, 5);
						showLowerLimit = true;
					}
					upperLimit = Random.Range (lowerLimit, lowerLimit + 4);
				} else if (numberSelector == 2) {
					numberType = "N";
					lowerLimit = 1;
					if (Random.Range (0, 2) == 0) {					// whether to give lower limit or not
						lowerLimit = Random.Range (1, 5);
						showLowerLimit = true;
					}
					upperLimit = Random.Range (lowerLimit, lowerLimit + 4);
				} else if (numberSelector == 3) {
					numberType = "Z";
					showLowerLimit = true;

					lowerLimit = Random.Range (0, 4);
					lowerLimit = -lowerLimit;
					upperLimit = Random.Range (0, 4);
				}
				for (var i = lowerLimit; i <= upperLimit; i++) {
					subNumbers.Add (i);
				}

				selector = GetRandomSelector (1, 3);
				if (selector == 1) {
					//ax + b
					Answer = getRosterForm (new int[]{ num1, num2 }, subNumbers.ToArray ());
					if (showLowerLimit) {
						subQuestionText.text = "\\lbrace " + num1.ToString () + "x + " + num2.ToString () + "|x\\in " + numberType + ", " + lowerLimit.ToString () + "<=x<=" + upperLimit.ToString () + "\\rbrace";
					} else {
						subQuestionText.text = "\\lbrace " + num1.ToString () + "x + " + num2.ToString () + "|x\\in " + numberType + ", x<=" + upperLimit.ToString () + " \\rbrace";
					}
				} else if (selector == 2) {
					//ax2 + bx + c
					Answer = getRosterForm (new int[]{ num1, num2, num3 }, subNumbers.ToArray ());
					if (showLowerLimit) {
						subQuestionText.text = "\\lbrace " + num1.ToString () + "x^2 + " + num2.ToString () + "x + " + num3.ToString() + " |x\\in " + numberType + ", " + lowerLimit.ToString () + "<=x<=" + upperLimit.ToString () + "\\rbrace";
					} else {
						subQuestionText.text = "\\lbrace " + num1.ToString () + "x^2 + " + num2.ToString () + "x + " + num3.ToString() + " |x\\in " + numberType + ", x<=" + upperLimit.ToString () + " \\rbrace";
					}
				}
			} 

			else if (level == 3) {			
				QuestionLatext.text = "Express in set builder form";
				subQuestionText.gameObject.SetActive (true);

				var num1 = Random.Range (1, 10);    //a
				var num2 = Random.Range (1, 10);	//b
				var numberSelector = Random.Range (1, 4);  // W, N, Z
				string numberType = "";
				var lowerLimit = 0;
				var upperLimit = 0;
				List<int> subNumbers = new List<int> ();
				if (numberSelector == 1) {
					numberType = "W";
					lowerLimit = 0;
					if (Random.Range (0, 2) == 0) {					// whether to give lower limit or not
						lowerLimit = Random.Range (0, 5);
					}
					upperLimit = Random.Range (lowerLimit, lowerLimit + 4);
				} else if (numberSelector == 2) {
					numberType = "N";
					lowerLimit = 1;
					if (Random.Range (0, 2) == 0) {					// whether to give lower limit or not
						lowerLimit = Random.Range (1, 5);
					}
					upperLimit = Random.Range (lowerLimit, lowerLimit + 4);
				} else if (numberSelector == 3) {
					numberType = "Z";
					lowerLimit = Random.Range (0, 4);
					lowerLimit = -lowerLimit;
					upperLimit = Random.Range (0, 4);
				}
				for (var i = lowerLimit; i <= upperLimit; i++) {
					subNumbers.Add (i);
				}

				selector = GetRandomSelector (1, 3);
				if (selector == 1) {
					//ax + b
					RangeButtons.SetActive (false);
					EquationButtons.SetActive (true);
					GeneralButton.gameObject.SetActive (false);

					answerButton = EquationButtons.transform.Find ("Button1").GetComponent<Button> ();
					EquationButtons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").text = "";
					EquationButtons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
					EquationButtons.transform.Find ("Button1").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
					InputButtonClicked (1);

					Answer = new int[] {num1, num2};
					var question = getRosterForm (new int[]{ num1, num2 }, subNumbers.ToArray ());
					var questionString = getStringFromArray (question);
					subQuestionText.text = "\\lbrace " + questionString + " \\rbrace";
					subQuestionText2.gameObject.SetActive (true);
					subQuestionText2.text = " ? |x\\in " + numberType + ", " + lowerLimit.ToString () + "<=x<=" + upperLimit.ToString () + "\\rbrace";
				} else if (selector == 2) {
					//ranges
					RangeButtons.SetActive (true);
					EquationButtons.SetActive (false);
					GeneralButton.gameObject.SetActive (false);
					answerButton = RangeButtons.transform.Find ("Button1").GetComponent<Button> ();
					RangeButtons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").text = "";
					RangeButtons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
					RangeButtons.transform.Find ("Button1").GetComponent<Button> ().gameObject.GetChildByName<Text> ("Text").color = MaterialColor.textDark;
					InputButtonClicked (1);

					Answer = new int[] {lowerLimit, upperLimit};
					var question = getRosterForm (new int[]{ num1, num2 }, subNumbers.ToArray ());
					var questionString = getStringFromArray (question);
					subQuestionText.text = "\\lbrace " + questionString + " \\rbrace";
					subQuestionText2.gameObject.SetActive (true);
					subQuestionText2.text = "\\lbrace " + num1.ToString () + "x + " + num2.ToString () + "|x\\in " + numberType + ", ? <=x<=? \\rbrace";
				}
			} 

			CerebroHelper.DebugLog (getStringFromArray (Answer));
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
		}

		public void InputButtonClicked(int index) {
			GameObject buttons;
			if (selector == 1) {
				buttons = EquationButtons;
			} else {
				buttons = RangeButtons;
			}
			if (index == 1) {
				answerButton = buttons.transform.Find ("Button1").GetComponent<Button> ();
				buttons.transform.Find ("Button2").GetComponent<Button> ().gameObject.GetChildByName<Image> ("Image").color = new Color (1f, 1f, 1f, 1f);
			} else if (index == 2) {
				answerButton = buttons.transform.Find ("Button2").GetComponent<Button> ();
				buttons.transform.Find ("Button1").GetComponent<Button> ().gameObject.GetChildByName<Image> ("Image").color = new Color (1f, 1f, 1f, 1f);
			}
			answerButton.gameObject.GetChildByName<Image> ("Image").color = new Color (200f/255f, 220f/255f, 255f/255f, 1f);
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
		}

		int[] getRosterForm(int[] equation, int[] numbers) {
			List<int> answers = new List<int> ();
			if (equation.Length == 2) {
				for (var i = 0; i < numbers.Length ; i++) {
					answers.Add ((equation [0] * numbers [i]) + equation [1]);
				}
			} else if (equation.Length == 3) {
				for (var i = 0; i < numbers.Length ; i++) {
					answers.Add ((equation [0] * numbers [i] * numbers [i]) + (equation [1] * numbers [i]) + equation [2]);
				}
			}
			return answers.ToArray ();
		}
		int[] getRandomArray()
		{
			int[] tmpNums = numbers.Clone () as int[];
			for (int t = 0; t < tmpNums.Length; t++ )
			{
				int tmp = tmpNums[t];
				int r = Random.Range(t, tmpNums.Length);
				tmpNums[t] = tmpNums[r];
				tmpNums[r] = tmp;
			}
			return tmpNums;
		}

		int[] getDifference(int[] setA, int[] setB) {
			List<int> answer = new List<int> ();
			for (var i = 0; i < setA.Length; i++) {
				var found = false;
				for (var j = 0; j < setB.Length; j++) {
					if (setA [i] == setB [j]) {
						found = true;
						break;
					}
				}
				if (!found) {
					answer.Add (setA [i]);
				}
			}
			return answer.ToArray ();
		}

		int[] getIntersection(int[] setA, int[] setB) {
			return setA.Intersect (setB).ToArray ();
		}

		int[] getUnion(int[] setA, int[] setB) {
			return setA.Union (setB).ToArray ();
		}

		string getStringFromArray(int[] arr) {
			string str = "";
			for (var i = 0; i < arr.Length-1; i++){
				str = str + arr[i].ToString() + ",";
			}
			if (arr.Length != 0) {
				str = str + arr [arr.Length - 1].ToString ();
			}
			return str;
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
			} else if (value == 12) {   // ,
				if(checkLastTextFor(new string[1]{","})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ",";
			} else if (value == 13) {   // -
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
		}
	}
}
