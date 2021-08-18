using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace MDImageHelper.Internal
{
    public class MarkDown
    {
        private const string IMG_TAG_REGEX = @"!\[([^]]*)]\(((?:(http[s]?://|ftp://)|[\\\./]*)(?:(?:[^\\/\n]+[\\/])*)([^\\/\n]+\.[a-zA-Z]{1,5}))\)";
        private const string HTTP_IMG_REGEX = @"^!\[([^]]*)]\((?<imgurl>[\w\W]*?)\)";
        private const string RELATIVE_IMAGE_FOLDER = "image";
        private HttpClient _httpClient;
        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }

                return _httpClient;
            }
        }

        public MarkDown(string path,string imageFolder, bool overwrite = false)
        {
            MDPath = path;
            Overwrite = overwrite;
            if (string.IsNullOrWhiteSpace(imageFolder))
                DowloadImageFolder = RELATIVE_IMAGE_FOLDER;
            else
                DowloadImageFolder = imageFolder;
            IsRelativeDownloadImageFolder = !Path.GetFullPath(DowloadImageFolder).Equals(DowloadImageFolder);
        }

        ~MarkDown()
        {
            if (_httpClient != null)
            {
                _httpClient.Dispose();
            }
        }

        public string MDPath { get; private set; }
        public bool Overwrite { get; private set; }
        public string DowloadImageFolder { get; private set; }
        public bool IsRelativeDownloadImageFolder { get; private set; }

        public void ReplaceRemoteImages()
        {
            string content = Read();

            MatchCollection imgMatchCollection = Regex.Matches(content, IMG_TAG_REGEX);
            NameValueCollection mappingRemoteLocal = new();
            foreach (Match imgMatch in imgMatchCollection)
            {
                Match httpMatch = Regex.Match(imgMatch.Value, HTTP_IMG_REGEX);
                if (!httpMatch.Success)
                {
                    Console.WriteLine("warning: HTTP_IMG_REGEX not matched-" + imgMatch.Value + " - " + MDPath);
                    continue;
                }
                string imageUrl = httpMatch.Groups["imgurl"].Value;
                if (string.IsNullOrWhiteSpace(imageUrl))
                {
                    Console.WriteLine("warning: imageUrl is empty -" + MDPath);
                    continue;
                }

                if (!DownloadRemoteImage(imageUrl, out string loalImagePath))
                    continue;

                mappingRemoteLocal[imageUrl] = loalImagePath;
            }

            UpdateToFile(content, mappingRemoteLocal);
        }

        private string Read()
        {
            using FileStream fileStream = File.Open(MDPath, FileMode.Open);
            using StreamReader streamReader = new(fileStream);
            string content = streamReader.ReadToEnd();
            return content;
        }

        private bool DownloadRemoteImage(string imageUrl, out string loadImagePath)
        {
            loadImagePath = string.Empty;
            try
            {
                byte[] resBytes = HttpClient.GetByteArrayAsync(imageUrl).GetAwaiter().GetResult();
                string ext = Path.GetExtension(imageUrl);
                string newImagename = string.Format($"{DateTime.Now:yyyy_MM_dd_hh_mm_ss}{(string.IsNullOrWhiteSpace(ext) ? ".png" : ext)}");
                string absoluteDownloadFolder;
                if (!IsRelativeDownloadImageFolder)
                {
                    absoluteDownloadFolder = DowloadImageFolder;
                }
                else
                {
                    string dirName = Path.GetDirectoryName(MDPath);
                    absoluteDownloadFolder = Path.Combine(dirName, DowloadImageFolder);
                }

                if (!Directory.Exists(absoluteDownloadFolder))
                {
                    Directory.CreateDirectory(absoluteDownloadFolder);
                }

                loadImagePath = Path.Combine(absoluteDownloadFolder, newImagename);
                File.WriteAllBytes(loadImagePath, resBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine("warning: download exception -" + MDPath + "-" + ex.Message + ex.StackTrace);
                return false;
            }

            return true;
        }
    
        private void UpdateToFile(string content, NameValueCollection mappingRemoteLocal)
        {
            string replacedContent = content;
            foreach (string remotePath in mappingRemoteLocal.AllKeys)
            {
                string localPath = mappingRemoteLocal[remotePath];
                string relativePath = Path.GetRelativePath(Path.GetDirectoryName(MDPath), localPath);
                relativePath = relativePath.Replace('\\', '/');
                replacedContent = Regex.Replace(replacedContent, remotePath, relativePath);
                //replacedContent = replacedContent.Replace(remotePath, localPath);
            }

            var mdPath = MDPath;
            if(!Overwrite)
            {
                mdPath = Util.GetBackupPath(MDPath);
                if(File.Exists(mdPath))
                {
                    mdPath = Util.AddTimeStampFileName(mdPath);
                }
            }

            using StreamWriter sw = new(mdPath);
            sw.Write(replacedContent);
            sw.Flush();
        }
    }
}
