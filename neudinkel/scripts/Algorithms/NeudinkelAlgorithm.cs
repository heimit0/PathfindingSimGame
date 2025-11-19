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
		currentStep = steps[currentStep](this.graph);

		if (currentStep == -1) {
            GD.Print("Finished oder Error!");
        }

        // currentStep an signal für die UI senden
        return currentStep;
    }

}
