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
    private GamePhase gamePhase;
    public Graph graph { get; set; }

    public GraphNode startNode { get; set; }
    public GraphNode goalNode { get; set; }

	[Export]
    public float automaticSimulationSpeed = 1; // für simulationsmodus
    public bool isSimulationAutomatic { get; set; }

    public NeudinkelAlgorithm currentAlgorithm { get; set; }

    // TODO(hsimosse oder gfriedr3): Welche Aufgaben muss der GameManager übernehmen?
    // TODO(hsimosse oder gfriedr3): Welche Signale muss der GM empfangen können?

    [Signal]
    public delegate void InitOrChangeAlgorithmEventHandler(int algorithm); // algorithmus setzen
                                                                           // usage: EmitSignal(GameManager.InitOrChangeAlgorithm, (int) ENeudinkelAlgorithm.ASTAR);

    [Signal]
    public delegate void DoNextSearchStepEventHandler(); // nächsten algorithmus-schritt

    [Signal]
    public delegate void UpdateUIPseudoCodeEventHandler(int currentStep); // pseudo code anzeigen

    [Signal]
    public delegate void AlgorithmFinishedEventHandler(); // weg gefunden, weg anzeigen

    [Signal]
    public delegate void ToggleGamePhaseEventHandler(bool isNewPhaseSimulationPhase);

    [Signal]
    public delegate void UpdateInternalGraphEventHandler(Graph graph); // für baumodus

    [Signal]
    public delegate void SetAutomaticSimulationSpeedEventHandler(int newSpeed); // slider für speed im simulations-modus

    [Signal]
    public delegate void SetStartNodeEventHandler(GraphNode node);

    [Signal]
    public delegate void setGoalNodeEventHandler(GraphNode node);

    [Signal]
    public delegate void setIsSimulationAutomaticEventHandler(bool isSimulationAutomatic);

    [Signal]
    public delegate void SetCameraPositionEventHandler(Vector2 pos);

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
        ToggleGamePhase += onToggleGamePhase;
        SetStartNode += onSetStartNode;
        setGoalNode += onSetGoalNode;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
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
        onSetGoalNode(this.graph.getNode(3, 3));
    }

    // ruft gleichnamige Methode in jedem NeudinkelAlgorithm auf
    // gibt den nächsten index im schritt-array des algorithmus' zurück (für UI)
    public void onDoNextSearchStep()
    {
        if (this.isSimulationAutomatic) GD.Print("isSimulationAutomatic is now: false");
        this.isSimulationAutomatic = false;

        doNextSearchStepHelper();

        // EmitSignal(SignalName.UpdateUIPseudoCode, nextStep);
    }

	// NICHT DAS SIGNAL, extra funktion für abstraktion
	public void doNextSearchStepHelper()
	{
		if (this.currentAlgorithm.currentStep == -1)
		{
            if (this.currentAlgorithm is NeudinkelAlgorithm)
            {
                ((NeudinkelAlgorithm)this.currentAlgorithm).visualize(this.graph);
            }

            return;
        }

        GD.Print("Executing step: " + this.currentAlgorithm.currentStep);
        int nextStep = this.currentAlgorithm.doSearchStep();
	}

    public void onToggleGamePhase(bool isNewPhaseSimulationPhase)
    {
        // if Baumodus => Simulationsmodus
        if ((int)this.gamePhase == 0 && isNewPhaseSimulationPhase)
        {
            EmitSignal(SignalName.InitOrChangeAlgorithm, (int)ENeudinkelAlgorithm.ASTAR);
        }

        this.gamePhase = isNewPhaseSimulationPhase ? GamePhase.SIMULATION : GamePhase.BUILDING;
        GD.Print("GamePhase changed to: " + this.gamePhase.ToString());
    }

    // start immer alles neu
    // int algorithm == -1 => gleichen Algorithmus nochmal, i.e. nur neustarten
    public void onInitOrChangeAlgorithm(int algorithm)
    {
        // gleichen algorithmus nochmal, nur neustarten
        if (algorithm == -1)
        {
            if (this.currentAlgorithm is AlgorithmAStar)
            {
                this.currentAlgorithm = new AlgorithmAStar(this.graph, this.startNode, this.goalNode);
            }
            else if (this.currentAlgorithm is AlgorithmDijkstra)
            {
                this.currentAlgorithm = new AlgorithmDijkstra(this.graph, this.startNode, this.goalNode);
            }
            else if (this.currentAlgorithm is AlgorithmDFS)
            {
                this.currentAlgorithm = new AlgorithmDFS(this.graph, this.startNode, this.goalNode);
            }
        }

        switch (algorithm)
        {
            case ((int)ENeudinkelAlgorithm.ASTAR):
                this.currentAlgorithm = new AlgorithmAStar(this.graph, this.startNode, this.goalNode);
                break;
            case ((int)ENeudinkelAlgorithm.DIJKSTRA):
                this.currentAlgorithm = new AlgorithmDijkstra(this.graph, this.startNode, this.goalNode);
                break;
            case ((int)ENeudinkelAlgorithm.DFS):
                this.currentAlgorithm = new AlgorithmDFS(this.graph, this.startNode, this.goalNode);
                break;
        }

        GD.Print("Initialized Algorithm: " + this.currentAlgorithm.GetType().Name);
    }

    public void onSetStartNode(GraphNode node)
    {
        this.startNode = node;
    }

    public void onSetGoalNode(GraphNode node)
    {
        this.goalNode = node;
    }

    public void onSetIsSimulationAutomatic(bool isSimulationAutomatic)
    {
        this.isSimulationAutomatic = isSimulationAutomatic;
        GD.Print("isSimulationAutomatic is now: " + this.isSimulationAutomatic.ToString());

        if (isSimulationAutomatic)
        {
            executeAutomaticAlgorithmStepsAsync();
        }
    }

    // automatisches Ausführen vom Algorithmus mit Geschwindigkeit automaticSimulationSpeed
    // hört automatisch auf, wenn isSimulationAutomatic = false
    private async void executeAutomaticAlgorithmStepsAsync()
    {
        while (isSimulationAutomatic)
        {
            doNextSearchStepHelper();

            // wait
            // TODO: evtl geschwindikeits-berechnung ändern
            await ToSignal(GetTree().CreateTimer(1.0 / automaticSimulationSpeed), SceneTreeTimer.SignalName.Timeout);
        }
    }

    // custom getter, weil c# dumm ist
    public GamePhase getGamePhase()
    {
        return this.gamePhase;
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
