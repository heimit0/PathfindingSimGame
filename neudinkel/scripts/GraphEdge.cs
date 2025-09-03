using Godot;
using System;

/// <summary>
/// Alle moeglichen Typen fuer eine Strasse
/// </summary>
public enum StreetType
{
	//enum weist jedem member nen integer zu (0,1,2,...) die kann man manuel setzen
	//und mit Typecasts abrufen (int)type gibt den int oder falls nötig (StreetType)int
	//den typ nach kosten 
	//Probleme enstehen wenn man paare casted die nicht enthalten sind
	AUTOBAHN = 10,
	LANSTRASSE = 5,
	FELDWEG = 1
};

public partial class GraphEdge : Node3D
{

	private GraphNode beginNode;
	private GraphNode endNode;
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
		return (float)this.type;
	}

	public void setBeginNode(GraphNode beginNode)
	{
		this.beginNode = beginNode;
	}
	public void setEndNode(GraphNode endNode)
	{
		this.endNode = endNode;
	}
	public void setStreetType(StreetType type)
	{
		this.type = type;
	}

	public GraphNode getBeginNode()
	{
		return this.beginNode;
	}
	public GraphNode getEndNode()
	{
		return this.endNode;
	}
	public StreetType getStreetType()
	{
		return this.type;
	}

}
