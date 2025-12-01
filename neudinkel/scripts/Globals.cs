using Godot;
using System;
using System.Text.Json;

public class NeudinkelConfig
{

	// Persistente Variablen
	public int progress{ get; set; }

}

public partial class Globals : Node
{

	//variablen
	public static Random rand{ get; set; }
	public static string nextLevel; // Gibt mit welches lvl File geladen werden soll
	public static GraphSearchAlgorithm pruefungsAlgorithmus; //Gibt mit welcher Algorithmus bei der Pruefung genutzt wird
	public static NeudinkelConfig config;
	public static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };

	public Globals()
	{
		Globals.rand = new Random();
		Globals.config = new NeudinkelConfig();

		Globals.loadGame();
	}

	public static void saveGame()
	{
		string json;
		FileAccess file;

		file = FileAccess.Open("user://config.json", FileAccess.ModeFlags.Write);

		json = JsonSerializer.Serialize(Globals.config, jsonOptions);
		GD.Print("Config wird geschrieben: " + json);
		file.StoreString(json);
	}

	public static void loadGame()
	{
		string json;
		FileAccess file;

		if (!FileAccess.FileExists("user:///config.json"))
		{
			Globals.config = new NeudinkelConfig();
			return;
		}

		file = FileAccess.Open("user:///config.json", FileAccess.ModeFlags.Read);
		json = file.GetAsText();
		Globals.config = JsonSerializer.Deserialize<NeudinkelConfig>(json);
	}

}
