using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using System.IO;

namespace Cerebro
{

	public class GooglyScript : MonoBehaviour
	{
		private string questionType;

		public GameObject MCQ;

		public GameObject MCQOptions;
		public GameObject inputPanel;

		private string userAnswer;
		private string correctAnswer;

		private List<TEXDraw> optionsTextObject;

		private bool isImageFullScreen = false;

		public Text Question;

		private MissionItemData currentQuestion;

		private bool isAnimating = false;
		public GameObject fsImage;

		private Vector3 questionTextOriginalPosition;
		private Vector3 inputPanelOriginalPosition;

		private string TimeStarted;

		public void Initialize (MissionItemData missionItemData)
		{
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			GetComponent<RectTransform> ().position = new Vector3 (0f, 0f);

			fsImage.SetActive (false);

			questionTextOriginalPosition = Question.transform.position;
			inputPanelOriginalPosition = inputPanel.gameObject.transform.position;

			transform.Find ("Title").gameObject.GetComponent<Text> ().text = missionItemData.QuestionTitle;
			
			optionsTextObject = new List<TEXDraw> ();
			optionsTextObject.Add (MCQOptions.transform.Find ("OptionA").gameObject.GetChildByName<TEXDraw> ("Text"));
			optionsTextObject.Add (MCQOptions.transform.Find ("OptionB").gameObject.GetChildByName<TEXDraw> ("Text"));
			optionsTextObject.Add (MCQOptions.transform.Find ("OptionC").gameObject.GetChildByName<TEXDraw> ("Text"));
			optionsTextObject.Add (MCQOptions.transform.Find ("OptionD").gameObject.GetChildByName<TEXDraw> ("Text"));

			currentQuestion = missionItemData;
			showNextQuestion ();
		}

		private void showNextQuestion ()
		{
			Question.text = currentQuestion.QuestionText;

			CerebroHelper.DebugLog (currentQuestion.QuestionMediaType);

			if (currentQuestion.QuestionMediaType == "Video") {
				inputPanel.transform.Find ("MediaVideo").SetAsLastSibling ();
				inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (true);
				inputPanel.transform.Find ("MediaImage").gameObject.SetActive (false);
			} else if (currentQuestion.QuestionMediaType == "Image") {
				inputPanel.transform.Find ("MediaImage").SetAsLastSibling ();
				inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (false);
				inputPanel.transform.Find ("MediaImage").gameObject.SetActive (true);
				StartCoroutine (LoadImage (currentQuestion.QuestionMediaURL, inputPanel.transform.Find ("MediaImage").gameObject));
				inputPanel.transform.Find ("MediaImage").gameObject.GetChildByName<Graphic> ("Icon").GetComponent<Image> ().color = new Color (1, 1, 1, 0);
			} else {
				inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (false);
				inputPanel.transform.Find ("MediaImage").gameObject.SetActive (false);
			}

			MCQ.SetActive (true);

			userAnswer = "";

			correctAnswer = currentQuestion.Answer;
			var options = currentQuestion.AnswerOptions.Split ("@" [0]);

			for (var i = 0; i < options.Length; i++) {
				optionsTextObject [i].transform.parent.gameObject.SetActive (true);
				optionsTextObject [i].text = options [i];
			}
			for (var j = options.Length; j < 4; j++) {
				optionsTextObject [j].transform.parent.gameObject.SetActive (false);
			}
			for (var i = 0; i < optionsTextObject.Count; i++) {
				optionsTextObject [i].transform.parent.Find ("Image").GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
			}

			TimeStarted = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");

			StartCoroutine (StartAnimation ());
		}

		public void videoIconPressed ()
		{
			VideoHelper.instance.OpenVideoWithUrl (currentQuestion.QuestionMediaURL);
		}

