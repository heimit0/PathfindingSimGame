using Godot;
using System;

public partial class UiToggle : HBoxContainer   // <-- Geändert zu VBoxContainer
{
	[Export] public string RootName = "Node3D";
	[Export] public string UiLayerName = "UI";

	private CanvasLayer uiCanvasLayer;

	private NodePath uiCanvasLayerPath;

	public override void _Ready()
	{
		FindUI();
	}

	private void FindUI()
	{
		Node current = this;

		while (current != null)
		{
			// Baumodus Root suchen
			if (current.Name == RootName)
			{
				uiCanvasLayer = FindCanvasLayerRecursive(current, UiLayerName);
				
				if (uiCanvasLayer == null)
				{
					GD.PrintErr($"Konnte CanvasLayer '{UiLayerName}' nicht finden.");
				}
				else
				{
					uiCanvasLayerPath = uiCanvasLayer.GetPath();
					GD.Print($"UI gefunden: {uiCanvasLayer.GetPath()}");
				}
				return;
			}

			current = current.GetParent();
		}

		GD.PrintErr("Root wurde im Elternpfad nicht gefunden");
	}

	private CanvasLayer FindCanvasLayerRecursive(Node node, string name)
	{
		if (node is CanvasLayer cl && node.Name == name)
			return cl;

		foreach (Node child in node.GetChildren())
		{
			var found = FindCanvasLayerRecursive(child, name);
			if (found != null)
				return found;
		}

		return null;
	}

	public void UiAusblenden()
	{
		if (uiCanvasLayer == null)
		{
			GD.PrintErr("UiAusblenden: uiCanvasLayer ist null, versuche erneut FindUI()");
			FindUI();
			if (uiCanvasLayer == null)
			{
				GD.PrintErr("UiAusblenden: Abbruch – keine gültige UI-Referenz");
				return;
			}
		}

		uiCanvasLayer.Visible = false;
		GD.Print("UI ausgeblendet");
	}

	public void UiEinblenden()
	{
		if (uiCanvasLayer == null)
		{
			GD.PrintErr("UiEinblenden: uiCanvasLayer ist null, versuche erneut FindUI()");
			FindUI();
			if (uiCanvasLayer == null)
			{
				GD.PrintErr("UiEinblenden: Abbruch – keine gültige UI-Referenz");
				return;
			}
		}

		uiCanvasLayer.Visible = true;
		GD.Print("UI eingeblendet");
	}
}
