using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    class GraphNode
    {
        public int Index { get; set; }

        public GraphNode()
        {
            this.Index = -1;
        }

        public GraphNode(int index)
        {
            this.Index = index;
        }
    }
}
