using Godot;
using System;

/*
	TODO:
		Perspektive - muss ueberarbeitet werden, noch nicht schoen (eventuell im Editor?)
		Zoom-Grenze - damit man nicht durch die Karte "durchzoomen" kann
		Weiches Zoomen - bis jetzt ist es sehr ruckartig
		Perspektivisches Zoomen - siehe Warcraft III / Starcraft II
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
	Vector3 distanceToScroll;
	private Vector3 scrollToPosition;
	private bool scrolling = false;
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
		
		
		if(distanceToScroll.Length() > 0.1f )
		{
			Vector3 newPosition= this.Position + Transform.Basis.Z * scrollInput * scrollSpeed * (float) delta * 60;
			if(newPosition.Y > 5)
			{
				resetCamera();
				
				return;
			}
			this.Position = newPosition;
			this.Rotation = getRotation(this.Position.Y);
			distanceToScroll = this.Position - scrollToPosition;
		}
		else
		{
			scrolling = false;
		}
		
		
		

	}
	private void setScroll(float scrollDirection)
	{
			scrollInput = scrollDirection;
			scrolling = true;
			scrollToPosition = this.Position + Transform.Basis.Z * scrollInput*2;
			distanceToScroll = this.Position - scrollToPosition;
	}
	
	private Vector3 getRotation(float position)
	{
		
		float test = Mathf.Min(position+2 / 5,1);
		return new Vector3(test*-45*Mathf.Pi/180,0,0);
	}
	private void resetCamera()
	{
		scrolling = false;
		distanceToScroll = new Vector3(0,0,0);
	}

}
