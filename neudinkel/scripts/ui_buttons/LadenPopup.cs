using Godot;
using System;

public partial class LadenPopup : PopupMenu
{
    public event Action<string>? Submitted;

    public void Open()
    {
        foreach (string filename in LevelLoadList.getFileNames())
        {
            AddItem(filename);
        }

        Size = new Vector2I(Size.X, 500);
        Show();
        MoveToCenter();
    }

    public void onLaden()
    {
        Hide();
    }

    public void onAbbrechen()
    {
        Hide();
    }
}
