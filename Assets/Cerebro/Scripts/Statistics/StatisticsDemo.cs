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

			statisticsHelper.SetStatisticsType (StatisticsType.PieToDrag);
			statisticsHelper.SetPieParameters (
				new List<string> (){ "Ramiz", "Negi", "Ankit", "Sagar" },
				new List<int> (){ 10, 20, 30, 40 }
			);
			statisticsHelper.SetPieRadius (150f); //250 default radius
			//statisticsHelper.ShiftPosition (new Vector2 (-270, 200));
			statisticsHelper.DrawGraph ();

		}
}
}
