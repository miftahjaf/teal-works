//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    public abstract class DialogAnimator
    {
		protected MaterialDialog m_Dialog;
        public MaterialDialog dialog
        {
            get { return m_Dialog; }
            set { m_Dialog = value; }
        }

		protected float m_AnimationDuration;
        public float animationDuration
        {
            get { return m_AnimationDuration; }
        }

		private CanvasGroup m_Background;
        public CanvasGroup background
        {
            get
            {
                if (m_Background == null)
                {
					m_Background = Cerebro.PrefabManager.InstantiateGameObject("Dialogs/Dialog Background", m_Dialog.rectTransform.parent).GetComponent<CanvasGroup>();

                    RectTransform backgroundTransform = m_Background.transform as RectTransform;
                    backgroundTransform.SetAsFirstSibling();
                    backgroundTransform.anchoredPosition = Vector2.zero;
                    backgroundTransform.sizeDelta = Vector2.zero;

                    m_Background.GetComponent<DialogBackground>().dialogAnimator = this;
                }
                return m_Background;
            }
        }

//		private float m_BackgroundAlpha = 0.5f;
		private float m_BackgroundAlpha = 0f;
        public float backgroundAlpha
        {
            get { return m_BackgroundAlpha; }
            set { m_BackgroundAlpha = value; }
        }

        public DialogAnimator(float animationDuration = 0.5f)
        {
            m_AnimationDuration = animationDuration;
        }

        public virtual void AnimateShow(Action callback)
        {
            AnimateShowBackground();
        }

        public virtual void AnimateShowBackground()
        {
            background.blocksRaycasts = true;
            TweenManager.AutoTween(background.gameObject, background, "alpha", m_BackgroundAlpha, m_AnimationDuration);
        }

        public virtual void AnimateHide(Action callback)
        {
            AnimateHideBackground();
        }

        public virtual void AnimateHideBackground()
        {
            background.blocksRaycasts = false;
            TweenManager.AutoTween(background.gameObject, background, "alpha", 0f, m_AnimationDuration, callback: () => UnityEngine.Object.Destroy(background.gameObject));
        }

        public virtual void OnBackgroundClick()
        {
            m_Dialog.OnBackgroundClick();
        }
    }

    public class DialogAnimatorSlideUp : DialogAnimator
    {
        public DialogAnimatorSlideUp(float animationDuration = 0.5f)
        {
            m_AnimationDuration = animationDuration;
        }

        public override void AnimateShow(Action callback)
        {
            base.AnimateShow(callback);

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Dialog.rectTransform);

            m_Dialog.rectTransform.anchoredPosition = new Vector2(0f, (-Screen.height / 2f) - ((m_Dialog.rectTransform.GetProperSize().y / 2f) * 1.1f));

            m_Dialog.rectTransform.localScale = Vector3.one;

            TweenManager.AutoTween(m_Dialog.gameObject, m_Dialog.rectTransform, "anchoredPosition",
			                       Vector2.zero, m_AnimationDuration, callback: callback);
        }

        public override void AnimateHide(Action callback)
        {
            base.AnimateHide(callback);

            TweenManager.AutoTween(m_Dialog.gameObject, m_Dialog.rectTransform, "anchoredPosition",
                new Vector2(0f, (-Screen.height / 2f) - ((m_Dialog.rectTransform.GetProperSize().y / 2f) * 1.1f)),
                m_AnimationDuration, 0f, Tween.TweenType.EaseInCubed, callback: callback);
        }

        public override void AnimateHideBackground()
        {
            background.blocksRaycasts = false;
            TweenManager.AutoTween(background.gameObject, background, "alpha", 0f, m_AnimationDuration, 0f, Tween.TweenType.Linear, callback: () => UnityEngine.Object.Destroy(background.gameObject));
        }
    }
}