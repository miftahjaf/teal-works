using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;

namespace Cerebro {
	public class VerbalizeScript : MonoBehaviour {

		public GameObject videoText, ProgressCircle, SpeedSelector;
		public Text SpeedText;
		public float speed = 1;

		private bool IsTextStartedMoving;
		private GameObject StartButton, PauseButton, StopButton, NumberText, VideoBG;

		private string TimeStarted;
		private float LastClickTimeSpeed;
		private bool IsSpeedSelectorOpen, IsLandscapeLeft;

		void Awake ()
		{
			//Screen.orientation = ScreenOrientation.Portrait;
		}

		// Use this for initialization
		void Start () 
		{			
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Verbalize);
			WelcomeScript.instance.IsVerbalizeStarted = true;
			this.gameObject.name = "Verbalize";

			GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
			GetComponent<RectTransform> ().sizeDelta = Vector2.zero;

			if (Screen.orientation == ScreenOrientation.LandscapeRight) {
				Debug.Log ("Calling right from start "+IsLandscapeLeft);
				ChangeToLandscapeRight ();
			} else {
				Debug.Log ("Calling left from start "+IsLandscapeLeft);
				ChangeToLandscapeLeft ();
			}

			StartButton = transform.FindChild ("StartButton").gameObject;
			PauseButton = transform.FindChild ("PauseButton").gameObject;
			NumberText = transform.FindChild ("NumberText").gameObject;
			VideoBG = transform.FindChild ("VideoBG").gameObject;

			IsTextStartedMoving = false;
			StartButton.SetActive (true);
			videoText.transform.FindChild("Title").GetComponent<Text> ().text = LaunchList.instance.mVerbalize.VerbTitle + "\n";
			videoText.transform.FindChild("Title").GetComponent<Text> ().text += "by " + LaunchList.instance.mVerbalize.VerbAuthor;
			videoText.GetComponent<Text> ().text += LaunchList.instance.mVerbalize.PromptText;
			StartPreview ();
		}
		
		// Update is called once per frame
		void Update () 
		{

			if (IsTextStartedMoving) 
			{
				//print (videoText.GetComponent<RectTransform> ().offsetMin.y);
				videoText.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (videoText.GetComponent<RectTransform> ().anchoredPosition.x, videoText.GetComponent<RectTransform> ().anchoredPosition.y + Time.deltaTime * 50f * speed);
				if (videoText.GetComponent<RectTransform> ().anchoredPosition.y > videoText.GetComponent<RectTransform>().rect.height) {
					IsTextStartedMoving = false;
					StopRecording ();
				}
			}

			if (IsLandscapeLeft && Screen.orientation == ScreenOrientation.LandscapeRight) {
				Debug.Log ("calling right from update "+IsLandscapeLeft);
				ChangeToLandscapeRight ();
				#if UNITY_IOS && !UNITY_EDITOR
				_RotationChanged ("false");
				#endif
			} else if(!IsLandscapeLeft && Screen.orientation == ScreenOrientation.LandscapeLeft) {
				Debug.Log ("calling left from update "+IsLandscapeLeft);
				ChangeToLandscapeLeft ();
				#if UNITY_IOS && !UNITY_EDITOR
				_RotationChanged ("true");
				#endif
			}
		
		}

