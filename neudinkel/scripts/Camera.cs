using Godot;
using System;
public partial class Camera : Camera3D
{
    [Export]
    public float moveSpeed;
    [Export]
    public float scrollSpeed;
    private bool isDragging = false;
    private Vector3 originPos;
    private Vector2 originMousePos;

    public override void _Ready() { }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("mouse_drag"))
        {
            if (!isDragging)
            {
                isDragging = true;
                originPos = this.Position;
                originMousePos = GetViewport().GetMousePosition();
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

            this.Position = originPos + new Vector3(mouseDiff.X * moveSpeed * 0.01f, 0, mouseDiff.Y * moveSpeed * 0.01f) * (float) delta * 60;
        }

        float scrollInput = 0.0f;

        if (Input.IsActionJustPressed("mouse_scroll_up"))
            scrollInput = -1.0f;

		if (Input.IsActionJustPressed("mouse_scroll_down"))
            scrollInput = 1.0f;
		
		this.Position += Transform.Basis.Z * scrollInput * scrollSpeed * (float) delta * 60;
    }
}