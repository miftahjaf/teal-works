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
			float angle = linePoint.angle;
			float radius = linePoint.radius;

			if(!linePoint.origin.Equals(linePoint.nextPoint))
			{
				angle = MathFunctions.GetangleBetweenPoints (linePoint.origin, linePoint.nextPoint);
				radius = Vector2.Distance (linePoint.origin, linePoint.nextPoint);
			}

			//Calculate arrow positin
			Vector2 position = MathFunctions.PointAtDirection(linePoint.origin,angle, radius * 0.98f);

			//Disable or enbale arrow
			this.arrow.enabled = linePoint.shouldShowArrow;

			this.dot.enabled = !string.IsNullOrEmpty (linePoint.name) && linePoint.shouldShowDot;

			//Set arrow position
			this.arrow.GetComponent<RectTransform> ().anchoredPosition = position;


			//Set arrow angle
			this.arrow.GetComponent<RectTransform> ().localEulerAngles = new Vector3 (0f, 0f, 180 + angle);

			//Calculate dot postion
			if ((radius > 0 ||radius < 0 ) && linePoint.shouldShowArrow && linePoint.shouldShowDot) 
			{
				position = MathFunctions.PointAtDirection(linePoint.origin,angle, radius * 0.75f);
			}

			//Set dot position
			this.dot.GetComponent<RectTransform> ().anchoredPosition = position;


			if (linePoint.pointTextOffset == Vector2.zero) {
			//Get point position 15 angle up down 
				float newAngle = linePoint.textDirection == 0 ? (angle < 180 ? angle - 15 : angle + 15) : angle + (15 * linePoint.textDirection);
				if (radius > 0 || radius < 0) {
					position = MathFunctions.PointAtDirection(linePoint.origin, newAngle, radius * 0.75f);
			} else {
				position = position + new Vector2 (linePoint.textDirection == 0 ? 0f : 15f * linePoint.textDirection, linePoint.textDirection == 0 ? -20f : 0);
			}
				
			} else {
				position = position + linePoint.pointTextOffset;
			}
			this.pointName.GetComponent<RectTransform> ().anchoredPosition =  position ;

			//Set point label
			this.pointName.text = linePoint.name;



		}
	}
}

