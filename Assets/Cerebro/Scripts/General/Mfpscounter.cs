using UnityEngine;
using System.Collections;
namespace Cerebro
{
	public class Mfpscounter : MonoBehaviour {

		float mMSPF;
		private GUIStyle counterStyle;

		void Awake()
		{
			counterStyle = new GUIStyle ();
			counterStyle.fontSize = 42;
			counterStyle.normal.textColor = Color.green;
		}

		void OnGUI() {
			if (CerebroHelper.isTestUser () && mMSPF >= 17.0) {
				GUI.Label (new Rect (100, 10, 100, 20), mMSPF.ToString (), counterStyle);
			}
		}

		void  Update()
		{
			if (CerebroHelper.isTestUser ()) {
				mMSPF = Time.deltaTime * 1000.0f;
			}
		}
	}
}
