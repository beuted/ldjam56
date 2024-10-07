using Godot;
using System;

public class SplashScreen : Control
{
    private SceneTransition _sceneTransition;
    private CursorManager _cursorManager;
    private AnimationPlayer _animationPlayer;

    public override void _Ready()
    {
        // Autoloads
        _sceneTransition = (SceneTransition)GetNode($"/root/{nameof(SceneTransition)}"); // Singleton
        _cursorManager = (CursorManager)GetNode($"/root/{nameof(CursorManager)}"); // Singleton

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _cursorManager.ChangeCursorUiOpened(true);

        // Hack: In order for it to stop resizing
        var godotLogo = GetNode<TextureRect>("GodotLogo");
        godotLogo.RectSize = GetViewport().Size;
        var dekajooLogo = GetNode<TextureRect>("DekajooLogo");
        dekajooLogo.RectSize = GetViewport().Size;
    }

    public override void _Input(InputEvent evt)
    {
        if (evt.IsActionPressed("left_click") || evt.IsActionPressed("ui_accept"))
        {
            if (_animationPlayer.CurrentAnimationPosition < 4.0f)
                _animationPlayer.Seek(3.9f, true);
            else if (_animationPlayer.CurrentAnimationPosition < 8.0f)
                _animationPlayer.Seek(7.9f, true);
        }
    }

    public void GoToMenuScene()
    {
        _sceneTransition.FadeTo("Scenes/Menu.tscn");
    }
}
