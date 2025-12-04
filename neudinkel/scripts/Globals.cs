using Godot;
using System;
using System.Text.Json;

public class NeudinkelConfig
{

	// Persistente Variablen
	public int progress { get; set; }

}

public partial class Globals : Node
{

	public static readonly string configFilePath = "user://config.json";

	//variablen
	public static Random rand { get; set; }
	public static string nextLevel; // Gibt mit welches lvl File geladen werden soll
	public static GraphSearchAlgorithm pruefungsAlgorithmus; //Gibt mit welcher Algorithmus bei der Pruefung genutzt wird
	public static NeudinkelConfig config;
	public static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };

	public static string baumodusInitLevel { get; set; } // null, falls der baumodus kein level vorladen soll, andernfalls der dateiname (ohne .json)

	public Globals()
	{
		Globals.rand = new Random();
		Globals.config = new NeudinkelConfig();

		Globals.config.progress = 128;
		Globals.saveGame();
		Globals.loadGame();
	}

	public static void saveGame()
	{
		string json;
		FileAccess file;

		file = FileAccess.Open(configFilePath, FileAccess.ModeFlags.Write);

		json = JsonSerializer.Serialize(Globals.config, jsonOptions);
		file.StoreString(json);
		file.Close();
	}

	public static void loadGame()
	{
		string json;
		FileAccess file;

		if (!FileAccess.FileExists(configFilePath))
		{
			Globals.config = new NeudinkelConfig();
			return;
		}

		file = FileAccess.Open(configFilePath, FileAccess.ModeFlags.Read);
		json = file.GetAsText();
		Globals.config = JsonSerializer.Deserialize<NeudinkelConfig>(json);
		file.Close();
	}

	public static void openSimpleInfoModal(Node origin, String text)
	{
		PopupPanel modal = new PopupPanel();
		origin.AddChild(modal);

		Label lbl = new Label();
		lbl.Text = text;

		modal.AddChild(lbl);
		modal.PopupCentered();
	}

}
