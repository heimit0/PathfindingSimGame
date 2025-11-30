using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;

/*
	TODO:
		Raycasting/Auswahl - zum Auswaehlen der Tiles per Maus (vielleicht auch im Graph?)
		Marker - zum Hervorheben der ausgewaehlten Tile
		Variationen - zufaellige Auswahl einer Tile-Variation
		Batch Rendering - damit wir viele Tiles haben koennen
*/

/// <summary>
/// Alle moeglichen Typen fuer eine Strasse
/// </summary>
public enum StreetType
{
	//enum weist jedem member nen integer zu (0,1,2,...) die kann man manuel setzen
	//und mit Typecasts abrufen (int)type gibt den int oder falls nötig (StreetType)int
	//den typ nach kosten 
	//Probleme enstehen wenn man paare casted die nicht enthalten sind
	EMPTY = 0,
	AUTOBAHN = 1,
	LANSTRASSE = 2,
	FELDWEG = 3,
	SANDWEG = 4
};

public partial class GraphNode : Node3D
{

	/// <summary>
	/// Position auf dem Grid in Tile-Koordinaten.
	/// Dabei wird das hexagonale Grid auf ein rechteckiges
	/// abgebildet. Jede Zeile ist also zur vorherigen
	/// versetzt.
	/// </summary>
	public int tilePosX;
	public int tilePosY;

	/// <summary>
	/// Benachbarte Knoten.
	/// Die Referenz ist hier eigentlich ueberfluessig, da aus dem
	/// Verbindungspunkt ableitbar ist welcher Node angrenzt, aber naja.
	/// </summary>
	public GraphNode[] neighbours; // 0 obenrechts, 1 rechts, 2 rechtsunten, 3 linksunten, 4 links, 5 linksoben
	public Node3D tile;
	public Node3D tileBase;

	/// <summary>
	/// zeigt ob node gehighlighted ist oder nich
	/// </summary>
	public bool highlighted { get; set; }

	private StreetType streetType;

	public GraphNode() : base()
	{
		neighbours = new GraphNode[6];
	}

	public override void _Ready()
	{
		base._Ready();
		this.tileBase = (Node3D)ResourceLoader.Load<PackedScene>("assets/testing/testtile00.tscn").Instantiate();
		this.AddChild(this.tileBase);
		updateModel();
		this.highlighted = false;
	}

	public override void _Process(double delta) { }

	/// <summary>
	/// Aktualisiert das gerenderte Modell je nach Status.
	/// </summary>
	public void updateModel()
	{
		if (this.GetChildren().Contains(tile)) this.RemoveChild(tile);

		tile = new Node3D();

		List<Int32> connections = [];
		for (int n = 0; n < neighbours.Length; n++)
		{
			if (neighbours[n] != null)
			{
				connections.Add(n);
			}
		}

		// GD.Print(this.getTilePosString() + " has nei: " + connections.Count);
		// string s = this.getTilePosString() + " has neis: [";

		// foreach (GraphNode n in this.neighbours) if (n != null) s += "(" + n.getTilePosString() + "), ";

		// s += "]";
		// GD.Print(s);

		if (connections.Count == 0)
		{
			tile = getEmptyTile();
		}
		else if (connections.Count != 2)
		{
			int c = connections.Count < 2 ? 3 : 0;

			tile = getCityTile(connections.Count + c);
		}
		else
		{
			// StreetType streetType = StreetType.EMPTY;
			// bool isDifferentStreetType = false;

			// foreach (GraphNode n in this.getNeighboursUnsorted())
			// {
			// 	if (n == null) continue;
			//     if ((int)streetType != 0 && (int)streetType != (int)n.getStreetType())
			//     {
			//         isDifferentStreetType = true;
			//         GD.Print("diff street type: " + (int) streetType + ", " + (int) n.getStreetType());
			//         GD.Print(this.getTilePosString() + " :: " + n.getTilePosString());
			//     }
			//     streetType = n.getStreetType();
			// }


			// TODO idk
			// tile = isDifferentStreetType ? getCityTile(3) : getStreetTile(connections[0], connections[1]);
			tile = getStreetTile(connections[0], connections[1]);
		}

		AddChild(tile);
	}

	private Node3D getStreetTile(int from, int to)
	{
		int diff = to - from;
		int variation = Globals.rand.Next(1, 3);

		if (diff > 3)
		{
			from = to;
			diff = 6 - diff;
		}

		var scene = ResourceLoader.Load<PackedScene>("assets/tiles/street" + (int)this.streetType + "_0" + diff + "_" + "1" + ".tscn"); // TODO: add variations
		var streetModell = scene.Instantiate();
		Node3D street = new();
		street.AddChild(streetModell);
		street.RotateY(Mathf.DegToRad(-60 * from));
		return street;
	}

