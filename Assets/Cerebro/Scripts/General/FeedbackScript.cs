using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;

namespace Cerebro
{
	public class FeedbackScript : MonoBehaviour
	{
		public GameObject BottomPanel;
		public GameObject LoaderScreen;
		public Text FormTextObject;
		string formText = "";

		// Use this for initialization
		void Start ()
		{
			gameObject.name = "Feedback";

			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Feedback);

			GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			#if UNITY_IOS && !UNITY_EDITOR
			_AddFeedbackTextView (32f, 96f, 960f, 590f);
			FormTextObject.transform.parent.parent.parent.gameObject.SetActive(false);
			#endif
		}

		public void BackPressed() {
			#if UNITY_IOS && !UNITY_EDITOR
			_RemoveFeedbackTextView();
			#endif

			Destroy (gameObject);
			WelcomeScript.instance.ShowScreen (false);
		}

		public void SubmitPressed() {
			#if UNITY_IOS && !UNITY_EDITOR
			_RemoveFeedbackTextView();
			#else
			formText = FormTextObject.text;
			#endif

			CerebroHelper.DebugLog (formText);
			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
			HTTPRequestHelper.instance.SendFeedback (studentID,formText,FeedbackSent);

			LoaderScreen.SetActive (true);
		}

		void FeedbackSent(int result) {
			if (result == 1) {
				Destroy (gameObject);
				WelcomeScript.instance.ShowScreen (false);
			} else { // what to do on failure
				Destroy (gameObject);
				WelcomeScript.instance.ShowScreen (false);
			}
		}

		#if UNITY_IOS && !UNITY_EDITOR
		public void KeyboardShow(string dummy)
		{
		Vector3 currentPos = BottomPanel.transform.GetComponent<RectTransform> ().localPosition;
		BottomPanel.transform.GetComponent<RectTransform> ().localPosition = new Vector3 (currentPos.x, -387f, currentPos.z);
		}

		public void KeyboardHide(string dummy)
		{
		Vector3 currentPos = BottomPanel.transform.GetComponent<RectTransform> ().localPosition;
		BottomPanel.transform.GetComponent<RectTransform> ().localPosition = new Vector3 (currentPos.x, -730f, currentPos.z);
		}

		public void GetTextFieldString(string text)
		{
		print ("got back from native with string "+text);
		formText = text;
		}
		#endif

		[DllImport ("__Internal")]
		private static extern void _AddFeedbackTextView (float x, float y, float width, float height);
		[DllImport ("__Internal")]
		private static extern void _RemoveFeedbackTextView();
	}
}
