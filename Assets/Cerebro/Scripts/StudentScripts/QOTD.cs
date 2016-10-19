using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using System.IO;
using System;
using SimpleJSON;

namespace Cerebro
{

	public static class QOTDTypes
	{
		public const string Subjective = "Subjective";
		public const string MCQ = "MCQ";
		public const string Numeric = "Numeric";
	}

	public class QOTD : MonoBehaviour
	{
		public Font MCQOptionTextNormal;
		public Font MCQOptionTextSelected;

		public string qotdID;

		public Text questionNumberText;
		public Text questionRemainingText;
		public GameObject QuestionLine;

		public GameObject Feedback;
		public GameObject ScoreCardProgressScreen;
		public GameObject ReviewScreen;

		private string questionType;

		public GameObject SubjectiveQuestion;
		public GameObject MCQ;
		public GameObject NumericQuestion;

		public GameObject numericCorrectAnswer;

		public GameObject numPad;
		public GameObject MCQOptions;
		public GameObject subjectivePanel;
		public GameObject inputPanel;

		private Text userAnswerText;
		private string userAnswer;
		private string correctAnswer;

		public GameObject nextBtn;
		public GameObject prevBtn;
		public GameObject viewSolutionBtn;

		private List<Text> optionsTextObject;

		private bool isImageFullScreen = false;

		public Text Question;
		private Text SubQuestion;

		private List<QuizData> allQuestions;

		private int currentQuestion = -1;

		private bool isAnimating = false;
		public GameObject fsImage;

		private bool isRevisiting = false;
		private bool IsLoadingImage = false;
		private bool viewingAnswersMode = false;

		private Vector3 questionTextOriginalPosition;
		private Vector3 inputPanelOriginalPosition;
		private SubmittedQuizData submittedQuizData;

		private float[] msubQuestionLength = new float[]{ 510f, 835f };
		private float[] QuestionLineLength = new float[]{ 250f, 150f };

		private Color textNormalColor;
		private Color textDarkColor;
		private Color borderWrongColor;
		private Color borderCorrectColor;
		private Color borderSelectionColor;
		private Color borderNormalColor;

		public void Initialize (List<QuizData> questions, SubmittedQuizData _submittedQuizData, bool _isRevisiting = false, bool _isViewingAnswers = false)
		{
			textNormalColor = CerebroHelper.HexToRGB ("191923");
			textDarkColor = CerebroHelper.HexToRGB ("14141C");
			borderWrongColor = CerebroHelper.HexToRGB ("FC374B");
			borderCorrectColor = CerebroHelper.HexToRGB ("29CDB1");
			borderSelectionColor = CerebroHelper.HexToRGB ("4B4B55");
			borderNormalColor = CerebroHelper.HexToRGB ("CDCDD2");

			fsImage.SetActive (false);
			viewSolutionBtn.SetActive (false);

			submittedQuizData = _submittedQuizData;

			ReviewScreen.SetActive (false);
			Feedback.SetActive (false);

			questionTextOriginalPosition = Question.transform.position;
			inputPanelOriginalPosition = inputPanel.gameObject.transform.position;

			optionsTextObject = new List<Text> ();
			optionsTextObject.Add (MCQOptions.transform.Find ("OptionA").gameObject.GetChildByName<Text> ("Text"));
			optionsTextObject.Add (MCQOptions.transform.Find ("OptionB").gameObject.GetChildByName<Text> ("Text"));
			optionsTextObject.Add (MCQOptions.transform.Find ("OptionC").gameObject.GetChildByName<Text> ("Text"));
			optionsTextObject.Add (MCQOptions.transform.Find ("OptionD").gameObject.GetChildByName<Text> ("Text"));

			allQuestions = questions;
			viewingAnswersMode = _isViewingAnswers;
			isRevisiting = _isRevisiting;
			currentQuestion = -1;

			if (isRevisiting && !viewingAnswersMode) {
				foreach (var question in allQuestions) {
					question.userAnswer = "";
				}
			}

			showNextQuestion (true, true);
		}

