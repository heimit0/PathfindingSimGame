using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;

public partial class Minimap : CanvasLayer
{
    public GameManager gameManager { get; set; }
    public Graph gameGraph { get; set; }
    public int minX = 1000, minY = 1000;
    public int maxX = 0, maxY = 0;
    public int minimapNodeSize = 15;
    public int padding = 10; // px vom rand
    public int sizeX = 200, sizeY = 200;
    public int edgeThickness = 5;

    public Dictionary<GraphNode, Polygon2D> minimapNodes;
    public Dictionary<GraphNode, List<GraphNode>> edges;

    public Vector2[] tmpPolygon = {
        new Vector2(0.5f, 0),
        new Vector2(1, 0.3f),
        new Vector2(1, 0.7f),
        new Vector2(0.5f, 1),
        new Vector2(0, 0.7f),
        new Vector2(0, 0.3f)
    };

    public void initMiniMap(GameManager gameManager)
    {
        this.sizeX -= minimapNodeSize;
        this.sizeY -= minimapNodeSize;
        this.gameManager = gameManager;
        this.gameGraph = this.gameManager.graph;
        this.minimapNodes = new Dictionary<GraphNode, Polygon2D>();

        // init max size
        foreach (GraphNode node in this.gameGraph.getCities())
        {
            if (node.tilePosX < minX) minX = node.tilePosX;
            if (node.tilePosY < minY) minY = node.tilePosY;
            if (node.tilePosX > maxX) maxX = node.tilePosX;
            if (node.tilePosY > maxY) maxY = node.tilePosY;
        }

        // slope = (output_end - output_start) / (input_end - input_start)
        // output = output_start + slope * (input - input_start)

        // render
        foreach (GraphNode node in this.gameGraph.getCities())
        {
            float slopeX = ((sizeX - padding) - padding) / (maxX - minX);
            float slopeY = ((sizeY - padding) - padding) / (maxY - minY);
            int outputX = (int)(padding + slopeX * (node.tilePosX - minX));
            int outputY = (int)(padding + slopeY * (node.tilePosY - minY));

            Polygon2D newMinimapNode = new Polygon2D();
            newMinimapNode.Polygon = this.tmpPolygon;
            newMinimapNode.ZIndex = 20;
            newMinimapNode.Position = new Vector2I(outputX, outputY);
            newMinimapNode.Scale = Vector2.One * this.minimapNodeSize;
            this.minimapNodes.Add(node, newMinimapNode);
            this.AddChild(newMinimapNode);
        }

        // Edges
        this.setGraphEdges(this.gameGraph);
        foreach (KeyValuePair<GraphNode, List<GraphNode>> edge in this.edges)
        {
            foreach (GraphNode neighbour in edge.Value)
            {
                Vector2 startPos = Vector2.Zero, goalPos = Vector2.Zero;

                foreach (KeyValuePair<GraphNode, Polygon2D> minimapNode in this.minimapNodes)
                {
                    if (edge.Key.Equals(minimapNode.Key)) startPos = minimapNode.Value.Position;
                    if (neighbour.Equals(minimapNode.Key)) goalPos = minimapNode.Value.Position;
                }

                // sollte funktionieren, solang padding != 0 && minimapNodeSize != 0
                if (startPos.Equals(Vector2.Zero) || goalPos.Equals(Vector2.Zero))
                {
                    GD.PrintErr("Minimap edges irgendwas null! sollte nicht passieren!");
                    return;
                }

                Vector2 edgeDirection = goalPos - startPos;
                Vector2 edgeThicknessHelper = (new Vector2(-edgeDirection.Y, edgeDirection.X)).Normalized();
                startPos += (Vector2.One * this.minimapNodeSize / 2) - (edgeThicknessHelper * (this.edgeThickness / 2)); // center of each node
                goalPos += (Vector2.One * this.minimapNodeSize / 2) - (edgeThicknessHelper * (this.edgeThickness / 2));

                Polygon2D newMinimapEdge = new Polygon2D();
                // newMinimapEdge.Position = startPos;
                newMinimapEdge.Color = Colors.Gray;
                newMinimapEdge.ZIndex = 10;
                newMinimapEdge.Polygon = new Vector2[] {
                    startPos,
                    goalPos,
                    goalPos + edgeThicknessHelper * this.edgeThickness,
                    startPos + edgeThicknessHelper * this.edgeThickness
                };

                this.AddChild(newMinimapEdge);
            }
        }
    }

    public void setGraphEdges(Graph graph)
    {
        this.edges = new Dictionary<GraphNode, List<GraphNode>>();

        foreach (GraphNode from in graph.getCities())
        {
            foreach (GraphNode to in from.getNeighbouringCities(new GraphNode[] { }))
            {
                if (this.edges.ContainsKey(from) && this.edges[from].Contains(to)) continue;
                if (this.edges.ContainsKey(to) && this.edges[to].Contains(from)) continue;

                if (this.edges.ContainsKey(from))
                {
                    this.edges[from].Add(to);
                }
                else
                {
                    this.edges.Add(from, new List<GraphNode>() { to });
                }
            }
        }
    }
}
