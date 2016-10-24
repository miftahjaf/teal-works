using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class Graphs7 : BaseAssessment {

		private string Answer;
		private List<string> AnswerList;
		private string[] quadrants = new string[] {"first", "second", "third", "fourth"};
		private int randSelector;
		private int xCord, yCord;

		public TEXDraw subQuestionTEX;
		public GameObject MCQ;
		public GraphHelper graphHelper;

		void Start () {

			base.Initialise ("M", "GRA07", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;


			GenerateQuestion ();
		
		}

		public override void SubmitClick(){
			if (ignoreTouches || userAnswerText.text == "") {
				return;
			}
			int increment = 0;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (MCQ.activeSelf) 
			{
				if (Answer == userAnswerText.text) {
					correct = true;
				} else {
					correct = false;
					AnimateMCQOptionCorrect (Answer);
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

				if (float.TryParse(userAnswerText.text, out userAnswer))
				{
					userAnswer = float.Parse(userAnswerText.text);
				}
				else
				{
					directCheck = true;
				}

				if (answer != userAnswer)
				{
					correct = false;
				}
				if (directCheck)
				{
					if (userAnswerText.text == Answer || AnswerList.Contains (userAnswerText.text)) 
					{
						correct = true;
					}
					else
					{
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
				} 
					
				UpdateStreak(8, 12);

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
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.green800;
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
			userAnswerText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<Text> ();
			answerButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
			SubmitClick ();
		}

		protected override IEnumerator ShowWrongAnimation()
		{
			userAnswerText.color = MaterialColor.red800;
			Go.to(userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
			yield return new WaitForSeconds(0.5f);
			if (isRevisitedQuestion)
			{
				if (numPad.activeSelf)
				{               // is not MCQ type question
					userAnswerText.text = "";
				}
				if (userAnswerText != null)
				{
					userAnswerText.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
			}
			else
			{
				if (numPad.activeSelf)
				{               // is not MCQ type question
					userAnswerText.text = Answer.ToString();
					userAnswerText.color = MaterialColor.green800;
				}
				else
				{
					CerebroHelper.DebugLog("going in else");
					userAnswerText.color = MaterialColor.textDark;
				}
			}
			ShowContinueButton();
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

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			level = Queslevel;
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
			subQuestionTEX.gameObject.SetActive (false);
			QuestionLatext.gameObject.SetActive (false);
			//SetNumpadMode ();
			numPad.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (false);
			AnswerList = new List<string> ();
			graphHelper.Reset ();
			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) //788    //highlight quadrant
				{
					randSelector = Random.Range (0, 4);
					Answer = quadrants[randSelector];
					QuestionText.text = string.Format ("Identify the {0} quadrant.", Answer);

					graphHelper.SetGridParameters(new Vector2(20,20),25);
					graphHelper.DrawGraph();


				}
				else if (selector == 2) // 788   //highlight axis
				{
					randSelector = Random.Range (0, 2);
					if (randSelector == 0)
						Answer = "x";
					else
						Answer = "y";
					QuestionText.text = string.Format ("Identify the {0}-axis.", Answer);
				}
				else if (selector == 3)
				{
					randSelector = Random.Range (0, 4);
					xCord = Random.Range (1, 6);
					yCord = Random.Range (1, 6);
					Answer = quadrants[randSelector];

					if (Answer == "second")
						xCord *= -1;
					else if (Answer == "fourth")
						yCord *= -1;
					else if (Answer == "third")
					{
						xCord *= -1;
						yCord *= -1;
					}

					QuestionText.text = string.Format ("Identify the quadrant in which the point ({0}, {1}) lies.", xCord, yCord);
				}
				else if (selector == 4)
				{
					randSelector = Random.Range (0, 2);
					xCord = Random.Range (-5, 6);
					yCord = Random.Range (-5, 6);

					while (xCord == yCord)
						yCord = Random.Range (-5, 6);
					
					Answer = (Random.Range (0, 2) == 0 ? xCord : yCord).ToString ();

					QuestionText.text = string.Format ("What is the {2} of the point ({0}, {1})?", xCord, yCord, (Answer == xCord.ToString() ? "abscissa" : "ordinate"));
				}
				else if (selector == 5)
				{
					xCord = Random.Range (-5, 6);
					yCord = Random.Range (-5, 6);

					QuestionText.text = string.Format ("Plot the point ({0}, {1}) on the given graph.", xCord, yCord);
					Answer = string.Format ("({0}, {1})", xCord, yCord);
				}
			}
			#endregion
			CerebroHelper.DebugLog (Answer);
			userAnswerText = GeneralButton.gameObject.GetChildByName<Text>("Text");
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
			} else if (value >= 12 && value <= 19) {
				userAnswerText.text += ((char)(53 + value)).ToString ();
			} else if (value == 20) {
				userAnswerText.text += "O";
			} else if (value == 21) {
				userAnswerText.text += ".";
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
		}
	}
}