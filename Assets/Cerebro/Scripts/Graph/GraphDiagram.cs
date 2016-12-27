using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
using UnityEngine.UI.Extensions;


namespace Cerebro
{
	public class GraphDiagram 
	{
		public List<GraphPointScript> graphPoints;
		public VectorLine vectorLine;
		public bool isCloseDiagram;
		private LineShapeType lineShapeType;
		private List<UIPolygon> arcs;

		public GraphDiagram(VectorLine vectorLine,LineShapeType lineShapeType)
		{
			this.vectorLine = vectorLine;
			this.lineShapeType = lineShapeType;
			this.graphPoints = new List<GraphPointScript> ();
			this.arcs = new List<UIPolygon> ();
		}

		public void AddGraphPoint(GraphPointScript graphPoint)
		{
			graphPoints.Add (graphPoint);
		}

		public void Draw()
		{
			if (this.vectorLine == null) {
				return;
			}

		
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

		public void SetArcs (List<UIPolygon> arcs)
		{
			this.arcs = arcs;
		}

		public bool DragPiePoint(GraphPointScript piePoint, Vector2 position)
		{
			int index = graphPoints.IndexOf (piePoint);

			index = (index / 2);
			int previousIndex = index - 1;
			int nextIndex = index + 1;

			if (previousIndex <0 )
			{
				previousIndex =  arcs.Count - 1;
			}

			if (nextIndex >= arcs.Count)
			{
				nextIndex = 0;
			}
				
			UIPolygon currentArc = arcs [index];
			UIPolygon previousArc = arcs [previousIndex];
			UIPolygon nextArc = arcs [nextIndex];

			float diff =  MathFunctions.GetangleBetweenPoints(Vector2.zero, position) - MathFunctions.GetangleBetweenPoints (Vector2.zero, piePoint.linePoint.origin);

			if (diff > 180f) {
				diff =  diff - 360f;
			}

			if (diff < -180f) {
				diff = 360 + diff;
			}


			float newRotation =  currentArc.rotation + diff;
			float minAngle = MathFunctions.GetClampedAngle (previousArc.rotation) + 5f;
			float maxAngle = MathFunctions.GetClampedAngle (nextArc.rotation) - 5f;
		
			if (minAngle > maxAngle) {
				maxAngle = 360f + maxAngle;
			}

			if (newRotation < minAngle) {
				newRotation =  360f + newRotation;
			}

			if (newRotation > maxAngle) {
				newRotation =  newRotation - 360f;
			}

			if (newRotation < minAngle || newRotation > maxAngle)
			{
				return false;
			}

			currentArc.rotation = newRotation;
			currentArc.ReDraw ();

			piePoint.linePoint.origin = position;

			this.Draw();
			this.UpdatePieArcFill ();

			return true;
		}

		public void UpdatePieArc(float radius)
		{
			int nextIndex = 0;
			foreach (UIPolygon arc in arcs) 
			{
				PolygonCollider2D collider = arc.gameObject.AddComponent<PolygonCollider2D> ();
				List<Vector2> colliderPoints = new List<Vector2> ();

				nextIndex++;
		
				if (nextIndex >= arcs.Count) {
					nextIndex = 0;
				}
			
				float nextAngle =  (nextIndex == 0? 540 + arcs[nextIndex].rotation : 180f +  arcs[nextIndex].rotation);
				Debug.Log (" Start angle " + (180f + arc.rotation) + " next angle " + nextAngle);
				colliderPoints.Add (Vector2.zero);
				Debug.Log ("Start Angle " + (180f + arc.rotation) + " end angle " + nextAngle);
				for (float angle =   180f + arc.rotation; angle <= nextAngle; angle = angle + 5) {
					colliderPoints.Add (MathFunctions.PointAtDirection (Vector2.zero, angle, radius));
				}

				collider.SetPath (0, colliderPoints.ToArray ());
			}
		}

		public void UpdatePieArcFill()
		{
			int nextIndex = 0;
			foreach (UIPolygon arc in arcs) 
			{
				nextIndex++;

				if (nextIndex >= arcs.Count) {
					nextIndex = 0;
				}

			
			}
		}


	}
}