using Godot;
using System;
using System.Collections.Generic;
using System.Data;

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
    AUTOBAHN = 1,
    LANSTRASSE = 2,
    FELDWEG = 3
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

    /// <summary>
    /// zeigt ob node gehighlighted ist oder nich
    /// </summary>
    private bool highlighted = false;

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
        int diff = to - from;
        int variation = Globals.rand.Next(1, 3);

        if (diff > 3)
        {
            from = to;
            diff = 6 - diff;
        }

        var scene = ResourceLoader.Load<PackedScene>("assets/tiles/street1_0" + diff + "_" + variation + ".tscn"); // TODO: Adjust for type
        var streetModell = scene.Instantiate();
        Node3D street = new();
        street.AddChild(streetModell);
        street.RotateY(Mathf.DegToRad(-60 * from));
        return street;
    }

    private Node3D getCityTile(int size)
    {
        var scene = ResourceLoader.Load<PackedScene>("assets/testing/testtile_city03" /**+ size**/ + ".tscn"); //TODO replace with actual city
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
        if ((int)getNeighbourType(this, a) != -1)
        {
            this.neighbors[(int)getNeighbourType(this, a)] = a;
        }
        else
            GD.Print("(" + a.getTilePosX() + ", " + a.getTilePosY() + ") ist kein gültiger Nachbarknoten für (" + this.tilePosX + ", " + this.tilePosY + ")");
    }

    // gibt nur die existierenden Nachbarn mit der TileRelation zurück
    // für Algorithmen
    public Dictionary<GraphNode, int> getNeighborsUnsorted()
    {
        Dictionary<GraphNode, int> _neighbors = new Dictionary<GraphNode, int>();

        for (int i = 0; i < this.neighbors.Length; i++)
        {
            if (this.neighbors[i] != null)
            {
                _neighbors.Add(this.neighbors[i], i);
            }
        }

        return _neighbors;
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
        this.streetType = t;
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
        int tilePosX = (int)Math.Truncate(preTilePosX);
        float tilePosXFracture = preTilePosX - (float)Math.Truncate(preTilePosX);
        if (tilePosXFracture > 0.5)
        {
            tilePosX++;
        }
        int tilePosY = (int)Math.Truncate(preTilePosY);
        float tilePosYFracture = preTilePosY - (float)Math.Truncate(preTilePosY);
        if (tilePosYFracture > 0.5)
        {
            tilePosY++;
        }

        return new int[] { tilePosX, tilePosY };
    }

    public void toggleHightlight(bool highlight)
    {
        this.highlighted = highlight;
        Vector3 pos = this.Position;
        Vector3 relPos = new Vector3(0.0f, -0.5f, 0.0f);
        if (highlighted)
        {
            this.Position = pos + relPos;
        }
        else
        {
            this.Position = pos - relPos;
        }
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

}
