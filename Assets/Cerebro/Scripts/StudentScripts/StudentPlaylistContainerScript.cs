using UnityEngine;
using System.Collections;

namespace Cerebro {
	public class StudentPlaylistContainerScript : CerebroTestScript {

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
	}
}
