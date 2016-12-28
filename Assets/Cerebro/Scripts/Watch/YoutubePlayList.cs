using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using SimpleJSON;
using System.Collections;

namespace Cerebro
{
	public class YoutubePlayList : MonoBehaviour 
	{
		public InputField searchField;
		public Text errorText;
		public YoutubeVideoSelector youtubeVideoSelector;



		private bool filterByDate = false;
		private bool useCategoryFilter =false;
		private string YourAPIKey = "AIzaSyDD-lxGLHsBIFPFPt2i31fc0tAHGeAb8mc";
		private string category="";
		private int maxresults = 50;
		private string nextPageToken =""; 
		private bool nextPageLoaded = false;
		public void Start()
		{
			searchField.onEndEdit.AddListener (SearchTextEnd);

			#if !UNITY_EDITOR
				searchField.placeholder.gameObject.SetActive (false);
				searchField.textComponent.gameObject.SetActive (false);
			#endif

			youtubeVideoSelector.nextPageLoaded = CallNextPage;
		}

		public void SearchTextEnd(string searchString)
		{
			searchField.text = searchString;
		}

		public void SearchPressed()
		{
			if (string.IsNullOrEmpty (searchField.text)) {
				return;
			}
			nextPageToken = null;
			Cerebro.LaunchList.instance.SendYoutubeAnalytics ("you_tube_search_log", System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss"), searchField.text, "", "", "");

			youtubeVideoSelector.youtubeVideosData = new List<VideoData> ();
			youtubeVideoSelector.Reload ();

			YoutubeV3Call (searchField.text);

		}

		public void CallNextPage(int dataIndex)
		{
			if (nextPageLoaded) {
				return;
			}
			if (dataIndex ==  youtubeVideoSelector.youtubeVideosData.Count-1) {
				nextPageLoaded = true;
				YoutubeV3Call (searchField.text);
			}
		}
			
		public void YoutubeV3Call(string searchString)
		{
			string newSearchString = searchString.Replace(" ", "%20");
			if (filterByDate) {
				if (useCategoryFilter)
					StartCoroutine (VideoSearchV3 ("https://www.googleapis.com/youtube/v3/search?q=" + newSearchString + "&key="+YourAPIKey+"&part=snippet,id&order=date&type=video&safeSearch=strict&category="+category+"&maxResults=" + maxresults + ""));
				else
					StartCoroutine (VideoSearchV3 ("https://www.googleapis.com/youtube/v3/search?q=" + newSearchString + "&key="+YourAPIKey+"&part=snippet,id&order=date&type=video&safeSearch=strict&maxResults=" + maxresults + ""));
			} else {
				if(useCategoryFilter)
					StartCoroutine (VideoSearchV3("https://www.googleapis.com/youtube/v3/search?q="+newSearchString+"&key="+YourAPIKey+"&part=snippet,id&type=video&safeSearch=strict&category="+category+"&maxResults="+maxresults+""));
				else
					StartCoroutine (VideoSearchV3("https://www.googleapis.com/youtube/v3/search?q="+newSearchString+"&key="+YourAPIKey+"&part=snippet,id&type=video&safeSearch=strict&maxResults="+maxresults+""));
			}

		}

		IEnumerator VideoSearchV3(string url)
		{
			if (!string.IsNullOrEmpty (nextPageToken)) {
				url += "&pageToken=" + nextPageToken;
			}
			Debug.Log (url);
			WWW call = new WWW (url);
			yield return call;
			if (call.text == "" || call.text == null) {
				errorText.text = "Something went wrong.";
				yield break;
			}
			Debug.Log (call.text);
			JSONNode youtubeReturn = JSONNode.Parse (call.text);
			nextPageToken = youtubeReturn["nextPageToken"].Value;
			youtubeReturn = youtubeReturn ["items"];
			Debug.Log (nextPageToken);
			for (int i = 0; i < youtubeReturn.Count; i++) {
				VideoData youtubeVideoData = new VideoData ();
				youtubeVideoData.videoId = youtubeReturn [i] ["id"] ["videoId"].Value;
				youtubeVideoData.thumbnailUrl = youtubeReturn[i]["snippet"]["thumbnails"]["default"]["url"].Value;
				youtubeVideoData.title = youtubeReturn[i]["snippet"]["title"].Value;
				youtubeVideoData.description = youtubeReturn [i] ["snippet"] ["description"].Value;
				youtubeVideoData.thumbnailSprite = null;
				youtubeVideoSelector.youtubeVideosData.Add (youtubeVideoData);
			}
			youtubeVideoSelector.Reload ();
			nextPageLoaded = false;
		}


	}
}
