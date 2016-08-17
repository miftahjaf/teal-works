using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Cerebro {
	public class ConsoleScreenScript : MonoBehaviour {

		public Text log;

		[SerializeField]
		Canvas consoleCanvas;

		private static ConsoleScreenScript m_Instance;
		public static ConsoleScreenScript instance
		{
			get
			{
				return m_Instance;
			}
		}

		void Awake()
		{
			if (m_Instance != null && m_Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			m_Instance = this;
			DontDestroyOnLoad (transform.root.gameObject);
		}

		void Start () {
			gameObject.SetActive (false);
		}

		public void ShowScreen() {
			consoleCanvas.sortingOrder = 1;
			transform.SetAsLastSibling ();
			gameObject.SetActive (true);
		}

		public void CloseButtonPressed() {
			consoleCanvas.sortingOrder = 0;
			transform.SetAsFirstSibling ();
			gameObject.SetActive (false);
		}
	}
}
