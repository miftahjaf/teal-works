using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public class CerebroTouchScript : MonoBehaviour
	{
		private int count = 0;

		void ShowPasswordDialog ()
		{
			GameObject.Find ("LaunchList").GetComponent<CerebroScript> ().showPasswordDialog ();
		}

		void Update ()
		{
			if (Input.GetMouseButton (0) && Input.mousePosition.y > (Screen.height - 100)) {
				count++;
				if (count > 200) {
					if (Input.mousePosition.x < 200) {
						ConsoleScreenScript.instance.ShowScreen ();
					} else {
						ShowPasswordDialog ();
					}
				}
			} else {
				count = 0;
			}
		}
	}
}
