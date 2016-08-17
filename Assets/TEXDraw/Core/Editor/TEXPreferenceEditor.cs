#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using TexDrawLib;

//TO DO: Add Search Feature & Filter by Type
[CustomEditor(typeof(TEXPreference))]
public class TEXPreferenceEditor : Editor
{
	
    static internal class Styles
    {
        public static GUIContent[] HeaderUpdate = new GUIContent[]
        {
            new GUIContent("Auto Refresh"),
            new GUIContent("Auto Refresh"),
            new GUIContent("Refresh Now")
        };

        public static GUIStyle[] HeaderStyles = new GUIStyle[3]
        {
            new GUIStyle(EditorStyles.miniButtonLeft),
            new GUIStyle(EditorStyles.miniButtonMid),
            new GUIStyle(EditorStyles.miniButtonRight)
        };
        public static GUIStyle ManagerFamily = new GUIStyle(EditorStyles.boldLabel);
        public static GUIStyle ManagerChild = new GUIStyle(EditorStyles.miniButton);
        public static GUIStyle ManagerChildModifiable = new GUIStyle(EditorStyles.miniButtonLeft);
        public static GUIStyle ManagerChildExpand = new GUIStyle(EditorStyles.miniButtonRight);
        public static GUIStyle FontPreviewSymbols = new GUIStyle(EditorStyles.objectFieldThumb);
        public static GUIStyle FontPreviewRelation = new GUIStyle(EditorStyles.textArea);
        public static GUIStyle FontPreviewEnabled = new GUIStyle(EditorStyles.helpBox);
        public static GUIStyle FontPreviewDisabled = new GUIStyle(EditorStyles.label);

        public static GUIStyle[] SetterHeader = new GUIStyle[]
        {
            new GUIStyle(EditorStyles.miniButtonLeft),
            new GUIStyle(EditorStyles.miniButtonRight)
        };
        public static GUIContent[] SetterHeaderContent = new GUIContent[]
        {
            new GUIContent("Properties"),
            new GUIContent("Relations")
        };
        public static GUIStyle SetterPreview = new GUIStyle(EditorStyles.helpBox);
        public static GUIStyle SetterNextLarger = new GUIStyle(EditorStyles.helpBox);
        public static GUIStyle SetterExtendTop = new GUIStyle(EditorStyles.helpBox);
        public static GUIStyle SetterExtendMiddle = new GUIStyle(EditorStyles.helpBox);
        public static GUIStyle SetterExtendBottom = new GUIStyle(EditorStyles.helpBox);
        public static GUIStyle SetterExtendRepeat = new GUIStyle(EditorStyles.helpBox);
        public static GUIStyle SetterTitle = new GUIStyle(EditorStyles.label);
        public static GUIStyle SetterFont = new GUIStyle(EditorStyles.label);
        public static GUIStyle GlueLabelH = new GUIStyle(EditorStyles.label);
        public static GUIStyle GlueLabelV = new GUIStyle(EditorStyles.label);
        public static GUIStyle GlueProgBack;
        public static GUIStyle GlueProgBar;
        public static GUIStyle GlueProgText;

        public static GUIContent[] FontContents = new GUIContent[129];
        public static GUIContent[] SetterCharMap = new GUIContent[28];
        public static GUIContent[] DefaultTypes = new GUIContent[]
        {
            new GUIContent("Numbers"),
            new GUIContent("Capitals"),
            new GUIContent("Small"),
            new GUIContent("Commands"),
            new GUIContent("Text"),
            new GUIContent("Unicode")
        };
        public static GUIContent[] CharTypes = new GUIContent[]
        {
            new GUIContent("Ordinary"),
            new GUIContent("Geometry"),
            new GUIContent("Operator"),
            new GUIContent("Relation"),
            new GUIContent("Arrow"),
            new GUIContent("Open Delimiter"),
            new GUIContent("Close Delimiter"),
            new GUIContent("Big Operator"),
            new GUIContent("Inner"),
//			new GUIContent ("Accent")
        };
        public static int[] DefaultTypesInt = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        public static int[] SetterCharMapInt = new int[28];

