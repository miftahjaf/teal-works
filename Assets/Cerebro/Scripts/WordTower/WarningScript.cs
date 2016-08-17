using UnityEngine;
using System.Collections;
using System;

namespace Cerebro.WordTower {
	public class WarningScript : MonoBehaviour {

		public event EventHandler ContinuePressedEvent;

		void Start() {
			
		}

		public void CancelPressed() {
			Destroy (gameObject);
		}

		public void ContinuePressed() {
			if (ContinuePressedEvent != null) {
				ContinuePressedEvent.Invoke (this, new EventArgs ());
			}
			Destroy (gameObject);
		}
	}
}
