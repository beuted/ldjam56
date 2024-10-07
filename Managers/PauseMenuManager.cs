using Godot;
using System;
using System.Linq;

public class PauseMenuManager : Node
{
  private PauseMenu _pauseMenu;

  public override void _Ready()
  {
    PauseMode = PauseModeEnum.Process;
  }

  public void Init(PauseMenu pauseMenu)
  {
    _pauseMenu = pauseMenu;
  }

  public void ShowPauseMenu()
  {
    _pauseMenu.ShowPauseMenu();
  }

  public void HidePauseMenu()
  {
    _pauseMenu.HidePauseMenu();
  }

  public void TogglePauseMenu()
  {
    _pauseMenu.TogglePauseMenu();
  }

  public bool IsPauseMenuOpened()
  {
    return _pauseMenu.Visible;
  }

}