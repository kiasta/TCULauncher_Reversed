using System.ComponentModel;
using System.Runtime.InteropServices;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher.Forms;

public class LocalSaveMgrForm : Form
{
  public static readonly string ALLOWED_CHARS_INPUT_INT = "0123456789";
  public static readonly string ALLOWED_CHARS_INPUT_FLOAT = "0123456789.eE+-";
  public static readonly string ALLOWED_CHARS_INPUT_BOOL = "01";
  private readonly Launcher launcher;
  private GameInstance? GameInstance;
  private ServerInstance? ServerInstance;
  private ServerConfig? ServerConfig;
  private Dictionary<string, Cheat> AvailableCheats = new Dictionary<string, Cheat>();
  private bool changingCheat;
  private 
  #nullable disable
  FontManager fontManager = new FontManager().SetupDefaultFonts();
  private IContainer components;
  private Label LabelSave;
  private ComboBox SaveSelect;
  private Button BtnAddSave;
  private ToolTip tooltip;
  private Button BtnRemoveSave;
  private Button BtnImportSave;
  private Button BtnExportSave;
  private Button BtnImportDumpedSave;
  private ComboBox CheatSelect;
  private Label LabelCheats;
  private TextBox CheatInput;
  private OpenFileDialog openFileDialog;
  private SaveFileDialog saveFileDialog;
  private Button BtnEditSave;
  private Button BtnApplyCheats;

  public LocalSaveMgrForm(
  #nullable enable
  Launcher launcher)
  {
    this.launcher = launcher;
    this.InitializeComponent();
    this.SetupStuff();
    this.AddTooltips();
    this.AddEventListeners();
  }

  protected void SetupStuff()
  {
    this.Icon = Launcher.DefaultIcon;
    this.LabelSave.Text = this.launcher.loc.Get("title_save_active");
    this.BtnImportSave.Text = this.launcher.loc.Get("title_save_import");
    this.BtnExportSave.Text = this.launcher.loc.Get("title_save_export");
    this.BtnImportDumpedSave.Text = this.launcher.loc.Get("title_save_import_ubi");
    this.BtnApplyCheats.Text = this.launcher.loc.Get("apply");
    this.LabelCheats.Text = this.launcher.loc.Get("title_cheats");
    this.openFileDialog.Title = this.launcher.loc.Get("title_save_export_select");
    this.Text = this.launcher.loc.Get("local_server_manager");
    this.openFileDialog.Multiselect = false;
    this.openFileDialog.DefaultExt = LauncherGlobals.SAVEGAME_EXT;
    this.openFileDialog.Filter = "TCU Savegame files|*." + LauncherGlobals.SAVEGAME_EXT;
    this.saveFileDialog.DefaultExt = LauncherGlobals.SAVEGAME_EXT;
    this.saveFileDialog.Filter = "TCU Savegame files|*." + LauncherGlobals.SAVEGAME_EXT;
    this.SaveSelect.Items.Clear();
    this.CheatSelect.Items.Clear();
    this.fontManager?.OverrideFonts((Control) this, new bool?(true));
  }

  protected void AddTooltips()
  {
    this.tooltip.SetToolTip((Control) this.BtnAddSave, this.launcher.loc.Get("tooltip_save_add"));
    this.tooltip.SetToolTip((Control) this.BtnRemoveSave, this.launcher.loc.Get("tooltip_save_remove"));
    this.tooltip.SetToolTip((Control) this.BtnImportSave, this.launcher.loc.Get("tooltip_save_import"));
    this.tooltip.SetToolTip((Control) this.BtnExportSave, this.launcher.loc.Get("tooltip_save_export"));
    this.tooltip.SetToolTip((Control) this.BtnImportDumpedSave, this.launcher.loc.Get("tooltip_save_import_ubi"));
    this.tooltip.SetToolTip((Control) this.BtnEditSave, this.launcher.loc.Get("tooltip_save_rename"));
  }

