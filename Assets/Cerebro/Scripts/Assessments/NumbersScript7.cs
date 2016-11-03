using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MaterialUI;
using B83.ExpressionParser;

namespace Cerebro {
	public class NumbersScript7 : BaseAssessment {

		public TEXDraw subQuestionText;

		private string Answer;

		// Use this for initialization
		void Start () {
			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "NS07", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}

        bool checkFractions(string[] userAnswers, string[] correctAnswers)
        {
            if (userAnswers[0].Contains("-") && !userAnswers[0].StartsWith("-"))
                return false;
            float num1 = float.Parse(userAnswers[0]);
            float num2 = float.Parse(correctAnswers[0]);
            float den2 = 1;
            if (correctAnswers.Length == 2)
            {                
            den2 = float.Parse(correctAnswers[1]);             
            }
            float den1 = 1;
            if (userAnswers.Length == 2)
            {
                if (userAnswers[1].Contains("-") && !userAnswers[1].StartsWith("-"))
                    return false;
                den1 = float.Parse(userAnswers[1]);
                if (den1 == 0)
                    return false;
            }


            if (Mathf.Abs((num1 / num2) - (den1 / den2)) <= 0.0001)
            {
                return true;
            }
            return false;
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

            questionsAttempted++;
            updateQuestionsAttempted();
            if (userAnswerText.text.EndsWith("/") || userAnswerText.text.EndsWith("-") || userAnswerText.text.StartsWith("/") )
            {
               
                correct = false;
            }else if((userAnswerText.text.Contains("-") && !userAnswerText.text.Contains("/") && (userAnswerText.text.IndexOf("-") != userAnswerText.text.LastIndexOf("-")) ))
            {
                correct = false;
            }
            else if (!Answer.Contains("/") && !userAnswerText.text.Contains("/"))
            {
                var answerNumber = float.Parse(Answer);
                float userAnswerNumber = 0;
                if (float.TryParse(userAnswerText.text, out userAnswerNumber))
                {
                    userAnswerNumber = float.Parse(userAnswerText.text);
                }

                if (answerNumber == userAnswerNumber)
                {
                    correct = true;
                }
                else {
                    correct = false;
                }
            }
            else
            {
                var answerSplits = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
                var userAnswerSplits = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
               
               
                    correct = checkFractions(userAnswerSplits, answerSplits);
               
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

		protected override void GenerateQuestion () {
            ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			level = Queslevel;

			answerButton = GeneralButton;
			subQuestionText.gameObject.SetActive (false);
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

           
            #region L1
            if (level == 1) {
				selector = GetRandomSelector (1, 6);
               
                QuestionLatext.text = "";
                subQuestionText.text = "";

                if (selector == 1) {                                               
                    int a = Random.Range(0, 100);
                    int b = Random.Range(a+1, 1000);
					QuestionLatext.text = "How many whole numbers are there between " + a + " and " + b + "?";
                    int Ans = b - a - 1;					
					Answer = Ans.ToString ();
				} else if (selector == 2) {											
					QuestionLatext.text = "Find the value of \\xalgebra:";
					int p = Random.Range (5, 100);
					int q = Random.Range (5, 100);
                    int r = Random.Range(1, 100);
					subQuestionText.gameObject.SetActive (true);

                    subQuestionText.text = p + "\\times(  \\xalgebra   + " + r + " ) = ";

                    subQuestionText.text += (p +"\\times" + q) + " + " + (p +"\\times"+ r);

                    Answer = q.ToString();


				} else if (selector == 3) {											
					subQuestionText.gameObject.SetActive (true);
                    int a = Random.Range(2, 30);
                    int b = Random.Range(2, 30);
                    int e = Random.Range(2, 30);
                    int f = Random.Range(2, 30);
                 
					QuestionLatext.text = "Using the properties of integers, evaluate:";

                    subQuestionText.text = a + "\\times " + b +" + (" + (-e) + "\\times" + f + ")";
                    int Ans = (a * b ) - (e * f);
					Answer =Ans.ToString ();
				} else if (selector == 4) {											
					subQuestionText.gameObject.SetActive (true);
                    int a = Random.Range(-200, 200);
                   
                    QuestionLatext.text = "Find the value of \\xalgebra:";
                    subQuestionText.text =   "\\xalgebra + (" +  a + ") = 0";
                    Answer = (-a ).ToString();

                }              
                else if (selector == 5)
                {
                    subQuestionText.gameObject.SetActive(true);
                    int a = Random.Range(-1000, 1000);
                    while(a == 0)
                        a = Random.Range(-1000, 1000); 
                    int b = Random.Range(-1000, 1000);
                    while (b == 0 || a % b == 0 )
                        b = Random.Range(-50, 999);
                   
					QuestionLatext.text = "Find the value of \\xalgebra:";
					subQuestionText.text = "\\frac{" + a +"}{"+b+"} \\times ( \\xalgebra ) = 1" ;
                    int hcf = MathFunctions.GetHCF(a, b);
                    while (hcf != 1)
                    {
                        a /= hcf;
                        b /= hcf;
                        hcf = MathFunctions.GetHCF(a, b);
                    }
                    if (a*b > 0)
                         Answer = (Mathf.Abs(b)).ToString() + "/" + (Mathf.Abs(a)).ToString();
                    else Answer = (-Mathf.Abs(b)).ToString() + "/" + (Mathf.Abs(a)).ToString();
                    if (b == 0)
                        Answer = 0.ToString();
                    {
                        if (a == 1)
                            Answer = (b).ToString();
                        else if (a == -1)
                            Answer = (-b).ToString();
                    }

                }
                
            }
            #endregion
            #region L2
            else if (level == 2) {
				selector = GetRandomSelector (1, 6);
                QuestionLatext.text = "";
                subQuestionText.text = "";
               if (selector == 1)
                {
                    //-a[(-b-\\frac{c}{d})- e]
                    subQuestionText.gameObject.SetActive(true);
                    int a = 0, b = 0, c = 0, d = 0, e = 0;

                    if (a == 0 || a == 1)
                        a = Random.Range(-2, 3);
                    if (b == 0)
                        b = Random.Range(-100, 100);
                    if (c == 0)
                        c = Random.Range(-100, 100);
                    if (d == 0 || d == 1)
                        d = Random.Range(-10, 10);
                    if (e == 0)
                        e = Random.Range(-100, 100);
                    while (d == 0 || (10 * c) % d != 0)
                        d = Random.Range(-100, 100);


                    QuestionLatext.text = "Evaluate:";
                    if (e < 0)
						subQuestionText.text = -a + "{[{({" + (-b) + "- \\frac{" + c + "}{" + d + "} })" + "- {({" + e + "})}}]}";
					else subQuestionText.text = -a + "{[{({" + (-b) + "- \\frac{" + c + "}{" + d + "} })" + "-" + e + "}}]}";
                    int Ans = -a * (-(d * b) - c - (d * e));
                    int Ans2 = d;
                    int hcf = MathFunctions.GetHCF(Ans, Ans2);
                    while (hcf != 1)
                    {
                        Ans /= hcf;
                        Ans2 /= hcf;
                        hcf = MathFunctions.GetHCF(Ans, Ans2);
                    }
                    if (Ans * Ans2 > 0)
                        Answer = Mathf.Abs(Ans).ToString() + "/" + Mathf.Abs(Ans2).ToString();
                    else
                        Answer = (-Mathf.Abs(Ans)).ToString() + "/" + Mathf.Abs(Ans2).ToString();
                    if (Ans == 0)
                        Answer = 0.ToString();
                    else
                    {
                        if (Ans2 == 1)
                            Answer = (Ans).ToString();
                        else if (Ans2 == -1)
                            Answer = (-Ans).ToString();
                    }
                }
                else if (selector == 2)
                {                                               //a[-b -[c+d( \\frac{ef}{g} + (h-i))]]
                    subQuestionText.gameObject.SetActive(true);
                    int a = Random.Range(2, 10);
                    int b = Random.Range(1, 10);
                    int c = Random.Range(1, 10);
                    int d = Random.Range(2, 10);
                    int e = Random.Range(2, 10);
                    int f = Random.Range(2, 10);
                    int g = Random.Range(2, 10);
                    int h = Random.Range(1, 10);
                    int i = Random.Range(1, 10);

                    QuestionLatext.text = "Simplify:";

					subQuestionText.text = a + "{[{" + (-b) + "-{[{" + c + "+" + d + "{({ " + e + "\\times" + f + "\\div" + g + " + {({" + h + "-" + i + "})}})}}]}}]}";
                    int Ans = a * (-(g * b) - (g * c) - (d * e * f) - (g * d * h) + (g * d * i));
                    int Ans2 = g;
                    int hcf = MathFunctions.GetHCF(Ans, Ans2);
                    while (hcf != 1)
                    {
                        Ans /= hcf;
                        Ans2 /= hcf;
                        hcf = MathFunctions.GetHCF(Ans, Ans2);
                    }
                    if (Ans * Ans2 > 0)
                        Answer = Mathf.Abs(Ans).ToString() + "/" + Mathf.Abs(Ans2).ToString();
                    else
                        Answer = (-Mathf.Abs(Ans)).ToString() + "/" + Mathf.Abs(Ans2).ToString();
                    if (Ans == 0)
                        Answer = 0.ToString();
                    {
                        if (Ans2 == 1)
                            Answer = (Ans).ToString();
                        else if (Ans2 == -1)
                            Answer = (-Ans).ToString();
                    }
                }
                else if (selector == 3)
                {
                    int c = Random.Range(1, 10);
                    int a = Random.Range(c + 1, 15);
                    int b = Random.Range(1, 15);

                    QuestionLatext.text = "The division of a whole number N by " + a + " gives a quotient of " + b + " and a remainder of " + c + ". Find N.";
                    int Ans = (b * a) + c;
                    Answer = Ans.ToString();

                }
                else if (selector == 4)
                {
                    int a = Random.Range(1, 10);
                    int b = Random.Range(1, 10);
                    int c = Random.Range(1, 10);
                    int d = Random.Range(1, 10);

                    while (a == b || a == c || a == d)
                    {
                         b = Random.Range(1, 10);
                         c = Random.Range(1, 10);
                         d = Random.Range(1, 10);
                    }
                    int n = Random.Range(1, 3);
                    string s = "";
                    int Ans = 0;
                    if(n == 1)
                    {
                        s += a.ToString() + b.ToString() + c.ToString() + d.ToString();
                        Ans = 999 * a;
                    }
                    else if(n ==2)
                    {
                        s += b.ToString() + a.ToString() + c.ToString() + d.ToString();
                        Ans = 99 * a;
                    }
                    else if(n==3)
                    {
                        s += c.ToString() + b.ToString() + a.ToString() + d.ToString();
                        Ans = 9 * a;
                    }

                    QuestionLatext.text = "Calculate the difference in place value and face value of " + a + " in " + s + ".";
                    Answer = Ans.ToString();

                }
                else if (selector == 5)
                {
                    int a = Random.Range(1, 10);
                    int b = Random.Range(1, 10);
                    while(a == b)
                        b = Random.Range(1, 10);
                    int c = Random.Range(1, 10);
                    while (a == c)
                        c = Random.Range(1, 10);
                    int d = Random.Range(1, 10);
                    while (a == d)
                        d = Random.Range(1, 10);
                    int e = Random.Range(1, 10);
                    while (a == e)
                        e = Random.Range(1, 10);

                    string s1 = b.ToString() + c.ToString() + a.ToString() + d.ToString();
                    string s2 = "";
                    int tmep = Random.Range(1, 4) , Ans = 0;
                  
                    if(tmep == 1)
                    {
                        s2 = a.ToString() + e.ToString() + b.ToString() + d.ToString();
                        Ans = (-990) * a;
                    }else if(tmep == 2)
                    {
                        s2 = c.ToString() + e.ToString() + a.ToString() + b.ToString();
                        Ans = 0;
                    }
                    else
                    {
                        s2 = c.ToString() + d.ToString() + e.ToString() + a.ToString();
                        Ans = 9 * a;
                    }

                    int num2 = int.Parse(s1);
                    int num3 = int.Parse(s2);
                    QuestionLatext.text = "What is the difference of place values of " + a + " in the numbers " + num2 + " and " + num3 + "?";
                    Answer = Ans.ToString();
                }
                //else if (selector == 4)
                //{
                //    int a = Random.Range(1, 4);
                //    int b = Random.Range(4, 7);
                //    int c = Random.Range(7, 10);
                  
                //    QuestionLatext.text = "How many 3 digit numbers can be formed by using " + a + ", " + b + ", " + c + " as its digits without using repetition of digits?";

                //    Answer = 6.ToString();
                //}
              
            }
            #endregion
            #region L3
            else if (level == 3) {
				selector = GetRandomSelector (1, 4);
              
                QuestionLatext.text = "";
                subQuestionText.text = "";
               if (selector == 1)
                {
                    int a = Random.Range(1, 1000);
                    int b = Random.Range(1, 10);
                    int c = Random.Range(1, 100);
                    int d = Random.Range(2, 10);
                    while ((a / d) < (c / d))
                    {
                        a = Random.Range(1, 1000);
                        c = Random.Range(1, 100);
                    }
                    subQuestionText.gameObject.SetActive(true);

                    QuestionLatext.text = "A drum full of rice weighs \\xalgebra kgs. If the empty drum weighs \\yalgebra kgs. Find the weight(in kgs) of rice in the drum.";
                    subQuestionText.text = "\\xalgebra = \\frac{" + a + "}{" + b + "}, \\yalgebra = \\frac{" + c + "}{" + d + "}";
                    int a1 = (a * d) - (c * b);
                    int a2 = b * d;
                    int hcf = MathFunctions.GetHCF(a1, a2);
                    while (hcf != 1)
                    {
                        a1 /= hcf;
                        a2 /= hcf;
                        hcf = MathFunctions.GetHCF(a1, a2);
                    }
                    Answer = a1.ToString() + "/" + a2.ToString();
                    if (a1 == 0)
                        Answer = 0.ToString();
                    else
                    {
                        if (a2 == 1)
                            Answer = (a1).ToString();
                        else if (a2 == -1)
                            Answer = (-a1).ToString();
                    }
                }
                else if (selector == 2)
                {
                    int rand = Random.Range(0, 2);
                    if (rand == 1)
                    {

                        int a = 0;
                        int b = 0;
                        int c = 0;
                        int d = 0;
                        while (a == b || b == c || c == d || d == b || a == c || a == d)
                        {
                            a = Random.Range(1, 10);
                            b = Random.Range(1, 10);
                            c = Random.Range(1, 10);
                            d = Random.Range(1, 10);
                        }
                        int[] temp = { a, b, c, d };

                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = i; j < 4; j++)
                            {
                                if (temp[i] < temp[j])
                                {
                                    int t = temp[j];
                                    temp[j] = temp[i];
                                    temp[i] = t;
                                }
                            }
                        }



                        QuestionLatext.text = "Write the largest 4 digit number using the digits " + a + ", " + b + ", " + c + ", " + d + " without repetition of digits.";
                        int Ans = temp[0] * 1000 + temp[1] * 100 + temp[2] * 10 + temp[3];
                        Answer = Ans.ToString();

                    }
                    else
                    {
                        int a = 0;
                        int b = 0;
                        int c = 0;
                        int d = 0;
                        while (a == b || b == c || c == d || d == b || a == c || a == d)
                        {
                            a = Random.Range(1, 10);
                            b = Random.Range(1, 10);
                            c = Random.Range(1, 10);
                            d = Random.Range(1, 10);
                        }
                        int big = Mathf.Max(Mathf.Max(a, b), Mathf.Max(c, d));
                        QuestionLatext.text = "Write the largest 4 digit number using the digits " + a + ", " + b + ", " + c + ", " + d + " with repetition of digits allowed.";
                        int Ans = big * 1000 + big * 100 + big * 10 + big;
                        Answer = Ans.ToString();

                    }
                }
                else if (selector == 3)
                {

                    subQuestionText.gameObject.SetActive(true);
                    int a = Random.Range(1, 10);
                    int b = Random.Range(2, 10);
                    int c = Random.Range(1, 10);
                    int d = Random.Range(2, 10);
                    int e = Random.Range(1, 10);
                    int f = Random.Range(2, 10);
                    int num = Random.Range(10000, 100000);
                    while (num % 1000 != 0)
                        num = Random.Range(10000, 100000);
                    int temp;

                    while ((num * a) % b != 0 || a >= b)
                    {
                        a = Random.Range(1, 10);
                        b = Random.Range(2, 10);
                    }
                    // r1 = a / b;                     
                    temp = num - ((a * num) / b);
                    CerebroHelper.DebugLog(temp + " : 1");
                    while ((temp * c) % d != 0 || c >= d)
                    {
                        c = Random.Range(1, 10);
                        d = Random.Range(2, 10);
                    }
                    // r2 = c / d;
                    temp = temp - ((c * temp) / d);
                    CerebroHelper.DebugLog(temp + " : 2");
                    while ((temp * e) % f != 0 || e >= f)
                    {
                        e = Random.Range(1, 10);
                        f = Random.Range(2, 10);
                    }
                    //    r3 = e / f;
                    temp = temp - ((e * temp) / f);
                    CerebroHelper.DebugLog(temp + " : 3");
                    QuestionLatext.text = "Adrian earns Rs. " + num + " per month. He spends \\xalgebra of his income on food; \\yalgebra of the remainder on rent and \\zalgebra of the remainder on education. How much money is still left with him?";
                    subQuestionText.text = "\\xalgebra = \\frac{" + a + "}{" + b + "}, \\yalgebra = \\frac{" + c + "}{" + d + "}, \\zalgebra = \\frac{" + e + "}{" + f + "}";

                    Answer = temp.ToString();

                }
               
			}
            #endregion L3
            userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
			CerebroHelper.DebugLog (Answer);
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
			} else if (value == 12) {   // -
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
            else if (value == 13)
            {   // /
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += "/";
            }
            else if (value == 14)
            {   // .
                if (checkLastTextFor(new string[1] { "." }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += ".";
            }
        }
	}
}
