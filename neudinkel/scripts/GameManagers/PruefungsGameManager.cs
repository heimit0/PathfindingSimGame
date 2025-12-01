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
        string levelToLoad = string.IsNullOrEmpty(Globals.nextLevel) ? "test" : Globals.nextLevel;
        algo = Globals.pruefungsAlgorithmus;
		prepScene(levelToLoad); 
		onInitOrChangeAlgorithm((int)algo);
        initialSteps();
    }

    public override void _Process(double delta)
	{
		base._Process(delta);
	}

	//Macht bei allen Algorithmen die ersten initilaisierungs steps
	public void initialSteps()
	{
		currentAlgorithm.doSearchStep();
		currentAlgorithm.doSearchStep();
		currentAlgorithm.doSearchStep();
	}


	public void playerInteraction(int[] tilePos)
	{
		GD.Print("Player did smth");
		currentAlgorithm.doSearchStep();

	}



}
