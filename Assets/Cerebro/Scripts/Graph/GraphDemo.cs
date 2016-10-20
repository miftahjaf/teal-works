using UnityEngine;
using System.Collections;
using Cerebro;
public class GraphDemo : MonoBehaviour {

	public GraphHelper graphHelper;

	void Start () 
	{

		graphHelper.Reset ();
		graphHelper.SetGridParameters (new Vector2 (20, 20),25);
		graphHelper.ShiftGraphOrigin (new Vector2 (6, 5));
		graphHelper.SetGraphParameters (new  Vector2 (5, 4));
		graphHelper.SetFontMultiplier (0.6f);
		//SetSnapValue (12.5f);
		graphHelper.DrawGraph ();
		//graphHelper.PlotPoint (new Vector2 (20, 20),"A");
		//graphHelper.PlotPoint (new Vector2 (20, -20));
		//graphHelper.PlotPoint (new Vector2 (30, -20));
		//graphHelper.PlotPoint (new Vector2 (30, 70));
		//graphHelper.PlotPoint (new Vector2 (30, 120));
		//graphHelper.PlotPoint (new Vector2 (1000, 120));
		graphHelper.DrawRanomLine();
		//graphHelper.DrawRanomLine();
		//graphHelper.DrawRanomLine();
	}
	

}
