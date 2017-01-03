using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MaterialUI;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine.UI.ProceduralImage;

namespace Cerebro
{
	public class AssessmentScript : MonoBehaviour
	{

		public AudioClip wrongSound;
		public AudioClip rightSound;

		private AudioSource audioSource;

		private string timeStarted;
		private float timeini;
		private float timeend;
		private float timetaken;

		public GameObject scoreIncrement;
		public GameObject bonusScoreIncrement;
		public Text coinsText;
		public Text title;
		public Text levelUp;

		public string mPracticeName;
		public string mPracticeID;

		private float originalY = 0;
		private int incrementScore = 0;
		private int currentScore = 0;

		private float updateCntr = 0f;

		private int currentStreak = 0;
		public Image[] streakImages;
		public Text bonusText;

		public GameObject chooseAssessment;
		public BaseAssessment baseAssessment;

		private int incrementBy = 1;
		public Text testingText;
		public Button regenButton;
		public bool shouldRegenQuestion;

		private Mission mission;
		private string currentAssessmentType;
		private Dictionary<string, string> missionQuestionData;
		private Dictionary<string, string> kcQuestionData;

		private string currentQuestionMapping ="1t1";

		public bool isMissionCompleted = false;

		public Slider masterySlider;
		public Text backButtonText;

		private List<KCQuestion> KCMappings;

		private int currentMappingIndex ;
		public string currenKCMapping;

		public string practiceItemID = "";
		public string KCID = "";
		protected string CurrentLevelSeed;

		private bool testMode = false;

		public KCQuestion kcQuestion;



		// Use this for initialization
		void Start ()
		{
			audioSource = GetComponent<AudioSource> ();
			timeini = 0;
			timeend = 0;
			timetaken = 0;

			levelUp.gameObject.SetActive (false);

			originalY = scoreIncrement.transform.position.y - 100;

			scoreIncrement.transform.position = new Vector3 (scoreIncrement.transform.position.x, originalY, scoreIncrement.transform.position.z);
			scoreIncrement.GetComponent<Text> ().color = new Color (1, 1, 1, 0);

			bonusScoreIncrement.GetComponent<Text> ().color = new Color (1, 1, 1, 0);

			currentScore = LaunchList.instance.mCurrentStudent.Coins;
			coinsText.text = "Coins: " + currentScore.ToString ();
			bonusText.text = "x1";

			if (CerebroProperties.instance.ShowCoins) {
				coinsText.gameObject.SetActive (true);
			} else {
				coinsText.gameObject.SetActive (false);
			}
			currentAssessmentType = "";
		}

		public void Initialize (string assessmentType, string _title, GameObject _chooseAssessment, bool testMode = false,string _practiceItemID="", string _KCID ="", string _practiceCodeId ="")
		{
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			chooseAssessment = _chooseAssessment;

			mPracticeName = _title;
			mPracticeName = mPracticeName.Trim ();
			string backButtonTitle = "Practice";
			practiceItemID = _practiceItemID;
			if (!_KCID.Equals (""))
			{
				if (LaunchList.instance.mKCMastery.ContainsKey (KCID)) {
					UpdateMasterySlider (LaunchList.instance.mKCMastery [KCID]);
				} else {
					UpdateMasterySlider (0);
				}
				title.text = "";
				if(LaunchList.instance.mPracticeItems.ContainsKey(practiceItemID) && LaunchList.instance.mPracticeItems [practiceItemID].KnowledgeComponents.ContainsKey(KCID))
				{
					backButtonTitle = LaunchList.instance.mPracticeItems [practiceItemID].KnowledgeComponents [KCID].KCName;
				}
			}
			else 
			{
				title.text = StringHelper.RemoveNumbers (_title);
				masterySlider.gameObject.SetActive (false);
			}
				

			if (testMode && backButtonTitle.Length > 10) {
				backButtonTitle = backButtonTitle.Substring (0, 10) +"...";
			} else if (backButtonTitle.Length > 30) {
				backButtonTitle = backButtonTitle.Substring (0, 30) +"...";
			}

			backButtonText.text = backButtonTitle;

			GameObject gameobject = PrefabManager.InstantiateGameObject (assessmentType, gameObject.transform);
			gameobject.transform.SetAsFirstSibling ();
			transform.Find ("Texture").SetAsFirstSibling ();
			gameobject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 708f);
			gameobject.GetComponent<RectTransform> ().position = new Vector3 (0, 0);
			baseAssessment = gameobject.GetComponent<BaseAssessment> ();
			baseAssessment.assessmentName = _title.Replace (" ", "");
			baseAssessment.isMissionQuestion = (mission!=null);
			baseAssessment.missionQuestionData = missionQuestionData;
			baseAssessment.practiceCodeID = _practiceCodeId;
			baseAssessment.KCID = KCID;
			baseAssessment.testMode = testMode;
			baseAssessment.kcQuestionData = kcQuestionData;

			
			if (testMode) {
				testingText.gameObject.SetActive (true);
				regenButton.gameObject.SetActive (!WelcomeScript.instance.testingAllScreens);
			} else {
				testingText.gameObject.SetActive (false);
				regenButton.gameObject.SetActive (false);
			}
			if (WelcomeScript.instance && WelcomeScript.instance.testingAllScreens) {
				StartCoroutine (startChapters ());
			}
			currentAssessmentType = assessmentType;
		}

