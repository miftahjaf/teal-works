﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

	public class LinePoint
	{
		public string name;
		public Vector2 origin;
		public float angle;
		public float radius;
		public bool shouldShowArrow;
		public int textDirection;
		public Vector2 textOffset;
	    public LinePoint(string name,Vector2 origin,float angle,bool shouldShowArrow,float radius =100f,int textDirection=0)
	   {
			this.name = name;
			this.origin = origin;
			this.angle = angle;
			this.radius = radius;
			this.shouldShowArrow = shouldShowArrow;
			this.textDirection = textDirection;
			this.textOffset = textOffset;
		}
		public LinePoint(string name,Vector2 origin,float angle,bool shouldShowArrow,Vector2 textOffset,float radius =100f)
		{
			this.name = name;
			this.origin = origin;
			this.angle = angle;
			this.radius = radius;
			this.shouldShowArrow = shouldShowArrow;
			this.textDirection = textDirection;
			this.textOffset = textOffset;
		}

	}

	public class AngleArc
	{
		public string value;
		public float startAngle;
		public float endAngle;
		public Vector2 origin;
		public float radius;

		public AngleArc(string value,Vector2 origin,float startAngle,float endAngle,float radius=0)
		{
			this.value = value;
			this.startAngle = startAngle;
			this.endAngle = endAngle;
			this.origin = origin;
			this.radius = radius;
		}
	}

	public class LineAndAngle : MonoBehaviour 
	{
		private List<LinePoint> linePoints = new List<LinePoint> ();
		private List<AngleArc> angleArcs = new List<AngleArc> ();
		private VectorLine vectorLine;
		public GameObject pointPrefab;
		public GameObject arcPrefab;

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


		public void DrawDiagram()
		{
			//Add pairs of point to generate disrete line
			List <Vector2> lineValues = new List <Vector2> ();
			foreach (LinePoint linePoint in linePoints)
			{
				//starting point
				lineValues.Add (linePoint.origin);
			
				//Calculate ending point using angle and given radius
				Vector2 newPoint =MathFunctions.PointAtDirection(linePoint.origin,linePoint.angle,linePoint.radius);

			
				lineValues.Add(newPoint);

				//Instantiate point prefab to show point, point name and arrow
			     GameObject endPoint = GameObject.Instantiate (pointPrefab);
				 endPoint.transform.SetParent (this.transform, false);

				//set arrow, point and label poistion
				endPoint.GetComponent<LinePointScript> ().SetPoint (linePoint);


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

		}

		public void SetScale(float scale)
		{
			this.transform.localScale = Vector3.one * scale;
		}




	}

