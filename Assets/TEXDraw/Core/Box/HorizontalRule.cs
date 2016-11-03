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
		public static HorizontalRule Get (float Height, float Width, float Shift)
        {
            var box = ObjPool<HorizontalRule>.Get();
            box.width = Width;
	        box.height = Height;
            box.shift = Shift;
            return box;
        }
		
		public static HorizontalRule Get (float Height, float Width, float Shift, float Depth)
		{
			var box = ObjPool<HorizontalRule>.Get();
			box.width = Width;
			box.height = Height;
			box.depth = Depth;
			box.shift = Shift;
			return box;
		}
		

		public override void Draw (DrawingContext drawingContext, float scale, float x, float y)
		{
            base.Draw (drawingContext, scale, x, y);
            Vector2 z = Vector2.zero;
			drawingContext.Draw (TexUtility.blockFontIndex, new Vector2 (
				(x) * scale, (y - depth) * scale), new Vector2(width * scale, totalHeight * scale)
                , z, z, z, z);
		}
            
        public override void Flush()
        {
            base.Flush();
            ObjPool<HorizontalRule>.Release(this);
        }
    }
}