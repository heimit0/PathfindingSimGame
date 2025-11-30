using Godot;
using System;

public partial class LevelmenuDFS : Control
{
	private void _on_back_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levelmenu.tscn");
	}

	private void _on_level_1_pressed()
	{
		Globals.nextLevel = "level1_dfs";
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

	private void _on_level_2_pressed()
	{
		Globals.nextLevel = "level2_dfs";
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}

	private void _on_level_3_pressed()
	{
		Globals.nextLevel = "level3_dfs";
		GetTree().ChangeSceneToFile("res://scenes/pruefungsmodus.tscn");
	}


}
