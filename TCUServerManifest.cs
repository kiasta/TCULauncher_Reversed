using System.Text.Json;
using System.Text.Json.Nodes;

#nullable enable
namespace TCULauncher;

public class TCUServerManifest
{
  private readonly string latestVersion;
  private readonly TCUServerVersion[]? versions;

  public TCUServerManifest(string latestVersion, TCUServerVersion[] versions)
  {
    this.latestVersion = latestVersion;
    this.versions = versions;
  }

  public string GetLatestVersion() => this.latestVersion;

  public TCUServerVersion[]? GetVersions() => this.versions;

  public TCUServerVersion? GetVersionFromString(string version)
  {
    if (this.versions != null)
    {
      string lower = version.ToLower();
      foreach (TCUServerVersion version1 in this.versions)
      {
        if (version1.GetVersion().ToLower().Equals(lower))
          return version1;
      }
    }
    return (TCUServerVersion) null;
  }

  public TCUServerVersion? PickLatestForGameVersion(string? gameVersion)
  {
    if (gameVersion == null || gameVersion.Length == 0)
      return (TCUServerVersion) null;
    TCUServerVersion versionFromString = this.GetVersionFromString(this.latestVersion);
    if (versionFromString != null && versionFromString.IsGameVersionSupported(gameVersion))
      return versionFromString;
    if (this.versions != null)
    {
      for (int index = 0; index < this.versions.Length; ++index)
      {
        TCUServerVersion version = this.versions[this.versions.Length - 1 - index];
        if (version.IsGameVersionSupported(gameVersion))
          return version;
      }
    }
    return (TCUServerVersion) null;
  }

  public TCUServerVersion? IsPatchOutdated(string? gameVersion, TCUPatchManifest patchManifest)
  {
    TCUServerVersion tcuServerVersion = this.PickLatestForGameVersion(gameVersion);
    return tcuServerVersion != null && (tcuServerVersion.GetVersion() != patchManifest.GetPatchVersion() || gameVersion != patchManifest.GetGameVersion()) ? tcuServerVersion : (TCUServerVersion) null;
  }

  public static TCUServerManifest? LoadFromTCUNet(TCUNet tcuNet)
  {
    string manifestAsString = tcuNet.GetResourceInfo()?.GetServerManifestAsString(tcuNet.GetHttpClient(), tcuNet.GetRootUrl());
    if (manifestAsString != null)
    {
      try
      {
        JsonNode jsonNode1 = JsonSerializer.Deserialize<JsonNode>(manifestAsString);
        if (jsonNode1 != null)
        {
          string version1 = jsonNode1["LatestVersion"]?.ToString();
          TCUServerVersion[] versions = Array.Empty<TCUServerVersion>();
          JsonNode jsonNode2 = jsonNode1["Versions"];
          if (jsonNode2 != null && jsonNode2.GetType() == typeof (JsonArray))
          {
            JsonArray jsonArray1 = (JsonArray) jsonNode2;
            versions = new TCUServerVersion[jsonArray1.Count];
            for (int index1 = 0; index1 < jsonArray1.Count; ++index1)
            {
              JsonNode jsonNode3 = jsonArray1[index1];
              if (jsonNode3 != null)
              {
                string version2 = jsonNode3["Version"]?.ToString();
                string file = jsonNode3["File"]?.ToString();
                string str = jsonNode3["IsEncrypted"]?.ToString();
                string hash = jsonNode3["SHA256"]?.ToString();
                string[] supportedGameVersions = Array.Empty<string>();
                JsonArray jsonArray2 = jsonNode3["SupportedGameVersions"]?.AsArray();
                if (jsonArray2 != null)
                {
                  supportedGameVersions = new string[jsonArray2.Count];
                  for (int index2 = 0; index2 < jsonArray2.Count; ++index2)
                    supportedGameVersions[index2] = jsonArray2[index2]?.ToString() ?? "";
                }
                if (version2 != null && file != null)
                {
                  TCUServerVersion tcuServerVersion = new TCUServerVersion(version2, file, str != null && str.Equals("true"), hash, supportedGameVersions);
                  versions[index1] = tcuServerVersion;
                }
              }
            }
            if (version1 == null)
              version1 = versions[versions.Length - 1].GetVersion();
          }
          if (version1 != null)
            return new TCUServerManifest(version1, versions);
        }
      }
      catch (JsonException ex)
      {
      }
    }
    return (TCUServerManifest) null;
  }
}
