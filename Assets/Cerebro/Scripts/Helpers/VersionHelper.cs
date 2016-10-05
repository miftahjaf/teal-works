using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public class VersionHelper : MonoBehaviour
	{
		[SerializeField]
		private string mVersionNumber;

		private static string VersionNumber;

		void Awake() {
			VersionNumber = mVersionNumber;
		}

		public static string GetVersionNumber() {
			return VersionNumber;
		}
	}
}
