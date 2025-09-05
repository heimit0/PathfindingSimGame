using System;
using System.Collections.Generic;

public class AlgorithmDijkstra : NeudinkelAlgorithm
{

	// jede Node zeigt auf ihren besten Nachbarn ("Elternteil"), um den besten Pfad zu rekonstruieren
	public Dictionary<GraphNode, GraphNode> cameFrom;
	
	// TODO(mkolb2): Implementieren (Siehe Erklärungen NeudinkelAlgorithm.cs)
	public AlgorithmDijkstra(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;

		this.startNode = startNode;
		this.currentNode = startNode;
		this.goalNode = goalNode;
		this.unvisitedNodes.Enqueue(startNode, 0);

		// alle nodes mit distanz = unendlich initialisieren
		foreach (GraphNode node in graph.getNodes()) {
			this.distanceFromStart.Add(node, float.MaxValue);
		}

		this.steps = new Func<Graph, int>[10]; // idk wie viele
		// schritte funktionen zuordnen
	}
	
}
