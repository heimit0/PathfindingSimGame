using Godot;
using System;
using System.Collections;
using System.Data;

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
	private GraphEdge[] neighbors; // 0 obenrechts, 1 rechts, 2 rechtsunten, 3 linksunten, 4 links, 5 linksoben
	private Node3D tile;

	public override void _Ready()
	{
		base._Ready();
		neighbors = new GraphEdge[6];
		updateModel();
	}

	public override void _Process(double delta) { }

	/// <summary>
	/// Aktualisiert das gerenderte Modell je nach Status.
	/// </summary>
	public void updateModel()
	{
		// TODO(hsimosse): Der GraphNode sollte als Kindobjekt nen Model haben.
		// Wir ersetzen hier das Modell, je nach Zustand
		if (this.GetChildren().Contains(tile)){
			this.RemoveChild(tile);
		}
		tile = new Node3D();
		int nrconnections = 0;
		ArrayList positionConnections = new ArrayList();
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
				tile = getStreetTile((int)positionConnections[0], (int)positionConnections[1]);
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

	/// <summary>
	/// Fuegt eine Edge als Nachbar hinzu.
	/// Erinnerung: Edge ebenfalls updaten!
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
		position.Y = tilePosY * 0.86f;
		// Und hier weil es Hexagone sind und die Zeilen versetzt sein muessen
		position.X = tilePosX;
		if ( tilePosY % 2 == 0 )
			position.X += 0.5f;
		this.tilePosX = tilePosX;
		this.tilePosY = tilePosY;
		this.Position = position;
	}
	
}
