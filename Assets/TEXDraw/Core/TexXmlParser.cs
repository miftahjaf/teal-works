#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;

namespace TexDrawLib
{
    public class TexXmlParser
    {
        #if UNITY_WEBPLAYER
		
		public static void LoadPrimaryDefinitions(TEXPreference pref)
		{
			throw new System.Exception("Please switch your platform into PC first!");
		}
		
		public static void ReadSymbols(TEXPreference pref)
		{
			throw new System.Exception("Please switch your platform into PC first!");
		}
		
		public static void ReadPreferences(TEXPreference pref)
		{
			throw new System.Exception("Please switch your platform into PC first!");
		}
		
		public static void WriteSymbols(TEXPreference pref)
		{
			throw new System.Exception("Please switch your platform into PC first!");
		}
		
		public static void WritePreferences(TEXPreference pref)
		{
			throw new System.Exception("Please switch your platform into PC first!");
		}
		
		
		


#else
        //All importing scenario begin in here.

        public static void LoadPrimaryDefinitions(TEXPreference pref)
        {
            //Load all variables and XML Data
            XmlDocument doc = new XmlDocument();
            EditorUtility.DisplayProgressBar("Reloading", "Reloading from XML Contents...", 0);
            doc.LoadXml(pref.XMLFontDefinitions.text);
            XmlNode configNodes = doc.SelectSingleNode("/TexFont/Params");
            XmlNode cNode;
            List<TexFont> Datas = new List<TexFont>();
            List<TexFontHeader> Headers = new List<TexFontHeader>();
            List<TexConfigurationMember> configs = new List<TexConfigurationMember>();
            //Look up all font & sprites
            if (AssetDatabase.IsValidFolder(pref.MainFolderPath + "/Fonts/Math")) {
                string[] customF = AssetDatabase.FindAssets("t:Font", new string[]{ pref.MainFolderPath + "/Fonts/Math" });
                for (int i = 0; i < customF.Length; i++) {
                    if (Datas.Count >= 31)
                        break;
                    string realPath = AssetDatabase.GUIDToAssetPath(customF[i]);
                    string id = System.IO.Path.GetFileNameWithoutExtension(realPath).ToLower();
                    UpdateProgress(0, id, i, customF.Length);
                    Datas.Add(new TexFont(id, Datas.Count, realPath, id));           
                }
                Headers.Add(new TexFontHeader("Math Fonts", customF.Length));
            }
            if (AssetDatabase.IsValidFolder(pref.MainFolderPath + "/Fonts/User")) {
                string[] customF = AssetDatabase.FindAssets("t:Font", new string[]{ pref.MainFolderPath + "/Fonts/User" });
                for (int i = 0; i < customF.Length; i++) {
                    if (Datas.Count >= 31)
                        break;
                    string realPath = AssetDatabase.GUIDToAssetPath(customF[i]);
                    string id = System.IO.Path.GetFileNameWithoutExtension(realPath).ToLower();
                    UpdateProgress(1, id, i, customF.Length);
                    Datas.Add(new TexFont(id, Datas.Count, realPath, id));           
                }
                Headers.Add(new TexFontHeader("User Fonts", customF.Length));
            }
            if (AssetDatabase.IsValidFolder(pref.MainFolderPath + "/Fonts/Sprites")) {
                string[] customS = AssetDatabase.FindAssets("t:Texture", new string[]{ pref.MainFolderPath + "/Fonts/Sprites" });
                for (int i = 0; i < customS.Length; i++) {
                    if (Datas.Count >= 31)
                        break;
                    string realPath = AssetDatabase.GUIDToAssetPath(customS[i]);
                    string id = System.IO.Path.GetFileNameWithoutExtension(realPath).ToLower();
                    UpdateProgress(2, id, i, customS.Length);
                    Datas.Add(new TexFont(id, Datas.Count, realPath, id));
                }
                Headers.Add(new TexFontHeader("Font Sprites", customS.Length));
            }
            EditorUtility.DisplayProgressBar("Reloading", "Preparing Stuff...", .93f);
            //Load all configurations.
            for (int i = 0; i < configNodes.ChildNodes.Count; i++) {
                cNode = configNodes.ChildNodes[i];
                configs.Add(new TexConfigurationMember(cNode.Attributes["name"].Value, cNode.Attributes["desc"].Value, 
                        float.Parse(cNode.Attributes["value"].Value), float.Parse(cNode.Attributes["min"].Value), float.Parse(cNode.Attributes["max"].Value)));
            }	

            pref.fontData = Datas.ToArray();
            pref.fontHeaders = Headers.ToArray();
            pref.configs = configs.ToArray();
            pref.PushToDictionaries(true);
        }

