using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
    public class SimpleInterest7 : BaseAssessment
    {

        private float Principal;
        private float Rate;
        private float Interest;
        private float Amount;
        private float Years;
        private float Months;

        private float Answer;
        private string Answerstring;

        private float multiplier;

        public Text AmountText;
        public Text PrincipalText;
        public Text RateText;
        public Text TimeText;
        public Text InterestText;

        //		public Text QuestionText;

        public Text PrincipalQuestion;
        public Text TimeQuestion;
        public Text RateQuestion;
        public Text InterestQuestion;
        public Text AmountQuestion;

        public Button PrincipalButton;
        public Button TimeButton;
        public Button RateButton;
        public Button InterestButton;
        public Button AmountButton;

        private float[] RateValues = new float[] { 2f, 3f, 4f, 5f, 6f, 8f, 9f, 10f, 12f, 15f };
        private float[] TimeValues = new float[] { 2f, 3f, 4f, 5f, 6f, 8f, 10f };
        private float[] TimeValues2 = new float[] { 1f, 2f, 4f, 5f, 10f, 20f, 25f, 50f, 100f }; //1, 2, 4, 5, 10, 20, 25, 50, 100
        private float[] MonthValues = new float[] { 2f, 4f, 6f, 3f, 8f, 10f };

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "SI07", "S01", "A01");

            scorestreaklvls = new int[5];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            Principal = 0f;
            Rate = 0f;
            Years = 0f;
            Interest = 0f;

            levelUp = false;

            multiplier = 0;
            Answer = 0f;
            GenerateQuestion();
        }

        public override void SubmitClick()
        {
            if (ignoreTouches || userAnswerText.text == "")
            {
                return;
            }

            questionsAttempted++;
            updateQuestionsAttempted();

            int increment = 0;
            //var correct = false;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            var correct = false;
            //float answer = 0;
            float userAnswer = 0;
            //bool directCheck = false;
            if (level == 3 && selector == 5)
            {
                var correctAnswers = Answerstring.Split(new string[] { "/" }, System.StringSplitOptions.None);
                //CerebroHelper.DebugLog(correctAnswers[0]);
                //CerebroHelper.DebugLog(correctAnswers[1]);
                var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                //CerebroHelper.DebugLog(userAnswers[0]);
                //CerebroHelper.DebugLog(userAnswers[1]);
                correct = MathFunctions.checkFractions(userAnswers, correctAnswers);
                CerebroHelper.DebugLog(correct);
            }
            else
            {
                //if (float.TryParse(Answer.ToString(), out answer))
                //{
                //    answer = float.Parse(Answer.ToString());
                //}
                //else
                //{
                //    directCheck = true;
                //}
                if (float.TryParse(userAnswerText.text, out userAnswer))
                {
                    userAnswer = float.Parse(userAnswerText.text);
                }

                if (Answer != userAnswer)
                {
                    correct = false;
                }
                else
                    correct = true;
            }
            if (correct == true)
            {
                //correct = true;

                if (Queslevel == 1)
                {
                    increment = 5;
                }
                else if (Queslevel == 2)
                {
                    increment = 5;
                }
                else if (Queslevel == 3)
                {
                    increment = 10;
                }
                else if (Queslevel == 4)
                {
                    increment = 10;
                }
                else if (Queslevel == 5)
                {
                    increment = 15;
                }
                else if (Queslevel == 6)
                {
                    increment = 15;
                }

                UpdateStreak(5, 8);

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
            if (level == 3 && selector == 5)
            {
                if (isRevisitedQuestion)
                {
                    userAnswerText.text = "";
                    userAnswerText.color = MaterialColor.textDark;
                    ignoreTouches = false;
                }
                else
                {
                    userAnswerText.text = Answerstring;
                    userAnswerText.color = MaterialColor.green800;
                }
            }
            else
            {
                if (isRevisitedQuestion)
                {
                    userAnswerText.text = "";
                    userAnswerText.color = MaterialColor.textDark;
                    ignoreTouches = false;
                }
                else
                {
                    userAnswerText.text = Answer.ToString();
                    userAnswerText.color = MaterialColor.green800;
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

            Principal = Random.Range(1, 21);
            Principal = Principal * 1200;

            Rate = RateValues[Random.Range(0, RateValues.Length)];
            Years = TimeValues[Random.Range(0, TimeValues.Length)];
            Months = MonthValues[Random.Range(0, MonthValues.Length)];

            PrincipalQuestion.text = "Rs. " + Principal.ToString();
            RateQuestion.text = Rate.ToString() + " %";
            TimeQuestion.text = Years.ToString() + " Years";

            Interest = Principal * Rate * Years / 100f;
            Amount = Principal + Interest;

            InterestQuestion.text = "Rs. " + Interest.ToString();
            AmountQuestion.text = "Rs. " + Amount.ToString();

            PrincipalButton.gameObject.SetActive(false);
            RateButton.gameObject.SetActive(false);
            InterestButton.gameObject.SetActive(false);
            TimeButton.gameObject.SetActive(false);
            GeneralButton.gameObject.SetActive(false);
            AmountButton.gameObject.SetActive(false);
            AmountText.gameObject.SetActive(false);

            PrincipalQuestion.gameObject.SetActive(false);
            RateQuestion.gameObject.SetActive(false);
            TimeQuestion.gameObject.SetActive(false);
            InterestQuestion.gameObject.SetActive(false);
            AmountQuestion.gameObject.SetActive(false);

            PrincipalText.gameObject.SetActive(false);
            RateText.gameObject.SetActive(false);
            TimeText.gameObject.SetActive(false);
            InterestText.gameObject.SetActive(false);
            AmountText.gameObject.SetActive(false);

            answerButton = null;
            userAnswerText = null;

            level = Queslevel;

            if (Queslevel > scorestreaklvls.Length)
            {
                level = UnityEngine.Random.Range(1, scorestreaklvls.Length + 1);
            }

            //level = 2;
            
            #region level1
            if (level == 1)
            {
                QuestionText.text = "Find the missing data";

                PrincipalQuestion.gameObject.SetActive(true);
                RateQuestion.gameObject.SetActive(true);
                TimeQuestion.gameObject.SetActive(true);
                InterestQuestion.gameObject.SetActive(true);

                PrincipalText.gameObject.SetActive(true);
                RateText.gameObject.SetActive(true);
                TimeText.gameObject.SetActive(true);
                InterestText.gameObject.SetActive(true);
                 
                selector = GetRandomSelector(1, 6);

                // selector = 2;

                if (selector == 1)
                {
                    InterestQuestion.text = "Rs. ";
                    InterestButton.gameObject.SetActive(true);
                    answerButton = InterestButton;
                    Answer = Interest;
                }
                else if(selector==2)
                {
                    PrincipalQuestion.text = "Rs. ";
                    PrincipalButton.gameObject.SetActive(true);
                    answerButton = PrincipalButton;
                    Answer = Principal;
                }
                else if(selector==3)
                {
                    RateQuestion.text = "         %";
                    RateButton.gameObject.SetActive(true);
                    answerButton = RateButton;
                    Answer = Rate;
                }
                else if(selector==4)
                {
                    TimeQuestion.text = Years.ToString() + " Years and " + Months + " Months";
                    RateQuestion.text = Rate + " % per annum";
                    InterestQuestion.text = "Rs. ";
                    InterestButton.gameObject.SetActive(true);
                    answerButton = InterestButton;
                    Years += Months / 12;
                    Interest = Principal * Rate * Years / 100f;
                    Amount = Principal + Interest;
                    Answer = Interest;
                }
                else if(selector==5)
                {
                    TimeQuestion.text = Years.ToString() + " Years and " + Months + " Months";
                    RateQuestion.text = Rate.ToString() + " % per quarter";
                    InterestQuestion.text = "Rs. ";
                    InterestButton.gameObject.SetActive(true);
                    answerButton = InterestButton;
                    Years *= 4;
                    Months /= 3;
                    Years += Months;
                    Interest = Principal * Rate * Years / 100f;
                    Amount = Principal + Interest;
                    Answer = Interest;
                }
            }
            #endregion
            #region level2
            if(level==2)
            {
                selector = GetRandomSelector(1, 6);

                //selector = 1;

                if (selector == 1)
                {
                    QuestionText.text = "Find the missing data";

                    PrincipalQuestion.gameObject.SetActive(true);
                    RateQuestion.gameObject.SetActive(true);
                    TimeQuestion.gameObject.SetActive(true);
                    InterestQuestion.gameObject.SetActive(true);

                    PrincipalText.gameObject.SetActive(true);
                    RateText.gameObject.SetActive(true);
                    TimeText.gameObject.SetActive(true);
                    InterestText.gameObject.SetActive(true);

                    //Years = Months = 11;

                    TimeQuestion.text = Years.ToString() + " Years and " + Months + " Months";
                    int rand = Random.Range(1, 4);
                    int rand2 = Random.Range(2, 5);
                    while (rand >= rand2 || (rand == 2 && rand2 == 4))
                        rand2 = Random.Range(1, 5);
                    RateQuestion.text = rand + "/" + rand2 + " % per month";
                    InterestQuestion.text = "Rs. ";
                    InterestButton.gameObject.SetActive(true);
                    answerButton = InterestButton;
                    Years *= 12;
                    Years += Months;
                    Interest = (Principal * rand * Years) / (rand2 * 100f);
                    Amount = Principal + Interest;
                    Answer = Interest;
                }
                else if (selector == 2)
                {
                    QuestionText.text = "Find the missing data";

                    PrincipalQuestion.gameObject.SetActive(true);
                    RateQuestion.gameObject.SetActive(true);
                    TimeQuestion.gameObject.SetActive(true);
                    InterestQuestion.gameObject.SetActive(true);

                    PrincipalText.gameObject.SetActive(true);
                    RateText.gameObject.SetActive(true);
                    TimeText.gameObject.SetActive(true);
                    InterestText.gameObject.SetActive(true);

                    TimeQuestion.text = Years.ToString() + " Years and " + Months + " Months";
                    InterestQuestion.gameObject.SetActive(false);
                    InterestText.gameObject.SetActive(false);
                    RateQuestion.text = Rate + " % per annum";
                    AmountQuestion.text = "Rs. ";
                    AmountQuestion.gameObject.SetActive(true);
                    AmountButton.gameObject.SetActive(true);
                    AmountText.gameObject.SetActive(true);
                    answerButton = AmountButton;
                    Years += Months / 12;
                    Interest = Principal * Rate * Years / 100f;
                    Amount = Principal + Interest;
                    Answer = Amount;
                }
                else if (selector == 3)
                {
                    answerButton = GeneralButton;
                    GeneralButton.gameObject.SetActive(true);

                    QuestionText.text = "Find the amount received after "+Years+" years on Rs. " + Principal.ToString() + " at the rate of " + Rate + "% per annum.";
                    Answer = Amount;
                }
                else if (selector == 4)     
                {
                    answerButton = GeneralButton;
                    GeneralButton.gameObject.SetActive(true);

                    QuestionText.text = "Oliver invests Rs. " + Principal + " at the rate of interest of " + Rate + "% per annum. How much interest does he earn after a period of " + Years + " years " + Months + " months?";
                    Years += Months / 12;
                    Interest = (Principal * Rate * Years) / 100f;
                    Answer = Interest;

                }
                else if (selector == 5)            
                {
                    answerButton = GeneralButton;
                    GeneralButton.gameObject.SetActive(true);

                    while (Years >= 6)
                        Years = TimeValues[Random.Range(0, TimeValues.Length)];
                    while (Rate > 5)
                        Rate = RateValues[Random.Range(0, RateValues.Length)];
                    QuestionText.text = "Find the amount recieved after " + Years + " years on Rs. " + Principal + " at the rate of interest of " + Rate + "% per month.";
                    Years *= 12;
                    Interest = (Principal * Rate * Years) / 100f;
                    Amount = Interest + Principal;
                    Answer = Amount;
                }  
            }
            #endregion
            #region level3
            if (level == 3)
            {
                GeneralButton.gameObject.SetActive(true);
                answerButton = GeneralButton;
                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
                    QuestionText.text = "Rs. " + Interest + " is the interest accrued on a loan of Rs. " + Principal + " for a period of " + Years + " years. What is the rate of interest per annum?";
                    Answer = Rate;
                }
                else if (selector == 2)
                {
                    QuestionText.text = "Maggie invests a sum of Rs. " + Principal + ". How long (in years) will it take at the rate of interest of " + Rate + "%  per annum for the sum to amount to Rs. " + Amount + "?";
                    Answer = Years;
                }
                else if (selector == 3)
                {
                    float Years2 = TimeValues2[Random.Range(0, TimeValues2.Length)];
                    QuestionText.text = "At what rate of simple interest (per annum) a sum of money will double in " + Years2 + " years?";
                    Answer = 100 / Years2;
                }
                else if (selector == 4)
                {
                    int rand = Random.Range(1, 5);
                    int rand2 = Random.Range(2, 10);
                    while (rand2 == rand)
                        rand2 = Random.Range(2, 10);
                    int hcf = MathFunctions.GetHCF(rand, rand2);
                    rand /= hcf;
                    rand2 /= hcf;
                    //while (rand >= rand2 || MathFunctions.GetHCF(rand,rand2)!=1 || (rand == 1 && rand2 == 2))
                    //    rand2 = Random.Range(1, 5);
                    //float Years2 = TimeValues[Random.Range(1, TimeValues.Length)];            
                    QuestionText.text = "The interest on a sum of money in " + Years + " years is " + rand + "/" + rand2 + " of the original sum. Calculate the rate of interest per annum. (Correct to two decimal places)";
                    float tempans = (rand * 100) / (rand2 * Years);
                    Answer = Mathf.Round(tempans * 100) / (float)100;
                }
                else if (selector == 5)
                {
                    while (Years == 3 || Years == 5)
                        Years = TimeValues[Random.Range(0, TimeValues.Length)];
                    float Years2 = TimeValues[Random.Range(0, TimeValues.Length)];
                    while (Years == Years2)
                        Years2 = TimeValues[Random.Range(0, TimeValues.Length)];
                    QuestionText.text = "What will be the ratio of simple interests earned when a certain amount of money is invested at the same rate of interest for " + Years + " years and " + Years2 + " years?";
                    int num = (int)Years;
                    int den = (int)Years2;
                    int hcf = MathFunctions.GetHCF(num, den);
                    num /= hcf;
                    den /= hcf;
                    Answerstring = Mathf.Abs(num) + "/" + Mathf.Abs(den);
                    //Answerstring = Years + "/" + Years2;
                }
            }
            #endregion
            #region level4
            if(level==4)
            {
                answerButton = GeneralButton;
                GeneralButton.gameObject.SetActive(true);

                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
                    QuestionText.text = "What sum of money will amount to Rs. " + Amount + " at " + Rate + "% simple interest per annum in " + Years + " years?";
                    Answer = Principal;
                }
                else if (selector == 2)
                {
                    float Years2 = TimeValues[Random.Range(0, TimeValues.Length)];
                    while (Years2 == Years)
                        Years2 = TimeValues[Random.Range(0, TimeValues.Length)];
                    float Interest2 = Principal * Rate * Years2 / 100f;
                    float Amount2 = Principal + Interest2;
                    QuestionText.text = "A sum of money amounts to Rs. " + Amount + " in " + Years + " years and Rs. " + Amount2 + " in " + Years2 + " years. What was the rate of interest per annum?";
                    Answer = Rate;
                }
                else if (selector == 3)      //check this
                {
                    Principal = 730*Random.Range(1, 21);

                    int startdate = Random.Range(1, 31);
                    int enddate = Random.Range(1, 31);
                    int days = 31 - startdate;
                    days += 181;
                    days += enddate;
                    //Years = days / 365;
                    Interest = (float)(Principal * Rate * days) / (float)36500f;
                    Amount = Interest + Principal;
                    QuestionText.text = "Mr. Blade borrowed Rs. " + Principal + " from a bank on January " + startdate + ". He returns the money on August " + enddate + " of the same year. Calculate the interest he pays at the rate of interest of " + Rate + "% per annum. February has 28 days. (Correct to two decimal places)";
                    float ans = Mathf.Round(Interest * 100) / (float)100;
                    Answer = ans;
                }
                else if (selector == 4)
                {
                    while (Years == 2 || Years == 3 || Years == 4)
                        Years = TimeValues[Random.Range(0, TimeValues.Length)];
                    float Years2 = Years - 2;
                    float Principal2 = Random.Range(1, 10);
                    Principal2 *= 1000;
                    while (Principal2 >= Principal)
                    {
                        Principal2 = Random.Range(1, 10);
                        Principal2 *= 1000;
                    }
                    QuestionText.text = "Mr Prasoon took a loan of Rs. " + Principal + " from a bank, for a period of " + Years + " years. He pays Rs. " + Principal2 + " at the end of " + Years2 + " years and the remaining amount after " + Years + " years. Rate of interest is " + Rate + "% per annum. Find the remaining amount he pays. (Correct to two decimal places)";
                    float Amount2 = (Principal * Rate * Years2) / 100f + Principal;
                    float newPrincipal = Amount2 - Principal2;
                    float tempans = (newPrincipal * Rate * (Years - Years2)) / 100f + newPrincipal;
                    tempans = Mathf.Round(tempans * 100) / (float)100;
                    Answer = tempans;
                }
                else if (selector == 5)
                {
                    float Years2 = Years - 1;
                    QuestionText.text = "Kate takes Rs. " + Principal + " from her friend Nate for a period of " + Years + " years. Kate pays Rs. " + Principal + " at the end of year " + Years2 + " and the remaining amount after year " + Years + ". Nate charges interest at the rate of " + Rate + "% per annum. Find the remaining loan amount at the end of year " + Years2 + ".";
                    Answer = Interest;
                }
            }
            #endregion
            #region level5
            if(level==5)
            {
                answerButton = GeneralButton;
                GeneralButton.gameObject.SetActive(true);

                selector = GetRandomSelector(1, 6);

                //selector = 5;

                if (selector == 1)
                {
                    float Rate2 = RateValues[Random.Range(0, RateValues.Length)];
                    while (Rate == Rate2)
                        Rate2 = RateValues[Random.Range(0, RateValues.Length)];
                    float Interest2 = (Principal * Rate2 * (Years)) / 100f;
                    float diff = Interest2 - Interest;
                    if (diff > 0)
                    {
                        QuestionText.text = "Bank A offers a return of " + Rate + "% per annum, whereas bank B offers a return of " + Rate2 + "% per annum. Mandy invested an equal amount in both the banks. After " + Years + " years he receives Rs. " + diff + " more from bank B than bank A. How much money was invested in each bank?";
                        Answer = Principal;
                    }
                    else
                    {
                        diff *= -1;
                        QuestionText.text = "Bank A offers a return of " + Rate + "% per annum, whereas bank B offers a return of " + Rate2 + "% per annum. Mandy invested an equal amount in both the banks. After " + Years + " years he receives Rs. " + diff + " more from bank A than bank B. How much money was invested in each bank?";
                        Answer = Principal;
                    }
                }
                else if (selector == 2)
                {
                    float Rate2 = RateValues[Random.Range(0, RateValues.Length)];
                    while (Rate == Rate2)
                        Rate2 = RateValues[Random.Range(0, RateValues.Length)];
                    //Rate2 = (ansrand * Rate) / (2 * Principal - 2 * ansrand);
                    QuestionText.text = "Elaine invests a sum of Rs. " + Principal + ". A portion of it is invested in plan A (" + Rate + "% annual return), while the remainder is invested in plan B (" + Rate2 + "% annual return). After 2 years the interest accrued from plan A is twice the interest accrued from plan B. What amount did she invest in plan A.(Correct to two decimal places)";
                    float tempans = ((2 * Rate2 * Principal) / (Rate + 2 * Rate2));
                    tempans = Mathf.Round(tempans * 100) / (float)100;
                    Answer = Mathf.Round(tempans);
                }
                else if (selector == 3)
                {

                    Interest = Rate * Rate * Principal / 100;
                    QuestionText.text = "Jerry took a loan of Rs. " + Principal + " for as many years as the rate of interest per annum. If he paid Rs. " + Interest + " as interest at the end of the loan period, what was the rate of interest per annum?";
                    Answer = Rate;
                }
                else if (selector == 4)             //include decimal in numpad
                {
                    QuestionText.text = "A dishonest financier claims to lend money at simple interest, but he includes the interest every six months to the principal. What is the effective rate of interest if the interest he charges is " + Rate + "% per annum? (Correct to two decimal places)";
                    float tempans = (400 * Rate + Rate * Rate) / (400);
                    Answer = Mathf.Round(tempans * 100) / (float)100;
                }
                else if (selector == 5)
                {
                    float Principal2 = Random.Range(1, 21);
                    Principal2 = Principal2 * 600;
                    while (Principal2 >= Principal)
                    {
                        Principal2 = Random.Range(1, 21);
                        Principal2 = Principal2 * 600;
                    }
                    Interest = Principal * Rate / 100f;
                    Months = Random.Range(2, 10);
                    var Months2 = 12 - Months;
                    Interest+=Principal2 * Rate * Months2 / 600f;
                    QuestionText.text = "A sum of Rs. " + Principal + " is lent in the beginning of a year at a certain rate of interest. After " + Months + " months Rs. " + Principal2 + " more is lent but at twice the previous rate of interest. At the year's end, Rs. " + Interest + " is earned as interest from both the loans. What was the original rate of interest charged per annum?";
                    Answer = Rate;
                }
            }
            #endregion
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
            {   // All Clear
                userAnswerText.text = "";
            }
            else if (value == 12)
            {    // /
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += "/";
            }
            else if (value == 13)
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
