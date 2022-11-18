using ShareInvest.Properties;

using System;
using System.Diagnostics;

namespace ShareInvest.Services;

static class Startup
{
    internal static void StartProcess(string name)
    {
        var processes = Process.GetProcessesByName(nameof(Resources.API));

        if (processes.Length == 1)
        {
            processes = Process.GetProcessesByName(name);

            if (processes.Length == 0)
            {
                using (var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        FileName = Resources.APP,
                        WorkingDirectory = Resources.WD86,
                        Verb = Resources.ADMIN
                    }
                })
                    if (process.Start())
                    {
                        GC.Collect();
                    }
            }
#if DEBUG
            foreach (var process in processes)
            {
                Debug.WriteLine(process.ProcessName);
            }
#endif
        }
        else
        {
#if DEBUG
            foreach (var process in processes)
            {
                Debug.WriteLine(process.ProcessName);
            }
#endif
        }
    }
}