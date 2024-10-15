using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;


public struct CoinData
{
  public Vector2 Position;
  public Vector2 StartPosition;
}

public struct MushroomCapData
{
  public Vector2 Position;
  public Vector2 StartPosition;
}

public struct SeedData
{
  public Vector2 Position;
  public Vector2 StartPosition;
}

public class CollectibleData
{
  public Dictionary<string, CoinData> CoinsData = new Dictionary<string, CoinData>();
  public Dictionary<string, MushroomCapData> MushroomsCapData = new Dictionary<string, MushroomCapData>();
  public Dictionary<string, SeedData> SeedsData = new Dictionary<string, SeedData>();
}

public class CollectiblesManager : Node
{
  private Node2D _collectiblesNode;
  private SaveManager _saveManager;
  private PackedScene _soulScene;
  private PackedScene _coinScene;
  private PackedScene _mushroomCapScene;
  private PackedScene _seedScene;
  private Dictionary<string, Coin> _coins; // used to keep track of godot Node2Ds

  private Dictionary<string, MushroomCap> _mushroomCaps; // used to keep track of godot Node2Ds

  private Dictionary<string, Seed> _seeds; // used to keep track of godot Node2Ds


  public override void _Ready()
  {
    _saveManager = GetNode<SaveManager>($"/root/{nameof(SaveManager)}"); // Singleton

    _coinScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Coin.tscn");
    _mushroomCapScene = ResourceLoader.Load<PackedScene>("res://Prefabs/MushroomCap.tscn");
    _seedScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Seed.tscn");
  }

  internal void Init(Node2D collectiblesNode)
  {
    _collectiblesNode = collectiblesNode;

    _coins = new Dictionary<string, Coin>();
    _mushroomCaps = new Dictionary<string, MushroomCap>();
    _seeds = new Dictionary<string, Seed>();
  }

  public CollectibleData GetCollectiblesData()
  {
    return new CollectibleData()
    {
      CoinsData = _coins.ToDictionary(kvp => kvp.Key, kvp => new CoinData()
      {
        Position = kvp.Value.Position,
        StartPosition = kvp.Value.StartPosition,
      }),
      MushroomsCapData = _mushroomCaps.ToDictionary(kvp => kvp.Key, kvp => new MushroomCapData()
      {
        Position = kvp.Value.Position,
        StartPosition = kvp.Value.StartPosition,
      }),
      SeedsData = _seeds.ToDictionary(kvp => kvp.Key, kvp => new SeedData()
      {
        Position = kvp.Value.Position,
        StartPosition = kvp.Value.StartPosition,
      }),
    };
  }

  internal void InitWithSaveData(CollectibleData collectiblesData, TileMap tileMap, Node2D seedContainerNode)
  {
    Dictionary<string, CoinData> coinsData;
    Dictionary<string, MushroomCapData> mushroomCapsData;
    Dictionary<string, SeedData> seedsData;

    if (collectiblesData == null)
    {
      // New save usecase
      coinsData = seedContainerNode.GetChildrenOfType(typeof(Coin))
        .ToDictionary(kvp => (kvp as Coin).UID, kvp => new CoinData()
        {
          Position = new Vector2((kvp as Coin).Position),
          StartPosition = new Vector2((kvp as Coin).Position), // Start position in the case of new game is the position of theseed on the map
        });
      mushroomCapsData = new Dictionary<string, MushroomCapData>()
      { };
      // get the see position in the node container
      seedsData = seedContainerNode.GetChildrenOfType(typeof(Seed))
        .ToDictionary(kvp => (kvp as Seed).UID, kvp => new SeedData()
        {
          Position = new Vector2((kvp as Seed).Position),
          StartPosition = new Vector2((kvp as Seed).Position), // Start position in the case of new game is the position of theseed on the map
        });

      // Clear the node container
      // TODO: YSort grave hacky pour pas delete les vines
      seedContainerNode.DeleteChildrenExceptTypes(new[] { typeof(FinalZone), typeof(Flower), typeof(Door), typeof(CanvasModulate), typeof(PowerUp), typeof(TileMap), typeof(Mushroom), typeof(Light2D), typeof(Player), typeof(SlowCollectiblePusher), typeof(YSort) });
    }
    else
    {
      // Actual save
      coinsData = collectiblesData.CoinsData;
      mushroomCapsData = collectiblesData.MushroomsCapData;
      seedsData = collectiblesData.SeedsData;
    }

    SyncCollectibles(coinsData, mushroomCapsData, seedsData, tileMap);
  }

