using Godot;
using System;

public enum SmokePuffType
{
  ExplosionSmoke = 0,
  WalkPuff = 1,
  StartRollLeft = 2,
  StartRollRight = 3,
  StartRollUpLeft = 4,
  StartRollUpRight = 5,
  StartRollDownLeft = 6,
  StartRollDownRight = 7,
  StartRollDown = 8,
  StartRollUp = 9,
  LandRoll = 10,
}

public class SmokePuff : Node2D
{
  private AnimationPlayer _animationPlayer;
  public SmokePuffType Type = SmokePuffType.ExplosionSmoke;

  public void InitStartRoll(Vector2 orientation)
  {
    // 0.866 = sqrt(3)/2
    if (orientation.y < -0.866) // Back
    {
      Type = SmokePuffType.StartRollUp;
    }
    else if (orientation.y > 0.866) // Front
    {
      Type = SmokePuffType.StartRollDown;
    }
    else if (orientation.x < -0.866) // left
    {
      Type = SmokePuffType.StartRollLeft;
    }
    else if (orientation.x > 0.866) // right
    {
      Type = SmokePuffType.StartRollRight;
    }
    else if (orientation.x > 0 && orientation.y <= -0.3) // Profil Back right
    {
      Type = SmokePuffType.StartRollUpRight;
    }
    else if (orientation.x > 0 && orientation.y > -0.3) // Profil front right
    {
      Type = SmokePuffType.StartRollDownRight;
    }
    else if (orientation.x <= 0 && orientation.y <= -0.3) // Profil Back left
    {
      Type = SmokePuffType.StartRollUpLeft;
    }
    else // Profil front left
    {
      Type = SmokePuffType.StartRollDownLeft;
    }
  }

  public override void _Ready()
  {
    _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

    switch (Type)
    {
      case SmokePuffType.ExplosionSmoke:
        _animationPlayer.Play("explode");
        break;
      case SmokePuffType.WalkPuff:
        _animationPlayer.Play("walk-smoke-puff");
        break;
      case SmokePuffType.StartRollLeft:
        _animationPlayer.Play("roll-left-smoke");
        break;
      case SmokePuffType.StartRollRight:
        _animationPlayer.Play("roll-right-smoke");
        break;
      case SmokePuffType.StartRollUpLeft:
        _animationPlayer.Play("roll-up-left-smoke");
        break;
      case SmokePuffType.StartRollUpRight:
        _animationPlayer.Play("roll-up-right-smoke");
        break;
      case SmokePuffType.StartRollDownLeft:
        _animationPlayer.Play("roll-down-left-smoke");
        break;
      case SmokePuffType.StartRollDownRight:
        _animationPlayer.Play("roll-down-right-smoke");
        break;
      case SmokePuffType.StartRollDown:
        _animationPlayer.Play("roll-down-smoke");
        break;
      case SmokePuffType.StartRollUp:
        _animationPlayer.Play("roll-up-smoke");
        break;
      case SmokePuffType.LandRoll:
        _animationPlayer.Play("land-roll-smoke");
        break;
    }
  }

  public void Destroy()
  {
    QueueFree();
  }
}
