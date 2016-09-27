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



		public void Initialise(string _practiceId, KnowledgeComponent _knowledgeComponent,Action<string,string> _OnClickAction)
		{
			knowledgeComponent = _knowledgeComponent;
			praticeId = _practiceId;
	    	name.text = _knowledgeComponent.KCName;
			this.CoinTextAnimation ();
			onClickAction = _OnClickAction;
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

		public void CoinTextAnimation()
		{
			this.coins.text = (knowledgeComponent.TotalCoins - knowledgeComponent.CurrentCoins).ToString() +" coins left";
		}

		public void Refresh()
		{
			CoinTextAnimation ();
		}

	}
}