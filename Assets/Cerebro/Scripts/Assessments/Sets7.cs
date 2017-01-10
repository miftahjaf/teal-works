using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;


namespace Cerebro {
	public class Sets7 : BaseAssessment {

		public TEXDraw subQuestionText;
		public TEXDraw subQuestionText2;
		public GameObject MCQ;

		private int[] answer;
		private string Answer;
		private bool answerTypeSet, answerTypePowerSet;

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "SET07", "S01", "A01");

			scorestreaklvls = new int[2];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			answer = new int[1];
			GenerateQuestion ();
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

			if (MCQ.activeSelf) {
				if (Answer == userAnswerLaText.text) {
					correct = true;
				} else {
					correct = false;
					AnimateMCQOptionCorrect (Answer);
				}
			} else if (answerTypeSet) {
				correct = MathFunctions.checkSets (userAnswerLaText.text, Answer, true);
			} else if (answerTypePowerSet) {
				correct = MathFunctions.checkPowerSets (userAnswerLaText.text, Answer);
			} else {
				int userAns = 0;
				if (int.TryParse (userAnswerLaText.text, out userAns)) {
					userAns = int.Parse (userAnswerLaText.text);
				}

				if (userAns == int.Parse (Answer)) {
					correct = true;
				} else {
					correct = false;
				}
			}
			if (correct == true) {
				if (Queslevel == 1) {
					increment = 10;
				} else if (Queslevel == 2) {
					increment = 10;
				} else if (Queslevel == 3) {
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

		void AnimateMCQOptionCorrect(string ans)
		{
			if (isRevisitedQuestion) {
				return;
			}
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
		}
		IEnumerator AnimateMCQOption (int value)
		{
			var GO = MCQ.transform.Find ("Option" + value.ToString ()).gameObject;
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
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

		protected override IEnumerator ShowWrongAnimation() {
			userAnswerLaText.color = MaterialColor.red800;
			Go.to (userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				if (numPad.activeSelf) {
					userAnswerLaText.text = "";
				} 
				if (userAnswerLaText != null) {
					userAnswerLaText.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
			} else {
				if (numPad.activeSelf) {
					userAnswerLaText.text = Answer;
					userAnswerLaText.color = MaterialColor.green800;
				} else {
					userAnswerLaText.color = MaterialColor.textDark;
				}
			}

			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			userAnswerLaText.color = MaterialColor.green800;

			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations( 2, GoLoopType.PingPong );
			var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
			var tween = new GoTween( userAnswerLaText.gameObject.transform, 0.2f, config );
			flow.insert( 0f, tween );
			flow.play ();

			yield return new WaitForSeconds (1f);
			userAnswerLaText.color = MaterialColor.textDark;

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

			answerTypeSet = false;
			answerTypePowerSet = false;
			subQuestionText.gameObject.SetActive (true);
			subQuestionText2.gameObject.SetActive (true);
			SetNumpadMode ();

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			#region level1
			if (level == 1)
			{
				selector = GetRandomSelector (1, 7);
				if (selector <= 3) {
					answerTypeSet = true;
				}
				if (selector == 1)
				{
					char randAlpha = (char)(65 + Random.Range (0, 26));
					List<int> universalSet = MathFunctions.GetUniqueIntRandomDataSet (1, 16, Random.Range (5, 11));
					List<int> dataSet = MathFunctions.GetSubset (universalSet, universalSet.Count / 2);

					QuestionLatext.text = string.Format ("Find {0}^{{\\prime}}.", randAlpha);
					subQuestionText.text = string.Format ("\\xi = {0}", MathFunctions.getArrayAsSet (universalSet.ToArray (), true));
					subQuestionText2.text = string.Format ("{0} = {1}", randAlpha, MathFunctions.getArrayAsSet (dataSet.ToArray (), true));

					answer = MathFunctions.getDifference (universalSet.ToArray (), dataSet.ToArray ());
					Answer = MathFunctions.getArrayAsSet (answer, true, true);
				}
				else if (selector == 2)
				{
					char randAlpha = (char)(65 + Random.Range (0, 26));
					int rangeMin = Random.Range (10, 20);
					int rangeMax = Random.Range (rangeMin * 2, (int)(rangeMin * 2.5f));

					List<int> dataSet = MathFunctions.GetPrimes (rangeMin, rangeMax, false);

					QuestionLatext.text = string.Format ("Find {0}^{{\\prime}}.", randAlpha);
					subQuestionText.text = string.Format ("{0} = \\lbrace{{\\xalgebra | {1}}}\\rbrace", "\\xi", GetInequality (rangeMin, rangeMax, 1));
					subQuestionText2.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra is not a prime, {1}}}\\rbrace", randAlpha, GetInequality (rangeMin, rangeMax, MathFunctions.isPrime (rangeMin)? 1: Random.Range (0, 2)));

					answer = dataSet.ToArray ();
					Answer = MathFunctions.getArrayAsSet (answer, true, true);
				}
				else if (selector == 3)
				{
					char randAlpha = (char)(65 + Random.Range (0, 26));
					int rangeMin = Random.Range (10, 20);
					int rangeMax = Random.Range (rangeMin + 5, rangeMin + 11);

					List<int> dataSet = new List<int> ();
					string OddEven = Random.Range (0, 2) == 0? "odd": "even";

					if (OddEven.Equals ("odd")) {
						for (int i = rangeMin; i < rangeMax; i++) {
							if (i % 2 == 0) {
								dataSet.Add (i);
							}
						}
					} else {
						for (int i = rangeMin; i < rangeMax; i++) {
							if (i % 2 != 0) {
								dataSet.Add (i);
							}
						}
					}

					QuestionLatext.text = string.Format ("Find {0}^{{\\prime}}.", randAlpha);
					subQuestionText.text = string.Format ("{0} = \\lbrace{{\\xalgebra | {1}}}\\rbrace", "\\xi", GetInequality (rangeMin, rangeMax, 1), OddEven);
					subQuestionText2.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra is {2}, {1}}}\\rbrace", randAlpha, GetInequality (rangeMin, rangeMax, 1), OddEven);

					answer = dataSet.ToArray ();
					Answer = MathFunctions.getArrayAsSet (answer, true, true);
				}
				else if (selector == 4)
				{
					SetMCQMode (2);

					char randAlpha1 = (char)(65 + Random.Range (0, 26));
					char randAlpha2 = (char)(65 + Random.Range (0, 26));
					while (randAlpha1 == randAlpha2) {
						randAlpha2 = (char)(65 + Random.Range (0, 26));
					}

					int rangeMin = Random.Range (0, 10);
					int rangeMax = Random.Range (rangeMin + 5, rangeMin + 11);
					int randSelector = Random.Range (0, 2);

					QuestionLatext.text = "Chose the correct option.";
					if (randSelector == 0) 
					{
						subQuestionText.text = string.Format ("{0} = \\lbrace{{\\xalgebra | {1}, \\xalgebra \\in N}}\\rbrace", randAlpha1, GetInequality (rangeMin, rangeMax, Random.Range (0, 4)));
						subQuestionText2.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra < {1}, \\xalgebra \\in W}}\\rbrace", randAlpha2, rangeMax + Random.Range (1, 10));
					} 
					else if (randSelector == 1) 
					{
						subQuestionText2.text = string.Format ("{0} = \\lbrace{{\\xalgebra | {1}, \\xalgebra \\in N}}\\rbrace", randAlpha1, GetInequality (rangeMin, rangeMax, Random.Range (0, 4)));
						subQuestionText.text = string.Format ("{0} = \\lbrace{{\\xalgebra | \\xalgebra < {1}, \\xalgebra \\in W}}\\rbrace", randAlpha2, rangeMax + Random.Range (1, 10));
					} 

					randSelector = Random.Range (0, 4);
					if (randSelector == 0) 
					{
						MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = string.Format ("{0} \\subset {1}", randAlpha1, randAlpha2);
						MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = string.Format ("{0} \\supset {1}", randAlpha1, randAlpha2);
						Answer = string.Format ("{0} \\subset {1}", randAlpha1, randAlpha2);
					} 
					else if (randSelector == 1) 
					{
						MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = string.Format ("{0} \\subseteq {1}", randAlpha1, randAlpha2);
						MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = string.Format ("{0} \\supset {1}", randAlpha1, randAlpha2);
						Answer = string.Format ("{0} \\subseteq {1}", randAlpha1, randAlpha2);
					}
					else if (randSelector == 2) 
					{
						MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = string.Format ("{0} \\supseteq {1}", randAlpha2, randAlpha1);
						MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = string.Format ("{0} \\subseteq {1}", randAlpha2, randAlpha1);
						Answer = string.Format ("{0} \\supseteq {1}", randAlpha1, randAlpha2);
					}
					else if (randSelector == 3) 
					{
						MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = string.Format ("{0} \\supset {1}", randAlpha2, randAlpha1);
						MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = string.Format ("{0} \\subseteq {1}", randAlpha2, randAlpha1);
						Answer = string.Format ("{0} \\supset {1}", randAlpha1, randAlpha2);
					}
				}
				else if (selector == 5)
				{
					subQuestionText2.gameObject.SetActive (false);

					char randAlpha = (char)(65 + Random.Range (0, 26));
					int cardinalNumber = Random.Range (2, 7);
					List<int> dataSet = MathFunctions.GetUniqueIntRandomDataSet (1, 10, cardinalNumber);

					QuestionLatext.text = string.Format ("How many subsets does the set {0} have?", randAlpha);
					subQuestionText.text = string.Format ("{0} = {1}", randAlpha, MathFunctions.getArrayAsSet (dataSet.ToArray (), true));

					Answer = string.Format ("{0}", Mathf.RoundToInt (Mathf.Pow (2, cardinalNumber)));
				}
				else if (selector == 6)
				{
					subQuestionText2.gameObject.SetActive (false);
					answerTypePowerSet = true;

					char randAlpha = (char)(65 + Random.Range (0, 26));
					int cardinalNumber = Random.Range (2, 4);
					List<int> dataSet = MathFunctions.GetUniqueIntRandomDataSet (1, 10, cardinalNumber);

					QuestionLatext.text = string.Format ("Write the power set of {0}.", randAlpha);
					subQuestionText.text = string.Format ("{0} = {1}", randAlpha, MathFunctions.getArrayAsSet (dataSet.ToArray (), true));
					List<List<int>> PowerSet = MathFunctions.GetPowerSet (dataSet);

					Answer = "\\lbrace{";
					for (int i = 0; i < PowerSet.Count; i++) {
						Answer += MathFunctions.getArrayAsSet (PowerSet[i].ToArray (), true, true) + ",";
					}
					Answer = Answer.Substring (0, Answer.Length - 1) + "}\\rbrace";
				}
			}
			#endregion
			#region level2
			else if (level == 2)
			{
				selector = GetRandomSelector (1, 6);
				subQuestionText2.gameObject.SetActive (false);
				StartSequence (3);

				int nVolley = Random.Range (2, 11);
				int nBadminton = Random.Range (2, 11);
				int nBoth = Random.Range (2, 6);
				int nNone = Random.Range (2, 6);
				int total = nVolley + nBadminton + nBoth + nNone;

				QuestionLatext.text = string.Format ("In a group of {0} students, {1} play Volleyball, {2} play Badminton and {3} play neither sport. Consider the set V represents Volleyball players and the set B represents Badminton players.", total, nVolley + nBoth, nBadminton + nBoth, nNone);

				if (selector == 1) 
				{
					subQuestionText.text = "\n\nHow many play both the games?";
					Answer = string.Format ("{0}", nBoth);
				}
				else if (selector == 2)
				{
					int randSelector = Random.Range (0, 2);
					subQuestionText.text = string.Format ("\n\nFind the number of students who play only {0}.", randSelector == 0? "Volleyball": "Badminton");
					Answer = string.Format ("{0}", randSelector == 0? nVolley: nBadminton);
				}
				else if (selector == 3)
				{
					int randSelector = Random.Range (0, 2);
					subQuestionText.text = string.Format ("\n\nCalculate \\nalgebra({0}).", randSelector == 0? "B - V": "V - B");
					Answer = string.Format ("{0}", randSelector == 0? nBadminton: nVolley);
				}
				else if (selector == 4)
				{
					int randSelector = Random.Range (0, 2);
					subQuestionText.text = string.Format ("\n\nCalculate \\nalgebra({0}^{{\\prime}}).", randSelector == 0? "V": "B");
					Answer = string.Format ("{0}", randSelector == 0? nBadminton + nNone: nVolley + nNone);
				}
				else if (selector == 5)
				{
					int randSelector = Random.Range (0, 2);
					subQuestionText.text = string.Format ("\n\nCalculate \\nalgebra({0}^{{\\prime}}).", randSelector == 0? "V \\cup B": "V \\cap B");
					Answer = string.Format ("{0}", randSelector == 0? nNone: total - nBoth);
				}
			}
			#endregion

			Debug.Log (Answer);
			userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw>("Text");
			userAnswerLaText.text = "";
		}

		string GetInequality (int rangeMin, int rangeMax, int Case)
		{
			string inequality = rangeMin.ToString ();

			if (Case == 0) {
				inequality += " < \\xalgebra < " + rangeMax;
			} else if (Case == 1) {
				inequality += " \\leq \\xalgebra < " + rangeMax;
			} else if (Case == 2) {
				inequality += " \\leq \\xalgebra \\leq " + rangeMax;
			} else if (Case == 3) {
				inequality += " < \\xalgebra \\leq " + rangeMax;
			}
			return inequality;
		}

		public override void numPadButtonPressed(int value) {
			if (ignoreTouches) {
				return;
			}
			if (value <= 9) {
				userAnswerLaText.text += value.ToString ();
			} else if (value == 10) {    //Back
				if (checkLastTextFor (new string[1]{"}\\rbrace"}) || checkLastTextFor (new string[1]{"\\lbrace{"}))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 8);
				}
				else if (userAnswerLaText.text.Length > 0) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
			} else if (value == 11) {   // All Clear
				userAnswerLaText.text = "";
			} else if (value == 12) {   //} 
				userAnswerLaText.text += "}\\rbrace";
			} else if (value == 13) {   //{
				userAnswerLaText.text += "\\lbrace{";
			} else if (value == 14) {   // ,
				if(checkLastTextFor(new string[1]{","})) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ",";
			} else if (value == 15) {   // -
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += "-";
			}
		}

		protected void SetMCQMode (int NumberOfMCQ = 4)
		{
			this.MCQ.SetActive (true);
			Vector2[] positions;

			if (NumberOfMCQ == 3) 
			{
				positions = new Vector2[] {
					new Vector2 (-255, 40f),
					new Vector2 (0, 40f),
					new Vector2 (255, 40f),
					new Vector2 (0, 0f)
				};	
			} 
			else if (NumberOfMCQ == 2) {
				positions = new Vector2[] {
					new Vector2 (-180, 40f),
					new Vector2 (180, 40f),
					new Vector2 (-180, 0f),
					new Vector2 (180, 0f)
				};
			}
			else 
			{
				positions = new Vector2[] {
					new Vector2 (-180, 80f),
					new Vector2 (180, 80f),
					new Vector2 (-180, 0f),
					new Vector2 (180, 0f)
				};
			}

			for (int i = 1; i <= 4; i++)
			{
				MCQ.transform.Find ("Option" + i).gameObject.SetActive (i<=NumberOfMCQ);
				MCQ.transform.Find ("Option" + i).GetComponent<RectTransform> ().anchoredPosition = positions[i-1];
			}
			this.MCQ.SetActive (true);
			this.numPad.SetActive (false);
			this.GeneralButton.gameObject.SetActive (false);

			for (int i = 1; i <= 4; i++) {
				if(MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ())
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
				else
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}
		}

		protected void SetNumpadMode ()
		{
			this.numPad.SetActive (true);
			this.MCQ.SetActive (false);
			this.GeneralButton.gameObject.SetActive (true);
			ContinueBtn.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,ContinueBtn.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
			FlagButton.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,FlagButton.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
			SolutionButton.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,SolutionButton.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
		}
	}
}
