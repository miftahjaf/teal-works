using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

namespace Cerebro
{
	public class CerebroScreens
	{
		public const string Welcome = "Welcome";
		public const string Practice = "Practice";
		public const string Watch = "Watch";
		public const string WritersCorner = "WritersCorner";
		public const string DailyQuiz = "DailyQuiz";
		public const string Revisit = "Revisit";
		public const string GOT = "GOT";
		public const string WordTower = "WordTower";
		public const string Verbalize = "Verbalize";
		public const string Coding = "Coding";
		public const string Feedback = "Feedback";
		public const string Profile = "Profile";
		public const string Analytics = "Analytics";
	}

	public class CerebroAnalytics : MonoBehaviour
	{
		private string mCurrentScreen = "";
		private float screenTime = 0f;

		private System.DateTime sessionStart;
		private System.DateTime sessionEnd;

		private static CerebroAnalytics m_Instance;

		public static CerebroAnalytics instance {
			get {
				return m_Instance;
			}
		}

		void Awake ()
		{
			m_Instance = this;
		}

		public void ScreenOpen (string name)
		{
			if (mCurrentScreen != "") {
				ScreenClosed (mCurrentScreen);
			}
			mCurrentScreen = name;
			screenTime = 0f;
		}

		public void ScreenClosed (string name = null)
		{
			if (name != null && mCurrentScreen != name) {
				ScreenClosed ();
			} else if (name != null && mCurrentScreen == name) {
				LogFileEntryJSON ("Screen", mCurrentScreen, System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss"), screenTime.ToString ());
				screenTime = 0f;
				mCurrentScreen = "";
			} else if (mCurrentScreen != "") {
				LogFileEntryJSON ("Screen", mCurrentScreen, System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss"), screenTime.ToString ());
				screenTime = 0f;
				mCurrentScreen = "";
			}
		}

		public void SessionStarted ()
		{
			sessionStart = System.DateTime.Now.ToUniversalTime ();
			LogFileEntryJSON ("Session", "Start", sessionStart.ToString ("yyyy-MM-ddTHH:mm:ss"));
		}

		public void SessionEnded ()
		{
			if (mCurrentScreen != "") {
				LogFileEntryJSON ("Screen", mCurrentScreen, System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss"), screenTime.ToString ());
				screenTime = 0f;
				mCurrentScreen = "";
			}
			sessionEnd = System.DateTime.Now.ToUniversalTime ();
			System.TimeSpan differenceTime = sessionEnd.Subtract (sessionStart);
			float diff = (float)differenceTime.TotalSeconds;
			diff = Mathf.Floor (diff * 10.0f) / 10.0f;
			LogFileEntryJSON ("Session", "End", sessionEnd.ToString ("yyyy-MM-ddTHH:mm:ss"), diff.ToString ());
		}

		void LogFileEntry (string type, string description, string timestamp, string timeSpent = "0")
		{

			string fileName = Application.persistentDataPath + "/UsageAnalytics.txt";

			StreamWriter sr = null;
			if (File.Exists (fileName)) {
				sr = File.AppendText (fileName);
			} else {
				sr = File.CreateText (fileName);
			}

			sr.WriteLine ("{0},{1},{2},{3}", type, description, timestamp, timeSpent);
			sr.Close ();
		}

		void LogFileEntryJSON (string type, string description, string timestamp, string timeSpent = "0")
		{
			if (!LaunchList.instance.mUseJSON) {
				LogFileEntry (type, description, timestamp, timeSpent);
				return;
			}

			string fileName = Application.persistentDataPath + "/UsageAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return;
			
			int cnt = 0;
			JSONNode N = JSONClass.Parse ("{\"Data\"}");
			if (File.Exists (fileName)) {				
				string data = File.ReadAllText (fileName);
				N = JSONClass.Parse (data);
				cnt = N ["Data"].Count;
				File.WriteAllText (fileName, string.Empty);
			}
			N ["Data"] [cnt] ["type"] = type;
			N ["Data"] [cnt] ["description"] = description;
			N ["Data"] [cnt] ["timestamp"] = timestamp;
			N ["Data"] [cnt] ["timeSpent"] = timeSpent;
			N ["VersionNumber"] = LaunchList.instance.VersionData;
			File.WriteAllText (fileName, N.ToString());
		}

		public void CleanLocalLogFile (List<Telemetry> list)
		{
			string fileName = Application.persistentDataPath + "/UsageAnalytics.txt";

			List<string> writeLines = new List<string> ();
			List<string> timestamps = new List<string> ();
			foreach (var item in list) {
				timestamps.Add (item.Timestamp);
			}
			if (File.Exists (fileName)) {
				var sreader = File.OpenText (fileName);
				var line = sreader.ReadLine ();
				while (line != null) {
					var lineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
					if (!timestamps.Contains (lineArr [2])) {
						writeLines.Add (line);
					}
					line = sreader.ReadLine ();
				}
				sreader.Close ();
			}

			StreamWriter sr = File.CreateText (fileName);
			foreach (var line in writeLines) {
				sr.WriteLine (line);
			}
			sr.Close ();
		}

		public void CleanLocalLogFileJSON (List<Telemetry> list)
		{
			if (!LaunchList.instance.mUseJSON) {
				CleanLocalLogFile (list);
				return;
			}

			string fileName = Application.persistentDataPath + "/UsageAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return;
			
			string data = File.ReadAllText (fileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				return;
			}
			JSONNode CurrLocal = JSONClass.Parse (data);
			JSONNode NextLocal = JSONClass.Parse ("{\"Data\"}");

			List<string> timestamps = new List<string> ();
			foreach (var item in list) {
				timestamps.Add (item.Timestamp);
			}

			int cnt = 0;
			for (int i = 0; i < CurrLocal ["Data"].Count; i++) {
				if (!timestamps.Contains(CurrLocal ["Data"] [cnt] ["timestamp"].Value)) {
					NextLocal ["Data"] [cnt] ["type"] = CurrLocal ["Data"] [cnt] ["type"].Value;
					NextLocal ["Data"] [cnt] ["description"] = CurrLocal ["Data"] [cnt] ["description"].Value;
					NextLocal ["Data"] [cnt] ["timestamp"] = CurrLocal ["Data"] [cnt] ["timestamp"].Value;
					NextLocal ["Data"] [cnt] ["timeSpent"] = CurrLocal ["Data"] [cnt] ["timeSpent"].Value;
					cnt++;
				}
			}
			NextLocal ["VersionNumber"] = CurrLocal ["VersionNumber"].Value;
			File.WriteAllText (fileName, NextLocal.ToString());
		}

		public List<Telemetry> GetNextLogsToSend ()
		{
			string fileName = Application.persistentDataPath + "/UsageAnalytics.txt";

			List<Telemetry> list = new List<Telemetry> ();
			if (File.Exists (fileName)) {
				var sreader = File.OpenText (fileName);
				var line = sreader.ReadLine ();
				while (line != null && list.Count < 20) {
					var lineArr = line.Split (new string[] { "," }, System.StringSplitOptions.None);
					Telemetry row = new Telemetry ();
					row.Type = lineArr [0];
					row.Description = lineArr [1];
					row.Timestamp = lineArr [2];
					row.TimeSpent = lineArr [3];
					list.Add (row);
					line = sreader.ReadLine ();
				}
				sreader.Close ();
			}
			return list;
		}

		public List<Telemetry> GetNextLogsToSendJSON ()
		{
			List<Telemetry> list = new List<Telemetry> ();

			if (!LaunchList.instance.mUseJSON) {
				list = GetNextLogsToSend ();
				return list;
			}

			string fileName = Application.persistentDataPath + "/UsageAnalyticsJSON.txt";
			if (!File.Exists (fileName))
				return null;
			
			string data = File.ReadAllText (fileName);
			if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
				return null;
			}
			JSONNode N = JSONClass.Parse (data);
			for (int i = 0; i < N ["Data"].Count; i++) {
				Telemetry row = new Telemetry ();
				row.Type = N ["Data"] [i] ["type"].Value;
				row.Description = N ["Data"] [i] ["description"].Value;
				row.Timestamp = N ["Data"] [i] ["timestamp"].Value;
				row.TimeSpent = N ["Data"] [i] ["timeSpent"].Value;
				list.Add (row);
			}
			return list;
		}

		void Update ()
		{
			if (mCurrentScreen != "") {
				screenTime += Time.deltaTime;
			}
		}
	}
}
