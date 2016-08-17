//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEditor.AnimatedValues;
using System;

namespace MaterialUI
{
	public class MaterialBaseEditor : Editor
    {
		private bool m_ShowColors;
		private bool m_ShowComponents;

		private string m_ColorsPrefKey;
		private string m_ComponentsPrefKey;

		private AnimBool m_ShowComponentsAnimBool;
		private AnimBool m_ShowColorsAnimBool;

		protected void OnBaseEnable()
		{
			string prefKey = this.GetType().Name;

			m_ColorsPrefKey = prefKey + "_Show_Colors";
			m_ComponentsPrefKey = prefKey + "_Show_Components";

			m_ShowColors = EditorPrefs.GetBool(m_ColorsPrefKey, true);
			m_ShowComponents = EditorPrefs.GetBool(m_ComponentsPrefKey, false);

			m_ShowColorsAnimBool = new AnimBool { value = m_ShowColors };
			m_ShowColorsAnimBool.valueChanged.AddListener(Repaint);
			m_ShowComponentsAnimBool = new AnimBool { value = m_ShowComponents };
			m_ShowComponentsAnimBool.valueChanged.AddListener(Repaint);
		}

		protected void OnBaseDisable()
		{
			m_ShowComponentsAnimBool.valueChanged.RemoveListener(Repaint);
			m_ShowColorsAnimBool.valueChanged.RemoveListener(Repaint);
		}

		protected void DrawFoldoutColors(Action drawSection)
		{
			EditorGUI.BeginChangeCheck();
			m_ShowColors = EditorGUILayout.Foldout(m_ShowColors, "Colors");
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool(m_ColorsPrefKey, m_ShowColors);
			}

			m_ShowColorsAnimBool.target = m_ShowColors;

			if (EditorGUILayout.BeginFadeGroup(m_ShowColorsAnimBool.faded))
			{
				drawSection();
			}
			EditorGUILayout.EndFadeGroup();
		}

		protected void DrawFoldoutComponents(Action drawSection)
		{
			EditorGUI.BeginChangeCheck();
			m_ShowComponents = EditorGUILayout.Foldout(m_ShowComponents, "Components");
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool(m_ComponentsPrefKey, m_ShowComponents);
			}

			m_ShowComponentsAnimBool.target = m_ShowComponents;

			if (EditorGUILayout.BeginFadeGroup(m_ShowComponentsAnimBool.faded))
			{
				drawSection();
			}
			EditorGUILayout.EndFadeGroup();
		}
    }
}