using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher;

public class FontManager
{
  public static readonly FontEntry[] FONTS = new FontEntry[4]
  {
    new FontEntry("TheCrew Sans Regular", Resources.TheCrewSans_Regular_edit),
    new FontEntry("TheCrew Sans Bold", Resources.TheCrewSans_Bold_edit),
    new FontEntry("TheCrew Street Regular", Resources.TheCrewStreet_Regular_edit),
    new FontEntry("TheCrew Script Regular", Resources.thecrewscript_regular_webfont)
  };
  public PrivateFontCollection Fonts;
  private Dictionary<string, FontData> FontDatas;

  public FontManager()
  {
    this.Fonts = new PrivateFontCollection();
    this.FontDatas = new Dictionary<string, FontData>();
  }

  public Font? GetFont(string fontName, float size, FontStyle style)
  {
    FontData fontData;
    return this.FontDatas.TryGetValue(fontName, out fontData) ? fontData.GetFont(this, size, style) : (Font) null;
  }

  public void SetupFont(string fontName, byte[] fontdata)
  {
    int length = fontdata.Length;
    IntPtr num = Marshal.AllocCoTaskMem(length);
    Marshal.Copy(fontdata, 0, num, length);
    uint pcFonts = 0;
    FontManager.AddFontMemResourceEx(num, (uint) fontdata.Length, IntPtr.Zero, ref pcFonts);
    this.Fonts.AddMemoryFont(num, length);
    FontData fontData = new FontData(fontName, num);
    this.FontDatas.Add(fontName, fontData);
  }

  public FontManager SetupDefaultFonts()
  {
    foreach (FontEntry fontEntry in FontManager.FONTS)
      this.SetupFont(fontEntry.FontName, fontEntry.Data);
    return this;
  }

  public void Dispose()
  {
    this.Fonts.Dispose();
    foreach (string key in this.FontDatas.Keys)
      Marshal.FreeCoTaskMem(this.FontDatas[key].memData);
    this.FontDatas.Clear();
  }

  public void OverrideFonts(Control control, bool? setCompatibleTextRendering)
  {
    string originalFontName = control.Font.OriginalFontName;
    if (originalFontName != null)
    {
      Font font = this.GetFont(originalFontName, control.Font.Size, control.Font.Style);
      if (font != null)
        control.Font = font;
    }
    bool? nullable = setCompatibleTextRendering;
    bool flag = true;
    if (nullable.GetValueOrDefault() == flag & nullable.HasValue && control.GetType() == typeof (Label))
      ((Label) control).UseCompatibleTextRendering = setCompatibleTextRendering.Value;
    foreach (Control control1 in (ArrangedElementCollection) control.Controls)
      this.OverrideFonts(control1, setCompatibleTextRendering);
  }

  [DllImport("gdi32.dll")]
  private static extern IntPtr AddFontMemResourceEx(
    IntPtr pbFont,
    uint cbFont,
    IntPtr pdv,
    [In] ref uint pcFonts);
}
