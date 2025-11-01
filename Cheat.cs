namespace TCULauncher;

public class Cheat
{
  public readonly string Name;
  public readonly string? Type;
  public readonly string? DefaultValue;
  public readonly string? Description;

  public Cheat(string Name, string? Type, string? DefaultValue, string? Description)
  {
    this.Name = Name;
    this.Type = Type;
    this.DefaultValue = DefaultValue;
    this.Description = Description;
  }
}
