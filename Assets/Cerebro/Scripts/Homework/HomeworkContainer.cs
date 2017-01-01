using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Cerebro
{
	public class HomeworkContainer : MonoBehaviour 
	{
		public GameObject NoFeedAvailable;

		[SerializeField]
		private GameObject ProgressCircle;

		private HomeworkSelector homeworkSelector;

		private HomeworkData hwData;
		private bool isMoreFeed;
		private int currPage = 0;
		private int countPerPage = 10;

		private string timeFilter, statusFilter, subjectFilter;

		public void Start()
		{
			homeworkSelector = GetComponentInChildren<HomeworkSelector> ();
			hwData = new HomeworkData ();
			ProgressCircle.SetActive (true);
			isMoreFeed = true;
			NoFeedAvailable.SetActive (false);
			LoadNextPage ();
		}

		public void RefreshFeed()
		{
			hwData = new HomeworkData ();
			ProgressCircle.SetActive (true);
			isMoreFeed = true;
			NoFeedAvailable.SetActive (false);
			string studentId = PlayerPrefs.GetString (PlayerPrefKeys.IDKey, "1");
			HTTPRequestHelper.instance.GetHomeworkFeed (studentId, countPerPage * currPage, 1, OnGotResponseFeed);
		}

		public void LoadNextPage()
		{
			Debug.Log (isMoreFeed+" "+currPage);
			if (!isMoreFeed) {
				homeworkSelector.isHomeworkFeedLoading = false;
				return;
			}
			currPage++;
			string studentId = PlayerPrefs.GetString (PlayerPrefKeys.IDKey, "1");
			HTTPRequestHelper.instance.GetHomeworkFeed (studentId, countPerPage, currPage, OnGotResponseFeed);
		}

		public void OnGotResponseFeed(JSONNode currJson)
		{
			if (currJson != null) {				
				isMoreFeed = currJson ["feed_data"] ["is_more_feed"].AsBool;
				hwData.FillData (currJson);
				if (hwData.dataList.Count > 0) {
					UpdateHomeworkListData ();
					SaveHomeworkFeedToLocal ();
				} else {
					NoFeedAvailable.SetActive (true);
				}
			} else {
				isMoreFeed = false;
				LoadHomeworkFeedFromLocal ();
				if (hwData.dataList.Count > 0) {
					UpdateHomeworkListData ();
				} else {
					NoFeedAvailable.SetActive (true);
				}
			}
			homeworkSelector.isHomeworkFeedLoading = false;
			ProgressCircle.SetActive (false);
		}

		void SaveHomeworkFeedToLocal()
		{
			string fileName = Application.persistentDataPath + "/HomeworkFeedJSON.txt";
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (System.IO.File.Exists (fileName)) {				
				string data = System.IO.File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				System.IO.File.WriteAllText (fileName, string.Empty);
			}
			int cnt = hwData.dataList.Count;
			for (int i = 0; i < cnt; i++) {
				N ["Data"] ["Homeworks"] [hwData.dataList [i].id] ["Feed"] = hwData.dataList [i].ConvertFeedToJson ();
			}
			System.IO.File.WriteAllText (fileName, N.ToString());
		}

		void LoadHomeworkFeedFromLocal()
		{
			string fileName = Application.persistentDataPath + "/HomeworkFeedJSON.txt";
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (System.IO.File.Exists (fileName)) {				
				string data = System.IO.File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				if(N ["Data"] ["Homeworks"] != null)
				{
					int cnt = N ["Data"] ["Homeworks"].Count;
					for (int i = 0; i < cnt; i++) {
						HomeworkDataCell dataCell = new HomeworkDataCell ();
						if (N ["Data"] ["Homeworks"] [i] ["Feed"] != null) {
							dataCell.FillFeedDataFromLocal (N ["Data"] ["Homeworks"] [i] ["Feed"]);
						}
						if (N ["Data"] ["Homeworks"] [i] ["ResponseFeed"] != null) {
							dataCell.FillResponseDataFromLocal (N ["Data"] ["Homeworks"] [i] ["ResponseFeed"]);
						}
						hwData.dataList.Add (dataCell);
					}
				}
			}
			LoadOfflineResponsesFromLocal ();
		}

		void LoadOfflineResponsesFromLocal()
		{
			string fileName = Application.persistentDataPath + "/HomeworkFeedJSON.txt";
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (System.IO.File.Exists (fileName)) {				
				string data = System.IO.File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				int cnt = 0;
				if(N ["Data"] ["Response"] != null) {
					cnt = N ["Data"] ["Response"].Count;
					for (int i = 0; i < cnt; i++) {
						string id = N ["Data"] ["Response"] [i] ["id"].Value;
						JSONNode res = N ["Data"] ["Response"] [i];
						HomeworkDataCell dataCell = hwData.dataList.Find (x => x.contextId == res ["contextId"].Value);
						if (dataCell != null) {
							dataCell.wcData.userResponses.Add (new WCResponse (id, res ["responseData"].Value, false, LaunchList.instance.ConvertStringToStandardDate(res ["createdAt"].Value)));
						}
					}
				}
				if(N ["Data"] ["Comment"] != null) {
					cnt = N ["Data"] ["Comment"].Count;
					for (int i = 0; i < cnt; i++) {
						string id = N ["Data"] ["Comment"] [i] ["id"].Value;
						JSONNode cmt = N ["Data"] ["Comment"] [i];
						HomeworkDataCell dataCell = hwData.dataList.Find (x => x.contextId == cmt ["contextId"].Value);
						if (dataCell != null) {
							dataCell.comments.Add (new HomeworkComment (id, cmt ["commentData"].Value, LaunchList.instance.ConvertStringToStandardDate(cmt ["createdAt"].Value), cmt ["teacherId"].Value, HomeworkComment.CommentFrom.Me));
						}
					}
				}
				if(N ["Data"] ["Announcement"] != null) {
					cnt = N ["Data"] ["Announcement"].Count;
					for (int i = 0; i < cnt; i++) {
						string id = N ["Data"] ["Announcement"] [i] ["id"].Value;
						JSONNode ann = N ["Data"] ["Announcement"] [i];
						HomeworkDataCell dataCell = hwData.dataList.Find (x => x.contextId == ann ["contextId"].Value);
						if (dataCell != null) {
							dataCell.announcementData.isRead = true;
							dataCell.announcementData.uploadedToServer = false;
						}
					}
				}
			}
		}

		public void ClearFilters()
		{
			timeFilter = "all";
			statusFilter = "all";
			subjectFilter = "all";
		}

		public void UpdateHomeworkListData()
		{
//			homeworkSelector.currHomeworks = hwData.dataList;
			string[] allStatus = new string[] { "open", "late", "closed" };
			homeworkSelector.currHomeworks = new List<HomeworkCell> ();
			for (int i = 0; i < allStatus.Length; i++) {
				List<HomeworkDataCell> dataCells = hwData.dataList.FindAll (x=>x.status.ToString() == allStatus[i]);
				if (dataCells.Count > 0) {
					homeworkSelector.currHomeworks.Add (new HomeworkTitleCell (allStatus[i]));
					foreach (HomeworkDataCell dataCell in dataCells) {
						homeworkSelector.currHomeworks.Add (dataCell);
					}
				}
			}
			homeworkSelector.ReloadData ();	
		}

		public void OnBackPressed()
		{
			WelcomeScript.instance.ShowScreen (false);
			Destroy (gameObject);
		}

	}
}