        public static GUIContent[] HeaderTitles = new GUIContent[3];
        public static GUIContent[] fontSettings;
        public static GUIStyle Buttons = new GUIStyle(EditorStyles.miniButton);

        static Styles()
        {  
            ManagerFamily.alignment = TextAnchor.MiddleCenter; 
            FontPreviewEnabled.alignment = TextAnchor.MiddleCenter;
            FontPreviewSymbols.alignment = TextAnchor.MiddleCenter;
            FontPreviewRelation.alignment = TextAnchor.MiddleCenter;
            FontPreviewDisabled.alignment = TextAnchor.MiddleCenter;
            FontPreviewRelation.fixedHeight = 0; 
            FontPreviewRelation.onActive = FontPreviewEnabled.onActive; 
            FontPreviewRelation.onNormal = FontPreviewRelation.focused;
            FontPreviewRelation.focused = FontPreviewEnabled.focused;

            SetterTitle.fontStyle = FontStyle.Bold;
            SetterTitle.fontSize = 16;
            SetterFont.richText = true;
            SetterPreview.fontSize = 34;
            SetterPreview.alignment = TextAnchor.MiddleCenter;
            SetterNextLarger.fontSize = 24;
            SetterNextLarger.alignment = TextAnchor.MiddleCenter;

            SetterExtendTop.fontSize = 24;
            SetterExtendTop.alignment = TextAnchor.MiddleCenter;
            SetterExtendMiddle.fontSize = 24;
            SetterExtendMiddle.alignment = TextAnchor.MiddleCenter;
            SetterExtendBottom.fontSize = 24;
            SetterExtendBottom.alignment = TextAnchor.MiddleCenter;
            SetterExtendRepeat.fontSize = 24;
            SetterExtendRepeat.alignment = TextAnchor.MiddleCenter;
            for (int i = 0; i < 129; i++)
            {
                FontContents[i] = new GUIContent(char.ConvertFromUtf32(TEXPreference.TranslateChar(i)));
            }
            for (int i = 0; i < 28; i++)
            {
                SetterCharMap[i] = new GUIContent(new string(TexChar.possibleCharMaps[i], 1));
                SetterCharMapInt[i] = i;
            }
            SetterCharMap[0].text = "(Unassigned)"; //Yeah, just space isn't funny
            SetterCharMap[4].text = "(Forward Slash)"; //It can't be rendered correctly using actual character
            SetterCharMap[27].text = "&&"; //The ampersand character need to be done like this
            HeaderTitles = new GUIContent[3]
            {
                new GUIContent("Symbol & Relations"),
                new GUIContent("Global Configuration"),
                new GUIContent("Glue Management")
            };
            for (int i = 0; i < 3; i++)
            {
                HeaderStyles[i].fontSize = 12;
            }

            GlueLabelH.alignment = TextAnchor.MiddleRight;
            GlueLabelV.alignment = TextAnchor.MiddleLeft;

            GlueProgBack = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ProgressBarBack");
            GlueProgBar = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ProgressBarBar");
            GlueProgText = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("ProgressBarText");
            GlueProgText.alignment = TextAnchor.MiddleCenter;
            Buttons.alignment = TextAnchor.MiddleCenter;
            Buttons.fontSize = 11;
        }
    }

    #region Base of all Renderings

    static TEXPreference targetPreference;
    [SerializeField]
    //0 = Auto Update; 1 = Manual, No Change Applied; 2 = Manual, Pending Change
    int changeState = 0;

    void OnEnable()
    {
        Undo.undoRedoPerformed += RecordRedrawCallback;
        matProp = serializedObject.FindProperty("watchedMaterial");
    }

    void OnDisable()
    {
        if (targetPreference)
        {
            targetPreference.PushToDictionaries();
        } 
        Undo.undoRedoPerformed -= RecordRedrawCallback;
        //EditorApplication.SaveAssets(); //TO DO: Save Asset soon as switching inspector
    }
 
