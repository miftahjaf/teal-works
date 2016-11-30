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
		private bool multiplePossibleAnswers;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "SAF06", "S01", "A01");
                                    
            scorestreaklvls = new int[4];
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

        public override void SubmitClick()
        {
            if (ignoreTouches || userAnswerLaText.text == "")
            {
                return;
            }
            int increment = 0;
            //var correct = false;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
			var correct = false;

            questionsAttempted++;
            updateQuestionsAttempted();

			if (multiplePossibleAnswers)
			{
				correct = MathFunctions.checkAlgebraicExpressions (Answer, userAnswerLaText.text);
			}
			else if (Answer.Contains ("/")) {
				var answerSplits = Answer.Split (new string[] { "/" }, System.StringSplitOptions.None);
				var userAnswerSplits = userAnswerLaText.text.Split (new string[] { "/" }, System.StringSplitOptions.None);
				correct = MathFunctions.checkFractions (userAnswerSplits, answerSplits);
			} else {
				float answer = 0;
				float userAnswer = 0;
				bool directCheck = false;
				if (float.TryParse (Answer, out answer)) {
					answer = float.Parse (Answer);
				} else {
					directCheck = true;
				}

				if (float.TryParse (userAnswerLaText.text, out userAnswer)) {
					userAnswer = float.Parse (userAnswerLaText.text);
				} else {
					directCheck = true;
				}


				if (directCheck) {
					if (userAnswerLaText.text == Answer) {
						correct = true;
					} else {
						correct = false;
					}
				} else {
					correct = (answer == userAnswer);
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
				else if (Queslevel == 5)
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
			userAnswerLaText.color = MaterialColor.red800;
			Go.to(userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
			yield return new WaitForSeconds(0.5f);
			if (isRevisitedQuestion)
			{
				userAnswerLaText.text = "";
				userAnswerLaText.color = MaterialColor.textDark;
				ignoreTouches = false;
			}
			else {
				userAnswerLaText.text = Answer;
				userAnswerLaText.color = MaterialColor.green800;
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
			multiplePossibleAnswers = false;
			answerButton = GeneralButton;

			GeneralButton.gameObject.SetActive(true);
			QuestionText.gameObject.SetActive(true);
			numPad.SetActive (true);

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

                
                while (coeff1 == 0)
               	{
        	        coeff1 = Random.Range(-9, 9);
    		   	}
				while (coeff2 == 0)
				{
					coeff2 = Random.Range(-9, 9);
				}     
                
                selector = GetRandomSelector (1, 4);
               
                expression3 = "";
               
                if (selector == 1)
                {
					int randSelector = Random.Range(0, 2);
					Debug.Log ("randSelector = " + randSelector);
                    if (randSelector == 1)
                    {
                        if (coeff1 == 0)
                        {
                            expression1 = "";
                        }
                        else if (coeff1 == 1)
                        {
                            expression1 = "\\xalgebra";
                        }
                        else if (coeff1 == -1)
                        {
                            expression1 = "- x";
                        }
                        else
                        {
                            expression1 = coeff1 + "\\xalgebra";
                        }


                        if (coeff2 == 0)
                        {
                            expression2 = expression1;
                        }
                        else if (coeff2 == 1)
                        {
                            expression2 = expression1 + " + \\yalgebra";
                        }
                        else if (coeff2 == -1)
                        {
                            expression2 = expression1 + " - \\yalgebra";
                        }
                        else
                        {
                            if (coeff2 < -1)
                            {
								expression2 = expression1 + " - " + (-coeff2) + "\\yalgebra";
                            }
                            else
                            {
                                expression2 = expression1 + " + " + coeff2 + "\\yalgebra";
                            }
                        }


                        if (coeff3 == 0)
                        {
                            expression3 = expression2;
                        }
                        else if (coeff3 == 1)
                        {
                            expression3 = expression2 + " + \\zalgebra";
                        }
                        else if (coeff3 == -1)
                        {
                            expression3 = expression2 + " - \\zalgebra";
                        }
                        else
                        {
                            if (coeff3 < -1)
                            {
								expression3 = expression2 + " - " + (-coeff3) + "\\zalgebra";
                            }
                            else
                            {
                                expression3 = expression2 + " + " + coeff3 + "\\zalgebra";
                            }
                        }

                        
                        int tempAns = coeff1 * x + coeff2 * y + coeff3 * z;
                        Answer = tempAns.ToString();

                    }
                   
                    else
                    {
                        if (coeff1 == 0)
                        {
                            expression1 = "";
                        }
                        else if (coeff1 == 1)
                        {
                            expression1 = "\\xalgebra\\yalgebra";
                        }
                        else if (coeff1 == -1)
                        {
                            expression1 = "- \\xalgebra\\yalgebra";
                        }
                        else
                        {
                            expression1 = coeff1 + "\\xalgebra\\yalgebra";
                        }


                        if (coeff2 == 0)
                        {
                            expression2 = expression1;
                        }
                        else if (expression1 != "")
                        {
                            if (coeff2 == 1)
                                expression2 = expression1 + " + \\yalgebra\\zalgebra";
                            else if (coeff2 == -1)
                                expression2 = expression1 + " - \\yalgebra\\zalgebra ";
                            else
                            {
                                if (coeff2 < -1)
                                {
									expression2 = expression1 + " - " + (-coeff2) + "\\yalgebra\\zalgebra";
                                }
                                else
                                {
                                    expression2 = expression1 + " + " + coeff2 + "\\yalgebra\\zalgebra";
                                }
                            }

                        }
                       

                        if (coeff3 == 0)
                        {
                            expression3 = expression2;
                        }
                        else if (coeff3 == 1 && expression2 != "")
                        {
                            expression3 = expression2 + " + \\xalgebra\\zalgebra";
                        }
                        else if (coeff3 == 1 && expression2 == "")
                        {
                            expression3 = expression2 + " \\xalgebra\\zalgebra";
                        }
                        else if (coeff3 == -1)
                        {
                            expression3 = expression2 + " - \\xalgebra\\zalgebra";
                        }
                        else
                        {
                            if (coeff3 < -1)
                            {
								expression3 = expression2 + " - " + (-coeff3) + "\\xalgebra\\zalgebra";
                            }
                            else
                            {
                                expression3 = expression2 + " + " + coeff3 + "\\xalgebra\\zalgebra";
                            }
                        }

                        int tempAns = coeff1 * x * y + coeff2 * y * z + coeff3 * z * x;
                        Answer = tempAns.ToString();

                    }

					QuestionText.text = "Solve for x = " + x + ", y = " + y + ", z = " + z + ".";

                }
                else if (selector == 2)
                {
					int randSelector = Random.Range(1, 3);
					Debug.Log ("randSelector = " + randSelector);
                    if (randSelector == 1)
                    {
						coeff1 = GenerateRandomIntegerExcluding01 (-5, 6);
						coeff2 = GenerateRandomIntegerExcluding01 (-5, 6);
						coeff3 = GenerateRandomIntegerExcluding01 (-5, 6);
						coeff4 = GenerateRandomIntegerExcluding01 (-5, 6);
                        coeff5 = Random.Range(-5, 6);
                        x = Random.Range(-2, 3);
                        y = Random.Range(-2, 3);
                        z = Random.Range(-2, 3);

                        if (coeff2 > 0)
                            expression3 = coeff1 + "\\xalgebra^4" + " + " + coeff2 + "\\xalgebra^3";
                        else if (coeff2 < 0)
                            expression3 = coeff1 + "\\xalgebra^4" + " " + coeff2 + "\\xalgebra^3";

                        if (coeff3 > 0)
                            expression3 = expression3 + " + " + coeff3 + "\\xalgebra^2";
                        else if (coeff3 < 0)
                            expression3 = expression3 + " " + coeff3 + "\\xalgebra^2";

                        if (coeff4 > 0)
                            expression3 = expression3 + " + " + coeff4 + "\\xalgebra";
                        else if (coeff4 < 0)
                            expression3 = expression3 + " " + coeff4 + "\\xalgebra";

                        if (coeff5 > 0)
                            expression3 = expression3 + " + " + coeff5;
                        else if (coeff5 < 0)
                            expression3 = expression3 + " " + coeff5;


                        int tempAns = (coeff1 * x * x * x * x) + (coeff2 * x * x * x) + (coeff3 * x * x) + (coeff4 * x) + coeff5;
                        Answer = tempAns.ToString();
						QuestionText.text = "Solve for x = " + x + ".";
                    }
                    else
                    {
						coeff1 = GenerateRandomIntegerExcluding01 (-5, 6);
                        coeff4 = Random.Range(-5, 5);
                        coeff5 = Random.Range(-5, 5);
                        coeff6 = Random.Range(-5, 5);
                        

                        if (coeff4 == 1)
                        {
                            expression1 = "(\\yalgebra + \\zalgebra)";
                        }
                        else if (coeff4 == -1)
                        {
                            expression1 = "(\\yalgebra - \\zalgebra)";
                        }
                        else if (coeff4 < -1)
                        {
                            expression1 = "(\\yalgebra " + coeff4 + "\\zalgebra)";
                        }
                        else if (coeff4 > 1)
                        {
                            expression1 = "(\\yalgebra + " + coeff4 + "\\zalgebra)";
                        }
                        else if (coeff4 == 0)
                            expression1 = "\\yalgebra";

                        if (coeff5 == 1)
                        {
                            expression2 = "(\\zalgebra + \\xalgebra)";
                        }
                        else if (coeff5 == -1)
                        {
                            expression2 = "(\\zalgebra - \\xalgebra)";
                        }
                        else if (coeff5 < -1)
                        {
                            expression2 = "(\\zalgebra " + coeff5 + "\\xalgebra)";
                        }
                        else if (coeff5 > 1)
                        {
                            expression2 = "(\\zalgebra + " + coeff5 + "\\xalgebra)";
                        }
                        else if (coeff5 == 0)
                            expression2 = "\\zalgebra";

                        if (coeff6 == 1)
                        {
                            expression3 = "(\\yalgebra + \\xalgebra)";
                        }
                        else if (coeff6 == -1)
                        {
                            expression3 = "(\\yalgebra - \\xalgebra)";
                        }
                        else if (coeff6 < -1)
                        {
                            expression3 = "(\\yalgebra " + coeff6 + "\\xalgebra)";
                        }
                        else if (coeff6 > 1)
                        {
                            expression3 = "(\\yalgebra + " + coeff6 + "\\xalgebra)";
                        }
                        else if (coeff6 == 0)
                            expression3 = "\\yalgebra";							               

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

						QuestionText.text = "Solve for x = " + x + ", y = " + y + ", z = " + z + ".";
                        int tempAns = coeff1 * (y + coeff4 * z) + coeff2 * (z + coeff5 * x) + coeff3 * (y + coeff6 * x);
                        Answer = tempAns.ToString();


                    }
                }
                else if (selector == 3)
                {
                    // coeff4, coeff5 , coeff 6 are denominators
                    coeff4 = Random.Range(2, 6);
                    coeff5 = Random.Range(1, 6);
                    coeff6 = Random.Range(1, 6);
                    a = Random.Range(2, 6);
                    b = Random.Range(1, 6);
                    c = Random.Range(1, 6);

					while ((a * b * c) % coeff4 != 0 || Mathf.Abs (coeff1) == coeff4)
                        coeff4 = Random.Range(2, 6);
					while ((a * b * c) % coeff5 != 0 || Mathf.Abs (coeff2) == coeff5)
						coeff5 = Random.Range(1, 6);
					while ((a * b * c) % coeff6 != 0 || Mathf.Abs (coeff3) == coeff6)
                        coeff6 = Random.Range(1, 6);

                   

                    if (coeff1 != 0)                 
						expression3 = NegativeSign (coeff1) + "\\frac{" + Mathf.Abs (coeff1) + "}{" + coeff4 + "}" + "\\aalgebra\\balgebra^2\\calgebra";
                       
                    if (coeff2 < 0)
                    {
                        if (coeff5 != 1 )
							expression3 += "- \\frac{" + Mathf.Abs(coeff2) + "}{" + coeff5 + "}" + "\\balgebra\\calgebra^2\\aalgebra";
                        else
							expression3 += " - " + Mathf.Abs(coeff2) + "\\balgebra\\calgebra^2\\aalgebra";
                    }
                    else if (coeff2 > 0)
                    {

                        if (coeff5 != 1 && expression3 != " ")
							expression3 += " + \\frac{" + coeff2 + "}{" + coeff5 + "}" + "\\balgebra\\calgebra^2\\aalgebra";
                        else if(expression3 == " " && coeff5 != 1)
							expression3 += " \\frac{" + coeff2 + "}{" + coeff5 + "}" + "\\balgebra\\calgebra^2\\aalgebra";
                        else
							expression3 += " + " + coeff2 + "\\balgebra\\calgebra^2\\aalgebra";
                    }


                    if (coeff3 > 0)
                    {
                        if (coeff6 != 1 && expression3 != " ")
                            expression3 += " + \\frac{" + coeff3 + "}{" + coeff6 + "}" + "\\calgebra^3\\aalgebra\\balgebra^3";
                        else if (coeff6 != 1 )
                            expression3 += " \\frac{" + coeff3 + "}{" + coeff6 + "}" + "\\calgebra^3\\aalgebra\\balgebra^3";
                        else
                            expression3 += " + " + coeff3 + "\\calgebra^3\\aalgebra\\balgebra^3";
                    }
                    else if (coeff3 < 0)
                    {
                        if (coeff6 != 1 && expression3 != " ")
                            expression3 += "- \\frac{" + Mathf.Abs(coeff3) + "}{" + coeff6 + "}" + "\\calgebra^3\\aalgebra\\balgebra^3";
                        else if (coeff6 != 1 )
                            expression3 += " \\frac{" + (coeff3) + "}{" + coeff6 + "}" + "\\calgebra^3\\aalgebra\\balgebra^3";
                        else
                            expression3 += " - " + Mathf.Abs(coeff3) + "\\calgebra^3\\aalgebra\\balgebra^3";
                    }
					QuestionText.text = "Solve for a = " + a + ", b = " + b + ", c = " + c + ".";                               

                    int tempAns = (int)((((float)coeff1 / (float)coeff4) * a * b * b * c) + (((float)coeff2 / (float)coeff5) * b * c * c * a) + (((float)coeff3 / (float)coeff6) * c * c * c * a * b * b * b));
                   
                  
                    Answer = tempAns.ToString();


                }
                QuestionTEX.text = expression3;
            }
            #endregion level1
            #region level2 
            else if (level == 2)
            {
                a = Random.Range(-5, 5);
                b = Random.Range(-5, 5);
                c = Random.Range(-5, 5);


				selector = GetRandomSelector (1, 3);

				if (selector == 1) // Division
                {
                    a = Random.Range(-5, 5);
                    b = Random.Range(-5, 5);
                    c = Random.Range(-5, 5);
					int randSelector = Random.Range (1, 4);
					Debug.Log ("randSelector = " + randSelector);
                    if (randSelector == 1)
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
							expression1 = "\\aalgebra^2";
                        else if (coeff1 == -1)
							expression1 = "- \\aalgebra^2";
						else expression1 = coeff1 + "\\aalgebra^2";

                        if (coeff2 == 1)
                        {   if(expression1 == "")
                                 expression1 += "\\balgebra^2";
                            else
                            {
                                expression1 += " + \\balgebra^2";
                            }
                        }
                        else if (coeff2 == -1)
                            expression1 +=  " - \\balgebra^2";
                        else if (coeff2 > 1)
                            expression1 += " + " + coeff2 + "\\balgebra^2";
                        else if (coeff2 == 0)
                            expression1 += "";
                        else expression1 += coeff2 + "\\balgebra^2";

                        if (coeff3 == 1)
                        {
                            if (expression1 == "")
                                expression1 += "\\calgebra^2";
                            else expression1 += " + \\calgebra^2";
                        }
                        else if (coeff3 == -1)
                            expression1 += " - \\calgebra^2";
                        else if (coeff3 > 1)
                            expression1 += " + " + coeff3 + "\\calgebra^2";
                        else if (coeff3 == 0)
                            expression1 += " ";
                        else expression1 += coeff3 + "\\calgebra^2";


                        //denominator = expression2
                        if (coeff4 == 0)
                            expression2 = "";
                        else if (coeff4 == 1)
                            expression2 = " \\balgebra^2";
                        else if (coeff4 == -1)
                            expression2 = "- \\balgebra^2";
                        else expression2 = coeff4 + "\\balgebra^2";

                        if (coeff5 == 1)
						{   
							if(expression2 != "")
							{
								expression2 += " + \\aalgebra^2";
							}
                            else
                            {
								expression2 += "\\aalgebra^2";
                            }
                        }
                        else if (coeff5 == -1)
							expression2 +=  " - \\aalgebra^2";
                        else if (coeff5 > 1)
							expression2 += " + " + coeff5 + "\\aalgebra^2";
                        else if (coeff5 == 0)
                            expression2 += "";
						else expression2 += coeff5 + "\\aalgebra^2";

                        if (coeff6 == 1)
							expression2 += " + \\calgebra^2";
                        else if (coeff6 == -1)
                            expression2 += " - \\calgebra^2";
                        else if (coeff6 > 1)
                            expression2 += " + " + coeff6 + "\\calgebra^2";
                        else if (coeff6 == 0)
                            expression2 += "";
                        else expression2 += coeff6 + "\\calgebra^2";


                        //expression3 = numerator/denominator
                        expression3 = "\\frac{" + expression1 + "}{" + expression2 + "}";

                        int tempAns = (coeff1 * a * a + coeff2 * b * b + coeff3 * c * c);
                        int tempAns2 = (coeff4 * b * b + coeff5 * a * a + coeff6 * c * c);
						int hcf = MathFunctions.GetHCF (Mathf.Abs (tempAns), Mathf.Abs (tempAns2));
						Answer = NegativeSign (tempAns * tempAns2) + Mathf.Abs (tempAns/hcf) + "/" + Mathf.Abs (tempAns2/hcf);
                      

                    }
					else if (randSelector == 2)
                    {
                        coeff1 = Random.Range(-5, 5);
                        coeff2 = Random.Range(-5, 5);
                        coeff3 = Random.Range(-5, 5);
                       
						while (coeff1 == 0)
						{
							coeff1 = Random.Range(-5, 5);
						}
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
                       if (coeff1 == 1)
                            expression1 = "\\aalgebra\\balgebra\\calgebra";
                        else if (coeff1 == -1)
                            expression1 = "- \\aalgebra\\balgebra\\calgebra";
                        else expression1 = coeff1 + "\\aalgebra\\balgebra\\calgebra";

                        if (coeff2 == 0)
                            expression1 += "";
                        else if (coeff2 == 1)
							expression1 += " + \\aalgebra^2\\balgebra\\calgebra^0";
                        else if (coeff2 == -1)
							expression1 += " - \\aalgebra^2\\balgebra\\calgebra^0";
                        else if (coeff2 > 1)
							expression1 += " + " + coeff2 + "\\aalgebra^2\\balgebra\\calgebra^0";
						else expression1 += " " + coeff2 + "\\aalgebra^2\\balgebra\\calgebra^0";

                        //expression2 = denominator
                        expression2 = "";
                        if (coeff3 == 1)
                            expression2 += "2\\aalgebra\\balgebra^0\\calgebra";
                        else if (coeff3 == -1)
                            expression2 += " -(2\\aalgebra\\balgebra^0\\calgebra)";
                        else expression2 += coeff3 + "(2\\aalgebra\\balgebra^0\\calgebra)";
                    
			            expression3 = "\\frac{" + expression1 + "}{" + expression2 + "}";

                        int tempAns = ((coeff1 * a * b * c) + (coeff2 * a * a * b));
                        int tempAns2 = (coeff3 * 2 * a * c);
						int hcf = MathFunctions.GetHCF (Mathf.Abs (tempAns), Mathf.Abs (tempAns2));
						Answer = NegativeSign (tempAns * tempAns2) + Mathf.Abs (tempAns/hcf) + "/" + Mathf.Abs (tempAns2/hcf);                        
                    }              
					else if (randSelector == 3)
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
							expression1 = "\\aalgebra^2";
                        else if (coeff1 == -1)
							expression1 = "- \\aalgebra^2";
						else expression1 = coeff1 + "\\aalgebra^2";

                        if (coeff2 == 1)
                        {   if (expression1 == " ")
                                expression1 += "\\balgebra\\calgebra";
                            else expression1 += "+ \\balgebra\\calgebra";
                        }
                        else if (coeff2 == -1)
                            expression1 += " " + "- \\balgebra\\calgebra";
                        else if (coeff2 > 1)
                            expression1 += " + " + coeff2 + "\\balgebra\\calgebra";     
                        else if(coeff2 < 0)
                            expression1 += coeff2 + "\\balgebra\\calgebra";


                        if (coeff3 == 1)
                        {   if (expression1 == " ")
                                expression1 = "\\calgebra^2";
                            else expression1 += " + \\calgebra^2";
                        }
                        else if (coeff3 == -1)
                            expression1 += " " + "- \\calgebra^2";
                        else if (coeff3 > 1)
                            expression1 += " + " + coeff3 + "\\calgebra^2";
                        else if (coeff3 < 0)
                            expression1 += coeff3 + "\\calgebra^2";


                        //denominator = expression2
                        if (coeff4 == 0)
                            expression2 = " ";
                        else if (coeff4 == 1)
                            expression2 = "\\aalgebra\\balgebra";
                        else if (coeff4 == -1)
							expression2 = "- \\aalgebra\\balgebra";
                        else expression2 = coeff4 + "\\aalgebra\\balgebra";

                        if (coeff5 == 1)
                        {   if (expression2 == " ")
                                expression2 += "\\calgebra\\aalgebra";
                            else expression2 += " + \\calgebra\\aalgebra";
                        }
                        else if (coeff5 == -1)
                            expression2 += " " + "- \\calgebra\\aalgebra";
                        else if (coeff5 > 1 && expression2 != " ")
                            expression2 += " + " + coeff5 + "\\calgebra\\aalgebra";
                        else if (coeff5 == 0)
                            expression2 += " ";
                        else expression2 += coeff5 + "\\calgebra\\aalgebra";

                        if (coeff6 == 1)
                        {   if (expression2 != " ")
                                expression2 += "+ \\balgebra\\calgebra";
                            else expression2 += "\\balgebra\\calgebra";
                        }
                        else if (coeff6 == -1)
                            expression2 += " " + "- \\balgebra\\calgebra";
                        else if (coeff6 > 1 && expression2 != " ")
                            expression2 += " + " + coeff6 + "\\balgebra\\calgebra";
                        else if (coeff6 == 0)
                            expression2 += " ";
                        else expression2 += coeff6 + "\\balgebra\\calgebra";

                        expression3 = "\\frac{" + expression1 + "}{" + expression2 + "}";
						                       
                        int tempAns = (coeff1 * a * a + coeff2 * b * c + coeff3 * c * c);
                        int tempAns2  = ((coeff4 * a * b) + (coeff5 * c * a) + (coeff6 * b * c));
						int hcf = MathFunctions.GetHCF (Mathf.Abs (tempAns), Mathf.Abs (tempAns2));
						Answer = NegativeSign (tempAns * tempAns2) + Mathf.Abs (tempAns/hcf) + "/" + Mathf.Abs (tempAns2/hcf);
                    }
					QuestionText.text = "Solve for a = " + a + ", b = " + b + ", c = " + c + ".\n(Answer in fraction.)" ;
                }
				else if (selector == 2)           // Power
                {   
					int randSelector = Random.Range (1, 5);
					Debug.Log ("randSelector = " + randSelector);
					if (randSelector == 1)
                    {
                        a = Random.Range(1, 7);
                        b = Random.Range(1, 5);
						expression3 = "{(\\aalgebra\\balgebra)}^{\\balgebra}";

						QuestionText.text = "Solve for a = " + a + ", b = " + b + ".";
                        int tempAns = ((int)Mathf.Pow(b,b))*a;
                        Answer = tempAns.ToString();

                    }
					else if (randSelector == 2)
                    {
                        a = Random.Range(1, 3);
                        b = Random.Range(1, 3);
                        c = Random.Range(1, 9);
                        int tempSel = Random.Range(0, 2);
                        if (tempSel == 0 && (a*b != 1))
							expression3 = "{(\\aalgebra\\balgebra\\calgebra)}^{-\\aalgebra\\balgebra}";
						else expression3 = "{(\\aalgebra\\balgebra\\calgebra)}^{\\aalgebra\\balgebra}";

						QuestionText.text = "Solve for a = " + a + ", b = " + b + ", c = " + c + ".\n(Answer in fraction if it is not an integer.)";
                        float tempAns = (Mathf.Pow((a * b * c), ( a * b)));
                        Answer = tempAns.ToString("F2");
                        if (tempSel == 0)
                            Answer =  " 1/" + tempAns.ToString();


                    }
					else if (randSelector == 3)
                    {
                        a = Random.Range(1, 5);
                        b = Random.Range(1, 5);
                        c = Random.Range(1, 5);
                        x = Random.Range(1, 3);
                        

						expression3 = "{(\\aalgebra^2\\balgebra\\calgebra)}^{\\xalgebra}\\xalgebra\\yalgebra";

						QuestionText.text = "Find coefficient of y for a = " + a + ", b = " + b + ", c = " + c + ", x = " + x + ".";
                        int  tA1 = ((int)Mathf.Pow(a, 2))*b*c;
                        int tempAns = (int)Mathf.Pow(tA1, x);

                        Answer = tempAns.ToString() ;
                    }
					else if (randSelector == 4)
					{
						a = Random.Range(1, 5);
						expression3 = "\\aalgebra^{\\aalgebra}";

						QuestionText.text = "Solve for a = " + a + ".";

						int tempAns = (int)Mathf.Pow(a,a);


						Answer = tempAns.ToString() ;
					}
				}
                QuestionTEX.text = expression3;

            }
            #endregion level2
            #region level3
            if (level == 3)                     // brackets
            {
				selector = GetRandomSelector (1, 3);

				if (selector == 1) 
                {
					int randSelector = Random.Range (1, 3);
					Debug.Log ("randSelector = " + randSelector);
					if (randSelector == 1) // (x3y2 + y2)- (x3y2 - y2)
                    {
						coeff1 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff2 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff3 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff4 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff5 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff6 = GenerateRandomIntegerExcluding0 (-5, 5);
                        x = Random.Range(-3, 4);
                        y = Random.Range(-3, 4);                                      

                        if (coeff2 == 1)
                        {
                            if (coeff3 == 1)
                                expression1 = "\\xalgebra^3\\yalgebra^2 + \\yalgebra^2";
                            else if (coeff3 == -1)
                                expression1 = "\\xalgebra^3\\yalgebra^2 - \\yalgebra^2";
                            else if (coeff3 > 1)
                                expression1 = "\\xalgebra^3\\yalgebra^2 +" + coeff3 + " \\yalgebra^2";
                            else 
								expression1 = "\\xalgebra^3\\yalgebra^2 " + coeff3 + " \\yalgebra^2";
                        }
                        else if (coeff2 == -1)
                        {
                            if (coeff3 == 1)
                                expression1 = "- \\xalgebra^3\\yalgebra^2 + \\yalgebra^2";
                            else if (coeff3 == -1)
                                expression1 = "- \\xalgebra^3\\yalgebra^2 - \\yalgebra^2";
                            else if (coeff3 > 1)
                                expression1 = "- \\xalgebra^3\\yalgebra^2 +" + coeff3 + " \\yalgebra^2";
                            else 
								expression1 = "- \\xalgebra^3\\yalgebra^2 " + coeff3 + " \\yalgebra^2";
                        }
                        else
                        {
                            if (coeff3 == 1)
                                expression1 = coeff2 + "\\xalgebra^3\\yalgebra^2 + \\yalgebra^2";
                            else if (coeff3 == -1)
                                expression1 = coeff2 + "\\xalgebra^3\\yalgebra^2 - \\yalgebra^2";
                            else if (coeff3 > 1)
                                expression1 = coeff2 + "\\xalgebra^3\\yalgebra^2 +" + coeff3 + " \\yalgebra^2";
                            else 
								expression1 = coeff2 + "\\xalgebra^3\\yalgebra^2 " + coeff3 + " \\yalgebra^2";
                        }

                        if (coeff1 == 1)
                            expression1 = "(" + expression1 + ")";
                        else if (coeff1 == -1)
                            expression1 = "-(" + expression1 + ")";
                        else 
							expression1 = coeff1 + "(" + expression1 + ")";

                        // second term
                        
                        if (coeff5 == 1)
                        {
                            if (coeff6 == 1)
                                expression2 = "\\xalgebra^3\\yalgebra^2 + \\yalgebra^2";
                            else if (coeff6 == -1)
                                expression2 = "\\xalgebra^3\\yalgebra^2 - \\yalgebra^2";
                            else if (coeff6 > 1)
                                expression2 = "\\xalgebra^3\\yalgebra^2 +" + coeff6 + " \\yalgebra^2";
                            else 
								expression2 = "\\xalgebra^3\\yalgebra^2 " + coeff6 + " \\yalgebra^2";
                        }
                        else if (coeff5 == -1)
                        {
                            if (coeff6 == 1)
                                expression2 = "- \\xalgebra^3\\yalgebra^2 + \\yalgebra^2";
                            else if (coeff6 == -1)
                                expression2 = "- \\xalgebra^3\\yalgebra^2 - \\yalgebra^2";
                            else if (coeff6 > 1)
                                expression2 = "- \\xalgebra^3\\yalgebra^2 +" + coeff6 + " \\yalgebra^2";
                            else 
								expression2 = "- \\xalgebra^3\\yalgebra^2 " + coeff6 + " \\yalgebra^2";
                        }
                        else
                        {
                            if (coeff6 == 1)
                                expression2 = coeff5 + "\\xalgebra^3\\yalgebra^2 + \\yalgebra^2";
                            else if (coeff6 == -1)
                                expression2 = coeff5 + "\\xalgebra^3\\yalgebra^2 - \\yalgebra^2";
                            else if (coeff6 > 1)
                                expression2 = coeff5 + "\\xalgebra^3\\yalgebra^2 +" + coeff6 + " \\yalgebra^2";
                            else 
								expression2 = coeff5 + "\\xalgebra^3\\yalgebra^2 " + coeff6 + " \\yalgebra^2";
                        }

                        if (coeff4 == 1)
                            expression2 = "+ (" + expression2 + ")";
                        else if (coeff4 == -1)
                            expression2 = "- (" + expression2 + ")";
                        else if (coeff4 >= 0)
                            expression2 = " + " + coeff4 + "(" + expression2 + ")";
                        else 
							expression2 = coeff4 + "(" + expression2 + ")";

                        expression3 = expression1 + expression2;

						QuestionText.text = "Solve for x = " + x + ", y = " + y + ".";

                        int tempAns = (coeff1 * ((coeff2 * x * x * x * y * y) + (coeff3 * y * y)))+ (coeff4 * ((coeff5 * x * x * x * y * y) + (coeff6 * y *y)));


                       Answer = tempAns.ToString();

                    }
					else if (randSelector == 2)                //x-[y-{z2-(x+y)0}]
                    {
						coeff1 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff2 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff3 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff4 = GenerateRandomIntegerExcluding0 (-5, 5);
						coeff5 = GenerateRandomIntegerExcluding0 (-5, 5);

                        x = Random.Range(-5, 5);
                        y = Random.Range(-5, 5);
                        z = Random.Range(-3, 4);
             
                        int pow1 = Random.Range(0, 3);

                        if (coeff5 == 1)
                            expression1 = "+ \\yalgebra";
                        else if (coeff5 == -1)
                            expression1 = "- \\yalgebra";
                        else if (coeff5 < -1)
                            expression1 = coeff5 + "\\yalgebra";
                        else if (coeff5 > 1)
                            expression1 = " + " + coeff5 + "\\yalgebra";

                        if (coeff4 == 1)
                            expression1 = "x" + expression1;
                        else if (coeff4 == -1)
                            expression1 = "-x" + expression1;
                        else 
							expression1 = coeff4 + "\\xalgebra" + expression1;


						expression1 = "{(" + expression1 + ")}^" + pow1;

                        if (coeff3 == 1)
                            expression1 = " + " + expression1;
                        else if (coeff3 == -1)
                            expression1 = " - " + expression1;
                        else if (coeff3 > 1)
                            expression1 = " + " + coeff3 + expression1;
                        else 
							expression1 = coeff3 + expression1;
                    

						expression1 = "\\lbrace{\\zalgebra^2 " + expression1 + "}\\rbrace";

                        if (coeff2 == 1)
                            expression1 = " + " + expression1;
                        else if (coeff2 == -1)
                            expression1 = " - " + expression1;
                        else if (coeff2 > 1)
                            expression1 = " + " + coeff2 + expression1;
                        else if (coeff2 < -1)
                            expression1 = coeff2 + expression1;

						expression1 = "[{\\yalgebra " + expression1 + "}]";


                        if (coeff1 == 1)
                            expression1 = " + " + expression1;
                        else if (coeff1 == -1)
                            expression1 = " - " + expression1;
                        else if (coeff1 > 1)
                            expression1 = " + " + coeff1 + expression1;
                        else if (coeff1 < -1)
                            expression1 = coeff1 + expression1;

                        expression3 = " x " + expression1;

						QuestionText.text = "Solve for x = " + x + ", y = " + y + ", z = " + z + ".";

                        int tempAns = x + coeff1 * (y +  (coeff2 * ((int)Mathf.Pow(z, 2) + coeff3 * ((int)Mathf.Pow((coeff4 * x + coeff5 * y), pow1)))));    //x-[y-{z2-c3*(c4*x+c5*y)^pow1}]


                        Answer = tempAns.ToString();

                    }
                   
                }
				else if (selector == 2)
                {            
					int randSelector = Random.Range (1, 3);
					Debug.Log ("randSelector = " + randSelector);
					if (randSelector == 1)
					{
						int v = Random.Range(-25, 25);   // v
						int u = Random.Range(-25, 25);   // u
						int t = Random.Range(-25, 25);   // t

						while ((v - u) % t != 0 || v == u || Mathf.Abs (v - u) == t)
						{
							u = Random.Range(-25, 25);   
							t = Random.Range(-25, 25);
						}
						QuestionText.text = "Using first equation of motion : v = u + at, determine the value of a.\nGiven : v = " + v + ", u = " + u + ", t = " + t + ".";
                        expression3 = "";
                        
						Answer = ((v - u) / t).ToString();
                    }
					else if (randSelector == 2)
                    {
                        x = Random.Range(-25, 25);   
                        y = Random.Range(-25, 25);

						QuestionText.text = "The equation of a circle with radius \'r\' is given below. Find the value of r,\nif x = " + x + ", y = " + y + " (round to two decimal places).";
                        expression3 = "\\xalgebra^2 + \\yalgebra^2 = \\ralgebra^2";
						float tempAns = MathFunctions.GetRounded (Mathf.Sqrt(x * x + y * y), 2);
                        Answer = tempAns.ToString();
                    }
                }

                QuestionTEX.text = expression3;

            }
            #endregion level3
			#region level4
			else if (level == 4)
			{
				selector = GetRandomSelector(1, 7);
				multiplePossibleAnswers = true;
				List<int> coeff = new List<int>();
				QuestionText.text = "Simplify :";

				if (selector == 1)  
				{
					if (Random.Range (1, 3) == 1)    // x + y - (x - c1y) + (c1x - y)
					{
						coeff.Add (Random.Range (2, 10));

						QuestionTEX.text = string.Format ("{1} + {2} - ({1} - {0}{2}) + ({0}{1} - {2})", coeff[0], "x".Algebra (), "y".Algebra ());
						Answer = string.Format ("{0}{1}+{0}{2}", coeff[0], "x".Algebra (), "y".Algebra ());
					}
					else     // (x + y + z) + c1(x + y + z) - c2(x + y + z)
					{
						coeff.Add (Random.Range (2, 10));
						coeff.Add (Random.Range (2, 10));

						coeff1 = 1 + coeff[0] - coeff[1];

						QuestionTEX.text = string.Format ("({2} + {3} + {4}) + {0}({2} + {3} + {4}) - {1}({2} + {3} + {4})", coeff[0], coeff[1], "x".Algebra (), "y".Algebra (), "z".Algebra ());
						Answer = string.Format ("{0}{1}{2}", AlgebraicDisplayForm (coeff1, "x".Algebra (), true), AlgebraicDisplayForm (coeff1, "y".Algebra ()), AlgebraicDisplayForm (coeff1, "z".Algebra ()));
					}
				}
				else if (selector == 2)  // c1x2 - c1x(x - c2)
				{
					for (int i = 0; i < 2; i++)
						coeff.Add (Random.Range (2, 10));

					QuestionTEX.text = string.Format ("{0}{2}^{{2}} - {0}{2}({2} - {1})", coeff[0], coeff[1], "x".Algebra ());
					Answer = string.Format ("{0}{1}", coeff[0] * coeff[1], "x".Algebra ());
				}
				else if (selector == 3)  // c1x - [c2y - {c3x - (c4y - c5x)}]
				{
					for (int i = 0; i < 5; i++)
						coeff.Add (Random.Range (2, 10));

					QuestionTEX.text = string.Format ("{0}{5} - [{1}{6} - \\lbrace{{{2}{5} - ({3}{6} - {4}{5})}}\\rbrace]", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], "x".Algebra (), "y".Algebra ());
					Answer = string.Format ("{0}{2}-{1}{3}", coeff[0] + coeff[2] + coeff[4], coeff[1] + coeff[3], "x".Algebra (), "y".Algebra ());
				}
				else if (selector == 4)  // c1y - [c2y + x - {c3x + (c4x - c5y + y)}]
				{
					for (int i = 0; i < 5; i++)
						coeff.Add (Random.Range (2, 10));

					coeff1 = coeff[0] - coeff[1] - coeff[4] + 1;

					QuestionTEX.text = string.Format ("{0}{6} - [{{{1}{6} + {5} - \\lbrace{{{{{2}{5} + ({{{3}{5} - {4}{6} + {6}}})}}}}\\rbrace}}]", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], "x".Algebra (), "y".Algebra ());
					Answer = string.Format ("{0}{1}", coeff[3] + coeff[2] - 1, "x".Algebra (), AlgebraicDisplayForm (coeff1, "y".Algebra ()));
				}
				else if (selector == 5)  // c1x(c2x2 - c3x + c4) - c5x(c6x2 - c7x - c8) - c9x(x2 - c10x + c11)
				{
					for (int i = 0; i < 11; i++)
						coeff.Add (Random.Range (2, 10));

					coeff1 = coeff[0] * coeff[1] - coeff[4] * coeff[5] - coeff[9];
					coeff2 = -coeff[0] * coeff[2] + coeff[4] * coeff[6] + coeff[8] * coeff[9];
					coeff3 = coeff[0] * coeff[3] + coeff[4] * coeff[7] - coeff[8] * coeff[10];	

					QuestionTEX.text = string.Format ("{0}{11}({1}{11}^{{2}} - {2}{11} + {3}) - {4}{11}({5}{11}^{{2}} - {6}{11} + {7}) - {8}{11}({11}^{{2}} - {9}{11} + {10})", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], coeff[5], coeff[6], coeff[7], coeff[8], coeff[9], coeff[10], "x".Algebra ());
					Answer = string.Format ("{0}{1}{2}", AlgebraicDisplayForm (coeff1, "x".Algebra () + "^{3}", true), AlgebraicDisplayForm (coeff2, "x".Algebra () + "^{2}"), AlgebraicDisplayForm (coeff3, "x".Algebra ()));
				}
				else if (selector == 6)  // c1x - c2y - [c3x - c4y - {c5x - y - (x + c6y)}]
				{
					for (int i = 0; i < 6; i++)
						coeff.Add (Random.Range (2, 10));

					coeff1 = coeff[0] - coeff[2] + coeff[4] -1;
					coeff2 = -coeff[1] + coeff[3] - coeff[5] - 1;

					QuestionTEX.text = string.Format ("{0}{6} - {1}{7} - [{{{2}{6} - {3}{7} - \\lbrace{{{{{4}{6} - {7} - ({{{6} + {5}{7}}})}}}}\\rbrace}}]", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], coeff[5], "x".Algebra (), "y".Algebra ());
					Answer = string.Format ("{0}{1}", AlgebraicDisplayForm (coeff1, "x".Algebra (), true), AlgebraicDisplayForm (coeff2, "y".Algebra ()));
				}
			}
			#endregion
			userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw>("Text");
            userAnswerLaText.text = "";
			Debug.Log (Answer);
		}

		public int GenerateRandomIntegerExcluding01 (int minRange, int maxRange)
		{
			int randNumber = Random.Range (minRange, maxRange);
			while (randNumber == 0 || randNumber == 1 || randNumber == -1) {
				randNumber = Random.Range (minRange, maxRange);
			}
			return randNumber;
		}

		public int GenerateRandomIntegerExcluding0 (int minRange, int maxRange)
		{
			int randNumber = Random.Range (minRange, maxRange);
			while (randNumber == 0) {
				randNumber = Random.Range (minRange, maxRange);
			}
			return randNumber;
		}

		public string Sign (float number) //returns sign (both + and -)
		{
			return number < 0 ? "-" : "+";
		}

		public string NegativeSign (float number) // returns sign only for negative number
		{
			return number < 0 ? "-" : "";
		}

		public string AlgebraicDisplayForm (int constant, string variable, bool isFirstTerm = false)
		{
			string str;
			if (constant == 0) {
				return "";
			} else if (constant > 0) {
				if (constant == 1){
					str = "+";
				} else {
					str = "+" + constant;
				}
			} else {
				if (constant == -1){
					str = "-";
				} else {
					str = "-" + (-constant);
				}
			}
			str += variable;
			if (isFirstTerm && constant > 0) {
				return str.Substring (1);
			} else {
				return str;	
			}
		}

        public override void numPadButtonPressed(int value)
        {
            if (ignoreTouches)
            {
                return;
            }
            if (value <= 9)
            {
                userAnswerLaText.text += value.ToString();
            }
            else if (value == 10)
            {    //Back
                if (userAnswerLaText.text.Length > 0)
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
            }
            else if (value == 11)
            {   // /
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                userAnswerLaText.text += "/";
            }
            else if (value == 12)
            {   // :
                if (checkLastTextFor(new string[1] { "-" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                userAnswerLaText.text += "-";
            }
            else if (value == 13)
            {   // .
                if (checkLastTextFor(new string[1] { "." }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
                userAnswerLaText.text += ".";
            }
            else if (value == 14)
            {   // All Clear
              
                userAnswerLaText.text = "";
            }
			else if (value == 15)
			{   // :
				if (checkLastTextFor(new string[1] { "\\xalgebra" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += "\\xalgebra";
			}
			else if (value == 16)
			{   // :
				if (checkLastTextFor(new string[1] { "\\yalgebra" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += "\\yalgebra";
			}
			else if (value == 17)
			{   // :
				if (checkLastTextFor(new string[1] { "\\zalgebra" }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += "\\zalgebra";
			}
        }
    }
}
