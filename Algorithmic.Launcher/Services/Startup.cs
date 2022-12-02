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
                StartProcess(Resources.APP,
                             Resources.WD86);
            }
        }
    }
    internal static void StartProcess()
    {
        var programs = Process.GetProcessesByName(nameof(Resources.SERVER));

        if (programs.Length > 0)

            for (int i = 0; i < programs.Length; i++)
            {
                var companyName = programs[i].MainModule?
                                             .FileVersionInfo
                                             .CompanyName;

                if (string.IsNullOrEmpty(companyName) is false &&
                    companyName.Equals(Install.CompanyName))
                {
#if DEBUG
                    Debug.WriteLine(companyName);
#endif
                    return;
                }
            }
        Install.Copy("*");

        StartProcess(Resources.SERVER,
                     Resources.PATH);
    }
    static void StartProcess(string fileName,
                             string workingDirectory)
    {
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = fileName,
                WorkingDirectory = workingDirectory,
                Verb = Resources.ADMIN
            }
        })
            if (process.Start())
            {
                GC.Collect();
            }
    }
}