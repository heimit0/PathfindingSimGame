using Godot;
using System;

public partial class GraphNode : Node3D
{

	/// <summary>
	/// Position auf dem Grid in Tile-Koordinaten.
	/// Dabei wird das hexagonale Grid auf ein rechteckiges
	/// abgebildet. Jede zweite Zeile wird im world-space also
	/// um 0.5*breite verschoben.
	/// </summary> 
	private readonly int tilePosX;
	private readonly int tilePosY;
	
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
		// TODO(mkolb2): Berechnung Weltkoordinaten (siehe Kommentar an Variablen oben)
	}
	
	public int getTilePosX()
	{
		return this.tilePosX;	
	}
	
	public int getTilePosY()
	{
		return this.tilePosY;	
	}

	/// <summary>
	/// Fuegt eine Edge als Nachbar hinzu.
	/// Erinnerung: Edge ebenfalls updaten!
	/// </summary>
	public void addNeighbor(GraphNode a)
	{
		// TODO(jbrause)
	}
	
	// TODO(jbrause): Getter implementieren, Setter nur wo sinnvoll
	// (Als readonly gesetzte Variablen koennen nicht nochmal gesetzt werden)
	
}
