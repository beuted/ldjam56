using Godot;
using System;
using System.Runtime.CompilerServices;

public class InputManager : Node2D
{

  [Signal] public delegate void controller_in_use_changed();

  public bool ControllerInUse = false;

  public override void _Ready()
  {
    PauseMode = PauseModeEnum.Process;
  }

  public override void _Input(InputEvent evt)
  {
    if (evt is InputEventJoypadMotion evtJoypad)
    {
      ControllerInUse = true;
      EmitSignal(nameof(controller_in_use_changed), ControllerInUse);
    }
    else if (evt is InputEventMouse evtMouse)
    {
      ControllerInUse = false;
      EmitSignal(nameof(controller_in_use_changed), ControllerInUse);
    }
  }

}