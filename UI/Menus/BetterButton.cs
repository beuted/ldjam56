using Godot;
using System;
using System.Data.SqlClient;

public class BetterButton : Button
{
    private InputManager _inputManager;

    private bool _betterFocus = false;

    public override void _Ready()
    {
        _inputManager = (InputManager)GetNode($"/root/{nameof(InputManager)}"); // Singleton

        _inputManager.Connect("controller_in_use_changed", this, nameof(ControllerInUseChanged));
    }


    public bool BetterFocus
    {
        get
        {
            return _betterFocus;
        }
        set
        {
            ChangeStyle(value && _inputManager.ControllerInUse);
            _betterFocus = value;
        }
    }

    private void ControllerInUseChanged(bool controllerInUse)
    {
        ChangeStyle(_betterFocus && controllerInUse);
    }

    private void ChangeStyle(bool isFocus)
    {
        Modulate = isFocus ? new Color("#c6c6c6") : new Color("#ffffff");
    }
}
