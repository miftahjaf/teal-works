using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using System.IO;
using System;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;

namespace Cerebro
{
	public class DescribeImageScript : MonoBehaviour
	{
		[SerializeField]
		private GameObject ProgressCircle;
		[SerializeField]
		private GameObject QuestionView;
		[SerializeField]
		private GameObject DisplayView;
		[SerializeField]
		private GameObject LoaderView;
		[SerializeField]
		private GameObject ErrorView;

		public Text inputFieldCharLimit;

		public GameObject subjectivePanel;
		public GameObject inputPanel;

		private Text userAnswerText;
		private string userAnswer;

		public GameObject submitBtn;

		private bool isImageFullScreen = false;

		public Text Question;

		private DescribeImage mQuestion;

		private bool isAnimating = false;
		public GameObject fsImage;

		private Vector3 questionTextOriginalPosition;
		private Vector3 inputPanelOriginalPosition;
		private int charLimit = 500;

		private string TimeStarted;

		private List<GameObject> Boxes;

		private TouchScreenKeyboard keyboard;
		Rect textViewRect;

		float[] yPositions;
		float[] xPositions;
		float padding = 16f;
		float textPadding = 16f;
		float displayViewContainerTopPadding = 600f;

		void Start ()
		{
			gameObject.name = "WritersCorner";

			DisplayView.SetActive (false);
			QuestionView.SetActive (false);
			ProgressCircle.SetActive (true);
			LoaderView.SetActive (false);
			ErrorView.SetActive (false);

			GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0f, 0f);
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);

			charLimit = LaunchList.instance.mDescribeImage.CharLimit;
			inputFieldCharLimit.text = charLimit.ToString ();

			userAnswerText = subjectivePanel.transform.Find ("InputField").gameObject.GetChildByName<Text> ("Text");

