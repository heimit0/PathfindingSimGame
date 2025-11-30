using Godot;
using System;

public partial class LevelmenuDijkstra : Control
{
	private void _on_back_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levelmenu.tscn");
	}
	private void _on_level_1_pressed()
	{
		Globals.nextLevel = "level1_dijkstra";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DIJKSTRA;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

	private void _on_level_2_pressed()
	{
		Globals.nextLevel = "level2_dijkstra";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DIJKSTRA;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

	private void _on_level_3_pressed()
	{
		Globals.nextLevel = "level3_dijkstra";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.DIJKSTRA;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}


}
