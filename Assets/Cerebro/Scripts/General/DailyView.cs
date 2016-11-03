using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Cerebro
{
	public class DailyView : MonoBehaviour {

		public GameObject[] TaskList;



		void Start () 
		{
			
		}

		void FillDummyDailyTasks()
		{
			LaunchList.instance.CurrDaily = new Daily ();
			List<DailyTaskItem> allTasks = new List<DailyTaskItem> ();
			DailyTaskItem item = new DailyTaskItem ();
			item.TaskDesc = "Practice";
			item.TaskIcon = null;
			item.IsDone = true;
			item.CurrType = DailyTaskItem.TaskType.Practice;
			allTasks.Add (item);
			DailyTaskItem item1 = new DailyTaskItem ();
			item1.TaskDesc = "WC";
			item1.TaskIcon = null;
			item1.IsDone = true;
			item1.CurrType = DailyTaskItem.TaskType.WC;
			allTasks.Add (item1);
			DailyTaskItem item2 = new DailyTaskItem ();
			item2.TaskDesc = "Verbalize";
			item2.TaskIcon = null;
			item2.IsDone = true;
			item2.CurrType = DailyTaskItem.TaskType.Verbalize;
			allTasks.Add (item2);
			LaunchList.instance.CurrDaily.AllTasks = allTasks;
		}

		public void Initialize()
		{
			FillDummyDailyTasks ();
			for (int i = 0; i < 3; i++) {
				TaskList [i].SetActive (false);
			}
			List<DailyTaskItem> CurrAllTasks = LaunchList.instance.CurrDaily.AllTasks;
			for (int i = 0; i < CurrAllTasks.Count && i < 3; i++) {
				TaskList [i].SetActive (true);
				if (CurrAllTasks [i].TaskIcon != null) {
					TaskList [i].transform.FindChild ("Icon").gameObject.SetActive (true);
					TaskList [i].transform.FindChild ("Icon").GetComponent<Image> ().sprite = CurrAllTasks [i].TaskIcon;
				} else {
					TaskList [i].transform.FindChild ("Icon").gameObject.SetActive (false);
				}
				TaskList [i].transform.FindChild ("Desc").GetComponent<Text> ().text = CurrAllTasks [i].TaskDesc;
				if (CurrAllTasks [i].IsDone) {
					TaskList [i].transform.FindChild ("Tick").gameObject.SetActive (true);
					TaskList [i].GetComponent<Button> ().onClick.RemoveAllListeners ();
				} else if (CurrAllTasks[i].CurrType == DailyTaskItem.TaskType.WC) {
					TaskList [i].GetComponent<Button> ().onClick.AddListener (WelcomeScript.instance.OpenEnglishAssessment);
				} else if (CurrAllTasks[i].CurrType == DailyTaskItem.TaskType.Verbalize) {
					TaskList [i].GetComponent<Button> ().onClick.AddListener (WelcomeScript.instance.OpenVerbalize);
				}
			}
			StartCoroutine (AnimateTicks());
		}

		IEnumerator AnimateTicks()
		{
			yield return new WaitForSeconds (1);
			for (int i = 0; i < TaskList.Length; i++) {
				GameObject gTemp = TaskList [i].transform.FindChild ("Tick").gameObject;
				if (gTemp.activeSelf) {
					gTemp.GetComponent<MaterialUI.EasyTween> ().Tween("EaseIn");
					yield return new WaitForSeconds (0.3f);
				}
			}
		}

		public void TaskClicked(int index)
		{
			
		}
	}

	public class Daily
	{
		public List<DailyTaskItem> AllTasks;
		public bool[] WeeklyDoneStatus;
	}

	public class DailyTaskItem
	{
		public string TaskDesc;
		public Sprite TaskIcon;
		public bool IsDone;
		public TaskType CurrType;

		public enum TaskType
		{
			WC,
			Verbalize,
			Practice
		}
	}
}