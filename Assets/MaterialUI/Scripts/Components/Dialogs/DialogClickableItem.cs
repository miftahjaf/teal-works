//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Clickable Option", 100)]
    public class DialogClickableOption : MonoBehaviour, IPointerClickHandler, ISubmitHandler
    {

		private Action<int> m_OnClickAction;
        public Action<int> onClickAction
        {
            get { return m_OnClickAction; }
            set { m_OnClickAction = value; }
        }

		private string m_type;
		public string type
		{
			get { return m_type; }
			set { m_type = value; }
		}

		private string m_Id;
		public string Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		private Dictionary<string,string> m_typeData;
		public Dictionary<string,string> typeData
		{
			get { return m_typeData; }
			set { m_typeData = value; }
		}

		private int m_Index;
        public int index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

		private bool m_IsExpanded;
		public bool IsExpanded
		{
			get { return m_IsExpanded; }
			set { m_IsExpanded = value; }
		}

		private bool m_IsSelected;
		public bool IsSelected
		{
			get { return m_IsSelected; }
			set { m_IsSelected = value; }
		}

		private GameObject m_SubList;
		public GameObject SubList
		{
			get { return m_SubList; }
			set { m_SubList = value; }
		}

		private List<DialogSimpleOption> m_Children;
		public List<DialogSimpleOption> Children
		{
			get { return m_Children; }
			set { m_Children = value; }
		}

        public void OnPointerClick(PointerEventData eventData)
        {
            if (m_OnClickAction != null)
            {
                m_OnClickAction.Invoke(m_Index);
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (m_OnClickAction != null)
            {
                m_OnClickAction.Invoke(m_Index);
            }
        }
    }
}