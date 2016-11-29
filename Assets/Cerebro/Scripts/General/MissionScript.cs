using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using System;
namespace Cerebro
{
	public class MissionScript : MonoBehaviour
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

		public void ShowScreen(string animateMissionItem = null) 
		{
			ProgressCircle.SetActive (false);
			//if (LaunchList.instance.mMission == null || LaunchList.instance.mMission.Questions == null) //Old Mission
			//	return;
			if (LaunchList.instance.missionData.dataList == null || LaunchList.instance.missionData.Count() <= 0) {
				return;
			}

			if (MissionTexts == null) {
				MissionTexts = new List<GameObject> ();
			} else {
				for (var k = 0; k < MissionTexts.Count; k++) {
					Destroy (MissionTexts [k]);
				}
				MissionTexts.Clear ();
			}

			//Title.GetComponent<Text> ().text = LaunchList.instance.mMission.xMissionName; //Old Mission
			Title.GetComponent<Text> ().text ="";
			float startY = 110f;
			int i = 0;

			gameObject.SetActive (true);
			transform.SetAsLastSibling ();

			int cnt = 0;
			foreach (Mission mission in LaunchList.instance.missionData.dataList) {
				GameObject missionText = Instantiate (MissionTextPrefab);
				missionText.transform.SetParent (transform, false);
				missionText.transform.localPosition = new Vector2 (0f, startY - (66*i));
				missionText.gameObject.GetChildByName<Text> ("Text").text = mission.missionText; 
				Button btn = missionText.GetComponent<Button> ();
				btn.name = i.ToString();
				int index = i;
				if (!string.IsNullOrEmpty (mission.endTime)) {
					missionText.transform.Find ("Incomplete").gameObject.SetActive (false);
					missionText.transform.Find ("Complete").gameObject.SetActive (true);
					missionText.transform.Find("Complete").GetComponent<Image>().color = mission.IsSucceed()? MaterialColor.green800:MaterialColor.red800;
				} else {
					missionText.transform.Find ("Incomplete").gameObject.SetActive (true);
					missionText.transform.Find ("Complete").gameObject.SetActive (false);
					btn.onClick.AddListener(() => MissionClicked(index));
				}
				if (animateMissionItem == mission.missionText) {
					StartCoroutine (AnimateMissionItemCompletion (missionText.gameObject));
					Invoke ("UpdateMission", 2f);
				}
				if (CerebroHelper.isTestUser ()) {
					Button streakButton = missionText.transform.Find ("TestButton").Find("Streak").GetComponent<Button> ();
					streakButton.onClick.AddListener (() => StreakClicked (index));
					Button attemptButton = missionText.transform.Find ("TestButton").Find("Attempt").GetComponent<Button> ();
					attemptButton.onClick.AddListener (() => AttemptClicked (index));
					missionText.transform.Find ("TestButton").gameObject.SetActive (true);
				} else {
					missionText.transform.Find ("TestButton").gameObject.SetActive (false);
				}

				MissionTexts.Add (missionText);
				i++;
			}

			//if (LaunchList.instance.mMission == null) {  //Old Mission
			//	return;
			//}

		/*	i = 0;
			foreach(var item in LaunchList.instance.mMission.Questions) {
				GameObject missionText = Instantiate (MissionTextPrefab);
				missionText.transform.SetParent (transform, false);
				missionText.transform.localPosition = new Vector2 (0f, startY - (66*i));
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
				if (item.Value.Type == "Video") {
					LaunchList.instance.AddMissionVideoToPlaylist (item.Value.PracticeItemID);
				}
//				float CompletionPercentage = Mathf.Floor (float.Parse(item.Value.ConditionCurrentValue) * 100f / Completionvalue);

//				float AccuracyPercentage = 0;
//				if (float.Parse (item.Value.TotalAttempts) != 0) {
//					AccuracyPercentage = Mathf.Floor (float.Parse(item.Value.CorrectAttempts) * 100f / float.Parse(item.Value.TotalAttempts));
//				}

				if (item.Value.CompleteBool == "true") {
					missionText.transform.Find ("Incomplete").gameObject.SetActive (false);
					missionText.transform.Find ("Complete").gameObject.SetActive (true);
				} else {
					missionText.transform.Find ("Incomplete").gameObject.SetActive (true);
					missionText.transform.Find ("Complete").gameObject.SetActive (false);
//					missionText.transform.Find ("Incomplete").Find ("Text").GetComponent<Text> ().text = CompletionPercentage.ToString () + "%";
//					if (CompletionPercentage == 0) {
//						missionText.transform.Find ("Incomplete").Find ("Text").gameObject.SetActive (false);
//					} else {
//						missionText.transform.Find ("Incomplete").Find ("Text").gameObject.SetActive (true);
//					}

//					Image circle = missionText.transform.Find ("Incomplete").Find ("Completion").GetComponent<Image> ();
//					circle.fillAmount = 0f;
				}
				if (animateMissionItem == item.Value.QuestionID) {
					StartCoroutine (AnimateMissionItemCompletion (missionText.gameObject));
				}
				MissionTexts.Add (missionText);

			} */


//			FG.transform.localPosition = new Vector2 (FG.transform.localPosition.x, -768);
//			StartCoroutine (AnimateShow ());
		}

