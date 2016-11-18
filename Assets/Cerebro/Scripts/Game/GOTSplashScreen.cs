using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cerebro
{
	public class GOTSplashScreen : MonoBehaviour
	{
		[SerializeField]
		GameObject DisplayView;
		[SerializeField]
		GameObject ProgressBar;

		string currentGameID;
		public GameObject[] Groups;
		public GameObject Leaderboard;
		public Sprite[] GroupFlags;
		public Shader BlurShader;
		public Text LeaderboardDate;

		private Text[] GroupNameTexts;
		private Text[] GroupCellTexts;
		private Text[] GroupCoinTexts;
		private Text[] GroupPointTexts;
		private GameObject[] GroupTrophy;
		private GameObject[] GroupPointIcons;

		public GameObject UICamera, LeaderboardCamera, AvatarCamera, CurrAvatar;

		private bool IsLerpStarted, IsLerpLeaderboardStarted, IsAvatarScreenOpening, IsLeaderboardOpening, IsAvatarSelectionOpen, IsLeaderboardOpen;
		private float LerpStartTime, LerpValue;
		private float LerpTotalTime = 0.5f;

		private GameObject AvatarSelectorButtons, BackButton;

		void Start() {
			// when you want to start game directly     
			// StartGame ();

			DisplayView.SetActive (false);
			ProgressBar.SetActive (true);
			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			HTTPRequestHelper.instance.GetGamesForStudent (studentID, GotGames);

			if (!IsAvatarSelectionOpen) {
				AvatarSelectorButtons = CurrAvatar.transform.parent.FindChild ("Buttons").gameObject;
				BackButton = CurrAvatar.transform.root.FindChild ("BackButton").gameObject;
				AvatarSelectorButtons.SetActive (false);
				BackButton.SetActive (false);
				CurrAvatar.SetActive (false);
			}

			if (!IsLeaderboardOpen) {
				GroupNameTexts = new Text[4];
				GroupCellTexts = new Text[4];
				GroupCoinTexts = new Text[4];
				GroupPointTexts = new Text[4];
				GroupTrophy = new GameObject[4];
				GroupPointIcons = new GameObject[4];
				for (int i = 0; i < 4; i++) {
					GroupNameTexts[i] = Groups [i].transform.FindChild ("Name").GetComponent<Text> ();
					GroupCellTexts[i] = Groups [i].transform.FindChild ("Land").GetComponent<Text> ();
					GroupCoinTexts[i] = Groups [i].transform.FindChild ("Coins").GetComponent<Text> ();
					GroupPointTexts[i] = Groups [i].transform.FindChild ("Points").GetComponent<Text> ();
					GroupPointIcons [i] = Groups [i].transform.FindChild ("PointIcon").gameObject;
					GroupTrophy[i] = Groups [i].transform.FindChild ("BG").gameObject;
				}
				Leaderboard.SetActive (false);
				LeaderboardCamera.SetActive (false);
			}
		}

		void GotGames(int status) {
			if (status == 1) {
				// show buttons for all startable games	

				// short circuiting this right now
				// to directly start mStartable[0]
				var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
				// this will fill mCurrentGame
				HTTPRequestHelper.instance.GetGOTGameStatus (studentID, LaunchList.instance.mStartableGames[0].GameID, GotGameData);
			} else {
				print ("NO GAMES FOUND!");
			}
		}

		// being short circuited above
		void GameButtonClicked()
		{
			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			// this will fill mCurrentGame
			// HTTPRequestHelper.instance.GetGOTGameStatus (studentID, "91", GotGameData);
		}

		void Update()
		{
			if (LaunchList.instance.mUpdateTimer) {
				Text TimeStatus = DisplayView.transform.Find ("Time").GetComponent<Text> ();
				if (LaunchList.instance.mTimer.TotalSeconds <= 0) {
					// game has started or ended
					LaunchList.instance.mUpdateTimer = false;
					Start ();
					return;
				}
				string timeStr = "";
				if (LaunchList.instance.mTimer.Days > 0) {
					timeStr += LaunchList.instance.mTimer.Days + " days and ";
					timeStr += LaunchList.instance.mTimer.Hours + " hours";
				} else if (LaunchList.instance.mTimer.Hours > 0) {
					timeStr += LaunchList.instance.mTimer.Hours + " hours and ";
					timeStr += LaunchList.instance.mTimer.Minutes + " minutes";
				} else if (LaunchList.instance.mTimer.Minutes > 0) {
					timeStr += LaunchList.instance.mTimer.Minutes + " minutes and ";
					timeStr += LaunchList.instance.mTimer.Seconds + " seconds";
				} else {
					timeStr += LaunchList.instance.mTimer.Seconds + " seconds!";
				}
				if (LaunchList.instance.mGameStatus [0].Status == "playing") {
					TimeStatus.text = "Game Ends in " + timeStr;
				} else {					
					TimeStatus.text = "Next Game Starts in " + timeStr;
				}
			}

			if (IsLerpStarted) 
			{
				if (Time.time - LerpStartTime < LerpTotalTime) 
				{
					LerpValue = Mathf.Lerp (0f, 1f, (Time.time - LerpStartTime) / LerpTotalTime);
				}
				else 
				{
					IsLerpStarted = false;
					LerpValue = 1;
					if (IsAvatarScreenOpening) {
						AvatarSelectorButtons.SetActive (true);
						BackButton.SetActive (true);
					} else {
						UICamera.GetComponent<Blur> ().enabled = false;
					}
				}
				if (IsAvatarScreenOpening) {
					OpenAvatarCustomization ();
				} else {
					CloseAvatarCustomization ();
				}
			}

			if (IsLerpLeaderboardStarted) 
			{
				if (Time.time - LerpStartTime < LerpTotalTime) 
				{
					LerpValue = Mathf.Lerp (0f, 1f, (Time.time - LerpStartTime) / LerpTotalTime);
				}
				else 
				{
					IsLerpLeaderboardStarted = false;
					LerpValue = 1;
					if (!IsLeaderboardOpening) {
						UICamera.GetComponent<Blur> ().enabled = false;
						AvatarCamera.GetComponent<Blur> ().enabled = false;
						LeaderboardCamera.SetActive (false);
					}
				}
				if (IsLeaderboardOpening) {
					OpenLeaderboard ();
				} else {
					CloseLeaderboard ();
				}
			}
		}


		void GotGameData(int status) {
			if (DisplayView == null) {
				Debug.Log ("tried to access displayview when it was null - GotSplash- GotGameData");
				return;
			}
			Text TimeStatus = DisplayView.transform.Find ("Time").GetComponent<Text> ();
			GameObject PreviousGameData = DisplayView.transform.Find ("PreviousGameData").gameObject;
			Text PrevGameWinner = PreviousGameData.transform.Find ("Winner").GetComponent<Text> ();
			Text PrevGameCoinsSpent = PreviousGameData.transform.Find ("MostCoinsSpent").GetComponent<Text> ();
			Text PrevGameTerritoriesCaptured = PreviousGameData.transform.Find ("MostTerritoriesCaptured").GetComponent<Text> ();

			if (LaunchList.instance.mGameStatus [0].GOTLeaderboard.Count > 0) {
				List<GOTLeaderboard> CurrGOTLeaderboard = LaunchList.instance.mGameStatus [0].GOTLeaderboard;
				CurrGOTLeaderboard = CurrGOTLeaderboard.OrderByDescending (l => l.GroupPoint).ToList ();
				for (int i = 0; i < CurrGOTLeaderboard.Count - 1 && i < 4; i++) {
					for (int j = 0; j < CurrGOTLeaderboard.Count - 1 && j < 4; j++) {
						if (CurrGOTLeaderboard [j].GroupPoint == CurrGOTLeaderboard [j + 1].GroupPoint && CurrGOTLeaderboard [j].GroupCell < CurrGOTLeaderboard [j + 1].GroupCell) {
							GOTLeaderboard l = CurrGOTLeaderboard [j];
							CurrGOTLeaderboard [j] = CurrGOTLeaderboard [j + 1];
							CurrGOTLeaderboard [j + 1] = l;
						}
					}
				}

				for (int i = 0; i < CurrGOTLeaderboard.Count && i < 4; i++) {
					Debug.Log (CurrGOTLeaderboard [i].GroupCell.ToString ());

					GroupNameTexts [i].text = CurrGOTLeaderboard [i].GroupName;
					GroupCoinTexts [i].text = CurrGOTLeaderboard [i].GroupCoin.ToString ();
					GroupCellTexts [i].text = CurrGOTLeaderboard [i].GroupCell.ToString ();
					GroupPointTexts [i].text = CurrGOTLeaderboard [i].GroupPoint.ToString ();
					if (CurrGOTLeaderboard [i].GroupID == GroupMapping.Group1)
						GroupPointIcons [i].GetComponent<Image> ().sprite = GroupFlags [0];
					else if (CurrGOTLeaderboard [i].GroupID == GroupMapping.Group2)
						GroupPointIcons [i].GetComponent<Image> ().sprite = GroupFlags [1];
					else if (CurrGOTLeaderboard [i].GroupID == GroupMapping.Group3)
						GroupPointIcons [i].GetComponent<Image> ().sprite = GroupFlags [2];
					else if (CurrGOTLeaderboard [i].GroupID == GroupMapping.Group4)
						GroupPointIcons [i].GetComponent<Image> ().sprite = GroupFlags [3];
					if (CurrGOTLeaderboard [i].GroupPoint == CurrGOTLeaderboard [0].GroupPoint) {
						if (CurrGOTLeaderboard [i].GroupCell == CurrGOTLeaderboard [0].GroupCell) {
							GroupTrophy [i].SetActive (true);
						} else {
							GroupTrophy [i].SetActive (false);
						}
					} else {
						GroupTrophy [i].SetActive (false);
					}
				}
					
				DateTime GameDate = System.DateTime.ParseExact (LaunchList.instance.mGameStatus [0].LeaderboardEndDate, "yyyy-MM-ddTHH:mm:ss", null);
				LeaderboardDate.text = "Game Ended on "+GameDate.ToString ("MMM dd, yyyy");
			}

			if (status == 1) {
				currentGameID = LaunchList.instance.mGameStatus [0].GameID;

				if(LaunchList.instance.mGameStatus [0].Status == "playing") {
					// enable start button
					DisplayView.transform.Find("Start").gameObject.SetActive(true);
					var t = System.DateTime.Parse (LaunchList.instance.mGameStatus [0].EndTime);
					var tS = System.DateTime.Parse (LaunchList.instance.mGameStatus [0].ServerTime);
					LaunchList.instance.mTimer = t - tS;
				} else {	
					// disable start button
					DisplayView.transform.Find("Start").gameObject.SetActive(false);
					var t = System.DateTime.Parse (LaunchList.instance.mGameStatus [0].StartTime);
					var tS = System.DateTime.Parse (LaunchList.instance.mGameStatus [0].ServerTime);
					LaunchList.instance.mTimer = t - tS;
				}

				//InvokeRepeating("UpdateTimer", 1, 1.0F);
				LaunchList.instance.mUpdateTimer = true;

				if (LaunchList.instance.mGameStatus [0].PreviousGameData.Count > 0) {
					PrevGameWinner.text = "Previous Game Winner: " + LaunchList.instance.mGameStatus [0].PreviousGameData["Winner"];
					PrevGameCoinsSpent.text = "Most Coins Spent: " + LaunchList.instance.mGameStatus [0].PreviousGameData["MostCoinsSpent"];
					PrevGameTerritoriesCaptured.text = "Most Territories Captured: " + LaunchList.instance.mGameStatus [0].PreviousGameData["MostTerritoryWon"];
				} else {
					PreviousGameData.SetActive (false);
				}
			} else {
				currentGameID = "1";
				PreviousGameData.SetActive (false);
				TimeStatus.text = "No active games!";
			}
				
			DisplayView.transform.Find("WinnerList").gameObject.SetActive(true);
			ProgressBar.SetActive (false);
			DisplayView.SetActive (true);
			CurrAvatar.SetActive (true);
		}
			
		public void StartButtonPressed() {
			StartGame ();
		}

		void StartGame() {
			gameObject.SetActive (false);
			GameObject GOT = PrefabManager.InstantiateGameObject (ResourcePrefabs.GOTGame, transform.parent.parent);
			GOT.GetComponent<GOTGame> ().Initialise (this, currentGameID);
		}

		public void BackPressed() {
			UnityEngine.SceneManagement.SceneManager.LoadScene ("CerebroScene");
		}

		void OnApplicationFocus( bool focusStatus )
		{
			if (!focusStatus) {

			} else {
				Start ();
			}
		}

		public void LeaderboardButtonClicked()
		{
			IsLerpLeaderboardStarted = true;
			LerpValue = 0;
			LerpStartTime = Time.time;
			if (!UICamera.GetComponent<Blur> ()) {
				UICamera.AddComponent<Blur> ();
				UICamera.GetComponent<Blur> ().blurShader = BlurShader;
			}
			UICamera.GetComponent<Blur> ().enabled = true;
			if (!AvatarCamera.GetComponent<Blur> ()) {
				AvatarCamera.AddComponent<Blur> ();
				AvatarCamera.GetComponent<Blur> ().blurShader = BlurShader;
			}
			AvatarCamera.GetComponent<Blur> ().enabled = true;
			LeaderboardCamera.SetActive (true);
			IsLeaderboardOpening = true;
			IsLeaderboardOpen = true;
			Leaderboard.SetActive (true);
		}

		public void CloseLeaderboardClicked()
		{
			IsLerpLeaderboardStarted = true;
			LerpValue = 0;
			LerpStartTime = Time.time;
			IsLeaderboardOpening = false;
			IsLeaderboardOpen = false;
		}

		public void ChangeButtonClicked()
		{
			IsLerpStarted = true;
			LerpValue = 0;
			LerpStartTime = Time.time;
			if (!UICamera.GetComponent<Blur> ()) {
				UICamera.AddComponent<Blur> ();
				UICamera.GetComponent<Blur> ().blurShader = BlurShader;
			}
			UICamera.GetComponent<Blur> ().enabled = true;
			IsAvatarScreenOpening = true;
			IsAvatarSelectionOpen = true;
		}

		public void CloseButtonClicked()
		{
			IsLerpStarted = true;
			LerpValue = 0;
			LerpStartTime = Time.time;
			IsAvatarScreenOpening = false;
			IsAvatarSelectionOpen = false;
			AvatarSelectorButtons.SetActive (false);
			BackButton.SetActive (false);
			CurrAvatar.GetComponent<CustomizeAvatar> ().Start ();
		}

		public void OpenLeaderboard()
		{
			UICamera.GetComponent<Blur> ().iterations = (int)(Mathf.Lerp (0, 5, LerpValue));
			AvatarCamera.GetComponent<Blur> ().iterations = (int)(Mathf.Lerp (0, 5, LerpValue));
			Leaderboard.GetComponent<RectTransform> ().anchoredPosition = Vector2.Lerp (new Vector2(-1024f, 0f), Vector2.zero, LerpValue);
		}

		public void CloseLeaderboard()
		{
			UICamera.GetComponent<Blur> ().iterations = (int)(Mathf.Lerp (5, 0, LerpValue));
			AvatarCamera.GetComponent<Blur> ().iterations = (int)(Mathf.Lerp (5, 0, LerpValue));
			Leaderboard.GetComponent<RectTransform> ().anchoredPosition = Vector2.Lerp (Vector2.zero, new Vector2(-1024f, 0f), LerpValue);
		}

		public void OpenAvatarCustomization()
		{
			UICamera.GetComponent<Blur> ().iterations = (int)(Mathf.Lerp (0, 5, LerpValue));
			CurrAvatar.GetComponent<RectTransform> ().localScale = Vector2.Lerp (new Vector2(0.4f, 0.4f), new Vector2(0.5f, 0.5f), LerpValue);
		}

		public void CloseAvatarCustomization()
		{
			UICamera.GetComponent<Blur> ().iterations = (int)(Mathf.Lerp (5, 0, LerpValue));
			CurrAvatar.GetComponent<RectTransform> ().localScale = Vector2.Lerp (new Vector2(0.5f, 0.5f), new Vector2(0.4f, 0.4f), LerpValue);
		}

		public void BackOnScreen(bool fromFocus) {
			if (fromFocus) {
				gameObject.SetActive (true);
				Start ();
			} else {
				gameObject.SetActive (true);

			}
		}
	}
}
