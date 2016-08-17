﻿//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MaterialUI;

namespace Cerebro
{
	public class ClickableOption : MonoBehaviour, IPointerClickHandler, ISubmitHandler
	{

		private Action<int> m_OnClickAction;
		public Action<int> onClickAction
		{
			get { return m_OnClickAction; }
			set { m_OnClickAction = value; }
		}

		private string m_Id;
		public string Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		private string m_PhotoUrl;
		public string PhotoUrl
		{
			get { return m_PhotoUrl; }
			set { m_PhotoUrl = value; }
		}

		private string m_Url;
		public string Url
		{
			get { return m_Url; }
			set { m_Url = value; }
		}

		private int m_Index;
		public int index
		{
			get { return m_Index; }
			set { m_Index = value; }
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