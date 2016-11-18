using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI.ProceduralImage;
using EnhancedUI.EnhancedScroller;

namespace Cerebro
{
	public class KCItemScript  :EnhancedScrollerCellView
	{
		public Text name;
		public Text coins;
		public Slider mastrySlider;
		public Text mastryText;
		public Text indexText;
		public Image mastryImage;
	

		private Action<string,string> m_OnClickAction;
		public Action<string,string> onClickAction
		{
			get { return m_OnClickAction; }
			set { m_OnClickAction = value; }
		}

		private KnowledgeComponent m_KnowledgeComponent;
		public KnowledgeComponent knowledgeComponent
		{
			get { return m_KnowledgeComponent; }
			set { m_KnowledgeComponent = value; }
		}
			
		public void Initialise(KnowledgeComponent _knowledgeComponent,Action<string,string> _OnClickAction)
		{
			knowledgeComponent = _knowledgeComponent;
			indexText.text = knowledgeComponent.Index+".";
			name.text = _knowledgeComponent.KCName;
			this.UpdateCoinText ();
			onClickAction = _OnClickAction;
			if (LaunchList.instance.mKCMastery.ContainsKey (knowledgeComponent.ID))
			{
				knowledgeComponent.Mastery = LaunchList.instance.mKCMastery [knowledgeComponent.ID];
			}
			UpdateMastry ();

			//ChangeMastryStatus (false);
		}

		public void OnPointerClick()
		{
			if (m_OnClickAction != null)
			{
				Debug.Log ("Practice Id " + knowledgeComponent.PracticeID + "Knowledge Component Id " + knowledgeComponent.ID);
				m_OnClickAction.Invoke(knowledgeComponent.PracticeID,knowledgeComponent.ID);
			}
		}

		public void UpdateCoinText()
		{
			int diffCoins = knowledgeComponent.TotalCoins - knowledgeComponent.CurrentCoins;

			if (diffCoins < 0)
				diffCoins = 0;
			
			this.coins.text = diffCoins.ToString() +" coins left";
		}

		public override void RefreshCellView()
		{
			UpdateMastry ();
			UpdateCoinText ();
		}

		public void UpdateMastry()
		{
			int level = GetLevel ();
			if (level == 1) 
			{
				mastryImage.color = CerebroHelper.HexToRGB ("9A9AA4");
				//mastryImage.GetComponent<FreeModifier> ().Radius = new Vector4 (5f, 0f, 0f, 5f);
			} 
			else if (level == 2) 
			{
				mastryImage.color = CerebroHelper.HexToRGB ("FDD000");
				//mastryImage.GetComponent<FreeModifier> ().Radius = new Vector4 (5f, 0f, 0f, 5f);
			}
			else
			{
				mastryImage.color = CerebroHelper.HexToRGB ("24C8A6");
				//mastryImage.GetComponent<FreeModifier> ().Radius = new Vector4 (5f,5f, 5f, 5f);
			}

			int mastery = knowledgeComponent.Mastery;
			if (mastery >= 99) {
				mastery = 100;
			}

			mastryText.text = mastery + "%";
			mastrySlider.value =Mathf.Clamp( mastery / 100f,0f,1f);

			//ChangeMastryStatus (true);

		}

		public int GetLevel()
		{
			if (knowledgeComponent.Mastery == 0)
				return 1;
			if (knowledgeComponent.Mastery >= 95f)
				return 3;

			return 2;
		}





	}
}