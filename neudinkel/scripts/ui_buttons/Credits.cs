using Godot;
using System;

public partial class Credits : Node
{
	public override void _Ready()
	{
		 
	}

	public override void _Process(double delta) { }

	public void onBackButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/mainmenu.tscn");
	}
}
