using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

#if NET461

using Microsoft.AspNetCore.Hosting.WindowsServices;
using System.Diagnostics;
using System.Linq;

#endif

namespace TodoApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var pathToContentRoot = Directory.GetCurrentDirectory();
#if NET461
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                pathToContentRoot = Path.GetDirectoryName(pathToExe);
            }
#endif

            var host = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(pathToContentRoot)
                .UseStartup<Startup>()
                .Build();

#if NET461
            if (isService)
            {
                host.RunAsService();
                return;
            }
#endif
            host.Run();
        }
    }
}