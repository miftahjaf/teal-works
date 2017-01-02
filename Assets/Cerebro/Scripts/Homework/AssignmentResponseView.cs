using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using MaterialUI;
using SimpleJSON;

namespace Cerebro
{
	public class AssignmentResponseView : MonoBehaviour 
	{
		public static float KeyboardHeight = 345f;

		public Transform QuestionView;
		public GameObject ResponseSelectorGm;
		public InputField CommentTextfield;
		[SerializeField]
		private GameObject ProgressCircle;
		public GameObject FullScreenImage;
		public GameObject AssessmentListGm;
		public bool ScrollListToEnd = false;

		private WritersCornerData currData;
		private ResponseSelector responseSelector;
		private System.DateTime commentCreationTime;
		private bool isImageFullScreen;
		private GameObject parentForWC;
		public Text tempText;

		public HomeworkDataCell currDataCell;

		void OnEnable()
		{
			if (PluginMsgHandler.getInst () != null) {
				PluginMsgHandler.getInst ().OnShowKeyboard += OnShowKeyboard;
			}
			WelcomeScript.instance.onDashboardClicked += DashboardClicked;
		}

		void OnDisable()
		{
			CommentTextfield.GetComponent<NativeEditBox>().SetFocusNative (false);
			if (PluginMsgHandler.getInst () != null) {
				PluginMsgHandler.getInst ().OnShowKeyboard -= OnShowKeyboard;
			}
			WelcomeScript.instance.onDashboardClicked -= DashboardClicked;
		}

		void Start () 
		{
			responseSelector = ResponseSelectorGm.GetComponent<ResponseSelector> ();
			parentForWC = transform.FindChild ("ParentForWC").gameObject;
			#if !UNITY_EDITOR
				CommentTextfield.placeholder.gameObject.SetActive (false);
				CommentTextfield.textComponent.gameObject.SetActive (false);
			#endif
		}

		public void Initialize()
		{
			if (currDataCell.questionType == HomeworkDataCell.QuestionType.wc) {
				currData = currDataCell.wcData;
				QuestionView.transform.Find ("QuestionText").GetComponent<Text> ().text = currData.promptText;
				if (currData.mediaType == "Video") {
					QuestionView.transform.Find ("MediaVideo").SetAsLastSibling ();
					QuestionView.transform.Find ("MediaVideo").gameObject.SetActive (true);
					QuestionView.transform.Find ("MediaImage").gameObject.SetActive (false);
					var firstSplit = currData.mediaUrl.Split ("v=" [1]);
					var videoID = firstSplit [1].Split ("&" [0]) [0];
					var videoUrl = "https://www.youtube.com/embed/" + videoID;
					var imgurl = "https://img.youtube.com/vi/" + videoID + "/default.jpg";
					StartCoroutine (LoadThumbnail (currData.thumbnailUrl, currData.id));
				} else if (currData.mediaType == "Image") {
					QuestionView.transform.Find ("MediaImage").SetAsLastSibling ();
					QuestionView.transform.Find ("MediaVideo").gameObject.SetActive (false);
					QuestionView.transform.Find ("MediaImage").gameObject.SetActive (true);
					StartCoroutine (LoadImage (currData.mediaUrl, QuestionView.transform.Find ("MediaImage").gameObject, false));
					QuestionView.transform.Find ("MediaImage").gameObject.GetChildByName<Graphic> ("Icon").GetComponent<Image> ().color = new Color (1, 1, 1, 0);
				} else {
					QuestionView.transform.Find ("MediaVideo").gameObject.SetActive (false);
					QuestionView.transform.Find ("MediaImage").gameObject.SetActive (false);
				}
			}

			RefreshResponseFeed ();
		}

		public void RefreshResponseFeed()
		{
			ProgressCircle.SetActive (true);
			HTTPRequestHelper.instance.GetResponseFeed (currDataCell.contextId, OnFeedResponse);
		}

		public void OnFeedResponse(JSONNode jsonNode)
		{
			if (jsonNode != null) {
				currDataCell.FillResponseData (jsonNode);
				SaveHomeworkResponseToLocal ();
				ReloadData ();
				ProgressCircle.SetActive (false);
			}
		}

