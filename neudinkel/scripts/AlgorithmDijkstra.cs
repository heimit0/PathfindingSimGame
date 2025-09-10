using System;
using System.Collections.Generic;

public class AlgorithmDijkstra : NeudinkelAlgorithm
{

	private GraphNode currentNeighbour;
	private float cost;
	public Dictionary<GraphNode, GraphNode> cameFrom;

	public AlgorithmDijkstra(Graph graph, GraphNode startNode, GraphNode goalNode)
	{
		this.graph = graph;

		this.startNode = startNode;
		this.currentNode = startNode;
		this.goalNode = goalNode;
		this.unvisitedNodes.Enqueue(startNode, 0);
		this.distanceFromStart.Add(startNode,0);
		this.cameFrom = new Dictionary<GraphNode, GraphNode>();

		// alle nodes mit distanz = unendlich initialisieren
		foreach (GraphNode node in graph.getNodes()) {
			this.distanceFromStart.Add(node, float.MaxValue);
		}

		this.steps = new Func<Graph, int>[13]; // idk wie viele

		// schritte funktionen zuordnen
		this.steps[0] = dijkstra;
		this.steps[1] = chooseNextNode;
		this.steps[2] = visitNextNode;
		this.steps[3] = checkForFinish;
		this.steps[4] = iterateThroughNeighbours;
		this.steps[5] = calculateDistance;
		this.steps[6] = interpretDistance;
		this.steps[7] = checkForDistanceUpdate;
		this.steps[8] = updateDistance;
		this.steps[9] = updateCameFrom;
		this.steps[10] = addSourceNode;
		this.steps[11] = setDistance;
		this.steps[12] = addToQueue;
	}
	
	public int dijkstra(Graph g)
	{
		while(this.unvisitedNodes.Count>0)
		{
			chooseNextNode(g);
			visitNextNode(g);
			checkForFinish(g);
			iterateThroughNeighbours(g);
		}
		return 1;
	}
	
	///<summary>
	///waehlt den naechsten knoten anhand der kosten aus
	///</summary>
	public int chooseNextNode(Graph g)
	{
		this.currentNode = this.unvisitedNodes.Dequeue();
		return 2;
	}
	
	///<summary>
	/// besucht den naechsten knoten
	///</summary>
	public int visitNextNode(Graph g)
	{
		this.visitedNodes.Add(this.currentNode);
		return 3;
	}
	
	///<summary>
	/// schaut ob der derzeitige Knoten das Ziel ist
	///</summary>
	public int checkForFinish(Graph g)
	{
		if(this.currentNode.Equals(goalNode))
		{
		resultToString();
			return -1;
		}
		return 4;
	}
	
	///<summary>
	/// geht durch alle nachbarn durch und berechnet distanz und interpretiert diese
	///</summary>
	public int iterateThroughNeighbours(Graph g)
	{
		foreach(GraphNode node in this.currentNode.getNeighbours())
		{
			currentNeighbour = node;
			calculateDistance(g);
			interpretDistance(g);
		}
		return 5;
	}
	
	///<summary>
	/// berechnet die Distanz
	///</summary>
	public int calculateDistance(Graph g)
	{
		cost = this.distanceFromStart[this.currentNode] + (float)currentNeighbour.getStreetType();
		return 6;
	}
	
	///<summary>
	/// schaut ob der Knoten schonmal besucht wurde 
	/// wenn ja dann ueberprueft die kosten
	/// wenn nein setzt vorgaenger/kosten und fuegt den knoten der warteschlange hinzu 
	///</summary>
	public int interpretDistance(Graph g)
	{
		if(this.distanceFromStart.ContainsKey(currentNeighbour))
		{
			checkForDistanceUpdate(g);
			return 7;
		}
		else
		{
			addSourceNode(g);
			setDistance(g);
			addToQueue(g);
			return 10;
		}
	}
	
	///<summary>
	/// ueberprueft ob ein billigerer weg gefunden wurde und updatet den wenn ja
	///</summary>
	public int checkForDistanceUpdate(Graph g)
	{
		if(cost < this.distanceFromStart[currentNeighbour])
		{
			updateDistance(g);
			updateCameFrom(g);
			return 8;
		}
		return 0;
	}
	
	///<summary>
	/// aktualisiert die Kosten
	///</summary>
	public int updateDistance(Graph g)
	{
		this.distanceFromStart[currentNeighbour] = cost;
		return 9;
	}
	
	///<summary>
	/// aktualisiert den vorgaenger
	///</summary>
	public int updateCameFrom(Graph g)
	{
		this.cameFrom[currentNeighbour] = this.currentNode;
		return 0;
	}
	
	///<summary>
	/// setzt den vorgaenger
	///</summary>
	public int addSourceNode(Graph g)
	{
		this.cameFrom.Add(currentNeighbour,this.currentNode);
		return 11;
	}
	
	///<summary>
	///gibt den weg in nem String wieder
	///</summary>
	public String resultToString()
	{
		String result = "";
		return result;
	}
	
	///<summary>
	/// setzt die distanz
	///</summary>
	public int setDistance(Graph g)
	{
		this.distanceFromStart.Add(currentNeighbour,cost);
		return 12;
	}
	
	///<summary>
	/// fuegt den knoten der warteschlange hinzu
	///</summary>
	public int addToQueue(Graph g)
	{
		this.unvisitedNodes.Enqueue(currentNeighbour, cost);
		return 0;
	}

}
