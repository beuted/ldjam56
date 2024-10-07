using Godot;
using System;

public class Mushroom : Node2D
{
  private Sprite _sprite;
  private CollisionShape2D _collisionShape2d;

  private CollectiblesManager _collectiblesManager;
  private SoundManager _soundManager;

  public override void _Ready()
  {
    // Singletons
    _collectiblesManager = (CollectiblesManager)GetNode($"/root/{nameof(CollectiblesManager)}"); // Singleton
    _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton


    _sprite = GetNode<Sprite>("Sprite");
    _collisionShape2d = GetNode<CollisionShape2D>("CollisionShape2D");

    _sprite.Frame = 258;
  }

  public void SetFull()
  {
    _sprite.Frame = 258;
  }

  internal void LooseCap()
  {
    if (_sprite.Frame == 259) // To avoid double collision
      return;

    _soundManager.PlayBounce();
    _collectiblesManager.DropMushroomCapOnMapMaster(Position, new Vector2(0, -150), Position);

    _sprite.Frame = 259;
    _collisionShape2d.Disabled = true;
  }
}