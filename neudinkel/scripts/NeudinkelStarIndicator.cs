using System;
using Godot;

/// <summary>
/// Das Sternicon was angibt ob ein Prüfungslevel geschafft wurde.
/// </summary>
public partial class NeudinkelStarIndicator : Sprite2D
{

	[Export]
	public ENeudinkelAlgorithm algorithm;

	[Export(PropertyHint.Range, "0,2,")]
	public int starNum;

	public override void _Ready()
	{
		bool progress;
		int state = 0;

		if (algorithm == ENeudinkelAlgorithm.DIJKSTRA) state = Globals.config.progress & 0x07;
		if (algorithm == ENeudinkelAlgorithm.ASTAR)    state = (Globals.config.progress >> 3) & 0x07;
		if (algorithm == ENeudinkelAlgorithm.DFS)      state = (Globals.config.progress >> 6) & 0x07;

		if ((state >> starNum & 0x01) == 1)
			// done
			this.Texture = (Texture2D) GD.Load("assets/ui_icons/leerer-stern.svg");
		else
			// not done
			this.Texture = (Texture2D) GD.Load("assets/ui_icons/voller-stern.svg");
	}

}
