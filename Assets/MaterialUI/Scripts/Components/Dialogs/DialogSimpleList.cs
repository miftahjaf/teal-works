//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Dialogs/Simple List", 1)]
    public class DialogSimpleList : MaterialDialog
    {
		
        [SerializeField]
        private DialogTitleSection m_TitleSection = new DialogTitleSection();
        public DialogTitleSection titleSection
        {
            get { return m_TitleSection; }
            set { m_TitleSection = value; }
        }

        [SerializeField]
        private VerticalScrollLayoutElement m_ListScrollLayoutElement;
        public VerticalScrollLayoutElement listScrollLayoutElement
        {
            get { return m_ListScrollLayoutElement; }
            set { m_ListScrollLayoutElement = value; }
        }

        private List<DialogSimpleOption> m_SelectionItems;
        public List<DialogSimpleOption> selectionItems
        {
            get { return m_SelectionItems; }
        }

        private OptionDataList m_OptionDataList;
        public OptionDataList optionDataList
        {
            get { return m_OptionDataList; }
            set { m_OptionDataList = value; }
        }

        [SerializeField]
        private GameObject m_OptionTemplate;
		[SerializeField]
		private GameObject m_VideoOptionTemplate;

        private Action<int> m_OnItemClick;

		private List<Color> colors;
		private bool touchActive = true;
		public bool isAssigning = false;
		private List<string> assignedContent;

		void OnEnable()
		{
			assignedContent = new List<string> ();
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

		public void setAssignSwitchValue(bool value) {
			isAssigning = value;
			assignedContent = new List<string> ();
			for (var i = 0; i < m_SelectionItems.Count; i++) {
				if(m_SelectionItems[i].type == "Content" && m_SelectionItems[i].IsSelected == true) {
					m_SelectionItems[i].IsSelected = false;
					Graphic graphic2 = m_SelectionItems [i].gameObject.GetChildByName<Graphic> ("Background Image");
					graphic2.GetComponent<Image> ().color = colors [3];
				}
			}
		}

		public void Initialize(OptionDataList optionDataList, Action<int> onItemClick, string titleText, ImageData icon)
        {
			
			Graphic bg = m_OptionTemplate.transform.parent.parent.gameObject.GetChildByName<Graphic> ("Background Image");
			bg.GetComponent<Image> ().color = colors [0];

			m_TitleSection.SetTitle(titleText, icon);

            m_OptionDataList = optionDataList;
            m_SelectionItems = new List<DialogSimpleOption>();

            Image imageIcon = m_OptionTemplate.GetChildByName<Image>("Icon");
            VectorImage vectorIcon = m_OptionTemplate.GetChildByName<VectorImage>("Icon");

			if (m_OptionDataList.options.Count > 0 && m_OptionDataList.options[0].imageData != null)
			{
				if (m_OptionDataList.options[0].imageData.imageDataType == ImageDataType.Sprite)
				{
					DestroyImmediate(vectorIcon.gameObject);
				}
				else
				{
					DestroyImmediate(imageIcon.gameObject);
				}
			}
			else
			{
				DestroyImmediate(imageIcon.gameObject);
			}

            for (int i = 0; i < m_OptionDataList.options.Count; i++)
            {
				m_SelectionItems.Add(CreateSubListItem(i,m_OptionDataList,"Subject",0));
            }

            float availableHeight = DialogManager.rectTransform.rect.height;

            LayoutGroup textAreaRectTransform = m_TitleSection.text.transform.parent.GetComponent<LayoutGroup>();

            if (textAreaRectTransform.gameObject.activeSelf)
            {
                textAreaRectTransform.CalculateLayoutInputVertical();
                availableHeight -= textAreaRectTransform.preferredHeight;
            }

            m_ListScrollLayoutElement.maxHeight = availableHeight - 48f;

//            Destroy(m_OptionTemplate);
			m_OptionTemplate.gameObject.SetActive(false);
			m_VideoOptionTemplate.gameObject.SetActive(false);

            m_OnItemClick = onItemClick;

            Initialize();
        }

		private DialogSimpleOption CreateSubListItem(int i, OptionDataList tmpoptionDataList, string type, int level)
		{
			GameObject abc = Instantiate (m_OptionTemplate);
			abc.SetActive (true);
			DialogSimpleOption option = abc.GetComponent<DialogSimpleOption>();
			option.rectTransform.SetParent(m_OptionTemplate.transform.parent);
			option.rectTransform.localScale = Vector3.one;
			option.rectTransform.localEulerAngles = Vector3.zero;

			OptionData data = tmpoptionDataList.options[i];

			Text text = option.gameObject.GetChildByName<Text>("Text");
			Graphic graphic = option.gameObject.GetChildByName<Graphic>("Icon");
			Graphic graphic2 = option.gameObject.GetChildByName<Graphic> ("Background Image");
			graphic2.GetComponent<Image> ().color = colors [level];

			text.fontSize = 30 - (2 * level);
			text.text = data.text;

			if (data.imageData == null)
			{
//				text.rectTransform.sizeDelta = new Vector2(-48f, -36f);
//				text.rectTransform.anchoredPosition = new Vector2(0f, 0f);
				Destroy(graphic.gameObject);
			}
			else
			{
				graphic.SetImage(data.imageData);
			}

			option.Id = data.id;
			option.type = type;
			option.index = i;
			option.onClickAction += OnItemClick;
			option.IsExpanded = false;


			return option;
		}

		private DialogSimpleOption CreateVideoSubListItem(int i, OptionDataList tmpoptionDataList, string url)
		{
			GameObject abc = Instantiate (m_VideoOptionTemplate);
			abc.SetActive (true);
			DialogSimpleOption option = abc.GetComponent<DialogSimpleOption>();
			option.rectTransform.SetParent(m_OptionTemplate.transform.parent);
			option.rectTransform.localScale = Vector3.one;
			option.rectTransform.localEulerAngles = Vector3.zero;

			OptionData data = tmpoptionDataList.options[i];

			Text text = option.gameObject.GetChildByName<Text>("Text");
			Graphic graphic = option.gameObject.GetChildByName<Graphic> ("Image");
			Graphic graphic2 = option.gameObject.GetChildByName<Graphic> ("Background Image");
			graphic.GetComponent<Image> ().color = Color.white;
			graphic2.GetComponent<Image> ().color = colors [3];

			text.text = data.text;

			var str = url;
			var firstSplit = str.Split ("v="[1]);
			var videoID = firstSplit [1].Split ("&"[0])[0];

			var videoUrl = "https://www.youtube.com/embed/" + videoID;
			var imgurl = "https://img.youtube.com/vi/" + videoID + "/default.jpg";

			option.typeData = new Dictionary<string,string> ();
			option.typeData.Add ("videoUrl", videoUrl);
			option.typeData.Add ("imgurl", imgurl);

			option.Id = data.id;
			option.type = "Content";
			option.index = i;
			option.onClickAction += OnItemClick;
			option.IsExpanded = false;
			option.IsSelected = false;


			if (assignedContent.IndexOf (data.id) != -1) {
				option.IsSelected = true;
				graphic2.GetComponent<Image> ().color = MaterialColor.blueA400;
			}

			return option;
		}

		IEnumerator LoadImage(string url, Graphic graph) {
			WWW remoteImage = new WWW(url);
			yield return remoteImage;
			if (remoteImage.error == null) {
				var newsprite = Sprite.Create (remoteImage.texture, new Rect (0f, 0f, remoteImage.texture.width, remoteImage.texture.height), new Vector2 (0.5f, 0.5f));
				graph.GetComponent<Image> ().sprite = newsprite;
			}
		}

		IEnumerator LoadImages(List<DialogSimpleOption> arr, List<string> videoUrls) {
			
			for (var i = 0; i < arr.Count; i++) {
				Graphic graphic = arr[i].gameObject.GetChildByName<Graphic> ("Image");

//				var str = "https://www.youtube.com/watch?v=JC82Il2cjqA&x=6";
				var str = videoUrls[i];
				var firstSplit = str.Split ("v="[1]);
				var videoID = firstSplit [1].Split ("&"[0])[0];
				var imgurl = arr [i].typeData ["imgurl"];
				WWW remoteImage = new WWW(imgurl);
				yield return remoteImage;
				if (remoteImage.error == null) {
					var newsprite = Sprite.Create (remoteImage.texture, new Rect (0f, 0f, remoteImage.texture.width, remoteImage.texture.height), new Vector2 (0.5f, 0.5f));
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

        }

		public float ContractList(int index) {
			m_SelectionItems [index].IsExpanded = false;

			int numberOfNodes = 0;
			var totalHeightChange = 0f;
		
			var numElements = m_SelectionItems [index].Children.Count;
			numberOfNodes += numElements;
				
			Destroy(m_SelectionItems [index].SubList);

			for (var i = 0; i < m_SelectionItems [index].Children.Count; i++) {
				totalHeightChange += m_SelectionItems [index].Children[i].gameObject.GetComponent<LayoutElement> ().preferredHeight;
				if (m_SelectionItems [index].Children [i].IsExpanded) {
					ContractList (index + 1);
				}
				m_SelectionItems.Remove (m_SelectionItems [index].Children [i]);
			}
			for (int i = index + 1; i < m_SelectionItems.Count; i++) {
				m_SelectionItems [i].index -= m_SelectionItems [index].Children.Count;
			}

			return totalHeightChange;
		}

		public List<string> GetAssignedContentList() {
			return assignedContent;
		}
    }
}