using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class AlgorithmAStar : NeudinkelAlgorithm
{
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
		this.currentNode = null;
		this.goalNode = goalNode;

		cameFrom = [];
		totalCost = [];

		// Schritte-Array (nur Submethoden)
		this.steps = new Func<Graph, int>[] {
			/*  0 */ initUnvisitedNodes,
			/*  1 */ initVisitedNodes,
			/*  2 */ initCosts,
			/*  3 */ checkIfOpenListIsEmpty,
			/*  4 */ getBestNodeFromQueue,
			/*  5 */ addNodeToVisited,
			/*  6 */ checkIfGoal,
			/*  7 */ neighbourForLoop,
			/*  8 */ checkIfNeighbourVisited,
			/*  9 */ calcTentativeG,
			/* 10 */ checkIfTentativeIsBetter,
			/* 11 */ updateNeighbourTotalCost,
			/* 12 */ updateNeighbourCameFrom,
			/* 13 */ calcNeighbourFScore,
			/* 14 */ updateNeighbourFScore
		};

		this.pseudoCodeSteps = new[] {
			( 0, "unvisited = PriorityQueue; // (node, priority)"),
			( 0, "unvisited.add(startNode, heuristic(startNode));"),
			( 1, "visited = empty list;"),
			( 2, "g(all nodes) = ∞;"),
			( 2, "g(startNode) = 0;"),
			( 3, "while unvisited has entries {"),
			( 4, "    currentNode = unvisited.getBest();"),
			( 5, "    visited.add(currentNode);"),
			( 6, "    if currentNode == goalNode {"),
			(-1, "        ende: \"ziel erreicht!\";"),
			( 6, "    }"),
			( 7, "    foreach neighbour of currentNode {"),
			( 8, "        if neighbour is visited {"),
			( 8, "            skip this neighbour;"),
			( 8, "        }"),
			( 9, "        tentative_g = g(currentNode) + d(currentNode, neighbour);"),
			(10, "        if tentative_g < g(neighbour {"),
			(11, "            g(neighbour) = tentative_g;"),
			(12, "            cameFrom(neighbour) = currentNode;"),
			(13, "            f(neighbour) = g(neighbour) + h(neighbour);"),
			(14, "            unvisited.add(neighbour, f(neighbour))"),
			(10, "        }"),
			( 7, "    }"),
			( 3, "}"),
			(-2, "ende: \"kein weg gefunden!\";")
		};
	}

	// PriorityQueue(node, approximated_distance) mit der Startnode direkt in der queue
	// diese wird nach und nach mit nachbarn gefüllt
	private int initUnvisitedNodes(Graph g)
	{
		unvisitedNodes = new PriorityQueue<GraphNode, float>();
		unvisitedNodes.Enqueue(startNode, heuristic(startNode));
		GD.Print("A Star innit!");

		return currentStep + 1;
	}

	// die bereits besuchten nodes und
	// elternbeziehungen (um den besten pfad am ende zu bilden)
	private int initVisitedNodes(Graph g)
	{
		visitedNodes = [];
		cameFrom = [];

		return currentStep + 1;
	}

	// die kosten (distanzen) für alle nodes werden am anfang auf unendlich gesetzt,
	// außer für die Startnode
	private int initCosts(Graph g)
	{
		foreach (GraphNode node in graph.getCities())
		{
			totalCost[node] = float.MaxValue;
		}

		totalCost[startNode] = 0f;

		return currentStep + 1;
	}

	// while loop condition: gehe solange bis es keine nodes zu besuchen mehr gibt
	// (außer es wird vorher der pfad gefunden)
	private int checkIfOpenListIsEmpty(Graph g)
	{
		if (unvisitedNodes.Count == 0)
		{
			// Error: no path found
			return -2;
		}

		return currentStep + 1;
	}

	// hole die derzeit günstigste node aus der PriorityQueue
	// um sie zu verarbeiten
	private int getBestNodeFromQueue(Graph g)
	{
		GD.Print("step 4 pwease");
		GraphNode tmp = currentNode != null ? currentNode : new GraphNode();
		lastNode = currentNode;
		currentNode = unvisitedNodes.Dequeue();
		GD.Print("from: [" + tmp.getTilePosString() + "] to: [" + currentNode.getTilePosString() + "]");
		currentDistance = totalCost[currentNode];

		visualize(g);

		return currentStep + 1;
	}

	// füge die derzeitige node zu den besuchten hinzu
	private int addNodeToVisited(Graph g)
	{
		visitedNodes.Add(currentNode);

		return currentStep + 1;
	}

	// win condition: falls die derzeitige node die zielnode ist
	private int checkIfGoal(Graph g)
	{
		if (currentNode.Equals(goalNode))
		{
			return -1; // pfad gefunden
		}

		// get next Knoten
		currentNeighbours = currentNode.getNeighbouringCities(new List<GraphNode>());

		foreach (GraphNode n in currentNeighbours)
		{
			GD.Print("[" + currentNode.getTilePosString() + "] has neighbour: [" + n.getTilePosString() + "]");
		}

		currentNeighbourIndex = 0;

		return currentStep + 1;
	}

	private int neighbourForLoop(Graph g)
	{
		if (currentNeighbourIndex >= currentNeighbours.Length) return 3;

		return currentStep + 1;
	}

	// falls der derzeitig zu überprüfende nachbar schon besucht wurde,
	// überspringe diesen nachbarn => wiederhole diesen schritt mit dem nächsten nachbarn
	private int checkIfNeighbourVisited(Graph g)
	{

		if (visitedNodes.Contains(currentNeighbours[currentNeighbourIndex]))
		{
			currentNeighbourIndex++;
			return 7; // go to for loop
		}

		return currentStep + 1;
	}

	// errechne die direkten kosten vom start zum derzeitigen nachnarn nachbarn
	private int calcTentativeG(Graph g)
	{
		tentativeG = totalCost[currentNode] + edgeCost(currentNode, currentNeighbours[currentNeighbourIndex]);

		return currentStep + 1;
	}

	// überprüfe, ob die errechneten kosten zum derzeitigen nachbarn besser sind,
	// als die vorgespeicherten nachbarn
	private int checkIfTentativeIsBetter(Graph g)
	{
		// falls ja, gehe weiter
		if (tentativeG < totalCost[currentNeighbours[currentNeighbourIndex]])
		{
			return currentStep + 1;
		}

		// falls nein, gehe zum nächsten nachbarn
		currentNeighbourIndex++;
		return 7; // checkIfNeighbourVisited-index
	}

	// überschreibe die gesamtkosten für den derzeitigen nachbarn
	private int updateNeighbourTotalCost(Graph g)
	{
		totalCost[currentNeighbours[currentNeighbourIndex]] = tentativeG;

		return currentStep + 1;
	}

	// überschreibe das elternteil des derzeitigen nachbarns mit der derzeitigen node,
	// da die gesamtkosten hier billiger sind
	private int updateNeighbourCameFrom(Graph g)
	{
		cameFrom[currentNeighbours[currentNeighbourIndex]] = currentNode;

		return currentStep + 1;
	}

	// F-Score = current distance to start + approximated distance to goal
	//
	// errechne die geschätzten gesamtkosten zum ziel für den derzeitigen nachbarn
	private int calcNeighbourFScore(Graph g)
	{
		neighbourFScore = tentativeG + heuristic(currentNeighbours[currentNeighbourIndex]);

		return this.currentStep + 1;
	}

	// füge den derzeitigen nachbarn zur PriorityQueue hinzu mit
	// dem gerade errechneten F-Score
	private int updateNeighbourFScore(Graph g)
	{
		unvisitedNodes.Enqueue(currentNeighbours[currentNeighbourIndex], neighbourFScore);

		currentNeighbourIndex++;
		return 7; // checkIfNeighbourVisited-index
	}

	// Heuristik für A* --> einfache Dreiecksberechnung
	private float heuristic(GraphNode node)
	{
		return getDistanceNodes(node, goalNode);
	}

	private float edgeCost(GraphNode from, GraphNode to)
	{
		// falls to = direkter Nachbar
		foreach (GraphNode n in from.getNeighboursUnsorted())
		{
			if (n.Equals(to)) return (float) to.getStreetType();
		}

		float cost = 0.0f;

		foreach (GraphNode n in from.getPathToNeighbouringCity(to))
		{
			cost += (float) n.getStreetType();
		}

		return cost;
	}

	private float getDistanceNodes(GraphNode node1, GraphNode node2)
	{
		float xDistance = node2.getTilePosX() - node1.getTilePosX();
		float yDistance = node2.getTilePosY() - node1.getTilePosY();
		float distance = Mathf.Sqrt(Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2));
		return distance;
	}
}