  public void SyncCollectibles(Dictionary<string, CoinData> coinsData, Dictionary<string, MushroomCapData> mushroomCapsData, Dictionary<string, SeedData> seedsData, TileMap tileMap)
  {
    foreach (var coinData in coinsData)
    {
      var coinInstance = _coinScene.Instance() as Coin;

      coinInstance.Name = coinData.Key; // needed to sync server and client
      coinInstance.UID = coinData.Key;
      coinInstance.Position = coinData.Value.Position;
      coinInstance.StartPosition = coinData.Value.StartPosition;

      _coins.Add(coinData.Key, coinInstance);
      _collectiblesNode.AddChild(coinInstance);
    }

    foreach (var mushroomCapData in mushroomCapsData)
    {
      var mushroomCapInstance = _mushroomCapScene.Instance() as MushroomCap;

      mushroomCapInstance.Name = mushroomCapData.Key; // needed to sync server and client
      mushroomCapInstance.UID = mushroomCapData.Key;
      mushroomCapInstance.Position = mushroomCapData.Value.Position;
      mushroomCapInstance.StartPosition = mushroomCapData.Value.StartPosition;

      _mushroomCaps.Add(mushroomCapData.Key, mushroomCapInstance);
      _collectiblesNode.AddChild(mushroomCapInstance);
    }

    foreach (var seedData in seedsData)
    {
      var seedInstance = _seedScene.Instance() as Seed;

      seedInstance.Name = seedData.Key; // needed to sync server and client
      seedInstance.UID = seedData.Key;
      seedInstance.Position = seedData.Value.Position;
      seedInstance.TileMap = tileMap;
      seedInstance.StartPosition = seedData.Value.StartPosition;

      _seeds.Add(seedData.Key, seedInstance);
      _collectiblesNode.AddChild(seedInstance);
    }
  }

  // ---

  public void DropCoinOnMapMaster(Vector2 position, Vector2 initialVelocity, Vector2 startPosition)
  {
    var coinUID = Guid.NewGuid().ToString();

    // Add Random offset to position
    var offsetX = RandomGeneratorService.Random.RandiRange(-10, 10);
    var offsetY = RandomGeneratorService.Random.RandiRange(-10, 10);
    var pos = position + new Vector2(offsetX, offsetY);

    DropCoinRemoteSync(coinUID, pos, initialVelocity, startPosition);
  }

  public void DropMushroomCapOnMapMaster(Vector2 position, Vector2 initialVelocity, Vector2 startPosition)
  {
    var mushroomCapUID = Guid.NewGuid().ToString();

    // Add Random offset to position
    var offsetX = RandomGeneratorService.Random.RandfRange(-6, 6);
    var offsetY = RandomGeneratorService.Random.RandfRange(-6, 6);
    var pos = position + new Vector2(offsetX, offsetY);

    DropMushroomCapRemoteSync(mushroomCapUID, pos, initialVelocity, startPosition);
  }

  public void DropSeedOnMapMaster(Vector2 position, Vector2 initialVelocity, TileMap tileMap, Vector2 startPosition)
  {
    var seedUID = Guid.NewGuid().ToString();

    // Add Random offset to position
    var offsetX = RandomGeneratorService.Random.RandiRange(-10, 10);
    var offsetY = RandomGeneratorService.Random.RandiRange(-10, 10);
    var pos = position + new Vector2(offsetX, offsetY);

    DropSeedRemoteSync(seedUID, pos, initialVelocity, tileMap, startPosition);
  }

