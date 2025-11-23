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

	public override void _Ready()
	{
		prepScene("baumodus_save");

		DoSearchForward += onDoSearchForward;
		InitOrChangeAlgorithm += onInitOrChangeAlgorithm;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	// ruft ensprechende Methode in jedem NeudinkelAlgorithm auf
	// gibt den nächsten index im schritt-array des algorithmus' zurück (für UI)
	public void onDoSearchForward(bool step)
	{
		if (this.isSimulationAutomatic) GD.Print("isSimulationAutomatic is now: false");
		this.isSimulationAutomatic = false;

		doSearchForwardHelper();

		// EmitSignal(SignalName.UpdateUIPseudoCode, nextStep);
	}

	// NICHT DAS SIGNAL, extra funktion für abstraktion
	public void doSearchForwardHelper()
	{
		if (this.currentAlgorithm.currentStep == -1)
		{
			if (this.currentAlgorithm is NeudinkelAlgorithm)
			{
				((NeudinkelAlgorithm)this.currentAlgorithm).visualize(this.graph);
			}

			return;
		}

		textBoxBox.resetAllFormatting();
		textBoxBox.highlightCurrentStep(this.currentAlgorithm.currentStep);

		GD.Print("Executing step: " + this.currentAlgorithm.currentStep);
		int nextStep = this.currentAlgorithm.doSearchStep();

		textBoxBox.highlightNextStep(nextStep);
	}

	// automatisches Ausführen vom Algorithmus mit Geschwindigkeit automaticSimulationSpeed
	// hört automatisch auf, wenn isSimulationAutomatic = false
	private async void executeAutomaticAlgorithmStepsAsync()
	{
		while (isSimulationAutomatic)
		{
			doSearchForwardHelper();

			// wait
			// TODO: evtl geschwindikeits-berechnung ändern
			await ToSignal(GetTree().CreateTimer(1.0 / automaticSimulationSpeed), SceneTreeTimer.SignalName.Timeout);
		}
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

		this.currentAlgorithm.visualize(this.graph);
		onSetIsSimulationAutomatic(false);

		GD.Print("Initialized Algorithm: " + this.currentAlgorithm.GetType().Name);
		textBoxBox = GetNode<PseudocodeTextBox>("Simulationsmodus/UI/Pseudocodeblock/PseudocodeTextBox");
		textBoxBox.onInitOrChangeAlgorithm(algorithm);
	}
}
