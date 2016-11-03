using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class AttrLinkAtom : Atom
    {

        public static AttrLinkAtom Get(Atom baseAtom, string metaKey, bool underLink)
        {
            var atom = ObjPool<AttrLinkAtom>.Get();
            atom.BaseAtom = baseAtom;
            atom.MetaKey = metaKey;
            atom.UnderLink = underLink;
            return atom;
        }

        public Atom BaseAtom;

        public string MetaKey;

        public bool UnderLink;

        public override Box CreateBox(TexStyle style)
        {
            if (BaseAtom == null)
                return StrutBox.Empty;
            else {
                Box box;
                if (UnderLink) {
                    var baseBox = BaseAtom.CreateBox(style);
                    float margin = TEXPreference.main.GetPreference("NegateMargin", style) / 2;
                    float thick = TEXPreference.main.GetPreference("LineThickness", style) / 2;

                    box = HorizontalBox.Get(baseBox);
                    box.Add(StrutBox.Get(-box.width, 0, 0, 0));
                    box.Add(StrikeBox.Get(baseBox.height, baseBox.width, baseBox.depth,
                            margin, thick, StrikeBox.StrikeMode.underline, 0, 0));
                    return AttrLinkBox.Get(box, MetaKey);
                } else
                    return AttrLinkBox.Get(BaseAtom.CreateBox(style), MetaKey);
            }
        }

        public override void Flush()
        {
            if (BaseAtom != null) {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<AttrLinkAtom>.Release(this);
        }

    }
}
