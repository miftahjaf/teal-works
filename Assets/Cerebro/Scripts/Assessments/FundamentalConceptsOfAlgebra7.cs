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
        int[] end = new int[5];
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

        protected override IEnumerator ShowWrongAnimation()
        {
            userAnswerLaText.color = MaterialColor.red800;
            Go.to(answerButton.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
            yield return new WaitForSeconds(0.5f);
            if(level == 3 && selector == 3)
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
                      
                    str += Answerarray[0];
                       
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
               
                selector = GetRandomSelector(1, 6);
                subQuestionText.text = "";
              
                if (selector == 1)
                {
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(10, 50);
                   // subQuestionText.gameObject.SetActive(true);
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionText.text = "Choose the variable from the expression:";
                    int temp = Random.Range(1, 4);
                    if (temp == 1)
                    {
                        QuestionTEX.text = coeff1 + " + " + coeff2 + "x";
                        Answer = coeff2 + "x";
                    }
                    else if(temp == 2)
                    {
                        QuestionTEX.text = coeff1 + " + " + coeff2 + "y";
                        Answer = coeff2 + coeff2 + "y";
                    }
                    else
                    {
                        QuestionTEX.text = coeff1 + " + " + coeff2 + "z";
                        Answer = coeff2 + "z";
                    }
                   
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    coeff4 = Random.Range(10, 50);
                   // subQuestionText.gameObject.SetActive(true);
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionText.text = "Choose the constant from the expression:";
                    int temp = Random.Range(1, 3);
                    if (temp == 1)
                    {
                        QuestionTEX.text = coeff3 + "x +" + coeff4 + "y +" + coeff1 + " + " + coeff2 + "z";
                    }
                    else if(temp == 2)
                    {
                        QuestionTEX.text = coeff3 + "x^2 +" + coeff4 + "y +"  + coeff2 + "z + " + coeff1 ;
                    }
                    Answer = coeff1.ToString();
                  
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    pow1 = Random.Range(1, 10);
                    pow2 = Random.Range(1, 10);

                    QuestionText.text = "The number of separate terms in the following expression:";
                    QuestionTEX.gameObject.SetActive(true);
                    expression1 = "(" + coeff1 + "x^{" + pow1 + "})";
                    int temp = Random.Range(1, 4);
                    if (temp == 1)
                    {
                        expression1 += " + (" + coeff2 + "y^{" + pow2 + "})+ (" + coeff3 + "xy(x+y))";
                        Answer = "3";
                    }
                    else if (temp == 2)
                    {
                        expression1 += " + (" + coeff3 + "xy(x+y))";
                        Answer = "2";
                    }
                    else if(temp == 3)
                    {
                        expression1 += " + (" + coeff2 + "(x+y))+ (" + coeff3 + "xy(x+y))";
                        Answer = "3";
                    }
                    else
                    {
                        expression1 += coeff3 + "xy(x+y)";
                        Answer = "1";
                    }
                    
                    QuestionTEX.text = expression1;
                   
                }
                else if (selector == 4)
                {
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionText.text = "Write the number of terms:";
                    int temp = Random.Range(1, 3);
                    coeff1 = Random.Range(1, 9);
                    if (temp == 1)
                        QuestionTEX.text = "\\frac{0}{xy}";
                    else QuestionTEX.text = "\\frac{0}{x^" + coeff1 + "y}";
                    Answer = "0";
                }
                else if (selector == 5)
                {
                    QuestionTEX.gameObject.SetActive(true);
                    MCQ.gameObject.SetActive(true);
                    numPad.SetActive(false);
                    QuestionText.text = "State True or False:";
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
                    coeff1 = Random.Range(1, 100);
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
                    coeff1 = Random.Range(-10, 10);
                    pow1 = Random.Range(-10, 10);
                    pow2 = Random.Range(-10, 10);
                    pow3 = Random.Range(-10, 10);
                    QuestionText.text = "Write the numerical co-efficient of :";
                    expression3 = coeff1 + "x\\^{" + pow1 + "}y\\^{" + pow2 + "}z\\^{" + pow3 + "}";
                    QuestionTEX.text = expression3;
                    Answer = coeff1.ToString();
                }
                else if (selector == 3)
                {
                    subQuestionText.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    QuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);
                    QuestionText.gameObject.SetActive(false);
                    coeff1 = Random.Range(-10, 10);
                    pow1 = Random.Range(2, 10);
                    pow2 = Random.Range(2, 10);
                    pow3 = Random.Range(2, 10);
                    subQuestionText.text = "For the above expression, write the co-efficient of the following:";
                    expression1 = coeff1 + "x\\^{" + pow1 + "}y\\^{" + pow2 + "}z\\^{" + pow3 + "}";
                    subQuestionTEX.text = expression1;
                    expression3 = "x\\^{" + pow1 + "}";
                    QuestionTEX.text = expression3;
                    Answer = coeff1 + "y^{" + pow2 + "}z^{" + pow3 + "}";

                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(2, 100);
                    a = Random.Range(2, 10);
                    b = Random.Range(2, 10);
                    c = Random.Range(2, 10);
                    subQuestionText.gameObject.SetActive(true);
                   // subQuestionTEX.gameObject.SetActive(true);
                    QuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);
                    QuestionText.gameObject.SetActive(false);
                    subQuestionText.text = "Write the required coefficient for following:";
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
                    numPad.SetActive(false);
                    coeff1 = Random.Range(-10, 10);
                    coeff2 = Random.Range(2, 10);
                    int temp = Random.Range(1, 3);
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
                    QuestionText.text = "Is the following a monomial, binomial or trrinomial?";
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
                    a = Random.Range(2, 10);
                    b = Random.Range(2, 10);
                    c = Random.Range(2, 10);
                    while (a + b + c > 11)
                    {
                        a = Random.Range(2, 10);
                        b = Random.Range(2, 10);
                        c = Random.Range(2, 10);
                    }
                    subQuestionText.gameObject.SetActive(true);

                    subQuestionText.text = "Write the exponential form of the following expression :";
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


                    Answer = coeff1 + "x^{" + a + "}" + coeff2 + "y^{" + b + "}" + coeff3 + "z^{" + c + "}";
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(1, 100);
                    coeff3 = Random.Range(1, 100);
                    //subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Find the degree of ";
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
                    coeff1 = Random.Range(1, 10);
                    coeff2 = Random.Range(1, 10);
                    coeff3 = Random.Range(1, 10);
                    coeff4 = Random.Range(1, 10);
                    coeff5 = Random.Range(1, 10);
                    coeff6 = Random.Range(10, 50);
                    //subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Find the degree of ";
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
                    //subQuestionText.gameObject.SetActive(true);
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(1, 100);
                    expression3 = "Sum of X and " + coeff1 + " Subtracted from " + coeff1 + " Y is equal to " + coeff2;
                    QuestionText.text = expression3;
                    string[] answer = new string[4];
                    
                     answer[0] = coeff1 + "y-x-" + coeff1 + "=" + coeff2;
                     answer[1] = coeff1 + "y-x=" + coeff2 + coeff1;
                    answer[2] = "-x+" + coeff1 + "y=" + coeff2 + coeff1;
                    answer[3] = coeff1 + "y-" + coeff1 + "-x=" + coeff2;
                    Answerarray = answer; 
                    Answer = coeff1 + "y-x-" + coeff1 + "=" + coeff2; ;
                }
                else if (selector == 5)
                {
                    coeff2 = Random.Range(1, 10);
                    coeff3 = Random.Range(1, 100);
                    //subQuestionText.gameObject.SetActive(true);
                    expression3 = coeff1 + "x added to -" + coeff2 + "x gives " + coeff3;
                    QuestionText.text = expression3;
                    string[] answer = new string[4];
                   
                     answer[0] = coeff1-coeff2 + "x=" + coeff3;
                     answer[1] = coeff1 + "x=" + coeff2 + "x+" + coeff3;
                     answer[2] = coeff1 + "x=" + coeff3 + coeff2 + "x";
                    Answerarray = answer;
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

                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    coeff4 = Random.Range(1, 100);
                    coeff5 = Random.Range(10, 50);
                    coeff6 = Random.Range(10, 50);
                    subQuestionText.gameObject.SetActive(true);
                    subQuestionText.text = "Add :";
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionTEX.text = coeff1 + "x + " + coeff2 + "y + " + coeff3 + "z^{2}, " + coeff4 + "x + " + coeff5 + "y + " + coeff6 + "z^{2}, ";
                    Answer = (coeff1 + coeff4) + "x+" + (coeff2 + coeff5) + "y+" + (coeff3 + coeff6) + "z^{2}";
                }
                else if (selector == 2)
                {
                    QuestionTEX.gameObject.SetActive(true);
                    QuestionText.text = " Subtract ";
                    float[] coeff = new float[6];
                    string[] var = new string[4];
                    string tempExp1 = "", tempExp2 = "";
                    string[] AnswerTemp = new string[3];
                    string[] exp = new string[9];
                    var[0] = "x^{2}";
                    var[1] = "y^{2}";
                    var[2] = "xy";

                    for (int i = 0; i < 6; i++)
                    {
                        coeff[i] = Random.Range(-9, 10);
                        if (coeff[i] == -1 || coeff[i] == 0)
                            coeff[i] = -1;
                        if (coeff[i] > 0)
                        {
                            if (coeff[i] == 1)
                            {
                                if ((i % 3) == 0)
                                    exp[i] = var[i % 3];
                                else
                                    exp[i] = "+" + var[i % 3];
                            }
                            else {
                                if ((i % 3) == 0)
                                    exp[i] = coeff[i] + var[i % 3];
                                else
                                    exp[i] = "+" + coeff[i] + var[i % 3];
                            }
                        }
                        else {
                            if (coeff[i] == -1)
                                exp[i] = "-" + var[i % 3];
                            else
                                exp[i] = coeff[i] + var[i % 3];

                        }
                    }
                    tempExp1 = exp[0] + exp[1] + exp[2];
                    tempExp2 = exp[3] + exp[4] + exp[5];
                    //QuestionLatext.text = tempExp1 + " from " + tempExp2;
                    QuestionTEX.text = tempExp1 + " from " + tempExp2;

                    for (int i = 0; i < 3; i++)
                    {
                        if ((coeff[i + 3] - coeff[i]) == 0)
                            AnswerTemp[i] = "";
                        else if ((coeff[i + 3] - coeff[i]) == 1)
                            AnswerTemp[i] = "+" + var[i];
                        else if ((coeff[i + 3] - coeff[i]) == -1)
                            AnswerTemp[i] = "-" + var[i];
                        else if ((coeff[i + 3] - coeff[i]) > 0)
                            AnswerTemp[i] = "+" + (coeff[i + 3] - coeff[i]).ToString() + var[i];
                        else
                            AnswerTemp[i] = (coeff[i + 3] - coeff[i]).ToString() + var[i];
                    }
                    Answer = AnswerTemp[0] + AnswerTemp[1] + AnswerTemp[2];
                }
                else if (selector == 3)
                {
                    QuestionTEX.gameObject.SetActive(true);

                    QuestionText.text = "Subtract ";
                    coeff1 = Random.Range(1, 100);
                    coeff1 = 1;
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    coeff4 = Random.Range(1, 100);
                    coeff5 = Random.Range(10, 50);
                    a = Random.Range(10, 100);
                    b = Random.Range(10, 50);
                    c = Random.Range(10, 50);
                    int d = Random.Range(1, 100);
                    int e = Random.Range(10, 50);
                    QuestionTEX.text = coeff1 + "x^4+" + coeff2 + "x^3+" + coeff3 + "x^2+" + coeff4 + "x+" + coeff5 + " from " + a + "x^4+" + b + "x^3+" + c + "x^2+" + d + "x+" + e;
                    Answer = (a - coeff1) + "x^{4}";
                    if ((b - coeff2) > 0)
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

                    QuestionText.text = "Simplify the following ";
                    coeff1 = 0;
                    coeff2 = 0;
                    coeff3 = 0;
                    coeff4 = 0;
                    coeff5 = 0;
                    a = 0;
                    b = 0;
                    c = 0;
                    int d = 0;
                    int e = 0;
                    int f = 0;
                    int g = 0;
                    while (coeff1 == 0 || coeff2 == 0 || coeff3 == 0 || coeff4 == 0 || coeff5 == 0)
                    {
                        coeff1 = Random.Range(-20, 20);
                        coeff2 = Random.Range(-20, 20);
                        coeff3 = Random.Range(-20, 20);
                        coeff4 = Random.Range(-20, 20);
                        coeff5 = coeff4;
                    }
                    while (a == 0 || b == 0 || c == 0 || d == 0 || e == 0 || f==0 || g==0)
                    {
                        a = Random.Range(-20, 20);
                        b = Random.Range(-20, 20);
                        c = Random.Range(-20, 20);
                        d = Random.Range(-20, 20);
                        e = Random.Range(-20, 20);
                        f = Random.Range(-20, 20);
                        g = f;
                    }
                    if(coeff1 + coeff5 + c == 1)           
                        c++;
                    if (coeff3 + c + d == 1)
                        d++;
                    if (coeff2 + coeff4 + a == 1)
                        a++;

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

                    Answer = (coeff1 + coeff5 + c) + "x";
                    if (coeff2 + coeff4 + a > 0)
                        Answer += "+" + (coeff2 + coeff4 + a) + "y";
                    else Answer += (coeff2 + coeff4 + a) + "y";
                    //if (coeff3 + c + d > 0)
                    //    Answer += "+" + (coeff3 + c + d) + "x^2y^2";
                    //else Answer += (coeff3 + c + d) + "x^2y^2";
                    Answer += "+1x^2y^2";

                }
                else if (selector == 5)
                {

                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(10, 50);
                    coeff3 = Random.Range(10, 50);
                    coeff4 = Random.Range(1, 100);
                    coeff5 = Random.Range(10, 50);
                    coeff6 = Random.Range(10, 50);
                    // subQuestionText.gameObject.SetActive(fa);
                    QuestionText.text = "What must be subtracted from equation(A) to get equation(B)?";
                    QuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);
                    QuestionTEX.text = "B : " + coeff1 + "x^3 + " + coeff2 + "x^2y^2 + " + coeff3 + "z";
                    tempQuestionTEX.text = "A : " + coeff4 + "x^3 + " + coeff5 + "x^2y^2 + " + coeff6 + "z";

                    Answer = (coeff1 - coeff4) + "x^{3}+" + (coeff2 - coeff5) + "x^{2}y^{2}+" + (coeff3 - coeff6) + "z";
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
                    coeff1 = Random.Range(1, 20);
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(10, 20);
                    coeff4 = Random.Range(1, 20);
                    coeff5 = Random.Range(10, 50);
                    coeff6 = Random.Range(10, 50);
                    a = Random.Range(1, 20);
                    b = Random.Range(10, 20);
                    c = Random.Range(10, 20);
                    //subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Subtract A from the sum of B and C";
                    QuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);
                    QuestionTEX.text = "B : " + coeff1 + "x^2 + " + coeff2 + "xy + " + coeff3 + "y^2";
                    subQuestionTEX.text = "A : " + coeff4 + "x^2 + " + coeff5 + "xy + " + coeff6 + "y^2";
                    tempQuestionTEX.text = "C : " + a + "x^2 + " + b + "xy + " + c + "y^2";

                    Answer = (coeff1 + a - coeff4) + "x^2";
                    if ((coeff2 + b - coeff5) > 0)
                    {
                        Answer += "+" + (coeff2 + b - coeff5) + "xy";
                    }
                    else
                    {
                        Answer += (coeff2 + b - coeff5) + "xy";
                    }
                    if ((coeff3 + c - coeff6) > 0)
                    {
                        Answer += "+" + (coeff3 + c - coeff6) + "y ^ 2";
                    }
                    else
                    {
                        Answer += (coeff3 + c - coeff6) + "y^2";
                    }

                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(1, 20);
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(10, 20);
                    coeff4 = Random.Range(1, 20);
                    coeff5 = Random.Range(10, 50);
                    coeff6 = Random.Range(10, 50);

                    // subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Find the perimeter of a rectangle whose length and breadth are given as A and B.";
                    QuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);

                    QuestionTEX.text = "B : " + coeff4 + "x + " + coeff5 + "y + " + coeff6 + "z";
                    subQuestionTEX.text = "A : " + coeff1 + "x + " + coeff2 + "y + " + coeff3 + "z";

                    Answer = 2 * (coeff1 + coeff4) + "x+" + 2 * (coeff2 + coeff5) + "y+" + 2 * (coeff3 + coeff6) + "z";
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(1, 20);
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(10, 20);
                    coeff4 = Random.Range(1, 20);
                    coeff5 = Random.Range(10, 50);
                    coeff6 = Random.Range(10, 50);
                    a = 1; b = 1; c = 1;
                    while ((coeff1 + coeff4 + a) % 2 != 0 )
                        a = Random.Range(2, 20);
                    while ((coeff2 + coeff5 + b) % 2 != 0 )
                        b = Random.Range(2, 20);
                    while ((coeff3 + coeff6 + c) % 2 != 0 )
                        c = Random.Range(2, 20);


                    //  subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "Find the semiperimeter(s) of a triangle whose sides are given as A, B and C.";
                    QuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);

                    subQuestionTEX.text = "A : " + coeff1 + "x + " + coeff2 + "y + " + coeff3 + "z";
                    QuestionTEX.text = "B : " + coeff4 + "x + " + coeff5 + "y + " + coeff6 + "z";
                    tempQuestionTEX.text = "C : " + a + "x + " + b + "y + " + c + "z";

                    Answer = (coeff1 + coeff4 + a) / 2 + "x+" + (coeff2 + coeff5 + b) / 2 + "y+" + (coeff3 + coeff6 + c) / 2 + "z";
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(1, 20);
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(10, 20);
                    coeff4 = Random.Range(1, 20);
                    coeff5 = Random.Range(10, 50);
                    coeff6 = Random.Range(10, 50);
                    a = 1; b = 1; c = 1;
                    while ((coeff1 + coeff4 + a) % 2 != 0)
                        a = Random.Range(2, 20);
                    while ((coeff2 + coeff5 + b) % 2 != 0)
                        b = Random.Range(2, 20);
                    while ((coeff3 + coeff6 + c) % 2 != 0)
                        c = Random.Range(2, 20);


                    // subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "The semiperimeter of a triangle is given by A and the two sides are B and C.Determine the third side.";
                    QuestionTEX.gameObject.SetActive(true);
                    subQuestionTEX.gameObject.SetActive(true);
                    tempQuestionTEX.gameObject.SetActive(true);

                    subQuestionTEX.text = "A : " + (coeff1 + coeff4 + a) / 2 + "x + " + (coeff2 + coeff5 + b) / 2 + "y + " + (coeff3 + coeff6 + c) / 2 + "z";
                    QuestionTEX.text = "B : " + coeff4 + "x + " + coeff5 + "y + " + coeff6 + "z";
                    tempQuestionTEX.text = "C : " + coeff1 + "x + " + coeff2 + "y + " + coeff3 + "z";

                    Answer = a + "x+" + b + "y+" + c + "z";
                }
            }

            #endregion L5

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

