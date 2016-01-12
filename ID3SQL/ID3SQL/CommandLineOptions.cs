using System;

using CommandLine;
using CommandLine.Text;

namespace ID3SQL
{
    public class CommandLineOptions
    {
        [ParserState]
        public IParserState LastParserState { get; set; }

        [ValueOption(0)]
        public string Statement { get; set; }

        [Option('f', "fileRegex", HelpText = ".NET-style Regex to determine whether file is to be evaluated")]
        public string FileRegex { get; set; }

        [Option('d', "directory", HelpText = "Directory to recursively search for tag files. Defaults to current users \"My Music\" directory")]
        public string Directory { get; set; }

        [Option('v', "verbose", HelpText = "Enable for debugging. Prints more information to STDOUT as the process runs", DefaultValue = false)]
        public bool Verbose { get; set; }

        [Option('r', "recycle", HelpText = "When running DELETE statements, determine whether to ", DefaultValue = false)]
        public bool Recycle { get; set; }

        [Option('y', "dryRun", HelpText = "When running UPDATE or DELETE statements, flags whether to actually make changes to the files. If true, no changes will be made, but they will log via the verbose (-v, --verbose) flag as if they were actually updated/deleted", DefaultValue = false)]
        public bool DryRun { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
