using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
//using TexDrawLib;

namespace TexDrawLib
{
	// Box representing single character.
	public class CharBox : Box
	{
		public static CharBox Get(TexStyle style, TexCharMetric Char)
		{
            var box = ObjPool<CharBox>.Get();
            if(Char == null)
                throw new NullReferenceException();
			box.character = Char;
			box.depth = Char.depth;
			box.height = Char.height;
			box.bearing = Char.bearing;
			box.italic = Char.italic;
			box.width = Char.width;
            return box;
        }

        public TexCharMetric character;

        public float bearing;

        public float italic;

		public override void Draw (DrawingContext drawingContext, float scale, float x, float y)
		{
            base.Draw (drawingContext, scale, x, y);

			// Draw character at given position.
			 CharacterInfo c = DrawingContext.GetCharInfo (character.ch.fontIndex, (char)TEXPreference.TranslateChar(character.ch.index), drawingContext.prefFontSize);
            var vert = new Rect ((x - bearing)	* scale, (y - depth) * scale, 
				(bearing + italic) * scale, totalHeight * scale);
			var uv = new Vector2[] {
				c.uvBottomLeft,
				c.uvBottomRight,
				c.uvTopRight,
				c.uvTopLeft
			};
			drawingContext.Draw (character.ch.fontIndex, vert, uv);
            c = default(CharacterInfo);
            hasDraw = true;
		}
        bool hasDraw = false;
        public override void Flush()
        {
            if(hasDraw)
                hasDraw = false;
            else
                return;
            base.Flush();
            if(character != null)
            {
                character.Flush();
                character = null;
            }
            ObjPool<CharBox>.Release(this);
        }
	}
}