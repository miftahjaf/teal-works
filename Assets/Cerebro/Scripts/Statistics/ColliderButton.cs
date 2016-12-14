using UnityEngine;
using System.Collections;
using System;


namespace Cerebro
{
	public class ColliderButton : MonoBehaviour
	{
		public Action OnClicked;
		public void OnMouseDown()
		{
			if (OnClicked != null) {
				OnClicked.Invoke ();
			}
		}
	}
}
