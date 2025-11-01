using System.Xml;

#nullable enable
namespace TCULauncher;

public class Cheats
{
  public static List<Cheat> ReadCheatManifestXML(string directory)
  {
    List<Cheat> cheatList = new List<Cheat>();
    XmlDocument xmlDocument = new XmlDocument();
    if (!File.Exists(directory))
    {
      Console.WriteLine("Can't load cheat manifest from directory: " + directory);
      return cheatList;
    }
    xmlDocument.Load(directory);
    XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("cheat");
    if (elementsByTagName.Count > 0)
    {
      foreach (XmlNode xmlNode in elementsByTagName)
      {
        if (xmlNode.Attributes != null && xmlNode.Attributes.Count > 0)
        {
          string Name = (string) null;
          string Type = (string) null;
          string DefaultValue = (string) null;
          foreach (XmlAttribute attribute in (XmlNamedNodeMap) xmlNode.Attributes)
          {
            switch (attribute.Name)
            {
              case "name":
                Name = attribute.Value;
                continue;
              case "type":
                Type = attribute.Value;
                continue;
              case "default":
                DefaultValue = attribute.Value;
                continue;
              default:
                continue;
            }
          }
          string innerText = xmlNode.InnerText;
          if (Name != null)
            cheatList.Add(new Cheat(Name, Type, DefaultValue, innerText));
        }
      }
    }
    return cheatList;
  }
}
