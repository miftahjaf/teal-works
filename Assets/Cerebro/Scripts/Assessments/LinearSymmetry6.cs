﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class LinearSymmetry6 : BaseAssessment {

		private string Answer;
		private List<string> AnswerList;
		private string[] expressions;
		private int randSelector;
		private const float diagramWidth = 150f;
		private float diagramBase;
		private int xCord, yCord, xCord1, yCord1, xCord2, yCord2, xCord3, yCord3, xCord4, yCord4, side1, side2;
		private float angle;

		public TEXDraw subQuestionTEX;
		public GameObject MCQ; 
		public GraphHelper graphHelper;     
		public DiagramHelper diagramHelper; 
		public GameObject CheckButton;
		public GameObject numPadBg;
		void Start () {

			base.Initialise ("M", "LIS06", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[1];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;


			GenerateQuestion ();
		
		}

		public override void SubmitClick(){
			if (ignoreTouches || (userAnswerText.text == "" && graphHelper.graphQuesType == GraphQuesType.None) || (graphHelper.graphQuesType != GraphQuesType.None && !graphHelper.IsAnswered())) {
				return;
			}
			int increment = 0;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (graphHelper.graphQuesType == GraphQuesType.None) {

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
					Debug.Log ("User Answer " + userAnswerText.text + " QuestionType " + graphHelper.graphQuesType);

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
				} else if (Queslevel == 5) {
					increment = 15;
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
			if (isRevisitedQuestion) {
				return;
			}
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
			graphHelper.HandleIncorrectAnwer (isRevisitedQuestion);
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
				graphHelper.ResetAnswer ();
			}
			else
			{
				this.CheckButton.gameObject.SetActive(false);
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
			diagramHelper.Reset ();

			AnswerList = new List<string> ();
			graphHelper.Reset ();
			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 5);
				if (selector == 1) 
				{
					SetNumpadMode ();
					randSelector = Random.Range (0, 3);

					int[] lines = new int[]{1, 2, 3};
					lines.Shuffle ();

					if (randSelector == 0) // right angled
					{
						diagramBase = diagramWidth;
							
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, false, diagramBase));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase, 0), 135, false, diagramWidth * Mathf.Sqrt (2)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, diagramWidth), 270, false, diagramWidth));

						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 45, true, diagramWidth,1).SetLineType(LineShapeType.Dotted));
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 225, true, diagramWidth / 5f).SetLineType(LineShapeType.Dotted).SetLineText(lines[0].ToString()).SetLineTextDirection(TextDir.Up));

						diagramHelper.AddLinePoint (new LinePoint ("" , new Vector2 (diagramWidth, 0), 157.5f, true, diagramWidth * 1.3f,1).SetLineType(LineShapeType.Dotted));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramWidth, 0), 337.5f, true, diagramWidth / 5f).SetLineType(LineShapeType.Dotted).SetLineText(lines[1].ToString()).SetLineTextDirection(TextDir.Down));

						diagramHelper.AddLinePoint (new LinePoint ("" , new Vector2 (0, diagramWidth), 112.5f, true, diagramWidth / 5f,1).SetLineType(LineShapeType.Dotted).SetLineText(lines[2].ToString()).SetLineTextDirection(TextDir.Left));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, diagramWidth), 292.5f, true, diagramWidth * 1.3f).SetLineType(LineShapeType.Dotted));

						Answer = lines[0].ToString();
					}
					else if (randSelector == 1) //isosceles triangle
					{
						diagramBase = 0.8f * diagramWidth;

						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, false, diagramBase));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase, 0), 180f - Mathf.Rad2Deg * Mathf.Atan (2f * diagramWidth / diagramBase), false, Mathf.Sqrt (diagramWidth * diagramWidth + diagramBase * diagramBase / 4f)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase / 2f, diagramWidth), 180f + Mathf.Rad2Deg * Mathf.Atan (2f * diagramWidth / diagramBase), false, Mathf.Sqrt (diagramWidth * diagramWidth + diagramBase * diagramBase / 4f)));

						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 30, true, diagramWidth * 1.3f,1).SetLineType(LineShapeType.Dotted));
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 210, true, diagramWidth / 5f).SetLineType(LineShapeType.Dotted).SetLineText(lines[0].ToString()).SetLineTextDirection(TextDir.Up));

						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase, 0), 150, true, diagramWidth * 1.3f,1).SetLineType(LineShapeType.Dotted));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase, 0), 330, true, diagramWidth / 5f).SetLineType(LineShapeType.Dotted).SetLineType(LineShapeType.Dotted).SetLineText(lines[1].ToString()).SetLineTextDirection(TextDir.Up));

						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase / 2f, diagramWidth), 270, true, diagramWidth * 1.1f,1).SetLineType(LineShapeType.Dotted));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase / 2f, diagramWidth), 90, true, diagramWidth / 10f).SetLineType(LineShapeType.Dotted).SetLineText(lines[2].ToString()).SetLineTextDirection(TextDir.Left));

						Answer = lines[2].ToString();
					}
					else if (randSelector == 2) //rectangle
					{
						diagramBase = 1.5f * diagramWidth;

						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, false, diagramBase));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase, 0), 90, false, diagramWidth));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase, diagramWidth), 180, false, diagramBase));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, diagramWidth), 270, false, diagramWidth));

						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, Mathf.Rad2Deg * Mathf.Atan (diagramWidth / diagramBase), true, 1.1f * Mathf.Sqrt (diagramWidth * diagramWidth + diagramBase * diagramBase),1).SetLineType(LineShapeType.Dotted));
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 180f + Mathf.Rad2Deg * Mathf.Atan (diagramWidth / diagramBase), true, diagramWidth / 5f).SetLineType(LineShapeType.Dotted).SetLineText(lines[0].ToString()).SetLineTextDirection(TextDir.Up));

						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase, 0), 180f - Mathf.Rad2Deg * Mathf.Atan (diagramWidth / diagramBase), true, 1.1f * Mathf.Sqrt (diagramWidth * diagramWidth + diagramBase * diagramBase),1).SetLineType(LineShapeType.Dotted));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (diagramBase, 0), 360f - Mathf.Rad2Deg * Mathf.Atan (diagramWidth / diagramBase), true, diagramWidth / 5f).SetLineType(LineShapeType.Dotted).SetLineText(lines[1].ToString()).SetLineTextDirection(TextDir.Up));

						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, diagramWidth / 2f), 0, true, 1.2f * diagramBase,1).SetLineType(LineShapeType.Dotted));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, diagramWidth / 2f), 180, true, diagramBase / 5f).SetLineType(LineShapeType.Dotted).SetLineText(lines[2].ToString()).SetLineTextDirection(TextDir.Up));

						Answer = lines[2].ToString();
					}
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- new Vector2 (diagramBase / 2, diagramWidth / 2));

					QuestionText.text = "Which of the following is a line of symmetry?";
				}
				else if (selector == 2) 
				{
					SetNumpadMode ();
					expressions = new string[] {"1a line segment", "0a scalene triangle", "1an isosceles triangle", "3an equilateral triangle", "∞a circle", "2a rectangle", "4a square", "1the letter 'M'", "2a rhombus", "0a parallelogram", "1a kite"};
					randSelector = Random.Range (0, expressions.Length);
					QuestionText.text = string.Format ("How many lines of symmetry are there in {0}?", expressions[randSelector].Substring (1));
					Answer = expressions[randSelector].Substring (0, 1);
				}
				else if (selector == 3)
				{
					randSelector = Random.Range (0, 5);

					graphHelper.SetGridParameters (new Vector2(20,20),22);
					graphHelper.DrawGraph (false);
					graphHelper.DrawRandomLine(true,false,LineShapeType.Dotted);

					if (randSelector == 0) //circle
					{
						xCord = Random.Range (-5, 6);
						yCord = Random.Range (-5, 6);
						side1 = Random.Range (2, Mathf.Min (10 - Mathf.Abs (xCord), 10 - Mathf.Abs (yCord))); //radius of the circle
						graphHelper.SetGraphQuesType (GraphQuesType.PointLiesOnLine);
						graphHelper.SetCorrectPlottedPoint(new Vector2(xCord, yCord));
						graphHelper.SetCurrentLineParameters(new List<Vector3>(){new Vector3(1,1,-xCord-yCord)});
						graphHelper.DrawArc (new Vector2 (xCord, yCord), side1, 0, 360);

					}
					else if (randSelector == 1)//isosceles triangle
					{
						xCord = Random.Range (-7, -1);
						xCord1 = xCord + 2 * Random.Range (2, 5);
						yCord = Random.Range (-7, 4);
						yCord1 = yCord;							
						xCord2 = (xCord + xCord1) / 2;
						yCord2 = yCord + Random.Range (3, 10 - yCord);

						graphHelper.SetGraphQuesType (GraphQuesType.PlotLine);
						graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord,yCord),new Vector2(xCord1,yCord1),new Vector2(xCord2,yCord2),new Vector2(xCord, yCord)});
						graphHelper.SetCurrentLineParameters(new List<Vector3>(){new Vector3(1,0,-xCord2)});
					}
					else if (randSelector == 2)//square
					{
						xCord = Random.Range (-7, 3);
						yCord = Random.Range (-7, 3);
						side1 = Random.Range (4, Mathf.Min (10 - xCord, 10 - yCord));
						xCord1 = xCord + side1;
						yCord1 = yCord;
						xCord2 = xCord1;
						yCord2 = yCord1 + side1;
						xCord3 = xCord;
						yCord3 = yCord2;

						graphHelper.SetSnapValue (11);
						graphHelper.SetGraphQuesType (GraphQuesType.PlotLine);
						graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord,yCord),new Vector2(xCord1,yCord1),new Vector2(xCord2,yCord2),new Vector2(xCord3, yCord3),new Vector2(xCord, yCord)});
						graphHelper.SetCurrentLineParameters(new List<Vector3>(){new Vector3(1,0,-(xCord + side1/2f)),new Vector3(0,1,-(yCord + side1/2f)),new Vector3(1,-1,-(xCord - yCord)),new Vector3(1,1,-(xCord1 + yCord1))});
					}
					else if (randSelector == 3)//rectangle
					{
						xCord = Random.Range (-7, 3);
						yCord = Random.Range (-7, 3);
						side1 = Random.Range (4, 10 - xCord);
						side2 = Random.Range (4, 10 - yCord);

						while (side1 == side2)
							side2 = Random.Range (4, 10 - yCord);
						
						xCord1 = xCord + side1;
						yCord1 = yCord;
						xCord2 = xCord1;
						yCord2 = yCord1 + side2;
						xCord3 = xCord;
						yCord3 = yCord2;

						graphHelper.SetGraphQuesType (GraphQuesType.PlotLine);
						graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord,yCord),new Vector2(xCord1,yCord1),new Vector2(xCord2,yCord2),new Vector2(xCord3, yCord3),new Vector2(xCord, yCord)});
						graphHelper.SetCurrentLineParameters(new List<Vector3>(){new Vector3(1,0,-(xCord + side1/2f)),new Vector3(0,1,-(yCord + side2/2f))});
					}
					else if (randSelector == 4) // rhombus
					{
						do{
							xCord = Random.Range (-7, 3);
							yCord3 = Random.Range (-7, 3);
							side1 = Random.Range (2, 5 - xCord / 2);   // half of diagonal 1
							side2 = Random.Range (2, 5 - yCord3 / 2);   // half of diagonal 2

						} while (side1 == side2 || xCord == 0 || yCord3 == 0);

						xCord1 = xCord + side1;
						yCord1 = yCord3 + 2 * side2;
						xCord2 = xCord + 2 * side1;
						yCord = yCord3 + side2;
						yCord2 = yCord;
						xCord3 = xCord1;

						graphHelper.SetSnapValue (11);
						graphHelper.SetGraphQuesType (GraphQuesType.PlotLine);
						graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord,yCord),new Vector2(xCord1,yCord1),new Vector2(xCord2,yCord2),new Vector2(xCord3, yCord3),new Vector2(xCord, yCord)});
						graphHelper.SetCurrentLineParameters(new List<Vector3>(){new Vector3(1,0,-xCord1),new Vector3(0,1,-yCord)});
					}

					QuestionText.text = "Draw a line of symmetry of the given figure.";
				}
				else if (selector == 4)
				{
					randSelector = Random.Range (0, 3);

					graphHelper.SetGridParameters (new Vector2(20,20),22);
					graphHelper.DrawGraph (false);
					graphHelper.DrawRandomLine(true,false,LineShapeType.Dotted);

					if (randSelector == 0) //arrow
					{
						xCord = Random.Range (1, 5);
						xCord *= Random.Range (1, 3) == 1? -1: 1;
						yCord = Random.Range (-7, -1);
						side1 = Random.Range (3, 9);   //lower part
						side2 = Random.Range (2, 4);   // upper part

						graphHelper.SetGraphQuesType (GraphQuesType.PlotLine);
						graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord - 1,yCord),new Vector2(xCord + 1, yCord),new Vector2(xCord + 1,yCord + side1),new Vector2(xCord + 1.5f, yCord + side1),new Vector2(xCord, yCord + side1 + side2)});
						graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord - 1,yCord),new Vector2(xCord - 1,yCord + side1),new Vector2(xCord - 1.5f, yCord + side1),new Vector2(xCord,yCord + side1 + side2)});
						graphHelper.SetCurrentLineParameters(new List<Vector3>(){new Vector3(1,0,-xCord)});
					}
					else if (randSelector == 1) //square with sides cut into curves
					{
						side1 = Random.Range (4, 9);  // square side
						side2 = Random.Range (1, 1 + side1 / 3);  //circle radius
						xCord = Random.Range (-7, 10 - side1);
						yCord = Random.Range (-7, 10 - side1);

						graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side2, yCord), new Vector2 (xCord + side1 - side2, yCord)});
						graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side2, yCord + side1), new Vector2 (xCord + side1 - side2, yCord + side1)});
						graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord, yCord + side2), new Vector2 (xCord, yCord + side1 - side2)});
						graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side1, yCord + side2), new Vector2 (xCord + side1, yCord + side1 - side2)});

						graphHelper.DrawArc (new Vector2 (xCord, yCord), new Vector2(xCord + side2, yCord), new Vector2 (xCord, yCord + side2));
						graphHelper.DrawArc (new Vector2 (xCord + side1, yCord), new Vector2 (xCord + side1, yCord + side2), new Vector2(xCord + side1 - side2, yCord));
						graphHelper.DrawArc (new Vector2 (xCord + side1, yCord + side1), new Vector2 (xCord + side1 - side2, yCord + side1), new Vector2(xCord + side1, yCord + side1 - side2));
						graphHelper.DrawArc (new Vector2 (xCord, yCord + side1), new Vector2 (xCord, yCord + side1 - side2), new Vector2(xCord + side2, yCord + side1));

						graphHelper.SetSnapValue (11);
						graphHelper.SetGraphQuesType (GraphQuesType.PlotLine);
						graphHelper.SetCurrentLineParameters(new List<Vector3>(){new Vector3(1,0,-(xCord + side1/2f)),new Vector3(0,1,-(yCord + side1/2f)),new Vector3(1,-1,-(xCord - yCord)),new Vector3(1,1,-(xCord + yCord + side1))});
					}
					else if (randSelector == 2) //hexagon
					{
						do{
							xCord = Random.Range (-8, -2);
							yCord4 = Random.Range (-7, 3);

						} while (xCord == 0 || yCord4 == 0);

						side1 = Random.Range (2, 4 - xCord / 4);  
						side2 = Random.Range (2, 5 - yCord4 / 2); 

						xCord1 = xCord + side1;
						xCord2 = xCord + 2 * side1;
						xCord3 = xCord + 3 * side1;
						yCord = yCord4 + side2;
						yCord1 = yCord4 + 2 * side2;

						graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord,yCord),new Vector2(xCord1,yCord1),new Vector2(xCord2,yCord1),new Vector2(xCord3, yCord)});
						graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord,yCord),new Vector2(xCord1,yCord4),new Vector2(xCord2,yCord4),new Vector2(xCord3, yCord)});

						graphHelper.SetGraphQuesType (GraphQuesType.PlotLine);
						graphHelper.SetCurrentLineParameters(new List<Vector3>(){new Vector3(1,0,-(xCord1 + side1/2f)),new Vector3(0,1,-yCord)});
					}

					QuestionText.text = "Draw a line of symmetry of the given figure.";
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
				if (checkLastTextFor (new string[1]{ "∞" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "∞";
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