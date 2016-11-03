using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
    // Box representing single character.
    public class CharBox : Box
    {
        public static CharBox Get(TexStyle style, TexCharMetric Char)
        {
            return Get(style, Char, TexUtility.RenderFontStyle);
        }
        public static CharBox Get(TexStyle style, TexCharMetric Char, FontStyle fontStyle)
        {
            var box = ObjPool<CharBox>.Get();
            if (Char == null)
                throw new NullReferenceException();
            box.character = Char;
            //I can't say more but our cached glyph is slightly incorrect because
            //the usage of int in character Info, so we need to...
            if (Char.ch.font.type == TexFontType.Font && Char.ch.supported) {
                CharacterInfo c;
                if (DrawingContext.GetCharInfo(Char.ch.fontIndex, (char)TEXPreference.TranslateChar(Char.ch.index),
                            (int)(TexUtility.RenderTextureSize * Char.appliedScale) + 1, fontStyle, out c)) {
                    float ratio = (c.size == 0 ? (float)TexUtility.RenderTextureSize : c.size) / Char.appliedScale;
                    box.depth = -c.minY / ratio;
                    box.height = c.maxY / ratio;
                    box.bearing = -c.minX / ratio;
                    box.italic = c.maxX / ratio;
                    box.width = c.advance / ratio;
                    box.c = c;
                    return box;
                }
            } 

            box.depth = Char.depth;
            box.height = Char.height;
            box.bearing = Char.bearing;
            box.italic = Char.italic;
            box.width = Char.width;
            
            return box;
        }

        public TexCharMetric character;

        public CharacterInfo c;

        public float bearing;

        public float italic;

        public override void Draw(DrawingContext drawingContext, float scale, float x, float y)
        {
            base.Draw(drawingContext, scale, x, y);

            // Draw character at given position.
            Vector2 vPos = new Vector2((x - bearing) * scale, (y - depth) * scale); 
            Vector2 vSize = new Vector2((bearing + italic) * scale, totalHeight * scale);
            TexChar ch = character.ch;
            if (ch.font.type == TexFontType.Font) {
                drawingContext.Draw(ch.fontIndex, vPos, vSize, 
                    c.uvBottomLeft, c.uvBottomRight, c.uvTopRight, c.uvTopLeft);
            } else {
                Rect u = ch.sprite_uv;
                if (!ch.font.sprite_alphaOnly) {
                    //Using RGB? then the color should be black
                    //see the shader why it's happen to be like that
                    Color tmpC = TexUtility.RenderColor;
                    TexUtility.RenderColor = Color.black;
                    drawingContext.Draw(ch.fontIndex, vPos, vSize, 
                        u.min, new Vector2(u.xMax, u.yMin),
                        u.max, new Vector2(u.xMin, u.yMax));
                    TexUtility.RenderColor = tmpC;
                } else {
                    drawingContext.Draw(ch.fontIndex, vPos, vSize,
                        u.min, new Vector2(u.xMax, u.yMin),
                        u.max, new Vector2(u.xMin, u.yMax));
                }
            }
//            hasDraw = true;
        }

        //      bool hasDraw = false;

        public override void Flush()
        {
            /*        if (hasDraw)
                hasDraw = false;
            else
                return;*/
            base.Flush();
            if (character != null) {
                character.Flush();
                character = null;
            }
            ObjPool<CharBox>.Release(this);
        }
    }
}