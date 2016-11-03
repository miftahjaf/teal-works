using UnityEngine;
//using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace TexDrawLib
{
    public class DrawingContext
    {
        public TexFormulaParser parser;
        public List<TexFormula> parsed = ListPool<TexFormula>.Get();
        public List<string> linkBoxKey = ListPool<string>.Get();
        public List<Rect> linkBoxRect = ListPool<Rect>.Get();
        public List<Color> linkBoxTint = ListPool<Color>.Get();
        bool hasInit = false;
        public bool parsingComplete = false;
        public FillHelper vertex;
        public int prefFontSize = 0;

        public DrawingContext()
        {
            vertex = new FillHelper();
            parser = new TexFormulaParser();
            hasInit = true;
        }


        static string[] chars = new string[0xffff]; 

        public static bool GetCharInfo(int fontId, char Char, int size, FontStyle style, out CharacterInfo c)
        {
            Font f = TEXPreference.main.fontData[fontId].font;
            if (!f.dynamic)
            {
                return f.GetCharacterInfo(Char, out c, 0);
           }
            int idx = (int)Char;
            if(chars[idx] == null)
                chars[idx] = Char.ToString();
            f.RequestCharactersInTexture(chars[idx], size, style);
            return f.GetCharacterInfo(Char, out c, size, style);
        }

        public void Clear()
        {
            vertex.Clear();
        }

        public Vector2 VectorCode(int id)
        {
            return new Vector2((id % 8) / 8f, (id >> 3) / 4f);
        }

        public void Draw(int id, Rect v, Vector2[] uv)
        {
            Vector2 c = VectorCode(id); 
            int t = vertex.currentVertCount;
            //Top-Left
            vertex.AddVert(new Vector2(v.xMin, v.yMin), TexUtility.RenderColor, uv[0], c);
            //Top-Right
            vertex.AddVert(new Vector2(v.xMax, v.yMin), TexUtility.RenderColor, uv[1], c);
            //Bottom-Right
            vertex.AddVert(new Vector2(v.xMax, v.yMax), TexUtility.RenderColor, uv[2], c);
            //Bottom-Left
            vertex.AddVert(new Vector2(v.xMin, v.yMax), TexUtility.RenderColor, uv[3], c);

            vertex.AddTriangle(t + 0, t + 1, t + 2);
            vertex.AddTriangle(t + 2, t + 3, t + 0);
        }

        public void Draw(int id, Vector2 vPos, Vector2 vSize, Vector2 uvTL, Vector2 uvTR, Vector2 uvBR, Vector2 uvBL)
        {
            Vector2 c = VectorCode(id); 
            int t = vertex.currentVertCount;
            //Top-Left
            vertex.AddVert(vPos, TexUtility.RenderColor, uvTL, c);
            //Top-Right
            vPos.x += vSize.x;
            vertex.AddVert(vPos, TexUtility.RenderColor, uvTR, c);
            //Bottom-Right
            vPos.y += vSize.y;
            vertex.AddVert(vPos, TexUtility.RenderColor, uvBR, c);
            //Bottom-Left
            vPos.x -= vSize.x;
            vertex.AddVert(vPos, TexUtility.RenderColor, uvBL, c);

            vertex.AddTriangle(t + 0, t + 1, t + 2);
            vertex.AddTriangle(t + 2, t + 3, t + 0);
        }

        public void DrawWireDebug(Rect v, Color c)
        {
            int t = vertex.currentVertCount;
            Vector2 r = VectorCode(TexUtility.blockFontIndex); 
            vertex.AddVert(new Vector2(v.xMin, v.yMin), c, Vector2.zero, r);
            vertex.AddVert(new Vector2(v.xMax, v.yMin), c, Vector2.zero, r);
            vertex.AddVert(new Vector2(v.xMax, v.yMax), c, Vector2.zero, r);
            vertex.AddVert(new Vector2(v.xMin, v.yMax), c, Vector2.zero, r);
        
            vertex.AddTriangle(t + 0, t + 1, t + 3);
            vertex.AddTriangle(t + 3, t + 1, t + 2);
        }

        public Color DrawLink(Rect v, string key)
        {
            linkBoxKey.Add(key);
            linkBoxRect.Add(v);
            if(linkBoxKey.Count > linkBoxTint.Count)
                linkBoxTint.Add(Color.white);
            return linkBoxTint[linkBoxKey.Count - 1];
        }

        public void Draw(int id, Vector2[] v, Vector2[] uv)
        {
            Vector2 c = VectorCode(id); 
            int t = vertex.currentVertCount;
            //Top-Left
            vertex.AddVert(v[0], TexUtility.RenderColor, uv[0], c);
            //Top-Right
            vertex.AddVert(v[1], TexUtility.RenderColor, uv[1], c);
            //Bottom-Right
            vertex.AddVert(v[2], TexUtility.RenderColor, uv[2], c);
            //Bottom-Left
            vertex.AddVert(v[3], TexUtility.RenderColor, uv[3], c);

            vertex.AddTriangle(t + 0, t + 1, t + 2);
            vertex.AddTriangle(t + 2, t + 3, t + 0);
        }

	    static readonly char[] newLineChar = new char[]{ '\n' };

        public bool Parse(string input, out string errResult)
        {
            if (!hasInit)
            {
                vertex = new FillHelper();
                parser = new TexFormulaParser();
            }
            try
            {
                parsingComplete = false;
	            string[] strings = input.Split(newLineChar, StringSplitOptions.None);
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
                vertex = new FillHelper();
                parser = new TexFormulaParser();
            }
            try
            {
                parsingComplete = false;
	            string[] strings = input.Split(newLineChar, StringSplitOptions.RemoveEmptyEntries);
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
                //param.formulas = ToRenderers(parsed, param);
                linkBoxKey.Clear();
                linkBoxRect.Clear();
                param.Render();
            }
            Push2Mesh(m);
        }


        /// Convert Atom into Boxes
        public static List<TexRenderer> ToRenderers(List<TexFormula> formulas, DrawingParams param)
        {
            var list = param.formulas;
            TexUtility.RenderTextureSize = param.fontSize;
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

        protected void Push2Mesh(Mesh m)
        {
            vertex.FillMesh(m);        
        }
     }
}