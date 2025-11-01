using SevenZip;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using TCULauncher.Forms;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher;

public class Launcher
{
    public static readonly string PROGRAM_TITLE = Assembly.GetEntryAssembly()?.GetName().Name ?? "ERROR";
    public static readonly string VERSION = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "ERROR";
    public TCULauncherManifest? LauncherManifest;
    public TCUServerManifest? ServerManifest;
    public TCUNet TcuNet = new TCUNet();
    public bool OfflineMode;
    private static StartupInfoForm? StartupForm;
    public static LauncherMainForm? FormMain;
    public static Icon DefaultIcon = Resources.IconTCU;
    public static Bitmap DefaultInstanceIcon = Resources.TheCrew;
    public static string UserdataDir = $"{Directory.GetCurrentDirectory()}\\{LauncherGlobals.FILE_USERDATA}";
    public static string DownloadsDir = Directory.GetCurrentDirectory() + LauncherGlobals.DIR_DOWNLOADS;
    public static string PatchesDir = Directory.GetCurrentDirectory() + LauncherGlobals.DIR_PATCHES;
    public static string LangDir = Directory.GetCurrentDirectory() + LauncherGlobals.DIR_LANG;
    public static readonly string UpdaterExeDir = $"{Directory.GetCurrentDirectory()}\\{LauncherGlobals.FILE_UPDATER_EXE}";
    public readonly UserData userdata;
    public readonly Localizator loc;
    public List<GameInstance> instances = new List<GameInstance>();
    public int currentInstance = -1;
    public List<ServerInstance> serverInstances = new List<ServerInstance>();
    public ServerInstance? currentServer;
    public List<LauncherProcess> processes = new List<LauncherProcess>();
    public GameInstance? currentGame;
    private Process? currentGameProcess;
    private Thread? currentGameWaitExit;

    public Launcher(bool forceOffline)
    {
        this.OfflineMode = forceOffline;
        this.userdata = this.LoadUserData();
        this.loc = this.LoadLanguage();
    }

    public void Destroy()
    {
        this.TcuNet.Dispose();
        this.SaveUserData();
        Launcher.FormMain = (LauncherMainForm)null;
        if (LauncherGlobals.KILL_GAME_ON_LAUNCHER_EXIT && this.currentGameProcess != null)
        {
            this.currentGameProcess.Kill(true);
            this.currentGameProcess = (Process)null;
        }
        this.currentGameWaitExit?.Interrupt();
        this.currentGameWaitExit = (Thread)null;
    }

    [STAThread]
    private static void Main()
    {
        // ISSUE: reference to a compiler-generated method
        ApplicationConfiguration.Initialize();
        Application.SetCompatibleTextRenderingDefault(true);
        if (File.Exists(Directory.GetCurrentDirectory() + "\\7z.dll"))
            SevenZipBase.SetLibraryPath(Directory.GetCurrentDirectory() + "\\7z.dll");
        else if (File.Exists(Directory.GetCurrentDirectory() + "\\lib\\7z.dll"))
            SevenZipBase.SetLibraryPath(Directory.GetCurrentDirectory() + "\\lib\\7z.dll");
        Launcher launcher = new Launcher(false);
        Action action = (Action)(() =>
        {
            LauncherUtils.CleanupOutdatedDownloads(Launcher.DownloadsDir, LauncherGlobals.KEEP_DOWNLOAD_CACHE_FOR_DAYS);
            launcher.DeleteLocalTempFolder();
            if (launcher.userdata.IsScanForRunningInstances())
                launcher.DetectAlreadyRunningInstances();
            Application.Run((Form)(Launcher.FormMain = new LauncherMainForm(launcher)));
            Launcher.FormMain.BringToFront();
            launcher.Destroy();
        });
        if (!launcher.OfflineMode)
        {
            new Thread((ThreadStart)(() => Application.Run((Form)(Launcher.StartupForm = new StartupInfoForm(launcher))))).Start();
            Thread.Sleep(1000);
            while (Launcher.StartupForm == null)
                Thread.Sleep(50);
            if (Launcher.StartupForm != null)
            {
                launcher.LoadTcuNet();
                launcher.LoadManifests();
                Launcher.StartupForm.Invoke(new Action(Application.Exit));
            }
            action();
        }
        else
            action();
    }

