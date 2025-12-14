using System;
using System.Collections.Generic;
using System.Linq;
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
	private GraphNode oldCurrentNode;
	public GraphNode lastNode { get; set; }
	public GraphNode[] currentNeighbours;
	public int currentNeighbourIndex;
	public Dictionary<GraphNode, GraphNode> cameFrom { get; set; }


	// noch nicht besuchte Nodes
	public NeudinkelPriorityQueue<GraphNode, float> unvisitedNodes;
	// bereits besuchte Nodes
	public HashSet<GraphNode> visitedNodes;
	// tatsächliche, bekannte Kosten pro Node zum Start
	public Dictionary<GraphNode, float> distanceFromStart;
	public Func<Graph, int>[] steps; // gibt den nächsten schritt zurück
	public (int stepIndex, string content)[] pseudoCodeSteps; // mappt von einem index aus den steps auf den pseudocode
	public int currentStep { get; set; }
	// der breakpoint für die forward taste
	public int breakpoint;
	private HashSet<GraphNode> highlightedNeighbours = new HashSet<GraphNode>();


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

	/// <summary>
	/// Fuehrt Einzelschritte aus bis der im Code festgelegte Breakpoint erreicht ist.
	/// </summary>
	public int doSearchForward()
	{
		int iterationCount = 0;
		int maxIterations = 100000; // Safety limit to prevent infinite loops

		do
		{
			doSearchStep();
			iterationCount++;

			if (iterationCount > maxIterations)
			{
				GD.PrintErr("ERROR: doSearchForward() exceeded maximum iterations!");
				GD.PrintErr("currentStep: " + currentStep + ", breakpoint: " + breakpoint);
				GD.PrintErr("This indicates an infinite loop in the algorithm.");
				return currentStep;
			}
		}
		while (currentStep != breakpoint && currentStep >= 0);

		GD.Print("doSearchForward completed after " + iterationCount + " iterations");
		return currentStep;
	}

    public new void visualize(Graph g, SimulationGameManager gameManager)
    {
        // erst alle ausschalten
        foreach (GraphNode n in g.getNodes()) n.setHighlighted(false);
		GraphNode _currentNode;

        // Zielweg anzeigen
        if (currentStep == -1)
        {
            GD.Print("Showing best path...");

			_currentNode = this.goalNode;
			

			while (cameFrom.ContainsKey(_currentNode))
			{
				_currentNode.setHighlighted(true);

				foreach (GraphNode n in _currentNode.getPathToNeighbouringCity(cameFrom[_currentNode]))
				{
					n.setHighlighted(true);
				}

				_currentNode = cameFrom[_currentNode];
			}

            if(_currentNode.getPathToNeighbouringCity(startNode) is null) return;

            foreach (GraphNode n in _currentNode.getPathToNeighbouringCity(startNode))
            {
                n.setHighlighted(true);
            }

			_currentNode.setHighlighted(true);
			gameManager.minimap.updateMinimapNodes(this);
			return;
		}

		foreach (GraphNode n in g.getNodes()) n.setHighlighted(false);
		
		//reset der vergangen nachbarn wegen wechsel der grundnode
		highlightedNeighbours = new HashSet<GraphNode>();
		
		// 1. Schritt ignorieren
		GD.Print("highlighting current node");
		if (currentNode is null) return;
		currentNode.setHighlighted(true);

		// bisherigen pfad highlighten + nachbar
		_currentNode = this.currentNode;
		if(oldCurrentNode is not null && oldCurrentNode != currentNode)
		{
			oldCurrentNode.isCurrentNode = false;
			oldCurrentNode.updateModel();
		}
		currentNode.isCurrentNode = true;
		currentNode.updateModel();
		oldCurrentNode = currentNode;


		while (cameFrom.ContainsKey(_currentNode))
		{
			// ganzen pfad
			foreach (GraphNode pathNode in _currentNode.getPathToNeighbouringCity(cameFrom[_currentNode]))
			{
				pathNode.setHighlighted(true);
			}

			_currentNode = cameFrom[_currentNode];
		}

		// derzeitiger nachbar
		if (this.currentNeighbours is null || this.currentNeighbourIndex >= this.currentNeighbours.Length) return;
		this.currentNeighbours[this.currentNeighbourIndex].setHighlighted(true);
	}
	//highlighted den derzeitig zubearbeiteten nachbar und setzt das highlighting der zuvor gehighlighteten nachbarn wieder zurück
	public void visualizeNeighbours(Graph g,GraphNode node)
	{
		highlightedNeighbours.Add(node);
		foreach (GraphNode n in highlightedNeighbours) n.setHighlighted(false);
		node.setHighlighted(true);
	}

	public float edgeCost(GraphNode from, GraphNode to)
	{
		// falls to = direkter Nachbar
		foreach (GraphNode n in from.getNeighbours())
		{
			if (n.Equals(to)) return (float)to.streetType;
		}

		float cost = 0.0f;

		var pathToNeighbour = from.getPathToNeighbouringCity(to);
		if (pathToNeighbour == null)
		{
			GD.PrintErr("Kein Pfad zwischen " + from.getTilePosString() + " und " + to.getTilePosString() + " gefunden!");
			return float.MaxValue;
		}

		foreach (GraphNode n in pathToNeighbour)
		{
			cost += (float)n.streetType;
		}

        return cost;
    }
}
