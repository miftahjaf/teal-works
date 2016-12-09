using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Vectrosity;

namespace Cerebro
{
	public enum StatisticsType
	{
		VerticalBar,
		HorizontalBar,
		Pie,
		Line,
		None
	}

	public class StatisticsHelper : MonoBehaviour
	{
		public GameObject vectorObjectPrefab; //Vector line prefab to draw line
		public GameObject vectorDottedObjectPrefab; //Vector line prefab to draw dotted line
		public GameObject linePointPrefab;   // point prefab to render point 
		public GameObject arcPrefab;
		public GameObject textObjectPrefab;

		public Sprite arrowSprite;
		private GameObject pointLineDisplay;   //Display line when change plotted point position
		private GameObject touchObj;            // Handle touch position
		private GameObject axisObj;             // Axis line object
		private Vector2 gridCoordinateRange;    //Grid cooridnate 
		private Vector2 graphOrigin;            //Graph origin (Default graph center)
		private Vector2 graphCenter;            //Graph center
		private Vector2 graphCenterOffset;      //Graph center offset to move grid origin
		private Vector2 gridPosition;           //Grid position
		private Vector2 axisOffset;             //Axis offset (Default 1)
		private Vector2 graphMinValue;          //Graph Min x and y point value
		private Vector2 graphMaxValue;          //Graph Max x and y point value
		private Vector2 pointOffset;

		private float gridOffset;               //Grid offset
		private float fontMultiPlier;           //Font multiplier to make graph font smaller or bigger
		private float snapValue;                //Graph sanp value (Default grid offset)

		private bool isInteractable;

		private string graphTitle;

		public StatisticsType statisticType;

		private StatisticsAxis[] statisticsAxises;
		private List<StatisticsBar> statisticsBars;


		//Reset old set values
		public void Reset()
		{
			//Destroy all childs
			foreach (Transform child in transform) 
			{
				GameObject.Destroy(child.gameObject);
			}
			graphCenterOffset = Vector2.zero;
			axisOffset = new Vector2 (1, 1);
			fontMultiPlier = 1f;
			statisticType = StatisticsType.None;
			statisticsAxises = new StatisticsAxis[]{new StatisticsAxis(),new StatisticsAxis() };
			statisticsBars = new List<StatisticsBar> ();
			isInteractable = false;
			this.ShiftPosition(Vector2.zero);

		}

		//Set grid parameters
		public void SetGridParameters (Vector2 _gridCoordinateRange ,float _gridOffset)
		{
			gridCoordinateRange = _gridCoordinateRange;
			gridOffset = _gridOffset;
			graphCenter = new Vector2 (Mathf.RoundToInt(gridCoordinateRange.x * gridOffset / 2f),Mathf.RoundToInt(gridCoordinateRange.y * gridOffset / 2f));
			gridPosition = new Vector2 (-gridCoordinateRange.x * gridOffset/2f , -gridCoordinateRange.y * gridOffset/2f);
			graphOrigin = graphCenter;
			snapValue = gridOffset;
			this.GetComponent<Image> ().GetComponent<RectTransform> ().sizeDelta = Vector2.one * _gridCoordinateRange.x * gridOffset;
			ShiftGraphOrigin (new Vector2 (2, 2) - _gridCoordinateRange / 2);
		}

		//Set graph parameters
		public void SetGraphParameters (StatisticsAxis[] _statisticsAxises)
		{
			statisticsAxises = _statisticsAxises;
			axisOffset = new Vector2 (statisticsAxises [0].offsetValue, statisticsAxises [1].offsetValue);
			pointOffset = new Vector2 (statisticsAxises [0].pointOffset,statisticsAxises [1].pointOffset);
		}

		//Set Is Interactable
		public void SetInteractable(bool _isInteractable)
		{
			this.isInteractable = _isInteractable;
		}

		//Set snap value
		public void SetSnapValue(float _snapValue)
		{
			this.snapValue = _snapValue;
		}
			
