using System;

public class NeudinkelAlgorithm {
    public GraphSearchState state { get; set; }
    public Graph graph { get; set; }
    public Func<GraphSearchState, Graph, int>[] steps; // gibt den nächsten schritt zurück
    public int currentStep { get; set; }

    public void doSearchStep() {
        currentStep = steps[currentStep](this.state, this.graph);

        // currentStep an signal für die UI senden
    }
}