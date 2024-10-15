using Godot;
using System;
using System.Collections.Generic;

public class Vine : Area2D
{
  private CollisionShape2D _collisionShape2d;
  private Line2D _line2D;
  private CollectiblesManager _collectiblesManager;
  private SoundManager _soundManager;
  private PackedScene _flowerScene;
  public float GrowthSpeed = 70f;

  public TileMap TileMap;
  private bool _stopGrowth = false;

  public override void _Ready()
  {
    // Singletons
    _collectiblesManager = (CollectiblesManager)GetNode($"/root/{nameof(CollectiblesManager)}"); // Singleton
    _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton

    _flowerScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Flower.tscn");

    _collisionShape2d = GetNode<CollisionShape2D>("CollisionShape2D");
    _line2D = GetNode<Line2D>("Line2D");

    _line2D.Points = new[] {
      Vector2.Zero,
      Vector2.Zero,
    };

    _collisionShape2d.Shape = _collisionShape2d.Shape.Duplicate() as Shape2D;
    _soundManager.PlayVine();
  }

  public override void _Process(float delta)
  {
    if (_stopGrowth)
      return;

    var newLength = -(_line2D.Points[1].y - delta * GrowthSpeed);

    _line2D.Points = new[] {
      Vector2.Zero,
      new Vector2(_line2D.Points[1].x, -newLength)
    };

    // Update collision box
    _collisionShape2d.Position = new Vector2(0, -newLength / 2);
    (_collisionShape2d.Shape as RectangleShape2D).Extents = new Vector2(4, newLength / 2);

    if (ShouldStop(newLength))
    {
      _stopGrowth = true;
      _soundManager.StopVine();

      // We have grown enought ot blow a flower
      if (newLength >= 600f)
      {
        var flowerInstance = _flowerScene.Instance() as Flower;
        flowerInstance.Position = new Vector2(0, -600f);
        AddChild(flowerInstance);
      }
    }
  }

  public bool ShouldStop(float length)
  {
    if (length < 16f) // We don't stop until the vine is at least 16 pixel high
      return false;

    if (length >= 600f) // If too long stop
      return true;

    if (TileMap != null)
    {
      // We detect the position + half a tile down to stop "a bit later"
      var positionTile = TileMap.WorldToMap(Position + Vector2.Up * (length - 10f));
      var tile = TileMap.GetCellv(positionTile);

      if (tile != -1)
      {
        // Tile not empty => we stop
        return true;
      }
    }

    return false;
  }

  public void OnBodyEntered(Node body)
  {
    if (body is Player player)
    {
      player.IsGrabbingVine += 1;
    }
  }

  public void OnBodyExited(Node body)
  {
    if (body is Player player)
    {
      player.IsGrabbingVine -= 1;
    }
  }
}