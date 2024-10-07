using Godot;
using System;

public class PlayerManager : Node
{
  public Player CurrentPlayer { get; private set; }
  private PackedScene _playerScene;
  private CameraManager _cameraManager;
  private Node2D _ySort;

  public static Vector2 StartPosition = new Vector2(12, -183);
  // public static Vector2 StartPosition = new Vector2(12, -183); // NORMAL
  //public static Vector2 StartPosition = new Vector2(3512, -678); // Spawn at final final
  //public static Vector2 StartPosition = new Vector2(2155, -200); // Spawn at final final

  public override void _Ready()
  {
    _playerScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Player.tscn");
  }

  public Player Init(Node2D ySort)
  {
    _ySort = ySort;

    return InstanciatePlayer();
  }

  public void InitWithSaveData(SaveData saveData, TileMap tileMap)
  {
    if (saveData != null)
      CurrentPlayer.Init(saveData.PlayerName, saveData.PlayerPosition, tileMap);
    else
      CurrentPlayer.Init("dekajoo", StartPosition, tileMap);
  }

  public void DamagePlayer(float dmg)
  {
    CurrentPlayer.TakeDamage(dmg);
  }

  private Player InstanciatePlayer()
  {
    var playerInstance = _playerScene.Instance() as Player;

    _ySort.AddChild(playerInstance);

    CurrentPlayer = playerInstance;

    return playerInstance;
  }
}