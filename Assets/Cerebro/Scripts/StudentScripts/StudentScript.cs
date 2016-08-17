using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
using UnityEngine.UI;

namespace Cerebro {
	
	public class StudentScript : CerebroTestScript {

		public GameObject dashboardIcon;

		public GameObject subScreen;
		public GameObject subScreenParent;

		public GameObject[] Buttons;
		private List<GameObject> Lines;

		private bool isAnimating = false;
		float optionHeight;

		void Start () {
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			options = new string[]{ "Practice", "Watch", "Play", "Revisit", "QOTD" };
		}

		public void Initialise () {
			ShowDashboardIcon ();
			subScreenParent.SetActive (false);
			LaunchList.instance.setWifiIcon ();
			StartCoroutine (ShowButtons ());
		}

		public void HideHeading() {
			for (int i = Buttons.Length - 1; i >= 0; i--) {
				Buttons [i].SetActive (false);
			}
		}

		public void ShowHeading() {
			for (int i = Buttons.Length - 1; i >= 0; i--) {
				Buttons [i].SetActive (true);
			}
		}

		IEnumerator ShowButtons() {
			Lines = new List<GameObject> ();
			float height = 768f;
			optionHeight = Buttons[0].gameObject.GetComponent<RectTransform>().sizeDelta.y;
			float optionHeights = optionHeight * Buttons.Length;
			float padding = optionHeight/4f;
			float paddingTotal = padding * (Buttons.Length - 1);
			float occupiedSpace = height - (optionHeights + paddingTotal);

			for (int i = Buttons.Length - 1; i >= 0; i--) {
				Buttons [i].transform.localPosition = new Vector3 (0, 768 / 2 + optionHeight, 0);
				Buttons [i].transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			}

			float posY = (768f - occupiedSpace) / 2 - ((optionHeight + padding) * (Buttons.Length - 1));
			for (int i = Buttons.Length - 1; i >= 0 ; i--) {
				Go.to( Buttons[i].transform, 0.3f, new GoTweenConfig().localPosition( new Vector3( 0f, posY - optionHeight/2, 0 ), false).setEaseType( GoEaseType.BackOut ));
				if (i != 0) {
					GameObject line = PrefabManager.InstantiateGameObject (ResourcePrefabs.Separator, gameObject.transform);
					line.transform.localPosition = new Vector3 (0, 768 / 2, 0);
					Lines.Add (line);
					yield return new WaitForSeconds (0.1f);
					Go.to (line.transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (0f, posY + padding / 2, 0), false).setEaseType (GoEaseType.BackOut));
				}
				yield return new WaitForSeconds (0.1f);
				posY += (optionHeight + padding);
			}
		}

		public void showRevision() {
			StartCoroutine (AnimateHideAssessment ("Revisit"));
		}

		public void showQOTD() {
			StartCoroutine (AnimateHideAssessment ("QOTD"));
		}

		public void showAssessment() {
			StartCoroutine (AnimateHideAssessment ("Practice"));
		}

		public void showPlaylist() {
			StartCoroutine (AnimateHideAssessment ("Watch"));
		}

		IEnumerator AnimateHideAssessment(string type) {
			if (isAnimating) {
				yield break;
			}

			isAnimating = true;

			var animateIndex = -1;
//			if (type == "Assessment") { 
//				animateIndex = 1;
//			} else if (type == "Playlist") {
//				animateIndex = 0;
//			} else if (type == "QOTD") {
//				animateIndex = 3;
//			}

			if (animateIndex == -1) {
				var posY = -(768 / 2);
				for (int i = Buttons.Length - 1; i >= 0; i--) {
					Go.to (Buttons [i].transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (0f, posY-optionHeight/2, 0), false).setEaseType (GoEaseType.BackIn));
					yield return new WaitForSeconds (0.1f);
					if (i != 0) {
						Go.to (Lines [Buttons.Length - 1 - i].transform, 0.3f, new GoTweenConfig ().localPosition (new Vector3 (0f, posY - 10, 0), false).setEaseType (GoEaseType.BackIn));
						yield return new WaitForSeconds (0.1f);
					}
				}
				yield return new WaitForSeconds (0.3f);
			} 
			else {
				var posY = -(768 / 2);
				for (int i = Buttons.Length - 1; i >= 0; i--) {
					if (i != animateIndex) {
						Text text = Buttons [i].transform.Find ("Text").GetComponent<Text> ();
						Go.to (text, 0.1f, new GoTweenConfig ().colorProp ("color", new Color (1, 1, 1, 0)));
					} else {
						Go.to (Buttons [i].transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 0), false));
					}

