using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using SimpleJSON;

namespace Cerebro
{
	public class WelcomeScript : MonoBehaviour
	{
		private GameObject childrenView;
		private GameObject profileScreenView;
//		private GameObject DailyView;

		[SerializeField]
		private GameObject FeatureSection;
		[SerializeField]
		private GameObject Mission;
		[SerializeField]
		public GameObject dashboardIcon;
		[SerializeField]
		private GameObject ProfileImage;
		[SerializeField]
		private GameObject CancelButton;
		[SerializeField]
		private GameObject MainView;
		[SerializeField]
		private GameObject BaseScreen;
		[SerializeField]
		private GameObject BottomBarObject;
		[SerializeField]
		private Text CoinsText;
		[SerializeField]
		private Text NameText;
		// Use this for initialization

		GameObject Googly;

		private bool BottomScrollStart;
		private float BottomScrollTime, BottomStartPos, BottomTargetPos;

		Feature mcurrentFeature;
		bool mFetchingFeature = false;

		string profileImageFile = "";

		public bool takingScreenshots = false;
		public bool testingAllScreens = false;
		public bool autoTestMissionCorrect = false;
		public bool autoTestMissionMix = false;

		public System.Action onDashboardClicked;

		private static WelcomeScript m_Instance;
		List<string> practiceOptions;

		// for debugging feature section
		System.DateTime mFeatureDate;


		public static WelcomeScript instance {
			get {
				return m_Instance;
			}
		}

		void Awake ()
		{
			if (m_Instance != null && m_Instance != this) {
				Destroy (gameObject);
				return;
			}

			this.gameObject.name = "WelcomeScreen";

			m_Instance = this;

//			StartCoroutine (ShowDailyView());
		}

		IEnumerator ShowDailyView()
		{
			yield return new WaitForSeconds (0.1f);
			GameObject DailyView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Daily, MainView.transform);
			DailyView.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (300f, 110f);
			DailyView.transform.SetAsLastSibling ();
			DailyView.GetComponent<DailyView> ().Initialize ();
		}

		public void ProfileImagePressed ()
		{
			if (profileScreenView == null) {
				profileScreenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.ProfileScreen, transform.parent);
				profileScreenView.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			}
			profileScreenView.GetComponent<ProfileScript> ().Initialise ();
			profileScreenView.SetActive (true);
			profileScreenView.transform.SetAsLastSibling ();

