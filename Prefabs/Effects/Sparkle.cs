using Godot;
using System;

public class Sparkle : Node2D
{
    private AnimationPlayer _animationPlayer;


    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _animationPlayer.Play("Play");
        _animationPlayer.Seek(RandomGeneratorService.Random.RandiRange(0, 30)); // Randomaize the starting point of the sparkle
    }
}
