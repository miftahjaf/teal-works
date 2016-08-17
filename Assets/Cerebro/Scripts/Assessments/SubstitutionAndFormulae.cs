using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;


namespace Cerebro
{
    public class SubstitutionAndFormulae : BaseAssessment
    {

        public TEXDraw QuestionTEX;
        private string Answer;
        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
        private int coeff6;
        private int x, y, z, a, b, c;
        private string expression1;
        private string expression2;
        private string expression3;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "SAF06", "S01", "A01");
                                    
            scorestreaklvls = new int[3];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;
            
            coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = coeff6 = 0;
            x = y = z = a = b = c = 0;
           
            Answer = " ";
            GenerateQuestion();
        }

        public override void SubmitClick()
        {
            if (ignoreTouches || userAnswerText.text == " ")
            {
                return;
            }
            int increment = 0;
            //var correct = false;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            var correct = true;

            questionsAttempted++;
            updateQuestionsAttempted();

            var answerSplits = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
            var userAnswerSplits = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);

            float ansflt = (float.Parse(answerSplits[0]) / float.Parse(answerSplits[1]));
            float userflt = (float.Parse(userAnswerSplits[0]) / float.Parse(userAnswerSplits[1]));
            if(answerSplits[1] != "")
            {
                if(ansflt != userflt)
                {
                    correct = false;
                   
                }
            }
            else if (answerSplits.Length == userAnswerSplits.Length)
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
                userAnswerText.text = " ";
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

                x = Random.Range(-10, 11);
                y = Random.Range(-10, 11);
                z = Random.Range(-10, 11);
                coeff1 = Random.Range(-5, 5);
                coeff2 = Random.Range(-5, 5);
                coeff3 = Random.Range(-5, 5);

                
                while (coeff1 == 0 && coeff2 == 0 && coeff3 == 0)
                   {
                       coeff1 = Random.Range(-9, 9);
                       coeff2 = Random.Range(-9, 9);
                   }
                     
                
                selector = GetRandomSelector(1, 4);
               
                expression3 = " ";
               
                #region selector1
                if (selector == 1)
                {
                    int activate = Random.Range(0, 2);
                    
                    if (activate == 1)
                    {
                        if (coeff1 == 0)
                        {
                            expression1 = " ";
                        }
                        else if (coeff1 == 1)
                        {
                            expression1 = "x";
                        }
                        else if (coeff1 == -1)
                        {
                            expression1 = "-x";
                        }
                        else
                        {
                            expression1 = coeff1 + "x";
                        }


                        if (coeff2 == 0)
                        {
                            expression2 = expression1 + " ";
                        }
                        else if (coeff2 == 1)
                        {
                            expression2 = expression1 + " + y";
                        }
                        else if (coeff2 == -1)
                        {
                            expression2 = expression1 + " - y";
                        }
                        else
                        {
                            if (coeff2 < -1)
                            {
                                expression2 = expression1 + " " + coeff2 + "y";
                            }
                            else
                            {
                                expression2 = expression1 + " + " + coeff2 + "y";
                            }
                        }


                        if (coeff3 == 0)
                        {
                            expression3 = expression2 + " ";
                        }
                        else if (coeff3 == 1)
                        {
                            expression3 = expression2 + " + z";
                        }
                        else if (coeff3 == -1)
                        {
                            expression3 = expression2 + " - z";
                        }
                        else
                        {
                            if (coeff3 < -1)
                            {
                                expression3 = expression2 + " " + coeff3 + "z";
                            }
                            else
                            {
                                expression3 = expression2 + " + " + coeff3 + "z";
                            }
                        }

                        
                        int tempAns = coeff1 * x + coeff2 * y + coeff3 * z;
                        Answer = tempAns.ToString();

                    }
                   
                    else
                    #region 
                    {
                        if (coeff1 == 0)
                        {
                            expression1 = " ";
                        }
                        else if (coeff1 == 1)
                        {
                            expression1 = "xy";
                        }
                        else if (coeff1 == -1)
                        {
                            expression1 = "-xy";
                        }
                        else
                        {
                            expression1 = coeff1.ToString() + "xy";
                        }


                        if (coeff2 == 0)
                        {
                            expression2 = expression1 + " ";
                        }
                        else if (expression1 != " ")
                        {
                            if (coeff2 == 1)
                                expression2 = expression1 + " + yz";
                            else if (coeff2 == -1)
                                expression2 = expression1 + "- yz ";
                            else
                            {
                                if (coeff2 < -1)
                                {
                                    expression2 = expression1 + " " + coeff2 + "yz";
                                }
                                else
                                {
                                    expression2 = expression1 + " + " + coeff2 + "yz";
                                }
                            }

                        }
                       

                        if (coeff3 == 0)
                        {
                            expression3 = expression2 + " ";
                        }
                        else if (coeff3 == 1 && expression2 != " ")
                        {
                            expression3 = expression2 + " + xz";
                        }
                        else if (coeff3 == 1 && expression2 == " ")
                        {
                            expression3 = expression2 + " xz";
                        }
                        else if (coeff3 == -1)
                        {
                            expression3 = expression2 + " -xz";
                        }
                        else
                        {
                            if (coeff2 < -1)
                            {
                                expression3 = expression2 + " " + coeff3.ToString() + "xz";
                            }
                            else
                            {
                                expression3 = expression2 + " + " + coeff3.ToString() + "xz";
                            }
                        }

                        int tempAns = coeff1 * x * y + coeff2 * y * z + coeff3 * z * x;
                        Answer = tempAns.ToString();

                    }
                    #endregion

					QuestionText.text = "Compute value for x=" + x.ToString() + ", y=" + y.ToString() + ", z=" + z.ToString();

                }
                #endregion selector1
                #region selector2
                if (selector == 2)
                {
                    int activate = Random.Range(0, 2);
                   
                    if (activate == 1)
                    #region
                    {
                        coeff4 = Random.Range(-2, 3);
                        coeff5 = Random.Range(-2, 3);
                        x = Random.Range(-2, 3);
                        y = Random.Range(-2, 3);
                        z = Random.Range(-2, 3);

                        if (coeff2 > 0)
                            expression3 = coeff1 + "x\\^4" + " + " + coeff2 + "x\\^3";
                        else if (coeff2 < 0)
                            expression3 = coeff1 + "x\\^4" + " " + coeff2 + "x\\^3";

                        if (coeff3 > 0)
                            expression3 = expression3 + " + " + coeff3 + "x\\^2";
                        else if (coeff3 < 0)
                            expression3 = expression3 + " " + coeff3 + "x\\^2";

                        if (coeff4 > 0)
                            expression3 = expression3 + " + " + coeff4 + "x";
                        else if (coeff4 < 0)
                            expression3 = expression3 + " " + coeff4 + "x";

                        if (coeff5 > 0)
                            expression3 = expression3 + " + " + coeff5;
                        else if (coeff5 < 0)
                            expression3 = expression3 + " " + coeff5;


                        int tempAns = (coeff1 * x * x * x * x) + (coeff2 * x * x * x) + (coeff3 * x * x) + (coeff4 * x) + coeff5;
                        Answer = tempAns.ToString();
						QuestionText.text = "Compute value for x=" + x.ToString();
                    }
                    #endregion
                    else
                    #region
                    {
                        coeff4 = Random.Range(-5, 5);
                        coeff5 = Random.Range(-5, 5);
                        coeff6 = Random.Range(-5, 5);
                        

                        if (coeff4 == 1)
                        {
                            expression1 = "\\( y + z \\)";
                        }
                        else if (coeff4 == -1)
                        {
                            expression1 = "\\( y - z \\)";
                        }
                        else if (coeff4 < -1)
                        {
                            expression1 = "\\( y " + coeff4 + "z \\)";
                        }
                        else if (coeff4 > 1)
                        {
                            expression1 = "\\( y + " + coeff4 + "z \\)";
                        }
                        else if (coeff4 == 0)
                            expression1 = "y";

                        if (coeff5 == 1)
                        {
                            expression2 = "\\( z + x \\)";
                        }
                        else if (coeff5 == -1)
                        {
                            expression2 = "\\( z - x \\)";
                        }
                        else if (coeff5 < -1)
                        {
                            expression2 = "\\( z " + coeff5 + "x \\)";
                        }
                        else if (coeff5 > 1)
                        {
                            expression2 = "\\( z + " + coeff5 + "x \\)";
                        }
                        else if (coeff5 == 0)
                            expression2 = "z";

                        if (coeff6 == 1)
                        {
                            expression3 = "\\( y + x \\)";
                        }
                        else if (coeff6 == -1)
                        {
                            expression3 = "\\( y - x \\)";
                        }
                        else if (coeff6 < -1)
                        {
                            expression3 = "\\( y " + coeff6 + "x \\)";
                        }
                        else if (coeff6 > 1)
                        {
                            expression3 = "\\( y + " + coeff6 + "x \\)";
                        }
                        else if (coeff6 == 0)
                            expression3 = "y";

                        if (coeff1 == 0)
                        {
                            expression1 = " ";
                        }




                        if (coeff2 == 1)
                        {
                            expression2 = coeff1 + expression1 + " + " + expression2;
                        }
                        else if (coeff2 > 1)
                        {
                            expression2 = coeff1 + expression1 + " + " + coeff2 + expression2;
                        }
                        else if (coeff2 == -1)
                        {
                            expression2 = coeff1 + expression1 + " - " + expression2;
                        }
                        else if (coeff2 < -1)
                        {
                            expression2 = coeff1 + expression1 + " " + coeff2 + expression2;
                        }
                        else if (coeff2 == 0)
                        {
                            expression2 = coeff1 + expression1;
                        }

                        if (coeff3 == 1)
                        {
                            expression3 = expression2 + " + " + expression3;
                        }
                        else if (coeff3 > 1)
                        {
                            expression3 = expression2 + " + " + coeff3 + expression3;
                        }
                        else if (coeff3 == -1)
                        {
                            expression3 = expression2 + " - " + expression3;
                        }
                        else if (coeff3 < -1)
                        {
                            expression3 = expression2 + " " + coeff3 + expression3;
                        }
                        else if (coeff3 == 0)
                        {
                            expression3 = expression2;
                        }

						QuestionText.text = "Compute value for x=" + x.ToString() + ", y=" + y.ToString() + ", z=" + z.ToString();
                        int tempAns = coeff1 * (y + coeff4 * z) + coeff2 * (z + coeff5 * x) + coeff3 * (y + coeff6 * x);
                        Answer = tempAns.ToString();


                    }
                    #endregion

                }
                #endregion selector2
                #region selector3
                if (selector == 3)
                #region
                {
                    // coeff4, coeff5 , coeff 6 are denominators
                    coeff4 = Random.Range(2, 9);
                    coeff5 = Random.Range(1, 9);
                    coeff6 = Random.Range(1, 9);
                    a = Random.Range(2, 6);
                    b = Random.Range(1, 6);
                    c = Random.Range(1, 3);

                    while (c % coeff4 != 0 && b % coeff4 != 0 && a % coeff4 != 0)
                        coeff4 = Random.Range(2, 9);
                    while (c % coeff5 != 0 && b % coeff5 != 0 && a % coeff5 != 0)
                        coeff5 = Random.Range(2, 9);
                    while (c % coeff6 != 0 && b % coeff6 != 0 && a % coeff6 != 0)
                        coeff6 = Random.Range(2, 9);

                   

                    if (coeff1 != 0)                 
                            expression3 = "\\frac{" + coeff1 + "}{" + coeff4 + "}" + "ab\\^2c";
                       
                    if (coeff2 < 0)
                    {
                        if (coeff5 != 1 )
                            expression3 += "- \\frac{" + Mathf.Abs(coeff2) + "}{" + coeff5 + "}" + "bc\\^2a";
                        else
                            expression3 += " - " + Mathf.Abs(coeff2) + "bc\\^2a";
                    }
                    else if (coeff2 > 0)
                    {

                        if (coeff5 != 1 && expression3 != " ")
                            expression3 += " + \\frac{" + coeff2 + "}{" + coeff5 + "}" + "bc\\^2a";
                        else if(expression3 == " " && coeff5 != 1)
                            expression3 += " \\frac{" + coeff2 + "}{" + coeff5 + "}" + "bc\\^2a";
                        else
                            expression3 += " + " + coeff2 + "bc\\^2a";
                    }


                    if (coeff3 > 0)
                    {
                        if (coeff6 != 1 && expression3 != " ")
                            expression3 += " + \\frac{" + coeff3 + "}{" + coeff6 + "}" + "c\\^3ab\\^3";
                        else if (coeff6 != 1 )
                            expression3 += " \\frac{" + coeff3 + "}{" + coeff6 + "}" + "c\\^3ab\\^3";
                        else
                            expression3 += " + " + coeff3 + "c\\^3ab\\^3";
                    }
                    else if (coeff3 < 0)
                    {
                        if (coeff6 != 1 && expression3 != " ")
                            expression3 += "- \\frac{" + Mathf.Abs(coeff3) + "}{" + coeff6 + "}" + "c\\^3ab\\^3";
                        else if (coeff6 != 1 )
                            expression3 += " \\frac{" + (coeff3) + "}{" + coeff6 + "}" + "c\\^3ab\\^3";
                        else
                            expression3 += " - " + Mathf.Abs(coeff3) + "c\\^3ab\\^3";
                    }
					QuestionText.text = "Compute value for a=" + a.ToString() + ", b=" + b.ToString() + ", c=" + c.ToString();                                

                    int tempAns = (int)((((float)coeff1 / (float)coeff4) * a * b * b * c) + (((float)coeff2 / (float)coeff5) * b * c * c * a) + (((float)coeff3 / (float)coeff6) * c * c * c * a * b * b * b));
                   
                  
                    Answer = tempAns.ToString();


                }
                #endregion
                #endregion selector3
                QuestionTEX.text = expression3;
            }
            #endregion level1
            #region level2 
            if (level == 2)
            {
                a = Random.Range(-5, 5);
                b = Random.Range(-5, 5);
                c = Random.Range(-5, 5);


				int sublevel = GetRandomSelector (0, 2);
               
                if (sublevel == 0) // Division
                {
                    selector = Random.Range(1, 4);
                    a = Random.Range(-5, 5);
                    b = Random.Range(-5, 5);
                    c = Random.Range(-5, 5);

                    
                    #region selector1
                    if (selector == 1)
                    {

                        coeff1 = Random.Range(-5, 5);
                        coeff2 = Random.Range(-5, 5);
                        coeff3 = Random.Range(-5, 5);
                        coeff4 = Random.Range(-5, 5);
                        coeff5 = Random.Range(-5, 5);
                        coeff6 = Random.Range(-5, 5);
                      
                        while ((coeff4 * b * b + coeff5 * a * a + coeff6 * c * c) == 0)
                            {
                                coeff4 = Random.Range(-5, 5);
                                coeff5 = Random.Range(-5, 5);
                                coeff6 = Random.Range(-5, 5);

                            a = Random.Range(-5, 5);
                            b = Random.Range(-5, 5);
                            c = Random.Range(-5, 5);
                             }
                        
                        
                        //  numerator = expression1
                        if (coeff1 == 0)
                            expression1 = "";
                        else if (coeff1 == 1)
                            expression1 = "a\\^2";
                        else if (coeff1 == -1)
                            expression1 = "- a\\^2";
                        else expression1 = coeff1 + "a\\^2";

                        if (coeff2 == 1)
                        {   if(expression1 == "")
                                 expression1 += "b\\^2";
                            else
                            {
                                expression1 += " + b\\^2";
                            }
                        }
                        else if (coeff2 == -1)
                            expression1 +=  " - b\\^2";
                        else if (coeff2 > 1)
                            expression1 += " + " + coeff2 + "b\\^2";
                        else if (coeff2 == 0)
                            expression1 += "";
                        else expression1 += coeff2 + "b\\^2";

                        if (coeff3 == 1)
                        {
                            if (expression1 == "")
                                expression1 += "c\\^2";
                            else expression1 += " + c\\^2";
                        }
                        else if (coeff3 == -1)
                            expression1 += " - c\\^2";
                        else if (coeff3 > 1)
                            expression1 += " + " + coeff3 + "c\\^2";
                        else if (coeff3 == 0)
                            expression1 += " ";
                        else expression1 += coeff3 + "c\\^2";


                        //denominator = expression2
                        if (coeff4 == 0)
                            expression2 = "";
                        else if (coeff4 == 1)
                            expression2 = " b\\^2";
                        else if (coeff4 == -1)
                            expression2 = "- b\\^2";
                        else expression2 = coeff4 + "b\\^2";

                        if (coeff5 == 1)
                        {   if(expression2 != "")
                                expression2 += " + a\\^2";
                            else
                            {
                                expression2 += "a\\^2";
                            }
                        }
                        else if (coeff5 == -1)
                            expression2 +=  " - a\\^2";
                        else if (coeff5 > 1)
                            expression2 += " + " + coeff5 + "a\\^2";
                        else if (coeff5 == 0)
                            expression2 += "";
                        else expression2 += coeff5 + "a\\^2";

                        if (coeff6 == 1)
                            expression2 += " + c\\^2";
                        else if (coeff6 == -1)
                            expression2 += " - c\\^2";
                        else if (coeff6 > 1)
                            expression2 += " + " + coeff6 + "c\\^2";
                        else if (coeff6 == 0)
                            expression2 += "";
                        else expression2 += coeff6 + "c\\^2";


                        //expression3 = numerator/denominator
                        expression3 = "\\frac{" + expression1 + "}{" + expression2 + "}";

						QuestionText.text = "Solve the equation for values a=" + a.ToString() + ", b=" + b.ToString() + ", c=" + c.ToString();


                        int tempAns = (coeff1 * a * a + coeff2 * b * b + coeff3 * c * c);
                        int tempAns2 = (coeff4 * b * b + coeff5 * a * a + coeff6 * c * c);
                        Answer = tempAns.ToString() + " / " + tempAns2.ToString() ;
                      

                    }
                    #endregion selector1
                    #region selector2
                    if (selector == 2)
                    {
                        coeff1 = Random.Range(-3, 3);
                        coeff2 = Random.Range(-3, 3);
                        coeff3 = Random.Range(-3, 4);
                       
                            while (coeff3 == 0)
                            {
                                coeff3 = Random.Range(-5, 5);
                            }
                            while (a == 0)
                            {
                                a = Random.Range(-5, 5);
                            }
                            while (b == 0)
                            {
                                b = Random.Range(-5, 5);
                            }
                            while (c == 0)
                            {
                                c = Random.Range(-5, 5);
                            }
                        
                        //expression1 = numerator
                        if (coeff1 == 0)
                            expression1 = "0";
                        else if (coeff1 == 1)
                            expression1 = "abc";
                        else if (coeff1 == -1)
                            expression1 = "- abc";
                        else expression1 = coeff1 + "abc";

                        if (coeff2 == 0)
                            expression1 += "";
                        else if (coeff2 == 1)
                            expression1 += " + a\\^2bc\\^0";
                        else if (coeff2 == -1)
                            expression1 += " - a\\^2bc\\^0";
                        else if (coeff2 > 1)
                            expression1 += " + " + coeff2 + "a\\^2bc\\^0";
                        else expression1 += " " + coeff2 + "a\\^2bc\\^0";

                        //expression2 = denominator
                        expression2 = "";
                        if (coeff3 == 1)
                            expression2 += "2ab\\^0c";
                        else if (coeff3 == -1)
                            expression2 += " -(2ab\\^0c)";
                        else expression2 += coeff3 + "(2ab\\^0c)";
                    

                        

                            expression3 = "\\frac{" + expression1 + "}{" + expression2 + "}";

						QuestionText.text = "Solve the equation for values a=" + a.ToString() + ", b=" + b.ToString() + ", c=" + c.ToString() + " give the answer in fraction ";


                        int tempAns = ((coeff1 * a * b * c) + (coeff2 * a * a * b));
                        int tempAns2 = (coeff3 * 2 * a * c);
                        Answer = tempAns.ToString() + " / " + tempAns2.ToString();
                        

                    }
                    #endregion selector2
                    #region selector3
                    if (selector == 3)
                    {
                        coeff1 = Random.Range(-5, 5);
                        coeff2 = Random.Range(-5, 5);
                        coeff3 = Random.Range(-5, 5);
                        coeff4 = Random.Range(-5, 5);
                        coeff5 = Random.Range(-5, 5);
                        coeff6 = Random.Range(-5, 5);
                        while (((coeff4 * a * b) + (coeff5 * c * a) + (coeff6 * b * c)) == 0)
                        {
                            coeff4 = Random.Range(-5, 5);
                            coeff5 = Random.Range(-5, 5);
                            coeff6 = Random.Range(-5, 5);
                            a = Random.Range(-5, 5);
                            b = Random.Range(-5, 5);
                            c = Random.Range(-5, 5);

                        }

                        //  numerator = expression1
                        if (coeff1 == 0)
                            expression1 = " ";
                        else if (coeff1 == 1)
                            expression1 = "a\\^2";
                        else if (coeff1 == -1)
                            expression1 = "- a\\^2";
                        else expression1 = coeff1 + "a\\^2";

                        if (coeff2 == 1)
                        {   if (expression1 == " ")
                                expression1 += "bc";
                            else expression1 += "+ bc";
                        }
                        else if (coeff2 == -1)
                            expression1 += " " + "- bc";
                        else if (coeff2 > 1)
                            expression1 += " + " + coeff2 + "bc";     
                        else if(coeff2 < 0)
                            expression1 += coeff2 + "bc";


                        if (coeff3 == 1)
                        {   if (expression1 == " ")
                                expression1 = "c\\^2";
                            else expression1 += " + c\\^2";
                        }
                        else if (coeff3 == -1)
                            expression1 += " " + "- c\\^2";
                        else if (coeff3 > 1)
                            expression1 += " + " + coeff3 + "c\\^2";
                        else if (coeff3 < 0)
                            expression1 += coeff3 + "c\\^2";


                        //denominator = expression2
                        if (coeff4 == 0)
                            expression2 = " ";
                        else if (coeff4 == 1)
                            expression2 = "ab";
                        else if (coeff4 == -1)
                            expression2 = "- ab";
                        else expression2 = coeff4 + "ab";

                        if (coeff5 == 1)
                        {   if (expression2 == " ")
                                expression2 += "ca";
                            else expression2 += " + ca";
                        }
                        else if (coeff5 == -1)
                            expression2 += " " + "- ca";
                        else if (coeff5 > 1 && expression2 != " ")
                            expression2 += " + " + coeff5 + "ca";
                        else if (coeff5 == 0)
                            expression2 += " ";
                        else expression2 += coeff5 + "ca";

                        if (coeff6 == 1)
                        {   if (expression2 != " ")
                                expression2 += "+ bc";
                            else expression2 += "bc";
                        }
                        else if (coeff6 == -1)
                            expression2 += " " + "- bc";
                        else if (coeff6 > 1 && expression2 != " ")
                            expression2 += " + " + coeff6 + "bc";
                        else if (coeff6 == 0)
                            expression2 += " ";
                        else expression2 += coeff6 + "bc";

                        expression3 = "\\frac{" + expression1 + "}{" + expression2 + "}";

						QuestionText.text = "Solve the equation for values a=" + a.ToString() + ", b=" + b.ToString() + ", c=" + c.ToString() + " write answer in fraction" ;
                       
                        int tempAns = (coeff1 * a * a + coeff2 * b * c + coeff3 * c * c);
                        int tempAns2  = ((coeff4 * a * b) + (coeff5 * c * a) + (coeff6 * b * c));
                        Answer = tempAns.ToString() + " / " + tempAns2.ToString();
                      

                    }
                    #endregion selector3

                }
                else                // Power
                {
                    selector = Random.Range(1, 5);
                    
                    #region selector1
                    if (selector == 1)
                    {
                        a = Random.Range(1, 7);
                        b = Random.Range(1, 5);
                        expression3 = "(ab)\\^b";

						QuestionText.text = "Solve the equation for values a=" + a.ToString() + ", b=" + b.ToString();
                        int tempAns = ((int)Mathf.Pow(b,b))*a;
                        Answer = tempAns.ToString();

                    }
                    #endregion selector1
                    #region selector2
                    if (selector == 2)
                    {
                        a = Random.Range(1, 3);
                        b = Random.Range(1, 3);
                        c = Random.Range(1, 9);
                        int tempSel = Random.Range(0, 2);
                        if (tempSel == 0 && (a*b != 1))
                            expression3 = "(abc)\\^{-ab}";
                        else expression3 = "(abc)\\^{ab}";

						QuestionText.text = "Solve the equation for values a=" + a.ToString() + ", b=" + b.ToString() + ", c=" + c.ToString() + " write the answer in fraction if it is not an integer";
                        float tempAns = (Mathf.Pow((a * b * c), ( a * b)));
                        Answer = tempAns.ToString("F2");
                        if (tempSel == 0)
                            Answer =  " 1/ " + tempAns.ToString();


                    }
                    #endregion selector2
                    #region selector3
                    if (selector == 3)
                    {
                        a = Random.Range(1, 5);
                        b = Random.Range(1, 5);
                        c = Random.Range(1, 5);
                        x = Random.Range(1, 3);
                        

                        expression3 = "(a\\^2bc)\\^xxy";

						QuestionText.text = "Solve the equation for values a=" + a.ToString() + ", b=" + b.ToString() + ", c=" + c.ToString() + ", x=" + x.ToString() + " write coefficient of y in the box";
                        int  tA1 = ((int)Mathf.Pow(a, 2))*b*c;
                        int tempAns = (int)Mathf.Pow(tA1, x);

                       
                        Answer = tempAns.ToString() ;


                    }
                    #endregion selector3
                    #region selector4
                    if (selector == 4)
                    {
                        a = Random.Range(1, 5);
                        expression3 = "a\\^a";

						QuestionText.text = "Solve the equation for values a=" + a.ToString() ;
                       
                        int tempAns = (int)Mathf.Pow(a,a);


                        Answer = tempAns.ToString() ;


                    }
                    #endregion selector4-
                }

                QuestionTEX.text = expression3;

            }
            #endregion level2
            #region level3
            if (level == 3)                     // brackets
            {
				int sublevel = GetRandomSelector (0, 2);
               
                if (sublevel == 0)
                {
                    selector = Random.Range(1, 3);
                   
                    #region selector1
                    if (selector == 1)       // (x3y2 + y2)- (x3y2 - y2)
                    {
                        coeff1 = Random.Range(-5, 5);
                        coeff2 = Random.Range(-5, 5);
                        coeff3 = Random.Range(-5, 5);
                        coeff4 = Random.Range(-5, 5);
                        coeff5 = Random.Range(-5, 5);
                        coeff6 = Random.Range(-5, 5);
                        x = Random.Range(-3, 4);
                        y = Random.Range(-3, 4);
                        if (coeff1 == 0 && coeff4 == 0)
                        {
                            while (coeff1 == 0 && coeff4 == 0)
                            {
                                coeff4 = Random.Range(-5, 5);
                                coeff6 = Random.Range(-5, 5);
                            }
                        }

                        //first term
                        if (coeff1 == 0)
                            expression1 = "";
                        if (coeff2 == 1)
                        {
                            if (coeff3 == 0)
                                expression1 = "x\\^3y\\^2";
                            else if (coeff3 == 1)
                                expression1 = "x\\^3y\\^2 + y\\^2";
                            else if (coeff3 == -1)
                                expression1 = "x\\^3y\\^2 - y\\^2";
                            else if (coeff3 > 1)
                                expression1 = "x\\^3y\\^2 +" + coeff3 + " y\\^2";
                            else expression1 = "x\\^3y\\^2 " + coeff3 + " y\\^2";
                        }
                        else if (coeff2 == -1)
                        {
                            if (coeff3 == 0)
                                expression1 = "- x\\^3y\\^2";
                            else if (coeff3 == 1)
                                expression1 = "- x\\^3y\\^2 + y\\^2";
                            else if (coeff3 == -1)
                                expression1 = "- x\\^3y\\^2 - y\\^2";
                            else if (coeff3 > 1)
                                expression1 = "- x\\^3y\\^2 +" + coeff3 + " y\\^2";
                            else expression1 = "- x\\^3y\\^2 " + coeff3 + " y\\^2";
                        }
                        else
                        {
                            if (coeff3 == 0)
                                expression1 = coeff2 + "x\\^3y\\^2";
                            else if (coeff3 == 1)
                                expression1 = coeff2 + "x\\^3y\\^2 + y\\^2";
                            else if (coeff3 == -1)
                                expression1 = coeff2 + "x\\^3y\\^2 - y\\^2";
                            else if (coeff3 > 1)
                                expression1 = coeff2 + "x\\^3y\\^2 +" + coeff3 + " y\\^2";
                            else expression1 = coeff2 + "x\\^3y\\^2 " + coeff3 + " y\\^2";
                        }

                        if (coeff1 == 1)
                            expression1 = "(" + expression1 + ")";
                        else if (coeff1 == -1)
                            expression1 = "-(" + expression1 + ")";
                        else expression1 = coeff1 + "(" + expression1 + ")";

                        // second term
                        if (coeff4 == 0)
                            expression2 = " ";
                        if (coeff5 == 1)
                        {
                            if (coeff6 == 0)
                                expression2 = "x\\^3y\\^2";
                            else if (coeff6 == 1)
                                expression2 = "x\\^3y\\^2 + y\\^2";
                            else if (coeff6 == -1)
                                expression2 = "x\\^3y\\^2 - y\\^2";
                            else if (coeff6 > 1)
                                expression2 = "x\\^3y\\^2 +" + coeff6 + " y\\^2";
                            else expression2 = "x\\^3y\\^2 " + coeff6 + " y\\^2";
                        }
                        else if (coeff5 == -1)
                        {
                            if (coeff6 == 0)
                                expression2 = "- x\\^3y\\^2";
                            else if (coeff6 == 1)
                                expression2 = "- x\\^3y\\^2 + y\\^2";
                            else if (coeff6 == -1)
                                expression2 = "- x\\^3y\\^2 - y\\^2";
                            else if (coeff6 > 1)
                                expression2 = "- x\\^3y\\^2 +" + coeff6 + " y\\^2";
                            else expression2 = "- x\\^3y\\^2 " + coeff6 + " y\\^2";
                        }
                        else
                        {
                            if (coeff6 == 0)
                                expression2 = coeff5 + "x\\^3y\\^2";
                            else if (coeff6 == 1)
                                expression2 = coeff5 + "x\\^3y\\^2 + y\\^2";
                            else if (coeff6 == -1)
                                expression2 = coeff5 + "x\\^3y\\^2 - y\\^2";
                            else if (coeff6 > 1)
                                expression2 = coeff5 + "x\\^3y\\^2 +" + coeff6 + " y\\^2";
                            else expression2 = coeff5 + "x\\^3y\\^2 " + coeff6 + " y\\^2";
                        }

                        if (coeff4 == 1)
                            expression2 = "+ (" + expression2 + ")";
                        else if (coeff4 == -1)
                            expression2 = "-(" + expression2 + ")";
                        else if (coeff4 >= 0)
                            expression2 = " + " + coeff4 + "(" + expression2 + ")";
                        else expression2 = coeff4 + "(" + expression2 + ")";

                        expression3 = expression1 + expression2;

						QuestionText.text = "Solve the equation for values x=" + x.ToString() + ", y=" + y.ToString();

                        int tempAns = (coeff1 * ((coeff2 * x * x * x * y * y) + (coeff3 * y * y)))+ (coeff4 * ((coeff5 * x * x * x * y * y) + (coeff6 * y *y)));


                       Answer = tempAns.ToString();

                    }
                    #endregion selector1
                    #region selector2
                    if (selector == 2)                   //x-[y-{z2-(x+y)0}]
                    {
                        coeff1 = Random.Range(-5, 5);
                        coeff2 = Random.Range(-5, 5);
                        coeff3 = Random.Range(-5, 5);
                        coeff4 = Random.Range(-5, 5);
                        coeff5 = Random.Range(-5, 5);
                        x = Random.Range(-5, 5);
                        y = Random.Range(-5, 5);
                        z = Random.Range(-3, 4);
                       
                        
                        while (coeff2 == 0 || coeff1 == 0)
                        {
                          coeff1 = Random.Range(-7, 7);
                          coeff2 = Random.Range(-7, 7);
                        }

                        int pow1 = Random.Range(0, 2);

                        expression1 = "";
                        if (coeff3 != 0)
                        {


                            if (coeff5 == 1)
                                expression1 = "+ y";
                            else if (coeff5 == -1)
                                expression1 = "- y";
                            else if (coeff5 < -1)
                                expression1 = coeff5 + "y";
                            else if (coeff5 > 1)
                                expression1 = " + " + coeff5 + "y";

                            if (coeff4 == 1)
                                expression1 = "+ x" + expression1;
                            else if (coeff4 == -1)
                                expression1 = "-x" + expression1;
                            else expression1 = coeff4 + "x" + expression1;


                            expression1 = "(" + expression1 + ")\\^" + pow1;

                            if (coeff3 == 1)
                                expression1 = " + " + expression1;
                            else if (coeff3 == -1)
                                expression1 = " - " + expression1;
                            else if (coeff3 > 1)
                                expression1 = " + " + coeff3 + expression1;
                            else expression1 = coeff3 + expression1;
                        }
                        else
                        {
                            expression1 = "";
                        }

                        expression1 = "( z\\^2 " + expression1 + ")";

                        if (coeff2 == 1)
                            expression1 = " + " + expression1;
                        else if (coeff2 == -1)
                            expression1 = " - " + expression1;
                        else if (coeff2 > 1)
                            expression1 = " + " + coeff2 + expression1;
                        else if (coeff2 < -1)
                            expression1 = coeff2 + expression1;

                        expression1 = "[y " + expression1 + "]";


                        if (coeff1 == 1)
                            expression1 = " + " + expression1;
                        else if (coeff1 == -1)
                            expression1 = " - " + expression1;
                        else if (coeff1 > 1)
                            expression1 = " + " + coeff1 + expression1;
                        else if (coeff1 < -1)
                            expression1 = coeff1 + expression1;

                        expression3 = " x " + expression1;

						QuestionText.text = "Solve the equation for values x=" + x.ToString() + ", y=" + y.ToString() + ", z=" + z.ToString();

                        int tempAns = x + coeff1 * (y +  (coeff2 * ((int)Mathf.Pow(z, 2) + coeff3 * ((int)Mathf.Pow((coeff4 * x + coeff5 * y), pow1)))));    //x-[y-{z2-c3*(c4*x+c5*y)^pow1}]


                        Answer = tempAns.ToString();

                    }
                    #endregion selector2
                   
                }
                else
                {
                    selector = Random.Range(1, 3);
                    
                    if (selector == 1)
                    {
                        x = Random.Range(-25, 25);   // v
                        y = Random.Range(-25, 25);   // u
                        z = Random.Range(-25, 25);   // t
                        int v = x;
                        int u = y;
                        int t = z;

						QuestionText.text = "If the first Equation of motion is v = u + at, then determine the value of a. where v=" + v.ToString() + ", u=" + u.ToString() + ", t=" + t.ToString();
                        expression3 = " ";
                        int tempAns = (v - u);
                        Answer = tempAns.ToString() + " / " + t.ToString();
                        if (tempAns % t == 0)
                            Answer = (tempAns / t).ToString();
                    }
                    if(selector == 2)
                    {
                        x = Random.Range(-25, 25);   
                        y = Random.Range(-25, 25);

						QuestionText.text = "the Equation of Circle with r is radius is given below,then find value of r. where x=" + x.ToString() + ", y=" + y.ToString()  ;
                        expression3 = "x\\^2 + y\\^2 = r\\^2";
                        float tempAns = Mathf.Sqrt(x * x + y * y);
                        Answer = tempAns.ToString("F2");
                        

                    }
                    

                }

                QuestionTEX.text = expression3;

            }
            #endregion level3


            //CerebroHelper.DebugLog(Answer);
                userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
                userAnswerText.text = " ";
            
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
            {   // All Clear
              
                userAnswerText.text = " ";
            }
        }
    }
}
