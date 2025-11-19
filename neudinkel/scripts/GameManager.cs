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
    private GamePhase gamePhase;
    public Graph graph { get; set; }

    public int automaticSimulationSpeed = 1; // für simulationsmodus

    public NeudinkelAlgorithm currentAlgorithm { get; set; }

    // TODO(hsimosse oder gfriedr3): Welche Aufgaben muss der GameManager übernehmen?
    // TODO(hsimosse oder gfriedr3): Welche Signale muss der GM empfangen können?

    [Signal]
    public delegate void InitOrChangeAlgorithmEventHandler(int algorithm); // algorithmus setzen

    [Signal]
    public delegate void DoNextSearchStepEventHandler(); // nächsten algorithmus-schritt

    [Signal]
    public delegate void AlgorithmFinishedEventHandler(); // weg gefunden, weg anzeigen

    [Signal]
    public delegate void ChangeGamePhaseEventHandler(GamePhase newGamePhase);

    [Signal]
    public delegate void UpdateInternalGraphEventHandler(Graph graph); // für baumodus

    [Signal]
    public delegate void SetAutomaticSimulationSpeedEventHandler(int newSpeed); // slider für speed im simulations-modus

    public override void _Ready()
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
		
		loadLevel("test");

        // signale
        DoNextSearchStep += onDoNextSearchStep;
        InitOrChangeAlgorithm += onInitOrChangeAlgorithm;
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

	// ruft gleichnamige Methode in jedem NeudinkelAlgorithm auf
	// gibt den nächsten index im schritt-array des algorithmus' zurück (für UI)
	public void onDoNextSearchStep()
	{
        int nextStep = this.currentAlgorithm.doSearchStep();
    }

	public void onChangeGamePhase(GamePhase newGamePhase)
	{
        this.gamePhase = newGamePhase;

		// if gamePhase: Baumodus -> SimulationsModus ==> initAlgorithm
    }

	public void onInitOrChangeAlgorithm(int algorithm)
	{
        
    }

	// ruft die visualize() funktion im jeweiligen Algorithmus auf,
	// welche die nötigen Tiles im Graphen anspricht
	public void highlightGraph<T>(T algorithm) where T : INeudinkelAlgorithm {
		algorithm.visualize(this.graph);
	}

	// custom getter, weil c# dumm ist
	public GamePhase getGamePhase()
	{
        return this.gamePhase;
    }
}
