using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace PathFinder
{
    using NavGraph = SparseGraph<NavGraphNode, GraphEdge>;

    static class GraphHelper
    {

        //
        //  returns true if x,y is a valid position in the map
        public static bool ValidNeighbour(int x, int y, int numCellsX, int numCellsY)
        {
            return x >= 0 && x < numCellsX && y >= 0 && y < numCellsY;
        }

        //
        //  use to add he eight neighboring edges of a graph node that 
        //  is positioned in a grid layout
        public static void AddAllNeighboursToGridNode(NavGraph graph, int row, int col, int numCellsX, int numCellsY)
        {
            for (int i = -1; i <= +1; ++i)
            {
                for (int j = -1; j <= +1; ++j)
                {
                    //skip if equal to this node
                    if (i == 0 && j == 0)
                        continue;

                    int nodeX = col + j;
                    int nodeY = row + i;

                    //check to see if this is a valid neighbour
                    if (ValidNeighbour(nodeX, nodeY, numCellsX, numCellsY))
                    {
                        //calculate the distance to this node
                        Vector2 posNode = graph.GetNode(row * numCellsX + col).Position;
                        Vector2 posNeighbour = graph.GetNode(nodeY * numCellsX + nodeX).Position;

                        float dist = Vector2.Length(posNeighbour - posNode);

                        GraphEdge newEdge;

                        newEdge = new GraphEdge(row * numCellsX + col, nodeY * numCellsX + nodeX, dist);
                        graph.AddEdge(newEdge);

                        //if graph is not a diagraph then an edge needs to be added going
                        //in the other direction
                        if (!graph.IsDigraph())
                        {
                            newEdge = new GraphEdge(nodeY * numCellsX + nodeX, row * numCellsX + col, dist);
                            graph.AddEdge(newEdge);
                        }
                    }
                }
            }
        }

        //
        //  creates a graph based on a grid layout. This function requires the 
        //  dimensions of the environment and the number of cells required horizontally
        //  and vertically 
        public static void CreateGrid(NavGraph graph, int cySize, int cxSize, int numCellsY, int numCellsX)
        {
            //need some temporaries to help calculate each node center
            float cellWidth = (float)cxSize / (float)numCellsX;
            float cellHeight = (float)cySize / (float)numCellsY;

            float midX = cellWidth / 2.0f;
            float midY = cellHeight / 2.0f;

            //first create all the nodes
            for (int row = 0; row < numCellsY; ++row)
            {
                for (int col = 0; col < numCellsX; ++col)
                {
                    graph.AddNode(new NavGraphNode(graph.GetNextFreeNodeIndex(), new Vector2(midX + col * cellWidth, midY + row * cellHeight)));
                }
            }

            //now to calculate the edges. (A position in a 2d array [x][y] is the
            //same as [y*numCellsX + x] in a 1d array). Each cell has up to eight
            //neighbours.
            for (int row = 0; row < numCellsY; ++row)
            {
                for (int col = 0; col < numCellsX; ++col)
                {
                    AddAllNeighboursToGridNode(graph, row, col, numCellsX, numCellsY);
                }
            }
        }
    }
}
