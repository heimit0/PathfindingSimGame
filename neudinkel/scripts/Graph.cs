using Godot;
using System;

public enum GraphSearchAlgorithm
{
	A_STAR,
	DIJKSTRA,
	DFS
}

/// <summary>
/// Bildet den Zustand einer Wegfindung ab.
/// </summary>
public class GraphSearchState
{
	
	private readonly GraphSearchAlgorithm algorithm;
	
	public GraphSearchState(GraphSearchAlgorithm algorithm)
	{
		
	}
	
}

public partial class Graph : Node3D
{

	/// <summary>
	/// Erstellt ein Graph-Objekt
	/// </summary>
	public Graph()
	{
		 
	}
	
	/// <summary>
	/// Iteriert durch alle Kindknoten und gibt
	/// die Tile mit der angegebenen TilePos zurück.
	/// </summary>
	/// <returns> Das angefragte Tile, oder null falls es das nicht gibt. </returns>
	public GraphNode getTile(int tilePosX, int tilePosY)
	{
		return null;
	}
	
	public GraphEdge[] getRoute(GraphNode from, GraphNode to)
	{
		return null;
	}

}
