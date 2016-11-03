using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class AttrShiftAtom : Atom
    {

        public static AttrShiftAtom Get(Atom Base, string Offset)
        {
            var atom = ObjPool<AttrShiftAtom>.Get();
            atom.baseAtom = Base;
            float s;
            if(float.TryParse(Offset, out s))
                atom.offset = s;
            else
                atom.offset = 0;
            return atom;
        }

        public Atom baseAtom;
        public float offset;

        public override Box CreateBox(TexStyle style)
        {
            var box = baseAtom.CreateBox(style);
            box.shift -= offset;
            return HorizontalBox.Get(box);
        }

        public override void Flush()
        {
            base.Flush();
            if (baseAtom != null) {
                baseAtom.Flush();
                baseAtom = null;
            }
            ObjPool<AttrShiftAtom>.Release(this);
        }
    }
}