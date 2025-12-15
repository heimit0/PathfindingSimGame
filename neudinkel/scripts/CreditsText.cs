using Godot;
using System;

public partial class CreditsText : RichTextLabel
{

	[Export]
	public float speed = 1.0f;

	public override void _Ready() { }

	public override void _Process(double delta)
	{
		Vector2 pos = this.GetPosition();
		this.SetPosition(pos - new Vector2(0.0f, speed * (float) delta), false);
	}
}
