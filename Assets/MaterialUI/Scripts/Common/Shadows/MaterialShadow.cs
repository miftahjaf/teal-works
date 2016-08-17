//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace MaterialUI
{
    [AddComponentMenu("MaterialUI/Material Shadow", 50)]
    [ExecuteInEditMode]
    public class MaterialShadow : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public enum ShadowsActive
        {
            Hovered,
            Clicked
        }

        [SerializeField]
        private ShadowAnim[] m_ShadowAnims;
		public ShadowAnim[] shadowAnims
		{
			get { return m_ShadowAnims; }
			set { m_ShadowAnims = value; }
		}

        [SerializeField]
        [Range(0, 3)]
        private int m_ShadowNormalSize = 1;
		public int shadowNormalSize
		{
			get { return m_ShadowNormalSize; }
			set { m_ShadowNormalSize = value; }
		}

        [SerializeField]
        [Range(0, 3)]
        private int m_ShadowActiveSize = 2;
		public int shadowActiveSize
		{
			get { return m_ShadowActiveSize; }
			set { m_ShadowActiveSize = value; }
		}

        [SerializeField]
        private ShadowsActive m_ShadowsActiveWhen = ShadowsActive.Hovered;
		public ShadowsActive shadowsActiveWhen
		{
			get { return m_ShadowsActiveWhen; }
			set { m_ShadowsActiveWhen = value; }
		}
		
        [SerializeField]
        private bool m_IsEnabled = true;
		public bool isEnabled
		{
			get { return m_IsEnabled; }
			set { m_IsEnabled = value; }
		}

        private int m_LastShadowNormalSize = int.MinValue;

        void Update()
        {
            if (m_LastShadowNormalSize != shadowNormalSize)
            {
                SetShadowsInstant();
                m_LastShadowNormalSize = shadowNormalSize;
            }
        }

        void OnDestroy()
        {
            m_ShadowAnims = new ShadowAnim[0];
        }

        public void OnPointerDown(PointerEventData data)
        {
            if (shadowsActiveWhen == ShadowsActive.Clicked)
            {
                SetShadows(shadowActiveSize);
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (shadowsActiveWhen == ShadowsActive.Clicked)
            {
                SetShadows(shadowNormalSize);
            }
        }

        public void OnPointerEnter(PointerEventData data)
        {
            if (shadowsActiveWhen == ShadowsActive.Hovered)
            {
                SetShadows(shadowActiveSize);
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            SetShadows(shadowNormalSize);
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (shadowsActiveWhen == ShadowsActive.Hovered)
            {
                SetShadows(shadowActiveSize);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SetShadows(shadowNormalSize);
        }

        public void SetShadows(int shadowOn)
        {
            if (isEnabled)
            {
                for (int i = 0; i < m_ShadowAnims.Length; i++)
                {
                    m_ShadowAnims[i].SetShadow(false);
                }

                if (shadowOn - 1 >= 0)
                {
                    shadowAnims[shadowOn - 1].SetShadow(true);
                }
            }
        }

        private void SetShadowsInstant()
        {
            m_ShadowAnims = GetComponentsInChildren<ShadowAnim>();

            if (m_ShadowAnims != null)
            {
                for (int i = 0; i < m_ShadowAnims.Length; i++)
                {
                    m_ShadowAnims[i].GetComponent<CanvasGroup>().alpha = 0f;
                    m_ShadowAnims[i].isOn = false;
                    m_ShadowAnims[i].animate = false;

                    if (i == m_ShadowNormalSize)
                    {
                        m_ShadowAnims[i].enabled = true;
                    }
                }

                if (shadowNormalSize >= 1 && m_ShadowAnims.Length > shadowNormalSize)
                {
                    shadowAnims[shadowNormalSize - 1].GetComponent<CanvasGroup>().alpha = 1f;
                    shadowAnims[shadowNormalSize - 1].isOn = true;
                    shadowAnims[shadowNormalSize - 1].animate = true;
                    for (int i = 0; i < m_ShadowAnims.Length; i++)
                    {
                        m_ShadowAnims[i].enabled = true;
                    }
                }
            }
        }
    }
}