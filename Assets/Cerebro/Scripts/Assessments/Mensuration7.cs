using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro {
	public class Mensuration7 : BaseAssessment {

		public TEXDraw SubQuestionLaText;
		public GameObject MCQ;
		private float Answer;

		public GameObject Square;
		public GameObject Circle;
		public GameObject Shape;
		public GameObject Polygon;
		public GameObject Rectangle;
		public GameObject Triangle;
		public GameObject ShadedRectangle;
		public GameObject TwoRoadsRectangle;
		public GameObject TwoRectangles;

		void Start () {

			base.Initialise ("M", "MEN07", "S01", "A01");

			StartCoroutine(StartAnimation ());


			scorestreaklvls = new int[6];
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
				} else if (Queslevel == 7) {
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

		public void MCQOptionClicked (int value)
		{
			if (ignoreTouches) {
				return;
			}
			AnimateMCQOption (value);
			userAnswerText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<Text> ();
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

			QuestionText.fontSize = 22;
			MCQ.SetActive (false);
			SubQuestionLaText.gameObject.SetActive (false);
			Square.SetActive (false);
			Shape.SetActive (false);
			Circle.SetActive (false);
			Polygon.SetActive (false);
			Rectangle.SetActive (false);
			Triangle.SetActive (false);
			ShadedRectangle.SetActive (false);
			TwoRectangles.SetActive (false);
			TwoRoadsRectangle.SetActive (false);

			level = Queslevel;
			if (Queslevel > scorestreaklvls.Length) {
				level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
			}
			if (level == 1) {
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {  // perimeter of triangle
					GeneralButton.gameObject.SetActive (true);
//					Triangle.SetActive (true);
					int side1 = Random.Range (5, 15);
					int side2 = Random.Range (5, 15);
					int side3 = Random.Range (5, 15);
					QuestionText.text = "Find the perimeter of a triangle with sides " + side1.ToString() + "cm, " + side2.ToString() + "cm and "+ side3.ToString() + "cm.";
//					Triangle.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
//					Triangle.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
//					Triangle.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 + side2 + side3);
				} else if (selector == 2) {   // perimeter of rectangle
					GeneralButton.gameObject.SetActive (true);
//					Rectangle.SetActive (true);
					int side1 = Random.Range (5, 11);
					int side2 = Random.Range (11, 15);
					QuestionText.text = "Find the perimeter of a rectangle with length " + side2.ToString() + "cm and breadth " + side1.ToString() + "cm.";
//					Rectangle.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
//					Rectangle.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 + side2) * 2f;
				} else if (selector == 3) {  // perimeter of square
					GeneralButton.gameObject.SetActive (true);
//					Square.SetActive (true);
					int side = Random.Range (5, 15);
					QuestionText.text = "Find the perimeter of a square with side " + side.ToString() + "cm.";
//					Square.transform.Find ("Side").gameObject.GetComponent<Text> ().text = side.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = side * 4f;
				} else if (selector == 4) {   // perimeter of polygon
					GeneralButton.gameObject.SetActive (true);
					int side1 = Random.Range (2, 11);
					int side2 = Random.Range (2, 11);
					int side3 = Random.Range (2, 11);
					int side4 = Random.Range (2, 11);
					QuestionText.text = "Find the perimeter of a quadrilateral with sides " + side1.ToString() + "cm, " + side2.ToString() + "cm, " + side3.ToString() + "cm and " + side4.ToString() + "cm.";
//					Polygon.SetActive (true);
//					Polygon.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
//					Polygon.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
//					Polygon.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = side3.ToString () + " cm";
//					Polygon.transform.Find ("Side4").gameObject.GetComponent<Text> ().text = side4.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (side1 + side2 + side3 + side4);
				} else if (selector == 5) {   // perimeter of polygon
					GeneralButton.gameObject.SetActive (true);
					int diameter = Random.Range (2, 7) * 7;
					QuestionText.text = "Find the perimeter of a circle with diameter " + diameter.ToString() + "cm.";
//					Circle.SetActive (true);
//					Circle.transform.Find ("Diameter").gameObject.GetComponent<Text> ().text = diameter.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = (diameter/7) * 22;
				}
			} else if (level == 2) {
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {    // perimeter of Equilateral Triangle given, side = ?
					int side = Random.Range (2, 11);
					QuestionText.text = "Find the area of a equilateral triangle with side length " + side.ToString () + " cm";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = side * side * 0.43f;
					Answer = (float)System.Math.Round ((((float)Answer) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
				} else if (selector == 2) {    // Perimeter of Isosceles Triangle given, sides = ?
					int[] baseLengths = new int[] {3, 5, 8, 7};
					int[] heights = new int[] {4, 12, 15, 24};
					int[] otherSide = new int[] {5, 13, 17, 25};
					int currSelector = Random.Range (0, baseLengths.Length);
					int baseLength = baseLengths [currSelector];
					int height = heights [currSelector];
					int side3 = otherSide [currSelector];
					QuestionText.text = "Find the area of a triangle whose sides are " + baseLength.ToString () + "cm, " + height.ToString () + "cm and " + side3.ToString () + "cm.";
					Answer = baseLength * height * 0.5f;
					Answer = (float)System.Math.Round ((((float)Answer) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
				} else if (selector == 3) {    // Perimeter of Square given, side = ?
					int baseLength = Random.Range (2, 11);
					int altitude = Random.Range (2, 11);
					QuestionText.text = "Find the area of a triangle with base " + baseLength.ToString () + "cm and altitude " + altitude.ToString() + "cm";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = baseLength * altitude * 0.5f;
					Answer = (float)System.Math.Round ((((float)Answer) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
				} else if (selector == 4) {    // Perimeter of Rectangle, side1 given, side2 = ?
					int x = Random.Range (2, 6) * 5;
					int y = (x/5) * 8;
					float area = (y/4)*(Mathf.Sqrt(4*x*x-y*y));
					print ("x " + x + " y " + y + " sqrt of " + (4 * x * x - y * y) + " is " + (Mathf.Sqrt (4 * x * x - y * y)));
					QuestionText.text = "Find the area of the isosceles triangle.";
					GeneralButton.gameObject.SetActive (true);
					Triangle.SetActive (true);
					Triangle.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = y.ToString() + " cm";
					Triangle.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = x.ToString() + " cm";
					Triangle.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = x.ToString() + " cm";
					answerButton = GeneralButton;
					Answer = area;
				} else if (selector == 5) {    // Perimeter of Rectangle, side1 given, side2 = ?
					int x = Random.Range (2, 11);
					QuestionText.text = "Find the area of the square(in sq. m).";
					Square.SetActive (true);
					Square.transform.Find ("Side").gameObject.GetComponent<Text> ().text = x.ToString () + " cm";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = x * x;
				}
			} else if (level == 3) {
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {    // Rectangle -> Square
					int baseLength = Random.Range (2, 11);
					int altitude = Random.Range (2, 11);
					float area = baseLength * altitude * 0.5f;;
					QuestionText.text = "Find the altitude of the given triangle.";
					SubQuestionLaText.gameObject.SetActive (true);
					SubQuestionLaText.text = "Area = " + area.ToString () + "cm^2 and Base = " + baseLength.ToString () +"cm.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = altitude;
				} else if (selector == 2) {    // Polygon -> Equilateral Triangle
					int side = Random.Range(2, 11);
					int perimeter = side * 4;
					QuestionText.text = "FInd the area of square whose perimeter is " + perimeter.ToString() + "cm.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = side * side;
				} else if (selector == 3) {    // Polygon -> Equilateral Triangle
					int side = Random.Range(2, 11);
					QuestionText.text = "FInd the area of square whose perimeter is : ";
					SubQuestionLaText.gameObject.SetActive (true);
					SubQuestionLaText.text = side.ToString() + "\\root{ 2 } cm.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = side * 4;
				} else if (selector == 4) {    // Cost of fencing
					int side = Random.Range(2, 7) * 7;
					QuestionText.text = "Find the perimeter of given shape.";
					Shape.SetActive (true);
					Shape.transform.Find ("Side").gameObject.GetComponent<Text> ().text = side.ToString() + " cm";
					Shape.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side.ToString() + " cm";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = 2 * 22 * (side / 7);
					Answer = (float)System.Math.Round ((((float)Answer) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
				} else if (selector == 5) {    // Cost of fencing
					int side = Random.Range(2, 6) * 5;
					int baseLength = (side/5) * 6;
					int perimeter = 2 * side + baseLength;
					float area = (baseLength/4)*(Mathf.Sqrt(4*side*side-baseLength*baseLength));
					QuestionText.text = "Find the area of an isosceles triangle if the base of such triangle is " + baseLength.ToString() + "cm and its perimeter is " + perimeter.ToString() + "cm.";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Triangle.SetActive (true);
					Triangle.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = baseLength.ToString() + " cm";
					Triangle.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = "x";
					Triangle.transform.Find ("Side3").gameObject.GetComponent<Text> ().text = "x";
					Answer = area;
					Answer = (float)System.Math.Round ((((float)Answer) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
				}
			} else if (level == 4) {
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {   // area of square
					int length = Random.Range(2, 7);
					int width = Random.Range(5, 11);
					while(length == width)
						width = Random.Range(5, 11);
					int perimeter = 2 * length + 2 * width;
					QuestionText.text = "The perimeter of a table top is " + perimeter.ToString() + "cm. If the length of the table is " + length.ToString() + "cm, Find its breadth";
					GeneralButton.gameObject.SetActive (true);
					answerButton = GeneralButton;
					Answer = width;
				} else if (selector == 2) {  // area of rectangle
					GeneralButton.gameObject.SetActive (true);
					Rectangle.SetActive (true);
					float[] rnd = new float[]{ 1.5f, 2.5f, 3.5f};
					int rndSelector = Random.Range (0, rnd.Length);
					float side1 = 3 * rnd[rndSelector];
					float side2 = 4 * rnd[rndSelector];
					float side3 = 5 * rnd[rndSelector];
					QuestionText.text = "The diagonal of a rectangle is " + side3.ToString() + "m. The shorter side of the rectangle is " + side1.ToString() + "m. Find the area of the rectangle(in sq. m).";
					Rectangle.transform.Find ("Side1").gameObject.GetComponent<Text> ().text = side1.ToString () + " cm";
					Rectangle.transform.Find ("Side2").gameObject.GetComponent<Text> ().text = side2.ToString () + " cm";
					answerButton = GeneralButton;
					Answer = side1 * side2;
				} else if (selector == 3) { // perimeter of square given, area = ?
					GeneralButton.gameObject.SetActive (true);
					int side = Random.Range (2, 11);
					int length = Random.Range (1, 2 * side - 1);
					while(length == side)
						length = Random.Range (1, 2 * side - 1);
					int width = 2 * side - length;
					QuestionText.text = "A wire is in shape of a square of side " + side.ToString() + "cm. If the wire is rebent into a rectangle of length " + length.ToString() + "cm, find its breadth";
					answerButton = GeneralButton;
					Answer = width;
				} else if (selector == 4) {  //perimeter of rect given, l = multiplier * b, area = ?
					GeneralButton.gameObject.SetActive (true);
					int subLength = Random.Range (1, 3);
					int subWidth = Random.Range (1, 3);
					int length = Random.Range (7, 10);
					int width = Random.Range (7, 10);
					int area = width * length - subLength * subWidth;
					int charge = Random.Range (2, 5);
					QuestionText.text = "A door frame of dimensions " + subLength.ToString() + "mx" + subWidth.ToString() + "m is fixed on the wall of dimension " + length.ToString() + "mx" + width.ToString() + "m." ;
					//SubQuestionLaText.gameObject.SetActive (true);
					QuestionText.text += "Find the total labour charges for painting the wall if the labour charges for painting 1 sq. m of the wall is Rs. 2.50";
					TwoRectangles.SetActive (true);
					TwoRectangles.transform.Find ("length").gameObject.GetComponent<Text> ().text = length.ToString () + " m";
					TwoRectangles.transform.Find ("width").gameObject.GetComponent<Text> ().text = width.ToString () + " m";
					TwoRectangles.transform.Find ("sublength").gameObject.GetComponent<Text> ().text = subLength.ToString () + " m";
					TwoRectangles.transform.Find ("subwidth").gameObject.GetComponent<Text> ().text = subWidth.ToString () + " m";
					answerButton = GeneralButton;
					Answer = area * charge;
				} else if (selector == 5) {  // cost of tiling room
					GeneralButton.gameObject.SetActive (true);
					int side = Random.Range (20, 40);
					int areaSquare = side * side;
					string factors = MathFunctions.GetFactors (areaSquare);
					string[] facts = factors.Split (',');
					while (facts.Length <= 3) {
						side = Random.Range (20, 40);
						areaSquare = side * side;
						factors = MathFunctions.GetFactors (areaSquare);
						facts = factors.Split (',');
					}
					int width = int.Parse(facts[Random.Range(2, facts.Length - 2)]);
					while(width == side)
						width = int.Parse(facts[Random.Range(2, facts.Length - 2)]);
					int length = areaSquare / width;
					if (length < width) {
						int temp = width;
						width = length;
						length = temp;
					}
					QuestionText.text = "The area of a square and a rectangle are equal. If the side of the square is " + side.ToString() + "cm and the bredth of the rectangle is " + width.ToString() + "cm, find the perimeter of the rectangle.";
					answerButton = GeneralButton;
					Answer = length + width;
				}
			} else if (level == 5) {
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {   //Shaded Region area
					GeneralButton.gameObject.SetActive (true);
					int firstPerc = Random.Range (1, 5) * 10;
					int secondPerc = Random.Range (1, 3) * 10;
					QuestionText.text = "A cotton towel when bleached, lost " + firstPerc.ToString() + "% of its length and " + secondPerc.ToString() + "% of its breadth. Find the percentage of decrease in area.";
					answerButton = GeneralButton;
					Answer = 100 * (((firstPerc / 100) + (secondPerc / 100)) - ((firstPerc / 100) * (secondPerc / 100)));
//					Answer = 100 - 100 * firstPerc * 0.1f;
//					Answer = Answer - Answer * secondPerc * 0.1f;
				} else if (selector == 2) {   //Shaded Region pavement cost
					GeneralButton.gameObject.SetActive (true);
					int radius = Random.Range (1, 8) * 7;
					QuestionText.text = "Sagar divides a solid circular disc of radius " + radius.ToString() + "cm into two equal parts. What is the perimeter of each semicircular shaped disc?";
					answerButton = GeneralButton;
					Answer = (radius / 7) * 22;
				} else if (selector == 3) {  //tiles required to cover room
					int error = Random.Range (2, 5);
					QuestionText.text = "An error of " + error.ToString() + "% in excess is made while measuring the side of a square. Find the percentage of error in the calculated area of the square. (upto 2 decimal places.)";
					answerButton = GeneralButton;
					Answer = ((100f + error) * (100f + error) - 100f * 100f) / 100f;
					Answer = (float)System.Math.Round ((((float)Answer) * 100), System.MidpointRounding.AwayFromZero) / (float)100;
				} else if (selector == 4) { //L-shaped Shaded Region Area
					MCQ.SetActive(true);
					QuestionText.text = "A man walked diagonally across a square lot. Approximately what was the percentage saved by not walking along the edges.";
					SubQuestionLaText.gameObject.SetActive (true);
					SubQuestionLaText.text = "(Use \\root{ 2 } = 1.41)";
					MCQ.transform.Find ("Option1").Find ("Text").GetComponent<Text> ().text = "20";
					MCQ.transform.Find ("Option2").Find ("Text").GetComponent<Text> ().text = "24";
					MCQ.transform.Find ("Option3").Find ("Text").GetComponent<Text> ().text = "30";
					MCQ.transform.Find ("Option4").Find ("Text").GetComponent<Text> ().text = "36";
					GeneralButton.gameObject.SetActive (false);
					answerButton = GeneralButton;
					Answer = 30;
				} else if (selector == 5) {  //L-shaped Area
					GeneralButton.gameObject.SetActive (true);
					int width = Random.Range (20, 30);
					int length = Random.Range (30, 40);
					int trackLength = Random.Range (2, 5);
					int totalArea = width * length;
					int parkArea = (width - trackLength) * (length - trackLength);
					QuestionText.text = "A uniform track of " + trackLength.ToString() + " metre width surrounds a rectangular park " + length.ToString() + " metre long and " + width.ToString() + " metre broad. Calculate the area of the track.";
					ShadedRectangle.SetActive (true);
					ShadedRectangle.transform.Find ("width").gameObject.GetComponent<Text> ().text = width.ToString () + " m";
					ShadedRectangle.transform.Find ("length").gameObject.GetComponent<Text> ().text = length.ToString () + " m";
					ShadedRectangle.transform.Find ("track").gameObject.GetComponent<Text> ().text = trackLength.ToString () + " m";
					answerButton = GeneralButton;
					Answer = totalArea - parkArea;
				}
			} else if (level == 6) {
				selector = GetRandomSelector (1, 6);
				if (selector == 1) {   //Shaded Region area
					GeneralButton.gameObject.SetActive (true);
					int width = Random.Range (4, 8) * 5;
					int length = Random.Range (8, 15) * 5;
					int tileSize = MathFunctions.GetHCF (width, length);
					int cost = Random.Range (3, 9) * 5;
					QuestionText.text = "The assembly hall of AIS is " + length.ToString() + "m long and " + width.ToString() + "m wide. The floor has to be paved with square tiles, each having side length " + tileSize.ToString() + "cm. Determine the total cost of tiling at the rate of Rs. " + cost.ToString() + " per square tile.";
					answerButton = GeneralButton;
					Answer = ((width * length) / tileSize) * cost;
				} else if (selector == 2) {   //Shaded Region pavement cost
					GeneralButton.gameObject.SetActive (true);
					int side1 = Random.Range (6, 12) * 5;
					int side2 = (side1 / 5) * 4;
					int perimeter = side1 + side2;
					int area = side1 * side2;
					int cost = Random.Range (10, 30);
					QuestionText.text = "The length and breadth of a rectangle field are in the raio 5:4 and the perimeter of this field is " + perimeter.ToString() + "m. Calculate the cost of growing grass at the rate of Rs. " + cost.ToString() + " for sq. metre over the area.";
					answerButton = GeneralButton;
					Answer = area * cost;
				} else if (selector == 3) {  //tiles required to cover room
					int length = Random.Range(30, 40);
					int width = Random.Range (20, 30);
					int diff = length - width;
					int perimeter = 2 * (width + length);
					int area = length * width;
					QuestionText.text = "The difference between the length and breadth of a rectangle is " + diff.ToString() + "m. If its perimeter is " + perimeter.ToString() + "m, then find its area.";
					answerButton = GeneralButton;
					Answer = area;
				} else if (selector == 4) { //L-shaped Shaded Region Area
					GeneralButton.gameObject.SetActive (true);
					QuestionText.fontSize = 18;
					int length = Random.Range (100, 130);
					int width = Random.Range (80, 110);
					int horiRoad = Random.Range (3, 6);
					int vertRoad = Random.Range (4, 8);
					int roadOne = horiRoad * length;
					int roadTwo = vertRoad * width;
					int totalArea = roadOne + roadTwo - horiRoad * vertRoad;
					int cost = Random.Range (10, 30);
					QuestionText.text = "A rectangular playground, " + length.ToString() + "m long and " + width.ToString() + "m wide, has two crossroads. One road is " + horiRoad.ToString() + "m wide and runs parallel to the length ";
					QuestionText.text += "and another road is " + vertRoad.ToString() + "m wide which runs parallel to the breadth. Determine the cost of construction of both roads at the rate of Rs. " + cost.ToString() + " per sq. metres.";
					TwoRoadsRectangle.SetActive (true);
					TwoRoadsRectangle.transform.Find ("length").gameObject.GetComponent<Text> ().text = length.ToString () + " m";
					TwoRoadsRectangle.transform.Find ("width").gameObject.GetComponent<Text> ().text = width.ToString () + " m";
					TwoRoadsRectangle.transform.Find ("horiRoad").gameObject.GetComponent<Text> ().text = horiRoad.ToString () + " m";
					TwoRoadsRectangle.transform.Find ("vertRoad").gameObject.GetComponent<Text> ().text = vertRoad.ToString () + " m";
					answerButton = GeneralButton;
					Answer = totalArea * cost;
				} else if (selector == 5) {  //L-shaped Area
					GeneralButton.gameObject.SetActive (true);
					int length = Random.Range (5, 9) * 50;
					int width = Random.Range (1, 5) * 50;
					int perimeter = (width + length) * 2;
					//int speed = Random.Range (1, 5) * 6;
					int time = Random.Range (1, 5);
					int speed = (perimeter * 3) / (time * 50);
//					int perimeter = (speed / 3) * time * 10 * 5;
//					int width = Random.Range (2, perimeter/4);
//					int length = perimeter/2 - width;
					int hcf = MathFunctions.GetHCF (length, width);
					QuestionText.text = "The ratio between the length and breadth of a rectangular park is " + (length/hcf).ToString() + ":" + (width/hcf).ToString() + ". If a man cycling along the boundary of the park at the speed of ";
					QuestionText.text += speed.ToString() + " km/hr completes one round in " + time.ToString() + " minutes then find the area of the park(in sq metres).";
					answerButton = GeneralButton;
					Answer = width * length;
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
			} else if (value == 12) {   // .
				if (checkLastTextFor (new string[1]{ "." })) {
					userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
				}
				userAnswerText.text += ".";
			}
		}
	}
}
