using UnityEngine;
using System.Collections;
using MaterialUI;

using System;

using System.Collections.Generic;
using System.Threading;
using System.Linq;

using System.Net;
using System.IO;

using UnityEngine.UI;
using SimpleJSON;
using System.Runtime.InteropServices;

namespace Cerebro
{
	// XXX keep this enum synced with mTableNames below
	enum TableTypes
	{
		tSubject,
		tTopic,
		tSubtopic,
		tContent,
		tStudent,
		tProperties,
		tPracticeItems,
		kMaxTables}

	;

	public class LaunchList : MonoBehaviour
	{
		
		public List<Subject> mSubjects = new List<Subject> ();
		public List<Topic> mTopics = new List<Topic> ();
		public List<Subtopic> mSubtopics = new List<Subtopic> ();
		public List<ContentItem> mContentItems = new List<ContentItem> ();
		public List<Student> mStudents = new List<Student> ();
		public List<World> mWorld = new List<World> ();
		public Student mCurrentStudent = new Student ();
		public CellGame mCurrentGame = new CellGame ();
		public Moves mCurrentMove = null;
		public List<Moves> mMoves = new List<Moves> ();
		public List<QuizData> mQuizQuestions = new List<QuizData> ();
		public List<QuizAnalytics> mQuizAnalytics = new List<QuizAnalytics> ();
		public Missions mMission = new Missions ();
		public Dictionary<string, PracticeItems> mPracticeItems = new Dictionary<string, PracticeItems> ();
		public DescribeImage mDescribeImage;
		public Verbalize mVerbalize;
		public bool VerbalizeSaving;
		public List<Verbalize> UploadingVerbalize;
		public List<DescribeImageUserResponse> mDescribeImageUserResponses = new List<DescribeImageUserResponse> ();
		public List<LeaderBoard> mLeaderboard = new List<LeaderBoard> ();
		public Dictionary<string,Explanation> mExplanation = new Dictionary<string,Explanation> ();
		public List<GOTGameStatus> mGameStatus = new List<GOTGameStatus> ();
		public List<StartableGame> mStartableGames = new List<StartableGame> ();
		public Dictionary<string,int> mKCCoins =new Dictionary<string, int>();

		public bool mUpdateTimer = false;
		public System.TimeSpan mTimer;
		int interval = 1; 
		float nextTime = 0;

		private bool IAmServer = false;
		// analytics table intentionally not added here
		private string[] mTableNames = new string[] { "Subject", "Topic", "Subtopic", "ContentItem", "Student" };
		public bool[] mTableLoaded = new bool[] { false, false, false, false, false, false, false };

		private bool mTablesLoaded = false;

		private bool allTablesLoaded = false;

		public bool mCoinsValueChanged = false;
		public bool mDoingCoinsUpdate = false;

		public bool mDoingWorldUpdate = false;
		public bool mPollMovesClient = false;
		public bool mWaitingforMovesandWorldServer = false;
		public bool mProcessingMoves = false;
		public bool mPollingMovesTable = false;
		public bool mGettingQuiz = false;
		public bool mGettingQuizAnalytics = false;

		public event EventHandler HandleAllTablesLoaded;
		public event EventHandler WorldChanged;
		public event EventHandler LeaderboardLoaded;
		public event EventHandler MissionLoaded;
		public event EventHandler DescribeImageLoaded;
		public event EventHandler DescribeImageResponsesLoaded;
		public event EventHandler DescribeImageResponseSubmitted;
		public event EventHandler VerbalizeTextLoaded;

		public bool mhasInternet;
		private bool mcheckingInternet;

		public GameObject wifiOff;

		private InternetReachabilityVerifier irv;

		private bool firstTimeNetCheck = true;

		public string mCurrLocalTempPath;
		public bool WelcomeScreenActive = true;
		private static LaunchList m_Instance;

		private bool mHitDynamoDB = false;
		private bool mHitServer = true;

		public static LaunchList instance {
			get {
//				if (m_Instance == null)
//				{
//					m_Instance = new LaunchList();
//				}

				return m_Instance;
			}
		}
			

		void Awake ()
		{
			if (m_Instance != null && m_Instance != this) {
				CerebroHelper.DebugLog ("Destroying this shit");
				m_Instance.wifiOff = GameObject.Find ("WifiOff");
				(m_Instance.wifiOff.GetComponent<Button> ()).onClick.AddListener (() => m_Instance.checkWifi ());
				m_Instance.setWifiIcon ();
				if (m_Instance.HandleAllTablesLoaded != null) {
					m_Instance.HandleAllTablesLoaded.Invoke (this, null);
				} else {
					CerebroHelper.DebugLog ("NO HANDLE ALL TABLES");
				}
				Destroy (gameObject);
				return;
			}

			wifiOff = GameObject.Find ("WifiOff");
			wifiOff.GetComponent<Button> ().onClick.AddListener (() => checkWifi ());

			m_Instance = this;

			DontDestroyOnLoad (transform.gameObject);

			Debug.Log("Asking for temp path");
			#if UNITY_IOS && !UNITY_EDITOR
			_GetTempDirectory("Temp Path");
			#endif
		}

		public void LogoutUser() {
			if (WelcomeScript.instance) {
				WelcomeScript.instance.DestroyPooledPrefabs ();
			}
			mSubjects.Clear ();
			mTopics.Clear ();
			mSubtopics.Clear ();
			mContentItems.Clear ();
			mStudents.Clear ();
			mWorld.Clear ();
			mCurrentStudent = new Student ();
			mCurrentGame = new CellGame ();
			mCurrentMove = null;
			mMoves.Clear ();
			mQuizQuestions.Clear ();
			mQuizAnalytics.Clear ();
			mMission = new Missions ();
			mPracticeItems.Clear ();
			mKCCoins.Clear ();
			mDescribeImageUserResponses.Clear ();
			mLeaderboard.Clear ();
			mExplanation.Clear ();
			mGameStatus.Clear ();

			PlayerPrefs.DeleteAll ();
			DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
			FileInfo[] info = dir.GetFiles("*.txt");
			for (int i = 0; i < info.Length; i++) {
				File.Delete (info [i].FullName);
			}
			FileInfo[] infoImages = dir.GetFiles("*.jpg");
			for (int i = 0; i < infoImages.Length; i++) {
				File.Delete (infoImages [i].FullName);
			}
		}

		void netStatusChanged (InternetReachabilityVerifier.Status newStatus)
		{
			CerebroHelper.DebugLog ("Net status changed: " + newStatus);
			if (newStatus == InternetReachabilityVerifier.Status.Error) {
				CerebroHelper.DebugLog (irv.lastError);
			}
			if (firstTimeNetCheck) {
				if (newStatus == InternetReachabilityVerifier.Status.NetVerified) {
					mhasInternet = true;
					ScanTables ();
					setWifiIcon ();
					firstTimeNetCheck = false;
				} else if (newStatus == InternetReachabilityVerifier.Status.Offline || newStatus == InternetReachabilityVerifier.Status.Error) {
					mhasInternet = false;
					setWifiIcon ();
					mCurrentStudent.Coins = PlayerPrefs.GetInt (PlayerPrefKeys.Coins) + PlayerPrefs.GetInt (PlayerPrefKeys.DeltaCoins);
					GotProperties ();
					GotPracticeItems ();
					AllTablesLoaded ();
					firstTimeNetCheck = false;
				}
			} else {
				if (newStatus == InternetReachabilityVerifier.Status.NetVerified) {
					if (mhasInternet == false) {
						mhasInternet = true;
						setWifiIcon ();
						InternetBack ();
					}
					mhasInternet = true;
				} else if (newStatus == InternetReachabilityVerifier.Status.Offline || newStatus == InternetReachabilityVerifier.Status.Error) {
					setWifiIcon ();
					mhasInternet = false;
				}
			}
		}

		// Use this for initialization
		void Start ()
		{
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.Coins)) {
				PlayerPrefs.SetInt (PlayerPrefKeys.Coins, 0);
			}
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.DeltaCoins)) {
				PlayerPrefs.SetInt (PlayerPrefKeys.DeltaCoins, 0);
			}

			LoadPracticeItems ();

			wifiOff.SetActive (false);

			irv = GetComponent<InternetReachabilityVerifier> ();
			irv.statusChangedDelegate += netStatusChanged;

			firstTimeNetCheck = true;

