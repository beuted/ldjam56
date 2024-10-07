using Godot;
using System;
using System.Threading.Tasks;

public class Options : Control
{
    private SoundManager _soundManager;
    private SaveManager _saveManager;

    private int _savesInProgress = 0;
    private BetterButton _defaultFocusControl;
    private Control _focusControl = null;

    [Signal] public delegate void options_closed();

    public override void _Ready()
    {
        // Autoloads
        _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton
        _saveManager = (SaveManager)GetNode($"/root/{nameof(SaveManager)}"); // Singleton

        var musicSlider = GetNode<HSlider>("VBoxContainer/Music/HSlider");
        musicSlider.Value = _soundManager.GetMusicVolumePercent();

        var fxSlider = GetNode<HSlider>("VBoxContainer/Fx/HSlider");
        fxSlider.Value = _soundManager.GetFxVolumePercent();

        _defaultFocusControl = GetNode<BetterButton>("VBoxContainer/FullscreenToggleButton");
    }

    public override void _Input(InputEvent evt)
    {
        if (_focusControl == null || Visible == false) return;

        this.UpdateFocusBasedOnEvent(evt, ref _focusControl);
    }

    public void ShowOptions()
    {
        SetDefaultFocus();
        Visible = true;
    }

    public void SetDefaultFocus()
    {
        _focusControl = _defaultFocusControl;
        _defaultFocusControl.BetterFocus = true;
    }

    public void HideOptions()
    {
        Visible = false;
    }

    public void OnClickToggleFullscreen()
    {
        OS.WindowFullscreen = !OS.WindowFullscreen;
        UpdateSaveFile();
    }

    public void ChangedMusicVolume(float value)
    {
        _soundManager.ChangeMusicVolumePercent(value);
        UpdateSaveFile();

    }

    public void ChangedFxVolume(float value)
    {
        _soundManager.ChangeFxVolumePercent(value);
        UpdateSaveFile();
    }

    public void UpdateSaveFile()
    {
        // Debounce all saves
        _savesInProgress++;
        GetTree().CreateTimer(1, true).Connect("timeout", this, "UpdateSaveFileCb"); // We avoid using await due to https://www.reddit.com/r/godot/comments/x5p9w9/how_to_destroy_a_scene_while_its_awaiting_a_signal/
    }

    public void UpdateSaveFileCb()
    {
        if (_savesInProgress > 1)
        {
            _savesInProgress--;
            return;
        }
        _savesInProgress--;

        var newConfigData = _saveManager.ConfigData;

        newConfigData.FullScreen = OS.WindowFullscreen;
        newConfigData.FxVolume = _soundManager.GetFxVolumePercent();
        newConfigData.MusicVolume = _soundManager.GetMusicVolumePercent();

        _saveManager.SaveConfig(newConfigData);
    }

    public void Close()
    {
        EmitSignal(nameof(options_closed));
    }
}
