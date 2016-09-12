using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
    public class SpecialProducts7 : BaseAssessment
    {

        public Text subQuestionText;
        public TEXDraw subQuestionTEX;
		public GameObject MCQ;
        private string Answer;
        private int AnswerInt;
        private string[] Answerarray;

        int flag;  
       // private TEXDraw Answer2;
        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
        private int coeff6;
        private int pow1, pow2, pow3;
        private int randact, splitans;
        private int x, y, z, a, b, c;
        private string expression1;
        private string expression2;
        private string expression3;
        private string expression4;
        private string expression5;
        private int upflag=0;
		private bool SingleExpression;
		private bool FracUpper;
		private bool FracLower;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "SPP07", "S01", "A01");

            scorestreaklvls = new int[5];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;

            coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = coeff6 = 0;
            x = y = z = a = b = c = 0;

            Answer = "";
            GenerateQuestion();
        }

        bool checkArrayValues(string[] A, string[] B)
        {
            if (A.Length != B.Length)
            {
                CerebroHelper.DebugLog(A.Length);
                CerebroHelper.DebugLog(B.Length);
                CerebroHelper.DebugLog("Length not equal");
                return false;
            }
            for (var i = 0; i < A.Length; i++)
            {
                var found = false;
                for (var j = 0; j < B.Length; j++)
                {
                    if (A[i] == B[j])
                    {
                        CerebroHelper.DebugLog(A[i] + "found");
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    CerebroHelper.DebugLog(A[i] + " not found");
                    return false;
                }
            }
            return true;
        }

        public override void SubmitClick()
        {
            if (ignoreTouches || userAnswerLaText.text == "")
            {
                return;
            }

			upflag = 0;
			FracUpper = false;
			FracLower = false;
			numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
			numPad.transform.Find ("PanelLayer").Find ("upper").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
			numPad.transform.Find ("PanelLayer").Find ("lower").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

            int increment = 0;
            //var correct = false;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            var correct = true;
            CerebroHelper.DebugLog("!" + userAnswerLaText.text + "!");
            CerebroHelper.DebugLog("*" + Answer + "*");
            questionsAttempted++;
            updateQuestionsAttempted();
			if (MCQ.activeSelf) 
			{
				if (Answer == userAnswerLaText.text) 
				{
					correct = true;
				}
				else
				{
					correct = false;
					AnimateMCQOptionCorrect(Answer);

				}
			}
			else if (!SingleExpression)
            {
				userAnswerLaText.text.Replace ("-", "+-");
                string[] userAnswerSplits = userAnswerLaText.text.Split(new string[] { "+" }, System.StringSplitOptions.None);

                List<string> userAnswers = new List<string>();

                for (var i = 0; i < userAnswerSplits.Length; i++)
                {
                    string userAnswer = "";
                    userAnswer = userAnswerSplits[i];
                    userAnswers.Add(userAnswer);
                }

				Answer.Replace ("-", "+-");
				Answerarray = Answer.Split(new string[] { "+" }, System.StringSplitOptions.None);

				List<string> Answers = new List<string>();

				for (var i = 0; i < Answerarray.Length; i++)
				{
					string userAnswer = "";
					userAnswer = Answerarray[i];
					Answers.Add(userAnswer);
				}

				if (checkArrayValues(Answers.ToArray(), userAnswers.ToArray()))
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
				if (float.TryParse(userAnswerLaText.text, out userAnswer))
				{
					userAnswer = float.Parse(userAnswerLaText.text);
				}
				else
				{
					correct = false;
				}
				if (answer != userAnswer)
				{
					correct = false;
				}
				if (directCheck)
				{
					if (userAnswerLaText.text == Answer)
					{
						correct = true;
					}
					else
					{
						correct = false;
					}
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
					increment = 10;
				}

                UpdateStreak(12, 16);

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
            userAnswerLaText.color = MaterialColor.red800;
			Go.to(userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
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
				if (userAnswerLaText != null)
				{
					userAnswerLaText.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
			}
			else
			{
				if (numPad.activeSelf)
				{               // is not MCQ type question
					print("going in if");
					if (AnswerInt != 9999)
						userAnswerLaText.text = AnswerInt.ToString();
					else
						userAnswerLaText.text = Answer.ToString();
					userAnswerLaText.color = MaterialColor.green800;
				}
				else
				{
					CerebroHelper.DebugLog("going in else");
					userAnswerLaText.color = MaterialColor.textDark;
				}
			}
            ShowContinueButton();
        }
        protected override IEnumerator ShowCorrectAnimation()
        {
            userAnswerLaText.color = MaterialColor.green800;
            var config = new GoTweenConfig()
                .scale(new Vector3(1.1f, 1.1f, 1f))
                .setIterations(2, GoLoopType.PingPong);
            var flow = new GoTweenFlow(new GoTweenCollectionConfig().setIterations(1));
			var tween = new GoTween(userAnswerLaText.gameObject.transform, 0.2f, config);
            flow.insert(0f, tween);
            flow.play();
            yield return new WaitForSeconds(1f);
            userAnswerLaText.color = MaterialColor.textDark;

            showNextQuestion();

            if (levelUp)
            {
                StartCoroutine(HideAnimation());
                base.LevelUp();
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(StartAnimation());
            }

        }

		void AnimateMCQOptionCorrect(string ans)
		{
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
		}

		public void MCQOptionClicked (int value)
		{
			if (ignoreTouches) {
				return;
			}
			AnimateMCQOption (value);
			userAnswerLaText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<TEXDraw> ();
			answerButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
			SubmitClick ();
		}

		IEnumerator AnimateMCQOption (int value)
		{
			var GO = MCQ.transform.Find ("Option" + value.ToString ()).gameObject;
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
		}

		int[] SolveEquation(int No1, int No2, int No3, int No4)
		{
			int[] ans = new int[3];
			ans [0] = No1 * No3;
			ans [1] = No2 * No3 + No1 * No4;
			ans [2] = No2 * No4;
			return ans;
		}

		float[] SolveEquation(float No1, float No2, float No3, float No4)
		{
			float[] ans = new float[3];
			ans [0] = No1 * No3;
			ans [0] = (float)System.Math.Round ((((float)ans [0]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
			ans [1] = No2 * No3 + No1 * No4;
			ans [1] = (float)System.Math.Round ((((float)ans [1]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
			ans [2] = No2 * No4;
			ans [2] = (float)System.Math.Round ((((float)ans [2]) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
			return ans;
		}

		int[] SolveEquationDiffPoly(int No1, int No2, int No3, int No4)
		{
			int[] ans = new int[4];
			ans [0] = No1 * No3;
			ans [1] = No2 * No3;
			ans [2] = No1 * No4;
			ans [3] = No2 * No4;
			return ans;
		}

		void RandomizeMCQOptionsAndFill(List<string> options)
		{
			int index = 0;
			int cnt = options.Count;
			for (int i = 1; i <= cnt; i++) {
				index = Random.Range (0, options.Count);
				MCQ.transform.Find ("Option"+i).Find ("Text").GetComponent<TEXDraw> ().text = options [index];
				options.RemoveAt (index);
			}
		}

        protected override void GenerateQuestion()
        {
            ignoreTouches = false;
            base.QuestionStarted();
            // Generating the parameters

            level = Queslevel;

            answerButton = GeneralButton;
            GeneralButton.gameObject.SetActive(true);
            subQuestionText.gameObject.SetActive(false);
            subQuestionTEX.gameObject.SetActive(false);
            QuestionText.gameObject.SetActive(true);
            numPad.SetActive(true);
			MCQ.SetActive (false);
			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}

            subQuestionText.text = null;
            QuestionText.text = null;
            subQuestionTEX.text = null;

            AnswerInt = 9999;
            flag = 0;

            if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
            
			subQuestionTEX.gameObject.SetActive(true);
			SingleExpression = false;
            
            #region level1
            if (level == 1)
            {				          
                selector = GetRandomSelector(1,6);

                if (selector == 1)
                {					
					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2,10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(x+" + coeff1 + ")(x+" + coeff2 + ")";
					int[] Ans = SolveEquation(1, coeff1, 1, coeff2);
					MCQ.SetActive (true);
					GeneralButton.gameObject.SetActive(false);
					numPad.SetActive(false);
					List<string> options = new List<string>();
					options.Add("x^{2}+"+(Ans[1] == 1 ? "" : Ans[1].ToString())+"x+"+Ans[2]);
					options.Add("x^{2}+"+(Ans[2] == 1 ? "" : Ans[2].ToString())+"x+"+Ans[1]);
					options.Add("x^{2}+"+Ans[2]);
					options.Add("x^{2}+"+(Ans[1] == 1 ? "" : Ans[1].ToString())+"x+"+(2*Ans[2]));
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);
				} 
				else if (selector == 2)
				{
					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2,10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					coeff2 *= -1;
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(x+" + coeff1 + ")(x" + coeff2 + ")";
					int[] Ans = SolveEquation(1, coeff1, 1, coeff2);
					Answer = "x^{2}"+(Ans[1].ToString().Contains("-")?(Ans[1] == -1 ? "-" : Ans[1].ToString()):(Ans[1] == 1 ? "+" : Ans[1].ToString()))+"x"+Ans[2];
					MCQ.SetActive (true);
					GeneralButton.gameObject.SetActive(false);
					numPad.SetActive(false);
					List<string> options = new List<string>();
					options.Add("x^{2}"+(Ans[1].ToString().Contains("-")?(Ans[1] == -1 ? "-" : Ans[1].ToString()):(Ans[1] == 1 ? "+" : Ans[1].ToString()))+"x"+Ans[2]);
					options.Add("x^{2}"+(Ans[2].ToString().Contains("-")?(Ans[2] == -1 ? "-" : Ans[2].ToString()):(Ans[2] == 1 ? "+" : Ans[2].ToString()))+"x"+(Ans[1].ToString().Contains("-")?"":"+")+Ans[1]);
					options.Add("x^{2}"+Ans[2]);
					options.Add("x^{2}"+(Ans[1].ToString().Contains("-")?(Ans[1] == -1 ? "-" : Ans[1].ToString()):(Ans[1] == 1 ? "+" : Ans[1].ToString()))+"x"+(2*Ans[2]));
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);
				}
				else if (selector == 3)
				{
					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2,10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					coeff2 *= -1;
					float No1 = coeff1 / 10f;
					float No2 = coeff2 / 10f;
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(xy+" + No1 + ")(xy" + No2 + ")";
					float[] Ans = SolveEquation(1f, No1, 1f, No2);
					MCQ.SetActive (true);
					GeneralButton.gameObject.SetActive(false);
					numPad.SetActive(false);
					List<string> options = new List<string>();
					options.Add("x^{2}y^{2}"+(Ans[1].ToString().Contains("-")?(Ans[1] == -1 ? "-" : Ans[1].ToString()):(Ans[1] == 1 ? "+" : Ans[1].ToString()))+"xy"+(Ans[2].ToString().Contains("-")?"":"+")+Ans[2]);
					options.Add("x^{2}y^{2}"+(Ans[2].ToString().Contains("-")?(Ans[1] == -1 ? "-" : Ans[2].ToString()):(Ans[1] == 1 ? "+" : Ans[1].ToString()))+"xy"+(Ans[1].ToString().Contains("-")?"":"+")+Ans[1]);
					options.Add("x^{2}y^{2}"+(Ans[2].ToString().Contains("-")?"":"+")+Ans[2]);
					options.Add("x^{2}y^{2}"+(Ans[1].ToString().Contains("-")?(Ans[1] == -1 ? "-" : Ans[1].ToString()):(Ans[1] == 1 ? "+" : Ans[1].ToString()))+"xy"+(Ans[2].ToString().Contains("-")?"":"+")+(2*Ans[2]));
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range(2,5);
					coeff2 = Random.Range(coeff1,10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(coeff1,10);
					coeff1 *= -1;
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(x^{2}" + coeff1 + "y)(x^{2}+" + coeff2 + "y)";
					int[] Ans = SolveEquation(1, coeff1, 1, coeff2);
					Answer = "x^{4}"+(Ans[1].ToString().Contains("-")?"":"+")+(Ans[1] == 1 ? "" : Ans[1].ToString())+"x^{2}y" + (Ans[2] < 0 ? "" : "+") +Ans[2]+"y^{2}";  
				}
				else if (selector == 5)
				{
					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2, 5);
					coeff3 = Random.Range(2,10);
					coeff4 = coeff2 * Random.Range(2, 5);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					while(coeff3 == coeff4)
						coeff4 = coeff2 * Random.Range(2, 5);
					int hcf = MathFunctions.GetHCF(coeff1, coeff2);
					if(hcf > 1)
					{
						coeff1 /= hcf;
						coeff2 /= hcf;
					}
					hcf = MathFunctions.GetHCF(coeff3, coeff4);
					if(hcf > 1)
					{
						coeff3 /= hcf;
						coeff4 /= hcf;
					}
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(\\frac{" + coeff1 + "}{" + coeff2 + "}+x)(\\frac{" + coeff3 + "}{" + coeff4 + "}-x)";
					int No1 = coeff1 * coeff3;
					int No2 = coeff2 * coeff4;
					hcf = MathFunctions.GetHCF(No1, No2);
					if(hcf > 1)
					{
						No1 /= hcf;
						No2 /= hcf;
					}
					int No3 = coeff3 - coeff1;
					hcf = MathFunctions.GetHCF(No3, coeff2);
					if(hcf > 1)
					{
						No3 /= hcf;
						coeff2 /= hcf;
					}
					string sign = Mathf.Sign(No3) == 1 ? "+" : "-";
					if(No3 < 0)
						No3 *= -1;
					if(coeff2 == 1)
						Answer = "\\frac{"+ No1 + "}{" + No2 + "}" + sign + (No3 == 1 ? "" : No3.ToString()) + "x-x^{2}";
					else
						Answer = "\\frac{"+ No1 + "}{" + No2 + "}" + sign + "\\frac{" + (No3 == 1 ? "" : No3.ToString()) + "}{" + coeff2 + "}x-x^{2}";
				}
            }
            #endregion
            #region level2
			else if(level == 2)
			{
				selector = GetRandomSelector(1,6);

				if (selector == 1)
				{
					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2,10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(" + coeff1 + "x+" + coeff2 + "y)^{2}";
					int[] Ans = SolveEquationDiffPoly(coeff1, coeff2, coeff1, coeff2);
					Answer = Ans[0] + "x^{2}+"+(Ans[1]+Ans[2])+"xy+"+Ans[3]+"y^{2}";  
				}
				else if (selector == 2)
				{
					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					coeff3 = Random.Range(2,10);
					coeff4 = Random.Range(2, 10);
					while(coeff3 == coeff4)
						coeff4 = Random.Range(2,10);
					int hcf = MathFunctions.GetHCF(coeff1, coeff2);
					if(hcf > 1)
					{
						coeff1 /= hcf;
						coeff2 /= hcf;
					}
					hcf = MathFunctions.GetHCF(coeff3, coeff4);
					if(hcf > 1)
					{
						coeff3 /= hcf;
						coeff4 /= hcf;
					}
					coeff5 = Random.Range(2, 5);
					QuestionText.text = "Solve:";
					subQuestionTEX.text = coeff5 + "(\\frac{" + coeff1 + "}{" + coeff2 + "}x+\\frac{" + coeff3 + "}{" + coeff4 + "}y)^{2}";
					int No1 = coeff1 * coeff1 * coeff5;
					int No2 = coeff2 * coeff2;
					hcf = MathFunctions.GetHCF(No1, No2);
					if(hcf > 1)
					{
						No1 /= hcf;
						No2 /= hcf;
					}
					int No3 = coeff1 * coeff3 * coeff5 * 2;
					int No4 = coeff2 * coeff4;
					int hcf1 = MathFunctions.GetHCF(No3, No4);
					if(hcf1 > 1)
					{
						No3 /= hcf1;
						No4 /= hcf1;
					}
					int No5 = coeff3 * coeff3 * coeff5;
					int No6 = coeff4 * coeff4;
					int hcf2 = MathFunctions.GetHCF(No5, No6);
					if(hcf2 > 1)
					{
						No5 /= hcf2;
						No6 /= hcf2;
					}
					if(No4 == 1)
						Answer = "\\frac{" +No1 + "}{" + No2 + "}x^{2}+" + No3 + "xy-\\frac{" + No5 + "}{" + No6 + "}y^{2}";
					else
						Answer = "\\frac{" +No1 + "}{" + No2 + "}x^{2}+" + "\\frac{" + No3 + "}{" + No4 + "}xy-\\frac{" + No5 + "}{" + No6 + "}y^{2}";
				}
				else if (selector == 3)
				{
					coeff1 = Random.Range(2, 10);;
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);					
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(\\frac{x}{" + coeff1 + "}-\\frac{y}{" + coeff2 + "})^{2}";
					Answer = "\\frac{x^{2}}{" + (coeff1*coeff1) + "}-\\frac{xy}{" + (coeff1 * coeff2) + "}+\\frac{y^{2}}{" + (coeff2*coeff2) + "}";
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range(2, 10);;
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);					
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(" + coeff1 + "x+" + coeff2 + "y)(" + coeff1 + "x-" + coeff2 + "y)";
					Answer = (coeff1*coeff1) + "x^{2}-" + (coeff2*coeff2) + "y^{2}";
				}
				else if (selector == 5)
				{
					coeff1 = Random.Range(2, 10);;
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					int hcf = MathFunctions.GetHCF(coeff1, coeff2);
					if(hcf > 1)
					{
						coeff1 /= hcf;
						coeff2 /= hcf;
					}
					QuestionText.text = "Solve:";
					subQuestionTEX.text = "(\\frac{" + coeff1 + "}{" + coeff2 + "}x^{2}+\\frac{" + coeff2 + "}{" + coeff1 + "}y^{2})(\\frac{" + coeff1 + "}{" + coeff2 + "}x^{2}-\\frac{" + coeff2 + "}{" + coeff1 + "}y^{2})";
					Answer = "\\frac{" + (coeff1*coeff1) + "}{" + (coeff2*coeff2) + "}x^{4}-\\frac{" + (coeff2*coeff2) + "}{" + (coeff1*coeff1) + "}y^{4}";
				}
			}
			#endregion
			#region level3
            else if (level == 3)
            {
				selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					coeff1 = 100 + coeff1;
					coeff2 = 100 - coeff2;
					QuestionText.text = "Solve:";
					subQuestionTEX.text = coeff1 + "\\times" + coeff2;
					Answer = (coeff1*coeff2).ToString();
					SingleExpression = true;
                }
                else if (selector == 2)
                {
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					coeff1 = 100 + coeff1;
					coeff2 = 100 + coeff2;
					QuestionText.text = "Solve:";
					subQuestionTEX.text = coeff1 + "\\times" + coeff2;
					Answer = (coeff1*coeff2).ToString();
					SingleExpression = true;
                }
                else if (selector == 3)
                {
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					coeff3 = Random.Range(2, 5);
					while(coeff3 == coeff2)
						coeff3 = Random.Range(2,5);
					QuestionText.text = "Solve:";
					subQuestionTEX.text = coeff3 + "(" + coeff1 + "x-\\frac{" + coeff2 + "}{" + coeff3 + "x})^{2}"; 
					Answer = (coeff1*coeff1*coeff3) + "x^{2}-\\frac{" + (coeff2*coeff2) + "}{" + coeff3 + "x^{2}}";
                }
				else if (selector == 4)
				{
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,5);
					QuestionText.text = "Express as a perfect square:";
					subQuestionTEX.text = (coeff1*coeff1) + "x^{2}-" + (coeff1*coeff2) + "xy+" + (coeff2*coeff2) + "y^{2}";
					Answer = "(" + coeff1 + "x-" + (coeff2) + "y)^{2}";
				}
				else if (selector == 5)
				{
					coeff1 = Random.Range(2, 10);
					coeff2 = 500 + coeff1;
					coeff3 = 500 - coeff1;
					QuestionText.text = "Evaluate:";
					subQuestionTEX.text = coeff2 + "\\times" + coeff3;
					Answer = (coeff2*coeff3).ToString();
				}					
            }
			#endregion
			#region level4
			else if(level == 4)
			{
				selector = GetRandomSelector(1, 5);

				if (selector == 1)
				{
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,5);
					QuestionText.text = "";
					subQuestionTEX.text = "If x+\\frac{1}{x} = " + coeff1 + " and x-\\frac{1}{x} = " + coeff2 + ". Determine x^{2}-\\frac{1}{x^{2}}";
					Answer = (coeff1*coeff2).ToString();
					SingleExpression = true;
				}
				else if (selector == 2)
				{
					coeff1 = Random.Range(2, 10);
					QuestionText.text = "";
					subQuestionTEX.text = "If x+\\frac{1}{x} = " + coeff1 + ", Determine x^{2}+\\frac{1}{x^{2}}";
					Answer = ((coeff1*coeff1)-2).ToString();
					SingleExpression = true;
				}
				else if (selector == 3)
				{
					coeff1 = Random.Range(2, 10) + 100;
					QuestionText.text = "Evaluate:";
					subQuestionTEX.text = "(" + coeff1 + ")^{2}";
					Answer = (coeff1*coeff1).ToString();
					SingleExpression = true;
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					while(coeff1 == coeff2)
						coeff2 = Random.Range(2,5);
					QuestionText.text = "Simplify:";
					subQuestionTEX.text = "(" + coeff1 + "x+" + coeff2 + "y)^{2} - (" + coeff1 + "x-" + coeff2 + "y)^{2}";
					Answer = (coeff1*coeff2*4) + "xy";
					SingleExpression = true;
				}
			}
			#endregion
			#region level5
			else if(level == 5)
			{
				subQuestionTEX.gameObject.SetActive(true);
				selector = GetRandomSelector(1, 5);
				if (selector == 1)
				{
					coeff1 = Random.Range(2, 10);
					QuestionText.text = "Simplify:";
					subQuestionTEX.text = "(x+y)^{2} + (x-y)^{2} - " + coeff1 + "(x+y)(x-y)";
					if(coeff1 == 2)
						Answer = "0";
					else if(coeff1 == 3)
						Answer = "-" + "x^{2}-" + "y^{2}";
					else
						Answer = "-" + (coeff1-2) + "x^{2}-" + (coeff1-2) + "y^{2}";
				}
			                
                if (selector == 2)
                {
					coeff1 = Random.Range(2, 8);
					coeff2 = Random.Range(2, 8);
					coeff3 = Random.Range(1, coeff1*coeff2);
					QuestionText.text = "What must be added to the middle term of below equation to make it a perfect square expression?";
					subQuestionTEX.text = (coeff1*coeff1) + "x^{2}+" + coeff3 + "xy+" + (coeff2*coeff2) + "y^{2}";
					Answer = (coeff1*coeff2*2 - coeff3) + "xy";
                }
				else if (selector == 3)
                {
					coeff1 = Random.Range(2, 8);
					coeff3 = Random.Range(2, 8);
					QuestionText.text = "What must be added to the middle term of below equation to make it a perfect square expression?";
					subQuestionTEX.text = (coeff1*coeff1) + "x^{2}+" + coeff3 + "\\frac{x}{y}+\\frac{1}{" + (coeff1*coeff1) + "y^{2}}";
					Answer = (1 - coeff3) + "\\frac{x}{y}";
                }
				else if (selector == 4)
                {
					coeff1 = Random.Range(2, 12);
					coeff2 = (coeff1*coeff1) - 2;
					QuestionText.text = "";
					subQuestionTEX.text = "If x+\\frac{1}{x}=" + coeff1 + ", Find the value of x^{4}+\\frac{1}{x^{4}}";
					Answer = ((coeff2*coeff2) - 2).ToString();
					SingleExpression = true;
                }
            }
            #endregion
			CerebroHelper.DebugLog (Answer);
            userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw>("Text");
            userAnswerLaText.text = "";
        }

        public override void numPadButtonPressed(int value)
        {
            if (ignoreTouches)
            {
                return;
            }
            if (value <= 9)
            {
				if (upflag == 1) {
					if (FracUpper) {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 4);
						userAnswerLaText.text += value.ToString ();
						userAnswerLaText.text += "}}{}";
					} else if (FracLower) {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 2);
						userAnswerLaText.text += value.ToString ();
						userAnswerLaText.text += "}}";
					} else {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
						userAnswerLaText.text += value.ToString ();
						userAnswerLaText.text += "}";
					}
				} else if (FracUpper) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					userAnswerLaText.text += value.ToString ();
					userAnswerLaText.text += "}{}";
				} else if (FracLower) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += value.ToString ();
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += value.ToString ();
				}

            }
            else if (value == 10)
            {    //,
				if (FracUpper || FracLower)
					return;
				
                if (checkLastTextFor(new string[1] { "." }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += ".";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += ".";
				}
            }
            else if (value == 11)
            {   // All Clear
//                userAnswerLaText.text = "";
                
				upflag = 0;
				FracLower = false;
				FracUpper = false;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				numPad.transform.Find ("PanelLayer").Find ("upper").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				numPad.transform.Find ("PanelLayer").Find ("lower").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

				if (userAnswerLaText.text.Length == 0) {
					return;
				}

				if (checkLastTextFor (new string[1] { "^{}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
				} else if (checkLastTextFor (new string[1] { "\\frac{}{}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
				} else if (checkLastTextFor (new string[1] { "}" })) {
					int lastPower = -1;
					int lastFrac = -1;
					lastPower = userAnswerLaText.text.LastIndexOf ("^");
					lastFrac = userAnswerLaText.text.LastIndexOf ("\\frac");
					if (lastFrac > lastPower) {
						if (checkLastTextFor (new string[1] { "}{}" })) {
							userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 4);
							userAnswerLaText.text += "}{}";
							FracUpper = true;
							numPad.transform.Find ("PanelLayer").Find ("upper").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
						} else {
							userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 2);
							userAnswerLaText.text += "}";
							FracLower = true;
							numPad.transform.Find ("PanelLayer").Find ("lower").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
						}
					} else {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 2);
						userAnswerLaText.text += "}";
						upflag = 1;
						numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
					}
				} else {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}

				// last check to remove ^{} if that's the last part of userAnswer
				if (checkLastTextFor (new string[1] { "^{}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					upflag = 0;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}

            }
            else if (value == 12)
            {   // -
                if (checkLastTextFor(new string[1] { "-" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "-";
					userAnswerLaText.text += "}";
				} else if (FracUpper) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					userAnswerLaText.text += "-";
					userAnswerLaText.text += "}{}";
				} else if (FracLower) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "-";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "-";
				}
            }
            else if (value == 13)
            {   // +
                if (checkLastTextFor(new string[1] { "+" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "+";
					userAnswerLaText.text += "}";
				} else if (FracUpper) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					userAnswerLaText.text += "+";
					userAnswerLaText.text += "}{}";
				} else if (FracLower) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "+";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "+";
				}
            }
            else if (value == 14)
            {   // ^
				
                if (upflag == 0)
                {
					if (FracUpper) {
						if (checkLastTextFor (new string[1] { "}{}" })) {
							userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
						}
						if (checkLastTextFor (new string[1] { "}" })) {
							return;
						}
						userAnswerLaText.text += "^{}}{}";
						upflag = 1;
						numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
					} else if (FracLower) {
						if (checkLastTextFor (new string[1] { "}" })) {
							userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
						}
						if (checkLastTextFor (new string[1] { "}" })) {
							return;
						}
						userAnswerLaText.text += "^{}}";
						upflag = 1;
						numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
					} else {
						if (checkLastTextFor (new string[1] { "^{}" })) {
							userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
						}
						if (checkLastTextFor (new string[1] { "}" })) {
							return;
						}
						userAnswerLaText.text += "^{}";
						upflag = 1;
						numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
					}
                }
                else if(upflag==1)
                {
                    if (checkLastTextFor(new string[1] { "}" }))
                    {
                        userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    }
                    userAnswerLaText.text += "}";
                    upflag = 0;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
                }
            }
            else if (value == 15)
            {   // x
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				if (checkLastTextFor(new string[1] { "x" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (FracUpper) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					userAnswerLaText.text += "x";
					userAnswerLaText.text += "}{}";
				} else if (FracLower) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "x";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "x";
				}
            }
            else if (value == 16)
            {   // y
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
                if (checkLastTextFor(new string[1] { "y" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (FracUpper) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					userAnswerLaText.text += "y";
					userAnswerLaText.text += "}{}";
				} else if (FracLower) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "y";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "y";
				}
            }
            else if (value == 17)
            {   // z
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
                if (checkLastTextFor(new string[1] { "z" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (FracUpper) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					userAnswerLaText.text += "z";
					userAnswerLaText.text += "}{}";
				} else if (FracLower) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "z";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "z";
				}
            }
            else if (value == 18)
            {   // (
				if (FracUpper || FracLower)
					return;
				
                if (checkLastTextFor(new string[1] { "(" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "(";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "(";
				}
            }
            else if (value == 19)
            {   // (
				if (FracUpper || FracLower)
					return;
				
                if (checkLastTextFor(new string[1] { ")" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += ")";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += ")";
				}
            }
            else if (value == 20)
			{   // FracUpper

				upflag = 0;
				FracLower = false;
				numPad.transform.Find ("PanelLayer").Find ("lower").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				FracUpper = !FracUpper;
				if (FracUpper) {
					if (checkLastTextFor (new string[1] { "\\frac{}{}" })) {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 9);
					}
					if (checkLastTextFor (new string[1] { "}" })) {
						return;
					}
					userAnswerLaText.text += "\\frac{}{}";
					numPad.transform.Find ("PanelLayer").Find ("upper").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				} else {
					numPad.transform.Find ("PanelLayer").Find ("upper").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}
			}
            else if (value == 21)
			{   // FracLower

				upflag = 0;
				FracUpper = false;
				numPad.transform.Find ("PanelLayer").Find ("upper").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				FracLower = !FracLower;
				if (FracLower) {
//					if (checkLastTextFor (new string[1] { "{}" })) {
//						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 2);
//					}
//					if (checkLastTextFor (new string[1] { "}" })) {
//						return;
//					}
//					userAnswerLaText.text += "{}";
					numPad.transform.Find ("PanelLayer").Find ("lower").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				} else {
					numPad.transform.Find ("PanelLayer").Find ("lower").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}
			}
        }
    }
}

