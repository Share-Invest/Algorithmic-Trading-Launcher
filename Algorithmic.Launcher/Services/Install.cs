using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ShareInvest.Services;

static class Install
{
    internal static void Copy(string pattern)
    {
        foreach (var file in Directory.GetFiles(Path.Combine(Properties.Resources.PATH,
                                                             Properties.Resources.ROOT),
                                                pattern,
                                                SearchOption.AllDirectories))
        {
            var fi = new FileInfo(file.Replace(Properties.Resources.ROOT,
                                               string.Empty)
                                      .Replace(@"\\",
                                               @"\"));

            if (fi.Exists)
            {
                var source = FileVersionInfo.GetVersionInfo(file);
                var dest = FileVersionInfo.GetVersionInfo(fi.FullName);

                if (string.IsNullOrEmpty(source.FileVersion) ||
                    source.FileVersion.Equals(dest.FileVersion) is false)
                {
                    File.Copy(file, fi.FullName, true);
                }
                continue;
            }
            File.Copy(file, fi.FullName);
        }
    }
    internal static IEnumerable<FileVersionInfo> GetVersionInfo(string fileName)
    {
        string? dirName = string.Empty;

        foreach (var file in Directory.EnumerateFiles(Properties.Resources.SOURCES,
                                                      fileName,
                                                      SearchOption.AllDirectories))
        {
            var info = FileVersionInfo.GetVersionInfo(file);

            if (string.IsNullOrEmpty(CompanyName) is false &&
                CompanyName.Equals(info.CompanyName))
            {
                dirName = Path.GetDirectoryName(info.FileName);

                if (string.IsNullOrEmpty(dirName) is false &&
                    dirName.EndsWith(Properties.Resources.PUBLISH,
                                     StringComparison.OrdinalIgnoreCase))
                {
#if DEBUG
                    Debug.WriteLine(JsonConvert.SerializeObject(info,
                                                                Formatting.Indented));

                    Build(dirName,
                          string.Concat(Properties.Resources.BUILD,
                                        fileName.Split('.')[0]
                                                .ToUpperInvariant() switch
                                        {
                                            nameof(Properties.Resources.SECURITIES) =>

                                                Properties.Resources.SECURITIES,

                                            nameof(Properties.Resources.SERVER) =>

                                                Properties.Resources._64SELFCONTAINED,

                                            _ => throw new ArgumentNullException(fileName)
                                        }));
#endif
                    break;
                }
            }
        }
        foreach (var file in Directory.EnumerateFiles(dirName,
                                                      "*",
                                                      SearchOption.AllDirectories))
        {
#if DEBUG
            Debug.WriteLine(file);
#endif
            yield return FileVersionInfo.GetVersionInfo(file);
        }
    }
    internal static string? CompanyName
    {
        get
        {
            var location = Assembly.GetExecutingAssembly().Location;

            return FileVersionInfo.GetVersionInfo(location).CompanyName;
        }
    }
    static void Build(string workingDirectory, string commandLine)
    {
        DirectoryInfo? directoryInfo;

        while (workingDirectory.Length > 0)
        {
            directoryInfo = Directory.GetParent(workingDirectory);

            if (directoryInfo != null)
            {
                workingDirectory = directoryInfo.FullName;

                if (directoryInfo?.GetFiles(Properties.Resources.CSPROJ,
                                            SearchOption.TopDirectoryOnly)
                                  .Length > 0)
                    break;
            }
        }
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                Verb = Properties.Resources.ADMIN,
                FileName = Properties.Resources.POWERSHELL,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        })
        {
#if DEBUG
            process.OutputDataReceived += (sender, e) =>
            {
                Debug.WriteLine(e.Data);
            };
#endif
            if (process.Start())
            {
                process.BeginOutputReadLine();
                process.StandardInput.Write(commandLine + Environment.NewLine);
                process.StandardInput.Close();
                process.WaitForExit();
            }
        }
    }
}