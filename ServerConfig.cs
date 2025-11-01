using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

#nullable enable
namespace TCULauncher;

public class ServerConfig
{
  private string IP;
  private string Username;
  private string? Password;
  public bool isLoaded;

  public ServerConfig()
  {
    this.IP = "";
    this.Username = "EnterUsername";
    this.Password = (string) null;
  }

  public ServerConfig(string IP, string Username, string? Password)
  {
    this.IP = IP;
    this.Username = Username;
    this.Password = Password;
  }

  public virtual bool IsLocalServer() => false;

  public string GetIP() => this.IP;

  public string GetUsername() => this.Username;

  public bool HasPassword() => this.Password != null && this.Password.Length > 0;

  public string? GetPassword() => this.Password;

  public ServerConfig SetIP(string IP)
  {
    this.IP = IP;
    return this;
  }

  public ServerConfig SetUsername(string Username, bool ResetPassword)
  {
    this.Username = Username;
    if (ResetPassword)
      this.Password = (string) null;
    return this;
  }

  public ServerConfig SetPassword(string PasswordString)
  {
    this.Password = PasswordString;
    return this;
  }

  private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
  {
    byte[] buffer = new byte[plainText.Length + salt.Length];
    for (int index = 0; index < plainText.Length; ++index)
      buffer[index] = plainText[index];
    for (int index = 0; index < salt.Length; ++index)
      buffer[plainText.Length + index] = salt[index];
    return SHA256.Create().ComputeHash(buffer);
  }

  protected virtual void writeIntoJson(JsonObject root)
  {
    root.Add("Username", (JsonNode) this.Username);
    if (this.Password == null)
      return;
    root.Add("Password", (JsonNode) Encoding.ASCII.GetString(ServerConfig.GenerateSaltedHash(Encoding.ASCII.GetBytes(this.Password), Encoding.ASCII.GetBytes(LauncherUtils.GetMachineGuid()))));
  }

  protected virtual void readPropertiesFromJson(JsonObject root)
  {
    string str = root["Username"]?.ToString();
    if (str != null)
      this.Username = str;
    root["Password"]?.ToString();
    if (str == null)
      return;
    this.Username = str;
  }

  public void SetConfigFile(GameInstance gameInstance, ServerInstance serverInstance)
  {
  }

  protected virtual string writeTopComment()
  {
    return LauncherGlobals.SERVER_INI_TOP_COMMENT + Environment.NewLine + Environment.NewLine;
  }

  protected virtual string writeIniSectionServer(UserData userdata)
  {
    string str = "";
    if (!LauncherGlobals.LOCAL_IP.Equals(this.IP))
      str = $"{$"{str}[Server]{Environment.NewLine}"}IP = {this.IP}{Environment.NewLine}";
    return str;
  }

  protected virtual string writeIniSectionClient(UserData userdata)
  {
    string str = $"{"[Client]" + Environment.NewLine}UserName = {this.Username}{Environment.NewLine}";
    if (this.Password != null && this.Password.Length > 0)
      str = $"{str}Password = {this.Password}{Environment.NewLine}";
    return str;
  }

  protected virtual string writeIniSections(UserData userdata)
  {
    return "" + this.writeIniSectionServer(userdata) + this.writeIniSectionClient(userdata);
  }

  public void WriteIniFile(UserData userdata, string outputFile)
  {
    if (!this.isLoaded)
      return;
    File.WriteAllText(outputFile, this.writeTopComment() + this.writeIniSections(userdata));
  }

  public virtual void ReadFromIni(UserData userdata, IniFile serverIni)
  {
    Dictionary<string, string> section1 = serverIni.GetSection("Client");
    if (section1 != null)
    {
      if (section1.ContainsKey("UserName"))
        this.SetUsername(section1["UserName"], true);
      if (section1.ContainsKey("Password"))
        this.SetPassword(section1["Password"]);
    }
    Dictionary<string, string> section2 = serverIni.GetSection("Server");
    if (section2 != null && section2.ContainsKey("IP"))
      this.SetIP(section2["IP"]);
    this.isLoaded = true;
  }
}
