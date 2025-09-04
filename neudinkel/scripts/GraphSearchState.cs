using Godot;
using System;
using System.Collections.Generic;
using System.Net;

/// <summary>
/// Bildet den Zustand einer Wegfindung ab.
/// </summary>
public class GraphSearchState
{

	private readonly GraphSearchAlgorithm algorithm { get; set; };

	private GraphNode startNode { get; set; }
	private GraphNode goalNode { get; set; }
	private GraphNode currentNode { get; set; }

	/*
		Algorithmus: A*
		 - openSet / openList => unvisitedNodes
		 - closedSet / closedList => visitedNodes
		 - cameFrom => cameFrom
		 - g(n) => distanceFromStart
		 - f(n) => totalCost
	*/

	/*
		Algorithmus: Dijkstra
		 - unvisited => unvisitedNodes
		 - visited => visitedNodes
		 - previous => cameFrom
		 - distances => distanceFromStart
	*/

	/*
		Algorithmus: DFS
		 - stack => nodesToProcess
		 - visited => visitedNodes
		 - not visited => unvisitedNodes
	*/

	// noch nicht besuchte Nodes
	public PriorityQueue<GraphNode, float> unvisitedNodes;
	// bereits besuchte Nodes
	public HashSet<GraphNode> visitedNodes;
	// jede Node zeigt auf ihren besten Nachbarn ("Elternteil"), um den besten Pfad zu rekonstruieren
	public Dictionary<GraphNode, GraphNode> cameFrom;
	// tatsächliche, bekannte Kosten pro Node zum Start
	public Dictionary<GraphNode, float> distanceFromStart;

	// Algorithmus: A*	
	// f(n) = distanceFromStart + heuristic
	public Dictionary<GraphNode, float> totalCost;

	// Algorithmus: DFS
	public Stack<GraphNode> nodesToProcess;

	// TODO(gfriedr3): Diese Klasse sollte auch die "Zwischenschritte" der jeweiligen Algorithmen abbilden können.
	// Dafür sollten sich Enums eigenen. Kann man den Zustand für alle Algos per Union in eine Variable packen?

	public GraphSearchState(GraphSearchAlgorithm algorithm)
	{
		this.algorithm = algorithm;
		// TODO(gfriedr3, mkolb2 oder jbrause): Objektinstanzen anlegen
	}
}