	private Node3D getCityTile(int size)
	{
		var scene = ResourceLoader.Load<PackedScene>("assets/tiles/city_0" + size + "_" + "1" + ".tscn"); //TODO add variations
		Node3D citytile = new Node3D();
		var instantiatedcity = scene.Instantiate();
		citytile.AddChild(instantiatedcity);
		return citytile;
	}

	private Node3D getEmptyTile()
	{
		var scene = ResourceLoader.Load<PackedScene>("assets/testing/testtile00.tscn"); //TODO replace with actual empty tile
		Node3D emptyTile = new Node3D();
		var instanciated = scene.Instantiate();
		emptyTile.AddChild(instanciated);
		return emptyTile;
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
		return neighbours;
	}

	// get next Knoten (Knoten := neighbours > 2)
	public GraphNode[] getNeighbouringCities(List<GraphNode> _visitedNeighbours)
	{
		List<GraphNode> neighbouringCities = new List<GraphNode>();
		List<GraphNode> visitedNeighbours = _visitedNeighbours;

		foreach (GraphNode immediateNeighbour in this.getNeighboursUnsorted())
		{
			if (visitedNeighbours.Contains(immediateNeighbour)) continue;
			visitedNeighbours.Add(immediateNeighbour);

			if (immediateNeighbour.getNeighboursUnsorted().Length > 2 && !neighbouringCities.Contains(immediateNeighbour))
			{
				neighbouringCities.Add(immediateNeighbour);
				continue;
			}

			// gucke den anderen nachbar an und guck, ob er ein Knoten ist
			foreach (GraphNode n in immediateNeighbour.getNeighboursUnsorted())
			{
				if (!n.Equals(this) && !neighbouringCities.Contains(n) && n.getNeighboursUnsorted().Length > 2)
				{
					neighbouringCities.Add(n);
					visitedNeighbours.Add(n);
				}
			}

			GraphNode[] currentResult = immediateNeighbour.getNeighbouringCities(visitedNeighbours);
			if (currentResult == null) continue;

			neighbouringCities.AddRange(currentResult);
		}

		return neighbouringCities.ToArray().Length > 0 ? neighbouringCities.ToArray() : null;
	}

	// Wir gehen davon aus, dass man weiß, dass es benachbarte Knoten gibt
	// Außerdem muss ein Nachbar als Startpunkt übergeben werden
	public GraphNode[] getPathToNeighbouringCity(GraphNode goalNode)
	{
		List<List<GraphNode>> paths = []; // bis zu 6 Pfade für jeden direkten Nachbarn von der Startnode
		foreach (GraphNode head in this.getNeighboursUnsorted()) paths.Add([head]); // pfade initialisieren

		// nicht fragen bitte ~georg
		while (true)
		{
			foreach (List<GraphNode> path in paths)
			{
				// win condition
				GraphNode head = path[path.Count - 1];
				if (head.Equals(goalNode)) return path.ToArray();

				// jeden Pfad um 1 vom start weg erweitern
				foreach (GraphNode newHead in head.getNeighboursUnsorted())
				{
					if (!newHead.Equals(this) && (path.Count < 2 || !newHead.Equals(path[path.Count - 2]))) path.Add(newHead);
				}
			}
		}
	}

	/// <summary>
	/// Fuegt einen Node als Nachbar hinzu.
	/// </summary>
	public void addNeighbor(GraphNode a)
	{
		// TODO(jbrause): Pruefen ob wir WIRKLICH einen Nachbarknoten haben
		// TODO(jbrause oder mkolb2): Bestimmten um welchen Nachbarn es sich handelt auslagern in statische Hilfsmethode
		if ((int)getNeighbourType(this, a) != -1)
		{
			this.neighbours[(int)getNeighbourType(this, a)] = a;
		}
		else
			GD.Print("(" + a.getTilePosX() + ", " + a.getTilePosY() + ") ist kein gültiger Nachbarknoten für (" + this.tilePosX + ", " + this.tilePosY + ")");
	}

	public void removeNeighbour(GraphNode n)
	{
		for (int i = 0; i < this.neighbours.Length; i++)
		{
			GraphNode current = this.neighbours[i];

			if (current == null) continue;

			// GD.Print(this.getTilePosString() + ": current = " + current.getTilePosString() + ", origin=" + n.getTilePosString());
			if (current.getTilePosX().Equals(n.getTilePosX()) && current.getTilePosY().Equals(n.getTilePosY()))
			{
				GD.Print("neighbour " + this.neighbours[i].getTilePosString() + " removed from " + this.getTilePosString());
				this.neighbours[i] = null;
			}
		}

		updateModel();
	}

