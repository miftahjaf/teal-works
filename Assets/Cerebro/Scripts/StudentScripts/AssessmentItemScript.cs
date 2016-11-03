using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System;
using UnityEngine.EventSystems;
using EnhancedUI.EnhancedScroller;

namespace Cerebro
{
	public class AssessmentItemScript  : EnhancedScrollerCellView
	{
		public Text name;
		public Text stats;
		public Text mastryLevel;
		public Text coins;
		public FreeModifier bgModifier;
		public GameObject bg;
		//public ProgressHelper progressHelper;
		public Sprite plus;
		public Sprite minus;
		public Image accordionIcon;

		private PracticeItems m_PracticeItems;
		public PracticeItems practiceItems
		{
			get { return m_PracticeItems; }
			set { m_PracticeItems = value; }
		}
			
		private Action<string,string> m_OnClickAction;
		public Action<string,string> onClickAction
		{
			get { return m_OnClickAction; }
			set { m_OnClickAction = value; }
		}

		private Action<string> m_OnKCViewOpenStateChanged;
		public Action<string> OnKCViewOpenStateChanged
		{
			get { return m_OnKCViewOpenStateChanged; }
			set { m_OnKCViewOpenStateChanged = value; }
		}
			
		public void Initialise(PracticeItems _practiceItem,Action<string,string> _onClickAction,Action<string> _OnKCViewOpenStateChanged)
		{
			this.practiceItems = _practiceItem;
			this.onClickAction = _onClickAction;
			this.name.text = StringHelper.RemoveNumbers(_practiceItem.PracticeItemName);
			this.bg.GetComponent<Image> ().color = practiceItems.stripColor;
			this.OnKCViewOpenStateChanged = _OnKCViewOpenStateChanged;
			this.Refresh ();
		}

		public void UpdateAccordionIcon()
		{
			this.accordionIcon.sprite =	practiceItems.isKCViewOpened ? minus : plus;
			this.bgModifier.Radius = practiceItems.isKCViewOpened ? new Vector4 (4f, 4f, 0, 0) : Vector4.one * 4f;
		}
			
		private void RefreshStats()
		{
			if (PracticeData.mPracticeData.ContainsKey (practiceItems.PracticeID))
			{
				name.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (name.GetComponent<RectTransform> ().anchoredPosition.x, 12f);
				stats.gameObject.SetActive (true);
				stats.text = PracticeData.mPracticeData [practiceItems.PracticeID].totalCorrect + " Correct | " + PracticeData.mPracticeData [practiceItems.PracticeID].totalAttempts + " Attempts";
			} else {
				stats.gameObject.SetActive (false);
				name.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (name.GetComponent<RectTransform> ().anchoredPosition.x, 0f);
			}
		}

		public override void RefreshCellView()
		{
			Refresh ();
		}

		public void OnPointerClick()
		{
			if (m_OnClickAction != null)
			{
				m_OnClickAction.Invoke(practiceItems.PracticeID,"");
			}
		}
			
		private void CoinBarAnimation()
		{
			float ratio = practiceItems.TotalCoins >0f?((practiceItems.TotalCoins- practiceItems.CurrentCoins) / (float)practiceItems.TotalCoins):0;
			bg.transform.localScale = new Vector3 (ratio, 1, 1);
		}

		public void Refresh()
		{
			this.RefreshStats ();
			this.CoinBarAnimation ();
			this.CoinTextAnimation ();
			this.RefreshMasteryLoaded ();
			this.UpdateAccordionIcon ();
		}


		public void CoinTextAnimation()
		{
			int remainingCoins = practiceItems.TotalCoins - practiceItems.CurrentCoins;
			if (remainingCoins < 0) 
			{
				remainingCoins = 0;
			}
			this.coins.text = remainingCoins.ToString ("0") + " coins left";
		}
			
		public void AccordionPressed()
		{
			if (m_OnKCViewOpenStateChanged != null) 
			{
				m_OnKCViewOpenStateChanged.Invoke (practiceItems.PracticeID);
			}
		}
			
		public void RefreshMasteryLoaded()
		{
			int started = 0;
			int inprogress = 0;
			int mastered = 0;
			foreach (var KC in practiceItems.KnowledgeComponents)
			{
				if (LaunchList.instance.mKCMastery.ContainsKey (KC.Value.ID)) 
				{
					int level = GetLevel (LaunchList.instance.mKCMastery [KC.Value.ID]);
					if (level == 1)
						started++;
					else if (level == 2)
						inprogress++;
					else if (level == 3)
						mastered++;
				} else {
					started++;
				}

			}
			mastryLevel.text = GetMasteryText (started, inprogress, mastered);
		}

		public int GetLevel(int mastery)
		{
			if (mastery == 0)
				return 1;
			if (mastery >= 99f)
				return 3;

			return 2;
		}

		public string GetMasteryText(int started,int inprogress,int mastered)
		{
			return "<color=#9A9AA4>● </color><color=black>" + started.ToString ().PadRight (5, ' ') +"</color><color=#FDD000>● </color><color=black>"+inprogress.ToString ().PadRight (5, ' ') +"</color><color=#24C8A6>● </color><color=black>"+mastered.ToString ().PadRight (5, ' ') +"</color>";
		}
	}
}
