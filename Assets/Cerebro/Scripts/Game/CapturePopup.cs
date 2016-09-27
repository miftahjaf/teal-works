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

		public delegate void CaptureClicked();
		public CaptureClicked OnCaptureClicked;

		public void InitializePopup(string title, int coins, string BabaId, string groupId, CaptureClicked CaptureFunction, Vector3 cellPosition)
		{
			OnCaptureClicked = CaptureFunction;
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
			StartCoroutine (makeAnimation(cellPosition));
		}

		IEnumerator makeAnimation(Vector3 cellPosition)
		{
			yield return new WaitForSeconds (0.4f);
			transform.FindChild("Parent").gameObject.SetActive (true);
			Vector2 stPoint = Camera.main.WorldToScreenPoint (cellPosition);
			Vector3 pos = Camera.main.WorldToViewportPoint (transform.FindChild("Parent").FindChild ("Pointer").transform.position);
			Vector2 endPoint = new Vector2 (pos.x * Screen.width, pos.y * Screen.height);
			Debug.Log ("pos "+transform.position+" height "+GetComponent<RectTransform>().rect.height);
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
			transform.FindChild("Parent").gameObject.SetActive (false);
		}

		public void BackPressed()
		{
			OnCaptureClicked = null;
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

			DisableAllComponents ();
			if (IsBoy) {
				AvatartGameobject.transform.FindChild ("boy_body").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("boy_body").GetComponent<AvatarComponentList>().Initialize(CurrBodyId, groupID);
				AvatartGameobject.transform.FindChild ("boy_head").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("boy_head").GetComponent<AvatarComponentList>().Initialize(CurrHeadId, groupID);
				AvatartGameobject.transform.FindChild ("boy_hair").gameObject.SetActive (true);
				AvatartGameobject.transform.FindChild ("boy_hair").GetComponent<AvatarComponentList>().Initialize(CurrHairID, groupID);
			} else {
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