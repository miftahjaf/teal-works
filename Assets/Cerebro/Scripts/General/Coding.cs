using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public class Coding : MonoBehaviour
	{
		void Start ()
		{
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Coding);

			GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0f, 0f);
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
		}

		public void BackPressed ()
		{
			WelcomeScript.instance.ShowScreen (false);
			Destroy (gameObject);
		}
	}
}
