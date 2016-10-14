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
	public class WCLandingPage : MonoBehaviour
	{
		private System.DateTime CurrentDate;
		private System.DateTime OldestDate;
		private System.DateTime TodayDate;

		public GameObject WCContainer;
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

		private bool isTestUser = false;

		private DescribeImage mQuestion;

		// Use this for initialization
		void Start ()
		{
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.WritersCorner);

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

			FetchQuestionForDate (CurrentDate.ToString ("MMdd"));
		}

		public void nextDatePressed ()
		{
			if (isAnimating) {
				return;
			}
			if (CurrentDate.ToString ("yyyyMMdd") != TodayDate.ToString ("yyyyMMdd") || isTestUser) {
				CurrentDate = CurrentDate.AddDays (1);
				SetDate (true, false);

				message.text = "";
				cardTitle.text = "Fetch Question";
				Card.SetActive (true);
				cardIcon.SetActive (false);


				startButton.SetActive (false);
				fetchQuestionBtn.SetActive (true);
			}
		}

		public void previousDatePressed ()
		{
			if (isAnimating) {
				return;
			}
			if (CurrentDate.ToString ("yyyyMMdd") != OldestDate.ToString ("yyyyMMdd") || isTestUser) {
				CurrentDate = CurrentDate.AddDays (-1);
				SetDate (true, true);

				message.text = "";
				cardTitle.text = "Fetch Question";
				Card.SetActive (true);
				cardIcon.SetActive (false);

				startButton.SetActive (false);
				fetchQuestionBtn.SetActive (true);
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
			FetchQuestionForDate (CurrentDate.ToString ("MMdd"));
		}

		private void FetchQuestionForDate (string date)
		{
			fetchQuestionBtn.SetActive (false);
			nextDateBtn.SetActive (false);
			prevDateBtn.SetActive (false);
			progressCircle.SetActive (true);
			startButton.SetActive (false);
			message.text = "";
			cardTitle.text = "Fetching Question";
			cardIcon.SetActive (false);
			Card.SetActive (true);

			LaunchList.instance.DescribeImageLoaded += DescribeImageLoaded;
			HTTPRequestHelper.instance.GetWritersCornerForDate (date);
		}

		public void DescribeImageLoaded (object sender, EventArgs e)
		{
			if (this == null || this.gameObject == null) {
				return;
			}

			mQuestion = LaunchList.instance.mDescribeImage;

			SetDate (false);

			if (mQuestion == null) {
				message.text = "You need internet to use this feature. Press the fetch button to try again";
				progressCircle.SetActive (false);
				startButton.SetActive (true);
				startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Fetch";
			} else if (mQuestion.ImageID == null) {
				message.text = "Oh no! No question has been prepared yet. Check back later";
				progressCircle.SetActive (false);
				startButton.SetActive (true);
				startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Fetch";
				cardTitle.text = "Fetch Question";
				cardIcon.SetActive (true);
				cardIcon.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("Images/WarningIcon");
				cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("FF5541");
				return;
			} else if (mQuestion.UserSubmitted) {
				message.text = "Press the View button to view submitted answers!";
				cardTitle.text = "View Answers";
				cardIcon.SetActive (true);
				cardIcon.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("Images/QuizIcon");
				cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("6800FFF");
				progressCircle.SetActive (false);
				startButton.SetActive (true);
				startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "View Answers";
			} else {
				message.text = "Press the Start button to answer your question of the day!";
				cardTitle.text = "Submit Answer";
				cardIcon.SetActive (true);
				cardIcon.GetComponent<Image> ().sprite = Resources.Load <Sprite> ("Images/QuizIcon");
				cardIcon.GetComponent<Image> ().color = CerebroHelper.HexToRGB ("6800FF");
				progressCircle.SetActive (false);
				startButton.SetActive (true);
				startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text = "Start";
			}
		}

		public void BackPressed ()
		{
			//StudentScript ss = GameObject.FindGameObjectWithTag ("StudentView").GetComponent<StudentScript> ();
			//ss.Initialise ();
			LaunchList.instance.DescribeImageLoaded -= DescribeImageLoaded;
			WelcomeScript.instance.ShowScreen (false);
			Destroy (gameObject);	
		}

		public void StartQuestionPressed ()
		{
			string buttonTxt = startButton.transform.Find ("Text").gameObject.GetComponent<Text> ().text;
			if (buttonTxt == "Fetch") {
				FetchQuestionForDate (CurrentDate.ToString ("MMdd"));
				progressCircle.SetActive (true);
				startButton.SetActive (false);
				message.text = "";
				cardTitle.text = "Fetching Question";
				cardIcon.SetActive (false);
			} else {
				GameObject WC = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.DescribeImage, WCContainer.transform);
			}
		}

		public void BackOnScreen ()
		{
			landingPage.SetActive (true);
			if (mQuestion != null) {
				LaunchList.instance.CheckForSubmittedImageResponsesJSON (mQuestion.ImageID);
			}
			DescribeImageLoaded (null, null);
		}
	}
}
