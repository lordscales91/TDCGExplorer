using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;

namespace PathFinder
{
    using NavGraph = SparseGraph<NavGraphNode, GraphEdge>;

    public partial class Form1 : Form
    {
        const int numCellsX = 19;
        const int numCellsY = 19;

        NavGraph graph = new NavGraph();

        //this vector will store any path returned from a graph search
        LinkedList<int> path = new LinkedList<int>();

        //this vector of edges is used to store any subtree returned from 
        //any of the graph algorithms (such as an SPT)
        List<GraphEdge> subTree = new List<GraphEdge>();

        //the dimensions of the cells
        float cellWidth, cellHeight;

        //the indices of the source and target cells
        int sourceCell, targetCell;

        public Form1()
        {
            ClientSize = new Size(400, 400);
            InitializeComponent();
            int cellsUp = numCellsY;
            int cellsAcross = numCellsX;
            int cellsX = cellsAcross;
            int cellsY = cellsUp;
            cellWidth = (float)ClientSize.Width / (float)cellsX;
            cellHeight = (float)ClientSize.Height / (float)cellsY;
            sourceCell = 200;
            targetCell = 105;
            GraphHelper.CreateGrid(graph, ClientSize.Height, ClientSize.Width, cellsUp, cellsAcross);
            CreatePathBFS();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (NavGraphNode node in graph.nodes)
            {
                int left = (int)(node.Position.X - cellWidth / 2.0f);
                int top = (int)(node.Position.Y - cellHeight / 2.0f);
                e.Graphics.DrawRectangle(Pens.Black, left, top, cellWidth, cellHeight);
            }

            foreach (NavGraphNode node in graph.nodes)
            {
                e.Graphics.FillEllipse(Brushes.LightGray, node.Position.X - 2, node.Position.Y - 2, 5, 5);
                foreach (GraphEdge edge in graph.edges[node.Index])
                {
                    Vector2 from = node.Position;
                    Vector2 to = graph.GetNode(edge.To).Position;
                    e.Graphics.DrawLine(Pens.LightGray, from.X, from.Y, to.X, to.Y);
                }
            }

            foreach (GraphEdge edge in subTree)
            {
                Vector2 from = graph.GetNode(edge.From).Position;
                Vector2 to = graph.GetNode(edge.To).Position;
                e.Graphics.DrawLine(Pens.Red, from.X, from.Y, to.X, to.Y);
            }
        }

        void CreatePathDFS()
        {
            //clear any existing path
            path.Clear();
            subTree.Clear();

            //do the search
            Graph_SearchDFS DFS = new Graph_SearchDFS(graph, sourceCell, targetCell);

            //now grab the path (if one has been found)
            if (DFS.Found)
            {
                path = DFS.GetPathToTarget();
            }
            subTree = DFS.GetSearchTree();
        }

        void CreatePathBFS()
        {
            //clear any existing path
            path.Clear();
            subTree.Clear();

            //do the search
            Graph_SearchBFS BFS = new Graph_SearchBFS(graph, sourceCell, targetCell);

            //now grab the path (if one has been found)
            if (BFS.Found)
            {
                path = BFS.GetPathToTarget();
            }
            subTree = BFS.GetSearchTree();
        }
    }
}
