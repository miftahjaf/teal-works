using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vectrosity;
using MaterialUI;


namespace Cerebro
{
	//Graph quetsions type to handle answer
	public enum GraphQuesType
	{
		HighlightQuadrant,
		HighlightAxis,
		PlotLine,
		PlotFixedLine,
		PlotPoint,
		None

	}
	public class GraphHelper : MonoBehaviour,IPointerClickHandler
	{
		public GameObject vectorObjectPrefab; //Vector line prefab to draw line
		public GameObject linePointPrefab;   // point prefab to render point 

		private GameObject pointLineDisplay;   //Display line when change plotted point position
		private GameObject highLightedQuadrant; // Highlight selected quadrant
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
		private Vector2 correctPlottedPoint;    //Correct current plotted value used to check answer 
		private Vector2[] fixedLinePoints;      //Points of line with fixed point

		private Vector3 currentLineParameters;  //Line paramter to check line equation (x = a, y = b, z = c) ax+by+c=0

		private float gridOffset;               //Grid offset
		private float fontMultiPlier;           //Font multiplier to make graph font smaller or bigger
		private float snapValue;                //Graph sanp value (Default grid offset)

		private int currentSelectedQuadrant;    //Current selected quadrant 
		private int currentCorrectQuadrant;     //Current correct quadrant
		private int currentSelectedAxis;        //Current selected axis
		private int currentCorrectAxis;         //Current correct axis

		private GraphPointScript currentPlottedPoint; //Current plotted point

		private GraphLine currenGraphLine;            //Current plotted graph line
		public GraphQuesType graphQuesType;           //Current graph question type
		 
		private bool canClick;                        //Disable Or Enable Click;

		//Reset old set values
		public void Reset()
		{
			//Destroy all childs
			foreach (Transform child in transform) 
			{
				GameObject.Destroy(child.gameObject);
			}

			axisOffset = new Vector2 (1, 1);
			fontMultiPlier = 1f;
			graphQuesType = GraphQuesType.None;
			currentSelectedQuadrant = 0;
			currentLineParameters = Vector3.zero;
			currentSelectedAxis = -1;
			currentCorrectAxis = -1;
			currentSelectedQuadrant = 0;
			currentCorrectQuadrant = 0;
			this.ShiftPosition(Vector2.zero);
			fixedLinePoints = new Vector2[]{ Vector2.zero, Vector2.zero };
			canClick = true;
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
		}

		//Set snap value
		public void SetSnapValue(float _snapValue)
		{
			this.snapValue = _snapValue;
		}

		//Set graph parameters
		public void SetGraphParameters (Vector2 _axisOffset)
		{
		  axisOffset = _axisOffset;
		}
			
		//Set graph origin
		public void ShiftGraphOrigin(Vector2 offset)
		{
			graphCenterOffset = offset;
			graphOrigin = graphCenter - new Vector2 (Mathf.RoundToInt(offset.x  * gridOffset ),Mathf.RoundToInt(offset.y* gridOffset));
		}

		//Set quetion type
		public void SetGraphQuesType(GraphQuesType _graphQuesType)
		{
			graphQuesType = _graphQuesType;
		}

