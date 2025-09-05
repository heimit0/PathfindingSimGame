using Godot;
using System;

public enum GamePhase
{
	BUILDING,
	SIMULATION,
	EXAM
}

/// <summary>
/// Der GameManager ist die RootNode der Spielszene.
/// Das Objekt uebersieht alle Aktionen und greift wenn noetig ein
/// </summary>
public partial class GameManager : Node3D
{
	public GamePhase gamePhase { get; set; }

	// TODO(hsimosse oder gfriedr3): Welche Aufgaben muss der GameManager übernehmen?
	// TODO(hsimosse oder gfriedr3): Welche Signale muss der GM empfangen können?

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}
