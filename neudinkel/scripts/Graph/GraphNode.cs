using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/*
	TODO:
		Marker - zum Hervorheben der ausgewaehlten Tile
		Variationen - zufaellige Auswahl einer Tile-Variation
		Batch Rendering - damit wir viele Tiles haben koennen
*/

public enum StreetType
{
	EMPTY = 0,
	AUTOBAHN = 1,
	LANSTRASSE = 2,
	FELDWEG = 3,
	SANDWEG = 4
};

public partial class GraphNode : Node3D
{
	// Position im Neudinkel-Hexagon-Grid-System
	public int tilePosX { get; set; }
	public int tilePosY { get; set; }

	// Array mit Länge 6, jeder Index = null oder GraphNode
	public GraphNode[] neighbours { get; set; } = new GraphNode[6];
	// 0 obenrechts, 1 rechts, 2 rechtsunten, 3 linksunten, 4 links, 5 linksoben

	// city / road - sprite
	public Node3D tile;

	// base-sprite (gras)
	public Node3D tileBase;

	// start/goal markers
	private Node3D startFlag;
	private Node3D goalPost;

	// hervorgehobn ja / nein
	public bool highlighted;
	public StreetType streetType { get; set; }

    public Boolean isStartNode;
    public Boolean isGoalNode;



	[Export]
	public float highlightedYOffset = 0.25f;

	public GraphNode()
	{
		this.neighbours = new GraphNode[6];
	}

	public override void _Ready()
	{
		this.highlighted = false;
		updateModel();
	}

	/// <summary>
	/// Aktualisiert das gerenderte Modell je nach Status.
	/// </summary>
	public void updateModel()
	{
		// sicher ist sicher
		if (this.GetChildren().Contains(tile)) this.RemoveChild(tile);

		// Remove old start flag if it exists
		if (startFlag != null && this.GetChildren().Contains(startFlag))
		{
			this.RemoveChild(startFlag);
			startFlag.QueueFree();
			startFlag = null;
		}

		// Remove old goal post if it exists
		if (goalPost != null && this.GetChildren().Contains(goalPost))
		{
			this.RemoveChild(goalPost);
			goalPost.QueueFree();
			goalPost = null;
		}

		// base-sprite (gras)
		this.tileBase = (Node3D)ResourceLoader.Load<PackedScene>("assets/testing/testtile00.tscn").Instantiate();
		this.AddChild(this.tileBase);

		GraphNode[] _neighbours = this.getNeighbours();

		switch (_neighbours.Length)
		{
			// if empty tile
			case 0:
				this.tile = loadAsset("testtile00");
				break;
			// if city
			case int n when n != 2:
				int c = _neighbours.Length < 2 ? 3 : 0;
				this.tile = loadAsset("city_0" + (_neighbours.Length + c) + "_" + "1");
				loadCityConnectionsAssets();
				break;
			// if street
			case 2:
				this.tile = getStreetTileAsset(_neighbours[0], _neighbours[1]);
				break;
		}

        if(isStartNode)
        {
            startFlag = (Node3D)ResourceLoader.Load<PackedScene>("res://assets/testing/startflag.tscn").Instantiate(); // TODO: add variations
            this.AddChild(startFlag);
        }
        if(isGoalNode)
        {
            goalPost = (Node3D)ResourceLoader.Load<PackedScene>("res://assets/testing/goalpost.tscn").Instantiate(); // TODO: add variations
            this.AddChild(goalPost);
        }
		this.AddChild(tile);
	}

	private Node3D getStreetTileAsset(GraphNode n1, GraphNode n2)
	{
		int from = (int)getNeighbourType(this, n1);
		int to = (int)getNeighbourType(this, n2);

		int diff = to - from;
		int variation = Globals.rand.Next(1, 3);

		if (diff > 3)
		{
			from = to;
			diff = 6 - diff;
		}

		Node3D street = loadAsset("street" + (int)this.streetType + "_0" + diff + "_" + "1");
		street.RotateY(Mathf.DegToRad(-60 * from));
		return street;
	}

