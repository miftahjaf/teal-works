using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
	// Box containing vertical stack of child boxes.
	public class VerticalBox : Box
	{
		private float leftMostPos = float.MaxValue;
		private float rightMostPos = float.MinValue;
        public bool ExtensionMode = false;

		public static VerticalBox Get (Box Box, float Height, TexAlignment Alignment)
		{
            var box = ObjPool<VerticalBox>.Get();
			box.Add (Box);
            if(Box.totalHeight >= box.height)
                return box;
            float rest = Box.totalHeight - box.height;
			if (Alignment == TexAlignment.Center)
			{
				var strutBox = StrutBox.Get (0, rest / 2, 0, 0);
                box.Add (0, strutBox);
				box.height += rest / 2;
				box.depth += rest / 2;
				box.Add (strutBox);
			}
			else if (Alignment == TexAlignment.Top)
			{
				box.depth += rest;
				box.Add (StrutBox.Get (0, rest, 0, 0));
			}
			else if (Alignment == TexAlignment.Bottom)
			{
				box.height += rest;
                box.Add (0, StrutBox.Get (0, rest, 0, 0));
			}
            return box;
		}

        public static VerticalBox Get ()
        {           
            return ObjPool<VerticalBox>.Get();
        }

		public override void Add (Box box)
		{
			base.Add (box);

			if (Children.Count == 1)
			{
				height = box.height;
				depth = box.depth;
			}
			else
			{
				depth += box.height + box.depth;
			}
			RecalculateWidth (box);
		}

		public override void Add (int position, Box box)
		{
			base.Add (position, box);

			if (position == 0)
			{
				depth += box.depth + height;
				height = box.height;
			}
			else
			{
				depth += box.height + box.depth;
			}
			RecalculateWidth (box);
		}

		private void RecalculateWidth (Box box)
		{
			leftMostPos = Mathf.Min (leftMostPos, box.shift);
			rightMostPos = Mathf.Max (rightMostPos, box.shift + (box.width > 0 ? box.width : 0));
			width = rightMostPos - leftMostPos;
		}

		public override void Draw (DrawingContext drawingContext, float scale, float x, float y)
		{
			base.Draw (drawingContext, scale, x, y);

            float offset = ExtensionMode ? TEXPreference.main.GetPreference("ExtentPadding") : 0;
			var curY = y + height;
            for (int i = 0; i < Children.Count; i++)
            {
                Box child = Children[i];
                curY -= child.height;
                if(i > 0)
                    child.height += offset;
                if(i < Children.Count - 1)
                    child.depth += offset;
                child.Draw (drawingContext, scale, x + child.shift - leftMostPos, curY);
                if(i > 0)
                    child.height -= offset;
                if(i < Children.Count - 1)
                    child.depth -= offset;
                curY -= child.depth;
            }
		}
        public override void Flush()
        {
            base.Flush();
            leftMostPos = float.MaxValue;
            rightMostPos = float.MinValue;
            ExtensionMode = false;
            ObjPool<VerticalBox>.Release(this);
        }
	}
}