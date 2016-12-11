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
			statisticsHelper.SetStatisticsType (StatisticsType.HorizontalBar);
			//statisticsHelper.ShiftPosition (new Vector2 (-270, 200));
			//statisticsHelper.SetInteractable (true);
			statisticsHelper.SetGraphParameters (new StatisticsAxis[]
				{
					new StatisticsAxis().SetOffsetValue(5).SetAxisName("Marks").SetPointOffset(2),
					new StatisticsAxis().SetStatisticsValues
					(
						new List<StatisticsValue>{
							new StatisticsValue("Jan",30),
							new StatisticsValue("Nov",new int[]{5,15}),
							new StatisticsValue("Sep",new int[]{10,25,}),
							new StatisticsValue("July",new int[]{10,20}),
						}
					).SetAxisName("Name").SetPointOffset(3)
				}
			);
			statisticsHelper.SetGraphTitle ("ashdkjadhskjahsdjkasdh");
			statisticsHelper.DrawGraph ();
			statisticsHelper.PlotPoint (new Vector2 (50, 2));*/
			statisticsHelper.SetStatisticsType (StatisticsType.Pie);
			statisticsHelper.SetPieParameters (
				new List<string> (){ "Ramiz", "Negi", "Ankit", "Sagar" },
				new List<int> (){ 10, 20, 30, 40 }
			);
			statisticsHelper.SetPieRadius (250f); //250 default radius
			//statisticsHelper.ShiftPosition (new Vector2 (-270, 200));
			statisticsHelper.DrawGraph ();
		}
}
}