    /*
    void OnDestroy()
    {
        if (prevMesh)
            DestroyImmediate(prevMesh);
        if (prevRender != null)
            prevRender.Cleanup();
    }
    */

    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();
        Rect r = new Rect(46, 24, 146, 16);
        if (headerActive > 0)
        {
            if (GUI.Toggle(r, changeState == 0, Styles.HeaderUpdate[changeState], Styles.Buttons))
            { 
                if (changeState == 1)
                {
                    RecordRedraw();
                    changeState = 0;
                }
                else if (changeState == 2)
                {
                    targetPreference.PushToDictionaries();
                    targetPreference.CallRedraw();
                    changeState = 1;
                }
            }
            else
                changeState = changeState == 0 ? 1 : changeState;
        }
        else
        {
            if (GUI.Button(r, Styles.HeaderUpdate[2], Styles.Buttons))
            {
                targetPreference.PushToDictionaries();
                targetPreference.CallRedraw();
                EditorUtility.SetDirty(targetPreference);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (!targetPreference)
        {
            targetPreference = TEXPreference.main;
            if (selectedFont != null)
            {
                Styles.FontPreviewEnabled.font = selectedFont.font;
                Styles.FontPreviewSymbols.font = selectedFont.font;
                Styles.FontPreviewRelation.font = selectedFont.font;
                Styles.SetterPreview.font = selectedFont.font;
            }
        }
        RecordUndo();
        DrawHeaderOption();
        if (headerActive == 0)
        {
            Rect v = EditorGUILayout.GetControlRect(GUILayout.Height(5));
            DrawManager();
            EditorGUI.indentLevel = 0;
            EditorGUILayout.Separator();
            if (selectedFont != null)
            {
                v.xMin += EditorGUIUtility.labelWidth + 4;
                v.height = Screen.height - ViewerHeight;
                CheckEvent(false);
                DrawViewer(v);
                DrawSetter();
            }
        }
        else if (headerActive == 1)
            DrawConfiguration();
        else if (headerActive == 2)
            DrawGlue();
        serializedObject.ApplyModifiedProperties();
    }

    [SerializeField]
    int headerActive = 0;

    void DrawHeaderOption()
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(24));
        r.width /= 3f;
        for (int i = 0; i < 3; i++)
        {
            if (GUI.Toggle(r, i == headerActive, Styles.HeaderTitles[i], Styles.HeaderStyles[i]))
                headerActive = i;
            r.x += r.width;
        }
    }

    #endregion

    #region Character Management

    const float ViewerHeight = 120f;
    const float SetterHeight = 520f;
    [SerializeField]
    Vector2 ViewerScroll;
    [SerializeField]
    Vector2 ManagerScroll;
    [SerializeField]
    Vector2 SetterScroll;
    [SerializeField]
    int selectedFontIdx;
    [SerializeField]
    int selectedCharIdx;

    TexFont selectedFont
    {
        get { return targetPreference.fontData[selectedFontIdx]; }
        set { selectedFontIdx = value.index; }
    }

    TexChar selectedChar
    {
        get { return targetPreference.fontData[selectedFontIdx].chars[selectedCharIdx]; }
        set
        {
            selectedCharIdx = value.index;
            selectedFontIdx = value.fontIndex;
        }
    }

    bool lastCharChanged = false;
    int setterState = 0;

    void DrawManager()
    {
        ManagerScroll = EditorGUILayout.BeginScrollView(ManagerScroll, false, true, GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.MaxHeight(Screen.height - SetterHeight));
        int Total = 15 + targetPreference.fontHeaders.Length, pool = 0, headerPool = 0, dataPool = 0;
        for (int i = 0; i < Total; i++)
        {
            if (pool == 0)
            {
                EditorGUI.indentLevel = 0;
                TexFontHeader h = targetPreference.fontHeaders[headerPool];
                EditorGUILayout.LabelField(h.caption, Styles.ManagerFamily, GUILayout.Width(EditorGUIUtility.labelWidth - 30));

                pool = h.contentLength;
                headerPool++;
            }
            else
            {
                TexFont d = targetPreference.fontData[dataPool];
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(EditorGUIUtility.labelWidth - 24));
                if (d.modifiable)
                    r.width -= 40;
                if ((selectedFontIdx == dataPool) != EditorGUI.ToggleLeft(r, d.Name, selectedFontIdx == dataPool, d.modifiable ? Styles.ManagerChildModifiable : Styles.ManagerChild))
                {
                    selectedFontIdx = dataPool;
                    Styles.FontPreviewEnabled.font = d.font;
                    Styles.FontPreviewSymbols.font = d.font;
                    Styles.FontPreviewRelation.font = d.font;
                    Styles.SetterPreview.font = d.font;
                }
                if (d.modifiable)
                {
                    r.x += r.width;
                    r.width = 40;
                    int modIdx = d.index - 8;
                    modIdx = EditorGUI.Popup(r, modIdx, targetPreference.ModifiableIDs, Styles.ManagerChildExpand);
                    if (modIdx != d.index - 8)
                    {
                        
                        targetPreference.ShiftFontIndex(d.index, modIdx + 8);
                        RecordDirty();
                    }
                }
                pool--;
                dataPool++;
            }
        }
        EditorGUILayout.EndScrollView();
    }

    GUIStyle SubDetermineStyle(TexChar c)
    {
        if (c.supported)
        {	
            if (!string.IsNullOrEmpty(c.symbolName))
                return Styles.FontPreviewSymbols;
            else if (c.nextLargerExist || c.extensionExist)
                return Styles.FontPreviewRelation;
            else
                return Styles.FontPreviewEnabled;
        }
        else
            return Styles.FontPreviewDisabled;
    }

    void DrawViewer(Rect drawRect)
    {
        Rect r;
        Vector2 childSize = new Vector2(drawRect.width / 8f - 4, selectedFont.font.lineHeight * (drawRect.width / 250) + 15);
        ViewerScroll = GUI.BeginScrollView(drawRect, ViewerScroll, new Rect(Vector2.zero, new Vector2((childSize.x + 2) * 8 - 2, (childSize.y + 2) * 16)));
        Styles.FontPreviewEnabled.fontSize = (int)childSize.x / 2;
        Styles.FontPreviewSymbols.fontSize = (int)childSize.x / 2;
        Styles.FontPreviewRelation.fontSize = (int)childSize.x / 2;

        for (int x = 0; x < 16; x++)
        {
            r = new Rect(new Vector2(1, x * (childSize.y + 2)), childSize);
            for (int y = 0; y < 8; y++)
            {
                int z = (x << 3) | y;
                bool selectable = selectedFont.chars[z].supported;
                int l = selectedCharIdx;
                if (CustomToggle(r, selectedCharIdx == z, selectable, Styles.FontContents[z], SubDetermineStyle(selectedFont.chars[z])))
                {
                    int newS = z + (selectedCharIdx - l);
                    if (newS != selectedCharIdx && lastCharChanged)
                    {
                        RecordDirty();
                        lastCharChanged = false;
                    }
                    selectedCharIdx = newS;
                }
                r.x += r.width + 2;
            }
        }
        GUI.EndScrollView();
    }

    void DrawSetter()
    {
        GUILayoutOption max = GUILayout.MaxWidth(EditorGUIUtility.labelWidth);

        EditorGUILayout.LabelField(selectedFont.Name, Styles.SetterTitle, max, GUILayout.MaxHeight(25));
        EditorGUILayout.SelectableLabel(selectedFont.Description, max, GUILayout.Height(18));
        EditorGUILayout.LabelField(string.Format("ID \t : <b>{0}</b> (#{1:X})", selectedFont.ID, selectedFont.index), Styles.SetterFont, max);

        TexChar c = selectedChar;
        if (c.supported)
        {
            GUILayoutOption max2 = GUILayout.MaxWidth(EditorGUIUtility.labelWidth / 2);
            Rect r = EditorGUILayout.GetControlRect(max2);
            for (int i = 0; i < 2; i++)
            {
                if (GUI.Toggle(r, i == setterState, Styles.SetterHeaderContent[i], Styles.SetterHeader[i]))
                    setterState = i;
                r.x += r.width;
            }
            EditorGUI.BeginChangeCheck();
            if (setterState == 0)
            {
                EditorGUILayout.LabelField(Styles.FontContents[selectedCharIdx], Styles.SetterPreview, GUILayout.Height(selectedFont.font.lineHeight * 2.2f), max);
                EditorGUILayout.LabelField(string.Format("Index \t : <b>{0}</b> (#{0:X2})", selectedCharIdx), Styles.SetterFont, max);
                EditorGUILayout.LabelField(string.Format("Writed as \t : <b>{0}</b> (#{1:X2})", (char)TEXPreference.TranslateChar(selectedCharIdx), TEXPreference.TranslateChar(selectedCharIdx)), Styles.SetterFont, max);

                EditorGUILayout.LabelField("Symbol Definition", max);
                r = EditorGUILayout.GetControlRect(max2);
                c.symbolAlt = EditorGUI.TextField(r, c.symbolAlt); //Secondary
                r.x += r.width;
                c.symbolName = EditorGUI.TextField(r, c.symbolName); //Primary
                r = EditorGUILayout.GetControlRect(max2);
                EditorGUI.LabelField(r, "Symbol Type");
                r.x += r.width;
                c.type = (CharType)EditorGUI.EnumPopup(r, c.type);
                r = EditorGUILayout.GetControlRect(max2);
                EditorGUI.LabelField(r, "Mapped As"); 
                r.x += r.width;
                c.characterMap = EditorGUI.IntPopup(r, c.characterMap, Styles.SetterCharMap, Styles.SetterCharMapInt);		
            }
            else
            {
                SetterScroll = EditorGUILayout.BeginScrollView(SetterScroll, max);
                max = GUILayout.Width(EditorGUIUtility.labelWidth - 24);
                max2 = GUILayout.Width(EditorGUIUtility.labelWidth / 2 - 12);
                EditorGUILayout.LabelField(string.Format("Index \t : <b>{1}</b> (#{0:X1}{1:X2})", selectedFontIdx, selectedCharIdx), Styles.SetterFont, max);
                c.nextLargerHash = SubDrawThumbnail(c.nextLargerHash, max, max2, "Is Larger Character Exist?", Styles.SetterNextLarger);
                EditorGUILayout.Space();
                if (EditorGUILayout.ToggleLeft("Is Part of Extension?", c.extensionExist, max))
                {
                    EditorGUI.indentLevel++;
                    c.extensionExist = true;
                    c.extentTopHash = SubDrawThumbnail(c.extentTopHash, max, max2, "Has Top Extension?", Styles.SetterExtendTop);
                    c.extentMiddleHash = SubDrawThumbnail(c.extentMiddleHash, max, max2, "Has Middle Extension?", Styles.SetterExtendMiddle);
                    c.extentBottomHash = SubDrawThumbnail(c.extentBottomHash, max, max2, "Has Bottom Extension?", Styles.SetterExtendBottom);
                    c.extentRepeatHash = SubDrawThumbnail(c.extentRepeatHash, max, max2, "Has Tiled Extension?", Styles.SetterExtendRepeat);
                    EditorGUI.indentLevel--;
                }
                else
                    c.extensionExist = false;
                EditorGUILayout.EndScrollView();
            }
            if (EditorGUI.EndChangeCheck())
            {
                RecordDirty();
                lastCharChanged = true;
            }
        }
        else
        {
            EditorGUILayout.LabelField("<i>Select a Supported Character</i>", Styles.SetterFont, max);
        }
    }

    int SubDrawThumbnail(int hash, GUILayoutOption max, GUILayoutOption max2, string confirmTxt, GUIStyle style)
    {
        if (EditorGUILayout.ToggleLeft(confirmTxt, hash != -1, max))
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginChangeCheck();
            if (hash < 0)
                hash = selectedChar.ToHash() + 1;
            int font = hash >> 8, ch = hash % 128;
            Rect r = EditorGUILayout.GetControlRect(max2);
            font = Mathf.Clamp(EditorGUI.IntField(r, font), 0, 15);
            r.y += r.height * 2;
            ch = Mathf.Clamp(EditorGUI.IntField(r, ch), 0, 127);
            style.font = targetPreference.fontData[font].font;
            r.y -= r.height * 2;
            r.x += r.width + 2;
            r.height = targetPreference.fontData[font].font.lineHeight * 1.7f;
            EditorGUI.indentLevel--;
            EditorGUI.LabelField(r, Styles.FontContents[ch], style);
            EditorGUILayout.GetControlRect(GUILayout.Height(r.height - 18), max);
            if (EditorGUI.EndChangeCheck())
                return TEXPreference.CharToHash(font, ch);
            else
                return hash;
        }
        else
            return -1;
    }

    void CheckEvent(bool noCmd)
    {
        Event e = Event.current;
        if (headerActive == 0 && selectedFont != null && selectedFont.chars[selectedCharIdx].supported)
        {
            if (e.isKey & e.type != EventType.KeyUp)
            {
                if (e.control | noCmd)
                {
                    doInput:
                    if (e.keyCode == KeyCode.UpArrow)
                        selectedCharIdx = (int)Mathf.Repeat(selectedCharIdx - 8, 128);
                    else if (e.keyCode == KeyCode.DownArrow)
                        selectedCharIdx = (int)Mathf.Repeat(selectedCharIdx + 8, 128);
                    else if (e.keyCode == KeyCode.LeftArrow)
                        selectedCharIdx = (int)Mathf.Repeat(selectedCharIdx - 1, 128);
                    else if (e.keyCode == KeyCode.RightArrow)
                        selectedCharIdx = (int)Mathf.Repeat(selectedCharIdx + 1, 128);
                    else if (e.keyCode == KeyCode.Home)
                        selectedFont.chars[selectedCharIdx].type = (CharType)(int)Mathf.Repeat((int)selectedFont.chars[selectedCharIdx].type - 1, 9);
                    else if (e.keyCode == KeyCode.End)
                        selectedFont.chars[selectedCharIdx].type = (CharType)(int)Mathf.Repeat((int)selectedFont.chars[selectedCharIdx].type + 1, 9);
                    else
                        goto skipUse;
                    if (!selectedFont.chars[selectedCharIdx].supported)
                        goto doInput;
                    float ratio = (selectedFont.lineHeightRaw * (Screen.width - EditorGUIUtility.labelWidth - 60) / 250f);
                    //int maxRatio = Screen.height / (int)ratio;
                    ViewerScroll.y = Mathf.Clamp(ViewerScroll.y, (selectedCharIdx / 8 - 3) * ratio, (selectedCharIdx / 8 - 1) * ratio);
                    e.Use();
                    skipUse:
                    return;
                }
            }
        }
    }

    #endregion

    #region Configuration

    const float configHeight = 355;
    [SerializeField]
    Vector2 configScroll;
    [SerializeField]
    Vector2 configTypeScroll;
    SerializedProperty matProp;

    void DrawConfiguration()
    {
        if (Styles.fontSettings == null)
        {
            Styles.fontSettings = new GUIContent[targetPreference.fontData.Length];
            for (int i = 0; i < Styles.fontSettings.Length; i++)
            {
                Styles.fontSettings[i] = new GUIContent(targetPreference.fontData[i].ID);
            }
        }

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Global Configurations", Styles.SetterTitle, GUILayout.Height(24));
        configScroll = EditorGUILayout.BeginScrollView(configScroll, GUILayout.Height(Screen.height - configHeight));
        for (int i = 0; i < targetPreference.configs.Length; i++)
        {
            TexConfigurationMember c = targetPreference.configs[i];
            c.value = EditorGUILayout.Slider(new GUIContent(c.name, c.desc), c.value, c.min, c.max); 
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.GetControlRect();
        configTypeScroll = EditorGUILayout.BeginScrollView(configTypeScroll, GUILayout.Height(configHeight - 172));
        EditorGUILayout.LabelField("Default Typeface", Styles.SetterTitle, GUILayout.Height(24));
        EditorGUI.indentLevel++;
        for (int i = 0; i < 6; i++)
        {
            targetPreference.defaultTypefaces[(TexCharKind)i] = EditorGUILayout.IntPopup(Styles.DefaultTypes[i],
                targetPreference.defaultTypefaces[(TexCharKind)i], Styles.fontSettings, Styles.DefaultTypesInt);
        }
        EditorGUI.indentLevel--;
        EditorGUILayout.LabelField("Material Setup", Styles.SetterTitle, GUILayout.Height(24));
        EditorGUI.indentLevel++;
        EditorGUI.BeginChangeCheck();
        targetPreference.defaultMaterial = (Material)EditorGUILayout.ObjectField("Default Material", targetPreference.defaultMaterial, typeof(Material), false);
        EditorGUILayout.PropertyField(matProp, true);
        EditorGUI.indentLevel--;
        if (EditorGUI.EndChangeCheck())
            targetPreference.RebuildMaterial();
        EditorGUILayout.EndScrollView();
        if (EditorGUI.EndChangeCheck())
        {
            RecordDirty();
        }
    }

    /* Pending Functions - Additional Stuff for live preview
    Mesh prevMesh;
    PreviewRenderUtility prevRender;
    string prevString = "x";
    DrawingContext prevContext;
    DrawingParams prevParams;

    void ValidateParams()
    {
        //Any Non-Serializabled Classes are loaded here.
        if (prevContext == null)
        {
            prevRender = new PreviewRenderUtility();
            prevRender.m_Camera.transform.position = Vector3.back * 7; 

            prevContext = new DrawingContext();
            prevParams = new DrawingParams();
            prevParams.alignment = Vector2.one * 0.5f;
            prevParams.autoFit = true;
            //prevParams.hasRect = true;
            prevParams.color = Color.white;
            prevParams.pivot = Vector2.one * 0.5f;
            prevParams.spaceSize = 0.5f;
            prevParams.scale = 30;
            prevParams.rectArea = new Rect(-10, -5, 20, 10);
        }
    }

    void ValidatePreview()
    {
        if (!prevMesh)
            prevMesh = new Mesh();
        ValidateParams();
        prevContext.Parse(prevString);
        prevContext.Render(prevMesh, prevParams);
    }

    public override bool HasPreviewGUI()
    {
        //	return base.HasPreviewGUI();
        return headerActive == 1;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
        if (Event.current.type == EventType.Repaint)
        {
            ValidatePreview();
            prevRender.BeginPreview(r, background);

            prevRender.DrawMesh(prevMesh, Vector3.zero, Quaternion.identity, targetPreference.defaultMaterial, 0);
            prevRender.m_Camera.Render();
            Texture image = prevRender.EndPreview();
            //GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);
            EditorGUI.DrawPreviewTexture(r, image);
        }
    }
    */

    #endregion

    #region Glue Management

    void DrawGlue()
    {
        labelMatrixHeight = (Screen.width - 150) / 9f;
        glueSimmetry = GUI.Toggle(EditorGUILayout.GetControlRect(GUILayout.Height(22)), glueSimmetry, "Edit Symmetrically", Styles.Buttons);
        //labelMatrixWidth = ;
        SubDrawMatrix();
    }

    int GlueGet(int l, int r)
    {
        return targetPreference.glueTable[l * 10 + r];
    }

    void GlueSet(int l, int r, int v)
    {
        targetPreference.glueTable[l * 10 + r] = v;
        RecordDirty();
    }

    const float labelMatrixWidth = 110;
    float labelMatrixHeight = 38;
    [SerializeField]
    bool glueSimmetry = true;
    Vector2 scrollGlue;

    void SubDrawMatrix()
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(labelMatrixWidth));
        GUI.matrix = Matrix4x4.TRS(new Vector2(labelMatrixWidth - r.x, r.y + labelMatrixWidth), Quaternion.Euler(0, 0, -90), Vector3.one);
        r.position = Vector2.zero;
        r.y += labelMatrixHeight / 2 + 4;
        for (int i = 0; i < 9; i++)
        {
            EditorGUI.LabelField(r, Styles.CharTypes[i], Styles.GlueLabelV);
            r.y += labelMatrixHeight;
        }
        GUI.matrix = Matrix4x4.identity;
        EditorGUILayout.GetControlRect(GUILayout.Height(labelMatrixWidth - 36));
        scrollGlue = EditorGUILayout.BeginScrollView(scrollGlue, GUILayout.Height(Screen.height - 245));
        r = EditorGUILayout.GetControlRect(GUILayout.Width(labelMatrixWidth));
        r.height = labelMatrixHeight;
        float xx = r.x;
        int cur, now;
        for (int i = 0; i < 9; i++)
        {
            GUI.Label(r, Styles.CharTypes[i], Styles.GlueLabelH);
            r.x += labelMatrixWidth;
            r.width = labelMatrixHeight;
            for (int j = 0; j < 9; j++)
            {
                if (glueSimmetry)
                {
                    cur = GlueGet(i, j) != GlueGet(j, i) ? -1 : GlueGet(i, j);
                    now = CustomTuner(r, cur);
                    if (cur != now)
                    {
                        GlueSet(i, j, now);
                        GlueSet(j, i, now);
                    }
                }
                else
                {
                    cur = GlueGet(i, j);
                    now = CustomTuner(r, cur);
                    if (cur != now)
                        GlueSet(i, j, now);
                }
                r.x += labelMatrixHeight;
                if (glueSimmetry && ((j) >= (i)))
                    break;
            }
            r.x = xx;
            r.y += labelMatrixHeight;
            r.width = labelMatrixWidth;
        }
        EditorGUILayout.GetControlRect(GUILayout.Height(labelMatrixHeight * 9));
        EditorGUILayout.EndScrollView();
    }

    #endregion

    #region Undo & Functionality

    void RecordDirty()
    {
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(targetPreference);
        switch (changeState)
        {
            case 0:
                RecordRedraw();
                break;
            case 1:
                changeState = 2;
                break;
        }
    }

    void RecordRedrawCallback()
    {
        switch (changeState)
        {
            case 0:
                RecordRedraw();
                break;
            case 1:
                changeState = 2;
                break;
        }
    }

    void RecordRedraw()
    {
        if (headerActive == 1)
            targetPreference.PushToDictionaries(true);
        if (headerActive > 0)
            targetPreference.CallRedraw();
    }

    void RecordUndo()
    {
        //   Undo.IncrementCurrentGroup();
        Undo.RecordObjects(new Object[]{ targetPreference, this }, "Changes to TEXDraw Preference");
    }

    [MenuItem("Edit/Project Settings/TEXDraw Preference")]
    public static void OpenPreference()
    {
        if (!targetPreference)
        {
            targetPreference = TEXPreference.main;
        }
        Selection.activeObject = targetPreference;
    }

    #endregion

    #region Custom GUI Controls

    const int customToggleHash = 0x05f8;

    bool CustomToggle(Rect r, bool value, bool selectable, GUIContent content, GUIStyle style)
    {
        //TO DO: Add functionality for Tab & Page Up/Down
        int controlID = GUIUtility.GetControlID(customToggleHash, selectable ? EditorGUIUtility.native : FocusType.Passive);
        bool result = GUI.Toggle(r, value, content, style);
        if (value != result)
            GUIUtility.keyboardControl = controlID;
        if (GUIUtility.keyboardControl == controlID)
            CheckEvent(true);
        return result;// || ;
    }

    const int customTunerHash = 0x08e3;

    int CustomTuner(Rect r, int value)
    {
        int controlID = GUIUtility.GetControlID(customTunerHash, EditorGUIUtility.native, r);
        Event current = Event.current;
        EventType typeForControl = current.GetTypeForControl(controlID);
        if (typeForControl == EventType.Repaint)
        {
            Styles.GlueProgBack.Draw(r, false, false, false, false);
            Rect r2 = new Rect(r);
            r2.yMin = Mathf.Lerp(r2.yMax, r2.yMin, value == 10 ? 1 : (value * 0.06f + 0.2f));
            if (value > 0)
                Styles.GlueProgBar.Draw(r2, false, false, false, false);
            Styles.GlueProgText.Draw(r, value == -1 ? "--" : value.ToString(), false, false, false, false);
        }
        else if (typeForControl == EventType.MouseDrag)
        {
            Vector2 mousePos = (current.mousePosition);
            if (!r.Contains(mousePos))
                return value;
            float normValue = Mathf.InverseLerp(r.yMin, r.yMax, mousePos.y);
            value = Mathf.Clamp(Mathf.FloorToInt((Mathf.Sqrt((1 - normValue)) / 0.6f - 0.4f) * 10f), 0, 10);
        }
        else if (typeForControl == EventType.MouseDown)
        {
            if (!r.Contains(current.mousePosition))
                return value;
            return (int)Mathf.Repeat(value + (current.shift ? -1 : 1), 11);
        }
        return value;
    }

    #endregion

}
#endif