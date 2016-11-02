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
		private int xCord, yCord, xCord1, yCord1, xCord2, yCord2, slope, intercept;

		public TEXDraw subQuestionTEX;
		public GameObject MCQ;
		public GraphHelper graphHelper;
		public GameObject CheckButton;
		public GameObject numPadBg;
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
			if (ignoreTouches || (userAnswerText.text == "" && graphHelper.graphQuesType == GraphQuesType.None)) {
				return;
			}
			int increment = 0;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;

			if (graphHelper.graphQuesType == GraphQuesType.None) {
				
				questionsAttempted++;
				updateQuestionsAttempted ();

				if (MCQ.activeSelf) {
					if (Answer == userAnswerText.text) {
						correct = true;
					} else {
						correct = false;
						AnimateMCQOptionCorrect (Answer);
					}
				} else {
					float answer = 0;
					float userAnswer = 0;
					bool directCheck = false;
					if (float.TryParse (Answer, out answer)) {
						answer = float.Parse (Answer);
					} else {
						directCheck = true;
					}

					if (float.TryParse (userAnswerText.text, out userAnswer)) {
						userAnswer = float.Parse (userAnswerText.text);
					} else {
						directCheck = true;
					}


					if (directCheck) {
						if (userAnswerText.text == Answer || AnswerList.Contains (userAnswerText.text)) {
							correct = true;
						} else {
							correct = false;
						}
					} else {
						correct = (answer == userAnswer);
					}
				} 
			} 
			else
			{
				correct = graphHelper.CheckAnswer ();
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
			graphHelper.HandleIncorrectAnwer ();
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
			this.CheckButton.gameObject.SetActive(false);
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
			graphHelper.HandleCorrectAnswer ();
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
		//	QuestionLatext.gameObject.SetActive (false);
			SetGraphMode ();


			AnswerList = new List<string> ();
			graphHelper.Reset ();
			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 7);
				if (selector == 1) //788    //highlight quadrant
				{
					randSelector = Random.Range (0, 4);
					Answer = quadrants[randSelector];
					QuestionText.text = string.Format ("Identify the {0} quadrant.", Answer);
					graphHelper.SetGridParameters(new Vector2(20,20),22);
					graphHelper.SetCurrentCorrectQuadrant(randSelector+1);
					graphHelper.SetGraphQuesType(GraphQuesType.HighlightQuadrant);
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

					graphHelper.SetGridParameters(new Vector2(20,20),22);
					graphHelper.SetGraphQuesType(GraphQuesType.HighlightAxis);
					graphHelper.SetCurrentCorrectAxis(randSelector);
					graphHelper.DrawGraph();

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
					graphHelper.SetGridParameters(new Vector2(20,20),22);
					graphHelper.SetCurrentCorrectQuadrant(randSelector+1);
					graphHelper.SetGraphQuesType(GraphQuesType.HighlightQuadrant);
					graphHelper.DrawGraph();
				}
				else if (selector == 4)
				{
					SetNumpadMode();
					randSelector = Random.Range (0, 2);
					xCord = Random.Range (-5, 6);
					yCord = Random.Range (-5, 6);

					while (xCord == yCord || yCord==0)
						yCord = Random.Range (-5, 6);
					
					while (xCord == yCord || xCord==0)
						xCord = Random.Range (-5, 6);

					Answer = (Random.Range (0, 2) == 0 ? xCord : yCord).ToString ();
					QuestionText.text = string.Format ("What is the {2} of the point ({0}, {1})?", xCord, yCord, (Answer == xCord.ToString() ? "abscissa" : "ordinate"));
				}
				else if (selector == 5)
				{
					xCord = Random.Range (-5, 6);
					yCord = Random.Range (-5, 6);

					while (xCord == yCord || yCord==0)
						yCord = Random.Range (-5, 6);

					while (xCord == yCord || xCord==0)
						xCord = Random.Range (-5, 6);
					
					QuestionText.text = string.Format ("Plot the point ({0}, {1}) on the given graph.", xCord, yCord);
					Answer = string.Format ("({0},{1})", xCord, yCord);


					graphHelper.SetGridParameters(new Vector2(20,20),22);
					graphHelper.SetGraphQuesType(GraphQuesType.PlotPoint);
					graphHelper.SetCorrectPlottedPoint(new Vector2(xCord,yCord));
					graphHelper.DrawGraph();
					graphHelper.PlotRandomPoint();
				}
				else if (selector == 6)
				{
					SetNumpadMode();
					xCord = Random.Range (-5, 6);
					yCord = Random.Range (-5, 6);

					while (xCord == yCord || yCord==0)
						yCord = Random.Range (-5, 6);

					while (xCord == yCord || xCord==0)
						xCord = Random.Range (-5, 6);

					QuestionText.text = "Determine the coordinates of the point shown on the graph.";
					graphHelper.SetGridParameters(new Vector2(12,12),15);
					graphHelper.DrawGraph();
					graphHelper.PlotPoint(new Vector2(xCord,yCord),"A",false);
					graphHelper.ShiftPosition(new Vector2(0,180f));
					Answer = string.Format ("({0},{1})", xCord, yCord);
				}
			}
			#endregion
			#region level2
			if (level == 2) 
			{
				selector = GetRandomSelector (1, 7);
				if (selector == 1) 
				{
					xCord = Random.Range (-5, 6);
					yCord = Random.Range (-5, 6);
					xCord1 = Random.Range (-5, 6);
					yCord1 = Random.Range (-5, 6);
					while (Vector2.Distance(new Vector2(xCord,yCord),new Vector2(xCord1,yCord1)) < 4)  // to set sufficient distance between the points
					{
						xCord1 = Random.Range (-5, 6);
						yCord1 = Random.Range (-5, 6);
					}
					QuestionText.text = string.Format ("Plot the points ({0}, {1}) and ({2}, {3}) and connect them using the given line.", xCord, yCord, xCord1, yCord1);
					Answer = string.Format ("({0}, {1})", xCord, yCord);
					graphHelper.SetGridParameters(new Vector2(20,20),22);
					graphHelper.SetGraphQuesType(GraphQuesType.PlotFixedLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetFixedLinePoints(new Vector2[]{new Vector2(xCord,yCord),new Vector2(xCord1,yCord1)});
				}
				else if (selector == 2) 
				{
					SetNumpadMode();

					Repeat:   //In case Loop Hangs

					xCord = Random.Range (-5, 6);
					yCord = Random.Range (-5, 6);
					slope = Random.Range (1, 4);
					if (Random.Range (1, 3) == 1)
						slope *= -1;
					
					int loopCount = 0;
					do
					{	
						loopCount ++;
						xCord1 = Random.Range (-5, 6);
						yCord1 = yCord - slope * (xCord - xCord1);
						xCord2 = Random.Range (-5, 6);
						yCord2 = yCord - slope * (xCord - xCord2);
						if (loopCount == 1000)
							goto Repeat;
					} while (yCord1 > 5 || yCord1 < -5 || yCord1 == yCord || yCord2 > 5 || yCord2 < -5 || xCord2 == xCord1 || xCord2 == xCord);

					randSelector = Random.Range (0, 2);
					QuestionText.text = string.Format ("What is the {5} of the point with {6} = {0} on the line connecting ({1}, {2}) and ({3}, {4})?", (randSelector == 0 ? xCord2 : yCord2), xCord, yCord, xCord1, yCord1, (randSelector == 1 ? "abscissa" : "ordinate"), (randSelector == 0 ? "abscissa" : "ordinate"));

					graphHelper.SetGridParameters(new Vector2(12,12),15);
					graphHelper.DrawGraph();
					graphHelper.DrawLineBetweenPoints (new Vector2 (xCord, yCord), new Vector2 (xCord1, yCord1));
					graphHelper.ShiftPosition(new Vector2(0,165f));
					Answer = string.Format ("{0}", (randSelector == 1 ? xCord2 : yCord2));
				}
				else if (selector == 3)
				{
					slope = Random.Range (1, 5); 
					QuestionText.text = string.Format ("Plot two points that lie on the line y = {0}x.", slope == 1? "": "" + slope);
					graphHelper.SetGridParameters(new Vector2(20,20),22);
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(slope,-1,0));
				}
				else if (selector == 4)
				{
					slope = - Random.Range (1, 5); 
					QuestionText.text = string.Format ("Plot two points that lie on the line y = {0}x.", slope == -1? "- ": "- " + Mathf.Abs (slope));
					graphHelper.SetGridParameters(new Vector2(20,20),22);
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(slope,-1,0));
				}
				else if (selector == 5)
				{
					slope = Random.Range (1, 5);
					QuestionText.text = string.Format ("Plot the line y = {0}x.", slope == 1? "": "" + slope);
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(slope,-1,0));
				}
				else if (selector == 6)
				{
					slope = - Random.Range (1, 5);
					QuestionText.text = string.Format ("Plot the line y = {0}x.", slope == -1? "- ": "- " + -slope);
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(slope,-1,0));

				}
			}
			#endregion
			#region level3
			if (level == 3) 
			{
				selector = GetRandomSelector (1, 6);
				if (selector == 1) 
				{
					slope = Random.Range (1, 5); 
					intercept = (Random.Range (1, 3) == 1 ? -1 : 1) * Random.Range (1, 5); 
					QuestionText.text = string.Format ("Plot two points that lie on the line y = {0}x {1}.", slope == 1? "": "" + slope, (intercept < 0? "- ": "+ ") + Mathf.Abs(intercept));
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(slope,-1,intercept));
				}
				else if (selector == 2) 
				{
					slope = - Random.Range (1, 5); 
					intercept = (Random.Range (1, 3) == 1 ? -1 : 1) * Random.Range (1, 5); 
					QuestionText.text = string.Format ("Plot two points that lie on the line y = {0}x {1}.", slope == -1? "- ": "- " + -slope, (intercept < 0? "- ": "+ ") + Mathf.Abs(intercept));
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(slope,0,intercept));
				}
				else if (selector == 3) 
				{
					slope = Random.Range (1, 5); 
					intercept = (Random.Range (1, 3) == 1 ? -1 : 1) * Random.Range (1, 5); 
					QuestionText.text = string.Format ("Plot the line y = {0}x {1}.", slope == 1? "": "" + slope, (intercept < 0? "- ": "+ ") + Mathf.Abs(intercept));
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(slope,-1,intercept));
				}
				else if (selector == 4) 
				{
					slope = - Random.Range (1, 5); 
					intercept = (Random.Range (1, 3) == 1 ? -1 : 1) * Random.Range (1, 5); 
					QuestionText.text = string.Format ("Plot the line y = {0}x {1}.", slope == -1? "- ": "- " + -slope, (intercept < 0? "- ": "+ ") + Mathf.Abs(intercept));
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(slope,-1,intercept));
				}
				else if (selector == 5) 
				{
					intercept = Random.Range (-5, 6);
					int random = Random.Range (1, 3);
					QuestionText.text = string.Format ("Plot the line {0} = {1}.", (random ==1 ? "x" : "y"), intercept);
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(random==1?1:0,random==1?0:1,-intercept));
				}
				else if (selector == 6) 
				{
					xCord1 = (Random.Range (1,3) == 1 ? 1 : -1) * Random.Range (2, 6); // coeff of x
					yCord1 = (Random.Range (1,3) == 1 ? 1 : -1) * Random.Range (2, 6); // coeff of y
					intercept = xCord1 * yCord1;
					QuestionText.text = string.Format ("Plot the line {0}y {1}x = {2}.", yCord1, (xCord1 < 0? "- ": "+ ") + Mathf.Abs (xCord1), intercept);
					graphHelper.SetGraphQuesType(GraphQuesType.PlotLine);
					graphHelper.DrawGraph();
					graphHelper.DrawRandomLine();
					graphHelper.SetCurrentLineParameters(new Vector3(xCord1,yCord1,-intercept));
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
			} else if (value == 12)
			{
				if (checkLastTextFor (new string[1]{ "(" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "(";
			}
			else if (value == 13)
			{
				if (checkLastTextFor (new string[1]{ ")" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ")";
			}
			else if (value == 14)
			{
				if (checkLastTextFor (new string[1]{ "-" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
			else if (value == 15)
			{
				if (checkLastTextFor (new string[1]{ "," })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ",";
			}
			else if (value == 16)
			{
				if (checkLastTextFor (new string[1]{ "." })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
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
			this.numPadBg.SetActive (true);
			this.CheckButton.SetActive (false);
			this.numPad.SetActive (true);
			this.MCQ.SetActive (false);
			this.GeneralButton.gameObject.SetActive (true);
			ContinueBtn.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,ContinueBtn.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
			FlagButton.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,FlagButton.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
			SolutionButton.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f,SolutionButton.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
		}

		protected void SetGraphMode()
		{
			this.numPadBg.SetActive (false);
			this.CheckButton.gameObject.SetActive(true);
			this.numPad.SetActive (false);
			this.MCQ.SetActive (false);
			this.GeneralButton.gameObject.SetActive (false);
			ContinueBtn.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (375f,ContinueBtn.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
			FlagButton.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (375f,FlagButton.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
			SolutionButton.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (375f,SolutionButton.gameObject.GetComponent<RectTransform> ().anchoredPosition.y);
		}
	}
}