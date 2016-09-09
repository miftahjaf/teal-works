using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.EventSystems;
using System;

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

		private bool isTestUser = false;

		private Verbalize mQuestion;

		// Use this for initialization
		void Start ()
		{
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Verbalize);

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

		public void ManageCardDataForDate(DateTime currdate, bool autoFetching = false)
		{
			DisableAllButtons ();
			if (LaunchList.instance.VerbalizeSaving) {
				message.text = "Please Wait.";
				cardTitle.text = "Saving Video";

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
					message.text = "Press the Start button to start your session of the day!";
					cardTitle.text = "Submit Video";
					cardIcon.SetActive (true);
					cardIcon.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("Images/QuizIcon");
					cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("6800FF");
					progressCircle.SetActive (false);
					startButton.SetActive (true);
				} else if (!Verb.UserSubmitted) {
					message.text = "Press the View button to view submitted Video!";
					cardTitle.text = "View Video";

					LaunchList.instance.mVerbalize = Verb;

					ViewRetakeButton.SetActive (true);
					progressCircle.SetActive (false);
					fetchQuestionBtn.SetActive (false);
				} else {
					message.text = "Press the View button to view submitted Video!";
					cardTitle.text = "View Video";

					LaunchList.instance.mVerbalize = Verb;

					ViewButton.SetActive (true);
					progressCircle.SetActive (false);
					fetchQuestionBtn.SetActive (false);
				}
			} else {
				if (autoFetching) {
					FetchQuestionForDate (currdate);
				} else {
					message.text = "";
					cardTitle.text = "Fetch Question";

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
					cardTitle.text = "Saving Video";
					cardIcon.SetActive (false);
					Card.SetActive (true);
					return;
				}
			} else {
				nextDateBtn.SetActive (false);
				prevDateBtn.SetActive (false);
				progressCircle.SetActive (true);
				message.text = "";
				cardTitle.text = "Fetching Question";
				cardIcon.SetActive (false);
				Card.SetActive (true);

				LaunchList.instance.VerbalizeTextLoaded += VerbalizeTextLoaded;
				HTTPRequestHelper.instance.GetVerbalizeForDate (date.ToString ("MMdd"));
			}
		}

		public void ViewButtonPressed()
		{
			CerebroHelper.DebugLog ("Viewing "+LaunchList.instance.mVerbalize.UserResponseURL);
			Debug.Log ("Viewing "+LaunchList.instance.mVerbalize.UserResponseURL);
			#if UNITY_IOS && !UNITY_EDITOR
			if(LaunchList.instance.mVerbalize.UploadedToServer)
				Handheld.PlayFullScreenMovie (LaunchList.instance.mVerbalize.UserResponseURL);
			else
				Handheld.PlayFullScreenMovie ("file://"+LaunchList.instance.mVerbalize.UserResponseURL);
			#endif
			StartCoroutine (DisableProgressCircle ());
		}

		IEnumerator DisableProgressCircle()
		{
			progressCircle.SetActive (true);
			yield return 0;
			yield return 0;
			progressCircle.SetActive (false);
		}

		public void DoneButtonPressed()
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
			HTTPRequestHelper.instance.uploadProfileVid ("vid.mov", LaunchList.instance.mVerbalize.UserResponseURL, LaunchList.instance.mVerbalize);
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
				message.text = "Oh no! No question has been prepared yet. Check back later";
				progressCircle.SetActive (false);
				DisableAllButtons ();
				fetchQuestionBtn.SetActive (true);
				cardTitle.text = "Fetch Question";
				cardIcon.SetActive (true);
				cardIcon.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("Images/WarningIcon");
				cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("FF5541");
				return;
			} /*else if (mQuestion.UserSubmitted && mQuestion.UploadedToServer) {
				message.text = "Press the View button to view last recorded Video!";
				cardTitle.text = "Video Video";
				cardIcon.SetActive (true);
				cardIcon.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("Images/QuizIcon");
				cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("6800FFF");
				progressCircle.SetActive (false);
				DisableAllButtons ();
				ViewRetakeButton.SetActive (true);
			} else if (mQuestion.UserSubmitted) {
				message.text = "Please wait.";
				cardTitle.text = "Uploading Video";
				cardIcon.SetActive (false);
				progressCircle.SetActive (false);
				DisableAllButtons ();
			}*/ else {
				message.text = "Press the Start button to start your session of the day!";
				cardTitle.text = "Submit Video";
				cardIcon.SetActive (true);
				cardIcon.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("Images/QuizIcon");
				cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("6800FF");
				progressCircle.SetActive (false);
				DisableAllButtons ();
				startButton.SetActive (true);
			}
		}

		public void BackPressed ()
		{
			LaunchList.instance.VerbalizeTextLoaded -= VerbalizeTextLoaded;
			WelcomeScript.instance.ShowScreen (false);
			Destroy (gameObject);	
		}

		public void StartQuestionPressed ()
		{
			GameObject Verbalize = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Verbalize, VerbalizeContainer.transform);
			landingPage.SetActive (false);
		}

		public void BackOnScreen (bool ShowRatingPopup = false)
		{
			landingPage.SetActive (true);
			if (LaunchList.instance.mVerbalize != null) {
				DateTime dt = DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbalizeDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
				ManageCardDataForDate (dt);
			}

			transform.localPosition = new Vector2 (transform.localPosition.x, -1152f);
			Go.to (transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (-512f, -384f), false));

			if(ShowRatingPopup)
				StartCoroutine (ShowPopup());
		}

		IEnumerator ShowPopup()
		{
			yield return new WaitForSeconds (0.2f);
			DateTime stTime = DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbStartTime, "yyyy-MM-ddTHH:mm:ss", null);
			DateTime endTime = DateTime.ParseExact (LaunchList.instance.mVerbalize.VerbEndTime, "yyyy-MM-ddTHH:mm:ss", null);
			float TimeSpent = (float)(endTime.Subtract(stTime).TotalSeconds);
			Debug.Log ("Total Time "+TimeSpent);
			WelcomeScript.instance.ShowRatingPopup ("VERBALIZE", TimeSpent,LaunchList.instance.mVerbalize.VerbalizeID, "How was your experience of speaking out loud?");
		}
	}
}
