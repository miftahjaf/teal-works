//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MaterialUI;

namespace Cerebro
{
	public class PasswordDialogScript : MonoBehaviour
	{
		public InputField input;
		private string password = "donotenter";
		private CerebroScript ss;

		public void Initialize(CerebroScript _ss) {
			ss = _ss;
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			GetComponent<RectTransform> ().position = new Vector3 (0f, 0f);
		}
		public void checkPassword() {
			if (input.text == password) {
				ss.correctPassword ();
			} else {
				input.text = "";
			}
		}

		public void HidePasswordDialog() {
			ss.HidePasswordDialog ();
		}
	}
}