using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Cerebro {

	public class LinePointScript : MonoBehaviour
	{
		public Image arrow;
		public Text pointName;
		public UICircle dot;

		public void SetPoint( LinePoint linePoint)
		{
			//Calculate arrow positin
			Vector2 position =new Vector2( linePoint.origin.x+linePoint.radius * Mathf.Cos(Mathf.Deg2Rad * linePoint.angle), linePoint.origin.y+linePoint.radius*Mathf.Sin(Mathf.Deg2Rad*linePoint.angle));

			//Disable or enbale arrow
			this.arrow.enabled = linePoint.shouldShowArrow;

			this.dot.enabled = !string.IsNullOrEmpty (linePoint.name);

			//Set arrow position
			this.arrow.GetComponent<RectTransform> ().anchoredPosition = position;

			//Set arrow angle
			this.arrow.GetComponent<RectTransform> ().localEulerAngles = new Vector3 (0f, 0f, 180 + linePoint.angle);

			//Calculate dot postion
			if ((linePoint.radius > 0 || linePoint.radius < 0 ) && linePoint.shouldShowArrow) 
			{
				position =new Vector2( linePoint.origin.x+linePoint.radius *0.75f * Mathf.Cos(Mathf.Deg2Rad * linePoint.angle), linePoint.origin.y+linePoint.radius*0.75f*Mathf.Sin(Mathf.Deg2Rad*linePoint.angle));
			}

			//Set dot position
			this.dot.GetComponent<RectTransform> ().anchoredPosition = position;


		if (linePoint.textOffset == Vector2.zero) {
			//Get point position 15 angle up down 
			float newAngle = linePoint.textDirection == 0 ? (linePoint.angle < 180 ? linePoint.angle - 15 : linePoint.angle + 15) : linePoint.angle + (15 * linePoint.textDirection);
			if (linePoint.radius > 0 || linePoint.radius < 0) {
				position = new Vector2 (linePoint.origin.x + linePoint.radius * 0.75f * Mathf.Cos (Mathf.Deg2Rad * newAngle), linePoint.origin.y + linePoint.radius * 0.75f * Mathf.Sin (Mathf.Deg2Rad * newAngle));
			} else {
				position = position + new Vector2 (linePoint.textDirection == 0 ? 0f : 15f * linePoint.textDirection, linePoint.textDirection == 0 ? -20f : 0);
			}
				
		} else {
			position = position + linePoint.textOffset;
		}
			this.pointName.GetComponent<RectTransform> ().anchoredPosition =  position ;

			//Set point label
			this.pointName.text = linePoint.name;



		}
	}
}

