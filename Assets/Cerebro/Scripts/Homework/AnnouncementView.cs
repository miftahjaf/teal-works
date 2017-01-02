﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Cerebro
{
	public class AnnouncementView : MonoBehaviour 
	{

		public Text AnnouncementText;
		
		public void Initialize(string annText)
		{
			AnnouncementText.text = annText;
		}

		public void OnBackPressed()
		{
			Destroy (this.gameObject);
		}

	}
}