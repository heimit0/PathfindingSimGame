using System;

/// <summary>
/// Elternklasse aller Suchalgorithmen.
/// </summary>
public abstract class NeudinkelAlgorithm
{

	// TODO: Eventuell in diese Klasse mergen?
	public GraphSearchState state { get; set; }
	public Graph graph { get; set; }
	public Func<GraphSearchState, Graph, int>[] steps; // gibt den nächsten schritt zurück
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
	public void doSearchStep()
	{
		currentStep = steps[currentStep](this.state, this.graph);

		// currentStep an signal für die UI senden
	}

}
