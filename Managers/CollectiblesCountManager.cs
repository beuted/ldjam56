using Godot;

public class CollectiblesCountManager : Node
{
  [Signal] public delegate void coin_count_changed();

  public int CoinCount { get; private set; } = 0;

  public override void _Ready()
  {
  }

  public void InitWithSaveData(int? coinQuantity)
  {
    if (coinQuantity.HasValue)
      CoinCount = coinQuantity.Value;
    else
      CoinCount = 0;

    EmitSignal(nameof(coin_count_changed));
  }

  public void AddCoin(int coinQuantity)
  {
    // Safe guard
    if (coinQuantity < 0)
      GD.PrintErr("this method should not be used to remove coin: ", coinQuantity);

    CoinCount += coinQuantity;

    EmitSignal(nameof(coin_count_changed));
  }

  public void RemoveCoin(int coinQuantity)
  {
    // Safe guard
    if (coinQuantity < 0)
      GD.PrintErr("this method should not be used to add coin: ", coinQuantity);

    CoinCount -= coinQuantity;

    EmitSignal(nameof(coin_count_changed));
  }
}
