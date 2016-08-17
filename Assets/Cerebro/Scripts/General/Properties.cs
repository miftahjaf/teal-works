using UnityEngine;
using System.Collections;

namespace Cerebro {
	public class CerebroProperties {

		public float PollActiveTimeAlert { get; set; }
		public float MaxActiveTimeAlert { get; set; }

		public int StreakMultiplier { get; set; }
		public bool ShowCoins { get; set; }

		private static CerebroProperties m_Instance;
		public static CerebroProperties instance
		{
			get
			{
				return m_Instance;
			}

			set {
				m_Instance = value;
			}
		}

		public CerebroProperties () {
			PollActiveTimeAlert = 300f;
			MaxActiveTimeAlert = 3600f;
			StreakMultiplier = 10;
			ShowCoins = false;
		}
	}
}
