using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class SimpleInterest : BaseAssessment {

		private float Principal;
		private float Rate;
		private float Interest;
		private float Amount;
		private float Years;

		private float Answer;

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

		private float[] RateValues = new float[] {2f,3f,4f,5f,6f,8f,9f,10f,12f,15f};
		private float[] TimeValues = new float[] {2f,3f,4f,5f,6f,8f,10f};

		void Start () {

			StartCoroutine(StartAnimation ());
			base.Initialise ("M", "SI06", "S01", "A01");

			scorestreaklvls = new int[4];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			Principal = 0f;
			Rate = 0f;
			Years = 0f;
			Interest = 0f;

			levelUp = false;

			multiplier = 0;
			Answer = 0f;
			GenerateQuestion ();
		}

		public override void SubmitClick(){
			if (ignoreTouches || userAnswerText.text == "") {
				return;
			}

			questionsAttempted++;
			updateQuestionsAttempted ();

			int increment = 0;
			//var correct = false;
			ignoreTouches = true;
			//Checking if the response was correct and computing question level
			var correct = false;
			float answer = 0;
			if(float.TryParse(userAnswerText.text,out answer)) {
				answer = float.Parse (userAnswerText.text);
			}

			if (answer == Answer) {
				correct = true;

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

				UpdateStreak(5,5);

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

			Principal = Random.Range (1, 21);
			Principal = Principal * 1200;

			Rate = RateValues[Random.Range(0,RateValues.Length)];
			Years = TimeValues[Random.Range(0,TimeValues.Length)];

			float Months = Random.Range (1, 11);
			Months = Months * 6;
			var displayMonth = false;

			PrincipalQuestion.text = "Rs. " + Principal.ToString ();
			RateQuestion.text = Rate.ToString () + " %";
			TimeQuestion.text = Years.ToString () + " Years";

			Interest = Principal * Rate * Years / 100f;
			Amount = Principal + Interest;

			InterestQuestion.text = "Rs. " + Interest.ToString ();
			AmountQuestion.text = "Rs. " + Amount.ToString ();

			//Generating random questions on Simple Interest Level 1 Questions

			PrincipalButton.gameObject.SetActive (false);
			RateButton.gameObject.SetActive (false);
			InterestButton.gameObject.SetActive (false);
			TimeButton.gameObject.SetActive (false);
			GeneralButton.gameObject.SetActive (false);
			AmountButton.gameObject.SetActive (false);
			AmountText.gameObject.SetActive (false);

			PrincipalQuestion.gameObject.SetActive (false);
			RateQuestion.gameObject.SetActive (false);
			TimeQuestion.gameObject.SetActive (false);
			InterestQuestion.gameObject.SetActive (false);
			AmountQuestion.gameObject.SetActive (false);

			PrincipalText.gameObject.SetActive (false);
			RateText.gameObject.SetActive (false);
			TimeText.gameObject.SetActive (false);
			InterestText.gameObject.SetActive (false);
			AmountText.gameObject.SetActive (false);

			answerButton = null;
			userAnswerText = null;

			level = Queslevel;

			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}

			if (level == 1 || level == 2) {
//				if (level == 2) {
//					displayMonth = true;
//					Years = Months / 12;
//					TimeQuestion.text = Months.ToString () + " Months";
//					Interest = Principal * Rate * Years / 100f;
//					Amount = Principal + Interest;
//					InterestQuestion.text = "Rs. " + Interest.ToString ();
//					AmountQuestion.text = "Rs. " + Amount.ToString ();
//				}
				QuestionText.text = "Find the missing data";

				PrincipalQuestion.gameObject.SetActive (true);
				RateQuestion.gameObject.SetActive (true);
				TimeQuestion.gameObject.SetActive (true);
				InterestQuestion.gameObject.SetActive (true);

				PrincipalText.gameObject.SetActive (true);
				RateText.gameObject.SetActive (true);
				TimeText.gameObject.SetActive (true);
				InterestText.gameObject.SetActive (true);

				if (level == 1) {
					selector = GetRandomSelector (3, 4); // Just ask for Interest
				}
				else if (level == 2) {
					selector = GetRandomSelector (1, 6);
				}

				if (selector == 1) {
					PrincipalQuestion.text = "Rs. ";
					PrincipalButton.gameObject.SetActive (true);
					answerButton = PrincipalButton;
					Answer = Principal;
				} else if (selector == 2) {
					RateQuestion.text = "         %";
					RateButton.gameObject.SetActive (true);
					answerButton = RateButton;
					Answer = Rate;
				} else if (selector == 3) {
					InterestQuestion.text = "Rs. ";
					InterestButton.gameObject.SetActive (true);
					answerButton = InterestButton;
					Answer = Interest;
				} else if (selector == 4) {
					if (displayMonth) {
						TimeQuestion.text = "         Months";
						Answer = Months;
					} else {
						TimeQuestion.text = "         Years";
						Answer = Years;
					}
					TimeButton.gameObject.SetActive (true);
					answerButton = TimeButton;

				} else if (selector == 5) {
					AmountQuestion.text = "Rs. ";
					AmountQuestion.gameObject.SetActive (true);
					AmountButton.gameObject.SetActive (true);
					AmountText.gameObject.SetActive (true);
					answerButton = AmountButton;
					Answer = Amount;
				}
			}

			// Generating random questions on Simple Interest Level 2 Questions

			if (level == 3) {
				selector = GetRandomSelector (1, 4);
				if (selector == 1) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					QuestionText.text = "Rs. " + Interest.ToString () + " is charged as interest for a loan on Rs. " + Principal.ToString () + " for " + Years.ToString () + " years. What is the rate of interest in percentage ?";
					Answer = Rate;
				} else if (selector == 2) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					QuestionText.text = "How long will it take for Rs. " + Principal.ToString () + " at " + Rate.ToString () + " % of simple interest to become Rs. " + Amount.ToString ();
					Answer = Years;	
				} else {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					QuestionText.text = "How much does Rs. " + Principal.ToString () + " become when kept in a bank paying simple interest at " + Rate.ToString () + " % per year for " + Years.ToString () + " years ? ";
					Answer = Amount;
				}
			}


			// Generating random questions on Simple Interest Level 3 Questions

			if (level == 4) {
				selector = GetRandomSelector (1, 3);
				multiplier = Random.Range (2, 8);

				if (selector == 1) {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					QuestionText.text = "A sum of money when kept in bank for " + Years.ToString() + " years becomes Rs. " + Amount.ToString() + " at " + Rate.ToString() + " % simple interest. What was the original sum of money ? ";
					Answer = Principal;
				}
				else {
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					float answer = Random.Range (1, 6);
					multiplier = (answer * Years) + 1;
					QuestionText.text = "At what rate of simple interest will money become " + multiplier.ToString() + " times its initial value in " + Years.ToString() + " years ?";
					Answer = answer*100;
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
			} 
		}
	}
}
