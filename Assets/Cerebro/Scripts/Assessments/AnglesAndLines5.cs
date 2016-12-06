using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class AnglesAndLines5 : BaseAssessment
	{

		public Text subQuestionText;
		public GameObject MCQ;
		public DiagramHelper diagramHelper;
		private string Answer;
		private string alternateAnswer;
		private int coeff1;
		private int coeff2;
		private int coeff3;
		private int coeff4;
		private int coeff5;
		private int coeff6;
		private int upflag = 0;
	
		void Start()
		{
			

			StartCoroutine(StartAnimation());
			base.Initialise("M", "AAL05", "S01", "A01");

			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++)
			{
				scorestreaklvls[i] = 0;
			}

			levelUp = false;

			coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = coeff6 = 0;

			Answer = "";//
			alternateAnswer = "";
			GenerateQuestion();
		}

		public override void SubmitClick()
		{
			upflag = 0;
			if (ignoreTouches || userAnswerLaText.text == "")
			{
				return;
			}
			upflag = 0;
			numPad.transform.Find ("PanelLayer").Find ("angle").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");			int increment = 0;
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
					directCheck = true;
					//correct = false;
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
					UpdateStreak(8, 12);
					increment = 10;
				}
				else if (Queslevel == 3)
				{
					UpdateStreak(10, 15);
					increment = 15;
				}
				else if (Queslevel == 4)
				{
					UpdateStreak(8, 12);
					increment = 15;
				}
				else if (Queslevel == 5)
				{
					UpdateStreak(8, 12);
					increment = 15;
				}
				else if (Queslevel == 6)
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
			for (int i = 0; i < 4; i++)
				MCQ.transform.Find ("Option" + (i + 1)).Find ("Text").GetComponent<TEXDraw> ().text = options [i];
		}

		protected override void GenerateQuestion()
		{

			Vector2 newPoint;
			float newRadius;

			ignoreTouches = false;
			base.QuestionStarted();
			// Generating the parameters

			level = Queslevel;
			List<string> options = new List<string>();

			answerButton = GeneralButton;
			subQuestionText.gameObject.SetActive(false);
			QuestionText.gameObject.SetActive(false);
			QuestionLatext.gameObject.SetActive(true);
			SetNumpadMode ();
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
				selector = GetRandomSelector(1, 6);
				if (selector == 1)
				{
					if (Random.Range(1, 3) == 1)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (-25, 0), 90, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (-25, 0), 270, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (25, 0), 90, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (25, 0), 270, true, 60));
					} 
					else
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (-65, 0), 130, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (-65, 0), 310, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 130, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 310, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (65, 0), 130, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (65, 0), 310, true, 60));
					}
					diagramHelper.ShiftPosition (new Vector2 (0, 30));
					diagramHelper.Draw ();
					QuestionLatext.text = "What type of lines are the following?";

					SetMCQMode ();
					options.Add("Parallel");
					options.Add("Intersecting");
					options.Add("Perpendicular");
					options.Add("Intersecting and Perpendicular");
					Answer = options[0];
					FillMCQOptions (options);

				}
				else if (selector == 2)
				{
					if (Random.Range(1, 3) == 1)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 30, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 150, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 210, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 330, true, 60));
					}
					else
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 60, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 120, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 240, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 300, true, 60));
					}
					diagramHelper.ShiftPosition (new Vector2 (0, 30));
					diagramHelper.Draw ();
					QuestionLatext.text = "What type of lines are the following?";

					SetMCQMode ();
					options.Add("Parallel");
					options.Add("Intersecting");
					options.Add("Perpendicular");
					options.Add("Intersecting and Perpendicular");
					Answer = options[1];
					FillMCQOptions (options);

				}
				else if (selector == 3)
				{
					if (Random.Range(1, 3) == 1)
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 35, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 125, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 305, true, 60));
					}
					else
					{
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 45, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 135, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 225, true, 60));
						diagramHelper.AddLinePoint (new LinePoint("" , new Vector2 (0, 0), 315, true, 60));
					}
					diagramHelper.ShiftPosition (new Vector2 (0, 30));
					diagramHelper.Draw ();
					QuestionLatext.text = "What type of lines are the following?";

					SetMCQMode ();
					options.Add("Parallel");
					options.Add("Intersecting");
					options.Add("Perpendicular");
					options.Add("Intersecting and Perpendicular");
					Answer = options[3];
					FillMCQOptions (options);

				}
				else if (selector == 4)
				{
					coeff1 = 10 * Random.Range(2,36);
					if (coeff1 == 180 || coeff1 >= 270)
						coeff1 = 90;
					QuestionLatext.text = "If \\angle{A} = " + coeff1 + MathFunctions.deg + ", what type of angle is \\angle{A}?";
					SetMCQMode ();
					options.Add("Acute");
					options.Add("Obtuse");
					options.Add("Right");
					options.Add("Reflex");
					if (coeff1 < 90)
						Answer = options[0];
					else if (coeff1 == 90)
						Answer = options[2];
					else if (coeff1 > 90 && coeff1 < 180)
						Answer = options[1];
					else
						Answer = options[3];
					FillMCQOptions (options);

				}
				else if (selector == 5)
				{
					QuestionLatext.text = "What is the measure of a whole angle?";
					Answer = "360" + MathFunctions.deg;

				}
			}
			#endregion
			#region level2

			if (level == 2)
			{
				selector = GetRandomSelector(1, 6);

				if(selector == 1)
				{
					coeff1 = Random.Range (30, 90);
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 270 - coeff1, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 270, true, 100));
					diagramHelper.AddAngleArc (new AngleArc ("?", Vector2.zero, 270, 630 - coeff1));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff1 + MathFunctions.deg, Vector2.zero, 270 - coeff1, 270));

					diagramHelper.ShiftPosition (new Vector2 (0, 65));
					diagramHelper.Draw ();

					QuestionLatext.text = "Find the missing angle.";
					Answer = (360 - coeff1).ToString () + MathFunctions.deg;

				}
				else if(selector == 2)
				{
					float randAngle = Random.Range (-50, 50);
					float COB = Random.Range (30, 80);
					float AOB = Random.Range (30, 80);
					float AOE = Random.Range (30, 80);
					float EOD = Random.Range (30, 80);
					diagramHelper.AddLinePoint (new LinePoint ("C", Vector2.zero, randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("B", Vector2.zero, COB + randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("A", Vector2.zero, AOB + COB + randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("E", Vector2.zero, AOE + AOB + COB + randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("D", Vector2.zero, EOD + AOE + AOB + COB + randAngle, true, 100));

					diagramHelper.ShiftPosition (new Vector2 (0, 20));
					diagramHelper.Draw ();
					QuestionLatext.text = "In the given figure, which of the angles are adjacent?";

					SetMCQMode ();
					options.Add("\\angle{AOB} and \\angle{AOE}");
					options.Add("\\angle{BOC} and \\angle{AOE}");
					options.Add("\\angle{EOD} and \\angle{BOC}");
					options.Add("\\angle{DOC} and \\angle{AOB}");
					Answer = options[0];
					RandomizeMCQOptionsAndFill (options);

				}
				else if (selector > 2)
				{
					float randAngle = Random.Range (-50, 50);
					float AOB = Random.Range (30, 70);
					while (AOB == 45)
						AOB = Random.Range (30, 70);

					diagramHelper.AddLinePoint (new LinePoint ("O", Vector2.zero, 0, false, 0));
					diagramHelper.AddLinePoint (new LinePoint ("A", Vector2.zero, randAngle, true, 90));
					diagramHelper.AddLinePoint (new LinePoint ("B", Vector2.zero, AOB + randAngle, true, 90));
					diagramHelper.AddLinePoint (new LinePoint ("C", Vector2.zero, 90f + randAngle, true, 90));
					diagramHelper.AddLinePoint (new LinePoint ("D", Vector2.zero, 180f + randAngle, true, 90));
					diagramHelper.AddLinePoint (new LinePoint ("E", Vector2.zero, 180f + AOB + randAngle, true, 90));

					diagramHelper.ShiftPosition (new Vector2 (0, 20));
					diagramHelper.Draw ();

					if (selector == 3)
					{
						QuestionLatext.text = "Name the complement of \\angle{AOB}?";
						Answer = "\\angle{BOC}";
						alternateAnswer = "\\angle{COB}";
					} 
					else if (selector == 4)
					{
						QuestionLatext.text = "Name the supplement of \\angle{AOB}?";
						Answer = "\\angle{DOB}";
						alternateAnswer = "\\angle{BOD}";
					}
					else if (selector == 5)
					{
						QuestionLatext.text = "Name the vertically opposite angle of \\angle{AOB}?";
						Answer = "\\angle{DOE}";
						alternateAnswer = "\\angle{EOD}";
					}

				}
			}
			#endregion
			#region level3
			if (level == 3)
			{

				selector = GetRandomSelector(1, 7);

				if (selector < 4)
				{
					SetMCQMode ();

					float randAngle = 90 + Random.Range (-50, 50);
					float AOE = Random.Range (30, 70);
					while (AOE == 45)
						AOE = Random.Range (30, 70);
					
					diagramHelper.AddLinePoint (new LinePoint ("O", Vector2.zero, 0, false, 0));
					diagramHelper.AddLinePoint (new LinePoint ("A", Vector2.zero, randAngle, true, 90));
					diagramHelper.AddLinePoint (new LinePoint ("E", Vector2.zero, AOE + randAngle, true, 90));
					diagramHelper.AddLinePoint (new LinePoint ("B", Vector2.zero, AOE - 90 + randAngle, true, 90));
					diagramHelper.AddLinePoint (new LinePoint ("D", Vector2.zero, 90 + AOE + randAngle, true, 90));
					diagramHelper.AddLinePoint (new LinePoint ("C", Vector2.zero, 180 + randAngle, true, 90));

					diagramHelper.Draw ();

					QuestionLatext.text = "Given : AC and BD are straight lines and OE is perpendicular to BD.\n";

					if (selector == 1)
					{
						QuestionLatext.text += "Identify complementary angles.";
						options.Add("\\angle{AOB} and \\angle{AOE}");
						options.Add("\\angle{AOD} and \\angle{AOB}");
						options.Add("\\angle{AOE} and \\angle{DOC}");
						options.Add("\\angle{DOC} and \\angle{AOB}");
						Answer = options[0];
					}
					if (selector == 2)
					{
						QuestionLatext.text += "Identify supplementary angles.";
						options.Add("\\angle{AOB} and \\angle{AOD}");
						options.Add("\\angle{EOD} and \\angle{DOC}");
						options.Add("\\angle{AOE} and \\angle{AOB}");
						options.Add("\\angle{BOE} and \\angle{BOC}");
						Answer = options[0];
					}
					if (selector == 3)
					{
						QuestionLatext.text += "Identify vertically opposite angles.";
						options.Add("\\angle{AOB} and \\angle{DOC}");
						options.Add("\\angle{EOD} and \\angle{BOC}");
						options.Add("\\angle{AOE} and \\angle{BOC}");
						options.Add("\\angle{DOC} and \\angle{AOE}");
						Answer = options[0];
					}

					RandomizeMCQOptionsAndFill (options);

				}
				else if(selector == 4)
				{
					coeff1 = Random.Range (11, 80);
					QuestionLatext.text = "Find the complement of " + coeff1 + MathFunctions.deg + ".";
					Answer = (90 - coeff1) + "" + MathFunctions.deg;
				}
				else if(selector == 5)
				{
					coeff1 = Random.Range (11, 170);
					QuestionLatext.text = "Find the supplementary angle of " + coeff1 + MathFunctions.deg + ".";
					Answer = (180 - coeff1) + "" + MathFunctions.deg;
				}
				else if(selector == 6)
				{
					coeff1 = Random.Range (11, 170);
					QuestionLatext.text = "Find the value of the vertically opposite angle of \\angle{XYZ}, if : \n \\angle{XYZ} = " + coeff1 + MathFunctions.deg + ".";
					Answer = coeff1 + "" + MathFunctions.deg;
				}
			}
			#endregion
			#region level4
			if (level == 4) 
			{
				selector = GetRandomSelector (1, 5);
				QuestionLatext.text = "Find the missing angle.";

				if (selector == 1) 
				{
					float randAngle = Random.Range (-50, 50);
					coeff1 = Random.Range (50, 130);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, coeff1 + randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 180 + randAngle, true, 100));

					diagramHelper.AddAngleArc (new AngleArc ("" + coeff1 + MathFunctions.deg, Vector2.zero, randAngle, randAngle + coeff1));
					diagramHelper.AddAngleArc (new AngleArc ("?", Vector2.zero, randAngle + coeff1, randAngle + 180));

					diagramHelper.Draw ();

					Answer = "" + (180 - coeff1) + MathFunctions.deg;

				} 
				else if(selector == 2)
				{
					coeff1 = Random.Range (25, 70);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 0, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, coeff1 , true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 90 , true, 100));

					diagramHelper.AddAngleArc (new AngleArc ("" + coeff1 + MathFunctions.deg, Vector2.zero, 0, coeff1));
					diagramHelper.AddAngleArc (new AngleArc ("?", Vector2.zero, coeff1, 90));

					diagramHelper.ShiftPosition (new Vector2 (-30, -30));
					diagramHelper.Draw ();

					Answer = "" + (90 - coeff1) + MathFunctions.deg;

				}
				else if(selector == 3)
				{
					float randAngle = Random.Range (10, 80);
					coeff1 = Random.Range (50, 130);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, coeff1 + randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 180 + randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, 180 + coeff1 + randAngle, true, 100));

					diagramHelper.AddAngleArc (new AngleArc ("" + coeff1 + MathFunctions.deg, Vector2.zero, 180 + randAngle, 180 + randAngle + coeff1));
					diagramHelper.AddAngleArc (new AngleArc ("?", Vector2.zero, randAngle, randAngle + coeff1));

					diagramHelper.Draw ();

					Answer = "" + coeff1 + MathFunctions.deg;

				}
				else if(selector == 4)
				{
					float randAngle = Random.Range (10, 80);
					coeff1 = Random.Range (50, 130);

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle - coeff1, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + 180, true, 100));

					diagramHelper.AddAngleArc (new AngleArc ("?", Vector2.zero, 360 + randAngle - coeff1, 360 + randAngle));
					diagramHelper.AddAngleArc (new AngleArc ("" + (180 - coeff1) + MathFunctions.deg, Vector2.zero, randAngle + 180, 360 + randAngle - coeff1));

					diagramHelper.Draw ();

					Answer = "" + coeff1 + MathFunctions.deg;

				}
			}
			#endregion
			#region level 5
			if (level == 5) 
			{
				selector = GetRandomSelector(1, 6);
				if(selector == 1)
				{
					coeff1 = Random.Range (11, 85);
					QuestionLatext.text = "An angle measures " + (180 - coeff1) + MathFunctions.deg + ". Find the measure of the complement of its supplementary angle.";
					Answer = (90 - coeff1) + "" + MathFunctions.deg;
						
				}
				else if(selector == 2)
				{
					coeff1 = Random.Range (11, 85);
					QuestionLatext.text = "The complement of an angle is " + coeff1 + MathFunctions.deg + ". Find the value of an angle that is vertically opposite to its supplement.";
					Answer = (90 + coeff1) + "" + MathFunctions.deg;
				}
				else if(selector == 3)
				{
					float randAngle = 0f;
					coeff1 = Random.Range (30, 70);
					coeff2 = Random.Range (30, 70);

					QuestionLatext.text = "Find the missing angle.";
					Answer = (180 - coeff1 - coeff2) + "" + MathFunctions.deg;

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + 180, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + 180 + coeff1, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + 360 - coeff2, true, 100));

					diagramHelper.AddAngleArc (new AngleArc ("" + coeff1 + MathFunctions.deg, Vector2.zero, randAngle + 180, coeff1 + randAngle + 180));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff2 + MathFunctions.deg, Vector2.zero, randAngle + 360 - coeff2, randAngle + 360));
					diagramHelper.AddAngleArc (new AngleArc ("?", Vector2.zero, coeff1 + randAngle + 180, randAngle + 360 - coeff2));

					diagramHelper.ShiftPosition (new Vector2 (0, 70));
					diagramHelper.Draw ();

				}
				else if(selector == 4)
				{
					float randAngle = Random.Range (0, 90);
					coeff1 = Random.Range (40, 70);
					coeff3 = Random.Range (70, 140);
					coeff4 = Random.Range (40, 70);
					coeff5 = Random.Range (40, 70);
					coeff2 = 360 - (coeff1 + coeff5 + coeff3 + coeff4);

					QuestionLatext.text = "Find the missing angle :";
					Answer = coeff2 + "" + MathFunctions.deg;

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + coeff1, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + coeff1 + coeff2, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + coeff1 + coeff2 + coeff3, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + coeff1 + coeff2 + coeff3 + coeff4, true, 100));

					diagramHelper.AddAngleArc (new AngleArc ("" + coeff1 + MathFunctions.deg, Vector2.zero, randAngle, coeff1 + randAngle));
					diagramHelper.AddAngleArc (new AngleArc ("?", Vector2.zero, coeff1 + randAngle, coeff1 + coeff2 + randAngle));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff3 + MathFunctions.deg, Vector2.zero, coeff1 + coeff2 + randAngle, coeff1 + coeff2 + coeff3 + randAngle));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff4 + MathFunctions.deg, Vector2.zero, coeff1 + coeff2 + coeff3 + randAngle, coeff1 + coeff2 + coeff3 + coeff4 + randAngle));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff5 + MathFunctions.deg, Vector2.zero, coeff1 + coeff2 + coeff3 + coeff4 + randAngle, coeff1 + coeff2 + coeff3 + coeff4 + coeff5 + randAngle));

					diagramHelper.ShiftPosition (new Vector2 (0, 20));
					diagramHelper.Draw ();

				}
				else if(selector == 5)
				{
					float randAngle = Random.Range (0, 90);
					coeff1 = Random.Range (40, 70);
					coeff2 = Random.Range (40, 70);
					coeff3 = Random.Range (40, 70);
					coeff4 = 360 - (coeff1 + coeff2 + coeff3);

					QuestionLatext.text = "Find the missing angle :";
					Answer = coeff4 + "" + MathFunctions.deg;

					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + coeff1, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + coeff1 + coeff2, true, 100));
					diagramHelper.AddLinePoint (new LinePoint ("", Vector2.zero, randAngle + coeff1 + coeff2 + coeff3, true, 100));

					diagramHelper.AddAngleArc (new AngleArc ("" + coeff1 + MathFunctions.deg, Vector2.zero, randAngle, coeff1 + randAngle));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff2 + MathFunctions.deg, Vector2.zero, coeff1 + randAngle, coeff1 + coeff2 + randAngle));
					diagramHelper.AddAngleArc (new AngleArc ("" + coeff3 + MathFunctions.deg, Vector2.zero, coeff1 + coeff2 + randAngle, coeff1 + coeff2 + coeff3 + randAngle));
					diagramHelper.AddAngleArc (new AngleArc ("?", Vector2.zero, coeff1 + coeff2 + coeff3 + randAngle, coeff1 + coeff2 + coeff3 + coeff4 + randAngle));

					diagramHelper.Draw ();
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
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += value.ToString ();
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += value.ToString ();
				}

			}
			else if (value == 10)
			{    //.
				if (checkLastTextFor(new string[1] { "." }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += ".";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += ".";
				}
			}
			else if (value == 11)
			{   // Back
				if (userAnswerLaText.text.Length > 0)
				{
					if (checkLastTextFor (new string[1] { "\\angle{}" })) 
					{
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 8);
						upflag = 0;
						numPad.transform.Find ("PanelLayer").Find ("angle").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
					}
					else if (upflag == 0 && checkLastTextFor (new string[1] { "}" }))
					{
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 2);
						userAnswerLaText.text += "}";
						upflag = 1;
						numPad.transform.Find ("PanelLayer").Find ("angle").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
					}
					else if (upflag == 1)
					{
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 2);
						userAnswerLaText.text += "}";
					}
					else
					{
						userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
					}
				}
			}
			else if (value == 12)
			{   // angle
				if (upflag == 0)
				{
					if (checkLastTextFor(new string[1] { "\\angle{}" }))
					{
						userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 8);
					}
					if (checkLastTextFor (new string[1] { "}" })) {
						return;
					}
					userAnswerLaText.text += "\\angle{}";
					upflag = 1;
					numPad.transform.Find ("PanelLayer").Find ("angle").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				}
				else if(upflag == 1)
				{
					if (checkLastTextFor(new string[1] { "}" }))
					{
						userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
					}
					userAnswerLaText.text += "}";
					upflag = 0;
					numPad.transform.Find ("PanelLayer").Find ("angle").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}			
			}
			else if (value == 13)
			{   // Deg

				if (checkLastTextFor(new string[1] { "" + MathFunctions.deg }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "" + MathFunctions.deg;
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "" + MathFunctions.deg;
				}
			
			}
			else if (value == 14) 
			{   // All Clear
				userAnswerLaText.text = "";
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("angle").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
			}
			else if (value == 15)
			{    //A
				if (checkLastTextFor(new string[1] { "A" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "A";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "A";
				}
			}
			else if (value == 16)
			{    //B
				if (checkLastTextFor(new string[1] { "B" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "B";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "B";
				}
			}
			else if (value == 17)
			{    //C
				if (checkLastTextFor(new string[1] { "C" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "C";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "C";
				}
			}
			else if (value == 18)
			{    //D
				if (checkLastTextFor(new string[1] { "D" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "D";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "D";
				}
			}
			else if (value == 19)
			{    //E
				if (checkLastTextFor(new string[1] { "E" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "E";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "E";
				}
			}
			else if (value == 20)
			{    //F
				if (checkLastTextFor(new string[1] { "F" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "F";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "F";
				}
			}
			else if (value == 21)
			{    //G
				if (checkLastTextFor(new string[1] { "G" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "G";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "G";
				}
			}
			else if (value == 22)
			{    //O
				if (checkLastTextFor(new string[1] { "O" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "O";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "O";
				}
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
				alternateAnswer += answers[i] + MathFunctions.deg;

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
			Vector2[] fourChoicePositions = new Vector2[] {
				new Vector2 (-180, -80f),
				new Vector2 (180, -80f),
				new Vector2 (-180, 0f),
				new Vector2 (180, 0f)
			};
			for (int i = 1; i <= 4; i++)
			{
				MCQ.transform.Find ("Option" + i).gameObject.SetActive (i<=NumberOfMCQ);
				MCQ.transform.Find ("Option" + i).GetComponent<RectTransform> ().anchoredPosition = fourChoicePositions[i-1];
			}
			this.MCQ.SetActive (true);
			this.numPad.SetActive (false);
			this.GeneralButton.gameObject.SetActive (false);

			float[] threeChoicePositions = new float[]{ -255f, 0f, 255f };
			if (NumberOfMCQ == 3)
			{
				for (int i = 1; i <= 3; i++)
				{
					MCQ.transform.Find ("Option" + i).GetComponent<RectTransform> ().anchoredPosition = new Vector2 (threeChoicePositions[i-1],0f);
				}
			}

			for (int i = 1; i <= 4; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
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

