using Godot;
using System;

[Tool]
public class Shockwave : ColorRect
{
	[Export]
	public float ShockwaveDuration = 2f;

	[Export]
	public NodePath MaskPath = new NodePath();

	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}


	public void Blast(float speed = 1.0f, float thickness = 0.8f)
	{
		_animationPlayer.PlaybackSpeed = speed;
		((ShaderMaterial)Material).SetShaderParam("thickness", thickness);
		_animationPlayer.Play("Blast");
	}
}
