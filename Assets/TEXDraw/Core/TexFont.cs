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
		public Font font;
		public string ID;
		public int index;
		public TexChar[] chars;
		public float lineHeight;
		public float lineHeightRaw;

		#if UNITY_EDITOR
        public string Name;
        public string Description;
        public bool modifiable;
        #endif

		public TexFont ()
		{
		}

		#if UNITY_EDITOR
        public TexFont(string id, int Index, string fontPath, string name, string description, bool Modifiable)
        {
            font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if (!font)
                throw new System.NullReferenceException("Primary Font in " + fontPath + "didn't Exist!, please check the path");
            ID = id;
            index = Index;
            lineHeight = font.lineHeight / font.fontSize;
            lineHeightRaw = font.lineHeight;
            modifiable = Modifiable;
            Name = name;
            Description = description;
            PopulateCharacter(fontPath);
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
                chars[i] = new TexChar(this, i, font.HasCharacter((char)TEXPreference.TranslateChar(i)));
            }

            fontData.fontTextureCase = FontTextureCase.Dynamic;
            fontData.SaveAndReimport();
        }
        #endif
	}
}
