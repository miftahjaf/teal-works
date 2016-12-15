using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.Linq;

namespace Cerebro {
	public class Statistics5 : BaseAssessment {

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

			base.Initialise ("M", "STA05", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;
			GenerateQuestion ();
		
		}

		public override void SubmitClick(){
			if (ignoreTouches || (userAnswerText.text == "" && !statisticsHelper.IsInteractable()) || (statisticsHelper.IsInteractable() && !statisticsHelper.IsAnswered())) {
				return;
			}
			int increment = 0;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (!statisticsHelper.IsInteractable()) {

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


					if (directCheck) {
						if (userAnswerText.text == Answer){
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
			statisticsHelper.HandleIncorrectAnwer (isRevisitedQuestion);
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
				statisticsHelper.ResetAnswer ();
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

					statisticsHelper.ShowCorrectAnswer ();
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
			statisticsHelper.HandleCorrectAnswer (); 
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

		void RandomizeMCQOptionsAndFill(List<string> options)
		{
			options.Shuffle ();
			int cnt = options.Count;
			for (int i = 1; i <= cnt; i++) {
				MCQ.transform.Find ("Option"+i).Find ("Text").GetComponent<Text> ().text = options [i - 1];
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
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<Text> ().color = MaterialColor.textDark;
			}

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 9);
				subQuestionTEX.gameObject.SetActive (true);
				StartSequence (8);

				axisValueOffset = 10;
				gridValOffset = axisValueOffset / 2;
				int minValue, maxValue;
				int numberOfBars = 4;
				do {
					minValue = Random.Range (2, 10);
					maxValue = Random.Range (minValue, 11);
				} while (maxValue - minValue < numberOfBars - 1);

				List<string> months = new List<string> (){"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
				months.Shuffle ();

				coeff.Add (gridValOffset * minValue);
				for (int i = 1; i < numberOfBars - 1; i++){
					coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
				}
				coeff.Add (gridValOffset * maxValue);
				coeff.Shuffle ();

				QuestionText.text = "The given graph shows the marks that Srinivas got in four maths tests. The tests were out of 50 marks.";

				statisticsHelper.SetGridParameters (new Vector2 (14, 14), 15f);
				statisticsHelper.SetStatisticsType (StatisticsType.HorizontalBar);
				statisticsHelper.ShiftPosition (new Vector2 (-270, 235));
				statisticsHelper.SetGraphParameters (new StatisticsAxis[]
					{
						new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Marks").SetPointOffset (2),
						new StatisticsAxis ().SetStatisticsValues
						(
							new List<StatisticsValue>(){
								new StatisticsValue (months[0].Substring (0, 3), coeff[0]),
								new StatisticsValue (months[1].Substring (0, 3), coeff[1]),
								new StatisticsValue (months[2].Substring (0, 3), coeff[2]),
								new StatisticsValue (months[3].Substring (0, 3), coeff[3]),
							}
						).SetAxisName ("Test Month").SetPointOffset (3)
					}
				);
				statisticsHelper.DrawGraph ();

				if (selector == 1)
				{
					SetMCQMode (3);

					int randSelector1 = Random.Range (0, 2);
					subQuestionTEX.text = string.Format ("What does the {0} scale represent?", randSelector1 == 0? "vertical": "horizontal");
					options.Add ("Test Month");
					options.Add ("Marks");
					options.Add (months[Random.Range(0, 4)]);
					Answer = options[randSelector1];
					RandomizeMCQOptionsAndFill (options);
				}
				else if (selector == 2)
				{
					int randSelector1 = Random.Range (0, 4);
					subQuestionTEX.text = string.Format ("How many marks did Srinivas score in his {0} test?", months[randSelector1]);
					Answer = string.Format ("{0}", coeff[randSelector1]);
				}
				else if (selector == 3)
				{
					subQuestionTEX.text = "What is his minimum score?";
					Answer = string.Format ("{0}", minValue * gridValOffset);
				}
				else if (selector == 4)
				{
					subQuestionTEX.text = "What is his maximum score?";
					Answer = string.Format ("{0}", maxValue * gridValOffset);
				}
				else if (selector == 5)
				{
					SetMCQMode (4);
					for (int i = 0; i < numberOfBars; i++){
						options.Add (months[i]);
					}
					RandomizeMCQOptionsAndFill (options);

					subQuestionTEX.text = "In which month did he score his minimum?";
					Answer = string.Format ("{0}", months[coeff.IndexOf (minValue * gridValOffset)]);
				}
				else if (selector == 6)
				{
					SetMCQMode (4);
					for (int i = 0; i < numberOfBars; i++){
						options.Add (months[i]);
					}
					RandomizeMCQOptionsAndFill (options);

					subQuestionTEX.text = "In which month did he score his maximum?";
					Answer = string.Format ("{0}", months[coeff.IndexOf (Mathf.Max (coeff.ToArray ()))]);
				}
				else if (selector == 7)
				{
					int randSelector1 = Random.Range (0, numberOfBars);

					subQuestionTEX.text = "What is the difference between maximum and minimum marks?";
					Answer = string.Format ("{0}", (maxValue - minValue) * gridValOffset);
				}
				else if (selector == 8)
				{
					int randSelector1;
					int randSelector2;
					do {
						randSelector1 = Random.Range (0, numberOfBars);
						randSelector2 = Random.Range (0, numberOfBars);
					} while (randSelector1 == randSelector2);

					string expression = coeff[randSelector1] > coeff[randSelector2]? "more": "fewer";
					subQuestionTEX.text = string.Format ("How many {0} marks did he get in the {1} test as compared to the {2} test?", expression, months[randSelector1], months[randSelector2]);
					Answer = string.Format ("{0}", Mathf.Abs (coeff[randSelector1] - coeff[randSelector2]));
				}
			}
			#endregion
			#region level2
			else if (level == 2)
			{
				selector = GetRandomSelector (1, 4);
				StartSequence(3);
				subQuestionTEX.gameObject.SetActive (true);

				List<int> coeff1 = new List<int> ();

				axisValueOffset = 2;
				gridValOffset = axisValueOffset / 2;
				int minValue, minValue1, maxValue, maxValue1;
				int numberOfBars = 4;
				do {
					minValue = Random.Range (2, 10);
					maxValue = Random.Range (minValue, 13);
				} while (maxValue - minValue < numberOfBars - 1);
				do {
					minValue1 = Random.Range (2, 10);
					maxValue1 = Random.Range (minValue1, 13);
				} while (maxValue1 - minValue1 < numberOfBars - 1);

				List<string> Books = new List<string> (){"Mystery", "Action", "Comics", "Fiction", "Satire", "Drama", "Horror"};
				Books.Shuffle ();

				coeff.Add (minValue * gridValOffset);
				coeff1.Add (minValue1 * gridValOffset);
				for (int i = 1; i < numberOfBars - 1; i++){
					coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					coeff1.Add (gridValOffset * Random.Range (minValue1 + 1, maxValue1));
				}
				coeff.Add (maxValue * gridValOffset);
				coeff1.Add (maxValue1 * gridValOffset);
				coeff.Shuffle ();
				coeff1.Shuffle ();

				QuestionText.text = "Use the given graph to answer the following question.";

				statisticsHelper.SetGridParameters (new Vector2 (14, 14), 15f);
				statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
				statisticsHelper.ShiftPosition (new Vector2 (-270, 235));
				statisticsHelper.SetBarValues (new List<string> () {"Girls", "Boys"});
				statisticsHelper.SetGraphParameters (new StatisticsAxis[]
					{
						new StatisticsAxis ().SetStatisticsValues
						(
							new List<StatisticsValue>(){
								new StatisticsValue (Books[0], new int[] {coeff[0], coeff1[0]}),
								new StatisticsValue (Books[1], new int[] {coeff[1], coeff1[1]}),
								new StatisticsValue (Books[2], new int[] {coeff[2], coeff1[2]}),
								new StatisticsValue (Books[3], new int[] {coeff[3], coeff1[3]}),
							}
						).SetAxisName ("Types of Books").SetPointOffset (3),
						new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number of Boys/Girls").SetPointOffset (2)
					}
				);
				statisticsHelper.DrawGraph ();
									
				if (selector == 1)
				{
					int randSelector1 = Random.Range (0, numberOfBars);
					string expression = coeff[randSelector1] > coeff1[randSelector1]? "more": "fewer";
					subQuestionTEX.text = string.Format ("How many {0} girls than boys read {1}?", expression, Books[randSelector1]);
					Answer = string.Format ("{0}", Mathf.Abs (coeff[randSelector1] - coeff1[randSelector1])); 
				} 
				else if (selector == 2)
				{
					string expression = Random.Range (0, 2) == 0? "girls": "boys";
					subQuestionTEX.text = string.Format ("What is the total number of {0} in the class?", expression);
					int total = 0;
					if (expression == "girls") {
						foreach (int i in coeff){
							total += i;
						}
					} else {
						foreach (int i in coeff1){
							total += i;
						}
					}
					Answer = string.Format ("{0}", total);
				}
				else if (selector == 3)
				{
					SetMCQMode (4);
					string expression = Random.Range (0, 2) == 0? "girls": "boys";
					subQuestionTEX.text = string.Format ("Which are the most popular kind of books among {0}?", expression);

					if (expression == "girls") {
						Answer = string.Format ("{0}", Books[coeff.IndexOf (maxValue * gridValOffset)]);				
					} else {
						Answer = string.Format ("{0}", Books[coeff1.IndexOf (maxValue1 * gridValOffset)]);				
					}
					for (int i = 0; i < 4; i++){
						options.Add (Books [i]);
					}
					RandomizeMCQOptionsAndFill (options);
				}
			}
			#endregion
			#region level3
			else if (level == 3)
			{
				selector = GetRandomSelector (1, 5);

				if (selector == 1) 
				{
					SetStatisticsMode ();
					SetTable ();

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();

					axisValueOffset = 10 * Random.Range (1, 5);
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfBars = 5;
					do {
						minValue = Random.Range (2, 8);
						maxValue = Random.Range (10, 13);
					} while (maxValue - minValue < numberOfBars);

					List<string> Animals = new List<string> (){"Rabbit", "Monkey", "Lion", "Elephant", "Zebra", "Giraffe", "Tiger", "Leopard"};
					Animals.Shuffle ();

					for (int i = 0; i < numberOfBars; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Shuffle ();

					TableContentsColumn1.Add ("<size=25>Animals</size>");
					TableContentsColumn1.AddRange (Animals);
					TableContentsColumn2.Add ("<size=25>Number in Zoo</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					QuestionText.text = "Draw a bar graph to show the following information.\n";

					statisticsHelper.SetGridParameters (new Vector2 (18, 18), 18f);
					statisticsHelper.ShiftPosition (new Vector2 (-185f, 90f));
					statisticsHelper.SetStatisticsType (StatisticsType.HorizontalBar);
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number in Zoo").SetPointOffset (2),
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (Animals[0], coeff[0]),
									new StatisticsValue (Animals[1], coeff[1]),
									new StatisticsValue (Animals[2], coeff[2]),
									new StatisticsValue (Animals[3], coeff[3]),
									new StatisticsValue (Animals[4], coeff[4])
								}
							).SetAxisName ("Animals").SetPointOffset (3)
						}
					);
					statisticsHelper.SetInteractable (true);
					statisticsHelper.DrawGraph ();

				}
				else if (selector == 2)
				{
					SetStatisticsMode ();
					SetTable (3);

					List<string> TableContentsColumn1 = new List<string>();
					List<string> TableContentsColumn2 = new List<string>();
					List<string> TableContentsColumn3 = new List<string>();
					List<int> coeff1 = new List<int> ();

					axisValueOffset = 10 * Random.Range (1, 5);
					gridValOffset = axisValueOffset / 2;
					int minValue, minValue1, maxValue, maxValue1;
					int numberOfBars = 5;
					do {
						minValue = Random.Range (2, 8);
						maxValue = Random.Range (10, 13);
					} while (maxValue - minValue < numberOfBars);
					do {
						minValue1 = Random.Range (2, 8);
						maxValue1 = Random.Range (10, 13);
					} while (maxValue1 - minValue1 < numberOfBars);

					List<string> months = new List<string> (){"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
					months.Shuffle ();

					for (int i = 0; i < numberOfBars; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
						coeff1.Add (gridValOffset * Random.Range (minValue1 + 1, maxValue1));
					}
					coeff.Shuffle ();

					TableContentsColumn1.Add ("<size=25>Months</size>");
					TableContentsColumn1.AddRange (months);
					TableContentsColumn2.Add ("<size=25>Refrigerators</size>");
					TableContentsColumn3.Add ("<size=25>ACs</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					foreach (int i in coeff1){
						TableContentsColumn3.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());
					StatTableColumn3.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn3.ToArray ());

					QuestionText.text = "Draw a bar graph to show the following information.\n";

					statisticsHelper.SetGridParameters (new Vector2 (18, 18), 18f);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 90f));
					statisticsHelper.SetSnapValue (new Vector2 (9f, 18f));
					statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
					statisticsHelper.SetBarValues (new List<string> () {"Refrigerators", "ACs"});
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (months[0].Substring (0, 3), new int[] {coeff[0], coeff1[0]}),
									new StatisticsValue (months[1].Substring (0, 3), new int[] {coeff[1], coeff1[1]}),
									new StatisticsValue (months[2].Substring (0, 3), new int[] {coeff[2], coeff1[2]}),
									new StatisticsValue (months[3].Substring (0, 3), new int[] {coeff[3], coeff1[3]}),
									new StatisticsValue (months[4].Substring (0, 3), new int[] {coeff[4], coeff1[4]})
								}
							).SetAxisName ("Months").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number of Units").SetPointOffset (2)
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

