using System;
using System.Collections.Generic;
using System.Text;

namespace Tso2MqoGui
{
    public class VertexHeap<T>
    {
        public Dictionary<T, ushort>  map    = new Dictionary<T, ushort>();
        public List<T>                verts  = new List<T>();

        public void Clear()
        {
            map.Clear();
            verts.Clear();
        }

        public ushort Add(T v)
        {
            ushort n;

            if(map.TryGetValue(v, out n))
                return n;

            n   = (ushort)verts.Count;
            map.Add(v, n);
            verts.Add(v);
            return n;
        }

        public int      Count           { get { return verts.Count;   } }
        public ushort   this[T index]   { get { return map[index];    } }
        public T        this[int index] { get { return verts[index];  } }
    }
}
