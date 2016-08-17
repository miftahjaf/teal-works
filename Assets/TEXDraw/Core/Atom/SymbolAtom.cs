using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
	/// Atom representing symbol (non-alphanumeric character).
	public class SymbolAtom : CharSymbol
	{
		public static SymbolAtom GetAtom (string name)
		{
            return SymbolAtom.Get(TEXPreference.main.GetChar(name));
		}

		public static SymbolAtom Get (TexChar ch)
		{
            var atom = ObjPool<SymbolAtom>.Get();
			atom.Type = ch.type;
            atom.Name = ch.symbolName;
            atom.IsDelimeter = ch.type == CharType.OpenDelimiter || ch.type == CharType.CloseDelimiter || ch.type == CharType.Relation || ch.type == CharType.Arrow;
            return atom;
        }

		public static SymbolAtom Get (string name, CharType type, bool isDelimeter, string pair)
		{
            var atom = ObjPool<SymbolAtom>.Get();
            atom.Type = type;
            atom.Name = name;
            atom.IsDelimeter = isDelimeter;
            atom.PairName = pair;
            return atom;
		}

        public bool IsDelimeter;

        public string Name;

        public string PairName;

		public override Box CreateBox (TexStyle style)
		{
			return CharBox.Get (style, TEXPreference.main.GetCharMetric (Name, style));
		}

		public override TexChar GetChar ()
		{
			return TEXPreference.main.GetChar (Name);
		}

        public override void Flush()
        {
            ObjPool<SymbolAtom>.Release(this);
        }
	}
}