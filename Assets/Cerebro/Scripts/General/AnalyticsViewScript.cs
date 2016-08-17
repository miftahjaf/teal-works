using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Cerebro
{
	public class AnalyticsViewScript : MonoBehaviour
	{
		public Text QuestionsAttemptedTodayText;
		public Text AccuracyTodayText;
		public Text AverageAttemptsText;

		public GameObject ScaleValues;
		public GameObject AttemptsBars;
		public GameObject CorrectBars;
		public GameObject Dates;

		// Use this for initialization
		void Start ()
		{
			CerebroAnalytics.instance.ScreenOpen (CerebroScreens.Analytics);
			FillData ();
		}

		void FillData ()
		{
			System.DateTime todayDate = System.DateTime.Now;
			List<int> Attempts = new List<int> ();
			List<int> Correct = new List<int> ();
			List<string> dates = new List<string> ();
			int totalCorrect = 0;
			int totalAttempts = 0;
			int questionsAttemptedToday = 0;
			float AccuracyToday = 0f;
			float AverageAttempts = 0f;
			int maxValue = 0;
			for (var i = 0; i < 7; i++) {
				System.DateTime date = todayDate.AddDays (-1 * i);
				Dictionary<string,int> data = WelcomeScript.instance.GetPracticeCount (date.ToString ("yyyyMMdd"));
				dates.Add (date.ToString ("yyyyMMdd"));
				Correct.Add (data ["correct"]);
				Attempts.Add (data ["attempts"]);
				totalCorrect += data ["correct"];
				totalAttempts += data ["attempts"];
				if (maxValue < data ["attempts"]) {
					maxValue = data ["attempts"];
				}
				if (i == 0) {
					questionsAttemptedToday = data ["attempts"];
					AccuracyToday = Mathf.Round (((float)data ["correct"] / (float)data ["attempts"]) * 100f);
					if(data ["attempts"] == 0) {
						AccuracyToday = 0;
					}
				}
			}
			AverageAttempts = Mathf.Round ((float)totalAttempts / 7f);

			int maxScaleValue = Mathf.CeilToInt ((float)maxValue / 4f) * 4;
			if (maxScaleValue < 4) {
				maxScaleValue = 4;
			}

			QuestionsAttemptedTodayText.text = questionsAttemptedToday.ToString ();
			AccuracyTodayText.text = AccuracyToday.ToString () + " %";
			if (questionsAttemptedToday == 0) {
				AccuracyTodayText.text = "NA";
			}

			AverageAttemptsText.text = AverageAttempts.ToString ();

			float maxHeight = 384f;

			for (var i = 0; i < Dates.transform.childCount; i++) {
				Dates.transform.GetChild (i).GetComponent<Text> ().text = System.DateTime.ParseExact (dates [i], "yyyyMMdd", null).ToString ("dd MMM");

				Vector2 barSizeAttempts = AttemptsBars.transform.GetChild (i).GetComponent<RectTransform> ().sizeDelta;
				float heightAttempts = ((float)Attempts [i] / (float)maxScaleValue) * maxHeight;

				AttemptsBars.transform.GetChild (i).GetComponent<RectTransform> ().sizeDelta = new Vector2 (barSizeAttempts.x, heightAttempts);

				Vector2 barSizeCorrect = CorrectBars.transform.GetChild (i).GetComponent<RectTransform> ().sizeDelta;
				float heightCorrect = ((float)Correct [i] / (float)maxScaleValue) * maxHeight;
				CorrectBars.transform.GetChild (i).GetComponent<RectTransform> ().sizeDelta = new Vector2 (barSizeCorrect.x, heightCorrect);

				AttemptsBars.transform.GetChild (i).localScale = new Vector2 (1f, 0f);
				CorrectBars.transform.GetChild (i).localScale = new Vector2 (1f, 0f);
				Go.to (AttemptsBars.transform.GetChild (i), 0.3f, new GoTweenConfig ().scale (new Vector3 (1f, 1f, 1), false));
				Go.to (CorrectBars.transform.GetChild (i), 0.3f, new GoTweenConfig ().scale (new Vector3 (1f, 1f, 1), false));

			}

			for (var i = 0; i < ScaleValues.transform.childCount; i++) {
				ScaleValues.transform.GetChild (i).GetComponent<Text> ().text = (((float)maxScaleValue / 4f) * (i + 1)).ToString ();
			}
		}

		public void BackPressed() {
			WelcomeScript.instance.ShowScreen (false);
			Destroy (this.gameObject);
		}

	}
}
