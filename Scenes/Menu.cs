using Godot;
using System;

public class Menu : Control
{
  private SceneTransition _sceneTransition;
  private SoundManager _soundManager;
  private CursorManager _cursorManager;
  private SaveManager _saveManager;

  private Options _options;
  private BetterButton _playButton;
  private Control _optionsButton;
  private Control _focusControl = null; // We use a custom focus because the default logic didn't allow to switch easily between mouse controls and gamepad controls

  public override void _Ready()
  {
    // Autoloads
    _sceneTransition = (SceneTransition)GetNode($"/root/{nameof(SceneTransition)}"); // Singleton
    _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton
    _cursorManager = (CursorManager)GetNode($"/root/{nameof(CursorManager)}"); // Singleton
    _saveManager = (SaveManager)GetNode($"/root/{nameof(SaveManager)}"); // Singleton

    // UI elements
    _options = GetNode<Options>("Options");
    _options.HideOptions();

    // Setup the default focus for joypad menu control
    _playButton = GetNode<BetterButton>("Panel/VBoxContainer/PlayButton");
    _optionsButton = GetNode<Control>("Panel/VBoxContainer/OptionsButton");

    // Start Music
    _soundManager.Init();
    _soundManager.PlayMusic();

    _cursorManager.ChangeCursorUiOpened(true);

    RectScale = _saveManager.ConfigData.UiScaleFactor * Vector2.One;
    RectSize = GetViewport().Size / _saveManager.ConfigData.UiScaleFactor;

    _focusControl = _playButton;
    _playButton.BetterFocus = true;
  }


  public override void _Input(InputEvent evt)
  {
    if (_focusControl == null || _options.Visible == true)
      return;

    this.UpdateFocusBasedOnEvent(evt, ref _focusControl);
  }

  // Main
  public void OnClickPlay()
  {
    // New game
    _saveManager.ClearSaveData();
    _sceneTransition.FadeTo("Scenes/Main.tscn");
    _soundManager.PlayClick();
  }

  public void OnClickContinue()
  {
    // Continue game
    _saveManager.ReadSaveData();
    _sceneTransition.FadeTo("Scenes/Main.tscn");
    _soundManager.PlayClick();
  }

  public void OnClickExit()
  {
    GetTree().Quit();
  }

  // Options

  public void OnClickOptions()
  {
    _options.ShowOptions();
  }

  public void OnClickBack()
  {
    _options.HideOptions();
  }
}
