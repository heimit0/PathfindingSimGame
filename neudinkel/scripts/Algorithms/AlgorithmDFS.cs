using System;
using System.Collections.Generic;
using Godot;

public class AlgorithmDFS : NeudinkelAlgorithm
{

	private Queue<GraphNode> neighbourQueue;
	private GraphNode currentNeighbour; /* Zaehlvariable in der for loop */

	public AlgorithmDFS(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;
		this.startNode = startNode;
		this.currentNode = null;
		this.goalNode = goalNode;
		this.cameFrom = [];
		
		this.neighbourQueue = null;
		this.currentNeighbour = null;

		// Schritte-Array (nur Submethoden)
		this.steps = new Func<Graph, int>[] {
			/*  0 */ initUnvisitedNodes,
			/*  1 */ initVisitedNodes,
			/*  2 */ enqueueStartNode,
			/*  3 */ whileQueueNotEmpty,
			/*  4 */ getNodeFromQueue,
			/*  5 */ forEveryNeighbour,
			/*  6 */ checkIfIsGoal,
			/*  7 */ ifNodeUnvisited,
			/*  8 */ enqueueNeighbour,
			/*  9 */ markNeighbourVisited
		};

		this.pseudoCodeSteps = new[] {
			(0,"Initialisiere Queue der unbesuchten Knoten"),
			(1,"Initilialisiere List der besuchen Knoten"),
			(2,"Füge den ersten Knoten in die Queue der unbesuchten Knoten hinzu"),
			(3,"Wiederhole solange bis die Queue der unbesuchten Knoten leer ist"),
			(4,"     Entnehme einen Knoten aus der Queue der unbesuchten Knoten"),
			(5,"     Für jeden nachbarn des Knoten :"),
			(6,"          Überprüfe ob der aktuelle Knoten der Zielknoten ist, wenn ja breche ab"),
			(7,"          Falls der Knoten nicht in der Liste der besuchten Knoten ist:"),
			(8,"          Füge ihn in die Liste der unbesuchten Knoten hinzu"),
			(9,"    Füge den Knoten in die Liste der Besuchten Knoten hinzu")
		};

	}

	/* Schritt 1 */
	public int initUnvisitedNodes(Graph g)
	{
		this.unvisitedNodes = new PriorityQueue<GraphNode, float>();
		return this.currentStep + 1;
	}

	/* Schritt 2 */
	public int initVisitedNodes(Graph g)
	{
		this.visitedNodes = [];
		return this.currentStep + 1;
	}

	/* Schritt 3 */
	public int enqueueStartNode(Graph g)
	{
		this.unvisitedNodes.Enqueue(this.startNode, 0);
		Events.Instance.EmitSignal(Events.SignalName.PushOpenQueue, this.startNode, 0);
		return this.currentStep + 1;
	}

	/* Schritt 3 */
	public int whileQueueNotEmpty(Graph g)
	{
		if (this.unvisitedNodes.Count != 0)
			return this.currentStep + 1;
		else
			return -2;
	}

	/* Schritt 4 */
	public int getNodeFromQueue(Graph g)
	{
		this.currentNode = this.unvisitedNodes.Dequeue();
		Events.Instance.EmitSignal(Events.SignalName.PopOpenQueue);
		this.visualize(g);
		return this.currentStep + 1;
	}

	/* Schritt 5 */
	public int forEveryNeighbour(Graph g)
	{
		if (this.neighbourQueue == null)
		{
			// Neue Iteration
			this.neighbourQueue = new Queue<GraphNode>();
			foreach (GraphNode node in this.currentNode.getNeighboursUnsorted())
				this.neighbourQueue.Enqueue(node);
		}
		
		if (this.neighbourQueue.Count > 0)
		{
			this.currentNeighbour = this.neighbourQueue.Dequeue();
			return this.currentStep + 1;
		}
		else
		{
			this.neighbourQueue = null; /* vorbereiten auf naechste Iteration */
			return 3; /* whileQueueNotEmpty */ 
		}
	}

	/* Schritt 6 */
	public int checkIfIsGoal(Graph g)
	{
		if (this.currentNeighbour == this.goalNode)
		{
			this.cameFrom.Add(currentNeighbour, currentNode);
			return -1;
		}
		return this.currentStep + 1;
	}

	/* Schritt 7 */
	public int ifNodeUnvisited(Graph g)
	{
		if (!this.visitedNodes.Contains(this.currentNeighbour))
			return this.currentStep + 1;
		else
			return 5; /* forEveryNeighbour */
	}

	/* Schritt 8 */
	public int enqueueNeighbour(Graph g)
	{
		this.unvisitedNodes.Enqueue(this.currentNeighbour, 0);
		Events.Instance.EmitSignal(Events.SignalName.PushOpenQueue, this.currentNeighbour, 0);
		this.cameFrom.Add(currentNeighbour, currentNode);
		return this.currentStep + 1;
	}

	/* Schritt 9 */
	public int markNeighbourVisited(Graph g)
	{
		this.visitedNodes.Add(this.currentNeighbour);
		return 5; /* forEveryNeighbour */
	}

}
