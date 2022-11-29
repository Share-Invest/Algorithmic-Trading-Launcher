using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace ShareInvest
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (Status.IsAdministrator)
            {
                base.OnStartup(e);

                return;
            }
#if DEBUG
            foreach (var arg in e.Args)
            {
                Debug.WriteLine(arg);
            }
#else
            using (var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Environment.CurrentDirectory,
                    Verb = ShareInvest.Properties.Resources.ADMIN,
                    UseShellExecute = true,
                    FileName = string.Concat(Assembly.GetEntryAssembly()?
                                                     .ManifestModule
                                                     .Name
                                                     .Split('.')[0],
                                             ShareInvest.Properties.Resources.EXE)
                }
            })
                if (process.Start())
                {
                    GC.Collect();
                }
            Process.GetCurrentProcess().Kill();
#endif
        }
    }
}