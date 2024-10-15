using Godot;
using System;

public class MessagesManager : Node
{
    private InputManager _inputManager;

    private Control _containerNode;

    private Control _containerPickup;
    private Control _containerBalling;
    private Control _containerFinal;
    private Control _containerFinalFinal;
    private Control _containerStart;

    private RichTextLabel _pickupLabel;
    private RichTextLabel _ballingLabel;

    private bool _showMessagePickup;
    private bool _showMessageBalling;
    private bool _showMessageFinalFinal;
    private bool _showMessageFinal;
    private bool _showMessageStart;

    public float SpeedOfReveal = 0.2f;
    private bool _hasShowMessageFinal;
    private bool _hasShowMessageFinalFinal;

    public override void _Ready()
    {

    }

    internal void Init(Control containerNode)
    {
        _inputManager = (InputManager)GetNode($"/root/{nameof(InputManager)}"); // Singleton

        _containerNode = containerNode;

        _containerPickup = _containerNode.GetNode<Control>("CenterContainerPickup");
        _containerBalling = _containerNode.GetNode<Control>("CenterContainerBalling");
        _containerFinal = _containerNode.GetNode<Control>("CenterContainerFinal");
        _containerFinalFinal = _containerNode.GetNode<Control>("CenterContainerFinalFinal");
        _containerStart = _containerNode.GetNode<Control>("CenterContainerStart");

        _pickupLabel = _containerPickup.GetNode<RichTextLabel>("RichTextLabel");
        _ballingLabel = _containerBalling.GetNode<RichTextLabel>("RichTextLabel");

        ResetTransparenciesToZero();

        _containerBalling.Visible = true;
        _containerPickup.Visible = true;
        _containerFinal.Visible = true;
        _containerFinalFinal.Visible = true;
    }

    public override void _Process(float delta)
    {
        if (_containerBalling == null || _containerPickup == null)
            return;

        // Balling powerup
        if (_showMessageBalling)
        {
            var newTransparency = _containerBalling.Modulate.a + delta * SpeedOfReveal;
            if (newTransparency >= 1f)
            {
                newTransparency = 1f;
                _showMessageBalling = false;
            }

            _containerBalling.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }
        else
        {
            var newTransparency = _containerBalling.Modulate.a - delta * SpeedOfReveal;
            if (newTransparency <= 0f)
            {
                newTransparency = 0f;
            }
            _containerBalling.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }

        // Pickup powerup
        if (_showMessagePickup)
        {
            var newTransparency = _containerPickup.Modulate.a + delta * SpeedOfReveal;

            if (newTransparency >= 1f)
            {
                newTransparency = 1f;
                _showMessagePickup = false;
            }
            _containerPickup.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }
        else
        {
            var newTransparency = _containerPickup.Modulate.a - delta * SpeedOfReveal;

            if (newTransparency <= 0f)
            {
                newTransparency = 0f;
            }
            _containerPickup.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }

        // Final message
        if (_showMessageFinal)
        {
            var newTransparency = _containerFinal.Modulate.a + delta * SpeedOfReveal;

            if (newTransparency >= 1f)
            {
                newTransparency = 1f;
                _showMessageFinal = false;
            }
            _containerFinal.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }
        else
        {
            var newTransparency = _containerFinal.Modulate.a - delta * SpeedOfReveal;

            if (newTransparency <= 0f)
            {
                newTransparency = 0f;
            }
            _containerFinal.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }

        // Final final message
        if (_showMessageFinalFinal)
        {
            var newTransparency = _containerFinalFinal.Modulate.a + delta * SpeedOfReveal;

            if (newTransparency >= 1f)
            {
                newTransparency = 1f;
                _showMessageFinalFinal = false;
            }
            _containerFinalFinal.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }
        else
        {
            var newTransparency = _containerFinalFinal.Modulate.a - delta * SpeedOfReveal;

            if (newTransparency <= 0f)
            {
                newTransparency = 0f;
            }
            _containerFinalFinal.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }

        // Start message
        if (_showMessageStart)
        {
            var newTransparency = _containerStart.Modulate.a + delta * SpeedOfReveal;

            if (newTransparency >= 1f)
            {
                newTransparency = 1f;
                _showMessageStart = false;
            }
            _containerStart.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }
        else
        {
            var newTransparency = _containerStart.Modulate.a - delta * SpeedOfReveal;

            if (newTransparency <= 0f)
            {
                newTransparency = 0f;
            }
            _containerStart.Modulate = new Color(1f, 1f, 1f, newTransparency);
        }
    }

