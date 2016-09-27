using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System;
using UnityEngine.EventSystems;


namespace Cerebro
{
	public class AssessmentItemScript  : MonoBehaviour, IPointerClickHandler, ISubmitHandler 
	{
		
		public Text name;
		public Text stats;
		public Text mastryLevel;
		public Text coins;
		public FreeModifier bgModifier;
		public GameObject bg;
		public GameObject KCprefab;
		public GameObject KCParent;
		public AccordionElement accordionElement;


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

		private Action m_OnScrollChanged;
		public Action onScrollChanged
		{
			get { return m_OnScrollChanged; }
			set { m_OnScrollChanged = value; }
		}
			

		public void Initialise(PracticeItems _practiceItem,Action<string,string> _onClickAction,Color _stripColor,Action _onScrollChanged)
		{
			this.practiceItems = _practiceItem;
			this.onClickAction = _onClickAction;
			this.onScrollChanged = _onScrollChanged;
			name.text = StringHelper.RemoveNumbers(_practiceItem.PracticeItemName);
			this.RefreshStats ();
			this.CoinBarAnimation ();
			this.CoinTextAnimation ();
			this.bg.GetComponent<Image> ().color = _stripColor;
			this.CreateKCList ();

			accordionElement.onAnimationStarted += OnAccordionAnimationStarted;
			accordionElement.onAnimationCompleted += OnAccordioAnimationCompleted;

		    
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

		public void OnPointerClick(PointerEventData eventData)
		{
			if (m_OnClickAction != null)
			{
				m_OnClickAction.Invoke(practiceItems.PracticeID,"");
			}
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (m_OnClickAction != null)
			{
				m_OnClickAction.Invoke(practiceItems.PracticeID,"");
			}
		}


		private void CoinBarAnimation()
		{
			if (CerebroProperties.instance.ShowCoins) 
			{
				bg.transform.localScale = new Vector3 (0, 1, 1);

				float ratio = practiceItems.TotalCoins >0f?((practiceItems.TotalCoins- practiceItems.CurrentCoins) / (float)practiceItems.TotalCoins):0;
				Go.to (bg.transform, 0.75f, new GoTweenConfig ().scale (new Vector3 (ratio, 1, 1), false));
			} 
			else
			{
				bg.gameObject.SetActive (false);
				coins.gameObject.SetActive (false);
			}

		}

		public void Refresh()
		{
			this.RefreshStats ();
			this.CoinBarAnimation ();
			this.CoinTextAnimation ();
			this.RefreshKCList ();
		}


		public void CoinTextAnimation()
		{
			int remainingCoins = practiceItems.TotalCoins - practiceItems.CurrentCoins;
			iTween.ValueTo(this.gameObject, iTween.Hash(
				"from", practiceItems.TotalCoins,
				"to", remainingCoins,
				"time",0.4f,
				"onUpdate", (Action<object>) (value =>{this.coins.text = ((float)value).ToString("0") +" coins left";})
			));
		}

		public void CreateKCList()
		{
			int cnt = 1;
			foreach (var KC in practiceItems.KnowledgeComponents) 
			{
				GameObject KCObj = Instantiate (KCprefab);
				KCObj.SetActive (true);
				KCItemScript kcItemScript = KCObj.GetComponent<KCItemScript> ();
				KCObj.GetComponent<RectTransform>().SetParent(KCParent.transform);
				KCObj.GetComponent<RectTransform>().localScale = Vector3.one;
				KCObj.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;

				kcItemScript.Initialise (cnt,practiceItems.PracticeID, KC.Value,m_OnClickAction);

				cnt++;
			}
			this.KCParent.transform.localScale = Vector3.zero;

		}

		public  void OnAccordionAnimationStarted(bool state)
		{
			if (state) {
				
				this.KCParent.transform.localScale = Vector3.one;
				bgModifier.Radius = new Vector4(4f, 4f, 0f, 0f);
				Debug.Log (bgModifier.Radius);
			}
			RefreshKCList ();
			if (m_OnScrollChanged != null) 
			{
				m_OnScrollChanged.Invoke ();
			}
		}

		public  void OnAccordioAnimationCompleted(bool state)
		{
			
			this.KCParent.transform.localScale = state ? Vector3.one:Vector3.zero;
			bgModifier.Radius =state ? new Vector4(4f, 4f, 0f, 0f): new Vector4(4f, 4f, 4f, 4f);
			if (m_OnScrollChanged != null) 
			{
				m_OnScrollChanged.Invoke ();
			}
			RefreshKCList ();
		}

		public void RefreshKCList()
		{
			if (!accordionElement.isOn) 
			{
				return;
			}
			foreach (KCItemScript kcItem in KCParent.GetComponentsInChildren<KCItemScript>()) 
			{
				kcItem.Refresh ();
			}


		}
	}
}
