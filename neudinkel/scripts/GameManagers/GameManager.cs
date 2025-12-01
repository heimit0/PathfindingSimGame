using Godot;
using System;
using System.Collections;
using System.Globalization;
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
	public delegate void setGoalNodeEventHandler(GraphNode node);

	[Signal]
	public delegate void SetCameraPositionEventHandler(Vector2 pos);

	public override void _Ready()
	{
		prepScene("empty");

		// signale
		ToggleGamePhase += onToggleGamePhase;
		SetStartNode += onSetStartNode;
		setGoalNode += onSetGoalNode;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void prepScene(string levelName) 
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

		loadLevel(levelName);
	}

	/// <summary>
	/// Lädt das angegebene Level.
	/// loadLevel("test") lädt .das Level levels/test.json.
	/// </summary>
	public void loadLevel(string levelName)
	{
		JsonObject json;
		using var file = FileAccess.Open("res:///levels/" + levelName + ".json", FileAccess.ModeFlags.Read);

		this.graph = new Graph();
		json = JsonNode.Parse(file.GetAsText()).AsObject();
		this.AddChild(this.graph);
		this.graph.fromJsonObject(json);

		onSetStartNode(this.graph.getNode(0, 0));
		onSetGoalNode(this.graph.getNode(2, 2));
	}

	public void onToggleGamePhase(bool isNewPhaseSimulationPhase)
	{
		// TODO change scene, when changing modi

		// if Baumodus => Simulationsmodus
		// if ((int)this.gamePhase == 0 && isNewPhaseSimulationPhase)
		// {
		//     EmitSignal(SignalName.InitOrChangeAlgorithm, (int)ENeudinkelAlgorithm.ASTAR);
		// }

		// this.gamePhase = isNewPhaseSimulationPhase ? GamePhase.SIMULATION : GamePhase.BUILDING;
		// GD.Print("GamePhase changed to: " + this.gamePhase.ToString());
	}

	public void onSetStartNode(GraphNode node)
	{
		this.startNode = node;
	}

	public void onSetGoalNode(GraphNode node)
	{
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
