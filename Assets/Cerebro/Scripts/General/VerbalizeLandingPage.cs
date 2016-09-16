using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.EventSystems;
using System;
using System.Runtime.InteropServices;

namespace Cerebro
{
	public class VerbalizeLandingPage : MonoBehaviour
	{
		private System.DateTime CurrentDate;
		private System.DateTime OldestDate;
		private System.DateTime TodayDate;

		public GameObject VerbalizeContainer;
		public GameObject landingPage;
		public GameObject progressCircle;
		public GameObject prevDateBtn;
		public GameObject nextDateBtn;
		public GameObject fetchQuestionBtn;

		public GameObject Card;

		public Text currentDate;
		public Text nextDate;
		private bool isAnimating = false;

		public Text message;
		public Text cardTitle;
		public GameObject cardIcon;
		public GameObject startButton;
		public GameObject ViewRetakeButton;
		public GameObject ViewButton;
		public GameObject TranscriptButton;

		private DateTime CurrDateTime;

		private bool isTestUser = false;

		private Verbalize mQuestion;

		// Use this for initialization
		void Start ()
		{
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Verbalize);

			this.gameObject.name = "VerbalizeLandingPage";

			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			GetComponent<RectTransform> ().position = new Vector3 (0, 0);

			fetchQuestionBtn.SetActive (false);

			isTestUser = CerebroHelper.isTestUser ();

			OldestDate = System.DateTime.ParseExact ("20160412", "yyyyMMdd", null);
			TodayDate = System.DateTime.Now;

			CurrentDate = TodayDate;
			SetDate (false);
			message.text = "";
			Card.SetActive (true);

			ManageCardDataForDate (CurrentDate, true);
		}

		public void nextDatePressed ()
		{
			if (isAnimating) {
				return;
			}
			if (CurrentDate.ToString ("yyyyMMdd") != TodayDate.ToString ("yyyyMMdd") || isTestUser) {
				CurrentDate = CurrentDate.AddDays (1);

				Card.SetActive (true);
				cardIcon.SetActive (false);

				SetDate (true, false);

				ManageCardDataForDate (CurrentDate);
			}
		}

		public void previousDatePressed ()
		{
			if (isAnimating) {
				return;
			}
			if (CurrentDate.ToString ("yyyyMMdd") != OldestDate.ToString ("yyyyMMdd") || isTestUser) {
				CurrentDate = CurrentDate.AddDays (-1);
				Card.SetActive (true);
				cardIcon.SetActive (false);

				SetDate (true, false);

				ManageCardDataForDate (CurrentDate);
			}
		}

		public void GotThumbnailImage(string ImagePath)
		{
			//StartCoroutine (SetThumbnail(ImagePath));
		}

		public void ManageCardDataForDate(DateTime currdate, bool autoFetching = false)
		{
			#if UNITY_IOS && !UNITY_EDITOR
			_GetThumbnail("Temp Path");
			#endif
			DisableAllButtons ();
			if (LaunchList.instance.VerbalizeSaving) {
				message.text = "Please Wait.";
				cardTitle.text = "Saving Recording";

				progressCircle.SetActive (false);
				fetchQuestionBtn.SetActive (false);
				return;
			}
			string date = currdate.ToString ("yyyyMMdd");
			Verbalize Verb = LaunchList.instance.CheckForSubmittedVerbalizeViaDate (date);
			LaunchList.instance.mVerbalize = Verb;
			if (Verb != null && Verb.PromptText != "") {
				Debug.Log (Verb.VerbalizeDate + " " + Verb.UserSubmitted + " " + Verb.UserResponseURL + " " + Verb.UploadedToServer);
				if (Verb.UserResponseURL == "") {
					message.text = "Press the Start button to record your speech.";
					cardTitle.text = "Read out aloud!";
					cardIcon.SetActive (true);
					progressCircle.SetActive (false);
					startButton.SetActive (true);
				} else if (!Verb.UserSubmitted) {
					message.text = "";
					cardTitle.text = "View Recording";

					LaunchList.instance.mVerbalize = Verb;
					cardIcon.SetActive (false);
					ViewButton.SetActive (true);
					ViewRetakeButton.SetActive (true);
					progressCircle.SetActive (false);
					fetchQuestionBtn.SetActive (false);
				} else {
					message.text = "";
					cardTitle.text = "View Recording";

					LaunchList.instance.mVerbalize = Verb;
					cardIcon.SetActive (false);
					ViewButton.SetActive (true);
					TranscriptButton.SetActive (true);
					progressCircle.SetActive (false);
					fetchQuestionBtn.SetActive (false);
				}
			} else {
				if (autoFetching) {
					FetchQuestionForDate (currdate);
				} else {
					message.text = "";
					cardTitle.text = "Fetch Verbalize";

					startButton.SetActive (false);
					progressCircle.SetActive (false);
					fetchQuestionBtn.SetActive (true);
				}
			}
		}


