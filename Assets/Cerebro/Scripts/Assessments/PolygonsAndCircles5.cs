using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class PolygonsAndCircles5 : BaseAssessment
	{

		public Text subQuestionText;
		public GameObject MCQ;
		public DiagramHelper diagramHelper;
		private string Answer;
		private string alternateAnswer;
		List<string> options;
		private int randSelector;
		private int coeff1;
		private int coeff2;
		private int coeff3;
		private int coeff4;
		private float lengthForDiagram;
		private const float breadthForDiagram = 120f;
		private const float arcRadius = 40f;
		float angle1, angle2;
		Vector2 origin;

		void Start()
		{
			StartCoroutine(StartAnimation());
			base.Initialise("M", "PAC05", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++)
			{
				scorestreaklvls[i] = 0;
			}

			levelUp = false;

			Answer = "";//
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
			var correct = false;
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
					UpdateStreak(8, 12);
					increment = 5;
				}
				else if (Queslevel == 2)
				{
					UpdateStreak(10, 15);
					increment = 10;
				}
				else if (Queslevel == 3)
				{
					UpdateStreak(8, 12);
					increment = 15;
				}
				else if (Queslevel == 4)
				{
					UpdateStreak(8, 12);
					increment = 15;
				}
	
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

			Vector2 newPoint;
			float newRadius;

			ignoreTouches = false;
			base.QuestionStarted();
			// Generating the parameters

			level = Queslevel;

			answerButton = GeneralButton;
			subQuestionText.gameObject.SetActive(false);
			QuestionText.gameObject.SetActive(false);
			QuestionLatext.gameObject.SetActive(true);
			SetNumpadMode ();
			diagramHelper.Reset ();
			options = new List<string>();

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
					SetMCQMode (3);

					QuestionLatext.text = "Identify the type of triangle in the image given below :";
					options.Add ("Acute angled");
					options.Add ("Right angled");
					options.Add ("Obtuse angled");

					randSelector = Random.Range (0, 3);
					Answer = options [randSelector];
					RandomizeMCQOptionsAndFill (options);

					if (randSelector == 0)  // Acute angled triangle
					{
						angle1 = Random.Range (30, 60);
						angle2 = Random.Range (90 - (int) angle1 + 1, 80);   // angle2 > 90 - angle1
					}
					else if (randSelector == 1)  // right angled triangle
					{
						angle1 = Random.Range (30, 61);
						angle2 = 90 - angle1;
					}
					else if (randSelector == 2)  // Obtuse angled triangle
					{
						angle1 = Random.Range (30, 60);
						angle2 = Random.Range (30, 90 - (int) angle1);   // angle2 < 90 - angle1
					}
						
					lengthForDiagram = breadthForDiagram * (1f / Mathf.Tan (angle1 * Mathf.Deg2Rad) + 1f / Mathf.Tan (angle2 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));

					diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, Vector2.zero, 0f, angle1));
					diagramHelper.AddAngleArc (new AngleArc ("" + angle2 + MathFunctions.deg, new Vector2 (lengthForDiagram, 0), 180f - angle2, 180f));
					diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle1 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1, 360f - angle2, 0, false, false));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 2)
				{
					SetMCQMode (3);

					QuestionLatext.text = "Identify the type of triangle in the image given below :";
					options.Add ("Scalene");
					options.Add ("Isosceles");
					options.Add ("Equilateral");

					randSelector = Random.Range (0, 3);
					Answer = options [randSelector];
					RandomizeMCQOptionsAndFill (options);

					if (randSelector == 0)  //Scalene
					{
						coeff1 = Random.Range (3, 10);

						do {
							coeff2 = Random.Range (3, 10);
							coeff3 = Random.Range (3, 10);
						
						} while (coeff1 == coeff2 || coeff2 == coeff3 || coeff3 == coeff1);
							
						do {
							angle1 = Random.Range (30, 60);
							angle2 = Random.Range (90 - (int) angle1 + 1, 80);   // angle2 > 90 - angle1

						} while (angle1 == angle2 || angle1 == 180 - (angle1 + angle2) || angle2 == 180 - (angle1 + angle2));
					}
					else if (randSelector == 1)  //Isosceles
					{
						coeff1 = Random.Range (3, 10);

						do {
							coeff2 = Random.Range (3, 10);
						} while (coeff2 == coeff1);

						coeff3 = coeff2;

						angle1 = Random.Range (40, 70);

						while (angle1 == 60){
							angle1 = Random.Range (40, 70);
						}

						angle2 = angle1;
					}
					else if (randSelector == 2)  // equilateral
					{
						coeff1 = Random.Range (3, 10);
						coeff2 = coeff1;
						coeff3 = coeff1;
						angle1 = 60;
						angle2 = 60;
					}

					lengthForDiagram = breadthForDiagram * (1f / Mathf.Tan (angle1 * Mathf.Deg2Rad) + 1f / Mathf.Tan (angle2 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, false, lengthForDiagram).SetLineText(coeff1 + " cm").SetLineTextDirection(TextDir.Down));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetLineText(coeff2 + " cm").SetLineTextDirection(TextDir.Left));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetLineText(coeff3 + " cm").SetLineTextDirection(TextDir.Right));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 3)
				{
					List<Vector2> points = new List<Vector2>();
					SetMCQMode (4);

					QuestionLatext.text = "Identify the given quadrilateral :";
					string[] answerOptions = new string[] {"Square", "Parallelogram", "Rhombus", "Rectangle", "Trapezium"};
					answerOptions.Shuffle ();
					for (int i = 0; i < 4; i++){
						options.Add (answerOptions [i]);
					}
					Answer = options [0];
					RandomizeMCQOptionsAndFill (options);

					if (Answer == "Square")
					{
						lengthForDiagram = breadthForDiagram;
						points.Add (new Vector2 (0, 0));
						points.Add (new Vector2 (lengthForDiagram, 0));
						points.Add (new Vector2 (lengthForDiagram, breadthForDiagram));
						points.Add (new Vector2 (0, breadthForDiagram));
					} 
					else if (Answer == "Parallelogram")
					{
						angle1 = 90 + Random.Range (10, 40) * (Random.Range (1, 3) == 1? 1: -1);
						lengthForDiagram = breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad) * Random.Range (1.2f, 1.8f);

						points.Add (new Vector2 (0, 0));
						points.Add (new Vector2 (lengthForDiagram, 0));
						points.Add (new Vector2 (lengthForDiagram + breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
						points.Add (new Vector2 (breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
						lengthForDiagram += breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad);  //to centralise the figure
					}
					else if (Answer == "Rhombus")
					{
						angle1 = 90 + Random.Range (10, 40) * (Random.Range (1, 3) == 1? 1: -1);
						lengthForDiagram = breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad);

						points.Add (new Vector2 (0, 0));
						points.Add (new Vector2 (lengthForDiagram, 0));
						points.Add (new Vector2 (lengthForDiagram + breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
						points.Add (new Vector2 (breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
						lengthForDiagram += breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad);  //to centralise the figure
					}
					else if (Answer == "Rectangle")
					{
						lengthForDiagram = breadthForDiagram * Random.Range (1.2f, 1.8f);
						points.Add (new Vector2 (0, 0));
						points.Add (new Vector2 (lengthForDiagram, 0));
						points.Add (new Vector2 (lengthForDiagram, breadthForDiagram));
						points.Add (new Vector2 (0, breadthForDiagram));
					}
					else if (Answer == "Trapezium")
					{
						float lengthForDiagram1 = breadthForDiagram * Random.Range (1.2f, 1.5f);
						do {
							angle1 = 90 + Random.Range (10, 30) * MathFunctions.GenerateRandomIntegerExcluding0 (-1, 2);
							angle2 = 90 + Random.Range (10, 30) * MathFunctions.GenerateRandomIntegerExcluding0 (-1, 2);
							lengthForDiagram = lengthForDiagram1 + breadthForDiagram * (1f / Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1f / Mathf.Tan (angle1 * Mathf.Deg2Rad));
						} while (Mathf.Abs (angle1 - angle2) < 10f ||  lengthForDiagram < lengthForDiagram1 / 2f);

						if (angle1 < 90)
						{
							points.Add (new Vector2 (0, 0));
							points.Add (new Vector2 (lengthForDiagram1, 0));
							points.Add (new Vector2 (lengthForDiagram1 + breadthForDiagram / Mathf.Tan (angle2 * Mathf.Deg2Rad), breadthForDiagram));
							points.Add (new Vector2 (breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
						} 
						else {
							points.Add (new Vector2 (- breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), 0));
							points.Add (new Vector2 (lengthForDiagram1 - breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), 0));
							points.Add (new Vector2 (lengthForDiagram, breadthForDiagram));
							points.Add (new Vector2 (0, breadthForDiagram));
						}
						lengthForDiagram = lengthForDiagram > lengthForDiagram1 ? lengthForDiagram : lengthForDiagram1;
					}
					origin = new Vector2 (lengthForDiagram / 2, breadthForDiagram / 2);

					for (int i = 0; i < points.Count; i++){
						diagramHelper.AddLinePoint (new LinePoint ("", points[i], points[MathFunctions.AddCyclic (i, 1, 4)]));
					}

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 4)
				{
					SetMCQMode (4);

					QuestionLatext.text = "Identify the given polygon :";
					List<string> answerOptions = new List<string>() {"Pentagon", "Hexagon", "Heptagon", "Octagon", "Nonagon", "Decagon"};
					List<int> NumberOfSides = new List<int>() {5, 6, 7, 8, 9, 10}; 

					randSelector = Random.Range (0, answerOptions.Count); 
					Answer = answerOptions [randSelector];
					int numberOfSides = NumberOfSides[randSelector];

					answerOptions.Remove (Answer);
					answerOptions.Shuffle ();
					options.Add (Answer);
					for (int i = 0; i < 3; i++){
						options.Add (answerOptions [i]);
					}
					RandomizeMCQOptionsAndFill (options);

					angle1 = 360f / numberOfSides;
					angle2 = Random.Range (0, 90);
				
					for (int i = 0; i < numberOfSides; i++){
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (Vector2.zero, angle2, breadthForDiagram * 0.6f), MathFunctions.PointAtDirection (Vector2.zero, angle2 + angle1, breadthForDiagram * 0.6f)));
						angle2 += angle1;
					}
					diagramHelper.Draw ();
				}
				else if (selector == 5)
				{
					List<string> vertexNames = new List<string>();
					SetMCQMode (3);

					QuestionLatext.text = "Which one of the given options is a diagonal?";
					int numberOfSides = Random.Range (4, 11);
					int minVertexDifference = numberOfSides < 8? 2: 3;  // minimum difference between vertices of 'diagonal' and 'not diagonal'
					angle1 = 360f / numberOfSides;
					angle2 = Random.Range (0, 90);

					for (int i = 0; i <= numberOfSides; i++){
						vertexNames.Add ("" + (char)(65 + i));
					}
					vertexNames.Shuffle ();

					randSelector = Random.Range (0, numberOfSides);   // starting vertex for 'side (answerOption)' 
					int randSelector1 = MathFunctions.AddCyclic (randSelector, MathFunctions.GenerateRandomIntegerExcluding0 (-1, 2), numberOfSides); // end vertex for 'side (answerOption)'
					options.Add (vertexNames[randSelector] + vertexNames[randSelector1]);  //side

					randSelector = MathFunctions.AddCyclic (randSelector, Random.Range (1, numberOfSides), numberOfSides);   // starting vertex for 'diagonal' 
					randSelector1 = MathFunctions.AddCyclic (randSelector, Random.Range (minVertexDifference, numberOfSides - minVertexDifference + 1), numberOfSides);  // end vertex for 'diagonal'
					options.Add (vertexNames[randSelector] + vertexNames[randSelector1]);  // diagonal
					diagramHelper.AddLinePoint (new LinePoint ("" , MathFunctions.PointAtDirection (Vector2.zero, angle2 + angle1 * randSelector, breadthForDiagram * 0.6f), MathFunctions.PointAtDirection (Vector2.zero, angle2 + angle1 * randSelector1, breadthForDiagram * 0.6f)).SetShouldShowDot (false).SetPointTextOffset (MathFunctions.PointAtDirection (Vector2.zero, angle2, 15f))); //diagonal

					do {
						randSelector = MathFunctions.AddCyclic (randSelector, Random.Range (minVertexDifference - 1, numberOfSides - minVertexDifference + 2), numberOfSides);  // starting vertex for 'not diagonal'
					} while (randSelector == randSelector1);  // checking whether ending vertex of 'diagonal' and starting vertex of 'not diagonal' are not equal

					randSelector1 = MathFunctions.AddCyclic (randSelector, numberOfSides == 4? 2 : Random.Range (minVertexDifference, numberOfSides - minVertexDifference), numberOfSides);  // end vertex for 'not diagonal'
					options.Add (vertexNames[randSelector] + vertexNames[numberOfSides]); // not a diagonal
					diagramHelper.AddLinePoint (new LinePoint (vertexNames [numberOfSides], MathFunctions.PointAtDirection (Vector2.zero, angle2 + angle1 * randSelector, breadthForDiagram * 0.6f), (MathFunctions.PointAtDirection (Vector2.zero, angle2 + angle1 * randSelector1, breadthForDiagram * 0.6f) + MathFunctions.PointAtDirection (Vector2.zero, angle2 + angle1 * (randSelector1 + 1), breadthForDiagram * 0.6f)) / 2f).SetShouldShowDot (false).SetPointTextOffset (MathFunctions.PointAtDirection (Vector2.zero, angle2 + angle1 * (randSelector1 + 0.5f), 15f)));  //not a diagonal

					Answer = options[1];
					RandomizeMCQOptionsAndFill (options);

					for (int i = 0; i < numberOfSides; i++){
						angle2 += angle1;
						diagramHelper.AddLinePoint (new LinePoint (vertexNames[MathFunctions.AddCyclic (i, 1, numberOfSides)], MathFunctions.PointAtDirection (Vector2.zero, angle2 - angle1, breadthForDiagram * 0.6f), MathFunctions.PointAtDirection (Vector2.zero, angle2, breadthForDiagram * 0.6f)).SetShouldShowDot (false).SetPointTextOffset (MathFunctions.PointAtDirection (Vector2.zero, angle2, 15f)));
					}
					diagramHelper.Draw ();
				}
			}
			#endregion
			#region level2

			if (level == 2)
			{
				selector = GetRandomSelector(1, 7);

				if (selector == 1)
				{
					angle1 = Random.Range (30, 120); 
					angle2 = Random.Range (30, 150 - (int) angle1);

					QuestionLatext.text = string.Format ("In a \\Delta{{ABC}} : \\angle{{A}} = {0}{2}, \\angle{{B}} = {1}{2}.\nFind \\angle{{C}}.", angle1, angle2, MathFunctions.deg);
					Answer = string.Format ("{0}{1}", 180 - angle1 - angle2, MathFunctions.deg);
				}
				else if (selector == 2)
				{
					List<int> side = new List<int>();
					SetMCQMode (2);

					side.Add (Random.Range (3, 10));
					side.Add (Random.Range (3, 10));
					Answer = Random.Range (1, 3) == 1? "True": "False";

					if (Answer == "True"){
						int minSide = Mathf.Abs (side[0] - side[1]) < 2 ? 3 : Mathf.Abs (side[0] - side[1]) + 1;
						side.Add (Random.Range (minSide, side[0] + side[1]));
					} 
					else {
						side.Add (Random.Range (Mathf.Abs (side[0] + side[1]), 3 + side[0] + side[1]));
					}

					side.Shuffle ();
					QuestionLatext.text = string.Format ("In a triangle, the sides are {0} cm, {1} cm and {2} cm.\nIs such a triangle possible?", side[0], side[1], side[2]);

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if (selector == 3)
				{
					SetMCQMode (2);

					angle1 = Random.Range (30, 120); 
					angle2 = Random.Range (30, 150 - (int) angle1);
					Answer = Random.Range (1, 3) == 1 ? "Possible" : "Not Possible";
					float angle3 = Answer == "Possible"? 180 - angle1- angle2 : 180 + MathFunctions.GenerateRandomIntegerExcluding0 (-10, 11) - angle1 - angle2; 
						
					QuestionLatext.text = string.Format ("In a triangle, the angles are {0}{3}, {1}{3} and {2}{3}.\nIs such a triangle possible?", angle1, angle2, angle3, MathFunctions.deg);

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "Possible";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "Not Possible";
				}
				else if (selector == 4)
				{
					List<Vector2> points = new List<Vector2>();
					float[] angle = new float[4];
					angle1 = Random.Range (0, 90);
					do {
						angle[0] = Random.Range (30, 61);
						angle[1] = Random.Range (30, 61);
						angle[2] = Random.Range (30, 61);
						angle[3] = Random.Range (30, 61);
					} while (angle[0] + angle[1] + angle[2] + angle[3] != 180);

					for (int i = 0; i < 4; i++){
						points.Add (MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram * 0.7f));
						angle1 += 180 - 2 * angle[i];  // relative position of vertex
					}
					for (int i = 0; i < 4; i++){
						diagramHelper.AddLinePoint (new LinePoint ("", points[i], points[MathFunctions.AddCyclic(i, 1, 4)]));
					}
					diagramHelper.AddLinePoint (new LinePoint ("", points[1], points[3]));

					for (int i = 0; i < 3; i++){
						angle2 = angle[i] + angle[MathFunctions.AddCyclic (i, -1, 4)];    // angle at vertex
						if (i == 1){
							diagramHelper.AddAngleArc (new AngleArc ("" + ((int)Vector2.Angle (points[2] - points[1], points[3] - points[1])) + MathFunctions.deg, points[i], 180 - angle[i] + angle1, 180 - angle[i] + Vector2.Angle (points[2] - points[1], points[3] - points[1]) + angle1, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - (int)Vector2.Angle (points[2] - points[1], points[3] - points[1])) + MathFunctions.deg, points[i], 180 - angle[i] + Vector2.Angle (points[2] - points[1], points[3] - points[1]) + angle1, 180 - angle[i] + angle2 + angle1, arcRadius * 1.2f));
							angle1 += 180 - 2 * angle[i];  
							continue;
						}
						diagramHelper.AddAngleArc (new AngleArc ("" + angle2 + MathFunctions.deg, points[i], 180 - angle[i] + angle1, 180 - angle[i] + angle2 + angle1, arcRadius));
						angle1 += 180 - 2 * angle[i];  // relative position of vertex
					}
					angle2 = angle[3] + angle[2];     // angle at vertex
					diagramHelper.AddAngleArc (new AngleArc ("?", points[3], 180 - angle[3] + angle1, 180 - angle[3] + angle2 + angle1, arcRadius, false, false));
					diagramHelper.Draw ();

					QuestionLatext.text = "Using properties of triangles, find the missing angle of the given quadrilateral.";
					Answer = string.Format ("{0}{1}", angle2, MathFunctions.deg);
				}
				else if(selector == 5)
				{
					float angle3;
					do {
						angle1 = Random.Range (30, 200);
						angle2 = Random.Range (30, 200);
						angle3 = Random.Range (30, 200);
					} while (angle1 + angle2 + angle3 > 330);

					QuestionLatext.text = string.Format ("If three angles of a quadrilateral are {1}{0}, {2}{0} and {3}{0}, find the fourth angle.", MathFunctions.deg, angle1, angle2, angle3);
					Answer = string.Format ("{1}{0}", MathFunctions.deg, 360 - angle1 - angle2 - angle3);
				}
				else if(selector == 6)
				{
					SetMCQMode (2);

					string[] questionsList = new string[] {string.Format ("\\Delta{{ABC}} is a right angled triangle with \\angle{{A}} = 90{0} and \\angle{{B}} = 90{0}.", MathFunctions.deg),
														   string.Format ("\\Delta{{ABC}} is a right angled triangle with \\angle{{A}} = 90{0} and \\angle{{B}} is an obtuse angle.", MathFunctions.deg),
														   string.Format ("\\Delta{{ABC}} is an equilateral triangle with \\angle{{A}} = \\angle{{B}} = \\angle{{C}} = 60{0}", MathFunctions.deg)};
					randSelector = Random.Range (0, 3);
					QuestionLatext.text = questionsList[randSelector];
					Answer = randSelector == 3? "Possible": "Not Possible";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "Possible";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "Not Possible";
				}
			}
			#endregion
			#region level3
			if (level == 3)
			{
				selector = GetRandomSelector(1, 6);

				if (selector == 1)
				{
					List<Vector2> points = new List<Vector2>();
					float[] angle = new float[4];
					angle1 = Random.Range (0, 90);
					do {
						angle[0] = Random.Range (30, 61);
						angle[1] = Random.Range (30, 61);
						angle[2] = Random.Range (30, 61);
						angle[3] = Random.Range (30, 61);
					} while (angle[0] + angle[1] + angle[2] + angle[3] != 180);

					for (int i = 0; i < 4; i++){
						points.Add (MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram * 0.7f));
						angle1 += 180 - 2 * angle[i];  // relative position of vertex
					}
					for (int i = 0; i < 4; i++){
						diagramHelper.AddLinePoint (new LinePoint ("", points[i], points[MathFunctions.AddCyclic(i, 1, 4)]));
					}
					for (int i = 0; i < 3; i++){
						angle2 = angle[i] + angle[MathFunctions.AddCyclic (i, -1, 4)];    // angle at vertex
						diagramHelper.AddAngleArc (new AngleArc ("" + angle2 + MathFunctions.deg, points[i], 180 - angle[i] + angle1, 180 - angle[i] + angle2 + angle1, arcRadius));
						angle1 += 180 - 2 * angle[i];  // relative position of vertex
					}
					angle2 = angle[3] + angle[2];     // angle at vertex
					diagramHelper.AddAngleArc (new AngleArc ("?", points[3], 180 - angle[3] + angle1, 180 - angle[3] + angle2 + angle1, arcRadius, false, false));
					diagramHelper.Draw ();

					QuestionLatext.text = "Find the missing angle.";
					Answer = string.Format ("{0}{1}", angle2, MathFunctions.deg);
				}
				else if(selector == 2)
				{
					SetMCQMode (3);

					float diaAngle = Random.Range (0, 360);
					float chordAngle = diaAngle + Random.Range (70, 110);
					float radiusForFigure = 0.6f * breadthForDiagram;
					float chordDistFromCenter = radiusForFigure - Random.Range (15, 35);
					List<string> partToIdentify = new List<string>() {"centre", "radius", "diameter", "chord"};
					List<string> vertices = new List<string>() {"A", "B", "C", "D", "E"};
					vertices.Shuffle ();

					randSelector = Random.Range (0, 4);
					if (randSelector == 0)  //Centre
					{
						options.Add ("O");
						for (int i = 0; i < 2; i++){
							options.Add (vertices[i]);
						}
						RandomizeMCQOptionsAndFill (options);
						Answer = "O";
					}
					else 
					{
						options.Add ("O" + vertices[0]);   // radius
						options.Add (vertices[1] + vertices[2]);   // diameter
						options.Add (vertices[3] + vertices[4]);   // chord

						Answer = options[randSelector - 1];
						if (randSelector == 3)
						{
							options.RemoveAt (1);
							options.Add (vertices[1] + "O");
						}
						RandomizeMCQOptionsAndFill (options);
					}
					QuestionLatext.text = string.Format ("Identify {0}.", partToIdentify[randSelector]);
					//circle
					diagramHelper.AddAngleArc (new AngleArc ("", Vector2.zero, 0, 360, 2f * radiusForFigure));

					//center
					diagramHelper.AddLinePoint (new LinePoint ("O", Vector2.zero, 0, false, 0f));

					//radius 
					diagramHelper.AddLinePoint (new LinePoint (vertices[0], Vector2.zero, diaAngle + 270, false, radiusForFigure).SetShouldShowDot (false).SetShouldShowDot (false).SetPointTextOffset (MathFunctions.PointAtDirection (Vector2.zero, diaAngle + 270, 15)));

					//diameter
					diagramHelper.AddLinePoint (new LinePoint (vertices[1], Vector2.zero, diaAngle, false, radiusForFigure).SetShouldShowDot (false).SetShouldShowDot (false).SetPointTextOffset (MathFunctions.PointAtDirection (Vector2.zero, diaAngle, 15)));
					diagramHelper.AddLinePoint (new LinePoint (vertices[2], Vector2.zero, diaAngle + 180, false, radiusForFigure).SetShouldShowDot (false).SetPointTextOffset (MathFunctions.PointAtDirection (Vector2.zero, diaAngle + 180, 15)));
				
					//chord
					diagramHelper.AddLinePoint (new LinePoint (vertices[3], MathFunctions.PointAtDirection (Vector2.zero, chordAngle, chordDistFromCenter), 270 + chordAngle, false, Mathf.Sqrt (radiusForFigure * radiusForFigure - chordDistFromCenter * chordDistFromCenter)).SetPointTextOffset (MathFunctions.PointAtDirection (Vector2.zero, chordAngle + 270, 15)).SetShouldShowDot (false));
					diagramHelper.AddLinePoint (new LinePoint (vertices[4], MathFunctions.PointAtDirection (Vector2.zero, chordAngle, chordDistFromCenter), 90 + chordAngle, false, Mathf.Sqrt (radiusForFigure * radiusForFigure - chordDistFromCenter * chordDistFromCenter)).SetPointTextOffset (MathFunctions.PointAtDirection (Vector2.zero, chordAngle + 90, 15)).SetShouldShowDot (false));
				
					diagramHelper.Draw ();
				}
				else if(selector == 3)
				{
					SetMCQMode (2);           

					options.Add ("TThe chord of a circle can be shorter than the radius of a cicle.");
					options.Add ("FThe chord of a circle can not be shorter than the radius of a cicle.");
					options.Add ("TAll the radii of a particular cicle are of equal length.");
					options.Add ("FTwo radii of a particular cicle can be of different lengths.");
					options.Add ("TThe diameter is the longest chord of a cicle.");
					options.Add ("FThe arc is part of the diameter.");
					options.Add ("TThe arc is part of the circumference.");
					options.Add ("FThe diameter is half the circumference.");

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if(selector == 4)
				{
					int radius, diameter;
					string Lunit = Random.Range (1, 3) == 1? " cm" : " m";
					radius = Random.Range (11, 51);
					diameter = 2 * radius;

					if (Random.Range (1, 3) == 1){
						QuestionLatext.text = string.Format ("The {0} of a circle is {2}{3}. Find its {1} (in{3}).", "radius", "diameter", radius, Lunit);
						Answer = string.Format ("{0}", diameter);
					}
					else {
						QuestionLatext.text = string.Format ("The {0} of a circle is {2}{3}. Find its {1} (in{3}).", "diameter", "radius", diameter, Lunit);
						Answer = string.Format ("{0}", radius);
					}
				}
				else if(selector == 5)
				{
					int radius, diameter;
					string Lunit = Random.Range (1, 3) == 1? " cm" : " m";
					radius = Random.Range (11, 51);
					diameter = 2 * radius;
					angle1 = (Random.Range (1, 3) == 1? 90: 270) + Random.Range (-45, 46);
						
					if (Random.Range (1, 3) == 1){
						QuestionLatext.text = string.Format ("Using the given figure, find the {0} (in{1}).", "diameter", Lunit);
						Answer = string.Format ("{0}", diameter);

						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, angle1, false, 0.6f * breadthForDiagram).SetLineType(LineShapeType.Dotted).SetLineText("" + radius + Lunit).SetLineTextDirection (TextDir.Right));
					}
					else {
						QuestionLatext.text = string.Format ("Using the given figure, find the {0} (in{1}).", "radius", Lunit);
						Answer = string.Format ("{0}", radius);

						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (Vector2.zero, 180 + angle1, breadthForDiagram * 0.6f), angle1, false, 1.2f * breadthForDiagram).SetLineType(LineShapeType.Dotted).SetLineText("" + diameter + Lunit).SetLineTextDirection (TextDir.Right));
					}

					diagramHelper.AddAngleArc (new AngleArc ("", Vector2.zero, 0, 360, breadthForDiagram * 1.2f));
					diagramHelper.Draw ();
				}
			}
			#endregion

			userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw>("Text");
			userAnswerLaText.text = "";
			Debug.Log ("" + Answer);
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
			{   // Back
				if (userAnswerLaText.text.Length > 0)
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
			}
			else if (value == 12)
			{   // min
				if (checkLastTextFor(new string[1] {""+ MathFunctions.min }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ""+MathFunctions.min;			
			}
			else if (value == 13)
			{   // Sec

				if (checkLastTextFor(new string[1] { ""+MathFunctions.sec }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ""+MathFunctions.sec;
			}
			else if (value == 14)
			{   // Deg

				if (checkLastTextFor(new string[1] {""+ MathFunctions.deg }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ""+MathFunctions.deg;

			}
			else if (value == 15)
			{   // comma
				if (checkLastTextFor(new string[1] { "," }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ",";
			}
			else if (value == 16) {   // All Clear
				userAnswerLaText.text = "";
			}
		}

		private void SetAnswerValue(float[] answers)
		{
			Answer = "";
			alternateAnswer = "";

			int length = answers.Length;
			for(int i=0;i<length;i++)
			{
				Answer += MathFunctions.GetAngleValueInString (answers[i]);
				alternateAnswer += "" + answers[i] + MathFunctions.deg;

				if ( i < length - 1) 
				{
					Answer += ",";
					alternateAnswer += ",";
				}

			}
		}
		protected void SetMCQMode (int NumberOfMCQ = 4)
		{
			this.MCQ.SetActive (true);
			Vector2[] positions;
			float width = 340f;

			if (NumberOfMCQ == 3) 
			{
				positions = new Vector2[] {
					new Vector2 (-255, 0f),
					new Vector2 (0, 0f),
					new Vector2 (255, 0f),
					new Vector2 (0, 0f)
				};	
				width = 230f;
			} 
			else if (NumberOfMCQ == 2) {
				positions = new Vector2[] {
					new Vector2 (-180, 0f),
					new Vector2 (180, 0f),
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
				MCQ.transform.Find ("Option" + i).GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, MCQ.transform.Find ("Option" + i).GetComponent<RectTransform> ().sizeDelta.y);
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