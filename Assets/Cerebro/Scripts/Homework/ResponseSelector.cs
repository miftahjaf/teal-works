using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;

namespace Cerebro
{
	public class ResponseSelector : MonoBehaviour, IEnhancedScrollerDelegate
	{
		public EnhancedScrollerCellView ResponseDataCellView;

		private EnhancedScroller enhancedScroller;

		[HideInInspector]
		public bool isResponseFeedLoading;

		private List<ResponseData> mCurrResponses;
		public List<ResponseData> currResponses
		{
			get { return mCurrResponses; }
			set { mCurrResponses = value; }
		}


		void Start () 
		{
			enhancedScroller = GetComponent<EnhancedScroller> ();
			isResponseFeedLoading = false;
		}

		public void ReloadData ()
		{
			enhancedScroller.Delegate = this;
			enhancedScroller.ReloadData ();
		}

		#region IEnhancedScrollerDelegate implementation
		public int GetNumberOfCells (EnhancedScroller scroller)
		{
			return currResponses.Count;
		}

		public float GetCellViewSize (EnhancedScroller scroller, int dataIndex)
		{
			return currResponses[dataIndex].cellSize;
		}

		public EnhancedScrollerCellView GetCellView (EnhancedScroller scroller, int dataIndex, int cellIndex)
		{
			ResponseDataCellView cellView = scroller.GetCellView (ResponseDataCellView) as ResponseDataCellView; 
			cellView.InitializeCell (currResponses [dataIndex]);
			return cellView;
		}
		#endregion
	}

	public class ResponseData
	{
		public string id;
		public string title;
		public string responseText;
		public string profilePicUrl;
		public System.DateTime createdAt;
		public ResponseType responseType;
		public TeacherData teacherData;
		public bool isLateSubmission;
		public CommentFrom from;
		public bool isFromLocal;
		public Sprite profilePicSprite;
		public float cellSize;

		public enum ResponseType
		{
			Submission,
			Comment
		}

		public enum CommentFrom
		{
			Teacher,
			Me
		}

		public ResponseData()
		{
			id = "";
			title = "";
			responseText = "";
			profilePicUrl = "";
			createdAt = System.DateTime.Now;
			responseType = ResponseType.Submission;
			teacherData = new TeacherData ();
			isLateSubmission = false;
			from = CommentFrom.Me;
			isFromLocal = false;
			profilePicSprite = null;
			cellSize = 0;
		}

		public ResponseData(string _id, string _title, string _responseText, string _profilePicUrl, System.DateTime _createdAt, ResponseType _responseType, CommentFrom _from, bool _isFromLocal)
		{
			id = _id;
			title = _title;
			responseText = _responseText;
			profilePicUrl = _profilePicUrl;
			createdAt = _createdAt;
			responseType = _responseType;
			teacherData = new TeacherData ();
			isLateSubmission = false;
			from = _from;
			isFromLocal = _isFromLocal;
			profilePicSprite = null;
			cellSize = 0;
		}
	}
}