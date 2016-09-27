//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MaterialUI;

namespace Cerebro
{
	public class AssessmentEventArgs : EventArgs
	{
		public string practiceId;
		public string KCID;
	}


	public class AssessmentSelector : MonoBehaviour
	{

		[SerializeField]
		private VerticalScrollLayoutElement m_ListScrollLayoutElement;
		public VerticalScrollLayoutElement listScrollLayoutElement
		{
			get { return m_ListScrollLayoutElement; }
			set { m_ListScrollLayoutElement = value; }
		}

		private List<GameObject> m_SelectionItems;
		public List<GameObject> selectionItems
		{
			get { return m_SelectionItems; }
		}

		private List<PracticeItems> m_OptionDataList;
		public List<PracticeItems> optionDataList
		{
			get { return m_OptionDataList; }
			set { m_OptionDataList = value; }
		}

		public Scrollbar scrollElement;

		[SerializeField]
		private GameObject m_OptionTemplate;

		[SerializeField]
		private GameObject m_OptionChildTemplate;

		public event EventHandler AssessmentSelected;

		private List<int> coinBarValues;
		private List<int> decrementValues;

		private float listHeight = 700f;
		private string[] stripColors = new string[]{"ff5541","2c96dc","29cdb1","7d33ff","ff9633"};
	
		public void RefreshCellData () {
			coinBarValues = new List<int> ();
			decrementValues = new List<int> ();

			for (var i = 0; i < m_OptionDataList.Count; i++)
			{
				AssessmentItemScript assessmentItem = m_SelectionItems[i].GetComponentInChildren<AssessmentItemScript>();
				assessmentItem.Refresh ();
//				ClickableOption option = m_SelectionItems[i].GetComponent<ClickableOption>();
//				Text stats = option.gameObject.GetChildByName<Text>("Stats");
//
//				if (PracticeData.mPracticeData.ContainsKey (m_OptionDataList[i].PracticeID)) {
//					stats.gameObject.SetActive (true);
//					stats.text = PracticeData.mPracticeData [m_OptionDataList[i].PracticeID].totalCorrect + " Correct | " + PracticeData.mPracticeData [m_OptionDataList[i].PracticeID].totalAttempts + " Attempts";
//				} else {
//					stats.gameObject.SetActive (false);
//				}
//
//				if (CerebroProperties.instance.ShowCoins) {
//					animateCoinsBar (option, i);
//				} else {
//					option.gameObject.transform.Find ("bg").gameObject.SetActive (false);
//					option.gameObject.transform.Find ("Coins").gameObject.SetActive (false);
//				}
			}
		}

		void animateCoinsBar(ClickableOption option, int index) {
			GameObject bg = option.gameObject.transform.Find ("Header/bg").gameObject;
			GameObject coinsText = option.gameObject.transform.Find ("Header/Coins").gameObject;
			string optionText = m_OptionDataList [index].PracticeItemName;

			if (LaunchList.instance.mPracticeItems.ContainsKey (m_OptionDataList [index].PracticeID)) {
				bg.SetActive (true);
				coinsText.SetActive (true);
				int totalCoins = LaunchList.instance.mPracticeItems [m_OptionDataList [index].PracticeID].TotalCoins;
				int currentCoins = LaunchList.instance.mPracticeItems [m_OptionDataList [index].PracticeID].CurrentCoins;
				int coinsLeft = totalCoins - currentCoins;
				coinsText.GetComponent<Text> ().text = totalCoins.ToString () + " Coins Left";

				float ratio =totalCoins >0f?( (float)coinsLeft / (float)totalCoins):0;

				coinBarValues.Add (coinsLeft);
				int decrementBy = (totalCoins - coinsLeft) / 30;
				if (decrementBy == 0) {
					decrementBy = 1;
				}
				decrementValues.Add (decrementBy);

				var curPosition = coinsText.transform.localPosition;
				bg.transform.localScale = new Vector3 (0, 1, 1);

//				coinsText.GetComponent<RectTransform>().anchoredPosition = new Vector3 (-496f, curPosition.y, curPosition.z);
//
//				float positionX = -496f + (ratio * 992f);
//				if (positionX < -400f) {
//					positionX = -400f;
//				}

				Go.to (bg.transform, 0.75f, new GoTweenConfig ().scale (new Vector3 (ratio, 1, 1), false));
//				Go.to (coinsText.transform, 1f, new GoTweenConfig ().localPosition (new Vector3 ((ratio-1) * 886, 0,0), true));
//				Go.to (coinsText.transform, 1f, new GoTweenConfig ().localPosition (new Vector3 (positionX, coinsText.transform.localPosition.y,coinsText.transform.localPosition.z), false));
			} else {
				bg.SetActive (false);
				coinsText.SetActive (false);
				option.gameObject.SetActive (false);				//uncomment this to remove option if not on server! 
			}
		}

		public void Initialize(List<PracticeItems> assessments)
		{
			coinBarValues = new List<int> ();
			decrementValues = new List<int> ();

			m_OptionDataList = assessments;
			m_SelectionItems = new List<GameObject>();

			for (int i = 0; i < m_OptionDataList.Count; i++)
			{
				string title = StringHelper.RemoveNumbers (m_OptionDataList [i].PracticeItemName);

				m_SelectionItems.Add(CreateSubListItem(i,m_OptionDataList [i]));
			}
			//			StartCoroutine (LoadImages (m_SelectionItems));

			float availableHeight = DialogManager.rectTransform.rect.height;

			UpdateVerticalScrolllLayout ();

			//			            Destroy(m_OptionTemplate);
			m_OptionTemplate.gameObject.SetActive(false);

			m_OptionChildTemplate.gameObject.SetActive(false);

			GetComponent<RectTransform>().sizeDelta = new Vector2(1024f,GetComponent<RectTransform>().sizeDelta.y);
			GetComponent<RectTransform> ().localPosition = new Vector3 (GetComponent<RectTransform> ().localPosition.x, 316f);
			//			Initialize();

		}

