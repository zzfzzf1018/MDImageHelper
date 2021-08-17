using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MDImageHelper.Internal
{
    public class Core
    {
        private const string MD_SEARCH = "*.md";
        private const string IMG_TAG_REGEX = @"!\[([^]]*)]\(((?:(http[s]?://|ftp://)|[\\\./]*)(?:(?:[^\\/\n]+[\\/])*)([^\\/\n]+\.[a-zA-Z]{1,5}))\)";
        private const string HTTP_IMG_REGEX = @"^!\[([^]]*)]\((?<imgurl>[\w\W]*?)\)";
        public void Handle(CliOptions cliOptions)
        {
            if (!Directory.Exists(cliOptions.MDPath))
            {
                throw new DirectoryNotFoundException(cliOptions.MDPath);
            }

            string[] arrMDFilePath = Directory.GetFiles(cliOptions.MDPath, MD_SEARCH, SearchOption.AllDirectories);
            foreach(string mdFilePath in arrMDFilePath)
            {
                using FileStream fileStream = File.Open(mdFilePath, FileMode.Open);
                using StreamReader streamReader = new (fileStream);
                string content = streamReader.ReadToEnd();

                MatchCollection imgMatchCollection = Regex.Matches(content, IMG_TAG_REGEX);
                foreach(Match imgMatch in imgMatchCollection)
                {
                    Match httpMatch = Regex.Match(imgMatch.Value, HTTP_IMG_REGEX);
                    if (!httpMatch.Success)
                    {
                        Console.WriteLine("warning: HTTP_IMG_REGEX not matched-" + imgMatch.Value);
                        continue;
                    }
                    string imageUrl = httpMatch.Groups["imgurl"].Value;
                    if (string.IsNullOrWhiteSpace(imageUrl))
                    {
                        Console.WriteLine("warning: imageUrl is empty");
                        continue;
                    }

                    using HttpClient httpClient = new ();
                    try
                    {
                        byte[] resBytes = httpClient.GetByteArrayAsync(imageUrl).GetAwaiter().GetResult();
                        File.WriteAllBytes(@"C:\angular_dev\test" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".gif", resBytes);
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
        }
    }
}
