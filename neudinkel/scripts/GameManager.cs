using Godot;
using System;

public enum GamePhase
{
	BUILDING,
	SIMULATION,
	EXAM
}

public partial class GameManager : Node3D
{
	public GamePhase gamePhase { get; set; }

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
