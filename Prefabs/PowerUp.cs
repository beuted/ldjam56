using Godot;
using System;

public class PowerUp : Area2D
{
    private Sprite _sprite;
    private AnimationPlayer _animationPlayer;
    private MessagesManager _messagesManager;
    private CameraManager _cameraManager;
    private SoundManager _soundManager;

    [Export] public bool GrabPowerUp = false;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Sprite");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _messagesManager = (MessagesManager)GetNode($"/root/{nameof(MessagesManager)}"); // Singleton
        _cameraManager = (CameraManager)GetNode($"/root/{nameof(CameraManager)}"); // Singleton
        _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton

        _sprite.Frame = GrabPowerUp ? 319 : 320;
    }

    public void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            if (GrabPowerUp)
                player.CanGrab = true;
            else if (!GrabPowerUp)
                player.CanBall = true;
        }

        if (!_animationPlayer.IsPlaying())
        {
            _animationPlayer.Play("Explosion");
            _cameraManager.AddTrauma(0.6f);
            _soundManager.PlayPowerUp();
        }

    }

    public void Destroy()
    {
        if (GrabPowerUp)
            _messagesManager.ShowMessagePickup();
        else if (!GrabPowerUp)
            _messagesManager.ShowMessageBalling();

        QueueFree();
    }
}