    public bool IsLauncherOutdated()
    {
        return this.LauncherManifest != null && this.LauncherManifest.GetLatestLauncher().Length > 0 && Launcher.VERSION != this.LauncherManifest.GetLatestLauncher();
    }

    public GameInstance NewInstanceFromDirectory(
      string? instanceName,
      string? instanceIcon,
      string directoryToExe)
    {
        return new GameInstance(instanceName, instanceIcon, directoryToExe, LauncherUtils.GetVersion(directoryToExe), TCUPatcher.IsDirectoryPatched(directoryToExe), false, (string)null);
    }

    public ServerInstance? GetServerInstance(string? server)
    {
        if (server == null)
            return (ServerInstance)null;
        foreach (ServerInstance serverInstance in this.serverInstances)
        {
            if (serverInstance.GetName() == server)
                return serverInstance;
        }
        return (ServerInstance)null;
    }

    public int AddInstance(GameInstance instance)
    {
        this.instances.Add(instance);
        return this.instances.Count - 1;
    }

    public bool RemoveInstance(GameInstance instance) => this.instances.Remove(instance);

    public void SelectInstance(int index)
    {
        this.currentInstance = index;
        if (index == -1)
            Launcher.FormMain?.ShowMainPage();
        else
            Launcher.FormMain?.ShowInstancePage(this.instances[index]);
    }

    public void SelectServer(ServerInstance? svInstance)
    {
        if (svInstance == null)
            svInstance = ServerInstance.SERVER_LOCAL;
        GameInstance instance = this.instances[this.currentInstance];
        instance.SetActiveServer(svInstance.IsLocal() ? (string)null : svInstance.GetAddress());
        Launcher.FormMain?.UpdateSelectedServer(instance, svInstance);
    }

    public void ResetUserData() => this.userdata.DefaultSettings();

    public void SaveUserData()
    {
        this.userdata.SetGameInstances(this.instances);
        this.userdata.SetServerInstances(this.serverInstances);
        if (Launcher.FormMain != null)
        {
            if (Launcher.FormMain.WindowState == FormWindowState.Maximized)
            {
                this.userdata.SetWindowSize(this.userdata.GetWidth(), this.userdata.GetHeight(), true);
            }
            else
            {
                UserData userdata = this.userdata;
                Size size = Launcher.FormMain.Size;
                int width = size.Width;
                size = Launcher.FormMain.Size;
                int height = size.Height;
                userdata.SetWindowSize(width, height, false);
            }
        }
        if (Launcher.FormMain != null)
            this.userdata.SetSplit(Launcher.FormMain.GetSplitH(), Launcher.FormMain.GetSplitV());
        this.userdata.SaveTo(Launcher.UserdataDir);
    }

    public UserData LoadUserData()
    {
        UserData userData = new UserData();
        userData.LoadFrom(Launcher.UserdataDir);
        this.instances = userData.GetGameInstances();
        this.serverInstances = userData.GetServerInstances();
        this.OfflineMode |= userData.IsOfflineMode();
        return userData;
    }

    public void LoadTcuNet()
    {
        this.TcuNet = LauncherBoot.LoadTCUNet(this.OfflineMode);
        this.OfflineMode |= this.TcuNet.isOffline;
    }

    public void LoadManifests()
    {
        this.LauncherManifest = LauncherBoot.LoadLauncherManifest(this.TcuNet);
        this.ServerManifest = LauncherBoot.LoadServerManifest(this.TcuNet);
    }

    public Localizator LoadLanguage()
    {
        return new Localizator(this.userdata.GetLang() ?? Resources.DefaultLanguage);
    }

    public void AddProcess(LauncherProcess process)
    {
        this.processes.Add(process);
        if (this.processes.Count != 1)
            return;
        Launcher.FormMain?.ShowProcess(process);
    }

