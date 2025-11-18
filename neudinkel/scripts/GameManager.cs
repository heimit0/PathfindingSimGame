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
/// Der GameManager ist der Parent Node der Spielszene.
/// Das Objekt uebersieht alle Aktionen und greift wenn noetig ein
/// </summary>
public partial class GameManager : Node3D
{
	public GamePhase gamePhase { get; set; }
	public Graph graph { get; set; }

	// TODO(hsimosse oder gfriedr3): Welche Aufgaben muss der GameManager übernehmen?
	// TODO(hsimosse oder gfriedr3): Welche Signale muss der GM empfangen können?

	public override void _Ready()
	{
		Camera cam;
		
		base._Ready();
		cam = new Camera();
		this.AddChild(cam);
		cam.Position = new Vector3(0.0f, 5.0f, 0.0f);
		cam.Rotation = new Vector3(Mathf.DegToRad(-45.0f), 0, 0);
		cam.MakeCurrent();
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
		JsonObject json;
		using var file = FileAccess.Open("res:///levels/" + levelName + ".json", FileAccess.ModeFlags.Read);

		this.graph = new Graph();
		json = JsonNode.Parse(file.GetAsText()).AsObject();
		this.graph.fromJsonObject(json);
		this.AddChild(this.graph);
	}

	// ruft die visualize() funktion im jeweiligen Algorithmus auf,
	// welche die nötigen Tiles im Graphen anspricht
	public void highlightGraph<T>(T algorithm) where T : INeudinkelAlgorithm {
		algorithm.visualize(this.graph);
	}
}
