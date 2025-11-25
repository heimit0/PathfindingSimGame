using Godot;
using System;

public partial class NeudinkelItemList : ItemList
{

	/// <summary>
	/// Inserts item at given index
	/// </summary>
	public void InsertItem(int index, string text, Texture2D icon = null, bool selectable = true)
	{
		int addedIndex;
		
		if (index > this.ItemCount)
			return;
		addedIndex = this.AddItem(text, icon, selectable);
		this.MoveItem(addedIndex, index);
		
		return;
	}

}
