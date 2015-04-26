using Camurphy.CompletedTorrentOrganiser.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            string[] paths = Directory.GetDirectories(completedDownloadsDirectory);

            foreach (string path in paths)
            {
                string directoryName = Path.GetFileName(path);

                string[] files = Directory.GetFiles(path);

                foreach (string file in files.Where(f => Settings.Default.SupportedFileExtensions.Contains(Path.GetExtension(f.ToLower()))))
                {
                    string destination = completedDownloadsDirectory + directoryName + Path.GetExtension(file.ToLower());
                    File.Move(file, destination);
                    break;
                }

                Directory.Delete(path, true);
            }
        }
    }
}