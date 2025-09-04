using System;

public class AlgorithmAStar : NeudinkelAlgorithm
{
	
	// TODO(gfriedr3 oder hsimosse): Implementieren (Siehe Erklärungen NeudinkelAlgorithm.cs)
	public AlgorithmAStar(GraphSearchState state, Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.state = state;
		this.graph = graph;

		this.state.startNode = startNode;
		this.state.currentNode = startNode;
		this.state.goalNode = goalNode;
		this.state.unvisitedNodes.Enqueue(startNode, heuristic(startNode, goalNode));

		// alle nodes mit distanz = unendlich initialisieren
		// ist der F-score
		foreach (GraphNode node in graph.getNodes()) {
			this.state.totalCost.Add(node, float.MaxValue);
		}

		this.steps = new Func<GraphSearchState, Graph, int>[10]; // idk wie viele
		// schritte funktionen zuordnen
	}

	public float heuristic(GraphNode startNode, GraphNode goalNode)
	{
		return -1.0f;
	}
	
}
