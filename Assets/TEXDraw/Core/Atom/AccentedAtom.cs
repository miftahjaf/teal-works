using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TexDrawLib
{
    // Atom representing base atom with accent above it.
    public class AccentedAtom : Atom
    {
        public static AccentedAtom Get(Atom baseAtom, string accentName)
        {
            var atom = ObjPool<AccentedAtom>.Get();
            atom.BaseAtom = baseAtom;
            if(atom.BaseAtom == null)
                atom.BaseAtom = SpaceAtom.Get(0, 0, 0);
            atom.AccentAtom = SymbolAtom.GetAtom(accentName);
            return atom;
        }

        public static AccentedAtom Get(Atom baseAtom, TexFormula accent)
        {
            var atom = ObjPool<AccentedAtom>.Get();
            var rootSymbol = accent.RootAtom as SymbolAtom;
            atom.AccentAtom = (SymbolAtom)rootSymbol;
            atom.BaseAtom = baseAtom;
            if(atom.BaseAtom == null)
                atom.BaseAtom = SpaceAtom.Get(0, 0, 0);
            return atom;
        }

        public static AccentedAtom Get(Atom baseAtom, Atom accent)
        {
            var atom = ObjPool<AccentedAtom>.Get();
            var rootSymbol = accent;
            atom.AccentAtom = (SymbolAtom)rootSymbol;
            atom.BaseAtom = baseAtom;
            if(atom.BaseAtom == null)
                atom.BaseAtom = SpaceAtom.Get(0, 0, 0);
            return atom;
        }

        // Atom over which accent symbol is placed.
        public Atom BaseAtom;
        // Atom representing accent symbol to place over base atom.
        public SymbolAtom AccentAtom;

        public override Box CreateBox(TexStyle style)
        {
	
            // Create box for base atom.
            var baseBox = BaseAtom == null ? StrutBox.Empty : BaseAtom.CreateBox(TexUtility.GetCrampedStyle(style));
            // Find character of best scale for accent symbol.
            TexCharMetric accentChar = TEXPreference.main.GetCharMetric(AccentAtom.Name, style);
            while (accentChar.ch.nextLargerExist)
            {
                var nextLargerChar = TEXPreference.main.GetCharMetric(accentChar.ch.nextLarger, style);
                if (nextLargerChar.width > baseBox.width)
                    break;
                accentChar = nextLargerChar;
            }

            var resultBox = ObjPool<VerticalBox>.Get();

            // Create and add box for accent symbol.
            Box accentBox = ObjPool<HorizontalBox>.Get();
            resultBox.Add(HorizontalBox.Get(CharBox.Get(style, accentChar)));

            var delta = Mathf.Min(-accentChar.depth, baseBox.height);
            resultBox.Add(StrutBox.Get(0, -delta + TEXPreference.main.GetPreference("OverUnderMargin", style), 0, 0));

            // Centre and add box for base atom. Centre base box and accent box with respect to each other.
            var boxWidthsDiff = (baseBox.width - accentBox.width) / 2;
            accentBox.shift = Mathf.Max(boxWidthsDiff, 0);
            if (boxWidthsDiff < 0)
            {
                baseBox = HorizontalBox.Get(baseBox, accentBox.width, TexAlignment.Center);
            }
            resultBox.Add(baseBox);

            // Adjust height and depth of result box.
            var depth = baseBox.depth;
            var totalHeight = resultBox.height + resultBox.depth;
            resultBox.depth = depth;
            resultBox.height = totalHeight - depth;

            return resultBox;
        }

        public override void Flush()
        {
            if (BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            if (AccentAtom != null)
            {
                AccentAtom.Flush();
                AccentAtom = null;
            }
            ObjPool<AccentedAtom>.Release(this);
        }
    }
}