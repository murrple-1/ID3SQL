using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using TagLib;

namespace ID3SQL
{
    public class Program
    {
        private static Regex musicFileRegex;

        public static void Main(string[] args)
        {
            musicFileRegex = new Regex(@".*\.(wma|mp3|m4a)");

            ICollection<string> musicFiles = new List<string>();

            DirectorySearch(@"D:\Users\Roadrunner\Music\", musicFiles);

            foreach (string musicFile in musicFiles)
            {
                try
                {
                    using (TagLib.File file = TagLib.File.Create(musicFile))
                    {
                        if (file.Tag.Disc < 1 || file.Tag.DiscCount < 1)
                        {
                            file.Tag.Disc = 1;
                            file.Tag.DiscCount = 1;
                            file.Save();

                            System.Console.WriteLine("{0} has been updated", musicFile);
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("{0} could not be written to. Error: {1}", musicFile, e);
                }
            }
        }

        private static void DirectorySearch(string directoryPath, ICollection<string> musicFiles)
        {
            foreach (string subDirectory in Directory.GetDirectories(directoryPath))
            {
                DirectorySearch(subDirectory, musicFiles);
            }

            foreach (string file in Directory.GetFiles(directoryPath))
            {
                if (musicFileRegex.IsMatch(file))
                {
                    musicFiles.Add(file);
                }
            }
        }
    }
}
