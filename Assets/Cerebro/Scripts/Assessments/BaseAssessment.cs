using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;
using System.IO;

namespace Cerebro {

	public abstract class BaseAssessment:MonoBehaviour {
		private string mSubjectID= "M";
		private string mTopicID = "Alg06";
		private string mSubTopicID = "SO1";
		private string mAssessmentID = "A01";

		public string assessmentName;			//Name of the assessment (Simple Interest, Algebra, etc)

		protected string QuestionsAttemptedKey;	//PlayerPrefs Key to store questions attempted in the current level
		protected string CurrentLevelKey;		//PlayerPrefs Key to store the current level

		public Text QuestionText;				//Main Question Text UI
		public TEXDraw QuestionLatext;			//Main Question with Latex requirements

		private AssessmentScript parentAssessmentScript;	//Main Assessment Holder
		private Revisit revisitScript;	//Main Assessment Holder

		public GameObject numPad;				//numPad animation
		public GameObject inputPanel;			//Contains gameObjects other than Main Question and numPad. This section animates from the top.

		protected Text userAnswerText;			//User Input TextBox
		protected TEXDraw userAnswerLaText;		//User Input TextBox Latex
		protected Button answerButton;			//which button is currently accepting answer. There could be multiple Buttons on the screen.

		public GameObject ContinueBtn;			//Comes up on incorrect answer given
		public GameObject FlagButton;			//Comes up on incorrect answer given
		public GameObject SolutionButton;		//Comes up on incorrect answer given

		protected int selector;					//Type of question under each level. Random number within a range for each question
		protected int[] scorestreaklvls;		//Array contains current correct streak of each level

		protected bool ignoreTouches = false;	//To stop user input on the keypad

		protected int questionsAttempted;		//Total Questions Attempted for a particular level of a particular assessment

		protected int Queslevel;				//Current level of the user for particular assessment. Could be the last level in which random question from any other level is chosen
		protected int level;					//Level of the current question shown on screen

		public Button GeneralButton;			//General Button for user Input Answer

		private int randomSeed;					//Seed used to generate each question
		protected bool levelUp;					//Set this to true to show levelUp animation between two questions.

		public bool isDestroyed = false;

		public bool isRevisitedQuestion = false;
		public Dictionary<string, string> revisitedQuestionData;

		public MissionItemData missionItemData;
		protected int forceSelector;

		private int currentQuestionDifficulty;
		private string currentQuestionAssessmentKey;

		public bool testMode = false;
		public int testSelector = 0;
		public int testQuestionLevel = 1;

		private bool didLevelUp = false;
		private Explanation currentExplanation;

		protected void Initialise(string subjectID, string topicID, string subTopicID, string assessmentID) {
			parentAssessmentScript = gameObject.transform.parent.GetComponent<AssessmentScript> ();
			revisitScript = gameObject.transform.parent.GetComponent<Revisit> ();
			mSubjectID= subjectID;
			mTopicID = topicID;
			mSubTopicID = subTopicID;
			mAssessmentID = assessmentID;
			string PracticeItemID = subjectID + topicID + subTopicID + assessmentID;
			QuestionsAttemptedKey = PracticeItemID + "QuestionsAttempted";
			CurrentLevelKey = PracticeItemID + "LevelKey";

			if (parentAssessmentScript != null) {
				parentAssessmentScript.mPracticeID = subjectID + topicID + subTopicID + assessmentID;
			}

			questionsAttempted = 0;
			if (PlayerPrefs.HasKey (QuestionsAttemptedKey)) {
				questionsAttempted = PlayerPrefs.GetInt (QuestionsAttemptedKey);
			} else {
				updateQuestionsAttempted ();
			}

			if (PlayerPrefs.HasKey (CurrentLevelKey)) {
				Queslevel = PlayerPrefs.GetInt (CurrentLevelKey);
			} else {
				SetLevelToLocalDB (1);
				Queslevel = 1;	
			}
		}

