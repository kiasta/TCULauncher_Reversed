#nullable enable
namespace TCULauncher;

internal class FontData
{
  public string fontName;
  public IntPtr memData;

  public FontData(string fontName, IntPtr memData)
  {
    this.fontName = fontName;
    this.memData = memData;
  }

  public Font? GetFont(FontManager fontManager, float size, FontStyle style)
  {
    foreach (FontFamily family in fontManager.Fonts.Families)
    {
      if (family.Name.Equals(this.fontName))
        return new Font(family, size, style);
    }
    return (Font) null;
  }
}
