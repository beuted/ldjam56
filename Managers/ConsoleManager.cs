using Godot;
using System;
using System.Linq;

public class ConsoleManager : Node
{
  private Console _wrapper;
  private PlayerManager _playerManager;
  private CameraManager _cameraManager;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    _wrapper = GetTree().Root.GetNode<Console>("CSharpConsole");
    _wrapper.AddCommand("sayHello", this, nameof(PrintHello))
            .SetDescription("prints \"hello %name%!\"")
            .AddArgument("name", Variant.Type.String)
            .Register();

    _wrapper.AddCommand("damagePlayer", this, nameof(DamagePlayer))
            .SetDescription("Damage the player for 10hp")
            .Register();

    _wrapper.AddCommand("showDebug", this, nameof(ShowDebug))
            .SetDescription("Show debug info on screen")
            .Register();

    // Autoloads
    _playerManager = (PlayerManager)GetNode("/root/PlayerManager"); // Singleton
    _cameraManager = (CameraManager)GetNode($"/root/{nameof(CameraManager)}"); // Singleton
  }

  public bool IsConsoleShown()
  {
    return _wrapper.IsConsoleShown();
  }

  public void HideConsole()
  {
    _wrapper.HideConsole();
  }

  public void PrintHello(string name = null)
  {
    _wrapper.WriteLine($"Hello {name}! {_wrapper.IsConsoleShown()}");
  }

  public void DamagePlayer()
  {
    _playerManager.DamagePlayer(10f);
  }

  public void ShowDebug()
  {
    _cameraManager.ToggleShowDebug();
  }

}