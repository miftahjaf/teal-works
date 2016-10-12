using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using MaterialUI;

namespace Cerebro
{
	public class EquationsAndInequations7 : BaseAssessment
    {

		public TEXDraw subQuestionText;
        private string Answer;
		private List<string> AnswerList;
		private string[] numberType = new string[] {"N", "W"};
		private int randSelector;
        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
        private int coeff6;
		private int ans;
		private int ans_num;
		private int ans_den;
		private bool answerIsList;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "EAI07", "S01", "A01");

            scorestreaklvls = new int[5];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;

            coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = coeff6 = 0;
            
            Answer = "";
            GenerateQuestion();
        }

		bool checkAnswerLists(List<string> A, List<string> B)
		{
			if (A.Count != B.Count)
				return false;

			int length = A.Count; // = B.Count

			foreach (string str in A)
			{
				if (!B.Contains (str))
					return false;
			}
			return true;
		}

        public override void SubmitClick()
        {
            if (ignoreTouches || userAnswerText.text == "")
            {
                return;
            }
            int increment = 0;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            var correct = true;
            CerebroHelper.DebugLog("!" + userAnswerText.text + "!");
            CerebroHelper.DebugLog("*" + Answer + "*");
            questionsAttempted++;
            updateQuestionsAttempted();
            float answer = 0;
            float userAns = 0;

			if (answerIsList)
			{
				string[] userAnswerSplits = userAnswerText.text.Split (new string[] { "," }, System.StringSplitOptions.None);

				List<string> userAnswers = new List<string>();

				for (var i = 0; i < userAnswerSplits.Length; i++)
					userAnswers.Add(userAnswerSplits[i]);

				if (checkAnswerLists (AnswerList, userAnswers))
				{
					correct = true;
				}
				else
				{
					correct = false;
				}
			}
			else
			{
				bool directCheck = false;
            	if (float.TryParse(Answer, out answer))
            	{
                	answer = float.Parse(Answer);
            	}
            	else
            	{
                	directCheck = true;
            	}
				if (float.TryParse(userAnswerText.text, out userAns))
            	{
                	userAns = float.Parse(userAnswerText.text);
            	}
				else
            	{
               		correct = false;
            	}
            	if (answer != userAns)
            	{
                	correct = false;
            	}
			}
            if (correct == true)
            {
                if (Queslevel == 1)
                {
                    increment = 5;
                }
                else if (Queslevel == 2)
                {
                    increment = 5;
                }
                else if (Queslevel == 3)
                {
                    increment = 10;
                }
                else if (Queslevel == 4)
                {
                    increment = 10;
                }
				else if (Queslevel == 5)
				{
					increment = 15;
				}
				else if (Queslevel == 6)
				{
					increment = 15;
				}


                UpdateStreak(8, 12);

                updateQuestionsAttempted();
                StartCoroutine(ShowCorrectAnimation());
            }
            else
            {
                for (var i = 0; i < scorestreaklvls.Length; i++)
                {
                    scorestreaklvls[i] = 0;
                }
                StartCoroutine(ShowWrongAnimation());
            }

            base.QuestionEnded(correct, level, increment, selector);

        }

        protected override IEnumerator ShowWrongAnimation()
        {
            userAnswerText.color = MaterialColor.red800;
			Go.to(userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
            yield return new WaitForSeconds(0.5f);
            if (isRevisitedQuestion)
            {
                if (numPad.activeSelf)
                {               // is not MCQ type question
                    userAnswerText.text = "";
                }
                if (userAnswerText != null)
                {
                    userAnswerText.color = MaterialColor.textDark;
                }
                if (userAnswerText != null)
                {
                    userAnswerText.color = MaterialColor.textDark;
                }
                ignoreTouches = false;
            }
            else
            {
                if (numPad.activeSelf)
                {               // is not MCQ type question
                    CerebroHelper.DebugLog("going in if");
					if (answerIsList) 
					{
						userAnswerText.text = "";

						foreach (string str in AnswerList)
							userAnswerText.text += str + ",";

						userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
					}
                    else
						userAnswerText.text = Answer.ToString();
                    userAnswerText.color = MaterialColor.green800;
                }
                else
                {
                    CerebroHelper.DebugLog("going in else");
                    userAnswerText.color = MaterialColor.textDark;
                }
            }

            ShowContinueButton();
        }

        protected override IEnumerator ShowCorrectAnimation()
        {
            userAnswerText.color = MaterialColor.green800;
            var config = new GoTweenConfig()
                .scale(new Vector3(1.1f, 1.1f, 0))
                .setIterations(2, GoLoopType.PingPong);
            var flow = new GoTweenFlow(new GoTweenCollectionConfig().setIterations(1));
			var tween = new GoTween(userAnswerText.gameObject.transform, 0.2f, config);
            flow.insert(0f, tween);
            flow.play();
            yield return new WaitForSeconds(1f);
            userAnswerText.color = MaterialColor.textDark;

            showNextQuestion();

            if (levelUp)
            {
                StartCoroutine(HideAnimation());
                base.LevelUp();
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(StartAnimation());
            }

        }

        protected override void GenerateQuestion()
        {
            ignoreTouches = false;
            base.QuestionStarted();
            // Generating the parameters

            level = Queslevel;

            answerButton = GeneralButton;

			subQuestionText.gameObject.SetActive (true);
            QuestionText.gameObject.SetActive (true);
            GeneralButton.gameObject.SetActive (true);
            numPad.SetActive (true);

			AnswerList = new List<string> ();
			answerIsList = false;

            if (Queslevel > scorestreaklvls.Length)
            {
                level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
            }

            #region level1
            if (level == 1)
            {
                selector = GetRandomSelector (1, 6);

                if (selector == 1)
                {
                    coeff1 = Random.Range (1, 50);
					coeff2 = Random.Range (50, 100);

                    QuestionText.text = "Solve for x :";
                    subQuestionText.text = coeff1 + " + x = " + coeff2;

                    ans = coeff2 - coeff1;
                    Answer = ans.ToString();
                }
				else if (selector == 2)
				{
					coeff1 = Random.Range (2, 10);
					coeff2 = coeff1 * Random.Range (2, 10);

					QuestionText.text = "Solve for x :";
					subQuestionText.text = coeff1 + "x = " + coeff2;

					ans = coeff2 / coeff1;
					Answer = ans.ToString();
				}
                else if (selector == 3)
                {
                    coeff1 = Random.Range (2, 10);
                    coeff2 = Random.Range (2, 10);

					QuestionText.text = "Solve for x :";
					subQuestionText.text = "x/" + coeff1 + " = " + coeff2;

					ans = coeff1 * coeff2;
                    Answer = ans.ToString();
                }
				else if (selector == 4)
				{
					answerIsList = true;
					randSelector = Random.Range (0, numberType.Length);

					coeff1 = Random.Range (1, 10);
					coeff2 = coeff1 + Random.Range (2, 8);

					QuestionText.text = "Solve for x :";
					subQuestionText.text = "x + " + coeff1 + " \\leq " + coeff2 + "\t, x \\epsilon " + numberType [randSelector];

					for (int i = (numberType [randSelector] == "W" ? 0: 1); i <= coeff2 - coeff1; i++)
						AnswerList.Add ("" + i);
				}
				else if (selector == 5)
				{
					answerIsList = true;
					randSelector = Random.Range (0, numberType.Length);

					coeff1 = Random.Range (2, 10);
					coeff2 = coeff1 * Random.Range (2, 8);

					QuestionText.text = "Solve for x :";
					subQuestionText.text = "-" + coeff1 + "x \\geq -" + coeff2 + "\t, x \\epsilon " + numberType [randSelector];

					for (int i = (numberType [randSelector] == "W" ? 0: 1); i <= coeff2 / coeff1; i++)
						AnswerList.Add ("" + i);
				}
            }
            #endregion
            #region level2

            if (level == 2)
            {
                selector = GetRandomSelector (1, 6);
                
                if (selector == 1)
                {
                    coeff1 = Random.Range (1, 10);
                    coeff2 = Random.Range (1, 10);
                    coeff3 = Random.Range (2, 10);
					coeff4 = Random.Range (5, 15);

					while (MathFunctions.GetHCF (coeff2, coeff3) != 1 || coeff3 <= coeff2)
					{
						coeff2 = Random.Range (1, 10);
						coeff3 = Random.Range (2, 10);
					}

					QuestionText.text = "Solve for x :";
					subQuestionText.text = "x - " + coeff1 + "\\frac{" + coeff2.ToString () + "}{" + coeff3.ToString () + "} = " + coeff4;

					ans_num = (coeff4 + coeff1) * coeff3 + coeff2;
					ans_den = coeff3;
					Answer = ans_num.ToString () + "/" + ans_den.ToString ();
                }
                else if (selector == 2)
                {
					coeff1 = Random.Range (2, 10);
					coeff2 = coeff1 * Random.Range (2, 10);
					coeff3 = coeff1 * Random.Range (10, 20);

					QuestionText.text = "Solve for x :";
					subQuestionText.text = coeff1 + "x + " + coeff2 + " = " + coeff3;

					ans = (coeff3 - coeff2)/ coeff1;
					Answer = ans.ToString();
                }
                else if (selector == 3)
                {
					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (2, 10);
					coeff3 = Random.Range (10, 20);

					while (coeff1 == coeff2)
						coeff2 = Random.Range (2, 10);

					coeff4 = 1 + coeff1 + coeff2;

					QuestionText.text = "Solve for x :";
					subQuestionText.text = coeff1 + "(x - " + coeff2 + ") + " + coeff2 + "(x - " + coeff1 + ") = " + coeff3 + " + " + coeff4 + "(x - " + coeff3 + ")";

					ans = coeff3 * (coeff1 + coeff2) - 2 * coeff1 * coeff2;
					Answer = ans.ToString();
                }
                else if (selector == 4)
                {
					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (2, 6);
					coeff3 = Random.Range (2, 10);
					coeff4 = Random.Range (2, 6);
					coeff5 = Random.Range (2, 10);

					QuestionText.text = "Solve for x :";
					subQuestionText.text = "\\frac{" + "x - " + coeff2.ToString() + "}{" + (2 * coeff2).ToString() + "} - \\frac{"+ coeff3.ToString() + "x + " + coeff4.ToString() + "}{" + (3 * coeff2).ToString() + "} = " + coeff5;

					ans_num = (6 * coeff2 * coeff5 + 3 * coeff1 + 2 * coeff4);
					ans_den = 2 * coeff3 - 3;
					int hcf = MathFunctions.GetHCF (ans_num, ans_den);

					if (hcf == ans_den)
						Answer = "-" + (ans_num/hcf).ToString();
					else
	    				Answer = "-" + (ans_num/hcf).ToString() + "/" + (ans_den/hcf).ToString();
                }
                else if (selector == 5)
                {
					answerIsList = true;
					randSelector = Random.Range (0, numberType.Length);

					coeff2 = Random.Range (2, 6);
					coeff3 = Random.Range (6, 11);
					coeff1 = coeff2 * Random.Range (2, 6) - coeff3;
					coeff4 = coeff2 * Random.Range (6, 10) - coeff3;

					QuestionText.text = "Solve for x :";
					subQuestionText.text = coeff1 + " \\leq " + coeff2 + "x - " + coeff3 + " \\leq " + coeff4 + "\t, x \\epsilon " + numberType [randSelector];

					int lower = (coeff1 + coeff3) / coeff2;
					int upper = (coeff4 + coeff3) / coeff2;

					for (int i = lower; i <= upper; i++)
						AnswerList.Add ("" + i);
                }
            }
            #endregion
            #region level3
            if (level == 3)
            {
                selector = GetRandomSelector (1, 6);

                if (selector == 1)
                {
					coeff1 = Random.Range (2, 10);
					coeff3 = Random.Range (2, 10);
					coeff2 = coeff1 * coeff3 + Random.Range (2, 10);
					coeff4 = (coeff2 - coeff1 * coeff3) * Random.Range (2, 10) - coeff2 * coeff3;

					QuestionText.text = "Solve for m :";
					subQuestionText.text = "(" + coeff1 + "m + " + coeff2 + ")(m - " + coeff3 + ") = " + coeff4 + " + " + coeff1 + "m^2";

					ans = (coeff4 + coeff2 * coeff3)/(coeff2 - coeff1 * coeff3);
					Answer = ans.ToString();
                }
                else if (selector == 2)
                {
					float coeff1 = 0.2f * (float) Random.Range (2, 20);
					float coeff4 = coeff1 / 2f;
					float coeff2 = coeff4 * (float) Random.Range (2, 10);
					float coeff3 = coeff4 * (float) Random.Range (2, 10);

					while (coeff1 == coeff2 || coeff1 == 1f || coeff1 == 2f || coeff2 == coeff3)
					{
						coeff1 = 0.2f * (float) Random.Range (2, 20);
						coeff4 = coeff1 / 2;
						coeff2 = coeff4 * (float) Random.Range (2, 10);
						coeff3 = coeff4 * (float) Random.Range (2, 10);
					}

					QuestionText.text = "Solve for x :";
					subQuestionText.text = coeff1 + "x - " + coeff2 + " + " + coeff3 + " = " + coeff4 + "x";

					float ans = (coeff2 - coeff3) / (coeff1 - coeff2);
					Answer = ans.ToString();
                }
                else if (selector == 3)
                {
					coeff1 = Random.Range (2, 10);
					coeff2 = 5 * Random.Range (2, 11);
					coeff3 = Random.Range (2, 21);

					while ((coeff2 * coeff3) % 100 != 0)
					{
						coeff2 = 5 * Random.Range (2, 10);
						coeff3 = Random.Range (2, 11);
					}

					coeff4 = (coeff1 + coeff2 * coeff3 / 100) * Random.Range (5, 20);

					QuestionText.text = "Solve for m :";
					subQuestionText.text = coeff1 + "m + " + coeff2 + "% of " + coeff3 + "m = " + coeff4;

					ans = 100 * coeff4 / (100 * coeff1 + coeff2 * coeff3);
					Answer = ans.ToString();
                }
                else if (selector == 4)
                {
					answerIsList = true;
					randSelector = Random.Range (0, numberType.Length);

					coeff1 = Random.Range (2, 10);
					coeff3 = coeff1 * Random.Range (5, 15);
					coeff2 = -1;

					while (coeff2 <= 0)
						coeff2 = coeff3 / coeff1 - Random.Range (2, coeff3 / coeff1);
					
					QuestionText.text = "Solve for x :";
					subQuestionText.text = "-" + coeff1 + "(x + " + coeff2 + ") > -" + coeff3 + "\t, x \\epsilon " + numberType [randSelector];

					for (int i = (numberType [randSelector] == "W" ? 0: 1); i < coeff3 / coeff1 - coeff2; i++)
						AnswerList.Add ("" + i);
                }
                else if (selector == 5)
                {
					answerIsList = true;
					randSelector = Random.Range (0, numberType.Length);

					coeff4 = Random.Range (2, 10);
					coeff1 = coeff4 + Random.Range (2, 6);
					coeff2 = Random.Range (2, 10);
					coeff5 = Random.Range (2, 10);
					coeff3 = 1;

					while (coeff3 >= 0)
						coeff3 = (coeff1 - coeff4) * Random.Range (1,4) - coeff5 * coeff4 - coeff1 * coeff2;
					
					QuestionText.text = "Solve for x :";
					subQuestionText.text = coeff1 + "(x - " + coeff2 + ") " + coeff3 + " \\leq " + coeff4 + "(x + " + coeff5 + ")\t, x \\epsilon " + numberType [randSelector];

					for (int i = (numberType [randSelector] == "W" ? 0: 1); i <= (coeff4 * coeff5 - coeff3 + coeff1 * coeff2) / (coeff1 - coeff4); i++)
						AnswerList.Add ("" + i);
                }
            }
            #endregion
			#region level4

			if (level == 4)
			{
				selector = GetRandomSelector (1, 6);
				subQuestionText.gameObject.SetActive (false);

				if (selector == 1)
				{
					coeff1 = Random.Range (100, 500);
					coeff2 = Random.Range (100, 500);

					QuestionText.text = "What number must be subtracted from " + coeff1 + " to get " + coeff2 + "?";

					ans = coeff1 - coeff2;
					Answer = ans.ToString() ;
				}
				else if (selector == 2)
				{
					coeff1 = Random.Range (10, 30);
					coeff2 = Random.Range (50, 100);

					while ((coeff2 - coeff1) % 2 != 0)
						coeff1 = Random.Range (10, 50);
					
					QuestionText.text = "A number is " + coeff1 + " less than another. Their sum is found to be " + coeff2 + ". Determine the number.";

					ans = (coeff2 - coeff1)/2;
					Answer = ans.ToString() ;
				}
				else if (selector == 3)
				{
					coeff1 = 11 * Random.Range (2, 20);

					QuestionText.text = "One fifth of a number is added to one sixth of it and the sum equals to " + coeff1 + ". Determine the number.";

					ans = 30 * (coeff1/11);
					Answer = ans.ToString();
				}
				else if (selector == 4)
				{
					coeff1 = 4 * Random.Range (15, 50) + 6;

					QuestionText.text = "If the sum of four consecutive whole numbers is " + coeff1 + ". Determine the largest number.";

					ans = (coeff1 - 6)/4 + 3;
					Answer = ans.ToString();
				}
				else if (selector == 5)
				{
					subQuestionText.gameObject.SetActive (true);

					answerIsList = true;
					randSelector = Random.Range (0, numberType.Length);

					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (2, 10);
					coeff3 = Random.Range (2, 10);
					coeff4 = Random.Range (2, 10);

					while (coeff1 <= coeff4 && (coeff1 - coeff4) * coeff3 <= 8)
					{
						coeff1 = Random.Range (2, 10);
						coeff3 = Random.Range (2, 10); 
						coeff4 = Random.Range (2, 10); 
					}

					QuestionText.text = "Solve for x:";
					subQuestionText.text = coeff1 + " > \\frac{" + " x - " + coeff2.ToString() + "}{" + coeff3.ToString() +"} > " + coeff4 + "\t, x \\epsilon " + numberType [randSelector];

					for (int i = coeff2 + coeff3 * coeff4 + 1; i < coeff2 + coeff3 * coeff2; i++)
						AnswerList.Add ("" + i);
				}
			}
			#endregion
			#region level5

			if (level == 5)
			{
				selector = GetRandomSelector (1, 6);
				subQuestionText.gameObject.SetActive (false);

				if (selector == 1)
				{
					coeff1 = 4 * Random.Range (10, 25);

					QuestionText.text = "If the sum of two consecutive odd numbers is " + coeff1 + ", determine the numbers.";

					ans = (coeff1 - 2 )/2;
					Answer = ans.ToString() + ", " + (ans + 2).ToString();
				}
				else if (selector == 2)
				{
					coeff1 = Random.Range (40, 56);
					coeff2 = Random.Range (5, 19);

					QuestionText.text = "A father is " + coeff1 + " years old and his son is " + coeff2 + " years old. After how many years will the father's age be twice that of his son ?";

					ans = coeff1 - 2 * coeff2;
					Answer = ans.ToString();
				}
				else if (selector == 3)
				{
					coeff1 = Random.Range (2, 6);
					coeff2 = Random.Range (2, 6);

					while (coeff1 == coeff2)
						coeff2 = Random.Range (2, 6);
					
					coeff3 = (coeff1 + coeff2) * 10 * Random.Range (10, 21);
					coeff4 = (coeff1 + coeff2) * 10 * Random.Range (2, 6);

					QuestionText.text = "The cost of " + coeff1 + " balls and " + coeff2 + " bats is Rs. " + coeff3 + ". If the cost of bat is Rs. " + coeff4 + " more than the ball, then determine the cost (in Rs.) of one ball and one bat.";

					ans = 2 * (coeff3 - coeff2 * coeff4)/(coeff1 + coeff2) + coeff4;
					Answer = ans.ToString();
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range (10, 31);
					coeff2 = Random.Range (80, 121);

					while ((2 * coeff1 + coeff2) % 6 != 0)
						coeff2 = Random.Range (80, 121);
					
					QuestionText.text = "The length of a rectangular playground is " + coeff1 + "m less than twice its breadth. If the perimeter of the ground is " + coeff2 + "m, determine the length and the breadth of the ground.";

					int breadth = (2 * coeff1 + coeff2) / 6;
					int length = 2 * breadth - coeff1;

					Answer = length.ToString() + "," + breadth.ToString();
				}
				else if (selector == 5)
				{
					coeff1 = 2 * Random.Range (10, 41);

					QuestionText.text = "If two supplementary angles differ from one another by " + coeff1 + MathFunctions.deg + ", determine the measure of the smaller angle (in " + MathFunctions.deg + ").";

					ans = (180 - coeff1) / 2;
					Answer = ans.ToString();
				}
			}
			#endregion
            userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
            userAnswerText.text = "";
        }

        public override void numPadButtonPressed(int value)
        {
            if (ignoreTouches)
            {
                return;
            }
            if (value <= 9)
            {
                userAnswerText.text += value.ToString();
            }
            else if (value == 10)
            {    //.
				if (checkLastTextFor (new string[1] { "." }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += ".";
            }
            else if (value == 11)
            {   // All Clear
                if (userAnswerText.text.Length > 0)
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
            }
			else if (value == 12)
			{    //,
				if (checkLastTextFor (new string[1] { "," }))
				{
					userAnswerText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerText.text += ",";
			}
			else if (value == 13)
			{    //,
				if (checkLastTextFor (new string[1] { "/" }))
				{
					userAnswerText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerText.text += "/";
			}
        }
    }
}

