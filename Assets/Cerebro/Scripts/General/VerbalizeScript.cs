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
		public Slider mSlider;
		public GameObject TranscriptView;

		[HideInInspector]
		public bool IsTranscriptToShown;

		private bool IsTextStartedMoving;
		private GameObject StartButton, PauseButton, StopButton, NumberText, VideoBG, TimeRemaining;
		private GameObject RecordingButton;

		private string TimeStarted;
		private float LastClickTimeSpeed, TotalHeight;
		private bool IsSpeedSelectorOpen, IsLandscapeLeft, IsStopEnabled, IsStartedOnce;
		private float[] SpeedRange;
		private int CurrSpeed;

		void Awake ()
		{
			//Screen.orientation = ScreenOrientation.Portrait;
		}

		// Use this for initialization
		void Start () 
		{			
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Verbalize);
			this.gameObject.name = "Verbalize";
			WelcomeScript.instance.HideDashboardIcon ();

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
			StopButton = transform.FindChild ("DoneButton").gameObject;
			NumberText = transform.FindChild ("NumberText").gameObject;
			VideoBG = transform.FindChild ("VideoBG").gameObject;
			TimeRemaining = transform.FindChild ("TimeRemaining").gameObject;
			RecordingButton = transform.FindChild ("RecordingButton").gameObject;

			SpeedRange = new float[5];
			SpeedRange[0] = 0.5f;
			SpeedRange[1] = 0.75f;
			SpeedRange[2] = 1.0f;
			SpeedRange[3] = 1.25f;
			SpeedRange[4] = 1.5f;
			speed = SpeedRange [PlayerPrefs.GetInt(PlayerPrefKeys.VerbalizeSpeed, 2)];
			CurrSpeed = PlayerPrefs.GetInt(PlayerPrefKeys.VerbalizeSpeed, 2);
			SpeedText.text = speed + "x";
			CheckForSpeedButtonEnable ();

			IsTextStartedMoving = false;
			NumberText.SetActive (false);

			if (!IsTranscriptToShown) {
				StartButton.SetActive (true);
				videoText.transform.parent.gameObject.SetActive (true);
				videoText.transform.FindChild("Title").GetComponent<Text> ().text = LaunchList.instance.mVerbalize.VerbTitle + "\n";
				videoText.transform.FindChild("Title").GetComponent<Text> ().text += "by " + LaunchList.instance.mVerbalize.VerbAuthor;
				videoText.GetComponent<Text> ().text = LaunchList.instance.mVerbalize.PromptText;
				WelcomeScript.instance.ShowGenericPopup ("Please rotate your device to portrait mode.", 1, true, RotationPopupOkPressed);
			} else {
				TranscriptView.SetActive (true);
				TranscriptView.transform.FindChild ("Content").FindChild("PromptText").GetComponent<Text>().text = LaunchList.instance.mVerbalize.PromptText;
				TranscriptView.transform.FindChild ("Content").FindChild("Title").GetComponent<Text>().text = LaunchList.instance.mVerbalize.VerbTitle + "\n";
				TranscriptView.transform.FindChild ("Content").FindChild("Title").GetComponent<Text>().text += "by " + LaunchList.instance.mVerbalize.VerbAuthor + "\n\n";
				StartCoroutine (SetSizeOfScrollRect());
			}
		}

		IEnumerator SetSizeOfScrollRect()
		{
			yield return 0;
			float height = 0;
			TranscriptView.transform.FindChild ("Content").FindChild ("Title").GetComponent<RectTransform> ().anchoredPosition = new Vector2 (TranscriptView.transform.FindChild ("Content").FindChild ("Title").GetComponent<RectTransform> ().anchoredPosition.x, -height);
			height += Mathf.Abs(TranscriptView.transform.FindChild ("Content").FindChild ("Title").GetComponent<RectTransform> ().rect.height);
			Debug.Log (height);
			TranscriptView.transform.FindChild ("Content").FindChild ("PromptText").GetComponent<RectTransform> ().anchoredPosition = new Vector2 (TranscriptView.transform.FindChild ("Content").FindChild ("PromptText").GetComponent<RectTransform> ().anchoredPosition.x, -height);
			height += Mathf.Abs(TranscriptView.transform.FindChild ("Content").FindChild ("PromptText").GetComponent<RectTransform> ().rect.height);
			Debug.Log (height);
			TranscriptView.transform.FindChild ("Content").FindChild ("End").GetComponent<RectTransform> ().anchoredPosition = new Vector2 (TranscriptView.transform.FindChild ("Content").FindChild ("End").GetComponent<RectTransform> ().anchoredPosition.x, -height);
			height += Mathf.Abs(TranscriptView.transform.FindChild ("Content").FindChild ("End").GetComponent<RectTransform> ().rect.height);
			Debug.Log (height);
			Debug.Log (TranscriptView.transform.FindChild ("Content").FindChild ("End").GetComponent<RectTransform> ().anchoredPosition);
			Debug.Log (TranscriptView.transform.FindChild ("Content").FindChild ("End").GetComponent<RectTransform> ().rect);
			TranscriptView.transform.FindChild ("Content").GetComponent<RectTransform> ().sizeDelta = new Vector2 (TranscriptView.transform.FindChild ("Content").GetComponent<RectTransform> ().sizeDelta.x, height);
		}

		public void RotationPopupOkPressed()
		{
			StartPreview ();
		}
		
		// Update is called once per frame
		void Update () 
		{
			CalculateRemainingTime ();
			if (IsTextStartedMoving) 
			{				
				//print (videoText.GetComponent<RectTransform> ().offsetMin.y);
				videoText.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (videoText.GetComponent<RectTransform> ().anchoredPosition.x, videoText.GetComponent<RectTransform> ().anchoredPosition.y + Time.deltaTime * 50f * speed);
				if (videoText.GetComponent<RectTransform> ().anchoredPosition.y > TotalHeight) {					
					StopRecording ();
				}
			}

			if (!IsStopEnabled) {
				if (videoText.GetComponent<RectTransform> ().anchoredPosition.y > videoText.GetComponent<RectTransform>().rect.height - videoText.transform.parent.GetComponent<RectTransform>().rect.height) {					
					EnableStopButton ();
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

		void CalculateRemainingTime()
		{
			float diff = TotalHeight - videoText.GetComponent<RectTransform> ().anchoredPosition.y;
			float seconds = diff / (50.0f * speed);
			TimeSpan t = TimeSpan.FromSeconds (seconds);
			TimeRemaining.GetComponent<Text> ().text = String.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
		}

		void EnableStopButton()
		{			
			StopButton.transform.FindChild ("ImageEnabled").gameObject.SetActive (true);
			StopButton.transform.FindChild ("Text").GetComponent<Text> ().color = new Color (1, 1, 1, 1);
			StopButton.GetComponent<Button> ().enabled = true;
			IsStopEnabled = true;
		}

		public void ChangeToLandscapeLeft()
		{
			Debug.Log ("into left "+IsLandscapeLeft);
			GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, 90f);
			Debug.Log ("into left curr "+transform.parent.GetComponent<RectTransform> ().eulerAngles);
			IsLandscapeLeft = true;
		}

		public void ChangeToLandscapeRight()
		{
			Debug.Log ("into right "+IsLandscapeLeft);
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

		void EnableRecordingButton()
		{
			RecordingButton.SetActive (true);
		}

		void DisableRecordingButton()
		{
			RecordingButton.SetActive (false);
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
			StopButton.SetActive (true);
			VideoBG.SetActive (true);
			TimeRemaining.SetActive (true);
			IsTextStartedMoving = true;
//			mSlider.gameObject.SetActive (true);
			SpeedText.gameObject.SetActive(true);
			EnableRecordingButton ();
			videoText.transform.FindChild ("End").GetComponent<RectTransform> ().anchoredPosition = new Vector2 (videoText.transform.FindChild("End").GetComponent<RectTransform>().anchoredPosition.x, -videoText.GetComponent<RectTransform>().rect.height);
			TotalHeight = videoText.GetComponent<RectTransform> ().rect.height + videoText.transform.FindChild ("End").GetComponent<RectTransform> ().rect.height;
			Debug.Log ("Height "+TotalHeight+" "+videoText.GetComponent<RectTransform> ().rect.height);
			TimeStarted = System.DateTime.Now.ToString ("yyyy-MM-ddTHH:mm:ss");
			IsStartedOnce = true;
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
		}

		void PauseText()
		{
			if (IsTextStartedMoving) {
				IsTextStartedMoving = false;
				DisableRecordingButton ();
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
				EnableRecordingButton ();
				PauseButton.transform.FindChild ("Text").GetComponent<Text> ().text = "Pause";
				#if UNITY_IOS && !UNITY_EDITOR
				_ResumeButton ("Resume");
				#endif
			}
		}

		public void StopRecording()
		{
			IsTextStartedMoving = false;
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
			DateTime stTime = DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbStartTime, "yyyy-MM-ddTHH:mm:ss", null);
			DateTime endTime = DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbEndTime, "yyyy-MM-ddTHH:mm:ss", null);
			float TimeSpent = (float)(endTime.Subtract(stTime).TotalSeconds);
			Debug.Log ("Total Time "+TimeSpent);
			WelcomeScript.instance.ShowRatingPopup ("VERBALIZE", TimeSpent,LaunchList.instance.mVerbalize.VerbalizeID, "How would you rate your speech?", PopupContinuePressed);
			GameObject Rating = GameObject.Find ("RatingPopup(Clone)");
			if (Rating != null) {
				Rating.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, 90);
			}
		}

		public void PopupContinuePressed()
		{
			HideScreen ();	
		}

		public void BackPressedOkButton()
		{
			#if UNITY_IOS && !UNITY_EDITOR
			_BackButton ("Back Pressed");
			#endif
			HideScreen ();
		}

		public void BackPressed ()
		{
			if (IsStartedOnce) {
				if (IsTextStartedMoving) {
					PauseText ();
				}
				WelcomeScript.instance.ShowGenericPopup ("Are you sure? Your current progress will be lost.", 2, true, BackPressedOkButton, null);
			} else {
				BackPressedOkButton ();
			}
		}

		void CheckForSpeedButtonEnable()
		{
			Color curr = SpeedText.transform.FindChild ("UpButton").FindChild ("Image").GetComponent<Image> ().color;
			SpeedText.transform.FindChild ("UpButton").FindChild ("Image").GetComponent<Image> ().color = new Color (curr.r, curr.g, curr.b, 1f);
			SpeedText.transform.FindChild ("DownButton").FindChild ("Image").GetComponent<Image> ().color = new Color (curr.r, curr.g, curr.b, 1f);
			if (CurrSpeed == 0) {
				SpeedText.transform.FindChild ("DownButton").FindChild ("Image").GetComponent<Image> ().color = new Color (curr.r, curr.g, curr.b, 0.5f);	
			} else if (CurrSpeed == 4) {
				SpeedText.transform.FindChild ("UpButton").FindChild ("Image").GetComponent<Image> ().color = new Color (curr.r, curr.g, curr.b, 0.5f);
			}
		}

		public void IncrementSpeed()
		{
			CurrSpeed++;
			CurrSpeed = Mathf.Clamp (CurrSpeed, 0, 4);
			speed = SpeedRange [CurrSpeed];
			PlayerPrefs.SetInt (PlayerPrefKeys.VerbalizeSpeed, CurrSpeed);
			SpeedText.text = speed + "x";
			CheckForSpeedButtonEnable ();
		}

		public void DecrementSpeed()
		{
			CurrSpeed--;
			CurrSpeed = Mathf.Clamp (CurrSpeed, 0, 4);
			speed = SpeedRange [CurrSpeed];
			PlayerPrefs.SetInt (PlayerPrefKeys.VerbalizeSpeed, CurrSpeed);
			SpeedText.text = speed + "x";
			CheckForSpeedButtonEnable ();
		}

		public void SetSpeed()
		{
			int value = (int)mSlider.value;
			speed = SpeedRange [value];
			PlayerPrefs.SetInt (PlayerPrefKeys.VerbalizeSpeed, value);
		}

		public void DashboardButtonPressed()
		{
			if (IsStartedOnce) {
				if (IsTextStartedMoving) {
					PauseText ();
				}
				WelcomeScript.instance.ShowGenericPopup ("Are you sure? Your current progress will be lost.", 2, true, DashboardPressedOkButton, null);
			} else {
				DashboardPressedOkButton ();
			}
		}

		public void DashboardPressedOkButton()
		{
			#if UNITY_IOS && !UNITY_EDITOR
			_BackButton ("Back Pressed");
			#endif
			WelcomeScript.instance.ShowScreen ();
		}

		void HideScreen()
		{
			WelcomeScript.instance.ShowDashboardIcon ();
			VerbalizeLandingPage page = gameObject.transform.parent.parent.GetComponent<VerbalizeLandingPage> ();
			page.BackOnScreen ();
			Destroy (gameObject);
		}

		void OnApplicationFocus( bool focusStatus )
		{
			if(!focusStatus)
			{
				PauseText ();
				CerebroHelper.DebugLog ("Going to background");
			}
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