using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TexDrawLib
{
	// Atom representing whitespace.
	public class SpaceAtom : Atom
	{

		private float width;
		private float height;
		private float depth;
        private StrutPolicy policy;

		public static SpaceAtom Get (bool isGlue, float Width, float Height, float Depth)
		{
            var atom = ObjPool<SpaceAtom>.Get();
			if(isGlue)
			{
				float factor = TexUtility.glueRatio;
				atom.width = Width * factor;
                atom.height = Height * factor;
                atom.depth = Depth * factor;
                atom.policy = StrutPolicy.Glue;
			}
			else
			{
                atom.width = Width;
                atom.height = Height;
                atom.depth = Depth;
                atom.policy = StrutPolicy.Misc;
			}
            return atom;
		}

        public static SpaceAtom Get (float Width, float Height, float Depth)
		{
            var atom = ObjPool<SpaceAtom>.Get();
			atom.width = Width;
            atom.height = Height;
            atom.depth = Depth;
            atom.policy = StrutPolicy.Misc;
            return atom;
		}

        public static SpaceAtom Get()
		{
            var atom = Get(TexUtility.spaceWidth, TexUtility.spaceHeight, 0);
            atom.policy = StrutPolicy.BlankSpace;
            return atom;
		}

		public override Box CreateBox (TexStyle style)
		{
			float factor = TexUtility.SizeFactor(style);
			return StrutBox.Get (width * factor, height * factor, depth * factor, 0, policy);
		}

		public static Box CreateGlueBox(CharType leftType, CharType rightType, TexStyle style)
		{
			return TexUtility.GetBox(SpaceAtom.Get(true, TEXPreference.main.GetGlue(leftType, rightType), 0, 0), style);
		}

        public override void Flush()
        {
            ObjPool<SpaceAtom>.Release(this);
        }
	}
}