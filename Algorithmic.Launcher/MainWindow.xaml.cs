using ShareInvest.Services;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ShareInvest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            menu = new System.Windows.Forms.ContextMenuStrip
            {
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            menu.Items.AddRange(new[]
            {
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Name = nameof(Properties.Resources.REGISTER),
                    Text = Properties.Resources.REGISTER
                },
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Name = nameof(Properties.Resources.EXIT),
                    Text = Properties.Resources.EXIT
                }
            });
            menu.ItemClicked += (sender, e) =>
            {
                switch (e.ClickedItem.Name)
                {
                    case nameof(Properties.Resources.REGISTER):

                        return;
                }
                Visibility = Visibility.Hidden;

                Close();
            };
            icons = new[]
            {
                Properties.Resources.RECYCLING,
                Properties.Resources.RECYCLE
            };
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                ContextMenuStrip = menu,
                Visible = true,
                Text = Properties.Resources.TITLE,
                Icon = icons[^1],
                BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
            };
            notifyIcon.MouseDoubleClick += (sender, e) =>
            {
                if (IsVisible == false)
                {
                    Show();

                    WindowState = WindowState.Normal;
                }
            };
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += (sender, e) =>
            {
                var now = DateTime.Now;

                switch (now.Minute % 3)
                {
                    case 0 when now.Second == 0:

                        break;

                    case 1 when now.Second == 0x1E:

                        break;
                }
                notifyIcon.Icon = icons[now.Second % 2];
            };
            Title = Properties.Resources.TITLE;

            InitializeComponent();

            var hRgn = WindowAttribute.CreateRoundRectRgn(0,
                                                          0,
                                                          menu.Width,
                                                          menu.Height,
                                                          9,
                                                          9);
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

            WindowAttribute.DwmSetWindowAttribute(new WindowInteropHelper(this).EnsureHandle(),
                                                  DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                                                  ref preference,
                                                  sizeof(uint));

            _ = WindowAttribute.SetWindowRgn(menu.Handle, hRgn, true);
#if DEBUG
            foreach (var info in Install.GetVersionInfo(Properties.Resources.SERVER))
            {

            }
#endif
            timer.Start();
        }
        void OnClosing(object sender, CancelEventArgs e)
        {
            if (MessageBoxResult.Cancel.Equals(MessageBox.Show(Properties.Resources.WARNING.Replace('|', '\n'),
                                                               Title,
                                                               MessageBoxButton.OKCancel,
                                                               MessageBoxImage.Warning,
                                                               MessageBoxResult.Cancel)))
            {
                e.Cancel = true;

                return;
            }
            GC.Collect();
        }
        void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState.Normal != WindowState)
            {
                Hide();
            }
        }
        readonly DispatcherTimer timer;
        readonly System.Windows.Forms.ContextMenuStrip menu;
        readonly System.Windows.Forms.NotifyIcon notifyIcon;
        readonly System.Drawing.Icon[] icons;
    }
}