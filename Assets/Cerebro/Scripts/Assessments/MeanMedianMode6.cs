using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using MaterialUI;

namespace Cerebro {
	public class MeanMedianMode6 : BaseAssessment {

		private string Answer;
		private string[] expressions;
		private string[] questions;
		private int randSelector;
		private int coeff;
		private List<int> iDataSet;
		private List<float> fDataSet;
		private int dataSetLength;
		private int iMean;
		private float fMean;
		private float median;
		private List<int> iModes;
		private List<float> fModes;
		private int iRange;
		private float fRange;

		public Text subQuestionText;

		void Start () {

			base.Initialise ("M", "MMM06", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			GenerateQuestion ();
		}

		public override void SubmitClick(){
			if (ignoreTouches || userAnswerText.text == "") {
				return;
			}
			int increment = 0;
			ignoreTouches = true;
			var correct = true;

			questionsAttempted++;
			updateQuestionsAttempted ();

			float answer = 0;
			float userAnswer = 0;
			bool directCheck = false;

			if (float.TryParse(Answer, out answer))
			{
				answer = float.Parse(Answer);
			}
			else
			{
				directCheck = true;
			}

			if (float.TryParse(userAnswerText.text, out userAnswer))
			{
				userAnswer = float.Parse(userAnswerText.text);
			}
			else
			{
				directCheck = true;
			}
			if (answer != userAnswer)
			{
				correct = false;
			}
			if (directCheck)
			{
				if (userAnswerText.text == Answer)
				{
					correct = true;
				}
				else
				{
					correct = false;
				}
			}
			if (correct == true) {
				if (Queslevel == 1) {
					increment = 5;
				} else if (Queslevel == 2) {
					increment = 10;
				} else if (Queslevel == 3) {
					increment = 15;
				} else if (Queslevel == 4) {
					increment = 15;
				}
					
				UpdateStreak(8, 12);

				updateQuestionsAttempted ();
				StartCoroutine (ShowCorrectAnimation());
			} 
			else {
				for (var i = 0; i < scorestreaklvls.Length; i++) {
					scorestreaklvls [i] = 0;
				}
				StartCoroutine (ShowWrongAnimation());
			}

			base.QuestionEnded (correct, level, increment, selector);

		}

		protected override IEnumerator ShowWrongAnimation() {
			userAnswerText.color = MaterialColor.red800;
			Go.to( userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake( new Vector3( 0, 0, 20 ), GoShakeType.Eulers ) );
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				userAnswerText.text = "";
				userAnswerText.color = MaterialColor.textDark;
				ignoreTouches = false;
			} else {
				userAnswerText.text = Answer.ToString ();
				userAnswerText.color = MaterialColor.green800;
			}
			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			userAnswerText.color = MaterialColor.green800;
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations( 2, GoLoopType.PingPong );
			var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
			var tween = new GoTween( userAnswerText.gameObject.transform, 0.2f, config );
			flow.insert( 0f, tween );
			flow.play ();
			yield return new WaitForSeconds (1f);
			userAnswerText.color = MaterialColor.textDark;

			showNextQuestion ();

			if (levelUp) {
				StartCoroutine (HideAnimation ());
				base.LevelUp ();
				yield return new WaitForSeconds (1.5f);
				StartCoroutine (StartAnimation ());
			}

		}

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			level = Queslevel;
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
				
			subQuestionText.gameObject.SetActive (false);

