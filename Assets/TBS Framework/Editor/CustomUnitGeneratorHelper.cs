using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cerebro.CustomUnitGenerator))]
public class CustomUnitGeneratorHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

		Cerebro.CustomUnitGenerator unitGenerator = (Cerebro.CustomUnitGenerator)target;

        if(GUILayout.Button("Snap to Grid"))
        {
            unitGenerator.SnapToGrid();
        }
    }
}
