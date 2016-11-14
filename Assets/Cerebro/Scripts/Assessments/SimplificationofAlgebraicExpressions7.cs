using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class SimplificationofAlgebraicExpressions7 : BaseAssessment
    {
        public TEXDraw subQuestionTEX;
        private string Answer;

        int flag;  
		private List<int> coeff;
		private int hcf;
		private List<int> pow;
        private string expression1;
        private TEXDraw expressionTEX1;
        private TEXDraw expressionTEX2;
        private TEXDraw expressionTEX3;
        private string expression2;
        private string expression3;
        private string expression4;
        private string expression5;
        private int upflag = 0;
		private bool multiplePossibleAnswers;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "SAE07", "S01", "A01");

            scorestreaklvls = new int[3];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;

            Answer = "";
            GenerateQuestion();
        }

		public override void SubmitClick ()
		{
			if (ignoreTouches) {
				return;
			}
			if (numPad.activeSelf && userAnswerLaText.text == "") {
				return;
			}

			int increment = 0;
			ignoreTouches = true;

			//Checking if the response was correct and computing question level
			var correct = false;

			questionsAttempted++;
			updateQuestionsAttempted ();

			if (multiplePossibleAnswers)
			{
				correct = MathFunctions.checkAlgebraicExpressions (Answer, userAnswerLaText.text);
			}
		 	else {
				if (userAnswerText.text == Answer) {
					correct = true;
				} else {
					correct = false;
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
					increment = 10;
				} 

				UpdateStreak (8, 12);

				updateQuestionsAttempted ();
				StartCoroutine (ShowCorrectAnimation ());
			} else {
				for (var i = 0; i < scorestreaklvls.Length; i++) {
					scorestreaklvls [i] = 0;
				}
				StartCoroutine (ShowWrongAnimation ());
			}

			base.QuestionEnded (correct, level, increment, selector);

		}

		protected override IEnumerator ShowWrongAnimation()
		{
			userAnswerLaText.color = MaterialColor.red800;
			Go.to(userAnswerLaText.gameObject.transform, 0.5f, new GoTweenConfig().shake(new Vector3(0, 0, 20), GoShakeType.Eulers));
			yield return new WaitForSeconds(0.5f);
			if (isRevisitedQuestion)
			{
				userAnswerLaText.text = "";
				userAnswerLaText.color = MaterialColor.textDark;
				ignoreTouches = false;
			}
			else {
				userAnswerLaText.text = Answer;
				userAnswerLaText.color = MaterialColor.green800;
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

        protected override void GenerateQuestion()
        {
            ignoreTouches = false;
            base.QuestionStarted();
            // Generating the parameters

            level = Queslevel;

            GeneralButton.gameObject.SetActive(true);
			subQuestionTEX.gameObject.SetActive(true);
            QuestionText.gameObject.SetActive(true);
			numPad.SetActive (true);

			coeff = new List<int> ();
			multiplePossibleAnswers = true;

            if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
            
            #region level1
			if (level == 1)
			{
				selector = GetRandomSelector(1, 10);

				if (selector == 1)  //c1x/c2
                {
					coeff.Add (Random.Range (2, 10));
					coeff.Add (Random.Range (2, 10));
					coeff[0] *= coeff[1];

					QuestionText.text = "Express in lowest term :";
					subQuestionTEX.text = string.Format ("\\frac{{{0}{1}}}{{{2}}}", coeff[0], "x".Algebra (), coeff[1]); 
					Answer = string.Format ("{1}{0}", "x".Algebra (), coeff[0]/coeff[1]);
				}
				else if (selector == 2)  //c1 xp0 yp1 zp2 / c2 xp3 yp4 zp5 
				{
					coeff.Add (Random.Range (2, 10));
					coeff.Add (coeff[0] * Random.Range (2, 10));

					pow = new List<int> ();
					for (int i = 0; i < 3; i++){
						pow.Add (Random.Range (2, 10));
					}
					for (int i = 0; i < 3; i++){
						pow.Add (pow[i] + (Random.Range (1, 3) == 1? 1 : -1) * Random.Range (2, 6));
					}

					QuestionText.text = "Express in lowest term :";
					subQuestionTEX.text = string.Format ("\\frac{{{0}{1}^{{{5}}}{2}^{{{6}}}{3}^{{{7}}}}}{{{4}{1}^{{{8}}}{2}^{{{9}}}{3}^{{{10}}}}}", coeff[0], "x".Algebra (), "y".Algebra (), "z".Algebra (), coeff[1], pow[0], pow[1], pow[2], pow[3], pow[4], pow[5]); 
					Answer = string.Format ("{0}^{{{3}}}{1}^{{{4}}}{2}^{{{5}}}/{6}", "x".Algebra (), "y".Algebra (), "z".Algebra (), pow[0] - pow[3], pow[1] - pow[4], pow[2] - pow[5], coeff[1]/coeff[0]);
				}
				else if (selector == 3)  //x/c0- (x-c1)/c2
				{
					for (int i = 0; i < 3; i++)
						coeff.Add (Random.Range (2, 10));

					while (coeff [0] == coeff[2]){
						coeff[2] = Random.Range (2, 10);
					}

					QuestionText.text = "Simplify :";
					subQuestionTEX.text = string.Format ("\\frac{{{3}}}{{{0}}} - \\frac{{{3}-{1}}}{{{2}}}", coeff[0], coeff[1], coeff[2], "x".Algebra ()); 
					//Answer = string.Format ("");
				}
				else if (selector == 4)   //c1xp0/c2 * c3xp1/c4
				{
					coeff.Add (Random.Range (2, 20));
					coeff.Add (Random.Range (2, 20));
					coeff.Add (coeff[1] * Random.Range (2, 10));
					coeff.Add (coeff[0] * Random.Range (2, 10));

					pow = new List<int> ();
					pow.Add (Random.Range (2, 10));
					pow.Add (Random.Range (2, 10));

					QuestionText.text = "Simplify :";
					subQuestionTEX.text = string.Format ("\\frac{{{0}}}{{{1}}}{4}^{{{5}}} \\times \\frac{{{2}}}{{{3}}}{4}^{{{6}}}", coeff[0], coeff[1], coeff[2], coeff[3], "x".Algebra (), pow[0], pow[1]); 
					//Answer = string.Format ("", "x".Algebra (), );
				}
				else if (selector == 5)
				{
					for (int i = 0; i < 3; i++)
						coeff.Add (Random.Range (2, 10));

					while (coeff[2] == coeff[1]){
						coeff[2] = Random.Range (2, 10);
					} 

					QuestionText.text = "Simplify :";
					subQuestionTEX.text = string.Format ("({{{3} + \\frac{{{3}}}{{{0}}}}}) \\div ({{\\frac{{{3}}}{{{1}}} + \\frac{{{3}}}{{{2}}}}})", coeff[0], coeff[1], coeff[2], "m".Algebra ()); 
					//Answer = (m+m/c1)/(m/c2+m/c3)
				}
				else if (selector == 6)   //(x/c1 * c2/c3) / (c4x/c5 * c6 c7/c8)
				{
					coeff.Add (Random.Range (2, 6));
					coeff.Add (Random.Range (1, 5));
					coeff.Add (Random.Range (coeff[1] + 1, 6));

					while (MathFunctions.GetHCF (coeff[1], coeff[2]) > 1)
						coeff[2] = Random.Range (coeff[1] + 1, 6);
					
					coeff.Add (Random.Range (1, 5));
					coeff.Add (Random.Range (coeff[3] + 1, 6));

					while (MathFunctions.GetHCF (coeff[3], coeff[4]) > 1)
						coeff[4] = Random.Range (coeff[3] + 1, 6);
					
					coeff.Add (Random.Range (1, 6));
					coeff.Add (Random.Range (1, 5));
					coeff.Add (Random.Range (coeff[6] + 1, 6));

					while (MathFunctions.GetHCF (coeff[6], coeff[7]) > 1)
						coeff[7] = Random.Range (coeff[6] + 1, 6);

					QuestionText.text = "Simplify :";
					subQuestionTEX.text = string.Format ("({{\\frac{{{8}}}{{{0}}} \\times \\frac{{{1}}}{{{2}}}}}) \\div ({{\\frac{{{3}}}{{{4}}}{8} \\times {5}\\frac{{{6}}}{{{7}}}}})", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], coeff[5], coeff[6], coeff[7], "x".Algebra ()); 
					//Answer = 
				}
				else if (selector == 7) //(c1ax + ax/c2) / (c3ax + ax/c4)
				{
					for (int i = 0; i < 4; i++)
						coeff.Add (Random.Range (2, 10));

					while (coeff[1] == coeff[3])
						coeff[3] = Random.Range (2, 10);

					QuestionText.text = "Simplify :";
					subQuestionTEX.text = string.Format ("({{{0}{4}{5} + \\frac{{{4}{5}}}{{{1}}}}}) \\div ({{{2}{4}{5} + \\frac{{{4}{5}}}{{{3}}}}})", coeff[0], coeff[1], coeff[2], coeff[3], "a".Algebra (), "x".Algebra ()); 
					//Answer = 
				}
				else if (selector == 8)
				{
					for (int i = 0; i < 3; i++)
						coeff.Add (Random.Range (2, 10));

					while (MathFunctions.GetHCF (coeff[0], coeff[2]) > 1)
						coeff[0] = Random.Range (2, 10);

					QuestionText.text = "Simplify :";
					subQuestionTEX.text = string.Format ("\\frac{{{0}({3} - {1})}}{{{2}}} + \\frac{{{2}({3} + {1})}}{{{0}}}", coeff[0], coeff[1], coeff[2], "x".Algebra ()); 
					//Answer = c1(x-c2)/c3+c3(x+c2)/c1
				}
				else if (selector == 9)  //c1xp0/c2yp1 ÷ c3xp2/c4yp3
				{
					coeff.Add (Random.Range (2, 5));
					coeff.Add (Random.Range (5, 20));

					while (MathFunctions.GetHCF (coeff[0], coeff[1]) > 1)
						coeff[0] = Random.Range (2, coeff[1]);

					coeff.Add (coeff[0] * Random.Range (2, 10));
					coeff.Add (coeff[1] * Random.Range (2, 10));


					pow = new List<int> ();
					for (int i = 0; i < 4; i++){
						pow.Add (Random.Range (2, 10));
					}

					QuestionText.text = "Simplify :";
					subQuestionTEX.text = string.Format ("\\frac{{{0}{4}^{{{6}}}}}{{{1}{5}^{{{7}}}}} \\div \\frac{{{2}{4}^{{{8}}}}}{{{3}{5}^{{{9}}}}}", coeff[0], coeff[1], coeff[2], coeff[3], "x".Algebra (), "y".Algebra (), pow[0], pow[1], pow[2], pow[3]); 
					//Answer = 
				}

            }
            #endregion
            #region level2
            else if (level == 2)
            {
                selector = GetRandomSelector(1, 6);
                
                if (selector == 1)  
                {
					QuestionText.text = "Insert a bracket before the required term :";
					 
					if (Random.Range (1, 3) == 1)    //c1x + c2y + z - c3 before c2y
					{
						for (int i = 0; i < 3; i++)
							coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("{0}{3} + {1}{4} + {5} - {2} ; before {1}{4}", coeff[0], coeff[1], coeff[2], "x".Algebra (), "y".Algebra (), "z".Algebra ());
					}
					else     //x2 + c1xy - c2y2 + c3 before c1xy
					{
						for (int i = 0; i < 3; i++)
							coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("{3}^{{2}} + {0}{3}{4} - {1}{4}^{{2}} + {2} ; before {0}{3}{4}", coeff[0], coeff[1], coeff[2], "x".Algebra (), "y".Algebra ());
					}
						
                }
                else if (selector == 2)
                {
					QuestionText.text = "Insert a bracket before the required term :";

					if (Random.Range (1, 3) == 1)    //x - c2y - c1z + c3 before c2y
					{
						for (int i = 0; i < 3; i++)
							coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("{3} - {1}{4} - {0}{5} + {2} ; before {1}{4}", coeff[0], coeff[1], coeff[2], "x".Algebra (), "y".Algebra (), "z".Algebra ());
					}
					else     //m2 - c2n2 + c1mn + c3 before c1mn
					{
						for (int i = 0; i < 3; i++)
							coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("{3}^{{2}} - {1}{4}^{{2}} + {0}{3}{4} + {2} ; before {0}{3}{4}", coeff[0], coeff[1], coeff[2], "m".Algebra (), "n".Algebra ());
					}
                }
           
                else if (selector == 3)
                {
					QuestionText.text = "Remove brackets :";

					if (Random.Range (1, 3) == 1)    //c1x + (c2y - z + c3) 
					{
						for (int i = 0; i < 3; i++)
							coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("{0}{3} + ({1}{4} - {5} + {2})", coeff[0], coeff[1], coeff[2], "x".Algebra (), "y".Algebra (), "z".Algebra ());
					}
					else     //c1x + (-c2y + z + c3)
					{
						for (int i = 0; i < 3; i++)
							coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("{0}{3} + (-{1}{4} + {5} + {2})", coeff[0], coeff[1], coeff[2], "x".Algebra (), "y".Algebra (), "z".Algebra ());
					}
                    
                }
                else if (selector == 4)
                {
					QuestionText.text = "Remove brackets :";

					if (Random.Range (1, 3) == 1)    //c1x2 - (-c2y2 + c3xy - c4y - z)
					{
						for (int i = 0; i < 4; i++)
							coeff.Add (Random.Range (2, 10));
						
						subQuestionTEX.text = string.Format ("{0}{4}^{{2}} - (-{1}{5}^{{2}} + {2}{4}{5} - {3}{5} - {6})", coeff[0], coeff[1], coeff[2], coeff[3], "x".Algebra (), "y".Algebra (), "z".Algebra ());
					}
					else     // c1m2n2 - (m2 - c2n2 + c3mn - c4)
					{
						for (int i = 0; i < 4; i++)
							coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("{0}{3}^{{2}}{5}^{{2}} - ({4}^{{2}} - {1}{5}^{{2}} + {2}{4}{5} - {3})", coeff[0], coeff[1], coeff[2], coeff[3], "m".Algebra (), "n".Algebra ());
					}

                }
				else if (selector == 5)  //c1x - (c2x + c3y - c4z)
                {
					for (int i = 0; i < 4; i++)
						coeff.Add (Random.Range (2, 10));
					
					subQuestionTEX.text = string.Format ("{0}{4} - ({1}{4} + {2}{5} - {3}{6})", coeff[0], coeff[1], coeff[2], coeff[3], "x".Algebra (), "y".Algebra (), "z".Algebra ());
                }
            }
            #endregion
            #region level3
            else if (level == 3)
            {
				selector = GetRandomSelector(1, 7);

				QuestionText.text = "Simplify :";

				if (selector == 1)  
				{
					if (Random.Range (1, 3) == 1)    // x + y - (x - c1y) + (c1x - y)
					{
						coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("{1} + {2} - ({1} - {0}{2}) + ({0}{1} - {2})", coeff[0], "x".Algebra (), "y".Algebra ());
					}
					else     // (p + q + r) + c1(p + q + r) - c2(p + q + r)
					{
						coeff.Add (Random.Range (2, 10));
						coeff.Add (Random.Range (2, 10));

						subQuestionTEX.text = string.Format ("({2} + {3} + {4}) + {0}({2} + {3} + {4}) - {1}({2} + {3} + {4})", coeff[0], coeff[1], "p".Algebra (), "q".Algebra (), "r".Algebra ());
					}
				}
				else if (selector == 2)  // c1x2 - c1x(x - c2)
				{
					for (int i = 0; i < 2; i++)
						coeff.Add (Random.Range (2, 10));

					subQuestionTEX.text = string.Format ("{0}{2}^{{2}} - {0}{2}({2} - {1})", coeff[0], coeff[1], "x".Algebra ());
				}
				else if (selector == 3)  // c1x - [c2y - {c3x - (c4y - c5x)}]
				{
					for (int i = 0; i < 5; i++)
						coeff.Add (Random.Range (2, 10));
					
					subQuestionTEX.text = string.Format ("{0}{5} - [{1}{6} - \\lbrace{{{2}{5} - ({3}{6} - {4}{5})}}\\rbrace]", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], "x".Algebra (), "y".Algebra ());
				}
				else if (selector == 4)  // c1y - [c2y + x - {c3x + (c4x - c5y + y)}]
				{
					for (int i = 0; i < 5; i++)
						coeff.Add (Random.Range (2, 10));

					subQuestionTEX.text = string.Format ("{0}{6} - [{1}{6} + {5} - \\lbrace{{{2}{5} + ({3}{5} - {4}{6} + {6})}}\\rbrace]", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], "x".Algebra (), "y".Algebra ());
				}
				else if (selector == 5)  // c1x(c2x2 - c3x + c4) - c5x(c6x2 - c7x - c8) - c9x(x2 - c10x + c11)
				{
					for (int i = 0; i < 11; i++)
						coeff.Add (Random.Range (2, 10));

					subQuestionTEX.text = string.Format ("{0}{11}({1}{11}^{{2}} - {2}{11} + {3}) - {4}{11}({5}{11}^{{2}} - {6}{11} + {7}) - {8}{11}({11}^{{2}} - {9}{11} + {10})", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], coeff[5], coeff[6], coeff[7], coeff[8], coeff[9], coeff[10], "x".Algebra ());
				}
				else if (selector == 6)  // c1x - c2y - [c3x - c4y - {c5x - y - (x + c6y)}]
				{
					for (int i = 0; i < 6; i++)
						coeff.Add (Random.Range (2, 10));

					subQuestionTEX.text = string.Format ("{0}{6} - {1}{7} - [{2}{6} - {3}{7} - \\lbrace{{{4}{6} - {7} - ({6} + {5}{7})}}\\rbrace]", coeff[0], coeff[1], coeff[2], coeff[3], coeff[4], coeff[5], "x".Algebra (), "y".Algebra ());
				}
            }
            #endregion
            userAnswerLaText = GeneralButton.gameObject.GetChildByName<TEXDraw>("Text");
            userAnswerLaText.text = "";
			Debug.Log ("Answer = " + Answer);
        }

        public override void numPadButtonPressed(int value)
        {
            if (ignoreTouches)
            {
                return;
            }
            if (value <= 9)
            {
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += value.ToString ();
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += value.ToString ();
				}

            }
            else if (value == 10)
            {    //,
                if (checkLastTextFor(new string[1] { "," }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += ",";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += ",";
				}
            }
            else if (value == 11)
            {   // All Clear
//                userAnswerLaText.text = "";
                
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

				if (userAnswerLaText.text.Length == 0) {
					return;
				}

				if (checkLastTextFor (new string[1] { "^{}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
				} else if (checkLastTextFor (new string[1] { "}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 2);
					userAnswerLaText.text += "}";
					upflag = 1;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				} else if(checkLastTextFor (new string[3] { "\\xalgebra","\\yalgebra","\\zalgebra" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 9);
				}
				else {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}

				// last check to remove ^{} if that's the last part of userAnswer
				if (checkLastTextFor (new string[1] { "^{}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					upflag = 0;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}

            }
            else if (value == 12)
            {   // -
                if (checkLastTextFor(new string[1] { "-" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "-";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "-";
				}
            }
            else if (value == 13)
            {   // +
                if (checkLastTextFor(new string[1] { "+" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "+";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "+";
				}
            }
            else if (value == 14)
            {   // ^
                if (upflag == 0)
                {
					if (checkLastTextFor(new string[1] { "^{}" }))
                    {
                        userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 3);
                    }
					if (checkLastTextFor (new string[1] { "}" })) {
						return;
					}
					userAnswerLaText.text += "^{}";
                    upflag = 1;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
                }
                else if(upflag==1)
                {
                    if (checkLastTextFor(new string[1] { "}" }))
                    {
                        userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                    }
                    userAnswerLaText.text += "}";
                    upflag = 0;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
                }
            }
            else if (value == 15)
            {   // x
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				if (checkLastTextFor(new string[1] { "\\xalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\xalgebra";
            }
            else if (value == 16)
            {   // y
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
                if (checkLastTextFor(new string[1] { "\\yalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\yalgebra";
            }
            else if (value == 17)
            {   // z
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
                if (checkLastTextFor(new string[1] { "\\zalgebra" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 9);
                }
                userAnswerLaText.text += "\\zalgebra";
            }
            else if (value == 18)
            {   // (
                if (checkLastTextFor(new string[1] { "(" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "(";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "(";
				}
            }
            else if (value == 19)
            {   // (
                if (checkLastTextFor(new string[1] { ")" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += ")";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += ")";
				}
            }
            else if (value == 20)
            {   // /
                if (checkLastTextFor(new string[1] { "/" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "/";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "/";
				}
            }
            else if (value == 21)
            {   // =
                if (checkLastTextFor(new string[1] { "=" }))
                {
                    userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
                }
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "=";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "=";
				}
            }
        }
    }
}

