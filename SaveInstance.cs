using DamienG.Security.Cryptography;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

#nullable enable
namespace TCULauncher;

public class SaveInstance
{
  private readonly string datajsonPath;
  private readonly JsonNode jsonRoot;
  private bool zlib;
  private byte version = SaveManager.DEFAULT_SAVE_VERSION;

  public SaveInstance(string datajsonPath, bool zlib, byte version)
  {
    this.version = version;
    this.datajsonPath = datajsonPath;
    this.jsonRoot = JsonNode.Parse("{ \"userdata\": {} }") ?? throw new JsonException("Failed to create a new blank local save!");
    this.zlib = zlib;
  }

  public SaveInstance(string datajsonPath, JsonNode jsonRoot, bool zlib, byte version)
  {
    this.version = version;
    this.datajsonPath = datajsonPath;
    this.jsonRoot = jsonRoot;
    this.zlib = zlib;
  }

  public string GetOriginalFilePath() => this.datajsonPath;

  public byte GetVersion() => this.version;

  public bool isUseCompression() => this.zlib;

  public void SetUseCompression(bool useCompression) => this.zlib = useCompression;

  public List<string> GetPlayerNames()
  {
    List<string> playerNames = new List<string>();
    JsonNode jsonNode1 = this.jsonRoot["userdata"];
    if (jsonNode1 != null && jsonNode1.GetType() == typeof (JsonArray))
    {
      foreach (JsonNode jsonNode2 in jsonNode1.AsArray())
      {
        if (jsonNode2 != null)
        {
          string str = jsonNode2[SaveManager.PLAYERNAME_TAG]?.ToString();
          if (str != null && str.Length > 0)
            playerNames.Add(str);
        }
      }
    }
    return playerNames;
  }

  public bool PlayerNameExists(string playerName)
  {
    JsonNode jsonNode1 = this.jsonRoot["userdata"];
    if (jsonNode1 != null && jsonNode1.GetType() == typeof (JsonArray))
    {
      foreach (JsonNode jsonNode2 in jsonNode1.AsArray())
      {
        if (jsonNode2 != null)
        {
          string str = jsonNode2[SaveManager.PLAYERNAME_TAG]?.ToString();
          if (str != null && str.ToLower().Equals(playerName.ToLower()))
            return true;
        }
      }
    }
    return false;
  }

  protected JsonArray ValidateUserdatas(JsonNode? userdatas)
  {
    if (userdatas == null || !(userdatas.GetType() == typeof (JsonArray)))
    {
      userdatas = (JsonNode) new JsonArray(new JsonNodeOptions?());
      this.jsonRoot["userdata"] = userdatas;
    }
    return (JsonArray) userdatas;
  }

  public bool AddNewSave(string NewSaveName)
  {
    foreach (string playerName in this.GetPlayerNames())
    {
      if (playerName.ToLower().Equals(NewSaveName.ToLower()))
        return false;
    }
    this.ValidateUserdatas(this.jsonRoot["userdata"]).Add<JsonObject>(new JsonObject()
    {
      {
        SaveManager.PLAYERNAME_TAG,
        (JsonNode) NewSaveName
      }
    });
    return true;
  }

  public bool RemoveSave(string PlayerName)
  {
    JsonArray jsonArray = this.ValidateUserdatas(this.jsonRoot["userdata"]);
    for (int index = 0; index < jsonArray.Count; ++index)
    {
      JsonNode jsonNode1 = jsonArray[index];
      if (jsonNode1 != null)
      {
        JsonNode jsonNode2 = jsonNode1.AsObject()[SaveManager.PLAYERNAME_TAG];
        if (jsonNode2 != null && jsonNode2.ToString().ToLower().Equals(PlayerName.ToLower()))
        {
          jsonArray.RemoveAt(index);
          return true;
        }
      }
    }
    return false;
  }

  public string? ReadPlayerNameFromJson(string jsonStr)
  {
    try
    {
      JsonNode jsonNode = JsonSerializer.Deserialize<JsonNode>(jsonStr);
      if (jsonNode != null)
        return jsonNode[SaveManager.PLAYERNAME_TAG]?.ToString();
    }
    catch (JsonException ex)
    {
    }
    return (string) null;
  }

