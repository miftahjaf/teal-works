using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class AlgebraicExpression : BaseAssessment
	{
       
		public Text subQuestionText;
		public TEXDraw QuestionTEX;
		private string Answer;
		private int[] coeff = new int[9];
		private float x, y, z, a, b, c;
		string[] exp = new string[9];
		int[] end = new int[5];
		private int upflag = 0;
		int noOfTerms = 0;
		int round = 0;
		float finalRound = 0;

		void Start ()
		{
			//CerebroHelper.DebugLog("instart");
			StartCoroutine (StartAnimation ());
			base.Initialise ("M", "OAE06", "S01", "A01");

			scorestreaklvls = new int[3];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
			//  SubmitClick();
		}


		override public void SubmitClick ()
		{
            
			bool correct = false;
			if (userAnswerLaText.text == Answer) {
				correct = true;
			} else if (userAnswerLaText.text == "") {
				correct = false;
			} else {
				if (userAnswerLaText.text.Contains ("^{}")) {
					userAnswerLaText.text = userAnswerLaText.text.Replace ("^{}", "");
				}
				if (userAnswerLaText.text.EndsWith ("^{")) {
					string[] tempo = userAnswerLaText.text.Split (new string[] { "^{" }, System.StringSplitOptions.None);
					userAnswerLaText.text = tempo [0];
				}
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");

				for (int i = 0; i < end.Length; i++)
					end [i] = 0;
				char[] userAns = userAnswerLaText.text.ToCharArray ();

				string[] userTerms = SeparateTerms (userAns);
				int Ulast = noOfTerms;
				float[] UserCoeff = SeparateCoeff (userTerms);
				int[,] UserPow = CheckCoeffAndVar (end, userTerms);
				for (int i = 0; i < end.Length; i++) {
					end [i] = 0;
				}
				char[] ans = Answer.ToCharArray ();
				string[] AnsTerms = SeparateTerms (ans);
				int Alast = noOfTerms;
				float[] AnsCoeff = SeparateCoeff (AnsTerms);
				int[,] AnsPow = CheckCoeffAndVar (end, AnsTerms);
          


				for (int i = 0; i < Alast; i++) {                      //AnswerPow
					for (int j = 0; j < Ulast; j++) {                  //UserPow
						if (UserPow [j, 0] == -100)
							continue;
                   
						for (int k = 0; k < 6; k++) {
                   
							if (AnsPow [i, k] == UserPow [j, k]) {
								correct = true;
								if (k == 5) {
									if (Mathf.Abs(UserCoeff [j] - AnsCoeff [i]) <= 0.05f) {
										correct = true;
										for (int m = 0; m < 6; m++)
											UserPow [j, m] = -100;
									} else
										correct = false;
                               
									break;
								}
							} else {
								correct = false;
								break;
							}
                       
						}
					}
				}
			}
			int increment = 0;
			if (correct == true) {
				if (Queslevel == 1) {
					increment = 5;
				} else if (Queslevel == 2) {
					increment = 10;
				} else if (Queslevel == 3) {
					increment = 15;
				} else if (Queslevel == 4) {
					increment = 15;
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

		public string[] SeparateTerms (char[] userAns)
		{

			string[] userTerms = new string[20];
			for (int k = 0; k < 20; k++)
				userTerms [k] = "";
            
			int j = 0;
			for (int i = 0; i < userAns.Length; i++) {                           // seperates terms with an operator
				if (userAns [i] == '+' && i == 0)
					continue;
              
				userTerms [j] = userTerms [j] + userAns [i].ToString ();
				if (i != (userAns.Length - 1))
				if (userAns [i] != '{' && (userAns [i + 1] == '+' || userAns [i + 1] == '-')) {
					userTerms [j] = userTerms [j] + "\0";
					j++;
				}
			}
			noOfTerms = j + 1;
			return userTerms;
         
		}

		public float[] SeparateCoeff (string[] terms)
		{
			float[] coeffs = new float[15];
			string c = "";
			char[] temp;
			bool deci = false, slash = false;
			for (int i = 0; i <= noOfTerms; i++) {                  //to jump from term to term
				temp = terms [i].ToCharArray ();
				//CerebroHelper.DebugLog(temp);
				for (int j = 0; j < temp.Length; j++) {                           //to traverse through terms to get their coeff
					if (temp [j] == '+')
						continue;
					else if (temp [j] == '-')
						c = c + temp [j].ToString ();
					else if (temp [j] == '/') {
						c = c + temp [j].ToString ();
						slash = true;
					} else if (temp [j] == '.') {
						if (temp [j] == 0)
							c = c + "0";
						c = c + temp [j].ToString ();
						deci = true;
					} else if (temp [j] >= '0' && temp [j] <= '9')
						c = c + temp [j].ToString ();
					else {
						end [i] = j;
						break;
					}
				}
				//CerebroHelper.DebugLog(c);
				if (slash) {
					string[] num = c.Split (new string[] { "/" }, System.StringSplitOptions.None);
					int nume = int.Parse (num [0]);
					int denome = int.Parse (num [1]);
					if (denome == 0) {
						terms [i] = "\0";
						break;
					}
					int tempNum = (int)((nume * 100) / denome);
					coeffs [i] = tempNum / 100f;
					CerebroHelper.DebugLog ("FRACTION VALUE = " +  coeffs [i]);

					slash = false;
				} else if (deci) {

					float tempNum = float.Parse (c);
					int tempn = (int)(tempNum * 100);
					coeffs [i] = tempn / 100f;
					deci = false;
				} else if (c == "" || c == "+")
					coeffs [i] = 1;
				else if (c == "-")
					coeffs [i] = -1;
				else
					coeffs [i] = float.Parse (c);

				if (coeffs [i] == 0)
					terms [i] = "\0";
				c = "";
				slash = false;
				deci = false;
			}
			return coeffs;
		}

		public void splitCoeff (string[] terms, int[] end)
		{
			string[] variable = { "x", "y", "z", "a", "b", "c" };
			for (int i = 0; i < terms.Length; i++) {
				for (int j = 0; j < 6; j++) {
					string[] split = terms [i].Split (new string[] { (variable [j]) }, System.StringSplitOptions.None);

					if (split.Length == 1) {
						if (terms [i].Contains (variable [j]))
							terms [i] = terms [i] + "^{1}";
						else
							continue;
					} else {
						if (split [1].StartsWith ("^"))
							continue;
						else
							terms [i] = split [0] + variable [j] + "^{1}" + split [1];
					}
				}
			}
		}

		public int[,] CheckCoeffAndVar (int[] end, string[] terms)
		{
         
			int pow = 0;
			int[,] power = new int[6, 6];
			string[] var2;
			string variable;
			splitCoeff (terms, end);
			for (int i = 0; i < noOfTerms; i++) {   
				string[] var = terms [i].Split (new string[] { "^{" }, System.StringSplitOptions.None);
                
				if (var [0] == "\0")
					continue;
				char[] temp2 = terms [i].ToCharArray ();

			//	CerebroHelper.DebugLog (end [i]);
				if (temp2.Length == end [i])
					continue;
				variable = temp2 [end [i]].ToString ();
				for (int j = 0; j < var.Length - 1; j++) {
					var2 = var [j + 1].Split (new string[] { "}" }, System.StringSplitOptions.None);
					pow = int.Parse (var2 [0]);
                    
					switch (variable) {
					case "a":
						power [i, 0] = pow;
						break;

					case "b":
						power [i, 1] = pow;
						break;
					case "c":
						power [i, 2] = pow;
						break;
					case "x":
						power [i, 3] = pow;
						break;
					case "y":
						power [i, 4] = pow;
						break;
					case "z":
						power [i, 5] = pow;
						break;

					}
					variable = var2 [1];
                   
				}
			}
			return power;
		}

		protected override IEnumerator ShowWrongAnimation ()
		{
			userAnswerLaText.color = MaterialColor.red800;
			Go.to (userAnswerLaText.gameObject.transform, 0.3f, new GoTweenConfig ().shake (new Vector3 (0, 0, 10), GoShakeType.Eulers));
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				userAnswerLaText.text = "";
				userAnswerLaText.color = MaterialColor.textDark;
				ignoreTouches = false;
			} else {
				userAnswerLaText.text = Answer.ToString ();
				userAnswerLaText.color = MaterialColor.green800;
			}
			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation ()
		{
			userAnswerLaText.color = MaterialColor.green800;
			var config = new GoTweenConfig ()
                .scale (new Vector3 (1.1f, 1.1f, 1f))
                .setIterations (2, GoLoopType.PingPong);
			var flow = new GoTweenFlow (new GoTweenCollectionConfig ().setIterations (1));
			var tween = new GoTween (userAnswerLaText.gameObject.transform, 0.2f, config);
			flow.insert (0f, tween);
			flow.play ();
			yield return new WaitForSeconds (1f);
			userAnswerLaText.color = MaterialColor.textDark;

			showNextQuestion ();

			if (levelUp) {
				StartCoroutine (HideAnimation ());
				base.LevelUp ();
				yield return new WaitForSeconds (1.5f);
				StartCoroutine (StartAnimation ());
			}

		}

		protected override void GenerateQuestion ()
		{
			ignoreTouches = false;
			base.QuestionStarted ();
			// Generating the parameters


			answerButton = GeneralButton;
			//subQuestionText.gameObject.SetActive (false);
			int Answertemp = 0;

			level = Queslevel;
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
            //level = 3;

			if (level == 1) {
				selector = GetRandomSelector (1, 12);
                
				if (selector == 1) {	//addition
					for (int i = 0; i < 4; i++)
						coeff [i] = Random.Range (1, 10);

					for (int i = 0; i < 3; i++) {
						if (coeff [i] == coeff [i + 1])
							exp [i] = "x^{2}y^{2}";
						else if (coeff [i + 1] == 1)
							exp [i] = coeff [i].ToString () + "x^{2}y^{2}";
						else
							exp [i] = "\\frac{" + coeff [i] + "}{" + coeff [i + 1] + "}" + "x^{2}y^{2}";
					}

					QuestionLatext.text = exp [0] + ", " + exp [1] + ", " + exp [2];
					round = (int)(((coeff [0] * 100) / coeff [1] + (coeff [1] * 100) / coeff [2] + (coeff [2] * 100) / coeff [3]));
					finalRound = round / 100f;
					Answer = finalRound + "x^{2}y^{2}";
					QuestionText.text = "Add the following:\n ";
				} else if (selector == 2) {
					float[] coeff = new float[4];
					for (int i = 0; i < 4; i++) {
						coeff [i] = Random.Range (-0.9f, 1.0f);
						int temp = (int)(coeff [i] * 100);
						coeff [i] = temp / 100f;
						if (coeff [i] == 0) {
							coeff [i] = 1;
							exp [i] = "ab";
						} else
							exp [i] = coeff [i] + "ab";
					}

					QuestionLatext.text = exp [0] + ", " + exp [1] + ", " + exp [2] + ", " + exp [3];

					round = (int)((coeff [0] + coeff [1] + coeff [2] + coeff [3])*100);
                    finalRound = round / 100f;
					Answer = finalRound.ToString () + "ab";
					QuestionText.text = "Add the following:\n ";
				} else if (selector == 3) {
                      
					for (int i = 0; i < 4; i++) {
						coeff [i] = Random.Range (-9, 0);

						if (coeff [i] == -1)
							exp [i] = "-xy";
						else
							exp [i] = coeff [i] + "xy";
					}

					QuestionLatext.text = exp [0] + ", " + exp [1] + ", " + exp [2] + ", " + exp [3];
					Answertemp = coeff [0] + coeff [1] + coeff [2] + coeff [3];
					Answer = Answertemp.ToString () + "xy";
					QuestionText.text = "Add the following:\n ";
				} else if (selector == 4) {
					float[] coeff = new float[8];
					for (int i = 0; i < 8; i++)
						coeff [i] = (int)Random.Range (1f, 10f);

					for (int i = 0; i < 2; i++) {
                            
						if (coeff [2 * i + 1] == 1)
							coeff [2 * i + 1] = 2;
						if (coeff [2 * i] == coeff [2 * i + 1])
							exp [i] = "x";
						else
							exp [i] = "\\frac{" + coeff [2 * i] + "}{" + coeff [2 * i + 1] + "}" + "x";
					}
					QuestionLatext.text = exp [0] + " from " + exp [1];
					round = (int)((-coeff [0] * 100) / coeff [1] + (coeff [2] * 100) / coeff [3]);
					finalRound = round / 100f;
					Answer = finalRound.ToString () + "x";
					QuestionText.text = "Subtract ";
				} else if (selector == 5) {
					float[] coeff = new float[8];
					float ans = 0;
					for (int i = 0; i < 2; i++) {
						coeff [i] = (int)Random.Range (1, 999);
                            
						coeff [i] = coeff [i] / 100f;
						if (coeff [i] == 0) {
							coeff [i] = 1;
							exp [i] = "xy";
						} else
							exp [i] = coeff [i] + "xy";
					}

					QuestionLatext.text = exp [0] + " from " + exp [1];
					round=(int)(coeff[1]*100 - coeff[0]*100);
					ans = round / 100f;
					Answer = ans.ToString () + "xy";
					QuestionText.text = "Subtract ";
				} else if (selector == 6) {
					float[] coeff = new float[8];
					for (int i = 0; i < 8; i++) {
						coeff [i] = (int)Random.Range (-9f, 10f);
						if (coeff [i] == 0)
							coeff [i] = -1;
					}
					for (int i = 0; i < 4; i++) {
						if (coeff [2 * i + 1] == -1 || coeff [2 * i + 1] == 1)
							coeff [2 * i + 1] = 2;
						if (coeff [2 * i] == coeff [2 * i + 1])
							exp [i] = "x^{2}";
						else if ((coeff [2 * i] * coeff [2 * i + 1]) > 0)
							exp [i] = "\\frac{" + Mathf.Abs (coeff [2 * i]) + "}" + "{" + Mathf.Abs (coeff [2 * i + 1]) + "}" + "x^{2}";
						else {
							exp [i] = "-" + "\\frac{" + Mathf.Abs (coeff [2 * i]) + "}" + "{" + Mathf.Abs (coeff [2 * i + 1]) + "}" + "x^{2}";
						}
                            
					}

					QuestionLatext.text = exp [0] + " from " + exp [1];
					round = (int)((-coeff [0] * 100) / coeff [1] + (coeff [2] * 100) / coeff [3]);
					finalRound = round / 100f;
					Answer = finalRound.ToString () + "x^{2}";
					QuestionText.text = "Subtract ";
				} else if (selector >= 7 && selector <= 9) {            //simplify
					for (int i = 0; i < 7; i++) {
						exp [i] = "";
						coeff [i] = Random.Range (-9, 10);
						if (coeff [i] == -1 || coeff [i] == 0 || coeff [i] == 1) {
							coeff [i] = -1;
							exp [i] = "-";
						} else if (coeff [i] < 0)
							exp [i] = coeff [i].ToString ();
						else if (coeff [i] > 0 && i != 0)
							exp [i] = "+" + coeff [i];
						else
							exp [i] = coeff [i].ToString ();
					}

					if (selector == 7) {
						string varc = "xyz";
						coeff [4] = Random.Range (-9, 10);
						if (coeff [4] >= 0)
							exp [4] = "+" + coeff [4];
						else
							exp [4] = coeff [4].ToString ();
						QuestionLatext.text = exp [0] + varc + exp [1] + varc + exp [2] + varc + exp [3] + varc + exp [4];
						Answertemp = coeff [0] + coeff [1] + coeff [2] + coeff [3];
						Answer = Answertemp.ToString () + varc + exp [4];
					} else if (selector == 8) {
						string varc = "ab";

						QuestionLatext.text = exp [0] + varc + exp [1] + varc + exp [2] + varc;
						Answertemp = coeff [0] + coeff [1] + coeff [2];
						Answer = Answertemp.ToString () + varc;

					} else if (selector == 9) {
						float[] coeff = new float[4];
						float Answertemp2;
						for (int i = 0; i < 3; i++) {
							coeff [i] = Random.Range (-0.9f, 10.0f);
							int temp = (int)(coeff [i] * 100);
							coeff [i] = temp / 100f;

							if (coeff [i] == -1 || coeff [i] == 0 || coeff [i] == 1) {
								coeff [i] = -1;
								exp [i] = "-";
							} else if (coeff [i] > 0 && i != 0)
								exp [i] = "+" + coeff [i];
							else
								exp [i] = coeff [i].ToString ();
						}
						QuestionLatext.text = exp [0] + "x^{2}y" + exp [1] + "x^{2}y" + exp [2] + "x^{2}y";
						Answertemp2 = coeff [0] + coeff [1] + coeff [2];
						Answer = Answertemp2.ToString () + "x^{2}y";

					}
					QuestionText.text = "Simplify the following ";
				} else if (selector >= 10 && selector <= 11) {
					if (selector == 10) {
						string[] varc = { "x", "y", "x^{2}y^{2}" };
						string tempExp1 = "", tempExp2 = "", tempExp3 = "";
						int[] tempAns = new int[3];

                        for (int i = 0; i < 7; i++)
                        {
                            exp[i] = "";
                            coeff[i] = Random.Range(-9, 10);
                            if (coeff[i] == -1 || coeff[i] == 0 || coeff[i] == 1)
                            {
                                coeff[i] = -1;
                                exp[i] = "-";
                            }
                            else if (coeff[i] < 0)
                                exp[i] = coeff[i].ToString();
                            else if (coeff[i] > 0 && i != 0)
                                exp[i] = "+" + coeff[i];
                            else
                                exp[i] = coeff[i].ToString();
                        }

                        tempExp1 = exp [0] + varc [0] + exp [1] + varc [1] + exp [2] + varc [2];
						tempExp2 = exp [3] + varc [0] + exp [4] + varc [1] + exp [5] + varc [2];
						tempAns [0] = coeff [0] + coeff [3];
						tempAns [1] = coeff [1] + coeff [4];
						tempAns [2] = coeff [2] + coeff [5];
						for (int i = 0; i < 3; i++) {
							if (coeff [i + 3] == -1 || coeff[i+3] == 0)
								coeff [i + 3] = 2;
							if (coeff [i] == coeff [i + 3])
								exp [i] = "+" + varc [i];
							else if ((coeff [i + 3] * coeff [i]) > 0) {
								coeff [i] = Mathf.Abs (coeff [i]);
								coeff [i + 3] = Mathf.Abs (coeff [i + 3]);
								exp [i] = "+" + "\\frac{" + coeff [i] + "}{" + coeff [i + 3] + "}" + varc [i];
							} else if ((coeff [i] * coeff [i + 3]) < 0) {
								int co1 = Mathf.Abs (coeff [i]);
								int co2 = Mathf.Abs (coeff [i + 3]);
								exp [i] = "-" + "\\frac{" + co1 + "}{" + co2 + "}" + varc [i];
							}
						}

						tempExp3 = exp [0] + exp [1] + exp [2];
						Answer = "";
						QuestionLatext.text = tempExp1 + tempExp2 + tempExp3;
						int round = 0;
						float finalRound = 0;
                        
                       
                        for (int i = 0; i < 3; i++) {
                            round = (int)((coeff[i] * 100 / coeff[i + 3]));
                            finalRound = round / 100f;
							if ((tempAns [i] + finalRound) > 0)
								Answer = Answer + "+" + (tempAns [i] + finalRound).ToString () + varc [i];
							else if ((tempAns [i] + finalRound) == 0) {
							}
							else
                                Answer = Answer + (tempAns [i] + finalRound).ToString () + varc [i];
							
						}
					} else if (selector == 11) {

						string tempExp1 = "", tempExp2 = "", tempExp3 = "";
						string[] varc = { "a", "b" , "a^{3}b^{3}" };
                        for (int i = 0; i < 7; i++)
                        {
                            exp[i] = "";
                            coeff[i] = Random.Range(-9, 10);
                            if (coeff[i] == -1 || coeff[i] == 0 || coeff[i] == 1)
                            {
                                coeff[i] = -1;
                                exp[i] = "-";
                            }
                            else if (coeff[i] < 0)
                                exp[i] = coeff[i].ToString();
                            else if (coeff[i] > 0 && i != 0)
                                exp[i] = "+" + coeff[i];
                            else
                                exp[i] = coeff[i].ToString();
                        }
                        tempExp1 = exp [0] + varc [0] + exp [1] + varc [1] + exp [2] + varc[2];
						tempExp2 = exp [3] + varc [0] + exp [4] + varc [1] + exp [5] + "b^{3}a^{3}";
						tempExp3 = Random.Range (1, 10).ToString ();
						QuestionLatext.text = tempExp1 + tempExp2 + " - " + tempExp3;
						Answer = "";
						for (int i = 0; i < 3; i++) {
							if ((coeff [i] + coeff [i + 3]) > 0)
								Answer = Answer + "+" + (coeff [i] + coeff [i + 3]).ToString () + varc [i];
							else if ((coeff [i] + coeff [i + 3]) == 0) {
							}
							else
								Answer = Answer + (coeff [i] + coeff [i + 3]).ToString () + varc [i];
						}
                       
                        Answer = Answer + "-"+tempExp3;
					}
					QuestionText.text = "Simplify the following: ";
				}
                

			}/********END OF LEVEL 1********/

			if (level == 2) {
				string[] var = new string[4];
				string tempExp1 = "", tempExp2 = "";
				string[] AnswerTemp = new string[3];

				selector = GetRandomSelector (1, 3);
				if (selector >= 1 && selector <= 2) {                      //SUBTRACT
					if (selector == 1) {
						var [0] = "x^{2}";
						var [1] = "y^{2}";
						var [2] = "xy";

						for (int i = 0; i < 6; i++) {
							coeff [i] = Random.Range (-9, 10);
							if (coeff [i] == -1 || coeff [i] == 0)
								coeff [i] = -1;
							if (coeff [i] > 0) {
								if (coeff [i] == 1) {
									if ((i % 3) == 0)
										exp [i] = var [i % 3];
									else
										exp [i] = "+" + var [i % 3];
								} else {
									if ((i % 3) == 0)
										exp [i] = coeff [i] + var [i % 3];
									else
										exp [i] = "+" + coeff [i] + var [i % 3];
								}
							} else {
								if (coeff [i] == -1)
									exp [i] = "-" + var [i % 3];
								else
									exp [i] = coeff [i] + var [i % 3];

							}
						}
						tempExp1 = exp [0] + exp [1] + exp [2];
						tempExp2 = exp [3] + exp [4] + exp [5];
						//QuestionLatext.text = tempExp1 + " from " + tempExp2;
						QuestionLatext.text = tempExp1 + " from " + tempExp2;

						for (int i = 0; i < 3; i++) {
							if ((coeff [i + 3] - coeff [i]) == 0)
								AnswerTemp [i] = "";
							else if ((coeff [i + 3] - coeff [i]) == 1)
								AnswerTemp [i] = "+" + var [i];
							else if ((coeff [i + 3] - coeff [i]) == -1)
								AnswerTemp [i] = "-" + var [i];
							else if ((coeff [i + 3] - coeff [i]) > 0)
								AnswerTemp [i] = "+" + (coeff [i + 3] - coeff [i]).ToString () + var [i];
							else
								AnswerTemp [i] = (coeff [i + 3] - coeff [i]).ToString () + var [i];
						}
					} else if (selector == 2) {
						float[] coeff = new float[6];

						var [0] = "x^{2}";
						var [1] = "y^{2}";
						var [2] = "z^{2}";

						for (int i = 0; i < 6; i++) {
							coeff [i] = (int)Random.Range (-999, 999);
							coeff [i] = coeff [i] / 100f;
							if (coeff [i] == -1.0f || coeff [i] == 0)
								coeff [i] = -1.0f;
							if (coeff [i] > 0) {
								if (coeff [i] == 1.0f) {
									if ((i % 3) == 0)
										exp [i] = var [i % 3];
									else
										exp [i] = "+" + var [i % 3];
								} else {
									if ((i % 3) == 0)
										exp [i] = coeff [i] + var [i % 3];
									else
										exp [i] = "+" + coeff [i] + var [i % 3];
								}
							} else if (coeff [i] < 0) {
								if (coeff [i] == -1.0f)
									exp [i] = "-" + var [i % 3];
								else
									exp [i] = coeff [i].ToString () + var [i % 3];

							}
						}
						tempExp1 = exp [0] + exp [1] + exp [2];
						tempExp2 = exp [3] + exp [4] + exp [5];
						//QuestionLatext.text = tempExp1 + " from " + tempExp2;
						QuestionLatext.text = tempExp1 + " from " + tempExp2;

						for (int i = 0; i < 3; i++) {
							if ((coeff [i + 3] - coeff [i]) == 0)
								AnswerTemp [i] = "";
							else if ((coeff [i + 3] - coeff [i]) == 1)
								AnswerTemp [i] = "+" + var [i];
							else if ((coeff [i + 3] - coeff [i]) == -1)
								AnswerTemp [i] = "-" + var [i];
							else if ((coeff [i + 3] - coeff [i]) > 0)
								AnswerTemp [i] = "+" + (coeff [i + 3] - coeff [i]).ToString () + var [i];
							else
								AnswerTemp [i] = (coeff [i + 3] - coeff [i]).ToString () + var [i];
						}
					}

                    
					Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2];

					QuestionText.text = " Subtract ";
				} else if (selector == 3) {                      //SIMPLIFY

					var [0] = "y";
					var [1] = "x";
					var [2] = "x";
					var [3] = "y";

					for (int i = 0; i < 4; i++) {
						coeff [i] = Random.Range (-9, 10);
						if (coeff [i] == -1 || coeff [i] == 0)
							coeff [i] = -1;
						if (coeff [i] > 0) {
							if (coeff [i] == 1) {
								if ((i % 2) == 0)
									exp [i] = var [i];
								else
									exp [i] = "+" + var [i];
							} else {
								if ((i % 2) == 0)
									exp [i] = coeff [i] + var [i];
								else
									exp [i] = "+" + coeff [i] + var [i];
							}
						} else if (coeff [i] < 0) {
							if (coeff [i] == -1)
								exp [i] = "-" + var [i];
							else
								exp [i] = coeff [i] + var [i];

						}
					}

					tempExp1 = exp [0] + exp [1];
					tempExp2 = exp [2] + exp [3];
					int j = 0;
					for (int i = 0; i < 2; i++) {
						if (i == 0)
							j = 3;
						if (i == 1)
							j = 2;
						if ((coeff [i] - coeff [j]) == 0)
							AnswerTemp [i] = "";
						else if ((coeff [i] - coeff [j]) == 1 && i != 0)
							AnswerTemp [i] = "+" + var [i];
						else if ((coeff [i] - coeff [j]) == -1)
							AnswerTemp [i] = "-" + var [i];
						else if ((coeff [i] - coeff [j]) > 0 && i != 0)
							AnswerTemp [i] = "+" + (coeff [i] - coeff [j]).ToString () + var [i];
						else
							AnswerTemp [i] = (coeff [i] - coeff [j]).ToString () + var [i];
					}
					Answer = AnswerTemp [0] + AnswerTemp [1];

					QuestionLatext.text = " (" + tempExp1 + ")" + " - " + "(" + tempExp2 + ")";
                    
					QuestionText.text = " Simplify ";
				} else if (selector >= 4 && selector <= 5) {                                      //TAKE AWAY
					var [0] = "x";
					var [1] = "y";
					var [2] = "z";
					int value = Random.Range (0, 3);
					int value2;
					float[] coeff = new float[7];
					for (int i = 0; i < 6; i++)
						exp [i] = "";

					for (int i = 0; i < 7; i++)
						do {
							coeff [i] = Random.Range (-9.9f, 10.0f);
							int temp = (int)(coeff [i] * 100);
							coeff [i] = temp / 100f;
						} while (coeff [i] == 0);
					selector = Random.Range (1, 3);

					if (selector == 4) {
						if (value != 0)
							exp [value] = "+";
						exp [value] = exp [value] + "\\frac{" + "0" + "}" + "{" + var [value] + "}";
						int oppValue = value + 3;
                       
						coeff [oppValue] = (int)coeff [oppValue];
						if (coeff [oppValue] >= 0 && oppValue != 3)
							exp [oppValue] = "+";
                        
						exp [oppValue] = exp [oppValue] + coeff [oppValue].ToString () + var [oppValue % 3];

                        
						do {                                                                      //for another variable
							value2 = Random.Range (0, 2);
						} while (value2 == value);
						coeff [value2] = (int)coeff [value2];
						if (coeff [value2] > 0 && value2 != 0)
							exp [value2] = "+";

						exp [value2] = exp [value2] + coeff [value2].ToString () + var [value2];

						int oppValue2 = value2 + 3;
						coeff [oppValue2] = (int)coeff [oppValue2];
						if (coeff [oppValue2] > 0 && oppValue2 != 3)
							exp [oppValue2] = "+";

						exp [oppValue2] = exp [oppValue2] + coeff [oppValue2].ToString () + var [oppValue2 % 3];

						int i = 0;
						while (true) {                                                        //for last variable
							if (i != value && i != value2)
								break;
							i++;
						}
                       
						if (coeff [i] > 0 && i != 0)
							exp [i] = "+";
						exp [i] = exp [i] + coeff [i].ToString () + var [i];

						coeff [i + 3] = (int)coeff [i + 3];
						coeff [6] = (int)coeff [6];

						if (coeff [i + 3] == coeff [6])
							exp [i + 3] = "+" + var [(i + 3) % 3];
						else if (coeff [6] == 1) {
							if (coeff [i + 3] > 0)
								exp [i + 3] = "+";
							exp [i + 3] = exp [i + 3] + coeff [i + 3] + var [(i + 3) % 3];
						} else if ((coeff [i + 3] * coeff [6]) > 0)
							exp [i + 3] = "+" + "\\frac{" + Mathf.Abs (coeff [i + 3]) + "}" + "{" + Mathf.Abs (coeff [6]) + "}" + var [(i + 3) % 3];
						else
							exp [i + 3] = "-" + "\\frac{" + Mathf.Abs (coeff [i + 3]) + "}" + "{" + Mathf.Abs (coeff [6]) + "}" + var [(i + 3) % 3];
                       

						for (int j = 0; j < 3; j++) {
							AnswerTemp [j] = "";
							if (j == (value))
								coeff [j] = coeff [oppValue];
							else if (j == i)
								coeff [j] = ((int)((coeff [i + 3] * 100) / coeff [6] - (coeff [i] * 100))) / 100f;
							else
								coeff [j] = coeff [j + 3] - coeff [j];

							if (coeff [j] == 0)
								AnswerTemp [j] = "";
							else if (coeff [j] == 1)
								AnswerTemp [j] = "+" + var [j];
							else if (coeff [j] == -1)
								AnswerTemp [j] = "-" + var [j];
							else if (coeff [j] > 0)
								AnswerTemp [j] = "+" + coeff [j].ToString () + var [j];
							else
								AnswerTemp [j] = coeff [j].ToString () + var [j];
						}
                
						Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2];

						QuestionLatext.text = exp [0] + exp [1] + exp [2] + " from " + exp [3] + exp [4] + exp [5];

					} else if (selector == 5) {
						coeff [2] = (int)Random.Range (-9.0f, 0.0f);
						int temp = (int)(coeff [2] * 100);
						coeff [2] = temp / 100.0f;
						coeff [5] = (int)Mathf.Abs (coeff [2]);

						for (int i = 0; i < 6; i++) {
							coeff [i] = (int)coeff [i];
							int temp2 = (int)(coeff [i] * 100);
							coeff [i] = temp2 / 100.0f;
							if (coeff [i] == -1 || coeff [i] == 0)
								coeff [i] = -1;
							if (coeff [i] > 0) {
								if (coeff [i] == 1)
									exp [i] = "+";
								else
									exp [i] = "+" + coeff [i];
							} else {
								if (coeff [i] == -1)
									exp [i] = "-";
								else
									exp [i] = coeff [i].ToString ();

							}
						}
						exp [0] = exp [0] + "a^{0}";
						exp [1] = exp [1] + "b^{1}";
						exp [2] = exp [2] + "ab";
						exp [3] = exp [3] + "a^{2}";
						exp [4] = exp [4] + "b^{1}";
						exp [5] = exp [5] + "ab";
                        
						if ((coeff [4] - coeff [1]) == 0)
							AnswerTemp [1] = "";
						else if ((coeff [4] - coeff [1]) == 1)
							AnswerTemp [1] = "+" + "b";
						else if ((coeff [4] - coeff [1]) == -1)
							AnswerTemp [1] = "-" + "b";
						else if ((coeff [4] - coeff [1]) > 0)
							AnswerTemp [1] = "+" + (coeff [4] - coeff [1]).ToString () + "b";
						else
							AnswerTemp [1] = (coeff [4] - coeff [1]).ToString () + "b";

						if ((coeff [5] - coeff [2]) == 0)
							AnswerTemp [2] = "";
						else if ((coeff [5] - coeff [2]) == 1)
							AnswerTemp [2] = "+" + "ab";
						else if ((coeff [5] - coeff [2]) == -1)
							AnswerTemp [2] = "-" + "ab";
						else if ((coeff [5] - coeff [2]) > 0)
							AnswerTemp [2] = "+" + (coeff [5] - coeff [2]).ToString () + "ab";
						else
							AnswerTemp [2] = (coeff [5] - coeff [2]).ToString () + "ab";

                      
                    
						QuestionLatext.text = exp [0] + exp [1] + exp [2] + " from " + exp [3] + exp [4] + exp [5];
						Answer = (0 - coeff [0]).ToString () + AnswerTemp [1] + AnswerTemp [2] + exp [3];
					}
					QuestionText.text = "Take away ";

				} else if (selector == 6) {                                           //WHAT MUST BE ADDED?
					var [0] = "x";
					var [1] = "y";
					var [2] = "z";

					for (int i = 0; i < 6; i++) {
						coeff [i] = Random.Range (-9, 10);
						if (coeff [i] == -1 || coeff [i] == 0)
							coeff [i] = -1;
						if (coeff [i] > 0) {
							if (coeff [i] == 1) {
								if ((i % 3) == 0)
									exp [i] = var [i % 3];
								else
									exp [i] = "+" + var [i % 3];
							} else {
								if ((i % 3) == 0)
									exp [i] = coeff [i] + var [i % 3];
								else
									exp [i] = "+" + coeff [i] + var [i % 3];
							}
						} else if (coeff [i] < 0) {
							if (coeff [i] == -1)
								exp [i] = "-" + var [i % 3];
							else
								exp [i] = coeff [i] + var [i % 3];

						}
					}

					tempExp1 = exp [0] + exp [1] + exp [2];
					tempExp2 = exp [3] + exp [4] + exp [5];
					QuestionLatext.text = tempExp1 + " to get " + tempExp2;
					QuestionText.text = "What must be added to ";
					for (int j = 0; j < 3; j++) {
						if ((coeff [j + 3] - coeff [j]) == 0)
							AnswerTemp [j] = "";
						else if ((coeff [j + 3] - coeff [j]) == 1)
							AnswerTemp [j] = "+" + var [j];
						else if ((coeff [j + 3] - coeff [j]) == -1)
							AnswerTemp [j] = "-" + var [j];
						else if ((coeff [j + 3] - coeff [j]) > 0)
							AnswerTemp [j] = "+" + (coeff [j + 3] - coeff [j]).ToString () + var [j];
						else
							AnswerTemp [j] = (coeff [j + 3] - coeff [j]).ToString () + var [j];
					}

					Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2];
				} else if (selector == 7) {                                          //WHAT MUST BE SUBTRACTED?
					float[] coeff = new float[5];
					var [0] = "x^{2}";
					var [1] = "y^{2}";
					var [2] = "z^{2}";

					for (int i = 0; i < 4; i++) {
						if ((i % 2) == 1) {
							coeff [i] = Random.Range (-0.9f, 1.0f);
							int temp = (int)(coeff [i] * 100);
							coeff [i] = temp / 100f;
						} else
							coeff [i] = (int)Random.Range (-9, 10);

						if (coeff [i] == 0)
							exp [i] = "";
						if (coeff [i] > 0) {
							if (coeff [i] == 1) {
								if ((i % 2) == 0)
									exp [i] = var [i % 2];
								else
									exp [i] = "+" + var [i % 2];
							} else {
								if ((i % 2) == 0)
									exp [i] = coeff [i] + var [i % 2];
								else
									exp [i] = "+" + coeff [i] + var [i % 2];
							}
						} else if (coeff [i] < 0) {
							if (coeff [i] == -1)
								exp [i] = "-" + var [i % 2];
							else
								exp [i] = coeff [i] + var [i % 2];

						}
					}
					tempExp1 = exp [0] + exp [1];
					tempExp2 = exp [2] + exp [3];
					for (int j = 0; j < 2; j++) {
						if ((coeff [j] - coeff [j + 2]) == 0)
							AnswerTemp [j] = "";
						else if ((coeff [j] - coeff [j + 2]) == 1)
							AnswerTemp [j] = "+" + var [j];
						else if ((coeff [j] - coeff [j + 2]) == -1)
							AnswerTemp [j] = "-" + var [j];
						else if ((coeff [j] - coeff [j + 2]) > 0)
							AnswerTemp [j] = "+" + (coeff [j] - coeff [j + 2]).ToString () + var [j];
						else
							AnswerTemp [j] = (coeff [j] - coeff [j + 2]).ToString () + var [j];
					}
					coeff [0] = (int)(coeff [0] + coeff [1]);
					coeff [1] = (int)(coeff [2] + coeff [3]);
					coeff [2] = (int)((coeff [2] + coeff [3]) / 2);
					coeff [3] = (int)((coeff [0] + coeff [1]) / 2);

					for (int i = 0; i < 2; i++) {
						if (coeff [2 * i] == 0)
							exp [i + 4] = "";
						if (coeff [2 * i + 1] == 0)
							coeff [2 * i + 1] = 2;
						else if (coeff [2 * i] == coeff [2 * i + 1])
							exp [i + 4] = "+" + "z^{2}";
						else if (((coeff [2 * i] * coeff [2 * i + 1]) < 0) && (coeff [2 * i] == Mathf.Abs (coeff [2 * i + 1])) || (coeff [2 * i + 1] == Mathf.Abs (coeff [2 * i])))
							exp [i + 4] = "-" + "z^{2}";
						else if ((coeff [2 * i] * coeff [2 * i + 1]) > 0) {
							coeff [2 * i] = Mathf.Abs (coeff [2 * i]);
							coeff [2 * i + 1] = Mathf.Abs (coeff [2 * i + 1]);
							exp [i + 4] = "+" + "\\frac{" + coeff [2 * i] + "}{" + coeff [2 * i + 1] + "}" + "z^{2}";
						} else
							exp [i + 4] = "-" + "\\frac{" + Mathf.Abs (coeff [2 * i]) + "}{" + Mathf.Abs (coeff [2 * i + 1]) + "}" + "z^{2}";

					}

					tempExp1 = tempExp1 + exp [4];
					tempExp2 = tempExp2 + exp [5];
					int round = 0;
					float finalRound = 0;
					round = (int)((coeff [0] * 100) / coeff [1] - (coeff [2] * 100) / coeff [3]);
					finalRound = round / 100f;
					if (finalRound > 0)
						AnswerTemp [2] = "+";
					else
						AnswerTemp [2] = "";
					AnswerTemp [2] = AnswerTemp [2] + finalRound.ToString () + "z^{2}";
					QuestionLatext.text = tempExp1 + " to get " + tempExp2;
					Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2];

					QuestionText.text = "What must be subtracted from ";

				} else if (selector == 8) {                                      //HOW MUCH SHOULD
					var [0] = "x^{2}";
					var [1] = "y^{2}";
					var [2] = "z^{2}";
					coeff [6] = Random.Range (-9, 10);

					for (int i = 0; i < 6; i++) {
						if (i < 3)
							coeff [i] = Random.Range (-9, 10);
						else
							coeff [i] = coeff [6] * coeff [i % 3];
						if (coeff [i] == 0)
							coeff [i] = -1;
						if (coeff [i] > 0) {
							if (coeff [i % 3] == 1) {
								if ((i % 3) == 0)
									exp [i] = var [i % 3];
								else
									exp [i] = "+" + var [i % 3];
							} else {
								if ((i % 3) == 0)
									exp [i] = coeff [i] + var [i % 3];
								else
									exp [i] = "+" + coeff [i] + var [i % 3];
							}
						} else if (coeff [i] < 0) {
							if (coeff [i] == -1)
								exp [i] = "-" + var [i % 3];
							else
								exp [i] = coeff [i] + var [i % 3];
						}
					}

					tempExp1 = exp [0] + exp [1] + exp [2];
					tempExp2 = exp [3] + exp [4] + exp [5];
					QuestionLatext.text = tempExp1 + " be to get " + tempExp2;
					Answer = coeff [6].ToString ();
					QuestionText.text = "How much should  ";

				}
				/*************END OF SUBLEVEL 6*************/
			} ///********************END OF LEVEL 2********************///


			else if (level == 3) {              //multiply
				string[] AnswerTemp = new string[3];
				string[] var = new string[3];
				selector = GetRandomSelector (1, 21);
               // selector = 20;
				if (selector >= 1 && selector <= 5) {
					int[] expon = new int[6];

					for (int i = 0; i < 6; i++)
						expon [i] = Random.Range (0, 5);
					
					if (selector == 1) {
						float[] coeff = new float[4];
						for (int i = 0; i < 2; i++) {
							exp [i] = "x^{" + expon [3 * i] + "}" + "y^{" + expon [3 * i + 1] + "}" + "z^{" + expon [3 * i + 2] + "}";
							coeff [2 * i] = (int)Random.Range (-9.0f, 10.0f);
							coeff [2 * i + 1] = (int)Random.Range (-9.0f, 10.0f);
							int temp = (int)(coeff [2 * i] * 100);
							coeff [2 * i] = temp / 100f;
							int temp2 = (int)(coeff [2 * i + 1] * 100);
							coeff [2 * i + 1] = temp2 / 100f;

							if (coeff [2 * i + 1] == 0)
								coeff [2 * i + 1] = 1;
							if (coeff [2 * i] == 0)
								exp [i] = "0";
							else if (coeff [2 * i] == coeff [2 * i + 1])
								continue;
							else if (((coeff [2*i] * coeff [2*i + 1]) < 0) && (coeff [2*i] == Mathf.Abs (coeff [2*i + 1])) || (coeff [2*i + 1] == Mathf.Abs (coeff [2*i])))
								exp [i] = "-" + exp [i];
							else if ((coeff [2 * i] * coeff [2 * i + 1]) > 0) {
								coeff [2 * i] = Mathf.Abs (coeff [2 * i]);
								coeff [2 * i + 1] = Mathf.Abs (coeff [2 * i + 1]);
								exp [i] = "\\frac{" + coeff [2 * i] + "}{" + coeff [2 * i + 1] + "}" + exp [i];
							} else {
								exp [i] = "-" + "\\frac{" + Mathf.Abs (coeff [2 * i]) + "}{" + Mathf.Abs (coeff [2 * i + 1]) + "}" + exp [i];
							}

						}
						int round = 0;
						float finalRound = 0;
						round = (int)(((coeff [0] * 100) / coeff [1]) * ((coeff [2]) / coeff [3]));
						finalRound = round / 100f;
						Answer = finalRound.ToString () + "x^{" + (expon [0] + expon [3]) + "}" + "y^{" + (expon [1] + expon [4]) + "}" + "z^{" + (expon [2] + expon [5]) + "}";
						QuestionLatext.text = exp [0] + " and " + exp [1];
					}

					else if (selector == 2) {
						float[] coeff = new float[4];
						int keytemp = Random.Range (1, 4);
						var [0] = "x";
						var [1] = "y";
						var [2] = "z";
						coeff [0] = (int)Random.Range (-9.0f, 10.0f);
						exp [0] = coeff [0] + var [keytemp % 3];

						QuestionLatext.text = exp [0] + " and " + "0";
						Answer = "0";

					}

					else if (selector == 3) {
						float[] coeff = new float[4];
						int keytemp = Random.Range (1, 3);
						var [0] = "x";
						var [1] = "y";
						var [2] = "z";
						coeff [0] = (int)Random.Range (-9.0f, 10.0f);
						coeff [1] = (int)Random.Range (-9.0f, 10.0f);
						exp [0] = coeff [0] + var [keytemp];
						exp [1] = coeff [1] + var [keytemp];

						QuestionLatext.text = exp [0] + " and " + exp [1];
						Answer = (coeff [0] * coeff [1]).ToString () + var [keytemp] + "^{2}";
					}

					else if (selector == 4) {
						float[] coeff = new float[4];
						int keytemp = Random.Range (0, 3);
						var [0] = "x";
						var [1] = "y";
						var [2] = "z";
						coeff [0] = (int)Random.Range (-9.0f, 10.0f);
						coeff [1] = (int)Random.Range (-9.0f, 10.0f);
						exp [0] = coeff [0] + var [keytemp];
						exp [1] = coeff [1] + var [(keytemp + 1) % 3];

						QuestionLatext.text = exp [0] + " and " + exp [1];
						Answer = (coeff [0] * coeff [1]).ToString () + var [keytemp] + var [(keytemp + 1) % 3];
					}

					else if (selector == 5) {
						float[] coeff = new float[4];
						coeff [0] = Random.Range (-0.9f, 1.0f);
						int temp = (int)(coeff [0] * 100);
						coeff [0] = temp / 100.0f;
						coeff [1] = (int)Random.Range (1.0f, 10.0f);
						coeff [2] = (int)Random.Range (1.0f, 10.0f);
						int key0 = Random.Range (1, 4);
						int key1 = Random.Range (1, 4);
						if (coeff [1] == coeff [2])
							exp [1] = "x^{" + key0 + "}";
						else if (coeff [2] == 1)
							exp [1] = coeff [1] + "x^{" + key0 + "}";
						else
							exp [1] = "\\frac{" + coeff [1] + "}" + "{" + coeff [2] + "}" + "x^{" + key0 + "}";
						if (coeff [0] == 0) {
							exp [0] = "x^{" + key1 + "}";
							coeff [0] = 1;
						} else
							exp [0] = coeff [0] + "x^{" + key1 + "}";
						key0 = key0 + key1;
						QuestionLatext.text = exp [0] + " and " + exp [1];
						int round = 0;
						float finalRound = 0;
						round = (int)((coeff [0] * 100 * coeff [1]) / coeff [2]);
						finalRound = round / 100f;
						Answer = finalRound + "x^{" +   key0 + "}";//


					}

					QuestionText.text = "Multiply ";
				}

				int[] key = new int[6];
				int[] power = new int[6];
				string[] var1 = { "x", "y", "z", "a", "b", "c" };
            
				for (int i = 0; i < 6; i++)
					coeff [i] = Random.Range (1, 10);

				for (int i = 0; i < 6; i++) {
					key [i] = Random.Range (0, 4);
					power [i] = Random.Range (1, 5);
				}

				if (selector >= 6 && selector <= 9) {              //MULTIPLY
					if (selector == 6) {

						exp [0] = coeff [0] + var1 [key [0]] + "^{" + power [0] + "}" + "-" + coeff [1] + var1 [(key [0] + 1) % 3] + "^{" + power [1] + "}";
						exp [1] = coeff [2] + var1 [key [0]] + "^{" + power [2] + "}" + var1 [(key [0] + 1) % 3] + "^{" + power [3] + "}" + var1 [(key [0] + 2) % 3] + "^{" + power [4] + "}";
						QuestionLatext.text = exp [0] + " by " + exp [1];
						AnswerTemp [0] = (coeff [0] * coeff [2]).ToString () + var1 [key [0]] + "^{" + (power [0] + power [2]) + "}" + var1 [(key [0] + 1) % 3] + "^{" + power [3] + "}" + var1 [(key [0] + 2) % 3] + "^{" + power [4] + "}";
						AnswerTemp [1] = "-" + (coeff [1] * coeff [2]).ToString () + var1 [(key [0] + 1) % 3] + "^{" + (power [3] + power [1]) + "}" + var1 [key [0]] + "^{" + power [2] + "}" + var1 [(key [0] + 2) % 3] + "^{" + power [4] + "}";
						Answer = AnswerTemp [0] + AnswerTemp [1];
					}

					else if (selector == 7) {
						string[] expTmp = new string[3];

						for (int i = 0; i < 6; i++)
							key [i] = Random.Range (0, 5);

						for (int i = 0; i < 2; i++) {
							if (coeff [2 * i] == coeff [2 * i + 1])
								expTmp [i] = "+" + var1 [(key [0] + i) % 3] + "^{2}" + var1 [(key [0] + i + 1) % 3];
							else if (coeff [2 * i + 1] == 1) {
								coeff [2 * i] = Mathf.Abs (coeff [2 * i]);
								expTmp [i] = "+" + Mathf.Abs (coeff [2 * i]).ToString () + var1 [(key [0] + i) % 3] + "^{2}" + var1 [(key [0] + i + 1) % 3];
							} else if ((coeff [2 * i] * coeff [2 * i + 1]) > 0) {
								int co1 = Mathf.Abs (coeff [2 * i]);
								int co2 = Mathf.Abs (coeff [2 * i + 1]);
								expTmp [i] = "+" + "\\frac{" + co1 + "}" + "{" + co2 + "}" + var1 [(key [0] + i) % 3] + "^{2}" + var1 [(key [0] + i + 1) % 3];
							} else {
								int co1 = Mathf.Abs (coeff [2 * i]);
								int co2 = Mathf.Abs (coeff [2 * i + 1]);
								expTmp [i] = "-" + "\\frac{" + co1 + "}" + "{" + co2 + "}" + var1 [(key [0] + i) % 3] + "^{2}" + var1 [(key [0] + i + 1) % 3];
							}

						}
						coeff [4] = Random.Range (2, 20);
						expTmp [2] = coeff [4] + var1 [key [0] % 3] + var1 [(key [0] + 1) % 3] + var1 [(key [0] + 2) % 3];

						int round = 0;
						float finalRound = 0;
						round = (int)((coeff [0] * 100 * coeff [4]) / coeff [1]);
						finalRound = round / 100f;

						AnswerTemp [0] = finalRound.ToString () + var1 [key [0] % 3] + "^{3}" + var1 [(key [0] + 1) % 3] + "^{2}" + var1 [(key [0] + 2) % 3];

						round = (int)((coeff [2] * coeff [4] * 100) / coeff [3]);
						finalRound = round / 100f;
						AnswerTemp [1] = finalRound.ToString () + var1 [(key [0] + 2) % 3] + "^{2}" + var1 [(key [0] + 1) % 3] + "^{3}" + var1 [key [0] % 3];

						if ((coeff [2] * coeff [4] / coeff [3]) > 0)
							Answer = AnswerTemp [0] + "+" + AnswerTemp [1];
						else
							Answer = AnswerTemp [0] + AnswerTemp [1];
						QuestionLatext.text = expTmp [0] + expTmp [1] + " by " + expTmp [2];

					}
					else if (selector == 8) {
						for (int i = 0; i < 6; i++)
							power [i] = Random.Range (2, 9);
						var [0] = "x";
						var [1] = "y";
						var [2] = "z";
						exp [0] = "(" + var [key [0] % 3] + "^{" + power [0] + "}" + var [(key [0] + 1) % 3] + "^{" + power [1] + "}" + " - " + var [(key [0] + 2) % 3] + "^{" + power [2] + "}" + ")";
						exp [1] = "(" + var [key [0] % 3] + "^{" + power [0] + "}" + var [(key [0] + 1) % 3] + "^{" + power [1] + "}" + " + " + var [(key [0] + 2) % 3] + "^{" + power [2] + "}" + ")";
						QuestionLatext.text = exp [0] + " and " + exp [1];
						Answer = var [key [0] % 3] + "^{" + (power [0] * 2) + "}" + var [(key [0] + 1) % 3] + "^{" + (power [1] * 2) + "}" + "-" + var [(key [0] + 2) % 3] + "^{" + (power [2] * 2) + "}";
					}

					else if (selector == 9) {
						var [0] = "x";
						var [1] = "y";
						var [2] = "z";
						exp [0] = "(" + coeff [0] + var [key [0] % 3] + "^{" + power [0] + "}" + " + " + coeff [1] + var [(key [0] + 1) % 3] + "^{" + power [1] + "}" + ")";
						exp [1] = "(" + coeff [2] + var [key [0] % 3] + "^{" + power [2] + "}" + " + " + coeff [3] + var [(key [0] + 1) % 3] + "^{" + power [3] + "}" + var [key [0] % 3] + "^{" + power [3] + "}" + ")";
						QuestionLatext.text = exp [0] + " and " + exp [1];
						AnswerTemp [0] = (coeff [0] * coeff [2]).ToString () + var [key [0] % 3] + "^{" + (power [0] + power [2]) + "}";
						AnswerTemp [0] = AnswerTemp [0] + "+" + (coeff [0] * coeff [3]).ToString () + var [key [0] % 3] + "^{" + (power [0] + power [3]) + "}" + var [(key [0] + 1) % 3] + "^{" + power [3] + "}";
						AnswerTemp [0] = AnswerTemp [0] + "+" + (coeff [1] * coeff [3]).ToString () + var [(key [0] + 1) % 3] + "^{" + (power [1] + power [3]) + "}" + var [key [0] % 3] + "^{" + power [3] + "}";
						Answer = AnswerTemp [0] + "+" + (coeff [1] * coeff [2]).ToString () + var [(key [0] + 1) % 3] + "^{" + power [1] + "}" + var [key [0] % 3] + "^{" + power [2] + "}";
					}

					QuestionText.text = "Multiply ";
				}
				/*************END OF SUBLEVEL 2***************/

				if (selector >= 10 && selector <= 17) {               //DIVIDE
					var [0] = "x";
					var [1] = "y";
					var [2] = "z";
					if (selector == 10) {
                        
						for (int i = 0; i < 2; i++) {

							coeff [i] = Random.Range (1, 100);
							if (coeff [i] == 1)
								exp [i] = "" + var [key [0] % 3] + "^{" + power [i] + "}" + var [(key [0] + 1) % 3] + "^{" + power [i + 1] + "}" + var [(key [0] + 2) % 3] + "^{" + power [i + 2] + "}";
							else
								exp [i] = coeff [i].ToString () + var [key [0] % 3] + "^{" + power [i] + "}" + var [(key [0] + 1) % 3] + "^{" + power [i + 1] + "}" + var [(key [0] + 2) % 3] + "^{" + power [i + 2] + "}";
						}
						QuestionLatext.text = exp [0] + " by " + exp [1];
                        round = (int)((coeff[0] * 100) / coeff[1]);
                        finalRound = round / 100f;
						Answer = finalRound.ToString() + var [key [0] % 3] + "^{" + (power [0] - power [1]) + "}" + var [(key [0] + 1) % 3] + "^{" + (power [1] - power [2]) + "}" + var [(key [0] + 2) % 3] + "^{" + (power [2] - power [3]) + "}";

					}


					else if (selector == 11) {
						if (coeff [0] == 1)
							coeff [0] = 2;
						exp [0] = coeff [0].ToString () + var [key [0] % 3] + "^{" + power [0] + "}";
						exp [1] = coeff [0].ToString ();
						QuestionLatext.text = exp [0] + " by " + exp [1];
						Answer = var [key [0] % 3] + "^{" + power [0] + "}";
					}

					else if (selector == 12) {
						exp [0] = var [key [0] % 3] + "^{0}";
						exp [1] = var [(key [0] + 1) % 3] + "^{0}";
						QuestionLatext.text = exp [0] + " by " + exp [1];
						Answer = "1";
					}

					else if (selector == 13) {
						for (int i = 0; i < 2; i++) {
							if (coeff [2 * i] == coeff [2 * i + 1])
								exp [i] = "" + var [key [0] % 3] + "^{" + power [i] + "}" + var [(key [0] + 1) % 3] + "^{" + power [i + 1] + "}" + var [(key [0] + 2) % 3] + "^{" + power [i + 2] + "}";
							else if (coeff [2 * i + 1] == 1)
								exp [i] = coeff [2 * i].ToString () + var [key [0] % 3] + "^{" + power [i] + "}" + var [(key [0] + 1) % 3] + "^{" + power [i + 1] + "}" + var [(key [0] + 2) % 3] + "^{" + power [i + 2] + "}";
							else
								exp [i] = "\\frac{" + coeff [2 * i] + "}" + "{" + coeff [2 * i + 1] + "}" + var [key [0] % 3] + "^{" + power [i] + "}" + var [(key [0] + 1) % 3] + "^{" + power [i + 1] + "}" + var [(key [0] + 2) % 3] + "^{" + power [i + 2] + "}";
                            
						}
						QuestionLatext.text = exp [0] + " by " + exp [1];
						int round = 0;
						float finalRound = 0;
						round = (int)((coeff [0] * coeff [3] * 100) / (coeff [1] * coeff [2]));
						finalRound = round / 100f;
						Answer = finalRound + var [key [0] % 3] + "^{" + (power [0] - power [1]) + "}" + var [(key [0] + 1) % 3] + "^{" + (power [1] - power [2]) + "}" + var [(key [0] + 2) % 3] + "^{" + (power [2] - power [3]) + "}";
					}
					else if (selector == 14) {
						// int temp;
						for (int i = 0; i < 2; i++) {
							if (coeff [2 * i] == 1)
								coeff [2 * i] = 2;
							if (coeff [2 * i] == coeff [2 * i + 1])
								exp [i] = "" + "\\frac{" + var [key [0] % 3] + "^{" + power [2 * i] + "}" + "}" + "{" + var [(key [0] + 1) % 3] + "^{" + power [2 * i + 1] + "}" + "}";
							else if (coeff [2 * i + 1] == 1)
								exp [i] = coeff [2 * i].ToString () + "\\frac{" + var [key [0] % 3] + "^{" + power [2 * i] + "}" + "}" + "{" + var [(key [0] + 1) % 3] + "^{" + power [2 * i + 1] + "}" + "}";
							else
								exp [i] = "\\frac{" + coeff [2 * i] + var [key [0] % 3] + "^{" + power [2 * i] + "}" + "}" + "{" + coeff [2 * i + 1] + var [(key [0] + 1) % 3] + "^{" + power [2 * i + 1] + "}" + "}";

						}
						QuestionLatext.text = exp [0] + " by " + exp [1];
						round = (int)((coeff [0] * coeff [3] * 100) / (coeff [1] * coeff [2]));
						//CerebroHelper.DebugLog (round);
						finalRound = round / 100f;
                        
						Answer = finalRound + var [(key [0]) % 3] + "^{" + (power [0] - power [2]) + "}" + var [(key [0] + 1) % 3] + "^{" + (-power [1] + power [3]) + "}";
					}

					else if (selector == 15) {
						if (coeff [0] == 1)
							coeff [0] = 2;
						if (coeff [0] == coeff [1])
							exp [0] = "" + "\\frac{" + var [key [0] % 3] + "^{" + power [0] + "}" + var [(key [0] + 1) % 3] + "^{" + power [1] + "}" + "}" + "{" + var [(key [0] + 2) % 3] + "^{" + power [2] + "}" + "}";
						else if (coeff [1] == 1)
							exp [0] = coeff [0].ToString () + "\\frac{" + var [key [0] % 3] + "^{" + power [0] + "}" + var [(key [0] + 1) % 3] + "^{" + power [1] + "}" + "}" + "{" + var [(key [0] + 2) % 3] + "^{" + power [2] + "}" + "}";
						else
							exp [0] = "\\frac{" + coeff [0] + var [key [0] % 3] + "^{" + power [0] + "}" + var [(key [0] + 1) % 3] + "^{" + power [1] + "}" + "}" + "{" + coeff [1] + var [(key [0] + 2) % 3] + "^{" + power [2] + "}" + "}";

						do {
							coeff [2] = Random.Range (-20, 0);
						} while (coeff [2] != -1);

						exp [1] = coeff [2].ToString () + var [key [0] % 3] + "^{" + power [3] + "}" + var [(key [0] + 2) % 3] + "^{" + power [4] + "}";
						QuestionLatext.text = exp [0] + " by " + exp [1];
						round = (int)((coeff [0] * 100) / (coeff [2] * coeff [1]));
						finalRound = round / 100f;
						Answer = finalRound + var [key [0] % 3] + "^{" + (power [0] - power [3]) + "}" + var [(key [0] + 1) % 3] + "^{" + power [1] + "}" + var [(key [0] + 2) % 3] + "^{" + (-power [4] - power [2]) + "}";
						//   string var2 = "";
					}

					else if (selector == 16) {

						coeff [5] = Random.Range (2, 10);
						for (int i = 0; i < 5; i++) {
							//  string var2 = "nikita";
							if (i == 3)
								continue;
							if (i != 4)
								exp [i] = (coeff [5] * coeff [i]).ToString () + var1 [key [0]] + "^{" + (power [0] + i + 1) + "}" + var1 [(key [0] + 1)] + "^{" + (power [1] + i + 1) + "}" + var1 [(key [0] + 2)] + "^{" + (power [2] + i + 1) + "}";
							else
								exp [i] = (-1 * coeff [5]).ToString () + var1 [key [0]] + "^{" + power [0] + "}" + var1 [(key [0] + 1)] + "^{" + power [1] + "}" + var1 [(key [0] + 2)] + "^{" + power [2] + "}";

						}
						exp [3] = (coeff [5] * coeff [3]).ToString ();
						for (int i = 0; i < 3; i++) {
							if ((coeff [i] * -1) > 0)
								AnswerTemp [i] = "+" + (coeff [i] * -1) + var1 [key [0]] + "^{" + (power [0] + i + 1) + "}" + var1 [(key [0] + 1)] + "^{" + (power [1] + i + 1) + "}" + var1 [(key [0] + 2)] + "^{" + (power [2] + i + 1) + "}";
							else
								AnswerTemp [i] = (coeff [i] * -1) + var1 [key [0]] + "^{" + (power [0] + i + 1) + "}" + var1 [(key [0] + 1)] + "^{" + (power [1] + i + 1) + "}" + var1 [(key [0] + 2)] + "^{" + (power [2] + i + 1) + "}";
						}
						Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2] + (coeff [3] * -1).ToString ();
						QuestionLatext.text = exp [0] + " + " + exp [1] + " + " + exp [2] + " + " + exp [3] + " by " + "-" + coeff [5].ToString ();
					}

					QuestionText.text = "Divide ";

				}

				if (selector == 17) {                       //PRODUCT OF
                    for (int i = 0; i < 3; i++)
                    {
                        coeff[i] = Random.Range(2, 10);
                        power[i] = Random.Range(1, 10);
                    }
					exp [0] = coeff [0].ToString () + var1 [key [0]] + "^{" + power [0] + "}" + var1 [(key [0] + 1)] + "^{" + power [1] + "}" + " + " + coeff [1].ToString () + var1 [(key [0] + 1)] + "^{" + power [1] + "}" + var1 [(key [0] + 2)] + "^{" + power [2] + "}";
					exp [1] = coeff [2].ToString () + var1 [key [0]] + "^{" + power [0] + "}" + var1 [(key [0] + 1)] + "^{" + power [1] + "}" + var1 [(key [0] + 2)] + "^{" + power [2] + "}" + " + " + coeff [3];

					AnswerTemp [0] = (coeff [0] * coeff [2]) + var1 [key [0]] + "^{" + (2 * power [0]) + "}" + var1 [(key [0] + 1)] + "^{" + (2 * power [1]) + "}" + var1 [(key [0] + 2)] + "^{" + power [2] + "}";
					AnswerTemp [1] = "+" + (coeff [1] * coeff [2]) + var1 [(key [0] + 1)] + "^{" + (2 * power [1]) + "}" + var1 [(key [0] + 2)] + "^{" + (2 * power [2]) + "}" + var1 [key [0]] + "^{" + power [0] + "}";
					AnswerTemp [2] = "+" + (coeff [0] * coeff [3]).ToString () + var1 [key [0]] + "^{" + power [0] + "}" + var1 [(key [0] + 1)] + "^{" + power [1] + "}";
					AnswerTemp [2] = AnswerTemp [2] + "+" + (coeff [1] * coeff [3]).ToString () + var1 [(key [0] + 1)] + "^{" + power [1] + "}" + var1 [(key [0] + 2)] + "^{" + power [2] + "}";
					Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2];

					QuestionLatext.text = exp [0] + " and " + exp [1];
					QuestionText.text = "Determine the product of ";
				}

				if (selector >= 18 && selector <= 19) {                                                           //GREATER THAN OR LESSER THAN
					for (int i = 0; i < 6; i++) {
						while (coeff [i] == 0)
							coeff [i] = Random.Range (-9, 10);
						if (coeff [i] == 1)
						if (i == 0)
							exp [i] = var1 [(key [0] + i) % 3] + "^{" + power [i % 3] + "}";
						else
							exp [i] = "+" + var1 [(key [0] + i) % 3] + "^{" + power [i % 3] + "}";
						else if (coeff [i] == -1)
							exp [i] = "-" + var1 [(key [0] + i) % 3] + "^{" + power [i % 3] + "}";
						else if (coeff [i] > 0)
							exp [i] = "+" + coeff [i] + var1 [(key [0] + i) % 3] + "^{" + power [i % 3] + "}";
						else
							exp [i] = coeff [i] + var1 [(key [0] + i) % 3] + "^{" + power [i % 3] + "}";
					}

					if (selector == 18) {
						QuestionLatext.text = exp [0] + exp [1] + exp [2] + " greater than " + exp [3] + exp [4] + exp [5];
						for (int i = 0; i < 3; i++) {
							if ((coeff [i] - coeff [i + 3]) > 0)
								AnswerTemp [i] = "+" + (coeff [i] - coeff [i + 3]) + var1 [(key [0] + i) % 3] + "^{" + power [i] + "}";
							else
								AnswerTemp [i] = (coeff [i] - coeff [i + 3]) + var1 [(key [0] + i) % 3] + "^{" + power [i] + "}";
						}
						Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2];
					}
					else if (selector == 19) {
						QuestionLatext.text = exp [0] + exp [1] + exp [2] + " lesser than " + exp [3] + exp [4] + exp [5];
						for (int i = 0; i < 3; i++) {
							if ((coeff [i + 3] - coeff [i]) > 0)
								AnswerTemp [i] = "+" + (coeff [i + 3] - coeff [i]) + var1 [(key [0] + i) % 3] + "^{" + power [i] + "}";
							else
								AnswerTemp [i] = (coeff [i + 3] - coeff [i]) + var1 [(key [0] + i) % 3] + "^{" + power [i] + "}";
						}
						Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2];
					}
					QuestionText.text = "How much is ";

				}
				/***********END OF SUBLEVEL 5***************/

				if (selector == 20) {
					int pow = 2;
					for (int i = 0; i < 6; i++)
						while (coeff [i] == 0)
							coeff [i] = Random.Range (-9, 10);
					for (int i = 0; i < 3; i++) {
						if (i == 1) {
							var1 [(key [0] + i) % 3] = var1 [(key [0] + 2) % 3] + var1 [key [0] % 3];
							pow = 1;
						}
						if (coeff [i] == 1)
						if (i == 0)
							exp [i] = var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						else
							exp [i] = "+" + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						else if (coeff [i] == -1)
							exp [i] = "-" + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						else if (coeff [i] > 0)
							exp [i] = "+" + coeff [i] + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						else
							exp [i] = coeff [i] + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						pow = 2;
					}
                    
					exp [0] = exp [0] + exp [1] + exp [2];

					string[] expTmp = new string[3];
					for (int i = 0; i < 3; i++) {
						if (i == 1) {
							var1 [(key [0] + i) % 3] = var1 [(key [0] + 2) % 3] + var1 [key [0] % 3];
							pow = 1;
						}
						if (coeff [2 * i] == coeff [2 * i + 1])
							expTmp [i] = "+" + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						else if ((coeff [2 * i] * coeff [2 * i + 1]) > 0) {
							int co1 = Mathf.Abs (coeff [2 * i]);
							int co2 = Mathf.Abs (coeff [2 * i + 1]);
							expTmp [i] = "+" + "\\frac{" + co1 + "}" + "{" + co2 + "}" + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						} else {
							int co1 = Mathf.Abs (coeff [2 * i]);
							int co2 = Mathf.Abs (coeff [2 * i + 1]);
							expTmp [i] = "-" + "\\frac{" + co1 + "}" + "{" + co2 + "}" + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						}
						pow = 2;
					}

					exp [1] = expTmp [0] + expTmp [1] + expTmp [2];
					QuestionLatext.text = exp [0] + " and if one of them is " + exp [1];
					QuestionText.text = "Find the other expression, if the sum of two algebraic expressions is ";
					for (int i = 0; i < 3; i++) {
						round = (int)((coeff [i] * 100 - ((coeff [2 * i] * 100) / coeff [2 * i + 1])));
						finalRound = round / 100f;
						if (i == 1) {
							var1 [(key [0] + i) % 3] = var1 [(key [0] + 2) % 3] + var1 [key [0] % 3];
							pow = 1;
						}
						if (round > 0)
							AnswerTemp [i] = "+" + finalRound + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						else
							AnswerTemp [i] = finalRound + var1 [(key [0] + i) % 3] + "^{" + pow + "}";
						pow = 2;
					}
					Answer = AnswerTemp [0] + AnswerTemp [1] + AnswerTemp [2];
				}
			}

			//CerebroHelper.DebugLog (Answer);
			userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw> ("Text");
			userAnswerLaText.text = "";
		}

		public override void numPadButtonPressed (int value)
		{
			if (ignoreTouches) {
				return;
			}
			if (value <= 9) {
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += value.ToString ();
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += value.ToString ();
				}

			} else if (value == 10) {    //,
				if (checkLastTextFor (new string[1] { "/" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "/";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "/";
				}
			} else if (value == 11) {   // All Clear
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
				} else {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}

				// last check to remove ^{} if that's the last part of userAnswer
				if (checkLastTextFor (new string[1] { "^{}" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					upflag = 0;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}

			} else if (value == 12) {   // -
				if (checkLastTextFor (new string[1] { "-" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "-";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "-";
				}
			} else if (value == 13) {   // +
				if (checkLastTextFor (new string[1] { "+" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "+";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "+";
				}
			} else if (value == 14) {   // ^
				if (upflag == 0) {
					if (checkLastTextFor (new string[1] { "^{}" })) {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 3);
					}
					if (!(checkLastTextFor (new string[1] { "x" }) || checkLastTextFor (new string[1] { "y" }) || checkLastTextFor (new string[1] { "z" }) || checkLastTextFor (new string[1] { "a" }) || checkLastTextFor (new string[1] { "b" }) || checkLastTextFor (new string[1] { "c" }))) {
						return;
					}
					userAnswerLaText.text += "^{}";
					upflag = 1;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("6464DC");
				} else if (upflag == 1) {
					if (checkLastTextFor (new string[1] { "}" })) {
						userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					}
					userAnswerLaText.text += "}";
					upflag = 0;
					numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				}
			} else if (value == 15) {   // x
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				if (checkLastTextFor (new string[1] { "x" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += "x";
			} else if (value == 16) {   // y
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				if (checkLastTextFor (new string[1] { "y" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += "y";
			} else if (value == 17) {   // z
				upflag = 0;
				numPad.transform.Find ("PanelLayer").Find ("^").gameObject.GetChildByName<Image> ("Image").color = CerebroHelper.HexToRGB ("191923");
				if (checkLastTextFor (new string[1] { "z" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += "z";
			} else if (value == 18) {   // (
				if (checkLastTextFor (new string[1] { "a" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "a";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "a";
				}
			} else if (value == 19) {   // (
				if (checkLastTextFor (new string[1] { "b" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "b";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "b";
				}
			} else if (value == 20) {   // /
				if (checkLastTextFor (new string[1] { "c" })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += "c";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += "c";
				}
			} else if (value == 21) {   // =
				if (checkLastTextFor (new string[1] { "." })) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
				}
				if (upflag == 1) {
					userAnswerLaText.text = userAnswerLaText.text.Substring (0, userAnswerLaText.text.Length - 1);
					userAnswerLaText.text += ".";
					userAnswerLaText.text += "}";
				} else {
					userAnswerLaText.text += ".";
				}
			}
		}
	}
}