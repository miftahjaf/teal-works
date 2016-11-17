using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class PowerAndRootsScript7 : BaseAssessment {

		//public Text subQuestionText;

		private string Answer;
        public GameObject MCQ;
        void Start () {
			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "PAR07", "S01", "A01");

			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}
			
		public override void SubmitClick(){
			if (ignoreTouches || userAnswerText.text == "") {
				return;
			}
			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = true;

			questionsAttempted++;
			updateQuestionsAttempted ();

            //if((userAnswerText.text.LastIndexOf(".") != userAnswerText.text.IndexOf(".")))
            //{
            //    var userAnswers = userAnswerText.text.Split(new string[] { "." }, System.StringSplitOptions.None);

            //    if(userAnswers[0] != "" &&  userAnswers[0].Contains("."))
            //    {
            //        correct = false;
            //    }

            //    if(userAnswers.Length == 2 )
            //    {
            //        if (userAnswers[1].Contains("."))
            //            correct = false;
            //    }

            //}
            if (MCQ.activeSelf)
            {
                if (userAnswerText.text == Answer)
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
                if(!userAnswerText.text.Contains("/") && (userAnswerText.text.LastIndexOf(".") != userAnswerText.text.IndexOf(".")))
                {
                    correct = false;
                }
                else if (userAnswerText.text.EndsWith("/") || userAnswerText.text.EndsWith("-") || userAnswerText.text.StartsWith("/") || userAnswerText.text == "." || (userAnswerText.text.Contains("-") && !userAnswerText.text.Contains("/") && !userAnswerText.text.StartsWith("-")))
                {
                    correct = false;
                    CerebroHelper.DebugLog("1");
                }
                else
                {
                    if (Answer.Contains("."))
                    {


                        float user = -9999.9999f;
                        if (userAnswerText.text.Contains("/"))
                        {
                            var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                            if ((userAnswers[0].Contains("-") && !userAnswers[0].StartsWith("-")) || (userAnswers[1].Contains("-") && !userAnswers[1].StartsWith("-")) || (userAnswers[1].LastIndexOf(".") != userAnswers[1].IndexOf(".")) || (userAnswers[0].LastIndexOf(".") != userAnswers[0].IndexOf(".")))
                            {
                                correct = false;
                            }
                            else
                            {
                                float ua1 = float.Parse(userAnswers[0]);
                                float ua2 = float.Parse(userAnswers[1]);
                                if (ua2 == 0.0f)
                                    correct = false;
                                else
                                    user = ua1 / ua2;
                            }
                        }
                        else
                        {
                            user = float.Parse(userAnswerText.text);
                        }
                        float correctAns = float.Parse(Answer);
                        if (Mathf.Abs(user - correctAns) <= 0.001)
                            correct = true;
                        else
                            correct = false;
                        CerebroHelper.DebugLog("2");

                    }
                    else if (Answer.Contains("/"))
                    {
                        CerebroHelper.DebugLog("3");
                        var correctAnswers = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
                        var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                        correct = checkFractions(userAnswers, correctAnswers);

                    }
                    else if (userAnswerText.text.Contains("/"))
                    {
                        CerebroHelper.DebugLog("4");
                        var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                        if ((userAnswers[0].Contains("-") && !userAnswers[0].StartsWith("-")) || (userAnswers[1].Contains("-") && !userAnswers[1].StartsWith("-")) || (userAnswers[1].LastIndexOf(".") != userAnswers[1].IndexOf(".")) || (userAnswers[0].LastIndexOf(".") != userAnswers[0].IndexOf(".")))
                        {
                            correct = false;
                        }
                        else {
                            CerebroHelper.DebugLog("5");
                            float ua1 = float.Parse(userAnswers[0]);
                            float ua2 = float.Parse(userAnswers[1]);
                            if (ua2 == 0 || (((float)ua1 / (float)ua2) != float.Parse(Answer)))
                                correct = false;
                        }
                    }
                    else
                    {
                        CerebroHelper.DebugLog("6");
                        CerebroHelper.DebugLog(float.Parse(userAnswerText.text) + " : DAP");
                        if (float.Parse(userAnswerText.text) == float.Parse(Answer))
                        {
                            correct = true;
                        }
                        else {
                            correct = false;
                        }

                    }

                }
            }

			if (correct == true) {
				if (Queslevel == 1) {
					increment = 5;
				} else if (Queslevel == 2) {
					increment = 5;
				} else if (Queslevel == 3) {
					increment = 10;
				} else if (Queslevel == 4) {
					increment = 10;
				}else if (Queslevel == 5) {
                    increment = 15;
                }else if (Queslevel == 6) {
                    increment = 15;
                }

                UpdateStreak(8,10);

				updateQuestionsAttempted ();
				StartCoroutine (ShowCorrectAnimation());
			} 
			else {
				for (var i = 0; i < scorestreaklvls.Length; i++) {
					scorestreaklvls [i] = 0;
				}
				StartCoroutine (ShowWrongAnimation());
			}

			base.QuestionEnded (correct, level, increment, selector);

		}


		bool checkFractions (string[] userAnswers, string[] correctAnswers)
		{
            if (userAnswers[0].Contains("-") && !userAnswers[0].StartsWith("-"))
                return false;
            float num1 = float.Parse (userAnswers [0]);
			float num2 = float.Parse (correctAnswers [0]);
			float den2 = float.Parse (correctAnswers [1]);
			float den1 = 1;
			if (userAnswers.Length == 2) {
                if (userAnswers[1].Contains("-") && !userAnswers[1].StartsWith("-"))
                    return false;
                den1 = float.Parse (userAnswers [1]);
			}


			if (Mathf.Abs((num1 / num2) - (den1 / den2)) <= 0.0001 ) {
				return true;
			}
			return false;
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
                if (numPad.activeSelf)
                {               // is not MCQ type question
                    CerebroHelper.DebugLog("going in if");
                    userAnswerText.text = Answer.ToString();
                    userAnswerText.color = MaterialColor.green800;
                }
                else
                {
                    CerebroHelper.DebugLog("going in else");
                    userAnswerText.color = MaterialColor.textDark;
                   
                }
                
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

		string getFraction(int num1,int num2){
			string res;
			if (num1 == num2)
				res = "1";
			else if (num1 == 0)
				res = "0";
			else{
				if (num1 < 0 && num2 < 0)
					res = (-num1).ToString () + "/" + (-num2).ToString ();
				else if (num1 < 0) {
					res = "-" + (-num1).ToString () + "/" + num2.ToString ();
				}
				else if (num2 < 0) {
					res = "-" + (num1).ToString () + "/" + (-num2).ToString ();
				}
				else
					res = num1.ToString () + "/" + num2.ToString ();
			}
			return res;

		}

        public void MCQOptionClicked(int value)
        {
            if (ignoreTouches)
            {
                return;
            }
            AnimateMCQOption(value);
            userAnswerText = MCQ.transform.Find("Option" + value.ToString()).Find("Text").GetComponent<Text>();
            
            answerButton = MCQ.transform.Find("Option" + value.ToString()).GetComponent<Button>();
            SubmitClick();
        }

        IEnumerator AnimateMCQOption(int value)
        {
            var GO = MCQ.transform.Find("Option" + value.ToString()).gameObject;
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1.2f, 1.2f, 1), false));
            yield return new WaitForSeconds(0.2f);
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1, 1, 1), false));
        }

        protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
            // Generating the parameters
            level = Queslevel;

			answerButton = GeneralButton;
            GeneralButton.gameObject.SetActive(false);
            MCQ.gameObject.SetActive(false);
            numPad.SetActive(true);


            if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
           
            #region L1
            if (level == 1)
            {
                int num1;
                int power1;
                
                int power2;
                selector = GetRandomSelector(1, 6);
                QuestionText.text = "";
                QuestionLatext.text = "";
                if (selector == 1)
                {                                               //compute the powers
                    num1 = Random.Range(2, 6);
                    power1 = Random.Range(3, 7);
                    QuestionLatext.text = num1.ToString() + "^" + power1.ToString();
                    GeneralButton.gameObject.SetActive(true);
                    QuestionText.text = "Compute the power : ";
                    Answer = Mathf.Ceil(Mathf.Pow(num1, power1)).ToString();
                }
                else if (selector == 2)
                {   //power is 0
                    num1 = Random.Range(1, 401);
                    power1 = 0;
                    QuestionText.text = "Compute the power : ";
                    QuestionLatext.text = num1.ToString() + "^" + power1.ToString();
                    GeneralButton.gameObject.SetActive(true);
                    Answer = "1";
                }             
                else if (selector == 3)
                {                                           //product of two powers
                    num1 = Random.Range(2, 101);
                    power1 = Random.Range(2, 50);
                    power2 = Random.Range(2, 50);
                    QuestionText.text = "Compute the power : ";
                    QuestionLatext.text = num1.ToString() + "^{" + power1.ToString() + "}\\cdot"
                    + num1.ToString() + "^{" + power2.ToString() + "} = " + num1.ToString() + "^? ";
                    GeneralButton.gameObject.SetActive(true);
                    Answer = (power1 + power2).ToString();
                }
                else if (selector == 4)
                {                           //division of two powers
                    num1 = Random.Range(2, 101);
                    power1 = Random.Range(2, 51);
                    power2 = Random.Range(2, 51);
                    int temp;
                    if (power2 > power1)
                    {
                        temp = power1;
                        power1 = power2;
                        power2 = temp;
                    }
                    QuestionText.text = "Compute the power : ";
                    QuestionLatext.text = num1.ToString() + "^{" + power1.ToString() + "}\\div "
                    + num1.ToString() + "^{" + power2.ToString() + "} = " + num1.ToString() + "^? ";
                    GeneralButton.gameObject.SetActive(true);
                    Answer = (power1 - power2).ToString();
                }
                else if (selector == 5)
                {   //power in negative numbers
                    num1 = -1;
                    power1 = Random.Range(1, 100);
                    QuestionText.text = "Compute the power : ";
					QuestionLatext.text = "{(" + num1.ToString() + ")}^{" + power1.ToString() + "}";
                    GeneralButton.gameObject.SetActive(true);
                    if (power1 % 2 == 0)
                        Answer = "1";
                    else
                        Answer = "-1";
                }             
            }
            #endregion L1
            #region L2
            else if (level == 2) {
				selector = GetRandomSelector (1, 6);
                int num1;
                int power1;
                int num2;
                int power2;
                QuestionText.text = "";
                QuestionLatext.text = "";
                if (selector == 1)
                {         //distribution of two powers
                    
                    num1 = Random.Range(2, 21);
                    num2 = Random.Range(2, 21);
                    power1 = Random.Range(1, 101);
                    GeneralButton.gameObject.SetActive(true);
                    QuestionText.text = "Compute the power : ";
                                
                        QuestionLatext.text = num1.ToString() + "^{" + power1.ToString() + "}\\cdot"
						+ num2.ToString() + "^{" + power1.ToString() + "} = {(" + num1.ToString() + "\\cdot" +
						num2.ToString() + ")}^? ";
                        Answer = power1.ToString();
                   
                }
                else if (selector == 2)
                {                         //power of power
                    num1 = Random.Range(2, 6);
                    power1 = Random.Range(2, 4);
                    power2 = Random.Range(2, 4);
                    QuestionText.text = "Compute the power : ";
					QuestionLatext.text = "{(" + num1.ToString() + "^" + power1.ToString() + ")}^" + power2.ToString();
                    GeneralButton.gameObject.SetActive(true);
                    Answer = Mathf.Ceil(Mathf.Pow(num1, (power1 * power2))).ToString();
                }
                else if (selector == 3) {                                               
                    int n1 = Random.Range(1, 10);
                    int p1 = Random.Range(0, 4);

                    int n2 = Random.Range(1, 10);
                    int n3 = Random.Range(2, 10);
                    int p2 = Random.Range(2, 5);

                    int n4 = Random.Range(1, 10);
                    int n5 = Random.Range(2, 10);
                    int p3 = Random.Range(1, 4);

                    int n6 = Random.Range(2, 10);
                    int p4 = Random.Range(-4, 0);

                    int n7 = Random.Range(9, 50);
                    QuestionText.text = "Simplify (correct to 2 decimal places):";
					QuestionLatext.text = n1 + "^" + p1 + "\\div {(" + (-n2) + ")}^" + p2 + " \\times {(\\frac{" + n4 + "}{" + n5 + "})}^ " + p3 + " \\div " + n6 + "^{" + p4 + "}";
                    float ans1 = Mathf.Pow(n1, p1) / Mathf.Pow((-(float)n2), p2);
                    CerebroHelper.DebugLog(ans1 + "ans1 ");


                    float ans2 = ans1 * Mathf.Pow(((float)n4 / (float)n5), p3);
                    float ans3 = ans2 / Mathf.Pow(n6, p4);
                    CerebroHelper.DebugLog(ans2 + "ans2 "); CerebroHelper.DebugLog(ans3 + "ans3 ");

                    Answer = ans3.ToString("F2");
                    GeneralButton.gameObject.SetActive(true);

                } else if (selector == 4) {           
                    int n1 = Random.Range(1, 4);
                    int p1 = Random.Range(1, 3);

                    int n2 = Random.Range(1, 5);
                    int p2 = Random.Range(2, 5);

                    int n3 = Random.Range(3, 8);
                    int n4 = Random.Range(2, 8);
                    int pow = Random.Range(-2, 3);

                    int n5 = Random.Range(-5, 5);
                    if (n5 == 0)
                        n5 = 1;
                    int p3 = Random.Range(-2, 3);
                    QuestionText.text = "Simplify (correct to 2 decimal places):";
					QuestionLatext.text = n1 + "^" + p1 + "\\times" + n2 + "^{" + p3 + "} \\div {(\\frac{" + n3 + "}{" + n4 + "})}^{" + pow + "} \\times {(" + n5 + ")}^{" + p3 + "}";
                    Answer = (((Mathf.Pow(n1, p1) * Mathf.Pow(n2, p3)) / Mathf.Pow(((float)n3 / (float)n4), pow)) * Mathf.Pow(n5, p3)).ToString("F2");

                    GeneralButton.gameObject.SetActive(true);
                } else if (selector == 5) {
                    int r1 = 0;
                    int rand = Random.Range(1, 3);
                    if(rand == 1)
                         r1 = Random.Range(32, 101);
                    else  r1 = Random.Range(320, 1000);


                    QuestionText.text = "Use long division method to find the square root of " + r1*r1 + ".";
                    Answer = r1.ToString();

                    GeneralButton.gameObject.SetActive (true);
					
				} 
			}
            #endregion L2
            #region L3
            else if(level == 3)
            {
                selector = GetRandomSelector(1, 6);
                QuestionText.text = "";
                QuestionLatext.text = "";
                if (selector == 1)
                {
                    int r1 = 0;
                    int rand = Random.Range(1, 3);
                    if (rand == 1)
                        r1 = Random.Range(101, 200);
                    else r1 = Random.Range(1001, 2000);

                    QuestionText.text = "Use long division method to find the square root of " + r1 * r1 + ".";
                    Answer = r1.ToString();

                    GeneralButton.gameObject.SetActive(true);
                }
                else if (selector == 2)
                {

                    int r1 = Random.Range(6, 15);
                    int r2 = Random.Range(3, 20);
                    while (r1 % r2 == 0)
                        r2 = Random.Range(3, 20);
                    int r3 = Random.Range(3, 11);
                    //  int r4 = Random.Range(4, 10);
                    QuestionText.text = "Find the cube root of:";
                    QuestionLatext.text = "\\frac{" + (Mathf.Pow(r1, 3) * Mathf.Pow(r3, 2)) + "}{" + (Mathf.Pow(r2, 3) * Mathf.Pow(r3, 2)) + "}";
                    if (r2 < 0)
                        Answer = (-r1).ToString() + "/" + (-r2).ToString();
                    else
                        Answer = (r1).ToString() + "/" + (r2).ToString();

                    GeneralButton.gameObject.SetActive(true);

                }
                else if (selector == 3)
                {

                    int r1 = Random.Range(5, 19);

                    QuestionText.text = "Which of the following is a perfect cube?";


                    MCQ.gameObject.SetActive(true);
                    int tp = 0, x = 0, y = 0;

                    tp = (int)Mathf.Pow(r1, 3);

                    int temp = Random.Range(1, 4);      // this is to break pattern in ans 
                    if (temp == 1)
                    {
                        x = tp + Random.Range(10, 100);
                        y = tp - Random.Range(10, 100);
                    }
                    else if (temp == 2)
                    {
                        x = tp + Random.Range(10, 100);
                        y = tp + Random.Range(10, 100);
                    }
                    else if (temp == 3)
                    {
                        x = tp - Random.Range(10, 100);
                        y = tp - Random.Range(10, 100);
                    }


                    MCQ.gameObject.SetActive(true);
                    numPad.SetActive(false);

                    int rand = Random.Range(1, 4);
                    int rand1 = Random.Range(1, 4);
                    while (rand == rand1)
                        rand1 = Random.Range(1, 4);
                    int rand2 = 6 - (rand + rand1);

                    MCQ.transform.Find("Option" + rand.ToString()).Find("Text").GetComponent<Text>().text = tp.ToString();
                    MCQ.transform.Find("Option" + rand1.ToString()).Find("Text").GetComponent<Text>().text = x.ToString();
                    MCQ.transform.Find("Option" + rand2.ToString()).Find("Text").GetComponent<Text>().text = y.ToString();
                    //GeneralButton.gameObject.SetActive (true);
                    Answer = tp.ToString();

                }
                else if (selector == 4)
                {
                    

                    int r1 = Random.Range(2, 8);
                    int pow1 = Random.Range(2, 6), pow2 = 0;
                    int r2 = Random.Range(8, 20);

                    if (r2 > 10)
                        pow2 = Random.Range(2, 5);
                    else pow2 = Random.Range(2, 8);

                    int r3 = Random.Range(2, 9);
                    int pow3 = Random.Range(2, 9);
                    int countr = 0;
                    while (r2 % r3 != 0 && countr++ <= 10)
                    {
                        r3 = Random.Range(2, 10);
                    }
                    if (r2 % r3 != 0)
                    {
                        r3 = r2;
                        pow3 = Random.Range(2, 5);
                    }
                    int r4 = Random.Range(-6, 6);

                    while (r4 == 0)
                        r4 = Random.Range(-6, 7);
                    int pow4 = Random.Range(1, 5);
                  
                    QuestionText.text = "Simplify (answer in fraction):";
                    if (r4 > 0)
                        QuestionLatext.text = "\\frac{" + r1 + "^" + pow1 + "\\times" + r2 + "^" + pow2 + "}{" + r4 + "^" + pow4 + "\\times " + r3 + "^" + pow3 + "}";
                    else
						QuestionLatext.text = "\\frac{" + r1 + "^" + pow1 + "\\times" + r2 + "^" + pow2 + "}{" + r3 + "^" + pow3 + "\\times {(" + r4 + ")}^" + pow4 + "}";

                    int ans1 = (int)(Mathf.Pow(r1, pow1) * Mathf.Pow(r2, pow2));
                    int ans2 = (int)(Mathf.Pow(r3, pow3) * Mathf.Pow(r4, pow4));
                    int counter = 0;
                    while (MathFunctions.GetHCF(ans1, ans2) != 1 && counter <= 10)
                    {
                        int hcf = MathFunctions.GetHCF(ans1, ans2);
                        ans1 /= hcf;
                        ans2 /= hcf;
                        counter++;
                        CerebroHelper.DebugLog("counter = " + counter + ", hcf = " + hcf + ", ans1 =" + ans1 + ",ans2 =" + ans2);
                    }
                    if (ans2 < 0)
                        Answer = (-ans1).ToString() + "/" + (-ans2).ToString();
                    else
                        Answer = ans1.ToString() + "/" + ans2.ToString();
                    GeneralButton.gameObject.SetActive(true);

                }
                else if (selector == 5)
                {

                    int r1 = Random.Range(6, 30);

                    int r2 = Random.Range(8, 40);

                    QuestionText.text = "Find the cube root using prime factorization:";
                    QuestionLatext.text = "\\frac{" + Mathf.Pow(r1, 3) + "}{" + Mathf.Pow(r2, 3) + "}";


                    if (r2 < 0)
                        Answer = (-r1).ToString() + "/" + (-r2).ToString();
                    else
                        Answer = (r1).ToString() + "/" + r2.ToString();

                    GeneralButton.gameObject.SetActive(true);
                }
            }
            #endregion L3
            #region L4
            else if (level == 4) {
				selector = GetRandomSelector (1, 6);
                QuestionLatext.text = "";
                QuestionText.text = "";

				if (selector == 1) {
                    int r1 = Random.Range(101, 300);
                   
                    QuestionText.text = "Find the smallest number that should be multiplied to " + r1 + " to make it a perfect square.";
                    int ans = 0;
                    for (int i = 1; i <= r1; i++)
                    {
                        float tempf = Mathf.Sqrt(r1 * i);
                        int tempi = (int)tempf;
                        if ((tempf - (float)tempi) == 0)
                        {
                            ans = i;
                            break;
                        }
                    }
                    Answer = ans.ToString();
                    GeneralButton.gameObject.SetActive(true);
                } else if (selector == 2) {

                    int r1 = Random.Range(11, 29);
                    QuestionText.text = "A gardener is planting " + Mathf.Pow(r1, 2) + " plants in such a way that there are as many plants in each row as the number of rows in the garden. Find the number of rows in the garden.";
                     Answer = r1.ToString();

                    GeneralButton.gameObject.SetActive (true);
					
				} else if (selector == 3) {
                    int r1 = Random.Range(2, 9);
                    int r2 = Random.Range(2, 6);
                    int r3 = Random.Range(2, 6);
                    int r4 = Random.Range(6, 11);
                    while(MathFunctions.GetHCF(r2,r3) != 1)
                        r3 = Random.Range(2, 9);
                    int num = r1 * r1  * r2  * r3 * r4 * r4 ;
                    QuestionText.text = "Find the smallest number with which " + num + " should be divided to make it a perfect square.";
                    int ans = r2*r3;
                    
                    Answer = ans.ToString();
                    GeneralButton.gameObject.SetActive(true);
         
                } else if (selector == 4) {

                    int r1 = Random.Range(150, 300);
                    int rand = Random.Range(0, 2);                   
                    float r11 = r1 * r1 , Ans = 0.0f;
                    if (rand == 0)
                    {
                        r11 = (float)r11 / 100;
                        Ans = (float)r1 / 10;
                    }
                    else
                    {
                        r11 = (float)r11 / 10000;
                        Ans = (float)r1 / 100;
                    }
                    QuestionText.text = "Use long division method to find the square root of " + r11 + ".";
                    Answer = Ans.ToString();
                    GeneralButton.gameObject.SetActive (true);
				
				} else if(selector == 5)    // give an error on 0.1/6
                {
                    int r1 = Random.Range(60, 300);
                    float r11 = r1 * r1;
                    r11 = (float)r11 / 1000000;
                    QuestionText.text = "Use long division method to find the square root of " + r11 + ".";
                    Answer = ((float)r1 / 1000).ToString();
                    GeneralButton.gameObject.SetActive(true);
                } 
			}
            #endregion L4
            #region L5
            else if (level == 5) {
				selector = GetRandomSelector (1, 5);
               
				QuestionText.text = "";
                QuestionLatext.text = "";

                if (selector == 1)
                {
                    int r1 = Random.Range(5, 50);
                    int r2 = Random.Range(11, 30);
                    QuestionText.text = "Find the value of x :";
					QuestionLatext.text = "\\root{ \\xalgebra + " + r1 + "} = " + r2;
                    int ans = (int)Mathf.Pow(r2, 2) - r1;
                    Answer = ans.ToString();
                    GeneralButton.gameObject.SetActive(true);

                }else if (selector == 2) {          
                    int r1 = Random.Range(50, 100);
                    int r2 = Random.Range(10, 20);
                    QuestionText.text = "Kinu wants to arrange " + ((r1 * r1) + r2) + " cows in a stable. She wants to have as many cows in each row as the number of rows in the stable. But while arranging she finds that there are " + r2 + " cows still left. How many rows did she manage to make?";
                    Answer = r1.ToString();
                    GeneralButton.gameObject.SetActive(true);
                }
                else if (selector == 3) {          
                    int r1 = Random.Range(11, 50);
                    int Ans = Random.Range(1, 11);
                    while(r1 % Ans != 0)
                    {
                        r1 = Random.Range(1, 50);
                    }
                    int r2 = r1 / Ans;

                    QuestionText.text = "A cube has its edge as " + r1 + " cm. How many small cubes of edge " + r2 + " cm can fit inside it?";
                    Answer = Mathf.Pow( Ans, 3).ToString();
                    GeneralButton.gameObject.SetActive(true);
                } else if (selector == 4) {

                    int r1 = Random.Range(2, 6);
                    int p1 = Random.Range(2, 5);
                    int p2 = Random.Range(p1 +1 , 8);

                    int x = Random.Range(11, 19);
                    int counter = 0;
                    while ((p1 * x) % p2 != 0 && counter++ <= 50)
                    {
                        p1 = Random.Range(2, 5);
                        p2 = Random.Range(p1 + 1, 9);
                    }
                    if((p1 * x) % p2 != 0)
                    {
                        p1 = 6;
                        p2 = 3;
                    }
                    
                    int y = p1 * x / p2;
                    int tot = x + y;
                    QuestionText.text = "Find the value of x - y :";
					QuestionLatext.text = "" + Mathf.Pow(r1, p1) + "^{\\xalgebra} = " + Mathf.Pow(r1, p2) + "^{\\yalgebra} and \\xalgebra + \\yalgebra = " + tot ;
                    Answer = (x - y).ToString();
                    GeneralButton.gameObject.SetActive (true);
				} 
			}
            #endregion L5

            
            
			CerebroHelper.DebugLog (Answer);
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
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
			} else if (value == 12) {   // '/'
				if(checkLastTextFor(new string[1]{"/"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			} else if (value == 13) {   // .
				if(checkLastTextFor(new string[1]{"."})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			} else if (value == 14) {   //-
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}  
		}
	}
}
