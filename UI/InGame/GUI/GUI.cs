using Godot;
using System;
using System.Threading.Tasks;

public class GUI : Control
{
  private CollectiblesCountManager _collectiblesCountManager;

  private Control _topLeft;
  private Label _healthAmount;
  private Label _coinsAmount;
  private Label _fpsCounter;
  private Control _bottomRight;
  private TextureProgress _healthBar;
  private Control _topRight;
  private Player _player;

  public override void _Ready()
  {
    _collectiblesCountManager = GetNode<CollectiblesCountManager>($"/root/{nameof(CollectiblesCountManager)}"); // Singleton

    // Top Left
    _topLeft = GetNode<Control>("TopLeft");
    _healthBar = GetNode<TextureProgress>("TopLeft/HealthBarBg/HealthBar");
    _healthAmount = GetNode<Label>("TopLeft/HealthBarBg/HealthAmount");

    // Top Right
    _topRight = GetNode<Control>("TopRight");
    _coinsAmount = GetNode<Label>("TopRight/CollectiblesCoins/CoinsAmount");
    _fpsCounter = GetNode<Label>("TopRight/FpsCounter");

    //Bottom right
    _bottomRight = GetNode<Control>("BottomRight");

    _healthBar.Value = 100;
    _healthAmount.Visible = false;

    // TODO: Should be configurable with "UiScale" an option in the menu
    var zoomFactor = 3f;

    _topLeft.RectScale = new Vector2(zoomFactor, zoomFactor);
    _topLeft.AnchorLeft = 0;
    _topLeft.AnchorTop = 0;

    _topRight.RectScale = new Vector2(zoomFactor, zoomFactor);
    _topRight.AnchorRight = 1f / zoomFactor;
    _topRight.AnchorTop = 0;

    _bottomRight.RectScale = new Vector2(zoomFactor, zoomFactor);
    _bottomRight.AnchorRight = 1f / zoomFactor;
    _bottomRight.AnchorBottom = 1f / zoomFactor;
  }

  public void Init(Player player)
  {
    // Init
    _player = player;

    // Connect signals
    _player.Connect("player_health_changed", this, nameof(ChangeHealthValue));

    _collectiblesCountManager.Connect("coin_count_changed", this, nameof(ChangeCoinsValue));

    // Update values on first display
    ChangeHealthValue(_player.PlayerLife, Player.PlayerMaxLife);
    ChangeCoinsValue();
  }

  public override void _Process(float delta)
  {
    _fpsCounter.Text = ((int) Engine.GetFramesPerSecond()).ToString();
  }

  public void ShowAmounts()
  {
    _healthAmount.Visible = true;
  }

  public void HideAmounts()
  {
    _healthAmount.Visible = false;
  }


  public void ChangeHealthValue(float pv, float maxPv)
  {
    _healthBar.Value = 100 * (pv / maxPv);
    _healthAmount.Text = $"{Mathf.Floor(pv)}/{Mathf.Floor(maxPv)}";
  }

  public void ChangeCoinsValue()
  {
    _coinsAmount.Text = $"{_collectiblesCountManager.CoinCount}";
  }

}
