#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TEXDraw3D))]
[CanEditMultipleObjects]
public class TEXDraw3DEditor : Editor
{
    SerializedProperty m_Text;
    SerializedProperty m_FontIndex;
    SerializedProperty m_Size;
    SerializedProperty m_FontSize;
    SerializedProperty m_SpaceSize;
    SerializedProperty m_Align;
    SerializedProperty m_Color;
    SerializedProperty m_Material;
	
    SerializedProperty m_debugReport;
    //static bool foldExpand = false;

    // Use this for initialization
    void OnEnable()
    {
        m_Text = serializedObject.FindProperty("m_Text");
        m_FontIndex = serializedObject.FindProperty("m_FontIndex");
        m_Size = serializedObject.FindProperty("m_Size");
        m_FontSize = serializedObject.FindProperty("m_FontSize");
        m_Align = serializedObject.FindProperty("m_Align");
        m_SpaceSize = serializedObject.FindProperty("m_SpaceSize");
        m_Color = serializedObject.FindProperty("m_Color");
        m_Material = serializedObject.FindProperty("m_Material");
        m_debugReport = serializedObject.FindProperty("debugReport");		
    }
	
    // Update is called once per frame
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
		
        EditorGUILayout.PropertyField(m_Text);
        EditorGUILayout.PropertyField(m_Size);
        //foldExpand = EditorGUILayout.Foldout(foldExpand, "More Properties");
        //if (foldExpand)
        {
            EditorGUI.indentLevel++;
            DoFontIndexSelection();

            EditorGUILayout.PropertyField(m_FontSize);
            DoTextAligmentControl(EditorGUILayout.GetControlRect(), m_Align);
            EditorGUILayout.PropertyField(m_SpaceSize);
            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.PropertyField(m_Material);		
            EditorGUI.indentLevel--;
        }

