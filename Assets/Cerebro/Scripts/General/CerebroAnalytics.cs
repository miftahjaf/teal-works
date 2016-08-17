using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
				LogFileEntry ("Screen", mCurrentScreen, System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss"), screenTime.ToString ());
				screenTime = 0f;
				mCurrentScreen = "";
			} else if (mCurrentScreen != "") {
				LogFileEntry ("Screen", mCurrentScreen, System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss"), screenTime.ToString ());
				screenTime = 0f;
				mCurrentScreen = "";
			}
		}

		public void SessionStarted ()
		{
			sessionStart = System.DateTime.Now.ToUniversalTime ();
			LogFileEntry ("Session", "Start", sessionStart.ToString ("yyyy-MM-ddTHH:mm:ss"));
		}

		public void SessionEnded ()
		{
			if (mCurrentScreen != "") {
				LogFileEntry ("Screen", mCurrentScreen, System.DateTime.Now.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ss"), screenTime.ToString ());
				screenTime = 0f;
				mCurrentScreen = "";
			}
			sessionEnd = System.DateTime.Now.ToUniversalTime ();
			System.TimeSpan differenceTime = sessionEnd.Subtract (sessionStart);
			float diff = (float)differenceTime.TotalSeconds;
			diff = Mathf.Floor (diff * 10.0f) / 10.0f;
			LogFileEntry ("Session", "End", sessionEnd.ToString ("yyyy-MM-ddTHH:mm:ss"), diff.ToString ());
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

		void Update ()
		{
			if (mCurrentScreen != "") {
				screenTime += Time.deltaTime;
			}
		}
	}
}
