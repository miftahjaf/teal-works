using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MaterialUI;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;

namespace Cerebro {

	public static class ResourcePrefabs
	{
		public const string StudentList = "StudentList";
		public const string PasswordDialog = "PasswordDialog";
		public const string SettingsDialog = "SettingsDialog";
		public const string VersionDialog = "VersionDialog";
		public const string StudentView = "StudentView";
		public const string WelcomeScreen = "WelcomeScreen";
		public const string Missions = "Missions/Missions";
		public const string DescribeImage = "DescribeImage";
		public const string MissionComplete = "MissionComplete";
		public const string CameraPrefab = "CameraPrefab";
		public const string QOTD = "QOTD";
		public const string QOTDLandingPage = "QOTDLandingPage";
		public const string WCLandingPage = "WCLandingPage";
		public const string VerbalizeLandingPage = "VerbalizeLandingPage";
		public const string QOTDLeaderboard = "QOTDLeaderboard";
		public const string StudentPlaylistContainer = "StudentPlaylistContainer";
		public const string Revisit = "Revisit";
		public const string Googly = "Googly";
		public const string StudentPlaylist = "StudentPlaylist";
		public const string Achievements = "Achievements";
		public const string ChooseAssessments = "ChooseAssessments";
		public const string ChooseLearnContent = "ChooseLearnContent";
		public const string Assessments = "Assessment";
		public const string Separator = "Line";
		public const string ProfileScreen = "ProfileScreen";
		public const string AnalyticsScreen = "AnalyticsScreen";
		public const string Coding = "Coding";
		public const string Feedback = "Feedback";
		public const string Verbalize = "Verbalize";
		public const string RatingPopup = "RatingPopup";
		public const string GenericPopup = "GenericPopup";
		public const string Daily = "Daily";

		public const string WordTower = "WordTower/WordTower";
		public const string WordTowerMovableComponent = "WordTower/Movable";
		public const string WordTowerStaticComponent = "WordTower/Static";
		public const string Group1Player = "Game/Unit1";
		public const string Group2Player = "Game/Unit2";
		public const string Group3Player = "Game/Unit3";
		public const string Group4Player = "Game/Unit4";
		public const string GOTGame = "Game/GOT";
	}

	public static class PlayerPrefKeys 
	{
		public const string nameKey = "Name";
		public const string IDKey = "ID";
		public const string GradeKey = "Grade";
		public const string SectionKey = "Section";
		public const string Coins = "Coins";
		public const string DeltaCoins = "DeltaCoins";
		public const string MissionID = "MissionIDKey";
		public const string ProfilePicKey = "ProfilePic";
		public const string TextureLastCleared = "TextureLastCleared";
		public const string VerbalizeSpeed = "VerbalizeSpeed";
		public const string BabaID = "BabaID";
		public const string GOTGameTeamID = "GOTGameTeamID";
		public const string IsVersionUpdated = "IsVersionUpdated";
	}

	public static class PrefabManager
	{
		private static readonly List<GameObject> m_Prefabs = new List<GameObject>();
		private static readonly List<string> m_Names = new List<string>();

		public static class ResourcePrefabs
		{
			public const string progressIndicatorCircular = "Progress Indicators/Circle Progress Indicator";
			public const string progressIndicatorLinear = "Progress Indicators/Linear Progress Indicator";

			public const string dialogAlert = "Dialogs/DialogAlert";
			public const string dialogProgress = "Dialogs/DialogProgress";
			public const string dialogSimpleList = "Dialogs/DialogSimpleList";
			public const string ListOption = "Dialogs/ListContent";
			public const string dialogCheckboxList = "Dialogs/DialogCheckboxList";
			public const string dialogRadioList = "Dialogs/DialogRadioList";
			public const string dialogTimePicker = "Dialogs/Pickers/DialogTimePicker";
			public const string dialogDatePicker = "Dialogs/Pickers/DialogDatePicker";

			public const string disabledPanel = "DisabledPanel";
			public const string sliderDot = "SliderDot";
			public const string dropdownPanel = "Menus/Dropdown Panel";

			public const string snackbar = "Snackbar";
			public const string toast = "Toast";
		}

		public static GameObject GetGameObject(string nameWithPath)
		{
			GameObject gameObject = null;

			if (!m_Names.Contains(nameWithPath))
			{
				gameObject = Resources.Load<GameObject>(nameWithPath);

				if (gameObject != null)
				{
					m_Prefabs.Add(gameObject);
					m_Names.Add(nameWithPath);
				}
			}
			else
			{
				for (int i = 0; i < m_Prefabs.Count; i++)
				{
					if (m_Names[i] == nameWithPath)
					{
						if (m_Prefabs[i] != null)
						{
							gameObject = m_Prefabs[i];
						}
					}
				}
			}

			return gameObject;
		}

