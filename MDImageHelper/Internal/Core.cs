
using System.IO;

namespace MDImageHelper.Internal
{
    public class Core
    {
        public static void Process(CliOptions cliOptions)
        {
            if (!Directory.Exists(cliOptions.MDPath))
            {
                throw new DirectoryNotFoundException(cliOptions.MDPath);
            }

            foreach(string mdFilePath in Util.GetMDFiles(cliOptions.MDPath))
            {
                MarkDown md = new(mdFilePath, cliOptions.ImageFolder, cliOptions.Overwrite);
                md.ReplaceRemoteImages();
            }
        }
    }
}