    public void RemoveProcess(LauncherProcess process)
    {
        this.processes.Remove(process);
        Launcher.FormMain?.StopProcess();
    }

    public int GetNumOngoingProcesses() => this.processes.Count;

    public void PatchInstance(GameInstance gameInstance, string? manualPatchFile, string? password)
    {
        bool isManualPatch = manualPatchFile != null && manualPatchFile.Length > 0;
        int num1;
        if (!isManualPatch)
        {
            if (this.OfflineMode)
            {
                string textHeader = this.loc.Get("is_offline");
                string textDesc = this.loc.Get("launcher_patch_offline");
                Bitmap offline = Resources.offline;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction("OK");
                num1 = index + 1;
                int num2 = (int)new ConfirmForm(textHeader, textDesc, (Image)offline, buttonActionList).ShowDialog((IWin32Window)Launcher.FormMain);
                return;
            }
            if (this.ServerManifest == null)
            {
                string textHeader = this.loc.Get("is_error");
                string textDesc = this.loc.Get("launcher_patch_no_svmanifest");
                Bitmap offline = Resources.offline;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction("OK");
                num1 = index + 1;
                int num3 = (int)new ConfirmForm(textHeader, textDesc, (Image)offline, buttonActionList).ShowDialog((IWin32Window)Launcher.FormMain);
                return;
            }
        }
        TCUServerVersion patchVer = isManualPatch ? (TCUServerVersion)null : this.ServerManifest?.PickLatestForGameVersion(gameInstance.GetGameVersion());
        if (patchVer == null && !isManualPatch)
        {
            string textHeader = this.loc.Get("is_error");
            string textDesc = this.loc.Get("launcher_patch_nopatch", new string[1]
            {
        gameInstance.GetGameVersion()
            });
            Bitmap icRefuse = Resources.ic_refuse_;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction("OK");
            num1 = index + 1;
            int num4 = (int)new ConfirmForm(textHeader, textDesc, (Image)icRefuse, buttonActionList).ShowDialog((IWin32Window)Launcher.FormMain);
        }
        else
        {
            LauncherProcess process = (LauncherProcess)null;
            process = new LauncherProcess(this.loc.Get("process_patching", new string[1]
            {
        gameInstance.GetInstanceName()
            }), LauncherProcessType.Patch, gameInstance, (Action)(() =>
            {
                if (process != null)
                    this.AddProcess(process);
                Launcher.FormMain?.UpdateInstanceInfo(gameInstance);
                Launcher.FormMain?.UpdateInstancePage(gameInstance);
                new Thread((ThreadStart)(() =>
          {
                  KeyValuePair<bool, string> result = !isManualPatch ? TCUPatcher.DownloadAndDeployPatch(this.loc, this.TcuNet, patchVer, this.ServerManifest, gameInstance, password) : (!TCUPatcher.CheckArchive(manualPatchFile, password) ? new KeyValuePair<bool, string>(false, this.loc.Get("errormsg_patch_archivecheck_failed")) : TCUPatcher.DeployPatch(this.loc, gameInstance, manualPatchFile, LauncherGlobals.MANUAL_PATCH_VERSION_PLACEHOLDER, password));
                  if (result.Key)
                      Launcher.FormMain?.Invoke((Action)(() => process?.Complete()));
                  else
                      Launcher.FormMain?.Invoke((Action)(() => process?.Fail(result.Value ?? this.loc.Get("fail_no_reason"))));
              }))
                {
                    IsBackground = true
                }.Start();
            }), (Action<int>)(percent => { }), (Action)(() =>
            {
                if (process != null)
                    this.RemoveProcess(process);
                gameInstance.SetPatched(true);
                if (gameInstance == this.instances[this.currentInstance])
                {
                    Launcher.FormMain?.UpdateInstanceInfo(gameInstance);
                    Launcher.FormMain?.UpdateInstancePage(gameInstance);
                }
                string textHeader = this.loc.Get("is_success");
                string textDesc = this.loc.Get("launcher_patch_success", new string[2]
          {
          gameInstance.GetInstanceName(),
          isManualPatch ? LauncherGlobals.MANUAL_PATCH_VERSION_PLACEHOLDER : patchVer.GetVersion()
              });
                Bitmap check = Resources.check;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(this.loc.Get("ok"));
                int num5 = index + 1;
                int num6 = (int)new ConfirmForm(textHeader, textDesc, (Image)check, buttonActionList).ShowDialog((IWin32Window)Launcher.FormMain);
            }), (Action<string>)(failReason =>
            {
                if (process != null)
                    this.RemoveProcess(process);
                gameInstance.SetPatched(false);
                if (gameInstance == this.instances[this.currentInstance])
                {
                    Launcher.FormMain?.UpdateInstanceInfo(gameInstance);
                    Launcher.FormMain?.UpdateInstancePage(gameInstance);
                }
                string textHeader = this.loc.Get("patching_failed");
                string textDesc = this.loc.Get("launcher_patch_failed", new string[2]
          {
          gameInstance.GetInstanceName(),
          failReason
              });
                Bitmap icRefuse = Resources.ic_refuse_;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(this.loc.Get("ok"));
                int num7 = index + 1;
                int num8 = (int)new ConfirmForm(textHeader, textDesc, (Image)icRefuse, buttonActionList).ShowDialog((IWin32Window)Launcher.FormMain);
            }));
            process.Begin();
        }
    }

