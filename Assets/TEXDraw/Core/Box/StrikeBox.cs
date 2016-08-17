using System;
using UnityEngine;

namespace TexDrawLib
{
    public class StrikeBox : Box
    {
        const string NegateSkin = "negateskin";

        public enum StrikeMode
        {
            diagonal = 0,
            diagonalInverse = 1,
            horizontal = 2,
            doubleHorizontal = 3
        }

        public float margin;
        public float thickness;
        public StrikeMode mode;

        public static StrikeBox Get(float Height, float Width, float Depth, float Margin, float Thickness, StrikeMode Mode)
        {
            var box = Get(Height, Width, Depth);
            box.margin = Margin;
            box.thickness = Thickness;
            box.mode = Mode;
            return box;
        }

        public static StrikeBox Get(float Height, float Width, float Depth)
        {
            var box = ObjPool<StrikeBox>.Get();
            box.width = Width;
            box.height = Height;
            box.depth = Depth;
            return box;
        }

        public override void Draw(DrawingContext drawingContext, float scale, float x, float y)
        {
            base.Draw(drawingContext, scale, x, y);
            PrepareDraw(drawingContext);
            
            float angle = Mathf.Atan2(totalHeight + margin*2, width);
            float s = Mathf.Sin(angle) * thickness, c = Mathf.Cos(angle) * thickness;
            float w = width, ww = width + margin, h = totalHeight/2f + margin;
            Vector2[] v = new Vector2[4];
            y += totalHeight / 2f - depth;
            switch (mode)
            {
                case StrikeMode.diagonal:
                    v[0] = new Vector2((x + w + s) * scale, (y + h - c) * scale); //Top-Left
                    v[1] = new Vector2((x + w - s) * scale, (y + h + c) * scale); //Top-Right
                    v[2] = new Vector2((x - s) * scale, (y - h + c) * scale); //Bottom-Right
                    v[3] = new Vector2((x + s) * scale, (y - h - c) * scale); //Bottom-Left
                    break;
                case StrikeMode.diagonalInverse:
                    v[0] = new Vector2((x + s) * scale, (y + h + c) * scale); //Top-Left
                    v[1] = new Vector2((x - s) * scale, (y + h - c) * scale); //Top-Right
                    v[2] = new Vector2((x + w - s) * scale, (y - h - c) * scale); //Bottom-Right
                    v[3] = new Vector2((x + w + s) * scale, (y - h + c) * scale); //Bottom-Left
                    break;
                case StrikeMode.horizontal:
                    v[0] = new Vector2((x - margin) * scale, (y + thickness) * scale); //Top-Left
                    v[1] = new Vector2((x + ww) * scale, (y + thickness) * scale); //Top-Right
                    v[2] = new Vector2((x + ww) * scale, (y - thickness) * scale); //Bottom-Right
                    v[3] = new Vector2((x - margin) * scale, (y - thickness) * scale); //Bottom-Left
                    break;
                case StrikeMode.doubleHorizontal:
                    float doubleOffset = TEXPreference.main.GetPreference("DoubleNegateOffset");
                    v[0] = new Vector2((x - margin) * scale, (y + thickness + doubleOffset) * scale); //Top-Left
                    v[1] = new Vector2((x + ww) * scale, (y + thickness + doubleOffset) * scale); //Top-Right
                    v[2] = new Vector2((x + ww) * scale, (y - thickness + doubleOffset) * scale); //Bottom-Right
                    v[3] = new Vector2((x - margin) * scale, (y - thickness + doubleOffset) * scale); //Bottom-Left
                    Draw(drawingContext, v);

                    v[0] = new Vector2((x - margin) * scale, (y + thickness - doubleOffset) * scale); //Top-Left
                    v[1] = new Vector2((x + ww) * scale, (y + thickness - doubleOffset) * scale); //Top-Right
                    v[2] = new Vector2((x + ww) * scale, (y - thickness - doubleOffset) * scale); //Bottom-Right
                    v[3] = new Vector2((x - margin) * scale, (y - thickness - doubleOffset) * scale); //Bottom-Left
                    break;
            }
            Draw(drawingContext, v);
        }

        Vector2[] uv;
        int fontIdx;

        void PrepareDraw(DrawingContext context)
        {
            TexChar n = TEXPreference.main.GetChar(NegateSkin);
            CharacterInfo c = DrawingContext.GetCharInfo(n.fontIndex, (char)TEXPreference.TranslateChar(n.index), context.prefFontSize);
            fontIdx = n.fontIndex;
            uv = new Vector2[4]
            {
                c.uvBottomLeft,
                c.uvBottomRight,
                c.uvTopRight,
                c.uvTopLeft
            };
        }

        void Draw(DrawingContext context, Vector2[] verts)
        {
            context.Draw(fontIdx, verts, uv);
        }

        public override void Flush()
        {
            base.Flush();
            ObjPool<StrikeBox>.Release(this);
        }
    }
}

