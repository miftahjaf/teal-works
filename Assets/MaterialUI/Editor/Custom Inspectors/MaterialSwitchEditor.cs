//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine.UI;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialSwitch))]
	class MaterialSwitchEditor : MaterialBaseEditor
    {
        private MaterialSwitch m_MaterialSwitch;

        private SerializedProperty m_Graphic;
        private SerializedProperty m_iconData;
        private SerializedProperty m_LabelText;
        private SerializedProperty m_GraphicChangesWithToggleState;
        private SerializedProperty m_ToggleOnLabel;
        private SerializedProperty m_ToggleOffLabel;
        private SerializedProperty m_ToggleOnIcon;
        private SerializedProperty m_ToggleOffIcon;

        private SerializedProperty m_AnimationDuration;
        private SerializedProperty m_OnColor;
        private SerializedProperty m_OffColor;
        private SerializedProperty m_DisabledColor;
        private SerializedProperty m_BackOnColor;
        private SerializedProperty m_BackOffColor;
        private SerializedProperty m_BackDisabledColor;
        private SerializedProperty m_ChangeGraphicColor;
        private SerializedProperty m_GraphicOnColor;
        private SerializedProperty m_GraphicOffColor;
        private SerializedProperty m_GraphicDisabledColor;
        private SerializedProperty m_ChangeRippleColor;
        private SerializedProperty m_RippleOnColor;
        private SerializedProperty m_RippleOffColor;
        private SerializedProperty m_SwitchImage;
        private SerializedProperty m_BackImage;
		private SerializedProperty m_ShadowImage;

        void OnEnable()
        {
			OnBaseEnable();

            m_Graphic = serializedObject.FindProperty("m_Graphic");
            m_iconData = serializedObject.FindProperty("m_Icon");
            m_LabelText = serializedObject.FindProperty("m_Label");
            m_GraphicChangesWithToggleState = serializedObject.FindProperty("m_ToggleGraphic");
            m_ToggleOnLabel = serializedObject.FindProperty("m_ToggleOnLabel");
            m_ToggleOffLabel = serializedObject.FindProperty("m_ToggleOffLabel");
            m_ToggleOnIcon = serializedObject.FindProperty("m_ToggleOnIcon");
            m_ToggleOffIcon = serializedObject.FindProperty("m_ToggleOffIcon");

            m_AnimationDuration = serializedObject.FindProperty("m_AnimationDuration");
            m_OnColor = serializedObject.FindProperty("m_OnColor");
            m_OffColor = serializedObject.FindProperty("m_OffColor");
            m_DisabledColor = serializedObject.FindProperty("m_DisabledColor");
            m_BackOnColor = serializedObject.FindProperty("m_BackOnColor");
            m_BackOffColor = serializedObject.FindProperty("m_BackOffColor");
            m_BackDisabledColor = serializedObject.FindProperty("m_BackDisabledColor");
            m_ChangeGraphicColor = serializedObject.FindProperty("m_ChangeGraphicColor");
            m_GraphicOnColor = serializedObject.FindProperty("m_GraphicOnColor");
            m_GraphicOffColor = serializedObject.FindProperty("m_GraphicOffColor");
            m_GraphicDisabledColor = serializedObject.FindProperty("m_GraphicDisabledColor");
            m_ChangeRippleColor = serializedObject.FindProperty("m_ChangeRippleColor");
            m_RippleOnColor = serializedObject.FindProperty("m_RippleOnColor");
            m_RippleOffColor = serializedObject.FindProperty("m_RippleOffColor");
            m_SwitchImage = serializedObject.FindProperty("m_SwitchImage");
            m_BackImage = serializedObject.FindProperty("m_BackImage");
			m_ShadowImage = serializedObject.FindProperty("m_ShadowImage");
		}

		void OnDisable()
		{
			OnBaseDisable();
		}

        public override void OnInspectorGUI()
        {
            m_MaterialSwitch = (MaterialSwitch)target;
            serializedObject.Update();

            if (m_Graphic.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(m_GraphicChangesWithToggleState);

                if (m_Graphic.objectReferenceValue.GetType() == typeof(Image) || m_Graphic.objectReferenceValue.GetType() == typeof(VectorImage))
                {
                    if (m_GraphicChangesWithToggleState.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_ToggleOnIcon);
                        EditorGUILayout.PropertyField(m_ToggleOffIcon);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(m_iconData);
                    }
                }
                else
                {
                    if (m_GraphicChangesWithToggleState.boolValue)
                    {
                        EditorGUILayout.PropertyField(m_ToggleOnLabel);
                        EditorGUILayout.PropertyField(m_ToggleOffLabel);
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(m_LabelText);
                    }
                }
            }

            EditorGUILayout.PropertyField(m_AnimationDuration);

			DrawFoldoutColors(ColorsSection);
			DrawFoldoutComponents(ComponentsSection);

            serializedObject.ApplyModifiedProperties();
		}

		private void ColorsSection()
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(m_OnColor);
			EditorGUILayout.PropertyField(m_OffColor);
			EditorGUILayout.PropertyField(m_DisabledColor);
			EditorGUILayout.PropertyField(m_BackOnColor);
			EditorGUILayout.PropertyField(m_BackOffColor);
			EditorGUILayout.PropertyField(m_BackDisabledColor);

			if (m_Graphic.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(m_ChangeGraphicColor);

				if (m_ChangeGraphicColor.boolValue)
				{
					EditorGUILayout.PropertyField(m_GraphicOnColor);
					EditorGUILayout.PropertyField(m_GraphicOffColor);
					EditorGUILayout.PropertyField(m_GraphicDisabledColor);
				}
			}

			if (m_MaterialSwitch.GetComponent<MaterialRipple>())
			{
				EditorGUILayout.PropertyField(m_ChangeRippleColor);
				if (m_ChangeRippleColor.boolValue)
				{
					EditorGUILayout.PropertyField(m_RippleOnColor);
					EditorGUILayout.PropertyField(m_RippleOffColor);
				}
			}
			EditorGUI.indentLevel--;
		}

		private void ComponentsSection()
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(m_Graphic);
			EditorGUILayout.PropertyField(m_SwitchImage);
			EditorGUILayout.PropertyField(m_BackImage);
			EditorGUILayout.PropertyField(m_ShadowImage);
			EditorGUI.indentLevel--;
		}
    }
}