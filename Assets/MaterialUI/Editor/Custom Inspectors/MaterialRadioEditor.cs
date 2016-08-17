//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine.UI;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MaterialRadio))]
	class MaterialRadioEditor : MaterialBaseEditor
    {
        private MaterialRadio m_MaterialRadio;

        private SerializedProperty m_Graphic;
        private SerializedProperty m_iconData;
        private SerializedProperty m_LabelText;
        private SerializedProperty m_GraphicChangesWithToggleState;
        private SerializedProperty m_ToggleOnLabel;
        private SerializedProperty m_ToggleOffLabel;
        private SerializedProperty m_ToggleOnIcon;
		private SerializedProperty m_ToggleOffIcon;

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
		}

		void OnDisable()
		{
			OnBaseDisable();
		}

        public override void OnInspectorGUI()
        {
            m_MaterialRadio = (MaterialRadio)target;
            serializedObject.Update();

            bool isControlledByParent = m_MaterialRadio.GetComponentInParent<MaterialRadioGroup>() && m_MaterialRadio.GetComponentInParent<MaterialRadioGroup>().isControllingChildren;
            if (isControlledByParent)
            {
                EditorGUILayout.HelpBox("Some options are controlled by parent MaterialRadioGroup", MessageType.Warning);
            }

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

            EditorGUI.BeginDisabledGroup(isControlledByParent);
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_AnimationDuration"), true);

				DrawFoldoutColors(ColorsSection);
            }
            EditorGUI.EndDisabledGroup();

			DrawFoldoutComponents(ComponentsSection);

            serializedObject.ApplyModifiedProperties();
		}

		private void ColorsSection()
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnColor"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OffColor"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_DisabledColor"), true);

			if (serializedObject.FindProperty("m_Graphic").objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ChangeGraphicColor"), true);

				if (serializedObject.FindProperty("m_ChangeGraphicColor").boolValue)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GraphicOnColor"), true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_GraphicOffColor"), true);
				}
			}

			if (m_MaterialRadio.GetComponent<MaterialRipple>())
			{
				EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ChangeRippleColor"), true);
				if (serializedObject.FindProperty("m_ChangeRippleColor").boolValue)
				{
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RippleOnColor"), true);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RippleOffColor"), true);
				}
			}
			EditorGUI.indentLevel--;
		}

		private void ComponentsSection()
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Graphic"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_DotImage"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RingImage"), true);
			EditorGUI.indentLevel--;
		}
    }
}