	// Verbindungsstücke von der Stadt zur nächsten Straße laden
	private void loadCityConnectionsAssets()
	{
		GraphNode[] _neighbours = this.getNeighbours();

		if (_neighbours.Length != 1 && _neighbours.Length <= 2)
		{
			GD.PrintErr("Tried to add city connections assets on a non-city node!");
			return;
		}

		foreach (GraphNode n in _neighbours)
		{
			Node3D newConnection = (Node3D)ResourceLoader.Load<PackedScene>("assets/tiles/connection_00_" + (int)n.streetType + ".tscn").Instantiate();
			newConnection.RotateY(Mathf.DegToRad(((int)getNeighbourType(this, n) * -60.0f)));
			this.AddChild(newConnection);
		}
	}

    private Node3D loadAsset(string fileName)
    {
        Node3D scene = (Node3D)ResourceLoader.Load<PackedScene>("assets/tiles/" + fileName + ".tscn").Instantiate(); // TODO: add variations
        Node3D obj = new Node3D();
        obj.AddChild(scene);

        return obj;
    }

	// suche alle anliegenden Knoten mit genau 1 oder > 2 Nachbarn
	public GraphNode[] getNeighbouringCities(GraphNode[] visited)
	{
		List<GraphNode> heads = [];
		foreach (GraphNode head in this.getNeighbours()) heads.Add(head);
		List<GraphNode> visitedNodes = [this, .. heads, .. visited];
		int numCitiesFound = 0;

		while (numCitiesFound < heads.Count)
		{
			// c# bullshit
			List<GraphNode> newHeads = [];
			foreach (GraphNode head in heads)
			{
				newHeads.Add(head);
			}

			numCitiesFound = 0;
			foreach (GraphNode head in heads)
			{
				// win condition
				if (head.getNeighbours().Length == 1 || head.getNeighbours().Length > 2)
				{
					numCitiesFound++;
					continue;
				}

				foreach (GraphNode newHead in head.getNeighbours())
				{
					// if not alter nachbar => nur neue nachbarn angucken
					if (!visitedNodes.Contains(newHead))
					{
						visitedNodes.Add(newHead);
						newHeads.Remove(head);
						newHeads.Add(newHead);
					}
					// if bereits gefundener Knoten
					else if (newHead.getNeighbours().Length == 1 || newHead.getNeighbours().Length > 2)
					{
						newHeads.Remove(head);
					}
				}
			}

			heads = newHeads;
		}

		return heads.ToArray();
	}

	// Wir gehen davon aus, dass man weiß, dass es benachbarte Knoten gibt
	// Außerdem muss ein Nachbar als Startpunkt übergeben werden
	public GraphNode[] getPathToNeighbouringCity(GraphNode goalNode)
	{
		Dictionary<GraphNode, GraphNode> cameFrom = [];
		List<List<GraphNode>> paths = []; // bis zu 6 Pfade für jeden direkten Nachbarn von der Startnode
		foreach (GraphNode head in this.getNeighbours())
		{
			paths.Add([head]); // pfade initialisieren
			cameFrom.Add(head, this);
		}

		// nicht fragen bitte ~georg
		for (int i = 0; i < 1000; i++)
		{
			foreach (List<GraphNode> path in paths)
			{
				// win condition
				GraphNode head = path[path.Count - 1];
				if (head.Equals(goalNode))
				{
					// pfad rückwirkend erstellen
					List<GraphNode> finalPath = [];

					while (cameFrom.ContainsKey(head))
					{
						finalPath.Add(head);
						// GD.Print("final path has: " + head.getTilePosString());
						head = cameFrom[head];
					}
					return finalPath.ToArray();
				}

				// jeden Pfad um 1 vom start weg erweitern
				foreach (GraphNode newHead in head.getNeighbours())
				{
					if (!newHead.Equals(this) && !path.Contains(newHead))
					{
						path.Add(newHead);
						// Only add if not already in cameFrom (multiple paths can reach same node)
						if (cameFrom.TryAdd(newHead, head))
						{
							// GD.Print(newHead.getTilePosString() + " came from " + head.getTilePosString());
						}
					}
				}
			}
		}

		GD.PrintErr("Es wurde versucht den Pfad zwischen zwei nicht direkt miteinander verbundenen Nodes zu erhalten!");
		GD.PrintErr("startNode: " + this.getTilePosString());
		GD.PrintErr(" goalNode: " + goalNode.getTilePosString());

		return null;
	}

	public GraphNode[] getNeighbours()
	{
		return this.neighbours.Where(n => n != null).ToArray();
	}

