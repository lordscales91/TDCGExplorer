using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PathFinder
{
    class SparseGraph<NodeType, EdgeType>
        where NodeType : GraphNode
        where EdgeType : GraphEdge, new()
    {
        public List<NodeType> nodes = new List<NodeType>();
        public List<LinkedList<EdgeType>> edges = new List<LinkedList<EdgeType>>();

        bool digraph;

        public bool IsDigraph()
        {
            return digraph;
        }

        //the index of the next node to be added
        int nextNodeIndex;

        public SparseGraph(bool digraph)
        {
            this.nextNodeIndex = 0;
            this.digraph = digraph;
        }

        //returns the node at the given index
        public NodeType GetNode(int index)
        {
            Debug.Assert(index < nodes.Count && index >= 0, 
                    "<SparseGraph::GetNode>: invalid index");

            return nodes[index];
        }

        //const method for obtaining a reference to an edge
        public EdgeType GetEdge(int from, int to)
        {
            Debug.Assert(from < nodes.Count && from >= 0 && nodes[from].Index != -1,
                    "<SparseGraph::GetEdge>: invalid 'from' index");

            Debug.Assert(to < nodes.Count && to >= 0 && nodes[to].Index != -1,
                    "<SparseGraph::GetEdge>: invalid 'to' index");

            foreach (EdgeType edge in edges[from])
            {
                if (edge.To == to)
                    return edge;
            }
            return null;
        }

        //retrieves the next free node index
        public int GetNextFreeNodeIndex()
        {
            return nextNodeIndex;
        }

        //adds a node to the graph and returns its index
        public int AddNode(NodeType node)
        {
            if (node.Index < nodes.Count)
            {
                nodes[node.Index] = node;
                return nextNodeIndex;
            }
            else
            {
                nodes.Add(node);
                edges.Add(new LinkedList<EdgeType>());
                return nextNodeIndex++;
            }
        }

        //removes a node by setting its index to invalid_node_index
        public void RemoveNode(int node)
        {
        }

        //
        //  returns true if the edge is not present in the graph. Used when adding
        //  edges to prevent duplication
        public bool UniqueEdge(int from, int to)
        {
            foreach (EdgeType edge in edges[from])
            {
                if (edge.To == to)
                    return false;
            }
            return true;
        }

        //Use this to add an edge to the graph. The method will ensure that the
        //edge passed as a parameter is valid before adding it to the graph. If the
        //graph is a digraph then a similar edge connecting the nodes in the opposite
        //direction will be automatically added.
        public void AddEdge(EdgeType edge)
        {
            //first make sure the from and to nodes exist within the graph 
            Debug.Assert(edge.From < nextNodeIndex && edge.To < nextNodeIndex,
                    "<SparseGraph::AddEdge>: invalid node index");

            //make sure both nodes are active before adding the edge
            if (nodes[edge.To].Index != -1 && nodes[edge.From].Index != -1)
            {
                //add the edge, first making sure it is unique
                if (UniqueEdge(edge.From, edge.To))
                    edges[edge.From].AddLast(edge);

                //if the graph is undirected we must add another connection in the opposite
                //direction
                if (!digraph)
                {
                    //check to make sure the edge is unique before adding
                    if (UniqueEdge(edge.To, edge.From))
                    {
                        EdgeType newEdge = new EdgeType();

                        newEdge.To = edge.From;
                        newEdge.From = edge.To;
                        newEdge.Cost = edge.Cost;

                        edges[edge.To].AddLast(newEdge);
                    }
                }
            }
        }

        //removes the edge connecting from and to from the graph (if present). If
        //a digraph then the edge connecting the nodes in the opposite direction 
        //will also be removed.
        public void RemoveEdge(int from, int to)
        {
        }

        public int NumNodes { get { return nodes.Count; } }
    }
}
