using Godot;
using System;
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
		
		LoadLevel("res://levels/level3_astar");
	}
	
	private void LoadLevel(string levelFile)
{
	var jsonFile = FileAccess.Open(levelFile + ".json", FileAccess.ModeFlags.Read);
	string jsonText = jsonFile.GetAsText();
	jsonFile.Close();

	JsonNode rootNode = JsonNode.Parse(jsonText);
	if (rootNode == null)
	{
		GD.PrintErr("Failed to parse level JSON!");
		return;
	}

	JsonObject json = rootNode.AsObject();

	// Create a new Graph to hold the visual tiles
	Graph levelGraph = new Graph();
	AddChild(levelGraph);

	float minX = float.MaxValue, maxX = float.MinValue;
	float minZ = float.MaxValue, maxZ = float.MinValue;

	// Instantiate nodes
	foreach (JsonObject n in json["tiles"].AsArray())
	{
		GraphNode node = new GraphNode();
		int x = (int)n["posX"];
		int y = (int)n["posY"];
		node.setTilePos(x, y);
		node.streetType = (StreetType)(int)n["type"];
		node.updateModel();
		levelGraph.AddChild(node);

		// Track bounds for center calculation
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
		Globals.nextLevel = "level" + level + "_" + _algoName;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

}
