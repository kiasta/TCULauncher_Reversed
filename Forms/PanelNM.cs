#nullable disable
namespace TCULauncher.Forms;

public class PanelNM : Panel
{
  protected override void WndProc(ref Message m)
  {
    if (m.Msg == 132)
      m.Result = new IntPtr(-1);
    else
      base.WndProc(ref m);
  }
}
