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

	private static readonly string configFilePath = "user://config.json";
	private static AudioStreamPlayer asp;

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

		asp = new AudioStreamPlayer();
		AddChild(asp);

		Globals.loadGame();
		initUserLevelDir();
	}

	/* <HACK> */
	/// <summary>
	/// Sicherstellen dass es das Nutzer-Level-Verzeichnis WIRKLICH gibt.
	/// Erstellt außerdem die default baumodus_save, falls nicht existent.
	/// </summary>
	public void initUserLevelDir()
	{
		string path = "user:///levels/";
		if (!DirAccess.DirExistsAbsolute(path))
		{
			DirAccess.MakeDirAbsolute(path);
		}

		if (!FileAccess.FileExists("user:///levels/baumodus_save.json"))
		{
			DirAccess.CopyAbsolute("res:///levels/baumodus_save.json", "user:///levels/baumodus_save.json");
		}

		return;
	}
	/* </HACK> */

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

		

		if (gm is PruefungsGameManager)
		{
			return GameModi.PruefM;
			
		}else if(gm is SimulationGameManager){
			
			return GameModi.SimM;

		}else if(gm is BauGameManager)
		{
			return GameModi.BauM;
		}

		return GameModi.BauM;
		
		
	}

	/// <summary>
	/// Spielt einen BG-Musik-Track ab. Unterbricht dabei einen etwaigen
	/// alten Track.
	/// Notiz: Die Audio-Ressource sollte auf "Loop Forward" gestellt sein.
	/// </summary>
	public static void playBackgroundMusic(string file)
	{
		Globals.asp.Stop();
		Globals.asp.Stream = GD.Load<AudioStream>(file);
		Globals.asp.Play();
	}

	/// <summary>
	/// Hält den aktuellen BG-Musik-Track an.
	/// </summary>
	public static void stopBackgroundMusic()
	{
		Globals.asp.Stop();
	}

}
