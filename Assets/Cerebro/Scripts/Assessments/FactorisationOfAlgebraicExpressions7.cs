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
					subQuestionTEX.text = coeff1 + "x^{2} + " + coeff2 + "xy";
					expression1 = coeff1 + "x(x + " + (coeff2/coeff1) + "y)";
					expression2 = (coeff2/coeff1) + "x(" + coeff1 + "x + "  + (coeff2/coeff1) + "y)";
					expression3 = coeff1 + "x(" + (coeff2/coeff1) + "x + y)";
					expression4 = "x(" + coeff1 + "x + " + (coeff2/coeff1) + "y)";
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
					subQuestionTEX.text = coeff1 + "x^{2}y^{3} - " + coeff2 + "xy^{2}";
					expression1 = coeff1 + "xy^{2}(xy - " + (coeff2/coeff1) + ")";
					expression2 = (coeff2/coeff1) + "xy^{2}(" + coeff1 + "xy - "  + (coeff2/coeff1) + ")";
					expression3 = coeff1 + "xy^{2}(" + (coeff2/coeff1) + "xy - " + coeff1 + ")";
					expression4 = coeff1 + "xy^{2}(xy - " + coeff2 + ")";
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
					subQuestionTEX.text = (coeff1 * coeff1) + "x^{2} - " + (coeff2 * coeff2) + "y^{2}";
					expression1 = "(" + coeff1 + "x - " + coeff2 + "y)(" + coeff1 + "x + " + coeff2 + "y)";
					expression2 = "(" + coeff1 + "x - " + coeff2 + "y)(" + coeff1 + "x - " + coeff2 + "y)";
					expression3 = "(" + coeff2 + "x - " + coeff1 + "y)(" + coeff1 + "x + " + coeff2 + "y)";
					expression4 = "(" + coeff2 + "x - " + coeff1 + "y)(" + coeff2 + "x + " + coeff1 + "y)";
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
					subQuestionTEX.text = coeff1 + "x^{3} " + (sign == 1? "+ ":"- ") + coeff1 + "x^{2} + " + coeff2 + "x " + (sign == 1? "+ ":"- ") + coeff2;
					expression1 = coeff1 + "(x^{2} + " + (coeff2/coeff1) + ")(x " + (sign == 1? "+ ":"- ") + "1)";
					expression2 = coeff1 + "(x^{2} " + (sign == 1? "+ ":"- ") + (coeff2/coeff1) + ")(x " + (sign == -1? "+ ":"- ") + "1)";
					expression3 = "(" + coeff1 + "x^{2} + 1)(x " + (sign == 1? "+ ":"- ") + coeff2 + ")";
					expression4 = "(" + coeff1 + "x^{2} " + (sign == 1? "+ ":"- ") + "1)(x - " + coeff2 + ")";
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
					subQuestionTEX.text = coeff1 + "x^{2} " + (sign == 1? "+ ":"- ") + (coeff3 * coeff1) + "xy + " + coeff2 + "x^{2} " + (sign == 1? "+ ":"- ") + (coeff3 * coeff2) + "xy";
					coeff4 = coeff1 + coeff2;
					coeff5 = coeff3 * coeff4;
					coeff6 = coeff3 * (coeff2 - coeff1);
					int hcf = MathFunctions.GetHCF (coeff4, coeff6); 
					expression1 = coeff4 + "x(x " + (sign == 1? "+ ":"- ") + coeff3 + "y)";
					expression2 = coeff4 + "x(x " + (sign == -1? "+ ":"- ") + coeff3 + "y)";
					expression3 = hcf + "x(" + ((coeff4/hcf) == 1? "" : (coeff4/hcf).ToString()) + "x " + (sign == 1? "+ ":"- ") + ((coeff6/hcf) == 1? "" : (coeff6/hcf).ToString()) + "y)";
					expression4 = hcf + "x(" + ((coeff4/hcf) == 1? "" : (coeff4/hcf).ToString()) + "x " + (sign == -1? "+ ":"- ") + ((coeff6/hcf) == 1? "" : (coeff6/hcf).ToString()) + "y)";
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
					subQuestionTEX.text = coeff1 + "x(a " + (sign == 1? "+ ":"- ") + "b) " + (sign == -1? "+ ":"- ") + coeff2 + "y(a " + (sign == 1? "+ ":"- ") + "b)";
					expression1 = coeff1 + "(x " + (sign == -1? "+ ":"- ") + (coeff2/coeff1) + "y)(a " + (sign == 1? "+ ":"- ") + "b)";
					expression2 = coeff1 + "(x " + (sign == 1? "+ ":"- ") + (coeff2/coeff1) + "y)(a " + (sign == -1? "+ ":"- ") + "b)";
					expression3 = coeff1 + "(x " + (sign == 1? "+ ":"- ") + (coeff2/coeff1) + "y)(a " + (sign == 1? "+ ":"- ") + "b)";
					expression4 = coeff1 + "(x " + (sign == -1? "+ ":"- ") + (coeff2/coeff1) + "y)(a " + (sign == -1? "+ ":"- ") + "b)";
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
					subQuestionTEX.text = coeff1 + "(a - b)^{" + coeff3 + "} - " + coeff2 + "(a - b)^{" + (coeff3 + 1) + "}";
					expression1 = "(a - b)^{" + coeff3 + "}(" + coeff1 + " - " + coeff2 + "a + " + coeff2 + "b)";
					expression2 = "(a - b)^{" + coeff3 + "}(" + coeff1 + " + " + coeff2 + "a - " + coeff2 + "b)";
					expression3 = "(a - b)^{" + coeff3 + "}(" + coeff1 + " - " + coeff2 + "a - " + coeff2 + "b)";
					expression4 = "(a - b)^{" + coeff3 + "}(" + coeff1 + "a - " + coeff1 + "b - " + coeff2 + ")";
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
					subQuestionTEX.text = coeff1 + "ax + " + coeff2 + "ay + " + coeff1 + "bx + " + coeff2 + "by";
					expression1 = "(a + b)(" + coeff1 + "x + " + coeff2 + "y)";
					expression2 = "(a + b)(" + coeff2 + "x + " + coeff1 + "y)";
					expression3 = "(x + y)(" + coeff1 + "a + " + coeff2 + "b)";
					expression4 = "(x + y)(" + coeff2 + "a + " + coeff1 + "b)";
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
					subQuestionTEX.text = "mx + nx " + (sign == -1? "+":"-") + " mz " + (sign == 1? "+":"-") + " my " + (sign == 1? "+":"-") + " ny " + (sign == -1? "+":"-") + " nz";
					expression1 = "(m + n)(x " + (sign == 1? "+":"-") + " y " + (sign == -1? "+":"-") + " z)";
					expression2 = "(m + n)(x " + (sign == -1? "+":"-") + " y " + (sign == 1? "+":"-") + " z)";
					expression3 = "(m - n)(x " + (sign == 1? "+":"-") + " y " + (sign == 1? "+":"-") + " z)";
					expression4 = "(m - n)(x " + (sign == -1? "+":"-") + " y " + (sign == -1? "+":"-") + " z)";
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
					subQuestionTEX.text = "\\frac{x^{2}}{" + (coeff1 * coeff1) + "} - \\frac{y^{2}}{" + (coeff2 * coeff2) + "}";
					expression1 = "(\\frac{x}{" + coeff1 + "} - \\frac{y}{" + coeff2 + "})(\\frac{x}{" + coeff1 + "} + \\frac{y}{" + coeff2 + "})";
					expression2 = "(\\frac{x}{" + coeff2 + "} - \\frac{y}{" + coeff1 + "})(\\frac{x}{" + coeff2 + "} + \\frac{y}{" + coeff1 + "})";
					expression3 = "(\\frac{x}{" + coeff1 + "} - \\frac{y}{" + coeff2 + "})(\\frac{x}{" + coeff1 + "} - \\frac{y}{" + coeff2 + "})";
					expression4 = "(\\frac{x}{" + coeff2 + "} - \\frac{y}{" + coeff1 + "})(\\frac{x}{" + coeff2 + "} - \\frac{y}{" + coeff1 + "})";
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
					subQuestionTEX.text = "a^{2} + " + coeff1 + "a^{4} " + (sign == 1? "+ ":"- ") + coeff2 + "a^{7} " + (sign == -1? "+ ":"- ") + coeff3 + "a^{9}";
					expression1 = "a^{2}(1 + " + coeff1 + " " + (sign == 1? "+ ":"- ") + coeff2 + "a^{5} " + (sign == -1? "+ ":"- ") + coeff3 + "a^{7})";
					expression2 = "a^{2}(1 + " + coeff1 + " " + (sign == 1? "+ ":"- ") + coeff2 + "a^{5} " + (sign == -1? "+ ":"- ") + coeff3 + "a^{7})";
					expression3 = "a^{2}(1 + " + coeff1 + " " + (sign == 1? "+ ":"- ") + coeff2 + "a^{5} " + (sign == -1? "+ ":"- ") + coeff3 + "a^{7})";
					expression4 = "a^{2}(1 + " + coeff1 + " " + (sign == 1? "+ ":"- ") + coeff2 + "a^{5} " + (sign == -1? "+ ":"- ") + coeff3 + "a^{7})";
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
					subQuestionTEX.text = coeff2 + "xyz(x + y + z)^{2} - " + coeff1 + "xy(x + y + z)";
					expression1 = coeff1 + "xy(x + y + z)(" + coeff3 + "xz + " + coeff3 + "yz + " + coeff3 + "z^{2} - 1)";
					expression2 = coeff1 + "xy(x + y + z)(" + coeff3 + "xz + " + coeff3 + "yz + " + coeff3 + "z^{2})";
					expression3 = coeff1 + "xyz(x + y + z)(" + coeff3 + "x + " + coeff3 + "y - " + coeff3 + "z - 1)";
					expression4 = coeff1 + "xyz(x + y + z)(" + coeff3 + "x + " + coeff3 + "y + " + coeff3 + "z)";
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
					subQuestionTEX.text = MathFunctions.GetRounded (num1 * num1, 2) + "x^{2} - \\frac{y^{2}}{" + MathFunctions.GetRounded (num2 * num2, 2) + "}";
					expression1 = "(" + num1 + "x - \\frac{y}{" + num2 + "})(" + num1 + "x + \\frac{y}{" + num2 + "})";
					expression2 = "(" + num1 + "x - \\frac{y}{" + num2 + "})(" + num1 + "x - \\frac{y}{" + num2 + "})";
					expression3 = "(" + num1 + "y - \\frac{x}{" + num2 + "})(" + num1 + "y + \\frac{x}{" + num2 + "})";
					expression4 = "(" + num1 + "y - \\frac{x}{" + num2 + "})(" + num1 + "y - \\frac{x}{" + num2 + "})";
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
					subQuestionTEX.text = "x^{2} - xy - " + coeff1 + "x + " + coeff1 + "y";
					expression1 = "(x - y)(x - " + coeff1 + ")";
					expression2 = "(x + y)(x - " + coeff1 + ")";
					expression3 = "(x - y)(x + " + coeff1 + ")";
					expression4 = "(x - 1)(x - " + coeff1 + "y)";
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
					subQuestionTEX.text = "1 - (x " + (sign == 1? "+":"-") + " y " + (sign == -1? "+":"-") + " z)^{2}";
					expression1 = "(1 + x " + (sign == 1? "+":"-") + " y " + (sign == -1? "+":"-") + " z)(1 - x " + (sign == -1? "+":"-") + " y " + (sign == 1? "+":"-") + " z)";
					expression2 = "(1 + x " + (sign == -1? "+":"-") + " y " + (sign == 1? "+":"-") + " z)(1 - x " + (sign == -1? "+":"-") + " y " + (sign == 1? "+":"-") + " z)";
					expression3 = "(1 + x " + (sign == 1? "+":"-") + " y " + (sign == -1? "+":"-") + " z)(1 - x " + (sign == 1? "+":"-") + " y " + (sign == -1? "+":"-") + " z)";
					expression4 = "(1 - x " + (sign == -1? "+":"-") + " y " + (sign == 1? "+":"-") + " z)(1 - x " + (sign == -1? "+":"-") + " y " + (sign == 1? "+":"-") + " z)";
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
					subQuestionTEX.text = (coeff2 * coeff2) + "x - " + (coeff1 * coeff1) + "x^{3}";
					expression1 = "x(" + coeff1 + "x - " + coeff2 + ")(" + coeff1 + "x + " + coeff2 + ")";
					expression2 = "x(" + coeff1 + "x - " + coeff2 + ")(" + coeff1 + "x - " + coeff2 + ")";
					expression3 = "x(" + coeff1 + " - " + coeff2 + "x)(" + coeff1 + " - " + coeff2 + "x)";
					expression4 = "x(" + coeff2 + " - " + coeff1 + "x)(" + coeff2 + " + " + coeff1 + "x)";
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
					QuestionText.text = "Factorise :";
					subQuestionTEX.text = "x^{4} - " + Mathf.Pow(coeff1, 4);
					expression1 = "(x^{2} + " + (coeff1 * coeff1) + ")(x + " + coeff1 + ")(x - " + coeff1 + ")";
					expression2 = "(x^{2} - " + (coeff1 * coeff1) + ")(x - " + coeff1 + ")(x + " + coeff1 + ")";
					expression3 = "(x + " + coeff1 + ")^{2}(x - " + coeff1 + ")^{2}";
					expression4 = "(x^{2} + " + coeff1 + ")(x + " + coeff1 + ")(x - " + coeff1 + ")";
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
					subQuestionTEX.text = coeff1 + "(x - y)^{2} - " + (coeff1 * coeff2 * coeff2) + "(x - y)^{4}";
					expression1 = coeff1 + "(x - y)^{2}(1 - " + coeff2 + "x + " + coeff2 + "y)(1 + " + coeff1 + "x - " + coeff2 + "y)";
					expression2 = coeff1 + "(x - y)^{2}(1 + " + coeff2 + "x - " + coeff2 + "y)(1 + " + coeff1 + "x - " + coeff2 + "y)";
					expression3 = coeff1 + "(x - y)^{2}(1 - " + coeff2 + "x + " + coeff2 + "y)(1 - " + coeff1 + "x + " + coeff2 + "y)";
					expression4 = coeff1 + "(x - y)^{2}(1 - " + coeff2 + "x + " + coeff2 + "y)(" + coeff1 + "x - " + coeff2 + "y)";
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

