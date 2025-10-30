using Godot;
using System;


/*
	TODO:
		Perspektive - muss ueberarbeitet werden, noch nicht schoen (eventuell im Editor?)
*/

public partial class Camera : Camera3D
{

	[Export]
	public float moveSpeed;
	[Export]
	public float scrollSpeed;
	private bool isDragging = false;
	private Vector3 originPos;
	private float scrollInput = 0.0f;
	private Vector2 originMousePos;
	private Vector3 scrollToPosition;
	private bool scrolling = false;
	public override void _Ready() { }
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton eventMouseButton)
		 	{
				Vector2 screenSize = GetViewport().GetVisibleRect().Size;
				Vector2 relativeScreenPos = eventMouseButton.Position/screenSize;
				float miau = relativeScreenPos.Y;
				float distanceToGround = 4.925f * Mathf.Pow(miau,4)-17.855f*Mathf.Pow(miau,3)+24.521f*Mathf.Pow(miau,2)-19.091f*miau+(7.5f+this.Position.Y);
				Vector3 size = this.ProjectPosition(eventMouseButton.Position,distanceToGround); //5-12,5
				GD.Print("Mouse Click/Unclick at: ", size);
			}
	} 
	
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

		

		
		if(!scrolling)
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
		
		
		if(scrolling)
		{
			Vector3 newPosition= this.Position + Transform.Basis.Z * scrollInput * scrollSpeed * (float) delta * 60;
			if(newPosition.Y > 7 || newPosition.Y < 0.6f)
			{
				scrolling = false;
				return;
			}
			if(scrollInput < 0 && newPosition.Y < scrollToPosition.Y)
			{
				scrolling = false;
				return;
			}
			if(scrollInput > 0 && newPosition.Y > scrollToPosition.Y)
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
			scrollToPosition = new Vector3(0,this.Position.Y+scrollDirection * 1.5f,0);
	}
	
	private Vector3 getRotation(float position)
	{
		float test = Mathf.Min(position+2 / 5,1);
		return new Vector3(test*-45*Mathf.Pi/180,0,0);
	}
}
