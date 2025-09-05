using Godot;
using System;
using System.Collections.Generic;
using System.Net;

/// <summary>
/// Bildet den Zustand einer Wegfindung ab.
/// </summary>
public class GraphSearchState
{

	public GraphSearchAlgorithm algorithm { get; set; }

	public GraphNode startNode { get; set; }
	public GraphNode goalNode { get; set; }
	public GraphNode currentNode { get; set; }

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

	public GraphSearchState(GraphSearchAlgorithm algorithm)
	{
		this.algorithm = algorithm;
		// TODO(gfriedr3, mkolb2 oder jbrause): Objektinstanzen anlegen
	}
}
