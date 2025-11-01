#nullable enable
namespace TCULauncher;

public class TCUNetUtils
{
  public static readonly string TCUNET_INFO_FILE = "tcu.info.json";
  public static readonly string[] TCUNET_HOSTNAMES = new string[2]
  {
    "https://thecrewunlimited.com",
    "https://the-crew-unlimited.github.io"
  };

  public static string? LoadResourceAsString(HttpClient? web, string address)
  {
    if (web == null)
      return (string) null;
    Task<string> stringAsync = web.GetStringAsync(address);
    try
    {
      stringAsync.Wait();
    }
    catch (Exception ex)
    {
      return (string) null;
    }
    return stringAsync.Result;
  }

  public static string RemoveLastSlashFromUrl(string url)
  {
    if (url.EndsWith("/"))
      url = url.Substring(0, url.Length - 1);
    return url;
  }
}
