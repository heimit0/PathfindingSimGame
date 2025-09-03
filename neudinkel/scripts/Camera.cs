using Godot;
using System;

public partial class Camera : Camera3D
{
	public override void _Ready() { }
	
	public override void _Process(double delta) {
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		Vector2 mousePos = GetViewport().GetMousePosition() / screenSize;
		
		if (mousePos.X > 0.9)
			this.Position += new Vector3(1.0f * (float)delta, 0.0f, 0.0f);
		else if (mousePos.X < 0.1)
			this.Position += new Vector3(-1.0f * (float)delta, 0.0f, 0.0f);
		else if (mousePos.Y > 0.9)
			this.Position += new Vector3(0.0f, 0.0f, 1.0f * (float)delta);
		else if (mousePos.Y < 0.1)
			this.Position += new Vector3(0.0f, 0.0f, -1.0f * (float)delta);
	}
}
