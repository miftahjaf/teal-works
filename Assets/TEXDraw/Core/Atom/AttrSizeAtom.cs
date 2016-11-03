using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class AttrSizeAtom : Atom
    {
              
        public static AttrSizeAtom Get(Atom baseAtom, string sizeStr)
        {
            var atom = ObjPool<AttrSizeAtom>.Get();
            atom.BaseAtom = baseAtom;
            if (sizeStr != null) {
                int pos = sizeStr.IndexOf('-');
                if (pos < 0 || !float.TryParse(sizeStr.Substring(pos), out atom.Offset))
                    atom.Offset = 0;
                if (pos < 1 || !float.TryParse(sizeStr.Substring(0, pos), out atom.Size)) {
                    if (pos == 0 || !float.TryParse(sizeStr, out atom.Size))
                        atom.Size = 1;
                }
            } else {
                atom.Size = 1;
                atom.Offset = 0;
            }
            return atom;
        }

        public Atom BaseAtom;

        public float Size;

        public float Offset;

        public override Box CreateBox(TexStyle style)
        {
            if (BaseAtom == null)
                return StrutBox.Empty;
            else {
                var oldSize = TexUtility.RenderSizeFactor;
                TexUtility.RenderSizeFactor = Size;
                var box = BaseAtom.CreateBox(style);
                box.shift += Offset;
                TexUtility.RenderSizeFactor = oldSize;
                return box;
            }
        }

        public override void Flush()
        {
            if (BaseAtom != null) {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<AttrSizeAtom>.Release(this);
        }
    }
}

