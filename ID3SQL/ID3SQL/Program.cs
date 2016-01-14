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
#if DEBUG
            return @"C:\Temp\";
#else
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
#endif
        }

        public static void Main(string[] args)
        {
            CommandLineOptions options = new CommandLineOptions();

            try
            {
                if (CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    Regex fileRegex;
                    try
                    {
                        fileRegex = new Regex(options.FileRegex ?? DefaultFileRegex());
                    }
                    catch(Exception ex)
                    {
                        throw new ID3SQLException("Error parsing fileRegex", ex);
                    }

                    string startDirectory = options.Directory ?? DefaultDirectoryPath();

                    string statement = options.Statement;

                    Action<IEnumerable<string>, ExecutionPlanOptions> executionPlan = ExecutionPlan.GenerateExecutionPlan(statement);

                    ICollection<string> tagFilePaths = new List<string>();
                    try
                    {
                        BuildFileList(startDirectory, tagFilePaths, fileRegex);
                    }
                    catch(Exception ex)
                    {
                        throw new ID3SQLException(string.Format("Error building file list from startDirectory '{0}'", startDirectory), ex);
                    }

                    ExecutionPlanOptions executionPlanOptions = new ExecutionPlanOptions()
                    {
                        Recycle = options.Recycle,
                        DryRun = options.DryRun,
                        Verbose = options.Verbose
                    };

                    executionPlan.Invoke(tagFilePaths, executionPlanOptions);
                }
                else
                {
                    Console.Write(options.GetUsage());
                }
            }
            catch(ID3SQLException ex)
            {
                Console.WriteLine(ex.Message);
                if(options.Verbose)
                {
                    Console.Write(ex);
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
