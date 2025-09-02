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

	private readonly GraphNode begin;
	private readonly GraphNode end;
	private readonly StreetType type;

	/// <summary>
	/// Member initialisieren
	/// </summary>
	public GraphEdge(GraphNode begin, GraphNode end, StreetType type)
	{
		// TODO(jbrause)
		// TODO(hsimosse): Beim Erstellen soll automatisch ein passendes Modell geladen
		// und als Kindobjekt hinzugefuegt werden. Eventuell gleich updateModel nutzen?
	}
	
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
