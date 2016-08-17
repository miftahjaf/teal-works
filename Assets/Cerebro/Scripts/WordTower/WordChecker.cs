using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace Cerebro.WordTower
{
	public class WordChecker : MonoBehaviour
	{

		Dictionary<string, string> myDictionary;
		string wordToCheck;

		private static WordChecker m_Instance;

		public static WordChecker instance {
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

			var MytextAsset = Resources.Load ("words", typeof(TextAsset))  as TextAsset;
			myDictionary = MytextAsset.text.Split ("\n" [0]).ToDictionary (item => item, item => item);
		}

		public bool CheckWord (string word)
		{
			string lowercaseword = word.ToLower ();
			return myDictionary.ContainsKey (lowercaseword);      
		}
	}
}
