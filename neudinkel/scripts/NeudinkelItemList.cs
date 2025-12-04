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
		
		if (index > this.ItemCount + 1 || index < 0)
		{
			GD.Print("NeudinkelItemList.InsertItem: Sinnloser Index " + index);
			return;
		}

		addedIndex = this.AddItem(text, icon, selectable);
		this.MoveItem(addedIndex, index);
		
		return;
	}

}
