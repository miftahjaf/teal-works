using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class Triangles6 : BaseAssessment
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
		private int coeff5;
		private int coeff6;
		private float lengthForDiagram;
		private const float breadthForDiagram = 120f;
		float angle1, angle2;
		Vector2 origin;

		void Start()
		{


			StartCoroutine(StartAnimation());
			base.Initialise("M", "TRI06", "S01", "A01");

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

					QuestionLatext.text = "Identify :";
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

					QuestionLatext.text = "Identify :";
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
					SetMCQMode (2);           

					options.Add (string.Format ("TSum of the interior angles of a triangle is always 180{0}.", MathFunctions.deg));
					options.Add (string.Format ("FA triangle can have more than one right angle."));
					options.Add (string.Format ("TSum of the acute angles of a right angled triangle is 90{0}.", MathFunctions.deg));
					options.Add (string.Format ("TAn obtuse angled triangle cannot have an angle measuring 90{0}.", MathFunctions.deg));
					options.Add (string.Format ("FAn obtuse angled triangle can have two obtuse angles."));
					options.Add (string.Format ("FExterior angle of a triangle is equal to the sum of its interior angles."));
					options.Add (string.Format ("TA right angled triangle can be isosceles."));
					options.Add (string.Format ("TAn equilateral triangle is always acute angled."));
					options.Add (string.Format ("FAright angled triangle can be equilateral."));
					options.Add (string.Format ("FA triangle can have each of its angles less than 60{0}.", MathFunctions.deg));
					options.Add (string.Format ("TAn isosceles triangle can be equilateral."));

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if (selector == 4)
				{
					angle1 = Random.Range (30, 70);
					QuestionLatext.text = string.Format ("In a right angled triangle, one of the acute angles is {0}{1}. Find the other acute angle.", angle1, MathFunctions.deg);
					Answer = string.Format ("{0}{1}", 90 - angle1, MathFunctions.deg);
				}
				else if (selector == 5)
				{
					angle1 = Random.Range (30, 120); 
					angle2 = Random.Range (30, 150 - (int) angle1);

					QuestionLatext.text = string.Format ("The measures of two angles of a triangle are {0}{2} and {1}{2}. Find the third angle.", angle1, angle2, MathFunctions.deg);
					Answer = string.Format ("{0}{1}", 180 - angle1 - angle2, MathFunctions.deg);
				}
			}
			#endregion
			#region level2

			if (level == 2)
			{
				selector = GetRandomSelector(1, 6);

				if(selector == 1)
				{
					angle1 = Random.Range (40, 70); 
					angle2 = Random.Range (20, 50);
					coeff1 = Random.Range (3, 10);

					QuestionLatext.text = string.Format ("In the given figure, calculate \\angle{{R}}.");
					Answer = string.Format ("{0}{1}", angle1, MathFunctions.deg);

					lengthForDiagram = breadthForDiagram * (2f / Mathf.Tan (angle1 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("R", Vector2.zero, 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("P", new Vector2 (lengthForDiagram, 0), 180f - angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetLineText(coeff1 + " cm").SetLineTextDirection(TextDir.Right));
					diagramHelper.AddLinePoint (new LinePoint ("Q", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180 + angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetLineText(coeff1 + " cm").SetLineTextDirection(TextDir.Left));
					diagramHelper.AddLinePoint (new LinePoint ("S", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180 + angle1 + angle2, false, breadthForDiagram / Mathf.Sin ((angle1 + angle2) * Mathf.Deg2Rad)));

					diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, Vector2.zero, 0f, angle1));
					diagramHelper.AddAngleArc (new AngleArc ("" + angle2 + MathFunctions.deg, MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1, 180f + angle1 + angle2, 0, false, false));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 2)
				{
					coeff1 = Random.Range (2, 10);
					do{
						coeff2 = Random.Range (2, 10);
					} while (MathFunctions.GetHCF (coeff1, coeff2) > 1);

					coeff3 = Random.Range (10, 180 / (coeff1 + coeff2)); // common ratio = x
					angle1 = 180 - (coeff1 + coeff2) * coeff3; 
					angle2 = coeff1 * coeff3;

					QuestionLatext.text = "Find \\xalgebra :";
					Answer = string.Format ("{0}{1}", coeff3, MathFunctions.deg);

					lengthForDiagram = breadthForDiagram * (1f / Mathf.Tan (angle1 * Mathf.Deg2Rad) + 1f / Mathf.Tan (angle2 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));

					diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, Vector2.zero, 0f, angle1));
					diagramHelper.AddAngleArc (new AngleArc (coeff1 + "\\xalgebra", new Vector2 (lengthForDiagram, 0), 180f - angle2, 180f, 0, false, false));
					diagramHelper.AddAngleArc (new AngleArc (coeff2 + "\\xalgebra", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1, 360f - angle2, 0, false, false));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 3)
				{
					do {
						coeff1 = Random.Range (2, 10);
						do{
							coeff2 = Random.Range (2, 10);
						} while (MathFunctions.GetHCF (coeff1, coeff2) > 1);

						coeff3 = Random.Range (10, 180 / (coeff1 + coeff2)); // common ratio = x
						angle2 = 180 - (coeff1 + coeff2) * coeff3; 
						angle1 = coeff1 * coeff3;

					} while (angle1 < 30f || angle2 < 30f);  // to maintain reasonable aspect ratio of the figure

					QuestionLatext.text = "Find \\xalgebra :";
					Answer = string.Format ("{0}{1}", coeff3, MathFunctions.deg);

					lengthForDiagram = breadthForDiagram * (1f / Mathf.Tan (angle1 * Mathf.Deg2Rad) + 1f / Mathf.Tan (angle2 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, false, 1.5f * lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));

					diagramHelper.AddAngleArc (new AngleArc (coeff1 + "\\xalgebra", Vector2.zero, 0f, angle1, 0, false, false));
					diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2) + MathFunctions.deg, new Vector2 (lengthForDiagram, 0), 0f, 180f - angle2));
					diagramHelper.AddAngleArc (new AngleArc (coeff2 + "\\xalgebra", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1, 360f - angle2, 0, false, false));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 4)
				{
					angle1 = Random.Range (40, 70); 
					angle2 = Random.Range (20, 50);
					coeff1 = Random.Range (3, 10);

					QuestionLatext.text = string.Format ("In the given figure, calculate \\angle{{SPR}}.");
					Answer = string.Format ("{0}{1}", 180 - (2 * angle1 + angle2), MathFunctions.deg);

					lengthForDiagram = breadthForDiagram * (2f / Mathf.Tan (angle1 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("R", Vector2.zero, 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("P", new Vector2 (lengthForDiagram, 0), 180f - angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetLineText(coeff1 + " cm").SetLineTextDirection(TextDir.Right));
					diagramHelper.AddLinePoint (new LinePoint ("Q", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180 + angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetLineText(coeff1 + " cm").SetLineTextDirection(TextDir.Left));
					diagramHelper.AddLinePoint (new LinePoint ("S", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180 + angle1 + angle2, false, breadthForDiagram / Mathf.Sin ((angle1 + angle2) * Mathf.Deg2Rad)));

					diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, Vector2.zero, 0f, angle1));
					diagramHelper.AddAngleArc (new AngleArc ("" + angle2 + MathFunctions.deg, MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1, 180f + angle1 + angle2, 0, false, false));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if(selector == 5)
				{
					coeff1 = Random.Range (2, 10);
					do{
						coeff2 = Random.Range (2, 10);
					} while (MathFunctions.GetHCF (coeff1, coeff2) > 1);

					coeff3 = Random.Range (10, 180 / (coeff1 + coeff2)); // common ratio
					angle1 = 180 - (coeff1 + coeff2) * coeff3;
					randSelector = Random.Range (1, 3);

					QuestionLatext.text = string.Format ("One of the angles of a triangle is {0}{3} and the other two angles are in the ratio \n{1} : {2}. Find the {4} of the two unknown angles.", angle1, coeff1, coeff2, MathFunctions.deg, randSelector == 1? "smaller": "greater");
					Answer = string.Format ("{0}{1}", randSelector == 1? Mathf.Min (coeff1 * coeff3, coeff2 * coeff3) : Mathf.Max (coeff1 * coeff3, coeff2 * coeff3), MathFunctions.deg);
				}
			}
			#endregion
			#region level3
			if (level == 3)
			{

				selector = GetRandomSelector(1, 6);

				if (selector == 1)
				{int count = 1;
					do{
						coeff1 = Random.Range (1, 7);
						do {
							coeff3 = Random.Range (6, 19);
						} while (180 % coeff3 != 0);

						coeff2 = Random.Range (1, coeff3 - coeff1);
						coeff4 = 180 / coeff3; // common ratio
						coeff3 = coeff3 - (coeff1 + coeff2);

						count ++;
						if (count == 1000){
							Debug.Log ("Loop FASA");
							break;
						}
					} while (MathFunctions.GetHCF (coeff1, coeff2, coeff3) > 1 || coeff3 == coeff2 || coeff1 == coeff3 || coeff2 == coeff1);
						
					angle1 = 180 - (coeff1 + coeff2) * coeff3;
					randSelector = Random.Range (1, 3);

					QuestionLatext.text = string.Format ("The angles of a triangle are in the ratio {0} : {1} : {2}. Find the {3} angle.", coeff1, coeff2, coeff3, randSelector == 1? "smallest": "greatest");
					Answer = string.Format ("{0}{1}", randSelector == 1? Mathf.Min (Mathf.Min (coeff1 * coeff4, coeff2 * coeff4), coeff3 * coeff4) : Mathf.Max (Mathf.Max (coeff1 * coeff4, coeff2 * coeff4), coeff3 * coeff4), MathFunctions.deg);
				}
				else if (selector == 2)
				{
					angle1 = Random.Range (30, 70);

					QuestionLatext.text = string.Format ("Find \\xalgebra, if AB = AC.");
					Answer = string.Format ("{0}{1}", 180 - 2 * angle1, MathFunctions.deg);

					lengthForDiagram = breadthForDiagram * (2f / Mathf.Tan (angle1 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("C", Vector2.zero, 0, false, 1.5f * lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("A", new Vector2 (lengthForDiagram, 0), 180f - angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("B", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180 + angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));

					diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle1) + MathFunctions.deg, new Vector2 (lengthForDiagram, 0), 0f, 180f - angle1));
					diagramHelper.AddAngleArc (new AngleArc ("\\xalgebra", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1, 360f - angle1, 0 , false, false));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if(selector == 3)
				{
					angle1 = Random.Range (30, 50);
					angle2 = Random.Range (50, 80);
					coeff1 = 180 - (int) angle1 - (int) angle2 - Random.Range (20, 35); // angle3
					coeff2 = 180 - (int) angle1 - (int) angle2 - coeff1;  // angle4

					QuestionLatext.text = string.Format ("Find \\xalgebra.");
					Answer = string.Format ("{0}{1}", 180 - 2 * angle1, MathFunctions.deg);

					lengthForDiagram = breadthForDiagram * (1f / Mathf.Tan (angle1 * Mathf.Deg2Rad) + 1f / Mathf.Tan (angle2 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, false, 1.5f * lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1 + coeff1, false, breadthForDiagram / Mathf.Sin ((angle2 + coeff2) * Mathf.Deg2Rad)));

					diagramHelper.AddAngleArc (new AngleArc ("\\xalgebra", Vector2.zero, 0f, angle1));
					diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2) + MathFunctions.deg, new Vector2 (lengthForDiagram, 0), 0f, 180f - angle2));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff1 + MathFunctions.deg, MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1, 180f + angle1 + coeff1, 0, false, false));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff2 + MathFunctions.deg, MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f + angle1 + coeff1, 180f + angle1 + coeff1 + coeff2, 0, false, false));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if(selector == 4)
				{
					angle1 = 2 * Random.Range (40, 66);
					QuestionLatext.text = string.Format ("One of the angles of an isosceles triangle measures {0}{1}. Find the measure of the remaining angles.", angle1, MathFunctions.deg);
					Answer = string.Format ("{0}{1}", 180 - (int) angle1 / 2, MathFunctions.deg);
				}
				else if(selector == 5)
				{
					angle1 = Random.Range (40, 70);

					QuestionLatext.text = "Find \\angle{AEB}, if AE = BE.";
					Answer = string.Format ("{0}{1}", 180 - 2 * angle1, MathFunctions.deg);

					lengthForDiagram = breadthForDiagram * (2f / Mathf.Tan (angle1 * Mathf.Deg2Rad));
					origin = new Vector2 (lengthForDiagram / 2f, breadthForDiagram / 2f);

					diagramHelper.AddLinePoint (new LinePoint ("B", Vector2.zero, 0, true, 1.5f * lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("E", new Vector2 (lengthForDiagram, 0), 180f - angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("A", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180 + angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("D", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 0, true, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("C", MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 180f, false, 0.5f * lengthForDiagram));

					diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, MathFunctions.PointAtDirection (Vector2.zero, angle1, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)), 360f - angle1, 360f));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
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