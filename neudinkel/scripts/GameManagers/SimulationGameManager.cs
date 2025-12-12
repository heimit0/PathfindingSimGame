using Godot;
using System;

public partial class SimulationGameManager : GameManager
{
	[Signal]
	public delegate void setIsSimulationAutomaticEventHandler(bool isSimulationAutomatic);

	[Signal]
	public delegate void InitOrChangeAlgorithmEventHandler(int algorithm); // algorithmus setzen
																		   // usage: EmitSignal(GameManager.InitOrChangeAlgorithm, (int) ENeudinkelAlgorithm.ASTAR);

	[Signal]
	public delegate void DoSearchForwardEventHandler(bool step); // Algorithmus vorwärts bis Breakpoint. Wenn step true dann einzelner Schritt

	[Signal]
	public delegate void UpdateUIPseudoCodeEventHandler(int currentStep); // pseudo code anzeigen

	[Signal]
	public delegate void AlgorithmFinishedEventHandler(); // weg gefunden, weg anzeigen

	[Signal]
	public delegate void SetAutomaticSimulationSpeedEventHandler(int newSpeed); // slider für speed im simulations-modus




	[Export]
	public float automaticSimulationSpeed = 1; // für simulationsmodus
	public bool isSimulationAutomatic { get; set; }

	public NeudinkelAlgorithm currentAlgorithm { get; set; }

	public PseudocodeTextBox textBoxBox { get; set; }
	public Minimap minimap;

	public GraphNode lastNode;


	private NodePath minimapPath = "Simulationsmodus/UI/Info/totalBlock/minimapBox/Minimap";

	public override void _Ready()
	{
		prepScene("baumodus_save");

		DoSearchForward += onDoSearchForward;
		InitOrChangeAlgorithm += onInitOrChangeAlgorithm;
		SetStartNode += onSetStartNode;
		SetGoalNode += onSetGoalNode;

		startNode.updateModel();
		goalNode.updateModel();
		lastNode = startNode;

		Minimap minimap = GetNodeOrNull<Minimap>(minimapPath);
		minimap.initMiniMap(this);

	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	// Handler for tile hover/selection events
	private void OnTileHovered(Vector2I tilePos)
	{
		GraphNode playerNode = this.graph.getNode(tilePos.X, tilePos.Y);

		if(playerNode is not null && playerNode != lastNode)
		{
			if(lastNode is not null)
			{
				lastNode.setHighlighted(false);
			}
			lastNode = playerNode;
			playerNode.setHighlighted(true);
		}
	}

	// ruft ensprechende Methode in jedem NeudinkelAlgorithm auf
	// gibt den nächsten index im schritt-array des algorithmus' zurück (für UI)
	public void onDoSearchForward(bool step)
	{
		if (this.isSimulationAutomatic) GD.Print("isSimulationAutomatic is now: false");
		this.isSimulationAutomatic = false;

		doSearchForwardHelper(step);

		// EmitSignal(SignalName.UpdateUIPseudoCode, nextStep);
	}

	// NICHT DAS SIGNAL, extra funktion für abstraktion
	public void doSearchForwardHelper(bool step)
	{
		if (this.currentAlgorithm.currentStep == -1)
		{
			if (this.currentAlgorithm is NeudinkelAlgorithm)
			{
				((NeudinkelAlgorithm)this.currentAlgorithm).visualize(this.graph, this);
			}

			return;
		}

		textBoxBox.resetAllFormatting();
		textBoxBox.highlightCurrentStep(this.currentAlgorithm.currentStep);

		GD.Print("Executing step: " + this.currentAlgorithm.currentStep);
		int nextStep = step ? this.currentAlgorithm.doSearchStep() : this.currentAlgorithm.doSearchForward();

		textBoxBox.highlightNextStep(nextStep);

		if (this is PruefungsGameManager)
		{
			Minimap minimap = GetNode<Minimap>(minimapPath);
			minimap.updateMinimapNodes(this.currentAlgorithm);
		}
		else
		{
			Minimap minimap = GetNode<Minimap>(minimapPath);
			minimap.updateMinimapNodes(this.currentAlgorithm);
		}
		this.minimap.updateMinimapNodes(this.currentAlgorithm);
	}

	// automatisches Ausführen vom Algorithmus mit Geschwindigkeit automaticSimulationSpeed
	// hört automatisch auf, wenn isSimulationAutomatic = false
	private async void executeAutomaticAlgorithmStepsAsync()
	{
		while (isSimulationAutomatic)
		{
			doSearchForwardHelper(true);

			// wait
			// TODO: evtl geschwindikeits-berechnung ändern
			await ToSignal(GetTree().CreateTimer(1.0 / automaticSimulationSpeed), SceneTreeTimer.SignalName.Timeout);
		}
	}

	public void onSetIsSimulationAutomatic(bool isSimulationAutomatic)
	{
		// muessen wir so schreiben, sonst starten wir die Schleife mehrfach
		if (isSimulationAutomatic)
		{
			if (!this.isSimulationAutomatic)
			{
				this.isSimulationAutomatic = true;
				executeAutomaticAlgorithmStepsAsync();
			}
		}
		else
		{
			this.isSimulationAutomatic = false;
		}
		GD.Print("isSimulationAutomatic is now: " + this.isSimulationAutomatic.ToString());
	}

	// start immer alles neu
	// int algorithm == -1 => gleichen Algorithmus nochmal, i.e. nur neustarten
	public void onInitOrChangeAlgorithm(int algorithm)
	{
		GD.Print("start change algorithm");

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

		Events.Instance.EmitSignal(Events.SignalName.ClearOpenQueue);

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

		GD.Print("start visualizing algorithm");
		this.currentAlgorithm.visualize(this.graph, this);
		onSetIsSimulationAutomatic(false);

		GD.Print("Initialized Algorithm: " + this.currentAlgorithm.GetType().Name);
		if (this is PruefungsGameManager)
		{
			textBoxBox = GetNode<PseudocodeTextBox>("Pruefungsmodus/UI/PseudocodeBlock/PseudocodePanel/PseudocodeTextBox");
		}
		else
		{
			textBoxBox = GetNode<PseudocodeTextBox>("Simulationsmodus/UI/PseudocodeBlock/PseudocodePanel/PseudocodeTextBox");
		}
		textBoxBox.onInitOrChangeAlgorithm(algorithm);

		// Step-Buttons freigeben (theoretisch nur beim ersten Mal notwendig, aber zur Sicherheit immer)
		if (this is PruefungsGameManager) return;

		foreach (Node i in GetNode("Simulationsmodus/UI/ButtonsTopRight/ButtonsStep").GetChildren())
		{
			// Sanity check, auch wenn alle Kinder Buttons sein sollten...
			if (i is Button) ((Button)i).Disabled = false;
		}

		GD.Print("Pseudocde done");
	}

	public void onChangeToBauMode()
	{
		// müssen uns nix merken
		GetTree().ChangeSceneToFile("res://scenes/baumodus.tscn");
	}

}
