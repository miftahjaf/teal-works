using UnityEngine;
using System.Collections.Generic;
using System;

namespace TexDrawLib
{
    [Serializable]
    public class TexSymbolDictionary : SerializableDictionary<string, int>
    {
        protected override string NotFoundError()
        {
            return "Please Make sure the symbol exist and defined";
        }
    }

    [Serializable]
    public class TexCharMapDictionary : SerializableDictionary<char, int>
    {
        protected override string NotFoundError()
        {
            return "Please Make sure the character selected and defined";
        }
    }

    [Serializable]
    public class TexPrefsDictionary : SerializableDictionary<string, float>
    {
        protected override string NotFoundError()
        {
            return "Please Make sure you already imported XML files properly";
        }
    }

    //No More make it a serialized dictionary, for faster and less GC Overhead
    [Serializable]
    public class TexTypeFaceDictionary
    {
        [SerializeField]
        int[] typefaces = new int[6];

        public void Clear()
        {
            for (int i = 0; i < typefaces.Length; i++)
            {
                typefaces[i] = 0;
            }
        }

        public int Count
        {
            get
            {
                return typefaces.Length;
            }
        }

        public void Add(TexCharKind key, int value)
        {
            typefaces[(int)key] = value;
        }

        public int this [TexCharKind key]
        {
            get
            {
                return typefaces[(int)key];
            }
            set
            {
                Add(key, value);
            }
        }
    }

    [Serializable]
    public abstract class SerializableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public List<TKey> keys;
        public List<TValue> values;

        public SerializableDictionary()
        {
            keys = new List<TKey>();
            values = new List<TValue>();
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public int Count
        {
            get
            {
                return keys.Count;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (keys.Exists(x => x.Equals(key)))
                throw new ArgumentException("Duplicate key found");
            keys.Add(key);
            values.Add(value);
        }

        public TValue this [TKey key]
        {
            get
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if (key.Equals(keys[i]))
                        return values[i];
                }
                throw new KeyNotFoundException(string.Format("Key \"{0}\" Not Found, {1}", key, NotFoundError()));
            }
            set
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    if (key.Equals(keys[i]))
                    {
                        values[i] = value;
                        return;
                    }
                }
            }
        }

        public TValue GetOrNone(TKey key, TValue none)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (key.Equals(keys[i]))
                    return values[i];
            }
            return none;
        }

        protected abstract string NotFoundError();
    }
}