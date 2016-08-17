using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;
//using System;

namespace Cerebro {
	
	public class FactorsAndMultiplesScript7 : BaseAssessment {

		private string Answer;
//		private string userAnswer;
		public TEXDraw subQuestion;
		public GameObject MCQ;
		private List<int> primes = new List<int>{2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293};

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "FAM07", "S01", "A01");

			scorestreaklvls = new int[4];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		protected override void GenerateQuestion () {

            ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters

			level = Queslevel;
            
			answerButton = GeneralButton;

			subQuestion.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (false);
			MCQ.gameObject.SetActive (false);
			numPad.SetActive (true);

            if (Queslevel > scorestreaklvls.Length)
            {
                level = Random.Range(1, scorestreaklvls.Length + 1);
            }
           
            #region L1
            if (level == 1) {
				selector = GetRandomSelector (1, 5);
               
                subQuestion.text = "";
                QuestionText.text = "";
                if (selector == 1) {

                    int num1 = Random.Range(2, 99);                
                    string tempAns = MathFunctions.GetFactors(num1);
                    int counter = 1;
                    foreach(char i in tempAns)
                    {
                        if (i == ',')
                            counter++;
                    }
                   
                   
                    Answer = counter.ToString();
                   
                    QuestionText.text = "Total number of factors of " + num1 +" are :";
                    GeneralButton.gameObject.SetActive(true);

                } else if (selector == 2) {
                    int num1 = Random.Range(7, 100);
                    int tempAns = 0;
					foreach(int i in primes)
                    {
                        if (i > num1)                      
                            break;
                        tempAns++;
                    }
                 
                    QuestionText.text = "Total number of Prime numbers upto and including " + num1 + " are :";
					Answer = tempAns.ToString ();
					GeneralButton.gameObject.SetActive (true);

				} else if (selector == 3) {
					
					int num1 = Random.Range(3, 10);
                    int num2 = Random.Range (10, 25);
					int num3 = Random.Range(26, 50);
                    
                    int rand = Random.Range(1, 5);
                    if(rand != 1)
                    {
                        while(MathFunctions.GetHCF(MathFunctions.GetHCF(num1,num2),num3) == 1)
                        {
                            num1 = Random.Range(3, 15);
                            num2 = Random.Range(2, 25);
                            num3 = Random.Range(2, 50);
                        }
                    }
					
					//if (Random.Range (1, 3) == 1) {
					//	Answer = "No";
					//	num3 = num3 + Random.Range (1, num1);
					//}

                    int tempAns = MathFunctions.GetHCF(num1, num2);
                     tempAns = MathFunctions.GetHCF(tempAns, num3);
                    //subQuestion.gameObject.SetActive (true);
                 
                    QuestionText.text = "The HCF of " + num1 + ", " + num2 + " and " + num3 + " is :";
                    Answer = tempAns.ToString();
                    GeneralButton.gameObject.SetActive(true);

                } else if (selector == 4) {
                    int num1 = Random.Range(2, 8);
                    int num2 = Random.Range(2, 8);
                    int pow = Random.Range(2, 4);
                    
                    while(num2 % num1 == 0 || (MathFunctions.GetHCF(num1, num2) != 1))
                        num1 = Random.Range(2, 10);

                    int rand = Random.Range(3, 6);
                    int rand2 = Random.Range(6, 9);
                    num1 = (int)Mathf.Pow(num1, pow );
                    num2 = (int)Mathf.Pow(num2, pow) ;
                    int num11 = num1 * rand * rand2;
                    int num22 = num2 * rand * rand2;
                    subQuestion.gameObject.SetActive (true);
                    QuestionText.text = "Reduce to lowest terms using prime factorization";

                    subQuestion.text = "\\frac{" + num11 + "}{" + num22 + "}";
                    Answer = num1.ToString() + "/" + num2.ToString();
                    GeneralButton.gameObject.SetActive(true);
                }
            }
            #endregion L1
            #region L2
            else if (level == 2) {
				selector = GetRandomSelector (1, 5);
                subQuestion.text = "";
                QuestionText.text = "";

                if (selector == 1) {
					int num1 = Random.Range (2, 101);
                    int num2 = Random.Range(3, 101);
                    int num3 = Random.Range(5, 101);
                 
                    QuestionText.text = "Find the shortest length which can be measured exactly by 3 measuring rods of length " + num1 + " cm, " + num2 + " cm and " + num3 + " cm.";
                    int tempAns = MathFunctions.GetLCM(num1, num2, num3);
                    Answer = tempAns.ToString();
                    GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
					int num1 = Random.Range (2, 10);
                    int num2 = Random.Range(10, 20);
                    int num3 = Random.Range(20, 30);
                    QuestionText.text = "Sam, Suzi and Ben are hiking through the mountains. Sam stops to rest every " + num1 + " kms, Suzi stops every " +  num2 + " kms and Ben stops every " + num3 + " kms. At what distance will they all stop together for the first time?";
                    int tempAns = MathFunctions.GetLCM(num1, num2, num3);                   
					Answer = tempAns.ToString();
					GeneralButton.gameObject.SetActive (true);
				} else if (selector == 3) {
					int num1 = Random.Range (3, 30);
                    int num2 = Random.Range(2, 25);
                    while(num1 == num2)
                        num2 = Random.Range(2, 25); 
                    QuestionText.text = "Yaya has a football game every " + num1 + " days. Kolo has one every " + num2 + " days. When will they have a game on the same day?";
                    Answer = MathFunctions.GetLCM(num1, num2).ToString();
                    GeneralButton.gameObject.SetActive(true);

                } else if (selector == 4)
                {
                    int num1 = Random.Range(3, 20);
                    int num2 = Random.Range(num1 + 1, 30);
                    int num3 = Random.Range(1, num1);

                    int lcm = MathFunctions.GetLCM(num1, num2);
                    Answer = (lcm + num3).ToString();
                 
                    QuestionText.text = "Find the least possible number that is divisible by both " + num1.ToString() + " and " + num2.ToString() + " while leaving a remainder of " + num3.ToString() + ".";
                    GeneralButton.gameObject.SetActive(true);
                }
               
            }
            #endregion L2
            #region L3
            if (level == 3)
            {
                selector = GetRandomSelector(1, 5);
                subQuestion.text = "";
                QuestionText.text = "";
                if (selector == 1)
                {
                    int multiple = Random.Range(2, 10);
                    int num1 = Random.Range(3, 50);
                    int num2 = Random.Range(3, 50);
                    while (num2 == num1)
                    {
                        num2 = Random.Range(1, 10);
                    }
                    num1 = num1 * multiple;
                    num2 = num2 * multiple;

                    int hcf = MathFunctions.GetHCF(num1, num2);
                    Answer = hcf.ToString();

                    int num3 = Random.Range(1, hcf);
                    int num4 = Random.Range(1, hcf);
                    int num5 = num1 + num3;
                    int num6 = num2 + num4;

                    QuestionText.text = "Find the greatest number that will divide " + num5.ToString() + " and " + num6.ToString() + " while leaving remainder " + num3.ToString() + " and " + num4.ToString() + " respectively.";
                    GeneralButton.gameObject.SetActive(true);
                }
                else if (selector == 2)
                {
                    int num1 = Random.Range(3, 15);
                    int num2 = Random.Range(3, 20);
                    int num3 = Random.Range(3, 25);
                    while (num1 == num2)
                        num2 = Random.Range(3, 20);
                    while (num2 == num3 || num1 == num3)
                        num3 = Random.Range(3, 25);

                    int min = 9999;
                    if (num1 <= num2)
                        min = num1;
                    else min = num2;
                    if (min >= num3)
                        min = num3;
                    if (min >= 10)
                        min = 10;
                    int num4 = Random.Range(1, min);

                    QuestionText.text = "Find the smallest number which is divisible by " + num1.ToString() + ", " + num2.ToString() + " and " + num3.ToString() + " while leaving a remainder of " + num4.ToString() + ".";
                    int tempAns = MathFunctions.GetLCM(num1, num2, num3);
                    tempAns += num4;
                    Answer = tempAns.ToString();
                    GeneralButton.gameObject.SetActive(true);

                }
                else if (selector == 3)
                {
                    int num1 = Random.Range(3, 50);
                    int num2 = Random.Range(3, 50);
                    int num3 = Random.Range(3, 50);
                    while (num1 == num2)
                        num2 = Random.Range(3, 50);
                    while (num2 == num3 || num1 == num3)
                        num3 = Random.Range(3, 50);


                    int min = 9999;
                    if (num1 <= num2)
                        min = num1;
                    else min = num2;
                    if (min >= num3)
                        min = num3;
                    if (min >= 10)
                        min = 10;
                    int num4 = Random.Range(1, min);

                    QuestionText.text = "Find the smallest number which when increased by " + num4.ToString() + " is exactly divisible by " + num2.ToString() + ", " + num3.ToString() + " and " + num1.ToString() + ".";
                    int tempAns = MathFunctions.GetLCM(num1, num2, num3);
                    tempAns -= num4;
                    Answer = tempAns.ToString();
                    GeneralButton.gameObject.SetActive(true);

                }
                else if (selector == 4)
                {
                    int num1 = Random.Range(2, 10);
                    int num2 = Random.Range(2, 10);
                    while (MathFunctions.GetHCF(num1, num2) != 1)
                        num2 = Random.Range(2, 20);

                    int num3 = Random.Range(2, 50);


                    Answer = MathFunctions.GetLCM(num1 * num3, num2 * num3).ToString();
                    QuestionText.text = "The ratio of two numbers is " + num1 + ":" + num2 + " and their HCF is " + num3 + ". Their LCM is ";
                    GeneralButton.gameObject.SetActive(true);
                }
            }
            #endregion L3
            #region L4
            else if (level == 4) {
				selector = GetRandomSelector (1, 5);
                subQuestion.text = "";
                QuestionText.text = "";

                if (selector == 1) {       

                    int num1 = Random.Range(2, 50);
                    int num2 = Random.Range(2, 50);
                    int num3 = Random.Range(2, 50);
                 
                    QuestionText.text = "Find the greatest number of 4 digits which is exactly divisible by " + num1 + ", " + num2 +" and "+ num3 + ".";
                    int temp = MathFunctions.GetLCM(num1, num2, num3);
                  while(temp >= 9999)
                    {
                        num2 = Random.Range(2, 50);
                        num3 = Random.Range(2, 50);
                        temp = MathFunctions.GetLCM(num1, num2, num3);
                    }


                    int remainer = 9999 % temp;
                    temp = 9999 - remainer;
                   
                    Answer = temp.ToString();
                    GeneralButton.gameObject.SetActive (true);
				} else if (selector == 2) {
                    int num1 = Random.Range(2, 16) ;
                    int num2 = Random.Range(3, 16);
                    int mul = Random.Range(2, 6);
                    num1 *= mul; 
                    num2 *= mul;
                    int ans = MathFunctions.GetHCF(num1,num2);
                 
                    Answer = ans.ToString();
                    QuestionText.text = "A room has the dimension " + num1 + "\" X " + num2 + "\". Find the side length (in inches) of largest square tile which will cover the floor exactly.";  
					GeneralButton.gameObject.SetActive (true);
				}
                else if(selector == 3)
                {
                    int num1 = Random.Range(1, 10) ;
                    int num2 = Random.Range(1, 10) ;
                    int num3 = Random.Range(1, 10) ;
                    while(num1 == num2 || num2 == num3 || num1 == num3)
                    {
                         num1 = Random.Range(1, 10);
                         num2 = Random.Range(1, 10);
                         num3 = Random.Range(1, 10);
                    }
                    int x = Random.Range(1, 101) ;
                    int lcm = MathFunctions.GetLCM(num1, num2, num3) * x;
                    Answer = x.ToString();
                 
                    QuestionText.text = "Three numbers are in ratio " + num1 + ":" + num2 + ":" + num3 + " and their LCM is " + lcm + ". Find their HCF.";
                    GeneralButton.gameObject.SetActive(true);

                }
                else if(selector == 4)
                {
                    int x = Random.Range(2,51);
                    int y = Random.Range(2, 51);
                    int sum = x + y;
                    int lcm = MathFunctions.GetLCM(x, y);
                    int hcf = MathFunctions.GetHCF(x, y);
                 
                    QuestionText.text = "The sum of two numbers is " + sum + ". Their HCF and LCM are " + hcf + ", " + lcm + " respectively. Find the sum of the reciprocals of the two numbers.";
                    Answer = (x + y).ToString() + "/" + (x * y).ToString();
                    GeneralButton.gameObject.SetActive(true);
                }
             

            }
            #endregion L4
          

			

			if (answerButton != null) {
				userAnswerText = answerButton.gameObject.GetChildByName<Text> ("Text");
				userAnswerText.text = "";
			}

			CerebroHelper.DebugLog (Answer);
		}

       

