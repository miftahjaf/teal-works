//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MaterialUI;
using EnhancedUI.EnhancedScroller;

namespace Cerebro
{
	public class AssessmentEventArgs : EventArgs
	{
		public string practiceId;
		public string KCID;
	}


	public class AssessmentSelector : MonoBehaviour,IEnhancedScrollerDelegate
	{
		private List<GameObject> m_SelectionItems;
		public List<GameObject> selectionItems
		{
			get { return m_SelectionItems; }
		}

		private List<AssessmentItem> m_OptionDataList;
		public List<AssessmentItem> optionDataList
		{
			get { return m_OptionDataList; }
			set { m_OptionDataList = value; }
		}

		public Scrollbar scrollElement;

		public ScrollRect scrollRect;

		[SerializeField]
		private EnhancedScrollerCellView m_OptionTemplate;

		[SerializeField]
		private EnhancedScrollerCellView m_OptionChildTemplate;

		public EnhancedScroller enhanceScroller;

		public event EventHandler AssessmentSelected;

		private string[] stripColors = new string[]{"ff5541","2c96dc","29cdb1","7d33ff","ff9633"};

		private float animStartTime;

		private bool canUpdateList;
	
		public void Initialize(List<PracticeItems> assessments)
		{
			enhanceScroller.Delegate = this;
			canUpdateList = true;
			//m_OptionDataList = assessments;
			m_OptionDataList =new List<AssessmentItem>();
			List<PracticeItems> practiceItems = assessments;
			int cnt = 0;
			foreach (PracticeItems praticeItem in practiceItems)
			{
				praticeItem.stripColor =CerebroHelper.HexToRGB (stripColors [cnt % stripColors.Length]);
				m_OptionDataList.Add (praticeItem);
				cnt++;
			}
			enhanceScroller.ReloadData ();
		}
			
		public void OnItemClick(string practiceId,string KCID)
		{	
			if (AssessmentSelected != null) {
				AssessmentEventArgs args = new AssessmentEventArgs ();
				args.practiceId = practiceId;
				args.KCID = KCID;
				AssessmentSelected.Invoke (this, args);
			}
		}

		public void RefreshActiveCellViews()
		{
			this.enhanceScroller.RefreshActiveCellViews ();
		}

		public void UpdateVerticalScrolllLayout()
		{
			Invoke ("UpdateVerticalScrolllLayoutAfterTime", 0.35f);
		}

		public void UpdateVerticalScrolllLayoutAfterTime()
		{
			scrollRect.verticalNormalizedPosition = Mathf.Clamp (scrollRect.verticalNormalizedPosition, 0f, 1f);
		}
			
		#region EnhancedScroller Handlers

		public int GetNumberOfCells(EnhancedScroller scroller)
		{
			return optionDataList.Count;
		}

		public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
		{
			if (m_OptionDataList [dataIndex] is PracticeItems) {
				return 120f;
			} else {
				return 70f;
			}
		}

		public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
		{

			if (m_OptionDataList [dataIndex] is PracticeItems) {
				AssessmentItemScript cellView = scroller.GetCellView (m_OptionTemplate) as AssessmentItemScript; 
				cellView.Initialise ((PracticeItems)m_OptionDataList [dataIndex], OnItemClick, UpdateKCViewShowStatus);
				return cellView;
			} 
			else 
			{
				KnowledgeComponent KC = (KnowledgeComponent)m_OptionDataList [dataIndex];
				KCItemScript cellView = scroller.GetCellView (m_OptionChildTemplate) as KCItemScript;
				cellView.Initialise ( KC, OnItemClick);
				return cellView;
			}

		}

		#endregion

		public void UpdateKCViewShowStatus(string selectedPracticeItemID)
		{
			
			if (!canUpdateList) {
				return;
			}
			int cnt = 0;
			bool anyKCOpened = false;
			PracticeItems practiceItemSelected =null;
			foreach (AssessmentItem assessmentItem in optionDataList)
			{
				if (assessmentItem is PracticeItems)
				{
					PracticeItems practiceItem = (PracticeItems)assessmentItem;
					if (practiceItem.isKCViewOpened && practiceItem.KnowledgeComponents.Count>0) 
					{
						anyKCOpened = true;
					}
					if (practiceItem.PracticeID == selectedPracticeItemID)
					{
						practiceItemSelected = practiceItem;
						practiceItem.isKCViewOpened = !practiceItem.isKCViewOpened;

					} else {
						practiceItem.isKCViewOpened = false;
					}
				} else {
					cnt++;
				}
			}	
			if (practiceItemSelected !=null && practiceItemSelected.KnowledgeComponents.Count <= 0 && !anyKCOpened) 
			{
				Debug.Log ("Update Data ");
				enhanceScroller.RefreshActiveCellViews ();
				return;
			}
			StopCoroutine ("UpdateData");
			StartCoroutine ("UpdateData", cnt);	
		}
			
		public IEnumerator UpdateData(int cnt)
		{
			canUpdateList = false;
			List<AssessmentItem> practiceItems = new List<AssessmentItem>(optionDataList);
			optionDataList = new List<AssessmentItem> ();
			foreach (AssessmentItem assessmentItem in practiceItems)
			{
				if (assessmentItem is PracticeItems) {
					PracticeItems practiceItem = (PracticeItems)assessmentItem;
					optionDataList.Add (assessmentItem);
					if (practiceItem.isKCViewOpened && practiceItem.KnowledgeComponents.Count>0) {
						foreach (var KC in practiceItem.KnowledgeComponents) {
							optionDataList.Add (KC.Value);
						}
					}
				} else  if(assessmentItem is KnowledgeComponent)
				{
					KnowledgeComponent KC = (KnowledgeComponent)assessmentItem;
					optionDataList.Remove (assessmentItem);
				}
			}
			enhanceScroller.ReloadData (true);
			scrollRect.content.GetComponent<RectTransform> ().position = new Vector2 (scrollRect.content.GetComponent<RectTransform> ().position.x,scrollRect.content.GetComponent<RectTransform> ().position.y+0.01f);
			yield return new WaitForSeconds (0.1f);
			UpdateVerticalScrolllLayoutAfterTime ();
			canUpdateList = true;
		}
	}
}