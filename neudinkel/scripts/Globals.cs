using Godot;
using System;
using System.Text.Json;

public class NeudinkelConfig
{

	// Persistente Variablen
	public int progress { get; set; }

	public NeudinkelConfig()
	{
		this.progress = 0;
	}

}

public partial class Globals : Node
{

	public static readonly string configFilePath = "user://config.json";

	//variablen
	public static Random rand { get; set; }
	public static string nextLevel; // Gibt mit welches lvl File geladen werden soll

	public static int stufe; //Man kann next lvl in algorithmus und stufe aufteilen, waer schoener aber kein muss TODO

	public enum GameModi
	{
		BauM,
		SimM,
		PruefM
	}

	public static GraphSearchAlgorithm pruefungsAlgorithmus; //Gibt mit welcher Algorithmus bei der Pruefung genutzt wird
	public static NeudinkelConfig config;
	public static JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true };

	public static string baumodusInitLevel { get; set; } // null, falls der baumodus kein level vorladen soll, andernfalls der dateiname (ohne .json)

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

	public static async void openSimpleInfoModal(Node origin, String text, int showForSeconds)
	{
		PopupPanel modal = new PopupPanel();
		origin.AddChild(modal);

		Label lbl = new Label();
		lbl.Text = text;

		modal.AddChild(lbl);
		modal.MaxSize = new Vector2I(500, 30);
		modal.PopupCentered();

		await origin.ToSignal(origin.GetTree().CreateTimer(showForSeconds), "timeout");
		modal.Hide();
		origin.RemoveChild(modal);
		modal.QueueFree();
	}

	public static GameModi getGameMode(Node node)
	{
		GameManager gm = node.GetNode<GameManager>("/root/Node3D");

		

		if (gm is SimulationGameManager)
        {
						GD.Print("Simmanager");
            return GameModi.SimM;
			
		}else if(gm is BauGameManager){
						GD.Print("Baumanager");
			return GameModi.BauM;

		}else if(gm is PruefungsGameManager)
		{
						GD.Print("Pruefmanager");
			return GameModi.PruefM;
		}

		return GameModi.BauM;
		
		
	}
}
