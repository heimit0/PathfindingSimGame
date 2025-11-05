using Godot;
using System;

public partial class LevelmenuDFS : Control
{
	private void _on_back_button_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levelmenu.tscn");
	}
}
