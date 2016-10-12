using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class Circles7 : BaseAssessment {

		private string Answer;
		private List<string> alternateAnswers;
		private float radius;
		private float radiusForFigure;
		private float diameter;
		private float circumference;
		private float area;
		private float diaAngle;
		private string[] expressions;
		private string[] preDefinedAnswersArray;
		private string[] alphabets;
		private int randSelector;
		private string[] Lunits = new string[] {" cm", " m", " ft", " inch"};
		private string Lunit;
		private const float PIvalue = 3.14f;
		private const char PIchar = 'π';

		public TEXDraw subQuestionTEX;
		public Text subQuestionText;
		public DiagramHelper diagramHelper;
		public GameObject MCQ;

		void Start () {

			base.Initialise ("M", "CIR07", "S01", "A01");

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
					if (userAnswerText.text == Answer || alternateAnswers.Contains (userAnswerText.text))
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
			subQuestionText.gameObject.SetActive (false);
			diagramHelper.Reset ();
			SetNumpadMode ();
			alternateAnswers = new List<string> ();

			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) 
				{
					subQuestionText.gameObject.SetActive (true); 
					SetMCQMode (2);

					expressions = new string[] {"Diameter is three times the radius.",
												"Length of a major arc is more than a semicircle.",
												"Diameter is the longest chord.",
												"A circle has infinite diameters."};
					
					preDefinedAnswersArray = new string[] {"False", "True", "True", "True"};

					randSelector = Random.Range (0, expressions.Length);

					QuestionText.text = "State 'True' or 'False'.";
					subQuestionText.text = expressions [randSelector];

					MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "True";
					MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = "False";

					Answer = preDefinedAnswersArray [randSelector];
				}
				else if (selector == 2) 
				{
					subQuestionText.gameObject.SetActive (true); 
					SetMCQMode (2);

					expressions = new string[] {"A tangent cuts the circle at two points.",
												"Radius through the point of contact of a tangent is perpendicular to the tangent.",
												"A secant intersects the circle at two points.",
												"A sector is a region bounded by two chords.",
												"A quadrant is one-fourth of a circle."};

					preDefinedAnswersArray = new string[] {"False", "True", "True", "False", "True"};

					randSelector = Random.Range (0, expressions.Length);

					QuestionText.text = "State 'True' or 'False'.";
					subQuestionText.text = expressions [randSelector];

					MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "True";
					MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = "False";

					Answer = preDefinedAnswersArray [randSelector];
				}
				else if (selector >= 3) 
				{
					expressions = new string[] {"radius", "diameter"};
					Lunit = Lunits [Random.Range (0, Lunits.Length)];
					randSelector = Random.Range (1, 3);	

					radius = Random.Range (11, 100);
					diameter = MathFunctions.GetRounded (2f * radius, 2);

					if (selector == 3)
					{
						QuestionText.text = "If the " + expressions[randSelector == 1 ? 0 : 1] + " of a circle is " + (randSelector == 1 ? radius : diameter) + Lunit + ", find its " + expressions[randSelector == 1 ? 1 : 0] + " (in" + Lunit + ").";
					}
					else if (selector == 4)
					{
						if (Random.Range (1, 3) == 1)
						{
							QuestionText.text = "If the circumference of a circle is " + diameter + PIchar + Lunit + ", find its " + expressions[randSelector == 1 ? 1 : 0] + " (in" + Lunit + ").";
						}
						else
						{
							QuestionText.text = "A circular field has a circumference of " + diameter + PIchar + Lunit + ". Find its " + expressions[randSelector == 1 ? 1 : 0] + " (in" + Lunit + ").";
						}
					}
					else
					{
						if (Random.Range (1, 3) == 1)
						{
							QuestionText.text = "A fiber of length " + diameter + PIchar + Lunit + " is bent to form a circle. What is the " + expressions[randSelector == 1 ? 1 : 0] + " (in" + Lunit + ") of the circle?";
						}
						else
						{
							QuestionText.text = "The tip of the minute hand travels " + diameter + PIchar + Lunit + " in one hour. Find the length of the minute hand (in" + Lunit + ").";
							randSelector = 0; // so that Answer = radius
						}
					}
					Answer = (randSelector == 1 ? diameter : radius).ToString ();
				}
			}
			#endregion
			#region level2
			if (level == 2) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) 
				{
					expressions = new string[] {"diameter", "chord", "secant", "tangent"}; // always put diameter at [0]
					alphabets = new string[] {"A","B","C","D","E","F","G","H"};
					alphabets.Shuffle ();  
					preDefinedAnswersArray = new string[] {alphabets[0] + alphabets[1], alphabets[2] + alphabets[3], alphabets[4] + alphabets[5], alphabets[6] + alphabets[7]}; 
					randSelector = Random.Range (0, expressions.Length);

					radiusForFigure = 70f;
					diaAngle = 360 + Random.Range (-45, 45);
					float chordAngle = diaAngle + Random.Range (70, 110);
					float chordDistFromCenter = radiusForFigure - Random.Range (15, 35);

					//center
					diagramHelper.AddLinePoint (new LinePoint ("O", Vector2.zero, 0, false, 0f));

					//diameter
					diagramHelper.AddLinePoint (new LinePoint (alphabets[0], Vector2.zero, diaAngle, false, radiusForFigure, -1));
					diagramHelper.AddLinePoint (new LinePoint (alphabets[1], Vector2.zero, diaAngle + 180, false, radiusForFigure, 1));

					//chord
					diagramHelper.AddLinePoint (new LinePoint (alphabets[2], MathFunctions.PointAtDirection (Vector2.zero, chordAngle, chordDistFromCenter), 270 + chordAngle, false, Mathf.Sqrt (radiusForFigure * radiusForFigure - chordDistFromCenter * chordDistFromCenter), -1));
					diagramHelper.AddLinePoint (new LinePoint (alphabets[3], MathFunctions.PointAtDirection (Vector2.zero, chordAngle, chordDistFromCenter), 90 + chordAngle, false, Mathf.Sqrt (radiusForFigure * radiusForFigure - chordDistFromCenter * chordDistFromCenter), 1));

					//secant
					chordDistFromCenter = radiusForFigure - Random.Range (15, 35);
					chordAngle += Random.Range (160, 200);
					diagramHelper.AddLinePoint (new LinePoint (alphabets[4], MathFunctions.PointAtDirection (Vector2.zero, chordAngle, chordDistFromCenter), 270 + chordAngle, true, Mathf.Sqrt (radiusForFigure * radiusForFigure - chordDistFromCenter * chordDistFromCenter) + radiusForFigure / 4f, 1));
					diagramHelper.AddLinePoint (new LinePoint (alphabets[5], MathFunctions.PointAtDirection (Vector2.zero, chordAngle, chordDistFromCenter), 90 + chordAngle, true, Mathf.Sqrt (radiusForFigure * radiusForFigure - chordDistFromCenter * chordDistFromCenter) + radiusForFigure / 4f, -1));

					//tangent
					chordDistFromCenter = radiusForFigure;
					chordAngle += Random.Range (100, 150);
					diagramHelper.AddLinePoint (new LinePoint (alphabets[6], MathFunctions.PointAtDirection (Vector2.zero, chordAngle, chordDistFromCenter), 270 + chordAngle, true, 2f * radiusForFigure / 3f, 1));
					diagramHelper.AddLinePoint (new LinePoint (alphabets[7], MathFunctions.PointAtDirection (Vector2.zero, chordAngle, chordDistFromCenter), 90 + chordAngle, true, 2f * radiusForFigure / 3f, -1));

					diagramHelper.AddAngleArc (new AngleArc ("", Vector2.zero, 0, 360, 2 * radiusForFigure));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (0, 20));

					QuestionText.text = "Name the " + expressions [randSelector] + " of the given circle.";
					Answer = preDefinedAnswersArray [randSelector];
					alternateAnswers.Add ("" + preDefinedAnswersArray [randSelector][1] + preDefinedAnswersArray [randSelector][0]);

					if (expressions [randSelector] == "chord")
					{
						QuestionText.text = "Name a " + expressions [randSelector] + " of the given circle.";

						alternateAnswers.Add (alphabets[0] + alphabets[1]);
						alternateAnswers.Add (alphabets[1] + alphabets[0]);
					}
				}
				else if (selector >= 2) 
				{
					Lunit = Lunits [Random.Range (0, Lunits.Length)];
					radius = Random.Range (11, 51);
					diameter = 2f * radius;
					area = MathFunctions.GetRounded (PIvalue * radius * radius, 2);
					circumference = MathFunctions.GetRounded (PIvalue * diameter, 2);
					radiusForFigure = 80f;
					diaAngle = Random.Range (45, 135);

					if (selector == 2 || selector == 4)
						diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, diaAngle, false, radiusForFigure, 0).SetLineType(LineShapeType.Dotted).SetLineText("" + radius + Lunit).SetLineTextDirection (TextDir.Right));
					else
						diagramHelper.AddLinePoint (new LinePoint ("", MathFunctions.PointAtDirection (Vector2.zero, diaAngle, radiusForFigure), 180 + diaAngle, false, 2 * radiusForFigure, 0).SetLineType(LineShapeType.Dotted).SetLineText("" + diameter + Lunit).SetLineTextDirection (TextDir.Right));
					
					diagramHelper.AddAngleArc (new AngleArc ("", Vector2.zero, 0, 360, 2 * radiusForFigure));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (0, 20));

					if (selector == 2 || selector == 3)
					{
						QuestionText.text = "Find the circumference (in" + Lunit + ", use " + PIchar + " = " + PIvalue + ").";
						Answer = circumference.ToString ();
					}
					else
					{
						QuestionText.text = "Find the area (in sq." + Lunit + ", use " + PIchar + " = " + PIvalue + ").";
						Answer = area.ToString ();
					}
				}
			}
			#endregion
			#region level3
			if (level == 3) 
			{
				selector = GetRandomSelector (1, 6);
				Lunit = Lunits [Random.Range (0, Lunits.Length)];

				if (selector <= 2) 
				{
					radius = Random.Range (11, 51);
					diameter = 2f * radius;
					area = MathFunctions.GetRounded (PIvalue * radius * radius, 2);
					circumference = MathFunctions.GetRounded (PIvalue * diameter, 2);
					radiusForFigure = 80f;
					diaAngle = Random.Range (0, 360);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, diaAngle, false, radiusForFigure, 0).SetLineType(LineShapeType.Dotted));
					diagramHelper.AddAngleArc (new AngleArc ("", Vector2.zero, 0, 360, 2 * radiusForFigure));
					diagramHelper.Draw ();
					diagramHelper.ShiftPosition (new Vector2 (0, 20));

					if (selector == 1)
					{
						QuestionText.text = "Given area = " + area + " sq." + Lunit + ". Find the circumference (in" + Lunit + ", use " + PIchar + " = " + PIvalue + ").";
						Answer = circumference.ToString ();
					}
					else
					{
						QuestionText.text = "Given circumference = " + circumference + Lunit + ". Find the area (in sq." + Lunit + ", use " + PIchar + " = " + PIvalue + ").";
						Answer = area.ToString ();
					}	
				}
				else if (selector == 3 || selector == 4) 
				{
					radius = Random.Range (11, 51);
					diameter = 2f * radius;
					area = MathFunctions.GetRounded (PIvalue * radius * radius, 2);
					circumference = MathFunctions.GetRounded (PIvalue * diameter, 2);

					if (selector == 3)
					{
						QuestionText.text = "The circumference of a paddy fied is " + circumference + Lunit + ". What is the area (in sq." + Lunit + ") of the paddy field (use " + PIchar + " = " + PIvalue + ")?";
						Answer = area.ToString ();
					}
					else
					{
						randSelector = Random.Range (2, 10);

						QuestionText.text = "If Nancy runs " + randSelector + " times along the boundary of a circular park and covers a distance of " + (randSelector * circumference) + Lunit + ", what is the area (in sq." + Lunit + ") of the park (use " + PIchar + " = " + PIvalue + ")?";
						Answer = area.ToString ();
					}
				}
				else if (selector == 5) 
				{
					SetMCQMode ();
					diameter = MathFunctions.GetRounded (Random.Range (20, 100), 1);
					radius = MathFunctions.GetRounded (Random.Range (diameter / 2 - 10,diameter / 2 + 10), 1);

					QuestionText.text = "A circle has a diameter of " + diameter + Lunit + ". A point 'P' lies at a distance " + radius + Lunit + " from the centre 'O' of the circle.";

					MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "P lies in the interior of the circle.";
					MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = "P lies in the exterior of the circle.";
					MCQ.transform.Find("Option3").Find("Text").GetComponent<Text>().text = "P lies on the circle.";
					MCQ.transform.Find("Option4").Find("Text").GetComponent<Text>().text = "Cannot be determined.";

					if (Mathf.Abs (diameter / 2f - radius) > 0.0001f)
						Answer = "P lies in the interior of the circle.";
					else if (Mathf.Abs (diameter / 2f - radius) < 0.0001f)
						Answer = "P lies in the exterior of the circle.";
					else
						Answer = "P lies on the circle.";
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