using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MaterialUI;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;

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

		private string currentQuestionMapping ="1t1";

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
		}

		public void Initialize (string assessmentType, string _title, GameObject _chooseAssessment, MissionItemData missionItemData = null, bool testMode = false,string practiceId="", string KCID ="")
		{
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			chooseAssessment = _chooseAssessment;
			mPracticeName = _title;
			mPracticeName = mPracticeName.Trim ();
			title.text = StringHelper.RemoveNumbers (_title);

			GameObject gameobject = PrefabManager.InstantiateGameObject (assessmentType, gameObject.transform);
			gameobject.transform.SetAsFirstSibling ();
			transform.Find ("Texture").SetAsFirstSibling ();
			gameobject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 708f);
			gameobject.GetComponent<RectTransform> ().position = new Vector3 (0, 0);
			baseAssessment = gameobject.GetComponent<BaseAssessment> ();
			baseAssessment.assessmentName = _title.Replace (" ", "");
			baseAssessment.missionItemData = missionItemData;
			baseAssessment.practiceID = practiceId;
			baseAssessment.KCID = KCID;
			baseAssessment.testMode = testMode;

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

			string practiceID = mPracticeID;

			if (LaunchList.instance.mUseJSON) {
				PracticeData.UpdateLocalFileJSON (practiceID, isCorrect);
			} else {
				PracticeData.UpdateLocalFile (practiceID, isCorrect);
			}
			List<string> missionQuestionIds = CheckMissions (isCorrect, difficulty, sublevel, practiceID);

			if (missionQuestionIds.Count != 0) {
				string missionString = LaunchList.instance.mMission.MissionID;
				foreach (var str in missionQuestionIds) {
					missionString = missionString + "@" + str;
				}
				Cerebro.LaunchList.instance.WriteAnalyticsToFileJSON (assessKey, difficulty, isCorrect, day, timeStarted, Mathf.FloorToInt (timetaken), "0", randomSeed, missionString, UserAnswer);  
			} else {
				Cerebro.LaunchList.instance.WriteAnalyticsToFileJSON (assessKey, difficulty, isCorrect, day, timeStarted, Mathf.FloorToInt (timetaken), "0", randomSeed, " ", UserAnswer);  
			}

			if (isCorrect) {
				if (rightSound != null) {
					audioSource.PlayOneShot (rightSound);
				}
				var bonus = increaseStreak ();
				var increment = _increment + bonus;

				/*if (LaunchList.instance.mPracticeItems.ContainsKey (mPracticeID)) {
					if (LaunchList.instance.mPracticeItems[mPracticeID].RegenerationStarted != "") {
						increment = 0;
					}
				}*/

				if (CerebroProperties.instance.ShowCoins)
					increment = UpdatePracticeItems (baseAssessment.GetPracticeItemID(), baseAssessment.GetCurrentKCID (), increment);
				else
					increment =0;

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
		}

		List<string> CheckMissions (bool isCorrect, int level, int sublevel, string practiceID)
		{
			List<string> missionQuestionIDs = new List<string> ();

			if (practiceID != "" && LaunchList.instance.mMission.Questions != null) {
				foreach (var item in LaunchList.instance.mMission.Questions) {
					bool foundInMission = false;
					if (practiceID == item.Value.PracticeItemID) {
						if (item.Value.QuestionLevel == "-1") {
							foundInMission = true;
						} else {
							if (!item.Value.QuestionLevel.Contains ("@")) {
								if (item.Value.QuestionLevel == level.ToString ()) {
									if (item.Value.SubLevel == "-1") {
										foundInMission = true;
									} else if (item.Value.SubLevel == sublevel.ToString ()) {
										foundInMission = true;
									}
								}
							} else {
								string[] levelVals = item.Value.QuestionLevel.Split ("@" [0]);
								string[] sublevelVals = item.Value.SubLevel.Split ("@" [0]);
								for (var i = 0; i < levelVals.Length; i++) {
									if (levelVals [i] == level.ToString () && sublevelVals [i] == sublevel.ToString ()) {
										foundInMission = true;
									}
								}
							}
						}

						if (foundInMission) {
							missionQuestionIDs.Add (item.Value.QuestionID);
							LaunchList.instance.UpdateLocalMissionFileJSON (item.Value, item.Value.QuestionID, isCorrect);
						}
					}
				}
			}
			return missionQuestionIDs;
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
			if (LaunchList.instance.mPracticeItems.ContainsKey (practiceID)) 
			{
				PracticeItems practiceItem = LaunchList.instance.mPracticeItems[practiceID];

				if (practiceItem.KnowledgeComponents.ContainsKey (KCID))
				{
					KnowledgeComponent KCComponent = practiceItem.KnowledgeComponents [KCID];
				
					if (KCComponent.CurrentCoins + increment > KCComponent.TotalCoins) 
					{
						increment = (KCComponent.TotalCoins - KCComponent.CurrentCoins);
					} 

					KCComponent.CurrentCoins += increment;
					practiceItem.CurrentCoins += increment;
					LaunchList.instance.mPracticeItems [practiceID] = practiceItem;

				}
				else
					return increment;

				LaunchList.instance.mPracticeItems [practiceID] = practiceItem;
			}
			else
				return increment;



			if (increment <= 0)
				return increment;
			

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

		/*public void UpdatePracticeItems (string practiceID, int increment)
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
						if (splitArr [1] == practiceID) {
							var currentCoins = int.Parse (splitArr [6]);
							currentCoins += increment;

							PracticeItems pItem = LaunchList.instance.mPracticeItems [splitArr [1]];
							pItem.CurrentCoins = currentCoins;
							LaunchList.instance.mPracticeItems [pItem.PracticeID] = pItem;
							string regenerationStarted = "";
							if (currentCoins >= pItem.TotalCoins) {
								regenerationStarted = MaxCoinsReached (pItem.PracticeID);
							}
							string newLine = splitArr [0] + "," + splitArr [1] + "," + splitArr [2] + "," + splitArr [3] + "," + splitArr [4] + "," + splitArr [5] + "," + currentCoins.ToString () + "," + regenerationStarted + "," + splitArr[8];
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

	}
}
