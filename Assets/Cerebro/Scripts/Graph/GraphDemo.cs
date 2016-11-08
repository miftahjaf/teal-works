using UnityEngine;
using System.Collections;
using Cerebro;
using System.Collections.Generic;


public class GraphDemo : MonoBehaviour {

	public GraphHelper graphHelper;
	GraphLine graphLine ;
	void Start () 
	{

		graphHelper.Reset ();
		graphHelper.SetGridParameters (new Vector2 (20, 20),25);
		graphHelper.DrawGraph ();
		int side1 = Random.Range (4, 9);  // square side
		int side2 = Random.Range (1, 1 + side1 / 3);  //circle radius
		int xCord = Random.Range (-7, 10 - side1);
		int yCord = Random.Range (-7, 10 - side1);                        graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side2, yCord), new Vector2 (xCord + side1 - side2, yCord)},Vectrosity.LineType.Continuous);
		graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side2, yCord + side1), new Vector2 (xCord + side1 - side2, yCord + side1)},Vectrosity.LineType.Continuous);
		graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord, yCord + side2), new Vector2 (xCord, yCord + side1 - side2)},Vectrosity.LineType.Continuous);
		graphHelper.DrawDiagram (new List<Vector2> (){new Vector2 (xCord + side1, yCord + side2), new Vector2 (xCord + side1, yCord + side1 - side2)},Vectrosity.LineType.Continuous);                        graphHelper.DrawArc (new Vector2 (xCord, yCord), new Vector2(xCord + side2, yCord), new Vector2 (xCord, yCord + side2));
		graphHelper.DrawArc (new Vector2 (xCord + side1, yCord), new Vector2 (xCord + side1, yCord + side2), new Vector2(xCord + side1 - side2, yCord));
		graphHelper.DrawArc (new Vector2 (xCord + side1, yCord + side1), new Vector2 (xCord + side1 - side2, yCord + side1), new Vector2(xCord + side1, yCord + side1 - side2));
		graphHelper.DrawArc (new Vector2 (xCord, yCord + side1), new Vector2 (xCord, yCord + side1 - side2), new Vector2(xCord + side2, yCord + side1));

	}



}
