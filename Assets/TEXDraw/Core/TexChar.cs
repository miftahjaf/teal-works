using UnityEngine;
using System.Collections;
using TexDrawLib;


namespace TexDrawLib
{
    [System.Serializable]
    public class TexChar
    {

        //Standard Property, is it supported? what's index? what's type?
        public bool supported;
        public int index;
        public CharType type;

        //Character (Glyph) Properties
        public float height;
        public float depth;
        public float bearing;
        public float italic;
        public float width;
        public int font_reqGlyphSize;

        public TexCharMetric GetMetric(float scale)
        {
            return TexCharMetric.Get(this, height, depth, bearing, italic, width, scale);
        }

        //Rect to UV Texture, if it sprite
        public Rect sprite_uv;

        //Reference to actual Font
        public int fontIndex;

        public TexFont font
        {
            get { return TEXPreference.main.fontData[fontIndex]; }
            set { fontIndex = value.index; }
        }

        #region Extension Character Database

        //Is Larger (Similar) Character Exist ?, Then reference to it
        public int nextLargerHash = -1;

        public bool nextLargerExist { get { return nextLargerHash > -1; } }

        public TexChar nextLarger
        {
            get { return TEXPreference.main.GetChar(nextLargerHash); }
            set { nextLargerHash = value != null ? value.ToHash() : -1; }
        }

        //Extension Setup
        public bool extensionExist = false;

        public bool extensionHorizontal = false;

        public int extentTopHash;

        public TexChar extentTop
        {
            get { return TEXPreference.main.GetChar(extentTopHash); }
            set { extentTopHash = value != null ? value.ToHash() : -1; }
        }

        public int extentMiddleHash;

        public TexChar extentMiddle
        {
            get { return TEXPreference.main.GetChar(extentMiddleHash); }
            set { extentMiddleHash = value != null ? value.ToHash() : -1; }
        }

        public int extentBottomHash;

        public TexChar extentBottom
        {
            get { return TEXPreference.main.GetChar(extentBottomHash); }
            set { extentBottomHash = value != null ? value.ToHash() : -1; }
        }

        public int extentRepeatHash;

        public TexChar extentRepeat
        {
            get { return TEXPreference.main.GetChar(extentRepeatHash); }
            set { extentRepeatHash = value != null ? value.ToHash() : -1; }
        }

        public TexCharMetric[] GetExtentMetrics(TexStyle style)
        {
            
            var metric = new TexCharMetric[4];
            metric[0] = TEXPreference.main.GetCharMetric(extentTop, style);
            metric[1] = TEXPreference.main.GetCharMetric(extentMiddle, style);
            metric[2] = TEXPreference.main.GetCharMetric(extentBottom, style);
            metric[3] = TEXPreference.main.GetCharMetric(extentRepeat, style);
                
            return metric;
        }

        #endregion

        public int ToHash()
        {
            return index | (fontIndex << 8);
        }

        public string symbolName;
        public string symbolAlt;
        public int characterMap = 0;
        public const string possibleCharMaps = " +-*/=()[]<>|.,;:`~\'\"?!@#$%&{}\\_^";

        public void CheckValidity()
        {
            if (!string.IsNullOrEmpty(symbolAlt) && string.IsNullOrEmpty(symbolName)) {
                symbolName = symbolAlt;
                symbolAlt = string.Empty;
            }
        }

        public TexChar()
        {
        }

        public TexChar(TexFont Font, int Index, bool Supported, float scale)
        {
            fontIndex = Font.index;
            index = Index;
            supported = Supported;
            extentTopHash = -1;
            extentMiddleHash = -1;
            extentBottomHash = -1;
            extentRepeatHash = -1;
            if (supported) {
                if (Font.type == TexFontType.Font) {
                    CharacterInfo c = getCharInfo(Font.font, (char)TEXPreference.TranslateChar(index));
                    UpdateGlyph(Font.font, c);
                } else {
                    depth = 0;
                    height = scale;
                    bearing = 0;
                    italic = scale;
                    width = scale;
                }
            }
        }

        public void UpdateGlyph(Font font, CharacterInfo c)
        {
            font_reqGlyphSize = c.size == 0 ? font.fontSize : c.size;
            float ratio = (float)font_reqGlyphSize;
            depth = -c.minY / ratio;
            height = c.maxY / ratio;
            bearing = -c.minX / ratio;
            italic = c.maxX / ratio;
            width = c.advance / ratio;
               
        }

        static CharacterInfo getCharInfo(Font font, char ch)
        {
            string s = new string(ch, 1);
            font.RequestCharactersInTexture(s);
            CharacterInfo o;
            if (font.GetCharacterInfo(ch, out o))
                return o;
            else
                return new CharacterInfo();
        }
    }
}