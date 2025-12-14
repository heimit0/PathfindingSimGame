using System;
using System.Collections.Generic;
using System.Linq;
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

		this.breakpoint = 4;

		// Schritte-Array (nur Submethoden)
		this.steps = new Func<Graph, int>[] {
			/*  0 */ initUnvisitedNodes,
			/*  1 */ initVisitedNodes,
			/*  2 */ enqueueStartNode,
			/*  3 */ whileQueueNotEmpty,
			/*  4 */ getNodeFromQueue,
			/*  5 */ checkIfIsGoal,
			/*  6 */ forEveryNeighbour,
			/*  7 */ ifNodeUnvisited,
			/*  8 */ enqueueNeighbour,
			/*  9 */ markNeighbourVisited
		};

		this.pseudoCodeSteps = new[] {
			( 0, "unvisited := Stack"),
			( 1, "visited := List"),
			( 2, "add startnode to unvisited"),
			( 3, "while unvisited is not empty do"),
			( 4, "    current := pop from unvisited"),
			( 5, "    if current is target then"),
			(-1, "        Path found."),
			( 6, "    for each neighbour of current do"),
			( 7, "        if neighbour is in visited then"),
			( 8, "            push neighbour to unvisited"),
			( 8, "            cameFrom[neighbour] := current"),
			( 9, "            add neighbour to visited"),
			( 6, "    "),
			( 3, ""),
			(-2, "No path found.")
		};

	}

	/* Schritt 0 */
	public int initUnvisitedNodes(Graph g)
	{
		this.unvisitedNodes = new NeudinkelPriorityQueue<GraphNode, float>();
		return this.currentStep + 1;
	}

	/* Schritt 1 */
	public int initVisitedNodes(Graph g)
	{
		this.visitedNodes = [];
		return this.currentStep + 1;
	}

	/* Schritt 2 */
	public int enqueueStartNode(Graph g)
	{
		enqueueLikeStack(this.startNode);
		this.visitedNodes.Add(startNode);
		Events.Instance.EmitSignal(Events.SignalName.UpdateOpenQueue);
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
		this.lastNode = this.currentNode;
		this.currentNode = this.unvisitedNodes.Dequeue();
		Events.Instance.EmitSignal(Events.SignalName.UpdateOpenQueue);
		return this.currentStep + 1;
	}

	/* Schritt 5 */
	public int checkIfIsGoal(Graph g)
	{
		if (this.currentNode == this.goalNode)
		{
			return -1;
		}
		return this.currentStep + 1;
	}

	/* Schritt 6 */
	public int forEveryNeighbour(Graph g)
	{
		if (this.neighbourQueue == null)
		{
			// Neue Iteration
			this.neighbourQueue = new Queue<GraphNode>();
			foreach (GraphNode node in this.currentNode.getNeighbouringCities(this.visitedNodes.ToArray()))
				this.neighbourQueue.Enqueue(node);

			foreach (GraphNode n in this.neighbourQueue)
			{
				GD.Print("[" + currentNode.getTilePosString() + "] has neighbour: [" + n.getTilePosString() + "]");
			}
		}

		if (this.neighbourQueue.Count > 0)
		{
			this.currentNeighbour = this.neighbourQueue.Dequeue();
			visualizeNeighbours(g,currentNeighbour);
			return this.currentStep + 1;
		}
		else
		{
			this.neighbourQueue = null; /* vorbereiten auf naechste Iteration */
			return 3; /* whileQueueNotEmpty */
		}
	}

	/* Schritt 7 */
	public int ifNodeUnvisited(Graph g)
	{
		if (!this.visitedNodes.Contains(this.currentNeighbour))
			return this.currentStep + 1;
		else
			return 6; /* forEveryNeighbour */
	}

	/* Schritt 8 */
	public int enqueueNeighbour(Graph g)
	{
		enqueueLikeStack(this.currentNeighbour);
		Events.Instance.EmitSignal(Events.SignalName.UpdateOpenQueue);
		this.cameFrom.Add(currentNeighbour, currentNode);
		return this.currentStep + 1;
	}

	/* Schritt 9 */
	public int markNeighbourVisited(Graph g)
	{
		this.visitedNodes.Add(this.currentNeighbour);
		return 6; /* forEveryNeighbour */
	}

	/* Funny hack because we always use a priority queue for unvisited nodes */
	public void enqueueLikeStack(GraphNode n)
	{
		this.unvisitedNodes.Enqueue(n, -this.unvisitedNodes.Count);
	}

}