			StartCoroutine (HideScreen (false));
		}

		public void DestroyPooledPrefabs() {
			if (profileScreenView != null) {
				profileScreenView.GetComponent<ProfileScript> ().DestroyTexture ();
				Destroy (profileScreenView);
				profileScreenView = null;
			}
		}

		public void AnalyticsPressed() {
			GameObject analyticsView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.AnalyticsScreen, transform.parent);
			analyticsView.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			analyticsView.transform.SetAsLastSibling ();

			StartCoroutine (HideScreen (false));
		}

		private Texture2D FetchImageFromFile (string fileName)
		{
			byte[] fileData;
			Texture2D tex = null;
			if (File.Exists (fileName)) {
				fileData = File.ReadAllBytes (fileName);
				tex = new Texture2D (1, 1);
				tex.LoadImage (fileData);
			}
			return tex;
		}

		public void ShowScreen (bool hideOption = true, string animateMissionItem = null, bool imageChanged = false)
		{
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Welcome);

			LaunchList.instance.WelcomeScreenActive = true;
			LaunchList.instance.setWifiIcon ();

			if (PlayerPrefs.HasKey (PlayerPrefKeys.nameKey)) {
				NameText.text = PlayerPrefs.GetString (PlayerPrefKeys.nameKey);
			} else {
				NameText.text = "";
			}
			if (PlayerPrefs.HasKey (PlayerPrefKeys.Coins)) {
				CoinsText.text = "Coins: " + PlayerPrefs.GetInt (PlayerPrefKeys.Coins);
			} else {
				CoinsText.text = "Coins: " + 0;
			}

			if (imageChanged) {
				StartCoroutine (SetProfileImage ());
			}

			if (CerebroProperties.instance.ShowCoins) {
				CoinsText.gameObject.SetActive (true);
			} else {
				CoinsText.gameObject.SetActive (false);
			}

			ComputeCardsData ();
			HideDashboardIcon ();

			MainView.SetActive (true);
			transform.SetAsLastSibling ();

			if (hideOption) {
				CancelButton.SetActive (true);
				MainView.transform.localPosition = new Vector2 (MainView.transform.localPosition.x, -768);
				Go.to (MainView.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (0f, 0f), false));
			} else {
				CancelButton.SetActive (false);
				MainView.transform.localPosition = new Vector2 (MainView.transform.localPosition.x, 0);
			}

			Mission.GetComponent<MissionScript> ().ShowScreen (animateMissionItem);
		}

		IEnumerator SetProfileImage() {
			yield return new WaitForSeconds (0.1f);
			Texture2D tex = FetchImageFromFile (profileImageFile);
			if (tex != null) {
				Sprite oldSprite = ProfileImage.GetComponent<Image> ().sprite;
				if (oldSprite.name != "profile-pic") {
					Destroy (oldSprite.texture);
					Destroy (oldSprite);
				}
				var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
				ProfileImage.GetComponent<Image> ().sprite = newsprite;
			}
		}

		public void ShowMissionComplete (float accuracy)
		{
			GameObject missionComplete = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.MissionComplete, transform.parent);
			missionComplete.transform.SetAsLastSibling ();
			missionComplete.GetComponent<MissionComplete> ().Initialise (accuracy);
		}

		public void UpdateMission ()
		{
			Mission.GetComponent<MissionScript> ().UpdateMission ();
		}

		IEnumerator HideScreen (bool showAnimation)
		{
			LaunchList.instance.WelcomeScreenActive = false;
			LaunchList.instance.setWifiIcon ();
			if (showAnimation) {
				Go.to (MainView.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (0f, -768f), false));
				yield return new WaitForSeconds (0.2f);
			}
			MainView.SetActive (false);
			yield return new WaitForSeconds (0.1f);
		}

		void Start ()
		{
			profileImageFile = Application.persistentDataPath + "/ProfileImage.jpg";

			Color currentColor = MainView.GetComponent<Image> ().color;
			MainView.GetComponent<Image> ().color = new Color (currentColor.r, currentColor.g, currentColor.b, 0);
			Go.to (MainView.GetComponent<Image> (), 0.3f, new GoTweenConfig ().colorProp ("color", new Color (currentColor.r, currentColor.g, currentColor.b, 1)));

			LaunchList.instance.WelcomeScreenActive = true;
			LaunchList.instance.setWifiIcon ();

			mFeatureDate = System.DateTime.Now;
			string forDate = mFeatureDate.ToString ("MMdd");
			FetchFeatureData (forDate);

			if (LaunchList.instance.mUseJSON) {
				PracticeData.PopulateFromFileJSON ();
			} else {
				PracticeData.PopulateFromFile ();
			}

			ShowScreen (false, null, true);
		}

		public void FromGenericPopup()
		{
			Debug.Log ("from generic pop up");
		}

		public void ShowRatingPopup(string type, float timeSpent, string videoID, string question, RatingPopup.OkClicked OkFunction = null) {
			GameObject ratingPopup = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.RatingPopup, transform.parent);
			ratingPopup.transform.SetAsLastSibling ();
			ratingPopup.GetComponent<RatingPopup> ().Initialise (type, timeSpent, videoID, question, OkFunction);
		}

		public void ShowGenericPopup(string question, int NumberOfButton, bool IsPortrait, GenericPopup.OkClicked OkFunction = null, GenericPopup.CancelClicked CancelFunction = null, Sprite icon = null) {
			GameObject GenericPopup = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.GenericPopup, transform.parent);
			GenericPopup.transform.SetAsLastSibling ();
			GenericPopup.GetComponent<GenericPopup> ().Initialise (question, NumberOfButton, IsPortrait, OkFunction, CancelFunction, icon);
		}

		public void RetryFeatureData ()
		{
			string forDate = mFeatureDate.ToString ("MMdd");
			FetchFeatureData (forDate);
		}

		private void ComputeCardsData ()
		{
			Text watchData = BottomBarObject.transform.Find ("Watch").Find ("Info").GetComponent<Text> ();
			Text practiceData = BottomBarObject.transform.Find ("Practice").Find ("Info").GetComponent<Text> ();
			Text wcData = BottomBarObject.transform.Find ("WC").Find ("Info").GetComponent<Text> ();
			Text revisitData = BottomBarObject.transform.Find ("Revisit").Find ("Info").GetComponent<Text> ();
			Text qotdData = BottomBarObject.transform.Find ("QOTD").Find ("Info").GetComponent<Text> ();
			Text verbData = BottomBarObject.transform.Find ("Verbalize").Find ("Info").GetComponent<Text> ();
			Text homeworkData = BottomBarObject.transform.Find ("Homework").Find ("Info").GetComponent<Text> ();

			if (CerebroHelper.isTestUser ()) {
				BottomBarObject.transform.Find ("Verbalize").gameObject.SetActive (true);
				BottomBarObject.transform.Find ("WordTower").gameObject.SetActive (true);
				BottomBarObject.transform.Find ("GOT").gameObject.SetActive (true);
				BottomBarObject.transform.Find ("Coding").gameObject.SetActive (true);
				BottomBarObject.transform.Find ("TestScreens").gameObject.SetActive (true);
				BottomBarObject.transform.Find ("TestScreens").Find ("Info").GetComponent<Text> ().text = "Screenshots " + takingScreenshots.ToString ();
			} else {
//				BottomBarObject.transform.Find ("Verbalize").gameObject.SetActive (false);
				BottomBarObject.transform.Find ("WordTower").gameObject.SetActive (false);
				//BottomBarObject.transform.Find ("GOT").gameObject.SetActive (false);
				BottomBarObject.transform.Find ("Coding").gameObject.SetActive (false);
				BottomBarObject.transform.Find ("TestScreens").gameObject.SetActive (false);
			}

			int hr = System.DateTime.Now.Hour;
			int timeLeft = (24 - hr);
			if (timeLeft == 1) {
				qotdData.text = (24 - hr).ToString () + " hour left";
			} else {
				qotdData.text = (24 - hr).ToString () + " hours left";
			}
			int revisitQuestionCount = 0;
			if(LaunchList.instance.mUseJSON) {
				revisitQuestionCount = LaunchList.instance.GetFlaggedQuestionsJSON ().Count;
			} else {
				revisitQuestionCount = LaunchList.instance.GetFlaggedQuestions ().Count;
			}
			revisitData.text = revisitQuestionCount.ToString () + " flagged questions";

			List<string> allVideos = LaunchList.instance.GetWatchList ();
			List<string> watchedVideos = GetWatchedVideosJSON ();
			foreach (var id in watchedVideos) {
				allVideos.Remove (id);
			}
			watchData.text = allVideos.Count.ToString () + " unwatched videos";

			Dictionary<string,string> imageIDDict = LaunchList.instance.GetNextImageIDJSON ();
			string imageID = imageIDDict ["imageID"];
			bool fetchBool = imageIDDict ["fetchBool"] == "true" ? true : false;
			if (LaunchList.instance.CheckForSubmittedImageResponsesJSON (imageID) && !fetchBool) {
				wcData.text = "Submitted";
			} else {
				wcData.text = "Pending";
			}

//			Dictionary<string,string> VerbalizeIDDict = LaunchList.instance.GetNextVerbalizeIDJSON ();
//			string VerbalizeID = VerbalizeIDDict ["VerbalizeID"];
//			bool fetchBoolVerb = VerbalizeIDDict ["fetchBoolVerb"] == "true" ? true : false;
//			if (LaunchList.instance.CheckForSubmittedVerbalize (VerbalizeID) > -1 && !fetchBoolVerb) {
//				verbData.text = "Submitted";
//			} else {
//				verbData.text = "Pending";
//			}
			verbData.text = "Read out aloud";

			string today = System.DateTime.Now.ToString ("yyyyMMdd");
			if (LaunchList.instance.mUseJSON) {
				Dictionary<string, int> practiceCount = GetPracticeCountJSON (today);
				if (practiceCount != null && practiceCount ["attempts"] != null) {
					practiceData.text = practiceCount ["attempts"].ToString () + " questions solved";
				}
			} else {
				practiceData.text = GetPracticeCount (today) ["attempts"].ToString () + " questions solved";
			}

			homeworkData.text = "Pending";
		}

		public Dictionary<string,int> GetPracticeCount (string date)
		{
			string fileName = Application.persistentDataPath + "/PracticeCount.txt";
			int totalAttempts = 0;
			int totalCorrect = 0;
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				while (line != null) {
					var splitArr = line.Split ("," [0]);
					if (splitArr [0] == date) {
						totalAttempts = int.Parse (splitArr [1]);
						totalCorrect = int.Parse (splitArr [2]);
					}
					line = sr.ReadLine ();
				}
				sr.Close ();
			}
			Dictionary<string,int> dict = new Dictionary<string,int> ();
			dict.Add ("attempts", totalAttempts);
			dict.Add ("correct", totalCorrect);
			return dict;
		}

		public Dictionary<string,int> GetPracticeCountJSON (string date)
		{
			Dictionary<string,int> dict = new Dictionary<string,int> ();
			if (!LaunchList.instance.mUseJSON) {
				dict = GetPracticeCount (date);
				return dict;
			}

			string fileName = Application.persistentDataPath + "/PracticeCountJSON.txt";
			if (!File.Exists (fileName))
				return null;
			
			int totalAttempts = 0;
			int totalCorrect = 0;
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
					return null;
				}
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					if (N ["Data"] [i] ["date"].Value == date) {
						totalAttempts = N ["Data"] [i] ["attempts"].AsInt;
						totalCorrect = N ["Data"] [i] ["correct"].AsInt;
					}
				}
			}
			dict.Add ("attempts", totalAttempts);
			dict.Add ("correct", totalCorrect);
			return dict;
		}

		public Dictionary<string,int> GetPracticeQuestionsData ()
		{
			string fileName = Application.persistentDataPath + "/PracticeData.txt";
			int totalAttempts = 0;
			int totalCorrect = 0;
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;
				while (line != null) {
					splitArr = line.Split ("," [0]);
					totalAttempts += int.Parse (splitArr [1]);
					totalCorrect += int.Parse (splitArr [2]);
					line = sr.ReadLine ();
				}

				sr.Close ();
			}
			Dictionary<string,int> dict = new Dictionary<string,int> ();
			dict.Add ("attempts", totalAttempts);
			dict.Add ("correct", totalCorrect);
			return dict;

		}

		public Dictionary<string,int> GetPracticeQuestionsDataJSON ()
		{
			Dictionary<string,int> dict = new Dictionary<string,int> ();
			if (!LaunchList.instance.mUseJSON) {
				dict = GetPracticeQuestionsData ();
				return dict;
			}

			string fileName = Application.persistentDataPath + "/PracticeDataJSON.txt";
			if (!File.Exists (fileName))
				return null;
			
			int totalAttempts = 0;
			int totalCorrect = 0;
			if (File.Exists (fileName)) {
				string data = File.ReadAllText (fileName);
				if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
					return null;
				}
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					totalAttempts += N ["Data"] [i] ["attempts"].AsInt;
					totalCorrect += N ["Data"] [i] ["correct"].AsInt;
				}
			}
			dict.Add ("attempts", totalAttempts);
			dict.Add ("correct", totalCorrect);
			return dict;
		}

		public List<string> GetWatchedVideos ()
		{
			List<string> watchedVideos = new List<string> ();
			string fileName = Application.persistentDataPath + "/WatchedVideos.txt";
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				if (line != null && line != "") {
					watchedVideos.Add (line);
				}
				while (line != null) {
					line = sr.ReadLine ();
					if (line != null && line != "") {
						watchedVideos.Add (line);
					}
				}  
				sr.Close ();
			} else {
				CerebroHelper.DebugLog ("Could not Open the file: " + fileName + " for reading.");
			}
			return watchedVideos;
		}

		public List<string> GetWatchedVideosJSON ()
		{
			List<string> watchedVideos = new List<string> ();
			if (!LaunchList.instance.mUseJSON) {
				watchedVideos = GetWatchedVideos ();
				return watchedVideos;
			}
				
			string fileName = Application.persistentDataPath + "/WatchedVideosJSON.txt";
			if (!File.Exists (fileName))
				return watchedVideos;

			string data = File.ReadAllText (fileName);
			if (LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				JSONNode N = JSONClass.Parse (data);
				for (int i = 0; i < N ["Data"].Count; i++) {
					watchedVideos.Add (N ["Data"] [i] ["VideoContentID"].Value);
				}
			}
			return watchedVideos;
		}

		public void FetchFeatureData (string forDate)
		{
			mFetchingFeature = true;
			FeatureSection.transform.Find ("VideoIcon").gameObject.SetActive (false);
			FeatureSection.transform.Find ("BottomBar").gameObject.SetActive (false);
			FeatureSection.transform.Find ("FG").gameObject.SetActive (false);
			FeatureSection.transform.Find ("FeatureText").gameObject.SetActive (false);
			FeatureSection.transform.Find ("ErrorView").gameObject.SetActive (false);
			FeatureSection.transform.Find ("DisplayImage").Find ("Icon").GetComponent<Graphic> ().color = new Color (1, 1, 1, 0);
			FeatureSection.transform.Find ("DisplayImage").Find ("ProgressCircle").gameObject.SetActive (true);
			LaunchList.instance.GetFeatureForDate (forDate, GotFeature);
			if (CerebroHelper.isTestUser()) {
				if (PlayerPrefs.HasKey (PlayerPrefKeys.nameKey)) {
					NameText.text = PlayerPrefs.GetString (PlayerPrefKeys.nameKey);
				} else {
					NameText.text = "";
				}
				NameText.text += " " + forDate;
			}
		}

		int GotFeature (Feature featureDetails)
		{
			mFetchingFeature = false;
			if (FeatureSection == null) {
				print ("tried to access feature section but it was null");
				return -1;
			}
			FeatureSection.transform.Find ("FG").gameObject.SetActive (true);
			if (featureDetails != null) {
				mcurrentFeature = featureDetails;
				if (featureDetails.Type == "Image") {
					if (featureDetails.MediaText != "") {
						FeatureSection.transform.Find ("BottomBar").gameObject.SetActive (true);
						FeatureSection.transform.Find ("BottomBar").Find ("Text").GetComponent<Text> ().text = featureDetails.MediaText;
					}
					StartCoroutine (LoadImage (featureDetails.MediaUrl, FeatureSection.transform.Find ("DisplayImage").gameObject));	
				} else if (featureDetails.Type == "Video") {
					FeatureSection.transform.Find ("VideoIcon").gameObject.SetActive (true);
					FeatureSection.transform.Find ("BottomBar").gameObject.SetActive (true);
					FeatureSection.transform.Find ("BottomBar").Find ("Text").GetComponent<Text> ().text = featureDetails.MediaText;
					StartCoroutine (LoadImage (featureDetails.ImageUrl, FeatureSection.transform.Find ("DisplayImage").gameObject));	
				} else if (featureDetails.Type == "Text") {
					FeatureSection.transform.Find ("FeatureText").gameObject.SetActive (true);
					FeatureSection.transform.Find ("FeatureText").GetComponent<Text> ().text = featureDetails.MediaText;
					FeatureSection.transform.Find ("DisplayImage").Find ("ProgressCircle").gameObject.SetActive (false);
				}
			} else {
				FeatureSection.transform.Find ("ErrorView").gameObject.SetActive (true);
				FeatureSection.transform.Find ("DisplayImage").Find ("ProgressCircle").gameObject.SetActive (false);
			}
			return 1;
		}

		public void VideoIconPressed ()
		{
			VideoHelper.instance.OpenVideoWithUrl (mcurrentFeature.MediaUrl);
		}

		IEnumerator LoadImage (string imgurl, GameObject go)
		{
			Graphic graphic = go.transform.Find ("Icon").GetComponent<Graphic> ();
			go.transform.Find ("ProgressCircle").gameObject.SetActive (true);

			Texture2D tex = null;
			CerebroHelper.DebugLog (imgurl);
			if (CerebroHelper.remoteQuizTextures.ContainsKey (imgurl)) {
				tex = CerebroHelper.remoteQuizTextures [imgurl];
				yield return new WaitForSeconds (0.2f);
			} else {
				WWW remoteImage = new WWW (imgurl);
				yield return remoteImage;
				if (remoteImage.error == null) {
					tex = remoteImage.texture;
					if (CerebroHelper.remoteQuizTextures.ContainsKey (imgurl)) {
						CerebroHelper.remoteQuizTextures[imgurl] =  tex;
					} else {
						CerebroHelper.remoteQuizTextures.Add (imgurl, tex);
					}
				}
			}
			if (tex != null) {
				go.transform.Find ("ProgressCircle").gameObject.SetActive (false);
				float holderWidth = graphic.gameObject.GetComponent<RectTransform> ().sizeDelta.x;//600f;
				float holderHeight = graphic.gameObject.GetComponent<RectTransform> ().sizeDelta.y;//416f;

				float holderAspectRatio = holderWidth / holderHeight;
				float imageAspectRatio = (float)tex.width / (float)tex.height;

				float scaleRatio = 1;
				CerebroHelper.DebugLog (holderAspectRatio + "," + imageAspectRatio);
				if (holderAspectRatio >= imageAspectRatio) {			// scale to match width
					float actualImageWidth = tex.width * (holderHeight / tex.height);
					scaleRatio = holderWidth / actualImageWidth;
				} else {
					float actualImageHeight = tex.height * (holderWidth / tex.width);
					scaleRatio = holderHeight / actualImageHeight;
				}


				var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
				graphic.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
				graphic.GetComponent<Image> ().sprite = newsprite;
				graphic.GetComponent<RectTransform> ().localScale = new Vector3 (scaleRatio, scaleRatio, 1f);
			} else {
				go.transform.Find ("ProgressCircle").gameObject.SetActive (false);
				StartCoroutine (LoadImage (imgurl, go));
			}
		}

		public void CancelPressed ()
		{
			ShowDashboardIcon ();
			StartCoroutine (HideScreen (true));	
		}

		public void TestScreens ()
		{						// To Open all the screens for 2 seconds as a test
			testingAllScreens = true;
			practiceOptions = new List<string> ();
			Object[] assets = Resources.LoadAll ("Assessments/");
			foreach (Object asset in assets) {
				practiceOptions.Add (StringHelper.CreateSpacesFromCamelCase (asset.name));
			}
			GoToNextChapter();
			//StartCoroutine (OpenMultipleScreens ());
		}

		public void OpenScreen (string screens, Mission mission = null)
		{
			CerebroHelper.DebugLog ("opening");	
			// screens string is a comma-separated string of the path to follow to get to the final string. e.g. - to get to Sets Assessment, the string should be "Practice,Sets"
			RemoveScreens ();

			string[] screenArr = screens.Split ("," [0]);

//			childrenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.StudentView, BaseScreen.transform);
//			childrenView.GetComponent<CerebroTestScript> ().ForceOpenScreen (screenArr, 0, missionItemData);

			ShowDashboardIcon ();

			if (screenArr [0] == "Practice") {
				childrenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.ChooseAssessments, BaseScreen.transform);
				if (screenArr.Length > 1) {
					childrenView.GetComponent<CerebroTestScript> ().ForceOpenScreen (screenArr, 1, mission);
				}
			} else if (screenArr [0] == "Watch") {
				childrenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.StudentPlaylistContainer, BaseScreen.transform);
				if (screenArr.Length > 1) {
					childrenView.GetComponent<CerebroTestScript> ().ForceOpenScreen (screenArr, 1, mission);
				}
			} else if (screenArr [0] == "Revisit") {
				childrenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Revisit, BaseScreen.transform);
			} else if (screenArr [0] == "QOTD") {
				childrenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.QOTDLandingPage, BaseScreen.transform);
			} else if (screenArr [0] == "DescribeImage") {
				HideDashboardIcon ();
				childrenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.WCLandingPage, BaseScreen.transform);
			} else if (screenArr [0] == "Play") {
				LaunchList.instance.LoadGame ();
			} else if (screenArr [0] == "Verbalize") {
				childrenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.VerbalizeLandingPage, BaseScreen.transform);
			} else if (screenArr [0] == "Homework") {
				childrenView = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.HomeworkContainer, BaseScreen.transform);
			} 

			StartCoroutine (HideScreen (false));
		}

		IEnumerator OpenMultipleScreens ()
		{
			
			string[] options = new string[]{ "Practice", "Watch", "Revisit", "QOTD" };
			List<string> subOptions = new List<string> ();
			Object[] assets = Resources.LoadAll ("Assessments/");
			foreach (Object asset in assets) {
				subOptions.Add (StringHelper.CreateSpacesFromCamelCase (asset.name));
			}
			for (var i = 0; i < options.Length; i++) {
				OpenScreen (options [i]);
				yield return new WaitForSeconds (2.0f);
			}
			for (var i = 0; i < subOptions.Count; i++) {
				CerebroHelper.DebugLog ("Opening " + subOptions [i]);
				OpenScreen (options [0] + "," + subOptions [i]);
				yield return new WaitForSeconds (2.0f);
			}
		}

		int currScreen = 0;
		public string currChapter = "";
		public void NextChapter()
		{
//			if (currScreen == 0) {
//				currScreen = practiceOptions.Count - 1;
//			}
			if (currScreen < practiceOptions.Count) {
				OpenScreen ("Practice," + practiceOptions [currScreen]);
				currChapter = practiceOptions [currScreen];
				currScreen++;
			} else {
				StartCoroutine (GetOtherScreens());
			}
		}

		IEnumerator GetOtherScreens()
		{
			yield return new WaitForSeconds (1f);
			OpenScreen ("Watch");
			yield return new WaitForSeconds (1f);
			if (WelcomeScript.instance && WelcomeScript.instance.takingScreenshots) {
				Application.CaptureScreenshot (Application.persistentDataPath+ "/Screenshots/Watch.png");
			}
			yield return new WaitForSeconds (0.5f);
			OpenScreen ("Revisit");
			yield return new WaitForSeconds (1f);
			if (WelcomeScript.instance && WelcomeScript.instance.takingScreenshots) {
				Application.CaptureScreenshot (Application.persistentDataPath+ "/Screenshots/Revisit.png");
			}
			yield return new WaitForSeconds (0.5f);
			OpenScreen ("QOTD");
			yield return new WaitForSeconds (1f);
			if (WelcomeScript.instance && WelcomeScript.instance.takingScreenshots) {
				Application.CaptureScreenshot (Application.persistentDataPath+ "/Screenshots/QOTD.png");
			}
			yield return new WaitForSeconds (1f);
			testingAllScreens = false;
		}

		public void OpenWordTowerGame ()
		{
			if (WordTower.WordTowerScript.instance != null) {
				WordTower.WordTowerScript.instance.Activate ();
			} else {
				PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.WordTower, transform.parent);
			}
			StartCoroutine (HideScreen (false));
		}

		public void OpenCoding ()
		{
			PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Coding, BaseScreen.transform);
			StartCoroutine (HideScreen (false));
		}

		public void OpenFeedback ()
		{
			PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Feedback, BaseScreen.transform);
			StartCoroutine (HideScreen (false));
		}

		public void OpenMissions ()
		{
			Mission.GetComponent<MissionScript> ().ShowScreen ();
		}

		public void OpenPractice ()
		{
			OpenScreen ("Practice");
		}

		public void OpenWatch ()
		{
			OpenScreen ("Watch");
		}

		public void OpenPlay ()
		{
			LaunchList.instance.LoadGame ();
		}

		public void OpenRevisit ()
		{
			OpenScreen ("Revisit");
		}

		public void OpenDailyQuiz ()
		{
			OpenScreen ("QOTD");
		}

		public void OpenVerbalize ()
		{
			OpenScreen ("Verbalize");
		}

		public void OpenHomework ()
		{
			OpenScreen ("Homework");
		}

		public void LeftButtonBottomBarPressed()
		{
			if (BottomScrollStart)
				return;
			
			BottomStartPos = BottomBarObject.transform.parent.GetComponent<ScrollRect> ().normalizedPosition.x;
			BottomTargetPos = 0f;
			BottomScrollStart = true;
			BottomScrollTime = Time.time;
		}

		public void RightButtonBottomBarPressed()
		{
			if (BottomScrollStart)
				return;
			
			BottomStartPos = BottomBarObject.transform.parent.GetComponent<ScrollRect> ().normalizedPosition.x;
			BottomTargetPos = 1f;
			BottomScrollStart = true;
			BottomScrollTime = Time.time;
		}

		public void OpenGoogly (MissionItemData missionItemData)
		{
			Googly = PrefabManager.InstantiateGameObject (ResourcePrefabs.Googly, transform.parent);
			Googly.GetComponent<GooglyScript> ().Initialize (missionItemData);
			StartCoroutine (HideScreen (false));
		}

		public void RemoveScreens ()
		{
			if (Googly != null) {
				Destroy (Googly);
				Googly = null;
			}
			for (var i = BaseScreen.transform.childCount - 1; i >= 0; i--) {
				Destroy (BaseScreen.transform.GetChild (i).gameObject);
			}
		}

		public void OpenEnglishAssessment ()
		{
			OpenScreen ("DescribeImage");
		}

		public void ShowDashboardIcon ()
		{
			dashboardIcon.transform.SetAsLastSibling ();
			dashboardIcon.SetActive (true);
		}

		public void HideDashboardIcon ()
		{
			dashboardIcon.SetActive (false);
		}

		public void DashboardPressed ()
		{
			if (onDashboardClicked != null) {
				onDashboardClicked.Invoke ();
			}
			WelcomeScript.instance.ShowScreen ();
		}

		[DllImport ("__Internal")]
		private static extern void _BackButton (
			string message);

		public void GoToNextChapter()
		{
			NextChapter ();
		}

		// used for saving video prompter recorded videos on s3
		public void GetSavedVideoPath(string path)
		{
			print ("got back from native with path "+path);
			LaunchList.instance.mVerbalize.UserResponseURL = path;
			LaunchList.instance.WriteVerbalizeResponseToFile (LaunchList.instance.mVerbalize);
			LaunchList.instance.VerbalizeSaving = false;
			GameObject VerbLandPage = GameObject.FindGameObjectWithTag("VerbalizeLandingPage");
			if(VerbLandPage != null)
			{
				if (LaunchList.instance.mVerbalize != null) {
					System.DateTime dt = System.DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbalizeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
					VerbLandPage.GetComponent<VerbalizeLandingPage>().ManageCardDataForDate (dt);
				}
			}
		}

		void Update ()
		{
//			if (Input.GetMouseButton (0) && CerebroHelper.isTestUser()) {
//				
//				Vector2 pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
//				//CerebroHelper.DebugLog (pos.x);
//				if (pos.x < 0 && !mFetchingFeature) {						
//					mFeatureDate = mFeatureDate.AddDays (-1);
//					string forDate = mFeatureDate.ToString ("yyyy-MM-dd");
//					//CerebroHelper.DebugLog ("KJFDJKDFJKFDKJFDJFKD FETCHING NEXT FEATURE " + forDate);
//					//FetchFeatureData (forDate);
//				} else if (pos.x > 0 && !mFetchingFeature) {					
//					mFeatureDate = mFeatureDate.AddDays (1);
//					string forDate = mFeatureDate.ToString ("yyyy-MM-dd");
//					//CerebroHelper.DebugLog ("KJFDJKDFJKFDKJFDJFKD FETCHING PREVIOUS FEATURE " + forDate);
//					//FetchFeatureData (forDate);					
//				} else {
//					//CerebroHelper.DebugLog ("relax, looks like we are fetching a feature");
//				}
//			}
			if (Input.GetKeyDown (KeyCode.S)) {
				takingScreenshots = !takingScreenshots;
				BottomBarObject.transform.Find ("TestScreens").Find ("Info").GetComponent<Text> ().text = "Screenshots " + takingScreenshots.ToString ();
			}

			if (BottomScrollStart) {
				if (Time.time - BottomScrollTime <= 0.25f) {
					BottomBarObject.transform.parent.GetComponent<ScrollRect> ().normalizedPosition = new Vector2 (Mathf.Lerp (BottomStartPos, BottomTargetPos, (Time.time - BottomScrollTime) / 0.25f), 0);
				} else {
					BottomBarObject.transform.parent.GetComponent<ScrollRect> ().normalizedPosition = new Vector2 (BottomTargetPos, 0);
					BottomScrollStart = false;
				}
			}
		}

		System.DateTime OldestDate = System.DateTime.ParseExact ("20160412", "yyyyMMdd", null);
		System.DateTime TodayDate = System.DateTime.Now;
		public void FeaturePreviousPressed()
		{
			if (!mFetchingFeature && (mFeatureDate.ToString ("yyyyMMdd") != OldestDate.ToString ("yyyyMMdd") || CerebroHelper.isTestUser ())) {
				mFeatureDate = mFeatureDate.AddDays (-1);
				string forDate = mFeatureDate.ToString ("MMdd");
				//CerebroHelper.DebugLog ("KJFDJKDFJKFDKJFDJFKD FETCHING NEXT FEATURE " + forDate);
				FetchFeatureData (forDate);
			}
		}

		public void FeatureNextPressed()
		{
			if (!mFetchingFeature && (mFeatureDate.ToString ("yyyyMMdd") != TodayDate.ToString ("yyyyMMdd") || CerebroHelper.isTestUser ())) {
				mFeatureDate = mFeatureDate.AddDays (1);
				string forDate = mFeatureDate.ToString ("MMdd");
				//CerebroHelper.DebugLog ("KJFDJKDFJKFDKJFDJFKD FETCHING PREVIOUS FEATURE " + forDate);
				FetchFeatureData (forDate);
			}
		}

		public void RefreshFeature() {
			mFeatureDate = System.DateTime.Now;
			string forDate = mFeatureDate.ToString ("MMdd");
			FetchFeatureData (forDate);
		}
			
	}
}
