using UnityEngine;
using System.Collections;

namespace TexDrawLib
{

	internal class AttrColorBox : Box
	{

		public static AttrColorBox Get(Box BaseBox, Color RenderColor)
		{
            var box = ObjPool<AttrColorBox>.Get();
			box.baseBox = BaseBox;
            box.width = BaseBox.width;
            box.height = BaseBox.height;
            box.depth = BaseBox.depth;
            box.renderColor = RenderColor;
            return box;
		}

		public Color renderColor;

		public Box baseBox;

		public override void Draw (DrawingContext drawingContext, float scale, float x, float y)
		{
			base.Draw (drawingContext, scale, x, y);
			Color tmpC = TexUtility.RenderColor;
			TexUtility.RenderColor = renderColor;
			baseBox.Draw(drawingContext, scale, x, y);
			TexUtility.RenderColor = tmpC;
		}

        public override void Flush()
        {
            base.Flush();
            if(baseBox != null)
            {
                baseBox.Flush();
                baseBox = null;
            }
            ObjPool<AttrColorBox>.Release(this);
        }
	}
}