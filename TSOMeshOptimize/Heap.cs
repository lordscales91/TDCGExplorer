using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TSOMeshOptimize
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
        public void Add(T item)
        {
            map[item] = (ushort)ary.Count;
            ary.Add(item);
        }
        public ushort this[T item]
        {
            get { return map[item]; }
        }
    }
}
