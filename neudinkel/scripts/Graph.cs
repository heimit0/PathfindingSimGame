using Godot;
using System;
using System.Text.Json;
using System.Collections;
using System.Text.Json.Nodes;
using System.Collections.Generic;

/*
	TODO:
		Debug Drawing - weitergeben an die Nodes/Tiles
*/

public enum GraphSearchAlgorithm
{
    A_STAR,
    DIJKSTRA,
    DFS
}

public partial class Graph : Node3D
{
    public List<GraphNode> nodes;

    public override void _Ready()
    {
        base._Ready();
        nodes = new List<GraphNode>();
    }

    public override void _Process(double delta) { }

    /// <summary>
    /// Iteriert durch alle Kindknoten und gibt
    /// die Node mit der angegebenen TilePos zurück.
    /// </summary>
    /// <returns> Die angefragte Node, oder null falls es sie nicht gibt. </returns>
    public GraphNode getNode(int tilePosX, int tilePosY)
    {
        foreach (object obj in this.GetChildren())
        {
            if (obj.GetType() == typeof(GraphNode))
            {
                GraphNode node = (GraphNode)obj;
                if (node.getTilePosX() == tilePosX && node.getTilePosY() == tilePosY)
                {
                    return node;
                }
            }
        }

        return null;
    }

    public GraphNode getNode(GraphNode node)
    {
        foreach (GraphNode n in this.nodes)
        {
            if (node.Equals(n)) return n;
        }

        return null;
    }

    public NewTileGraphNode getGhostNode(int x, int y)
    {
        foreach (object obj in this.GetChildren())
        {
            if (obj.GetType() == typeof(NewTileGraphNode))
            {
                NewTileGraphNode node = (NewTileGraphNode)obj;
                if (node.getTilePosX() == x && node.getTilePosY() == y)
                {
                    return node;
                }
            }
        }

        return null;
    }

    public GraphNode[] getNodes()
    {
        // List<GraphNode> result = new List<GraphNode>();
        // foreach (object node in this.GetChildren())
        // {
        // 	if (node.GetType() == typeof(GraphNode))
        // 		result.Add((GraphNode)node);
        // }
        // return result.ToArray();

        return this.nodes.ToArray();
    }

    /// <summary>
    /// Deserialisiert einen JSON-Objekt in ein gueltiges
    /// Graph-Element mit entsprechenden Kindknoten.
    /// </summary>
    /// <returns> false bei Erfolg, sonst true </returns>
    // TODO: Sehr unsicher, wir prüfen viel zu wenig auf Struktur und bei Casts
    public bool fromJsonObject(JsonObject json)
    {
        this.nodes = new List<GraphNode>();
        GameManager gm = this.GetParent<GameManager>();

        // Struktur des Root testen
        if (!(json.ContainsKey("tiles") && json.ContainsKey("edges")))
        {
            GD.Print("fromJsonObject: Level-Objekt ist kaputt, es fehlen tiles und/oder edges :(");
            return true;
        }

        // Kamera-Position setzen
        if (json.ContainsKey("startPos") && json["startPos"].AsArray().Count == 2)
        {
            JsonArray startPos = json["startPos"].AsArray();
            gm.EmitSignal(GameManager.SignalName.SetCameraPosition, new Vector2((float)startPos[0], (float)startPos[1]));
        }

        foreach (JsonObject n in json["tiles"].AsArray())
        {
            GraphNode node = new GraphNode();
            // wild dass C# das einfach so erlaubt...
            node.setTilePos((int)n["posX"], (int)n["posY"]);
            // abomination.
            node.setStreetType((StreetType)(int)n["type"]);
            this.nodes.Add(node);
        }

        foreach (JsonObject e in json["edges"].AsArray())
        {
            GraphNode from = this.nodes[(int)e["from"]];
            GraphNode to = this.nodes[(int)e["to"]];
            if (from is null || to is null)
            {
                GD.Print("fromJsonObject: Falsche Referenz in einer Kante");
                return true;
            }
            from.addNeighbor(to);
            if ((bool)e["bi"] == true)
                to.addNeighbor(from);
        }

        foreach (GraphNode n in this.nodes)
            this.AddChild(n);

        return false;
    }

    public void addNode(GraphNode node)
    {
        this.nodes.Add(node); // should already be child of graph
    }

    public void addRelation(GraphNode node1, GraphNode node2)
    {
        try
        {
            getNode(node1).addNeighbor(getNode(node2));
            getNode(node2).addNeighbor(getNode(node1));
        }
		catch (NullReferenceException e)
		{
            GD.PrintErr("Tried to add relation in graph of a/two non-existing node/s!");
            GD.PrintErr(e.StackTrace);
        }
    }
}
