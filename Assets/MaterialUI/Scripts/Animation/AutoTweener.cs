//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace MaterialUI
{
    public class AutoTweener : MonoBehaviour
    {
        private bool m_CurrentlyAnimating;
        private float m_StartTime;
        private float m_DeltaTime;
        private float m_Duration;

        private AnimationCurve[] m_AnimationCurves;
        private Keyframe[][] m_Keyframes;

        private Tween.TweenType m_TweenType;

        private object m_ValueReference
        {
            get
            {
                if (m_IsProperty)
                {
                    return m_PropertyInfo.GetValue(m_ParentClass, null);
                }
                else
                {
                    return m_FieldInfo.GetValue(m_ParentClass);
                }
            }
            set
            {
                if (m_IsProperty)
                {
                    m_PropertyInfo.SetValue(m_ParentClass, value, null);
                }
                else
                {
                    m_FieldInfo.SetValue(m_ParentClass, value);
                }
            }
        }

        private Vector4 m_StartVector4;
        private Vector4 m_CurrentVector4;
        private Vector4 m_EndVector4;

        private Action m_Callback;

        private bool[] m_ModifySubvalues = new bool[4];

        private string m_ValueType;
        private int m_ValueLength;

        private bool m_IsProperty;

        private GameObject m_ParentGameObject;
        private object m_ParentClass;
        private FieldInfo m_FieldInfo;
        private PropertyInfo m_PropertyInfo;

        //	Initialize AutoTweener
        public void Init(GameObject parentGameObject, object parentComponent, string variableName, object tweenEndValue,
            float tweenDuration, float delay, Tween.TweenType type, AnimationCurve sourceCurve, bool[] modifySubvalues, Action callback)
        {
            m_ParentGameObject = parentGameObject;
            if (parentComponent == null) return;
            m_ParentClass = parentComponent;
            m_Duration = tweenDuration;
            m_TweenType = type;
            m_Callback = callback;

            for (int i = 0; i < modifySubvalues.Length; i++)
            {
                this.m_ModifySubvalues[i] = modifySubvalues[i];
            }

            CheckValueType(variableName);

            if (delay > 0f)
            {
                StartCoroutine(DelayedSetup(tweenEndValue, sourceCurve, delay));
            }
            else
            {
                Setup(tweenEndValue, sourceCurve);
            }
        }

        IEnumerator DelayedSetup(object tweenEndValue, AnimationCurve sourceCurve, float delay)
        {
            yield return new WaitForSeconds(delay);
            Setup(tweenEndValue, sourceCurve);
        }

        private void Setup(object tweenEndValue, AnimationCurve sourceCurve)
        {
            SetTypeStart(m_ValueReference, tweenEndValue);
            SetupCurves(sourceCurve);
            m_StartTime = Time.timeSinceLevelLoad;
            m_CurrentlyAnimating = true;
        }

        //	Need to check if the variable is a Field or Property, and what type it is (float, Vector3 etc)
        private void CheckValueType(string variableName)
        {
            m_FieldInfo = m_ParentClass.GetType().GetField(variableName);
            m_PropertyInfo = m_ParentClass.GetType().GetProperty(variableName);

            if (m_FieldInfo != null)
            {
                m_IsProperty = false;
                m_ValueType = m_FieldInfo.GetValue(m_ParentClass).GetType().Name;
            }
            else if (m_PropertyInfo != null)
            {
                m_IsProperty = true;
                m_ValueType = m_PropertyInfo.GetValue(m_ParentClass, null).GetType().Name;
            }
            else
            {
                Debug.LogWarning("Variable not found. Are you sure you spelled/referenced it correctly?");
                Destroy(this);
            }
        }

        //	Convert value/s to (part of) Vector4s, so we can tween the values individually
        private void SetTypeStart(object startValue, object endValue)
        {
            switch (m_ValueType)
            {
                case "Int32":
                    m_ValueLength = 1;
                    m_StartVector4 = new Vector4((int)startValue, 0f, 0f, 0f);
                    m_EndVector4 = new Vector4((int)endValue, 0f, 0f, 0f);
                    break;

                case "Single":
                    m_ValueLength = 1;
                    m_StartVector4 = new Vector4((float)startValue, 0f, 0f, 0f);
                    m_EndVector4 = new Vector4((float)endValue, 0f, 0f, 0f);
                    break;

                case "Vector2":
                    m_ValueLength = 2;
                    Vector2 tempVector2 = (Vector2)startValue;
                    m_StartVector4 = new Vector4(tempVector2.x, tempVector2.y, 0f, 0f);
                    tempVector2 = (Vector2)endValue;
                    m_EndVector4 = new Vector4(tempVector2.x, tempVector2.y, 0f, 0f);
                    break;

                case "Vector3":
                    m_ValueLength = 3;
                    Vector3 tempVector3 = (Vector3)startValue;
                    m_StartVector4 = new Vector4(tempVector3.x, tempVector3.y, tempVector3.z, 0f);
                    tempVector3 = (Vector3)endValue;
                    m_EndVector4 = new Vector4(tempVector3.x, tempVector3.y, tempVector3.z, 0f);
                    break;

                case "Vector4":
                    m_ValueLength = 4;
                    m_StartVector4 = (Vector4)startValue;
                    m_EndVector4 = (Vector4)endValue;
                    break;

                case "Rect":
                    m_ValueLength = 4;
                    Rect tempRect = (Rect)startValue;
                    m_StartVector4 = new Vector4(tempRect.x, tempRect.y, tempRect.width, tempRect.height);
                    tempRect = (Rect)endValue;
                    m_EndVector4 = new Vector4(tempRect.x, tempRect.y, tempRect.width, tempRect.height);
                    break;

                case "Color":
                    {
                        m_ValueLength = 4;
                        Color tempColor = (Color)startValue;
                        m_StartVector4 = new Vector4(tempColor.r, tempColor.g, tempColor.b, tempColor.a);
                        tempColor = (Color)endValue;
                        m_EndVector4 = new Vector4(tempColor.r, tempColor.g, tempColor.b, tempColor.a);
                        break;
                    }

                default:
                    {
                        Debug.LogWarning("Value type not supported. Destroying AutoTweener.");
                        Destroy(this);
                        return;
                    }
            }
        }

        private Vector4 GetValueAsVector4()
        {
            switch (m_ValueType)
            {
                case "Int32":
                    return new Vector4((int)m_ValueReference, 0f, 0f, 0f);

                case "Single":
                    return new Vector4((float)m_ValueReference, 0f, 0f, 0f);

                case "Vector2":
                    Vector2 tempVector2 = (Vector2)m_ValueReference;
                    return new Vector4(tempVector2.x, tempVector2.y, 0f, 0f);

                case "Vector3":
                    Vector3 tempVector3 = (Vector3)m_ValueReference;
                    return new Vector4(tempVector3.x, tempVector3.y, tempVector3.z, 0f);

                case "Vector4":
                    return (Vector4)m_ValueReference;

                case "Rect":
                    Rect tempRect = (Rect)m_ValueReference;
                    return new Vector4(tempRect.x, tempRect.y, tempRect.width, tempRect.height);

                case "Color":
                    Color tempColor = (Color)m_ValueReference;
                    return new Vector4(tempColor.r, tempColor.g, tempColor.b, tempColor.a);
            }
            return new Vector4();
        }

        //	Gets the keyframes and stretches them to the animation values/duration
        private void SetupCurves(AnimationCurve sourceCurve)
        {
            m_AnimationCurves = new AnimationCurve[m_ValueLength];  // This depends on vector length
            m_Keyframes = new Keyframe[m_ValueLength][];  // This depends on vector length

            if (m_TweenType == Tween.TweenType.Custom)
            {
                for (int i = 0; i < m_ValueLength; i++)
                {
                    m_Keyframes[i] = sourceCurve.keys;
                }
            }
            else
            {
                GetKeys(Tween.GetAnimCurveKeys(m_TweenType));
            }

            for (int i = 0; i < m_ValueLength; i++)
            {
                for (int j = 0; j < m_Keyframes[i].Length; j++)
                {
                    m_Keyframes[i][j].value *= m_EndVector4[i] - m_StartVector4[i];
                    m_Keyframes[i][j].value += m_StartVector4[i];
                    m_Keyframes[i][j].time *= m_Duration;
                }

                m_AnimationCurves[i] = new AnimationCurve(m_Keyframes[i]);

                if (sourceCurve == null)
                {
                    for (int j = 0; j < m_Keyframes[i].Length; j++)
                    {
                        m_AnimationCurves[i].SmoothTangents(j, 0f);
                    }
                }
            }
        }

        void Update()
        {
            if (m_ParentGameObject == null)
            {
                Destroy(this);
                return;
            }

            if (!m_CurrentlyAnimating) return;

            m_DeltaTime = Time.timeSinceLevelLoad - m_StartTime;

            if (m_DeltaTime <= m_Duration)
            {
                UpdateCurrentValue();
            }
            else
            {
                m_DeltaTime = m_Duration;
                UpdateCurrentValue(true);
                if (m_Callback != null)
                {
                    m_Callback.Invoke();
                    m_Callback = null;
                }
                Destroy(this);
            }
        }

        private void UpdateCurrentValue(bool forceDone = false)
        {
            m_CurrentVector4 = GetValueAsVector4();

            if (m_ModifySubvalues[0])
            {
                if (forceDone)
                {
                    m_CurrentVector4.x = m_EndVector4.x;
                }
                else
                {
                    m_CurrentVector4.x = m_AnimationCurves[0].Evaluate(m_DeltaTime);
                }

            }

            if (m_ValueLength > 1 && m_ModifySubvalues[1])
            {
                if (forceDone)
                {
                    m_CurrentVector4.y = m_EndVector4.y;
                }
                else
                {
                    m_CurrentVector4.y = m_AnimationCurves[1].Evaluate(m_DeltaTime);
                }
            }

            if (m_ValueLength > 2 && m_ModifySubvalues[2])
            {
                if (forceDone)
                {
                    m_CurrentVector4.z = m_EndVector4.z;
                }
                else
                {

                    m_CurrentVector4.z = m_AnimationCurves[2].Evaluate(m_DeltaTime);
                }
            }

            if (m_ValueLength > 3 && m_ModifySubvalues[3])
            {
                if (forceDone)
                {
                    m_CurrentVector4.w = m_EndVector4.w;
                }
                else
                {

                    m_CurrentVector4.w = m_AnimationCurves[3].Evaluate(m_DeltaTime);
                }
            }

            switch (m_ValueType)
            {
                case "Int32":
                    m_ValueReference = Mathf.RoundToInt(m_CurrentVector4.x);
                    break;

                case "Single":
                    m_ValueReference = m_CurrentVector4.x;
                    break;

                case "Vector2":
                    m_ValueReference = new Vector2(m_CurrentVector4.x, m_CurrentVector4.y);
                    break;

                case "Vector3":
                    m_ValueReference = new Vector3(m_CurrentVector4.x, m_CurrentVector4.y, m_CurrentVector4.z);
                    break;

                case "Vector4":
                    m_ValueReference = m_CurrentVector4;
                    break;

                case "Rect":
                    m_ValueReference = new Rect(m_CurrentVector4.x, m_CurrentVector4.y, m_CurrentVector4.z, m_CurrentVector4.w);
                    break;

                case "Color":
                    m_ValueReference = new Color(m_CurrentVector4.x, m_CurrentVector4.y, m_CurrentVector4.z, m_CurrentVector4.w);
                    break;
            }
        }

        private void GetKeys(float[][] source)
        {
            for (int i = 0; i < m_ValueLength; i++)
            {
                m_Keyframes[i] = new Keyframe[source.Length];

                for (int j = 0; j < m_Keyframes[i].Length; j++)
                {
                    m_Keyframes[i][j].time = source[j][0];
                    m_Keyframes[i][j].value = source[j][1];
                }
            }
        }

    }
}