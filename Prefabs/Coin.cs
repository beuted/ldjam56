using Godot;
using Newtonsoft.Json;
using System;

public class Coin : Collectible
{
  private CollectiblesCountManager _collectiblesCountManager;
  private CollectiblesManager _collectiblesManager;
  private PlayerManager _playerManager;
  private AnimationPlayer _animationPlayer;
  public string UID = Guid.NewGuid().ToString();
  public int PlayerOwner;

  private Player _player = null;
  private Player _playerIsBelow;

  public Coin() : base()
  {
  }

  public override void _Ready()
  {
    _collectiblesCountManager = GetNode<CollectiblesCountManager>($"/root/{nameof(CollectiblesCountManager)}"); // Singleton
    _collectiblesManager = GetNode<CollectiblesManager>($"/root/{nameof(CollectiblesManager)}"); // Singleton
    _playerManager = GetNode<PlayerManager>($"/root/{nameof(PlayerManager)}"); // Singleton

    _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
  }

  public void StartShineAnimation()
  {
    _animationPlayer.Play("Shine");
    _animationPlayer.Seek(RandomGeneratorService.Random.RandfRange(0, 9)); // Randomize the starting point of the ShineEffect
  }

  public override void _PhysicsProcess(float delta)
  {
    if (Position.y > 1000f)
    {
      // Restart where you where at first
      // Changing position wasn't working wtf !!!
      _collectiblesManager.DropCoinOnMapMaster(StartPosition, Vector2.Zero, StartPosition);

      QueueFree();  //TODO: I should use _collectiblesManager instead of just queue free if I want to allow saving mid term
      return;
    }

    if (_playerIsBelow != null)
    {
      Position = Position + new Vector2(-2f * delta, -2f * delta);
      if (_playerIsBelow.IsJumping)
      {
        ApplyCentralImpulse(new Vector2(0, -20f));
      }
    }
  }

  public override void _IntegrateForces(Physics2DDirectBodyState state)
  {
    // Prevent the MushroomCap from rotating
    var rotation_radians = Mathf.Deg2Rad(RotationDegrees);
    var new_rotation = 0;//Mathf.Clamp(rotation_radians, -0.2f, 0.2f);
    var new_transform = new Transform2D(new_rotation, Position);
    state.Transform = new_transform;
  }

  public void OnBodyEntered(Node body)
  {
    if (body is Player player)
    {
      _playerIsBelow = player;
    }

    // We bounce on mushroom and mushroom cap only if above them
    if (body is MushroomCap mushroomCap)
    {
      if (Position.y < mushroomCap.Position.y)
        ApplyCentralImpulse(new Vector2(0, -200f));
    }
    else if (body is Mushroom mushroom)
    {
      if (Position.y < mushroom.Position.y)
        ApplyCentralImpulse(new Vector2(0, -200f));
    }
    else if (body is Door door)
    {
      door.IncrementCoin();
      QueueFree();  //TODO: I should use _collectiblesManager instead of just queue free if I want to allow saving mid term
    }
  }

  public void OnBodyExited(Node body)
  {
    if (body is Player player)
    {
      _playerIsBelow = null;
    }
  }
}
