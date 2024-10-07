using Godot;
using System;

public class SoundManager : Node
{
    private AudioStreamPlayer _audioStreamPlayerMusic;
    private AudioStreamPlayer _audioStreamPlayerClick;
    private AudioStreamPlayer _audioStreamPlayerJump;
    private AudioStreamPlayer _audioStreamPlayerLand;
    private AudioStreamPlayer _audioStreamPlayerPickup;
    private AudioStreamPlayer _audioStreamPlayerPowerUpNappe;
    private AudioStreamPlayer _audioStreamPlayerPowerUp;
    private AudioStreamPlayer _audioStreamPlayerBounce;
    private AudioStreamPlayer _audioStreamPlayerVine;
    private AudioStreamPlayer _audioStreamPlayerRoll;
    private AudioStreamPlayer _audioStreamPlayerOpeningDoor;
    private AudioStreamPlayer _audioStreamPlayerFillingDoor;
    private int _fxBusIndex;
    private int _musicBusIndex;
    private bool _isMusicMuted = false;
    private bool _areEffectMuted = false;

    public override void _Ready()
    {
        _audioStreamPlayerMusic = GetNode<AudioStreamPlayer>($"AudioStreamPlayerMusic");
        _audioStreamPlayerClick = GetNode<AudioStreamPlayer>($"AudioStreamPlayerClick");
        _audioStreamPlayerJump = GetNode<AudioStreamPlayer>($"AudioStreamPlayerJump");
        _audioStreamPlayerLand = GetNode<AudioStreamPlayer>($"AudioStreamPlayerLand");
        _audioStreamPlayerPickup = GetNode<AudioStreamPlayer>($"AudioStreamPlayerPickup");
        _audioStreamPlayerPowerUpNappe = GetNode<AudioStreamPlayer>($"AudioStreamPlayerPowerUpNappe");
        _audioStreamPlayerPowerUp = GetNode<AudioStreamPlayer>($"AudioStreamPlayerPowerUp");
        _audioStreamPlayerBounce = GetNode<AudioStreamPlayer>($"AudioStreamPlayerBounce");
        _audioStreamPlayerVine = GetNode<AudioStreamPlayer>($"AudioStreamPlayerVine");
        _audioStreamPlayerRoll = GetNode<AudioStreamPlayer>($"AudioStreamPlayerRoll");
        _audioStreamPlayerOpeningDoor = GetNode<AudioStreamPlayer>($"AudioStreamPlayerOpeningDoor");
        _audioStreamPlayerFillingDoor = GetNode<AudioStreamPlayer>($"AudioStreamPlayerFillingDoor");

        _fxBusIndex = AudioServer.GetBusIndex("Fx");
        _musicBusIndex = AudioServer.GetBusIndex("Music");
    }

    public void Init()
    {
        _audioStreamPlayerMusic.Bus = "Music";
        _audioStreamPlayerClick.Bus = "Fx";
        _audioStreamPlayerJump.Bus = "Fx";
        _audioStreamPlayerLand.Bus = "Fx";
        _audioStreamPlayerPickup.Bus = "Fx";
        _audioStreamPlayerPowerUpNappe.Bus = "Fx";
        _audioStreamPlayerPowerUp.Bus = "Fx";
        _audioStreamPlayerBounce.Bus = "Fx";
        _audioStreamPlayerVine.Bus = "Fx";
        _audioStreamPlayerRoll.Bus = "Fx";
        _audioStreamPlayerOpeningDoor.Bus = "Fx";
        _audioStreamPlayerFillingDoor.Bus = "Fx";
    }

    public void PlayMusic()
    {
        _audioStreamPlayerMusic.Play();
    }

    public void PlayClick()
    {
        _audioStreamPlayerClick.Play();
    }

    public void PlayJump()
    {
        _audioStreamPlayerJump.PitchScale = RandomGeneratorService.Random.RandfRange(0.8f, 1.2f);
        _audioStreamPlayerJump.Play();
    }

    public void PlayLand(float speed)
    {
        if (speed < 300f)
            return;

        var volume = Mathf.Min(speed - 300f, 200f) / 200f;
        _audioStreamPlayerLand.VolumeDb = -15f + 10f * volume;
        _audioStreamPlayerLand.PitchScale = RandomGeneratorService.Random.RandfRange(0.8f, 1.2f);
        _audioStreamPlayerLand.Play();
    }

    public void PlayPickup()
    {
        _audioStreamPlayerJump.PitchScale = RandomGeneratorService.Random.RandfRange(0.4f, 0.6f);
        _audioStreamPlayerPickup.Play();
    }

    public void PlayPowerUp()
    {
        _audioStreamPlayerPowerUp.Play();
        _audioStreamPlayerPowerUpNappe.Play();
    }

    public void PlayBounce(bool smallBounce = false)
    {
        if (smallBounce)
            _audioStreamPlayerBounce.VolumeDb = -10f;
        else
            _audioStreamPlayerBounce.VolumeDb = 0f;
        _audioStreamPlayerBounce.PitchScale = RandomGeneratorService.Random.RandfRange(0.9f, 1.1f);
        _audioStreamPlayerBounce.Play();
    }

    public void PlayVine()
    {
        _audioStreamPlayerVine.Play();
    }

    public void StopVine()
    {
        _audioStreamPlayerVine.Stop();
    }

    public void PlayRoll()
    {
        if (!_audioStreamPlayerRoll.Playing)
            _audioStreamPlayerRoll.Play();
    }

    public void StopRoll()
    {
        _audioStreamPlayerRoll.Stop();
    }


    public void PlayOpeningDoor()
    {
        _audioStreamPlayerOpeningDoor.Play();
    }

    public void PlayFillingDoor()
    {
        _audioStreamPlayerFillingDoor.Play();
    }


    public bool ToggleMuteMusic()
    {
        if (!_isMusicMuted)
        {
            AudioServer.SetBusMute(_musicBusIndex, true);
        }
        else
        {
            AudioServer.SetBusMute(_musicBusIndex, false);
        }

        _isMusicMuted = !_isMusicMuted;

        return !_isMusicMuted;
    }

    public void ToggleMuteEffects()
    {
        if (!_areEffectMuted)
        {
            AudioServer.SetBusMute(_fxBusIndex, true);
        }
        else
        {
            AudioServer.SetBusMute(_fxBusIndex, false);
        }

        _areEffectMuted = !_areEffectMuted;
    }

    // volumePercent between 0 and 1
    internal void ChangeFxVolumePercent(float volumePercent)
    {
        AudioServer.SetBusVolumeDb(_fxBusIndex, GD.Linear2Db(volumePercent));
    }

    internal void ChangeMusicVolumePercent(float volumePercent)
    {
        AudioServer.SetBusVolumeDb(_musicBusIndex, GD.Linear2Db(volumePercent));
    }

    internal float GetFxVolumePercent()
    {
        return GD.Db2Linear(AudioServer.GetBusVolumeDb(_fxBusIndex));
    }

    internal float GetMusicVolumePercent()
    {
        return GD.Db2Linear(AudioServer.GetBusVolumeDb(_musicBusIndex));
    }
}
