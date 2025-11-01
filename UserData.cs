using System.Xml;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher;

public class UserData
{
  private bool offlinemode;
  private bool launcherUpdatePrompts = true;
  private bool offlineCheats;
  private bool scanForRunningInstances = true;
  private bool warnedAboutDumpedSaves;
  private bool shownLegalInfo;
  private int width;
  private int height;
  private bool maximized;
  private int splitH;
  private int splitV;
  private string? lang;
  private List<GameInstance> instances;
  private List<ServerInstance> svInstances;

  public UserData()
  {
    this.instances = new List<GameInstance>();
    this.svInstances = new List<ServerInstance>();
    this.DefaultSettings();
  }

  public void DefaultSettings()
  {
    this.offlinemode = true;
    this.launcherUpdatePrompts = true;
    this.warnedAboutDumpedSaves = true;
    this.shownLegalInfo = false;
    this.lang = (string) null;
  }

  public bool IsOfflineMode() => this.offlinemode;

  public bool IsLauncherUpdatePromptsEnabled() => this.launcherUpdatePrompts;

  public bool IsOfflineCheatsEnabled() => this.offlineCheats;

  public bool IsScanForRunningInstances() => this.scanForRunningInstances;

  public bool IsWarnedAboutDumpedSaves() => this.warnedAboutDumpedSaves;

  public bool IsShownLegalInfo() => this.shownLegalInfo;

  public int GetWidth() => this.width;

  public int GetHeight() => this.height;

  public bool IsMaximized() => this.maximized;

  public int GetSplitH() => this.splitH;

  public int GetSplitV() => this.splitV;

  public string? GetLang() => this.lang;

  public List<GameInstance> GetGameInstances() => this.instances;

  public List<ServerInstance> GetServerInstances() => this.svInstances;

  public UserData SetOfflineMode(bool offlinemode)
  {
    this.offlinemode = offlinemode;
    return this;
  }

  public UserData SetLauncherUpdatePromptsEnabled(bool launcherUpdatePrompts)
  {
    this.launcherUpdatePrompts = launcherUpdatePrompts;
    return this;
  }

  public UserData SetOfflineCheatsEnabled(bool offlineCheats)
  {
    this.offlineCheats = offlineCheats;
    return this;
  }

  public UserData SetScanForRunningInstances(bool scanForRunningInstances)
  {
    this.scanForRunningInstances = scanForRunningInstances;
    return this;
  }

  public UserData SetIsWarnedAboutDumpedSaves(bool warnedAboutDumpedSaves)
  {
    this.warnedAboutDumpedSaves = warnedAboutDumpedSaves;
    return this;
  }

  public UserData SetIsShownLegalInfo(bool showedLegalInfo)
  {
    this.shownLegalInfo = showedLegalInfo;
    return this;
  }

  public UserData SetWindowSize(int width, int height, bool maximized)
  {
    this.width = width;
    this.height = height;
    this.maximized = maximized;
    return this;
  }

  public UserData SetSplit(int h, int v)
  {
    this.splitH = h;
    this.splitV = v;
    return this;
  }

  public UserData SetLang(string lang)
  {
    this.lang = lang;
    return this;
  }

  public UserData SetGameInstances(List<GameInstance> instances)
  {
    this.instances = instances;
    return this;
  }

  public UserData SetServerInstances(List<ServerInstance> svInstances)
  {
    this.svInstances = svInstances;
    return this;
  }