        public override void SubmitClick() {
			if (ignoreTouches) {
				return;
			}
			int increment = 0;
			ignoreTouches = true;
			///Checking if the response was correct and computing question level
            
           

			var correct = true;
			questionsAttempted++;
			updateQuestionsAttempted ();
			if (!numPad.activeSelf) {
				if (userAnswerText.text == Answer) {
					correct = true;
				} else {
					correct = false;
				}
			} else {

                if (userAnswerText.text.EndsWith("-") || userAnswerText.text.EndsWith("/") || userAnswerText.text.StartsWith("/") || userAnswerText.text == "")
                {
                    correct = false;
                }
                else if (Answer.Contains("/") && userAnswerText.text.Contains("/")) {

                    var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                    var correctAnswers = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);

                    if(userAnswers[0].EndsWith("-") || userAnswers[0].EndsWith("/") || userAnswers[1].EndsWith("-") || userAnswers[1].EndsWith("/"))
                    {
                        correct = false;
                    }
                    else if ((float.Parse(userAnswers[0]) / (float.Parse(userAnswers[1])) != (float.Parse(correctAnswers[0]) / float.Parse(correctAnswers[1])))) {
                        correct = false;
                    }
                }
                else if (Answer.Contains("/") && (!userAnswerText.text.Contains("/"))){

                    var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                    var correctAnswers = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
                    if (userAnswers[0].EndsWith("-"))
                    {
                        correct = false;
                    }
                    else if ((float.Parse(userAnswers[0])) != (float.Parse(correctAnswers[0]) / float.Parse(correctAnswers[1])))
                    {
                        correct = false;
                    }
                }
                else if ((userAnswerText.text.Contains("/")))
                {

                   
                        var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                        var correctAnswers = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
                        if (userAnswers[0].EndsWith("-") || userAnswers[0].EndsWith("/") || userAnswers[1].EndsWith("-") || userAnswers[1].EndsWith("/"))
                        {
                            correct = false;
                        }
                        else if (((float.Parse(userAnswers[0]))/ float.Parse(userAnswers[1])) != (float.Parse(correctAnswers[0])))
                        {
                            correct = false;
                        }
                    
                }
                else if(userAnswerText.text.Contains("-") && (!userAnswerText.text.StartsWith("-")))
                {
                    correct = false;
                }
                else
                {
                                     
					if (float.Parse(userAnswerText.text) == float.Parse(Answer)) {
						correct = true;
					} else {
						correct = false;
					}
				}
			}

