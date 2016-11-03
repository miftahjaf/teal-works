using UnityEngine;
using System.Collections;
using TexDrawLib;

#if UNITY_EDITOR
using UnityEditor;
#endif
//A Class contain each font data, using Serializable to make sure it can be preserved on build
namespace TexDrawLib
{
    [System.Serializable]
    public class TexFont
    {
        // core data
        public TexFontType type;
        public string id;
        public int index;
        public string name;

        // font data
        public Font font;
        public float font_lineHeight;

        // sprite data
        public Texture sprite;
        public int sprite_xLength;
        public int sprite_yLength;
        public bool sprite_detectAlpha;
        public float sprite_scale;
        public float sprite_lineOffset;
        public bool sprite_alphaOnly;

        public TexChar[] chars;

        public TexFont()
        {
        }

        #if UNITY_EDITOR
        public TexFont(string ID, int Index, string fontPath, string Name)
        {
            font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if (!font)
            {
                sprite = AssetDatabase.LoadAssetAtPath<Texture>(fontPath);
                if (!sprite)
                    throw new System.NullReferenceException("Primary Font/Sprite in " + fontPath + " doesn't Exist!, please check the path");
                else
                    type = TexFontType.Sprite;
            }
            else
                type = TexFontType.Font;
            
            id = ID;
            index = Index;
            name = Name;
            if (type == TexFontType.Font)
                PopulateCharacter(fontPath);
            else
                PopulateSprite();
        }

        public void PopulateCharacter(string path)
        {
            //A GLITCH: Unity's Font.HasCharacter doesn't work properly on dynamic mode, we need to change it to Unicode first
            TrueTypeFontImporter fontData = (TrueTypeFontImporter)AssetImporter.GetAtPath(path);

            fontData.fontTextureCase = FontTextureCase.Unicode;
            fontData.SaveAndReimport();

            chars = new TexChar[128];
            for (int i = 0; i < 128; i++)
            {
                chars[i] = new TexChar(this, i, font.HasCharacter((char)TEXPreference.TranslateChar(i)), 1);
            }

            fontData.fontTextureCase = FontTextureCase.Dynamic;
            fontData.SaveAndReimport();
        }

        public void PopulateSprite()
        {
            if (sprite_xLength < 1 || sprite_yLength < 1 || sprite_scale <= 0.0e-5f)
                SuggestTileSize();
            if (chars == null)
                chars = new TexChar[128];
            int maxCount = Mathf.Min(sprite_xLength * sprite_yLength, 128);
            Vector2 size = new Vector2(1 / (float)sprite_xLength, 1 / (float)sprite_yLength);

            for (int i = 0; i < 128; i++)
            {
                int x = i % sprite_xLength, y = i / sprite_xLength;
                if (chars[i] == null)
                {
                    chars[i] = new TexChar(this, i, i < maxCount, sprite_scale);
                    chars[i].depth = -sprite_lineOffset;
                    chars[i].height = sprite_scale + sprite_lineOffset;
                }
                else
                {
                    chars[i].depth = -sprite_lineOffset;
                    chars[i].height = sprite_scale + sprite_lineOffset;
                    chars[i].bearing = 0;
                    chars[i].italic = sprite_scale;
                    chars[i].width = sprite_scale;
                    chars[i].supported = i < maxCount;
                }
                TexChar c = chars[i];
                c.sprite_uv = new Rect(Vector2.Scale(new Vector2(x, sprite_yLength - y - 1), size), size);
                c.supported = i < maxCount;
            }
            font_lineHeight = (sprite.height / (float)sprite_yLength) / (sprite.width / (float)sprite_xLength); 
        }

        void SuggestTileSize()
        {
            sprite_xLength = 8;
            sprite_yLength = 4;
            sprite_scale = 1;
        }
        #endif
    }
}