		public void Initialize(GameObject _chooseAssessment,Mission _missionData)
		{
			mission = _missionData;
			chooseAssessment = _chooseAssessment;
			UpdateNextQuestion ();

		}

		public void Initialize( GameObject _chooseAssessment,string _practiceItemID, string _KCID, bool _testMode)
		{
			practiceItemID = _practiceItemID;
			KCID = _KCID;
			testMode = _testMode;
			chooseAssessment = _chooseAssessment;
			Debug.Log ("Init practice Id "+practiceItemID + " KC ID " + KCID);
			if (KCMappings == null)
			{
				LoadKnowledgeComponentMappings ();
			}

			UpdateKCNextQuestion ();
		}

		public void UpdateNextQuestion()
		{
			if (mission == null) {
				return;
			}
			if (mission.IsMissionCompleted ()) {
				OnMissionCompletion ();
				return;
			}
			MissionQuestion missionNextQuestion = mission.GetNextQuestion ();
			missionQuestionData = new Dictionary<string, string> ();
			missionQuestionData.Add ("difficulty", missionNextQuestion.difficulty.ToString());
			missionQuestionData.Add ("sublevel", missionNextQuestion.subLevel.ToString());
			string prefabName = missionNextQuestion.practiceName.Replace (" ", "");
			string assessmentType = "Assessments/" +prefabName;

			if (baseAssessment != null && currentAssessmentType.Equals (assessmentType)) {
				baseAssessment.isMissionQuestion = (mission!=null);
				baseAssessment.missionQuestionData = missionQuestionData;
				baseAssessment.practiceCodeID = missionNextQuestion.practiceItemID;
				baseAssessment.ShowNextQuestionWithAnimation ();
			} else {
				if (baseAssessment != null) {
					baseAssessment.isDestroyed = true;
					Destroy (baseAssessment.gameObject);
				}
				Initialize (assessmentType, missionNextQuestion.practiceName, chooseAssessment,false, missionNextQuestion.practiceItemID, "", missionNextQuestion.practiceItemID);
			}

			currentAssessmentType = assessmentType;
		}

		public void UpdateKCNextQuestion()
		{

			KCQuestion kcQuestion = GetKCMapping();
			string prefabName =  kcQuestion.practiceName.Replace (" ", "");
			string assessmentType = "Assessments/" +prefabName;
			kcQuestionData = new Dictionary<string, string> ();
			kcQuestionData.Add ("difficulty", kcQuestion.difficulty.ToString());
			kcQuestionData.Add ("sublevel", kcQuestion.subLevel.ToString());
			if (baseAssessment != null && currentAssessmentType.Equals (assessmentType)) {
				baseAssessment.kcQuestionData = kcQuestionData;
				baseAssessment.practiceCodeID = kcQuestion.practiceCodeId;
				baseAssessment.ShowNextQuestionWithAnimation ();
			} else {
				if (baseAssessment != null) {
					baseAssessment.isDestroyed = true;
					Destroy (baseAssessment.gameObject);
				}
				Initialize (assessmentType, kcQuestion.practiceName, chooseAssessment, testMode,practiceItemID, KCID, kcQuestion.practiceCodeId);
			}

			currentAssessmentType = assessmentType;
		}