		IEnumerator AnimateMissionItemCompletion(GameObject missionItem) {
			yield return new WaitForSeconds (0.2f);
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations( 2, GoLoopType.PingPong );
			var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
			if (missionItem) {
				var tween = new GoTween (missionItem.transform, 0.3f, config);
				flow.insert (0f, tween);
				flow.play ();
			}
		}

		/*void MissionClicked(Button b) { //Old Mission

			WelcomeScript.instance.RemoveScreens ();

			MissionItemData mid = LaunchList.instance.mMission.Questions [b.name];
			CerebroHelper.DebugLog ("SEARCHING FOR " + mid.PracticeItemID);
			string practiceItemName = "";
			if (LaunchList.instance.mPracticeItems.ContainsKey (mid.PracticeItemID)) {
				practiceItemName = LaunchList.instance.mPracticeItems [mid.PracticeItemID].PracticeItemName;
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
		}*/

		void MissionClicked(int index)
		{
			WelcomeScript.instance.RemoveScreens ();
			Mission missionData = LaunchList.instance.missionData.ElementAt(index);
			if (missionData.completionCondition == "Attempt" || missionData.completionCondition == "Streak" || missionData.completionCondition == "Correct") {
				WelcomeScript.instance.OpenScreen ("Practice,Assessment", missionData);
			} else {
				CerebroHelper.DebugLog ("Unsupported Type");
			}
		}

		void StreakClicked(int index)
		{
			Mission mission = LaunchList.instance.missionData.ElementAt(index);
			mission.AutoUpdateMission (true);
			LaunchList.instance.missionData.SaveData ();
			UpdateMission ();
		}

		void AttemptClicked(int index)
		{
			Mission mission = LaunchList.instance.missionData.ElementAt(index);
			mission.AutoUpdateMission (false);
			LaunchList.instance.missionData.SaveData ();
			UpdateMission ();
		}

		void Start() {
//			GetComponent<RectTransform> ().sizeDelta = new Vector2 (568f, 668f);

			CerebroHelper.DebugLog ("START");

//			FG.transform.localPosition = new Vector2 (228f, -768f);
			UpdateMission();
		}

		public void UpdateMission() {
			ProgressCircle.SetActive (true);

			Title.GetComponent<Text> ().text = "Preparing Mission...";
			if (MissionTexts != null) {
				for (var k = 0; k < MissionTexts.Count; k++) {
					Destroy (MissionTexts [k]);
				}
				MissionTexts.Clear ();
			}

			LaunchList.instance.MissionLoaded -= MisionLoadedHandler;
			LaunchList.instance.MissionLoaded += MisionLoadedHandler;
			/*	string missionId = "";                                   //Old Mission
				if (PlayerPrefs.HasKey (PlayerPrefKeys.MissionID)) {              //Old Mission
					missionId = PlayerPrefs.GetString (PlayerPrefKeys.MissionID); //Old Mission
				}
				LaunchList.instance.GetMission (missionId);                      //Old Mission 
			*/
			LaunchList.instance.GetMission ();

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
//			Title.transform.localPosition = new Vector2 (Title.transform.localPosition.x, 284f);
//
			for (var i = 0; i < MissionTexts.Count; i++) {
				MissionTexts [i].transform.localPosition = new Vector2 (1024f, MissionTexts [i].transform.localPosition.y);
			}
//			Go.to (FG.transform, 0.2f, new GoTweenConfig ().localPosition (new Vector2 (228f, -50f), false));
//			yield return new WaitForSeconds (0.2f);
//
			for (var i = 0; i < MissionTexts.Count; i++) {
				Go.to (MissionTexts [i].transform, 0.3f, new GoTweenConfig ().localPosition (new Vector2 (-1024f, 0f), true).setEaseType(GoEaseType.BackOut));
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