//			mhasInternet = true;
//			QueryAnalyticsTable (null);
//			BackupTable (null, "Moves", Application.persistentDataPath + "/MovesCSV.csv");
//			BackupTable (null, "Analytics", Application.persistentDataPath + "/Analytics.csv");
		}

		public void setWifiIcon ()
		{
			if (wifiOff != null) {
				if (!mhasInternet && WelcomeScreenActive) {
					wifiOff.SetActive (true);
				} else {
					wifiOff.SetActive (false);
				}
			}
		}

		public void checkWifi ()
		{
			CerebroHelper.DebugLog ("CHECKING WIFI");
		}

		// gets called automatically if all 4 tables are loaded
		public void AllTablesLoaded ()
		{
			allTablesLoaded = true;
			if (IAmServer == true) {
				LoadGame ();
				return;
			}
			if (HandleAllTablesLoaded != null) {
				HandleAllTablesLoaded.Invoke (this, null);
			} else {
				GetComponent<CerebroScript> ().AllTablesLoaded (null, null);
			}
			PushLocalAnalyticsToServer ();
		}

		public StudentPlaylist LoadStudentPlayList (GameObject parent)
		{
			if (allTablesLoaded) {
				StudentPlaylistOptionDataList optionDataList = new StudentPlaylistOptionDataList ();
				var student = mCurrentStudent;

				foreach (var key in student.ContentIDs.Keys) {
					var contentID = (student.ContentIDs [key]);
					var contentIndex = LaunchList.instance.FindContentItem (contentID);
					if (contentIndex != -1) {
						var content = LaunchList.instance.mContentItems [contentIndex];
						StudentPlaylistOptionData optionData = new StudentPlaylistOptionData (content.ContentName, content.ContentDescription, content.ContentLink, content.ContentID, null);
						optionDataList.options.Add (optionData);
					}
				}
				StudentPlaylist playlist = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.StudentPlaylist, parent.transform).GetComponent<StudentPlaylist> ();
				playlist.Initialize (optionDataList);
				return playlist;
			}
			return null;
		}

		public void AddMissionVideoToPlaylist (string addcontentID)
		{
			var student = mCurrentStudent;
			bool foundID = false;
			if (student.ContentIDs != null) {
				foreach (var key in student.ContentIDs.Keys) {
					var currentcontentID = (student.ContentIDs [key]);
					if (addcontentID == currentcontentID) {
						foundID = true;
						break;
					}
				}
				if (!foundID) {
					student.ContentIDs.Add (student.ContentIDs.Count, addcontentID);
				}
			}
		}

		public List<string> GetWatchList ()
		{
			List<string> list = new List<string> ();
			if (allTablesLoaded) {
				var student = mCurrentStudent;
				if (student.ContentIDs != null) {
					foreach (var key in student.ContentIDs.Keys) {
						var contentID = (student.ContentIDs [key]);
						list.Add (contentID);
					}
					return list;
				}
			}
			return list;
		}

		public void LoadGame ()
		{
			if (allTablesLoaded) {
				UnityEngine.SceneManagement.SceneManager.LoadScene ("Game");
			}
		}
			
		public void LoadStudentList (GameObject parent)
		{
			if (parent != null) {
				StudentOptionDataList optionDataList = new StudentOptionDataList ();

				for (int i = 0; i < mStudents.Count; i++) {
					var profileImage = "https://img.youtube.com/vi/JC82Il2cjqA/default.jpg";
					StudentOptionData optionData = new StudentOptionData (mStudents [i].StudentName, profileImage, mStudents [i].StudentID, null);
					optionDataList.options.Add (optionData);
				}
				//StudentOptionData optionData = new StudentOptionData("Shawn", "https://img.youtube.com/vi/JC82Il2cjqA/default.jpg","std01",null);

				StudentList dialog = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.StudentList, parent.transform).GetComponent<StudentList> ();
				dialog.Initialize (optionDataList, null, null, null);
				dialog.Show ();
				dialog.LoadImages ();
			}
		}

		// get index of topic given topic id
		public int FindSubject (string subjectID)
		{
			for (int i = 0; i < mSubjects.Count; i++) {
				if (mSubjects [i].SubjectID == subjectID) {
					return i;
				}						
			}
			CerebroHelper.DebugLog ("SubjectID " + subjectID + " not found!!");
			return -1;
		}

		// get index of topic given topic id
		public int FindTopic (string topicID)
		{
			for (int i = 0; i < mTopics.Count; i++) {
				if (mTopics [i].TopicID == topicID) {
					return i;
				}						
			}
			CerebroHelper.DebugLog ("TOPIC " + topicID + " not found!!");
			return -1;
		}

		// returns a list of indices into the mTopics List, for a given subject
		public List<int> GetTopicsForSubject (int subjectIndex)
		{
			List<int> topics = new List<int> ();
			for (int i = 0; i < mSubjects [subjectIndex].TopicIDs.Count; i++) {
				string topicID = mSubjects [subjectIndex].TopicIDs [i];
				int topicIndex = FindTopic (topicID);
				if (topicIndex != -1) {
					topics.Add (topicIndex);
				}
			}
			return topics;
		}


		// get index of subtopic given subtopic id
		public int FindSubtopic (string subtopicID)
		{
			for (int i = 0; i < mSubtopics.Count; i++) {
				if (mSubtopics [i].SubtopicID == subtopicID) {
					return i;
				}						
			}
			CerebroHelper.DebugLog ("SubTopic " + subtopicID + " not found!!");
			return -1;
		}

		// returns a list of indices into the mSubtopics List, for a given Topic
		public List<int> GetSubtopicsForTopic (int topicIndex)
		{
			CerebroHelper.DebugLog (topicIndex);
			List<int> subtopics = new List<int> ();
			for (int i = 0; i < mTopics [topicIndex].SubtopicIDs.Count; i++) {
				string subtopicID = mTopics [topicIndex].SubtopicIDs [i];
				int subtopicIndex = FindSubtopic (subtopicID);
				if (subtopicIndex != -1) {
					subtopics.Add (subtopicIndex);
				}
			}
			return subtopics;
		}


		// get index of subtopic given subtopic id
		public int FindContentItem (string contentId)
		{
			for (int i = 0; i < mContentItems.Count; i++) {
				if (mContentItems [i].ContentID == contentId) {
					return i;
				}						
			}
			CerebroHelper.DebugLog ("Content " + contentId + " not found!!");
			return -1;
		}

		// returns a list of indices into the mSubtopics List, for a given Topic
		public List<int> GetContentForSubtopic (int subtopicIndex)
		{
			List<int> contentItems = new List<int> ();
			for (int i = 0; i < mSubtopics [subtopicIndex].ContentItemIDs.Count; i++) {
				string contentId = mSubtopics [subtopicIndex].ContentItemIDs [i];
				int contentIndex = FindContentItem (contentId);
				if (contentIndex != -1) {
					contentItems.Add (contentIndex);
				}
			}
			return contentItems;
		}

		public void PrintAllData ()
		{
			List<int> topics = GetTopicsForSubject (0); // for math
			CerebroHelper.DebugLog ("SUBJECT ----- Math");
			CerebroHelper.DebugLog (topics);
			for (int i = 0; i < topics.Count; i++) {
				CerebroHelper.DebugLog ("----- TOPIC -----" + mTopics [topics [i]].TopicName);
				List<int> subtopics = GetSubtopicsForTopic (topics [i]);
				for (int j = 0; j < subtopics.Count; j++) {
					CerebroHelper.DebugLog ("---------- SUBTOPIC -----" + mSubtopics [subtopics [j]].SubtopicName);
					List<int> contentItems = GetContentForSubtopic (subtopics [j]);
					for (int k = 0; k < contentItems.Count; k++) {
						CerebroHelper.DebugLog ("--------------- CONTENT -----" + mContentItems [contentItems [k]].ContentName); 
						CerebroHelper.DebugLog ("--------------- CONTENT YOUTUBE -----" + mContentItems [contentItems [k]].ContentLink); 
					}
				}
			}
		}

		public void ExceptionThrown ()
		{
			CerebroHelper.DebugLog ("Exception EXCEPTION");
		}

		public void ScanTables ()
		{
			CerebroHelper.DebugLog ("Scanning Tables");
			mTablesLoaded = false;

			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("ScanTables - no ID set");
				return;
			}

			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			for (int i = 0; i < (int)Cerebro.TableTypes.kMaxTables; i++) { 
				mTableLoaded [i] = false;
			}

			if (PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				GetProperties ();

				GetPracticeItems ();
				GetExplanation ();
				for (int i = 0; i < (int)Cerebro.TableTypes.kMaxTables - 2; i++) { 
					mTableLoaded [i] = true;
				}
				ScanTablesForWatchSection ();
			}

