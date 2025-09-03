using Godot;
using System;

public partial class Camera : Camera3D
{
	
	[Export]
	public float speed;
	
	public override void _Ready() { }
	
	public override void _Process(double delta) {
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		Vector2 mousePos = GetViewport().GetMousePosition() / screenSize;
		
		if (mousePos.X > 0.9)
			this.Position += new Vector3(speed * (float)delta, 0.0f, 0.0f);
		if (mousePos.X < 0.1)
			this.Position += new Vector3(-speed * (float)delta, 0.0f, 0.0f);
		if (mousePos.Y > 0.9)
			this.Position += new Vector3(0.0f, 0.0f, speed * (float)delta);
		if (mousePos.Y < 0.1)
			this.Position += new Vector3(0.0f, 0.0f, -speed * (float)delta);
	}
	
}
