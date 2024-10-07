using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class UiHelper
{
  public static void UpdateFocusBasedOnEvent(this Control node, InputEvent evt, ref Control focusControl)
  {
    if (evt is InputEventJoypadMotion evtJoypad)
    {
      if (evtJoypad.Axis == (int)JoystickList.Axis1)
      {
        if (evtJoypad.AxisValue == -1.0) // full motion up
        {
          var prev = node.GetNode<Control>(focusControl.GetPath() + "/" + focusControl.FocusPrevious);

          if (prev != null)
          {
            if (focusControl is BetterButton focusControlBtn) focusControlBtn.BetterFocus = false;
            else if (focusControl is BetterHSlider focusControlHSider) focusControlHSider.BetterFocus = false;
            focusControl = prev;
            if (focusControl is BetterButton focusControlBtn2) focusControlBtn2.BetterFocus = true;
            else if (focusControl is BetterHSlider focusControlHSider2) focusControlHSider2.BetterFocus = true;

          }

          node.GetTree().SetInputAsHandled();
        }
        else if (evtJoypad.AxisValue == 1.0) // full motion down
        {
          var next = node.GetNode<Control>(focusControl.GetPath() + "/" + focusControl.FocusNext);
          if (next != null)
          {
            if (focusControl is BetterButton focusControlBtn) focusControlBtn.BetterFocus = false;
            else if (focusControl is BetterHSlider focusControlHSider) focusControlHSider.BetterFocus = false;
            focusControl = next;
            if (focusControl is BetterButton focusControlBtn2) focusControlBtn2.BetterFocus = true;
            else if (focusControl is BetterHSlider focusControlHSider2) focusControlHSider2.BetterFocus = true;
          }

          node.GetTree().SetInputAsHandled();
        }
      }
      else if (evtJoypad.Axis == (int)JoystickList.Axis0) // Left and right
      {
        if (evtJoypad.AxisValue == 1.0) // full motion right
        {
          if (focusControl is BetterHSlider focusControlBtn) focusControlBtn.Value += focusControlBtn.Step * BetterHSlider.StepWithGamePad;
        }
        else if (evtJoypad.AxisValue == -1.0) // full motion left
        {
          if (focusControl is BetterHSlider focusControlBtn) focusControlBtn.Value -= focusControlBtn.Step * BetterHSlider.StepWithGamePad;
        }
      }
    }

    if (evt.IsActionPressed("ui_accept"))
    {
      if (focusControl is Button focusControlBtn)
      {
        focusControlBtn.EmitSignal("button_up");
      }
      node.GetTree().SetInputAsHandled();
    }
    else if (evt.IsActionPressed("ui_up"))
    {
      var prev = node.GetNode<Control>(focusControl.GetPath() + "/" + focusControl.FocusPrevious);
      if (prev != null)
      {
        if (focusControl is BetterButton focusControlBtn) focusControlBtn.BetterFocus = false;
        else if (focusControl is BetterHSlider focusControlHSider) focusControlHSider.BetterFocus = false;
        focusControl = prev;
        if (focusControl is BetterButton focusControlBtn2) focusControlBtn2.BetterFocus = true;
        else if (focusControl is BetterHSlider focusControlHSider2) focusControlHSider2.BetterFocus = true;
      }

      node.GetTree().SetInputAsHandled();
    }
    else if (evt.IsActionPressed("ui_down"))
    {
      var next = node.GetNode<Control>(focusControl.GetPath() + "/" + focusControl.FocusNext);
      if (next != null)
      {
        if (focusControl is BetterButton focusControlBtn) focusControlBtn.BetterFocus = false;
        else if (focusControl is BetterHSlider focusControlHSider) focusControlHSider.BetterFocus = false;
        focusControl = next;
        if (focusControl is BetterButton focusControlBtn2) focusControlBtn2.BetterFocus = true;
        else if (focusControl is BetterHSlider focusControlHSider2) focusControlHSider2.BetterFocus = true;

      }

      node.GetTree().SetInputAsHandled();
    }
    else if (evt.IsActionPressed("ui_right"))
    {
      if (focusControl is BetterHSlider focusControlBtn) focusControlBtn.Value += focusControlBtn.Step * BetterHSlider.StepWithGamePad;
      node.GetTree().SetInputAsHandled();
    }
    else if (evt.IsActionPressed("ui_left"))
    {
      if (focusControl is BetterHSlider focusControlBtn) focusControlBtn.Value -= focusControlBtn.Step * BetterHSlider.StepWithGamePad;
      node.GetTree().SetInputAsHandled();
    }
  }

}