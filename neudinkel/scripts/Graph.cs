using Godot;
using System;
using System.Collections;

public enum GraphSearchAlgorithm
{
	A_STAR,
	DIJKSTRA,
	DFS
}

public partial class Graph : Node3D
{
    public ArrayList nodes { get; set; }

    public override void _Ready()
	{
		base._Ready();

        nodes = new ArrayList();

        // TODO: temp code
        for (int i = -10; i < 10; i++)
		{
			for (int j = -10; j < 10; j++)
			{
				GraphNode n = new GraphNode();
				n.setTilePos(i, j);

                nodes.Add(n);
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

	/// <summary>
	/// Fuehrt einen Suchschritt aus, und speichert dabei
	/// den gesamten Zustand in state. Parameter liegen ebenfalls
	/// in state.
	/// </summary>
	public void doSearchStep(GraphSearchState state) {
		switch (state.algorithm)
		{
			// Tipp: Die jeweiligen Algorithmen sollten für die Übersichtlichkeit
			// in anderen Funktionen aufgeteilt werden, z.B. doSearchStepDijkstra().
			case GraphSearchAlgorithm.A_STAR:
				// TODO (gfriedr3): Implementieren
				break;
			case GraphSearchAlgorithm.DIJKSTRA:
				// TODO (mkolb2): Implementieren
				break;
			case GraphSearchAlgorithm.DFS:
				// TODO (jbrause): Implementieren
				break;
			default:
				GD.Print("Unbekannter Suchalgorithmus? Wird übersprungen...");
                break;
        }
	}

}
