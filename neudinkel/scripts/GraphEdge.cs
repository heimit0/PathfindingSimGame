using Godot;
using System;

/// <summary>
/// Alle moeglichen Typen fuer eine Strasse
/// </summary>
public enum StreetType
{
	// TODO(mkolb2): Gibt es eine Moeglichkeit die Kosten im Enum zu speichern?
	DEFAULT
};

public partial class GraphEdge : Node3D
{

	private GraphNode begin;
	private GraphNode end;
	private StreetType type;

	public override void _Ready()
	{
		base._Ready();
		// TODO(hsimosse): Beim Erstellen soll automatisch ein passendes Modell geladen
		// und als Kindobjekt hinzugefuegt werden. Eventuell gleich updateModel nutzen?
	}

	public override void _Process(double delta) { }

	/// <summary>
	/// Aktualisiert das gerenderte Modell je nach Status.
	/// </summary> 
	public void updateModel()
	{
		// TODO(hsimosse): Der GraphNode sollte als Kindobjekt nen Model haben.
		// Wir ersetzen hier das Modell, je nach Zustand 
	}
	
	/// <summary>
	/// Berechnet die Traversierungskosten der Kante anhand des Typs
	/// </summary>
	public float getCost()
	{
		// TODO(jbrause oder mkolb2)
		return 0.0f;
	}

	// TODO(mkolb2): Setter

	public GraphNode getBeginNode()
	{
		return this.begin;
	}
	
	public GraphNode getEndNode()
	{
		return this.end;
	}
	
	public StreetType getStreetType()
	{
		return this.type;
	}

}
