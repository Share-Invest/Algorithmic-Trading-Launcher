using System;
using System.ComponentModel;
using System.Windows;

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
                    Name = nameof(Properties.Resources.LINK),
                    Text = Properties.Resources.LINK
                },
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Name = nameof(Properties.Resources.EXIT),
                    Text = Properties.Resources.EXIT
                }
            });
            menu.ItemClicked += (sender, e) =>
            {
                if (nameof(Properties.Resources.LINK).Equals(e.ClickedItem.Name,
                                                             StringComparison.OrdinalIgnoreCase))
                {


                    return;
                }
                Visibility = Visibility.Hidden;

                Close();
            };
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                ContextMenuStrip = menu,
                Visible = true,
                Text = Properties.Resources.TITLE,
                Icon = Properties.Resources.RECYCLE,
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
            Title = Properties.Resources.TITLE;

            InitializeComponent();
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
        readonly System.Windows.Forms.ContextMenuStrip menu;
        readonly System.Windows.Forms.NotifyIcon notifyIcon;
    }
}