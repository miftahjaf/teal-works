using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
    public class FundamentalConceptsOfAlgebra : BaseAssessment
    {

        public Text subQuestionText;
        public TEXDraw subQuestionTEX;
       // public TEXDraw QuestionTEX;
      //  public TEXDraw tempQuestionTEX;
        public GameObject MCQ;
        public GameObject ThreeChoice;
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
        private TEXDraw expressionTEX1;
        private TEXDraw expressionTEX2;
        private TEXDraw expressionTEX3;
        private string expression2;
        private string expression3;
        private string expression4;
        private string expression5;
        private int upflag=0;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "FCA06", "S01", "A01");

            scorestreaklvls = new int[9];
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
            upflag = 0;
            if (ignoreTouches || userAnswerLaText.text == "")
            {
                return;
            }

			upflag = 0;
			numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

            int increment = 0;
            //var correct = false;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            var correct = true;
            CerebroHelper.DebugLog("!" + userAnswerLaText.text + "!");
            CerebroHelper.DebugLog("*" + Answer + "*");
            questionsAttempted++;
            updateQuestionsAttempted();
			if (level == 1 && selector != 1)
            {
                string[] userAnswerSplits = userAnswerLaText.text.Split(new string[] { "," }, System.StringSplitOptions.None);

                List<string> userAnswers = new List<string>();

                for (var i = 0; i < userAnswerSplits.Length; i++)
                {
                    string userAnswer = "";
                    userAnswer = userAnswerSplits[i];
                    userAnswers.Add(userAnswer);
                    /*if (userAnswerSplits[i])
                    {
                        userAnswer = userAnswerSplits[i];
                        userAnswers.Add(userAnswer);
                    }
                    else
                    {
                        correct = false;
                        break;
                    }*/
                }

                if (checkArrayValues(Answerarray, userAnswers.ToArray()))
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
                //var answerSplits = Answer.Split(new string[] { "." }, System.StringSplitOptions.None);
                //var userAnswerSplits = userAnswerLaText.text.Split(new string[] { "." }, System.StringSplitOptions.None);
                bool checkingMCQ = false;
                bool checkingThreeChoice = false;
                if (MCQ.activeSelf)
                {
                    checkingMCQ = true;
                }
                if (ThreeChoice.activeSelf)
                {
                    checkingThreeChoice = true;
                }
                if (checkingMCQ)
                {
                    if (userAnswerLaText.text == Answer)
                    {
                        correct = true;
                    }
                    else
                    {
                        correct = false;
						AnimateMCQOptionCorrect(Answer);
                    }
                }
                else if (checkingThreeChoice)
                {
                    if (userAnswerLaText.text == Answer)
                    {
                        correct = true;
                    }
                    else
                    {
                        correct = false;
						AnimateThreeChoiceOptionCorrect(Answer);
                    }
                }
                else
                {
                    float answer = 0;
                    float userAnswer = 0;
                    bool directCheck = false;
                    if (level==3 && selector==2 || AnswerInt != 9999)
                    {
                        if (AnswerInt != 9999)
                            Answer = AnswerInt.ToString();
                        var correctAnswers = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
                        var userAnswers = userAnswerLaText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
						correct = MathFunctions.checkFractions(userAnswers, correctAnswers);
                    }
                    else if(flag==1)
                    {
                        
                        string currentuserans=userAnswerLaText.text;
                        int coeff;
						bool check = true;
                        if (int.TryParse(Answerarray[Answerarray.Length-1], out coeff))
                        {
                            for (int i = 0; i < Answerarray.Length; i++)
                            {
                                if (currentuserans.Contains(Answerarray[i]))
                                {
                                    currentuserans = currentuserans.Remove(currentuserans.IndexOf(Answerarray[i]), Answerarray[i].Length);
                                }
                            }
							int currAns = -1;
							if (int.TryParse (currentuserans, out currAns)) {
								if (coeff == currAns) {
									currentuserans = "";
								}
							}
                        }
                        else
                        {
                            for (int i = 0; i < Answerarray.Length; i++)
                            {
								if (currentuserans == "") {
									correct = false;
									check = false;
									break;
								}
								if (currentuserans.Contains (Answerarray [i])) {
									currentuserans = currentuserans.Remove (currentuserans.IndexOf (Answerarray [i]), Answerarray [i].Length);
								} else {
									correct = false;
									check = false;
									break;
								}
                            }
                        }
						if (check) {
							if (currentuserans == "")
								correct = true;
							else
								correct = false;
						}
                    }
                    //else if(AnswerInt!=9999)
                    //{
                    //    Answer = AnswerInt.ToString();
                    //    //int useranswerint = int.Parse(userAnswerLaText.text);
                    //    var correctAnswers = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
                    //    var userAnswers = userAnswerLaText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                    //    correct = MathFunctions.checkFractions(userAnswers, correctAnswers);
                    //    //if (AnswerInt == useranswerint)
                    //    //    correct = true;
                    //    //else
                    //    //    correct = false;
                    //}
                    else
                    {
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
                    increment = 5;
                }
                else if (Queslevel == 4)
                {
                    increment = 5;
                }
                else if (Queslevel == 5)
                {
                    increment = 10;
                }
                else if (Queslevel == 6)
                {
                    increment = 10;
                }
                else if (Queslevel == 7)
                {
                    increment = 10;
                }
                else if (Queslevel == 8)
                {
                    increment = 15;
                }
                else if (Queslevel == 9)
                {
                    increment = 15;
                }
                else if (Queslevel == 10)
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

        public void MCQOptionClicked(int value)
        {
            if (ignoreTouches)
            {
                return;
            }
            AnimateMCQOption(value);
            userAnswerLaText = MCQ.transform.Find("Option" + value.ToString()).Find("Text").GetComponent<TEXDraw>();
            answerButton = MCQ.transform.Find("Option" + value.ToString()).GetComponent<Button>();
            SubmitClick();
        }
        public void ThreeChoiceOptionClicked(int value)
        {
            if (ignoreTouches)
            {
                return;
            }
            AnimateThreeChoiceOption(value);
            userAnswerLaText = ThreeChoice.transform.Find("Option" + value.ToString()).Find("Text").GetComponent<TEXDraw>();
            answerButton = ThreeChoice.transform.Find("Option" + value.ToString()).GetComponent<Button>();
            SubmitClick();
        }
        IEnumerator AnimateMCQOption(int value)
        {
            var GO = MCQ.transform.Find("Option" + value.ToString()).gameObject;
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1.2f, 1.2f, 1), false));
            yield return new WaitForSeconds(0.2f);
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1, 1, 1), false));
        }
		void AnimateMCQOptionCorrect(string ans)
		{
			if (isRevisitedQuestion) {
				return;
			}
			for (int i = 1; i <= 2; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
		}
        IEnumerator AnimateThreeChoiceOption(int value)
        {
            var GO = ThreeChoice.transform.Find("Option" + value.ToString()).gameObject;
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1.2f, 1.2f, 1), false));
            yield return new WaitForSeconds(0.2f);
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1, 1, 1), false));
        }
		void AnimateThreeChoiceOptionCorrect(string ans)
		{
			if (isRevisitedQuestion) {
				return;
			}
			int index = -1;
			for (int i = 1; i <= 3; i++) {
				if (ThreeChoice.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					//index = i;
					ThreeChoice.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
			if (index != -1) {
				var GO = ThreeChoice.transform.Find ("Option" + index.ToString ()).gameObject;
				Go.to (ThreeChoice.gameObject.transform, 0.5f, new GoTweenConfig ().shake (new Vector3 (0, 0, 20), GoShakeType.Eulers));
			}
		}
        protected override IEnumerator ShowWrongAnimation()
        {
            userAnswerLaText.color = MaterialColor.red800;
			Go.to(userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
            yield return new WaitForSeconds(0.5f);
			if (level == 1 && selector != 1)
            {
                if (isRevisitedQuestion)
                {
                    userAnswerLaText.text = "";
                    userAnswerLaText.color = MaterialColor.textDark;
                    ignoreTouches = false;
                }
                else
                {
                    string str = "";
                    for (int i = 0; i < Answerarray.Length; i++)
                    {
                        if (i == (Answerarray.Length - 1))
                            str += Answerarray[i];
                        else
                            str += Answerarray[i] + ", ";
                    }
                    userAnswerLaText.text = " " + str + " ";
                    userAnswerLaText.color = MaterialColor.green800;
                }
            }
            else
            {
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
            //QuestionTEX.gameObject.SetActive(false);
            //tempQuestionTEX.gameObject.SetActive(false);
			QuestionLatext.gameObject.SetActive(false);
            QuestionText.gameObject.SetActive(true);
            MCQ.gameObject.SetActive(false);
            ThreeChoice.gameObject.SetActive(false);
            numPad.SetActive(true);
			for (int i = 1; i < 4; i++) {
				ThreeChoice.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}
			for (int i = 1; i < 3; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}

			QuestionLatext.text = null;
           QuestionText.text =null;
           // tempQuestionTEX.text = null;
            subQuestionTEX.text = null;
           // QuestionTEX.text = null;

            AnswerInt = 9999;
            flag = 0;

            if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
            
            //level = 10;

            #region level1
			if (level == 1)
			{
				subQuestionTEX.gameObject.SetActive(true);
				selector = GetRandomSelector(1,5);

				if (selector == 1 || selector == 2)
                {
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					coeff3 = Random.Range(2, 10);
					coeff4 = Random.Range(2, 10);
					pow1 = Random.Range(2,9);
					pow2 = Random.Range(2, 10);

					//splitans = 2;
					//randact = 2;

					//int tempAns;
					expression3 = "";
					if (selector == 1)
                    {
                        expression3 = coeff1 + "\\xalgebra^{" + pow1 + "} + " + coeff4; ;
						subQuestionTEX.text = expression3;
                      QuestionText.text = "Constant in this equation:";
                        AnswerInt = coeff4;
                        //tempAns = coeff4;
                        //Answer = tempAns.ToString();
                    }

					else 
                    {
                        expression3 = "";
                        expression3 = coeff1 + "\\xalgebra^{" + pow1 + "}\\yalgebra^{" + pow2 + "} + "+coeff4;
						subQuestionTEX.text = expression3;
						QuestionText.text = "Variables in this equation:";
						flag = 1;
						string[] answer = new string[2];
						answer[0] = "\\xalgebra";
						answer[1] = "\\yalgebra";
						Answerarray = answer;
						//Answer = "xy";
                    }
				}
				else
				{
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					coeff3 = Random.Range(2, 10);
					pow1 = Random.Range(2, 5);
					pow2 = Random.Range(2,5);
					expression1 = "";
					expression2 = "";
					expression3 = "";
					expression5 = "";
					expression4 = "";

	                if (selector == 3)
	                {
						QuestionText.text = "Separate the terms of the expression :";
						expression1 = coeff1 + "\\xalgebra";
	                    CerebroHelper.DebugLog(expression1);
						expression2 = "\\yalgebra";
	                    CerebroHelper.DebugLog(expression2);
	                    expression3 = expression1 + "-"+expression2;
						subQuestionTEX.text = expression3;
	                    string[] answer = new string[2];
	                    answer[0] = expression1;
	                    answer[1] = "-" + expression2;
	                    Answerarray = answer;    
	                }
	                if (selector == 4)
	                {
						QuestionText.text = "Separate the terms of the expression :";
	                    expression1 = coeff1 + "\\xalgebra^{" + pow1+"}";
	                    expression2 = coeff2 + "\\yalgebra^{" + pow2+"}";
						expression4 = coeff1 * coeff3 + "\\xalgebra";
						expression5 = coeff2 * coeff3 + "\\yalgebra";
						expression3 = expression1 + "-" + expression2 + "+" + coeff3 + "\\xalgebra\\yalgebra(\\frac{" + coeff1 + "}{\\yalgebra} + \\frac{" + coeff2 + "}{\\xalgebra})";
						subQuestionTEX.text = expression3;
	                    string[] answer = new string[4];
						answer[0] = expression1;
						answer[1] = "-"+ expression2;
						answer[2] = expression4;
						answer[3] = expression5;
	                    Answerarray = answer;
	              	}
	                if (selector == 5)
					{
						QuestionText.text = "Separate the terms of the expression :";
	                    expression1 = coeff1.ToString();
						expression2 = coeff2 + "\\xalgebra";
	                    expression3 = coeff3 + "\\xalgebra^{"+pow1+"}";
	                    expression4 = expression1 + "+" + expression2 + "+" +expression3;
						subQuestionTEX.text = expression4;
	                    string[] answer = new string[3];
						answer[0] = expression1;
						answer[1] = expression2;
						answer[2] = expression3;
	                    Answerarray = answer;
	             	}
				}
            }
            #endregion
            #region level2
            if (level == 2)
            {
				subQuestionTEX.gameObject.SetActive(true);
				coeff1 = Random.Range(2,10);
				coeff2 = Random.Range(0,10);
				coeff3 = Random.Range(2,10);
                pow1 = Random.Range(-10, 10);
                pow2 = Random.Range(-10, 10);
                pow3 = Random.Range(-10, 10);
              QuestionText.text = "Write the number of terms in the following expression :";
                selector = GetRandomSelector(1, 6);
                //selector = 6;
                if (selector == 1)
                {
					coeff1 *= Random.Range(0,2);
					expression3 = "\\xalgebra+" + coeff1;
					subQuestionTEX.text = expression3;
					if (coeff1 == 0)
                    	AnswerInt = 1;
					else 
						AnswerInt = 2;
                }
                if (selector == 2)
                {
					expression3 = coeff1 + "\\xalgebra-\\yalgebra";
                    AnswerInt = 2;
					subQuestionTEX.text = expression3;
                }
           
                if (selector == 3)
                {
					expression3 = coeff1 + "\\xalgebra+" + coeff2 + "-\\yalgebra";
					subQuestionTEX.text = expression3;
                    if (coeff2 == 0)
                        AnswerInt = 2;
                    else
                        AnswerInt = 3;
                }
                if (selector == 4)
                {
					expression3 = "\\xalgebra^{" + pow1 + "}+\\yalgebra^{" + pow2 + "}+\\zalgebra^{" + pow3 + "}-\\xalgebra\\yalgebra\\zalgebra";
					subQuestionTEX.text = expression3;
                    AnswerInt = 4;
                }
                if (selector == 5)
                {
					if (coeff2 != 1)
						expression3 = "(" + coeff1 + "\\xalgebra-" + coeff2 + "\\yalgebra)/\\zalgebra";
					else
						expression3 = "(" + coeff1 + "\\xalgebra-\\yalgebra)/\\zalgebra";
					subQuestionTEX.text = expression3;
                    if (coeff2 == 0)
                        AnswerInt = 1;
                    else
                        AnswerInt = 2;
                }
            }
            #endregion
            #region level3
            if (level == 3)
            {
				subQuestionTEX.gameObject.SetActive(true);
                coeff1 = Random.Range(-10, 10);
				while (coeff1 == 0 || coeff1 == 1 || coeff1 ==-1)
                    coeff1 = Random.Range(-10, 10);
                coeff2 = Random.Range(2, 10);
                pow1 = Random.Range(2, 10);
                pow2 = Random.Range(2, 10);
                pow3 = Random.Range(2, 10);
              QuestionText.text = "Write the numerical co-efficient of :";
                selector = GetRandomSelector(1, 4);
                //selector = 2;
                if (selector == 1)
                {
					expression3 = coeff1 + "^0\\xalgebra\\yalgebra^2\\zalgebra";
					subQuestionTEX.text = expression3;
					if (coeff1 > 0)
						AnswerInt = 1;
					else
						AnswerInt = -1;
                }
                if (selector == 2)
                {
					while (coeff1 == coeff2 || MathFunctions.GetHCF (coeff1, coeff2) != 1)
						coeff2 = Random.Range(2, 10);
                    expression3 = "\\frac{" + coeff1 + "}{" + coeff2 + "}" + "\\xalgebra^{" + pow1 + "}\\yalgebra^{" + pow2 + "}";
					subQuestionTEX.text = expression3;    
                    Answer =  coeff1 + "/" + coeff2;
                }
                if (selector == 3)
                {
                    expression3 = coeff2 + "\\xalgebra^{" + pow1 + "}\\yalgebra^{" + pow2 + "}\\zalgebra^{" + pow3 + "}";
					subQuestionTEX.text = expression3;
                    AnswerInt = coeff2;
                }
            }
            #endregion
            #region level4
            if (level == 4)
            {
                subQuestionTEX.gameObject.SetActive(true);
				QuestionLatext.gameObject.SetActive(true);
                coeff1 = Random.Range(-10, 10);
				while (coeff1 == 0 || coeff1 == 1 || coeff1 == -1)
                    coeff1 = Random.Range(-10, 10);
                pow1 = Random.Range(2,5);
                pow2 = Random.Range(2,5);
              QuestionText.text = "Write the co-efficient of :";
                selector = GetRandomSelector(1, 4);

                //selector = 3;

                if (selector == 1)
                {
					subQuestionTEX.text = "\\xalgebra^{ " + pow1 + "}\\yalgebra^{ " + pow2 + "} in";
                    expression3 = coeff1 + "\\xalgebra^{" + pow1 + "}\\yalgebra^{" + pow2 + "}";
					subQuestionTEX.text +="\n\n"+ expression3;
                    AnswerInt = coeff1;
                }
                if (selector == 2)
                {
					subQuestionTEX.text = "\\xalgebra in";
					expression3 = coeff1 + "\\xalgebra";
					subQuestionTEX.text +="\n\n"+ expression3;
                    AnswerInt = coeff1;
                }
                if (selector == 3)
                {
					subQuestionTEX.text = "\\xalgebra in";
					expression3 = coeff1 + "\\xalgebra\\yalgebra^2";
					subQuestionTEX.text +="\n\n"+ expression3;
                    flag = 1;
                    string[] answer = new string[2];
                    answer[1] = "\\yalgebra^{2}";
                    answer[0] = coeff1.ToString();
                    Answerarray = answer;
                    Answer = coeff1+"\\yalgebra^{2}";
                }
            }
            #endregion
            #region level5
            if (level == 5)
            {
				subQuestionTEX.gameObject.SetActive(true);

				coeff1 = Random.Range(2, 10);
                coeff2 = Random.Range(2, 10);
                coeff3 = Random.Range(2, 10);
                coeff4 = Random.Range(2, 10);
                expression3 = null;
               QuestionText.text = "Write the exponential form of :";

                selector = GetRandomSelector(1, 4);
                
                //selector = 2;

                if (selector == 1)
                {
					expression3 = "\\xalgebra \\times \\xalgebra \\times \\xalgebra \\times \\yalgebra \\times \\yalgebra \\times \\yalgebra \\times \\yalgebra";
					subQuestionTEX.text = expression3;
                    flag = 1;
                    string[] answer = new string[2];
                    answer[0] = "\\xalgebra^{3}";
                    answer[1] = "\\yalgebra^{4}";
                    Answerarray = answer;
                    Answer = "\\xalgebra^{3}"+"\\yalgebra^{4}";
                }
                if (selector == 2)
                {
                    for (int i = 0; i < coeff1; i++)
                    {
						expression3 = expression3 + "\\xalgebra \\times ";
                    }
                    for (int i = 0; i < coeff1; i++)
                    {
                        if(i==coeff1-1)
							expression3 = expression3 + "\\yalgebra";
                        else
							expression3 = expression3 + "\\yalgebra \\times ";
                    }
                    subQuestionTEX.text = expression3;
                    flag = 1;
                    string[] answer = new string[2];
                    answer[0] = "\\xalgebra^{" + coeff1 + "}";
                    answer[1] = "\\yalgebra^{" + coeff1 + "}";
                    Answerarray = answer;
                    Answer = "\\xalgebra^{"+coeff1+"}\\yalgebra^{"+coeff1+"}";
                }
                if (selector == 3)
                {
                    //coeff1 = Random.Range(2, 7);
					expression3 = coeff1 + " \\times " + coeff2 + " \\times ";
                    for (int i = 0; i < coeff3; i++)
                    {
						expression3 = expression3 + "\\xalgebra \\times ";
                    }
					expression3 = expression3 + coeff1 + " \\times " + coeff2 + " \\times ";
                    for (int i = 0; i < coeff4; i++)
                    {
                        if (i == (coeff4 - 1))
							expression3 = expression3 + "\\yalgebra";
                        else
							expression3 = expression3 + "\\yalgebra \\times ";
                    }
                    subQuestionTEX.text = expression3;
                    int temp;
                    temp = coeff1 * coeff2 * coeff1 * coeff2;
                    flag = 1;
                    string[] answer = new string[3];
                    answer[0] = "\\xalgebra^{" + coeff3 + "}";
                    answer[1] = "\\yalgebra^{" + coeff4 + "}";
                    answer[2] = temp.ToString();
                    Answerarray = answer;
                    Answer = temp + "\\xalgebra^{" + coeff3 + "}\\yalgebra^{" + coeff4 + "}";
                }
            }
            #endregion
            #region level6
            if (level == 6)
            {
				subQuestionTEX.gameObject.SetActive (true);
				QuestionLatext.gameObject.SetActive(true);
                //tempQuestionTEX.gameObject.SetActive(true);
                QuestionText.gameObject.SetActive(false);

                coeff1 = Random.Range(-10, 10);
				while(coeff1 == 1 || coeff1 == 0 || coeff1 == -1)
                    coeff1 = Random.Range(-10, 10);
                pow1 = Random.Range(2, 10);
                pow2 = Random.Range(2, 10);
                pow3 = Random.Range(2, 10);
                expression1 = coeff1 + "\\xalgebra^{" + pow1 + "}\\yalgebra^{" + pow2 + "}\\zalgebra^{" + pow3 + "}";
				QuestionLatext.text = expression1;
				subQuestionTEX.text =  "For the given term, write the co-efficient of :";

                selector = GetRandomSelector(1, 5);

                if (selector == 1)
                {
                    expression3 = coeff1 + "\\zalgebra^{" + pow3 + "}";
					subQuestionTEX.text +="\n\n"+ expression3;
                    flag = 1;
                    string[] answer = new string[2];
                    answer[0] = "\\xalgebra^{" + pow1 + "}";
                    answer[1] = "\\yalgebra^{" + pow2 + "}";
                    Answerarray = answer;
                    Answer = "\\xalgebra^{"+pow1+"}\\yalgebra^{" + pow2 + "}";
                    
                }
                if (selector == 2)
                {
                    expression3 = "\\xalgebra^{" + pow1 + "}";
					subQuestionTEX.text +="\n\n"+ expression3;
                    flag = 1;
                    string[] answer = new string[3];
                    answer[0] = "\\yalgebra^{" + pow2 + "}";
                    answer[1] = "\\zalgebra^{" + pow3 + "}";
                    answer[2] = coeff1.ToString();
                    Answerarray = answer;
                    Answer = coeff1+"\\yalgebra^{" + pow2 + "}\\zalgebra^{"+pow3+"}";
                    CerebroHelper.DebugLog("!" + Answer + "!");
                }
                if (selector == 3)
                {
                    expression3 = "\\yalgebra^{" + pow2 + "}\\zalgebra^{" + pow3 + "}";
					subQuestionTEX.text +="\n\n"+ expression3;
                    flag = 1;
                    string[] answer = new string[2];
                    answer[0] = "\\xalgebra^{" + pow1 + "}";
                    answer[1] = coeff1.ToString();
                    Answerarray = answer;
                    Answer = coeff1+"\\xalgebra^{" + pow1 + "}";
                    
                }
                if (selector == 4)
                {
                    expression3 = coeff1 + "\\xalgebra^{" + pow1 + "}\\zalgebra^{" + pow3 + "}";
					subQuestionTEX.text +="\n\n"+ expression3;
                    Answer = "\\yalgebra^{"+pow2+"}";
                    
                }
            }
            #endregion
            #region level7
            if (level == 7)
            {
				subQuestionTEX.gameObject.SetActive(true);
                GeneralButton.gameObject.SetActive(false);
                ThreeChoice.SetActive(true);
                numPad.SetActive(false);
                coeff1 = Random.Range(2, 10);
                QuestionText.text = "Is the following a monomial, binomial or trinomial?";
                selector = GetRandomSelector(1, 5);
                if (selector == 1)
                {
					expression3 = coeff1 + "\\xalgebra/\\yalgebra";
					subQuestionTEX.text = expression3;
                    Answer = "Monomial";
                    ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "Monomial";
                    ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "Binomial";
                    ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<TEXDraw>().text = "Trinomial";

                }
                if (selector == 2)
                {
					expression3 = "\\xalgebra^3 + \\xalgebra - " + coeff1;
					subQuestionTEX.text = expression3;
                    Answer = "Trinomial";
                    ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "Monomial";
                    ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "Binomial";
                    ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<TEXDraw>().text = "Trinomial";
                }
                if (selector == 3)
                {
					expression3 = coeff1 + "\\xalgebra - " + coeff1 + "\\yalgebra";
					subQuestionTEX.text = expression3;
                    Answer = "Binomial";
                    ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "Monomial";
                    ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "Binomial";
                    ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<TEXDraw>().text = "Trinomial";
                }
                if (selector == 4)
                {
					expression3 = coeff1 + "\\xalgebra";
					subQuestionTEX.text = expression3;
                    Answer = "Monomial";
                    ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "Monomial";
                    ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "Binomial";
                    ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<TEXDraw>().text = "Trinomial";
                }
               // ThreeChoice.gameObject.SetActive(false);
            }
            #endregion
            #region level8
            if (level == 8)
            {
				selector = GetRandomSelector(1, 6);
				if (selector == 1)
				{
	                subQuestionTEX.gameObject.SetActive(true);
	                coeff1 = Random.Range(2, 10);
	                pow1 = Random.Range(2, 10);
	              QuestionText.text = "What is the degree of the polynomial?";

					randact = Random.Range(1, 4);

	                //selector = 2;

					if (randact == 1)
	                {
						expression3 = "\\xalgebra + " + coeff1;
	                    subQuestionTEX.text = expression3;
	                    AnswerInt = 1;
	                }
					if (randact == 2)
	                {
	                    expression3 = coeff1.ToString()+" - "+coeff1 + "\\xalgebra^{" + pow1 + "}";
	                    subQuestionTEX.text = expression3;
	                    AnswerInt = pow1;
	                }
					if (randact == 3)
	                {
	                    expression3 = coeff1 + "\\xalgebra^{"+pow1+"}";
	                    subQuestionTEX.text = expression3;
	                    AnswerInt = pow1;
	                }
				}
				else
				{
		            subQuestionText.gameObject.SetActive (true);
		            coeff1 = Random.Range(2, 10);
		            coeff2 = Random.Range(2, 10);
		            coeff3 = Random.Range(2, 10);
		            float rand = Random.Range(-10, 10);
		          QuestionText.text = "Write the following statement in algebraic form:";
		            //selector = 5;
		            if (selector == 2)
		            {
						subQuestionTEX.gameObject.SetActive(true);
						subQuestionText.gameObject.SetActive(false);
						expression3 = coeff1 + " less than thrice of \\xalgebra gives " + coeff2;
		                subQuestionTEX.text = expression3;
						Answer = "3\\xalgebra-" + coeff1 + "=" + coeff2;
		            }
		            if (selector == 3)
		            {
					     subQuestionTEX.gameObject.SetActive(true);
						subQuestionText.gameObject.SetActive(false);
						expression3 = "Difference of \\xalgebra and " + coeff1 + " subtracted from " + coeff1 + "\\yalgebra is equal to " + coeff2;
		                subQuestionTEX.text = expression3;
		                Answer = coeff1 + "\\yalgebra-(\\xalgebra-"+coeff1+")="+coeff2;
		            }
		            if (selector == 4)
		            {
						subQuestionTEX.gameObject.SetActive(true);
						subQuestionText.gameObject.SetActive(false);
						expression3 = coeff1 + "\\xalgebra decreased by " + coeff1 + "\\yalgebra is equal to -" + coeff1 + "\\zalgebra";
		                subQuestionTEX.text = expression3;
						Answer = coeff1 + "\\yalgebra-" + coeff1 + "\\xalgebra=-" + coeff1 + "\\zalgebra";
		            }
		            if (selector == 5)
		            {
						while (MathFunctions.GetHCF(coeff1,coeff2) != 1)
							coeff1 = Random.Range(2, 10);
						subQuestionTEX.gameObject.SetActive(true);
		                subQuestionText.gameObject.SetActive(false);
						expression3 = "\\frac{" + coeff1 + "}{" + coeff2 + "} of the difference of " + coeff3 + "\\xalgebra and 7";
						subQuestionTEX.text = expression3;
						Answer = coeff1 + "(" + coeff3 + "\\xalgebra-7)/" + coeff2;
					}
				}
            } 
            #endregion
            #region level9
            if (level == 9)
            {
			  subQuestionTEX.gameObject.SetActive(true);
              QuestionText.text = "State True or False:";
                selector = GetRandomSelector(1, 5);
				GeneralButton.gameObject.SetActive(false);
                MCQ.SetActive(true);
                numPad.SetActive(false);
               // GeneralButton.enabled = false;
                //selector = 1;
                if (selector == 1)
                {

					expression3 = "\\xalgebra\\yalgebra is a monomial";
					subQuestionTEX.text = expression3;
                    Answer = "True";
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "True";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "False";
                    CerebroHelper.DebugLog(Answer);
                }
                if (selector == 2)
                {
					expression3 = "\\xalgebra\\yalgebra\\zalgebra and -\\xalgebra\\yalgebra\\zalgebra are like terms";
					subQuestionTEX.text = expression3;
                    Answer = "True";
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "True";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "False";
                }
                if (selector == 3)
                {
					expression3 = "\\frac{2}{3}\\xalgebra\\yalgebra^2, -\\frac{1}{3}\\xalgebra\\yalgebra^2 and \\frac{2}{3}\\xalgebra^2" + "\\yalgebra are like terms";
					subQuestionTEX.text = expression3;
                    Answer = "False";
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "True";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "False";
                }
                if (selector == 4)
                {
					expression3 = "\\xalgebra\\yalgebra\\zalgebra and \\zalgebra\\yalgebra\\xalgebra are like terms";
					subQuestionTEX.text = expression3;
                    Answer = "True";
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "True";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "False";
                }

            }
            #endregion
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
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += value.ToString ();
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += value.ToString ();
				}

            }
            else if (value == 10)
            {    //,
                if (checkLastTextFor(new string[1] { "," }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += ",";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += ",";
				}
            }
            else if (value == 11)
            {   // All Clear
//                userAnswerLaText.text = "";
                
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

				if (userAnswerLaText.text.Length == 0) {
					return;
				}

				if (checkLastTextFor (new string[1] { "^{}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
				} else if (checkLastTextFor (new string[1] { "}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 2);
					userAnswerLaText.text += "}";
					upflag = 1;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				} else if(checkLastTextFor (new string[3] { "\\xalgebra","\\yalgebra","\\zalgebra" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 9);
				}
				else {
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
				} else {
					userAnswerLaText.text += "+";
				}
            }
            else if (value == 14)
            {   // ^
                if (upflag == 0)
                {
					if (checkLastTextFor(new string[1] { "^{}" }))
                    {
                        userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 3);
                    }
					if (checkLastTextFor (new string[1] { "}" })) {
						return;
					}
					userAnswerLaText.text += "^{}";
                    upflag = 1;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
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
				if (checkLastTextFor(new string[1] { "\\xalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\xalgebra";
            }
            else if (value == 16)
            {   // y
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
                if (checkLastTextFor(new string[1] { "\\yalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\yalgebra";
            }
            else if (value == 17)
            {   // z
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
                if (checkLastTextFor(new string[1] { "\\zalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\zalgebra";
            }
            else if (value == 18)
            {   // (
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
            {   // /
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "/";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "/";
				}
            }
            else if (value == 21)
            {   // =
                if (checkLastTextFor(new string[1] { "=" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "=";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "=";
				}
            }
        }
    }
}

