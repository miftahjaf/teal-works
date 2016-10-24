using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;
namespace Cerebro
{
	public class GraphLine 
	{
		public VectorLine vectorLine;
		public GraphPointScript point1;
		public GraphPointScript point2;

		public GraphLine(VectorLine vectorLine,GraphPointScript point1,GraphPointScript point2)
		{
			this.vectorLine = vectorLine;
			this.point1 = point1;
			this.point2 = point2;
		}

		public void Draw()
		{
			if (this.vectorLine == null)
				return;

			this.vectorLine.points2 = new List<Vector2> (){ point1.linePoint.origin, point2.linePoint.origin };
			this.vectorLine.Draw ();
		}

	}
}
