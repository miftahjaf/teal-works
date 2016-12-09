using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cerebro
{
	public class StatisticsAxis
	{
		public string axisName;
		public int startValue;
		public int offsetValue;
		public List<StatisticsValue> statisticsValues;
		public int pointOffset;
		public StatisticsAxis()
		{
			offsetValue = 1;
			axisName = "";
			startValue = 0;
			statisticsValues = new List<StatisticsValue> ();
			pointOffset = 1;
		}

		public StatisticsAxis SetOffsetValue(int _offsetValue)
		{
			offsetValue = _offsetValue;
			return this;
		}

		public StatisticsAxis SetAxisName(string _axisName="")
		{
			axisName = _axisName;
			return this;
		}

		public StatisticsAxis SetStartValue(int _startValue)
		{
			startValue = _startValue;
			return this;
		}

		public StatisticsAxis SetStatisticsValues(List<StatisticsValue> _statisticsValues)
		{
			statisticsValues = _statisticsValues;
			return this;
		}

		public StatisticsAxis SetPointOffset(int _pointOffset)
		{
			pointOffset = _pointOffset;
			return this;
		}
	}

	public class StatisticsValue
	{
		public string name;
		public int[] values;


		public StatisticsValue(string _name,int[] _values)
		{
			name = _name;
			values = _values;
		}

		public StatisticsValue(string _name,int _value)
		{
			name = _name;
			values = new int[]{_value};
		}
	}
}
