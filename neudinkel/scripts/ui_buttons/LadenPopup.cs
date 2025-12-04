using Godot;
using System;

public partial class LadenPopup : PopupMenu
{
    public event Action<string>? Submitted;

    public override void _Ready()
    {
        IndexPressed += onItemSelected;
        HideOnItemSelection = false;
    }

    public void Open()
    {
		Clear();

        foreach (string filename in LevelLoadList.getFileNames())
        {
            AddItem(filename);
        }

        MaxSize = new Vector2I(250, 500);
        Show();
        MoveToCenter();
    }

    private void onItemSelected(long index)
    {
        string content = this.GetItemText((int)index);

		if (content.Contains(".json")) content = content.Split(".")[0];

        Submitted?.Invoke(content);
		Hide();
    }
}
