using System;
using System.Collections.Generic;

public class AlgorithmDFS : NeudinkelAlgorithm
{

	public Stack<GraphNode> nodesToProcess;
	
	private List<GraphNode> currentNeighbours = new List<GraphNode>();
	private int currentNeighbourIndex;
	
	public AlgorithmDFS(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;
		this.startNode = startNode;
		this.currentNode = startNode;
		this.goalNode = goalNode;

		
		
		// Schritte-Array (nur Submethoden)
		this.steps = new Func<Graph, int>[]{
			initStack,
			initVisitedNode,
			initFirstNode,
			addFirstNodeToStack,
			whileStackNotEmpty,
			pullNextNodeFromStack,
			forEachNeigbourOfCurrentNode,
			ifNotVisited,
			checkIfGoalNodeFound,
			addToStack,
			visitNextNode,
			
		};
	}
	private int initStack(Graph g)
	{
		nodesToProcess = new Stack<GraphNode>();
		return currentStep + 1;
	}
	private int initVisitedNode(Graph g)
	{
		visitedNodes = new HashSet<GraphNode>();
		return currentStep + 1;
	}
	private int initFirstNode(Graph g)
	{
		visitedNodes.Add(startNode);
		return currentStep + 1;
	}
	private int addFirstNodeToStack(Graph g)
	{
		nodesToProcess.Push(startNode);
		return currentStep + 1;
	}
	private int whileStackNotEmpty(Graph g)
	{
		if(nodesToProcess.Count == 0){
			return -2;
		}
		return currentStep + 1;
	}
	private int pullNextNodeFromStack(Graph g)
	{
		currentNode = nodesToProcess.Pop();
		neighboursHelper(currentNode.getNeighbours());
		currentNeighbourIndex = 0;
		return currentStep + 1;
	}
	private int forEachNeigbourOfCurrentNode(Graph g)
	{
		if (currentNeighbourIndex < currentNeighbours.Count)
		{
			currentNeighbourIndex++;
			currentNode = currentNeighbours[currentNeighbourIndex];
			return currentStep + 1;
		}
		return 4;
	}
	private int checkIfGoalNodeFound(Graph g)
	{
		if(currentNode.Equals(goalNode)){
			return -1;
		}
		return currentStep + 1;
	}
	private int ifNotVisited(Graph g)
	{
		if(visitedNodes.Contains(currentNode))
		{
			return 6;
		}
		return currentStep + 1;
	}
	private int addToStack(Graph g)
	{
		nodesToProcess.Push(currentNode);
		return currentStep + 1;
	}
	private int visitNextNode(Graph g)
	{
		visitedNodes.Add(currentNode);
		return 6;
	}
	
	private void neighboursHelper(GraphNode[] neighbours)
	{
		foreach (GraphNode node in neighbours)
		{
			currentNeighbours.Add(node);
		}
	}
}
