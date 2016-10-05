using UnityEngine;
using System.Collections;

namespace Cerebro 
{
	public class AngleArc 
	{

		public string value;
		public float startAngle;
		public float endAngle;
		public Vector2 origin;
		public float radius;
		public bool textInsideArc;
		public bool squareArcFor90;

		public AngleArc(string value,Vector2 origin,float startAngle,float endAngle,float radius=0, bool textInsideArc = false, bool squareArcFor90 = true)
		{
			this.value = value;
			this.startAngle = startAngle;
			this.endAngle = endAngle;
			this.origin = origin;
			this.radius = radius;
			this.textInsideArc = textInsideArc;
			this.squareArcFor90 = squareArcFor90;
		}
	}
}
