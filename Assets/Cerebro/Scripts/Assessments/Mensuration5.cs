using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class Mensuration5 : BaseAssessment {

		private int Answer;
		private int length;
		private int breadth;
		private int side;
		private int triHeight;
		private int triBase;
		private int perimeter;
		private int area;

		public TEXDraw subQuestionTEX;
		public DiagramHelper diagramHelper;

		void Start () {

			base.Initialise ("M", "MEN05", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = 0;
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
			int answer = 0;
			if(int.TryParse(userAnswerText.text,out answer)) {
				answer = int.Parse (userAnswerText.text);
			}

			if (answer == Answer) {
				correct = true;

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
				} else if (Queslevel == 6) {
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
			userAnswerText.color = MaterialColor.red800;
			Go.to( userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake( new Vector3( 0, 0, 20 ), GoShakeType.Eulers ) );
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				userAnswerText.text = "";
				userAnswerText.color = MaterialColor.textDark;
				ignoreTouches = false;
			} else {
				userAnswerText.text = Answer.ToString ();
				userAnswerText.color = MaterialColor.green800;
			}
			ShowContinueButton ();
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
			GeneralButton.gameObject.SetActive (true);
			subQuestionTEX.gameObject.SetActive (false);
			numPad.SetActive (true);
			diagramHelper.Reset ();

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 6);
				subQuestionTEX.gameObject.SetActive (true);

				if (selector == 1) 
				{
					length = Random.Range (3, 11);
					breadth = Random.Range (2, length);
					perimeter = 2 * (length + breadth);

					QuestionText.text = "Find the perimeter (in cm).";
					subQuestionTEX.text = "Length = " + length + " cm, Breadth = " + breadth + " cm";

					Answer = perimeter;
				}
				else if (selector == 2) 
				{
					side = Random.Range (2, 11);
					perimeter = 4 * side;

					QuestionText.text = "Find the perimeter (in cm).";
					subQuestionTEX.text = "Side = " + side + " cm";

					Answer = perimeter;
				}
				else if (selector == 3) 
				{
					length = Random.Range (3, 11);
					breadth = Random.Range (2, length);
					area = length * breadth;

					QuestionText.text = "Find the area (in sq. cm).";
					subQuestionTEX.text = "Length = " + length + " cm, Breadth = " + breadth + " cm";

					Answer = area;
				}
				else if (selector == 4) 
				{
					side = Random.Range (2, 11);
					area = side * side;

					QuestionText.text = "Find the area (in sq. cm).";
					subQuestionTEX.text = "Side = " + side + " cm";

					Answer = area;
				}
				else if (selector == 5) 
				{
					subQuestionTEX.gameObject.SetActive (false);

					
				}
			}
			#endregion
			#region level2
			if (level == 2) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) 
				{
					subQuestionTEX.gameObject.SetActive (true);

					length = Random.Range (3, 11);
					breadth = Random.Range (2, length);
					area = length * breadth;

					QuestionText.text = "Find the missing data.";
					subQuestionTEX.text = "Area = " + area + " m^{2}, Length = " + length + " m, Breadth = ? m";

					Answer = breadth;
				}
				else if (selector == 2) 
				{
					subQuestionTEX.gameObject.SetActive (true);

					side = Random.Range (2, 11);
					area = side * side;

					QuestionText.text = "Find the missing data.";
					subQuestionTEX.text = "Area = " + area + " m^{2}, Side = ? m";

					Answer = side;
				}
				else if (selector == 3) 
				{
					subQuestionTEX.gameObject.SetActive (true);

					length = Random.Range (3, 11);
					breadth = Random.Range (2, length);
					area = length * breadth;

					QuestionText.text = "Find the missing data.";
					subQuestionTEX.text = "Area = " + area + " m^{2}, Breadth = " + breadth + " m, Length = ? m";

					Answer = length;
				}
				else if (selector == 4) 
				{
					length = Random.Range (30, 110);
					breadth = Random.Range (length / 2, length - 5);
					perimeter = 2 * (length + breadth);

					float lengthForDiagram = 50f * length / breadth;
					float breadthForDiagram = 50f;

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- lengthForDiagram, - breadthForDiagram), 0, false, 2 * lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, - breadthForDiagram), 90, false, 2 * breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadthForDiagram), 180, false, 2 * lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- lengthForDiagram, breadthForDiagram), 270, false, 2 * breadthForDiagram));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (0, 20));

					QuestionText.text = "Find the perimeter (in cm) of the given figure.";

					Answer = perimeter;	
				}
				else if (selector == 5) 
				{
					side = Random.Range (10, 51);
					perimeter = 4 * side;

					float sideForDiagram = 50f;

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- sideForDiagram, - sideForDiagram), 0, false, 2 * sideForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (sideForDiagram, - sideForDiagram), 90, false, 2 * sideForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (sideForDiagram, sideForDiagram), 180, false, 2 * sideForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- sideForDiagram, sideForDiagram), 270, false, 2 * sideForDiagram));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (0, 20));

					QuestionText.text = "Find the perimeter (in cm) of the given figure.";

					Answer = perimeter;	
				}
			}
			#endregion
			#region level3
			if (level == 3) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) 
				{
					length = Random.Range (30, 110);
					breadth = Random.Range (length / 2, length - 5);
					area = length * breadth;

					float lengthForDiagram = 50f * length / breadth;
					float breadthForDiagram = 50f;

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- lengthForDiagram, - breadthForDiagram), 0, false, 2 * lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, - breadthForDiagram), 90, false, 2 * breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadthForDiagram), 180, false, 2 * lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- lengthForDiagram, breadthForDiagram), 270, false, 2 * breadthForDiagram));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (0, 20));

					QuestionText.text = "Find the area (in sq. cm) of the given figure.";

					Answer = area;
				}
				else if (selector == 2) 
				{
					side = Random.Range (10, 51);
					area = side * side;

					float sideForDiagram = 50f;

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- sideForDiagram, - sideForDiagram), 0, false, 2 * sideForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (sideForDiagram, - sideForDiagram), 90, false, 2 * sideForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (sideForDiagram, sideForDiagram), 180, false, 2 * sideForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (- sideForDiagram, sideForDiagram), 270, false, 2 * sideForDiagram));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (0, 20));

					QuestionText.text = "Find the area (in sq. cm) of the given figure.";

					Answer = area;
				}
				else if (selector == 3) 
				{
					length = Random.Range (10, 26);
					breadth = Random.Range (2, length / 2);
					area = length * breadth;

					QuestionText.text = "A projector screen in a movie hall is " + length + " m long and " + breadth + " m broad. What is the area (in sq. m) of the screen?";

					Answer = area;
				}
				else if (selector == 4) 
				{
					length = Random.Range (20, 50);
					breadth = Random.Range (5, length);
					area = length * breadth;
					perimeter = 2 * (length + breadth);

					QuestionText.text = "The perimeter of a room is " + perimeter + " m. If its length is " + length + " m, find its area (in sq. m).";

					Answer = area;
				}
				else if (selector == 5) 
				{
					length = Random.Range (25, 100);
					breadth = Random.Range (length - 15, length);
					area = length * breadth;

					QuestionText.text = "The area of a painting is " + area + " sq. cm. If its breadth is " + breadth + " cm, what is the length (in cm) of the painting?";

					Answer = length;
				}
			}
			#endregion
			#region level4
			if (level == 4) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) 
				{
					breadth = Random.Range (20, 100);
					length = Random.Range(2, 6) * breadth;
					area = length * breadth;

					QuestionText.text = "The length of a garden is " + (length / breadth) + " times its breadth. If its breadth is " + breadth + " m, what is the area (in sq. m) of the garden?";

					Answer = area;
				}
				else if (selector == 2) 
				{
					side = Random.Range (8, 16);
					area = side * side;

					QuestionText.text = "The area of a square handkerchief is " + area + " sq. cm. What is the measurement of its side (in cm)?";

					Answer = side;
				}
				else if (selector == 3) 
				{
					length = Random.Range (2, 10); 
					int length1 = Random.Range (2, 10);
					breadth = Random.Range (2, 8);
					int breadth1 = Random.Range (breadth + 2, 10);
					area = length1 * breadth1 + length * breadth;

					float lengthForDiagram = 120f * length / breadth1;
					float length1ForDiagram = 120f * length1 / breadth1;
					float breadthForDiagram = 120f * breadth / breadth1;
					float breadth1ForDiagram = 120f;

					Vector2 Origin = new Vector2 ((lengthForDiagram + length1ForDiagram) / 2, breadth1ForDiagram / 2);

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 0, false, length1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram + length1ForDiagram, 0), 90, false, breadth1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram + length1ForDiagram, breadth1ForDiagram), 180, false, length1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadth1ForDiagram), 270, false, breadth1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadth1ForDiagram), 180, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadth1ForDiagram), 270, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadth1ForDiagram - breadthForDiagram), 0, false, lengthForDiagram));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- Origin);

					QuestionText.text = "Find the area of the given figure (in sq. cm).";

					Answer = area;
				}
				else if (selector == 4) 
				{
					int length1 = Random.Range (2, 10);
					int length2 = Random.Range (2, 10);
					length = length1 + length2 + Random.Range (1, 5);
					int breadth1 = Random.Range (2, 8);
					int breadth2 = Random.Range (breadth1 + 2, 10);
					breadth = Random.Range (2, 5);
					area = length1 * breadth1 + length * breadth + length2 * breadth2;

					float lengthForDiagram = 120f * length / (breadth + breadth2);
					float length1ForDiagram = 120f * length1 / (breadth + breadth2);
					float breadthForDiagram = 120f * breadth / (breadth + breadth2);
					float breadth1ForDiagram = 120f * breadth1 / (breadth + breadth2);
					float length2ForDiagram = 120f * length2 / (breadth + breadth2);
					float breadth2ForDiagram = 120f * breadth2 / (breadth + breadth2);

					Vector2 Origin = new Vector2 (lengthForDiagram / 2, (breadthForDiagram + breadth2ForDiagram) / 2);

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadthForDiagram + breadth2ForDiagram), 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadthForDiagram + breadth2ForDiagram), 270, false, breadthForDiagram + breadth2ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, breadth2ForDiagram), 180, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadth2ForDiagram), 90, false, breadthForDiagram));

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 180, false, length2ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram - length2ForDiagram, 0), 90, false, breadth2ForDiagram));

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram - length2ForDiagram, breadth2ForDiagram - breadth1ForDiagram), 180, false, length1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram - length2ForDiagram - length1ForDiagram, breadth2ForDiagram - breadth1ForDiagram), 90, false, breadth1ForDiagram));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- Origin);

					QuestionText.text = "Find the area of the given figure (in sq. cm).";

					Answer = area;
				}
				else if (selector == 5) 
				{
					length = Random.Range (5, 16);
					breadth = Random.Range (length - 2, length + 3);
					area = 4 * length * breadth + breadth * breadth;

					float lengthForDiagram = 120f * length / (2 * length + breadth);
					float breadthForDiagram = 120f * breadth / (2 * length + breadth);

					Vector2 Origin = new Vector2 ((2 * lengthForDiagram + breadthForDiagram) / 2,(2 * lengthForDiagram + breadthForDiagram) / 2);

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 0, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram + breadthForDiagram, 0), 90, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram + breadthForDiagram, lengthForDiagram), 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (2 * lengthForDiagram + breadthForDiagram, lengthForDiagram), 90, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (2 * lengthForDiagram + breadthForDiagram, lengthForDiagram + breadthForDiagram), 180, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram + breadthForDiagram, lengthForDiagram + breadthForDiagram), 90, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram + breadthForDiagram, 2 * lengthForDiagram + breadthForDiagram), 180, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 2 * lengthForDiagram + breadthForDiagram), 270, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, lengthForDiagram + breadthForDiagram), 180, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, lengthForDiagram + breadthForDiagram), 270, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, lengthForDiagram), 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, lengthForDiagram), 270, false, lengthForDiagram));

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, lengthForDiagram), 0, false, breadthForDiagram).SetLineType(LineShapeType.Dotted));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram + breadthForDiagram, lengthForDiagram), 90, false, breadthForDiagram).SetLineType(LineShapeType.Dotted));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram + breadthForDiagram, lengthForDiagram + breadthForDiagram), 180, false, breadthForDiagram).SetLineType(LineShapeType.Dotted));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, lengthForDiagram), 90, false, breadthForDiagram).SetLineType(LineShapeType.Dotted));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- Origin);

					QuestionText.text = "Find the area of the given figure (in sq. cm).";

					Answer = area;
				}
			}
			#endregion
			#region level5
			if (level == 5) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) 
				{
					int length1 = Random.Range (2, 6);
					length = Random.Range (length1 + 2, 10);
					int breadth1 = Random.Range (length1 + 1, length1 + 5);
					breadth = 2 * breadth1 + Random.Range (2, breadth1);
					area = 2 * length1 * breadth + length * (breadth - 2 * breadth1);

					float lengthForDiagram = 120f * length / breadth;
					float breadthForDiagram = 120f;
					float length1ForDiagram = 120f * length1 / breadth;
					float breadth1ForDiagram = 120f * breadth1 / breadth;

					Vector2 Origin = new Vector2 ((2 * length1ForDiagram + lengthForDiagram) / 2, breadthForDiagram / 2);

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, 0), 0, false, length1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram, 0), 90, false, breadth1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram, breadth1ForDiagram), 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram + lengthForDiagram, breadth1ForDiagram), 270, false, breadth1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram + lengthForDiagram, 0), 0, false, length1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (2 * length1ForDiagram + lengthForDiagram, 0), 90, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (2 * length1ForDiagram + lengthForDiagram, breadthForDiagram), 180, false, length1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram + lengthForDiagram, breadthForDiagram), 270, false, breadth1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram + lengthForDiagram, breadthForDiagram - breadth1ForDiagram), 180, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram, breadthForDiagram - breadth1ForDiagram), 90, false, breadth1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram, breadthForDiagram), 180, false, length1ForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadthForDiagram), 270, false, breadthForDiagram));

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram, breadth1ForDiagram), 90, false, (breadthForDiagram - 2 * breadth1ForDiagram)).SetLineType(LineShapeType.Dotted));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (length1ForDiagram + lengthForDiagram, breadth1ForDiagram), 90, false, (breadthForDiagram - 2 * breadth1ForDiagram)).SetLineType(LineShapeType.Dotted));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- Origin);

					QuestionText.text = "Find the area of the given figure (in sq. cm).";

					Answer = area;

				}
				else if (selector == 2) 
				{
					triBase = Random.Range (10, 30);
					triHeight = Random.Range (triBase - 5, triBase + 5);
				
					while ((triBase * triHeight) % 2 != 0)
						triHeight = Random.Range (triBase - 5, triBase + 5);

					area = (triBase * triHeight) / 2;

					float triBaseForDiagram = 120f * triBase / triHeight;
					float triHeightForDiagram = 120f;

					Vector2 Origin = new Vector2 (triBaseForDiagram / 2, triHeightForDiagram / 2);

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, 0), 0, false, triBaseForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (triBaseForDiagram, 0), 180f - Mathf.Rad2Deg * Mathf.Atan ((2f * triHeight) / triBase), false,  Mathf.Sqrt (triHeightForDiagram * triHeightForDiagram + triBaseForDiagram * triBaseForDiagram / 4f)));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (triBaseForDiagram / 2f, triHeightForDiagram), 180f + Mathf.Rad2Deg * Mathf.Atan ((2f * triHeight) / triBase), false,  Mathf.Sqrt (triHeightForDiagram * triHeightForDiagram + triBaseForDiagram * triBaseForDiagram / 4f)));

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (triBaseForDiagram / 2f, 0), 90, false, triHeightForDiagram).SetLineType(LineShapeType.Dotted));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- Origin);

					QuestionText.text = "Find the area of the given figure (in sq. cm).";

					Answer = area;
				}
				else if (selector == 3) 
				{
					triBase = Random.Range (10, 30);
					triHeight = Random.Range (triBase - 5, triBase + 5);

					while ((triBase * triHeight) % 2 != 0)
						triHeight = Random.Range (triBase - 5, triBase + 5);

					area = (triBase * triHeight) / 2;

					float triBaseForDiagram = 120f * triBase / triHeight;
					float triHeightForDiagram = 120f;

					Vector2 Origin = new Vector2 (triBaseForDiagram / 2, triHeightForDiagram / 2);

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, 0), 0, false, triBaseForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (triBaseForDiagram, 0), 180f - Mathf.Rad2Deg * Mathf.Atan (triHeightForDiagram / triBaseForDiagram), false,  Mathf.Sqrt (triHeightForDiagram * triHeightForDiagram + triBaseForDiagram * triBaseForDiagram)));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, triHeightForDiagram), 270, false, triHeightForDiagram));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- Origin);

					QuestionText.text = "Find the area of the given figure (in sq. cm).";

					Answer = area;
				}
				else if (selector == 4) 
				{
					breadth = Random.Range (2, 10);
					length = 3 * breadth;
					area = 6 * breadth * breadth;

					float lengthForDiagram = 120f;
					float breadthForDiagram = 120f / 3f;

					Vector2 Origin = new Vector2 (lengthForDiagram / 2, lengthForDiagram / 2);

					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, 0), 0, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, 0), 90, false, lengthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram, lengthForDiagram), 180, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram - breadthForDiagram, lengthForDiagram), 270, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (lengthForDiagram - breadthForDiagram, lengthForDiagram - breadthForDiagram), 180, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, lengthForDiagram - breadthForDiagram), 270, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (breadthForDiagram, breadthForDiagram), 180, false, breadthForDiagram));
					diagramHelper.AddLinePoint (new LinePoint ("", new Vector2 (0, breadthForDiagram), 270, false, breadthForDiagram));

					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (- Origin);

					QuestionText.text = "Find the area of the given figure (in sq. cm).";

					Answer = area;
				}
				else if (selector == 5) 
				{
					triBase = Random.Range (3, 20);
					triHeight = Random.Range (triBase, 2 * triBase);

					while ((triBase * triHeight) % 2 != 0)
						triHeight = Random.Range (triBase, 2 * triBase);

					area = (triBase * triHeight) / 2;
					
					QuestionText.text = "A triangular flag's base measures " + triBase + " m and its height is " + triHeight + " cm. What is the area of one face of the flag?";

					Answer = area;
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
			} 
		}
	}
}
