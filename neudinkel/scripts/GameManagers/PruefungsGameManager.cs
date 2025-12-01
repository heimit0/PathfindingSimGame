using Godot;
using System;

public partial class PruefungsGameManager : SimulationGameManager
{

	[Signal]
	public delegate void TileSelectedEventHandler(Vector2I tilePos);
    private GraphSearchAlgorithm algo;
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
		this.currentAlgorithm.doSearchStep();
		GD.Print("step 2");
		this.currentAlgorithm.doSearchStep();
		GD.Print("step 3");
		this.currentAlgorithm.doSearchStep();
	}


	public void playerInteraction(Vector2I tilePos)
	{
		GD.Print("Player did smth");
		currentAlgorithm.doSearchStep();
		if(this.graph.getNode(tilePos.X, tilePos.Y) == currentAlgorithm.currentNode)
		{
			GD.Print("Correct Tile Clicked");
			if (currentAlgorithm.currentNode != null) currentAlgorithm.currentNode.setHighlighted(true);
		}
		else
		{
			GD.Print("Wrong Tile Clicked");
		}

	}



}