        if (serializedObject.targetObjects.Length == 1)
        {
            if (m_debugReport.stringValue == System.String.Empty)
                EditorGUILayout.HelpBox("No Errors", MessageType.Info);
            else
                EditorGUILayout.HelpBox(m_debugReport.stringValue, MessageType.Warning);
        }
        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
            Redraw();
        
    }

    public void DoFontIndexSelection()
    {
        TEXDraw3D t = (TEXDraw3D)target;
        if (!m_FontIndex.hasMultipleDifferentValues)
        {
            m_FontIndex.intValue = EditorGUILayout.IntPopup("Font Index", m_FontIndex.intValue, t.pref.FontIDs, t.pref.FontIndexs);
            return;
        }
        int v = EditorGUILayout.IntPopup("Font Index", -2, t.pref.FontIDs, t.pref.FontIndexs);
        if (v == -2)
            return;
        m_FontIndex.intValue = v;
    }

	
    public void Redraw()
    { 
        foreach (TEXDraw3D i in (serializedObject.targetObjects))
        {
            i.Redraw();   
            i.Repaint();
        }
    }
	
	
    //These bunch of Codes are actually coming from UnityEditor.UI thanks for it's open source code

    #region Alignment Control

    //The license below applies for this region only
    /*
		The MIT License (MIT)

		Copyright (c) 2014-2015, Unity Technologies

		Permission is hereby granted, free of charge, to any person obtaining a copy
		of this software and associated documentation files (the "Software"), to deal
		in the Software without restriction, including without limitation the rights
		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
		copies of the Software, and to permit persons to whom the Software is
		furnished to do so, subject to the following conditions:

		The above copyright notice and this permission notice shall be included in
		all copies or substantial portions of the Software.

		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
		THE SOFTWARE.
	*/
	
    private const int kAlignmentButtonWidth = 20;
	
    static int s_TextAlignmentHash = "DoTextAligmentControl".GetHashCode();

    static private class Styles
    {
        public static GUIStyle alignmentButtonLeft = new GUIStyle(EditorStyles.miniButtonLeft);
        public static GUIStyle alignmentButtonMid = new GUIStyle(EditorStyles.miniButtonMid);
        public static GUIStyle alignmentButtonRight = new GUIStyle(EditorStyles.miniButtonRight);
		
        public static GUIContent m_EncodingContent;
		
        public static GUIContent m_LeftAlignText;
        public static GUIContent m_CenterAlignText;
        public static GUIContent m_RightAlignText;
        public static GUIContent m_TopAlignText;
        public static GUIContent m_MiddleAlignText;
        public static GUIContent m_BottomAlignText;
		
        public static GUIContent m_LeftAlignTextActive;
        public static GUIContent m_CenterAlignTextActive;
        public static GUIContent m_RightAlignTextActive;
        public static GUIContent m_TopAlignTextActive;
        public static GUIContent m_MiddleAlignTextActive;
        public static GUIContent m_BottomAlignTextActive;

        static Styles()
        {
            m_EncodingContent = new GUIContent("Rich Text", "Use emoticons and colors");
			
            // Horizontal Aligment Icons
            m_LeftAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left", "Left Align");
            m_CenterAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center", "Center Align");
            m_RightAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right", "Right Align");
            m_LeftAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_left_active", "Left Align");
            m_CenterAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_center_active", "Center Align");
            m_RightAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_horizontally_right_active", "Right Align");
			
            // Vertical Aligment Icons
            m_TopAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top", "Top Align");
            m_MiddleAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center", "Middle Align");
            m_BottomAlignText = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom", "Bottom Align");
            m_TopAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_top_active", "Top Align");
            m_MiddleAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_center_active", "Middle Align");
            m_BottomAlignTextActive = EditorGUIUtility.IconContent(@"GUISystem/align_vertically_bottom_active", "Bottom Align");
			
            FixAlignmentButtonStyles(alignmentButtonLeft, alignmentButtonMid, alignmentButtonRight);
        }

        static void FixAlignmentButtonStyles(params GUIStyle[] styles)
        {
            foreach (GUIStyle style in styles)
            {
                style.padding.left = 2;
                style.padding.right = 2;
            }
        }
    }

	
    private void DoTextAligmentControl(Rect position, SerializedProperty alignment)
    {
        GUIContent alingmentContent = new GUIContent("Alignment");
		
        int id = EditorGUIUtility.GetControlID(s_TextAlignmentHash, EditorGUIUtility.native, position);
		
        EditorGUIUtility.SetIconSize(new Vector2(15, 15));
        EditorGUI.BeginProperty(position, alingmentContent, alignment);
        {
            Rect controlArea = EditorGUI.PrefixLabel(position, id, alingmentContent);
			
            float width = kAlignmentButtonWidth * 3;
            float spacing = Mathf.Clamp(controlArea.width - width * 2, 2, 10);
			
            Rect horizontalAligment = new Rect(controlArea.x, controlArea.y, width, controlArea.height);
            Rect verticalAligment = new Rect(horizontalAligment.xMax + spacing, controlArea.y, width, controlArea.height);
			
            DoHorizontalAligmentControl(horizontalAligment, alignment);
            DoVerticalAligmentControl(verticalAligment, alignment);
        }
        EditorGUI.EndProperty();
        EditorGUIUtility.SetIconSize(Vector2.zero);
    }

    private static void DoHorizontalAligmentControl(Rect position, SerializedProperty alignment)
    {
        float horizontalAlignment = alignment.vector2Value.x;
		
        bool leftAlign = (horizontalAlignment == 0f);
        bool centerAlign = (horizontalAlignment == 0.5f);
        bool rightAlign = (horizontalAlignment == 1f);
		
        if (alignment.hasMultipleDifferentValues)
        {
            foreach (var obj in alignment.serializedObject.targetObjects)
            {
                TEXDraw3D text = obj as TEXDraw3D;
                horizontalAlignment = text.alignment.x;
                leftAlign = leftAlign || (horizontalAlignment == 0f);
                centerAlign = centerAlign || (horizontalAlignment == 0.5f);
                rightAlign = rightAlign || (horizontalAlignment == 1f);
            }
        }
		
        position.width = kAlignmentButtonWidth;
		
        EditorGUI.BeginChangeCheck();
        EditorToggle(position, leftAlign, leftAlign ? Styles.m_LeftAlignTextActive : Styles.m_LeftAlignText, Styles.alignmentButtonLeft);
        if (EditorGUI.EndChangeCheck())
        {
            SetHorizontalAlignment(alignment, 0f);
        }
		
        position.x += position.width;
        EditorGUI.BeginChangeCheck();
        EditorToggle(position, centerAlign, centerAlign ? Styles.m_CenterAlignTextActive : Styles.m_CenterAlignText, Styles.alignmentButtonMid);
        if (EditorGUI.EndChangeCheck())
        {
            SetHorizontalAlignment(alignment, 0.5f);
        }
		
        position.x += position.width;
        EditorGUI.BeginChangeCheck();
        EditorToggle(position, rightAlign, rightAlign ? Styles.m_RightAlignTextActive : Styles.m_RightAlignText, Styles.alignmentButtonRight);
        if (EditorGUI.EndChangeCheck())
        {
            SetHorizontalAlignment(alignment, 1f);
        }
    }

    private static void DoVerticalAligmentControl(Rect position, SerializedProperty alignment)
    {
        float verticalTextAligment = alignment.vector2Value.y;
		
        bool topAlign = (verticalTextAligment == 1f);
        bool middleAlign = (verticalTextAligment == 0.5f);
        bool bottomAlign = (verticalTextAligment == 0f);
		
        if (alignment.hasMultipleDifferentValues)
        {
            foreach (var obj in alignment.serializedObject.targetObjects)
            {
                TEXDraw3D text = obj as TEXDraw3D;
                verticalTextAligment = text.alignment.y;
                topAlign = topAlign || (verticalTextAligment == 1f);
                middleAlign = middleAlign || (verticalTextAligment == 0.5f);
                bottomAlign = bottomAlign || (verticalTextAligment == 0f);
            }
        }
		
		
        position.width = kAlignmentButtonWidth;
		
        // position.x += position.width;
        EditorGUI.BeginChangeCheck();
        EditorToggle(position, topAlign, topAlign ? Styles.m_TopAlignTextActive : Styles.m_TopAlignText, Styles.alignmentButtonLeft);
        if (EditorGUI.EndChangeCheck())
        {
            SetVerticalAlignment(alignment, 1f);
        }
		
        position.x += position.width;
        EditorGUI.BeginChangeCheck();
        EditorToggle(position, middleAlign, middleAlign ? Styles.m_MiddleAlignTextActive : Styles.m_MiddleAlignText, Styles.alignmentButtonMid);
        if (EditorGUI.EndChangeCheck())
        {
            SetVerticalAlignment(alignment, 0.5f);
        }
		
        position.x += position.width;
        EditorGUI.BeginChangeCheck();
        EditorToggle(position, bottomAlign, bottomAlign ? Styles.m_BottomAlignTextActive : Styles.m_BottomAlignText, Styles.alignmentButtonRight);
        if (EditorGUI.EndChangeCheck())
        {
            SetVerticalAlignment(alignment, 0f);
        }
    }

    private static bool EditorToggle(Rect position, bool value, GUIContent content, GUIStyle style)
    {
        int hashCode = "AlignToggle".GetHashCode();
        int id = EditorGUIUtility.GetControlID(hashCode, EditorGUIUtility.native, position);
        Event evt = Event.current;
		
        // Toggle selected toggle on space or return key
        if (EditorGUIUtility.keyboardControl == id && evt.type == EventType.KeyDown && (evt.keyCode == KeyCode.Space || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter))
        {
            value = !value;
            evt.Use();
            GUI.changed = true;
        }
		
        if (evt.type == EventType.KeyDown && Event.current.button == 0 && position.Contains(Event.current.mousePosition))
        {
            GUIUtility.keyboardControl = id;
            EditorGUIUtility.editingTextField = false;
            HandleUtility.Repaint();
        }
		
        bool returnValue = GUI.Toggle(position, id, value, content, style);
		
        return returnValue;
    }
	
    // We can't go through serialzied properties here since we're showing two controls for a single SerializzedProperty.
    private static void SetHorizontalAlignment(SerializedProperty alignment, float horizontalAlignment)
    {
        foreach (var obj in alignment.serializedObject.targetObjects)
        {
            TEXDraw3D text = obj as TEXDraw3D;
            Undo.RecordObject(text, "Horizontal Alignment");
            text.alignment = new Vector2(horizontalAlignment, text.alignment.y);
            EditorUtility.SetDirty(obj);
        }
    }

    private static void SetVerticalAlignment(SerializedProperty alignment, float verticalAlignment)
    {
        foreach (var obj in alignment.serializedObject.targetObjects)
        {
            TEXDraw3D text = obj as TEXDraw3D;
            Undo.RecordObject(text, "Vertical Alignment");
            text.alignment = new Vector2(text.alignment.x, verticalAlignment);
            EditorUtility.SetDirty(obj);
        }
    }

	
	
    #endregion

    [MenuItem("GameObject/3D Object/TEXDraw 3D", false, 3300)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("TEXDraw 3D");
        go.AddComponent<TEXDraw3D>();
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

}

#endif