		private void showNextQuestion (bool nextBool, bool firstItem = false)
		{
			currentQuestion++;
			questionNumberText.gameObject.SetActive (true);
			questionRemainingText.gameObject.SetActive (true);
			QuestionLine.SetActive (true);
			IsLoadingImage = false;

			for (var i = 0; i < optionsTextObject.Count; i++) {
				var GO = optionsTextObject [i].transform.parent.transform.gameObject;
				GO.transform.localScale = new Vector3 (1, 1, 1);
			}

			if (currentQuestion < allQuestions.Count) {
				QuizData curQuestion = allQuestions [currentQuestion];
				Question.text = curQuestion.QuestionText;
				float subQuestionWidth = 0f;

				if (curQuestion.QuestionMediaType == "Video") {
					inputPanel.transform.Find ("MediaVideo").SetAsLastSibling ();
					inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (true);
					inputPanel.transform.Find ("MediaImage").gameObject.SetActive (false);
					QuestionLine.GetComponent<RectTransform> ().sizeDelta = new Vector2 (QuestionLine.GetComponent<RectTransform> ().sizeDelta.x, QuestionLineLength [0]);
					subQuestionWidth = msubQuestionLength [0];
				} else if (curQuestion.QuestionMediaType == "Image") {
					inputPanel.transform.Find ("MediaImage").SetAsLastSibling ();
					inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (false);
					inputPanel.transform.Find ("MediaImage").gameObject.SetActive (true);
					StartCoroutine (LoadImage (curQuestion.QuestionMedia, inputPanel.transform.Find ("MediaImage").gameObject));
					inputPanel.transform.Find ("MediaImage").gameObject.GetChildByName<Graphic> ("Icon").GetComponent<Image> ().color = new Color (1, 1, 1, 0);
					QuestionLine.GetComponent<RectTransform> ().sizeDelta = new Vector2 (QuestionLine.GetComponent<RectTransform> ().sizeDelta.x, QuestionLineLength [0]);
					subQuestionWidth = msubQuestionLength [0];
				} else {
					inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (false);
					inputPanel.transform.Find ("MediaImage").gameObject.SetActive (false);
					QuestionLine.GetComponent<RectTransform> ().sizeDelta = new Vector2 (QuestionLine.GetComponent<RectTransform> ().sizeDelta.x, QuestionLineLength [1]);
					subQuestionWidth = msubQuestionLength [1];
				}
				if (curQuestion.QuestionType == QOTDTypes.Numeric) {
					SubjectiveQuestion.SetActive (false);
					MCQ.SetActive (false);
					NumericQuestion.SetActive (true);
					numericCorrectAnswer.SetActive (false);

					correctAnswer = curQuestion.CorrectAnswer;
					SubQuestion = NumericQuestion.GetChildByName<Text> ("subQuestion");
					SubQuestion.GetComponent<RectTransform> ().sizeDelta = new Vector2 (subQuestionWidth, SubQuestion.GetComponent<RectTransform> ().sizeDelta.y);
					userAnswerText = NumericQuestion.transform.Find ("GeneralButton").gameObject.GetChildByName<Text> ("Text");
					SubQuestion.text = curQuestion.QuestionSubText;
					userAnswerText.text = "";
				} else if (curQuestion.QuestionType == QOTDTypes.MCQ) {
					SubjectiveQuestion.SetActive (false);
					MCQ.SetActive (true);
					NumericQuestion.SetActive (false);

					userAnswer = "";

					correctAnswer = curQuestion.CorrectAnswer;
					var options = curQuestion.AnswerOptions.Split ("@" [0]);
					SubQuestion = MCQ.GetChildByName<Text> ("subQuestion");
					SubQuestion.GetComponent<RectTransform> ().sizeDelta = new Vector2 (subQuestionWidth, SubQuestion.GetComponent<RectTransform> ().sizeDelta.y);
					SubQuestion.text = curQuestion.QuestionSubText;


					for (var i = 0; i < options.Length; i++) {
						optionsTextObject [i].transform.parent.gameObject.SetActive (true);
						optionsTextObject [i].text = options [i];
						optionsTextObject [i].color = textNormalColor;
						optionsTextObject [i].font = MCQOptionTextNormal;
					}
					for (var j = options.Length; j < 4; j++) {
						optionsTextObject [j].transform.parent.gameObject.SetActive (false);
					}
					for (var i = 0; i < optionsTextObject.Count; i++) {
						optionsTextObject [i].transform.parent.Find ("Border").GetComponent<Image> ().color = borderNormalColor;
						optionsTextObject [i].transform.parent.Find ("Number").GetComponent<Text> ().color = textNormalColor;
					}

				} else if (curQuestion.QuestionType == QOTDTypes.Subjective) {
					SubjectiveQuestion.SetActive (true);
					MCQ.SetActive (false);
					NumericQuestion.SetActive (false);

					SubQuestion = SubjectiveQuestion.GetChildByName<Text> ("subQuestion");
					SubQuestion.GetComponent<RectTransform> ().sizeDelta = new Vector2 (subQuestionWidth, SubQuestion.GetComponent<RectTransform> ().sizeDelta.y);
					userAnswerText = subjectivePanel.transform.Find ("InputField").gameObject.GetChildByName<Text> ("Text");
					InputField subjectiveField = subjectivePanel.GetChildByName<InputField> ("InputField");
					SubQuestion.text = curQuestion.QuestionSubText;
					subjectiveField.text = "";
				}

				questionNumberText.text = (currentQuestion + 1).ToString ();
				questionRemainingText.text = (allQuestions.Count - (currentQuestion + 1)).ToString () + " remaining";

				if (currentQuestion == 0) {
					prevBtn.SetActive (false);
				} else {
					prevBtn.SetActive (true);
				}
				if (!isRevisiting && curQuestion.userAnswer != null && curQuestion.userAnswer != "") {
					MarkExistingAnswer ();
					nextBtn.SetActive (true);
				} else {
					nextBtn.SetActive (false);
				}

				if (isRevisiting && !viewingAnswersMode) {
					nextBtn.SetActive (true);
					if (curQuestion.userAnswer != null && curQuestion.userAnswer != "") {
						MarkExistingAnswer ();
						MarkCorrectAnswer ();
					}
				}

				if (viewingAnswersMode) {
					nextBtn.SetActive (true);
					if (curQuestion.AnswerURL != null && curQuestion.AnswerURL.Trim () != "") {
						viewSolutionBtn.SetActive (true);
					} else {
						viewSolutionBtn.SetActive (false);
					}
					MarkExistingAnswer ();
					string value = curQuestion.CorrectAnswer;
					userAnswer = value.ToString ();
					MarkCorrectAnswer ();
				}
				
				curQuestion.TimeStarted = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");
				curQuestion.currentSessionTimeStarted = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");

				//StartCoroutine (StartAnimation (nextBool));
				// to animate only from second item 
				if (!firstItem) {
					StartCoroutine (StartAnimation (nextBool));
				} else {
					Question.gameObject.SetActive (true);
				}

			} else {
				if (isRevisiting) {
					if (viewingAnswersMode) {
						if (submittedQuizData != null) {
							ShowScoreCard (submittedQuizData.totalAttempts, submittedQuizData.totalCorrect);
						} else {
							int totalAttempts = 0;
							int totalCorrect = 0;
							for (var i = 0; i < allQuestions.Count; i++) {
								if (allQuestions [i].QuestionType != QOTDTypes.Subjective) {
									totalAttempts++;
									if (allQuestions [i].isCorrect == true) {
										totalCorrect++;
									}
								}
							}
							ShowScoreCard (totalAttempts, totalCorrect);
						}
					} else {
						int totalAttempts = 0;
						int totalCorrect = 0;
						for (var i = 0; i < allQuestions.Count; i++) {
							if (allQuestions [i].QuestionType != QOTDTypes.Subjective) {
								totalAttempts++;
								if (allQuestions [i].isCorrect == true) {
									totalCorrect++;
								}
							}
						}
						ShowScoreCard (totalAttempts, totalCorrect);
					}
				} else {
					questionNumberText.gameObject.SetActive (false);
					questionRemainingText.gameObject.SetActive (false);
					QuestionLine.SetActive (false);

					ScoreCardProgressScreen.SetActive (false);
					ReviewScreen.SetActive (true);
					ReviewScreen.transform.Find ("Error").gameObject.SetActive (false);
				}
				viewSolutionBtn.SetActive (false);
				nextBtn.SetActive (false);
				prevBtn.SetActive (false);
			}
		}

