using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    class GraphEdge
    {
        public int From { get; set; }
        public int To { get; set; }
        public float Cost { get; set; }

        public GraphEdge(int from, int to, float cost)
        {
            this.From = from;
            this.To = to;
            this.Cost = cost;
        }

        public GraphEdge(int from, int to)
        {
            this.From = from;
            this.To = to;
            this.Cost = 1.0f;
        }

        public GraphEdge()
        {
            this.From = -1;
            this.To = -1;
            this.Cost = 1.0f;
        }
    }
}
