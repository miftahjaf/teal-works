using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
    public class SimpleEquations : BaseAssessment
    {
        public TEXDraw QuestionTEX;
        private string Answer;
        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
        private int coeff6;
        private float x, y, z, a, b, c;
        private string expression1;
        private string expression2;
        private string expression3;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "SE06", "S01", "A01");
                                    
            scorestreaklvls = new int[3];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;

            coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = coeff6 = 0;
            x = y = z = a = b = c = 0.0f;

            Answer = "";
            GenerateQuestion();
        }

        public override void SubmitClick()
        {
            if (ignoreTouches || userAnswerText.text == "")
            {
                return;
            }
            int increment = 0;
            //var correct = false;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            var correct = true;
            bool isone = true;

            questionsAttempted++;
            updateQuestionsAttempted();
         
            var  answerSplits = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
            var  userAnswerSplits = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);

            if (Answer.Contains(","))
            {
                answerSplits = Answer.Split(new string[] { "," }, System.StringSplitOptions.None);
                userAnswerSplits = userAnswerText.text.Split(new string[] { "," }, System.StringSplitOptions.None);
            }
            if(answerSplits.Length == 2 && (answerSplits[1].Contains("1.5")))
            {
                int tempuser = int.Parse(userAnswerSplits[0]) + int.Parse(userAnswerSplits[1]);
                int tempans = (int)(float.Parse(answerSplits[0]) + float.Parse(answerSplits[1]));
                if (tempans == tempuser)
                {
                    correct = true;
                  
                    isone = false;
                }
                else
                {
                    Answer = "x , " + tempans + "- x"; 
                }
            }
            if (isone)
            {
                if (answerSplits.Length == userAnswerSplits.Length)
                {
                    for (var i = 0; i < answerSplits.Length; i++)
                    {
                        float answer = 0;
                        float userAnswer = 0;

                        if (float.TryParse(answerSplits[i], out answer))
                        {
                            answer = float.Parse(answerSplits[i]);
                        }
                        else {
                            correct = false;
                            break;
                        }
                        if (float.TryParse(userAnswerSplits[i], out userAnswer))
                        {
                            userAnswer = float.Parse(userAnswerSplits[i]);
                        }
                        else {
                            correct = false;
                            break;
                        }
                        if (answer != userAnswer)
                        {
                            correct = false;
                            break;
                        }
                    }
                }
                else {
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
                    increment = 10;
                }
                else if (Queslevel == 3)
                {
                    increment = 15;
                }
                else if (Queslevel == 4)
                {
                    increment = 15;
                }


                UpdateStreak(8, 12);

                updateQuestionsAttempted();
                StartCoroutine(ShowCorrectAnimation());
            }
            else {
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
                userAnswerText.text = "";
                userAnswerText.color = MaterialColor.textDark;
                ignoreTouches = false;
            }
            else {
                userAnswerText.text = Answer.ToString();
                userAnswerText.color = MaterialColor.green800;
            }
            ShowContinueButton();
        }

        protected override IEnumerator ShowCorrectAnimation()
        {
            userAnswerText.color = MaterialColor.green800;
            var config = new GoTweenConfig()
                .scale(new Vector3(1.1f, 1.1f, 1f))
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


			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
            
            #region level1
            if (level == 1)
            {
                int sublevel = Random.Range(0, 2);
              
                if (sublevel == 0) // addition only
                {
                selector = GetRandomSelector(1, 4);
                    selector = 1;
                    if (selector == 1)
                    {
                        coeff1 = Random.Range(-2, 2);
                        if (coeff1 == 0)
                            coeff1 = 1;
                        a = Random.Range(-5, 5);
                        b = Random.Range(-2, 2);


                        if (coeff1 == 1)
                            expression3 = "x + (" + b + ") = " + a;
                        else if (coeff1 == -1)
                            expression3 = "-x + (" + b + ") = " + a;
                        else expression3 = coeff1 + "x + (" + b + ") = " + a;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns = (int)a - (int)b;
                        
                        int tempAns1 = coeff1;
                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                    }

                    if (selector == 2)
                    {
                        coeff1 = Random.Range(-9, 9);
                        coeff2 = Random.Range(-5, 5);
                        coeff3 = Random.Range(-9, 9);
                        if (coeff3 == 0)
                            coeff3 = Random.Range(-9,9);

                        if (coeff1 >= 0)
                             expression1 =  " + " + coeff1 + "\\frac{" + coeff2 + "}{" +coeff3+ "}";
                        else expression1 = coeff1 + "\\frac{" + coeff2 + "}{" + coeff3 + "}";
                       

                        if (coeff1 == 0)
                            coeff1 = 1;
                        a = Random.Range(-5, 5);
                       


                        if (coeff1 == 1)
                            expression3 = "x " + expression1 + " = " + a;
                        else if (coeff1 == -1)
                            expression3 = "-x " + expression1 + " = " + a;
                        else expression3 = coeff1 + "x " + expression1 + " = " + a;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns = (int)a*(coeff3) - (coeff1)*(coeff2);
                        int tempAns1 = coeff1 * coeff3;
                        
                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                    }

                    if (selector == 3)
                    {
                        coeff1 = Random.Range(-5, 5);
                        coeff2 = Random.Range(-5, 5);

                        if (coeff1 == 0)
                            coeff1 = 1;
                        a = Random.Range(-5, 5);
                        b = Random.Range(-5, 5);


                        if (coeff1 == 1)
                            expression1 = "x";
                        else if (coeff1 == -1)
                            expression1 = " -x";
                        else expression1 =  coeff1 + "x";

                        if (coeff2 == 1)
                            expression3 = expression1 + " + (" + b + ") = " + a;
                        else if (coeff2 == -1)
                            expression3 = expression1 + " + (" + b + ") = -(" + a + ")";
                        else expression3 = expression1 + " + (" + b + ") = " + coeff2 + "(" + a + ")";


                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns = (int)a * (coeff2) - (int)b;
                        int tempAns1 = coeff1 ;

                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();
                    }

                }
                #region sublevel1
                if (sublevel == 1) // multiplication and addition
                {
                    selector = Random.Range(1, 8);
                   
                    if (selector == 1)
                    {
                        a = Random.Range(-9, 9);
                        b = Random.Range(-9, 9);
                        if (b == 0)
                            b = 0.1f;

                        if (b == 1)
                            expression3 = "x = " + a;
                        else if (b == -1)
                            expression3 = "-x = " + a;
                        else expression3 = b + "x =" + a;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns = (int)a;
                        int tempAns1 = (int)b;

                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();
                    }
                    if (selector == 2)
                    {
                        b = Random.Range(-9, 9);
                        coeff1 = Random.Range(-5, 5);
                        coeff2 = Random.Range(-5, 5);

                        expression1 = "\\frac{" + coeff1 + "}{" + coeff2 + "}";

                        if (b == 0)
                            b = -4;

                        if (b == 1)
                            expression3 = "x = " + expression1;
                        else if (b == -1)
                            expression3 = "-x = " + expression1;
                        else expression3 = b + "x =" + expression1;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns = coeff1;
                        int tempAns1 = (int)b * coeff2;

                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                    }
                    if (selector == 3)
                    {
                        a = Random.Range(-5, 5);
                        coeff1 = Random.Range(-9, 9);
                        if (coeff1 == 0)
                            coeff1 = 1;
                        if (coeff1 > 0)
                            expression3 = "\\frac{x}{" + coeff1 + "} =" + a;
                        else expression3 = "\\frac{ -x}{" + Mathf.Abs(coeff1) + "} =" + a;

                        QuestionText.text = "Solve the following equation. And give the value of x";// in fraction";

                        int tempAns = (int)a * coeff1;
                        //   int tempAns1 = (int)b * ;

                        Answer = tempAns.ToString();// + "/" + tempAns1.ToString();

                    }
                    if (selector == 4)
                    {
                        a = Random.Range(-99, 99);
                        coeff1 = (int)Random.Range(-9, 9);
                        if (coeff1 == 0)
                            coeff1 = 6;
                        expression3 = a + " = " + "\\frac{" + coeff1 + "}{y}";

                        QuestionText.text = "Solve the following equation. And give the value of y in fraction";

                        int tempAns = coeff1;
                        int tempAns1 = (int)a;

                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                    }
                    if (selector == 5)
                    {
                        coeff1 = Random.Range(-9, 9);
                        coeff2 = (int)Random.Range(-9, 9);
                        coeff3 = (int)Random.Range(-9, 9);

                        if (coeff1 == 0)
                            coeff1 = 4;
                        if (coeff3 == 0)
                            coeff3 = 16;

                        expression1 = "\\frac{" + coeff2 + "}{" + coeff3 + "}";
                        expression2 = "\\frac{x}{" + coeff1 + "}";
                        expression3 = expression1 + " = " + expression2;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns = coeff1*coeff2;
                        int tempAns1 = coeff3;

                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                    }

                    if (selector == 6)
                    {
                        coeff1 = Random.Range(-9, 9);
                        coeff2 = Random.Range(-9, 9);
                        coeff3 = Random.Range(-9, 9);
                        coeff4 = Random.Range(-9, 9);
                        a = Random.Range(-20, 20);
                        if (coeff2 == 0)
                            coeff2 = 4;
                        if (coeff4 == 0)
                            coeff4 = 16;

                        if (coeff1 == 1)
                            expression1 = "x";
                        else if (coeff1 == -1)
                            expression1 = " -x";
                        else expression1 = coeff1 + "x";

                        expression1 = "\\frac{" + expression1 + "}{" + coeff2 + "}";

                        if (a > 0)
                            expression2 = "(x +" + a + ")";
                        else expression2 = "(x " + a + ")";

                        if (coeff3 == -1)
                            expression2 = "-" + expression2;
                        else expression2 = coeff3 + expression2;

                        expression2 = "\\frac{" + expression2 + "}{" + coeff4 + "}";
                        expression3 = expression1 + " = " + expression2;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns = (int)a * coeff3 * coeff2;
                        int tempAns1 = (coeff4 * coeff1) - (coeff3 * coeff2);

                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                    }

                    if (selector == 7)
                    {
                        coeff1 = Random.Range(-9, 9);
                        coeff2 = Random.Range(-100, 101);
                        a = Random.Range(-9, 9);

                        if (coeff1 == 0)
                            coeff1 = Random.Range(-9, 9);
                        if (a == 0)
                            a = Random.Range(-9, 9);

                        while (coeff2 == 0)
                              coeff2 = Random.Range(-100, 101);

                        if (coeff2 == 1)
                            expression1 = "x";
                        if (coeff2 == -1)
                            expression1 = "-x";
                        else expression1 = coeff2 + "x";

                        expression2 = "\\frac{" + coeff1 + "}{" + expression1 + "}";
                        expression3 = expression2 + " = " + a;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns =  coeff1 ;
                        int tempAns1 = (int)a * coeff2;

                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                    }
                }
                #endregion sublevel1
                    
                QuestionTEX.text = expression3;
            }
                #endregion level1
            #region level2
                if(level == 2)
                {
                    selector = GetRandomSelector(1, 6);
                   
                    if(selector == 1)
                    { // ax + b + y = cx + z
                        a = Random.Range(-9, 9);
                        b = Random.Range(-9, 9);
                        c = Random.Range(-9, 9);

                        y = Random.Range(-9, 9);
                        z = Random.Range(-9, 9);

                        while(c==0)
                            c = Random.Range(-9, 9);
                        
                    if (a == 1)
                        expression1 = "x";
                    else expression1 = a + "x";

                        if (b > 0)
                            expression1 += " + " + b;
                        else expression1 +=  b;

                        if (y > 0)
                            expression1 += " + " + y;
                        else expression1 += y;

                        expression2 = c + "x";
                        if (z > 0)
                            expression2 += " + " + z;
                        else expression2 += z;

                        expression3 = expression1 + " = " + expression2;

                    QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                    int tempAns = (int)z - (int)b - (int)y;

                    int tempAns1 = (int)a - (int)c;
                    Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                }
                    if(selector == 2)
                    {// x/coeff1 + coeff2/coeff3 = x/coefff4 + coeff5/coeff6
                        coeff1 = Random.Range(-9, 9);
                        coeff2 = Random.Range(-9, 9);
                        coeff3 = Random.Range(-9, 9);
                        coeff4 = Random.Range(-9, 9);
                        coeff5 = Random.Range(-9, 9);
                        coeff6 = Random.Range(-9, 9);

                       
                            while(coeff1 == 0)
                                coeff1 = Random.Range(-9, 9);
                            while (coeff3 == 0)
                                coeff3 = Random.Range(-9, 9);
                            while (coeff4 == 0)
                                coeff4 = Random.Range(-9, 9);
                            while (coeff6 == 0)
                               coeff6 = Random.Range(-9, 9);
                        

                        if (coeff1 < 0)
                            expression1 = "\\frac{-x}{" + Mathf.Abs(coeff1) +"}";
                        else expression1 = "\\frac{x}{" + coeff1 + "}";

                        if((coeff2*coeff3) > 0)
                            expression1 += " + (\\frac{" + (coeff2) + "}{" + (coeff3) + "})";
                        else expression1 += " - (\\frac{" + Mathf.Abs(coeff2) + "}{" + Mathf.Abs(coeff3) + "})";

                        if (coeff4 < 0)
                            expression2 = "\\frac{-x}{" + Mathf.Abs(coeff4) + "}";
                        else expression2 = "\\frac{x}{" + coeff4 + "}";

                        if ((coeff5 * coeff6) > 0)
                            expression2 += " + (\\frac{" + (coeff5) + "}{" + (coeff6) + "})";
                        else expression2 += " - (\\frac{" + Mathf.Abs(coeff5) + "}{" + Mathf.Abs(coeff6) + "})";

                        expression3 = expression1 + " = " + expression2;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                    int tempAns = ((coeff5 * coeff3) - (coeff6 * coeff2)) * (coeff4 * coeff1);
                        int tempAns1 = (coeff6 * coeff3) * (coeff4 - coeff1);

                       
                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                }
                    if(selector == 3)
                    {// (c1/c2)x + x/c3 = c4
                        coeff1 = Random.Range(-9, 9);
                        coeff2 = Random.Range(-9, 9);
                        coeff3 = Random.Range(-9, 9);
                        coeff4 = Random.Range(-9, 9);

                        if (coeff2 == 0 || coeff3 == 0)
                        {
                            while (coeff2 == 0)
                                coeff2 = Random.Range(-9, 9);
                            while (coeff3 == 0)
                                coeff3 = Random.Range(-9, 9);
                        }

                        if((coeff1* coeff2)>= 0)
                            expression1 = "\\frac{" + Mathf.Abs(coeff1) + "}{" + Mathf.Abs(coeff2) + "}x";
                        else expression1 = "- \\frac{" + Mathf.Abs(coeff1) + "}{" + Mathf.Abs(coeff2) + "}x";

                        if (coeff3 < 0)
                            expression1 += " - \\frac{x}{" + Mathf.Abs(coeff3) + "}";
                        else expression1 += " + \\frac{x}{" + coeff3 + "}";

                        expression3 = expression1 + " = " + coeff4;


                    QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                    int tempAns = coeff2 * (coeff4 * coeff3);
                    int tempAns1 = (coeff1 * coeff3) + (coeff2);


                    Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                }
                    if(selector == 4)
                    {// (c1/c2)(x + a) = (c3/c4)x + (c5/c6)

                        coeff1 = Random.Range(-9, 9);
                        coeff2 = Random.Range(-9, 9);
                        coeff3 = Random.Range(-9, 9);
                        coeff4 = Random.Range(-9, 9);
                        coeff5 = Random.Range(-9, 9);
                        coeff6 = Random.Range(-9, 9);
                        a = Random.Range(-9, 9);

                        if (coeff2 == 0 || coeff4 == 0 || coeff6 ==0)
                        {
                            while (coeff2 == 0)
                                coeff2 = Random.Range(-9, 9);
                            while (coeff4 == 0)
                                coeff4 = Random.Range(-9, 9);
                            while (coeff6 == 0)
                                coeff6 = Random.Range(-9, 9);
                        }

                        if (a >= 0)
                            expression1 = "(x +" + a + ")";
                        else expression1 = "(x" + a + ")";

                        expression1 = "\\frac{" + coeff1 + "}{" + coeff2 + "}" + expression1;

                    if (coeff3 * coeff4 > 0)
                        expression2 = "(\\frac{" + Mathf.Abs(coeff3) + "}{" + Mathf.Abs(coeff4) + "})x";
                    else expression2 = "(\\frac{" + coeff3 + "}{" + coeff4 + "})x";

                    if (coeff5 * coeff6 >0)
                        expression2 += "+ \\frac{" + Mathf.Abs(coeff5) + "}{" + Mathf.Abs(coeff6) + "}";
                    else expression2 += "+ (\\frac{" + (coeff5) + "}{" + (coeff6) + "})";

                    expression3 = expression1 + " = " + expression2;

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                        int tempAns = ((coeff5 * coeff2) - ((int)a * coeff6 * coeff1)) * coeff4;
                        int tempAns1 = ((coeff4 * coeff1) - (coeff3 * coeff2))*(coeff6);


                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();


                }
                    if(selector == 5)
                    {//c1(x + a) + c2(c3x + b) + c(c4x + y/z) = 0;
                        coeff1 = Random.Range(-9, 9);
                        coeff2 = Random.Range(-9, 9);
                        coeff3 = Random.Range(-9, 9);
                        coeff4 = Random.Range(-10, 10);
                        a = Random.Range(-9, 9);
                        b = Random.Range(-9, 9);
                        c = Random.Range(-9, 9);
                        y = Random.Range(-9, 9);
                        z = Random.Range(-9, 9);

                    while(z == 0)
                        z = Random.Range(-9, 9);

                    if (a >= 0)
                            expression1 = coeff1 + "(x +" + a + ")";
                        else expression1 = coeff1 + "(x" + a + ")";

                    if (b >= 0)
                    {
                        if (coeff2 > 0)
                            expression1 += " + " + coeff2 + "(" + coeff3 + "x +" + b + ")";
                        else expression1 += " " + coeff2 + "(" + coeff3 + "x +" + b + ")";
                    }
                    else
                    {
                        if(coeff2 > 0)
                            expression1 += " + " + coeff2 + "(" + coeff3 + "x " + b + ")";
                        else expression1 += " " + coeff2 + "(" + coeff3 + "x " + b + ")";
                    }

                    if (c >= 0)
                    {
                        if (y * z > 0)
                            expression2 = " + " + c + "(" + coeff4 + "x" + "+ \\frac{" + Mathf.Abs(y) + "}{" + Mathf.Abs(z) + "})";
                        else expression2 = " + " + c + "(" + coeff4 + "x" + "+ (\\frac{" + (y) + "}{" + (z) + "}) )";
                    }
                    else
                    {
                        if(y*z > 0)
                             expression2 = " " + c + "(" + coeff4 + "x" + "+ \\frac{" + Mathf.Abs(y) + "}{" + Mathf.Abs(z) + "})";
                        else expression2 = " " + c + "(" + coeff4 + "x" + "+ \\frac{" + y + "}{" + z + "})";
                    }

                        expression3 = expression1 + expression2 + " = " + " 0";

                        QuestionText.text = "Solve the following equation. And give the value of x in fraction";

                    int tempAns = (-1)*(((int)b * (int)z * coeff2) + ((int)a * (int)z * coeff1) + ((int)(c * y)));
                    int tempAns1 = (coeff1 + (coeff3 * coeff2) + ((int)c * coeff4)) * (int)z;


                        Answer = tempAns.ToString() + "/" + tempAns1.ToString();

                }

                    QuestionTEX.text = expression3;
                }
            #endregion level2
            #region level3
            if(level == 3)
            {
                selector = GetRandomSelector(1, 11);
               
                if(selector == 1)
                {
                    a = Random.Range(-9, 9);
                    b = Random.Range(-99, 99);

                    QuestionText.text = "When A number is increased by " + a + " gives " + b + ". Find that number.";

                    int tempAns = (int)b - (int)a;
                   
                    Answer = tempAns.ToString();

                }
                if (selector == 2)
                {
                   
                    a = Random.Range(-99, 99);

                    QuestionText.text = "One fifth of a number when added to two third of the number gives " + a + ". Find the number.";

                    int tempAns =   (int)(a * 15);
                    
                    int tempAns1 = 13;

                    Answer = tempAns.ToString() + " / " + tempAns1.ToString();


                }
               
                if (selector == 3)
                {

                    a = Random.Range(2, 999);

                    QuestionText.text = "Sum of two natural number is " + a + ". Find two numbers.";

                   
                    Answer = (a-(1.5)).ToString() + " , " + (1.5).ToString(); 

                }
                if (selector == 4)
                {
                    a = 0; b = 0;
                    while (a >= b)
                    { 
                    a = Random.Range(1, 9);
                    b = Random.Range(1, 99);
                     }

                    QuestionText.text = "If the length of a rectangular playground is " + a + "m more than twice its breadth. if the perimeter is " + b + "m. then find length and breadth";

                    int width = (int)((b - (2 * a))/6);

                    int length = (2 * width + (int)a);

                    Answer = length.ToString() + " , " + width.ToString();


                }
                if (selector == 5)
                {
                    a = 0; b = 0;
                    while ((2 * b) >= a)
                    {
                        a = Random.Range(1, 99);
                        b = Random.Range(1, 99);
                    }
                    QuestionText.text = "Vipin is " + a + " years old and his son is " + b + " years old. After how many years will Vipin's age be twice that of his son.";

                    int tempAns = (int)(a - (2 * b));

                    Answer = tempAns.ToString() ;


                }
                if (selector == 6)
                {
                    
                    a = Random.Range(1, 999);
                   

                    QuestionText.text = "If the perimeter of square is " + a + "m. Find the sum of two sides of square.";
                    float tempAns = a / 2;
                    Answer = tempAns.ToString("F2");

                }
                if (selector == 7)
                {

                    a = Random.Range(-9, 9);
                    b = Random.Range(-9, 9);
                    c = Random.Range(-9, 9);

                    QuestionText.text = "If I add " + a + " to the number " + b + " and then multiply the result by " + c + " then answer is :";
                    int tempAns = (int)((b + a) * c);

                    Answer = tempAns.ToString();

                }
                if (selector == 8)
                {
                    a = 2;
                    while(a % 2 == 0)
                        a = Random.Range(-50, 50);
                 
                    QuestionText.text = "three consecutive odd integers whose sum is " + (3 * a) + ", are : ";

                    Answer = (a - 2).ToString() + " , " + a.ToString() + " , " + (a + 2).ToString();


                }
                if (selector == 9)
                {
                    a = 7;
                    while((a + 6) % 6 != 0)
                        a = Random.Range(1, 99);

                    QuestionText.text = "Find x of the rectangle which has perimeter of " + a + "cm and its length is double than its breadth, which is (x - 1)cm .";

                    int tempAns = (int)(a + 6) / 6;
                    Answer = tempAns.ToString();


                }
                if (selector == 10)
                {
                    a = 0; b = 1; c = 0;
                    while ((c+ b+ a) % (b+1) != 0)
                    {
                        a = Random.Range(1, 99);
                        b = Random.Range(1, 10);
                        c = Random.Range(30, 99);
                    }

                    QuestionText.text = "If after " + a + " year, John's aunt will be " + b + " times older than John was last year. The sum of John's present age and his aunt's age is " + c + ". Find the present ages of both of them.(first john then aunt)";

                    int john = (int)((c + b + a) / (b + 1));
                    int aunt = (int)(c - john);

                    Answer = john.ToString() + " , " + aunt.ToString(); 

                }


            }
            #endregion level3

            CerebroHelper.DebugLog(Answer);
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
            {    //Back
                if (userAnswerText.text.Length > 0)
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
            }
            else if (value == 11)
            {   // /
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += "/";
            }
            else if (value == 12)
            {   // :
                if (checkLastTextFor(new string[1] { "-" }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += "-";
            }
            else if (value == 13)
            {   // .
                if (checkLastTextFor(new string[1] { "." }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += ".";
            }
            else if (value == 14)
            {   // ,

                userAnswerText.text += ",";
            }
        }
    }
}