		public static GameObject InstantiateGameObject(string nameWithPath, Transform parent)
		{
			GameObject go = GetGameObject(nameWithPath);

			if (go == null)
			{
				return null;
			}

			go = GameObject.Instantiate(go);

			if (parent == null)
			{
				return go;
			}

			go.transform.SetParent(parent);
			go.transform.localScale = Vector3.one;
			go.transform.localEulerAngles = Vector3.zero;
			go.transform.localPosition = Vector3.zero;

			return go;
		}
	}

	public class CerebroScript : MonoBehaviour {

		private float timeElapsed = 0f;
		private float activeTimeElapsed = 0f;
		private bool isAppActive = true;

		private PasswordDialogScript passwordsDialog;
		private SettingsDialogScript settingsDialog;

		private GameObject DialogScreen;
		private GameObject mainView;
		private GameObject SplashScreen;

		private GameObject BaseScreen;
		public bool waitForMainScreen = true;
		public bool showNextScreen = false;

		private static CerebroScript m_Instance;
		public static CerebroScript instance
		{
			get
			{
				return m_Instance;
			}
		}

		void Awake()
		{
			if (m_Instance != null && m_Instance != this)
			{
				m_Instance.DialogScreen = GameObject.Find ("DialogScreen");
				Destroy(gameObject);
				return;
			}

			if (CerebroProperties.instance == null) {
				CerebroProperties.instance = new CerebroProperties ();
			}

			m_Instance = this;
			Application.targetFrameRate = 60;
			DontDestroyOnLoad (transform.gameObject);
		}

		// Use this for initialization
		void Start () {

			SplashScreen = GameObject.Find ("SplashScreen").gameObject;
			BaseScreen = GameObject.Find ("BaseScreen").gameObject;

			Graphic line = SplashScreen.transform.Find ("Line").GetComponent<Image> ();
			line.color = new Color (1, 1, 1, 0);

			StartCoroutine (ShowSplashAnimation ());
			StartCoroutine (ShowDummyScreen ());

			LaunchList.instance.HandleAllTablesLoaded += AllTablesLoaded;
			DialogScreen = GameObject.Find ("DialogScreen");

			Application.targetFrameRate = 60;

			if (!PlayerPrefs.HasKey (PlayerPrefKeys.GradeKey)) {
				PlayerPrefs.SetString (PlayerPrefKeys.GradeKey, "6");
			}

			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				correctPassword ();
			}
//			SetCerebroProperties ();

//			AllTablesLoaded (null, null);
		}

		public void SetCerebroProperties() {
			CerebroHelper.DebugLog ("SETTING CEREBRO PROPERTIES");
			Dictionary<string,string> properties = GetPropertiesFromFile ();
			foreach (var item in properties) {
				var property = CerebroProperties.instance.GetType ().GetProperty (item.Key);
				if (property == null) {
					continue;
				}
				var valueType = property.PropertyType.Name;
				if (valueType == "Single") {
					CerebroProperties.instance.GetType ().GetProperty (item.Key).SetValue (CerebroProperties.instance, float.Parse(item.Value), null);
				} else if (valueType == "Int32") {
					CerebroProperties.instance.GetType ().GetProperty (item.Key).SetValue (CerebroProperties.instance, int.Parse(item.Value), null);
				} else if (valueType == "String") {
					CerebroProperties.instance.GetType ().GetProperty (item.Key).SetValue (CerebroProperties.instance, item.Value, null);
				} else if (valueType == "Boolean") {
					if (item.Value == "true") {
						CerebroProperties.instance.GetType ().GetProperty (item.Key).SetValue (CerebroProperties.instance, true, null);
					} else {
						CerebroProperties.instance.GetType ().GetProperty (item.Key).SetValue (CerebroProperties.instance, false, null);
					}
				}
			}
//			CerebroProperties.instance.ShowCoins = true;
		}

