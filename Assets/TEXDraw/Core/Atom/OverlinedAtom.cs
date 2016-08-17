using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Atom representing other atom with horizontal rule above it.
namespace TexDrawLib
{
	public class OverlinedAtom : Atom
	{
		public static OverlinedAtom Get (Atom baseAtom)
		{
            var atom = ObjPool<OverlinedAtom>.Get();

			atom.Type = CharType.Ordinary;
			atom.BaseAtom = baseAtom;
            return atom;
		}

        public Atom BaseAtom;

		public override Box CreateBox (TexStyle style)
		{
			// Create box for base atom, in cramped style.
			var baseBox = BaseAtom == null ? StrutBox.Empty : BaseAtom.CreateBox (TexUtility.GetCrampedStyle (style));

			// Create result box.
			var defaultLineThickness = TEXPreference.main.GetPreference ("LineThickness", style);
			var resultBox = OverBar.Get (baseBox, 3 * defaultLineThickness, defaultLineThickness);

			// Adjust height and depth of result box.
			resultBox.height = baseBox.height + 5 * defaultLineThickness;
			resultBox.depth = baseBox.depth;

			return resultBox;
		}

        public override void Flush()
        {
            if(BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<OverlinedAtom>.Release(this);
        }

	}
}