		public void OnMissionCompletion()
		{
			if (mission == null) {
				mission = LaunchList.instance.missionData.GetLastCompletedMission ();
			}
			if (mission == null) {
				return;
			}
			LaunchList.instance.missionData.SaveData ();
			WelcomeScript.instance.ShowScreen (true, mission.missionText);
		}
			
		public void QuestionStarted ()
		{
			timeini = Time.realtimeSinceStartup;
			timeStarted = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");
		}

		public void QuestionEnded (bool isCorrect, int difficulty, int _increment, string assessKey, int randomSeed, int sublevel, string UserAnswer = "")
		{
			AddToPracticeCountJSON (isCorrect);
			timeend = Time.realtimeSinceStartup;
			timetaken = timeend - timeini;
			string day = System.DateTime.Now.ToUniversalTime().ToString ("yyyyMMdd");

			//string practiceID = mPracticeID;
			string practiceCodeID = baseAssessment.GetPracticeCodeID ();

			if (LaunchList.instance.mUseJSON) {
				PracticeData.UpdateLocalFileJSON (practiceCodeID, isCorrect);
			} else {
				PracticeData.UpdateLocalFile (practiceCodeID, isCorrect);
			}
			//List<string> missionQuestionIds = CheckMissions (isCorrect, difficulty, sublevel, practiceID);  //Old Mission

			int increment = 0;
			string KCID = GetCurrentKCID ();

			if (isCorrect) {
				if (rightSound != null) {
					audioSource.PlayOneShot (rightSound);
				}
				var bonus = increaseStreak ();
				increment = _increment + bonus;


				if (CerebroProperties.instance.ShowCoins) {
					increment = UpdatePracticeItems (practiceItemID, KCID , increment);
				} else {
					increment = 0;
				}

				if (increment >0) {
					incrementScore = increment;
					incrementBy = Mathf.FloorToInt (increment / 5);

					if (CerebroProperties.instance.ShowCoins) {
						StartCoroutine (animateScoreIncrement (_increment));
						//UpdatePracticeItems (baseAssessment.GetPracticeItemID (), increment);
					}
					LaunchList.instance.SetCoins ( increment);
				}
			} else {
				if (wrongSound != null) {
					audioSource.PlayOneShot (wrongSound);
				}
				decreseStreak ();

				// Uncomment to give negative for wrong answers.
//				var increment = -5;
//				var studentID = Cerebro.LaunchList.instance.mGame.StudentID;
//				var groupID = Cerebro.LaunchList.instance.mGame.GroupID;
//				StartCoroutine (animateScoreIncrement (increment));
//				incrementScore = increment;
//				incrementBy = Mathf.FloorToInt(increment / 5);
//				LaunchList.instance.SetCoins (groupID, studentID, increment);
			}

			Cerebro.LaunchList.instance.WriteAnalyticsToFileJSON (assessKey, difficulty, isCorrect, day, timeStarted, Mathf.FloorToInt (timetaken), "0", randomSeed, " ", UserAnswer, increment);  
			UpdateKCMastery (practiceItemID, KCID, isCorrect);

			isMissionCompleted = LaunchList.instance.missionData.CheckAndUpdateMissionData (practiceCodeID, difficulty, sublevel, isCorrect, randomSeed, UserAnswer);
		}
			
		int increaseStreak ()
		{
			currentStreak++;

			for (var i = 0; i < streakImages.Length; i++) {
				if (i < currentStreak % 5) {
					streakImages [i].sprite = Resources.Load <Sprite> ("Images/Round");
				} else {
					streakImages [i].sprite = Resources.Load <Sprite> ("Images/Ring");
				}
			}

			if (currentStreak % 5 == 0) {
				var bonus = (currentStreak / 5) * CerebroProperties.instance.StreakMultiplier;
				bonusText.text = "x" + ((currentStreak / 5) + 1);
				StartCoroutine (animateBonusScoreIncrement (bonus));
				return bonus;
			}

			return 0;
		}

		void decreseStreak ()
		{
			if (currentStreak != 0) {
				Go.to (streakImages [0].gameObject.transform.parent, 0.3f, new GoTweenConfig ().shake (new Vector3 (10, 0, 0), GoShakeType.Position));
			}
			currentStreak = 0;
			bonusText.text = "x" + ((currentStreak / 5) + 1);
			for (var i = 0; i < streakImages.Length; i++) {
				streakImages [i].sprite = Resources.Load <Sprite> ("Images/Ring");
			}
		}

