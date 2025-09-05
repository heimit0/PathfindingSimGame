using Godot;
using System;
using System.Text.Json;
using System.Collections;
using System.Text.Json.Nodes;
using System.Collections.Generic;

public enum GraphSearchAlgorithm
{
	A_STAR,
	DIJKSTRA,
	DFS
}

public partial class Graph : Node3D
{
	public override void _Ready()
	{
		base._Ready();
		// TODO: Temp code
		string json = """
		{
			"name": "Testlevel",
			"tiles": [
				{ "posX": 0, "posY": 0 },
				{ "posX": 0, "posY": 1 }
			],
			"edges": [
				{ "from": 0, "to": 1, "bi": true, "type": 1  }
			]
		}
		""";
		this.fromJsonObject(JsonNode.Parse(json).AsObject());
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

	public GraphNode[] getNodes()
	{
		List<GraphNode> result = new List<GraphNode>();
		foreach (object node in this.GetChildren())
		{
			if (node.GetType() == typeof(GraphNode))
				result.Add((GraphNode)node);
		}
		return result.ToArray();
	}
	
	/// <summary>
	/// Deserialisiert einen JSON-Objekt in ein gueltiges
	/// Graph-Element mit entsprechenden Kindknoten.
	/// </summary>
	/// <returns> false bei Erfolg, sonst true </returens>
	// TODO: Sehr unsicher, wir prüfen viel zu wenig auf Struktur und bei Casts
	public bool fromJsonObject(JsonObject json)
	{
		List<GraphNode> nodes = new List<GraphNode>();
		
		// Struktur des Root testen
		if (!(json.ContainsKey("tiles") && json.ContainsKey("edges")))
		{
			GD.Print("Level-Objekt ist kaputt, es fehlen tiles und/oder edges :(");
			return true;
		}
		
		foreach (JsonObject n in json["tiles"].AsArray())
		{
			GraphNode node = new GraphNode();
			// wild dass C# das einfach so erlaubt...
			node.setTilePos((int) n["posX"], (int) n["posY"]);
			nodes.Add(node);
		}
		
		foreach (JsonObject e in json["edges"].AsArray())
		{
			// TODO: Edges lesen
		}
		
		foreach (GraphNode n in nodes)
			this.AddChild(n);
		
		return false;
	}
	
}
