﻿using UnityEngine;
using System.Collections.Generic;
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
	public class Stick
	{
		public int numberOfSticks;
		public float fractionLength;
		public Stick(int numberOfSticks,float fractionLength)
		{
			this.numberOfSticks = numberOfSticks;
			this.fractionLength = fractionLength;
		}
	}
	public class LinePoint 
	{
		public string name;
	
		public float angle;
		public float radius;

		public bool shouldShowArrow;
		public bool shouldShowDot;
		public bool shouldFlip;

		public string lineText;

		public Vector2 origin;
		public Vector2 pointTextOffset;
		public Vector2 nextPoint;

		public List<Stick> sticks;
		public int textDirection;

		public LineShapeType lineType;
		public TextDir lineTextDirection;

		public LinePoint()
		{
			this.name = "";
			this.origin = Vector2.zero;
			this.angle = 0;
			this.radius = 0;
			this.shouldShowArrow = false;
			this.shouldShowDot = true;
			this.textDirection = 0;
			this.pointTextOffset = Vector2.zero;
			this.lineType = LineShapeType.Normal;
			this.lineText = "";
			this.lineTextDirection = TextDir.None;
			this.nextPoint = Vector2.zero;
			this.sticks = new List<Stick> ();
			this.shouldFlip = false;
		}



		public LinePoint(string name,Vector2 origin,float angle,bool shouldShowArrow,float radius =100f,int textDirection=0) :this()
		{
			this.name = name;
			this.origin = origin;
			this.angle = angle;
			this.radius = radius;
			this.shouldShowArrow = shouldShowArrow;
			this.textDirection = textDirection;
			this.nextPoint = this.origin;
		}


		public LinePoint(string name,Vector2 origin,float angle,bool shouldShowArrow,Vector2 pointTextOffset,float radius =100f):this()
		{
			this.name = name;
			this.origin = origin;
			this.angle = angle;
			this.radius = radius;
			this.shouldShowArrow = shouldShowArrow;
			this.pointTextOffset = pointTextOffset;
			this.nextPoint = this.origin;
		}

		public LinePoint(string name,Vector2 origin,Vector2 nextPoint,bool shouldShowArrow = false):this()
		{
			this.name = name;
			this.origin = origin;
			this.nextPoint = nextPoint;
			this.shouldShowArrow = shouldShowArrow;
			this.pointTextOffset = pointTextOffset;
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

		public LinePoint SetShouldShowDot(bool shouldShowDot)
		{
			this.shouldShowDot = shouldShowDot;
			return this;
		}

		public LinePoint SetTextDirection(int textDirection)
		{
			this.textDirection = textDirection;
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
			
		public LinePoint SetSticks(List<Stick> sticks)
		{
			this.sticks = sticks;
			return this;
		}

		public LinePoint FlipArrow()
		{
			if (nextPoint.Equals (origin)) {
				this.origin = MathFunctions.PointAtDirection (origin, angle, radius);
				this.angle = angle + 180f;
			} else {
				Vector2 tempPoint = this.origin;
				this.origin = this.nextPoint;
				this.nextPoint = tempPoint;
			}
			return this;

		}

			
	}
}