			if (correct == true) {
				if (Queslevel == 1) {
					increment = 5;
				} else if (Queslevel == 2) {
					increment = 8;
				} else if (Queslevel == 3) {
					increment = 12;
				}
                else if (Queslevel == 4)
                {
                    increment = 15;
                }
                else if (Queslevel == 5)
                {
                    increment = 15;
                }



                UpdateStreak(8,12);

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

		bool checkArrays(string[] a, string[]b) {
			
			foreach (var numberA in a)
			{
				bool hasDuplicate = false;
				foreach (var numberB in b)
				{
					if (numberA == numberB)
					{
						hasDuplicate = true;
					}
				}
				if (!hasDuplicate) {
					return false;
				}
			}
			return true;
		}

		public void MCQOptionClicked(int value) {
			if (ignoreTouches) {
				return;
			}
			AnimateMCQOption (value);
			userAnswerText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<Text> ();
			answerButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
			SubmitClick ();
		}

		IEnumerator AnimateMCQOption(int value) {
			var GO = MCQ.transform.Find ("Option" + value.ToString ()).gameObject;
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
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

		protected override IEnumerator ShowWrongAnimation() {
			userAnswerText.color = MaterialColor.red800;
			Go.to( userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake( new Vector3( 0, 0, 20 ), GoShakeType.Eulers ) );
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				if (numPad.activeSelf) {				// is not MCQ type question
					userAnswerText.text = "";
				}
				userAnswerText.color = MaterialColor.textDark;
				ignoreTouches = false;
			} else {
				if (numPad.activeSelf) {				// is not MCQ type question
					userAnswerText.text = Answer.ToString ();
					userAnswerText.color = MaterialColor.green800;
				} else {
					userAnswerText.color = MaterialColor.textDark;
				}
			}
			ShowContinueButton ();
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
			} else if (value == 12) {   // /
				if(checkLastTextFor(new string[1]{"/"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "/";
			} else if (value == 13) {   // -
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
		}
	}
}
