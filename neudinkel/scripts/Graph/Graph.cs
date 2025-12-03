using Godot;
using System;
using System.Text.Json;
using System.Collections;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

/*
	TODO:
		Debug Drawing - weitergeben an die Nodes/Tiles
*/

public enum GraphSearchAlgorithm
{
    A_STAR,
    DIJKSTRA,
    DFS
}

public partial class Graph : Node3D
{
    public List<GraphNode> nodes;

    public override void _Ready()
    {
        base._Ready();
        nodes = new List<GraphNode>();
    }

    public override void _Process(double delta) { }

    /// <summary>
    /// Iteriert durch alle Kindknoten und gibt
    /// die Node mit der angegebenen TilePos zurück.
    /// </summary>
    /// <returns> Die angefragte Node, oder null falls es sie nicht gibt. </returns>
    public GraphNode getNode(int tilePosX, int tilePosY)
    {
        foreach (object obj in this.GetChildren())
        {
            if (obj.GetType() == typeof(GraphNode))
            {
                GraphNode node = (GraphNode)obj;
                if (node.tilePosX == tilePosX && node.tilePosY == tilePosY)
                {
                    return node;
                }
            }
        }

        return null;
    }

    public GraphNode getNode(GraphNode node)
    {
        foreach (GraphNode n in this.nodes)
        {
            if (node.Equals(n)) return n;
        }

        return null;
    }

    public NewTileGraphNode getGhostNode(int x, int y)
    {
        foreach (object obj in this.GetChildren())
        {
            if (obj.GetType() == typeof(NewTileGraphNode))
            {
                NewTileGraphNode node = (NewTileGraphNode)obj;
                if (node.tilePosX == x && node.tilePosY == y)
                {
                    return node;
                }
            }
        }

        return null;
    }

    public GraphNode[] getNodes()
    {
        // List<GraphNode> result = new List<GraphNode>();
        // foreach (object node in this.GetChildren())
        // {
        //	if (node.GetType() == typeof(GraphNode))
        //		result.Add((GraphNode)node);
        // }
        // return result.ToArray();

        return this.nodes.ToArray();
    }

    /// <summary>
    /// Deserialisiert einen JSON-Objekt in ein gueltiges
    /// Graph-Element mit entsprechenden Kindknoten.
    /// </summary>
    /// <returns> false bei Erfolg, sonst true </returns>
    // TODO: Sehr unsicher, wir prüfen viel zu wenig auf Struktur und bei Casts
    public bool fromJsonObject(JsonObject json)
    {
        this.nodes = new List<GraphNode>();
        GameManager gm = this.GetParent<GameManager>();
        int startTile;
        int endTile;
        int i;

        // Struktur des Root testen
        if (!(json.ContainsKey("tiles") && json.ContainsKey("edges")))
        {
            GD.Print("fromJsonObject: Level-Objekt ist kaputt, es fehlen tiles und/oder edges :(");
            return true;
        }

        // Weiter testen und dabei gleich das Start- und Endtile setzen
        if (json.ContainsKey("startTile") && json.ContainsKey("endTile"))
        {
            startTile = (int)json["startTile"];
            endTile = (int)json["endTile"];
        }
        else
        {
            GD.Print("fromJsonObject: Es fehlt das Start- oder End-Tile!");
            return true;
        }

        // Kamera-Position setzen
        if (json.ContainsKey("startPos") && json["startPos"].AsArray().Count == 2)
        {
            JsonArray startPos = json["startPos"].AsArray();
            gm.EmitSignal(GameManager.SignalName.SetCameraPosition, new Vector2((float)startPos[0], (float)startPos[1]));
        }

        i = 0; // meh
        foreach (JsonObject n in json["tiles"].AsArray())
        {
            GraphNode node = new GraphNode();
            // wild dass C# das einfach so erlaubt...
            node.setTilePos((int)n["posX"], (int)n["posY"]);
            // abomination.
            node.streetType = (StreetType)(int)n["type"];
            this.nodes.Add(node);
            // sind wir ein start- bzw. endtile?
            if (i == startTile) gm.startNode = node;
            if (i == endTile) gm.goalNode = node;
            i++;
        }

        foreach (JsonObject e in json["edges"].AsArray())
        {
            GraphNode from = this.nodes[(int)e["from"]];
            GraphNode to = this.nodes[(int)e["to"]];

            if (from is null || to is null)
            {
                GD.Print("fromJsonObject: Falsche Referenz in einer Kante");
                return true;
            }

            from.addNeighbour(to);
            if ((bool)e["bi"] == true) to.addNeighbour(from);
        }

        foreach (GraphNode n in this.nodes)
            this.AddChild(n);

        initCoordsNodes();

        return false;
    }

