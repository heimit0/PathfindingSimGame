using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Godot;

public class AlgorithmDijkstra : NeudinkelAlgorithm
{

	private float cost;
	private GraphNode[] currentNeighbours;
	private int currentNeighbourIndex;
	private float currentNeighbourCost;

	public AlgorithmDijkstra(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;

		this.startNode = startNode;
		this.currentNode = null;
		this.goalNode = goalNode;
		this.cameFrom = new Dictionary<GraphNode, GraphNode>();
		this.distanceFromStart = new Dictionary<GraphNode, float>();

		this.steps = new Func<Graph, int>[] {
			/*  0 */ initUnvisitedNodes,
			/*  1 */ initVisitedNodes,
			/*  2 */ initCosts,
			/*  3 */ addStartNodeToQueue,
			/*  4 */ setStartNodeValueToZero,
			/*  5 */ checkIfOpenListIsEmpty,
			/*  6 */ getBestNodeFromQueue,
			/*  7 */ addNodeToVisited,
			/*  8 */ checkIfGoal,
			/*  9 */ calculteDistance,
			/* 10 */ checkIfDistanceIsKnown,
			/* 11 */ checkForBetterDistance,
			/* 12 */ updateNeighbourCost,
			/* 13 */ updateNeighbourCameFrom,
			/* 14 */ addNeighbourParent,
			/* 15 */ addNeighbourCost,
			/* 16 */ addNeighbourToQueue
		}; //TODO kann verbessert werden

		this.pseudoCodeSteps = new[] {
			(0,"Initialisiere Priority Queue der unbesuchten Knoten"),
			(1,"Initialisere Liste der besuchten Knoten"),
			(2,"Setze die Kosten aller Knoten auf \\U+221E"),
			(3,"Füge Startknoten zur Queue hinzu"),
			(4,"Setzte den Wert des Startknoten auf Null"),
			(5,"Während die Priority Queue nicht leer ist:"),
			(6,"     Nehme den Knoten mit dem geringsten Kosten aus der Queue"),
			(7,"     Füge diesen Knoten in die Liste der Besuchten Knoten hinzu"),
			(8,"     Überprüfe ob dieser Knoten der Zielknoten ist, wenn ja beende den Algorithmus"),
			(9,"     Berechne die Distanz vom Start zu diesem Knoten"),
			(10,"     Überprüfe ob die Distanz zu diesem Nachbarn bereits bekannt ist"),
			(11,"     Falls bekannt: überprüfe ob die neue Distanz besser ist"),
			(12,"     Aktualisiere die Kosten des Nachbarn"),
			(13,"     Aktualisiere den Vorgänger des Nachbarn"),
			(14,"     Füge das Elternteil des Nachbarn hinzu"),
			(15,"     Füge die Kosten des Nachbarn hinzu"),
			(16,"     Füge den Nachbarn zur Priority Queue hinzu")

		};

	}


	// PriorityQueue(node, calculatedDistance) mit der Startnode direkt in der queue
	// diese wird nach und nach mit nachbarn gefüllt
	private int initUnvisitedNodes(Graph g)
	{
		unvisitedNodes = new PriorityQueue<GraphNode, float>();
		unvisitedNodes.Enqueue(startNode, 0);

		return currentStep + 1;
	}

	private int addStartNodeToQueue(Graph g)
	{
		Events.Instance.EmitSignal(Events.SignalName.PushOpenQueue, startNode, 0);
		return currentStep +1;
	}

