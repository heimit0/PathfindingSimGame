using System;
using Godot;

/// <summary>
/// Kapselt alle Code-Internen Signale
/// in einer Klasse die ge-Autoload-et wird.
/// </summary>
public partial class Events : Node
{

	[Signal]
	public delegate void PushOpenQueueEventHandler(GraphNode node, float prio);

	[Signal]
	public delegate void PopOpenQueueEventHandler();

	[Signal]
	public delegate void ClearOpenQueueEventHandler();

	public static Events Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
	}

}
