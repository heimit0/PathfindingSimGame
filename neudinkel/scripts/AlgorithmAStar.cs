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
	{
		this.graph = graph;

		this.startNode = startNode;
		this.currentNode = startNode;
		this.goalNode = goalNode;

		cameFrom = [];
		totalCost = [];

		

		// alle nodes mit distanz = unendlich initialisieren
		// ist der F-score
		foreach (GraphNode node in graph.getNodes()) {
			this.totalCost.Add(node, float.MaxValue);
		}


		totalCost[startNode] = 0;
		this.unvisitedNodes.Enqueue(startNode, heuristic(startNode));

		this.steps = new Func<Graph, int>[10]; // idk wie viele
		// schritte funktionen zuordnen

		while(unvisitedNodes.Count>0)
		{
			GraphNode lastNode = currentNode;
			currentNode = unvisitedNodes.Dequeue();
			visitedNodes.Add(currentNode);
			totalCost[currentNode] = totalCost[lastNode] + (int) lastNode.getStreetType();
			cameFrom.Add(currentNode, lastNode);
			if(currentNode == goalNode){
				return;
			}
			else
			{
				foreach(GraphNode n in currentNode.getNeighbours())
				{
					if(!visitedNodes.Contains(n))
					{
						unvisitedNodes.Enqueue(n, heuristic(n));
					}
				}
			}

		}
		throw new ArgumentException("Es existiert kein Weg zum Ziel");


	}





/// <summary>
/// Erstellt eine heuristik zu der Distanz und dem bereits zurueckgelegtem Weg 
/// </summary>
/// <param name="startNode"></param>
/// <returns></returns>
private float heuristic(GraphNode startNode)
{
	float cost = totalCost[startNode];
	float straightLineDistance = getDistanceNodes(startNode, goalNode);
	return cost + straightLineDistance;
}

	/// <summary>
/// Berechnet die Distanz zwischen zwei Knoten
/// </summary>
/// <param name="node1"></param>
/// <param name="node2"></param>
/// <returns>float Distance</returns>
private float getDistanceNodes(GraphNode node1, GraphNode node2)
{
	float xDistance = node2.getTilePosX() - node1.getTilePosX();
	float yDistance = node2.getTilePosY() - node1.getTilePosY();
	float distance = Mathf.Sqrt(Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2));
	return distance;
}




	}
