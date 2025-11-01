using System.ComponentModel;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher.Forms;

public class StartupInfoForm : Form
{
  private readonly Launcher launcher;
  private 
  #nullable disable
  FontManager fontManager = new FontManager().SetupDefaultFonts();
  private IContainer components;
  private Label LabelHeader;
  private Label LabelDesc;
  private Button button1;

  public StartupInfoForm(
  #nullable enable
  Launcher launcher)
  {
    this.launcher = launcher;
    this.InitializeComponent();
    this.SetupStuff();
  }

  protected void SetupStuff()
  {
    this.LabelHeader.Text = this.launcher.loc.Get("tcunet_connecting");
    this.LabelDesc.Text = this.launcher.loc.Get("tcunet_connecting_hold");
    this.Text = this.launcher.loc.Get("launcher_starting");
    this.StartPosition = FormStartPosition.CenterScreen;
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
    this.LabelHeader = new Label();
    this.LabelDesc = new Label();
    this.button1 = new Button();
    this.SuspendLayout();
    this.LabelHeader.AutoSize = true;
    this.LabelHeader.Font = new Font("TheCrew Sans Bold", 24f);
    this.LabelHeader.ForeColor = Color.White;
    this.LabelHeader.Location = new Point(79, 9);
    this.LabelHeader.Name = "LabelHeader";
    this.LabelHeader.Size = new Size(275, 46);
    this.LabelHeader.TabIndex = 0;
    this.LabelHeader.Text = "Connecting to TCU Net...";
    this.LabelHeader.TextAlign = ContentAlignment.MiddleCenter;
    this.LabelHeader.UseCompatibleTextRendering = true;
    this.LabelDesc.AutoSize = true;
    this.LabelDesc.Font = new Font("TheCrew Sans Regular", 16f);
    this.LabelDesc.ForeColor = Color.White;
    this.LabelDesc.Location = new Point(97, 55);
    this.LabelDesc.Name = "LabelDesc";
    this.LabelDesc.Size = new Size(125, 32 /*0x20*/);
    this.LabelDesc.TabIndex = 1;
    this.LabelDesc.Text = "Please wait...";
    this.LabelDesc.TextAlign = ContentAlignment.MiddleCenter;
    this.LabelDesc.UseCompatibleTextRendering = true;
    this.button1.BackColor = Color.Transparent;
    this.button1.BackgroundImage = (Image) Resources.datatowerlibhr;
    this.button1.BackgroundImageLayout = ImageLayout.Zoom;
    this.button1.Enabled = false;
    this.button1.FlatStyle = FlatStyle.Flat;
    this.button1.ForeColor = Color.Transparent;
    this.button1.Location = new Point(10, 9);
    this.button1.Name = "button1";
    this.button1.Size = new Size(72, 78);
    this.button1.TabIndex = 2;
    this.button1.UseVisualStyleBackColor = false;
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackColor = Color.Black;
    this.ClientSize = new Size(405, 107);
    this.Controls.Add((Control) this.button1);
    this.Controls.Add((Control) this.LabelHeader);
    this.Controls.Add((Control) this.LabelDesc);
    this.FormBorderStyle = FormBorderStyle.None;
    this.Name = nameof (StartupInfoForm);
    this.ShowIcon = false;
    this.StartPosition = FormStartPosition.CenterScreen;
    this.Text = "Starting TCU Launcher...";
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}
