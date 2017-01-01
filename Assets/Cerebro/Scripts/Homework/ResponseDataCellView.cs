using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace Cerebro {

	public class ResponseDataCellView : EnhancedScrollerCellView {

		public Image profilePicSprite;
		public Text titleText;
		public Text submissionText;
		public Text lateSubmissionText;
		public Text responseText;
		public Text createdTimeText;
		public Sprite defaultSprite;

		private ResponseData mCurrDataCell;
		public ResponseData currDataCell
		{
			get { return mCurrDataCell; }
			set { mCurrDataCell = value; }
		}

		public void InitializeCell(ResponseData dataCell)
		{
			currDataCell = dataCell;
			titleText.text = currDataCell.title;
			responseText.text = currDataCell.responseText;
			lateSubmissionText.gameObject.SetActive (false);
			submissionText.gameObject.SetActive (false);
			if (currDataCell.responseType == ResponseData.ResponseType.Submission) {
				if (currDataCell.isLateSubmission) {
					lateSubmissionText.gameObject.SetActive (true);
				} else {
					submissionText.gameObject.SetActive (true);
				}
			}
			var sizeY = responseText.preferredHeight;
			if (sizeY > 0)
			{
//				// if the size has been set by the content size fitter, then we add in some padding so the
//				// the text isn't up against the border of the cell
//				sizeY += textBuffer.top + textBuffer.bottom;
				dataCell.cellSize = Mathf.Max (80f, 40f + sizeY);
				responseText.GetComponent<RectTransform> ().sizeDelta = new Vector2 (responseText.GetComponent<RectTransform> ().sizeDelta.x, responseText.preferredHeight);
			}

			int date = int.Parse(currDataCell.createdAt.ToString ("dd"));
			createdTimeText.text = AddOrdinal(date) + " " + currDataCell.createdAt.ToString("MMM");
			if (currDataCell.profilePicSprite == null) {
				profilePicSprite.sprite = defaultSprite;
				if (currDataCell.from == ResponseData.CommentFrom.Me) {
					Invoke ("LoadProfilePicImage", 0.01f);
				} else if (currDataCell.profilePicUrl != null && currDataCell.profilePicUrl.Length > 0) {
					Invoke ("LoadProfilePicImage", 0.01f);
				}
			} else {
				profilePicSprite.sprite = currDataCell.profilePicSprite;
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

		void LoadProfilePicImage()
		{
			if (!transform.parent.gameObject.activeSelf) {
				return;
			}

			StartCoroutine (LoadProfilePic());
		}

		IEnumerator LoadProfilePic()
		{
			Texture2D tex = null;
			ResponseData reqDataCell = currDataCell;
			if (reqDataCell.from == ResponseData.CommentFrom.Me) {
				yield return new WaitForSeconds (0.1f);
				tex = FetchImageFromFile (Application.persistentDataPath + "/ProfileImage.jpg");
				if (tex != null) {
					var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
					reqDataCell.profilePicSprite = newsprite;
					if (currDataCell == reqDataCell) {
						profilePicSprite.color = new Color (1, 1, 1, 1);
						profilePicSprite.sprite = newsprite;
					}
				}
			} else {
				WWW remoteImage = new WWW (reqDataCell.profilePicUrl);
				yield return remoteImage;
				if (remoteImage.error == null) {
					tex = remoteImage.texture;
					var newsprite = Sprite.Create (tex, new Rect (0f, 0f, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
					reqDataCell.profilePicSprite = newsprite;
					if (currDataCell == reqDataCell) {
						profilePicSprite.color = new Color (1, 1, 1, 1);
						profilePicSprite.sprite = newsprite;
					}
				} else {
					print (remoteImage.error + ",for," + reqDataCell.profilePicUrl + " " + reqDataCell.profilePicUrl.Length);
				}
			}
		}

		private Texture2D FetchImageFromFile (string fileName)
		{
			byte[] fileData;
			Texture2D tex = null;
			if (System.IO.File.Exists (fileName)) {
				fileData = System.IO.File.ReadAllBytes (fileName);
				tex = new Texture2D (1, 1);
				tex.LoadImage (fileData);
			}
			return tex;
		}

		public override void RefreshCellView()
		{

		}

	}

}
