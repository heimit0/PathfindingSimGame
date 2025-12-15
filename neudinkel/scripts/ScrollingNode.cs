using Godot;
using System;

public partial class ScrollingNode : Node2D
{
	[Export]
	public float speed = 20.0f;

	public override void _Ready() { }

	public override void _Process(double delta)
	{
		Position -= new Vector2(0.0f, speed * (float) delta);
	}
}
