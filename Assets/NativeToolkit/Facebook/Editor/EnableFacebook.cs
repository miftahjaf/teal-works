using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[InitializeOnLoad]
public class EnableFacebook {
	
	static EnableFacebook () 
	{
		foreach(Assembly a in AppDomain.CurrentDomain.GetAssemblies())
		{
			string lib = a.FullName.Split('.')[0].Split(',')[0];
			
			if(lib == "IFacebook" || lib == "FB")
			{
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "facebook");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "facebook");
				return;
			}
		}
	}

	[MenuItem("Tools/Download Facebook SDK")]
	static void DownloadFacebookSDK()
	{
		Application.OpenURL("https://developers.facebook.com/docs/unity/downloads");
	}
}