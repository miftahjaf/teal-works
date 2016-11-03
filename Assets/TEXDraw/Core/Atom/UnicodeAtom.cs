using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class UnicodeAtom : Atom
    {
        public static UnicodeAtom Get (int FontIndex, char c)
        {
            var atom = ObjPool<UnicodeAtom>.Get();
            atom.fontIndex = FontIndex;
            atom.charIndex = c;
            return atom;
        }

        public int fontIndex;
        public char charIndex;

        public override Box CreateBox(TexStyle style)
        {
            CharacterInfo ch;
            Font f =TEXPreference.main.fontData[fontIndex].font;
            f.RequestCharactersInTexture(new string(charIndex, 1));
            f.GetCharacterInfo(charIndex, out ch);
            return UnicodeBox.Get(style, fontIndex, ch, TexUtility.SizeFactor(style));                
        }

        public override void Flush()
        {
            ObjPool<UnicodeAtom>.Release(this);
        }
    }
}
