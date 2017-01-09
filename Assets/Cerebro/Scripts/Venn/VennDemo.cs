using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cerebro
{
	public class VennDemo : MonoBehaviour
	{
		public VennHelper venHelper;

		public void Start()
		{
			venHelper.SetDiagramValues (new List<string> (){"8 9 10", "1\n 3 \n 5 ","0 \n 2 \n7","8 9 5" });
			//venHelper.SetIsInteractable (true);
			venHelper.SetFillValue(0);
			venHelper.SetFillValue(3);
			venHelper.SetVennType (VennType.Overlapping);
			venHelper.Draw ();
		}

	}
}
