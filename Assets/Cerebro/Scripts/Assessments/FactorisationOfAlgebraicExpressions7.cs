using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class FactorisationOfAlgebraicExpressions7 : BaseAssessment
    {

        public TEXDraw subQuestionTEX;
		public GameObject MCQ;
        private string Answer;
        private string[] Answerarray;

        private int coeff1;
        private int coeff2;
        private int coeff3;
        private int coeff4;
        private int coeff5;
		private int coeff6;
		private int sign;
        private string expression1;
        private string expression2;
        private string expression3;
        private string expression4;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "FAE07", "S01", "A01");

            scorestreaklvls = new int[4];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;

            coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = 0;

            Answer = "";
            GenerateQuestion();
        }

        public override void SubmitClick()
        {
            if (ignoreTouches || userAnswerLaText.text == "")
            {
                return;
            }
				
            int increment = 0;
            //var correct = false;
            ignoreTouches = true;
            //Checking if the response was correct and computing question level
            var correct = true;
            CerebroHelper.DebugLog("!" + userAnswerLaText.text + "!");
            CerebroHelper.DebugLog("*" + Answer + "*");
            questionsAttempted++;
            updateQuestionsAttempted();
			if (Answer == userAnswerLaText.text) 
			{
				correct = true;
			}
			else
			{
				correct = false;
				AnimateMCQOptionCorrect(Answer);

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
				}else if (Queslevel == 5)
				{
					increment = 15;
				}


                UpdateStreak(12, 16);

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
            userAnswerLaText.color = MaterialColor.red800;
			Go.to(userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
            yield return new WaitForSeconds(0.5f);
			if (isRevisitedQuestion)
			{
				if (userAnswerText != null)
				{
					userAnswerText.color = MaterialColor.textDark;
				}
				if (userAnswerLaText != null)
				{
					userAnswerLaText.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
			}
			else
			{
					CerebroHelper.DebugLog("going in else");
					userAnswerLaText.color = MaterialColor.textDark;
			}
            ShowContinueButton();
        }
        protected override IEnumerator ShowCorrectAnimation()
        {
            userAnswerLaText.color = MaterialColor.green800;
            var config = new GoTweenConfig()
                .scale(new Vector3(1.1f, 1.1f, 1f))
                .setIterations(2, GoLoopType.PingPong);
            var flow = new GoTweenFlow(new GoTweenCollectionConfig().setIterations(1));
			var tween = new GoTween(userAnswerLaText.gameObject.transform, 0.2f, config);
            flow.insert(0f, tween);
            flow.play();
            yield return new WaitForSeconds(1f);
            userAnswerLaText.color = MaterialColor.textDark;

            showNextQuestion();

            if (levelUp)
            {
                StartCoroutine(HideAnimation());
                base.LevelUp();
                yield return new WaitForSeconds(1.5f);
                StartCoroutine(StartAnimation());
            }

        }

		void AnimateMCQOptionCorrect(string ans)
		{
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.green800;
				}
			}
		}

		public void MCQOptionClicked (int value)
		{
			if (ignoreTouches) {
				return;
			}
			AnimateMCQOption (value);
			userAnswerLaText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<TEXDraw> ();
			answerButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
			SubmitClick ();
		}

		IEnumerator AnimateMCQOption (int value)
		{
			var GO = MCQ.transform.Find ("Option" + value.ToString ()).gameObject;
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1.2f, 1.2f, 1), false));
			yield return new WaitForSeconds (0.2f);
			Go.to (GO.transform, 0.2f, new GoTweenConfig ().scale (new Vector3 (1, 1, 1), false));
		}

		void RandomizeMCQOptionsAndFill(List<string> options)
		{
			int index = 0;
			int cnt = options.Count;
			for (int i = 1; i <= cnt; i++) {
				index = Random.Range (0, options.Count);
				MCQ.transform.Find ("Option"+i).Find ("Text").GetComponent<TEXDraw> ().text = options [index];
				options.RemoveAt (index);
			}
		}

        protected override void GenerateQuestion()
        {
            ignoreTouches = false;
            base.QuestionStarted();
            // Generating the parameters

            level = Queslevel;

			subQuestionTEX.gameObject.SetActive(true);
            QuestionText.gameObject.SetActive(true);
			MCQ.SetActive (true);
			List<string> options = new List<string>();
			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}

            QuestionText.text = null;
            subQuestionTEX.text = null;


            if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

            
            
            #region level1
            if (level == 1)
            {				          
                selector = GetRandomSelector(1,6);

                if (selector == 1)
                {					
					coeff1 = Random.Range(2,10);
					coeff2 = coeff1 * Random.Range(2,10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff1 + "\\xalgebra^{2} + " + coeff2 + "\\xalgebra\\yalgebra";
					expression1 = coeff1 + "\\xalgebra(\\xalgebra + " + (coeff2/coeff1) + "\\yalgebra)";
					expression2 = (coeff2/coeff1) + "\\xalgebra(" + coeff1 + "\\xalgebra + "  + (coeff2/coeff1) + "\\yalgebra)";
					expression3 = coeff1 + "\\xalgebra(" + (coeff2/coeff1) + "\\xalgebra + \\yalgebra)";
					expression4 = "\\xalgebra(" + coeff1 + "\\xalgebra + " + (coeff2/coeff1) + "\\yalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);
				} 
				else if (selector == 2)
				{

					coeff1 = Random.Range(2,10);
					coeff2 = coeff1 * Random.Range(2,10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff1 + "\\xalgebra^{2}\\yalgebra^{3} - " + coeff2 + "\\xalgebra\\yalgebra^{2}";
					expression1 = coeff1 + "\\xalgebra\\yalgebra^{2}(\\xalgebra\\yalgebra - " + (coeff2/coeff1) + ")";
					expression2 = (coeff2/coeff1) + "\\xalgebra\\yalgebra^{2}(" + coeff1 + "\\xalgebra\\yalgebra - "  + (coeff2/coeff1) + ")";
					expression3 = coeff1 + "\\xalgebra\\yalgebra^{2}(" + (coeff2/coeff1) + "\\xalgebra\\yalgebra - " + coeff1 + ")";
					expression4 = coeff1 + "\\xalgebra\\yalgebra^{2}(\\xalgebra\\yalgebra - " + coeff2 + ")";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 3)
				{

					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2,10);
					while (coeff1 == coeff2)
						coeff2 = Random.Range(2,10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = (coeff1 * coeff1) + "\\xalgebra^{2} - " + (coeff2 * coeff2) + "\\yalgebra^{2}";
					expression1 = "(" + coeff1 + "\\xalgebra - " + coeff2 + "\\yalgebra)(" + coeff1 + "\\xalgebra + " + coeff2 + "\\yalgebra)";
					expression2 = "(" + coeff1 + "\\xalgebra - " + coeff2 + "\\yalgebra)(" + coeff1 + "\\xalgebra - " + coeff2 + "\\yalgebra)";
					expression3 = "(" + coeff2 + "\\xalgebra - " + coeff1 + "\\yalgebra)(" + coeff1 + "\\xalgebra + " + coeff2 + "\\yalgebra)";
					expression4 = "(" + coeff2 + "\\xalgebra - " + coeff1 + "\\yalgebra)(" + coeff2 + "\\xalgebra + " + coeff1 + "\\yalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 4)
				{
					sign = 1;
					if (Random.Range (1,3) == 1)
						sign *= -1;
					coeff1 = Random.Range (2,10);
					coeff2 = coeff1 * Random.Range (2,10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff1 + "\\xalgebra^{3} " + (sign == 1? "+ ":"- ") + coeff1 + "\\xalgebra^{2} + " + coeff2 + "\\xalgebra " + (sign == 1? "+ ":"- ") + coeff2;
					expression1 = coeff1 + "(\\xalgebra^{2} + " + (coeff2/coeff1) + ")(\\xalgebra " + (sign == 1? "+ ":"- ") + "1)";
					expression2 = coeff1 + "(\\xalgebra^{2} " + (sign == 1? "+ ":"- ") + (coeff2/coeff1) + ")(\\xalgebra " + (sign == -1? "+ ":"- ") + "1)";
					expression3 = "(" + coeff1 + "\\xalgebra^{2} + 1)(\\xalgebra " + (sign == 1? "+ ":"- ") + coeff2 + ")";
					expression4 = "(" + coeff1 + "\\xalgebra^{2} " + (sign == 1? "+ ":"- ") + "1)(\\xalgebra - " + coeff2 + ")";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 5)
				{
					sign = 1;
					if (Random.Range (1,3) == 1)
						sign *= -1;
					coeff1 = Random.Range(2,10);
					coeff2 = coeff1 * Random.Range(2,10);
					coeff3 = Random.Range (2, 6);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff1 + "\\xalgebra^{2} " + (sign == 1? "+ ":"- ") + (coeff3 * coeff1) + "\\xalgebra\\yalgebra + " + coeff2 + "\\xalgebra^{2} " + (sign == 1? "+ ":"- ") + (coeff3 * coeff2) + "\\xalgebra\\yalgebra";
					coeff4 = coeff1 + coeff2;
					coeff5 = coeff3 * coeff4;
					coeff6 = coeff3 * (coeff2 - coeff1);
					int hcf = MathFunctions.GetHCF (coeff4, coeff6); 
					expression1 = coeff4 + "\\xalgebra(\\xalgebra " + (sign == 1? "+ ":"- ") + coeff3 + "\\yalgebra)";
					expression2 = coeff4 + "\\xalgebra(\\xalgebra " + (sign == -1? "+ ":"- ") + coeff3 + "\\yalgebra)";
					expression3 = hcf + "\\xalgebra(" + ((coeff4/hcf) == 1? "" : (coeff4/hcf).ToString()) + "\\xalgebra " + (sign == 1? "+ ":"- ") + ((coeff6/hcf) == 1? "" : (coeff6/hcf).ToString()) + "\\yalgebra)";
					expression4 = hcf + "\\xalgebra(" + ((coeff4/hcf) == 1? "" : (coeff4/hcf).ToString()) + "\\xalgebra " + (sign == -1? "+ ":"- ") + ((coeff6/hcf) == 1? "" : (coeff6/hcf).ToString()) + "\\yalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
            }
            #endregion
            #region level2
			else if(level == 2)
			{
				selector = GetRandomSelector(1,6);

				if (selector == 1)
				{

					sign = 1;
					if (Random.Range (1,3) == 1)
						sign *= -1;
					coeff1 = Random.Range(2,10);
					coeff2 = coeff1 * Random.Range(2,10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff1 + "\\xalgebra(\\aalgebra " + (sign == 1? "+ ":"- ") + "\\balgebra) " + (sign == -1? "+ ":"- ") + coeff2 + "\\yalgebra(\\aalgebra " + (sign == 1? "+ ":"- ") + "\\balgebra)";
					expression1 = coeff1 + "(\\xalgebra " + (sign == -1? "+ ":"- ") + (coeff2/coeff1) + "\\yalgebra)(\\aalgebra " + (sign == 1? "+ ":"- ") + "\\balgebra)";
					expression2 = coeff1 + "(\\xalgebra " + (sign == 1? "+ ":"- ") + (coeff2/coeff1) + "\\yalgebra)(\\aalgebra " + (sign == -1? "+ ":"- ") + "\\balgebra)";
					expression3 = coeff1 + "(\\xalgebra " + (sign == 1? "+ ":"- ") + (coeff2/coeff1) + "\\yalgebra)(\\aalgebra " + (sign == 1? "+ ":"- ") + "\\balgebra)";
					expression4 = coeff1 + "(\\xalgebra " + (sign == -1? "+ ":"- ") + (coeff2/coeff1) + "\\yalgebra)(\\aalgebra " + (sign == -1? "+ ":"- ") + "\\balgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 2)
				{
					
					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					while (MathFunctions.GetHCF(coeff1, coeff2) > 1)
						coeff1 = Random.Range (2, 10);
					coeff3 = Random.Range (2, 6);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff1 + "(\\aalgebra -\\balgebra)^{" + coeff3 + "} - " + coeff2 + "(\\aalgebra -\\balgebra)^{" + (coeff3 + 1) + "}";
					expression1 = "(\\aalgebra -\\balgebra )^{" + coeff3 + "}(" + coeff1 + " - " + coeff2 + "\\aalgebra + " + coeff2 + "\\balgebra)";
					expression2 = "(\\aalgebra -\\balgebra )^{" + coeff3 + "}(" + coeff1 + " + " + coeff2 + "\\aalgebra - " + coeff2 + "\\balgebra)";
					expression3 = "(\\aalgebra -\\balgebra )^{" + coeff3 + "}(" + coeff1 + " - " + coeff2 + "\\aalgebra - " + coeff2 + "\\balgebra)";
					expression4 = "(\\aalgebra -\\balgebra )^{" + coeff3 + "}(" + coeff1 + "\\aalgebra - " + coeff1 + "\\balgebra - " + coeff2 + ")";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 3)
				{

					coeff1 = Random.Range(2, 10);
					coeff2 = Random.Range(2, 10);
					while (MathFunctions.GetHCF(coeff1, coeff2) > 1)
						coeff1 = Random.Range (2, 10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff1 + "\\aalgebra\\xalgebra + " + coeff2 + "\\aalgebra\\yalgebra + " + coeff1 + "\\balgebra\\xalgebra + " + coeff2 + "\\balgebra\\yalgebra";
					expression1 = "(\\aalgebra + \\balgebra)(" + coeff1 + "\\xalgebra + " + coeff2 + "\\yalgebra)";
					expression2 = "(\\aalgebra + \\balgebra)(" + coeff2 + "\\xalgebra + " + coeff1 + "\\yalgebra)";
					expression3 = "(\\xalgebra + \\yalgebra)(" + coeff1 + "\\aalgebra + " + coeff2 + "\\balgebra)";
					expression4 = "(\\xalgebra + \\yalgebra)(" + coeff2 + "\\aalgebra + " + coeff1 + "\\balgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 4)
				{

					sign = 1;
					if (Random.Range (1,3) == 1)
						sign *= -1;
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = "\\malgebra\\xalgebra + \\nalgebra\\xalgebra " + (sign == -1? "+":"-") + " \\malgebra\\zalgebra " + (sign == 1? "+":"-") + " \\malgebra\\yalgebra " + (sign == 1? "+":"-") + " \\nalgebra\\yalgebra " + (sign == -1? "+":"-") + " \\nalgebra\\zalgebra";
					expression1 = "(\\malgebra + \\nalgebra)(\\xalgebra " + (sign == 1? "+":"-") + " \\yalgebra " + (sign == -1? "+":"-") + " \\zalgebra)";
					expression2 = "(\\malgebra + \\nalgebra)(\\xalgebra " + (sign == -1? "+":"-") + " \\yalgebra " + (sign == 1? "+":"-") + " \\zalgebra)";
					expression3 = "(\\malgebra - \\nalgebra)(\\xalgebra " + (sign == 1? "+":"-") + " \\yalgebra " + (sign == 1? "+":"-") + " \\zalgebra)";
					expression4 = "(\\malgebra - \\nalgebra)(\\xalgebra " + (sign == -1? "+":"-") + " \\yalgebra " + (sign == -1? "+":"-") + " \\zalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 5)
				{

					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2,10);
					while (coeff1 == coeff2)
						coeff1 = Random.Range (2, 10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = "\\frac{\\xalgebra^{2}}{" + (coeff1 * coeff1) + "} - \\frac{\\yalgebra^{2}}{" + (coeff2 * coeff2) + "}";
					expression1 = "(\\frac{\\xalgebra}{" + coeff1 + "} - \\frac{\\yalgebra}{" + coeff2 + "})(\\frac{\\xalgebra}{" + coeff1 + "} + \\frac{\\yalgebra}{" + coeff2 + "})";
					expression2 = "(\\frac{\\xalgebra}{" + coeff2 + "} - \\frac{\\yalgebra}{" + coeff1 + "})(\\frac{\\xalgebra}{" + coeff2 + "} + \\frac{\\yalgebra}{" + coeff1 + "})";
					expression3 = "(\\frac{\\xalgebra}{" + coeff1 + "} - \\frac{\\yalgebra}{" + coeff2 + "})(\\frac{\\xalgebra}{" + coeff1 + "} - \\frac{\\yalgebra}{" + coeff2 + "})";
					expression4 = "(\\frac{\\xalgebra}{" + coeff2 + "} - \\frac{\\yalgebra}{" + coeff1 + "})(\\frac{\\xalgebra}{" + coeff2 + "} - \\frac{\\yalgebra}{" + coeff1 + "})";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
			}
			#endregion
			#region level3
            else if (level == 3)
            {
				selector = GetRandomSelector(1, 6);

                if (selector == 1)
                {

					sign = 1;
					if (Random.Range (1,3) == 1)
						sign *= -1;
					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (2, 10);
					coeff3 = 2 * coeff1 - 1;
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = "\\aalgebra^{2} + " + coeff1 + "\\aalgebra^{4} " + (sign == 1? "+ ":"- ") + coeff2 + "\\aalgebra^{7} " + (sign == -1? "+ ":"- ") + coeff3 + "\\aalgebra^{9}";
					expression1 = "\\aalgebra^{2}(1 + " + coeff1 + "\\aalgebra^{2} " + (sign == 1? "+ ":"- ") + coeff2 + "\\aalgebra^{5} " + (sign == -1? "+ ":"- ") + coeff3 + "\\aalgebra^{7})";
					expression2 = "\\aalgebra^{2}(1 + " + coeff1 + "\\aalgebra^{2} " + (sign == -1? "+ ":"- ") + coeff2 + "\\aalgebra^{5} " + (sign == -1? "+ ":"- ") + coeff3 + "\\aalgebra^{7})";
					expression3 = "\\aalgebra^{2}(1 + " + coeff1 + "\\aalgebra^{2} " + (sign == 1? "+ ":"- ") + coeff2 + "\\aalgebra^{5} " + (sign == 1? "+ ":"- ") + coeff3 + "\\aalgebra^{7})";
					expression4 = "\\aalgebra^{2}(1 + " + coeff1 + "\\aalgebra^{2} " + (sign == -1? "+ ":"- ") + coeff2 + "\\aalgebra^{5} " + (sign == -1? "+ ":"- ") + coeff3 + "\\aalgebra^{7})";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

                }
                else if (selector == 2)
                {
					
					coeff1 = Random.Range (2, 10);
					coeff2 = coeff1 * Random.Range (2, 10);
					coeff3 = coeff2/coeff1;
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff2 + "\\xalgebra\\yalgebra\\zalgebra(\\xalgebra + \\yalgebra + \\zalgebra)^{2} - " + coeff1 + "\\xalgebra\\yalgebra(\\xalgebra + \\yalgebra + \\zalgebra)";
					expression1 = coeff1 + "\\xalgebra\\yalgebra(\\xalgebra + \\yalgebra + \\zalgebra)(" + coeff3 + "\\xalgebra\\zalgebra + " + coeff3 + "\\yalgebra\\zalgebra + " + coeff3 + "\\zalgebra^{2} - 1)";
					expression2 = coeff1 + "\\xalgebra\\yalgebra(\\xalgebra + \\yalgebra + \\zalgebra)(" + coeff3 + "\\xalgebra\\zalgebra + " + coeff3 + "\\yalgebra\\zalgebra + " + coeff3 + "\\zalgebra^{2})";
					expression3 = coeff1 + "\\xalgebra\\yalgebra\\zalgebra(\\xalgebra + \\yalgebra + \\zalgebra)(" + coeff3 + "\\xalgebra + " + coeff3 + "\\yalgebra - " + coeff3 + "\\zalgebra - 1)";
					expression4 = coeff1 + "\\xalgebra\\yalgebra\\zalgebra(\\xalgebra + \\yalgebra + \\zalgebra)(" + coeff3 + "\\xalgebra + " + coeff3 + "\\yalgebra + " + coeff3 + "\\zalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

                }
                else if (selector == 3)
                {

					coeff1 = Random.Range(2,10);
					coeff2 = Random.Range(2,10);
					while (coeff1 == coeff2)
						coeff1 = Random.Range (2, 10);
					float num1 = MathFunctions.GetRounded (coeff1/10f,1);
					float num2 = MathFunctions.GetRounded (coeff2/10f,1);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = MathFunctions.GetRounded (num1 * num1, 2) + "\\xalgebra^{2} - \\frac{\\yalgebra^{2}}{" + MathFunctions.GetRounded (num2 * num2, 2) + "}";
					expression1 = "({" + num1 + "\\xalgebra - \\frac{\\yalgebra}{" + num2 + "}})({" + num1 + "\\xalgebra + \\frac{\\yalgebra}{" + num2 + "}})";
					expression2 = "({" + num1 + "\\xalgebra - \\frac{\\yalgebra}{" + num2 + "}})({" + num1 + "\\xalgebra - \\frac{\\yalgebra}{" + num2 + "}})";
					expression3 = "({" + num1 + "\\yalgebra - \\frac{\\xalgebra}{" + num2 + "}})({" + num1 + "\\yalgebra + \\frac{\\xalgebra}{" + num2 + "}})";
					expression4 = "({" + num1 + "\\yalgebra - \\frac{\\xalgebra}{" + num2 + "}})({" + num1 + "\\yalgebra - \\frac{\\xalgebra}{" + num2 + "}})";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);


                }
				else if (selector == 4)
				{

					sign = 1;
					if (Random.Range (1,3) == 1)
						sign *= -1;
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = "\\xalgebra^{2} - \\xalgebra\\yalgebra - " + coeff1 + "\\xalgebra + " + coeff1 + "\\yalgebra";
					expression1 = "(\\xalgebra - \\yalgebra)(\\xalgebra - " + coeff1 + ")";
					expression2 = "(\\xalgebra + \\yalgebra)(\\xalgebra - " + coeff1 + ")";
					expression3 = "(\\xalgebra - \\yalgebra)(\\xalgebra + " + coeff1 + ")";
					expression4 = "(\\xalgebra - 1)(\\xalgebra - " + coeff1 + "\\yalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 5)
				{

					sign = 1;
					if (Random.Range (1,3) == 1)
						sign *= -1;
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = "1 - (\\xalgebra " + (sign == 1? "+":"-") + " \\yalgebra " + (sign == -1? "+":"-") + " \\zalgebra)^{2}";
					expression1 = "(1 + \\xalgebra " + (sign == 1? "+":"-") + " \\yalgebra " + (sign == -1? "+":"-") + " \\zalgebra)(1 - \\xalgebra " + (sign == -1? "+":"-") + " \\yalgebra " + (sign == 1? "+":"-") + " \\zalgebra)";
					expression2 = "(1 + \\xalgebra " + (sign == -1? "+":"-") + " \\yalgebra " + (sign == 1? "+":"-") + " \\zalgebra)(1 - \\xalgebra " + (sign == -1? "+":"-") + " \\yalgebra " + (sign == 1? "+":"-") + " \\zalgebra)";
					expression3 = "(1 + \\xalgebra " + (sign == 1? "+":"-") + " \\yalgebra " + (sign == -1? "+":"-") + " \\zalgebra)(1 - \\xalgebra " + (sign == 1? "+":"-") + " \\yalgebra " + (sign == -1? "+":"-") + " \\zalgebra)";
					expression4 = "(1 - \\xalgebra " + (sign == -1? "+":"-") + " \\yalgebra " + (sign == 1? "+":"-") + " \\zalgebra)(1 - \\xalgebra " + (sign == -1? "+":"-") + " \\yalgebra " + (sign == 1? "+":"-") + " \\zalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}					
            }
			#endregion
			#region level4
			else if(level == 4)
			{
				selector = GetRandomSelector(1, 4);

				if (selector == 1)
				{

					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (2, 10);
					while (MathFunctions.GetHCF(coeff1, coeff2) > 1)
						coeff2 = Random.Range (2, 10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = (coeff2 * coeff2) + "\\xalgebra - " + (coeff1 * coeff1) + "\\xalgebra^{3}";
					expression1 = "\\xalgebra(" + coeff1 + "\\xalgebra - " + coeff2 + ")(" + coeff1 + "\\xalgebra + " + coeff2 + ")";
					expression2 = "\\xalgebra(" + coeff1 + "\\xalgebra - " + coeff2 + ")(" + coeff1 + "\\xalgebra - " + coeff2 + ")";
					expression3 = "\\xalgebra(" + coeff1 + " - " + coeff2 + "\\xalgebra)(" + coeff1 + " - " + coeff2 + "\\xalgebra)";
					expression4 = "\\xalgebra(" + coeff2 + " - " + coeff1 + "\\xalgebra)(" + coeff2 + " + " + coeff1 + "\\xalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[3];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 2)
				{
					
					coeff1 = Random.Range (2, 10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = "\\xalgebra^{4} - " + Mathf.Pow(coeff1, 4);
					expression1 = "(\\xalgebra^{2} + " + (coeff1 * coeff1) + ")(\\xalgebra + " + coeff1 + ")(\\xalgebra - " + coeff1 + ")";
					expression2 = "(\\xalgebra^{2} - " + (coeff1 * coeff1) + ")(\\xalgebra - " + coeff1 + ")(\\xalgebra + " + coeff1 + ")";
					expression3 = "(\\xalgebra + " + coeff1 + ")^{2}(\\xalgebra - " + coeff1 + ")^{2}";
					expression4 = "(\\xalgebra^{2} + " + coeff1 + ")(\\xalgebra + " + coeff1 + ")(\\xalgebra - " + coeff1 + ")";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
				else if (selector == 3)
				{

					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (2, 10);  
					while (MathFunctions.GetHCF(coeff1, coeff2) > 1)
						coeff1 = Random.Range (2, 10);
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = coeff1 + "(\\xalgebra - \\yalgebra)^{2} - " + (coeff1 * coeff2 * coeff2) + "(\\xalgebra - \\yalgebra)^{4}";
					expression1 = coeff1 + "(\\xalgebra - \\yalgebra)^{2}(1 - " + coeff2 + "\\xalgebra + " + coeff2 + "\\yalgebra)(1 + " + coeff2 + "\\xalgebra - " + coeff2 + "\\yalgebra)";
					expression2 = coeff1 + "(\\xalgebra - \\yalgebra)^{2}(1 - " + coeff2 + "\\xalgebra + " + coeff2 + "\\yalgebra)(1 - " + coeff2 + "\\xalgebra + " + coeff2 + "\\yalgebra)";
					expression3 = coeff1 + "(\\xalgebra - \\yalgebra)^{2}(1 - " + coeff2 + "\\xalgebra - " + coeff2 + "\\yalgebra)(1 + " + coeff2 + "\\xalgebra + " + coeff2 + "\\yalgebra)";
					expression4 = coeff1 + "(\\xalgebra - \\yalgebra)^{2}(1 + " + coeff2 + "\\xalgebra - " + coeff2 + "\\yalgebra)(1 + " + coeff2 + "\\xalgebra - " + coeff2 + "\\yalgebra)";
					options.Add(expression1);	
					options.Add(expression2);
					options.Add(expression3);
					options.Add(expression4);
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);

				}
			}
			#endregion

			CerebroHelper.DebugLog (Answer);
           // userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw>("Text");
            //userAnswerLaText.text = "";
        }
		public override void numPadButtonPressed(int value) {
			
		}

    }
}

