using Godot;
using System;

public partial class PruefungsGameManager : SimulationGameManager
{

    private GraphSearchAlgorithm algo;
    public override void  _Ready()
    {
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

	public void initialSteps()
	{
		
	}

}
