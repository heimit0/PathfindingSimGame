using System;

public class AlgorithmDFS : NeudinkelAlgorithm {
    public AlgorithmDFS(GraphSearchState state, Graph graph, GraphNode startNode, GraphNode goalNode) {
        this.state = state;
        this.graph = graph;

        this.state.startNode = startNode;
        this.state.currentNode = startNode;
        this.state.goalNode = goalNode;

        this.state.nodesToProcess.Push(startNode);

        this.steps = new Func<GraphSearchState, Graph, int>[10]; // idk wie viele
        // schritte funktionen zuordnen
    }
}