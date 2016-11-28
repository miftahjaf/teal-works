using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class CongruencyOfTriangles7 : BaseAssessment
	{

		public Text subQuestionText;
		public GameObject MCQ;
		public DiagramHelper diagramHelper;
		private string Answer;
		private string alternateAnswer;
		List<string> options;
		private int randSelector;
		private int randSelector1;
		private int sideScaleFactor;
		private int side1;
		private int side2;
		private int side3;
		private float lengthForDiagram;
		private float breadthForDiagram;
		private const float arcRadius = 40f;
		float angle1, angle2, angle3;
		Vector2 origin;
				
		void Start()
		{
			StartCoroutine(StartAnimation());
			base.Initialise("M", "COT07", "S01", "A01");

			scorestreaklvls = new int[2];
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

		void FillMCQOptions(List<string> options)
		{
			int cnt = options.Count;
			for (int i = 1; i <= cnt; i++) {
				MCQ.transform.Find ("Option"+i).Find ("Text").GetComponent<TEXDraw> ().text = options [i - 1];
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
			breadthForDiagram = 120f;

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
				selector = GetRandomSelector(1, 7);

				if (selector == 1)
				{
					SetMCQMode (3);

					QuestionLatext.text = "Pick the option that best describes the given figures :";
					options.Add ("Congruent");
					options.Add ("Similar");
					options.Add ("None");

					randSelector = Random.Range (0, 3);
					Answer = options [randSelector];
					FillMCQOptions (options);

					randSelector1 = Random.Range (0, 3);
					Debug.Log ("RS = " + randSelector + " , RS1 = " + randSelector1);

					if (randSelector == 0)  // Congruent
					{
						if (randSelector1 == 0)  // triangle
						{
							do {
								angle1 = Random.Range (0, 40);
								angle2 = 90 - Random.Range (0, 40);
							} while (angle2 - angle1 < 30);

							angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));
								
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));
						}
						else if (randSelector1 == 1)  // rectangle
						{
							lengthForDiagram = Random.Range (0.5f, 1.5f) * breadthForDiagram;

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), 0, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f + lengthForDiagram, 0), 90, false, breadthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f + lengthForDiagram, breadthForDiagram), 180, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270, false, breadthForDiagram));

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f - lengthForDiagram, 0), 0, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f, 0), 90, false, breadthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f, breadthForDiagram), 180, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f - lengthForDiagram, breadthForDiagram), 270, false, breadthForDiagram));
						}
						else if (randSelector1 == 2)  // circle
						{
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (breadthForDiagram * 5f / 6f, breadthForDiagram / 2f), 0, 360, breadthForDiagram));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (- breadthForDiagram * 5f / 6f, breadthForDiagram / 2f), 0, 360, breadthForDiagram));
						}
					}
					else if (randSelector == 1)  // Similar
					{
						if (randSelector1 == 0)  // triangle
						{
							do {
								angle1 = Random.Range (0, 40);
								angle2 = 90 - Random.Range (0, 40);
							} while (angle2 - angle1 < 30);

							float scaledBreadthForDiagram = breadthForDiagram * Random.Range (0.5f, 0.9f);
							angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (-scaledBreadthForDiagram - breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), angle1, false, scaledBreadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (-scaledBreadthForDiagram - breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), angle2, false, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (-scaledBreadthForDiagram - breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, scaledBreadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));
						}
						else if (randSelector1 == 1)  // rectangle
						{
							lengthForDiagram = Random.Range (0.5f, 1.5f) * breadthForDiagram;

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), 0, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f + lengthForDiagram, 0), 90, false, breadthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f + lengthForDiagram, breadthForDiagram), 180, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270, false, breadthForDiagram));

							float scaleFactor = Random.Range (0.5f, 0.9f);
							float scaledLengthForDiagram = lengthForDiagram * scaleFactor;
							float scaledBreadthForDiagram = breadthForDiagram * scaleFactor;

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f - scaledLengthForDiagram, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 0, false, scaledLengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 90, false, scaledBreadthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180, false, scaledLengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f - scaledLengthForDiagram, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 270, false, scaledBreadthForDiagram));
						}
						else if (randSelector1 == 2)  // circle
						{
							float scaleFactor = Random.Range (0.5f, 0.9f);
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (breadthForDiagram * 5f / 6f, breadthForDiagram / 2f), 0, 360, breadthForDiagram));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (- breadthForDiagram * 5f / 6f, breadthForDiagram / 2f), 0, 360, scaleFactor * breadthForDiagram));
						}
					}
					else if (randSelector == 2)  // None 
					{
						if (randSelector1 == 0)  // triangle & rectangle
						{
							do {
								angle1 = Random.Range (0, 40);
								angle2 = 90 - Random.Range (0, 40);
							} while (angle2 - angle1 < 30);

							angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));
							lengthForDiagram = Random.Range (0.5f, 1.5f) * breadthForDiagram;

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f - lengthForDiagram, 0), 0, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f, 0), 90, false, breadthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f, breadthForDiagram), 180, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram / 3f - lengthForDiagram, breadthForDiagram), 270, false, breadthForDiagram));
						}
						else if (randSelector1 == 1)  // rectangle & circle
						{
							lengthForDiagram = Random.Range (0.5f, 1.5f) * breadthForDiagram;

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, 0), 0, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f + lengthForDiagram, 0), 90, false, breadthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f + lengthForDiagram, breadthForDiagram), 180, false, lengthForDiagram));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270, false, breadthForDiagram));

							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (- breadthForDiagram * 5f / 6f, breadthForDiagram / 2f), 0, 360, breadthForDiagram));
						}
						else if (randSelector1 == 2)  // circle and triangle
						{
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (breadthForDiagram * 5f / 6f, breadthForDiagram / 2f), 0, 360, breadthForDiagram));

							do {
								angle1 = Random.Range (0, 40);
								angle2 = 90 - Random.Range (0, 40);
							} while (angle2 - angle1 < 30);

							angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));
						}
					}
						
					origin = new Vector2 (0, breadthForDiagram / 2f);

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 2) //SSS
				{
					SetMCQMode ();

					QuestionLatext.text = "Pick the correct option about the given triangles :";
					options.Add ("Congruent by SSS");
					options.Add ("Congruent by SAS");
					options.Add ("Congruent by ASA");
					options.Add ("Not Congruent");

					FillMCQOptions (options);
					randSelector = Random.Range (0, 4);
					randSelector1 = Random.Range (0, 2);

					do {
						angle1 = Random.Range (0, 40);
						angle2 = 90 - Random.Range (0, 40);
					} while (angle2 - angle1 < 30);

					angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (2, 0.5f)}));

					if (randSelector1 == 0){
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetSticks (new List<Stick>(){new Stick (3, 0.5f)}));
						Answer = options [0];
					}
					else{
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));
						Answer = options [3];
					}

					if (randSelector == 0)  //mirror image
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (2, 0.5f)}));

						if (randSelector1 == 0){
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetSticks (new List<Stick>(){new Stick (3, 0.5f)}));
						}
						else{
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));
						}
					}
					else if (randSelector == 1) // rotated anticlockwise by 90
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (2, 0.5f)}));

						if (randSelector1 == 0){
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetSticks (new List<Stick>(){new Stick (3, 0.5f)}));
						}
						else{
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));
						}
					}
					else if (randSelector == 2) // rotated by 180
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (2, 0.5f)}));

						if (randSelector1 == 0){
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetSticks (new List<Stick>(){new Stick (3, 0.5f)}));
						}
						else{
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));
						}
					}
					else if (randSelector == 3) // rotated by 270
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (2, 0.5f)}));

						if (randSelector1 == 0){
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetSticks (new List<Stick>(){new Stick (3, 0.5f)}));
						}
						else{
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));
						}
					}

					origin = new Vector2 (0, breadthForDiagram / 2f);

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 3) //SAS
				{
					SetMCQMode ();

					QuestionLatext.text = "Pick the correct option about the given triangles :";
					options.Add ("Congruent by SSS");
					options.Add ("Congruent by SAS");
					options.Add ("Congruent by ASA");
					options.Add ("Not Congruent");

					FillMCQOptions (options);
					randSelector = Random.Range (0, 4);
					do {
						angle1 = Random.Range (0, 40);
						angle2 = 90 - Random.Range (0, 40);
					} while (angle2 - angle1 < 30);

					angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

					randSelector1 = Random.Range (0, 3);
					Debug.Log ("randSelector = " + randSelector + ", randSelector1 = " + randSelector1);

					if (randSelector == 0)  //mirror image
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						if (randSelector1 == 0)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, 180 - angle1));
							Answer = options [1];
						}
						else if (randSelector1 == 1)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, 180 - angle1));
							Answer = options [3];
						}
						else if (randSelector1 == 2)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle2, 360 - angle3));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, 360 - angle2));
							Answer = options [3];
						}
					}
					else if (randSelector == 1) // rotated anticlockwise by 90
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						if (randSelector1 == 0)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1)+ MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, 90 + angle2));
							Answer = options [1];
						}
						else if (randSelector1 == 1)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, 90 + angle2));
							Answer = options [3];
						}
						else if (randSelector1 == 2)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle2, 360 - angle3));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 + angle2, 360 + 90 - angle3));
							Answer = options [3];
						}
					}
					else if (randSelector == 2) // rotated by 180
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						if (randSelector1 == 0)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, 180 + angle2));
							Answer = options [1];
						}
						else if (randSelector1 == 1)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, 180 + angle2));
							Answer = options [3];
						}
						else if (randSelector1 == 2)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle2, 360 - angle3));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle2, 180 - angle3));
							Answer = options [3];
						}
					}
					else if (randSelector == 3) // rotated by 270
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						if (randSelector1 == 0)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, 270 + angle2));
							Answer = options [1];
						}
						else if (randSelector1 == 1) //angles not equal
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2));
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, 270 + angle2));
							Answer = options [3];
						}
						else if (randSelector1 == 2) // !included angle
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle2, 360 - angle3));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 + angle2, 270 - angle3));
							Answer = options [3];
						}
					}
					// sticks to be added to lines	

					origin = new Vector2 (0, breadthForDiagram / 2f);

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 4)  // ASA
				{
					SetMCQMode ();

					QuestionLatext.text = "Pick the correct option about the given triangles :";
					options.Add ("Congruent by SSS");
					options.Add ("Congruent by SAS");
					options.Add ("Congruent by ASA");
					options.Add ("Not Congruent");
					FillMCQOptions (options);

					randSelector = Random.Range (0, 4);

					do {	
						do {
							angle1 = Random.Range (0, 40);
							angle2 = 90 - Random.Range (0, 40);
						} while (angle2 - angle1 < 30);

						angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));
					} while (angle2 - angle1 == (180 - angle2 - (int)angle3) || angle2 - angle1 == (int)angle3 + angle1 || (int)angle3 + angle1 == 180 - angle2 - (int) angle3 || angle1 + angle2 + 2 * angle3 >= 180);

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

					diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2, arcRadius));
					diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle2, 360 - angle3, arcRadius));

					randSelector1 = Random.Range (0, 3);
					Debug.Log ("randSelector = " + randSelector + ", randSelector1 = " + randSelector1);

					if (randSelector == 0)  //mirror image
					{
						if (randSelector1 == 0)
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, 180 - angle1, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, 360 - angle2, arcRadius));
							Answer = options [2];
						}
						else if (randSelector1 == 1)  // angles not equal
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, 180 - angle1, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (int)(180 - angle2 - angle3 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, 360 - angle2, arcRadius));
							Answer = options [3];
						}
						else if (randSelector1 == 2)  // not included side
						{
							float scaleFactor = Mathf.Sin (Mathf.Deg2Rad * (angle3 + angle1)) / Mathf.Sin (Mathf.Deg2Rad * (180 - angle2 - angle3)); //scalefactor < 1 always (checked above)
							float scaledBreadthForDiagram = breadthForDiagram * scaleFactor;
							Debug.Log ("scaleFactor = " + scaleFactor);

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 - angle1, false, scaledBreadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 - angle2, false, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 - angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, scaledBreadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 - angle2, 180 - angle1, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 - angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, 360 - angle2, arcRadius));
							Answer = options [3];
						}
					}
					else if (randSelector == 1) // rotated anticlockwise by 90
					{
						if (randSelector1 == 0)
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, 90 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 + angle2, 360 + 90 - angle3, arcRadius));
							Answer = options [2];
						}
						else if (randSelector1 == 1)  // angles not equal
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, 90 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 + angle2, 360 + 90 - angle3, arcRadius));
							Answer = options [3];
						}
						else if (randSelector1 == 2)  // not included side
						{
							float scaleFactor = Mathf.Sin (Mathf.Deg2Rad * (angle3 + angle1)) / Mathf.Sin (Mathf.Deg2Rad * (180 - angle2 - angle3)); //scalefactor < 1 always (checked above)
							float scaledBreadthForDiagram = breadthForDiagram * scaleFactor;

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 90 + angle1, false, scaledBreadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 90 + angle2, false, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 90 + angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, scaledBreadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 90 + angle1, 90 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, (breadthForDiagram - scaledBreadthForDiagram) / 2f), 90 + angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 + angle2, 360 + 90 - angle3, arcRadius));
							Answer = options [3];
						}
					}
					else if (randSelector == 2) // rotated by 180
					{
						if (randSelector1 == 0)
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, 180 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle2, 180 - angle3, arcRadius));
							Answer = options [2];
						}
						else if (randSelector1 == 1)  // angles not equal
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, 180 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle2, 180 - angle3, arcRadius));
							Answer = options [3];
						}
						else if (randSelector1 == 2)  // not included side
						{
							float scaleFactor = Mathf.Sin (Mathf.Deg2Rad * (angle3 + angle1)) / Mathf.Sin (Mathf.Deg2Rad * (180 - angle2 - angle3)); //scalefactor < 1 always (checked above)
							float scaledBreadthForDiagram = breadthForDiagram * scaleFactor;

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 + angle1, false, scaledBreadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 + angle2, false, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 + angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, scaledBreadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 + angle1, 180 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (scaledBreadthForDiagram + breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 180 + angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle2, 180 - angle3, arcRadius));
							Answer = options [3];
						}
					}
					else if (randSelector == 3) // rotated by 270
					{
						if (randSelector1 == 0)
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, 270 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 + angle2, 270 - angle3, arcRadius));
							Answer = options [2];
						}
						else if (randSelector1 == 1)  // angles not equal
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, 270 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 + angle2, 270 - angle3, arcRadius));
							Answer = options [3];
						}
						else if (randSelector1 == 2)  // not included side
						{
							float scaleFactor = Mathf.Sin (Mathf.Deg2Rad * (angle3 + angle1)) / Mathf.Sin (Mathf.Deg2Rad * (180 - angle2 - angle3)); //scalefactor < 1 always (checked above)
							float scaledBreadthForDiagram = breadthForDiagram * scaleFactor;

							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 270 + angle1, false, scaledBreadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 270 + angle2, false, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 270 + angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, scaledBreadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

							diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 270 + angle1, 270 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, scaledBreadthForDiagram + (breadthForDiagram - scaledBreadthForDiagram) / 2f), 270 + angle2, scaledBreadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 + angle2, 270 - angle3, arcRadius));
							Answer = options [3];
						}
					}

					origin = new Vector2 (0, breadthForDiagram / 2f);

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 5)  //mixed bag of conditions
				{
					SetMCQMode ();

					QuestionLatext.text = "Pick the correct option about the given triangles :";
					options.Add ("Congruent by SSS");
					options.Add ("Congruent by SAS");
					options.Add ("Congruent by ASA");
					options.Add ("Not Congruent");

					FillMCQOptions (options);
					randSelector = Random.Range (0, 4);

					do {	
						do {
							angle1 = Random.Range (0, 40);
							angle2 = 90 - Random.Range (0, 40);
						} while (angle2 - angle1 < 30);

						angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));
					} while (angle2 - angle1 == (180 - angle2 - (int)angle3) || angle2 - angle1 == (int)angle3 + angle1 || (int)angle3 + angle1 == 180 - angle2 - (int) angle3);
						
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

					diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2, arcRadius));
					diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle2 - (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle2, 360 - angle3, arcRadius));

					randSelector1 = Random.Range (0, 2);
					Debug.Log ("randSelector = " + randSelector + ", randSelector1 = " + randSelector1);

					if (randSelector == 0)  //mirror image
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, 180 - angle1, arcRadius));

						if (randSelector1 == 0)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 + (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), 360 - angle1, 360 + angle3, arcRadius));
							Answer = options [2];
						}
						else if (randSelector1 == 1)  // angles not equal
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 + (int)angle3 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), 360 - angle1, 360 + angle3, arcRadius));
							Answer = options [3];
						}
					}
					else if (randSelector == 1) // rotated anticlockwise by 90
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, 90 + angle2, arcRadius));

						if (randSelector1 == 0)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 + (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), 270 - angle3, 270 + angle1, arcRadius));
							Answer = options [2];
						}
						else if (randSelector1 == 1)  // angles not equal
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 + (int)angle3 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), 270 - angle3, 270 + angle1, arcRadius));
							Answer = options [3];
						}
					}
					else if (randSelector == 2) // rotated by 180
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, 180 + angle2, arcRadius));

						if (randSelector1 == 0)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 + (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), 360 - angle3, 360 + angle1, arcRadius));
							Answer = options [2];
						}
						else if (randSelector1 == 1)  // angles not equal
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 + (int)angle3 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), 360 - angle3, 360 + angle1, arcRadius));
							Answer = options [3];
						}
					}
					else if (randSelector == 3) // rotated by 270
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, 270 + angle2, arcRadius));

						if (randSelector1 == 0)
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 + (int)angle3) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), 90 - angle3, 90 + angle1, arcRadius));
							Answer = options [2];
						}
						else if (randSelector1 == 1)  // angles not equal
						{
							diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 + (int)angle3 + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6)) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), 90 - angle3, 90 + angle1, arcRadius));
							Answer = options [3];
						}
					}

					origin = new Vector2 (0, breadthForDiagram / 2f);

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 6) // Corresponding Sides and Vertices
				{
					SetMCQMode (3);

					string[] stringABC = {"A", "B", "C"};
					string[] stringXYZ = {"X", "Y", "Z"};
					stringABC.Shuffle ();
					stringXYZ.Shuffle ();

					randSelector = Random.Range (1, 3);  // 1 for side and 2 for vertex
		
					QuestionLatext.text = string.Format ("Given that \\Delta{{ABC}} and \\Delta{{XYZ}} are congruent, find the {0} corresponding to {1}\n in \\Delta{{XYZ}}.", randSelector == 1? "side": "vertex", randSelector == 1? stringABC[0] + stringABC[1]: stringABC [2]);
					Answer = randSelector == 1? stringXYZ[0] + stringXYZ[1]: stringXYZ [2];

					options.Add (Answer);
					options.Add (randSelector == 1? stringXYZ[1] + stringXYZ[2]: stringXYZ [0]);
					options.Add (randSelector == 1? stringXYZ[2] + stringXYZ[0]: stringXYZ [1]);
					RandomizeMCQOptionsAndFill (options);

					do {
						angle1 = Random.Range (0, 40);
						angle2 = 90 - Random.Range (0, 40);
					} while (angle2 - angle1 < 30);

					angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));

					diagramHelper.AddLinePoint (new LinePoint (stringABC[0], new Vector2 (- 4f * breadthForDiagram / 3f, 0), 0, false, 0).SetPointTextOffset (new Vector2 (-20f, 0)).SetShouldShowDot (false));
					diagramHelper.AddLinePoint (new LinePoint (stringABC[1], new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetPointTextOffset (new Vector2 (20f, 0)).SetShouldShowDot (false));
					diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

					if (randSelector == 1)
					{
						diagramHelper.AddLinePoint (new LinePoint (stringABC[2], new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetPointTextOffset (new Vector2 (15f, 15f)).SetShouldShowDot (false));
						diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2, arcRadius * 1.2f));
						diagramHelper.AddAngleArc (new AngleArc ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle2, 360 - angle3, arcRadius));
					}
					else
					{
						diagramHelper.AddLinePoint (new LinePoint (stringABC[2], new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetPointTextOffset (new Vector2 (15f, 15f)).SetShouldShowDot (false).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2, arcRadius));
					}

					randSelector1 = Random.Range (0, 4);
					Debug.Log ("randSelector = " + randSelector + ", randSelector1 = " + randSelector1);
					if (randSelector1 == 0)  //mirror image
					{
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[0], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 0, false, 0).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (20f, 0)));
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[1], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-20f, 0)));
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15f, 15f)));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						if (randSelector == 1)
						{
							diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15f, 15f)));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, 180 - angle1, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, 180 - angle1, arcRadius * 1.2f));
							diagramHelper.AddAngleArc (new AngleArc ("", MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, 360 - angle2, arcRadius));
						}
						else
						{
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, 180 - angle1, arcRadius));
							diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15f, 15f)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						}
					}
					else if (randSelector1 == 1) // rotated anticlockwise by 90
					{
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[0], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 0, false, 0).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (20f, 0)));
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[1], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15f, 15f)));
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-20f, 0)));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						if (randSelector == 1)
						{
							diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-20f, 0)));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, 90 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, 90 + angle2, arcRadius * 1.2f));
							diagramHelper.AddAngleArc (new AngleArc ("", MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 + angle2, 360 + 90 - angle3, arcRadius));
						}
						else
						{
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle1, 90 + angle2, arcRadius));
							diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-20f, 0)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						}
					}
					else if (randSelector1 == 2) // rotated by 180
					{
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[0], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 0, false, 0).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15f, 15f)));
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[1], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-20f, 0)));
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (20f, 0)));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						if (randSelector == 1)
						{
							diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (20f, 0)));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, 180 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, 180 + angle2, arcRadius * 1.2f));
							diagramHelper.AddAngleArc (new AngleArc ("", MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle2, 180 - angle3, arcRadius));
						}
						else
						{
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, 180 + angle2, arcRadius));
							diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (20f, 0)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						}
					}
					else if (randSelector1 == 3) // rotated by 270
					{
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[0], new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 0, false, 0).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15f, 15f)));
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[1], new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (- 15f, -15f)));
						diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (0, 20f)));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))));

						if (randSelector == 1)
						{
							diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (0, 20f)));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, 270 + angle2, arcRadius));
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, 270 + angle2, arcRadius * 1.2f));
							diagramHelper.AddAngleArc (new AngleArc ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 + angle2, 270 - angle3, arcRadius));
						}
						else
						{
							diagramHelper.AddAngleArc (new AngleArc ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, 270 + angle2, arcRadius));
							diagramHelper.AddLinePoint (new LinePoint (stringXYZ[2], new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (0, 20f)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						}
					}

					origin = new Vector2 (0, breadthForDiagram / 2f);

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
			}
			#endregion
			#region level2

			if (level == 2)
			{
				selector = GetRandomSelector(1, 6);

				if (selector == 1) // using angle properties
				{
					SetMCQMode ();

					QuestionLatext.text = "Pick the correct option about the given triangles :";
					options.Add ("Congruent by SSS");
					options.Add ("Congruent by SAS");
					options.Add ("Congruent by ASA");
					options.Add ("Not Congruent");
					randSelector = Random.Range (1, 3);
					Answer = options [randSelector];
					FillMCQOptions (options);

					origin = new Vector2 (0, breadthForDiagram / 2f);
					angle1 = Random.Range (0, 30);
					angle2 = randSelector == 1? Mathf.Rad2Deg * Mathf.Atan (2 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) : Mathf.Rad2Deg * Mathf.Atan (2);

					if (randSelector == 1)
					{
						randSelector1 = Random.Range (1, 3);
						float fracLocation1 = 0.5f / (2 - Mathf.Tan (angle1 * Mathf.Deg2Rad));
						float fracLocation2 = 0.5f * (3 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) / (2 - Mathf.Tan (angle1 * Mathf.Deg2Rad));
	
						if (randSelector1 == 1)
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, 0), 90 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, 0), 90 + angle2, false, breadthForDiagram / Mathf.Cos (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, fracLocation1), new Stick (2, fracLocation2)}));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram, 0), 90 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram, 0), 90 - angle2, false, breadthForDiagram / Mathf.Cos (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, fracLocation1), new Stick (2, fracLocation2)}));
						}
						else
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, breadthForDiagram), 270 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, breadthForDiagram), 270 - angle2, false, breadthForDiagram / Mathf.Cos (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, fracLocation1), new Stick (2, fracLocation2)}));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram, breadthForDiagram), 270 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Cos (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, fracLocation1), new Stick (2, fracLocation2)}));
						}
					}
					else 
					{
						randSelector1 = Random.Range (1, 3);

						if (randSelector1 == 1)
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, breadthForDiagram), 270 - angle1, true, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, breadthForDiagram), 270 - angle2, false, breadthForDiagram / Mathf.Cos (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, 0.25f), new Stick (1, 0.75f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram, 0), 90 - angle1, true, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram, breadthForDiagram), 270 - angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), MathFunctions.PointAtDirection (new Vector2 (- breadthForDiagram, 0), 90 - angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad))).SetSticks (new List<Stick>(){new Stick (2, 0.25f), new Stick (2, 0.75f)}));
						}
						else 
						{
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, 0), 90 + angle1, true, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, 0), 90 + angle2, false, breadthForDiagram / Mathf.Cos (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick>(){new Stick (1, 0.25f), new Stick (1, 0.75f)}));
							diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- breadthForDiagram, breadthForDiagram), 270 + angle1, true, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)));
							diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram, 0), 90 + angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)), MathFunctions.PointAtDirection (new Vector2 (- breadthForDiagram, breadthForDiagram), 270 + angle1, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad))).SetSticks (new List<Stick>(){new Stick (2, 0.25f), new Stick (2, 0.75f)}));
						}
					}
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 2)
				{
					do {
						angle1 = 2 * Random.Range (1, 25);
						angle2 = 2 * Random.Range ((int) angle1 + 5, 45);
						lengthForDiagram = (1f / Mathf.Tan (angle1 * Mathf.Deg2Rad) + 1f / Mathf.Tan (angle2 * Mathf.Deg2Rad)) * breadthForDiagram;
					} while (lengthForDiagram / breadthForDiagram > 2f || lengthForDiagram / breadthForDiagram < 0.9f);

					QuestionLatext.text = "Given : \\Delta{ABC} is congruent to \\Delta{ABD}. Find \\angle{ABC}.";
					Answer = string.Format ("{1}{0},{2}{0}", MathFunctions.deg, angle1, (180 - angle1 - angle2) / 2);

					origin = new Vector2 (lengthForDiagram, breadthForDiagram) / 2f;
					randSelector = Random.Range (1, 3);

					if (randSelector == 1)
					{
						diagramHelper.AddLinePoint (new LinePoint ("B", Vector2.zero, Vector2.zero).SetShouldShowDot (false));
						diagramHelper.AddLinePoint (new LinePoint ("C", Vector2.zero, new Vector2 (lengthForDiagram, 0)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (0, -20f)));
						diagramHelper.AddLinePoint (new LinePoint ("D", Vector2.zero, angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15f, 15f)));
						diagramHelper.AddLinePoint (new LinePoint ("A", Vector2.zero, angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15f, 15f)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));

						diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, new Vector2 (lengthForDiagram, 0), 180 - angle1, 180, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle1 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (lengthForDiagram, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle1, 360 - angle2, arcRadius));
					}
					else
					{
						diagramHelper.AddLinePoint (new LinePoint ("B", new Vector2 (0, breadthForDiagram), new Vector2 (0, breadthForDiagram)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (0, 20f)));
						diagramHelper.AddLinePoint (new LinePoint ("C", new Vector2 (0, breadthForDiagram), new Vector2 (lengthForDiagram, breadthForDiagram)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (0, 20f)));
						diagramHelper.AddLinePoint (new LinePoint ("D", new Vector2 (0, breadthForDiagram), 360 - angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-15f, -15f)));
						diagramHelper.AddLinePoint (new LinePoint ("A", new Vector2 (0, breadthForDiagram), 360 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-15f, -15f)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));

						diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, new Vector2 (lengthForDiagram, breadthForDiagram), 180, angle1 + 180, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle1 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (lengthForDiagram, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle2, 180 - angle1, arcRadius));
					}
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 3)
				{
					randSelector = Random.Range (0, 4); 
					do {
						do {  
							angle1 = Random.Range (0, 40);
							angle2 = 90 - Random.Range (0, 40);
						} while (angle2 - angle1 < 30);

						angle3 = Mathf.Rad2Deg * Mathf.Atan (((1 - Mathf.Tan (angle1 * Mathf.Deg2Rad)) * Mathf.Tan (angle2 * Mathf.Deg2Rad)) / (Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1));
						sideScaleFactor = Random.Range (5, 20);
						side1 = (int) (breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad) / sideScaleFactor);
						side2 = (int) (breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad) / sideScaleFactor);
						side3 = (int) (breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))) / sideScaleFactor;
					} while (side1 == side2 || side2 == side3 || side3 == side1);

					randSelector1 = Random.Range (0, 2);
					QuestionLatext.text = string.Format ("Find {0}.", randSelector1 == 0? "\\angle{BCA}": "\\angle{CBA}");
					Answer = string.Format ("{0}{1}", "" + (randSelector1 == 0 ? 180 - (int) angle3 - angle2 : angle1 + (int) angle3), MathFunctions.deg);

					diagramHelper.AddLinePoint (new LinePoint ("A", new Vector2 (- 4f * breadthForDiagram / 3f, 0), new Vector2 (-4f * breadthForDiagram / 3f, 0)).SetPointTextOffset (new Vector2 (0, - 15f)).SetShouldShowDot (false));
					diagramHelper.AddLinePoint (new LinePoint ("B", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetPointTextOffset (new Vector2 (15f, 0)).SetLineText (side1 + " cm").SetLineTextDirection (TextDir.Down).SetShouldShowDot (false));
					diagramHelper.AddLinePoint (new LinePoint ("C", new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetPointTextOffset (new Vector2 (- 15f, 0)).SetLineText (side2 + " cm").SetLineTextDirection (TextDir.Left).SetShouldShowDot (false));
					diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 360 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetLineText (side3 + " cm").SetLineTextDirection (TextDir.Right));
					diagramHelper.AddAngleArc (new AngleArc ("" + (angle2 - angle1) + MathFunctions.deg, new Vector2 (- 4f * breadthForDiagram / 3f, 0), angle1, angle2, arcRadius));

					if (randSelector == 0)  //mirror image
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetLineText (side1 + " cm").SetLineTextDirection (TextDir.Down));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetLineText (side2 + " cm").SetLineTextDirection (TextDir.Right));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetLineText (side3 + " cm").SetLineTextDirection (TextDir.Left));
						diagramHelper.AddAngleArc (new AngleArc ("" + (180 - (int) angle3 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle3, 360 - angle2, arcRadius));
					}
					else if (randSelector == 1) // rotated anticlockwise by 90
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetLineText (side1 + " cm").SetLineTextDirection (TextDir.Right));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetLineText (side2 + " cm").SetLineTextDirection (TextDir.Down));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetLineText (side3 + " cm").SetLineTextDirection (TextDir.Up));
						diagramHelper.AddAngleArc (new AngleArc ("" + (180 - (int) angle3 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, 0), 90 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 + angle2, 360 + 90 - angle3, arcRadius));
					}
					else if (randSelector == 2) // rotated by 180
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetLineText (side1 + " cm").SetLineTextDirection (TextDir.Up));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetLineText (side2 + " cm").SetLineTextDirection (TextDir.Right));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram + breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetLineText (side3 + " cm").SetLineTextDirection (TextDir.Left));
						diagramHelper.AddAngleArc (new AngleArc ("" + (180 - (int) angle3 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (4f * breadthForDiagram / 3f, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle2, 180 - angle3, arcRadius));
					}
					else if (randSelector == 3) // rotated by 270
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle1, false, breadthForDiagram / Mathf.Cos (angle1 * Mathf.Deg2Rad)).SetLineText (side1 + " cm").SetLineTextDirection (TextDir.Left));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetLineText (side2 + " cm").SetLineTextDirection (TextDir.Up));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 270 - angle3, false, breadthForDiagram * Mathf.Sqrt (Mathf.Pow (1 - Mathf.Tan (angle1 * Mathf.Deg2Rad), 2) + Mathf.Pow (1 - 1 / Mathf.Tan (angle2 * Mathf.Deg2Rad), 2))).SetLineText (side3 + " cm").SetLineTextDirection (TextDir.Down));
						diagramHelper.AddAngleArc (new AngleArc ("" + (180 - (int) angle3 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (breadthForDiagram / 3f, breadthForDiagram), 270 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 90 + angle2, 270 - angle3, arcRadius));
					}

					origin = new Vector2 (0, breadthForDiagram / 2f);

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 4)
				{
					do {
						angle1 = 2 * Random.Range (1, 25);
						angle2 = 2 * Random.Range ((int) angle1 + 5, 45);
						lengthForDiagram = (1f / Mathf.Tan (angle1 * Mathf.Deg2Rad) + 1f / Mathf.Tan (angle2 * Mathf.Deg2Rad)) * breadthForDiagram;
					} while (lengthForDiagram / breadthForDiagram > 2f || lengthForDiagram / breadthForDiagram < 0.9f);

					QuestionLatext.text = "Find \\xalgebra and \\yalgebra.";
					Answer = string.Format ("{1}{0},{2}{0}", MathFunctions.deg, angle1, (180 - angle1 - angle2) / 2);

					origin = new Vector2 (lengthForDiagram, breadthForDiagram) / 2f;
					randSelector = Random.Range (1, 3);

					if (randSelector == 1)
					{
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, new Vector2 (lengthForDiagram, 0)));
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.3f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.3f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));

						diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, Vector2.zero, 0, angle1, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("\\xalgebra", new Vector2 (lengthForDiagram , 0), 180 - angle1, 180, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("2\\yalgebra", MathFunctions.PointAtDirection (Vector2.zero, angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle2, 360 - angle1, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle1 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (lengthForDiagram, 0), 180 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), 180 + angle1, 360 - angle2, arcRadius));
					}
					else
					{
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadthForDiagram), new Vector2 (lengthForDiagram, breadthForDiagram)));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadthForDiagram), 360 - angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.3f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadthForDiagram), 360 - angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadthForDiagram), 180 + angle1, false, breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (2, 0.3f)}));
						diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadthForDiagram), 180 + angle2, false, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));

						diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, new Vector2 (0, breadthForDiagram), 360 - angle1, 360, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("\\xalgebra", new Vector2 (lengthForDiagram , breadthForDiagram), 180, 180 + angle1, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("2\\yalgebra", MathFunctions.PointAtDirection (new Vector2 (0, breadthForDiagram), 360 - angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle1, 180 - angle2, arcRadius));
						diagramHelper.AddAngleArc (new AngleArc ("" + (180 - angle1 - angle2) + MathFunctions.deg, MathFunctions.PointAtDirection (new Vector2 (lengthForDiagram, breadthForDiagram), 180 + angle2, breadthForDiagram / Mathf.Sin (angle2 * Mathf.Deg2Rad)), angle2, 180 - angle1, arcRadius));
					}
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 5)
				{
					breadthForDiagram = 160f;
					float side;
					do {
						angle1 = Random.Range (40, 80);
						side = 2 * breadthForDiagram / 3f;
						lengthForDiagram = side + breadthForDiagram / (2 * Mathf.Tan (angle1 * Mathf.Deg2Rad));
						angle2 = Mathf.Rad2Deg * Mathf.Atan (breadthForDiagram / (2 * lengthForDiagram));
					} while (angle1 - angle2 < 20f);

					origin = new Vector2 (lengthForDiagram, breadthForDiagram) / 2f;
					sideScaleFactor = Random.Range (2, 10);
					side1 = sideScaleFactor * Random.Range (5, 11);

					QuestionLatext.text = "Given : \\angle{ACB} is congruent to \\angle{DCB}. Find \\xalgebra and \\yalgebra.";
					Answer = string.Format ("{0},{1}{2}", side1 / sideScaleFactor, angle1 - (int) angle2, MathFunctions.deg);

					diagramHelper.AddLinePoint (new LinePoint ("B", Vector2.zero, new Vector2 (lengthForDiagram, breadthForDiagram / 2f)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (20f, 0)).SetLineText ("" + side1).SetLineTextDirection (TextDir.Right));
					diagramHelper.AddLinePoint (new LinePoint ("D", new Vector2 (lengthForDiagram - side, breadthForDiagram / 2f), Vector2.zero).SetShouldShowDot (false).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}).SetPointTextOffset (new Vector2 (-20f, 0)));
					diagramHelper.AddLinePoint (new LinePoint ("C", new Vector2 (lengthForDiagram, breadthForDiagram / 2f), new Vector2 (lengthForDiagram - side, breadthForDiagram / 2f)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-20f, 0)));
					diagramHelper.AddLinePoint (new LinePoint ("A", new Vector2 (lengthForDiagram, breadthForDiagram / 2f), new Vector2 (0, breadthForDiagram)).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-20f, 0)).SetLineText (sideScaleFactor + "\\xalgebra").SetLineTextDirection (TextDir.Right));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadthForDiagram), new Vector2 (lengthForDiagram - side, breadthForDiagram / 2f)).SetSticks (new List<Stick> (){new Stick (1, 0.5f)}));

					diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 - (int) angle2) + MathFunctions.deg, Vector2.zero, angle2, angle1, arcRadius * 1.5f));
					diagramHelper.AddAngleArc (new AngleArc ("\\yalgebra", new Vector2 (0, breadthForDiagram), 360 - angle1, 360 - angle2, arcRadius * 1.5f));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
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
					new Vector2 (-180, 0f),
					new Vector2 (180, 0f),
					new Vector2 (-180, -80f),
					new Vector2 (180, -80f)
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