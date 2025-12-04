using Godot;
using System;

public partial class Laden : TextureButton
{
	public void onLevelLaden()
	{
		LadenPopup popup = GetNode<LadenPopup>("Simulationsmodus/UI/LadenPopup");
		// popup.Submitted += actuallySpeichern;
		popup.Open();
	}
}
