#nullable enable
namespace TCULauncher;

public class FontEntry
{
  public readonly string FontName;
  public readonly byte[] Data;

  public FontEntry(string FontName, byte[] Data)
  {
    this.FontName = FontName;
    this.Data = Data;
  }
}
