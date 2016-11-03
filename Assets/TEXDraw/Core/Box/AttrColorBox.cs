using UnityEngine;
using System.Collections;

namespace TexDrawLib
{

    internal class AttrColorBox : Box
    {

        public static AttrColorBox Get(Color RenderColor, int MixMode, AttrColorBox EndBox)
        {
            var box = ObjPool<AttrColorBox>.Get();
            box.renderColor = RenderColor;
            box.mixMode = MixMode;
            box.endBox = EndBox;
            return box;
        }

        public Color renderColor;

        //If null, then this is the end box
        public AttrColorBox endBox;

        //0 = Overwrite, 1 = Alpha-Multiply, 2 = RGBA-Multiply
        public int mixMode;

        public override void Draw(DrawingContext drawingContext, float scale, float x, float y)
        {
            var oldColor = TexUtility.RenderColor;
            var newColor = endBox != null ? ProcessFinalColor(oldColor) : (Color32)renderColor;

            if (endBox != null)
                endBox.renderColor = oldColor;

            TexUtility.RenderColor = newColor;
        }

        Color32 ProcessFinalColor(Color32 old)
        {
            switch (mixMode) {
                case 1:
                    return TexUtility.MultiplyAlphaOnly(renderColor, old.a / 255f);
                case 2:
                    return TexUtility.MultiplyColor(old, renderColor);
            }
            return renderColor;
        }

        public override void Flush()
	    {
            base.Flush();
		    endBox = null;
		    ObjPool<AttrColorBox>.Release(this);
        }
    }
}