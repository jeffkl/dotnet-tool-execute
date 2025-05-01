using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DotnetToolExecute
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (string.Equals(Environment.GetEnvironmentVariable("DEBUG_DTX"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                Debugger.Launch();
            }

            (string? toolName, List<string> args1, List<string> args2) = ParseArgs(args);

            if (toolName == null)
            {
                return PrintUsage();
            }

            (int installToolExitCode, FileInfo? toolExecutable) = InstallTool(toolName, args1);

            if (installToolExitCode != 0 || toolExecutable == null)
            {
                return installToolExitCode;
            }

            return RunTool(toolExecutable, args2);
        }

        private static (int, FileInfo?) InstallTool(string toolName, List<string> args1)
        {
            DirectoryInfo toolPath = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "dtx", toolName));

            using Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = string.Join(" ", ["tool", "install", toolName, "--tool-path", toolPath, .. args1]),
                },
            };

            process.Start();

            process.WaitForExit();

            return (process.ExitCode, process.ExitCode == 0 ? toolPath.EnumerateFiles().FirstOrDefault() : null);
        }

        private static (string? toolName, List<string> args1, List<string> args2) ParseArgs(string[] args)
        {
            var args1 = new List<string>();
            var args2 = new List<string>();
            bool firstArgs = true;

            string? toolName = null;

            for (int i = 0; i < args.Length; i++)
            {
                string? arg = args[i];
                if (i == 0)
                {
                    toolName = arg;
                    continue;
                }

                if (arg == "--")
                {
                    firstArgs = false;
                    continue;
                }

                if (firstArgs)
                {
                    switch (arg)
                    {
                        case "--global":
                        case "-g":
                        case "--local":
                            continue;

                        case "--help":
                        case "-h":
                        case "-?":
                            toolName = null;
                            continue;

                        case "--tool-path":
                        case "--tool-manifest":
                        case "--create-manifest-if-needed":
                            i++;
                            continue;
                        default:
                            args1.Add(arg);
                            break;
                    }
                }
                else
                {
                    args2.Add(arg);
                }
            }

            return (toolName, args1, args2);
        }

        private static int PrintUsage()
        {
            Console.WriteLine(@"Description:
  Install and run a .NET tool.

Usage:
  dtx <packageId> [options] -- [tool options]

Arguments:
  <PACKAGE_ID>  The NuGet Package Id of the tool to install.

Options:
  --version <VERSION>          The version of the tool package to install.
  --configfile <FILE>          The NuGet configuration file to use.
  --add-source <ADDSOURCE>     Add an additional NuGet package source to use during installation.
  --source <SOURCE>            Replace all NuGet package sources to use during installation with these.
  --framework <FRAMEWORK>      The target framework to install the tool for.
  --prerelease                 Include pre-release packages.
  --disable-parallel           Prevent restoring multiple projects in parallel.
  --ignore-failed-sources      Treat package source failures as warnings.
  --no-http-cache              Do not cache packages and http requests.
  --interactive                Allows the command to stop and wait for user input or action (for example to complete authentication).
  -v, --verbosity <LEVEL>      Set the MSBuild verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic].
  -a, --arch <arch>            The target architecture.
  --allow-downgrade            Allow package downgrade when installing a .NET tool package.
  --allow-roll-forward         Allow a .NET tool to roll forward to newer versions of the .NET runtime if the runtime it targets isn't installed.
  -?, -h, --help               Show command line help.");

            return 0;
        }

        private static int RunTool(FileInfo toolExecutable, List<string> args2)
        {
            using Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = toolExecutable.FullName,
                    Arguments = string.Join(" ", args2),
                },
            };

            process.StartInfo.EnvironmentVariables["DOTNET_ROLL_FORWARD"] = "LatestMajor";

            process.Start();

            process.WaitForExit();

            return process.ExitCode;
        }
    }
}
