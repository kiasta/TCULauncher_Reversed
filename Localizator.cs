#nullable enable
namespace TCULauncher;

public class Localizator
{
  public static readonly string UNLOCALIZED_SYMBOL = "$";
  public static readonly string DEFAULT_STRING = Localizator.UNLOCALIZED_SYMBOL + "UNLOCALIZED";
  private Lang curLang;

  public Localizator(string langFile)
  {
    this.curLang = new Lang();
    this.curLang.LoadFromFile(langFile);
  }

  public Lang GetCurrentLanguage() => this.curLang;

  public bool HasString(string id) => this.curLang.GetString(id, (string[]) null) != null;

  public string Get(string id) => this.Get(id, (string[]) null);

  public string Get(string id, string?[]? args)
  {
    if (id.StartsWith(Localizator.UNLOCALIZED_SYMBOL))
    {
      string str = id;
      int length = Localizator.UNLOCALIZED_SYMBOL.Length;
      id = str.Substring(length, str.Length - length);
    }
    string str1 = this.curLang.GetString(id, args);
    if (str1 != null)
      return str1;
    if (!LauncherGlobals.SHOW_STRING_ID_WHEN_UNLOCALIZED)
      return Localizator.DEFAULT_STRING;
    if (!id.StartsWith(Localizator.UNLOCALIZED_SYMBOL))
      id = Localizator.UNLOCALIZED_SYMBOL + id;
    return id;
  }
}
