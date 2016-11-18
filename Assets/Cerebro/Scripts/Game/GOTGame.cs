using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public class GOTGame : MonoBehaviour
	{

		GOTSplashScreen parent;
		public string CurrentGameID;

		// Use this for initialization
		void Start ()
		{
			transform.position = new Vector3 (-1.245304f, -0.4655751f, -0.7086951f);
			Camera.main.transform.position = new Vector3 (0f, 0f, -10f);
		}

		public void Initialise(GOTSplashScreen _parent, string GameID) {
			parent = _parent;
			CurrentGameID = GameID;
		}

		void OnApplicationFocus( bool focusStatus )
		{
			if (!focusStatus) {
				
			} else {
				bool fromFocus = true;
				BackPressed (fromFocus);
			}
		}

		public void BackPressed (bool fromFocus = false)
		{
			CapturePopup popup = transform.FindChild ("CellGrid").GetComponent<CellGrid> ().CapturePopup.GetComponent<CapturePopup> ();
			if (popup.IsPopupEnabled) {
				transform.FindChild ("CellGrid").GetComponent<CellGrid> ().CapturePopup.GetComponent<CapturePopup> ().BackPressed ();
			} else {
//				LaunchList.instance.WorldChanged += null;
				Destroy (gameObject);
				parent.BackOnScreen (fromFocus);
			}
		}
	}
}
