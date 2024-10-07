using Godot;
using System;

public class PauseMenu : Control
{
  private SceneTransition _sceneTransition;
  private CursorManager _cursorManager;
  private SaveManager _saveManager;
  private Options _options;
  private BetterButton _resumeButton;
  private Control _optionsButton;
  private Control _focusControl = null; // We use a custom focus because the default logic didn't allow to switch easily between mouse controls and gamepad controls

  public override void _Ready()
  {
    // Autoloads
    _sceneTransition = (SceneTransition)GetNode($"/root/{nameof(SceneTransition)}"); // Singleton
    _cursorManager = (CursorManager)GetNode($"/root/{nameof(CursorManager)}"); // Singleton
    _saveManager = (SaveManager)GetNode($"/root/{nameof(SaveManager)}"); // Singleton

    // UI elements
    _options = GetNode<Options>("Options");
    _options.HideOptions();

    // Setup the default focus for joypad menu control
    _resumeButton = GetNode<BetterButton>("Panel/VBoxContainer/ResumeButton");
    _optionsButton = GetNode<Control>("Panel/VBoxContainer/OptionsButton");

    RectScale = _saveManager.ConfigData.UiScaleFactor * Vector2.One;
    RectSize = GetViewport().Size / _saveManager.ConfigData.UiScaleFactor;

    // Set the default focus
    _focusControl = _resumeButton;
    _resumeButton.BetterFocus = true;
  }

  public override void _Input(InputEvent evt)
  {
    if (evt.IsActionPressed("toggle_pause_menu"))
    {
      TogglePauseMenu();
    }

    if (!Visible)
      return;

    // Menu focus moving control
    this.UpdateFocusBasedOnEvent(evt, ref _focusControl);
  }

  public void ShowPauseMenu()
  {
    _cursorManager.ChangeCursorUiOpened(true);
    GetTree().Paused = true;
    Visible = true;
  }

  public void HidePauseMenu()
  {
    _options.HideOptions(); // We close the options if the pause menu is hidden

    _cursorManager.ChangeCursorUiOpened(false);
    GetTree().Paused = false;
    Visible = false;
  }

  public void TogglePauseMenu()
  {
    if (Visible)
      HidePauseMenu();
    else
      ShowPauseMenu();
  }

  public void QuitToMenu()
  {
    GetTree().Paused = false;
    _saveManager.SaveGame();
    _sceneTransition.FadeTo("Scenes/Menu.tscn");
  }

  public void OnClickOptions()
  {
    _options.ShowOptions();
  }

  public void OnClickBackOnOptions()
  {
    _options.HideOptions();
  }
}
