using Godot;
using System;

public partial class BackToMainmenu : Node
{

    private void OnBacktomenuPressed(){
		GetTree().ChangeSceneToFile("res://scenes/mainmenu.tscn");
	}

	private void resetLevel()
	{
		GetTree().ReloadCurrentScene();
	}
	
}