	public void clearNeighbours()
	{
		// clear connection from neighbours to this
		foreach (GraphNode n in this.getNeighboursUnsorted())
		{
			n.removeNeighbour(this);
		}

		this.neighbours = new GraphNode[6];
	}

	// gibt nur die existierenden Nachbarn mit der TileRelation zurück
	// für Algorithmen
	public GraphNode[] getNeighboursUnsorted()
	{
		List<GraphNode> _neighbours = new List<GraphNode>();

		for (int i = 0; i < this.neighbours.Length; i++)
		{
			if (this.neighbours[i] != null)
			{
				_neighbours.Add(this.neighbours[i]);
			}
		}

		return _neighbours.ToArray();
	}

	/// <summary>
	/// Setzt die TilePos-Variablen und updated dabei
	/// die World-Position des Tiles.
	/// </summary>
	public void setTilePos(int tilePosX, int tilePosY)
	{
		this.tilePosX = tilePosX;
		this.tilePosY = tilePosY;
		this.Position = convertTilePosToPos(tilePosX, tilePosY);
	}

	public void setStreetType(StreetType t)
	{
		this.streetType = t; // updateMode() in BauGameManager
	}

	///<summary>
	///tilePosY 0.86 Offset wegen der Geometrie der Tiles
	///tilePosX 0.5 mal tilePosY Offset weil es Hexagone sind und die Zeilen versetzt sein muessen
	///</summary>
	public static Vector3 convertTilePosToPos(int tilePosX, int tilePosY)
	{
		return new Vector3(tilePosX + 0.5f * tilePosY, 0.0f, tilePosY * 0.86f);
	}

	///<summary>
	///convertiert nach den gleichen regeln wie TilePosToPos nur andersrum
	///</summary>
	public static int[] convertPosToTilePos(Vector3 pos)
	{
		float preTilePosY = pos.Z / 0.86f;
		float preTilePosX = pos.X - 0.5f * preTilePosY;
		int tilePosX = (int) Math.Round(preTilePosX);
		float tilePosXFracture = preTilePosX - (float)Math.Round(preTilePosX);
		if (tilePosXFracture > 0.5)
		{
			tilePosX++;
		}
		int tilePosY = (int)Math.Round(preTilePosY);
		float tilePosYFracture = preTilePosY - (float)Math.Round(preTilePosY);
		if (tilePosYFracture > 0.5)
		{
			tilePosY++;
		}

		return new int[] { tilePosX, tilePosY };
	}

	public void toggleHighlight(bool highlight)
	{
		Vector3 pos = this.Position;
		Vector3 relPos = new Vector3(0.0f, 0.25f, 0.0f);

		if (!highlighted && highlight)
		{
			this.Position = pos + relPos;
		}
		else if (highlighted && !highlight)
		{
			this.Position = pos - relPos;
		}

		this.highlighted = highlight;
	}

	public static TileRelation getNeighbourType(GraphNode middle, GraphNode neighbour)
	{
		int x = middle.getTilePosX() - neighbour.tilePosX;
		int y = middle.getTilePosY() - neighbour.tilePosY;

		// FIXME: Fix
		/**if (Mathf.Sqrt( Mathf.pow(a.tilePosX - a.getTilePosY, 2) + Mathf.pow(a.tilePosY - tile.tilePosY, 2)) != 1) 
		{
			return TileRelation.INVALID;
		}**/


		//topright x negativ y positiv
		//right x negativ y 0
		//bottomright x 0 y negativ
		//bottomleft x positiv y negativ
		//left x positiv y 0
		//topleft x 0 y posititv
		if (x < 0 && y > 0)
			return TileRelation.TOPRIGHT;
		else if (x < 0 && y == 0)
			return TileRelation.RIGHT;
		else if (x == 0 && y < 0)
			return TileRelation.BOTTOMRIGHT;
		else if (x > 0 && y < 0)
			return TileRelation.BOTTOMLEFT;
		else if (x > 0 && y == 0)
			return TileRelation.LEFT;
		else if (x == 0 && y > 0)
			return TileRelation.TOPLEFT;
		else
			return TileRelation.INVALID;
	}

	public String getTilePosString()
	{
		return this.getTilePosX() + ", " + this.getTilePosY();
	}

	public int[] getNeighbourPos(int direction)
	{
		switch (direction)
		{
			case 0: return new int[] { this.tilePosX + 1, this.tilePosY - 1 };
			case 1: return new int[] { this.tilePosX + 1, this.tilePosY };
			case 2: return new int[] { this.tilePosX, this.tilePosY + 1 };
			case 3: return new int[] { this.tilePosX - 1, this.tilePosY + 1 };
			case 4: return new int[] { this.tilePosX - 1, this.tilePosY };
			case 5: return new int[] { this.tilePosX, this.tilePosY - 1 };
		}

		// shouldnt happen
		return null;
	}
}