		void SaveHomeworkResponseToLocal()
		{
			string fileName = Application.persistentDataPath + "/HomeworkFeedJSON.txt";
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (System.IO.File.Exists (fileName)) {				
				string data = System.IO.File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				System.IO.File.WriteAllText (fileName, string.Empty);
			}
			N ["Data"] ["Homeworks"] [currDataCell.id] ["ResponseFeed"] = currDataCell.ConvertResponseToJson ();
			System.IO.File.WriteAllText (fileName, N.ToString());
		}

		public void ReloadData()
		{
//			responseSelector.GetComponent<CanvasGroup> ().alpha = 0f;
			List<ResponseData> responseData = new List<ResponseData> ();
			int cnt = currDataCell.responseFeed.Count;
			List<ResponseFeed> rFeed = currDataCell.responseFeed;
			for (int i = 0; i < cnt; i++) {
				ResponseData rd = new ResponseData ();
				if (rFeed[i].responseType == ResponseFeed.ResponseType.Submission) {
					WCResponse res = currDataCell.wcData.userResponses.Find (x => x.id == rFeed[i].dataId);
					if (res != null) {
						rd.id = res.id;
						rd.title = LaunchList.instance.mCurrentStudent.StudentName;
						tempText.text = res.userResponse;
						rd.cellSize = Mathf.Max (80f, 40f + tempText.preferredHeight);
						rd.responseText = res.userResponse;
						rd.createdAt = res.createdAt;
						rd.responseType = ResponseData.ResponseType.Submission;
						rd.isLateSubmission = currDataCell.isLateSubmission;
						rd.from = ResponseData.CommentFrom.Me;
						rd.isFromLocal = rFeed [i].isFromLocal;
						responseData.Add (rd);
					}
				} else {
					HomeworkComment cmt = currDataCell.comments.Find (x => x.id == rFeed[i].dataId);
					if (cmt != null) {
						rd.id = cmt.id;
						if (cmt.fromType == HomeworkComment.CommentFrom.Me) {
							rd.title = LaunchList.instance.mCurrentStudent.StudentName;
							rd.from = ResponseData.CommentFrom.Me;
						} else {
							rd.title = currDataCell.teacherData [cmt.fromId].firstName + " " + currDataCell.teacherData [cmt.fromId].lastName;
							rd.from = ResponseData.CommentFrom.Teacher;
						}
						tempText.text = cmt.comment;
						rd.cellSize = Mathf.Max (80f, 40f + tempText.preferredHeight);
						rd.responseText = cmt.comment;
						rd.createdAt = cmt.createdAt;
						rd.responseType = ResponseData.ResponseType.Comment;
						rd.isFromLocal = rFeed [i].isFromLocal;
						responseData.Add (rd);
					}
				}
			}
			tempText.text = "";
			responseSelector.currResponses = responseData;
			responseSelector.ReloadData ();
			AssessmentListGm.GetComponent<HomeworkAssessments> ().InitializeAssessments (currDataCell);
//			StopCoroutine ("WaitForTextAdjust");
//			StartCoroutine ("WaitForTextAdjust", fromUpdate);
			if (ScrollListToEnd) {
				responseSelector.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0f;
			}
			responseSelector.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0f;
			ScrollListToEnd = false;
		}

		IEnumerator WaitForTextAdjust(bool fromUpdate = false)
		{
			yield return new WaitForEndOfFrame();
			responseSelector.ReloadData ();
			yield return new WaitForEndOfFrame();
			responseSelector.ReloadData ();
			responseSelector.GetComponent<CanvasGroup> ().alpha = 1f;
			if (fromUpdate) {
				responseSelector.GetComponent<ScrollRect> ().verticalNormalizedPosition = 0f;
			}
		}