  // ---

  private void DropCoinRemoteSync(string coinUID, Vector2 position, Vector2 initialVelocity, Vector2 startPosition)
  {
    var coinInstance = _coinScene.Instance() as Coin;

    coinInstance.Name = coinUID; // needed to sync server and client
    coinInstance.UID = coinUID;
    coinInstance.Position = position;
    coinInstance.LinearVelocity = initialVelocity;
    coinInstance.StartPosition = startPosition;

    _coins.Add(coinUID, coinInstance);
    _collectiblesNode.CallDeferred("add_child", coinInstance); // TODO: I used to Replace _itemsNode.AddChild(itemDropInstance); with this not 100% sure why there was errors with this
  }

  private void DropMushroomCapRemoteSync(string mushroomCapUID, Vector2 position, Vector2 initialVelocity, Vector2 startPosition)
  {
    var mushroomCapInstance = _mushroomCapScene.Instance() as MushroomCap;

    mushroomCapInstance.Name = mushroomCapUID; // needed to sync server and client
    mushroomCapInstance.UID = mushroomCapUID;
    mushroomCapInstance.Position = position;
    mushroomCapInstance.LinearVelocity = initialVelocity;
    mushroomCapInstance.StartPosition = startPosition;


    _mushroomCaps.Add(mushroomCapUID, mushroomCapInstance);
    _collectiblesNode.CallDeferred("add_child", mushroomCapInstance); // TODO: I used to Replace _itemsNode.AddChild(itemDropInstance); with this not 100% sure why there was errors with this
  }

  private void DropSeedRemoteSync(string seedUID, Vector2 position, Vector2 initialVelocity, TileMap tileMap, Vector2 startPosition)
  {
    var seedInstance = _seedScene.Instance() as Seed;

    seedInstance.Name = seedUID; // needed to sync server and client
    seedInstance.UID = seedUID;
    seedInstance.Position = position;
    seedInstance.LinearVelocity = initialVelocity;
    seedInstance.TileMap = tileMap;
    seedInstance.StartPosition = startPosition;


    _seeds.Add(seedUID, seedInstance);
    _collectiblesNode.CallDeferred("add_child", seedInstance); // TODO: I used to Replace _itemsNode.AddChild(itemDropInstance); with this not 100% sure why there was errors with this
  }

  // ---

  public Coin GetCoin(string mushroomCapUID)
  {
    if (!_coins.TryGetValue(mushroomCapUID, out var coin))
    {
      return null;
    }
    return coin;
  }

  public MushroomCap GetMushroomCap(string mushroomCapUID)
  {
    if (!_mushroomCaps.TryGetValue(mushroomCapUID, out var mushroomCap))
    {
      return null;
    }
    return mushroomCap;
  }

  public Seed GetSeed(string UID)
  {
    if (!_seeds.TryGetValue(UID, out var seed))
    {
      return null;
    }
    return seed;
  }

  // ---

  public void RemoveCoin(string coinUID)
  {
    if (!_coins.TryGetValue(coinUID, out var coin))
    {
      GD.PrintErr($"Coin {coinUID} was not found in _coins");
      return;
    }

    coin.QueueFree();

    _coins.Remove(coinUID);
  }

  public void RemoveMushroomCap(string mushroomCapUID)
  {
    if (!_mushroomCaps.TryGetValue(mushroomCapUID, out var mushroomCap))
    {
      GD.PrintErr($"MushroomCap {mushroomCapUID} was not found in _mushroomCap");
      return;
    }

    mushroomCap.QueueFree();

    _mushroomCaps.Remove(mushroomCapUID);
  }

  public void RemoveSeed(string UID)
  {
    if (!_seeds.TryGetValue(UID, out var seed))
    {
      GD.PrintErr($"Seed {UID} was not found in _seeds");
      return;
    }

    seed.QueueFree();

    _seeds.Remove(UID);
  }

}
