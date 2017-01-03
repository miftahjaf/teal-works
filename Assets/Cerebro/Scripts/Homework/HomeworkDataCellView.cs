using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using SimpleJSON;

namespace Cerebro {

	public class HomeworkDataCellView : EnhancedScrollerCellView {

		public Image thumbnailSprite;
		public Text titleText;
		public Text detailsText;
		public Text dueTimeText;
		public Sprite defaultSprite;

		private HomeworkDataCell mCurrDataCell;
		public HomeworkDataCell currDataCell
		{
			get { return mCurrDataCell; }
			set { mCurrDataCell = value; }
		}

		private GameObject parentForAssignment;
		private System.DateTime announcementReadTime;

		public void InitializeCell(HomeworkDataCell dataCell, GameObject _parentForAssignment)
		{
			currDataCell = dataCell;
			if (currDataCell.questionType == HomeworkDataCell.QuestionType.wc) {
				if (currDataCell.wcData != null && currDataCell.wcData.promptText != null && currDataCell.wcData.promptText.Length > 0) {
					if (currDataCell.wcData.promptText.Length > 30) {
						titleText.text = currDataCell.wcData.promptText.Substring (0, Mathf.Min (currDataCell.wcData.promptText.Length, 27)) + "...";
					} else {
						titleText.text = currDataCell.wcData.promptText;
					}
					titleText.text = currDataCell.wcData.promptText.Substring (0, Mathf.Min (currDataCell.wcData.promptText.Length, 30));
				} else {
					titleText.text = "Writer's Corner";
				}
			} else if (currDataCell.questionType == HomeworkDataCell.QuestionType.announcement) {
				if (currDataCell.announcementData != null && currDataCell.announcementData.announcementText != null && currDataCell.announcementData.announcementText.Length > 0) {
					if (currDataCell.announcementData.announcementText.Length > 30) {
						titleText.text = currDataCell.announcementData.announcementText.Substring (0, Mathf.Min (currDataCell.announcementData.announcementText.Length, 27)) + "...";
					} else {
						titleText.text = currDataCell.announcementData.announcementText;
					}
				} else {
					titleText.text = "Announcement";
				}
			}
			detailsText.text = currDataCell.createdBy != null ? ("By " + currDataCell.createdBy.firstName + " " + currDataCell.createdBy.lastName) : "";
			detailsText.text += currDataCell.subject.Length > 0 ? ((detailsText.text.Length > 0 ? " | " : "") + currDataCell.subject) : "";
			int date = int.Parse(currDataCell.dueTime.ToString ("dd"));
			dueTimeText.text = "Submit By:"+ AddOrdinal(date) + " " + currDataCell.dueTime.ToString("MMM");
			parentForAssignment = _parentForAssignment;
			if (currDataCell.thumbnailSprite == null) {
				thumbnailSprite.sprite = defaultSprite;
				if (currDataCell.thumbnailUrl != null && currDataCell.thumbnailUrl.Length > 0) {
					Invoke ("LoadThumbnailImage", 0.01f);
				}
			} else {
				thumbnailSprite.sprite = currDataCell.thumbnailSprite;
			}
		}

		private string AddOrdinal(int num)
		{
			switch(num % 100)
			{
			case 11:
			case 12:
			case 13:
				return num + "th";
			}

			switch(num % 10)
			{
			case 1:
				return num + "st";
			case 2:
				return num + "nd";
			case 3:
				return num + "rd";
			default:
				return num + "th";
			}

		}

		void LoadThumbnailImage()
		{
			if (!transform.parent.gameObject.activeSelf) {
				return;
			}

			StartCoroutine (LoadThumbnail());
		}

		IEnumerator LoadThumbnail()
		{
			HomeworkDataCell reqDataCell = currDataCell;
			string imgurl = reqDataCell.thumbnailUrl;
			if (imgurl.Length <= 0)
				yield break;
			
			Texture2D tex = null;
			if (CerebroHelper.remoteQuizTextures.ContainsKey (imgurl)) {
				tex = CerebroHelper.remoteQuizTextures [imgurl];
				yield return new WaitForSeconds (0.1f);
			} else {
				WWW remoteImage = new WWW (imgurl);
				yield return remoteImage;
				if (remoteImage.error == null) {
					tex = remoteImage.texture;
					if (!CerebroHelper.remoteQuizTextures.ContainsKey (imgurl)) {
						CerebroHelper.remoteQuizTextures.Add (imgurl, tex);
					}
				} else {
					print (remoteImage.error + ",for," + imgurl + " " + imgurl.Length);
				}
			}

			if (tex != null) {
				var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
				reqDataCell.thumbnailSprite = newsprite;
				if (currDataCell == reqDataCell) {
					thumbnailSprite.color = new Color (1, 1, 1, 1);
					thumbnailSprite.sprite = newsprite;
				}
			}
		}

		public override void RefreshCellView()
		{
			
		}

		public void OnAnnouncementSent(bool isSuccess)
		{
			Debug.Log ("announcement sent "+isSuccess);
			if (!isSuccess) {
				SaveResponseLocal ();
			}
		}

		void SaveResponseLocal()
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
			while (N ["Data"] ["Announcement"] [id] != null) {
				id = System.Guid.NewGuid ().ToString ();
			}
			N ["Data"] ["Announcement"] [id] ["id"] = id;
			N ["Data"] ["Announcement"] [id] ["contextId"] = currDataCell.contextId;
			N ["Data"] ["Announcement"] [id] ["createdAt"] = LaunchList.instance.ConvertDateToStandardString(announcementReadTime);
			System.IO.File.WriteAllText (fileName, N.ToString());
		}

		public void OnCellClicked()
		{
			if (currDataCell.questionType == HomeworkDataCell.QuestionType.wc) {
				GameObject ResponseGm = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.AssignmentResponse, parentForAssignment.transform);
				ResponseGm.GetComponent<AssignmentResponseView> ().currDataCell = currDataCell;
				ResponseGm.GetComponent<AssignmentResponseView> ().Initialize ();
			} else if (currDataCell.questionType == HomeworkDataCell.QuestionType.announcement) {
				GameObject AnnouncementGM = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.AnnouncementView, parentForAssignment.transform);
				AnnouncementGM.GetComponent<AnnouncementView> ().Initialize (currDataCell.announcementData.announcementText);
				announcementReadTime = System.DateTime.Now.Subtract(new System.TimeSpan(5, 30, 0));
				HTTPRequestHelper.instance.SendHomeworkResponseAnnouncement (currDataCell.contextId, LaunchList.instance.ConvertDateToStandardString(announcementReadTime), OnAnnouncementSent);
			}
		}

	}

}
