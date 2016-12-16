using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Vectrosity;
using UnityEngine.UI.Extensions;
using System.Linq;
using MaterialUI;

namespace Cerebro
{
	public enum StatisticsType
	{
		VerticalBar,
		HorizontalBar,
		Pie,
		PieToFill,
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
		private Vector2 snapValue;                //Graph sanp value (Default grid offset)

		private float gridOffset;               //Grid offset
		private float fontMultiPlier;           //Font multiplier to make graph font smaller or bigger
		private float pieRadius;


		private bool isInteractable;

		private string graphTitle;

		public StatisticsType statisticType;

		private StatisticsAxis[] statisticsAxises;
		private List<StatisticsBar> statisticsBars;

		public List<string> barValues;
		private List<int> pieValues;
		private List<string> pieStrings;


		private List<UIPolygon> pieArcList;
		private List<string> currentColors;
		private List<GraphPointScript> lineGraphPoints;

		private bool canClick;

		private Color currentSelectedColor;

		private GraphDiagram currentLineGraphDiagram;

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
			pieValues = new List<int> ();
			pieStrings = new List<string> ();
			this.ShiftPosition(Vector2.zero);
			pieRadius = 150f;
			pieArcList = new List<UIPolygon> ();
			currentColors = new List<string> ();
			currentSelectedColor = Color.white;
			currentSelectedColor.a = 0;
			barValues = new List<string> ();
			canClick = true;
			lineGraphPoints = new List<GraphPointScript> ();

		}

		//Set grid parameters
		public void SetGridParameters (Vector2 _gridCoordinateRange ,float _gridOffset)
		{
			gridCoordinateRange = _gridCoordinateRange;
			gridOffset = _gridOffset;
			graphCenter = new Vector2 (Mathf.RoundToInt(gridCoordinateRange.x * gridOffset / 2f),Mathf.RoundToInt(gridCoordinateRange.y * gridOffset / 2f));
			gridPosition = new Vector2 (-gridCoordinateRange.x * gridOffset/2f , -gridCoordinateRange.y * gridOffset/2f);
			graphOrigin = graphCenter;
			snapValue = Vector2.one * gridOffset;
			this.GetComponent<Image> ().GetComponent<RectTransform> ().sizeDelta = Vector2.one * _gridCoordinateRange.x * gridOffset;
			ShiftGraphOrigin (- _gridCoordinateRange / 2);
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
			this.snapValue = Vector2.one * _snapValue;
		}

		//Set snap value
		public void SetSnapValue(Vector2 _snapValue)
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
			if (statisticType == StatisticsType.Pie || statisticType == StatisticsType.PieToFill) 
			{
				DrawPieGraph ();
			} else {
				DrawGrid ();
				DrawAxis (showAxis);
				SetTitles ();
			}
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

