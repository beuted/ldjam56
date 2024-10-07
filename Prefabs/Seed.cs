using Godot;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;

public class Seed : Collectible
{
    private Vector2 _velocity = Vector2.Zero;
    private CollectiblesManager _collectiblesManager;
    private PlayerManager _playerManager;
    private TimeManager _timeManager;
    private PackedScene _vineScene;
    private AnimationPlayer _animationPlayer;
    private Area2D _area2D;
    public string UID = Guid.NewGuid().ToString();

    private Player _playerIsBelow;

    private ulong _lastDetectionTick = 0;
    public ulong DetectionTickPeriod = 50;

    public TileMap TileMap;
    private int _collideWithVine;

    public Seed() : base()
    {
    }

    public override void _Ready()
    {
        _collectiblesManager = GetNode<CollectiblesManager>($"/root/{nameof(CollectiblesManager)}"); // Singleton
        _playerManager = GetNode<PlayerManager>($"/root/{nameof(PlayerManager)}"); // Singleton
        _timeManager = GetNode<TimeManager>($"/root/{nameof(TimeManager)}"); // Singleton

        _vineScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Vine.tscn");

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _area2D = GetNode<Area2D>("Area2D");
    }

    public override void _Process(float delta)
    {
        var now = _timeManager.GameTimeSinceStartUlong;

        if (now > _lastDetectionTick + DetectionTickPeriod)
        {
            _lastDetectionTick = now;
            DetectTileMapCollision();
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (Position.y > 1000f)
        {
            // Restart where you where at first
            // Changing position wasn't working wtf !!!
            _collectiblesManager.DropSeedOnMapMaster(StartPosition, Vector2.Zero, TileMap, StartPosition);

            QueueFree();  //TODO: I should use _collectiblesManager instead of just queue free if I want to allow saving mid term
            return;
        }

        if (_playerIsBelow != null)
        {
            Position = Position + new Vector2(-2f * delta, -2f * delta);
            if (_playerIsBelow.IsJumping)
            {
                ApplyCentralImpulse(new Vector2(0, -20f));
            }
        }
    }

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        // Prevent the MushroomCap from rotating
        var rotation_radians = Mathf.Deg2Rad(RotationDegrees);
        var new_rotation = 0;//Mathf.Clamp(rotation_radians, -0.2f, 0.2f);
        var new_transform = new Transform2D(new_rotation, Position);
        state.Transform = new_transform;
    }

    public void DetectTileMapCollision()
    {
        if (TileMap != null)
        {
            var positionTile = TileMap.WorldToMap(Position + Vector2.Down * 8f);
            var tile = TileMap.GetCellv(positionTile);
            if (tile == 38 && !(_collideWithVine > 0))
            {
                GrowIntoVine(positionTile);

                GetParent().RemoveChild(this);
                QueueFree();
            }
        }
    }

    private void GrowIntoVine(Vector2 tilePosition)
    {
        var vineInstance = _vineScene.Instance() as Vine;
        vineInstance.Position = tilePosition * 16f + new Vector2(8f, 0);
        vineInstance.TileMap = TileMap;

        GetParent().GetNode<YSort>("VineContainer").AddChild(vineInstance);
    }

    public void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            _playerIsBelow = player;
        }

        // We bounce on muhsroom and mushroom cap only if above them
        if (body is MushroomCap mushroomCap)
        {
            if (Position.y < mushroomCap.Position.y)
                ApplyCentralImpulse(new Vector2(0, -200f));
        }
        if (body is Mushroom mushroom)
        {
            if (Position.y < mushroom.Position.y)
                ApplyCentralImpulse(new Vector2(0, -200f));
        }
    }

    public void OnBodyExited(Node body)
    {
        if (body is Player player)
        {
            _playerIsBelow = null;
        }
    }

    public void OnArea2dEntered(Area2D body)
    {
        if (body is Vine vine)
        {
            _collideWithVine += 1;
        }
    }

    public void OnArea2dExited(Area2D body)
    {
        if (body is Vine vine)
        {
            _collideWithVine -= 1;
        }
    }
}
