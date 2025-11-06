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

    private GraphNode[] currentNeighbours;
    private int currentNeighbourIndex;
    private float tentativeG;
    private float neighbourFScore;

    // implemented code:

    //     // addStartNode(graph);

    //     	while (unvisitedNodes.Count > 0)
    //     	{
    //         	getNewNodeFromQueue(graph);              // setzt currentNode

    //         	if (checkIfGoal(graph) == 1)             // Ziel erreicht?
    //             	return 1;

    //         	addNodeToVisited(graph);                 // current -> visited
    //         	heuristic(graph);                        // berechnet currentHeuristic (optional für Visualisierung)
		
	// 			foreach (GraphNode n in currentNode.getNeighbours())
	// 			{
	// 				if (visitedNodes.Contains(n))
	// 					continue;

	// 				float tentativeG = totalCost[currentNode] + edgeCost(currentNode, n);

	// 				if (tentativeG < totalCost[n])
	// 				{
	// 					totalCost[n] = tentativeG;
	// 					cameFrom[n] = currentNode;
	// 					float fScore = tentativeG + heuristicValue(n);
	// 					unvisitedNodes.Enqueue(n, fScore);
	// 				}
	// 			}
    //     	}

    public AlgorithmAStar(Graph graph, GraphNode startNode, GraphNode goalNode)
    {
        this.graph = graph;
        this.startNode = startNode;
        this.currentNode = startNode;
        this.goalNode = goalNode;

        cameFrom = [];
        totalCost = [];

        // Schritte-Array (nur Submethoden)
        this.steps = new Func<Graph, int>[] {
            initOpenList,
            initClosedList,
            initCosts,
            checkIfOpenListIsEmpty,
            getBestNodeFromQueue,
            addNodeToVisited,
            checkIfGoal,
            checkIfNeighbourVisited,
            calcTentativeG,
            checkIfTentativeIsBetter,
            updateNeighbourTotalCost,
            updateNeighbourCameFrom,
            calcNeighbourFScore,
            updateNeighbourFScore
        };
    }

    private int initOpenList(Graph g)
    {
        visitedNodes = [];

        return currentStep + 1;
    }

    private int initClosedList(Graph g)
    {
        unvisitedNodes = new PriorityQueue<GraphNode, float>();
        unvisitedNodes.Enqueue(startNode, heuristic(startNode));

        return currentStep + 1;
    }

    private int initCosts(Graph g)
    {
        foreach (GraphNode node in graph.getNodes())
        {
            totalCost[node] = float.MaxValue;
        }

        totalCost[startNode] = 0f;

        return currentStep + 1;
    }

    private int checkIfOpenListIsEmpty(Graph g)
    {
        if (unvisitedNodes.Count == 0)
        {
            // Error: no path found
            return -1;
        }

        return currentStep + 1;
    }

    private int getBestNodeFromQueue(Graph g)
    {
        currentNode = unvisitedNodes.Dequeue();
        currentDistance = totalCost[currentNode];

        return currentStep + 1;
    }

    private int addNodeToVisited(Graph g)
    {
        visitedNodes.Add(currentNode);

        return currentStep + 1;
    }

    private int checkIfGoal(Graph g)
    {
        if (currentNode.Equals(goalNode))
        {
            return -1;
        }

        // init for loop over each neighbour
        currentNeighbours = currentNode.getNeighbours();
        currentNeighbourIndex = 0;

        return currentStep + 1;
    }

    private int checkIfNeighbourVisited(Graph g)
    {
        if (visitedNodes.Contains(currentNeighbours[currentNeighbourIndex]))
        {
            currentNeighbourIndex++;
            return currentStep; // repeat this
        }

        return currentStep + 1;
    }

    private int calcTentativeG(Graph g)
    {
        tentativeG = totalCost[currentNode] + edgeCost(currentNode, currentNeighbours[currentNeighbourIndex]);

        return currentStep + 1;
    }

    private int checkIfTentativeIsBetter(Graph g)
    {
        if (tentativeG < totalCost[currentNeighbours[currentNeighbourIndex]])
        {

        }

        return 7; // checkIfNeighbourVisited-index
    }

    private int updateNeighbourTotalCost(Graph g)
    {
        totalCost[currentNeighbours[currentNeighbourIndex]] = tentativeG;

        return currentStep + 1;
    }

    private int updateNeighbourCameFrom(Graph g)
    {
        cameFrom[currentNeighbours[currentNeighbourIndex]] = currentNode;

        return currentStep + 1;
    }

    // F-Score = current distance to start + approximated distance to goal
    private int calcNeighbourFScore(Graph g)
    {
        neighbourFScore = tentativeG + heuristic(currentNeighbours[currentNeighbourIndex]);

        return this.currentStep + 1;
    }

    private int updateNeighbourFScore(Graph g)
    {
        unvisitedNodes.Enqueue(currentNeighbours[currentNeighbourIndex], neighbourFScore);

        return 7; // checkIfNeighbourVisited-index
    }


    private float heuristic(GraphNode node)
    {
        return getDistanceNodes(node, goalNode);
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