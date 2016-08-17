using System;
using System.Collections.Generic;
using UnityEngine;

namespace TexDrawLib
{
    public static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>();

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            if(toRelease.Count > 0 && toRelease[0] is IFlushable)
            {
                for (int i = 0; i < toRelease.Count; i++)
                {
                    IFlushable obj = (IFlushable)toRelease[i];
                    (obj).Flush();
                }
            }
            toRelease.Clear();
            s_ListPool.Release(toRelease);
        }

        public static void ReleaseNoFlush(List<T> toRelease)
        {
            toRelease.Clear();
            s_ListPool.Release(toRelease);
        }
    }

    /* public static class ObjPool<T> where T : class, new()
    {
        public static Stack<T> m_objectStack = new Stack<T>();

        public static T Get()
        {
            if (m_objectStack.Count == 0)
            {
                Debug.Log("Pop New " + typeof(T).FullName);
                return new T();
            }
            else
            {
                T x = m_objectStack.Pop();
                return x;
            }
        }

        public static void Release(T t)
        {
            m_objectStack.Push(t);
        }
    }*/
    internal static class ObjPool<T> where T : class, IFlushable, new()
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<T> s_ObjPool = new ObjectPool<T>();

        public static T Get()
        {
            T obj = s_ObjPool.Get();
            obj.SetFlushed(false);
            return obj;
        }

        public static void Release(T toRelease)
        {
            if(toRelease.GetFlushed())
                return;
            toRelease.SetFlushed(true);
            s_ObjPool.Release(toRelease);
        }
    }

    internal interface IFlushable
    {
        bool GetFlushed();

        void SetFlushed(bool flushed);

        void Flush();
    }
}