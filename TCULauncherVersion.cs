#nullable enable
namespace TCULauncher;

public class TCULauncherVersion
{
  private readonly string version;
  private readonly string file;
  private readonly bool isEncrypted;

  public TCULauncherVersion(string version, string file, bool isEncrypted)
  {
    this.version = version;
    this.file = file;
    this.isEncrypted = isEncrypted;
  }

  public string GetVersion() => this.version;

  public string GetFile() => this.file;

  public bool IsEncrypted() => this.isEncrypted;
}
