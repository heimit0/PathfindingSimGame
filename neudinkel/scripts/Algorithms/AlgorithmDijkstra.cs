using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

public class AlgorithmDijkstra : NeudinkelAlgorithm, INeudinkelAlgorithm
{

    private float cost;
    public Dictionary<GraphNode, GraphNode> cameFrom;

    private GraphNode[] currentNeighbours;
    private int currentNeighbourIndex;
    private float currentNeighbourCost;

    public AlgorithmDijkstra(Graph graph, GraphNode startNode, GraphNode goalNode)
    {
        this.graph = graph;

        this.startNode = startNode;
        this.currentNode = startNode;
        this.goalNode = goalNode;
        this.cameFrom = new Dictionary<GraphNode, GraphNode>();

        this.steps = new Func<Graph, int>[] {
			initUnvisitedNodes,
			initVisitedNodes,
			initCosts,
			checkIfOpenListIsEmpty,
			getBestNodeFromQueue,
			addNodeToVisited,
			checkIfGoal,
			calculteDistance,
			checkIfDistanceIsKnown,
			checkForBetterDistance,
			updateNeighbourCost,
			updateNeighbourCameFrom,
			addNeighbourParent,
			addNeighbourCost,
			addNeighbourToQueue
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

        distanceFromStart[startNode] = 0f;

        return currentStep + 1;
    }

    // while loop condition: gehe solange bis es keine nodes zu besuchen mehr gibt
    // (außer es wird vorher der pfad gefunden)
    private int checkIfOpenListIsEmpty(Graph g)
    {
        if (unvisitedNodes.Count == 0)
        {
            // Error: no path found
            return -1;
        }

        return currentStep + 1;
    }

    // hole die derzeit günstigste node aus der PriorityQueue
    // um sie zu verarbeiten
    private int getBestNodeFromQueue(Graph g)
    {
        currentNode = unvisitedNodes.Dequeue();

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
            return -1;
        }

        // falls die derzeitige node nicht die zielnode ist,
        // initialisiere die for loop über den nachbarn der derzeitigen node
        currentNeighbours = currentNode.getNeighbours();
        currentNeighbourIndex = 0;

        return currentStep + 1;
    }

    // errechne die direkten kosten vom start zum derzeitigen nachnarn nachbarn
    private int calculteDistance(Graph g)
    {
        if (currentNeighbourIndex > currentNeighbours.Length) return -1;

        currentNeighbourCost = distanceFromStart[currentNode] + edgeCost(currentNode, currentNeighbours[currentNeighbourIndex]);

        return currentStep + 1;
    }

	// überprüfe, ob eine distanz zu dieser node bereits bekannt ist
    private int checkIfDistanceIsKnown(Graph g)
    {
        if (distanceFromStart.ContainsKey(currentNeighbours[currentNeighbourIndex]))
        {
            return 9; // check for better distance
        }

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
        distanceFromStart.Add(currentNeighbours[currentNeighbourIndex], currentNeighbourCost);

        return currentStep + 1;
    }

	// füge den derzeitigen nachbarn zur PriorityQueue hinzu, um ihn später zu überprüfen
    private int addNeighbourToQueue(Graph g)
    {
        unvisitedNodes.Enqueue(currentNeighbours[currentNeighbourIndex], currentNeighbourCost);

        currentNeighbourIndex++;
        return 7;
    }

    private float edgeCost(GraphNode from, GraphNode to)
    {
        // Verwendet den Kantentyp des Zielpunkts als Kosten
        // Alternativ: Graph sollte Kantenwerte speichern
        return (float)to.getStreetType();
    }


    public void visualize(Graph g)
    {
        throw new NotImplementedException();
    }
}
