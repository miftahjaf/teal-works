using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace TexDrawLib
{
    public class DrawingContext : IDisposable
    {
        static readonly Vector3 defaultNormal = Vector3.back;
        static readonly Vector4 defaultTangen = new Vector4(1, 0, 0, -1);
        public TexFormulaParser parser;
        public List<TexFormula> parsed = ListPool<TexFormula>.Get();
        bool hasInit = false;
        public bool parsingComplete = false;
        public VertexHelper vertex;
        public int prefFontSize = 0;

        public DrawingContext()
        {
            vertex = new VertexHelper();
            parser = new TexFormulaParser();
            hasInit = true;
        }

        public static CharacterInfo GetCharInfo(int fontId, char Char)
        {
            return GetCharInfo(fontId, Char, 0);
        }

        static string[] chars = new string[0xffff]; 

        public static CharacterInfo GetCharInfo(int fontId, char Char, int size)
        {
            CharacterInfo info;
            Font f = TEXPreference.main.fontData[fontId].font;
            if (!f.dynamic)
                return new CharacterInfo();
            int idx = (int)Char;
            if(chars[idx] == null)
                chars[idx] = new string(Char, 1);
            f.RequestCharactersInTexture(chars[idx], size);
            f.GetCharacterInfo(Char, out info, size);
            return info;
        }

        public void Clear()
        {
            #if UNITY_5_3 || UNITY_5_4_OR_NEWER
            vertex.Clear();
            #else
            vertex.Dispose();
            vertex = new VertexHelper();
            #endif
        }

        public Vector2 VectorCode(int id)
        {
            return new Vector2((id % 4) / 4f, (id >> 2) / 4f);
        }

        public void Draw(int id, Rect v, Vector2[] uv)
        {
            Vector2 c = VectorCode(id); 

            int t = vertex.currentVertCount;
            //Top-Left
            vertex.AddVert(new Vector2(v.xMin, v.yMin), TexUtility.RenderColor, uv[0], c, defaultNormal, defaultTangen);
            //Top-Right
            vertex.AddVert(new Vector2(v.xMax, v.yMin), TexUtility.RenderColor, uv[1], c, defaultNormal, defaultTangen);
            //Bottom-Right
            vertex.AddVert(new Vector2(v.xMax, v.yMax), TexUtility.RenderColor, uv[2], c, defaultNormal, defaultTangen);
            //Bottom-Left
            vertex.AddVert(new Vector2(v.xMin, v.yMax), TexUtility.RenderColor, uv[3], c, defaultNormal, defaultTangen);

            vertex.AddTriangle(t + 0, t + 1, t + 2);
            vertex.AddTriangle(t + 2, t + 3, t + 0);
        }

        public void DrawWireDebug(Rect v, Color c)
        {
            int t = vertex.currentVertCount;
            Vector2 r = VectorCode(15); 
            vertex.AddVert(new Vector2(v.xMin, v.yMin), c, Vector2.zero, r, defaultNormal, defaultTangen);
            vertex.AddVert(new Vector2(v.xMax, v.yMin), c, Vector2.zero, r, defaultNormal, defaultTangen);
            vertex.AddVert(new Vector2(v.xMax, v.yMax), c, Vector2.zero, r, defaultNormal, defaultTangen);
            vertex.AddVert(new Vector2(v.xMin, v.yMax), c, Vector2.zero, r, defaultNormal, defaultTangen);
        
            vertex.AddTriangle(t + 0, t + 1, t + 3);
            vertex.AddTriangle(t + 3, t + 1, t + 2);
        }

        public void Draw(int id, Vector2[] v, Vector2[] uv)
        {
            Vector2 c = VectorCode(id); 
            int t = vertex.currentVertCount;
            //Top-Left
            vertex.AddVert(v[0], TexUtility.RenderColor, uv[0], c, defaultNormal, defaultTangen);
            //Top-Right
            vertex.AddVert(v[1], TexUtility.RenderColor, uv[1], c, defaultNormal, defaultTangen);
            //Bottom-Right
            vertex.AddVert(v[2], TexUtility.RenderColor, uv[2], c, defaultNormal, defaultTangen);
            //Bottom-Left
            vertex.AddVert(v[3], TexUtility.RenderColor, uv[3], c, defaultNormal, defaultTangen);

            vertex.AddTriangle(t + 0, t + 1, t + 2);
            vertex.AddTriangle(t + 2, t + 3, t + 0);
        }

        public bool Parse(string input, out string errResult)
        {
            if (!hasInit)
            {
                vertex = new VertexHelper();
                parser = new TexFormulaParser();
            }
            try
            {
                parsingComplete = false;
                string[] strings = input.Split(new char[]{ '\n' }, StringSplitOptions.None);
                if(parsed.Count > 0)
                {
                    for (int i = 0; i < parsed.Count; i++) 
                        parsed[i].Flush();
                }
                parsed.Clear();
                for (int i = 0; i < strings.Length; i++)
                    parsed.Add(parser.Parse(strings[i]));
                parsingComplete = true;
            }
            catch (Exception ex)
            {
                errResult = ex.Message;
                return false;
            }
            errResult = String.Empty;
            return true;
        }

        public bool Parse(string input)
        {
            if (!hasInit)
            {
                vertex = new VertexHelper();
                parser = new TexFormulaParser();
            }
            try
            {
                parsingComplete = false;
                string[] strings = input.Split(new char[]{ '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if(parsed.Count > 0)
                {
                    for (int i = 0; i < parsed.Count; i++) 
                        parsed[i].Flush();
                }
                parsed.Clear();
                for (int i = 0; i < strings.Length; i++)
                    parsed.Add(parser.Parse(strings[i]));
                parsingComplete = true;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Render(Mesh m, DrawingParams param)
        {
            m.Clear();
            Clear();
            if (parsingComplete)
            {
                prefFontSize = (int)param.fontSize;
                TexUtility.RenderColor = param.color;
                param.context = this;
                param.formulas = ToRenderers(parsed, param);
                param.Render();
            }
            Push2Mesh(m);
        }

        public static List<TexRenderer> ToRenderers(List<TexFormula> formulas, DrawingParams param)
        {
            var list = param.formulas;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Flush();
            }
            list.Clear();
            for (int i = 0; i < formulas.Count; i++)
            {
                list.Add(formulas[i].GetRenderer(TexStyle.Display, param.scale));
            }
            return list;
        }

        public void Render(VertexHelper vh, DrawingParams param)
        {
            vertex = vh;
            if (parsingComplete)
            {
                
                prefFontSize = (int)param.fontSize;
                TexUtility.RenderColor = param.color;
                param.context = this;
                param.formulas = ToRenderers(parsed, param);
                param.Render();
            }
        }
        /*
        public void Render (Mesh m, float scale, Vector3 drawSize, int fontScale, float spaceSize, Vector2 align, Vector2 Offset)
        {
            if (parsed != null)
            {
                m.Clear ();
                Clear ();
                if (!parsingComplete)
                    return;
                prefFontSize = fontScale;
                TexRenderer[] r = new TexRenderer[parsed.Length];
                Vector2 totalSize = Vector2.zero;
                for (int i = 0; i < parsed.Length; i++)
                {
                    r [i] = parsed [i].GetRenderer (TexStyle.Display, scale);
                        
                    totalSize = new Vector2 (Mathf.Max (r [i].RenderSize.x, totalSize.x), totalSize.y + r [i].RenderSize.y);
                }
                var factor = 1f;
                if (drawSize.sqrMagnitude > 0)
                {
                    if (totalSize.x > 0)
                    {
                        factor = Mathf.Min (drawSize.x / totalSize.x, factor);
                    }
                    if (totalSize.y > 0)
                    {
                        factor = Mathf.Min (drawSize.y / totalSize.y, factor);
                    }
                }
                spaceSize *= scale * factor;
                totalSize.y += (spaceSize - 1) * r.Length;
                float y = 0;
                for (int i = 0; i < parsed.Length; i++)
                {
                    r [i].Scale *= factor;
            
                    Vector2 offset = r [i].RenderSize;
                    offset.x *= -align.x;
                    offset.y *= 0;
                    offset.y += (spaceSize * i + y) - (totalSize.y * factor) * (1 - align.y);
                    offset += Offset;

                    y += r [i].RenderSize.y;

                    r [i].Render (this, offset.x, offset.y);
                }
                RePop (m);
            }
        }
        */

        protected void Push2Mesh(Mesh m)
        {
            vertex.FillMesh(m);        
        }

        public void Dispose()
        {
            vertex.Dispose();
        }
    }
}