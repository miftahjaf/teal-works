using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using Cerebro;
namespace Cerebro {	


	public class DiagramHelper : MonoBehaviour 
	{
		private List<LinePoint> linePoints = new List<LinePoint> ();
		private List<AngleArc> angleArcs = new List<AngleArc> ();
		private VectorLine vectorLine;
		public GameObject pointPrefab;
		public GameObject arcPrefab;
		public GameObject lineTextPrefab;

		public void Awake()
		{
			//Set instance of line to set points
			this.vectorLine = this.GetComponent<VectorObject2D> ().vectorLine;
		}
		public void AddLinePoint(LinePoint linePoint)
		{
			this.linePoints.Add (linePoint);
		}

		public void AddAngleArc(AngleArc arc)
		{
			this.angleArcs.Add (arc);
		}


		public void Draw()
		{
			//Add pairs of point to generate disrete line
			List <Vector2> lineValues = new List <Vector2> ();
			foreach (LinePoint linePoint in linePoints) {
				//Calculate ending point using angle and given radius
				Vector2 newPoint = MathFunctions.PointAtDirection (linePoint.origin, linePoint.angle, linePoint.radius);

				if (linePoint.lineType == LineShapeType.Normal) {
					lineValues.Add (linePoint.origin);
					lineValues.Add (newPoint);

				} else {
					int counter = 0;
					Vector2 stPoint = linePoint.origin;
					Vector2 lastPoint = newPoint;

					if (newPoint.y < linePoint.origin.y) {
						stPoint = newPoint;
						lastPoint = linePoint.origin;
					}

					float dy = lastPoint.y - stPoint.y;

					if (Mathf.Abs (dy) > 0.0001f) {
						float dx = lastPoint.x - stPoint.x;
						for (float i = stPoint.y; i < lastPoint.y; i += 4f) {
							counter++;
							float x = ((i - stPoint.y) * (dx / dy)) + stPoint.x;
							lineValues.Add (new Vector2 (x, i));
						}
					} else { 
						if (newPoint.x < linePoint.origin.x) {
							stPoint = newPoint;
							lastPoint = linePoint.origin;
						}
						for (float x = stPoint.x; x < lastPoint.x; x += 4f) {
							counter++;
							lineValues.Add (new Vector2 (x, stPoint.y));
						}
					}
					if (counter % 2 != 0)
						lineValues.Add (lastPoint);
				}




				//Instantiate point prefab to show point, point name and arrow
				GameObject endPoint = GameObject.Instantiate (pointPrefab);
				endPoint.transform.SetParent (this.transform, false);

				//set arrow, point and label poistion
				endPoint.GetComponent<LinePointScript> ().SetPoint (linePoint);

				//add line text
				if (!string.IsNullOrEmpty (linePoint.lineText)) 
				{
					AddTextInLine (linePoint.lineText, linePoint.origin, newPoint,linePoint.lineTextDirection);
				}

			}
		

			//Assign point to vectrosity line alogrithm to draw line between points
			this.vectorLine.points2 = lineValues;

			//Line type discrete (Draw line between each 2 points)
			this.vectorLine.lineType = LineType.Discrete;

			this.vectorLine.Draw ();

			//Draw angle according to given starting and ending angle

			//Starting radius for arc ,will increase in each arc draw to show each arc in different way
			float arcRadius = 35; 

			foreach (AngleArc arcAngle in angleArcs) 
			{
				//Instantiate arc prefab
				GameObject arc = GameObject.Instantiate (arcPrefab);
				arc.transform.SetParent (this.transform, false);

				//UI polygon component to draw arc
				UIPolygon UIpolygon = arc.GetComponent<UIPolygon> ();

				//Difference between given angle
				float diff = arcAngle.endAngle - arcAngle.startAngle;

				float tempRadius=(arcAngle.radius == 0 ? arcRadius : arcAngle.radius);
				//set arc size according to radius
				UIpolygon.GetComponent<RectTransform> ().sizeDelta = Vector2.one * tempRadius;


				//If difference is 90 then draw square instead of arc
				if (Mathf.Abs(diff) == 90) 
				{
					UIpolygon.sides = 4;   //for square
					UIpolygon.rotation = 45f;  
					UIpolygon.fillPercent = 100;

					Vector2 quadrants = GetQuadrants (arcAngle.startAngle);
					//Set square in given position
					UIpolygon.GetComponent<RectTransform> ().anchoredPosition = arcAngle.origin + new Vector2( (quadrants.x*tempRadius) / 3f,  (quadrants.y*tempRadius) / 3f) ;
				} 
				else 
				{
					UIpolygon.fillPercent = Mathf.CeilToInt (100f * (diff) / 360f);
					UIpolygon.rotation = arcAngle.startAngle + 180f;
					UIpolygon.GetComponent<RectTransform> ().anchoredPosition = arcAngle.origin;

				}


				//Get center position to diplay arc text
				Vector2 centerPosition = (new Vector2 (arcAngle.origin.x + tempRadius* Mathf.Cos (Mathf.Deg2Rad * (arcAngle.startAngle + 90f)), arcAngle.origin.y + tempRadius* Mathf.Sin (Mathf.Deg2Rad * (arcAngle.startAngle + 90f))) - new Vector2 (arcAngle.origin.x + tempRadius * Mathf.Cos (Mathf.Deg2Rad * (arcAngle.endAngle + 90f)), arcAngle.origin.y + tempRadius * Mathf.Sin (Mathf.Deg2Rad * (arcAngle.endAngle + 90f)))).normalized * tempRadius;

				//Set text position in center of  arc
				UIpolygon.GetComponentInChildren<Text> ().GetComponent<RectTransform> ().anchoredPosition = centerPosition;

				//Set arc value text
				UIpolygon.GetComponentInChildren<Text> ().text = arcAngle.value;

				//Update arc radius for next arc draw
				arcRadius += 10f;

				UIpolygon.ReDraw ();


			}



		}

