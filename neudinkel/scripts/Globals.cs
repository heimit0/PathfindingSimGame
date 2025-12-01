using Godot;
using System;

public partial class Globals : Node
{

	//variablen
	public static Random rand{ get; set; }
	public static string nextLevel; // Gibt mit welches lvl File geladen werden soll
	public static GraphSearchAlgorithm pruefungsAlgorithmus; //Gibt mit welcher Algorithmus bei der Pruefung genutzt wird

	//Persistente variablen
	public static int progress;

	public Globals()
	{
		Globals.rand = new Random();
	}

	public static void saveGame()
	{
		
	}

	public static void loadGame()
	{
		
	}

}
