using System.Runtime.InteropServices;
using TCULauncher.Forms;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher;

public class GameInstance
{
  private string? instanceName;
  private string? instanceIcon;
  private string? directoryPath;
  private string? gameVersion;
  private bool patched;
  private bool forcePatched;
  private string? customEXE;
  private Image? icon;
  private string? activeServer;
  private SaveInstance? saveInstance;

  public GameInstance()
  {
  }

  public GameInstance(
    string? instanceName,
    string? instanceIcon,
    string directoryPath,
    string? gameVersion,
    bool patched,
    bool forcePatched,
    string? activeServer)
  {
    this.instanceName = instanceName;
    this.instanceIcon = instanceIcon;
    this.directoryPath = directoryPath;
    this.gameVersion = gameVersion;
    this.patched = patched;
    this.forcePatched = forcePatched;
    this.SetInstanceIconPath(instanceIcon);
    this.SetDirectoryPath(directoryPath);
    this.activeServer = activeServer;
  }

  public string GetInstanceName() => this.instanceName ?? "The Crew Instance";

  public string? GetInstanceIconPath() => this.instanceIcon;

  public Image? GetInstanceIcon() => this.icon;

  public string? GetDirectoryPath() => this.directoryPath;

  public string? GetGameVersion() => this.gameVersion;

  public bool IsPatched() => this.patched;

  public bool IsForcePatched() => this.forcePatched;

  public string? GetCustomEXE() => this.customEXE;

  public string? GetActiveServer() => this.activeServer;

  public SaveInstance? GetSaveInstance() => this.saveInstance;

  public GameInstance SetInstanceName(string instanceName)
  {
    this.instanceName = instanceName;
    return this;
  }

  public GameInstance SetInstanceIconPath(string? instanceIcon)
  {
    this.instanceIcon = instanceIcon;
    this.icon = (Image) Launcher.DefaultInstanceIcon;
        if (instanceIcon != null)
    {
      if (instanceIcon.Length > 0)
      {
        try
        {
          this.icon = Image.FromFile(instanceIcon);
        }
        catch (FileNotFoundException ex)
        {
          this.icon = (Image) Launcher.DefaultInstanceIcon;
          this.instanceIcon = (string) null;
        }
      }
    }
    return this;
  }

  public GameInstance SetPatched(bool patched)
  {
    this.patched = patched;
    return this;
  }

  public GameInstance SetForcePatched(bool forcePatched)
  {
    this.forcePatched = forcePatched;
    return this;
  }

  public GameInstance SetActiveServer(string? activeServer)
  {
    this.activeServer = activeServer;
    return this;
  }

  public GameInstance SetDirectoryPath(string? directoryPath)
  {
    this.saveInstance = (SaveInstance) null;
    this.directoryPath = directoryPath;
    string path = LauncherUtils.VerifyDirectoryToExe(directoryPath) ? Path.GetDirectoryName(directoryPath) : throw new Exception("Invalid directory: " + directoryPath);
    if (this.instanceName == null || this.instanceName.Length == 0)
    {
      this.instanceName = path != null ? Path.GetFileName(path) : (string) null;
      if (this.instanceName == null)
        this.instanceName = "The Crew Instance";
    }
    if ((this.instanceIcon == null || this.instanceIcon.Length == 0 || !File.Exists(this.instanceIcon)) && path != null)
      this.SetInstanceIconPath(path + "\\TheCrew.ico");
    return this;
  }

  public GameInstance SetCustomEXE(string? customEXE)
  {
    this.customEXE = customEXE;
    return this;
  }

  public bool AutoDetectLocalSave(Launcher launcher)
  {
    string directoryName = Path.GetDirectoryName(this.directoryPath);
    string loadSaveFromPath = "";
    if (File.Exists($"{directoryName}\\{SaveManager.FILENAME}{SaveManager.FORMAT_BIN}"))
      loadSaveFromPath = $"{directoryName}\\{SaveManager.FILENAME}{SaveManager.FORMAT_BIN}";
    else if (LauncherGlobals.ALLOW_DATA_JSON && File.Exists($"{directoryName}\\{SaveManager.FILENAME}{SaveManager.FORMAT_JSON}"))
      loadSaveFromPath = $"{directoryName}\\{SaveManager.FILENAME}{SaveManager.FORMAT_JSON}";
    if (loadSaveFromPath.Length > 0)
    {
      KeyValuePair<SaveInstance, bool> keyValuePair1 = SaveManager.LoadServerSave(loadSaveFromPath);
      this.saveInstance = keyValuePair1.Key;
      if (keyValuePair1.Value)
        return false;
      if (this.saveInstance == null)
      {
        string bakPath = LauncherUtils.GetSaveBackup(loadSaveFromPath);
        if (bakPath != null)
        {
          keyValuePair1 = SaveManager.LoadServerSave(bakPath);
          this.saveInstance = keyValuePair1.Key;
          if (this.saveInstance != null)
          {
            Action restoreFromBak = (Action) null;
            restoreFromBak = (Action) (() =>
            {
              KeyValuePair<bool, string> keyValuePair2 = LauncherUtils.RestoreSaveFromBackup(loadSaveFromPath, bakPath, launcher.loc);
              if (keyValuePair2.Value == null)
                return;
              int num1;
              if (keyValuePair2.Key)
              {
                string textHeader = launcher.loc.Get("alert");
                string textDesc = keyValuePair2.Value;
                Bitmap icDboxInfo = Resources.ic_dbox_info;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(launcher.loc.Get("ok"));
                num1 = index + 1;
                int num2 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxInfo, buttonActionList).ShowDialog((IWin32Window) Launcher.FormMain);
                this.saveInstance = SaveManager.LoadServerSave(loadSaveFromPath).Key;
              }
              else
              {
                string textHeader = launcher.loc.Get("warning");
                string textDesc = keyValuePair2.Value;
                Bitmap icRefuse = Resources.ic_refuse_;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index1 = 0;
                span[index1] = new ButtonAction(launcher.loc.Get("try_again"), restoreFromBak);
                int index2 = index1 + 1;
                span[index2] = new ButtonAction(launcher.loc.Get("cancel"));
                num1 = index2 + 1;
                int num3 = (int) new ConfirmForm(textHeader, textDesc, (Image) icRefuse, buttonActionList).ShowDialog((IWin32Window) Launcher.FormMain);
              }
            });
            restoreFromBak();
          }
        }
      }
    }
    if (this.saveInstance != null)
      return true;
    bool zlib = true;
    this.saveInstance = new SaveInstance($"{directoryName}\\{SaveManager.FILENAME}{(zlib ? SaveManager.FORMAT_BIN : SaveManager.FORMAT_JSON)}", zlib, SaveManager.DEFAULT_SAVE_VERSION);
    this.saveInstance.AddNewSave(LauncherGlobals.SAVEGAME_DEFAULT_NAME);
    this.saveInstance.SaveToFile((string) null);
    return false;
  }

  public GameInstance SetSaveInstance(SaveInstance? saveInstance)
  {
    this.saveInstance = saveInstance;
    return this;
  }

  public void UpdateInstanceInfo(bool checkPatch)
  {
    this.gameVersion = LauncherUtils.GetVersion(this.directoryPath);
    if (this.gameVersion == null)
      this.gameVersion = "?.?.?.?";
    if (!checkPatch)
      return;
    this.patched = TCUPatcher.IsDirectoryPatched(this.directoryPath);
  }
}
