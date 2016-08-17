//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using MaterialUI;

namespace Cerebro
{
	[Serializable]
	public class StudentOptionDataList
	{
		[SerializeField]
		private ImageDataType m_ImageType = ImageDataType.VectorImage;
		public ImageDataType imageType
		{
			get { return m_ImageType; }
			set { m_ImageType = value; }
		}

		[SerializeField]
		private List<StudentOptionData> m_Options = new List<StudentOptionData>();
		public List<StudentOptionData> options
		{
			get { return m_Options; }
			set { m_Options = value; }
		}
	}

	[Serializable]
	public class StudentOptionData
	{
		[SerializeField]
		private string m_Text;
		public string text
		{
			get { return m_Text; }
			set { m_Text = value; }
		}

		private string m_Id;
		public string id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		private string m_ProfileUrl;
		public string profileUrl
		{
			get { return m_ProfileUrl; }
			set { m_ProfileUrl = value; }
		}

		private Action m_OnOptionSelected;
		public Action onOptionSelected
		{
			get { return m_OnOptionSelected; }
			set { m_OnOptionSelected = value; }
		}

		public StudentOptionData() { }

		public StudentOptionData(string text, string profileUrl, string id, Action onOptionSelected = null)
		{
			m_Id = id;
			m_Text = text;
			m_ProfileUrl = profileUrl;
			m_OnOptionSelected = onOptionSelected;
		}
	}
}