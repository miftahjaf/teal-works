using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using SimpleJSON;
using System.Runtime.InteropServices;

namespace Cerebro
{
	public class GOTWorld
	{
		public List<World> World;
	}

	public class MoveValidateArgs : EventArgs
	{
		public bool Valid;
		public int Coins;

		public MoveValidateArgs (bool valid, int newValue)
		{
			Valid = valid;
			Coins = newValue;
		}
	}

	public class HTTPRequestHelper : MonoBehaviour
	{
		

		//		private string SERVER_URL = "http://192.168.1.28:3000/";
		private string SERVER_URL = "http://apis.aischool.net/cerebro/";//"http://10.0.4.237:3000/cerebro/";//"https://teal-server.herokuapp.com/"; //;
		//private string SERVER_URL = "https://teal-server-staging.herokuapp.com/";
		private string SERVER_NEW_URL = "http://apis.aischool.net/";
		//private string SERVER_NEW_URL ="http://10.0.4.237:3000/";
		public event EventHandler MoveValidated;
		public event EventHandler DescribeImageResponseSubmitted;

		private static HTTPRequestHelper m_Instance;

		public static HTTPRequestHelper instance {
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

			m_Instance = this;
			DontDestroyOnLoad (transform.gameObject);
		}

		public void StressTestServer ()
		{
			for (var i = 0; i < 50; i++) {
				GetStudentData ("316", "7");
			}
		}

