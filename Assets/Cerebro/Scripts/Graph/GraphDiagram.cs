using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;


namespace Cerebro
{
	public class GraphDiagram 
	{
		public List<GraphPointScript> graphPoints;
		public VectorLine vectorLine;
		public bool isCloseDiagram;
		private LineShapeType lineShapeType;
		public GraphDiagram(VectorLine vectorLine,LineShapeType lineShapeType)
		{
			this.vectorLine = vectorLine;
			this.lineShapeType = lineShapeType;
			this.graphPoints = new List<GraphPointScript> ();
		}

		public void AddGraphPoint(GraphPointScript graphPoint)
		{
			graphPoints.Add (graphPoint);
		}

		public void Draw()
		{
			if (this.vectorLine == null  )
				return;

		
			this.vectorLine.points2 = GetPointList();
			this.vectorLine.Draw ();
		}

		public List<Vector2> GetPointList()
		{
			List<Vector2> points = new List<Vector2> ();
			foreach (GraphPointScript graphPoint in graphPoints) {
				points.Add (graphPoint.linePoint.origin);
			}
			if (isCloseDiagram && points.Count > 1) {
				points.Add (points [0]);
			}
			return points;
		}

		public void SetColor(Color color)
		{
			this.vectorLine.color = color;
		}

		public void SetIsCloseDiagram(bool isCloseDiagram)
		{
			this.isCloseDiagram = isCloseDiagram;
		}

		public LineShapeType LineShapeType()
		{
			return this.lineShapeType;
		}
			
	}
}