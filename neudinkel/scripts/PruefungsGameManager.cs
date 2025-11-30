using Godot;
using System;

public partial class PruefungsGameManager : GameManager
{
    public override void  _Ready()
    {
        string levelToLoad = string.IsNullOrEmpty(Globals.nextLevel) ? "test" : Globals.nextLevel;
		prepScene(levelToLoad);
        
    }

    public override void _Process(double delta)
	{
		base._Process(delta);
	}

}
