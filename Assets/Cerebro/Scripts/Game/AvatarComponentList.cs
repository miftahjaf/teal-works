using UnityEngine;
using System.Collections;
using MaterialUI;
using UnityEngine.UI;

namespace Cerebro
{
	public class AvatarComponentList : MonoBehaviour 
	{
		public string prefix;
		public GameObject[] ChildComponents;

		private bool IsLerpStarted;
		private float LerpStartTime, LerpValue;
		private float LerpTotalTime = 0.2f, OffsetPosition = 2400f;

		private Vector2[] StartPosition, EndPosition, InitialPosition;
		private CustomizeAvatar parentAvatar;
		private Color[] InitialTeamColor;

		void Awake () 
		{
			StartPosition = new Vector2[4];
			EndPosition = new Vector2[4];
			InitialPosition = new Vector2[4];
			ChildComponents = new GameObject[4];
			InitialTeamColor = new Color[4];
			parentAvatar = transform.parent.GetComponent<CustomizeAvatar> ();

			int offset = transform.name.Contains("girl") ? 4 : 0;
			for (int i = 0; i < 4; i++) 
			{
				ChildComponents [i] = transform.FindChild (prefix+(i+1+offset)).gameObject;
				InitialTeamColor [i] = ChildComponents [i].GetComponent<Image> ().color;
				Vector2 currPos = ChildComponents [i].GetComponent<RectTransform> ().anchoredPosition;
				InitialPosition [i] = currPos;
			}
		}

		public void Initialize(int id, string groupID = "")
		{
			if (!transform.name.Contains ("head")) {
				var teamColor = new Color (0, 0, 0);
				if (groupID == "") {
					groupID = PlayerPrefs.GetString (PlayerPrefKeys.GOTGameTeamID, "1");
				}
				Debug.Log ("curr "+groupID);
				if (groupID == GroupMapping.Group1) {
					teamColor = new Color (0.99f, 0.39f, 0.15f);
				} else if (groupID == GroupMapping.Group2) {
					teamColor = new Color (0.05f, 0.9f, 0.9f);
				} else if (groupID == GroupMapping.Group3) {
					teamColor = new Color (0.62f, 0.62f, 0.62f);
				} else if (groupID == GroupMapping.Group4) {
					teamColor = new Color (0.39f, 0.62f, 0.92f);
				}

				for (int i = 0; i < 4; i++) {
					ChildComponents [i].GetComponent<Image> ().color = InitialTeamColor[i] * teamColor;
				}
			}

			for (int i = 0; i < 4; i++) {
				Vector2 currPos = InitialPosition[i];
				currPos = new Vector2 (currPos.x + i * OffsetPosition, currPos.y);
				StartPosition[i] = new Vector2 (currPos.x - (id - 1) * OffsetPosition, currPos.y);
				ChildComponents [i].GetComponent<RectTransform> ().anchoredPosition = StartPosition [i];
			}
		}

		void Update()
		{
			if (IsLerpStarted) 
			{
				if (Time.time - LerpStartTime < LerpTotalTime) 
				{
					LerpValue = Mathf.Lerp (0f, 1f, (Time.time - LerpStartTime) / LerpTotalTime);
				}
				else 
				{
					IsLerpStarted = false;
					LerpValue = 1;
					parentAvatar.IsTransitionOn = false;
				}

				MakeTransition ();
			}
		}

		public void NextButtonPressed()
		{
			IsLerpStarted = true;
			LerpValue = 0;
			LerpStartTime = Time.time;
			for (int i = 0; i < 4; i++) 
			{
				EndPosition[i] = new Vector2 (StartPosition[i].x - OffsetPosition, StartPosition[i].y);
			}
		}

		public void PreviousButtonPressed()
		{
			IsLerpStarted = true;
			LerpValue = 0;
			LerpStartTime = Time.time;
			for (int i = 0; i < 4; i++) 
			{
				EndPosition[i] = new Vector2 (StartPosition[i].x + OffsetPosition, StartPosition[i].y);
			}
		}

		public void MakeTransition()
		{
			if (LerpValue == 1) 
			{
				for (int i = 0; i < 4; i++) 
				{
					ChildComponents [i].GetComponent<RectTransform> ().anchoredPosition = EndPosition [i];
					StartPosition [i] = EndPosition [i];
				}
			} 
			else 
			{
				for (int i = 0; i < 4; i++) 
				{
					ChildComponents [i].GetComponent<RectTransform> ().anchoredPosition = Vector2.Lerp (StartPosition [i], EndPosition [i], LerpValue);
				}
			}
		}

	}
}
