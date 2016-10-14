using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class PracticeData : MonoBehaviour {

	public class PData {
		public int totalCorrect;
		public int totalAttempts;
	}

	public static Dictionary<string,PracticeData.PData> mPracticeData;

	public static void UpdateLocalFile(string itemID, bool isCorrect) {
		string fileName = Application.persistentDataPath + "/PracticeData.txt";

		if (mPracticeData.ContainsKey (itemID)) {
			mPracticeData [itemID].totalAttempts += 1;
			if (isCorrect) {
				mPracticeData [itemID].totalCorrect += 1;
			}
		} else {
			var pData = new PData ();
			pData.totalAttempts = 1;
			if (isCorrect) {
				pData.totalCorrect = 1;
			}
			mPracticeData.Add (itemID,pData);
		}

		List<string> writeLines = new List<string> ();
		foreach (var id in mPracticeData) {
			writeLines.Add (id.Key + "," + id.Value.totalAttempts + "," + id.Value.totalCorrect);
		}

		StreamWriter writesr = File.CreateText (fileName);
		for (var i = 0; i < writeLines.Count; i++) {
			writesr.WriteLine (writeLines [i]);
		}
		writesr.Close ();
	}

	public static void UpdateLocalFileJSON(string itemID, bool isCorrect) {
		string fileName = Application.persistentDataPath + "/PracticeDataJSON.txt";
		JSONNode N = JSONClass.Parse ("{\"Data\"}");
		int cnt = 0;
		if (File.Exists (fileName)) {				
			string data = File.ReadAllText (fileName);
			N = JSONClass.Parse (data);
			cnt = N ["Data"].Count;
			File.WriteAllText (fileName, string.Empty);
		}

		if (mPracticeData.ContainsKey (itemID)) {
			mPracticeData [itemID].totalAttempts += 1;
			if (isCorrect) {
				mPracticeData [itemID].totalCorrect += 1;
			}
		} else {
			var pData = new PData ();
			pData.totalAttempts = 1;
			if (isCorrect) {
				pData.totalCorrect = 1;
			}
			mPracticeData.Add (itemID,pData);
		}

		bool found = false;
		for (int i = 0; i < N ["Data"].Count; i++) {
			if (N ["Data"] [i] ["practiceId"].Value == itemID) {
				N ["Data"] [i] ["attempts"] = (N ["Data"] [i] ["attempts"].AsInt + 1).ToString();
				if (isCorrect) {
					N ["Data"] [i] ["correct"] = (N ["Data"] [i] ["correct"].AsInt + 1).ToString ();
				}
				found = true;
			}
		}
		if (!found) {
			N ["Data"] [cnt] ["practiceId"] = itemID;
			N ["Data"] [cnt] ["attempts"] = "1";
			if (isCorrect) {
				N ["Data"] [cnt] ["correct"] = "1";
			} else {
				N ["Data"] [cnt] ["correct"] = "0";
			}
		}
		File.WriteAllText (fileName, N.ToString());	
	}

	public static void PopulateFromFile() {
		string fileName = Application.persistentDataPath + "/PracticeData.txt";
		mPracticeData = new Dictionary<string,PracticeData.PData> ();
		if (File.Exists (fileName)) {
			var sr = File.OpenText (fileName);
			var line = sr.ReadLine ();
			string[] splitArr;
			while (line != null) {
				splitArr = line.Split ("," [0]);
				var pData = new PData ();
				pData.totalAttempts = int.Parse (splitArr [1]);
				pData.totalCorrect = int.Parse (splitArr [2]);
				mPracticeData.Add (splitArr[0],pData);

				line = sr.ReadLine ();
			}  
			sr.Close ();
		}
	}

	public static void PopulateFromFileJSON()
	{
		string fileName = Application.persistentDataPath + "/PracticeDataJSON.txt";
		mPracticeData = new Dictionary<string,PracticeData.PData> ();
		if (File.Exists (fileName))
		{
			string data = File.ReadAllText (fileName);
			JSONNode N = JSONClass.Parse (data);
			for (int i = 0; i < N ["Data"].Count; i++) {
				var pData = new PData ();
				pData.totalAttempts = N ["Data"] [i] ["attempts"].AsInt;
				pData.totalCorrect = N ["Data"] [i] ["correct"].AsInt;
				mPracticeData.Add (N ["Data"] [i] ["practiceId"].Value,pData);
			}
		}
	}
}
