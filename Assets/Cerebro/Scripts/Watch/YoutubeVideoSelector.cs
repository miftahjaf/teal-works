using UnityEngine;
using System.Collections;
using EnhancedUI.EnhancedScroller;
using System.Collections.Generic;
using System;
namespace Cerebro
{
	public class YoutubeVideoSelector : MonoBehaviour,IEnhancedScrollerDelegate
	{
		public EnhancedScrollerCellView youtubeVideoCell;
		public List<VideoData> youtubeVideosData;
		private EnhancedScroller youtubeList;
		public Action<int> nextPageLoaded;

		public void Start()
		{
			youtubeVideosData = new List<VideoData>();
			youtubeList = GetComponent<EnhancedScroller> ();
			youtubeList.Delegate = this;
			youtubeList.ReloadData ();
		}
		#region IEnhancedScrollerDelegate implementation

		public int GetNumberOfCells (EnhancedScroller scroller)
		{
			return youtubeVideosData.Count;
		}

		public float GetCellViewSize (EnhancedScroller scroller, int dataIndex)
		{
			return 100;
		}

		public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
		{
			if (nextPageLoaded != null) {
				nextPageLoaded.Invoke (dataIndex);
			}

			YoutubeCell cellView = scroller.GetCellView (youtubeVideoCell) as YoutubeCell;
			cellView.SetYoutubeData (youtubeVideosData[dataIndex]);
			return cellView;
		}

		#endregion

		public void Reload()
		{
			youtubeList.Delegate = this;
			youtubeList.ReloadData ();
		}
	}
}
