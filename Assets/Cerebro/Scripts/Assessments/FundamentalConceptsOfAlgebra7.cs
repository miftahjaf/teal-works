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
        public TEXDraw QuestionTEX;
        public TEXDraw tempQuestionTEX;
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
                    increment = 7;
                }
                else if (Queslevel == 3)
                {
                    increment = 10;
                }
                else if (Queslevel == 4)
                {
                    increment = 13;
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
			for (int i = 1; i <= 2; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
		}

		void AnimateThreeChoiceOptionCorrect(string ans)
		{
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


            subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition = new Vector2(subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition.x, -50);

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
            subQuestionTEX.gameObject.SetActive(false);
            QuestionTEX.gameObject.SetActive(false);
            tempQuestionTEX.gameObject.SetActive(false);
            QuestionText.gameObject.SetActive(true);
            MCQ.gameObject.SetActive(false);
            ThreeChoice.gameObject.SetActive(false);
            numPad.SetActive(true);
            if (Queslevel > scorestreaklvls.Length)
            {
                level = UnityEngine.Random.Range(1, scorestreaklvls.Length + 1);
            }
           
            #region L1
            if (level == 1)
            {
               
                selector = GetRandomSelector(1, 5);
                subQuestionText.text = "";
              
                if (selector == 1)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                   // subQuestionText.gameObject.SetActive(true);
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionText.text = "Choose the variable from the expression:";
                    int temp = Random.Range(1, 4);
                    if (temp == 1)
                    {
                        QuestionTEX.text = coeff1 + " + " + coeff2 + "x";
                        Answer = "x";
                    }
                    else if(temp == 2)
                    {
                        QuestionTEX.text = coeff1 + " + " + coeff2 + "y";
                        Answer = "y";
                    }
                    else
                    {
                        QuestionTEX.text = coeff1 + " + " + coeff2 + "z";
                        Answer = "z";
                    }
                   
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    coeff4 = Random.Range(10, 50);
                   // subQuestionText.gameObject.SetActive(true);
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionText.text = "Choose the constant from the expression:";
                    int temp = Random.Range(1, 3);
                    if (temp == 1)
                    {
                        QuestionTEX.text = coeff3 + "x + " + coeff4 + "y + " + coeff1 + " + " + coeff2 + "z";
                    }
                    else if(temp == 2)
                    {
                        QuestionTEX.text = coeff3 + "x^2 + " + coeff4 + "y + "  + coeff2 + "z + " + coeff1 ;
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
                    QuestionTEX.gameObject.SetActive(true);
                    expression1 = coeff1 + "x^{" + pow1 + "}";
                    int temp = Random.Range(1, 4);
                    if (temp == 1)
                    {
                        expression1 += " + " + coeff2 + "y^{" + pow2 + "} + " + coeff3 + "xy(x+y)";
                        Answer = "4";
                    }
                    else if (temp == 2)
                    {
                        expression1 += " + " + coeff3 + "xy(x+y)";
                        Answer = "3";
                    }
                    else 
                    {
                        expression1 += " + " + coeff2 + "(x+y) + " + coeff3 + "xy(x+y)";
                        Answer = "5";
                    }
                    QuestionTEX.text = expression1;
                   
                }
                
                else if (selector == 4)
                {
                    QuestionTEX.gameObject.SetActive(true);
					GeneralButton.gameObject.SetActive(false);
                    MCQ.gameObject.SetActive(true);
                    numPad.SetActive(false);
                    QuestionText.text = "State True or False :";
                    int temp = Random.Range(1, 3);
                    if (temp == 1)
                    {
                        expression3 = "\\frac{2}{3}xy\\^2, -\\frac{1}{3}x^2y and \\frac{2}{3}xy\\^2 are like terms";
                        Answer = "False";
                    }
                    else
                    {
                        expression3 = "\\frac{2}{3}xy\\^2, -\\frac{1}{3}xy\\^2 and \\frac{2}{3}xy\\^2 are like terms";
                        Answer = "True";
                    }
                    QuestionTEX.text = expression3;
                   
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<TEXDraw>().text = "True";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<TEXDraw>().text = "False";
                }
            }
            #endregion L1
            #region L2
            else if (level == 2)
            {
                selector = GetRandomSelector(1, 6);
                subQuestionText.text = "";
               
                if (selector == 1)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                   // subQuestionText.gameObject.SetActive(true);
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionText.text = "Find the unlike term from the expression :";
                    int temp = Random.Range(1, 3) ;
                    if(temp == 1)
                    {
                        QuestionTEX.text = coeff1 + "xy^{2} + " + coeff2 + "x^{2}y + " + coeff3 + "xy^{2}";
                    }else
                    {
                        QuestionTEX.text = coeff1 + "xy^{2} + " + coeff3 + "xy^{2} + " + coeff2 + "x^{2}y" ;
                    }
                   
                    Answer = coeff2 + "x^{2}y";
                }
                else if (selector == 2)
                {
                    QuestionTEX.gameObject.SetActive(true);
                    coeff1 = Random.Range(2, 50);
					if(Random.Range(1,3) == 1)
						coeff1 *= -1;
                    pow1 = Random.Range(2, 6);
					pow2 = Random.Range(2, 6);
					pow3 = Random.Range(2, 6);
                    QuestionText.text = "Write the numerical co-efficient of :";
                    expression3 = coeff1 + "x\\^{" + pow1 + "}y\\^{" + pow2 + "}z\\^{" + pow3 + "}";
                    QuestionTEX.text = expression3;
                    Answer = coeff1.ToString();
                }
                else if (selector == 3)
                {
					QuestionTEX.gameObject.SetActive(true);
                    coeff1 = Random.Range(2,10);
					if(Random.Range(1,3) == 1)
						coeff1 *= -1;
                    pow1 = Random.Range(2, 10);
                    pow2 = Random.Range(2, 10);
                    pow3 = Random.Range(2, 10);
					QuestionText.text = "Write the coefficient of :";
                    expression1 = coeff1 + "x\\^{" + pow1 + "}y\\^{" + pow2 + "}z\\^{" + pow3 + "}";
                    expression3 = "x\\^{" + pow1 + "}";
					QuestionTEX.text = expression3 + " in " + expression1;
                    Answer = coeff1 + "y^{" + pow2 + "}z^{" + pow3 + "}";

                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(2, 100);
					if(Random.Range(1,3) == 1)
						coeff1 *= -1;
                    a = Random.Range(2, 10);
                    b = Random.Range(2, 10);
                    c = Random.Range(2, 10);
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionText.text = "Write the coefficient of :";
                    int temp = Random.Range(1, 3);
                    if (temp == 1)
                    {
                        QuestionTEX.text = "x^{" + a + "}y^{" + b + "} in " + coeff1 + "x^{" + a + "}y^{" + b + "}z^{" + c + "}";
                        Answer = coeff1.ToString() + "z^{" + c + "}";
                    }
                    else if(temp ==2){
                        QuestionTEX.text = "x^{" + a + "}z^{" + c + "} in " + coeff1 + "x^{" + a + "}y^{" + b + "}z^{" + c + "}";
                        Answer = coeff1.ToString() + "y^{" + b + "}";
                    }
                    else
                    {
                        QuestionTEX.text = "x^{" + a + "}y^{" + b + "}z^{" + c + "} in " + coeff1 + "x^{" + a + "}y^{" + b + "}z^{" + c + "}";
                        Answer = coeff1.ToString() ;
                    }
                }
                else if (selector == 5)
                {
                    QuestionTEX.gameObject.SetActive(true);
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
                        expression3 = coeff1 + "\\frac{x}{y}";
                        Answer = "Monomial";
                    }
                    else if(temp == 2)
                    {
                        expression3 = coeff1 + "\\frac{x}{y} + " + coeff2 + "x + 13x";
                        Answer = "Binomial";
                    }
                    else
                    {
                        expression3 = coeff1 + "\\frac{x}{y} + " + coeff2 + "\\frac{y}{x} + x";
                        Answer = "Trinomial";
                    }
                    QuestionText.text = "Is the following a monomial, binomial or trinomial?";
                    QuestionTEX.text = expression3;
                    
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
                subQuestionText.text = "";
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
                    //subQuestionText.gameObject.SetActive(true);

                    QuestionText.text = "Write the exponential form of the following expression :";
                    QuestionTEX.gameObject.SetActive(true);

                    QuestionTEX.text = coeff1.ToString();

                    for (int i = 0; i < a; i++)
                    {
                        QuestionTEX.text += "\\times x ";
                    }
                    QuestionTEX.text += "\\times " + coeff2.ToString();
                    for (int i = 0; i < b; i++)
                    {
                        QuestionTEX.text += "\\times y ";
                    }
                    QuestionTEX.text += "\\times " + coeff3.ToString();
                    for (int i = 0; i < c; i++)
                    {
                        QuestionTEX.text += "\\times z ";
                    }


					Answer = coeff1*coeff2*coeff3 + "x^{" + a + "}" + "y^{" + b + "}" + "z^{" + c + "}";
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(2, 100);
                    coeff3 = Random.Range(2, 100);
                    //subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Find the degree of :";
                    QuestionTEX.gameObject.SetActive(true);
                    a = Random.Range(2, 10);
                    int temp = Random.Range(1, 3);
                    if(temp == 1)
                        QuestionTEX.text = coeff1 + "x^" + a + " + " + coeff2 + "x + " + coeff3;
                    else QuestionTEX.text = coeff2 + "x + " + coeff1 + "x^" + a + " + " + coeff3;
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
                    //subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Find the degree of :";
                    QuestionTEX.gameObject.SetActive(true);
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
                    QuestionTEX.text = coeff1 + "x^2 + \\frac{" + 1 + "}{" + coeff2 + "}y^" + c + " + " + coeff3 + "x^" + a + "y^" + b + " + " + coeff4 + "." + coeff5 + "x^" + c + "y^" + d + " +" + coeff6;
                    Answer = (a + b).ToString();
                }
                else if (selector == 4)
                {
                    subQuestionText.gameObject.SetActive(true);
                    coeff1 = Random.Range(2, 100);
                    coeff2 = Random.Range(2, 100);
					QuestionText.text = "Express in algebraic form :";
                    expression3 = coeff1 + " times x subtracted from " + coeff1 + " times y is equal to " + coeff2;
                    subQuestionText.text = expression3;
                    string[] answer = new string[4];
                    
                    answer[0] = coeff1 + "y-" + coeff1 + "x=" + coeff2;
					answer[1] = "-" + coeff1 + "x" + coeff1 + "y=" + coeff2;
					answer[2] = coeff1 + "y=" + coeff1 + "x+" + coeff2;
                    Answerarray = answer; 
					Answer = coeff1 + "y-" + coeff1 + "x=" + coeff2;
                }
                else if (selector == 5)
                {
					coeff1 = Random.Range(2, 10);
                    coeff2 = Random.Range(2, 10);
					while (coeff1 == coeff2)
						coeff2 = Random.Range(2, 10);
                    coeff3 = Random.Range(1, 100);
					QuestionText.text = "Express in algebraic form :";
                    subQuestionText.gameObject.SetActive(true);
                    expression3 = coeff1 + "x added to -" + coeff2 + "x gives " + coeff3;
                    subQuestionText.text = expression3;
                    string[] answer = new string[4];
                   
                    answer[0] = coeff1-coeff2 + "x=" + coeff3;
                    answer[1] = coeff1 + "x=" + coeff2 + "x+" + coeff3;
                    answer[2] = coeff1 + "x=" + coeff3 + coeff2 + "x";
                    Answerarray = answer;
					if (coeff1 - coeff2 ==1)
						Answer = "x=" + coeff3;
					else if( coeff1 - coeff2 == -1)
						Answer = "-x=" + coeff3;
					else
                    	Answer = coeff1 - coeff2 + "x=" + coeff3;
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
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionTEX.text = coeff1 + "x + " + coeff2 + "y + " + coeff3 + "z^{2}, " + coeff4 + "x - " + coeff5 + "y + " + coeff6 + "z^{2}";
					Answer = (coeff1 + coeff4) + "x" + (coeff2 - coeff5 < 0 ? (coeff2 - coeff5).ToString() : ("+" + (coeff2 - coeff5).ToString())) + "y+" + (coeff3 + coeff6) + "z^{2}";
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
					QuestionTEX.gameObject.SetActive(true);
					QuestionTEX.text = coeff1 + "x " + (coeff2 > 0 ? "+" : "") + coeff2 + "y " + (coeff3 > 0 ? "+" : "") + coeff3 + "z^{2} from " + coeff4 + "x " + (coeff5 > 0 ? "+" : "") + coeff5 + "y " + (coeff6 > 0 ? "+" : "") + coeff6 + "z^{2}";
					Answer = "";
					if (coeff4 - coeff1 == 1)
						Answer += "x";
					else if (coeff4 - coeff1 == -1)
						Answer += "-x";
					else if (coeff4 - coeff1 == 0)
						Answer += "";
					else
						Answer += (coeff4 - coeff1 > 0 ? "+" : "") + (coeff4 - coeff1) + "x";
					
					if (coeff5 - coeff2 == 1)
						Answer += "+y";
					else if (coeff5 - coeff2 == -1)
						Answer += "-y";
					else if (coeff5 - coeff2 == 0)
						Answer += "";
					else
						Answer += (coeff5 - coeff2 > 0 ? "+" : "") + (coeff5 - coeff2) + "y";

					if (coeff6 - coeff3 == 1)
						Answer += "+z^{2}";
					else if (coeff6 - coeff3 == -1)
						Answer += "-z^{2}";
					else if (coeff6 - coeff3 == 0)
						Answer += "";
					else
						Answer += (coeff6 - coeff3 > 0 ? "+" : "") + (coeff6 - coeff3) + "z^{2}";
					
					while (Answer[0] == '+')
						Answer = Answer.Substring(1);


				}
                else if (selector == 3)
                {
                    QuestionTEX.gameObject.SetActive(true);

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
                    QuestionTEX.text = coeff1 + "x^4+" + coeff2 + "x^3+" + coeff3 + "x^2+" + coeff4 + "x+" + coeff5 + " from " + a + "x^4+" + b + "x^3+" + c + "x^2+" + d + "x+" + e;
                    Answer = (a - coeff1) + "x^{4}";
                    if (b - coeff2 > 0)
                    {
                        Answer += "+" + (b - coeff2) + "x^{3}";
                    }
                    else
                    {
                        Answer += (b - coeff2) + "x^{3}";
                    }
                    if ((c - coeff3) > 0)
                    {
                        Answer += "+" + (c - coeff3) + "x^{2}";
                    }
                    else
                    {
                        Answer += (c - coeff3) + "x^{2}";
                    }
                    if ((d - coeff4) > 0)
                    {
                        Answer += "+" + (d - coeff4) + "x";
                    }
                    else
                    {
                        Answer += (d - coeff4) + "x";
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
                    QuestionTEX.gameObject.SetActive(true);

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

                    QuestionTEX.text = coeff1 + "x ";
                    if (coeff2 > 0)
                        QuestionTEX.text +=  "+" + coeff2 + "y ";
                    else QuestionTEX.text +=   coeff2 + "y ";
                    if (coeff3 > 0)
                        QuestionTEX.text += "+" + coeff3 + "x^2y^2 ";
                    else QuestionTEX.text += coeff3 + "x^2y^2 ";
                    if (coeff4 > 0)
                        QuestionTEX.text += "+" + coeff4 + "y ";
                    else QuestionTEX.text += coeff4 + "y ";
                    if (coeff5 > 0)
                        QuestionTEX.text += "+" + coeff5 + "x ";
                    else QuestionTEX.text += coeff5 + "x ";
                    if (a > 0)
                        QuestionTEX.text += "+" + a + "y ";
                    else QuestionTEX.text += a + "y ";
                    if (b > 0)
                        QuestionTEX.text += "+" + b + "x^2y^2 ";
                    else QuestionTEX.text += b + "x^2y^2 ";
                    if (c > 0)
                        QuestionTEX.text += "+" + c + "x ";
                    else QuestionTEX.text += c + "x ";
                    if (d > 0)
                        QuestionTEX.text += "+" + d + "x^2y^2 ";
                    else QuestionTEX.text += d + "x^2y^2 ";

					Answer = coeff1 + coeff5 + c == 0 ? "" : ((coeff1 + coeff5 + c) + "x");
                    if (coeff2 + coeff4 + a > 0)
                        Answer += "+" + (coeff2 + coeff4 + a) + "y";
					else Answer += coeff2 + coeff4 + a == 0 ? "" : ((coeff2 + coeff4 + a) + "y");
					if (coeff3 + b + d > 0)
						Answer += "+" + (coeff3 + b + d) + "x^2y^2";
					else Answer += coeff3 + b + d == 0 ? "" : ((coeff3 + b + d) + "x^2y^2");

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
                    QuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);
					QuestionTEX.text = "B : " + coeff1 + "x^3 " + (coeff2 < 0 ? "" : "+") + coeff2 + "x^2y^2 " +(coeff3 < 0 ? "" : "+") + coeff3 + "z";
					tempQuestionTEX.text = "A : " + coeff4 + "x^3 " + (coeff5 < 0 ? "" : "+") + coeff5 + "x^2y^2 " + (coeff6 < 0 ? "" : "+") + coeff6 + "z";

					Answer = (coeff4 - coeff1) + "x^{3}" + (coeff5 - coeff2 < 0 ? "" : "+") + (coeff5 - coeff2) + "x^{2}y^{2}" + (coeff6 - coeff3 < 0 ? "" : "+") + (coeff6 - coeff3) + "z";
                }

            }
            #endregion L4
            #region L5
            else if (level == 5)
            {
                selector = GetRandomSelector(1, 5);
                QuestionText.text = "";
                subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition = new Vector2(subQuestionTEX.gameObject.GetAddComponent<RectTransform>().anchoredPosition.x, -110);
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
                    //subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Subtract A from the sum of B and C.";
                    QuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);
					QuestionTEX.text = "B : " + coeff1 + "x^2 " + (coeff2 < 0 ? coeff2.ToString() : ("+ " + coeff2.ToString())) + "xy " + (coeff3 < 0 ? coeff3.ToString() :( "+ " + coeff3.ToString())) + "y^2";
					subQuestionTEX.text = "A : " + coeff4 + "x^2 " + (coeff5 < 0 ? coeff5.ToString() : ("+ " + coeff5.ToString())) + "xy " + (coeff6 < 0 ? coeff6.ToString() : ("+ " + coeff6.ToString())) + "y^2";
					tempQuestionTEX.text = "C : " + a + "x^2 " + (b < 0 ? b.ToString() : ("+ " + b.ToString())) + "xy " + (c < 0 ? c.ToString() : ("+ " + c.ToString())) + "y^2";

					if (coeff1 + a - coeff4 == 1)
						Answer = "x^2";
					else if (coeff1 + a - coeff4 == -1)
						Answer = "-x^2";
					else if (coeff1 + a - coeff4 == 0)
						Answer = "";
					else
                    	Answer = (coeff1 + a - coeff4) + "x^2";
                    if (coeff2 + b - coeff5 > 0)
                    {
						if (coeff2 + b - coeff5 == 1)
							Answer += "+xy";
						else
                        	Answer += "+" + (coeff2 + b - coeff5) + "xy";
                    }
                    else
                    {
						if (coeff2 + b - coeff5 == -1)
							Answer += "-xy";
						else
                        	Answer += (coeff2 + b - coeff5) + "xy";
                    }
                    if ((coeff3 + c - coeff6) > 0)
                    {
						if (coeff3 + c - coeff6 == 1)
							Answer += "+y^2";
						else
                        	Answer += "+" + (coeff3 + c - coeff6) + "y^2";
                    }
                    else
                    {
						if (coeff3 + c - coeff6 == -1)
							Answer += "-y^2";
						else
                        	Answer += (coeff3 + c - coeff6) + "y^2";
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

                    // subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Find the perimeter of a rectangle whose length and breadth are given as A and B.";
                    QuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);

					QuestionTEX.text = "B : " + coeff4 + "x " + (coeff5 < 0 ? coeff5.ToString() : ("+ " + coeff5.ToString())) + "y " + (coeff6 < 0 ? coeff6.ToString() : ("+ " + coeff6.ToString())) + "z";
					subQuestionTEX.text = "A : " + coeff1 + "x " + (coeff2 < 0 ? coeff2.ToString() : ("+ " + coeff2.ToString())) + "y " + (coeff3 < 0 ? coeff3.ToString() : ("+ " + coeff3.ToString())) + "z";

					Answer = (2*(coeff1 + coeff4)).ToString() + "x" + (coeff2 + coeff5 > 0 ? "+" : "") + (2*(coeff2 + coeff5)).ToString() + "y" + (coeff3 + coeff6 > 0 ? "+" : "") + (2*(coeff3 + coeff6)).ToString() + "z";
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


                    //  subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Find the semiperimeter of a triangle whose sides are given as A, B and C.";
                    QuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);

					subQuestionTEX.text = "A : " + coeff1 + "x " + (coeff2 > 0 ? "+" : "") + coeff2 + "y " + (coeff3 > 0 ? "+" : "") + coeff3 + "z";
					QuestionTEX.text = "B : " + coeff4 + "x " + (coeff5 > 0 ? "+" : "") + coeff5 + "y " + (coeff6 > 0 ? "+" : "") + coeff6 + "z";
					tempQuestionTEX.text = "C : " + a + "x " + (b > 0 ? "+" : "") + b + "y " + (c > 0 ? "+" : "") + c + "z";

					Answer = (coeff1 + coeff4 + a) / 2 + "x" + (coeff2 + coeff5 + b > 0 ? "+" : "") + (coeff2 + coeff5 + b) / 2 + "y" + (coeff3 + coeff6 + c > 0 ? "+" : "") + (coeff3 + coeff6 + c) / 2 + "z";
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
					


                    // subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "The semiperimeter of a triangle is given by A and the two sides are B and C. Determine the third side.";
                    QuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);

					subQuestionTEX.text = "A : " + (coeff1 + coeff4 + a) / 2 + "x" + (coeff2 + coeff5 + b > 0 ? "+" : "") + (coeff2 + coeff5 + b) / 2 + "y" + (coeff3 + coeff6 + c > 0 ? "+" : "") + (coeff3 + coeff6 + c) / 2 + "z";
					QuestionTEX.text = "B : " + coeff4 + "x " + (coeff5 > 0 ? "+" : "") + coeff5 + "y " + (coeff6 > 0 ? "+" : "") + coeff6 + "z";
					tempQuestionTEX.text = "C : " + coeff1 + "x " + (coeff2 > 0 ? "+" : "") + coeff2 + "y " + (coeff3 > 0 ? "+" : "") + coeff3 + "z";

					Answer = a + "x " + (b > 0 ? "+" : "") + b + "y " + (c > 0 ? "+" : "") + c + "z";
                }
            }

            #endregion L5

			#region L6
			else if (level == 6)
			{
				selector = GetRandomSelector(1, 5);
				QuestionTEX.gameObject.SetActive(true);
				subQuestionTEX.gameObject.SetActive(false);

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
					QuestionTEX.text = "x^{3}" + (coeff1 > 0 ? "+" : "") + coeff1 + "x" + (coeff2 > 0 ? "+" : "") + coeff2 + " by x^{2}" + (coeff3 > 0 ? "+" : "") + coeff3 + "x" + (coeff4 > 0 ? "+" : "") + coeff4;
					int No1 = (coeff3*coeff3) + coeff1 - coeff4;
					int No2 = coeff2 + (coeff3*coeff4);
					if(No1 == 0)
						Answer = No2.ToString();
					else if(No1 == 1)
						Answer = "x" + No2.ToString();
					else if(No1 == -1)
						Answer = "-x" + No2.ToString();
					else
						Answer = No1 + "x" + (No2 > 0 ? "+" : "") + No2;
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
					QuestionTEX.text = "\\frac{x^{"+coeff1+"}y^{"+coeff2+"}z^{"+coeff3+"}}{x^{"+coeff4+"}y^{"+coeff5+"}z^{"+coeff6+"}}";
					Answer = "x^{" + (coeff1-coeff4) + "}y^{" + (coeff2-coeff5) + "}z^{" + (coeff3-coeff6) + "}";
				}
				else if (selector == 3)
				{
					QuestionTEX.gameObject.SetActive(false);
					coeff1 = Random.Range(2, 8); 
					coeff2 = Random.Range(2, 8);
					coeff3 = Random.Range(2, 8); 
					coeff4 = Random.Range(2, 8); 
					coeff5 = Random.Range(2, 8); 
					coeff6 = Random.Range(2, 8); 
					QuestionText.text = "The sides of a rectangle are given by " + coeff1 + "x+" + coeff2 + "y+" + coeff3 + "z and " + coeff4 + "x+" + coeff5 + "y+" + coeff6 + "z. Find its area.";
					Answer = (coeff1*coeff4) + "x^{2}+" + (coeff2*coeff5) + "y^{2}+" + (coeff3*coeff6) + "z^{2}+" + (coeff1*coeff5 + coeff2*coeff4) + "xy+" + (coeff2*coeff6 + coeff3*coeff5) + "yz+" + (coeff1*coeff6 + coeff3*coeff4) + "xz";
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range(5, 15) * 2;
					coeff2 = Random.Range(1, coeff1/2);
					coeff3 = Random.Range(1, 3);
					coeff4 = coeff1 - (coeff2+coeff3+1);
					coeff5 = Random.Range(2, 6);
					coeff6 = Random.Range(6, 9);
					QuestionText.text = "Find x :";
					QuestionTEX.text = "(\\frac{" + coeff5 + "}{" + coeff6 + "})\\^{-" + coeff2 + "}\\times(\\frac{" + coeff5 + "}{" + coeff6 + "})\\^{-" + coeff3 + "}\\times(\\frac{" + coeff5 + "}{" + coeff6 + "})\\^{-" + coeff4 + "} = (\\frac{" + coeff5 + "}{" + coeff6 + "})\\^{1-2x}";
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
                else {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
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
                if (checkLastTextFor(new string[1] { "x" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                userAnswerLaText.text += "x";
            }
            else if (value == 16)
            {   // y
                upflag = 0;
                numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");
                if (checkLastTextFor(new string[1] { "y" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                userAnswerLaText.text += "y";
            }
            else if (value == 17)
            {   // z
                upflag = 0;
                numPad.transform.Find("PanelLayer").Find("^").gameObject.GetChildByName<Image>("Image").color = CerebroHelper.HexToRGB("191923");
                if (checkLastTextFor(new string[1] { "z" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                userAnswerLaText.text += "z";
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

