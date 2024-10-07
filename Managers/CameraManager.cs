using Godot;
using System;

public class CameraManager : Node2D
{
  private Camera _camera;
  private PackedScene _cameraScene;

  public static float ZoomFactor = 8;

  public override void _Ready()
  {
    _cameraScene = ResourceLoader.Load<PackedScene>("res://Scenes/Camera.tscn");
  }

  public void Init(Player playerInstance)
  {
    // set the camera on the current player
    var cameraInstance = _cameraScene.Instance() as Camera;
    cameraInstance.Current = true;
    playerInstance.AddChild(cameraInstance);

    _camera = cameraInstance;
    _camera.Zoom = new Vector2(1 / ZoomFactor, 1 / ZoomFactor);
  }

  public void ToggleShowDebug()
  {
    _camera.ToggleShowDebug();
  }

  public void AddTrauma(float amount, float maxTrauma = -1f)
  {
    _camera.AddTrauma(amount, maxTrauma);
  }

  public void SetDamageVignette(float intensity)
  {
    _camera.SetDamageVignette(intensity);
  }

  public void Blast(float speed = 1.0f, float thickness = 0.8f)
  {
    _camera.Blast(speed, thickness);
  }
}