﻿using Microsoft.Win32;

using Newtonsoft.Json;

using System;
using System.Diagnostics;
using System.IO;

namespace ShareInvest.Services;

class Register
{
    internal Register(string reg)
    {
        this.reg = reg;
    }
    internal bool GetValue(string program, string fileName)
    {
        bool result = false;

        using (var regKey = Registry.CurrentUser.OpenSubKey(reg, true))

            if (regKey != null &&
                string.IsNullOrEmpty(fileName) is false)
            {
                result = regKey.GetValue(program) != null;

                regKey.Close();
            }
        return result;
    }
    internal string? AddStartupProgram(string program, string fileName)
    {
#if DEBUG
        Debug.WriteLine(JsonConvert.SerializeObject(new
        {
            program,
            path = Path.Combine(Environment.CurrentDirectory,
                                fileName),
            isWritable = IsWritable
        },
        Formatting.Indented));
#else
        using (var regKey = Registry.CurrentUser.OpenSubKey(reg, true))
            if (regKey != null)
                try
                {
                    if (IsWritable)
                    {
                        if (string.IsNullOrEmpty(fileName) is false &&
                            regKey.GetValue(program) is null)
                        {
                            regKey.SetValue(program,
                                            Path.Combine(Environment.CurrentDirectory,
                                                         fileName));
                        }
                    }
                    else
                        regKey.DeleteValue(program, false);

                    regKey.Close();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
#endif
        return null;
    }
    internal bool IsWritable
    {
        get; set;
    }
    readonly string reg;
}