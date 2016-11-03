using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{

	public class TexRenderer : IFlushable
	{
		public static TexRenderer Get (Box box, float scale)
		{
            var renderer = ObjPool<TexRenderer>.Get();
            renderer.Box = box;
            renderer.Scale = scale;
            return renderer;
		}

        public Box Box;

        public float Scale;

        /// 0 = No, 1 = Yes, 2 = Yes (and there's a space erased)
		public int partOfPreviousLine = 0;

		public Vector2 RenderSize
		{
			get
			{
				return new Vector2 (Box.width * Scale, Box.totalHeight * Scale);
			}
		}

		public float Baseline
		{
			get
			{
				return Box.height / Box.totalHeight * Scale;
			}
		}

		public void Render (DrawingContext drawingContext, float x, float y)
		{
            if(Box != null)
                Box.Draw (drawingContext, Scale, x / Scale, y / Scale + Box.height);
        }

        public void Flush()
        {
            if(Box != null)
            {
                Box.Flush();
                Box = null;
            }
			partOfPreviousLine = 0;
            ObjPool<TexRenderer>.Release(this);
        }

        bool m_flushed = false;
        public bool IsFlushed { get { return m_flushed; } set { m_flushed = value; } }

	}
}