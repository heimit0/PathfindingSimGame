using System;
using System.Collections.Generic;
using Godot;

/// <summary>
/// Elternklasse aller Suchalgorithmen.
/// </summary>
public abstract class NeudinkelAlgorithm
{

    public Graph graph { get; set; }
    public GraphNode startNode { get; set; }
    public GraphNode goalNode { get; set; }
    public GraphNode currentNode { get; set; }
    public Dictionary<GraphNode, GraphNode> cameFrom;


    // noch nicht besuchte Nodes
    public PriorityQueue<GraphNode, float> unvisitedNodes;
    // bereits besuchte Nodes
    public HashSet<GraphNode> visitedNodes;
    // tatsächliche, bekannte Kosten pro Node zum Start
    public Dictionary<GraphNode, float> distanceFromStart;
    public Func<Graph, int>[] steps; // gibt den nächsten schritt zurück
    public int currentStep { get; set; }

    /*
		Zum Implementieren eines Suchalgorithmus muessen zuerst die einzelnen
		Schritte als separate Funktionen geschrieben werden (Prototyp siehe Typ
		von steps).
		Dann wird der Konstruktor implementiert und darin sein Zustand
		initialisiert. Ausserdem muessen jetzt die Schritte selbst in der
		richtigen Reihenfolge in das steps-Array verpackt werden.
		Dazu wird einem Objekt des Typs Func einfach die Funktion selbst
		zugewiesen, und das Objekt wird in das Array gepackt.
	*/


    /// <summary>
    /// Ruft den aktuell naechsten Schritt auf und informiert die UI.
    /// </summary>
    public int doSearchStep()
    {
        if (currentStep < 0)
        {
            GD.Print("Der Algorithmus sollte nicht mehr laufen! Er ist bereits beendet!");
            return currentStep;
        }

        try
        {
            currentStep = steps[currentStep].Invoke(this.graph);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.GetBaseException());
        }

        if (currentStep == -1)
        {
            GD.Print("Der beste Pfad wurde gefunden!");
        }
        else if (currentStep == -2)
        {
            GD.Print("Es konnte kein Pfad gefunden werden!");
        }

        return currentStep;
    }

    public new void visualize(Graph g)
    {
        // Zielweg anzeigen
        if (currentStep == -1)
        {
            GD.Print("Showing best path...");
            // erst alle ausschalten
            foreach (GraphNode n in g.getNodes()) n.toggleHighlight(false);

            GraphNode _currentNode = this.goalNode;

            while (cameFrom.ContainsKey(_currentNode))
            {
                _currentNode.toggleHighlight(true);
                _currentNode = cameFrom[_currentNode];
                GD.Print(_currentNode.getTilePosString());
            }

            _currentNode.toggleHighlight(true);
            return;
        }

        // 1. Schritt ignorieren
        if (currentNode == null) return;

        foreach (GraphNode n in g.getNodes())
        {
            n.toggleHighlight(false);

            if (n.getTilePosX() == currentNode.getTilePosX() && n.getTilePosY() == currentNode.getTilePosY())
            {
                n.toggleHighlight(true);
            }
        }
    }
}
