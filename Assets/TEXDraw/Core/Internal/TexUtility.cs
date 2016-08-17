using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace TexDrawLib
{
	public static class TexUtility
	{
		// Few const for simple adjusting ------------------------------------------------------------------------------
		public const float FloatPrecision = 0.001f;
		
		// Preserved Dynamic Configurations ----------------------------------------------------------------------------
		public static float RenderSize;
		public static Color32 RenderColor;
        public static int RenderFont = -1;
            
		public static float spaceWidth
		{
			get	{ return TEXPreference.main.preferences["SpaceWidth"]; }
		}
        public static float spaceHeight
        {
            get { return TEXPreference.main.preferences["LineHeight"]; }
        }
        public static float glueRatio
		{
			get	{ return TEXPreference.main.preferences["GlueRatio"]; }
		}
		public static float lineThickness
		{
			get	{ return TEXPreference.main.preferences["LineThickness"]; }
		}

		// TexStyle manipulations (for different style, etc.) ----------------------------------------------------------
		public static float SizeFactor (TexStyle style)
		{
			if (style < TexStyle.Script)
				return 1f;
			else if (style < TexStyle.ScriptScript)
				return TEXPreference.main.preferences["ScriptFactor"];
			else
				return TEXPreference.main.preferences["NestedScriptFactor"];
		}

		public static TexStyle GetCrampedStyle (TexStyle Style)
		{
			return (int)Style % 2 == 1 ? Style : Style + 1;
		}

		public static TexStyle GetNumeratorStyle (TexStyle Style)
		{
			return Style + 2 - 2 * ((int)Style / 6);
		}

		public static TexStyle GetDenominatorStyle (TexStyle Style)
		{
			return (TexStyle)(2 * ((int)Style / 2) + 1 + 2 - 2 * ((int)Style / 6));
		}

		public static TexStyle GetRootStyle ()
		{
			return TexStyle.Script;
		}

		public static TexStyle GetSubscriptStyle (TexStyle Style)
		{
			return (TexStyle)(2 * ((int)Style / 4) + 4 + 1);
		}

		public static TexStyle GetSuperscriptStyle (TexStyle Style)
		{
			return (TexStyle)(2 * ((int)Style / 4) + 4 + ((int)Style % 2));
		}

        public static void CentreBox(Box box, TexStyle style)
        {
            float axis = TEXPreference.main.GetPreference("AxisHeight", style);
            box.shift = (box.height - box.depth) / 2 - axis;
        }

        public static Box GetBox (Atom atom, TexStyle style)
        {
            var box = atom.CreateBox(style);
            atom.Flush();
            return box;
        }

        static public List<T> GetRangePool<T> (this List<T> source, int index, int count)
        {
            List<T> list = ListPool<T>.Get();
            T[] a = new T[count];
            //list.AddRange
            source.CopyTo(index, a, 0, count);
            list.AddRange(a);
//            System.Array.Copy (source, index, list, 0, count);
            return list;
        }
    }
}