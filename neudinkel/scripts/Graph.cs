using Godot;
using System;
using System.Collections;
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

		// TODO: temp code
		for (int i = -10; i < 10; i++)
		{
			for (int j = -10; j < 10; j++)
			{
				GraphNode n = new GraphNode();
				n.setTilePos(i, j);

				this.AddChild(n);
			}
		}
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
}
