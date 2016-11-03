using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class AttrColorAtom : Atom
    {
		      
        public static AttrColorAtom Get(string colorStr, int mix, out AttrColorAtom endBlock)
        {
            var atom = ObjPool<AttrColorAtom>.Get();
            endBlock = ObjPool<AttrColorAtom>.Get();
            atom.EndAtom = endBlock;
            atom.Mix = mix;
            endBlock.Mix = mix;
            if (colorStr == null)
                return atom;
            if (colorStr.Length == 1)
                colorStr = "#" + new string(colorStr[0], 3);
            if (!ColorUtility.TryParseHtmlString(colorStr, out atom.color))
            if (!ColorUtility.TryParseHtmlString("#" + colorStr, out atom.color))
                atom.color = Color.white;
            endBlock.color = atom.color;
            return atom;
        }

        public AttrColorAtom EndAtom;

        public int Mix;

        internal AttrColorBox ProcessedEndBox;

        Color color = Color.white;

        public override Box CreateBox(TexStyle style)
        {
            if (ProcessedEndBox != null) {
                if (ProcessedEndBox.IsFlushed)
                    ProcessedEndBox = null;
                else
                    return ProcessedEndBox;
            }
            ProcessedEndBox = AttrColorBox.Get(color, Mix, EndAtom == null ? null : (AttrColorBox)EndAtom.CreateBox(style));
            return ProcessedEndBox;
            
        }

        public override void Flush()
        {
            EndAtom = null;
            //ProcessedEndBox = null;
            ObjPool<AttrColorAtom>.Release(this);
        }
    }
}
