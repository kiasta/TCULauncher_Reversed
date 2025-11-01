using System.ComponentModel;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher.Forms;

public class TextInputForm : Form
{
  private readonly string windowTitle;
  private readonly string labelTitle;
  private readonly string? placeholderText;
  private readonly string? starterText;
  private readonly char? passwordChar;
  private readonly string acceptButtonTitle;
  private readonly int? minLength;
  private readonly int? maxLength;
  private readonly Func<char, bool>? charFilter;
  private readonly Action<string?>? onAccept;
  private 
  #nullable disable
  FontManager fontManager = new FontManager().SetupDefaultFonts();
  private IContainer components;
  private Button BtnAccept;
  private Label LabelTitle;
  private TextBox TextInput;

  public TextInputForm(
    #nullable enable
    string windowTitle,
    string labelTitle,
    string? placeholderText,
    string? starterText,
    char? passwordChar,
    string acceptButtonTitle,
    int? minLength,
    int? maxLength,
    Func<char, bool>? charFilter,
    Action<string?>? onAccept)
  {
    this.InitializeComponent();
    this.windowTitle = windowTitle;
    this.labelTitle = labelTitle;
    this.placeholderText = placeholderText;
    this.starterText = starterText;
    this.passwordChar = passwordChar;
    this.acceptButtonTitle = acceptButtonTitle;
    this.minLength = minLength;
    this.maxLength = maxLength;
    this.charFilter = charFilter;
    this.onAccept = onAccept;
    this.SetupStuff();
  }

  protected void SetupStuff()
  {
    this.Text = this.windowTitle;
    this.LabelTitle.Text = this.labelTitle;
    this.TextInput.PlaceholderText = this.placeholderText;
    this.TextInput.Text = this.starterText;
    if (this.passwordChar.HasValue)
      this.TextInput.PasswordChar = this.passwordChar.Value;
    this.BtnAccept.Text = this.acceptButtonTitle;
    this.Icon = Resources.IconTCU;
    this.TopMost = true;
    this.TextInput.KeyPress += (KeyPressEventHandler) ((sender, e) =>
    {
      if (char.IsControl(e.KeyChar))
        return;
      string text = this.TextInput.Text;
      if (this.maxLength.HasValue)
      {
        int length = text.Length;
        int? maxLength = this.maxLength;
        int valueOrDefault = maxLength.GetValueOrDefault();
        if (!(length < valueOrDefault & maxLength.HasValue))
        {
          e.Handled = false;
          e.KeyChar = char.MinValue;
          return;
        }
      }
      Func<char, bool> charFilter = this.charFilter;
      if ((charFilter != null ? (charFilter(e.KeyChar) ? 1 : 0) : 1) != 0)
        return;
      e.Handled = false;
      e.KeyChar = char.MinValue;
    });
    Func<string, string> stringUpdated = (Func<string, string>) (curStr =>
    {
      int? nullable;
      if (this.maxLength.HasValue)
      {
        int length = curStr.Length;
        nullable = this.maxLength;
        int valueOrDefault = nullable.GetValueOrDefault();
        if (length > valueOrDefault & nullable.HasValue)
          curStr = curStr.Substring(0, this.maxLength.Value);
      }
      this.BtnAccept.Enabled = true;
      if (this.maxLength.HasValue)
      {
        Button btnAccept = this.BtnAccept;
        int num1 = btnAccept.Enabled ? 1 : 0;
        int length = curStr.Length;
        nullable = this.maxLength;
        int valueOrDefault = nullable.GetValueOrDefault();
        int num2 = length <= valueOrDefault & nullable.HasValue ? 1 : 0;
        btnAccept.Enabled = (num1 & num2) != 0;
      }
      if (this.minLength.HasValue)
      {
        Button btnAccept = this.BtnAccept;
        int num3 = btnAccept.Enabled ? 1 : 0;
        int length = curStr.Length;
        nullable = this.minLength;
        int valueOrDefault = nullable.GetValueOrDefault();
        int num4 = length >= valueOrDefault & nullable.HasValue ? 1 : 0;
        btnAccept.Enabled = (num3 & num4) != 0;
      }
      return curStr;
    });
    string str1 = stringUpdated(this.TextInput.Text);
    this.TextInput.TextChanged += (EventHandler) ((sender, e) =>
    {
      string str2 = stringUpdated(this.TextInput.Text);
      if (this.TextInput.Text.Equals(str2))
        return;
      this.TextInput.Text = str2;
    });
    this.BtnAccept.Click += (EventHandler) ((sender, e) =>
    {
      this.Close();
      Action<string> onAccept = this.onAccept;
      if (onAccept == null)
        return;
      onAccept(this.TextInput.Text);
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
    this.BtnAccept = new Button();
    this.LabelTitle = new Label();
    this.TextInput = new TextBox();
    this.SuspendLayout();
    this.BtnAccept.BackColor = Color.FromArgb(5, 173, 241);
    this.BtnAccept.FlatStyle = FlatStyle.Flat;
    this.BtnAccept.Font = new Font("TheCrew Sans Bold", 15f, FontStyle.Bold);
    this.BtnAccept.ForeColor = Color.Black;
    this.BtnAccept.Location = new Point(12, 66);
    this.BtnAccept.Name = "BtnAccept";
    this.BtnAccept.Size = new Size(150, 30);
    this.BtnAccept.TabIndex = 8;
    this.BtnAccept.Text = "Accept";
    this.BtnAccept.UseCompatibleTextRendering = true;
    this.BtnAccept.UseVisualStyleBackColor = false;
    this.LabelTitle.AutoSize = true;
    this.LabelTitle.BackColor = Color.Transparent;
    this.LabelTitle.Font = new Font("TheCrew Sans Regular", 12f);
    this.LabelTitle.ForeColor = Color.White;
    this.LabelTitle.Location = new Point(12, 9);
    this.LabelTitle.Name = "LabelTitle";
    this.LabelTitle.Size = new Size(30, 25);
    this.LabelTitle.TabIndex = 7;
    this.LabelTitle.Text = "Title";
    this.LabelTitle.UseCompatibleTextRendering = true;
    this.TextInput.Location = new Point(12, 37);
    this.TextInput.Name = "TextInput";
    this.TextInput.PlaceholderText = "Placeholder Text";
    this.TextInput.Size = new Size(447, 23);
    this.TextInput.TabIndex = 6;
    this.AutoScaleDimensions = new SizeF(7f, 15f);
    this.AutoScaleMode = AutoScaleMode.Font;
    this.BackColor = Color.FromArgb(14, 14, 14);
    this.ClientSize = new Size(471, 106);
    this.Controls.Add((Control) this.BtnAccept);
    this.Controls.Add((Control) this.LabelTitle);
    this.Controls.Add((Control) this.TextInput);
    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
    this.MaximizeBox = false;
    this.MinimizeBox = false;
    this.Name = nameof (TextInputForm);
    this.StartPosition = FormStartPosition.CenterParent;
    this.Text = nameof (TextInputForm);
    this.ResumeLayout(false);
    this.PerformLayout();
  }
}