			currentColors = CerebroHelper.GetRandomColorValues (barValues.Count>0?barValues.Count:1);
			for(int barValueCnt = barValues.Count-1; barValueCnt >=0 ; barValueCnt--)
			{
				GenerateGraphLabel (barValues [barValueCnt], new Vector2 (-gridPosition.x +20f , gridPosition.y +  barValues.Count *30f - barValueCnt * 30f), currentColors [barValueCnt], gridOffset);
			}

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
				if (statisticType == StatisticsType.HorizontalBar || statisticType == StatisticsType.VerticalBar || statisticType == StatisticsType.Line) {
					startOffset = axisOffset.x / pointOffset.x;
				}
				pointInPosXAxis = statisticsAxis.statisticsValues.Count;
				graphMaxValue.x =  Mathf.Abs((Mathf.RoundToInt((gridCoordinateRange.x/2f + graphCenterOffset.x)))) * axisOffset.x/pointOffset.x ;
			}

			for (int i = 1; i <= pointInPosXAxis; i++) 
			{
				string text = (i * axisOffset.x).ToString ();
				int value = 0;

				if (statisticType == StatisticsType.VerticalBar || statisticType == StatisticsType.Line) {
					text = "";
					if(statisticsAxis.statisticsValues.Count > i - 1 )
					{
						text =  statisticsAxis.statisticsValues [i - 1].name;
						int totalValues = statisticsAxis.statisticsValues [i - 1].values.Length;
						float offsetValue = (2 *startOffset) / totalValues;
						float startPoint = i * axisOffset.x - GetStartOffsetValue(startOffset, totalValues);
						for (int count = 0; count < totalValues; count++) {
							value = statisticsAxis.statisticsValues [i - 1].values [count];

							if (statisticType == StatisticsType.Line) {
								GenerateLineGraphPoint (new Vector2 (startPoint, 0f), new Vector2 (startPoint, value));
							} else {
								GenerateGraphBar (new Vector2 (startPoint, 0f), new Vector2 (startPoint, value), currentColors [count], 2f / totalValues, 50f / totalValues);
							}
							startPoint +=offsetValue;
						}
					}

				}
				GenerateLinePoint (new LinePoint (text, GraphPosToUIPos (new Vector2 (i * axisOffset.x-startOffset, 0)), 0f, false,0).SetPointTextOffset(new Vector2(0,-15)),axisParent);
			}

			if (statisticType == StatisticsType.Line) {
				DrawLineGraph ();
			}

			//Neg X
			graphMinValue.x = 0f;
			statisticsAxis = statisticsAxises [1];

			//Pos Y
			int pointInPosYAxis =  0;
			startOffset = 0f;

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
							GenerateGraphBar (new Vector2 (0f,startPoint), new Vector2 (value, startPoint),currentColors[count],2f/totalValues,50f/totalValues);
							startPoint +=offsetValue;
						}
					}

				}
				GenerateLinePoint (new LinePoint (text, GraphPosToUIPos (new Vector2 (0, i  * axisOffset.y -startOffset)), 0f, false, 0).SetPointTextOffset(new Vector2(statisticsAxis.statisticsValues.Count>0?-30f:-20f,0)),axisParent);
			}


			//Neg 
			graphMinValue.y = 0f;
			axisParent.gameObject.SetActive (showAxis);
		}

		private float GetStartOffsetValue(float offset, int noOfValues)
		{
			float value = offset;
			for (int i =1; i < noOfValues; i++) {
				value += (offset/ (i*2f));
			}
			return value;
		}




		public void DrawLineGraph()
		{
			GameObject diagramObj = GameObject.Instantiate (vectorObjectPrefab);
			diagramObj.transform.SetParent (this.transform,false);
			diagramObj.name = "line graph";


			VectorLine vectorLine = diagramObj.GetComponent<VectorObject2D> ().vectorLine;
			vectorLine.SetWidth (2f);
			vectorLine.color = Color.black;
			vectorLine.lineType = LineType.Continuous;
		
			currentLineGraphDiagram = new GraphDiagram (vectorLine,LineShapeType.Normal);
			foreach (GraphPointScript graphPointScript in lineGraphPoints) 
			{
				graphPointScript.SetDigramObject (currentLineGraphDiagram);
				currentLineGraphDiagram.AddGraphPoint (graphPointScript);
			}

			currentLineGraphDiagram.Draw ();
		}

		public void GenerateLineGraphPoint(Vector2 startPoint,Vector2 endPoint)
		{
			GraphPointScript graphPointScript = PlotPoint (startPoint +  new Vector2 (0f, axisOffset.y/pointOffset.y), "", true, false);
			graphPointScript.SetPointMovementType (PointMovementType.Vertical);
			lineGraphPoints.Add (graphPointScript);
		}

		public void GenerateGraphBar(Vector2 startPoint,Vector2 endPoint,string colorCode,float width = 2f,float pointSize =50f)
		{
			GameObject barObj = new GameObject ();
			barObj.transform.SetParent (this.transform, false);
			StatisticsBar statisticsBar = barObj.AddComponent<StatisticsBar> ();
			barObj.AddComponent<Image> ();
			barObj.GetComponent<Image> ().raycastTarget = false;
			statisticsBar.SetColor (CerebroHelper.HexToRGB(colorCode));
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
			statisticsBar.SetIsHorizontal (statisticType == StatisticsType.HorizontalBar);
			float height = Vector2.Distance (GraphPosToUIPos (startPoint), GraphPosToUIPos (endPoint));
			statisticsBar.SetHeight (height);
			statisticsBar.SetStartHeight (isInteractable?gridOffset: height);
			statisticsBar.SetWidth (gridOffset*width);
			statisticsBar.SetBar ();
			statisticsBars.Add (statisticsBar);

			if (isInteractable) 
			{
				GraphPointScript pointScript = PlotPoint (startPoint + (statisticType == StatisticsType.VerticalBar ? new Vector2 (0f, axisOffset.y/pointOffset.y) : new Vector2 (axisOffset.x/pointOffset.x, 0f)), "", true, false,pointSize);
				pointScript.SetStatisticsBar (statisticsBar);
				pointScript.SetPointMovementType (statisticType == StatisticsType.HorizontalBar ? PointMovementType.Horizontal : PointMovementType.Vertical);
				statisticsBar.SetGraphPoint (pointScript);

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
		public GraphPointScript PlotPoint(Vector2 point,string displayText="",bool canDrag =true,bool checkInsideGraph = true,float size = 50f)
		{
			if (IsContainInGraph (point) || !checkInsideGraph) {
				GraphPointScript graphPointScript = GenerateLinePoint (new LinePoint (displayText, GraphPosToUIPos (point), 0f, false,0).SetPointTextOffset(new Vector2(0,-10)));
				graphPointScript.SetPointColor (Color.blue);
				graphPointScript.SetDotSize (3f);
				graphPointScript.SetIsDragabble (canDrag);
				graphPointScript.SetSize (size);
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
			return new Vector2 (Mathf.RoundToInt (position.x/snapValue.x ) * snapValue.x , Mathf.RoundToInt (position.y/snapValue.y) *snapValue.y);
		}

		//Get snap point from given point
		public Vector2 GetSnapPoint(Vector2 point)
		{
			Vector2 snapOffsetValue = (snapValue / gridOffset);
			return new Vector2 (Mathf.RoundToInt (point.x/(axisOffset.x/pointOffset.x * snapOffsetValue.x)) *(axisOffset.x/pointOffset.x * snapOffsetValue.x), Mathf.RoundToInt (point.y/(axisOffset.y/pointOffset.y * snapOffsetValue.y)) * (axisOffset.y/pointOffset.y * snapOffsetValue.y));
		}

		//Move plotted point or line point and change line according to point movement
		public void MovePlottedPoint(GraphPointScript graphPointObj,Vector2 position)
		{
			RectTransform pointRectTransform = graphPointObj.GetComponent<RectTransform> ();
			Vector2 oldPos = pointRectTransform.anchoredPosition;


			if (graphPointObj.GetPointMovementType () == PointMovementType.Vertical) {
				position.x = graphPointObj.transform.position.x;
					
			} else if (graphPointObj.GetPointMovementType () == PointMovementType.Horizontal) {
				position.y = graphPointObj.transform.position.y;
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
			if (graphPointObj.diagramObj != null) {
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
			bool correct = false;
			switch (statisticType) 
			{
				case StatisticsType.HorizontalBar:
				case StatisticsType.VerticalBar:
					correct = IsAllBarAnswersCorrect ();
					break;

				case StatisticsType.PieToFill:
					correct = IsPieFilledCorrect ();
					break;

			}

			return correct;
		}

		public bool IsAllBarAnswersCorrect()
		{
			foreach (StatisticsBar bar in statisticsBars) {
				if (!bar.IsCorrect ()) {
					return false;
				}
			}

			return true;
		}

		public bool IsPieFilledCorrect()
		{
			int cnt = currentColors.Count;
			for(int i=0;i< cnt;i++)
			{
				List<UIPolygon> tempPolygons = pieArcList.FindAll(x=>x.color == CerebroHelper.HexToRGB(currentColors[i]));
				Debug.Log ("Color "+CerebroHelper.HexToRGB(currentColors[i]) +"Polygon count "+tempPolygons.Count);
				if (tempPolygons.Count > 1 || tempPolygons.Count<=0) {
					return false;
				} 
			}
			Dictionary<int,List<int>> tempValueList = new Dictionary<int, List<int>> ();
			for (int i = 0; i < cnt; i++) {
				if (tempValueList.ContainsKey (pieValues [i])) {
					tempValueList [pieValues [i]].Add (i);
				} else {
					tempValueList [pieValues [i]] = new List<int> ();
					tempValueList [pieValues [i]].Add (i);
				}
			}
			List<bool> correctPieFillValues = new List<bool>();
			for (int i = 0; i < cnt; i++)
			{
				List<int>  tempColorValues = tempValueList [pieValues [i]];
				bool correctColor = false;
				foreach(int value in tempColorValues)
				{
					if (pieArcList [i].color == CerebroHelper.HexToRGB(currentColors[value])) {
						correctColor = true;
					}
				}
				correctPieFillValues.Add (correctColor);
			}

			return correctPieFillValues.Count(x=>x == true) == cnt;
		}

		public void HandleCorrectAnswer ()
		{
			if (!isInteractable) {
				return;
			}
			switch (statisticType) {
			case StatisticsType.HorizontalBar:
			case StatisticsType.VerticalBar:
				ShowCorrectBars ();
				break;

			case StatisticsType.PieToFill:
				ShowCorrectPieToFillArcs ();
				break;

			}
		}

		public void ResetAnswer ()
		{
			if (!isInteractable) {
				return;
			}
			canClick = true;
			switch (statisticType) {
			case StatisticsType.HorizontalBar:
			case StatisticsType.VerticalBar:
				ResetAllBars ();
				break;

			case StatisticsType.PieToFill:
				ResetAllPieArcs ();
				break;

			}
		}

		public void HandleIncorrectAnwer (bool isRevisited)
		{
			if (!isInteractable) {
				return;
			}
			switch (statisticType) {
			case StatisticsType.HorizontalBar:
			case StatisticsType.VerticalBar:
				ShowWrongBars ();
				break;

			case StatisticsType.PieToFill:
				ShowWrongPieToFillArcs ();
				if (!isRevisited) {
					canClick = false;
				}
				break;
			}
		}

		public void ShowCorrectAnswer()
		{
			if (!isInteractable) {
				return;
			}
			switch (statisticType) {
			case StatisticsType.HorizontalBar:
			case StatisticsType.VerticalBar:
				ShowCorrectBarAnswers ();
				break;

			case StatisticsType.PieToFill:
				ShowCorrectPieToFillAnswers ();
				break;
			}
		}

		public void ShowCorrectBarAnswers()
		{
			foreach (StatisticsBar bar in statisticsBars) 
			{
				bar.SetCorrectAnswer ();
			}
		}

		public void ShowCorrectPieToFillAnswers()
		{
			int cnt = 0;
			foreach (UIPolygon arc in pieArcList) {
				arc.fill = true;
				arc.color = CerebroHelper.HexToRGB (currentColors [cnt]);
			    cnt++;
			}
			SetPieFillBorderColor (MaterialColor.green800);

		}

		public void ShowCorrectBars()
		{
			foreach (StatisticsBar bar in statisticsBars) 
			{
					bar.ChangeColor (MaterialColor.green800);
			}
		}

		public void ShowCorrectPieToFillArcs()
		{
			SetPieFillBorderColor (MaterialColor.green800);
		}

		public void SetPieFillBorderColor(Color color)
		{
			Transform obj;
			if (obj = transform.Find ("PieArcFill"))
			{
				if(obj.GetComponent<UIPolygon> ())
				{
					obj.GetComponent<UIPolygon> ().color = color;
				}
			}

			if (obj = transform.Find ("PieLineFill"))
			{
				if(obj.GetComponent<VectorObject2D> ())
				{
					obj.GetComponent<VectorObject2D> ().vectorLine.color = color;
				}
			}
		}
			

		public void ShowWrongBars()
		{
			foreach (StatisticsBar bar in statisticsBars) 
			{
				if (!bar.IsCorrect ()) {
					bar.ChangeColor (MaterialColor.red800);
				} else {
					bar.ChangeColor (MaterialColor.green800);
				}
			}
		}

		public void ShowWrongPieToFillArcs()
		{
			SetPieFillBorderColor (MaterialColor.red800);
		}

		public bool IsAnswered ()
		{
			bool isAnswered = true;

			if (!isInteractable) {
				return isAnswered;
			}

			switch(statisticType)
			{
				case StatisticsType.HorizontalBar:
				case StatisticsType.VerticalBar:
					isAnswered = IsGraphAnswered ();
				break;

				case StatisticsType.PieToFill:
				   isAnswered = IsPieGraphFilled ();
				break;
			}

			return isAnswered;
		}


		public bool IsGraphAnswered()
		{
			foreach (StatisticsBar bar in statisticsBars) {
				if (bar.IsChanged ()) {
					return true;
				}
			}
			return false;
		}

	    public void ResetAllBars()
		{
			foreach (StatisticsBar bar in statisticsBars) 
			{
				bar.Reset ();
			}

		}

		public void ResetAllPieArcs()
		{
			foreach (UIPolygon arc in pieArcList) {
				arc.fill = false;
				arc.color = Color.black;
			}
			currentSelectedColor.a = 0;
			SetPieFillBorderColor (Color.black);
		}

		public bool IsPieGraphFilled()
		{
			foreach (UIPolygon arc in pieArcList) {
				if (arc.fill) {
					return true;
				}
			}
			return false;
		}


		//Shift graph position
		public void ShiftPosition (Vector2 position)
		{
			this.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -30f) + position;
		}

		private void SetTitles()
		{
			GenerateTextObject (new Vector2(0,-gridPosition.y + 17f),graphTitle,"XAxis Title",15);
			GenerateTextObject (new Vector2(0,gridPosition.y - 37f),statisticsAxises[0].axisName,"XAxis Title");
			GenerateTextObject (new Vector2(gridPosition.x - (statisticsAxises[1].statisticsValues.Count>0?65f:45f),0),statisticsAxises[1].axisName,"yAxis Title",13,90);
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

		public void SetPieParameters(List<string> _pieStrings, List<int> _pieValues)
		{
			pieStrings = _pieStrings;
			pieValues = _pieValues;
		}

		public void SetPieRadius(float _pieRadius)
		{
			pieRadius = _pieRadius;
		}

		public void DrawPieGraph()
		{
			int count = pieStrings.Count;
			float startAngle = Random.Range(0f,360f);
			int totalValue = pieValues.Sum(x => System.Convert.ToInt32(x));
			int pieValueCount = pieValues.Count;
			float offsetPos = pieRadius / 5.5f;
			Vector2 labelPosition = new Vector2 (pieRadius+offsetPos+10f, pieRadius/2f);
			currentColors = CerebroHelper.GetRandomColorValues (pieValueCount);

			List<Vector2> linePoints = new List<Vector2> ();

			List<int> randomList =  Enumerable.Range(0, pieValueCount).ToList();
			UIPolygon[] tempPolygons = new UIPolygon[pieValueCount];
			if (statisticType == StatisticsType.PieToFill) {
				randomList.Shuffle ();
			}

			UIPolygon[]  tempPieArcList = new UIPolygon[count];
			for (int i = 0; i < pieValueCount; i++) {
				//Instantiate arc prefab
				GameObject arc = GameObject.Instantiate (arcPrefab);
				arc.transform.SetParent (this.transform, false);

				//UI polygon component to draw arc
				UIPolygon UIpolygon = arc.GetComponent<UIPolygon> ();
			
				float nextAngle = 0f;
				if (pieValueCount > i) {
					nextAngle = 360f * pieValues [randomList[i]] /totalValue;
				}
			
				if(nextAngle>0)
				{
					Color color = CerebroHelper.HexToRGB(currentColors [i]);
					//set arc size according to radius
					UIpolygon.GetComponent<RectTransform> ().sizeDelta = Vector2.one * pieRadius * 2f;

					UIpolygon.fillPercent =  ((100f * nextAngle) / 360f) + 0.5f;
					UIpolygon.rotation = startAngle + 180f;
					UIpolygon.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;

					UIpolygon.ReDraw ();
					GenerateGraphLabel (pieStrings [i], labelPosition, currentColors [i],offsetPos + 10f,statisticType == StatisticsType.PieToFill);
					labelPosition -= new Vector2(0, offsetPos + 18f);

					if (statisticType == StatisticsType.PieToFill) {
						UIpolygon.color = Color.black;
						linePoints.Add (Vector2.zero);
						linePoints.Add (MathFunctions.PointAtDirection (Vector2.zero, startAngle, pieRadius));


						PolygonCollider2D collider = UIpolygon.gameObject.AddComponent<PolygonCollider2D> ();
						List<Vector2> colliderPoints = new List<Vector2> ();
						colliderPoints.Add (Vector2.zero);
						for(float angle = startAngle ; angle<= startAngle + nextAngle ; angle = angle+5)
						{
							colliderPoints.Add (MathFunctions.PointAtDirection(Vector2.zero,angle,pieRadius));
						}
						collider.SetPath(0, colliderPoints.ToArray());

						ColliderButton colliderButton = UIpolygon.gameObject.AddComponent<ColliderButton> ();
						colliderButton.OnClicked = delegate {
							if(currentSelectedColor.a>0 && canClick)
							{
								UIpolygon.color =currentSelectedColor;
								UIpolygon.fill = true;
							}
						};

					} else {
						UIpolygon.color = color;
						UIpolygon.fill = true;
					}
					startAngle += nextAngle;
					tempPolygons [randomList [i]] = UIpolygon;
				}
			}

			pieArcList = tempPolygons.ToList ();

			if (linePoints.Count > 0) {


				//Instantiate arc prefab
				GameObject arc = GameObject.Instantiate (arcPrefab);
				arc.transform.SetParent (this.transform, false);
				UIPolygon UIpolygon = arc.GetComponent<UIPolygon> ();
				UIpolygon.fillPercent = 100f;
				UIpolygon.GetComponent<RectTransform> ().sizeDelta = Vector2.one * pieRadius * 2f;
				UIpolygon.ReDraw ();
				UIpolygon.name = "PieArcFill";

				GameObject lineObject = GameObject.Instantiate (vectorObjectPrefab);
				lineObject.transform.SetParent (this.transform,false);
				lineObject.name = "PieLineFill";
				lineObject.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
				VectorLine vectorLine = lineObject.GetComponent<VectorObject2D> ().vectorLine;
				vectorLine.points2 = linePoints;
				vectorLine.lineType = LineType.Discrete;
				vectorLine.Draw ();

				GameObject raycastDetector = new GameObject ();
				raycastDetector.AddComponent<RayCastDetector> ();
				raycastDetector.transform.SetParent (this.transform, false);
				raycastDetector.name = "Raycaster";
			}
		}

		public void GenerateGraphLabel(string text, Vector2 position, string colorCode,float size,bool addButton =false)
		{
			GameObject pieLabel = new GameObject ();
			pieLabel.name = "pielLabel";
			pieLabel.transform.SetParent (this.transform, false);
			Image pieLabelImage = pieLabel.AddComponent<Image> ();
			pieLabel.GetComponent<RectTransform> ().anchoredPosition = position;
			pieLabelImage.color = CerebroHelper.HexToRGB(colorCode);
			pieLabel.GetComponent<RectTransform> ().sizeDelta = Vector2.one * size;
			if (addButton) {
				Button button = pieLabel.AddComponent<Button> ();
				button.onClick.AddListener (() => {
					currentSelectedColor = CerebroHelper.HexToRGB (colorCode);
				});
			}

			GameObject pieLabelText = GameObject.Instantiate (textObjectPrefab);
			pieLabelText.name = text;
			pieLabelText.transform.SetParent (this.transform, false);
			Text pieText = pieLabelText.GetComponent<Text> ();
			pieText.alignment = TextAnchor.MiddleLeft;
			pieText.text = text;
			pieLabelText.GetComponent<RectTransform> ().anchoredPosition = position+new Vector2(size,0);
			pieLabelText.GetComponent<RectTransform> ().sizeDelta = Vector2.zero;
		}

		public void SetBarValues(List<string> _barValues)
		{
			barValues = _barValues;
		}

		public bool IsInteractable()
		{
			return isInteractable;
		}
			
	}
}
