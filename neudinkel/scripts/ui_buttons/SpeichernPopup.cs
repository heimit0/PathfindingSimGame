using Godot;
using System;

public partial class SpeichernPopup : PopupPanel
{
	public event Action<string>? Submitted;
	LineEdit input;

	public override void _Ready()
	{
		input = GetNode<LineEdit>("VBoxContainer/UserInput");
	}

	public void Open()
	{
		input.Text = "";
		Show();
		MoveToCenter();
		input.GrabFocus();
	}

	public void onSpeichern()
	{
		Submitted?.Invoke(input.Text);
		Hide();
	}

	public void onAbbrechen()
	{
		Hide();
	}
}
