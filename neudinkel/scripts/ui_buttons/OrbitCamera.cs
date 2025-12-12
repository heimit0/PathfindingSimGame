using Godot;
using System;

public partial class OrbitCamera : Camera3D
{
	[Export] public Vector3 Center = Vector3.Zero; 
	[Export] public float Distance = 40000f;          
	[Export] public float Height = 13f;             
	[Export] public float RotationSpeed = 0.05f;   

	private float _angle = 3.14f;

	public override void _Process(double delta)
	{
		_angle += RotationSpeed * (float)delta;
		float x = (Center.X + Distance) * Mathf.Cos(_angle);
		float z = (Center.Z + Distance) * Mathf.Sin(_angle);
		float y = Center.Y + Height;

		GlobalPosition = new Vector3(x, y, z);
		LookAt(Center, Vector3.Up);
	}
}
