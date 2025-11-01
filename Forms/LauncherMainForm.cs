using Microsoft.Web.WebView2.Core;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using TCULauncher.Properties;

#nullable enable
namespace TCULauncher.Forms;

public class LauncherMainForm : Form
{
    private static readonly GameInstance NOTHING_INSTANCE = new GameInstance();
    private Microsoft.Web.WebView2.WinForms.WebView2? NewsWebBrowser;
    private DateTime LastNewsRequest = new DateTime(2025, 1, 1);
    private readonly Launcher launcher;
    private bool promptedAboutLauncherUpdate;
    private bool promptedAboutSettingsChangeRestart;
    private const int cGrip = 16 /*0x10*/;
    private const int cCaption = 9999;
    private const int HTLEFT = 10;
    private const int HTRIGHT = 11;
    private const int HTTOP = 12;
    private const int HTTOPLEFT = 13;
    private const int HTTOPRIGHT = 14;
    private const int HTBOTTOM = 15;
    private const int HTBOTTOMLEFT = 16 /*0x10*/;
    private const int HTBOTTOMRIGHT = 17;
    private const int _ = 10;
    private readonly Dictionary<GameInstance, Panel> PanelByInstance = new Dictionary<GameInstance, Panel>();
    private readonly Dictionary<ServerInstance, Panel> PanelByServerInstance = new Dictionary<ServerInstance, Panel>();
    private
#nullable disable
    FontManager fontManager = new FontManager().SetupDefaultFonts();
    private IContainer components;
    private Panel MainPanel;
    private Panel InstanceListPanel;
    private Panel InstancePanel;
    private Button InstanceButton;
    private SplitContainer SplitPanels;
    private Label InstanceLabelBig;
    private Label InstanceVersion;
    private Button PlayButton;
    private Label InstancePatchStatus;
    private ProgressBar ProgressBar;
    private Panel InstancesOptionsPanel;
    private Button BtnGoToInstanceDir;
    private Button BtnRemoveInstance;
    private Button BtnNewInstance;
    private Button SettingsButton;
    private OpenFileDialog OpenFileGameExe;
    private FlowLayoutPanel InstancesListPanel;
    private Panel MainPagePanel;
    private Label MainPageHeader;
    private Panel ProgressBarPanel;
    private LinkLabel MainPageDesc;
    private Button NewsButton;
    private Button BtnMoveDown;
    private Button BtnMoveUp;
    private ToolTip ToolTip;
    private SplitContainer InstanceInfoPanel;
    private Label ServerListLabel;
    private Panel ServerList;
    private Panel ServerSample;
    private Button ServerButton;
    private Label ServerName;
    private Label ServerDescription;
    private Label SvPlayInfo;
    private Button BtnServerSelect;
    private Button BtnPatch;
    private ErrorProvider errorProvider1;
    private Button BtnManageServer;
    private Button DiscordButton;
    private Button InstructionsButton;
    private OpenFileDialog OpenManualPatch;
    private Button PatreonButton;
    private Button YoutubeButton;
    private Button TCUWebButton;
    private Button ExitButton;
    private Button MinimizeButton;
    private PictureBoxNM InstanceIconSmall;
    private PictureBoxNM InstanceIconBig;
    private LabelNM ProgressLabel;
    private PictureBoxNM InstancePatchStatusIcon;
    private PictureBoxNM IconBackdrop;
    private PanelNM InfoPanel;
    private LabelNM InfoLabel;
    private PictureBoxNM PlayArrowImg;
    private PictureBoxNM ServerIcon;
    private PictureBoxNM ServerIconBig;
    private FlowLayoutPanelNM BottomButtonsPanel;
    private PictureBoxNM InstancePlatformIcon;
    private PanelNM TopPanel;
    private PictureBoxNM TopIcon;
    private LabelNM TopLabel;
    private FlowLayoutPanelNM WindowControls;

