using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Atom representing single character that can be marked as text symbol.
namespace TexDrawLib
{
	public abstract class CharSymbol : Atom
	{
		public CharSymbol ()
		{
			IsTextSymbol = false;
		}

		public bool IsTextSymbol
		{
			get;
			set;
		}

		public abstract TexChar GetChar ();
	}
}