  public void SaveTo(string directory)
  {
    XmlDocument xmlDocument = new XmlDocument();
    string[] userdataTopComments = LauncherGlobals.USERDATA_TOP_COMMENTS;
    if (userdataTopComments != null && userdataTopComments.Length != 0)
    {
      foreach (string data in userdataTopComments)
        xmlDocument.AppendChild((XmlNode) xmlDocument.CreateComment(data));
    }
    XmlNode element1 = (XmlNode) xmlDocument.CreateElement("userdata");
    xmlDocument.AppendChild(element1);
    XmlElement element2 = xmlDocument.CreateElement("settings");
    element2.SetAttribute("offlinemode", this.offlinemode ? "1" : "0");
    element2.SetAttribute("updateprompts", this.launcherUpdatePrompts ? "1" : "0");
    element2.SetAttribute("offlinecheats", this.offlineCheats ? "1" : "0");
    element2.SetAttribute("scan_running_instances", this.scanForRunningInstances ? "1" : "0");
    element2.SetAttribute("warned_about_dumped_saves", this.warnedAboutDumpedSaves ? "1" : "0");
    element2.SetAttribute("shown_legal_info", this.shownLegalInfo ? "1" : "0");
    element2.SetAttribute("lang", this.lang ?? Resources.DefaultLanguage);
    element1.AppendChild((XmlNode) element2);
    XmlElement element3 = xmlDocument.CreateElement("window");
    element3.SetAttribute("width", this.width.ToString());
    element3.SetAttribute("height", this.height.ToString());
    element3.SetAttribute("maximized", this.maximized ? "1" : "0");
    element3.SetAttribute("splitH", this.splitH.ToString());
    element3.SetAttribute("splitV", this.splitV.ToString());
    element1.AppendChild((XmlNode) element3);
    XmlNode element4 = (XmlNode) xmlDocument.CreateElement("instances");
    element1.AppendChild(element4);
    if (this.instances != null)
    {
      foreach (GameInstance instance in this.instances)
      {
        XmlElement element5 = xmlDocument.CreateElement("instance");
        element5.SetAttribute("name", instance.GetInstanceName());
        element5.SetAttribute("icon", instance.GetInstanceIconPath());
        element5.SetAttribute(nameof (directory), instance.GetDirectoryPath());
        element5.SetAttribute("patched", instance.IsPatched() ? "1" : "0");
        if (instance.IsForcePatched())
          element5.SetAttribute("forcepatched", instance.IsForcePatched() ? "1" : "0");
        if (instance.GetCustomEXE() != null)
          element5.SetAttribute("runEXE", instance.GetCustomEXE());
        element4.AppendChild((XmlNode) element5);
      }
    }
    XmlNode element6 = (XmlNode) xmlDocument.CreateElement("servers");
    element1.AppendChild(element6);
    if (this.svInstances != null)
    {
      foreach (ServerInstance svInstance in this.svInstances)
      {
        if (svInstance.IsValid())
        {
          XmlElement element7 = xmlDocument.CreateElement("server");
          element7.SetAttribute("address", svInstance.GetAddress());
          element7.SetAttribute("name", svInstance.GetName());
          element7.SetAttribute("desc", svInstance.GetDescription());
          element7.SetAttribute("gameVersion", svInstance.GetGameVersion());
          element4.AppendChild((XmlNode) element7);
        }
      }
    }
    xmlDocument.Save(directory);
  }

