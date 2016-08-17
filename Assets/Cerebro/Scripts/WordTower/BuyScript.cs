using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Cerebro.WordTower {
	public class LettersBoughtEventArgs : EventArgs
	{
		public List<int> Letters { get; set; }
		public int Coins { get; set; }
		public int wildcardLettersLeft;
		public LettersBoughtEventArgs(List<int> letters, int coins, int wildcardlettersleft)
		{
			Letters = letters;
			Coins = coins;
			wildcardLettersLeft = wildcardlettersleft;
		}
	}

	public class BuyScript : MonoBehaviour {

		public Text coinsText;
		public Text lettersText;
		public Text wildCardText;

		public GameObject BuyBlock;
		public GameObject wildCardSelector;

		public GameObject wildcardBtn;

		private int coins;
		private int lettersAvailable;
		private int wildcardsAvailable;
		public List<BuyBlockScript> options;

		public List<int> selectedLetters;

		public event EventHandler LettersBought;

		public void Initialize(int currentCoins, int availability, int _wildcardsAvailable) {
			coins = currentCoins;
			lettersAvailable = availability;
			wildcardsAvailable = _wildcardsAvailable;
			SetCoinsText ();
		}
		// Use this for initialization
		void Start () {
			SetCoinsText ();

			selectedLetters = new List<int> ();

			Vector2 parentSize = coinsText.transform.parent.GetComponent<RectTransform> ().sizeDelta;
			Vector2 optionSize = BuyBlock.GetComponent<RectTransform> ().sizeDelta;
			float offsetX = - optionSize.x * 7 /2;

			float xMultiplier = offsetX;
			float yMultiplier = 0;
			options = new List<BuyBlockScript> ();
			for (var i = 0; i < 10; i++) {
				int toAdd = UnityEngine.Random.Range (0, Constants.characters.Count);
				if (i < 2) {
					toAdd = Constants.vowels [UnityEngine.Random.Range (0, Constants.vowels.Count)];
				}

				GameObject option = Instantiate (BuyBlock);
				option.transform.SetParent (coinsText.transform.parent,false);
				option.transform.localPosition = new Vector2 (xMultiplier, yMultiplier);
				option.transform.localScale = new Vector2 (1, 1);
				BuyBlockScript script = option.GetComponent<BuyBlockScript> ();
				script.Initialize (toAdd, 100);
				script.OptionClicked += OptionClicked;

				options.Add (option.GetComponent<BuyBlockScript> ());

				xMultiplier += (optionSize.x * 3/2);
				if (i == 4) {
					xMultiplier = offsetX;
					yMultiplier -= optionSize.y * 3 / 2;
				}
			}
		}

		private void SetCoinsText() {
			coinsText.text = "Coins Available: " + coins.ToString ();
			lettersText.text = "Letters Available: " + lettersAvailable.ToString ();
			if (wildcardsAvailable < 1) {
				wildcardBtn.SetActive (false);
			} else {
				wildcardBtn.SetActive (true);
			}
			wildCardText.text = "Wildcard Letters Available: " + wildcardsAvailable.ToString ();
		}

		private void OptionClicked(object sender, EventArgs e) {
			BuyBlockScript script = (sender as BuyBlockScript);
			if (!script.selected) {
				if (script.price <= coins && lettersAvailable != 0) {
					script.gameObject.GetComponent<Image> ().color = new Color (0, 1, 0);
					script.selected = true;
					coins -= script.price;
					lettersAvailable--;
					selectedLetters.Add (script.characterIndex);
				} else {
					if (script.price > coins) {
						CerebroHelper.DebugLog ("INSUFFICIENT COINS");
						StartCoroutine (AnimateText (coinsText));
					} else if (lettersAvailable == 0) {
						CerebroHelper.DebugLog ("No more letters left");
						StartCoroutine (AnimateText (lettersText));
					}
				}
			} else {
				script.gameObject.GetComponent<Image> ().color = new Color (1, 1, 1);
				script.selected = false;
				coins += script.price;
				lettersAvailable++;
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
			if (selectedLetters.Count == 0) {
				coins -= 100;
				if (coins < 0) {
					coins = 0;
				}
			}
			if (LettersBought != null) {
				LettersBought.Invoke(this,new LettersBoughtEventArgs(selectedLetters,coins,wildcardsAvailable));
			}
			//gameObject.SetActive (false);
		}

		public void WildcardButtonPressed() {
			GameObject wildcardselector = Instantiate (wildCardSelector);
			wildcardselector.transform.SetParent (transform, false);
			wildcardselector.transform.localScale = new Vector2 (1, 1);
			wildcardselector.transform.localPosition = new Vector2 (0,0);
			wildcardselector.GetComponent<WildcardSelectScript> ().Initialize (lettersAvailable, wildcardsAvailable);
			wildcardselector.GetComponent<WildcardSelectScript> ().WildcardLettersBought += WildcardLettersBought;
		}

		private void WildcardLettersBought (object sender, EventArgs e)
		{
			(sender as WildcardSelectScript).WildcardLettersBought -= WildcardLettersBought;
			Destroy ((sender as WildcardSelectScript).gameObject);
			List<int> wildcardlettersBought = (e as WildcardLettersBoughtEventArgs).Letters;
			wildcardsAvailable = (e as WildcardLettersBoughtEventArgs).wildcardLettersLeft;
			for (var i = 0; i < wildcardlettersBought.Count; i++) {
				selectedLetters.Add (wildcardlettersBought [i]);
			}
			if (wildcardlettersBought.Count != 0) {
				if (LettersBought != null) {
					LettersBought.Invoke(this,new LettersBoughtEventArgs(selectedLetters,coins,wildcardsAvailable));
				}
			}
		}
		public void BGPressed() {
			//gameObject.SetActive (false);
		}
	}
}