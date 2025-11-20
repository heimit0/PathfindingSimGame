using Godot;
using System;

public partial class BaumodusUndSimulationsButtons : Node
{
    private void _on_baumodus_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/Simulationsmodus.tscn");
	}
    private void _on_simulationsmodus_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/baumodus.tscn");
	}


}
