using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

namespace Cerebro
{
	public class GenericPopup : MonoBehaviour
	{
		public delegate void OkClicked();
		public OkClicked OnOkClicked;

		public delegate void CancelClicked();
		public CancelClicked OnCancelClicked;

		GameObject Card;
		Text questionObject;

		private bool IsPortrait, IsLandscapeLeft;

		void Start ()
		{
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			GetComponent<RectTransform> ().anchoredPosition = new Vector3 (0f, 0f);
		}

		public void Initialise (string Question, int NumberOfButton, bool _IsPortrait, OkClicked OkFunction = null, CancelClicked CancelFunction = null, Sprite Icon = null)
		{
			IsPortrait = _IsPortrait;
			Card = transform.Find ("Card").gameObject;
			if (IsPortrait) {
				IsLandscapeLeft = true;
				transform.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, 90);
				if (IsLandscapeLeft && Screen.orientation == ScreenOrientation.LandscapeRight) {
					Debug.Log ("calling right from update " + IsLandscapeLeft);
					IsLandscapeLeft = false;
					transform.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, -90);
				} else if (!IsLandscapeLeft && Screen.orientation == ScreenOrientation.LandscapeLeft) {
					Debug.Log ("calling left from update " + IsLandscapeLeft);
					transform.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, 90);
					IsLandscapeLeft = true;
				}
//				Card.transform.position = new Vector3 (-384f, Card.transform.position.y, 0f);
//				Go.to (Card.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (309f, Card.transform.position.y, 0), false).setEaseType (GoEaseType.BackOut));
			} else {
//				Card.transform.position = new Vector3 (Card.transform.position.x, -384f, 0f);
//				Go.to (Card.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (Card.transform.position.x, 232f, 0), false).setEaseType (GoEaseType.BackOut));
			}
			if (Icon != null) {
				Color curr = Card.transform.FindChild ("Icon").GetComponent<Image> ().color;
				Card.transform.FindChild ("Icon").GetComponent<Image> ().color = new Color (curr.r, curr.g, curr.b, 1.0f);
				Card.transform.FindChild ("Icon").GetComponent<Image> ().sprite = Icon;
			}
			if (NumberOfButton == 1) {
				GameObject ButtonParent = Card.transform.FindChild ("Buttons").gameObject;
				ButtonParent.transform.FindChild("OkButton").GetComponent<RectTransform> ().anchorMin = new Vector2 (0, 0);
				Debug.Log (ButtonParent.transform.FindChild("OkButton").GetComponent<RectTransform> ().anchoredPosition);
				ButtonParent.transform.FindChild ("CancelButton").gameObject.SetActive (false);
				ButtonParent.transform.FindChild ("Separator").gameObject.SetActive (false);
			}
			questionObject = Card.transform.Find ("Question").GetComponent<Text> ();
			questionObject.text = Question;
			OnOkClicked = OkFunction;
			OnCancelClicked = CancelFunction;
		}

		void Update()
		{
			if (IsPortrait) {
				if (IsLandscapeLeft && Screen.orientation == ScreenOrientation.LandscapeRight) {
					Debug.Log ("calling right from update " + IsLandscapeLeft);
					IsLandscapeLeft = false;
					transform.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, -90);
				} else if (!IsLandscapeLeft && Screen.orientation == ScreenOrientation.LandscapeLeft) {
					Debug.Log ("calling left from update " + IsLandscapeLeft);
					transform.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0, 0, 90);
					IsLandscapeLeft = true;
				}
			}
		}

		public void OkPressed() {
			if (OnOkClicked != null) {
				OnOkClicked ();
			}
			BackPressed ();
		}

		public void CancelPressed() {
			if (OnCancelClicked != null) {
				OnCancelClicked ();
			}
			BackPressed ();
		}

		public void BackPressed ()
		{
			Destroy (gameObject);
		}
	}
}
