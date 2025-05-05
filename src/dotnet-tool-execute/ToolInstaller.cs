using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace DotnetToolExecute
{
    internal sealed class ToolInstaller
    {
        public static bool TryStartAndWaitForExit(string toolName, bool silent, List<string> arguments, out int exitCode, [NotNullWhen(true)] out DirectoryInfo? toolPath)
        {
            toolPath = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dtx", toolName));

            using Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = string.Join(" ", ["tool", "install", toolName, "--tool-path", toolPath, .. arguments]),
                    RedirectStandardOutput = silent,
                    RedirectStandardError = silent,
                },
            };

            process.Start();

            if (process.StartInfo.RedirectStandardOutput)
            {
                process.BeginOutputReadLine();
            }

            if (process.StartInfo.RedirectStandardError)
            {
                process.BeginErrorReadLine();
            }


            process.WaitForExit();

            exitCode = process.ExitCode;

            return exitCode == 0;
        }
    }
}