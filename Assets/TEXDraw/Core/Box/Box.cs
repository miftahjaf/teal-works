using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
	// Represents graphical box that is part of math expression, and can itself contain child boxes.
	public abstract class Box : IFlushable
	{
		protected List<Box> children;
		
		protected Box ()
		{
            children = ListPool<Box>.Get();
		}

		public List<Box> Children
		{
			get { return children; }
		}


		public float totalHeight
		{
			get { return height + depth; }
		}

        public float width;
        public float height;

        public float depth;

        public float shift;

		public virtual void Draw (DrawingContext drawingContext, float scale, float x, float y)
		{
            //EASTER-EGG: Un-strip this line for a fun part ;)
            //drawingContext.DrawWireDebug(new Rect(x * scale, (y - depth) * scale, width * scale, totalHeight * scale), new Color(1,1,1, 0.07f));
            //PS : this line is intended for debugging only
		}

		public virtual void Add (Box box)
		{
			children.Add (box);
		}

		public virtual void Add (int position, Box box)
		{
			children.Insert (position, box);
		}

        public virtual void Flush()
        {
            width = 0;
            height = 0;
            depth = 0;
            shift = 0;
            if(children.Count > 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    children[i].Flush();
                }
                children.Clear();
            }
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