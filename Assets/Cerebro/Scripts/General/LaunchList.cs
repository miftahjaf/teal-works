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
		tKCMasetry,
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
		public Missions mMission = new Missions ();   //Old Mission
		public Dictionary<string,string> mPracticeItemNames = new Dictionary<string, string>();
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
		public Dictionary<string,string> mRegenerationData =new Dictionary<string, string>();
		public Dictionary<string,int> mKCMastery =new Dictionary<string, int>();
		public ProficiencyConstants proficiencyConstants;
		public bool IsVersionUptoDate, CheckingForVersion;
		public Avatar mAvatar;
		public Daily CurrDaily;

		[NonSerialized]
		public MissionData missionData = new MissionData();
		[NonSerialized]
		public HomeworkData homeworkData = new HomeworkData();

		public bool mUpdateTimer = false;
		public System.TimeSpan mTimer;
		int interval = 1; 
		float nextTime = 0;

		private bool IAmServer = false;
		// analytics table intentionally not added here
		private string[] mTableNames = new string[] { "Subject", "Topic", "Subtopic", "ContentItem", "Student" };
		public bool[] mTableLoaded = new bool[] { false, false, false, false, false, false, false,false };

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

		public InternetReachabilityVerifier irv;

		private bool firstTimeNetCheck = true;

		public string mCurrLocalTempPath;
		public bool WelcomeScreenActive = true;
		private static LaunchList m_Instance;

		private bool mHitDynamoDB = false;
		private bool mHitServer = true;
		private bool mUpdatingServerFlagged = false;

		private System.TimeSpan mServerTimeSpan;
		private bool mUpdateServerTime = false;
		private float mServerNextTime;
		private int mServerTimeInterval = 1;
		private DateTime mLastServerTime;
		public bool mUseJSON = false;
		public string VersionData = "v0.1.0.0";
		private string VersionFlagged = "v0.1.0.0";

		private string[] AllLocalFiles;

		public Mfpscounter mpfsCounter;

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

			#if UNITY_IOS && !UNITY_EDITOR
			_GetTempDirectory("Temp Path");
			#endif

			AllLocalFiles = new string[2];
			AllLocalFiles [0] = "DescribeImageSubmitted";
			AllLocalFiles [1] = "Missions";
			missionData.LoadData ();
			ChangeMpfsCounterVisibility ();
		}

		public void LogoutUser() {
			if (WelcomeScript.instance) {
				WelcomeScript.instance.DestroyPooledPrefabs ();
			}
			if (HTTPRequestHelper.instance != null) {
				HTTPRequestHelper.instance.RemoveDeviceToken ();
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
			mPracticeItemNames.Clear ();
			mKCCoins.Clear ();
			mKCMastery.Clear ();
			mDescribeImageUserResponses.Clear ();
			mLeaderboard.Clear ();
			mExplanation.Clear ();
			mGameStatus.Clear ();
			missionData.Clear ();

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
				if (newStatus == InternetReachabilityVerifier.Status.NetVerified && IsVersionUptoDate) {
					mhasInternet = true;
					ScanTables ();
					setWifiIcon ();
					firstTimeNetCheck = false;
				} else if (newStatus == InternetReachabilityVerifier.Status.Offline || newStatus == InternetReachabilityVerifier.Status.Error) {
					bool ver = true;
					Debug.Log (PlayerPrefs.GetString (PlayerPrefKeys.IsVersionUpdated));
					if (PlayerPrefs.GetString (PlayerPrefKeys.IsVersionUpdated, "false") == ver.ToString ()) {
						mhasInternet = false;
						setWifiIcon ();
						mCurrentStudent.Coins = PlayerPrefs.GetInt (PlayerPrefKeys.Coins) + PlayerPrefs.GetInt (PlayerPrefKeys.DeltaCoins);
						GotProperties ();
						GotPracticeItems ();
						GotKCMastery ();
						AllTablesLoaded ();
						firstTimeNetCheck = false;
					} else {
						GetComponent<CerebroScript> ().showVersionDialog ();
					}
				}
			} else {
				if (newStatus == InternetReachabilityVerifier.Status.NetVerified && IsVersionUptoDate) {
					if (mhasInternet == false) {
						mhasInternet = true;
						setWifiIcon ();
						InternetBack ();
					}
					mhasInternet = true;
				} else if (newStatus == InternetReachabilityVerifier.Status.Offline || newStatus == InternetReachabilityVerifier.Status.Error) {
					bool ver = true;
					Debug.Log (PlayerPrefs.GetString (PlayerPrefKeys.IsVersionUpdated));
					if (PlayerPrefs.GetString (PlayerPrefKeys.IsVersionUpdated, "false") == ver.ToString ()) {
						setWifiIcon ();
						mhasInternet = false;
					} else {
						GetComponent<CerebroScript> ().showVersionDialog ();
					}
				}
			}
			if (!IsVersionUptoDate && !CheckingForVersion) {
				CheckVersionNumber ();
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
			string lastVersion = PlayerPrefs.GetString (PlayerPrefKeys.CerebroLastVersion, "0");
			string ConvertedToJSON = PlayerPrefs.GetString (PlayerPrefKeys.IsConvertedToJSON, "check");
			if (lastVersion != VersionHelper.GetVersionNumber() && ConvertedToJSON == "false") {
				PlayerPrefs.SetString (PlayerPrefKeys.IsConvertedToJSON, "check");
			}
			PlayerPrefs.SetString (PlayerPrefKeys.CerebroLastVersion, VersionHelper.GetVersionNumber());

//			UpdateServerTime ();
//			LoadPracticeItems ();
			CheckIfFileJSON ();

			wifiOff.SetActive (false);

			irv = GetComponent<InternetReachabilityVerifier> ();
			irv.statusChangedDelegate += netStatusChanged;

			firstTimeNetCheck = true;

//			mhasInternet = true;
//			QueryAnalyticsTable (null);
//			BackupTable (null, "Moves", Application.persistentDataPath + "/MovesCSV.csv");
//			BackupTable (null, "Analytics", Application.persistentDataPath + "/Analytics.csv");
		}

		public void UpdateServerTime()
		{
			HTTPRequestHelper.instance.GetServerTime ();
		}

		public DateTime GetCurrentTime()
		{
			DateTime currTime = mLastServerTime.Add (mServerTimeSpan);
//			Debug.Log (currTime.ToString("G"));
			GetCurrentLocalTime ();
			return currTime;
		}

		public DateTime GetCurrentLocalTime()
		{
			DateTime currTime = mLastServerTime.Add (mServerTimeSpan);
			currTime = currTime.Add (new TimeSpan (5, 30, 0));
//			Debug.Log ("Local "+currTime.ToString("G"));
			return currTime;
		}

		public void SetCurrentTime(string time)
		{
			if (time == "-1" || time.Length < 23) {
				DateTime lastDt = DateTime.MinValue;
				time = PlayerPrefs.GetString (PlayerPrefKeys.LastLocalTime, "-1");
				Debug.Log ("time "+time);
				if (time != "-1" && DateTime.TryParseExact (time, "yyyy-MM-ddTHH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out lastDt)) {
					DateTime now = System.DateTime.Now;
					TimeSpan diff = now.Subtract (lastDt);
					Debug.Log ("diff "+diff.Ticks);
					if (diff.Ticks > 0) {
						mServerTimeSpan = mServerTimeSpan.Add (diff);
						mUpdateServerTime = true;
					} else {
						Debug.Log ("Invalid time. have to use local time");
					}
				}
			} else {
				if (time.Length > 23) {
					time = time.Substring (0, 23);
				}

				DateTime now = DateTime.MinValue;
				if (DateTime.TryParseExact (time, "yyyy-MM-ddTHH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out now)) {
					Debug.Log ("time " + now.ToString ("T"));
					mServerTimeSpan = new TimeSpan (0);
					mUpdateServerTime = true;
					mLastServerTime = now;
					mServerNextTime = Time.time + mServerTimeInterval;
				}
			}
		}

//		void OnApplicationFocus( bool focusStatus )
//		{
//			if(!focusStatus)
//			{				
//				if (mUpdateServerTime) {
//					PlayerPrefs.SetString (PlayerPrefKeys.LastLocalTime, System.DateTime.Now.ToString ("yyyy-MM-ddTHH:mm:ss.fff"));
//				} else {
//					PlayerPrefs.SetString (PlayerPrefKeys.LastLocalTime, "-1");
//				}
////				Debug.Log ("setting "+PlayerPrefs.GetString (PlayerPrefKeys.LastLocalTime, "-1"));
//				mUpdateServerTime = false;
//			}
//			else
//			{
//				if (mhasInternet) {
//					Debug.Log ("I have internet");
//					UpdateServerTime ();
//				} else {
//					DateTime lastDt = DateTime.MinValue;
//					string time = PlayerPrefs.GetString (PlayerPrefKeys.LastLocalTime, "-1");
////					Debug.Log ("time "+time);
//					if (time != "-1" && DateTime.TryParseExact (time, "yyyy-MM-ddTHH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out lastDt)) {
//						DateTime now = System.DateTime.Now;
//						TimeSpan diff = now.Subtract (lastDt);
////						Debug.Log ("diff "+diff.TotalSeconds);
//						if (diff.Ticks > 0) {
//							DateTime currTime = mLastServerTime.Add (mServerTimeSpan);
//							currTime = currTime.Add (new TimeSpan (5, 30, 0));
////							Debug.Log ("before "+currTime.ToString("G"));
////							Debug.Log ("b seconds "+mServerTimeSpan.TotalSeconds);
//							mServerTimeSpan = mServerTimeSpan.Add (diff);
//							currTime = mLastServerTime.Add (mServerTimeSpan);
//							currTime = currTime.Add (new TimeSpan (5, 30, 0));
////							Debug.Log ("after "+currTime.ToString("G"));
////							Debug.Log ("a seconds "+mServerTimeSpan.TotalSeconds);
//							mUpdateServerTime = true;
//						} else {
//							Debug.Log ("Invalid time. have to use local time");
//						}
//					}
//				}
//			}
//		}

		string GetEmptyFileWithVersion()
		{
			JSONNode N = JSONClass.Parse ("{\"VersionNumber\"}");
			N ["VersionNumber"] = VersionData;
			return N.ToString ();
		}

		public void CheckIfFileJSON()
		{			
			if (!mUseJSON)
				return;

			string ConvertedToJSON = PlayerPrefs.GetString (PlayerPrefKeys.IsConvertedToJSON, "check");
			Debug.Log ("checking "+ConvertedToJSON);
			if (ConvertedToJSON == "false") {
				mUseJSON = false;
				return;
			} else if (ConvertedToJSON == "true") {
				return;
			}

			try {
				string fileName = Application.persistentDataPath + "/DescribeImageSubmitted.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck (data)) {
						ConvertDescribeImageSubmittedToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/DescribeImageSubmittedJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/Missions.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertMissionFileToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/MissionsJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/FlaggedQuestions.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertFlaggedQuestionsToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/FlaggedQuestionsJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/LocalAnalytics.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertLocalAnalyticsToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/LocalAnalyticsJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/QuizSubmitted.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertQuizSubmittedToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/QuizSubmittedJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/QuizAnalytics.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertQuizAnalyticsToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/QuizAnalyticsJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/QuizHistory.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertQuizHistoryToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/QuizHistoryJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/SentAnalytics.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertSentAnalyticsToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/SentAnalyticsJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/UsageAnalytics.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertUsageAnalyticsToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/UsageAnalyticsJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/PracticeData.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertPracticeDataToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/PracticeDataJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/PracticeCount.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertPracticeCountToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/PracticeCountJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/WatchedVideos.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertWatchedVideosToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/WatchedVideosJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/LastImageID.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertLastImageIDToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/LastImageIDJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				fileName = Application.persistentDataPath + "/LastVerbalizeID.txt";
				if (File.Exists (fileName)) {
					string data = File.ReadAllText (fileName);
					if (!IsJsonValidDirtyCheck(data)) {
						ConvertLastVerbalizeIDToJSON ();
					}
				} else {
					fileName = Application.persistentDataPath + "/LastVerbalizeIDJSON.txt";
					if (!File.Exists (fileName)) {
						File.WriteAllText (fileName, GetEmptyFileWithVersion ());
					}
				}

				PlayerPrefs.SetString (PlayerPrefKeys.IsConvertedToJSON, "true");
			} catch(Exception e) {
				mUseJSON = false;
				CerebroHelper.DebugLog ("conversion to JSON didn't work. going back to csv");
				HTTPRequestHelper.instance.SendFeedback (PlayerPrefs.GetString(PlayerPrefKeys.IDKey, "0"), "ErrorLog "+ e.ToString(), ExceptionSentToServer);
			}
		}

		void ExceptionSentToServer(int result)
		{
			if (result == 1) {
				PlayerPrefs.SetString (PlayerPrefKeys.IsConvertedToJSON, "false");
			}
		}

		void ConvertLastImageIDToJSON()
		{
			string fileName = Application.persistentDataPath + "/LastImageID.txt";
			string targetFileName = Application.persistentDataPath + "/LastImageIDJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				if (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 1) {
						N ["Data"] ["ImageID"] = lineArr [0];
						N ["Data"] ["Date"] = lineArr [1];
					}
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		void ConvertLastVerbalizeIDToJSON()
		{
			string fileName = Application.persistentDataPath + "/LastVerbalizeID.txt";
			string targetFileName = Application.persistentDataPath + "/LastVerbalizeIDJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				if (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 1) {
						N ["Data"] ["VerbalizeID"] = lineArr [0];
						N ["Data"] ["Date"] = lineArr [1];
					}
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		void ConvertWatchedVideosToJSON()
		{
			string fileName = Application.persistentDataPath + "/WatchedVideos.txt";
			string targetFileName = Application.persistentDataPath + "/WatchedVideosJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 0) {
						N ["Data"] [cnt] ["VideoContentID"] = lineArr [0];
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		void ConvertQuizSubmittedToJSON()
		{
			string fileName = Application.persistentDataPath + "/QuizSubmitted.txt";
			string targetFileName = Application.persistentDataPath + "/QuizSubmittedJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 3) {
						N ["Data"] [cnt] ["quizDate"] = lineArr [0];
						N ["Data"] [cnt] ["totalAttempts"] = lineArr [1];
						N ["Data"] [cnt] ["correct"] = lineArr [2];
						N ["Data"] [cnt] ["score"] = lineArr [3];
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		void ConvertQuizAnalyticsToJSON()
		{
			string fileName = Application.persistentDataPath + "/QuizAnalytics.txt";
			string targetFileName = Application.persistentDataPath + "/QuizAnalyticsJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 6) {
						N ["Data"] ["Quiz"] [cnt] ["studentID"] = lineArr [0];
						N ["Data"] ["Quiz"] [cnt] ["StudentAndQuestionID"] = lineArr [1];
						N ["Data"] ["Quiz"] [cnt] ["Answer"] = lineArr [2];
						N ["Data"] ["Quiz"] [cnt] ["IsCorrect"] = lineArr [3];
						N ["Data"] ["Quiz"] [cnt] ["TimeStarted"] = lineArr [4];
						N ["Data"] ["Quiz"] [cnt] ["TimeTaken"] = lineArr [5];
						N ["Data"] ["Quiz"] [cnt] ["quizDate"] = lineArr [6];
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		void ConvertQuizHistoryToJSON()
		{
			string fileName = Application.persistentDataPath + "/QuizHistory.txt";
			string targetFileName = Application.persistentDataPath + "/QuizHistoryJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 6) {
						N ["Data"] ["Quiz"] [cnt] ["studentID"] = lineArr [0];
						N ["Data"] ["Quiz"] [cnt] ["StudentAndQuestionID"] = lineArr [1];
						N ["Data"] ["Quiz"] [cnt] ["Answer"] = lineArr [2];
						N ["Data"] ["Quiz"] [cnt] ["IsCorrect"] = lineArr [3];
						N ["Data"] ["Quiz"] [cnt] ["TimeStarted"] = lineArr [4];
						N ["Data"] ["Quiz"] [cnt] ["TimeTaken"] = lineArr [5];
						N ["Data"] ["Quiz"] [cnt] ["quizDate"] = lineArr [6];
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		void ConvertUsageAnalyticsToJSON()
		{
			string fileName = Application.persistentDataPath + "/UsageAnalytics.txt";
			string targetFileName = Application.persistentDataPath + "/UsageAnalyticsJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 3) {
						N ["Data"] [cnt] ["type"] = lineArr [0];
						N ["Data"] [cnt] ["description"] = lineArr [1];
						N ["Data"] [cnt] ["timestamp"] = lineArr [2];
						N ["Data"] [cnt] ["timeSpent"] = lineArr [3];
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		void ConvertPracticeDataToJSON()
		{
			string fileName = Application.persistentDataPath + "/PracticeData.txt";
			string targetFileName = Application.persistentDataPath + "/PracticeDataJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 2) {
						N ["Data"] [cnt] ["practiceId"] = lineArr [0];
						N ["Data"] [cnt] ["attempts"] = lineArr [1];
						N ["Data"] [cnt] ["correct"] = lineArr [2];
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		void ConvertPracticeCountToJSON()
		{
			string fileName = Application.persistentDataPath + "/PracticeCount.txt";
			string targetFileName = Application.persistentDataPath + "/PracticeCountJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 2) {
						N ["Data"] [cnt] ["date"] = lineArr [0];
						N ["Data"] [cnt] ["attempts"] = lineArr [1];
						N ["Data"] [cnt] ["correct"] = lineArr [2];
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		public bool IsJsonValidDirtyCheck(string data)
		{
			if (data.Length > 1 && data [0] == '{' && data [data.Length - 1] == '}') {
				return true;
			}
			return false;
		}

		public void CheckVersionNumber()
		{
			CheckingForVersion = true;
			HTTPRequestHelper.instance.CheckVersionNumber (VersionVerified);
		}

		public string ConvertDateToStandardString(DateTime date)
		{
			return date.ToString("yyyy-MM-ddTHH:mm:ss");
		}

		public DateTime ConvertStringToStandardDate(string dateString)
		{
			return DateTime.ParseExact (dateString, "yyyy-MM-ddTHH:mm:ss", null);
		}

		public void ChangeMpfsCounterVisibility()
		{
			mpfsCounter.enabled = CerebroHelper.isTestUser ();
		}

		public void VersionVerified(bool dummy)
		{
			PlayerPrefs.SetString (PlayerPrefKeys.IsVersionUpdated, LaunchList.instance.IsVersionUptoDate.ToString ());
			if (LaunchList.instance.IsVersionUptoDate) {
				CerebroHelper.DebugLog ("Version upto date");
				GetComponent<CerebroScript> ().RemoveVersionDialog ();
				irv.Start ();
			} else {
				CerebroHelper.DebugLog ("Old Version");
				GetComponent<CerebroScript> ().showVersionDialog ();
			}
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
			PushLocalAnalyticsToServerJSON ();
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

				GetPracticeItemNames ();
				GetExplanation ();
				GetKCMastery ();
				for (int i = 0; i < (int)Cerebro.TableTypes.kMaxTables - 3; i++) { 
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

			for (int i = 0; i < (int)Cerebro.TableTypes.kMaxTables - 3; i++) { 
				mTableLoaded [i] = true;
				if (i == (int)Cerebro.TableTypes.tContent) {
					mTableLoaded [i] = false;
					if (mHitServer) {
						HTTPRequestHelper.instance.GetContentItems ();
					}
				} else if (i == (int)Cerebro.TableTypes.tStudent) {
					mTableLoaded [i] = false;
					GetStudentData (studentID, grade);
					if (PlayerPrefs.HasKey (PlayerPrefKeys.DeviceToken)) {
						Debug.Log ("added token");
						HTTPRequestHelper.instance.SendUrbanDeviceToken (PlayerPrefs.GetString (PlayerPrefKeys.DeviceToken));
					} else {
						Debug.Log ("added token not set");
					}
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

		public void GetMission (string missionID) //Old Mission
		{
			if (CheckLocalMissionCompleteJSON ()) {
				return;
			}
			if (mMission != null && mMission.MissionID == missionID) {
				GotMissions ();
				return;
			}
			if (GetFileMissionIDJSON () == missionID) {
				PopulateMissionFromLocalFileJSON ();
			} else {
				if (mHitServer) {
					HTTPRequestHelper.instance.GetMissionQuestions (missionID);
				} 
			}
		}	
			

		public void GetMission()
		{
			
			if (missionData.GetNotCompletedMissionCount () > 0) {
				GotMission ();
			} else if (missionData.dataList.Count > 0) {
				UploadCompletedMission ();
			}
			else
			{
				if (mHitServer) {
					HTTPRequestHelper.instance.GetMission ();
				} 
			}
		}

		public void UploadCompletedMission()
		{
			for (int i = 0; i < missionData.dataList.Count; i++) {
				if (!string.IsNullOrEmpty (missionData.dataList [i].endTime)) {
					HTTPRequestHelper.instance.SubmitCompletedMission (missionData.dataList [i]);
				}
			}
		}

		public void GotMission()
		{
			if (m_Instance.MissionLoaded != null) {
				m_Instance.MissionLoaded.Invoke (this, null);
			} else {
				CerebroHelper.DebugLog ("NO HANDLE Mission Loaded");
			}
		}

	
		public void GotMissions () //Old Mission
		{
			WriteMissionToFileJSON ();
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

		private void ConvertMissionFileToJSON ()
		{
			string fileName = Application.persistentDataPath + "/Missions.txt";
			string targetFileName = Application.persistentDataPath + "/MissionsJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			string line = "";
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				line = sr.ReadLine ();
				var lineArray = line.Split (new string[] { "," }, System.StringSplitOptions.None);
				if (mMission.MissionID != lineArray [0]) {
					line = "";
				}
				if (lineArray != null && lineArray.Length > 2) {
					N ["Data"] ["MissionID"] = lineArray [0];
					N ["Data"] ["MissionName"] = lineArray [1];
					N ["Data"] ["TimeStarted"] = lineArray [2];
				}

				line = sr.ReadLine ();
				int cnt = 0;
				while (line != null) {					
					var lineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
					MissionItemData itemData = new MissionItemData ();

					if (lineArr.Length > 11) {
						N ["Data"] ["MissionItemData"] [cnt] ["QuestionID"] = lineArr [0];
						N ["Data"] ["MissionItemData"] [cnt] ["PracticeItemID"] = lineArr [1];
						N ["Data"] ["MissionItemData"] [cnt] ["QuestionTitle"] = lineArr [2];
						N ["Data"] ["MissionItemData"] [cnt] ["QuestionLevel"] = lineArr [3];
						N ["Data"] ["MissionItemData"] [cnt] ["SubLevel"] = lineArr [4];
						N ["Data"] ["MissionItemData"] [cnt] ["CompletionCondition"] ["key"] = lineArr [5];
						N ["Data"] ["MissionItemData"] [cnt] ["CompletionCondition"] ["value"] = lineArr [6];
						N ["Data"] ["MissionItemData"] [cnt] ["ConditionCurrentValue"] = lineArr [7];
						N ["Data"] ["MissionItemData"] [cnt] ["TotalAttempts"] = lineArr [8];
						N ["Data"] ["MissionItemData"] [cnt] ["CorrectAttempts"] = lineArr [9];
						N ["Data"] ["MissionItemData"] [cnt] ["CompleteBool"] = lineArr [10];
						N ["Data"] ["MissionItemData"] [cnt] ["Type"] = lineArr [11];
						if (lineArr.Length > 12 && lineArr [12] != "null")
							N ["Data"] ["MissionItemData"] [cnt] ["QuestionText"] = lineArr [12];
						else
							N ["Data"] ["MissionItemData"] [cnt] ["QuestionText"] = "";
						if (lineArr.Length > 13 && lineArr [13] != "null")
							N ["Data"] ["MissionItemData"] [cnt] ["AnswerOptions"] = lineArr [13];
						else
							N ["Data"] ["MissionItemData"] [cnt] ["AnswerOptions"] = "";
						if (lineArr.Length > 14 && lineArr [14] != "null")
							N ["Data"] ["MissionItemData"] [cnt] ["Answer"] = lineArr [14];
						else
							N ["Data"] ["MissionItemData"] [cnt] ["Answer"] = "";
						if (lineArr.Length > 15 && lineArr [15] != "null")
							N ["Data"] ["MissionItemData"] [cnt] ["QuestionMediaType"] = lineArr [15];
						else
							N ["Data"] ["MissionItemData"] [cnt] ["QuestionMediaType"] = "";
						if (lineArr.Length > 16 && lineArr [16] != "null")
							N ["Data"] ["MissionItemData"] [cnt] ["QuestionMediaURL"] = lineArr [16];
						else
							N ["Data"] ["MissionItemData"] [cnt] ["QuestionMediaURL"] = "";
					}
					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		private void WriteMissionToFileJSON ()
		{
			if (!mUseJSON) {
				WriteMissionToFile ();
				return;
			}
			
			string fileName = Application.persistentDataPath + "/MissionsJSON.txt";
			if (!File.Exists (fileName))
				return;

			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);

			if (N ["Data"] ["MissionID"].Value != mMission.MissionID) {
				N ["Data"] ["MissionID"] = mMission.MissionID;
				N ["Data"] ["MissionName"] = mMission.MissionName;
				N ["Data"] ["TimeStarted"] = mMission.TimeStarted;
			}

			int cnt = 0;
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

				N ["Data"] ["MissionItemData"] [cnt] ["QuestionID"] = item.Value.QuestionID;
				N ["Data"] ["MissionItemData"] [cnt] ["PracticeItemID"] = item.Value.PracticeItemID;
				N ["Data"] ["MissionItemData"] [cnt] ["QuestionTitle"] = item.Value.QuestionTitle;
				N ["Data"] ["MissionItemData"] [cnt] ["QuestionLevel"] = item.Value.QuestionLevel;
				N ["Data"] ["MissionItemData"] [cnt] ["SubLevel"] = item.Value.SubLevel;
				N ["Data"] ["MissionItemData"] [cnt] ["CompletionCondition"] ["key"] = conditionKey;
				N ["Data"] ["MissionItemData"] [cnt] ["CompletionCondition"] ["value"] = conditionValue;
				N ["Data"] ["MissionItemData"] [cnt] ["ConditionCurrentValue"] = item.Value.ConditionCurrentValue;
				N ["Data"] ["MissionItemData"] [cnt] ["TotalAttempts"] = item.Value.TotalAttempts;
				N ["Data"] ["MissionItemData"] [cnt] ["CorrectAttempts"] = item.Value.CorrectAttempts;
				N ["Data"] ["MissionItemData"] [cnt] ["CompleteBool"] = item.Value.CompleteBool;
				N ["Data"] ["MissionItemData"] [cnt] ["Type"] = item.Value.Type;
				N ["Data"] ["MissionItemData"] [cnt] ["QuestionText"] = item.Value.QuestionText;
				N ["Data"] ["MissionItemData"] [cnt] ["AnswerOptions"] = item.Value.AnswerOptions;
				N ["Data"] ["MissionItemData"] [cnt] ["Answer"] = item.Value.Answer;
				N ["Data"] ["MissionItemData"] [cnt] ["QuestionMediaType"] = item.Value.QuestionMediaType;
				N ["Data"] ["MissionItemData"] [cnt] ["QuestionMediaURL"] = item.Value.QuestionMediaURL;
				cnt++;
			}
				
			File.WriteAllText (fileName, N.ToString());
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
				CheckIfMissionCompleteJSON ();
			}
		}

		public void UpdateLocalMissionFileJSON (MissionItemData item, string questionID, bool isCorrect)
		{
			if (!mUseJSON) {
				UpdateLocalMissionFile (item, questionID, isCorrect);
				return;
			}

			List<string> toWriteLines = new List<string> ();

			bool openMissions = false;
			string completemissionItemID = "";
			string fileName = Application.persistentDataPath + "/MissionsJSON.txt";
			if (!File.Exists (fileName)) {
				return;
			}
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);
			for (int i = 0; i < N ["data"] ["MissionItemData"].Count; i++) {					
				if (N ["data"] ["MissionItemData"] [i] ["QuestionID"].Value == questionID) {
					int currentValue = N ["data"] ["MissionItemData"] [i] ["ConditionCurrentValue"].AsInt;
					int completionValue = N ["Data"] ["MissionItemData"] [i] ["CompletionCondition"] ["value"].AsInt;
					int totalAttempts = N ["Data"] ["MissionItemData"] [i] ["TotalAttempts"].AsInt;
					int correctAttempts = N ["Data"] ["MissionItemData"] [i] ["CorrectAttempts"].AsInt;
					string completeBool = N ["Data"] ["MissionItemData"] [i] ["CompleteBool"].Value;
					totalAttempts++;
					if (N ["Data"] ["MissionItemData"] [i] ["CompletionCondition"] ["key"].Value == "Attempts") {
						currentValue++;
						if (isCorrect) {
							correctAttempts++;
						}
					} else if (N ["Data"] ["MissionItemData"] [i] ["CompletionCondition"] ["key"].Value == "Streak") {
						if (isCorrect) {
							currentValue++;
							correctAttempts++;
						} else {
							currentValue = 0;
						}
					} else if (N ["Data"] ["MissionItemData"] [i] ["CompletionCondition"] ["key"].Value == "Correct" && isCorrect) {
						correctAttempts++;
						currentValue++;	
					}
					if (currentValue >= completionValue && completeBool == "false") {
						completeBool = "true";
						openMissions = true;
						completemissionItemID = N ["Data"] ["MissionID"].Value;
					}
					N ["data"] ["MissionItemData"] [i] ["ConditionCurrentValue"] = currentValue.ToString ();
					N ["Data"] ["MissionItemData"] [i] ["CompleteBool"] = completeBool;
					N ["Data"] ["MissionItemData"] [i] ["TotalAttempts"] = totalAttempts.ToString ();
					N ["Data"] ["MissionItemData"] [i] ["CorrectAttempts"] = correctAttempts.ToString ();
				}
			}

			File.WriteAllText (fileName, string.Empty);
			File.WriteAllText (fileName, N.ToString());

			if (openMissions) {
				if (WelcomeScript.instance.autoTestMissionCorrect == true || WelcomeScript.instance.autoTestMissionMix) {
					WelcomeScript.instance.RemoveScreens ();
				}
				WelcomeScript.instance.ShowScreen (true, completemissionItemID);
				CheckIfMissionCompleteJSON ();
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

		bool CheckLocalMissionCompleteJSON ()
		{
			bool toReturn = false;
			if (!mUseJSON) {
				toReturn = CheckLocalMissionComplete ();
				return toReturn;
			}

			string fileName = Application.persistentDataPath + "/MissionsJSON.txt";
			if (!File.Exists (fileName)) {
				return false;
			}
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return false;
			}
			JSONNode N = JSONClass.Parse (data);
			if (N ["Data"] ["UploadBool"] != null && N ["Data"] ["UploadBool"].Value == "false") {
				string day = N ["Data"] ["Day"].Value;
				string timeStarted = N ["Data"] ["TimeStarted"].Value;
				string timeEnded = N ["Data"] ["TimeEnded"].Value;
				string missionID = N ["Data"] ["MissionID"].Value;
				int totalAttempts = 0;
				int totalCorrect = 0;

				for (int i = 0; i < N ["Data"] ["MissionItemData"].Count; i++) {
					int correctAnswers = N ["Data"] ["MissionItemData"] [i] ["CorrectAttempts"].AsInt;
					int attempts = N ["Data"] ["MissionItemData"] [i] ["TotalAttempts"].AsInt;
					totalAttempts += attempts;
					totalCorrect += correctAnswers;
				}

				float accuracy = totalCorrect / totalAttempts;
				toReturn = true;
				PushMissionComplete (missionID, day, timeStarted, timeEnded, accuracy);
			}

			return toReturn;
		}

		bool CheckMissionCompleteJSON ()
		{
			bool toReturn = false;
			string fileName = Application.persistentDataPath + "/MissionJSON.txt";
			if (!File.Exists (fileName)) {
				return false;
			}
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return false;
			}
			JSONNode N = JSONClass.Parse (data);
			if (N ["Data"] ["UploadBool"] != null && N ["Data"] ["UploadBool"].Value == "false") {
				
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
				MissionCompleteAnalyticsJSON (accuracy);
			}
		}

		void CheckIfMissionCompleteJSON ()
		{
			if (!mUseJSON) {
				CheckIfMissionComplete ();
				return;
			}

			int totalAttempts = 0;
			int totalCorrect = 0;
			string fileName = Application.persistentDataPath + "/MissionsJSON.txt";
			bool allComplete = true;
			if (!File.Exists (fileName)) {
				return;
			}
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);
			for (int i = 0; i < N ["Data"] ["MissionItemData"].Count; i++) {
				bool completeBool = N ["Data"] ["MissionItemData"] [i] ["CompleteBool"].AsBool;
				int correctAnswers = N ["Data"] ["MissionItemData"] [i] ["CorrectAttempts"].AsInt;
				int attempts = N ["Data"] ["MissionItemData"] [i] ["TotalAttempts"].AsInt;
				totalAttempts += attempts;
				totalCorrect += correctAnswers;
				if (!completeBool) {
					allComplete = false;
				}
			}

			if (allComplete) {
				float accuracy = Mathf.Round ((float)totalCorrect * 100 / (float)totalAttempts);
				CerebroHelper.DebugLog ("ALL COMPLETE");
				WelcomeScript.instance.ShowMissionComplete (accuracy);
				MissionCompleteAnalyticsJSON (accuracy);
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

		private void MissionCompleteAnalyticsJSON (float accuracy)
		{
			if (!mUseJSON) {
				MissionCompleteAnalytics (accuracy);
				return;
			}

			string fileName = Application.persistentDataPath + "/MissionsJSON.txt";
			if (!File.Exists (fileName)) {
				return;
			}
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);

			string day = System.DateTime.Now.ToUniversalTime ().ToString ("yyyyMMdd");
			string timeStarted = N ["Data"] ["TimeStarted"].Value;
			string timeEnded = System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss");

			PushMissionComplete (mMission.MissionID, day, timeStarted, timeEnded, accuracy);
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

		public void UpdateLocalMissionFileForCompletionJSON (float accuracy, bool uploadBool)
		{
			if (!mUseJSON) {
				UpdateLocalMissionFileForCompletion (accuracy, uploadBool);
				return;
			}

			List<string> toWriteLines = new List<string> ();
			string fileName = Application.persistentDataPath + "/MissionsJSON.txt";
			if (!File.Exists (fileName)) {
				return;
			}
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);

			if (N ["Data"] ["Day"] == null || N ["Data"] ["Day"].Value == "") {
				N ["Data"] ["Day"] = System.DateTime.Now.ToString ("yyyyMMdd");
			}
			if (N ["Data"] ["TimeEnded"] == null || N ["Data"] ["TimeEnded"].Value == "") {
				N ["Data"] ["TimeEnded"] = System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss");
			}
			N ["Data"] ["UploadBool"] = uploadBool.ToString ();
			File.WriteAllText (fileName, string.Empty);
			File.WriteAllText (fileName, N.ToString());

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

		private string GetFileMissionIDJSON ()
		{
			if (!mUseJSON) {
				string id = GetFileMissionID ();
				return id;
			}

			string fileName = Application.persistentDataPath + "/MissionsJSON.txt";
			if (!File.Exists (fileName)) {
				return "";
			}
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return "";
			}
			JSONNode N = JSONClass.Parse (data);
			if (N ["Data"] ["MissionID"] != null)
				return N ["Data"] ["MissionID"].Value;
			else
				return "";
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

		private void PopulateMissionFromLocalFileJSON ()
		{
			if (!mUseJSON) {
				PopulateMissionFromLocalFile ();
				return;
			}

			string fileName = Application.persistentDataPath + "/MissionsJSON.txt";
			if (!File.Exists (fileName)) {
				return;
			}
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);

			mMission.MissionID = N ["Data"] ["MissionID"].Value;
			mMission.MissionName = N ["Data"] ["MissionName"].Value;
			mMission.TimeStarted = N ["Data"] ["TimeStarted"].Value;
			mMission.Questions = new SortedDictionary<string, MissionItemData> ();
			for(int i = 0; i < N ["Data"] ["MissionItemData"].Count; i++) {
				MissionItemData itemData = new MissionItemData ();

				itemData.QuestionID = N ["Data"] ["MissionItemData"] [i] ["QuestionID"].Value;
				itemData.PracticeItemID = N ["Data"] ["MissionItemData"] [i] ["PracticeItemID"].Value;
				itemData.QuestionTitle = N ["Data"] ["MissionItemData"] [i] ["QuestionTitle"].Value;
				itemData.QuestionLevel = N ["Data"] ["MissionItemData"] [i] ["QuestionLevel"].Value;
				itemData.SubLevel = N ["Data"] ["MissionItemData"] [i] ["SubLevel"].Value;
				itemData.CompletionCondition = new SortedDictionary<string, string> ();
				itemData.CompletionCondition.Add (N ["Data"] ["MissionItemData"] [i] ["CompletionCondition"] ["key"].Value, N ["Data"] ["MissionItemData"] [i] ["CompletionCondition"] ["value"].Value);
				itemData.ConditionCurrentValue = N ["Data"] ["MissionItemData"] [i] ["ConditionCurrentValue"].Value;
				itemData.TotalAttempts = N ["Data"] ["MissionItemData"] [i] ["TotalAttempts"].Value;
				itemData.CorrectAttempts = N ["Data"] ["MissionItemData"] [i] ["CorrectAttempts"].Value;
				itemData.CompleteBool = N ["Data"] ["MissionItemData"] [i] ["CompleteBool"].Value;
				itemData.Type = N ["Data"] ["MissionItemData"] [i] ["Type"].Value;
				itemData.QuestionText = N ["Data"] ["MissionItemData"] [i] ["QuestionText"].Value;
				itemData.AnswerOptions = N ["Data"] ["MissionItemData"] [i] ["AnswerOptions"].Value;
				itemData.Answer = N ["Data"] ["MissionItemData"] [i] ["Answer"].Value;
				itemData.QuestionMediaType = N ["Data"] ["MissionItemData"] [i] ["QuestionMediaType"].Value;
				itemData.QuestionMediaURL = N ["Data"] ["MissionItemData"] [i] ["QuestionMediaURL"].Value;
				mMission.Questions.Add (itemData.QuestionID, itemData);
			}
			GotMissions ();
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

		void ConvertSentAnalyticsToJSON()
		{
			string fileName = Application.persistentDataPath + "/SentAnalytics.txt";
			string targetFileName = Application.persistentDataPath + "/SentAnalyticsJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 0) {
						N ["Data"] ["AnalyticsID"] [cnt] = lineArr [0];
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		// XXX what if this function gets hit asynchronously from two threads?
		public void WriteSentAnalyticsJSON (string analyticsId)
		{
			if (!mUseJSON) {
				WriteSentAnalytics (analyticsId);
				return;
			}

			string fileName = Application.persistentDataPath + "/SentAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return;
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);
			int cnt = N ["Data"] ["AnalyticsID"].Count;
			N ["Data"] ["AnalyticsID"] [cnt] = analyticsId;
			File.WriteAllText (fileName, N.ToString());
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

		public void CleanLocalAnalyticsJSON ()
		{
			if (!mUseJSON) {
				CleanLocalAnalytics ();
				return;
			}

			string fileName = Application.persistentDataPath + "/LocalAnalyticsJSON.txt";
			string sentFileName = Application.persistentDataPath + "/SentAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return;
			if (!File.Exists (sentFileName))
				return;
			
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode CurrLocal = JSONClass.Parse (data);
			JSONNode NextLocal = JSONClass.Parse ("{\"Data\"}");

			string data1 = File.ReadAllText (sentFileName);
			if (!IsJsonValidDirtyCheck (data1)) {
				return;
			}
			JSONNode Sent = JSONClass.Parse (data1);

			int myCnt = 0;
			for (int i = 0; i < CurrLocal ["Data"] ["Analytics"].Count; i++) {
				bool found = false;
				for (int j = 0; j < Sent ["Data"] ["AnalyticsID"].Count; j++) {
					if (CurrLocal ["Data"] ["Analytics"] [i] ["assessmentID"].Value == Sent ["Data"] ["AnalyticsID"] [j].Value) {
						found = true;
					}
				}
				if (!found) {
					NextLocal ["Data"] ["Analytics"] [myCnt] ["assessmentID"] = CurrLocal ["Data"] ["Analytics"] [i] ["assessmentID"].Value;
					myCnt++;
				}
			}
			NextLocal ["VersionNumber"] = CurrLocal ["VersionNumber"].Value;
			File.WriteAllText (fileName, NextLocal.ToString());
		}

		public void PushLocalAnalyticsToServer ()
		{
			CerebroHelper.DebugLog ("PushLocalAnalyticsToServer");
			CleanLocalAnalyticsJSON ();
			
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

		public void PushLocalAnalyticsToServerJSON ()
		{
			if (!mUseJSON) {
				PushLocalAnalyticsToServer ();
				return;
			}

			string fileName = Application.persistentDataPath + "/LocalAnalyticsJSON.txt";
			string sentFileName = Application.persistentDataPath + "/SentAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return;
			if (!File.Exists (sentFileName))
				return;
			
			string data = File.ReadAllText (fileName);
			if (!IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode CurrLocal = JSONClass.Parse (data);

			string data1 = File.ReadAllText (sentFileName);
			if (!IsJsonValidDirtyCheck (data1)) {
				return;
			}
			JSONNode Sent = JSONClass.Parse (data1);

			for (int i = 0; i < CurrLocal ["Data"] ["Analytics"].Count; i++) {
				bool found = false;
				for (int j = 0; j < Sent ["Data"] ["AnalyticsID"].Count; j++) {
					if (CurrLocal ["Data"] ["Analytics"] [i] ["assessmentID"].Value == Sent ["Data"] ["AnalyticsID"] [j].Value) {
						found = true;
					}
				}
				if (!found) {
					string playTime = "0";
					string seed = "0";
					string missionField = "";

					if (CurrLocal ["Data"] ["Analytics"] [i] ["playTime"] != null) {
						playTime = CurrLocal ["Data"] ["Analytics"] [i] ["playTime"].Value;
					}
					if (CurrLocal ["Data"] ["Analytics"] [i] ["seed"] != null) {
						seed = CurrLocal ["Data"] ["Analytics"] [i] ["seed"].Value;
					}
					if (CurrLocal ["Data"] ["Analytics"] [i] ["missionField"] != null) {
						missionField = CurrLocal ["Data"] ["Analytics"] [i] ["missionField"].Value;
					}
						
					CerebroHelper.DebugLog (seed);
					JSONNode curr = CurrLocal ["Data"] ["Analytics"] [i];
					if (curr ["isCorrect"].AsBool) {						
						SendAnalytics (curr["assessmentID"], curr["difficulty"], true, curr["day"], curr["timeStarted"], curr["timeTaken"], playTime, seed, missionField);
					} else {
						SendAnalytics (curr["assessmentID"], curr["difficulty"], false, curr["day"], curr["timeStarted"], curr["timeTaken"], playTime, seed, missionField);
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

		void ConvertFlaggedQuestionsToJSON()
		{
			string fileName = Application.persistentDataPath + "/FlaggedQuestions.txt";
			string targetFileName = Application.persistentDataPath + "/FlaggedQuestionsJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 3) {
						N ["Data"] [cnt] ["assessmentID"] = lineArr [0];
						N ["Data"] [cnt] ["difficulty"] = lineArr [1];
						N ["Data"] [cnt] ["sublevel"] = lineArr [2];
						N ["Data"] [cnt] ["seed"] = lineArr [3];
						N ["Data"] [cnt] ["isFlagged"] = "true";
						N ["Data"] [cnt] ["updatedOnServer"] = "false";
						N ["Data"] [cnt] ["FlaggedVersion"] = VersionFlagged;
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		public void WriteFlaggedQuestionToFileJSON (string assessmentID, int difficulty, int sublevel, int seed)
		{
			if (!mUseJSON) {
				WriteFlaggedQuestionToFile (assessmentID, difficulty, sublevel, seed);
				return;
			}

			string fileName = Application.persistentDataPath + "/FlaggedQuestionsJSON.txt";
			if (!File.Exists (fileName))
				return;
			
			int cnt = 0;
			bool isFound = false;
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {				
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				cnt = N ["Data"].Count;
				File.WriteAllText (fileName, string.Empty);
				for (int i = 0; i < cnt; i++) {
					if (N ["Data"] [i] ["assessmentID"].Value == assessmentID && N ["Data"] [i] ["difficulty"].AsInt == difficulty &&
						N ["Data"] [i] ["sublevel"].AsInt == sublevel && N ["Data"] [i] ["seed"].AsInt == seed)
					{
						N ["Data"] [i] ["isFlagged"] = "true";
						N ["Data"] [i] ["updatedOnServer"] = "false";
						isFound = true;
						break;
					}
				}
			}
			if (!isFound) {
				N ["Data"] [cnt] ["assessmentID"] = assessmentID;
				N ["Data"] [cnt] ["difficulty"] = difficulty.ToString ();
				N ["Data"] [cnt] ["sublevel"] = sublevel.ToString ();
				N ["Data"] [cnt] ["seed"] = seed.ToString ();
				N ["Data"] [cnt] ["isFlagged"] = "true";
				N ["Data"] [cnt] ["updatedOnServer"] = "false";
				N ["Data"] [cnt] ["FlaggedVersion"] = VersionFlagged;
			}
			File.WriteAllText (fileName, N.ToString());
			CheckForFlaggedQuestionToSend ();
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

		public void MarkUnflagged (string assessmentID)
		{
			if (!mUseJSON) {
				RemoveFlaggedQuestionFromFile (assessmentID);
				return;
			}

			string fileName = Application.persistentDataPath + "/FlaggedQuestionsJSON.txt";
			if (!File.Exists (fileName))
				return;

			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			string data = File.ReadAllText (fileName);
			N = JSONClass.Parse (data);
			File.WriteAllText (fileName, string.Empty);
			for (int i = 0; i < N["Data"].Count; i++) {
				if (N ["Data"] [i] ["assessmentID"].Value == assessmentID) {
					N ["Data"] [i] ["isFlagged"] = "false";
					N ["Data"] [i] ["updatedOnServer"] = "false";
					break;
				}
			}
			File.WriteAllText (fileName, N.ToString());
			CheckForFlaggedQuestionToSend ();
		}

		public void MarkAsSentOnServer (string assessmentID)
		{
			string fileName = Application.persistentDataPath + "/FlaggedQuestionsJSON.txt";
			if (!File.Exists (fileName))
				return;
			
			int cnt = 0;
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			JSONNode N1 = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {				
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				cnt = N ["Data"].Count;
				File.WriteAllText (fileName, string.Empty);
				int myCnt = 0;
				for (int i = 0; i < cnt; i++) {
					if (N ["Data"] [i] ["assessmentID"].Value == assessmentID) {
						if (N ["Data"] [i] ["isFlagged"].AsBool) {
							N1 ["Data"] [myCnt] = N ["Data"] [i];
							N1 ["Data"] [myCnt] ["updatedOnServer"] = "true";
							myCnt++;
						}
					} else {
						N1 ["Data"] [myCnt] = N ["Data"] [i];
						myCnt++;
					}
				}
				N1 ["VersionNumber"] = N ["VersionNumber"].Value;
				File.WriteAllText (fileName, N1.ToString());
			}
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

		public List<FlaggedQuestion> GetFlaggedQuestionsJSON()
		{
			string fileName = Application.persistentDataPath + "/FlaggedQuestionsJSON.txt";
			List<FlaggedQuestion> AllFlaggedQuestions = new List<FlaggedQuestion> ();
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!IsJsonValidDirtyCheck (data)) {
					return null;
				}
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					if (N ["Data"] [i] ["isFlagged"].AsBool) {
						FlaggedQuestion f = new FlaggedQuestion ();
						f.AssessmentItemID = N ["Data"] [i] ["assessmentID"].Value;
						f.Difficulty = N ["Data"] [i] ["difficulty"].Value;
						f.SubLevel = N ["Data"] [i] ["sublevel"].Value;
						f.RandomSeed = N ["Data"] [i] ["seed"].Value;
						AllFlaggedQuestions.Add (f);
					}
				}
			}
			return AllFlaggedQuestions;
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

		void ConvertLocalAnalyticsToJSON()
		{
			string fileName = Application.persistentDataPath + "/LocalAnalytics.txt";
			string targetFileName = Application.persistentDataPath + "/LocalAnalyticsJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				while (line != null) {
					var lineArr = line.Split ("," [0]);

					if (lineArr.Length > 9) {
						N ["Data"] ["Analytics"] [cnt] ["studentID"] = lineArr [0];
						N ["Data"] ["Analytics"] [cnt] ["assessmentID"] = lineArr [1];
						N ["Data"] ["Analytics"] [cnt] ["difficulty"] = lineArr [2];
						N ["Data"] ["Analytics"] [cnt] ["isCorrect"] = lineArr [3];
						N ["Data"] ["Analytics"] [cnt] ["day"] = lineArr [4];
						N ["Data"] ["Analytics"] [cnt] ["timeStarted"] = lineArr [5];
						N ["Data"] ["Analytics"] [cnt] ["timeTaken"] = lineArr [6];
						N ["Data"] ["Analytics"] [cnt] ["playTime"] = lineArr [7];
						N ["Data"] ["Analytics"] [cnt] ["seed"] = lineArr [8];
						N ["Data"] ["Analytics"] [cnt] ["missionField"] = lineArr [9];
						N ["Data"] ["Analytics"] [cnt] ["CoinsEarned"] = "0";
						cnt++;
					}

					line = sr.ReadLine ();
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		public void WriteAnalyticsToFileJSON (string assessmentID, int difficulty, bool correct, string day, string timeStarted, int timeTaken, string playTime, int seed, string missionField, string UserAnswer = "", int CoinsEarned = 0, bool ignoreInternet = false)
		{
			if (!mUseJSON) {
				WriteAnalyticsToFile (assessmentID, difficulty, correct, day, timeStarted, timeTaken, playTime, seed, missionField, UserAnswer, ignoreInternet);
				return;
			}

			if (mhasInternet && !ignoreInternet) {
				SendAnalytics (assessmentID, difficulty.ToString (), correct, day, timeStarted, timeTaken.ToString (), playTime, seed.ToString (), missionField, UserAnswer, CoinsEarned);
			} else {
				if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
					CerebroHelper.DebugLog ("WriteAnalyticsToFile - no ID set");
					return;
				}

				var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

				string fileName = Application.persistentDataPath + "/LocalAnalyticsJSON.txt";
				if (!File.Exists (fileName))
					return;
				
				string data = File.ReadAllText (fileName);
				if (!IsJsonValidDirtyCheck (data)) {
					return;
				}
				JSONNode N = JSONClass.Parse (data);
				int cnt = N ["Data"]["Analytics"].Count;

				N ["Data"]["Analytics"][cnt]["studentID"] = studentID;
				N ["Data"]["Analytics"][cnt]["assessmentID"] = assessmentID;
				N ["Data"]["Analytics"][cnt]["difficulty"] = difficulty.ToString();
				N ["Data"]["Analytics"][cnt]["isCorrect"] = correct.ToString();
				N ["Data"]["Analytics"][cnt]["day"] = day;
				N ["Data"]["Analytics"][cnt]["timeStarted"] = timeStarted;
				N ["Data"]["Analytics"][cnt]["timeTaken"] = timeTaken.ToString();
				N ["Data"]["Analytics"][cnt]["playTime"] = playTime;
				N ["Data"]["Analytics"][cnt]["seed"] = seed.ToString();
				N ["Data"]["Analytics"][cnt]["missionField"] = missionField;
				N ["Data"] ["Analytics"] [cnt] ["CoinsEarned"] = CoinsEarned.ToString ();

				File.WriteAllText (fileName, string.Empty);
				File.WriteAllText (fileName, N.ToString());
			}
		}

	
		// if we are offline this function needs to save this data away
		// and then upload it when we come online
		public void SendAnalytics (string assessmentID, string difficulty, bool correct, string day, string timeStarted, string timeTaken, string playTime, string seed, string missionField, string UserAnswer = "", int CoinsEarned = 0)
		{
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("SendAnalytics - no ID set");
				return;
			}
				
			if (mHitServer) {
				HTTPRequestHelper.instance.SendAnalytics (assessmentID, difficulty, correct, day, timeStarted, timeTaken, playTime, seed, missionField, UserAnswer, CoinsEarned);
			}

		}

		public void SendYoutubeAnalytics(string componentName,string createdAt, string searchOrVideoText, string videoId, string startTime, string endTime,System.Action<JSONNode> callback = null)
		{
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("SendAnalytics - no ID set");
				return;
			}

			if (mHitServer) {
				HTTPRequestHelper.instance.SendYoutubeAnalytics (componentName, createdAt, searchOrVideoText, videoId, startTime, endTime,callback);
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
			//CheckLocalMissionCompleteJSON ();
			UploadCompletedMission();
			CheckForHomeworkResponseToSend ();
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

			/*if (CerebroHelper.isTestUser ()) {
				mMSPF = Time.deltaTime * 1000.0f;
			}*/

			if (mUpdateServerTime) {
				if (Time.time > mServerNextTime) {
					mServerTimeSpan = mServerTimeSpan.Add (new TimeSpan (0, 0, mServerTimeInterval));
					mServerNextTime += mServerTimeInterval;
//					GetCurrentTime ();
				}
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

		public void SentQuizAnalyticsJSON (string studentAndQuestionID, string quizDate)
		{
			if (!mUseJSON) {
				SentQuizAnalytics (studentAndQuestionID, quizDate);
				return;
			}

			string fileName = Application.persistentDataPath + "/QuizAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return;
			
			string data = File.ReadAllText (fileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);
			JSONNode N1 = JSONClass.Parse ("{\"Data\"}");
			int cnt = 0;
			for (int i = 0; i < N ["Data"] ["Quiz"].Count; i++) {
				if (N ["Data"] ["Quiz"] [i] ["quizDate"].Value != quizDate && N ["Data"] ["Quiz"] [i] ["StudentAndQuestionID"].Value != studentAndQuestionID) {
					N1 ["Data"] ["Quiz"] [cnt] ["studentID"] = N ["Data"] ["Quiz"] [i] ["studentID"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["StudentAndQuestionID"] = N ["Data"] ["Quiz"] [i] ["StudentAndQuestionID"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["Answer"] = N ["Data"] ["Quiz"] [i] ["Answer"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["IsCorrect"] = N ["Data"] ["Quiz"] [i] ["IsCorrect"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["TimeStarted"] = N ["Data"] ["Quiz"] [i] ["TimeStarted"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["TimeTaken"] = N ["Data"] ["Quiz"] [i] ["TimeTaken"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["quizDate"] = N ["Data"] ["Quiz"] [i] ["quizDate"].Value;
					cnt++;
				}
			}
			N1 ["VersionNumber"] = N ["VersionNumber"].Value;
			File.WriteAllText (fileName, N1.ToString());
		}

		public void SentQuizAnalyticsGrouped (List<QuizAnalytics> AllQuestions)
		{
			foreach (var question in AllQuestions) {
				SentQuizAnalyticsJSON (question.StudentAndQuestionID, question.QuizDate);
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

		public void LoadKCMastery()
		{
			string fileName = Application.persistentDataPath + "/KCsMasteryLatest.txt";
			proficiencyConstants = new ProficiencyConstants ();

			if (File.Exists (fileName)) 
			{
				mKCMastery.Clear ();
				string json = File.ReadAllText (fileName);
				JSONNode jsonNode = JSONNode.Parse (json);

				if (jsonNode != null) {
					
					if (jsonNode ["Data"] ["Mastery"] != null) {
						int length = jsonNode ["Data"] ["Mastery"].Count;
						for (int i = 0; i < length; i++) {
							string KCID = jsonNode ["Data"] ["Mastery"] [i] ["id"];
							int mastery = jsonNode ["Data"] ["Mastery"] [i] ["mastery"].AsInt;
							if (mKCMastery.ContainsKey (KCID)) {
								mKCMastery [KCID] = mastery;
							} else {
								mKCMastery.Add (KCID, mastery);
							}
						}
					}


					if (jsonNode ["Data"] ["student_proficiency_constants"] != null) {
						proficiencyConstants.slipUp = jsonNode ["Data"] ["student_proficiency_constants"] ["slip_up"].AsFloat;
						proficiencyConstants.guess = jsonNode ["Data"] ["student_proficiency_constants"] ["guess"].AsFloat;
						proficiencyConstants.initial = jsonNode ["Data"] ["student_proficiency_constants"] ["initial"].AsFloat;
						proficiencyConstants.learntWhileSolving = jsonNode ["Data"] ["student_proficiency_constants"] ["learnt_while_solving"].AsFloat;
					}
				}
			}
		}

		public void UpdateKCProficiencyConstants(JSONNode proficiency)
		{
			Debug.Log ("Update KC proficiency constants");
			string fileName = Application.persistentDataPath + "/KCsMasteryLatest.txt";
			JSONNode jsonNode;
			if (File.Exists (fileName)) {
				string json = File.ReadAllText (fileName);
				jsonNode = JSONNode.Parse (json);
				if (jsonNode != null) {
					jsonNode ["Data"] ["student_proficiency_constants"] = proficiency;
				}

			} else {
				jsonNode = JSONClass.Parse ("{\"Data\"}");
				jsonNode["Data"] ["student_proficiency_constants"] = proficiency;
			}
			proficiencyConstants = new ProficiencyConstants ();
			proficiencyConstants.slipUp = proficiency ["slip_up"].AsFloat;
			proficiencyConstants.guess = proficiency ["guess"].AsFloat;
			proficiencyConstants.initial = proficiency ["initial"].AsFloat;
			proficiencyConstants.learntWhileSolving = proficiency ["learnt_while_solving"].AsFloat;

			File.WriteAllText (fileName,jsonNode.ToString());

		}

		public void UpdateKCMastery()
		{
			string fileName = Application.persistentDataPath + "/KCsMasteryLatest.txt";
			JSONNode jsonNode;
			if (File.Exists (fileName))
			{
				string json = File.ReadAllText (fileName);
				jsonNode = JSONNode.Parse (json);
				JSONNode masteryData = JSONNode.Parse ("[]");
				foreach(var KCMastery in mKCMastery)
				{
			    	JSONNode data = JSONNode.Parse ("{}");
					data ["id"] = KCMastery.Key.ToString();
					data["mastery"]= KCMastery.Value.ToString();
					masteryData.Add (data);
				}
				jsonNode ["Data"] ["Mastery"] = masteryData;
				File.WriteAllText (fileName, jsonNode.ToString ());
			}
		}

		public void LoadPracticeItems ()
		{
			string fileName = Application.persistentDataPath + "/PracticeItemNames.txt";
			if (File.Exists (fileName)) 
			{
				string json = File.ReadAllText (fileName);

				JSONNode jsonNode = JSONNode.Parse (json);
				int length = jsonNode.Count;

				for (int i = 0; i < length; i++) 
				{
					string practiceItemId = jsonNode [i] ["practice_id"].Value;
					string practiceItemName = jsonNode [i] ["practice_item_name"].Value;

					if (!mPracticeItemNames.ContainsKey (practiceItemId)) 
					{
						mPracticeItemNames.Add (practiceItemId, practiceItemName);
					}
				}
			}
		   fileName = Application.persistentDataPath + "/PracticeItemCoins.txt";
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

			fileName = Application.persistentDataPath + "/PracticeItemRegeneration.txt";
			if (File.Exists (fileName)) 
			{
				mRegenerationData.Clear ();
				var sr = File.OpenText (fileName);
				string json = sr.ReadToEnd ();
				JSONNode jsonNode = JSONNode.Parse (json);
				if (jsonNode != null) 
				{
					int length = jsonNode.Count;
					for (int i = 0; i < length; i++)
					{
						string practiceID = jsonNode [i] ["PracticeId"].Value;
						string regenerationDate = jsonNode [i] ["RegenerationDate"].Value;
						if (mRegenerationData.ContainsKey (practiceID)) {
							mRegenerationData [practiceID] = regenerationDate;
						} else {
							mRegenerationData.Add (practiceID, regenerationDate);
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
						int cnt = 1;
						for (int j = 0; j < KClength; j++)
						{
							KnowledgeComponent KC = new KnowledgeComponent ();
							KC.PracticeID = pItem.PracticeID;
							KC.ID = KCdata[j] ["id"].Value;
							KC.KCName = KCdata[j] ["name"].Value;
							KC.TotalCoins = KCdata[j] ["coins"].AsInt;
							KC.CurrentCoins = 0;

							if (mKCCoins.ContainsKey (KC.ID)) 
							{
								if (mKCCoins [KC.ID] > KC.TotalCoins)
									mKCCoins [KC.ID] = KC.TotalCoins;
								
								KC.CurrentCoins = mKCCoins [KC.ID];
							}
							pItem.TotalCoins += KC.TotalCoins;
							pItem.CurrentCoins += KC.CurrentCoins;
							KC.Mappings = new List<KCQuestion>();
						
							JSONNode MappingData = KCdata[j]["question_types"];
							int Mappinglength = MappingData.Count;
							for(int k=0; k<Mappinglength; k++)
							{
								
								KCQuestion kcQuestion = new KCQuestion ();
								kcQuestion.practiceCodeId = MappingData [k] ["practice_item_id"].Value;
								if(mPracticeItemNames.ContainsKey(kcQuestion.practiceCodeId ))
								{
									kcQuestion.difficulty = MappingData [k] ["difficulty"].AsInt;
									kcQuestion.subLevel = MappingData [k] ["sub_level"].AsInt;
									kcQuestion.practiceName = mPracticeItemNames [kcQuestion.practiceCodeId];
									if (!KC.Mappings.Contains (kcQuestion)) 
									{
										KC.Mappings.Add (kcQuestion);
									}
								}

							}
							if (!pItem.KnowledgeComponents.ContainsKey (KC.ID) && KC.Mappings.Count>0) 
							{
								KC.Index = cnt;
								cnt++;
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
				sr.Close ();
			}
			PracticeRegeneration ();
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

		public void LoadExplanationsJSON ()
		{
			if (!mUseJSON) {
				LoadExplanations ();
				return;
			}

			string fileName = Application.persistentDataPath + "/ExplanationsJSON.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!IsJsonValidDirtyCheck (data)) {
					LoadExplanations ();
					return;
				}
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					Explanation ex = new Explanation ();
					ex.PracticeItemID = N ["Data"] [i] ["practice_item_id"].Value;
					ex.Level = N ["Data"] [i] ["difficulty"].Value;
					ex.SubLevel = N ["Data"] [i] ["sub_level"].Value;
					ex.URL = N ["Data"] [i] ["explanation_url"].Value;
					string key = ex.PracticeItemID + "L" + ex.Level + ex.SubLevel;
					mExplanation.Add (key, ex);
				}
			}
		}

		public void PracticeRegeneration()
		{
			foreach (var pItem in mPracticeItems) 
			{
				if (mRegenerationData.ContainsKey (pItem.Value.PracticeID)) 
				{
					System.DateTime timestarted = System.DateTime.ParseExact (mRegenerationData [pItem.Value.PracticeID], "yyyyMMddHHmmss", null);
					System.DateTime timeNow = System.DateTime.Now;
					System.TimeSpan differenceTime = timeNow.Subtract (timestarted);
					float diff = (float)differenceTime.TotalSeconds;
					float secondsForRegeneration = float.Parse (pItem.Value.RegenRate) * 60 * 60 * 24;
					if (diff < secondsForRegeneration) {
						continue;
					}
					bool isFinishedKCS = IsFinishedCertainKCS (pItem.Value);
					if (isFinishedKCS) {
						ResetCoinsAfterRegenration (pItem.Value);
						mRegenerationData [pItem.Value.PracticeID] = DateTime.Now.ToString ("yyyyMMddHHmmss");
					}
				}
				else if(pItem.Value.Show == "true" || CerebroHelper.isTestUser())
				{
					mRegenerationData [pItem.Value.PracticeID] = DateTime.Now.ToString ("yyyyMMddHHmmss");
					ResetCoinsAfterRegenration (pItem.Value);
				}
			}

			UpdateKCCoinsData ();
			UpdateRegenerationData ();
		}

		public void UpdateRegenerationData()
		{
			string fileName = Application.persistentDataPath + "/PracticeItemRegeneration.txt";
			JSONNode jsonNode = JSONNode.Parse ("[]");
			foreach(var reg in mRegenerationData)
			{
				JSONNode data = JSONNode.Parse ("{}");
				data ["PracticeId"] = reg.Key.ToString();
				data["RegenerationDate"]= reg.Value.ToString();
				jsonNode.Add (data);
			}

			File.WriteAllText (fileName, jsonNode.ToString ());
		}

		public void ResetCoinsAfterRegenration(PracticeItems pItem)
		{
			pItem.CurrentCoins = 0;
			foreach (var KC in pItem.KnowledgeComponents)
			{
				KC.Value.CurrentCoins = 0;
				if(mKCCoins.ContainsKey(KC.Value.ID))
				{
					mKCCoins [KC.Value.ID] = 0;
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
			foreach(var KCCoin in mKCCoins)
			{
				JSONNode data = JSONNode.Parse ("{}");
				data ["ID"] = KCCoin.Key.ToString();
				data["Coins"]= KCCoin.Value.ToString();
				jsonNode.Add (data);
			}

			File.WriteAllText (fileName, jsonNode.ToString ());
		}

		public void GotPraticeItemNames()
		{
			GetPracticeItems ();
		}
			
		public void GotPracticeItems ()
		{		
			mTableLoaded [(int)Cerebro.TableTypes.tPracticeItems] = true;	
			LoadPracticeItems ();
		}

		public void GetPracticeItemNames()
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.GetPracticeItemsName ();
			}  
		}

		public void GetPracticeItems ()
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.GetPracticeItems ();
			}  
		}

		public void GotKCMastery()
		{
			mTableLoaded [(int)Cerebro.TableTypes.tKCMasetry] = true;
			LoadKCMastery ();
		}

		public void GetKCMastery()
		{
			if (mHitServer) {
				HTTPRequestHelper.instance.GetKCMastery ();
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
			CheckForSubmittedImageResponsesJSON (mDescribeImage.ImageID);
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

		public void WriteImageResponseToFileJSON (string userResponse)
		{
			if (!mUseJSON) {
				WriteImageResponseToFile (userResponse);
				return;
			}

			string fileName = Application.persistentDataPath + "/DescribeImageSubmittedJSON.txt";
			if (!File.Exists (fileName))
				return;
			
			int cnt = 0;
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {				
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				cnt = N ["Data"].Count;
				File.WriteAllText (fileName, string.Empty);
			}
			N ["Data"] [cnt] ["ImageID"] = mDescribeImage.ImageID;
			N ["Data"] [cnt] ["MediaType"] = mDescribeImage.MediaType;
			N ["Data"] [cnt] ["MediaURL"] = mDescribeImage.MediaURL;
			N ["Data"] [cnt] ["PromptText"] = mDescribeImage.PromptText;
			N ["Data"] [cnt] ["SubPromptText"] = mDescribeImage.SubPromptText;
			N ["Data"] [cnt] ["UserResponse"] = userResponse;
			N ["Data"] [cnt] ["UserSubmitted"] = "true";
			File.WriteAllText (fileName, N.ToString());
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

		public Dictionary<string,string> GetNextImageIDJSON ()
		{
			string fetchNewDate = "false";
			Dictionary<string,string> returnDict = new Dictionary<string,string> ();

			if (!mUseJSON) {
				returnDict = GetNextImageID ();
				return returnDict;
			}

			string fileName = Application.persistentDataPath + "/LastImageIDJSON.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!IsJsonValidDirtyCheck (data)) {
					return null;
				}
				JSONNode N = JSONClass.Parse (data);
				if (N ["Data"] ["ImageID"] == null || N ["Data"] ["Date"] == null) {
					returnDict.Add ("imageID", "1");
					returnDict.Add ("fetchBool", "true");
					return returnDict;
				}
				
				string imgId = N ["Data"] ["ImageID"].Value;
				string date = N ["Data"] ["Date"].Value;

				System.DateTime imageSubmittedDate = System.DateTime.ParseExact (date, "yyyyMMdd", null);
				string today = System.DateTime.Now.ToString ("yyyyMMdd");
				System.DateTime todayDate = System.DateTime.ParseExact (today, "yyyyMMdd", null);

				if (todayDate > imageSubmittedDate) {
					fetchNewDate = "true";
				}

				returnDict.Add ("imageID", imgId);
				returnDict.Add ("fetchBool", fetchNewDate);

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

		public void SetLastImageIDJSON (string imageID, string date)
		{
			if (!mUseJSON) {
				SetLastImageID (imageID, date);
				return;
			}

			string fileName = Application.persistentDataPath + "/LastImageIDJSON.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!IsJsonValidDirtyCheck (data)) {
					return;
				}
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				File.WriteAllText (fileName, string.Empty);
				N ["Data"] ["ImageID"] = imageID;
				N ["Data"] ["Date"] = date;
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (fileName, N.ToString());
			}
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

		public void ConvertDescribeImageSubmittedToJSON()
		{
			string fileName = Application.persistentDataPath + "/DescribeImageSubmitted.txt";
			string targetFileName = Application.persistentDataPath + "/DescribeImageSubmittedJSON.txt";
			if (File.Exists (targetFileName)) {
				string data = File.ReadAllText (targetFileName);
				if (IsJsonValidDirtyCheck (data)) {
					return;
				}
			}
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				int cnt = 0;
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				string imageID = "";
				string mediaType = "";
				string mediaURL = "";
				string promptText = "";
				string subPromptText = "";
				string response = "";
				while (line != null) {
					var lineArr = line.Split ("," [0]);
					if (lineArr.Length > 5) {
						if (imageID.Length > 0) {
							N ["Data"] [cnt] ["ImageID"] = imageID;
							N ["Data"] [cnt] ["MediaType"] = mediaType;
							N ["Data"] [cnt] ["MediaURL"] = mediaURL;
							N ["Data"] [cnt] ["PromptText"] = promptText;
							N ["Data"] [cnt] ["SubPromptText"] = subPromptText;
							N ["Data"] [cnt] ["UserResponse"] = response;
							N ["Data"] [cnt] ["UserSubmitted"] = "true";
							cnt++;	
						}

						imageID = lineArr [0];
						mediaType = lineArr [1];
						mediaURL = lineArr [2];
						promptText = lineArr [3];
						subPromptText = lineArr [4];
						response = lineArr [5];
						if (lineArr.Length > 6) {
							for (var i = 6; i < lineArr.Length; i++) {
								response += "," + lineArr [i];
							}
						}
					} else {
						response += Environment.NewLine+line;
					}
					line = sr.ReadLine ();									
				}
				sr.Close ();
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (targetFileName, N.ToString());
			}
		}

		public bool CheckForSubmittedImageResponsesJSON (string ImageID)
		{
			if (!mUseJSON) {
				bool toReturn = CheckForSubmittedImageResponses (ImageID);
				return toReturn;
			}

			string fileName = Application.persistentDataPath + "/DescribeImageSubmittedJSON.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!IsJsonValidDirtyCheck (data)) {
					return false;
				}
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					if (N ["Data"][i]["ImageID"].Value == ImageID) {
						if (mDescribeImage == null) {
							mDescribeImage = new DescribeImage ();
						}
						mDescribeImage.ImageID = ImageID;
						mDescribeImage.MediaType = N ["Data"] [i] ["MediaType"].Value;
						mDescribeImage.MediaURL = N ["Data"] [i] ["MediaURL"].Value;
						mDescribeImage.PromptText = N ["Data"] [i] ["PromptText"].Value;
						mDescribeImage.SubPromptText = N ["Data"] [i] ["SubPromptText"].Value;
						mDescribeImage.UserResponse = N ["Data"] [i] ["UserResponse"].Value;
						mDescribeImage.UserSubmitted = true;
						return true;
					}
				}
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

		public Dictionary<string,string> GetNextVerbalizeIDJSON ()
		{
			string fetchNewDate = "false";
			Dictionary<string,string> returnDict = new Dictionary<string,string> ();

			if (!mUseJSON) {
				returnDict = GetNextVerbalizeID ();
				return returnDict;
			}

			string fileName = Application.persistentDataPath + "/LastVerbalizeIDJSON.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!IsJsonValidDirtyCheck (data)) {
					return null;
				}
				JSONNode N = JSONClass.Parse (data);
				if (N ["Data"] ["VerbalizeID"] == null || N ["Data"] ["Date"] == null) {
					returnDict.Add ("VerbalizeID", "1");
					returnDict.Add ("fetchBoolVerb", "true");
					return returnDict;
				}

				string imgId = N ["Data"] ["VerbalizeID"].Value;
				string date = N ["Data"] ["Date"].Value;

				System.DateTime imageSubmittedDate = System.DateTime.ParseExact (date, "yyyyMMdd", null);
				string today = System.DateTime.Now.ToString ("yyyyMMdd");
				System.DateTime todayDate = System.DateTime.ParseExact (today, "yyyyMMdd", null);

				if (todayDate > imageSubmittedDate) {
					fetchNewDate = "true";
				}

				returnDict.Add ("VerbalizeID", imgId);
				returnDict.Add ("fetchBoolVerb", fetchNewDate);

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

		public void SetLastVerbalizeIDJSON (string verbalizeID, string date)
		{
			if (!mUseJSON) {
				SetLastVerbalizeID (verbalizeID, date);
				return;
			}

			string fileName = Application.persistentDataPath + "/LastVerbalizeIDJSON.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!IsJsonValidDirtyCheck (data)) {
					return;
				}
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				File.WriteAllText (fileName, string.Empty);
				N ["Data"] ["Verbalize"] = verbalizeID;
				N ["Data"] ["Date"] = date;
				N ["VersionNumber"] = VersionData;
				File.WriteAllText (fileName, N.ToString());
			}
		}

		public void DeleteVerbalize(string VerbID)
		{
			string fileName = Application.persistentDataPath + "/VerbalizeSubmitted.txt";
			int cnt = 0;
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			JSONNode N1 = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {				
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				cnt = N ["Data"].Count;
				int currId = CheckForSubmittedVerbalize (VerbID);
				if (currId > -1) {
					File.WriteAllText (fileName, string.Empty);
					int myCnt = 0;
					for (int i = 0; i < cnt; i++) {
						if (i == currId) {
							continue;
						}
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
					N1 ["VersionNumber"] = N ["VersionNumber"].Value;
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
			string fileName = Application.persistentDataPath + "/VerbalizeSubmitted.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					if (N ["Data"][i]["UserSubmitted"].AsBool && !N ["Data"][i]["UploadedToServer"].AsBool) {
						Verbalize Verb = CheckForSubmittedVerbalizeViaDate (N ["Data"][i]["VerbalizeDate"].Value);
						if (UploadingVerbalize == null)
							UploadingVerbalize = new List<Verbalize> ();
						UploadingVerbalize.Add (Verb);
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

		public void CheckForFlaggedQuestionToSend ()
		{
			if (mUpdatingServerFlagged)
				return;

			string fileName = Application.persistentDataPath + "/FlaggedQuestionsJSON.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					if (!N ["Data"][i]["updatedOnServer"].AsBool) {
						mUpdatingServerFlagged = true;
						JSONNode n = N ["Data"] [i];
						HTTPRequestHelper.instance.SendFlaggedData (n ["assessmentID"].Value, n ["seed"].Value, n ["difficulty"].AsInt, n ["sublevel"].AsInt, n ["isFlagged"].AsBool, n ["FlaggedVersion"].Value, OnFlaggedSentResponse);
						break;
					}
				}
			}
		}

		private string lastIdHwResponseSent = "";

		public void CheckForHomeworkResponseToSend ()
		{
			Debug.Log ("checking for hw offline");
			string fileName = Application.persistentDataPath + "/HomeworkResponseLocalJSON.txt";
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				JSONNode N = JSONClass.Parse (data);
				int cnt = 0;
				if(N ["Data"] ["Response"] != null) {
					cnt = N ["Data"] ["Response"].Count;
					for (int i = 0; i < cnt; i++) {
						lastIdHwResponseSent = N ["Data"] ["Response"] [i] ["id"].Value;
						JSONNode res = N ["Data"] ["Response"] [i];
						HTTPRequestHelper.instance.SendHomeworkResponseWC (res ["contextId"].Value, res ["createdAt"].Value, res ["responseData"].Value, HomeworkResponseSent);
						return;
					}
				}
				if(N ["Data"] ["Comment"] != null) {
					cnt = N ["Data"] ["Comment"].Count;
					for (int i = 0; i < cnt; i++) {
						lastIdHwResponseSent = N ["Data"] ["Comment"] [i] ["id"].Value;
						JSONNode cmt = N ["Data"] ["Comment"] [i];
						Debug.Log ("found cmt "+cmt ["commentData"].Value);
						HTTPRequestHelper.instance.SendHomeworkComment (cmt ["contextId"].Value, cmt ["createdAt"].Value, cmt ["teacherId"].Value, cmt ["commentData"].Value, HomeworkResponseSent);
						return;
					}
				}
				if(N ["Data"] ["Announcement"] != null) {
					cnt = N ["Data"] ["Announcement"].Count;
					for (int i = 0; i < cnt; i++) {
						lastIdHwResponseSent = N ["Data"] ["Announcement"] [i] ["id"].Value;
						JSONNode ann = N ["Data"] ["Announcement"] [i];
						HTTPRequestHelper.instance.SendHomeworkResponseAnnouncement (ann ["contextId"].Value, ann ["createdAt"].Value, HomeworkResponseSent);
						return;
					}
				}
			}
		}

		public void HomeworkResponseSent(bool isSuccess)
		{
			Debug.Log ("homework sent "+isSuccess);
			if (isSuccess) {
				string fileName = Application.persistentDataPath + "/HomeworkResponseLocalJSON.txt";
				JSONNode N = JSONClass.Parse ("{\"Data\"}");
				JSONNode N1 = JSONClass.Parse ("{\"Data\"}");
				if (File.Exists (fileName)) {				
					string data = File.ReadAllText (fileName);
					N = JSONClass.Parse (data);
					File.WriteAllText (fileName, string.Empty);
					int myCnt = 0;
					if (N ["Data"] ["Response"] != null) {
						for (int i = 0; i < N ["Data"] ["Response"].Count; i++) {
							if (N ["Data"] ["Response"] [i] ["id"].Value != lastIdHwResponseSent) {
								N1 ["Data"] ["Response"] [myCnt] = N ["Data"] ["Response"] [i];
								myCnt++;
							}
						}
						myCnt++;
					}
					myCnt = 0;
					if (N ["Data"] ["Comment"] != null) {
						for (int i = 0; i < N ["Data"] ["Comment"].Count; i++) {
							if (N ["Data"] ["Comment"] [i] ["id"].Value != lastIdHwResponseSent) {
								N1 ["Data"] ["Comment"] [myCnt] = N ["Data"] ["Comment"] [i];
								myCnt++;
							}
						}
						myCnt++;
					}
					myCnt = 0;
					if (N ["Data"] ["Announcement"] != null) {
						for (int i = 0; i < N ["Data"] ["Announcement"].Count; i++) {
							if (N ["Data"] ["Announcement"] [i] ["id"].Value != lastIdHwResponseSent) {
								N1 ["Data"] ["Announcement"] [myCnt] = N ["Data"] ["Announcement"] [i];
								myCnt++;
							}
						}
						myCnt++;
					}
					N1 ["VersionNumber"] = N ["VersionNumber"].Value;
					File.WriteAllText (fileName, N1.ToString ());
				}
			}
		}

		public void OnFlaggedSentResponse(string assessmentID)
		{
			mUpdatingServerFlagged = false;
			if (assessmentID != "") {
				MarkAsSentOnServer (assessmentID);
			}
			CheckForFlaggedQuestionToSend ();
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

		public void WriteAvatar(Avatar avt)
		{
			string fileName = Application.persistentDataPath + "/AvatarIDs.txt";
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			N ["Data"] ["Avatar"] ["BodyID"] = avt.BodyID.ToString();
			N ["Data"] ["Avatar"] ["HeadID"] = avt.HeadID.ToString();
			N ["Data"] ["Avatar"] ["HairID"] = avt.HairID.ToString();
			N ["Data"] ["Avatar"] ["HatID"] = avt.HatID.ToString();
			N ["Data"] ["Avatar"] ["GogglesID"] = avt.GogglesID.ToString();
			N ["Data"] ["Avatar"] ["BadgeID"] = avt.BadgeID.ToString();
			N ["Data"] ["Avatar"] ["ColorId"] = avt.ColorId;
			N ["Data"] ["Avatar"] ["IsBoy"] = avt.IsBoy.ToString();
			for (int i = 0; i < avt.PurchasedHats.Count; i++) {
				N ["Data"] ["Avatar"] ["PurchasedHats"] [i] = avt.PurchasedHats [i].ToString ();
			}
			for (int i = 0; i < avt.PurchasedGoggles.Count; i++) {
				N ["Data"] ["Avatar"] ["PurchasedGoggles"] [i] = avt.PurchasedGoggles [i].ToString ();
			}
			for (int i = 0; i < avt.PurchasedBadges.Count; i++) {
				N ["Data"] ["Avatar"] ["PurchasedBadges"] [i] = avt.PurchasedBadges [i].ToString ();
			}
			File.WriteAllText (fileName, N.ToString());
		}

		public void ReadAvatar()
		{
			string fileName = Application.persistentDataPath + "/AvatarIDs.txt";
			if (!File.Exists (fileName)) {
				string BabaId = PlayerPrefs.GetString (PlayerPrefKeys.BabaID, "111");
				mAvatar = new Avatar ();
				mAvatar.HairID = int.Parse (BabaId [0].ToString ());
				mAvatar.HeadID = int.Parse (BabaId [1].ToString ());
				mAvatar.BodyID = int.Parse (BabaId [2].ToString ());
				mAvatar.HatID = -1;
				mAvatar.GogglesID = -1;
				mAvatar.BadgeID = -1;
				mAvatar.ColorId = PlayerPrefs.GetString(PlayerPrefKeys.GOTGameTeamID, "1");
				if (mAvatar.BodyID > 4) {
					mAvatar.IsBoy = false;
				} else {
					mAvatar.IsBoy = true;
				}
				mAvatar.PurchasedHats = new List<int> ();
				mAvatar.PurchasedGoggles = new List<int> ();
				mAvatar.PurchasedBadges = new List<int> ();
			} else {
				string data = File.ReadAllText (fileName);
				JSONNode N = JSONClass.Parse (data);
				mAvatar = new Avatar ();
				mAvatar.HairID = N ["Data"] ["Avatar"] ["HairID"].AsInt;
				mAvatar.BodyID = N ["Data"] ["Avatar"] ["BodyID"].AsInt;
				mAvatar.HeadID = N ["Data"] ["Avatar"] ["HeadID"].AsInt;
				mAvatar.HatID = N ["Data"] ["Avatar"] ["HatID"].AsInt;
				mAvatar.GogglesID = N ["Data"] ["Avatar"] ["GogglesID"].AsInt;
				mAvatar.BadgeID = N ["Data"] ["Avatar"] ["BadgeID"].AsInt;
				mAvatar.ColorId = N ["Data"] ["Avatar"] ["ColorId"].Value;
				mAvatar.IsBoy = N ["Data"] ["Avatar"] ["IsBoy"].AsBool;
				mAvatar.PurchasedHats = new List<int> ();
				for (int i = 0; i < N ["Data"] ["Avatar"] ["PurchasedHats"].Count; i++) {
					mAvatar.PurchasedHats.Add (N ["Data"] ["Avatar"] ["PurchasedHats"] [i].AsInt);
				}
				mAvatar.PurchasedGoggles = new List<int> ();
				for (int i = 0; i < N ["Data"] ["Avatar"] ["PurchasedGoggles"].Count; i++) {
					mAvatar.PurchasedGoggles.Add (N ["Data"] ["Avatar"] ["PurchasedGoggles"] [i].AsInt);
				}
				mAvatar.PurchasedBadges = new List<int> ();
				for (int i = 0; i < N ["Data"] ["Avatar"] ["PurchasedBadges"].Count; i++) {
					mAvatar.PurchasedBadges.Add (N ["Data"] ["Avatar"] ["PurchasedBadges"] [i].AsInt);
				}
			}
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
//			LoadExplanationsJSON ();
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
		public string ContentID { get; set; }
		public string ContentName { get; set; }
		public string ContentDate { get; set; }
		public string ContentDescription { get; set; }
		public string ContentLink { get; set; }
		public string ContentType { get; set; }
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
		public int BabaHatId{ get; set; }
		public int BabaGogglesId{ get; set; }
		public int BabaBadgeId{ get; set; }
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

	public class AssessmentItem
	{
		public string PracticeID { get; set; }
	}

	public class PracticeItems : AssessmentItem
	{
		
		public string DifficultyLevels{ get; set; }
		//public string Grade { get; set; }
		public string PracticeItemName{ get; set; }
		public string RegenRate{ get; set; }
		public int TotalCoins{ get; set; }
		public string Show { get; set; }
		public int CurrentCoins{ get; set; }
		public string RegenerationStarted { get; set; }
		public Dictionary<string ,KnowledgeComponent> KnowledgeComponents;
		public bool isKCViewOpened;
		public Color stripColor;
	}

	public class KnowledgeComponent : AssessmentItem
	{
		public int Index { get; set;}
		public string ID { get; set; }
		public string KCName { get; set; }
		public List<KCQuestion> Mappings { get; set; }
		public int TotalCoins { get; set; }
		public int CurrentCoins { get; set; }
		public int Mastery{ get; set;}
	}

	public class KCQuestion
	{
		public string practiceCodeId;
		public int difficulty;
		public int subLevel;
		public string practiceName;
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

	public class Missions //Old Mission
	{
		public string MissionID { get; set; }
		public string MissionName{ get; set; }
		public SortedDictionary<string, string> QuestionsString { get; set; }
		public string TimeStarted { get; set; }
		public SortedDictionary<string, MissionItemData> Questions { get; set; }
	}

	public class MissionItemData  //Old Mission
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
		public int CharLimit { get; set; }
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
		public string LeaderboardEndDate{ get; set; }
		public string[] GroupNames{ get; set; }
		public int[] GroupCurrScores{ get; set; }
		public int[] GroupTargetScores{ get; set; }
	}

	public class GOTLeaderboard
	{
		public string GroupName;
		public int GroupCoin;
		public string GroupID;
		public int GroupCell;
		public int GroupPoint;
	}

	public class ProficiencyConstants
	{
		public float slipUp;
		public float guess;
		public float initial;
		public float learntWhileSolving;

		public ProficiencyConstants()
		{
			slipUp = 0f;
			guess = 0f;
			initial = 0f;
			learntWhileSolving = 0f;
		}

	}


}
// namespace ends
