using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Atom representing single character in specific text style.
namespace TexDrawLib
{
	public class CharAtom : CharSymbol
	{
 
		public static CharAtom Get (char character, int textStyle)
		{
            var atom = ObjPool<CharAtom>.Get();
			atom.Character = character;
			atom.TextStyle = textStyle;
            return atom;
		}

		public static CharAtom Get (char character)
		{
            return Get(character, TexUtility.RenderFont);
		}

        public char Character;

        public int TextStyle;

		public override Box CreateBox (TexStyle style)
		{
			var charInfo = GetChar(style);
			return CharBox.Get (style, charInfo);
		}

		public TexCharMetric GetChar (TexStyle style)
		{
			if (TextStyle == -1)
				return TEXPreference.main.GetCharMetric (Character, style);
			else
				return TEXPreference.main.GetCharMetric (TextStyle, Character, style);
		}

		public override TexChar GetChar ()
		{
			return GetChar(TexStyle.Display).ch;
		}

        public override void Flush()
        {
            Character = default(char);
            TextStyle = -1;
            ObjPool<CharAtom>.Release(this);
        }


	}
}