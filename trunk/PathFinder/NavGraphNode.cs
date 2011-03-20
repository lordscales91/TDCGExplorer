using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace PathFinder
{
    class NavGraphNode : GraphNode
    {
        public Vector2 Position { get; set; }

        public NavGraphNode(int index, Vector2 position)
        {
            this.Index = index;
            this.Position = position;
        }
    }
}
