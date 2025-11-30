using Godot;
using System;

public partial class BaumodusUndSimulationsButtons : Node
{
	private void _on_baumodus_pressed(){
        BauGameManager gameManager = GetNode<BauGameManager>("/root/Node3D"); // TODO fix
        gameManager.EmitSignal("ChangeToSimulationMode");
	}
	private void _on_simulationsmodus_pressed(){
		GetTree().ChangeSceneToFile("res://scenes/baumodus.tscn");
	}


}
