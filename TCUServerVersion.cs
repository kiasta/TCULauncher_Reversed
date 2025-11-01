#nullable enable
namespace TCULauncher;

public class TCUServerVersion
{
  private readonly string version;
  private readonly string file;
  private readonly bool isEncrypted;
  private readonly string? hash;
  private readonly string[] supportedGameVersions;

  public TCUServerVersion(
    string version,
    string file,
    bool isEncrypted,
    string? hash,
    string[] supportedGameVersions)
  {
    this.version = version;
    this.file = file;
    this.isEncrypted = isEncrypted;
    this.hash = hash;
    this.supportedGameVersions = supportedGameVersions;
  }

  public string GetVersion() => this.version;

  public string GetFile() => this.file;

  public bool IsEncrypted() => this.isEncrypted;

  public string? GetHash() => this.hash;

  public string[] GetSupportedGameVersions() => this.supportedGameVersions;

  public bool IsGameVersionSupported(string? gameVersion)
  {
    if (gameVersion == null || this.version == null)
      return false;
    foreach (string supportedGameVersion in this.GetSupportedGameVersions())
    {
      if (supportedGameVersion.Equals(gameVersion))
        return true;
    }
    return false;
  }
}
