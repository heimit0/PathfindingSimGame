using Godot;
using System;

/// <summary>
/// Alle moeglichen Typen fuer eine Strasse
/// </summary>
enum StreetType
{
	DEFAULT
};

public partial class GraphEdge : Node3D
{

	private GraphNode begin;
	private GraphNode end;
	private StreetType type;

	/// <summary>
	/// Member initialisieren
	/// </summary>
	public GraphEdge(GraphNode begin, GraphNode end, StreetType type)
	{
		
	}

}
