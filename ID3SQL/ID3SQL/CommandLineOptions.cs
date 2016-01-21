using System;
using System.Text;
using System.Collections.Generic;

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

        [Option('f', "fileRegex", HelpText = ".NET-style Regex to determine whether file is to be evaluated. Evaluates the whole file path. Defaults to /.*\\.(wma|mp3|m4a)/")]
        public string FileRegex { get; set; }

        [Option('d', "directory", HelpText = "Directory to recursively search for tag files. Defaults to current users \"My Music\" directory")]
        public string Directory { get; set; }

        [Option('v', "verbose", HelpText = "Prints more information to STDOUT as the process runs", DefaultValue = false)]
        public bool Verbose { get; set; }

        [Option('r', "recycle", HelpText = "When running DELETE statements, determine whether to fully delete files or just put them in Windows Recycle Bin", DefaultValue = false)]
        public bool Recycle { get; set; }

        [Option('y', "dryRun", HelpText = "When running UPDATE or DELETE statements, flags whether to actually make changes to the files. If true, no changes will be made, but they will log via the verbose (-v, --verbose) flag as if they were actually updated/deleted", DefaultValue = false)]
        public bool DryRun { get; set; }

        [Option('s', "stringLiteralSeparator", HelpText = "When comparing a string against a string array, sets the delimiter to use to split the string", DefaultValue = ';')]
        public char StringLiteralSeparator { get; set; }

        [Option('l', "ignoreCaseRegex", HelpText = "Sets whether the where-clause \"LIKE\" regex should ignore case", DefaultValue = true)]
        public bool RegexIgnoreCase { get; set; }

        [Option('c', "--columnNames", HelpText = "When running SELECT statements, flags whether to print the column names as the first line", DefaultValue = true)]
        public bool ColumnNames { get; set; }

        [Option('l', "--columnSeparator", HelpText = "When running SELECT statements, the string given here will be the glue between each column. Usually a variation of the pipe-character", DefaultValue = "\t|\t")]
        public string ColumnSeparator { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current)));

            sb.AppendLine();
            sb.AppendLine("Available Getters");
            sb.AppendLine();

            IEnumerable<string> getterPropertyNames = TagFunctionManager.AllGetFunctionPropertyNames();
            foreach(string getterPropertyName in getterPropertyNames)
            {
                sb.AppendLine(getterPropertyName);
            }

            sb.AppendLine();
            sb.AppendLine("Available Setters");
            sb.AppendLine();

            IEnumerable<string> setterPropertyNames = TagFunctionManager.AllSetFunctionPropertyNames();
            foreach(string setterPropertyName in setterPropertyNames)
            {
                sb.AppendLine(setterPropertyName);
            }

            sb.AppendLine();
            sb.AppendLine("Getter and Setter Property names are case-sensitive");

            return sb.ToString();
        }
    }
}
