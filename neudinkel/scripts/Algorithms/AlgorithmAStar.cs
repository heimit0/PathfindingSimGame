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

		// Entferne alte Labels von vorherigen Instanzen
		removeAllLabels();

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
			( 2, "gScore[all nodes] = ∞"),
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
			initShowCost(node);
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
		currentNeighbours = currentNode.getNeighbouringCities(this.visitedNodes.ToArray());

		foreach (GraphNode n in currentNeighbours)
		{
			GD.Print("[" + currentNode.getTilePosString() + "] has neighbour: [" + n.getTilePosString() + "]");
		}

		currentNeighbourIndex = 0;

		return currentStep + 1;
	}

	private int neighbourForLoop(Graph g)
	{
		if (currentNeighbourIndex >= currentNeighbours.Length) return 4;

		return currentStep + 1;
	}

	// falls der derzeitig zu überprüfende nachbar schon besucht wurde,
	// überspringe diesen nachbarn => wiederhole diesen schritt mit dem nächsten nachbarn
	private int checkIfNeighbourVisited(Graph g)
	{

		if (visitedNodes.Contains(currentNeighbours[currentNeighbourIndex]))
		{
			currentNeighbourIndex++;
			return 8; // go to for loop
		}

		return currentStep + 1;
	}

	// errechne die direkten kosten vom start zum derzeitigen nachnarn nachbarn
	private int calcTentativeG(Graph g)
	{
		tentativeG = totalCost[currentNode] + edgeCost(currentNode, currentNeighbours[currentNeighbourIndex]);
		visualizeNeighbours(g,currentNeighbours[currentNeighbourIndex]);
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
		return 8; // checkIfNeighbourVisited-index
	}

	// überschreibe die gesamtkosten für den derzeitigen nachbarn
	private int updateNeighbourTotalCost(Graph g)
	{
		totalCost[currentNeighbours[currentNeighbourIndex]] = tentativeG;
		updateCost(currentNeighbours[currentNeighbourIndex],LabelMode.TOTALCOST,tentativeG);
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
		updateCost(currentNeighbours[currentNeighbourIndex],LabelMode.FSCORE,neighbourFScore);
		return this.currentStep + 1;
	}

	// füge den derzeitigen nachbarn zur PriorityQueue hinzu mit
	// dem gerade errechneten F-Score
	private int updateNeighbourFScore(Graph g)
	{
		unvisitedNodes.Enqueue(currentNeighbours[currentNeighbourIndex], neighbourFScore);
		Events.Instance.EmitSignal(Events.SignalName.PushOpenQueue, currentNeighbours[currentNeighbourIndex], neighbourFScore);

		currentNeighbourIndex++;
		return 8; // checkIfNeighbourVisited-index
	}

	// Heuristik für A* --> einfache Dreiecksberechnung
	private float heuristic(GraphNode node)
	{
		float heuristics = getDistanceNodes(node, goalNode);
		updateCost(node,LabelMode.HEURISTICS,heuristics);
		return heuristics;
	}

	private float getDistanceNodes(GraphNode node1, GraphNode node2)
	{
		float xDistance = node2.tilePosX - node1.tilePosX;
		float yDistance = node2.tilePosY - node1.tilePosY;
		float distance = Mathf.Sqrt(Mathf.Pow(xDistance, 2) + Mathf.Pow(yDistance, 2));
		return distance;
	}
	
	private void initShowCost(GraphNode node)
	{
		Label3D costLabel = new Label3D();
		costLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		costLabel.Text = "t: ∞";
		costLabel.Position = costLabel.Position + new Vector3(0f,0.9f,0f);
		costLabel.Name = "CostLabel";
		costLabel.RenderPriority = 100;
		costLabel.NoDepthTest = true;
		costLabel.FontSize = 20;
		costLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		node.AddChild(costLabel);
		Label3D fScoreLabel = new Label3D();
		fScoreLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		fScoreLabel.Text = "f: 0";
		fScoreLabel.Position = fScoreLabel.Position + new Vector3(0f,0.7f,0f);
		fScoreLabel.Name = "fScoreLabel";
		fScoreLabel.RenderPriority = 100;
		fScoreLabel.NoDepthTest = true;
		fScoreLabel.FontSize = 20;
		fScoreLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		node.AddChild(fScoreLabel);
		Label3D TentGLabel = new Label3D();
		TentGLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		TentGLabel.Text = "g: 0";
		TentGLabel.Position = TentGLabel.Position + new Vector3(0f,0.50f,0f);
		TentGLabel.Name = "TentGLabel";
		TentGLabel.RenderPriority = 100;
		TentGLabel.NoDepthTest = true;
		TentGLabel.FontSize = 20;
		TentGLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
		node.AddChild(TentGLabel);
	}
	private void updateCost(GraphNode node,LabelMode mode,float uValue)
	{
		Label3D uLabel = new Label3D();
		Label3D costLabel = new Label3D();
		Label3D fScoreLabel = new Label3D();
		Label3D TentGLabel = new Label3D();
		foreach (object obj in node.GetChildren())
		{
			if (obj.GetType() == typeof(Label3D))
			{
				uLabel = (Label3D)obj;
				switch(uLabel.Name)
				{
					case "TentGLabel":  TentGLabel = uLabel; break;
					case "fScoreLabel": fScoreLabel = uLabel;break;
					case "CostLabel": costLabel = uLabel;break;
					default: break;
				}
			}
		}
		switch(mode)
		{
			case (LabelMode)0: updateCostHeuristics(TentGLabel,uValue); break;
			case (LabelMode)1: updateCostFScore(fScoreLabel,uValue);break;
			case (LabelMode)2: updateCostTotalCost(costLabel,uValue);break;
			default: break;
		}
	}
	private void updateCostHeuristics(Label3D costLabel, float uValue)
	{
		costLabel.Text = "h: "+Math.Round(uValue,2).ToString();
	}
	private void updateCostFScore(Label3D costLabel, float uValue)
	{
		costLabel.Text = "f: "+Math.Round(uValue,2).ToString();
	}
	private void updateCostTotalCost(Label3D costLabel, float uValue)
	{
		costLabel.Text = "t: "+Math.Round(uValue,2).ToString();
	}
}
public enum LabelMode
{
HEURISTICS = 0,
FSCORE = 1,
TOTALCOST = 2,
};