		public string GetPracticeItemID() {
			return mSubjectID + mTopicID + mSubTopicID + mAssessmentID;
		}
		protected void QuestionStarted() {
			forceSelector = -1;
			didLevelUp = false;
			if (parentAssessmentScript != null) {
				parentAssessmentScript.testingText.text = "";
			}

			if (isRevisitedQuestion) {
				Queslevel = int.Parse (revisitedQuestionData ["difficulty"]);
				forceSelector = int.Parse (revisitedQuestionData ["sublevel"]);
				randomSeed = int.Parse (revisitedQuestionData ["seed"]);
				Random.seed = randomSeed;
			} else if (missionItemData != null) {
				int randomIndex = -1;

				if (missionItemData.QuestionLevel.Contains ("@")) {
					string[] options = missionItemData.QuestionLevel.Split ("@"[0]);
					List<int> levels = new List<int> ();
					foreach (var str in options) {
						levels.Add(int.Parse (str));
					}
					randomIndex = Random.Range (0, levels.Count);
					Queslevel = levels [randomIndex];
				} else {
					Queslevel = int.Parse (missionItemData.QuestionLevel);
					if (missionItemData.QuestionLevel == "-1") {
						Queslevel = scorestreaklvls.Length + 1;
					}
				}

				if (missionItemData.SubLevel.Contains ("@")) {
					string[] options = missionItemData.SubLevel.Split ("@"[0]);
					List<int> levels = new List<int> ();
					foreach (var str in options) {
						levels.Add(int.Parse (str));
					}
					if (randomIndex == -1) {
						randomIndex = Random.Range (0, levels.Count);
					}
					forceSelector = levels [randomIndex];
				} else {
					if (missionItemData.SubLevel != "-1") {
						forceSelector = int.Parse (missionItemData.SubLevel);
					}
				}
				if((WelcomeScript.instance.autoTestMissionMix || WelcomeScript.instance.autoTestMissionCorrect)&& CerebroHelper.isTestUser())
					StartCoroutine (autoCorrectMission());
			} else if (testMode && scorestreaklvls != null) {
				Queslevel = testQuestionLevel;
				if (Queslevel > scorestreaklvls.Length) {
					if (WelcomeScript.instance.testingAllScreens) {
						WelcomeScript.instance.GoToNextChapter ();
					}
					parentAssessmentScript.testingText.text = "Done";
				}
			} else {
//				randomSeed = Mathf.RoundToInt (Time.time * 10000) + Random.Range (0, 999999);
//				Random.seed = randomSeed;
			}
			if (parentAssessmentScript != null) {
				parentAssessmentScript.QuestionStarted ();
			}
		}

		IEnumerator autoCorrectMission()
		{
			yield return new WaitForSeconds (1f);
			if (WelcomeScript.instance.autoTestMissionCorrect == true && CerebroHelper.isTestUser()) {
				QuestionEnded (true, level, 10, selector);
			} else {
				if (Random.Range (0, 50) % 2 == 0)
					QuestionEnded (true, level, 10, selector);
				else
					QuestionEnded (false, level, 10, selector);
			}
			yield return new WaitForSeconds (1f);
			GenerateQuestion ();
		}

		private void WriteToFile(string name, string level, string type) {
			string fileName = Application.persistentDataPath + "/Misbah.txt";
			StreamWriter sr;
			if (File.Exists (fileName)) {
				sr = File.AppendText (fileName);
			} else {
				sr = File.CreateText (fileName);
			}
			sr.WriteLine (name + "," + level + "," + type);
			sr.Close ();
		}

		protected int GetRandomSelector(int lowerLimit, int upperLimit) {
			// To CerebroHelper.DebugLog out max number in each level and write it to file.
			// FYI uncomment the below and go into TestScreens to spit out levels of difficulty and subtype per level
			// in a file called Misbah.txt
			/*if (testQuestionLevel <= scorestreaklvls.Length) {
				WriteToFile (parentAssessmentScript.mPracticeName, testQuestionLevel.ToString(), (upperLimit-lowerLimit).ToString());
				testQuestionLevel++;
				GenerateQuestion ();
				return lowerLimit;
			}*/

			int randomSelector = -1;

			if (forceSelector != -1) {
				if (forceSelector >= lowerLimit && forceSelector < upperLimit) {
					randomSelector = forceSelector;
				}
			} else if (testMode && testQuestionLevel <= scorestreaklvls.Length) {
				if (testSelector+lowerLimit < upperLimit) {
					testSelector++;
					int returnSelector = (testSelector + lowerLimit) - 1;
					if (parentAssessmentScript != null) {
						parentAssessmentScript.testingText.text = testQuestionLevel.ToString () + "t" + testSelector.ToString ();
					}
					if(testSelector+lowerLimit == upperLimit) {
						testSelector = 0;
						testQuestionLevel++;
					}
					randomSelector = returnSelector;
				}
			}
			if (randomSelector == -1) {
				randomSelector = Random.Range (lowerLimit, upperLimit);
			}
			if (isRevisitedQuestion) {
				randomSeed = int.Parse (revisitedQuestionData ["seed"]);
			} else {
				randomSeed = Mathf.RoundToInt (Time.time * 10000) + Random.Range (0, 999999);
			}
			Random.seed = randomSeed;

			return randomSelector;
		}