		IEnumerator LoadImage (string imgurl, GameObject go)
		{
			CerebroHelper.DebugLog ("Loading Image " + imgurl);
			Graphic graphic = go.GetChildByName<Graphic> ("Icon");
			go.transform.Find ("ProgressCircle").gameObject.SetActive (true);
			Texture2D tex = null;
			IsLoadingImage = true;

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
				IsLoadingImage = false;
			} else {
				go.transform.Find ("ProgressCircle").gameObject.SetActive (false);
				StartCoroutine (LoadImage (imgurl, go));
			}
		}

		private void MarkExistingAnswer ()
		{
			QuizData curQuestion = allQuestions [currentQuestion];
			if (curQuestion.QuestionType == QOTDTypes.Subjective) {
				InputField subjectiveField = subjectivePanel.GetChildByName<InputField> ("InputField");
				subjectiveField.text = curQuestion.userAnswer;
			} else if (curQuestion.QuestionType == QOTDTypes.MCQ) {
				for (var i = 0; i < optionsTextObject.Count; i++) {
					optionsTextObject [i].transform.parent.Find ("Border").GetComponent<Image> ().color = borderNormalColor;
					optionsTextObject [i].color = textNormalColor;
					optionsTextObject [i].font = MCQOptionTextNormal;
				}
				if (curQuestion.userAnswer != null && curQuestion.userAnswer.Trim () != "") {
					int value = int.Parse (curQuestion.userAnswer);
					userAnswer = value.ToString ();
					if (viewingAnswersMode || isRevisiting) {
						optionsTextObject [value].transform.parent.Find ("Border").GetComponent<Image> ().color = borderWrongColor;
						optionsTextObject [value].transform.parent.Find ("Number").GetComponent<Text> ().color = borderWrongColor;
						optionsTextObject [value].color = borderWrongColor;
					} else {
						optionsTextObject [value].transform.parent.Find ("Border").GetComponent<Image> ().color = borderSelectionColor;
						optionsTextObject [value].color = textDarkColor;
						optionsTextObject [value].font = MCQOptionTextSelected;
					}
				}
			} else if (curQuestion.QuestionType == QOTDTypes.Numeric) {
				userAnswerText.text = curQuestion.userAnswer;
			}
		}

		private void MarkCorrectAnswer ()
		{
			QuizData curQuestion = allQuestions [currentQuestion];
			if (curQuestion.QuestionType == QOTDTypes.MCQ) {
				int value = int.Parse (curQuestion.CorrectAnswer);
				optionsTextObject [value].transform.parent.Find ("Border").GetComponent<Image> ().color = borderCorrectColor;
				optionsTextObject [value].transform.parent.Find ("Number").GetComponent<Text> ().color = borderCorrectColor;
				optionsTextObject [value].color = borderCorrectColor;
			} else if (curQuestion.QuestionType == QOTDTypes.Numeric) {
				numericCorrectAnswer.SetActive (true);
				NumericQuestion.transform.Find ("CorrectAnswer").Find ("Text").GetComponent<Text> ().text = curQuestion.CorrectAnswer;
			}
		}

		public void ViewSolutionPressed ()
		{
			VideoHelper.instance.OpenVideoWithUrl (allQuestions [currentQuestion].AnswerURL);
		}