		private void AddTextInLine(string lineText,Vector2 point1,Vector2 point2,TextDir textDirection)
		{
			if (lineTextPrefab == null)
				return;
			
			Vector2 midPoint = new Vector2 ((point1.x + point2.x) / 2f, (point1.y + point2.y) / 2f);
			if (textDirection == TextDir.Up)
				midPoint.y += 15f;
			else if(textDirection == TextDir.Down)
				midPoint.y -= 15f;
			else if(textDirection == TextDir.Left)
				midPoint.x -= 60f;
			else if(textDirection == TextDir.Right)
				midPoint.x += 60f;
			
			GameObject lineTextObj = GameObject.Instantiate (lineTextPrefab);
			lineTextObj.transform.SetParent (this.transform, false);
			lineTextObj.GetComponent<RectTransform> ().anchoredPosition = midPoint;
			Text  lineTextComponent =lineTextObj.GetComponent<Text> ();
			lineTextComponent.text = lineText;

			float distance = Vector2.Distance (point1, point2);
			if (textDirection == TextDir.Left) 
			{
				lineTextComponent.alignment = TextAnchor.MiddleRight;
				lineTextObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (100f, distance <30 ?30f:distance );
			} 
			else if (textDirection == TextDir.Right) 
			{
				lineTextComponent.alignment = TextAnchor.MiddleLeft;
				lineTextObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (100f, distance <30 ?30f:distance );
			} 
			else {
				lineTextComponent.alignment = TextAnchor.MiddleCenter;
				lineTextObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (distance>100?distance:100f,40f);
			}
				
		}



		private Vector2 GetQuadrants(float angle)
		{
			int xSign = 1,ySign =1;
			if (angle >= 0 && angle <= 90f) {
				xSign = 1;
				ySign = 1;
			} else if (angle > 90 && angle <= 180f) {
				xSign = -1;
				ySign = 1;
			} else if (angle > 180f && angle <= 270f) 
			{
				xSign = -1;
				ySign = -1;
			} else if (angle > 270f && angle <= 360f) 
			{
				xSign = 1;
				ySign = -1;
			}

			return new Vector2(xSign,ySign);
		}

		public void Reset()
		{
			//Destroy child
			foreach (Transform child in transform) 
			{
				GameObject.Destroy(child.gameObject);
			}
			this.SetScale (1.2f);
			//Reset vectrosity line
			List <Vector2> lineValues = new List <Vector2> (){ new Vector2 (0, 0), new Vector2 (0, 0) };
			this.vectorLine.points2 = lineValues;
			this.linePoints.Clear ();
			this.angleArcs.Clear ();
			this.vectorLine.Draw ();
			this.ShiftPosition (Vector2.zero);
			this.vectorLine.SetColor (Color.black);
			this.vectorLine.lineWidth = 2f;

		}

		public void SetScale (float scale)
		{
			this.transform.localScale = Vector3.one * scale;
		}

		public void ShiftPosition (Vector2 position)
		{
			this.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 140f) + position;
		}
		public void RotatePosition (float angle)
		{
			this.GetComponent<RectTransform> ().localEulerAngles = new Vector3 (0, 0, angle);
		}

	
		public void DrawGrid(int x,int y,float offset)
		{
			List <Vector2> lineValues = new List <Vector2> ();
			for (int i = 0; i <= x; i++) 
			{
				lineValues.Add (new Vector2(i * offset, 0));
				lineValues.Add (new Vector2(i * offset, y * offset));
			}

			for (int i = 0; i <= y; i++) 
			{
				lineValues.Add (new Vector2(0, i * offset));
				lineValues.Add (new Vector2(x * offset,i * offset ));
			}

			//Assign point to vectrosity line alogrithm to draw line between points
			this.vectorLine.points2 = lineValues;

			//Line type discrete (Draw line between each 2 points)
			this.vectorLine.lineType = LineType.Discrete;

			this.vectorLine.SetColor (new Color(0.39f,0.39f,0.39f,1f),0,Mathf.RoundToInt(this.vectorLine.points2.Count/2f));
			this.vectorLine.SetWidth (0.5f, 0, Mathf.RoundToInt (this.vectorLine.points2.Count / 2f));

			this.vectorLine.Draw ();
		}

		public void DrawLineOnGrid(Vector2 point1,Vector2 point2 ,float offset)
		{

			this.vectorLine.points2.Add (point1 *offset);
			this.vectorLine.points2.Add (point2 *offset);

			//Line type discrete (Draw line between each 2 points)
			this.vectorLine.lineType = LineType.Discrete;

			this.vectorLine.SetColor (Color.black,Mathf.RoundToInt(this.vectorLine.points2.Count/2f)-1,Mathf.RoundToInt(this.vectorLine.points2.Count/2f));
			this.vectorLine.SetWidth (2f,Mathf.RoundToInt(this.vectorLine.points2.Count/2f)-1,Mathf.RoundToInt(this.vectorLine.points2.Count/2f));

			this.vectorLine.Draw ();
		}


	}
}