    public LauncherMainForm(
#nullable enable
    Launcher launcher)
    {
        this.launcher = launcher;
        this.InitializeComponent();
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.ResizeRedraw, true);
        this.SetupStuff();
        this.RebuildInstancesPanels();
        this.GetOnlineNews();
        if (!launcher.userdata.IsShownLegalInfo())
        {
            int num = (int)launcher.LegalInfoForm().ShowDialog((IWin32Window)this);
        }
        this.ShowMainPage();
    }

    public int GetSplitH() => this.SplitPanels.SplitterDistance;

    public int GetSplitV() => this.InstanceInfoPanel.SplitterDistance;

    public void SetSplits(int h, int v)
    {
        if (h > 0)
            this.SplitPanels.SplitterDistance = h;
        if (v <= 0)
            return;
        this.InstanceInfoPanel.SplitterDistance = v;
    }

    private Rectangle Top => new Rectangle(0, 0, this.ClientSize.Width, 10);

    private Rectangle Left => new Rectangle(0, 0, 10, this.ClientSize.Height);

    private Rectangle Bottom
    {
        get
        {
            Size clientSize = this.ClientSize;
            int y = clientSize.Height - 10;
            clientSize = this.ClientSize;
            int width = clientSize.Width;
            return new Rectangle(0, y, width, 10);
        }
    }

    private Rectangle Right
    {
        get
        {
            Size clientSize = this.ClientSize;
            int x = clientSize.Width - 10;
            clientSize = this.ClientSize;
            int height = clientSize.Height;
            return new Rectangle(x, 0, 10, height);
        }
    }

    private Rectangle TopLeft => new Rectangle(0, 0, 10, 10);

    private Rectangle TopRight => new Rectangle(this.ClientSize.Width - 10, 0, 10, 10);

    private Rectangle BottomLeft => new Rectangle(0, this.ClientSize.Height - 10, 10, 10);

    private Rectangle BottomRight
    {
        get
        {
            Size clientSize = this.ClientSize;
            int x = clientSize.Width - 10;
            clientSize = this.ClientSize;
            int y = clientSize.Height - 10;
            return new Rectangle(x, y, 10, 10);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Size clientSize = this.ClientSize;
        int x = clientSize.Width - 16;
        int y = clientSize.Height - 16;
        Rectangle bounds = new Rectangle(x, y, 16, 16);
        ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, bounds);
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg != 132)
            return;
        Point client = this.PointToClient(Cursor.Position);
        bool flag = this.WindowState == FormWindowState.Maximized;
        if (!flag && this.TopLeft.Contains(client))
            m.Result = new IntPtr(13);
        else if (!flag && this.TopRight.Contains(client))
            m.Result = new IntPtr(14);
        else if (!flag && this.BottomLeft.Contains(client))
            m.Result = new IntPtr(16);
        else if (!flag && this.BottomRight.Contains(client))
            m.Result = new IntPtr(17);
        else if (!flag && this.Top.Contains(client))
            m.Result = new IntPtr(12);
        else if (!flag && this.Left.Contains(client))
            m.Result = new IntPtr(10);
        else if (!flag && this.Right.Contains(client))
            m.Result = new IntPtr(11);
        else if (!flag && this.Bottom.Contains(client))
        {
            m.Result = new IntPtr(15);
        }
        else
        {
            if (client.Y >= 9999)
                return;
            m.Result = new IntPtr(2);
        }
    }

    private void SetupStuff()
    {
        this.Icon = Resources.IconTCU;
        this.Text = $"{Resources.ProjectName} Launcher {Launcher.VERSION}";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.PlayButton.Text = this.launcher.loc.Get("play");
        this.ServerListLabel.Text = this.launcher.loc.Get("server_list");
        this.BtnManageServer.Text = this.launcher.loc.Get("server_manage");
        this.InstancePanel.Visible = false;
        this.ServerSample.Visible = false;
        this.InstancesListPanel.Controls.Remove((Control)this.InstancePanel);
        this.InstancesListPanel.AutoScroll = false;
        this.InstancesListPanel.HorizontalScroll.Enabled = false;
        this.InstancesListPanel.HorizontalScroll.Visible = false;
        this.InstancesListPanel.HorizontalScroll.Maximum = 0;
        this.InstancesListPanel.AutoScroll = true;
        this.InstancesListPanel.SizeChanged += (EventHandler)((sender, e) =>
        {
            foreach (Panel panel in this.PanelByInstance.Values)
            {
                panel.Size = new Size(this.InstancesListPanel.Width - 1, panel.Size.Height);
                if (panel.Controls.Count > 1)
                    panel.Controls[1].Size = panel.Size;
            }
        });
        this.InfoPanel.Visible = true;
        this.ProgressBarPanel.Visible = false;
        this.OpenManualPatch.Multiselect = false;
        this.OpenManualPatch.FileName = "";
        this.OpenManualPatch.Filter = "TCU Patch Archive|" + LauncherGlobals.MANUAL_PATCH_EXTENSIONS;
        this.MainPageHeader.Text = this.launcher.loc.Get("welcome_to_tcu", new string[1]
        {
      Resources.ProjectName
        });
        this.MainPagePanel.SizeChanged += (EventHandler)((sender, e) =>
        {
            if (this.NewsWebBrowser == null)
                return;
            this.NewsWebBrowser.Size = this.MainPagePanel.Size;
        });
        Point location1 = this.InstanceIconBig.Location;
        int x1 = location1.X;
        location1 = this.IconBackdrop.Location;
        int x2 = location1.X;
        int x3 = x1 - x2;
        Point location2 = this.InstanceIconBig.Location;
        int y1 = location2.Y;
        location2 = this.IconBackdrop.Location;
        int y2 = location2.Y;
        int y3 = y1 - y2;
        this.InstanceIconBig.Parent = (Control)this.IconBackdrop;
        this.InstanceIconBig.Location = new Point(x3, y3);
        if (this.launcher.userdata.GetWidth() > 0 && this.launcher.userdata.GetHeight() > 0)
            this.Size = new Size(this.launcher.userdata.GetWidth(), this.launcher.userdata.GetHeight());
        this.SetSplits(this.launcher.userdata.GetSplitH(), this.launcher.userdata.GetSplitV());
        if (this.launcher.userdata.IsMaximized())
            this.WindowState = FormWindowState.Maximized;
        this.MinimizeButton.Click += (EventHandler)((sender, e) => this.WindowState = FormWindowState.Minimized);
        this.ExitButton.Click += (EventHandler)((sender, e) =>
        {
            Action close = new Action(((Form)this).Close);
            if (this.launcher.processes.Count > 0)
            {
                ConfirmForm dialogForm = (ConfirmForm)null;
                string textHeader1 = this.launcher.loc.Get("processes_ongoing");
                string textDesc1 = this.launcher.loc.Get("processes_ongoing_details");
                Bitmap icDboxWarning1 = Resources.ic_dbox_warning;
                List<ButtonAction> buttonActionList1 = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList1, 2);
                Span<ButtonAction> span1 = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList1);
                int index3 = 0;
                span1[index3] = new ButtonAction(this.launcher.loc.Get("exit_anyway"), (Action)(() =>
            {
                if (this.launcher.processes.Count > 0)
                {
                    dialogForm?.Close();
                    string textHeader2 = this.launcher.loc.Get("warning");
                    string textDesc2 = this.launcher.loc.Get("processes_ongoing_exit_details");
                    Bitmap icDboxWarning2 = Resources.ic_dbox_warning;
                    List<ButtonAction> buttonActionList2 = new List<ButtonAction>();
                    CollectionsMarshal.SetCount<ButtonAction>(buttonActionList2, 2);
                    Span<ButtonAction> span2 = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList2);
                    int index4 = 0;
                    span2[index4] = new ButtonAction(this.launcher.loc.Get("yes"), close);
                    int index5 = index4 + 1;
                    span2[index5] = new ButtonAction(this.launcher.loc.Get("cancel"));
                    int num3 = index5 + 1;
                    int num4 = (int)new ConfirmForm(textHeader2, textDesc2, (Image)icDboxWarning2, buttonActionList2).ShowDialog((IWin32Window)this);
                }
                else
                    close();
            }));
                int index6 = index3 + 1;
                span1[index6] = new ButtonAction(this.launcher.loc.Get("cancel"));
                int num5 = index6 + 1;
                dialogForm = new ConfirmForm(textHeader1, textDesc1, (Image)icDboxWarning1, buttonActionList1);
                int num6 = (int)dialogForm.ShowDialog((IWin32Window)this);
            }
            else
                close();
        });
        TCUNetResourceInfo resourceInfo = this.launcher.TcuNet?.GetResourceInfo();
        string tcuwebLink = resourceInfo?.GetTCUWebsiteLink();
        string discordLink = resourceInfo?.GetDiscordLink();
        string patreonLink = resourceInfo?.GetPatreonLink();
        string youtubeLink = resourceInfo?.GetYouTubeLink();
        this.TCUWebButton.Visible = tcuwebLink != null;
        this.TCUWebButton.Click += (EventHandler)((sender, e) =>
        {
            if (tcuwebLink == null)
                return;
            string textHeader = this.launcher.loc.Get("opening_tcuwebsite");
            string textDesc = this.launcher.loc.Get("ext_link_traversal_details", new string[1]
        {
        tcuwebLink
          });
            Bitmap tcuwebsiteLogo = Resources.tcuwebsite_logo;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index7 = 0;
            span[index7] = new ButtonAction(this.launcher.loc.Get("ok"), (Action)(() => Process.Start(new ProcessStartInfo(tcuwebLink)
            {
                UseShellExecute = true
            })));
            int index8 = index7 + 1;
            span[index8] = new ButtonAction(this.launcher.loc.Get("cancel"));
            int num7 = index8 + 1;
            int num8 = (int)new ConfirmForm(textHeader, textDesc, (Image)tcuwebsiteLogo, buttonActionList).ShowDialog((IWin32Window)this);
        });
        this.DiscordButton.Visible = discordLink != null;
        this.DiscordButton.Click += (EventHandler)((sender, e) =>
        {
            if (discordLink == null)
                return;
            string textHeader = this.launcher.loc.Get("opening_discord");
            string textDesc = this.launcher.loc.Get("ext_link_traversal_details", new string[1]
        {
        discordLink
          });
            Bitmap discordSymbolWhite = Resources.Discord_Symbol_White;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index9 = 0;
            span[index9] = new ButtonAction(this.launcher.loc.Get("ok"), (Action)(() => Process.Start(new ProcessStartInfo(discordLink)
            {
                UseShellExecute = true
            })));
            int index10 = index9 + 1;
            span[index10] = new ButtonAction(this.launcher.loc.Get("cancel"));
            int num9 = index10 + 1;
            int num10 = (int)new ConfirmForm(textHeader, textDesc, (Image)discordSymbolWhite, buttonActionList).ShowDialog((IWin32Window)this);
        });
        this.PatreonButton.Visible = patreonLink != null;
        this.PatreonButton.Click += (EventHandler)((sender, e) =>
        {
            if (patreonLink == null)
                return;
            string textHeader = this.launcher.loc.Get("opening_patreon");
            string textDesc = this.launcher.loc.Get("ext_link_traversal_details", new string[1]
        {
        patreonLink
          });
            Bitmap patreonWhite = Resources.patreon_white;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index11 = 0;
            span[index11] = new ButtonAction(this.launcher.loc.Get("ok"), (Action)(() => Process.Start(new ProcessStartInfo(patreonLink)
            {
                UseShellExecute = true
            })));
            int index12 = index11 + 1;
            span[index12] = new ButtonAction(this.launcher.loc.Get("cancel"));
            int num11 = index12 + 1;
            int num12 = (int)new ConfirmForm(textHeader, textDesc, (Image)patreonWhite, buttonActionList).ShowDialog((IWin32Window)this);
        });
        this.YoutubeButton.Visible = youtubeLink != null;
        this.YoutubeButton.Click += (EventHandler)((sender, e) =>
        {
            if (youtubeLink == null)
                return;
            string textHeader = this.launcher.loc.Get("opening_youtube");
            string textDesc = this.launcher.loc.Get("ext_link_traversal_details", new string[1]
        {
        youtubeLink
          });
            Bitmap youtubeLogo = Resources.youtube_logo;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index13 = 0;
            span[index13] = new ButtonAction(this.launcher.loc.Get("ok"), (Action)(() => Process.Start(new ProcessStartInfo(youtubeLink)
            {
                UseShellExecute = true
            })));
            int index14 = index13 + 1;
            span[index14] = new ButtonAction(this.launcher.loc.Get("cancel"));
            int num13 = index14 + 1;
            int num14 = (int)new ConfirmForm(textHeader, textDesc, (Image)youtubeLogo, buttonActionList).ShowDialog((IWin32Window)this);
        });
        this.SettingsButton.Click += (EventHandler)((sender, e) =>
        {
            Action showSettings = (Action)null;
            showSettings = (Action)(() =>
        {
            bool flag = this.launcher.userdata.IsOfflineMode();
            string str3 = this.launcher.userdata.GetLang() ?? Resources.DefaultLanguage;
            int num18 = (int)new SettingsForm(this.launcher, showSettings).ShowDialog((IWin32Window)this);
            string str4 = this.launcher.userdata.GetLang() ?? Resources.DefaultLanguage;
            if (!this.promptedAboutSettingsChangeRestart & (!str3.Equals(str4) || flag != this.launcher.userdata.IsOfflineMode()))
            {
                this.promptedAboutSettingsChangeRestart = true;
                string textHeader = this.launcher.loc.Get("restart");
                string textDesc = this.launcher.loc.Get("settings_restart_changes_details");
                Bitmap icDboxInfo = Resources.ic_dbox_info;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                int num19 = index + 1;
                int num20 = (int)new ConfirmForm(textHeader, textDesc, (Image)icDboxInfo, buttonActionList).ShowDialog((IWin32Window)this);
            }
            this.launcher.SaveUserData();
            if (this.launcher.currentInstance == -1 || this.launcher.currentInstance >= this.launcher.instances.Count)
                return;
            this.UpdateInstancePage(this.launcher.instances[this.launcher.currentInstance]);
        });
            showSettings();
        });
        this.InstructionsButton.Click += (EventHandler)((sender, e) =>
        {
            string textHeader = this.launcher.loc.Get("title_how_to_use");
            string textDesc = this.launcher.loc.Get("launcher_instructions", new string[3]
        {
        this.launcher.loc.Get("instance_patch"),
        this.launcher.loc.Get("play"),
        LauncherGlobals.TCUPATCH_MANIFEST
          });
            Bitmap icDboxInfo = Resources.ic_dbox_info;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            int num21 = index + 1;
            int num22 = (int)new ConfirmForm(textHeader, textDesc, (Image)icDboxInfo, buttonActionList).ShowDialog((IWin32Window)this);
        });
        this.UpdateAppInfo();
        this.ToolTip.SetToolTip((Control)this.BtnNewInstance, this.launcher.loc.Get("tooltip_instance_new"));
        this.ToolTip.SetToolTip((Control)this.BtnRemoveInstance, this.launcher.loc.Get("tooltip_instance_remove"));
        this.ToolTip.SetToolTip((Control)this.BtnMoveUp, this.launcher.loc.Get("tooltip_instance_move_up"));
        this.ToolTip.SetToolTip((Control)this.BtnMoveDown, this.launcher.loc.Get("tooltip_instance_move_down"));
        this.ToolTip.SetToolTip((Control)this.BtnGoToInstanceDir, this.launcher.loc.Get("tooltip_instance_opendir"));
        this.ToolTip.SetToolTip((Control)this.NewsButton, this.launcher.loc.Get("tooltip_btn_news"));
        this.ToolTip.SetToolTip((Control)this.SettingsButton, this.launcher.loc.Get("tooltip_btn_settings"));
        this.ToolTip.SetToolTip((Control)this.TCUWebButton, this.launcher.loc.Get("tooltip_btn_tcuwebsite"));
        this.ToolTip.SetToolTip((Control)this.DiscordButton, this.launcher.loc.Get("tooltip_btn_discord", new string[1]
        {
      "Discord"
        }));
        this.ToolTip.SetToolTip((Control)this.PatreonButton, this.launcher.loc.Get("tooltip_btn_patreon", new string[1]
        {
      "Patreon"
        }));
        this.ToolTip.SetToolTip((Control)this.YoutubeButton, this.launcher.loc.Get("tooltip_btn_youtube", new string[1]
        {
      "YouTube"
        }));
        this.ToolTip.SetToolTip((Control)this.InstructionsButton, this.launcher.loc.Get("tooltip_instructions"));
        this.fontManager?.OverrideFonts((Control)this, new bool?(true));
    }

    protected void GetOnlineNews()
    {
        if (this.NewsWebBrowser != null)
        {
            this.NewsWebBrowser.Parent?.Controls.Remove((Control)this.NewsWebBrowser);
            this.NewsWebBrowser = (Microsoft.Web.WebView2.WinForms.WebView2)null;
        }
        string newsList = !this.launcher.OfflineMode ? this.launcher.TcuNet.GetResourceInfo()?.GetNewsList(this.launcher.TcuNet.GetHttpClient(), this.launcher.TcuNet.GetRootUrl()) : (string)null;
        if (!this.launcher.OfflineMode && newsList == null)
        {
            bool flag = this.launcher.TcuNet.SanityCheck();
            this.launcher.OfflineMode = !flag;
            if (!flag)
            {
                string textHeader = this.launcher.loc.Get("is_error");
                string textDesc = this.launcher.loc.Get("errormsg_no_connection_to_tcunet", new string[1]
                {
          Resources.NetworkTitle
                });
                Bitmap offline = Resources.offline;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                int num1 = index + 1;
                int num2 = (int)new ConfirmForm(textHeader, textDesc, (Image)offline, buttonActionList).ShowDialog((IWin32Window)this);
            }
        }
        string str1;
        if (newsList != null)
        {
            JsonNode jsonRoot = JsonSerializer.Deserialize<JsonNode>(newsList);
            str1 = jsonRoot == null ? (string)null : new TCUNewsList(jsonRoot).GetLatestNewsFile();
        }
        else
            str1 = (string)null;
        if (str1 != null)
        {
            this.NewsWebBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.NewsWebBrowser.Source = new Uri($"{this.launcher.TcuNet.GetRootUrl()}/news_html/?post={str1}");
            this.NewsWebBrowser.Size = this.MainPagePanel.Size;
            this.NewsWebBrowser.DefaultBackgroundColor = Color.Transparent;
            this.MainPagePanel.Controls.Add((Control)this.NewsWebBrowser);
            Action<bool> ToggleWebPage = (Action<bool>)(loaded =>
            {
                this.MainPageHeader.Visible = !loaded;
                this.MainPageDesc.Visible = !loaded;
                this.NewsWebBrowser.Visible = loaded;
                if (loaded)
                    return;
                this.MainPageHeader.Text = this.launcher.loc.Get("tcunet_getting_news");
                this.MainPageDesc.Text = this.launcher.loc.Get("tcunet_connecting_hold");
            });
            ToggleWebPage(false);
            this.NewsWebBrowser.NavigationStarting += (EventHandler<CoreWebView2NavigationStartingEventArgs>)((sender, e) =>
            {
                if ("about:blank".Equals(e.Uri.ToLower()))
                {
                    e.Cancel = true;
                }
                else
                {
                    bool flag = false;
                    foreach (string str2 in TCUNetUtils.TCUNET_HOSTNAMES)
                    {
                        if (e.Uri.StartsWith(str2))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        return;
                    Process.Start(new ProcessStartInfo(e.Uri)
                    {
                        UseShellExecute = true
                    });
                    e.Cancel = true;
                }
            });
            this.NewsWebBrowser.ContentLoading += (EventHandler<CoreWebView2ContentLoadingEventArgs>)((sender, e) => ToggleWebPage(true));
        }
        else
        {
            this.MainPageHeader.Text = this.launcher.OfflineMode ? this.launcher.loc.Get("tcunet_offline") : this.launcher.loc.Get("tcunet_failed_to_get_news");
            this.MainPageDesc.Text = this.launcher.OfflineMode ? this.launcher.loc.Get("tcunet_offline_details") : this.launcher.loc.Get("tcunet_failed_to_get_news_details");
            this.MainPageHeader.Visible = false;
            this.MainPageDesc.Visible = false;
        }
    }

    protected void GetOnlineNewsOld()
    {
        if (this.NewsWebBrowser != null)
        {
            this.NewsWebBrowser.Parent?.Controls.Remove((Control)this.NewsWebBrowser);
            this.NewsWebBrowser = (Microsoft.Web.WebView2.WinForms.WebView2)null;
        }
        string uriString = (string)null;
        if (!this.launcher.OfflineMode && uriString == null)
        {
            bool flag = this.launcher.TcuNet.SanityCheck();
            this.launcher.OfflineMode = !flag;
            if (!flag)
            {
                string textHeader = this.launcher.loc.Get("is_error");
                string textDesc = this.launcher.loc.Get("errormsg_no_connection_to_tcunet", new string[1]
                {
          Resources.NetworkTitle
                });
                Bitmap offline = Resources.offline;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                int num1 = index + 1;
                int num2 = (int)new ConfirmForm(textHeader, textDesc, (Image)offline, buttonActionList).ShowDialog((IWin32Window)this);
            }
        }
        if (uriString != null)
        {
            this.NewsWebBrowser = new Microsoft.Web.WebView2.WinForms.WebView2();
            Uri newsUri = new Uri(uriString);
            this.NewsWebBrowser.Source = newsUri;
            this.NewsWebBrowser.Size = this.MainPagePanel.Size;
            this.NewsWebBrowser.DefaultBackgroundColor = Color.Transparent;
            this.MainPagePanel.Controls.Add((Control)this.NewsWebBrowser);
            Action<bool> ToggleWebPage = (Action<bool>)(loaded =>
            {
                this.MainPageHeader.Visible = !loaded;
                this.MainPageDesc.Visible = !loaded;
                this.NewsWebBrowser.Visible = loaded;
                if (loaded)
                    return;
                this.MainPageHeader.Text = this.launcher.loc.Get("tcunet_getting_news");
                this.MainPageDesc.Text = this.launcher.loc.Get("tcunet_connecting_hold");
            });
            ToggleWebPage(false);
            this.NewsWebBrowser.NavigationStarting += (EventHandler<CoreWebView2NavigationStartingEventArgs>)((sender, e) =>
            {
                if ("about:blank".Equals(e.Uri.ToLower()))
                {
                    e.Cancel = true;
                }
                else
                {
                    if (e.Uri.Equals((object)newsUri))
                        return;
                    Process.Start(new ProcessStartInfo(e.Uri)
                    {
                        UseShellExecute = true
                    });
                    e.Cancel = true;
                }
            });
            this.NewsWebBrowser.ContentLoading += (EventHandler<CoreWebView2ContentLoadingEventArgs>)((sender, e) => ToggleWebPage(true));
        }
        else
        {
            this.MainPageHeader.Text = this.launcher.loc.Get("tcunet_offline");
            this.MainPageDesc.Text = this.launcher.loc.Get("tcunet_offline_details");
            this.MainPageHeader.Visible = true;
            this.MainPageDesc.Visible = true;
        }
    }

    public void RebuildInstancesPanels()
    {
        this.PanelByInstance.Clear();
        this.InstancesListPanel.Controls.Clear();
        if (this.launcher.instances.Count > 0)
        {
            for (int index = 0; index < this.launcher.instances.Count; ++index)
                this.CreateNewInstancePanel(this.launcher.instances[index], index);
        }
        else
            this.CreateNewInstancePanel((GameInstance)null, 0);
    }

    public void RebuildServerInstancePanels(
      GameInstance gameInstance,
      List<ServerInstance> serverList)
    {
        this.PanelByServerInstance.Clear();
        this.ServerList.Controls.Clear();
        if ((gameInstance.IsPatched() || gameInstance.IsForcePatched() ? 1 : (this.launcher.ServerManifest == null ? 0 : (this.launcher.ServerManifest.PickLatestForGameVersion(gameInstance.GetGameVersion()) != null ? 1 : 0))) != 0)
            this.CreateNewServerInstancePanel(ServerInstance.SERVER_LOCAL, 0);
        if (serverList.Count <= 0)
            return;
        for (int index = 0; index < serverList.Count; ++index)
            this.CreateNewServerInstancePanel(serverList[index], index + 1);
    }

    private string GameInstanceToHash(GameInstance gameInstance)
    {
        return gameInstance.GetHashCode().ToString("X");
    }

    private string ServerInstanceToHash(ServerInstance serverInstance)
    {
        return serverInstance.GetHashCode().ToString("X");
    }

    public void CreateNewInstancePanel(GameInstance? gameInstance, int index)
    {
        Panel panel = new Panel();
        PictureBox pictureBox = new PictureBox();
        Button button = new Button();
        string str = gameInstance != null ? this.GameInstanceToHash(gameInstance) : "add_new";
        panel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        panel.Controls.Add((Control)pictureBox);
        panel.Controls.Add((Control)button);
        panel.Margin = new Padding(0);
        panel.Name = "InstancePanel" + str;
        panel.Size = new Size(175, 37);
        panel.BackColor = Color.Transparent;
        panel.TabIndex = index;
        pictureBox.BackColor = Color.Transparent;
        pictureBox.Enabled = false;
        pictureBox.Image = gameInstance != null ? gameInstance.GetInstanceIcon() : (Image)Resources.icon_add;
        pictureBox.Location = new Point(6, 6);
        pictureBox.Name = "InstanceIconSmall" + str;
        pictureBox.Size = new Size(24, 24);
        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox.TabIndex = 0;
        pictureBox.TabStop = false;
        button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        button.AutoEllipsis = true;
        button.BackColor = Color.FromArgb(5, 173, 241);
        button.FlatStyle = FlatStyle.Flat;
        button.Font = new Font("TheCrew Sans Regular", 12f);
        button.Location = new Point(0, 0);
        button.Name = "InstanceButton" + str;
        button.Margin = new Padding(0);
        button.Padding = new Padding(28, 0, 0, 0);
        button.Size = new Size(175, 37);
        button.Text = gameInstance != null ? gameInstance.GetInstanceName() : this.launcher.loc.Get("instance_new");
        button.TextAlign = ContentAlignment.MiddleLeft;
        if (gameInstance != null)
            button.Click += (EventHandler)((sender, e) => this.launcher.SelectInstance(index));
        else
            button.Click += (EventHandler)((sender, e) => this.BtnNewInstance.PerformClick());
        panel.Size = new Size(this.InstancesListPanel.Width - 1, panel.Size.Height);
        button.Size = panel.Size;
        pictureBox.Parent = (Control)button;
        pictureBox.Location = new Point(6, 6);
        this.fontManager?.OverrideFonts((Control)panel, new bool?(true));
        this.InstancesListPanel.Controls.Add((Control)panel);
        if (gameInstance != null)
        {
            this.PanelByInstance[gameInstance] = panel;
            this.UpdateInstanceInfo(gameInstance);
        }
        else
            this.PanelByInstance[LauncherMainForm.NOTHING_INSTANCE] = panel;
        this.UpdateAppInfo();
    }

    public void RemoveInstancePanel(GameInstance gameInstance)
    {
        Panel panel = this.PanelByInstance[gameInstance];
        if (panel != null)
            this.InstancesListPanel.Controls.Remove((Control)panel);
        this.UpdateAppInfo();
    }

    public void ShowMainPage()
    {
        this.MainPagePanel.Visible = true;
        this.InstanceInfoPanel.Visible = false;
        DateTime now = DateTime.Now;
        if ((now - this.LastNewsRequest).TotalSeconds > (double)LauncherGlobals.TCUNET_NEWS_REQUEST_FREQ)
        {
            this.GetOnlineNews();
            this.LastNewsRequest = now;
        }
        this.UpdateAppInfo();
    }

    public void ShowInstancePage(GameInstance instance)
    {
        this.MainPagePanel.Visible = false;
        this.InstanceInfoPanel.Visible = true;
        this.UpdateInstancePage(instance);
    }

    public void UpdateAppInfo()
    {
        this.TopLabel.Text = this.Text;
        this.TopIcon.Image = (Image)this.Icon?.ToBitmap();
        this.InfoLabel.Text = "";
        if (this.launcher.TcuNet.isOffline)
        {
            LabelNM infoLabel = this.InfoLabel;
            infoLabel.Text = $"{infoLabel.Text}[{this.launcher.loc.Get("is_offline").ToUpper()}] ";
        }
        LabelNM infoLabel1 = this.InfoLabel;
        string text = infoLabel1.Text;
        Localizator loc = this.launcher.loc;
        string[] args = new string[3]
        {
      Resources.ProjectName,
      Launcher.VERSION,
      null
        };
        int index = this.launcher.instances.Count;
        args[2] = index.ToString();
        string str = loc.Get("status_bar", args);
        infoLabel1.Text = text + str;
        if (this.launcher.TcuNet.isOffline || !this.launcher.IsLauncherOutdated() || this.launcher.LauncherManifest == null)
            return;
        LabelNM infoLabel2 = this.InfoLabel;
        infoLabel2.Text = $"{infoLabel2.Text} {this.launcher.loc.Get("status_bar_update_pending", new string[1]
        {
      this.launcher.LauncherManifest.GetLatestLauncher()
        })}";
        if (this.promptedAboutLauncherUpdate || !this.launcher.userdata.IsLauncherUpdatePromptsEnabled())
            return;
        this.promptedAboutLauncherUpdate = true;
        string textHeader = this.launcher.loc.Get("new_update");
        string textDesc = this.launcher.loc.Get("new_update_details", new string[1]
        {
      this.launcher.LauncherManifest.GetLatestLauncher()
        });
        Bitmap icDboxInfo = Resources.ic_dbox_info;
        List<ButtonAction> buttonActionList = new List<ButtonAction>();
        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
        index = 0;
        span[index] = new ButtonAction(this.launcher.loc.Get("yes"), new Action(this.launcher.UpdateLauncher));
        ++index;
        span[index] = new ButtonAction(this.launcher.loc.Get("no"));
        ++index;
        int num = (int)new ConfirmForm(textHeader, textDesc, (Image)icDboxInfo, buttonActionList).ShowDialog((IWin32Window)this);
    }

    public void UpdateInstancePage(GameInstance? instance)
    {
        if (this.launcher.currentGame == null && this.launcher.userdata.IsScanForRunningInstances())
            this.launcher.DetectAlreadyRunningInstances();
        if (instance == null)
            return;
        bool flag1 = LauncherUtils.IsSteamVersion(instance.GetDirectoryPath());
        string customExe = instance.GetCustomEXE();
        bool flag2 = flag1 && customExe == null;
        instance.UpdateInstanceInfo(true);
        this.InstanceLabelBig.Text = instance.GetInstanceName();
        this.InstanceVersion.Text = instance.GetGameVersion();
        this.InstanceIconBig.Image = instance.GetInstanceIcon();
        this.InstancePlatformIcon.Image = flag1 ? (Image)Resources.platform_steam : (Image)Resources.platform_ubi;
        ToolTip toolTip = this.ToolTip;
        PictureBoxNM instancePlatformIcon = this.InstancePlatformIcon;
        string caption;
        if (!flag1)
            caption = this.launcher.loc.Get("tooltip_platform_ubi", new string[1]
            {
        "Ubisoft Connect"
            });
        else
            caption = this.launcher.loc.Get("tooltip_platform_steam", new string[1]
            {
        "Steam"
            });
        toolTip.SetToolTip((Control)instancePlatformIcon, caption);
        bool flag3 = false;
        foreach (LauncherProcess process in this.launcher.processes)
        {
            if (process.GetAssociatedInstance() == instance && process.GetProcessType() == LauncherProcessType.Patch)
            {
                flag3 = true;
                break;
            }
        }
        string directoryName = Path.GetDirectoryName(instance.GetDirectoryPath());
        TCUPatchManifest patchManifest = directoryName != null ? TCUPatcher.GetPatchManifest(directoryName) : (TCUPatchManifest)null;
        TCUServerManifest serverManifest = this.launcher.ServerManifest;
        TCUServerVersion tcuServerVersion1 = serverManifest?.PickLatestForGameVersion(instance.GetGameVersion());
        bool flag4 = instance.IsPatched() || instance.IsForcePatched() || tcuServerVersion1 != null;
        if (flag3)
        {
            this.InstancePatchStatus.Text = this.launcher.loc.Get("instance_unpatched");
            this.InstancePatchStatusIcon.Image = (Image)Resources.ic_refuse_;
            this.BtnPatch.Visible = true;
            this.BtnPatch.Enabled = false;
            this.BtnPatch.Text = this.launcher.loc.Get("instance_patching");
            this.PlayButton.Enabled = false;
        }
        else
        {
            this.PlayButton.Enabled = this.launcher.currentGame == null;
            this.PlayButton.Text = this.launcher.currentGame == instance ? this.launcher.loc.Get("already_running") : this.launcher.loc.Get(flag2 ? "play_on_steam" : "play");
            if (!flag4)
            {
                this.InstancePatchStatus.Text = this.launcher.loc.Get("instance_unpatched");
                this.InstancePatchStatusIcon.Image = (Image)Resources.ic_dbox_warning;
                this.BtnPatch.Visible = true;
                this.BtnPatch.Enabled = true;
                this.BtnPatch.Text = this.launcher.loc.Get("instance_patch");
            }
            else if (instance.IsPatched() || instance.IsForcePatched())
            {
                TCUServerVersion tcuServerVersion2 = patchManifest != null ? serverManifest?.IsPatchOutdated(instance.GetGameVersion(), patchManifest) : (TCUServerVersion)null;
                if ((directoryName != null ? (TCUPatcher.ValidatePatchFilesByManifest(directoryName) ? 1 : 0) : 1) == 0)
                {
                    this.InstancePatchStatus.Text = this.launcher.loc.Get("instance_patch_invalid");
                    this.InstancePatchStatusIcon.Image = (Image)Resources.ic_dbox_warning;
                    this.BtnPatch.Visible = true;
                    this.BtnPatch.Enabled = true;
                    this.BtnPatch.Text = this.launcher.loc.Get("instance_patch_update");
                }
                else if (!instance.IsForcePatched() && tcuServerVersion2 != null)
                {
                    this.InstancePatchStatus.Text = this.launcher.loc.Get("instance_patch_update_available");
                    this.InstancePatchStatusIcon.Image = (Image)Resources.ic_dbox_info;
                    this.BtnPatch.Visible = true;
                    this.BtnPatch.Enabled = true;
                    this.BtnPatch.Text = this.launcher.loc.Get("instance_patch_update");
                }
                else
                {
                    this.InstancePatchStatus.Text = this.launcher.loc.Get("instance_patched");
                    this.InstancePatchStatusIcon.Image = (Image)Resources.check;
                    this.BtnPatch.Visible = false;
                    this.BtnPatch.Enabled = false;
                    this.BtnPatch.Text = this.launcher.loc.Get("instance_patched");
                }
            }
            else
            {
                this.InstancePatchStatus.Text = this.launcher.loc.Get("instance_unpatched");
                this.InstancePatchStatusIcon.Image = (Image)Resources.ic_refuse_;
                this.BtnPatch.Visible = true;
                this.BtnPatch.Enabled = true;
                this.BtnPatch.Text = this.launcher.loc.Get("instance_patch");
            }
        }
        this.LoadServerList(instance);
        this.UpdateSelectedServer(instance, this.launcher.GetServerInstance(instance.GetActiveServer()) ?? (flag4 ? ServerInstance.SERVER_LOCAL : (ServerInstance)null));
        if (instance.GetSaveInstance() != null)
            return;
        instance.AutoDetectLocalSave(this.launcher);
    }

    public void UpdateInstanceInfo(GameInstance instance)
    {
        Panel panel = this.PanelByInstance[instance];
        if (panel == null)
            return;
        string hash = this.GameInstanceToHash(instance);
        PictureBox pictureBox = (PictureBox)panel.Controls.Find("InstanceIconSmall" + hash, true)[0];
        Control control = panel.Controls.Find("InstanceButton" + hash, true)[0];
        pictureBox.Image = instance.GetInstanceIcon();
        if (this.launcher.currentGame == instance)
            control.Text = "[>]" + instance.GetInstanceName();
        else
            control.Text = instance.GetInstanceName();
    }

    public void CreateNewServerInstancePanel(ServerInstance server, int index)
    {
        string hash = this.ServerInstanceToHash(server);
        Panel panel = new Panel();
        PictureBox pictureBox = new PictureBox();
        Button button = new Button();
        panel.Controls.Add((Control)button);
        panel.Location = new Point(0, 0);
        panel.Margin = new Padding(0);
        panel.Name = "ServerSample-" + hash;
        panel.Size = new Size(260, 32 /*0x20*/);
        panel.TabIndex = 2;
        pictureBox.BackColor = Color.Transparent;
        pictureBox.Enabled = false;
        pictureBox.Image = (Image)Resources.TheCrew;
        pictureBox.Location = new Point(3, 3);
        pictureBox.Name = "ServerIcon-" + hash;
        pictureBox.Size = new Size(24, 24);
        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        pictureBox.TabIndex = 0;
        pictureBox.TabStop = false;
        button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        button.AutoEllipsis = true;
        button.BackColor = Color.FromArgb(5, 173, 241);
        button.FlatStyle = FlatStyle.Flat;
        button.Font = new Font("TheCrew Sans Regular", 12f);
        button.Location = new Point(-1, 0);
        button.Margin = new Padding(0);
        button.Name = "ServerButton-" + hash;
        button.Padding = new Padding(28, 0, 0, 0);
        button.Size = new Size(260, 32 /*0x20*/);
        button.TabIndex = 1;
        button.Text = "The Crew Server";
        button.TextAlign = ContentAlignment.MiddleLeft;
        button.UseVisualStyleBackColor = false;
        this.ServerList.Controls.Add((Control)panel);
        button.Controls.Add((Control)pictureBox);
        panel.TabIndex = index;
        button.Text = !server.GetName().StartsWith(Localizator.UNLOCALIZED_SYMBOL) ? server.GetName() : this.launcher.loc.Get(server.GetName());
        button.Click += (EventHandler)((sender, e) => this.UpdateSelectedServer(this.launcher.instances[this.launcher.currentInstance], server));
        pictureBox.Image = server.GetServerIcon() ?? (Image)Resources.TheCrew;
        this.fontManager?.OverrideFonts((Control)panel, new bool?(true));
        this.PanelByServerInstance[server] = panel;
    }

    public void LoadServerList(GameInstance curInstance)
    {
        List<ServerInstance> serverList = new List<ServerInstance>();
        foreach (ServerInstance serverInstance in this.launcher.serverInstances)
        {
            if (serverInstance.IsVersionMatching(curInstance))
                serverList.Add(serverInstance);
        }
        this.RebuildServerInstancePanels(curInstance, serverList);
    }

    public void UpdateSelectedServer(GameInstance instance, ServerInstance? svInstance)
    {
        bool flag1 = this.launcher.currentGame == instance;
        this.ServerIconBig.Visible = svInstance != null;
        this.BtnManageServer.Visible = svInstance != null;
        this.BtnManageServer.Enabled = (instance.IsPatched() || instance.IsForcePatched()) && !flag1;
        this.BtnServerSelect.Visible = svInstance != null;
        this.ServerDescription.Visible = svInstance != null;
        if (svInstance != null)
        {
            LauncherUtils.ReadServerIni(this.launcher.userdata, instance, svInstance.GetServerConfig());
            if (!flag1)
            {
                SaveInstance saveInstance = instance.GetSaveInstance();
                if (saveInstance != null && !saveInstance.GetPlayerNames().Contains(svInstance.GetServerConfig().GetUsername()))
                    LauncherUtils.SelectFirstAvailableSave(instance, svInstance.GetServerConfig());
                this.launcher.WriteServerIni(instance, svInstance.GetServerConfig());
            }
            string name = svInstance.GetName();
            string description = svInstance.GetDescription();
            string str1 = !name.StartsWith(Localizator.UNLOCALIZED_SYMBOL) ? name : this.launcher.loc.Get(name);
            string str2 = !description.StartsWith(Localizator.UNLOCALIZED_SYMBOL) ? description : this.launcher.loc.Get(description);
            Label svPlayInfo = this.SvPlayInfo;
            string str3;
            if (!svInstance.IsOnline())
            {
                ServerConfig serverConfig = svInstance.GetServerConfig();
                if ((serverConfig != null ? (serverConfig.GetUsername().Length > 0 ? 1 : 0) : 0) == 0)
                    str3 = this.launcher.loc.Get("server_youre_playing_offline");
                else
                    str3 = this.launcher.loc.Get("server_youre_playing_offline_as", new string[1]
                    {
            svInstance.GetServerConfig().GetUsername()
                    });
            }
            else
                str3 = this.launcher.loc.Get("server_youre_playing_on", new string[1]
                {
          str1
                });
            svPlayInfo.Text = str3;
            this.ServerName.Text = str1;
            this.ServerDescription.Text = str2;
            this.ServerIconBig.Image = svInstance.GetServerIcon() ?? (Image)Resources.TheCrew;
            bool flag2 = instance.GetActiveServer() == svInstance.GetAddress() || instance.GetActiveServer() == null && svInstance.IsLocal();
            this.BtnServerSelect.Text = flag2 ? this.launcher.loc.Get("selected") : this.launcher.loc.Get("select");
            this.BtnServerSelect.Enabled = !flag2;
        }
        else
        {
            this.SvPlayInfo.Text = this.launcher.loc.Get("server_youre_not_playing");
            this.ServerName.Text = this.launcher.loc.Get("title_unavailable");
        }
    }

    public void ShowProcess(LauncherProcess process)
    {
        this.ProgressBarPanel.Visible = true;
        this.InfoPanel.Visible = false;
        this.ProgressBar.Value = 0;
        this.ProgressLabel.Text = process.GetProcessName();
    }

    public void UpdateProcess(LauncherProcess process, int percent)
    {
        this.ProgressBar.Value = percent;
        this.ProgressLabel.Text = $"{process.GetProcessName()} ({percent.ToString()}%)";
    }

    public void StopProcess()
    {
        this.ProgressBar.Value = 0;
        this.ProgressBarPanel.Visible = false;
        this.InfoPanel.Visible = true;
    }

    public string? ArchivePasswordPrompt(IWin32Window? parent)
    {
        string output = (string)null;
        int num = (int)new TextInputForm(this.launcher.loc.Get("archive_enter_password"), this.launcher.loc.Get("archive_enter_password_details"), (string)null, (string)null, new char?('*'), this.launcher.loc.Get("accept"), new int?(), new int?(), (Func<char, bool>)null, (Action<string>)(password => output = password)).ShowDialog(parent);
        return output;
    }

    private void ClickedNewInstance(object sender, EventArgs e)
    {
        this.OpenFileGameExe.InitialDirectory = Directory.GetCurrentDirectory();
        this.OpenFileGameExe.FileName = LauncherGlobals.FILE_NAME_EXE;
        this.OpenFileGameExe.Multiselect = false;
        this.OpenFileGameExe.Filter = "TheCrew.exe|*exe";
        if (this.OpenFileGameExe.ShowDialog((IWin32Window)this) != DialogResult.OK)
            return;
        string fileName = this.OpenFileGameExe.FileName;
        this.OpenFileGameExe.FileName = (string)null;
        int num1;
        if (LauncherUtils.VerifyDirectoryToExe(fileName))
        {
            KeyValuePair<int, GameInstance>? instanceFromDirectory = this.launcher.GetInstanceFromDirectory(fileName);
            if (instanceFromDirectory.HasValue)
            {
                string textHeader = this.launcher.loc.Get("alert");
                string textDesc = this.launcher.loc.Get("errormsg_duplicate_instance", new string[1]
                {
          instanceFromDirectory.Value.Value.GetInstanceName()
                });
                Bitmap icDboxInfo = Resources.ic_dbox_info;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                num1 = index + 1;
                int num2 = (int)new ConfirmForm(textHeader, textDesc, (Image)icDboxInfo, buttonActionList).ShowDialog((IWin32Window)this);
            }
            else
            {
                if (LauncherUtils.IsSteamVersion(fileName))
                {
                    foreach (GameInstance instance in this.launcher.instances)
                    {
                        if (LauncherUtils.IsSteamVersion(instance.GetDirectoryPath()))
                        {
                            string textHeader = this.launcher.loc.Get("alert");
                            string textDesc = this.launcher.loc.Get("warnmsg_multiple_steam_installs", new string[2]
                            {
                "Steam",
                LauncherGlobals.FILE_USERDATA
                            });
                            Bitmap icDboxInfo = Resources.ic_dbox_info;
                            List<ButtonAction> buttonActionList = new List<ButtonAction>();
                            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                            int index = 0;
                            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                            num1 = index + 1;
                            int num3 = (int)new ConfirmForm(textHeader, textDesc, (Image)icDboxInfo, buttonActionList).ShowDialog((IWin32Window)this);
                            break;
                        }
                    }
                }
                int index1 = this.launcher.AddInstance(this.launcher.NewInstanceFromDirectory((string)null, (string)null, fileName));
                this.RebuildInstancesPanels();
                if (this.launcher.currentInstance == -1)
                    this.launcher.SelectInstance(index1);
                this.launcher.SaveUserData();
            }
        }
        else
        {
            string textHeader = this.launcher.loc.Get("directory_invalid");
            string textDesc = this.launcher.loc.Get("directory_invalid_details", new string[2]
            {
        "The Crew",
        LauncherGlobals.FILE_NAME_EXE
            });
            Bitmap icRefuse = Resources.ic_refuse_;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num1 = index + 1;
            int num4 = (int)new ConfirmForm(textHeader, textDesc, (Image)icRefuse, buttonActionList).ShowDialog((IWin32Window)this);
        }
    }

    private void ClickedRemoveInstance(object sender, EventArgs e)
    {
        if (this.launcher.currentInstance == -1)
            return;
        GameInstance instance = this.launcher.instances[this.launcher.currentInstance];
        int num1;
        if (this.launcher.currentGame != instance)
        {
            if (instance == null)
                return;
            string textHeader = this.launcher.loc.Get("warning");
            string textDesc = this.launcher.loc.Get("instance_remove_details", new string[1]
            {
        instance.GetInstanceName()
            });
            Bitmap icDboxWarning = Resources.ic_dbox_warning;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index1 = 0;
            span[index1] = new ButtonAction(this.launcher.loc.Get("ok"), (Action)(() =>
            {
                this.RemoveInstancePanel(instance);
                this.launcher.RemoveInstance(instance);
                this.launcher.SelectInstance(Math.Min(this.launcher.currentInstance, this.launcher.instances.Count - 1));
                this.RebuildInstancesPanels();
                if (this.launcher.instances.Count == 0)
                {
                    this.launcher.SelectInstance(-1);
                    this.ShowMainPage();
                }
                else
                {
                    GameInstance instance1 = this.launcher.currentInstance <= 0 || this.launcher.currentInstance >= this.launcher.instances.Count ? (GameInstance)null : this.launcher.instances[this.launcher.currentInstance];
                    if (instance1 != null)
                        this.UpdateInstancePage(instance1);
                    else
                        this.ShowMainPage();
                }
                this.launcher.SaveUserData();
            }));
            int index2 = index1 + 1;
            span[index2] = new ButtonAction(this.launcher.loc.Get("cancel"));
            num1 = index2 + 1;
            int num2 = (int)new ConfirmForm(textHeader, textDesc, (Image)icDboxWarning, buttonActionList).ShowDialog((IWin32Window)this);
        }
        else
        {
            string textHeader = this.launcher.loc.Get("alert");
            string textDesc = this.launcher.loc.Get("instance_action_while_running");
            Bitmap icRefuse = Resources.ic_refuse_;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num1 = index + 1;
            int num3 = (int)new ConfirmForm(textHeader, textDesc, (Image)icRefuse, buttonActionList).ShowDialog((IWin32Window)this);
        }
    }

    private void ClickedGoToDirInstance(object sender, EventArgs e)
    {
        if (this.launcher.currentInstance == -1)
            return;
        GameInstance instance = this.launcher.instances[this.launcher.currentInstance];
        if (instance == null)
            return;
        string directoryPath = instance.GetDirectoryPath();
        if (directoryPath == null)
            return;
        string directoryName = Path.GetDirectoryName(directoryPath);
        if (directoryName == null)
            return;
        Process.Start(new ProcessStartInfo(directoryName)
        {
            UseShellExecute = true
        });
    }

    private void MoveInstance(GameInstance instance, int index)
    {
        if (instance == null)
            return;
        if (this.launcher.instances[this.launcher.currentInstance] == instance)
        {
            this.launcher.currentInstance = index;
            this.launcher.instances.Remove(instance);
            this.launcher.instances.Insert(index, instance);
        }
        else if (this.launcher.currentInstance != -1 && this.launcher.currentInstance == index)
        {
            GameInstance instance1 = this.launcher.instances[this.launcher.currentInstance];
            this.launcher.instances.Remove(instance);
            this.launcher.instances.Insert(index, instance);
            this.launcher.instances.IndexOf(instance1);
            this.launcher.currentInstance = index;
        }
        else
        {
            this.launcher.instances.Remove(instance);
            this.launcher.instances.Insert(index, instance);
        }
        this.RebuildInstancesPanels();
    }

    private void ClickedMoveInstanceUp(object sender, EventArgs e)
    {
        if (this.launcher.currentInstance == -1)
            return;
        int currentInstance = this.launcher.currentInstance;
        if (currentInstance <= 0)
            return;
        this.MoveInstance(this.launcher.instances[currentInstance], this.launcher.currentInstance = currentInstance - 1);
        this.launcher.SaveUserData();
    }

    private void ClickedMoveInstanceDown(object sender, EventArgs e)
    {
        if (this.launcher.currentInstance == -1)
            return;
        int currentInstance = this.launcher.currentInstance;
        if (currentInstance >= this.launcher.instances.Count - 1)
            return;
        this.MoveInstance(this.launcher.instances[currentInstance], this.launcher.currentInstance = currentInstance + 1);
        this.launcher.SaveUserData();
    }

    private void ClickedPlay(object sender, EventArgs e)
    {
        if (this.launcher.currentInstance == -1)
            return;
        GameInstance instance = this.launcher.instances[this.launcher.currentInstance];
        this.UpdateInstancePage(instance);
        int num1;
        if (this.launcher.currentGame != null)
        {
            string textHeader = this.launcher.loc.Get("warning");
            string textDesc = this.launcher.loc.Get("instance_already_running");
            Bitmap icRefuse = Resources.ic_refuse_;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num1 = index + 1;
            ConfirmForm confirmForm = new ConfirmForm(textHeader, textDesc, (Image)icRefuse, buttonActionList);
        }
        else
        {
            if (instance == null)
                return;
            ServerInstance currentServer = this.launcher.currentServer;
            currentServer?.GetServerConfig().SetConfigFile(instance, currentServer);
            Action startGame = (Action)(() =>
            {
                if (!this.launcher.PlayInstance(instance, (Action)null))
                {
                    string textHeader = this.launcher.loc.Get("is_error");
                    string textDesc = this.launcher.loc.Get("instance_failed_to_launch");
                    Image instanceIcon = instance.GetInstanceIcon();
                    List<ButtonAction> buttonActionList = new List<ButtonAction>();
                    CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                    Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                    int index = 0;
                    span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                    int num2 = index + 1;
                    int num3 = (int)new ConfirmForm(textHeader, textDesc, instanceIcon, buttonActionList).ShowDialog((IWin32Window)this);
                }
                this.UpdateInstanceInfo(instance);
                this.UpdateInstancePage(instance);
            });
            if (instance.IsPatched() || instance.IsForcePatched())
            {
                startGame();
            }
            else
            {
                string textHeader = this.launcher.loc.Get("unpatched_instance");
                string textDesc = this.launcher.loc.Get("instance_unpatched_details");
                Image instanceIcon = instance.GetInstanceIcon();
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index1 = 0;
                span[index1] = new ButtonAction(this.launcher.loc.Get("run_anyway"), (Action)(() => startGame()));
                int index2 = index1 + 1;
                span[index2] = new ButtonAction(this.launcher.loc.Get("cancel"));
                num1 = index2 + 1;
                int num4 = (int)new ConfirmForm(textHeader, textDesc, instanceIcon, buttonActionList).ShowDialog((IWin32Window)this);
            }
        }
    }

    protected void SelectManualPatchFile(Action<string>? onSelected)
    {
        if (this.OpenManualPatch.ShowDialog() != DialogResult.OK)
            return;
        string fileName = this.OpenManualPatch.FileName;
        if (fileName.Length <= 0 || onSelected == null)
            return;
        onSelected(fileName);
    }

    private void ClickedPatch(object sender, EventArgs e)
    {
        GameInstance instance = this.launcher.instances[this.launcher.currentInstance];
        if (instance == null)
            return;
        int num1;
        if (this.launcher.currentGame == instance)
        {
            string textHeader = this.launcher.loc.Get("alert");
            string textDesc = this.launcher.loc.Get("instance_action_while_running");
            Bitmap icRefuse = Resources.ic_refuse_;
            List<ButtonAction> buttonActionList = new List<ButtonAction>();
            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
            int index = 0;
            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
            num1 = index + 1;
            int num2 = (int)new ConfirmForm(textHeader, textDesc, (Image)icRefuse, buttonActionList).ShowDialog((IWin32Window)this);
        }
        else
        {
            if (!this.launcher.TcuNet.isOffline)
                this.launcher.LoadManifests();
            string gameVersion = instance.GetGameVersion();
            if (gameVersion == null)
            {
                string textHeader = this.launcher.loc.Get("version_error");
                string textDesc = this.launcher.loc.Get("instance_noversion");
                Bitmap icRefuse = Resources.ic_refuse_;
                List<ButtonAction> buttonActionList = new List<ButtonAction>();
                CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                int index = 0;
                span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                num1 = index + 1;
                int num3 = (int)new ConfirmForm(textHeader, textDesc, (Image)icRefuse, buttonActionList).ShowDialog((IWin32Window)this);
            }
            else
            {
                string directoryName = Path.GetDirectoryName(instance.GetDirectoryPath());
                if (directoryName != null)
                    TCUPatcher.ValidatePatchFilesByManifest(directoryName);
                if (this.launcher.OfflineMode)
                {
                    ConfirmForm confirmForm = (ConfirmForm)null;
                    string textHeader = this.launcher.loc.Get("is_offline");
                    string textDesc = this.launcher.loc.Get("instance_patch_offline");
                    Bitmap offline = Resources.offline;
                    List<ButtonAction> buttonActionList = new List<ButtonAction>();
                    CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 2);
                    Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                    int index1 = 0;
                    //span[index1] = new ButtonAction(this.launcher.loc.Get("manual_install"), (Action)(() => ));
                    this.SelectManualPatchFile((Action<string>)(file =>
                    {
                        string password = (string)null;
                        if (TCUPatcher.IsArchiveEncrypted(file))
                        {
                            password = this.ArchivePasswordPrompt((IWin32Window)confirmForm);
                            if (password == null)
                                return;
                        }
                        this.launcher.PatchInstance(instance, file, password);
                    }));
                    int index2 = index1 + 1;
                    span[index2] = new ButtonAction(this.launcher.loc.Get("cancel"));
                    num1 = index2 + 1;
                    confirmForm = new ConfirmForm(textHeader, textDesc, (Image)offline, buttonActionList);
                    //int num4 = (int)confirmForm.ShowDialog((IWin32Window)this);
                }
                else
                {
                    bool flag1 = instance.IsPatched();
                    TCUServerVersion patchVersion = this.launcher.ServerManifest != null ? this.launcher.ServerManifest.PickLatestForGameVersion(gameVersion) : (TCUServerVersion)null;
                    if (patchVersion != null)
                    {
                        ConfirmForm confirmForm = (ConfirmForm)null;
                        string textHeader = this.launcher.loc.Get(flag1 ? "title_update" : "title_patch");
                        string textDesc = this.launcher.loc.Get(flag1 ? "instance_patch_update_details" : "instance_patch_details", new string[1]
                        {
              patchVersion.GetVersion()
                        });
                        Image icon = instance.GetInstanceIcon() ?? (Image)Resources.ic_dbox_warning;
                        List<ButtonAction> buttonActionList = new List<ButtonAction>();
                        CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 3);
                        Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                        int index3 = 0;
                        span[index3] = new ButtonAction(this.launcher.loc.Get("yes"), (Action)(() =>
                        {
                            string password = (string)null;
                            if (patchVersion.IsEncrypted())
                            {
                                password = this.ArchivePasswordPrompt((IWin32Window)confirmForm);
                                if (password == null)
                                    return;
                            }
                            this.launcher.PatchInstance(instance, (string)null, password);
                        }));
                        int index4 = index3 + 1;
                        span[index4] = new ButtonAction(this.launcher.loc.Get("manual_install"), (Action)(() => this.SelectManualPatchFile((Action<string>)(file =>
                        {
                            string password = (string)null;
                            if (TCUPatcher.IsArchiveEncrypted(file))
                            {
                                password = this.ArchivePasswordPrompt((IWin32Window)confirmForm);
                                if (password == null)
                                    return;
                            }
                            this.launcher.PatchInstance(instance, file, password);
                        }))));
                        int index5 = index4 + 1;
                        span[index5] = new ButtonAction(this.launcher.loc.Get("no"));
                        num1 = index5 + 1;
                        confirmForm = new ConfirmForm(textHeader, textDesc, icon, buttonActionList);
                        int num5 = (int)confirmForm.ShowDialog((IWin32Window)this);
                    }
                    else
                    {
                        if (this.launcher.OfflineMode)
                            return;
                        bool flag2 = this.launcher.TcuNet.SanityCheck();
                        this.launcher.OfflineMode = !flag2;
                        if (flag2)
                        {
                            string textHeader = this.launcher.loc.Get("title_unsupported");
                            string textDesc = this.launcher.loc.Get("instance_unsupported_details", new string[1]
                            {
                                gameVersion
                            });
                            Bitmap icDboxWarning = Resources.ic_dbox_warning;
                            List<ButtonAction> buttonActionList = new List<ButtonAction>();
                            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                            int index = 0;
                            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                            num1 = index + 1;
                            int num6 = (int)new ConfirmForm(textHeader, textDesc, (Image)icDboxWarning, buttonActionList).ShowDialog((IWin32Window)this);
                        }
                        else
                        {
                            string textHeader = this.launcher.loc.Get("is_error");
                            string textDesc = this.launcher.loc.Get("errormsg_no_connection_to_tcunet", new string[1]
                            {
                Resources.NetworkTitle
                            });
                            Bitmap offline = Resources.offline;
                            List<ButtonAction> buttonActionList = new List<ButtonAction>();
                            CollectionsMarshal.SetCount<ButtonAction>(buttonActionList, 1);
                            Span<ButtonAction> span = CollectionsMarshal.AsSpan<ButtonAction>(buttonActionList);
                            int index = 0;
                            span[index] = new ButtonAction(this.launcher.loc.Get("ok"));
                            num1 = index + 1;
                            int num7 = (int)new ConfirmForm(textHeader, textDesc, (Image)offline, buttonActionList).ShowDialog((IWin32Window)this);
                            this.UpdateInstancePage(instance);
                        }
                    }
                }
            }
        }
    }

    private void ClickedManageServer(object sender, EventArgs e)
    {
        if (this.launcher.currentInstance == -1)
            return;
        GameInstance instance = this.launcher.instances[this.launcher.currentInstance];
        if (instance == null)
            return;
        ServerInstance serverInstance = this.launcher.currentServer ?? ServerInstance.SERVER_LOCAL;
        if (serverInstance == null)
            return;
        ServerConfig serverConfig = serverInstance.GetServerConfig();
        if (serverConfig.IsLocalServer())
        {
            if (!LauncherUtils.CanUseSaveFile(this.launcher, instance))
            {
                this.launcher.ShowSaveInUseWarningForm();
                return;
            }
            LocalSaveMgrForm localSaveMgrForm = new LocalSaveMgrForm(this.launcher);
            localSaveMgrForm.SetGameAndServerInstances(instance, serverInstance);
            int num = (int)localSaveMgrForm.ShowDialog((IWin32Window)this);
        }
        this.launcher.WriteServerIni(instance, serverConfig);
        this.UpdateSelectedServer(instance, serverInstance);
    }

    private void ClickedSelectServer(object sender, EventArgs e)
    {
        this.launcher.SelectServer(this.launcher.currentServer);
        this.launcher.SaveUserData();
    }

    private void ClickedNews(object sender, EventArgs e)
    {
        this.launcher.currentInstance = -1;
        this.ShowMainPage();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && this.components != null)
            this.components.Dispose();
        base.Dispose(disposing);
        this.fontManager.Dispose();
    }

    private void InitializeComponent()
    {
        this.components = (IContainer)new System.ComponentModel.Container();
        this.PlayButton = new Button();
        this.InstanceVersion = new Label();
        this.InstancePatchStatus = new Label();
        this.InstancePatchStatusIcon = new PictureBoxNM();
        this.InstanceLabelBig = new Label();
        this.InstanceIconBig = new PictureBoxNM();
        this.IconBackdrop = new PictureBoxNM();
        this.PlayArrowImg = new PictureBoxNM();
        this.InstancePanel = new Panel();
        this.InstanceIconSmall = new PictureBoxNM();
        this.InstanceButton = new Button();
        this.SplitPanels = new SplitContainer();
        this.InstancesOptionsPanel = new Panel();
        this.BtnGoToInstanceDir = new Button();
        this.BtnMoveDown = new Button();
        this.BtnMoveUp = new Button();
        this.BtnRemoveInstance = new Button();
        this.BtnNewInstance = new Button();
        this.InstancesListPanel = new FlowLayoutPanel();
        this.InstanceInfoPanel = new SplitContainer();
        this.InstancePlatformIcon = new PictureBoxNM();
        this.BtnPatch = new Button();
        this.SvPlayInfo = new Label();
        this.BtnManageServer = new Button();
        this.BtnServerSelect = new Button();
        this.ServerDescription = new Label();
        this.ServerIconBig = new PictureBoxNM();
        this.ServerName = new Label();
        this.ServerListLabel = new Label();
        this.ServerList = new Panel();
        this.ServerSample = new Panel();
        this.ServerIcon = new PictureBoxNM();
        this.ServerButton = new Button();
        this.MainPagePanel = new Panel();
        this.MainPageDesc = new LinkLabel();
        this.MainPageHeader = new Label();
        this.ProgressBar = new ProgressBar();
        this.ProgressLabel = new LabelNM();
        this.SettingsButton = new Button();
        this.OpenFileGameExe = new OpenFileDialog();
        this.ProgressBarPanel = new Panel();
        this.NewsButton = new Button();
        this.InfoPanel = new PanelNM();
        this.InfoLabel = new LabelNM();
        this.ToolTip = new ToolTip(this.components);
        this.errorProvider1 = new ErrorProvider(this.components);
        this.DiscordButton = new Button();
        this.InstructionsButton = new Button();
        this.OpenManualPatch = new OpenFileDialog();
        this.BottomButtonsPanel = new FlowLayoutPanelNM();
        this.PatreonButton = new Button();
        this.YoutubeButton = new Button();
        this.TCUWebButton = new Button();
        this.TopPanel = new PanelNM();
        this.WindowControls = new FlowLayoutPanelNM();
        this.ExitButton = new Button();
        this.MinimizeButton = new Button();
        this.TopIcon = new PictureBoxNM();
        this.TopLabel = new LabelNM();
        ((ISupportInitialize)this.InstancePatchStatusIcon).BeginInit();
        ((ISupportInitialize)this.InstanceIconBig).BeginInit();
        ((ISupportInitialize)this.IconBackdrop).BeginInit();
        ((ISupportInitialize)this.PlayArrowImg).BeginInit();
        this.InstancePanel.SuspendLayout();
        ((ISupportInitialize)this.InstanceIconSmall).BeginInit();
        this.SplitPanels.BeginInit();
        this.SplitPanels.Panel1.SuspendLayout();
        this.SplitPanels.Panel2.SuspendLayout();
        this.SplitPanels.SuspendLayout();
        this.InstancesOptionsPanel.SuspendLayout();
        this.InstancesListPanel.SuspendLayout();
        this.InstanceInfoPanel.BeginInit();
        this.InstanceInfoPanel.Panel1.SuspendLayout();
        this.InstanceInfoPanel.Panel2.SuspendLayout();
        this.InstanceInfoPanel.SuspendLayout();
        ((ISupportInitialize)this.InstancePlatformIcon).BeginInit();
        ((ISupportInitialize)this.ServerIconBig).BeginInit();
        this.ServerList.SuspendLayout();
        this.ServerSample.SuspendLayout();
        ((ISupportInitialize)this.ServerIcon).BeginInit();
        this.MainPagePanel.SuspendLayout();
        this.ProgressBarPanel.SuspendLayout();
        this.InfoPanel.SuspendLayout();
        ((ISupportInitialize)this.errorProvider1).BeginInit();
        this.BottomButtonsPanel.SuspendLayout();
        this.TopPanel.SuspendLayout();
        this.WindowControls.SuspendLayout();
        ((ISupportInitialize)this.TopIcon).BeginInit();
        this.SuspendLayout();
        this.PlayButton.BackColor = Color.FromArgb(5, 173, 241);
        this.PlayButton.FlatStyle = FlatStyle.Flat;
        this.PlayButton.Font = new Font("TheCrew Sans Bold", 15f);
        this.PlayButton.ForeColor = Color.Black;
        this.PlayButton.Location = new Point(152, 77);
        this.PlayButton.Name = "PlayButton";
        this.PlayButton.Size = new Size(137, 35);
        this.PlayButton.TabIndex = 3;
        this.PlayButton.Text = "Play";
        this.PlayButton.UseCompatibleTextRendering = true;
        this.PlayButton.UseVisualStyleBackColor = false;
        this.PlayButton.Click += new EventHandler(this.ClickedPlay);
        this.InstanceVersion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.InstanceVersion.BackColor = Color.Transparent;
        this.InstanceVersion.Font = new Font("TheCrew Sans Regular", 12f);
        this.InstanceVersion.ForeColor = Color.White;
        this.InstanceVersion.Location = new Point(523, 28);
        this.InstanceVersion.Name = "InstanceVersion";
        this.InstanceVersion.Size = new Size(133, 19);
        this.InstanceVersion.TabIndex = 2;
        this.InstanceVersion.Text = "0.0.0.0";
        this.InstanceVersion.TextAlign = ContentAlignment.MiddleRight;
        this.InstancePatchStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.InstancePatchStatus.BackColor = Color.Transparent;
        this.InstancePatchStatus.FlatStyle = FlatStyle.Flat;
        this.InstancePatchStatus.Font = new Font("TheCrew Sans Regular", 12f);
        this.InstancePatchStatus.ForeColor = Color.White;
        this.InstancePatchStatus.Location = new Point(544, 51);
        this.InstancePatchStatus.Name = "InstancePatchStatus";
        this.InstancePatchStatus.Size = new Size(110, 19);
        this.InstancePatchStatus.TabIndex = 4;
        this.InstancePatchStatus.Text = "Unpatched!";
        this.InstancePatchStatus.TextAlign = ContentAlignment.MiddleRight;
        this.InstancePatchStatusIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.InstancePatchStatusIcon.BackColor = Color.Transparent;
        this.InstancePatchStatusIcon.Image = (Image)Resources.ic_refuse_;
        this.InstancePatchStatusIcon.InitialImage = (Image)null;
        this.InstancePatchStatusIcon.Location = new Point(653, 49);
        this.InstancePatchStatusIcon.Name = "InstancePatchStatusIcon";
        this.InstancePatchStatusIcon.Size = new Size(24, 24);
        this.InstancePatchStatusIcon.SizeMode = PictureBoxSizeMode.StretchImage;
        this.InstancePatchStatusIcon.TabIndex = 5;
        this.InstancePatchStatusIcon.TabStop = false;
        this.InstanceLabelBig.AutoEllipsis = true;
        this.InstanceLabelBig.BackColor = Color.Transparent;
        this.InstanceLabelBig.Font = new Font("TheCrew Sans Bold", 24f);
        this.InstanceLabelBig.ForeColor = Color.White;
        this.InstanceLabelBig.Location = new Point(146, 31 /*0x1F*/);
        this.InstanceLabelBig.MaximumSize = new Size(437, 99);
        this.InstanceLabelBig.Name = "InstanceLabelBig";
        this.InstanceLabelBig.Size = new Size(388, 39);
        this.InstanceLabelBig.TabIndex = 1;
        this.InstanceLabelBig.Text = "The Crew Instance";
        this.InstanceLabelBig.TextAlign = ContentAlignment.MiddleLeft;
        this.InstanceIconBig.BackColor = Color.Transparent;
        this.InstanceIconBig.BackgroundImageLayout = ImageLayout.None;
        this.InstanceIconBig.Image = (Image)Resources.TheCrew;
        this.InstanceIconBig.Location = new Point(25, 22);
        this.InstanceIconBig.Name = "InstanceIconBig";
        this.InstanceIconBig.Size = new Size(99, 100);
        this.InstanceIconBig.SizeMode = PictureBoxSizeMode.Zoom;
        this.InstanceIconBig.TabIndex = 0;
        this.InstanceIconBig.TabStop = false;
        this.IconBackdrop.BackColor = Color.Transparent;
        this.IconBackdrop.Image = (Image)Resources.libhighres_i71;
        this.IconBackdrop.Location = new Point(8, 11);
        this.IconBackdrop.Name = "IconBackdrop";
        this.IconBackdrop.Size = new Size(137, (int)sbyte.MaxValue);
        this.IconBackdrop.SizeMode = PictureBoxSizeMode.StretchImage;
        this.IconBackdrop.TabIndex = 6;
        this.IconBackdrop.TabStop = false;
        this.PlayArrowImg.BackColor = Color.Transparent;
        this.PlayArrowImg.Image = (Image)Resources.scribblearrow12_white_1;
        this.PlayArrowImg.InitialImage = (Image)null;
        this.PlayArrowImg.Location = new Point(291, 70);
        this.PlayArrowImg.Name = "PlayArrowImg";
        this.PlayArrowImg.Size = new Size(30, 30);
        this.PlayArrowImg.SizeMode = PictureBoxSizeMode.StretchImage;
        this.PlayArrowImg.TabIndex = 7;
        this.PlayArrowImg.TabStop = false;
        this.InstancePanel.Controls.Add((Control)this.InstanceIconSmall);
        this.InstancePanel.Controls.Add((Control)this.InstanceButton);
        this.InstancePanel.Location = new Point(0, 0);
        this.InstancePanel.Margin = new Padding(0);
        this.InstancePanel.Name = "InstancePanel";
        this.InstancePanel.Size = new Size(175, 37);
        this.InstancePanel.TabIndex = 1;
        this.InstanceIconSmall.BackColor = Color.Transparent;
        this.InstanceIconSmall.Enabled = false;
        this.InstanceIconSmall.Image = (Image)Resources.TheCrew;
        this.InstanceIconSmall.Location = new Point(6, 6);
        this.InstanceIconSmall.Name = "InstanceIconSmall";
        this.InstanceIconSmall.Size = new Size(24, 24);
        this.InstanceIconSmall.SizeMode = PictureBoxSizeMode.Zoom;
        this.InstanceIconSmall.TabIndex = 0;
        this.InstanceIconSmall.TabStop = false;
        this.InstanceButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.InstanceButton.AutoEllipsis = true;
        this.InstanceButton.BackColor = Color.FromArgb(5, 173, 241);
        this.InstanceButton.FlatStyle = FlatStyle.Flat;
        this.InstanceButton.Font = new Font("TheCrew Sans Regular", 12f);
        this.InstanceButton.Location = new Point(0, 0);
        this.InstanceButton.Margin = new Padding(0);
        this.InstanceButton.Name = "InstanceButton";
        this.InstanceButton.Padding = new Padding(28, 0, 0, 0);
        this.InstanceButton.Size = new Size(175, 37);
        this.InstanceButton.TabIndex = 1;
        this.InstanceButton.Text = "The Crew Instance";
        this.InstanceButton.TextAlign = ContentAlignment.MiddleLeft;
        this.InstanceButton.UseVisualStyleBackColor = false;
        this.SplitPanels.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.SplitPanels.Location = new Point(13, 53);
        this.SplitPanels.Name = "SplitPanels";
        this.SplitPanels.Panel1.AccessibleName = "";
        this.SplitPanels.Panel1.AutoScroll = true;
        this.SplitPanels.Panel1.Controls.Add((Control)this.InstancesOptionsPanel);
        this.SplitPanels.Panel1.Controls.Add((Control)this.InstancesListPanel);
        this.SplitPanels.Panel2.Controls.Add((Control)this.InstanceInfoPanel);
        this.SplitPanels.Panel2.Controls.Add((Control)this.MainPagePanel);
        this.SplitPanels.Size = new Size(881, 419);
        this.SplitPanels.SplitterDistance = 182;
        this.SplitPanels.TabIndex = 1;
        this.InstancesOptionsPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.InstancesOptionsPanel.BackColor = Color.Transparent;
        this.InstancesOptionsPanel.BorderStyle = BorderStyle.Fixed3D;
        this.InstancesOptionsPanel.Controls.Add((Control)this.BtnGoToInstanceDir);
        this.InstancesOptionsPanel.Controls.Add((Control)this.BtnMoveDown);
        this.InstancesOptionsPanel.Controls.Add((Control)this.BtnMoveUp);
        this.InstancesOptionsPanel.Controls.Add((Control)this.BtnRemoveInstance);
        this.InstancesOptionsPanel.Controls.Add((Control)this.BtnNewInstance);
        this.InstancesOptionsPanel.Location = new Point(0, 387);
        this.InstancesOptionsPanel.Name = "InstancesOptionsPanel";
        this.InstancesOptionsPanel.Size = new Size(180, 32 /*0x20*/);
        this.InstancesOptionsPanel.TabIndex = 2;
        this.BtnGoToInstanceDir.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        this.BtnGoToInstanceDir.BackgroundImage = (Image)Resources.icon_folder;
        this.BtnGoToInstanceDir.BackgroundImageLayout = ImageLayout.Stretch;
        this.BtnGoToInstanceDir.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.BtnGoToInstanceDir.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.BtnGoToInstanceDir.FlatStyle = FlatStyle.Flat;
        this.BtnGoToInstanceDir.Location = new Point(136, 1);
        this.BtnGoToInstanceDir.Name = "BtnGoToInstanceDir";
        this.BtnGoToInstanceDir.Size = new Size(24, 24);
        this.BtnGoToInstanceDir.TabIndex = 2;
        this.BtnGoToInstanceDir.Text = "/";
        this.BtnGoToInstanceDir.UseVisualStyleBackColor = true;
        this.BtnGoToInstanceDir.Click += new EventHandler(this.ClickedGoToDirInstance);
        this.BtnMoveDown.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        this.BtnMoveDown.BackgroundImage = (Image)Resources.icon_down;
        this.BtnMoveDown.BackgroundImageLayout = ImageLayout.Stretch;
        this.BtnMoveDown.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.BtnMoveDown.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.BtnMoveDown.FlatStyle = FlatStyle.Flat;
        this.BtnMoveDown.Location = new Point(106, 1);
        this.BtnMoveDown.Name = "BtnMoveDown";
        this.BtnMoveDown.Size = new Size(24, 24);
        this.BtnMoveDown.TabIndex = 4;
        this.BtnMoveDown.UseVisualStyleBackColor = true;
        this.BtnMoveDown.Click += new EventHandler(this.ClickedMoveInstanceDown);
        this.BtnMoveUp.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        this.BtnMoveUp.BackgroundImage = (Image)Resources.icon_up;
        this.BtnMoveUp.BackgroundImageLayout = ImageLayout.Stretch;
        this.BtnMoveUp.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.BtnMoveUp.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.BtnMoveUp.FlatStyle = FlatStyle.Flat;
        this.BtnMoveUp.Location = new Point(76, 1);
        this.BtnMoveUp.Name = "BtnMoveUp";
        this.BtnMoveUp.Size = new Size(24, 24);
        this.BtnMoveUp.TabIndex = 3;
        this.BtnMoveUp.UseVisualStyleBackColor = true;
        this.BtnMoveUp.Click += new EventHandler(this.ClickedMoveInstanceUp);
        this.BtnRemoveInstance.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        this.BtnRemoveInstance.BackgroundImage = (Image)Resources.icon_remove;
        this.BtnRemoveInstance.BackgroundImageLayout = ImageLayout.Stretch;
        this.BtnRemoveInstance.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.BtnRemoveInstance.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.BtnRemoveInstance.FlatStyle = FlatStyle.Flat;
        this.BtnRemoveInstance.Location = new Point(46, 1);
        this.BtnRemoveInstance.Name = "BtnRemoveInstance";
        this.BtnRemoveInstance.Size = new Size(24, 24);
        this.BtnRemoveInstance.TabIndex = 1;
        this.BtnRemoveInstance.UseVisualStyleBackColor = true;
        this.BtnRemoveInstance.Click += new EventHandler(this.ClickedRemoveInstance);
        this.BtnNewInstance.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
        this.BtnNewInstance.BackgroundImage = (Image)Resources.icon_add;
        this.BtnNewInstance.BackgroundImageLayout = ImageLayout.Stretch;
        this.BtnNewInstance.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.BtnNewInstance.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.BtnNewInstance.FlatStyle = FlatStyle.Flat;
        this.BtnNewInstance.Location = new Point(16 /*0x10*/, 1);
        this.BtnNewInstance.Name = "BtnNewInstance";
        this.BtnNewInstance.Size = new Size(24, 24);
        this.BtnNewInstance.TabIndex = 0;
        this.BtnNewInstance.UseVisualStyleBackColor = true;
        this.BtnNewInstance.Click += new EventHandler(this.ClickedNewInstance);
        this.InstancesListPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.InstancesListPanel.AutoScroll = true;
        this.InstancesListPanel.BorderStyle = BorderStyle.Fixed3D;
        this.InstancesListPanel.Controls.Add((Control)this.InstancePanel);
        this.InstancesListPanel.FlowDirection = FlowDirection.TopDown;
        this.InstancesListPanel.Location = new Point(0, 0);
        this.InstancesListPanel.Margin = new Padding(0);
        this.InstancesListPanel.Name = "InstancesListPanel";
        this.InstancesListPanel.Size = new Size(180, 389);
        this.InstancesListPanel.TabIndex = 3;
        this.InstancesListPanel.WrapContents = false;
        this.InstanceInfoPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.InstanceInfoPanel.BorderStyle = BorderStyle.Fixed3D;
        this.InstanceInfoPanel.Location = new Point(0, 0);
        this.InstanceInfoPanel.Name = "InstanceInfoPanel";
        this.InstanceInfoPanel.Orientation = Orientation.Horizontal;
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.InstancePlatformIcon);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.BtnPatch);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.SvPlayInfo);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.PlayButton);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.InstanceLabelBig);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.PlayArrowImg);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.InstanceIconBig);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.IconBackdrop);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.InstancePatchStatusIcon);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.InstancePatchStatus);
        this.InstanceInfoPanel.Panel1.Controls.Add((Control)this.InstanceVersion);
        this.InstanceInfoPanel.Panel2.Controls.Add((Control)this.BtnManageServer);
        this.InstanceInfoPanel.Panel2.Controls.Add((Control)this.BtnServerSelect);
        this.InstanceInfoPanel.Panel2.Controls.Add((Control)this.ServerDescription);
        this.InstanceInfoPanel.Panel2.Controls.Add((Control)this.ServerIconBig);
        this.InstanceInfoPanel.Panel2.Controls.Add((Control)this.ServerName);
        this.InstanceInfoPanel.Panel2.Controls.Add((Control)this.ServerListLabel);
        this.InstanceInfoPanel.Panel2.Controls.Add((Control)this.ServerList);
        this.InstanceInfoPanel.Size = new Size(695, 419);
        this.InstanceInfoPanel.SplitterDistance = 176 /*0xB0*/;
        this.InstanceInfoPanel.TabIndex = 9;
        this.InstancePlatformIcon.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.InstancePlatformIcon.BackColor = Color.Transparent;
        this.InstancePlatformIcon.Image = (Image)Resources.platform_ubi;
        this.InstancePlatformIcon.InitialImage = (Image)null;
        this.InstancePlatformIcon.Location = new Point(654, 26);
        this.InstancePlatformIcon.Name = "InstancePlatformIcon";
        this.InstancePlatformIcon.Size = new Size(22, 22);
        this.InstancePlatformIcon.SizeMode = PictureBoxSizeMode.Zoom;
        this.InstancePlatformIcon.TabIndex = 12;
        this.InstancePlatformIcon.TabStop = false;
        this.BtnPatch.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.BtnPatch.BackColor = Color.FromArgb(5, 173, 241);
        this.BtnPatch.FlatStyle = FlatStyle.Flat;
        this.BtnPatch.Font = new Font("TheCrew Sans Bold", 15f);
        this.BtnPatch.ForeColor = Color.Black;
        this.BtnPatch.Location = new Point(544, 77);
        this.BtnPatch.Name = "BtnPatch";
        this.BtnPatch.Size = new Size(137, 35);
        this.BtnPatch.TabIndex = 11;
        this.BtnPatch.Text = "Patch";
        this.BtnPatch.UseCompatibleTextRendering = true;
        this.BtnPatch.UseVisualStyleBackColor = false;
        this.BtnPatch.Click += new EventHandler(this.ClickedPatch);
        this.SvPlayInfo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.SvPlayInfo.AutoSize = true;
        this.SvPlayInfo.Font = new Font("TheCrew Sans Regular", 12f);
        this.SvPlayInfo.ForeColor = Color.White;
        this.SvPlayInfo.Location = new Point(8, 144 /*0x90*/);
        this.SvPlayInfo.Name = "SvPlayInfo";
        this.SvPlayInfo.Size = new Size(193, 19);
        this.SvPlayInfo.TabIndex = 10;
        this.SvPlayInfo.Text = "You're playing on: The Crew Server";
        this.BtnManageServer.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.BtnManageServer.AutoEllipsis = true;
        this.BtnManageServer.BackColor = Color.FromArgb(5, 173, 241);
        this.BtnManageServer.FlatStyle = FlatStyle.Flat;
        this.BtnManageServer.Font = new Font("TheCrew Sans Bold", 15f);
        this.BtnManageServer.ForeColor = Color.Black;
        this.BtnManageServer.Location = new Point(398, 189);
        this.BtnManageServer.Name = "BtnManageServer";
        this.BtnManageServer.Size = new Size(137, 35);
        this.BtnManageServer.TabIndex = 12;
        this.BtnManageServer.Text = "Manage";
        this.BtnManageServer.UseCompatibleTextRendering = true;
        this.BtnManageServer.UseVisualStyleBackColor = false;
        this.BtnManageServer.Click += new EventHandler(this.ClickedManageServer);
        this.BtnServerSelect.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.BtnServerSelect.AutoEllipsis = true;
        this.BtnServerSelect.BackColor = Color.FromArgb(5, 173, 241);
        this.BtnServerSelect.FlatStyle = FlatStyle.Flat;
        this.BtnServerSelect.Font = new Font("TheCrew Sans Bold", 15f);
        this.BtnServerSelect.ForeColor = Color.Black;
        this.BtnServerSelect.Location = new Point(544, 189);
        this.BtnServerSelect.Name = "BtnServerSelect";
        this.BtnServerSelect.Size = new Size(137, 35);
        this.BtnServerSelect.TabIndex = 11;
        this.BtnServerSelect.Text = "Select";
        this.BtnServerSelect.UseCompatibleTextRendering = true;
        this.BtnServerSelect.UseVisualStyleBackColor = false;
        this.BtnServerSelect.Click += new EventHandler(this.ClickedSelectServer);
        this.ServerDescription.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.ServerDescription.Font = new Font("TheCrew Sans Regular", 12f);
        this.ServerDescription.ForeColor = Color.White;
        this.ServerDescription.Location = new Point(278, 55);
        this.ServerDescription.Name = "ServerDescription";
        this.ServerDescription.Size = new Size(349, (int)sbyte.MaxValue);
        this.ServerDescription.TabIndex = 9;
        this.ServerDescription.Text = "Description of this server.";
        this.ServerDescription.TextAlign = ContentAlignment.TopRight;
        this.ServerIconBig.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.ServerIconBig.BackColor = Color.Transparent;
        this.ServerIconBig.BackgroundImageLayout = ImageLayout.None;
        this.ServerIconBig.Image = (Image)Resources.TheCrew;
        this.ServerIconBig.Location = new Point(633, 24);
        this.ServerIconBig.Name = "ServerIconBig";
        this.ServerIconBig.Size = new Size(48 /*0x30*/, 48 /*0x30*/);
        this.ServerIconBig.SizeMode = PictureBoxSizeMode.Zoom;
        this.ServerIconBig.TabIndex = 8;
        this.ServerIconBig.TabStop = false;
        this.ServerName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.ServerName.AutoEllipsis = true;
        this.ServerName.BackColor = Color.Transparent;
        this.ServerName.Font = new Font("TheCrew Sans Bold", 18f);
        this.ServerName.ForeColor = Color.White;
        this.ServerName.Location = new Point(390, 24);
        this.ServerName.MaximumSize = new Size(437, 99);
        this.ServerName.Name = "ServerName";
        this.ServerName.Size = new Size(237, 31 /*0x1F*/);
        this.ServerName.TabIndex = 2;
        this.ServerName.Text = "The Crew Server";
        this.ServerName.TextAlign = ContentAlignment.MiddleRight;
        this.ServerListLabel.AutoSize = true;
        this.ServerListLabel.Font = new Font("TheCrew Sans Regular", 12f);
        this.ServerListLabel.ForeColor = Color.White;
        this.ServerListLabel.Location = new Point(8, 8);
        this.ServerListLabel.Name = "ServerListLabel";
        this.ServerListLabel.Size = new Size(65, 25);
        this.ServerListLabel.TabIndex = 1;
        this.ServerListLabel.Text = "Server List";
        this.ServerListLabel.UseCompatibleTextRendering = true;
        this.ServerList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        this.ServerList.AutoScroll = true;
        this.ServerList.BackColor = Color.Black;
        this.ServerList.BorderStyle = BorderStyle.Fixed3D;
        this.ServerList.Controls.Add((Control)this.ServerSample);
        this.ServerList.Location = new Point(8, 36);
        this.ServerList.Name = "ServerList";
        this.ServerList.Size = new Size(264, 188);
        this.ServerList.TabIndex = 0;
        this.ServerSample.Controls.Add((Control)this.ServerIcon);
        this.ServerSample.Controls.Add((Control)this.ServerButton);
        this.ServerSample.Location = new Point(0, 0);
        this.ServerSample.Margin = new Padding(0);
        this.ServerSample.Name = "ServerSample";
        this.ServerSample.Size = new Size(260, 32 /*0x20*/);
        this.ServerSample.TabIndex = 2;
        this.ServerIcon.BackColor = Color.Transparent;
        this.ServerIcon.Enabled = false;
        this.ServerIcon.Image = (Image)Resources.TheCrew;
        this.ServerIcon.Location = new Point(3, 3);
        this.ServerIcon.Name = "ServerIcon";
        this.ServerIcon.Size = new Size(24, 24);
        this.ServerIcon.SizeMode = PictureBoxSizeMode.Zoom;
        this.ServerIcon.TabIndex = 0;
        this.ServerIcon.TabStop = false;
        this.ServerButton.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.ServerButton.AutoEllipsis = true;
        this.ServerButton.BackColor = Color.FromArgb(5, 173, 241);
        this.ServerButton.FlatStyle = FlatStyle.Flat;
        this.ServerButton.Font = new Font("TheCrew Sans Regular", 12f);
        this.ServerButton.Location = new Point(-1, 0);
        this.ServerButton.Margin = new Padding(0);
        this.ServerButton.Name = "ServerButton";
        this.ServerButton.Padding = new Padding(28, 0, 0, 0);
        this.ServerButton.Size = new Size(260, 32 /*0x20*/);
        this.ServerButton.TabIndex = 1;
        this.ServerButton.Text = "The Crew Server";
        this.ServerButton.TextAlign = ContentAlignment.MiddleLeft;
        this.ServerButton.UseVisualStyleBackColor = false;
        this.MainPagePanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.MainPagePanel.BorderStyle = BorderStyle.Fixed3D;
        this.MainPagePanel.Controls.Add((Control)this.MainPageDesc);
        this.MainPagePanel.Controls.Add((Control)this.MainPageHeader);
        this.MainPagePanel.Location = new Point(0, 0);
        this.MainPagePanel.Name = "MainPagePanel";
        this.MainPagePanel.Size = new Size(694, 419);
        this.MainPagePanel.TabIndex = 5;
        this.MainPageDesc.ActiveLinkColor = Color.Cyan;
        this.MainPageDesc.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.MainPageDesc.BackColor = Color.Transparent;
        this.MainPageDesc.DisabledLinkColor = Color.DarkCyan;
        this.MainPageDesc.Font = new Font("TheCrew Sans Regular", 16f);
        this.MainPageDesc.ForeColor = Color.White;
        this.MainPageDesc.LinkArea = new LinkArea(0, 0);
        this.MainPageDesc.LinkBehavior = LinkBehavior.HoverUnderline;
        this.MainPageDesc.LinkColor = Color.FromArgb(5, 173, 241);
        this.MainPageDesc.Location = new Point(8, 62);
        this.MainPageDesc.Name = "MainPageDesc";
        this.MainPageDesc.Size = new Size(669, 314);
        this.MainPageDesc.TabIndex = 2;
        this.MainPageDesc.Text = "Description";
        this.MainPageDesc.VisitedLinkColor = Color.FromArgb(49, 199, (int)byte.MaxValue);
        this.MainPageHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.MainPageHeader.Font = new Font("TheCrew Sans Bold", 32f);
        this.MainPageHeader.ForeColor = Color.White;
        this.MainPageHeader.Location = new Point(3, 0);
        this.MainPageHeader.Name = "MainPageHeader";
        this.MainPageHeader.Size = new Size(678, 62);
        this.MainPageHeader.TabIndex = 0;
        this.MainPageHeader.Text = "TCU Launcher";
        this.MainPageHeader.TextAlign = ContentAlignment.MiddleCenter;
        this.ProgressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.ProgressBar.Location = new Point(0, 2);
        this.ProgressBar.Name = "ProgressBar";
        this.ProgressBar.Size = new Size(165, 24);
        this.ProgressBar.Style = ProgressBarStyle.Continuous;
        this.ProgressBar.TabIndex = 5;
        this.ProgressBar.Value = 50;
        this.ProgressLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.ProgressLabel.AutoEllipsis = true;
        this.ProgressLabel.AutoSize = true;
        this.ProgressLabel.Font = new Font("TheCrew Sans Regular", 12f);
        this.ProgressLabel.ForeColor = Color.White;
        this.ProgressLabel.Location = new Point(171, 4);
        this.ProgressLabel.Name = "ProgressLabel";
        this.ProgressLabel.Size = new Size(195, 19);
        this.ProgressLabel.TabIndex = 6;
        this.ProgressLabel.Text = "Patching The Crew Instance... 50%";
        this.SettingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.SettingsButton.BackColor = Color.Transparent;
        this.SettingsButton.BackgroundImage = (Image)Resources.options_icon;
        this.SettingsButton.BackgroundImageLayout = ImageLayout.Stretch;
        this.SettingsButton.FlatAppearance.BorderSize = 0;
        this.SettingsButton.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.SettingsButton.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.SettingsButton.FlatStyle = FlatStyle.Flat;
        this.SettingsButton.Location = new Point(264, 3);
        this.SettingsButton.Name = "SettingsButton";
        this.SettingsButton.Size = new Size(28, 28);
        this.SettingsButton.TabIndex = 7;
        this.SettingsButton.UseVisualStyleBackColor = false;
        this.OpenFileGameExe.FileName = "openFileDialog1";
        this.ProgressBarPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.ProgressBarPanel.BackColor = Color.Transparent;
        this.ProgressBarPanel.Controls.Add((Control)this.ProgressLabel);
        this.ProgressBarPanel.Controls.Add((Control)this.ProgressBar);
        this.ProgressBarPanel.Location = new Point(14, 483);
        this.ProgressBarPanel.Name = "ProgressBarPanel";
        this.ProgressBarPanel.Size = new Size(512 /*0x0200*/, 26);
        this.ProgressBarPanel.TabIndex = 8;
        this.NewsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.NewsButton.BackColor = Color.Transparent;
        this.NewsButton.BackgroundImage = (Image)Resources.datatowerlibhr;
        this.NewsButton.BackgroundImageLayout = ImageLayout.Stretch;
        this.NewsButton.FlatAppearance.BorderSize = 0;
        this.NewsButton.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.NewsButton.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.NewsButton.FlatStyle = FlatStyle.Flat;
        this.NewsButton.Location = new Point(60, 3);
        this.NewsButton.Name = "NewsButton";
        this.NewsButton.Size = new Size(28, 28);
        this.NewsButton.TabIndex = 9;
        this.NewsButton.UseVisualStyleBackColor = false;
        this.NewsButton.Click += new EventHandler(this.ClickedNews);
        this.InfoPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.InfoPanel.BackColor = Color.Transparent;
        this.InfoPanel.Controls.Add((Control)this.InfoLabel);
        this.InfoPanel.Location = new Point(12, 483);
        this.InfoPanel.Name = "InfoPanel";
        this.InfoPanel.Size = new Size(513, 26);
        this.InfoPanel.TabIndex = 7;
        this.InfoLabel.AutoSize = true;
        this.InfoLabel.BackColor = Color.Transparent;
        this.InfoLabel.ForeColor = Color.Gray;
        this.InfoLabel.Location = new Point(7, 6);
        this.InfoLabel.Name = "InfoLabel";
        this.InfoLabel.Size = new Size(171, 15);
        this.InfoLabel.TabIndex = 0;
        this.InfoLabel.Text = "TCU Launcher v0.0 | 0 Instances";
        this.InfoLabel.TextAlign = ContentAlignment.MiddleLeft;
        this.ToolTip.AutoPopDelay = 30000;
        this.ToolTip.InitialDelay = 500;
        this.ToolTip.IsBalloon = true;
        this.ToolTip.ReshowDelay = 100;
        this.ToolTip.ShowAlways = true;
        this.errorProvider1.ContainerControl = (ContainerControl)this;
        this.DiscordButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.DiscordButton.BackColor = Color.Transparent;
        this.DiscordButton.BackgroundImage = (Image)Resources.Discord_Symbol_White;
        this.DiscordButton.BackgroundImageLayout = ImageLayout.Stretch;
        this.DiscordButton.FlatAppearance.BorderSize = 0;
        this.DiscordButton.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.DiscordButton.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.DiscordButton.FlatStyle = FlatStyle.Flat;
        this.DiscordButton.Location = new Point(128 /*0x80*/, 3);
        this.DiscordButton.Name = "DiscordButton";
        this.DiscordButton.Size = new Size(28, 28);
        this.DiscordButton.TabIndex = 10;
        this.DiscordButton.UseVisualStyleBackColor = false;
        this.InstructionsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.InstructionsButton.BackColor = Color.Transparent;
        this.InstructionsButton.BackgroundImage = (Image)Resources.tutorial;
        this.InstructionsButton.BackgroundImageLayout = ImageLayout.Stretch;
        this.InstructionsButton.FlatAppearance.BorderSize = 0;
        this.InstructionsButton.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.InstructionsButton.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.InstructionsButton.FlatStyle = FlatStyle.Flat;
        this.InstructionsButton.Location = new Point(230, 3);
        this.InstructionsButton.Name = "InstructionsButton";
        this.InstructionsButton.Size = new Size(28, 28);
        this.InstructionsButton.TabIndex = 11;
        this.InstructionsButton.UseVisualStyleBackColor = false;
        this.OpenManualPatch.FileName = "openFileDialog1";
        this.BottomButtonsPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.BottomButtonsPanel.BackColor = Color.Transparent;
        this.BottomButtonsPanel.Controls.Add((Control)this.SettingsButton);
        this.BottomButtonsPanel.Controls.Add((Control)this.InstructionsButton);
        this.BottomButtonsPanel.Controls.Add((Control)this.PatreonButton);
        this.BottomButtonsPanel.Controls.Add((Control)this.YoutubeButton);
        this.BottomButtonsPanel.Controls.Add((Control)this.DiscordButton);
        this.BottomButtonsPanel.Controls.Add((Control)this.TCUWebButton);
        this.BottomButtonsPanel.Controls.Add((Control)this.NewsButton);
        this.BottomButtonsPanel.FlowDirection = FlowDirection.RightToLeft;
        this.BottomButtonsPanel.Location = new Point(599, 478);
        this.BottomButtonsPanel.Name = "BottomButtonsPanel";
        this.BottomButtonsPanel.Size = new Size(295, 31 /*0x1F*/);
        this.BottomButtonsPanel.TabIndex = 12;
        this.PatreonButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.PatreonButton.BackColor = Color.Transparent;
        this.PatreonButton.BackgroundImage = (Image)Resources.patreon_white;
        this.PatreonButton.BackgroundImageLayout = ImageLayout.Stretch;
        this.PatreonButton.FlatAppearance.BorderSize = 0;
        this.PatreonButton.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.PatreonButton.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.PatreonButton.FlatStyle = FlatStyle.Flat;
        this.PatreonButton.Location = new Point(196, 3);
        this.PatreonButton.Name = "PatreonButton";
        this.PatreonButton.Size = new Size(28, 28);
        this.PatreonButton.TabIndex = 12;
        this.PatreonButton.UseVisualStyleBackColor = false;
        this.YoutubeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.YoutubeButton.BackColor = Color.Transparent;
        this.YoutubeButton.BackgroundImage = (Image)Resources.youtube_logo;
        this.YoutubeButton.BackgroundImageLayout = ImageLayout.Stretch;
        this.YoutubeButton.FlatAppearance.BorderSize = 0;
        this.YoutubeButton.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.YoutubeButton.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.YoutubeButton.FlatStyle = FlatStyle.Flat;
        this.YoutubeButton.Location = new Point(162, 3);
        this.YoutubeButton.Name = "YoutubeButton";
        this.YoutubeButton.Size = new Size(28, 28);
        this.YoutubeButton.TabIndex = 13;
        this.YoutubeButton.UseVisualStyleBackColor = false;
        this.TCUWebButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.TCUWebButton.BackColor = Color.Transparent;
        this.TCUWebButton.BackgroundImage = (Image)Resources.tcuwebsite_logo;
        this.TCUWebButton.BackgroundImageLayout = ImageLayout.Stretch;
        this.TCUWebButton.FlatAppearance.BorderSize = 0;
        this.TCUWebButton.FlatAppearance.MouseDownBackColor = Color.Silver;
        this.TCUWebButton.FlatAppearance.MouseOverBackColor = Color.DimGray;
        this.TCUWebButton.FlatStyle = FlatStyle.Flat;
        this.TCUWebButton.Location = new Point(94, 3);
        this.TCUWebButton.Name = "TCUWebButton";
        this.TCUWebButton.Size = new Size(28, 28);
        this.TCUWebButton.TabIndex = 14;
        this.TCUWebButton.UseVisualStyleBackColor = false;
        this.TopPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.TopPanel.BackColor = Color.Transparent;
        this.TopPanel.BorderStyle = BorderStyle.Fixed3D;
        this.TopPanel.Controls.Add((Control)this.WindowControls);
        this.TopPanel.Controls.Add((Control)this.TopIcon);
        this.TopPanel.Controls.Add((Control)this.TopLabel);
        this.TopPanel.Location = new Point(12, 10);
        this.TopPanel.Name = "TopPanel";
        this.TopPanel.Size = new Size(882, 33);
        this.TopPanel.TabIndex = 13;
        this.WindowControls.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.WindowControls.Controls.Add((Control)this.ExitButton);
        this.WindowControls.Controls.Add((Control)this.MinimizeButton);
        this.WindowControls.FlowDirection = FlowDirection.RightToLeft;
        this.WindowControls.Location = new Point(731, -2);
        this.WindowControls.Margin = new Padding(0);
        this.WindowControls.Name = "WindowControls";
        this.WindowControls.Size = new Size(149, 33);
        this.WindowControls.TabIndex = 4;
        this.ExitButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.ExitButton.BackColor = Color.Black;
        this.ExitButton.FlatAppearance.BorderColor = Color.White;
        this.ExitButton.FlatAppearance.BorderSize = 0;
        this.ExitButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 230, (int)byte.MaxValue);
        this.ExitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(5, 173, 241);
        this.ExitButton.FlatStyle = FlatStyle.Flat;
        this.ExitButton.Font = new Font("TheCrew Street Regular", 15.75f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        this.ExitButton.ForeColor = Color.White;
        this.ExitButton.Location = new Point(116, 0);
        this.ExitButton.Margin = new Padding(0);
        this.ExitButton.Name = "ExitButton";
        this.ExitButton.Size = new Size(33, 33);
        this.ExitButton.TabIndex = 2;
        this.ExitButton.Text = "X";
        this.ExitButton.UseCompatibleTextRendering = true;
        this.ExitButton.UseVisualStyleBackColor = false;
        this.MinimizeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        this.MinimizeButton.BackColor = Color.Black;
        this.MinimizeButton.FlatAppearance.BorderColor = Color.White;
        this.MinimizeButton.FlatAppearance.BorderSize = 0;
        this.MinimizeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(100, 230, (int)byte.MaxValue);
        this.MinimizeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(5, 173, 241);
        this.MinimizeButton.FlatStyle = FlatStyle.Flat;
        this.MinimizeButton.Font = new Font("TheCrew Sans Regular", 27.75f, FontStyle.Regular, GraphicsUnit.Point, (byte)0);
        this.MinimizeButton.ForeColor = Color.White;
        this.MinimizeButton.Location = new Point(83, 0);
        this.MinimizeButton.Margin = new Padding(0);
        this.MinimizeButton.Name = "MinimizeButton";
        this.MinimizeButton.Size = new Size(33, 33);
        this.MinimizeButton.TabIndex = 3;
        this.MinimizeButton.Text = "-";
        this.MinimizeButton.UseCompatibleTextRendering = true;
        this.MinimizeButton.UseVisualStyleBackColor = false;
        this.TopIcon.Image = (Image)Resources.icon_offlineserver;
        this.TopIcon.InitialImage = (Image)null;
        this.TopIcon.Location = new Point(3, 2);
        this.TopIcon.Name = "TopIcon";
        this.TopIcon.Size = new Size(24, 24);
        this.TopIcon.SizeMode = PictureBoxSizeMode.Zoom;
        this.TopIcon.TabIndex = 1;
        this.TopIcon.TabStop = false;
        this.TopLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.TopLabel.AutoEllipsis = true;
        this.TopLabel.Font = new Font("TheCrew Street Regular", 12f);
        this.TopLabel.ForeColor = Color.White;
        this.TopLabel.Location = new Point(-2, 3);
        this.TopLabel.Name = "TopLabel";
        this.TopLabel.Size = new Size(882, 23);
        this.TopLabel.TabIndex = 0;
        this.TopLabel.Text = "TCU Launcher v0.0.0.0";
        this.TopLabel.TextAlign = ContentAlignment.MiddleCenter;
        this.TopLabel.UseCompatibleTextRendering = true;
        this.AutoScaleDimensions = new SizeF(96f, 96f);
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.AutoValidate = AutoValidate.EnableAllowFocusChange;
        this.BackColor = Color.Black;
        this.ClientSize = new Size(902, 515);
        this.Controls.Add((Control)this.TopPanel);
        this.Controls.Add((Control)this.BottomButtonsPanel);
        this.Controls.Add((Control)this.InfoPanel);
        this.Controls.Add((Control)this.ProgressBarPanel);
        this.Controls.Add((Control)this.SplitPanels);
        this.FormBorderStyle = FormBorderStyle.None;
        this.MinimumSize = new Size(893, 489);
        //this.Name = nameof(LauncherMainForm); // **TODO** Why is this causing the designer to break?
        this.Name = "LauncherMainForm";
        this.Text = "TCU Launcher";
        ((ISupportInitialize)this.InstancePatchStatusIcon).EndInit();
        ((ISupportInitialize)this.InstanceIconBig).EndInit();
        ((ISupportInitialize)this.IconBackdrop).EndInit();
        ((ISupportInitialize)this.PlayArrowImg).EndInit();
        this.InstancePanel.ResumeLayout(false);
        ((ISupportInitialize)this.InstanceIconSmall).EndInit();
        this.SplitPanels.Panel1.ResumeLayout(false);
        this.SplitPanels.Panel2.ResumeLayout(false);
        this.SplitPanels.EndInit();
        this.SplitPanels.ResumeLayout(false);
        this.InstancesOptionsPanel.ResumeLayout(false);
        this.InstancesListPanel.ResumeLayout(false);
        this.InstanceInfoPanel.Panel1.ResumeLayout(false);
        this.InstanceInfoPanel.Panel1.PerformLayout();
        this.InstanceInfoPanel.Panel2.ResumeLayout(false);
        this.InstanceInfoPanel.Panel2.PerformLayout();
        this.InstanceInfoPanel.EndInit();
        this.InstanceInfoPanel.ResumeLayout(false);
        ((ISupportInitialize)this.InstancePlatformIcon).EndInit();
        ((ISupportInitialize)this.ServerIconBig).EndInit();
        this.ServerList.ResumeLayout(false);
        this.ServerSample.ResumeLayout(false);
        ((ISupportInitialize)this.ServerIcon).EndInit();
        this.MainPagePanel.ResumeLayout(false);
        this.ProgressBarPanel.ResumeLayout(false);
        this.ProgressBarPanel.PerformLayout();
        this.InfoPanel.ResumeLayout(false);
        this.InfoPanel.PerformLayout();
        ((ISupportInitialize)this.errorProvider1).EndInit();
        this.BottomButtonsPanel.ResumeLayout(false);
        this.TopPanel.ResumeLayout(false);
        this.WindowControls.ResumeLayout(false);
        ((ISupportInitialize)this.TopIcon).EndInit();
        this.ResumeLayout(false);
    }
}
