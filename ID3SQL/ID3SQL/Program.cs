using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using TagLib;

using Irony.Parsing;

namespace ID3SQL
{
    public class Program
    {
        private static string DefaultFileRegex()
        {
            return @".*\.(wma|mp3|m4a)";
        }

        private static string DefaultDirectoryPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }

        public static void Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();

            try
            {
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    Regex fileRegex = new Regex(options.FileRegex ?? DefaultFileRegex());
                    string startDirectory = options.Directory ?? DefaultDirectoryPath();

                    string statement = options.Statement;

                    Grammar grammar = new ID3SQLGrammar();
                    LanguageData languageData = new LanguageData(grammar);
                    Parser parser = new Parser(languageData);
                    ParseTree parseTree = parser.Parse(statement);
                    ParseTreeNode root = parseTree.Root;

                    ICollection<string> tagFilePaths = new List<string>();
                    BuildFileList(startDirectory, tagFilePaths, fileRegex);

                    foreach (string tagFilePath in tagFilePaths)
                    {
                        using (TagLib.File file = TagLib.File.Create(tagFilePath))
                        {
                            // TODO
                        }
                    }
                }
                else
                {
                    Console.Write(options.GetUsage());
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Unknown error has occured");
                if(options.Verbose)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static void BuildFileList(string directoryPath, ICollection<string> tagFilePaths, Regex fileRegex)
        {
            foreach (string subDirectoryPath in Directory.GetDirectories(directoryPath))
            {
                BuildFileList(subDirectoryPath, tagFilePaths, fileRegex);
            }

            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                if (fileRegex.IsMatch(filePath))
                {
                    tagFilePaths.Add(filePath);
                }
            }
        }
    }
}
