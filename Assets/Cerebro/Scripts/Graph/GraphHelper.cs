using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
namespace Cerebro
{
	public class GraphHelper : MonoBehaviour 
	{
		public GameObject vectorObjectPrefab;
		public GameObject linePointPrefab;
		private Vector2 gridCoordinateRange;
		private Vector2 graphOrigin;
		private Vector2 graphCenter;
		private Vector2 graphCenterOffset;
		private Vector2 gridPosition;
		private Vector2 axisOffset;
		private float gridOffset;
		private float fontMultiPlier = 1f;
		private float snapValue;
		private Vector2 graphMinValue;
		private Vector2 graphMaxValue;
		private GameObject pointLineDisplay;
		private List<GraphLine> graphLines;


		public void Reset()
		{
			//Destroy child
			foreach (Transform child in transform) 
			{
				GameObject.Destroy(child.gameObject);
			}
			graphLines = new List<GraphLine> ();
			axisOffset = new Vector2 (10, 10);
			fontMultiPlier = 1f;
		}

		public void SetGridParameters (Vector2 _gridCoordinateRange ,float _gridOffset)
		{
		     gridCoordinateRange = _gridCoordinateRange;
			 gridOffset = _gridOffset;
			 graphCenter = new Vector2 (Mathf.RoundToInt(gridCoordinateRange.x * gridOffset / 2f),Mathf.RoundToInt(gridCoordinateRange.y * gridOffset / 2f));
			 gridPosition = new Vector2 (-gridCoordinateRange.x * gridOffset/2f , -gridCoordinateRange.y * gridOffset/2f);
			 graphOrigin = graphCenter;
			 snapValue = gridOffset;
			 
		}

		public void SetSnapValue(float _snapValue)
		{
			this.snapValue = _snapValue;
		}
			
		public void SetGraphParameters (Vector2 _axisOffset)
		{
		  axisOffset = _axisOffset;
		}

		public void ShiftGraphOrigin(Vector2 offset)
		{
			graphCenterOffset = offset;
			graphOrigin = graphCenter - new Vector2 (Mathf.RoundToInt(offset.x  * gridOffset ),Mathf.RoundToInt(offset.y* gridOffset));
		}

		public void DrawGraph()
		{
			DrawGrid ();
			DrawAxis ();
		}

		public void DrawGrid()
		{
			GameObject grid = GameObject.Instantiate (vectorObjectPrefab);
			grid.transform.SetParent (this.transform,false);
			grid.name = "grid";
			grid.transform.GetComponent<RectTransform> ().anchoredPosition = gridPosition;

			List <Vector2> linePoints = new List <Vector2> ();

			for (int i = 0; i <= gridCoordinateRange.x; i++) 
			{
				linePoints.Add (new Vector2(i * gridOffset, 0));
				linePoints.Add (new Vector2(i * gridOffset, gridCoordinateRange.y * gridOffset));
			}

			for (int i = 0; i <= gridCoordinateRange.y; i++) 
			{
				linePoints.Add (new Vector2(0, i * gridOffset));
				linePoints.Add (new Vector2(gridCoordinateRange.x * gridOffset,i * gridOffset ));
			}

			VectorLine vectorLine = grid.GetComponent<VectorObject2D> ().vectorLine;
			vectorLine.points2 = linePoints;
			vectorLine.SetWidth (0.7f);
			vectorLine.lineType = LineType.Discrete;
			vectorLine.SetColor (new Color(0.58f,0.58f,0.58f,1f));
			vectorLine.Draw ();
		}

