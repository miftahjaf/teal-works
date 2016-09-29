using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Cerebro
{
	public class KCItemScript  : MonoBehaviour, IPointerClickHandler, ISubmitHandler 
	{
		public Text name;
		public Text coins;
		public Slider mastrySlider;
		public Text mastryText;
		public Image mastryImage;
		private string m_PracticeId;
		public string praticeId
		{
			get { return m_PracticeId; }
			set { m_PracticeId = value; }
		}

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
			
		public void Initialise(int index,string _practiceId, KnowledgeComponent _knowledgeComponent,Action<string,string> _OnClickAction)
		{
			knowledgeComponent = _knowledgeComponent;
			praticeId = _practiceId;
			name.text = index+". "+ _knowledgeComponent.KCName;
			this.UpdateCoinText ();
			onClickAction = _OnClickAction;
			ChangeMastryStatus (false);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (m_OnClickAction != null)
			{
				m_OnClickAction.Invoke(praticeId,knowledgeComponent.ID);
			}
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (m_OnClickAction != null)
			{
				m_OnClickAction.Invoke(praticeId,knowledgeComponent.ID);
			}
		}

		public void UpdateCoinText()
		{
			this.coins.text = (knowledgeComponent.TotalCoins - knowledgeComponent.CurrentCoins).ToString() +" coins left";
		}



		public void UpdateMastry()
		{
			int level = GetLevel ();
			if (level == 1) 
			{
				mastryImage.color = CerebroHelper.HexToRGB ("9A9AA4");
				mastryImage.GetComponent<FreeModifier> ().Radius = new Vector4 (5f, 0f, 0f, 5f);
			} 
			else if (level == 2) 
			{
				mastryImage.color = CerebroHelper.HexToRGB ("FDD000");
				mastryImage.GetComponent<FreeModifier> ().Radius = new Vector4 (5f, 0f, 0f, 5f);
			}
			else
			{
				mastryImage.color = CerebroHelper.HexToRGB ("24C8A6");
				mastryImage.GetComponent<FreeModifier> ().Radius = new Vector4 (5f,5f, 5f, 5f);
			}

			mastryText.text = knowledgeComponent.Mastery + "%";
			mastrySlider.value =Mathf.Clamp( knowledgeComponent.Mastery / 100f,0f,1f);

			ChangeMastryStatus (true);

		}

		public int GetLevel()
		{
			if (knowledgeComponent.Mastery == 0)
				return 1;
			if (knowledgeComponent.Mastery == 100)
				return 3;

			return 2;
		}

		public void ChangeMastryStatus(bool enable)
		{
			mastrySlider.gameObject.SetActive (enable);
			mastryText.gameObject.SetActive (enable);
		}

	}
}