#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace TexDrawLib
{
    public class TEXPreference : ScriptableObject
    {
        #if UNITY_EDITOR
        //PAY ATTENTION TO THIS: IF YOU DECIDE TO MOVE WHOLE TEXDRAW
        //FOLDER, THEN REPLACE THIS IN DEBUG INSPECTOR (NOT HERE)
        public string MainFolderPath = "Assets/TEXDraw";
        const string DefaultTexFolder = "Assets/TEXDraw";

        public bool IncludeMathSlot = true;
 
        #region Data Management

        public TextAsset XMLFontDefinitions;
        public TextAsset XMLSymbolDefinitions;
	    public TextAsset XMLMathDefinitions;
	    public TextAsset XMLConfiguration;
        public TexFontHeader[] fontHeaders;
        public TexConfigurationMember[] configs;
        public bool editorReloading = false;

        [ContextMenu("Read (Import) from XML Data")]
        public void Reload()
	    {
		    #if UNITY_WEBPLAYER
		    	throw new System.Exception("Please switch your platform into PC first!");
			#else
            if (!EditorUtility.DisplayDialog("Confirm Reload", 
                "Are you sure to reload all data from XML files? This will take a moment.\n (your unexported changes will lost)", 
                "OK, Discard my Changes", "No"))
                return;	
		    FirstInitialize(MainFolderPath);
		    #endif
        }

        [ContextMenu("Write (Export) to XML Data")] 
        public void WriteData()
        {
            #if UNITY_WEBPLAYER
		    	throw new System.Exception("Please switch your platform into PC first!");
			#else
	        if (!TryPushToDictionaries())
                return;
            if (!EditorUtility.DisplayDialog("Confirm Export", 
	            "Are you sure to save modified data to XML files? This will take a moment.\n (existing XML will be overwritten)", 
                "OK, Overwrite it", "No"))
                return;	
	        TexXmlParser.WriteSymbols(this, false);
            TexXmlParser.WritePreferences(this);
            AssetDatabase.Refresh();
            #endif
        }
	    
	    [ContextMenu("Overwrite Math to XML Data")] 
	    public void WriteMathData()
	    {
            #if UNITY_WEBPLAYER
		    	throw new System.Exception("Please switch your platform into PC first!");
			#else
		    if (!TryPushToDictionaries())
			    return;
		    if (!EditorUtility.DisplayDialog("Confirm Overwrite", 
			    "Are you sure you want to overwrite Math Definition? \n (This is not recommended, Math definition shouldn't be overwritten by users)", 
			    "OK, Overwrite it", "No"))
			    return;	
		    TexXmlParser.WriteSymbols(this, true);
		    AssetDatabase.Refresh();
            #endif
	    }

        public void FirstInitialize(string mainPath)
        {
            try
            {
                editorReloading = true;
                EditorUtility.DisplayProgressBar("Reloading", "Reading XML Files...", 0f);
	            
	            preferences = new TexPrefsDictionary();
                symbolData = new TexSymbolDictionary();
                defaultTypefaces = new TexTypeFaceDictionary();
                charMapData = new TexCharMapDictionary();
                glueTable = new int[100];

                MainFolderPath = mainPath;
                XMLFontDefinitions = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexFontDefinitions.xml");
                XMLSymbolDefinitions = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexSymbolDefinitions.xml");
	            XMLMathDefinitions = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexMathDefinitions.xml");
	            XMLConfiguration = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexConfigurations.xml");

                TexXmlParser.LoadPrimaryDefinitions(this);
                EditorUtility.DisplayProgressBar("Reloading", "Reading Configurations...", .95f);
	            TexXmlParser.ReadSymbols(this, true);
	            TexXmlParser.ReadSymbols(this, false);
	            TexXmlParser.ReadPreferences(this);
                EditorUtility.DisplayProgressBar("Reloading", "Refreshing Instances...", .95f);

                PaintFontList();
                RebuildMaterial();
                editorReloading = false;
                CallRedraw();
            }
            catch (System.Exception ex)
            {
                Debug.LogError("TEXPreference was fail to Import XML Data, Error Message:\n" + ex.ToString());
            }
            EditorUtility.ClearProgressBar();
        }

        void Reset()
        {
            string dataPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            if(string.IsNullOrEmpty(dataPath))
                return;
            m_main = AssetDatabase.LoadAssetAtPath<TEXPreference>(dataPath);
            FirstInitialize (dataPath);
        }
        #endregion

        #region Runtime

        public void PushToDictionaries()
        {
            PushToDictionaries(false);
        }

        public void PushToDictionaries(bool onlyPref)
        {
            int lastHash = 0;
            try
            {
                if (!onlyPref)
                {
                    ClearDictionary();
                    for (int i = 0; i < fontData.Length; i++)
                    {
                        for (int j = 0; j < 128; j++)
                        {
                            TexChar c = GetChar(i, j);
                            c.CheckValidity();
                            lastHash = c.ToHash();
                            if (!string.IsNullOrEmpty(c.symbolName))
                                symbolData.Add(c.symbolName, lastHash);
                            if (!string.IsNullOrEmpty(c.symbolAlt))
                                symbolData.Add(c.symbolAlt, lastHash);
                            if (c.characterMap > 0)
                                charMapData.Add(TexChar.possibleCharMaps[c.characterMap], lastHash);
                        }
                    }
                }
                else
                    preferences.Clear();
                for (int i = 0; i < configs.Length; i++)
                {
                    preferences.Add(configs[i].name, configs[i].value);
                }
            }
            catch (System.Exception ex)
            {
                if (ex is System.ArgumentException)
                {
                    int pair;
                    try
                    {
                        pair = GetChar(GetChar(lastHash).symbolName).ToHash();
                    }
                    catch
                    {
                        return;
                    }
                    Debug.LogError(string.Format("Duplicate Definitions Exist at {0:X3} and {1:X3}, Please fix it since runtime conversion didn't complete yet. \n Real Message:{2}", lastHash, pair, ex.Message));
                }
                else
                {
                    Debug.LogError("Unknown Error: " + ex.GetType().Name + " at " + lastHash.ToString("0:X") + ", Please fix it since runtime conversion didn't complete yet.\n Real Message:" + ex.Message);
                }
                return;
            }
        }

        public bool TryPushToDictionaries()
        {
            int lastHash = 0;
            try
            {
                ClearDictionary();
                for (int i = 0; i < fontData.Length; i++)
                {
                    for (int j = 0; j < 128; j++)
                    {
                        TexChar c = GetChar(i, j);
                        c.CheckValidity();
                        lastHash = c.ToHash();
                        if (!string.IsNullOrEmpty(c.symbolName))
                            symbolData.Add(c.symbolName, lastHash);
                        if (!string.IsNullOrEmpty(c.symbolAlt))
                            symbolData.Add(c.symbolAlt, lastHash);
                        if (c.characterMap > 0)
                            charMapData.Add(TexChar.possibleCharMaps[c.characterMap], lastHash);
                    }
                }
                for (int i = 0; i < configs.Length; i++)
                {
                    preferences.Add(configs[i].name, configs[i].value);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                if (ex is System.ArgumentException)
                {
                    Debug.LogError(string.Format("Failed Exporting to XML Files: Duplicate Definitions Exist at {0:X}, Please fix it. Message:\n{1}", lastHash, ex.Message));
                }
                else
                {
                    Debug.LogError("Failed Exporting to XML Files: " + ex.GetType().Name + " at " + lastHash.ToString("0:X") + ", Message:\n" + ex.Message);
                }
                return false;
            }
        }

        void ClearDictionary()
        {
            if (symbolData != null)
                symbolData.Clear();
            else
                symbolData = new TexSymbolDictionary();
            if (charMapData != null)
                charMapData.Clear();
            else
                charMapData = new TexCharMapDictionary();
            if (preferences != null)
                preferences.Clear();
            else
                preferences = new TexPrefsDictionary();
        }

        public void CallRedraw()
        {
            Component[] tex = Object.FindObjectsOfType<Component>();
            for (int i = 0; i < tex.Length; i++) {
                if(tex[i] is ITEXDraw)
                    ((ITEXDraw)tex[i]).SetTextDirty(true);
            }
            SceneView.RepaintAll();
        }

        public string[] ConfigIDs;
        public string[] FontIDs;
        public int[] FontIndexs;

        public void PaintFontList()
        {
            List<string> s = new List<string>();
            List<string> t = new List<string>();
            List<int> n = new List<int>();
            n.Add(-1);
            t.Add("-1 (Use Math Typefaces)");
            for (int i = 0; i < fontData.Length; i++)
            {
                t.Add(string.Format("{0} - {1}.ttf", i , fontData[i].id));
                n.Add(i);
                s.Add(fontData[i].id);
            }
            ConfigIDs = s.ToArray();
            FontIDs = t.ToArray();
            FontIndexs = n.ToArray();
        }

        public Material[] watchedMaterial;

        public void RebuildMaterial()
        {
            if (!defaultMaterial)
            {
                defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>(MainFolderPath + "/TEX-Default.mat");
                if (!defaultMaterial)
                    Debug.LogError("TEXDraw default Material Asset didn't found! please assign it manually.");
            }

            if(!watchedMaterial.Contains(defaultMaterial))
                ArrayUtility.Insert(ref watchedMaterial, 0, defaultMaterial);
            foreach (var mat in watchedMaterial)
            {
	            if(!mat)
	            	continue;
                for (int i = 0; i < fontData.Length; i++)
                {
                    string propName = string.Format("_Font{0:X}", i);
                    if(!mat.HasProperty(propName))
                    {
                        mat.shader = Shader.Find("TEXDraw/Default/4 Pass");
                        Debug.LogWarning("Changing Material (named '" + mat.name + "') shader to Default 4 Pass since it doesn't have a suifficient texture slot", mat);
                    }
                    if(fontData[i].type == TexFontType.Font)
                        mat.SetTexture(propName, fontData[i].font.material.mainTexture);
                    else
                        mat.SetTexture(propName, fontData[i].sprite);
                }
            }
        }

        #endregion

        #endif

        #region Preference Locator

        static TEXPreference m_main;
        #if UNITY_EDITOR
        //Main & Shared access to TEXDraw Preference
        static public TEXPreference main
        {
            get
            {
                if (!m_main)
                {
                    //Get the Preference
                    string[] targetData = AssetDatabase.FindAssets("t:TEXPreference");
                    if (targetData.Length > 0)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(targetData[0]);
                        m_main = AssetDatabase.LoadAssetAtPath<TEXPreference>(path);
                        m_main.MainFolderPath = System.IO.Path.GetDirectoryName(path);
                        if (targetData.Length > 1)
                            Debug.LogWarning("You have more than one TEXDraw preference, ensure that only one Preference exist in your Project");
                    }
                    else
                    {
                        //Create New One
                        m_main = ScriptableObject.CreateInstance<TEXPreference>();
                        if (AssetDatabase.IsValidFolder(DefaultTexFolder))
                        {
                            AssetDatabase.CreateAsset(m_main, DefaultTexFolder + "/TEXDrawPreference.asset");
                            m_main.FirstInitialize(DefaultTexFolder);
                        }
                        else
                        {
                            //Find alternative path to the TEXPreference, that's it: Parent path of TEXPreference script.
                            string AlternativePath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(m_main));
                            AlternativePath = System.IO.Directory.GetParent(AlternativePath).Parent.FullName;
                            AssetDatabase.CreateAsset(m_main, AlternativePath + "/TEXDrawPreference.asset");
                            m_main.FirstInitialize(AlternativePath);
                        }
                    }
                }
                return m_main;
            }
            set
            {
                //Leaving this empty,
                //TEXDraw will assign this manually
                //when on real build.
            }
        }
        #else
	static public TEXPreference main
	{
		get
		{
			return m_main;
		}
		set
		{
			m_main = value;
		}
	}
	#endif
        #endregion

        #region Runtime Utilities
        //Default Material
        public Material defaultMaterial;
        //The most important reference to Central font data
        public TexFont[] fontData;
        //Dictionaries for references, etc.
        public TexSymbolDictionary symbolData;
        public TexCharMapDictionary charMapData;
        public TexPrefsDictionary preferences;
        public TexTypeFaceDictionary defaultTypefaces;
        //Rule Table: Left * 10 + Right
        public int[] glueTable;

        public TexFont GetFontByID(string id)
        {
            try
            {
                return System.Array.Find<TexFont>(fontData, x => x.id == id);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public int GetFontIndexByID(string id)
        {
            try
            {
                return System.Array.Find<TexFont>(fontData, x => x.id == id).index;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        public TexChar GetChar(int font, int ch)
        {
            return fontData[font].chars[ch];
        }

        public TexChar GetChar(int hash)
        {
            if (hash < 0)
                return null;
            return fontData[hash >> 8].chars[hash % 128];
        }

        public TexChar GetChar(string symbol)
        {
            return GetChar(symbolData.GetOrNone(symbol, -1));
        }

        public TexCharMetric GetCharMetric(string symbol, float size)
        {
            return GetChar(symbol).GetMetric(size);
        }

        public TexCharMetric GetCharMetric(string symbol, TexStyle style)
        {
            return GetChar(symbol).GetMetric(TexUtility.SizeFactor(style));
        }

        public TexCharMetric GetCharMetric(TexChar Char, TexStyle style)
        {
            if (Char != null)
                return Char.GetMetric(TexUtility.SizeFactor(style));
            else
                return null;
        }

        public TexCharMetric GetCharMetric(char ch, TexStyle style)
        {
            if (char.IsUpper(ch))
                return fontData[defaultTypefaces[TexCharKind.Capitals]].chars[ch].GetMetric(TexUtility.SizeFactor(style));
            if (char.IsLower(ch))
                return fontData[defaultTypefaces[TexCharKind.Small]].chars[ch].GetMetric(TexUtility.SizeFactor(style));
            if (char.IsDigit(ch))
                return fontData[defaultTypefaces[TexCharKind.Numbers]].chars[ch].GetMetric(TexUtility.SizeFactor(style));
            return fontData[(int)preferences["DefaultMuFontID"]].chars[ch].GetMetric(TexUtility.SizeFactor(style));
        }

        public TexCharMetric GetCharMetric(int font, char ch, TexStyle style)
        {
            return fontData[font].chars[ch].GetMetric(TexUtility.SizeFactor(style));
        }

        public float GetPreference(string name)
        {
            return preferences[name];
        }

        public float GetPreference(string name, float size)
        {
            return preferences[name] * size;
        }

        public float GetPreference(string name, TexStyle style)
        {
            return preferences[name] * TexUtility.SizeFactor(style);
        }

        public int GetGlue(CharType leftType, CharType rightType)
        {
            return glueTable[(int)leftType * 10 + (int)rightType]; 
        }

        static public int CharToHash(TexChar ch)
        {
            return ch.index | ch.font.index << 8;
        }

        static public int CharToHash(int font, int ch)
        {
            return ch | font << 8;
        }

        static public int TranslateChar(int charIdx)
        {
            //An Integer Conversion from TEX-Character-Space (0-7F) to Actual-Character-Map (ASCII Latin-1)
            if (charIdx >= 0x0 && charIdx <= 0xf)
                return charIdx + 0xc0;
            if (charIdx == 0x10)
                return 0xb0;
            if (charIdx >= 0x11 && charIdx <= 0x16)
                return charIdx + (0xd1 - 0x11);
            if (charIdx == 0x17)
                return 0xb7;
            if (charIdx >= 0x18 && charIdx <= 0x1c)
                return charIdx + (0xd8 - 0x18);
            if (charIdx >= 0x1d && charIdx <= 0x1e)
                return charIdx + (0xb5 - 0x1d);
            if (charIdx == 0x1f)
                return 0xdf;
            if (charIdx == 0x20)
                return 0xef;
            if (charIdx >= 0x21 && charIdx <= 0x7e)
                return charIdx;
            if (charIdx == 0x7f)
                return 0xff;
            return 0;
        }
		
        #endregion
    }
}