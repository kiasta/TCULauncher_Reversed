#nullable enable
namespace TCULauncher;

public class LauncherBoot
{
  public static TCUNet LoadTCUNet(bool forceOffline)
  {
    string rootUrl1 = "https://thecrewunlimited.com";
    string rootUrl2 = "https://the-crew-unlimited.github.io";
    TCUNet tcuNet1 = new TCUNet(rootUrl1, $"{rootUrl1}/{TCUNetUtils.TCUNET_INFO_FILE}", forceOffline);
    if (!forceOffline && tcuNet1.isOffline)
    {
      TCUNet tcuNet2 = new TCUNet(rootUrl2, $"{rootUrl2}/{TCUNetUtils.TCUNET_INFO_FILE}", false);
      if (!tcuNet2.isOffline)
        return tcuNet2;
    }
    return tcuNet1;
  }

  public static TCULauncherManifest? LoadLauncherManifest(TCUNet TcuNet)
  {
    return !TcuNet.isOffline ? TCULauncherManifest.LoadFromTCUNet(TcuNet) : (TCULauncherManifest) null;
  }

  public static TCUServerManifest? LoadServerManifest(TCUNet TcuNet)
  {
    return !TcuNet.isOffline ? TCUServerManifest.LoadFromTCUNet(TcuNet) : (TCUServerManifest) null;
  }
}
