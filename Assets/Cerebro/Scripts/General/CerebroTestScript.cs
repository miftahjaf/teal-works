using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public abstract class CerebroTestScript : MonoBehaviour
	{
		protected string[] options;
		public abstract void ForceOpenScreen (string[] screen, int index, Mission missionData);
		public abstract void ForceOpenScreen (string[] screen, int index, MissionItemData missionItemData);
		public abstract string[] GetOptions ();
	}
}
