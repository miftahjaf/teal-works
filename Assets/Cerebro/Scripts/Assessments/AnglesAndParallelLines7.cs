using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using MaterialUI;

namespace Cerebro
{
	public class AnglesAndParallelLines7 : BaseAssessment
	{

		public Text subQuestionText;
		public GameObject MCQ;
		public LineAndAngle lineAndAngle;
		private string Answer;
		private string alternateAnswer;
		private int coeff1;
		private int coeff2;
		private int coeff3;
		private int coeff4;
		private int coeff5;
		private int coeff6;

	
		void Start()
		{

			StartCoroutine(StartAnimation());
			base.Initialise("M", "APL07", "S01", "A01");

			scorestreaklvls = new int[5];
			for (var i = 0; i < scorestreaklvls.Length; i++)
			{
				scorestreaklvls[i] = 0;
			}

			levelUp = false;

			coeff1 = coeff2 = coeff3 = coeff4 = coeff5 = coeff6 = 0;

			Answer = "";//
			alternateAnswer = "";
			GenerateQuestion();
		}

		public override void SubmitClick()
		{
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
			Debug.Log ("submit");
			if (MCQ.activeSelf) 
			{
				if (Answer == userAnswerLaText.text) 
				{
					correct = true;
					StartCoroutine(AnimateMCQOption (Answer));
				}
				else
				{
					correct = false;
					AnimateMCQOptionCorrect(Answer);

				}
			}
			else
			{
				float answer = 0;
				float userAnswer = 0;
				bool directCheck = false;
				if (float.TryParse(Answer, out answer))
				{
					answer = float.Parse(Answer);
				}
				else
				{
					directCheck = true;
				}
				if (float.TryParse(alternateAnswer, out answer))
				{
					answer = float.Parse(alternateAnswer);
				}
				else
				{
					directCheck = true;
				}
				if (float.TryParse(userAnswerLaText.text, out userAnswer))
				{
					userAnswer = float.Parse(userAnswerLaText.text);
				}
				else
				{
					correct = false;
				}
				if (answer != userAnswer)
				{
					correct = false;
				}
				if (directCheck)
				{
					if (userAnswerLaText.text == Answer || userAnswerLaText.text == alternateAnswer)
					{
						correct = true;
					}
					else
					{
						correct = false;
					}
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
					increment = 10;
				}
				else if (Queslevel == 3)
				{
					increment = 15;
				}
				else if (Queslevel == 4)
				{
					increment = 15;
				}


				UpdateStreak(5, 12);

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
				if (numPad.activeSelf)
				{               // is not MCQ type question
					userAnswerLaText.text = "";
				}
				if (userAnswerLaText != null)
				{
					userAnswerLaText.color = MaterialColor.textDark;
				}
				if (userAnswerLaText != null)
				{
					userAnswerLaText.color = MaterialColor.textDark;
				}
				ignoreTouches = false;
			}
			else
			{
				if (numPad.activeSelf)
				{               // is not MCQ type question
					CerebroHelper.DebugLog("going in if");
					userAnswerLaText.text = Answer.ToString();
					userAnswerLaText.color = MaterialColor.green800;
				}
				else
				{
					CerebroHelper.DebugLog("going in else");
					userAnswerLaText.color = MaterialColor.textDark;
				}
			}

			ShowContinueButton();
		}

		protected override IEnumerator ShowCorrectAnimation()
		{
			userAnswerLaText.color = MaterialColor.green800;
			var config = new GoTweenConfig()
				.scale(new Vector3(1.1f, 1.1f, 0))
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
			userAnswerLaText = MCQ.transform.Find ("Option" + value.ToString ()).Find ("Text").GetComponent<TEXDraw> ();
			answerButton = MCQ.transform.Find ("Option" + value.ToString ()).GetComponent<Button> ();
			SubmitClick ();
		}

