using System;
using System.Collections.Generic;

public class AlgorithmAStar : NeudinkelAlgorithm
{

	// jede Node zeigt auf ihren besten Nachbarn ("Elternteil"), um den besten Pfad zu rekonstruieren
	public Dictionary<GraphNode, GraphNode> cameFrom;
	// f(n) = distanceFromStart + heuristic
	public Dictionary<GraphNode, float> totalCost;
	
	// TODO(gfriedr3 oder hsimosse): Implementieren (Siehe Erklärungen NeudinkelAlgorithm.cs)
	public AlgorithmAStar(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;

		this.startNode = startNode;
		this.currentNode = startNode;
		this.goalNode = goalNode;
		this.unvisitedNodes.Enqueue(startNode, heuristic(startNode, goalNode));

		// alle nodes mit distanz = unendlich initialisieren
		// ist der F-score
		foreach (GraphNode node in graph.getNodes()) {
			this.totalCost.Add(node, float.MaxValue);
		}

		this.steps = new Func<Graph, int>[10]; // idk wie viele
		// schritte funktionen zuordnen
	}

	public float heuristic(GraphNode startNode, GraphNode goalNode)
	{
		return -1.0f;
	}
	
}
