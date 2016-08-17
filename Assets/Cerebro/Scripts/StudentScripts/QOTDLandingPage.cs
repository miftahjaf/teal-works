using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Cerebro
{
	public class SubmittedQuizData
	{
		public string quizDate;
		public int totalAttempts;
		public int totalCorrect;
		public int score;
	}

	public class QOTDLandingPage : MonoBehaviour
	{

		private System.DateTime CurrentDate;
		private System.DateTime OldestDate;
		private System.DateTime TodayDate;

		public GameObject QOTDContainer;
		public GameObject landingPage;
		public GameObject progressCircle;
		public GameObject prevDateBtn;
		public GameObject nextDateBtn;
		public GameObject fetchQuizBtn;

		public GameObject Card;
		public GameObject ScoreCard;

		public Text currentDate;
		public Text nextDate;
		private bool isAnimating = false;

		public Text message;
		public Text cardTitle;
		public GameObject cardIcon;
		public GameObject startButton;

		private List<QuizData> allQuestions;
		private SubmittedQuizData quizGivenData;
		private bool isTestUser = false;

		// Use this for initialization
		void Start ()
		{
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.DailyQuiz);

			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			GetComponent<RectTransform> ().position = new Vector3 (0, 0);

			fetchQuizBtn.SetActive (false);

			isTestUser = CerebroHelper.isTestUser ();

			OldestDate = System.DateTime.ParseExact ("20160412", "yyyyMMdd", null);
			TodayDate = System.DateTime.Now;

			CurrentDate = TodayDate;
			SetDate (false);
			message.text = "";
			ScoreCard.SetActive (false);
			Card.SetActive (true);
			FetchQuizForDate (CurrentDate.ToString ("yyyyMMdd"));
		}

		public void nextDatePressed ()
		{
			if (isAnimating) {
				return;
			}
			if (CurrentDate.ToString ("yyyyMMdd") != TodayDate.ToString ("yyyyMMdd") || isTestUser) {
				CurrentDate = CurrentDate.AddDays (1);
				SetDate (true,false);

				message.text = "";
				cardTitle.text = "Fetch Quiz";
				ScoreCard.SetActive (false);
				Card.SetActive (true);
				cardIcon.SetActive (false);


				startButton.SetActive (false);
				fetchQuizBtn.SetActive (true);
			}
		}

		public void previousDatePressed ()
		{
			if (isAnimating) {
				return;
			}
			if (CurrentDate.ToString ("yyyyMMdd") != OldestDate.ToString ("yyyyMMdd") || isTestUser) {
				CurrentDate = CurrentDate.AddDays (-1);
				SetDate (true,true);

				message.text = "";
				cardTitle.text = "Fetch Quiz";
				ScoreCard.SetActive (false);
				Card.SetActive (true);
				cardIcon.SetActive (false);

				startButton.SetActive (false);
				fetchQuizBtn.SetActive (true);
			}
		}


		private void SetDate (bool shouldAnimate, bool animateLeft = false)
		{
			if (!shouldAnimate) {
				currentDate.text = CurrentDate.ToString ("MMM dd, yyyy");
			} else {
				nextDate.text = CurrentDate.ToString ("MMM dd, yyyy");
				StartCoroutine (AnimateDate (animateLeft));
			}

			if (CurrentDate.ToString ("yyyyMMdd") == OldestDate.ToString ("yyyyMMdd") && !isTestUser) {
				prevDateBtn.SetActive (false);
				nextDateBtn.SetActive (true);
			} else if (CurrentDate.ToString ("yyyyMMdd") == TodayDate.ToString ("yyyyMMdd") && !isTestUser) {
				nextDateBtn.SetActive (false);
				prevDateBtn.SetActive (true);
			} else {
				nextDateBtn.SetActive (true);
				prevDateBtn.SetActive (true);
			}
		}

		IEnumerator AnimateDate(bool animateLeft) {
			isAnimating = true;
			if (animateLeft) {
				nextDate.transform.localPosition = new Vector3 (-1024, nextDate.transform.localPosition.y, nextDate.transform.localPosition.z);
				Go.to (currentDate.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (1024, 0, 0), true));
				Go.to (nextDate.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (1024, 0, 0), true));
			} else {
				nextDate.transform.localPosition = new Vector3 (1024, nextDate.transform.localPosition.y, nextDate.transform.localPosition.z);
				Go.to (currentDate.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (-1024, 0, 0), true));
				Go.to (nextDate.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (-1024, 0, 0), true));
			}
			yield return new WaitForSeconds (0.5f);
			Text tmp = currentDate;
			currentDate = nextDate;
			nextDate = tmp;
			isAnimating = false;
		}

		public void FetchQuizButtonPressed ()
		{
			FetchQuizForDate (CurrentDate.ToString ("yyyyMMdd"));
		}

		private void FetchQuizForDate (string date)
		{
			fetchQuizBtn.SetActive (false);
			nextDateBtn.SetActive (false);
			prevDateBtn.SetActive (false);
			progressCircle.SetActive (true);
			startButton.SetActive (false);
			message.text = "";
			cardTitle.text = "Fetching Quiz";
			cardIcon.SetActive (false);
			ScoreCard.SetActive (false);
			Card.SetActive (true);
			LaunchList.instance.GetQuizForDate (date, GotQuiz);
		}

		void GotQuiz (List<QuizData> questions)
		{
			if (this == null) {  // object has been destroyed while fetching quiz
				return;
			}

			SetDate (false);

			if (questions != null) {
				
				if (questions.Count == 0) {
					message.text = "Oh no! No quiz has been prepared yet. Check back later";
					progressCircle.SetActive (false);
					startButton.SetActive (true);
					startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Fetch";
					cardTitle.text = "Fetch Quiz";
					cardIcon.SetActive (true);
					cardIcon.GetComponent<Image>().sprite = Resources.Load <Sprite> ("Images/WarningIcon");
					cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("FF5541");
					return;
				} else {
					quizGivenData = CheckForQuizSubmitted (questions [0].QuizDate);
					message.text = "Press the Start button to start your quiz of the day!";
					cardTitle.text = "Start Quiz";
					cardIcon.SetActive (true);
					cardIcon.GetComponent<Image>().sprite = Resources.Load <Sprite> ("Images/QuizIcon");
					cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("29CDB1");
					progressCircle.SetActive (false);
					startButton.SetActive (true);
					startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Start";
				}

				if (quizGivenData != null) {
					ScoreCard.SetActive (true);
					Card.SetActive (false);
					ScoreCard.transform.Find ("Score").GetComponent<Text> ().text = quizGivenData.score.ToString();
					ScoreCard.transform.Find ("Attempts").GetComponent<Text> ().text = quizGivenData.totalAttempts.ToString();
					ScoreCard.transform.Find ("Correct").GetComponent<Text> ().text = quizGivenData.totalCorrect.ToString();
				}

				allQuestions = questions;
				var studentID = "";
				if (PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
					studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
				}

				for (var i = 0; i < allQuestions.Count; i++) {
					QuizData curQuestion = allQuestions [i];
					var answer = GetUserAnswerFromLocalFile (curQuestion.QuizDate, studentID + "Q" + curQuestion.QuestionID);
					if (answer != null) {
						curQuestion.userAnswer = answer;
					}
				}
			} else {
				message.text = "You need internet to use this feature. Press the fetch button to try again";
				progressCircle.SetActive (false);
				startButton.SetActive (true);
				startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Fetch";
			}
			return;
		}

		public void BackPressed ()
		{
			//StudentScript ss = GameObject.FindGameObjectWithTag ("StudentView").GetComponent<StudentScript> ();
			//ss.Initialise ();
			WelcomeScript.instance.ShowScreen (false);
			Destroy (gameObject);	
		}

		public void StartQuizPressed ()
		{
			string buttonTxt = startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text;
			if (buttonTxt == "Fetch") {
				LaunchList.instance.GetQuizForDate (CurrentDate.ToString ("yyyyMMdd"), GotQuiz);
				progressCircle.SetActive (true);
				startButton.SetActive (false);
				message.text = "";
				cardTitle.text = "Fetching Quiz";
				cardIcon.SetActive (false);
			} else {
				landingPage.SetActive (false);
				GameObject QOTD = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.QOTD, QOTDContainer.transform);
				QOTD.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 708f);
				QOTD.GetComponent<RectTransform> ().position = new Vector3 (0, 0);
				QOTD.GetComponent<QOTD> ().Initialize (allQuestions, quizGivenData);
			}
		}

		public void BackOnScreen ()
		{
			landingPage.SetActive (true);
			quizGivenData = CheckForQuizSubmitted (allQuestions [0].QuizDate);
			if (quizGivenData != null) {
				ScoreCard.SetActive (true);
				Card.SetActive (false);
				ScoreCard.transform.Find ("Score").GetComponent<Text> ().text = quizGivenData.score.ToString();
				ScoreCard.transform.Find ("Attempts").GetComponent<Text> ().text = quizGivenData.totalAttempts.ToString();
				ScoreCard.transform.Find ("Correct").GetComponent<Text> ().text = quizGivenData.totalCorrect.ToString();

				for (var i = 0; i < allQuestions.Count; i++) {
					QuizData curQuestion = allQuestions [i];
					curQuestion.userAnswer = "";
				}
			}
		}

		public void ViewLeaderboardPressed () {
			GameObject leaderboard = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.QOTDLeaderboard, transform);
			leaderboard.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			leaderboard.GetComponent<RectTransform> ().position = new Vector3 (0, 0);
			leaderboard.GetComponent<QOTDLeaderboard> ().Initialise (allQuestions [0].QuizDate);
		}

		public void ViewAnswersPressed () {
			landingPage.SetActive (false);

			var studentID = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			}

			GameObject QOTD = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.QOTD, QOTDContainer.transform);
			QOTD script = QOTD.GetComponent<QOTD> ();
			QOTD.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 708f);
			QOTD.GetComponent<RectTransform> ().position = new Vector3 (0, 0);

			for (var i = 0; i < allQuestions.Count; i++) {
				QuizData curQuestion = allQuestions [i];
				var answer = script.GetUserAnswerFromHistoryFile (curQuestion.QuizDate, studentID + "Q" + curQuestion.QuestionID);
				if (answer != null) {
					curQuestion.userAnswer = answer;
				}
			}
			script.Feedback.SetActive (false);
			script.Initialize (allQuestions, quizGivenData,true,true);
		}

		public void RevisitQuizPressed () {
			landingPage.SetActive (false);

			GameObject QOTD = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.QOTD, QOTDContainer.transform);
			QOTD script = QOTD.GetComponent<QOTD> ();
			QOTD.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 708f);
			QOTD.GetComponent<RectTransform> ().position = new Vector3 (0, 0);
			script.Feedback.SetActive (false);
			script.Initialize (allQuestions, quizGivenData, true, false);
		}

		public string GetUserAnswerFromLocalFile (string quizDate, string StudentAndQuestionID)
		{
			string fileName = Application.persistentDataPath + "/QuizAnalytics.txt";


			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET");
				return null;
			}

			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;
				if (line != null) {
					splitArr = line.Split ("," [0]);
					if (quizDate == splitArr [6] && StudentAndQuestionID == splitArr [1]) {
						sr.Close ();
						return splitArr [2];
					}
				}

				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						splitArr = line.Split ("," [0]);
						if (quizDate == splitArr [6] && StudentAndQuestionID == splitArr [1]) {
							sr.Close ();
							return splitArr [2];
						}
					}
				}  
				sr.Close ();
			}
			return null;
		}

		public SubmittedQuizData CheckForQuizSubmitted (string quizDate)
		{
			string fileName = Application.persistentDataPath + "/QuizSubmitted.txt";
			CerebroHelper.DebugLog ("Checking for " + quizDate);
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;

				while (line != null) {
					splitArr = line.Split ("," [0]);
					CerebroHelper.DebugLog ("found " + splitArr [0]);
					if (quizDate == splitArr [0]) {
						SubmittedQuizData data = new SubmittedQuizData ();
						data.quizDate = splitArr [0];
						data.totalAttempts = int.Parse (splitArr [1]);
						data.totalCorrect = int.Parse (splitArr [2]);
						data.score = int.Parse (splitArr [3]);
						sr.Close ();
						return data;
					}
					line = sr.ReadLine ();
				}  
				sr.Close ();
			}
			return null;
		}
	}
}
