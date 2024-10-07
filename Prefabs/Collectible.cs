using Godot;
using System;

public class Collectible : RigidBody2D
{
  public Vector2 StartPosition; // To be able to reset the collictible to its original place

  public virtual void PickUp(Player player)
  {
    GetParent().RemoveChild(this);
    QueueFree();

    player.SetAboveHead(this);
  }
}