using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace PathFinder
{
    using NavGraph = SparseGraph<NavGraphNode, GraphEdge>;

    class Graph_SearchAStar
    {
        NavGraph graph;

        List<float> GCosts;
        List<float> FCosts;

        int source, target;

        List<GraphEdge> shortestPathTree;
        List<GraphEdge> searchFrontier;

        //calculate the straight line distance from node nd1 to node nd2
        float Calculate(NavGraph graph, int nd1, int nd2)
        {
            return Vector2.Length(graph.GetNode(nd2).Position - graph.GetNode(nd1).Position);
        }

        void Search()
        {
            //create an indexed priority queue of nodes. The nodes with the
            //lowest overall F cost (G+H) are positioned at the front.
            IndexedPriorityQueueLow queue = new IndexedPriorityQueueLow(FCosts, graph.NumNodes);

            //put the source node on the queue
            queue.Enqueue(source);

            //while the queue is not empty
            while (!queue.IsEmpty())
            {
                //get lowest cost node from the queue
                int nextClosestNode = queue.Dequeue();

                //move this node from the frontier to the spanning tree
                shortestPathTree[nextClosestNode] = searchFrontier[nextClosestNode];

                //if the target has been found exit
                if (nextClosestNode == target)
                    return;

                //now to relax the edges.

                //for each edge connected to the next closest node
                foreach (GraphEdge edge in graph.edges[nextClosestNode])
                {
                    //calculate the heuristic cost from this node to the target (H)                       
                    float HCost = Calculate(graph, target, edge.To);

                    //calculate the 'real' cost to this node from the source (G)
                    float GCost = GCosts[nextClosestNode] + edge.Cost;

                    //if the node has not been added to the frontier, add it and update
                    //the G and F costs
                    if (searchFrontier[edge.To] == null)
                    {
                        FCosts[edge.To] = GCost + HCost;
                        GCosts[edge.To] = GCost;
                        queue.Enqueue(edge.To);
                        searchFrontier[edge.To] = edge;
                    }
                    //if this node is already on the frontier but the cost to get here
                    //is cheaper than has been found previously, update the node
                    //costs and frontier accordingly.
                    else if (GCost < GCosts[edge.To] && shortestPathTree[edge.To] == null)
                    {
                        FCosts[edge.To] = GCost + HCost;
                        GCosts[edge.To] = GCost;
                        queue.ChangePriority(edge.To);
                        searchFrontier[edge.To] = edge;
                    }
                }
            }
        }

        public Graph_SearchAStar(NavGraph graph, int source, int target)
        {
            this.graph = graph;

            this.shortestPathTree = new List<GraphEdge>();
            for (int i = 0; i < graph.NumNodes; ++i)
                shortestPathTree.Add(null);

            this.FCosts = new List<float>();
            for (int i = 0; i < graph.NumNodes; ++i)
                FCosts.Add(0.0f);

            this.GCosts = new List<float>();
            for (int i = 0; i < graph.NumNodes; ++i)
                GCosts.Add(0.0f);

            this.searchFrontier = new List<GraphEdge>();
            for (int i = 0; i < graph.NumNodes; ++i)
                searchFrontier.Add(null);

            this.source = source;
            this.target = target;
            Search();
        }

        //returns the vector of edges that the algorithm has examined
        public List<GraphEdge> GetSPT()
        {
            return shortestPathTree;
        }

        //returns a vector of node indexes that comprise the shortest path
        //from the source to the target
        public LinkedList<int> GetPathToTarget()
        {
            LinkedList<int> path = new LinkedList<int>();

            //just return an empty path if no target or no path found
            if (target < 0)
                return path;

            int nd = target;
            path.AddFirst(nd);
            while (nd != source && shortestPathTree[nd] != null)
            {
                nd = shortestPathTree[nd].From;
                path.AddFirst(nd);
            }
            return path;
        }

        //returns the total cost to the target
        public float GetCostToTarget()
        {
            return GCosts[target];
        }
    }
}
