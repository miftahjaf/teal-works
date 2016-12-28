using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Cerebro {
	public class StudentPlaylistContainerScript : CerebroTestScript {

		public ToggleGroup toggleGroup;

		YoutubePlayList youtubePlayList;
		StudentPlaylist playlist;
		MissionItemData mItemData;
		// Use this for initialization
		void Start () {
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Watch);

			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			playlist = LaunchList.instance.LoadStudentPlayList (gameObject);
			if (playlist != null && mItemData != null) {
				playlist.OpenMissionVideo (mItemData);
			}
			InstantiateYoutubeHelper ();
		}

		public void BackPressed() {
			//StudentScript ss = GameObject.FindGameObjectWithTag ("StudentView").GetComponent<StudentScript> ();
			//ss.Initialise ();
			if (playlist != null) {
				playlist.DestroyingScreen ();
			}
		
			WelcomeScript.instance.ShowScreen(false);
			Destroy (gameObject);
		}

		public override void ForceOpenScreen (string[] screen, int index, MissionItemData missionData)
		{
			mItemData = missionData;
			if (playlist != null) {
				playlist.OpenMissionVideo (missionData);
			}
		}

		public override void ForceOpenScreen (string[] screen, int index, Mission mission)
		{
		}

		public override string[] GetOptions ()
		{
			return options;
		}

		public void InstantiateYoutubeHelper()
		{
			youtubePlayList = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.YoutubePlaylist,gameObject.transform).GetComponent<YoutubePlayList> ();
			OnToggleChanged ();
		}

		public void OnToggleChanged() //Youtube or suggestion
		{
			IEnumerable<Toggle>	activeToggles = toggleGroup.ActiveToggles ();

			foreach (Toggle toggle in activeToggles) 
			{
				if (toggle.isOn) {
					bool isYoutube = (toggle.name == "Youtube");
					playlist.gameObject.SetActive (!isYoutube);
					youtubePlayList.gameObject.SetActive (isYoutube);
					CerebroAnalytics.instance.ScreenOpen (isYoutube ? CerebroScreens.Youtube : CerebroScreens.Watch);
				}
			}
			//toggleGroup.gameObject.SetActive (false);
		}
	}
}
