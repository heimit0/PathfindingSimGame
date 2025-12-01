using Godot;
using System;

public partial class PruefungsGameManager : SimulationGameManager
{

	[Signal]
	public delegate void TileSelectedEventHandler(int[] tilePos);
    private GraphSearchAlgorithm algo;
    public override void  _Ready()
    {
		TileSelected += playerInteraction;
		SetStartNode += onSetStartNode;
		SetGoalNode += onSetGoalNode;
		DoSearchForward += onDoSearchForward;
		InitOrChangeAlgorithm += onInitOrChangeAlgorithm;
        string levelToLoad = string.IsNullOrEmpty(Globals.nextLevel) ? "test" : Globals.nextLevel;
        algo = Globals.pruefungsAlgorithmus;
		prepScene(levelToLoad);
		onInitOrChangeAlgorithm((int)algo);
        initialSteps();
		currentAlgorithm.currentNode.toggleHighlight(true);
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
		
		this.currentAlgorithm.goalNode.toggleHighlight(true);

		GD.Print("step 1");
		this.currentAlgorithm.doSearchStep();
		GD.Print("step 2");
		this.currentAlgorithm.doSearchStep();
		GD.Print("step 3");
		this.currentAlgorithm.doSearchStep();
	}


	public void playerInteraction(int[] tilePos)
	{
		GD.Print("Player did smth");
		currentAlgorithm.doSearchStep();
		if(this.graph.getNode(tilePos[0], tilePos[1]) == currentAlgorithm.currentNode)
		{
			GD.Print("Correct Tile Clicked");
			currentAlgorithm.currentNode.toggleHighlight(true);
		}
		else
		{
			GD.Print("Wrong Tile Clicked");
		}

	}



}
