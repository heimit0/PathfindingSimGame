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

		this.breakpoint = 5; /* addNodeToVisisted */

		cameFrom = [];
		totalCost = [];

		// Schritte-Array (nur Submethoden)
		this.steps = new Func<Graph, int>[] {
			/*  0 */ initUnvisitedNodes,
			/*  1 */ initVisitedNodes,
			/*  2 */ initCosts,
			/*  3 */ addStartNodeToQueue,
			/*  4 */ checkIfOpenListIsEmpty,
			/*  5 */ getBestNodeFromQueue,
			/*  6 */ addNodeToVisited,
			/*  7 */ checkIfGoal,
			/*  8 */ neighbourForLoop,
			/*  9 */ checkIfNeighbourVisited,
			/* 10 */ calcTentativeG,
			/* 11 */ checkIfTentativeIsBetter,
			/* 12 */ updateNeighbourTotalCost,
			/* 13 */ updateNeighbourCameFrom,
			/* 14 */ calcNeighbourFScore,
			/* 15 */ updateNeighbourFScore
		};

		this.pseudoCodeSteps = new[] {
			( 0, "unbesuchteKnoten = new PriorityQueue()"),
			( 1, "besuchteKnoten = new List()"),
			( 2, "gScore[all nodes] = \\U+221E"),
			( 3, "unbesuchteKnoten.add(startNode)"),
			( 3, "gScore[startNode] = 0"),
			( 3, "while (!unbesuchteKnoten.isEmpty()):"),
			( 4, "    current = unbesuchteKnoten.dequeue()"),
			( 5, "    besuchteKnoten.add(current)"),
			( 6, "    if (current == targetNode):"),
			(-1, "        Weg zu Ziel gefunden"),
			( 6, "    "),
			( 7, "    for (neighbor in current.neighbors):"),
			( 8, "        if (besuchteKnoten.contains(neighbor)):"),
			( 8, "            continue"),
			( 8, "        "),
			( 9, "        tentativeGScore = gScore[current] + distance(current, neighbor)"),
			(10, "        if (tentativeGScore < gScore[neighbor]):"),
			(11, "            gScore[neighbor] = tentativeGScore"),
			(12, "            cameFrom[neighbor] = current"),
			(13, "            fScore[neighbor] = gScore[neighbor] + heuristic(neighbor, target)"),
			(14, "            unbesuchteKnoten.add(neighbor, fScore[neighbor])"),
			(10, "        "),
			( 7, "    "),
			( 3, ""),
			(-2, "Kein Weg gefunden")
		};
	}

	// PriorityQueue(node, approximated_distance) mit der Startnode direkt in der queue
	// diese wird nach und nach mit nachbarn gefüllt
	private int initUnvisitedNodes(Graph g)
	{
		unvisitedNodes = new PriorityQueue<GraphNode, float>();
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

	private int addStartNodeToQueue(Graph g)
	{
		unvisitedNodes.Enqueue(startNode, heuristic(startNode));
		totalCost[startNode] = 0f;
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
		Events.Instance.EmitSignal(Events.SignalName.PopOpenQueue);
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
		Events.Instance.EmitSignal(Events.SignalName.PushOpenQueue, currentNeighbours[currentNeighbourIndex], neighbourFScore);

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
