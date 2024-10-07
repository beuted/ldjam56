using Godot;
using System;

public class Main : Node2D
{
  private PlayerManager _playerManager;
  private EffectManager _effectManager;
  private ModalManager _modalManager;
  private MessagesManager _messagesManager;
  private CameraManager _cameraManager;
  private CursorManager _cursorManager;
  private SaveManager _saveManager;
  private PauseMenuManager _pauseMenuManager;
  private CollectiblesManager _collectiblesManager;
  private TimeManager _timeManager;
  private PackedScene _vineScene;
  private GUI _gui;

  public override void _Ready()
  {
    // Autoloads
    _playerManager = (PlayerManager)GetNode("/root/" + nameof(PlayerManager)); // Singleton
    _effectManager = (EffectManager)GetNode($"/root/{nameof(EffectManager)}"); // Singleton
    _modalManager = (ModalManager)GetNode($"/root/{nameof(ModalManager)}"); // Singleton
    _messagesManager = (MessagesManager)GetNode($"/root/{nameof(MessagesManager)}"); // Singleton

    _cameraManager = (CameraManager)GetNode($"/root/{nameof(CameraManager)}"); // Singleton
    _cursorManager = (CursorManager)GetNode($"/root/{nameof(CursorManager)}"); // Singleton
    _saveManager = (SaveManager)GetNode($"/root/{nameof(SaveManager)}"); // Singleton
    _pauseMenuManager = (PauseMenuManager)GetNode($"/root/{nameof(PauseMenuManager)}"); // Singleton
    _collectiblesManager = GetNode<CollectiblesManager>($"/root/{nameof(CollectiblesManager)}"); // Singleton
    _timeManager = (TimeManager)GetNode($"/root/{nameof(TimeManager)}"); // Singleton

    _vineScene = ResourceLoader.Load<PackedScene>("res://Prefabs/Vine.tscn");


    // ALready instantiated in main scene
    _gui = GetNode<GUI>("UserInterface/GUI");
    _timeManager.RestartRun();


    // Inits
    _effectManager.Init(GetNode<Node2D>("Effects"));
    _modalManager.Init(GetNode<Control>("UserInterface/Modals"));
    _messagesManager.Init(GetNode<Control>("UserInterface/Messages"));
    var player = _playerManager.Init(GetNode<Node2D>("Map"));
    _cameraManager.Init(player);
    _pauseMenuManager.Init(GetNode<PauseMenu>("UserInterface/PauseMenu"));
    _collectiblesManager.Init(GetNode<Node2D>("Map"));
    _gui.Init(player);

    // Pour pouvoir voir qqchos en mode edit sur godot
    GetNode<CanvasModulate>("Map/CanvasModulate").Color = new Color("1f2028");

    // Welcome message
    _messagesManager.ShowMessageStart();

    // Load save data and init (SaveData has been loaded in the menu)
    # region save data init

    _playerManager.InitWithSaveData(_saveManager.SaveData, GetNode<TileMap>("Map/TileMap"));
    _collectiblesManager.InitWithSaveData(_saveManager.SaveData, GetNode<TileMap>("Map/TileMap"), GetNode<Node2D>("Map"));

    if (_saveManager.SaveData != null)
    {
      foreach (var vinesPosition in _saveManager.SaveData.VinesPositions)
      {
        GrowIntoVine(GetNode<YSort>("Map/VineContainer"), GetNode<TileMap>("Map/TileMap"), vinesPosition);
      }

      foreach (var vinesPosition in _saveManager.SaveData.VinesPositions)
      {
        GrowIntoVine(GetNode<YSort>("Map/VineContainer"), GetNode<TileMap>("Map/TileMap"), vinesPosition);
      }
    }


    #endregion

    // No Ui is opened, change the cursors accordingly
    _cursorManager.ChangeCursorUiOpened(false);
  }

  private void GrowIntoVine(Node2D parent, TileMap tileMap, Vector2 vinePosition)
  {
    var vineInstance = _vineScene.Instance() as Vine;
    vineInstance.Position = vinePosition;
    vineInstance.TileMap = tileMap;

    parent.AddChild(vineInstance);
  }
}

