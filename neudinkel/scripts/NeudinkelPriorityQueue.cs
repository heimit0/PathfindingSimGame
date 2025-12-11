using System;
using System.Collections.Generic;

public class NeudinkelPriorityQueue<TElement, TPriority> where TPriority : IComparable
														 where TElement : class?
{

	/*
		Implementiert eine naive PriorityQueue,
		weil C#s PQ irgendwie Müll ist.
	*/
	public List<(TPriority, TElement)> Items { get; set; }
	public int Count { get { return this.Items.Count; } }

	public NeudinkelPriorityQueue()
	{
		this.Items = new List<(TPriority, TElement)>();
	}

	public void Enqueue(TElement item, TPriority prio)
	{
		int pos = 0;

		for (int i = 0; i <= this.Items.Count; i++)
		{
			pos = i;
			if (i == this.Items.Count) break;

			TPriority _prio = this.Items[i].Item1;
			pos = i;
			// wenn das aktuelle Element eine größere Prio hat
			if (_prio.CompareTo(prio) > 0) break;  // prio < _prio
		}

		this.Items.Insert(pos, (prio, item));
		return;
	}

	public TElement? Dequeue()
	{
		if (this.Items.Count == 0)
		{
			return null;
		}
		else
		{
			TElement e = this.Items[0].Item2;
			this.Items.RemoveAt(0);
			return e;
		}
	}

}
