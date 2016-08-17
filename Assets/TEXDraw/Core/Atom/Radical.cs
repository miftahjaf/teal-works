using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TexDrawLib
{
	// Atom representing radical (nth-root) construction.
	public class Radical : Atom
	{
		private const string rootSymbol = "surdsign";

		private const float scale = 0.65f;

		public static Radical Get (Atom baseAtom)
		{
            return Get (baseAtom, null);
		}

		public static Radical Get (Atom baseAtom, Atom degreeAtom)
		{
            var atom = ObjPool<Radical>.Get();
            atom.Type = CharTypeInternal.Inner;
            atom.BaseAtom = baseAtom;
            atom.DegreeAtom = degreeAtom;
            return atom;
		}

        public Atom BaseAtom;
        public Atom DegreeAtom;

		public override Box CreateBox (TexStyle style)
		{
			// Calculate minimum clearance amount.
            if(BaseAtom == null)
                return StrutBox.Empty;
			float clearance;
			var lineThickness = TEXPreference.main.GetPreference ("LineThickness", style);
			clearance = lineThickness;
		
            // Create box for base atom, in cramped style.
			var baseBox = BaseAtom.CreateBox (TexUtility.GetCrampedStyle(style));

			// Create box for radical sign.
			var totalHeight = baseBox.height + baseBox.depth;
			var radicalSignBox = DelimiterFactory.CreateBox (rootSymbol, totalHeight + clearance + lineThickness,
				                          style);

			// Add half of excess height to clearance.
            lineThickness = Mathf.Max(radicalSignBox.height, lineThickness);
            clearance = radicalSignBox.totalHeight - totalHeight - lineThickness * 2;

			// Create box for square-root containing base box.
            TexUtility.CentreBox(radicalSignBox, style);
            var overBar = OverBar.Get (baseBox, clearance, lineThickness);
            TexUtility.CentreBox(overBar, style);
			var radicalContainerBox = HorizontalBox.Get (radicalSignBox);
			radicalContainerBox.Add (overBar);

			// If atom is simple radical, just return square-root box.
			if (DegreeAtom == null)
				return radicalContainerBox;

			// Atom is complex radical (nth-root).

			// Create box for root atom.
			var rootBox = DegreeAtom.CreateBox (TexUtility.GetRootStyle());
			var bottomShift = scale * (radicalContainerBox.height + radicalContainerBox.depth);
			rootBox.shift = radicalContainerBox.depth - rootBox.depth - bottomShift;

			// Create result box.
            var resultBox = HorizontalBox.Get();

			// Add box for negative kern.
            var negativeKern = SpaceAtom.Get (-((radicalSignBox.width) / 2f), 0, 0).CreateBox (TexStyle.Display);
			var xPos = rootBox.width + negativeKern.width;
			if (xPos < 0)
				resultBox.Add (StrutBox.Get (-xPos, 0, 0, 0));

			resultBox.Add (rootBox);
			resultBox.Add (negativeKern);
			resultBox.Add (radicalContainerBox);

			return resultBox;
		}

        public override void Flush()
        {
            if(BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            if(DegreeAtom != null)
            {
                DegreeAtom.Flush();
                DegreeAtom = null;
            }
            ObjPool<Radical>.Release(this);
        }

	}
}