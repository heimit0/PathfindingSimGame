using Godot;
using System;

/// <summary>
/// Alle moeglichen Typen fuer eine Strasse
/// </summary>
public enum StreetType
{
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
