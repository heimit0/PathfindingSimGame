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
		
	}
	
	/// <summary>
	/// Berechnet die Traversierungskosten der Kante
	/// </summary>
	public float getCost()
	{
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
	
	public void setStreetType(StreetType streetType)
	{
		this.type = streetType;
	}
	public StreetType getStreetType()
	{
		return this.type;
	}

}