        static void UpdateProgress(int phase, string name, int idx, int total)
        {
            var prog = idx / (float)total;
            prog = phase * 0.3f + (prog * 0.3f);
            EditorUtility.DisplayProgressBar("Reloading", "Reading " + name + "...", prog);
        }

        //Converting a hash from XML file to Our Font map,
        static int SyncHash(int origHash, List<int> syncMap)
        {
            if (origHash == -1)
                return -1;
            if (syncMap[origHash >> 8] == -1)
                return -1;
            return (syncMap[origHash >> 8] << 8) + (origHash % 128);
        }

	    public static void ReadSymbols(TEXPreference pref, bool isMath)
        {
            XmlDocument doc = new XmlDocument();
	        doc.LoadXml(isMath ? pref.XMLMathDefinitions.text : pref.XMLSymbolDefinitions.text);
            TexChar c; 

	        XmlNode SyncMap = doc.SelectSingleNode("/TexSymbols/FontIDs");
	        List<int> sMap = new List<int>();
	        for (int i = 0; i < SyncMap.ChildNodes.Count; i++) {
                XmlAttributeCollection attr = SyncMap.ChildNodes[i].Attributes;
		        int idx = pref.GetFontIndexByID(attr["id"].Value);
		        var max = int.Parse(attr["index"].Value);
		        if(max > sMap.Count)
		        {
		        	for (int j = 0; j < max; j++) {
			        	sMap.Add(j);
		        	}
		        }
	            sMap.Add(idx);
                if (idx >= 0) {
                    for (int j = 0; j < attr.Count; j++) {
                        switch (attr[j].Name) {
                            case "xLength":
                                pref.fontData[idx].sprite_xLength = int.Parse(attr[j].Value);
                                break;
                            case "yLength":
                                pref.fontData[idx].sprite_yLength = int.Parse(attr[j].Value);
                                break;
                            case "scale":
                                pref.fontData[idx].sprite_scale = float.Parse(attr[j].Value);
                                break;
                            case "lineOffset":
                                pref.fontData[idx].sprite_lineOffset = float.Parse(attr[j].Value);
                                break;
                            case "alphaOnly":
                                pref.fontData[idx].sprite_alphaOnly = int.Parse(attr[j].Value) > 0;
                                break;
                        }
                    }
                    if (pref.fontData[idx].type == TexFontType.Sprite)
                        pref.fontData[idx].PopulateSprite();
                }
            }

            XmlNode SymbolMap = doc.SelectSingleNode("/TexSymbols/SymbolMap");
	        foreach (XmlNode node in SymbolMap.ChildNodes) {
		        int hash = int.Parse(node.Attributes["hash"].Value);
		        if(!isMath && hash >> 8 < pref.fontHeaders[0].contentLength)
			        continue;
		        hash = SyncHash(hash, sMap);
                if (hash >= 0) {
	                c = pref.GetChar(hash);
                    var attr = node.Attributes;
                    for (int j = 0; j < attr.Count; j++) {
                        switch (attr[j].Name) {
                            case "name":
                                c.symbolName = attr[j].Value;
                                break;
                            case "nameAlt":
                                c.symbolAlt = attr[j].Value;
                                break;
                            case "typeId":
                                c.type = (CharType)int.Parse(attr[j].Value);
                                break;
                        }
                    }

                    pref.symbolData.Add(c.symbolName, c.ToHash());
                    if (!string.IsNullOrEmpty(c.symbolAlt))
                        pref.symbolData.Add(c.symbolAlt, c.ToHash());
                }
            }

            XmlNode LargerMap = doc.SelectSingleNode("/TexSymbols/LargerMap");
            foreach (XmlNode node in LargerMap.ChildNodes) {
	            int hash = int.Parse(node.Attributes["hash"].Value);
	            if(!isMath && hash >> 8 < pref.fontHeaders[0].contentLength)
		            continue;
	            hash = SyncHash(hash, sMap);
	            if (hash >= 0) {
                    c = pref.GetChar(hash);
                    c.nextLarger = pref.GetChar(SyncHash(int.Parse(node.Attributes["targetHash"].Value), sMap));
                }
            }

            XmlNode ExtensionMap = doc.SelectSingleNode("/TexSymbols/ExtensionMap");
            foreach (XmlNode node in ExtensionMap.ChildNodes) {
	            int hash = int.Parse(node.Attributes["hash"].Value);
	            if(!isMath && hash >> 8 < pref.fontHeaders[0].contentLength)
		            continue;
	            hash = SyncHash(hash, sMap);
	            if (hash >= 0) {
                    c = pref.GetChar(hash);
                    c.extensionExist = true;
                    var attr = node.Attributes;
                    for (int j = 0; j < attr.Count; j++) {
                        switch (attr[j].Name) {
                            case "top":
                                c.extentTopHash = SyncHash(int.Parse(attr[j].Value), sMap);
                                break;
                            case "middle":
                                c.extentMiddleHash = SyncHash(int.Parse(attr[j].Value), sMap);
                                break;
                            case "bottom":
                                c.extentBottomHash = SyncHash(int.Parse(attr[j].Value), sMap);
                                break;
                            case "repeat":
                                c.extentRepeatHash = SyncHash(int.Parse(attr[j].Value), sMap);
                                break;
                            case "horizontal":
                                c.extensionHorizontal = int.Parse(attr[j].Value) > 0;
                                break;
                        }
                    }
                }
            }

            XmlNode CharMap = doc.SelectSingleNode("/TexSymbols/CharMap");
            foreach (XmlNode node in CharMap.ChildNodes) {
	            int hash = int.Parse(node.Attributes["hash"].Value);
	            if(!isMath && hash >> 8 < pref.fontHeaders[0].contentLength)
		            continue;
	            hash = SyncHash(hash, sMap);
	            if (hash >= 0) {
                    c = pref.GetChar(hash); 
                    c.characterMap = int.Parse(node.Attributes["char"].Value);
                    if (c.characterMap > 0)
                        pref.charMapData.Add(TexChar.possibleCharMaps[c.characterMap], c.ToHash());
                }
            }
        }

