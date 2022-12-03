using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ShareInvest.Properties;
using ShareInvest.Infrastructure.Local;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ShareInvest.Services;

static class Startup
{
    internal static async Task StartProcess(string name)
    {
        var processes = Process.GetProcessesByName(nameof(Resources.API));

        if (processes.Length == 1)
        {
            processes = Process.GetProcessesByName(name);

            var client = new Update(Status.Address);

            await foreach (var item in client.GetAsyncEnumerable(name))

                if (string.IsNullOrEmpty(item.FileName) is false)
                {
                    var fullFileName = Resources.PUBLISH.Equals(item.Path) ||
                                       string.IsNullOrEmpty(item.Path) ?

                                       System.IO.Path.Combine(Resources.WD86,
                                                              item.FileName) :

                                       System.IO.Path.Combine(Resources.WD86,
                                                              item.Path[8..],
                                                              item.FileName);

                    if (new System.IO.FileInfo(fullFileName).Exists)
                    {
                        var vInfo = FileVersionInfo.GetVersionInfo(fullFileName);

                        if (item.FileVersion?.Length > 0 &&
                            item.FileVersion.Equals(vInfo.FileVersion))
                            continue;
                    }
                    var model = Convert.ToString(await client.GetAsync(nameof(FileVersionInfo),
                                                                       JToken.FromObject(new
                                                                       {
                                                                           app = item.App,
                                                                           path = Resources.PUBLISH.Equals(item.Path) ?
                                                                                  null :
                                                                                  item.Path?[8..],
                                                                           name = item.FileName
                                                                       })));
                    if (string.IsNullOrEmpty(model))
                        continue;

                    item.File = JsonConvert.DeserializeObject<Models.FileVersionInfo>(model)?.File;
#if DEBUG
                    Debug.WriteLine(JsonConvert.SerializeObject(new
                    {
                        item.App,
                        item.Path,
                        item.FileName,
                        item.FileVersion
                    },
                    Formatting.Indented));
#else
                    if (item.File != null)
                    {
                        await new File(fullFileName).WriteAllBytesAsync(item.File);
                    }
#endif
                }
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