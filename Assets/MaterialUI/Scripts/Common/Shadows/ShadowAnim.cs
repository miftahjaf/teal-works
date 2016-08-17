//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Shadow Anim", 100)]
    public class ShadowAnim : MonoBehaviour
    {
        [SerializeField]
        private bool m_IsOn;
		public bool isOn
		{
			get { return m_IsOn; }
			set { m_IsOn = value; }
		}

        [SerializeField]
        private bool m_Animate;
		public bool animate
		{
			get { return m_Animate; }
			set { m_Animate = value; }
		}

        [HideInInspector]
        public Image[] m_Shadows;
		public Image[] shadows
		{
			get { return m_Shadows; }
			set { m_Shadows = value; }
		}

        private CanvasGroup m_CanvasGroup;

        void Awake()
        {
            m_CanvasGroup = gameObject.GetComponent<CanvasGroup>();
            m_Shadows = gameObject.GetComponentsInChildren<Image>();
        }

        void Update()
        {
            if (animate)
            {
                if (isOn)
                {
                    if (m_CanvasGroup.alpha < 1f)
                    {
                        m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 1.1f, Time.deltaTime * 6);
                    }
                    else
                    {
                        m_CanvasGroup.alpha = 1f;
                        animate = false;
                    }
                }
                else
                {
                    if (m_CanvasGroup.alpha > 0f)
                    {
                        m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, -0.1f, Time.deltaTime * 6);
                    }
                    else
                    {
                        m_CanvasGroup.alpha = 0f;
                        animate = false;
                        for (int i = 0; i < m_Shadows.Length; i++)
                        {
                            m_Shadows[i].enabled = false;
                        }
                    }
                }
            }
        }

        public void SetShadow(bool set)
        {
            isOn = set;
            animate = true;
            for (int i = 0; i < m_Shadows.Length; i++)
            {
                m_Shadows[i].enabled = true;
            }
        }
    }
}