		//Set graph origin
		public void ShiftGraphOrigin(Vector2 offset)
		{
			graphCenterOffset = -offset;
			graphOrigin = graphCenter - new Vector2 (Mathf.RoundToInt(graphCenterOffset.x  * gridOffset ),Mathf.RoundToInt(graphCenterOffset.y* gridOffset));
		}

		//Set statistics type
		public void SetStatisticsType(StatisticsType _statisticsType)
		{
			statisticType = _statisticsType;
		}

		//Draw graph and grid according to parameters
		public void DrawGraph(bool showAxis = true)
		{
			DrawGrid ();
			DrawAxis (showAxis);
			SetTitles ();
		}

		//Draw grid
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

		//Draw axis and plot points on axis
		public void DrawAxis(bool showAxis = true)
		{
			GameObject axisParent = new GameObject ();
			axisParent.transform.SetParent (this.transform, false);
			axisParent.name = "axisparent";
			axisObj = GameObject.Instantiate (vectorObjectPrefab);
			axisObj.transform.SetParent (axisParent.transform,false);
			axisObj.name = "axis";
			axisObj.transform.GetComponent<RectTransform> ().anchoredPosition = gridPosition;


			List <Vector2> points = new List <Vector2> ();
			points.Add(new Vector2(graphOrigin.x,graphOrigin.y));
			points.Add(new Vector2(gridCoordinateRange.x * gridOffset,graphOrigin.y));
			points.Add(new Vector2(graphOrigin.x,graphOrigin.y));
			points.Add(new Vector2(graphOrigin.x,gridCoordinateRange.y * gridOffset));

			VectorLine vectorLine = axisObj.GetComponent<VectorObject2D> ().vectorLine;
			vectorLine.points2 = points;
			vectorLine.SetWidth (1f);
			vectorLine.SetColor (Color.black);
			vectorLine.lineType = LineType.Discrete;
			vectorLine.Draw ();

			float[] angles = new float[]{ 180f, 0f, 270f, 90f };
			int cnt = 0;
			foreach (Vector2 point in points)
			{
				if((graphOrigin == Vector2.zero || points[0].Equals(points[2])) && (cnt == 0 || cnt == 2))
				{
					cnt++;
					continue;
				}
				GenerateLinePoint (new LinePoint("",point + gridPosition, angles[cnt], true,0),axisParent);
				cnt++;
			}
		
			StatisticsAxis statisticsAxis = statisticsAxises [0];
			float startOffset=0;
			//Pos X
			int pointInPosXAxis =0;
			if(statisticsAxis.statisticsValues.Count==0)
			{
				pointInPosXAxis =  Mathf.Abs((Mathf.RoundToInt((gridCoordinateRange.x/2f + graphCenterOffset.x)/pointOffset.x)))-1;
				graphMaxValue.x = (gridCoordinateRange.x / 2f + graphCenterOffset.x) * axisOffset.x / pointOffset.x;
			}
			else
			{
				if (statisticType == StatisticsType.HorizontalBar || statisticType == StatisticsType.VerticalBar) {
					startOffset = axisOffset.x / pointOffset.x;
				}
				pointInPosXAxis = statisticsAxis.statisticsValues.Count;
				graphMaxValue.x =  Mathf.Abs((Mathf.RoundToInt((gridCoordinateRange.x/2f + graphCenterOffset.x)))) * axisOffset.x/pointOffset.x ;
			}

			for (int i = 1; i <= pointInPosXAxis; i++) 
			{
				string text = (i * axisOffset.x).ToString ();
				int value = 0;
				if (statisticType == StatisticsType.VerticalBar) {
					text = "";
					if(statisticsAxis.statisticsValues.Count > i - 1 )
					{
						text =  statisticsAxis.statisticsValues [i - 1].name;
						int totalValues = statisticsAxis.statisticsValues [i - 1].values.Length;
						float offsetValue = (2 *startOffset) / totalValues;
						float startPoint = i * axisOffset.x - GetStartOffsetValue(startOffset, totalValues);

						for (int count = 0; count < totalValues; count++) {
							value = statisticsAxis.statisticsValues [i - 1].values [count];
							GenerateGraphBar (new Vector2 ( startPoint,0f), new Vector2 (startPoint,value ),Color.black,2f/totalValues);
							startPoint +=offsetValue;
						}
					}

				}
				GenerateLinePoint (new LinePoint (text, GraphPosToUIPos (new Vector2 (i * axisOffset.x-startOffset, 0)), 0f, false,0).SetPointTextOffset(new Vector2(0,-15)),axisParent);
			}

			//Neg X
			graphMinValue.x = 0f;
			statisticsAxis = statisticsAxises [1];

			//Pos Y
			int pointInPosYAxis =  0;

			if(statisticsAxis.statisticsValues.Count==0)
			{
				pointInPosYAxis =  Mathf.Abs((Mathf.RoundToInt((gridCoordinateRange.y/2f + graphCenterOffset.y)/pointOffset.y)))-1;
				graphMaxValue.y =(gridCoordinateRange.y / 2f + graphCenterOffset.y) * axisOffset.y / pointOffset.y;
			}
			else
			{
				if (statisticType == StatisticsType.HorizontalBar || statisticType == StatisticsType.VerticalBar) {
					startOffset = axisOffset.y / pointOffset.y;
				}
				pointInPosYAxis = statisticsAxis.statisticsValues.Count;
				graphMaxValue.y = Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.y/2f + graphCenterOffset.y))) * axisOffset.y/pointOffset.y;
			}

			for (int i = 1; i <= pointInPosYAxis; i++) 
			{
				string text = (i * axisOffset.y).ToString ();
				int value = 0;
				if (statisticType == StatisticsType.HorizontalBar) {
					text = "";
					if(statisticsAxis.statisticsValues.Count > i - 1 )
					{
						text =  statisticsAxis.statisticsValues [i - 1].name;
						int totalValues = statisticsAxis.statisticsValues [i - 1].values.Length;
						float offsetValue = (2 *startOffset) / totalValues;
						float startPoint = i * axisOffset.y - GetStartOffsetValue(startOffset, totalValues);

						for (int count = 0; count < totalValues; count++) {
							value = statisticsAxis.statisticsValues [i - 1].values [count];
							GenerateGraphBar (new Vector2 (0f,startPoint), new Vector2 (value, startPoint),Color.black,2f/totalValues);
							startPoint +=offsetValue;
						}
					}

				}
				GenerateLinePoint (new LinePoint (text, GraphPosToUIPos (new Vector2 (0, i  * axisOffset.y -startOffset)), 0f, false, 0).SetPointTextOffset(new Vector2(-20,0)),axisParent);
			}


			//Neg 
			graphMinValue.y = 0f;
			axisParent.gameObject.SetActive (showAxis);
			Debug.Log ("graph min value "+graphMinValue + " graph max value " + graphMaxValue);
		}

		private float GetStartOffsetValue(float offset, int noOfValues)
		{
			float value = offset;
			for (int i =1; i < noOfValues; i++) {
				value += (offset/ (i*2f));
			}
			return value;
		}


		public void GenerateGraphBar(Vector2 startPoint,Vector2 endPoint,Color color,float width = 2f)
		{
			GameObject barObj = new GameObject ();
			barObj.transform.SetParent (this.transform, false);
			StatisticsBar statisticsBar = barObj.AddComponent<StatisticsBar> ();
			barObj.AddComponent<Image> ();
			barObj.GetComponent<Image> ().raycastTarget = false;
			barObj.GetComponent<Image> ().color = color;
			barObj.name = "Bar";
			barObj.transform.GetComponent<RectTransform> ().anchoredPosition = GraphPosToUIPos (startPoint);
			float intitalHeight;
			if (statisticType == StatisticsType.VerticalBar) 
			{
				barObj.transform.GetComponent<RectTransform> ().pivot = new Vector2 (0.5f, 0f);
			} 
			else 
			{
				barObj.transform.GetComponent<RectTransform> ().pivot = new Vector2 (0f, 0.5f);
			}
			statisticsBar.SetIsHorizontal (statisticType == StatisticsType.VerticalBar);
			float height = Vector2.Distance (GraphPosToUIPos (startPoint), GraphPosToUIPos (endPoint));
			statisticsBar.SetHeight (height);
			statisticsBar.SetCurrentHeight (isInteractable?gridOffset: height);
			statisticsBar.SetWidth (gridOffset*width);
			statisticsBar.SetBar ();
			statisticsBars.Add (statisticsBar);

			if (isInteractable) 
			{
				GraphPointScript pointScript = PlotPoint (startPoint + (statisticType == StatisticsType.VerticalBar ? new Vector2 (0f, axisOffset.y/pointOffset.y) : new Vector2 (axisOffset.x/pointOffset.x, 0f)), "", true, false);
				pointScript.SetStatisticsBar (statisticsBar);
			}
		}


			
		//Generate line point 
		public void GenerateLinePoint(LinePoint linePoint,GameObject parent)
		{
			GameObject linePointObj = GameObject.Instantiate (linePointPrefab);
			linePointObj.transform.SetParent (parent.transform, false);
			GraphPointScript graphPointScript = linePointObj.GetComponent<GraphPointScript> ();
			graphPointScript.SetPoint(linePoint);
			graphPointScript.SetTextFontMultiplier (fontMultiPlier);

		}

		//Generate line point 
		public GraphPointScript GenerateLinePoint(LinePoint linePoint)
		{
			GameObject linePointObj = GameObject.Instantiate (linePointPrefab);
			linePointObj.transform.SetParent (this.transform, false);
			GraphPointScript graphPointScript = linePointObj.GetComponent<GraphPointScript> ();
			graphPointScript.SetPoint(linePoint);
			graphPointScript.SetTextFontMultiplier (fontMultiPlier);

			return graphPointScript;
		}
			

		//Plot point and choose can drag or not?
		public GraphPointScript PlotPoint(Vector2 point,string displayText="",bool canDrag =true,bool checkInsideGraph = true)
		{
			if (IsContainInGraph (point) || !checkInsideGraph) {
				GraphPointScript graphPointScript = GenerateLinePoint (new LinePoint (displayText, GraphPosToUIPos (point), 0f, false,0).SetPointTextOffset(new Vector2(0,-10)));
				graphPointScript.SetPointColor (Color.blue);
				graphPointScript.SetDotSize (3f);
				graphPointScript.SetIsDragabble (canDrag);
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



		//Is given point inside graph?
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

		//Graph pos to UI pos	
		public Vector2 GraphPosToUIPos(Vector2 point)
		{
			return new Vector2((point.x / axisOffset.x * pointOffset.x) * gridOffset, (point.y / axisOffset.y * pointOffset.y) * gridOffset )+ gridPosition + graphOrigin;
		}

		//UI pos to graph pos
		public Vector2 UIPosToGraphPos(Vector2 position)
		{
			return new Vector2 (MathFunctions.GetRounded((position.x - gridPosition.x - graphOrigin.x) * axisOffset.x / pointOffset.x/ gridOffset,1), MathFunctions.GetRounded(((position.y - gridPosition.y - graphOrigin.y) * axisOffset.y / pointOffset.y / gridOffset),1));
		}

		//Set font multiplier
		public void SetFontMultiplier(float _fontMultiplier)
		{
			fontMultiPlier = _fontMultiplier;
		}

		//Set snap position
		public Vector2 GetSnapPosition(Vector2 position)
		{
			return new Vector2 (Mathf.RoundToInt (position.x/snapValue ) * snapValue , Mathf.RoundToInt (position.y/snapValue) *snapValue);
		}

		//Get snap point from given point
		public Vector2 GetSnapPoint(Vector2 point)
		{
			return new Vector2 (Mathf.RoundToInt (point.x/(axisOffset.x/pointOffset.x)) *(axisOffset.x/pointOffset.x), Mathf.RoundToInt (point.y/(axisOffset.y/pointOffset.y)) *(axisOffset.y/pointOffset.y));
		}

		//Move plotted point or line point and change line according to point movement
		public void MovePlottedPoint(GraphPointScript graphPointObj,Vector2 position)
		{
			RectTransform pointRectTransform = graphPointObj.GetComponent<RectTransform> ();
			Vector2 oldPos = pointRectTransform.anchoredPosition;

			if (graphPointObj.statisticsBar != null) 
			{
				if(graphPointObj.statisticsBar.IsHorizontal())
				{
					position.x = graphPointObj.transform.position.x;
				}
				else
				{
					position.y = graphPointObj.transform.position.y;
				}
			}
			graphPointObj.transform.position = position;

			Vector2 graphPoint = GetSnapPoint(UIPosToGraphPos(pointRectTransform.anchoredPosition));
			Vector2 pos = GraphPosToUIPos (graphPoint);
			pointRectTransform.anchoredPosition = pos;

			if(!IsContainInGraph(graphPoint))
			{
				pointRectTransform.anchoredPosition = oldPos;
				return;	
			}

			 
			if (graphPointObj.statisticsBar != null) 
			{
				graphPointObj.statisticsBar.SetCurrentHeight (GraphPosToUIPos(graphPoint));
				graphPointObj.statisticsBar.SetBar ();
			}
		
			if (pointLineDisplay == null) 
			{
				pointLineDisplay = GameObject.Instantiate (vectorObjectPrefab);
				pointLineDisplay.transform.SetParent (this.transform);
				pointLineDisplay.transform.localScale = Vector3.one;
				pointLineDisplay.GetComponent<RectTransform> ().anchoredPosition = gridPosition;
				pointLineDisplay.name = "PointDisplayLines";
			}

			graphPointObj.linePoint.origin = GraphPosToUIPos (graphPoint);


			if(graphPointObj.diagramObj != null)
			{
				graphPointObj.diagramObj.Draw ();
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


		//Drag End of plotted point
		public void MoveEndPlottedPoint(GraphPointScript graphPointObj)
		{
			if (pointLineDisplay != null)
			{
				VectorLine vectorLine = pointLineDisplay.GetComponent<VectorObject2D> ().vectorLine;
				vectorLine.points2 = new List<Vector2>(){Vector2.zero,Vector2.zero};
				vectorLine.Draw ();
			}
		}

		public bool CheckAnswer ()
		{
			return true;
		}

		public void HandleCorrectAnswer ()
		{

		}
		public void ResetAnswer ()
		{

		}
		public void HandleIncorrectAnwer (bool isRevisitedQuestion)
		{

		}

		public bool IsAnswered ()
		{

			return true;
		}

		//Shift graph position
		public void ShiftPosition (Vector2 position)
		{
			this.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -30f) + position;
		}

		private void SetTitles()
		{
			GenerateTextObject (new Vector2(0,-gridPosition.y + 17f),graphTitle,"XAxis Title",15);
			GenerateTextObject (new Vector2(0,gridPosition.y - 17f),statisticsAxises[0].axisName,"XAxis Title");
			GenerateTextObject (new Vector2(gridPosition.x - 20f,0),statisticsAxises[1].axisName,"yAxis Title",13,90);
		}

		private void GenerateTextObject(Vector2 position,string text,string name,int fontSize=13,float rotation=0)
		{
			if(string.IsNullOrEmpty(text))
			{
			  return;
			}
			GameObject textObj = GameObject.Instantiate (textObjectPrefab);
			textObj.GetComponent<Text> ().text = text;
			textObj.GetComponent<Text> ().fontSize = fontSize;
			textObj.GetComponent<RectTransform> ().anchoredPosition =position;
			textObj.transform.SetParent (this.transform, false);
			textObj.GetComponent<RectTransform> ().localEulerAngles = new Vector3 (0f, 0f, rotation);
			textObj.name = name;
		}

		public void SetGraphTitle(string _graphTitle)
		{
			graphTitle = _graphTitle;
		}
			
	}
}
