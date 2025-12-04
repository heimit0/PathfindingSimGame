using Godot;
using System;

public partial class Mainmenu : Control
{
	private CanvasLayer mainMenu;
	private CanvasLayer levelMenu;
	private CanvasLayer levelMenuDijkstra;
	private CanvasLayer levelMenuAStar;
	private CanvasLayer levelMenuDfs;

	public override void _Ready()
	{
		mainMenu = GetNode<CanvasLayer>("MainMenu");
		levelMenu = GetNode<CanvasLayer>("LevelMenu");
		levelMenuDijkstra = GetNode<CanvasLayer>("LevelMenuDijkstra");
		levelMenuAStar = GetNode<CanvasLayer>("LevelMenuAStar");
		levelMenuDfs = GetNode<CanvasLayer>("LevelMenuDFS");
	
		foreach (Node child in mainMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = true;

		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = false;
			
		foreach (Node child in levelMenuDijkstra.GetChildren())
			if (child is CanvasItem item) item.Visible = false;
		
		foreach (Node child in levelMenuAStar.GetChildren())
			if (child is CanvasItem item) item.Visible = false;
		
		foreach (Node child in levelMenuDfs.GetChildren())
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
	
	private void _on_levelMenuDijkstra_pressed()
	{
		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenuDijkstra.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}
	
	private void _on_levelMenuAStar_pressed()
	{
		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenuAStar.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}
	
	private void _on_levelMenuDFS_pressed()
	{
		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenuDfs.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}
	
	//---------- level menu Dijkstra ----------
	private void _on_backButtonDijkstra_pressed()
	{
		foreach (Node child in levelMenuDijkstra.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}
	
	private void _on_dijkstraLevel1_pressed()
	{
		Globals.nextLevel = "level1_dijkstra";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DIJKSTRA;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}
	
	private void _on_dijkstraLevel2_pressed()
	{
		Globals.nextLevel = "level2_dijkstra";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DIJKSTRA;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}
	
	private void _on_dijkstraLevel3_pressed()
	{
		Globals.nextLevel = "level3_dijkstra";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DIJKSTRA;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}
	
	//------------ level menu AStar -----------
	private void _on_backButtonAStar_pressed()
	{
		foreach (Node child in levelMenuAStar.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}
	
	private void _on_astarLevel1_pressed()
	{
		Globals.nextLevel = "level1_astar";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.A_STAR;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}
	
	private void _on_astarLevel2_pressed()
	{
		Globals.nextLevel = "level2_astar";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.A_STAR;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}
	
	private void _on_astarLevel3_pressed()
	{
		Globals.nextLevel = "level3_astar";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.A_STAR;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");	
	}
	
	//------------- level menu DFS ------------
	private void _on_backButtonDFS_pressed()
	{
				foreach (Node child in levelMenuDfs.GetChildren())
			if (child is CanvasItem item) item.Visible = false;

		foreach (Node child in levelMenu.GetChildren())
			if (child is CanvasItem item) item.Visible = true;
	}
	
	private void _on_dfsLevel1_pressed()
	{
		Globals.nextLevel = "level1_dfs";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DFS;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");	
	}
	
	private void _on_dfsLevel2_pressed()
	{
		Globals.nextLevel = "level1_dfs";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DFS;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");	
	}
	
	private void _on_dfsLevel3_pressed()
	{
		Globals.nextLevel = "level1_dfs";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DFS;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");	
	}
}
