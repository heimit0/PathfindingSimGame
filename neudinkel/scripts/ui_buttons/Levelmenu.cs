using Godot;
using System;

public partial class Levelmenu : Control
{
	private void _on_dijkstra_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levelmenu_dijkstra.tscn");
	}
	
	private void _on_a_star_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levelmenu_aStar.tscn");
	}
	
	private void _on_dfs_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levelmenu_dfs.tscn");
	}
	
	private void _on_back_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/mainmenu.tscn");
	}
}
