#nullable enable
namespace TCULauncher;

public class LocalServerConfig : ServerConfig
{
  private bool authEnabled;
  private bool factionSim;
  private bool refreshFactions;
  private Dictionary<string, string> CheatMap = new Dictionary<string, string>();

  public LocalServerConfig()
    : base(LauncherGlobals.LOCAL_IP, LauncherGlobals.SAVEGAME_DEFAULT_NAME, (string) null)
  {
  }

  public LocalServerConfig(string Username, string? Password)
    : base(LauncherGlobals.LOCAL_IP, Username, Password)
  {
  }

  public override bool IsLocalServer() => true;

  public bool IsAuthEnabled() => this.authEnabled;

  public bool IsFactionSim() => this.factionSim;

  public bool IsRefreshFactions() => this.refreshFactions;

  public string? GetCheatValue(string Cheat)
  {
    return this.CheatMap.ContainsKey(Cheat) ? this.CheatMap[Cheat] : (string) null;
  }

  public LocalServerConfig SetAuthEnabled(bool authEnabled)
  {
    this.authEnabled = authEnabled;
    return this;
  }

  public LocalServerConfig SetFactionSim(bool factionSim)
  {
    this.factionSim = factionSim;
    return this;
  }

  public LocalServerConfig SetRefreshFactions(bool refreshFactions)
  {
    this.refreshFactions = refreshFactions;
    return this;
  }

  public void SetCheatValue(string Cheat, string? Value)
  {
    if (Value != null && Value.Length > 0)
      this.CheatMap[Cheat] = Value;
    else
      this.CheatMap.Remove(Cheat);
  }

  public void ResetCheats() => this.CheatMap.Clear();

  protected override string writeIniSections(UserData userdata)
  {
    return "" + this.writeIniSectionServer(userdata) + this.writeIniSectionClient(userdata) + this.writeIniSectionCheats(userdata);
  }

  protected string writeIniSectionCheats(UserData userdata)
  {
    if (!userdata.IsOfflineCheatsEnabled() || this.CheatMap.Count <= 0)
      return "";
    string str = "[Cheats]" + Environment.NewLine;
    foreach (KeyValuePair<string, string> cheat in this.CheatMap)
    {
      if (cheat.Value != null)
        str = $"{str}{cheat.Key} = {cheat.Value}{Environment.NewLine}";
    }
    return str;
  }

  public override void ReadFromIni(UserData userdata, IniFile serverIni)
  {
    base.ReadFromIni(userdata, serverIni);
    this.CheatMap.Clear();
    if (!userdata.IsOfflineCheatsEnabled())
      return;
    Dictionary<string, string> section = serverIni.GetSection("Cheats");
    if (section == null)
      return;
    foreach (KeyValuePair<string, string> keyValuePair in section)
      this.CheatMap.Add(keyValuePair.Key, keyValuePair.Value);
  }
}
