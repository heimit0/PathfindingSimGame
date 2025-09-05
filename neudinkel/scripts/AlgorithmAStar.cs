using System;
using System.Collections.Generic;
using Godot;

public class AlgorithmAStar : NeudinkelAlgorithm
{

	// jede Node zeigt auf ihren besten Nachbarn ("Elternteil"), um den besten Pfad zu rekonstruieren
	public Dictionary<GraphNode, GraphNode> cameFrom;
	// f(n) = distanceFromStart + heuristic
	public Dictionary<GraphNode, float> totalCost;
	
	// TODO(gfriedr3 oder hsimosse): Implementieren (Siehe Erklärungen NeudinkelAlgorithm.cs)
	public AlgorithmAStar(Graph graph, GraphNode startNode, GraphNode goalNode)

	/*
		Algorithmus: A*
		 - openSet / openList => unvisitedNodes
		 - closedSet / closedList => visitedNodes
		 - cameFrom => cameFrom
		 - g(n) => distanceFromStart
		 - f(n) => totalCost
	*/
    public AlgorithmAStar(GraphSearchState state, Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;

        this.state.startNode = startNode;
		this.state.currentNode = startNode;
		this.state.goalNode = goalNode;
		this.state.unvisitedNodes.Enqueue(startNode, heuristic(startNode, goalNode));

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
        return getDistanceNodes(startNode, goalNode);
    }


	/// <summary>
	/// Berechnet die Distanz zwischen zwei Knoten
	/// </summary>
	/// <param name="node1"></param>
	/// <param name="node2"></param>
	/// <returns>float Distance</returns>
	public float getDistanceNodes(GraphNode node1, GraphNode node2){
        float xDistance = node2.getTilePosX() - node1.getTilePosX();
        float yDistance = node2.getTilePosY() - node1.getTilePosY();
        float distance = Mathf.Sqrt(Mathf.Pow(xDistance,2)+Mathf.Pow(yDistance,2));
        return distance;
    }



	
}
