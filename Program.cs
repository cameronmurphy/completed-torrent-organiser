using Camurphy.CompletedTorrentOrganiser.Properties;
using System;
using System.IO;
using System.Linq;

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

            foreach (string directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                var videoFiles =
                    (from file in directoryInfo.GetFiles()
                     where Settings.Default.SupportedFileExtensions.Contains(file.Extension.ToLower())
                     && file.Length / 1024 / 1024 > Settings.Default.MinimumFileSizeMb
                     select file);

                if (videoFiles.Count() == 1)
                {
                    var videoFile = videoFiles.First();
                    string destination = completedDownloadsDirectory + directoryInfo.Name + videoFile.Extension.ToLower();

                    if (!File.Exists(destination))
                    {
                        File.Move(videoFile.FullName, destination);
                    }

                    DeleteDirectory(directoryInfo.FullName);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private static void ResetFileAttributesRecursive(string path)
        {
            foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                ResetFileAttributesRecursive(directory);
            }
        }

        /// <summary>
        /// See http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true#answer-1703799
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteDirectory(string path)
        {
            ResetFileAttributesRecursive(path);

            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                // Give up
            }
            catch (UnauthorizedAccessException)
            {
                // Give up
            }
        }
    }
}