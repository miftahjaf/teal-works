using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;

namespace Cerebro {

    public class FactorsAndMultiplesScript5 : BaseAssessment {

        private string Answer;
        //		private string userAnswer;
        public Text subQuestionText;
        public GameObject MCQ;
        private List<int> primes = new List<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293 };

        void Start() {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "FAM05", "S01", "A01");

            scorestreaklvls = new int[3];
            for (var i = 0; i < scorestreaklvls.Length; i++) {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;

            Answer = "";
            GenerateQuestion();
        }

        // Update is called once per frame
        void Update() {

        }

        protected override void GenerateQuestion() {
            ignoreTouches = false;
            base.QuestionStarted();
            // Generating the parameters

            level = Queslevel;

            answerButton = GeneralButton;

            subQuestionText.gameObject.SetActive(false);
            GeneralButton.gameObject.SetActive(false);
            MCQ.gameObject.SetActive(false);
            numPad.SetActive(true);

            if (Queslevel > scorestreaklvls.Length) {
                level = Random.Range(1, scorestreaklvls.Length + 1);
            }
            if (level == 1) {
                selector = GetRandomSelector(1, 5);

                CerebroHelper.DebugLog("SELECTOR = " + selector);
                if (selector == 1) {
					int num = Random.Range(2,4) * Random.Range(5, 20);
                    string ans = MathFunctions.GetFactors(num);
                    CerebroHelper.DebugLog(ans);
                    Answer = ans;
                    QuestionText.text = "Find all the factors of " + num + ".";
                    GeneralButton.gameObject.SetActive(true);

                } else if (selector == 2) {
                    int lowestFactor = Random.Range(1, 16);
                    int multiplier1 = Random.Range(1, 6);
                    int multiplier2 = Random.Range(6, 11);

                    int num = lowestFactor * multiplier1;
                    int num1 = lowestFactor * multiplier2;
                    string ans = "";
                    int min = Mathf.Min(num, num1);
                    for (int i = 1; i <= min; i++)
                    {
                        if (num % i == 0 && num1 % i == 0)
                        {
                            if (ans != "")
                            {
                                ans = ans + ",";
                            }
                            ans = ans + i.ToString();
                        }
                    }
                    Answer = ans;
					QuestionText.text = "Find all the common factors of " + num + " & " + num1 + ".";
                    GeneralButton.gameObject.SetActive(true);


                } else if (selector == 3) {
                    int num1 = Random.Range(6, 20);
					QuestionText.text = "Find the first 5 multiples of " + num1.ToString() + ".";
                    Answer = MathFunctions.GetMultiples(num1, 5);
                    GeneralButton.gameObject.SetActive(true);
                } else if (selector == 4) {
					int multiplier = Random.Range (2, 6);
                    int num = multiplier * Random.Range(2, 6);
                    int num1 = multiplier * Random.Range(2, 10);
                    while(num==num1)
                    {
                        
                        num1 = multiplier * Random.Range(2, 21);
                    }
                    int lcm = MathFunctions.GetLCM(num, num1);
                    int lcm2 = lcm * 2;


                    Answer = lcm.ToString() + "," + lcm2.ToString();
					QuestionText.text = "Find the first two common multiples of " + num + " & " + num1 + ".";
                    GeneralButton.gameObject.SetActive(true);
                }


            } else if (level == 2) {
                selector = GetRandomSelector(1, 5);
                if (selector == 1) {
					int num = Random.Range(2,6) * Random.Range(5, 20);
                    string[] a = new string[100];
                    int j = 0;
                    List<string> ans = new List<string>();

                    string temp = "";
                    for (int i = 2; i <= num; i++)
                    {
                        if (num % i == 0)
                        {
                            a[j] = i.ToString();
                            j++;
                        }
                    }

                    for (int k = 0; k < j; k++)
                    {
                        bool flag = true;
                        int la = int.Parse(a[k]);
                        for (int z = 2; z < la; z++)
                        {
                            if (la % z == 0)
                            {
                                flag = false;
                                break;
                            }

                        }
                        if (flag == true)
                        {
                            ans.Add(la.ToString());

                        }
                    }
                    temp = string.Join(",", ans.ToArray());
					QuestionText.text = "Find all the prime factors of the number " + num + ".";
                    Answer = temp;
                    GeneralButton.gameObject.SetActive(true);




                } else if (selector == 2) {
                    int num1 = Random.Range(2, 21);
                    int num2 = Random.Range(2, 21);
					while (num1 == num2 || 2*num1 == num2 || 3*num2 == num1)
						num2 = Random.Range(2, 21);
					int num3 = 2 * num1;
					int num4 = 3 * num2;
                    int lcm = MathFunctions.GetLCM(num3, num4);
                    int m = Random.Range(1, 6);
                    var number = Random.Range(0, 2);
                    int ans = lcm * m;
					int opt1 = 2* Random.Range(ans - 10, ans + 100)/2;

                    while (opt1 % num1 == 0 && opt1 % num2 == 0 && opt1 % num3 == 0 && opt1 % num4 == 0)
                    {
                        opt1 = Random.Range(1, 10000);
                    }


                    Answer = ans.ToString();
					QuestionText.text = "Select the number divisible by " + num1 + ", " + num2 + ", " + num3 + " and " + num4 + ".";

                    if (number == 0)
                    {
                        MCQ.SetActive(true);
                        numPad.SetActive(false);
                        MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = ans.ToString();
                        MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = opt1.ToString();
                    }
                    else if (number == 1)
                    {
                        MCQ.SetActive(true);
                        numPad.SetActive(false);
                        MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = opt1.ToString();
                        MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = ans.ToString();
                    }

                } else if (selector == 3) {
					int hcf = Random.Range (2, 10);
					int num1 = Random.Range (2, 6)*hcf;
					int num2 = Random.Range (6, 10)*hcf;
					int num3 = Random.Range (10, 16)*hcf;
					hcf = MathFunctions.GetHCF (num1, num2);
					hcf = MathFunctions.GetHCF (hcf, num3);
                    Answer = hcf.ToString();
                    QuestionText.text = "Find the HCF of " + num1 + ", " + num2 + ", " + num3 + ".";
                    GeneralButton.gameObject.SetActive(true);
                } else if (selector == 4) {
					int hcf = Random.Range (2, 10);
					int num1 = Random.Range (2, 10)*hcf;
					int num2 = Random.Range (2, 10)*hcf;
					while (num1 == num2)
						num2 = Random.Range (2, 10)*hcf;
                    hcf = MathFunctions.GetHCF(num1, num2);
                    Answer = hcf.ToString();
                    QuestionText.text = "Find the largest number that divides " + num1 + " and " + num2 + " without leaving a remainder.";
                    GeneralButton.gameObject.SetActive(true);




                }
            }
            else if (level == 3)
            {

				subQuestionText.gameObject.SetActive(false);
                selector = GetRandomSelector(1, 5);

            
                 if (selector == 1)
                {
                    int num1 = Random.Range(2, 201);
                    int num2 = Random.Range(2, 201);
                    while(num1==num2 || MathFunctions.GetHCF(num1,num2)==1)
                    {
                        num2 = Random.Range(2, 201);
                    }
                    int hcf = MathFunctions.GetHCF(num1, num2);
                    int num3 = Random.Range(1, hcf);
                    int num4 = num1 + num3;
                    int num5 = num2 + num3;
                    Answer = hcf.ToString();
					QuestionText.text = "Find the largest number that divides " + num4 + " and " + num5 + " while leaving a remainder of " + num3 + ".";
                    GeneralButton.gameObject.SetActive(true);

                }

                else if (selector == 2)
                {
                    int number = Random.Range(2, 5);
                    int number1 = Random.Range(5, 8);
                    int number2 = Random.Range(8, 11);
                    int num1 = Random.Range(2, 51);
                    int num2 = num1 * number1;
                    int num3 = num1 * number2;
                    num1 = num1 * number;
                    int temp = MathFunctions.GetLCM(num1, num2);
                    int lcm = MathFunctions.GetLCM(temp, num3);
                    Answer = lcm.ToString();
					QuestionText.text = "Find the LCM of " + num1 + ", " + num2 + " and " + num3 + ".";
                    GeneralButton.gameObject.SetActive(true);

                }
                else if (selector == 3)
                {
                    int num1 = Random.Range(1, 101);
                    int number = Random.Range(5, 31);
                    int number1 = Random.Range(2, 11);
                    int num2 = num1 * number;
                    num1 = num1 * number1;
                    int lcm = MathFunctions.GetLCM(num1, num2);
                    int hcf = MathFunctions.GetHCF(num1, num2);
                    Answer = hcf.ToString();
                    QuestionText.text = "LCM of " + num1 + " and " + num2 + " is " + lcm + ". Find their HCF.";
                    GeneralButton.gameObject.SetActive(true);



                }
                else if (selector == 4)
                {
                    int num1 = Random.Range(1, 101);
                    int number = Random.Range(5, 21);
                    int number1 = Random.Range(2,5);
                    int num2 = num1 * number;
                    num1 = num1 * number1;
                    int lcm = MathFunctions.GetLCM(num1, num2);
                    int hcf = MathFunctions.GetHCF(num1, num2);
                    int product = num1 * num2;
                    Answer = hcf.ToString();
                    QuestionText.text = "The LCM of two numbers is " + lcm + ". If their product is " + product + ". What is the HCF?";
                    GeneralButton.gameObject.SetActive(true);
                    subQuestionText.gameObject.SetActive(true);

                }
            }



                if (answerButton != null) {
                    userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
                    userAnswerText.text = "";
                }

                CerebroHelper.DebugLog(Answer);
            }
        

