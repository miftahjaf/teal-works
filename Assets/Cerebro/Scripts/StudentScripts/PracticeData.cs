using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

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
}
