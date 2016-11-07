using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.EventSystems;
using System;


namespace Cerebro
{
	public class GraphPointScript : MonoBehaviour,IDragHandler,IEndDragHandler
	{


		public Image arrow;
		public Text pointName;
		public UICircle dot;
		public GraphLine lineObj;
		public Action<GraphPointScript,Vector2> onDragEvent;
		public Action<GraphPointScript> onDragEndEvent;
		public LinePoint linePoint;
		private bool isValueChanged;

		public void SetPoint( LinePoint linePoint)
		{
			this.linePoint = linePoint;

			//Calculate arrow positin
			Vector2 parentPosition =new Vector2( linePoint.origin.x+linePoint.radius * Mathf.Cos(Mathf.Deg2Rad * linePoint.angle), linePoint.origin.y+linePoint.radius*Mathf.Sin(Mathf.Deg2Rad*linePoint.angle));
		
			//Disable or enbale arrow
			this.arrow.enabled = linePoint.shouldShowArrow;

	
			this.GetComponent<RectTransform> ().anchoredPosition = parentPosition;

			this.arrow.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

			//Set arrow angle
			this.arrow.GetComponent<RectTransform> ().localEulerAngles = new Vector3 (0f, 0f, 180 + linePoint.angle);

			Vector2 position  = parentPosition;

			//Calculate dot postion
			if ((linePoint.radius > 0 || linePoint.radius < 0 ) && linePoint.shouldShowArrow) 
			{
				position =new Vector2( linePoint.origin.x+linePoint.radius *0.75f * Mathf.Cos(Mathf.Deg2Rad * linePoint.angle), linePoint.origin.y+linePoint.radius*0.75f*Mathf.Sin(Mathf.Deg2Rad*linePoint.angle));
			}

			//Set dot position
			this.dot.GetComponent<RectTransform> ().anchoredPosition = parentPosition - position;

			//Get point position 15 angle up down 
			float newAngle =linePoint.textDirection ==0? (linePoint.angle < 180 ? linePoint.angle -15 : linePoint.angle + 15) : linePoint.angle+(15*linePoint.textDirection);
			if (linePoint.radius > 0 || linePoint.radius<0) {
				position = new Vector2 (linePoint.origin.x + linePoint.radius * 0.75f * Mathf.Cos (Mathf.Deg2Rad * newAngle), linePoint.origin.y + linePoint.radius * 0.75f * Mathf.Sin (Mathf.Deg2Rad * newAngle));
			} else 
			{
				position = position - new Vector2 (linePoint.pointTextOffset.x,linePoint.pointTextOffset.y);
			}

			this.pointName.GetComponent<RectTransform> ().anchoredPosition = parentPosition - position ;

			//Set point label
			this.pointName.text = linePoint.name;

		}

		public void SetTextFontMultiplier(float multiplier)
		{
			pointName.fontSize = Mathf.RoundToInt( pointName.fontSize * multiplier);
		}

		public void SetLineObject(GraphLine _lineObj)
		{
			lineObj = _lineObj;
		}

		#region IDragHandler implementation

		public void OnDrag (PointerEventData eventData)
		{
			if (onDragEvent == null) 
			{
				return;
			}

			this.isValueChanged = true;
			this.onDragEvent.Invoke (this,eventData.position);
		}

		#endregion

		#region IEndDragHandler implementation

		public void OnEndDrag (PointerEventData eventData)
		{
			if (onDragEndEvent == null) 
			{
				return;
			}
			this.onDragEndEvent.Invoke (this);
		}

		#endregion

		public void SetPointColor(Color color)
		{
			this.dot.color = color;	
		}

		public void SetDotSize(float multiplier)
		{
			this.dot.transform.localScale = Vector3.one * multiplier;
		}

		public bool IsValueChanged()
		{
			return isValueChanged;
		}

		public void SetIsValueChanged(bool isValueChanged)
		{
			this.isValueChanged = isValueChanged;
		}
	}

}
