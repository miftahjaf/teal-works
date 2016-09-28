//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MaterialUI;
using System.IO;

namespace Cerebro
{
	//	[AddComponentMenu("MaterialUI/Dialogs/Simple List", 1)]
	public class StudentPlaylist : MonoBehaviour
	{
		[SerializeField]
		private VerticalScrollLayoutElement m_ListScrollLayoutElement;
		public VerticalScrollLayoutElement listScrollLayoutElement
		{
			get { return m_ListScrollLayoutElement; }
			set { m_ListScrollLayoutElement = value; }
		}

		private List<StudentPlaylistOption> m_SelectionItems;
		public List<StudentPlaylistOption> selectionItems
		{
			get { return m_SelectionItems; }
		}

		private StudentPlaylistOptionDataList m_OptionDataList;
		public StudentPlaylistOptionDataList optionDataList
		{
			get { return m_OptionDataList; }
			set { m_OptionDataList = value; }
		}

		[SerializeField]
		private GameObject m_OptionTemplate;

		private List<Color> colors;

		public bool isAssigning = false;
		private List<string> assignedStudents;

		private string currentContentID;
		private int currentListIndex;

		private List<string> watchedVideos;

		private bool screenActive = true;
		private Dictionary<int,bool> textureLoaded = new Dictionary<int,bool>();
		private Dictionary<int,bool> textureLoading = new Dictionary<int,bool>();

		void OnEnable()
		{
			assignedStudents = new List<string> ();
		}

		public void Initialize(StudentPlaylistOptionDataList optionDataList)
		{

			m_OptionDataList = optionDataList;
			m_SelectionItems = new List<StudentPlaylistOption>();

			watchedVideos = WelcomeScript.instance.GetWatchedVideos ();


			for (int i = 0; i < m_OptionDataList.options.Count; i++)
			{
				m_SelectionItems.Add(CreateSubListItem(i,m_OptionDataList));
			}

			float availableHeight = DialogManager.rectTransform.rect.height;

			m_ListScrollLayoutElement.maxHeight = availableHeight - 68f;

			m_OptionTemplate.gameObject.SetActive(false);

			GetComponent<RectTransform>().sizeDelta = new Vector2(1024f,GetComponent<RectTransform>().sizeDelta.y);
			GetComponent<RectTransform> ().localPosition = new Vector3 (GetComponent<RectTransform> ().localPosition.x, 316f);
			LoadImages ();
//			Initialize();
		}

		public void OpenMissionVideo(MissionItemData missionItemData) {
			int openVideoIndex = -1;
			for (int i = 0; i < m_OptionDataList.options.Count; i++)
			{
				if (missionItemData != null && missionItemData.PracticeItemID == m_OptionDataList.options [i].id) {
					openVideoIndex = i;
				}
			}
			if (openVideoIndex != -1) {
				OnItemClick (openVideoIndex);
			} else {
				CerebroHelper.DebugLog ("openVideoIndex = -1");
			}
		}

		private StudentPlaylistOption CreateSubListItem(int i, StudentPlaylistOptionDataList tmpoptionDataList)
		{
			GameObject abc = Instantiate (m_OptionTemplate);
			abc.SetActive (true);
			StudentPlaylistOption option = abc.GetComponent<StudentPlaylistOption>();
			option.rectTransform.SetParent(m_OptionTemplate.transform.parent);
			option.rectTransform.localScale = Vector3.one;
			option.rectTransform.localEulerAngles = Vector3.zero;

			StudentPlaylistOptionData data = tmpoptionDataList.options[i];

			Text name = abc.GetChildByName<Text>("Name");
			Image watchedIcon = abc.transform.Find ("Watched").gameObject.GetComponent<Image> ();
			name.text = data.name;

			var str = data.Url;
			var firstSplit = str.Split ("v="[1]);
			var videoID = firstSplit [1].Split ("&"[0])[0];

			var videoUrl = "https://www.youtube.com/embed/" + videoID;
			var imgurl = "https://img.youtube.com/vi/" + videoID + "/default.jpg";

			if (watchedVideos.Contains (data.id)) {
				watchedIcon.color = new Color (watchedIcon.color.r, watchedIcon.color.g, watchedIcon.color.b, 1);
			} else {
				watchedIcon.color = new Color (watchedIcon.color.r, watchedIcon.color.g, watchedIcon.color.b, 0);
			}

			Image icon = abc.GetChildByName<Image>("Icon");
			icon.color = new Color (1, 1, 1, 0);

			option.Id = data.id;
			option.index = i;
			option.onClickAction += OnItemClick;
			option.PhotoUrl = imgurl;
			option.Url = videoUrl;

			return option;
		}