    public void toJsonFile(string filename)
    {
        var saveFile = FileAccess.Open("res:///levels/" + filename + ".json", FileAccess.ModeFlags.Write);
        Camera camera = (Camera)GetViewport().GetCamera3D();
        BauGameManager gameManager = GetNode<BauGameManager>("/root/Node3D"); // TODO fix
        string output = "{\"name\":\"baumodus_save\",\"startPos\":[" + camera.Position.X.ToString(CultureInfo.InvariantCulture) + "," + camera.Position.Z.ToString(CultureInfo.InvariantCulture) + "],\"tiles\":[";
        int startNodeIndex = -1, goalNodeIndex = -1;
		int m = 0;

        // nodes
        foreach (GraphNode n in this.nodes)
        {
			if (n.Equals(gameManager.startNode)) startNodeIndex = m;
			if (n.Equals(gameManager.goalNode)) goalNodeIndex = m;

            output += "{\"posX\":" + n.tilePosX + ",\"posY\":" + n.tilePosY + ",\"type\":" + (int)n.streetType + "},";
			m++;
		}


        output = output.Substring(0, output.Length - 1); // remove last comma
        output += "],\"startTile\":" + startNodeIndex + ",\"endTile\":" + goalNodeIndex + ",\"edges\":[";
        bool hasEdges = false;

        // relations
        for (int i = 0; i < this.nodes.Count; i++)
        {
            GraphNode n = this.nodes[i];

            foreach (GraphNode neighbour in n.getNeighbours())
            {
                // dumme extra for loop, weil edges in unserem json mit indizes gespeichert werden aaaauugh
                for (int j = 0; j < this.nodes.Count; j++)
                {
                    if (this.nodes[j].Equals(neighbour))
                    {
                        output += "{\"from\":" + i + ",\"to\":" + j + ",\"bi\":true},";
                        hasEdges = true;
                    }
                }
            }
        }

        if (hasEdges) output = output.Substring(0, output.Length - 1); // remove last comma

        output += "]}";
        saveFile.StoreString(output);
        saveFile.Close();
    }

    // get Knoten
    public GraphNode[] getCities()
    {
        List<GraphNode> cities = new List<GraphNode>();

        foreach (GraphNode n in this.nodes)
        {
            if (n.getNeighbours().Length > 2 || n.getNeighbours().Length == 1) cities.Add(n);
        }

        return cities.ToArray();
    }

    public void addNode(GraphNode node)
    {
        this.nodes.Add(node); // should already be child of graph
    }

    public void addRelation(GraphNode node1, GraphNode node2)
    {
        try
        {
            getNode(node1).addNeighbour(getNode(node2));
            getNode(node2).addNeighbour(getNode(node1));
        }
        catch (NullReferenceException e)
        {
            GD.PrintErr("Tried to add relation in graph of a/two non-existing node/s!");
            GD.PrintErr(e.StackTrace);
        }
    }


    public void initCoordsNodes()
    {
        foreach (GraphNode n in getCities())
        {
            initCoordsNode(n);
        }

    }

    public void initCoordsNode(GraphNode n)
    {
        Label3D coordLabel = new Label3D();
        coordLabel.Text = "X: " + n.tilePosX + "  Y: " + n.tilePosY;
        coordLabel.Position = coordLabel.Position + new Vector3(0.0f, 0.3f, 0.3f);
        coordLabel.Name = "CoordsLabel";
        coordLabel.RenderPriority = 100;
        coordLabel.NoDepthTest = true;
        coordLabel.FontSize = 20;
        coordLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
        n.AddChild(coordLabel);
    }

}



