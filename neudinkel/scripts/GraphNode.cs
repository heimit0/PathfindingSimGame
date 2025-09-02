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
	private int tilePosX;
	private int tilePosY;
	
	/// <summary>
	/// Benachbarte Kanten.
	/// Muss aktuell mit der Edge Liste synchron
	/// gehalten werden.
	/// </summary>
	private GraphEdge[] neighbors;
	
public override void _Ready()
	{
		base._Ready();
		// TODO(hsimosse): Beim Erstellen soll automatisch ein passendes Modell geladen
		// und als Kindobjekt hinzugefuegt werden. Eventuell gleich updateModel nutzen?
	}

	public override void _Process(double delta) { }

	/// <summary>
	/// Aktualisiert das gerenderte Modell je nach Status.
	/// </summary>
	public void updateModel()
	{
		// TODO(hsimosse): Der GraphNode sollte als Kindobjekt nen Model haben.
		// Wir ersetzen hier das Modell, je nach Zustand
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

	// TODO(jbrause): Weitere Getter und Setter implementieren
	// TODO(mkolb2): Berechnung Weltkoordinaten im entspr. Setter (siehe Kommentar an Variablen oben)
	
}
