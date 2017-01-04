using UnityEngine;
using System.Collections;

namespace Cerebro
{
	public class HomeworkAssessments : MonoBehaviour 
	{
		public GameObject NotGradedGm;
		
		public void InitializeAssessments(HomeworkDataCell dataCell)
		{
			AssessmentObject[] objs = transform.GetComponentsInChildren<AssessmentObject> (true);
			for (int i = 0; objs != null && i < objs.Length; i++) {
				Destroy (objs[i].gameObject);
			}
			if (dataCell.currGradedScore.Count > 0) {
				NotGradedGm.SetActive (false);
				for (int i = 0; i < dataCell.currGradedScore.Count; i++) {
					GameObject assessmentObject = PrefabManager.InstantiateGameObject (Cerebro.ResourcePrefabs.AssessmentObject, transform);
					assessmentObject.GetComponent<AssessmentObject> ().InitializeAssessment (dataCell.currGradedScore [i].criteria, dataCell.currGradedScore [i].gradedScore);
				}
			} else {
				NotGradedGm.SetActive (true);
			}
		}

	}
}