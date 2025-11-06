using Godot;
using System;

public partial class Mainmenu : Control
{
	private void _on_baumodus_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/baumodus.tscn");
	}
	
	private void _on_pruefmodus_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/levelmenu.tscn");
	}
}
