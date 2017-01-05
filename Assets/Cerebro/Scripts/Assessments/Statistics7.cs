using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro {
	public class Statistics7 : BaseAssessment {

		private string Answer;
		private List<string> options;
		private int randSelector;
		private List<int> coeff;

		public TEXDraw subQuestionTEX;
		public Text StatTableColumn1;
		public Text StatTableColumn2;
		public Text StatTableColumn3;
		public GameObject MCQ;
		public StatisticsHelper statisticsHelper; 
		public GameObject CheckButton;
		public GameObject numPadBg;
		public int gridValOffset, axisValueOffset;

		void Start () {

			base.Initialise ("M", "STA07", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;
			GenerateQuestion ();
		
		}

		public override void SubmitClick(){
			if (ignoreTouches || (userAnswerLaText.text == "" && !statisticsHelper.IsInteractable()) || (statisticsHelper.IsInteractable() && !statisticsHelper.IsAnswered())) {
				return;
			}
			if (statisticsHelper.IsInteractable ()) {
				userAnswerLaText = GeneralButton.gameObject.GetChildByName<TEXDraw> ("Text");
			}
			int increment = 0;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (!statisticsHelper.IsInteractable()) {

				if (MCQ.activeSelf) {
					if (Answer == userAnswerLaText.text) {
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

					if (float.TryParse (userAnswerLaText.text, out userAnswer)) {
						userAnswer = float.Parse (userAnswerLaText.text);
					} else {
						directCheck = true;
					}


					if (directCheck) {
						if (userAnswerLaText.text == Answer){
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
				correct = statisticsHelper.CheckAnswer ();
			}
			if (correct == true) {
				if (Queslevel == 1) {
					increment = 5;
					UpdateStreak(12, 17);
				} else if (Queslevel == 2) {
					increment = 5;
					UpdateStreak(8, 12);
				} else if (Queslevel == 3) {
					increment = 10;
					UpdateStreak(8, 12);
				} else if (Queslevel == 4) {
					increment = 10;
					UpdateStreak(8, 12);
				} else if (Queslevel == 5) {
					increment = 10;
					UpdateStreak(8, 12);
				} else if (Queslevel == 6) {
					increment = 15;
					UpdateStreak(8, 12);
				}  

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
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
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
			userAnswerLaText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<TEXDraw> ();
			answerButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
			SubmitClick ();
		}

		protected override IEnumerator ShowWrongAnimation()
		{
			userAnswerLaText.color = MaterialColor.red800;
			Go.to(userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
			statisticsHelper.HandleIncorrectAnwer (isRevisitedQuestion);
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
				ignoreTouches = false;
				statisticsHelper.ResetAnswer ();
			}
			else
			{
				this.CheckButton.gameObject.SetActive(false);
				if (numPad.activeSelf && !statisticsHelper.IsInteractable())
				{               // is not MCQ type question
					userAnswerLaText.text = Answer.ToString();
					userAnswerLaText.color = MaterialColor.green800;
				}
				else
				{
					CerebroHelper.DebugLog("going in else");
					userAnswerLaText.color = MaterialColor.textDark;

					statisticsHelper.ShowCorrectAnswer ();
				}
			}

			ShowContinueButton();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			userAnswerLaText.color = MaterialColor.green800;
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations( 2, GoLoopType.PingPong );
			var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
			var tween = new GoTween( userAnswerLaText.gameObject.transform, 0.2f, config );
			flow.insert( 0f, tween );
			flow.play ();
			statisticsHelper.HandleCorrectAnswer (); 
			yield return new WaitForSeconds (1f);
			userAnswerLaText.color = MaterialColor.textDark;

			showNextQuestion ();

			if (levelUp) {
				StartCoroutine (HideAnimation ());
				base.LevelUp ();
				yield return new WaitForSeconds (1.5f);
				StartCoroutine (StartAnimation ());
			}

		}

		void RandomizeMCQOptionsAndFill(List<string> options)
		{
			options.Shuffle ();
			int cnt = options.Count;
			for (int i = 1; i <= cnt; i++) {
				MCQ.transform.Find ("Option"+i).Find ("Text").GetComponent<TEXDraw> ().text = options [i - 1];
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
			ResetTable ();
			SetNumpadMode ();

			options = new List<string> ();
			coeff = new List<int> ();
			statisticsHelper.Reset ();
			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 8);

				if (selector == 1)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 50;
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfBars = 5;
					minValue = Random.Range (2, 6);
					maxValue = Random.Range (12, 16);

					List<string> weekDays = new List<string>() {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday"};

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfBars - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = "The data of cars produced by a company in a 5-day week are given in the table below. Complete the given bar graph to represent the given information.";
					TableContentsColumn1.Add ("<size=25>Weekdays</size>");
					TableContentsColumn1.AddRange (weekDays);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (16, 18), 22f);
					statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 0));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (weekDays[0], coeff[0]),
									new StatisticsValue (weekDays[1], coeff[1]),
									new StatisticsValue (weekDays[2], coeff[2]),
									new StatisticsValue (weekDays[3], coeff[3]),
									new StatisticsValue (weekDays[4], coeff[4])
								}
							).SetAxisName ("Weekdays").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number of Cars").SetPointOffset (2)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 2) 
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 10;
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfBars = 5;
					minValue = Random.Range (6, 11);
					maxValue = 20;

					List<string> Subjects = new List<string>() {"Maths", "Physics", "Biology", "Chemistry", "English", "Economy", "Philosophy", "Computer"};
					Subjects.Shuffle ();

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfBars - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = "Justin is a student of class VI and secured the following marks out of 100 marks in various subjects. Complete the given bar graph to express his result.";
					TableContentsColumn1.Add ("<size=25>Subject</size>");
					TableContentsColumn1.AddRange (Subjects);
					TableContentsColumn2.Add ("<size=25>Marks</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (16, 22), 20f);
					statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 0));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (Subjects[0], coeff[0]),
									new StatisticsValue (Subjects[1], coeff[1]),
									new StatisticsValue (Subjects[2], coeff[2]),
									new StatisticsValue (Subjects[3], coeff[3]),
									new StatisticsValue (Subjects[4], coeff[4])
								}
							).SetAxisName ("Subject").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Marks").SetPointOffset (2)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 3)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 2 * Random.Range (5, 15);
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfBars = 5;
					minValue = Random.Range (4, 8);
					maxValue = Random.Range (10, 16);

					List<string> TravelMode = new List<string>() {"Bus", "Car", "Taxi", "Walking", "Auto", "Cycle"};
					TravelMode.Shuffle ();

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfBars - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = string.Format ("The given table shows the mode of travelling of {0} students of grade VI of a school. Complete the given bar graph to represent the data.", coeff.Sum ());
					TableContentsColumn1.AddRange (TravelMode);
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (16, 18), 22f);
					statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 0));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (TravelMode[0], coeff[0]),
									new StatisticsValue (TravelMode[1], coeff[1]),
									new StatisticsValue (TravelMode[2], coeff[2]),
									new StatisticsValue (TravelMode[3], coeff[3]),
									new StatisticsValue (TravelMode[4], coeff[4])
								}
							).SetAxisName ("Mode of Travel").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number of Students").SetPointOffset (2)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 4)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 100 * Random.Range (5, 15);
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfBars = 5;
					minValue = Random.Range (4, 8);
					maxValue = Random.Range (12, 16);

					List<string> Item = new List<string>() {"Food", "Education", "Clothing", "Travelling", "Saving"};
					Item.Shuffle ();

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfBars - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = string.Format ("The given table shows monthly expenses of Peter's family. Represent it in the given bar graph.");
					TableContentsColumn1.Add ("<size=25>Item</size>");
					TableContentsColumn1.AddRange (Item);
					TableContentsColumn2.Add ("<size=25>Expenditure in Rs</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (16, 18), 22f);
					statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 0));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (Item[0], coeff[0]),
									new StatisticsValue (Item[1], coeff[1]),
									new StatisticsValue (Item[2], coeff[2]),
									new StatisticsValue (Item[3], coeff[3]),
									new StatisticsValue (Item[4], coeff[4])
								}
							).SetAxisName ("Item").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Expenditure in Rs").SetPointOffset (2)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 4 || selector == 5)
				{
					subQuestionTEX.gameObject.SetActive (true);

					axisValueOffset = 2 * Random.Range (1, 6);
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfBars = 4;
					do {
						minValue = Random.Range (2, 10);
						maxValue = Random.Range (minValue, 11);
					} while (maxValue - minValue < numberOfBars - 1);

					List<string> Cities = new List<string> (){"Nainital", "Mussoorie", "Dehradun", "Shimla", "Mumbai", "Delhi", "Calcutta", "Chennai", "Bangalore", "Kanpur", "Lucknow"};
					Cities.Shuffle ();

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfBars - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = "The given graph shows the number of boarding schools in each of the given cities.";

					if (selector == 4)
					{
						SetMCQMode (4);

						int randSelector1 = Random.Range (0, 2);
						subQuestionTEX.text = string.Format ("Which city has the {0} number of boarding schools.", randSelector1 == 0? "maximum": "least");
						if (randSelector1 == 0){
							options.Add (Cities[coeff.IndexOf (gridValOffset * maxValue)]);
							for (int i = 1; i < 4; i++){
								options.Add (Cities[MathFunctions.AddCyclic (i, coeff.IndexOf (gridValOffset * maxValue), 4)]);
							}
						} else {
							options.Add (Cities[coeff.IndexOf (gridValOffset * minValue)]);
							for (int i = 1; i < 4; i++){
								options.Add (Cities[MathFunctions.AddCyclic (i, coeff.IndexOf (gridValOffset * minValue), 4)]);
							}
						}
						Answer = options[0];
						RandomizeMCQOptionsAndFill (options);
					}
					else if (selector == 5)
					{
						int randSelector1 = Random.Range (0, 4);
						subQuestionTEX.text = string.Format ("How many boarding schools are there in {0}?", Cities[randSelector1]);
						Answer = string.Format ("{0}", coeff[randSelector1]);
					}

					statisticsHelper.SetGridParameters (new Vector2 (18, 14), 15f);
					statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
					statisticsHelper.ShiftPosition (new Vector2 (-250, 215));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (Cities[0], coeff[0]),
									new StatisticsValue (Cities[1], coeff[1]),
									new StatisticsValue (Cities[2], coeff[2]),
									new StatisticsValue (Cities[3], coeff[3]),
								}
							).SetAxisName ("Cities").SetPointOffset (4),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number of Boarding Schools").SetPointOffset (2)
						}
					);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 6)
				{
					int numberOfData = Random.Range (20, 40);
					List<int> dataSet = MathFunctions.GetFrequencyTableDataSet (20, 50, numberOfData, Random.Range (5, 8));

					QuestionText.text = string.Format ("Complete the given frequency table for the given array data.\n\n{0}", MathFunctions.getArrayAsSet (dataSet.ToArray (), false, false, false));

					statisticsHelper.SetStatisticsType(StatisticsType.Frequency);
					statisticsHelper.SetFrequencyDataSets(dataSet);
					GeneralButton.gameObject.SetActive(false);
					statisticsHelper.SetInteractable(true);
					statisticsHelper.onFrequencyTextBoxClicked = OnFrequencyTextClicked;
					statisticsHelper.DrawFrequencyTable();
					statisticsHelper.ShiftPosition (new Vector2 (-240, 80));
				}
				else if (selector == 7)
				{
					int numberOfStudents = Random.Range (20, 40);
					int maxMarks = 25 * Random.Range (1, 5);
					int interval = 5 * maxMarks / 25;
					List<int> dataSet = MathFunctions.GetIntRandomDataSet (1, maxMarks + 1, numberOfStudents);

					QuestionText.text = string.Format ("Given are the marks of {0} students in a maths olympiad. Complete the given frequency table. Take interval = {1}.\n\n{2}", numberOfStudents, interval, MathFunctions.getArrayAsSet (dataSet.ToArray (), false, false, false));

					statisticsHelper.SetStatisticsType(StatisticsType.Frequency);
					statisticsHelper.SetFrequencyDataSets(dataSet);
					statisticsHelper.SetFrequencyInterval (interval);
					GeneralButton.gameObject.SetActive(false);
					statisticsHelper.SetInteractable(true);
					statisticsHelper.onFrequencyTextBoxClicked = OnFrequencyTextClicked;
					statisticsHelper.DrawFrequencyTable();
					statisticsHelper.ShiftPosition (new Vector2 (-240, 80));
				}
			}
			#endregion
			#region level2
			else if (level == 2)
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();
					List<string> Subjects = new List<string>() {"Maths", "English", "Hindi", "Science", "Social Science"};
					Subjects.Shuffle ();

					int numberOfData = 5;
					coeff = MathFunctions.GetPieDataSet (10, 100, numberOfData, 10);

					TableContentsColumn1.Add ("<size=25>Subjects</size>");
					TableContentsColumn1.AddRange (Subjects);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetStatisticsType (StatisticsType.PieToDrag);
					statisticsHelper.ShiftPosition (new Vector2 (-330, 110f));
					statisticsHelper.SetPieParameters (
						Subjects,
						coeff
					);
					statisticsHelper.SetInteractable(true);
					statisticsHelper.SetPieRadius (100f); 
					statisticsHelper.DrawGraph ();

					QuestionText.text = string.Format ("The individual preference in subjects of students in a school is given in the table. Complete the pie chart for the given data.");
				}
				else if (selector == 2)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();
					List<string> Subjects = new List<string>() {"Maths", "English", "Hindi", "Science", "History"};
					Subjects.Shuffle ();

					int numberOfData = 5;
					coeff = MathFunctions.GetPieDataSet (16, 100, numberOfData, 2);

					TableContentsColumn1.Add ("<size=25>Subjects</size>");
					TableContentsColumn1.AddRange (Subjects);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetStatisticsType (StatisticsType.PieToDrag);
					statisticsHelper.ShiftPosition (new Vector2 (-330, 110f));
					statisticsHelper.SetPieParameters (
						Subjects,
						coeff
					);
					statisticsHelper.SetInteractable(true);
					statisticsHelper.SetPieRadius (100f); 
					statisticsHelper.DrawGraph ();

					QuestionText.text = string.Format ("The following table shows the number of books of different subjects which are kept in the middle school section of a library. Complete the pie chart for this data.");
				}
				else if (selector == 3)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 10 * Random.Range (1, 6);
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfPoints = 5;
					minValue = Random.Range (4, 8);
					maxValue = Random.Range (12, 16);

					List<string> Sports = new List<string>() {"Cricket", "Badminton", "Hockey", "Tennis", "Swimming", "Football", "Squash", "Basketball", "Volleyball"};
					Sports.Shuffle ();

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfPoints - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = string.Format ("The following table shows the preference of students for various sports in a school. Represent the data in the given line graph.");
					TableContentsColumn1.Add ("<size=25>Sports</size>");
					TableContentsColumn1.AddRange (Sports);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (18, 18), 22f);
					statisticsHelper.SetStatisticsType (StatisticsType.Line);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 0));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (Sports[0], coeff[0]),
									new StatisticsValue (Sports[1], coeff[1]),
									new StatisticsValue (Sports[2], coeff[2]),
									new StatisticsValue (Sports[3], coeff[3]),
									new StatisticsValue (Sports[4], coeff[4])
								}
							).SetAxisName ("Sports").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number of Students").SetPointOffset (2)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 4)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 10;
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfPoints = 5;
					minValue = Random.Range (4, 8);
					maxValue = Random.Range (12, 16);

					List<string> Subjects = new List<string>() {"Maths", "English", "Hindi", "Science", "Social Science"};
					Subjects.Shuffle ();

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfPoints - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = string.Format ("The given table shows the performance of a student in various subjects in the annual examination. Complete the line graph for the data.");
					TableContentsColumn1.Add ("<size=25>Subjects</size>");
					TableContentsColumn1.AddRange (Subjects);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (18, 18), 22f);
					statisticsHelper.SetStatisticsType (StatisticsType.Line);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 0));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (Subjects[0], coeff[0]),
									new StatisticsValue (Subjects[1], coeff[1]),
									new StatisticsValue (Subjects[2], coeff[2]),
									new StatisticsValue (Subjects[3], coeff[3]),
									new StatisticsValue (Subjects[4], coeff[4])
								}
							).SetAxisName ("Subjects").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number of Students").SetPointOffset (2)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 5)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();
					List<string> TransportMode = new List<string>() {"School Bus", "Local Bus", "Bicycle", "Car", "Walking"};
					TransportMode.Shuffle ();

					int numberOfData = 5;
					coeff = MathFunctions.GetPieDataSet (200, 1000, numberOfData, 50);

					TableContentsColumn1.Add ("<size=25>Mode</size>");
					TableContentsColumn1.AddRange (TransportMode);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetStatisticsType (StatisticsType.PieToDrag);
					statisticsHelper.ShiftPosition (new Vector2 (-330, 110f));
					statisticsHelper.SetPieParameters (
						TransportMode,
						coeff
					);
					statisticsHelper.SetInteractable(true);
					statisticsHelper.SetPieRadius (100f); 
					statisticsHelper.DrawGraph ();

					QuestionText.text = string.Format ("The given table shows the various modes of transport used by the students to go to school and back home. Complete the pie chart to represent this data.");
				}
			}
			#endregion
			#region level3
			else if (level == 3)
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();
					List<string> Branch = new List<string>() {"Civil", "Chemical", "Mechanical", "IT", "Electrical", "Aerospace", "Computer Science", "Metallurgy"};
					Branch.Shuffle ();

					int numberOfData = 5;
					coeff = MathFunctions.GetPieDataSet (15, 99, numberOfData, 3);

					TableContentsColumn1.Add ("<size=25>Branch</size>");
					TableContentsColumn1.AddRange (Branch);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetStatisticsType (StatisticsType.PieToDrag);
					statisticsHelper.ShiftPosition (new Vector2 (-330, 110f));
					statisticsHelper.SetPieParameters (
						Branch,
						coeff
					);
					statisticsHelper.SetInteractable(true);
					statisticsHelper.SetPieRadius (100f); 
					statisticsHelper.DrawGraph ();

					QuestionText.text = string.Format ("The following table shows the list of candidates who are offered admission to various branches of engineering in an engineering college. Complete the given pie chart to represent the data.");
				}
				else if (selector == 2)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 2;
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfPoints = 7;
					minValue = Random.Range (4, 8);
					maxValue = Random.Range (12, 16);

					List<string> Days = new List<string>() {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"};

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfPoints - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = string.Format ("The following table shows the minimum temperatures for a week in a city. Represent the data in a line graph.");
					TableContentsColumn1.Add ("<size=25>Days</size>");
					TableContentsColumn1.AddRange (Days);
					TableContentsColumn2.Add ("<size=25>Temperature (" + MathFunctions.deg + "C)</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (16, 18), 22f);
					statisticsHelper.SetStatisticsType (StatisticsType.Line);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 0));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (Days[0], coeff[0]),
									new StatisticsValue (Days[1], coeff[1]),
									new StatisticsValue (Days[2], coeff[2]),
									new StatisticsValue (Days[3], coeff[3]),
									new StatisticsValue (Days[4], coeff[4]),
									new StatisticsValue (Days[5], coeff[5]),
									new StatisticsValue (Days[6], coeff[6])
								}
							).SetAxisName ("Days").SetPointOffset (2),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Temperature (" + MathFunctions.deg + "C)").SetPointOffset (2)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 3)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 2 * Random.Range (2, 10);
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfPoints = 5;
					minValue = Random.Range (4, 8);
					maxValue = Random.Range (12, 16);

					List<string> Make = new List<string>() {"Fiat", "Renault", "Bentley", "Vauxhall", "Rolls Royce", "Ford", "BMW", "Bugatti", "Aston Martin"};
					Make.Shuffle ();

					coeff.Add (gridValOffset * minValue);
					for (int i = 1; i < numberOfPoints - 1; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Add (gridValOffset * maxValue);
					coeff.Shuffle ();

					QuestionText.text = string.Format ("The table shows the number of cars of different makes in a car park. Represent the data on the given line graph.");
					TableContentsColumn1.Add ("<size=25>Make</size>");
					TableContentsColumn1.AddRange (Make);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (18, 18), 22f);
					statisticsHelper.SetStatisticsType (StatisticsType.Line);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 0));
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (Make[0], coeff[0]),
									new StatisticsValue (Make[1], coeff[1]),
									new StatisticsValue (Make[2], coeff[2]),
									new StatisticsValue (Make[3], coeff[3]),
									new StatisticsValue (Make[4], coeff[4])
								}
							).SetAxisName ("Make").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number").SetPointOffset (2)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();
				}
				else if (selector == 4)
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();
					List<string> Branch = new List<string>() {"Oats", "Barley", "Wheat", "Rye", "Beans"};
					Branch.Shuffle ();

					int numberOfData = 4;
					coeff = MathFunctions.GetPieDataSet (45, 300, numberOfData, 15);

					TableContentsColumn1.Add ("<size=25>Cereal</size>");
					TableContentsColumn1.AddRange (Branch);
					TableContentsColumn2.Add ("<size=25>Quantity (gm)</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetStatisticsType (StatisticsType.PieToDrag);
					statisticsHelper.ShiftPosition (new Vector2 (-330, 110f));
					statisticsHelper.SetPieParameters (
						Branch,
						coeff
					);
					statisticsHelper.SetInteractable(true);
					statisticsHelper.SetPieRadius (100f); 
					statisticsHelper.DrawGraph ();

					QuestionText.text = string.Format ("A packet of breakfast cereal weighing {0} gm contains four ingredients as given in the table. Complete the given pie chart for the data.", coeff.Sum ());
				}
				else if (selector == 5)  // show angles in pie
				{
					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();
					List<string> Expenses = new List<string>() {"Food", "House Rent", "Electricity Bill", "Water Bill ", "Savings", "School Fee"};
					Expenses.Shuffle ();

					int numberOfData = 6;
					coeff = MathFunctions.GetPieDataSet (2000, 15000, numberOfData, 1000);

					statisticsHelper.SetStatisticsType (StatisticsType.Pie);
					statisticsHelper.ShiftPosition (new Vector2 (-360f, 230f));
					statisticsHelper.SetPieParameters (
						Expenses,
						coeff
					);
					statisticsHelper.SetPieRadius (100f); 
					statisticsHelper.DrawGraph ();

					randSelector = Random.Range (0, Expenses.Count);
					QuestionText.text = string.Format ("Mr. Allen's monthly income is Rs. {0} and his monthly expenses are represented by the given pie chart. How much (in Rs.) does he spend on '{1}'?", coeff.Sum (), Expenses[randSelector]);
					Answer = string.Format ("{0}", coeff[randSelector]);
				}
			}
			#endregion
			#region level4
			else if (level == 4)
			{
				selector = GetRandomSelector (1, 5);
				subQuestionTEX.gameObject.SetActive (true);
				StartSequence (4);

				axisValueOffset = 10;
				gridValOffset = axisValueOffset / 2;
				int minValue, maxValue;
				int numberOfPoints = 6;
				do {
					minValue = Random.Range (2, 10);
					maxValue = Random.Range (minValue, 11);
				} while (maxValue - minValue < numberOfPoints - 1);

				List<string> Sports = new List<string>() {"Cricket", "Badminton", "Hockey", "Tennis", "Swimming", "Football", "Squash", "Basketball", "Volleyball"};
				Sports.Shuffle ();

				coeff.Add (gridValOffset * minValue);
				for (int i = 1; i < numberOfPoints - 1; i++){
					coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
				}
				coeff.Add (gridValOffset * maxValue);
				coeff.Shuffle ();

				QuestionText.text = "The line graph shows the number of children playing various games on a given day.";

				statisticsHelper.SetGridParameters (new Vector2 (20, 14), 16f);
				statisticsHelper.SetStatisticsType (StatisticsType.Line);
				statisticsHelper.ShiftPosition (new Vector2 (-240f, 240));
				statisticsHelper.SetGraphParameters (new StatisticsAxis[]
					{
						new StatisticsAxis ().SetStatisticsValues
						(
							new List<StatisticsValue>(){
								new StatisticsValue (Sports[0], coeff[0]),
								new StatisticsValue (Sports[1], coeff[1]),
								new StatisticsValue (Sports[2], coeff[2]),
								new StatisticsValue (Sports[3], coeff[3]),
								new StatisticsValue (Sports[4], coeff[4]),
								new StatisticsValue (Sports[5], coeff[5])
							}
						).SetAxisName ("Test Month").SetPointOffset (3),
						new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Marks").SetPointOffset (2)
					}
				);
				statisticsHelper.DrawGraph ();

				if (selector == 1)
				{
					SetMCQMode (4);
					options.Add (Sports[coeff.IndexOf (minValue * gridValOffset)]);
					for (int i = 0; i < numberOfPoints; i++){
						if (options.Count == 4){
							break;
						}
						if (i == coeff.IndexOf (minValue * gridValOffset)){
							continue;
						}
						options.Add (Sports[i]);
					}
					RandomizeMCQOptionsAndFill (options);

					subQuestionTEX.text = "Which game had the least number of players?";
					Answer = string.Format ("{0}", Sports[coeff.IndexOf (minValue * gridValOffset)]);
				}
				else if (selector == 2)
				{
					SetMCQMode (4);
					options.Add (Sports[coeff.IndexOf (maxValue * gridValOffset)]);
					for (int i = 0; i < numberOfPoints; i++){
						if (options.Count == 4){
							break;
						}
						if (i == coeff.IndexOf (maxValue * gridValOffset)){
							continue;
						}
						options.Add (Sports[i]);
					}
					RandomizeMCQOptionsAndFill (options);

					subQuestionTEX.text = "Which game had the most number of players?";
					Answer = string.Format ("{0}", Sports[coeff.IndexOf (maxValue * gridValOffset)]);
				}
				else if (selector == 3)
				{
					subQuestionTEX.text = "What was the total number of children playing all games?";
					int total = 0;
					foreach (int i in coeff){
						total += i;
					}
					Answer = string.Format ("{0}", total);
				}
				else if (selector == 4)
				{
					int randSelector1;
					int randSelector2;
					do {
						randSelector1 = Random.Range (0, numberOfPoints);
						randSelector2 = Random.Range (0, numberOfPoints);
					} while (randSelector1 == randSelector2);

					string expression = coeff[randSelector1] > coeff[randSelector2]? "more": "fewer";
					subQuestionTEX.text = string.Format ("How many {0} {1} players were there than {2} players?", expression, Sports[randSelector1], Sports[randSelector2]);
					Answer = string.Format ("{0}", Mathf.Abs (coeff[randSelector1] - coeff[randSelector2]));
				}
			}
			#endregion
			#region level5
			else if (level == 5)
			{
				selector = GetRandomSelector (1, 7);
				subQuestionTEX.gameObject.SetActive (true);
				StartSequence (6);

				axisValueOffset = 2 * Random.Range (1, 4);
				gridValOffset = axisValueOffset / 2;
				int numberOfPoints = 7;
				int minValue = Random.Range (1, 8);

				List<string> Time = new List<string>() {"8 am", "9 am", "10 am", "11 am", "12 noon", "1 pm", "2 pm"};
				int count = 0;
				do {
					minValue = Random.Range (1, 8);
					coeff = MathFunctions.GetIntRandomDataSet (1, 4, numberOfPoints);
				} while (coeff.Sum () + minValue > 12);

				for (int i = 0; i < numberOfPoints; i++){
					coeff[i] *= gridValOffset;
				}
				coeff[0] += gridValOffset * minValue;
				for (int i = 1; i < numberOfPoints; i++){
					coeff[i] += coeff[i-1];
				}

				QuestionText.text = "In the given line graph, the temperature of a particular day in North India is given.";

				statisticsHelper.SetGridParameters (new Vector2 (22, 14), 14f);
				statisticsHelper.SetStatisticsType (StatisticsType.Line);
				statisticsHelper.ShiftPosition (new Vector2 (-250, 235));
				statisticsHelper.SetGraphParameters (new StatisticsAxis[]
					{
						new StatisticsAxis ().SetStatisticsValues
						(
							new List<StatisticsValue>(){
								new StatisticsValue (Time[0], coeff[0]),
								new StatisticsValue (Time[1], coeff[1]),
								new StatisticsValue (Time[2], coeff[2]),
								new StatisticsValue (Time[3], coeff[3]),
								new StatisticsValue (Time[4], coeff[4]),
								new StatisticsValue (Time[5], coeff[5]),
								new StatisticsValue (Time[6], coeff[6])
							}
						).SetAxisName ("Test Month").SetPointOffset (3),
						new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Marks").SetPointOffset (2)
					}
				);
				statisticsHelper.DrawGraph ();

				if (selector == 1)
				{
					SetMCQMode (4);

					options.Add (Time[0]);
					options.Add (Time[Random.Range (1, numberOfPoints - 1)]);
					int randSelector1 = Random.Range (1, numberOfPoints - 1);
					while (options[1] == Time[randSelector1]){
						randSelector1 = Random.Range (1, numberOfPoints - 1);
					}
					options.Add (Time[randSelector1]);
					options.Add (Time[numberOfPoints - 1]);
					RandomizeMCQOptionsAndFill (options);

					subQuestionTEX.text = "At what time was the minimum temperature observed?";
					Answer = string.Format ("{0}", Time[0]);
				}
				else if (selector == 2)
				{
					SetMCQMode (4);

					options.Add (Time[0]);
					options.Add (Time[Random.Range (1, numberOfPoints - 1)]);
					int randSelector1 = Random.Range (1, numberOfPoints - 1);
					while (options[1] == Time[randSelector1]){
						randSelector1 = Random.Range (1, numberOfPoints - 1);
					}
					options.Add (Time[randSelector1]);
					options.Add (Time[numberOfPoints - 1]);
					RandomizeMCQOptionsAndFill (options);

					subQuestionTEX.text = "At what time was the maximum temperature observed?";
					Answer = string.Format ("{0}", Time[numberOfPoints - 1]);
				}
				else if (selector == 3)
				{
					subQuestionTEX.text = string.Format ("What is the miminum temperature (in {0}C) of the day?", MathFunctions.deg);
					Answer = string.Format ("{0}", coeff[0]);
				}
				else if (selector == 4)
				{
					subQuestionTEX.text = string.Format ("What is the maximum temperature (in {0}C) of the day?", MathFunctions.deg);
					Answer = string.Format ("{0}", coeff[numberOfPoints - 1]);
				}
				else if (selector == 5)
				{
					int	randSelector1 = Random.Range (0, numberOfPoints - 1);
					int	randSelector2 = Random.Range (randSelector1 + 1, numberOfPoints);

					subQuestionTEX.text = string.Format ("How much did the temperature rise (in {0}C) between {1} and {2}?", MathFunctions.deg, Time[randSelector1], Time[randSelector2]);
					Answer = string.Format ("{0}", coeff[randSelector2] - coeff[randSelector1]);
				}
				else if (selector == 6)
				{
					int randSelector1 = Random.Range (1, numberOfPoints - 1);
					subQuestionTEX.text = string.Format ("At what time was {0}{1}C temperature observed?", coeff[randSelector1], MathFunctions.deg);
					Answer = string.Format ("{0}", Time[randSelector1]);
				}
			}
			#endregion
			CerebroHelper.DebugLog (Answer);
			if (GeneralButton.gameObject.activeSelf) {
				userAnswerLaText = GeneralButton.gameObject.GetChildByName<TEXDraw> ("Text");
				userAnswerLaText.text = "";
			}
		}

		public void OnFrequencyTextClicked(GameObject textObj)
		{
			userAnswerLaText = textObj.gameObject.GetChildByName<TEXDraw> ("UserAnswer");
		}

		public override void numPadButtonPressed(int value) {
			if (ignoreTouches) {
				return;
			}
			if (value <= 9 && !userAnswerLaText.transform.parent.name.Contains("stick")) {
				userAnswerLaText.text += value.ToString ();
			} else if (value == 10) {    //Back
				if (checkLastTextFor (new string[1] { "}" })) {
					int lastIndex = userAnswerLaText.text.LastIndexOf ("\\not[-0.1,-0.1]{");
					Debug.Log ("Last Index " + lastIndex);
					if (lastIndex >= 0) {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, lastIndex) + userAnswerLaText.text.Substring (lastIndex + 16, userAnswerLaText.text.Length - 2 - (lastIndex + 15));
					}
				} else if (checkLastTextFor (new string[1] { "\\mid " })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 5);
				} else if (userAnswerLaText.text.Length > 0) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
			} else if (value == 11) {   // All Clear
				userAnswerLaText.text = "";
			} else if (value == 12  && userAnswerLaText.transform.parent.name.Contains("stick")) {
				int lastIndex = userAnswerLaText.text.LastIndexOf ("}");

				if (lastIndex >= 0) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, lastIndex + 1) + "\\not[-0.1,-0.1]{" + userAnswerLaText.text.Substring (lastIndex + 1, userAnswerLaText.text.Length - lastIndex - 1) + "}";
				} else {
					userAnswerLaText.text = "\\not[-0.1,-0.1]{" + userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length) + "}";
				}
			} else if (value == 13  && userAnswerLaText.transform.parent.name.Contains("range")) {
				if(checkLastTextFor (new string[1] { "-" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring (0,userAnswerLaText.text.Length-1);
				}
				userAnswerLaText.text += "-";
			} else if (value == 14 && userAnswerLaText.transform.parent.name.Contains("stick")) {
				userAnswerLaText.text += "\\mid ";
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

		protected void SetStatisticsMode()
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
		protected void SetTable (int number_of_columns = 2)
		{
			StatTableColumn1.gameObject.SetActive (true);
			StatTableColumn2.gameObject.SetActive (true);
			if (number_of_columns == 3) {
				StatTableColumn3.gameObject.SetActive (true);
			}
		}
		protected void ResetTable ()
		{
			StatTableColumn1.gameObject.SetActive (false);
			StatTableColumn2.gameObject.SetActive (false);
			StatTableColumn3.gameObject.SetActive (false);
		}
	}
}