using System.Threading.Tasks;
using Godot;

public class TimeManager : Node
{
  public float GameTimeSinceStart; // Time since the game started taking into account "pause"
  public float RunTimeSinceStart; // Time since the last run started taking into account "pause"

  public ulong GameTimeSinceStartUlong
  {
    get
    {
      return (ulong)(GameTimeSinceStart * 1000);
    }
  }

  public int GameTimeSinceStartInt // Can be an issue if the game run for 600 hours ...
  {
    get
    {
      return (int)(GameTimeSinceStart * 1000);
    }
  }

  public override void _Ready()
  {
    GameTimeSinceStart = 0f;
    RunTimeSinceStart = 0f;
  }

  internal void Init()
  {
  }

  public void RestartRun()
  {
    RunTimeSinceStart = 0f;
  }

  public override void _Process(float delta)
  {
    GameTimeSinceStart += delta;
    RunTimeSinceStart += delta;
  }

  public async Task FrameFreeze(float temporaryTimeScale, float durationSec)
  {
    Engine.TimeScale = temporaryTimeScale;
    await ToSignal(GetTree().CreateTimer(durationSec * temporaryTimeScale, false), "timeout");
    Engine.TimeScale = 1.0f;
  }
}
