using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public class ParentForWC : MonoBehaviour 
	{
		
		public void BackFromWC()
		{
			transform.parent.GetComponent<AssignmentResponseView> ().RefreshResponseFeed ();
		}

	}
}
