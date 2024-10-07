using Godot;
using System;

public class EffectManager : Node2D
{
  private PackedScene _floatingTextScene;
  private PackedScene _waterSplashScene;
  private PackedScene _spawnEffectScene;
  private PackedScene _smokePuffScene;
  private Node2D _effectNode;

  public override void _Ready()
  {
    _floatingTextScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Effects/FloatingText.tscn");
    _waterSplashScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Effects/WaterSplash.tscn");
    _spawnEffectScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Effects/SpawnEffect.tscn");
    _smokePuffScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Effects/SmokePuff.tscn");
  }

  public void Init(Node2D effectNode)
  {
    _effectNode = effectNode;
  }

  public void SpawnFloatingText(string text, Vector2 position, string color = "ffffff")
  {
    var floatingText = _floatingTextScene.Instance() as FloatingText;
    floatingText.Color = color;
    floatingText.Text = text;
    floatingText.Position = position;
    _effectNode.AddChild(floatingText);
  }

  public void SpawnWaterSlash(Vector2 position)
  {
    var waterSplash = _waterSplashScene.Instance() as WaterSplash;
    waterSplash.Position = position;
    _effectNode.AddChild(waterSplash);
  }

  public void GenerateWalkSmoke(Vector2 position)
  {
    var smokePuff = _smokePuffScene.Instance() as SmokePuff;
    smokePuff.Position = position;
    smokePuff.Type = SmokePuffType.WalkPuff;
    GetParent().AddChild(smokePuff);
  }

  public void GenerateSpawnEffect(Vector2 position)
  {
    var spawnEffect = _spawnEffectScene.Instance() as SpawnEffect;
    spawnEffect.Position = position;
    GetParent().AddChild(spawnEffect);
  }
}