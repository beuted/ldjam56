using Godot;
using System;

public class ParticleEffect : Particles2D
{
  private bool _start;
  private float _time = 0;

  public override void _Ready()
  {
    ProcessMaterial = ProcessMaterial.Duplicate() as Material; // To make the shader unique
  }

  public override void _Process(float delta)
  {
    if (Emitting)
    {
      _start = true;
    }
    if (_start)
      _time += delta;
    if (_time >= Lifetime)
    {
      QueueFree();
    }
  }
}
