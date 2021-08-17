using CommandLine;
using MDImageHelper.Internal;
using System.Threading.Tasks;

namespace MDImageHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<CliOptions>(args)
                   .WithParsed(o =>
                   {
                       var core = new Core();
                       core.Handle(o);
                   });
        }
    }
}