		public void CheckVersionNumber (Action<bool> callback)
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey, "");
			Debug.Log ("here "+studentID);
			if (studentID == null || studentID == "") {
				Debug.Log ("callback true");
				LaunchList.instance.IsVersionUptoDate = true;
				LaunchList.instance.CheckingForVersion = false;
				callback (true);
				return;
			}
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["student_id"] = studentID;
			string CurrVersion = VersionHelper.GetVersionNumber();
			if (CurrVersion [0] == 'v') {
				CurrVersion = CurrVersion.Substring (1);
			}
			N ["myData"] ["version"] = CurrVersion;

			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_URL + "student/version/validate", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.IsVersionUptoDate = jsonResponse ["is_version_valid"].AsBool;
					Debug.Log(LaunchList.instance.IsVersionUptoDate);
					callback(true);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
				LaunchList.instance.CheckingForVersion = false;
			});
		}

		public void ValidateMove (string cellID, string cost, string studentID, string groupID, string gameID)
		{
			LaunchList.instance.mWaitingforMovesandWorldServer = true;

			WWWForm form = new WWWForm ();
			form.AddField ("cell_id", cellID);
			form.AddField ("cost", cost);
			form.AddField ("student_id", studentID);
			form.AddField ("group_id", groupID);
			form.AddField ("game_id", gameID);
			//print ("VALIDATE MOVES FOR cell id " + cellID+ " cost "+ cost + " game id " + gameID + " group ID " + groupID);

			CreatePostRequest (SERVER_URL + "games/got/validate_moves", form, (jsonResponse) => {
				//print("GOT RESPONSE FOR VALIDATE MOVES " + jsonResponse.ToString());
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					bool success = false;
					int newValue = -1;
					for (int i = 0; i < jsonResponse.list.Count; i++) {

						string key = (string)jsonResponse.keys [i];
						JSONObject stringObject = (JSONObject)jsonResponse.list [i];

						if (key == "is_success") {
							success = stringObject.str == "true" ? true : false;
						} else if (key == "cost") {
							newValue = int.Parse (stringObject.str);
						}
					}
					if (MoveValidated != null) {
						MoveValidateArgs e = new MoveValidateArgs (success, newValue);
						MoveValidated.Invoke (this, e);
					}
				} else { 
					ValidateMove (cellID, cost, studentID, groupID, gameID);
				}
			});
		}

		public void SendAnalytics (string assessmentID, string difficulty, bool correct, string day, string timeStarted, string timeTaken, string playTime, string seed, string missionField, string UserAnswer = "", int CoinsEarned = 0)
		{
			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			string itemID = "";
			string endTime = "";
			string subLevel = "";
			string itemType = "";
			bool foundSublevel = false;
			bool foundTimeStamp = false;
			bool isLevelUp = false;
			string alteredAssessmentID = assessmentID;

			if (alteredAssessmentID.Contains ("CONTENT_")) {
				itemID = alteredAssessmentID.Substring (alteredAssessmentID.IndexOf ("CONTENT_"));
				alteredAssessmentID = alteredAssessmentID.Substring (0, alteredAssessmentID.IndexOf ("CONTENT_"));;
			}

			if (alteredAssessmentID.Contains ("VIDEO_")) {
				itemType = "VIDEO";
				foundSublevel = true;
				alteredAssessmentID = alteredAssessmentID.Remove (alteredAssessmentID.IndexOf ("VIDEO_"), 6);
			} else if (alteredAssessmentID.Contains ("GOOGLY_")) {
				itemType = "GOOGLY";
				foundSublevel = true;
			} else if (alteredAssessmentID.Contains ("SOLUTION_")) {
				itemType = "SOLUTION";
				alteredAssessmentID = alteredAssessmentID.Remove (alteredAssessmentID.IndexOf ("SOLUTION_"), 9);
			} else {
				itemType = "PRACTICE";
				if (alteredAssessmentID.Contains ("LEVEL_UP")) {
					isLevelUp = true;
					alteredAssessmentID = alteredAssessmentID.Split (new string[] { "LEVEL_UP" }, System.StringSplitOptions.None) [0];
				}
			}
			Debug.Log (alteredAssessmentID);

			for (var i = alteredAssessmentID.Length - 1; i >= 0; i--) {
				if (!foundSublevel && alteredAssessmentID [i] == "t" [0]) {
					foundSublevel = true;
					continue;
				}
				if (!foundSublevel) {
					subLevel += alteredAssessmentID [i];
					continue;
				}
				if (!foundTimeStamp && alteredAssessmentID [i] == "Z" [0]) {
					foundTimeStamp = true;
					continue;
				}
				if (!foundTimeStamp) {
					endTime += alteredAssessmentID [i];
					continue;
				}
				if (!itemID.Contains ("CONTENT_")) {
					itemID += alteredAssessmentID [i];
				}
			}
			if (itemID.Contains ("CONTENT_")) {				
				itemID = itemID.Remove (0, 8);
			} else {
				itemID = StringHelper.Reverse (itemID);
			}
			Debug.Log (itemID);
			endTime = StringHelper.Reverse (endTime);
			subLevel = StringHelper.Reverse (subLevel);

			Debug.Log (alteredAssessmentID+" "+itemID);
			CerebroHelper.DebugLog (timeStarted + " Timings " + endTime);
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["assessment_id"] = alteredAssessmentID.Substring (0, alteredAssessmentID.IndexOf ('Z'));
			N ["myData"] ["component_data"] ["fk_user_id"] = studentID;
			N ["myData"] ["component_data"] ["correct"] = correct.ToString ();
			N ["myData"] ["component_data"] ["difficulty"] = difficulty;
			N ["myData"] ["component_data"] ["start_time"] = timeStarted;
			N ["myData"] ["component_data"] ["time_taken"] = timeTaken;
			N ["myData"] ["component_data"] ["play_time"] = playTime;
			N ["myData"] ["component_data"] ["random_seed"] = seed;
			N ["myData"] ["component_data"] ["mission_data"] = missionField;
			N ["myData"] ["component_data"] ["item_id"] = itemID;
			N ["myData"] ["component_data"] ["end_time"] = endTime;
			N ["myData"] ["component_data"] ["sub_level"] = subLevel;
			N ["myData"] ["component_data"] ["item_type"] = itemType;
			N ["myData"] ["component_data"] ["level_up"] = isLevelUp.ToString ();
			N ["myData"] ["component_data"] ["answer"] = UserAnswer;
			N ["myData"] ["component_data"] ["coins"] = CoinsEarned.ToString ();;
			N ["myData"] ["component_name"] = "math_practice";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					LaunchList.instance.WriteSentAnalyticsJSON (assessmentID);
				} else {
					CerebroHelper.DebugLog ("Error in request");
					LaunchList.instance.WriteAnalyticsToFileJSON (assessmentID, int.Parse (difficulty), correct, day, timeStarted, int.Parse (timeTaken), playTime, int.Parse (seed), missionField, UserAnswer, CoinsEarned, true);
				}
			});
		}

		public void SendYoutubeAnalytics(string componentName,string createdAt, string searchOrVideoText, string videoId, string startTime, string endTime, Action<JSONNode> callback = null)
		{
			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_name"]= componentName;
			N ["myData"] ["component_data"] ["fk_user_id"] = studentID;
			N ["myData"] ["component_data"] ["created_at"] = createdAt.ToString ();

			if (componentName == "youtube_student_log") 
			{
				N ["myData"] ["component_data"] ["video_title"] = searchOrVideoText;
				N ["myData"] ["component_data"] ["video_id"] = videoId;
				N ["myData"] ["component_data"] ["start_time"] = startTime;
				N ["myData"] ["component_data"] ["end_time"] = endTime;
			} else {
				N ["myData"] ["component_data"] ["search_text"] = searchOrVideoText;
			}

			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null &&   jsonResponse.ToString()!= "") {
					CerebroHelper.DebugLog ("Added youtube analytics");
					if(callback!=null)
					{
						callback(jsonResponse);
					}

				} else {
					if(callback!=null)
					{
						callback(null);
					}
					CerebroHelper.DebugLog ("Error in request");
				}
			});
		}

		public void IncrementCoins ()
		{
			int currentDeltaValue = PlayerPrefs.GetInt (PlayerPrefKeys.DeltaCoins);	
			int currentValue = PlayerPrefs.GetInt (PlayerPrefKeys.Coins);

			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			WWWForm form = new WWWForm ();
			form.AddField ("user_id", studentID);
			form.AddField ("coin_value", currentDeltaValue);
			CreatePostRequestSimpleJSON (SERVER_URL + "games/got/increment_coin", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					// save any new coins earnt during this async query into DeltaCoins
					int newDeltaValue = PlayerPrefs.GetInt (PlayerPrefKeys.DeltaCoins);	
					PlayerPrefs.SetInt (PlayerPrefKeys.DeltaCoins, newDeltaValue - currentDeltaValue);

					int newCoinValue = currentValue + currentDeltaValue;
					if (newCoinValue < 0) {
						newCoinValue = 0;
					}
					PlayerPrefs.SetInt (PlayerPrefKeys.Coins, newCoinValue);

					int serverValue = (int)jsonResponse ["coins"].AsInt;

					if (newCoinValue != serverValue) {
						CerebroHelper.DebugLog ("MISMATCH DETECTED");
					}

					LaunchList.instance.mDoingCoinsUpdate = false;
				} else {
					LaunchList.instance.mDoingCoinsUpdate = false;
				}
			});
		}

		public void GetGOTWorld (string gameID, bool singleTime = false)
		{
			if (LaunchList.instance.mDoingWorldUpdate) {
				return;
			}
			LaunchList.instance.mDoingWorldUpdate = true;

			CerebroHelper.DebugLog ("GetGOTWorld");

			WWWForm form = new WWWForm ();
			form.AddField ("game_id", gameID);
			CreatePostRequestSimpleJSON (SERVER_URL + "games/got/get_world", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {

					bool worldExists = false;
					if (LaunchList.instance.mWorld.Count > 0) {
						worldExists = true;
					}
					Debug.Log("Count "+jsonResponse["World"].Count);
					for(int i = 0; i < jsonResponse["World"].Count; i++)
					{
						World newRecord = new World ();
						newRecord.CellID = jsonResponse["World"][i]["CellID"].Value;
						newRecord.Cost = jsonResponse["World"][i]["Cost"].Value;
						newRecord.StudentID = jsonResponse["World"][i]["StudentID"].Value;
						newRecord.GroupID = jsonResponse["World"][i]["GroupID"].Value;
						if(jsonResponse["World"][i]["BabaData"] != null)
						{
							if(jsonResponse["World"][i]["BabaData"]["head"] != null)
							{
								newRecord.BabaHairId = jsonResponse["World"][i]["BabaData"]["head"].AsInt;
							}
							if(jsonResponse["World"][i]["BabaData"]["face"] != null)
							{
								newRecord.BabaFaceId = jsonResponse["World"][i]["BabaData"]["face"].AsInt;
							}
							if(jsonResponse["World"][i]["BabaData"]["body"] != null)
							{
								newRecord.BabaBodyId = jsonResponse["World"][i]["BabaData"]["body"].AsInt;
							}
							if(jsonResponse["World"][i]["BabaData"]["hat"] != null)
							{
								newRecord.BabaHatId = jsonResponse["World"][i]["BabaData"]["hat"].AsInt;
							}
							if(jsonResponse["World"][i]["BabaData"]["goggle"] != null)
							{
								newRecord.BabaGogglesId = jsonResponse["World"][i]["BabaData"]["goggle"].AsInt;
							}
							if(jsonResponse["World"][i]["BabaData"]["badge"] != null)
							{
								newRecord.BabaBadgeId = jsonResponse["World"][i]["BabaData"]["badge"].AsInt;
							}
						}

						if (!worldExists) {
							LaunchList.instance.mWorld.Add (newRecord);
						} else {
							// find this record and update it
							int cellID = LaunchList.instance.FindCell (newRecord.CellID);
							if (cellID != -1) {
								LaunchList.instance.mWorld [cellID].Cost = newRecord.Cost;
								LaunchList.instance.mWorld [cellID].StudentID = newRecord.StudentID;
								LaunchList.instance.mWorld [cellID].GroupID = newRecord.GroupID;
								LaunchList.instance.mWorld [cellID].BabaHairId = newRecord.BabaHairId;
								LaunchList.instance.mWorld [cellID].BabaFaceId = newRecord.BabaFaceId;
								LaunchList.instance.mWorld [cellID].BabaBodyId = newRecord.BabaBodyId;
								LaunchList.instance.mWorld [cellID].BabaHatId = newRecord.BabaHatId;
								LaunchList.instance.mWorld [cellID].BabaGogglesId = newRecord.BabaGogglesId;
								LaunchList.instance.mWorld [cellID].BabaBadgeId = newRecord.BabaBadgeId;
							} else {
								CerebroHelper.DebugLog ("Couldn't update cuz cell id not foudn " + newRecord.CellID);
							}
						}
					}
					if(LaunchList.instance.mGameStatus.Count <= 0)
					{
						GOTGameStatus gs = new GOTGameStatus();
						LaunchList.instance.mGameStatus.Add(gs);
					}
					GOTGameStatus currGameStatus = LaunchList.instance.mGameStatus[0];
					if(currGameStatus.GroupNames == null || currGameStatus.GroupNames.Length <= 0)
					{
						currGameStatus.GroupNames = new string[4];
						for(int i = 0; i < 4; i++)
						{
							currGameStatus.GroupNames[i] = jsonResponse["team_score"][""+(i+1)]["name"].Value;
						}
					}
					if(currGameStatus.GroupCurrScores == null || currGameStatus.GroupCurrScores.Length <= 0)
					{
						currGameStatus.GroupCurrScores = new int[4];
						for(int i = 0; i < 4; i++)
						{
							currGameStatus.GroupCurrScores[i] = jsonResponse["team_score"][""+(i+1)]["score"].AsInt;
						}
					}
					if(currGameStatus.GroupTargetScores == null || currGameStatus.GroupTargetScores.Length <= 0)
					{
						currGameStatus.GroupTargetScores = new int[4];
					}
					for(int i = 0; i < 4; i++)
					{
						currGameStatus.GroupTargetScores[i] = jsonResponse["team_score"][""+(i+1)]["score"].AsInt;
					}

					LaunchList.instance.WorldLoaded (singleTime);
				} else {
					LaunchList.instance.mDoingWorldUpdate = false;
				}
			});
		}

		// returns all separate games that the student could play right now
		// games may be in a waiting or active state
		public void GetGamesForStudent (string studentID, Action<int> callback)
		{
			CerebroHelper.DebugLog ("GetGamesForStudent");

			WWWForm form = new WWWForm ();
			form.AddField ("fk_user_id", studentID);
			CreatePostRequestSimpleJSON (SERVER_URL + "games/got/get_game", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.mStartableGames.Clear();
					for (var i = 0; i < jsonResponse.Count; i++) {
						CerebroHelper.DebugLog ("Got Games List------------------------------------------");
						if(jsonResponse [i]["start_time"].Value == "null")
						{
							Debug.Log("skipping game without time");
							continue;
						}
						StartableGame pGame = new StartableGame();
						pGame.GameID = jsonResponse[i]["game_id"];
						pGame.GameName = jsonResponse[i]["name"];
						pGame.StartTime = jsonResponse [i]["start_time"].Value;
						// the rest of the fields to be filled in later	
						LaunchList.instance.mStartableGames.Add(pGame);
					}

					callback (1);
				} else {
					GetGamesForStudent (studentID, callback);
				}
			});	
		}

		public void GetGOTGameData (string gameID, string studentID, Action<int> callback)
		{
			CerebroHelper.DebugLog ("GetGOTGameData");

			WWWForm form = new WWWForm ();
			form.AddField ("fk_user_id", studentID);
			form.AddField ("game_id", gameID);
			CreatePostRequestSimpleJSON (SERVER_URL + "games/got/get_game_group", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog ("GAME LOADED------------------------------------------");
					LaunchList.instance.mCurrentGame.GroupID = jsonResponse ["group_color"].Value;
					LaunchList.instance.ReadAvatar();
					if(LaunchList.instance.mAvatar.ColorId != LaunchList.instance.mCurrentGame.GroupID)
					{
						LaunchList.instance.mAvatar.ColorId = LaunchList.instance.mCurrentGame.GroupID;
						LaunchList.instance.WriteAvatar(LaunchList.instance.mAvatar);
					}
					LaunchList.instance.mCurrentGame.GroupIDDB = jsonResponse ["group_id"].Value;
					LaunchList.instance.mCurrentGame.StudentID = studentID;
					callback (1);
				} else {
					GetGOTGameData (gameID, studentID, callback);
				}
			});
		}

		public void GetGOTGameStatus (string studentID, string gameID, Action<int> callback)
		{
			CerebroHelper.DebugLog ("GetGOTGameStatus");

			WWWForm form = new WWWForm ();
			form.AddField ("fk_user_id", studentID);
			form.AddField ("game_id", gameID);
			CreatePostRequestSimpleJSON (SERVER_URL + "games/got/get_game_status", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {

					CerebroHelper.DebugLog ("GAME STATUS LOADED------------------------------------------");
					LaunchList.instance.mGameStatus.Clear ();
					GOTGameStatus status = new GOTGameStatus ();
					status.ServerTime = jsonResponse ["current_server_time"].Value;
					status.GameID = jsonResponse  ["current_game_id"].Value;
					status.StartTime = jsonResponse ["current_start_time"].Value;
					status.EndTime = jsonResponse  ["current_end_time"].Value;
					status.Status = jsonResponse  ["current_game_state"].Value;
					status.PreviousGameID = jsonResponse  ["prev_game_id"].Value;
					status.PreviousGameData = new Dictionary<string,string> ();

					status.PreviousGameData.Add ("Winner", jsonResponse  ["prev_game_data"] ["winner"].Value);
					status.PreviousGameData.Add ("MostCoinsSpent", jsonResponse  ["prev_game_data"] ["most_coins_spent"].Value);
					status.PreviousGameData.Add ("MostTerritoryWon", jsonResponse  ["prev_game_data"] ["most_territory_won"].Value);
					
					status.GOTLeaderboard = new List<GOTLeaderboard>();
					if(jsonResponse ["prev_game_data"]["game_result"] != null && jsonResponse ["prev_game_data"]["game_result"] != "null")
					{							
						Debug.Log(jsonResponse ["prev_game_data"]["game_result"].Count);
						for(var i = 0; i < jsonResponse ["prev_game_data"]["game_result"].Count; i++)
						{
							GOTLeaderboard l = new GOTLeaderboard();
							Debug.Log(jsonResponse ["prev_game_data"]["game_result"][i]["coin_spent"].Value);
							l.GroupCoin = jsonResponse ["prev_game_data"]["game_result"][i]["coin_spent"].AsInt;
							l.GroupID = jsonResponse ["prev_game_data"]["game_result"][i]["group_color"].Value;
							l.GroupName = jsonResponse ["prev_game_data"]["game_result"][i]["group_name"].Value;
							l.GroupCell = jsonResponse ["prev_game_data"]["game_result"][i]["cells"].AsInt;
							if(jsonResponse ["prev_game_data"]["game_result"][i]["points"] != null && jsonResponse ["prev_game_data"]["game_result"][i]["points"] != "null")
							{
								l.GroupPoint = jsonResponse ["prev_game_data"]["game_result"][i]["points"].AsInt;
							}
							else
							{
								l.GroupPoint = 0;
							}
							status.GOTLeaderboard.Add(l);
						}
						status.LeaderboardEndDate = jsonResponse ["prev_game_data"]["end_time"].Value;
					}
					LaunchList.instance.mGameStatus.Add(status);
					callback (1);
				} else {
					//GetGOTGameStatus (studentID, callback);
					callback (0);
				}
			});
		}

		public void GetStudentData (string studentID, string gradeID)
		{
			// XXX Change this when Database changes
			LaunchList.instance.ReadAvatar();
			gradeID = gradeID.ToLower ();
			CerebroHelper.DebugLog ("FETCH STUDENT DATA FOR " + studentID);
			WWWForm form = new WWWForm ();
			form.AddField ("student_id", studentID);
			CreatePostRequestSimpleJSON (SERVER_URL + "student/get_student_data", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog (jsonResponse);
					string name = "";
					int coins = 0;
					LaunchList.instance.mCurrentStudent.StudentName = jsonResponse ["first_name"].Value + " " + jsonResponse ["last_name"].Value;
					LaunchList.instance.mCurrentStudent.StudentID = studentID;
					LaunchList.instance.mCurrentStudent.GradeID = gradeID;
					LaunchList.instance.mCurrentStudent.Coins = jsonResponse ["coins"].AsInt;
					if(jsonResponse ["mission"].Value != null)
						PlayerPrefs.SetString (PlayerPrefKeys.MissionID, jsonResponse ["mission"].Value);
					if(jsonResponse ["profile_image"].Value != null)
						PlayerPrefs.SetString (PlayerPrefKeys.ProfilePicKey, jsonResponse ["profile_image"].Value);
					LaunchList.instance.mCurrentStudent.ContentIDs = new SortedDictionary<int, string> ();
					for(int i = 0; i < jsonResponse ["student_playlist"].Count; i++)
					{
						LaunchList.instance.mCurrentStudent.ContentIDs.Add (i, jsonResponse ["student_playlist"][i].Value);
					}
					string BabaId = "";
					BabaId += jsonResponse ["baba_data"] ["head"].Value;
					BabaId += jsonResponse ["baba_data"] ["face"].Value;
					BabaId += jsonResponse ["baba_data"] ["body"].Value;
					PlayerPrefs.SetString(PlayerPrefKeys.BabaID, BabaId);
					LaunchList.instance.mAvatar.HairID = jsonResponse ["baba_data"] ["head"].AsInt;
					LaunchList.instance.mAvatar.HeadID = jsonResponse ["baba_data"] ["face"].AsInt;
					LaunchList.instance.mAvatar.BodyID = jsonResponse ["baba_data"] ["body"].AsInt;
					LaunchList.instance.mAvatar.HatID = jsonResponse ["baba_data"] ["hat"].AsInt;
					LaunchList.instance.mAvatar.GogglesID = jsonResponse ["baba_data"] ["goggle"].AsInt;
					LaunchList.instance.mAvatar.BadgeID = jsonResponse ["baba_data"] ["badge"].AsInt;
					if(jsonResponse ["baba_data"] ["color"] != null)
					{
						LaunchList.instance.mAvatar.ColorId = jsonResponse ["baba_data"] ["color"].Value;
						PlayerPrefs.SetString(PlayerPrefKeys.GOTGameTeamID, jsonResponse ["baba_data"] ["color"].Value);
					}
					LaunchList.instance.WriteAvatar(LaunchList.instance.mAvatar);

					if(jsonResponse["student_proficiency_constants"] !=null)
					{
						LaunchList.instance.UpdateKCProficiencyConstants(jsonResponse["student_proficiency_constants"]);
					}
					PlayerPrefs.SetInt (PlayerPrefKeys.Coins, LaunchList.instance.mCurrentStudent.Coins);
					int currentDeltaValue = PlayerPrefs.GetInt (PlayerPrefKeys.DeltaCoins);	
					if (currentDeltaValue != 0) {
						LaunchList.instance.mCurrentStudent.Coins = LaunchList.instance.mCurrentStudent.Coins + currentDeltaValue;
						LaunchList.instance.mCoinsValueChanged = true;
					}

					CerebroHelper.DebugLog ("student data " + name + " " + gradeID + " " + studentID + " " + coins);
					LaunchList.instance.StudentDataLoaded ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					GetStudentData (studentID, gradeID);  // TRY AGAIN
				}
			});
		}

		public void getLeaderBoard (string date, string grade)
		{
			WWWForm form = new WWWForm ();
			form.AddField ("date", date);
			form.AddField ("grade", grade);
			CreatePostRequestSimpleJSON (SERVER_URL + "quiz/leader_board/get", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					if (jsonResponse ["leader_board"] != null && jsonResponse ["leader_board"].Count > 0) {
						LaunchList.instance.mLeaderboard = new List<LeaderBoard> ();
						CerebroHelper.DebugLog ("got count " + jsonResponse ["leader_board"].Count);
						for (int i = 0; i < jsonResponse ["leader_board"].Count; i++) {
							LeaderBoard l = new LeaderBoard ();
							l.StudentID = jsonResponse ["leader_board"] [i] ["student_id"].Value;
							l.StudentName = jsonResponse ["leader_board"] [i] ["first_name"].Value + " " + jsonResponse ["leader_board"] [i] ["last_name"].Value;
							l.StudentScore = jsonResponse ["leader_board"] [i] ["score"].AsInt;
							CerebroHelper.DebugLog ("adding " + l.StudentID + " " + l.StudentName + " " + l.StudentScore);
							LaunchList.instance.mLeaderboard.Add (l);
						}
						LaunchList.instance.LeaderBoardLoadComplete (true);
					} else {
						LaunchList.instance.mLeaderboard = new List<LeaderBoard> ();
						LaunchList.instance.LeaderBoardLoadComplete (true);
					}

				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					LaunchList.instance.LeaderBoardLoadComplete (false);
				}
			});
		}

		public void GetUserProfile (string email, Func<StudentProfile,int> callback)
		{
			WWWForm form = new WWWForm ();
			CerebroHelper.DebugLog ("sending req with email " + email);
			form.AddField ("student_email", email);
			CreatePostRequest (SERVER_URL + "student/get_student_profile", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					JSONObject jsonObject = (JSONObject)jsonResponse;
					StudentProfile currProfile = new StudentProfile ();
					for (int i = 0; i < jsonObject.Count; i++) {
						string key = (string)jsonObject.keys [i];
						JSONObject stringObject = (JSONObject)jsonObject.list [i];
						if (key == "first_name") {
							currProfile.FirstName = stringObject.str;
						} else if (key == "last_name") {
							currProfile.LastName = stringObject.str;
						} else if (key == "student_id") {
							currProfile.StudentID = stringObject.n.ToString ();
						} else if (key == "grade") {
							currProfile.Grade = stringObject.n.ToString ();
						} else if (key == "section") {
							currProfile.Section = stringObject.str;
						} else if (key == "roll") {
							currProfile.RollNo = stringObject.n.ToString ();
						} else {
							CerebroHelper.DebugLog ("UNRECOGNIZED ATTRIB " + key);
						}
					}
					callback (currProfile);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback (null);
				}
			});
		}

		public void GetQuizForDate (string date, Action<List<QuizData>> callback)
		{
			string grade = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.GradeKey)) {
				grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
			} else {
				CerebroHelper.DebugLog ("No grade found..returning null");
				callback (null);
			}
				
			CerebroHelper.DebugLog (date);
			WWWForm form = new WWWForm ();
			form.AddField ("date", date);
			form.AddField ("grade", grade);
			CreatePostRequestSimpleJSON (SERVER_URL + "quiz/get", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.mQuizQuestions.Clear ();
					for (int i = 0; i < jsonResponse.Count; i++) {
						QuizData q = new QuizData ();
						q.QuizDate = jsonResponse [i] ["quiz_date"].Value;
						q.QuestionID = jsonResponse [i] ["question_id"].Value;
						int cnt = jsonResponse [i] ["answer_options"].Count;
						for(int j = 0; j < cnt; j++)
						{
							q.AnswerOptions += "@" + jsonResponse [i] ["answer_options"] [j] ["ans"].Value;
						}
						q.AnswerOptions = q.AnswerOptions.Substring(1);
//						q.AnswerOptions = jsonResponse [i] ["answer_options"].Value;
						q.CorrectAnswer = jsonResponse [i] ["correct_answer"].Value;
						q.Difficulty = jsonResponse [i] ["difficulty"].Value;
//						q.QuestionIndex = jsonResponse [i] ["question_index"].Value;
						q.QuestionMedia = jsonResponse [i] ["question_media_url"].Value;
						q.QuestionMediaType = jsonResponse [i] ["question_media_type"].Value;
						q.QuestionText = jsonResponse [i] ["ques_text"].Value;
						q.QuestionSubText = jsonResponse [i] ["question_sub_text"].Value;
						q.QuestionType = "MCQ";//jsonResponse [i] ["question_type"].Value;
//						q.AnswerType = jsonResponse [i] ["answer_type"].Value;
//						q.AnswerURL = jsonResponse [i] ["answer_url"].Value;
						q.AnswerURL = "";

						LaunchList.instance.mQuizQuestions.Add (q);
					}

					// sort based on question index
//					int test;
//					LaunchList.instance.mQuizQuestions.Sort (delegate(QuizData x, QuizData y) {
//						if (int.TryParse (x.QuestionIndex, out test) || int.TryParse (y.QuestionIndex, out test)) {
//							return int.Parse (x.QuestionIndex) - int.Parse (y.QuestionIndex);
//						} else {
//							return 0;
//						}
//					});

					callback (LaunchList.instance.mQuizQuestions);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback (null);
				}
			});
		}

		public void GetWritersCornerForDate (string date)
		{
			string grade = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.GradeKey)) {
				grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
			} else {
				CerebroHelper.DebugLog ("No grade found..returning null");
				LaunchList.instance.mDescribeImage = null;
				LaunchList.instance.GotEnglishImage ();
			}

			CerebroHelper.DebugLog (date);
			WWWForm form = new WWWForm ();
			form.AddField ("date", date);
			form.AddField ("grade", grade);
			CreatePostRequestSimpleJSON (SERVER_URL + "writers_corner/get_writers_corner", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					DescribeImage d = new DescribeImage ();
					if(jsonResponse ["image_id"] == null) {
						d.ImageID = null;
					} else {
						d.ImageID = (jsonResponse ["image_id"].AsInt).ToString ();
						d.MediaType = jsonResponse ["media_type"].Value;
						d.MediaURL = jsonResponse ["media_url"].Value;
						d.PromptText = jsonResponse ["prompt_text"].Value;
						d.SubPromptText = jsonResponse ["sub_prompt_text"].Value;
						if(jsonResponse ["word_limit"] != null)
						{
							d.CharLimit = jsonResponse ["word_limit"].AsInt;
						}
						else 
						{
							d.CharLimit = 500;
						}
					}
					LaunchList.instance.mDescribeImage = d;
					LaunchList.instance.GotEnglishImage ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					LaunchList.instance.mDescribeImage = null;
					LaunchList.instance.GotEnglishImage ();
				}
			});
		}

		public void GetVerbalizeForDate (string date)
		{
			string grade = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.GradeKey)) {
				grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
			} else {
				CerebroHelper.DebugLog ("No grade found..returning null");
				LaunchList.instance.mVerbalize = null;
				LaunchList.instance.GotVerbalizeText ();
			}

			CerebroHelper.DebugLog (date);
			WWWForm form = new WWWForm ();
			form.AddField ("date", date);
			form.AddField ("grade", grade);
			CreatePostRequestSimpleJSON (SERVER_URL + "verbalize/get_verbalize", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					if(jsonResponse.Count > 0)
					{
						jsonResponse = jsonResponse[0];
					}
					Verbalize d = new Verbalize ();
					if(jsonResponse ["verbalize_id"] == null) {
						d.VerbalizeID = null;
					} else {
						d.VerbalizeID = (jsonResponse ["verbalize_id"].AsInt).ToString ();
						d.VerbGrade = grade;
						d.VerbalizeDate = System.DateTime.Now.ToString("yyyy") + date;
						d.VerbDifficulty = jsonResponse["difficulty"].Value;
						d.VerbTitle = jsonResponse["title"].Value;
						if(jsonResponse["author"].Value != null)
							d.VerbAuthor = jsonResponse["author"].Value;	
						d.VerbGenre = jsonResponse["genre"].Value;
						d.PromptText = jsonResponse ["passage"].Value;
					}
					LaunchList.instance.mVerbalize = d;
					LaunchList.instance.GotVerbalizeText ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					LaunchList.instance.mVerbalize = null;
					LaunchList.instance.GotVerbalizeText ();
				}
			});
		}

		public void GetQuizAnalyticsForDate (string date)
		{
			string grade = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.GradeKey)) {
				grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
			} else {
				CerebroHelper.DebugLog ("No grade found..");
			}

			WWWForm form = new WWWForm ();
			form.AddField ("date", date);
			form.AddField ("grade", grade);
			form.AddField ("n_count", 10);
			CreatePostRequest (SERVER_URL + "quiz/get_quiz", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("response " + jsonResponse.str);
					LaunchList.instance.mQuizAnalytics.Clear ();
					for (int j = 0; j < jsonResponse.list.Count; j++) {
						JSONObject jsonObject = (JSONObject)jsonResponse.list [j];
						var newRecord = new QuizAnalytics ();
						for (int i = 0; i < jsonObject.Count; i++) {
							string attributeName = (string)jsonObject.keys [i];
							string value = ((JSONObject)jsonObject.list [i]).str;

							// XXX getting fancy here and using reflection, will this bite us?
							if (newRecord.GetType ().GetProperty (attributeName) != null) {
								newRecord.GetType ().GetProperty (attributeName).SetValue (newRecord, value, null);
							} else {								
								CerebroHelper.DebugLog ("UNRECOGNIZED ATTRIB " + attributeName);
							}
						}
						LaunchList.instance.mQuizAnalytics.Add (newRecord);
					}
					
					LaunchList.instance.GotQuizAnalyticsForDate ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}


		public void GetKCMastery()
		{
			string fileName = Application.persistentDataPath + "/KCsMasteryLatest.txt";
			if (File.Exists (fileName)) 
			{
				string json = File.ReadAllText (fileName);
				JSONNode jsonNode = JSONNode.Parse (json);
				if (jsonNode != null && jsonNode ["Data"] ["Mastery"] != null) 
				{
					LaunchList.instance.GotKCMastery ();
					return;
				}
			}
			try {

				WWWForm form = new WWWForm ();
				form.AddField ("fk_user_id", PlayerPrefs.GetString (PlayerPrefKeys.IDKey,"0"));
				Debug.Log("Load old kc mastery");
				CreatePostRequestSimpleJSON (SERVER_URL + "practice_item/kc/mastery_all",form, (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {

						JSONNode jsonNode;
						if (File.Exists (fileName)) 
						{
							string json = File.ReadAllText (fileName);
							jsonNode = JSONNode.Parse (json);
						}
						else
						{
							jsonNode = JSONClass.Parse ("{\"Data\"}");
						}

						jsonNode["Data"]["Mastery"] = jsonResponse;
						jsonNode["VersionNumber"] = LaunchList.instance.VersionData;
						File.WriteAllText(fileName,jsonNode.ToString());
						CerebroHelper.DebugLog ("Loaded complete : mastery");
						LaunchList.instance.GotKCMastery();


					} else {
						CerebroHelper.DebugLog ("EXCEPTION GetKCMastery");
					}
				});
			} catch (Exception e) {
				CerebroHelper.DebugLog ("Exception - GetKCMastery: " + e.Message);
			}
		}

		public void GetPracticeItems ()
		{
			string fileName = Application.persistentDataPath + "/PracticeItemsWithKC.txt";
			try {

				WWWForm form = new WWWForm ();
				form.AddField ("student_id", PlayerPrefs.GetString (PlayerPrefKeys.IDKey,"0"));
				Debug.Log("Load practice items");
				CreatePostRequestSimpleJSON (SERVER_URL + "practice_item/student/with_kc/question_object",form, (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {
						LaunchList.instance.mQuizAnalytics.Clear ();
						StreamWriter sr = File.CreateText (fileName);	
						sr.WriteLine (jsonResponse.ToString());
						sr.Close ();

						CerebroHelper.DebugLog ("LOAD COMPLETE");
						LaunchList.instance.GotPracticeItems ();


					} else {
						CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					}
				});
			} catch (Exception e) {
				CerebroHelper.DebugLog ("Exception - GetPractice: " + e.Message);
			}
		}

		public void GetPracticeItemsName()
		{
			string fileName = Application.persistentDataPath + "/PracticeItemNames.txt";
			try {

				WWWForm form = new WWWForm ();
				form.AddField ("grade", -1);
				Debug.Log("Load practice items");
				CreatePostRequestSimpleJSON (SERVER_URL + "practice_item/get_practice_items_by_grade",form, (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {
						LaunchList.instance.mPracticeItemNames.Clear ();
						StreamWriter sr = File.CreateText (fileName);	
						sr.WriteLine (jsonResponse.ToString());
						sr.Close ();

						CerebroHelper.DebugLog ("LOAD COMPLETE");
						LaunchList.instance.GotPraticeItemNames ();


					} else {
						CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					}
				});
			} catch (Exception e) {
				CerebroHelper.DebugLog ("Exception - GetPractice: " + e.Message);
			}
		}

		/*public void GetPracticeMastery(string praticeId,Action<int> callback)
		{
			try {
				WWWForm form = new WWWForm ();
				form.AddField("student_id", PlayerPrefs.GetString (PlayerPrefKeys.IDKey,""));
				form.AddField("practice_item", praticeId);

				CreatePostRequestSimpleJSON (SERVER_URL + "practice_item/kc/mastery",form, (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {
						CerebroHelper.DebugLog ("MASTERY LOAD COMPLETE");
						if(LaunchList.instance.mPracticeItems.ContainsKey(praticeId))
						{
							PracticeItems pItem = LaunchList.instance.mPracticeItems[praticeId];
							foreach(var kc in pItem.KnowledgeComponents)
							{
								if(jsonResponse[kc.Value.ID]!=null)
								{
									kc.Value.Mastery =jsonResponse[kc.Value.ID].AsInt;
								}
							}
							callback(1);
						}
						else
						{
							callback(0);
						}

					} else 
					{
						callback(0);
						CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					}
				});
			} catch (Exception e) {
				callback (0);
				CerebroHelper.DebugLog ("Exception - GetPractice: " + e.Message);
			}
		}*/

		public void GetFeatureForDate (string date, Func<Feature,int> callback, int grade)
		{
			string currGrade = grade.ToString ();
			if (currGrade == "-1") {
				if (PlayerPrefs.HasKey (PlayerPrefKeys.GradeKey)) {
					currGrade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
				} else {
					CerebroHelper.DebugLog ("No grade found..");
				}
			}

			WWWForm form = new WWWForm ();
			form.AddField ("date", date);
			form.AddField ("grade", currGrade);
			CreatePostRequestSimpleJSON (SERVER_URL + "feature/get_feature", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					if (jsonResponse.Count == 0) {
						callback (null);
					}
					Feature f = new Feature ();
					f.FeatureDate = date;
					f.Grade = grade.ToString ();
					f.Type = jsonResponse ["media_type"].Value;
					f.MediaUrl = jsonResponse ["media_url"].Value;
					f.MediaText = jsonResponse ["media_text"].Value;
					f.ImageUrl = jsonResponse ["thumb_url"].Value;

					callback (f);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback (null);
				}
			});
		}

		public void GetMissionQuestions (string missionID)  //Old Mission
		{
			//missionID = "MISMFRA0604R";
			CerebroHelper.DebugLog ("getting ques " + missionID);
			WWWForm form = new WWWForm ();
			form.AddField ("mission_id", missionID);
			CreatePostRequestSimpleJSON (SERVER_URL + "mission/get_mission_data", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.mMission.MissionID = missionID;
					LaunchList.instance.mMission.MissionName = jsonResponse ["mission_name"].Value;
					LaunchList.instance.mMission.TimeStarted = System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss");
					LaunchList.instance.mMission.Questions = new SortedDictionary<string, MissionItemData> ();
					for (int i = 0; i < jsonResponse ["mission_data"].Count; i++) {
						MissionItemData m = new MissionItemData ();
						m.QuestionID = jsonResponse ["mission_data"] [i] ["question_id"].Value;
						m.QuestionLevel = jsonResponse ["mission_data"] [i] ["question_level"].Value;
						m.SubLevel = jsonResponse ["mission_data"] [i] ["sub_level"].Value;
						m.PracticeItemID = jsonResponse ["mission_data"] [i] ["item_id"].Value;
						m.Type = jsonResponse ["mission_data"] [i] ["type"].Value;
						m.QuestionText = jsonResponse ["mission_data"] [i] ["question_text"].Value;
						m.AnswerOptions = jsonResponse ["mission_data"] [i] ["answer_options"].Value;
						m.Answer = jsonResponse ["mission_data"] [i] ["answer"].Value;
						m.QuestionMediaType = jsonResponse ["mission_data"] [i] ["question_media_type"].Value;
						m.QuestionMediaURL = jsonResponse ["mission_data"] [i] ["question_media_url"].Value;
						m.QuestionTitle = jsonResponse ["mission_data"] [i] ["question_title"].Value;
						m.CompletionCondition = new SortedDictionary<string, string> ();
						m.CompletionCondition.Add ("Attempts", jsonResponse ["mission_data"] [i] ["completion_condition"] ["Attempts"].Value);
						m.CompletionCondition.Add ("Correct", jsonResponse ["mission_data"] [i] ["completion_condition"] ["Correct"].Value);
						m.CompletionCondition.Add ("Streak", jsonResponse ["mission_data"] [i] ["completion_condition"] ["Streak"].Value);
						m.ConditionCurrentValue = "0";
						m.CompleteBool = "false";
						m.TotalAttempts = "0";
						m.CorrectAttempts = "0";
						LaunchList.instance.mMission.Questions.Add (m.QuestionID, m);
					}
					LaunchList.instance.GotMissions ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}


		public void GetMission()
		{
			CerebroHelper.DebugLog ("Getting mission ");
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			N ["myData"] ["request_data"] ["student_id"] = studentID;
	
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_NEW_URL + "missions/user/student/mission/get", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString()!= "") {
					if(jsonResponse["response"]["mission_data"]["question_types"]!=null && jsonResponse["response"]["mission_data"]["question_types"].Count>0)
					{
						LaunchList.instance.missionData.Add(new Mission().GetMission(jsonResponse["response"]["mission_data"]));
					}
					LaunchList.instance.GotMission();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void GetProperties ()
		{
			string fileName = Application.persistentDataPath + "/PropertiesJSON.txt";
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			try {
				WWWForm form = new WWWForm ();
				form.AddField ("student_id", studentID);
				CreatePostRequestSimpleJSON (SERVER_URL + "properties/get_properties", form, (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {
//						StreamWriter sr = File.CreateText (fileName);
//						for (int i = 0; i < jsonResponse.Count; i++) {
//							Properties p = new Properties ();
//							p.PropertyName = jsonResponse [i] ["property_name"].Value;
//							p.PropertyValue = jsonResponse [i] ["property_value"].Value;
//
//							sr.WriteLine (p.PropertyName + "," + p.PropertyValue);
//						}
						JSONNode N = JSONClass.Parse ("{\"Data\"}");
						N["Data"] = jsonResponse;
						N["VersionNumber"] = LaunchList.instance.VersionData;
						File.WriteAllText (fileName, N.ToString());
//						sr.Close ();
						LaunchList.instance.GotProperties ();
					} else {
						CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					}
				});
			} catch (Exception e) {
				CerebroHelper.DebugLog ("Exception - GetPractice: " + e.Message);
			}
		}

		public void GetEnglishImageResponses (string imageID)
		{
			string grade = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.GradeKey)) {
				grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
			} else {
				CerebroHelper.DebugLog ("No grade found..");
			}

			string section = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.SectionKey)) {
				section = PlayerPrefs.GetString (PlayerPrefKeys.SectionKey);
			} else {
				CerebroHelper.DebugLog ("No section found..");
			}

			WWWForm form = new WWWForm ();
			form.AddField ("grade", grade);
			form.AddField ("section", section);
			form.AddField ("wc_id", imageID);
			CreatePostRequestSimpleJSON (SERVER_URL + "writers_corner/response/get", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.mDescribeImageUserResponses.Clear ();
					for (int i = 0; i < jsonResponse.Count; i++) {
						DescribeImageUserResponse d = new DescribeImageUserResponse ();
						d.ImageID = jsonResponse [i] ["wc_id"].Value;
						d.StudentID = jsonResponse [i] ["fk_user_id"].Value;
						d.UserResponse = jsonResponse [i] ["user_response"].Value;
						d.StudentName = jsonResponse [i] ["student_name"].Value;
						if (jsonResponse [i] ["profile_image"].Value != "" && jsonResponse [i] ["profile_image"].Value != "null") {
							d.StudentImageUrl = jsonResponse [i] ["profile_image"].Value;
						}
						LaunchList.instance.mDescribeImageUserResponses.Add (d);
					}
					LaunchList.instance.GotEnglishImageResponses ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					GetEnglishImageResponses (imageID);
				}
			});
		}

		public void GetEnglishImage ()
		{
			Dictionary<string,string> imageIDDict = LaunchList.instance.GetNextImageIDJSON ();
			string imageID = imageIDDict ["imageID"];
			bool fetchBool = imageIDDict ["fetchBool"] == "true" ? true : false;

			CerebroHelper.DebugLog ("getting image " + imageID);
			if (LaunchList.instance.CheckForSubmittedImageResponsesJSON (imageID) && !fetchBool) {
				LaunchList.instance.GotEnglishImage ();
				return;
			}

			WWWForm form = new WWWForm ();
			form.AddField ("image_id", imageID);
			CreatePostRequestSimpleJSON (SERVER_URL + "describe_image/get_next_english_image", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					//for(int i = 0; i < 1; i++)
					//{
					DescribeImage d = new DescribeImage ();
					d.ImageID = (jsonResponse ["image_id"].AsInt).ToString ();
					d.MediaType = jsonResponse ["media_type"].Value;
					d.MediaURL = jsonResponse ["media_url"].Value;
					d.PromptText = jsonResponse ["prompt_text"].Value;
					d.SubPromptText = jsonResponse ["sub_prompt_text"].Value;
					//}
					LaunchList.instance.mDescribeImage = d;
					LaunchList.instance.GotEnglishImage ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					LaunchList.instance.mDescribeImage = null;
					LaunchList.instance.GotEnglishImage ();
				}
			});
		}

		public void GetExplanation ()
		{
			string fileName = Application.persistentDataPath + "/ExplanationsJSON.txt";

			try {
				WWWForm form = new WWWForm ();
				CreatePostRequestSimpleJSON (SERVER_URL + "practice_item/get_practice_item_explanation", form, (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {
//						StreamWriter sr = File.CreateText (fileName);
						for (int i = 0; i < jsonResponse.Count; i++) {
							Explanation explanation = new Explanation ();
							explanation.PracticeItemID = jsonResponse [i] ["practice_item_id"].Value;
							explanation.Level = jsonResponse [i] ["difficulty"].Value;
							explanation.SubLevel = jsonResponse [i] ["sub_level"].Value;
							explanation.URL = jsonResponse [i] ["explanation_url"].Value;
							explanation.ContentId = jsonResponse [i] ["content_id"].Value;
							string key = explanation.PracticeItemID + "L" + explanation.Level + explanation.SubLevel;
//							sr.WriteLine (explanation.PracticeItemID + "," + explanation.Level + "," + explanation.SubLevel + "," + explanation.URL);
							if (LaunchList.instance.mExplanation.ContainsKey (key)) {
								LaunchList.instance.mExplanation [key] = explanation;
							} else {
								LaunchList.instance.mExplanation.Add (key, explanation);
							}	
						}
//						sr.Close ();
						JSONNode N = JSONClass.Parse ("{\"Data\"}");
						N["Data"] = jsonResponse;
						N["VersionNumber"] = LaunchList.instance.VersionData;
						File.WriteAllText (fileName, N.ToString());
					} else {
						CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					}
					LaunchList.instance.ExplanationLoaded ();
				});
			} catch (Exception e) {
				CerebroHelper.DebugLog ("Exception - GetExplanation: " + e.Message);
				LaunchList.instance.ExplanationLoaded ();
			}
		}

		public void GetContentItems ()
		{
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			string studentId = PlayerPrefs.GetString(PlayerPrefKeys.IDKey, "1");
			N ["myData"] ["student_id"] = studentId;
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_URL + "content_item/get_content_items", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.mContentItems = new List<ContentItem> ();
					for (int i = 0; i < jsonResponse.Count; i++) {
						ContentItem c = new ContentItem ();
						c.ContentID = jsonResponse [i] ["content_id"].Value;
						c.ContentName = jsonResponse [i] ["content_name"].Value;
						c.ContentDate = jsonResponse [i] ["content_date"].Value;
						c.ContentDescription = jsonResponse [i] ["content_description"].Value;
						c.ContentLink = jsonResponse [i] ["content_link"].Value;
						c.ContentType = jsonResponse [i] ["content_type"].Value;
						LaunchList.instance.mContentItems.Add (c);
					}
					CerebroHelper.DebugLog ("added " + LaunchList.instance.mContentItems.Count + " rows");
					LaunchList.instance.mTableLoaded [(int)Cerebro.TableTypes.tContent] = true;
					LaunchList.instance.ContentTableLoaded ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void GetServerTime ()
		{
			Debug.Log ("calling for time");
			CreateGetRequestSimpleJSON (SERVER_URL + "get/server_time", (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					string currTime = jsonResponse ["current_time"].Value;
					Debug.Log("curr time "+currTime);
					LaunchList.instance.SetCurrentTime(currTime);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					LaunchList.instance.SetCurrentTime("-1");
				}
			});
		}

		public void GetHomeworkFeed(string studentId, int pageSize, int pageCount, Action<JSONNode> callback)
		{
			if (!LaunchList.instance.mhasInternet) {
				callback(null);
				return;
			}
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["request_data"] ["user_id"] = studentId;
			N ["myData"] ["request_data"] ["page_size"] = pageSize.ToString();
			N ["myData"] ["request_data"] ["page_count"] = pageCount.ToString();
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_NEW_URL + "homework/teacher/feed/get", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					if(jsonResponse["response"]["is_success"] != null && jsonResponse["response"]["is_success"].Value == "true")
					{
						callback(jsonResponse["response"]);
					}
					else
					{
						callback(null);
					}
				}  else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback(null);
				}
			} );
		}

		public void GetResponseFeed(string homeworkId, Action<JSONNode> callback)
		{
			CreateGetRequestSimpleJSON (SERVER_NEW_URL + "homework/teacher/responsefeed/get?hwc_id="+homeworkId, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					if(jsonResponse["response"]["is_success"] != null && jsonResponse["response"]["is_success"].Value == "true")
					{
						callback(jsonResponse["response"]);
					}
					else
					{
						callback(null);
					}
				}  else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback(null);
				}
			} );
		}

		//
		// PUT Requests
		//

		public void SendUrbanDeviceToken(string deviceToken)
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey, "");
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["student_id"] = studentID;
			N ["myData"] ["device_token"] = deviceToken;
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_URL + "student/device_token/set", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog ("Added new row");
					if(jsonResponse["is_success"].Value == "true")
					{
						Debug.Log ("token sent successfully");
					}
					else
					{
						Debug.Log("token not sent");
					}
				}  else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			} );
		}

		public void RemoveDeviceToken()
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey, "");
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["student_id"] = studentID;
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_URL + "student/device_token/unset", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog ("Added new row");
					if(jsonResponse["is_success"].Value == "true")
					{
						CerebroHelper.DebugLog("Removed device token from server");
					}
					else
					{
						CerebroHelper.DebugLog("Removed device token from server failed");
					}
				}  else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					CerebroHelper.DebugLog("Removed device token from server failed");
				}
			} );
		}

		public void SendHomeworkResponseWC(string homeworkContextId, string createdAt, string response, Action<bool> callback)
		{
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["request_data"] ["hwc_id"] = homeworkContextId;
			N ["myData"] ["request_data"] ["created_at"] = LaunchList.instance.ConvertDateToStandardString (DateTime.Now);//createdAt;
			N ["myData"] ["request_data"] ["type"] = "wc";
			N ["myData"] ["request_data"] ["data"] ["response"] = response;
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_NEW_URL + "homework/teacher/create/response", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog ("Added new row");
					if(jsonResponse["response"]["is_success"].Value == "true")
					{
						callback(true);
					}
					else
					{
						callback(false);
					}
				}  else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback(false);
				}
			} );
		}

		public void SendHomeworkComment(string homeworkContextId, string createdAt, string teacherId, string comment, Action<bool> callback)
		{
			Debug.Log ("sending cmt");
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey, "");
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["request_data"] ["hwc_id"] = homeworkContextId;
			N ["myData"] ["request_data"] ["created_at"] = LaunchList.instance.ConvertDateToStandardString (DateTime.Now);//createdAt;
			N ["myData"] ["request_data"] ["from"] = studentID;
			N ["myData"] ["request_data"] ["to"] = teacherId;
			N ["myData"] ["request_data"] ["data"] ["comment"] = comment;
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_NEW_URL + "homework/teacher/create/comment", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog ("Added new row");
					if(jsonResponse["response"]["is_success"].Value == "true")
					{						
						callback(true);
					}
					else
					{						
						callback(false);
					}
				}  else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback(false);
				}
			} );
		}

		public void SendHomeworkResponseAnnouncement(string homeworkContextId, string createdAt, Action<bool> callback)
		{
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["request_data"] ["hwc_id"] = homeworkContextId;
			N ["myData"] ["request_data"] ["created_at"] = LaunchList.instance.ConvertDateToStandardString (DateTime.Now);//createdAt;
			N ["myData"] ["request_data"] ["type"] = "announcement";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_NEW_URL + "homework/teacher/create/response", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog ("Added new row");
					if(jsonResponse["response"]["is_success"].Value == "true")
					{
						Debug.Log("true");
						callback(true);
					}
					else
					{
						Debug.Log("false");
						callback(false);
					}
				}  else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback(false);
				}
			} );
		}

		public void SubmitCompletedMission(Mission mission)
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["request_data"]["studentId"] = studentID;
			JSONNode missionJson = JSONNode.Parse (mission.missionJson);
		/*	N ["myData"] ["request_data"] ["missionJson"] ["kc_id"] = mission.KCID.ToString ();
			N ["myData"] ["request_data"] ["missionJson"] ["completion_condition"] = mission.completionCondition.ToString ();
			N ["myData"] ["request_data"] ["missionJson"] ["completion_questions_limit"] = mission.completionQuestionsLimit.ToString ();
			N ["myData"] ["request_data"] ["missionJson"] ["completion_questions_correct_limit"] = mission.completionQuestionsCorrectLimit.ToString ();
			N ["myData"] ["request_data"] ["missionJson"] ["mission_text"] = mission.missionText.ToString ();
			int count = mission.questions.Count;
			for (int i = 0; i < count; i++) {
				N ["myData"] ["request_data"] ["missionJson"] ["question_types"] [i] ["practice_item_id"] = mission.questions [i].practiceItemID.ToString();
				N ["myData"] ["request_data"] ["missionJson"] ["question_types"] [i] ["practice_item_name"] = mission.questions [i].practiceName.ToString();
				N ["myData"] ["request_data"] ["missionJson"] ["question_types"] [i] ["difficulity"] = mission.questions [i].difficulty.ToString();
				N ["myData"] ["request_data"] ["missionJson"] ["question_types"] [i] ["sub_level"] = mission.questions [i].subLevel.ToString();
			}*/
			N ["myData"] ["request_data"] ["missionJson"] = missionJson;
			int count = mission.answers.Count;
			for (int i = 0; i < count; i++) {
				N ["myData"] ["request_data"]  ["missionQuestionData"] [i] ["practice_item_id"] = mission.answers [i].practiceItemID.ToString();
				N ["myData"] ["request_data"]  ["missionQuestionData"] [i] ["seed"] = mission.answers [i].seed.ToString();
				N ["myData"] ["request_data"]  ["missionQuestionData"] [i] ["difficulity"] = mission.answers [i].difficulty.ToString();
				N ["myData"] ["request_data"]  ["missionQuestionData"] [i] ["sub_level"] = mission.answers [i].subLevel.ToString();
				N ["myData"] ["request_data"]  ["missionQuestionData"] [i] ["ans"] = mission.answers [i].ans.ToString();
				N ["myData"] ["request_data"]  ["missionQuestionData"] [i] ["correct"] = mission.answers [i].correct ?"1":"0";
			}
			N ["myData"] ["request_data"]  ["startTime"] = mission.startTime.ToString();
			N ["myData"] ["request_data"]  ["endTime"] = mission.endTime.ToString ();

			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_NEW_URL + "missions/mission/analytics/add", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added mission analytics");
					LaunchList.instance.missionData.Remove(mission);
					WelcomeScript.instance.UpdateMission ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void SendAvatarSet(string BabaId, int HatId, int GogglesId, int BadgeId, Action<bool> callback)
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["student_id"] = studentID;
			N ["myData"] ["baba_data"]["head"] = BabaId[0].ToString();
			N ["myData"] ["baba_data"]["face"] = BabaId[1].ToString();
			N ["myData"] ["baba_data"]["body"] = BabaId[2].ToString();
			N ["myData"] ["baba_data"]["hat"] = HatId.ToString();
			N ["myData"] ["baba_data"]["goggle"] = GogglesId.ToString();
			N ["myData"] ["baba_data"]["badge"] = BadgeId.ToString();
			N ["myData"] ["baba_data"]["color"] = LaunchList.instance.mAvatar.ColorId;
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "student/baba/set", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					callback(true);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback(false);
				}
			});
		}

		public void SendFlaggedData (string assessmentID, string seed, int level, int selector, bool isFlagged, string version, Action<string> callback)
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			string practiceItemId = assessmentID.Substring (0, assessmentID.IndexOf ("Z"));
			string timestamp = assessmentID.Substring (assessmentID.IndexOf ("Z") + 1, assessmentID.IndexOf ("t") - assessmentID.IndexOf ("Z") - 1);

			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["fk_user_id"] = studentID;
			N ["myData"] ["component_data"] ["practice_item_id"] = practiceItemId;
			N ["myData"] ["component_data"] ["created_date"] = timestamp;
			N ["myData"] ["component_data"] ["seed"] = seed;
			N ["myData"] ["component_data"] ["difficulty"] = level.ToString();
			N ["myData"] ["component_data"] ["sub_level"] = selector.ToString();
			N ["myData"] ["component_data"] ["is_flagged"] = isFlagged.ToString ();
			N ["myData"] ["component_data"] ["version"] = version;

			N ["myData"] ["component_name"] = "flagged_question";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					callback(assessmentID);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback("");
				}
			});
		}

		public void SendRatingInfo (string type, string studentID, string contentID, float timeSpent, int rating)
		{
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["content_id"] = contentID;
			N ["myData"] ["component_data"] ["type"] = type;
			N ["myData"] ["component_data"] ["student_id"] = studentID;
			N ["myData"] ["component_data"] ["rating"] = rating.ToString ();
			N ["myData"] ["component_data"] ["time_spent"] = timeSpent.ToString ();
			N ["myData"] ["component_data"] ["timestamp"] = System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss");

			N ["myData"] ["component_name"] = "video_rating";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void SendFeedback (string studentID, string message, Action<int> callback)
		{
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["student_id"] = studentID;
			N ["myData"] ["component_data"] ["message"] = message;

			N ["myData"] ["component_name"] = "feedback";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					callback (1);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback (0);
				}
			});
		}

		public void SubmitDescribeImageResponse (string studentID, string ImageID, string userResponse)
		{
			string grade = PlayerPrefs.GetString (PlayerPrefKeys.GradeKey);
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["wc_id"] = ImageID;
			N ["myData"] ["component_data"] ["fk_user_id"] = studentID;
			N ["myData"] ["component_data"] ["user_response"] = StringHelper.RemoveUnicodeCharacters (userResponse);
			N ["myData"] ["component_data"] ["grade"] = grade;
			N ["myData"] ["component_name"] = "writers_corner_response";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					if (DescribeImageResponseSubmitted != null) {
						DescribeImageResponseSubmitted.Invoke (this, null);
					}
					LaunchList.instance.SetLastImageIDJSON (LaunchList.instance.mDescribeImage.ImageID, System.DateTime.Now.ToString ("yyyyMMdd"));
					LaunchList.instance.WriteImageResponseToFileJSON (userResponse);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					SubmitDescribeImageResponse (studentID, ImageID, userResponse);
				}
			});
		}

		public void SubmitVerbalizeResponse (Verbalize verb)
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["verbalize_id"] = verb.VerbalizeID;
			N ["myData"] ["component_data"] ["fk_user_id"] = studentID;
			N ["myData"] ["component_data"] ["response_url"] = verb.UserResponseURL;
			N ["myData"] ["component_data"] ["start_time"] = verb.VerbStartTime;
			N ["myData"] ["component_data"] ["end_time"] = verb.VerbEndTime;
			N ["myData"] ["component_data"] ["speed"] = verb.VerbSpeed;
			N ["myData"] ["component_name"] = "verbalize_response";
			CerebroHelper.DebugLog (verb.VerbalizeID);
			CerebroHelper.DebugLog (studentID);
			CerebroHelper.DebugLog (verb.UserResponseURL);
			CerebroHelper.DebugLog (verb.VerbStartTime);
			CerebroHelper.DebugLog (verb.VerbEndTime);
			CerebroHelper.DebugLog (verb.VerbSpeed);
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					LaunchList.instance.SetLastVerbalizeIDJSON (verb.VerbalizeID, System.DateTime.Now.ToString ("yyyyMMdd"));
					verb.UploadedToServer = true;
					Debug.Log (verb.VerbalizeDate + " " + verb.UserSubmitted + " " + verb.UserResponseURL + " " + verb.UploadedToServer);
