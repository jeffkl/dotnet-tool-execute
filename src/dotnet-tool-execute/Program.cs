using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace DotnetToolExecute;

public static class Program
{
    public static int Main(string[] args)
    {
        if (string.Equals(Environment.GetEnvironmentVariable("DEBUG_DTX"), bool.TrueString, StringComparison.OrdinalIgnoreCase))
        {
            Debugger.Launch();
        }

        ParsedArguments arguments = ParsedArguments.Parse(args);

        if (arguments.ToolName == null || arguments.ShowHelp)
        {
            return PrintUsage();
        }

        if (!TryInstallTool(arguments, out int exitCode, out FileInfo? toolExecutable))
        {
            return exitCode;
        }

        return RunTool(toolExecutable, arguments);
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
  -s, --silent                 Suppress all output while installing the tool.
  -?, -h, --help               Show command line help.");

        return 0;
    }

    private static int RunTool(FileInfo toolExecutable, ParsedArguments arguments)
    {
        using Process process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = toolExecutable.FullName,
                Arguments = string.Join(" ", arguments.Args2),
            },
        };

        process.StartInfo.EnvironmentVariables["DOTNET_ROLL_FORWARD"] = "LatestMajor";

        process.Start();

        process.WaitForExit();

        return process.ExitCode;
    }

    private static bool TryInstallTool(ParsedArguments arguments, out int exitCode, [NotNullWhen(true)] out FileInfo? toolExecutable)
    {
        if (arguments.ToolName == null)
        {
            throw new ArgumentNullException(nameof(arguments.ToolName));
        }

        toolExecutable = null;

        if (!ToolInstaller.TryStartAndWaitForExit(arguments.ToolName, arguments.Silent, arguments.Args1, out exitCode, out DirectoryInfo? toolPath))
        {
            return false;
        }

        toolExecutable = toolPath.EnumerateFiles().First();

        return true;
    }
}
