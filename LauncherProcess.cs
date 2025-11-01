#nullable enable
namespace TCULauncher;

public class LauncherProcess
{
  private readonly string processName;
  private readonly LauncherProcessType processType;
  private byte progressPercent = byte.MaxValue;
  private readonly GameInstance? associatedInstance;
  private readonly Action? onStart;
  private readonly Action<int>? onUpdate;
  private readonly Action? onComplete;
  private readonly Action<string>? onFail;

  public LauncherProcess(
    string processName,
    LauncherProcessType type,
    GameInstance? associatedInstance,
    Action? onStart,
    Action<int>? onUpdate,
    Action? onComplete,
    Action<string>? onFail)
  {
    this.processName = processName;
    this.processType = type;
    this.associatedInstance = associatedInstance;
    this.onStart = onStart;
    this.onUpdate = onUpdate;
    this.onComplete = onComplete;
    this.onFail = onFail;
  }

  public string GetProcessName() => this.processName;

  public LauncherProcessType GetProcessType() => this.processType;

  public GameInstance? GetAssociatedInstance() => this.associatedInstance;

  public int GetProgress() => (int) this.progressPercent;

  public bool HasBegun()
  {
    return this.progressPercent != byte.MaxValue && this.progressPercent >= (byte) 0;
  }

  public bool HasEnded()
  {
    return this.progressPercent != byte.MaxValue && this.progressPercent >= (byte) 100;
  }

  public bool HasFailed()
  {
    return this.progressPercent != byte.MaxValue && this.progressPercent == (byte) 250;
  }

  public void Begin()
  {
    this.progressPercent = (byte) 0;
    Action onStart = this.onStart;
    if (onStart == null)
      return;
    onStart();
  }

  public void Update(int progressPercent)
  {
    this.progressPercent = (byte) progressPercent;
    Action<int> onUpdate = this.onUpdate;
    if (onUpdate == null)
      return;
    onUpdate(progressPercent);
  }

  public void Complete()
  {
    this.progressPercent = (byte) 100;
    Action onComplete = this.onComplete;
    if (onComplete == null)
      return;
    onComplete();
  }

  public void Fail(string reason)
  {
    this.progressPercent = (byte) 250;
    Action<string> onFail = this.onFail;
    if (onFail == null)
      return;
    onFail(reason);
  }
}
