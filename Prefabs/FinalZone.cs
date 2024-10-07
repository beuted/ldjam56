using Godot;
using System;

public class FinalZone : Area2D
{
    [Export]
    public bool isFinalFinal = false;

    [Export]
    public NodePath RichLabelTextNodePath;

    private MessagesManager _messagesManager;
    private TimeManager _timeManager;
    private RichTextLabel _richLabelText;

    private float? _timeSinceStart;


    public override void _Ready()
    {
        _messagesManager = (MessagesManager)GetNode($"/root/{nameof(MessagesManager)}"); // Singleton
        _timeManager = (TimeManager)GetNode($"/root/{nameof(TimeManager)}"); // Singleton

        _richLabelText = GetNode<RichTextLabel>(RichLabelTextNodePath);
    }

    public void OnBodyEntered(Node body)
    {
        if (body is Player player)
        {
            if (isFinalFinal)
            {
                _messagesManager.ShowMessageFinalFinal();

                if (!_timeSinceStart.HasValue)
                {
                    // Set the end of game counter
                    _timeSinceStart = _timeManager.RunTimeSinceStart;
                    var timeSpan = TimeSpan.FromMilliseconds((ulong)(_timeSinceStart * 1000));

                    _richLabelText.BbcodeText = $"[center]Game completed in\n{timeSpan.ToString()}![/center]";
                }
            }
            else
            {
                _messagesManager.ShowMessageFinal();
            }
        }
    }

}
