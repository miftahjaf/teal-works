using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class UnicodeBox : Box
    {
        public static UnicodeBox Get(TexStyle style, int fontIdx, CharacterInfo c)
        {
            var box = ObjPool<UnicodeBox>.Get();
            float ratio = TEXPreference.main.fontData[fontIdx].font.fontSize;
            box.character = c;
            box.fontIndex = fontIdx;
            box.depth = -c.minY / ratio;
            box.height = c.maxY / ratio;
            box.bearing = -c.minX / ratio;
            box.italic = c.maxX / ratio;
            box.width = c.advance / ratio;
            return box;
        }

        public int fontIndex;
        public CharacterInfo character;
        public float bearing;
        public float italic;

        public override void Draw(DrawingContext drawingContext, float scale, float x, float y)
        {
            base.Draw(drawingContext, scale, x, y);

            // Draw character at given position.
            CharacterInfo c = DrawingContext.GetCharInfo(fontIndex, (char)character.index, drawingContext.prefFontSize);
            var vert = new Rect(new Vector2((x - bearing) * scale, (y - depth) * scale), 
                       new Vector2((bearing + italic) * scale, totalHeight * scale));
            var uv = new Vector2[]
            {
                c.uvBottomLeft,
                c.uvBottomRight,
                c.uvTopRight,
                c.uvTopLeft
            };
            drawingContext.Draw(fontIndex, vert, uv);
        }

        public override void Flush()
        {
            base.Flush();
            ObjPool<UnicodeBox>.Release(this);
        }
    }
}
