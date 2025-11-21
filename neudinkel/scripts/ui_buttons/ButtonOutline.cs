using Godot;
using System;

public partial class ButtonOutline : VBoxContainer
{
	[Export] public Material OutlineMaterial;

	private TextureButton _lastPressed;

	public override void _Ready()
	{
		// Connect all TextureButtons inside the VBoxContainer
		foreach (Node child in GetChildren())
		{
			if (child is TextureButton button)
			{
				button.Pressed += () => OnButtonPressed(button);
			}
		}
	}

	private void OnButtonPressed(TextureButton button)
	{
		// Remove outline from previously pressed button
		if (_lastPressed != null)
		{
			_lastPressed.Material = null;
		}

		// Apply outline to this button
		button.Material = OutlineMaterial;

		_lastPressed = button;
	}
}
