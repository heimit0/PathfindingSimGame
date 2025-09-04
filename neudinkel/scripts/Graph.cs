using Godot;
using System;

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
	/// die Tile mit der angegebenen TilePos zurück.
	/// </summary>
	/// <returns> Das angefragte Tile, oder null falls es das nicht gibt. </returns>
	public GraphNode getTile(int tilePosX, int tilePosY)
	{
		foreach (GraphNode node in this.GetChildren())
		{
			if(node.getTilePosX() == tilePosX && node.getTilePosY() == tilePosY)
			{
				return node;
			}
		}
		return null;
	}

}
