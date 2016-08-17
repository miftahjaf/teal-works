using UnityEngine;
using System.Collections;

//Atom for Creating Diagonal Negate Line
namespace TexDrawLib
{
	public class NegateAtom : Atom
	{
		public int mode = 0;

		public static NegateAtom Get (Atom baseAtom, int Mode)
		{
            var atom = ObjPool<NegateAtom>.Get();
			atom.BaseAtom = baseAtom;
            atom.mode = Mode;
            return atom;
		}

        public Atom BaseAtom;

		public override Box CreateBox (TexStyle style)
		{
            if(BaseAtom == null)
                return StrutBox.Empty;
			float margin = TEXPreference.main.GetPreference ("NegateMargin", style) / 2;
			float thick = TEXPreference.main.GetPreference ("LineThickness", style) / 2;
			var baseBox = BaseAtom.CreateBox (style);
            var result = ObjPool<HorizontalBox>.Get();
            result.Add(baseBox);
			if(mode > 1)
				result.Add(0, StrutBox.Get (margin, 0, 0, 0));
			result.Add (StrutBox.Get (-baseBox.width, 0, 0, 0));

			var negateBox = StrikeBox.Get (baseBox.height, baseBox.width, baseBox.depth,
				margin, thick, (StrikeBox.StrikeMode)mode);
			result.Add (negateBox);
			if(mode > 1)
				result.Add(StrutBox.Get (margin, 0, 0, 0));
			return result;
		}

        public override void Flush()
        {
            if(BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<NegateAtom>.Release(this);
        }
	}
}