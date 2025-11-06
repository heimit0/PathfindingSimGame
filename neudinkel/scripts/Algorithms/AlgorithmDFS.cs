using System;
using System.Collections.Generic;

public class AlgorithmDFS : NeudinkelAlgorithm, INeudinkelAlgorithm
{

	public Stack<GraphNode> nodesToProcess;
	
	// TODO(jbrause): Implementieren (Siehe Erklärungen NeudinkelAlgorithm.cs)
	public AlgorithmDFS(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;

		this.startNode = startNode;
		this.currentNode = startNode;
		this.goalNode = goalNode;

		this.nodesToProcess.Push(startNode);

		this.steps = new Func<Graph, int>[10]; // idk wie viele
		// schritte funktionen zuordnen
	}

    public void visualize(Graph g)
    {
        throw new NotImplementedException();
    }
}
