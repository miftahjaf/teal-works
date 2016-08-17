using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Cerebro {
	public class FlaggedQuestion
	{
		public string AssessmentItemID { get; set; }
		public string Difficulty { get; set; }
		public string SubLevel { get; set; }
		public string RandomSeed { get; set; }
	}

	public class Revisit : MonoBehaviour {

		public BaseAssessment baseAssessment;
		public Text title;
		public Text questionNumberText;
		public GameObject noQuestionMessage;
		public GameObject numQuestionMessage;

		public GameObject StartButton;
		public GameObject FlagButton;
		public GameObject nextBtn;
		public GameObject previousBtn;

		List<FlaggedQuestion> flaggedQuestions;
		private GameObject currentAssessment;

		private int currentQuestion = -1;

		List<string> topicNames;
		List<string> topicIDs;
		// Use this for initialization
		void Start () {

			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Revisit);

			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);

			topicNames = new List<string> ();
			topicIDs = new List<string> ();
			foreach (var items in LaunchList.instance.mPracticeItems) {
				topicNames.Add (items.Value.PracticeItemName);
				topicIDs.Add (items.Value.PracticeID);
			}

			List<string> rawQuestions = LaunchList.instance.GetFlaggedQuestions();
			rawQuestions.Reverse ();
			flaggedQuestions = new List<FlaggedQuestion> ();
			for (var i = 0; i < rawQuestions.Count; i++) {
				var question = new FlaggedQuestion ();
				var splitArray = rawQuestions [i].Split ("," [0]);
				question.AssessmentItemID = splitArray[0];
				question.Difficulty = splitArray[1];
				question.SubLevel = splitArray[2];
				question.RandomSeed = splitArray[3];
				for (var j = 0; j < topicIDs.Count; j++) {
					if (question.AssessmentItemID.Contains (topicIDs [j])) {
						flaggedQuestions.Add (question);
						break;
					}
				}
			}

			noQuestionMessage.SetActive (false);
			numQuestionMessage.SetActive (false);

			ShowFirstScreen ();
		}

		void ShowFirstScreen() {
			if (transform.Find ("Texture") != null) {
				transform.Find ("Texture").gameObject.SetActive (true);
			}
			if (transform.Find ("TextureQuestions") != null) {
				transform.Find ("TextureQuestions").gameObject.SetActive (false);
			}

			if (flaggedQuestions.Count == 0) {
				StartButton.SetActive (false);
				noQuestionMessage.SetActive (true);
				numQuestionMessage.SetActive (false);
			} else {
				StartButton.transform.SetAsLastSibling ();
				StartButton.SetActive (true);

				numQuestionMessage.SetActive (true);
				noQuestionMessage.SetActive (false);

				numQuestionMessage.GetComponent<Text> ().text = "You have " + flaggedQuestions.Count + " flagged questions";
			}
		}

		public void StartPressed() {
			StartButton.SetActive (false);
			noQuestionMessage.SetActive (false);
			numQuestionMessage.SetActive (false);

			currentQuestion = -1;
			ShowNextQuestion ();
		}

		void openAssessment(FlaggedQuestion item) {

			if (transform.Find ("Texture") != null) {
				transform.Find ("Texture").gameObject.SetActive (false);
			}
			if (transform.Find ("TextureQuestions") != null) {
				transform.Find ("TextureQuestions").gameObject.SetActive (true);
			}

			FlagButton.SetActive (false);

			var assessmentType = "";
			for (var i = 0; i < topicIDs.Count; i++) {
				if (item.AssessmentItemID.Contains (topicIDs [i])) {
					assessmentType = topicNames [i];
					break;
				}
			}
			title.text = StringHelper.RemoveNumbers (assessmentType);
			assessmentType = assessmentType.Replace (" ", "");

			if (assessmentType != "") {
				if (currentAssessment != null) {
					currentAssessment.GetComponent<BaseAssessment> ().isDestroyed = true;
					Destroy (currentAssessment);
					currentAssessment = null;
				}
				currentAssessment = PrefabManager.InstantiateGameObject ("Assessments/" + assessmentType, gameObject.transform);
				currentAssessment.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 708f);
				currentAssessment.GetComponent<RectTransform> ().position = new Vector3 (0, 0);
				baseAssessment = currentAssessment.GetComponent<BaseAssessment> ();
				baseAssessment.assessmentName = assessmentType;
				baseAssessment.isRevisitedQuestion = true;
				baseAssessment.testMode = false;

				Dictionary<string, string> revisitedQuestionData = new Dictionary<string, string> ();
				revisitedQuestionData ["difficulty"] = item.Difficulty;
				revisitedQuestionData ["sublevel"] = item.SubLevel;
				revisitedQuestionData ["seed"] = item.RandomSeed;
				revisitedQuestionData ["assessmentKey"] = item.AssessmentItemID;

				baseAssessment.revisitedQuestionData = revisitedQuestionData;
				questionNumberText.text = (currentQuestion + 1) + " of " + flaggedQuestions.Count;

				if (flaggedQuestions.Count == 1) {
					nextBtn.SetActive (false);
					previousBtn.SetActive (false);
				}
				else if (currentQuestion == 0) {
					nextBtn.transform.SetAsLastSibling ();
					nextBtn.SetActive (true);
					previousBtn.SetActive (false);
				} else if (currentQuestion == flaggedQuestions.Count - 1) {
					previousBtn.transform.SetAsLastSibling ();
					nextBtn.SetActive (false);
					previousBtn.SetActive (true);
				} else {
					nextBtn.SetActive (true);
					previousBtn.SetActive (true);
					previousBtn.transform.SetAsLastSibling ();
					nextBtn.transform.SetAsLastSibling ();
				}
			} else {
				ShowNextQuestion ();
			}
		}

		public void CheckForNextQuestion() {
			ShowNextQuestion ();
		}

		public void showSameQuestion() {
			openAssessment (flaggedQuestions [currentQuestion]);
		}

		private void ShowNextQuestion() {
			currentQuestion++;
			if (currentQuestion < flaggedQuestions.Count) {
				openAssessment (flaggedQuestions [currentQuestion]);
			} else {
				if (currentAssessment != null) {
					currentAssessment.GetComponent<BaseAssessment> ().isDestroyed = true;
					Destroy (currentAssessment);
					currentAssessment = null;
				}
				nextBtn.SetActive (false);
				previousBtn.SetActive (false);
				FlagButton.SetActive (false);
				ShowFirstScreen ();
			}
		}

		private void ShowPreviousQuestion() {
			currentQuestion--;
			if (currentQuestion >= 0) {
				openAssessment (flaggedQuestions [currentQuestion]);
			}
		}

		public void NextBtnPressed() {
			ShowNextQuestion ();
		}

		public void PreviousBtnPressed() {
			ShowPreviousQuestion ();
		}

		public void ShowFlagButton() {
			FlagButton.SetActive (true);
			FlagButton.transform.SetAsLastSibling ();
			var buttonText = FlagButton.transform.Find ("Text").gameObject.GetComponent<Text> ();
			buttonText.text = "Unflag";
		}

		public void FlagButtonPressed() {
			var buttonText = FlagButton.transform.Find ("Text").gameObject.GetComponent<Text> ();
			if (buttonText.text == "Flag") {
				LaunchList.instance.WriteFlaggedQuestionToFile (flaggedQuestions [currentQuestion].AssessmentItemID, int.Parse(flaggedQuestions [currentQuestion].Difficulty), int.Parse(flaggedQuestions [currentQuestion].SubLevel),int.Parse(flaggedQuestions [currentQuestion].RandomSeed));
				buttonText.text = "Unflag";
			} else {
				buttonText.text = "Flag";
				LaunchList.instance.RemoveFlaggedQuestionFromFile (flaggedQuestions [currentQuestion].AssessmentItemID);
			}
		}
		public void BackPressed() {
			//StudentScript ss = GameObject.FindGameObjectWithTag ("StudentView").GetComponent<StudentScript> ();
			//ss.Initialise ();
			WelcomeScript.instance.ShowScreen(false);
			Destroy (gameObject);
		}
	}
}