		public void DrawAxis()
		{
			GameObject axis = GameObject.Instantiate (vectorObjectPrefab);
			axis.transform.SetParent (this.transform,false);
			axis.name = "axis";
			axis.transform.GetComponent<RectTransform> ().anchoredPosition = gridPosition;


			List <Vector2> points = new List <Vector2> ();
			points.Add(new Vector2(0f,graphOrigin.y));
			points.Add(new Vector2(gridCoordinateRange.x * gridOffset,graphOrigin.y));
			points.Add(new Vector2(graphOrigin.x,0f));
			points.Add(new Vector2(graphOrigin.x,gridCoordinateRange.y * gridOffset));

			VectorLine vectorLine = axis.GetComponent<VectorObject2D> ().vectorLine;
			vectorLine.points2 = points;
			vectorLine.SetWidth (2f);
			vectorLine.SetColor (Color.black);
			vectorLine.lineType = LineType.Discrete;
			vectorLine.Draw ();

			float[] angles = new float[]{ 180f, 0f, 270f, 90f };
			int cnt = 0;
			foreach (Vector2 point in points)
			{
				if(graphOrigin == Vector2.zero && (cnt == 0 || cnt == 2))
				{
					cnt++;
					continue;
				}
				GenerateLinePoint (new LinePoint("",point + gridPosition, angles[cnt], true,0));
				cnt++;
			}

			//Pos X
			int pointInPosXAxis = Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.x/2f + graphCenterOffset.x)));
			graphMaxValue.x = pointInPosXAxis * axisOffset.x;
			for (int i = 1; i < pointInPosXAxis; i++) 
			{
				GenerateLinePoint (new LinePoint ((i * axisOffset.x).ToString(), GraphPosToUIPos (new Vector2 (i * axisOffset.x, 0)), 0f, false, 0));
			}

			//Neg X
			int pointInNegXAxis = Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.x/2f - graphCenterOffset.x)));
			graphMinValue.x = -pointInNegXAxis * axisOffset.x;
			for (int i = 1; i < pointInNegXAxis; i++) 
			{
				GenerateLinePoint (new LinePoint ("-"+(i * axisOffset.x).ToString(), GraphPosToUIPos (new Vector2 (-i * axisOffset.x, 0)), 0f, false, 0));
			}

			//Pos Y
			int pointInPosYAxis =  Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.y/2 +  graphCenterOffset.y)));
			graphMaxValue.y = pointInPosYAxis * axisOffset.y;
			for (int i = 1; i < pointInPosYAxis; i++) 
			{
				GenerateLinePoint (new LinePoint ((i * axisOffset.y).ToString(), GraphPosToUIPos (new Vector2 (0, i * axisOffset.y)), 0f, false, 0,-1));
			}

			//Neg Y
			int pointInPNegYAxis =   Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.y/2f - graphCenterOffset.y)));
			graphMinValue.y = -pointInPNegYAxis * axisOffset.y;
			for (int i = 1; i < pointInPNegYAxis; i++) 
			{
				GenerateLinePoint (new LinePoint ("-"+(i * axisOffset.y).ToString(), GraphPosToUIPos (new Vector2 (0, -i * axisOffset.y)), 0f, false, 0,-1));
			}
		}

		public GraphPointScript GenerateLinePoint(LinePoint linePoint)
		{
			GameObject linePointObj = GameObject.Instantiate (linePointPrefab);
			linePointObj.transform.SetParent (this.transform, false);
			GraphPointScript graphPointScript = linePointObj.GetComponent<GraphPointScript> ();
			graphPointScript.SetPoint(linePoint);
			graphPointScript.SetTextFontMultiplier (fontMultiPlier);

			return graphPointScript;
		}

		public GraphPointScript PlotPoint(Vector2 point,string displayText="",bool canDrag =true)
		{
			if (IsContainInGraph (point)) {
				GraphPointScript graphPointScript = GenerateLinePoint (new LinePoint (string.IsNullOrEmpty(displayText) ?"": displayText+" (" + point.x + "," + point.y + ")", GraphPosToUIPos (point), 0f, false, 0));
				graphPointScript.SetPointColor (Color.blue);
				if (canDrag) 
				{
					graphPointScript.onDragEvent += MovePlottedPoint;
					graphPointScript.onDragEndEvent += MoveEndPlottedPoint;
				}
				return graphPointScript;
			} else {
				Debug.Log ("(" + point.x + "," + point.y + ") is not in graph");
			}
			return null;
		}



		public bool IsContainInGraph(Vector2 point)
		{
			if (point.x <= graphMaxValue.x && point.x >= graphMinValue.x && point.y <= graphMaxValue.y && point.y >= graphMinValue.y) {
				return true;
			} 
			else 
			{
				return false;
			}
		}
			
		public Vector2 GraphPosToUIPos(Vector2 point)
		{
			return new Vector2((point.x / axisOffset.x) * gridOffset, (point.y / axisOffset.y) * gridOffset)+ gridPosition + graphOrigin;
		}

		public Vector2 UIPosToGraphPos(Vector2 position)
		{
			return new Vector2 (MathFunctions.GetRounded((position.x - gridPosition.x - graphOrigin.x) * axisOffset.x / gridOffset,1), MathFunctions.GetRounded(((position.y - gridPosition.y - graphOrigin.y) * axisOffset.y / gridOffset),1));
		}

	
		public void SetFontMultiplier(float _fontMultiplier)
		{
			fontMultiPlier = _fontMultiplier;
		}

		public Vector2 SnapPosition(Vector2 position)
		{
			return new Vector2 (Mathf.RoundToInt (position.x/snapValue) * snapValue, Mathf.RoundToInt (position.y/snapValue) *snapValue);
		}

		public Vector2 SnapPoint(Vector2 point)
		{
			return new Vector2 (Mathf.RoundToInt (point.x/axisOffset.x) * axisOffset.x, Mathf.RoundToInt (point.y/axisOffset.y) *axisOffset.y);
		}


		public void MovePlottedPoint(GraphPointScript graphPointObj,Vector2 position)
		{
			RectTransform pointRectTransform = graphPointObj.GetComponent<RectTransform> ();
			Vector2 oldPos = pointRectTransform.anchoredPosition;
			graphPointObj.transform.position = position;
			pointRectTransform.anchoredPosition = SnapPosition(pointRectTransform.anchoredPosition);
			Vector2 pos =pointRectTransform.anchoredPosition;
			Vector2 graphPoint = UIPosToGraphPos (pos);
		
			if(!IsContainInGraph(graphPoint))
			{
				pointRectTransform.anchoredPosition = oldPos;
				return;	
			}

			if (!string.IsNullOrEmpty (graphPointObj.pointName.text)) {
				graphPointObj.pointName.text = "(" + graphPoint.x + "," + graphPoint.y + ")";
			}
			if (pointLineDisplay == null) 
			{
				pointLineDisplay = GameObject.Instantiate (vectorObjectPrefab);
				pointLineDisplay.transform.SetParent (this.transform);
				pointLineDisplay.transform.localScale = Vector3.one;
				pointLineDisplay.GetComponent<RectTransform> ().anchoredPosition = gridPosition;
				pointLineDisplay.name = "PointDisplayLines";
			}

			if (graphPointObj.lineObj != null)
			{
				graphPointObj.linePoint.origin = GraphPosToUIPos (graphPoint);
				graphPointObj.lineObj.Draw ();
			}

			VectorLine vectorLine = pointLineDisplay.GetComponent<VectorObject2D> ().vectorLine;
			Vector2 gridPos = pos - gridPosition;//GraphPosToGridPos (graphPoint);
			List <Vector2> points = new List <Vector2> ();
			points.Add(new Vector2(0f,gridPos.y));
			points.Add(new Vector2(gridCoordinateRange.x * gridOffset,gridPos.y));
			points.Add(new Vector2(gridPos.x,0f));
			points.Add(new Vector2(gridPos.x,gridCoordinateRange.y * gridOffset));

			vectorLine.points2 = points;
			vectorLine.SetWidth (1f);
			vectorLine.SetColor (Color.blue);
			vectorLine.lineType = LineType.Discrete;
			vectorLine.Draw ();
		}

		public void MoveEndPlottedPoint(GraphPointScript graphPointObj)
		{
			if (pointLineDisplay != null)
			{
				VectorLine vectorLine = pointLineDisplay.GetComponent<VectorObject2D> ().vectorLine;
				vectorLine.points2 = new List<Vector2>(){Vector2.zero,Vector2.zero};
				vectorLine.Draw ();
			}
		}


		public void DrawRanomLine()
		{
			Vector2 point1 = GetRanomPointInGraph();
			GraphPointScript graphPointScript1 = PlotPoint (point1,"");
			Vector2 point2 = GetRanomPointInGraph();
			while (Vector2.Distance (UIPosToGraphPos(point1), UIPosToGraphPos(point2)) <15f) 
			{
				point2 = GetRanomPointInGraph();
			}
			GraphPointScript graphPointScript2 = PlotPoint (point2,"");

			GameObject lineObj = GameObject.Instantiate (vectorObjectPrefab);
			lineObj.transform.SetParent (this.transform);
			lineObj.transform.localScale = Vector3.one;
			lineObj.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

			VectorLine vectorLine = lineObj.GetComponent<VectorObject2D> ().vectorLine;
			GraphLine graphLine = new GraphLine (vectorLine, graphPointScript1, graphPointScript2);
			graphLine.Draw ();
			graphPointScript1.SetLineObject (graphLine);
			graphPointScript2.SetLineObject (graphLine);

			graphLines.Add(graphLine);
		}

		public Vector2 GetRanomPointInGraph()
		{
			return SnapPoint(new Vector2 (Random.Range (graphMinValue.x, graphMaxValue.x), Random.Range (graphMinValue.y, graphMaxValue.y)));
		}
	}
}