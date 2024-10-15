using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SaveManager : Node
{
  private static SaveData DefaultSaveData = new SaveData()
  {
    PlayerName = "default player name",
    PlayerPosition = new Vector2(0, 0),
    CollectibleData = new CollectibleData(),
    CollectibleCountData = 0,
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
  private CollectiblesCountManager _collectiblesCountManager;
  private static string SaveFileName = "user://godot-ladybug-s-underworld-dekajoo.save"; // Saved @ C:\Users\dekajoo\AppData\Roaming\Godot\app_userdata\Godot Template
  private static string ConfigFileName = "user://godot-ladybug-s-underworld-config.save"; // Saved @ C:\Users\dekajoo\AppData\Roaming\Godot\app_userdata\Godot Template

  private string UserId; // TODO: This Id should be saved once and never change as long as the game is installed

  public SaveData SaveData { get; private set; }
  public ConfigData ConfigData { get; private set; }


  public override void _Ready()
  {
    _playerManager = (PlayerManager)GetNode($"/root/{nameof(PlayerManager)}"); // Singleton
    _soundManager = (SoundManager)GetNode($"/root/{nameof(SoundManager)}"); // Singleton
    _collectiblesManager = (CollectiblesManager)GetNode($"/root/{nameof(CollectiblesManager)}"); // Singleton
    _collectiblesCountManager = (CollectiblesCountManager)GetNode($"/root/{nameof(CollectiblesCountManager)}"); // Singleton


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
    saveGame.StoreLine(Newtonsoft.Json.JsonConvert.SerializeObject(positionPayer));

    var playerName = _playerManager.CurrentPlayer.PlayerName;
    saveGame.StoreLine(playerName);

    // The collectibles on the map with their positions etc...
    var collectibleData = _collectiblesManager.GetCollectiblesData();
    saveGame.StoreLine(Newtonsoft.Json.JsonConvert.SerializeObject(collectibleData));
    GD.Print(Newtonsoft.Json.JsonConvert.SerializeObject(collectibleData));

    // The counter of collectibles in the GUI
    var collectibleCountData = _collectiblesCountManager.CoinCount;
    saveGame.StoreLine(Newtonsoft.Json.JsonConvert.SerializeObject(collectibleCountData));

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
    var positionPayer = Newtonsoft.Json.JsonConvert.DeserializeObject<Vector2>(saveGame.GetLine());
    var playerName = saveGame.GetLine();

    // Get the saved dictionary from the next line in the save file
    var collectibleData = Newtonsoft.Json.JsonConvert.DeserializeObject<CollectibleData>(saveGame.GetLine());

    // Get the saved dictionary from the next line in the save file
    var collectibleCountData = Newtonsoft.Json.JsonConvert.DeserializeObject<int>(saveGame.GetLine());

    saveGame.Close();

    var saveData = new SaveData()
    {
      PlayerName = playerName,
      PlayerPosition = positionPayer,
      CollectibleData = collectibleData,
      CollectibleCountData = collectibleCountData
    };

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
  public string PlayerName;
  public Vector2 PlayerPosition;
  public CollectibleData CollectibleData;
  public int CollectibleCountData;
}

public class ConfigData
{
  public bool FullScreen;
  public float MusicVolume;
  public float FxVolume;
  public float UiScaleFactor;
}
