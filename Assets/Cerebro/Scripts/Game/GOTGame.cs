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
			LaunchList.instance.WorldChanged += null;
			Destroy(gameObject);
			parent.BackOnScreen (fromFocus);
		}
	}
}
