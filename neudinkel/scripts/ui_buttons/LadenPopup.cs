using Godot;
using System;

public partial class LadenPopup : Popup
{
    public event Action<string>? Submitted;

    private ItemList? _itemList;
    private Label? _titleLabel;

    public override void _Ready()
    {
        _itemList = GetNodeOrNull<ItemList>("VBoxContainer/ItemList");
        _titleLabel = GetNodeOrNull<Label>("VBoxContainer/HBoxContainer/Titel");

        if (_itemList != null)
        {
            _itemList.ItemSelected += OnItemSelected;

            _itemList.AddThemeConstantOverride("h_separation", 20); // Abstand zwischen Levelname und Außenwand
            _itemList.AddThemeConstantOverride("v_separation", 10);  // Abstand zwischen Zeilen

        }


    }

    public void Open()
    {
        if (_itemList == null) return;

        // Titel setzen (optional)
        if (_titleLabel != null)
            _titleLabel.Text = "Level auswählen:";

        _itemList.Clear();

        var files = LevelLoadList.getFileNames();
        if (files == null || files.Length == 0)
            return;

        foreach (string filename in files)
            // prüfungsmodus-lvl ausschließen
            if (!filename.ToLower().StartsWith("pruef")) _itemList.AddItem(filename);

        Size = new Vector2I(250, 350);
        PopupCentered();
    }

    private void OnItemSelected(long index)
    {
        if (_itemList == null) return;

        string content = _itemList.GetItemText((int)index);

        if (content.Contains(".json"))
            content = content.Split(".")[0];

        Submitted?.Invoke(content);
        Hide();
    }
}