using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
    public class FundamentalConceptsOfAlgebra7 : BaseAssessment
    {

        
        public Text subQuestionText;
        public TEXDraw subQuestionTEX;
       // public TEXDraw QuestionTEX;
      //  public TEXDraw tempQuestionTEX;
        public GameObject MCQ;
        public GameObject ThreeChoice;
        private string Answer;
        private string[] Answerarray;
        // private TEXDraw Answer2;
        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
        private int coeff6;
        private int pow1, pow2, pow3;
        private int randact, splitans;
        private int[] coeff = new int[9];
        private float x, y, z, a, b, c;
        string[] exp = new string[9];
        int[] end = new int[15];
        private int upflag = 0;
        int noOfTerms = 0;

        private string expression1;
        private TEXDraw expressionTEX1;
        private TEXDraw expressionTEX2;
        private TEXDraw expressionTEX3;
        private string expression2;
        private string expression3;
        private string expression4;
        private string expression5;
        

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "FCA07", "S01", "A01");

            scorestreaklvls = new int[6];
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


        public string[] SeparateTerms(char[] userAns)
        {

            string[] userTerms = new string[20];
            for (int k = 0; k < 20; k++)
                userTerms[k] = "";

            int j = 0;
            for (int i = 0; i < userAns.Length; i++)
            {                           // seperates terms with an operator
                if (userAns[i] == '+' && i == 0)
                    continue;

                userTerms[j] = userTerms[j] + userAns[i].ToString();
                if (i != (userAns.Length - 1))
                    if (userAns[i] != '{' && (userAns[i + 1] == '+' || userAns[i + 1] == '-'))
                    {
                        userTerms[j] = userTerms[j] + "\0";
                        j++;
                    }
            }
            noOfTerms = j + 1;
            return userTerms;

        }

        public float[] SeparateCoeff(string[] terms)
        {
            float[] coeffs = new float[15];
            string c = "";
            char[] temp;
            bool deci = false, slash = false;
            for (int i = 0; i <= noOfTerms; i++)
            {                  //to jump from term to term
                temp = terms[i].ToCharArray();
                //CerebroHelper.DebugLog(temp);
                for (int j = 0; j < temp.Length; j++)
                {                           //to traverse through terms to get their coeff
                    if (temp[j] == '+')
                        continue;
                    else if (temp[j] == '-')
                        c = c + temp[j].ToString();
                    else if (temp[j] == '/')
                    {
                        c = c + temp[j].ToString();
                        slash = true;
                    }
                    else if (temp[j] == '.')
                    {
                        if (temp[j] == 0)
                            c = c + "0";
                        c = c + temp[j].ToString();
                        deci = true;
                    }
                    else if (temp[j] >= '0' && temp[j] <= '9')
                        c = c + temp[j].ToString();
                    else {
                        end[i] = j;
                        break;
                    }
                }
                //CerebroHelper.DebugLog(c);
                if (slash)
                {
                    string[] num = c.Split(new string[] { "/" }, System.StringSplitOptions.None);
                    int nume = int.Parse(num[0]);
                    bool neg = false;
                    if(num[1] == "-")
                    {
                        num[1].Remove(0,1);
                        neg = true;
                    }
                    
                    int denome = int.Parse(num[1]);
                    CerebroHelper.DebugLog(num[1]);
                    if(neg)
                    {
                        denome *= -1;
                    }
                    if (denome == 0)
                    {
                        terms[i] = "\0";
                        break;
                    }
                    int tempNum = (int)((nume * 100) / denome);
                    coeffs[i] = tempNum / 100f;
                    CerebroHelper.DebugLog("FRACTION VALUE = " + coeffs[i]);

                    slash = false;
                }
                else if (deci)
                {

                    float tempNum = float.Parse(c);
                    int tempn = (int)(tempNum * 100);
                    coeffs[i] = tempn / 100f;
                    deci = false;
                }
                else if (c == "" || c == "+")
                    coeffs[i] = 1;
                else if (c == "-")
                    coeffs[i] = -1;
                else
                    coeffs[i] = float.Parse(c);

                if (coeffs[i] == 0)
                    terms[i] = "\0";
                c = "";
                slash = false;
                deci = false;
            }
            return coeffs;
        }

        public void splitCoeff(string[] terms, int[] end)
        {
            string[] variable = { "x", "y", "z", "a", "b", "c" };
            for (int i = 0; i < terms.Length; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    string[] split = terms[i].Split(new string[] { (variable[j]) }, System.StringSplitOptions.None);

                    if (split.Length == 1)
                    {
                        if (terms[i].Contains(variable[j]))
                            terms[i] = terms[i] + "^{1}";
                        else
                            continue;
                    }
                    else {
                        if (split[1].StartsWith("^"))
                            continue;
                        else
                            terms[i] = split[0] + variable[j] + "^{1}" + split[1];
                    }
                }
            }
        }

        public int[,] CheckCoeffAndVar(int[] end, string[] terms)
        {

            int pow = 0;
            int[,] power = new int[6, 6];
            string[] var2;
            string variable;
            splitCoeff(terms, end);
            for (int i = 0; i < noOfTerms; i++)
            {
                string[] var = terms[i].Split(new string[] { "^{" }, System.StringSplitOptions.None);

                if (var[0] == "\0")
                    continue;
                char[] temp2 = terms[i].ToCharArray();

                //	CerebroHelper.DebugLog (end [i]);
                if (temp2.Length == end[i])
                    continue;
                variable = temp2[end[i]].ToString();
                for (int j = 0; j < var.Length - 1; j++)
                {
                    var2 = var[j + 1].Split(new string[] { "}" }, System.StringSplitOptions.None);
                    pow = int.Parse(var2[0]);

                    switch (variable)
                    {
                        case "a":
                            power[i, 0] = pow;
                            break;

                        case "b":
                            power[i, 1] = pow;
                            break;
                        case "c":
                            power[i, 2] = pow;
                            break;
                        case "x":
                            power[i, 3] = pow;
                            break;
                        case "y":
                            power[i, 4] = pow;
                            break;
                        case "z":
                            power[i, 5] = pow;
                            break;

                    }
                    variable = var2[1];

                }
            }
            return power;
        }


        public override void SubmitClick()
        {
            upflag = 0;
            if (ignoreTouches || userAnswerLaText.text == "")
            {
                return;
            }

            upflag = 0;
            numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");

            int increment = 0;
            //var correct = false;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            bool correct = false;
           
            questionsAttempted++;
            updateQuestionsAttempted();
           

               
            if (level == 3 && (selector == 4 || selector == 5))
            {
                

             
                for (var i = 0; i < Answerarray.Length; i++)
                {
                    if(Answerarray[i] == userAnswerLaText.text)
                    {
                        correct = true;
                        break;
                    }
                }

                //if (checkArrayValues(Answerarray, userAnswers.ToArray()))
                //{
                //    correct = true;
                //}
                //else
                //{
                //    correct = false;
                //}
            }
            else if (MCQ.activeSelf || ThreeChoice.activeSelf)
            {
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
            }
            else if (userAnswerLaText.text == Answer)
            {
                correct = true;
            }
            else if (userAnswerLaText.text == "" || userAnswerLaText.text.EndsWith("/") || userAnswerLaText.text.StartsWith("/") || userAnswerLaText.text.StartsWith("=") || userAnswerLaText.text.EndsWith("=") || userAnswerLaText.text.EndsWith("-") )
            {
                correct = false;
            }
            else {
                if (userAnswerLaText.text.Contains("^{}"))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Replace("^{}", "");
                }
                if (userAnswerLaText.text.EndsWith("^{"))
                {
                    string[] tempo = userAnswerLaText.text.Split(new string[] { "^{" }, System.StringSplitOptions.None);
                    userAnswerLaText.text = tempo[0];
                }
                //upflag = 0;
                //numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");

                for (int i = 0; i < end.Length; i++)
                    end[i] = 0;
                char[] userAns = userAnswerLaText.text.ToCharArray();

                string[] userTerms = SeparateTerms(userAns);
                int Ulast = noOfTerms;
                float[] UserCoeff = SeparateCoeff(userTerms);
                int[,] UserPow = CheckCoeffAndVar(end, userTerms);
                for (int i = 0; i < end.Length; i++)
                {
                    end[i] = 0;
                }
                char[] ans = Answer.ToCharArray();
                string[] AnsTerms = SeparateTerms(ans);
                int Alast = noOfTerms;
                float[] AnsCoeff = SeparateCoeff(AnsTerms);
                int[,] AnsPow = CheckCoeffAndVar(end, AnsTerms);
                
               


                for (int i = 0; i < Alast; i++)
                {                      //AnswerPow
                    for (int j = 0; j < Ulast; j++)
                    {                  //UserPow
                        if (UserPow[j, 0] == -100)
                            continue;

                        for (int k = 0; k < 6; k++)
                        {
                            CerebroHelper.DebugLog(AnsPow[i, k] + "AND" + UserPow[j, k]);

                            if (AnsPow[i, k] == UserPow[j, k])
                            {
                                correct = true;
                                if (k == 5)
                                {
                                    if (Mathf.Abs(UserCoeff[j] - AnsCoeff[i]) <= 0.05f)
                                    {
                                        correct = true;
                                        for (int m = 0; m < 6; m++)
                                            UserPow[j, m] = -100;
                                    }
                                    else
                                    {
                                        CerebroHelper.DebugLog("1");
                                        correct = false;
                                    }
                                    break;
                                }
                            }
                            else {
                                CerebroHelper.DebugLog("2");
                                correct = false;
                                break;
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
				else if (Queslevel == 7)
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
        IEnumerator AnimateThreeChoiceOption(int value)
        {
            var GO = ThreeChoice.transform.Find("Option" + value.ToString()).gameObject;
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
            Go.to(answerButton.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
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
                        CerebroHelper.DebugLog("going in if");
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
            var tween = new GoTween(answerButton.gameObject.transform, 0.2f, config);
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


          //  subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition = new Vector2(subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition.x, -50);

            level = Queslevel;
			for (int i = 1; i < 3; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}
			for (int i = 1; i < 4; i++) {
				ThreeChoice.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}
            
            answerButton = GeneralButton;
            GeneralButton.gameObject.SetActive(true);
            subQuestionText.gameObject.SetActive(false);
			subQuestionTEX.gameObject.SetActive(true);
           // QuestionTEX.gameObject.SetActive(false);
          //  tempQuestionTEX.gameObject.SetActive(false);
			QuestionLatext.gameObject.SetActive(false);
            QuestionText.gameObject.SetActive(true);
            MCQ.gameObject.SetActive(false);
            ThreeChoice.gameObject.SetActive(false);
            numPad.SetActive(true);
            if (Queslevel > scorestreaklvls.Length)
            {
                level = UnityEngine.Random.Range(1, scorestreaklvls.Length + 1);
            }
			subQuestionTEX.text = "";
            #region L1
            if (level == 1)
            {
               
                selector = GetRandomSelector(1, 5);
                QuestionText.text =  "";
              
                if (selector == 1)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                   // //subQuestionText.gameObject.SetActive (true);
					subQuestionTEX.gameObject.SetActive(true);
                  QuestionText.text = "Choose the variable from the expression:";
                    int temp = Random.Range(1, 4);
                    if (temp == 1)
                    {
						subQuestionTEX.text = coeff1 + " + " + coeff2 + "\\xalgebra";
						Answer = "\\xalgebra";
                    }
                    else if(temp == 2)
                    {
						subQuestionTEX.text = coeff1 + " + " + coeff2 + "\\yalgebra";
						Answer = "\\yalgebra";
                    }
                    else
                    {
						subQuestionTEX.text = coeff1 + " + " + coeff2 + "\\zalgebra";
						Answer = "\\zalgebra";
                    }
                   
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    coeff4 = Random.Range(10, 50);
                   // //subQuestionText.gameObject.SetActive (true);
					subQuestionTEX.gameObject.SetActive(true);
                  QuestionText.text = "Choose the constant from the expression:";
                    int temp = Random.Range(1, 3);
                    if (temp == 1)
                    {
						subQuestionTEX.text = coeff3 + "\\xalgebra + " + coeff4 + "\\yalgebra + " + coeff1 + " + " + coeff2 + "\\zalgebra";
                    }
                    else if(temp == 2)
                    {
						subQuestionTEX.text = coeff3 + "\\xalgebra^{2} + " + coeff4 + "\\yalgebra + "  + coeff2 + "\\zalgebra + " + coeff1 ;
                    }
                    Answer = coeff1.ToString();
                  
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    pow1 = Random.Range(2, 10);
                    pow2 = Random.Range(2, 10);

                  QuestionText.text = "The number of terms in the following expression :";
					subQuestionTEX.gameObject.SetActive(true);
                    expression1 = coeff1 + "\\xalgebra^{" + pow1 + "}";
                    int temp = Random.Range(1, 4);
                    if (temp == 1)
                    {
						expression1 += " + " + coeff2 + "\\yalgebra^{" + pow2 + "} + " + coeff3 + "\\xalgebra\\yalgebra(\\xalgebra+\\yalgebra)";
                        Answer = "4";
                    }
                    else if (temp == 2)
                    {
						expression1 += " + " + coeff3 + "\\xalgebra\\yalgebra(\\xalgebra+\\yalgebra)";
                        Answer = "3";
                    }
                    else 
                    {
						expression1 += " + " + coeff2 + "(\\xalgebra+\\yalgebra) + " + coeff3 + "\\xalgebra\\yalgebra(\\xalgebra+\\yalgebra)";
                        Answer = "5";
                    }
					subQuestionTEX.text = expression1;
                   
                }
                
                else if (selector == 4)
                {
					subQuestionTEX.gameObject.SetActive(true);
					GeneralButton.gameObject.SetActive(false);
                    MCQ.gameObject.SetActive(true);
                    numPad.SetActive(false);
                  QuestionText.text = "State True or False :";
                    int temp = Random.Range(1, 3);
                    if (temp == 1)
                    {
						expression3 = "\\frac{2}{3}\\xalgebra\\yalgebra^2, -\\frac{1}{3}\\xalgebra^2\\yalgebra and \\frac{2}{3}\\xalgebra\\yalgebra^2 are like terms";
                        Answer = "False";
                    }
                    else
                    {
						expression3 = "\\frac{2}{3}\\xalgebra\\yalgebra^2, -\\frac{1}{3}\\xalgebra\\yalgebra^2 and \\frac{2}{3}\\xalgebra\\yalgebra^2 are like terms";
                        Answer = "True";
                    }
					subQuestionTEX.text = expression3;
                   
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "True";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "False";
                }
            }
            #endregion L1
            #region L2
            else if (level == 2)
            {
                selector = GetRandomSelector(1, 6);
                QuestionText.text =  "";
               
                if (selector == 1)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                   // //subQuestionText.gameObject.SetActive (true);
					subQuestionTEX.gameObject.SetActive(true);
                  QuestionText.text = "Find the unlike term from the expression :";
                    int temp = Random.Range(1, 3) ;
                    if(temp == 1)
                    {
						subQuestionTEX.text = coeff1 + "\\xalgebra\\yalgebra^{2} + " + coeff2 + "\\xalgebra^{2}\\yalgebra + " + coeff3 + "\\xalgebra\\yalgebra^{2}";
                    }else
                    {
						subQuestionTEX.text = coeff1 + "\\xalgebra\\yalgebra^{2} + " + coeff3 + "\\xalgebra\\yalgebra^{2} + " + coeff2 + "\\xalgebra^{2}\\yalgebra" ;
                    }
                   
					Answer = coeff2 + "\\xalgebra^{2}\\yalgebra";
                }
                else if (selector == 2)
                {
					subQuestionTEX.gameObject.SetActive(true);
                    coeff1 = Random.Range(2, 50);
					if(Random.Range(1,3) == 1)
						coeff1 *= -1;
                    pow1 = Random.Range(2, 6);
					pow2 = Random.Range(2, 6);
					pow3 = Random.Range(2, 6);
                  QuestionText.text = "Write the numerical co-efficient of :";
                    expression3 = coeff1 + "\\xalgebra^{" + pow1 + "}\\yalgebra^{" + pow2 + "}\\zalgebra^{" + pow3 + "}";
					subQuestionTEX.text = expression3;
                    Answer = coeff1.ToString();
                }
                else if (selector == 3)
                {
					subQuestionTEX.gameObject.SetActive(true);
                    coeff1 = Random.Range(2,10);
					if(Random.Range(1,3) == 1)
						coeff1 *= -1;
                    pow1 = Random.Range(2, 10);
                    pow2 = Random.Range(2, 10);
                    pow3 = Random.Range(2, 10);
					QuestionText.text = "Write the coefficient of :";
                    expression1 = coeff1 + "\\xalgebra^{" + pow1 + "}\\yalgebra^{" + pow2 + "}\\zalgebra^{" + pow3 + "}";
                    expression3 = "\\xalgebra^{" + pow1 + "}";
					subQuestionTEX.text = expression3 + " in " + expression1;
                    Answer = coeff1 + "\\yalgebra^{" + pow2 + "}\\zalgebra^{" + pow3 + "}";

                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(2, 100);
					if(Random.Range(1,3) == 1)
						coeff1 *= -1;
                    a = Random.Range(2, 10);
                    b = Random.Range(2, 10);
                    c = Random.Range(2, 10);
					subQuestionTEX.gameObject.SetActive(true);
                  QuestionText.text = "Write the coefficient of :";
                    int temp = Random.Range(1, 3);
                    if (temp == 1)
                    {
						subQuestionTEX.text = "\\xalgebra^{" + a + "}\\yalgebra^{" + b + "} in " + coeff1 + "\\xalgebra^{" + a + "}\\yalgebra^{" + b + "}\\zalgebra^{" + c + "}";
                        Answer = coeff1.ToString() + "\\zalgebra^{" + c + "}";
                    }
                    else if(temp ==2){
						subQuestionTEX.text = "\\xalgebra^{" + a + "}\\zalgebra^{" + c + "} in " + coeff1 + "\\xalgebra^{" + a + "}\\yalgebra^{" + b + "}\\zalgebra^{" + c + "}";
                        Answer = coeff1.ToString() + "\\yalgebra^{" + b + "}";
                    }
                    else
                    {
						subQuestionTEX.text = "\\xalgebra^{" + a + "}\\yalgebra^{" + b + "}\\zalgebra^{" + c + "} in " + coeff1 + "\\xalgebra^{" + a + "}\\yalgebra^{" + b + "}\\zalgebra^{" + c + "}";
                        Answer = coeff1.ToString() ;
                    }
                }
                else if (selector == 5)
                {
					subQuestionTEX.gameObject.SetActive(true);
                    ThreeChoice.gameObject.SetActive(true);
					GeneralButton.gameObject.SetActive(false);
                    numPad.SetActive(false);
                    coeff1 = Random.Range(2, 10);
					if(Random.Range(1,3) == 1)
						coeff1 *= -1;
                    coeff2 = Random.Range(2, 10);
                    int temp = Random.Range(1, 4);
                    if (temp == 1)
                    {
						expression3 = coeff1 + "\\frac{\\xalgebra}{\\yalgebra}";
                        Answer = "Monomial";
                    }
                    else if(temp == 2)
                    {
						expression3 = coeff1 + "\\frac{\\xalgebra}{\\yalgebra} + " + coeff2 + "\\xalgebra + 13\\xalgebra";
                        Answer = "Binomial";
                    }
                    else
                    {
						expression3 = coeff1 + "\\frac{\\xalgebra}{\\yalgebra} + " + coeff2 + "\\frac{\\yalgebra}{\\xalgebra} + \\xalgebra";
                        Answer = "Trinomial";
                    }
                  QuestionText.text = "Is the following a monomial, binomial or trinomial?";
					subQuestionTEX.text = expression3;
                    
                    ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "Monomial";
                    ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "Binomial";
                    ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<TEXDraw>().text = "Trinomial";

                }

            }
            #endregion L2
            #region L3
            else if (level == 3)
            {

              QuestionText.text = "";
                QuestionText.text =  "";
                subQuestionTEX.text = "";
                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
                    coeff1 = Random.Range(2, 10);
                    coeff2 = Random.Range(2, 10);
                    coeff3 = Random.Range(2, 10);
                    a = Random.Range(2, 6);
                    b = Random.Range(2, 6);
                    c = Random.Range(2, 6);
                    while (a + b + c > 11)
                    {
                        a = Random.Range(2, 10);
                        b = Random.Range(2, 10);
                        c = Random.Range(2, 10);
                    }
                    ////subQuestionText.gameObject.SetActive (true);

                  QuestionText.text = "Write the exponential form of the following expression :";
					subQuestionTEX.gameObject.SetActive(true);

					subQuestionTEX.text = coeff1.ToString();

                    for (int i = 0; i < a; i++)
                    {
						subQuestionTEX.text += "\\times \\xalgebra ";
                    }
					subQuestionTEX.text += "\\times " + coeff2.ToString();
                    for (int i = 0; i < b; i++)
                    {
						subQuestionTEX.text += "\\times \\yalgebra ";
                    }
					subQuestionTEX.text += "\\times " + coeff3.ToString();
                    for (int i = 0; i < c; i++)
                    {
						subQuestionTEX.text += "\\times \\zalgebra ";
                    }


					Answer = coeff1*coeff2*coeff3 + "\\xalgebra^{" + a + "}" + "\\yalgebra^{" + b + "}" + "\\zalgebra^{" + c + "}";
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(2, 100);
                    coeff3 = Random.Range(2, 100);
                    ////subQuestionText.gameObject.SetActive (true);
                  QuestionText.text = "Find the degree of :";
					subQuestionTEX.gameObject.SetActive(true);
                    a = Random.Range(2, 10);
                    int temp = Random.Range(1, 3);
                    if(temp == 1)
						subQuestionTEX.text = coeff1 + "\\xalgebra^" + a + " + " + coeff2 + "\\xalgebra + " + coeff3;
					else subQuestionTEX.text = coeff2 + "\\xalgebra + " + coeff1 + "\\xalgebra^" + a + " + " + coeff3;
                    Answer = a.ToString();
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(2, 10);
                    coeff2 = Random.Range(2, 10);
                    coeff3 = Random.Range(2, 10);
                    coeff4 = Random.Range(2, 10);
                    coeff5 = Random.Range(2, 10);
                    coeff6 = Random.Range(10, 50);
                    ////subQuestionText.gameObject.SetActive (true);
                  QuestionText.text = "Find the degree of :";
					subQuestionTEX.gameObject.SetActive(true);
                    a = Random.Range(2, 10);
                    b = Random.Range(2, 10);
                    c = Random.Range(2, 10);
                    int d = Random.Range(2, 10);
                    while (a + b < c + d)
                    {
                        a = Random.Range(2, 10);
                        d = Random.Range(2, 10);
                        c = 2;
                    }
					subQuestionTEX.text = coeff1 + "\\xalgebra^2 + \\frac{" + 1 + "}{" + coeff2 + "}\\yalgebra^" + c + " + " + coeff3 + "\\xalgebra^" + a + "\\yalgebra^" + b + " + " + coeff4 + "." + coeff5 + "\\xalgebra^" + c + "\\yalgebra^" + d + " +" + coeff6;
                    Answer = (a + b).ToString();
                }
                else if (selector == 4)
                {
                    subQuestionTEX.gameObject.SetActive(true);
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(2, 100);
					QuestionText.text = "Express in algebraic form :";
					expression3 = coeff1 + " times \\xalgebra subtracted from " + coeff1 + " times \\yalgebra is equal to " + coeff2;
					//subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition = new Vector2(subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition.x, -106);
                    subQuestionTEX.text = expression3;
                    string[] answer = new string[4];
                    
					answer[0] = coeff1 + "\\yalgebra-" + coeff1 + "\\xalgebra=" + coeff2;
					answer[1] = "-" + coeff1 + "\\xalgebra" + coeff1 + "\\yalgebra=" + coeff2;
					answer[2] = coeff1 + "\\yalgebra=" + coeff1 + "\\xalgebra+" + coeff2;
                    Answerarray = answer; 
					Answer = coeff1 + "\\yalgebra-" + coeff1 + "\\xalgebra=" + coeff2;
                }
                else if (selector == 5)
                {
					coeff1 = Random.Range(2, 10);
                    coeff2 = Random.Range(2, 10);
					while (coeff1 == coeff2)
						coeff2 = Random.Range(2, 10);
                    coeff3 = Random.Range(1, 100);
					QuestionText.text = "Express in algebraic form :";
                    subQuestionTEX.gameObject.SetActive(true);
					expression3 = coeff1 + "\\xalgebra added to -" + coeff2 + "\\xalgebra gives " + coeff3;
					//subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition = new Vector2(subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition.x, -106);
                    subQuestionTEX.text = expression3;
                    string[] answer = new string[4];
                   
					answer[0] = coeff1-coeff2 + "\\xalgebra=" + coeff3;
					answer[1] = coeff1 + "\\xalgebra=" + coeff2 + "\\xalgebra+" + coeff3;
					answer[2] = coeff1 + "\\xalgebra=" + coeff3 + coeff2 + "\\xalgebra";                    
					if (coeff1 - coeff2 ==1)
						Answer = "\\xalgebra=" + coeff3;
					else if( coeff1 - coeff2 == -1)
						Answer = "-\\xalgebra=" + coeff3;
					else
						Answer = (coeff1 - coeff2) + "\\xalgebra=" + coeff3;
					answer[3] = Answer;
					Answerarray = answer;
                }
            }
            #endregion L3
            #region L4
            else if (level == 4)
            {
                selector = GetRandomSelector(1, 6);
              QuestionText.text = "";
                

                if (selector == 1)
                {

                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
					coeff4 = Random.Range(2, 100);
                    coeff5 = Random.Range(10, 50);
                    coeff6 = Random.Range(10, 50);
					while (coeff2 == coeff5 || Mathf.Abs(coeff2-coeff5) == 1)
						coeff5 = Random.Range(10,50);
                  QuestionText.text = "Add :";
					subQuestionTEX.gameObject.SetActive(true);
					subQuestionTEX.text = coeff1 + "\\xalgebra + " + coeff2 + "\\yalgebra + " + coeff3 + "\\zalgebra^{2}, " + coeff4 + "\\xalgebra - " + coeff5 + "\\yalgebra + " + coeff6 + "\\zalgebra^{2}";
					Answer = (coeff1 + coeff4) + "\\xalgebra" + (coeff2 - coeff5 < 0 ? (coeff2 - coeff5).ToString() : ("+" + (coeff2 - coeff5).ToString())) + "\\yalgebra+" + (coeff3 + coeff6) + "\\zalgebra^{2}";
                }
                else if (selector == 2)
                {
					coeff1 = Random.Range(2, 100);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
					coeff2 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
					coeff3 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
					coeff4 = Random.Range(2, 100);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
					coeff5 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff5 *= -1;
					coeff6 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff6 *= -1;
					
					QuestionText.text = "Subtract :";
					subQuestionTEX.gameObject.SetActive(true);
					subQuestionTEX.text = coeff1 + "\\xalgebra " + (coeff2 > 0 ? "+" : "") + coeff2 + "\\yalgebra " + (coeff3 > 0 ? "+" : "") + coeff3 + "\\zalgebra^{2} from " + coeff4 + "\\xalgebra " + (coeff5 > 0 ? "+" : "") + coeff5 + "\\yalgebra " + (coeff6 > 0 ? "+" : "") + coeff6 + "\\zalgebra^{2}";
					Answer = "";
					if (coeff4 - coeff1 == 1)
						Answer += "\\xalgebra";
					else if (coeff4 - coeff1 == -1)
						Answer += "-\\xalgebra";
					else if (coeff4 - coeff1 == 0)
						Answer += "";
					else
						Answer += (coeff4 - coeff1 > 0 ? "+" : "") + (coeff4 - coeff1) + "\\xalgebra";
					
					if (coeff5 - coeff2 == 1)
						Answer += "+\\yalgebra";
					else if (coeff5 - coeff2 == -1)
						Answer += "-\\yalgebra";
					else if (coeff5 - coeff2 == 0)
						Answer += "";
					else
						Answer += (coeff5 - coeff2 > 0 ? "+" : "") + (coeff5 - coeff2) + "\\yalgebra";

					if (coeff6 - coeff3 == 1)
						Answer += "+\\zalgebra^{2}";
					else if (coeff6 - coeff3 == -1)
						Answer += "-\\zalgebra^{2}";
					else if (coeff6 - coeff3 == 0)
						Answer += "";
					else
						Answer += (coeff6 - coeff3 > 0 ? "+" : "") + (coeff6 - coeff3) + "\\zalgebra^{2}";
					
					while (Answer[0] == '+')
						Answer = Answer.Substring(1);


				}
                else if (selector == 3)
                {
					subQuestionTEX.gameObject.SetActive(true);

                  QuestionText.text = "Subtract :";
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    coeff4 = Random.Range(2, 100);
                    coeff5 = Random.Range(10, 50);
                    a = Random.Range(10, 100);
					while (a == coeff1 || Mathf.Abs(a - coeff1) == 1)
						a = Random.Range(10, 100);
                    b = Random.Range(10, 50);
					while (b == coeff2 || Mathf.Abs(b - coeff2) == 1)
						b = Random.Range(10, 5);
                    c = Random.Range(10, 50);
					while (c == coeff1 || Mathf.Abs(c - coeff3) == 1)
						c = Random.Range(10, 50);
                    int d = Random.Range(2, 100);
					while (d == coeff1 || Mathf.Abs(d - coeff4) == 1)
						d = Random.Range(2, 100);
                    int e = Random.Range(10, 50);
					while (e == coeff1 || Mathf.Abs(e - coeff5) == 1)
						e = Random.Range(2, 100);
					subQuestionTEX.text = coeff1 + "\\xalgebra^4+" + coeff2 + "\\xalgebra^3+" + coeff3 + "\\xalgebra^2+" + coeff4 + "\\xalgebra+" + coeff5 + " from " + a + "\\xalgebra^4+" + b + "\\xalgebra^3+" + c + "\\xalgebra^2+" + d + "\\xalgebra+" + e;
                    Answer = (a - coeff1) + "\\xalgebra^{4}";
                    if (b - coeff2 > 0)
                    {
                        Answer += "+" + (b - coeff2) + "\\xalgebra^{3}";
                    }
                    else
                    {
                        Answer += (b - coeff2) + "\\xalgebra^{3}";
                    }
                    if ((c - coeff3) > 0)
                    {
                        Answer += "+" + (c - coeff3) + "\\xalgebra^{2}";
                    }
                    else
                    {
                        Answer += (c - coeff3) + "\\xalgebra^{2}";
                    }
                    if ((d - coeff4) > 0)
                    {
						Answer += "+" + (d - coeff4) + "\\xalgebra";
                    }
                    else
                    {
						Answer += (d - coeff4) + "\\xalgebra";
                    }
                    if ((e - coeff5) > 0)
                    {
                        Answer += "+" + (e - coeff5);
                    }
                    else
                    {
                        Answer += (e - coeff5);
                    }


                }
                else if (selector == 4)
                {
					subQuestionTEX.gameObject.SetActive(true);

                  QuestionText.text = "Simplify the following :";
					coeff1 = Random.Range(2,20);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
					coeff2 = Random.Range(2,20);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
					coeff3 = Random.Range(2,20);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
					coeff4 = Random.Range(2,20);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
					coeff5 = coeff4;
					a = Random.Range(2,20);
					if (Random.Range(1,3) == 1)
						a *= -1;       
					b = Random.Range(2,20);
					if (Random.Range(1,3) == 1)
						b *= -1;
					c = Random.Range(2,20);
					if (Random.Range(1,3) == 1)
						c *= -1;
					int d = Random.Range(2,20);
					if (Random.Range(1,3) == 1)
						d *= -1;
            
                    if (coeff1 + coeff5 + c == 1)           
                        c++;
					if (coeff1 + coeff5 + c == -1)           
						c--;
                    if (coeff3 + b + d == 1)
                        d++;
					if (coeff3 + b + d == -1)
						d--;
                    if (coeff2 + coeff4 + a == 1)
                        a++;
					if (coeff2 + coeff4 + a == -1)
						a--;

					subQuestionTEX.text = coeff1 + "\\xalgebra ";
                    if (coeff2 > 0)
						subQuestionTEX.text +=  "+" + coeff2 + "\\yalgebra ";
					else subQuestionTEX.text +=   coeff2 + "\\yalgebra ";
                    if (coeff3 > 0)
						subQuestionTEX.text += "+" + coeff3 + "\\xalgebra^2\\yalgebra^2 ";
					else subQuestionTEX.text += coeff3 + "\\xalgebra^2\\yalgebra^2 ";
                    if (coeff4 > 0)
						subQuestionTEX.text += "+" + coeff4 + "\\yalgebra ";
					else subQuestionTEX.text += coeff4 + "\\yalgebra ";
                    if (coeff5 > 0)
						subQuestionTEX.text += "+" + coeff5 + "\\xalgebra ";
					else subQuestionTEX.text += coeff5 + "\\xalgebra ";
                    if (a > 0)
						subQuestionTEX.text += "+" + a + "\\yalgebra ";
					else subQuestionTEX.text += a + "\\yalgebra ";
                    if (b > 0)
						subQuestionTEX.text += "+" + b + "\\xalgebra^2\\yalgebra^2 ";
					else subQuestionTEX.text += b + "\\xalgebra^2\\yalgebra^2 ";
                    if (c > 0)
						subQuestionTEX.text += "+" + c + "\\xalgebra ";
					else subQuestionTEX.text += c + "\\xalgebra ";
                    if (d > 0)
						subQuestionTEX.text += "+" + d + "\\xalgebra^2\\yalgebra^2 ";
					else subQuestionTEX.text += d + "\\xalgebra^2\\yalgebra^2 ";

					Answer = coeff1 + coeff5 + c == 0 ? "" : ((coeff1 + coeff5 + c) + "\\xalgebra");
                    if (coeff2 + coeff4 + a > 0)
						Answer += "+" + (coeff2 + coeff4 + a) + "\\yalgebra";
					else Answer += coeff2 + coeff4 + a == 0 ? "" : ((coeff2 + coeff4 + a) + "\\yalgebra");
					if (coeff3 + b + d > 0)
						Answer += "+" + (coeff3 + b + d) + "\\xalgebra^{2}\\yalgebra^{2}";
					else Answer += coeff3 + b + d == 0 ? "" : ((coeff3 + b + d) + "\\xalgebra^{2}\\yalgebra^{2}");

                }
                else if (selector == 5)
                {

                    coeff1 = Random.Range(2, 100);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
                    coeff2 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
                    coeff3 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
                    coeff4 = Random.Range(2, 100);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
                    coeff5 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff5 *= -1;
                    coeff6 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff6 *= -1;
					if (coeff4-coeff1 == 0)
						coeff1 *= -1;
					if (coeff5-coeff2 == 0)
						coeff2 *= -1;
					if (coeff6-coeff3 == 0)
						coeff3 *= -1;
                    // subQuestionText.gameObject.SetActive(fa);
                  QuestionText.text = "What must be subtracted from expression A to get expression B?";
					subQuestionTEX.gameObject.SetActive(true);
                    //tempQuestionTEX.gameObject.SetActive(true);
					subQuestionTEX.text = "B : " + coeff1 + "\\xalgebra^3 " + (coeff2 < 0 ? "" : "+") + coeff2 + "\\xalgebra^2\\yalgebra^2 " +(coeff3 < 0 ? "" : "+") + coeff3 + "\\zalgebra";
					subQuestionTEX.text +="\n\n" + "A : " + coeff4 + "\\xalgebra^3 " + (coeff5 < 0 ? "" : "+") + coeff5 + "\\xalgebra^2\\yalgebra^2 " + (coeff6 < 0 ? "" : "+") + coeff6 + "\\zalgebra";

					Answer = (coeff4 - coeff1) + "\\xalgebra^{3}" + (coeff5 - coeff2 < 0 ? "" : "+") + (coeff5 - coeff2) + "\\xalgebra^{2}\\yalgebra^{2}" + (coeff6 - coeff3 < 0 ? "" : "+") + (coeff6 - coeff3) + "\\zalgebra";
                }

            }
            #endregion L4
            #region L5
            else if (level == 5)
            {
                selector = GetRandomSelector(1, 5);
              QuestionText.text = "";
               // subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition = new Vector2(subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition.x, -110);
                if (selector == 1)
                {
                    coeff1 = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
                    coeff2 = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
                    coeff3 = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
                    coeff4 = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
                    coeff5 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff5 *= -1;
                    coeff6 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff6 *= -1;
                    a = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						a *= -1;
                    b = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						b *= -1;
                    c = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						c *= -1;
                    ////subQuestionText.gameObject.SetActive (true);
                  QuestionText.text = "Subtract A from the sum of B and C.";
					subQuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                   // tempQuestionTEX.gameObject.SetActive(true);
					subQuestionTEX.text = "B : " + coeff1 + "\\xalgebra^2 " + (coeff2 < 0 ? coeff2.ToString() : ("+ " + coeff2.ToString())) + "\\xalgebra\\yalgebra " + (coeff3 < 0 ? coeff3.ToString() :( "+ " + coeff3.ToString())) + "\\yalgebra^2";
					subQuestionTEX.text +="\n\n" +  "A : " + coeff4 + "\\xalgebra^2 " + (coeff5 < 0 ? coeff5.ToString() : ("+ " + coeff5.ToString())) + "\\xalgebra\\yalgebra " + (coeff6 < 0 ? coeff6.ToString() : ("+ " + coeff6.ToString())) + "\\yalgebra^2";
					subQuestionTEX.text +="\n\n" + "C : " + a + "\\xalgebra^2 " + (b < 0 ? b.ToString() : ("+ " + b.ToString())) + "\\xalgebra\\yalgebra " + (c < 0 ? c.ToString() : ("+ " + c.ToString())) + "\\yalgebra^2";

					if (coeff1 + a - coeff4 == 1)
						Answer = "\\xalgebra^2";
					else if (coeff1 + a - coeff4 == -1)
						Answer = "-\\xalgebra^2";
					else if (coeff1 + a - coeff4 == 0)
						Answer = "";
					else
						Answer = (coeff1 + a - coeff4) + "\\xalgebra^2";
                    if (coeff2 + b - coeff5 > 0)
                    {
						if (coeff2 + b - coeff5 == 1)
							Answer += "+\\xalgebra\\yalgebra";
						else
							Answer += "+" + (coeff2 + b - coeff5) + "\\xalgebra\\yalgebra";
                    }
                    else
                    {
						if (coeff2 + b - coeff5 == -1)
							Answer += "-\\xalgebra\\yalgebra";
						else
							Answer += (coeff2 + b - coeff5) + "\\xalgebra\\yalgebra";
                    }
                    if ((coeff3 + c - coeff6) > 0)
                    {
						if (coeff3 + c - coeff6 == 1)
							Answer += "+\\yalgebra^2";
						else
							Answer += "+" + (coeff3 + c - coeff6) + "\\yalgebra^2";
                    }
                    else
                    {
						if (coeff3 + c - coeff6 == -1)
							Answer += "-\\yalgebra^2";
						else
							Answer += (coeff3 + c - coeff6) + "\\yalgebra^2";
                    }
					while (Answer[0] == '+')
						Answer = Answer.Substring(1);

                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
                    coeff2 = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
                    coeff3 = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
                    coeff4 = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
                    coeff5 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff5 *= -1;
                    coeff6 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff6 *= -1;
					if (coeff3 + coeff6 == 0)
						coeff3 *= -1;
					if (coeff1 + coeff4 == 0)
						coeff1 *= -1;
					if (coeff2 + coeff5 == 0)
						coeff2 *= -1;

                    // //subQuestionText.gameObject.SetActive (true);
                  QuestionText.text = "Find the perimeter of a rectangle whose length and breadth are given as A and B.";
					subQuestionTEX.gameObject.SetActive(true);
                   

					subQuestionTEX.text = "B : " + coeff4 + "\\xalgebra " + (coeff5 < 0 ? coeff5.ToString() : ("+ " + coeff5.ToString())) + "\\yalgebra " + (coeff6 < 0 ? coeff6.ToString() : ("+ " + coeff6.ToString())) + "\\zalgebra";
					subQuestionTEX.text  +="\n\n" +  "A : " + coeff1 + "\\xalgebra " + (coeff2 < 0 ? coeff2.ToString() : ("+ " + coeff2.ToString())) + "\\yalgebra " + (coeff3 < 0 ? coeff3.ToString() : ("+ " + coeff3.ToString())) + "\\zalgebra";

					Answer = (2*(coeff1 + coeff4)).ToString() + "\\xalgebra" + (coeff2 + coeff5 > 0 ? "+" : "") + (2*(coeff2 + coeff5)).ToString() + "\\yalgebra" + (coeff3 + coeff6 > 0 ? "+" : "") + (2*(coeff3 + coeff6)).ToString() + "\\zalgebra";
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
                    coeff2 = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
                    coeff3 = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
                    coeff4 = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
                    coeff5 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff5 *= -1;
                    coeff6 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff6 *= -1;
                    a = 3; b = 3; c = 3;
					while ((coeff1 + coeff4 + a) % 2 != 0 || coeff1 + coeff4 + a == 0 || coeff1 + coeff4 + a == 1 || coeff1 + coeff4 + a == -1)
						a = Random.Range(2, 20);
					while ((coeff2 + coeff5 + b) % 2 != 0 || coeff2 + coeff5 + b == 0 || coeff2 + coeff5 + b == 1 || coeff2 + coeff5 + b == -1)
						b = Random.Range(2, 20);
					while ((coeff3 + coeff6 + c) % 2 != 0 || coeff3 + coeff6 + c == 0 || coeff3 + coeff6 + c == 1 || coeff3 + coeff6 + c == -1)
						c = Random.Range(2, 20);


                    //  //subQuestionText.gameObject.SetActive (true);
                  QuestionText.text = "Find the semiperimeter of a triangle whose sides are given as A, B and C.";
					subQuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    //tempQuestionTEX.gameObject.SetActive(true);

					subQuestionTEX.text = "A : " + coeff1 + "\\xalgebra " + (coeff2 > 0 ? "+" : "") + coeff2 + "\\yalgebra " + (coeff3 > 0 ? "+" : "") + coeff3 + "\\zalgebra";
					subQuestionTEX.text +="\n\n" +  "B : " + coeff4 + "\\xalgebra " + (coeff5 > 0 ? "+" : "") + coeff5 + "\\yalgebra " + (coeff6 > 0 ? "+" : "") + coeff6 + "\\zalgebra";
					subQuestionTEX.text +="\n\n" + "C : " + a + "\\xalgebra " + (b > 0 ? "+" : "") + b + "\\yalgebra " + (c > 0 ? "+" : "") + c + "\\zalgebra";

					Answer = (coeff1 + coeff4 + a) / 2 + "\\xalgebra" + (coeff2 + coeff5 + b > 0 ? "+" : "") + (coeff2 + coeff5 + b) / 2 + "\\yalgebra" + (coeff3 + coeff6 + c > 0 ? "+" : "") + (coeff3 + coeff6 + c) / 2 + "\\zalgebra";
                }
                else if (selector == 4)
                {
					coeff1 = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
					coeff2 = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
					coeff3 = Random.Range(10, 20);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
					coeff4 = Random.Range(2, 20);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
					coeff5 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff5 *= -1;
					coeff6 = Random.Range(10, 50);
					if (Random.Range(1,3) == 1)
						coeff6 *= -1;
					a = 3; b = 3; c = 3;
					while ((coeff1 + coeff4 + a) % 2 != 0 || coeff1 + coeff4 + a == 0 || coeff1 + coeff4 + a == 2 || coeff1 + coeff4 + a == -2)
						a = Random.Range(2, 20);
					while ((coeff2 + coeff5 + b) % 2 != 0 || coeff2 + coeff5 + b == 0 || coeff2 + coeff5 + b == 2 || coeff2 + coeff5 + b == -2)
						b = Random.Range(2, 20);
					while ((coeff3 + coeff6 + c) % 2 != 0 || coeff3 + coeff6 + c == 0 || coeff3 + coeff6 + c == 2 || coeff3 + coeff6 + c == -2)
						c = Random.Range(2, 20);
					


                    // //subQuestionText.gameObject.SetActive (true);
                  QuestionText.text = "The semiperimeter of a triangle is given by A and the two sides are B and C. Determine the third side.";
					subQuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    //tempQuestionTEX.gameObject.SetActive(true);

					subQuestionTEX.text = "A : " + (coeff1 + coeff4 + a) / 2 + "\\xalgebra" + (coeff2 + coeff5 + b > 0 ? "+" : "") + (coeff2 + coeff5 + b) / 2 + "\\yalgebra" + (coeff3 + coeff6 + c > 0 ? "+" : "") + (coeff3 + coeff6 + c) / 2 + "\\zalgebra";
					subQuestionTEX.text +="\n\n" + "B : " + coeff4 + "\\xalgebra " + (coeff5 > 0 ? "+" : "") + coeff5 + "\\yalgebra " + (coeff6 > 0 ? "+" : "") + coeff6 + "\\zalgebra";
					subQuestionTEX.text += "\n\n" + "C : " + coeff1 + "\\xalgebra " + (coeff2 > 0 ? "+" : "") + coeff2 + "\\yalgebra " + (coeff3 > 0 ? "+" : "") + coeff3 + "\\zalgebra";

					Answer = a + "\\xalgebra " + (b > 0 ? "+" : "") + b + "\\yalgebra " + (c > 0 ? "+" : "") + c + "\\zalgebra";
                }
            }

            #endregion L5

			#region L6
			else if (level == 6)
			{
				selector = GetRandomSelector(1, 5);
				subQuestionTEX.gameObject.SetActive(true);


				if (selector == 1)
				{
					coeff1 = Random.Range(2, 10);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
					coeff2 = Random.Range(2, 10);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
					coeff3 = Random.Range(2, 10);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
					coeff4 = Random.Range(2, 10);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
					QuestionText.text = "Find the remainder :";
					subQuestionTEX.text = "\\xalgebra^{3}" + (coeff1 > 0 ? "+" : "") + coeff1 + "\\xalgebra" + (coeff2 > 0 ? "+" : "") + coeff2 + " by \\xalgebra^{2}" + (coeff3 > 0 ? "+" : "") + coeff3 + "\\xalgebra" + (coeff4 > 0 ? "+" : "") + coeff4;
					int No1 = (coeff3*coeff3) + coeff1 - coeff4;
					int No2 = coeff2 + (coeff3*coeff4);
					if(No1 == 0)
						Answer = No2.ToString();
					else if(No1 == 1)
						Answer = "\\xalgebra" + No2.ToString();
					else if(No1 == -1)
						Answer = "-\\xalgebra" + No2.ToString();
					else
						Answer = No1 + "\\xalgebra" + (No2 > 0 ? "+" : "") + No2;
				}
				else if (selector == 2)
				{
					coeff1 = Random.Range(2, 8);
					if (Random.Range(1,3) == 1)
						coeff1 *= -1;
					coeff2 = Random.Range(2, 8);
					if (Random.Range(1,3) == 1)
						coeff2 *= -1;
					coeff3 = Random.Range(2, 8);
					if (Random.Range(1,3) == 1)
						coeff3 *= -1;
					coeff4 = Random.Range(2, 8);
					if (Random.Range(1,3) == 1)
						coeff4 *= -1;
					coeff5 = Random.Range(2, 8);
					if (Random.Range(1,3) == 1)
						coeff5 *= -1;
					coeff6 = Random.Range(2, 8);
					if (Random.Range(1,3) == 1)
						coeff6 *= -1;
					QuestionText.text = "Solve using the laws of exponents :";
					subQuestionTEX.text = "\\frac{\\xalgebra^{"+coeff1+"}\\yalgebra^{"+coeff2+"}\\zalgebra^{"+coeff3+"}}{\\xalgebra^{"+coeff4+"}\\yalgebra^{"+coeff5+"}\\zalgebra^{"+coeff6+"}}";
					Answer = "\\xalgebra^{" + (coeff1-coeff4) + "}\\yalgebra^{" + (coeff2-coeff5) + "}\\zalgebra^{" + (coeff3-coeff6) + "}";
				}
				else if (selector == 3)
				{
					QuestionText.text = "";
					QuestionLatext.gameObject.SetActive(true);
					coeff1 = Random.Range(2, 8); 
					coeff2 = Random.Range(2, 8);
					coeff3 = Random.Range(2, 8); 
					coeff4 = Random.Range(2, 8); 
					coeff5 = Random.Range(2, 8); 
					coeff6 = Random.Range(2, 8); 
					QuestionLatext.text = "The sides of a rectangle are given by " + coeff1 + "\\xalgebra+" + coeff2 + "\\yalgebra+" + coeff3 + "\\zalgebra and " + coeff4 + "\\xalgebra+" + coeff5 + "\\yalgebra+" + coeff6 + "\\zalgebra. Find its area.";
					Answer = (coeff1*coeff4) + "\\xalgebra^{2}+" + (coeff2*coeff5) + "\\yalgebra^{2}+" + (coeff3*coeff6) + "\\zalgebra^{2}+" + (coeff1*coeff5 + coeff2*coeff4) + "\\xalgebra\\yalgebra+" + (coeff2*coeff6 + coeff3*coeff5) + "\\yalgebra\\zalgebra+" + (coeff1*coeff6 + coeff3*coeff4) + "\\xalgebra\\zalgebra";
				}
				else if (selector == 4)
				{
					QuestionLatext.gameObject.SetActive(true);
					QuestionText.text = "";
					coeff1 = Random.Range(5, 15) * 2;
					coeff2 = Random.Range(1, coeff1/2);
					coeff3 = Random.Range(1, 3);
					coeff4 = coeff1 - (coeff2+coeff3+1);
					coeff5 = Random.Range(2, 6);
					coeff6 = Random.Range(6, 9);
					QuestionLatext.text = "Find \\xalgebra :";
					subQuestionTEX.text = "{({\\frac{" + coeff5 + "}{" + coeff6 + "}})}^{-" + coeff2 + "}\\times{({\\frac{" + coeff5 + "}{" + coeff6 + "}})}^{-" + coeff3 + "}\\times{({\\frac{" + coeff5 + "}{" + coeff6 + "}})}^{-" + coeff4 + "} = {({\\frac{" + coeff5 + "}{" + coeff6 + "}})}^{1-2\\xalgebra}";
					Answer = (coeff1/2).ToString();
				}
			}

			#endregion L6

            CerebroHelper.DebugLog(level + "t" + selector + " @Answer by #DAP ::" + Answer);
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
                if (upflag == 1)
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    userAnswerLaText.text += value.ToString();
                    userAnswerLaText.text += "}";
                }
                else {
                    userAnswerLaText.text += value.ToString();
                }

            }
            else if (value == 10)
            {    //,
                if (checkLastTextFor(new string[1] { "," }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                if (upflag == 1)
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    userAnswerLaText.text += ",";
                    userAnswerLaText.text += "}";
                }
                else {
                    userAnswerLaText.text += ",";
                }
            }
            else if (value == 11)
            {   
                upflag = 0;
                numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");

                if (userAnswerLaText.text.Length == 0)
                {
                    return;
                }

                if (checkLastTextFor(new string[1] { "^{}" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 3);
                }
                else if (checkLastTextFor(new string[1] { "}" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 2);
                    userAnswerLaText.text += "}";
                    upflag = 1;
                    numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");
                }
				else if(checkLastTextFor (new string[3] { "\\xalgebra","\\yalgebra","\\zalgebra" })) {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 9);
					}
				else {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}

                // last check to remove ^{} if that's the last part of userAnswer
                if (checkLastTextFor(new string[1] { "^{}" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 3);
                    upflag = 0;
                    numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");
                }

            }
            else if (value == 12)
            {   // -
                if (checkLastTextFor(new string[1] { "-" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                if (upflag == 1)
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    userAnswerLaText.text += "-";
                    userAnswerLaText.text += "}";
                }
                else {
                    userAnswerLaText.text += "-";
                }
            }
            else if (value == 13)
            {   // +
                if (checkLastTextFor(new string[1] { "+" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                if (upflag == 1)
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    userAnswerLaText.text += "+";
                    userAnswerLaText.text += "}";
                }
                else {
                    userAnswerLaText.text += "+";
                }
            }
            else if (value == 14)
            {   // ^
                if (upflag == 0 && userAnswerLaText.text != "")
                {
                    if (checkLastTextFor(new string[1] { "^{}" }))
                    {
                        userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 3);
                    }
                    if (checkLastTextFor(new string[1] { "}" }))
                    {
                        return;
                    }
                    userAnswerLaText.text += "^{}";
                    upflag = 1;
                    numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("6464DC");
                }
                else if (upflag == 1)
                {
                    if (checkLastTextFor(new string[1] { "}" }))
                    {
                        userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    }
                    userAnswerLaText.text += "}";
                    upflag = 0;
                    numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");
                }
            }
            else if (value == 15)
            {   // x
                upflag = 0;
                numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");
                if (checkLastTextFor(new string[1] { "\\xalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\xalgebra";
            }
            else if (value == 16)
            {   // y
                upflag = 0;
                numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");
                if (checkLastTextFor(new string[1] { "\\yalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\yalgebra";
            }
            else if (value == 17)
            {   // z
                upflag = 0;
                numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");
                if (checkLastTextFor(new string[1] { "\\zalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\zalgebra";
            }
            else if (value == 19)
            {    // AC
                userAnswerLaText.text = "";
            }
            else if (value == 20)
            {   // /
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                if (upflag == 1)
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    userAnswerLaText.text += "/";
                    userAnswerLaText.text += "}";
                }
                else {
                    userAnswerLaText.text += "/";
                }
            }
            else if (value == 21)
            {   // =
                if (checkLastTextFor(new string[1] { "=" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                if (upflag == 1)
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    userAnswerLaText.text += "=";
                    userAnswerLaText.text += "}";
                }
                else {
                    userAnswerLaText.text += "=";
                }
            }
        }
    }
}

