using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;

namespace Cerebro {

	public class HomeworkSelector : MonoBehaviour ,IEnhancedScrollerDelegate
	{
		public EnhancedScrollerCellView HomeworkDataCellView;
		public EnhancedScrollerCellView HomeworkTitleCellView;

		public GameObject parentForAssignment;

		[HideInInspector]
		public bool isHomeworkFeedLoading;

		private EnhancedScroller enhancedScroller;

		private List<HomeworkCell> mCurrHomeworks;
		public List<HomeworkCell> currHomeworks
		{
			get { return mCurrHomeworks; }
			set { mCurrHomeworks = value; }
		}

		void Awake () 
		{
			enhancedScroller = GetComponent<EnhancedScroller> ();
			isHomeworkFeedLoading = false;
		}

		public void ReloadData ()
		{
			enhancedScroller.Delegate = this;
			enhancedScroller.ReloadData ();
		}

		#region IEnhancedScrollerDelegate implementation
		public int GetNumberOfCells (EnhancedScroller scroller)
		{
			return currHomeworks.Count;
		}

		public float GetCellViewSize (EnhancedScroller scroller, int dataIndex)
		{
			if (currHomeworks [dataIndex] is HomeworkDataCell) {
				return 100;
			} else {
				return 50;
			}
		}

		public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
		{
//			Debug.Log (dataIndex+" "+currHomeworks.Count);
			if (dataIndex == currHomeworks.Count - 1) {
				transform.parent.GetComponent<HomeworkContainer> ().LoadNextPage ();
				isHomeworkFeedLoading = true;
			}
			if (currHomeworks [dataIndex] is HomeworkDataCell) {
				HomeworkDataCellView cellView = scroller.GetCellView (HomeworkDataCellView) as HomeworkDataCellView; 
				cellView.InitializeCell ((HomeworkDataCell)currHomeworks [dataIndex], parentForAssignment);
				return cellView;
			} else {
				HomeworkTitleCellView cellView = scroller.GetCellView (HomeworkTitleCellView) as HomeworkTitleCellView; 
				cellView.InitializeCell ((HomeworkTitleCell)currHomeworks [dataIndex]);
				return cellView;
			}

		}
		#endregion

	}

}
