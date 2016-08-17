using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public class VideoEventArgs : EventArgs
	{
		public string videoID;

		public float timeSpent;		// Total time spent watching the video -> does not include buffering time, pause time, etc.
		public string timeStarted;
		public string timeEnded;
		public float timeIni;
		public float timeEnd;

		public float videoLength;
	}

	public class VideoHelper : MonoBehaviour
	{
		private static VideoHelper m_Instance;
		public static VideoHelper instance {
			get {
				return m_Instance;
			}
		}

		private string videoID = "";
		private float timeSpent = 0f;
		private string timeStarted;
		private string timeEnded;
		private float timeIni;
		private float timeEnd;

		private bool videoPlaying = false;
		private bool videoEnded = false;

		private float videoLength;
		private string _fileName = "Cerebro/youtube.html";

		public EventHandler VideoEnded;

		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
		private UniWebView _webView;
		#endif

		#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8

		void Awake() {
			m_Instance = this;
		}
		public void OpenVideoWithUrl (string url)
		{
			if (_webView != null) {
				return;
			}

			CerebroHelper.DebugLog ("VideoEnded = " + VideoEnded);

			timeStarted = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");
			timeIni = Time.realtimeSinceStartup;

			videoEnded = false;
			videoPlaying = false;
			timeSpent = 0f;
			videoLength = -1f;

			var splitArr = url.Split (new string[] { "https://www.youtube.com/embed/" }, System.StringSplitOptions.None);
			if (splitArr.Length == 1) {
				splitArr = url.Split (new string[] { "https://www.youtube.com/watch?v=" }, System.StringSplitOptions.None);
			}
			if (splitArr.Length == 1) {
				return;
			}
			videoID = splitArr [1];

			_webView = CreateWebView ();
			_webView.url = UniWebViewHelper.streamingAssetURLForPath (_fileName);

			CerebroHelper.DebugLog (_webView.transform);

			int bottomInset = UniWebViewHelper.screenHeight;
			_webView.insets = new UniWebViewEdgeInsets (0, 0, 40, 0);

			_webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
			_webView.OnLoadComplete += _webView_OnLoadComplete;

			_webView.OnReceivedMessage += OnReceivedMessage;

			_webView.OnWebViewShouldClose += OnShouldClose;

			_webView.Load ();
			_webView.Show ();

		}

		bool OnShouldClose (UniWebView webView)
		{
			CerebroHelper.DebugLog ("ON SHOULD CLOSE");
			videoPlaying = false;
			CloseWebView ();
			return false;
		}

		void OnReceivedMessage (UniWebView webView, UniWebViewMessage message)
		{

			if (message.path == "close") {
				Destroy (_webView);
				_webView = null;
			}
			if (message.path == "ytParams") {
				string duration = message.args ["duration"];
				CerebroHelper.DebugLog (duration);
				videoLength = float.Parse (duration);
			}
			if (message.path == "ytEvent") {
				string state = message.args ["state"];
				CerebroHelper.DebugLog ("Player returned " + state);
				if (state == "playing") {
					videoPlaying = true;
				} else if (state == "buffering") {
					videoPlaying = false;
					#if UNITY_EDITOR
						StartCoroutine(DummyVideoEnd());		
					#endif
				} else if (state == "ended") {
					videoPlaying = false;
					videoEnded = true;
					CloseWebView ();
				} else if (state == "paused") {
					videoPlaying = false;
				}
			}
		}

		IEnumerator DummyVideoEnd() {
			yield return new WaitForSeconds (5f);
			videoPlaying = false;
			CloseWebView ();
		}

		void CloseWebView ()
		{
			timeEnded = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");
			timeEnd = Time.realtimeSinceStartup;
			Destroy (_webView);
			_webView = null;

			CerebroHelper.DebugLog ("VideoEnded = " + VideoEnded);
			if (VideoEnded != null) {
				VideoEventArgs videoEventArgs = new VideoEventArgs ();
				videoEventArgs.videoID = videoID;
				videoEventArgs.timeSpent = timeSpent;
				videoEventArgs.timeStarted = timeStarted;
				videoEventArgs.timeEnded = timeEnded;
				videoEventArgs.timeIni = timeIni;
				videoEventArgs.timeEnd = timeEnd;
				videoEventArgs.videoLength = videoLength;
				VideoEnded.Invoke (this, videoEventArgs);
			}
		}

		void OnEvalJavaScriptFinished (UniWebView webView, string r)
		{
			CerebroHelper.DebugLog ("Javascript eval finished with " + r);
		}

		void _webView_OnLoadComplete (UniWebView webView, bool success, string errorMessage)
		{
			_webView.EvaluatingJavaScript ("loadVideo('" + videoID + "')");
		}

		UniWebView CreateWebView ()
		{
			var webViewGameObject = GameObject.Find ("WebView");
			if (webViewGameObject == null) {
				webViewGameObject = new GameObject ("WebView");
				webViewGameObject.transform.SetParent (GameObject.Find ("Main Camera").transform.parent);
			}

			var webView = webViewGameObject.AddComponent<UniWebView> ();

			webView.toolBarShow = true;
			return webView;
		}
		#else
	public void OpenVideoWithUrl(string url) {
	}
	#endif

		void Update () {
			if (videoPlaying) {
				timeSpent += Time.deltaTime;
			}
		}
	}


}