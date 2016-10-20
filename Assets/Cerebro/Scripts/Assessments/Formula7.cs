using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;


namespace Cerebro
{
    public class Formula7 : BaseAssessment
    {

		public TEXDraw subQuestionTEX;
        private string Answer;
		private int coeff1;
		private int coeff2;
		private int coeff3;
		private int coeff4;
		private int randSelector;
		private int PowFlag;
		private int FracNumFlag;
		private int FracDenFlag;
		private int RootFlag;
		private int cursorPos;
		private int nestedFlag;
		private List<string> QuestionList;
		private List<string> AnswerList;
		private bool multiplePossibleAnswers;
		private Dictionary<string,string> numpadKeys;

        void Start()
        {

            StartCoroutine(StartAnimation());
            base.Initialise("M", "FRM07", "S01", "A01");
                                    
            scorestreaklvls = new int[3];
            for (var i = 0; i < scorestreaklvls.Length; i++)
            {
                scorestreaklvls[i] = 0;
            }

            levelUp = false;
                    
            GenerateQuestion();
        }

		public override void SubmitClick()
		{
			PowFlag = 0;
			FracNumFlag = 0;
			FracDenFlag = 0;
			RootFlag = 0;

			if (ignoreTouches || userAnswerLaText.text == "")
			{
				return;
			}
			int increment = 0;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = true;
			CerebroHelper.DebugLog("!" + userAnswerLaText.text + "!");
			CerebroHelper.DebugLog("*" + Answer + "*");
			questionsAttempted++;
			updateQuestionsAttempted();
			float answer = 0;
			float userAns = 0;

			if (multiplePossibleAnswers)
			{
				if (AnswerList [0].Contains ("=")) 
				{
					int iMax = AnswerList.Count;
					for (int i = 0; i < iMax; i++) 
					{
						string[] LHS_RHS = AnswerList [i].Split (new string[] { "=" }, System.StringSplitOptions.None);
						AnswerList.Add (string.Format ("{0}={1}", LHS_RHS [1], LHS_RHS [0]));
					}
				}

				if (AnswerList.Contains (userAnswerLaText.text))
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
				bool directCheck = false;
				if (float.TryParse(Answer, out answer))
				{
					answer = float.Parse(Answer);
				}
				else
				{
					directCheck = true;
				}
				if (float.TryParse(userAnswerLaText.text, out userAns))
				{
					userAns = float.Parse(userAnswerLaText.text);
				}
				else
				{
					correct = false;
				}
				if (answer != userAns)
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


				UpdateStreak (8, 12);

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
                userAnswerLaText.text = "";
                userAnswerLaText.color = MaterialColor.textDark;
                ignoreTouches = false;
            }
            else {
				if (multiplePossibleAnswers)
					userAnswerLaText.text = AnswerList [0];
				else
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
			Answer = "";
			cursorPos = 0;
			nestedFlag = 0;
            answerButton = GeneralButton;
			QuestionLatext.gameObject.SetActive (false);
			subQuestionTEX.gameObject.SetActive (true);
			ChangeNumPadButton (new Dictionary<string, string>(){{"a", "x".Algebra ()}, {"b", "y".Algebra ()}, {"c", "z".Algebra ()}, {"d", "π"}, {"e", "+"}, {"f", "/"}, {"g", "="}});

			AnswerList = new List<string> ();
			multiplePossibleAnswers = false;
			QuestionList = new List<string> ();

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
           
            #region level1
            if (level == 1)
            {
				selector = GetRandomSelector(1, 6);

				multiplePossibleAnswers = true;
				QuestionText.text = "Frame a formula for the given statement.";

				if (selector == 1)
				{
					coeff1 = Random.Range (2, 50);

					subQuestionTEX.text = string.Format ("A number {0} when multiplied by {1} gives {2} as result.", "z".Algebra (), coeff1, "y".Algebra());

					AnswerList.Add (string.Format ("{0}{1}={2}", coeff1, "z".Algebra (), "y".Algebra()));
				}
				else if (selector == 2)
				{
					coeff1 = Random.Range (3, 20);

					subQuestionTEX.text = string.Format ("The age of the mother ({0}) will be double than her daughter ({1}) after {2} years.", "x".Algebra (), "y".Algebra (), coeff1);

					AnswerList.Add (string.Format ("{1}+{0}=2({2}+{0})", coeff1, "x".Algebra (), "y".Algebra()));
					AnswerList.Add (string.Format ("{1}+{0}=2{2}+{3}", coeff1, "x".Algebra (), "y".Algebra(), 2 * coeff1));
					AnswerList.Add (string.Format ("{1}=2{2}+{0}", coeff1, "x".Algebra (), "y".Algebra()));

					ChangeNumPadButton (new Dictionary<string, string>(){{"c", "("}, {"d", ")"}});
				}
				else if (selector == 3)
				{
					coeff1 = 4 * Random.Range (2, 50) + 6;

					subQuestionTEX.text = string.Format ("The sum of four consecutive integers is {0}.\n(assume the smallest integer to be x)", coeff1);

					AnswerList.Add (string.Format ("4{0}+6={1}", "x".Algebra (), coeff1));
					AnswerList.Add (string.Format ("6+4{0}={1}", "x".Algebra (), coeff1));
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range (4, 10);
					coeff2 = Random.Range (2, 10);
					coeff3 = 3 * Random.Range (2, 10) + Random.Range (1, 3);  //should not be a multiple of 3

					subQuestionTEX.text = string.Format ("If you multiply a number '{0}' by {1} and reduce it by {2}, you get {3} more than thrice the number.", "x".Algebra (), coeff1, coeff2, coeff3);

					AnswerList.Add (string.Format ("{1}{0}-{2}={3}+3{0}", "x".Algebra (), coeff1, coeff2, coeff3));
					AnswerList.Add (string.Format ("{1}{0}-{2}=3{0}", "x".Algebra (), (coeff1 + coeff2), coeff3));
				}
				else if (selector == 5)
				{
					QuestionList.Add (string.Format ("The area ({0}) of a triangle is half of the product of its base ({1}) and height ({2}).", "A".Algebra (), "b".Algebra (), "h".Algebra ()));
					QuestionList.Add (string.Format ("The volume ({0}) of a sphere is 4/3 times the product of {1} and {2}^3.", "V".Algebra (), "π".Algebra (), "r".Algebra ()));
					QuestionList.Add (string.Format ("The reciprocal of the focal length ({0}) is equal to the sum of the reciprocals of the object distance ({1}) and the image distance ({2}).", "f".Algebra (), "u".Algebra (), "v".Algebra ()));

					randSelector = Random.Range (0, QuestionList.Count);

					subQuestionTEX.text = QuestionList [randSelector];

					if (randSelector == 0)
					{
						AnswerList.Add ("A=bh/2");
						AnswerList.Add ("A=hb/2");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "h"}, {"c", "A"}});
					}
					else if (randSelector == 1)
					{
						AnswerList.Add ("V=4πr^{3}/3");
						AnswerList.Add ("V=4r^{3}π/3");
						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "^"}, {"b", "r"}, {"c", "V"}});
					}
					else if (randSelector == 2)
					{
						AnswerList.Add ("1/f=1/u+1/v");
						AnswerList.Add ("1/f=1/v+1/u");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "^"}, {"b", "r"}, {"c", "V"}, {"d", "f"}});
					}
				}
			}
			#endregion
			#region level 2
			else if (level == 2)
			{
				selector = GetRandomSelector(1, 10);
			
				multiplePossibleAnswers = false;
				QuestionText.gameObject.SetActive (false);
				QuestionLatext.gameObject.SetActive (true);

				if (selector == 1) // there are 10 questions of this type
				{
					QuestionList.Add (string.Format ("{0}={1}+{2}{3}","v".Algebra (), "u".Algebra (), "a".Algebra (), "t".Algebra ()));
					QuestionList.Add (string.Format ("Volume of a cylider {0}={1}{{{2}}}^2{3}", "V".Algebra (), "π".Algebra(), "r".Algebra (), "h".Algebra ()));
					QuestionList.Add (string.Format ("{0}=\\root{{\\frac{{{1}}}{{{2}}}}}", "m".Algebra (), "n".Algebra (), "p".Algebra ()));
					QuestionList.Add (string.Format ("{0}=\\frac{{{1}{2}{3}}}{{100}}", "I".Algebra (), "P".Algebra (), "R".Algebra (), "T".Algebra ()));
					QuestionList.Add (string.Format ("{0}={1}+({2}-1)+{3}", "l".Algebra (), "a".Algebra (), "n".Algebra (), "d".Algebra ()));
					QuestionList.Add (string.Format ("\\frac{{1}}{{{0}}}=\\frac{{1}}{{{1}}}-\\frac{{1}}{{{2}}}", "f".Algebra (), "u".Algebra (), "v".Algebra ()));
					QuestionList.Add (string.Format ("{0}=2{1}\\root{{\\frac{{{2}}}{{{3}}}}}", "T".Algebra (), "π".Algebra (), "l".Algebra (), "g".Algebra ()));
					QuestionList.Add (string.Format ("{0}=\\frac{{{1}{2}}}{{2}}","A".Algebra (), "b".Algebra (), "h".Algebra ()));
					QuestionList.Add (string.Format ("{0}=\\frac{{{2}{1}}}{{{3}+{2}{4}}}","I".Algebra (), "E".Algebra (), "n".Algebra (), "r".Algebra (), "R".Algebra ()));
					QuestionList.Add (string.Format ("\\frac{{{0}-{1}}}{{{0}+{1}}}={2}", "m".Algebra (), "n".Algebra (), "P".Algebra ()));

					randSelector = Random.Range (0, QuestionList.Count);
					subQuestionTEX.text = QuestionList [randSelector];
					multiplePossibleAnswers = true;

					if (randSelector == 0)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "a".Algebra ());

						AnswerList.Add ("a=\\frac{v-u}{t}");
						AnswerList.Add ("a=-\\frac{u-v}{t}");

						ChangeNumPadButton (new Dictionary<string, string>(){{"b", "v"}, {"c", "u"}, {"d", "t"}, {"f", "Frac"}});
					}
					else if (randSelector == 1)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "r".Algebra ());

						AnswerList.Add ("r=\\root{\\frac{V}{πh}}");
						AnswerList.Add ("r=\\root{\\frac{V}{hπ}}");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "V"}, {"b", "r"}, {"c", "h"}, {"e", "√"}, {"f", "Frac"}});
					} 
					else if (randSelector == 2)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "p".Algebra ());
						subQuestionTEX.text = QuestionList [randSelector];

						AnswerList.Add ("p=\\root{\\frac{n}{{m}^2}}");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "p"}, {"b", "n"}, {"c", "m"}, {"f", "^"}});
					}
					else if (randSelector == 3)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "P".Algebra ());

						AnswerList.Add ("P=\\frac{100I}{RT}");
						AnswerList.Add ("P=100\\frac{I}{RT}");
						AnswerList.Add ("P=I\\frac{100}{RT}");
						AnswerList.Add ("P=\\frac{100}{RT}I");
						AnswerList.Add ("P=\\frac{I}{RT}100");
						AnswerList.Add ("P=\\frac{100I}{TR}");
						AnswerList.Add ("P=100\\frac{I}{TR}");
						AnswerList.Add ("P=\\frac{I}{TR}100");
						AnswerList.Add ("P=I\\frac{100}{TR}");
						AnswerList.Add ("P=\\frac{100}{TR}I");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "P"}, {"b", "I"}, {"c", "R"}, {"d", "T"}, {"f", "Frac"}});
					}
					else if (randSelector == 4)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "d".Algebra ());

						AnswerList.Add ("d=\\frac{l-a}{n-1}");
						AnswerList.Add ("d=\\frac{a-l}{1-n}");
						AnswerList.Add ("d=-\\frac{a-l}{n-1}");
						AnswerList.Add ("d=-\\frac{l-a}{1-n}");

						ChangeNumPadButton (new Dictionary<string, string>(){{"b", "l"}, {"c", "n"}, {"f", "Frac"}});
					}
					else if (randSelector == 5)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "v".Algebra ());

						AnswerList.Add ("v=\\frac{fu}{f-u}");
						AnswerList.Add ("v=\\frac{uf}{f-u}");
						AnswerList.Add ("v=-\\frac{fu}{u-f}");
						AnswerList.Add ("v=-\\frac{uf}{u-f}");
						AnswerList.Add ("v=f\\frac{u}{f-u}");
						AnswerList.Add ("v=u\\frac{f}{f-u}");
						AnswerList.Add ("v=-f\\frac{u}{u-f}");
						AnswerList.Add ("v=-u\\frac{f}{u-f}");
						AnswerList.Add ("v=\\frac{u}{f-u}f");
						AnswerList.Add ("v=\\frac{f}{f-u}u");
						AnswerList.Add ("v=-\\frac{u}{u-f}f");
						AnswerList.Add ("v=-\\frac{f}{u-f}u");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "f"}, {"b", "u"}, {"c", "v"}, {"d", "T"}, {"f", "Frac"}});
					}
					else if (randSelector == 6)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "l".Algebra ());

						AnswerList.Add ("l=\\frac{T^{2}g}{4π^{2}}");
						AnswerList.Add ("l=g\\frac{T^{2}}{4π^{2}}");
						AnswerList.Add ("l=T^{2}\\frac{g}{4π^{2}}");
						AnswerList.Add ("l=\\frac{T^{2}}{4π^{2}}g");
						AnswerList.Add ("l=\\frac{g}{4π^{2}}T^{2}");
						AnswerList.Add ("l=\\frac{gT^{2}}{4π^{2}}");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "T"}, {"b", "l"}, {"c", "g"}, {"e", "^"}, {"f", "Frac"}});
					}
					else if (randSelector == 7)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "h".Algebra ());

						AnswerList.Add ("h=\\frac{2A}{b}");
						AnswerList.Add ("h=2\\frac{A}{b}");
						AnswerList.Add ("h=\\frac{A}{b}2");
						AnswerList.Add ("h=A\\frac{2}{b}");
						AnswerList.Add ("h=\\frac{2}{b}A");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "A"}, {"c", "h"}, {"f", "Frac"}});
					}
					else if (randSelector == 8)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "r".Algebra ());

						AnswerList.Add ("r=n(\\frac{E}{I}-R)");
						AnswerList.Add ("r=\\frac{nE}{I}-nR");
						AnswerList.Add ("r=n\\frac{E}{I}-nR");
						AnswerList.Add ("r=\\frac{n(E-IR)}{I}");
						AnswerList.Add ("r=\\frac{nE-nIR)}{I}");
						AnswerList.Add ("r=n\\frac{(E-IR)}{I}");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "E"}, {"b", "I"}, {"c", "R"}, {"d", "N"}, {"e", "r"}, {"f", "Frac"}, {"g", "-"}});
					}
					else if (randSelector == 9)
					{
						QuestionLatext.text = string.Format ("Make '{0}' as the subject of the formula.", "n".Algebra ());

						AnswerList.Add ("n=m\\frac{1-P}{1+P}");
						AnswerList.Add ("n=\\frac{1-P}{1+P}m");
						AnswerList.Add ("n=\\frac{m-mP}{1+P}");
						AnswerList.Add ("n=\\frac{Pm-m}{1+P}");
						AnswerList.Add ("n=-m\\frac{P-1}{1+P}");
						AnswerList.Add ("n=-\\frac{P-1}{1+P}m");
						AnswerList.Add ("n=-\\frac{mP-m}{1+P}");
						AnswerList.Add ("n=-\\frac{Pm-m}{1+P}");

						ChangeNumPadButton (new Dictionary<string, string>(){{"a", "P"}, {"b", "m"}, {"c", "n"}, {"e", "-"}, {"f", "Frac"}});
					}
				}
				else if (selector == 2)
				{
					QuestionText.gameObject.SetActive (true);
					QuestionLatext.gameObject.SetActive (false);
					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (2, 10);

					while (coeff1 == coeff2)
						coeff1 = Random.Range (2, 10);

					coeff3 = (coeff1 + coeff2) * Random.Range (2, 10); 

					QuestionText.text = "Solve :";
					subQuestionTEX.text = string.Format ("1/{0} of a number when added to 1/{1} of the same number gives {2}. Find the number.", coeff1, coeff2, coeff3);

					Answer = "" + (coeff1 * coeff2 * coeff3) / (coeff1 + coeff2);
				}
				else if (selector == 3)
				{
					coeff1 = Random.Range (2, 100); // u
					coeff2 = Random.Range (2, 100); // v

					while ((coeff1 * coeff2) % (coeff1 + coeff2) != 0 || coeff1 == coeff2)
					{
						coeff1 = Random.Range (2, 100); 
						coeff2 = Random.Range (2, 100);
					}
					coeff3 = (coeff1 * coeff2) / (coeff1 + coeff2);  //f

					QuestionLatext.text = string.Format ("If {2}={4} and {0}={3}, then determine the value of {1} from the given formula.", "u".Algebra (), "v".Algebra (), "f".Algebra (), coeff1, coeff3);
					subQuestionTEX.text = string.Format ("\\frac{{1}}{{{0}}}=\\frac{{1}}{{{1}}}-\\frac{{1}}{{{2}}}", "f".Algebra (), "u".Algebra (), "v".Algebra ());

					Answer = "" + ((coeff1 * coeff2) / (coeff1 + coeff2));
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range (10, 50); // u
					coeff2 = Random.Range (5, 30); // a
					coeff3 = Random.Range (10, 50);  //t
					coeff4 = coeff1 + coeff2 * coeff3; //v

					QuestionLatext.text = string.Format ("Given {0}={4} cm/s, {1}={5} cm/s and {2}={6} s. Determine the value of {3} (in cm/s^2) using the given equation.", "v".Algebra (), "u".Algebra (), "t".Algebra (), "a".Algebra (), coeff4, coeff1, coeff3);
					subQuestionTEX.text = string.Format ("{0}={1}+{2}{3}", "v".Algebra (), "u".Algebra (), "a".Algebra (), "t".Algebra ());

					Answer = "" + coeff2;
				}
				else if (selector == 5)
				{
					coeff1 = 100 * Random.Range (11, 100); // P
					coeff2 = Random.Range (5, 20); // r
					coeff3 = Random.Range (2, 10);  // t
					coeff4 = coeff1 + (coeff1 / 100) * coeff2 * coeff3; //A

					QuestionLatext.text = string.Format ("Find the value of {0} using {1}={4}, {2}={5} and {3}={6}.", "A".Algebra (), "P".Algebra (), "r".Algebra (), "t".Algebra (), coeff1, coeff2, coeff3);
					subQuestionTEX.text = string.Format ("{3}=\\frac{{100({0}-{1})}}{{{1}{2}}}", "A".Algebra (), "P".Algebra (), "r".Algebra (), "t".Algebra ());

					Answer = "" + coeff4;
				}
				else if (selector == 6)
				{
					coeff1 = Random.Range (10, 100); // n
					coeff2 = Random.Range (2, 20); // a
					coeff3 = Random.Range (2, 10);  // d
					coeff4 = coeff1 * coeff2 + (coeff1 * (coeff1 - 1) * coeff3) / 2; // S

					QuestionLatext.text = string.Format ("Determine the value of {3} when {0}={6}, {2}={5} and {1}={4}.", "S".Algebra (), "n".Algebra (), "a".Algebra (), "d".Algebra (), coeff1, coeff2, coeff4);
					subQuestionTEX.text = string.Format ("{0}=\\frac{{{1}}}{{2}}2{2}\\lbrace{{({1}-1){3}}}\\rbrace", "S".Algebra (), "n".Algebra (), "a".Algebra (), "d".Algebra ());

					Answer = "" + coeff3;
				}
				else if (selector == 7)
				{
					coeff1 = 2 * Random.Range (2, 16); // T/π
					coeff2 = 10; // g
					coeff3 = (coeff1 / 2) * (coeff1 / 2) * coeff2;  // l

					QuestionLatext.text = string.Format ("Determine {0} when {1}={4}{3} and {2}={5}}.", "l".Algebra (), "T".Algebra (), "g".Algebra (), "π".Algebra (), coeff1, coeff2);
					subQuestionTEX.text = string.Format ("{0}=\\frac{{{1}}}{{2}}{{2{2}+({1}-1){3}}}", "S".Algebra (), "n".Algebra (), "a".Algebra (), "d".Algebra ());

					Answer = "" + coeff3;
				}
				else if (selector == 8)
				{
					coeff1 = Random.Range (2, 10); 
					coeff2 = Random.Range (2, 10); 
					coeff3 = Random.Range (2, 10); 
					coeff4 = coeff1 + coeff2 + coeff3;
					coeff4 *= coeff4;
						
					QuestionLatext.text = string.Format ("If {0}={4}, {1}={5} and {2}={6}, then find the value of :", "x".Algebra (), "y".Algebra (), "z".Algebra (), coeff1, coeff2, coeff3);
					subQuestionTEX.text = string.Format ("{0}^2+{1}^2+{2}^2+2{0}{1}+2{1}{2}+2{2}{0}", "x".Algebra (), "y".Algebra (), "z".Algebra ());

					Answer = "" + coeff4;
				}
				else if (selector == 9)
				{
					multiplePossibleAnswers = true;

					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (2, 10);
					coeff3 = Random.Range (-10, -2);

					while (coeff1 + coeff2 + coeff3 == 0)
						coeff3 = Random.Range (-10, -2);

					coeff4 = MathFunctions.GetHCF (coeff1 * coeff1 + coeff2 * coeff2 - coeff3, coeff1 + coeff2 + coeff3);

					QuestionLatext.text = string.Format ("If {0}={4}, {1}={5} and {2}={6}, then find the value (in simplest form or round off to 2 decimal places) of :", "l".Algebra (), "p".Algebra (), "q".Algebra (), "r".Algebra (), coeff1, coeff2, coeff3);
					subQuestionTEX.text = string.Format ("\\frac{{{0}^2+{1}^2+{2}^2}}{{{0}+{1}+{2}}}", "p".Algebra (), "q".Algebra (), "r".Algebra ());

					AnswerList.Add ("" + (coeff1 * coeff1 + coeff2 * coeff2 - coeff3)/coeff4 + "/" + (coeff1 + coeff2 + coeff3)/coeff4);
					AnswerList.Add ("" + MathFunctions.GetRounded ((coeff1 * coeff1 + coeff2 * coeff2 - coeff3) / (coeff1 + coeff2 + coeff3), 2));
				}
			}
			#endregion
			userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw>("Text");
            userAnswerLaText.text = "";
            
        }

        public override void numPadButtonPressed (int value)
        {
            if (ignoreTouches)
            {
                return;
            }
			Debug.Log ("cuP, len >>>>>> " + cursorPos + " ,  " + userAnswerLaText.text.Length);
			if (value <= 9)
			{
				if (cursorPos == userAnswerLaText.text.Length)
					userAnswerLaText.text += value.ToString ();
				else
					userAnswerLaText.text = userAnswerLaText.text.Insert (cursorPos, value.ToString ());
				
				cursorPos++;
			} 
			else if (value == 10) 
			{    //Back
				if (userAnswerLaText.text [cursorPos - 1] == '{' || userAnswerLaText.text [cursorPos - 1] == '}') {
					userAnswerLaText.text = "";
					PowFlag = 0;
					FracDenFlag = 0;
					FracNumFlag = 0;
					RootFlag = 0;
					cursorPos = 0;
					nestedFlag = 0;

					if (numpadKeys.ContainsKey("^"))
						numPad.transform.Find ("PanelLayer").Find (numpadKeys["^"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

					if (numpadKeys.ContainsKey("√"))
						numPad.transform.Find ("PanelLayer").Find (numpadKeys["√"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

					if (numpadKeys.ContainsKey("Frac"))
						numPad.transform.Find ("PanelLayer").Find (numpadKeys["Frac"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

				} else {
					cursorPos--;
					userAnswerLaText.text = userAnswerLaText.text.Remove (cursorPos, 1);
				}
			} 
			else if (value == 11) 
			{   // All Clear
				userAnswerLaText.text = "";
				PowFlag = 0;
				FracDenFlag = 0;
				FracNumFlag = 0;
				RootFlag = 0;
				cursorPos = 0;
				nestedFlag = 0;

				if (numpadKeys.ContainsKey("^"))
					numPad.transform.Find ("PanelLayer").Find (numpadKeys["^"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

				if (numpadKeys.ContainsKey("√"))
					numPad.transform.Find ("PanelLayer").Find (numpadKeys["√"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

				if (numpadKeys.ContainsKey("Frac"))
					numPad.transform.Find ("PanelLayer").Find (numpadKeys["Frac"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
			} 
        }

		public void ChangedNumPadButttonPressed (string value)
		{
			Debug.Log ("cuP, len >>>>>>  " + cursorPos + " ,  " + userAnswerLaText.text.Length);
			if (userAnswerLaText.text != "^" && value == "^")
			{   // ^
				if (nestedFlag >= 2)
					return;
				if (PowFlag == 0)
				{
					if (RootFlag == 1 || FracNumFlag == 1 || FracDenFlag == 1)
						nestedFlag++;
					userAnswerLaText.text = userAnswerLaText.text.Insert (cursorPos, "^{}");
					cursorPos += 2;
					PowFlag = 1;
					numPad.transform.Find ("PanelLayer").Find (numpadKeys["^"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				}
				else if (PowFlag == 1)
				{
					cursorPos++;;
					PowFlag = 0;
					nestedFlag--;
					numPad.transform.Find ("PanelLayer").Find (numpadKeys["^"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}
			}
			else if (value == "√")
			{
				if (nestedFlag >= 2)
					return;
				if (RootFlag == 0)
				{
					if (PowFlag == 1 || FracNumFlag == 1 || FracDenFlag == 1)
						nestedFlag++;
					if (cursorPos == userAnswerLaText.text.Length)
						userAnswerLaText.text += "\\root{}";
					else
						userAnswerLaText.text = userAnswerLaText.text.Insert (cursorPos, "\\root{}");
					
					cursorPos += 6;
					RootFlag = 1;
					numPad.transform.Find ("PanelLayer").Find (numpadKeys["√"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				}
				else if (RootFlag == 1)
				{
					cursorPos++;
					RootFlag = 0;
					nestedFlag--;
					numPad.transform.Find ("PanelLayer").Find (numpadKeys["√"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}
			}
			else if (value == "Frac")
			{
				if (nestedFlag >= 2)
					return;
				if (FracNumFlag == FracDenFlag)  
				{
					if (RootFlag == 1 || PowFlag == 1)
						nestedFlag++;
					if (cursorPos == userAnswerLaText.text.Length)
						userAnswerLaText.text += "\\frac{}{}";
					else
						userAnswerLaText.text = userAnswerLaText.text.Insert (cursorPos, "\\frac{}{}");
					
					cursorPos += 6;
					FracNumFlag = 1;
					numPad.transform.Find ("PanelLayer").Find (numpadKeys ["Frac"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				}
				else if (FracNumFlag == 1) 
				{
					cursorPos += 2;
					FracDenFlag = 1;
					FracNumFlag = 0;
					numPad.transform.Find ("PanelLayer").Find (numpadKeys ["Frac"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("9FDFC4");
				}
				else if (FracDenFlag == 1) 
				{
					cursorPos++;
					nestedFlag--;
					FracDenFlag = 0;
					FracNumFlag = 0;
					numPad.transform.Find ("PanelLayer").Find (numpadKeys["Frac"]).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}
			}
			else  {
				if (cursorPos == userAnswerLaText.text.Length)
					userAnswerLaText.text += value.ToString ();
				else
					userAnswerLaText.text = userAnswerLaText.text.Insert (cursorPos, value.ToString ());
				
				cursorPos++;
			}
		}

		public void ChangeNumPadButton (Dictionary<string, string> buttonValues)
		{
			numpadKeys = new Dictionary<string, string> ();
			foreach (var buttonValue in buttonValues) 
			{
				string oldValue = buttonValue.Key;
				string newValue = buttonValue.Value;

				if(!numpadKeys.ContainsKey(newValue))
				{
					numpadKeys.Add(newValue,oldValue);
				}
				if (numPad.transform.Find("PanelLayer").Find (oldValue)) 
				{
					numPad.transform.Find ("PanelLayer").Find (oldValue).Find ("Text").GetComponent<Text> ().text = newValue;
					Button button = numPad.transform.Find ("PanelLayer").Find (oldValue).GetComponent<Button> ();
					button.onClick.RemoveAllListeners ();
					button.onClick.AddListener (() => {
						ChangedNumPadButttonPressed (newValue);
					});
					numPad.transform.Find ("PanelLayer").Find (oldValue).gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}
			}
		}
    }
}
