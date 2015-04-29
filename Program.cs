using Camurphy.CompletedTorrentOrganiser.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Camurphy.CompletedTorrentOrganiser
{
    class Program
    {
        static void Main(string[] args)
        {
            string completedDownloadsDirectory = Settings.Default.CompletedDownloadsDirectory;

            if (!completedDownloadsDirectory.EndsWith("\\")) {
                completedDownloadsDirectory += "\\";
            }

            string[] directories = Directory.GetDirectories(completedDownloadsDirectory);
            List<string> directoriesToDelete = new List<string>();

            foreach (string directory in directories)
            {
                string directoryName = Path.GetFileName(directory);
                string[] files = Directory.GetFiles(directory);

                foreach (string file in files.Where(f => Settings.Default.SupportedFileExtensions.Contains(Path.GetExtension(f.ToLower()))))
                {
                    string destination = completedDownloadsDirectory + directoryName + Path.GetExtension(file.ToLower());
                    File.Move(file, destination);
                    break;
                }

                directoriesToDelete.Add(directory);
            }

            Thread.Sleep(5000); // Chill out before cleaning up directories
            
            foreach (string directory in directoriesToDelete)
            {
                Directory.Delete(directory, true);
            }
        }
    }
}