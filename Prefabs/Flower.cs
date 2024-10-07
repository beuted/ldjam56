using Godot;
using System;

public class Flower : Collectible
{
    private Sprite _sprite;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Sprite");

        _sprite.Frame = 178;
    }

    public void Empty()
    {
        _sprite.Frame = 206;

        CollisionLayer = 0;
        CollisionMask = 0;
    }

    public override void PickUp(Player player)
    {
        Empty();

        player.SetAboveHead(this);
    }
}
