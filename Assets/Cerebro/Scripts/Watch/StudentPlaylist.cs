//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MaterialUI;
using System.IO;
using SimpleJSON;
using EnhancedUI.EnhancedScroller;
namespace Cerebro
{
	//	[AddComponentMenu("MaterialUI/Dialogs/Simple List", 1)]
	public class StudentPlaylist : MonoBehaviour,IEnhancedScrollerDelegate
	{

		public VideoCell videoCell;
		public List<VideoData> videosData;
		public EnhancedScroller videoSelector;

		private StudentPlaylistOptionDataList m_OptionDataList;
		public StudentPlaylistOptionDataList optionDataList
		{
			get { return m_OptionDataList; }
			set { m_OptionDataList = value; }
		}
			
		private string currentContentID;
		private int currentListIndex;

		private List<string> watchedVideos;

		private bool screenActive = true;
		private Dictionary<int,bool> textureLoaded = new Dictionary<int,bool>();
		private Dictionary<int,bool> textureLoading = new Dictionary<int,bool>();


		public void Initialize(StudentPlaylistOptionDataList optionDataList)
		{

			m_OptionDataList = optionDataList;
			videosData = new List<VideoData>();

			watchedVideos = WelcomeScript.instance.GetWatchedVideosJSON ();


			for (int i = 0; i < m_OptionDataList.options.Count; i++)
			{
				videosData.Add(CreateSubListItem(i,m_OptionDataList));
			}
				
			GetComponent<RectTransform>().sizeDelta = new Vector2(1024f,GetComponent<RectTransform>().sizeDelta.y);
			GetComponent<RectTransform> ().localPosition = new Vector3 (GetComponent<RectTransform> ().localPosition.x, 316f);
			videoSelector.Delegate = this;
			videoSelector.ReloadData ();
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

		#region IEnhancedScrollerDelegate implementation

		public int GetNumberOfCells (EnhancedScroller scroller)
		{
			if (videosData != null) {
				return videosData.Count;
			} else {
				return 0;
			}
		}

		public float GetCellViewSize (EnhancedScroller scroller, int dataIndex)
		{
			return 100f;
		}

		public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
		{
			VideoCell cellView = scroller.GetCellView (videoCell) as VideoCell;
			cellView.SetData (videosData[dataIndex],dataIndex,OnItemClick);
			return cellView;
		}

		#endregion

		private VideoData CreateSubListItem(int i, StudentPlaylistOptionDataList tmpoptionDataList)
		{
			VideoData videoData = new VideoData();
			StudentPlaylistOptionData data = tmpoptionDataList.options[i];
		
			var str = data.Url;
			var firstSplit = str.Split ("v="[1]);
			var videoID = firstSplit [1].Split ("&"[0])[0];
			var videoUrl = "https://www.youtube.com/embed/" + videoID;
			var imgurl = "https://img.youtube.com/vi/" + videoID + "/default.jpg";

			videoData.isWatched = watchedVideos.Contains (data.id);
			videoData.thumbnailSprite = null;
			videoData.title = data.name;
			videoData.videoId = videoUrl;
			videoData.thumbnailUrl = imgurl;
			videoData.videoUrl = videoUrl;
			videoData.contentId = data.id;
			return videoData;
		}

		/*IEnumerator LoadImages(List<StudentPlaylistOption> arr, int startIndex = 0, int lastIndex = -1) {
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
		}*/

		public void OnItemClick(int index)
		{	
			currentContentID = videosData [index].contentId;
			currentListIndex = index;
			VideoHelper.instance.VideoEnded += CloseWebView;
			VideoHelper.instance.OpenVideoWithUrl (videosData [index].videoUrl);
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
				videosData [currentListIndex].isWatched = true;
				videoSelector.RefreshActiveCellViews ();
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
				Cerebro.LaunchList.instance.WriteAnalyticsToFileJSON (contentKey, 0, videoWatched, day, eventArgs.timeStarted, Mathf.FloorToInt (totalimeTaken), Mathf.FloorToInt (eventArgs.timeSpent).ToString(), 0, missionString );  
			} else {
				Cerebro.LaunchList.instance.WriteAnalyticsToFileJSON (contentKey, 0, videoWatched, day, eventArgs.timeStarted, Mathf.FloorToInt (totalimeTaken), Mathf.FloorToInt (eventArgs.timeSpent).ToString(), 0, " " );  
			}

			if (videoWatched) {
				if (LaunchList.instance.mUseJSON) {
					string fileName = Application.persistentDataPath + "/WatchedVideosJSON.txt";
					if (File.Exists (fileName)) {
						string data = File.ReadAllText (fileName);
						JSONNode N = JSONClass.Parse ("{\"Data\"}");
						int cnt = 0;
						if (LaunchList.instance.IsJsonValidDirtyCheck (data)) {
							N = JSONClass.Parse (data);
							cnt = N ["Data"].Count;
						}
						N ["Data"] [cnt] ["VideoContentID"] = currentContentID;
						N ["VersionNumber"] = LaunchList.instance.VersionData;
						File.WriteAllText (fileName, string.Empty);
						File.WriteAllText (fileName, N.ToString());
					}
				} else {
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
			}

			VideoHelper.instance.VideoEnded -= CloseWebView;
		}

		List<string> CheckMissions(bool isCorrect) {
			
			List<string> missionQuestionIDs = new List<string> ();

			if (LaunchList.instance.mMission.Questions != null) {
				foreach (var item in LaunchList.instance.mMission.Questions) {
					if (currentContentID == item.Value.PracticeItemID) {
						missionQuestionIDs.Add (item.Value.QuestionID);
						LaunchList.instance.UpdateLocalMissionFileJSON (item.Value, item.Value.QuestionID, isCorrect);
					}
				}
			}
			return missionQuestionIDs;
		}



	
		public void DestroyingScreen() {
			screenActive = false;
			//StopCoroutine (LoadImages(m_SelectionItems));
		}
	}
}