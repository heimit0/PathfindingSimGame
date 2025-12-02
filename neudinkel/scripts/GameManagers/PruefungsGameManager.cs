using Godot;
using System;

public partial class PruefungsGameManager : SimulationGameManager
{

	private GraphNode checkPointNode;

	[Signal]
	public delegate void TileSelectedEventHandler(Vector2I tilePos);
    private GraphSearchAlgorithm algo;

	private int correctStepCounterSinceLastSave;
    public override void  _Ready()
    {
		TileSelected += playerInteraction;
		SetStartNode += onSetStartNode;
		SetGoalNode += onSetGoalNode;
		DoSearchForward += onDoSearchForward;
		InitOrChangeAlgorithm += onInitOrChangeAlgorithm;

        string levelToLoad = string.IsNullOrEmpty(Globals.nextLevel) ? "level1_dijkstra" : Globals.nextLevel;
        algo = Globals.pruefungsAlgorithmus;

		prepScene(levelToLoad);
		onInitOrChangeAlgorithm((int)algo);

        initialSteps();
		currentAlgorithm.startNode.setHighlighted(true);
		currentAlgorithm.goalNode.setHighlighted(true);
	}

    public override void _Process(double delta)
	{
		base._Process(delta);
	}

	//Macht bei allen Algorithmen die ersten initilaisierungs steps
	public void initialSteps()
	{
		GD.Print("Startile "+ this.currentAlgorithm.startNode.getTilePosString());
		GD.Print("Goaltile: "+ this.currentAlgorithm.goalNode.getTilePosString());
		
		GD.Print("step 1");
		this.doSearchForwardHelper(true);
		GD.Print("step 2");
		this.doSearchForwardHelper(true);
		GD.Print("step 3");
		this.doSearchForwardHelper(true);
		saveCheckpoint();
		correctStepCounterSinceLastSave = 0;
	}


	public void playerInteraction(Vector2I tilePos)
	{
		GD.Print("Player did smth");
		this.doSearchForwardHelper(true);
		if(this.graph.getNode(tilePos.X, tilePos.Y) == currentAlgorithm.currentNode)
		{
			GD.Print("Correct Tile Clicked");
			correctStepCounterSinceLastSave++;
			if(correctStepCounterSinceLastSave>=3)
			{
				correctStepCounterSinceLastSave = 0;
				saveCheckpoint();
			}
			if (currentAlgorithm.currentNode != null) currentAlgorithm.currentNode.setHighlighted(true);
		}
		else
		{
			GD.Print("Wrong Tile Clicked, Returning to Checkpoint");
			returnToCheckpoint();
		}

	}



	public void saveCheckpoint()
	{
		checkPointNode = currentAlgorithm.currentNode;
	}


	public int returnToCheckpoint()
	{
		GD.Print("Start resetting Algorithm");
		onInitOrChangeAlgorithm(-1);
		int allowedNumberOfSteps = 1000000;
		int failsafe = 0;
		while(currentAlgorithm.currentNode != checkPointNode && failsafe < allowedNumberOfSteps)
		{
			currentAlgorithm.doSearchStep();
			failsafe++;
		}
		if(checkPointNode == currentAlgorithm.currentNode)
		return 1;
		return -1;
	}



}