		IEnumerator animateScoreIncrement (int increment)
		{
			scoreIncrement.GetComponent<Text> ().color = new Color (0, 0, 0, 1);
			if (increment < 0) {
				scoreIncrement.GetComponent<Text> ().text = " " + increment;
			} else {
				scoreIncrement.GetComponent<Text> ().text = " + " + increment;
			}
			Go.to (scoreIncrement.transform, 0.5f, new GoTweenConfig ().position (new Vector3 (0, 100, 0), true));
			Go.to (scoreIncrement.GetComponent<Text> (), 0.5f, new GoTweenConfig ().colorProp ("color", new Color (1, 1, 1, 0)));
			yield return new WaitForSeconds (1f);
			scoreIncrement.transform.position = new Vector3 (scoreIncrement.transform.position.x, originalY, scoreIncrement.transform.position.z);
		}

		IEnumerator animateBonusScoreIncrement (int increment)
		{
			bonusScoreIncrement.GetComponent<Text> ().color = new Color (1, 1, 1, 1);
			bonusScoreIncrement.GetComponent<Text> ().text = " + " + increment;
			Go.to (bonusScoreIncrement.transform, 0.5f, new GoTweenConfig ().position (new Vector3 (0, 100, 0), true));
			Go.to (bonusScoreIncrement.GetComponent<Text> (), 0.5f, new GoTweenConfig ().colorProp ("color", new Color (1, 1, 1, 0)));
			yield return new WaitForSeconds (0.5f);
			Go.to (bonusScoreIncrement.transform, 0.1f, new GoTweenConfig ().position (new Vector3 (0, -100, 0), true));
		}

		public string GetTimeText ()
		{
			return "Time Taken: " + Mathf.Round (timetaken).ToString () + " Secs";
		}

		public void BackPressed ()
		{
			baseAssessment.SetSeedToLocalDB ();
			chooseAssessment.gameObject.SetActive (true);
			chooseAssessment.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			chooseAssessment.GetComponent<ChooseAssessmentScript> ().BackOnScreen ();

			Destroy (gameObject);
		}

		public void AnimateLevelUp ()
		{
			StartCoroutine (AnimateLevelUpStart ());
		}