  protected void AddEventListeners()
  {
    this.SaveSelect.SelectedIndexChanged += (EventHandler) ((sender, e) =>
    {
      string text = this.SaveSelect.Text;
      if (text == null || this.GameInstance == null || this.ServerInstance == null || this.ServerConfig == null)
        return;
      SaveInstance saveInstance = this.GameInstance.GetSaveInstance();
      if (saveInstance == null || !saveInstance.PlayerNameExists(text))
        return;
      this.ServerConfig.SetUsername(text, true);
      this.ServerConfig.SetConfigFile(this.GameInstance, this.ServerInstance);
    });
    this.BtnAddSave.Click += (EventHandler) ((sender, e) =>
    {
      if (!LauncherUtils.CanUseSaveFile(this.launcher, this.GameInstance))
      {
        this.launcher.ShowSaveInUseWarningForm();
      }
      else
      {
        SaveInstance save = this.GameInstance?.GetSaveInstance();
        if (save == null)
          return;
        SaveImportForm saveImportForm = new SaveImportForm(this.launcher, save, (string) null, SaveImportForm.SaveImportFormMode.AddNewSave, (Action<string, string>) ((filepath, savegameName) =>
        {
          int num5;
          if (save.AddNewSave(savegameName))
          {
            string textHeader = this.launcher.loc.Get("is_success");
            string textDesc = this.launcher.loc.Get("save_added", new string[1]
            {
              savegameName
            });
            Bitmap check = Resources.check;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num5 = index + 1;
            int num6 = (int) new ConfirmForm(textHeader, textDesc, (Image) check, buttonActionList).ShowDialog((IWin32Window) this);
            int num7 = this.SaveSelect.Items.Add((object) savegameName);
            save.SaveToFile((string) null);
            this.SaveSelect.SelectedIndex = num7;
          }
          else
          {
            string textHeader = this.launcher.loc.Get("is_error");
            string textDesc = this.launcher.loc.Get("save_added_failed");
            Bitmap icRefuse = Resources.ic_refuse_;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num5 = index + 1;
            int num8 = (int) new ConfirmForm(textHeader, textDesc, (Image) icRefuse, buttonActionList).ShowDialog((IWin32Window) this);
          }
        }));
        if (saveImportForm.IsDisposed)
          return;
        int num = (int) saveImportForm.ShowDialog((IWin32Window) this);
      }
    });
    this.BtnRemoveSave.Click += (EventHandler) ((sender, e) =>
    {
      if (!LauncherUtils.CanUseSaveFile(this.launcher, this.GameInstance))
      {
        this.launcher.ShowSaveInUseWarningForm();
      }
      else
      {
        if (this.GameInstance == null)
          return;
        SaveInstance save = this.GameInstance.GetSaveInstance();
        if (save == null)
          return;
        string saveName = this.SaveSelect.Text;
        if (saveName == null || saveName.Length <= 0)
          return;
        int num12;
        if (save.PlayerNameExists(saveName))
        {
          string textHeader1 = this.launcher.loc.Get("deleting_savegame");
          string textDesc1 = this.launcher.loc.Get("save_delete_details", new string[1]
          {
            saveName
          });
          Bitmap icDboxWarning1 = Resources.ic_dbox_warning;
          List<ButtonAction> buttonActionList1 = new List<ButtonAction>();
          CollectionsMarshal.SetCount<ButtonAction>(buttonActionList1, 2);
          Span<ButtonAction> span1 = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList1);
          int index1 = 0;
          span1[index1] = new ButtonAction(this.launcher.loc.Get("save_delete"), (Action) (() =>
          {
            int num13;
            if (save.RemoveSave(saveName))
            {
              if (save.GetPlayerNames().Count == 0)
                this.ServerConfig?.SetUsername(LauncherGlobals.SAVEGAME_DEFAULT_NAME, true);
              string textHeader2 = this.launcher.loc.Get("alert");
              string textDesc2 = this.launcher.loc.Get("save_delete_success", new string[1]
              {
                saveName
              });
              Bitmap icDboxWarning2 = Resources.ic_dbox_warning;
              List<ButtonAction> buttonActionList2 = new List<ButtonAction>();
              CollectionsMarshal.SetCount<ButtonAction>(buttonActionList2, 1);
              Span<ButtonAction> span2 = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList2);
              int index2 = 0;
              span2[index2] = new ButtonAction(this.launcher.loc.Get("ok"));
              num13 = index2 + 1;
              int num14 = (int) new ConfirmForm(textHeader2, textDesc2, (Image) icDboxWarning2, buttonActionList2).ShowDialog((IWin32Window) this);
              save.SaveToFile((string) null);
              this.SaveSelect.Items.Remove((object) saveName);
              this.SelectFirstAvailableSave(this.GameInstance, this.ServerConfig);
            }
            else
            {
              string textHeader3 = this.launcher.loc.Get("alert");
              string textDesc3 = this.launcher.loc.Get("save_delete_failed", new string[1]
              {
                saveName
              });
              Bitmap icDboxWarning3 = Resources.ic_dbox_warning;
              List<ButtonAction> buttonActionList3 = new List<ButtonAction>();
              CollectionsMarshal.SetCount<ButtonAction>(buttonActionList3, 1);
              Span<ButtonAction> span3 = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList3);
              int index3 = 0;
              span3[index3] = new ButtonAction(this.launcher.loc.Get("ok"));
              num13 = index3 + 1;
              int num15 = (int) new ConfirmForm(textHeader3, textDesc3, (Image) icDboxWarning3, buttonActionList3).ShowDialog((IWin32Window) this);
            }
          }));
          int index4 = index1 + 1;
          span1[index4] = new ButtonAction(this.launcher.loc.Get("no_scary"));
          num12 = index4 + 1;
          int num16 = (int) new ConfirmForm(textHeader1, textDesc1, (Image) icDboxWarning1, buttonActionList1).ShowDialog((IWin32Window) this);
        }
        else
        {
          string textHeader = this.launcher.loc.Get("alert");
          string textDesc = this.launcher.loc.Get("save_delete_failed_nonexistent", new string[1]
          {
            saveName
          });
          Bitmap icDboxWarning = Resources.ic_dbox_warning;
          List<ButtonAction> buttonActionList = new List<ButtonAction>();
          CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
          Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
          int index = 0;
          span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
          num12 = index + 1;
          int num17 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList).ShowDialog((IWin32Window) this);
        }
      }
    });
    this.BtnImportSave.Click += (EventHandler) ((sender, e) =>
    {
      if (!LauncherUtils.CanUseSaveFile(this.launcher, this.GameInstance))
      {
        this.launcher.ShowSaveInUseWarningForm();
      }
      else
      {
        SaveInstance saveInstance1 = this.GameInstance?.GetSaveInstance();
        if (saveInstance1 == null)
          return;
        SaveImportForm saveImportForm = new SaveImportForm(this.launcher, saveInstance1, (string) null, SaveImportForm.SaveImportFormMode.ImportSave, (Action<string, string>) ((filepath, savegameName) =>
        {
          if (this.GameInstance == null)
            return;
          SaveInstance saveInstance2 = this.GameInstance.GetSaveInstance();
          int num21;
          if (saveInstance2 != null)
          {
            string str = saveInstance2.ImportSave(filepath, savegameName);
            if (str == null)
              return;
            string textHeader = this.launcher.loc.Get("success");
            string textDesc = this.launcher.loc.Get("save_import_success", new string[1]
            {
              str
            });
            Bitmap check = Resources.check;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num21 = index + 1;
            int num22 = (int) new ConfirmForm(textHeader, textDesc, (Image) check, buttonActionList).ShowDialog((IWin32Window) this);
            saveInstance2.SaveToFile((string) null);
            this.SaveSelect.SelectedIndex = this.SaveSelect.Items.Add((object) savegameName);
          }
          else
          {
            string textHeader = this.launcher.loc.Get("is_error");
            string textDesc = this.launcher.loc.Get("save_import_failed");
            Bitmap icRefuse = Resources.ic_refuse_;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num21 = index + 1;
            int num23 = (int) new ConfirmForm(textHeader, textDesc, (Image) icRefuse, buttonActionList).ShowDialog((IWin32Window) this);
          }
        }));
        if (saveImportForm.IsDisposed)
          return;
        int num = (int) saveImportForm.ShowDialog((IWin32Window) this);
      }
    });
    this.BtnExportSave.Click += (EventHandler) ((sender, e) =>
    {
      if (this.GameInstance == null || this.ServerConfig == null)
        return;
      SaveInstance saveInstance = this.GameInstance.GetSaveInstance();
      if (saveInstance == null)
        throw new Exception("No save instance attached to game instance! " + this.GameInstance.GetInstanceName());
      string text = this.SaveSelect.Text;
      if (text == null || text.Length <= 0 || !saveInstance.PlayerNameExists(text))
        return;
      this.saveFileDialog.Title = this.launcher.loc.Get("save_export_dialog_title");
      if (this.saveFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
        return;
      string fileName = this.saveFileDialog.FileName;
      if (fileName.Length <= 0)
        return;
      int num24;
      if (saveInstance.ExportSave(text, fileName))
      {
        string textHeader = this.launcher.loc.Get("is_success");
        string textDesc = this.launcher.loc.Get("save_export_success");
        Bitmap check = Resources.check;
        List<ButtonAction> buttonActionList = new List<ButtonAction>();
        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
        int index = 0;
        span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
        num24 = index + 1;
        int num25 = (int) new ConfirmForm(textHeader, textDesc, (Image) check, buttonActionList).ShowDialog((IWin32Window) this);
      }
      else
      {
        string textHeader = this.launcher.loc.Get("is_error");
        string textDesc = this.launcher.loc.Get("save_export_failed");
        Bitmap icRefuse = Resources.ic_refuse_;
        List<ButtonAction> buttonActionList = new List<ButtonAction>();
        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
        int index = 0;
        span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
        num24 = index + 1;
        int num26 = (int) new ConfirmForm(textHeader, textDesc, (Image) icRefuse, buttonActionList).ShowDialog((IWin32Window) this);
      }
    });
    this.BtnImportDumpedSave.Click += (EventHandler) ((sender, e) =>
    {
      if (!LauncherUtils.CanUseSaveFile(this.launcher, this.GameInstance))
      {
        this.launcher.ShowSaveInUseWarningForm();
      }
      else
      {
        Action action = (Action) (() =>
        {
          this.launcher.userdata.SetIsWarnedAboutDumpedSaves(true);
          SaveInstance save = this.GameInstance?.GetSaveInstance();
          if (save == null)
            return;
          SaveImportForm saveImportForm = new SaveImportForm(this.launcher, save, (string) null, SaveImportForm.SaveImportFormMode.ImportUbiSaveDump, (Action<string, string>) ((filepath, savegameName) =>
          {
            ImportUbiSaveResult importUbiSaveResult = save.ImportUbiSave(filepath, savegameName);
            int num31;
            if (importUbiSaveResult.importedSave != null)
            {
              List<string> stringList = new List<string>();
              if (importUbiSaveResult.failedSave)
                stringList.Add("Save");
              if (importUbiSaveResult.failedStats)
                stringList.Add("Stats");
              if (importUbiSaveResult.failedScores)
                stringList.Add("Scores");
              if (importUbiSaveResult.failedFog)
                stringList.Add("Fog");
              if (importUbiSaveResult.failedAchievements)
                stringList.Add("Achievements");
              if (stringList.Count > 0)
              {
                string str = "";
                for (int index = 0; index < stringList.Count; ++index)
                {
                  str = stringList[index];
                  if (index < stringList.Count - 1)
                    str += ", ";
                }
                string textHeader = this.launcher.loc.Get("alert");
                string textDesc = this.launcher.loc.Get("save_import_ubi_semifailed", new string[1]
                {
                  str
                });
                Bitmap icDboxWarning = Resources.ic_dbox_warning;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index6 = 0;
                span[index6] = new ButtonAction(this.launcher.loc.Get("ok"));
                num31 = index6 + 1;
                int num32 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList).ShowDialog((IWin32Window) this);
              }
              else
              {
                string textHeader = this.launcher.loc.Get("is_success");
                string textDesc = this.launcher.loc.Get("save_import_ubi_success");
                Bitmap check = Resources.check;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                num31 = index + 1;
                int num33 = (int) new ConfirmForm(textHeader, textDesc, (Image) check, buttonActionList).ShowDialog((IWin32Window) this);
              }
              save.SaveToFile((string) null);
              this.SaveSelect.SelectedIndex = this.SaveSelect.Items.Add((object) savegameName);
            }
            else
            {
              string textHeader = this.launcher.loc.Get("is_error");
              string textDesc = this.launcher.loc.Get("save_import_ubi_failed");
              Bitmap icRefuse = Resources.ic_refuse_;
              List<ButtonAction> buttonActionList = new List<ButtonAction>();
              CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
              Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
              int index = 0;
              span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
              num31 = index + 1;
              int num34 = (int) new ConfirmForm(textHeader, textDesc, (Image) icRefuse, buttonActionList).ShowDialog((IWin32Window) this);
            }
          }));
          if (saveImportForm.IsDisposed)
            return;
          int num = (int) saveImportForm.ShowDialog((IWin32Window) this);
        });
        bool acceptedWarning = true;
        if (!this.launcher.userdata.IsWarnedAboutDumpedSaves())
        {
          acceptedWarning = false;
          string textHeader = this.launcher.loc.Get("info");
          string textDesc = this.launcher.loc.Get("save_import_ubi_notice", new string[1]
          {
            LauncherGlobals.DUMPED_SAVEGAME_DEFAULT_NAME
          });
          Bitmap iconFolder = Resources.icon_folder;
          List<ButtonAction> buttonActionList = new List<ButtonAction>();
          CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
          Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
          int index = 0;
          span[index] = new ButtonAction(this.launcher.loc.Get("ok"), (Action) (() => acceptedWarning = true));
          int num35 = index + 1;
          int num36 = (int) new ConfirmForm(textHeader, textDesc, (Image) iconFolder, buttonActionList).ShowDialog((IWin32Window) this);
        }
        if (!acceptedWarning)
          return;
        action();
      }
    });
    this.BtnEditSave.Click += (EventHandler) ((sender, e) =>
    {
      if (!LauncherUtils.CanUseSaveFile(this.launcher, this.GameInstance))
      {
        this.launcher.ShowSaveInUseWarningForm();
      }
      else
      {
        SaveInstance save = this.GameInstance?.GetSaveInstance();
        if (save == null)
          return;
        string selectedSave = this.SaveSelect.Text;
        if (selectedSave.Length <= 0 || !save.PlayerNameExists(selectedSave))
          return;
        int num41 = (int) new SaveImportForm(this.launcher, save, selectedSave, SaveImportForm.SaveImportFormMode.RenameSave, (Action<string, string>) ((filepath, newSavegameName) =>
        {
          int num42;
          if (save.PlayerNameExists(newSavegameName))
          {
            string textHeader = this.launcher.loc.Get("alert");
            string textDesc = this.launcher.loc.Get("save_rename_taken", new string[2]
            {
              selectedSave,
              newSavegameName
            });
            Bitmap icDboxWarning = Resources.ic_dbox_warning;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num42 = index + 1;
            int num43 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList).ShowDialog((IWin32Window) this);
          }
          else if (save.RenameSave(selectedSave, newSavegameName))
          {
            string textHeader = this.launcher.loc.Get("is_success");
            string textDesc = this.launcher.loc.Get("save_rename_success", new string[2]
            {
              selectedSave,
              newSavegameName
            });
            Bitmap check = Resources.check;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index9 = 0;
            span[index9] = new ButtonAction(this.launcher.loc.Get("ok"));
            num42 = index9 + 1;
            int num44 = (int) new ConfirmForm(textHeader, textDesc, (Image) check, buttonActionList).ShowDialog((IWin32Window) this);
            int index10 = 0;
            int selectedIndex = this.SaveSelect.SelectedIndex;
            foreach (string str in this.SaveSelect.Items)
            {
              if (str.Equals(selectedSave))
              {
                this.SaveSelect.Items.RemoveAt(index10);
                this.SaveSelect.Items.Insert(index10, (object) newSavegameName);
                if (selectedIndex == index10)
                {
                  this.SaveSelect.SelectedIndex = index10;
                  break;
                }
                break;
              }
              ++index10;
            }
            save.SaveToFile((string) null);
          }
          else
          {
            string textHeader = this.launcher.loc.Get("alert");
            string textDesc = this.launcher.loc.Get("save_rename_failed", new string[2]
            {
              selectedSave,
              newSavegameName
            });
            Bitmap icDboxWarning = Resources.ic_dbox_warning;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num42 = index + 1;
            int num45 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList).ShowDialog((IWin32Window) this);
          }
        })).ShowDialog((IWin32Window) this);
      }
    });
    this.CheatSelect.SelectedIndexChanged += (EventHandler) ((sender, e) =>
    {
      this.changingCheat = true;
      this.CheatInput.Text = "";
      this.changingCheat = false;
      this.tooltip.SetToolTip((Control) this.CheatSelect, (string) null);
      string text = this.CheatSelect.Text;
      Cheat cheat;
      if (text == null || text.Length <= 0 || !this.AvailableCheats.TryGetValue(text, out cheat))
        return;
      if (cheat != null)
      {
        this.tooltip.SetToolTip((Control) this.CheatSelect, cheat.Description);
        string str = "";
        if (cheat.Type != null)
          str = $"{str}[{cheat.Type}] ";
        if (cheat.DefaultValue != null)
          str += cheat.DefaultValue;
        this.CheatInput.PlaceholderText = str;
        if (this.ServerConfig == null || !(this.ServerConfig.GetType() == typeof (LocalServerConfig)))
          return;
        string cheatValue = ((LocalServerConfig) this.ServerConfig).GetCheatValue(text);
        if (cheatValue == null)
          return;
        this.CheatInput.Text = cheatValue;
      }
      else
        this.CheatInput.PlaceholderText = "";
    });
    this.CheatInput.KeyPress += (KeyPressEventHandler) ((sender, e) =>
    {
      if (char.IsControl(e.KeyChar))
      {
        e.Handled = false;
      }
      else
      {
        string text = this.CheatSelect.Text;
        Cheat cheat;
        if (text == null || text.Length <= 0 || !this.AvailableCheats.TryGetValue(text, out cheat) || cheat == null)
          return;
        string str = (string) null;
        switch (cheat.Type)
        {
          case "int":
            str = LocalSaveMgrForm.ALLOWED_CHARS_INPUT_INT;
            break;
          case "float":
            str = LocalSaveMgrForm.ALLOWED_CHARS_INPUT_FLOAT;
            break;
          case "bool":
            str = LocalSaveMgrForm.ALLOWED_CHARS_INPUT_BOOL;
            break;
        }
        if (str != null)
        {
          if (str.Contains(e.KeyChar.ToString()))
            e.Handled = false;
          else
            e.Handled = true;
        }
        else
          e.Handled = false;
      }
    });
    this.CheatInput.TextChanged += (EventHandler) ((sender, e) =>
    {
      if (this.changingCheat || this.ServerConfig == null || !(this.ServerConfig.GetType() == typeof (LocalServerConfig)))
        return;
      ((LocalServerConfig) this.ServerConfig).SetCheatValue(this.CheatSelect.Text, this.CheatInput.Text);
    });
    this.BtnApplyCheats.Click += (EventHandler) ((sender, e) =>
    {
      if (this.GameInstance == null || this.ServerConfig == null)
        return;
      this.launcher.WriteServerIni(this.GameInstance, this.ServerConfig);
    });
  }

  public GameInstance? GetGameInstance() => this.GameInstance;

  public ServerConfig? GetServerConfig() => this.ServerConfig;

  public void ResetGameAndServerInstances()
  {
    this.GameInstance = (GameInstance) null;
    this.ServerInstance = (ServerInstance) null;
    this.ServerConfig = (ServerConfig) null;
    this.SaveSelect.Text = (string) null;
    this.SaveSelect.Items.Clear();
    this.CheatSelect.Items.Clear();
    this.AvailableCheats.Clear();
  }

  protected void SelectFirstAvailableSave(GameInstance GameInstance, ServerConfig ServerConfig)
  {
    if (!LauncherUtils.SelectFirstAvailableSave(GameInstance, ServerConfig))
      return;
    this.SaveSelect.SelectedIndex = 0;
  }

  public LocalSaveMgrForm SetGameAndServerInstances(
    GameInstance? GameInstance,
    ServerInstance? ServerInstance)
  {
    this.ResetGameAndServerInstances();
    this.GameInstance = GameInstance;
    this.ServerInstance = ServerInstance;
    this.ServerConfig = ServerInstance?.GetServerConfig();
    if (ServerInstance == null || this.ServerConfig == null || GameInstance == null || GameInstance.GetSaveInstance() == null)
      throw new ArgumentNullException("Something went wrong, GameInstance/ServerInstance/ServerConfig is null here.");
    SaveInstance saveInstance = GameInstance.GetSaveInstance();
    if (saveInstance != null)
    {
      List<string> playerNames = saveInstance.GetPlayerNames();
      if (playerNames.Count > 0)
      {
        foreach (object obj in playerNames)
          this.SaveSelect.Items.Add(obj);
        if (!saveInstance.PlayerNameExists(this.ServerConfig.GetUsername()))
          this.SelectFirstAvailableSave(GameInstance, this.ServerConfig);
      }
      else
      {
        string username = this.ServerConfig.GetUsername();
        saveInstance.AddNewSave(username);
        this.SaveSelect.Items.Add((object) username);
        this.SelectFirstAvailableSave(GameInstance, this.ServerConfig);
      }
    }
    LauncherUtils.ReadServerIni(this.launcher.userdata, GameInstance, this.ServerConfig);
    bool flag = false;
    int num = 0;
    foreach (string str in this.SaveSelect.Items)
    {
      if (str.Equals(this.ServerConfig.GetUsername()))
      {
        this.SaveSelect.SelectedIndex = num;
        flag = true;
        break;
      }
      ++num;
    }
    if (!flag)
      this.SelectFirstAvailableSave(GameInstance, this.ServerConfig);
    if (this.launcher.userdata.IsOfflineCheatsEnabled())
    {
      this.CheatSelect.Enabled = true;
      this.CheatInput.Enabled = true;
      string str = $"{Path.GetDirectoryName(GameInstance.GetDirectoryPath())}\\{LauncherGlobals.CHEAT_MANIFEST}";
      if (File.Exists(str))
      {
        List<Cheat> cheatList = Cheats.ReadCheatManifestXML(str);
        if (cheatList.Count > 0)
        {
          foreach (Cheat cheat in cheatList)
          {
            this.AvailableCheats[cheat.Name] = cheat;
            this.CheatSelect.Items.Add((object) cheat.Name);
          }
          this.CheatSelect.SelectedItem = (object) cheatList[0].Name;
        }
      }
    }
    else
    {
      this.CheatSelect.Enabled = false;
      this.CheatSelect.Items.Add((object) this.launcher.loc.Get("server_cheats_disabled"));
      this.CheatSelect.SelectedIndex = 0;
      this.CheatInput.Enabled = false;
      this.CheatInput.PlaceholderText = "";
    }
    return this;
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
    this.LabelSave = new Label();
    this.SaveSelect = new ComboBox();
    this.BtnAddSave = new Button();
    this.tooltip = new ToolTip(this.components);
    this.BtnRemoveSave = new Button();
    this.BtnImportSave = new Button();
    this.BtnExportSave = new Button();
    this.BtnImportDumpedSave = new Button();
    this.CheatSelect = new ComboBox();
    this.LabelCheats = new Label();
    this.CheatInput = new TextBox();
    this.openFileDialog = new OpenFileDialog();
    this.saveFileDialog = new SaveFileDialog();
    this.BtnEditSave = new Button();
    this.BtnApplyCheats = new Button();
    this.SuspendLayout();
    this.LabelSave.AutoSize = true;
    this.LabelSave.BackColor = Color.Transparent;
    this.LabelSave.Font = new Font("TheCrew Sans Regular", 14.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.LabelSave.ForeColor = Color.White;
    this.LabelSave.Location = new Point(12, 9);
    this.LabelSave.Name = "LabelSave";
    this.LabelSave.Size = new Size(84, 29);
    this.LabelSave.TabIndex = 0;
    this.LabelSave.Text = "Active Save";
    this.LabelSave.UseCompatibleTextRendering = true;
    this.SaveSelect.DropDownStyle = ComboBoxStyle.DropDownList;
    this.SaveSelect.FormattingEnabled = true;
    this.SaveSelect.Location = new Point(12, 40);
    this.SaveSelect.Name = "SaveSelect";
    this.SaveSelect.Size = new Size(437, 23);
    this.SaveSelect.TabIndex = 1;
    this.BtnAddSave.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnAddSave.FlatStyle = FlatStyle.Flat;
    this.BtnAddSave.Font = new Font("TheCrew Sans Bold", 23.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.BtnAddSave.ForeColor = Color.Black;
    this.BtnAddSave.Location = new Point(485, 40);
    this.BtnAddSave.Name = "BtnAddSave";
    this.BtnAddSave.Size = new Size(23, 23);
    this.BtnAddSave.TabIndex = 2;
    this.BtnAddSave.Text = "+";
    this.BtnAddSave.UseCompatibleTextRendering = true;
    this.BtnAddSave.UseVisualStyleBackColor = false;
    this.tooltip.AutoPopDelay = 30000;
    this.tooltip.InitialDelay = 500;
    this.tooltip.IsBalloon = true;
    this.tooltip.ReshowDelay = 100;
    this.tooltip.ShowAlways = true;
    this.BtnRemoveSave.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnRemoveSave.FlatStyle = FlatStyle.Flat;
    this.BtnRemoveSave.Font = new Font("TheCrew Sans Bold", 23.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.BtnRemoveSave.ForeColor = Color.Black;
    this.BtnRemoveSave.Location = new Point(514, 40);
    this.BtnRemoveSave.Name = "BtnRemoveSave";
    this.BtnRemoveSave.Size = new Size(23, 23);
    this.BtnRemoveSave.TabIndex = 3;
    this.BtnRemoveSave.Text = "-";
    this.BtnRemoveSave.UseCompatibleTextRendering = true;
    this.BtnRemoveSave.UseVisualStyleBackColor = false;
    this.BtnImportSave.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnImportSave.FlatStyle = FlatStyle.Flat;
    this.BtnImportSave.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.BtnImportSave.ForeColor = Color.Black;
    this.BtnImportSave.Location = new Point(11, 69);
    this.BtnImportSave.Name = "BtnImportSave";
    this.BtnImportSave.Size = new Size(141, 30);
    this.BtnImportSave.TabIndex = 4;
    this.BtnImportSave.Text = "Import Save";
    this.BtnImportSave.UseCompatibleTextRendering = true;
    this.BtnImportSave.UseVisualStyleBackColor = false;
    this.BtnExportSave.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    this.BtnExportSave.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnExportSave.FlatStyle = FlatStyle.Flat;
    this.BtnExportSave.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.BtnExportSave.ForeColor = Color.Black;
    this.BtnExportSave.Location = new Point(160 /*0xA0*/, 69);
    this.BtnExportSave.Name = "BtnExportSave";
    this.BtnExportSave.Size = new Size(141, 30);
    this.BtnExportSave.TabIndex = 5;
    this.BtnExportSave.Text = "Export Save";
    this.BtnExportSave.UseCompatibleTextRendering = true;
    this.BtnExportSave.UseVisualStyleBackColor = false;
    this.BtnImportDumpedSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
    this.BtnImportDumpedSave.AutoEllipsis = true;
    this.BtnImportDumpedSave.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnImportDumpedSave.FlatStyle = FlatStyle.Flat;
    this.BtnImportDumpedSave.Font = new Font("TheCrew Sans Bold", 14.25f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.BtnImportDumpedSave.ForeColor = Color.Black;
    this.BtnImportDumpedSave.Location = new Point(309, 69);
    this.BtnImportDumpedSave.Name = "BtnImportDumpedSave";
    this.BtnImportDumpedSave.Size = new Size(141, 30);
    this.BtnImportDumpedSave.TabIndex = 6;
    this.BtnImportDumpedSave.Text = "Import UBI Save";
    this.BtnImportDumpedSave.UseCompatibleTextRendering = true;
    this.BtnImportDumpedSave.UseVisualStyleBackColor = false;
    this.CheatSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.CheatSelect.DropDownStyle = ComboBoxStyle.DropDownList;
    this.CheatSelect.FormattingEnabled = true;
    this.CheatSelect.Location = new Point(12, 147);
    this.CheatSelect.Name = "CheatSelect";
    this.CheatSelect.Size = new Size(404, 23);
    this.CheatSelect.TabIndex = 8;
    this.LabelCheats.AutoSize = true;
    this.LabelCheats.BackColor = Color.Transparent;
    this.LabelCheats.Font = new Font("TheCrew Sans Regular", 14.25f, FontStyle.Regular, GraphicsUnit.Point, (byte) 0);
    this.LabelCheats.ForeColor = Color.White;
    this.LabelCheats.Location = new Point(12, 118);
    this.LabelCheats.Name = "LabelCheats";
    this.LabelCheats.Size = new Size(53, 29);
    this.LabelCheats.TabIndex = 7;
    this.LabelCheats.Text = "Cheats";
    this.LabelCheats.UseCompatibleTextRendering = true;
    this.CheatInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.CheatInput.Location = new Point(422, 147);
    this.CheatInput.Name = "CheatInput";
    this.CheatInput.PlaceholderText = "[Default Value]";
    this.CheatInput.Size = new Size(115, 23);
    this.CheatInput.TabIndex = 9;
    this.openFileDialog.DefaultExt = "tcusave";
    this.openFileDialog.Filter = "TCU Savegame files|*.tcusave";
    this.openFileDialog.Title = "Select an exported savegame";
    this.BtnEditSave.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnEditSave.BackgroundImage = (Image) Resources.icon_edit_save;
    this.BtnEditSave.BackgroundImageLayout = ImageLayout.Stretch;
    this.BtnEditSave.FlatStyle = FlatStyle.Flat;
    this.BtnEditSave.Font = new Font("Microsoft Sans Serif", 23f, FontStyle.Bold);
    this.BtnEditSave.ForeColor = Color.Black;
    this.BtnEditSave.Location = new Point(456, 40);
    this.BtnEditSave.Name = "BtnEditSave";
    this.BtnEditSave.Size = new Size(23, 23);
    this.BtnEditSave.TabIndex = 10;
    this.BtnEditSave.UseCompatibleTextRendering = true;
    this.BtnEditSave.UseVisualStyleBackColor = false;
    this.BtnApplyCheats.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
    this.BtnApplyCheats.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnApplyCheats.FlatStyle = FlatStyle.Flat;
    this.BtnApplyCheats.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold, GraphicsUnit.Point, (byte) 0);
    this.BtnApplyCheats.ForeColor = Color.Black;
    this.BtnApplyCheats.Location = new Point(11, 177);
    this.BtnApplyCheats.Name = "BtnApplyCheats";
    this.BtnApplyCheats.Size = new Size(115, 30);
    this.BtnApplyCheats.TabIndex = 11;
    this.BtnApplyCheats.Text = "Apply";
    this.BtnApplyCheats.UseCompatibleTextRendering = true;
    this.BtnApplyCheats.UseVisualStyleBackColor = false;
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackColor = Color.FromArgb(14, 14, 14);
    this.ClientSize = new Size(549, 216);
    this.Controls.Add((Control) this.BtnApplyCheats);
    this.Controls.Add((Control) this.BtnEditSave);
    this.Controls.Add((Control) this.CheatInput);
    this.Controls.Add((Control) this.CheatSelect);
    this.Controls.Add((Control) this.LabelCheats);
    this.Controls.Add((Control) this.BtnImportDumpedSave);
    this.Controls.Add((Control) this.BtnExportSave);
    this.Controls.Add((Control) this.BtnImportSave);
    this.Controls.Add((Control) this.BtnRemoveSave);
    this.Controls.Add((Control) this.BtnAddSave);
    this.Controls.Add((Control) this.SaveSelect);
    this.Controls.Add((Control) this.LabelSave);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (LocalSaveMgrForm);
    this.StartPosition = FormStartPosition.CenterParent;
    this.Text = "Local Server Manager";
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
