using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Cerebro.WordTower {
	public class WildcardLettersBoughtEventArgs : EventArgs
	{
		public List<int> Letters { get; set; }
		public int wildcardLettersLeft;
		public WildcardLettersBoughtEventArgs(List<int> letters,int wildcardlettersleft)
		{
			Letters = letters;
			wildcardLettersLeft = wildcardlettersleft;
		}
	}

	public class WildcardSelectScript : MonoBehaviour {

		public Text lettersText;
		public Text wildCardText;

		public GameObject BuyBlock;

		private int lettersAvailable;
		private int wildcardsAvailable;

		public List<BuyBlockScript> options;

		public List<int> selectedLetters;

		public event EventHandler WildcardLettersBought;

		public void Initialize(int availability, int _wildcardsAvailable) {
			lettersAvailable = availability;
			wildcardsAvailable = _wildcardsAvailable;
			SetCoinsText ();
		}

		// Use this for initialization
		void Start () {
			SetCoinsText ();

			selectedLetters = new List<int> ();

			Vector2 parentSize = wildCardText.transform.parent.GetComponent<RectTransform> ().sizeDelta;
			Vector2 optionSize = BuyBlock.GetComponent<RectTransform> ().sizeDelta;
			float offsetX = - optionSize.x * 11 /2;

			float xMultiplier = offsetX;
			float yMultiplier = optionSize.y * 3;
			options = new List<BuyBlockScript> ();
			for (var i = 0; i < 26; i++) {
				if (i % 8 == 0) {
					xMultiplier = offsetX;
					yMultiplier -= optionSize.y * 3 / 2;
				}

				int toAdd = i;

				GameObject option = Instantiate (BuyBlock);
				option.transform.SetParent (wildCardText.transform.parent,false);
				option.transform.localPosition = new Vector2 (xMultiplier, yMultiplier);
				option.transform.localScale = new Vector2 (1, 1);
				BuyBlockScript script = option.GetComponent<BuyBlockScript> ();
				script.Initialize (toAdd, 100);
				script.OptionClicked += OptionClicked;

				options.Add (option.GetComponent<BuyBlockScript> ());

				xMultiplier += (optionSize.x * 3/2);
			}
		}

		private void SetCoinsText() {
			lettersText.text = "Letters Available: " + lettersAvailable.ToString ();
			wildCardText.text = "Wildcard Letters Available: " + wildcardsAvailable.ToString ();
		}

		private void OptionClicked(object sender, EventArgs e) {
			BuyBlockScript script = (sender as BuyBlockScript);
			if (!script.selected) {
				if (wildcardsAvailable != 0 && lettersAvailable != 0) {
					script.gameObject.GetComponent<Image> ().color = new Color (0, 1, 0);
					script.selected = true;
					lettersAvailable--;
					wildcardsAvailable--;
					selectedLetters.Add (script.characterIndex);
				} else {
					if (wildcardsAvailable == 0) {
						CerebroHelper.DebugLog ("No more wildcards left");
						StartCoroutine (AnimateText (wildCardText));
					} else if (lettersAvailable == 0) {
						CerebroHelper.DebugLog ("No more letters left");
						StartCoroutine (AnimateText (lettersText));
					}
				}
			} else {
				script.gameObject.GetComponent<Image> ().color = new Color (1, 1, 1);
				script.selected = false;
				lettersAvailable++;
				wildcardsAvailable++;
				selectedLetters.Remove (script.characterIndex);
			}
			SetCoinsText ();
		}

		IEnumerator AnimateText(Text textObject) {
			textObject.color = Color.red;
			yield return new WaitForSeconds (0.6f);
			textObject.color = Color.black;
		}
		public void DonePressed() {
			if (WildcardLettersBought != null) {
				WildcardLettersBought.Invoke(this,new WildcardLettersBoughtEventArgs(selectedLetters,wildcardsAvailable));
			}
			Destroy (gameObject);
		}
	}
}