		public void imagePressed ()
		{
			if (isImageFullScreen) {
				fsImage.SetActive (false);
				isImageFullScreen = false;
			} else {
				var sprite = inputPanel.transform.Find ("MediaImage").gameObject.GetChildByName<Graphic> ("Icon").GetComponent<Image> ().sprite;
				if (sprite != null) {
					fsImage.SetActive (true);
					fsImage.transform.SetAsLastSibling ();
					isImageFullScreen = true;
					fsImage.transform.Find ("Image").gameObject.GetComponent<Image> ().sprite = sprite;
				}
			}
		}

		public void submitClick (bool showNext = true)
		{
			if (isAnimating) {
				return;
			}

			isAnimating = true;

			var correct = false;
			if (userAnswer == correctAnswer) {
				correct = true;
			}

			if (correct == true) {
				StartCoroutine (ShowCorrectAnimation ());
			} else {
				StartCoroutine (ShowWrongAnimation ());
			}



			StartCoroutine (CheckForMission (correct));
		}

		IEnumerator CheckForMission(bool correct) {
			yield return new WaitForSeconds (1.0f);

			var studentID = "";
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET.");
			} else {
				studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			}

			System.DateTime timestarted = System.DateTime.ParseExact (TimeStarted, "yyyy-MM-ddTHH:mm:ss", null);
			System.DateTime timeNow = System.DateTime.Now.ToUniversalTime();
			System.TimeSpan differenceTime = timeNow.Subtract (timestarted);
			float diff = (float)differenceTime.TotalMilliseconds;
			diff = Mathf.Floor (diff / 100.0f) / 10.0f;

			List<string> missionQuestionIds = CheckMissions (correct);

			string uniqueTime = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");
			string assessKey = "GOOGLY_" + currentQuestion.QuestionID + "Z" + uniqueTime;
			string day = System.DateTime.Now.ToUniversalTime().ToString ("yyyyMMdd");
			if (missionQuestionIds.Count != 0) {
				string missionString = LaunchList.instance.mMission.MissionID;
				foreach (var str in missionQuestionIds) {
					missionString = missionString + "@" + str;
				}
				Cerebro.LaunchList.instance.WriteAnalyticsToFile (assessKey, int.Parse(currentQuestion.QuestionLevel), correct, day, TimeStarted, Mathf.FloorToInt (diff), "0", -1, missionString);  
			} else {
				Cerebro.LaunchList.instance.WriteAnalyticsToFile (assessKey, int.Parse(currentQuestion.QuestionLevel), correct, day, TimeStarted, Mathf.FloorToInt (diff), "0", -1, " ");  
			}
		}

		List<string> CheckMissions(bool isCorrect) {
			List<string> missionQuestionIDs = new List<string> ();

			if (LaunchList.instance.mMission.Questions != null) {
				foreach (var item in LaunchList.instance.mMission.Questions) {
					if (currentQuestion.QuestionID == item.Value.QuestionID) {
						missionQuestionIDs.Add (item.Value.QuestionID);
						LaunchList.instance.UpdateLocalMissionFile (item.Value, item.Value.QuestionID, isCorrect);
					}
				}
			}
			return missionQuestionIDs;
		}

		public void MCQOptionPressed (int value)
		{
			if (isAnimating) {
				return;
			}
			userAnswer = value.ToString ();
			submitClick ();
//			StartCoroutine (AnimateMCQOption (value));
		}

