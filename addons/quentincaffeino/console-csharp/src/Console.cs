using Godot;

public class Console : Node
{
  CanvasLayer _console;

  public override void _Ready()
  {
    _console = GetTree().Root.GetNode<CanvasLayer>("Console");
  }

  public bool IsConsoleShown()
  {
    return (bool)_console.Get("is_console_shown");
  }

  public void HideConsole()
  {
    if ((bool)_console.Get("is_console_shown"))
      _console.Call("toggle_console");
  }

  public CommandBuilder AddCommand(string name, Godot.Object target, string targetMethodName)
  {
    Godot.Object consoleCommand = _console.Call("add_command", name, target, targetMethodName) as Godot.Object;
    return new CommandBuilder(consoleCommand);
  }

  public void WriteLine(string message)
  {
    _console.Call("write_line", message);
  }

  public void Write(string message)
  {
    _console.Call("write", message);
  }
}