		public float getListHeight() {
			return listHeight;
		}

		public float getContentHeight() {
			return 150;
		}

		private GameObject CreateSubListItem(int index, PracticeItems optionData)
		{
			GameObject abc = Instantiate (m_OptionTemplate);
			abc.SetActive (true);
			AssessmentItemScript assessmentItemScript = abc.GetComponentInChildren<AssessmentItemScript> ();

			//ClickableOption option = abc.GetComponent<ClickableOption>();
			abc.GetComponent<RectTransform>().SetParent(m_OptionTemplate.transform.parent);
			abc.GetComponent<RectTransform>().localScale = Vector3.one;
			abc.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;

			Color stripColor = CerebroHelper.HexToRGB (stripColors [index % stripColors.Length]);

			assessmentItemScript.Initialise (optionData,OnItemClick, stripColor,UpdateVerticalScrolllLayout);

			/*Text name = option.gameObject.GetChildByName<Text>("Name");
			Text stats = option.gameObject.GetChildByName<Text>("Stats");
			Text masteryLevel = option.gameObject.GetChildByName<Text>("Level");

			name.text = StringHelper.RemoveNumbers(optionData.PracticeItemName);

			if (PracticeData.mPracticeData.ContainsKey (optionData.PracticeID)) {
				stats.gameObject.SetActive (true);
				stats.text = PracticeData.mPracticeData [optionData.PracticeID].totalCorrect + " Correct | " + PracticeData.mPracticeData [optionData.PracticeID].totalAttempts + " Attempts";
			} else {
				stats.gameObject.SetActive (false);
			}




			
//			option.gameObject.transform.Find ("Strip").GetComponent<Image> ().color = stripColor;
			option.gameObject.transform.Find ("Header/bg").GetComponent<Image> ().color = stripColor;
//			option.gameObject.transform.Find ("Strip").gameObject.SetActive (false);

			if (CerebroProperties.instance.ShowCoins) {
				animateCoinsBar (option, i);
			} else {
				option.gameObject.transform.Find ("Header/bg").gameObject.SetActive (false);
				option.gameObject.transform.Find ("Header/Coins").gameObject.SetActive (false);
			}

			option.onClickAction += OnItemClick;
			option.index = i;

			foreach (string key in optionData.KnowledgeComponents.Keys) 
			{
				KnowledgeComponent KC = optionData.KnowledgeComponents [key];
				GameObject KCObj = Instantiate (m_OptionChildTemplate);
				KCObj.SetActive (true);
				KCObj.GetComponent<RectTransform>().SetParent(abc.transform.Find("KCS"));
				KCObj.GetComponent<RectTransform>().localScale = Vector3.one;
				KCObj.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
				Text KCname = KCObj.gameObject.GetChildByName<Text>("Name");
				KCname.text = KC.KCName;

			}
*/
			return abc;
		}

		public void OnItemClick(string practiceId,string KCID)
		{	

			// TO LOCK Assessment ON NO COINS
//			string optionText = m_OptionDataList [index];
//			if (LaunchList.instance.mPracticeItems.ContainsKey (m_OptionDataList [index].PracticeID)) {
//				int totalCoins = int.Parse (LaunchList.instance.mPracticeItems [m_OptionDataList [index].PracticeID].TotalCoins);
//				int currentCoins = LaunchList.instance.mPracticeItems [m_OptionDataList [index].PracticeID].CurrentCoins;
//				int coinsLeft = totalCoins - currentCoins;
//				if (coinsLeft <= 0) {
//					CerebroHelper.DebugLog ("NO COINS LEFT");
//					return;
//				}
//			}

			if (AssessmentSelected != null) {
				AssessmentEventArgs args = new AssessmentEventArgs ();
				args.practiceId = practiceId;
				args.KCID = KCID;
				AssessmentSelected.Invoke (this, args);
			}
		}

		public void UpdateVerticalScrolllLayout()
		{
			Invoke ("UpdateVerticalScrolllLayoutAfterTime", 0.1f);
		}

		public void UpdateVerticalScrolllLayoutAfterTime()
		{
			this.m_ListScrollLayoutElement.maxHeight = listHeight;
		}

//		void Update() {
//			if (coinBarValues != null) {
//				for (var i = 0; i < coinBarValues.Count; i++) {
//					ClickableOption option = m_SelectionItems [i].GetComponent<ClickableOption> ();
//					GameObject coinsText = option.gameObject.transform.Find ("Header/Coins").gameObject;
//					var currentValue = int.Parse (coinsText.GetComponent<Text> ().text.Split (new string[] { " Coins Left" }, System.StringSplitOptions.None) [0]);
//					if (currentValue > coinBarValues [i]) {
//						currentValue -= decrementValues [i];
//					} else {
//						currentValue = coinBarValues [i];
//					}
//					if (currentValue < 0)
//						currentValue = 0;
//					coinsText.GetComponent<Text> ().text = currentValue + " Coins Left";
//				}	
//			}
//		}
	}
}