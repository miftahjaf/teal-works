using UnityEngine;
using System.Collections;

//Atom for Creating Diagonal Negate Line
namespace TexDrawLib
{
    public class NegateAtom : Atom
    {
        public int mode = 0;

        public float offsetM = 0;
        public float offsetP = 0;

        public static NegateAtom Get(Atom baseAtom, int Mode, string Offset)
        {
            var atom = ObjPool<NegateAtom>.Get();
            atom.BaseAtom = baseAtom;
            atom.mode = Mode;
            if (Offset != null) {
                int pos = Offset.IndexOf('-');
                if (pos < 0 || !float.TryParse(Offset.Substring(pos), out atom.offsetM))
                    atom.offsetM = 0;
                if (pos < 1 || !float.TryParse(Offset.Substring(0, pos), out atom.offsetP)) {
                    if (pos == 0 || !float.TryParse(Offset, out atom.offsetP))
                        atom.offsetP = 0;
                }
            } else {
                atom.offsetM = 0;
                atom.offsetP = 0;
            }
            return atom;
        }

        public Atom BaseAtom;

        public override Box CreateBox(TexStyle style)
        {
            if (BaseAtom == null)
                return StrutBox.Empty;
            float margin = TEXPreference.main.GetPreference("NegateMargin", style) / 2;
            float thick = TEXPreference.main.GetPreference("LineThickness", style) / 2;
            var baseBox = BaseAtom.CreateBox(style);
            var result = ObjPool<HorizontalBox>.Get();
			         
            var negateBox = StrikeBox.Get(baseBox.height, baseBox.width, baseBox.depth,
                                margin, thick, (StrikeBox.StrikeMode)mode, offsetM, offsetP);
            negateBox.shift = baseBox.shift;
            result.Add(negateBox);
            result.Add(StrutBox.Get(-baseBox.width, 0, 0, 0));
            //           if (mode > 1)
            //             result.Add(0, StrutBox.Get(margin, 0, 0, 0));
            result.Add(baseBox);
            //     if (mode > 1)
            //           result.Add(StrutBox.Get(margin, 0, 0, 0));
            return result;
        }

        public override void Flush()
        {
            if (BaseAtom != null) {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<NegateAtom>.Release(this);
        }
    }
}