			#region level1
			if (level == 1) 
			{
				selector = GetRandomSelector (1, 5);

				expressions = new string[] {"mean", "median", "mode", "range"};

				QuestionText.text = string.Format ("Find the {0} of the given data :\n\n", expressions [selector - 1]);

				if (selector == 1)
				{
					dataSetLength = Random.Range (5, 9);
					iDataSet = MathFunctions.GetIntegerMeanDataSet (2, 10, dataSetLength, out iMean);

					Answer = iMean.ToString ();

				} 
				else if (selector == 2)
				{
					dataSetLength = Random.Range (5, 13);
					iDataSet = MathFunctions.GetIntRandomDataSet (2, 10, dataSetLength); 
					median = MathFunctions.GetMedian (iDataSet); 

					Answer = median.ToString (); 

				}
				else if (selector == 3)
				{
					QuestionText.text = QuestionText.text.Substring (0, QuestionText.text.Length - 3);
					QuestionText.text += "(enter N for none) :\n\n";
						
					if (Random.Range (1, 8) == 1)
					{
						iDataSet = MathFunctions.GetIntModeDataSet (2, 10, dataSetLength, out iModes, 0);	
						Answer = "N";
					}
					else
					{
						iDataSet = MathFunctions.GetIntModeDataSet (2, 10, dataSetLength, out iModes, 1);  // this function gives its own dataSetLength 
						Answer = iModes [0].ToString ();
					}
				}
				else if (selector == 4)
				{
					dataSetLength = Random.Range (5, 13);
					iDataSet = MathFunctions.GetIntRandomDataSet (2, 2 * dataSetLength, dataSetLength); 
					iRange = iDataSet.Max () - iDataSet.Min (); 

					Answer = iRange.ToString ();
				}

				for (int i = 0; i < dataSetLength - 1; i++)
					QuestionText.text +=  iDataSet[i] + ", ";
				QuestionText.text +=  iDataSet[dataSetLength - 1].ToString ();

			}
			#endregion
			#region level2
			if (level == 2) 
			{
				selector = GetRandomSelector (1, 5);

				coeff = Random.Range (2, 6);
				expressions = new string[] {"mean", "median", "mode", "range"};
				dataSetLength = 12;
				questions = new string[]{string.Format ("Find the {0} for your data set of rainy days.", expressions [selector - 1]),
					string.Format ("If the number of rainy days doubles each month in the year 2007, what will be the {0} for the 2007 data?", expressions [selector - 1]),
					string.Format ("If there are {0} more rainy days per month in the year 2007, what will be the {1} for the 2007 data?", coeff, expressions [selector - 1])};

				randSelector = Random.Range (0, questions.Length);

				QuestionText.text = string.Format ("In {0} of 2006, your family moved to a tropical climate. For the year that followed, you recorded the number of rainy days that occurred each month (given below). {1} \n\n", MathFunctions.GetMonthName (Random.Range (6, 13)), questions [randSelector]);

				if (selector == 1)
				{
					iDataSet = MathFunctions.GetIntegerMeanDataSet (8, 16, dataSetLength, out iMean);

					Answer = iMean.ToString ();

				} 
				else if (selector == 2)
				{
					iDataSet = MathFunctions.GetIntRandomDataSet (8, 16, dataSetLength); 
					median = MathFunctions.GetMedian (iDataSet); 

					Answer = median.ToString (); 

				}
				else if (selector == 3)
				{
					QuestionText.text = QuestionText.text.Substring (0, QuestionText.text.Length - 2);
					QuestionText.text += "(enter N for none) :\n\n";

					if (Random.Range (1, 8) == 1)
					{
						iDataSet = MathFunctions.GetIntModeDataSet (8, 16, dataSetLength, out iModes, 0);	
						Answer = "N";
					}
					else
					{
						iDataSet = MathFunctions.GetIntModeDataSet (8, 16, dataSetLength, out iModes, 1);  // this function gives its own dataSetLength 
						Answer = iModes [0].ToString ();
					}
				}
				else if (selector == 4)
				{
					iDataSet = MathFunctions.GetIntRandomDataSet (8, 16, dataSetLength); 
					iRange = iDataSet.Max () - iDataSet.Min (); 

					Answer = iRange.ToString ();
				}

				for (int i = 0; i < dataSetLength - 1; i++)
					QuestionText.text +=  iDataSet[i] + ", ";
				QuestionText.text +=  iDataSet[dataSetLength - 1].ToString ();

			}
			#endregion
			#region level3
			if (level == 3) 
			{
				selector = GetRandomSelector (1, 5);

				expressions = new string[] {"mean", "median", "mode", "range"};
				dataSetLength = Random.Range (7, 16);

				QuestionText.text = string.Format ("The exchange rate for sterling against the US dollar for {0} days in {1} 2003 is given in the following table. Calculate the {2} of this data.\n\n", dataSetLength, MathFunctions.GetMonthName (Random.Range (1, 13)), expressions [selector - 1]);

				if (selector == 1)
				{
					fDataSet = MathFunctions.GetFloatMeanDataSet (1.1f, 2.5f, dataSetLength, out fMean);

					Answer = fMean.ToString ();

				} 
				else if (selector == 2)
				{
					fDataSet = MathFunctions.GetFloatRandomDataSet (1.1f, 2.5f, dataSetLength); 
					median = MathFunctions.GetMedian (fDataSet); 

					Answer = median.ToString (); 

				}
				else if (selector == 3)
				{
					QuestionText.text = QuestionText.text.Substring (0, QuestionText.text.Length - 3);
					QuestionText.text += " (enter N for none).\n\n";

					if (Random.Range (1, 8) == 1)
					{
						fDataSet = MathFunctions.GetFloatModeDataSet (1.1f, 2.5f, dataSetLength, out fModes, 0);	
						Answer = "N";
					}
					else
					{
						fDataSet = MathFunctions.GetFloatModeDataSet (1.1f, 2.5f, dataSetLength, out fModes, 1);  // this function gives its own dataSetLength 
						Answer = fModes [0].ToString ();
					}
				}
				else if (selector == 4)
				{
					fDataSet = MathFunctions.GetFloatRandomDataSet (1.1f, 2.5f, dataSetLength); 
					fRange = MathFunctions.GetRounded (fDataSet.Max () - fDataSet.Min (), 2); 

					Answer = fRange.ToString ();
				}

				for (int i = 0; i < dataSetLength - 1; i++)
					QuestionText.text +=  fDataSet[i] + ", ";
				QuestionText.text +=  fDataSet[dataSetLength - 1].ToString();

			}
			#endregion
			CerebroHelper.DebugLog (Answer);
			userAnswerText = GeneralButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
		}

		public override void numPadButtonPressed(int value) {
			if (ignoreTouches) {
				return;
			}
			if (value <= 9) {
				userAnswerText.text += value.ToString ();
			} else if (value == 10) {    //Back
				if (userAnswerText.text.Length > 0) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
			} else if (value == 11) {   // All Clear
				userAnswerText.text = "";
			} else if (value == 12) {
				userAnswerText.text = "N";
			} else if (value == 13) {
				userAnswerText.text += ".";
			}
		}
	}
}