		IEnumerator AnimateMCQOption (int value)
		{
			var GO = optionsTextObject [value].transform.parent.transform.gameObject;
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));

		}

		public void BackPressed ()
		{
			if (isImageFullScreen) {
				fsImage.SetActive (false);
				isImageFullScreen = false;
			} else {
				WelcomeScript.instance.ShowScreen ();
				Destroy (gameObject);
			}
		}

		IEnumerator StartAnimation ()
		{
			Question.gameObject.SetActive (true);
			inputPanel.gameObject.SetActive (true);

			Question.transform.localScale = new Vector3 (1, 1, 0);
			Question.transform.position = new Vector3 (questionTextOriginalPosition.x - Screen.width, questionTextOriginalPosition.y, questionTextOriginalPosition.z);
			inputPanel.gameObject.transform.position = new Vector3 (inputPanelOriginalPosition.x, inputPanelOriginalPosition.y, inputPanelOriginalPosition.z);

			inputPanel.gameObject.transform.localScale = new Vector3 (1, 0, 0);
			Go.to (Question.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (Screen.width, 0, 0), true).setEaseType (GoEaseType.BackOut));
			yield return new WaitForSeconds (0.3f);
			Go.to (inputPanel.gameObject.transform, 0.3f, new GoTweenConfig ().scale (new Vector3 (0, 1, 0), true).setEaseType (GoEaseType.BackOut));

			yield return new WaitForSeconds (0.1f);
			isAnimating = false;
		}

		IEnumerator ShowCorrectAnimation ()
		{
			GameObject GO = null;

			GO = optionsTextObject [int.Parse (correctAnswer)].transform.parent.transform.gameObject;
			optionsTextObject [int.Parse (correctAnswer)].transform.parent.Find ("Border").GetComponent<Image> ().color = MaterialColor.green400;
			optionsTextObject [int.Parse (correctAnswer)].color = MaterialColor.green400;
			if (GO != null) {
				Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
				yield return new WaitForSeconds (0.2f);
				Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
			}

			yield return new WaitForSeconds (1.0f);
		}

		IEnumerator ShowWrongAnimation ()
		{
			GameObject GO = null;
			GO = optionsTextObject [int.Parse (userAnswer)].transform.parent.transform.gameObject;
//			optionsTextObject [int.Parse (correctAnswer)].transform.parent.Find ("Image").GetComponent<Image> ().color = MaterialColor.green400;
//			optionsTextObject [int.Parse (userAnswer)].transform.parent.Find ("Image").GetComponent<Image> ().color = MaterialColor.red400;
			optionsTextObject [int.Parse (correctAnswer)].transform.parent.Find ("Border").GetComponent<Image> ().color = MaterialColor.green400;
			optionsTextObject [int.Parse (userAnswer)].transform.parent.Find ("Border").GetComponent<Image> ().color = MaterialColor.red400;
			optionsTextObject [int.Parse (correctAnswer)].color = MaterialColor.green400;
			optionsTextObject [int.Parse (userAnswer)].color = MaterialColor.red400;

			Go.to (GO.transform, 0.3f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));

			yield return new WaitForSeconds (2.0f);

		}

		IEnumerator LoadImage (string imgurl, GameObject go)
		{
			CerebroHelper.DebugLog ("Loading Image " + imgurl);
			Graphic graphic = go.GetChildByName<Graphic> ("Icon");
			go.transform.Find ("ProgressCircle").gameObject.SetActive (true);
			Texture2D tex = null;

			if (CerebroHelper.remoteQuizTextures.ContainsKey (imgurl)) {
				tex = CerebroHelper.remoteQuizTextures [imgurl];
				yield return new WaitForSeconds (0.2f);
			} else {
				WWW remoteImage = new WWW (imgurl);
				yield return remoteImage;
				if (remoteImage.error == null) {
					tex = remoteImage.texture;
					CerebroHelper.remoteQuizTextures.Add (imgurl, tex);
				}
			}
			if (tex != null) {
				go.transform.Find ("ProgressCircle").gameObject.SetActive (false);
				var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
				graphic.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
				graphic.GetComponent<Image> ().sprite = newsprite;
			} else {
				go.transform.Find ("ProgressCircle").gameObject.SetActive (false);
				StartCoroutine (LoadImage (imgurl, go));
			}
		}

		void resetColours ()
		{
			for (var i = 0; i < optionsTextObject.Count; i++) {
				optionsTextObject [i].transform.parent.Find ("Image").GetComponent<Image> ().color = new Color (1.0f, 1.0f, 1.0f);
			}
		}
	}
}
