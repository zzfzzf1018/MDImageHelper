using CommandLine;

namespace MDImageHelper.Internal
{
    public class CliOptions
    {
        [Option('i', "image", Required = false, HelpText = "Set download image folder.")]
        public string ImageFolder { get; set; }

        [Option('m', "markdown", Required = true, HelpText = "Set markdown image folder.")]
        public string MDPath { get; set; }

        [Option('o', "overwrite", Required = false, HelpText = "Overwrite md file.", Default = false)]
        public bool Overwrite { get; set; }
    }

}