					axisValueOffset = 10 * Random.Range (1, 5);
					gridValOffset = axisValueOffset / 2;
					int minValue, maxValue;
					int numberOfBars = 5;
					minValue = 3;
					maxValue = 10;

					List<string> weekDays = new List<string>() {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday"};

					for (int i = 0; i < numberOfBars; i++){
						coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
					}
					coeff.Shuffle ();

					QuestionText.text = "Given : Table of 'Weekdays' vs 'The number of people who visited a flower show'. Complete the given graph.";
					TableContentsColumn1.Add ("<size=25>Weekdays</size>");
					TableContentsColumn1.AddRange (weekDays);
					TableContentsColumn2.Add ("<size=25>Number</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetGridParameters (new Vector2 (18, 18), 18f);
					statisticsHelper.ShiftPosition (new Vector2 (-200, 90f));
					statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
					statisticsHelper.SetGraphParameters (new StatisticsAxis[]
						{
							new StatisticsAxis ().SetStatisticsValues
							(
								new List<StatisticsValue>(){
									new StatisticsValue (weekDays[0].Substring (0, 3), coeff[0]),
									new StatisticsValue (weekDays[1].Substring (0, 3), coeff[1]),
									new StatisticsValue (weekDays[2].Substring (0, 3), coeff[2]),
									new StatisticsValue (weekDays[3].Substring (0, 3), coeff[3]),
									new StatisticsValue (weekDays[4].Substring (0, 3), coeff[4])
								}
							).SetAxisName ("Weekdays").SetPointOffset (3),
							new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Number of People").SetPointOffset (2)
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
					List<string> pieStringData = new List<string> () {"Beach", "Trekking", "Relaxed", "Sightseeing"};
					pieStringData.Shuffle ();

					int numberOfData = 4;
					int unitData = Random.Range (5, 15);

					coeff = Random.Range (1, 3) == 1? new List<int> () {1,2,2,3}: new List<int> () {1,1,2,4};
					for (int i = 0; i < numberOfData; i++){
						coeff[i] *= unitData;
					}

					TableContentsColumn1.Add ("<size=25>Holiday</size>");
					TableContentsColumn1.AddRange (pieStringData);
					TableContentsColumn2.Add ("<size=25>Number of People</size>");
					foreach (int i in coeff){
						TableContentsColumn2.Add (i.ToString ());
					}
					StatTableColumn1.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}", TableContentsColumn1.ToArray ());
					StatTableColumn2.text = string.Format ("{0}\n{1}\n{2}\n{3}\n{4}", TableContentsColumn2.ToArray ());

					statisticsHelper.SetStatisticsType (StatisticsType.PieToFill);
					statisticsHelper.ShiftPosition (new Vector2 (-330, 110f));
					statisticsHelper.SetPieParameters (
						pieStringData,
						coeff
					);
					statisticsHelper.SetInteractable(true);
					statisticsHelper.SetPieRadius (100f); 
					statisticsHelper.DrawGraph ();

					QuestionText.text = string.Format ("{0} people were asked what kind of holiday was their favourite. Their responses are given in the table below. Colour (pick from the corresponding boxes) the circle graph accordingly.", 8 * unitData);
				}
			}
			#endregion
			#region level4
			else if (level == 4)
			{
				selector = GetRandomSelector (1, 4);
				StartSequence (3);
				subQuestionTEX.gameObject.SetActive (true);

				List<string> pieStringData = new List<string> () {"Arnav", "Nitya", "Zara", "Anvi", "Rachit"};
				pieStringData.Shuffle ();

				int numberOfData = 4;
				int unitData = Random.Range (5, 15);

				coeff = Random.Range (1, 3) == 1? new List<int> () {1,2,2,3}: new List<int> () {1,1,2,4};
				for (int i = 0; i < numberOfData; i++){
					coeff[i] *= unitData;
				}

				statisticsHelper.SetStatisticsType (StatisticsType.Pie);
				statisticsHelper.ShiftPosition (new Vector2 (-330, 200));
				statisticsHelper.SetPieParameters (
					pieStringData.GetRange (0, numberOfData),
					coeff
				);
				statisticsHelper.SetPieRadius (100f);  
				statisticsHelper.DrawGraph ();
									
				QuestionText.text = string.Format ("This circle graph shows the votes for the class election. There are {0} students in the class.", 8 * unitData);

				if (selector == 1)
				{
					int randSelector1 = Random.Range (0, numberOfData);
					subQuestionTEX.text = string.Format ("Estimate the number of votes {0} got?", pieStringData[randSelector1]);
					Answer = string.Format ("{0}", coeff[randSelector1]);
				}
				else if (selector == 2)
				{
					SetMCQMode (4);

					subQuestionTEX.text = "Who won the election?";
					for (int i = 0; i < 4; i++){
						options.Add (pieStringData[i]);
					}
					Answer = pieStringData[coeff.IndexOf (coeff.Max ())];
					RandomizeMCQOptionsAndFill (options);
				}
				else if (selector == 3)
				{
					int randSelector1;
					int randSelector2;
					do {
						randSelector1 = Random.Range (0, numberOfData);
						randSelector2 = Random.Range (0, numberOfData);
					} while (randSelector1 == randSelector2);

					string expression = coeff[randSelector1] > coeff[randSelector2]? "more": "fewer";
					subQuestionTEX.text = string.Format ("How many {0} votes did {1} get than {2}?", expression, pieStringData[randSelector1], pieStringData[randSelector2]);
					Answer = string.Format ("{0}", Mathf.Abs (coeff[randSelector1] - coeff[randSelector2]));
				}
			}
			#endregion
			#region level5
			else if (level == 5)
			{
				selector = GetRandomSelector (1, 3);
				subQuestionTEX.gameObject.SetActive (true);
				StartSequence (2);

				List<string> pieStringData = new List<string> () {"Maths", "Physics", "Biology", "Chemistry", "English", "Economy", "Philosophy", "Computer"};
				pieStringData.Shuffle ();

				int numberOfData = 4;
				int unitData = Random.Range (5, 15);

				coeff = Random.Range (1, 3) == 1? new List<int> () {1,2,2,3}: new List<int> () {1,1,2,4};
				for (int i = 0; i < numberOfData; i++){
					coeff[i] *= unitData;
				}

				statisticsHelper.SetStatisticsType (StatisticsType.Pie);
				statisticsHelper.ShiftPosition (new Vector2 (-330, 200));
				statisticsHelper.SetPieParameters (
					pieStringData.GetRange (0, numberOfData),
					coeff
				);
				statisticsHelper.SetPieRadius (100f); 
				statisticsHelper.DrawGraph ();					

				QuestionText.text = string.Format ("This circle graph shows the number of days Tirthan spent studying different subjects out of a total study time of {0} days.", coeff.Sum ());

				if (selector == 1)
				{
					int randSelector1 = Random.Range (0, numberOfData);
					int hcf = MathFunctions.GetHCF (coeff[randSelector1], coeff.Sum ());
					subQuestionTEX.text = string.Format ("Give the fraction shown by the circle graph for {0}.", pieStringData[randSelector1]);
					Answer = string.Format ("{0}/{1}", coeff[randSelector1] / hcf, coeff.Sum () / hcf);
				}
				else if (selector == 2)
				{
					int randSelector1 = Random.Range (0, numberOfData);
					subQuestionTEX.text = string.Format ("How many days did he spend studying {0} in that period?", pieStringData[randSelector1]);
					Answer = string.Format ("{0}", coeff[randSelector1]);
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
				if (checkLastTextFor (new string[1]{ "/" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
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