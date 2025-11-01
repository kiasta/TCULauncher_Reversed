using System.Text.Json;
using System.Text.Json.Nodes;

#nullable enable
namespace TCULauncher;

public class TCULauncherManifest
{
  private readonly string latestLauncher;
  private TCULauncherVersion[]? versions;

  public TCULauncherManifest(string latestLauncher, TCULauncherVersion[] versions)
  {
    this.latestLauncher = latestLauncher;
    this.versions = versions;
  }

  public string GetLatestLauncher() => this.latestLauncher;

  public TCULauncherVersion[]? GetVersions() => this.versions;

  public TCULauncherVersion? GetVersionFromString(string version)
  {
    if (this.versions != null)
    {
      string lower = version.ToLower();
      foreach (TCULauncherVersion version1 in this.versions)
      {
        if (version1.GetVersion().ToLower().Equals(lower))
          return version1;
      }
    }
    return (TCULauncherVersion) null;
  }

  public static TCULauncherManifest? LoadFromTCUNet(TCUNet tcuNet)
  {
    string manifestAsString = tcuNet.GetResourceInfo()?.GetLauncherManifestAsString(tcuNet.GetHttpClient(), tcuNet.GetRootUrl());
    if (manifestAsString != null)
    {
      try
      {
        JsonNode jsonNode1 = JsonSerializer.Deserialize<JsonNode>(manifestAsString);
        if (jsonNode1 != null)
        {
          string version1 = jsonNode1["LatestLauncherVer"]?.ToString();
          TCULauncherVersion[] versions = Array.Empty<TCULauncherVersion>();
          JsonNode jsonNode2 = jsonNode1["Versions"];
          if (jsonNode2 != null && jsonNode2.GetType() == typeof (JsonArray))
          {
            JsonArray jsonArray = (JsonArray) jsonNode2;
            versions = new TCULauncherVersion[jsonArray.Count];
            for (int index = 0; index < jsonArray.Count; ++index)
            {
              JsonNode jsonNode3 = jsonArray[index];
              if (jsonNode3 != null)
              {
                string version2 = jsonNode3["Version"]?.ToString();
                string file = jsonNode3["File"]?.ToString();
                string str = jsonNode3["IsEncrypted"]?.ToString();
                if (version2 != null && file != null)
                {
                  TCULauncherVersion tcuLauncherVersion = new TCULauncherVersion(version2, file, str == "true");
                  versions[index] = tcuLauncherVersion;
                }
              }
            }
            if (version1 == null)
              version1 = versions[versions.Length - 1].GetVersion();
          }
          return version1 != null ? new TCULauncherManifest(version1, versions) : (TCULauncherManifest) null;
        }
      }
      catch (JsonException ex)
      {
      }
    }
    return (TCULauncherManifest) null;
  }
}
