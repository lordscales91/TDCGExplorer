using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace tso2mqo
{
    class Heap<T>
    {
        public Dictionary<T, ushort> map = new Dictionary<T, ushort>();
        public List<T> ary = new List<T>();

        public void Clear()
        {
            map.Clear();
            ary.Clear();
        }
        public bool ContainsKey(T item)
        {
            return map.ContainsKey(item);
        }
        public int Count
        {
            get { return ary.Count; }
        }
        public ushort Add(T item)
        {
            ushort idx;
            if (map.ContainsKey(item))
            {
                idx = map[item];
            }
            else
            {
                idx = (ushort)ary.Count;
                map[item] = idx;
                ary.Add(item);
            }
            return idx;
        }
        public ushort this[T item]
        {
            get { return map[item]; }
        }
    }
}
