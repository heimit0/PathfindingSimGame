using Godot;
using System;
using System.Text.Json.Nodes;

/*
	TODO:
		Ablaufsteuerung - Was passiert wann wie?
		UI-Koordinierung - Signale etc.
*/

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
		loadLevel("test");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	
	/// <summary>
	/// Lädt das angegebene Level.
	/// loadLevel("test") lädt das Level levels/test.json.
	/// </summary>
	public void loadLevel(string levelName)
	{
		Graph g;
		JsonObject json;
		using var file = FileAccess.Open("res:///levels/" + levelName + ".json", FileAccess.ModeFlags.Read);

		g = new Graph();
		json = JsonNode.Parse(file.GetAsText()).AsObject();
		g.fromJsonObject(json);
		this.AddChild(g);
	}
}
