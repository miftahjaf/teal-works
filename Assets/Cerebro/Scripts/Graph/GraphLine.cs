using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
using UnityEngine.UI;
namespace Cerebro
{
	public class GraphLine 
	{
		public VectorLine vectorLine;
		public GraphPointScript point1;
		public GraphPointScript point2;
		private Sprite arrowSprite;
		private GameObject arrow1, arrow2;
		private bool isLineSegment;
		private bool isLineVisible;
		private LineShapeType lineShapeType;
		public GraphLine(VectorLine vectorLine,GraphPointScript point1,GraphPointScript point2,Sprite arrowSprite,bool isLineSegment =false,bool isLineVisible=true,LineShapeType lineShapeType = LineShapeType.Normal)
		{
			this.vectorLine = vectorLine;
			this.point1 = point1;
			this.point2 = point2;
			this.arrowSprite = arrowSprite;
			this.isLineSegment = isLineSegment;
			this.isLineVisible = isLineVisible;
			this.lineShapeType = lineShapeType;
		}

		public void Draw()
		{
			if (this.vectorLine == null || !isLineVisible )
				return;

			this.vectorLine.points2 = new List<Vector2> (){ point1.linePoint.origin, point2.linePoint.origin };
			this.vectorLine.Draw ();
		}

		public void Draw(Vector2[] points)
		{
			if (!isLineVisible) {
			}
			if (this.vectorLine == null && points.Length < 1 && !points [0].Equals (points [1])) {
				return;
			}

			this.vectorLine.points2 = new List<Vector2> (){ points[0], points[1] };
			float angle = points[0].x==points [1].x  ? -90f: Mathf.Atan ((points [1].y - points [0].y) /(points [1].x - points [0].x)) *Mathf.Rad2Deg ;
			if (arrow1 == null)
			{
				arrow1 = GenerateArrow ();
			}
			arrow1.GetComponent<RectTransform> ().anchoredPosition = points [0];
			arrow1.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0f, 0f,  angle );

			if (arrow2 == null)
			{
				arrow2 = GenerateArrow ();
			}
			arrow2.GetComponent<RectTransform> ().anchoredPosition = points [1];
			arrow2.GetComponent<RectTransform> ().eulerAngles = new Vector3 (0f, 0f,180+ angle );


			this.vectorLine.Draw ();
		}

		public GameObject GenerateArrow()
		{
			GameObject arrow = new GameObject ();
			arrow.name = "Arrow";
			arrow.transform.SetParent (vectorLine.rectTransform.transform,false);
			arrow.AddComponent<RectTransform> ();
			arrow.GetComponent<RectTransform> ().sizeDelta = new Vector2 (15, 15);
			arrow.AddComponent<Image> ();
			arrow.GetComponent<Image> ().raycastTarget = false;
			arrow.GetComponent<Image> ().color = Color.black;
			arrow.GetComponent<Image> ().sprite = arrowSprite;
			return arrow;
		}

		public bool IsLineSegment()
		{
			return this.isLineSegment;
		}

		public bool IsLineVisible()
		{
			return this.isLineVisible;
		}


		public LineShapeType LineShapeType()
		{
			return this.lineShapeType;
		}
	}
}
