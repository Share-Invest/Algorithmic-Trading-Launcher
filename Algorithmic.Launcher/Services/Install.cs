using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ShareInvest.Services;

static class Install
{
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
                    Debug.WriteLine(info.FileName);

                    break;
                }
            }
        }
        foreach (var file in Directory.EnumerateFiles(dirName,
                                                      "*",
                                                      SearchOption.AllDirectories))
        {
            Debug.WriteLine(file);

            yield return FileVersionInfo.GetVersionInfo(file);
        }
    }
    static string? CompanyName
    {
        get
        {
            var location = Assembly.GetExecutingAssembly().Location;

            return FileVersionInfo.GetVersionInfo(location).CompanyName;
        }
    }
}