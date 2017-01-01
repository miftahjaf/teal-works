using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public class ParentForAssignment : MonoBehaviour 
	{
		
		public void BackFromAssignment()
		{
			transform.parent.GetComponent<HomeworkContainer> ().RefreshFeed ();
		}

	}
}