		public void ChangeToLandscapeLeft()
		{
			Debug.Log ("into left "+IsLandscapeLeft);
			WelcomeScript.instance.dashboardIcon.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-1002f, -32f);
			GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, 90f);
			Debug.Log ("into left curr "+transform.parent.GetComponent<RectTransform> ().eulerAngles);
			IsLandscapeLeft = true;
		}

		public void ChangeToLandscapeRight()
		{
			Debug.Log ("into right "+IsLandscapeLeft);
			WelcomeScript.instance.dashboardIcon.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-22f, -736f);
			GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, -90f);
			Debug.Log ("into right curr "+transform.parent.GetComponent<RectTransform> ().eulerAngles);
			IsLandscapeLeft = false;
		}

		public void StartButtonPressed()
		{
			StartButton.SetActive (false);
			NumberText.SetActive (true);
			StartCoroutine (CountDownTimer ());
		}

		IEnumerator CountDownTimer()
		{
			yield return new WaitForSeconds (1f);
			NumberText.transform.FindChild ("Text").GetComponent<Text> ().text = "2";
			yield return new WaitForSeconds (1f);
			NumberText.transform.FindChild ("Text").GetComponent<Text> ().text = "1";
			yield return new WaitForSeconds (1f);
			NumberText.SetActive (false);
			PauseButton.SetActive (true);
			VideoBG.SetActive (true);
			IsTextStartedMoving = true;
			SpeedText.transform.parent.gameObject.SetActive (true);
			TimeStarted = System.DateTime.Now.ToString ("yyyy-MM-ddTHH:mm:ss");
			string CurrOrientationLeft = "true";
			if (Screen.orientation == ScreenOrientation.LandscapeRight)
				CurrOrientationLeft = "false";
			#if UNITY_IOS && !UNITY_EDITOR
			_StartButton (CurrOrientationLeft);
			#endif
		}

		public void PauseButtonPressed()
		{
			if (IsTextStartedMoving) {
				PauseText ();
			} else {
				ResumeText ();
			}
			if (IsSpeedSelectorOpen) {
				SetSpeed (0f);
			}
		}

		void PauseText()
		{
			if (IsTextStartedMoving) {
				IsTextStartedMoving = false;
				PauseButton.transform.FindChild ("Text").GetComponent<Text> ().text = "Resume";
				#if UNITY_IOS && !UNITY_EDITOR
				_PauseButton ("Pause");
				#endif
			}
		}

		void ResumeText()
		{
			if (!IsTextStartedMoving) {
				IsTextStartedMoving = true;
				PauseButton.transform.FindChild ("Text").GetComponent<Text> ().text = "Pause";
				#if UNITY_IOS && !UNITY_EDITOR
				_ResumeButton ("Resume");
				#endif
			}
		}

		public void StopRecording()
		{
			#if UNITY_IOS && !UNITY_EDITOR
			_StopButton ("Stop");
			#endif
			//System.DateTime timestarted = System.DateTime.ParseExact (TimeStarted, "yyyyMMddHHmmss", null);
			LaunchList.instance.mVerbalize.VerbStartTime = TimeStarted;
			LaunchList.instance.mVerbalize.VerbEndTime = System.DateTime.Now.ToString ("yyyy-MM-ddTHH:mm:ss");
			LaunchList.instance.mVerbalize.VerbSpeed = speed.ToString();
			LaunchList.instance.mVerbalize.UserSubmitted = false;
			LaunchList.instance.mVerbalize.UploadedToServer = false;
			LaunchList.instance.mVerbalize.UserResponseURL = "";
			LaunchList.instance.WriteVerbalizeResponseToFile (LaunchList.instance.mVerbalize);
			LaunchList.instance.VerbalizeSaving = true;
			#if UNITY_EDITOR
			WelcomeScript.instance.GetSavedVideoPath("Temp path");
			#endif
			HideScreen ();
		}

		public void BackPressed ()
		{
			#if UNITY_IOS && !UNITY_EDITOR
			_BackButton ("Back Pressed");
			#endif
			HideScreen ();
		}

		public void EnableSpeedSelector()
		{
			if (Time.time - LastClickTimeSpeed > 0.5f && !IsSpeedSelectorOpen) {
				PauseText ();
				SpeedText.gameObject.SetActive (false);
				LastClickTimeSpeed = Time.time;
				IsSpeedSelectorOpen = true;
				SpeedSelector.transform.localPosition = new Vector2 (-70f, -200f);
				Go.to (SpeedSelector.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (-70f, -45f), false));
			}
		}

		public void SetSpeed(float value)
		{
			if (IsSpeedSelectorOpen) {
				ResumeText ();
				IsSpeedSelectorOpen = false;
				SpeedText.gameObject.SetActive (true);
				if (value != 0) {
					SpeedText.text = value + "x";
					speed = value;
				}
				LastClickTimeSpeed = Time.time;
				SpeedSelector.transform.localPosition = new Vector2 (-70f, -45f);
				Go.to (SpeedSelector.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (-70f, -200f), false));
			}
		}

		void HideScreen()
		{
			WelcomeScript.instance.IsVerbalizeStarted = false;
			WelcomeScript.instance.dashboardIcon.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (-40f, -32f);
			VerbalizeLandingPage page = gameObject.transform.parent.parent.GetComponent<VerbalizeLandingPage> ();
			page.BackOnScreen ();
			//WelcomeScript.instance.ShowScreen (false);
			Destroy (gameObject);
		}

		public void StartPreview()
		{
			string CurrOrientationLeft = "true";
			if (Screen.orientation == ScreenOrientation.LandscapeRight)
				CurrOrientationLeft = "false";
			#if UNITY_IOS && !UNITY_EDITOR
			_StartPreview (CurrOrientationLeft);
			#endif
		}

		public void StartRecording(string text)
		{
			print ("Starting Recording");
			IsTextStartedMoving = true;
		}

//		public void GetSavedVideoPath(string path)
//		{
//			print ("got back from native with path "+path);
//			HTTPRequestHelper.instance.uploadProfileVid ("vid.mov", path);
//		}

		[DllImport ("__Internal")]
		private static extern void _StartButton (
			string message);

		[DllImport ("__Internal")]
		private static extern void _StartPreview (
			string message);

		[DllImport ("__Internal")]
		private static extern void _PauseButton (
			string message);

		[DllImport ("__Internal")]
		private static extern void _ResumeButton (
			string message);

		[DllImport ("__Internal")]
		private static extern void _StopButton (
			string message);

		[DllImport ("__Internal")]
		private static extern void _BackButton (
			string message);

		[DllImport ("__Internal")]
		private static extern void _RotationChanged (
			string message);
	}
}