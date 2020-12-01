using System.Collections.Generic;
using System.IO;

namespace PyExecutor.Utilities
{
    public class FileExtensions
    {
        public static string[] GetFilesFrom(string searchFolder, string[] filters, bool isRecursive)
        {
            List<string> filesFound = new List<string>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (string filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, string.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }

        public static byte[] GetFileBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}