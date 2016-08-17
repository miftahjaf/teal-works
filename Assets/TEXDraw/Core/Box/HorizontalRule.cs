using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
	// Box representing horizontal line.
	public class HorizontalRule : Box
	{
		public static HorizontalRule Get (float Thickness, float Width, float Shift)
        {
            var box = ObjPool<HorizontalRule>.Get();
            box.width = Width;
            box.height = Thickness;
            box.shift = Shift;
            return box;
		}

		public override void Draw (DrawingContext drawingContext, float scale, float x, float y)
		{
            base.Draw (drawingContext, scale, x, y);

			drawingContext.Draw (15, new Rect (
				(x) * scale, (y) * scale, width * scale, height * scale)
            , new Vector2[4]);
		}
            
        public override void Flush()
        {
            base.Flush();
            ObjPool<HorizontalRule>.Release(this);
        }
    }
}