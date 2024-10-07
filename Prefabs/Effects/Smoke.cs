using Godot;
using System;

public class Smoke : Particles2D
{

  [Export] public float Spread = 10;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    ProcessMaterial = ProcessMaterial.Duplicate() as Material; // TODO: should I avoid duplictaing this for every object ?
    (ProcessMaterial as ParticlesMaterial).Spread = Spread;
  }

}
