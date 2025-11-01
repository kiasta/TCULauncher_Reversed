using System.Text.Json;
using System.Text.Json.Nodes;

#nullable enable
namespace TCULauncher;

public class TCUNetResourceInfo
{
  public readonly string ResourceName;
  public readonly string ResourceDate;
  public readonly string HomePage;
  public readonly string NewsDirectory;
  public readonly string NewsList;
  public readonly string LauncherDirectory;
  public readonly string ServerDirectory;
  public readonly string LauncherManifest;
  public readonly string ServerManifest;
  public readonly string? TcuWebsiteLink;
  public readonly string? DiscordLink;
  public readonly string? PatreonLink;
  public readonly string? YouTubeLink;

  public TCUNetResourceInfo(
    string ResourceName,
    string ResourceDate,
    string HomePage,
    string NewsDirectory,
    string NewsList,
    string LauncherDirectory,
    string ServerDirectory,
    string LauncherManifest,
    string ServerManifest,
    string? TcuWebsiteLink,
    string? DiscordLink,
    string? PatreonLink,
    string? YouTubeLink)
  {
    this.ResourceName = ResourceName;
    this.ResourceDate = ResourceDate;
    this.HomePage = HomePage;
    this.NewsDirectory = NewsDirectory;
    this.NewsList = NewsList;
    this.LauncherDirectory = LauncherDirectory;
    this.ServerDirectory = ServerDirectory;
    this.LauncherManifest = LauncherManifest;
    this.ServerManifest = ServerManifest;
    this.TcuWebsiteLink = TcuWebsiteLink;
    this.DiscordLink = DiscordLink;
    this.PatreonLink = PatreonLink;
    this.YouTubeLink = YouTubeLink;
  }

  public static TCUNetResourceInfo? FromJson(string? jsonData)
  {
    if (jsonData != null)
    {
      string TcuWebsiteLink = (string) null;
      string DiscordLink = (string) null;
      string PatreonLink = (string) null;
      string YouTubeLink = (string) null;
      string str;
      string ServerManifest = str = "";
      string LauncherManifest = str;
      string ServerDirectory = str;
      string LauncherDirectory = str;
      string NewsList = str;
      string NewsDirectory = str;
      string HomePage = str;
      string ResourceDate = str;
      string ResourceName = str;
      try
      {
        JsonNode jsonNode = JsonSerializer.Deserialize<JsonNode>(jsonData);
        if (jsonNode != null)
        {
          ResourceName = jsonNode["ResourceName"]?.ToString();
          ResourceDate = jsonNode["ResourceDate"]?.ToString();
          HomePage = jsonNode["HomePage"]?.ToString();
          NewsDirectory = jsonNode["NewsDirectory"]?.ToString();
          NewsList = jsonNode["NewsList"]?.ToString();
          LauncherDirectory = jsonNode["LauncherDirectory"]?.ToString();
          ServerDirectory = jsonNode["ServerDirectory"]?.ToString();
          LauncherManifest = jsonNode["LauncherManifest"]?.ToString();
          ServerManifest = jsonNode["ServerManifest"]?.ToString();
          TcuWebsiteLink = jsonNode["TCUWebsiteLink"]?.ToString();
          DiscordLink = jsonNode["DiscordLink"]?.ToString();
          PatreonLink = jsonNode["PatreonLink"]?.ToString();
          YouTubeLink = jsonNode["YouTubeLink"]?.ToString();
        }
        if (ResourceName != null)
        {
          if (ResourceDate != null)
          {
            if (HomePage != null)
            {
              if (NewsDirectory != null)
              {
                if (LauncherDirectory != null)
                {
                  if (ServerDirectory != null)
                  {
                    if (LauncherManifest != null)
                    {
                      if (ServerManifest != null)
                        return new TCUNetResourceInfo(ResourceName, ResourceDate, HomePage, NewsDirectory, NewsList, LauncherDirectory, ServerDirectory, LauncherManifest, ServerManifest, TcuWebsiteLink, DiscordLink, PatreonLink, YouTubeLink);
                    }
                  }
                }
              }
            }
          }
        }
      }
      catch (JsonException ex)
      {
        return (TCUNetResourceInfo) null;
      }
    }
    return (TCUNetResourceInfo) null;
  }

  public string? GetTCUWebsiteLink() => this.TcuWebsiteLink;

  public string? GetDiscordLink() => this.DiscordLink;

  public string? GetPatreonLink() => this.PatreonLink;

  public string? GetYouTubeLink() => this.YouTubeLink;

  public string GetURLHomePage(string rootUrl)
  {
    rootUrl = TCUNetUtils.RemoveLastSlashFromUrl(rootUrl);
    return rootUrl + this.HomePage;
  }

  public string GetURLNewsDirectory(string rootUrl)
  {
    rootUrl = TCUNetUtils.RemoveLastSlashFromUrl(rootUrl);
    return rootUrl + this.NewsDirectory;
  }

  public string GetURLNewsListFile(string rootUrl)
  {
    rootUrl = TCUNetUtils.RemoveLastSlashFromUrl(rootUrl);
    return rootUrl + this.NewsList;
  }

  public string GetURLLauncherDirectory(string rootUrl)
  {
    rootUrl = TCUNetUtils.RemoveLastSlashFromUrl(rootUrl);
    return rootUrl + this.LauncherDirectory;
  }

  public string GetURLServerDirectory(string rootUrl)
  {
    rootUrl = TCUNetUtils.RemoveLastSlashFromUrl(rootUrl);
    return rootUrl + this.ServerDirectory;
  }

  public string GetURLLauncherManifest(string rootUrl)
  {
    rootUrl = TCUNetUtils.RemoveLastSlashFromUrl(rootUrl);
    return rootUrl + this.LauncherManifest;
  }

  public string GetURLServerManifest(string rootUrl)
  {
    rootUrl = TCUNetUtils.RemoveLastSlashFromUrl(rootUrl);
    return rootUrl + this.ServerManifest;
  }

  public string? GetLauncherVersionURL(HttpClient? web, string rootUrl, TCULauncherVersion version)
  {
    return web == null || version == null ? (string) null : $"{this.GetURLLauncherDirectory(rootUrl)}/{version.GetFile()}";
  }

  public string? GetServerVersionURL(HttpClient? web, string rootUrl, TCUServerVersion version)
  {
    return web == null || version == null ? (string) null : $"{this.GetURLServerDirectory(rootUrl)}/{version.GetFile()}";
  }

  public string? GetNewsList(HttpClient? web, string rootUrl)
  {
    return web == null ? (string) null : TCUNetUtils.LoadResourceAsString(web, this.GetURLNewsListFile(rootUrl));
  }

  public string? GetLauncherManifestAsString(HttpClient? web, string rootUrl)
  {
    return web == null ? (string) null : TCUNetUtils.LoadResourceAsString(web, this.GetURLLauncherManifest(rootUrl));
  }

  public string? GetServerManifestAsString(HttpClient? web, string rootUrl)
  {
    return web == null ? (string) null : TCUNetUtils.LoadResourceAsString(web, this.GetURLServerManifest(rootUrl));
  }
}
