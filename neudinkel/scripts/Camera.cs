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
				Vector3 clickPoint = this.ProjectPosition(eventMouseButton.Position,1); 
				Vector3 ray = new Vector3 (this.Position.X-size.X,this.Position.Y-size.Y,this.Position.Z-size.Z);
				float castDistance = this.Position.Y / test.Y;
				Vector3 nullPos = this.Position - test*miau;
				
				GD.Print("Mouse Click/Unclick at: ",nullpos);
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
