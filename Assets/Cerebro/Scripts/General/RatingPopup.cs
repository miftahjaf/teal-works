using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace Cerebro
{
	public class RatingPopup : MonoBehaviour
	{
		public delegate void OkClicked();
		public OkClicked OnOkClicked;

		private string mStarNormalColor = "E2E2E2";
		private string mStarHighlightColor = "29CDB1";

		string mVideoID;
		float mTimeSpent;
		string mType;
		int mRating;

		GameObject Stars;
		GameObject Card;
		Text questionObject;

		void Start ()
		{
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0f, 0f);
		}

		public void Initialise (string type, float timeSpent, string videoID, string question, OkClicked OkFunction = null)
		{
			mVideoID = videoID;
			mTimeSpent = timeSpent;
			mType = type;
			Card = transform.Find ("Card").gameObject;
			questionObject = Card.transform.Find ("Question").GetComponent<Text> ();
			Stars = Card.transform.Find ("Stars").gameObject;
			OnOkClicked = OkFunction;

			questionObject.text = question;

			StartCoroutine (SetCardSize ());
		}

		IEnumerator SetCardSize() {
			yield return new WaitForSeconds (0);

			float starsPosX = Stars.GetComponent<RectTransform> ().anchoredPosition.x;
			float starsPosY = questionObject.gameObject.GetComponent<RectTransform> ().anchoredPosition.y - questionObject.gameObject.GetComponent<RectTransform> ().sizeDelta.y - 64f;
			Stars.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (starsPosX, starsPosY);

			float cardSize = -(Stars.GetComponent<RectTransform> ().anchoredPosition.y - Stars.GetComponent<RectTransform> ().sizeDelta.y - 64f - 64f);
			Card.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Card.GetComponent<RectTransform> ().sizeDelta.x, cardSize);

			HighlightStars (0);
		}

		public void StarPressed (int number)
		{
			mRating = number;
			HighlightStars (number);	
		}

		public void OkPressed() {
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			HTTPRequestHelper.instance.SendRatingInfo (mType, studentID, mVideoID, mTimeSpent, mRating);
			if (OnOkClicked != null)
				OnOkClicked ();

			BackPressed ();
		}

		void HighlightStars(int number) {
			for (var i = 0; i < Stars.transform.childCount; i++) {
				if (i < number) {
					Stars.transform.GetChild (i).GetComponent<Image> ().color = CerebroHelper.HexToRGB (mStarHighlightColor);
				} else {
					Stars.transform.GetChild (i).GetComponent<Image> ().color = CerebroHelper.HexToRGB (mStarNormalColor);
				}
			}
		}

		public void BackPressed ()
		{
			Destroy (gameObject);
		}
	}
}
