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

		public AngleArc(string value,Vector2 origin,float startAngle,float endAngle,float radius=0)
		{
			this.value = value;
			this.startAngle = startAngle;
			this.endAngle = endAngle;
			this.origin = origin;
			this.radius = radius;
		}
	}
}
