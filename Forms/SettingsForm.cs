using System.ComponentModel;
using System.Runtime.InteropServices;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher.Forms;

public class SettingsForm : Form
{
  private readonly Launcher launcher;
  private Action? onSettingsReset;
  private Dictionary<string, string> LangFilesByLangName;
  private 
  #nullable disable
  FontManager fontManager = new FontManager().SetupDefaultFonts();
  private IContainer components;
  private CheckBox OptCBOfflineMode;
  private CheckBox OptCBLauncherUpdatePrompt;
  private ComboBox ListLang;
  private Label LabelLang;
  private CheckBox OptCBOfflineCheats;
  private Label LabelOptions;
  private ToolTip toolTip;
  private Button BtnClearDownloads;
  private Button BtnLegalInfo;
  private Button BtnUpdate;
  private Button BtnResetSettings;
  private CheckBox OptCBScanRunningInstances;

  public SettingsForm(
  #nullable enable
  Launcher launcher, Action? onSettingsReset)
  {
    this.InitializeComponent();
    this.launcher = launcher;
    this.onSettingsReset = onSettingsReset;
    this.LangFilesByLangName = new Dictionary<string, string>();
    this.SetupStuff();
  }

  protected static string truncatePath(string path, string file)
  {
    if (file.StartsWith(path))
    {
      string str = file;
      int length = path.Length;
      file = str.Substring(length, str.Length - length);
    }
    if (file.StartsWith("\\") || file.StartsWith("/"))
    {
      string str = file;
      file = str.Substring(1, str.Length - 1);
    }
    return file;
  }