    public void ShowMessagePickup()
    {
        var bbCodePickupMessage = $"[center]Atlas Grasp[/center]\n\n[center][wave amp=10 freq=2]You can now lift items by pressing $0[/wave][/center]";
        var pickUpMessageInput = "[color=#f0be1d]\"e\"[/color] or [color=#f0be1d]Left Click[/color]";
        if (_inputManager.ControllerInUse)
        {
            pickUpMessageInput = "[color=#f0be1d]\"RB\"[/color] or [color=#f0be1d]\"RT\"[/color]";
        }
        _pickupLabel.BbcodeText = bbCodePickupMessage.Replace("$0", pickUpMessageInput);

        _showMessagePickup = true;
        _showMessageBalling = false;
        _showMessageFinal = false;
        _showMessageFinalFinal = false;
        _showMessageStart = false;

        // Reset modulates
        ResetTransparenciesToZero();
    }

    public void ShowMessageBalling()
    {
        var bbCodeBallingMessage = $"[center]Curved Shell[/center]\n\n[center][wave amp=10 freq=2]You can now roll into a ball by pressing $0[/wave][/center]";
        var ballingMessageInput = "[color=#f0be1d]\"Shift\"[/color]";
        if (_inputManager.ControllerInUse)
        {
            ballingMessageInput = "[color=#f0be1d]\"LB\"[/color] or [color=#f0be1d]\"LT\"[/color]";
        }
        _ballingLabel.BbcodeText = bbCodeBallingMessage.Replace("$0", ballingMessageInput);

        _showMessageBalling = true;
        _showMessagePickup = false;
        _showMessageFinal = false;
        _showMessageFinalFinal = false;
        _showMessageStart = false;

        // Reset modulates
        ResetTransparenciesToZero();
    }

    public void ShowMessageFinal()
    {
        if (_hasShowMessageFinal)
            return;
        _hasShowMessageFinal = true;

        _showMessageFinal = true;
        _showMessagePickup = false;
        _showMessageBalling = false;
        _showMessageFinalFinal = false;
        _showMessageStart = false;

        // Reset modulates
        ResetTransparenciesToZero();
    }

    public void ShowMessageFinalFinal()
    {
        if (_hasShowMessageFinalFinal)
            return;
        _hasShowMessageFinalFinal = true;

        _showMessageFinalFinal = true;
        _showMessageFinal = false;
        _showMessagePickup = false;
        _showMessageBalling = false;
        _showMessageStart = false;

        // Reset modulates
        ResetTransparenciesToZero();
    }

    public void ShowMessageStart()
    {
        _showMessageStart = true;
        _showMessageFinalFinal = false;
        _showMessageFinal = false;
        _showMessagePickup = false;
        _showMessageBalling = false;

        // Reset modulates
        ResetTransparenciesToZero();
    }

    public void ResetTransparenciesToZero()
    {
        _containerBalling.Modulate = new Color(1f, 1f, 1f, 0f);
        _containerPickup.Modulate = new Color(1f, 1f, 1f, 0f);
        _containerFinal.Modulate = new Color(1f, 1f, 1f, 0f);
        _containerFinalFinal.Modulate = new Color(1f, 1f, 1f, 0f);
        _containerStart.Modulate = new Color(1f, 1f, 1f, 0f);
    }


}
