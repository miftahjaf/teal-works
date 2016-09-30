using UnityEngine;

namespace Cerebro 
{
	public enum LineShapeType
	{
		Normal,
		Dotted
	}
	public class LinePoint 
	{
		public string name;
		public Vector2 origin;
		public float angle;
		public float radius;
		public bool shouldShowArrow;
		public int textDirection;
		public Vector2 textOffset;
		public LineShapeType lineType;
		public LinePoint(string name,Vector2 origin,float angle,bool shouldShowArrow,float radius =100f,int textDirection=0)
		{
			this.name = name;
			this.origin = origin;
			this.angle = angle;
			this.radius = radius;
			this.shouldShowArrow = shouldShowArrow;
			this.textDirection = textDirection;

		}


		public LinePoint(string name,Vector2 origin,float angle,bool shouldShowArrow,Vector2 textOffset,float radius =100f)
		{
			this.name = name;
			this.origin = origin;
			this.angle = angle;
			this.radius = radius;
			this.shouldShowArrow = shouldShowArrow;
			this.textOffset = textOffset;
		}


		public LinePoint SetLineType(LineShapeType lineType)
		{
			this.lineType = lineType;
			return this;
		}
	}
}