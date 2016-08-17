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
            ObjPool<TexRenderer>.Release(this);
        }

        bool m_flushed;

        public bool GetFlushed()
        { 
            return m_flushed;
        }

        public void SetFlushed(bool flushed)
        {
            m_flushed = flushed;
        }
	}
}