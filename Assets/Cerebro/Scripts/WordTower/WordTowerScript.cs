using UnityEngine;
using System.Collections;

namespace Cerebro.WordTower
{
	public class WordTowerScript : MonoBehaviour
	{
		[SerializeField]
		private World WorldScript;

		private static WordTowerScript m_Instance;

		public static WordTowerScript instance {
			get {
				return m_Instance;
			}
		}

		void Awake ()
		{
			if (m_Instance != null && m_Instance != this) {
				Destroy (gameObject);
				return;
			}
			m_Instance = this;
		}

		void Start ()
		{
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (0f, 0f);
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.WordTower);
		}

		public void Activate() {
			gameObject.transform.SetAsLastSibling ();
			gameObject.SetActive (true);
			WorldScript.SetCoins ();
		}

		public void BackPressed() {
			gameObject.SetActive (false);
			WelcomeScript.instance.ShowScreen (false);
		}
	}
}
