using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
	// Atom representing fraction, with or without separation line.
	public class FractionAtom : Atom
	{

		public static FractionAtom Get (Atom Numerator, Atom Denominator)
		{
            return Get(Numerator, Denominator, TexUtility.lineThickness, TexAlignment.Center, TexAlignment.Center);
		}

		public static FractionAtom Get (Atom Numerator, Atom Denominator, bool HasLine)
		{
            return Get(Numerator, Denominator, HasLine?TexUtility.lineThickness:0, TexAlignment.Center, TexAlignment.Center);
		}

		public static FractionAtom Get (Atom Numerator, Atom Denominator, float LineThickness)
		{
            return Get (Numerator, Denominator, LineThickness, TexAlignment.Center, TexAlignment.Center);
        }

		public static FractionAtom Get (Atom Numerator, Atom Denominator, bool HasLine, 
			TexAlignment NumeratorAlignment, TexAlignment DenominatorAlignment)
		{
            return Get (Numerator, Denominator, HasLine?TexUtility.lineThickness:0, NumeratorAlignment, DenominatorAlignment);
        }

		public static FractionAtom Get (Atom Numerator, Atom Denominator, float LineThickness, 
			TexAlignment NumeratorAlignment, TexAlignment DenominatorAlignment)
		{
            var atom = ObjPool<FractionAtom>.Get();
            atom.Type = CharTypeInternal.Inner;
            atom.numerator = Numerator;
            atom.denominator = Denominator;
            atom.numeratorAlignment = NumeratorAlignment;
            atom.denominatorAlignment = DenominatorAlignment;
            atom.lineThickness = LineThickness;
            return atom;
		}

        public Atom numerator;

        public Atom denominator;

		private TexAlignment numeratorAlignment;
		private TexAlignment denominatorAlignment;
		private float lineThickness;

		public override Box CreateBox (TexStyle style)
		{

			float lineHeight = lineThickness * TexUtility.SizeFactor(style);

			// Create boxes for numerator and demoninator atoms, and make them of equal width.
			var numeratorBox = numerator == null ? StrutBox.Empty :
            numerator.CreateBox (TexUtility.GetNumeratorStyle (style));
			var denominatorBox = denominator == null ? StrutBox.Empty :
            denominator.CreateBox (TexUtility.GetDenominatorStyle (style));

            float maxWidth = (numeratorBox.width < denominatorBox.width ? denominatorBox.width : numeratorBox.width) + TEXPreference.main.GetPreference ("FractionMargin", style);
            numeratorBox = HorizontalBox.Get (numeratorBox, maxWidth, numeratorAlignment);
            denominatorBox = HorizontalBox.Get (denominatorBox, maxWidth, denominatorAlignment);

			// Calculate preliminary shift-up and shift-down amounts.
			float shiftUp, shiftDown;
			if (style < TexStyle.Text)
			{
                if (lineHeight > 0)
                    shiftUp = TEXPreference.main.GetPreference ("NumeratorShift", style);
                else
                    shiftUp = TEXPreference.main.GetPreference ("NumeratorShiftNoLine", style);
                shiftDown = TEXPreference.main.GetPreference ("DenominatorShift", style);
			}
			else
			{
				shiftDown = TEXPreference.main.GetPreference ("DenominatorNarrow", style);
                shiftUp = TEXPreference.main.GetPreference ("NumeratorNarrow", style);
            }

			// Create result box.
			var resultBox = VerticalBox.Get ();

			// add box for numerator.
			resultBox.Add (numeratorBox);

			// Calculate clearance and adjust shift amounts.
			//var axis = TEXPreference.main.GetPreference ("AxisHeight", style);

			if (lineHeight > 0)
			{
				// Draw fraction line.

				// Calculate clearance amount.
				float clearance = lineHeight;

				// Adjust shift amounts.
				var kern1 = shiftUp - numeratorBox.depth;
				var kern2 = - (denominatorBox.height - shiftDown);
				var delta1 = clearance - kern1;
				var delta2 = clearance - kern2;
				if (delta1 > 0)
				{
					shiftUp += delta1;
					kern1 += delta1;
				}
				if (delta2 > 0)
				{
					shiftDown += delta2;
					kern2 += delta2;
				}

				resultBox.Add (StrutBox.Get (0, kern1, 0, 0));
				resultBox.Add (HorizontalRule.Get (lineHeight, numeratorBox.width, 0));
				resultBox.Add (StrutBox.Get (0, kern2, 0, 0));
			}
			else
			{
				// Do not draw fraction line.

				// Calculate clearance amount.
				float clearance = lineHeight;

				// Adjust shift amounts.
				float kern = shiftUp - numeratorBox.depth - (denominatorBox.height - shiftDown);
				float delta = (clearance - kern);
				if (delta > 0)
				{
					shiftUp += delta;
					shiftDown += delta;
					kern += 2f * delta;
				}

				resultBox.Add (StrutBox.Get (0, kern, 0, 0)); 
			} 

			// add box for denominator.
			resultBox.Add (denominatorBox);

			// Adjust height and depth of result box.
			resultBox.height = shiftUp + numeratorBox.height;
			resultBox.depth = shiftDown + denominatorBox.depth;

            TexUtility.CentreBox(resultBox, style);
			return resultBox;
		}

        public override void Flush()
        {
            if(numerator != null)
            {
                numerator.Flush();
                numerator = null;
            }
            if(denominator != null)
            {
                denominator.Flush();
                denominator = null;
            }
            ObjPool<FractionAtom>.Release(this);
        }
    }
}