		IEnumerator AnimateMCQOption (string ans)
		{
			var GO = new GameObject();
			for (int i = 1; i <= 4; i++) {
				if (MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().text == ans) {
					GO = MCQ.transform.Find ("Option" + i.ToString ()).gameObject;
				}
			}
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

			Vector2 newPoint;
			float newRadius;

			ignoreTouches = false;
			base.QuestionStarted();
			// Generating the parameters

			level = Queslevel;

			answerButton = GeneralButton;
			subQuestionText.gameObject.SetActive(false);
			QuestionText.gameObject.SetActive(false);
			QuestionLatext.gameObject.SetActive(true);
			GeneralButton.gameObject.SetActive(true);
			numPad.SetActive(true);
			MCQ.SetActive (false);
			lineAndAngle.Reset ();

			for (int i = 1; i < 5; i++) {
				MCQ.transform.Find ("Option" + i.ToString ()).Find ("Text").GetComponent<TEXDraw> ().color = MaterialColor.textDark;
			}

			if (Queslevel > scorestreaklvls.Length)
			{
				level = UnityEngine.Random.Range(1, scorestreaklvls.Length + 1);
			}


			#region level1
			if (level == 1)
			{
				selector = GetRandomSelector(1, 6);
				if (selector == 1)
				{
					coeff1 = Random.Range(2, 7) * Random.Range(2, 7);
					QuestionLatext.text = "Calculate the measure of 1/" + coeff1 + " of a right angle.";
					float ans = 90f/(float)coeff1;
					this.SetAnswerValue(new float[]{ans});
				}
				else if (selector == 2)
				{
					coeff1 = 10 * Random.Range(2,36);
					if (coeff1 == 180 || coeff1 >= 270)
						coeff1 = 90;
					QuestionLatext.text = "If \\angle{A} = " + coeff1 + MathFunctions.deg + ", what type of angle is \\angle{A}?";
					MCQ.SetActive (true);
					GeneralButton.gameObject.SetActive(false);
					numPad.SetActive(false);
					List<string> options = new List<string>();
					options.Add("Acute");
					options.Add("Obtuse");
					options.Add("Right");
					options.Add("Reflex");
					if (coeff1 < 90)
						Answer = options[0];
					else if (coeff1 == 90)
						Answer = options[2];
					else if (coeff1 > 90 && coeff1 < 180)
						Answer = options[1];
					else
						Answer = options[3];
					RandomizeMCQOptionsAndFill(options);
				}
				else if (selector == 3)
				{
					QuestionLatext.text = "Calculate the value of x. ACB in a straight line.";
					//Diagram
					int value1 =Random.Range(60,120);
					while(value1==90)
					{
						value1 =Random.Range(60,120);
					}

					float angle = (180 -value1);

					lineAndAngle.AddLinePoint(new LinePoint("C", Vector2.zero,0,false,0));
					lineAndAngle.AddLinePoint (new LinePoint ("A", Vector2.zero, 180,true));
					lineAndAngle.AddLinePoint (new LinePoint ("B", Vector2.zero, 0,true));
					lineAndAngle.AddLinePoint (new LinePoint ("D", Vector2.zero, value1,true));

					lineAndAngle.AddAngleArc(new AngleArc(value1.ToString()+MathFunctions.deg,Vector2.zero,0,value1));
					lineAndAngle.AddAngleArc(new AngleArc("x",Vector2.zero,value1,180));

					lineAndAngle.DrawDiagram();


					this.SetAnswerValue(new float[]{angle});
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range(1, 50);
					coeff2 = Random.Range(10, 60);
					coeff3 = Random.Range(10, 60);
					coeff4 = Random.Range(1, 50);
					coeff5 = Random.Range(10, 60);
					coeff6 = Random.Range(10, 60);
					QuestionLatext.text = "If \\angle{A} = " + coeff1 + MathFunctions.deg + coeff2 + MathFunctions.min + coeff3 + MathFunctions.sec + " and \\angle{B} = " + coeff4 + MathFunctions.deg + coeff5 + MathFunctions.min + coeff6 + MathFunctions.sec + ", find 2\\angle{A} + \\angle{B}.";
					int anssec, ansmin, ansdeg;
					anssec = 2 * coeff3 + coeff6;
					ansmin = 2 * coeff2 + coeff5;
					ansdeg = 2 * coeff1 + coeff4;
					while (anssec >= 60){
						anssec -= 60;
						ansmin++;
					}
					while (ansmin >=  60){
						ansmin -= 60;
						ansdeg++;
					}
					Answer = "" + ansdeg + MathFunctions.deg + ansmin + MathFunctions.min + anssec + MathFunctions.sec;
				}
				else if (selector == 5)
				{
					QuestionLatext.text = "Which of the following are always equal?";
					MCQ.SetActive (true);
					GeneralButton.gameObject.SetActive(false);
					numPad.SetActive(false);
					List<string> options = new List<string>();
					options.Add("Vertically opposite angles");
					options.Add("Alternate angles");
					options.Add("Corresponding angles");
					options.Add("Co-interior angles");
					Answer = options[0];
					RandomizeMCQOptionsAndFill(options);
				}
			}
			#endregion
			#region level2

			if (level == 2)
			{
				selector = GetRandomSelector(1, 7);

				if(selector==1)
				{
					coeff1 = 10 * Random.Range(20, 50);
					QuestionLatext.text = "An angle measures " + coeff1 + MathFunctions.min + ". What is its measure in degrees and minutes?";

					this.SetAnswerValue(new float[]{coeff1 / 60f});

				}
				else if(selector==2)
				{
					coeff1 = Random.Range (2, 10);
					while (coeff1 == 6)
						coeff1 = Random.Range (2, 10);
					if (Random.Range (1,3) == 1)
						coeff1 = 2 * Random.Range (1,3) * coeff1 - 1;
					QuestionLatext.text = "The complement of an angle is 1/" + coeff1 + " of itself. What is the measure of the angle?";
					float ans = (float)(coeff1 * 90)/(float)(coeff1 + 1);
					this.SetAnswerValue(new float[]{ans});
				}
				else if (selector == 3)
				{
					coeff1 = Random.Range(2,9) * Random.Range(2,6);
					coeff2 = 48 - coeff1;
					if (coeff1 == coeff2){
						coeff1++;
						coeff2--;
					}
					int hcf = MathFunctions.GetHCF(coeff1, coeff2);
					coeff1 /= hcf;
					coeff2 /= hcf;
					QuestionLatext.text = "Two supplementary angles are in the ratio " + coeff1 + " : " + coeff2 + ". Calculate the measure of the smaller angle.";
					float ans = (float)(coeff1 * 180f)/(float)(coeff1 + coeff2);
					if (coeff1 > coeff2)
						ans = 180f - ans;
					this.SetAnswerValue(new float[]{ans});
				}
				else if (selector == 4)
				{
					coeff1 = Random.Range(91,151);
					while ((((coeff1 - 90) * 200 * 40) % (180 - coeff1)) != 0)
						coeff1 = Random.Range(91,151);
					float percent = (float)((coeff1 - 90) * 200) / (float)(180 - coeff1); 
					QuestionLatext.text = "An angle is " + percent + "% more than its supplement. Calculate its measure.";

					this.SetAnswerValue(new float[]{coeff1});
				}
				else if(selector==5)
				{
					QuestionLatext.text = "Calculate the value of x. ACB in a straight line :";
					//Diagram
					int value1 =Random.Range(10,40);
					int value2 =Random.Range(20,50);
					float angle = (180 + value2- value1)/2;

					lineAndAngle.AddLinePoint(new LinePoint("C", Vector2.zero,0,false,0));
					lineAndAngle.AddLinePoint (new LinePoint ("A", Vector2.zero, 180,true));
					lineAndAngle.AddLinePoint (new LinePoint ("B", Vector2.zero, 0,true));
					lineAndAngle.AddLinePoint (new LinePoint ("D", Vector2.zero, angle-value2,true));

					lineAndAngle.AddAngleArc(new AngleArc("x-"+value2+MathFunctions.deg,Vector2.zero,0,angle-value2,50f));
					lineAndAngle.AddAngleArc(new AngleArc("x+"+value1+MathFunctions.deg,Vector2.zero,angle-value2,180,40f));

					lineAndAngle.DrawDiagram();


					this.SetAnswerValue(new float[]{angle});
				}

				else if(selector ==6)
				{
					QuestionLatext.text = "In the given figure, lines AC and BD intersect each other at O. Find angles x, y, z :";

					int angle = Random.Range(55,75);
					lineAndAngle.AddLinePoint(new LinePoint("O", Vector2.zero,0,false,0));
					lineAndAngle.AddLinePoint(new LinePoint("A", Vector2.zero,angle/2f,true));
					lineAndAngle.AddLinePoint(new LinePoint("B", Vector2.zero,(180 -(angle/2f)),true));
					lineAndAngle.AddLinePoint(new LinePoint("C", Vector2.zero,(180 +(angle/2f)),true));
					lineAndAngle.AddLinePoint(new LinePoint("D", Vector2.zero,(360 -(angle/2f)),true));

					lineAndAngle.AddAngleArc(new AngleArc("x",Vector2.zero,angle/2f,(180 -(angle/2f)),60f));
					lineAndAngle.AddAngleArc(new AngleArc("y",Vector2.zero,(180 -(angle/2f)),(180 +(angle/2f)),45f));
					lineAndAngle.AddAngleArc(new AngleArc("z",Vector2.zero,(180 +(angle/2f)),(360 -(angle/2f)),60f));
					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg.ToString(),Vector2.zero,(360 -(angle/2f)),(360 +(angle/2f)),45f));

					lineAndAngle.DrawDiagram();

					this.SetAnswerValue(new float[]{180-angle,angle,180-angle});

				}
			}
			#endregion
			#region level3
			if (level == 3)
			{

				selector = GetRandomSelector(1, 6);

				if (selector == 1)
				{
					QuestionLatext.text = "Find x :";
					int angle =Random.Range(55,75);

					lineAndAngle.AddLinePoint(new LinePoint("C",new Vector2(0f,50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("D",new Vector2(0f,50f),180,true));
					lineAndAngle.AddLinePoint(new LinePoint("A",new Vector2(0f,-50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("B",new Vector2(0f,-50f),180,true));

					newRadius = Mathf.Abs(50f/Mathf.Sin(Mathf.Deg2Rad * angle));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,angle,false,newRadius));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,angle,newRadius); 
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint,angle,true,70f));


					lineAndAngle.AddAngleArc(new AngleArc("x",newPoint,0,angle));

					newRadius = Mathf.Abs(50f/Mathf.Sin(Mathf.Deg2Rad *  (angle+180f)));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero, (angle+180f),false,newRadius));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,(angle+180),newRadius); 
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint, (angle+180f),true,70f));
					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg,newPoint,0,angle));

					lineAndAngle.DrawDiagram();

					lineAndAngle.SetScale(0.8f);

					this.SetAnswerValue(new float[]{angle});
				}
				else if (selector == 2)
				{
					coeff2 = Random.Range(10, 150);
					coeff3 = Random.Range(2, 11);
					while ((40 * (90 + coeff2)) % (coeff3 + 1) != 0)
						coeff2 = Random.Range(10, 150);
					QuestionLatext.text = coeff3 + " times an angle is " + coeff2 + MathFunctions.deg + " more than its complement. Find the measure of its supplementary angle.";
					float ans = (float)(90 + coeff2)/(float)(coeff3 + 1);
					ans = 180f - ans;
					this.SetAnswerValue(new float[]{ans});
				}
				else if(selector ==3)
				{
					QuestionLatext.text = "Find x :";
					float angle ;
					do
					{
						coeff1 = Random.Range(2,6);
						coeff2 = Random.Range(2,10);
					    coeff3 = Random.Range(2,10);
					    angle= (270f/(coeff1+coeff2+coeff3));
					}
					while(21600%(coeff1+coeff2+coeff3)!=0 || (angle*coeff1==90) || (angle*coeff2==90)||(angle*coeff3==90));

						lineAndAngle.AddLinePoint (new LinePoint ("A", Vector2.zero, 90,true));
						lineAndAngle.AddLinePoint (new LinePoint ("B", Vector2.zero, 0,true));
						lineAndAngle.AddLinePoint (new LinePoint ("D", Vector2.zero, 90+(angle * coeff1),true));
						lineAndAngle.AddLinePoint (new LinePoint ("C", Vector2.zero, 90+(angle*(coeff2 +coeff1)),true));

						lineAndAngle.AddAngleArc(new AngleArc("",Vector2.zero,0f,90f));
						lineAndAngle.AddAngleArc(new AngleArc(coeff1 +"x",Vector2.zero,90f,90+(angle*coeff1)));
						lineAndAngle.AddAngleArc(new AngleArc(coeff2 +"x",Vector2.zero,90+(angle*coeff1),90+(angle*(coeff2 +coeff1))));
						lineAndAngle.AddAngleArc(new AngleArc(coeff3 +"x",Vector2.zero,90+(angle*(coeff2 +coeff1)),90+(angle*(coeff2 +coeff1+coeff3))));	

						lineAndAngle.DrawDiagram();

					   this.SetAnswerValue(new float[]{angle});
				}
				else if(selector ==4)
				{
					QuestionLatext.text = "In the figure, AB & CD are parallel lines and PQ is a traversal intersecting the lines at R and S. What is the measure of \\angle{ASQ}?";
					int angle =Random.Range(50,80);
					lineAndAngle.AddLinePoint(new LinePoint("B",new Vector2(0f,50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("A",new Vector2(0f,50f),180,true));
					lineAndAngle.AddLinePoint(new LinePoint("C",new Vector2(0f,-50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("D",new Vector2(0f,-50f),180,true));

					newRadius = Mathf.Abs (50 / Mathf.Sin (angle*Mathf.Deg2Rad));
					lineAndAngle.AddLinePoint(new LinePoint("S",Vector2.zero,angle,false,newRadius));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,angle,newRadius); 
					lineAndAngle.AddLinePoint(new LinePoint("Q",newPoint,angle,true,70f));

					newRadius = Mathf.Abs (50 / Mathf.Sin ((180+angle)*Mathf.Deg2Rad));
					lineAndAngle.AddLinePoint(new LinePoint("R",Vector2.zero,180+angle,false,newRadius,-1));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,(180+angle),newRadius);
					lineAndAngle.AddLinePoint(new LinePoint("P",newPoint,180+angle,true,70f));

					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString() +MathFunctions.deg,newPoint,0f,angle,70));	

					lineAndAngle.DrawDiagram();

					lineAndAngle.SetScale(0.8f);

					this.SetAnswerValue(new float[]{180f-angle});
				}
				else if(selector ==5)
				{
					QuestionLatext.text = "Find x :";
					int angle =Random.Range(110,130);
				
					 coeff3 = Random.Range (2, 10);
					 coeff4 = Random.Range (3, 10);

					while((80*(angle+coeff3))%coeff4 !=0)
					{
						coeff3 = Random.Range (2, 10);
					}
						
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(0f,50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(0f,50f),180,true));
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(0f,-50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(0f,-50f),180,true));

					newRadius = Mathf.Abs(50f/Mathf.Sin(Mathf.Deg2Rad * angle));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,angle,false,newRadius));


					newPoint =MathFunctions.PointAtDirection(Vector2.zero,angle,newRadius);
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint,angle,true,70f));


					lineAndAngle.AddAngleArc(new AngleArc(coeff3+"x-"+coeff4+MathFunctions.deg,newPoint,0,angle));

					newRadius = Mathf.Abs(50f/Mathf.Sin(Mathf.Deg2Rad *  (angle+180f)));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero, (angle+180f),false,newRadius));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero, (angle+180),newRadius);
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint, (angle+180f),true,70f));
					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg,newPoint,180,180+angle));

					lineAndAngle.DrawDiagram();

					lineAndAngle.SetScale(0.8f);

					this.SetAnswerValue(new float[]{(angle+coeff4)/(float)coeff3});
				}
			}
			#endregion
			#region level4
			if (level == 4) 
			{
				selector = GetRandomSelector (1, 6);

				if (selector == 1) {
					QuestionLatext.text = "Find y :";
					int angle =Random.Range(110,130);
					coeff1 = Random.Range (2, 10);
					coeff2 = Random.Range (5, 15);


					while((80*(180-angle-coeff2))%coeff1 !=0)
					{
						coeff1 = Random.Range (2, 10);
					}


					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(0f,50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(0f,50f),180,true));
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(0f,-50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(0f,-50f),180,true));

					newRadius = Mathf.Abs(50f/Mathf.Sin(Mathf.Deg2Rad * angle));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,angle,false,newRadius));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero, angle,newRadius);
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint,angle,true,70f));

					lineAndAngle.AddAngleArc(new AngleArc(coeff1+"y+"+coeff2+MathFunctions.deg,newPoint,angle,180f,50f));
				

					newRadius = Mathf.Abs(50f/Mathf.Sin(Mathf.Deg2Rad *  (angle+180f)));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero, (angle+180f),false,newRadius));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero, (angle+180),newRadius);

					lineAndAngle.AddLinePoint(new LinePoint("",newPoint, (angle+180f),true,70f));
					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg,newPoint,180,180+angle));

					lineAndAngle.DrawDiagram();

					lineAndAngle.SetScale(0.8f);

					this.SetAnswerValue(new float[]{(180-angle-coeff2)/(float)coeff1});

				} 
				else if(selector ==2)
				{
					QuestionLatext.text ="Find the measure of reflex angle at O :";

					int angle = Random.Range(40,80);

					float answer = angle / 2f;


					lineAndAngle.AddLinePoint (new LinePoint ("Q", Vector2.zero, 90f-answer,false,new Vector2(0,20f),70f));
					lineAndAngle.AddLinePoint (new LinePoint ("O", Vector2.zero, 90f-answer+angle,false,new Vector2(0,20f),70f));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero, (90f-answer),70f);
					lineAndAngle.AddAngleArc(new AngleArc("x",newPoint,270f-answer,270,40f));

					lineAndAngle.AddLinePoint (new LinePoint ("R", newPoint, 270,true));
					newPoint =MathFunctions.PointAtDirection(Vector2.zero, (90f-answer+angle),70f);
				
					lineAndAngle.AddLinePoint (new LinePoint ("N", newPoint, 270f,true));
					lineAndAngle.AddAngleArc(new AngleArc("x",newPoint,270f,270+answer,40f));

					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg,Vector2.zero,90f-answer,90f-answer+angle));

					lineAndAngle.DrawDiagram();

					this.SetAnswerValue(new float[]{360f-answer});

				}
				else if(selector==3)
				{
					QuestionLatext.text = "Find x :";

					int angle = Random.Range (100, 130);
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,angle-90f,true));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,angle+90f,true));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,  (angle-90f),50f);

					lineAndAngle.AddLinePoint (new LinePoint ("", newPoint, 90f,true));
					lineAndAngle.AddLinePoint (new LinePoint ("", newPoint, 270f,true));

					lineAndAngle.AddAngleArc(new AngleArc("x",newPoint,90+angle,270f));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,  (angle+90f),50f);

					lineAndAngle.AddLinePoint (new LinePoint ("", newPoint, 90f,true));
					lineAndAngle.AddLinePoint (new LinePoint ("", newPoint, 270f,true));

					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg,newPoint,270f,270+angle));

					lineAndAngle.DrawDiagram();

					lineAndAngle.SetScale(0.8f);

					this.SetAnswerValue(new float[]{180f-angle});
				}
				else if(selector ==4)
				{
					QuestionLatext.text ="Find x :";
					int angle = Random.Range(95,115);
					 coeff1 = Random.Range (2, 6);
					 coeff2 = Random.Range (2, 6);
					while((400*angle)%(coeff1+coeff2)!=0)
					{
						angle = Random.Range (95, 115);
					}
					float answer = (float)angle / (float)(coeff1 + coeff2);
					newRadius = Mathf.Abs(50f / Mathf.Sin (((answer *coeff1)) * Mathf.Deg2Rad));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,((answer *coeff1)),false,newRadius));
					newPoint = MathFunctions.PointAtDirection (Vector2.zero, (answer * coeff1) , newRadius); 
					lineAndAngle.AddAngleArc(new AngleArc(coeff1+"x",newPoint,180f,180+(answer * coeff1)));


					lineAndAngle.AddLinePoint(new LinePoint("Q",newPoint,(answer * coeff1) ,true,60f));
					newRadius = Mathf.Abs(50f / Mathf.Sin ((360f-(coeff2*answer))* Mathf.Deg2Rad));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,360f-(coeff2*answer),false,newRadius));
					newPoint = MathFunctions.PointAtDirection (Vector2.zero, (360f-(coeff2*answer)) , newRadius); 
					lineAndAngle.AddLinePoint(new LinePoint("R",newPoint,(360f-(coeff2*answer)) ,true,60f,-1));

					lineAndAngle.AddAngleArc(new AngleArc(coeff2+"x",newPoint,180-((answer *coeff2)),180f));

					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg,Vector2.zero,360f-(coeff2*answer),360f-(coeff2*answer)+angle));

					lineAndAngle.AddLinePoint(new LinePoint("C",new Vector2(0f,50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("D",new Vector2(0f,50f),180,true));
					lineAndAngle.AddLinePoint(new LinePoint("A",new Vector2(0f,-50f),0,true));
					lineAndAngle.AddLinePoint(new LinePoint("B",new Vector2(0f,-50f),180,true));

					lineAndAngle.DrawDiagram();

					this.SetAnswerValue(new float[]{answer});

					lineAndAngle.SetScale (0.8f);
				}
				else if (selector == 5)
				{
					QuestionLatext.text = "Find x :";
					int val = Random.Range(16000,24000);
					while (val % 400 == 0) 
					{
						val = Random.Range(16000,24000);
					}
					float angle =MathFunctions.GetRounded( val/ 400f,4);

					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,90,true));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,90-angle,true));
					lineAndAngle.AddLinePoint (new LinePoint ("", Vector2.zero, 0,true));	
					lineAndAngle.AddAngleArc(new AngleArc("",Vector2.zero,0f,90f,15f));
					lineAndAngle.AddAngleArc(new AngleArc(MathFunctions.GetAngleValueInString(angle),Vector2.zero,90-angle,90,120f));
					lineAndAngle.AddAngleArc(new AngleArc("x",Vector2.zero,0f,90f-angle));

					lineAndAngle.DrawDiagram ();

					this.SetAnswerValue (new float[]{90f-angle});


				}
			}


			#endregion
			#region level 5
			if (level == 5) 
			{
				selector = GetRandomSelector(1, 6);
				if(selector==1)
				{
					QuestionLatext.text ="Find a, x, y, z :";
					int angle1=Random.Range(70,110);
					while(angle1==90)
					{
						angle1=Random.Range(70,110);
					}
					int angle2=Random.Range(40,angle1-20);

					lineAndAngle.AddLinePoint(new LinePoint("B",new Vector2(0f,50f),0,true,140f,1));
					lineAndAngle.AddLinePoint(new LinePoint("A",new Vector2(0f,50f),180,true,140f,-1));
					lineAndAngle.AddLinePoint(new LinePoint("D",new Vector2(0f,-50f),0,true,140f));
					lineAndAngle.AddLinePoint(new LinePoint("C",new Vector2(0f,-50f),180,true,140f));

					newRadius = Mathf.Abs(50f/Mathf.Sin(Mathf.Deg2Rad * angle1));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,angle1,false,newRadius));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,angle1,newRadius); 
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint,angle1,true,70f));


					lineAndAngle.AddAngleArc(new AngleArc(angle1.ToString()+MathFunctions.deg,newPoint,180,180+angle1));

					newRadius = Mathf.Abs(50f/Mathf.Sin(Mathf.Deg2Rad *  (angle1+180f)));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero, (angle1+180f),false,newRadius));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,(angle1+180),newRadius); 
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint, (angle1+180f),true,70f));
					newRadius = Mathf.Abs(100f/Mathf.Sin(Mathf.Deg2Rad * angle2));

					lineAndAngle.AddLinePoint(new LinePoint("",newPoint, angle2,false,newRadius));
					lineAndAngle.AddAngleArc(new AngleArc("y",newPoint,180,180+angle1,35f));
					lineAndAngle.AddAngleArc(new AngleArc(angle2.ToString()+MathFunctions.deg,newPoint,0,angle2));
					lineAndAngle.AddAngleArc(new AngleArc("x",newPoint,angle2,angle1,35f));
				
					newPoint = MathFunctions.PointAtDirection(newPoint,angle2,newRadius);
					lineAndAngle.AddAngleArc(new AngleArc("z",newPoint,180,180+angle2,35f));
					lineAndAngle.AddAngleArc(new AngleArc("a",newPoint,180+angle2,360,35f));

					lineAndAngle.DrawDiagram ();

					this.SetAnswerValue (new float[]{180- angle2,angle1-angle2,angle1,angle2});

					lineAndAngle.SetScale (0.8f);

				}
				else if(selector==2)
				{
					QuestionLatext.text ="Find x, y, z :";
					int angle =Random.Range(80,110);
					while(angle==90)
					{
						angle=Random.Range(80,110);
					}
					int angle1 =Random.Range(40,60);

					lineAndAngle.AddLinePoint(new LinePoint("A",new Vector2(-80f,0f),90f,true,130,1));
					lineAndAngle.AddLinePoint(new LinePoint("B",new Vector2(-80f,0f),270f,true,130,-1));



					lineAndAngle.AddLinePoint(new LinePoint("D",new Vector2(80f,0f),90f,true,130));
					lineAndAngle.AddLinePoint(new LinePoint("C",new Vector2(80f,0f),270f,true,130));

					newRadius =  Mathf.Abs(160f / Mathf.Cos ((270+angle1)* Mathf.Deg2Rad));
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(-80f,60f),270+angle1,false,newRadius));
					newPoint = MathFunctions.PointAtDirection (new Vector2 (-80f, 60f), 270 + angle1, newRadius);
					lineAndAngle.AddAngleArc(new AngleArc(angle1.ToString()+MathFunctions.deg,newPoint,90f,90f+angle1));

					newRadius =  Mathf.Abs(160f / Mathf.Cos ((270+angle)* Mathf.Deg2Rad));
					lineAndAngle.AddLinePoint(new LinePoint("",new Vector2(-80f,60f),270+angle,false,newRadius));
					newPoint = MathFunctions.PointAtDirection (new Vector2 (-80f, 60f), 270 + angle, newRadius);
					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg,newPoint,90f,90f+angle));

					lineAndAngle.AddAngleArc(new AngleArc("x",new Vector2(-80f,60f),270f+angle,270+180));


					lineAndAngle.AddAngleArc(new AngleArc("z",new Vector2(-80f,60f),270f,270+angle1));

					lineAndAngle.AddAngleArc(new AngleArc("y",new Vector2(-80f,60f),270f+angle1,270+angle));


					lineAndAngle.DrawDiagram ();

					this.SetAnswerValue (new float[]{180- angle,angle-angle1,angle1});

					lineAndAngle.SetScale (0.8f);

				}
				else if(selector ==3)
				{
					coeff1 = Random.Range(10, 61);
					coeff2 = Random.Range(50, 101);
					coeff3 = Random.Range(3, 11);
					coeff4 = Random.Range(2,10);
					while ((40 * (coeff2 - coeff1) % (coeff3 - 1)) != 0 || (coeff2 < (coeff1 + 10)))
						coeff1 = Random.Range(10, 61);
					QuestionLatext.text = "A line cuts two parallel lines and two of the corresponding angles measure " +coeff4 +"x^{2} + "+ coeff3 + "x + " + coeff1 + MathFunctions.deg + " and "+coeff4 +"x^{2} + x + " + coeff2 + MathFunctions.deg + ". Find x :";
					float ans = (float)(coeff2 - coeff1) / (float)(coeff3 - 1);
					this.SetAnswerValue(new float[]{ans});
				}
				else if(selector ==4)
				{
					
					QuestionLatext.text ="Find X :";

					int angle = Random.Range (120, 150);
				    coeff1 = Random.Range (10, 30);
					int answer = 180 - angle - coeff1;
					float angle1 = (answer + coeff1) / 2f;
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,angle1,false,70));
					lineAndAngle.AddLinePoint(new LinePoint("",Vector2.zero,180+angle1,false,70));
					newPoint = MathFunctions.PointAtDirection (Vector2.zero, angle1, 70f); 
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint,360-angle1,true,70f));
					lineAndAngle.AddAngleArc(new AngleArc(angle.ToString()+MathFunctions.deg,newPoint,180+angle1,180+angle1+angle));
					newPoint =MathFunctions.PointAtDirection (Vector2.zero, 180+angle1, 70f); 
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint,180-angle1,true,70f));
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint,360-angle1,true));
					lineAndAngle.AddLinePoint(new LinePoint("",newPoint,180+angle1,true,70));
					lineAndAngle.AddAngleArc(new AngleArc("x +"+coeff1+MathFunctions.deg,newPoint,-angle1,angle1+10,55f));

					lineAndAngle.DrawDiagram ();

					this.SetAnswerValue(new float[]{answer});



				}
				else if(selector==5)
				{
					QuestionLatext.text = "Find \\angle{PFQ} :";
					int angle = Random.Range (120, 140);
					int angle1 = Random.Range (60, 80);
					lineAndAngle.AddLinePoint (new LinePoint ("F", Vector2.zero, 0, false, 0, 1));

					newRadius = Mathf.Abs (50f / Mathf.Sin (angle * Mathf.Deg2Rad));
					lineAndAngle.AddLinePoint (new LinePoint ("P", Vector2.zero, angle, false, newRadius, 1));


					newPoint =MathFunctions.PointAtDirection(Vector2.zero,  angle,newRadius);
					lineAndAngle.AddLinePoint (new LinePoint ("E", newPoint, angle, true, 70));

					lineAndAngle.AddAngleArc (new AngleArc (angle.ToString () + MathFunctions.deg, newPoint, 0, angle));

					float newAngle = (180 + angle1);
					newRadius = Mathf.Abs (50f / Mathf.Sin (newAngle * Mathf.Deg2Rad));
					lineAndAngle.AddLinePoint (new LinePoint ("Q", Vector2.zero, newAngle, false, newRadius, -1));

					newPoint =MathFunctions.PointAtDirection(Vector2.zero,  newAngle,newRadius);
					lineAndAngle.AddLinePoint (new LinePoint ("G", newPoint, newAngle, true, 70f));

					lineAndAngle.AddAngleArc (new AngleArc (angle1.ToString () + MathFunctions.deg, newPoint, 0, angle1));

					lineAndAngle.AddLinePoint (new LinePoint ("C", new Vector2 (0f, 50f), 0, true));
					lineAndAngle.AddLinePoint (new LinePoint ("D", new Vector2 (0f, 50f), 180, true));
					lineAndAngle.AddLinePoint (new LinePoint ("A", new Vector2 (0f, -50f), 0, true));
					lineAndAngle.AddLinePoint (new LinePoint ("B", new Vector2 (0f, -50f), 180, true));

					lineAndAngle.DrawDiagram ();

					this.SetAnswerValue (new float[]{180-angle+angle1});

					lineAndAngle.SetScale (0.8f);
				}
			}
			#endregion

			userAnswerLaText = answerButton.gameObject.GetChildByName<TEXDraw>("Text");
			userAnswerLaText.text = "";
		}

		public override void numPadButtonPressed(int value)
		{
			if (ignoreTouches)
			{
				return;
			}
			if (value <= 9)
			{
				userAnswerLaText.text += value.ToString();
			}
			else if (value == 10)
			{    //.
				if (checkLastTextFor(new string[1] { "." }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ".";
			}
			else if (value == 11)
			{   // All Clear
				if (userAnswerLaText.text.Length > 0)
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
			}
			else if (value == 12)
			{   // min
				if (checkLastTextFor(new string[1] {""+ MathFunctions.min }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ""+MathFunctions.min;			
			}
			else if (value == 13)
			{   // Sec

				if (checkLastTextFor(new string[1] { ""+MathFunctions.sec }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ""+MathFunctions.sec;
			}
			else if (value == 14)
			{   // Deg

				if (checkLastTextFor(new string[1] {""+ MathFunctions.deg }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ""+MathFunctions.deg;
			
			}
			else if (value == 15)
			{   // comma
				if (checkLastTextFor(new string[1] { "," }))
				{
					userAnswerLaText.text = userAnswerLaText.text.Substring(0, userAnswerLaText.text.Length - 1);
				}
				userAnswerLaText.text += ",";
			}
		}

		private void SetAnswerValue(float[] answers)
		{
			Answer = "";
			alternateAnswer = "";
				
			int length = answers.Length;
			for(int i=0;i<length;i++)
			{
				Answer += MathFunctions.GetAngleValueInString (answers[i]);
				alternateAnswer += answers[i] + MathFunctions.deg;

				if ( i < length - 1) 
				{
					Answer += ",";
					alternateAnswer += ",";
				}

			}
		}

	
	}


}

