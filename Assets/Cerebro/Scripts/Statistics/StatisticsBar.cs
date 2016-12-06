using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


namespace Cerebro
{
	public class StatisticsBar : MonoBehaviour
	{
		private float height = 0;
		private float currentHeight = 0;
		private bool isHorizontal =true;
		private float width =0;
		public void SetHeight(float _height)
		{
			height = _height;
		}

		public void SetWidth(float _width)
		{
			width = _width;
		}

		public void SetCurrentHeight(float _currentHeight)
		{
			currentHeight = _currentHeight;

		}

		public void SetIsHorizontal(bool _isHorizontal)
		{
			isHorizontal = _isHorizontal;
		}

		public void SetBar()
		{
			if (this.isHorizontal) {
				this.gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (width, currentHeight);
			} else {
				this.gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (currentHeight, width);
			}
		}

		public void SetCurrentHeight(Vector2 position)
		{
			currentHeight = Vector2.Distance (this.gameObject.GetComponent<RectTransform> ().anchoredPosition, position);
			SetBar ();
		}

		public bool IsHorizontal()
		{
			return this.isHorizontal;
		}
	}
}
