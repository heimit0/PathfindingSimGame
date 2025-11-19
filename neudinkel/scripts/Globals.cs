using Godot;
using Microsoft.VisualBasic;
using System;

public partial class Globals : Node
{

	public static Random rand{ get; set; }
	
	private static ConfigFile SaveFile;
	private static String path = "";

	//variablen
	public static string nextLevel;

	//Persistente variablen
	public static int progress;

	public Globals()
	{
		SaveFile = new ConfigFile();
		Globals.rand = new Random();
	}

	public static void safeGame()
	{
		if(SaveFile == null)
		{
			GD.Print("no safe file initialized");
			return;
		}
	   
		// SaveFile.SetValue("progress",""); 

		//in File speichern (wird überschrieben, wenn existent)
		SaveFile.Save("");
	}

	public static void loadGame()
	{
		var score_data = new Godot.Collections.Dictionary();

		//File laden
		Error err = SaveFile.Load(path);

		if(err != Error.Ok)
		{
			return;
		}

		/**foreach(String progress in SaveFile.GetSections())
		{
			var progress = (int)SaveFile.GetValue(progress, "");
		}**/
	}

}
