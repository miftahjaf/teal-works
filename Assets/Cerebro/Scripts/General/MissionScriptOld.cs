using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using System;
namespace Cerebro
{
	public class MissionScriptOld : MonoBehaviour
	{
		[SerializeField]
		private GameObject Title;
		[SerializeField]
		private GameObject MissionTextPrefab;
		[SerializeField]
		private GameObject FG;
		[SerializeField]
		private GameObject ProgressCircle;

		private List<GameObject> MissionTexts;


		private static MissionScriptOld m_Instance;
		public static MissionScriptOld instance {
			get {
				return m_Instance;
			}
		}

		void Awake ()
		{
			if (m_Instance != null && m_Instance != this) {
				Destroy (gameObject);
				return;
			}

			m_Instance = this;
		}

		public void ShowScreen() {
			ProgressCircle.SetActive (false);

			if (MissionTexts == null) {
				MissionTexts = new List<GameObject> ();
			} else {
				for (var k = 0; k < MissionTexts.Count; k++) {
					Destroy (MissionTexts [k]);
				}
				MissionTexts.Clear ();
			}

			Title.GetComponent<Text> ().text = LaunchList.instance.mMission.MissionName;
			float startY = 184f;
			int i = 0;
			foreach(var item in LaunchList.instance.mMission.Questions) {
				GameObject missionText = Instantiate (MissionTextPrefab);
				missionText.transform.SetParent (transform, false);
				missionText.transform.localPosition = new Vector2 (0f, startY - (100*i));
				missionText.gameObject.GetChildByName<Text> ("Text").text = item.Value.QuestionTitle; 
				Button btn = missionText.GetComponent<Button> ();
				btn.name = item.Key;
				btn.onClick.AddListener(() => MissionClicked(btn));
				i++;
				float Completionvalue = 1;
				foreach (var condition in item.Value.CompletionCondition) {
					if (condition.Value != "-1") {
						Completionvalue = float.Parse(condition.Value);
						break;
					}
				}

				float CompletionPercentage = Mathf.Floor (float.Parse(item.Value.ConditionCurrentValue) * 100f / Completionvalue);

				float AccuracyPercentage = 0;
				if (float.Parse (item.Value.TotalAttempts) != 0) {
					AccuracyPercentage = Mathf.Floor (float.Parse(item.Value.CorrectAttempts) * 100f / float.Parse(item.Value.TotalAttempts));
				}

				if (item.Value.CompleteBool == "true") {
					missionText.transform.Find ("Incomplete").gameObject.SetActive (false);
					missionText.transform.Find ("Complete").gameObject.SetActive (true);
				} else {
					missionText.transform.Find ("Incomplete").gameObject.SetActive (true);
					missionText.transform.Find ("Complete").gameObject.SetActive (false);
					missionText.transform.Find ("Incomplete").Find ("Text").GetComponent<Text> ().text = CompletionPercentage.ToString () + "%";
					if (CompletionPercentage == 0) {
						missionText.transform.Find ("Incomplete").Find ("Text").gameObject.SetActive (false);
					} else {
						missionText.transform.Find ("Incomplete").Find ("Text").gameObject.SetActive (true);
					}

					Image circle = missionText.transform.Find ("Incomplete").Find ("Completion").GetComponent<Image> ();
					circle.fillAmount = 0f;
				}

				MissionTexts.Add (missionText);

			}
			gameObject.SetActive (true);
			transform.SetAsLastSibling ();
			FG.transform.localPosition = new Vector2 (FG.transform.localPosition.x, -768);
			StartCoroutine (AnimateShow ());
		}

		void MissionClicked(Button b) {
			WelcomeScript.instance.RemoveScreens ();

			HideScreen (false);
			MissionItemData mid = LaunchList.instance.mMission.Questions [b.name];
			string practiceItemName = "";
			foreach (var item in LaunchList.instance.mPracticeItems) {
				if (item.Value.PracticeID.Contains(mid.PracticeItemID)) {
					practiceItemName = item.Value.PracticeItemName;
					break;
				}
			}
			if (mid.Type == "Practice") {
				if (practiceItemName != "") {
					WelcomeScript.instance.OpenScreen ("Practice," + practiceItemName, mid);
				} else {
					WelcomeScript.instance.OpenScreen ("Practice");
				}
			} else if (mid.Type == "Video") {
				WelcomeScript.instance.OpenScreen ("Watch,Video", mid);
			} else if (mid.Type == "Googly") {
				WelcomeScript.instance.OpenGoogly (mid);
			} else {
				CerebroHelper.DebugLog ("Unsupported Type");
			}
		}

		void Start() {
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);

			CerebroHelper.DebugLog ("START");

			FG.transform.localPosition = new Vector2 (FG.transform.localPosition.x, -768);
			ProgressCircle.SetActive (true);
			LaunchList.instance.MissionLoaded -= MisionLoadedHandler;
			LaunchList.instance.MissionLoaded += MisionLoadedHandler;
			string missionId = "";
			if (PlayerPrefs.HasKey (PlayerPrefKeys.MissionID)) {
				missionId = PlayerPrefs.GetString (PlayerPrefKeys.MissionID);
			}
			LaunchList.instance.GetMission (missionId);
		}

		void MisionLoadedHandler (object sender, EventArgs e)
		{
			if (this != null && this.gameObject.activeSelf) {
				ShowScreen ();
			}
		}

		public void HideScreen(bool withAnimation = true) {
			LaunchList.instance.MissionLoaded -= MisionLoadedHandler;
			if (withAnimation) {
				StartCoroutine (AnimateHide ());
			} else {
				if (MissionTexts != null) {
					for (var i = 0; i < MissionTexts.Count; i++) {
						Destroy (MissionTexts [i]);
					}
				}
				gameObject.SetActive (false);
			}
		}

		IEnumerator AnimateHide() {
			Go.to (gameObject.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (0f, -768f), false));
			yield return new WaitForSeconds (0.2f);
			for (var i = 0; i < MissionTexts.Count; i++) {
				Destroy (MissionTexts [i]);
			}
			gameObject.SetActive (false);
		}

		IEnumerator AnimateShow() {
			Title.transform.localPosition = new Vector2 (Title.transform.localPosition.x, 284f);

			for (var i = 0; i < MissionTexts.Count; i++) {
				MissionTexts [i].transform.localPosition = new Vector2 (-1024f, MissionTexts [i].transform.localPosition.y);
			}
			Go.to (FG.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (0f, 0f), false));
			yield return new WaitForSeconds (0.2f);

			for (var i = 0; i < MissionTexts.Count; i++) {
				Go.to (MissionTexts [i].transform, 0.3f, new GoTweenConfig ().localPosition (new Vector2 (1024f, 0f), true).setEaseType(GoEaseType.BackOut));
				yield return new WaitForSeconds (0.1f);
			}
			for (var i = 0; i < MissionTexts.Count; i++) {
				if (MissionTexts [i].transform.Find ("Incomplete") != null) {
					Image circle = MissionTexts [i].transform.Find ("Incomplete").Find ("Completion").GetComponent<Image> ();
					string perc = MissionTexts [i].transform.Find ("Incomplete").Find ("Text").GetComponent<Text> ().text;
					perc = perc.Substring (0, perc.Length - 1);
					float val = float.Parse (perc) / (float)100;
					circle.fillAmount = 0f;
					Go.to (circle, 1f, new GoTweenConfig ().floatProp ("fillAmount", val));
				}
			}
		}
	}
}