		//Draw graph and grid according to parameters
		public void DrawGraph()
		{
			DrawGrid ();
			DrawAxis ();
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
		public void DrawAxis()
		{
			GameObject axisParent = new GameObject ();
			axisParent.transform.SetParent (this.transform, false);
			axisParent.name = "axisparent";
			axisObj = GameObject.Instantiate (vectorObjectPrefab);
			axisObj.transform.SetParent (axisParent.transform,false);
			axisObj.name = "axis";
			axisObj.transform.GetComponent<RectTransform> ().anchoredPosition = gridPosition;


			List <Vector2> points = new List <Vector2> ();
			points.Add(new Vector2(0f,graphOrigin.y));
			points.Add(new Vector2(gridCoordinateRange.x * gridOffset,graphOrigin.y));
			points.Add(new Vector2(graphOrigin.x,0f));
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
				if(graphOrigin == Vector2.zero && (cnt == 0 || cnt == 2))
				{
					cnt++;
					continue;
				}
				GenerateLinePoint (new LinePoint("",point + gridPosition, angles[cnt], true,0),axisParent);
				cnt++;
			}

			//Pos X
			int pointInPosXAxis = Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.x/2f + graphCenterOffset.x)));
			graphMaxValue.x = pointInPosXAxis * axisOffset.x;
			for (int i = 1; i < pointInPosXAxis; i++) 
			{
				GenerateLinePoint (new LinePoint ((i * axisOffset.x).ToString(), GraphPosToUIPos (new Vector2 (i * axisOffset.x, 0)), 0f, false,0).SetPointTextOffset(new Vector2(0,-10)),axisParent);
			}

			//Neg X
			int pointInNegXAxis = Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.x/2f - graphCenterOffset.x)));
			graphMinValue.x = -pointInNegXAxis * axisOffset.x;
			for (int i = 1; i < pointInNegXAxis; i++) 
			{
				GenerateLinePoint (new LinePoint ("-"+(i * axisOffset.x).ToString(), GraphPosToUIPos (new Vector2 (-i * axisOffset.x, 0)), 0f, false,0).SetPointTextOffset(new Vector2(0,-10)),axisParent);
			}

			//Pos Y
			int pointInPosYAxis =  Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.y/2 +  graphCenterOffset.y)));
			graphMaxValue.y = pointInPosYAxis * axisOffset.y;
			for (int i = 1; i < pointInPosYAxis; i++) 
			{
				GenerateLinePoint (new LinePoint ((i * axisOffset.y).ToString(), GraphPosToUIPos (new Vector2 (0, i * axisOffset.y)), 0f, false, 0).SetPointTextOffset(new Vector2(-10,0)),axisParent);
			}

			//Neg Y
			int pointInPNegYAxis =   Mathf.Abs((Mathf.RoundToInt(gridCoordinateRange.y/2f - graphCenterOffset.y)));
			graphMinValue.y = -pointInPNegYAxis * axisOffset.y;
			for (int i = 1; i < pointInPNegYAxis; i++) 
			{
				GenerateLinePoint (new LinePoint ("-"+(i * axisOffset.y).ToString(), GraphPosToUIPos (new Vector2 (0, -i * axisOffset.y)), 0f, false, 0).SetPointTextOffset(new Vector2(-10,0)),axisParent);
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
		public GraphPointScript PlotPoint(Vector2 point,string displayText="",bool canDrag =true)
		{
			if (IsContainInGraph (point)) {
				GraphPointScript graphPointScript = GenerateLinePoint (new LinePoint (displayText, GraphPosToUIPos (point), 0f, false,0).SetPointTextOffset(new Vector2(0,-10)));
				graphPointScript.SetPointColor (Color.blue);
				graphPointScript.SetDotSize (2f);
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
			return new Vector2((point.x / axisOffset.x) * gridOffset, (point.y / axisOffset.y) * gridOffset)+ gridPosition + graphOrigin;
		}

		//UI pos to graph pos
		public Vector2 UIPosToGraphPos(Vector2 position)
		{
			return new Vector2 (MathFunctions.GetRounded((position.x - gridPosition.x - graphOrigin.x) * axisOffset.x / gridOffset,1), MathFunctions.GetRounded(((position.y - gridPosition.y - graphOrigin.y) * axisOffset.y / gridOffset),1));
		}

	    //Set font multiplier
		public void SetFontMultiplier(float _fontMultiplier)
		{
			fontMultiPlier = _fontMultiplier;
		}

		//Set snap position
		public Vector2 SetSnapPosition(Vector2 position)
		{
			return new Vector2 (Mathf.RoundToInt (position.x/snapValue) * snapValue, Mathf.RoundToInt (position.y/snapValue) *snapValue);
		}

		//Get snap point from given point
		public Vector2 GetSnapPoint(Vector2 point)
		{
			return new Vector2 (Mathf.RoundToInt (point.x/axisOffset.x) * axisOffset.x, Mathf.RoundToInt (point.y/axisOffset.y) *axisOffset.y);
		}

		//Move plotted point or line point and change line according to point movement
		public void MovePlottedPoint(GraphPointScript graphPointObj,Vector2 position)
		{
			RectTransform pointRectTransform = graphPointObj.GetComponent<RectTransform> ();
			Vector2 oldPos = pointRectTransform.anchoredPosition;
			graphPointObj.transform.position = position;
			pointRectTransform.anchoredPosition = SetSnapPosition(pointRectTransform.anchoredPosition);
			Vector2 pos =pointRectTransform.anchoredPosition;
			Vector2 graphPoint = UIPosToGraphPos (pos);
		
			if(!IsContainInGraph(graphPoint))
			{
				pointRectTransform.anchoredPosition = oldPos;
				return;	
			}

			//if (!string.IsNullOrEmpty (graphPointObj.pointName.text)) {
			//	graphPointObj.pointName.text = "(" + graphPoint.x + "," + graphPoint.y + ")";
			//}
			if (pointLineDisplay == null) 
			{
				pointLineDisplay = GameObject.Instantiate (vectorObjectPrefab);
				pointLineDisplay.transform.SetParent (this.transform);
				pointLineDisplay.transform.localScale = Vector3.one;
				pointLineDisplay.GetComponent<RectTransform> ().anchoredPosition = gridPosition;
				pointLineDisplay.name = "PointDisplayLines";
			}
				
			graphPointObj.linePoint.origin = GraphPosToUIPos (graphPoint);
			if (graphPointObj.lineObj != null)
			{
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

		//Draw line with random points
		public GraphLine DrawRandomLine()
		{
			Vector2 point1 = GetRandomPointInGraph();
			GraphPointScript graphPointScript1 = PlotPoint (point1,"");
			Vector2 point2 = GetRandomPointInGraph();
			int cnt = 0;
			while (Vector2.Distance (GraphPosToUIPos(point1), GraphPosToUIPos(point2)) <15f ||  cnt<=100f) 
			{
				point2 = GetRandomPointInGraph();
				cnt++;
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

			currenGraphLine = graphLine;

			return graphLine;
		}

		public GraphLine DrawLineBetweenPoints(Vector2 point1, Vector2 point2)
		{
			GraphPointScript graphPointScript1 = PlotPoint (point1,"");
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

			return graphLine;
		}

		//Get random point inside graph
		public Vector2 GetRandomPointInGraph()
		{
			Vector2 randomPoint = GetSnapPoint(new Vector2 (Random.Range (graphMinValue.x, graphMaxValue.x), Random.Range (graphMinValue.y, graphMaxValue.y)));

			while (randomPoint.x == 0 || randomPoint.y == 0) 
			{
				randomPoint = GetSnapPoint(new Vector2 (Random.Range (graphMinValue.x, graphMaxValue.x), Random.Range (graphMinValue.y, graphMaxValue.y)));
			}

			return randomPoint;
		}

		//Plot random point in grid
		public void PlotRandomPoint()
		{
			currentPlottedPoint = PlotPoint (GetRandomPointInGraph (), "A", true);
		}

		#region IPointerClickHandler implementation
		//Handle click event for highligh quadrant and highlight axis
		public void OnPointerClick (PointerEventData eventData)
		{
			if (!canClick) {
				return;
			}

			if (touchObj == null)
			{
				touchObj = new GameObject ();
				touchObj.name = "Touch Point";
				touchObj.transform.SetParent (this.transform, false);
				touchObj.AddComponent<RectTransform> ();
			}
			if (graphQuesType == GraphQuesType.HighlightQuadrant) {
				touchObj.transform.position = eventData.position;
				currentSelectedQuadrant = GetQuadrant (UIPosToGraphPos (touchObj.GetComponent<RectTransform> ().anchoredPosition));
				if (highLightedQuadrant == null) {
					highLightedQuadrant = GenerateQuadrantObject ();
				}
				SetQudrantPosition (highLightedQuadrant, currentSelectedQuadrant);
			} else if (graphQuesType == GraphQuesType.HighlightAxis) {
				touchObj.transform.position = eventData.position;
				Vector2 selectedPoint = UIPosToGraphPos (touchObj.GetComponent<RectTransform> ().anchoredPosition);
				if (Mathf.Abs (selectedPoint.x / axisOffset.x) <= 2 || Mathf.Abs (selectedPoint.y / axisOffset.y) <= 2) {
					currentSelectedAxis = (Mathf.Abs (selectedPoint.x / axisOffset.x) > Mathf.Abs (selectedPoint.y / axisOffset.y)) ? 0 : 1;
					axisObj.GetComponent<VectorObject2D> ().vectorLine.SetColor (Color.blue, currentSelectedAxis);
					axisObj.GetComponent<VectorObject2D> ().vectorLine.SetColor (Color.black, 1 - currentSelectedAxis);
				}
			
			} else if (graphQuesType == GraphQuesType.PlotPoint)
			{
				if (currentPlottedPoint == null) {
					return;
				}
				
				RectTransform pointRectTransform = currentPlottedPoint.GetComponent<RectTransform> ();
				Vector2 oldPos = pointRectTransform.anchoredPosition;
				currentPlottedPoint.transform.position = eventData.position;
				pointRectTransform.anchoredPosition = SetSnapPosition(pointRectTransform.anchoredPosition);
				Vector2 pos =pointRectTransform.anchoredPosition;
				Vector2 graphPoint = UIPosToGraphPos (pos);
				if(!IsContainInGraph(graphPoint))
				{
					pointRectTransform.anchoredPosition = oldPos;
					return;	
				}
			}
		}

		#endregion

		//Generate quadrant object 
		public GameObject GenerateQuadrantObject()
		{
			GameObject quadrantObj = new GameObject ();
			quadrantObj.name = "Quadrant";
			quadrantObj.transform.SetParent (this.transform, false);
			quadrantObj.AddComponent<Image> ();
			quadrantObj.GetComponent<Image> ().color = new Color (0f, 0f, 1f, 0.5f);

			return quadrantObj;
		}

		//Set quadrant position
		public void SetQudrantPosition(GameObject quadrantObj, int quadrant)
		{
			Vector2 quadrantSize = GetQuadrantSize (quadrant);
			quadrantObj.GetComponent<RectTransform> ().anchoredPosition = gridPosition +  graphOrigin + new Vector2(quadrantSize.x/2f,quadrantSize.y/2f) ;
			quadrantObj.GetComponent<RectTransform> ().sizeDelta = new Vector2 (Mathf.Abs (quadrantSize.x), Mathf.Abs (quadrantSize.y));
		}

		//Get quadrant size
		public Vector2 GetQuadrantSize(int quadrant)
		{
			Vector2 size = Vector2.zero ;

			switch (quadrant)
			{
			case 1:
				size = new Vector2( Mathf.RoundToInt(gridCoordinateRange.x/2f + graphCenterOffset.x) ,Mathf.RoundToInt(gridCoordinateRange.y/2 +  graphCenterOffset.y)) * gridOffset;
				break;

			case 2:
				size =  new Vector2(-Mathf.RoundToInt(gridCoordinateRange.x/2f - graphCenterOffset.x) ,Mathf.RoundToInt(gridCoordinateRange.y/2 +  graphCenterOffset.y)) * gridOffset;
				break;

			case 3:
				size =  new Vector2(-Mathf.RoundToInt(gridCoordinateRange.x/2f - graphCenterOffset.x) ,-Mathf.RoundToInt(gridCoordinateRange.y/2f - graphCenterOffset.y)) * gridOffset;
				break;

			case 4:
				size =  new Vector2(  Mathf.RoundToInt(gridCoordinateRange.x/2f + graphCenterOffset.x) ,-Mathf.RoundToInt(gridCoordinateRange.y/2f - graphCenterOffset.y)) * gridOffset;
				break;
			}
			return size;
		}


		//Get quadrant in which given point lies
		public int GetQuadrant(Vector2 point)
		{
			if (point.x >= 0 && point.x <= graphMaxValue.x && point.y >= 0 && point.y <= graphMaxValue.y)
			{
				return 1;
			} 
			else if (point.x <= 0 && point.x >= graphMinValue.x && point.y >= 0 && point.y <= graphMaxValue.y) 
			{
				return 2;
			}
			else if (point.x <= 0 && point.x >= graphMinValue.x && point.y <= 0 && point.y >= graphMinValue.y) 
			{
				return 3;
			}
			else if (point.x >= 0 && point.x <= graphMaxValue.x  && point.y <= 0 && point.y >= graphMinValue.y) 
			{
				return 4;
			}

			return 0;
		}

		//Check answer according to graph question type
		public bool CheckAnswer()
		{
			bool correct = false;
			canClick = false;
			switch (graphQuesType)
			{
				case GraphQuesType.HighlightQuadrant:
					correct = (currentSelectedQuadrant == currentCorrectQuadrant);
					break;

				case GraphQuesType.HighlightAxis:
					correct = (currentCorrectAxis == currentSelectedAxis);
					break;

				case GraphQuesType.PlotPoint:
					Vector2 graphPoint = UIPosToGraphPos (currentPlottedPoint.linePoint.origin);
					correct = (correctPlottedPoint.x == graphPoint.x && correctPlottedPoint.y == graphPoint.y);
					break;

				case GraphQuesType.PlotLine:
					correct = MathFunctions.IsValidLinePoint (currentLineParameters, UIPosToGraphPos (currenGraphLine.point1.linePoint.origin)) && MathFunctions.IsValidLinePoint (currentLineParameters, UIPosToGraphPos (currenGraphLine.point2.linePoint.origin));
					break;

				case GraphQuesType.PlotFixedLine:
					correct = (fixedLinePoints [0] == UIPosToGraphPos (currenGraphLine.point1.linePoint.origin) && fixedLinePoints [1] == UIPosToGraphPos (currenGraphLine.point2.linePoint.origin)) || (fixedLinePoints [1] == UIPosToGraphPos (currenGraphLine.point1.linePoint.origin) && fixedLinePoints [0] == UIPosToGraphPos (currenGraphLine.point2.linePoint.origin));
					break;
			}
			return correct;
		}

		//Handle correct answer according to graph question type
		public void HandleCorrectAnswer()
		{
			switch (graphQuesType)
			{
				case GraphQuesType.HighlightQuadrant:
					if (highLightedQuadrant) {
						highLightedQuadrant.GetComponent<Image> ().color = new Color (MaterialColor.green800.r, MaterialColor.green800.g, MaterialColor.green800.b, 0.5f);
					}
					break;

				case GraphQuesType.HighlightAxis:
						axisObj.GetComponent<VectorObject2D> ().vectorLine.SetColor (MaterialColor.green800, currentSelectedAxis);
					break;

			   case GraphQuesType.PlotPoint:
					currentPlottedPoint.dot.color = MaterialColor.green800;
					break;

				case GraphQuesType.PlotLine:
					RemoveDragEventInGraphPoint (currenGraphLine.point1);
					RemoveDragEventInGraphPoint (currenGraphLine.point2);
					currenGraphLine.vectorLine.color = MaterialColor.green800;
					break;

				case GraphQuesType.PlotFixedLine:
					RemoveDragEventInGraphPoint (currenGraphLine.point1);
					RemoveDragEventInGraphPoint (currenGraphLine.point2);
					currenGraphLine.vectorLine.color = MaterialColor.green800;
					break;
			}
		}

		//Handle incorrect answer according to graph question type
		public void HandleIncorrectAnwer()
		{
			switch (graphQuesType)
			{
				case GraphQuesType.HighlightQuadrant:
					GameObject correctQuadrant = GenerateQuadrantObject ();
					SetQudrantPosition (correctQuadrant, currentCorrectQuadrant);
					correctQuadrant.GetComponent<Image> ().color = new Color (MaterialColor.green800.r, MaterialColor.green800.g, MaterialColor.green800.b, 0.5f);
					if (highLightedQuadrant) {
						highLightedQuadrant.GetComponent<Image> ().color = new Color (MaterialColor.red800.r, MaterialColor.red800.g, MaterialColor.red800.b, 0.5f);
					}
					break;

				case GraphQuesType.HighlightAxis:
					axisObj.GetComponent<VectorObject2D> ().vectorLine.SetColor (MaterialColor.red800, currentSelectedAxis);
					axisObj.GetComponent<VectorObject2D> ().vectorLine.SetColor (MaterialColor.green800, currentCorrectAxis);
					break;

				case GraphQuesType.PlotPoint:
					RemoveDragEventInGraphPoint (currentPlottedPoint);
					currentPlottedPoint.dot.color = MaterialColor.red800;
					GraphPointScript plot = PlotPoint (correctPlottedPoint, currentPlottedPoint.linePoint.name, false);
					plot.dot.color = MaterialColor.green800;
					break;

				case GraphQuesType.PlotLine:
					RemoveDragEventInGraphPoint (currenGraphLine.point1);
					RemoveDragEventInGraphPoint (currenGraphLine.point2);
					currenGraphLine.vectorLine.color = MaterialColor.red800;
					GraphLine correctLine = DrawLineBetweenPoints (MathFunctions.GetValidLinePoint (currentLineParameters, Vector2.zero),MathFunctions.GetValidLinePoint (currentLineParameters, Vector2.one));
					correctLine.vectorLine.color = MaterialColor.green800;
					break;

				case GraphQuesType.PlotFixedLine:
					RemoveDragEventInGraphPoint (currenGraphLine.point1);
					RemoveDragEventInGraphPoint (currenGraphLine.point2);
					currenGraphLine.vectorLine.color = MaterialColor.red800;
					GraphLine correctLine1 = DrawLineBetweenPoints (fixedLinePoints [0],fixedLinePoints [1]);
					correctLine1.vectorLine.color = MaterialColor.green800;
					break;
			}
		}

	
		//Remove drag event to stop dragging
		public void RemoveDragEventInGraphPoint(GraphPointScript graphPoint)
		{
			graphPoint.onDragEvent = null;
			graphPoint.onDragEndEvent = null;
		}

		//Set correct plotted point
		public void SetCorrectPlottedPoint(Vector2 _correctPlottedPoint)
		{
			correctPlottedPoint = _correctPlottedPoint;
		}

		//Set current correct quadrant
		public void SetCurrentCorrectQuadrant(int _currentCorrectQuadrant)
		{
			currentCorrectQuadrant = _currentCorrectQuadrant;
		}

		//Shift graph position
		public void ShiftPosition (Vector2 position)
		{
			this.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -30f) + position;
		}

		//Set current correct axis
		public void SetCurrentCorrectAxis(int _currentCorrectAxis)
		{
			currentCorrectAxis = _currentCorrectAxis;
		}

		//Set current line parameters
		public void SetCurrentLineParameters(Vector3 _currentLineParameters)
		{
			currentLineParameters = _currentLineParameters;
		}

		//Set fixed line points
		public void SetFixedLinePoints(Vector2[] _fixedLinePoints)
		{
			fixedLinePoints = _fixedLinePoints;
		}
	}
}