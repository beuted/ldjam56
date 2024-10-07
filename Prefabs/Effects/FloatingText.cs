using Godot;
using System;

public class FloatingText : Position2D
{
  private Label _label;
  private Tween _tween;
  public string Text = "0";
  public string Color = "ffffff";
  private Vector2 _velocity;

  public override void _Ready()
  {
    _tween = GetNode<Tween>("Tween");
    _label = GetNode<Label>("Label");

    _label.Text = Text;
    _label.Set("custom_colors/font_color", new Color(Color));

    var horizontalVelocity = RandomGeneratorService.Random.RandiRange(-10, 10);
    _velocity = new Vector2(horizontalVelocity, 20);

    _tween.InterpolateProperty(this, "scale", Vector2.One, 0.1f * Vector2.One, 0.1f, Tween.TransitionType.Linear, Tween.EaseType.Out, 0.4f);
    _tween.Start();
  }

  public override void _Process(float delta)
  {
    Position -= _velocity * delta;
  }

  public void OnTweenTweenAllCompete()
  {
    QueueFree();
  }
}