	public static TileRelation getNeighbourType(GraphNode origin, GraphNode neighbour)
	{
		int dx = origin.tilePosX - neighbour.tilePosX;
		int dy = origin.tilePosY - neighbour.tilePosY;

		if (dx == -1 && dy == 1) return TileRelation.TOPRIGHT;
		if (dx == -1 && dy == 0) return TileRelation.RIGHT;
		if (dx == 0 && dy == -1) return TileRelation.BOTTOMRIGHT;
		if (dx == 1 && dy == -1) return TileRelation.BOTTOMLEFT;
		if (dx == 1 && dy == 0) return TileRelation.LEFT;
		if (dx == 0 && dy == 1) return TileRelation.TOPLEFT;

		return TileRelation.INVALID;
	}

	public Vector2I getNeighbourPos(int direction)
	{
		switch (direction)
		{
			case 0: return new Vector2I(this.tilePosX + 1, this.tilePosY - 1);
			case 1: return new Vector2I(this.tilePosX + 1, this.tilePosY);
			case 2: return new Vector2I(this.tilePosX, this.tilePosY + 1);
			case 3: return new Vector2I(this.tilePosX - 1, this.tilePosY + 1);
			case 4: return new Vector2I(this.tilePosX - 1, this.tilePosY);
			case 5: return new Vector2I(this.tilePosX, this.tilePosY - 1);
		}

		// shouldnt happen
		return new Vector2I(-1000, -1000);
	}

	public void addNeighbour(GraphNode newNeighbour)
	{
		if ((int)getNeighbourType(this, newNeighbour) == -1)
		{
			GD.Print("(" + newNeighbour.tilePosX + ", " + newNeighbour.tilePosY + ") ist kein gültiger Nachbarknoten für (" + this.tilePosX + ", " + this.tilePosY + ")");
			return;
		}

		this.neighbours[(int)getNeighbourType(this, newNeighbour)] = newNeighbour;
	}

	public void removeNeighbour(GraphNode neighbour)
	{
		for (int i = 0; i < this.neighbours.Length; i++)
		{
			GraphNode current = this.neighbours[i];
			if (current == null) continue;

			// GD.Print(this.getTilePosString() + ": current = " + current.getTilePosString() + ", origin=" + n.getTilePosString());
			if (current.Equals(neighbour))
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
		foreach (GraphNode n in this.getNeighbours())
		{
			n.removeNeighbour(this);
		}

		this.neighbours = new GraphNode[6];
	}

	public String getTilePosString()
	{
		return "(" + this.tilePosX + ", " + this.tilePosY + ")";
	}

	public void setTilePos(int tilePosX, int tilePosY)
	{
		this.tilePosX = tilePosX;
		this.tilePosY = tilePosY;
		this.Position = convertTilePosToPos(tilePosX, tilePosY);
	}

	// Konvertiert eine Neudinkel-Hexagon-Grid-Position zu einer Weltkoordinate
	// tilePosY 0.86 Offset wegen der Geometrie der Tiles
	// tilePosX 0.5 mal tilePosY Offset weil es Hexagone sind und die Zeilen versetzt sein muessen
	public static Vector3 convertTilePosToPos(int tilePosX, int tilePosY)
	{
		return new Vector3(tilePosX + 0.5f * tilePosY, 0.0f, tilePosY * 0.86f);
	}

	// Konvertiert eine Weltkoordinate in eine Neudinekl-Hexagon-Grid-Position
	public static Vector2I convertPosToTilePos(Vector3 pos)
	{
		float preTilePosY = pos.Z / 0.86f;
		float preTilePosX = pos.X - 0.5f * preTilePosY;

		int tilePosX = (int)Math.Round(preTilePosX);
		float tilePosXFracture = preTilePosX - (float)Math.Round(preTilePosX);
		if (tilePosXFracture > 0.5) tilePosX++;

		int tilePosY = (int)Math.Round(preTilePosY);
		float tilePosYFracture = preTilePosY - (float)Math.Round(preTilePosY);
		if (tilePosYFracture > 0.5) tilePosY++;

		return new Vector2I(tilePosX, tilePosY);
	}

	// hebt die Node visuell hervor, wenn highlight == true
	public void setHighlighted(bool highlight)
	{
		if (!highlighted && highlight)
		{
			this.Position = this.Position + Vector3.Up * this.highlightedYOffset;
		}
		else if (highlighted && !highlight)
		{
			this.Position = this.Position - Vector3.Up * this.highlightedYOffset;
		}

		this.highlighted = highlight;
	}



}
