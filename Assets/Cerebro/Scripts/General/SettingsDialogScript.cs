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
	public class SettingsDialogScript : MonoBehaviour
	{
		public InputField EmailID;

		private CerebroScript cerebroScript;

		public void Initialize(CerebroScript _ss) {
			GetComponent<RectTransform> ().sizeDelta = new Vector2 (1024f, 768f);
			GetComponent<RectTransform> ().position = new Vector3 (0f, 0f);

			cerebroScript = _ss;
			transform.SetAsLastSibling ();
		}
		public void submitPressed() {
			string str = EmailID.text;
			if (str.IndexOf ("=") == 0) {
				str = str.Replace ("=", "");
			} else {
				str = str + "@aischool.net";
			}
			str = str.ToLower ();
			cerebroScript.updateData (str);
		}
		public void HideSettingsDialog() {
			cerebroScript.HideSettingsDialog ();
		}
	}
}