  protected void SetupStuff()
  {
    this.Icon = Resources.IconTCU;
    this.Text = this.launcher.loc.Get("settings");
    this.LabelOptions.Text = this.launcher.loc.Get("settings_options");
    this.OptCBOfflineMode.Text = this.launcher.loc.Get("settings_launcher_offlinemode");
    this.OptCBLauncherUpdatePrompt.Text = this.launcher.loc.Get("settings_launcher_update_prompt");
    this.OptCBOfflineCheats.Text = this.launcher.loc.Get("settings_offline_cheats");
    this.OptCBScanRunningInstances.Text = this.launcher.loc.Get("settings_scan_running_instances");
    this.toolTip.SetToolTip((Control) this.OptCBOfflineMode, this.launcher.loc.Get("tooltip_settings_launcher_offlinemode", new string[1]
    {
      Resources.NetworkTitle
    }));
    this.toolTip.SetToolTip((Control) this.OptCBLauncherUpdatePrompt, this.launcher.loc.Get("tooltip_settings_launcher_update_prompt"));
    this.toolTip.SetToolTip((Control) this.OptCBOfflineCheats, this.launcher.loc.Get("tooltip_settings_offline_cheats"));
    this.toolTip.SetToolTip((Control) this.OptCBScanRunningInstances, this.launcher.loc.Get("tooltip_settings_scan_running_instances", new string[1]
    {
      "The Crew"
    }));
    this.LabelLang.Text = this.launcher.loc.Get("settings_language");
    this.BtnResetSettings.Text = this.launcher.loc.Get("settings_reset");
    this.BtnClearDownloads.Text = this.launcher.loc.Get("settings_clear_downloads");
    this.BtnUpdate.Text = this.launcher.OfflineMode ? this.launcher.loc.Get("settings_update_launcher_offline") : (this.launcher.IsLauncherOutdated() ? this.launcher.loc.Get("settings_update_launcher") : this.launcher.loc.Get("settings_launcher_updated"));
    this.BtnUpdate.Enabled = !this.launcher.OfflineMode && this.launcher.IsLauncherOutdated();
    this.BtnLegalInfo.Text = this.launcher.loc.Get("settings_legal_info");
    this.toolTip.SetToolTip((Control) this.BtnResetSettings, this.launcher.loc.Get("tooltip_settings_reset"));
    this.toolTip.SetToolTip((Control) this.BtnClearDownloads, this.launcher.loc.Get("tooltip_settings_clear_downloads", new string[1]
    {
      LauncherGlobals.KEEP_DOWNLOAD_CACHE_FOR_DAYS.ToString()
    }));
    this.toolTip.SetToolTip((Control) this.BtnUpdate, this.launcher.loc.Get("tooltip_settings_launcher_update"));
    this.toolTip.SetToolTip((Control) this.BtnLegalInfo, this.launcher.loc.Get("tooltip_settings_legal_info"));
    this.OptCBOfflineMode.Checked = this.launcher.userdata.IsOfflineMode();
    this.OptCBLauncherUpdatePrompt.Checked = this.launcher.userdata.IsLauncherUpdatePromptsEnabled();
    this.OptCBOfflineCheats.Checked = this.launcher.userdata.IsOfflineCheatsEnabled();
    this.OptCBScanRunningInstances.Checked = this.launcher.userdata.IsScanForRunningInstances();
    string langDir = Launcher.LangDir;
    if (!langDir.EndsWith("\\") && !langDir.EndsWith("/"))
      langDir += "\\";
    List<string> stringList = new List<string>();
    if (langDir.Length > 0 && Path.Exists(langDir))
    {
      foreach (string enumerateFile in Directory.EnumerateFiles(Launcher.LangDir))
      {
        string str = SettingsForm.truncatePath(Environment.CurrentDirectory, enumerateFile);
        if (str.EndsWith(LauncherGlobals.LANG_FILE_EXT))
          stringList.Add(str);
      }
    }
    this.ListLang.Items.Clear();
    if (stringList.Count > 0)
    {
      this.ListLang.Enabled = true;
      string str1 = this.launcher.userdata.GetLang() ?? Resources.DefaultLanguage;
      if (str1.StartsWith("\\") || str1.StartsWith("/"))
      {
        string str2 = str1;
        str1 = str2.Substring(1, str2.Length - 1);
      }
      int num = 0;
      foreach (string langFile in stringList)
      {
        Lang lang = new Lang();
        lang.LoadFromFile(langFile);
        string str3;
        if (!this.launcher.loc.HasString("settings_language_credit"))
          str3 = $"{lang.GetLanguage()} ({lang.GetAuthor()})";
        else
          str3 = this.launcher.loc.Get("settings_language_credit", new string[2]
          {
            lang.GetLanguage(),
            lang.GetAuthor() ?? "?"
          });
        string key = str3;
        this.LangFilesByLangName.Add(key, langFile);
        this.ListLang.Items.Add((object) key);
        if (langFile.Equals(str1.Replace("/", "\\")))
          this.ListLang.SelectedIndex = num;
        ++num;
      }
    }
    else
    {
      this.ListLang.Text = "No language files available!";
      this.ListLang.Enabled = false;
    }
    this.OptCBOfflineMode.CheckedChanged += (EventHandler) ((sender, e) => this.launcher.userdata.SetOfflineMode(this.OptCBOfflineMode.Checked));
    this.OptCBLauncherUpdatePrompt.CheckedChanged += (EventHandler) ((sender, e) => this.launcher.userdata.SetLauncherUpdatePromptsEnabled(this.OptCBLauncherUpdatePrompt.Checked));
    this.OptCBOfflineCheats.CheckedChanged += (EventHandler) ((sender, e) => this.launcher.userdata.SetOfflineCheatsEnabled(this.OptCBOfflineCheats.Checked));
    this.OptCBScanRunningInstances.CheckedChanged += (EventHandler) ((sender, e) => this.launcher.userdata.SetScanForRunningInstances(this.OptCBScanRunningInstances.Checked));
    this.BtnUpdate.Click += (EventHandler) ((sender, e) =>
    {
      if (this.launcher.OfflineMode || !this.launcher.IsLauncherOutdated())
        return;
      TCULauncherManifest launcherManifest = this.launcher.LauncherManifest;
      int num1;
      if (this.launcher.GetNumOngoingProcesses() > 0)
      {
        string textHeader = this.launcher.loc.Get("processes_ongoing");
        string textDesc = this.launcher.loc.Get("processes_ongoing_details");
        Bitmap icDboxWarning = Resources.ic_dbox_warning;
        List<ButtonAction> buttonActionList = new List<ButtonAction>();
        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
        int index = 0;
        span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
        num1 = index + 1;
        ConfirmForm confirmForm = new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList);
      }
      else
      {
        if (launcherManifest == null)
          return;
        string textHeader = this.launcher.loc.Get("new_update");
        string textDesc = this.launcher.loc.Get("settings_update_details", new string[1]
        {
          launcherManifest.GetLatestLauncher()
        });
        Bitmap icDboxInfo = Resources.ic_dbox_info;
        List<ButtonAction> buttonActionList = new List<ButtonAction>();
        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
        int index1 = 0;
        span[index1] = new ButtonAction(this.launcher.loc.Get("ok"), new Action(this.launcher.UpdateLauncher));
        int index2 = index1 + 1;
        span[index2] = new ButtonAction(this.launcher.loc.Get("cancel"));
        num1 = index2 + 1;
        int num2 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxInfo, buttonActionList).ShowDialog((IWin32Window) this);
      }
    });
    this.BtnResetSettings.Click += (EventHandler) ((sender, e) =>
    {
      string userdataDir = Launcher.UserdataDir;
      bool closeSelf = false;
      string textHeader = this.launcher.loc.Get("title_settings_reset");
      string textDesc = this.launcher.loc.Get("settings_reset_details", new string[1]
      {
        userdataDir
      });
      Bitmap icDboxWarning = Resources.ic_dbox_warning;
      List<ButtonAction> buttonActionList = new List<ButtonAction>();
      CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
      Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
      int index3 = 0;
      span[index3] = new ButtonAction(this.launcher.loc.Get("ok"), (Action) (() =>
      {
        closeSelf = true;
        this.launcher.ResetUserData();
      }));
      int index4 = index3 + 1;
      span[index4] = new ButtonAction(this.launcher.loc.Get("cancel"));
      int num3 = index4 + 1;
      int num4 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList).ShowDialog((IWin32Window) this);
      if (!closeSelf)
        return;
      this.Close();
      if (this.onSettingsReset == null)
        return;
      this.onSettingsReset();
    });
    this.BtnClearDownloads.Click += (EventHandler) ((sender, e) => LauncherUtils.CleanupOutdatedDownloads(Launcher.DownloadsDir, 0));
    int num5;
    this.BtnLegalInfo.Click += (EventHandler) ((sender, e) => num5 = (int) this.launcher.LegalInfoForm().ShowDialog((IWin32Window) this));
    this.ListLang.SelectedIndexChanged += (EventHandler) ((sender, e) =>
    {
      string text = this.ListLang.Text;
      string lang;
      if (text.Length <= 0 || !this.LangFilesByLangName.TryGetValue(text, out lang) || lang == null || lang.Equals(this.launcher.userdata.GetLang()))
        return;
      this.launcher.userdata.SetLang(lang);
    });
    this.fontManager?.OverrideFonts((Control) this, new bool?(true));
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && this.components != null)
      this.components.Dispose();
    base.Dispose(disposing);
    this.fontManager?.Dispose();
  }

  private void InitializeComponent()
  {
    this.components = (IContainer) new System.ComponentModel.Container();
    this.OptCBOfflineMode = new CheckBox();
    this.OptCBLauncherUpdatePrompt = new CheckBox();
    this.ListLang = new ComboBox();
    this.LabelLang = new Label();
    this.OptCBOfflineCheats = new CheckBox();
    this.LabelOptions = new Label();
    this.toolTip = new ToolTip(this.components);
    this.BtnClearDownloads = new Button();
    this.BtnLegalInfo = new Button();
    this.BtnUpdate = new Button();
    this.BtnResetSettings = new Button();
    this.OptCBScanRunningInstances = new CheckBox();
    this.SuspendLayout();
    this.OptCBOfflineMode.AutoSize = true;
    this.OptCBOfflineMode.Font = new Font("TheCrew Sans Regular", 12f);
    this.OptCBOfflineMode.ForeColor = Color.White;
    this.OptCBOfflineMode.Location = new Point(12, 108);
    this.OptCBOfflineMode.Name = "OptCBOfflineMode";
    this.OptCBOfflineMode.Size = new Size(149, 23);
    this.OptCBOfflineMode.TabIndex = 0;
    this.OptCBOfflineMode.Text = "Launcher Offline Mode";
    this.OptCBOfflineMode.UseVisualStyleBackColor = true;
    this.OptCBLauncherUpdatePrompt.AutoSize = true;
    this.OptCBLauncherUpdatePrompt.Font = new Font("TheCrew Sans Regular", 12f);
    this.OptCBLauncherUpdatePrompt.ForeColor = Color.White;
    this.OptCBLauncherUpdatePrompt.Location = new Point(12, 137);
    this.OptCBLauncherUpdatePrompt.Name = "OptCBLauncherUpdatePrompt";
    this.OptCBLauncherUpdatePrompt.Size = new Size(202, 23);
    this.OptCBLauncherUpdatePrompt.TabIndex = 1;
    this.OptCBLauncherUpdatePrompt.Text = "Prompt about Launcher Updates";
    this.OptCBLauncherUpdatePrompt.UseVisualStyleBackColor = true;
    this.ListLang.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    this.ListLang.DropDownStyle = ComboBoxStyle.DropDownList;
    this.ListLang.FormattingEnabled = true;
    this.ListLang.Location = new Point(12, 39);
    this.ListLang.Name = "ListLang";
    this.ListLang.Size = new Size(307, 23);
    this.ListLang.TabIndex = 2;
    this.LabelLang.AutoSize = true;
    this.LabelLang.BackColor = Color.Transparent;
    this.LabelLang.Font = new Font("TheCrew Sans Regular", 12f);
    this.LabelLang.ForeColor = Color.White;
    this.LabelLang.Location = new Point(12, 11);
    this.LabelLang.Name = "LabelLang";
    this.LabelLang.Size = new Size(59, 25);
    this.LabelLang.TabIndex = 3;
    this.LabelLang.Text = "Language";
    this.LabelLang.UseCompatibleTextRendering = true;
    this.OptCBOfflineCheats.AutoSize = true;
    this.OptCBOfflineCheats.Font = new Font("TheCrew Sans Regular", 12f);
    this.OptCBOfflineCheats.ForeColor = Color.White;
    this.OptCBOfflineCheats.Location = new Point(12, 166);
    this.OptCBOfflineCheats.Name = "OptCBOfflineCheats";
    this.OptCBOfflineCheats.Size = new Size(136, 23);
    this.OptCBOfflineCheats.TabIndex = 7;
    this.OptCBOfflineCheats.Text = "Offline Mode Cheats";
    this.OptCBOfflineCheats.UseVisualStyleBackColor = true;
    this.LabelOptions.AutoSize = true;
    this.LabelOptions.BackColor = Color.Transparent;
    this.LabelOptions.Font = new Font("TheCrew Sans Regular", 12f);
    this.LabelOptions.ForeColor = Color.White;
    this.LabelOptions.Location = new Point(12, 80 /*0x50*/);
    this.LabelOptions.Name = "LabelOptions";
    this.LabelOptions.Size = new Size(48 /*0x30*/, 25);
    this.LabelOptions.TabIndex = 8;
    this.LabelOptions.Text = "Options";
    this.LabelOptions.UseCompatibleTextRendering = true;
    this.toolTip.AutoPopDelay = 30000;
    this.toolTip.InitialDelay = 500;
    this.toolTip.IsBalloon = true;
    this.toolTip.ReshowDelay = 100;
    this.toolTip.ShowAlways = true;
    this.BtnClearDownloads.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.BtnClearDownloads.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnClearDownloads.FlatStyle = FlatStyle.Flat;
    this.BtnClearDownloads.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold);
    this.BtnClearDownloads.ForeColor = Color.Black;
    this.BtnClearDownloads.Location = new Point(168, 357);
    this.BtnClearDownloads.Name = "BtnClearDownloads";
    this.BtnClearDownloads.Size = new Size(150, 30);
    this.BtnClearDownloads.TabIndex = 11;
    this.BtnClearDownloads.Text = "Clear Downloads";
    this.BtnClearDownloads.UseCompatibleTextRendering = true;
    this.BtnClearDownloads.UseVisualStyleBackColor = false;
    this.BtnLegalInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.BtnLegalInfo.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnLegalInfo.FlatStyle = FlatStyle.Flat;
    this.BtnLegalInfo.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold);
    this.BtnLegalInfo.ForeColor = Color.Black;
    this.BtnLegalInfo.Location = new Point(168, 393);
    this.BtnLegalInfo.Name = "BtnLegalInfo";
    this.BtnLegalInfo.Size = new Size(150, 30);
    this.BtnLegalInfo.TabIndex = 12;
    this.BtnLegalInfo.Text = "Legal Info";
    this.BtnLegalInfo.UseCompatibleTextRendering = true;
    this.BtnLegalInfo.UseVisualStyleBackColor = false;
    this.BtnUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.BtnUpdate.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnUpdate.FlatStyle = FlatStyle.Flat;
    this.BtnUpdate.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold);
    this.BtnUpdate.ForeColor = Color.Black;
    this.BtnUpdate.Location = new Point(13, 393);
    this.BtnUpdate.Name = "BtnUpdate";
    this.BtnUpdate.Size = new Size(150, 30);
    this.BtnUpdate.TabIndex = 10;
    this.BtnUpdate.Text = "Update Launcher";
    this.BtnUpdate.UseCompatibleTextRendering = true;
    this.BtnUpdate.UseVisualStyleBackColor = false;
    this.BtnResetSettings.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.BtnResetSettings.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnResetSettings.FlatStyle = FlatStyle.Flat;
    this.BtnResetSettings.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold);
    this.BtnResetSettings.ForeColor = Color.Black;
    this.BtnResetSettings.Location = new Point(13, 357);
    this.BtnResetSettings.Name = "BtnResetSettings";
    this.BtnResetSettings.Size = new Size(150, 30);
    this.BtnResetSettings.TabIndex = 6;
    this.BtnResetSettings.Text = "Reset To Default";
    this.BtnResetSettings.UseCompatibleTextRendering = true;
    this.BtnResetSettings.UseVisualStyleBackColor = false;
    this.OptCBScanRunningInstances.AutoSize = true;
    this.OptCBScanRunningInstances.Font = new Font("TheCrew Sans Regular", 12f);
    this.OptCBScanRunningInstances.ForeColor = Color.White;
    this.OptCBScanRunningInstances.Location = new Point(12, 195);
    this.OptCBScanRunningInstances.Name = "OptCBScanRunningInstances";
    this.OptCBScanRunningInstances.Size = new Size(158, 23);
    this.OptCBScanRunningInstances.TabIndex = 13;
    this.OptCBScanRunningInstances.Text = "Scan Running Instances";
    this.OptCBScanRunningInstances.UseVisualStyleBackColor = true;
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackColor = Color.FromArgb(14, 14, 14);
    this.ClientSize = new Size(331, 436);
    this.Controls.Add((Control) this.OptCBScanRunningInstances);
    this.Controls.Add((Control) this.BtnLegalInfo);
    this.Controls.Add((Control) this.BtnClearDownloads);
    this.Controls.Add((Control) this.BtnUpdate);
    this.Controls.Add((Control) this.LabelOptions);
    this.Controls.Add((Control) this.OptCBOfflineCheats);
    this.Controls.Add((Control) this.BtnResetSettings);
    this.Controls.Add((Control) this.LabelLang);
    this.Controls.Add((Control) this.ListLang);
    this.Controls.Add((Control) this.OptCBLauncherUpdatePrompt);
    this.Controls.Add((Control) this.OptCBOfflineMode);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (SettingsForm);
    this.StartPosition = FormStartPosition.CenterParent;
    this.Text = "Settings";
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
