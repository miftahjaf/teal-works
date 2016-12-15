using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class PolygonsQuadrilateralsAndTheirTypes7 : BaseAssessment
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
			base.Initialise("M", "PQT07", "S01", "A01");

			scorestreaklvls = new int[5];
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
			else if (Answer.Contains (":")){
				var answerSplits = Answer.Split (new string[] { ":" }, System.StringSplitOptions.None);
				if (userAnswerLaText.text.Contains (":")) {
					var userAnswerSplits = userAnswerLaText.text.Split (new string[] { ":" }, System.StringSplitOptions.None);
					if (MathFunctions.checkFractions (answerSplits, userAnswerSplits)) {
						correct = true;
					} else {
						correct = false;
					}
				} else
					correct = false;
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
					UpdateStreak(13, 19);
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
			if (isRevisitedQuestion) {
				return;
			}
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

		void FillMCQOptions (List<string> options)
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
					SetMCQMode (4);
					List<Vector2> centres = new List<Vector2> ();
					List<float> angles0 = new List<float> ();
					List<float> angles1 = new List<float> ();
					List<float> sides = new List<float> ();

					QuestionLatext.text = "Which one of the given figures is a regular polygon?";
					List<int> NumberOfSides = new List<int>() {4, 5, 6, 7, 8, 9, 10}; 
					int numberOfSides = NumberOfSides[Random.Range (0, NumberOfSides.Count)];

					for (int i = 0; i < 3; i++){
						options.Add ("" + (char)(65 + i));
					}
					options.Add ("None");
					FillMCQOptions (options);

					angle1 = 360f / numberOfSides;
					angles0.Add (Random.Range (0, 360)); 
					angles1.Add (Random.Range (0, 360));
					sides.Add (breadthForDiagram * Random.Range (0.4f, 0.7f));

					centres.Add (new Vector2 (- breadthForDiagram * 1.5f, 0));
					centres.Add (Vector2.zero);
					centres.Add (new Vector2 (breadthForDiagram * 1.5f, 0));

					for(int i = 0; i < 3; i++){
						diagramHelper.AddLinePoint (new LinePoint ("" + (char)(65 + i), centres[i], centres[i]).SetShouldShowDot (false));
					}
					centres.Shuffle ();

					if (centres[0].Equals (new Vector2 (- breadthForDiagram * 1.5f, 0))){
						Answer = "A";
					} else if (centres[0].Equals (Vector2.zero)){
						Answer = "B";
					} else if (centres[0].Equals (new Vector2 (breadthForDiagram * 1.5f, 0))){
						Answer = "C";
					}

					for (int i = 1; i < numberOfSides; i++){
						angles0.Add (angles0[0] + i * angle1);
						angles1.Add (angles1[0] + i * angle1 + MathFunctions.GenerateRandomIntegerExcluding0 (-2, 3) * 5);
						sides.Add (breadthForDiagram * Random.Range (0.5f, 0.7f));
					}
					angles0.Add (360 + angles0[0]);
					angles1.Add (360 + angles1[0]);
					sides.Add (sides[0]);

					for (int i = 0; i < numberOfSides; i++){
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (centres[0], angles0[i], breadthForDiagram * 0.6f), MathFunctions.PointAtDirection (centres[0], angles0[i + 1], breadthForDiagram * 0.6f)));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (centres[1], angles1[i], breadthForDiagram * 0.6f), MathFunctions.PointAtDirection (centres[1], angles1[i + 1], breadthForDiagram * 0.6f)));
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (centres[2], angles0[i], sides[i]), MathFunctions.PointAtDirection (centres[2], angles0[i + 1], sides[i + 1])));
					}
					diagramHelper.Draw ();
				}
				else if (selector == 2)
				{
					int numberOfSides = Random.Range (5, 21);
					QuestionLatext.text = string.Format ("A polygon has {0} sides. Find the sum of its interior angles.", numberOfSides);
					Answer = string.Format ("{0}{1}", (numberOfSides - 2) * 180, MathFunctions.deg);
				}
				else if (selector == 3)
				{
					int numberOfSides = Random.Range (5, 21);
					while (360 % numberOfSides != 0)
					{
						numberOfSides = Random.Range (5, 21);
					}
				
					QuestionLatext.text = string.Format ("A regular polygon has {0} sides. Find the measure of each of its exterior angles.", numberOfSides);
					Answer = string.Format ("{0}{1}", 360 / numberOfSides, MathFunctions.deg);
				}
				else if (selector == 4)
				{
					int numberOfSides = Random.Range (5, 21);
					while (((numberOfSides - 2) * 180) % numberOfSides != 0)
					{
						numberOfSides = Random.Range (5, 21);
					}

					QuestionLatext.text = string.Format ("A regular polygon has {0} sides. Find the measure of each of its interior angles.", numberOfSides);
					Answer = string.Format ("{0}{1}", ((numberOfSides - 2) * 180) / numberOfSides, MathFunctions.deg);
				}
				else if (selector == 5)
				{
					int numberOfSides = Random.Range (4, 11);
					QuestionLatext.text = string.Format ("A regular polygon has {0} sides. Find the number of its diagonals.", numberOfSides);
					Answer = string.Format ("{0}", (numberOfSides * (numberOfSides - 3) / 2));
				}
			}
			#endregion
			#region level2
			else if (level == 2)
			{
				selector = GetRandomSelector(1, 6);

				if (selector == 1)
				{
					int numberOfSides = Random.Range (3, 21);
					coeff1 = numberOfSides % 2 == 0? 1: 2; 
					coeff2 = numberOfSides % 2 == 0? (numberOfSides - 2) / 2 : numberOfSides - 2;

					QuestionLatext.text = string.Format ("If the exterior angle and interior angle of a regular polygon are in the ratio \n{0} : {1}, calculate the number of sides in the polygon.", coeff1, coeff2);
					Answer = string.Format ("{0}", numberOfSides);
				}
				else if (selector == 2)
				{
					int numberOfSides = Random.Range (5, 21);
					QuestionLatext.text = string.Format ("If the sum of interior angles of a polygon is {0}{1}, calculate the number of sides in the polygon.", 180 * (numberOfSides - 2), MathFunctions.deg);
					Answer = string.Format ("{0}", numberOfSides);
				}
				else if (selector == 3)
				{
					int numberOfSides = Random.Range (5, 21);
					while (360 % numberOfSides != 0)
					{
						numberOfSides = Random.Range (5, 21);
					}

					QuestionLatext.text = string.Format ("If an exterior angle of a regular polygon is {0}{1}, calculate the number of sides in the polygon.", 360 / numberOfSides, MathFunctions.deg);
					Answer = string.Format ("{0}", numberOfSides);
				}
				else if (selector == 4)
				{
					int numberOfSides = Random.Range (5, 21);
					while ((180 * (numberOfSides - 2)) % numberOfSides != 0)
					{
						numberOfSides = Random.Range (5, 21);
					}

					QuestionLatext.text = string.Format ("If an interior angle of a regular polygon is {0}{1}, calculate the number of sides in the polygon.", (180 * (numberOfSides - 2)) / numberOfSides, MathFunctions.deg);
					Answer = string.Format ("{0}", numberOfSides);
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
			}
			#endregion
			#region level3
			else if (level == 3)
			{
				selector = GetRandomSelector(1, 9);

				if (selector == 1)
				{
					SetMCQMode (2);
					QuestionLatext.text = "Identify the isosceles trapezium.";
					options.Add ("A");
					options.Add ("B");
					FillMCQOptions (options);

					//First Trapezium
					List<Vector2> points = new List<Vector2> ();
					Vector2 offset1, offset2;
					string insideText1, insideText2;

					float lengthForDiagram1 = breadthForDiagram * Random.Range (1.2f, 1.5f);
					do {
						int sign = MathFunctions.GenerateRandomIntegerExcluding0 (-1, 2);
						angle1 = 90 + Random.Range (10, 30) * sign;
						angle2 = 90 - Random.Range (10, 30) * sign;
						lengthForDiagram = lengthForDiagram1 + breadthForDiagram * (1f / Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1f / Mathf.Tan (angle1 * Mathf.Deg2Rad));
					} while (Mathf.Abs (angle1 - angle2) < 10f ||  lengthForDiagram < lengthForDiagram1 / 2f || lengthForDiagram > lengthForDiagram1 * 1.3f || (angle1 + angle2 >= 175 && angle1 + angle2 <= 185));

					if (Random.Range (1, 3) == 1) 
					{
						offset1 = - new Vector2 (30 + (lengthForDiagram > lengthForDiagram1 ? lengthForDiagram : lengthForDiagram1), 0);
						offset2 = new Vector2 (30, 0);
						Answer = "B";
						insideText1 = "A";
						insideText2 = "B";
					} 
					else {
						offset2 = - new Vector2 (30 + (lengthForDiagram > lengthForDiagram1 ? lengthForDiagram : lengthForDiagram1), 0);
						offset1 = new Vector2 (30, 0);
						Answer = "A";
						insideText1 = "B";
						insideText2 = "A";
					}

					if (angle1 < 90)
					{
						points.Add (offset1 + new Vector2 (0, 0));
						points.Add (offset1 + new Vector2 (lengthForDiagram1, 0));
						points.Add (offset1 + new Vector2 (lengthForDiagram1 + breadthForDiagram / Mathf.Tan (angle2 * Mathf.Deg2Rad), breadthForDiagram));
						points.Add (offset1 + new Vector2 (breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
						points.Add (offset1 + new Vector2 (0, 0));
					} 
					else {
						points.Add (offset1 + new Vector2 (- breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), 0));
						points.Add (offset1 + new Vector2 (lengthForDiagram1 - breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), 0));
						points.Add (offset1 + new Vector2 (lengthForDiagram, breadthForDiagram));
						points.Add (offset1 + new Vector2 (0, breadthForDiagram));
						points.Add (offset1 + new Vector2 (- breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), 0));
						}
					Vector2 centre = (points[0] + points[1] + points[2] + points[3]) / 4f;
					diagramHelper.AddLinePoint (new LinePoint (insideText1, centre, centre).SetShouldShowDot (false).SetPointTextOffset (Vector2.zero));

					for (int i = 0; i < points.Count - 1; i++){
						diagramHelper.AddLinePoint (new LinePoint ("", points[i], points[i + 1]));
					}

					//Second Trapezium (isosceles)
					points = new List<Vector2> ();
					do {
						angle1 = 90 + Random.Range (10, 30) * MathFunctions.GenerateRandomIntegerExcluding0 (-1, 2);
						angle2 = 180 - angle1;
						lengthForDiagram = lengthForDiagram1 + breadthForDiagram * (1f / Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1f / Mathf.Tan (angle1 * Mathf.Deg2Rad));
					} while (lengthForDiagram < lengthForDiagram1 / 2f || lengthForDiagram > lengthForDiagram1 * 1.3f);

					if (angle1 < 90)
					{
						points.Add (offset2 + new Vector2 (0, 0));
						points.Add (offset2 + new Vector2 (lengthForDiagram1, 0));
						points.Add (offset2 + new Vector2 (lengthForDiagram1 + breadthForDiagram / Mathf.Tan (angle2 * Mathf.Deg2Rad), breadthForDiagram));
						points.Add (offset2 + new Vector2 (breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
						points.Add (offset2 + new Vector2 (0, 0));
					} 
					else {
						points.Add (offset2 + new Vector2 (- breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), 0));
						points.Add (offset2 + new Vector2 (lengthForDiagram1 - breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), 0));
						points.Add (offset2 + new Vector2 (lengthForDiagram, breadthForDiagram));
						points.Add (offset2 + new Vector2 (0, breadthForDiagram));
						points.Add (offset2 + new Vector2 (- breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), 0));
					}
					centre = (points[0] + points[1] + points[2] + points[3]) / 4f;
					diagramHelper.AddLinePoint (new LinePoint (insideText2, centre, centre).SetShouldShowDot (false).SetPointTextOffset (Vector2.zero));

					for (int i = 0; i < points.Count - 1; i++){
						diagramHelper.AddLinePoint (new LinePoint ("", points[i], points[i + 1]));
					}
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- new Vector2 (0, breadthForDiagram / 2f));
				}
				else if(selector == 2)
				{
					SetMCQMode (2);           

					//normal
					options.Add ("TThe bases are parallel.");
					options.Add ("FThe bases are not parallel.");
					options.Add ("TEach lower base angle is supplementary to the upper base angle on the same side.");
					options.Add ("FEach lower base angle is complementary to the upper base angle on the same side.");

					//isosceles
					options.Add ("TThe non parallel sides are equal.");
					options.Add ("FThe non parallel sides are not equal.");
					options.Add ("TThe lower base angles are equal.");
					options.Add ("FThe lower base angles are unequal.");
					options.Add ("TThe upper base angles are equal.");
					options.Add ("FThe upper base angles are unequal.");
					options.Add ("TThe diagonals are equal.");
					options.Add ("TThe diagonals are unequal.");

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = string.Format ("State whether the given statement about {0} is true or false:\n\n", randSelector > 3? "a trapezium": "an isosceles trapezium");
					QuestionLatext.text += options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if(selector == 3)
				{
					SetMCQMode (2);           

					options.Add ("TAll sides are equal.");
					options.Add ("FAll sides are not necessarily equal.");
					options.Add ("TOpposite sides are parallel.");
					options.Add ("FOpposite sides may not be parallel.");
					options.Add ("TAdjacent angles are supplementary.");
					options.Add ("FAdjacent angles are complementary.");
					options.Add ("TOpposite angles are equal.");
					options.Add ("FOpposite angles are not equal.");
					options.Add ("TThe diagonals bisect each other at right angles.");
					options.Add ("FThe diagonals do not bisect each other at right angles.");

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = "State whether the given statement about a rhombus is true or false:\n\n";
					QuestionLatext.text += options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if(selector == 4)
				{
					SetMCQMode (2);           

					options.Add ("TOpposite sides are parallel.");
					options.Add ("FOpposite sides may not be parallel.");
					options.Add ("TAdjacent angles are supplementary.");
					options.Add ("FAdjacent angles are complementary.");
					options.Add ("TOpposite angles are equal.");
					options.Add ("FOpposite angles are unequal.");
					options.Add ("TOpposite sides are equal.");
					options.Add ("FOpposite sides are unequal.");
					options.Add ("TThe diagonals divide it into two congruent triangles.");
					options.Add ("FThe diagonals do not divide it into two congruent triangles.");
					options.Add ("TThe diagonals bisect each other.");
					options.Add ("FThe diagonals do not bisect each other.");

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = "State whether the given statement about a paralleolgram is true or false:\n\n";
					QuestionLatext.text += options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if(selector == 5)
				{
					SetMCQMode (2);           

					options.Add ("FTwo pairs of opposite sides are parallel.");
					options.Add ("TTwo pairs of adjacent sides are equal.");
					options.Add ("FAdjacent angles are supplementary.");
					options.Add ("FAdjacent angles are complementary.");
					options.Add ("FOpposite angles are equal.");
					options.Add ("TTwo of the opposite angles are equal.");
					options.Add ("FOpposite sides are equal.");
					options.Add ("TOpposite sides are unequal.");

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = "State whether the given statement about a kite is true or false:\n\n";
					QuestionLatext.text += options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if(selector == 6)
				{
					SetMCQMode (2);           

					options.Add ("TOpposite sides are parallel.");
					options.Add ("FOpposite sides may not be parallel.");
					options.Add ("TAdjacent angles are supplementary.");
					options.Add ("FAdjacent angles are complementary.");
					options.Add ("TAll angles are equal.");
					options.Add ("FOpposite angles are unequal.");
					options.Add ("TOpposite sides are equal.");
					options.Add ("FOpposite sides are unequal.");
					options.Add ("TThe diagonals divide it into two congruent triangles.");
					options.Add ("FThe diagonals do not divide it into two congruent triangles.");
					options.Add ("TThe diagonals bisect each other.");
					options.Add ("FThe diagonals do not bisect each other.");

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = "State whether the given statement about a rectangle is true or false:\n\n";
					QuestionLatext.text += options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if(selector == 7)
				{
					SetMCQMode (2);           

					options.Add ("TOpposite sides are parallel.");
					options.Add ("FOpposite sides may not be parallel.");
					options.Add ("TAdjacent angles are supplementary.");
					options.Add ("FAdjacent angles are complementary.");
					options.Add ("TAll angles are equal.");
					options.Add ("FOpposite angles are unequal.");
					options.Add ("TOpposite sides are equal.");
					options.Add ("FOpposite sides are unequal.");
					options.Add ("TAll sides are equal.");
					options.Add ("FAll sides are not necessarily equal.");
					options.Add ("TThe diagonals divide it into two congruent triangles.");
					options.Add ("FThe diagonals do not divide it into two congruent triangles.");
					options.Add ("TThe diagonals bisect each other at right angles.");
					options.Add ("FThe diagonals do not bisect each other at right angles.");

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = "State whether the given statement about a square is true or false:\n\n";
					QuestionLatext.text += options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if(selector == 8)
				{
					SetMCQMode (2);           

					options.Add ("TA square is a rectangle.");
					options.Add ("TA square is a kite.");
					options.Add ("FA rectangle is a kite.");
					options.Add ("FA kite is a square.");
					options.Add ("FA trapezium is a rectangle.");
					options.Add ("TEvery parallelogram is a trapezium.");
					options.Add ("FA rhombus is a square.");
					options.Add ("TA square is a rhombus.");
					options.Add ("TA rhombus is a parallelogram.");
					options.Add ("FA parallelogram is a rhombus.");

					randSelector = Random.Range (0, options.Count);
					QuestionLatext.text = "State whether the given is true or false:\n\n";
					QuestionLatext.text += options [randSelector].Substring (1);

					Answer = options [randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
			}
			#endregion
			#region level4
			else if (level == 4)
			{
				selector = GetRandomSelector(1, 7);

				if (selector == 1)
				{
					List<Vector2> points = new List<Vector2> ();
					float lengthForDiagram1 = breadthForDiagram * Random.Range (1.2f, 1.5f);
					do {
						angle1 = 90 + Random.Range (10, 30) * MathFunctions.GenerateRandomIntegerExcluding0 (-1, 2);
						angle2 = 180 - angle1;
						lengthForDiagram = lengthForDiagram1 + breadthForDiagram * (1f / Mathf.Tan (angle2 * Mathf.Deg2Rad) - 1f / Mathf.Tan (angle1 * Mathf.Deg2Rad));
					} while (Mathf.Abs (angle1 - angle2) < 10f ||  lengthForDiagram < lengthForDiagram1 / 2f);

					QuestionLatext.text = string.Format ("Calculate the measure of \\angle{{Q}}, \\angle{{R}} and \\angle{{S}}.", angle1, MathFunctions.deg);
					Answer = string.Format ("{1}{0},{2}{0},{3}{0}", MathFunctions.deg, angle1, 180 - angle1, 180 - angle1);

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
					origin = new Vector2 (lengthForDiagram / 2, breadthForDiagram / 2);

					diagramHelper.AddLinePoint (new LinePoint ("P", points[3], points[0]).SetShouldShowDot (false).SetSticks (new List<Stick>() {new Stick (1, 0.5f)}).SetPointTextOffset (new Vector2 (-15, -15)));
					diagramHelper.AddLinePoint (new LinePoint ("Q", points[0], points[1],true).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (15, -15)));
					diagramHelper.AddLinePoint (new LinePoint ("R", points[1], points[2]).SetShouldShowDot (false).SetSticks (new List<Stick>() {new Stick (1, 0.5f)}).SetPointTextOffset (new Vector2 (15, 15)));
					diagramHelper.AddLinePoint (new LinePoint ("S", points[2], points[3],true).SetShouldShowDot (false).SetPointTextOffset (new Vector2 (-15, 15)));
					diagramHelper.AddAngleArc (new AngleArc ("" + angle1 + MathFunctions.deg, points[0], 0, angle1, arcRadius));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);
				}
				else if (selector == 2)
				{
					List<Vector2> points = new List<Vector2> ();
					string expression = "";
					angle1 = 90 + 2 * Random.Range (5, 20) * MathFunctions.GenerateRandomIntegerExcluding0 (-1, 2);
					lengthForDiagram = breadthForDiagram / Mathf.Sin (angle1 * Mathf.Deg2Rad);

					points.Add (new Vector2 (0, 0));
					points.Add (new Vector2 (lengthForDiagram, 0));
					points.Add (new Vector2 (lengthForDiagram + breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
					points.Add (new Vector2 (breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad), breadthForDiagram));
					lengthForDiagram += breadthForDiagram / Mathf.Tan (angle1 * Mathf.Deg2Rad);  //to centralise the figure

					origin = new Vector2 (lengthForDiagram / 2, breadthForDiagram / 2);

					diagramHelper.AddLinePoint (new LinePoint ("A", points[3], points[0], true).SetPointTextOffset (new Vector2 (-15, -15)).SetShouldShowDot (false).SetSticks (new List<Stick>() {new Stick (1, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("B", points[0], points[1], true).SetPointTextOffset (new Vector2 (15, -15)).SetShouldShowDot (false).SetSticks (new List<Stick>() {new Stick (1, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("C", points[1], points[2], true).SetPointTextOffset (new Vector2 (15, 15)).SetShouldShowDot (false).SetSticks (new List<Stick>() {new Stick (1, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("D", points[2], points[3], true).SetPointTextOffset (new Vector2 (-15, 15)).SetShouldShowDot (false).SetSticks (new List<Stick>() {new Stick (1, 0.5f)}));
					diagramHelper.AddLinePoint (new LinePoint ("", points[0], points[2]));
					diagramHelper.AddLinePoint (new LinePoint ("", points[1], points[3]));
					diagramHelper.AddLinePoint (new LinePoint ("O", (points[1] + points[3]) / 2, (points[1] + points[3]) / 2).SetPointTextOffset (new Vector2 (0, -15)).SetShouldShowDot (false));
					diagramHelper.AddAngleArc (new AngleArc ("" + (angle1 / 2) + MathFunctions.deg, points[0], 0, angle1 / 2, arcRadius));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- origin);

					randSelector = Random.Range (0, 4);
					if (randSelector == 0){
						expression += "\\angle{OBA}";
						Answer = string.Format ("{0}{1}", 90 - angle1 / 2, MathFunctions.deg);
					}
					else if (randSelector == 1){
						expression += "\\angle{DAB}";
						Answer = string.Format ("{0}{1}", angle1, MathFunctions.deg);
					}
					else if (randSelector == 2){
						expression += "\\angle{BCD}";
						Answer = string.Format ("{0}{1}", angle1, MathFunctions.deg);
					}
					else if (randSelector == 4){
						expression += "\\angle{ABC}";
						Answer = string.Format ("{0}{1}", 180 - angle1, MathFunctions.deg);
					}
					QuestionLatext.text = string.Format ("In the given figure, find {0}.", expression);
				}
				else if (selector == 3)
				{
					do {
						coeff1 = Random.Range (2, 19);
						coeff2 = Random.Range (2, 19);
					} while (180 % (coeff1 + coeff2) != 0 || MathFunctions.GetHCF (coeff1, coeff2) > 1);

					randSelector = Random.Range (1, 3);
					QuestionLatext.text = string.Format ("Two adjacent angles of a parallelogram are in the ratio {0} : {1}. Calculate the measure of the {2} angle.", coeff1, coeff2, randSelector == 1? "smaller" : "greater");

					if (randSelector == 1){
						Answer = string.Format ("{1}{0}", MathFunctions.deg, coeff1 < coeff2? (180 * coeff1) / (coeff1 + coeff2): (180 * coeff2) / (coeff1 + coeff2));
					} else {
						Answer = string.Format ("{1}{0}", MathFunctions.deg, coeff1 > coeff2? (180 * coeff1) / (coeff1 + coeff2): (180 * coeff2) / (coeff1 + coeff2));
					}
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range (11, 100);
					coeff2 = Random.Range (coeff1 / 2, coeff1);

					randSelector = Random.Range (1, 3);
					QuestionLatext.text = string.Format ("Two sides of a kite measure {0} cm and {1} cm. Calculate its perimeter (in cm).", coeff1, coeff2);

					Answer = string.Format ("{0}", 2 * (coeff1 + coeff2));
				}
				else if (selector == 5)
				{
					do {
						coeff1 = Random.Range (2, 21);
						coeff2 = Random.Range (coeff1 / 2, 21);
					} while (MathFunctions.GetHCF (coeff1, coeff2) > 1);

					int perimeter = (coeff1 + coeff2) * Random.Range (3, 10);
					randSelector = Random.Range (1, 3);
					QuestionLatext.text = string.Format ("The sides of a rectangle are in the ratio {0} : {1}. If the perimeter is {2} cm, calculate the measure of its {3} side (in cm).", coeff1, coeff2, perimeter, randSelector == 1? "smaller": "larger");

					if (randSelector == 1){
						Answer = string.Format ("{0}", coeff1 < coeff2? (perimeter * coeff1) / (coeff1 + coeff2): (perimeter * coeff2) / (coeff1 + coeff2));
					} else {
						Answer = string.Format ("{0}", coeff1 > coeff2? (perimeter * coeff1) / (coeff1 + coeff2): (perimeter * coeff2) / (coeff1 + coeff2));
					}
				}
				else if (selector == 6)
				{
					do {
						coeff1 = Random.Range (2, 21);
						coeff2 = Random.Range (coeff1 / 2, 21);
					} while (MathFunctions.GetHCF (coeff1, coeff2) > 1);

					randSelector = Random.Range (1, 3);
					QuestionLatext.text = string.Format ("Two squares have their sides in the ratio {0} : {1}. What is the ratio of their {2}?", coeff1, coeff2, randSelector == 1? "perimeters": "areas");

					if (randSelector == 1){
						Answer = string.Format ("{0}:{1}", coeff1, coeff2);
					} else {
						Answer = string.Format ("{0}:{1}", coeff1 * coeff1, coeff2 * coeff2);
					}
				}
			}
			#endregion
			#region level5
			else if (level == 5)
			{
				selector = GetRandomSelector(1, 3);

				if (selector == 1)
				{
					do {
						coeff1 = Random.Range (2, 10);
						coeff2 = Random.Range (2, 10);
						coeff3 = Random.Range (2, 10);
						coeff4 = Random.Range (2, 10);
					} while (360 % (coeff1 + coeff2 + coeff3 + coeff4) != 0 || MathFunctions.GetHCF (MathFunctions.GetHCF (coeff1, coeff2), MathFunctions.GetHCF (coeff3, coeff4)) > 1);

					int commonRatio = 360 / (coeff1 + coeff2 + coeff3 + coeff4);
					randSelector = Random.Range (1, 3);
					QuestionLatext.text = string.Format ("The angles of a quadrilateral are in the ratio {0} : {1} : {2} : {3}. Calculate the measure of its angles (in the order given in the ratio).", coeff1, coeff2 ,coeff3, coeff4);

					Answer = string.Format ("{1}{0},{2}{0},{3}{0},{4}{0}", MathFunctions.deg, coeff1 * commonRatio, coeff2 * commonRatio, coeff3 * commonRatio, coeff4 * commonRatio);
				}
				else if (selector == 2)
				{
					List<string> polygonName = new List<string>() {"a pentagon", "a hexagon", "a heptagon", "an octagon"};
					List<int> NumberOfSides = new List<int>() {5, 6 ,7 ,8};
					List<int> coeff;
					int sum; 
					randSelector = Random.Range (0, NumberOfSides.Count);
					int numberOfSides = NumberOfSides[randSelector];

					do {
						coeff = new List<int>();
						sum = 0;
						for (int i = 0; i < numberOfSides; i++)
						{
							coeff.Add (Random.Range (1, 10));
							sum += coeff[i];
						}
					} while ((180 * (numberOfSides - 2)) % sum != 0);
						
					string questionText = "The angles of " + polygonName[randSelector] + " are ";

					for (int i = 0; i < numberOfSides - 1; i++){
						questionText+=   MathFunctions.AlgebraicDisplayForm (coeff[i], "\\xalgebra, ", true);
					}
					questionText += "and " + MathFunctions.AlgebraicDisplayForm (coeff[numberOfSides - 1], "\\xalgebra. ", true);
					questionText += "\nFind the value of \\xalgebra.";
					QuestionLatext.text = questionText;
						
					Answer = string.Format ("{0}{1}", (180 * (numberOfSides - 2)) / sum, MathFunctions.deg);
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
			else if (value == 12) {   // All Clear
				userAnswerLaText.text = "";
			}
			else if (value == 13)
			{   // Sec

				if (checkLastTextFor(new string[1] {":"}))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ":";
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