		protected void QuestionEnded(bool isCorrect, int difficulty, int _increment, int type) {
			CerebroHelper.DebugLog ("Question Ended " + isCorrect);

			string uniqueTime = System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss");
			if (isRevisitedQuestion) {
				currentQuestionAssessmentKey = revisitedQuestionData ["assessmentKey"];
				currentQuestionDifficulty = int.Parse(revisitedQuestionData ["difficulty"]);
			} else {
				currentQuestionAssessmentKey = mSubjectID + mTopicID + mSubTopicID + mAssessmentID + "Z" + uniqueTime + "t" + type;
				if (didLevelUp) {
					currentQuestionAssessmentKey = currentQuestionAssessmentKey + "LEVEL_UP";
				}
				currentQuestionDifficulty = difficulty;
			}
			if (parentAssessmentScript != null) {
				parentAssessmentScript.QuestionEnded (isCorrect, difficulty, _increment, currentQuestionAssessmentKey, randomSeed, type);
			}
		}

		protected void showNextQuestion() {
			if (revisitScript != null) {
				revisitScript.ShowFlagButton ();
			} else {
				GenerateQuestion ();
				StartCoroutine(StartAnimation());
			}
		}

		protected void LevelUp() {
			levelUp = false;
			if (parentAssessmentScript != null) {
				parentAssessmentScript.AnimateLevelUp ();
			}
		}
		protected void SetLevelToLocalDB(int level) {
			PlayerPrefs.SetInt (CurrentLevelKey, level);
		}
		protected void updateQuestionsAttempted() {
			PlayerPrefs.SetInt (QuestionsAttemptedKey, questionsAttempted);
		}