		public override void SubmitClick() {
			if (ignoreTouches) {
				return;
			}
            if (numPad.activeSelf && userAnswerText.text == "")
            {
                return;
            }
            int increment = 0;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
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
				if (level == 6 && selector == 3) {
					var userAnswers = userAnswerText.text.Split(new string[] { "," }, System.StringSplitOptions.None);
					var correctAnswers = Answer.Split(new string[] { "," }, System.StringSplitOptions.None);
					for (var i = 0; i < userAnswers.Length; i++) {
						if (int.Parse (userAnswers [i]) % int.Parse (correctAnswers[0]) != 0) {
							correct = false;
						}
					}
				} else if (Answer.Contains (",")) {
					var correctAnswers = Answer.Split(new string[] { "," }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split(new string[] { "," }, System.StringSplitOptions.None);
					correct = checkArrays (correctAnswers, userAnswers);
				} else if (Answer.Contains ("x")) {
					var correctAnswers = Answer.Split(new string[] { "x" }, System.StringSplitOptions.None);
					var userAnswers = userAnswerText.text.Split(new string[] { "x" }, System.StringSplitOptions.None);
					correct = checkArrays (correctAnswers, userAnswers);
				} else {
					float correctAns = float.Parse (Answer);
					float userAns = float.MinValue;
					if (float.TryParse (userAnswerText.text, out userAns)) {
						userAns = float.Parse (userAnswerText.text);
					}
					if (userAns == correctAns) {
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
					increment = 5;
				} else if (Queslevel == 3) {
					increment = 10;
				} else if (Queslevel == 4) {
					increment = 15;
				} else if (Queslevel == 5) {
					increment = 15;
				} else if (Queslevel == 6) {
					increment = 15;
				} else if (Queslevel == 7) {
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
					float numA = float.MinValue;
					float numB = float.MinValue;
					if (float.TryParse (numberA, out numA)) {
						numA = float.Parse (numberA);
					}
					if (float.TryParse (numberB, out numB)) {
						numB = float.Parse (numberB);
					}
					if (numA == numB)
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
            ShowContinueButton();
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
			} else if (value == 12) {   // ,
				if(checkLastTextFor(new string[1]{","})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ",";
			} else if (value == 13) {   // -
				if(checkLastTextFor(new string[1]{"x"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "x";
			}
		}
	}
}
