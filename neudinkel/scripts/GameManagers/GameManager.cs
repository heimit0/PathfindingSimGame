using Godot;
using System;
using System.Collections;
using System.Globalization;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

/*
	TODO:
		Ablaufsteuerung - Was passiert wann wie?
		UI-Koordinierung - Signale etc.
*/

public enum GamePhase
{
	BUILDING = 0,
	SIMULATION = 1,
	EXAM = 2
}

public enum SimulationModus
{
	STEP_BY_STEP,
	AUTOMATIC
}

public enum ENeudinkelAlgorithm
{
	ASTAR = 0,
	DIJKSTRA = 1,
	DFS = 2
}

/// <summary>
/// Der GameManager ist der Parent Node der Spielszene.
/// Das Objekt uebersieht alle Aktionen und greift wenn noetig ein
/// </summary>
public partial class GameManager : Node3D
{
	public Graph graph { get; set; }

	public GraphNode startNode { get; set; }
	public GraphNode goalNode { get; set; }

	[Signal]
	public delegate void ToggleGamePhaseEventHandler(bool isNewPhaseSimulationPhase);

	[Signal]
	public delegate void UpdateInternalGraphEventHandler(Graph graph); // für baumodus

	[Signal]
	public delegate void SetStartNodeEventHandler(GraphNode node);

	[Signal]
	public delegate void SetGoalNodeEventHandler(GraphNode node);

	[Signal]
	public delegate void SetCameraPositionEventHandler(Vector2 pos);

	public override void _Ready()
	{
		prepScene("empty");

		// signale
		SetStartNode += onSetStartNode;
		SetGoalNode += onSetGoalNode;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	/* Funny hack with optional arguments because we literally have to turn this in tomorrow */
	public void prepScene(string levelName, bool user = false)
	{
		Camera cam;
		DirectionalLight3D light;

		base._Ready();
		cam = new Camera();
		this.AddChild(cam);
		cam.Position = new Vector3(0.0f, 5.0f, 0.0f);
		cam.Rotation = new Vector3(Mathf.DegToRad(-45.0f), 0, 0);
		cam.MakeCurrent();
		light = new DirectionalLight3D();
		this.AddChild(light);
		light.Position = new Vector3(0.0f, 10.0f, 0.0f);
		light.Rotation = new Vector3(Mathf.DegToRad(-45.0f), Mathf.DegToRad(45.0f), 0);

		if (!user)
			loadLevel(levelName);
		else
			loadUserLevel(levelName);
	}

	/// <summary>
	/// Lädt das angegebene Level.
	/// loadLevel("test") lädt .das Level levels/test.json.
	/// </summary>
	public void loadLevel(string levelName)
	{
		loadLevelInternal("res:///levels/" + levelName + ".json");
	}

	/// <summary>
	/// Wie loadLevel, aber laedt aus dem Ordner für Nutzer-Level.
	/// </summary>
	public void loadUserLevel(string levelName)
	{
		loadLevelInternal("user:///levels/" + levelName + ".json");
	}

	public void loadLevelInternal(string path)
	{
		JsonObject json;
		FileAccess file;

		if (FileAccess.FileExists(path))
		{
			GD.Print("Loading level: " + path);
			file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		}
		else
		{
			GD.PrintErr("Tried to open file: " + path);
			file = FileAccess.Open("res:///levels/empty.json", FileAccess.ModeFlags.Read);
		}

		this.graph = new Graph();
		json = JsonNode.Parse(file.GetAsText()).AsObject();
		this.AddChild(this.graph);
		this.graph.fromJsonObject(json);
	}

	public void onSetStartNode(GraphNode node)
	{
		GD.Print("onSetStartNode");
		this.startNode = node;
	}

	public void onSetGoalNode(GraphNode node)
	{
		GD.Print("onSetGoalNode");
		this.goalNode = node;
	}

	public static GamePhase IntToGamePhase(int value)
	{
		if (Enum.IsDefined(typeof(GamePhase), value))
		{
			return (GamePhase)value;
		}

		throw new ArgumentException($"Value {value} is not a valid Status");
	}
}
