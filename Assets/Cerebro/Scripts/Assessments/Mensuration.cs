using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class Mensuration : BaseAssessment {

//		public Text QuestionText;

		private float Answer;

		public GameObject Square;
		public GameObject Rectangle;
		public GameObject Triangle;
		public GameObject Polygon;
		public GameObject UShape;
		public GameObject LShape;
		public GameObject ShadedRect;
		public GameObject ShadedL;

		void Start () {

			base.Initialise ("M", "MEN06", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++) {
				scorestreaklvls [i] = 0;
			}

			levelUp = false;

			Answer = 0f;
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
			var correct = false;

			questionsAttempted++;
			updateQuestionsAttempted ();
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
					increment = 10;
				} else if (Queslevel == 5) {
					increment = 15;
				} else if (Queslevel == 6) {
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

			Square.SetActive (false);
			Rectangle.SetActive (false);
			Triangle.SetActive (false);
			Polygon.SetActive (false);
			ShadedL.SetActive (false);
			ShadedRect.SetActive (false);
			UShape.SetActive (false);
			LShape.SetActive (false);

			level = Queslevel;
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
			if (level == 1) {
				selector = GetRandomSelector (1, 5);
				if (selector == 1) {  // perimeter of square
					GeneralButton.gameObject.SetActive (true);
					Square.SetActive (true);
					QuestionText.text = "Find the perimeter (in cm) of the given square.";
					var side = Random.Range (2, 11);
					Square.transform.Find ("Side").gameObject.GetComponent<Text> ().text = side.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = side * 4f;
				} else if (selector == 2) {   // perimeter of rectangle
					GeneralButton.gameObject.SetActive (true);
					Rectangle.SetActive (true);
					QuestionText.text = "Find the perimeter (in cm) of the given rectangle.";
					var side1 = Random.Range (2, 11);
					var side2 = Random.Range (2, 11);
					Rectangle.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					Rectangle.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 + side2) * 2f;
				} else if (selector == 3) {  // perimeter of triangle
					GeneralButton.gameObject.SetActive (true);
					Triangle.SetActive (true);
					QuestionText.text = "Find the perimeter (in cm) of the given triangle.";
					var side1 = Random.Range (2, 11);
					var side2 = Random.Range (2, 11);
					var side3 = Random.Range (2, 11);
					Triangle.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					Triangle.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					Triangle.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 + side2 + side3);
				} else if (selector == 4) {   // perimeter of polygon
					GeneralButton.gameObject.SetActive (true);
					Polygon.SetActive (true);
					QuestionText.text = "Find the perimeter (in cm) of the given polygon.";
					var side1 = Random.Range (2, 11);
					var side2 = Random.Range (2, 11);
					var side3 = Random.Range (2, 11);
					var side4 = Random.Range (2, 11);
					var side5 = Random.Range (2, 11);
					Polygon.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					Polygon.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					Polygon.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
					Polygon.transform.Find ("Side4").gameObject.GetComponent<Text> ().text = side4.ToString () + " cm";
					Polygon.transform.Find ("Side5").gameObject.GetComponent<Text> ().text = side5.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 + side2 + side3 + side4 + side5);
				}
			} else if (level == 2) {
				selector = GetRandomSelector (1, 5);
				if (selector == 1) {    // perimeter of Equilateral Triangle given, side = ?
					var side = Random.Range (2, 11);
					var perimeter = side * 3;
					QuestionText.text = "The perimeter of an equilateral triangle is " + perimeter.ToString () + " cm. Find the length (in cm) of each side.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = side;
				} else if (selector == 2) {    // Perimeter of Isosceles Triangle given, sides = ?
					var side1 = Random.Range (2, 11);
					var side2 = Random.Range (2, 11);
					var perimeter = side1 + (side2 * 2f);
					if (Random.Range (1, 3) == 1) {
						QuestionText.text = "The perimeter of an isosceles triangle is " + perimeter.ToString () + " cm and the length of its unequal side is " + side1.ToString () + " cm. Find the length (in cm) of equal sides.";
						Answer = side2;
					} else {
						QuestionText.text = "The perimeter of an isosceles triangle is " + perimeter.ToString () + " cm and the length of its equal sides is " + side2.ToString () + " cm. Find the length (in cm) of the remaining side.";
						Answer = side1;
					}
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;

				} else if (selector == 3) {    // Perimeter of Square given, side = ?
					var side = Random.Range (2, 11);
					var perimeter = side * 4;
					QuestionText.text = "The perimeter of a square is " + perimeter.ToString () + " m . Find its side length (in m).";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = side;
				} else if (selector == 4) {    // Perimeter of Rectangle, side1 given, side2 = ?
					var side1 = Random.Range (2, 11);
					var side2 = Random.Range (2, 11);
					while (side1 == side2)
						side2 = Random.Range (2, 11);
					var perimeter = (side1 + side2) * 2;
					QuestionText.text = "The perimeter of a rectangle is " + perimeter.ToString () + " cm and the length of one of its sides is " + side1.ToString () + " cm. Find the length (in cm) of the other unequal side.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = side2;
				}
			} else if (level == 3) {
				selector = GetRandomSelector (1, 4);
				if (selector == 1) {    // Rectangle -> Square
					var side1 = Random.Range (2, 11) * 2;
					var side2 = Random.Range (2, 11) * 2;
					var perimeter = (side1 + side2) * 2;
					var sideOfSquare = perimeter / 4;
					QuestionText.text = "A rectangular plot has sides " + side1.ToString () + " m and " + side2.ToString () + " m. A square has the same perimeter as this. Find the side length (in m) of the square.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = sideOfSquare;
				} else if (selector == 2) {    // Polygon -> Equilateral Triangle
					var side1 = Random.Range (2, 7) * 3;
					var side2 = Random.Range (2, 7) * 3;
					var side3 = Random.Range (2, 7) * 3; 
					var perimeter = (side1 + side2 + side3);
					var sideOfTriangle = perimeter / 3;
					QuestionText.text = "A triangular plot has sides " + side1.ToString () + " m, " + side2.ToString () + " m and " + side3.ToString () + " m. An equilateral triangle has the same perimeter as this. Find the side length (in m) of the triangle.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = sideOfTriangle;
				} else if (selector == 3) {    // Cost of fencing
					var side1 = Random.Range (2, 11);
					var side2 = Random.Range (2, 11);
					var cost = Random.Range (2, 11);
					var perimeter = (side1 + side2) * 2;
					QuestionText.text = "The length of the sides of a rectanglular plot are " + side1.ToString () + " m and " + side2.ToString () + " m. If the cost of fencing 1 cm is Rs. " + cost.ToString () + ", what will be the cost (in Rs.) of fencing the rectangular plot.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = perimeter * cost;
				}
			} else if (level == 4) {
				selector = GetRandomSelector (1, 8);
				if (selector == 1) {   // area of square
					GeneralButton.gameObject.SetActive (true);
					Square.SetActive (true);
					QuestionText.text = "Find the area (in sq. cm) of the given square.";
					var side = Random.Range (2, 11);
					Square.transform.Find ("Side").gameObject.GetComponent<Text> ().text = side.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = side * side;
				} else if (selector == 2) {  // area of rectangle
					GeneralButton.gameObject.SetActive (true);
					Rectangle.SetActive (true);
					QuestionText.text = "Find the area (in sq. cm) of the given rectangle.";
					var side1 = Random.Range (2, 11);
					var side2 = Random.Range (2, 11);
					Rectangle.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					Rectangle.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 * side2);
				} else if (selector == 3) { // perimeter of square given, area = ?
					GeneralButton.gameObject.SetActive (true);
					var side = Random.Range (2, 11);
					var perimeter = side * 4f;
					QuestionText.text = "The perimeter of a square is " + perimeter.ToString () + "cm . Find the area (in sq. cm) of the square.";
					answerButton = GeneralButton;
					Answer = side * side;
				} else if (selector == 4) {  //perimeter of rect given, l = multiplier * b, area = ?
					GeneralButton.gameObject.SetActive (true);
					var side1 = Random.Range (2, 11);
					var multiplier = Random.Range (2, 6);
					var side2 = multiplier * side1;
					var perimeter = (side1 + side2) * 2f;
					QuestionText.text = "The perimeter of a rectangle is " + perimeter.ToString () + " cm. If the length of one side is " + multiplier.ToString () + " times the other side, find the area (in sq. cm) of the rectangle.";
					answerButton = GeneralButton;
					Answer = side1 * side2;
				} else if (selector == 5) {  // cost of tiling room
					GeneralButton.gameObject.SetActive (true);
					var side = Random.Range (2, 11);
					var perimeter = side * 4f;
					var cost = Random.Range (2, 6) * 2;
					QuestionText.text = "The perimeter of a square room is " + perimeter.ToString () + " m. Find the cost of tiling the room at Rs. " + cost.ToString () + " per sq. m.";
					answerButton = GeneralButton;
					Answer = side * side * cost;
				} else if (selector == 6) {   //square area = rect area. rect sides given. find square side
					GeneralButton.gameObject.SetActive (true);
					var side = Random.Range (4, 11);
					var area = side * side;
					var side1 = GetRandomMultiple (area);
					var side2 = area / side1;
					while (side1 == side2) {
						side = Random.Range (4, 11);
						area = side * side;
						side1 = GetRandomMultiple (area);
						side2 = area / side1;
					}
					var cost = Random.Range (2, 6) * 2;
					QuestionText.text = "A square has the same area as that of a rectangle with sides " + side1.ToString () + " cm and " + side2.ToString () + " cm. Find the length (in cm) of the side of the square.";
					answerButton = GeneralButton;
					Answer = side;
				} else if (selector == 7) {    //area given, length = multiplier * breadth, peri = ?
					GeneralButton.gameObject.SetActive (true);
					var side1 = Random.Range (2, 11);
					var multiplier = Random.Range (2, 6);
					var side2 = multiplier * side1;
					var perimeter = (side1 + side2) * 2f;
					var area = side1 * side2;
					QuestionText.text = "The area of a rectangle is " + area.ToString () + " sq. cm. If the length is " + multiplier.ToString () + " times the breadth, find the perimeter (in cm) of the rectangle.";
					answerButton = GeneralButton;
					Answer = perimeter;
				}
			} else if (level == 5) {
				selector = GetRandomSelector (1, 7);
				if (selector == 1) {   //Shaded Region area
					GeneralButton.gameObject.SetActive (true);
					ShadedRect.SetActive (true);
					QuestionText.text = "Find the area of the shaded region.";
					var side1 = Random.Range (10, 21);
					var side2 = Random.Range (10, 21);
					var side3 = Random.Range (2, 10);
					var side4 = Random.Range (2, 10);
					ShadedRect.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					ShadedRect.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					ShadedRect.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
					ShadedRect.transform.Find ("Side4").gameObject.GetComponent<Text> ().text = side4.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 * side2) - (side3 * side4);
				} else if (selector == 2) {   //Shaded Region pavement cost
					GeneralButton.gameObject.SetActive (true);
					ShadedRect.SetActive (true);
					var side1 = Random.Range (10, 21);
					var side2 = Random.Range (10, 21);
					var side3 = Random.Range (2, 10);
					var side4 = Random.Range (2, 10);
					var cost = Random.Range (2, 21);
					QuestionText.text = "If the rate of pavementing is Rs. " + cost.ToString () + " per sq. cm, find the cost of pavementing the shaded portion of the park.";
					ShadedRect.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					ShadedRect.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					ShadedRect.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
					ShadedRect.transform.Find ("Side4").gameObject.GetComponent<Text> ().text = side4.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = ((side1 * side2) - (side3 * side4)) * cost;
				} else if (selector == 3) {  //tiles required to cover room
					var mul1 = Random.Range (2, 9);
					var mul2 = Random.Range (2, 9);
					var tile1 = Random.Range (2, 16);
					var tile2 = Random.Range (2, 16);
					var side1 = tile1 * mul1;
					var side2 = tile2 * mul2;
					QuestionText.text = "A room of dimensions " + side1.ToString() + " cm and " + side2.ToString() + " cm has to be filled with tiles of dimensions " + tile1.ToString() + " cm and " + tile2.ToString() + " cm. Find the number of tiles required.";
					answerButton = GeneralButton;
					Answer = (mul1 * mul2);
				} else if (selector == 4) { //L-shaped Shaded Region Area
					GeneralButton.gameObject.SetActive (true);
					ShadedL.SetActive (true);
					var side1 = Random.Range (10, 21);
					var side2 = Random.Range (10, 21);
					var side3 = Random.Range (2, 10);
					var side4 = Random.Range (2, 10);
					QuestionText.text = "Find the area of the shaded region.";
					ShadedL.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					ShadedL.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					ShadedL.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
					ShadedL.transform.Find ("Side4").gameObject.GetComponent<Text> ().text = side4.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 * side2) - (side3 * side4);
				} else if (selector == 5) {  //L-shaped Area
					GeneralButton.gameObject.SetActive (true);
					LShape.SetActive (true);
					var side1 = Random.Range (10, 21);
					var side2 = Random.Range (10, 21);
					var side3 = Random.Range (2, 10);
					var side4 = Random.Range (2, 10);
					QuestionText.text = "Find the area of the figure given below.";
					LShape.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					LShape.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					LShape.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
					LShape.transform.Find ("Side4").gameObject.GetComponent<Text> ().text = side4.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 * side2) - ((side1 - side3) * (side2 - side4));
				} else if (selector == 6) {  //U-shaped Area
					GeneralButton.gameObject.SetActive (true);
					UShape.SetActive (true);
					var side1 = Random.Range (11, 21);
					var side3 = Random.Range (1,5);
					var side4 = Random.Range (1,5);

					var side2 = Random.Range (14, 21);
					var side5 = Random.Range (2, 10);
					var side6 = Random.Range (2, 10);
					QuestionText.text = "Find the area of the figure given below.";
					UShape.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					UShape.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					UShape.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
					UShape.transform.Find ("Side4").gameObject.GetComponent<Text> ().text = side4.ToString () + " cm";
					UShape.transform.Find ("Side5").gameObject.GetComponent<Text> ().text = side5.ToString () + " cm";
					UShape.transform.Find ("Side6").gameObject.GetComponent<Text> ().text = side6.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side2 * side3) + (side4 * side6) + ((side1 - (side3 + side4)) * (side2 - side5));
				}
			}
			CerebroHelper.DebugLog (Answer);
			userAnswerText = answerButton.gameObject.GetChildByName<Text>("Text");
			userAnswerText.text = "";
		}

		float GetRandomMultiple(float num) {
			List<float> multiples = new List<float>();
			for (int i = 2; i < num / 2; i++) {
				if (num % i == 0) {
					multiples.Add (i);
				}
			}
			return multiples[Random.Range(0,multiples.Count)];
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
