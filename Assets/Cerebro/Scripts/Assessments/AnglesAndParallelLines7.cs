using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class AnglesAndParallelLines7 : BaseAssessment
	{

		public Text subQuestionText;
		public GameObject MCQ;
		private string Answer;
		private string alternateAnswer;
		private int coeff1;
		private int coeff2;
		private int coeff3;
		private int coeff4;
		private int coeff5;
		private int coeff6;
		private char deg = '˚';
		private char min = '\'';
		private char sec = '\"';

		void Start()
		{

			StartCoroutine(StartAnimation());
			base.Initialise("M", "APL07", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++)
			{
				scorestreaklvls[i] = 0;
			}

			levelUp = false;

			coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = coeff6 = 0;

			Answer = "";
			alternateAnswer = "";
			GenerateQuestion();
		}

		public override void SubmitClick()
		{
			if (ignoreTouches || userAnswerLaText.text == "")
			{
				return;
			}
			int increment = 0;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = true;
			CerebroHelper.DebugLog("!" + userAnswerLaText.text + "!");
			CerebroHelper.DebugLog("*" + Answer + "*");
			questionsAttempted++;
			updateQuestionsAttempted();
			Debug.Log ("submit");
			if (MCQ.activeSelf) 
			{
				if (Answer == userAnswerLaText.text) 
				{
					correct = true;
					StartCoroutine(AnimateMCQOption (Answer));
				}
				else
				{
					correct = false;
					AnimateMCQOptionCorrect(Answer);

				}
			}
			else
			{
				float answer = 0;
				float userAnswer = 0;
				bool directCheck = false;
				if (float.TryParse(Answer, out answer))
				{
					answer = float.Parse(Answer);
				}
				else
				{
					directCheck = true;
				}
				if (float.TryParse(alternateAnswer, out answer))
				{
					answer = float.Parse(alternateAnswer);
				}
				else
				{
					directCheck = true;
				}
				if (float.TryParse(userAnswerLaText.text, out userAnswer))
				{
					userAnswer = float.Parse(userAnswerLaText.text);
				}
				else
				{
					correct = false;
				}
				if (answer != userAnswer)
				{
					correct = false;
				}
				if (directCheck)
				{
					if (userAnswerLaText.text == Answer || userAnswerLaText.text == alternateAnswer)
					{
						correct = true;
					}
					else
					{
						correct = false;
					}
				}
			}
			if (correct == true)
			{
				if (Queslevel == 1)
				{
					increment = 5;
				}
				else if (Queslevel == 2)
				{
					increment = 10;
				}
				else if (Queslevel == 3)
				{
					increment = 15;
				}
				else if (Queslevel == 4)
				{
					increment = 15;
				}


				UpdateStreak(5, 12);

				updateQuestionsAttempted();
				StartCoroutine(ShowCorrectAnimation());
			}
			else
			{
				for (var i = 0; i < scorestreaklvls.Length; i++)
				{
					scorestreaklvls[i] = 0;
				}
				StartCoroutine(ShowWrongAnimation());
			}

			base.QuestionEnded(correct, level, increment, selector);

		}

		protected override IEnumerator ShowWrongAnimation()
		{
			userAnswerLaText.color = MaterialColor.red800;
			Go.to(userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
			yield return new WaitForSeconds(0.5f);
			if (isRevisitedQuestion)
			{
				if (numPad.activeSelf)
				{               // is not MCQ type question
					userAnswerLaText.text = "";
				}
				if (userAnswerLaText != null)
				{
					userAnswerLaText.color = MaterialColor.textDark;
				}
				if (userAnswerLaText != null)
				{
					userAnswerLaText.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
			}
			else
			{
				if (numPad.activeSelf)
				{               // is not MCQ type question
					CerebroHelper.DebugLog("going in if");
					userAnswerLaText.text = Answer.ToString();
					userAnswerLaText.color = MaterialColor.green800;
				}
				else
				{
					CerebroHelper.DebugLog("going in else");
					userAnswerLaText.color = MaterialColor.textDark;
				}
			}

			ShowContinueButton();
		}

		protected override IEnumerator ShowCorrectAnimation()
		{
			userAnswerLaText.color = MaterialColor.green800;
			var config = new GoTweenConfig()
				.scale(new Vector3(1.1f, 1.1f, 0))
				.setIterations(2, GoLoopType.PingPong);
			var flow = new GoTweenFlow(new GoTweenCollectionConfig().setIterations(1));
			var tween = new GoTween(userAnswerLaText.gameObject.transform, 0.2f, config);
			flow.insert(0f, tween);
			flow.play();
			yield return new WaitForSeconds(1f);
			userAnswerLaText.color = MaterialColor.textDark;

			showNextQuestion();

			if (levelUp)
			{
				StartCoroutine(HideAnimation());
				base.LevelUp();
				yield return new WaitForSeconds(1.5f);
				StartCoroutine(StartAnimation());
			}

		}

		void AnimateMCQOptionCorrect(string ans)
		{
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
		}

		public void MCQOptionClicked (int value)
		{
			if (ignoreTouches) {
				return;
			}
			userAnswerLaText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<TEXDraw> ();
			answerButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
			SubmitClick ();
		}

		IEnumerator AnimateMCQOption (string ans)
		{
			var GO = new GameObject();
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					GO = MCQ.transform.Find ("Option" + i.ToString ()).gameObject;
				}
			}
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
		}

		void RandomizeMCQOptionsAndFill(List<string> options)
		{
			int index = 0;
			int cnt = options.Count;
			for (int i = 1; i <= cnt; i++) {
				index = Random.Range (0, options.Count);
				MCQ.transform.Find ("Option"+i).Find ("Text").GetComponent<TEXDraw> ().text = options [index];
				options.RemoveAt (index);
			}
		}

		protected override void GenerateQuestion()
		{
			ignoreTouches = false;
			base.QuestionStarted();
			// Generating the parameters

			level = Queslevel;

			answerButton = GeneralButton;
			subQuestionText.gameObject.SetActive(false);
			QuestionText.gameObject.SetActive(false);
			QuestionLatext.gameObject.SetActive(true);
			GeneralButton.gameObject.SetActive(true);
			numPad.SetActive(true);
			MCQ.SetActive (false);
			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}

			if (Queslevel > scorestreaklvls.Length)
			{
				level = UnityEngine.Random.Range(1, scorestreaklvls.Length + 1);
			}


			#region level1
			if (level == 1)
			{
				selector = GetRandomSelector(1, 6);
				if (selector == 1)
				{
					coeff1 = Random.Range(2, 7) * Random.Range(2, 7);
					QuestionLatext.text = "Calculate the measure of 1/" + coeff1 + " of a right angle.";
					float ans = 90f/(float)coeff1;
					alternateAnswer = ans.ToString() + deg;
					Answer = "" + Mathf.Floor(ans) + deg;
					ans -= Mathf.Floor(ans);
					ans *= 60f;
					if (ans > 0.0001)
					{
						ans = MathFunctions.GetRounded (ans, 3);
						Answer += "" + Mathf.Floor(ans) + min;
						ans -= Mathf.Floor(ans);
						if (ans > 0.001){
							ans *= 60f;
							Answer += "" + ans + sec;
						}
					}
				}
				else if (selector == 2)
				{
					coeff1 = 10 * Random.Range(2,36);
					if (coeff1 == 180 || coeff1 >= 270)
						coeff1 = 90;
					QuestionLatext.text = "If \\angle{A} = " + coeff1 + deg + ", what type of angle is \\angle{A}?";
					MCQ.SetActive (true);
					GeneralButton.gameObject.SetActive(false);
					numPad.SetActive(false);
					List<string> options = new List<string>();
					options.Add("Acute");
					options.Add("Obtuse");
					options.Add("Right");
					options.Add("Reflex");
					if (coeff1 < 90)
						Answer = options[0];
					else if (coeff1 == 90)
						Answer = options[2];
					else if (coeff1 > 90 && coeff1 < 180)
						Answer = options[1];
					else
						Answer = options[3];
					RandomizeMCQOptionsAndFill(options);
				}
				else if (selector == 3)
				{
					QuestionLatext.text = "Points A, B and C lie in a straight line. What is the measure of \\angle{ABC}?";
					Answer = "180" + deg;
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range(1, 50);
					coeff2 = Random.Range(10, 60);
					coeff3 = Random.Range(10, 60);
					coeff4 = Random.Range(1, 50);
					coeff5 = Random.Range(10, 60);
					coeff6 = Random.Range(10, 60);
					QuestionLatext.text = "If \\angle{A} = " + coeff1 + deg + coeff2 + min + coeff3 + sec + " and \\angle{B} = " + coeff4 + deg + coeff5 + min + coeff6 + sec + ", find 2\\angle{A} + \\angle{B}.";
					int anssec, ansmin, ansdeg;
					anssec = 2 * coeff3 + coeff6;
					ansmin = 2 * coeff2 + coeff5;
					ansdeg = 2 * coeff1 + coeff4;
					while (anssec >= 60){
						anssec -= 60;
						ansmin++;
					}
					while (ansmin >=  60){
						ansmin -= 60;
						ansdeg++;
					}
					Answer = "" + ansdeg + deg + ansmin + min + anssec + sec;
				}
				else if (selector == 5)
				{
					QuestionLatext.text = "Which of the following are not equal?";
					MCQ.SetActive (true);
					GeneralButton.gameObject.SetActive(false);
					numPad.SetActive(false);
					List<string> options = new List<string>();
					options.Add("Vertically opposite angles");
					options.Add("Alternate angles");
					options.Add("Corresponding angles");
					options.Add("Co-interior angles");
					Answer = options[3];
					RandomizeMCQOptionsAndFill(options);
				}
			}
			#endregion
			#region level2

			if (level == 2)
			{
				selector = GetRandomSelector(1, 5);

				if(selector==1)
				{
					coeff1 = 10 * Random.Range(20, 50);
					QuestionLatext.text = "An angle measures " + coeff1 + min + ". What is its measure in degrees and minutes?";
					int ansdeg = coeff1 / 60;
					int ansmin = coeff1 % 60;
					if (ansmin == 0)
						alternateAnswer = "" + ansdeg + deg;
					Answer = "" + ansdeg + deg + ansmin + min;
				}
				else if(selector==2)
				{
					coeff1 = Random.Range (2, 10);
					while (coeff1 == 6)
						coeff1 = Random.Range (2, 10);
					if (Random.Range (1,3) == 1)
						coeff1 = 2 * Random.Range (1,3) * coeff1 - 1;
					QuestionLatext.text = "The complement of an angle is 1/" + coeff1 + " of itself. What is the measure of the angle?";
					float ans = (float)(coeff1 * 90)/(float)(coeff1 + 1);
					alternateAnswer = ans.ToString() + deg;
					Answer = "" + Mathf.Floor(ans) + deg;
					ans -= Mathf.Floor(ans);
					ans *= 60f;
					if (ans > 0.0001)
					{
						ans = MathFunctions.GetRounded (ans, 3);
						Answer += "" + Mathf.Floor(ans) + min;
						ans -= Mathf.Floor(ans);
						if (ans > 0.001){
							ans *= 60f;
							Answer += "" + ans + sec;
						}
					}
				}
				else if (selector == 3)
				{
					coeff1 = Random.Range(2,9) * Random.Range(2,6);
					coeff2 = 48 - coeff1;
					if (coeff1 == coeff2){
						coeff1++;
						coeff2--;
					}
					int hcf = MathFunctions.GetHCF(coeff1, coeff2);
					coeff1 /= hcf;
					coeff2 /= hcf;
					QuestionLatext.text = "Two supplementary angles are in the ratio " + coeff1 + " : " + coeff2 + ". Calculate the measure of the smaller angle.";
					float ans = (float)(coeff1 * 180f)/(float)(coeff1 + coeff2);
					if (coeff1 > coeff2)
						ans = 180f - ans;
					alternateAnswer = "" + ans + deg;
					Answer = "" + Mathf.Floor(ans) + deg;
					ans -= Mathf.Floor(ans);
					ans *= 60f;
					if (ans > 0.0001)
					{
						ans = MathFunctions.GetRounded (ans, 3);
						Answer += "" + Mathf.Floor(ans) + min;
						ans -= Mathf.Floor(ans);
						if (ans > 0.001){
							ans *= 60f;
							Answer += "" + ans + sec;
						}
					}
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range(91,151);
					while ((((coeff1 - 90) * 200 * 40) % (180 - coeff1)) != 0)
						coeff1 = Random.Range(91,151);
					float percent = (float)((coeff1 - 90) * 200) / (float)(180 - coeff1); 
					QuestionLatext.text = "An angle is " + percent + "% more than its supplement. Calculate its measure.";
					float ans = coeff1;
					alternateAnswer = "" + ans + deg;
					Answer = "" + Mathf.Floor(ans) + deg;
					ans -= Mathf.Floor(ans);
					ans *= 60f;
					if (ans > 0.0001)
					{
						ans = MathFunctions.GetRounded (ans, 3);
						Answer += "" + Mathf.Floor(ans) + min;
						ans -= Mathf.Floor(ans);
						if (ans > 0.001){
							ans *= 60f;
							Answer += "" + ans + sec;
						}
					}
				}
			}
			#endregion
			#region level3
			if (level == 3)
			{

				selector = GetRandomSelector(1, 3);

				if (selector == 1)
				{
					coeff1 = Random.Range(10, 61);
					coeff2 = Random.Range(50, 101);
					coeff3 = Random.Range(3, 11);
					while ((40 * (coeff2 - coeff1) % (coeff3 - 1)) != 0 || (coeff2 < (coeff1 + 10)))
						coeff1 = Random.Range(10, 61);
					QuestionLatext.text = "Two corresponding angles measure " + coeff3 + "x + " + coeff1 + deg + " and x + " + coeff2 + deg + ". Find x.";
					float ans = (float)(coeff2 - coeff1) / (float)(coeff3 - 1);
					alternateAnswer = "" + ans + deg;
					Answer = "" + Mathf.Floor(ans) + deg;
					ans -= Mathf.Floor(ans);
					ans *= 60f;
					if (ans > 0.0001)
					{
						ans = MathFunctions.GetRounded (ans, 3);
						Answer += "" + Mathf.Floor(ans) + min;
						ans -= Mathf.Floor(ans);
						if (ans > 0.001){
							ans *= 60f;
							Answer += "" + ans + sec;
						}
					}
				}
				else if (selector == 2)
				{
					coeff2 = Random.Range(10, 150);
					coeff3 = Random.Range(2, 11);
					while ((40 * (90 + coeff2)) % (coeff3 + 1) != 0)
						coeff2 = Random.Range(10, 150);
					QuestionLatext.text = coeff3 + " times an angle is " + coeff2 + deg + " more than its complement. Find the measure of its supplementary angle.";
					float ans = (float)(90 + coeff2)/(float)(coeff3 + 1);
					ans = 180f - ans;
					alternateAnswer = "" + ans + deg;
					Answer = "" + Mathf.Floor(ans) + deg;
					ans -= Mathf.Floor(ans);
					ans *= 60f;
					if (ans > 0.0001)
					{
						ans = MathFunctions.GetRounded (ans, 3);
						Answer += "" + Mathf.Floor(ans) + min;
						ans -= Mathf.Floor(ans);
						if (ans > 0.001){
							ans *= 60f;
							Answer += "" + ans + sec;
						}
					}
				}
			}
			#endregion
			userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw>("Text");
			userAnswerLaText.text = "";
		}

		public override void numPadButtonPressed(int value)
		{
			if (ignoreTouches)
			{
				return;
			}
			if (value <= 9)
			{
				userAnswerLaText.text += value.ToString();
			}
			else if (value == 10)
			{    //.
				if (checkLastTextFor(new string[1] { "." }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ".";
			}
			else if (value == 11)
			{   // All Clear
				if (userAnswerLaText.text.Length > 0)
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
			}
			else if (value == 12)
			{   // All Clear
				if (!userAnswerLaText.text.Contains("" + min))
				{
					userAnswerLaText.text += "" + min;
				}				
			}
			else if (value == 13)
			{   // All Clear
				if (!userAnswerLaText.text.Contains("" + sec))
				{
					userAnswerLaText.text += "" + sec;
				}
			}
			else if (value == 14)
			{   // All Clear
				if (!userAnswerLaText.text.Contains("" + deg))
				{
					userAnswerLaText.text += "" + deg;
				}
			}
		}
			
	}
}

