using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
    public class Averages7 : BaseAssessment
    {

        public Text subQuestionText;
        private string Answer;
        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
        private int coeff6;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "AVG07", "S01", "A01");

            scorestreaklvls = new int[3];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;

            coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = coeff6 = 0;
            
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
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            var correct = true;
            CerebroHelper.DebugLog("!" + userAnswerText.text + "!");
            CerebroHelper.DebugLog("*" + Answer + "*");
            questionsAttempted++;
            updateQuestionsAttempted();
            float answer = 0;
            float userAnswer = 0;
            bool directCheck = false;
            if (float.TryParse(Answer, out answer))
            {
                answer = float.Parse(Answer);
            }
            else
            {
                directCheck = true;
            }
            if (float.TryParse(userAnswerText.text, out userAnswer))
            {
                userAnswer = float.Parse(userAnswerText.text);
            }
            else
            {
                correct = false;
            }
            if (answer != userAnswer)
            {
                correct = false;
            }
            if (directCheck)
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


                UpdateStreak(5, 12);

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

        protected override IEnumerator ShowWrongAnimation()
        {
            userAnswerText.color = MaterialColor.red800;
			Go.to(userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
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
                if (userAnswerText != null)
                {
                    userAnswerText.color = MaterialColor.textDark;
                }
                ignoreTouches = false;
            }
            else
            {
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

            ShowContinueButton();
        }

        protected override IEnumerator ShowCorrectAnimation()
        {
            userAnswerText.color = MaterialColor.green800;
            var config = new GoTweenConfig()
                .scale(new Vector3(1.1f, 1.1f, 0))
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
            subQuestionText.gameObject.SetActive(false);
            QuestionText.gameObject.SetActive(true);
            GeneralButton.gameObject.SetActive(true);
            numPad.SetActive(true);
            if (Queslevel > scorestreaklvls.Length)
            {
                level = UnityEngine.Random.Range(1, scorestreaklvls.Length + 1);
            }

            level = 3;

            #region level1
            if (level == 1)
            {
                selector = GetRandomSelector(1, 6);
                if (selector == 1)
                {
                    subQuestionText.gameObject.SetActive(true);
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(1, 100);
                    QuestionText.text = "Find the average of (Correct to one decimal place): ";
                    subQuestionText.text = coeff1 + "," + coeff2;
                    float ans = (float)(coeff1 + coeff2) / (float)2;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 2)
                {
                    subQuestionText.gameObject.SetActive(true);
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(1, 100);
                    coeff4 = Random.Range(1, 10);
                    QuestionText.text = "Find the average of the following numbers upto one decimal place:";
                    subQuestionText.text = coeff1 + "," + coeff2 +"," + coeff4;
                    float ans = (coeff1 + coeff2 + coeff4) / (float)3;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(40, 100);
                    coeff2 = Random.Range(4, 10);
                    QuestionText.text = "Brian scored a total of " + coeff1 + " marks in " + coeff2 + " subjects. Find his average score. (Correct to one decimal place)";
                    float ans = coeff1 / coeff2;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(30, 100);
                    coeff3 = Random.Range(50, 100);
                    coeff4 = Random.Range(70, 100);
                    coeff5 = Random.Range(1, 100);
                    QuestionText.text = "Hodor scores " + coeff1 + ", " + coeff2 + ", " + coeff3 + ", " + coeff4 + ", " + coeff5 + " runs in 5 IPL match innings. Find his average score to nearest unit.";
                    float ans = (float)(coeff1 + coeff2 + coeff3 + coeff4 + coeff5) / (float)5;
                    int intpart = (int)ans;
                    float decpart = ans - intpart;
                    if (decpart >= 0.5)
                        intpart += 1;
                    Answer = intpart.ToString();
                }
                else if (selector == 5)
                {
                    coeff1 = Random.Range(60, 80);
                    coeff2 = Random.Range(50, 70);
                    while (coeff2 + 3 > coeff1)
                         coeff2 = Random.Range(50, 70);
                    coeff3 = Random.Range(4, 10);
                    QuestionText.text = "The average weight of " + coeff3 + " boys is " + coeff1 + " kgs. If the average weight of first " + (coeff3 - 1) + " boys is " + coeff2 + " kgs, find the weight of the " + coeff3 + "th boy. (Correct to one decimal place)";
                    float ans = (float)(coeff1 * coeff3 - coeff2 * (coeff3 - 1));
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
            }
            #endregion
            #region level2

            if (level == 2)
            {
                selector = GetRandomSelector(1, 9);
                
                if(selector==1)
                {
                    subQuestionText.gameObject.SetActive(true);
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(1, 100);
                    coeff3 = Random.Range(1, 100);
                    coeff4 = Random.Range(1, 100);
                    coeff5 = Random.Range(1, 100);
                    QuestionText.text = "Find the average of (Correct to one decimal place): ";
                    subQuestionText.text = coeff1 + "," + coeff2 + "," + coeff3 + "," + coeff4 + "," + coeff5;
                    float ans = (float)(coeff1 + coeff2 + coeff3 + coeff4 + coeff5) / (float)5;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if(selector==2)
                {
                    subQuestionText.gameObject.SetActive(true);
                    coeff1 = Random.Range(1, 100);
                    coeff2 = Random.Range(1, 100);
                    coeff3 = Random.Range(1, 100);
                    coeff4 = Random.Range(1, 10);
                    float temp = (float)(coeff4 * 10) / (float)10;
                    coeff5 = Random.Range(1, 10);
                    float temp2 = (float)(coeff5 * 10) / (float)10;
                    QuestionText.text = "Find the average of the following numbers upto one decimal place:";
                    subQuestionText.text = coeff1 + "," + coeff2 + "," + temp + "," + coeff3 + "," + temp2 + "," + coeff4;
                    float ans = (coeff1 + coeff2 + coeff3 + coeff4 + temp + temp2) / 6;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(1, 5);
                    coeff1 *= 10;
                    coeff2 = Random.Range(30, 60);
                    coeff3 = Random.Range(30, 40);
                    coeff4 = Random.Range(30, 40);
                    while(coeff3==coeff4)
                        coeff4 = Random.Range(30, 40);
                    QuestionText.text = "In a class of " + coeff1 + " students, the average weight is " + coeff2 + " kgs. A student weighing " + coeff3 + " kgs leaves the school and a new student of weight " + coeff4 + " kgs joins. Find the new average weight (Correct to one decimal place).";
                    float ans = (float)(coeff1 * coeff2 + coeff4 - coeff3) / (float)coeff1;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(5, 10);
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(3, 10);
                    while (coeff3 == coeff1)
                        coeff3 = Random.Range(3, 10);
                    coeff4 = Random.Range(10, 30);
                    while(coeff2==coeff4)
                        coeff4 = Random.Range(10, 30);
                    QuestionText.text = "The average of " + coeff1 + " numbers is " + coeff2 + " and the average of " + coeff3 + " numbers is " + coeff4 + ". Find the average of these " + (coeff1 + coeff3) + " numbers (Correct to one decimal place).";
                    float ans = (float)(coeff1 * coeff2 + coeff3 * coeff4) / (float)(coeff1 + coeff3);
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 5)
                {
                    coeff1 = Random.Range(1, 21);
                    coeff2 = Random.Range(1, 21);
                    coeff3 = Random.Range(1, 21);
                    coeff4 = Random.Range(1, 21);
                    coeff5 = Random.Range(1, 21);
                    coeff6 = Random.Range(20, 50);
                    QuestionText.text = "The average of 6 numbers " + coeff1 + ", " + coeff2 + ", " + coeff3 + ", " + coeff4 + ", x and " + coeff5 + " is " + coeff6 + ". Find x.";
                    float ans = (float)(coeff6 * 6 - (coeff1 + coeff2 + coeff3 + coeff4 + coeff5));
                    Answer = ans.ToString();
                }
                else if (selector == 6)
                {
                    coeff1 = Random.Range(2, 10);
                    QuestionText.text = "Find the average of first " + coeff1 + " prime numbers(Correct to two decimal places).";
                    if (coeff1 == 2)
                        Answer = (2.5).ToString();
                    else if (coeff1 == 3)
                        Answer = (3.33).ToString();
                    else if (coeff1 == 4)
                        Answer = (4.25).ToString();
                    else if (coeff1 == 5)
                        Answer = (5.6).ToString();
                    else if (coeff1 == 6)
                        Answer = (6.83).ToString();
                    else if (coeff1 == 7)
                        Answer = (8.28).ToString();
                    else if (coeff1 == 8)
                        Answer = (9.62).ToString();
                    else if (coeff1 == 9)
                        Answer = (11.11).ToString();
                }
                else if (selector == 7)
                {
                    coeff1 = Random.Range(2, 10);
                    coeff2 = Random.Range(2, 10);
                    QuestionText.text = "Find the average of the first " + coeff1 + " multiples of " + coeff2 + ". (Correct to one decimal place)";
                    float ans = (float)(coeff2 * ((coeff1 * (coeff1 + 1)) / 2)) / (float)coeff1;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 8)
                {
                    coeff1 = Random.Range(30, 50);
                    coeff2 = Random.Range(30, 60);
                    coeff3 = Random.Range(40, 80);
                    coeff4 = Random.Range(40, 90);
                    while(coeff3==coeff4)
                        coeff4 = Random.Range(40, 90);
                    QuestionText.text = "Section 7A has " + coeff1 + " students and 7B has " + coeff2 + " students. The average test score of 7A is " + coeff3 + " and 7B is " + coeff4 + ". What is the average test score of these two sections? (Correct to one decimal place)";
                    float ans = (float)(coeff1 * coeff3 + coeff2 * coeff4) / (float)(coeff1 + coeff2);
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
            }
            #endregion
            #region level3
            if (level == 3)
            {

                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
                    coeff1 = Random.Range(30, 70);
                    coeff2 = Random.Range(30, 61);
                    int[] num = new int[5];
                    for (int i = 0; i < 5; i++)
                        num[i] = i;
                    int rand = Random.Range(1, 4);
                    int rand2 = Random.Range(2, 5);
                    while (rand >= rand2 || (rand == 2 && rand2 == 4))
                        rand2 = Random.Range(1, 5);
                    QuestionText.text = "A Nano car is driven at an average speed of " + coeff1 + "kms/hr to cover " + rand + "/" + rand2 + " part of a journey and the remaining part of journey with " + coeff2 + "kms/hr. Calculate the average speed of the whole journey. (Correct to one decimal place)";
                    int rand3 = rand2 - rand;
                    float ans = (float)((coeff1 * rand) + (coeff2 * rand3)) / (float)rand2;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(1, 4);
                    coeff1 *= 10;
                    coeff2 = Random.Range(1, 4);
                    coeff2 *= 10;
                    coeff3 = Random.Range(200, 500);
                    float rr = Random.Range(2.0f, 10.0f);
                    rr = Mathf.Round(rr * 10) / (float)10;
                    while (coeff3 < rr * coeff1 + 20)
                        coeff3 = Random.Range(200, 500);
                    QuestionText.text = "In a cricket game, for the first " + coeff1 + " overs the average scoring rate is " + rr + ". What should be the average scoring rate in the remaining " + coeff2 + " overs to score " + coeff3 + " runs in " + (coeff1 + coeff2) + " overs. (Correct to two decimal places)";
                    float ans = (float)((coeff3) - (rr * coeff1)) / (float)coeff2;
                    ans = Mathf.Round(ans * 100) / (float)100;
                    Answer = ans.ToString();
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(60, 80);
                    coeff2 = Random.Range(30, 60);
                    coeff3 = Random.Range(5, 20);
                    QuestionText.text = "A family has 2 grandparents, 2 parents and 3 children. The average age of grandparents is " + coeff1 + ", that of parents is " + coeff2 + " and that of the children is " + coeff3 + " years. Find the average age of the family. (Correct to one decimal place)";
                    float ans = (float)(coeff1 * 2 + coeff2 * 2 + coeff3 * 3) / (float)7;
                    ans = Mathf.Round(ans * 10) / (float)10;
                    Answer = ans.ToString();
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(80, 100);
                    coeff2 = Random.Range(60, 80);
                    coeff3 = Random.Range(2, 5);
                    QuestionText.text = "A student's marks were wrongly entered as " + coeff1 + " instead of " + coeff2 + ". As a result the average marks of the class got increased by 1/"+coeff3+". Find the number of students in the class (to the nearest unit). ";
                    Answer = (coeff3 * (coeff1 - coeff2)).ToString();
                }
                else if (selector == 5)
                {
                    coeff1 = Random.Range(3, 10);
                    coeff2 = Random.Range(40, 60);
                    coeff4 = Random.Range(30, 40);
                    coeff3 = Random.Range(1, 10);
                    while (coeff1 == coeff2)
                        coeff3 = Random.Range(1, 10);
                    QuestionText.text = "The average age of a man, wife and their child " + coeff1 + " years ago was " + coeff2 + " years. The average age of wife and the child " + coeff3 + " years ago was " + coeff4 + " years. Find the present age of the man.";
                    Answer = (3 * coeff2 + 3 * coeff1 - (2 * (coeff3 + coeff4))).ToString();
                }
            }
            #endregion
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
            {    //.
                if (checkLastTextFor(new string[1] { "." }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += ".";
            }
            else if (value == 11)
            {   // All Clear
                if (userAnswerText.text.Length > 0)
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
            }
        }
    }
}

