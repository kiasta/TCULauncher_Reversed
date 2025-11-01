#nullable enable
namespace TCULauncher;

public class TestStuff
{
  public static bool convertJsonToBin(string inputFileJson, string destFileBin)
  {
    SaveInstance key = SaveManager.LoadServerSave(inputFileJson).Key;
    if (key != null)
    {
      key.SetUseCompression(true);
      key.SaveToFile(destFileBin);
      return true;
    }
    Console.Error.WriteLine("Invalid file path or save: " + inputFileJson);
    return false;
  }

  public static bool convertBinToJson(string inputFileBin, string destFileJson)
  {
    SaveInstance key = SaveManager.LoadServerSave(inputFileBin).Key;
    if (key != null)
    {
      key.SetUseCompression(false);
      key.SaveToFile(destFileJson);
      return true;
    }
    Console.Error.WriteLine("Invalid file path or save: " + inputFileBin);
    return false;
  }

  public static void reconvertBin(string inputFileBin, string destFilebin)
  {
    SaveInstance key = SaveManager.LoadServerSave(inputFileBin).Key;
    if (key != null)
      key.SaveToFile(destFilebin);
    else
      Console.Error.WriteLine("Invalid file path or save: " + inputFileBin);
  }
}
