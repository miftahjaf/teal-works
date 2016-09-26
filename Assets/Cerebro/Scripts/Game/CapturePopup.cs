using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Cerebro
{
	public class CapturePopup : MonoBehaviour 
	{

		public delegate void CaptureClicked();
		public CaptureClicked OnCaptureClicked;

		public void InitializePopup(string title, int coins, CaptureClicked CaptureFunction = null, int BabaHairId = -1, int BabaFaceId = -1, int BabaBodyId = -1)
		{
			OnCaptureClicked = CaptureFunction;
			gameObject.SetActive (true);
			transform.FindChild ("Title").GetComponent<Text> ().text = title;
			transform.FindChild ("CoinsValue").GetComponent<Text> ().text = coins.ToString();
			if (BabaHairId != -1 && BabaFaceId != -1 && BabaBodyId != -1) {
				transform.FindChild ("UnitUI").gameObject.SetActive (true);
				transform.FindChild ("UnitUI").GetComponent<CustomizeAvatar> ().InitializeAvatar ("" + BabaHairId + "" + BabaFaceId + "" + BabaBodyId);
			} else {
				transform.FindChild ("UnitUI").gameObject.SetActive (false);
			}
		}

		public void CapturePressed() {
			if (OnCaptureClicked != null) {
				OnCaptureClicked ();
			}
			gameObject.SetActive (false);
		}

	}
}