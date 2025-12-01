using Godot;
using System;

public partial class UiToggle : HBoxContainer  
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
			// Root suchen mittels Namens - wir könnten auch mittels Typ suchen ist allerdings uneindeutiger 
			if (current.Name == RootName)
			{
				// Canvas Layer mit dem Namen UI finden damit wir es unsichtbar schalten können 
				uiCanvasLayer = FindCanvasLayerRecursive(current, UiLayerName);
				
				if (uiCanvasLayer == null)
				{
					GD.PrintErr("Konnte CanvasLayer nicht finden");
				}
				else
				{
					uiCanvasLayerPath = uiCanvasLayer.GetPath();
				}
				return;
			}

			current = current.GetParent();
		}

		GD.PrintErr("Root wurde nicht gefunden");
	}


	// Namen sagt eigentlich schon alles aus - rekursive suche nach dem CanvasLayer (hier doppelte Überprüfung ob Typ und Name gleich)
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
			// nochmals FindUI() nutzen falls das UI CanvasLayer nicht gefunden wurde - nur zur doppelten Sicherheit 
			FindUI();
			if (uiCanvasLayer == null)
			{
				return;
			}
		}

		uiCanvasLayer.Visible = false;
	}

	public void UiEinblenden()
	{
		if (uiCanvasLayer == null)
		{
			// selbes Spiel wie beim Ausblenden zur Sicherheit - falls der Layer irgendwie entfernt werden sollte  
			FindUI();
			if (uiCanvasLayer == null)
			{
				return;
			}
		}
		uiCanvasLayer.Visible = true;
	}
}
