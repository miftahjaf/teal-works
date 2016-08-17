using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Cerebro {
	public class AlertScreenScript : MonoBehaviour {

		public GameObject buttonObject;
		public Text message;

		private static AlertScreenScript m_Instance;
		public static AlertScreenScript instance
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
			DontDestroyOnLoad (transform.gameObject);
		}

		void Start () {
			gameObject.SetActive (false);
		}

		public void Initialise(string txt) {
			gameObject.transform.SetAsLastSibling ();
			gameObject.SetActive (true);
			message.text = txt;
		}
		
		public void buttonPressed() {
			gameObject.transform.SetAsFirstSibling ();
			gameObject.SetActive (false);
		}
	}
}