		IEnumerator LoadImages(List<StudentPlaylistOption> arr, int startIndex = 0, int lastIndex = -1) {
			if (lastIndex == -1 || lastIndex > arr.Count-1) {
				lastIndex = arr.Count-1;
			}
			for (var i = startIndex; i <= lastIndex; i++) {
				if ((textureLoaded.ContainsKey (i) && textureLoaded [i] == true) || (textureLoading.ContainsKey (i) && textureLoading [i] == true)) {
					CerebroHelper.DebugLog ("Already loaded");
				} else {
					CerebroHelper.DebugLog ("Loading image");

					if (textureLoading.ContainsKey (i))
						textureLoading [i] = true;
					else
						textureLoading.Add (i, true);
					
					Graphic graphic = arr [i].gameObject.GetChildByName<Graphic> ("Icon");
					var imgurl = arr [i].PhotoUrl;
					Texture2D tex = null;
					if (CerebroHelper.remoteWatchTextures.ContainsKey (imgurl)) {
						tex = CerebroHelper.remoteWatchTextures [imgurl];
						yield return new WaitForSeconds (0.2f);
					} else {
						WWW remoteImage = new WWW (imgurl);
						yield return remoteImage;
						if (remoteImage.error == null) {
							tex = remoteImage.texture;
							if (CerebroHelper.remoteWatchTextures.ContainsKey (imgurl))
							{
								CerebroHelper.remoteWatchTextures [imgurl] = tex;
							} 
							else
							{
								CerebroHelper.remoteWatchTextures.Add (imgurl, tex);
							}
						}
					}
					if (tex != null && screenActive) {
						var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
						graphic.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
						graphic.GetComponent<Image> ().sprite = newsprite;

						if (textureLoaded.ContainsKey (i))
							textureLoaded [i] = true;
						else
							textureLoaded.Add (i, true);
					}
				}
				textureLoading.Remove (i);
			}
		}

		public void OnItemClick(int index)
		{	
			currentContentID = m_SelectionItems [index].Id;
			currentListIndex = index;
			VideoHelper.instance.VideoEnded += CloseWebView;
			VideoHelper.instance.OpenVideoWithUrl (m_SelectionItems [index].Url);
		}

		void CloseWebView(object sender, System.EventArgs e) {

			CerebroHelper.DebugLog ("CLOSE WEBVIEW");
			VideoEventArgs eventArgs = e as VideoEventArgs;
			CerebroHelper.DebugLog (eventArgs);
			string day = System.DateTime.Now.ToUniversalTime().ToString ("yyyyMMdd");

			WelcomeScript.instance.ShowRatingPopup ("VIDEO", eventArgs.timeSpent,currentContentID, "How many stars would you give to this video?");

			var videoWatched = false;
			if (eventArgs.videoLength != -1 && eventArgs.timeSpent >= 0.8 * eventArgs.videoLength) {
				videoWatched = true;
				var watchedIcon = m_SelectionItems [currentListIndex].gameObject.transform.Find ("Watched").GetComponent<Image> ();
				watchedIcon.color = new Color (watchedIcon.color.r, watchedIcon.color.g, watchedIcon.color.b, 1);
			}

			var totalimeTaken = eventArgs.timeEnd - eventArgs.timeIni;
			string uniqueTime = eventArgs.timeEnded;
			string contentKey = "VIDEO_" + currentContentID + "Z" + uniqueTime;

			List<string> missionQuestionIds = CheckMissions (videoWatched);

			if (missionQuestionIds.Count != 0) {
				string missionString = LaunchList.instance.mMission.MissionID;
				foreach (var str in missionQuestionIds) {
					missionString = missionString + "@" + str;
				}
				Cerebro.LaunchList.instance.WriteAnalyticsToFile (contentKey, 0, videoWatched, day, eventArgs.timeStarted, Mathf.FloorToInt (totalimeTaken), Mathf.FloorToInt (eventArgs.timeSpent).ToString(), 0, missionString );  
			} else {
				Cerebro.LaunchList.instance.WriteAnalyticsToFile (contentKey, 0, videoWatched, day, eventArgs.timeStarted, Mathf.FloorToInt (totalimeTaken), Mathf.FloorToInt (eventArgs.timeSpent).ToString(), 0, " " );  
			}

			if (videoWatched) {
				string fileName = Application.persistentDataPath + "/WatchedVideos.txt";
				StreamWriter sr = null;
				if (File.Exists (fileName)) {
					sr = File.AppendText (fileName);
				} else {
					sr = File.CreateText (fileName);
				}
				sr.WriteLine (currentContentID);
				sr.Close ();
			}

			VideoHelper.instance.VideoEnded -= CloseWebView;
		}

		List<string> CheckMissions(bool isCorrect) {
			
			List<string> missionQuestionIDs = new List<string> ();

			if (LaunchList.instance.mMission.Questions != null) {
				foreach (var item in LaunchList.instance.mMission.Questions) {
					if (currentContentID == item.Value.PracticeItemID) {
						missionQuestionIDs.Add (item.Value.QuestionID);
						LaunchList.instance.UpdateLocalMissionFile (item.Value, item.Value.QuestionID, isCorrect);
					}
				}
			}
			return missionQuestionIDs;
		}

		public void LoadImages() {
			StartCoroutine (LoadImages (m_SelectionItems,0,5));
		}

		public List<string> GetAssignedStudentList() {
			return assignedStudents;
		}

		public void ListScrolled() {
			List<int> indices = new List<int> ();
			for (var i = 0; i < m_SelectionItems.Count; i++) {
				if (m_SelectionItems [i].gameObject.GetComponent<RectTransform> ().position.y > 0) {
					if (textureLoaded.ContainsKey (i) && textureLoaded[i] == true) {
						continue;
					}
					if (textureLoading.ContainsKey (i) && textureLoading[i] == true) {
						continue;
					}
					indices.Add (i);
				}
			}
			if (indices.Count > 0) {
				StartCoroutine (LoadImages (m_SelectionItems, indices [0], indices [indices.Count - 1]));
			}
		}
		public void DestroyingScreen() {
			screenActive = false;
			StopCoroutine (LoadImages(m_SelectionItems));
		}
	}
}