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
	}
	
	public override void _Process(double delta) { }

	/// <summary>
	/// Iteriert durch alle Kindknoten und gibt
	/// die Tile mit der angegebenen TilePos zurück.
	/// </summary>
	/// <returns> Das angefragte Tile, oder null falls es das nicht gibt. </returns>
	public GraphNode getTile(int tilePosX, int tilePosY)
	{
		// TODO(mkolb2)
		return null;
	}

}
