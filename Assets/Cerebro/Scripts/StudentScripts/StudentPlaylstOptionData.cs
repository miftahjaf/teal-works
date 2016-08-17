//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using MaterialUI;

namespace Cerebro
{
	[Serializable]
	public class StudentPlaylistOptionDataList
	{
		[SerializeField]
		private ImageDataType m_ImageType = ImageDataType.VectorImage;
		public ImageDataType imageType
		{
			get { return m_ImageType; }
			set { m_ImageType = value; }
		}

		[SerializeField]
		private List<StudentPlaylistOptionData> m_Options = new List<StudentPlaylistOptionData>();
		public List<StudentPlaylistOptionData> options
		{
			get { return m_Options; }
			set { m_Options = value; }
		}
	}

	[Serializable]
	public class StudentPlaylistOptionData
	{
		[SerializeField]
		private string m_Name;
		public string name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		private string m_Description;
		public string description
		{
			get { return m_Description; }
			set { m_Description = value; }
		}

		private string m_Id;
		public string id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

		private string m_Url;
		public string Url
		{
			get { return m_Url; }
			set { m_Url = value; }
		}

		private Action m_OnOptionSelected;
		public Action onOptionSelected
		{
			get { return m_OnOptionSelected; }
			set { m_OnOptionSelected = value; }
		}

		public StudentPlaylistOptionData() { }

		public StudentPlaylistOptionData(string name, string description, string Url, string id, Action onOptionSelected = null)
		{
			m_Id = id;
			m_Name = name;
			m_Description = description;
			m_Url = Url;
			m_OnOptionSelected = onOptionSelected;
		}
	}
}