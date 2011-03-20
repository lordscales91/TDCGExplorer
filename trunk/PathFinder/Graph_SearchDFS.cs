using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathFinder
{
    using NavGraph = SparseGraph<NavGraphNode, GraphEdge>;

    class Graph_SearchDFS
    {
        //to aid legibility
        const int Visited = 0;
        const int Unvisited = 1;
        const int NoParentAssigned = 2;

        //a reference to the graph to be searched
        NavGraph graph;

        //this records the indexes of all the nodes that are visited as the
        //search progresses
        List<int> visited;

        //this holds the route taken to the target. Given a node index, the value
        //at that index is the node's parent. ie if the path to the target is
        //3-8-27, then m_Route[8] will hold 3 and m_Route[27] will hold 8.
        List<int> route;

        //As the search progresses, this will hold all the edges the algorithm has
        //examined. THIS IS NOT NECESSARY FOR THE SEARCH, IT IS HERE PURELY
        //TO PROVIDE THE USER WITH SOME VISUAL FEEDBACK
        List<GraphEdge> spanningTree = new List<GraphEdge>();

        //the source and target node indices
        int source, target;

        //true if a path to the target has been found
        bool found;

        public Graph_SearchDFS(NavGraph graph, int source, int target)
        {
            this.graph = graph;
            this.source = source;
            this.target = target;
            this.found = false;

            this.visited = new List<int>();
            for (int i = 0; i < graph.NumNodes; ++i)
                visited.Add(Unvisited);
            
            this.route = new List<int>();
            for (int i = 0; i < graph.NumNodes; ++i)
                route.Add(NoParentAssigned);

            this.found = Search();
        }

        //this method performs the DFS search
        public bool Search()
        {
            //create a std stack of edges
            Stack<GraphEdge> stack = new Stack<GraphEdge>();

            //create a dummy edge and put on the stack
            GraphEdge dummy = new GraphEdge(source, source);
            
            stack.Push(dummy);

            //while there are edges in the stack keep searching
            while (stack.Count != 0)
            {
                //remove the edge from the stack
                GraphEdge next = stack.Pop();

                //make a note of the parent of the node this edge points to
                route[next.To] = next.From;

                //put it on the tree. (making sure the dummy edge is not placed on the tree)
                if (next != dummy)
                    spanningTree.Add(next);

                //and mark it visited
                visited[next.To] = Visited;

                //if the target has been found the method can return success
                if (next.To == target)
                    return true;

                //push the edges leading from the node this edge points to onto
                //the stack (provided the edge does not point to a previously 
                //visited node)
                foreach (GraphEdge edge in graph.edges[next.To])
                {
                    if (visited[edge.To] == Unvisited)
                        stack.Push(edge);
                }
            }

            //no path to target
            return false;
        }

        //returns a vector containing pointers to all the edges the search has examined
        public List<GraphEdge> GetSearchTree()
        {
            return spanningTree;
        }

        //returns true if the target node has been located
        public bool Found { get { return found; } }

        //returns a vector of node indexes that comprise the shortest path
        //from the source to the target
        public LinkedList<int> GetPathToTarget()
        {
            LinkedList<int> path = new LinkedList<int>();

            //just return an empty path if no path to target found or if
            //no target has been specified
            if (!found)
                return path;

            if (target < 0)
                return path;
            
            int nd = target;
            path.AddFirst(nd);
            while (nd != source)
            {
                nd = route[nd];
                path.AddFirst(nd);
            }

            return path;
        }
    }
}
