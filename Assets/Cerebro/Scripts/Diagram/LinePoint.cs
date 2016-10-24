using UnityEngine;

namespace Cerebro 
{
	public enum LineShapeType
	{
		Normal,
		Dotted
	}
	public enum TextDir
	{
		Up,
		Down,
		Left,
		Right,
		None
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
		public string lineText;
		public TextDir lineTextDirection;
		public Vector2 pointTextOffset;

		public LinePoint()
		{
			this.name = "";
			this.origin = Vector2.zero;
			this.angle = 0;
			this.radius = 0;
			this.shouldShowArrow = false;
			this.textDirection = 0;
			this.textOffset = Vector2.zero;
			this.lineType = LineShapeType.Normal;
			this.lineText = "";
			this.lineTextDirection = TextDir.None;
		}



		public LinePoint(string name,Vector2 origin,float angle,bool shouldShowArrow,float radius =100f,int textDirection=0) :this()
		{
			this.name = name;
			this.origin = origin;
			this.angle = angle;
			this.radius = radius;
			this.shouldShowArrow = shouldShowArrow;
			this.textDirection = textDirection;
		
		}


		public LinePoint(string name,Vector2 origin,float angle,bool shouldShowArrow,Vector2 textOffset,float radius =100f):this()
		{
			this.name = name;
			this.origin = origin;
			this.angle = angle;
			this.radius = radius;
			this.shouldShowArrow = shouldShowArrow;
			this.textOffset = textOffset;
		}

		public LinePoint SetName(string name)
		{
			this.name = name;
			return this;
		}

		public LinePoint SetOrigin(Vector2 origin)
		{
			this.origin = origin;
			return this;
		}


		public LinePoint SetAngle(float angle)
		{
			this.angle =angle;
			return this;
		}

		public LinePoint SetRadius(float radius)
		{
			this.radius = radius;
			return this;
		}

		public LinePoint SetShouldShowArrow(bool shouldShowArrow)
		{
			this.shouldShowArrow = shouldShowArrow;
			return this;
		}

		public LinePoint SetTextDirection(int textDirection)
		{
			this.textDirection = textDirection;
			return this;
		}

		public LinePoint SetTextOffset(Vector2 textOffset)
		{
			this.textOffset = textOffset;
			return this;
		}

			
		public LinePoint SetLineType(LineShapeType lineType)
		{
			this.lineType = lineType;
			return this;
		}

		public LinePoint SetLineText(string lineText )
		{
			this.lineText = lineText;

			return this;
		}

		public LinePoint SetLineTextDirection(TextDir lineTextDirection)
		{
			this.lineTextDirection = lineTextDirection;
			return this;
		}

		public LinePoint SetPointTextOffset(Vector2 pointTextOffset)
		{
			this.pointTextOffset = pointTextOffset;
			return this;
		}
	}
}