		protected IEnumerator StartAnimation() {
			GameObject GO = null;
			if (QuestionText != null) {
				GO = QuestionText.gameObject;
			}
			if (QuestionLatext != null) {
				GO = QuestionLatext.gameObject;
			}
			if (GO != null) {
				GO.transform.localScale = new Vector3 (1, 1, 1);
				GO.transform.position = new Vector3 (GO.transform.position.x - 1024, GO.transform.position.y, GO.transform.position.z);
				inputPanel.gameObject.transform.localScale = new Vector3 (1, 0, 1);
				Go.to (GO.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (1024, 0, 0), true).setEaseType (GoEaseType.BackOut));
				yield return new WaitForSeconds (0.3f);
				Go.to (inputPanel.gameObject.transform, 0.3f, new GoTweenConfig ().scale (new Vector3 (0, 1, 1), true).setEaseType (GoEaseType.BackOut));
				Go.to (numPad.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (numPad.transform.position.x, 0, 0), false));
			}
		}

		protected IEnumerator HideAnimation() {
			GameObject GO = new GameObject ();
			if (QuestionText != null) {
				GO = QuestionText.gameObject;
			}
			if (QuestionLatext != null) {
				GO = QuestionLatext.gameObject;
			}
			if (GO != null) {
				GO.transform.localScale = new Vector3 (1, 0, 0);
				inputPanel.gameObject.transform.localScale = new Vector3 (1, 0, 0);
				yield return new WaitForSeconds (0.1f);
			}
		}

		protected void UpdateStreak(int correctRequired, int questionsRequired) {
			if (Queslevel <= scorestreaklvls.Length) {
				scorestreaklvls [Queslevel - 1] = scorestreaklvls [Queslevel - 1] + 1;
				if (scorestreaklvls [Queslevel - 1] >= correctRequired && questionsAttempted > questionsRequired) {
					didLevelUp = true;
					levelUp = true;
					Queslevel = Queslevel + 1;    
					questionsAttempted = 0;
					SetLevelToLocalDB (Queslevel);
				}
			}
		}

		public virtual void ContinueButtonPressed() {
			showNextQuestion ();
			HideContinueButton ();
			if (userAnswerText != null) {
				userAnswerText.color = MaterialColor.textDark;
			} 
			if (userAnswerLaText != null) {
				userAnswerLaText.color = MaterialColor.textDark;
			}
		}

		protected bool checkLastTextFor(string[] strs) {
			string toCheck = "";
			if (userAnswerText != null) {
				toCheck = userAnswerText.text;
			} else if (userAnswerLaText != null) {
				toCheck = userAnswerLaText.text;
			}
			if (toCheck.Length > 0) {
				for (var i = 0; i < strs.Length; i++) {
					if (toCheck.Length >= strs [i].Length) {
						string checkVar = toCheck.Substring (toCheck.Length - strs [i].Length, strs [i].Length);
						if (checkVar == strs [i]) {
							return true;
						}
					}
				}
			}
			return false;
		}

		protected void ShowContinueButton() {
			if (isRevisitedQuestion) {
				return;
			}
			ContinueBtn.SetActive (true);
			FlagButton.SetActive (true);

			currentExplanation = null;
			if (parentAssessmentScript != null) {
				string key = parentAssessmentScript.mPracticeID + "L" + level + "t" + selector;
				foreach (var item in LaunchList.instance.mExplanation) {
					CerebroHelper.DebugLog (item.Key);
				}
				if (LaunchList.instance.mExplanation.ContainsKey (key)) {
					currentExplanation = LaunchList.instance.mExplanation[key];
					SolutionButton.SetActive (true);
				}
			}

			var buttonText = FlagButton.transform.Find ("Text").gameObject.GetComponent<Text> ();
			if (isRevisitedQuestion) {
				buttonText.text = "Unflag";
			} else {
				buttonText.text = "Flag";
			}
			Go.to (numPad.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (numPad.transform.position.x, -Screen.height, 0), false));
		}
			
		protected void HideContinueButton() {
			if (isRevisitedQuestion) {
				return;
			}
			if (!isDestroyed) {
				Go.to (numPad.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (numPad.transform.position.x, 0, 0), false));
				ContinueBtn.SetActive (false);
				FlagButton.SetActive (false);
				SolutionButton.SetActive (false);
			}
		}

		public virtual void FlagButtonPressed() {
			var buttonText = FlagButton.transform.Find ("Text").gameObject.GetComponent<Text> ();
			if (buttonText.text == "Flag") {
				LaunchList.instance.WriteFlaggedQuestionToFile (currentQuestionAssessmentKey, currentQuestionDifficulty, selector, randomSeed);
				buttonText.text = "Unflag";
			} else {
				buttonText.text = "Flag";
				CerebroHelper.DebugLog ("removing" + currentQuestionAssessmentKey);
				LaunchList.instance.RemoveFlaggedQuestionFromFile (currentQuestionAssessmentKey);
			}
		}

		public void TestNextQuestion() {
			GenerateQuestion ();
		}

		public virtual void SolutionButtonPressed() {
			VideoHelper.instance.VideoEnded += CloseWebView;
			VideoHelper.instance.OpenVideoWithUrl (currentExplanation.URL);
		}

		void CloseWebView(object sender, System.EventArgs e) {
			VideoEventArgs eventArgs = e as VideoEventArgs;
			CerebroHelper.DebugLog (eventArgs);
			string day = System.DateTime.Now.ToUniversalTime ().ToString ("yyyyMMdd");

			var videoWatched = false;
			if (eventArgs.videoLength != -1 && eventArgs.timeSpent >= 0.8 * eventArgs.videoLength) {
				videoWatched = true;
			}

			var totalimeTaken = eventArgs.timeEnd - eventArgs.timeIni;
			string uniqueTime = eventArgs.timeEnded;
			string key = mSubjectID + mTopicID + mSubTopicID + mAssessmentID;
			string uniquekey = key + "Z" + uniqueTime + "t" + selector;
			string contentKey = "SOLUTION_" + uniquekey + "CONTENT_" + currentExplanation.ContentId;

			WelcomeScript.instance.ShowRatingPopup ("SOLUTION", eventArgs.timeSpent, currentExplanation.ContentId, "How many stars would you give to this video?");

			Cerebro.LaunchList.instance.WriteAnalyticsToFile (contentKey, currentQuestionDifficulty, videoWatched, day, eventArgs.timeStarted, Mathf.FloorToInt (totalimeTaken), Mathf.FloorToInt (eventArgs.timeSpent).ToString(), 0, " " );  

			VideoHelper.instance.VideoEnded -= CloseWebView;
		}

		public abstract void numPadButtonPressed (int value);
		public abstract void SubmitClick();

		protected abstract void GenerateQuestion();
		protected abstract IEnumerator ShowWrongAnimation ();
		protected abstract IEnumerator ShowCorrectAnimation ();
	}
}
