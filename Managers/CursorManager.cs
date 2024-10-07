using Godot;
using System;
using System.Collections.Generic;
using static Godot.Input;

public class CursorManager : Node2D
{
  private bool _isDragging = false;
  private bool _uiOpened = false;
  private bool _isMousePressed = false;
  private InputManager _inputManager;
  private Resource _cursorHand;
  private Resource _cursorHandClick;
  private Resource _cursorHandCanGrab;
  private Resource _cursorHandGrabbing;
  private Resource _cursorTarget;
  private Resource _cursorIbeam;

  public override void _Ready()
  {
    _inputManager = (InputManager)GetNode($"/root/{nameof(InputManager)}"); // Singleton

    _cursorHand = ResourceLoader.Load("res://Assets/Graphics/Cursors/cursor-hand.png");
    _cursorHandClick = ResourceLoader.Load("res://Assets/Graphics/Cursors/cursor-hand-click.png");
    _cursorHandCanGrab = ResourceLoader.Load("res://Assets/Graphics/Cursors/cursor-hand-can-grab.png");
    _cursorHandGrabbing = ResourceLoader.Load("res://Assets/Graphics/Cursors/cursor-hand-grabbing.png");
    _cursorTarget = ResourceLoader.Load("res://Assets/Graphics/Cursors/cursor-target.png");
    _cursorIbeam = ResourceLoader.Load("res://Assets/Graphics/Cursors/cursor-ibeam.png");

    Input.SetCustomMouseCursor(_cursorIbeam, CursorShape.Ibeam, new Vector2(18, 18));

    UpdateCursors();

    _inputManager.Connect("controller_in_use_changed", this, nameof(ControllerInUseChanged));
  }

  public override void _Input(InputEvent evt)
  {
    if (evt.IsActionPressed("left_click"))
    {
      _isMousePressed = true;
      UpdateCursors();
    }
    else if (evt.IsActionReleased("left_click"))
    {
      _isMousePressed = false;
      UpdateCursors();
    }
  }

  public override void _Process(float delta)
  {
  }

  private void ControllerInUseChanged(bool controllerInUse)
  {
    if (controllerInUse)
    {
      Input.MouseMode = MouseModeEnum.Hidden;
    }
    else
    {
      Input.MouseMode = MouseModeEnum.Visible;
    }
  }

  public void ChangeCursorUiOpened(bool uiOpened)
  {
    _uiOpened = uiOpened;
    UpdateCursors();
  }

  private void UpdateCursors()
  {
    if (!_uiOpened)
    {
      Input.SetCustomMouseCursor(_cursorTarget, Input.CursorShape.Arrow, new Vector2(13, 13));
      Input.SetCustomMouseCursor(_cursorTarget, Input.CursorShape.PointingHand, new Vector2(13, 13));
      Input.SetCustomMouseCursor(_cursorTarget, Input.CursorShape.Drag, new Vector2(13, 13));
    }
    else if (_isDragging)
    {
      Input.SetCustomMouseCursor(_cursorHandGrabbing, Input.CursorShape.Arrow, new Vector2(2, 2));
      Input.SetCustomMouseCursor(_cursorHandGrabbing, Input.CursorShape.PointingHand, new Vector2(2, 2));
      Input.SetCustomMouseCursor(_cursorHandGrabbing, Input.CursorShape.Drag, new Vector2(2, 2));
    }
    else
    {
      if (!_isMousePressed)
      {
        Input.SetCustomMouseCursor(_cursorHand, Input.CursorShape.Arrow, new Vector2(2, 2));
        Input.SetCustomMouseCursor(_cursorHand, Input.CursorShape.PointingHand, new Vector2(2, 2));
        Input.SetCustomMouseCursor(_cursorHandCanGrab, Input.CursorShape.Drag, new Vector2(2, 2));
      }
      else
      {
        Input.SetCustomMouseCursor(_cursorHandClick, Input.CursorShape.Arrow, new Vector2(2, 2));
        Input.SetCustomMouseCursor(_cursorHandClick, Input.CursorShape.PointingHand, new Vector2(2, 2));
        Input.SetCustomMouseCursor(_cursorHandClick, Input.CursorShape.Drag, new Vector2(2, 2));
      }
    }
  }
}