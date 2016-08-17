using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
	// Atom representing other atom vertically centered with respect to axis.
	public class VerticalCenteredAtom : Atom
	{
		public static VerticalCenteredAtom Get (Atom atom)
		{
            var Atom = ObjPool<VerticalCenteredAtom>.Get();
			Atom.BaseAtom = atom;
            return Atom;
		}

        public Atom BaseAtom;

		public override Box CreateBox (TexStyle style)
		{
			var box = BaseAtom.CreateBox (style);
        
			// Centre box relative to horizontal axis.
			var totalHeight = box.height + box.depth;
			var axis = TEXPreference.main.GetPreference ("AxisHeight", style);
			box.shift = -(totalHeight / 2) - axis;

			return HorizontalBox.Get (box);
		}

        public override void Flush()
        {
            if(BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<VerticalCenteredAtom>.Release(this);
        }
	}
}