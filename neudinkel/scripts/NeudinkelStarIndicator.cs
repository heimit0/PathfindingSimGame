using System;
using Godot;

/// <summary>
/// Das Sternicon was angibt ob ein Prüfungslevel geschafft wurde.
/// </summary>
public partial class NeudinkelStarIndicator : Sprite2D
{

	[Export(PropertyHint.Range, "0,2,")]
	public int starNum;

	public override void _Ready()
	{
		update();
	}

	public void update()
	{
		bool progress;
		int state = 0;
		GraphSearchAlgorithm algo = Globals.pruefungsAlgorithmus;

		if (algo == GraphSearchAlgorithm.DIJKSTRA) state = Globals.config.progress & 0x07;
		if (algo == GraphSearchAlgorithm.A_STAR)   state = (Globals.config.progress >> 3) & 0x07;
		if (algo == GraphSearchAlgorithm.DFS)      state = (Globals.config.progress >> 6) & 0x07;
		GD.Print("State: " + state);
		GD.Print("Algo: " + (int)algo);
		if ((state >> starNum & 0x01) == 1)
			// done
			this.Texture = (Texture2D) GD.Load("assets/ui_icons/voller-stern.svg");
		else
			// not done
			this.Texture = (Texture2D) GD.Load("assets/ui_icons/leerer-stern.svg");
	}

}
