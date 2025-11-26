using Godot;
using System;
using System.Collections.Generic;

public partial class OpenQueueList : NeudinkelItemList
{

	/*
		Saves all GraphNodes with a given priority in the order added.
		Keep sorted when inserting items.
	*/
	List<(float, List<GraphNode>)> items;

	public override void _Ready()
	{
		this.items = new List<(float, List<GraphNode>)>();
		// Ensure no one can mess this up
		this.MaxColumns = 3;

		Events.Instance.PushOpenQueue += onPushOpenQueue;
		Events.Instance.PopOpenQueue += onPopOpenQueue;
	}

	public void onPushOpenQueue(GraphNode node, float prio)
	{
		List <GraphNode> list = null;
		(float, List<GraphNode>) newPrio;
		int newIndex = 0;

		foreach ((float _prio, List<GraphNode> _list) in this.items)
		{
			newIndex += _list.Count;
			if (_prio == prio)
			{
				list = _list;
				break;
			}
		}

		if (list == null)
		{
			// Add new priority
			newPrio = (prio, new List<GraphNode>());
			newPrio.Item2.Add(node);
			this.items.Add(newPrio);
			// Weird comparing stuff because i don't know how tuples compare
			// helllpppp
			this.items.Sort(Comparer<(float, List<GraphNode>)>.Create(
				(a, b) => a.Item1 > b.Item1 ? 1 : a.Item1 < b.Item1 ? -1 : 0
			));
			foreach ((float _prio, List<GraphNode> _list) in this.items)
			{
				newIndex += _list.Count;
				if (_prio == prio)
				{
					break;
				}
			}
		}
		else
		{
			list.Add(node);
		}
		this.InsertItem(newIndex    , node.getTilePosX().ToString(), null, false);
		this.InsertItem(newIndex + 1, node.getTilePosX().ToString(), null, false);
		this.InsertItem(newIndex + 2, prio.ToString(), null, false);
	}

	public void onPopOpenQueue()
	{
		if (this.items.Count > 0)
		{
			this.items[0].Item2.RemoveAt(0);
			// Don't keep empty lists around
			if (this.items[0].Item2.Count == 0) this.items.RemoveAt(0);
			// Update UI accordingly
			this.RemoveItem(0);
			this.RemoveItem(0);
			this.RemoveItem(0);
		}
	}

}