		public void submitQuizPressed ()
		{
//			int totalAttempts = 0;
//			int totalCorrect = 0;
//			int score = 0;
//			for (var i = 0; i < allQuestions.Count; i++) {
//				if (allQuestions [i].QuestionType != QOTDTypes.Subjective) {
//					totalAttempts++;
//
//					int multiplier = 1;
//					float timeTaken = allQuestions [i].TimeTaken;
//					if (timeTaken < 5f) {
//						multiplier = 5;
//					} else if (timeTaken < 10f) {
//						multiplier = 3;
//					} else if (timeTaken < 20f) {
//						multiplier = 2;
//					}
//					int difficulty = Int32.Parse (allQuestions [i].Difficulty);
//					int difficultyBonus = difficulty * 3;
//
//					if (allQuestions [i].isCorrect == true) {
//						totalCorrect++;
//						CerebroHelper.DebugLog ("Adding " + ((10 + difficultyBonus) * multiplier).ToString());
//						score += (10 + difficultyBonus) * multiplier;
//					}
//				}
//			}
//			CerebroHelper.DebugLog ("Net Score = " + score);
			List<QuizAnalytics> allQuestionAnalytics = new List<QuizAnalytics>();
			if (LaunchList.instance.mUseJSON) {

				AddQuestionsToHistoryForDateJSON (allQuestions [0].QuizDate);

				allQuestionAnalytics = GetQuizQuestionsForDateJSON (allQuestions [0].QuizDate);
			} else {
				List<string> quizQuestions = GetQuizQuestionsForDate (allQuestions [0].QuizDate);

				AddQuestionsToHistoryForDate (allQuestions [0].QuizDate);

				allQuestionAnalytics = new List<QuizAnalytics> ();
				for (var i = 0; i < quizQuestions.Count; i++) {
					QuizAnalytics row = new QuizAnalytics ();
					var splitArr = quizQuestions [i].Split ("," [0]);
					row.QuizDate = splitArr [6];
					row.StudentAndQuestionID = splitArr [1];
					row.Answer = splitArr [2];
					row.Correct = splitArr [3];
					row.TimeStarted = splitArr [4];
					row.TimeTaken = splitArr [5];
					allQuestionAnalytics.Add (row);
				}
			}
			ScoreCardProgressScreen.SetActive (true);
			LaunchList.instance.SendQuizAnalyticsGrouped (allQuestionAnalytics,QuizAnalyticsSent);
		}

		int QuizAnalyticsSent (QuizAnalyticsResponse response) {
			ScoreCardProgressScreen.SetActive (false);
			if (response.Success) {
				markQuizSubmittedJSON (response.Date, response.TotalAttempts, response.TotalCorrect, response.Score);
				BackPressed ();
			} else {
				ReviewScreen.transform.Find ("Error").gameObject.SetActive (true);
			}
			return 1;
		}

		private void ShowScoreCard (int attempts, int correct)
		{
			ReviewScreen.SetActive (false);
			Feedback.SetActive (true);

			Feedback.transform.Find ("Attempts").GetComponent<Text> ().text = "" + attempts;
			Feedback.transform.Find ("Correct").GetComponent<Text> ().text = "" + correct;
			questionNumberText.gameObject.SetActive (false);
			questionRemainingText.gameObject.SetActive (false);
			QuestionLine.SetActive (false);
		}

		public void editAnswersPressed ()
		{
			ReviewScreen.SetActive (false);
			currentQuestion = -1;
			showNextQuestion (true); 
		}

		public void videoIconPressed ()
		{
			QuizData curQuestion = allQuestions [currentQuestion];
			CerebroHelper.DebugLog (curQuestion.QuestionMedia);
			VideoHelper.instance.OpenVideoWithUrl (curQuestion.QuestionMedia);
		}

		public void imagePressed ()
		{
			QuizData curQuestion = allQuestions [currentQuestion];
			if (isImageFullScreen) {
				fsImage.SetActive (false);
				isImageFullScreen = false;
			} else {
				var sprite = inputPanel.transform.Find ("MediaImage").gameObject.GetChildByName<Graphic> ("Icon").GetComponent<Image> ().sprite;
				if (sprite != null && !IsLoadingImage) {
					fsImage.SetActive (true);
					fsImage.transform.SetAsLastSibling ();
					isImageFullScreen = true;
					fsImage.transform.Find ("Image").gameObject.GetComponent<Image> ().sprite = sprite;
				}
			}
		}

		public void submitClick (bool nextBool = true)
		{
			if (isAnimating) {
				return;
			}

			StopAllCoroutines ();

			if (viewingAnswersMode) {
				isAnimating = true;
				StartCoroutine (HideAnimation (nextBool));
				return;
			}

			isAnimating = true;
			QuizData curQuestion = allQuestions [currentQuestion];

			var correct = false;
			if (curQuestion.QuestionType == QOTDTypes.Numeric) {
				userAnswer = userAnswerText.text;
				float userAnswerFloat = float.MaxValue;
				float correctAnswerFloat = float.Parse (correctAnswer);
				if (float.TryParse (userAnswer, out userAnswerFloat)) {
					userAnswerFloat = float.Parse (userAnswer);
				}
				if (userAnswerFloat == correctAnswerFloat) {
					correct = true;
				}
				curQuestion.userAnswer = userAnswer;
			} else if (curQuestion.QuestionType == QOTDTypes.MCQ) {
				if (userAnswer == correctAnswer) {
					correct = true;
				}
				if (userAnswer != "") {
					curQuestion.userAnswer = userAnswer;
				}
			} else if (curQuestion.QuestionType == QOTDTypes.Subjective) {
				curQuestion.userAnswer = userAnswerText.text;
			}
			if (correct == true) {
				curQuestion.isCorrect = true;
			} else {
				curQuestion.isCorrect = false;
			}

			var studentID = "";
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET.");
			} else {
				studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			}

