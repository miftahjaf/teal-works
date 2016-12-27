using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Cerebro
{
	public class StatisticsDemo : MonoBehaviour
	{
		public StatisticsHelper statisticsHelper;

		void Start () 
		{
			statisticsHelper.Reset ();
	
			/*statisticsHelper.SetGridParameters (new Vector2 (25, 25), 20);
			statisticsHelper.SetStatisticsType (StatisticsType.Line);
			//statisticsHelper.ShiftPosition (new Vector2 (-270, 200));
			statisticsHelper.SetInteractable (true);
			statisticsHelper.SetGraphParameters (new StatisticsAxis[]
				{
					new StatisticsAxis().SetStatisticsValues
					(
						new List<StatisticsValue>{
							new StatisticsValue("Jan",30),
							new StatisticsValue("Nov",10),
							new StatisticsValue("Sep",30),
							new StatisticsValue("July",40),
							new StatisticsValue("Aug",60)

						}
					).SetAxisName("Name").SetPointOffset(2),
					new StatisticsAxis().SetOffsetValue(5).SetAxisName("Marks").SetPointOffset(2)
				}
			);
			statisticsHelper.SetSnapValue (new Vector2 (10, 20));
			statisticsHelper.SetGraphTitle ("ashdkjadhskjahsdjkasdh");
			statisticsHelper.DrawGraph ();*/

			/*statisticsHelper.SetStatisticsType (StatisticsType.PieToDrag);
			statisticsHelper.SetPieParameters (
				new List<string> (){ "Ramiz", "Negi", "Ankit", "Sagar" },
				new List<int> (){ 10, 20, 30, 40 }
			);
			statisticsHelper.SetPieRadius (150f); //250 default radius
			//statisticsHelper.ShiftPosition (new Vector2 (-270, 200));
			statisticsHelper.DrawGraph ();*/

			List<int> coeff = new List<int> ();
			int axisValueOffset = 10;
			int gridValOffset = axisValueOffset / 2;
			int minValue, maxValue;
			int numberOfBars = 4;
			do {
				minValue = Random.Range (2, 10);
				maxValue = Random.Range (minValue, 11);
			} while (maxValue - minValue < numberOfBars - 1);

			List<string> Sports = new List<string>() {"Cricket", "Badminton", "Hockey", "Tennis", "Swimming", "Football"};
			Sports.Shuffle ();

			coeff.Add (gridValOffset * minValue);
			for (int i = 1; i < numberOfBars - 1; i++){
				coeff.Add (gridValOffset * Random.Range (minValue + 1, maxValue));
			}
			coeff.Add (gridValOffset * maxValue);
			coeff.Shuffle ();

			statisticsHelper.SetGridParameters (new Vector2 (14, 14), 15f);
			statisticsHelper.SetStatisticsType (StatisticsType.Line);
			statisticsHelper.ShiftPosition (new Vector2 (-270, 235));
			statisticsHelper.SetGraphParameters (new StatisticsAxis[]
				{
					new StatisticsAxis ().SetStatisticsValues
					(
						new List<StatisticsValue>(){
							new StatisticsValue (Sports[0].Substring (0, 3), coeff[0]),
							new StatisticsValue (Sports[1].Substring (0, 3), coeff[1]),
							new StatisticsValue (Sports[2].Substring (0, 3), coeff[2]),
							new StatisticsValue (Sports[3].Substring (0, 3), coeff[3]),
						}
					).SetAxisName ("Test Month").SetPointOffset (3),
					new StatisticsAxis ().SetOffsetValue (axisValueOffset).SetAxisName ("Marks").SetPointOffset (2)
				}
			);
			statisticsHelper.DrawGraph ();
		}
}
}
