using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Atom representing big operator with optional limits.
namespace TexDrawLib
{
    public class BigOperatorAtom : Atom
    {
        private static Box ChangeWidth(Box box, float maxWidth)
        {
            // Centre specified box in new box of specified width, if necessary.
            if (Mathf.Abs(maxWidth - box.width) > TexUtility.FloatPrecision)
            {
                return HorizontalBox.Get(box, maxWidth, TexAlignment.Center);
            }
            else
                return box;
        }

        public static BigOperatorAtom Get(Atom baseAtom, Atom lowerLimitAtom, Atom upperLimitAtom)
        {
           return Get(baseAtom, lowerLimitAtom, upperLimitAtom, null);
        }

        public static BigOperatorAtom Get(Atom baseAtom, Atom lowerLimitAtom, Atom upperLimitAtom, bool? useVerticalLimits)
        {
            var atom = ObjPool<BigOperatorAtom>.Get();
            atom.Type = CharType.BigOperator;
            atom.BaseAtom = baseAtom;
            atom.LowerLimitAtom = lowerLimitAtom;
            atom.UpperLimitAtom = upperLimitAtom;
            atom.UseVerticalLimits = useVerticalLimits;
            return atom;
        }
 

        // Atom representing big operator.
        public Atom BaseAtom;

        // Atoms representing lower and upper limits.
        public Atom LowerLimitAtom;

        public Atom UpperLimitAtom;

        // True if limits should be drawn over and under the base atom; false if they should be drawn as scripts.
        public bool? UseVerticalLimits;

        public override Box CreateBox(TexStyle style)
        {
            if ((UseVerticalLimits.HasValue && !UseVerticalLimits.Value) ||
                (!UseVerticalLimits.HasValue && style >= TexStyle.Text))
            // Attach atoms for limits as scripts.
                return TexUtility.GetBox(ScriptsAtom.Get(BaseAtom, LowerLimitAtom, UpperLimitAtom), (style));

            // Create box for base atom.
            Box baseBox;
            float delta;

            if (BaseAtom is SymbolAtom && BaseAtom.Type == CharType.BigOperator)
            {
                // Find character of best scale for operator symbol.
                var opChar = TEXPreference.main.GetCharMetric(((SymbolAtom)BaseAtom).Name, style);
                if (style < TexStyle.Text && opChar.ch.nextLargerExist)
                    opChar = TEXPreference.main.GetCharMetric(opChar.ch.nextLarger, style);
                var charBox = CharBox.Get(style, opChar);
                charBox.shift = -(charBox.height + charBox.depth) / 2;
                baseBox = HorizontalBox.Get(charBox);

                delta = opChar.bearing;
            }
            else
            {
                baseBox = HorizontalBox.Get(BaseAtom == null ? StrutBox.Empty : BaseAtom.CreateBox(style));
                delta = 0;
            }

            // Create boxes for upper and lower limits.
	        Box  upperLimitBox, lowerLimitBox;
	        if(UpperLimitAtom is SymbolAtom && ((SymbolAtom)UpperLimitAtom).Character.extensionHorizontal)
	        	upperLimitBox = DelimiterFactory.CreateBoxHorizontal(((SymbolAtom)UpperLimitAtom).Name, baseBox.width, TexUtility.GetSuperscriptStyle(style));
	        else
	        	upperLimitBox = UpperLimitAtom == null ? null : UpperLimitAtom.CreateBox(TexUtility.GetSuperscriptStyle(style));
	        
	        if(LowerLimitAtom is SymbolAtom && ((SymbolAtom)LowerLimitAtom).Character.extensionHorizontal)
	        	lowerLimitBox = DelimiterFactory.CreateBoxHorizontal(((SymbolAtom)LowerLimitAtom).Name, baseBox.width, TexUtility.GetSubscriptStyle(style));
	        else
	        	lowerLimitBox = LowerLimitAtom == null ? null : LowerLimitAtom.CreateBox(TexUtility.GetSubscriptStyle(style));
           
            // Make all component boxes equally wide.
            var maxWidth = Mathf.Max(Mathf.Max(baseBox.width, upperLimitBox == null ? 0 : upperLimitBox.width),
                      lowerLimitBox == null ? 0 : lowerLimitBox.width);
            if (baseBox != null)
                baseBox = ChangeWidth(baseBox, maxWidth);
            if (upperLimitBox != null)
                upperLimitBox = ChangeWidth(upperLimitBox, maxWidth);
            if (lowerLimitBox != null)
                lowerLimitBox = ChangeWidth(lowerLimitBox, maxWidth);

            var resultBox = new VerticalBox();
            var opSpacing5 = TEXPreference.main.GetPreference("BigOpMargin", style);
            var kern = 0f;

            // Create and add box for upper limit.
            if (UpperLimitAtom != null)
            {
                resultBox.Add(StrutBox.Get(0, opSpacing5, 0, 0));
                upperLimitBox.shift = delta / 2;
                resultBox.Add(upperLimitBox);
                kern = Mathf.Max(TEXPreference.main.GetPreference("BigOpUpShift", style), 
                    TEXPreference.main.GetPreference("BigOpUpperGap", style) - upperLimitBox.depth);
                resultBox.Add(StrutBox.Get(0, kern, 0, 0));
            }

            // Add box for base atom.
            resultBox.Add(baseBox);

            // Create and add box for lower limit.
            if (LowerLimitAtom != null)
            {
                resultBox.Add(StrutBox.Get(0, Mathf.Max(TEXPreference.main.GetPreference("BigOpLowShift", style), 
                            TEXPreference.main.GetPreference("BigOpLowerGap", style) - lowerLimitBox.height), 0, 0));
                lowerLimitBox.shift = -delta / 2;
                resultBox.Add(lowerLimitBox);
                resultBox.Add(StrutBox.Get(0, opSpacing5, 0, 0));
            }

            // Adjust height and depth of result box.
            var baseBoxHeight = baseBox.height;
            var totalHeight = resultBox.height + resultBox.depth;
            if (upperLimitBox != null)
                baseBoxHeight += opSpacing5 + kern + upperLimitBox.height + upperLimitBox.depth;
            resultBox.height = baseBoxHeight;
            resultBox.depth = totalHeight - baseBoxHeight;
            TexUtility.CentreBox(resultBox, style);
            return resultBox;
        }

        public override void Flush()
        {
            if (BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            if (LowerLimitAtom != null)
            {
                LowerLimitAtom.Flush();
                LowerLimitAtom = null;
            }
            if (UpperLimitAtom != null)
            {
                UpperLimitAtom.Flush();
                UpperLimitAtom = null;
            }
            ObjPool<BigOperatorAtom>.Release(this);
        }
    }
}