//			SetInitialCells ();

		}

		public void ScanTablesForWatchSection ()
		{
			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			var grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
			var section = PlayerPrefs.GetString (PlayerPrefKeys.SectionKey);
			grade = grade + section;
			CerebroHelper.DebugLog ("Scanning for watch section" + studentID + " " + grade + " " + section);

			for (int i = 0; i < (int)Cerebro.TableTypes.kMaxTables - 2; i++) { 
				mTableLoaded [i] = true;
				if (i == (int)Cerebro.TableTypes.tContent) {
					mTableLoaded [i] = false;
					if (mHitServer) {
						HTTPRequestHelper.instance.GetContentItems ();
					}
				} else if (i == (int)Cerebro.TableTypes.tStudent) {
					mTableLoaded [i] = false;
					GetStudentData (studentID, grade);
				}
			}
		}

		public void GetStudentData (string studentID, string gradeID)
		{
			if (mHitServer) {
				CerebroHelper.DebugLog ("Server Student data");
				HTTPRequestHelper.instance.GetStudentData (studentID, gradeID);
			} 

		}
				
		public void WorldLoaded (bool singleTime)
		{
			CerebroHelper.DebugLog ("------WORLDLOADED");
			mDoingWorldUpdate = false;
			if (singleTime) {
			} else {
				if (WorldChanged != null) {
					WorldChanged.Invoke (this, null);
				}
			}
		}


		public void GetGame (string gameID, string studentID, Action<int> callback)
		{			
			HTTPRequestHelper.instance.GetGOTGameData (gameID,studentID,callback);
		}

		// get index of Cell given Cell id
		public int FindCell (string cellid)
		{
			for (int i = 0; i < mWorld.Count; i++) {
				if (mWorld [i].CellID == cellid) {
					return i;
				}						
			}
			CerebroHelper.DebugLog ("Cell " + cellid + " not found!!");
			return -1;
		}			

		public void SetCoins ( int increment)
		{
			if (!CerebroProperties.instance.ShowCoins) {
				return;
			}
			mCurrentStudent.Coins = increment + mCurrentStudent.Coins;
			if (mCurrentStudent.Coins < 0) {
				mCurrentStudent.Coins = 0;
			}
			int currentDeltaValue = PlayerPrefs.GetInt (PlayerPrefKeys.DeltaCoins);	
			PlayerPrefs.SetInt (PlayerPrefKeys.DeltaCoins, currentDeltaValue + increment);

			mCoinsValueChanged = true;
		}


		private void UpdateCoinsToDatabase ()
		{
			mCoinsValueChanged = false;
			mDoingCoinsUpdate = true;

			if (mHitServer) {
				HTTPRequestHelper.instance.IncrementCoins ();
			} 
		}

		public void GetMission (string missionID)
		{
			if (CheckLocalMissionComplete ()) {
				return;
			}
			if (mMission != null && mMission.MissionID == missionID) {
				GotMissions ();
				return;
			}
			if (GetFileMissionID () == missionID) {
				PopulateMissionFromLocalFile ();
			} else {
				if (mHitServer) {
					HTTPRequestHelper.instance.GetMissionQuestions (missionID);
				} 
			}
		}			
			
		public void GotMissions ()
		{
			WriteMissionToFile ();
			if (m_Instance.MissionLoaded != null) {
				m_Instance.MissionLoaded.Invoke (this, null);
			} else {
				CerebroHelper.DebugLog ("NO HANDLE Mission Loaded");
			}
		}

		private void WriteMissionToFile ()
		{
			string fileName = Application.persistentDataPath + "/Missions.txt";

			string line = "";
			if (File.Exists (fileName)) {
				var sreader = File.OpenText (fileName);
				line = sreader.ReadLine ();
				var lineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
				if (mMission.MissionID != lineArr [0]) {
					line = "";
				}
				sreader.Close ();
			}

			StreamWriter sr = File.CreateText (fileName);
			if (line != "") {
				sr.WriteLine (line);
			} else {
				sr.WriteLine ("{0},{1},{2}", mMission.MissionID, mMission.MissionName, mMission.TimeStarted);
			}
			foreach (var item in mMission.Questions) {
				string conditionKey = "";
				string conditionValue = "";
				foreach (var condition in item.Value.CompletionCondition) {
					if (condition.Value != "-1") {
						conditionKey = condition.Key;
						conditionValue = condition.Value;
						break;
					}
				}
				sr.WriteLine ("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}", item.Value.QuestionID, item.Value.PracticeItemID, item.Value.QuestionTitle, item.Value.QuestionLevel, item.Value.SubLevel, conditionKey, conditionValue, item.Value.ConditionCurrentValue, item.Value.TotalAttempts, item.Value.CorrectAttempts, item.Value.CompleteBool, item.Value.Type, item.Value.QuestionText, item.Value.AnswerOptions, item.Value.Answer, item.Value.QuestionMediaType, item.Value.QuestionMediaURL);
			}

			sr.Close ();	
		}

		public void UpdateLocalMissionFile (MissionItemData item, string questionID, bool isCorrect)
		{
			List<string> toWriteLines = new List<string> ();

			bool openMissions = false;
			string completemissionItemID = "";
			string fileName = Application.persistentDataPath + "/Missions.txt";
			if (!File.Exists (fileName)) {
				return;
			} else {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();	
				toWriteLines.Add (line);

				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						var lineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);

						if (lineArr [0] == questionID) {
							int currentValue = int.Parse (lineArr [7]);
							int completionValue = int.Parse (lineArr [6]);
							int totalAttempts = int.Parse (lineArr [8]);
							int correctAttempts = int.Parse (lineArr [9]);
							string completeBool = lineArr [10];
							totalAttempts++;
							if (lineArr [5] == "Attempts") {
								currentValue++;
								if (isCorrect) {
									correctAttempts++;
								}
							} else if (lineArr [5] == "Streak") {
								if (isCorrect) {
									currentValue++;
									correctAttempts++;
								} else {
									currentValue = 0;
								}
							} else if (lineArr [5] == "Correct" && isCorrect) {
								correctAttempts++;
								currentValue++;	
							}
							if (currentValue >= completionValue && completeBool == "false") {
								completeBool = "true";
								openMissions = true;
								completemissionItemID = lineArr [0];
							}
							item.ConditionCurrentValue = currentValue.ToString ();
							item.CompleteBool = completeBool;
							item.TotalAttempts = totalAttempts.ToString ();
							item.CorrectAttempts = correctAttempts.ToString ();
							string strToWrite = lineArr [0] + "," + lineArr [1] + "," + lineArr [2] + "," + lineArr [3] + "," + lineArr [4] + "," + lineArr [5] + "," + lineArr [6] + "," + currentValue.ToString () + "," + totalAttempts.ToString () + "," + correctAttempts.ToString () + "," + completeBool + "," + lineArr [11] + "," + lineArr [12] + "," + lineArr [13] + "," + lineArr [14] + "," + lineArr [15] + "," + lineArr [16];
							toWriteLines.Add (strToWrite);
						} else {
							toWriteLines.Add (line);
						}
					}
				}
				sr.Close ();
			}

			StreamWriter swr = null;
			swr = File.CreateText (fileName);

			for (int i = 0; i < toWriteLines.Count; i++) {
				swr.WriteLine ("{0}", toWriteLines [i]);
			}                
			swr.Close ();

			if (openMissions) {
				if (WelcomeScript.instance.autoTestMissionCorrect == true || WelcomeScript.instance.autoTestMissionMix) {
					WelcomeScript.instance.RemoveScreens ();
				}
				WelcomeScript.instance.ShowScreen (true, completemissionItemID);
				CheckIfMissionComplete ();
			}
		}

		bool CheckLocalMissionComplete ()
		{
			bool toReturn = false;
			string fileName = Application.persistentDataPath + "/Missions.txt";
			if (!File.Exists (fileName)) {
				return false;
			} else {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();	
				var lineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
				if (lineArr.Length == 6 && lineArr [5] == "false") {
					string day = lineArr [3];
					string timeStarted = lineArr [2];
					string timeEnded = lineArr [4];
					string missionID = lineArr [0];

					int totalAttempts = 0;
					int totalCorrect = 0;
					while (line != null) {
						line = sr.ReadLine ();
						if (line != null) {
							var lineArr2 = line.Split (new string[] { "," }, System.StringSplitOptions.None);
							int correctAnswers = int.Parse (lineArr2 [9]);
							int attempts = int.Parse (lineArr2 [8]);
							totalAttempts += attempts;
							totalCorrect += correctAnswers;
						}
					}

					float accuracy = totalCorrect / totalAttempts;
					toReturn = true;
					PushMissionComplete (missionID, day, timeStarted, timeEnded, accuracy);
				}
				sr.Close ();
			}
			return toReturn;
		}

		void CheckIfMissionComplete ()
		{
			int totalAttempts = 0;
			int totalCorrect = 0;
			string fileName = Application.persistentDataPath + "/Missions.txt";
			bool allComplete = true;
			if (!File.Exists (fileName)) {
				return;
			} else {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();	

				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						var lineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
						string completeBool = lineArr [10];
						int correctAnswers = int.Parse (lineArr [9]);
						int attempts = int.Parse (lineArr [8]);
						totalAttempts += attempts;
						totalCorrect += correctAnswers;
						if (completeBool == "false") {
							allComplete = false;
						}
					}
				}
				sr.Close ();
			}

			if (allComplete) {
				float accuracy = Mathf.Round ((float)totalCorrect * 100 / (float)totalAttempts);
				CerebroHelper.DebugLog ("ALL COMPLETE");
				WelcomeScript.instance.ShowMissionComplete (accuracy);
				MissionCompleteAnalytics (accuracy);
			}
		}

		private void MissionCompleteAnalytics (float accuracy)
		{
			CerebroHelper.DebugLog ("MissionCompleteAnalytics");
			string fileName = Application.persistentDataPath + "/Missions.txt";
			if (!File.Exists (fileName)) {
				return;
			} else {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();	
				var splitArr = line.Split ("," [0]);
				string newLine = "";
				if (splitArr.Length == 3) {
					newLine = line;
				} else {
					newLine = splitArr [0] + "," + splitArr [1] + "," + splitArr [2];
				}
				string day = System.DateTime.Now.ToUniversalTime ().ToString ("yyyyMMdd");
				string timeStarted = splitArr [2];
				string timeEnded = System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss");
				sr.Close ();

				PushMissionComplete (mMission.MissionID, day, timeStarted, timeEnded, accuracy);
			}
		}

		public void UpdateLocalMissionFileForCompletion (float accuracy, bool uploadBool)
		{
			List<string> toWriteLines = new List<string> ();
			string fileName = Application.persistentDataPath + "/Missions.txt";
			if (!File.Exists (fileName)) {
				return;
			} else {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();	
				var splitArr = line.Split ("," [0]);

				string newLine = "";
				string day = System.DateTime.Now.ToString ("yyyyMMdd");
				string timeStarted = splitArr [2];
				string timeEnded = System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss");
				if (splitArr.Length != 3) {
					day = splitArr [3];
					timeEnded = splitArr [4];
				}

				if (uploadBool) {
					newLine = splitArr [0] + "," + splitArr [1] + "," + timeStarted + "," + day + "," + timeEnded + ",true";
				} else {
					newLine = splitArr [0] + "," + splitArr [1] + "," + timeStarted + "," + day + "," + timeEnded + ",false";
				}

				toWriteLines.Add (newLine);

				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						toWriteLines.Add (line);
					}
				}
				sr.Close ();
			}

			StreamWriter swr = null;
			swr = File.CreateText (fileName);

			for (int i = 0; i < toWriteLines.Count; i++) {
				swr.WriteLine ("{0}", toWriteLines [i]);
			}                
			swr.Close ();

			if (uploadBool) {
				WelcomeScript.instance.UpdateMission ();
			}
		}

		public void PushMissionComplete (string missionID, string day, string timeStarted, string timeEnded, float accuracy)
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.PushMissionComplete (missionID, day, timeStarted, timeEnded, accuracy);
			}    

		}
			
		private string GetFileMissionID ()
		{
			string fileName = Application.persistentDataPath + "/Missions.txt";
			if (!File.Exists (fileName)) {
				return "";
			} else {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				if (line != null) {
					var firstLineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
					sr.Close ();
					return firstLineArr [0];
				} else {
					return "";
				}
			}

		}

		private void PopulateMissionFromLocalFile ()
		{
			string fileName = Application.persistentDataPath + "/Missions.txt";
			if (!File.Exists (fileName)) {
				return;
			} else {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				var firstLineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
				mMission.MissionID = firstLineArr [0];
				mMission.MissionName = firstLineArr [1];
				mMission.TimeStarted = firstLineArr [2];
				mMission.Questions = new SortedDictionary<string, MissionItemData> ();
				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						var lineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
						MissionItemData itemData = new MissionItemData ();

						itemData.QuestionID = lineArr [0];
						itemData.PracticeItemID = lineArr [1];
						itemData.QuestionTitle = lineArr [2];
						itemData.QuestionLevel = lineArr [3];
						itemData.SubLevel = lineArr [4];
						itemData.CompletionCondition = new SortedDictionary<string, string> ();
						itemData.CompletionCondition.Add (lineArr [5], lineArr [6]);
						itemData.ConditionCurrentValue = lineArr [7];
						itemData.TotalAttempts = lineArr [8];
						itemData.CorrectAttempts = lineArr [9];
						itemData.CompleteBool = lineArr [10];
						itemData.Type = lineArr [11];
						itemData.QuestionText = lineArr [12];
						itemData.AnswerOptions = lineArr [13];
						itemData.Answer = lineArr [14];
						itemData.QuestionMediaType = lineArr [15];
						itemData.QuestionMediaURL = lineArr [16];
						mMission.Questions.Add (itemData.QuestionID, itemData);
					}
				}
				sr.Close ();
				GotMissions ();
			}
		}
		// XXX what if this function gets hit asynchronously from two threads?
		public void WriteSentAnalytics (string analyticsId)
		{
			string fileName = Application.persistentDataPath + "/SentAnalytics.txt";
			StreamWriter sr = null;
			if (File.Exists (fileName)) {
				CerebroHelper.DebugLog (fileName + " found, appending.");
				sr = File.AppendText (fileName);
			} else {
				sr = File.CreateText (fileName);
			}

			sr.WriteLine ("{0}", analyticsId);
			sr.Close ();
		}

		public void CleanLocalAnalytics ()
		{
			CerebroHelper.DebugLog ("CleanLocalAnalytics");

			// take everything in sent analytics and remove it from local analytics
			// also empty sent analytics at this point

			// Load Local Analytics
			List<string> localAnalytics = new List<string> ();
			string fileName = Application.persistentDataPath + "/LocalAnalytics.txt";
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				localAnalytics.Add (line);
				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						localAnalytics.Add (line);
					}
				}  
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + fileName + " for reading.");
				return;
			}


			// Load Sent Analytics
			List<string> sentAnalytics = new List<string> ();
			string sentFileName = Application.persistentDataPath + "/SentAnalytics.txt";
			if (File.Exists (sentFileName)) {
				var sr = File.OpenText (sentFileName);
				var line = sr.ReadLine ();
				sentAnalytics.Add (line);
				while (line != null) {
					line = sr.ReadLine ();
					sentAnalytics.Add (line);
				}  
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + sentFileName + " for reading.");
				return;
			}

			// Cross check both lists building newLocalAnalytics
			List<string> newLocalAnalytics = new List<string> ();
			for (int i = 0; i < localAnalytics.Count; i++) {
				CerebroHelper.DebugLog (localAnalytics [i]);
				if (localAnalytics [i] == null || localAnalytics [i] == "") {
					continue;
				}
				string assessmentID = localAnalytics [i].Split ("," [0]) [1];
				bool foundInSent = false;
				for (int j = 0; j < sentAnalytics.Count; j++) {
					if (sentAnalytics [j] == assessmentID) {
						// already sent .. remove from local!
						foundInSent = true;
						break;
					}                
				}
				if (!foundInSent) {
					newLocalAnalytics.Add (localAnalytics [i]);
				}
			}


			// WRITE NEW LOCAL ANALYTICS
			StreamWriter swr = null;
			swr = File.CreateText (fileName);

			for (int i = 0; i < newLocalAnalytics.Count; i++) {
				swr.WriteLine ("{0}", newLocalAnalytics [i]);
			}                
			swr.Close ();

			// WRITE EMPTY SENT ANALYTICS
			swr = File.CreateText (sentFileName);
			swr.WriteLine ("");
			swr.Close ();

		}

		public void PushLocalAnalyticsToServer ()
		{
			CerebroHelper.DebugLog ("PushLocalAnalyticsToServer");
			CleanLocalAnalytics ();
			
			// Read the file
			List<string> localAnalytics = new List<string> ();
			string fileName = Application.persistentDataPath + "/LocalAnalytics.txt";
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				localAnalytics.Add (line);
				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						localAnalytics.Add (line);
					}
				}
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + fileName + " for reading.");
				return;
			}

			List<string> sentAnalytics = new List<string> ();
			string sentFileName = Application.persistentDataPath + "/SentAnalytics.txt";
			if (File.Exists (sentFileName)) {
				var sr = File.OpenText (sentFileName);
				var line = sr.ReadLine ();
				sentAnalytics.Add (line);
				while (line != null) {
					line = sr.ReadLine ();
					sentAnalytics.Add (line);
				}  
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + sentFileName + " for reading.");
			}



			for (int i = 0; i < localAnalytics.Count; i++) {
				if (localAnalytics [i] == null || localAnalytics [i] == "") {
					continue;
				}
				string[] items = localAnalytics [i].Split ("," [0]);
				bool foundInSent = false;
				for (int j = 0; j < sentAnalytics.Count; j++) {
					if (sentAnalytics [j] == items [1]) {
						foundInSent = true;
						break;
					}
				}
				if (!foundInSent) {
					string playTime = "0";
					string seed = "0";
					string missionField = "";

					if (items.Length == 8) {
						playTime = items [7];
					}
					if (items.Length == 9) {
						seed = items [8];
					}
					if (items.Length == 10) {
						missionField = items [9];
					}
					CerebroHelper.DebugLog (seed);
					if (items [3] == "true") {
						SendAnalytics (items [1], items [2], true, items [4], items [5], items [6], playTime, seed, missionField);
					} else {
						SendAnalytics (items [1], items [2], false, items [4], items [5], items [6], playTime, seed, missionField);
					}

				}
			}
		}

		// JUST FOR DEBUGGING DONT ACTUALLY CALL UNLESS FOR CerebroHelper.DebugLogING
		public void ReadAnalyticsFromFile ()
		{            
			string fileName = Application.persistentDataPath + "/LocalAnalytics.txt";
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				while (line != null) {
					CerebroHelper.DebugLog (line); // CerebroHelper.DebugLogs each line of the file
					line = sr.ReadLine ();
				}
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + fileName + " for reading.");
				return;
			}
		}

		public void WriteFlaggedQuestionToFile (string assessmentID, int difficulty, int sublevel, int seed)
		{
			string fileName = Application.persistentDataPath + "/FlaggedQuestions.txt";

			StreamWriter sr = null;
			if (File.Exists (fileName)) {
				sr = File.AppendText (fileName);
			} else {
				sr = File.CreateText (fileName);
			}

			sr.WriteLine ("{0},{1},{2},{3}", assessmentID, difficulty, sublevel, seed);
			sr.Close ();
		}

		public void RemoveFlaggedQuestionFromFile (string assessmentID)
		{
			string fileName = Application.persistentDataPath + "/FlaggedQuestions.txt";

			List<string> lines = new List<string> ();
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string assID = line.Split ("," [0]) [0];
				if (assID != assessmentID) {
					lines.Add (line);
				}
				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						assID = line.Split ("," [0]) [0];
						if (assID != assessmentID) {
							lines.Add (line);
						}
					}
				}  
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + fileName + " for reading.");
				return;
			}

			StreamWriter writesr = File.CreateText (fileName);
			CerebroHelper.DebugLog (lines.Count);
			for (var i = 0; i < lines.Count; i++) {
				CerebroHelper.DebugLog (lines [i]);
				writesr.WriteLine (lines [i]);
			}
			writesr.Close ();
		}

		public List<string> GetFlaggedQuestions ()
		{
			string fileName = Application.persistentDataPath + "/FlaggedQuestions.txt";
			List<string> lines = new List<string> ();
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				if (line != null) {
					string assID = line.Split ("," [0]) [0];
					lines.Add (line);
				}
				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						lines.Add (line);
					}
				}  
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + fileName + " for reading.");
			}
			return lines;
		}
		// for video the signature becomes (videoID, difficulty, isComplete, day, timeStarted, timeTaken, playTime, seed)
		public void WriteAnalyticsToFile (string assessmentID, int difficulty, bool correct, string day, string timeStarted, int timeTaken, string playTime, int seed, string missionField, string UserAnswer = "", bool ignoreInternet = false)
		{
			if (mhasInternet && !ignoreInternet) {
				SendAnalytics (assessmentID, difficulty.ToString (), correct, day, timeStarted, timeTaken.ToString (), playTime, seed.ToString (), missionField, UserAnswer);
			} else {
				if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
					CerebroHelper.DebugLog ("WriteAnalyticsToFile - no ID set");
					return;
				}

				var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

				string fileName = Application.persistentDataPath + "/LocalAnalytics.txt";
				StreamWriter sr = null;
				if (File.Exists (fileName)) {
					CerebroHelper.DebugLog (fileName + " found, appending.");
					sr = File.AppendText (fileName);
				} else {
					sr = File.CreateText (fileName);
				}
				if (correct) {
					sr.WriteLine ("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", studentID, assessmentID, difficulty, "true", day, timeStarted, timeTaken, playTime, seed, missionField);
				} else {
					sr.WriteLine ("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", studentID, assessmentID, difficulty, "false", day, timeStarted, timeTaken, playTime, seed, missionField);
				}

				sr.Close ();
			}

		}

	
		// if we are offline this function needs to save this data away
		// and then upload it when we come online
		public void SendAnalytics (string assessmentID, string difficulty, bool correct, string day, string timeStarted, string timeTaken, string playTime, string seed, string missionField, string UserAnswer = "")
		{
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("SendAnalytics - no ID set");
				return;
			}
				
			if (mHitServer) {
				HTTPRequestHelper.instance.SendAnalytics (assessmentID, difficulty, correct, day, timeStarted, timeTaken, playTime, seed, missionField, UserAnswer);
			}

		}
				

		public void StudentDataLoaded ()
		{
			mTableLoaded [(int)Cerebro.TableTypes.tStudent] = true;
		}

		public void SubjectTableLoaded ()
		{
		}

		public void TopicTableLoaded ()
		{
		}

		public void SubtopicTableLoaded ()
		{
		}

		public void ContentTableLoaded ()
		{
			
		}


		// Find index of student, given id
		public int FindStudent (string studentId)
		{
			for (int i = 0; i < mStudents.Count; i++) {
				if (mStudents [i].StudentID == studentId) {
					return i;
				}						
			}
			CerebroHelper.DebugLog ("Student " + studentId + " not found!!");
			return -1;
		}
			

		// Server
		// studentID + moveTime uniquely identify each move
		public int FindMove (string studentID, string moveTime)
		{
			int moveIndex = -1;
			// find move 
			for (int i = 0; i < mMoves.Count; i++) {
				if (mMoves [i].StudentID == studentID && mMoves [i].MoveTime == moveTime) {
					moveIndex = i;
				}
			}
			if (moveIndex == -1) {
				CerebroHelper.DebugLog ("MOVE NOT FOUND " + studentID + " " + moveTime);
			}
			return moveIndex;
		}



		bool CheckConnection ()
		{
			mcheckingInternet = true;
//			while (InternetReachabilityVerifier.Instance.status == InternetReachabilityVerifier.Status.PendingVerification) {
//				CerebroHelper.DebugLog ("LOOPING");
//			}
//		
			CerebroHelper.DebugLog (InternetReachabilityVerifier.Instance.status);
			if (InternetReachabilityVerifier.Instance.status == InternetReachabilityVerifier.Status.NetVerified) {
				return true;
			}
			return false;
		}

		void InternetBack ()
		{
			CheckLocalMissionComplete ();
			ScanTables ();
		}

		// Update is called once per frame
		void Update ()
		{
			if (!mDoingCoinsUpdate && mCoinsValueChanged) {
				UpdateCoinsToDatabase ();
			}
			if (!mTablesLoaded) {
				bool tablesLoaded = true;

				for (int i = 0; i < (int)Cerebro.TableTypes.kMaxTables; i++) {
					if (!mTableLoaded [i]) {
						tablesLoaded = false;
						break;
					}
				}
				if (tablesLoaded) {
					mTablesLoaded = true;
					AllTablesLoaded ();
				}
			}

			if (Time.time >= nextTime) {

				//do something here every interval seconds
				if (mUpdateTimer) {
					mTimer = mTimer.Subtract(new TimeSpan(0,0,1));
				}

				nextTime = Time.time + interval; 

			}


		}

		//void GotQuizForDate()
		int GotQuizForDate (List<QuizData> questions)
		{
			CerebroHelper.DebugLog ("GOT QUIZ");
			for (int i = 0; i < mQuizQuestions.Count; i++) {
				QuizData record = mQuizQuestions [i];
				for (int j = 0; j < record.GetType ().GetProperties ().Count (); j++) {
					CerebroHelper.DebugLog (record.GetType ().GetProperties () [j].Name + " " + record.GetType ().GetProperties () [j].GetValue (record, null));
				}
				//record.GetType().GetProperty (attributeName).SetValue (newRecord, value, null);
			}
			CerebroHelper.DebugLog ("END QUIZ");
			GetLeaderboardForDate (System.DateTime.Now.ToString ("yyyy-MM-dd"));
			return 1;
		}

		public void GetQuizForDate (string date, Action<List<QuizData>> callback)
		{
			mGettingQuiz = true;

			if (mHitServer) {
				HTTPRequestHelper.instance.GetQuizForDate (date, callback);
			}  
			mGettingQuiz = false;
		}


		public void GetUserProfile (string email, Func<StudentProfile,int> callback)
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.GetUserProfile (email, callback);
			} 
		}

		public void GetFeatureForDate (string date, Func<Feature,int> callback, int grade = -1)
		{
			mGettingQuiz = true;

			if (mHitServer) {
				HTTPRequestHelper.instance.GetFeatureForDate (date, callback, grade);
			}  

			mGettingQuiz = false;
		}

		public void PushedItWooshedIt ()
		{
			
		}

		public void PushTheWoosh (string studentID, string deviceID, string pushToken)
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.PushTheWoosh (studentID, deviceID, pushToken);
			}    
		}

		public void SentQuizAnalytics (string studentAndQuestionID, string quizDate)
		{
			string fileName = Application.persistentDataPath + "/QuizAnalytics.txt";

			List<string> lines = new List<string> ();
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;
				if (line != null) {
					splitArr = line.Split ("," [0]);
					if (!(quizDate == splitArr [6] && studentAndQuestionID == splitArr [1])) {
						lines.Add (line);
					}
				}

				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						splitArr = line.Split ("," [0]);
						if (!(quizDate == splitArr [6] && studentAndQuestionID == splitArr [1])) {
							lines.Add (line);
						}
					}
				}  
				sr.Close ();
			}

			StreamWriter writesr = File.CreateText (fileName);
			for (var i = 0; i < lines.Count; i++) {
				writesr.WriteLine (lines [i]);
			}
			writesr.Close ();
		}

		public void SentQuizAnalyticsGrouped (List<QuizAnalytics> AllQuestions)
		{
			foreach (var question in AllQuestions) {
				SentQuizAnalytics (question.StudentAndQuestionID, question.QuizDate);
			}
		}

		public void SendQuizAnalytics (string date, string studentAndQuestionID, string answer, string correct, string timeStarted, string timeTaken)
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.SendQuizAnalytics (date, studentAndQuestionID, answer, correct, timeStarted, timeTaken);
			}    
		}

		public void SendQuizAnalyticsGrouped (List<QuizAnalytics> allQuestions, Func<QuizAnalyticsResponse,int> callback)
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.SendQuizAnalyticsGrouped (allQuestions,callback);
			}    
		}
			
		void BuildLeaderBoard ()
		{
			// First clear current leaderboard
			mLeaderboard.Clear ();
			for (int i = 0; i < mQuizAnalytics.Count; i++) {
				QuizAnalytics record = mQuizAnalytics [i];
				string studentAndQuestionID = record.StudentAndQuestionID;
				string studentID = studentAndQuestionID.Split ("Q" [0]) [0];
				string questionID = "Q" + studentAndQuestionID.Split ("Q" [0]) [1];

				// if time taken < 5   x5
				//			     < 10  x3
				//               < 20  x2
				//               > 20  x1
				int multiplier = 1;
				float timeTaken = Convert.ToSingle (record.TimeTaken);
				if (timeTaken < 5) {
					multiplier = 5;
				} else if (timeTaken < 10) {
					multiplier = 3;
				} else if (timeTaken < 20) {
					multiplier = 2;
				}

				bool qFound = false;
				int difficulty = 0;
				// XXX currently assumes questions table for same date
				// is loaded... we can get rid of this by storing diff. in analytics
				for (int j = 0; j < mQuizQuestions.Count; j++) {
					if (mQuizQuestions [j].QuestionID == questionID) {
						qFound = true;
						difficulty = Int32.Parse (mQuizQuestions [j].Difficulty);
						break;
					}
				}
				if (qFound == false) {
					CerebroHelper.DebugLog ("QUESTION WITH ID " + questionID + " not found");
				}

				int difficultyBonus = difficulty * 3;

				int score = 0;
				if (record.Correct == "true") {
					score = (10 + difficultyBonus) * multiplier;
				}

//				if (mLeaderboard.ContainsKey (studentID)) {
//					mLeaderboard [studentID] += score;
//				} else {
//					mLeaderboard [studentID] = score;
//				}

				if (m_Instance.LeaderboardLoaded != null) {
					m_Instance.LeaderboardLoaded.Invoke (this, null);
				} else {
					CerebroHelper.DebugLog ("NO HANDLE LEADERBOARD");
				}
			}

//			// CerebroHelper.DebugLog leaderboard
//			foreach(var entry in mLeaderboard)
//			{				
//				CerebroHelper.DebugLog (entry.Key + " " + entry.Value);
//			}
		}

		public void GotQuizAnalyticsForDate ()
		{
			for (int i = 0; i < mQuizAnalytics.Count; i++) {
				QuizAnalytics record = mQuizAnalytics [i];
				for (int j = 0; j < record.GetType ().GetProperties ().Count (); j++) {
					CerebroHelper.DebugLog (record.GetType ().GetProperties () [j].Name + " " + record.GetType ().GetProperties () [j].GetValue (record, null));
				}
				//record.GetType().GetProperty (attributeName).SetValue (newRecord, value, null);
			}
			BuildLeaderBoard ();
		}

		// send as a "quiz id" formatted as yyyymmdd
		public void GetLeaderboardForDate (string date)
		{
			mGettingQuizAnalytics = true;

			if (mHitServer) {
				HTTPRequestHelper.instance.getLeaderBoard (date, PlayerPrefs.GetString (PlayerPrefKeys.GradeKey));
			}  
			mGettingQuizAnalytics = false;
		}

		public void LoadPracticeItems ()
		{
			string fileName = Application.persistentDataPath + "/PracticeItemCoins.txt";
			if (File.Exists (fileName)) 
			{
				mKCCoins.Clear ();
				var sr = File.OpenText (fileName);
				string json = sr.ReadToEnd ();
				JSONNode jsonNode = JSONNode.Parse (json);
				if (jsonNode != null) 
				{
					int length = jsonNode.Count;
					for (int i = 0; i < length; i++)
					{
						string KCID = jsonNode [i] ["ID"].Value;
						int coins = jsonNode [i] ["Coins"].AsInt;
						if (mKCCoins.ContainsKey (KCID)) {
							mKCCoins [KCID] = coins;
						} else {
							mKCCoins.Add (KCID, coins);
						}
					}
				}
				sr.Close ();
			}
			fileName = Application.persistentDataPath + "/PracticeItemsWithKC.txt";
			//List<string> resetRegenerationList = new List<string> ();

			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				string json = sr.ReadToEnd ();
				JSONNode jsonNode = JSONNode.Parse (json);
				Debug.Log (json);
				if (jsonNode != null) 
				{
					int length = jsonNode.Count;
					string grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
					for( int i=0 ; i<length; i++)
					{
						PracticeItems pItem = new PracticeItems ();
						pItem.PracticeID = jsonNode [i] ["practice_id"].Value;
						pItem.DifficultyLevels = jsonNode [i] ["difficulty_levels"].Value;
						pItem.PracticeItemName = jsonNode [i] ["practice_item_name"].Value;
						char lastChar = pItem.PracticeItemName[pItem.PracticeItemName.Length - 1];
						int lastNumber = -1;
						if(!int.TryParse(lastChar+"", out lastNumber))
						{
							pItem.PracticeItemName += grade;
						}

						pItem.RegenRate = jsonNode [i] ["regen_rate"].Value;
						pItem.Show =  jsonNode [i] ["show"].Value;
						pItem.RegenerationStarted = "";
						pItem.TotalCoins = 0 ;
						pItem.CurrentCoins = 0;
						pItem.KnowledgeComponents = new Dictionary<string, KnowledgeComponent> ();

						JSONNode KCdata = jsonNode [i] ["topics"];
						int KClength = KCdata.Count;
						for (int j = 0; j < KClength; j++)
						{
							KnowledgeComponent KC = new KnowledgeComponent ();
							KC.ID = KCdata[j] ["id"].Value;
							KC.KCName = KCdata[j] ["name"].Value;
							KC.TotalCoins = KCdata[j] ["coins"].AsInt;
							if (mKCCoins.ContainsKey (KC.ID)) 
							{
								KC.CurrentCoins = mKCCoins [KC.ID];
							}
							pItem.TotalCoins += KC.TotalCoins;
							pItem.CurrentCoins += KC.CurrentCoins;
							KC.Mappings = new List<string>();
						
							JSONNode MappingData = KCdata[j]["question_types"];
							int Mappinglength = MappingData.Count;
							for(int k=0; k<Mappinglength; k++)
							{
								string mapping = MappingData [k] .Value;
								if (!KC.Mappings.Contains (mapping)) 
								{
									KC.Mappings.Add (mapping);
								}
							}
							if (!pItem.KnowledgeComponents.ContainsKey (KC.ID)) 
							{
								pItem.KnowledgeComponents.Add (KC.ID, KC);
							}
						}
						if (mPracticeItems.ContainsKey (pItem.PracticeID)) {
							mPracticeItems [pItem.PracticeID] = pItem;
						} else {
							mPracticeItems.Add (pItem.PracticeID, pItem);
						}


					}
				}
				/*var line = sr.ReadLine ();
				string[] splitArr;

				while (line != null) {
					if (line != null) {
						splitArr = line.Split ("," [0]);
						PracticeItems pItem = new PracticeItems ();
						pItem.Grade = splitArr [0];
						pItem.PracticeID = splitArr [1];
						pItem.DifficultyLevels = splitArr [2];
						pItem.PracticeItemName = splitArr [3];
						pItem.RegenRate = splitArr [4];
						pItem.TotalCoins = splitArr [5];
						pItem.CurrentCoins = int.Parse (splitArr [6]);
						pItem.RegenerationStarted = splitArr [7];
						if (splitArr.Length == 9) {
							pItem.Show = splitArr [8];
						} else {
							pItem.Show = "true";
						}
						if (pItem.RegenerationStarted != "" && pItem.RegenerationStarted != " ") {
							if (checkRegeneration (pItem)) {
								pItem.RegenerationStarted = "";
								pItem.CurrentCoins = 0;
								resetRegenerationList.Add (pItem.PracticeID);
							}
						}
						if (mPracticeItems.ContainsKey (pItem.PracticeID)) {
							mPracticeItems [pItem.PracticeID] = pItem;
						} else {
							mPracticeItems.Add (pItem.PracticeID, pItem);
						}
					}

					line = sr.ReadLine ();
				} */ 

				sr.Close ();
			}
			PracticeRegeneration ();
			//ResetRegenerationinPracticeItems (resetRegenerationList);
		}

		public void LoadExplanations ()
		{
			string fileName = Application.persistentDataPath + "/Explanations.txt";

			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;

				while (line != null) {
					if (line != null) {
						splitArr = line.Split ("," [0]);
						Explanation explanation = new Explanation ();
						explanation.PracticeItemID = splitArr [0];
						explanation.Level = splitArr [1];
						explanation.SubLevel = splitArr [2];
						explanation.URL = splitArr [3];
						string key = explanation.PracticeItemID + "L" + explanation.Level + explanation.SubLevel;
						mExplanation.Add (key, explanation);
					}

					line = sr.ReadLine ();
				}  
				sr.Close ();
			}
		}


		public void PracticeRegeneration()
		{
			string fileName = Application.persistentDataPath + "/PracticeItemRegeneration.txt";

			JSONNode regenerationData;
			if (File.Exists (fileName))
			{
				string json = File.ReadAllText (fileName);
				regenerationData = JSONNode.Parse (json);
				int length = regenerationData.Count;
				for (int i = 0; i < length; i++)
				{
					if(!LaunchList.instance.mPracticeItems.ContainsKey(regenerationData[i]["PracticeId"]))
						continue;
					PracticeItems pItem = LaunchList.instance.mPracticeItems[regenerationData[i]["PracticeId"]];
					System.DateTime timestarted = System.DateTime.ParseExact (regenerationData[i]["RegenerationDate"], "yyyyMMddHHmmss", null);

					System.DateTime timeNow = System.DateTime.Now;
					System.TimeSpan differenceTime = timeNow.Subtract (timestarted);
					float diff = (float)differenceTime.TotalSeconds;
					float secondsForRegeneration = float.Parse (pItem.RegenRate) * 60 * 60 * 24;
					if (diff < secondsForRegeneration) {
						continue;
					}
					bool isFinishedKCS = IsFinishedCertainKCS (pItem);
					if (isFinishedKCS) 
					{
						ResetCoinsAfterRegenration (pItem);
						regenerationData[i] ["RegenerationDate"] = DateTime.Now.ToString("yyyyMMddHHmmss");
					}
				}
			}
			else
			{
				regenerationData = JSONNode.Parse ("[]");
				foreach (var pItem in mPracticeItems)
				{
					JSONNode practiceRegenerationData = JSONNode.Parse ("{}");
					practiceRegenerationData ["PracticeId"] = pItem.Value.PracticeID;
					practiceRegenerationData ["RegenerationDate"] = DateTime.Now.ToString("yyyyMMddHHmmss");
					regenerationData.Add (practiceRegenerationData);

					ResetCoinsAfterRegenration (pItem.Value);
				}
			}
				
			File.WriteAllText (fileName,regenerationData.ToString());
			UpdateKCCoinsData ();
		}

		public void ResetCoinsAfterRegenration(PracticeItems pItem)
		{
			pItem.CurrentCoins = 0;
			foreach (var KC in pItem.KnowledgeComponents)
			{
				KC.Value.CurrentCoins = 0;
				if(LaunchList.instance.mKCCoins.ContainsKey(KC.Value.ID))
				{
					LaunchList.instance.mKCCoins [KC.Value.ID] = 0;
				}
			}
		}


		public bool IsFinishedCertainKCS(PracticeItems pItem)
		{
			foreach (var KC in pItem.KnowledgeComponents)
			{
				if (KC.Value.CurrentCoins < (KC.Value.TotalCoins * 0.5f))
					return false;
			}

			return true;
		}


		public  void UpdateKCCoinsData()
		{
			string fileName = Application.persistentDataPath + "/PracticeItemCoins.txt";
			JSONNode jsonNode = JSONNode.Parse ("[]");
			foreach(var KCCoin in LaunchList.instance.mKCCoins)
			{
				JSONNode data = JSONNode.Parse ("{}");
				data ["ID"] = KCCoin.Key.ToString();
				data["Coins"]= KCCoin.Value.ToString();
				jsonNode.Add (data);
			}

			File.WriteAllText (fileName, jsonNode.ToString ());
		}

		/*public bool checkRegeneration (PracticeItems pItem)
		{
			System.DateTime timestarted = System.DateTime.ParseExact (pItem.RegenerationStarted, "yyyyMMddHHmmss", null);
			System.DateTime timeNow = System.DateTime.Now;
			System.TimeSpan differenceTime = timeNow.Subtract (timestarted);
			float diff = (float)differenceTime.TotalSeconds;
			float secondsForRegeneration = float.Parse (pItem.RegenRate) * 60 * 60 * 24;
			if (diff >= secondsForRegeneration) {
				return true;
			}
			return false;
		}

		public void ResetRegenerationinPracticeItems (List<string> practiceIDs)
		{
			string fileName = Application.persistentDataPath + "/PracticeItems.txt";

			List<string> lines = new List<string> ();
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;
				while (line != null) {

					if (line != null) {
						splitArr = line.Split ("," [0]);
						if (practiceIDs.Contains (splitArr [1])) {
							string newLine = splitArr [0] + "," + splitArr [1] + "," + splitArr [2] + "," + splitArr [3] + "," + splitArr [4] + "," + splitArr [5] + ",0,," + splitArr [8];
							lines.Add (newLine);
						} else {
							lines.Add (line);
						}
					}
					line = sr.ReadLine ();
				}  
				sr.Close ();
			} else {
				return;
			}

			StreamWriter writesr = File.CreateText (fileName);
			for (var i = 0; i < lines.Count; i++) {
				writesr.WriteLine (lines [i]);
			}
			writesr.Close ();
		}*/


		public void GotPracticeItems ()
		{		
			mTableLoaded [(int)Cerebro.TableTypes.tPracticeItems] = true;	
			LoadPracticeItems ();
		}

		public void GetPracticeItems ()
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.GetPracticeItems ();
			}  
		}

		public List<PracticeItems> GetGradePracticeItems ()
		{
			string grade = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.GradeKey)) {
				grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
			}
			List<PracticeItems> validItems = new List<PracticeItems> ();
			foreach (var p in mPracticeItems) {
			   //if (p.Value.Grade == grade && (p.Value.Show == "true" || CerebroHelper.isTestUser()))
				if (p.Value.Show == "true" || CerebroHelper.isTestUser())
				{
					validItems.Add (p.Value);
				}
			}
			return validItems;
		}

		public void GotProperties ()
		{
			CerebroHelper.DebugLog ("G0T PROPERTIES");
			mTableLoaded [(int)Cerebro.TableTypes.tProperties] = true;
			GetComponent<CerebroScript> ().SetCerebroProperties ();
		}

		public void GetProperties ()
		{
			CerebroHelper.DebugLog ("GET PROPERTIES");
			if (mHitServer) {
				HTTPRequestHelper.instance.GetProperties ();
			}  
		}

		public void GotEnglishImage ()
		{
			CheckForSubmittedImageResponses (mDescribeImage.ImageID);
			if (m_Instance.DescribeImageLoaded != null) {
				m_Instance.DescribeImageLoaded.Invoke (this, null);
			}
		}

		public void GotEnglishImageResponses ()
		{
			if (m_Instance.DescribeImageResponsesLoaded != null) {
				m_Instance.DescribeImageResponsesLoaded.Invoke (this, null);
			}
		}

		public void GotVerbalizeText ()
		{
//			CheckForSubmittedVerbalize (mVerbalize.VerbalizeID);
			if(mVerbalize != null && mVerbalize.VerbalizeID != null)
			WriteVerbalizeResponseToFile(mVerbalize);
			if (m_Instance.VerbalizeTextLoaded != null) {
				m_Instance.VerbalizeTextLoaded.Invoke (this, null);
			}
		}

		public void GetEnglishImageResponses (string imageID)
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.GetEnglishImageResponses (imageID);
			}   

		}

		public void GetEnglishImage ()
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.GetEnglishImage ();
			}   

		}
		public void SubmitDescribeImageResponse (string studentID, string ImageID, string userResponse)
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.SubmitDescribeImageResponse (studentID, ImageID, userResponse);
			}   

		}


		public void WriteImageResponseToFile (string userResponse)
		{
			string fileName = Application.persistentDataPath + "/DescribeImageSubmitted.txt";
			StreamWriter sr;
			if (File.Exists (fileName)) {
				sr = File.AppendText (fileName);
			} else {
				sr = File.CreateText (fileName);
			}
			sr.WriteLine ("{0},{1},{2},{3},{4},{5}", mDescribeImage.ImageID, mDescribeImage.MediaType, mDescribeImage.MediaURL, mDescribeImage.PromptText.Trim(), mDescribeImage.SubPromptText.Trim(), userResponse);
			sr.Close ();	
		}

		public void WriteVerbalizeResponseToFile (Verbalize verb)
		{
			string fileName = Application.persistentDataPath + "/VerbalizeSubmitted.txt";
			int cnt = 0;
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {				
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				cnt = N ["Data"].Count;
				int currId = CheckForSubmittedVerbalize (verb.VerbalizeID);
				if (currId > -1)
					cnt = currId;
				File.WriteAllText (fileName, string.Empty);
			}
			N ["Data"] [cnt] ["VerbalizeID"] = verb.VerbalizeID;
			N ["Data"] [cnt] ["VerbalizeDate"] = verb.VerbalizeDate;
			N ["Data"] [cnt] ["UserSubmitted"] = verb.UserSubmitted.ToString();
			N ["Data"] [cnt] ["UserResponseURL"] = verb.UserResponseURL;
			N ["Data"] [cnt] ["UploadedToServer"] = verb.UploadedToServer.ToString();
			N ["Data"] [cnt] ["VerbGrade"] = verb.VerbGrade;
			N ["Data"] [cnt] ["VerbDifficulty"] = verb.VerbDifficulty;
			N ["Data"] [cnt] ["VerbGenre"] = verb.VerbGenre;
			N ["Data"] [cnt] ["VerbTitle"] = verb.VerbTitle;
			N ["Data"] [cnt] ["VerbAuthor"] = verb.VerbAuthor;
			N ["Data"] [cnt] ["PromptText"] = verb.PromptText;
			N ["Data"] [cnt] ["VerbSpeed"] = verb.VerbSpeed;
			N ["Data"] [cnt] ["VerbStartTime"] = verb.VerbStartTime;
			N ["Data"] [cnt] ["VerbEndTime"] = verb.VerbEndTime;
			File.WriteAllText (fileName, N.ToString());	
		}

		public Dictionary<string,string> GetNextImageID ()
		{
			string fileName = Application.persistentDataPath + "/LastImageID.txt";
			string fetchNewDate = "false";
			Dictionary<string,string> returnDict = new Dictionary<string,string> ();

			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				if (line != null) {
					var lineArr = line.Split ("," [0]);
					string imgId = lineArr [0];
					string date = lineArr [1];

					System.DateTime imageSubmittedDate = System.DateTime.ParseExact (date, "yyyyMMdd", null);
					string today = System.DateTime.Now.ToString ("yyyyMMdd");
					System.DateTime todayDate = System.DateTime.ParseExact (today, "yyyyMMdd", null);

					if (todayDate > imageSubmittedDate) {
						fetchNewDate = "true";
					}
					sr.Close ();

					returnDict.Add ("imageID", imgId);
					returnDict.Add ("fetchBool", fetchNewDate);

					return returnDict;
				}
				sr.Close ();
			} else {
				returnDict.Add ("imageID", "1");
				returnDict.Add ("fetchBool", "true");
				return returnDict;
			}
			returnDict.Add ("imageID", "1");
			returnDict.Add ("fetchBool", "true");
			return returnDict;
		}

		public void SetLastImageID (string imageID, string date)
		{
			string fileName = Application.persistentDataPath + "/LastImageID.txt";
			StreamWriter sr = File.CreateText (fileName);
			sr.WriteLine ("{0},{1}", imageID, date);
			sr.Close ();
		}

		public bool CheckForSubmittedImageResponses (string ImageID)
		{
			string fileName = Application.persistentDataPath + "/DescribeImageSubmitted.txt";
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				while (line != null) {
					var lineArr = line.Split ("," [0]);
					string imgId = lineArr [0];
					if (imgId == ImageID) {
						if (mDescribeImage == null) {
							mDescribeImage = new DescribeImage ();
						}
						mDescribeImage.UserSubmitted = true;
						mDescribeImage.ImageID = imgId;
						mDescribeImage.MediaType = lineArr [1];
						mDescribeImage.MediaURL = lineArr [2];
						mDescribeImage.PromptText = lineArr [3];
						mDescribeImage.SubPromptText = lineArr [4];
						mDescribeImage.UserResponse = lineArr [5];
						if (lineArr.Length > 6) {
							for (var i = 6; i < lineArr.Length; i++) {
								mDescribeImage.UserResponse += "," + lineArr [i];
							}
						}
						sr.Close ();
						return true;
					}
					line = sr.ReadLine ();
				}
				sr.Close ();
			} else {
				return false;
			}
			return false;
		}

		public Dictionary<string,string> GetNextVerbalizeID ()
		{
			string fileName = Application.persistentDataPath + "/LastVerbalizeID.txt";
			string fetchNewDate = "false";
			Dictionary<string,string> returnDict = new Dictionary<string,string> ();

			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				if (line != null) {
					var lineArr = line.Split ("," [0]);
					string verbId = lineArr [0];
					string date = lineArr [1];

					System.DateTime imageSubmittedDate = System.DateTime.ParseExact (date, "yyyyMMdd", null);
					string today = System.DateTime.Now.ToString ("yyyyMMdd");
					System.DateTime todayDate = System.DateTime.ParseExact (today, "yyyyMMdd", null);

					if (todayDate > imageSubmittedDate) {
						fetchNewDate = "true";
					}
					sr.Close ();

					returnDict.Add ("VerbalizeID",verbId);
					returnDict.Add ("fetchBoolVerb", fetchNewDate);

					return returnDict;
				}
				sr.Close ();
			}  else {
				returnDict.Add ("VerbalizeID", "1");
				returnDict.Add ("fetchBoolVerb", "true");
				return returnDict;
			}
			returnDict.Add ("VerbalizeID", "1");
			returnDict.Add ("fetchBoolVerb", "true");
			return returnDict;
		}

		public void SetLastVerbalizeID (string verbalizeID, string date)
		{
			string fileName = Application.persistentDataPath + "/LastVerbalizeID.txt";
			StreamWriter sr = File.CreateText (fileName);
			sr.WriteLine ("{0},{1}", verbalizeID, date);
			sr.Close ();
		}

		public void DeleteVerbalize(string VerbID)
		{
			Debug.Log ("deleting");
			string fileName = Application.persistentDataPath + "/VerbalizeSubmitted.txt";
			int cnt = 0;
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			JSONNode N1 = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {				
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				cnt = N ["Data"].Count;
				int currId = CheckForSubmittedVerbalize (VerbID);
				Debug.Log ("found "+currId+" "+cnt);
				if (currId > -1) {
					File.WriteAllText (fileName, string.Empty);
					int myCnt = 0;
					for (int i = 0; i < cnt; i++) {
						Debug.Log ("curr "+currId+" "+i);
						if (i == currId) {
							continue;
						}
						Debug.Log ("cpoying "+i+" "+myCnt);
						N1 ["Data"] [myCnt] = N ["Data"] [i];
//						N1 ["Data"][myCnt]["VerbalizeID"] = N ["Data"][i]["VerbalizeID"].Value;
//						N1 ["Data"][myCnt]["VerbalizeDate"] = N ["Data"][i]["VerbalizeDate"].Value;
//						N1 ["Data"][myCnt]["UserSubmitted"] = N ["Data"][i]["UserSubmitted"].Value;
//						N1 ["Data"][myCnt]["UserResponseURL"] = N ["Data"][i]["UserResponseURL"].Value;
//						N1 ["Data"][myCnt]["UploadedToServer"] = N ["Data"][i]["UploadedToServer"].Value;
//						N1 ["Data"][myCnt]["VerbGrade"] = N ["Data"][i]["VerbGrade"].Value;
//						N1 ["Data"][myCnt]["VerbDifficulty"] = N ["Data"][i]["VerbDifficulty"].Value;
//						N1 ["Data"][myCnt]["VerbGenre"] = N ["Data"][i]["VerbGenre"].Value;
//						N1 ["Data"][myCnt]["VerbTitle"] = N ["Data"][i]["VerbTitle"].Value;
//						N1 ["Data"][myCnt]["VerbAuthor"] = N ["Data"][i]["VerbAuthor"].Value;
//						N1 ["Data"][myCnt]["PromptText"] = N ["Data"][i]["PromptText"].Value;
//						N1 ["Data"][myCnt]["VerbSpeed"] = N ["Data"][i]["VerbSpeed"].Value;
//						N1 ["Data"][myCnt]["VerbStartTime"] = N ["Data"][i]["VerbStartTime"].Value;
//						N1 ["Data"][myCnt]["VerbEndTime"] = N ["Data"][i]["VerbEndTime"].Value;
						myCnt++;
					}
					File.WriteAllText (fileName, N1.ToString());	
				}
			}
		}

		public int CheckForSubmittedVerbalize (string VerbalizeID)
		{
			string fileName = Application.persistentDataPath + "/VerbalizeSubmitted.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					if (N ["Data"][i]["VerbalizeID"].Value == VerbalizeID) {
//						if (mVerbalize == null) {
//							mVerbalize = new Verbalize ();
//						}
//						mVerbalize.VerbalizeID = N ["Data"][i]["VerbalizeID"].Value;
//						mVerbalize.VerbalizeDate = N ["Data"][i]["VerbalizeDate"].Value;
//						mVerbalize.UserSubmitted = N ["Data"][i]["UserSubmitted"].AsBool;
//						mVerbalize.UserResponseURL = N ["Data"][i]["UserResponseURL"].Value;
//						mVerbalize.UploadedToServer = N ["Data"][i]["UploadedToServer"].AsBool;
						return i;
					}
				}
			}
			return -1;
		}

		public Verbalize CheckForSubmittedVerbalizeViaDate (string VerbalizeDate)
		{
			string fileName = Application.persistentDataPath + "/VerbalizeSubmitted.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					if (N ["Data"][i]["VerbalizeDate"].Value == VerbalizeDate) {
						Verbalize Verb = new Verbalize ();
						Verb.VerbalizeID = N ["Data"][i]["VerbalizeID"].Value;
						Verb.VerbalizeDate = N ["Data"][i]["VerbalizeDate"].Value;
						Verb.UserSubmitted = N ["Data"][i]["UserSubmitted"].AsBool;
						Verb.UserResponseURL = N ["Data"][i]["UserResponseURL"].Value;
						Verb.UploadedToServer = N ["Data"][i]["UploadedToServer"].AsBool;
						Verb.VerbGrade = N ["Data"][i]["VerbGrade"].Value;
						Verb.VerbDifficulty = N ["Data"][i]["VerbDifficulty"].Value;
						Verb.VerbGenre = N ["Data"][i]["VerbGenre"].Value;
						Verb.VerbTitle = N ["Data"][i]["VerbTitle"].Value;
						Verb.VerbAuthor = N ["Data"][i]["VerbAuthor"].Value;
						Verb.PromptText = N ["Data"][i]["PromptText"].Value;
						Verb.VerbSpeed = N ["Data"][i]["VerbSpeed"].Value;
						Verb.VerbStartTime = N ["Data"][i]["VerbStartTime"].Value;
						Verb.VerbEndTime = N ["Data"][i]["VerbEndTime"].Value;
						return Verb;
					}
				}
			}
			return null;
		}

		public void CheckForVerbalizeToUpload ()
		{
			Debug.Log ("checking for verbalize upload in launchlist");
			string fileName = Application.persistentDataPath + "/VerbalizeSubmitted.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					if (N ["Data"][i]["UserSubmitted"].AsBool && !N ["Data"][i]["UploadedToServer"].AsBool) {
						Verbalize Verb = CheckForSubmittedVerbalizeViaDate (N ["Data"][i]["VerbalizeDate"].Value);
						Debug.Log ("found one for verbalize upload "+Verb.VerbTitle);
						#if UNITY_EDITOR
						HTTPRequestHelper.instance.SubmitVerbalizeResponse(Verb);
						#endif
						#if UNITY_IOS && !UNITY_EDITOR
						HTTPRequestHelper.instance.uploadProfileVid ("vid.mov", LaunchList.instance.mCurrLocalTempPath + Verb.UserResponseURL, Verb);
						#endif
						break;
					}
				}
			}
		}

		public void GotTempDirectory(string tempPath)
		{
			print ("got temp path "+tempPath);
			mCurrLocalTempPath = tempPath;
		}

		[DllImport ("__Internal")]
		private static extern void _GetTempDirectory (
			string message);

		public bool SetVerbalizeUploaded (Verbalize Verb)
		{
			string fileName = Application.persistentDataPath + "/VerbalizeSubmitted.txt";
			int cnt = -1;
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				cnt = CheckForSubmittedVerbalize (Verb.VerbalizeID);
				if (cnt > -1) {
					string OldPath = LaunchList.instance.mCurrLocalTempPath + N ["Data"] [cnt] ["UserResponseURL"].Value;
					#if UNITY_IOS && !UNITY_EDITOR
					_DeleteLocalVideo (OldPath);
					#endif
					bool uploaded = true;
					N ["Data"] [cnt] ["UserResponseURL"] = Verb.UserResponseURL;
					N ["Data"] [cnt] ["UploadedToServer"] = uploaded.ToString();
					File.WriteAllText (fileName, string.Empty);
					File.WriteAllText (fileName, N.ToString());
					return true;
				}
			}
			return false;
		}

		[DllImport ("__Internal")]
		private static extern void _DeleteLocalVideo (
			string path);

		public void LeaderBoardLoadComplete (bool isSuccess)
		{
			if (isSuccess && m_Instance.LeaderboardLoaded != null) {
				m_Instance.LeaderboardLoaded.Invoke (this, null);
			} else {
				CerebroHelper.DebugLog ("NO HANDLE LEADERBOARD");
			}
		}

		public void GetExplanation ()
		{
			if (mHitServer)
				HTTPRequestHelper.instance.GetExplanation ();
		}

		public void ExplanationLoaded ()
		{
			//LoadExplanations ();
		}
	}
	// class ends

	public class LeaderBoard
	{
		public string StudentID { get; set; }
		public string StudentName { get; set; }
		public int StudentScore { get; set; }
	}

	public class Explanation
	{
		public string PracticeItemID { get; set; }
		public string Level { get; set; }
		public string SubLevel { get; set; }
		public string URL { get; set; }
		public string ContentId { get; set; }
	}


	public class Subject
	{
		   // Hash key.
		public string SubjectID { get; set; }
		public string SubjectGrade { get; set; }
		public string SubjectName { get; set; }
		public List<string> TopicIDs { get; set; }
		public string Userdata { get; set; }
	}

	public class Topic
	{
		   // Hash key.
		public string SubjectID { get; set; }
		public string TopicID { get; set; }
		public string TopicName { get; set; }
		public string TopicGrade { get; set; }
		public List<string> SubtopicIDs { get; set; }
		public string Userdata { get; set; }
	}

	public class Subtopic
	{
		   // Hash key.
		public string TopicID { get; set; }
		public string SubtopicID { get; set; }
		public string SubtopicName { get; set; }
		public List<string> ContentItemIDs { get; set; }
		public string Userdata { get; set; }
	}

	public class ContentItem
	{
		   // Hash key.
		public string SubtopicID { get; set; }
		public string ContentID { get; set; }
		public string ContentName { get; set; }
		public string ContentDate { get; set; }
		public string ContentDescription { get; set; }
		public string ContentLink { get; set; }
		public string ContentType { get; set; }
		public int ContentDifficulty { get; set; }
		public int ContentRating { get; set; }
		public int ContentViews { get; set; }
		public List<string> ContentTags { get; set; }
		public string Userdata { get; set; }
	}

	public class Student
	{
		public string GradeID { get; set; }
		public string StudentID { get; set; }
		public string StudentName { get; set; }
		public SortedDictionary<int, string> ContentIDs { get; set; }
		public int Coins {get; set; }
	}


	public class Analytics
	{
		public string StudentID{ get; set; }
		public string AssessmentItemID { get; set; }
		public string Date { get; set; }
		public string TimeStarted { get; set; }
		public string TimeTaken { get; set; }
		public bool Correct { get; set; }
		public string Difficulty { get; set; }
		public string PlayTime { get; set; }
		public string RandomSeed { get; set; }
	}

	public class World
	{
		public string CellID{ get; set; }
		public string GroupID{ get; set; }
		public string StudentID{ get; set; }
		public string Cost{ get; set; }
		public int BabaHairId{ get; set; }
		public int BabaFaceId{ get; set; }
		public int BabaBodyId{ get; set; }
	}

	public class CellGame
	{
		public string StudentID { get; set; }
		public string GroupID{ get; set; }
		public string GroupIDDB { get; set; }
		public string GameID {get; set;}
		public string GameName { get; set; }
		public string StartTime{ get; set; }
	}

	public class StartableGame
	{
		public string GameID {get; set;}
		public string GameName { get; set; }
		public string StartTime{ get; set; }
	}


	public class Moves
	{
		public string StudentID { get; set; }
		public string MoveTime{ get; set; }
		public string GroupID{ get; set; }
		public string CellID{ get; set; }
		public string MoveBid{ get; set; }
		public string MoveDone { get; set; }
		public string MoveValid{ get; set; }
	}

	public class Push
	{
		public string StudentID { get; set; }
		public string DeviceID{ get; set; }
		public string PushToken{ get; set; }
	}

	public class Flags
	{
		public string StudentID { get; set; }
		public string AssessmentID{ get; set; }
		public string Difficulty{ get; set; }
		public string Flagged{ get; set; }
		public string Seed{ get; set; }
	}

	// This is a bit hacky-hackerson
	// but the range(sort) key here will look like this:
	// QuizID - YYYYMMDD of when the quiz was pushed
	// Timestamp of this answer
	// QuestionID
	// concat the above three
	// this way when searching to build leaderboard
	// I can do a BEGIN_WITH searching for "quizid"

	public class QuizAnalytics
	{
		public string QuizDate { get; set; }
		public string StudentAndQuestionID{ get; set; }
		public string Answer{ get; set; }
		public string Correct{ get; set; }
		public string TimeStarted{ get; set; }
		public string TimeTaken{ get; set; }
	}

	public class PracticeItems
	{
		public string PracticeID { get; set; }
		public string DifficultyLevels{ get; set; }
		//public string Grade { get; set; }
		public string PracticeItemName{ get; set; }
		public string RegenRate{ get; set; }
		public int TotalCoins{ get; set; }
		public string Show { get; set; }
		public int CurrentCoins{ get; set; }
		public string RegenerationStarted { get; set; }
		public Dictionary<string ,KnowledgeComponent> KnowledgeComponents;
	}

	public class KnowledgeComponent
	{
		public string ID { get; set; }
		public string KCName { get; set; }
		public List<string> Mappings { get; set; }
		public int TotalCoins { get; set; }
		public int CurrentCoins { get; set; }
	}
		
	public class QuizData
	{
		public string QuizDate { get; set; }
		public string QuestionID{ get; set; }
		public string AnswerOptions{ get; set; }
		public string CorrectAnswer{ get; set; }
		public string Difficulty{ get; set; }
		public string QuestionIndex{ get; set; }
		public string QuestionMedia{ get; set; }
		public string QuestionMediaType{ get; set; }
		public string QuestionText{ get; set; }
		public string QuestionSubText{ get; set; }
		public string QuestionType{ get; set; }
		public string AnswerType{ get; set; }
		public string AnswerURL{ get; set; }
		public bool isCorrect { get; set; }
		public string userAnswer { get; set; }
		public string TimeStarted { get; set; }
		public string currentSessionTimeStarted { get; set; }
		public float TimeTaken { get; set; }
	}

	public class Properties
	{
		public string PropertyName { get; set; }
		public string PropertyValue{ get; set; }
	}

	public class Missions
	{
		public string MissionID { get; set; }
		public string MissionName{ get; set; }
		public SortedDictionary<string, string> QuestionsString { get; set; }
		public string TimeStarted { get; set; }
		public SortedDictionary<string, MissionItemData> Questions { get; set; }
	}

	public class MissionItemData
	{
		public string QuestionID { get; set; }
		public string QuestionLevel { get; set; }
		public string SubLevel { get; set; }
		public string PracticeItemID { get; set; }
		public string Type { get; set; }
		public string QuestionText { get; set; }
		public string AnswerOptions { get; set; }
		public string Answer { get; set; }
		public string QuestionMediaType { get; set; }
		public string QuestionMediaURL { get; set; }
		public string QuestionTitle { get; set; }
		public SortedDictionary<string, string> CompletionCondition { get; set; }
		public string ConditionCurrentValue { get; set; }
		public string CompleteBool { get; set; }
		public string TotalAttempts { get; set; }
		public string CorrectAttempts { get; set; }
	}


	public class DescribeImage
	{
		public string ImageID { get; set; }
		public string MediaType { get; set; }
		public string MediaURL { get; set; }
		public string PromptText { get; set; }
		public string SubPromptText { get; set; }
		public bool UserSubmitted { get; set; }
		public string UserResponse { get; set; }
	}

	public class Verbalize
	{
		public string VerbalizeID { get; set; }

		public string VerbalizeDate { get; set; }

		public string VerbGrade { get; set; }

		public string VerbDifficulty { get; set; }

		public string VerbGenre { get; set; }

		public string VerbTitle { get; set; }

		public string VerbAuthor { get; set; }

		public string PromptText { get; set; }

		public string VerbSpeed { get; set; }

		public string VerbStartTime { get; set; }

		public string VerbEndTime { get; set; }

		public bool UserSubmitted { get; set; }

		public string UserResponseURL { get; set; }

		public bool UploadedToServer { get; set; }

		public Verbalize()
		{
			VerbalizeID = "";
			VerbalizeDate = "";
			VerbGrade = "";
			VerbDifficulty = "";
			VerbGenre = "";
			VerbTitle = "";
			VerbAuthor = "";
			PromptText = "";
			VerbSpeed = "";
			VerbStartTime = "";
			VerbEndTime = "";
			UserSubmitted = false;
			UserResponseURL = "";
			UploadedToServer = false;
		}
	}
		
	public class DescribeImageUserResponse
	{
		public string ImageID { get; set; }
		public string StudentID { get; set; }
		public string UserResponse { get; set; }
		public string StudentName { get; set; }
		public string StudentImageUrl { get; set; }
	}
		
	public class StudentProfile
	{
		public string Email { get; set; }
		public string StudentID { get; set; }
		public string Grade { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Section { get; set; }
		public string RollNo { get; set; }
	}
		
	public class Feature
	{		
		public string FeatureDate { get; set; }
		public string Grade { get; set; }
		public string Type { get; set; }
		public string MediaUrl { get; set; }
		public string MediaText { get; set; }
		public string ImageUrl { get; set; }
	}

	public class QuizAnalyticsResponse 
	{
		public string TotalAttempts;
		public string Date; 
		public string TotalCorrect; 
		public string Score;
		public bool Success;
	}

	public class Telemetry {
		public string Timestamp;
		public string TimeSpent;
		public string Type;
		public string Description;
	}

	public class GOTGameStatus
	{
		public string ServerTime { get; set; }
		public string GameID { get; set; }
		public string StartTime{ get; set; }
		public string EndTime{ get; set; }
		public string Status { get; set; }
		public string PreviousGameID{ get; set; }
		public Dictionary<string,string> PreviousGameData{ get; set; }
		public List<GOTLeaderboard> GOTLeaderboard{ get; set; }
	}

	public class GOTLeaderboard
	{
		public string GroupName;
		public int GroupCoin;
		public string GroupID;
		public int GroupCell;
	}


}
// namespace ends
