using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
	public class AttrColorAtom : Atom
	{
		Color color = Color.white;
		public static AttrColorAtom Get (Atom baseAtom, string colorStr)
		{
            var atom = ObjPool<AttrColorAtom>.Get();
			atom.BaseAtom = baseAtom;
			ColorUtility.TryParseHtmlString(colorStr, out atom.color);
            return atom;
		}

        public Atom BaseAtom;

		public override Box CreateBox (TexStyle style)
		{
            if(BaseAtom == null)
                return StrutBox.Empty;
            else
            {
                return AttrColorBox.Get(BaseAtom.CreateBox(style), color);
            }
		}

        public override void Flush()
        {
            if(BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<AttrColorAtom>.Release(this);
        }
	}
}
