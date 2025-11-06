using System;
using System.Collections.Generic;
using Godot;

public class AlgorithmAStar : NeudinkelAlgorithm, INeudinkelAlgorithm
{
	// jede Node zeigt auf ihren besten Nachbarn ("Elternteil"), um den besten Pfad zu rekonstruieren
	public Dictionary<GraphNode, GraphNode> cameFrom;

	// g(n) = bereits zurückgelegte Distanz (distanceFromStart)
	public Dictionary<GraphNode, float> totalCost;

	private float currentHeuristic;
	private float currentDistance;

	public AlgorithmAStar(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;
		this.startNode = startNode;
		this.currentNode = startNode;
		this.goalNode = goalNode;

		cameFrom = [];
		totalCost = [];

		// Schritte-Array (nur Submethoden)
		this.steps = new Func<Graph, int>[8];
		this.steps[0] = aStar;                 // orchestriert nur Submethoden
		this.steps[1] = initNodes;
		this.steps[2] = addStartNode;
		this.steps[3] = getNewNodeFromQueue;
		this.steps[4] = addNodeToVisited;
		this.steps[5] = checkIfGoal;
		this.steps[6] = addNeighboursToList;
        this.steps[7] = heuristic;

		initNodes(graph);
        // Wenn kein Pfad gefunden wird, aStar gibt 0 zurück -> Exception
        if (aStar(graph) == 0)
			throw new ArgumentException("Es existiert kein Weg zum Ziel");
	}

	private int aStar(Graph graph)
	{
		
		addStartNode(graph);

		while (unvisitedNodes.Count > 0)
		{
			getNewNodeFromQueue(graph);              // setzt currentNode
			
			if (checkIfGoal(graph) == 1)             // Ziel erreicht?
				return 1;

			addNodeToVisited(graph);                 // current -> visited
			heuristic(graph);                        // berechnet currentHeuristic (optional für Visualisierung)
			addNeighboursToList(graph);              // Nachbarn mit f(n) in PQ
		}

		return 0;
	}

	// Submethoden

	private int initNodes(Graph graph)
	{
		cameFrom.Clear();
		totalCost.Clear();
		visitedNodes = [];
		unvisitedNodes = new PriorityQueue<GraphNode, float>();

		foreach (GraphNode node in graph.getNodes())
			totalCost[node] = float.MaxValue;

		return this.currentStep + 1;
	}

	private int addStartNode(Graph graph)
	{
		currentNode = startNode;
		totalCost[startNode] = 0f;
		unvisitedNodes.Enqueue(startNode, heuristicValue(startNode));
		return 1;
	}

	private int getNewNodeFromQueue(Graph graph)
	{
		currentNode = unvisitedNodes.Dequeue();
		currentDistance = totalCost[currentNode];
		return 1;
	}

	private int addNodeToVisited(Graph graph)
	{
		visitedNodes.Add(currentNode);
		return 1;
	}

	private int checkIfGoal(Graph graph)
	{
		return (currentNode == goalNode) ? 1 : 0;
	}

	private int addNeighboursToList(Graph graph)
	{
		foreach (GraphNode n in currentNode.getNeighbours())
		{
			if (visitedNodes.Contains(n))
				continue;

			float tentativeG = totalCost[currentNode] + edgeCost(currentNode, n);
			
			if (tentativeG < totalCost[n])
			{
				totalCost[n] = tentativeG;
				cameFrom[n] = currentNode;
				float fScore = tentativeG + heuristicValue(n);
				unvisitedNodes.Enqueue(n, fScore);
			}
		}
		return 1;
	}

	private int heuristic(Graph graph)
	{
		currentHeuristic = heuristicValue(currentNode);
		return 1;
	}

	private float heuristicValue(GraphNode node)
	{
		return getDistanceNodes(node, goalNode); // euklidische Luftliniendistanz als zulässige Heuristik
	}

	private float edgeCost(GraphNode from, GraphNode to)
	{
		// Verwendet den Kantentyp des Zielpunkts als Kosten
		// Alternativ: Graph sollte Kantenwerte speichern
		return (float)to.getStreetType();
	}

	private float getDistanceNodes(GraphNode node1, GraphNode node2)
	{
		float xDistance = node2.getTilePosX() - node1.getTilePosX();
		float yDistance = node2.getTilePosY() - node1.getTilePosY();
		float distance = Mathf.Sqrt(Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2));
		return distance;
	}

    public void visualize(Graph g)
    {
        throw new NotImplementedException();
    }
}