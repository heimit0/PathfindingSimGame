using Godot;
using Microsoft.VisualBasic;
using System;

public partial class Globals : Node
{
	
	ConfigFile SaveFile;
	String path = "";

	//variablen
	public string nextLevel;

	//Persistente variablen
	public int progress;

	public Globals()
	{
		SaveFile = new ConfigFile();

	}

	public void safeGame()
	{
		if(SaveFile == null)
		{
			GD.Print("no safe file initialized");
			return;
		}
	   
		SaveFile.SetValue("progress",""); 

		//in File speichern (wird überschrieben, wenn existent)
		SaveFile.Save("")
	}

	public void loadGame()
	{
		var score_data = new Godot.Collections.Dictionary();

		//File laden
		Error err = SaveFile.Load(path);

		if(err != Error.Ok)
		{
			return;
		}

		foreach(String progress in SaveFile.GetSections())
		{
			var progress = (int)SaveFile.GetValue(progress, "");
		}
	}

}
