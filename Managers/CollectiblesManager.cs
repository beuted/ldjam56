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
  public Node2D CollectiblesNode; // Hack : set public bc I need to acess it in the saveManager
  private SaveManager _saveManager;
  private PackedScene _soulScene;
  private PackedScene _coinScene;
  private PackedScene _mushroomCapScene;
  private PackedScene _seedScene;

  public Dictionary<string, Coin> Coins; // used to keep track of godot Node2Ds

  public Dictionary<string, MushroomCap> MushroomCaps; // used to keep track of godot Node2Ds

  public Dictionary<string, Seed> Seeds; // used to keep track of godot Node2Ds


  public override void _Ready()
  {
    _saveManager = GetNode<SaveManager>($"/root/{nameof(SaveManager)}"); // Singleton

    _coinScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Coin.tscn");
    _mushroomCapScene = ResourceLoader.Load<PackedScene>("res://Prefabs/MushroomCap.tscn");
    _seedScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Seed.tscn");
  }

  internal void Init(Node2D collectiblesNode)
  {
    CollectiblesNode = collectiblesNode;

    Coins = new Dictionary<string, Coin>();
    MushroomCaps = new Dictionary<string, MushroomCap>();
    Seeds = new Dictionary<string, Seed>();
  }

  public CollectibleData GetCollectiblesData()
  {
    return new CollectibleData()
    {
      CoinsData = Coins.ToDictionary(kvp => kvp.Key, kvp => new CoinData()
      {
        Position = kvp.Value.Position,
        StartPosition = kvp.Value.StartPosition,
      }),
      MushroomsCapData = MushroomCaps.ToDictionary(kvp => kvp.Key, kvp => new MushroomCapData()
      {
        Position = kvp.Value.Position,
        StartPosition = kvp.Value.StartPosition,
      }),
      SeedsData = Seeds.ToDictionary(kvp => kvp.Key, kvp => new SeedData()
      {
        Position = kvp.Value.Position,
        StartPosition = kvp.Value.StartPosition,
      }),
    };
  }

  internal void InitWithSaveData(SaveData saveData, TileMap tileMap, Node2D seedContainerNode)
  {
    Dictionary<string, CoinData> coinsData;
    Dictionary<string, MushroomCapData> mushroomCapsData;
    Dictionary<string, SeedData> seedsData;

    if (saveData == null)
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
      coinsData = saveData.CoinsStartPosition.ToDictionary(x => Guid.NewGuid().ToString(), x => new CoinData()
      {
        Position = x,
        StartPosition = x,
      });
      mushroomCapsData = new Dictionary<string, MushroomCapData>() { }; // MushroomCap reset on mushroom
      seedsData = saveData.SeedsStartPosition.ToDictionary(x => Guid.NewGuid().ToString(), x => new SeedData()
      {
        Position = x,
        StartPosition = x,
      });

      // Clear the node container
      // TODO: YSort grave hacky pour pas delete les vines
      seedContainerNode.DeleteChildrenExceptTypes(new[] { typeof(FinalZone), typeof(Flower), typeof(Door), typeof(CanvasModulate), typeof(PowerUp), typeof(TileMap), typeof(Mushroom), typeof(Light2D), typeof(Player), typeof(SlowCollectiblePusher), typeof(YSort) });
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

      Coins.Add(coinData.Key, coinInstance);
      CollectiblesNode.AddChild(coinInstance);
    }

    foreach (var mushroomCapData in mushroomCapsData)
    {
      var mushroomCapInstance = _mushroomCapScene.Instance() as MushroomCap;

      mushroomCapInstance.Name = mushroomCapData.Key; // needed to sync server and client
      mushroomCapInstance.UID = mushroomCapData.Key;
      mushroomCapInstance.Position = mushroomCapData.Value.Position;
      mushroomCapInstance.StartPosition = mushroomCapData.Value.StartPosition;

      MushroomCaps.Add(mushroomCapData.Key, mushroomCapInstance);
      CollectiblesNode.AddChild(mushroomCapInstance);
    }

    foreach (var seedData in seedsData)
    {
      var seedInstance = _seedScene.Instance() as Seed;

      seedInstance.Name = seedData.Key; // needed to sync server and client
      seedInstance.UID = seedData.Key;
      seedInstance.Position = seedData.Value.Position;
      seedInstance.TileMap = tileMap;
      seedInstance.StartPosition = seedData.Value.StartPosition;

      Seeds.Add(seedData.Key, seedInstance);
      CollectiblesNode.AddChild(seedInstance);
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

    Coins.Add(coinUID, coinInstance);
    CollectiblesNode.CallDeferred("add_child", coinInstance); // TODO: I used to Replace _itemsNode.AddChild(itemDropInstance); with this not 100% sure why there was errors with this
  }

  private void DropMushroomCapRemoteSync(string mushroomCapUID, Vector2 position, Vector2 initialVelocity, Vector2 startPosition)
  {
    var mushroomCapInstance = _mushroomCapScene.Instance() as MushroomCap;

    mushroomCapInstance.Name = mushroomCapUID; // needed to sync server and client
    mushroomCapInstance.UID = mushroomCapUID;
    mushroomCapInstance.Position = position;
    mushroomCapInstance.LinearVelocity = initialVelocity;
    mushroomCapInstance.StartPosition = startPosition;


    MushroomCaps.Add(mushroomCapUID, mushroomCapInstance);
    CollectiblesNode.CallDeferred("add_child", mushroomCapInstance); // TODO: I used to Replace _itemsNode.AddChild(itemDropInstance); with this not 100% sure why there was errors with this
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


    Seeds.Add(seedUID, seedInstance);
    CollectiblesNode.CallDeferred("add_child", seedInstance); // TODO: I used to Replace _itemsNode.AddChild(itemDropInstance); with this not 100% sure why there was errors with this
  }

  // ---

  public Coin GetCoin(string mushroomCapUID)
  {
    if (!Coins.TryGetValue(mushroomCapUID, out var coin))
    {
      return null;
    }
    return coin;
  }

  public MushroomCap GetMushroomCap(string mushroomCapUID)
  {
    if (!MushroomCaps.TryGetValue(mushroomCapUID, out var mushroomCap))
    {
      return null;
    }
    return mushroomCap;
  }

  public Seed GetSeed(string UID)
  {
    if (!Seeds.TryGetValue(UID, out var seed))
    {
      return null;
    }
    return seed;
  }

  // ---

  public void RemoveCoin(string coinUID)
  {
    if (!Coins.TryGetValue(coinUID, out var coin))
    {
      GD.PrintErr($"Coin {coinUID} was not found in _coins");
      return;
    }

    coin.QueueFree();

    Coins.Remove(coinUID);
  }

  public void RemoveMushroomCap(string mushroomCapUID)
  {
    if (!MushroomCaps.TryGetValue(mushroomCapUID, out var mushroomCap))
    {
      GD.PrintErr($"MushroomCap {mushroomCapUID} was not found in _mushroomCap");
      return;
    }

    mushroomCap.QueueFree();

    MushroomCaps.Remove(mushroomCapUID);
  }

  public void RemoveSeed(string UID)
  {
    if (!Seeds.TryGetValue(UID, out var seed))
    {
      GD.PrintErr($"Seed {UID} was not found in _seeds");
      return;
    }

    seed.QueueFree();

    Seeds.Remove(UID);
  }

}
