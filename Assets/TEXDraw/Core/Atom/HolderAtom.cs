using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class HolderAtom : Atom
    {
        public static HolderAtom Get(Atom baseAtom, float Width, float Height)
        {
            var atom = ObjPool<HolderAtom>.Get();
            atom.BaseAtom = baseAtom;
            atom.width = Width;
            atom.height = Height;
            return atom;
        }


        public Atom BaseAtom;

        public float width = 0;
        public float height = 0;
        public TexAlignment align;

        public override Box CreateBox(TexStyle style)
        {
            if (BaseAtom == null)
                return StrutBox.Get(width, height, 0, 0);
            else
            {
                if (width == 0 && BaseAtom is SymbolAtom)
                    return VerticalBox.Get(DelimiterFactory.CreateBox(((SymbolAtom)BaseAtom).Name, height, style), height, align);
                else if (height == 0 && BaseAtom is SymbolAtom)
                    return HorizontalBox.Get(DelimiterFactory.CreateBoxHorizontal(((SymbolAtom)BaseAtom).Name, width, style), width, align);
                else
                    return VerticalBox.Get(HorizontalBox.Get(BaseAtom.CreateBox(style), width, align), height, align);
            }
        }

        public override void Flush()
        {
            if (BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<HolderAtom>.Release(this);
        }


    }
}
