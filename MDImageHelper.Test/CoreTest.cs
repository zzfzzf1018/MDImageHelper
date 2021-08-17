using MDImageHelper.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MDImageHelper.Test
{
    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void TestHandle()
        {
            Core core = new();
            core.Handle(new CliOptions
            {
                MDPath = @"C:\zzf_dev\aaa",
                ImageFolder = "",
                Overwrite = false
            });
        }
    }
}
