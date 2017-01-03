using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using System.IO;
using System;
using UnityEngine.EventSystems;
using System.Runtime.InteropServices;
using SimpleJSON;

namespace Cerebro
{
	public class WritersCornerView : MonoBehaviour
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

		public HomeworkDataCell currDataCell;
		private WritersCornerData currQuestion;

		private bool isImageFullScreen = false;

		public Text Question;

//		private DescribeImage mQuestion;

		private bool isAnimating = false;
		public GameObject fsImage;

		private Vector3 questionTextOriginalPosition;
		private Vector3 inputPanelOriginalPosition;
		private int charLimit = 500;

		private string TimeStarted;
		private System.DateTime creationTime;

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

			currQuestion = currDataCell.wcData;

			inputFieldCharLimit.text = charLimit.ToString ();

			userAnswerText = subjectivePanel.transform.Find ("InputField").gameObject.GetChildByName<Text> ("Text");

			Initialise ();
		}

		void Initialise() {
			if (currQuestion == null) {
				ErrorView.SetActive (true);
				ErrorView.transform.Find ("Text").GetComponent<Text> ().text = "No Internet Connection\nThis feature requires internet connection";
				ProgressCircle.SetActive (false);
			} else if (currQuestion.id == null) {
				ErrorView.SetActive (true);
				ErrorView.transform.Find ("Text").GetComponent<Text> ().text = "No more images\nPlease check back later! ";
				ProgressCircle.SetActive (false);
			} else {
				print ("Initializing");
				Initialize ();
			}
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

		private void showNextQuestion ()
		{
			print ("showNextQuestion");
			Question.text = currQuestion.promptText;

			if (currQuestion.mediaType == "Video") {
				inputPanel.transform.Find ("MediaVideo").SetAsLastSibling ();
				inputPanel.transform.Find ("MediaVideo").gameObject.SetActive (true);
				inputPanel.transform.Find ("MediaImage").gameObject.SetActive (false);
				var firstSplit = currQuestion.mediaUrl.Split ("v="[1]);
				var videoID = firstSplit [1].Split ("&"[0])[0];
				var videoUrl = "https://www.youtube.com/embed/" + videoID;
				var imgurl = "https://img.youtube.com/vi/" + videoID + "/default.jpg";
				StartCoroutine (LoadThumbnail(currQuestion.thumbnailUrl, currQuestion.id));
			} else if (currQuestion.mediaType == "Image") {
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

			charLimit = currQuestion.wordLimit;
			inputFieldCharLimit.text = charLimit.ToString ();

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
			_AddTextView (textViewRect.x, textViewRect.y, textViewRect.width, textViewRect.height,currQuestion.wordLimit,userAnswerText.text);
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
				#if UNITY_IOS && !UNITY_EDITOR
				_AddTextView (textViewRect.x, textViewRect.y, textViewRect.width, textViewRect.height,currQuestion.wordLimit,userAnswerText.text);
				#endif
			} else {
				#if UNITY_IOS && !UNITY_EDITOR
				_RemoveTextView ();
				#endif
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

			#if UNITY_IOS && !UNITY_EDITOR
			_RemoveTextView();
			#endif
			LoaderView.SetActive (true);
			creationTime = System.DateTime.Now.Subtract(new TimeSpan(5, 30, 0));

			HTTPRequestHelper.instance.SendHomeworkResponseWC (currDataCell.contextId, LaunchList.instance.ConvertDateToStandardString(creationTime), userAnswerText.text, OnSaveResponse);
		}

		public void OnSaveResponse(bool IsSuccess)
		{
			if (!IsSuccess) {
				string id = SaveResponseLocal ();
				WCResponse res = new WCResponse (id, userAnswerText.text, creationTime);
				currDataCell.wcData.userResponses.Add (res);
			}
			LoaderView.SetActive (false);
			ParentForWC parent = gameObject.transform.parent.GetComponent<ParentForWC> ();
			parent.BackFromWC ();
			StartCoroutine (HideAnimation ());
		}

		string SaveResponseLocal()
		{
			string fileName = Application.persistentDataPath + "/HomeworkResponseLocalJSON.txt";
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (System.IO.File.Exists (fileName)) {				
				string data = System.IO.File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				System.IO.File.WriteAllText (fileName, string.Empty);
			} else {
				N ["VersionNumber"] = LaunchList.instance.VersionData;
			}
			string id = System.Guid.NewGuid ().ToString ();
			while (N ["Data"] ["Response"] [id] != null) {
				id = System.Guid.NewGuid ().ToString ();
			}
			N ["Data"] ["Response"] [id] ["id"] = id;
			N ["Data"] ["Response"] [id] ["contextId"] = currDataCell.contextId;
			N ["Data"] ["Response"] [id] ["createdAt"] = LaunchList.instance.ConvertDateToStandardString(creationTime);
			N ["Data"] ["Response"] [id] ["responseData"] = userAnswerText.text;
			System.IO.File.WriteAllText (fileName, N.ToString());
			return id;
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

				ParentForWC parent = gameObject.transform.parent.GetComponent<ParentForWC> ();
				parent.BackFromWC ();

				StartCoroutine(HideAnimation());
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
			if (currQuestion.mediaType == "Image") {
				StartCoroutine (LoadImage (currQuestion.mediaUrl, inputPanel.transform.Find ("MediaImage").gameObject, false));
			}
		}

		IEnumerator LoadThumbnail(string imgurl, string id)
		{
			if (imgurl.Length <= 0)
				yield break;
			
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
				GameObject gm = inputPanel.transform.Find ("MediaVideo").FindChild ("BG").gameObject;
				if (id == currQuestion.id && gm != null) {
					var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
					gm.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
					gm.GetComponent<Image> ().sprite = newsprite;
				}
			}
		}

		IEnumerator LoadImage (string imgurl, GameObject go, bool isProfileImage)
		{
			CerebroHelper.DebugLog ("Loading Image " + imgurl);
			if (imgurl.Length <= 0)
				yield break;
			
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
			Destroy (gameObject);
		}

		public void previousButtonPressed ()
		{
			submitClick (false);
		}

		public void videoIconPressed ()
		{
			_RemoveTextView ();
			VideoHelper.instance.VideoEnded += CloseWebView;
			VideoHelper.instance.OpenVideoWithUrl (currQuestion.mediaUrl);
		}

		void CloseWebView(object sender, System.EventArgs e) {
			_AddTextView (textViewRect.x, textViewRect.y, textViewRect.width, textViewRect.height,currQuestion.wordLimit,userAnswerText.text);
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