		private void SetDate (bool shouldAnimate, bool animateLeft = false)
		{
			if (!shouldAnimate) {
				currentDate.text = CurrentDate.ToString ("MMM dd, yyyy");
			} else {
				nextDate.text = CurrentDate.ToString ("MMM dd, yyyy");
				StartCoroutine (AnimateDate (animateLeft));
			}

			if (CurrentDate.ToString ("yyyyMMdd") == OldestDate.ToString ("yyyyMMdd") && !isTestUser) {
				prevDateBtn.SetActive (false);
				nextDateBtn.SetActive (true);
			} else if (CurrentDate.ToString ("yyyyMMdd") == TodayDate.ToString ("yyyyMMdd") && !isTestUser) {
				nextDateBtn.SetActive (false);
				prevDateBtn.SetActive (true);
			} else {
				nextDateBtn.SetActive (true);
				prevDateBtn.SetActive (true);
			}
			if (LaunchList.instance.VerbalizeSaving) {
				nextDateBtn.SetActive (false);
				prevDateBtn.SetActive (false);
			}
		}

		IEnumerator AnimateDate (bool animateLeft)
		{
			isAnimating = true;
			if (animateLeft) {
				nextDate.transform.localPosition = new Vector3 (-1024, nextDate.transform.localPosition.y, nextDate.transform.localPosition.z);
				Go.to (currentDate.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (1024, 0, 0), true));
				Go.to (nextDate.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (1024, 0, 0), true));
			} else {
				nextDate.transform.localPosition = new Vector3 (1024, nextDate.transform.localPosition.y, nextDate.transform.localPosition.z);
				Go.to (currentDate.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (-1024, 0, 0), true));
				Go.to (nextDate.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (-1024, 0, 0), true));
			}
			yield return new WaitForSeconds (0.5f);
			Text tmp = currentDate;
			currentDate = nextDate;
			nextDate = tmp;
			isAnimating = false;
		}

		public void FetchQuestionButtonPressed ()
		{
			FetchQuestionForDate (CurrentDate);
		}

		private void FetchQuestionForDate (DateTime date)
		{
			DisableAllButtons ();
			Verbalize Verb = LaunchList.instance.CheckForSubmittedVerbalizeViaDate (date.ToString ("yyyyMMdd"));
			if (Verb != null) {
				Debug.Log (Verb.VerbalizeDate + " " + Verb.UserSubmitted + " " + Verb.UserResponseURL + " " + Verb.UploadedToServer);
				if (Verb.UserResponseURL == "") {
					progressCircle.SetActive (false);
					message.text = "Please wait.";
					cardTitle.text = "Saving Recording";
					cardIcon.SetActive (false);
					Card.SetActive (true);
					return;
				}
			} else {
				nextDateBtn.SetActive (false);
				prevDateBtn.SetActive (false);
				progressCircle.SetActive (true);
				message.text = "";
				cardTitle.text = "Fetching Verbalize";
				cardIcon.SetActive (false);
				Card.SetActive (true);

				LaunchList.instance.VerbalizeTextLoaded += VerbalizeTextLoaded;
				HTTPRequestHelper.instance.GetVerbalizeForDate (date.ToString ("MMdd"));
			}
		}

		public void TranscriptButtonPressed()
		{
			GameObject Verbalize = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Verbalize, VerbalizeContainer.transform);
			Verbalize.GetComponent<VerbalizeScript> ().IsTranscriptToShown = true;
			landingPage.SetActive (false);
		}

		public void ViewButtonPressed()
		{			
			CerebroHelper.DebugLog ("Viewing "+(LaunchList.instance.mCurrLocalTempPath+LaunchList.instance.mVerbalize.UserResponseURL));
			Debug.Log ("Viewing "+LaunchList.instance.mCurrLocalTempPath+" "+LaunchList.instance.mVerbalize.UserResponseURL);
			#if UNITY_IOS && !UNITY_EDITOR
			if(LaunchList.instance.mVerbalize.UploadedToServer)
				Handheld.PlayFullScreenMovie (LaunchList.instance.mVerbalize.UserResponseURL);
			else
			{
				String currPath = LaunchList.instance.mCurrLocalTempPath+LaunchList.instance.mVerbalize.UserResponseURL;
//				FileInfo info = new FileInfo(currPath);
//				if (info == null || info.Exists == false) {
//					Debug.Log("File not exists for "+LaunchList.instance.mVerbalize.VerbalizeDate);
//					CurrDateTime = DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbalizeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
//					LaunchList.instance.DeleteVerbalize (LaunchList.instance.mVerbalize.VerbalizeID);
//					WelcomeScript.instance.ShowGenericPopup ("Your recording was deleted from device. Please record again.", 1, false, VideoDeletedOkButton);
//				} else {
//				Handheld.PlayFullScreenMovie ("file://"+currPath);
//				}
				Handheld.PlayFullScreenMovie ("file://"+currPath);
			}
			#endif
			StartCoroutine (DisableProgressCircle ());
		}

		public void VideoDeletedOkButton()
		{
			ManageCardDataForDate (CurrDateTime);
		}

		IEnumerator DisableProgressCircle()
		{
			progressCircle.SetActive (true);
			yield return 0;
			yield return 0;
			progressCircle.SetActive (false);
		}

		public void SubmitButtonPressed()
		{
			LaunchList.instance.mVerbalize.UserSubmitted = true;
			LaunchList.instance.WriteVerbalizeResponseToFile (LaunchList.instance.mVerbalize);
			if (LaunchList.instance.UploadingVerbalize == null)
				LaunchList.instance.UploadingVerbalize = new List<Verbalize> ();
			LaunchList.instance.UploadingVerbalize.Add (LaunchList.instance.mVerbalize);
			#if UNITY_EDITOR
			HTTPRequestHelper.instance.SubmitVerbalizeResponse(LaunchList.instance.mVerbalize);
			#endif
			#if UNITY_IOS && !UNITY_EDITOR
			HTTPRequestHelper.instance.uploadProfileVid ("vid.mov", LaunchList.instance.mCurrLocalTempPath + LaunchList.instance.mVerbalize.UserResponseURL, LaunchList.instance.mVerbalize);
			#endif
			if (LaunchList.instance.mVerbalize != null) {
				DateTime dt = DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbalizeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
				ManageCardDataForDate (dt);
			}
		}

		public void RefreshPage(string date)
		{
			if (date == CurrentDate.ToString("yyyyMMdd")) {
				VerbalizeTextLoaded (null, null);
			}
		}

		void DisableAllButtons()
		{
			startButton.SetActive (false);
			fetchQuestionBtn.SetActive (false);
			ViewRetakeButton.SetActive (false);
			ViewButton.SetActive (false);
			TranscriptButton.SetActive (false);
		}

		public void VerbalizeTextLoaded (object sender, EventArgs e)
		{
			if (this == null || this.gameObject == null) {
				return;
			}

			Verbalize Verb = LaunchList.instance.CheckForSubmittedVerbalizeViaDate (LaunchList.instance.mVerbalize.VerbalizeDate);
			if (Verb == null) {
				mQuestion = LaunchList.instance.mVerbalize;	
			} else {
				mQuestion = Verb;
			}
			CerebroHelper.DebugLog (mQuestion.VerbalizeDate + " " + mQuestion.UserSubmitted + " " + mQuestion.UserResponseURL + " " + mQuestion.UploadedToServer);

			SetDate (false);

			if (mQuestion == null) {
				message.text = "You need internet to use this feature. Press the fetch button to try again";
				progressCircle.SetActive (false);
				DisableAllButtons ();
				fetchQuestionBtn.SetActive (true);
			} else if (mQuestion.VerbalizeID == null) {
				message.text = "Oh no! No Verbalize has been prepared yet. Check back later";
				progressCircle.SetActive (false);
				DisableAllButtons ();
				fetchQuestionBtn.SetActive (true);
				cardTitle.text = "Fetch Verbalize";
				cardIcon.SetActive (true);
				return;
			} else {
				message.text = "Press the Start button to record your speech.";
				cardTitle.text = "Read out aloud!";
				cardIcon.SetActive (true);
				progressCircle.SetActive (false);
				DisableAllButtons ();
				startButton.SetActive (true);
			}
		}

		IEnumerator SetThumbnail(string url)
		{
			CerebroHelper.DebugLog ("Getting Image");
			WWW remoteImage = new WWW (url);
			yield return remoteImage;
			CerebroHelper.DebugLog ("Got Image");
			if (remoteImage.error == null) {
				var newsprite = Sprite.Create (remoteImage.texture, new Rect (0f, 0f, remoteImage.texture.width, remoteImage.texture.height), new Vector2 (0.5f, 0.5f));
				Card.transform.FindChild ("Temp").GetComponent<Image> ().color = new Color (1, 1, 1, 1);
				Card.transform.FindChild ("Temp").GetComponent<Image> ().sprite = newsprite;
			} else {
				Debug.Log ("Error "+remoteImage.error);
			}
		}

		public void BackPressed ()
		{
			LaunchList.instance.VerbalizeTextLoaded -= VerbalizeTextLoaded;
			WelcomeScript.instance.ShowScreen (false);
			Destroy (gameObject);	
		}

		public void RetakeButtonPressed()
		{
			WelcomeScript.instance.ShowGenericPopup ("Are you sure? Your last recording will be deleted.", 2, false, StartQuestionPressed, null);	
		}

		public void StartQuestionPressed ()
		{
			GameObject Verbalize = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Verbalize, VerbalizeContainer.transform);
			landingPage.SetActive (false);
		}

		public void BackOnScreen ()
		{
			landingPage.SetActive (true);
			if (LaunchList.instance.mVerbalize != null) {
				DateTime dt = DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbalizeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
				ManageCardDataForDate (dt);
			}

			transform.localPosition = new Vector2 (transform.localPosition.x, -1152f);
			Go.to (transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (-512f, -384f), false));
		}

		[DllImport ("__Internal")]
		private static extern void _GetThumbnail (
			string message);
	}
}
