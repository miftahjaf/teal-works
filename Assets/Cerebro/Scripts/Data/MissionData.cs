using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;


namespace Cerebro
{
	[System.Serializable]
	public class MissionData : Data<Mission>
	{
		public MissionData() : base("MissionJSON")
		{
		}

		public void CheckAndUpdateMissionData(string practiceItemID,int difficulty,int sublevel,bool isCorrect,int seed,string answer)
		{
			Debug.Log ("Practice Id "+ practiceItemID+" difficulty" + difficulty +" sublevel" + sublevel);
			int count = dataList.Count;
			for (int i = 0; i < count ; i++) {
				if (!dataList [i].IsMissionCompleted ()) {
					if (dataList [i].questions.Exists (x => x.practiceItemID == practiceItemID && x.difficulty == difficulty && x.subLevel == sublevel)) {
						dataList [i].answers.Add (new MissionAnswer (practiceItemID, seed, answer, isCorrect,difficulty,sublevel));
						//dataList [i].UpdateQuestion (practiceItemID, difficulty, sublevel);
					}
				}
			}
			SaveData ();
		}

		public int GetNotCompletedMissionCount()
		{
			return dataList.FindAll (x => x.endTime == "").Count;
		}


	}

	[System.Serializable]
	public class Mission
	{
		public string KCID;
		public string completionCondition;
		public string missionText;
		public int completionQuestionsLimit;
		public int completionQuestionsCorrectLimit;
		public List<MissionQuestion> questions;
		public List<MissionAnswer> answers;
		public string startTime;
		public string endTime;
		public string missionJson;
		public Mission()
		{
			KCID = "";
			completionCondition = "";
			missionText = "";
			completionQuestionsLimit = 0;
			completionQuestionsCorrectLimit = 0;
			questions = new List<MissionQuestion> ();
			answers = new List<MissionAnswer> ();
			startTime = "";
			endTime = "";
			missionJson = "";
		}

		public Mission GetMission(string json)
		{
			if (!json.IsValidJSON ()) {
				return this;
			}
			return GetMission (JSONNode.Parse (json));
		}

		public Mission GetMission(JSONNode jsonNode)
		{
			if (jsonNode == null) {
				return this;
			}
			missionJson = jsonNode.ToString ();
			KCID = jsonNode["kc_id"].Value;
			completionCondition = jsonNode["completion_condition"].Value;
			completionQuestionsCorrectLimit = jsonNode["completion_questions_correct_limit"].AsInt;
			completionQuestionsLimit = jsonNode["completion_questions_limit"].AsInt;
			startTime = jsonNode["startTime"].Value;
			endTime = jsonNode["endTime"].Value;
			missionText = jsonNode["mission_text"].Value;
			questions = new List<MissionQuestion> ();
			answers = new List<MissionAnswer> ();
			int questionsLength = jsonNode ["question_types"].Count;
			for (int i = 0; i < questionsLength; i++) {
				questions.Add(new MissionQuestion ().GetMissionQuestion(jsonNode ["question_types"][i]));
			}
			startTime = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");
			return this;
		}

		public bool IsMissionCompleted()
		{
			if (!string.IsNullOrEmpty (endTime)) {
				return true;
			}
			bool isMissionCompleted = false;
			switch (completionCondition) 
			{
			case "Attempt":
			case "Correct":
				isMissionCompleted = (answers.Count == completionQuestionsLimit);
				break;

			case "Streak":
				int correctInRow = 0;
				for (int i = 0; i < answers.Count; i++) 
				{
					if (answers [i].correct) {
						correctInRow++;
					} else {
						correctInRow = 0;
					}
				}
				isMissionCompleted = (answers.Count == completionQuestionsLimit || correctInRow == completionQuestionsCorrectLimit);
				break;
			}

			if (isMissionCompleted) {
				endTime = System.DateTime.Now.ToUniversalTime().ToString ("yyyy-MM-ddTHH:mm:ss");
			}
			return isMissionCompleted;
		}

		public bool IsSucceed()
		{
			bool isSucceed =false;
			switch (completionCondition) 
			{
				case "Attempt":
					isSucceed = (answers.Count == completionQuestionsLimit);
				break;

				case "Streak":
					int correctInRow = 0;
					for (int i = 0; i < answers.Count; i++) 
					{
						if (answers [i].correct) {
							correctInRow++;
						} else {
							correctInRow = 0;
						}
					}
					isSucceed = (correctInRow == completionQuestionsCorrectLimit);
				break;

				case "Correct":
					int correct = answers.FindAll (x => x.correct == true).Count;
					isSucceed = (correct == completionQuestionsCorrectLimit);
				break;
			}
			return isSucceed;
		}

		public MissionQuestion GetNextQuestion()
		{
			List<MissionQuestion> missionQuestions = questions.FindAll (x => x.numberOfQuestions > 0);
			if (missionQuestions.Count > 0) {

				return missionQuestions [Random.Range (0, missionQuestions.Count)];
			}

			return questions[Random.Range(0,questions.Count)];
		}

		public void UpdateQuestion(string practiceItemID, int difficulty,int subLevel)
		{
			MissionQuestion question = questions.Find (x => x.practiceItemID == practiceItemID && x.difficulty == difficulty && x.subLevel == subLevel);
			if (question != null) 
			{
				if (question.numberOfQuestions > 0) 
				{
					question.numberOfQuestions--;
				}
			}
		}

	}

	[System.Serializable]
	public class MissionQuestion
	{
		public string practiceName;
		public string practiceItemID;
		public int difficulty;
		public int subLevel;
		public int numberOfQuestions;

		public MissionQuestion()
		{
			practiceName = "";
			practiceItemID = "";
			difficulty = 0;
			subLevel = 0;
			numberOfQuestions = 0;
		}

		public MissionQuestion GetMissionQuestion(string json)
		{
			if (!json.IsValidJSON ()) {
				return this;
			}
			return GetMissionQuestion (JSONNode.Parse (json));
		}
		public MissionQuestion GetMissionQuestion (JSONNode jsonNode)
		{
			if (jsonNode == null) {
				return this;
			}

			practiceItemID = jsonNode ["practice_item_id"].Value;
			practiceName = jsonNode ["practice_item_name"].Value;
			difficulty =jsonNode ["difficulity"].AsInt;
			subLevel = jsonNode["sub_level"].AsInt;

			return this;
		}
	}

	[System.Serializable]
	public class MissionAnswer
	{
		public string practiceItemID;
		public int difficulty;
		public int subLevel;
		public int seed;
		public string ans;
		public bool correct;

		public MissionAnswer() :base()
		{
			practiceItemID = "";
			difficulty = 0;
			subLevel = 0;
			seed = 0;
			ans = "";
			correct = false;
		}

		public MissionAnswer(string _practiceItemID,int _seed, string _ans, bool _correct,int _difficulty, int _subLevel)
		{
			practiceItemID = _practiceItemID;
			seed = _seed;
			ans = _ans;
			correct = _correct;
			difficulty = _difficulty;
			subLevel = _subLevel;
		}
	}
		
}
