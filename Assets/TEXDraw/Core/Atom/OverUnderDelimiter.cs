using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TexDrawLib
{
	// Atom representing other atom with delimeter and script atoms over or under it.
	public class OverUnderDelimiter : Atom
	{
		private static float GetMaxWidth (Box baseBox, Box delimeterBox, Box scriptBox)
		{
			var maxWidth = Mathf.Max (baseBox.width, delimeterBox.height + delimeterBox.depth);
			if (scriptBox != null)
				maxWidth = Mathf.Max (maxWidth, scriptBox.width);
			return maxWidth;
		}

		public static OverUnderDelimiter Get(Atom baseAtom, Atom script, SymbolAtom symbol, float kern, bool over)
		{
            var atom = ObjPool<OverUnderDelimiter>.Get();
			atom.Type = CharTypeInternal.Inner;
            atom.BaseAtom = baseAtom;
            atom.Script = script;
            atom.Symbol = symbol;
            atom.Kern = SpaceAtom.Get (0, kern, 0);
            atom.Over = over;
            return atom;
		} 

        public Atom BaseAtom;

        private Atom Script;

        private SymbolAtom Symbol;

		// Kern between delimeter symbol and script.
        private SpaceAtom Kern;

		// True to place delimeter symbol Over base; false to place delimeter symbol under base.
        public bool Over;

		public override Box CreateBox (TexStyle style)
		{
			// Create boxes for base, delimeter, and script atoms.
			var baseBox = BaseAtom == null ? StrutBox.Empty : BaseAtom.CreateBox (style);
			var delimeterBox = DelimiterFactory.CreateBox (Symbol.Name, baseBox.width, style);
			var scriptBox = Script == null ? null : Script.CreateBox (Over ? 
            TexUtility.GetSuperscriptStyle (style) : TexUtility.GetSubscriptStyle (style));

			// Create centered horizontal box if any box is smaller than maximum width.
			var maxWidth = GetMaxWidth (baseBox, delimeterBox, scriptBox);
			if (Mathf.Abs (maxWidth - baseBox.width) > TexUtility.FloatPrecision)
				baseBox = HorizontalBox.Get (baseBox, maxWidth, TexAlignment.Center);
			if (Mathf.Abs (maxWidth - delimeterBox.height - delimeterBox.depth) > TexUtility.FloatPrecision)
				delimeterBox = VerticalBox.Get (delimeterBox, maxWidth, TexAlignment.Center);
			if (scriptBox != null && Mathf.Abs (maxWidth - scriptBox.width) > TexUtility.FloatPrecision)
				scriptBox = HorizontalBox.Get (scriptBox, maxWidth, TexAlignment.Center);

			return OverUnderBox.Get (baseBox, delimeterBox, scriptBox, Kern.CreateBox (style).height, Over);
		}

        public override void Flush()
        {
            if(BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            if(Script != null)
            {
                Script.Flush();
                Script = null;
            }
            ObjPool<OverUnderDelimiter>.Release(this);
        }

	}
}