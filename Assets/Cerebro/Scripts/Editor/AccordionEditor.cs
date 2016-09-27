using UnityEngine;
using UnityEditor;

namespace UnityEngine.UI.Extensions
{
	[CustomEditor(typeof(AccordionElement))]
	public class AccordionEditor : Editor
	{


		public override void OnInspectorGUI()
		{

			EditorGUI.BeginChangeCheck ();
			DrawDefaultInspector ();
		}


	}
}