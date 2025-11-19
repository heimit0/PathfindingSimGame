using Godot;
using Microsoft.VisualBasic;
using System;

public partial class Globals : Node
{
    ConfigFile SaveFile;
    String path = "";
    

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
        SaveFile.SetValue("progress","dijkstra","");
        //TODO
    }

    public void loadGame()
    {
        //TODO
    }




}
