using Godot;
using System;

public class InputHelp : Node2D
{
    [Export]
    public string Letter = "E";

    [Export]
    public string ControllerInput = "RB"; //TODO: deal with controller support

    private int _price = -1;

    private InputManager _inputManager;

    private Label _keyLabel;

    public override void _Ready()
    {
        _inputManager = (InputManager)GetNode($"/root/{nameof(InputManager)}"); // Singleton

        _keyLabel = GetNode<Label>("Sprite/Label");

        _inputManager.Connect("controller_in_use_changed", this, nameof(RefreshLabelVisual));

        RefreshLabelVisual(_inputManager.ControllerInUse);
    }

    public void RefreshLabelVisual(bool controllerInUse)
    {
        if (controllerInUse)
            _keyLabel.Text = ControllerInput;
        else
            _keyLabel.Text = Letter;
    }
}
