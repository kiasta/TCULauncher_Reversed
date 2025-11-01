using SevenZip;
using System.Security.Cryptography;
using System.Text;

#nullable enable
namespace TCULauncher;

public class TCUPatcher
{
  public static string GetPatchManifestDirectory(string gameDirectory)
  {
    return $"{gameDirectory}\\{LauncherGlobals.TCUPATCH_MANIFEST}";
  }

  public static TCUPatchManifest? GetPatchManifest(string gameDirectory)
  {
    string manifestDirectory = TCUPatcher.GetPatchManifestDirectory(gameDirectory);
    if (!File.Exists(manifestDirectory))
      return (TCUPatchManifest) null;
    TCUPatchManifest patchManifest = new TCUPatchManifest();
    patchManifest.ReadFromFile(manifestDirectory);
    return patchManifest;
  }

  public static TCUPatchManifest GenerateTCUPatchManifest(
    DateTime installedDate,
    GameInstance gameInstance,
    string tcuPatchVersion,
    List<string> filelist)
  {
    if (!filelist.Contains(LauncherGlobals.TCUPATCH_MANIFEST))
      filelist.Add(LauncherGlobals.TCUPATCH_MANIFEST);
    return new TCUPatchManifest(installedDate, gameInstance.GetGameVersion() ?? "0", tcuPatchVersion, filelist);
  }

  public static bool ValidatePatchFilesByManifest(string gameDirectory)
  {
    string manifestDirectory = TCUPatcher.GetPatchManifestDirectory(gameDirectory);
    if (!File.Exists(manifestDirectory))
      return false;
    TCUPatchManifest tcuPatchManifest = new TCUPatchManifest();
    tcuPatchManifest.ReadFromFile(manifestDirectory);
    foreach (string file in tcuPatchManifest.GetFileList())
    {
      if (!Path.Exists($"{gameDirectory}\\{file}"))
        return false;
    }
    return true;
  }

  public static bool IsDirectoryPatched(string? directoryToExe)
  {
    if (directoryToExe == null || directoryToExe.Length == 0)
      return false;
    string directoryName = Path.GetDirectoryName(directoryToExe);
    return directoryName != null && File.Exists($"{directoryName}\\{LauncherGlobals.TCUPATCH_MANIFEST}");
  }

  public static bool PatchDirectory(string? directoryToExe)
  {
    return directoryToExe != null && directoryToExe.Length != 0;
  }

  public static bool IsArchiveEncrypted(string archiveDir)
  {
    if (!File.Exists(archiveDir))
      return false;
    using (SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(archiveDir))
      return !sevenZipExtractor.Check();
  }

  public static bool CheckArchive(string archiveDir, string? password)
  {
    if (!File.Exists(archiveDir))
      return false;
    using (SevenZipExtractor sevenZipExtractor = password != null ? new SevenZipExtractor(archiveDir, password) : new SevenZipExtractor(archiveDir))
      return sevenZipExtractor.Check();
  }

  protected static bool DeployPatchZip(string fileDir, string extractPath, string? password)
  {
    try
    {
      using (SevenZipExtractor sevenZipExtractor = password != null ? new SevenZipExtractor(fileDir, password) : new SevenZipExtractor(fileDir))
        sevenZipExtractor.ExtractArchive(extractPath);
      Console.WriteLine("Extraction complete!");
      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine("Extraction error: " + ex.Message);
      return false;
    }
  }

  protected static List<string> GetFilesFromZip(string zipFileDir, string? password)
  {
    List<string> filesFromZip = new List<string>();
    using (SevenZipExtractor sevenZipExtractor = password != null ? new SevenZipExtractor(zipFileDir, password) : new SevenZipExtractor(zipFileDir))
    {
      sevenZipExtractor.Check();
      foreach (string archiveFileName in sevenZipExtractor.ArchiveFileNames)
        filesFromZip.Add(archiveFileName);
    }
    return filesFromZip;
  }

