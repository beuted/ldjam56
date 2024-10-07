using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Player : KinematicBody2D
{
  [Signal] public delegate void player_health_changed();

  public string PlayerName { get; private set; }

  private float _pv = 100;
  public float PlayerLife
  {
    get { return _pv; }
    set
    {
      _pv = value;
      EmitSignal(nameof(player_health_changed), _pv, PlayerMaxLife);
    }
  }

  public int IsGrabbingVine = 0; // if > 0 => player is grabbing vine

  public static readonly float PlayerMaxLife = 100;

  private float _motionSpeed = 0;
  private const float _maxMotionSpeed = 130;
  private const float _horizontalAcceleration = 3000f;
  private const int _gravity = 20;
  private const int _jumpSpeed = 270;
  private bool _jumping = false;
  public bool IsJumping => _jumping;

  public bool CanGrab = false; // Power 1
  public bool CanBall = false; // Power 2

  private bool _goingUp;
  private bool _jump = false;
  private bool _balling = false;
  private bool _isGrounded = false;
  private float _lastTimeRooted = 0f; // Coyote time
  private float CoyoteTimeLimit = 0.15f; // Coyote time limit

  private Vector2 _speed = Vector2.Zero;
  private bool _bouncing;
  private Vector2 _aimingPosition;
  private float _inputDirection;

  // Nodes
  private Sprite _sprite;
  private Node2D _aboveHeadPointMushroom;
  private Node2D _aboveHeadPointSeed;
  private Node2D _aboveHeadPointCoin;
  private Node2D _aboveHeadPoint;
  private CollisionShape2D _collisionShape2D;
  private InputHelp _inputHelp;
  private AnimationPlayer _animationPlayerEffects;
  private AnimationPlayer _animationPlayer;

  // Singletons
  private CameraManager _cameraManager;
  private EffectManager _effectManager;
  private CollectiblesManager _collectiblesManager;
  private SoundManager _soundManager;

  // Scenes
  private PackedScene _spawnEffectScene;
  private PackedScene _smokePuffScene;
  private Dictionary<Collectible, bool> _collectiblesInRange;
  private TileMap _tileMap;
  private bool _goingDown;
  private Vector2 _aboveHeadStartPosition;

  public override void _Ready()
  {
    _collectiblesInRange = new Dictionary<Collectible, bool>();

    // Autoloads
    _cameraManager = GetNode<CameraManager>($"/root/{nameof(CameraManager)}"); // Singleton
    _effectManager = GetNode<EffectManager>($"/root/{nameof(EffectManager)}"); // Singleton
    _collectiblesManager = GetNode<CollectiblesManager>($"/root/{nameof(CollectiblesManager)}"); // Singleton
    _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton

    _animationPlayerEffects = GetNode<AnimationPlayer>("AnimationPlayerEffects");
    _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    _sprite = GetNode<Sprite>("Sprite");
    _aboveHeadPointMushroom = GetNode<Node2D>("AboveHead/Mushroom");
    _aboveHeadPointSeed = GetNode<Node2D>("AboveHead/Seed");
    _aboveHeadPointCoin = GetNode<Node2D>("AboveHead/Coin");
    _aboveHeadPoint = GetNode<Node2D>("AboveHead");
    _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
    _inputHelp = GetNode<InputHelp>("InputHelp");


    // Init stuff as invisible to be sure
    _aboveHeadPointMushroom.Visible = false;
    _aboveHeadPointSeed.Visible = false;
    _aboveHeadPointCoin.Visible = false;
  }

  public override void _Input(InputEvent ev)
  {
    _aimingPosition = GetGlobalMousePosition() - _aboveHeadPoint.GlobalPosition; // TODO: support controller
    _inputDirection = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");

    if ((ev.IsActionPressed("left_click") || ev.IsActionReleased("pickup")) && CanGrab && !_balling)
    {
      if (_aboveHeadPointMushroom.Visible || _aboveHeadPointSeed.Visible || _aboveHeadPointCoin.Visible)
      {
        DropPickup(_aimingPosition.Normalized());
      }
      else if (_collectiblesInRange != null && _collectiblesInRange.Count > 0)
      {
        _soundManager.PlayPickup();
        _collectiblesInRange.First().Key.PickUp(this);
      }
    }

    if (ev.IsActionPressed("jump"))
    {
      if (!_jumping)
      {
        _jump = true;
        _soundManager.PlayJump();
      }
      _jumping = true;
    }

    if (ev.IsActionReleased("jump"))
    {
      _jumping = false;
    }

    if (ev.IsActionPressed("move_down"))
    {
      _goingDown = true;
    }
    else if (ev.IsActionReleased("move_down"))
    {
      _goingDown = false;
    }

    if (ev.IsActionPressed("move_up"))
    {
      _goingUp = true;
    }
    else if (ev.IsActionReleased("move_up"))
    {
      _goingUp = false;
    }

    if (ev.IsActionPressed("ball") && CanBall)
    {
      _balling = true;
      (_collisionShape2D.Shape as RectangleShape2D).Extents = new Vector2(3.5f, 3.5f);
      DropPickup(Vector2.Zero);
    }

    if (ev.IsActionReleased("ball"))
    {
      (_collisionShape2D.Shape as RectangleShape2D).Extents = new Vector2(3.5f, 5.5f);
      _balling = false;
    }
  }

  public override void _Process(float delta)
  {
    // Input help visible or not
    if (CanGrab && !(_aboveHeadPointMushroom.Visible || _aboveHeadPointSeed.Visible || _aboveHeadPointCoin.Visible) && _collectiblesInRange != null && _collectiblesInRange.Count > 0 && !_balling)
    {
      _inputHelp.Visible = true;
    }
    else
    {
      _inputHelp.Visible = false;
    }
  }

  public override void _PhysicsProcess(float delta)
  {
    // Reseting when off map
    if (Position.y > 1000f)
    {
      DropPickup(Vector2.Zero);
      Position = PlayerManager.StartPosition;
    }

    // Roll sound
    if (_balling && _isGrounded)
    {
      _soundManager.PlayRoll();
    }
    else
    {
      _soundManager.StopRoll();
    }


    // Animation stuff
    if (_bouncing)
    {
      // When bouncing we rorate the other way
      _animationPlayer.Play("Ball");
      _sprite.Rotate((_sprite.FlipH ? 1f : -1f) * 10f * delta);
    }
    else if (_balling)
    {
      _animationPlayer.Play("Ball");
      _sprite.Rotate((_sprite.FlipH ? -1f : 1f) * 30f * delta);
    }
    else if (IsGrabbingVine > 0)
    {

    }
    else if (_inputDirection < 0)
    {
      _sprite.Rotation = 0f;
      _sprite.FlipH = true;
      _animationPlayer.Play("Walk");
    }
    else if (_inputDirection > 0)
    {
      _sprite.Rotation = 0f;
      _sprite.FlipH = false;
      _animationPlayer.Play("Walk");
    }
    else
    {
      _sprite.Rotation = 0f;
      _animationPlayer.Play("Idle");
    }

    // Friction if _inputDirection = 0
    if (_speed.x >= 0)
      _speed.x = Mathf.Max(0.0f, _speed.x - 2000f * delta);
    else
      _speed.x = Mathf.Min(0.0f, _speed.x + 2000f * delta);

    if (_speed.x == 0f)
      _bouncing = false; // Stop the bouncing when we reach 0f

    // Max speed is faster when balling, acceleration is the same
    var realMaxMotionSpeed = _balling ? _maxMotionSpeed * 1.5f : _maxMotionSpeed;

    // We increase progressively the speed not suddenly
    var direction = _balling ? (_sprite.FlipH ? -1f : 1f) : _inputDirection;
    if (!_bouncing)
      _speed.x = Mathf.Max(-realMaxMotionSpeed, Mathf.Min(realMaxMotionSpeed, _speed.x + delta * _horizontalAcceleration * direction));

    // Gravity
    if (!(IsGrabbingVine > 0))
      _speed.y += _gravity;

    // Jump
    if (_jump && (_isGrounded || _lastTimeRooted < CoyoteTimeLimit) && !(IsGrabbingVine > 0))
    {
      _speed.y = -_jumpSpeed;
      if (_isGrounded || _lastTimeRooted < CoyoteTimeLimit)
        _lastTimeRooted = 10000f; // disable coyote time until we touch floor again (if we used it or if we jumped from the floor)
    }

    // Jump Pro tip: inverted gravity
    if (_jumping && !(IsGrabbingVine > 0))
    {
      _speed.y -= _gravity / 3;

      if (_speed.y < 100 && !_isGrounded && !_balling && !(IsGrabbingVine > 0))
      {
        _animationPlayer.Play("Jump");
      }
    }

    // Walking up on vine
    if (IsGrabbingVine > 0 && _goingUp)
    {
      _speed.y = -100f;
    }
    else if (IsGrabbingVine > 0 && _goingDown)
    {
      _speed.y = 100f;
    }
    else if (IsGrabbingVine > 0)
    {
      _speed.y = 0f;
    }

    if (_speed.y > 100 && !_balling && !(IsGrabbingVine > 0))
    {
      _animationPlayer.Play("Down");
    }

    var prevSpeed = _speed;

    _speed = MoveAndSlide(_speed, Vector2.Up, false, 4, Mathf.Pi / 4f, false);

    _isGrounded = IsOnFloor();
    if (_isGrounded)
    {
      if (prevSpeed.y > 300f)
      {
        // if jump for more than half a sec
        _soundManager.PlayLand(prevSpeed.y);
        if (prevSpeed.y > 400f)
        {
          _effectManager.GenerateWalkSmoke(Position + new Vector2(3f, 3f));
          _effectManager.GenerateWalkSmoke(Position + new Vector2(-3f, 3f));
        }
      }
      _lastTimeRooted = 0;
    }
    else
    {
      _lastTimeRooted += delta;
    }

    // If we succeed to jump or not whatever just set back to false and way for next jump button press
    _jump = false;

    // Collisions stuff
    for (var i = 0; i < GetSlideCount(); i++)
    {
      var collision = GetSlideCollision(i);

      var colliderPhysicBody2d = collision.Collider as RigidBody2D;
      if (colliderPhysicBody2d != null && colliderPhysicBody2d.IsInGroup("collectibles"))
      {
        // Every collectibles that can be pushed
        // NB: This is needed because we set infinit inertia to false in MoveAndSlide above
        // We move onyl on the horizontal axis and only if the collision is horizontale
        // We add a small component upward to avoid the item getting stuck on the ground
        var horizontalComponent = (-collision.Normal).Dot(Vector2.Right);
        if (horizontalComponent > 0.2f || horizontalComponent < -0.2f)
        {
          var pushDirection = ((horizontalComponent * Vector2.Right) + (0.4f * Vector2.Up)).Normalized();
          colliderPhysicBody2d.ApplyCentralImpulse(pushDirection * 30f);
        }

        if (collision.Collider is MushroomCap mushroomCap)
        {
          if (collision.Normal.y < -0.9f)
          {
            // If we collide from above
            _speed = new Vector2(_speed.x, -500f);
            _soundManager.PlayBounce();
          }
        }
      }
      else if (collision.Collider as Mushroom != null)
      {
        if (collision.Normal.y < -0.9f)
        {
          // If we collide from above we BOUNCE
          _speed = new Vector2(_speed.x, -500f);
          _soundManager.PlayBounce();
        }
        if ((collision.Normal.x < -0.9f || collision.Normal.x > 0.9f) && _balling)
        {
          // If we collide horizontally while balling we loose the cap of the mushroom
          (collision.Collider as Mushroom).LooseCap();
        }
      }
      else
      {
        // Hit wall or enemy
      }

      // Bounce when balling on wall or other
      if (collision.Normal.x < -0.9f && _balling)
      {
        // If we collide from the right
        _speed = new Vector2(-200f, _speed.y);
        _bouncing = true;
        _soundManager.PlayBounce(true);
      }
      else if (collision.Normal.x > 0.9f && _balling)
      {
        // If we collide from the left
        _speed = new Vector2(200f, _speed.y);
        _bouncing = true;
        _soundManager.PlayBounce(true);
      }
    }

  }

  public void Init(string playerName, Vector2 playerPosition, TileMap tileMap)
  {
    PlayerName = playerName;
    Position = playerPosition;
    _tileMap = tileMap;

    _effectManager.GenerateSpawnEffect(Position);
    _cameraManager.Blast(1f, 1f);
  }

  public void TakeDamage(float dmg)
  {
    PlayerLife -= dmg;

    _cameraManager.AddTrauma(0.3f);
    _cameraManager.SetDamageVignette(1f - PlayerLife / PlayerMaxLife);
    _effectManager.SpawnFloatingText($"-{dmg}", Position, "ff0000");
    _animationPlayerEffects.Play("TakeDamage");
  }

  public void Heal(float heal)
  {
    if (PlayerLife >= PlayerMaxLife)
      return;

    var newPv = Mathf.Min(PlayerLife + heal, PlayerMaxLife);
    var actualHeal = newPv - PlayerLife;

    PlayerLife = newPv;
    // If we want floating point diplay ? string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:f}", heal)
    _effectManager.SpawnFloatingText(((int)actualHeal).ToString(), Position, "00ff00");
  }

  public void GenerateWalkSmoke()
  {
    if (_isGrounded)
      _effectManager.GenerateWalkSmoke(Position + new Vector2(0, 3f));
  }

  internal void SetAboveHead(Collectible collectible)
  {
    _aboveHeadStartPosition = collectible.StartPosition;

    if (collectible is Seed)
      _aboveHeadPointSeed.Visible = true;
    else if (collectible is MushroomCap)
      _aboveHeadPointMushroom.Visible = true;
    else if (collectible is Coin || collectible is Flower)
      _aboveHeadPointCoin.Visible = true;
    else
      GD.PrintErr("Error in SetAboveHead: collectible not recognized");
  }


  public void OnPickUpZoneEnter(Node body)
  {

    if (body is Collectible collectible)
    {
      // TODO: Make pickup stuff visible is _mushroomCapInRange not empty
      _collectiblesInRange.Add(collectible, true);
    }
  }

  public void OnPickUpZoneLeave(Node body)
  {
    if (body is Collectible collectible)
    {
      if (_collectiblesInRange.TryGetValue(collectible, out _))
      {
        _collectiblesInRange.Remove(collectible);
      }

      // TODO: Make pickup stuff invisible if _mushroomCapInRange empty
    }
  }

  public void DropPickup(Vector2 direction)
  {
    if (_aboveHeadPointMushroom.Visible)
    {
      _collectiblesManager.DropMushroomCapOnMapMaster(_aboveHeadPoint.GlobalPosition, _speed + 50f * direction, _aboveHeadStartPosition);
      _aboveHeadPointMushroom.Visible = false;
    }
    else if (_aboveHeadPointSeed.Visible)
    {
      _collectiblesManager.DropSeedOnMapMaster(_aboveHeadPoint.GlobalPosition, _speed + 50f * direction, _tileMap, _aboveHeadStartPosition);
      _aboveHeadPointSeed.Visible = false;
    }
    else if (_aboveHeadPointCoin.Visible)
    {
      _collectiblesManager.DropCoinOnMapMaster(_aboveHeadPoint.GlobalPosition, _speed + 50f * direction, _aboveHeadStartPosition);
      _aboveHeadPointCoin.Visible = false;

    }
  }
}
