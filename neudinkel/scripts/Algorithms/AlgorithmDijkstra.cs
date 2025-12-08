using System;
using System.Collections.Generic;
using System.Linq;
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

		removeAllLabels();

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
		}; // TODO kann verbessert werden

		this.pseudoCodeSteps = new[] {
			(-10, "     DIJKSTRA"),
			( 0, "unbesuchteKnoten = new PriorityQueue()"),
			( 1, "besuchteKnoten = new List()"),
			( 2, "distance[all nodes] = ∞"),
			( 3, "unbesuchteKnoten.add(startNode, 0)"),
			( 4, "distance[startNode] = 0"),
			( 4, "while (!unbesuchteKnoten.isEmpty()):"),
			( 5, "    current = unbesuchteKnoten.dequeue()"),
			( 6, "    besuchteKnoten.add(current)"),
			( 7, "    if (current == targetNode):"),
			(-1, "        Weg zu Ziel gefunden"),
			( 7, "    "),
			( 8, "    for (neighbor in current.neighbors):"),
			( 9, "        tentativeDist = distance[current] + edgeCost(current, neighbor)"),
			(10, "        if (distance[neighbor] != ∞):"),
			(11, "            if (tentativeDist < distance[neighbor]):"),
			(12, "                distance[neighbor] = tentativeDist"),
			(13, "                cameFrom[neighbor] = current"),
			(11, "        "),
			(14, "        else:"),
			(15, "            cameFrom[neighbor] = current"),
			(16, "            distance[neighbor] = tentativeDist"),
			(17, "            unbesuchteKnoten.add(neighbor, tentativeDist)"),
			( 8, "    "),
			( 4, ""),
			(-2, "Kein Weg gefunden")
		};

	}

    private void removeAllLabels()
    {
        if (graph == null) return;

		foreach (GraphNode node in graph.getCities())
		{
			// Sammle alle Labels, die entfernt werden sollen
			List<Label3D> labelsToRemove = new List<Label3D>();

			foreach (Node child in node.GetChildren())
			{
				if (child is Label3D label)
				{
					if (label.Name == "CostLabel" || label.Name == "fScoreLabel" || label.Name == "TentGLabel")
					{
						labelsToRemove.Add(label);
					}
				}
			}

			// Entferne alle gefundenen Labels
			foreach (Label3D label in labelsToRemove)
			{
				node.RemoveChild(label);
				label.QueueFree();
			}
		}
    }

    // PriorityQueue(node, calculatedDistance) mit der Startnode direkt in der queue
    // diese wird nach und nach mit nachbarn gefüllt
    private int initUnvisitedNodes(Graph g)
	{
		unvisitedNodes = new PriorityQueue<GraphNode, float>();
		unvisitedNodes.Enqueue(startNode, 0);
		// Events.Instance.EmitSignal(Events.SignalName.PushOpenQueue, startNode, 0);

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
		updateCost(startNode);
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
		foreach (GraphNode node in graph.getCities())
		{
			distanceFromStart[node] = float.MaxValue;
			showCost(node);
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
		lastNode = currentNode;
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
		currentNeighbours = currentNode.getNeighbouringCities(this.visitedNodes.ToArray());
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
		if (currentNeighbourIndex >= currentNeighbours.Length) return 5; // exit for-loop, jz normaler durchlauf
	
		GD.Print("distance from start to " + currentNode.getTilePosString() + " is: " + distanceFromStart[currentNode]);
		currentNeighbourCost = distanceFromStart[currentNode] + edgeCost(currentNode, currentNeighbours[currentNeighbourIndex]);
		
		visualizeNeighbours(g,currentNeighbours[currentNeighbourIndex]);

		return currentStep + 1;
	}

	// überprüfe, ob eine distanz zu dieser node bereits bekannt ist
	private int checkIfDistanceIsKnown(Graph g)
	{
		if (distanceFromStart[currentNeighbours[currentNeighbourIndex]] != float.MaxValue)
		{
			GD.Print("Distance from start: [" + currentNeighbours[currentNeighbourIndex].getTilePosString() + "] is known!");
			return 11; // check for better distance
		}

		GD.Print("Distance from start: [" + currentNeighbours[currentNeighbourIndex].getTilePosString() + "] is unknown! adding it...");
		return 16; // add node to lists
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
		return 9;
	}

	// falls die derzeitige route besser ist, update den nachbarn
	private int updateNeighbourCost(Graph g)
	{
		distanceFromStart[currentNeighbours[currentNeighbourIndex]] = currentNeighbourCost;
		updateCost(currentNeighbours[currentNeighbourIndex]);
		return currentStep + 1;
	}

	// überschreibe das elternteil des derzeitigen nachbarns mit der derzeitigen node,
	// da die gesamtkosten hier billiger sind
	private int updateNeighbourCameFrom(Graph g)
	{
		cameFrom[currentNeighbours[currentNeighbourIndex]] = currentNode;

		currentNeighbourIndex++;
		return 9;
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
		updateCost(currentNeighbours[currentNeighbourIndex]);
		return currentStep + 1;
	}

	// füge den derzeitigen nachbarn zur PriorityQueue hinzu, um ihn später zu überprüfen
	private int addNeighbourToQueue(Graph g)
	{
		distanceFromStart[currentNeighbours[currentNeighbourIndex]] = currentNeighbourCost;
		updateCost(currentNeighbours[currentNeighbourIndex]);
		unvisitedNodes.Enqueue(currentNeighbours[currentNeighbourIndex], currentNeighbourCost);
		GD.Print("neighbourcost: " + currentNeighbourCost);
		Events.Instance.EmitSignal(Events.SignalName.PushOpenQueue, currentNeighbours[currentNeighbourIndex], currentNeighbourCost);

		currentNeighbourIndex++;
		return 9;
	}
	
	private void showCost(GraphNode node)
	{
		float cost = this.distanceFromStart[node];
		Label3D costLabel = new Label3D();
		costLabel.Position = costLabel.Position + new Vector3(0f,0.5f,0f);
		costLabel.RenderPriority = 100;
		costLabel.NoDepthTest = true;
		costLabel.FontSize = 20;
		costLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		if(cost < 1000000f)
		{
			costLabel.Text = cost.ToString();
		}
		else
		{
			costLabel.Text = "∞";
		}
		
		
		costLabel.Name = "CostLabel";
		node.AddChild(costLabel);
	}
	private void updateCost(GraphNode node)
	{
		Label3D costLabel = new Label3D();
		float cost = this.distanceFromStart[node];
		foreach (object obj in node.GetChildren())
		{
			if (obj.GetType() == typeof(Label3D))
			{
				costLabel = (Label3D)obj;
			}
		}
		costLabel.Text = cost.ToString();
	}
}
