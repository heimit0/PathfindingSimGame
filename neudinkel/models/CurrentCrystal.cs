using Godot;
using System;

public partial class CurrentCrystal : Node3D
{

    public override void _Process(double delta)
    {
        this.RotateY(1.0f * (float) delta);
    }
}  