		IEnumerator LoadThumbnail(string imgurl, string id)
		{
			Texture2D tex = null;
			WWW remoteImage = new WWW (imgurl);
			yield return remoteImage;
			if (remoteImage.error == null) {
				GameObject gm = QuestionView.transform.Find ("MediaVideo").FindChild ("BG").gameObject;
				if (id == currData.id && gm != null) {
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

		public void OnBackPressed()
		{
			if (isImageFullScreen) {
				FullScreenImage.SetActive (false);
				isImageFullScreen = false;
			} else {
				transform.parent.GetComponent<ParentForAssignment> ().BackFromAssignment ();
				Destroy (this.gameObject);
			}
		}

		public void AddResponseButton()
		{
			GameObject WC = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.WritersCornerForHomework, parentForWC.transform);
			WC.GetComponent<WritersCornerView> ().currDataCell = currDataCell;
		}

		public void imagePressed ()
		{
			if (isImageFullScreen) {
				FullScreenImage.SetActive (false);
				isImageFullScreen = false;
				CommentTextfield.GetComponent<NativeEditBox> ().SetVisible (true);
			} else {
				CommentTextfield.GetComponent<NativeEditBox> ().SetVisible (false);
				var sprite = QuestionView.transform.Find ("MediaImage").gameObject.GetChildByName<Graphic> ("Icon").GetComponent<Image> ().sprite;
				if (sprite != null) {
					FullScreenImage.SetActive (true);
					FullScreenImage.transform.SetAsLastSibling ();
					isImageFullScreen = true;
					FullScreenImage.transform.Find ("Image").gameObject.GetComponent<Image> ().sprite = sprite;
				}
			}
		}

		public void videoIconPressed ()
		{
			CommentTextfield.GetComponent<NativeEditBox> ().SetVisible (false);
			VideoHelper.instance.VideoEnded += CloseWebView;
			VideoHelper.instance.OpenVideoWithUrl (currData.mediaUrl);
		}

		void CloseWebView(object sender, System.EventArgs e) {
//			CommentTextfield.GetComponent<NativeEditBox> ().SetVisible (true);
			VideoHelper.instance.VideoEnded -= CloseWebView;
		}

		public void OnShowKeyboard(bool bKeyboardShow, int nKeyHeight)
		{
			Vector2 pos = CommentTextfield.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition;
			CommentTextfield.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (pos.x, (bKeyboardShow ? 345f : 0f));
			CommentTextfield.GetComponent<NativeEditBox>().SetRectNative (CommentTextfield.transform.GetComponent<RectTransform> ());
		}

		public void sendButtonClicked ()
		{
			commentCreationTime = System.DateTime.Now.Subtract(new System.TimeSpan(5, 30, 0));
			ProgressCircle.SetActive (true);
			Vector2 pos = CommentTextfield.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition;
			CommentTextfield.transform.parent.gameObject.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (pos.x, 0f);
			HTTPRequestHelper.instance.SendHomeworkComment (currDataCell.contextId, LaunchList.instance.ConvertDateToStandardString(commentCreationTime), currDataCell.createdBy.id, CommentTextfield.text, OnSaveComment);
		}

		public void DashboardClicked()
		{
			CommentTextfield.GetComponent<NativeEditBox> ().SetVisible (false);
		}

		public void OnSaveComment(bool IsSuccess)
		{
			if (IsSuccess) {
				ScrollListToEnd = true;
				RefreshResponseFeed ();
			} else {
				string id = SaveCommentLocal ();
				ProgressCircle.SetActive (false);
				HomeworkComment cmt = new HomeworkComment (id, CommentTextfield.text, commentCreationTime, LaunchList.instance.mCurrentStudent.StudentID, HomeworkComment.CommentFrom.Me);
				currDataCell.comments.Add (cmt);
				currDataCell.responseFeed.Add(new ResponseFeed(id, true, ResponseFeed.ResponseType.Comment));
				ScrollListToEnd = true;
				ReloadData ();
			}
			CommentTextfield.text = "";
			CommentTextfield.GetComponent<NativeEditBox> ().SetTextNative ("");
		}

		string SaveCommentLocal()
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
			while (N ["Data"] ["Comment"] [id] != null) {
				id = System.Guid.NewGuid ().ToString ();
			}
			N ["Data"] ["Comment"] [id] ["id"] = id;
			N ["Data"] ["Comment"] [id] ["contextId"] = currDataCell.contextId;
			N ["Data"] ["Comment"] [id] ["createdAt"] = LaunchList.instance.ConvertDateToStandardString(commentCreationTime);
			N ["Data"] ["Comment"] [id] ["teacherId"] = currDataCell.createdBy.id;
			N ["Data"] ["Comment"] [id] ["commentData"] = CommentTextfield.text;
			System.IO.File.WriteAllText (fileName, N.ToString());
			return id;
		}
	}
}
