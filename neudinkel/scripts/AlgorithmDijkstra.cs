using System;
using System.Collections.Generic;

public class AlgorithmDijkstra : NeudinkelAlgorithm
{

	private GraphNode currentNeighbour;

	// TODO(mkolb2): Implementieren (Siehe Erklärungen NeudinkelAlgorithm.cs)
	public AlgorithmDijkstra(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;

		this.startNode = startNode;
		this.currentNode = startNode;
		this.goalNode = goalNode;
		this.unvisitedNodes.Enqueue(startNode, 0);
		this.distanceFromStart.Add(startNode,0);

		// alle nodes mit distanz = unendlich initialisieren
		foreach (GraphNode node in graph.getNodes()) {
			this.distanceFromStart.Add(node, float.MaxValue);
		}

		this.steps = new Func<Graph, int>[10]; // idk wie viele

		// schritte funktionen zuordnen
		while(this.unvisitedNodes.Count>0)
		{
			chooseNextNode(this.graph);
			visitNextNode(this.graph);
			if(this.currentNode.Equals(goalNode))
			{
				return;
			}
			foreach(GraphNode node in this.currentNode.getNeighbours())
			{
				currentNeighbour = node;
				checkNeighbour(graph);
			}
		}
	}
	
	///<summary>
	///ueberprueft die kosten zum naechsten nachbar, speichert diese und reiht in 
	///in die queue ein
	///</summary>
	public int checkNeighbour(Graph g)
	{
		int cost = this.distanceFromStart[this.currentNode] + (int)currentNeighbour.getStreetType();
		this.distanceFromStart.Add(currentNeighbour,cost);
		this.unvisitedNodes.Enqueue(currentNeighbour, cost);
		return 0;
	}
	///<summary>
	/// besucht den naechsten knoten
	///</summary>
	public int visitNextNode(Graph g)
	{
		this.visitedNodes.Add(this.currentNode);
		return 0;
	}
	///<summary>
	///waehlt den naechsten knoten anhand der kosten aus
	///</summary>
	public int chooseNextNode(Graph g)
	{
		this.currentNode = this.unvisitedNodes.Dequeue();
		return 0;
	}
}