		IEnumerator AnimateLevelUpStart ()
		{
			levelUp.gameObject.SetActive (true);
			Go.to (levelUp.gameObject.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector3 (0, 384, 0), true).setEaseType (GoEaseType.BackOut));
			yield return new WaitForSeconds (1.0f);
			Go.to (levelUp.gameObject.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector3 (0, -384, 0), true).setEaseType (GoEaseType.BackIn));
			yield return new WaitForSeconds (0.4f);
			levelUp.gameObject.SetActive (false);
		}



		public int UpdatePracticeItems(string practiceID,string KCID,int increment)
		{
			if(string.IsNullOrEmpty(practiceID) || string.IsNullOrEmpty (KCID))
			{
				return 0;
			}
			if (LaunchList.instance.mPracticeItems.ContainsKey (practiceID)) {
				PracticeItems practiceItem = LaunchList.instance.mPracticeItems [practiceID];

				if (practiceItem.KnowledgeComponents.ContainsKey (KCID)) {
					KnowledgeComponent KCComponent = practiceItem.KnowledgeComponents [KCID];
				
					if (KCComponent.CurrentCoins + increment > KCComponent.TotalCoins) {
						increment = (KCComponent.TotalCoins - KCComponent.CurrentCoins);
					} 

					KCComponent.CurrentCoins += increment;
					practiceItem.CurrentCoins += increment;
					LaunchList.instance.mPracticeItems [practiceID] = practiceItem;

				} else
					return increment;

				LaunchList.instance.mPracticeItems [practiceID] = practiceItem;
			} else {
				return increment;
			}


			if (increment <= 0) {
				return 0;
			}

			if (!LaunchList.instance.mKCCoins.ContainsKey (KCID))
			{
				LaunchList.instance.mKCCoins.Add (KCID,increment);
			}
			else 
			{
				LaunchList.instance.mKCCoins [KCID] = LaunchList.instance.mKCCoins [KCID] + increment;
			}
			LaunchList.instance.UpdateKCCoinsData ();
			return increment;

		}

		public void UpdateKCMastery(string practiceID, string KCID, bool isCorrect)
		{
			if(string.IsNullOrEmpty(practiceID) || string.IsNullOrEmpty (KCID))
			{
				return;
			}
			float oldProficiency = 0.05f;

			if (LaunchList.instance.mKCMastery.ContainsKey (KCID)) 
			{
				oldProficiency = LaunchList.instance.mKCMastery [KCID]/100f;

				if (oldProficiency < 0.05f) 
				{
					oldProficiency = 0.05f;
				}
			}

			int newProficiency = 0;
			ProficiencyConstants proficiencyConstants = LaunchList.instance.proficiencyConstants;
			float newProficiencyValue = 0f;
			if (isCorrect) 
			{
				newProficiencyValue = (oldProficiency * (1f - proficiencyConstants.slipUp)) / ((oldProficiency * (1f - proficiencyConstants.slipUp)) + ((1f - oldProficiency) * proficiencyConstants.guess));
			} 
			else
			{
				newProficiencyValue = (oldProficiency * proficiencyConstants.slipUp) / ((oldProficiency * proficiencyConstants.slipUp) + ((1f - oldProficiency) * (1f - proficiencyConstants.guess)));
			}

			newProficiency = Mathf.RoundToInt(MathFunctions.GetRounded(newProficiencyValue + ( ( 1 - newProficiencyValue) * proficiencyConstants.learntWhileSolving ) , 3) *100f);
			if (newProficiency >= 100) 
			{
				newProficiency = 99;
			}
	
			if (newProficiency < 0) {
				newProficiency = 0;
			}

			if (LaunchList.instance.mKCMastery.ContainsKey (KCID)) {
				LaunchList.instance.mKCMastery [KCID] = newProficiency;
			} else {
				LaunchList.instance.mKCMastery.Add (KCID,newProficiency);
		
			}

			if (LaunchList.instance.mPracticeItems.ContainsKey (practiceID) &&  LaunchList.instance.mPracticeItems [practiceID].KnowledgeComponents.ContainsKey(KCID))
			{
				KnowledgeComponent KC = LaunchList.instance.mPracticeItems [practiceID].KnowledgeComponents[KCID];
				KC.Mastery = newProficiency;
			}

			LaunchList.instance.UpdateKCMastery ();
			UpdateMasterySlider (newProficiency);
		}
			
		string MaxCoinsReached (string practiceID)
		{
			PracticeItems pItem = LaunchList.instance.mPracticeItems [practiceID];
			pItem.RegenerationStarted = System.DateTime.Now.ToString ("yyyyMMddHHmmss");
			LaunchList.instance.mPracticeItems [pItem.PracticeID] = pItem;
			BackPressed ();
			return pItem.RegenerationStarted;
		}

		void AddToPracticeCount(bool isCorrect) {
			string fileName = Application.persistentDataPath + "/PracticeCount.txt";
			string today = System.DateTime.Now.ToString ("yyyyMMdd");
			List<string> writeLines = new List<string> ();
			bool foundDate = false;
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				while (line != null) {
					var splitArr = line.Split ("," [0]);

					if (splitArr [0] == today) {
						foundDate = true;
						int currentAttemptsCount = int.Parse (splitArr [1]) + 1;
						int currentCorrectCount = int.Parse (splitArr [2]);
						if (isCorrect) {
							currentCorrectCount++;
						}
						writeLines.Add (splitArr [0] + "," + currentAttemptsCount + "," + currentCorrectCount);
					} else {
						System.DateTime todayDate = System.DateTime.ParseExact (today, "yyyyMMdd",null);
						System.DateTime compareDate = System.DateTime.ParseExact (splitArr [0], "yyyyMMdd",null);
						System.TimeSpan diff = todayDate - compareDate;  
						if (diff.Days < 7) {
							writeLines.Add (line);
						}
					}
					line = sr.ReadLine ();
				}
				sr.Close ();
			} 

			if(!foundDate) {
				int currentAttemptsCount = 1;
				int currentCorrectCount = 0;
				if (isCorrect) {
					currentCorrectCount++;
				}
				writeLines.Add (today + "," + currentAttemptsCount + "," + currentCorrectCount);
			}

			var writesr = File.CreateText (fileName);
			foreach (var str in writeLines) {
				writesr.WriteLine (str);
			}
			writesr.Close();
		}

		void AddToPracticeCountJSON(bool isCorrect) {
			if (!LaunchList.instance.mUseJSON) {
				AddToPracticeCount (isCorrect);
				return;
			}

			string fileName = Application.persistentDataPath + "/PracticeCountJSON.txt";
			if (!File.Exists (fileName))
				return;

			string today = System.DateTime.Now.ToString ("yyyyMMdd");
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			JSONNode N1 = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {				
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				File.WriteAllText (fileName, string.Empty);
			}
			bool found = false;
			int myCnt = 0;
			for (int i = 0; i < N ["Data"].Count; i++) {
				if (N ["Data"] [i] ["date"].Value == today) {
					N1 ["Data"] [myCnt] ["date"] = N ["Data"] [i] ["date"].Value;
					N1 ["Data"] [myCnt] ["attempts"] = (N ["Data"] [i] ["attempts"].AsInt + 1).ToString ();
					if (isCorrect) {
						N1 ["Data"] [myCnt] ["correct"] = (N ["Data"] [i] ["correct"].AsInt + 1).ToString ();
					} else {
						N1 ["Data"] [myCnt] ["correct"] = N ["Data"] [i] ["correct"].Value;
					}
					myCnt++;
					found = true;
				} else {
					System.DateTime todayDate = System.DateTime.ParseExact (today, "yyyyMMdd",null);
					System.DateTime compareDate = System.DateTime.ParseExact (N ["Data"] [i] ["date"].Value, "yyyyMMdd",null);
					System.TimeSpan diff = todayDate - compareDate;  
					if (diff.Days < 7) {
						N1 ["Data"] [myCnt] ["date"] = N ["Data"] [i] ["date"].Value;
						N1 ["Data"] [myCnt] ["attempts"] = N ["Data"] [i] ["attempts"].Value;
						N1 ["Data"] [myCnt] ["correct"] = N ["Data"] [i] ["correct"].Value;
						myCnt++;
					}
				}
			}
			if (!found) {
				N1 ["Data"] [myCnt] ["date"] = today;
				N1 ["Data"] [myCnt] ["attempts"] = "1";
				if (isCorrect) {
					N1 ["Data"] [myCnt] ["correct"] = "1";
				} else {
					N1 ["Data"] [myCnt] ["correct"] = "0";
				}
			}
			N1 ["VersionNumber"] = N ["VersionNumber"].Value;
			File.WriteAllText (fileName, N1.ToString());
		}

		public void TestingButtonPressed() {
			if (baseAssessment != null && !shouldRegenQuestion) {
				baseAssessment.TestNextQuestion ();
			}
		}

		public void RegenButtonPressed() {
			if (baseAssessment != null) {
				shouldRegenQuestion = true;
				baseAssessment.TestNextQuestion ();
			}
		}


