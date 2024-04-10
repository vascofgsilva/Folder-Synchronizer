using System.IO;

namespace FolderSynchronization
{
    internal static class Synchronization
    {
        /// <summary>
        /// Validates source and replica paths.
        /// Creates replica folder if it doesn't exist.
        /// </summary>
        /// <param name="source">The path to the source folder.</param>
        /// <param name="replica">The path to the replica folder.</param>
        /// <exception cref="DirectoryNotFoundException">Throw when source folder not found</exception>
        /// <exception cref="ArgumentException">Throw when argument for replica folder is null or whitespace</exception>
        public static void ValidatePaths(string source, string replica, string logPath)
        {
            if (!Directory.Exists(source)) throw new DirectoryNotFoundException($"{source} directory not found.");
            if (string.IsNullOrWhiteSpace(replica)) throw new ArgumentException("Unspecified replica path.");
            if (string.IsNullOrWhiteSpace(logPath)) throw new ArgumentException("Unspecified log path.");
            if (!Directory.Exists(replica)) Directory.CreateDirectory(replica);
        }

        /// <summary>
        /// Main sync method.
        /// </summary>
        /// <param name="source">Path to source folder</param>
        /// <param name="replica">Path to replica folder</param>
        /// <param name="logPath">Path to log file</param>
        public static void SyncFolder(string source, string replica, string logPath)
        {
            ValidatePaths(source, replica, logPath);
            LogMessage("Synching process started...", logPath);
            DeleteFromFolder(replica, source, replica, logPath);
            CopyToFolder(source, source, replica, logPath);
            LogMessage("Synching process ended...", logPath);
        }

        /// <summary>
        /// Iterates through replica folder structure deleting any folder and file not found in the source folder.
        /// </summary>
        /// <param name="iterableFolder">Folder to iterate.</param>
        /// <param name="sourceFolder">Path to the source folder.</param>
        /// <param name="replicaFolder">Path to the replica folder.</param>
        private static void DeleteFromFolder(string iterableFolder, string sourceFolder, string replicaFolder, string logPath)
        {
            try
            {
                var files = Directory.EnumerateFiles(iterableFolder);
                foreach (var file in files)
                {
                    var sourceFileMirrorPath = GetMirrorPath(file, replicaFolder, sourceFolder);
                    if (!File.Exists(sourceFileMirrorPath))
                    {
                        File.Delete(file);
                        LogMessage($"Deleted {file};", logPath);
                    }
                }

                var directories = Directory.EnumerateDirectories(iterableFolder);
                foreach (var directory in directories)
                {
                    var sourceFolderMirrorPath = GetMirrorPath(directory, replicaFolder, sourceFolder);
                    if (!Directory.Exists(sourceFolderMirrorPath))
                    {
                        Directory.Delete(directory, true);
                        LogMessage($"Deleted {directory};", logPath);
                    }
                    else
                    {                        
                        DeleteFromFolder(directory, sourceFolder, replicaFolder, logPath);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
        }

        private static void CopyToFolder(string iterableFolder, string sourceFolder, string replicaFolder, string logPath)
        {
            try
            {
                var files = Directory.EnumerateFiles(iterableFolder);
                foreach (var file in files)
                {
                    var fileReplicaMirrorPath = GetMirrorPath(file, sourceFolder, replicaFolder);
                    if (!File.Exists(fileReplicaMirrorPath))
                    {
                        File.Copy(file, fileReplicaMirrorPath);
                        LogMessage($"Copied {file} to {fileReplicaMirrorPath};", logPath);
                    }
                    else
                    {
                        if (!Hash.FilesHaveSameHash(file, fileReplicaMirrorPath)) {
                            File.Copy(file, fileReplicaMirrorPath, true);
                            LogMessage($"Copied {file} to {fileReplicaMirrorPath} replacing existing file;", logPath);
                        }
                    }
                }

                var directories = Directory.EnumerateDirectories(iterableFolder);
                foreach (var directory in directories)
                {
                    var folderReplicaMirrorPath = GetMirrorPath(directory, sourceFolder, replicaFolder);
                    if (!Directory.Exists(folderReplicaMirrorPath))
                    {
                        Directory.CreateDirectory(folderReplicaMirrorPath);
                        LogMessage($"Created {folderReplicaMirrorPath};", logPath);
                    }
                    CopyToFolder(directory, sourceFolder, replicaFolder, logPath);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
        }

        #region helpers
        private static string GetMirrorPath(string path, string relativePath, string combineToPath)
        {
            return Path.Combine(combineToPath, Path.GetRelativePath(relativePath, path));
        }

        private static void LogMessage(string message, string logPath)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            Logger logger = new(logPath);
            logger.Log($"{DateTime.Now} - {message}");
            Console.WriteLine(message);
        }
        #endregion
    }
}