					if (i != 0) {
						Go.to (Lines [Buttons.Length - 1 - i].gameObject.GetComponent<Image>(), 0.1f, new GoTweenConfig().colorProp( "color", new Color(1,1,1,0)));
					}
				}
				yield return new WaitForSeconds (0.2f);
				Go.to (Buttons [animateIndex].transform, 0.2f, new GoTweenConfig ().localPosition (new Vector3 (0f, 355, 0), false));
				Go.to (Buttons [animateIndex].transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (0.7f, 0.7f, 0), false));

				yield return new WaitForSeconds (0.3f);
				for (int i = Buttons.Length - 1; i >= 0; i--) {
					if (i == animateIndex) {
						continue;
					}
					Buttons [i].transform.localPosition = new Vector3 (0, 768 / 2 + optionHeight, 0);
					Text text = Buttons [i].transform.Find ("Text").GetComponent<Text> ();
					text.color =  new Color(1,1,1,1);
					Buttons [i].transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
				}
			}

			for (var i = 0; i < Lines.Count; i++) {
				Destroy (Lines [i].gameObject);
			}


			isAnimating = false;
			OpenScreen (type);
		}

		public void LaunchGame() {
			if (isAnimating) {
				return;
			}
			StartCoroutine (AnimateHideAssessment ("Play"));
		}

		public void DashboardPressed() {
			WelcomeScript.instance.ShowScreen ();
		}

		public override void ForceOpenScreen (string[] screen, int index, MissionItemData missionItemData) {
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			string nextScreen = null;
			if (index < screen.Length-1) {
				nextScreen = screen [index + 1];
			}
			OpenScreen (screen[index]);
			if (nextScreen != null) {
				subScreen.GetComponent<CerebroTestScript> ().ForceOpenScreen (screen, index + 1, missionItemData); 	//to open any assessment directly
			}
		}

		public override string[] GetOptions ()
		{
			return options;
		}

		private void OpenScreen (string type) {

			optionHeight = Buttons[0].gameObject.GetComponent<RectTransform>().sizeDelta.y;
			for (int i = Buttons.Length - 1; i >= 0; i--) {
				Buttons [i].transform.localPosition = new Vector3 (0, 768 / 2 + optionHeight, 0);
			}
			if (Lines != null) {
				for (var i = 0; i < Lines.Count; i++) {
					Destroy (Lines [i].gameObject);
				}
			}

			List<Transform> children = new List<Transform> ();
			for (var i = 0; i < subScreenParent.transform.childCount; i++) {
				children.Add(subScreenParent.transform.GetChild(i));
			}
			foreach (Transform child in children) {
				Destroy (child.gameObject);
			}

			if (type == "Practice") {
				subScreen = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.ChooseAssessments, subScreenParent.transform);
				subScreenParent.SetActive (true);
			} else if (type == "Watch") {
				subScreen = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.StudentPlaylistContainer, subScreenParent.transform);
				subScreenParent.SetActive (true);
			} else if (type == "Play") {
				LaunchList.instance.LoadGame ();
			} else if (type == "Revisit") {
				subScreen = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.Revisit, subScreenParent.transform);
				subScreenParent.SetActive (true);
			} else if (type == "QOTD") {
				subScreen = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.QOTDLandingPage, subScreenParent.transform);
				subScreenParent.SetActive (true);
			}
		}

		public void ShowDashboardIcon() {
			dashboardIcon.SetActive (true);
		}

		public void HideDashboardIcon() {
			dashboardIcon.SetActive (false);
		}
	}
}
