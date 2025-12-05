using Godot;
using System;
using System.Collections.Generic;

public partial class PseudocodeTextBox : RichTextLabel
{
	Dictionary<Label, int> pseudoCodeNodes;
	SimulationGameManager gameManager;

	public override void _Ready()
	{
		this.gameManager = GetNode<SimulationGameManager>("/root/Node3D");
		// this.gameManager.InitOrChangeAlgorithm += onInitOrChangeAlgorithm;
	}

	public void onInitOrChangeAlgorithm(int algorithm)
	{
		foreach (Label node in this.GetChildren())
		{
			node.QueueFree();
		}

		this.pseudoCodeNodes = new Dictionary<Label, int>();
		if (this.gameManager.currentAlgorithm.pseudoCodeSteps is null)
		{
			GD.PrintErr("Algorithmus bei InitOrChangeAlgo is null");
			return;
		}

		GD.Print("adding " + this.gameManager.currentAlgorithm.pseudoCodeSteps.Length + " boxes");
		// textbox-objecte erstellen
		for (int i = 0; i < this.gameManager.currentAlgorithm.pseudoCodeSteps.Length; i++)
		{
			(int stepsIndex, string content) p = this.gameManager.currentAlgorithm.pseudoCodeSteps[i];

			Label newTextNode = new Label();

			newTextNode.Text += p.content;
			newTextNode.Position = new Vector2(0, i * 20);

			this.pseudoCodeNodes.Add(newTextNode, p.stepsIndex);

			newTextNode.AddThemeColorOverride("font_color", Colors.White);
			newTextNode.AddThemeFontSizeOverride("font_size", 12);

			AddChild(newTextNode);
		}
	}

	public void resetAllFormatting()
	{
		foreach (KeyValuePair<Label, int> entry in this.pseudoCodeNodes)
		{
			entry.Key.AddThemeColorOverride("font_color", Colors.White);
		}
	}

	public void highlightCurrentStep(int currentStep)
	{
		foreach (KeyValuePair<Label, int> entry in this.pseudoCodeNodes)
		{
			if (entry.Value == currentStep)
			{
				entry.Key.AddThemeColorOverride("font_color", Colors.Red);
			}
		}
	}

	public void highlightNextStep(int nextStep)
	{
		foreach (KeyValuePair<Label, int> entry in this.pseudoCodeNodes)
		{
			if (entry.Value == nextStep)
			{
				entry.Key.AddThemeColorOverride("font_color", Colors.Yellow);
			}
		}
	}
}