  protected static string ComputeSha256Hash(byte[] rawData)
  {
    using (SHA256 shA256 = SHA256.Create())
    {
      byte[] hash = shA256.ComputeHash(rawData);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("x2"));
      return stringBuilder.ToString();
    }
  }

  public static KeyValuePair<bool, string?> DeployPatch(
    Localizator loc,
    GameInstance gameInstance,
    string deployDir,
    string patchVersion,
    string? password)
  {
    string directoryPath = gameInstance.GetDirectoryPath();
    string gameVersion = gameInstance.GetGameVersion();
    if (directoryPath == null)
      return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_gameinstance_nopath", new string[1]
      {
        gameInstance.GetInstanceName()
      }));
    if (gameVersion == null)
      return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_gameinstance_invalid_version"));
    DirectoryInfo parent = Directory.GetParent(directoryPath);
    if (parent == null)
      return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_gameinstance_invalidpath", new string[1]
      {
        gameInstance.GetInstanceName()
      }));
    List<string> filesFromZip = TCUPatcher.GetFilesFromZip(deployDir, password);
    if (TCUPatcher.DeployPatchZip(deployDir, parent.FullName, password))
    {
      TCUPatcher.GenerateTCUPatchManifest(DateTime.Now, gameInstance, patchVersion, filesFromZip).WriteToFile($"{parent.FullName}\\{LauncherGlobals.TCUPATCH_MANIFEST}");
      return new KeyValuePair<bool, string>(true, loc.Get("successmsg_patch_installed", new string[1]
      {
        patchVersion
      }));
    }
    return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_deploy_failed", new string[1]
    {
      patchVersion
    }));
  }

  public static KeyValuePair<bool, string?> DownloadAndDeployPatch(
    Localizator loc,
    TCUNet tcuNet,
    TCUServerVersion patchVer,
    TCUServerManifest serverManifest,
    GameInstance gameInstance,
    string? password)
  {
    if (tcuNet.isOffline)
      return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_launcheroffline"));
    string directoryPath = gameInstance.GetDirectoryPath();
    string gameVersion = gameInstance.GetGameVersion();
    if (directoryPath == null)
      return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_gameinstance_nopath", new string[1]
      {
        gameInstance.GetInstanceName()
      }));
    if (gameVersion == null)
      return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_gameinstance_invalid_version"));
    if (Directory.GetParent(directoryPath) == null)
      return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_gameinstance_invalidpath", new string[1]
      {
        gameInstance.GetInstanceName()
      }));
    if (patchVer != null)
    {
      if (!Directory.Exists(Launcher.PatchesDir))
        Directory.CreateDirectory(Launcher.PatchesDir);
      string str = Launcher.PatchesDir + patchVer.GetFile();
      string upper = patchVer.GetHash()?.ToUpper();
      bool flag = false;
      if (LauncherGlobals.RETRIEVE_LOCAL_PATCH_DOWNLOADS && upper != null && File.Exists(str) && (TCUPatcher.ComputeSha256Hash(File.ReadAllBytes(str)).ToUpper().Equals(upper) || !LauncherGlobals.VERIFY_LOCAL_PATCH_DOWNLOADS))
        flag = true;
      string serverVersionUrl = tcuNet.GetResourceInfo()?.GetServerVersionURL(tcuNet.GetHttpClient(), tcuNet.GetRootUrl(), patchVer);
      if (serverVersionUrl != null)
      {
        using (Task<Stream> task = tcuNet.DownloadFile(serverVersionUrl))
        {
          if (task == null)
            return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_download_failed_completely"));
          try
          {
            using (FileStream destination = new FileStream(str, FileMode.Create))
              task.Result.CopyTo((Stream) destination);
          }
          catch (AggregateException ex)
          {
            return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_download_failed_verbose", new string[3]
            {
              patchVer.GetVersion(),
              gameVersion,
              ex.Message
            }));
          }
          flag = true;
        }
        if (flag)
        {
          if (!TCUPatcher.CheckArchive(str, password))
            return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_archivecheck_failed"));
          KeyValuePair<bool, string> keyValuePair = TCUPatcher.DeployPatch(loc, gameInstance, str, patchVer.GetVersion(), password);
          if ((!LauncherGlobals.STORE_PATCH_DOWNLOADS ? 1 : (!patchVer.IsEncrypted() ? 0 : (!LauncherGlobals.STORE_ENCRYPTED_PATCH_DOWNLOADS ? 1 : 0))) == 0 || !File.Exists(str))
            return keyValuePair;
          //File.Delete(str);
          return keyValuePair;
        }
        return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_download_failed", new string[1]
        {
          patchVer.GetVersion()
        }));
      }
      return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_download_missing", new string[1]
      {
        patchVer.GetVersion()
      }));
    }
    return new KeyValuePair<bool, string>(false, loc.Get("errormsg_patch_no_patch_found", new string[1]
    {
      gameVersion
    }));
  }
}
