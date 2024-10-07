using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class SaveManager : Node
{
  private static SaveData DefaultSaveData = new SaveData()
  {
    PlayerPosition = new Vector2(0, 0),
  };

  private static ConfigData DefaultConfigData = new ConfigData()
  {
    FullScreen = true,
    MusicVolume = 0.5f,
    FxVolume = 0.5f,
    UiScaleFactor = 2f
  };

  private PlayerManager _playerManager;
  private SoundManager _soundManager;
  private CollectiblesManager _collectiblesManager;
  private static string SaveFileName = "user://godot-ladybug-s-underworld-2-dekajoo.save"; // Saved @ C:\Users\dekajoo\AppData\Roaming\Godot\app_userdata\Godot Template
  private static string ConfigFileName = "user://godot-ladybug-s-underworld-2-config.save"; // Saved @ C:\Users\dekajoo\AppData\Roaming\Godot\app_userdata\Godot Template

  private string UserId; // TODO: This Id should be saved once and never change as long as the game is installed

  public SaveData SaveData { get; private set; }
  public ConfigData ConfigData { get; private set; }


  public override void _Ready()
  {
    _playerManager = (PlayerManager)GetNode($"/root/{nameof(PlayerManager)}"); // Singleton
    _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton
    _collectiblesManager = (CollectiblesManager)GetNode($"/root/{nameof(CollectiblesManager)}"); // Singleton


    // Init from save (this should be done once per program launch)
    var configData = LoadConfig();

    OS.WindowFullscreen = configData.FullScreen;
    UpdateSoundManagerAfterTimeout(configData);
  }

  public async Task UpdateSoundManagerAfterTimeout(ConfigData configData)
  {
    // Hacky stuff if we update the sound manager too soon the values of the Buses volumes are not correctly updated
    await ToSignal(GetTree().CreateTimer(1, false), "timeout");
    _soundManager.ChangeFxVolumePercent(configData.FxVolume);
    _soundManager.ChangeMusicVolumePercent(configData.MusicVolume);
  }

  public bool HasSaveGameFile()
  {
    var saveGame = new File();
    return saveGame.FileExists(SaveFileName);
  }

  public void SaveConfig(ConfigData configData)
  {
    var saveGame = new File();
    saveGame.Open(ConfigFileName, File.ModeFlags.Write);

    ConfigData = configData;
    // Store the config dictionary as a new line in the save file.
    saveGame.StoreLine(Newtonsoft.Json.JsonConvert.SerializeObject(configData));

    saveGame.Close();
  }

  public ConfigData LoadConfig()
  {
    var saveGame = new File();
    if (!saveGame.FileExists(ConfigFileName))
    {
      ConfigData = DefaultConfigData;
      return DefaultConfigData; // Error! We don't have a save to load.
    }

    // Load the file line by line and process that dictionary to restore the object it represents.
    saveGame.Open(ConfigFileName, File.ModeFlags.Read);

    var line = saveGame.GetLine();
    ConfigData = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigData>(line);

    return ConfigData;
  }

  public void SaveGame()
  {
    var saveGame = new File();
    saveGame.Open(SaveFileName, File.ModeFlags.Write);

    // Store the save dictionary as a new line in the save file.
    var positionPayer = _playerManager.CurrentPlayer.Position;
    var seedsStartPosition = _collectiblesManager.Seeds.Select(x => x.Value.StartPosition).ToList();
    var coinsStartPosition = _collectiblesManager.Coins.Select(x => x.Value.StartPosition).ToList();
    var vinesPosition = _collectiblesManager.CollectiblesNode.GetChildrenOfType(typeof(Vine)).Select(x => (x as Vine).Position).ToList();
    var flowersPosition = _collectiblesManager.CollectiblesNode.GetChildrenOfType(typeof(Flower)).Select(x => (x as Flower).Position).ToList();
    var doorStates = _collectiblesManager.CollectiblesNode.GetChildrenOfType(typeof(Door)).Select(x => new DoorState()
    {
      Position = (x as Door).Position,
      NbCoins = (x as Door).NbCoins,
    }).ToList();

    var saveData = new SaveData()
    {
      PlayerPosition = positionPayer,
      SeedsStartPosition = seedsStartPosition,
      CoinsStartPosition = coinsStartPosition,
      VinesPositions = vinesPosition,
      FlowersPositions = flowersPosition,
      DoorStates = doorStates
    };

    saveGame.StoreLine(Newtonsoft.Json.JsonConvert.SerializeObject(saveData));

    saveGame.Close();
  }

  public bool HasSaveData()
  {
    var saveGame = new File();
    return saveGame.FileExists(SaveFileName);
  }

  public SaveData ReadSaveData()
  {
    var saveGame = new File();
    if (!saveGame.FileExists(SaveFileName))
      return DefaultSaveData; // Error! We don't have a save to load.

    // Load the file line by line and process that dictionary to restore the object it represents.
    saveGame.Open(SaveFileName, File.ModeFlags.Read);

    // Get the saved dictionary from the next line in the save file
    var saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(saveGame.GetLine());

    saveGame.Close();


    SaveData = saveData;

    return saveData;
  }

  public void ClearSaveData()
  {
    SaveData = null;
  }
}



public class SaveData
{
  public Vector2 PlayerPosition;

  // planted seeds
  public List<Vector2> SeedsStartPosition; // We remove the seed that have been planted from this list

  // planted vines + if coin taken and planted in door
  public List<Vector2> VinesPositions;
  public List<Vector2> FlowersPositions;

  // planted coins
  public List<Vector2> CoinsStartPosition; // We remove the seed that have been planted from this list

  // for each door number of coin
  public List<DoorState> DoorStates;
}

public class DoorState
{
  public Vector2 Position;
  public int NbCoins;
}

public class ConfigData
{
  public bool FullScreen;
  public float MusicVolume;
  public float FxVolume;
  public float UiScaleFactor;
}
