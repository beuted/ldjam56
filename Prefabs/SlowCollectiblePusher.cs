using Godot;
using System;
using System.Collections.Generic;

public class SlowCollectiblePusher : Node2D
{

    [Export] public bool Invert = false;
    private Sprite _sprite;
    public Dictionary<Collectible, bool> collectiblesToPush = new Dictionary<Collectible, bool>();

    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Sprite");
        _sprite.Visible = false; // Sprite is here to help visualize only
    }

    public override void _PhysicsProcess(float delta)
    {
        foreach (Collectible collectible in collectiblesToPush.Keys)
        {
            collectible.LinearVelocity = collectible.LinearVelocity + new Vector2((Invert ? -200f : 200f) * delta, 0f);
        }
    }

    public void OnBodyEntered(Node body)
    {
        if (body is Collectible collectible)
        {
            collectiblesToPush.Add(collectible, true);
        }
    }

    public void OnBodyExited(Node body)
    {
        if (body is Collectible collectible && collectiblesToPush.TryGetValue(collectible, out _))
        {
            collectiblesToPush.Remove(collectible);
        }
    }
}
