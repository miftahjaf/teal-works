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
			statisticsHelper.SetGridParameters (new Vector2 (20, 22), 22);
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
			statisticsHelper.DrawGraph ();
		}
}
}
