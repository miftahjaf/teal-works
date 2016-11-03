using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class UnicodeBox : Box
    {
        public static UnicodeBox Get(TexStyle style, int fontIdx, CharacterInfo c, float Scale)
        {
            var box = ObjPool<UnicodeBox>.Get();
            float ratio = TEXPreference.main.fontData[fontIdx].font.fontSize / Scale;
            box.character = c;
            box.fontIndex = fontIdx;
            box.depth = -c.minY / ratio;
            box.height = c.maxY / ratio;
            box.bearing = -c.minX / ratio;
            box.italic = c.maxX / ratio;
            box.width = c.advance / ratio;
            box.scaleApplied = Scale;
            return box;
        }

        public int fontIndex;
        public CharacterInfo character;
        public float bearing;
        public float italic;
        public float scaleApplied;

        public override void Draw(DrawingContext drawingContext, float scale, float x, float y)
        {
            base.Draw(drawingContext, scale, x, y);

            // Draw character at given position.
            CharacterInfo c;
            DrawingContext.GetCharInfo(fontIndex, (char)character.index, drawingContext.prefFontSize, FontStyle.Normal, out c);

            Vector2 vPos, vSize;

            if (c.size > 0) {
                var factor = scaleApplied / (float)c.size;
                var ratio = scale * factor;
                vPos = new Vector2((x + c.minX * factor) * scale, (y + c.minY * factor) * scale);
                vSize = new Vector2(c.glyphWidth * ratio, c.glyphHeight * ratio);
                width = c.advance * factor;
            } else {
                vPos = new Vector2((x - bearing) * scale, (y - depth) * scale); 
                vSize = new Vector2((bearing + italic) * scale, totalHeight * scale);
            }

            drawingContext.Draw(fontIndex, vPos, vSize, 
                c.uvBottomLeft,
                c.uvBottomRight,
                c.uvTopRight,
                c.uvTopLeft);
        }

        public override void Flush()
        {
            base.Flush();
            ObjPool<UnicodeBox>.Release(this);
        }
    }
}
