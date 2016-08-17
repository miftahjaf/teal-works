//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace MaterialUI
{
    [CustomEditor(typeof(MaterialButton), true)]
    [CanEditMultipleObjects]
    public class MaterialButtonEditor : MaterialBaseEditor
    {
        private MaterialButton m_MaterialButton;

        private SerializedProperty m_ShadowsCanvasGroup;
        private SerializedProperty m_ContentRectTransform;
        private SerializedProperty m_ImageObject;
        private SerializedProperty m_TextObject;
        private SerializedProperty m_Icon;

        private SerializedProperty m_Text;
        private SerializedProperty m_TextColor;
        private SerializedProperty m_IconColor;
        private SerializedProperty m_IconData;
        private SerializedProperty m_ButtonColor;

        private SerializedProperty m_ContentPaddingX;
        private SerializedProperty m_ContentPaddingY;
        private SerializedProperty m_ButtonPadding;

        private SerializedProperty m_FitWidthToContent;
        private SerializedProperty m_FitHeightToContent;

        private AnimBool m_TextObjectAnimBool;
        private AnimBool m_IconObjectAnimBool;

        bool m_ColorChanged;
        bool m_TextChanged;
        bool m_TextColorChanged;
        bool m_IconColorChanged;

        void OnEnable()
        {
            OnBaseEnable();

            m_MaterialButton = (MaterialButton)target;

            m_ShadowsCanvasGroup = serializedObject.FindProperty("m_ShadowsCanvasGroup");
            m_ContentRectTransform = serializedObject.FindProperty("m_ContentRectTransform");

            m_ImageObject = serializedObject.FindProperty("m_ImageObject");
            m_TextObject = serializedObject.FindProperty("m_TextObject");
            m_Icon = serializedObject.FindProperty("m_Icon");

            m_Text = serializedObject.FindProperty("m_Text");
            m_TextColor = serializedObject.FindProperty("m_TextColor");
            m_IconColor = serializedObject.FindProperty("m_IconColor");
            m_IconData = serializedObject.FindProperty("m_IconData");
            m_ButtonColor = serializedObject.FindProperty("m_ButtonColor");

            m_ContentPaddingX = serializedObject.FindProperty("m_ContentPadding.x");
            m_ContentPaddingY = serializedObject.FindProperty("m_ContentPadding.y");

            m_FitWidthToContent = serializedObject.FindProperty("m_FitWidthToContent");
            m_FitHeightToContent = serializedObject.FindProperty("m_FitHeightToContent");

            m_TextObjectAnimBool = new AnimBool { value = m_TextObject.objectReferenceValue != null };
            m_TextObjectAnimBool.valueChanged.AddListener(Repaint);

            m_IconObjectAnimBool = new AnimBool { value = m_Icon.objectReferenceValue != null };
            m_IconObjectAnimBool.valueChanged.AddListener(Repaint);
        }

        void OnDisable()
        {
            OnBaseDisable();

            m_TextObjectAnimBool.valueChanged.RemoveListener(Repaint);
            m_IconObjectAnimBool.valueChanged.RemoveListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            m_ColorChanged = false;
            m_TextChanged = false;
            m_TextColorChanged = false;
            m_IconColorChanged = false;

            serializedObject.Update();

            ButtonSettingsSection();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_FitWidthToContent);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialButton.ClearTracker();
            }
            if (m_FitWidthToContent.boolValue)
            {
                EditorGUILayout.PropertyField(m_ContentPaddingX, new GUIContent());
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_FitHeightToContent);
            if (EditorGUI.EndChangeCheck())
            {
                m_MaterialButton.ClearTracker();
            }
            if (m_FitHeightToContent.boolValue)
            {
                EditorGUILayout.PropertyField(m_ContentPaddingY, new GUIContent());
            }
            EditorGUILayout.EndHorizontal();

            DrawFoldoutComponents(ComponentsSection);

            GUIContent convertText = new GUIContent();

            if (m_ShadowsCanvasGroup.objectReferenceValue != null)
            {
                convertText.text = "Convert to flat button";
            }
            else
            {
                convertText.text = "Convert to raised button";
            }

            if (Selection.objects.Length > 1)
            {
                GUI.enabled = false;
                convertText.text = "Convert button";
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(EditorGUIUtility.labelWidth);
                if (GUILayout.Button(convertText, EditorStyles.miniButton))
                {
                    m_MaterialButton.Convert();
                }
            }
            EditorGUILayout.EndHorizontal();

            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();

            if (m_ColorChanged)
            {
                m_MaterialButton.ChangeColor();
            }
            if (m_TextChanged)
            {
                m_MaterialButton.ChangeText();
            }
            if (m_TextColorChanged)
            {
                m_MaterialButton.ChangeTextColor();
            }
            if (m_IconColorChanged)
            {
                m_MaterialButton.ChangeIconColor();
            }
        }

        private void ButtonSettingsSection()
        {
            m_TextObjectAnimBool.target = m_TextObject.objectReferenceValue != null;
            m_IconObjectAnimBool.target = m_Icon.objectReferenceValue != null;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_ButtonColor);
            if (EditorGUI.EndChangeCheck())
            {
                m_ColorChanged = true;
            }

            if (EditorGUILayout.BeginFadeGroup(m_TextObjectAnimBool.faded))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Text);
                if (EditorGUI.EndChangeCheck())
                {
                    m_TextChanged = true;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_TextColor);
                if (EditorGUI.EndChangeCheck())
                {
                    m_TextColorChanged = true;
                }
            }
            EditorGUILayout.EndFadeGroup();

            if (EditorGUILayout.BeginFadeGroup(m_IconObjectAnimBool.faded))
            {
                EditorGUILayout.PropertyField(m_IconData);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_IconColor);
                if (EditorGUI.EndChangeCheck())
                {
                    m_IconColorChanged = true;
                }
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_ContentRectTransform);
            EditorGUILayout.PropertyField(m_ImageObject);
            EditorGUILayout.PropertyField(m_ShadowsCanvasGroup);
            EditorGUILayout.PropertyField(m_TextObject);
            EditorGUILayout.PropertyField(m_Icon);
            EditorGUI.indentLevel--;
        }
    }
}