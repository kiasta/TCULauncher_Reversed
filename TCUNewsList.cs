using System.Text.Json.Nodes;

#nullable enable
namespace TCULauncher;

public class TCUNewsList
{
  private List<string> newsFiles = new List<string>();

  public TCUNewsList(JsonNode jsonRoot) => this.LoadFromJson(jsonRoot);

  public void LoadFromJson(JsonNode jsonRoot)
  {
    this.newsFiles.Clear();
    JsonNode jsonNode = jsonRoot["NewsList"];
    if (jsonNode == null || !(jsonNode.GetType() == typeof (JsonArray)))
      return;
    foreach (object obj in jsonNode.AsArray())
    {
      string str = obj?.ToString();
      if (str != null)
        this.newsFiles.Add(str);
    }
  }

  public List<string> GetNewsFiles() => this.newsFiles;

  public string? GetLatestNewsFile()
  {
    return this.newsFiles.Count > 0 ? this.newsFiles[this.newsFiles.Count - 1] : (string) null;
  }
}
