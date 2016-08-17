using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MaterialUI;

namespace Cerebro {

    public class OperationsAndApplications5 : BaseAssessment {

        private string Answer;
        //		private string userAnswer;
        public TEXDraw subQuestionText;

        void Start() {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "OTA05", "S01", "A01");

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
            numPad.SetActive(true);

            if (Queslevel > scorestreaklvls.Length) {
                level = Random.Range(1, scorestreaklvls.Length + 1);
            }
            if (level == 1) {
                selector = GetRandomSelector(1, 5);

                CerebroHelper.DebugLog("SELECTOR = " + selector);
                if (selector == 1) {
                    int num1 = Random.Range(10000,1000000);
                    int num2= Random.Range(10000, 1000000);
                    int ans = num1 + num2;
                    Answer = ans.ToString();
                    QuestionText.text = "Add " + num1 + " and " + num2 + ".";
                    GeneralButton.gameObject.SetActive(true);

                  } else if (selector == 2) {

                    int num1 = Random.Range(10000, 1000000);
                    int num2 = Random.Range(10000, 1000000);
                    int ans = num1 - num2;
                    subQuestionText.gameObject.SetActive(true);
                    Answer = ans.ToString();
                    QuestionText.text = "Subtract:";
                    subQuestionText.text = num1 + " - " + num2;
                    GeneralButton.gameObject.SetActive(true);
                     

                }
                else if (selector == 3) {
                    int num1 = Random.Range(100, 100000);
                    int num2 = Random.Range(9, 100);
                    int num3 = Random.Range(100, 1000);
                    int number = Random.Range(0, 2);
                    int ans;
                    subQuestionText.gameObject.SetActive(true);
                    if(number==0)
                    {
                        ans = num1 * num2;
                        Answer = ans.ToString();
                        QuestionText.text = "Multiply:";
                        subQuestionText.text = num1 + " \\times " + num2;
                        GeneralButton.gameObject.SetActive(true);

                    }
                    else
                    {
                        ans = num1 * num3;
                        Answer = ans.ToString();
                        QuestionText.text = "Multiply:";
                        subQuestionText.text = num1 + " \\times " + num3;
                        GeneralButton.gameObject.SetActive(true);
                    }
                } else if (selector == 4) {
                    int num1 = Random.Range(1000, 100000);
                    int num2 = Random.Range(9, 100);
                    int num3 = Random.Range(100, 1000);
                    int number = Random.Range(0, 2);
                    int ans;
                    subQuestionText.gameObject.SetActive(true);
                    if (number == 0)
                    {
                        ans = num1 / num2; 
                        Answer = ans.ToString();
                        QuestionText.text = "Find the quotient:";
                        subQuestionText.text = num1 + " / " + num2;
                        GeneralButton.gameObject.SetActive(true);

                    }
                    else
                    {
                        ans = num1 / num3;
                        Answer = ans.ToString();
                        QuestionText.text = "Find the quotient:";
                        subQuestionText.text = num1 + " / " + num3;
                        GeneralButton.gameObject.SetActive(true);
                    }
                }


            } else if (level == 2) {
                selector = GetRandomSelector(1, 6);
                if (selector == 1) {
                    int num1 = Random.Range(10000, 100000);
                    int num2 = Random.Range(1000, 10000);
                    int ans = num1 - num2;
                    Answer = ans.ToString();
                    QuestionText.text = "A library has " + num1 + " books. They lent a total of " + num2 + " books throughout the day. How many books are left at the library?";
                    GeneralButton.gameObject.SetActive(true);

                } else if (selector == 2) {

                    int num1 = Random.Range(1000, 10001);
                    int num2 = Random.Range(100, 1001);
                    int ans = num1 + num2;
                    Answer = ans.ToString();
                    QuestionText.text = "A box already has " + num1 + " pencils. They need " + num2 + " more pencils in the box to complete the ordered package. What is the total number of pencils ordered ?";
                    GeneralButton.gameObject.SetActive(true);

                } else if (selector == 3) {
                    int num1 = Random.Range(10, 101);
                    int num2 = Random.Range(50, 501);
                    Answer = (num1 * num2).ToString();
                    QuestionText.text = "A tree has " + num1 + " branches. If each branch has " + num2 + " fruits on it, find the total number of fruits on the tree.";
                    GeneralButton.gameObject.SetActive(true);
                } else if (selector == 4) {
                    int num1 = Random.Range(50, 151)*100;
                    int num2 = Random.Range(1, 6)*10;
                    while(num1%num2!=0)
                    {
                         num1 = Random.Range(50, 151) * 100;
                         num2 = Random.Range(1, 6) * 10;
                    }
                    int ans = num1 / num2;
                    Answer = ans.ToString();
                    QuestionText.text = "A school has " + num1 + " students. Each classroom can hold " + num2 + " students. How many classrooms are there in school?";
                    GeneralButton.gameObject.SetActive(true);

                }
                else if (selector==5)
                {
                    int number = Random.Range(0, 2); 
                    int num1 = Random.Range(1, 1000);
                    if (number == 0)
                    {
                        num1 = 0;
                    }

                    int num2 = Random.Range(1, 1000);
                    int num3 = Random.Range(1, 1000);
                    int num4 = Random.Range(1, 1000);
                    int num5 = Random.Range(1, 1000);
                    int sum = num1 + num2 + num3 + num4 + num5;
                    while(sum%5!=0)
                    {
                        num5 = num5+1;
                        sum = num1 + num2 + num3 + num4 + num5;
                    }
                    int avg;
                    

                    avg = (num1 + num2 + num3 + num4 + num5) / 5;
                    Answer = avg.ToString();
                    QuestionText.text = "Find the average of " + num1 + ", " + num2 + ", " + num3 + ", " + num4 + ", " + num5 + ".";
                    GeneralButton.gameObject.SetActive(true);

                    
                 
                }
            }
            else if (level == 3)
            {


                selector = GetRandomSelector(1, 5);



                if (selector == 1)
                {
                    int marks = Random.Range(10, 101);
                    int num = Random.Range(2, 6);
                    int ans = marks * num;
                    Answer = ans.ToString();
                    QuestionText.text = "Sharan scored an average of " + marks + " in " + num + " tries. What was his total score?";
                    GeneralButton.gameObject.SetActive(true);


                }
                else if (selector == 2)
                {
                    int num1 = Random.Range(10, 101);
                    int num2 = Random.Range(10, 101);
                    int num3 = Random.Range(10, 101);
                    int num4 = Random.Range(10, 101);
                    int num5 = Random.Range(10, 101);
                    int num6 = Random.Range(10, 101);
                    int num7 = Random.Range(10, 101);
                    int number = Random.Range(5, 8);
                    int ans;
                    int sum;
                    if(number==5)
                    {
                        sum = num1 + num2 + num3 + num4 + num5;
                        while(sum%5!=0)
                        {
                            num5 = num5 + 1;
                            sum = num1 + num2 + num3 + num4 + num5;
                        }
                        ans = sum / 5;
                        Answer = ans.ToString();
                        QuestionText.text = "A vendor sells " + num1 + ", " + num2 + ", " + num3 + ", " + num4 + ", " + num5 + " balloons during five days. What is the average number balloons sold per day?";
                        GeneralButton.gameObject.SetActive(true); 

                    }
                    else if (number==6)
                    {
                        sum = num1 + num2 + num3 + num4 + num5 + num6;
                        while(sum%6!=0)
                        {
                            num6 = num6 + 1;
                            sum = num1 + num2 + num3 + num4 + num5 + num6;
                        }
                        ans = sum / 6;
                        Answer = ans.ToString();
						QuestionText.text = "A vendor sells " + num1 + ", " + num2 + ", " + num3 + ", " + num4 + ", " + num5 +", "+ num6+ " balloons during six days. What is the average number balloons sold per day?";
                        GeneralButton.gameObject.SetActive(true);

                    }
                    else if(number==7)
                    {
                        sum = num1 + num2 + num3 + num4 + num5 + num6+num7;
                        while (sum % 7 != 0)
                        {
                            num6 = num6 + 1;
                            sum = num1 + num2 + num3 + num4 + num5 + num6 + num7;
                        }
                        ans = sum / 7;
                        Answer = ans.ToString();

						QuestionText.text = "A vendor sells " + num1 + ", " + num2 + ", " + num3 + ", " + num4 + ", " + num5 +", "+ num6+", "+ num7+  " balloons during the week. What is the average number balloons sold per day?";
                        GeneralButton.gameObject.SetActive(true);

                    }
                }
                else if (selector == 3)
                {
                    int num1 = Random.Range(50, 501);
                    int num2 = Random.Range(2,4);
                    int num3 = Random.Range(501, 801);
                    int num4 = Random.Range(4, 9);
                    int number = Random.Range(0, 2); 
                    int speed;
                    if(number==0)
                    {
                        while(num1%num2!=0)
                        {
                            num1 = num1 + 1;
                        }
                        speed = num1 / num2;
                        Answer = speed.ToString();
						QuestionText.text = "Anup drove " + num1 + " km in " + num2 + " hour. What was his average speed(in kms/hr)?";
                        GeneralButton.gameObject.SetActive(true); 

                    }
                    else
                    {

                        while(num3%num4!=0)
                        {
                            num3 = num3 + 1;
                        }
                        speed = num3 / num4;
                        Answer = speed.ToString();
						QuestionText.text = "Anup drove " + num3 + " km in " + num4 + " hour. What was his average speed(in kms/hr)?";
                        GeneralButton.gameObject.SetActive(true);
                    }
                    

                }
                else if (selector==4)
                {
                    int num1 = Random.Range(10, 51);
                    Answer = (num1 * 30).ToString();
                    subQuestionText.gameObject.SetActive(true);
                    QuestionText.text = "A doctor takes about " + num1 + " patients a day. Assuming he worked on the weekends too, how many patients did he see in the month?";
                    subQuestionText.text = "Number of days in the month = 30";
                    GeneralButton.gameObject.SetActive(true);
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
            if(numPad.activeSelf && userAnswerText.text=="")
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
					if (userAnswerText.text == Answer) {
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
				if(checkLastTextFor(new string[1]{"."})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			} else if (value == 13) {   // -
				if(checkLastTextFor(new string[1]{"-"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += "-";
			}
		}
	}
}
