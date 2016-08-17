//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    public class ButtonRectInstantiationHelper : InstantiationHelper
    {
        [SerializeField]
        private MaterialButton m_Button;

        [SerializeField]
        private MaterialDropdown m_Dropdown;

        [SerializeField]
        private RectTransform m_RectTransform;

        [SerializeField]
        private HorizontalLayoutGroup m_Content;

        [SerializeField]
        private RectTransform m_Text;

        [SerializeField]
        private Graphic m_Icon;

        [SerializeField]
        private VectorImageData m_IconData;

        public override void HelpInstantiate(params InstantiationOptions[] options)
        {
            if (!options.Contains(InstantiationOptions.Raised))
            {
                m_Button.Convert(true);
            }

            if (!options.Contains(InstantiationOptions.HasDropdown))
            {
                DestroyImmediate(m_Dropdown);
                m_Button.buttonObject.onClick = null;
                m_Icon.rectTransform.SetAsFirstSibling();
                m_Icon.SetImage(m_IconData);
                RectOffset offset = m_Content.padding;
                offset.right = 0;
                m_Content.padding = offset;
                m_Button.text = "BUTTON";
            }
            else
            {
                m_Button.icon = null;
            }

            if (!options.Contains(InstantiationOptions.HasContent))
            {
                m_Button.contentRectTransform = m_Text;
                m_Text.SetParent(m_RectTransform);
                m_Text.anchorMin = Vector2.zero;
                m_Text.anchorMax = Vector2.one;
                DestroyImmediate(m_Content.gameObject);
                m_Button.icon = null;
                m_Button.SetLayoutDirty();
            }
            else
            {
                m_Button.iconData.vectorImageData = m_IconData;
            }

            base.HelpInstantiate(options);
        }
    }
}