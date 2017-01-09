using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cerebro
{
	public class CerebroHelper : MonoBehaviour
	{
		private static List<string> testIds = new List<string>(){"9000002","9000003","9000004", "301"};
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
			if (HTTPRequestHelper.instance != null) {
				return HTTPRequestHelper.instance.IsStagingEnable ();
			} else {
				return false;
			}
//			if (PlayerPrefs.HasKey (PlayerPrefKeys.IDKey)) {
//				var id = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);
//				if (testIds.Contains (id)) {
//					return true;
//				}
//			}
//			return false;
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

		public static List<string> GetRandomColorValues(int number)
		{
			/*List<string> colors = new List<string>() {
				"F1522A",
				"FEC841",
				"0A356A",
				"DE377C",
				"B29A76",
				"B6C894",
				"EEC6E0",
				"827773",
				"9772A5",
				"7ECCBE"
			};*/
			List<string> colors = new List<string>() {
				"EF9A9A",
				"B39DDB",
				"81D4FA",
				"00838F",
				"1DE9B6",
				"C5E1A5",
				"FFE082",
				"FFAB91",
				"BCAAA4",
				"90A4AE",
			};



			colors.Shuffle ();

			return colors.GetRange (0, number);;
		}


	}
}
