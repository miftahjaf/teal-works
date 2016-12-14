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
		private float startHeight = 0;
		private Color color;
		private GraphPointScript graphPointScript;
		private Vector2 graphPointStartPosition;

		public void SetHeight(float _height)
		{
			height = _height;
		}

		public void SetWidth(float _width)
		{
			width = _width;
		}

		public void SetStartHeight(float _startheight)
		{
			startHeight = _startheight;
			currentHeight = _startheight;
		}

		public void SetColor(Color _color)
		{
			color = _color;
			SetColor ();
		}

		public void SetCurrentHeight(float _currentHeight)
		{
			currentHeight = _currentHeight;

		}

		public void SetIsHorizontal(bool _isHorizontal)
		{
			isHorizontal = _isHorizontal;
		}

		public void SetGraphPoint(GraphPointScript _graphPointScript)
		{
			graphPointScript = _graphPointScript;
			graphPointStartPosition = graphPointScript.linePoint.origin;
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

		public bool IsCorrect()
		{
			return currentHeight == height;
		}

		public bool IsChanged()
		{
			return currentHeight != startHeight;
		}

		public void Reset()
		{
			currentHeight = startHeight;

			if (graphPointScript != null)
			{
				graphPointScript.gameObject.SetActive (true);
				graphPointScript.linePoint.origin = graphPointStartPosition;
				graphPointScript.Reset ();
			}
			SetBar ();
			SetColor ();
		}

		public void SetColor()
		{
			this.GetComponent<Image> ().color = color;
		}

		public void SetCorrectAnswer()
		{
			currentHeight = height;
			SetBar ();
			SetColor ();
			graphPointScript.gameObject.SetActive (false);
		}

		public void ChangeColor(Color color)
		{
			this.GetComponent<Image> ().color = color;
		}




	}
}
