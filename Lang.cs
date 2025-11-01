using System.Xml;

#nullable enable
namespace TCULauncher;

public class Lang
{
  public static readonly char[] ARG_WRAP = "{}".ToCharArray();
  private string? language;
  private string? author;
  private Dictionary<string, string> strings;

  public Lang() => this.strings = new Dictionary<string, string>();

  public string? GetLanguage() => this.language;

  public string? GetAuthor() => this.author;

  protected string formatString(string str)
  {
    str = str.Replace(" \\n ", Environment.NewLine);
    str = str.Replace(" \\n", Environment.NewLine);
    str = str.Replace("\\n  ", Environment.NewLine);
    str = str.Replace("\\n", Environment.NewLine);
    return str;
  }

  public string? GetString(string id, string?[]? args)
  {
    string str1;
    if (!this.strings.TryGetValue(id, out str1))
      return (string) null;
    string str2 = str1;
    if (args != null)
    {
      for (int index = 0; index < args.Length; ++index)
        str2 = str2.Replace(Lang.ARG_WRAP[0].ToString() + index.ToString() + Lang.ARG_WRAP[1].ToString(), args[index]);
    }
    return this.formatString(str2);
  }

  public void LoadFromFile(string langFile)
  {
    this.strings.Clear();
    if (!File.Exists(langFile))
      throw new ArgumentException("Can't load lang file!: " + langFile);
    File.ReadAllText(langFile);
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.Load(langFile);
    XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("info");
    XmlNode xmlNode1 = elementsByTagName.Count > 0 ? elementsByTagName.Item(0) : throw new ArgumentException("Invalid info node in lang file: " + langFile);
    if (xmlNode1 == null || xmlNode1.Attributes == null)
      throw new ArgumentException("Invalid info node in lang file: " + langFile);
    foreach (XmlNode attribute in (XmlNamedNodeMap) xmlNode1.Attributes)
    {
      switch (attribute.Name)
      {
        case "lang":
          if (attribute.Value != null)
          {
            this.language = attribute.Value;
            continue;
          }
          continue;
        case "author":
          this.author = attribute.Value;
          continue;
        default:
          continue;
      }
    }
    if (this.language == null)
      throw new ArgumentException("Info node is missing a \"lang\" attribute in lang file: " + langFile);
    foreach (XmlNode xmlNode2 in xmlDocument.GetElementsByTagName("string"))
    {
      string key = (string) null;
      string innerText = xmlNode2.InnerText;
      if (xmlNode2.Attributes != null)
      {
        foreach (XmlNode attribute in (XmlNamedNodeMap) xmlNode2.Attributes)
        {
          if (attribute.Name == "id")
            key = attribute.Value;
        }
      }
      if (key != null)
      {
        if (innerText == null)
          throw new ArgumentException($"Invalid string node in lang file: {langFile} (Missing inner text)");
        this.strings.Add(key, innerText);
      }
      else
        throw new ArgumentException($"Invalid string node in lang file: {langFile} (\"{innerText}\")");
    }
  }
}
