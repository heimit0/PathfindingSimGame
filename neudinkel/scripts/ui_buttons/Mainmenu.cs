using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;

public partial class Mainmenu : Control
{
	private CanvasLayer mainMenu;
	private CanvasLayer levelMenu;
	private CanvasLayer levelMenuZip;
	private Label algoName;
	private Godot.Collections.Array<Node> stars;

	public override void _Ready()
	{
		mainMenu = GetNode<CanvasLayer>("MainMenu");
		levelMenu = GetNode<CanvasLayer>("LevelMenu");
		levelMenuZip = GetNode<CanvasLayer>("LevelMenu_zip");
		algoName = GetNode<Label>("LevelMenu_zip/Label");
		stars = GetNode<HBoxContainer>("LevelMenu_zip/VBoxContainer/HBoxContainer").GetChildren();

		foreach (Node child in mainMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = true;

		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = false;
			
		foreach (Node child in levelMenuZip.GetChildren())
			if (child is CanvasItem item) item.Visible = false;
			//Loads random lvl from array Backdrops
		String[] backdrops = {"baumodus_save", "pruef_level1_astar","level3_dijkstra","level1_dfs"}; //kann noch erweitert werden
		Random r = new();
		loadRandomBackdrop();
	
	}

	public void loadRandomBackdrop()
	{
		//Loads random lvl from array Backdrops
		String[] backdrops = getLevelFiles().ToArray(); //kann noch erweitert werden
		Random r = new();
		loadBackdrop(backdrops[Mathf.PosMod(r.Next(),backdrops.Length)]);
	}


	public List<string> getLevelFiles()
{
	var levelFiles = new List<string>();
	var dir = DirAccess.Open("res://levels");
	
	if (dir != null)
	{
		dir.ListDirBegin();
		string fileName = dir.GetNext();
		
		while (fileName != "")
		{
			if (!dir.CurrentIsDir() && fileName.EndsWith(".json"))
			{
				levelFiles.Add(fileName.Substring(0,fileName.Length-5));
				GD.Print(fileName);
			}
			fileName = dir.GetNext();
		}
		dir.ListDirEnd();
	}
	
	return levelFiles;
}
	
	
	private void loadBackdrop(string levelFile)
	{
		

		Graph levelGraph;
		JsonObject json;
		FileAccess file;

		if (FileAccess.FileExists("res:///levels/" + levelFile + ".json"))
		{
			GD.Print(" loadBackdrop: Loading level: " + levelFile);
			file = FileAccess.Open("res:///levels/" + levelFile + ".json", FileAccess.ModeFlags.Read);
		}
		else
		{
			GD.PrintErr("Tried to open file: " + levelFile + ".json");
			file = FileAccess.Open("res:///levels/empty.json", FileAccess.ModeFlags.Read);
		}

		levelGraph = new Graph();
		json = JsonNode.Parse(file.GetAsText()).AsObject();
		this.AddChild(levelGraph);
		levelGraph.fromJsonObject(json);

			// center of the level
	float minX = float.MaxValue, maxX = float.MinValue;
	float minZ = float.MaxValue, maxZ = float.MinValue;
	foreach (GraphNode node in levelGraph.nodes)
	{
		Vector3 pos = node.Position;
		minX = Mathf.Min(minX, pos.X);
		maxX = Mathf.Max(maxX, pos.X);
		minZ = Mathf.Min(minZ, pos.Z);
		maxZ = Mathf.Max(maxZ, pos.Z);
	}
	Vector3 center = new Vector3((minX + maxX) / 2f, 0, (minZ + maxZ) / 2f);

		
		OrbitCamera orbitCam = GetNode<OrbitCamera>("Camera3D"); 
		if (orbitCam != null)
		{
			orbitCam.Center = center;
		}

		setCameraDynamic(levelGraph,orbitCam);
	}


	public void setCameraDynamic( Graph g, OrbitCamera c)
	{
		float smalsize = Mathf.Sqrt(g.getNodes().Length)+5;
		c.setDistance(smalsize);
		c.Height=smalsize/2;
	}
	
	
	//-------------- main menu ---------------
	private void _on_baumodus_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/baumodus.tscn");
	}

	private void _on_pruefmodus_pressed()
	{
		foreach (Node child in mainMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}
	
	//-------------- level menu --------------
	private void _on_backButtonLevelMenu_pressed()
	{
		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in mainMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}
	
	private void _on_algorithm_selected(int algo)
	{
		Globals.pruefungsAlgorithmus = (GraphSearchAlgorithm) algo;

		// Überschrift setzen
		if (algo == (int) GraphSearchAlgorithm.DFS)
			algoName.Text = "Depth First Search";
		else if (algo == (int) GraphSearchAlgorithm.DIJKSTRA)
			algoName.Text = "Dijkstra";
		else if (algo == (int) GraphSearchAlgorithm.A_STAR)
			algoName.Text = "A*";

			foreach (Node star in stars)
				((NeudinkelStarIndicator)star).update();

		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenuZip.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}

	//---------- level menu level selection ----------
	private void _on_back_button_zip_pressed()
	{
		foreach (Node child in levelMenuZip.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}

	private void _on_level_selected(int level)
	{
		String _algoName = "";

		if (Globals.pruefungsAlgorithmus == GraphSearchAlgorithm.DFS)
			_algoName = "dfs";
		else if (Globals.pruefungsAlgorithmus == GraphSearchAlgorithm.DIJKSTRA)
			_algoName = "dijkstra";
		else if (Globals.pruefungsAlgorithmus == GraphSearchAlgorithm.A_STAR)
			_algoName = "astar";
		
		Globals.stufe = level;
		Globals.nextLevel = "pruef_level" + level + "_" + _algoName;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

}
