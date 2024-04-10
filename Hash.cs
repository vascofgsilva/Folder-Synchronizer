using System.Security.Cryptography;
using System.Text;

namespace FolderSynchronization
{
    internal static class Hash
    {
        /// <summary>
        /// Computes hash of both files and returns true if they have the same hash.
        /// </summary>
        /// <param name="path1">path of first file</param>
        /// <param name="path2">path of second file</param>
        /// <returns></returns>
        public static bool FilesHaveSameHash(string path1, string path2)
        {
            return GetHashFromFile(path1) == GetHashFromFile(path2);
        }

        private static string GetHashFromFile(string path)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    byte[] hashValue = mySHA256.ComputeHash(fileStream);
                    return ByteArrayToString(hashValue);
                }
            }
        }

        private static string ByteArrayToString(byte[] data)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("X2"));
            }
            return builder.ToString();
        }
    }
}
