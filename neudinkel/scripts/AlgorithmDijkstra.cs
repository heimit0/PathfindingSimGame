using System;

public class AlgorithmDijkstra : NeudinkelAlgorithm
{
    public AlgorithmDijkstra(GraphSearchState state, Graph graph, GraphNode startNode, GraphNode goalNode)
    {
        this.state = state;
        this.graph = graph;

        this.state.startNode = startNode;
        this.state.currentNode = startNode;
        this.state.goalNode = goalNode;
        this.state.unvisitedNodes.Enqueue(startNode, 0);

        // alle nodes mit distanz = unendlich initialisieren
        foreach (GraphNode node in graph.getNodes()) {
            this.state.distanceFromStart.Add(node, float.MaxValue);
        }

        this.steps = new Func<GraphSearchState, Graph, int>[10]; // idk wie viele
        // schritte funktionen zuordnen
    }
}