
using System;
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

            Console.WriteLine("======================================================");
            Console.WriteLine("===================Start==============================");
            Console.WriteLine($"=== -m={cliOptions.MDPath} ===");
            Console.WriteLine($"=== -i={cliOptions.ImageFolder} ===");
            Console.WriteLine($"=== -o={cliOptions.Overwrite} ===");
            Console.WriteLine("======================================================");
            foreach (string mdFilePath in Util.GetMDFiles(cliOptions.MDPath))
            {
                Console.WriteLine($"Begin to process - {mdFilePath}");
                MarkDown md = new(mdFilePath, cliOptions.ImageFolder, cliOptions.Overwrite);
                md.ReplaceRemoteImages();
                Console.WriteLine($"Finish to process - {mdFilePath}");
            }

            Console.WriteLine("======================================================");
            Console.WriteLine("===================Finish=====================");
            Console.WriteLine("======================================================");
        }
    }
}
