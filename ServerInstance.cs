#nullable enable
using TCULauncher.Properties;

namespace TCULauncher;

public class ServerInstance
{
  public static readonly ServerInstance SERVER_LOCAL = new ServerInstance().SetLocalServer().SetServerIcon((Image)Resources.icon_offlineserver).SetServerConfig((ServerConfig) new LocalServerConfig());
  public static readonly string VERSION_ANY = "any";
  private byte isValid = byte.MaxValue;
  private string name = "";
  private string description = "";
  private string gameVersion = "";
  private string address = "";
  private Image? serverIcon;
  private ServerConfig serverConfig;

  public ServerInstance() => this.serverConfig = new ServerConfig();

  public bool IsValid() => this.isValid != byte.MaxValue;

  public bool IsLocal() => this.isValid == (byte) 0;

  public bool IsOnline() => this.isValid == (byte) 1;

  public string GetName() => this.name;

  public string GetDescription() => this.description;

  public string GetGameVersion() => this.gameVersion;

  public bool IsVersionMatching(GameInstance gameInstance)
  {
    return this.gameVersion == ServerInstance.VERSION_ANY || gameInstance.GetGameVersion() == this.gameVersion;
  }

  public string GetAddress() => this.address;

  public Image? GetServerIcon() => this.serverIcon;

  public ServerConfig GetServerConfig() => this.serverConfig;

  public ServerInstance SetServerInfo(
    string address,
    string name,
    string description,
    string gameVersion)
  {
    this.isValid = (byte) 1;
    this.address = address;
    this.name = name;
    this.description = description;
    this.gameVersion = gameVersion;
    return this;
  }

  public ServerInstance SetLocalServer()
  {
    this.isValid = (byte) 0;
    this.address = "127.0.0.1";
    this.name = Localizator.UNLOCALIZED_SYMBOL + "server_local_name";
    this.description = Localizator.UNLOCALIZED_SYMBOL + "server_local_desc";
    this.gameVersion = ServerInstance.VERSION_ANY;
    return this;
  }

  public ServerInstance SetServerIcon(Image? serverIcon)
  {
    this.serverIcon = serverIcon;
    return this;
  }

  public ServerInstance SetServerConfig(ServerConfig serverConfig)
  {
    this.serverConfig = serverConfig;
    return this;
  }
}
