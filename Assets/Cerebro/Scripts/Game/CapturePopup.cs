using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Vectrosity;
using System.Collections.Generic;

namespace Cerebro
{
	public class CapturePopup : MonoBehaviour 
	{

		public GameObject AvatartGameobject, DefaultAvatar;
		public GameObject VectorLineGm;
		public Texture2D EndCapTexture;
		public bool IsPopupEnabled;

		public delegate void CaptureClicked();
		public CaptureClicked OnCaptureClicked;

		public delegate void CancelClicked();
		public CancelClicked OnCancelClicked;

		public void InitializePopup(string title, int coins, string BabaId, string groupId, CaptureClicked CaptureFunction, Vector3 cellPosition, CancelClicked CancelFunction, bool IsEnoughCoins = true)
		{
			OnCaptureClicked = CaptureFunction;
			OnCancelClicked = CancelFunction;
			IsPopupEnabled = true;
			if (BabaId == "") {
				DefaultAvatar.SetActive (true);
				AvatartGameobject.SetActive (false);
			} else {
				DefaultAvatar.SetActive (false);
				AvatartGameobject.SetActive (true);
				InitializeAvatar (BabaId, groupId);
			}
			transform.FindChild("Parent").FindChild ("Title").GetComponent<Text> ().text = title;
			transform.FindChild("Parent").FindChild ("CoinsValue").GetComponent<Text> ().text = coins.ToString();
			if (!IsEnoughCoins) {
				transform.FindChild ("Parent").FindChild ("BuyButton").FindChild ("BG").GetComponent<Image> ().color = new Color (0.5f, 0.5f, 0.5f, 1f);
				transform.FindChild ("Parent").FindChild ("BuyButton").GetComponent<Button> ().enabled = false;
			} else {
				transform.FindChild ("Parent").FindChild ("BuyButton").FindChild ("BG").GetComponent<Image> ().color = new Color (1f, 1f, 1f, 1f);
				transform.FindChild ("Parent").FindChild ("BuyButton").GetComponent<Button> ().enabled = true;
			}
			StartCoroutine (makeAnimation(cellPosition));
		}

		IEnumerator makeAnimation(Vector3 cellPosition)
		{
			yield return new WaitForSeconds (0.4f);
			transform.FindChild("Parent").gameObject.SetActive (true);
			cellPosition = new Vector3 (cellPosition.x, cellPosition.y, cellPosition.z);
			Vector2 stPoint = Camera.main.WorldToScreenPoint (cellPosition);
			stPoint = new Vector2 (stPoint.x * (1024f/Screen.width), stPoint.y * (1024f/Screen.width));
			Vector2 endPoint = new Vector2 (512f, 384f);
			Debug.Log ("stpoint "+stPoint);
			Debug.Log ("endPoint "+endPoint);
//			VectorLineGm.GetComponent<VectorObject2D> ().vectorLine = VectorLine.SetLine (Color.white, stPoint, endPoint);

			float dx = endPoint.x - stPoint.x;
			float dy = endPoint.y - stPoint.y;
			List<Vector2> allPoints = new List<Vector2> ();
			List<float> allWidths = new List<float> ();
			float width = 0.5f;
			for (float i = stPoint.y; i < endPoint.y; i += 5) {
				float x = ((i - stPoint.y) * (dx / dy)) + stPoint.x;
				width += 0.3f;
				allWidths.Add (width);
				allPoints.Add(new Vector2(x, i));
			}
			allWidths.RemoveAt (allWidths.Count - 1);
			VectorLineGm.GetComponent<VectorObject2D> ().vectorLine.points2 = allPoints;
			VectorLineGm.GetComponent<VectorObject2D> ().vectorLine.SetWidths (allWidths);
//			VectorLine.SetEndCap ("EndCap", EndCap.Back, EndCapTexture);
//			VectorLineGm.GetComponent<VectorObject2D> ().vectorLine.endCap = "EndCap";
			VectorLineGm.GetComponent<VectorObject2D> ().vectorLine.Draw ();
		}

		public void CapturePressed() {
			if (OnCaptureClicked != null) {
				OnCaptureClicked ();
			}
			Go.to (Camera.main.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (0f, 0f, -10f), false).setEaseType (GoEaseType.BackIn));
			IsPopupEnabled = false;
			transform.FindChild("Parent").gameObject.SetActive (false);
		}

		public void BackPressed()
		{
			if (OnCancelClicked != null) {
				OnCancelClicked ();
			}
			Go.to (Camera.main.transform, 0.4f, new GoTweenConfig ().position (new Vector3 (0f, 0f, -10f), false).setEaseType (GoEaseType.BackIn));
			OnCaptureClicked = null;
			IsPopupEnabled = false;
			transform.FindChild("Parent").gameObject.SetActive (false);
		}

		public void InitializeAvatar(string BabaId, string groupID)
		{
			int CurrHairID = int.Parse (BabaId [0].ToString());
			int CurrHeadId = int.Parse (BabaId [1].ToString());
			int CurrBodyId = int.Parse (BabaId [2].ToString());
			bool IsBoy = false;
			if (CurrBodyId > 4) {
				IsBoy = false;
				CurrBodyId -= 4;
				CurrHairID -= 4;
				CurrHeadId -= 4;
			} else {
				IsBoy = true;
			}

			Debug.Log ("pop up "+BabaId+" "+IsBoy+" "+CurrHairID+" "+CurrHeadId+" "+CurrBodyId);
			DisableAllComponents ();
			if (IsBoy) {
				Debug.Log ("opening boy");
				AvatartGameobject.transform.FindChild ("boy_body").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId, groupID);
				AvatartGameobject.transform.FindChild ("boy_head").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("boy_head").GetComponent<AvatarComponentList>().Initialize(CurrHeadId, groupID);
				AvatartGameobject.transform.FindChild ("boy_hair").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().Initialize(CurrHairID, groupID);
			} else {
				Debug.Log ("opening girl");
				AvatartGameobject.transform.FindChild ("girl_body").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("girl_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId, groupID);
				AvatartGameobject.transform.FindChild ("girl_head").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("girl_head").GetComponent<AvatarComponentList>().Initialize(CurrHeadId, groupID);
				AvatartGameobject.transform.FindChild ("girl_hair_back").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("girl_hair_back").GetComponent<AvatarComponentList>().Initialize(CurrHairID, groupID);
				AvatartGameobject.transform.FindChild ("girl_hair_front").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("girl_hair_front").GetComponent<AvatarComponentList>().Initialize(CurrHairID, groupID);
			}
		}

		void DisableAllComponents()
		{
			AvatartGameobject.transform.FindChild ("boy_body").gameObject.SetActive (false);
			AvatartGameobject.transform.FindChild ("boy_head").gameObject.SetActive (false);
			AvatartGameobject.transform.FindChild ("boy_hair").gameObject.SetActive (false);
			AvatartGameobject.transform.FindChild ("girl_body").gameObject.SetActive (false);
			AvatartGameobject.transform.FindChild ("girl_head").gameObject.SetActive (false);
			AvatartGameobject.transform.FindChild ("girl_hair_back").gameObject.SetActive (false);
			AvatartGameobject.transform.FindChild ("girl_hair_front").gameObject.SetActive (false);
		}

	}
}