//					LaunchList.instance.WriteVerbalizeResponseToFile (verb);
					GameObject VerbLandPage = GameObject.FindGameObjectWithTag("VerbalizeLandingPage");

					LaunchList.instance.SetVerbalizeUploaded(verb);
					LaunchList.instance.CheckForVerbalizeToUpload();
					if(LaunchList.instance.UploadingVerbalize.Contains(verb))
					{
						LaunchList.instance.UploadingVerbalize.Remove(verb);
					}

					if(VerbLandPage != null)
					{
						if (LaunchList.instance.mVerbalize != null) {
							System.DateTime dt = System.DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbalizeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
							VerbLandPage.GetComponent<VerbalizeLandingPage>().ManageCardDataForDate (dt);
						}
					}
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void SendQuizAnalytics (string date, string studentAndQuestionID, string answer, string correct, string timeStarted, string timeTaken)
		{
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["quiz_date"] = date;
			N ["myData"] ["component_data"] ["student_question_id"] = studentAndQuestionID;
			N ["myData"] ["component_data"] ["answer"] = answer;
			N ["myData"] ["component_data"] ["correct"] = correct;
			N ["myData"] ["component_data"] ["start_time"] = timeStarted;
			N ["myData"] ["component_data"] ["time_taken"] = timeTaken;
			N ["myData"] ["component_name"] = "quiz_analytics";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					LaunchList.instance.SentQuizAnalyticsJSON (studentAndQuestionID, date);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void SendQuizAnalyticsGrouped (List<QuizAnalytics> allQuestions, Func<QuizAnalyticsResponse,int> callback)
		{
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");

			for (var i = 0; i < allQuestions.Count; i++) {
				QuizAnalytics question = allQuestions [i];

				N ["myData"] ["component_data"] [i] ["quiz_date"] = question.QuizDate;
				N ["myData"] ["component_data"] [i] ["student_question_id"] = question.StudentAndQuestionID;
				N ["myData"] ["component_data"] [i] ["answer"] = question.Answer;
				N ["myData"] ["component_data"] [i] ["correct"] = question.Correct;
				N ["myData"] ["component_data"] [i] ["start_time"] = question.TimeStarted;
				N ["myData"] ["component_data"] [i] ["time_taken"] = question.TimeTaken;
			}
			N ["myData"] ["component_name"] = "quiz_analytics_grouped";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.SentQuizAnalyticsGrouped (allQuestions);
					QuizAnalyticsResponse response = new QuizAnalyticsResponse ();
					response.Date = jsonResponse ["return_data"] ["quiz_date"].Value;
					response.Success = jsonResponse ["return_data"] ["is_success"].AsBool;
					response.TotalAttempts = jsonResponse ["return_data"] ["total_attempts"].Value;
					response.TotalCorrect = jsonResponse ["return_data"] ["total_correct"].Value;
					response.Score = jsonResponse ["return_data"] ["score"].Value;
					callback (response);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					QuizAnalyticsResponse response = new QuizAnalyticsResponse ();
					response.Success = false;
					callback (response);
				}
			});
		}

		public void PushTheWoosh (string studentID, string deviceID, string pushToken)
		{
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["student_id"] = studentID;
			N ["myData"] ["component_data"] ["device_id"] = deviceID;
			N ["myData"] ["component_data"] ["push_token"] = pushToken;
			N ["myData"] ["component_name"] = "push";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					LaunchList.instance.PushedItWooshedIt ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void PushMissionComplete (string missionID, string day, string timeStarted, string timeEnded, float accuracy) //Old Mission
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			System.DateTime timeStart = System.DateTime.ParseExact (timeStarted, "yyyy-MM-ddTHH:mm:ss", null);
			System.DateTime timeEnd = System.DateTime.ParseExact (timeEnded, "yyyy-MM-ddTHH:mm:ss", null);
			System.DateTime dayTime = System.DateTime.ParseExact (day, "yyyyMMdd", null);

			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["student_id"] = studentID;
			N ["myData"] ["component_data"] ["mission_id"] = missionID;
			N ["myData"] ["component_data"] ["accuracy"] = accuracy.ToString ();
			N ["myData"] ["component_data"] ["created_date"] = dayTime.ToString ("yyyy-MM-dd");
			N ["myData"] ["component_data"] ["end_time"] = timeEnd.ToString ("yyyy-MM-ddTHH:mm:ss");
			N ["myData"] ["component_data"] ["start_time"] = timeStart.ToString ("yyyy-MM-ddTHH:mm:ss");
			N ["myData"] ["component_name"] = "missions_analytics";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArraySimpleJSON (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					string nextMissionId = jsonResponse ["return_data"] ["next_mission_id"].Value;
					PlayerPrefs.SetString (PlayerPrefKeys.MissionID, nextMissionId);
					LaunchList.instance.UpdateLocalMissionFileForCompletionJSON (accuracy, true);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					LaunchList.instance.UpdateLocalMissionFileForCompletionJSON (accuracy, false);
				}
			});
		}





		public void uploadProfilePic (string FileName, Texture2D tex)
		{
			string url = "upload/sign-s3-unity?file-name=" + FileName + "&file-type=image/png";
			string nextURL = "", signedURL = "";
			CreateGetRequestSimpleJSON (SERVER_URL + url, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					signedURL = jsonResponse ["signedRequest"].Value;
					nextURL = jsonResponse ["url"].Value;
					uploadImage (signedURL, nextURL, tex);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		void uploadImage (string uploadURL, string picURL, Texture2D tex)
		{
			byte[] imageData = tex.EncodeToPNG ();
			CreatePutRequestByteArray (uploadURL, imageData, "image/png", (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "" && jsonResponse ["is_success"].AsBool == true) {
					print ("Photo Successfully uploaded");
					PlayerPrefs.SetString (PlayerPrefKeys.ProfilePicKey, picURL);
					SetProfileImage (picURL);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void uploadProfileVid (string FileName, string vidPath, Verbalize verb)
		{
			string url = "upload/sign-s3-unity-video?file-name=" + FileName + "&file-type=video/quicktime";
			print ("Getting URL " + url);
			string nextURL = "", signedURL = "";
			CreateGetRequestSimpleJSON (SERVER_URL + url, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					signedURL = jsonResponse ["signedRequest"].Value;
					nextURL = jsonResponse ["url"].Value;
					StartCoroutine (uploadVideo (signedURL, nextURL, vidPath, verb));
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		IEnumerator uploadVideo (string uploadURL, string picURL, string vidPath, Verbalize verb)
		{
			print ("Got URL " + uploadURL);
			print ("picURL " + picURL);
			print ("vidPath " + vidPath);
			WWW vidFile = new WWW ("file:///" + vidPath);
			yield return vidFile;
			if (vidFile.error != null) {
				print ("Error in opening file " + vidFile.error);
				yield break;
			}
			byte[] imageData = vidFile.bytes;
			CreatePutRequestByteArray (uploadURL, imageData, "video/quicktime", (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "" && jsonResponse ["is_success"].AsBool == true) {
					print ("success video upload");
					verb.UserResponseURL = picURL;
					SubmitVerbalizeResponse(verb);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		void SetProfileImage (string imageUrl)
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			WWWForm form = new WWWForm ();
			form.AddField ("student_id", studentID);
			form.AddField ("image_url", imageUrl);

			CreatePostRequest (SERVER_URL + "student/set_student_profile_image", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog ("Profile Image succesfully set");
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void SendUsageAlarm ()
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["student_id"] = studentID;
			N ["myData"] ["component_data"] ["type"] = "USAGE";
			N ["myData"] ["component_data"] ["alarm_time"] = System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss");
			N ["myData"] ["component_name"] = "alarm";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Usage Alarm sent succesfully!");
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		public void SendTelemetry (string studentID, List<Telemetry> list)
		{

			if (list.Count == 0) {
				return;
			}

			JSONNode N = JSONSimple.Parse ("{\"myData\"}");

			for (var i = 0; i < list.Count; i++) {
				Telemetry row = list [i];

				N ["myData"] ["component_data"] [i] ["timestamp"] = row.Timestamp;
				N ["myData"] ["component_data"] [i] ["student_id"] = studentID;
				N ["myData"] ["component_data"] [i] ["description"] = row.Description;
				N ["myData"] ["component_data"] [i] ["time_spent"] = row.TimeSpent;
				N ["myData"] ["component_data"] [i] ["type"] = row.Type;
			}
			N ["myData"] ["component_name"] = "usage_details_grouped";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Telemetry sent succesfully!");
					CerebroAnalytics.instance.CleanLocalLogFileJSON (list);
					CerebroScript.instance.SendUsageAnalytics ();
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
				}
			});
		}

		private void CreateGetRequest (string url, Action< JSONObject > callback = null)
		{
			HTTP.Request someRequest = new HTTP.Request ("get", url);
			someRequest.Send (( request) => {
				string json = "";
				if (request.exception == null) {
					json = request.response.Text;
				} else {
					CerebroHelper.DebugLog ("Error in request" + request.exception);
				}
				JSONObject jsonObject = new JSONObject (json);
				callback (jsonObject);
			});
		}

		private void CreatePostRequest (string url, WWWForm form, Action< JSONObject > callback = null)
		{
			HTTP.Request someRequest = new HTTP.Request ("post", url, form);
			someRequest.Send (( request) => {
				string json = "";
				if (request.exception == null) {
					json = request.response.Text;
				} else {
					CerebroHelper.DebugLog ("Error in request " + request.exception);
				}
				CerebroHelper.DebugLog (url + "," + "current response -------------" + json);
				JSONObject jsonObject = new JSONObject (json);
				callback (jsonObject);
			});
		}

		private void CreatePostRequestSimpleJSON (string url, WWWForm form, Action< JSONNode > callback = null)
		{
			HTTP.Request someRequest = new HTTP.Request ("post", url, form);
			someRequest.Send (( request) => {
				string json = "";
				if (request.exception == null) {
					json = request.response.Text;
				} else {
					CerebroHelper.DebugLog ("Error in request " + request.exception);
				}
				CerebroHelper.DebugLog ("current response -------------" + json);
				if (IsJson (json)) {
					JSONNode jsonObject = JSONSimple.Parse (json);
					callback (jsonObject);
				} else {
					JSONNode jsonObject = null;
					callback (jsonObject);
					CerebroHelper.DebugLog ("error in JSON");
				}
			});
		}

		private void CreatePostRequestNoFormSimpleJSON (string url, Action< JSONNode > callback = null)
		{
			HTTP.Request someRequest = new HTTP.Request ("post", url);
			someRequest.Send (( request) => {
				string json = "";
				if (request.exception == null) {
					json = request.response.Text;
				} else {
					CerebroHelper.DebugLog ("Error in request " + request.exception);
				}
				CerebroHelper.DebugLog ("current response -------------" + json);
				if (IsJson (json)) {
					JSONNode jsonObject = JSONSimple.Parse (json);
					callback (jsonObject);
				} else {
					JSONNode jsonObject = null;
					callback (jsonObject);
					CerebroHelper.DebugLog ("error in JSON");
				}
			});
		}

		private void CreatePostRequestByteArray (string url, byte[] form, Action< JSONObject > callback = null)
		{
			HTTP.Request someRequest = new HTTP.Request ("post", url, form, true);
			someRequest.Send (( request) => {
				string json = "";
				if (request.exception == null) {
					json = request.response.Text;
				} else {
					CerebroHelper.DebugLog ("Error in request " + request.exception);
				}
				CerebroHelper.DebugLog ("current response -------------" + json);
				JSONObject jsonObject = new JSONObject (json);
				callback (jsonObject);
			});
		}

		private void CreatePostRequestByteArraySimpleJSON (string url, byte[] form, Action< JSONNode > callback = null)
		{
			HTTP.Request someRequest = new HTTP.Request ("post", url, form, true);
			someRequest.Send (( request) => {
				string json = "";
				if (request.exception == null) {
					json = request.response.Text;
				} else {
					CerebroHelper.DebugLog ("Error in request " + request.exception);
				}
				CerebroHelper.DebugLog ("current response ------"+url+"-------" + json);
				if (IsJson (json)) {
					JSONNode jsonObject = JSONSimple.Parse (json);
					callback (jsonObject);
				} else {
					JSONNode jsonObject = null;
					callback (jsonObject);
					CerebroHelper.DebugLog ("error in JSON");
				}
			});
		}

		private void CreateGetRequestSimpleJSON (string url, Action< JSONNode > callback = null)
		{
			HTTP.Request someRequest = new HTTP.Request ("get", url);
			someRequest.Send (( request) => {
				string json = "";
				if (request.exception == null) {
					json = request.response.Text;
				} else {
					CerebroHelper.DebugLog ("Error in request " + request.exception);
				}
				CerebroHelper.DebugLog ("current response -------------" + json);
				if (IsJson (json)) {
					JSONNode jsonObject = JSONSimple.Parse (json);
					callback (jsonObject);
				} else {
					JSONNode jsonObject = null;
					callback (jsonObject);
					CerebroHelper.DebugLog ("error in JSON");
				}
			});
		}

		private void CreatePutRequestByteArray (string url, byte[] form, string mimetype, Action< JSONNode > callback = null)
		{
			JSONNode N = JSONSimple.Parse ("{\"is_success\"}");
			HTTP.Request someRequest = new HTTP.Request ("put", url, form, mimetype);
			someRequest.Send (( request) => {
				if (request.exception == null) {
					if (request.response.status == 200) {
						N ["is_success"] = true.ToString ();
					} else {
						N ["is_success"] = false.ToString ();
					}
				} else {
					CerebroHelper.DebugLog ("Error in request " + request.exception);
				}
				CerebroHelper.DebugLog ("current response -------------" + N.ToString ());
				callback (N);
			});
		}

		public bool IsJson (string input)
		{ 
			input = input.Trim (); 
			return input.StartsWith ("{") && input.EndsWith ("}")
			|| input.StartsWith ("[") && input.EndsWith ("]"); 
		}

		void accessData (JSONObject obj)
		{
			switch (obj.type) {
			case JSONObject.Type.OBJECT:
				for (int i = 0; i < obj.list.Count; i++) {
					string key = (string)obj.keys [i];
					JSONObject j = (JSONObject)obj.list [i];
					CerebroHelper.DebugLog (key);
					accessData (j);
				}
				break;
			case JSONObject.Type.ARRAY:
				foreach (JSONObject j in obj.list) {
					accessData (j);
				}
				break;
			case JSONObject.Type.STRING:
				CerebroHelper.DebugLog (obj.str);
				break;
			case JSONObject.Type.NUMBER:
				CerebroHelper.DebugLog (obj.n);
				break;
			case JSONObject.Type.BOOL:
				CerebroHelper.DebugLog (obj.b);
				break;
			case JSONObject.Type.NULL:
				CerebroHelper.DebugLog ("NULL");
				break;

			}
		}

		string trimString (string s)
		{
			return s.Replace ('\"', ' ').Trim ();
		}
	}
}
