using Godot;
using System;

public partial class LevelmenuAStar : Control
{
	private void _on_back_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levelmenu.tscn");
	}

	private void _on_level_1_pressed()
	{
		Globals.nextLevel = "level1_astar";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.A_STAR;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

	private void _on_level_2_pressed()
	{
		Globals.nextLevel = "level2_astar";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.A_STAR;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

	private void _on_level_3_pressed()
	{
		Globals.nextLevel = "level3_astar";
		Globals.pruefungsAlgorithmus = GraphSearchAlgorithm.A_STAR;
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

}
