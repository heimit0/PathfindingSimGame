using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class WeightedGraphEdge
{
    public GraphNode from { get; set; }
    public GraphNode to { get; set; }
    public float weight { get; set; }
    public Color color { get; set; }

    public WeightedGraphEdge(GraphNode from, GraphNode to, float weight)
    {
        this.from = from;
        this.to = to;
        this.weight = weight;
    }
}

public partial class Minimap : Panel
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
    public Dictionary<Polygon2D, WeightedGraphEdge> minimapEdges;
    public Dictionary<GraphNode, List<GraphNode>> edges;
    public List<WeightedGraphEdge> edgeWeights;
    public float minEdgeWeight = 1000.0f, maxEdgeWeight = 0.0f;
    public float edgeBaseGrayValue = 0.1f;

    public Vector2[] tmpPolygon = {
        new Vector2(0.5f, 0),
        new Vector2(1, 0.3f),
        new Vector2(1, 0.7f),
        new Vector2(0.5f, 1),
        new Vector2(0, 0.7f),
        new Vector2(0, 0.3f)
    };

    public override void _Ready()
    {
        this.sizeX -= minimapNodeSize;
        this.sizeY -= minimapNodeSize;
    }

    public void initMiniMap(GameManager gameManager)
    {
        foreach (Node child in this.GetChildren())
        {
            if (child is Polygon2D)
            {
                this.RemoveChild(child);
                child.QueueFree();
            }
        }

        this.gameManager = gameManager;
        this.gameGraph = this.gameManager.graph;
        this.minimapNodes = new Dictionary<GraphNode, Polygon2D>();
        this.minimapEdges = [];

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
            int outputX = (int)Mathf.Remap(node.tilePosX, minX, maxX, padding, sizeX - padding);
            int outputY = (int)Mathf.Remap(node.tilePosY, minY, maxY, padding, sizeY - padding);

            Polygon2D newMinimapNode = new Polygon2D();
            newMinimapNode.Polygon = this.tmpPolygon;
            newMinimapNode.Color = Colors.DarkGray;
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
                // falls die edge nicht existiert (nicht vom Graphen gefunden wurde)
                WeightedGraphEdge currentEdge = this.edgeWeights.Find(e => e.from.Equals(edge.Key) && e.to.Equals(neighbour));
                if (currentEdge is null)
                {
                    GD.PrintErr("weight of edge not known in minimap edges! Aborting...");
                    return;
                }

                // Farbe zwischen Rot und Grün basierend darauf, wie günstig (kosten) die Edge in Relation zum derzeitigen Level aussieht
                // i.e. 100% rot => teuerste Edge im Level
                // und 100% grün => günstigste Edge im Level
                // und braun => irgendwo dazwischen
                Color edgeColor = new Color(edgeBaseGrayValue, 1.0f - edgeBaseGrayValue, edgeBaseGrayValue);
                float weightColorDiff = Mathf.Remap(currentEdge.weight, this.minEdgeWeight, this.maxEdgeWeight, 0.0f, 1.0f - edgeBaseGrayValue);
                edgeColor.G -= weightColorDiff;
                edgeColor.R += weightColorDiff;
                currentEdge.color = edgeColor;

                Vector2 startPos = Vector2.Zero, goalPos = Vector2.Zero;

                // Minimap-Positionen (Viewport-Position) bekommen
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

                // schön-machen-berechnung tm c ~georg
                // berechnet ein Offset zum Center der Polygone, damit die edge mit thickness
                // basierend auf dem Winkel auch zentriert aussieht
                Vector2 edgeDirection = goalPos - startPos;
                Vector2 edgeThicknessHelper = (new Vector2(-edgeDirection.Y, edgeDirection.X)).Normalized();
                startPos += (Vector2.One * this.minimapNodeSize / 2) - (edgeThicknessHelper * (this.edgeThickness / 2)); // center of each node
                goalPos += (Vector2.One * this.minimapNodeSize / 2) - (edgeThicknessHelper * (this.edgeThickness / 2));

                // Edge hinzufügen
                Polygon2D newMinimapEdge = new Polygon2D();
                newMinimapEdge.Color = edgeColor;
                newMinimapEdge.ZIndex = 10;
                newMinimapEdge.Polygon = new Vector2[] {
                    startPos,
                    goalPos,
                    goalPos + edgeThicknessHelper * this.edgeThickness,
                    startPos + edgeThicknessHelper * this.edgeThickness
                };

                this.minimapEdges.Add(newMinimapEdge, currentEdge);
                this.AddChild(newMinimapEdge);
            }
        }
    }

    // mittlerweile ugly, aber egal
    public void setGraphEdges(Graph graph)
    {
        this.edges = new Dictionary<GraphNode, List<GraphNode>>();
        this.edgeWeights = new List<WeightedGraphEdge>();

        foreach (GraphNode from in graph.getCities())
        {
            foreach (GraphNode to in from.getNeighbouringCities(new GraphNode[] { }))
            {
                if (this.edges.ContainsKey(from) && this.edges[from].Contains(to)) continue;
                if (this.edges.ContainsKey(to) && this.edges[to].Contains(from)) continue;

                GraphNode[] edgePath = from.getPathToNeighbouringCity(to);
                float edgeWeight = 0.0f;
                foreach (GraphNode n in edgePath) edgeWeight += (int)n.streetType;
                this.edgeWeights.Add(new WeightedGraphEdge(from, to, edgeWeight));
                if (edgeWeight < this.minEdgeWeight) this.minEdgeWeight = edgeWeight;
                if (edgeWeight > this.maxEdgeWeight) this.maxEdgeWeight = edgeWeight;

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

    public void updateMinimapNodes(NeudinkelAlgorithm algorithm)
    {
        foreach (KeyValuePair<GraphNode, Polygon2D> entry in this.minimapNodes)
        {
            entry.Value.Color = Colors.DarkGray;
        }

        foreach (KeyValuePair<Polygon2D, WeightedGraphEdge> entry in this.minimapEdges)
        {
            entry.Key.Color = entry.Value.color;
        }

        GraphNode _currentNode;

        if (algorithm.currentStep == -1)
        {
            _currentNode = algorithm.goalNode;

            while (algorithm.cameFrom.ContainsKey(_currentNode))
            {
                foreach (KeyValuePair<Polygon2D, WeightedGraphEdge> entry in this.minimapEdges)
                {
                    if (entry.Value.from.Equals(_currentNode) && entry.Value.to.Equals(algorithm.cameFrom[_currentNode])) entry.Key.Color = Colors.Yellow;
                    if (entry.Value.to.Equals(_currentNode) && entry.Value.from.Equals(algorithm.cameFrom[_currentNode])) entry.Key.Color = Colors.Yellow;
                }

                foreach (KeyValuePair<GraphNode, Polygon2D> entry in this.minimapNodes)
                {
                    if (entry.Key.Equals(_currentNode)) entry.Value.Color = Colors.Yellow;
                }

                _currentNode = algorithm.cameFrom[_currentNode];
            }

            this.minimapNodes[algorithm.startNode].Color = Colors.Yellow;
            return;
        }

        // visited nodes
        foreach (KeyValuePair<GraphNode, Polygon2D> entry in this.minimapNodes)
        {
            if (algorithm.visitedNodes is null) break;
            entry.Value.Color = Colors.Gray;
            if (algorithm.visitedNodes.Contains(entry.Key)) entry.Value.Color = Colors.White;
        }

        // path to start + neighbour
        if (algorithm.currentNode is null) return;
        _currentNode = algorithm.currentNode;

        while (algorithm.cameFrom.ContainsKey(_currentNode))
        {
            foreach (KeyValuePair<Polygon2D, WeightedGraphEdge> entry in this.minimapEdges)
            {
                if (entry.Value.from.Equals(_currentNode) && entry.Value.to.Equals(algorithm.cameFrom[_currentNode])) entry.Key.Color = Colors.Yellow;
                if (entry.Value.to.Equals(_currentNode) && entry.Value.from.Equals(algorithm.cameFrom[_currentNode])) entry.Key.Color = Colors.Yellow;
            }

            foreach (KeyValuePair<GraphNode, Polygon2D> entry in this.minimapNodes)
            {
                if (entry.Key.Equals(_currentNode)) entry.Value.Color = Colors.Yellow;
            }

            _currentNode = algorithm.cameFrom[_currentNode];
        }

        this.minimapNodes[algorithm.startNode].Color = Colors.Yellow;

        // neighbours
        foreach (KeyValuePair<GraphNode, Polygon2D> entry in this.minimapNodes)
        {
            if (algorithm.currentNeighbours is null) continue;
            if (algorithm.currentNeighbourIndex >= algorithm.currentNeighbours.Length) continue;
			
            if (algorithm.currentNeighbours.Contains(entry.Key)) entry.Value.Color = Colors.Orange;
        }

        foreach (KeyValuePair<Polygon2D, WeightedGraphEdge> entry in this.minimapEdges)
        {
            if (algorithm.currentNeighbours is null) continue;
            if (algorithm.currentNeighbourIndex >= algorithm.currentNeighbours.Length) continue;

            if (entry.Value.from.Equals(algorithm.currentNode) && entry.Value.to.Equals(algorithm.currentNeighbours[algorithm.currentNeighbourIndex])) entry.Key.Color = Colors.Orange;
            if (entry.Value.to.Equals(algorithm.currentNode) && entry.Value.from.Equals(algorithm.currentNeighbours[algorithm.currentNeighbourIndex])) entry.Key.Color = Colors.Orange;
        }
    }
}
