using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
    public class LargeNos : BaseAssessment
    {

        public Text subQuestionText;
        public GameObject MCQ;
        public GameObject ThreeChoice;
        private string Answer;
        private string[] Answerarray;
        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
        private int coeff6;
        private int randact, splitans;
        private int x, y, z, a, b, c;
        private string expression3;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "LNS06", "S01", "A01");

            scorestreaklvls = new int[15];           //check this

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
        bool checkArrayValues(string[] A, string[] B)
        {
            if (A.Length != B.Length)
            {
                CerebroHelper.DebugLog(A.Length);
                CerebroHelper.DebugLog(B.Length);
                CerebroHelper.DebugLog("Length not equal");
                return false;
            }
            var found = true;
            for (var i = 0; i < A.Length; i++)
            {
                if (A[i] != B[i])
                {
                    CerebroHelper.DebugLog(A[i] + "not found1");
                    found = false;
                    break;
                }
                else
                    found = true;

            }
            if (!found)
            {
                CerebroHelper.DebugLog(A[i] + " not found2");
                return false;
            }
            return true;
        }
        bool checkArrayValuesForLevel7(string[] A, string[] B)
        {
            if (A.Length != B.Length)
            {
                CerebroHelper.DebugLog(A.Length);
                CerebroHelper.DebugLog(B.Length);
                CerebroHelper.DebugLog("Length not equal");
                return false;
            }
            for (var i = 0; i < A.Length; i++)
            {
                var found = false;
                for (var j = 0; j < B.Length; j++)
                {
                    if (A[i] == B[j])
                    {
                        CerebroHelper.DebugLog(A[i] + "found");
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    CerebroHelper.DebugLog(A[i] + " not found");
                    return false;
                }
            }
            return true;
        }

        public override void SubmitClick()
        {
            if (ignoreTouches || userAnswerText.text == "")
            {
                return;
            }

            int increment = 0;
            ignoreTouches = true;
            var correct = true;
            CerebroHelper.DebugLog("!" + userAnswerText.text + "!");
            questionsAttempted++;
            updateQuestionsAttempted();
            string userAnswerstring = userAnswerText.text;
            if (level == 3)
            {
                string[] userAnswerSplits = userAnswerText.text.Split(new string[] { "," }, System.StringSplitOptions.None);

                List<string> userAnswers = new List<string>();

                for (var i = 0; i < userAnswerSplits.Length; i++)
                {
                    string userAnswer = "";
                    userAnswer = userAnswerSplits[i];
                    userAnswers.Add(userAnswer);
                }
                //if (level == 7)
                //{
                //    if (checkArrayValuesForLevel7(Answerarray, userAnswers.ToArray()))
                //    {
                //        correct = true;
                //    }
                //    else
                //    {
                //        correct = false;
                //    }
                //}
                //else
                //{
                    if (checkArrayValues(Answerarray, userAnswers.ToArray()))
                    {
                        correct = true;
                    }
                    else
                    {
                        correct = false;
                    }
                //}
            }
            else
            {
                bool checkingMCQ = false;
                bool checkingThreeChoice = false;
                if (MCQ.activeSelf)
                {
                    checkingMCQ = true;
                }
                if (ThreeChoice.activeSelf)
                {
                    checkingThreeChoice = true;
                }
                if (checkingMCQ)
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
                else if (checkingThreeChoice)
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
                    float answer = 0;
                    float userAnswer = 0;
                    bool directCheck = false;
                    if (Answer.Contains("/"))
                    {
                        var correctAnswers = Answer.Split(new string[] { "/" }, System.StringSplitOptions.None);
                        var userAnswers = userAnswerText.text.Split(new string[] { "/" }, System.StringSplitOptions.None);
                        correct = MathFunctions.checkFractions(userAnswers, correctAnswers);
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

                        if (level == 11 || level == 6)
                        {
                            directCheck = true;
                            
                            if(userAnswerstring[0] == '0')
                            {
                                //print("Contains 0 at [0]");
                                userAnswerstring = userAnswerstring.Remove(0, 1);
                                //print(userAnswerstring);
                            }
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
                            if (level == 11 || level == 6)
                            {
                                if (userAnswerstring == Answer)
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
                    }
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
                    increment = 5;
                }
                else if (Queslevel == 3)
                {
                    increment = 5;
                }
                else if (Queslevel == 4)
                {
                    increment = 5;
                }
                else if (Queslevel == 5)
                {
                    increment = 5;
                }
                else if (Queslevel == 6)
                {
                    increment = 5;
                }
                else if (Queslevel == 7)
                {
                    increment = 10;
                }
                else if (Queslevel == 8)
                {
                    increment = 10;
                }
                else if (Queslevel == 9)
                {
                    increment = 10;
                }
                else if (Queslevel == 10)
                {
                    increment = 10;
                }
                else if (Queslevel == 11)
                {
                    increment = 10;
                }
                else if (Queslevel == 12)
                {
                    increment = 15;
                }
                else if (Queslevel == 13)
                {
                    increment = 15;
                }
                else if (Queslevel == 14)
                {
                    increment = 15;
                }
                else if (Queslevel == 15)
                {
                    increment = 15;
                }
                else if (Queslevel == 16)
                {
                    increment = 15;
                }
                UpdateStreak(5, 9);

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
        public void ThreeChoiceOptionClicked(int value)
        {
            if (ignoreTouches)
            {
                return;
            }
            AnimateThreeChoiceOption(value);
            userAnswerText = ThreeChoice.transform.Find("Option" + value.ToString()).Find("Text").GetComponent<Text>();
            answerButton = ThreeChoice.transform.Find("Option" + value.ToString()).GetComponent<Button>();
            SubmitClick();
        }
        IEnumerator AnimateMCQOption(int value)
        {
            var GO = MCQ.transform.Find("Option" + value.ToString()).gameObject;
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1.2f, 1.2f, 1), false));
            yield return new WaitForSeconds(0.2f);
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1, 1, 1), false));
        }
        IEnumerator AnimateThreeChoiceOption(int value)
        {
            var GO = ThreeChoice.transform.Find("Option" + value.ToString()).gameObject;
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1.2f, 1.2f, 1), false));
            yield return new WaitForSeconds(0.2f);
            Go.to(GO.transform, 0.2f, new GoTweenConfig().scale(new Vector3(1, 1, 1), false));
        }
        protected override IEnumerator ShowWrongAnimation()
        {

            userAnswerText.color = MaterialColor.red800;
			Go.to(userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
            yield return new WaitForSeconds(0.5f);
            if (level == 3 || level == 7)
            {
                if (isRevisitedQuestion)
                {
                    userAnswerText.text = "";
                    userAnswerText.color = MaterialColor.textDark;
                    ignoreTouches = false;
                }
                else
                {
                    string str = "";
                    for (int i = 0; i < Answerarray.Length; i++)
                    {
                        if (i == (Answerarray.Length - 1))
                            str += Answerarray[i];
                        else
                            str += Answerarray[i] + ", ";
                    }
                    userAnswerText.text = " " + str + " ";
                    userAnswerText.color = MaterialColor.green800;
                }
            }
            else
            {
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
            GeneralButton.gameObject.SetActive(true);
            MCQ.gameObject.SetActive(false);
            ThreeChoice.gameObject.SetActive(false);
            numPad.SetActive(true);
            if (Queslevel > scorestreaklvls.Length)
            {
                level = UnityEngine.Random.Range(1, scorestreaklvls.Length + 1);
            }
                       
            #region level1
            if (level == 1)
            {

                GeneralButton.gameObject.SetActive(false);
                ThreeChoice.SetActive(true);
                numPad.SetActive(false);
                selector = GetRandomSelector(1, 4);
                if (selector == 1)
                {
                    coeff1 = Random.Range(8000, 10000);
                    expression3 = coeff1 + " is _____ than 10000.";
                    Answer = "<";
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(80000, 100000);
                    expression3 = coeff1 + " is _____ than 100000.";
                    Answer = "<";
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(80000, 1000000);
                    expression3 = coeff1 + " is _____ than 1000000.";
                    Answer = "<";
                }
                QuestionText.text = expression3;
                ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "<";
                ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<Text>().text = ">";
                ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<Text>().text = "=";
            }
            #endregion
            #region level2
            if (level == 2)
            {
                GeneralButton.gameObject.SetActive(false);
                ThreeChoice.SetActive(true);
                numPad.SetActive(false);
                //coeff1 = Random.Range(8000, 10000000);
                selector = GetRandomSelector(1, 4);
                if (selector == 1)
                {
                    coeff1 = Random.Range(10000, 100000);
                    expression3 = coeff1 + " is _____ than 10000.";
                    Answer = ">";
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(100000, 1000000);
                    expression3 = coeff1 + " is _____ than 100000.";
                    Answer = ">";
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(1000000, 10000000);
                    expression3 = coeff1 + " is _____ than 1000000.";
                    Answer = ">";
                }
                QuestionText.text = expression3;
                ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "<";
                ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<Text>().text = ">";
                ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<Text>().text = "=";

            }
            #endregion
            #region level3
            if (level == 3)
            {
                subQuestionText.gameObject.SetActive(true);
                selector = GetRandomSelector(1, 3);
                coeff1 = Random.Range(1000, 10000);
                string[] str1 = new string[4];
                str1[0] = str1[1] = str1[2] = str1[3] = coeff1.ToString();
                string[] randarr = new string[5];
				randarr[0] = Random.Range(1, 10).ToString();
                randarr[1] = Random.Range(1, 10).ToString();
                randarr[2] = Random.Range(1, 10).ToString();
                randarr[3] = Random.Range(1, 10).ToString();
				randarr[4] = Random.Range(1, 10).ToString();

                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        int x = Random.Range(0, 5);
                        str1[j] += randarr[x];
                    }

                }
                subQuestionText.text = str1[0] + ", " + str1[1] + ", " + str1[2] + ", " + str1[3];
                int temp1 = 0;
                int temp2 = 0;
                string tempmax;
                bool a;
                if (selector == 1)
                {
                    QuestionText.text = "Rewrite numbers in ascending order:";
                    for (int i = 0; i < 4; ++i)
                    {
                        for (int j = i + 1; j < 4; ++j)
                        {
                            a = int.TryParse(str1[i], out temp1);
                            a = int.TryParse(str1[j], out temp2);
                            if (temp1 > temp2)
                            {
                                tempmax = str1[i];
                                str1[i] = str1[j];
                                str1[j] = tempmax;
                            }
                        }
                    }
                    for (int i = 0; i < 4; i++)
                        CerebroHelper.DebugLog(str1[i]);
                    Answerarray = str1;
                }
                else if (selector == 2)
                {
                    QuestionText.text = "Rewrite the given numbers in descending order:";
                    for (int i = 0; i < 4; ++i)
                    {
                        for (int j = i + 1; j < 4; ++j)
                        {
                            a = int.TryParse(str1[i], out temp1);
                            a = int.TryParse(str1[j], out temp2);
                            if (temp1 < temp2)
                            {
                                tempmax = str1[i];
                                str1[i] = str1[j];
                                str1[j] = tempmax;
                            }
                        }
                    }
                    for (int i = 0; i < 4; i++)
                        CerebroHelper.DebugLog(str1[i]);
                    Answerarray = str1;
                }
            }
            #endregion
            #region level4
            if (level == 4)
            {
                subQuestionText.gameObject.SetActive(true);
                randact = Random.Range(1, 4);
                selector = GetRandomSelector(1, 3);
                int[] digits = new int[5];
                for (int i = 0; i < 5; i++)
                {
                    digits[i] = Random.Range(1, 9);
                }
                int tempmax;
                if (selector == 1)
                {
                    if (randact == 1)
                    {
                        QuestionText.text = "What is the greatest 5 digit number you can form using these digits? (Repetition of digits is not allowed)";
                        subQuestionText.text = "0,9," + digits[0] + "," + digits[1] + "," + digits[2];
                        for (int i = 0; i < 3; ++i)
                        {
                            for (int j = i + 1; j < 3; ++j)
                            {
                                if (digits[i] < digits[j])
                                {
                                    tempmax = digits[i];
                                    digits[i] = digits[j];
                                    digits[j] = tempmax;
                                }
                            }
                        }
                        Answer = "9" + digits[0] + digits[1] + digits[2] + "0";
                    }
                    else if (randact == 2)
                    {

                        QuestionText.text = "What is the greatest 6 digit number you can form using these digits? (Repetition of digits is not allowed)";
                        subQuestionText.text = "0,9," + digits[0] + "," + digits[1] + "," + digits[2] + "," + digits[3];
                        for (int i = 0; i < 4; ++i)
                        {
                            for (int j = i + 1; j < 4; ++j)
                            {
                                if (digits[i] < digits[j])
                                {
                                    tempmax = digits[i];
                                    digits[i] = digits[j];
                                    digits[j] = tempmax;
                                }
                            }
                        }
                        Answer = "9" + digits[0] + digits[1] + digits[2] + digits[3] + "0";
                    }
                    else if (randact == 3)
                    {
                        QuestionText.text = "What is the greatest 7 digit number you can form using these digits? (Repetition of digits is not allowed)";
                        subQuestionText.text = "0,9," + digits[0] + "," + digits[1] + "," + digits[2] + "," + digits[3] + "," + digits[4];
                        for (int i = 0; i < 5; ++i)
                        {
                            for (int j = i + 1; j < 5; ++j)
                            {
                                if (digits[i] < digits[j])
                                {
                                    tempmax = digits[i];
                                    digits[i] = digits[j];
                                    digits[j] = tempmax;
                                }
                            }
                        }
                        Answer = "9" + digits[0] + digits[1] + digits[2] + digits[3] + digits[4] + "0";
                    }
                }
                else if (selector == 2)
                {
                    if (randact == 1)
                    {
                        QuestionText.text = "What is the smallest 5 digit number you can form using these digits? (Repetition of digits is not allowed)";
                        subQuestionText.text = "0,9," + digits[0] + "," + digits[1] + "," + digits[2];
                        for (int i = 0; i < 3; i++)
                        {
                            tempmax = digits[i];
                            for (int j = i + 1; j < 3; j++)
                            {
                                if (digits[i] > digits[j])
                                {
                                    tempmax = digits[i];
                                    digits[i] = digits[j];
                                    digits[j] = tempmax;
                                }
                            }
                        }
                        Answer = digits[0] + "0" + digits[1] + digits[2] + "9";
                    }
                    else if (randact == 2)
                    {
                        QuestionText.text = "What is the smallest 6 digit number you can form using these digits? (Repetition of digits is not allowed)";
                        subQuestionText.text = "0,9," + digits[0] + "," + digits[1] + "," + digits[2] + "," + digits[3];
                        for (int i = 0; i < 4; i++)
                        {
                            tempmax = digits[i];
                            for (int j = i + 1; j < 4; j++)
                            {
                                if (digits[i] > digits[j])
                                {
                                    tempmax = digits[i];
                                    digits[i] = digits[j];
                                    digits[j] = tempmax;
                                }
                            }
                        }
                        Answer = digits[0] + "0" + digits[1] + digits[2] + digits[3] + "9";
                    }
                    else if (randact == 3)
                    {
                        QuestionText.text = "What is the smallest 7 digit number you can form using these digits? (Repetition of digits is not allowed)";
                        subQuestionText.text = "0,9," + digits[0] + "," + digits[1] + "," + digits[2] + "," + digits[3] + "," + digits[4];
                        for (int i = 0; i < 5; i++)
                        {
                            tempmax = digits[i];
                            for (int j = i + 1; j < 5; j++)
                            {
                                if (digits[i] > digits[j])
                                {
                                    tempmax = digits[i];
                                    digits[i] = digits[j];
                                    digits[j] = tempmax;
                                }
                            }
                        }
                        Answer = digits[0] + "0" + digits[1] + digits[2] + digits[3] + digits[4] + "9";
                    }
                }
            }
            #endregion
            #region level5
            if (level == 5)
            {
                selector = GetRandomSelector(1, 2);
                if (selector == 1)
                {
                    //subQuestionText.text = "";
                    coeff1 = Random.Range(1, 10);
                    coeff2 = Random.Range(1, 10);
                    coeff3 = Random.Range(1, 10);
                    coeff4 = Random.Range(1, 10);
                    coeff5 = Random.Range(1, 10);
                    QuestionText.text = "Write a number which has " + coeff1 + " in ten thousands place, " + coeff2 + " in the thousands place, " + coeff3 + " in hundreds place, " + coeff4 + " in tens place and " + coeff5 + " in units place."; ;
                    Answer = coeff1.ToString() + coeff2 + coeff3 + coeff4 + coeff5;
                }
            }
            #endregion
            #region level6
            if (level == 6)
            {
                subQuestionText.gameObject.SetActive(true);
                QuestionText.text = "Write the numeral using commas (Indian place value system):";
                string[] tensarray = new string[10];
                tensarray[1] = "twenty";
                tensarray[2] = "thirty";
                tensarray[3] = "forty";
                tensarray[4] = "fifty";
                tensarray[5] = "sixty";
                tensarray[6] = "seventy";
                tensarray[7] = "eighty";
                tensarray[8] = "ninety";
                string[] unitsarray = new string[10];
                unitsarray[1] = "one";
                unitsarray[2] = "two";
                unitsarray[3] = "three";
                unitsarray[4] = "four";
                unitsarray[5] = "five";
                unitsarray[6] = "six";
                unitsarray[7] = "seven";
                unitsarray[8] = "eight";
                unitsarray[9] = "nine";
                string[] unitsarray2 = new string[10];
                unitsarray2[1] = "One";
                unitsarray2[2] = "Two";
                unitsarray2[3] = "Three";
                unitsarray2[4] = "Four";
                unitsarray2[5] = "Five";
                unitsarray2[6] = "Six";
                unitsarray2[7] = "Seven";
                unitsarray2[8] = "Eight";
                unitsarray2[9] = "Nine";
                coeff1 = Random.Range(1, 9);
                coeff2 = Random.Range(1, 10);
                coeff3 = Random.Range(1, 10);
                coeff4 = Random.Range(1, 9);
                selector = GetRandomSelector(1, 4);
                if (selector == 1)
                {
                    subQuestionText.text = unitsarray2[coeff2] + " crores " + unitsarray[coeff3] + " hundred";
                    Answer = coeff2 + ",00,00," + coeff3 + "00";
                }
                else if (selector == 2)
                {
                    subQuestionText.text = unitsarray2[coeff2] + " hundred and " + unitsarray[coeff3] + " crore";
                    Answer = coeff2 + ",0" + coeff3 + ",00,00,000";
                }
                else if (selector == 3)
                {
                    subQuestionText.text = unitsarray2[coeff2] + " lakhs and " + tensarray[coeff1];
                    Answer = coeff2 + ",00,0" + (coeff1 + 1) + "0";
                }
            }
            #endregion
            #region level7
            if (level == 7)
            {
                subQuestionText.text = "";
                selector = GetRandomSelector(1, 2);
                if (selector == 1)
                {
                    coeff1 = Random.Range(1, 10);
                    int[] str1 = new int[7];
                    for (int i = 0; i < 7; i++)
                    {
                        str1[i] = Random.Range(1, 10);
                        while (str1[i] == coeff1)
                            str1[i] = Random.Range(1, 10);
                    }
                    int pos1 = Random.Range(0, 7);
                    int pos2 = Random.Range(0, 7);
                    while (pos1 == pos2)
                        pos2 = Random.Range(0, 7);
                    str1[pos2] = str1[pos1] = coeff1;
                    QuestionText.text = "Find the difference in place values of the two " + coeff1 + "'s in " + str1[6] + +str1[5] + +str1[4] + +str1[3] + +str1[2] + +str1[1] + +str1[0];
                    int[] ans = new int[2];
                    if (pos1 == 6)
                        ans[0] = coeff1 * 1000000;
                    if (pos1 == 5)
                        ans[0] = coeff1 * 100000;
                    if (pos1 == 4)
                        ans[0] = coeff1 * 10000;
                    if (pos1 == 3)
                        ans[0] = coeff1 * 1000;
                    if (pos1 == 2)
                        ans[0] = coeff1 * 100;
                    if (pos1 == 1)
                        ans[0] = coeff1 * 10;
                    if (pos1 == 0)
                        ans[0] = coeff1;
                    if (pos2 == 6)
                        ans[1] = coeff1 * 1000000;
                    if (pos2 == 5)
                        ans[1] = coeff1 * 100000;
                    if (pos2 == 4)
                        ans[1] = coeff1 * 10000;
                    if (pos2 == 3)
                        ans[1] = coeff1 * 1000;
                    if (pos2 == 2)
                        ans[1] = coeff1 * 100;
                    if (pos2 == 1)
                        ans[1] = coeff1 * 10;
                    if (pos2 == 0)
                        ans[1] = coeff1;
                    CerebroHelper.DebugLog(ans[0]);
                    CerebroHelper.DebugLog(ans[1]);
                    //Answerarray = ans;
                    if (ans[0] >= ans[1])
                        Answer = (ans[0] - ans[1]).ToString();
                    else
                        Answer = (ans[1] - ans[0]).ToString();
                }
            }
            #endregion
            #region level8
            if (level == 8)
            {
                subQuestionText.gameObject.SetActive(true);
                selector = GetRandomSelector(1, 2);
                if (selector == 1)
                {
                    GeneralButton.gameObject.SetActive(false);
                    ThreeChoice.SetActive(true);
                    numPad.SetActive(false);
                    QuestionText.text = ">, < or = ?";
                    coeff1 = Random.Range(10, 100);
                    coeff2 = Random.Range(10, 100);
                    coeff3 = Random.Range(100, 1000);
                    int units, tens;
                    units = coeff2 % 10;
                    tens = coeff2 / 10;
                    subQuestionText.text = coeff1 + "," + coeff2 + "," + coeff3 + " ______ " + coeff1 + "," + units + tens + "," + coeff3;
                    if (units > tens)
                        Answer = "<";
                    else if (units < tens)
                        Answer = ">";
                    else
                        Answer = "=";
                    ThreeChoice.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "<";
                    ThreeChoice.transform.Find("Option2").Find("Text").GetComponent<Text>().text = ">";
                    ThreeChoice.transform.Find("Option3").Find("Text").GetComponent<Text>().text = "=";
                }
            }
            #endregion
            #region level9
            if (level == 9)
            {
                selector = GetRandomSelector(1, 5);
                randact = Random.Range(1, 3);
                if (selector == 1)
                {
                    coeff1 = Random.Range(1000000, 10000000);
                    if (randact == 1)
                    {
                        QuestionText.text = "How much is 1 lakh less than " + coeff1 + "?";
                        int tempans;
                        tempans = coeff1 - 100000;
                        Answer = tempans.ToString();
                    }
                    else if (randact == 2)
                    {
                        QuestionText.text = "How much is 1 lakh more than " + coeff1 + "?";
                        int tempans;
                        tempans = coeff1 + 100000;
                        Answer = tempans.ToString();
                    }
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(1000000, 10000000);
                    if (randact == 1)
                    {
                        QuestionText.text = "How much is 10 lakhs less than " + coeff1 + "?";
                        int tempans;
                        tempans = coeff1 - 1000000;
                        Answer = tempans.ToString();
                    }
                    else if (randact == 2)
                    {
                        QuestionText.text = "How much is 10 lakhs more than " + coeff1 + "?";
                        int tempans;
                        tempans = coeff1 + 1000000;
                        Answer = tempans.ToString();
                    }
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range(10000000, 100000000);
                    if (randact == 1)
                    {
                        QuestionText.text = "How much is 1 crore less than " + coeff1 + "?";
                        int tempans;
                        tempans = coeff1 - 10000000;
                        Answer = tempans.ToString();
                    }
                    else if (randact == 2)
                    {
                        QuestionText.text = "How much is 1 crore more than " + coeff1 + "?";
                        int tempans;
                        tempans = coeff1 + 10000000;
                        Answer = tempans.ToString();
                    }
                }
                else if (selector == 4)
                {
                    coeff1 = Random.Range(100000000, 1000000000);
                    if (randact == 1)
                    {
                        QuestionText.text = "How much is 10 crores less than " + coeff1 + "?";
                        int tempans;
                        tempans = coeff1 - 100000000;
                        Answer = tempans.ToString();
                    }
                    else if (randact == 2)
                    {
                        QuestionText.text = "How much is 10 crores more than " + coeff1 + "?";
                        int tempans;
                        tempans = coeff1 + 100000000;
                        Answer = tempans.ToString();
                    }
                }
            }
            #endregion
            #region level10
            if (level == 10)
            {
                subQuestionText.gameObject.SetActive(true);
                QuestionText.text = "Complete the pattern: ";
                selector = GetRandomSelector(1, 3);
                if (selector == 1)
                {
                    coeff1 = Random.Range(10, 100);
                    coeff2 = Random.Range(10, 100);
                    coeff3 = Random.Range(100, 1000);
                    subQuestionText.text = coeff1 + "," + coeff2 + "," + coeff3 + "  ;  " + (coeff1 + 1).ToString() + "," + coeff2 + "," + coeff3 + "  ;  " + (coeff1 + 2).ToString() + "," + coeff2 + "," + coeff3 + "  ; ________";
                    Answer = (coeff1 + 3).ToString() + coeff2 + coeff3;
                }
                else if (selector == 2)
                {
                    coeff1 = Random.Range(1, 10);
                    coeff2 = Random.Range(10, 100);
                    coeff3 = Random.Range(100, 1000);
                    subQuestionText.text = coeff1 + "," + coeff2 + "," + coeff3 + "  ;  " + coeff1 + "," + (coeff2 + 1).ToString() + "," + coeff3 + "  ;  " + coeff1 + "," + (coeff2 + 2).ToString() + "," + coeff3 + "  ; ________";
                    Answer = coeff1 + (coeff2 + 3).ToString() + coeff3;
                }
            }
            #endregion
            #region level11
            if (level == 11)
            {
                subQuestionText.gameObject.SetActive(true);
                string[] tensarray = new string[10];
                tensarray[1] = "twenty";
                tensarray[2] = "thirty";
                tensarray[3] = "forty";
                tensarray[4] = "fifty";
                tensarray[5] = "sixty";
                tensarray[6] = "seventy";
                tensarray[7] = "eighty";
                tensarray[8] = "ninety";
                string[] unitsarray = new string[10];
                unitsarray[1] = "one";
                unitsarray[2] = "two";
                unitsarray[3] = "three";
                unitsarray[4] = "four";
                unitsarray[5] = "five";
                unitsarray[6] = "six";
                unitsarray[7] = "seven";
                unitsarray[8] = "eight";
                unitsarray[9] = "nine";
                coeff1 = Random.Range(1, 9);
                coeff2 = Random.Range(1, 10);
                coeff3 = Random.Range(1, 10);
                coeff4 = Random.Range(1, 9);
                CerebroHelper.DebugLog(coeff1 + coeff2);
                string tens, units;
                selector = GetRandomSelector(1, 2);
                if (selector == 1)
                {
                    QuestionText.text = "Write the following in standard international form with commas:";
                    subQuestionText.text = tensarray[coeff1] + " " + unitsarray[coeff2] + " million " + unitsarray[coeff3] + " thousand and " + tensarray[coeff4];
                    Answer = (coeff1 + 1).ToString() + coeff2 + ",00" + coeff3 + ",0" + (coeff4 + 1) + "0";
                }
            }
            #endregion
            #region level12
            if (level == 12)
            {
                subQuestionText.gameObject.SetActive(true);
                coeff1 = Random.Range(1, 1000);
                selector = GetRandomSelector(1, 3);
                string[] roman = new string[1000];
                roman = numtoroman(coeff1);
                string roman2;
                roman2 = "";
                for (int k = 0; k < roman.Length; k++)
                    roman2 += roman[k];
                if (selector == 1)
                {
                    QuestionText.text = "Write the Hindu-Arabic numeral of the following:";
                    subQuestionText.text = roman2;
                    Answer = coeff1.ToString();
                }
                else if (selector == 2)
                {
                    QuestionText.text = "Write the Roman numeral of the following number:";
                    subQuestionText.text = coeff1.ToString();
                    Answer = roman2;
                }
            }
            #endregion
            #region level13
            if (level == 13)
            {
                subQuestionText.gameObject.SetActive(true);
                selector = GetRandomSelector(1, 5);
                if (selector == 1)
                {
                    QuestionText.text = "Round the following number to nearest 10:";
                    coeff1 = Random.Range(10, 100);
                    subQuestionText.text = coeff1.ToString();
                    int unit;
                    int ten;
                    ten = coeff1 / 10;
                    unit = coeff1 % 10;
                    if (unit >= 5)
                    {
                        ten += 1;
                        Answer = ten.ToString() + "0";
                    }
                    else if (unit < 5)
                    {
                        Answer = ten.ToString() + "0";
                    }
                }
                else if (selector == 2)
                {
                    QuestionText.text = "Round the following number to nearest 100:";
                    coeff1 = Random.Range(100, 1000);
                    subQuestionText.text = coeff1.ToString();
                    int hundred = coeff1 / 100;
                    int x1, x2;
                    x1 = hundred * 100;
                    x2 = (hundred + 1) * 100;
                    if (coeff1 >= (x1 + 50))
                        Answer = x2.ToString();
                    else
                        Answer = x1.ToString();
                }
                else if (selector == 3)
                {
                    QuestionText.text = "Round the following number to nearest 1000:";
                    coeff1 = Random.Range(1000, 10000);
                    subQuestionText.text = coeff1.ToString();
                    int thousand = coeff1 / 1000;
                    int x1, x2;
                    x1 = thousand * 1000;
                    x2 = (thousand + 1) * 1000;
                    if (coeff1 >= (x1 + 500))
                        Answer = x2.ToString();
                    else
                        Answer = x1.ToString();
                }
                else if (selector == 4)
                {
                    QuestionText.text = "Round the following number to nearest 10000:";
                    coeff1 = Random.Range(10000, 100000);
                    subQuestionText.text = coeff1.ToString();
                    int thousand = coeff1 / 10000;
                    int x1, x2;
                    x1 = thousand * 10000;
                    x2 = (thousand + 1) * 10000;
                    if (coeff1 >= (x1 + 5000))
                        Answer = x2.ToString();
                    else
                        Answer = x1.ToString();
                }
            }
            #endregion
            #region level14
            if (level == 14)
            {
                coeff1 = Random.Range(1, 10);
                string x = coeff1.ToString() + "5";
                selector = GetRandomSelector(1, 3);
                if (selector == 1)
                {
                    QuestionText.text = "An adopted puppy was supposed to be around " + x + " months. What could be the maximum age of the puppy in months?";
                    Answer = (coeff1 + 1).ToString() + "0";
                }
                else if (selector == 2)
                {
                    QuestionText.text = "An adopted puppy was supposed to be around " + x + " months. What could be the minimum age of the puppy in months?";
                    Answer = (coeff1).ToString() + "0";
                }
            }
            #endregion
            #region level15
            if (level == 15)
            {
                subQuestionText.gameObject.SetActive(true);
                selector = GetRandomSelector(1, 5);
                GeneralButton.gameObject.SetActive(false);
                MCQ.SetActive(true);
                numPad.SetActive(false);
                QuestionText.text = "Exact or Rounded figure?";
                if (selector == 1)
                {
                    subQuestionText.text = "Shoe size";
                    Answer = "Rounded";
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "Exact";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = "Rounded";
                }
                else if (selector == 2)
                {
                    subQuestionText.text = "Phone Number";
                    Answer = "Exact";
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "Exact";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = "Rounded";
                }
                else if (selector == 3)
                {
                    subQuestionText.text = "Number of people on a plane";
                    Answer = "Exact";
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "Exact";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = "Rounded";
                }
                else if (selector == 4)
                {
                    subQuestionText.text = "People at a fair";
                    Answer = "Rounded";
                    MCQ.transform.Find("Option1").Find("Text").GetComponent<Text>().text = "Exact";
                    MCQ.transform.Find("Option2").Find("Text").GetComponent<Text>().text = "Rounded";
                }
            }
            #endregion
            userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
            userAnswerText.text = "";
        }

        string[] romanval = new string[1000];
        private int i = 0;
        void predigit(string num1, string num2)
        {
            romanval[i++] = num1;
            romanval[i++] = num2;
        }
        void postdigit(string c, int n)
        {
            int j;
            for (j = 0; j < n; j++)
                romanval[i++] = c;
        }
        string[] numtoroman(int number)
        {
            int j;
            romanval = new string[1000];
            while (number != 0)
            {
                if (number >= 1000)
                {
                    postdigit("M", number / 1000);
                    number = number - (number / 1000) * 1000;
                }
                else if (number >= 500)
                {
                    if (number < (500 + 4 * 100))
                    {
                        postdigit("D", number / 500);
                        number = number - (number / 500) * 500;
                    }
                    else
                    {
                        predigit("C", "M");
                        number = number - (1000 - 100);
                    }
                }
                else if (number >= 100)
                {
                    if (number < (100 + 3 * 100))
                    {
                        postdigit("C", number / 100);
                        number = number - (number / 100) * 100;
                    }
                    else
                    {
                        predigit("L", "D");
                        number = number - (500 - 100);
                    }
                }
                else if (number >= 50)
                {
                    if (number < (50 + 4 * 10))
                    {
                        postdigit("L", number / 50);
                        number = number - (number / 50) * 50;
                    }
                    else
                    {
                        predigit("X", "C");
                        number = number - (100 - 10);
                    }
                }
                else if (number >= 10)
                {
                    if (number < (10 + 3 * 10))
                    {
                        postdigit("X", number / 10);
                        number = number - (number / 10) * 10;
                    }
                    else
                    {
                        predigit("X", "L");
                        number = number - (50 - 10);
                    }
                }
                else if (number >= 5)
                {
                    if (number < (5 + 4 * 1))
                    {
                        postdigit("V", number / 5);
                        number = number - (number / 5) * 5;
                    }
                    else
                    {
                        predigit("I", "X");
                        number = number - (10 - 1);
                    }
                }
                else if (number >= 1)
                {
                    if (number < 4)
                    {
                        postdigit("I", number / 1);
                        number = number - (number / 1) * 1;
                    }
                    else
                    {
                        predigit("I", "V");
                        number = number - (5 - 1);
                    }
                }
            }
            return romanval;
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
            {    //,
                if (checkLastTextFor(new string[1] { "," }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += ",";
            }
            else if (value == 11)
            {   // DEL
                if (userAnswerText.text.Length > 0)
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
            }
            else if (value == 12)
            {    //I
                userAnswerText.text += "I";
            }
            else if (value == 13)
            {    //X
                userAnswerText.text += "X";
            }
            else if (value == 14)
            {    //V
                userAnswerText.text += "V";
            }
            else if (value == 15)
            {    //C
                userAnswerText.text += "C";
            }
            else if (value == 16)
            {    //L
                userAnswerText.text += "L";
            }
            else if (value == 17)
            {    //M
                userAnswerText.text += "M";
            }
            else if (value == 18)
            {    //D
                userAnswerText.text += "D";
            }
        }
    }
}

