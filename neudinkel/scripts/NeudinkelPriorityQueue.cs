using System;
using System.Collections.Generic;

public class NeudinkelPriorityQueue<TElement, TPriority> where TPriority : IComparable
														 where TElement : class?
{

	/*
		Implementiert eine naive PriorityQueue,
		weil C#s PQ irgendwie Müll ist.
	*/
	List<(TPriority, TElement)> items { get; set; }

	public NeudinkelPriorityQueue()
	{
		this.items = new List<(TPriority, TElement)>();
	}

	public void Enqueue(TElement item, TPriority prio)
	{
		int pos = 0;

		for (int i = 0; i < this.items.Count; i++)
		{
			TPriority _prio = this.items[i].Item1;
			pos = i;
			// wenn das aktuelle Element eine größere Prio hat
			if (_prio.CompareTo(prio) > 0) break;  // prio < _prio
		}

		this.items.Insert(pos, (prio, item));
		return;
	}

	public TElement? Deqeueue()
	{
		if (this.items.Count == 0)
		{
			return null;
		}
		else
		{
			TElement e = this.items[0].Item2;
			this.items.RemoveAt(0);
			return e;
		}
	}

}
