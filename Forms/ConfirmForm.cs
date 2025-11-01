using System.ComponentModel;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher.Forms;

public class ConfirmForm : Form
{
  private 
  #nullable disable
  FontManager fontManager = new FontManager().SetupDefaultFonts();
  private IContainer components;
  private Label LabelHeader;
  private Label LabelDesc;
  private PictureBox HeaderIcon;
  private FlowLayoutPanel OptionList;
  private Button SampleButton;
  private Panel DescPanel;

  public ConfirmForm()
  {
    InitializeComponent();
    SetupStuff();
  }

    protected void SetupStuff() => Icon = Launcher.DefaultIcon;

  public ConfirmForm(string textHeader, string textDesc, 
  #nullable enable
  Image? icon, 
  #nullable disable
  List<ButtonAction> buttons)
  {
    InitializeComponent();
    OptionList.Controls.Clear();
    Text = textHeader;
    Icon = Launcher.DefaultIcon;
    LabelHeader.Text = textHeader;
    LabelDesc.Text = textDesc;
    fontManager?.OverrideFonts( this, new bool?(true));
    if (icon != null)
      HeaderIcon.Image = icon;
    foreach (ButtonAction button in buttons)
      OptionList.Controls.Add( createOptionButton(button));
    FlowLayoutPanel optionList = OptionList;
    int x1 = OptionList.Location.X - ((buttons.Count - 1) * SampleButton.Size.Width - (buttons.Count - 1) * (OptionList.Margin.Left + OptionList.Margin.Right)) / 2;
    Point location = OptionList.Location;
    int y1 = location.Y;
    Point point1 = new Point(x1, y1);
    optionList.Location = point1;
    Label labelDesc = LabelDesc;
    int x2 = LabelDesc.MaximumSize.Width / 2 - LabelDesc.Size.Width / 2;
    location = LabelDesc.Location;
    int y2 = location.Y;
    Point point2 = new Point(x2, y2);
    labelDesc.Location = point2;
  }

  private Button createOptionButton(ButtonAction buttonDesc)
  {
    Button optionButton = new Button();
    optionButton.AutoEllipsis = true;
    optionButton.BackColor = Color.FromArgb(5, 173, 241);
    optionButton.FlatAppearance.BorderSize = 0;
    optionButton.FlatStyle = FlatStyle.Flat;
    optionButton.Font = new Font("TheCrew Sans Bold", 15f);
    optionButton.Location = new Point(3, 3);
    optionButton.Name = "SampleButton";
    optionButton.Size = new Size(103, 35);
    optionButton.TabIndex = 0;
    optionButton.Text = "OK";
    optionButton.UseVisualStyleBackColor = false;
    if (buttonDesc != null)
    {
      optionButton.Text = buttonDesc.GetText();
      if (buttonDesc.GetAction() != null)
        optionButton.Click +=  (sender, e) => buttonDesc.GetAction()();
    }
    optionButton.Click +=  (sender, e) => Close();
    fontManager?.OverrideFonts( optionButton, new bool?(true));
    return optionButton;
  }

  protected override void Dispose(bool disposing)
  {
    if (disposing && components != null)
      components.Dispose();
    base.Dispose(disposing);
    fontManager?.Dispose();
  }

