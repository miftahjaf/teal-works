#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using TexDrawLib;
using System;

public class EditorObjectPool : EditorWindow
{
    [MenuItem("Window/Editor Pool Check")]
    public static void ShowWindow()
    {
        var w = ScriptableObject.CreateInstance<EditorObjectPool>();
        w.Show();
    }

    public static List<IObjectPool> l = new List<IObjectPool>();
	Vector2 scr;
	
	static string helpInfo = 
		"This is a tool for previewing how many times a class that implements TexDrawLib.IFlushable be instantianted, and preserved in memory." +
		"\nYou can also implement IFlushable and use TEXDrawLib.ObjPool as part of optimizing your in-game performance." +
		"\nIt's very recommended to implement this into non-serializable classes. See TEXDraw/Internal/ListPool.cs for the example of implementation";
	

    void OnGUI()
    {
	    EditorGUILayout.HelpBox(helpInfo, MessageType.Info);
        scr = EditorGUILayout.BeginScrollView(scr, GUILayout.ExpandHeight(true));
        for (int i = 0; i < l.Count; i++) {
            EditorGUILayout.LabelField(GetTypeName(l[i].GetType().GetGenericArguments()[0]), string.Format("Total {0} Idle {1}", l[i].countAll, l[i].countInactive), EditorStyles.helpBox);
        }
        EditorGUILayout.EndScrollView();
    }

    string GetTypeName(Type t)
    {
        string n = t.Name;
        var arg = t.GetGenericArguments();
        if (arg.Length > 0)
            n = string.Format("{0}<{1}>", n.Substring(0, n.Length - 2), GetTypeName(arg[0]));
        return n;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
#endif