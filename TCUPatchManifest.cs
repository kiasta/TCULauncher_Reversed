using System.Globalization;
using System.Xml;

#nullable enable
namespace TCULauncher;

public class TCUPatchManifest
{
  private DateTime installedDate = DateTime.MinValue;
  private string gameVersion = "";
  private string patchVersion = "";
  private List<string> filelist = new List<string>();

  public TCUPatchManifest() => this.reset();

  public TCUPatchManifest(
    DateTime installedDate,
    string gameVersion,
    string patchVersion,
    List<string> filelist)
  {
    this.installedDate = installedDate;
    this.gameVersion = gameVersion;
    this.patchVersion = patchVersion;
    this.filelist = filelist;
  }

  public DateTime GetInstalledDate() => this.installedDate;

  public string GetGameVersion() => this.gameVersion;

  public string GetPatchVersion() => this.patchVersion;

  public List<string> GetFileList() => this.filelist;

  protected void reset()
  {
    this.installedDate = DateTime.MinValue;
    this.gameVersion = "";
    this.patchVersion = "";
    this.filelist.Clear();
  }

  public bool ReadFromFile(string filePath)
  {
    this.reset();
    if (!File.Exists(filePath))
    {
      Console.WriteLine("Can't load TCU patch manifest from directory: " + filePath);
      return false;
    }
    XmlDocument xmlDocument = new XmlDocument();
    xmlDocument.Load(filePath);
    XmlNodeList elementsByTagName1 = xmlDocument.GetElementsByTagName("info");
    if (elementsByTagName1.Count > 0)
    {
      XmlNode xmlNode1 = elementsByTagName1[0];
      if (xmlNode1 != null)
      {
        XmlAttributeCollection attributes = xmlNode1.Attributes;
        int num = 0;
        if (attributes != null && attributes.Count > 0)
        {
          foreach (XmlAttribute xmlAttribute in (XmlNamedNodeMap) attributes)
          {
            switch (xmlAttribute.Name)
            {
              case "installedDate":
                if (xmlAttribute.Value.Length > 0)
                {
                  try
                  {
                    this.installedDate = DateTime.Parse(xmlAttribute.Value, (IFormatProvider) new CultureInfo("en-US"));
                    ++num;
                    continue;
                  }
                  catch (Exception ex)
                  {
                    this.installedDate = DateTime.MinValue;
                    continue;
                  }
                }
                else
                  continue;
              case "patchVersion":
                if (xmlAttribute.Value.Length > 0)
                {
                  this.patchVersion = xmlAttribute.Value;
                  ++num;
                  continue;
                }
                continue;
              case "gameVersion":
                if (xmlAttribute.Value.Length > 0)
                {
                  this.gameVersion = xmlAttribute.Value;
                  ++num;
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
        if (num >= 3)
        {
          XmlNodeList elementsByTagName2 = xmlDocument.GetElementsByTagName("file");
          if (elementsByTagName2.Count > 0)
          {
            foreach (XmlNode xmlNode2 in elementsByTagName2)
            {
              string innerText = xmlNode2.InnerText;
              if (innerText.Length > 0)
                this.filelist.Add(innerText);
            }
          }
          return true;
        }
      }
    }
    return false;
  }

  public bool WriteToFile(string outputFile)
  {
    string directoryName = Path.GetDirectoryName(outputFile);
    if (directoryName == null || !Path.Exists(directoryName))
      return false;
    XmlDocument xmlDocument = new XmlDocument();
    XmlNode element1 = (XmlNode) xmlDocument.CreateElement("tcupatch");
    xmlDocument.AppendChild(element1);
    XmlElement element2 = xmlDocument.CreateElement("info");
    element2.SetAttribute("installedDate", this.installedDate.ToString((IFormatProvider) new CultureInfo("en-US")));
    element2.SetAttribute("gameVersion", this.gameVersion);
    element2.SetAttribute("patchVersion", this.patchVersion);
    element1.AppendChild((XmlNode) element2);
    if (this.filelist.Count > 0)
    {
      foreach (string str in this.filelist)
      {
        XmlElement element3 = xmlDocument.CreateElement("file");
        element3.InnerText = str;
        element1.AppendChild((XmlNode) element3);
      }
    }
    xmlDocument.Save(outputFile);
    return true;
  }
}
