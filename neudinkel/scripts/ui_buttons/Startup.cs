using Godot;
using System;

public partial class Startup : Node
{

	public override void _Ready()
	{
		ColorRect cr = GetNode<ColorRect>("Rotfront");
		AnimationPlayer ap = GetNode<AnimationPlayer>("ADLR");

		ap.AnimationFinished += OnFadeFinished;
		ap.Play("fadetoblack");

		Globals.playBackgroundMusic("res:///assets/sounddesign/intro.wav");
	}

	public override void _Process(double delta) { }

	private void OnFadeFinished(StringName name)
	{
		GetTree().ChangeSceneToFile("res://scenes/mainmenu.tscn");
	}

}