    public void WriteServerIni(GameInstance gameInstance, ServerConfig serverConfig)
    {
        serverConfig.WriteIniFile(this.userdata, $"{Path.GetDirectoryName(gameInstance.GetDirectoryPath())}\\{LauncherGlobals.SERVER_CONFIG_FILE}");
    }

    public void UpdateLauncher()
    {
        if (this.OfflineMode || !this.IsLauncherOutdated() || this.GetNumOngoingProcesses() != 0 || this.LauncherManifest == null)
            return;
        Application.Exit();
        if (!File.Exists(Launcher.UpdaterExeDir))
            return;
        string processName = Process.GetCurrentProcess().ProcessName;
        if (!processName.ToLower().EndsWith(".exe"))
            processName += ".exe";
        Process.Start(new ProcessStartInfo(Launcher.UpdaterExeDir)
        {
            UseShellExecute = true,
            Arguments = "-autoinstall -runlauncher=" + processName
        });
    }

    protected Thread WaitForProcessExitThread(GameInstance instance, Process process, Action? onExit)
    {
        this.currentGameWaitExit = new Thread((ThreadStart)(() =>
        {
            try
            {
                process.WaitForExit();
                this.currentGame = (GameInstance)null;
                Action action = onExit;
                if (action != null)
                    action();
                if (this.instances[this.currentInstance] == instance)
                    Launcher.FormMain?.Invoke((Action)(() => Launcher.FormMain.UpdateInstancePage(instance)));
                Launcher.FormMain?.Invoke((Action)(() => Launcher.FormMain.UpdateInstanceInfo(instance)));
                this.currentGameProcess = (Process)null;
                this.currentGameWaitExit = (Thread)null;
            }
            catch (ThreadInterruptedException ex)
            {
                this.currentGameWaitExit = (Thread)null;
                this.currentGameProcess = (Process)null;
            }
        }));
        this.currentGameWaitExit.Start();
        return this.currentGameWaitExit;
    }

    public bool PlayInstance(GameInstance instance, Action? onExit)
    {
        ServerInstance serverInstance = this.currentServer ?? ServerInstance.SERVER_LOCAL;
        if (serverInstance != null)
            this.WriteServerIni(instance, serverInstance.GetServerConfig());
        string directoryPath = instance.GetDirectoryPath();
        string directoryName = Path.GetDirectoryName(directoryPath);
        if (directoryPath == null || directoryName == null)
            return false;
        int num = LauncherUtils.IsSteamVersion(instance.GetDirectoryPath()) ? 1 : 0;
        string customExe = instance.GetCustomEXE();
        if (num != 0 && customExe == null)
        {
            Process process = Process.Start(new ProcessStartInfo(LauncherGlobals.STEAM_START_LINK + LauncherGlobals.STEAM_APPID.ToString())
            {
                UseShellExecute = true
            });
            if (process != null)
            {
                this.currentGameProcess = process;
                this.currentGame = instance;
                this.WaitForProcessExitThread(instance, process, onExit);
            }
        }
        else
        {
            Process process = Process.Start(customExe != null ? $"{directoryName}\\{customExe}" : directoryPath);
            this.currentGameProcess = process;
            this.currentGame = instance;
            this.WaitForProcessExitThread(instance, process, onExit);
        }
        return true;
    }

