using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class FundamentalConceptsOfGeometry6 : BaseAssessment
	{
		public Text subQuestionText;
		public GameObject MCQ;
		public DiagramHelper diagramHelper;
		private string Answer;
		private string[] expressions;
		private int randSelector;
		private List<string> options;

		void Start()
		{


			StartCoroutine(StartAnimation());
			base.Initialise("M", "FCG06", "S01", "A01");

			scorestreaklvls = new int[1];
			for (var i = 0; i < scorestreaklvls.Length; i++)
			{
				scorestreaklvls[i] = 0;
			}

			levelUp = false;


			Answer = "";
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
					if (userAnswerLaText.text == Answer)
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
					UpdateStreak(10, 15);
					increment = 5;
				}
				else if (Queslevel == 2)
				{
					UpdateStreak(10, 15);
					increment = 10;
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
			//GeneralButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
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
			for (int i = 0; i < 4; i++)
				MCQ.transform.Find ("Option" + (i + 1)).Find ("Text").GetComponent<TEXDraw> ().text = options [i];
		}

		protected override void GenerateQuestion()
		{
			ignoreTouches = false;
			base.QuestionStarted();
			// Generating the parameters

			level = Queslevel;

			subQuestionText.gameObject.SetActive (false);
			SetNumpadMode ();
			options = new List<string> ();
			diagramHelper.Reset ();

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
					SetMCQMode ();
					QuestionText.text = "Which of the following units does not give the measure of a line segment?";

					expressions = new string[] {"millilitre", "litre"};
					options.Add (expressions [Random.Range (0, expressions.Length)]);
					options.Add ("kilometer");
					options.Add ("inch");
					options.Add ("mile");

					Answer = options [0];
					RandomizeMCQOptionsAndFill (options);
				}

				else if (selector == 2)
				{
					SetMCQMode ();
					randSelector = Random.Range (0, 8);
					QuestionText.text = "Pick the option that best describes the given lines.";

					options.Add("Parallel");
					options.Add("Intersecting");
					options.Add("Intersecting and Perpendicular");
					options.Add("Concurrent");
					Answer = options [randSelector / 2];
					FillMCQOptions (options);

					if (randSelector == 0)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (-25, 0), 90, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (-25, 0), 270, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (25, 0), 90, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (25, 0), 270, true, 60));
					} 
					else if (randSelector == 1)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (-65, 0), 130, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (-65, 0), 310, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 130, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 310, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (65, 0), 130, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (65, 0), 310, true, 60));
					}
			
					else if (randSelector == 2)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 30, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 150, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 210, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 330, true, 60));
					}
					else if (randSelector == 3)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 60, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 120, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 240, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 300, true, 60));
					}
				
					else if (randSelector == 4)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 35, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 125, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 305, true, 60));
					}
					else if (randSelector == 5)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 45, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 135, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 225, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 315, true, 60));
					}
					else if (randSelector >= 6)
					{
						float angle1 = Random.Range (0, 90);
						float angle2 = angle1 + Random.Range (40, 70);
						float angle3 = angle2 + Random.Range (40, 70);

						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), angle1, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), angle2, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), angle3, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), angle1 + 180, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), angle2 + 180, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), angle3 + 180, true, 60));
					}

					diagramHelper.ShiftPosition (new Vector2 (0, 65));
					diagramHelper.Draw ();
				}

				else if (selector == 3)
				{
					subQuestionText.gameObject.SetActive (true);
					SetMCQMode (2);

					QuestionText.text = "State True or False :";
					expressions = new string[] {"FA ray has a finite length.",
												"TThere are infinitely many lines on a plane.",
												"FA line has two end points.",
												"FThe lines in a plane always intersect.",
												"TA point has no length and no width.",
												"FA point has thickness.",
												"TAn infinite number of points lie on a circle.",
												"TA ray has one end point",
												"FThe length of a line can be measured.",
												"TThe length of a line can not be measured.",
												"FThe length of a line segment can not be measured.",
												"TThe length of a line segment can be measured.",
												"FA ray extends indefinitely on both sides.",
												"FFinite number of rays can be drawn from a point.",
												"TInfinite number of rays can be drawn from a point.",
												"FThe number of points on a line segment can be counted.",
												"TThe number of points on a line segment can not be counted.",
												"FThe number of points on a line can be counted.",
												"TThe top of a matchbox is an example of a plane.",
												"FInfinite number of lines can be drawn through two points A and B which lie on the same plane.",
												"TOnly one line can be drawn through two points A and B which lie on the same plane.",
												"TParallel lines never intersect.",
												"FTwo planes intersect in a line segment.",
												"TTwo planes intersect in a line.",
												"FConcurrent lines can be parallel to each other."};
					
					randSelector = Random.Range (0, expressions.Length);
					subQuestionText.text = expressions[randSelector].Substring (1);
					Answer = expressions[randSelector][0] == 'T'? "True": "False";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "True";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "False";
				}
				else if (selector == 4)
				{
					randSelector = Random.Range (1, 3);
					expressions = new string[] {"∞How many lines can be drawn through a point?",
												"1How many lines can be drawn through two fixed points?"};

					randSelector = Random.Range (0, expressions.Length);
					QuestionText.text = expressions[randSelector].Substring (1);
					Answer = expressions[randSelector][0].ToString();
				}
				else if (selector == 5)
				{
					expressions = new string[] {string.Format ("∞A line segment AB has length {0} cm. How many points are there on this line segment", Random.Range (2, 10)),
												string.Format ("{0}How many {1} points of intersection can three lines have?", randSelector == 1? 3 : 0, randSelector == 1? "maximum" : "minimum")};

					randSelector = Random.Range (0, expressions.Length);
					QuestionText.text = expressions[randSelector].Substring (1);
					Answer = expressions[randSelector][0].ToString();
				}
				else if (selector == 6)
				{
					SetMCQMode (2);
					randSelector = Random.Range (4, 7);

					QuestionText.text = "Three lines P, Q and R are concurrent and lines R, S and T are also concurrent. Is it necessary that lines P, Q and S will be concurrent?";
					Answer = "No";

					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<TEXDraw> ().text = "Yes";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<TEXDraw> ().text = "No";
				}
			}
			#endregion
			userAnswerLaText = GeneralButton.gameObject.GetChildByName<TEXDraw>("Text");
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
			{    //∞
				if (checkLastTextFor(new string[1] { "∞" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += "∞";
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

