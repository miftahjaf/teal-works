using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace Cerebro.WordTower {
	public class BuyBlockScript : MonoBehaviour, IPointerClickHandler {

		public int characterIndex;
		public int price;
		public bool selected;
		public event EventHandler OptionClicked;

		public void Initialize(int charIndex, int letterprice) {
			characterIndex = charIndex;
			price = letterprice;

			selected = false;
			gameObject.transform.Find ("Text").GetComponent<Text> ().text = Constants.characters [characterIndex];
		}

		public void OnPointerClick (PointerEventData eventData) {
			if (OptionClicked != null)
				OptionClicked.Invoke(this, new EventArgs());
		}
	}
}