using Godot;
using System;
using System.Collections.Generic;
using System.Data;

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
	/// abgebildet. Jede Zeile ist also zur vorherigen
	/// versetzt.
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

	public GraphNode() : base()
	{
		neighbors = new GraphNode[6];
	}

	public override void _Ready()
	{
		base._Ready();
		updateModel();
	}

	public override void _Process(double delta) { }

	/// <summary>
	/// Aktualisiert das gerenderte Modell je nach Status.
	/// </summary>
	public void updateModel()
	{
		if (this.GetChildren().Contains(tile))
		{
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
		if (nrconnections != 2)
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

	private Node3D getStreetTile(int from, int to)
	{
		int diff = Mathf.PosMod(to - from, 6);
		int model = diff > 3 ? 6 - diff : diff;
		var scene = ResourceLoader.Load<PackedScene>("assets/testtile0" + diff + ".tscn"); //TODO replace with actual street
		var streetModell = scene.Instantiate();
		Node3D street = new();
		street.AddChild(streetModell);
		street.RotateY(Mathf.DegToRad(60 * Mathf.PosMod(from, 6)));
		return street;
	}

	private Node3D getCityTile(int size)
	{
		var scene = ResourceLoader.Load<PackedScene>("assets/testtile_city00" + /**size +**/ ".tscn"); //TODO replace with actual city
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

	public StreetType getStreetType()
	{
		return streetType;
	}

	public GraphNode[] getNeighbours()
	{
		return neighbors;
	}

	/// <summary>
	/// Fuegt einen Node als Nachbar hinzu.
	/// </summary>
	public void addNeighbor(GraphNode a)
	{
		// TODO(jbrause): Pruefen ob wir WIRKLICH einen Nachbarknoten haben
		// TODO(jbrause oder mkolb2): Bestimmten um welchen Nachbarn es sich handelt auslagern in statische Hilfsmethode
		if (a.getTilePosX() > this.tilePosX && a.getTilePosY() < this.tilePosY)
			// Position 0
			this.neighbors[0] = a;
		else if (a.getTilePosX() > this.tilePosX && a.getTilePosY() == this.tilePosY)
			// Position 1
			this.neighbors[1] = a;
		else if (a.getTilePosX() == this.tilePosX && a.getTilePosY() > this.tilePosY)
			// Position 2
			this.neighbors[2] = a;
		else if (a.getTilePosX() < this.tilePosX && a.getTilePosY() > this.tilePosY)
			// Position 3
			this.neighbors[3] = a;
		else if (a.getTilePosX() < this.tilePosX && a.getTilePosY() == this.tilePosY)
			// Position 4
			this.neighbors[4] = a;
		else if (a.getTilePosX() == this.tilePosX && a.getTilePosY() < this.tilePosY)
			// Position 5
			this.neighbors[5] = a;
		else
			GD.Print("(" + a.getTilePosX() + ", " + a.getTilePosY() + ") ist kein gültiger Nachbarknoten für (" + this.tilePosX + ", " + this.tilePosY + ")");
	}
	
	/// <summary>
	/// Setzt die TilePos-Variablen und updated dabei
	/// die World-Position des Tiles.
	/// </summary>
	public void setTilePos(int tilePosX, int tilePosY)
	{
		this.tilePosX = tilePosX;
		this.tilePosY = tilePosY;
		this.Position = convertTilePosToPos(tilePosX,tilePosY);
	}

	public void setStreetType(StreetType t)
	{
		this.streetType = t;
	}

	///<summary>
	///tilePosY 0.86 Offset wegen der Geometrie der Tiles
	///tilePosX 0.5 mal tilePosY Offset weil es Hexagone sind und die Zeilen versetzt sein muessen
	///</summary>
	public static Vector3 convertTilePosToPos(int tilePosX, int tilePosY)
	{
		return new Vector3(tilePosX + 0.5f * tilePosY,0.0f,tilePosY * 0.86f);
	}
	
	///<summary>
	///convertiert nach den gleichen regeln wie TilePosToPos nur andersrum
	///</summary>
	public static int[] convertPosToTilePos(Vector3 pos)
	{
		float tilePosY = pos.Z / 0.86f;
		float tilePosX = pos.X - 0.5f * tilePosY;
		return new int[] {(int)tilePosX,(int)tilePosY};
	}

}
