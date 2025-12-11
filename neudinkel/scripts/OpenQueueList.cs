using Godot;
using System;
using System.Collections.Generic;

public partial class OpenQueueList : NeudinkelItemList
{

	public override void _Ready()
	{
		// Ensure no one can mess this up in the UI
		this.MaxColumns = 3;
		this.SameColumnWidth = true;
		this.FixedColumnWidth = 58;

		Events.Instance.UpdateOpenQueue += onUpdateOpenQueue;
		Events.Instance.ClearOpenQueue += onClearOpenQueue;
	}

	public void onUpdateOpenQueue()
	{
		NeudinkelPriorityQueue<GraphNode, float> queue;

		queue = GetNode<SimulationGameManager>("/root/Node3D").currentAlgorithm.unvisitedNodes;

		GD.Print("UnvisitedNodes:");
		foreach ((float _prio, GraphNode node) in queue.Items)
		{
			GD.Print(_prio);
		}
	}

	/// <summary>
	/// Leert die gesamte Queue.
	/// Entfernt alle Elemente aus der internen Liste und der UI.
	/// </summary>
	public void onClearOpenQueue()
	{
		this.Clear();
	}

}
