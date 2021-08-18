
using System;
using System.IO;

namespace MDImageHelper.Internal
{
    public class Util
    {
        private const string MD_EXT = ".md";
        private const string LOCAL = "local";
        private const string MD_SEARCH_PATTERN = "*.md";

        public static string GetBackupPath(string path)
        {
            var dirName = Path.GetDirectoryName(path);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);
            var newFileName = string.Format($"{fileNameWithoutExt}_{LOCAL}{MD_EXT}");
            return Path.Combine(dirName, newFileName);
        }

        public static string[] GetMDFiles(string path)
        {
            return Directory.GetFiles(path, MD_SEARCH_PATTERN, SearchOption.AllDirectories);
        }

        public static string AddTimeStampFileName(string filePath)
        {
            var dirName = Path.GetDirectoryName(filePath);
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
            var newFileName = string.Format($"{fileNameWithoutExt}_{DateTime.Now:yyyyMMddHHmmssfff}{MD_EXT}");
            return Path.Combine(Path.GetDirectoryName(filePath), newFileName);
        }
    }
}