  private void InitializeComponent()
  {
    LabelHeader = new Label();
    LabelDesc = new Label();
    HeaderIcon = new PictureBox();
    OptionList = new FlowLayoutPanel();
    SampleButton = new Button();
    DescPanel = new Panel();
    ((ISupportInitialize) HeaderIcon).BeginInit();
    OptionList.SuspendLayout();
    DescPanel.SuspendLayout();
    SuspendLayout();
    LabelHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
    LabelHeader.BackColor = Color.Transparent;
    LabelHeader.Font = new Font("TheCrew Sans Bold", 20f);
    LabelHeader.ForeColor = Color.White;
    LabelHeader.Location = new Point(8, 40);
    LabelHeader.Name = "LabelHeader";
    LabelHeader.Size = new Size(594, 34);
    LabelHeader.TabIndex = 0;
    LabelHeader.Text = "Header Text";
    LabelHeader.TextAlign = ContentAlignment.MiddleCenter;
    LabelDesc.AutoSize = true;
    LabelDesc.BackColor = Color.Transparent;
    LabelDesc.Font = new Font("TheCrew Sans Regular", 13f);
    LabelDesc.ForeColor = Color.White;
    LabelDesc.Location = new Point(0, 0);
    LabelDesc.MaximumSize = new Size(592, 9999);
    LabelDesc.Name = "LabelDesc";
    LabelDesc.Size = new Size(77, 27);
    LabelDesc.TabIndex = 1;
    LabelDesc.Text = "123456789";
    LabelDesc.TextAlign = ContentAlignment.TopCenter;
    LabelDesc.UseCompatibleTextRendering = true;
    HeaderIcon.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        HeaderIcon.Image = (Image) Resources.ic_dbox_warning;
    HeaderIcon.Location = new Point(288, 5);
    HeaderIcon.Name = "HeaderIcon";
    HeaderIcon.Size = new Size(32 /*0x20*/, 32 /*0x20*/);
    HeaderIcon.SizeMode = PictureBoxSizeMode.Zoom;
    HeaderIcon.TabIndex = 2;
    HeaderIcon.TabStop = false;
    OptionList.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    OptionList.AutoSize = true;
    OptionList.AutoSizeMode = AutoSizeMode.GrowAndShrink;
    OptionList.BackColor = Color.Transparent;
    OptionList.Controls.Add( SampleButton);
    OptionList.Font = new Font("TheCrew Sans Bold", 12f);
    OptionList.Location = new Point(248, 129);
    OptionList.MaximumSize = new Size(625, 41);
    OptionList.Name = "OptionList";
    OptionList.Size = new Size(109, 41);
    OptionList.TabIndex = 3;
    OptionList.WrapContents = false;
    SampleButton.AutoEllipsis = true;
    SampleButton.BackColor = Color.FromArgb(5, 173, 241);
    SampleButton.FlatAppearance.BorderSize = 0;
    SampleButton.FlatStyle = FlatStyle.Flat;
    SampleButton.Font = new Font("TheCrew Sans Bold", 15f);
    SampleButton.Location = new Point(3, 3);
    SampleButton.Name = "SampleButton";
    SampleButton.Size = new Size(103, 35);
    SampleButton.TabIndex = 0;
    SampleButton.Text = "OK";
    SampleButton.UseVisualStyleBackColor = false;
    DescPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
    DescPanel.AutoSize = true;
    DescPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
    DescPanel.BackColor = Color.Transparent;
    DescPanel.Controls.Add( LabelDesc);
    DescPanel.Location = new Point(8, 74);
    DescPanel.MaximumSize = new Size(592, 99999);
    DescPanel.Name = "DescPanel";
    DescPanel.Size = new Size(80 /*0x50*/, 27);
    DescPanel.TabIndex = 4;
    AutoScaleDimensions = new SizeF(7f, 15f);
    AutoScaleMode = AutoScaleMode.Font;
    AutoSize = true;
    AutoSizeMode = AutoSizeMode.GrowAndShrink;
    BackColor = Color.FromArgb(14, 14, 14);
    ClientSize = new Size(609, 176 /*0xB0*/);
    Controls.Add( DescPanel);
    Controls.Add( OptionList);
    Controls.Add( HeaderIcon);
    Controls.Add( LabelHeader);
    ForeColor = Color.Black;
    FormBorderStyle = FormBorderStyle.FixedToolWindow;
    MaximizeBox = false;
    MaximumSize = new Size(625, 9999);
    MinimizeBox = false;
    Name = nameof (ConfirmForm);
    StartPosition = FormStartPosition.CenterParent;
    Text = nameof (ConfirmForm);
    TopMost = true;
    ((ISupportInitialize) HeaderIcon).EndInit();
    OptionList.ResumeLayout(false);
    DescPanel.ResumeLayout(false);
    DescPanel.PerformLayout();
    ResumeLayout(false);
    PerformLayout();
  }
}