			Initialise (LaunchList.instance.mDescribeImage);
		}

		void Initialise(DescribeImage _question) {
			mQuestion = _question;

			if (mQuestion == null) {
				ErrorView.SetActive (true);
				ErrorView.transform.Find ("Text").GetComponent<Text> ().text = "No Internet Connection\nThis feature requires internet connection";
				ProgressCircle.SetActive (false);
			} else if (mQuestion.ImageID == null) {
				ErrorView.SetActive (true);
				ErrorView.transform.Find ("Text").GetComponent<Text> ().text = "No more images\nPlease check back later! ";
				ProgressCircle.SetActive (false);
			} else if (mQuestion.UserSubmitted) {
				print (mQuestion.UserSubmitted);
				GetResponses ();
			} else {
				print ("Initializing");
				Initialize ();
			}
		}

		void GetResponses ()
		{
			DisplayView.SetActive (false);
			QuestionView.SetActive (false);
			ProgressCircle.SetActive (true);
			ErrorView.SetActive (false);

			LaunchList.instance.DescribeImageResponsesLoaded -= DescribeImageResponsesLoaded;
			LaunchList.instance.DescribeImageResponsesLoaded += DescribeImageResponsesLoaded;
			LaunchList.instance.GetEnglishImageResponses (mQuestion.ImageID);
		}

		public void Initialize ()
		{
			ErrorView.SetActive (false);
			QuestionView.SetActive (true);
			ProgressCircle.SetActive (false);
			DisplayView.SetActive (false);
			LoaderView.SetActive (false);

			fsImage.SetActive (false);
		
			questionTextOriginalPosition = Question.transform.position;
			inputPanelOriginalPosition = inputPanel.gameObject.transform.position;

			showNextQuestion ();
		}

		void DescribeImageResponsesLoaded (object sender, EventArgs e)
		{
			ProgressCircle.SetActive (false);
			ShowDisplayView ();
		}

		private void showNextQuestion ()
		{
			print ("showNextQuestion");
			if (mQuestion.PromptText [mQuestion.PromptText.Length - 1] == "." [0]) {
				Question.text = mQuestion.PromptText + " " + mQuestion.SubPromptText;
			} else {
				Question.text = mQuestion.PromptText + ". " + mQuestion.SubPromptText;
			}

			if (mQuestion.MediaType == "Video") {
				inputPanel.transform.Find ("MediaVideo").SetAsLastSibling ();
				inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (true);
				inputPanel.transform.Find ("MediaImage").gameObject.SetActive (false);
				var firstSplit = mQuestion.MediaURL.Split ("v="[1]);
				var videoID = firstSplit [1].Split ("&"[0])[0];
				var videoUrl = "https://www.youtube.com/embed/" + videoID;
				var imgurl = "https://img.youtube.com/vi/" + videoID + "/default.jpg";
				StartCoroutine (LoadThumbnail(imgurl, mQuestion.ImageID));
			} else if (mQuestion.MediaType == "Image") {
				inputPanel.transform.Find ("MediaImage").SetAsLastSibling ();
				inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (false);
				inputPanel.transform.Find ("MediaImage").gameObject.SetActive (true);
//				StartCoroutine (LoadImage (mQuestion.MediaURL, inputPanel.transform.Find ("MediaImage").gameObject));
				inputPanel.transform.Find ("MediaImage").gameObject.GetChildByName<Graphic> ("Icon").GetComponent<Image> ().color = new Color (1, 1, 1, 0);
			} else {
				inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (false);
				inputPanel.transform.Find ("MediaImage").gameObject.SetActive (false);
			}
			
			userAnswerText = subjectivePanel.transform.Find ("InputField").gameObject.GetChildByName<Text> ("Text");
			InputField subjectiveField = subjectivePanel.GetChildByName<InputField> ("InputField");
			subjectiveField.text = "";

			print (subjectivePanel.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition);
			print (subjectivePanel.transform.parent.gameObject.GetComponent<RectTransform> ().sizeDelta);

			Vector3 currentPos = inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition;
			Vector2 currentSize = inputPanel.transform.Find ("Subjective").GetComponent<RectTransform> ().sizeDelta;
			inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition = new Vector3 (currentPos.x, -377f, currentPos.z);
			inputPanel.transform.Find ("Subjective").GetComponent<RectTransform> ().sizeDelta = new Vector2 (currentSize.x, 260f);

			textViewRect = new Rect ();

			textViewRect.x = subjectivePanel.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition.x;
			textViewRect.y = -subjectivePanel.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition.y;
			textViewRect.width = subjectivePanel.transform.parent.gameObject.GetComponent<RectTransform> ().sizeDelta.x;
			textViewRect.height = subjectivePanel.transform.parent.gameObject.GetComponent<RectTransform> ().sizeDelta.y;

			#if UNITY_IOS && !UNITY_EDITOR
			_AddTextView (textViewRect.x, textViewRect.y, textViewRect.width, textViewRect.height,charLimit,userAnswerText.text);
			subjectivePanel.SetActive(false);
			#else
				EventSystem.current.SetSelectedGameObject(userAnswerText.transform.parent.GetComponent<InputField> ().gameObject, null);
				userAnswerText.transform.parent.GetComponent<InputField> ().OnPointerClick (new PointerEventData(EventSystem.current));
				onInputFieldChange ();
			#endif

			TimeStarted = System.DateTime.Now.ToString ("yyyyMMddHHmmss");

			StartCoroutine (StartAnimation ());
		}

		public void imagePressed ()
		{
			if (isImageFullScreen) {
				fsImage.SetActive (false);
				isImageFullScreen = false;
				_AddTextView (textViewRect.x, textViewRect.y, textViewRect.width, textViewRect.height,charLimit,userAnswerText.text);
			} else {
				_RemoveTextView ();
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
		
			if (userAnswerText.text.Trim ().Length <= 10) {
				return;
			}

			isAnimating = true;
		
			var studentID = "";
			if (!PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				CerebroHelper.DebugLog ("NO STUDENT SET.");
			} else {
				studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			}

			System.DateTime timestarted = System.DateTime.ParseExact (TimeStarted, "yyyyMMddHHmmss", null);
			System.DateTime timeNow = System.DateTime.Now;
			System.TimeSpan differenceTime = timeNow.Subtract (timestarted);
			float diff = (float)differenceTime.TotalMilliseconds;
			diff = Mathf.Floor (diff / 100.0f) / 10.0f;
			#if UNITY_IOS && !UNITY_EDITOR
			_RemoveTextView();
			#endif
			LoaderView.SetActive (true);
			LaunchList.instance.DescribeImageResponseSubmitted -= DescribeImageResponseSubmitted;
			LaunchList.instance.DescribeImageResponseSubmitted += DescribeImageResponseSubmitted;
			HTTPRequestHelper.instance.DescribeImageResponseSubmitted -= DescribeImageResponseSubmitted;
			HTTPRequestHelper.instance.DescribeImageResponseSubmitted += DescribeImageResponseSubmitted;

			LaunchList.instance.SubmitDescribeImageResponse (studentID, mQuestion.ImageID, userAnswerText.text);
		}

		public void DescribeImageResponseSubmitted (object sender, EventArgs e)
		{
			LoaderView.SetActive (false);
			StartCoroutine (HideAnimation ());
		}

		public void onInputFieldChange ()
		{
			userAnswerText.transform.parent.GetComponent<InputField> ().text = StringHelper.RemoveUnicodeCharacters (userAnswerText.transform.parent.GetComponent<InputField> ().text);
			int currentCharLength = userAnswerText.transform.parent.GetComponent<InputField> ().text.Length;
			if (charLimit - currentCharLength < 0) {
				userAnswerText.transform.parent.GetComponent<InputField> ().text = userAnswerText.text;
				return;
			}
			inputFieldCharLimit.text = (charLimit - currentCharLength).ToString ();
		}

		public void BackPressed ()
		{
			if (isImageFullScreen) {
				fsImage.SetActive (false);
				isImageFullScreen = false;
			} else {
				#if UNITY_IOS && !UNITY_EDITOR
				_RemoveTextView();
				#endif

				WCLandingPage ss = gameObject.transform.parent.parent.GetComponent<WCLandingPage> ();
				ss.BackOnScreen ();

				LaunchList.instance.DescribeImageResponsesLoaded -= DescribeImageResponsesLoaded;
				LaunchList.instance.DescribeImageResponseSubmitted -= DescribeImageResponseSubmitted;
				HTTPRequestHelper.instance.DescribeImageResponseSubmitted -= DescribeImageResponseSubmitted;
				Destroy (gameObject);
			}
		}

		IEnumerator StartAnimation ()
		{
			Question.gameObject.SetActive (true);
			inputPanel.gameObject.SetActive (true);

			Question.transform.localScale = new Vector3 (1, 1, 1);
			Question.transform.position = new Vector3 (questionTextOriginalPosition.x - Screen.width, questionTextOriginalPosition.y, questionTextOriginalPosition.z);
			inputPanel.gameObject.transform.position = new Vector3 (inputPanelOriginalPosition.x, inputPanelOriginalPosition.y, inputPanelOriginalPosition.z);

			inputPanel.gameObject.transform.localScale = new Vector3 (1, 0, 1);
			Go.to (Question.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (Screen.width, 0, 0), true).setEaseType (GoEaseType.BackOut));
			yield return new WaitForSeconds (0.3f);
			Go.to (inputPanel.gameObject.transform, 0.3f, new GoTweenConfig ().scale (new Vector3 (0, 1, 1), true).setEaseType (GoEaseType.BackOut));

			yield return new WaitForSeconds (0.3f);
			isAnimating = false;
			if (mQuestion.MediaType == "Image") {
				StartCoroutine (LoadImage (mQuestion.MediaURL, inputPanel.transform.Find ("MediaImage").gameObject, false));
			}
		}

		IEnumerator LoadThumbnail(string imgurl, string ImageID)
		{
			Texture2D tex = null;
			WWW remoteImage = new WWW (imgurl);
			yield return remoteImage;
			if (remoteImage.error == null) {
				GameObject gm = inputPanel.transform.Find ("MediaVideo").FindChild ("BG").gameObject;
				if (ImageID == mQuestion.ImageID && gm != null) {
					tex = remoteImage.texture;
					var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
					gm.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
					gm.GetComponent<Image> ().sprite = newsprite;
				}
			} else {
				print (remoteImage.error + ",for," + imgurl);
			}
		}

		IEnumerator LoadImage (string imgurl, GameObject go, bool isProfileImage)
		{
			CerebroHelper.DebugLog ("Loading Image " + imgurl);
			Graphic graphic = go.transform.Find ("Mask").GetChildByName<Graphic> ("Icon");
			if (!isProfileImage) {
				graphic = go.transform.Find ("Mask").GetChildByName<Graphic> ("Icon");
				go.transform.Find ("ProgressCircle").gameObject.SetActive (true);
			} else {
				graphic = go.transform.Find ("Icon").GetComponent<Graphic> ();
			}

			Texture2D tex = null;

			if (CerebroHelper.remoteQuizTextures.ContainsKey (imgurl)) {
				tex = CerebroHelper.remoteQuizTextures [imgurl];
				yield return new WaitForSeconds (0.2f);
			} else {
				WWW remoteImage = new WWW (imgurl);
				yield return remoteImage;
				if (remoteImage.error == null) {
					tex = remoteImage.texture;
					if (!CerebroHelper.remoteQuizTextures.ContainsKey (imgurl)) {
						CerebroHelper.remoteQuizTextures.Add (imgurl, tex);
					}
				} else {
					print (remoteImage.error + ",for," + imgurl);
				}
			}
			if (tex != null) {
				if (!isProfileImage) {
					go.transform.Find ("ProgressCircle").gameObject.SetActive (false);
				}

				float holderWidth = graphic.gameObject.GetComponent<RectTransform> ().sizeDelta.x;//320f;
				float holderHeight = graphic.gameObject.GetComponent<RectTransform> ().sizeDelta.y;//278f;

				float holderAspectRatio = holderWidth / holderHeight;
				float imageAspectRatio = (float)tex.width / (float)tex.height;

				float scaleRatio = 1;
				if (holderAspectRatio >= imageAspectRatio) {			// scale to match width
					float actualImageWidth = tex.width * (holderHeight / tex.height);
					scaleRatio = holderWidth / actualImageWidth;
				} else {
					float actualImageHeight = tex.height * (holderWidth / tex.width);
					scaleRatio = holderHeight / actualImageHeight;
				}

				var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
				graphic.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
				graphic.GetComponent<Image> ().sprite = newsprite;
				graphic.GetComponent<RectTransform> ().localScale = new Vector3 (scaleRatio, scaleRatio, 1f);
			} else {
				if (!isProfileImage) {
					go.transform.Find ("ProgressCircle").gameObject.SetActive (false);
					StartCoroutine (LoadImage (imgurl, go, isProfileImage));
				}
			}
		}

		IEnumerator HideAnimation ()
		{
			Go.to (Question.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (Screen.width, 0, 0), true).setEaseType (GoEaseType.BackIn));
			Go.to (inputPanel.gameObject.transform, 0.3f, new GoTweenConfig ().position (new Vector3 (Screen.width, 0, 0), true).setEaseType (GoEaseType.BackIn));
			yield return new WaitForSeconds (0.5f);
			GetResponses ();
		}

		void ShowDisplayView ()
		{
			DisplayView.SetActive (true);
			QuestionView.SetActive (false);
			LoaderView.SetActive (false);
			ErrorView.SetActive (false);

			DisplayView.transform.Find ("MediaImage").SetAsFirstSibling ();
			DisplayView.transform.Find ("MediaVideo").SetAsFirstSibling ();
			DisplayView.transform.Find ("MediaImage").transform.localScale = new Vector2 (1f, 1f);
			DisplayView.transform.Find ("MediaVideo").transform.localScale = new Vector2 (1f, 1f);
			DisplayView.transform.Find ("MediaImage").gameObject.SetActive (true);
			DisplayView.transform.Find ("MediaVideo").gameObject.SetActive (true);
			DisplayView.transform.Find ("MediaImage").gameObject.GetChildByName<Graphic> ("Icon").GetComponent<Image> ().color = new Color (1, 1, 1, 0);
			if (mQuestion.MediaType == "Video") {
				DisplayView.transform.Find ("MediaImage").gameObject.SetActive (false);
			} else if (mQuestion.MediaType == "Image") {
				DisplayView.transform.Find ("MediaVideo").gameObject.SetActive (false);
				StartCoroutine (LoadImage (mQuestion.MediaURL, DisplayView.transform.Find ("MediaImage").gameObject,false));
			}

			GameObject box = DisplayView.transform.Find ("Boxes").Find ("Container").Find ("Box").gameObject;

			float boxWidth = box.GetComponent<RectTransform> ().sizeDelta.x;
			yPositions = new float[]{ -displayViewContainerTopPadding-padding, -displayViewContainerTopPadding-padding, -displayViewContainerTopPadding-padding };
			xPositions = new float[]{ padding, boxWidth + (padding*2), (boxWidth*2) + (padding*3) };

			Boxes = new List<GameObject> ();

			for (var i = 0; i < LaunchList.instance.mDescribeImageUserResponses.Count; i++) {
				GameObject newBox = GameObject.Instantiate (box);
				newBox.name = "Box" + i;
				newBox.transform.SetParent (box.transform.parent, false);
				newBox.transform.localScale = new Vector3 (1f, 1f, 1f);
				newBox.GetChildByName<Text> ("Text").text = LaunchList.instance.mDescribeImageUserResponses [i].UserResponse;
				newBox.GetChildByName<Text> ("Name").text = LaunchList.instance.mDescribeImageUserResponses [i].StudentName;
				if (LaunchList.instance.mDescribeImageUserResponses [i].StudentImageUrl != null) {
					StartCoroutine (LoadImage (LaunchList.instance.mDescribeImageUserResponses [i].StudentImageUrl, newBox.transform.Find ("ProfileImage").gameObject, true));
				}
				Boxes.Add (newBox);
			}
			StartCoroutine (ArrangeObjects ());
		}
		IEnumerator ArrangeObjects() {
			yield return new WaitForSeconds (0f);

			for (var i = 0; i < Boxes.Count; i++) {
				int yColumn = GetNextColumn ();
				Boxes [i].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (xPositions[yColumn], yPositions[yColumn]);
				float textHeight = Boxes [i].transform.Find ("Text").GetComponent<RectTransform> ().sizeDelta.y;
				float objectHeight = textHeight - Boxes [i].transform.Find ("Text").GetComponent<RectTransform> ().anchoredPosition.y + (textPadding);
				Boxes [i].transform.Find ("Image").GetComponent<RectTransform> ().sizeDelta = new Vector2 (Boxes [i].transform.Find ("Image").GetComponent<RectTransform> ().sizeDelta.x, objectHeight);
				Boxes [i].GetComponent<RectTransform> ().sizeDelta = new Vector2 (Boxes [i].GetComponent<RectTransform> ().sizeDelta.x, objectHeight);
				Color imageColor = new Color(1f,1f,1f,1f);
				Boxes [i].transform.Find ("Image").GetComponent<Image> ().color = imageColor;
				yPositions [yColumn] -= (objectHeight + padding);
			}
			GameObject box = DisplayView.transform.Find ("Boxes").Find ("Container").Find ("Box").gameObject;
			box.transform.parent.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, -GetMaxYValue ());
			box.SetActive (false);
		}

		int GetNextColumn() {
			float max = float.MinValue;
			int index = -1;
			for (var i = 0; i < yPositions.Length; i++) {
				if (yPositions [i] > max) {
					index = i;
					max = yPositions [i];
				}
			}
			return index;
		}

		float GetMaxYValue() {
			float min = 0;
			for (var i = 0; i < yPositions.Length; i++) {
				if (yPositions [i] < min) {
					min = yPositions [i];
				}
			}
			return min;
		}

		public void previousButtonPressed ()
		{
			submitClick (false);
		}

		public void videoIconPressed ()
		{
			_RemoveTextView ();
			VideoHelper.instance.VideoEnded += CloseWebView;
			VideoHelper.instance.OpenVideoWithUrl (mQuestion.MediaURL);
		}

		void CloseWebView(object sender, System.EventArgs e) {
			_AddTextView (textViewRect.x, textViewRect.y, textViewRect.width, textViewRect.height,charLimit,userAnswerText.text);
			VideoHelper.instance.VideoEnded -= CloseWebView;
		}

		void Update ()
		{
			#if UNITY_EDITOR
			Vector3 currentPos = inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition;
			Vector2 currentSize = inputPanel.transform.Find ("Subjective").GetComponent<RectTransform> ().sizeDelta;
			if (TouchScreenKeyboard.visible == true) {
				if (inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition.y != -377f) {
					inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition = new Vector3 (currentPos.x, -377f, currentPos.z);
					inputPanel.transform.Find ("Subjective").GetComponent<RectTransform> ().sizeDelta = new Vector2 (currentSize.x, 267f);
				}
			} else if(TouchScreenKeyboard.visible == false) {
				if (inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition.y != -730f) {
					inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition = new Vector3 (currentPos.x, -730f, currentPos.z);
					inputPanel.transform.Find ("Subjective").GetComponent<RectTransform> ().sizeDelta = new Vector2 (currentSize.x, 602f);
				}
			}
			#endif
		}

		#if UNITY_IOS && !UNITY_EDITOR
		public void KeyboardShow(string dummy)
		{
			Vector3 currentPos = inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition;
			inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition = new Vector3 (currentPos.x, -377f, currentPos.z);
		}

		public void KeyboardHide(string dummy)
		{
			Vector3 currentPos = inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition;
			inputPanel.transform.Find ("BottomPanel").GetComponent<RectTransform> ().localPosition = new Vector3 (currentPos.x, -730f, currentPos.z);
		}

		public void GetTextFieldString(string text)
		{
			print ("got back from native with string "+text);
			userAnswerText.text = text;
			int currentCharLength = userAnswerText.text.Length;
			inputFieldCharLimit.text = (charLimit - currentCharLength).ToString ();
		}
		#endif

		[DllImport ("__Internal")]
		private static extern void _AddTextView (float x, float y, float width, float height, float charLimit, string text);
		[DllImport ("__Internal")]
		private static extern void _RemoveTextView();
	}
}
