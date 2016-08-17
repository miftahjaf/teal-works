//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialRipple))]
    class MaterialRippleEditor : Editor
    {
        private SerializedProperty m_RippleParent;
        private SerializedProperty m_AutoSize;
        private SerializedProperty m_ManualSize;
        private SerializedProperty m_SizePercent;
        private SerializedProperty m_SizeMode;
        private SerializedProperty m_Speed;
        private SerializedProperty m_Color;
        private SerializedProperty m_StartAlpha;
        private SerializedProperty m_EndAlpha;
        private SerializedProperty m_MoveTowardCenter;
        private SerializedProperty m_ToggleMask;
        private SerializedProperty m_HighlightWhen;
        private SerializedProperty m_HighlightGraphic;
        private SerializedProperty m_AutoHighlightColor;
        private SerializedProperty m_HighlightColor;
        private SerializedProperty m_CheckForScroll;
        private SerializedProperty m_PlaceRippleBehind;

        void OnEnable()
        {
            m_RippleParent = serializedObject.FindProperty("m_RippleData.RippleParent");
            m_AutoSize = serializedObject.FindProperty("m_RippleData.AutoSize");
            m_ManualSize = serializedObject.FindProperty("m_RippleData.ManualSize");
            m_SizePercent = serializedObject.FindProperty("m_RippleData.SizePercent");
            m_SizeMode = serializedObject.FindProperty("m_RippleData.SizeMode");
            m_Speed = serializedObject.FindProperty("m_RippleData.Speed");
            m_Color = serializedObject.FindProperty("m_RippleData.Color");
            m_StartAlpha = serializedObject.FindProperty("m_RippleData.StartAlpha");
            m_EndAlpha = serializedObject.FindProperty("m_RippleData.EndAlpha");
            m_MoveTowardCenter = serializedObject.FindProperty("m_RippleData.MoveTowardCenter");
            m_ToggleMask = serializedObject.FindProperty("m_ToggleMask");
            m_HighlightWhen = serializedObject.FindProperty("m_HighlightWhen");
            m_HighlightGraphic = serializedObject.FindProperty("m_HighlightGraphic");
            m_AutoHighlightColor = serializedObject.FindProperty("m_AutoHighlightColor");
            m_HighlightColor = serializedObject.FindProperty("m_HighlightColor");
            m_CheckForScroll = serializedObject.FindProperty("m_CheckForScroll");
            m_PlaceRippleBehind = serializedObject.FindProperty("m_RippleData.PlaceRippleBehind");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_RippleParent, true);
            EditorGUILayout.PropertyField(m_AutoSize, true);

            if (!m_AutoSize.boolValue)
            {
                EditorGUILayout.PropertyField(m_ManualSize, true);
            }
            else
            {
                EditorGUILayout.PropertyField(m_SizePercent, true);
                EditorGUILayout.PropertyField(m_SizeMode, true);
            }

            EditorGUILayout.PropertyField(m_Speed, true);
            EditorGUILayout.PropertyField(m_Color, true);
            EditorGUILayout.PropertyField(m_StartAlpha, true);
            EditorGUILayout.PropertyField(m_EndAlpha, true);
            EditorGUILayout.PropertyField(m_MoveTowardCenter, true);
            EditorGUILayout.PropertyField(m_ToggleMask, true);

            EditorGUILayout.PropertyField(m_HighlightWhen, true);

            if (m_HighlightWhen.intValue > 0)
            {
                EditorGUILayout.PropertyField(m_HighlightGraphic, true);
                EditorGUILayout.PropertyField(m_AutoHighlightColor, true);

                if (m_AutoHighlightColor.boolValue == false)
                {
                    EditorGUILayout.PropertyField(m_HighlightColor, true);
                }
            }

            EditorGUILayout.PropertyField(m_CheckForScroll, true);
            EditorGUILayout.PropertyField(m_PlaceRippleBehind, true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}