	private int setStartNodeValueToZero(Graph g)
	{
		distanceFromStart[startNode] = 0f;

		return currentStep +1;
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
		foreach (GraphNode node in graph.getNodes())
		{
			distanceFromStart[node] = float.MaxValue;
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
		GraphNode tmp = currentNode != null ? currentNode : new GraphNode();
		currentNode = unvisitedNodes.Dequeue();
		Events.Instance.EmitSignal(Events.SignalName.PopOpenQueue);
		GD.Print("from: [" + tmp.getTilePosString() + "] to: [" + currentNode.getTilePosString() + "]");

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
			// Weg gefunden
			return -1;
		}

		// falls die derzeitige node nicht die zielnode ist,
		// initialisiere die for loop über den nachbarn der derzeitigen node
		currentNeighbours = currentNode.getNeighboursUnsorted();
		currentNeighbourIndex = 0;

		foreach (GraphNode n in currentNeighbours)
		{
			GD.Print("[" + currentNode.getTilePosString() + "] has neighbour: [" + n.getTilePosString() + "]");
		}

		return currentStep + 1;
	}

	// errechne die direkten kosten vom start zum derzeitigen nachnarn nachbarn
	// for-loop start
	private int calculteDistance(Graph g)
	{
		if (currentNeighbourIndex >= currentNeighbours.Length) return 3; // exit for-loop, jz normaler durchlauf
	
		currentNeighbourCost = distanceFromStart[currentNode] + edgeCost(currentNode, currentNeighbours[currentNeighbourIndex]);

		return currentStep + 1;
	}

	// überprüfe, ob eine distanz zu dieser node bereits bekannt ist
	private int checkIfDistanceIsKnown(Graph g)
	{
		if (distanceFromStart[currentNeighbours[currentNeighbourIndex]] != float.MaxValue)
		{
			GD.Print("Distance from start: [" + currentNeighbours[currentNeighbourIndex].getTilePosString() + "] is known!");
			return 9; // check for better distance
		}

		GD.Print("Distance from start: [" + currentNeighbours[currentNeighbourIndex].getTilePosString() + "] is unknown! adding it...");
		return 12; // add node to lists
	}

	// falls die node bekannt ist, überprüfe, ob die
	// derzeitige route besser ist
	private int checkForBetterDistance(Graph g)
	{
		if (currentNeighbourCost < distanceFromStart[currentNeighbours[currentNeighbourIndex]])
		{
			return currentStep + 1;
		}

		// gehe zum nächsten nachbarn
		currentNeighbourIndex++;
		return 7;
	}

	// falls die derzeitige route besser ist, update den nachbarn
	private int updateNeighbourCost(Graph g)
	{
		distanceFromStart[currentNeighbours[currentNeighbourIndex]] = currentNeighbourCost;

		return currentStep + 1;
	}

	// überschreibe das elternteil des derzeitigen nachbarns mit der derzeitigen node,
	// da die gesamtkosten hier billiger sind
	private int updateNeighbourCameFrom(Graph g)
	{
		cameFrom[currentNeighbours[currentNeighbourIndex]] = currentNode;

		currentNeighbourIndex++;
		return 7;
	}

	// füge den nachbarn zur elternbeziehungsliste hinzu
	private int addNeighbourParent(Graph g)
	{
		cameFrom.Add(currentNeighbours[currentNeighbourIndex], currentNode);

		return currentStep + 1;
	}

	// füge die kosten zum derzeitigen nachbarn hinzu
	private int addNeighbourCost(Graph g)
	{
		// nur update, weil alle schon initialisiert sind (infinity)
		distanceFromStart[currentNeighbours[currentNeighbourIndex]] = currentNeighbourCost;

		return currentStep + 1;
	}

	// füge den derzeitigen nachbarn zur PriorityQueue hinzu, um ihn später zu überprüfen
	private int addNeighbourToQueue(Graph g)
	{
		unvisitedNodes.Enqueue(currentNeighbours[currentNeighbourIndex], currentNeighbourCost);
		Events.Instance.EmitSignal(Events.SignalName.PushOpenQueue, currentNeighbours[currentNeighbourIndex], currentNeighbourCost);

		currentNeighbourIndex++;
		return 7;
	}

	private float edgeCost(GraphNode from, GraphNode to)
	{
		// Verwendet den Kantentyp des Zielpunkts als Kosten
		// Alternativ: Graph sollte Kantenwerte speichern
		return (float)to.getStreetType();
	}
}