        public static void ReadPreferences(TEXPreference pref)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pref.XMLConfiguration.text);

            XmlNode Params = doc.SelectSingleNode("/TexConfigurations/Parameters");
            for (int i = 0; i < Params.ChildNodes.Count; i++) {
                pref.preferences[Params.ChildNodes[i].Attributes[0].Name] = float.Parse(Params.ChildNodes[i].Attributes[0].Value);
                pref.configs[pref.preferences.keys.FindIndex(
                        x => x == Params.ChildNodes[i].Attributes[0].Name)].value = pref.preferences.values[i];
            }

            XmlNode Typefaces = doc.SelectSingleNode("/TexConfigurations/DefaultTypefaces");
            foreach (XmlNode node in Typefaces.ChildNodes) {
                pref.defaultTypefaces.Add((TexCharKind)int.Parse(node.Attributes["code"].Value), int.Parse(node.Attributes["fontId"].Value));
            }
      
            XmlNode GlueTables = doc.SelectSingleNode("/TexConfigurations/GlueTable");
            foreach (XmlNode node in GlueTables.ChildNodes) {
                pref.glueTable[int.Parse(node.Attributes["leftType"].Value) * 10 + int.Parse(node.Attributes["rightType"].Value)] 
			= int.Parse(node.Attributes["glueSize"].Value);
            }

        }

        const string symbolNotice = "AUTO-GENERATED: this file contains per-character configurations, do not modify directly unless you know what are you doing";

	    public static void WriteSymbols(TEXPreference pref, bool isMath)
        {
            StringBuilder syncMap = new StringBuilder();
            StringBuilder symbolMap = new StringBuilder();
            StringBuilder largerMap = new StringBuilder();
            StringBuilder extensionMap = new StringBuilder();
	        StringBuilder charMap = new StringBuilder();
	        var limit = pref.fontHeaders[0].contentLength;
	        var maxPool = (isMath ? limit : pref.fontData.Length);
	        for (int i = isMath? 0 : limit ; i < maxPool; i++) {
                TexFont f = pref.fontData[i];
                if (f.type == TexFontType.Font)
                    syncMap.AppendFormat("     <Font index=\"{0}\" id=\"{1}\"/>\r\n", i, f.id);
                else
                    syncMap.AppendFormat("     <Font index=\"{0}\" id=\"{1}\" xLength=\"{2}\" yLength=\"{3}\" scale=\"{4}\" lineOffset=\"{5}\" detectAlpha=\"{6}\" alphaOnly=\"{7}\"/>\r\n"
                        , i, f.id, f.sprite_xLength, f.sprite_yLength, f.sprite_scale, f.sprite_lineOffset, f.sprite_detectAlpha ? 1 : 0, f.sprite_alphaOnly ? 1 : 0); 
            }
	        for (int i = isMath? 0 : limit ; i < maxPool; i++) {
		        for (int j = 0; j < 128; j++) {
                    TexChar c = pref.GetChar(i, j);
                    if (c.supported) {
                        int h = j | (i << 8);//c.ToHash();
                        if (!string.IsNullOrEmpty(c.symbolName))
                            symbolMap.AppendFormat("\t\t<C{0:X} hash=\"{0}\" name=\"{1}\" nameAlt=\"{2}\" typeId=\"{3}\"/>\n", h, c.symbolName, c.symbolAlt, (int)c.type);
                        if (c.nextLarger != null)
                            largerMap.AppendFormat("\t\t<C{0:X} hash=\"{0}\" targetHash=\"{1}\"/>\n", h, c.nextLarger.ToHash());
                        if (c.extensionExist)
                            extensionMap.AppendFormat("        <C{0:X} hash=\"{0}\" top=\"{1}\" middle=\"{2}\" bottom=\"{3}\" repeat=\"{4}\" horizontal=\"{5}\"/>\r\n", h, 
                                c.extentTopHash, c.extentMiddleHash, c.extentBottomHash, c.extentRepeatHash, c.extensionHorizontal ? 1 : 0);
                        if (c.characterMap > 0)
                            charMap.AppendFormat("\t\t<C{0:X} hash=\"{0}\" char=\"{1}\"/>\n", h, c.characterMap);
                    }
                }	
            }
            string outputString = string.Format("<?xml version='1.0'?>\r\n<!-- {0} -->\r\n<TexSymbols>\r\n    <FontIDs>\r\n{1} </FontIDs>\r\n    <SymbolMap>\r\n{2} </SymbolMap>\r\n    <LargerMap>\r\n{3} </LargerMap>\r\n    <ExtensionMap>\r\n{4} </ExtensionMap>\r\n    <CharMap>\r\n{5} </CharMap>\r\n</TexSymbols>", 
                                      symbolNotice, syncMap, symbolMap, largerMap, extensionMap, charMap);
            //Pro Tips: Use .NET Function to write XML Data
	        if(isMath)
		        File.WriteAllText(Application.dataPath + (pref.MainFolderPath + "/XMLs/TexMathDefinitions.xml").Substring(6), outputString);
	        else
	        File.WriteAllText(Application.dataPath + (pref.MainFolderPath + "/XMLs/TexSymbolDefinitions.xml").Substring(6), outputString);
        }

        const string preferenceNotice = "AUTO-GENERATED: this file contains global TEXDraw preferences, do not modify directly unless you know what are you doing";

        public static void WritePreferences(TEXPreference pref)
        {
            StringBuilder parameters = new StringBuilder();
            StringBuilder typefaces = new StringBuilder();
            StringBuilder glues = new StringBuilder();
            for (int i = 0; i < pref.preferences.Count; i++) {
                parameters.AppendFormat("\t\t<Param {0}=\"{1}\"/>\n", pref.preferences.keys[i], pref.preferences.values[i]);
            }
            for (int i = 0; i < pref.defaultTypefaces.Count; i++) {
                typefaces.AppendFormat("\t\t<MapStyle code=\"{0}\" fontId=\"{1}\"/>\n", i, pref.defaultTypefaces[(TexCharKind)i]);
            }
            for (int i = 0; i < pref.glueTable.Length; i++) {
                if (pref.glueTable[i] > 0)
                    glues.AppendFormat("\t\t<Glue leftType=\"{0}\" rightType=\"{1}\" glueSize=\"{2}\"/>\n", i / 10, i % 10, pref.glueTable[i]);
            }
            string outputString = string.Format("<?xml version='1.0'?>\n<!-- {0} -->\n<TexConfigurations>\n\t<Parameters>\n{1}\t</Parameters>\n\t<DefaultTypefaces>\n{2}\t</DefaultTypefaces>\n\t<GlueTable>\n{3}\t</GlueTable>\n</TexConfigurations>", 
                                      preferenceNotice, parameters, typefaces, glues);
            //Pro Tips: Use .NET Function to write XML Data
            File.WriteAllText(Application.dataPath + (pref.MainFolderPath + "/XMLs/TexConfigurations.xml").Substring(6), outputString);
        }
        #endif
    }
}
#endif