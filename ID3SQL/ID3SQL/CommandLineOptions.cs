using System;

using CommandLine;
using CommandLine.Text;

namespace ID3SQL
{
    public class CommandLineOptions
    {
        [ValueOption(0)]
        public string Statement { get; set; }

        [Option('f', "fileRegex", HelpText = "")]
        public string FileRegex { get; set; }

        [Option('d', "directory", HelpText = "")]
        public string Directory { get; set; }

        [Option('v', "verbose", HelpText = "")]
        public bool Verbose { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,(HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
