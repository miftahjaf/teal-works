using UnityEngine;
using System.Collections;
using Cerebro;
using UnityEngine.UI;
using MaterialUI;
using System.Collections.Generic;

public class VolumeAndSurfaceArea7 : BaseAssessment
{

	public TEXDraw SubQuestionLaText;
	public GameObject MCQ;
	private float Answer;
	private string AnswerText;
	private bool AnswerInText;

	void Start () {

		base.Initialise ("M", "VSA07", "S01", "A01");

		StartCoroutine(StartAnimation ());


		scorestreaklvls = new int[5];
		for (var i = 0; i < scorestreaklvls.Length; i++) {
			scorestreaklvls [i] = 0;
		}

		levelUp = false;

		Answer = 0f;
		GenerateQuestion ();
	}

	public override void SubmitClick()
	{
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
		if (AnswerInText)
		{
			if (float.TryParse (userAnswerText.text, out answer))
			{
				answer = float.Parse (userAnswerText.text);
			} 

			correct = (answer == Answer);


		} else {
			var answerSplits = Answer.ToString ().Split (new string[] { ":" }, System.StringSplitOptions.None);
			var userAnswerSplits = userAnswerText.text.Split (new string[] { ":" }, System.StringSplitOptions.None);

			if (answerSplits.Length == userAnswerSplits.Length) {
				for (var i = 0; i < answerSplits.Length; i++) {
					 answer = 0;
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
		}
		if (correct) {
			

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
			userAnswerText.text =AnswerInText? Answer.ToString ():AnswerText;
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

		QuestionLatext.size = 25;
		MCQ.SetActive (false);
		AnswerText = "";
		AnswerInText = false;
		SubQuestionLaText.gameObject.SetActive (false);

		level = Queslevel;
		if (Queslevel > scorestreaklvls.Length) {
			level = UnityEngine.Random.Range (1, scorestreaklvls.Length + 1);
		}

	
		if (level == 1) {
			selector = GetRandomSelector (1, 6);
			if (selector == 1) {  // volume of cuboid
				GeneralButton.gameObject.SetActive (true);
				//					Triangle.SetActive (true);
				int side1 = Random.Range (5, 15);
				int side2 = Random.Range (5, 15);
				int side3 = Random.Range (5, 15);
				QuestionLatext.text = "A cuboid has its dimension " + side1.ToString () + "cm, " + side2.ToString () + "cm and " + side3.ToString () + "cm. Determine the volume.";

				answerButton = GeneralButton;
				Answer = (side1 * side2 * side3);
			} else if (selector == 2) {   // Volume of cube
				GeneralButton.gameObject.SetActive (true);

				int side1 = Random.Range (5, 15);

				QuestionLatext.text = "Find the volume of a cube in cm^{3} whose one edge measures " + side1.ToString () + " cm.";
				answerButton = GeneralButton;
				Answer = (side1 * side1 * side1);
			} else if (selector == 3) {  // Volume of cube
				GeneralButton.gameObject.SetActive (true);
				//					Square.SetActive (true);
				int side = Random.Range (5, 15);
				QuestionLatext.text = "The volume of a cube is " + (side * side * side) + " cm^{3}. Determine its edge length (in cm).";

				answerButton = GeneralButton;
				Answer = side;
			} else if (selector == 4) {   // Volume of cuboid
				GeneralButton.gameObject.SetActive (true);
				int length = Random.Range (5, 15);
				int breadth = Random.Range (5, 15);
				int height = Random.Range (5, 15);
				QuestionLatext.text = "Find the missing data for cuboid:\nVolume = " + (length * breadth * height).ToString () + "m^{3}, length = " + length.ToString () + "m, breadth = " + breadth.ToString () + ", height =?";

				answerButton = GeneralButton;
				Answer = height;
			} else if (selector == 5) {   //Surface area of cuboid
				GeneralButton.gameObject.SetActive (true);
				int length = Random.Range (5, 10) * 10;
				int breadth = Random.Range (5, 10) * 10;
				int height = Random.Range (5, 10) * 10;
				QuestionLatext.text = "An examination hall is " + length.ToString () + " metres long, " + breadth.ToString () + " metres wide and " + height.ToString () + " metres high. Find the surface area (in m^{2}) of four walls.";
				answerButton = GeneralButton;
				Answer = 2 * height * (length + breadth);
			}
		} else if (level == 2) {
			selector = GetRandomSelector (1, 6);
			if (selector == 1) {    //Total Surface area of cube
				GeneralButton.gameObject.SetActive (true);
				int length = Random.Range (5, 15);
				QuestionLatext.text = "If the total surface area of a cube is " + (6 * length * length).ToString () + " cm^{2}, determine the volume of cube.";
				answerButton = GeneralButton;
				Answer = length * length * length;
			} else if (selector == 2) {    //Total Surface area of cube

				GeneralButton.gameObject.SetActive (true);
				int length = Random.Range (5, 15);
				int breadth = Random.Range (5, 15);
				int height = Random.Range (5, 15);
				QuestionLatext.text = "Find the missing data for cuboid\nTotal surface area = " + (2 *(length * breadth + breadth *height +height *length)).ToString () + "cm^{2}, length =" + length.ToString () + "cm, breadth =" + breadth.ToString () + "cm, height =?";
				answerButton = GeneralButton;
				Answer = height;

			} else if (selector == 3) {    // joind cube TSA
				GeneralButton.gameObject.SetActive (true);
				int a = Random.Range (5, 10);
				int totalCube = Random.Range (2, 4);
				int length = totalCube * a;
				int breadth = totalCube;
				int height = a;
				QuestionLatext.text = totalCube+" cubes each of side " + a + "m are joined. Determine the total surface area (in m^{2}) of the resulting figure.";
				answerButton = GeneralButton;
				Answer = 2 * ((length * breadth) + (breadth * height) + (length * height));

				
			} else if (selector == 4) {    // Volume
				GeneralButton.gameObject.SetActive (true);
				int a = Random.Range (2, 5);

				QuestionLatext.text = "The perimeter of one face of cube is " + 6 * a * a + " cm. Find its volume.";
				answerButton = GeneralButton;
				Answer = a * a * a;
				
			} else if (selector == 5) {    // maximum length in box
				GeneralButton.gameObject.SetActive (true);
				int length;
				int breadth;
				int height;
				do {
					length = Random.Range (5, 15);
					breadth = Random.Range (5, 15);
					height = Random.Range (5, 15);
				} while(Mathf.Sqrt ((length * length) + (breadth * breadth) + (height * height)) % 1 != 0);
					
				QuestionLatext.text = "What is the maximum length of a pencil (in cm) that can be fit inside a rectangular box of dimension " + length + " X " + breadth + " X " + height + "cm";
				answerButton = GeneralButton;
				Answer = Mathf.Sqrt ((length * length) + (breadth * breadth) + (height * height));

			}
		}else if (level == 3)
			{
			selector = GetRandomSelector (1, 6); 
			if (selector == 1) {    // cost of  paint hall
				GeneralButton.gameObject.SetActive (true);
				int length = Random.Range (5, 15);
				int breadth = Random.Range (5, 15);
				int height = Random.Range (5, 15);
				int price = Random.Range (3, 8) * 10;
				QuestionLatext.text = "A rectangular hall has its dimensions "+length+"m, "+breadth+"m, and "+height +"m. Find the cost incurred to paint the four walls at Rs. "+price;
			    answerButton = GeneralButton;
				Answer =(2 * height * (length+breadth)) *price;

			} 
			else if (selector == 2)  //Volume of water
			{    
					GeneralButton.gameObject.SetActive (true);
					int depth = Random.Range (5, 10);
					float area = Random.Range (15, 25) / 10f;
					QuestionLatext.text = "In a shower, " + depth + "cm of rain falls. The volume (in m^{3}) of water that falls on " + area + " hectares of ground is ";
			     	answerButton = GeneralButton; 
				    Answer = Mathf.RoundToInt(area * 10000 * depth / 100);
			} 
			else if (selector == 3) //TSA of joined cube
		    {    
				GeneralButton.gameObject.SetActive (true);
				int a = Random.Range (5, 10);
				int totalCube = Random.Range (6, 10);
				int length = totalCube * a;
				int breadth = totalCube;
				int height = a;
				QuestionLatext.text = totalCube+" cubes each of side " + a + "cm are placed adjacent to each other. Determine the total surface area (in cm^{2}) of the newly formed cuboid.";
				answerButton = GeneralButton;
				Answer = 2 * ((length * breadth) + (breadth * height) + (length * height)); 
			} 
			else if (selector == 4)  // wooden block
			{    
				GeneralButton.gameObject.SetActive (true);
				int length = Random.Range (2, 6)*10;
				int breadth = Random.Range (2, 6)*10;
				int height = Random.Range (2, 6)*10;
				QuestionLatext.text = "The size of the wooden block is "+ length+" X "+breadth +" X "+height +"cm .How many such blocks will be required to construct a solid wooden cube of minimum size?";
				answerButton = GeneralButton;
				int lcm = MathFunctions.GetLCM (length, breadth, height);
				int volumeOfCube = (lcm * lcm * lcm);
				int volumeofBlock = length * breadth * height;
				Answer = (volumeOfCube / volumeofBlock);

			} 
			else if (selector == 5) // price to plaster walls and floor
			{   
				GeneralButton.gameObject.SetActive (true);
				int length = Random.Range (3, 15);
				int width = Random.Range (3, 15);
				float depth = Random.Range (10, 25) / 10f;
				int price = Random.Range (1, 5) * 10;
				QuestionLatext.text = "A water tank (rectangular) is "+length+"m long, "+width+"m wide and "+depth+"m deep, determine the cost of plastering the four walls and the floor at the rate of Rs. "+price+" per m^{2}";
				answerButton = GeneralButton;
				float LSA = 2 * depth * (length + width);
				float areaOfBase = length + width;
				Answer = (LSA + areaOfBase) * price;
			}
		} else if (level == 4) {
			selector = GetRandomSelector (1, 6);
			if (selector == 1) {  
				GeneralButton.gameObject.SetActive (true);
				int lengthBrick = Random.Range (5, 25);
				int widthBrick = Random.Range (5, 25);
				int heightBrick = Random.Range (5, 25);
				int lengthWall = Random.Range (20, 30);
				int thicknessWall = Random.Range (40, 60);
				int heightWall = Random.Range (5, 15);
				int mortarPercantage = Random.Range (1, 4) * 10;
				QuestionLatext.text = "Find the number of bricks, each measuring "+lengthBrick+" cm X "+widthBrick+" cm X "+heightBrick+" cm, required to construct a wall "+lengthWall+" m long, "+heightWall+" m high and "+thicknessWall+" cm thick, if "+mortarPercantage+"% of the wall is filled with mortar?";
				answerButton = GeneralButton;
				int wallVolume = lengthWall * 100 * thicknessWall * heightWall * 100;
				int volumeofBricks =Mathf.RoundToInt (((100-mortarPercantage)/100f) *wallVolume);
				int oneBrickVolume = lengthBrick * widthBrick * heightBrick;
				Answer = volumeofBricks / oneBrickVolume;
			} 
			else if (selector == 2) 
			{  
				int length = Random.Range (15, 25);
				int breadth = Random.Range (15, 25);
				int height = Random.Range (15, 25);
				GeneralButton.gameObject.SetActive (true);
				QuestionLatext.text = "A rectangular box has length "+length+" cm, breadth "+breadth+" cm and height "+height+" cm. If the length of box is doubled, then determine the ratio of total surface area of the new box to the old one.";
				answerButton = GeneralButton;
				int newlength = 2 * length;
				int newValue = (newlength * breadth + breadth * height + height * newlength);
				int oldValue = ((length * breadth) + (breadth * height) + (height * length));
				int GCD = MathFunctions.GetHCF (newValue, oldValue);
				AnswerInText = true;
				AnswerText =  string.Format("{0}:{1}", (newValue/ GCD), (oldValue / GCD));
				CerebroHelper.DebugLog(AnswerInText);

			} else if (selector == 3) { 
				
			} else if (selector == 4) { 
				
			} else if (selector == 5) {  
				
			}
		} else if (level == 5) {
			selector = GetRandomSelector (1, 6);
			if (selector == 1) {  

			} else if (selector == 2) {  

			} else if (selector == 3) {  

			} else if (selector == 4) { 
				
			} else if (selector == 5) {
				
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
		}else if (value == 13) {   // :
			if(checkLastTextFor(new string[1]{":"})) {
				userAnswerText.text = userAnswerText.text.Substring (0, userAnswerText.text.Length - 1);
			}
			userAnswerText.text += ":";
		} 
	}
}

