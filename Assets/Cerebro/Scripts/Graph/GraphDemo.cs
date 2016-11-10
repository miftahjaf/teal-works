using UnityEngine;
using System.Collections;
using Cerebro;
using System.Collections.Generic;


public class GraphDemo : MonoBehaviour {

	public GraphHelper graphHelper;
	public DiagramHelper diagramHelper;
	GraphLine graphLine ;

	void Start () 
	{
		
		/*graphHelper.Reset ();
		graphHelper.SetGridParameters (new Vector2 (20, 20),25);
		graphHelper.DrawGraph ();
		graphHelper.SetGraphQuesType (GraphQuesType.RotateDiagram);
		int side1 = Random.Range (4, 9);  // square side
		int side2 = Random.Range (1, 1 + side1 / 3);  //circle radius
		int xCord = Random.Range (-7, 10 - side1);
		int yCord = Random.Range (-7, 10 - side1);                        graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side2, yCord), new Vector2 (xCord + side1 - side2, yCord)},Vectrosity.LineType.Continuous);
		graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side2, yCord + side1), new Vector2 (xCord + side1 - side2, yCord + side1)},Vectrosity.LineType.Continuous);
		graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord, yCord + side2), new Vector2 (xCord, yCord + side1 - side2)},Vectrosity.LineType.Continuous);
		graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side1, yCord + side2), new Vector2 (xCord + side1, yCord + side1 - side2)},Vectrosity.LineType.Continuous);                        
		graphHelper.DrawArc (new Vector2 (xCord, yCord), new Vector2(xCord + side2, yCord), new Vector2 (xCord, yCord + side2));
		graphHelper.DrawArc (new Vector2 (xCord + side1, yCord), new Vector2 (xCord + side1, yCord + side2), new Vector2(xCord + side1 - side2, yCord));
		graphHelper.DrawArc (new Vector2 (xCord + side1, yCord + side1), new Vector2 (xCord + side1 - side2, yCord + side1), new Vector2(xCord + side1, yCord + side1 - side2));
		graphHelper.DrawArc (new Vector2 (xCord, yCord + side1), new Vector2 (xCord, yCord + side1 - side2), new Vector2(xCord + side2, yCord + side1));*/
		int xCord = Random.Range (-7, 8);
		int xCord1 = Random.Range (-7, 8);
		int xCord2 = Random.Range (-7, 8);
		int yCord = Random.Range (-7, 8);
		int yCord1 = Random.Range (-7, 8);
		int yCord2 = Random.Range (-7, 8);
		int xCord3 = Random.Range (-7, 8);
		int yCord3 = Random.Range (-7, 8);
		do{	
			 xCord = Random.Range (-7, 8);
			 xCord1 = Random.Range (-7, 8);
			 xCord2 = Random.Range (-7, 8);
			 yCord = Random.Range (-7, 8);
			 yCord1 = Random.Range (-7, 8);
			 yCord2 = Random.Range (-7, 8);
		} while (Mathf.Abs (xCord - xCord1) + Mathf.Abs (yCord - yCord1) < 5 || Mathf.Abs (xCord2 - xCord1) + Mathf.Abs (yCord2 - yCord1) < 5 || Mathf.Abs (xCord - xCord2) + Mathf.Abs (yCord - yCord2) < 5 || Mathf.Abs ((xCord - xCord1) * (yCord2 - yCord1) - (yCord - yCord1) * (xCord2 - xCord1)) < 5);
		graphHelper.Reset ();
		graphHelper.SetGridParameters(new Vector2(20,20),22);
		graphHelper.DrawGraph ();
		graphHelper.DrawDiagram(new List<Vector2>(){new Vector2(xCord,yCord),new Vector2(xCord1,yCord1),new Vector2(xCord2,yCord2),new Vector2(xCord, yCord)});
		graphHelper.DrawMovebleDiagram(new List<Vector2>(){new Vector2(xCord3,yCord3),new Vector2(Random.Range (-7, 8),Random.Range (-7, 8)),new Vector2(Random.Range (-7, 8),Random.Range (-7, 8)),new Vector2(xCord3, yCord3)});
	}



}
