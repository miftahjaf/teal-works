using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class RatioAndProportionScript : BaseAssessment {

		public Text subQuestionText;

		private string Answer;

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "RAP06", "S01", "A01");

			scorestreaklvls = new int[4];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = "";
			GenerateQuestion ();
		}
			
		public override void SubmitClick(){
			if (ignoreTouches || userAnswerText.text == "") {
				return;
			}
			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = true;

			questionsAttempted++;
			updateQuestionsAttempted ();

			var answerSplits = Answer.Split (new string[] { ":" }, System.StringSplitOptions.None);
			var userAnswerSplits = userAnswerText.text.Split (new string[] { ":" }, System.StringSplitOptions.None);

			if (answerSplits.Length == userAnswerSplits.Length) {
				for (var i = 0; i < answerSplits.Length; i++) {
					float answer = 0;
					float userAnswer = 0;

					if (float.TryParse (answerSplits [i], out answer)) {
						answer = float.Parse (answerSplits [i]);
					} else {
						correct = false;
						break;
					}
					if (float.TryParse (userAnswerSplits [i], out userAnswer)) {
						userAnswer = float.Parse (userAnswerSplits [i]);
					} else {
						correct = false;
						break;
					}
					if (answer != userAnswer) {
						correct = false;
						break;
					}
				}
			} else {
				correct = false;
			}

			if (correct == true) {
				if (Queslevel == 1) {
					increment = 5;
				} else if (Queslevel == 2) {
					increment = 5;
				} else if (Queslevel == 3) {
					increment = 10;
				} else if (Queslevel == 4) {
					increment = 15;
				} else if (Queslevel == 5) {
					increment = 15;
				}

				UpdateStreak(8,12);

				updateQuestionsAttempted ();
				StartCoroutine (ShowCorrectAnimation());
			} 
			else {
				for (var i = 0; i < scorestreaklvls.Length; i++) {
					scorestreaklvls [i] = 0;
				}
				StartCoroutine (ShowWrongAnimation());
			}

			base.QuestionEnded (correct, level, increment, selector);

		}

		protected override IEnumerator ShowWrongAnimation() {
			userAnswerText.color = MaterialColor.red800;
			Go.to( userAnswerText.gameObject.transform, 0.5f, new GoTweenConfig().shake( new Vector3( 0, 0, 20 ), GoShakeType.Eulers ) );
			yield return new WaitForSeconds (0.5f);
			if (isRevisitedQuestion) {
				userAnswerText.text = "";
				userAnswerText.color = MaterialColor.textDark;
				ignoreTouches = false;
			} else {
				userAnswerText.text = Answer.ToString ();
				userAnswerText.color = MaterialColor.green800;
			}
			ShowContinueButton ();
		}

		protected override IEnumerator ShowCorrectAnimation() {
			userAnswerText.color = MaterialColor.green800;
			var config = new GoTweenConfig ()
				.scale (new Vector3 (1.1f, 1.1f, 1f))
				.setIterations( 2, GoLoopType.PingPong );
			var flow = new GoTweenFlow( new GoTweenCollectionConfig().setIterations( 1 ) );
			var tween = new GoTween( userAnswerText.gameObject.transform, 0.2f, config );
			flow.insert( 0f, tween );
			flow.play ();
			yield return new WaitForSeconds (1f);
			userAnswerText.color = MaterialColor.textDark;

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

			level = Queslevel;

			answerButton = GeneralButton;
			subQuestionText.gameObject.SetActive (false);

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			var num1 = Random.Range (2, 10);
			var num2 = Random.Range (2, 10);
			while (num1 == num2) {
				num2 = Random.Range (2, 10);
			}
			var commonFactor = MathFunctions.GetHCF (num1, num2);
			num1 = num1 / commonFactor;
			num2 = num2 / commonFactor;
			var multiplier = Random.Range (2, 10);
			var n1 = num1 * multiplier;
			var n2 = num2 * multiplier;

			if (level == 1) {
				selector = GetRandomSelector (1, 4);

				if (selector == 1) {												//simplify the ratio to simplest form
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express in lowest terms: ";
					subQuestionText.text = n1.ToString () + " : " + n2.ToString ();
					Answer = num1.ToString () + ":" + num2.ToString ();
				} else if (selector == 2) {											//simplify the ratio to simplest form - Decimals
					subQuestionText.gameObject.SetActive (true);
					float nd1 = (float)n1 / 10;
					float nd2 = (float)n2 / 10;
					QuestionText.text = "Express in lowest terms: ";
					subQuestionText.text = nd1.ToString () + " : " + nd2.ToString ();
					Answer = num1.ToString () + ":" + num2.ToString ();
				} else if (selector == 3) {    										// Simple Word Problems
					string[] arr1 = new string[3]{ "potato", "pen", "shirt" };
					string[] arr2 = new string[3]{ "tomato", "pencil", "pant" };
					subQuestionText.gameObject.SetActive (false);
					var rand1 = Random.Range (0, arr1.Length);
					if (Random.Range (0, 2) == 0) {
						QuestionText.text = "A " + arr1 [rand1] + " costs Rs. " + n1.ToString () + ".\nA " + arr2 [rand1] + " costs Rs. " + n2.ToString () + ".\nFind the ratio of the costs of " + arr1 [rand1] + " and " + arr2 [rand1] + ".";
						Answer = num1.ToString () + ":" + num2.ToString ();
					} else {
						QuestionText.text = "A " + arr1 [rand1] + " costs Rs. " + n1.ToString () + ".\nA " + arr2 [rand1] + " costs Rs. " + n2.ToString () + ".\nFind the ratio of the costs of " + arr2 [rand1] + " and " + arr1 [rand1] + ".";
						Answer = num2.ToString () + ":" + num1.ToString ();
					}
				}
			} 

			else if (level == 2) {			
				selector = GetRandomSelector (1, 6);

				if (selector == 1) {    										// Word Problems, given nx:ny:nz, find x:y:z
					subQuestionText.gameObject.SetActive (false);
					string[] arr1 = new string[3]{ "Kohli", "Dhoni", "Dravid" };
					string[] arr2 = new string[3]{ "Gilchrist", "McCullum", "Amla" };
					string[] arr3 = new string[3]{ "Root", "Sangakkara", "Ponting" };
					var num3 = Random.Range (2, 10);
					var n3 = num3 * multiplier;
					var rand1 = Random.Range (0, arr1.Length);
					var rand2 = Random.Range (0, arr2.Length);
					var rand3 = Random.Range (0, arr3.Length);

					QuestionText.text = arr1 [rand1] + " scored " + n1.ToString () + " runs in a match, " + arr2 [rand2] + " scored " + n2.ToString () + " runs and " + arr3 [rand3] + " scored " + n3.ToString () + " runs. What is the ratio of the runs scored by " + arr1 [rand1] + ", " + arr2 [rand2] + " and " + arr3 [rand3] + "?";
					Answer = num1.ToString () + ":" + num2.ToString () + ":" + num3.ToString ();
				} else if (selector == 2) {											//ratio given, x given, find y
					string[] arr1 = new string[3]{ "copper", "gold", "platinum" };
					string[] arr2 = new string[3]{ "silver", "zinc", "aluminium" };
					var rand1 = Random.Range (0, arr1.Length);
					var rand2 = Random.Range (0, arr2.Length);
					QuestionText.text = "If the ratio of " + arr1 [rand1] + " to " + arr2 [rand2] + " in an alloy is " + num1.ToString () + " : " + num2.ToString () + ". If the weight of " + arr1 [rand1] + " is " + n1.ToString () + " g. Find the weight (in g) of " + arr2 [rand2] + " in the alloy.";  
					Answer = n2.ToString ();
				} else if (selector == 3) {    										// x:y:z given, total given, find z
					subQuestionText.gameObject.SetActive (false);
					var num3 = Random.Range (2, 10);
					var n3 = num3 * multiplier;
					var total = n3 + n1 + n2;
					var qrnd = Random.Range (0, 3);
					if (qrnd == 0) {
						QuestionText.text = "Rs. " + total.ToString () + " is to be divided amongst three people in the ratio of " + num1.ToString () + " : " + num2.ToString () + " : " + num3.ToString () + ". How much did the first person get?";
						Answer = n1.ToString ();
					} else if (qrnd == 1) {
						QuestionText.text = "Rs. " + total.ToString () + " is to be divided amongst three people in the ratio of " + num1.ToString () + " : " + num2.ToString () + " : " + num3.ToString () + ". How much did the second person get?";
						Answer = n2.ToString ();
					} else if (qrnd == 2) {
						QuestionText.text = "Rs. " + total.ToString () + " is to be divided amongst three people in the ratio of " + num1.ToString () + " : " + num2.ToString () + " : " + num3.ToString () + ". How much did the last person get?";
						Answer = n3.ToString ();
					}

				} else if (selector == 4) {											// Ratio of 3 numbers
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Express in lowest terms: ";
					var num3 = Random.Range (2, 10);
					var commonFactor2 = MathFunctions.GetHCF (num1, num3);
					var commonFactor3 = MathFunctions.GetHCF (num2, num3);
					if (commonFactor2 == commonFactor3) {
						num1 = num1 / commonFactor2;
						num2 = num2 / commonFactor2;
						num3 = num3 / commonFactor2;
						n1 = num1 * multiplier;
						n2 = num2 * multiplier;
					}
					var n3 = num3 * multiplier;
					subQuestionText.text = n1.ToString () + " : " + n2.ToString () + " : " + n3.ToString ();
					Answer = num1.ToString () + ":" + num2.ToString () + ":" + num3.ToString ();
				} else if (selector == 5) {											//Decimals + Measurement Word Problems
					subQuestionText.gameObject.SetActive (true);
					var rndnum = Random.Range (0, 3);
					if (rndnum == 0) {           //Kilograms
						int nd1 = n1 / 10;
						int nd2 = n2 / 10;
						int ndm1 = n1 % 10;
						int ndm2 = n2 % 10;

						QuestionText.text = "Express in lowest terms: ";
						subQuestionText.text = nd1.ToString () + " kg " + (ndm1 * 100).ToString () + " gm " + " : " + nd2.ToString () + " kg " + (ndm2 * 100).ToString () + " gm ";
					} else if (rndnum == 1) {           //Meters
						int nd1 = n1 / 10;
						int nd2 = n2 / 10;
						int ndm1 = n1 % 10;
						int ndm2 = n2 % 10;

						QuestionText.text = "Express in lowest terms: ";
						subQuestionText.text = nd1.ToString () + " m " + (ndm1 * 10).ToString () + " cm " + " : " + nd2.ToString () + " m " + (ndm2 * 10).ToString () + " cm ";
					} else if (rndnum == 2) {           //Kilometers
						int nd1 = n1 / 10;
						int nd2 = n2 / 10;
						int ndm1 = n1 % 10;
						int ndm2 = n2 % 10;

						QuestionText.text = "Express in lowest terms: ";
						subQuestionText.text = nd1.ToString () + " km " + (ndm1 * 100).ToString () + " m " + " : " + nd2.ToString () + " km " + (ndm2 * 100).ToString () + " m ";
					}

					Answer = num1.ToString () + ":" + num2.ToString ();
				}
			} 

			else if (level == 3) {			
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {												//Proportion. Find missing data
					subQuestionText.gameObject.SetActive (true);
					var multiplier2 = Random.Range (2, 10);
					var n3 = num1 * multiplier2;
					var n4 = num2 * multiplier2;
					subQuestionText.gameObject.SetActive (true);
					QuestionText.text = "Find the missing term: ";
					var qrnd = Random.Range (0, 4);
					if (qrnd == 0) {
						subQuestionText.text = " ? : " + n2.ToString () + " :: " + n3.ToString () + " : " + n4.ToString ();
						Answer = n1.ToString ();
					} else if (qrnd == 1) {
						subQuestionText.text = n1.ToString () + " : ? " + " :: " + n3.ToString () + " : " + n4.ToString ();
						Answer = n2.ToString ();
					} else if (qrnd == 2) {
						subQuestionText.text = n1.ToString () + " : " + n2.ToString () + " :: ? " + " : " + n4.ToString ();
						Answer = n3.ToString ();
					} else if (qrnd == 3) {
						subQuestionText.text = n1.ToString () + " : " + n2.ToString () + " :: " + n3.ToString () + " : ? ";
						Answer = n4.ToString ();
					}

				} else if (selector == 2) {    											// Word Problems, given ratio x:y, and total number. Find x or y
					subQuestionText.gameObject.SetActive (false);
					var total = n1 + n2;
					if (Random.Range (0, 2) == 0) {
						QuestionText.text = "There are " + total.ToString () + " students in a school, If the ratio of boys to girls is " + num1.ToString () + " : " + num2.ToString () + ". Find the number of boys ? ";
						Answer = n1.ToString ();
					} else {
						QuestionText.text = "There are " + total.ToString () + " students in a school, If the ratio of boys to girls is " + num1.ToString () + " : " + num2.ToString () + ". Find the number of girls ? ";
						Answer = n2.ToString ();
					}
				} else if (selector == 3) {    										// Word Problems, given ratio x:y, and total number. Find x or y
					subQuestionText.gameObject.SetActive (false);
					var total = n1 + n2;
					string[] arr1 = new string[3]{ "John", "Paul", "Joe" };
					string[] arr2 = new string[3]{ "Manish", "Aditya", "Naman" };
					var rand1 = Random.Range (0, arr1.Length);
					var rand2 = Random.Range (0, arr2.Length);
					if (Random.Range (0, 2) == 0) {
						QuestionText.text = arr1 [rand1] + " and " + arr2 [rand2] + " worked for " + num1.ToString () + " and " + num2.ToString () + " hours respectively. They received Rs. " + total.ToString () + " in total for their work. How much money (in Rs.) should " + arr1 [rand1] + " receive ? ";
						Answer = n1.ToString ();
					} else {
						QuestionText.text = arr1 [rand1] + " and " + arr2 [rand2] + " worked for " + num1.ToString () + " and " + num2.ToString () + " hours respectively. They received Rs. " + total.ToString () + " in total for their work. How much money (in Rs.) should " + arr2 [rand2] + " receive ? ";
						Answer = n2.ToString ();
					}
				} else if (selector == 4) {											//ratio given, x given, find y
					string[] arr1 = new string[3]{ "height", "weight", "earnings" };
					var rand1 = Random.Range (0, arr1.Length);
					if (rand1 == 0) {
						QuestionText.text = "The ratio of Raj and Ram's " + arr1 [rand1] + " is " + num1.ToString () + " : " + num2.ToString () + ". If Raj is " + n1.ToString () + " cm tall, find the " + arr1 [rand1] + " of Ram.";  
					} else if (rand1 == 1) {
						QuestionText.text = "The ratio of Raj and Ram's " + arr1 [rand1] + " is " + num1.ToString () + " : " + num2.ToString () + ". If Raj weighs " + n1.ToString () + " kgs, find the " + arr1 [rand1] + " of Ram.";  
					} else if (rand1 == 2) {
						QuestionText.text = "The ratio of Raj and Ram's " + arr1 [rand1] + " is " + num1.ToString () + " : " + num2.ToString () + ". If Raj earns Rs. " + n1.ToString () + ", find the " + arr1 [rand1] + " of Ram.";  
					}
					Answer = n2.ToString ();
				} 
			} 

			else if (level == 4) {			
				selector = GetRandomSelector (1, 5);

				if (selector == 1) {											//l:b of rectangle given, perimeter given, find l or b
					var perimeter = (n1 + n2) * 2;
					if (Random.Range (0, 2) == 0) {
						QuestionText.text = "The length and breadth of a rectangle are in the ratio of " + num1.ToString () + " : " + num2.ToString () + ". If the perimeter is " + perimeter.ToString () + ", find its length.";
						Answer = n1.ToString ();
					} else {
						QuestionText.text = "The length and breadth of a rectangle are in the ratio of " + num1.ToString () + " : " + num2.ToString () + ". If the perimeter is " + perimeter.ToString () + ", find its breadth.";
						Answer = n2.ToString ();
					}
				} else if (selector == 2) {    										//distance given, time in ratio given, find speed in ratio
					int lcm = MathFunctions.GetLCM (num1, num2);
					var multip = Random.Range (2, 6);
					var distance = multip * lcm;
					QuestionText.text = "Two cars complete " + distance.ToString () + " km race in " + num1.ToString () + " hrs and " + num2.ToString () + " hrs. Find the ratio of their speeds.";
					var s1 = distance / num1;
					var s2 = distance / num2;
					var cf = MathFunctions.GetHCF (s1, s2);
					Answer = (s1 / cf).ToString () + ":" + (s2 / cf).ToString ();
				}  else if (selector == 3) {										//x is a times y, z is b times x. Total given. find x or y or z
					subQuestionText.gameObject.SetActive (false);
					num1 = 1;
					var mul1 = Random.Range (2, 10);
					var mul2 = Random.Range (2, 10);
					num2 = mul1 * num1;
					var num3 = mul2 * num2;
					var ansmult = Random.Range (2, 10);
					var total = (num1 + num2 + num3) * ansmult;
					var qrnd = Random.Range (0, 3);
					if (qrnd == 0) {
						QuestionText.text = "Sonali Mam distributes " + total.ToString () + " coins between teal, black and blue teams. Teal team gets " + mul1.ToString () + " times the black team and the blue team gets " + (mul2).ToString () + " times the teal team. How many did the black team get?";
						Answer = (num1 * ansmult).ToString ();
					} else if (qrnd == 1) {
						QuestionText.text = "Sonali Mam distributes " + total.ToString () + " coins between teal, black and blue teams. Teal team gets " + mul1.ToString () + " times the black team and the blue team gets " + (mul2).ToString () + " times the teal team. How many did the teal team get?";
						Answer = (num2 * ansmult).ToString ();
					} else if (qrnd == 2) {
						QuestionText.text = "Sonali Mam distributes " + total.ToString () + " coins between teal, black and blue teams. Teal team gets " + mul1.ToString () + " times the black team and the blue team gets " + (mul2).ToString () + " times the teal team. How many did the blue team get?";
						Answer = (num3 * ansmult).ToString ();
					}
				} else if (selector == 4) {									//a gms coffee costs x , b gms tea costs y, find cost of coffee:tea
					var mult1 = Random.Range (2, 10);
					var mult2 = Random.Range (2, 10);
					var cost1 = n1 * mult1;
					var cost2 = n2 * mult2;
					subQuestionText.gameObject.SetActive (false);
					var qrnd = Random.Range (0, 2);
					if (qrnd == 0) {
						QuestionText.text = "The cost of " + mult1.ToString () + " gms coffee is Rs. " + cost1.ToString () + ".\nThe cost of " + mult2.ToString () + " gms tea is Rs. " + cost2.ToString () + ".\nFind the ratio of the cost of coffee : tea.";
						Answer = num1.ToString () + ":" + num2.ToString ();
					} else {
						QuestionText.text = "The cost of " + mult1.ToString () + " gms coffee is Rs. " + cost1.ToString () + ".\nThe cost of " + mult2.ToString () + " gms tea is Rs. " + cost2.ToString () + ".\nFind the ratio of the cost of tea : coffee.";
						Answer = num2.ToString () + ":" + num1.ToString ();
					}
				} 
			}
			CerebroHelper.DebugLog (Answer);
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
		}

		public override void numPadButtonPressed(int value) {
			if (ignoreTouches) {
				return;
			}
			if (value <= 9) {
				userAnswerText.text += value.ToString ();
			} else if (value == 10) {    //Back
				if (userAnswerText.text.Length > 0) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
			} else if (value == 11) {   // All Clear
				userAnswerText.text = "";
			} else if (value == 12) {   // :
				if(checkLastTextFor(new string[1]{":"})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ":";
			} else if (value == 13) {   // .
				if(checkLastTextFor(new string[1]{"."})) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			} 
		}
	}
}