//		public static bool NeedNewChapter = true;
//		public static string currChapter = "";
		IEnumerator startChapters()
		{
			yield return new WaitForSeconds (1f);
			baseAssessment.testMode = true;
			TestingButtonPressed ();
			yield return new WaitForSeconds (0.5f);
			if (WelcomeScript.instance.takingScreenshots) {
				
				string dirPath = Application.persistentDataPath + "/Screenshots/" + baseAssessment.assessmentName;
				//string path = dirPath + "/" + baseAssessment.testQuestionLevel + "t" + (baseAssessment.testSelector + 1) + ".png";
				string path = dirPath + "/" + baseAssessment.currentQuestionMapping + ".png";
				if (!Directory.Exists (dirPath)) {
					Directory.CreateDirectory (dirPath);
				}
				Application.CaptureScreenshot (path);
				CerebroHelper.DebugLog ("taking "+path);
			}
			StartCoroutine (startChapters());
		}

		void Update ()
		{
			if (LaunchList.instance.mCurrentGame == null) {
				return;
			}
			updateCntr += Time.deltaTime;
			if (updateCntr >= 0.1f) {
				updateCntr = 0f;
				if (incrementScore != 0) {
					if (incrementScore > 0) {
						currentScore += incrementBy;
						incrementScore -= incrementBy;
					} else {
						currentScore -= incrementBy;
						incrementScore += incrementBy;
					}
					coinsText.text = "Coins: " + currentScore;
				} else if (currentScore != LaunchList.instance.mCurrentStudent.Coins) {
					currentScore = LaunchList.instance.mCurrentStudent.Coins;
					incrementScore = 0;
					coinsText.text = "Coins: " + currentScore;
				}
			}

//			if (Input.GetKeyDown (KeyCode.S)) {
//				StartCoroutine (startChapters ());
//			}
		}

		public void UpdateMasterySlider(int mastery)
		{
			if (mastery >= 99) {
				mastery = 100;
			}
			//masterySlider.value = mastery / 100f;
			masterySlider.transform.Find ("MasteryText").GetComponent<Text> ().text = "Mastery " + mastery + "%";
			masterySlider.fillRect.GetComponent<ProceduralImage> ().color = GetColor (mastery);
			if (mastery >= 95f) {
				masterySlider.fillRect.GetComponent<FreeModifier> ().Radius = new Vector4 (100f, 100f, 100f, 100f);
			} else {
				masterySlider.fillRect.GetComponent<FreeModifier> ().Radius = new Vector4 (100f,0f, 0f, 100f);
			}

			GoTween masterySliderTween = new GoTween(masterySlider, 0.5f, new GoTweenConfig().floatProp("value",mastery / 100f));
			Go.addTween (masterySliderTween);
		}
		
		public Color GetColor(int mastery)
		{
			if (mastery == 0)
				return CerebroHelper.HexToRGB ("9A9AA4");
			if (mastery >= 95f)
				return CerebroHelper.HexToRGB ("24C8A6");

			return CerebroHelper.HexToRGB ("FDD000");
		}

		private void LoadKnowledgeComponentMappings()
		{
			currentMappingIndex = -1;
			KCMappings = new List<KCQuestion> ();

			if (!string.IsNullOrEmpty(practiceItemID) && !string.IsNullOrEmpty(KCID)) 
			{
				if (LaunchList.instance.mPracticeItems.ContainsKey (practiceItemID))
				{
					PracticeItems practiceItem = LaunchList.instance.mPracticeItems [practiceItemID];
					if (practiceItem.KnowledgeComponents.ContainsKey (KCID)) 
					{
						KnowledgeComponent KC =practiceItem.KnowledgeComponents[KCID];
						KCMappings = KC.Mappings;

					}
				}
			}
		}

		private KCQuestion GetKCMapping()
		{ 

			if (this.KCMappings.Count > 0 )
			{
				int index = 0;
				if (testMode)
				{
					
					if (!shouldRegenQuestion)
					{
						if (currentMappingIndex >= this.KCMappings.Count - 1) 
						{
							currentMappingIndex = 0;
						}
						else 
						{
							currentMappingIndex++;
						}	
						Debug.Log ("Current Map Index "+currentMappingIndex + " should regen "+shouldRegenQuestion);
					}
					else
					{
						shouldRegenQuestion = false;
					}

					index = currentMappingIndex;

				}
				else 
				{
					index = Random.Range (0, KCMappings.Count);
				}
				return KCMappings [index];
			} 

			return null;
		}

		public string GetCurrentKCID()
		{
			
			string practiceID = baseAssessment.GetPracticeCodeID ();
			if(!string.IsNullOrEmpty(KCID))
			{
				return KCID;
			}
			else if(!string.IsNullOrEmpty(currenKCMapping)  && !string.IsNullOrEmpty(practiceID) && LaunchList.instance.mPracticeItems.ContainsKey(practiceItemID))
			{
				PracticeItems practiceItem = LaunchList.instance.mPracticeItems [practiceItemID];
				string[] spiltRandomMapping = currenKCMapping.Split (new char[]{ 't' }, System.StringSplitOptions.RemoveEmptyEntries);

				int difficulty = int.Parse (spiltRandomMapping [0]);
				int subLevel = int.Parse (spiltRandomMapping [1]);

				Debug.Log ("difficulty " + difficulty + " sub level " + subLevel);

				foreach (var KC in practiceItem.KnowledgeComponents)
				{
						if (KC.Value.Mappings.Exists (x=>x.practiceCodeId == practiceID && x.difficulty == difficulty && x.subLevel == subLevel)) 
					{
						return KC.Value.ID;
					}
				}
			}
			return "";
		}
	}


}