    public KeyValuePair<int, GameInstance>? GetInstanceFromDirectory(string dirToExe)
    {
        for (int index = 0; index < this.instances.Count; ++index)
        {
            GameInstance instance = this.instances[index];
            string directoryPath = instance.GetDirectoryPath();
            if (directoryPath != null && Path.GetFullPath(dirToExe).Equals(Path.GetFullPath(directoryPath)))
                return new KeyValuePair<int, GameInstance>?(new KeyValuePair<int, GameInstance>(index, instance));
        }
        return new KeyValuePair<int, GameInstance>?();
    }

    public void DetectAlreadyRunningInstances()
    {
        foreach (Process process in Process.GetProcessesByName("TheCrew"))
        {
            string fileName = process.MainModule?.FileName;
            if (fileName != null && File.Exists(fileName))
            {
                KeyValuePair<int, GameInstance>? instanceFromDirectory = this.GetInstanceFromDirectory(fileName);
                if (instanceFromDirectory.HasValue)
                {
                    this.currentGame = instanceFromDirectory.Value.Value;
                    this.currentGameProcess = process;
                    Launcher.FormMain?.UpdateInstanceInfo(this.currentGame);
                    this.WaitForProcessExitThread(this.currentGame, process, (Action)null);
                }
            }
        }
    }

    public Form LegalInfoForm()
    {
        string textHeader = this.loc.Get("settings_legal_info");
        string textDesc = this.loc.Get("settings_legal_info_details", new string[14]
        {
      Resources.ProjectEst,
      Resources.Developers,
      "Ubisoft",
      "Ivory Tower",
      Launcher.VERSION,
      "C#",
      "WinForms",
      "Microsoft WebView2",
      "SevenZipSharp (by Squid-Box)",
      this.loc.Get("settings_clear_downloads"),
      TCUNetUtils.TCUNET_HOSTNAMES[0],
      this.loc.Get("settings_launcher_offlinemode"),
      "The Crew",
      "DYXiCZ"
        });
        Bitmap icDboxInfo = Resources.ic_dbox_info;
        List<ButtonAction> buttonActionList = new List<ButtonAction>();
        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
        int index = 0;
        span[index] = new ButtonAction(this.loc.Get("ok"), (Action)(() => this.userdata.SetIsShownLegalInfo(true)));
        int num = index + 1;
        return (Form)new ConfirmForm(textHeader, textDesc, (Image)icDboxInfo, buttonActionList);
    }

    public Form ShowSaveInUseWarningForm()
    {
        string textHeader = this.loc.Get("alert");
        string textDesc = this.loc.Get("save_already_in_use", new string[1]
        {
      SaveManager.FILENAME + SaveManager.FORMAT_BIN
        });
        Bitmap icDboxWarning = Resources.ic_dbox_warning;
        List<ButtonAction> buttonActionList = new List<ButtonAction>();
        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
        int index = 0;
        span[index] = new ButtonAction(this.loc.Get("ok"));
        int num = index + 1;
        return (Form)new ConfirmForm(textHeader, textDesc, (Image)icDboxWarning, buttonActionList);
    }

    public bool DeleteLocalTempFolder()
    {
        string path = Directory.GetCurrentDirectory() + LauncherGlobals.DIR_LOCAL_TEMP;
        if (!Directory.Exists(path))
            return false;
        try
        {
            Directory.Delete(path, true);
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            return false;
        }
    }
}
