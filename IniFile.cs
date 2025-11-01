#nullable enable
namespace TCULauncher;

public class IniFile
{
  public static readonly string INI_SPLITTER = Environment.NewLine;
  public static readonly string INI_COMMENT = "#";
  public static readonly string SECTION_DEFAULT = "Default";
  public static readonly string SECTION_WRAP_START = "[";
  public static readonly string SECTION_WRAP_END = "]";
  public static readonly string VALUE_EQUALS = "=";
  private Dictionary<string, Dictionary<string, string>> values;

  public IniFile() => this.values = new Dictionary<string, Dictionary<string, string>>();

  public string? GetValue(string section, string key)
  {
    if (!this.values.ContainsKey(section))
      return (string) null;
    Dictionary<string, string> dictionary = this.values[section];
    return dictionary.ContainsKey(key) ? dictionary[key] : (string) null;
  }

  public Dictionary<string, string>? GetSection(string section)
  {
    return this.values.ContainsKey(section) ? this.values[section] : (Dictionary<string, string>) null;
  }

  public void ReadFromFile(string fileDir)
  {
    this.values.Clear();
    if (!File.Exists(fileDir))
      return;
    this.ReadFrom(File.ReadAllText(fileDir));
  }

  public void ReadFrom(string fileContents)
  {
    this.values.Clear();
    char[] charArray = IniFile.INI_SPLITTER.ToCharArray();
    string[] strArray = fileContents.Split(charArray);
    string sectionDefault = IniFile.SECTION_DEFAULT;
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    this.values.Add(sectionDefault, dictionary);
    foreach (string str1 in strArray)
    {
      if (str1 != null && str1.Length > 0 && !str1.Equals(string.Empty) && !str1.StartsWith(IniFile.INI_COMMENT))
      {
        if (str1.StartsWith(IniFile.SECTION_WRAP_START) && str1.EndsWith(IniFile.SECTION_WRAP_END))
        {
          string key = LauncherUtils.RemoveSpaces(str1.Substring(IniFile.SECTION_WRAP_START.Length, str1.Length - IniFile.SECTION_WRAP_START.Length - IniFile.SECTION_WRAP_END.Length), true, true);
          dictionary = new Dictionary<string, string>();
          this.values.Add(key, dictionary);
        }
        else
        {
          int num = str1.IndexOf(IniFile.VALUE_EQUALS);
          if (num != -1)
          {
            string str2 = str1.Substring(0, num - 1);
            string str3 = str1.Substring(num + IniFile.VALUE_EQUALS.Length, str1.Length - (num + IniFile.VALUE_EQUALS.Length));
            string key = LauncherUtils.RemoveSpaces(str2, true, true);
            string str4 = LauncherUtils.RemoveSpaces(str3, true, false);
            dictionary.Add(key, str4);
          }
        }
      }
    }
  }
}
