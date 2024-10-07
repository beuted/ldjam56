using Godot;
using System;

public class Camera : Camera2D
{
  private float _decay = 0.8f;
  private float _trauma = 0;
  private float _maxRoll = 0.1f;
  private Vector2 _maxOffset = new Vector2(100, 75);
  private float _traumaPower = 2;
  private float _globalMaxTrauma = 0.6f;
  private float _noiseY = 0;
  private OpenSimplexNoise _noise = new OpenSimplexNoise();
  private ColorRect _damageVignette;
  private Label _fpsCounter;
  private Control _debugInfo;
  private Shockwave _shockwave;

  public override void _Ready()
  {
    _noise.Octaves = 2;
    _noise.Period = 4;
    _noise.Seed = System.Environment.TickCount;

    _damageVignette = GetNode<ColorRect>("CanvasLayer/DamageVignette");
    _fpsCounter = GetNode<Label>("CanvasLayer/DebugInfo/FPS/Value");
    _debugInfo = GetNode<Control>("CanvasLayer/DebugInfo");
    _shockwave = GetNode<Shockwave>("CanvasLayer/Shockwave");

    _debugInfo.Visible = false;
  }

  public override void _Process(float delta)
  {
    if (_trauma > 0)
    {
      _trauma = Mathf.Max(_trauma - _decay * delta, 0);
      Shake();
    }

    _fpsCounter.Text = ((int)Engine.GetFramesPerSecond()).ToString();
  }

  // intensity from 0 to 1
  public void SetDamageVignette(float intensity)
  {
    (_damageVignette.Material as ShaderMaterial).SetShaderParam("multiplier", 1.0f - intensity);
  }

  public void AddTrauma(float amount, float maxTrauma)
  {
    if (maxTrauma != -1f)
      _trauma = Mathf.Min(Mathf.Min(_trauma + amount, maxTrauma), _globalMaxTrauma);
    else
    {
      _trauma = Mathf.Min(_trauma + amount, _globalMaxTrauma);
    }
  }

  public void ToggleShowDebug()
  {
    _debugInfo.Visible = !_debugInfo.Visible;
  }

  public void Shake()
  {
    var amount = Mathf.Pow(_trauma, _traumaPower);
    _noiseY += 1.0f;
    Rotation = _maxRoll * amount * _noise.GetNoise2d(_noise.Seed, _noiseY);
    Offset = new Vector2(
      _maxOffset.x * amount * _noise.GetNoise2d(_noise.Seed * 2, _noiseY),
      _maxOffset.y * amount * _noise.GetNoise2d(_noise.Seed * 3, _noiseY)
    );
  }

  public void Blast(float speed = 1.0f, float thickness = 0.8f)
  {
    _shockwave.Blast(speed, thickness);
  }
}
