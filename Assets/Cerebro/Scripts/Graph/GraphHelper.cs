using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vectrosity;
using MaterialUI;
using UnityEngine.UI.Extensions;


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
		RotateDiagram,
		PointLiesOnLine,
		DiagramImage,
		None

	}
	public class GraphHelper : MonoBehaviour,IPointerClickHandler
	{
		public GameObject vectorObjectPrefab; //Vector line prefab to draw line
		public GameObject vectorDottedObjectPrefab; //Vector line prefab to draw dotted line
		public GameObject linePointPrefab;   // point prefab to render point 
		public GameObject arcPrefab;

		public Sprite arrowSprite;

		private GameObject pointLineDisplay;   //Display line when change plotted point position
		private GameObject highLightedQuadrant; // Highlight selected quadrant
		private GameObject touchObj;            // Handle touch position
		private GameObject axisObj;             // Axis line object
		private GameObject diagramParentObj;    //Diagram Parent

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
		private List<Vector2> currentDiagramPoints;

		private List<Vector3> currentLineParameters;  //Line paramter to check line equation (x = a, y = b, z = c) ax+by+c=0

		private float gridOffset;               //Grid offset
		private float fontMultiPlier;           //Font multiplier to make graph font smaller or bigger
		private float snapValue;                //Graph sanp value (Default grid offset)

		private int currentSelectedQuadrant;    //Current selected quadrant 
		private int currentCorrectQuadrant;     //Current correct quadrant
		private int currentSelectedAxis;        //Current selected axis
		private int currentCorrectAxis;         //Current correct axis
		private float currentRotationAngle;

		private GraphPointScript currentPlottedPoint; //Current plotted point

		private GraphLine currentGraphLine;            //Current plotted graph line
		public GraphQuesType graphQuesType;           //Current graph question type
		private GraphDiagram currentGraphDiagram;

		private bool canClick;                        //Disable Or Enable Click;



		//Reset old set values
		public void Reset()
		{
			//Destroy all childs
			foreach (Transform child in transform) 
			{
				GameObject.Destroy(child.gameObject);
			}
			diagramParentObj = null;
			graphCenterOffset = Vector2.zero;
			axisOffset = new Vector2 (1, 1);
			fontMultiPlier = 1f;
			graphQuesType = GraphQuesType.None;
			currentSelectedQuadrant = -1;
			currentLineParameters = new List<Vector3>();
			currentDiagramPoints = new List<Vector2> ();
			currentSelectedAxis = -1;
			currentCorrectAxis = -1;
			currentRotationAngle = 0f;
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
			graphCenterOffset = -offset;
			graphOrigin = graphCenter - new Vector2 (Mathf.RoundToInt(graphCenterOffset.x  * gridOffset ),Mathf.RoundToInt(graphCenterOffset.y* gridOffset));
		}

		//Set quetion type
		public void SetGraphQuesType(GraphQuesType _graphQuesType)
		{
			graphQuesType = _graphQuesType;
		}

		//Draw graph and grid according to parameters
		public void DrawGraph(bool showAxis = true)
		{
			DrawGrid ();
			DrawAxis (showAxis);
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
			axisParent.gameObject.SetActive (showAxis);
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
		public Vector2 GetSnapPosition(Vector2 position)
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
			pointRectTransform.anchoredPosition = GetSnapPosition(pointRectTransform.anchoredPosition);
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
			if (graphPointObj.lineObj != null && graphPointObj.lineObj.IsLineVisible())
			{
				if (graphPointObj.lineObj.IsLineSegment ()) 
				{
					graphPointObj.lineObj.Draw ();
				} else {
					Vector2[] maxBounds = GetMaxBoundPoints (new Vector2[] {
						UIPosToGraphPos (graphPointObj.lineObj.point1.linePoint.origin),
						UIPosToGraphPos (graphPointObj.lineObj.point2.linePoint.origin)
					});
					graphPointObj.lineObj.Draw (maxBounds);
				}
			}

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



		//Draw line with random points
	    public GraphLine DrawRandomLine(bool isLineVisible = true,bool isLineSegment = false,LineShapeType lineShapeType = LineShapeType.Normal)
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

			GameObject lineObj = GameObject.Instantiate (lineShapeType == LineShapeType.Normal ? vectorObjectPrefab : vectorDottedObjectPrefab);
			lineObj.transform.SetParent (this.transform);
			lineObj.transform.localScale = Vector3.one;
			lineObj.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

			VectorLine vectorLine = lineObj.GetComponent<VectorObject2D> ().vectorLine;
			GraphLine graphLine = new GraphLine (vectorLine, graphPointScript1, graphPointScript2,arrowSprite,isLineSegment,isLineVisible,lineShapeType);

			if (isLineSegment || !isLineVisible)
			{ 
				graphLine.Draw ();
			} else {
				Vector2[] maxBounds = GetMaxBoundPoints (new Vector2[]{point1,point2});
				graphLine.Draw (maxBounds);
			}

			graphPointScript1.SetLineObject (graphLine);
			graphPointScript2.SetLineObject (graphLine);

			currentGraphLine = graphLine;

			return graphLine;
		}

		public GraphLine DrawLineBetweenPoints(Vector2 point1, Vector2 point2,bool isLineVisible = true,bool isLineSegment =false,LineShapeType lineShapeType = LineShapeType.Normal,bool canDrag = true)
		{
			GraphPointScript graphPointScript1 = PlotPoint (point1,"",canDrag);
			GraphPointScript graphPointScript2 = PlotPoint (point2,"",canDrag);
			GameObject lineObj = GameObject.Instantiate (lineShapeType == LineShapeType.Normal ? vectorObjectPrefab : vectorDottedObjectPrefab);
			lineObj.transform.SetParent (this.transform);
			lineObj.transform.localScale = Vector3.one;
			lineObj.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

			VectorLine vectorLine = lineObj.GetComponent<VectorObject2D> ().vectorLine;
			GraphLine graphLine = new GraphLine (vectorLine, graphPointScript1, graphPointScript2,arrowSprite,isLineSegment,isLineVisible,lineShapeType);
			if (isLineSegment || !isLineVisible)
			{
				graphLine.Draw ();
			} else {
				Vector2[] maxBounds = GetMaxBoundPoints (new Vector2[]{point1,point2});
				graphLine.Draw (maxBounds);
			}

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
		public void PlotRandomPoint(string text ="A")
		{
			currentPlottedPoint = PlotPoint (GetRandomPointInGraph (), text, true);
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
				pointRectTransform.anchoredPosition = GetSnapPosition(pointRectTransform.anchoredPosition);
				Vector2 pos =pointRectTransform.anchoredPosition;
				Vector2 graphPoint = UIPosToGraphPos (pos);
				if(!IsContainInGraph(graphPoint))
				{
					pointRectTransform.anchoredPosition = oldPos;
					return;	
				}
				currentPlottedPoint.SetIsValueChanged (true);
				currentPlottedPoint.linePoint.origin = GraphPosToUIPos (graphPoint);
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

		//Check is userAnsweredQuetion
		public bool IsAnswered()
		{
			bool isAnswered = true;
			switch (graphQuesType)
			{
			case GraphQuesType.HighlightQuadrant:
				isAnswered = (currentSelectedQuadrant > -1);
				break;

			case GraphQuesType.HighlightAxis:
				isAnswered = (currentSelectedAxis > -1);
				break;

			case GraphQuesType.PlotPoint:
				isAnswered = currentPlottedPoint.IsValueChanged ();
				break;

			case GraphQuesType.PlotLine:
			case GraphQuesType.PlotFixedLine:
			case GraphQuesType.PointLiesOnLine:
				isAnswered = (currentGraphLine.point1.IsValueChanged () || currentGraphLine.point2.IsValueChanged ());
				break;

			case GraphQuesType.DiagramImage:
				isAnswered = false;
				foreach (GraphPointScript graphPoint in currentGraphDiagram.graphPoints) {
					if (graphPoint.IsValueChanged ()) {
						isAnswered = true;
					}
				}
				break;

			case GraphQuesType.RotateDiagram:
				isAnswered = diagramParentObj != null && diagramParentObj.transform.localEulerAngles.z > 0f;
				break;

			}

			return isAnswered;
		}

		//Check answer according to graph question type
		public bool CheckAnswer()
		{
			bool correct = false;
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
				correct = IsValidCurrenLinePoint(new Vector2[]{UIPosToGraphPos (currentGraphLine.point1.linePoint.origin),UIPosToGraphPos (currentGraphLine.point2.linePoint.origin)});
				break;

			case GraphQuesType.PlotFixedLine:
				correct = (fixedLinePoints [0] == UIPosToGraphPos (currentGraphLine.point1.linePoint.origin) && fixedLinePoints [1] == UIPosToGraphPos (currentGraphLine.point2.linePoint.origin)) || (fixedLinePoints [1] == UIPosToGraphPos (currentGraphLine.point1.linePoint.origin) && fixedLinePoints [0] == UIPosToGraphPos (currentGraphLine.point2.linePoint.origin));
				break;

			case GraphQuesType.PointLiesOnLine:
				correct = IsPointOnLine (new Vector2[] {
					UIPosToGraphPos (currentGraphLine.point1.linePoint.origin),
					UIPosToGraphPos (currentGraphLine.point2.linePoint.origin)
				});
				break;

			case GraphQuesType.DiagramImage:
				correct = IsValidImageOfCurrentDiagram ();
				break;

			case GraphQuesType.RotateDiagram:
				correct = diagramParentObj!=null && diagramParentObj.transform.localEulerAngles.z > currentRotationAngle - 3f && diagramParentObj.transform.localEulerAngles.z < currentRotationAngle + 3f; 
				break;
			}
			return correct;
		}

		public bool IsValidImageOfCurrentDiagram()
		{
			List<Vector2> currentPlottedPoints = currentGraphDiagram.GetPointList ();
			List<Vector2> currentCorrectPoints = new List<Vector2>();

			foreach (Vector2 point in currentDiagramPoints) {
				currentCorrectPoints.Add (GraphPosToUIPos (point));
			}

			foreach (Vector2 point in currentPlottedPoints)
			{
				currentCorrectPoints.RemoveAll (x => x == point);
			}

			return currentCorrectPoints.Count<=0;
		}

		public bool IsValidCurrenLinePoint(Vector2[] points)
		{
			if (points.Length < 2) {
				return false;
			}
			foreach (Vector3 lineParameter in currentLineParameters) {
				if (MathFunctions.IsValidLinePoint (lineParameter, points[0]) &&  MathFunctions.IsValidLinePoint (lineParameter, points[1])) {
					return true;
				}
			}

			return false;
		}

		public bool IsPointOnLine(Vector2[] points)
		{
			if (points.Length < 2) {
				return false;
			}
			Vector3 lineParameters = MathFunctions.GetLineParamters (points [0], points [1]);

			return MathFunctions.IsValidLinePoint(lineParameters,correctPlottedPoint);
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
			case GraphQuesType.PlotFixedLine:
			case GraphQuesType.PointLiesOnLine:
				RemoveDragEventInGraphPoint (currentGraphLine.point1);
				RemoveDragEventInGraphPoint (currentGraphLine.point2);
				if (currentGraphLine.IsLineVisible ()) {
					currentGraphLine.vectorLine.color = MaterialColor.green800;
				} else {
					currentGraphLine.point1.dot.color = MaterialColor.green800;
					currentGraphLine.point2.dot.color = MaterialColor.green800;
				}
				break;

			case GraphQuesType.DiagramImage:
				foreach (GraphPointScript graphPoint in currentGraphDiagram.graphPoints) {
					RemoveDragEventInGraphPoint (graphPoint);
				}
				currentGraphDiagram.vectorLine.color = MaterialColor.green800;
				break;

			case GraphQuesType.RotateDiagram:
				if (diagramParentObj) 
				{
					diagramParentObj.GetComponent<GraphDiagramRotator> ().canRotate = false;
					SetDiagramColor (diagramParentObj, MaterialColor.green800);
				}
				break;
			}
		}

		public void SetDiagramColor(GameObject diagramParent, Color color)
		{
			foreach (UIPolygon UIpolygon in diagramParent.GetComponentsInChildren<UIPolygon>()) {
				UIpolygon.color = color;
				UIpolygon.ReDraw ();
			}

			foreach (VectorObject2D vectorObject in diagramParent.GetComponentsInChildren<VectorObject2D>()) {
				vectorObject.vectorLine.color = color;
				vectorObject.vectorLine.Draw ();
			}
		}

		//Handle incorrect answer according to graph question type
		public void HandleIncorrectAnwer(bool isRevisited = false)
		{
			if (!isRevisited) {
				canClick = false;
			}
			switch (graphQuesType)
			{
			case GraphQuesType.HighlightQuadrant:
				if (!isRevisited) {
					GameObject correctQuadrant = GenerateQuadrantObject ();
					SetQudrantPosition (correctQuadrant, currentCorrectQuadrant);
					correctQuadrant.GetComponent<Image> ().color = new Color (MaterialColor.green800.r, MaterialColor.green800.g, MaterialColor.green800.b, 0.5f);
				}
				if (highLightedQuadrant) {
					highLightedQuadrant.GetComponent<Image> ().color = new Color (MaterialColor.red800.r, MaterialColor.red800.g, MaterialColor.red800.b, 0.5f);
				}
				break;

			case GraphQuesType.HighlightAxis:
				axisObj.GetComponent<VectorObject2D> ().vectorLine.SetColor (MaterialColor.red800, currentSelectedAxis);
				if (!isRevisited) {
					axisObj.GetComponent<VectorObject2D> ().vectorLine.SetColor (MaterialColor.green800, currentCorrectAxis);
				}
				break;

			case GraphQuesType.PlotPoint:
				currentPlottedPoint.dot.color = MaterialColor.red800;
				if (!isRevisited) {
					RemoveDragEventInGraphPoint (currentPlottedPoint);
					GraphPointScript plot = PlotPoint (correctPlottedPoint, currentPlottedPoint.linePoint.name, false);
					plot.dot.color = MaterialColor.green800;
				}
				break;

			case GraphQuesType.PlotLine:
			case GraphQuesType.PlotFixedLine:
			case GraphQuesType.PointLiesOnLine:
				if (currentGraphLine.IsLineVisible ()) {
					currentGraphLine.vectorLine.color = MaterialColor.red800;
				} else {
					currentGraphLine.point1.dot.color = MaterialColor.red800;
					currentGraphLine.point2.dot.color = MaterialColor.red800;
				}
				if (!isRevisited) {
					RemoveDragEventInGraphPoint (currentGraphLine.point1);
					RemoveDragEventInGraphPoint (currentGraphLine.point2);
					Vector2[] maxBoundPoints = GetMaxBoundPointsOnCurrentLine ();
					GraphLine correctLine = DrawLineBetweenPoints (maxBoundPoints [0], maxBoundPoints [1],true,currentGraphLine.IsLineSegment(),currentGraphLine.LineShapeType(),false);
					correctLine.vectorLine.color = MaterialColor.green800;

				}
				break;

			case GraphQuesType.DiagramImage:
				currentGraphDiagram.vectorLine.color = MaterialColor.red800;
				currentGraphDiagram.vectorLine.Draw ();
				if (!isRevisited) {
					
					foreach (GraphPointScript graphPoint in currentGraphDiagram.graphPoints) {
						RemoveDragEventInGraphPoint (graphPoint);
					}
					DrawDiagram (currentDiagramPoints, MaterialColor.green800, currentGraphDiagram.LineShapeType ());
				}
				break;

			case GraphQuesType.RotateDiagram:
				if (diagramParentObj != null) {
					SetDiagramColor (diagramParentObj, MaterialColor.red800);
					if (!isRevisited) {
						diagramParentObj.GetComponent<GraphDiagramRotator> ().canRotate = false;
						GameObject correctDiagram = GameObject.Instantiate (diagramParentObj);
						correctDiagram.transform.SetParent (this.transform, false);
						correctDiagram.transform.eulerAngles = new Vector3 (0f, 0f, currentRotationAngle);
						SetDiagramColor (correctDiagram, MaterialColor.green800);
					}
				}
				break;
			}
		}


		public void ResetAnswer()
		{
			switch (graphQuesType)
			{
			case GraphQuesType.HighlightQuadrant:
				if (highLightedQuadrant) {
					Destroy (highLightedQuadrant);
				}
				currentSelectedQuadrant = -1;
				break;

			case GraphQuesType.HighlightAxis:
				axisObj.GetComponent<VectorObject2D> ().vectorLine.SetColor (Color.black, currentSelectedAxis);
				currentSelectedAxis = -1;
				break;

			case GraphQuesType.PlotPoint:
				currentPlottedPoint.dot.color = Color.black;
				currentPlottedPoint.SetIsValueChanged (false);
				break;

			case GraphQuesType.PlotLine:
			case GraphQuesType.PlotFixedLine:
			case GraphQuesType.PointLiesOnLine:
				currentGraphLine.vectorLine.color = Color.black;
				currentGraphLine.point1.SetIsValueChanged (false);
				currentGraphLine.point2.SetIsValueChanged (false);
				break;

			case GraphQuesType.DiagramImage:
				currentGraphDiagram.vectorLine.color = Color.black;
				foreach (GraphPointScript graphPoint in currentGraphDiagram.graphPoints) {
					graphPoint.SetIsValueChanged (false);
				}
				break;

			case GraphQuesType.RotateDiagram:
				if (diagramParentObj != null) {
					SetDiagramColor (diagramParentObj, Color.black);
					diagramParentObj.GetComponent<GraphDiagramRotator> ().canRotate = true;
				}
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

		//Set current rotation angle
		public void SetCurrentRotationAngle(float _currentRotationAngle)
		{
			currentRotationAngle = _currentRotationAngle;
		}

		//Set current line parameters
		public void SetCurrentLineParameters(List<Vector3> _currentLineParameters)
		{
			currentLineParameters = _currentLineParameters;
		}

		//Set fixed line points
		public void SetFixedLinePoints(Vector2[] _fixedLinePoints)
		{
			fixedLinePoints = _fixedLinePoints;
		}

		//Set current diagram points
		public void SetCurrentDiagramPoints(List<Vector2>_currentDiagramPoints)
		{
			currentDiagramPoints = _currentDiagramPoints;
		}

		public Vector2[] GetMaxBoundPointsOnCurrentLine()
		{
			Vector2[] newPoints = new Vector2[2];
			switch (graphQuesType)
			{
			case GraphQuesType.PlotFixedLine:
				newPoints = fixedLinePoints;
				break;

			case GraphQuesType.PlotLine:
			case GraphQuesType.PointLiesOnLine:
				int randomLineNumber = Random.Range (0, currentLineParameters.Count);
				Vector2[] points = new Vector2[4];
				points [0] = new Vector2 (MathFunctions.GetPointX (currentLineParameters[randomLineNumber], graphMinValue.y), graphMinValue.y);
				points [1] = new Vector2 (MathFunctions.GetPointX (currentLineParameters[randomLineNumber], graphMaxValue.y), graphMaxValue.y);
				points [2] = new Vector2 (graphMaxValue.x, MathFunctions.GetPointY (currentLineParameters[randomLineNumber], graphMaxValue.x));
				points [3] = new Vector2 (graphMinValue.x, MathFunctions.GetPointY (currentLineParameters[randomLineNumber], graphMinValue.x));
				int cnt = 0;
				for (int i = 0; i < points.Length; i++) {
					if (IsContainInGraph (points [i]) && cnt < 2 && !System.Array.Exists (newPoints, x => x == points [i]) && !(currentLineParameters[randomLineNumber].x == 0 && points [i].x == 0) && !(currentLineParameters[randomLineNumber].y == 0 && points [i].y == 0)) {
						newPoints [cnt] = points [i];
						cnt++;
					}
				}
				break;
			}

			return newPoints;
		}

		public Vector2[] GetMaxBoundPoints(Vector2[] plottedPoint)
		{
			Vector2[] points = new Vector2[4];


			Vector3 lineParameters = MathFunctions.GetLineParamters (plottedPoint [0], plottedPoint [1]);

			points [0] = new Vector2 (MathFunctions.GetPointX (lineParameters, graphMinValue.y), graphMinValue.y);
			points [1] = new Vector2 (MathFunctions.GetPointX (lineParameters, graphMaxValue.y), graphMaxValue.y);
			points [2] = new Vector2 (graphMaxValue.x, MathFunctions.GetPointY (lineParameters, graphMaxValue.x));
			points [3] = new Vector2 (graphMinValue.x, MathFunctions.GetPointY (lineParameters, graphMinValue.x));

			Vector2[] newPoints = new Vector2[2];
			int cnt = 0;
			for (int i = 0; i < points.Length; i++) {
				if (IsContainInGraph (points [i]) && cnt<2  && !System.Array.Exists(newPoints,x=>x==points[i]) && !(lineParameters.x == 0 && points[i].x ==0) && !(lineParameters.y == 0 && points[i].y ==0)) {
					newPoints [cnt] = points [i];
					cnt++;
				}
			}

			if (newPoints [1].x >newPoints[0].x ) {
				Vector2 temp = newPoints [0];
				newPoints [0] = newPoints [1];
				newPoints [1] = temp;
			}
			newPoints [0] = GraphPosToUIPos (newPoints [0]);
			newPoints [1] = GraphPosToUIPos (newPoints [1]);
			return newPoints;
		}

		public void DrawDiagram(List<Vector2>  graphPoints, LineShapeType lineShapeType = LineShapeType.Normal,float width = 2,LineType lineType = LineType.Continuous)
		{
			DrawDiagram (graphPoints, Color.black, lineShapeType, width, lineType);
		}

		public void DrawDiagram(List<Vector2>  graphPoints,Color color, LineShapeType lineShapeType = LineShapeType.Normal,float width = 2,LineType lineType = LineType.Continuous)
		{
			GenerateDiagramParent ();
			GameObject diagramObj = GameObject.Instantiate (lineShapeType == LineShapeType.Normal? vectorObjectPrefab : vectorDottedObjectPrefab);
			diagramObj.transform.SetParent (diagramParentObj.transform,false);
			diagramObj.name = "diagram";

			List<Vector2> UIPoints = new List<Vector2> ();

			foreach (Vector2 graphPoint in graphPoints) {
				UIPoints.Add (GraphPosToUIPos (graphPoint));
			}

			VectorLine vectorLine = diagramObj.GetComponent<VectorObject2D> ().vectorLine;
			vectorLine.points2 = UIPoints;
			vectorLine.color = color;
			vectorLine.lineType = lineType;
			vectorLine.SetWidth (lineShapeType == LineShapeType.Dotted ?width *4f : width);
			vectorLine.Draw ();


		}


		public void DrawMovebleDiagram(List<Vector2>  graphPoints,LineShapeType lineShapeType = LineShapeType.Normal,float width =2f, LineType lineType = LineType.Continuous)
		{

			GenerateDiagramParent ();
			GameObject diagramObj = GameObject.Instantiate (lineShapeType == LineShapeType.Normal? vectorObjectPrefab : vectorDottedObjectPrefab);
			diagramObj.transform.SetParent (diagramParentObj.transform,false);
			diagramObj.name = "diagram";

			VectorLine vectorLine = diagramObj.GetComponent<VectorObject2D> ().vectorLine;
			vectorLine.SetWidth (width);
			vectorLine.color = Color.black;
			vectorLine.lineType = lineType;
			vectorLine.SetWidth (lineShapeType == LineShapeType.Dotted ?width *4f : width);
			currentGraphDiagram = new GraphDiagram (vectorLine,lineShapeType);

			if (graphPoints.Count < 2) {
				return;
			}

			currentGraphDiagram.SetIsCloseDiagram (graphPoints[0].Equals(graphPoints [graphPoints.Count - 1]));
			int cnt = 0;
			foreach (Vector2 graphPoint in graphPoints) 
			{
				if (cnt == graphPoints.Count-1 && graphPoints[0].Equals(graphPoints [graphPoints.Count - 1])) {
					continue;
				}
				GraphPointScript graphPointScript = PlotPoint (graphPoint,"");
				graphPointScript.SetDigramObject (currentGraphDiagram);
				currentGraphDiagram.AddGraphPoint (graphPointScript);
				cnt++;
			}

			currentGraphDiagram.Draw ();
		}


		public void DrawArc(Vector2 center,Vector2 point1,Vector2 point2)
		{
			GenerateDiagramParent ();
			center = GraphPosToUIPos(center);
			point1 = GraphPosToUIPos(point1);
			point2 = GraphPosToUIPos(point2);

			if(Mathf.Abs (Vector2.Distance(center,point1) - Vector2.Distance(center,point2)) >= 0.0001f)
			{
				Debug.Log("Points " + point1+" and "+point2 +" are not in same arc");
				return;
			}
			GameObject arcObj = GameObject.Instantiate (arcPrefab);
			arcObj.transform.SetParent (diagramParentObj.transform,false);
			arcObj.name = "arc";

			UIPolygon uiPolygon = arcObj.GetComponent<UIPolygon> ();
			arcObj.GetComponent<RectTransform> ().anchoredPosition =  center;
			arcObj.GetComponent<RectTransform> ().sizeDelta =  Vector2.one * Vector2.Distance(center,point1) *2f;

			float endAngle, startAngle;

			if (point1.x == center.x) {
				if (point1.y > center.y) {
					startAngle = 90f;
				} else {
					startAngle = 270f;
				}
			}
			else
				startAngle =  Mathf.Atan ((point1.y - center.y) / (point1.x - center.x)) * Mathf.Rad2Deg;

			if (startAngle < 0)
				startAngle += 360;

			if (point2.x == center.x) {
				if (point2.y > center.y) {
					endAngle = 90f;
				} else {
					endAngle = 270f;
				}
			}
			else
				endAngle = (Mathf.Atan ((point2.y - center.y) / (point2.x - center.x)) * Mathf.Rad2Deg);

			if (endAngle < 0)
				endAngle += 360;

			if (point1.x < center.x) {
				startAngle = 180 + startAngle;
			}
			if (point2.x < center.x) {
				endAngle = 180 + endAngle;
			}

			float diff = 0f; 
			if (startAngle > endAngle) {
				endAngle = 360 + endAngle;
				diff = endAngle - startAngle;
			} else {
				diff =  endAngle - startAngle;
			}

			uiPolygon.fillPercent = Mathf.CeilToInt (100f * (diff) / 360f);
			uiPolygon.rotation = startAngle+180f;
			uiPolygon.ReDraw ();
		}

		public void DrawArc(Vector2 center,float radius,float startAngle,float endAngle)
		{
			GenerateDiagramParent ();
			GameObject arcObj = GameObject.Instantiate (arcPrefab);
			arcObj.transform.SetParent (diagramParentObj.transform,false);
			arcObj.name = "arc";
			center = GraphPosToUIPos(center);
			UIPolygon uiPolygon = arcObj.GetComponent<UIPolygon> ();
			arcObj.GetComponent<RectTransform> ().anchoredPosition =  center;
			arcObj.GetComponent<RectTransform> ().sizeDelta =  Vector2.one * radius * gridOffset *2f;

			float diff = 0 ;
			if (startAngle > endAngle) {
				endAngle = 360 - endAngle;
				diff = endAngle - startAngle;
			} else {
				diff =  endAngle - startAngle;
			}

			uiPolygon.fillPercent = Mathf.CeilToInt (100f * (diff) / 360f);
			uiPolygon.rotation = startAngle+180f;
			uiPolygon.ReDraw ();
		}

		public void GenerateDiagramParent()
		{
			if (diagramParentObj == null) {
				diagramParentObj = new GameObject ();
				diagramParentObj.name = "Diagram Parent";
				diagramParentObj.transform.SetParent (this.transform,false);
				diagramParentObj.AddComponent<RectTransform> ();
				diagramParentObj.GetComponent<RectTransform>().sizeDelta = Vector2.one * gridCoordinateRange.x * gridOffset;
			}
		}

		//Generate clone of diagram to show rotation
		public void DiagramRotateClone()
		{
			GameObject rotationObject = GameObject.Instantiate (diagramParentObj);
			rotationObject.transform.SetParent (this.transform, false);
			rotationObject.AddComponent<Image> ();
			rotationObject.GetComponent<Image> ().color  =new Color(1f,1f,1f,0f);
			rotationObject.AddComponent<GraphDiagramRotator> ();
			SetDiagramColor (rotationObject, Color.blue);
			diagramParentObj = rotationObject;
		}



	}
}