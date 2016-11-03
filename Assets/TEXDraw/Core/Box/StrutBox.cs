using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
	// Box representing whitespace.
	public class StrutBox : Box
	{
        private static readonly StrutBox emptyStrutBox = StrutBox.Get (0, 0, 0, 0);

		public static StrutBox Empty
		{
			get 
            { 
                return emptyStrutBox; 
            }
		}

        public static StrutBox EmptyLine
        {
            get 
            { 
                return StrutBox.Get(0, TexUtility.spaceHeight, 0, 0); 
            }
        }


		public static StrutBox Get (float Width, float Height, float Depth, float Shift)
		{
            var box = Get();
			box.width = Width;
            box.height = Height;
            box.depth = Depth;
            box.shift = Shift;
            box.policy = StrutPolicy.Misc;
            return box;
            }

        public static StrutBox Get (float Width, float Height, float Depth, float Shift, StrutPolicy Policy)
        {
            var box = Get();
            box.width = Width;
            box.height = Height;
            box.depth = Depth;
            box.shift = Shift;
            box.policy = Policy;
            return box;
        }

        public static StrutBox Get()
        {
            return ObjPool<StrutBox>.Get();
        }

        public StrutPolicy policy;

		public override void Draw (DrawingContext drawingContext, float scale, float x, float y)
        {
            base.Draw (drawingContext, scale, x, y);
		}
        public override void Flush()
        {
            base.Flush();
           ObjPool<StrutBox>.Release(this);
        }
 	}
}