using System.Collections.Generic;

namespace DotnetToolExecute
{
    internal sealed class ParsedArguments
    {
        private ParsedArguments()
        {
        }

        public List<string> Args1 { get; } = [];

        public List<string> Args2 { get; } = [];

        public bool ShowHelp { get; set; }

        public bool Silent { get; set; }

        public string? ToolName { get; set; }

        public static ParsedArguments Parse(string[] args)
        {
            ParsedArguments parsedArgs = new();

            bool firstArgs = true;

            for (int i = 0; i < args.Length; i++)
            {
                string? arg = args[i];

                if (i == 0)
                {
                    parsedArgs.ToolName = arg;
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
                        case "-s":
                        case "--silent":
                            parsedArgs.Silent = true;
                            continue;

                        case "--global":
                        case "-g":
                        case "--local":
                            continue;

                        case "--help":
                        case "-h":
                        case "-?":
                            parsedArgs.ShowHelp = true;
                            continue;

                        case "--tool-path":
                        case "--tool-manifest":
                        case "--create-manifest-if-needed":
                            i++;
                            continue;
                        default:
                            parsedArgs.Args1.Add(arg);
                            break;
                    }
                }
                else
                {
                    parsedArgs.Args2.Add(arg);
                }
            }

            return parsedArgs;
        }
    }
}
