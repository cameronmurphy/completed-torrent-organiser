using Camurphy.CompletedTorrentOrganiser.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Camurphy.CompletedTorrentOrganiser
{
    class Program
    {
        static void Main(string[] args)
        {
            var completedDownloadsDirectory = Settings.Default.CompletedDownloadsDirectory;

            if (!completedDownloadsDirectory.EndsWith("\\")) {
                completedDownloadsDirectory += "\\";
            }

            var directories = Directory.GetDirectories(completedDownloadsDirectory);
            var directoriesToDelete = new List<string>();

            foreach (string directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                var largestVideoFiles =
                    (from file in directoryInfo.GetFiles()
                     where Settings.Default.SupportedFileExtensions.Contains(file.Extension.ToLower())
                     orderby file.Length descending
                     select file);

                if (largestVideoFiles.Any())
                {
                    var largestVideoFile = largestVideoFiles.First();

                    string destination = completedDownloadsDirectory + directoryInfo.Name + largestVideoFile.Extension.ToLower();
                    File.Move(largestVideoFile.FullName, destination);
                    directoriesToDelete.Add(directoryInfo.FullName);
                }
            }

            Thread.Sleep(5000); // Chill out before cleaning up directories
            
            foreach (string directory in directoriesToDelete)
            {
                Directory.Delete(directory, true);
            }
        }
    }
}