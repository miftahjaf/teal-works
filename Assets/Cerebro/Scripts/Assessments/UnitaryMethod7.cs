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
                else if (Queslevel == 7)
                {
                    increment = 15;
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
				            
            #region level1
            if (level == 1)
            {
                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
					coeff1 = Random.Range(6, 21);
                    coeff2 = Random.Range(6, 21);

                    while (coeff1 == coeff2)
						coeff2 = Random.Range(6, 21);
					
					coeff3 = coeff1 * Random.Range(6, 21);

                    QuestionText.text = "The cost of "+coeff1+" apples is Rs. "+coeff3+". What is the cost of "+coeff2+" apples?";
					int ans = (coeff3 * coeff2) / coeff1;
					Answer = ans.ToString();
                }
                else if (selector == 2)
                {
					coeff3 = Random.Range (2, 10);
					coeff2 = Random.Range (1, coeff3);
					coeff2 /= MathFunctions.GetHCF (coeff2, coeff3);
					coeff1 = coeff3 * Random.Range (5, 21);

					QuestionText.text = "A tap can fill half of a container in " + coeff1 + " minutes. How long (in minutes) will it take to fill " + coeff2 + "/" + coeff3 + " of the container?";
					int ans = (2 * coeff1 * coeff2) / coeff3;
					Answer = ans.ToString();
                }
                else if (selector == 3)
                {
					do {
	                    coeff1 = Random.Range(5, 10);
	                    coeff2 = Random.Range(10, 20);
						coeff3 = coeff2 * Random.Range(2, 10);
					} while (coeff1 == coeff2);

                    QuestionText.text = coeff1 + " workers can do a piece of work in " + coeff3 + " days. In how many days can " + coeff2 + " workers do the same work?";
					int ans = (coeff1 * coeff3) / coeff2;
					Answer = ans.ToString();
                }
                else if (selector == 4)
                {
					coeff3 = Random.Range (2, 10);
					coeff2 = Random.Range (1, coeff3);
					coeff2 /= MathFunctions.GetHCF (coeff2, coeff3);
					do {
						coeff5 = Random.Range (2, 10);
						coeff4 = Random.Range (1, coeff5);
					} while (coeff3 == coeff5);

					coeff4 /= MathFunctions.GetHCF (coeff4, coeff5);
					coeff1 = coeff2 * coeff5 * Random.Range (10, 50);

					QuestionText.text = "If " + coeff2 + "/" + coeff3 + " of Maeby's pocket money is Rs. " + coeff1 + ". What is " + coeff4 + "/" + coeff5 + " of her pocket money?";
					int ans = (coeff4 * coeff1 * coeff3) / (coeff2 * coeff5);
					Answer = ans.ToString();
                }
                else if (selector == 5)
                {
					coeff3 = Random.Range (2, 10);  //commmon ratio
					coeff1 = coeff3 * Random.Range (2, 10);
					coeff2 = coeff3 * Random.Range (1, coeff1 / coeff3);
                    
					QuestionText.text = "Lucille can complete a task in " + coeff1 + " days. What part of the task can she complete in \n" + coeff2 + " days? (Answer in fraction)";
					hcf = MathFunctions.GetHCF (coeff1, coeff2);
					Answer = (coeff2 / hcf) + "/" + (coeff1 / hcf);
                }
                
            }
            #endregion
            #region level2
            else if (level == 2)
            {
                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
					coeff2 = Random.Range (3, 10);
					coeff1 = coeff2 * Random.Range (25, 70);

                    QuestionText.text = "A scooter covers " + coeff1 + " km in " + coeff2 + " hrs. Find the average speed (in km/hr) of the \nscooter.";
					Answer = (coeff1 / coeff2).ToString();
                }
                else if (selector == 2)
                {
					do {
						coeff1 = Random.Range (10, 100);
						coeff2 = Random.Range (10, 100);
					} while (coeff1 == coeff2 || (24 * coeff2) % coeff1 != 0);

                    QuestionText.text = "A watch loses " + coeff1 + " seconds everyday (24 hrs). In how many hours will it lose " + coeff2 + " seconds?";
					Answer = ((24 * coeff2) / coeff1).ToString ();
                }
                else if (selector == 3)
                {
                    coeff1 = Random.Range (10, 50);
					coeff3 = Random.Range (coeff1 / 3, coeff1 / 2);
					coeff2 = (coeff1 - coeff3) * Random.Range (3, 10);

                    QuestionText.text = coeff1 + " students can decorate their classroom in " + coeff2 + " days. But " + coeff3 + " students drop out of the decorating job. How long will the remaining students take to decorate the room?";
					hcf = MathFunctions.GetHCF(num, den);
					Answer = ((coeff1 * coeff2) / (coeff1 - coeff3)).ToString ();
                }
                else if (selector == 4)
                {
					int coeff7;
					do {
						coeff1 = Random.Range (2, 10);
						coeff2 = Random.Range (2, 10);
						coeff3 = Random.Range (2, 10);
						coeff5 = Random.Range (2, 10);
						coeff6 = Random.Range (2, 10);
						coeff7 = Random.Range (2, 10);
						coeff4 = MathFunctions.GetLCM (coeff1, coeff2, coeff3) * Random.Range (2, 10);
					} while (coeff1 == coeff5 || coeff2 == coeff6 || coeff3 == coeff7 || coeff1 == coeff2 || (coeff4 * coeff7 * (coeff2 * coeff5 + coeff1 * coeff6)) % (coeff1 * coeff2 * coeff3) != 0);


                    QuestionText.text = coeff1 + " men or " + coeff2 + " women earn Rs. " + coeff4 + " in " + coeff3 + " days. How much will " + coeff5 + " men and " + coeff6 + " women earn in " + coeff7 + " days?";
					int ans = (coeff4 * coeff7 * (coeff2 * coeff5 + coeff1 * coeff6)) / (coeff1 * coeff2 * coeff3);
					Answer = ans.ToString ();
                }
                else if (selector == 5)
                {
					coeff1 = Random.Range (10, 21);
					coeff4 = Random.Range (coeff1 / 4, coeff1 / 2);
					coeff2 = Random.Range (5, 11);
					coeff3 = (coeff1 - coeff4) * Random.Range (2, coeff2 - 2);
					coeff2 *= coeff1 - coeff4;

                    QuestionText.text = "In a stable there is enough food for " + coeff1 + " horses to last " + coeff2 + " days. After " + coeff3 + " days, " + coeff4 + " horses are sold. How many more days will the food last for the rest of the horses?";
					int ans = ((coeff2 - coeff3) * coeff1) / (coeff1 - coeff4);
					Answer = ans.ToString ();
                }
            }
            #endregion
            #region level3
            else if (level == 3)
            {
                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
					do { 
						coeff1 = Random.Range (2, 100);
						coeff2 = Random.Range (2, 100);
					} while (coeff1 == coeff2 || (coeff1 * coeff2) % (coeff1 + coeff2) != 0);

					coeff3 = coeff1 * coeff2 / (coeff1 + coeff2);

                    QuestionText.text = "Bran can complete a task in " + coeff1 + " days, Bronn can do the same task in " + coeff2 + " days. How many days will they take working together?";
					Answer = coeff3.ToString ();
                }
                else if (selector == 2)
                {
					do { 
						coeff1 = Random.Range (2, 100);
						coeff2 = Random.Range (2, 100);
					} while (coeff1 == coeff2 || (coeff1 * coeff2) % (coeff1 + coeff2) != 0);

					coeff3 = coeff1 * coeff2 / (coeff1 + coeff2);

                    QuestionText.text = "Batla and Laxmi can do a job in " + coeff3 + " days working together. Batla can do it alone in " + coeff1 + " days. How much time will Laxmi take to do the job alone?";
					Answer = coeff2.ToString ();
                }
                else if (selector == 3)
                {
					do {
						coeff1 = Random.Range (2, 20);
						coeff2 = Random.Range (2, 20);
						coeff4 = Random.Range (2, 20);
						coeff5 = Random.Range (2, 20);
						coeff3 = Random.Range (2, 20);
					} while (coeff1 == coeff4 || coeff2 == coeff5 || (coeff3 * coeff1 * coeff5) % (coeff4 * coeff2) != 0);

                    QuestionText.text = coeff1 + " workers can construct a " + coeff2 + " m drain in " + coeff3 + " days. How long will " + coeff4 + " workers take to construct a " + coeff5 + " m drain?";
					int ans = coeff3 * coeff1 * coeff5 / (coeff4 * coeff2);
					Answer = ans.ToString ();                    
                }
                else if (selector == 4)
                {
					do {
						coeff1 = Random.Range (2, 20);
						coeff2 = Random.Range (2, 20);
						coeff3 = Random.Range (2, 20);
					} while ((coeff3 * coeff2) % coeff1 != 0 || coeff1 == coeff2 || coeff1 == coeff3);

                    QuestionText.text = "A motorbike takes " + coeff1 + " hrs to cover " + coeff2 + " km. How long (in hours) will it take to cover \n" + coeff3 + " km?";
					int ans = (coeff2 * coeff3) / coeff1;
					Answer = ans.ToString (); 
                }
                else if (selector == 5)
                {
					do {
						coeff1 = Random.Range (30, 80);
						coeff2 = Random.Range (30, 80);
						coeff3 = Random.Range (2, 10);
						coeff4 = Random.Range (2, 10);
					} while ((coeff1 * coeff3 + coeff2 * coeff4) % (coeff3 + coeff4) != 0 || coeff1 == coeff2 || coeff3 == coeff4);

					QuestionText.text = "A car travelling at a speed of " + coeff1 + " km/hr covers a distance of " + (coeff1 * coeff3) + " km and takes another " + coeff4 + " hrs to cover the next " + (coeff2 * coeff4) + " km. Find the average speed (km/hr) for the whole journey.";
					int ans = (coeff1 * coeff3 + coeff2 * coeff4) / (coeff3 + coeff4);
                    Answer = ans.ToString();
                }
            }
            #endregion
            #region level4
            else if (level == 4)
            {
                selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {
					do {
						coeff1 = Random.Range (20, 51);
						coeff2 = coeff1 + MathFunctions.GenerateRandomIntegerExcluding0 (-10, 11);
						coeff3 = Random.Range (3, 21);
					} while ((coeff3 * (coeff1 + coeff2)) % 60 != 0);

					QuestionText.text = "Frodo takes " + coeff1 + " mins to go to school but requires " + coeff2 + " mins for the return journey. If the school is " + ((coeff3 * (coeff1 + coeff2)) / 60) + " km away. Find his average speed (in km/hr).";
					Answer = coeff3.ToString();
                }
                else if (selector == 2)
                {
					do {
						coeff1 = Random.Range (30, 150); 
						coeff2 = Random.Range (5, 30);
					} while ((5 * coeff1 * coeff2) % 18 != 0); 

					QuestionText.text = "Speed of a train is " + coeff1 + " km/hr. It took " + coeff2 + " seconds to cross a signal post completely. Find the length of the train (in m).";
					Answer = ((5 * coeff1 * coeff2) / 18).ToString();
				}
                else if (selector == 3)
                {
					do {
						coeff1 = Random.Range (200, 1000);
						coeff2 = 10 * Random.Range (100, 500);
						coeff3 = Random.Range (30, 150);
					} while ((coeff1 + coeff2) % coeff3 != 0);

					QuestionText.text = "How long (in seconds) will a " + coeff1 + " m long train take to cross a " + coeff2 + " m long bridge if it is travelling at a speed of " + coeff3 + " m/s?";
					Answer = ((coeff1 + coeff2) / coeff3).ToString ();
                }
                else if (selector == 4)
                {
					do {
						coeff1 = Random.Range (6, 20);
						coeff2 = Random.Range (30, 60);
						coeff3 = coeff2 + Random.Range (5, coeff2 / 2 + 1);
					} while ((coeff1 * coeff2) % coeff3 != 0);

					QuestionText.text = "A car takes " + coeff1 + " hours to complete a journey travelling at " + coeff2 + " km/hr. What should be the speed (in km/hr) of the car if you wish to complete the journey in " + ((coeff1 * coeff2) / coeff3) + " hours?";
					Answer = coeff3.ToString ();
                }
                else if (selector == 5)
                {
					do {
						coeff1 = Random.Range (50, 200);
						coeff2 = Random.Range (2, 10);
						coeff3 = Random.Range (coeff2 + 2, coeff2 + 11);
					} while ((coeff1 * coeff2 * coeff3) % 12 != 0);

                    QuestionText.text = "Apples are sold at the rate of Rs. " + coeff1 + " per dozen. The weight of " + coeff2 + " apples is 1 kg. Find the cost (in Rs.) of " + coeff3 + " kg apples.";
					Answer = ((coeff1 * coeff2 * coeff3) / 12).ToString ();
                }
            }
            #endregion
            #region level5
            else if (level == 5)
            {
                selector = GetRandomSelector(1, 5);

                if (selector == 1)
                {
					do {	
						coeff1 = Random.Range (30, 60);
						coeff2 = coeff1 + Random.Range (5, coeff1 / 2 + 1);	
						coeff3 = Random.Range (3, 11);
					} while ((coeff1 * coeff3) % (coeff1 + coeff2) != 0);

					QuestionText.text = "A honda scooter takes " + coeff3 + " hrs to cover " + (coeff1 * coeff3) + " km distance. If its average speed is increased by " + coeff2 + " km/h, calculate the amount of time (in hrs) saved to cover the same distance.";
					Answer = (coeff3 - (coeff1 * coeff3) / (coeff1 + coeff2)).ToString ();
                }
                else if (selector == 2)
                {
					do {
						coeff1 = Random.Range (2, 20); 
						coeff2 = Random.Range (2, 20); 
						coeff3 = Random.Range (2, 20); 
						coeff4 = Random.Range (2, 20); 
						coeff5 = Random.Range (2, 20); 
					} while (coeff1 == coeff4 || coeff2 == coeff5 || (coeff3 * coeff4 * coeff5) % (coeff1 * coeff2) != 0);

                    QuestionText.text = coeff1 + " tailors can stitch " + coeff3 + " dresses in " + coeff2 + " days. How many dresses can be stitched by " + coeff4 + " tailors in " + coeff5 + " days?";
					Answer = ((coeff3 * coeff4 * coeff5) / (coeff1 * coeff2)).ToString ();
                }
                else if (selector == 3)
                {
					do {
						coeff1 = Random.Range (7, 50);
						coeff2 = Random.Range (coeff1 / 2, coeff1);
						coeff4 = (coeff1 * coeff2) / (coeff1 + coeff2);
						coeff3 = Random.Range (2, coeff4);
					} while ((coeff2 * coeff3) % coeff1 != 0);

                    QuestionText.text = "Albert can do a work in " + coeff1 + " days. Pinto can do the same work in " + coeff2 + " days. They start working together but Albert leaves the work after " + coeff3 + " days. How many more days will Pinto take to complete the remaining work?";
					int ans = coeff2 - coeff3 - (coeff2 * coeff3) / coeff1;
					Answer = ans.ToString ();
                }
                else if (selector == 4)
                {
					do {
						coeff1 = Random.Range (2, 10);
						coeff2 = Random.Range (coeff1 + 1, 20);
					} while ((coeff1 * coeff2) % (coeff2 - coeff1) != 0);

                    QuestionText.text = "A swimming pool has two pumps A and B. Pump A can fill the empty pool in " + coeff1 + " hours whereas pump B can empty the pool in " + coeff2 + " hours. The pool incharge mistakenly switched on both the pumps. In how many hours will the empty pool be filled?";
					int ans = (coeff1 * coeff2) / (coeff2 - coeff1);
					Answer = ans.ToString ();
                }
            }
            #endregion
            #region level6
            else if (level == 6)
            {
                selector = GetRandomSelector(1, 5);

                if (selector == 1)
                {
					do {
						coeff1 = Random.Range (2, 20); 
						coeff2 = Random.Range (2, 20); 
						coeff3 = Random.Range (2, 20);
						coeff4 = Random.Range (2, 20); 
						coeff5 = Random.Range (2, 20); 
					} while (coeff1 == coeff4 || coeff2 == coeff5 || (coeff3 * coeff4 * coeff5) % (coeff1 * coeff2) != 0);

                    QuestionText.text = coeff1 + " workers can construct a " + coeff3 + " m drain in " + coeff2 + " days. How long a drain can be constructed by " + coeff4 + " workers in " + coeff5 + " days?";
					Answer = ((coeff3 * coeff4 * coeff5) / (coeff1 * coeff2)).ToString ();
                }
                else if (selector == 2)
                {
					coeff1 = Random.Range (50, 200);
					do {
						coeff2 = Random.Range (10, 50);
						coeff3 = 10 * Random.Range (40, 100);
						coeff4 = Random.Range (2  * coeff3 / 5, 4 * coeff3 / 5);
					} while ((coeff2 * coeff3) % coeff4 != 0);

                    QuestionText.text = "A refugee camp has enough food for " + coeff1 + " people for " + coeff2 + " days when everyone gets " + coeff3 + " gms of food daily. If the amount of daily food is reduced to " + coeff4 + " gms a day, how long will the food last?";
					int ans = (coeff2 * coeff3) / coeff4;
					Answer = ans.ToString ();
                }
                else if (selector == 3)
                {
					do {
						coeff1 = Random.Range (200, 1000);
						coeff2 = 10 * Random.Range (100, 500);
						coeff3 = Random.Range (30, 150);
					} while (coeff2 % coeff3 != 0 || coeff1 % coeff3 != 0);

					QuestionText.text = "A train crosses a bridge of " + coeff2 + " m length in " + ((coeff1 + coeff2) / coeff3) + " seconds and a pole in " + (coeff1 / coeff3) + " seconds. Find the length (in m) of the train.";
					Answer = coeff1.ToString ();
                }
                else if (selector == 4)
                {
					do {
						coeff1 = Random.Range (200, 1000);
						coeff2 = Random.Range (200, 1000);
						coeff3 = Random.Range (30, 100);
						coeff4 = Random.Range (coeff3 + 10, 150);
					} while ((18 * (coeff1 + coeff2)) % (5 * (coeff4 - coeff3)) != 0);

					QuestionText.text = "Train A of length " + coeff1 + " m and train B of length " + coeff2 + " m are running on parallel tracks in the same direction. Their average speeds are " + coeff1 + " km/hr and " + coeff2 + " km/hr respectively. How long (in seconds) will B take to overtake A?";
					int ans = (18 * (coeff1 + coeff2)) / (5 * (coeff4 - coeff3));
					Answer = ans.ToString ();
                }

            }
            #endregion
			Debug.Log (Answer);
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
			{   // Back
				if (userAnswerText.text.Length > 0)
				{
					userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
				}
			}
            else if (value == 11)
            {   // All Clear
                if (userAnswerText.text.Length > 0)
                {
					userAnswerText.text = "";
                }
            }
            else if (value == 12)
            {    //.
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerText.text = userAnswerText.text.Substring(0, userAnswerText.text.Length - 1);
                }
                userAnswerText.text += "/";
            }
        }
    }
}

