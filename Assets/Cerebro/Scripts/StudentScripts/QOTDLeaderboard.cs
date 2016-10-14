using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.IO;
using SimpleJSON;

namespace Cerebro {
	public class QOTDLeaderboard : MonoBehaviour {
		public GameObject progressCircle;
		public Text title;
		public Text MyScore;
		public GameObject list;
		private string curQuizDate;

		public void Initialise(string quizDate) {
			curQuizDate = quizDate;
			LaunchList.instance.LeaderboardLoaded += LeaderboardLoaded;
			LaunchList.instance.GetLeaderboardForDate (quizDate);
			System.DateTime date = System.DateTime.ParseExact (quizDate,"yyyyMMdd",null);
			title.text =  date.ToString ("MMM dd, yyyy");
			list.SetActive (false);
			MyScore.transform.parent.gameObject.SetActive (false);
		}

		public void LeaderboardLoaded(object sender, System.EventArgs e) {
			progressCircle.SetActive (false);
			list.SetActive (true);
			MyScore.transform.parent.gameObject.SetActive (true);

			string studentID = PlayerPrefs.GetString (PlayerPrefKeys.IDKey);

			int i = 0;
			bool foundIDBool = false;
			foreach(var entry in LaunchList.instance.mLeaderboard)
			{	
				if (i < 5) {
					list.transform.Find ("Option" + i).Find ("Name").GetComponent<Text> ().text = entry.StudentName;
					list.transform.Find ("Option" + i).Find ("Score").GetComponent<Text> ().text = ""+entry.StudentScore;
					i++;
				}

				if (studentID == entry.StudentID) {
					foundIDBool = true;
					MyScore.text = "" + entry.StudentScore;
				}
			}

			if (!foundIDBool) {
				if (LaunchList.instance.mUseJSON) {
					string fileName = Application.persistentDataPath + "/QuizSubmittedJSON.txt";
					if (File.Exists (fileName)) {
						string data = File.ReadAllText (fileName);
						if (!LaunchList.instance.IsJsonValidDirtyCheck (data)) {
							return;
						}
						JSONNode N = JSONClass.Parse (data);
						for (int j = 0; j < N ["Data"].Count; j++) {
							if (N ["Data"] [j] ["quizDate"].Value == curQuizDate) {
								MyScore.text = N ["Data"] [j] ["score"].Value;
							}
						}
					}
				} else {
					string fileName = Application.persistentDataPath + "/QuizSubmitted.txt";

					if (File.Exists (fileName)) {
						var sr = File.OpenText (fileName);
						var line = sr.ReadLine ();
						string[] splitArr;
						string date = curQuizDate;
						while (line != null) {
							splitArr = line.Split ("," [0]);
							if (date == splitArr [0]) {
								MyScore.text = splitArr [3];
								CerebroHelper.DebugLog ("here " + MyScore.text);
							}
							line = sr.ReadLine ();
						}  
						sr.Close ();
					}
				}
			}

			for (int j=i; j < 5; j++) {
				list.transform.Find ("Option" + j).gameObject.SetActive (false);
			}
		}
		public void BackPressed() {
			LaunchList.instance.LeaderboardLoaded -= LeaderboardLoaded;
			Destroy (gameObject);
		}
	}
}
