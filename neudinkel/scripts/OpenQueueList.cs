using Godot;
using System;
using System.Collections.Generic;

public partial class OpenQueueList : NeudinkelItemList
{
	private bool _isDisposed = false;

	public override void _Ready()
	{
		// Ensure no one can mess this up in the UI
		this.MaxColumns = 3;
		this.SameColumnWidth = true;
		this.FixedColumnWidth = 58;

		_isDisposed = false;
		Events.Instance.UpdateOpenQueue += onUpdateOpenQueue;
		Events.Instance.ClearOpenQueue += onClearOpenQueue;
	}

	public override void _ExitTree()
	{
		// Mark as disposed and unsubscribe from events
		_isDisposed = true;
		if (Events.Instance != null)
		{
			Events.Instance.UpdateOpenQueue -= onUpdateOpenQueue;
			Events.Instance.ClearOpenQueue -= onClearOpenQueue;
		}
		base._ExitTree();
	}

	public void onUpdateOpenQueue()
	{
		// Early exit if already disposed or not valid
		if (_isDisposed || !GodotObject.IsInstanceValid(this))
		{
			_isDisposed = true;
			return;
		}

		try
		{
			NeudinkelPriorityQueue<GraphNode, float> queue;

			queue = GetNode<SimulationGameManager>("/root/Node3D").currentAlgorithm.unvisitedNodes;

			Clear();
			foreach ((float _prio, GraphNode _node) in queue.Items)
			{
				AddItem(_node.tilePosX.ToString(), null, false);
				AddItem(_node.tilePosY.ToString(), null, false);
				AddItem(_prio.ToString(), null, false);
			}
		}
		catch (ObjectDisposedException)
		{
			// Object was disposed, silently ignore
			_isDisposed = true;
			return;
		}
		catch (Exception)
		{
			// Catch any other exceptions (null reference, disposed objects, etc.)
			// This can happen if the object is being destroyed while signals are firing
			_isDisposed = true;
			return;
		}
	}

	/// <summary>
	/// Leert die gesamte Queue.
	/// Entfernt alle Elemente aus der internen Liste und der UI.
	/// </summary>
	public void onClearOpenQueue()
	{
		// Early exit if already disposed or not valid
		if (_isDisposed || !GodotObject.IsInstanceValid(this))
		{
			_isDisposed = true;
			return;
		}

		try
		{
			Clear();
		}
		catch (ObjectDisposedException)
		{
			// Object was disposed, silently ignore
			_isDisposed = true;
			return;
		}
		catch (Exception)
		{
			// Catch any other exceptions (null reference, disposed objects, etc.)
			// This can happen if the object is being destroyed while signals are firing
			_isDisposed = true;
			return;
		}
	}

}
