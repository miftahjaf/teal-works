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
        public static void LoadPrimaryDefinitions(TEXPreference pref)
        {
            XmlDocument doc = new XmlDocument();
            EditorUtility.DisplayProgressBar("Reloading", "Reloading from XML Contents...", 0);
            doc.LoadXml(pref.XMLFontDefinitions.text);
            XmlNode fontNodes = doc.SelectSingleNode("/TexFont/Fonts");
            XmlNode groupNodes = doc.SelectSingleNode("/TexFont/FamilyPath");
            XmlNode configNodes = doc.SelectSingleNode("/TexFont/Params");
            XmlNode fNode, gNode, cNode;
            List<TexFont> Datas = new List<TexFont>();
            List<TexFontHeader> Headers = new List<TexFontHeader>();
            List<TexConfigurationMember> configs = new List<TexConfigurationMember>();
            for (int i = 0; i < fontNodes.ChildNodes.Count; i++)
            {
                fNode = fontNodes.ChildNodes[i];
                EditorUtility.DisplayProgressBar("Reloading", string.Format("Reading {0}.ttf ...", fNode.Name), i / (float)fontNodes.ChildNodes.Count / 1.5f);
                string path = pref.MainFolderPath + "/Fonts/" + fNode.Name + "10.ttf";
                Datas.Add(new TexFont(fNode.Name, i, path,
                        fNode.Attributes["name"].Value, fNode.Attributes["description"].Value,
                        int.Parse(fNode.Attributes["modifiable"].Value) > 0));
            }
            if (AssetDatabase.IsValidFolder(pref.MainFolderPath + "/UserFont"))
            {
                string[] customF = AssetDatabase.FindAssets("t:Font", new string[]{ pref.MainFolderPath + "/UserFont" });
                for (int i = 0; i < customF.Length; i++)
                {
                    string realPath = AssetDatabase.GUIDToAssetPath(customF[i]);
                    string id = System.IO.Path.GetFileNameWithoutExtension(realPath).ToLower();
                    Datas.Add(new TexFont(id, Datas.Count, realPath,
                            id, string.Format("A User-Defined {0} Character Set", id), true));           
                }
            }
            EditorUtility.DisplayProgressBar("Reloading", "Preparing Stuff...", .93f);
            for (int i = 0; i < groupNodes.ChildNodes.Count; i++)
            {
                gNode = groupNodes.ChildNodes[i];
                Headers.Add(new TexFontHeader(gNode.Attributes["caption"].Value, int.Parse(gNode.Attributes["idLength"].Value)));
            }
            for (int i = 0; i < configNodes.ChildNodes.Count; i++)
            {
                cNode = configNodes.ChildNodes[i];
                configs.Add(new TexConfigurationMember(cNode.Attributes["name"].Value, cNode.Attributes["desc"].Value, 
                        float.Parse(cNode.Attributes["value"].Value), float.Parse(cNode.Attributes["min"].Value), float.Parse(cNode.Attributes["max"].Value)));
            }	
            pref.fontData = Datas.ToArray();
            pref.fontHeaders = Headers.ToArray();
            pref.configs = configs.ToArray();
            pref.PushToDictionaries(true);
        }

        static Font LoadFont(string ID, TEXPreference pref)
        {
            return AssetDatabase.LoadAssetAtPath<Font>(pref.MainFolderPath + "/Fonts/" + ID + "10.ttf");
        }

        static int SyncHash(int origHash, List<int> syncMap)
        {
            if (syncMap[origHash >> 8] == -1)
                return -1;
            return (syncMap[origHash >> 8] << 8) + (origHash % 128);
        }

        public static void ReadSymbols(TEXPreference pref)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pref.XMLSymbolDefinitions.text);
            TexChar c; 

            XmlNode SyncMap = doc.SelectSingleNode("/TexSymbols/FontIDs");
            List<int> sMap = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                sMap.Add(i);
            }
            for (int i = 0; i < SyncMap.ChildNodes.Count; i++)
            {
                sMap.Add(pref.GetFontIndexByID(SyncMap.ChildNodes[i].Attributes["id"].Value));
            }

            XmlNode SymbolMap = doc.SelectSingleNode("/TexSymbols/SymbolMap");
            foreach (XmlNode node in SymbolMap.ChildNodes)
            {
                int hash = SyncHash(int.Parse(node.Attributes["hash"].Value), sMap);
                if (hash >= 0)
                {
                    c = pref.GetChar(hash);
                    c.symbolName = node.Attributes["name"].Value;
                    c.symbolAlt = node.Attributes["nameAlt"].Value;
                    c.type = (CharType)int.Parse(node.Attributes["typeId"].Value);
                    pref.symbolData.Add(c.symbolName, c.ToHash());
                    if (!string.IsNullOrEmpty(c.symbolAlt))
                        pref.symbolData.Add(c.symbolAlt, c.ToHash());
                }
            }

            XmlNode LargerMap = doc.SelectSingleNode("/TexSymbols/LargerMap");
            foreach (XmlNode node in LargerMap.ChildNodes)
            {
                int hash = SyncHash(int.Parse(node.Attributes["hash"].Value), sMap);
                if (hash >= 0)
                {
                    c = pref.GetChar(hash);
                    c.nextLarger = pref.GetChar(int.Parse(node.Attributes["targetHash"].Value));
                }
            }

            XmlNode ExtensionMap = doc.SelectSingleNode("/TexSymbols/ExtensionMap");
            foreach (XmlNode node in ExtensionMap.ChildNodes)
            {
                int hash = SyncHash(int.Parse(node.Attributes["hash"].Value), sMap);
                if (hash >= 0)
                {
                    c = pref.GetChar(hash);
                    c.extensionExist = true;
                    c.extentTopHash = int.Parse(node.Attributes["top"].Value);
                    c.extentMiddleHash = int.Parse(node.Attributes["middle"].Value);
                    c.extentBottomHash = int.Parse(node.Attributes["bottom"].Value);
                    c.extentRepeatHash = int.Parse(node.Attributes["repeat"].Value);
                }
            }

            XmlNode CharMap = doc.SelectSingleNode("/TexSymbols/CharMap");
            foreach (XmlNode node in CharMap.ChildNodes)
            {
                int hash = SyncHash(int.Parse(node.Attributes["hash"].Value), sMap);
                if (hash >= 0)
                {
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
            for (int i = 0; i < Params.ChildNodes.Count; i++)
            {
                pref.preferences[Params.ChildNodes[i].Attributes[0].Name] = float.Parse(Params.ChildNodes[i].Attributes[0].Value);
                pref.configs[pref.preferences.keys.FindIndex(
                    x => x == Params.ChildNodes[i].Attributes[0].Name)].value = pref.preferences.values[i];
            }

            XmlNode Typefaces = doc.SelectSingleNode("/TexConfigurations/DefaultTypefaces");
            foreach (XmlNode node in Typefaces.ChildNodes)
            {
                pref.defaultTypefaces.Add((TexCharKind)int.Parse(node.Attributes["code"].Value), int.Parse(node.Attributes["fontId"].Value));
            }
      
            XmlNode GlueTables = doc.SelectSingleNode("/TexConfigurations/GlueTable");
            foreach (XmlNode node in GlueTables.ChildNodes)
            {
                pref.glueTable[int.Parse(node.Attributes["leftType"].Value) * 10 + int.Parse(node.Attributes["rightType"].Value)] 
			= int.Parse(node.Attributes["glueSize"].Value);
            }

        }

        const string symbolNotice = "AUTO-GENERATED: this file contains per-character configurations, do not modify directly unless you know what are you doing";

        public static void WriteSymbols(TEXPreference pref)
        {
            StringBuilder syncMap = new StringBuilder();
            StringBuilder symbolMap = new StringBuilder();
            StringBuilder largerMap = new StringBuilder();
            StringBuilder extensionMap = new StringBuilder();
            StringBuilder charMap = new StringBuilder();
            for (int i = 8; i < pref.fontData.Length; i++)
            {
                syncMap.AppendFormat("     <Font index=\"{0}\" id=\"{1}\"/>\n", i, pref.fontData[i].ID); 
            }
            for (int i = 0; i < pref.fontData.Length; i++)
            {
                for (int j = 0; j < 128; j++)
                {
                    TexChar c = pref.GetChar(i, j);
                    if (c.supported)
                    {
                        int h = j | (i << 8);//c.ToHash();
                        if (!string.IsNullOrEmpty(c.symbolName))
                            symbolMap.AppendFormat("\t\t<C{0:X} hash=\"{0}\" name=\"{1}\" nameAlt=\"{2}\" typeId=\"{3}\"/>\n", h, c.symbolName, c.symbolAlt, (int)c.type);
                        if (c.nextLarger != null)
                            largerMap.AppendFormat("\t\t<C{0:X} hash=\"{0}\" targetHash=\"{1}\"/>\n", h, c.nextLarger.ToHash());
                        if (c.extensionExist)
                            extensionMap.AppendFormat("\t\t<C{0:X} hash=\"{0}\" top=\"{1}\" middle=\"{2}\" bottom=\"{3}\" repeat=\"{4}\"/>\n", h, 
                                c.extentTopHash, c.extentMiddleHash, c.extentBottomHash, c.extentRepeatHash);
                        if (c.characterMap > 0)
                            charMap.AppendFormat("\t\t<C{0:X} hash=\"{0}\" char=\"{1}\"/>\n", h, c.characterMap);
                    }
                }	
            }
            string outputString = string.Format("<?xml version='1.0'?>\r\n<!-- {0} -->\r\n<TexSymbols>\r\n    <FontIDs>\r\n{1} </FontIDs>\r\n    <SymbolMap>\r\n{2} </SymbolMap>\r\n    <LargerMap>\r\n{3} </LargerMap>\r\n    <ExtensionMap>\r\n{4} </ExtensionMap>\r\n    <CharMap>\r\n{5} </CharMap>\r\n</TexSymbols>", 
                            symbolNotice, syncMap, symbolMap, largerMap, extensionMap, charMap);
            //Pro Tips: Use .NET Function to write XML Data
            File.WriteAllText(Application.dataPath + (pref.MainFolderPath + "/XMLs/TexSymbolDefinitions.xml").Substring(6), outputString);
        }

        const string preferenceNotice = "AUTO-GENERATED: this file contains global TEXDraw preferences, do not modify directly unless you know what are you doing";

        public static void WritePreferences(TEXPreference pref)
        {
            StringBuilder parameters = new StringBuilder();
            StringBuilder typefaces = new StringBuilder();
            StringBuilder glues = new StringBuilder();
            for (int i = 0; i < pref.preferences.Count; i++)
            {
                parameters.AppendFormat("\t\t<Param {0}=\"{1}\"/>\n", pref.preferences.keys[i], pref.preferences.values[i]);
            }
            for (int i = 0; i < pref.defaultTypefaces.Count; i++)
            {
                typefaces.AppendFormat("\t\t<MapStyle code=\"{0}\" fontId=\"{1}\"/>\n", i, pref.defaultTypefaces[(TexCharKind)i]);
            }
            for (int i = 0; i < pref.glueTable.Length; i++)
            {
                if (pref.glueTable[i] > 0)
                    glues.AppendFormat("\t\t<Glue leftType=\"{0}\" rightType=\"{1}\" glueSize=\"{2}\"/>\n", i / 10, i % 10, pref.glueTable[i]);
            }
            string outputString = string.Format("<?xml version='1.0'?>\n<!-- {0} -->\n<TexConfigurations>\n\t<Parameters>\n{1}\t</Parameters>\n\t<DefaultTypefaces>\n{2}\t</DefaultTypefaces>\n\t<GlueTable>\n{3}\t</GlueTable>\n</TexConfigurations>", 
                            preferenceNotice, parameters, typefaces, glues);
            //Pro Tips: Use .NET Function to write XML Data
            File.WriteAllText(Application.dataPath + (pref.MainFolderPath + "/XMLs/TexConfigurations.xml").Substring(6), outputString);
        }

    }
}
#endif