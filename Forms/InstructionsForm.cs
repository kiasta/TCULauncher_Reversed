using System.ComponentModel;

#nullable enable
namespace TCULauncher.Forms;

public class InstructionsForm : Form
{
  private 
  #nullable disable
  FontManager fontManager = new FontManager().SetupDefaultFonts();
  private IContainer components;
  private Label LabelInstructions;

  public InstructionsForm(
  #nullable enable
  Launcher launcher)
  {
    this.InitializeComponent();
    this.SetupStuff(launcher);
  }

  protected void SetupStuff(Launcher launcher)
  {
    this.Text = launcher.loc.Get("title_how_to_use");
    this.LabelInstructions.Text = launcher.loc.Get("launcher_instructions", new string[2]
    {
      launcher.loc.Get("instance_patch"),
      launcher.loc.Get("play")
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
    ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof (InstructionsForm));
    this.LabelInstructions = new Label();
    this.SuspendLayout();
    this.LabelInstructions.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    this.LabelInstructions.BackColor = Color.Transparent;
    this.LabelInstructions.Font = new Font("Segoe UI", 9f);
    this.LabelInstructions.ForeColor = Color.White;
    this.LabelInstructions.Location = new Point(12, 9);
    this.LabelInstructions.Name = "LabelInstructions";
    this.LabelInstructions.Size = new Size(542, 227);
    this.LabelInstructions.TabIndex = 1;
    this.LabelInstructions.Text = componentResourceManager.GetString("LabelInstructions.Text");
    this.LabelInstructions.UseCompatibleTextRendering = true;
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackColor = Color.FromArgb(14, 14, 14);
    this.ClientSize = new Size(566, 245);
    this.Controls.Add((Control) this.LabelInstructions);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (InstructionsForm);
    this.StartPosition = FormStartPosition.CenterParent;
    this.Text = nameof (InstructionsForm);
    this.ResumeLayout(false);
  }
}
