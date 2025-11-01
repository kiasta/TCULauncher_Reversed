using DamienG.Security.Cryptography;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

#nullable enable
namespace TCULauncher;

public class SaveManager
{
  public static readonly byte DEFAULT_SAVE_VERSION = 1;
  public static readonly string FORMAT_BIN = ".bin";
  public static readonly string FORMAT_JSON = ".json";
  public static readonly string FILENAME = "data";
  public static readonly string PLAYERNAME_TAG = "Player";
  public static readonly CompressionLevel ZLIB_COMP_LEVEL = CompressionLevel.Optimal;
  public static readonly Encoding JSON_ENCODING = Encoding.UTF8;

  public static KeyValuePair<SaveInstance?, bool> LoadServerSave(string dataPath)
  {
    if (!File.Exists(dataPath))
      throw new IOException("Server save data doesn't exist in: " + dataPath);
    byte defaultSaveVersion = SaveManager.DEFAULT_SAVE_VERSION;
    string json;
    bool zlib;
    if (dataPath.EndsWith(SaveManager.FORMAT_BIN) || dataPath.EndsWith(SaveManager.FORMAT_BIN + LauncherGlobals.SAVE_BAK_EXT))
    {
      try
      {
        int num1 = -1;
        using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
        {
          long length = fileStream.Length;
          if (length == 0L)
          {
            fileStream.Dispose();
            return new KeyValuePair<SaveInstance, bool>((SaveInstance) null, false);
          }
          int count = (int) (length - 9L);
          byte[] buffer = new byte[count];
          fileStream.Position = 9L;
          fileStream.Read(buffer, 0, count);
          byte[] hash = new Crc32(3988292384U, uint.MaxValue).ComputeHash(buffer);
          Array.Reverse<byte>(hash);
          num1 = BitConverter.ToInt32((ReadOnlySpan<byte>) hash);
        }
        using (FileStream fileStream = File.Open(dataPath, FileMode.Open))
        {
          byte[] buffer1 = new byte[9];
          fileStream.Read(buffer1, 0, 1);
          fileStream.Read(buffer1, 1, 4);
          fileStream.Read(buffer1, 5, 4);
          defaultSaveVersion = buffer1[0];
          int int32 = BitConverter.ToInt32(buffer1, 1);
          BitConverter.ToInt32(buffer1, 5);
          int num2 = num1;
          if (int32 != num2)
            return new KeyValuePair<SaveInstance, bool>((SaveInstance) null, false);
          ZLibStream zlibStream = new ZLibStream((Stream) fileStream, CompressionMode.Decompress, false);
          MemoryStream memoryStream = new MemoryStream();
          byte[] buffer2 = new byte[1024 /*0x0400*/];
          int count;
          while ((count = zlibStream.Read(buffer2, 0, buffer2.Length)) > 0)
            memoryStream.Write(buffer2, 0, count);
          memoryStream.Flush();
          byte[] array = memoryStream.ToArray();
          json = SaveManager.JSON_ENCODING.GetString(array);
          zlib = true;
          memoryStream.Dispose();
          zlibStream.Dispose();
        }
      }
      catch (IOException ex)
      {
        return new KeyValuePair<SaveInstance, bool>((SaveInstance) null, true);
      }
    }
    else
    {
      if (!dataPath.EndsWith(SaveManager.FORMAT_JSON) && !dataPath.EndsWith(SaveManager.FORMAT_JSON + LauncherGlobals.SAVE_BAK_EXT))
        return new KeyValuePair<SaveInstance, bool>((SaveInstance) null, false);
      json = File.ReadAllText(dataPath, SaveManager.JSON_ENCODING);
      zlib = false;
    }
    if (json != null)
    {
      if (json.Length > 0)
      {
        try
        {
          JsonNode jsonRoot = JsonSerializer.Deserialize<JsonNode>(json);
          if (jsonRoot != null)
          {
            if (dataPath.EndsWith(LauncherGlobals.SAVE_BAK_EXT))
              dataPath = dataPath.Substring(0, dataPath.Length - LauncherGlobals.SAVE_BAK_EXT.Length);
            return new KeyValuePair<SaveInstance, bool>(new SaveInstance(dataPath, jsonRoot, zlib, defaultSaveVersion), false);
          }
        }
        catch (JsonException ex)
        {
        }
      }
    }
    return new KeyValuePair<SaveInstance, bool>((SaveInstance) null, false);
  }
}
