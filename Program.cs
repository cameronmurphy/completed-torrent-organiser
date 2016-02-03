using Camurphy.CompletedTorrentOrganiser.Properties;
using System;
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

            foreach (string directory in directories)
            {
                var directoryInfo = new DirectoryInfo(directory);
                var videoFilesLargestToSmallest =
                    (from file in directoryInfo.GetFiles()
                     where Settings.Default.SupportedFileExtensions.Contains(file.Extension.ToLower())
                     orderby file.Length descending
                     select file);

                if (videoFilesLargestToSmallest.Any())
                {
                    var largestVideoFile = videoFilesLargestToSmallest.First();
                    string destination = completedDownloadsDirectory + directoryInfo.Name + largestVideoFile.Extension.ToLower();

                    if (!File.Exists(destination))
                    {
                        File.Move(largestVideoFile.FullName, destination);
                    }
                    
                    // Clean up other video files, just in case directory cleanup fails
                    foreach (var videoFile in videoFilesLargestToSmallest)
                    {
                        if (videoFile != largestVideoFile)
                        {
                            File.Delete(videoFile.FullName);
                        }
                    }

                    DeleteDirectory(directoryInfo.FullName);
                }
            }
        }

        /// <summary>
        /// See http://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true#answer-1703799
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteDirectory(string path)
        {
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
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }
    }
}