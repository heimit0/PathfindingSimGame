using Godot;
using System;


/*
	TODO:
		Perspektive - muss ueberarbeitet werden, noch nicht schoen (eventuell im Editor?)
*/

public partial class Camera : Camera3D
{

	[Export]
	public float moveSpeed = 2.0f;
	[Export]
	public float scrollSpeed = 0.2f;

	private bool isDragging = false;
	private Vector3 originPos;
	private float scrollInput = 0.0f;
	private Vector2 originMousePos;
	private Vector3 scrollToPosition;
	private bool scrolling = false;
	public bool isUserInputEnabled { get; set; }

	[Signal]
	public delegate void setIsUserInputEnabledEventHandler(bool enabled);

	[Signal]
	public delegate void TileHoveredEventHandler(Vector2I tilePos);

	public override void _Ready()
	{
		// FIXME: TERRIBLE HACK
		this.GetParent<GameManager>().SetCameraPosition += onSetCameraPosition;
		this.isUserInputEnabled = true;
		setIsUserInputEnabled += enabled => this.isUserInputEnabled = enabled;

		// Connect TileHovered signal to PruefungsGameManager if that's our parent
		GameManager parent = this.GetParent<GameManager>();
		if (parent is PruefungsGameManager pruefungsGameManager)
		{
			GD.Print("Camera: Connecting to PruefungsGameManager");
			this.TileHovered += pruefungsGameManager.onTileHovered;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton && isUserInputEnabled)
		{

			Globals.playSfx("res://assets/sounddesign/klick.wav");

			Vector2 diff = originMousePos - GetViewport().GetMousePosition();
			bool allowTileSelection = false;
			if (diff.Length() < 5)
			{
				allowTileSelection = true;
			}
			if (eventMouseButton.GetButtonIndex() == MouseButton.Left && eventMouseButton.IsReleased() && allowTileSelection)
			{


				Vector2 mousePos = eventMouseButton.Position;

				if (isMouseInUIDeadzone(mousePos)) //UI pos based Deadzone
					return;


				Vector3 clickPoint = this.ProjectPosition(eventMouseButton.Position, 1);
				Vector3 ray = new Vector3(this.Position.X - clickPoint.X, this.Position.Y - clickPoint.Y, this.Position.Z - clickPoint.Z);
				float castDistance = this.Position.Y / ray.Y;
				Vector3 nullPos = this.Position - ray * castDistance;
				Vector2I tilePos = GraphNode.convertPosToTilePos(nullPos);
				// GD.Print("Tile selected: [" + tilePos.X + ", " + tilePos.Y + "]");

				GameManager gameManager = GetNode<GameManager>("/root/Node3D");

				if (gameManager is BauGameManager || gameManager is PruefungsGameManager)
				{
					gameManager.EmitSignal("TileSelected", tilePos);
				}
			}
		}

		// Handle mouse motion for hover effect in SimulationGameManager
		if (@event is InputEventMouseMotion eventMouseMotion && isUserInputEnabled)
		{
			Vector2 mousePos = eventMouseMotion.Position;

			if (isMouseInUIDeadzone(mousePos))
				return;

			Vector3 clickPoint = this.ProjectPosition(eventMouseMotion.Position, 1);
			Vector3 ray = new Vector3(this.Position.X - clickPoint.X, this.Position.Y - clickPoint.Y, this.Position.Z - clickPoint.Z);
			float castDistance = this.Position.Y / ray.Y;
			Vector3 nullPos = this.Position - ray * castDistance;
			Vector2I tilePos = GraphNode.convertPosToTilePos(nullPos);

			GameManager gameManager = GetNode<GameManager>("/root/Node3D");

			

			if (gameManager is PruefungsGameManager)
			{
				EmitSignal(SignalName.TileHovered, tilePos);
			}
		}
	}

	public override void _Process(double delta)
	{
		if (!isUserInputEnabled) return;

		if (Input.IsActionPressed("mouse_drag"))
		{
			if (!isDragging)
			{
				isDragging = true;
				originPos = this.Position;
				originMousePos = GetViewport().GetMousePosition();
				GD.Print("Dragging Mouse");
			}
		}

		if (Input.IsActionJustReleased("mouse_drag"))
			isDragging = false;


		if (isDragging)
		{
			Vector2 currentMousePos = GetViewport().GetMousePosition();
			Vector2 mouseDiff = new Vector2(
				originMousePos.X - currentMousePos.X,
				originMousePos.Y - currentMousePos.Y
			);

			this.Position = originPos + new Vector3(mouseDiff.X * moveSpeed * 0.01f, 0, mouseDiff.Y * moveSpeed * 0.01f) * (float)delta * 60;
		}




		if (!scrolling)
		{
			if (Input.IsActionJustPressed("mouse_scroll_up"))
			{
				setScroll(-1.0f);
			}

			if (Input.IsActionJustPressed("mouse_scroll_down"))
			{
				setScroll(1.0f);
			}
		}


		if (scrolling)
		{
			Vector3 newPosition = this.Position + Transform.Basis.Z * scrollInput * scrollSpeed * (float)delta * 60;
			if (newPosition.Y > 7 || newPosition.Y < 0.6f)
			{
				scrolling = false;
				return;
			}
			if (scrollInput < 0 && newPosition.Y < scrollToPosition.Y)
			{
				scrolling = false;
				return;
			}
			if (scrollInput > 0 && newPosition.Y > scrollToPosition.Y)
			{
				scrolling = false;
				return;
			}
			this.Position = newPosition;
			this.Rotation = getRotation(this.Position.Y);
		}

	}

	private void setScroll(float scrollDirection)
	{
		scrollInput = scrollDirection;
		scrolling = true;
		scrollToPosition = new Vector3(0, this.Position.Y + scrollDirection * 1.5f, 0);
	}

	private Vector3 getRotation(float position)
	{
		float test = Mathf.Min(position + 2 / 5, 1);
		return new Vector3(test * -45 * Mathf.Pi / 180, 0, 0);
	}

	public void onSetCameraPosition(Vector2 pos)
	{
		this.Position = new Vector3(pos.X, this.Position.Y, pos.Y);
	}

	private bool isMouseInUIDeadzone(Vector2 mousePos)
	{
		// Get all buttons in the current scene
		var buttons = GetTree().Root.GetNode("Node3D").FindChildren("*", "Button", true, false);
		var textureButtons = GetTree().Root.GetNode("Node3D").FindChildren("*", "TextureButton", true, false);

		// Check TextureButtons
		foreach (var btn in textureButtons)
		{
			if (btn is TextureButton textureButton && textureButton.Visible)
			{
				Rect2 buttonRect = textureButton.GetGlobalRect();
				if (buttonRect.HasPoint(mousePos))
				{
					return true;
				}
			}
		}

		// Check regular Buttons
		foreach (var btn in buttons)
		{
			if (btn is Button button && button.Visible)
			{
				Rect2 buttonRect = button.GetGlobalRect();
				if (buttonRect.HasPoint(mousePos))
				{
					return true;
				}
			}
		}
		return false;
	}

}