			System.DateTime timestarted = System.DateTime.ParseExact (curQuestion.currentSessionTimeStarted, "yyyy-MM-ddTHH:mm:ss", null);
			System.DateTime timeNow = System.DateTime.Now.ToUniversalTime ();
			System.TimeSpan differenceTime = timeNow.Subtract (timestarted);
			float diff = (float)differenceTime.TotalMilliseconds;
			diff = Mathf.Floor (diff / 100.0f) / 10.0f;
			curQuestion.TimeTaken = diff;

			if (!isRevisiting) {
				WriteAnalyticsToFileJSON (curQuestion.QuizDate, studentID + "Q" + curQuestion.QuestionID, curQuestion.userAnswer, curQuestion.isCorrect, curQuestion.TimeStarted, diff);
			}

			StartCoroutine (HideAnimation (nextBool));
		}

		public void MCQOptionPressed (int value)
		{
			if (isAnimating || viewingAnswersMode) {
				return;
			}
			if (isRevisiting && userAnswer != "") {
				return;
			}
			userAnswer = value.ToString ();
			for (var i = 0; i < optionsTextObject.Count; i++) {
				optionsTextObject [i].transform.parent.Find ("Border").GetComponent<Image> ().color = borderNormalColor;
				optionsTextObject [i].color = textNormalColor;
				optionsTextObject [i].font = MCQOptionTextNormal;
			}
			optionsTextObject [value].transform.parent.Find ("Border").GetComponent<Image> ().color = borderSelectionColor;
			optionsTextObject [value].color = textDarkColor;
			optionsTextObject [value].font = MCQOptionTextSelected;

			nextBtn.SetActive (true);

			if (isRevisiting) {
				if (userAnswer != correctAnswer) {
					optionsTextObject [value].transform.parent.Find ("Border").GetComponent<Image> ().color = borderWrongColor;
					optionsTextObject [value].transform.parent.Find ("Number").GetComponent<Text> ().color = borderWrongColor;
					optionsTextObject [value].color = borderWrongColor;
					Go.to (optionsTextObject [value].transform.parent, 0.2f, new GoTweenConfig ().shake (new Vector3 (0, 0, 10), GoShakeType.Eulers));
				}
				MarkCorrectAnswer ();
				int correctvalue = int.Parse (correctAnswer);
				StartCoroutine (AnimateMCQOption (correctvalue));
			} else {
				StartCoroutine (AnimateMCQOption (value));
			}
		}

		IEnumerator AnimateMCQOption (int value)
		{
			var GO = optionsTextObject [value].transform.parent.transform.gameObject;
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
		}

		public void numPadButtonPressed (int value)
		{
			if (isAnimating || viewingAnswersMode) {
				return;
			}
			if (value <= 9) {
				userAnswerText.text += value.ToString ();
			} else if (value == 10) {    //Back
				if (userAnswerText.text.Length > 0) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
			} else if (value == 11) {   // All Clear
				userAnswerText.text = "";
			} else if (value == 12) {   // -
				if (checkLastTextFor (new string[1]{ "-" })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			} else if (value == 13) {   // .
				if (checkLastTextFor (new string[1]{ "." })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			} 

			if (userAnswerText.text == "") {
				nextBtn.SetActive (false);
			} else {
				nextBtn.SetActive (true);
			}
		}

		public void onInputFieldChange ()
		{
			if (userAnswerText.transform.parent.GetComponent<InputField> ().text == "") {
				nextBtn.SetActive (false);
			} else {
				nextBtn.SetActive (true);
			}	
		}

		private bool checkLastTextFor (string[] strs)
		{
			if (userAnswerText.text.Length > 0) {
				for (var i = 0; i < strs.Length; i++) {
					if (userAnswerText.text.Length >= strs [i].Length) {
						string toCheck = userAnswerText.text.Substring (userAnswerText.text.Length - strs [i].Length, strs [i].Length);
						if (toCheck == strs [i]) {
							return true;
						}
					}
				}
			}
			return false;
		}

		public void BackPressed ()
		{
			if (isImageFullScreen) {
				fsImage.SetActive (false);
				isImageFullScreen = false;
			} else {
				QOTDLandingPage ss = gameObject.transform.parent.parent.GetComponent<QOTDLandingPage> ();
				ss.BackOnScreen ();
				Destroy (gameObject);	
			}
		}

		IEnumerator StartAnimation (bool nextBool)
		{
			GameObject pad = null;

			if (allQuestions [currentQuestion].QuestionType == QOTDTypes.Numeric) {
				pad = numPad;
			}

			Question.gameObject.SetActive (true);
			inputPanel.gameObject.SetActive (true);

			Question.transform.localScale = new Vector3 (1, 1, 0);
			if (!nextBool) {
				Question.transform.position = new Vector3 (questionTextOriginalPosition.x - Screen.width, questionTextOriginalPosition.y, questionTextOriginalPosition.z);
			} else {
				Question.transform.position = new Vector3 (questionTextOriginalPosition.x + Screen.width, questionTextOriginalPosition.y, questionTextOriginalPosition.z);
			}
			inputPanel.gameObject.transform.position = new Vector3 (inputPanelOriginalPosition.x, inputPanelOriginalPosition.y, inputPanelOriginalPosition.z);

			inputPanel.gameObject.transform.localScale = new Vector3 (1, 0, 0);

			if (!nextBool) {
				Go.to (Question.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (Screen.width, 0, 0), true).setEaseType (GoEaseType.BackOut));
			} else {
				Go.to (Question.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (-Screen.width, 0, 0), true).setEaseType (GoEaseType.BackOut));
			}


			yield return new WaitForSeconds (0.4f);
			Go.to (inputPanel.gameObject.transform, 0.4f, new GoTweenConfig ().scale (new Vector3 (0, 1, 0), true).setEaseType (GoEaseType.BackOut));
			if (pad != null && !viewingAnswersMode) {
				Go.to (pad.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (pad.transform.position.x, 0, 0), false).setEaseType (GoEaseType.BackOut));
			} else if (pad != null && viewingAnswersMode) {
				pad.transform.position = new Vector3 (pad.transform.position.x, -Screen.height, 0);
			}

			yield return new WaitForSeconds (0.4f);
			isAnimating = false;
		}

		IEnumerator HideAnimation (bool nextBool)
		{
			GameObject pad = null;
			if (allQuestions [currentQuestion].QuestionType == QOTDTypes.Numeric) {
				pad = numPad;
			}
			if (pad != null) {
				Go.to (pad.transform, 0.2f, new GoTweenConfig ().position (new Vector3 (pad.transform.position.x, -Screen.height, 0), false).setEaseType (GoEaseType.BackIn));
			}
			if (!nextBool) {
				Go.to (Question.transform, 0.2f, new GoTweenConfig ().position (new Vector3 (Screen.width, 0, 0), true));
				Go.to (inputPanel.gameObject.transform, 0.2f, new GoTweenConfig ().position (new Vector3 (Screen.width, 0, 0), true));
			} else {
				Go.to (Question.transform, 0.2f, new GoTweenConfig ().position (new Vector3 (-Screen.width, 0, 0), true));
				Go.to (inputPanel.gameObject.transform, 0.2f, new GoTweenConfig ().position (new Vector3 (-Screen.width, 0, 0), true));
			}
			yield return new WaitForSeconds (0.3f);
			if (nextBool) {
				showNextQuestion (nextBool);
			} else {
				currentQuestion -= 2;
				CerebroHelper.DebugLog (currentQuestion);
				showNextQuestion (nextBool);
			}
		}

		public void previousButtonPressed ()
		{
			submitClick (false);
		}

		public void WriteAnalyticsToFile (string quizDate, string StudentAndQuestionID, string Answer, bool correct, string TimeStarted, float TimeTaken)
		{
			string fileName = Application.persistentDataPath + "/QuizAnalytics.txt";
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET.");
				return;
			}

			var studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			Debug.Log ("Adding quiz "+quizDate+" "+StudentAndQuestionID);
			string newAnalyticLine = "";

			bool addedNewLine = false;
			List<string> lines = new List<string> ();
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;
				if (line != null) {
					splitArr = line.Split ("," [0]);

					if (quizDate == splitArr [6] && StudentAndQuestionID == splitArr [1]) {
						float totalTime = TimeTaken + float.Parse (splitArr [5]);
						if (correct) {
							newAnalyticLine = "" + studentID + "," + StudentAndQuestionID + "," + Answer + "," + "true" + "," + splitArr [4] + "," + totalTime + "," + quizDate;
						} else {
							newAnalyticLine = "" + studentID + "," + StudentAndQuestionID + "," + Answer + "," + "false" + "," + splitArr [4] + "," + totalTime + "," + quizDate;
						}
						lines.Add (newAnalyticLine);
						addedNewLine = true;
					} else {
						lines.Add (line);
					}
				}
				Debug.Log ("lines "+lines.Count);
				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						splitArr = line.Split ("," [0]);
						if (quizDate == splitArr [6] && StudentAndQuestionID == splitArr [1]) {
							float totalTime = TimeTaken + float.Parse (splitArr [5]);
							if (correct) {
								newAnalyticLine = "" + studentID + "," + StudentAndQuestionID + "," + Answer + "," + "true" + "," + splitArr [4] + "," + totalTime + "," + quizDate;
							} else {
								newAnalyticLine = "" + studentID + "," + StudentAndQuestionID + "," + Answer + "," + "false" + "," + splitArr [4] + "," + totalTime + "," + quizDate;
							}
							lines.Add (newAnalyticLine);
							addedNewLine = true;
						} else {
							lines.Add (line);
						}
					}
					Debug.Log ("lines "+lines.Count);
				}  
				sr.Close ();
			}
			if (!addedNewLine) {
				if (correct) {
					newAnalyticLine = "" + studentID + "," + StudentAndQuestionID + "," + Answer + "," + "true" + "," + TimeStarted + "," + TimeTaken + "," + quizDate;
				} else {
					newAnalyticLine = "" + studentID + "," + StudentAndQuestionID + "," + Answer + "," + "false" + "," + TimeStarted + "," + TimeTaken + "," + quizDate;
				}
				lines.Add (newAnalyticLine);
			}
			Debug.Log ("lines "+lines.Count);

			StreamWriter writesr = File.CreateText (fileName);
			for (var i = 0; i < lines.Count; i++) {
				writesr.WriteLine (lines [i]);
			}
			writesr.Close ();
		}

		public void WriteAnalyticsToFileJSON (string quizDate, string StudentAndQuestionID, string Answer, bool correct, string TimeStarted, float TimeTaken)
		{
			if (!LaunchList.instance.mUseJSON) {
				WriteAnalyticsToFile (quizDate, StudentAndQuestionID, Answer, correct, TimeStarted, TimeTaken);
				return;
			}

			string fileName = Application.persistentDataPath + "/QuizAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return;

			if (Answer == null || Answer == "")
				return;
			
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET.");
				return;
			}
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			string data = File.ReadAllText (fileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);

			bool found = false;
			for (int i = 0; i < N ["Data"] ["Quiz"].Count; i++) {
				if (N ["Data"] ["Quiz"] [i] ["StudentAndQuestionID"].Value == StudentAndQuestionID && N ["Data"] ["Quiz"] [i] ["quizDate"].Value == quizDate) {
					N ["Data"] ["Quiz"] [i] ["Answer"] = Answer;
					N ["Data"] ["Quiz"] [i] ["IsCorrect"] = correct.ToString ();
					N ["Data"] ["Quiz"] [i] ["TimeStarted"] = TimeStarted;
					N ["Data"] ["Quiz"] [i] ["TimeTaken"] = (N ["Data"] ["Quiz"] [i] ["TimeTaken"].AsFloat + TimeTaken).ToString();
					found = true;
				}
			}

			if (!found) {
				int cnt = N ["Data"] ["Quiz"].Count;
				N ["Data"] ["Quiz"] [cnt] ["studentID"] = studentID;
				N ["Data"] ["Quiz"] [cnt] ["StudentAndQuestionID"] = StudentAndQuestionID;
				N ["Data"] ["Quiz"] [cnt] ["Answer"] = Answer;
				N ["Data"] ["Quiz"] [cnt] ["IsCorrect"] = correct.ToString();
				N ["Data"] ["Quiz"] [cnt] ["TimeStarted"] = TimeStarted;
				N ["Data"] ["Quiz"] [cnt] ["TimeTaken"] = TimeTaken.ToString();
				N ["Data"] ["Quiz"] [cnt] ["quizDate"] = quizDate;
			}
			File.WriteAllText (fileName, N.ToString());
		}

		public List<string> GetQuizQuestionsForDate (string quizDate)
		{
			string fileName = Application.persistentDataPath + "/QuizAnalytics.txt";


			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET.");
				return null;
			}

			List<string> lines = new List<string> ();
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;
				if (line != null) {
					splitArr = line.Split ("," [0]);
					if (quizDate == splitArr [6]) {
						lines.Add (line);
					}
				}

				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						splitArr = line.Split ("," [0]);
						if (quizDate == splitArr [6]) {
							lines.Add (line);
						}
					}
				}  
				sr.Close ();
			}

			return lines;
		}

		public List<QuizAnalytics> GetQuizQuestionsForDateJSON (string quizDate)
		{
			string fileName = Application.persistentDataPath + "/QuizAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return null;
			
			string data = File.ReadAllText (fileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				return null;
			}
			JSONNode N = JSONClass.Parse (data);

			List<QuizAnalytics> CurrQues = new List<QuizAnalytics> ();
			for (int i = 0; i < N ["Data"] ["Quiz"].Count; i++) {
				if (N ["Data"] ["Quiz"] [i] ["quizDate"].Value == quizDate) {
					QuizAnalytics qa = new QuizAnalytics ();
					qa.StudentAndQuestionID = N ["Data"] ["Quiz"] [i] ["StudentAndQuestionID"].Value;
					qa.Answer = N ["Data"] ["Quiz"] [i] ["Answer"].Value;
					qa.Correct = N ["Data"] ["Quiz"] [i] ["IsCorrect"].Value;
					qa.TimeStarted = N ["Data"] ["Quiz"] [i] ["TimeStarted"].Value;
					qa.TimeTaken = N ["Data"] ["Quiz"] [i] ["TimeTaken"].Value;
					qa.QuizDate = N ["Data"] ["Quiz"] [i] ["quizDate"].Value;
					CurrQues.Add (qa);
				}
			}
			return CurrQues;
		}

		public void AddQuestionsToHistoryForDate (string quizDate)
		{
			string fileName = Application.persistentDataPath + "/QuizAnalytics.txt";


			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET.");
				return;
			}

			List<string> lines = new List<string> ();
			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;
				if (line != null) {
					splitArr = line.Split ("," [0]);
					if (quizDate == splitArr [6]) {
						lines.Add (line);
					}
				}

				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						splitArr = line.Split ("," [0]);
						if (quizDate == splitArr [6]) {
							lines.Add (line);
						}
					}
				}  
				sr.Close ();
			}

			fileName = Application.persistentDataPath + "/QuizHistory.txt";
			StreamWriter writesr = null;
			if (File.Exists (fileName)) {
				writesr = File.AppendText (fileName);
			} else {
				writesr = File.CreateText (fileName);
			}
			for (var i = 0; i < lines.Count; i++) {
				writesr.WriteLine (lines [i]);
			}
			writesr.Close ();
		}

		public void AddQuestionsToHistoryForDateJSON (string quizDate)
		{
			if (!LaunchList.instance.mUseJSON) {
				AddQuestionsToHistoryForDate (quizDate);
				return;
			}

			string fileName = Application.persistentDataPath + "/QuizAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return;
			
			string data = File.ReadAllText (fileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);

			string historyFileName = Application.persistentDataPath + "/QuizHistoryJSON.txt";
			if (!File.Exists (historyFileName))
				return;
			
			string data1 = File.ReadAllText (historyFileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data1)) {
				return;
			}
			JSONNode N1 = JSONClass.Parse (data1);
			int cnt = N1 ["Data"] ["Quiz"].Count;

			for (int i = 0; i < N ["Data"] ["Quiz"].Count; i++) {
				if (N ["Data"] ["Quiz"] [i] ["quizDate"].Value == quizDate) {
					N1 ["Data"] ["Quiz"] [cnt] ["studentID"] = N ["Data"] ["Quiz"] [i] ["studentID"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["StudentAndQuestionID"] = N ["Data"] ["Quiz"] [i] ["StudentAndQuestionID"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["Answer"] = N ["Data"] ["Quiz"] [i] ["Answer"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["IsCorrect"] = N ["Data"] ["Quiz"] [i] ["IsCorrect"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["TimeStarted"] = N ["Data"] ["Quiz"] [i] ["TimeStarted"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["TimeTaken"] = N ["Data"] ["Quiz"] [i] ["TimeTaken"].Value;
					N1 ["Data"] ["Quiz"] [cnt] ["quizDate"] = N ["Data"] ["Quiz"] [i] ["quizDate"].Value;
					cnt++;
				}
			}

			File.WriteAllText (historyFileName, N1.ToString());
		}

		public void markQuizSubmitted (string quizDate, string totalAttempts, string correct, string score)
		{
			string fileName = Application.persistentDataPath + "/QuizSubmitted.txt";
			StreamWriter sr = null;
			if (File.Exists (fileName)) {
				CerebroHelper.DebugLog (fileName + " found, appending.");
				sr = File.AppendText (fileName);
			} else {
				sr = File.CreateText (fileName);
			}

			sr.WriteLine ("{0},{1},{2},{3}", quizDate, totalAttempts, correct, score);
			sr.Close ();
		}

		public void markQuizSubmittedJSON (string quizDate, string totalAttempts, string correct, string score)
		{
			if (!LaunchList.instance.mUseJSON) {
				markQuizSubmitted (quizDate, totalAttempts, correct, score);
				return;
			}

			string fileName = Application.persistentDataPath + "/QuizSubmittedJSON.txt";
			if (!File.Exists (fileName))
				return;
			
			string data = File.ReadAllText (fileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode N = JSONClass.Parse (data);
			int cnt = N ["Data"].Count;

			N ["Data"] [cnt] ["quizDate"] = quizDate;
			N ["Data"] [cnt] ["totalAttempts"] = totalAttempts;
			N ["Data"] [cnt] ["correct"] = correct;
			N ["Data"] [cnt] ["score"] = score;

			File.WriteAllText (fileName, N.ToString());
		}

		public string GetUserAnswerFromHistoryFile (string quizDate, string StudentAndQuestionID)
		{
			string fileName = Application.persistentDataPath + "/QuizHistory.txt";

			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET.");
				return null;
			}

			if (File.Exists (fileName)) {
				var sr = File.OpenText (fileName);
				var line = sr.ReadLine ();
				string[] splitArr;
				if (line != null) {
					splitArr = line.Split ("," [0]);
					if (quizDate == splitArr [6] && StudentAndQuestionID == splitArr [1]) {
						sr.Close ();
						return splitArr [2];
					}
				}

				while (line != null) {
					line = sr.ReadLine ();
					if (line != null) {
						splitArr = line.Split ("," [0]);
						if (quizDate == splitArr [6] && StudentAndQuestionID == splitArr [1]) {
							sr.Close ();
							return splitArr [2];
						}
					}
				}  
				sr.Close ();
			}
			return null;
		}

		public string GetUserAnswerFromHistoryFileJSON (string quizDate, string StudentAndQuestionID)
		{
			if (!LaunchList.instance.mUseJSON) {
				string ans = GetUserAnswerFromHistoryFile (quizDate, StudentAndQuestionID);
				return ans;
			}
			
			string fileName = Application.persistentDataPath + "/QuizHistoryJSON.txt";
			if (!File.Exists (fileName))
				return "";
			
			string data = File.ReadAllText (fileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				return null;
			}
			JSONNode N = JSONClass.Parse (data);

			for (int i = 0; i < N ["Data"] ["Quiz"].Count; i++) {
				if (N ["Data"] ["Quiz"] [i] ["quizDate"].Value == quizDate && N ["Data"] ["Quiz"] [i] ["StudentAndQuestionID"].Value == StudentAndQuestionID) {
					return N ["Data"] ["Quiz"] [i] ["Answer"].Value;
				}
			}
			return null;
		}

		void Update() {
			if (SwipeHelper.swipeDirection == Swipe.Right) {
				if (prevBtn.activeSelf) {
					submitClick (false);
				}
			} else if (SwipeHelper.swipeDirection == Swipe.Left) {
				if (nextBtn.activeSelf) {
					submitClick ();
				}
			}
		}
	}
}
