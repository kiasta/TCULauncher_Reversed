using Microsoft.Win32;
using System.Diagnostics;
using System.Text;

#nullable enable
namespace TCULauncher;

public class LauncherUtils
{
  public static bool VerifyDirectoryToExe(string? directoryToExe)
  {
    return directoryToExe != null && directoryToExe.Length != 0 && directoryToExe.EndsWith(LauncherGlobals.FILE_NAME_EXE);
  }

  public static string? GetVersion(string? directoryToExe)
  {
    return directoryToExe == null || directoryToExe.Length == 0 ? (string) null : FileVersionInfo.GetVersionInfo(directoryToExe).FileVersion;
  }

  public static bool IsSteamVersion(string? directoryToExe)
  {
    string directoryName = Path.GetDirectoryName(directoryToExe);
    return directoryName != null && (File.Exists($"{directoryName}\\{LauncherGlobals.STEAM_API_FILENAME_64}") || File.Exists($"{directoryName}\\{LauncherGlobals.STEAM_API_FILENAME}"));
  }

  public static IniFile ReadServerIni(
    UserData userdata,
    GameInstance gameInstance,
    ServerConfig serverConf)
  {
    IniFile serverIni = new IniFile();
    string str = $"{Path.GetDirectoryName(gameInstance.GetDirectoryPath())}\\{LauncherGlobals.SERVER_CONFIG_FILE}";
    if (File.Exists(str))
    {
      serverIni.ReadFromFile(str);
      serverConf.ReadFromIni(userdata, serverIni);
    }
    else
      serverConf.isLoaded = true;
    return serverIni;
  }

  public static bool SelectFirstAvailableSave(GameInstance GameInstance, ServerConfig ServerConfig)
  {
    SaveInstance saveInstance = GameInstance.GetSaveInstance();
    if (saveInstance != null)
    {
      List<string> playerNames = saveInstance.GetPlayerNames();
      if (playerNames.Count > 0)
      {
        ServerConfig.SetUsername(playerNames[0], true);
        return true;
      }
    }
    return false;
  }

  public static string FilterInvalidCharsInString(string savegameName, string allowedCharsStr)
  {
    if (savegameName.Length == 0)
      return savegameName;
    char[] array1 = savegameName.ToArray<char>();
    char[] array2 = allowedCharsStr.ToArray<char>();
    List<int> intList = new List<int>();
    for (int index1 = 0; index1 < array1.Length; ++index1)
    {
      bool flag = false;
      for (int index2 = 0; index2 < array2.Length; ++index2)
      {
        if ((int) array1[index1] == (int) array2[index2])
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        intList.Add(index1);
    }
    for (int index = 0; index < intList.Count; ++index)
    {
      int startIndex = intList[index] - index;
      savegameName = savegameName.Remove(startIndex, 1);
    }
    return savegameName;
  }

  public static string GetMachineGuid()
  {
    string name1 = "SOFTWARE\\Microsoft\\Cryptography";
    string name2 = "MachineGuid";
    using (RegistryKey registryKey1 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
    {
      using (RegistryKey registryKey2 = registryKey1.OpenSubKey(name1))
        return ((registryKey2 != null ? registryKey2.GetValue(name2) : throw new KeyNotFoundException($"Key Not Found: {name1}")) ?? throw new IndexOutOfRangeException($"Index Not Found: {name2}")).ToString() ?? throw new SystemException("Failed to retrieve MachineGuid as string!");
    }
  }

  public static string XorString(string value, byte[] xorSequence)
  {
    byte[] bytes = Encoding.ASCII.GetBytes(value);
    for (int index = 0; index < bytes.Length; ++index)
    {
      byte num1 = xorSequence[index % xorSequence.Length];
      byte num2 = (byte) ((uint) bytes[index] ^ (uint) num1);
      bytes[index] = num2;
    }
    return Encoding.ASCII.GetString(bytes);
  }

  public static string RemoveSpaces(string str, bool atStart, bool atEnd)
  {
    if (str.Length == 0)
      return str;
    string str1 = str;
    if (atStart)
    {
      while (str1.Length > 0 && str1.StartsWith(" "))
        str1 = str1.Substring(1);
    }
    if (atEnd)
    {
      while (str1.Length > 0 && str1.EndsWith(" "))
        str1 = str1.Substring(0, str1.Length - 1);
    }
    return str1;
  }

  public static string? GetSaveBackup(string pathToNormalSave)
  {
    string path = pathToNormalSave + LauncherGlobals.SAVE_BAK_EXT;
    return File.Exists(path) ? path : (string) null;
  }

  private static void LookForOutdatedFiles(
    string directory,
    int lifetimeInDays,
    List<string>? collectorList)
  {
    foreach (string fileSystemEntry in Directory.GetFileSystemEntries(directory))
    {
      if (File.GetAttributes(fileSystemEntry).HasFlag((Enum) FileAttributes.Directory))
        LauncherUtils.LookForOutdatedFiles(fileSystemEntry, lifetimeInDays, collectorList);
      else if (DateTime.Now.Subtract(File.GetLastWriteTime(fileSystemEntry)).Days >= lifetimeInDays)
      {
        collectorList?.Add(fileSystemEntry);
        File.Delete(fileSystemEntry);
      }
    }
  }

  public static List<string> CleanupOutdatedDownloads(string downloadsDir, int LifetimeInDays)
  {
    List<string> collectorList = new List<string>();
    if (Path.Exists(downloadsDir))
    {
      try
      {
        LauncherUtils.LookForOutdatedFiles(downloadsDir, LifetimeInDays, collectorList);
      }
      catch (Exception ex)
      {
        return collectorList;
      }
    }
    return collectorList;
  }

  public static bool IsFileLocked(FileInfo file)
  {
    try
    {
      using (FileStream fileStream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
        fileStream.Close();
    }
    catch (IOException ex)
    {
      return true;
    }
    return false;
  }

  public static bool CanUseSaveFile(Launcher launcher, GameInstance? gameInstance)
  {
    if (gameInstance != null && launcher.currentGame == gameInstance)
      return false;
    string originalFilePath = gameInstance?.GetSaveInstance()?.GetOriginalFilePath();
    return originalFilePath == null || !LauncherUtils.IsFileLocked(new FileInfo(originalFilePath));
  }

  public static KeyValuePair<bool, string?> RestoreSaveFromBackup(
    string mainFilePath,
    string bakFilePath,
    Localizator? loc)
  {
    FileInfo file1 = new FileInfo(bakFilePath);
    if (!file1.Exists)
      return new KeyValuePair<bool, string>(false, (string) null);
    if (!LauncherUtils.IsFileLocked(file1))
    {
      FileInfo file2 = new FileInfo(mainFilePath);
      if (file2.Exists)
      {
        if (!LauncherUtils.IsFileLocked(file2))
        {
          File.Delete(mainFilePath);
          File.Copy(bakFilePath, mainFilePath);
          return new KeyValuePair<bool, string>(true, loc?.Get("successmsg_savecorrupted_restored"));
        }
        string str;
        if (loc == null)
          str = (string) null;
        else
          str = loc.Get("errormsg_savecorrupted_filelocked", new string[1]
          {
            mainFilePath
          });
        return new KeyValuePair<bool, string>(false, str);
      }
      File.Copy(bakFilePath, mainFilePath);
      return new KeyValuePair<bool, string>(true, (string) null);
    }
    string str1;
    if (loc == null)
      str1 = (string) null;
    else
      str1 = loc.Get("errormsg_savecorrupted_filelocked", new string[1]
      {
        bakFilePath
      });
    return new KeyValuePair<bool, string>(false, str1);
  }
}
