#nullable enable
namespace TCULauncher;

public class TCUNet
{
  public readonly string rootUrl;
  private readonly string resourceInfoAddress;
  public bool isOffline;
  private bool ForceOffline;
  private HttpClient? web;
  private TCUNetResourceInfo? resourceInfo;

  public TCUNet()
  {
    this.rootUrl = "";
    this.resourceInfoAddress = "";
    this.isOffline = true;
    this.web = (HttpClient) null;
    this.resourceInfo = (TCUNetResourceInfo) null;
  }

  public TCUNet(string rootUrl, string resourceInfoAddress, bool isOffline)
  {
    this.web = new HttpClient();
    if (rootUrl.EndsWith('/') || rootUrl.EndsWith('\\'))
      rootUrl = rootUrl.Substring(0, rootUrl.Length - 1);
    this.rootUrl = rootUrl;
    this.resourceInfoAddress = resourceInfoAddress;
    if (!isOffline)
    {
      this.LoadResourceInfo();
    }
    else
    {
      this.ForceOffline = true;
      this.isOffline = true;
      this.resourceInfo = (TCUNetResourceInfo) null;
    }
  }

  protected bool LoadResourceInfo()
  {
    if (this.ForceOffline)
      return false;
    this.resourceInfo = TCUNetResourceInfo.FromJson(TCUNetUtils.LoadResourceAsString(this.web, this.resourceInfoAddress));
    if (this.resourceInfo != null)
      return true;
    this.isOffline = true;
    return false;
  }

  public bool SanityCheck() => this.LoadResourceInfo();

  public void Dispose() => this.web?.Dispose();

  public HttpClient? GetHttpClient() => this.web;

  public string GetRootUrl() => this.rootUrl;

  public TCUNetResourceInfo? GetResourceInfo() => this.resourceInfo;

  public Task<Stream>? DownloadFile(string fileUrl)
  {
    return this.web != null ? this.web.GetStreamAsync(fileUrl) : (Task<Stream>) null;
  }
}