  public string? ImportSave(string saveFilePath, string? newSaveName)
  {
    JsonNode jsonNode = JsonSerializer.Deserialize<JsonNode>(File.ReadAllText(saveFilePath));
    if (jsonNode == null)
      return (string) null;
    string str = jsonNode[SaveManager.PLAYERNAME_TAG]?.ToString() ?? LauncherGlobals.SAVEGAME_DEFAULT_NAME;
    if (newSaveName != null)
      str = newSaveName;
    jsonNode[SaveManager.PLAYERNAME_TAG] = (JsonNode) str;
    foreach (string playerName in this.GetPlayerNames())
    {
      if (playerName.ToLower().Equals(str.ToLower()))
        return (string) null;
    }
    this.ValidateUserdatas(this.jsonRoot["userdata"]).Add(jsonNode);
    return str;
  }

  public bool ExportSave(string playerName, string outputFilePath)
  {
    foreach (JsonNode validateUserdata in this.ValidateUserdatas(this.jsonRoot["userdata"]))
    {
      if (validateUserdata != null)
      {
        string str = validateUserdata[SaveManager.PLAYERNAME_TAG]?.ToString();
        if (str != null && str.ToLower().Equals(playerName.ToLower()))
        {
          JsonSerializerOptions options = new JsonSerializerOptions()
          {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
          };
          string contents = JsonSerializer.Serialize<JsonNode>(validateUserdata, options);
          File.WriteAllText(outputFilePath, contents);
          return true;
        }
      }
    }
    return false;
  }

