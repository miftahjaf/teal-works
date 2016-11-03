using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace TexDrawLib
{
    public class ObjectPool<T> : IObjectPool where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();

        public int countAll { get; set; }

        public int countActive { get { return countAll - countInactive; } }

        public int countInactive { get { return m_Stack.Count; } }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0) {
                element = new T();
                #if UNITY_EDITOR
                countAll++;
                // Debug.LogFormat( "Pop New {0}, Total {1}", typeof(T).Name, countAll);
                if (!EditorObjectPool.l.Contains(this))
                    EditorObjectPool.l.Add(this);
                #endif
            } else {
                element = m_Stack.Pop();
            }
            return element;
        }

        public void Release(T element)
        {
            #if UNITY_EDITOR
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            #endif
            m_Stack.Push(element);
        }
    }

    public interface IObjectPool
    {
        int countAll { get; set; }

        int countActive { get; }

        int countInactive { get; }
    }
}