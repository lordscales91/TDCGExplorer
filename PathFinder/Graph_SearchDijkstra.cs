using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    using NavGraph = SparseGraph<NavGraphNode, GraphEdge>;

    class Graph_SearchDijkstra
    {
        NavGraph graph;

        int source, target;

        List<GraphEdge> shortestPathTree;

        List<float> costToThisNode;

        List<GraphEdge> searchFrontier;

        void Search()
        {
            IndexedPriorityQueueLow queue = new IndexedPriorityQueueLow(costToThisNode, graph.NumNodes);

            //put the source node on the queue
            queue.Enqueue(source);

            //while the queue is not empty
            while (!queue.IsEmpty())
            {
                //get lowest cost node from the queue. Don't forget, the return value
                //is a *node index*, not the node itself. This node is the node not already
                //on the SPT that is the closest to the source node
                int nextClosestNode = queue.Dequeue();

                //move this edge from the frontier to the shortest path tree
                shortestPathTree[nextClosestNode] = searchFrontier[nextClosestNode];

                //if the target has been found exit
                if (nextClosestNode == target)
                    return;

                //now to relax the edges.

                //for each edge connected to the next closest node
                foreach (GraphEdge edge in graph.edges[nextClosestNode])
                {
                    //the total cost to the node this edge points to is the cost to the
                    //current node plus the cost of the edge connecting them.
                    float newCost = costToThisNode[nextClosestNode] + edge.Cost;

                    //if this edge has never been on the frontier make a note of the cost
                    //to get to the node it points to, then add the edge to the frontier
                    //and the destination node to the PQ.
                    if (searchFrontier[edge.To] == null)
                    {
                        costToThisNode[edge.To] = newCost;
                        queue.Enqueue(edge.To);
                        searchFrontier[edge.To] = edge;
                    }
                    //else test to see if the cost to reach the destination node via the
                    //current node is cheaper than the cheapest cost found so far. If
                    //this path is cheaper, we assign the new cost to the destination
                    //node, update its entry in the PQ to reflect the change and add the
                    //edge to the frontier
                    else if (newCost < costToThisNode[edge.To] && shortestPathTree[edge.To] == null)
                    {
                        costToThisNode[edge.To] = newCost;
                        //because the cost is less than it was previously, the PQ must be
                        //re-sorted to account for this.
                        queue.ChangePriority(edge.To);
                        searchFrontier[edge.To] = edge;
                    }
                }
            }
        }

        public Graph_SearchDijkstra(NavGraph graph, int source, int target)
        {
            this.graph = graph;

            this.shortestPathTree = new List<GraphEdge>();
            for (int i = 0; i < graph.NumNodes; ++i)
                shortestPathTree.Add(null);

            this.costToThisNode = new List<float>();
            for (int i = 0; i < graph.NumNodes; ++i)
                costToThisNode.Add(0.0f);

            this.searchFrontier = new List<GraphEdge>();
            for (int i = 0; i < graph.NumNodes; ++i)
                searchFrontier.Add(null);

            this.source = source;
            this.target = target;
            Search();
        }

        //returns the vector of edges that defines the SPT. If a target was given
        //in the constructor then this will be an SPT comprising of all the nodes
        //examined before the target was found, else it will contain all the nodes
        //in the graph.
        public List<GraphEdge> GetSPT()
        {
            return shortestPathTree;
        }

        //returns a vector of node indexes that comprise the shortest path
        //from the source to the target. It calculates the path by working
        //backwards through the SPT from the target node.
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
            return costToThisNode[target];
        }

        //returns the total cost to the given node
        public float GetCostToNode(int nd)
        {
            return costToThisNode[nd];
        }
    }
}
