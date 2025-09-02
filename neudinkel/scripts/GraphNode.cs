using Godot;
using System;

public partial class GraphNode : Node3D
{

	/// <summary>
	/// Position auf dem Grid in Tile-Koordinaten.
	/// Dabei wird das hexagonale Grid auf ein rechteckiges
	/// abgebildet. Jede zweite Zeile wird im world-space also
	/// um 0.5*width verschoben.
	/// </summary> 
	private int tilePosX;
	private int tilePosY;
	
	/// <summary>
	/// Benachbarte Kanten.
	/// Muss aktuell mit der Edge Liste synchron
	/// gehalten werden.
	/// </summary>
	private GraphEdge[] neighbors;
	
	/// <summary>
	/// Konstruktor.
	/// Berechnet automatisch die Position
	/// in der Welt aus der tilePos.
	/// </summary>
	public GraphNode(int tilePosX, int tilePosY)
	{
		this.tilePosX = tilePosX;
		this.tilePosY = tilePosY;
	}

}
