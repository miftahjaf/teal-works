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
	//	[AddComponentMenu("MaterialUI/Dialogs/Simple List", 1)]
	public class StudentList : MaterialDialog
	{

		[SerializeField]
		private VerticalScrollLayoutElement m_ListScrollLayoutElement;
		public VerticalScrollLayoutElement listScrollLayoutElement
		{
			get { return m_ListScrollLayoutElement; }
			set { m_ListScrollLayoutElement = value; }
		}

		private List<StudentListOption> m_SelectionItems;
		public List<StudentListOption> selectionItems
		{
			get { return m_SelectionItems; }
		}

		private StudentOptionDataList m_OptionDataList;
		public StudentOptionDataList optionDataList
		{
			get { return m_OptionDataList; }
			set { m_OptionDataList = value; }
		}

		[SerializeField]
		private GameObject m_OptionTemplate;
	
		private List<Color> colors;
		private bool touchActive = true;
		public bool isAssigning = false;
		private List<string> assignedStudents;

		void OnEnable()
		{
			assignedStudents = new List<string> ();
			colors = new List<Color> ();
			/*colors.Add (MaterialColor.teal700);
			colors.Add (MaterialColor.teal500);
			colors.Add (MaterialColor.teal300);
			colors.Add (MaterialColor.teal100);*/
			colors.Add (MaterialColor.grey400);
			colors.Add (MaterialColor.grey300);
			colors.Add (MaterialColor.grey200);
			colors.Add (MaterialColor.grey100);
			GetComponentInChildren<OverscrollConfig>().Setup();
		}

		public void Initialize(StudentOptionDataList optionDataList, Action<int> onItemClick, string titleText, ImageData icon)
		{

			Graphic bg = m_OptionTemplate.transform.parent.parent.gameObject.GetChildByName<Graphic> ("Background Image");
			bg.GetComponent<Image> ().color = colors [0];

			m_OptionDataList = optionDataList;
			m_SelectionItems = new List<StudentListOption>();

			for (int i = 0; i < m_OptionDataList.options.Count; i++)
			{
				m_SelectionItems.Add(CreateSubListItem(i,m_OptionDataList));
			}
//			StartCoroutine (LoadImages (m_SelectionItems));

			float availableHeight = DialogManager.rectTransform.rect.height;

			m_ListScrollLayoutElement.maxHeight = availableHeight - 48f;

			//			            Destroy(m_OptionTemplate);
			m_OptionTemplate.gameObject.SetActive(false);

			Initialize();

		}

		private StudentListOption CreateSubListItem(int i, StudentOptionDataList tmpoptionDataList)
		{
			GameObject abc = Instantiate (m_OptionTemplate);
			abc.SetActive (true);
			StudentListOption option = abc.GetComponent<StudentListOption>();
			option.rectTransform.SetParent(m_OptionTemplate.transform.parent);
			option.rectTransform.localScale = Vector3.one;
			option.rectTransform.localEulerAngles = Vector3.zero;

			StudentOptionData data = tmpoptionDataList.options[i];

			Text text = option.gameObject.GetChildByName<Text>("Text");
			Graphic graphic = option.gameObject.GetChildByName<Graphic> ("Image");
			Graphic graphic2 = option.gameObject.GetChildByName<Graphic> ("Background Image");
			graphic.GetComponent<Image> ().color = new Color (0, 0, 0, 0);
			graphic2.GetComponent<Image> ().color = colors [3];

			text.text = data.text;
					
			option.Id = data.id;
			option.index = i;
			option.onClickAction += OnItemClick;
			option.IsSelected = false;
			option.PhotoUrl = data.profileUrl;

			return option;
		}

		IEnumerator LoadImages(List<StudentListOption> arr) {
			for (var i = 0; i < arr.Count; i++) {
				Graphic graphic = arr[i].gameObject.GetChildByName<Graphic> ("Image");
				var imgurl = arr [i].PhotoUrl;
				WWW remoteImage = new WWW(imgurl);
				yield return remoteImage;
				if (remoteImage.error == null) {
					var newsprite = Sprite.Create (remoteImage.texture, new Rect (0f, 0f, remoteImage.texture.width, remoteImage.texture.height), new Vector2 (0.5f, 0.5f));
					graphic.GetComponent<Image> ().color = new Color (1, 1, 1, 1);
					graphic.GetComponent<Image> ().sprite = newsprite;
				}
			}
		}

		IEnumerator BlockClick(float time) {
			touchActive = false;
			yield return new WaitForSeconds (time);
			touchActive = true;
		}
		public void OnItemClick(int index)
		{	
			if (!touchActive) {
				return;
			}
			m_SelectionItems [index].IsSelected = !m_SelectionItems [index].IsSelected;
			Graphic graphic2 = m_SelectionItems [index].gameObject.GetChildByName<Graphic> ("Background Image");
			if (m_SelectionItems [index].IsSelected) {
				assignedStudents.Add (m_SelectionItems [index].Id);
				graphic2.GetComponent<Image> ().color = MaterialColor.blueA400;
			} else {
				assignedStudents.Remove (m_SelectionItems [index].Id);
				graphic2.GetComponent<Image> ().color = colors [3];
			}
		}

		public void LoadImages() {
		//	StartCoroutine (LoadImages (m_SelectionItems));
		}
		public List<string> GetAssignedStudentList() {
			return assignedStudents;
		}
	}
}