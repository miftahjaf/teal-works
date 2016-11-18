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
		public int NoOfChildren = 4;
		public Vector2[] InitialPosition;
		public bool IsOffsetCounted = false;
		public bool IsColorStatic = false;

		private bool IsLerpStarted;
		private float LerpStartTime, LerpValue;
		private float LerpTotalTime = 0.2f, OffsetPosition = 2400f;

		private Vector2[] StartPosition, EndPosition;
		private CustomizeAvatar parentAvatar;
		private Color[] InitialTeamColor;

		private int CurrID = 1;

		void Awake () 
		{
			if (InitialTeamColor != null && InitialTeamColor.Length > 0) {
				return;
			}
			StartPosition = new Vector2[NoOfChildren];
			EndPosition = new Vector2[NoOfChildren];
			InitialPosition = new Vector2[NoOfChildren];
			ChildComponents = new GameObject[NoOfChildren];
			InitialTeamColor = new Color[NoOfChildren];
			parentAvatar = transform.parent.GetComponent<CustomizeAvatar> ();

			int offset = IsOffsetCounted ? 4 : 0;
			for (int i = 0; i < NoOfChildren; i++) 
			{
				ChildComponents [i] = transform.FindChild (prefix+(i+1+offset)).gameObject;
				InitialTeamColor [i] = ChildComponents [i].GetComponent<Image> ().color;
				Vector2 currPos = ChildComponents [i].GetComponent<RectTransform> ().anchoredPosition;
				InitialPosition [i] = currPos;
				Debug.Log ("Init "+transform.name+" "+i+" "+currPos);
			}
		}

		public void Initialize(int id, string groupID = "")
		{
			if (InitialTeamColor == null || InitialTeamColor.Length <= 0) {
				Awake ();
			}
			if (!IsColorStatic) {
				var teamColor = new Color (0, 0, 0);
				if (groupID == "") {
					groupID = LaunchList.instance.mAvatar.ColorId;
				}
				Debug.Log ("curr "+groupID);
				if (groupID == GroupMapping.Group1) {
					teamColor = GroupMapping.Color1;
				} else if (groupID == GroupMapping.Group2) {
					teamColor = GroupMapping.Color2;
				} else if (groupID == GroupMapping.Group3) {
					teamColor = GroupMapping.Color3;
				} else if (groupID == GroupMapping.Group4) {
					teamColor = GroupMapping.Color4;
				}

				for (int i = 0; i < NoOfChildren; i++) {
					ChildComponents [i].GetComponent<Image> ().color = InitialTeamColor[i] * teamColor;
				}
			}
				
			CurrID = id;
			for (int i = 0; i < NoOfChildren; i++) {
				Vector2 currPos = InitialPosition[i];
				currPos = new Vector2 (currPos.x + i * OffsetPosition, currPos.y);
				StartPosition[i] = new Vector2 (currPos.x - (id - 1) * OffsetPosition, currPos.y);
				ChildComponents [i].GetComponent<RectTransform> ().anchoredPosition = StartPosition [i];
				ChildComponents [i].SetActive (false);
			}
			Debug.Log ("my id "+CurrID+" name "+gameObject.name);
			ChildComponents [CurrID - 1].SetActive (true);
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
					for (int i = 0; i < NoOfChildren; i++) 
					{
						ChildComponents [i].SetActive (false);
					}
					ChildComponents [CurrID - 1].SetActive (true);
				}

				MakeTransition ();
			}
		}

		public void NextButtonPressed(int NextId)
		{
			CurrID = NextId;
			IsLerpStarted = true;
			LerpValue = 0;
			LerpStartTime = Time.time;
			for (int i = 0; i < NoOfChildren; i++) 
			{
				EndPosition[i] = new Vector2 (StartPosition[i].x - OffsetPosition, StartPosition[i].y);
				ChildComponents [i].SetActive (true);
			}
		}

		public void PreviousButtonPressed(int NextId)
		{
			CurrID = NextId;
			IsLerpStarted = true;
			LerpValue = 0;
			LerpStartTime = Time.time;
			for (int i = 0; i < NoOfChildren; i++) 
			{
				EndPosition[i] = new Vector2 (StartPosition[i].x + OffsetPosition, StartPosition[i].y);
				ChildComponents [i].SetActive (true);
			}
		}

		public void MakeTransition()
		{
			if (LerpValue == 1) 
			{
				for (int i = 0; i < NoOfChildren; i++) 
				{
					ChildComponents [i].GetComponent<RectTransform> ().anchoredPosition = EndPosition [i];
					StartPosition [i] = EndPosition [i];
				}
			} 
			else 
			{
				for (int i = 0; i < NoOfChildren; i++) 
				{
					ChildComponents [i].GetComponent<RectTransform> ().anchoredPosition = Vector2.Lerp (StartPosition [i], EndPosition [i], LerpValue);
				}
			}
		}

	}
}
