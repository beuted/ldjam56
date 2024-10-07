using Godot;
using System;

public class WaterSplash : Node2D
{
  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {

  }

  public void Destroy()
  {
    QueueFree();
  }
}
