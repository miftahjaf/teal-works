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
			/*statisticsHelper.SetGridParameters (new Vector2 (20, 22), 22);
			statisticsHelper.ShiftGraphOrigin (new Vector2 (-7, -7));
			statisticsHelper.SetStatisticsType (StatisticsType.VerticalBar);
			statisticsHelper.SetGraphParameters (new StatisticsAxis[]
				{
					new StatisticsAxis().SetStatisticsValues
					(
						new List<StatisticsValue>{
							new StatisticsValue("Ramiz",20),
							new StatisticsValue("Vijaypal",40),
							new StatisticsValue("Ankit",50),
							new StatisticsValue("Misbah",50),
							new StatisticsValue("Vraj",70)
						}
					).SetAxisName("Name").SetPointOffset(3),
					new StatisticsAxis().SetOffsetValue(10).SetAxisName("Value").SetPointOffset(2)

				}
			);
			statisticsHelper.DrawGraph ();*/
			statisticsHelper.SetGridParameters (new Vector2 (18, 18), 13);
			statisticsHelper.SetStatisticsType (StatisticsType.HorizontalBar);
			statisticsHelper.ShiftPosition (new Vector2 (-270, 200));
			statisticsHelper.SetInteractable (true);
			statisticsHelper.SetGraphParameters (new StatisticsAxis[]
				{
					new StatisticsAxis().SetOffsetValue(5).SetAxisName("Marks").SetPointOffset(2),
					new StatisticsAxis().SetStatisticsValues
					(
						new List<StatisticsValue>{
							new StatisticsValue("Jan", 30),
							new StatisticsValue("Nov", 35),
							new StatisticsValue("Sep", 25),
							new StatisticsValue("July", 20),
						}
					).SetAxisName("Name").SetPointOffset(3)

				

				}
			);
			statisticsHelper.SetGraphTitle ("ashdkjadhskjahsdjkasdh");
			statisticsHelper.DrawGraph ();
			statisticsHelper.PlotPoint (new Vector2 (50, 2));
		}
}
}
