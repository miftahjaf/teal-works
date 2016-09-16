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
		private string SERVER_URL = "https://teal-server.herokuapp.com/";
//		private string SERVER_URL = "https://teal-server-staging.herokuapp.com/";

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

		public void SendAnalytics (string assessmentID, string difficulty, bool correct, string day, string timeStarted, string timeTaken, string playTime, string seed, string missionField, string UserAnswer = "")
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
			N ["myData"] ["component_data"] ["student_id"] = studentID;
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
			N ["myData"] ["component_name"] = "analytics";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					LaunchList.instance.WriteSentAnalytics (assessmentID);
				} else {
					CerebroHelper.DebugLog ("Error in request");
					LaunchList.instance.WriteAnalyticsToFile (assessmentID, int.Parse (difficulty), correct, day, timeStarted, int.Parse (timeTaken), playTime, int.Parse (seed), missionField, UserAnswer, true);
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
			CreatePostRequest (SERVER_URL + "games/got/get_world", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					JSONObject Wrld = (JSONObject)jsonResponse.list [0];

					bool worldExists = false;
					if (LaunchList.instance.mWorld.Count > 0) {
						worldExists = true;
					}
					foreach (JSONObject j in Wrld.list) {
						World newRecord = new World ();
						for (int i = 0; i < j.list.Count; i++) {
							string key = (string)j.keys [i];
							JSONObject stringObject = (JSONObject)j.list [i];

							if (key == "CellID") {
								newRecord.CellID = stringObject.n.ToString ();
							} else if (key == "Cost") {
								newRecord.Cost = stringObject.n.ToString ();
							} else if (key == "GroupID") {
								newRecord.GroupID = stringObject.n.ToString ();
							} else if (key == "StudentID") {
								newRecord.StudentID = stringObject.n.ToString ();
							} else {
								CerebroHelper.DebugLog ("UNRECOGNIZED ATTRIB " + key);
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
							} else {
								CerebroHelper.DebugLog ("Couldn't update cuz cell id not foudn " + newRecord.CellID);
							}
						}
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
			form.AddField ("student_id", studentID);
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
			form.AddField ("student_id", studentID);
			form.AddField ("game_id", gameID);
			CreatePostRequestSimpleJSON (SERVER_URL + "games/got/get_game_group", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					CerebroHelper.DebugLog ("GAME LOADED------------------------------------------");
					LaunchList.instance.mCurrentGame.GroupID = jsonResponse ["group_color"].Value;
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
			form.AddField ("student_id", studentID);
			form.AddField ("game_id", gameID);
			CreatePostRequestSimpleJSON (SERVER_URL + "games/got/get_game_status", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {

					CerebroHelper.DebugLog ("GAME STATUS LOADED------------------------------------------");
					LaunchList.instance.mGameStatus.Clear ();
					//for (var i = 0; i < jsonResponse.Count; i++) {
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
							for(var i = 0; i < jsonResponse ["prev_game_data"]["game_result"].Count; i++)
							{
								GOTLeaderboard l = new GOTLeaderboard();
								l.GroupCoin = jsonResponse ["prev_game_data"]["game_result"][i]["coin_spent"].AsInt;
								l.GroupID = jsonResponse ["prev_game_data"]["game_result"][i]["group_color"].Value;
								l.GroupName = jsonResponse ["prev_game_data"]["game_result"][i]["group_name"].Value;
								l.GroupCell = jsonResponse ["prev_game_data"]["game_result"][i]["cells"].AsInt;
								status.GOTLeaderboard.Add(l);
							}
						}
						LaunchList.instance.mGameStatus.Add(status);
					//}
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
			gradeID = gradeID.ToLower ();
			CerebroHelper.DebugLog ("FETCH STUDENT DATA FOR " + studentID);
			WWWForm form = new WWWForm ();
			form.AddField ("student_id", studentID);
			CreatePostRequest (SERVER_URL + "student/get_student_data", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog (jsonResponse);
					JSONObject jsonObject = (JSONObject)jsonResponse;
					string name = "";
					int coins = 0;
					for (int i = 0; i < jsonObject.Count; i++) {
						string key = (string)jsonObject.keys [i];
						JSONObject stringObject = (JSONObject)jsonObject.list [i];

						if (key == "first_name") {
							name = stringObject.str + name;
						} else if (key == "last_name") {
							name += " " + stringObject.str;
						} else if (key == "mission" && stringObject.str != null) {
							PlayerPrefs.SetString (PlayerPrefKeys.MissionID, stringObject.str);
						} else if (key == "student_playlist") {
							LaunchList.instance.mCurrentStudent.ContentIDs = new SortedDictionary<int, string> ();
							for (int j = 0; stringObject.list != null && j < stringObject.list.Count; j++) {
								LaunchList.instance.mCurrentStudent.ContentIDs.Add (j, stringObject.list [j].str);
							}
						} else if (key == "profile_image" && stringObject.str != null) {
							PlayerPrefs.SetString (PlayerPrefKeys.ProfilePicKey, stringObject.str);
						} else if (key == "coins") {
							coins = Mathf.FloorToInt (stringObject.n);
						} else {
							CerebroHelper.DebugLog ("UNRECOGNIZED ATTRIB " + key);
						}
					}
					LaunchList.instance.mCurrentStudent.StudentID = studentID;
					LaunchList.instance.mCurrentStudent.GradeID = gradeID;
					LaunchList.instance.mCurrentStudent.StudentName = name;
					LaunchList.instance.mCurrentStudent.Coins = coins;

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
			CreatePostRequestSimpleJSON (SERVER_URL + "quiz/get_quiz_leader_board", form, (jsonResponse) => {
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
			CreatePostRequestSimpleJSON (SERVER_URL + "quiz/get_quiz", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.mQuizQuestions.Clear ();
					for (int i = 0; i < jsonResponse.Count; i++) {
						QuizData q = new QuizData ();
						q.QuizDate = jsonResponse [i] ["quiz_date"].Value;
						q.QuestionID = jsonResponse [i] ["question_id"].Value;
						q.AnswerOptions = jsonResponse [i] ["answer_options"].Value;
						q.CorrectAnswer = jsonResponse [i] ["correct_answer"].Value;
						q.Difficulty = jsonResponse [i] ["difficulty"].Value;
						q.QuestionIndex = jsonResponse [i] ["question_index"].Value;
						q.QuestionMedia = jsonResponse [i] ["question_media"].Value;
						q.QuestionMediaType = jsonResponse [i] ["question_media_type"].Value;
						q.QuestionText = jsonResponse [i] ["question_text"].Value;
						q.QuestionSubText = jsonResponse [i] ["question_sub_text"].Value;
						q.QuestionType = jsonResponse [i] ["question_type"].Value;
						q.AnswerType = jsonResponse [i] ["answer_type"].Value;
						q.AnswerURL = jsonResponse [i] ["answer_url"].Value;

						LaunchList.instance.mQuizQuestions.Add (q);
					}

					// sort based on question index
					int test;
					LaunchList.instance.mQuizQuestions.Sort (delegate(QuizData x, QuizData y) {
						if (int.TryParse (x.QuestionIndex, out test) || int.TryParse (y.QuestionIndex, out test)) {
							return int.Parse (x.QuestionIndex) - int.Parse (y.QuestionIndex);
						} else {
							return 0;
						}
					});

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
			CreatePostRequestSimpleJSON (SERVER_URL + "describe_image/get_describe_image", form, (jsonResponse) => {
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

		public void GetPracticeItems ()
		{
			string fileName = Application.persistentDataPath + "/PracticeItems.txt";
			try {

				StreamWriter sr = null;

//				if (startKey != null) {
//					request.ExclusiveStartKey = startKey;
//					sr = File.AppendText (fileName);
//				} else {
				sr = File.CreateText (fileName);														
//				}

				CreatePostRequestNoFormSimpleJSON (SERVER_URL + "practice_item/get_practice_items", (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {
						LaunchList.instance.mQuizAnalytics.Clear ();
						for (int i = 0; i < jsonResponse.Count; i++) {
							PracticeItems p = new PracticeItems ();
							p.PracticeID = jsonResponse [i] ["practice_id"].Value;
							p.DifficultyLevels = jsonResponse [i] ["difficulty_levels"].Value;
							p.Grade = jsonResponse [i] ["grade"].Value;
							p.PracticeItemName = jsonResponse [i] ["practice_item_name"].Value;
							p.RegenRate = jsonResponse [i] ["regen_rate"].Value;
							p.TotalCoins = jsonResponse [i] ["total_coins"].Value;
							p.Show = jsonResponse [i] ["show"].Value;
							char lastChar = p.PracticeItemName[p.PracticeItemName.Length - 1];
							int lastNumber = -1;
							if(!int.TryParse(lastChar+"", out lastNumber))
							{
								p.PracticeItemName += p.Grade;
							}

							if (LaunchList.instance.mPracticeItems.ContainsKey (p.PracticeID)) {
								p.CurrentCoins = LaunchList.instance.mPracticeItems [p.PracticeID].CurrentCoins;
								p.RegenerationStarted = LaunchList.instance.mPracticeItems [p.PracticeID].RegenerationStarted;
							} else {
								p.CurrentCoins = 0;
								p.RegenerationStarted = "";
							}

							sr.WriteLine (p.Grade + "," + p.PracticeID + "," + p.DifficultyLevels + "," + p.PracticeItemName + "," + p.RegenRate + "," + p.TotalCoins + "," + p.CurrentCoins + "," + p.RegenerationStarted + "," + p.Show);
						}

						sr.Close ();
//						if (result.Response.IsSetLastEvaluatedKey ()) {
//							CerebroHelper.DebugLog ("LOAD PARTLY DONE");
//							GetPracticeItems (result.Response.LastEvaluatedKey);
//						} else {
						CerebroHelper.DebugLog ("LOAD COMPLETE");
						LaunchList.instance.GotPracticeItems ();
//						}

					} else {
						CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					}
				});
			} catch (Exception e) {
				CerebroHelper.DebugLog ("Exception - GetPractice: " + e.Message);
			}
		}

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
					for (int i = 0; i < jsonResponse.Count; i++) {
						Feature f = new Feature ();
						f.FeatureDate = date;
						f.Grade = grade.ToString ();
						f.Type = jsonResponse [i] ["type"].Value;
						f.MediaUrl = jsonResponse [i] ["media_url"].Value;
						f.MediaText = jsonResponse [i] ["media_text"].Value;
						f.ImageUrl = jsonResponse [i] ["image_url"].Value;

						callback (f);
						break;
					}
					if (jsonResponse.Count == 0) {
						callback (null);
					}
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					callback (null);
				}
			});
		}

		public void GetMissionQuestions (string missionID)
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

		public void GetProperties ()
		{
			string fileName = Application.persistentDataPath + "/Properties.txt";
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			StreamWriter sr = null;

			sr = File.CreateText (fileName);

			try {
				WWWForm form = new WWWForm ();
				form.AddField ("student_id", studentID);
				CreatePostRequestSimpleJSON (SERVER_URL + "properties/get_properties", form, (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {
						for (int i = 0; i < jsonResponse.Count; i++) {
							Properties p = new Properties ();
							p.PropertyName = jsonResponse [i] ["property_name"].Value;
							p.PropertyValue = jsonResponse [i] ["property_value"].Value;

							sr.WriteLine (p.PropertyName + "," + p.PropertyValue);
						}
						sr.Close ();
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
			form.AddField ("image_id", imageID);
			CreatePostRequestSimpleJSON (SERVER_URL + "describe_image/get_english_image_response", form, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.mDescribeImageUserResponses.Clear ();
					for (int i = 0; i < jsonResponse.Count; i++) {
						DescribeImageUserResponse d = new DescribeImageUserResponse ();
						d.ImageID = jsonResponse [i] ["image_id"].Value;
						d.StudentID = jsonResponse [i] ["student_id"].Value;
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
			Dictionary<string,string> imageIDDict = LaunchList.instance.GetNextImageID ();
			string imageID = imageIDDict ["imageID"];
			bool fetchBool = imageIDDict ["fetchBool"] == "true" ? true : false;

			CerebroHelper.DebugLog ("getting image " + imageID);
			if (LaunchList.instance.CheckForSubmittedImageResponses (imageID) && !fetchBool) {
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
			string fileName = Application.persistentDataPath + "/Explanations.txt";

			StreamWriter sr = null;

			sr = File.CreateText (fileName);

			try {
				WWWForm form = new WWWForm ();
				CreatePostRequestSimpleJSON (SERVER_URL + "practice_item/get_practice_item_explanation", form, (jsonResponse) => {
					if (jsonResponse != null && jsonResponse.ToString () != "") {
						for (int i = 0; i < jsonResponse.Count; i++) {
							Explanation explanation = new Explanation ();
							explanation.PracticeItemID = jsonResponse [i] ["practice_item_id"].Value;
							explanation.Level = jsonResponse [i] ["difficulty"].Value;
							explanation.SubLevel = jsonResponse [i] ["sub_level"].Value;
							explanation.URL = jsonResponse [i] ["explanation_url"].Value;
							explanation.ContentId = jsonResponse [i] ["content_id"].Value;
							string key = explanation.PracticeItemID + "L" + explanation.Level + explanation.SubLevel;
							sr.WriteLine (explanation.PracticeItemID + "," + explanation.Level + "," + explanation.SubLevel + "," + explanation.URL);
							if (LaunchList.instance.mExplanation.ContainsKey (key)) {
								LaunchList.instance.mExplanation [key] = explanation;
							} else {
								LaunchList.instance.mExplanation.Add (key, explanation);
							}	
						}
						sr.Close ();
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
			CreatePostRequestNoFormSimpleJSON (SERVER_URL + "content_item/get_content_items", (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.ToString () != "") {
					LaunchList.instance.mContentItems = new List<ContentItem> ();
					for (int i = 0; i < jsonResponse.Count; i++) {
						ContentItem c = new ContentItem ();
						c.SubtopicID = getValue (jsonResponse [i] ["sub_topic_id"]);
						c.ContentID = getValue (jsonResponse [i] ["content_id"]);
						c.ContentName = getValue (jsonResponse [i] ["content_name"]);
						c.ContentDate = getValue (jsonResponse [i] ["content_date"]);
						c.ContentDescription = getValue (jsonResponse [i] ["content_description"]);
						c.ContentLink = getValue (jsonResponse [i] ["content_link"]);
						c.ContentType = getValue (jsonResponse [i] ["content_type"]);
						c.ContentDifficulty = getInt (jsonResponse [i] ["content_difficulty"]);
						c.ContentRating = getInt (jsonResponse [i] ["content_rating"]);
						c.ContentViews = getInt (jsonResponse [i] ["content_views"]);
						c.ContentTags = new List<string> ();
						for (int j = 0; j < jsonResponse ["content_tags"].Count; j++) {
							if (jsonResponse [i] ["content_tags"] [j].Value != null)
								c.ContentTags.Add (jsonResponse [i] ["content_tags"] [j].Value);
						}
						c.Userdata = getValue (jsonResponse [i] ["user_date"]);
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

		string getValue (JSONNode j)
		{
			if (j.Value != null)
				return j.Value;
			else
				return "";
		}

		int getInt (JSONNode j)
		{
			if (j.Value != null)
				return j.AsInt;
			else
				return 0;
		}

		//
		// PUT Requests
		//

		public void SendFlaggedData (string practiceItemId, string seed, int level, int selector)
		{
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			JSONNode N = JSONSimple.Parse ("{\"myData\"}");
			N ["myData"] ["component_data"] ["student_id"] = studentID;
			N ["myData"] ["component_data"] ["practice_item_id"] = practiceItemId;
			N ["myData"] ["component_data"] ["seed"] = seed;
			N ["myData"] ["component_data"] ["difficulty"] = level.ToString();
			N ["myData"] ["component_data"] ["sub_level"] = selector.ToString();

			N ["myData"] ["component_name"] = "flagged_question";
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
			N ["myData"] ["component_data"] ["image_id"] = ImageID;
			N ["myData"] ["component_data"] ["student_id"] = studentID;
			N ["myData"] ["component_data"] ["user_response"] = StringHelper.RemoveUnicodeCharacters (userResponse);
			N ["myData"] ["component_data"] ["grade"] = grade;
			N ["myData"] ["component_name"] = "describe_image_user_response";
			CerebroHelper.DebugLog (N ["myData"].ToString ());
			byte[] formData = System.Text.Encoding.ASCII.GetBytes (N ["myData"].ToString ().ToCharArray ());
			CreatePostRequestByteArray (SERVER_URL + "put_data/ins_data", formData, (jsonResponse) => {
				if (jsonResponse != null && jsonResponse.type != JSONObject.Type.NULL) {
					CerebroHelper.DebugLog ("Added new row");
					if (DescribeImageResponseSubmitted != null) {
						DescribeImageResponseSubmitted.Invoke (this, null);
					}
					LaunchList.instance.SetLastImageID (LaunchList.instance.mDescribeImage.ImageID, System.DateTime.Now.ToString ("yyyyMMdd"));
					LaunchList.instance.WriteImageResponseToFile (userResponse);
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
			N ["myData"] ["component_data"] ["student_id"] = studentID;
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
					LaunchList.instance.SetLastVerbalizeID (verb.VerbalizeID, System.DateTime.Now.ToString ("yyyyMMdd"));
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
					SubmitVerbalizeResponse(verb);
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
					LaunchList.instance.SentQuizAnalytics (studentAndQuestionID, date);
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

		public void PushMissionComplete (string missionID, string day, string timeStarted, string timeEnded, float accuracy)
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
					LaunchList.instance.UpdateLocalMissionFileForCompletion (accuracy, true);
				} else {
					CerebroHelper.DebugLog ("EXCEPTION GetItemAsync");
					LaunchList.instance.UpdateLocalMissionFileForCompletion (accuracy, false);
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
					CerebroAnalytics.instance.CleanLocalLogFile (list);
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
