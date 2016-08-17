//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;

namespace MaterialUI
{
    /// <summary>
    /// Can be used to create 'Tweens', to animate just about any variable
    /// Used frequently to animate the MaterialUI components
    /// The types of values that can be tweened are int, float, Vector2, Vector3, Vector4, Rect, Color
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("MaterialUI/Managers/Tween Manager")]
    public class TweenManager : MonoBehaviour
    {
        private static TweenManager m_Instance;
        private static TweenManager instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new GameObject("Tween Manager").AddComponent<TweenManager>();
                }
                return m_Instance;
            }
        }

		private const float defaultDuration = 1f;
		private const Tween.TweenType defaultTweenType = MaterialUI.Tween.TweenType.EaseOutQuint;
        private static readonly bool[] m_DefaultSubvaluesToModify = { true, true, true, true };

		private bool m_ReadyToKill;

        private static void Clear()
        {
            TweenManager[] managers = FindObjectsOfType<TweenManager>();
            for (int i = 0; i < managers.Length; i++)
            {
                Destroy(managers[i].gameObject);
            }

            if (m_Instance)
            {
                Destroy(m_Instance.gameObject);
                m_Instance = null;
            }
        }

        void OnApplicationQuit()
        {
            m_ReadyToKill = true;
        }

        void Start()
        {
            if (!Application.isPlaying)
            {
                DestroyImmediate(gameObject);
            }
        }

        void Update()
        {
            if (m_ReadyToKill)
            {
                Clear();
            }
        }

        private AutoTweener Tween(GameObject parentGameObject, object parentClass, string variableName, object endValue, float duration, float delay, Tween.TweenType tweenType, AnimationCurve sourceCurve, bool[] modifySubvalues, Action callback)
        {
            Component component = (Component)parentClass;
            if (!component.gameObject.activeSelf)
            {
                return null;
            }
            AutoTweener autoTweener = gameObject.AddComponent<AutoTweener>();
            autoTweener.Init(parentGameObject, parentClass, variableName, endValue, duration, delay, tweenType, sourceCurve, modifySubvalues, callback);
            return autoTweener;
        }

        /// <summary>
        /// Creates an AutoTweener, the types of values that can be tweened are int, float, Vector2, Vector3, Vector4, Rect, Color
        /// </summary>
        /// <param name="parentGameObject">The gameobject that the variable being tweened belongs to</param>
        /// <param name="parentClass">The class that the variable being tweened belongs to</param>
        /// <param name="variableName">The name of the variable being tweened</param>
        /// <param name="endValue">The desired result of the value being tweened</param>
        /// <param name="duration">The duration of the tween, default is 1 second</param>
        /// <param name="delay">The time to wait before tweening, default is 0</param>
        /// <param name="tweenType">The type (curve) of the tween, default is EaseOutQuint</param>
        /// <param name="animationCurve">The custom AnimationCurve to use (tweenType must be 'Custom' to use this)</param>
        /// <param name="subvaluesToModify">If the value being tweened is a struct containing subvalues (ie. Vector2), then they can be specified here - 'false' subvalues will not be modified, default is all</param>
        /// <param name="callback">The Action to invoke after the tween has finished</param>
        /// <returns>The created AutoTweener</returns>
        public static AutoTweener AutoTween(GameObject parentGameObject, object parentClass, string variableName, object endValue, float duration = defaultDuration, float delay = 0f, Tween.TweenType tweenType = defaultTweenType, AnimationCurve animationCurve = null, bool[] subvaluesToModify = null, Action callback = null)
        {
            if (!Application.isPlaying) return null;

            if (subvaluesToModify == null)
            {
                subvaluesToModify = m_DefaultSubvaluesToModify;
            }

            if (tweenType == MaterialUI.Tween.TweenType.Custom)
            {
                return instance.Tween(parentGameObject, parentClass, variableName, endValue, duration, delay, 0, animationCurve, subvaluesToModify, callback);
            }
            else
            {
                return instance.Tween(parentGameObject, parentClass, variableName, endValue, duration, delay, tweenType, null, subvaluesToModify, callback);
            }
        }
    }
}