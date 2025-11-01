#nullable enable
namespace TCULauncher;

public class ButtonAction
{
  private readonly string buttonText;
  private readonly Action? action;

  public ButtonAction(string buttonText, Action? action)
  {
    this.buttonText = buttonText;
    this.action = action;
  }

  public ButtonAction(string buttonText)
    : this(buttonText, (Action) null)
  {
  }

  public string GetText() => this.buttonText;

  public Action? GetAction() => this.action;
}