		Dictionary<string,string> GetPropertiesFromFile() {
			string fileName = Application.persistentDataPath + "/PropertiesJSON.txt";

			Dictionary<string,string> properties = new Dictionary<string,string> ();

			if(File.Exists(fileName)){
				string data = File.ReadAllText (fileName);
				if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
					return null;
				}
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					properties.Add (N["Data"][i]["property_name"].Value, N["Data"][i]["property_value"].Value);
				}
			}
			return properties;	
		}

		IEnumerator ShowSplashAnimation() {
			Text splashText = SplashScreen.transform.Find ("Text").GetComponent<Text> ();
			GameObject progressBar = SplashScreen.transform.Find ("ProgressCircle").gameObject;
			progressBar.transform.localScale = new Vector3 (0, 0, 0);
			splashText.color = new Color (1,1,1,0);
			yield return new WaitForSeconds (1);
			Go.to( splashText, 1f, new GoTweenConfig().colorProp( "color", new Color(1,1,1,1)));
			if (progressBar != null) {
				progressBar.SetActive (true);
			}
			yield return new WaitForSeconds (2);
			waitForMainScreen = false;
			if (showNextScreen) {
				ShowNextScreen ();
				showNextScreen = false;
			}
		}

		private void ShowNextScreen() {
			SplashScreen = GameObject.Find ("SplashScreen").gameObject;
			BaseScreen = GameObject.Find ("BaseScreen").gameObject;
			StartCoroutine (AnimateStudentScreen ());
		}

		public void AllTablesLoaded(object sender, System.EventArgs e) {
			if (GameObject.Find ("SplashScreen") == null) {
				return;
			}
			if (!waitForMainScreen) {
				ShowNextScreen ();
			} else {
				showNextScreen = true;
			}
		}

		IEnumerator AnimateStudentScreen() {
			
			GameObject splashText = SplashScreen.transform.Find ("Text").gameObject;
			GameObject progressBar = SplashScreen.transform.Find ("ProgressCircle").gameObject;
			Go.to( splashText.transform, 0.2f, new GoTweenConfig().scale( new Vector3( 0, 0, 0 )));

			yield return new WaitForSeconds (0.2f);

			SplashScreen.SetActive (false);
			LaunchScreen ();
		}

		IEnumerator ShowDummyScreen() {
			GameObject dummyObject = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Assessments, BaseScreen.transform.parent.transform);
			dummyObject.transform.SetAsLastSibling ();
			dummyObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			dummyObject.GetComponent<AssessmentScript> ().Initialize ("Assessments/SimplifyExpressions6", "Simplify Expressions6", BaseScreen.gameObject);
			dummyObject.transform.SetAsFirstSibling ();
			yield return new WaitForSeconds (1);
			Destroy (dummyObject);
		}
			
		public void showPasswordDialog() {
			if (passwordsDialog == null && settingsDialog == null) {
				passwordsDialog = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.PasswordDialog, DialogScreen.gameObject.transform).GetComponent<PasswordDialogScript> ();
				passwordsDialog.Initialize (this);
				DialogScreen.transform.SetAsLastSibling ();
			}
		}

		public void RemoveVersionDialog()
		{
			GameObject gTemp = GameObject.Find ("VersionDialog");
			if (gTemp) {
				Destroy (gTemp);
			}
			DialogScreen.transform.SetAsFirstSibling ();
		}

		public void showVersionDialog() {
			GameObject versionUpdate = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.VersionDialog, DialogScreen.gameObject.transform);
			versionUpdate.name = "VersionDialog";
			versionUpdate.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			versionUpdate.GetComponent<RectTransform> ().position = new Vector3 (0f, 0f);
			DialogScreen.transform.SetAsLastSibling ();
		}

		public void HidePasswordDialog() {
			Destroy (passwordsDialog.gameObject);
			passwordsDialog = null;
			DialogScreen.transform.SetAsFirstSibling ();
		}

		public void HideSettingsDialog() {
			Destroy (settingsDialog.gameObject);
			settingsDialog = null;
			DialogScreen.transform.SetAsFirstSibling ();
		}

		public void correctPassword() {
			if (passwordsDialog != null) {
				HidePasswordDialog ();
			}
			settingsDialog = PrefabManager.InstantiateGameObject(Cerebro.ResourcePrefabs.SettingsDialog, DialogScreen.gameObject.transform).GetComponent<SettingsDialogScript>();
			settingsDialog.Initialize(this);
			DialogScreen.transform.SetAsLastSibling ();
		}

		public void updateData(string emailID) {
			HideSettingsDialog ();
			LaunchList.instance.GetUserProfile (emailID, GotProfile);
		}
		
		int GotProfile(StudentProfile studentProfile) {
			if (studentProfile != null) {
				LaunchList.instance.LogoutUser ();
				LaunchList.instance.CheckIfFileJSON ();

				CerebroHelper.DebugLog ("id "+studentProfile.StudentID);
				PlayerPrefs.SetString (PlayerPrefKeys.nameKey, studentProfile.FirstName + " " + studentProfile.LastName);
				PlayerPrefs.SetString (PlayerPrefKeys.IDKey, studentProfile.StudentID);
				PlayerPrefs.SetString (PlayerPrefKeys.GradeKey, studentProfile.Grade);
				PlayerPrefs.SetString (PlayerPrefKeys.SectionKey, studentProfile.Section);
				RestartApp ();
				return 1;
			}
			correctPassword ();
			return 0;
		}

		public void RestartApp() {
			if (mainView != null) {
				Destroy (mainView);
			}
			LaunchList.instance.ScanTables ();
			LaunchList.instance.mhasInternet = true;
			LaunchList.instance.setWifiIcon ();
			LaunchList.instance.CheckVersionNumber ();

			SplashScreen.SetActive (true);
			GameObject splashText = SplashScreen.transform.Find ("Text").gameObject;
			GameObject progressBar = SplashScreen.transform.Find ("ProgressCircle").gameObject;
			progressBar.transform.localScale = new Vector3 (1, 1, 0);
			splashText.transform.localScale = new Vector3 (1, 1, 0);
		}

		private void LaunchScreen() {
			if (mainView != null) {
				Destroy (mainView);
			}
			mainView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.WelcomeScreen, BaseScreen.transform);
			// 0,0 because the rectTransform has stretch-stretch behaviour

			mainView.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);				

