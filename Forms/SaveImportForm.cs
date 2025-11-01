using System.ComponentModel;
using System.Runtime.InteropServices;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher.Forms;

public class SaveImportForm : Form
{
  private readonly Launcher launcher;
  private readonly SaveInstance save;
  public readonly SaveImportForm.SaveImportFormMode mode;
  private readonly Action<string, string>? onComplete;
  private string? selectedSaveFile;
  private 
  #nullable disable
  FontManager fontManager = new FontManager().SetupDefaultFonts();
  private IContainer components;
  private TextBox SaveNameInput;
  private Label LabelSaveName;
  private Label LabelFilePath;
  private Button BtnImportSave;
  private Button BtnSelectFile;
  private OpenFileDialog openFileDialog;

  public SaveImportForm(
    #nullable enable
    Launcher launcher,
    SaveInstance save,
    string? startInput,
    SaveImportForm.SaveImportFormMode mode,
    Action<string, string>? onComplete)
  {
    this.launcher = launcher;
    this.save = save;
    this.mode = mode;
    this.onComplete = onComplete;
    this.InitializeComponent();
    this.SetupStuff(startInput);
  }

  public void SetupStuff(string? startInput)
  {
    if (this.mode == SaveImportForm.SaveImportFormMode.AddNewSave)
    {
      this.Text = this.launcher.loc.Get("title_save_create");
      this.SaveNameInput.PlaceholderText = this.launcher.loc.Get("save_enter_name");
      this.LabelSaveName.Text = this.launcher.loc.Get("save_name");
      this.BtnImportSave.Text = this.launcher.loc.Get("create");
      this.LabelFilePath.Visible = false;
      this.BtnSelectFile.Visible = false;
    }
    else if (this.mode == SaveImportForm.SaveImportFormMode.RenameSave)
    {
      this.Text = this.launcher.loc.Get("title_save_rename");
      this.SaveNameInput.PlaceholderText = this.launcher.loc.Get("title_save_enter_new_name");
      this.LabelSaveName.Text = this.launcher.loc.Get("save_name");
      this.BtnImportSave.Text = this.launcher.loc.Get("rename");
      this.LabelFilePath.Visible = false;
      this.BtnSelectFile.Visible = false;
    }
    else if (this.mode == SaveImportForm.SaveImportFormMode.ImportUbiSaveDump)
    {
      this.Text = this.launcher.loc.Get("title_save_import_ubi");
      this.LabelFilePath.Text = this.launcher.loc.Get("save_fileselect_none");
      this.SaveNameInput.PlaceholderText = this.launcher.loc.Get("save_enter_name");
      this.LabelSaveName.Text = this.launcher.loc.Get("save_name");
      this.BtnImportSave.Text = this.launcher.loc.Get("import");
      this.LabelFilePath.Visible = true;
      this.BtnSelectFile.Visible = true;
    }
    else if (this.mode == SaveImportForm.SaveImportFormMode.ImportSave)
    {
      this.Text = this.launcher.loc.Get("title_save_import");
      this.LabelFilePath.Text = this.launcher.loc.Get("save_fileselect_none");
      this.SaveNameInput.PlaceholderText = this.launcher.loc.Get("save_enter_name");
      this.LabelSaveName.Text = this.launcher.loc.Get("save_name");
      this.BtnImportSave.Text = this.launcher.loc.Get("import");
      this.LabelFilePath.Visible = true;
      this.BtnSelectFile.Visible = true;
    }
    Action updateButtonEnable = (Action) (() =>
    {
      string str = LauncherUtils.RemoveSpaces(this.SaveNameInput.Text, true, true);
      if (this.mode == SaveImportForm.SaveImportFormMode.RenameSave && str.Equals(startInput))
        this.BtnImportSave.Enabled = false;
      else if (str.Length > 0)
        this.BtnImportSave.Enabled = str.Length >= LauncherGlobals.SAVEGAME_NAME_CHARS_MIN && str.Length <= LauncherGlobals.SAVEGAME_NAME_CHARS_MAX && (this.selectedSaveFile != null || !this.mode.needsFile);
      else
        this.BtnImportSave.Enabled = false;
    });
    if (startInput != null)
    {
      this.SaveNameInput.Text = startInput;
      updateButtonEnable();
    }
    bool addNewSave = this.mode == SaveImportForm.SaveImportFormMode.AddNewSave;
    bool ubiSaveDump = this.mode == SaveImportForm.SaveImportFormMode.ImportUbiSaveDump;
    this.openFileDialog.Title = ubiSaveDump ? this.launcher.loc.Get("title_save_import_ubi_select") : this.launcher.loc.Get("title_save_import_select");
    this.openFileDialog.Multiselect = false;
    this.openFileDialog.FileName = "";
    if (ubiSaveDump)
    {
      this.openFileDialog.DefaultExt = LauncherGlobals.DUMPED_SAVEGAME_EXT;
      this.openFileDialog.Filter = "Dumped ubisoft savegame file|*." + LauncherGlobals.DUMPED_SAVEGAME_EXT;
    }
    else
    {
      this.openFileDialog.DefaultExt = LauncherGlobals.SAVEGAME_EXT;
      this.openFileDialog.Filter = "TCU Savegame files|*." + LauncherGlobals.SAVEGAME_EXT;
    }
    this.openFileDialog.FileOk += (CancelEventHandler) ((sender, e) =>
    {
      string fileName = this.openFileDialog.FileName;
      if (fileName.EndsWith(ubiSaveDump ? LauncherGlobals.DUMPED_SAVEGAME_EXT : LauncherGlobals.SAVEGAME_EXT))
      {
        this.selectedSaveFile = fileName;
        this.LabelFilePath.Text = "File: " + fileName;
        if (addNewSave || ubiSaveDump)
          return;
        string str = this.save.ReadPlayerNameFromJson(File.ReadAllText(this.selectedSaveFile));
        if (str == null)
          return;
        this.SaveNameInput.Text = str;
      }
      else
      {
        e.Cancel = true;
        this.selectedSaveFile = (string) null;
        this.LabelFilePath.Text = this.launcher.loc.Get("save_fileselect_none");
      }
    });
    this.BtnImportSave.Enabled = false;
    this.SaveNameInput.KeyPress += (KeyPressEventHandler) ((sender, e) =>
    {
      if (char.IsControl(e.KeyChar))
        return;
      foreach (int num in LauncherGlobals.SAVEGAME_NAME_CHARS_WHITELIST.ToCharArray())
      {
        if ((int) e.KeyChar == num)
          return;
      }
      e.Handled = false;
      e.KeyChar = char.MinValue;
    });
    this.SaveNameInput.TextChanged += (EventHandler) ((sender, e) => this.SaveNameInput.Invoke(updateButtonEnable));
    this.BtnImportSave.Click += (EventHandler) ((sender, e) =>
    {
      if (!this.mode.needsFile)
        this.selectedSaveFile = "";
      if (this.selectedSaveFile == null)
        return;
      string playerName = LauncherUtils.RemoveSpaces(this.SaveNameInput.Text, true, true);
      if (playerName.Length <= 0)
        return;
      int num1;
      if (playerName.Length >= LauncherGlobals.SAVEGAME_NAME_CHARS_MIN)
      {
        if (playerName.Length <= LauncherGlobals.SAVEGAME_NAME_CHARS_MAX)
        {
          if (!this.save.PlayerNameExists(playerName))
          {
            if (this.onComplete == null)
              return;
            this.Close();
            this.onComplete(this.selectedSaveFile, playerName);
          }
          else
          {
            string textHeader = this.launcher.loc.Get("alert");
            string textDesc = this.launcher.loc.Get("save_savename_exists", new string[1]
            {
              playerName
            });
            Bitmap icDboxWarning = Resources.ic_dbox_warning;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num1 = index + 1;
            int num2 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList).ShowDialog((IWin32Window) this);
          }
        }
        else
        {
          string textHeader = this.launcher.loc.Get("alert");
          string textDesc = this.launcher.loc.Get("save_savename_long", new string[2]
          {
            playerName,
            LauncherGlobals.SAVEGAME_NAME_CHARS_MAX.ToString()
          });
          Bitmap icDboxWarning = Resources.ic_dbox_warning;
          List<ButtonAction> buttonActionList = new List<ButtonAction>();
          CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
          Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
          int index = 0;
          span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
          num1 = index + 1;
          int num3 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList).ShowDialog((IWin32Window) this);
        }
      }
      else
      {
        string textHeader = this.launcher.loc.Get("alert");
        string textDesc = this.launcher.loc.Get("save_savename_short", new string[2]
        {
          playerName,
          LauncherGlobals.SAVEGAME_NAME_CHARS_MAX.ToString()
        });
        Bitmap icDboxWarning = Resources.ic_dbox_warning;
        List<ButtonAction> buttonActionList = new List<ButtonAction>();
        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
        int index = 0;
        span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
        num1 = index + 1;
        int num4 = (int) new ConfirmForm(textHeader, textDesc, (Image) icDboxWarning, buttonActionList).ShowDialog((IWin32Window) this);
      }
    });
    int num5;
    this.BtnSelectFile.Click += (EventHandler) ((sender, e) => num5 = (int) this.openFileDialog.ShowDialog((IWin32Window) this));
    if (this.mode.needsFile && this.openFileDialog.ShowDialog((IWin32Window) this) != DialogResult.OK)
      this.Close();
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
    this.SaveNameInput = new TextBox();
    this.LabelSaveName = new Label();
    this.LabelFilePath = new Label();
    this.BtnImportSave = new Button();
    this.BtnSelectFile = new Button();
    this.openFileDialog = new OpenFileDialog();
    this.SuspendLayout();
    this.SaveNameInput.Location = new Point(12, 61);
    this.SaveNameInput.Name = "SaveNameInput";
    this.SaveNameInput.PlaceholderText = "Enter save name";
    this.SaveNameInput.Size = new Size(435, 23);
    this.SaveNameInput.TabIndex = 0;
    this.LabelSaveName.AutoSize = true;
    this.LabelSaveName.BackColor = Color.Transparent;
    this.LabelSaveName.Font = new Font("TheCrew Sans Regular", 14f);
    this.LabelSaveName.ForeColor = Color.White;
    this.LabelSaveName.Location = new Point(12, 33);
    this.LabelSaveName.Name = "LabelSaveName";
    this.LabelSaveName.Size = new Size(114, 28);
    this.LabelSaveName.TabIndex = 1;
    this.LabelSaveName.Text = "Savegame name";
    this.LabelSaveName.UseCompatibleTextRendering = true;
    this.LabelFilePath.BackColor = Color.Transparent;
    this.LabelFilePath.Font = new Font("TheCrew Sans Regular", 10f);
    this.LabelFilePath.ForeColor = Color.White;
    this.LabelFilePath.Location = new Point(37, 9);
    this.LabelFilePath.Name = "LabelFilePath";
    this.LabelFilePath.Size = new Size(410, 21);
    this.LabelFilePath.TabIndex = 2;
    this.LabelFilePath.Text = "File: ...";
    this.LabelFilePath.TextAlign = ContentAlignment.MiddleLeft;
    this.LabelFilePath.UseCompatibleTextRendering = true;
    this.BtnImportSave.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnImportSave.FlatStyle = FlatStyle.Flat;
    this.BtnImportSave.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold);
    this.BtnImportSave.ForeColor = Color.Black;
    this.BtnImportSave.Location = new Point(12, 90);
    this.BtnImportSave.Name = "BtnImportSave";
    this.BtnImportSave.Size = new Size(150, 30);
    this.BtnImportSave.TabIndex = 5;
    this.BtnImportSave.Text = "Import";
    this.BtnImportSave.UseCompatibleTextRendering = true;
    this.BtnImportSave.UseVisualStyleBackColor = false;
    this.BtnSelectFile.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnSelectFile.BackgroundImage = (Image) Resources.icon_folder;
    this.BtnSelectFile.BackgroundImageLayout = ImageLayout.Zoom;
    this.BtnSelectFile.FlatStyle = FlatStyle.Flat;
    this.BtnSelectFile.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold);
    this.BtnSelectFile.ForeColor = Color.Black;
    this.BtnSelectFile.Location = new Point(12, 9);
    this.BtnSelectFile.Name = "BtnSelectFile";
    this.BtnSelectFile.Size = new Size(21, 21);
    this.BtnSelectFile.TabIndex = 6;
    this.BtnSelectFile.UseCompatibleTextRendering = true;
    this.BtnSelectFile.UseVisualStyleBackColor = false;
    this.openFileDialog.FileName = "openFileDialog1";
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackColor = Color.FromArgb(14, 14, 14);
    this.ClientSize = new Size(459, 131);
    this.Controls.Add((Control) this.BtnSelectFile);
    this.Controls.Add((Control) this.BtnImportSave);
    this.Controls.Add((Control) this.LabelFilePath);
    this.Controls.Add((Control) this.LabelSaveName);
    this.Controls.Add((Control) this.SaveNameInput);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.Name = nameof (SaveImportForm);
    this.StartPosition = FormStartPosition.CenterParent;
    this.Text = "Import dumped ubi savegame";
    this.ResumeLayout(false);
    this.PerformLayout();
  }

  public class SaveImportFormMode
  {
    public static readonly SaveImportForm.SaveImportFormMode AddNewSave = new SaveImportForm.SaveImportFormMode(false);
    public static readonly SaveImportForm.SaveImportFormMode RenameSave = new SaveImportForm.SaveImportFormMode(false);
    public static readonly SaveImportForm.SaveImportFormMode ImportUbiSaveDump = new SaveImportForm.SaveImportFormMode(true);
    public static readonly SaveImportForm.SaveImportFormMode ImportSave = new SaveImportForm.SaveImportFormMode(true);
    public readonly bool needsFile;

    private SaveImportFormMode(bool needsFile) => this.needsFile = needsFile;
  }
}