  public void LoadFrom(string directory)
  {
    XmlDocument xmlDocument = new XmlDocument();
    if (!File.Exists(directory))
    {
      Console.WriteLine("Can't load userdata from directory: " + directory);
    }
    else
    {
      xmlDocument.Load(directory);
      XmlNode xmlNode1 = xmlDocument.GetElementsByTagName("settings")[0];
      if (xmlNode1 != null && xmlNode1.Attributes != null)
      {
        XmlNode namedItem1 = xmlNode1.Attributes.GetNamedItem("offlinemode");
        if (namedItem1 != null)
        {
          string s = namedItem1.Value;
          if (s != null)
            this.offlinemode = int.Parse(s) > 0;
        }
        XmlNode namedItem2 = xmlNode1.Attributes.GetNamedItem("updateprompts");
        if (namedItem2 != null)
        {
          string s = namedItem2.Value;
          if (s != null)
            this.launcherUpdatePrompts = int.Parse(s) > 0;
        }
        XmlNode namedItem3 = xmlNode1.Attributes.GetNamedItem("offlinecheats");
        if (namedItem3 != null)
        {
          string s = namedItem3.Value;
          if (s != null)
            this.offlineCheats = int.Parse(s) > 0;
        }
        XmlNode namedItem4 = xmlNode1.Attributes.GetNamedItem("scan_running_instances");
        if (namedItem4 != null)
        {
          string s = namedItem4.Value;
          if (s != null)
            this.scanForRunningInstances = int.Parse(s) > 0;
        }
        XmlNode namedItem5 = xmlNode1.Attributes.GetNamedItem("warned_about_dumped_saves");
        if (namedItem5 != null)
        {
          string s = namedItem5.Value;
          if (s != null)
            this.warnedAboutDumpedSaves = int.Parse(s) > 0;
        }
        XmlNode namedItem6 = xmlNode1.Attributes.GetNamedItem("shown_legal_info");
        if (namedItem6 != null)
        {
          string s = namedItem6.Value;
          if (s != null)
            this.shownLegalInfo = int.Parse(s) > 0;
        }
        XmlNode namedItem7 = xmlNode1.Attributes.GetNamedItem("lang");
        if (namedItem7 != null)
        {
          string str = namedItem7.Value;
          if (str != null)
            this.lang = str;
        }
      }
      XmlNode xmlNode2 = xmlDocument.GetElementsByTagName("window")[0];
      if (xmlNode2 != null && xmlNode2.Attributes != null)
      {
        XmlNode namedItem8 = xmlNode2.Attributes.GetNamedItem("width");
        XmlNode namedItem9 = xmlNode2.Attributes.GetNamedItem("height");
        if (namedItem8 != null && namedItem9 != null)
        {
          string s1 = namedItem8.Value?.ToString();
          string s2 = namedItem9.Value?.ToString();
          if (s1 != null && s2 != null)
          {
            this.width = int.Parse(s1);
            this.height = int.Parse(s2);
          }
        }
        XmlNode namedItem10 = xmlNode2.Attributes.GetNamedItem("maximized");
        if (namedItem10 != null)
        {
          string s = namedItem10.Value?.ToString();
          if (s != null)
            this.maximized = int.Parse(s) > 0;
        }
        XmlNode namedItem11 = xmlNode2.Attributes.GetNamedItem("splitH");
        XmlNode namedItem12 = xmlNode2.Attributes.GetNamedItem("splitV");
        if (namedItem11 != null && namedItem12 != null && namedItem11.Value != null && namedItem12.Value != null)
        {
          string s3 = namedItem11.Value.ToString();
          string s4 = namedItem12.Value.ToString();
          if (s3 != null && s4 != null)
          {
            this.splitH = int.Parse(s3);
            this.splitV = int.Parse(s4);
          }
        }
      }
      foreach (XmlNode xmlNode3 in xmlDocument.GetElementsByTagName("instance"))
      {
        if (xmlNode3.Attributes != null)
        {
          try
          {
            GameInstance gameInstance = new GameInstance();
            foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode3.Attributes)
            {
              switch (attribute.Name)
              {
                case "name":
                  gameInstance.SetInstanceName(attribute.Value);
                  continue;
                case "icon":
                  gameInstance.SetInstanceIconPath(attribute.Value);
                  continue;
                case nameof (directory):
                  gameInstance.SetDirectoryPath(attribute.Value);
                  continue;
                case "patched":
                  gameInstance.SetPatched(attribute.Value == "1");
                  continue;
                case "forcepatched":
                  gameInstance.SetForcePatched(attribute.Value == "1");
                  continue;
                case "runEXE":
                  gameInstance.SetCustomEXE(attribute.Value);
                  continue;
                default:
                  continue;
              }
            }
            gameInstance.UpdateInstanceInfo(true);
            this.instances.Add(gameInstance);
          }
          catch (Exception ex)
          {
          }
        }
      }
      foreach (XmlNode xmlNode4 in xmlDocument.GetElementsByTagName("server"))
      {
        if (xmlNode4.Attributes != null)
        {
          ServerInstance serverInstance = new ServerInstance();
          string name = "";
          string description = "";
          string address = "";
          string gameVersion = "";
          foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode4.Attributes)
          {
            switch (attribute.Name)
            {
              case "name":
                name = attribute.Value;
                continue;
              case "desc":
                description = attribute.Value;
                continue;
              case "address":
                address = attribute.Value;
                continue;
              case "gameVersion":
                gameVersion = attribute.Value;
                continue;
              default:
                continue;
            }
          }
          serverInstance.SetServerInfo(address, name, description, gameVersion);
          this.svInstances.Add(serverInstance);
        }
      }
    }
  }
}
