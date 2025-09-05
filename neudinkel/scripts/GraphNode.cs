using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;

/// <summary>
/// Alle moeglichen Typen fuer eine Strasse
/// </summary>
public enum StreetType
{
	//enum weist jedem member nen integer zu (0,1,2,...) die kann man manuel setzen
	//und mit Typecasts abrufen (int)type gibt den int oder falls nötig (StreetType)int
	//den typ nach kosten 
	//Probleme enstehen wenn man paare casted die nicht enthalten sind
	AUTOBAHN = 10,
	LANSTRASSE = 5,
	FELDWEG = 1
};

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
	/// Benachbarte Knoten.
	/// Die Referenz ist hier eigentlich ueberfluessig, da aus dem
	/// Verbindungspunkt ableitbar ist welcher Node angrenzt, aber naja.
	/// </summary>
	private GraphNode[] neighbors; // 0 obenrechts, 1 rechts, 2 rechtsunten, 3 linksunten, 4 links, 5 linksoben
	private Node3D tile;

    private StreetType streetType;

    public override void _Ready()
	{
		base._Ready();
		neighbors = new GraphNode[6];
		updateModel();
	}

	public override void _Process(double delta) { }

	/// <summary>
	/// Aktualisiert das gerenderte Modell je nach Status.
	/// </summary>
	public void updateModel()
	{
		if (this.GetChildren().Contains(tile)){
			this.RemoveChild(tile);
		}
		tile = new Node3D();
		int nrconnections = 0;
		List<Int32> positionConnections = [];
		for (int n = 0; neighbors.Length > n; n++)
		{
			if (neighbors[n] != null)
			{
				nrconnections++;
				positionConnections.Add(n);
			}
		}
		if(nrconnections != 2)
			{
				nrconnections -= 3;
				tile = getCityTile(nrconnections);
				AddChild(tile);
			}
			else
			{
				tile = getStreetTile(positionConnections[0], positionConnections[1]);
				AddChild(tile);
			}
		}
	

	private Node3D getStreetTile(int from, int too){
		int diff = (too - from % 6);
		var scene = ResourceLoader.Load<PackedScene>("models/test_hexagon" + diff + ".fbx"); //TODO replace with actual street
		var streetModell = scene.Instantiate();
		Node3D street = new Node3D();
		street.AddChild(streetModell);
		street.RotateZ(Mathf.DegToRad(60*from));
		return street;
	}

	private Node3D getCityTile(int size){
		var scene = ResourceLoader.Load<PackedScene>("models/test_hexagon" + size + ".fbx"); //TODO replace with actual city
		Node3D citytile = new Node3D();
		var instantiatedcity = scene.Instantiate();
		citytile.AddChild(instantiatedcity);
		return citytile;
	}


	public int getTilePosX()
	{
		return this.tilePosX;	
	}
	
	public int getTilePosY()
	{
		return this.tilePosY;	
	}

	public StreetType getStreetType(){
        return streetType;
    }

	public GraphNode[] getNeighbours(){
        return neighbors;
    }

	/// <summary>
	/// Fuegt einen Node als Nachbar hinzu.
	/// </summary>
	public void addNeighbor(GraphNode a)
	{
		// TODO(jbrause)
	}

	// TODO(jbrause): Weitere Getter und Setter implementieren
	
	/// <summary>
	/// Setzt die TilePos-Variablen und updated dabei
	/// die World-Position des Tiles.
	/// </summary>
	public void setTilePos(int tilePosX, int tilePosY)
	{
		Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
		// Hier brauchen wir ein Offset wegen der Geometrie der Tiles
		position.Z = tilePosY * 0.86f;
		// Und hier weil es Hexagone sind und die Zeilen versetzt sein muessen
		position.X = tilePosX;
		if ( tilePosY % 2 == 0 )
			position.X += 0.5f;
		this.tilePosX = tilePosX;
		this.tilePosY = tilePosY;
		this.Position = position;
	}
	
}
