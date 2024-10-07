using Godot;
using System;

public class SpawnEffect : Node2D
{
    public override void _Ready()
    {
    }

    public void Destroy()
    {
        QueueFree();
    }
}
