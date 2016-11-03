using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class AttrLinkBox : Box
    {
        public string metaKey;
        public Box baseBox;

        public static AttrLinkBox Get(Box BaseBox, string MetaKey)
        {
            var box = ObjPool<AttrLinkBox>.Get();
            box.metaKey = MetaKey;
            box.baseBox = BaseBox;
            box.width = BaseBox.width;
            box.height = BaseBox.height;
            box.depth = BaseBox.depth;
            return box;
        }

        public override void Draw(DrawingContext drawingContext, float scale, float x, float y)
        {
	        base.Draw(drawingContext, scale, x, y);
	        float padding = TEXPreference.main.GetPreference("MatrixMargin");
	        
	        var tint = drawingContext.DrawLink(
		        new Rect((x - padding / 2f) * scale, (y - depth - padding / 2f) * scale, (width + padding) * scale, (totalHeight + padding) * scale), metaKey);
            if (tint == TexUtility.white)
                baseBox.Draw(drawingContext, scale, x, y);
            else {
                var oldColor = TexUtility.RenderColor;
                var newColor = TexUtility.MultiplyColor(oldColor, tint);

                TexUtility.RenderColor = newColor;
                baseBox.Draw(drawingContext, scale, x, y);
                TexUtility.RenderColor = oldColor;
            }
        }

        public override void Flush()
        {
            base.Flush();
            if (baseBox != null) {
                baseBox.Flush();
                baseBox = null;
            }
            ObjPool<AttrLinkBox>.Release(this);
        }
    }
}
