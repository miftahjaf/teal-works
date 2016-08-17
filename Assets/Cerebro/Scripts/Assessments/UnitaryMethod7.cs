using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
    public class UnitaryMethod7 : BaseAssessment
    {

        public Text subQuestionText;
        private string Answer;
        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
        private int coeff6;
        private int num;
        private int den;
        private int hcf;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "UNM07", "S01", "A01");

            scorestreaklvls = new int[6];
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
            //var correct = false;
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
            if (Answer.Contains("/"))
            {
                var correctAnswers = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
                var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                CerebroHelper.DebugLog("userAnswers[0]=" + userAnswers[0]);
				correct = MathFunctions.checkFractions(userAnswers, correctAnswers);
                CerebroHelper.DebugLog(correct);
            }
            else
            {
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
            }
            if (correct == true)
            {
                if (Queslevel == 1)
                {
                    increment = 2;
                }
                else if (Queslevel == 2)
                {
                    increment = 4;
                }
                else if (Queslevel == 3)
                {
                    increment = 6;
                }
                else if (Queslevel == 4)
                {
                    increment = 8;
                }
                else if (Queslevel == 5)
                {
                    increment = 10;
                }
                else if (Queslevel == 6)
                {
                    increment = 12;
                }
                else if (Queslevel == 7)
                {
                    increment = 14;
                }


                UpdateStreak(5, 8);

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
            subQuestionText.gameObject.SetActive(false);
            QuestionText.gameObject.SetActive(true);
            GeneralButton.gameObject.SetActive(true);
            numPad.SetActive(true);
            if (Queslevel > scorestreaklvls.Length)
            {
                level = UnityEngine.Random.Range(1, scorestreaklvls.Length + 1);
            }

            //level = 6;
            
            #region level1
            if (level == 1)
            {
                selector = GetRandomSelector(1, 6);

               // selector = 2;

                if (selector == 1)
                {
                    //subQuestionText.gameObject.SetActive(true);
                    coeff1 = 5*Random.Range(1, 8);
                    coeff2 = Random.Range(6, 40);
                    while(coeff1==coeff2)
                        coeff2 = Random.Range(6, 40);
                    coeff3 = 5*Random.Range(4, 20);
                    QuestionText.text = "The cost of "+coeff1+" apples is Rs."+coeff3+". What is the cost of "+coeff2+" apples? (Correct to two decimal places)";
                    float ans = (float)(coeff2 * coeff3) / (float)coeff1;
                    ans = Mathf.Round(ans * 100) / (float)100;
                    Answer = ans.ToString();
                    //Answer = Mathf.Abs(coeff2 * coeff3) + "/" + Mathf.Abs(coeff1);
                }
                else if (selector == 2)
                {
                    int rand = Random.Range(1, 4);
                    int rand2 = Random.Range(2, 5);
                    while (rand >= rand2 || (rand == 2 && rand2 == 4)|| (rand == 1 && rand2==2))
                        rand2 = Random.Range(2, 5);
                    coeff1 = Random.Range(1,15);
                    coeff1 *= 10;
                    QuestionText.text = "A tap can fill half of a container in " + coeff1 + " minutes. How long (in minutes) will it take to fill " + rand + "/" + rand2 + " of the container? (Answer in fractions)";
                    //float ans = (float)(2*coeff1*rand) / (float)rand2;
                    //ans = Mathf.Round(ans * 10) / (float)10;
                    //Answer = ans.ToString();
                    //Answer = Mathf.Abs(2 * coeff1 * rand) + "/" + Mathf.Abs(rand2);
                    num = 2 * coeff1 * rand;
                    den = rand2;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if(Mathf.Abs(den)==1)
                        Answer = num.ToString();
                    else 
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);

                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(5, 10);
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(20, 40);
                    QuestionText.text = coeff1 + " workers can do a piece of work in " + coeff3 + " days. In how many days can " + coeff2 + " workers do the same work? (Answer in fractions)";
                    //float ans = (float)(coeff2 * coeff3) / (float)coeff1;
                    //ans = Mathf.Round(ans * 10) / (float)10;
                    //Answer = ans.ToString();
                    //Answer = Mathf.Abs(coeff1 * coeff3) + "/" + Mathf.Abs(coeff2);
                    num = coeff1 * coeff3;
                    den = coeff2;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 4)
                {
                    coeff1 = 5*Random.Range(1, 21);
                    int rand = Random.Range(1, 5);
                    int rand2 = Random.Range(2, 9);
                    while (rand2 == rand || rand==7)
                        rand2 = Random.Range(2, 10);
                    int hcf = MathFunctions.GetHCF(rand, rand2);
                    rand /= hcf;
                    rand2 /= hcf;
                    //int rand = Random.Range(1, 10);
                    //int rand2 = Random.Range(2, 10);
                    //while (rand >= rand2 || (rand%rand2==0) || rand2%rand==0)
                    //    rand2 = Random.Range(1, 5);
                    //int rand3 = Random.Range(1, 10);
                    //int rand4 = Random.Range(2, 10);
                    //while (rand3 >= rand4 || rand % rand2 == 0 || rand2 % rand == 0 || (rand3==rand && rand4==rand2))
                    //    rand4 = Random.Range(1, 5);
                    int rand3 = Random.Range(1, 5);
                    int rand4 = Random.Range(2, 9);
                    while (rand3 == rand4 || (rand==rand3 && rand2==rand4) || rand4==7)
                        rand4 = Random.Range(2, 10);
                    int hcf2 = MathFunctions.GetHCF(rand3, rand4);
                    rand3 /= hcf2;
                    rand4 /= hcf2;
                    QuestionText.text = "If " + rand + "/" + rand2 + " of Maeby's pocket money is Rs. " + coeff1 + " What is " + rand3 + "/" + rand4 + " of her pocket money? (Correct to two decimal places)";
                    float ans = (float)(coeff1 * rand3 * rand2) / (float)rand4 * rand;
                    ans = Mathf.Round(ans * 100) / (float)100;
                    Answer = ans.ToString();
                    //Answer = Mathf.Abs(coeff1 * rand3 * rand2) + "/" + Mathf.Abs(rand4 * rand);
                }
                else if(selector==5)
                {
                    coeff1 = Random.Range(1, 10);
                    coeff1 *= 10;
                    coeff2 = Random.Range(1, 50);
                    while(coeff2>coeff1)
                        coeff2 = Random.Range(1, 50);
                    QuestionText.text = "Lucille can complete a task in " + coeff1 + " days. What part of task can she complete in " + coeff2 + " days? (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff2) + "/" + Mathf.Abs(coeff1);
                    num = coeff2;
                    den = coeff1;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                
            }
            #endregion
            #region level2
            if (level == 2)
            {
                selector = GetRandomSelector(1, 6);

                //selector = 12;
                if (selector == 1)
                {
                    coeff1 = Random.Range(50, 100);
                    coeff1 *= 2;
                    coeff2 = Random.Range(2, 10);
                    QuestionText.text = "A scooter covers " + coeff1 + " kms in " + coeff2 + " hrs. Find the average speed (km/hr) of the scooter. (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff1)+ "/" + Mathf.Abs(coeff2);
                    num = coeff1;
                    den = coeff2;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(1, 11);
                    coeff1 *= 5;
                    coeff2 = Random.Range(1, 11);
                    coeff2 *= 5;
                    while (coeff1 == coeff2)
                    {
                        coeff2 = Random.Range(1, 11);
                        coeff2 *= 5;
                    }
                    QuestionText.text = "A watch loses " + coeff1 + " seconds everyday (24hrs). In how many hours will it lose " + coeff2 + " seconds? (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff2 * 24) + "/" + Mathf.Abs(coeff1);
                    num = coeff2 * 24;
                    den = coeff1;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(10, 50);
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(3, 10);
                    while (coeff3 >= coeff1)
                        coeff3 = Random.Range(1, 10);
                    QuestionText.text = coeff1 + " students can paint their classroom in " + coeff2 + " days. But " + coeff3 + " students drop out of the painting job. How long will the remaining students take to paint the room? (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff1 * coeff2) + "/" + Mathf.Abs(coeff1-coeff3);
                    num = coeff1 * coeff2;
                    den = coeff1 - coeff3;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(2, 6);
                    coeff2 = Random.Range(2, 10);
                    while (coeff2 < coeff1)
                        coeff2 = Random.Range(2, 10);
                    coeff3 = Random.Range(10, 30);
                    coeff4 = coeff1 * coeff2 * Random.Range(5, 10) * 20;
                    coeff5 = Random.Range(2, 6);
                    while (coeff5 == coeff1)
                        coeff5 = Random.Range(2, 6);
                    coeff6 = Random.Range(2, 10);
                    while (coeff6 == coeff2 && coeff6 < coeff5)
                        coeff6 = Random.Range(2, 10);
                    int coeff7 = Random.Range(10, 30);
                    while (coeff7 == coeff3)
                        coeff7 = Random.Range(10, 30);
                    QuestionText.text = coeff1 + " men or " + coeff2 + " women can earn Rs." + coeff4 + " in " + coeff3 + " days. How much will " + coeff5 + " men and " + coeff6 + " women earn in " + coeff7 + " days? (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff4 * coeff7 * (coeff5 * coeff2 + coeff6 * coeff1)) + "/" + Mathf.Abs(coeff1 * coeff2 * coeff3);
                    num = coeff4 * coeff7 * (coeff5 * coeff2 + coeff6 * coeff1);
                    den = coeff1 * coeff2 * coeff3;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 5)
                {
                    coeff1 = Random.Range(1, 10);
                    coeff1 *= 10;
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(4, 11);
                    coeff4 = Random.Range(5, 25);
                    while (coeff4 >= coeff1)
                        coeff4 = Random.Range(5, 25);
                    QuestionText.text = "In a stable there is enough food for " + coeff1 + " horses to last " + coeff2 + " days. After " + coeff3 + " days, " + coeff4 + " horses are sold. How many more days will the food last for the rest of the horses? (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff1 * coeff2 - coeff3 * coeff1) + "/" + Mathf.Abs(coeff1 - coeff4);
                    num = coeff1 * coeff2 - coeff3 * coeff1;
                    den = coeff1 - coeff4;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
            }
            #endregion
            #region level3
            if (level == 3)
            {
                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
                    coeff1 = Random.Range(1, 10);
                    coeff1 *= 10;
                    coeff2 = Random.Range(1, 10);
                    coeff2 *= 10;
                    while (coeff2 == coeff1)
                    {
                        coeff2 = Random.Range(1, 10);
                        coeff2 *= 10;
                    }
                    QuestionText.text = "Bran can complete a task in " + coeff1 + " days, Bronn can do the same task in " + coeff2 + " days. How many days will they take working together? (Answer in fractions)";
                    // Answer = Mathf.Abs(coeff1 * coeff2) + "/" + Mathf.Abs(coeff1 + coeff2);
                    num = coeff1 * coeff2;
                    den = coeff1 + coeff2;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(5, 10);
                    coeff1 *= 10;
                    coeff3 = Random.Range(1, 5);
                    coeff3 *= 10;
                    while (coeff3 >= coeff1)
                    {
                        coeff3 = Random.Range(1, 5);
                        coeff3 *= 10;
                    }
                    QuestionText.text = "Batla and Laxmi can do a job in " + coeff3 + " days working together. Batla can do it alone in " + coeff1 + " days. How much time will Laxmi take to do the job alone? (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff3 * coeff1) + "/" + Mathf.Abs(coeff1 - coeff3);
                    num = coeff3 * coeff1;
                    den = coeff1 - coeff3;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(5, 21);
                    coeff2 = Random.Range(1, 10);
                    coeff2 *= 10;
                    coeff3 = Random.Range(5, 21);
                    coeff4 = Random.Range(3, 21);
                    while (coeff4 == coeff1)
                        coeff4 = Random.Range(3, 21);
                    coeff5 = Random.Range(1, 10);
                    coeff5 *= 10;
                    while (coeff5 == coeff2)
                    {
                        coeff5 = Random.Range(1, 10);
                        coeff5 *= 10;
                    }
                    //coeff1 = Random.Range(1, 21);
                    QuestionText.text = coeff1 + " workers can construct a " + coeff2 + "m drain in " + coeff3 + " days. How long will " + coeff4 + " workers take to construct a " + coeff5 + "m drain? (Answer in fractions)";
                    //Answer= Mathf.Abs(coeff3 * coeff1 * coeff5) + "/" + Mathf.Abs(coeff4 * coeff2);
                    num = coeff3 * coeff1 * coeff5;
                    den = coeff4 * coeff2;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);

                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(1, 11);
                    coeff2 = Random.Range(1, 30);
                    coeff2 *= 10;
                    coeff3 = Random.Range(1, 50);
                    coeff3 *= 10;
                    while (coeff3 <= coeff2)
                    {
                        coeff3 = Random.Range(1, 50);
                        coeff3 *= 10;
                    }
                    QuestionText.text = "A motorbike takes " + coeff1 + "hrs to cover " + coeff2 + "kms. How long (hours) will it take to cover " + coeff3 + "kms? (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff3 * coeff1) + "/" + Mathf.Abs(coeff2);
                    num = coeff3 * coeff1;
                    den = coeff2;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 5)
                {
                    coeff1 = Random.Range(5, 21);
                    coeff1 *= 5;
                    coeff2 = Random.Range(20, 40);
                    coeff2 *= 5;
                    coeff3 = Random.Range(2, 7);
                    coeff4 = Random.Range(20, 60);
                    coeff4 *= 5;
                    QuestionText.text = "A car travelling at a speed of " + coeff1 + "kms/hr covers a distance of " + coeff2 + "kms, and takes another " + coeff3 + "hrs to cover the next " + coeff4 + "kms. Find the average speed (km/hr) for the whole journey. (Correct to two decimal places)";
                    float ans = (float)(coeff2 * coeff1 + coeff1 * coeff4) / (float)(coeff2 + coeff1 * coeff3);
                    ans = Mathf.Round(ans * 100) / (float)100;
                    Answer = ans.ToString();
                }
            }
            #endregion
            #region level4
            if (level == 4)
            {
                selector = GetRandomSelector(1, 6);
                if (selector == 1)
                {
                    coeff1 = Random.Range(2, 6);
                    coeff1 *= 5;
                    coeff2 = coeff1 + (Random.Range(2, 4) * 5);
                    coeff3 = Random.Range(3, 10);
                    QuestionText.text = "Frodo takes " + coeff1 + "mins to go to school but requires " + coeff2 + "mins for the return journey. If the school is " + coeff3 + "kms away, find his average speed in km/hr. (Correct to two decimal places)";
                    float ans = (float)(120 * coeff3) / (float)(coeff2 + coeff1);
                    ans = Mathf.Round(ans * 100) / (float)100;
                    Answer = ans.ToString();
                    //Answer = Mathf.Abs(120 * coeff3) + "/" + Mathf.Abs(coeff2 + coeff1);
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(4, 10);
                    coeff1 *= 18;
                    coeff2 = Random.Range(10, 20);
                    QuestionText.text = "Speed of a train is " + coeff1 + "kms/hr. It took " + coeff2 + " seconds to cross a signal post completely. Find the length of the train in metres.";
                    float c1 = (float)coeff1 * 5 / (float)18;
                    //coeff1 = coeff1 * (5 /18); 
                    Answer = Mathf.Abs(c1 * coeff2).ToString();
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(100, 300);
                    coeff2 = Random.Range(3, 7);
                    coeff2 *= 10;
                    QuestionText.text = "How long (in seconds) will a 300m long train take to cross a " + coeff1 + "m long bridge if it is travelling at a speed of " + coeff2 + "m/s? (Answer in fractions)";
                    //Answer = Mathf.Abs(300 + coeff1) + "/" + Mathf.Abs(coeff2);
                    num = 300 + coeff1;
                    den = coeff2;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(3, 10);
                    coeff2 = Random.Range(50, 80);
                    coeff3 = Random.Range(2, 9);
                    while (coeff3 == coeff1)
                        coeff3 = Random.Range(2, 10);
                    QuestionText.text = "A car takes " + coeff1 + "hrs to complete a journey travelling at " + coeff2 + "kms/hr. What should be the speed (in kms/hr) of the car if you wish to complete the journey in " + coeff3 + " hours? (Correct to two decimal places";
                    float ans = (float)(coeff2 * coeff1) / (float)(coeff3);
                    ans = Mathf.Round(ans * 100) / (float)100;
                    Answer = ans.ToString();
                    //Answer = Mathf.Abs(coeff2 * coeff1) + "/" + Mathf.Abs(coeff3);
                }
                else if (selector == 5)
                {
                    coeff1 = Random.Range(10, 20);
                    coeff1 *= 12;
                    coeff2 = Random.Range(4, 10);
                    coeff3 = Random.Range(4, 20);
                    while (coeff3 == coeff2)
                        coeff3 = Random.Range(4, 20);
                    QuestionText.text = "Apples are sold at the rate of Rs. " + coeff1 + " per dozen. The weight of " + coeff2 + " apples is 1kg. Find the cost (in Rs.) of " + coeff3 + "kgs of apples. (Correct to two decimal places)";
                    float ans = (float)(coeff3 * coeff1 * coeff2) / (float)(12);
                    ans = Mathf.Round(ans * 100) / (float)100;
                    Answer = ans.ToString();
                    //Answer = Mathf.Abs(coeff3 * coeff1) + "/" + Mathf.Abs(coeff2 * 12);
                }
            }
            #endregion
            #region level5
            if (level == 5)
            {
                selector = GetRandomSelector(1, 5);
                if (selector == 1)
                {
                    coeff1 = Random.Range(5, 15);
                    coeff2 = Random.Range(1, 5);
                    coeff2 *= 100;
                    coeff3 = Random.Range(1, 5);
                    coeff3 *= 10;
                    QuestionText.text = "A honda scooter takes " + coeff1 + "hrs to cover " + coeff2 + "km distance. If its average speed is increased by " + coeff3 + "km/h, calculate the amount of time (in hrs) saved to cover the same distance. (Answer in fractions)";
                    //Answer = Mathf.Abs(coeff3 * coeff1 * coeff1) + "/" + Mathf.Abs(coeff2 + coeff1 * coeff3);
                    num = coeff3 * coeff1 * coeff1;
                    den = coeff2 + coeff1 * coeff3;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(10, 20);
                    coeff2 = Random.Range(5, 30);
                    coeff3 = Random.Range(3, 15);
                    coeff4 = Random.Range(5, 25);
                    while (coeff4 == coeff1)
                        coeff4 = Random.Range(5, 25);
                    coeff5 = Random.Range(3, 10);
                    while (coeff5 == coeff3)
                        coeff5 = Random.Range(3, 10);
                    QuestionText.text = coeff1 + " tailors can stitch " + coeff2 + " dresses in " + coeff3 + " days. How many dresses (answer in fractions) can be stitched by " + coeff4 + " tailors in " + coeff5 + " days?";
                    num = coeff2 * coeff5 * coeff4;
                    den = coeff1 * coeff3;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(2, 4);
                    coeff1 *= 5;
                    coeff2 = Random.Range(2, 6);
                    coeff2 *= 5;
                    while (coeff2 == coeff1)
                    {
                        coeff2 = Random.Range(2, 6);
                        coeff2 *= 5;
                    }

                    coeff3 = Random.Range(3, 10);
                    while (coeff3 >= coeff1 || coeff3 >= coeff2)
                        coeff3 = Random.Range(3, 10);
                    QuestionText.text = "Albert can do a work in " + coeff1 + " days. Pinto can do the same work in " + coeff2 + " days. They start working together but Albert leaves the work after " + coeff3 + " days. How many more days will Pinto take to complete the remaining work?";
                    //Answer = Mathf.Abs(coeff2 * coeff1 - coeff2 * coeff3 - coeff1 * coeff3) + "/" + Mathf.Abs(coeff1);
                    num = coeff2 * coeff1 - coeff2 * coeff3 - coeff1 * coeff3;
                    den = coeff1;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(1, 6);
                    coeff2 = Random.Range(1, 10);
                    while (coeff2 <= coeff1)
                        coeff2 = Random.Range(1, 10);
                    QuestionText.text = "A swimming pool has two pumps A and B. Pump A can fill the pump in " + coeff1 + " hrs whereas pump B can empty the pool in " + coeff2 + "hrs. The pool incharge mistakenly switched on both the pumps. In how many hrs will the empty pool be filled ?";
                    //Answer = Mathf.Abs(coeff2 - coeff1) + "/" + Mathf.Abs(coeff2 * coeff1);
                    den = coeff2 - coeff1;
                    num = coeff2 * coeff1;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
            }
            #endregion
            #region level6
            if (level == 6)
            {
                selector = GetRandomSelector(1, 5);

                //selector = 4;

                if (selector == 1)
                {
                    coeff2 = Random.Range(1, 10);
                    coeff2 *= 10;
                    coeff1 = Random.Range(5, 21);
                    coeff3 = Random.Range(5, 21);
                    coeff4 = Random.Range(3, 21);
                    while (coeff4 == coeff1)
                        coeff4 = Random.Range(3, 21);
                    coeff5 = Random.Range(5, 15);
                    QuestionText.text = coeff1 + " workers can construct a " + coeff2 + "m drain in " + coeff3 + " days. How long a drain can be constructed by " + coeff4 + " workers in " + coeff5 + " days?";
                    //Answer = Mathf.Abs(coeff2 * coeff4 * coeff5) + "/" + Mathf.Abs(coeff1 * coeff3);
                    num = coeff2 * coeff4 * coeff5;
                    den = coeff1 * coeff3;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);

                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(1, 10);
                    coeff1 *= 1000;
                    coeff2 = Random.Range(1, 10);
                    coeff2 *= 10;
                    coeff3 = Random.Range(1, 10);
                    coeff3 *= 100;
                    coeff4 = Random.Range(1, 10);
                    coeff4 *= 100;
                    while (coeff4 == coeff3)
                    {
                        coeff4 = Random.Range(1, 10);
                        coeff4 *= 100;
                    }
                    QuestionText.text = "A refugee camp has enough food for " + coeff1 + " people for " + coeff2 + " days when everyone gets " + coeff3 + "gms of food daily. If the amount of daily food is reduced to " + coeff4 + "gms a day, how long will the food last?";
                    //Answer = Mathf.Abs(coeff2 * coeff3) + "/" + Mathf.Abs(coeff4);
                    num = coeff2 * coeff3;
                    den = coeff4;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(1, 5);
                    coeff1 *= 100;
                    coeff2 = Random.Range(10, 20);
                    coeff3 = Random.Range(1, 9);
                    QuestionText.text = "A train crosses a bridge of " + coeff1 + "m length in " + coeff2 + " seconds and a pole in " + coeff3 + " seconds. Find the length (in metres) of the train.";
                    //Answer = Mathf.Abs(coeff1 * coeff3) + "/" + Mathf.Abs(coeff2-coeff3);
                    num = coeff1 * coeff3;
                    den = coeff2 - coeff3;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(4, 11);                  
                    coeff2 = Random.Range(11, 15);
                    while (coeff1 >= coeff2)
                    {
                        coeff2 = Random.Range(1, 10);
                        coeff2 *= 100;
                    }
                    coeff3 = Random.Range(100, 250);
                    coeff4 = Random.Range(100, 250);
                    QuestionText.text = "Train A of length "+coeff3+"m and train B of length "+coeff4+"m are running on parallel tracks in the same direction.Their average speeds are " + coeff1*18 + "kms/hr and " + coeff2*18 + "kms/hr respectively. How long (in seconds) will B take to overtake A?";
                    //Answer = Mathf.Abs(coeff3+coeff4) + "/" + Mathf.Abs(coeff2 - coeff1);
                    num = coeff3 + coeff4;
                    den = (coeff2 - coeff1)*5;
                    hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    if (Mathf.Abs(den) == 1)
                        Answer = num.ToString();
                    else
                        Answer = Mathf.Abs(num) + "/" + Mathf.Abs(den);
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
            {    ///
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += "/";
            }
            else if (value == 11)
            {   // All Clear
                //                userAnswerText.text = "";
                if (userAnswerText.text.Length > 0)
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
            }
            else if (value == 12)
            {    //.
                if (checkLastTextFor(new string[1] { "." }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += ".";
            }
        }
    }
}

