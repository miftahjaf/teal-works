using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cerebro
{
	public class CerebroHelper : MonoBehaviour
	{
		private static List<string> testIds = new List<string>(){"314","315","316"};
		public static Dictionary<string,Texture2D> remoteWatchTextures = new Dictionary<string,Texture2D>();
		public static Dictionary<string,Texture2D> remoteQuizTextures = new Dictionary<string,Texture2D>();
		//public static string 
		public static Color HexToRGB (string hex)
		{
			int bigint = int.Parse (hex, System.Globalization.NumberStyles.HexNumber);
			float r = (bigint >> 16) & 255;
			float g = (bigint >> 8) & 255;
			float b = bigint & 255;
			Color color = new Color (r / 255f, g / 255f, b / 255f, 1f);
			return color;
		}

		public static bool isTestUser ()
		{
			if (PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
				var id = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
				if (testIds.Contains (id)) {
					return true;
				}
			}
			return false;
		}

		public static void DebugLog(object log)
		{
			#if UNITY_EDITOR
			Debug.Log(log);
			#endif

			if (log == null || log.ToString ().Length > 500) {
				return;
			}
			if (ConsoleScreenScript.instance.log.text.Length > 5000) {
				ConsoleScreenScript.instance.log.text = ConsoleScreenScript.instance.log.text.Remove (5000);
			}
			string addString = log.ToString () + "\n" + "----------------------" + "\n";
			ConsoleScreenScript.instance.log.text = addString + ConsoleScreenScript.instance.log.text;
		}

		public static void ShowConsoleScreen()
		{
			ConsoleScreenScript.instance.ShowScreen();
		}
	}
}
