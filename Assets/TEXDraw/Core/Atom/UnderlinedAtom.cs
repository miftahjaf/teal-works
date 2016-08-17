using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
	// Atom representing other atom that is underlined.
	public class UnderlinedAtom : Atom
	{
		public static UnderlinedAtom Get (Atom baseAtom)
		{
            var atom = ObjPool<UnderlinedAtom>.Get();
			atom.Type = CharType.Ordinary;
            atom.BaseAtom = baseAtom;
            return atom;
		}

        public Atom BaseAtom;

		public override Box CreateBox (TexStyle style)
		{
			var defaultLineThickness =TEXPreference.main.GetPreference ("LineThickness", style);

			// Create box for base atom.
			var baseBox = BaseAtom == null ? StrutBox.Empty : BaseAtom.CreateBox (style);

			// Create result box.
            var resultBox = VerticalBox.Get();
			resultBox.Add (baseBox);
            resultBox.Add (StrutBox.Get (0, 3 * defaultLineThickness, 0, 0));
            resultBox.Add (HorizontalRule.Get (defaultLineThickness, baseBox.width, 0));

			resultBox.depth = baseBox.depth + 5 * defaultLineThickness;
			resultBox.height = baseBox.height;

			return resultBox;
		}

        public override void Flush()
        {
            if(BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
          ObjPool<UnderlinedAtom>.Release(this);
        }

	}
}