  public ImportUbiSaveResult ImportUbiSave(string saveFilePath, string newSaveName)
  {
    JsonObject importedSave = new JsonObject();
    bool failedSave = true;
    bool failedStats = true;
    bool failedScores = true;
    bool failedFog = true;
    bool failedAchievements = true;
    try
    {
      JsonNode jsonNode = JsonSerializer.Deserialize<JsonNode>(File.ReadAllText(saveFilePath));
      if (jsonNode != null)
      {
        string str1 = jsonNode["Save"]?.ToString();
        string str2 = jsonNode["Stats"]?.ToString();
        string s1 = jsonNode["Scores"]?.ToString();
        string str3 = jsonNode["Fog"]?.ToString();
        string s2 = jsonNode["Achievements"]?.ToString();
        if (str1 != null)
        {
          importedSave.Add("Save", (JsonNode) str1);
          failedSave = false;
          if (str2 != null)
          {
            importedSave.Add("Stats", (JsonNode) str2);
            failedStats = false;
          }
          if (str3 != null)
          {
            importedSave.Add("Fog", (JsonNode) str3);
            failedFog = false;
          }
          if (s2 != null)
          {
            try
            {
              byte[] numArray = Convert.FromBase64String(s2);
              int num = 8;
              byte[] inArray = new byte[numArray.Length - num];
              for (int index = num; index < numArray.Length; ++index)
                inArray[index - num] = numArray[index];
              string base64String = Convert.ToBase64String(inArray);
              importedSave.Add("Achievements", (JsonNode) base64String);
              failedAchievements = false;
            }
            catch (Exception ex)
            {
            }
          }
          if (s1 != null)
          {
            try
            {
              byte[] numArray1 = Convert.FromBase64String(s1);
              int capacity = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(numArray1, 17));
              int num1 = 21;
              int num2 = num1 + capacity * 8;
              List<long> longList = new List<long>(capacity);
              List<int> intList = new List<int>(capacity);
              int startIndex = num2 + 20;
              for (int index1 = 0; index1 < capacity; ++index1)
              {
                long num3 = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt64(numArray1, num1 + index1 * 8));
                int num4 = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(numArray1, startIndex));
                int num5 = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(numArray1, startIndex + 16 /*0x10*/));
                int num6 = 0;
                for (int index2 = 0; index2 < num5; ++index2)
                {
                  int num7 = BinaryPrimitives.ReverseEndianness(BitConverter.ToInt32(numArray1, startIndex + 20 + index2 * 4));
                  switch (num7)
                  {
                    case -1:
                    case 0:
                      continue;
                    default:
                      if (num7 > num6)
                      {
                        num6 = num7;
                        continue;
                      }
                      continue;
                  }
                }
                if (num6 != -1 && num6 != 0)
                {
                  longList.Add(num3);
                  intList.Add(num6);
                }
                int num8 = 12;
                startIndex += 4 + num4 + num8;
              }
              int count = intList.Count;
              int num9 = 4;
              int num10 = 12;
              byte[] numArray2 = new byte[num9 + count * num10];
              BitConverter.TryWriteBytes(numArray2.AsSpan<byte>(0, 4), BinaryPrimitives.ReverseEndianness(count));
              for (int index = 0; index < count; ++index)
              {
                BitConverter.TryWriteBytes(numArray2.AsSpan<byte>(num9 + index * num10, 8), BinaryPrimitives.ReverseEndianness(longList[index]));
                BitConverter.TryWriteBytes(numArray2.AsSpan<byte>(num9 + index * num10 + 8, 4), BinaryPrimitives.ReverseEndianness(intList[index]));
              }
              string base64String = Convert.ToBase64String(numArray2);
              importedSave.Add("Scores", (JsonNode) base64String);
              failedScores = false;
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
    }
    catch (JsonException ex)
    {
    }
    if (importedSave != null)
    {
      bool flag = false;
      foreach (string playerName in this.GetPlayerNames())
      {
        if (playerName.ToLower().Equals(newSaveName.ToLower()))
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return new ImportUbiSaveResult((JsonObject) null, newSaveName, failedSave, failedStats, failedScores, failedFog, failedAchievements);
      importedSave.Add(SaveManager.PLAYERNAME_TAG, (JsonNode) newSaveName);
      this.ValidateUserdatas(this.jsonRoot["userdata"]).Add<JsonObject>(importedSave);
    }
    return new ImportUbiSaveResult(importedSave, newSaveName, failedSave, failedStats, failedScores, failedFog, failedAchievements);
  }

  public bool RenameSave(string currentSavename, string newSavename)
  {
    if (this.PlayerNameExists(currentSavename) && !this.PlayerNameExists(newSavename))
    {
      foreach (JsonNode validateUserdata in this.ValidateUserdatas(this.jsonRoot["userdata"]))
      {
        if (validateUserdata != null)
        {
          string str = validateUserdata[SaveManager.PLAYERNAME_TAG]?.ToString();
          if (str != null && str.ToLower().Equals(currentSavename.ToLower()))
          {
            validateUserdata[SaveManager.PLAYERNAME_TAG] = (JsonNode) newSavename;
            return true;
          }
        }
      }
    }
    return false;
  }

  public void SaveToFile(string? newPath)
  {
    string path = newPath ?? this.datajsonPath;
    string str = JsonSerializer.Serialize<JsonNode>(this.jsonRoot, new JsonSerializerOptions()
    {
      WriteIndented = true,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
    if (this.isUseCompression())
    {
      MemoryStream memoryStream = new MemoryStream();
      ZLibStream zlibStream = new ZLibStream((Stream) memoryStream, SaveManager.ZLIB_COMP_LEVEL);
      byte[] bytes = SaveManager.JSON_ENCODING.GetBytes(str);
      zlibStream.Write(bytes, 0, bytes.Length);
      zlibStream.Flush();
      zlibStream.Dispose();
      byte[] array = memoryStream.ToArray();
      memoryStream.Dispose();
      int length1 = array.Length;
      int length2 = str.Length;
      byte[] hash = new Crc32().ComputeHash(array);
      Array.Reverse<byte>(hash);
      FileStream fileStream = File.Create(path);
      fileStream.Write((ReadOnlySpan<byte>) new byte[1]
      {
        this.version
      });
      fileStream.Write((ReadOnlySpan<byte>) hash);
      fileStream.Write((ReadOnlySpan<byte>) BitConverter.GetBytes(length2));
      fileStream.Write((ReadOnlySpan<byte>) array);
      fileStream.Flush();
      fileStream.Dispose();
    }
    else
      File.WriteAllText(path, str);
  }
}