//			if (isTeacher) {
//				mainView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.TeacherView, BaseScreen.transform);
//				mainView.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
//			} else {
//				mainView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.StudentView, BaseScreen.transform);
//				mainView.GetComponent<RectTransform> ().sizeDelta = new Vector2(0f, 0f);
//			}
		}

		void showTimeAlert() {
			AlertScreenScript.instance.Initialise ("You have been using Cerebro for a long time now! Time to take a break!");
		}
		void Update() {
			timeElapsed += Time.deltaTime;
			if (timeElapsed >= CerebroProperties.instance.PollActiveTimeAlert) {
				if (isAppActive) {
					activeTimeElapsed += timeElapsed;
					timeElapsed = 0;
				} else {
					activeTimeElapsed = 0;
					timeElapsed = 0;
				}
			}
			if (activeTimeElapsed >= CerebroProperties.instance.MaxActiveTimeAlert) {
				showTimeAlert ();
				activeTimeElapsed = 0f;
			}
		}

		void OnApplicationFocus( bool focusStatus )
		{
			if(!focusStatus)
			{
				CerebroAnalytics.instance.SessionEnded ();
				isAppActive = false;
				timeElapsed = 0;
				activeTimeElapsed = 0f;
				CerebroHelper.DebugLog ("Going to background");
			}
			else
			{
				CerebroAnalytics.instance.SessionStarted ();
				isAppActive = true;
				if (WelcomeScript.instance != null) {
					WelcomeScript.instance.RefreshFeature ();
				}

				if (PlayerPrefs.HasKey (PlayerPrefKeys.TextureLastCleared)) {
					string lastTime = PlayerPrefs.GetString (PlayerPrefKeys.TextureLastCleared);
					System.DateTime compareTo = System.DateTime.ParseExact (lastTime, "yyyyMMddHHmmss", null);
					System.DateTime now = System.DateTime.Now;
					System.TimeSpan diff = now - compareTo;  
					if (diff.Hours > 24) {									// XXX better check would be to clear based on texture memory size
						foreach (var item in CerebroHelper.remoteQuizTextures) {
							Destroy (item.Value);
						}
						CerebroHelper.remoteQuizTextures.Clear ();
						PlayerPrefs.SetString (PlayerPrefKeys.TextureLastCleared, now.ToString ("yyyyMMddHHmmss"));
					}
				} else {
					System.DateTime now = System.DateTime.Now;
					PlayerPrefs.SetString (PlayerPrefKeys.TextureLastCleared, now.ToString ("yyyyMMddHHmmss"));
				}

				SendUsageAnalytics ();

				StartCoroutine (WaitToCheckVerbalize());
			}
		}

		IEnumerator WaitToCheckVerbalize()
		{
			yield return new WaitForSeconds (15);
			LaunchList.instance.CheckForVerbalizeToUpload ();
			LaunchList.instance.CheckForFlaggedQuestionToSend ();
		}

		public void SendUsageAnalytics() {
			if (PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				string studentid = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
				List<Telemetry> listToSend = CerebroAnalytics.instance.GetNextLogsToSendJSON ();
				if(listToSend != null && listToSend.Count != 0) {
					HTTPRequestHelper.instance.SendTelemetry (studentid, listToSend);
				}
			}
		}
	}

}
