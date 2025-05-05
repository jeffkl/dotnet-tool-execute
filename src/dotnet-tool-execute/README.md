# dotnet-tool-execute (dtx)
[![NuGet package dotnet-tool-execute (with prereleases)](https://img.shields.io/nuget/vpre/dotnet-tool-execute?label=dotnet-tool-execute)](https://nuget.org/packages/dotnet-tool-execute)


A .NET tool which installs and executes .NET tools.

## Getting Started
To install the `dtx` tool, run the following command:
```
dotnet tool install --global dotnet-tool-execute --prelease --add-source https://api.nuget.org/v3/index.json --ignore-failed-sources
```

This will install `dtx` globally and add it to your `%PATH%`.

## MCP Servers
You can use the `dtx` tool to run MCP servers that are available as .NET tools. 
```json
// settings.json
{
  "mcp": {
    "servers": {
      "my-mcp-server": {
        "type": "stdio",
        "command": "dtx",
        "args": [ "packageId", "--", "arg1", "arg2" ]
      }
    }
  }
}

```

## Usage
```
dtx <packageId> [options] -- [tool arguments]

Arguments:
  <packageId>  The name of the NuGet package of the tool to install and execute.

Options:
  --version <VERSION>          The version of the tool package to install.
  --configfile <FILE>          The NuGet configuration file to use.
  --tool-manifest <PATH>       Path to the manifest file.
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
  -?, -h, --help               Show command line help.
```

To specify command-line arguments for the tool being executed, use `--` followed by the arguments. For example:
```
> dtx botsay -- Hello from botsay!
You can invoke the tool using the following command: botsay
Tool 'botsay' (version '1.0.0') was successfully installed.

        Hello from botsay!
    __________________
                      \
                       \
                          ....
                          ....'
                           ....
                        ..........
                    .............'..'..
                 ................'..'.....
               .......'..........'..'..'....
              ........'..........'..'..'.....
             .'....'..'..........'..'.......'.
             .'..................'...   ......
             .  ......'.........         .....
             .    _            __        ......
            ..    #            ##        ......
           ....       .                 .......
           ......  .......          ............
            ................  ......................
            ........................'................
           ......................'..'......    .......
        .........................'..'.....       .......
     ........    ..'.............'..'....      ..........
   ..'..'...      ...............'.......      ..........
  ...'......     ...... ..........  ......         .......
 ...........   .......              ........        ......
.......        '...'.'.              '.'.'.'         ....
.......       .....'..               ..'.....
   ..       ..........               ..'........
          ............               ..............
         .............               '..............
        ...........'..              .'.'............
       ...............              .'.'.............
      .............'..               ..'..'...........
      ...............                 .'..............
       .........                        ..............
        .....
```

Tools are installed to `%LocalAppData%\dtx` and then executed.