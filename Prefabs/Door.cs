using Godot;
using System;

public class Door : StaticBody2D
{
    public int NbCoins = 0;
    private CameraManager _cameraManager;
    private SoundManager _soundManager;

    private AnimationPlayer _animationPlayer;
    private Sprite _spriteCoin1;
    private Sprite _spriteCoin2;
    private Sprite _spriteCoin3;


    public override void _Ready()
    {
        _cameraManager = (CameraManager)GetNode($"/root/{nameof(CameraManager)}"); // Singleton
        _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _spriteCoin1 = GetNode<Sprite>("Sprite/Coin1");
        _spriteCoin2 = GetNode<Sprite>("Sprite/Coin2");
        _spriteCoin3 = GetNode<Sprite>("Sprite/Coin3");

        _spriteCoin1.Visible = false;
        _spriteCoin2.Visible = false;
        _spriteCoin3.Visible = false;
    }

    public void Open()
    {
        _animationPlayer.Play("Open");
        _soundManager.PlayOpeningDoor();
    }

    internal void IncrementCoin()
    {
        _soundManager.PlayFillingDoor();
        NbCoins++;

        _cameraManager.AddTrauma(0.3f);

        if (NbCoins >= 1)
        {
            _spriteCoin1.Visible = true;
        }
        if (NbCoins >= 2)
        {
            _spriteCoin2.Visible = true;
        }
        if (NbCoins >= 3)
        {
            _spriteCoin3.Visible = true;
        }

        if (NbCoins == 3)
        {
            Open();
        }
    }

    public void Shake()
    {
        _cameraManager.AddTrauma(0.1f);
    }
}
