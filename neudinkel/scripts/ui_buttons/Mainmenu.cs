using Godot;
using System;

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
	}

	//-------------- main menu ---------------
	private void _on_baumodus_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/baumodus.tscn");
	}

	private void _on_pruefmodus_pressed(){
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
